using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using Gat.Web.Common;
using Gat.Web.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Gat.Web.report.columbia
{
    public partial class rpt_transfer : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
        protected string shopCode = string.Empty;
        private string id = string.Empty;
        protected int SumQty { get; set; }
        protected decimal SumAmount { get; set; }

        public string CompanyID
        {
            get { return (ViewState["CompanyID"] ?? "").ToString(); }
            set { ViewState["CompanyID"] = value; }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                id = Request.QueryString["id"];
                this.TopTitle = "哥伦比亚报表";
                this.SubTitle = "中转数据";
                this.LoadSolution();

            }
            this.btnImport.Click += new EventHandler(btnImport_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.DeleteBut.Click += new EventHandler(DeleteBut_Click);
            this.SaveBut.Click += new EventHandler(SaveBut_Click);

            var s = this.CompanyID;
            DeleteBut.Attributes.Add("onclick ", "return confirm( '你确认要全部删除吗？');");
            #region AJAX 请求
            string mode = Request["mode"] ?? "";
            if (this.IsAJAXRequest || !string.IsNullOrEmpty(mode))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (mode.ToUpper())
                {
                    case "TRANSFER_READER":
                        LoadTempData(Request.Form["page"], Request["pageSize"], Request["shopName"], Request["strat"], Request["end"]);
                        break;
                    case "TRANSFER_UPDATE":
                        UpdateData(Request["list"]);
                        break;
                    case "TRANSFER_DELETE":
                        DeleteData(Request["list"]);
                        break;
                    case "TRANSFER_SAVE":
                        SaveAsToPureData(Request["selectedIds"]);
                        break;
                    case "EXPORTDATA":
                        byte[] fileArray = null;
                        string fileName = "";
                        returnText = ExportData(ref fileArray, ref fileName);
                        if (returnText == "success")
                        {
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray);
                            this.Response.End();
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(returnText) && returnText != "success")
                {
                    this.Response.ContentType = "application/text";
                    this.Response.Write(returnText);
                    this.Response.End();
                }
            }
            #endregion
        }

        private void SaveAsToPureData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferSaveAs(p);
                if (handle.Status == HandleStatus.Fail)
                {
                    ai.Message = handle.Message;
                    ai.Html = handle.Message;
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
                else
                {
                    //重新加载店铺信息
                    ai.StatusCode = "1";
                    ai.Html = "删除成功";
                    this.ResponseToClient(ai);
                }
            }
        }



        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="str"></param>
        private void UpdateData(string str)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.TrimEnd('|');
                string[] list = str.Split('|');
                if (list != null && list.Length > 0)
                {
                    foreach (var model in list)
                    {
                        var item = model.Split(',');
                        if (item != null && item.Length > 0)
                        {
                            int TransID = 0;
                            int.TryParse(item[0], out TransID);
                            int Qty = 0;
                            int.TryParse(item[1], out Qty);
                            HandleArgs handle = WebRepository.UpdateTransferQty(Qty, TransID);
                            if (handle.Status == HandleStatus.Fail)
                            {
                                ai.Message = handle.Message;
                                ai.Html = handle.Message;
                                ai.StatusCode = "2";
                                this.ResponseToClient(ai);
                                break;
                            }
                        }
                    }
                    ai.StatusCode = "1";
                    ai.Html = "保存成功";
                    this.ResponseToClient(ai);
                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="str"></param>
        private void DeleteData(string str)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.TrimEnd(',');
                HandleArgs handle = WebRepository.DeleteTransfer(str);
                if (handle.Status == HandleStatus.Fail)
                {
                    ai.Message = handle.Message;
                    ai.Html = handle.Message;
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
                //重新加载店铺信息
                ai.StatusCode = "1";
                ai.Html = "删除成功";
                this.ResponseToClient(ai);

            }
        }
        /// <summary>
        /// 删除全部
        /// </summary>
        /// <param name="str"></param>
        private void DeleteData()
        {
            var ai = new AJAXInterop();
            HandleArgs handle = WebRepository.DeleteTransfer();
            if (handle.Status == HandleStatus.Fail)
            {

                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('删除失败')</script>");
                return;
            }

            Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('删除成功')</script>");

        }

        /// <summary>
        /// 加载方案
        /// </summary>
        private void LoadSolution()
        {
            DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            ConnnectType connType = context.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
            using (var gat = new GatRepository(connType))
            {
                //绑定供应商
                ddlSoule.DataSource = gat.QuerySolutionsForVendor(context.VendorID);
                ddlSoule.DataTextField = "Title";
                ddlSoule.DataValueField = "ID";
                ddlSoule.DataBind();
            }
            if (!string.IsNullOrEmpty(id))
            {
                ListItem li = ddlSoule.Items.FindByValue(id);
                if (li != null) li.Selected = true;


            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="pageSize"></param>
        private void LoadTempData(string PageIndex, string pageSize, string shopName, string start_date, string end_date)
        {
            int solutionID = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(id, out solutionID);
            }
            else
            {
                int.TryParse(ddlSoule.SelectedValue, out solutionID);
            }
            ddlSoule.SelectedValue = id;
            ddlSoule.ClearSelection();
            ddlSoule.SelectedItem.Selected = true;

            int currentPageIndex = 1;
            int currentPageSize = 15;
            int.TryParse(PageIndex, out currentPageIndex);
            int.TryParse(pageSize, out currentPageSize);

            var list = WebRepository.GatColumbiaPosSaleTransfeSumCount(solutionID);

            //  Session["SumQty"] = (int)list.Sum(s => s.Qty);
            //Session["SumAmount"] = Math.Round((decimal)list.Sum(s => s.SalesAmount), 2);

            // this.SumQty= (int)list.Sum(s => s.Qty);
            // this.GridView = gvPos;
            var mats = WebRepository.QueryGatColumbiaPosSaleTransfer(solutionID, currentPageIndex, currentPageSize, shopName, start_date, end_date);
            var result = new DataSourceResult();

            result.Data = mats == null ? null : mats;
            result.Total = WebRepository.GatColumbiaPosSaleTransferCount(solutionID);
            result.Errors = "";



            //string s = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            this.ResponseToClient(result);

        }
        /// <summary>
        /// 呈现控件，输出 HTML
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private string RenderControl(Control control)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page page = new Page();
            HtmlForm form = new HtmlForm();
            this.ddlSoule.EnableViewState = false;
            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            form.Controls.Add(this.ddlSoule);
            page.RenderControl(htw);
            return sw.ToString();
        }
        /// <summary>
        /// 导出文件数据格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExport_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("/Files");
            string fileNameStock = "【" + ddlSoule.SelectedItem.Text + "】的提报仓库.xls";
            //   fileNameStock = System.IO.Path.Combine(path, fileNameStock);
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("公司名称");
                dtSourceStock.Columns.Add("店铺代码");
                //dtSourceStock.Columns.Add("店铺名称");
                //dtSourceStock.Columns.Add("仓库名称");
                dtSourceStock.Columns.Add("经营品牌");
                dtSourceStock.Columns.Add("交易编号");
                dtSourceStock.Columns.Add("交易时间");
                dtSourceStock.Columns.Add("产品编号");
                dtSourceStock.Columns.Add("规格");
                dtSourceStock.Columns.Add("产品条码");
                dtSourceStock.Columns.Add("销售金额");
                dtSourceStock.Columns.Add("销售数量");
                dtSourceStock.Columns.Add("销售员编号");
                dtSourceStock.Columns.Add("结算单价");
                dtSourceStock.Columns.Add("备注");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileNameStock);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Warning);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnImport_Click(object sender, EventArgs e)
        {

            string datetimeStr = string.Empty;
            var ai = new AJAXInterop();
            try
            {
                string Extension = Path.GetExtension(FileUpload.PostedFile.FileName);
                if (FileUpload.PostedFile.ContentLength <= 0)
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('内容为空')</script>");
                    return;
                }
                string[] valid = new string[] { ".xls", ".xlsx" };
                if (!valid.Contains(Extension))
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('导入的文件不合法')</script>");
                    return;
                }

                string dir = Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + FileUpload.FileName;
                if (!System.IO.Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                FileUpload.SaveAs(filePath);

                ExcelHelper Excel = new ExcelHelper();
                // DataTable dt1 = Excel.RenderDataTableFromExcels(filePath, "Sheet1", 0);
                DataTable dt1 = Excel.Import(filePath);
                // datetimeStr = dt1.Rows[0]["交易时间"].ToString();
                //  DataSet ds = Excel.ImportExcel(filePath, null, null, true);
                // DataTable dt = ds.Tables[0].Clone();
                DataTable dt = dt1.Clone();
                dt.Columns["公司名称"].ColumnName = "CompanyName";
                dt.Columns["店铺代码"].ColumnName = "ShopCode";
                //dt.Columns["店铺名称"].ColumnName = "ShopName";
                //dt.Columns["仓库名称"].ColumnName = "StockName";
                dt.Columns["经营品牌"].ColumnName = "CardName";
                dt.Columns["交易编号"].ColumnName = "BillNo";
                dt.Columns["交易时间"].DataType = typeof(DateTime);
                dt.Columns["交易时间"].ColumnName = "BillDate";
                dt.Columns["产品编号"].DataType = typeof(string);
                dt.Columns["产品编号"].ColumnName = "MaterialCode";
                dt.Columns["规格"].DataType = typeof(string);
                dt.Columns["规格"].ColumnName = "SizeName";
                dt.Columns["产品条码"].DataType = typeof(string);
                dt.Columns["产品条码"].ColumnName = "BarCode";
                dt.Columns["销售金额"].DataType = typeof(Decimal);
                dt.Columns["销售金额"].ColumnName = "SalesAmount";
                dt.Columns["销售数量"].DataType = typeof(Decimal);
                dt.Columns["销售数量"].ColumnName = "Qty";
                dt.Columns["销售员编号"].ColumnName = "PersonnelID";
                dt.Columns["结算单价"].DataType = typeof(Decimal);
                dt.Columns["结算单价"].ColumnName = "BalaPrice";
                dt.Columns["备注"].DataType = typeof(string);
                dt.Columns["备注"].ColumnName = "Remark";
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    dt.Rows.Add(dt1.Rows[i].ItemArray);
                }
                //  dt.Columns["交易时间"].DataType = typeof(DateTime);
                HandleArgs args = WebRepository.InsertTransferQty(dt, ddlSoule.SelectedValue, this.CompanyID);
                if (args.Status == HandleStatus.Completed)
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "", string.Format("<script>alert('导入成功！,共{0}条')</script>", dt1.Rows.Count));
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "", string.Format("<script>alert('导入失败{0}')</script>", args.Message));
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", string.Format("<script>alert('导入失败:{0}')</script>", ex.Message + datetimeStr));
            }

        }


        void DeleteBut_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        void SaveBut_Click(object sender, EventArgs e)
        {

            HandleArgs handle = WebRepository.TransferSaveAs("C");
            if (handle.Status == HandleStatus.Fail)
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", string.Format("<script>alert('保存失败:{0}')</script>", handle.Message));
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", string.Format("<script>alert('保存成功')</script>"));

            }

        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string ExportData(ref byte[] fileArry, ref string fileNameStock)
        {
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
            var solutionIdReq = Request["solutionId"];
            var shopName = Request["shopName"];
            string path = Server.MapPath("/Files");
            fileNameStock = "提报.xls";
            int solutionId = 0;
            if (!string.IsNullOrWhiteSpace(solutionIdReq))
                Int32.TryParse(solutionIdReq, out solutionId);
            var data = WebRepository.QueryGatColumbiaPosSaleTransferData(solutionId, shopName, startDate, endDate);
            System.Data.DataTable dtSourceStock = data.ToDataTable(new string[] { 
                "CompanyName", "ShopCode", "CardName", "BillNo", "BillDate", "MaterialCode", "SizeName", "BarCode",
                "SalesAmount", "Qty", "PersonnelID", "Remark" });

            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["CardName"].ColumnName = "经营品牌";
                dtSourceStock.Columns["BillNo"].ColumnName = "交易编号";
                dtSourceStock.Columns["BillDate"].ColumnName = "交易时间";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "产品编号";
                dtSourceStock.Columns["SizeName"].ColumnName = "规格";
                dtSourceStock.Columns["BarCode"].ColumnName = "产品条码";
                dtSourceStock.Columns["SalesAmount"].ColumnName = "销售金额";
                dtSourceStock.Columns["Qty"].ColumnName = "销售数量";
                dtSourceStock.Columns["PersonnelID"].ColumnName = "销售员编号";
                dtSourceStock.Columns["Remark"].ColumnName = "备注";
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
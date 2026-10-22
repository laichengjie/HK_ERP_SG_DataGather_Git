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
using GatNewBll.Blls;

namespace Gat.Web.report.adidas
{
    public partial class rpt_transfer : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
        protected string shopCode = string.Empty;
        private string id = string.Empty;
        //private decimal Qty = 0;
        //private decimal Amount = 0;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                //var mats = WebRepository.QueryGatTSTotal();
                //Qty = mats[0];
                //Amount = mats[1];
                //this.LabelTotalQty.Text = Qty.ToString();
                //this.LabelTotalAmount.Text = Amount.ToString();
                id = Request.QueryString["id"];
                this.TopTitle = "阿迪达斯报表";
                this.SubTitle = "中转数据";
                this.LoadSolution();

            }
            //this.btnImport.Click += new EventHandler(btnImport_Click);
            //this.btnExport.Click += new EventHandler(btnExport_Click);
            #region AJAX 请求
            string mode = Request["mode"] ?? "";
            if (this.IsAJAXRequest || !string.IsNullOrEmpty(mode))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (mode.ToUpper())
                {
                    case "TRANSFER_READER":
                        LoadTempData(Request.Form["page"], Request["pageSize"]);
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
                    case "TRANSFER_DELSELECT":
                        DelSelectedData(Request["selectedIds"]);
                        break;
                    case "TRANSFER_SAVEALL":
                        SaveAll(Request);
                        break;
                    case "TRANSFER_DELALL":
                        DelALL(Request);
                        break;
                    case "SEARCH":
                        Search(Request);
                        break;
                    case "DELETESEARCH":
                        DeleteSearch(Request);
                        break;
                    case "IMPORT":
                        HttpPostedFile file1 = Context.Request.Files["file"];
                        string fileFullName = FileHelper.uploadFile(file1, "~/FileUpload/");  //这里引用的是上面封装的方法
                        returnText = Import(fileFullName);
                        break;
                    case "EXPORT":
                        byte[] fileArray = null;
                        string fileName = "";
                        returnText = Export(ref fileArray, ref fileName);
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
        private void DelSelectedData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferDelAs(p);
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
        private void SaveAsToPureData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferAdSaveAs(p);
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
                            HandleArgs handle = WebRepository.UpdateAdTransferQty(Qty, TransID);
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
                HandleArgs handle = WebRepository.DeleteAdTransfer(str);
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
        private void LoadTempData(string PageIndex, string pageSize)
        {
            try
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
                if (ddlSoule.SelectedItem != null)
                {
                    ddlSoule.SelectedItem.Selected = true;
                }

                int currentPageIndex = 1;
                int currentPageSize = 15;
                int.TryParse(PageIndex, out currentPageIndex);
                int.TryParse(pageSize, out currentPageSize);

                // this.GridView = gvPos;
                var mats = WebRepository.QueryGatAdPosSaleTransfer(solutionID, currentPageIndex, currentPageSize);
                var result = new DataSourceResult();

                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GatAdPosSaleTransferCount(solutionID);
                result.Errors = "";

                //string s = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                this.ResponseToClient(result);
            }
            catch (Exception)
            {
                throw;
            }

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
                dtSourceStock.Columns.Add("店铺名称");
                dtSourceStock.Columns.Add("仓库名称");
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
                dtSourceStock.Columns.Add("小票号");
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
        /// 导出
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string Export(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "提报.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("公司名称");
                dtSourceStock.Columns.Add("店铺代码");
                dtSourceStock.Columns.Add("店铺名称");
                dtSourceStock.Columns.Add("仓库名称");
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
                dtSourceStock.Columns.Add("小票号");
                dtSourceStock.Columns.Add("备注");
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

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void btnImport_Click(object sender, EventArgs e)
        //{

        //    string datetimeStr = string.Empty;
        //    var ai = new AJAXInterop();
        //    try
        //    {
        //        string Extension = Path.GetExtension(FileUpload.PostedFile.FileName);
        //        if (FileUpload.PostedFile.ContentLength <= 0)
        //        {
        //            Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('内容为空')</script>");
        //            return;
        //        }
        //        string[] valid = new string[] { ".xls", ".xlsx" };
        //        if (!valid.Contains(Extension))
        //        {
        //            Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('导入的文件不合法')</script>");
        //            return;
        //        }

        //        string dir = Server.MapPath("/FileUpload");
        //        string filePath = dir + "\\" + FileUpload.FileName;
        //        if (!System.IO.Directory.Exists(dir))
        //        {
        //            Directory.CreateDirectory(dir);
        //        }
        //        if (File.Exists(filePath))
        //        {
        //            File.Delete(filePath);
        //        }

        //        FileUpload.SaveAs(filePath);

        //        ExcelHelper Excel = new ExcelHelper();
        //        // DataTable dt1 = Excel.RenderDataTableFromExcels(filePath, "Sheet1", 0);
        //        DataTable dt1 = Excel.Import(filePath);
        //        // datetimeStr = dt1.Rows[0]["交易时间"].ToString();
        //        //  DataSet ds = Excel.ImportExcel(filePath, null, null, true);
        //        // DataTable dt = ds.Tables[0].Clone();
        //        DataTable dt = dt1.Clone();
        //        dt.Columns["公司名称"].ColumnName = "CompanyName";
        //        dt.Columns["店铺代码"].ColumnName = "ShopCode";
        //        dt.Columns["店铺名称"].ColumnName = "ShopName";
        //        dt.Columns["仓库名称"].ColumnName = "StockName";
        //        dt.Columns["经营品牌"].ColumnName = "CardName";
        //        dt.Columns["交易编号"].ColumnName = "BillNo";
        //        dt.Columns["交易时间"].DataType = typeof(DateTime);
        //        dt.Columns["交易时间"].ColumnName = "BillDate";
        //        dt.Columns["产品编号"].DataType = typeof(string);
        //        dt.Columns["产品编号"].ColumnName = "MaterialCode";
        //        dt.Columns["规格"].DataType = typeof(string);
        //        dt.Columns["规格"].ColumnName = "SizeName";
        //        dt.Columns["产品条码"].DataType = typeof(string);
        //        dt.Columns["产品条码"].ColumnName = "BarCode";
        //        dt.Columns["销售金额"].DataType = typeof(Decimal);
        //        dt.Columns["销售金额"].ColumnName = "SalesAmount";
        //        dt.Columns["销售数量"].DataType = typeof(Decimal);
        //        dt.Columns["销售数量"].ColumnName = "Qty";
        //        dt.Columns["销售员编号"].ColumnName = "PersonnelID";
        //        dt.Columns["结算单价"].DataType = typeof(Decimal);
        //        dt.Columns["结算单价"].ColumnName = "BalaPrice";
        //        dt.Columns["小票号"].DataType = typeof(string);
        //        dt.Columns["小票号"].ColumnName = "SmallTicketNo";
        //        for (int i = 0; i < dt1.Rows.Count; i++)
        //        {
        //            dt.Rows.Add(dt1.Rows[i].ItemArray);
        //        }
        //        //  dt.Columns["交易时间"].DataType = typeof(DateTime);
        //        HandleArgs args = WebRepository.InsertAdTransferQty(dt, ddlSoule.SelectedValue);
        //        if (args.Status == HandleStatus.Completed)
        //        {
        //            Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入成功{0}')</script>", args.Message + datetimeStr));
        //        }
        //        else
        //        {
        //            Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入失败{0}')</script>", args.Message));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入失败:{0}')</script>", ex.Message + datetimeStr));
        //    }

        //}

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string Import(string fileName)
        {
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                #region old
                //dt.Columns["店铺代码"].ColumnName = "ShopCode";
                //dt.Columns["交易时间"].DataType = typeof(DateTime);
                //dt.Columns["交易时间"].ColumnName = "BillDate";
                //dt.Columns.Add(new DataColumn() { ColumnName = "BillTime", DataType = typeof(DateTime) });
                //dt.Columns["产品编号"].DataType = typeof(string);
                //dt.Columns["产品编号"].ColumnName = "MaterialCode";
                //dt.Columns["销售数量"].DataType = typeof(Decimal);
                //dt.Columns["销售数量"].ColumnName = "Qty";
                //dt.Columns["交易编号"].ColumnName = "BillNo";
                //dt.Columns["销售金额"].DataType = typeof(Decimal);
                //dt.Columns["销售金额"].ColumnName = "SalesAmount";
                //dt.Columns["销售员编号"].ColumnName = "PersonnelID";
                //dt.Columns["产品条码"].DataType = typeof(string);
                //dt.Columns["产品条码"].ColumnName = "BarCode";
                //dt.Columns["规格"].DataType = typeof(string);
                //dt.Columns["规格"].ColumnName = "SizeName";
                //dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
                //dt.Columns["公司名称"].ColumnName = "CompanyName";
                //dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
                //dt.Columns["经营品牌"].ColumnName = "CardName";
                //dt.Columns.Add(new DataColumn() { ColumnName = "SolutionID", DataType = typeof(string) });
                //dt.Columns["结算单价"].DataType = typeof(Decimal);
                //dt.Columns["结算单价"].ColumnName = "BalaPrice";
                //dt.Columns["店铺名称"].ColumnName = "ShopName";
                //dt.Columns["仓库名称"].ColumnName = "StockName";
                //dt.Columns["小票号"].DataType = typeof(string);
                //dt.Columns["小票号"].ColumnName = "SmallTicketNo";
                var solutionId = Request["solutionId"];
                #endregion
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["ShopCode"] = excelDt.Rows[i]["店铺代码"];
                    row["BillDate"] = excelDt.Rows[i]["交易时间"];
                    row["BillTime"] = DateTime.Now;
                    row["MaterialCode"] = excelDt.Rows[i]["产品编号"];
                    row["Qty"] = excelDt.Rows[i]["销售数量"];
                    row["BillNo"] = excelDt.Rows[i]["交易编号"];
                    row["SalesAmount"] = excelDt.Rows[i]["销售金额"];
                    row["PersonnelID"] = excelDt.Rows[i]["销售员编号"];
                    row["BarCode"] = excelDt.Rows[i]["产品条码"];
                    row["SizeName"] = excelDt.Rows[i]["规格"];
                    row["CompanyName"] = excelDt.Rows[i]["公司名称"];
                    row["CardName"] = excelDt.Rows[i]["经营品牌"];
                    row["SolutionID"] = solutionId;
                    row["BalaPrice"] = excelDt.Rows[i]["结算单价"];
                    row["ShopName"] = excelDt.Rows[i]["店铺名称"];
                    row["StockName"] = excelDt.Rows[i]["仓库名称"];
                    row["SmallTicketNo"] = excelDt.Rows[i]["小票号"];
                    if (excelDt.Columns.Contains("备注"))
                    {
                        row["Remark"] = excelDt.Rows[i]["备注"];
                    }
                    else
                    {
                        row["Remark"] = "";
                    }
                    dt.Rows.Add(row);
                }
                //args = WebRepository.InsertAdTransfer(dt, ddlSoule.SelectedValue);
                args = WebRepository.BulkCopyAdTransfer(dt);
                if (args.Status == HandleStatus.Completed)
                {
                    return "导入成功" + args.Message + datetimeStr;
                }
                else
                {
                    return "导入失败：写入数据库异常" + args.Message;
                }
            }
            catch (Exception ex)
            {
                return "导入失败:" + ex.ToString() + datetimeStr;
            }
        }

        private DataTable InitImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "TransID", DataType = typeof(int), AutoIncrement = true });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillDate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillNo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SalesAmount", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PersonnelID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BarCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionID", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BalaPrice", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SmallTicketNo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Remark", DataType = typeof(string) });
            return dt;
        }

        /// <summary>
        /// 全部保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SaveAll(HttpRequest request)
        {
            string strSolutionId = request["solutionId"];
            var result = new AJAXInterop();
            try
            {
                int solutionId = string.IsNullOrEmpty(strSolutionId) ? 0 : Convert.ToInt32(strSolutionId);

                HandleArgs handle = WebRepository.TransferAdSaveAs(solutionId);
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载店铺信息
                    result.StatusCode = "1";
                    result.Html = "保存成功";
                    this.ResponseToClient(result, true);
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = "2";
                result.Message = ex.Message;
                result.Html = ex.Message;
                this.ResponseToClient(result, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        /// <summary>
        /// 全部删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelALL(HttpRequest request)
        {
            string strSolutionId = request["solutionId"];
            var result = new AJAXInterop();
            try
            {
                int solutionId = string.IsNullOrEmpty(strSolutionId) ? 0 : Convert.ToInt32(strSolutionId);
                HandleArgs handle = WebRepository.DeleteAdTransfer(solutionId);
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载店铺信息
                    result.StatusCode = "1";
                    result.Html = "删除成功";
                    this.ResponseToClient(result, true);
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = "2";
                result.Message = ex.Message;
                result.Html = ex.Message;
                this.ResponseToClient(result, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Search(HttpRequest request)
        {

            string strSolutionId = request["solutionId"];
            string strStartBillDate = request["startBillDate"];
            string strEndBillDate = request["endBillDate"];
            string shopCode = request["shopCode"];
            string materialCode = request["materialCode"];
            string sizeCode = request["sizeCode"];
            string strPage = request["page"];
            string strPageSize = request["pageSize"];

            var result = new AJAXInterop();
            try
            {
                int solutionId = string.IsNullOrEmpty(strSolutionId) ? 0 : Convert.ToInt32(strSolutionId);
                //DateTime startBillDate = string.IsNullOrEmpty(strStartBillDate) ? Convert.ToDateTime("1900-01-01 00:00:00") : Convert.ToDateTime(strStartBillDate);
                //DateTime endBillDate = string.IsNullOrEmpty(strEndBillDate) ? DateTime.Now : Convert.ToDateTime(strEndBillDate);
                int page = string.IsNullOrEmpty(strPage) ? 1 : Convert.ToInt32(strPage);
                int pageSize = string.IsNullOrEmpty(strPageSize) ? 100 : Convert.ToInt32(strPageSize);

                //Dictionary<string, object> searchDic = WebRepository.QueryGatAdPosSaleTransfer(solutionId, startBillDate, endBillDate,shopCode, materialCode, sizeCode, page, pageSize);
                AdBll bll = new AdBll();
                var searchDic = bll.SelectPosSaleTransferPageList(page, pageSize, solutionId, strStartBillDate, strEndBillDate, shopCode, materialCode, sizeCode);
                result.StatusCode = "2";
                //var dict = new Dictionary<string, object>();
                //dict.Add("data", searchDic["data"]);
                //dict.Add("totalCount", searchDic["dataCount"]);
                result.list = searchDic;
                //this.LabelTotalQty.Text = searchDic["qty"]?.ToString();
                //this.LabelTotalAmount.Text = searchDic["amount"]?.ToString();
                this.ResponseToClient(result, true);
            }
            catch (Exception ex)
            {
                result.StatusCode = "2";
                result.Message = ex.Message;
                result.Html = ex.Message;
                this.ResponseToClient(result, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        /// <summary>
        /// 删除符合查询条件的数据
        /// </summary>
        /// <param name="request"></param>
        void DeleteSearch(HttpRequest request)
        {

            string strSolutionId = request["solutionId"];
            string strStartBillDate = request["startBillDate"];
            string strEndBillDate = request["endBillDate"];
            string shopCode = request["shopCode"];
            string materialCode = request["materialCode"];
            string sizeCode = request["sizeCode"];
            var result = new AJAXInterop();
            try
            {
                int solutionId = string.IsNullOrEmpty(strSolutionId) ? 0 : Convert.ToInt32(strSolutionId);
                DateTime startBillDate = string.IsNullOrEmpty(strStartBillDate) ? Convert.ToDateTime("1900-01-01 00:00:00") : Convert.ToDateTime(strStartBillDate);
                DateTime endBillDate = string.IsNullOrEmpty(strEndBillDate) ? DateTime.Now : Convert.ToDateTime(strEndBillDate);

                HandleArgs handle = WebRepository.DeleteAdTransfer(solutionId, startBillDate, endBillDate, materialCode, sizeCode, shopCode);
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载店铺信息
                    result.StatusCode = "1";
                    result.Html = "删除成功";
                    this.ResponseToClient(result, true);
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = "2";
                result.Message = ex.Message;
                result.Html = ex.Message;
                this.ResponseToClient(result, true);
            }
            finally
            {
                this.Response.End();
            }
        }
    }
}
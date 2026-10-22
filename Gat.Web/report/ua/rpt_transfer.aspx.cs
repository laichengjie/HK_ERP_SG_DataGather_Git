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
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Gat.Web.report.ua
{
    public partial class rpt_transfer : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
        protected string shopCode = string.Empty;
        private string id = string.Empty;
        private decimal Qty = 0;
        private decimal Prices = 0;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                var mats = WebRepository.QueryGatUaTSTotal();
                Qty = mats[0];
                Prices = mats[1];
                this.Label1.Text = Qty.ToString();
                this.Label2.Text = Prices.ToString();
                id = Request.QueryString["id"];
                this.TopTitle = "UA报表";
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
                switch (mode)
                {
                    case "transfer_Reader":
                        LoadTempData(Request.Form["page"], Request["pageSize"], Request["transferType"]);
                        break;
                    case "transfer_Update":
                        UpdateData(Request["list"], Request["transferType"]);
                        break;
                    case "transfer_Delete":
                        DeleteData(Request["list"], Request["transferType"]);
                        break;
                    case "transfer_Save":
                        SaveAsToPureData(Request["selectedIds"], Request["transferType"]);
                        break;
                    case "transfer_SaveAll":
                        SaveAsToPureData("C", Request["transferType"]);
                        break;
                    case "transfer_ALLdel":
                        ALLdelData(Request["selectedIds"], Request["transferType"]);
                        break;
                    case "changeCommandArgument":
                        ChangeCommandArgument(Request["transferType"]);
                        break;
                    case "Import":
                        HttpPostedFile file1 = Context.Request.Files["file"];
                        string fileFullName = FileHelper.uploadFile(file1, "~/FileUpload/");  //这里引用的是上面封装的方法
                        returnText = Import(Request["transferType"], fileFullName);
                        break;
                    case "Export":
                        byte[] fileArray = null;
                        string fileName = "";
                        returnText = Export(Request["transferType"], ref fileArray, ref fileName);
                        if (returnText == "success")
                        {
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray);
                            this.Response.End();
                        }
                        break;
                }
                if (returnText != "success")
                {
                    this.Response.ContentType = "application/text";
                    this.Response.Write(returnText);
                    this.Response.End();
                }
            }
            #endregion
        }
        private void ALLdelData(string p, string transferType)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = new HandleArgs();

                if (transferType.ToUpper() == "SALE")
                {
                    handle = WebRepository.DeleteUaTransfer(p);
                }
                else
                {
                    handle = WebRepository.DeleteUaMaterialTransfer(p);
                }
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
        private void SaveAsToPureData(string p, string transferType)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = new HandleArgs();

                if (transferType.ToUpper() == "SALE")
                {
                    handle = WebRepository.TransferUaSaveAs(p);
                }
                else
                {
                    handle = WebRepository.TransferUaMaterialSaveAs(p);
                }
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
        private void UpdateData(string str, string transferType)
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
                            HandleArgs handle = new HandleArgs();
                            if (transferType.ToUpper() == "SALE")
                            {
                                int TransID = 0;
                                int.TryParse(item[0], out TransID);
                                int Qty = 0;
                                int.TryParse(item[1], out Qty);
                                handle = WebRepository.UpdateUaTransferQty(Qty, TransID);
                            }
                            else
                            {
                                int TransID = 0;
                                int.TryParse(item[0], out TransID);
                                int ActualQty = 0;
                                int pathQty = 0;
                                int.TryParse(item[1], out ActualQty);
                                int.TryParse(item[2], out pathQty);
                                handle = WebRepository.UpdateUaMaterialTransferQty(ActualQty, pathQty, TransID);
                            }
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
        private void DeleteData(string str, string transferType)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.TrimEnd(',');
                HandleArgs handle = new HandleArgs();
                if (transferType.ToUpper() == "SALE")
                {
                    handle = WebRepository.DeleteUaTransfer(str);
                }
                else
                {
                    handle = WebRepository.DeleteUaMaterialTransfer(str);
                }
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
        private void LoadTempData(string PageIndex, string pageSize, string transferType)
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
            //ddlSoule.SelectedItem.Selected = true;

            int currentPageIndex = 1;
            int currentPageSize = 15;
            int.TryParse(PageIndex, out currentPageIndex);
            int.TryParse(pageSize, out currentPageSize);




            // this.GridView = gvPos;
            //solutionID = 1;
            var result = new DataSourceResult();
            if (transferType.ToUpper() == "SALE")
            {
                var mats = WebRepository.QueryGatUaPosSaleTransfer(solutionID, currentPageIndex, currentPageSize);
                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GatUaPosSaleTransferCount(solutionID);

                result.Errors = "";

                //string s = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                this.ResponseToClient(result);
            }
            else
            {
                var mats = WebRepository.QueryGatUaMaterialTransfer(solutionID, currentPageIndex, currentPageSize);
                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GatUaMaterialTransferCount(solutionID);
                result.Errors = "";

                //string s = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                this.ResponseToClient(result);
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
            if (ddlSoule.SelectedItem == null)
            {
                //Response.Write("<script>alert('请先选择方案!')</script>");
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", "<script>alert('请先选择方案')</script>");
                return;
            }
            string fileNameStock = "【" + ddlSoule.SelectedItem.Text + "】的提报仓库.xls";
            //   fileNameStock = System.IO.Path.Combine(path, fileNameStock);
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("行号");
                dtSourceStock.Columns.Add("公司名称");
                dtSourceStock.Columns.Add("店铺代码");
                dtSourceStock.Columns.Add("店铺名称");
                dtSourceStock.Columns.Add("经营品牌");
                dtSourceStock.Columns.Add("单号");
                dtSourceStock.Columns.Add("交易时间");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("尺码");
                dtSourceStock.Columns.Add("SKU");
                dtSourceStock.Columns.Add("销售数量");
                dtSourceStock.Columns.Add("原价");
                dtSourceStock.Columns.Add("结算单价");
                dtSourceStock.Columns.Add("折扣");
                dtSourceStock.Columns.Add("销售金额");
                dtSourceStock.Columns.Add("销售员代码");
                dtSourceStock.Columns.Add("销售员名称");
                dtSourceStock.Columns.Add("促销描述");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelByNPOI(dtSourceStock, fileNameStock);
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileNameStock);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", "<script>alert('ex.Message')</script>");
                //Response.Write("<script>alert('ex.Message')</script>");
                //this.Alert(ex.Message, Core.DialogIcon.Warning);
            }
        }

        private string Export(string transferType, ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            string transferTypeName = transferType.ToUpper() == "SALE" ? "销售" : "库存";
            //if (ddlSoule.SelectedItem == null)
            //{
            //    //Response.Write("<script>alert('请先选择方案!')</script>");
            //    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", "<script>alert('请先选择方案')</script>");
            //    return "请先选择方案";
            //}
            //string fileNameStock = "【" + ddlSoule.SelectedItem.Text + "】的" + transferTypeName + "提报.xls";
            fileNameStock = transferTypeName + "提报.xls";
            //   fileNameStock = System.IO.Path.Combine(path, fileNameStock);
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                if (transferType.ToUpper() == "SALE")//销售
                {
                    dtSourceStock.Columns.Add("行号");
                    dtSourceStock.Columns.Add("公司名称");
                    dtSourceStock.Columns.Add("店铺代码");
                    dtSourceStock.Columns.Add("店铺名称");
                    dtSourceStock.Columns.Add("经营品牌");
                    dtSourceStock.Columns.Add("单号");
                    dtSourceStock.Columns.Add("交易时间");
                    dtSourceStock.Columns.Add("条码");
                    dtSourceStock.Columns.Add("尺码");
                    dtSourceStock.Columns.Add("SKU");
                    dtSourceStock.Columns.Add("销售数量");
                    dtSourceStock.Columns.Add("原价");
                    dtSourceStock.Columns.Add("结算单价");
                    dtSourceStock.Columns.Add("折扣");
                    dtSourceStock.Columns.Add("销售金额");
                    dtSourceStock.Columns.Add("销售员代码");
                    dtSourceStock.Columns.Add("销售员名称");
                    dtSourceStock.Columns.Add("促销描述");
                }
                else//库存
                {
                    dtSourceStock.Columns.Add("店铺代码");
                    dtSourceStock.Columns.Add("店铺名称");
                    dtSourceStock.Columns.Add("条码");
                    dtSourceStock.Columns.Add("尺码");
                    dtSourceStock.Columns.Add("SKU");
                    dtSourceStock.Columns.Add("库存数量");
                    dtSourceStock.Columns.Add("在途数量");
                }
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                //Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileNameStock);
                //Response.BinaryWrite(ms.ToArray());
                //Response.End();
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                //Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", "<script>alert('ex.Message')</script>");
                return ex.Message;
                //Response.Write("<script>alert('ex.Message')</script>");
                //this.Alert(ex.Message, Core.DialogIcon.Warning);
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
        //        dt.Columns["行号"].ColumnName = "Sequence";
        //        dt.Columns["公司名称"].ColumnName = "CompanyName";
        //        dt.Columns["经营品牌"].ColumnName = "CardName";
        //        dt.Columns["店铺代码"].ColumnName = "ShopCode";
        //        dt.Columns["店铺名称"].ColumnName = "ShopName";
        //        dt.Columns["单号"].ColumnName = "BillCode";
        //        dt.Columns["交易时间"].DataType = typeof(DateTime);
        //        dt.Columns["交易时间"].ColumnName = "BillTime";
        //        dt.Columns["条码"].DataType = typeof(string);
        //        dt.Columns["条码"].ColumnName = "BarCode";
        //        dt.Columns["尺码"].DataType = typeof(string);
        //        dt.Columns["尺码"].ColumnName = "SizeName";
        //        dt.Columns["SKU"].DataType = typeof(string);
        //        dt.Columns["SKU"].ColumnName = "MaterialCode";
        //        dt.Columns["销售数量"].DataType = typeof(Decimal);
        //        dt.Columns["销售数量"].ColumnName = "Qty";
        //        dt.Columns["原价"].DataType = typeof(Decimal);
        //        dt.Columns["原价"].ColumnName = "Price";
        //        dt.Columns["结算单价"].DataType = typeof(Decimal);
        //        dt.Columns["结算单价"].ColumnName = "BalaPrice";
        //        dt.Columns["折扣"].DataType = typeof(Decimal);
        //        dt.Columns["折扣"].ColumnName = "Discount";
        //        dt.Columns["销售金额"].DataType = typeof(Decimal);
        //        dt.Columns["销售金额"].ColumnName = "Amount";
        //        dt.Columns["销售员代码"].ColumnName = "PersonnelCode";
        //        dt.Columns["销售员名称"].ColumnName = "PersonnelName";
        //        dt.Columns["促销描述"].ColumnName = "PromotionName";
        //        for (int i = 0; i < dt1.Rows.Count; i++)
        //        {
        //            dt.Rows.Add(dt1.Rows[i].ItemArray);
        //        }
        //        //  dt.Columns["交易时间"].DataType = typeof(DateTime);
        //        HandleArgs args = WebRepository.InsertUaTransferQty(dt, ddlSoule.SelectedValue);
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

        private string Import(string transferType, string fileName)
        {
            string datetimeStr = string.Empty;
            string solutionId = Request["solutionId"];
            try
            {
                //string Extension = Path.GetExtension(FileUpload.PostedFile.FileName);
                //if (FileUpload.PostedFile.ContentLength <= 0)
                //{
                //    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('内容为空')</script>");
                //    return "内容为空";
                //}
                //string[] valid = new string[] { ".xls", ".xlsx" };
                //if (!valid.Contains(Extension))
                //{
                //    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('导入的文件不合法')</script>");
                //    return "导入的文件不合法";
                //}

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;
                //if (!System.IO.Directory.Exists(dir))
                //{
                //    Directory.CreateDirectory(dir);
                //}
                //if (File.Exists(filePath))
                //{
                //    File.Delete(filePath);
                //}

                //FileUpload.SaveAs(filePath);

                ExcelHelper Excel = new ExcelHelper();
                // DataTable dt1 = Excel.RenderDataTableFromExcels(filePath, "Sheet1", 0);
                DataTable dt1 = Excel.Import(filePath);
                // datetimeStr = dt1.Rows[0]["交易时间"].ToString();
                //  DataSet ds = Excel.ImportExcel(filePath, null, null, true);
                // DataTable dt = ds.Tables[0].Clone();
                DataTable dt = dt1.Clone();
                HandleArgs args = new HandleArgs();
                if (transferType.ToUpper() == "SALE")//销售
                {
                    dt.Columns["行号"].ColumnName = "Sequence";
                    dt.Columns["公司名称"].ColumnName = "CompanyName";
                    dt.Columns["经营品牌"].ColumnName = "CardName";
                    dt.Columns["店铺代码"].ColumnName = "ShopCode";
                    dt.Columns["店铺名称"].ColumnName = "ShopName";
                    dt.Columns["单号"].ColumnName = "BillCode";
                    dt.Columns["交易时间"].DataType = typeof(DateTime);
                    dt.Columns["交易时间"].ColumnName = "BillTime";
                    dt.Columns["条码"].DataType = typeof(string);
                    dt.Columns["条码"].ColumnName = "BarCode";
                    dt.Columns["尺码"].DataType = typeof(string);
                    dt.Columns["尺码"].ColumnName = "SizeName";
                    dt.Columns["SKU"].DataType = typeof(string);
                    dt.Columns["SKU"].ColumnName = "MaterialCode";
                    dt.Columns["销售数量"].DataType = typeof(Decimal);
                    dt.Columns["销售数量"].ColumnName = "Qty";
                    dt.Columns["原价"].DataType = typeof(Decimal);
                    dt.Columns["原价"].ColumnName = "Price";
                    dt.Columns["结算单价"].DataType = typeof(Decimal);
                    dt.Columns["结算单价"].ColumnName = "BalaPrice";
                    dt.Columns["折扣"].DataType = typeof(Decimal);
                    dt.Columns["折扣"].ColumnName = "Discount";
                    dt.Columns["销售金额"].DataType = typeof(Decimal);
                    dt.Columns["销售金额"].ColumnName = "Amount";
                    dt.Columns["销售员代码"].ColumnName = "PersonnelCode";
                    dt.Columns["销售员名称"].ColumnName = "PersonnelName";
                    dt.Columns["促销描述"].ColumnName = "PromotionName";
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        dt.Rows.Add(dt1.Rows[i].ItemArray);
                    }
                    //  dt.Columns["交易时间"].DataType = typeof(DateTime);
                    args = WebRepository.InsertUaTransferQty(dt, solutionId);
                }
                else//库存
                {
                    dt.Columns["店铺代码"].ColumnName = "ShopCode";
                    dt.Columns["店铺名称"].ColumnName = "ShopName";
                    dt.Columns["条码"].ColumnName = "BarCode";
                    dt.Columns["尺码"].ColumnName = "SizeName";
                    dt.Columns["SKU"].ColumnName = "MaterialCode";
                    dt.Columns["库存数量"].DataType = typeof(Decimal);
                    dt.Columns["库存数量"].ColumnName = "ActualQty";
                    dt.Columns["在途数量"].DataType = typeof(Decimal);
                    dt.Columns["在途数量"].ColumnName = "PathQty";
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        dt.Rows.Add(dt1.Rows[i].ItemArray);
                    }
                    //  dt.Columns["交易时间"].DataType = typeof(DateTime);
                    args = WebRepository.InsertUaMaterialTransferQty(dt, solutionId);
                }
                if (args.Status == HandleStatus.Completed)
                {
                    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入成功{0}')</script>", args.Message + datetimeStr));
                    return "导入成功" + args.Message + datetimeStr;
                }
                else
                {
                    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入失败{0}')</script>", args.Message));
                    return "导入失败" + args.Message;
                }
            }
            catch (Exception ex)
            {
                //Page.ClientScript.RegisterStartupScript(Page.GetType(), "yuanyuan", string.Format("<script>alert('导入失败:{0}')</script>", ex.Message + datetimeStr));
                return "导入失败:" + ex.Message + datetimeStr;
            }
        }
        private void ChangeCommandArgument(string transferType)
        {
            if (transferType.ToUpper() == "SALE")
            {
                this.btnExport.CommandArgument = "sale";
                this.btnImport.CommandArgument = "sale";
            }
            else
            {
                this.btnExport.CommandArgument = "stock";
                this.btnImport.CommandArgument = "stock";
            }
        }

    }
}
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

namespace Gat.Web.report.On
{
    public partial class rpt_stock_transfer : Core.ReportBase
    {
        //页面传递的GatOnPosSaleTransferB代码参数123456
        protected string shopCode = string.Empty; 
        private string id = string.Empty;
        //private decimal Qty = 0;
        //private decimal Amount = 0;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                id = Request.QueryString["id"];
                this.TopTitle = "On昂跑报表";
                this.SubTitle = "库存中转数据";
                this.LoadSolution();

            }
            #region AJAX 请求
            string mode = Request["mode"] ?? "";
            if (this.IsAJAXRequest || !string.IsNullOrEmpty(mode))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (mode.ToUpper())
                {
                    case "TRANSFER_READER"://
                        LoadTempData(Request.Form["page"], Request["pageSize"]);
                        break;
                    case "TRANSFER_SAVE"://选择保存
                        SaveAsToPureData(Request["selectedIds"]);
                        break;
                    case "TRANSFER_DELSELECT"://选择删除
                        DelSelectedData(Request["selectedIds"]);
                        break;
                    case "TRANSFER_SAVEALL"://全部保存
                        SaveAll(Request);
                        break;
                    case "TRANSFER_DELALL"://全部删除
                        DelALL(Request);
                        break;
                    case "SEARCH"://查询
                        Search(Request);
                        break;
                    case "DELETESEARCH"://按条件删除
                        DeleteSearch(Request);
                        break;
                    case "IMPORT"://导入
                        HttpPostedFile file1 = Context.Request.Files["file"];
                        string fileFullName = FileHelper.uploadFile(file1, "~/FileUpload/");  //这里引用的是上面封装的方法
                        returnText = Import(fileFullName);
                        break;
                    case "EXPORT"://导出
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
                var mats = WebRepository.QueryGatOnStocTransfer(solutionID, currentPageIndex, currentPageSize);
                var result = new DataSourceResult();

                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GetOnStockTransferTotalCount(solutionID,"");
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
                int page = string.IsNullOrEmpty(strPage) ? 1 : Convert.ToInt32(strPage);
                int pageSize = string.IsNullOrEmpty(strPageSize) ? 100 : Convert.ToInt32(strPageSize);
                var searchDic = WebRepository.SelectOnStockTransferPageList(page, pageSize, solutionId, strStartBillDate, strEndBillDate, shopCode, materialCode, sizeCode);
                result.StatusCode = "2";
                result.list = searchDic;
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

        #region 导入导出

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string Export(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "On昂跑库存导入格式.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("品牌店铺编号");
                dtSourceStock.Columns.Add("SKU");
                dtSourceStock.Columns.Add("数量");
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
                Decimal Qty = 0;
                var solutionId = Request["solutionId"];
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    Qty = 0;
                    Qty = Convert.ToDecimal(excelDt.Rows[i]["数量"]);
                    DataRow row = dt.NewRow();
                    row["SolutionID"] = solutionId;
                    row["CompanyID"] = "";
                    row["store_code"] = excelDt.Rows[i]["品牌店铺编号"];
                    row["inventory_date"] = DateTime.Now;
                    row["sku"] = excelDt.Rows[i]["SKU"];
                    row["quantity"] = Qty;
                    row["CardID"] = "";
                    row["ShopID"] = "";
                    row["ShopCode"] = excelDt.Rows[i]["品牌店铺编号"];
                    row["StateCode"] = 0;
                    row["StateName"] = "导入";
                    row["ModifyDtm"] = DateTime.Now;
                    dt.Rows.Add(row);
                }
                args = WebRepository.BulkCopyOnStockTransfer(dt);
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
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "store_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "inventory_date", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "sku", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "quantity", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StateCode", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StateName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyDtm", DataType = typeof(DateTime) });
            return dt;
        }

        #endregion

        #region 保存删除
        /// <summary>
        /// 选择删除
        /// </summary>
        /// <param name="p"></param>
        private void DelSelectedData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferOnDeltet(p, "Stock");
                if (handle.Status == HandleStatus.Fail)
                {
                    ai.Message = handle.Message;
                    ai.Html = handle.Message;
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
                else
                {
                    //重新加载GatOnPosSaleTransferB信息
                    ai.StatusCode = "1";
                    ai.Html = "删除成功";
                    this.ResponseToClient(ai);
                }
            }

        }

        //选择保存
        private void SaveAsToPureData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferOnSave(p, "Stock");
                if (handle.Status == HandleStatus.Fail)
                {
                    ai.Message = handle.Message;
                    ai.Html = handle.Message;
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
                else
                {
                    //重新加载GatOnPosSaleTransferB信息
                    ai.StatusCode = "1";
                    ai.Html = "删除成功";
                    this.ResponseToClient(ai);
                }
            }
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

                HandleArgs handle = WebRepository.TransferOnSaveAll(solutionId, "Stock");
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载GatOnPosSaleTransferB信息
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
                HandleArgs handle = WebRepository.DeleteOnTransferAll(solutionId, "Stock");
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载GatOnPosSaleTransferB信息
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

                HandleArgs handle = WebRepository.DeleteOnTransfer(solutionId, startBillDate, endBillDate, materialCode, sizeCode, shopCode);
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Message = handle.Message;
                    result.Html = handle.Message;
                    result.StatusCode = "2";
                    this.ResponseToClient(result, true);
                }
                else
                {
                    //重新加载GatOnPosSaleTransferB信息
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

        #endregion

    }
}
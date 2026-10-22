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

namespace Gat.Web.report.nike_omni_channel.rpt_pos
{
    public partial class rpt_pos_transfer : Core.ReportBase
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

                id = Request.QueryString["id"];
                this.TopTitle = "Nike全渠道报表";
                this.SubTitle = "销售中转数据";
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


                NikeBll_OC bll = new NikeBll_OC();
                var searchDic = bll.SelectPosSaleTransferPageList_OC(page, pageSize, solutionId, strStartBillDate, strEndBillDate, shopCode, materialCode, sizeCode);
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

        /// <summary>
        /// 选择删除
        /// </summary>
        /// <param name="p"></param>
        private void DelSelectedData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.DeleteNikePosSaleTransfer_OC(p);
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
                HandleArgs handle = WebRepository.DeleteNikeTransfer_OC(solutionId);
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

                HandleArgs handle = WebRepository.DeleteNikeTransfer_OC(solutionId, startBillDate, endBillDate, materialCode, sizeCode, shopCode);
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
        /// 选择保存数据
        /// </summary>
        /// <param name="p"></param>
        private void SaveAsToPureData(string p)
        {
            var ai = new AJAXInterop();
            if (!string.IsNullOrEmpty(p))
            {
                HandleArgs handle = WebRepository.TransferNikeSaveAs_OC(p);
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
                    ai.Html = "保存成功";
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

                HandleArgs handle = WebRepository.TransferNikeSaveAs_OC(solutionId);
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
        /// 导出
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string Export(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "Nike全渠道POS单导入格式.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("销售店铺代码");
                dtSourceStock.Columns.Add("销售时间");
                dtSourceStock.Columns.Add("发货店铺代码");
                dtSourceStock.Columns.Add("发货时间");
                dtSourceStock.Columns.Add("单号");
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("货品全称");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("尺码代码");
                dtSourceStock.Columns.Add("尺码名称");
                dtSourceStock.Columns.Add("销售数量");
                dtSourceStock.Columns.Add("销售价");
                dtSourceStock.Columns.Add("销售金额");
                dtSourceStock.Columns.Add("零售价");
                dtSourceStock.Columns.Add("零售金额");
                dtSourceStock.Columns.Add("折扣金额");
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
                var solutionId = Request["solutionId"];
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["SolutionID"] = solutionId;
                    row["ShopCode"] = excelDt.Rows[i]["销售店铺代码"];
                    row["BillDate"] = excelDt.Rows[i]["销售时间"];
                    row["StockCode"] = excelDt.Rows[i]["发货店铺代码"];
                    row["OutBillDate"] = excelDt.Rows[i]["发货时间"];
                    row["BillNo"] = excelDt.Rows[i]["单号"];
                    row["MaterialCode"] = excelDt.Rows[i]["货号"];
                    row["MaterialName"] = excelDt.Rows[i]["货品全称"];
                    row["BarCode"] = excelDt.Rows[i]["条码"];
                    row["SizeCode"] = excelDt.Rows[i]["尺码代码"];
                    row["SizeName"] = excelDt.Rows[i]["尺码名称"];
                    row["Qty"] = excelDt.Rows[i]["销售数量"];
                    row["BalaPrice"] = excelDt.Rows[i]["销售价"];
                    row["SaleAmount"] = excelDt.Rows[i]["销售金额"];
                    row["RetailPrice"] = excelDt.Rows[i]["零售价"];
                    row["RetailAmount"] = excelDt.Rows[i]["零售金额"];
                    row["DiscountAmount"] = excelDt.Rows[i]["折扣金额"];
                    row["ModifyTime"] = DateTime.Now;
                    row["TradeType"] = "1";
                    row["BillTime"] = DateTime.Now;
                    row["IsImport"] = 0;
                    dt.Rows.Add(row);
                }
                args = WebRepository.BulkCopyNikeTransfer_OC(dt);
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
            //必须与数据库表结构顺序一致
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "TransID", DataType = typeof(int), AutoIncrement = true });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionID", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillNo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillDate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "OutBillDate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BarCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BalaPrice", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SaleAmount", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "RetailPrice", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "RetailAmount", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "DiscountAmount", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "TradeType", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsImport", DataType = typeof(bool) });
            return dt;
        }

    }
}
using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using Gat.Web.Common;
using Gat.Web.Core;
using GatNewBll.Blls; 
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Gat.Web.report.iker
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
                this.TopTitle = "Iker报表";
                this.SubTitle = "POS中转数据";
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
                HandleArgs handle = WebRepository.DeleteSelectIkerSaletransfer(p);
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

                string strSolutionId = ddlSoule.SelectedValue;
                HandleArgs handle = WebRepository.SaveIkerTransfer(strSolutionId, p, "POS");
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
                HandleArgs handle = WebRepository.DeleteSelectIkerSaletransfer(str);
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

                int Qty = 0;
                decimal Amoun = 0;
                int Count = 0;

                string start_date = "";
                string end_date = "";
                string shopName = "";
                // this.GridView = gvPos;
                //var mats = WebRepository.QueryGatAdPosSaleTransfer(solutionID, currentPageIndex, currentPageSize);
                var mats = WebRepository.QueryGatIkerPosSaleTransfer(solutionID, currentPageIndex, currentPageSize, shopName, start_date, end_date, out Qty, out Amoun, out Count);
                var result = new DataSourceResult();

                result.Data = mats == null ? null : mats;
                result.Total = Count;
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
                dtSourceStock.Columns.Add("店铺代码");
                dtSourceStock.Columns.Add("单号");
                dtSourceStock.Columns.Add("销售时间");
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("规格");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("零售价");
                dtSourceStock.Columns.Add("销售数量");
                dtSourceStock.Columns.Add("销售金额");
                dtSourceStock.Columns.Add("来源单号");
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

                #region old
           
                var solutionId = Request["solutionId"];
                #endregion
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    decimal Qty = decimal.Parse(excelDt.Rows[i]["销售数量"].ToString());
                    row["SolutionID"] = solutionId;
                    row["ShopCode"] = excelDt.Rows[i]["店铺代码"];
                    row["ShopName"] = "";
                    row["StockCode"] = "";
                    row["StockName"] = "";
                    row["BillNo"] = excelDt.Rows[i]["单号"];
                    row["IsReturn"] = Qty > 0 ? true : false;
                    row["BillDate"] = excelDt.Rows[i]["销售时间"];
                    row["BillTime"] = DateTime.Now;
                    row["MaterialCode"] = excelDt.Rows[i]["货号"];
                    row["MaterialName"] = "";
                    row["BarCode"] = excelDt.Rows[i]["条码"];
                    row["SizeCode"] = "";
                    row["SizeName"] = excelDt.Rows[i]["规格"];
                    row["SaleAmount"] = excelDt.Rows[i]["销售金额"];
                    row["BalaPrice"] = 0;
                    row["Qty"] = Qty;
                    row["RetailPrice"] = excelDt.Rows[i]["零售价"];
                    row["ModifyTime"] = DateTime.Now;
                    row["SourceBillNo"] = excelDt.Rows[i]["来源单号"];
                    dt.Rows.Add(row);
                }
                args = WebRepository.ImportIIkerPos(dt);
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
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionID", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillNo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsReturn", DataType = typeof(bool) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillDate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BarCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BalaPrice", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SaleAmount", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "RetailPrice", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SourceBillNo", DataType = typeof(string) });
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

                HandleArgs handle = WebRepository.SaveIkerTransfer(strSolutionId, "C","POS");
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
                HandleArgs handle = WebRepository.DeleteAllIkerSaletransfer();
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

            //var result = new AJAXInterop();
            //var result = new DataSourceResult();
            try
            {
                int solutionId = string.IsNullOrEmpty(strSolutionId) ? 0 : Convert.ToInt32(strSolutionId);
                //DateTime startBillDate = string.IsNullOrEmpty(strStartBillDate) ? Convert.ToDateTime("1900-01-01 00:00:00") : Convert.ToDateTime(strStartBillDate);
                //DateTime endBillDate = string.IsNullOrEmpty(strEndBillDate) ? DateTime.Now : Convert.ToDateTime(strEndBillDate);
                int page = string.IsNullOrEmpty(strPage) ? 1 : Convert.ToInt32(strPage);
                int pageSize = string.IsNullOrEmpty(strPageSize) ? 100 : Convert.ToInt32(strPageSize);

                //AdBll bll = new AdBll();
                //var searchDic = bll.SelectPosSaleTransferPageList(page, pageSize, solutionId, strStartBillDate, strEndBillDate, shopCode, materialCode, sizeCode);
                //result.StatusCode = "2";
                //result.list = searchDic;


                int Qty = 0;
                decimal Amoun = 0;
                int Count = 0;
                int solutionID = 0;
                if (!string.IsNullOrEmpty(strSolutionId))
                {
                    int.TryParse(strSolutionId, out solutionID);
                }
                else
                {
                    int.TryParse(ddlSoule.SelectedValue, out solutionID);
                }
                string shopName = ""; string start_date = ""; string end_date = "";

                var result = new AJAXInterop();
                var mats = WebRepository.QueryGatIkerPosSaleTransfer(solutionID, page, pageSize, shopName, start_date, end_date, out Qty, out Amoun, out Count);

                Dictionary<string, object> result2 = new Dictionary<string, object>();

                result2.Add("data", mats);
                result2.Add("totalCount", Count);
                result2.Add("qty", Qty);
                result2.Add("amount", Amoun);


                result.StatusCode = "2";
                
                result.list = result2;
                
                this.ResponseToClient(result, true);








            }
            catch (Exception ex)
            {
              
               
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
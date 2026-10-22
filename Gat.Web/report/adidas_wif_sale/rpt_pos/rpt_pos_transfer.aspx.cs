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

namespace Gat.Web.report.adidas_wif_sale.rpt_pos
{
    public partial class rpt_pos_transfer : Core.ReportBase
    {
        //页面传递的店铺编号参数123456
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
                this.TopTitle = "阿迪智慧零售报表";
                this.SubTitle = "销售中转数据";
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
                HandleArgs handle = WebRepository.TransferDelAs_Wif(p);
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
                HandleArgs handle = WebRepository.TransferAdSaveAs_Wif(p);
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
                            HandleArgs handle = WebRepository.UpdateAdTransferQty_Wif(Qty, TransID);
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
                HandleArgs handle = WebRepository.DeleteAdTransfer_Wif(str);
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
                var mats = WebRepository.QueryGatAdPosSaleTransfer_Wif(solutionID, currentPageIndex, currentPageSize);
                var result = new DataSourceResult();

                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GatAdPosSaleTransferCount_Wif(solutionID);
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
                dtSourceStock.Columns.Add("店铺编号");
                dtSourceStock.Columns.Add("订单编号");
                dtSourceStock.Columns.Add("订单明细编号");
                dtSourceStock.Columns.Add("销售日期");
                //dtSourceStock.Columns.Add("订单时间");
                dtSourceStock.Columns.Add("付款时间");
                dtSourceStock.Columns.Add("产品编号");
                dtSourceStock.Columns.Add("尺码名称");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("数量");
                dtSourceStock.Columns.Add("除赠品总数量");
                dtSourceStock.Columns.Add("零售单价");
                dtSourceStock.Columns.Add("成交单价");
                dtSourceStock.Columns.Add("零售总金额");
                dtSourceStock.Columns.Add("成交总金额");
                dtSourceStock.Columns.Add("营业员编号");
                dtSourceStock.Columns.Add("会员编号");
                dtSourceStock.Columns.Add("订单类型");
                dtSourceStock.Columns.Add("销售类型");
                dtSourceStock.Columns.Add("会员编号");
                //dtSourceStock.Columns.Add("插入时间");
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
            fileNameStock = "阿迪智慧零售POS单导入格式.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
               // dtSourceStock.Columns.Add("序号");
                dtSourceStock.Columns.Add("店铺编号");
                dtSourceStock.Columns.Add("订单编号");
                dtSourceStock.Columns.Add("订单明细编号");
                dtSourceStock.Columns.Add("销售日期");
                //dtSourceStock.Columns.Add("订单时间");
                dtSourceStock.Columns.Add("付款时间");
                dtSourceStock.Columns.Add("产品编号");
                dtSourceStock.Columns.Add("尺码名称");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("数量");
                dtSourceStock.Columns.Add("除赠品总数量");
                dtSourceStock.Columns.Add("零售单价");
                dtSourceStock.Columns.Add("成交单价");
                dtSourceStock.Columns.Add("零售总金额");
                dtSourceStock.Columns.Add("成交总金额");
                dtSourceStock.Columns.Add("营业员编号");
                dtSourceStock.Columns.Add("会员编号");
                dtSourceStock.Columns.Add("订单类型");
                dtSourceStock.Columns.Add("销售类型");
                //dtSourceStock.Columns.Add("插入时间");
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
                    row["CompanyID"] = "";
                    row["retailitem_id"] = excelDt.Rows[i]["订单明细编号"];
                    row["retail_id"] = excelDt.Rows[i]["订单编号"];
                    row["product_code"] = excelDt.Rows[i]["产品编号"];
                    row["size_code"] = excelDt.Rows[i]["尺码名称"];
                    row["bar_code"] = excelDt.Rows[i]["条码"];
                    row["store_code"] = excelDt.Rows[i]["店铺编号"];
                    row["paytime"] = excelDt.Rows[i]["付款时间"];
                    row["billtime"] = DateTime.Now;
                    row["billdate"] = excelDt.Rows[i]["销售日期"];
                    row["qty"] = excelDt.Rows[i]["数量"];
                    row["upt_quantity"] = excelDt.Rows[i]["除赠品总数量"];
                    row["pricelist"] = excelDt.Rows[i]["零售单价"];
                    row["priceactual"] = excelDt.Rows[i]["成交单价"];
                    row["tot_amt_list"] = excelDt.Rows[i]["零售总金额"];
                    row["tot_amt_actual"] = excelDt.Rows[i]["成交总金额"];
                    row["staff_code"] = excelDt.Rows[i]["营业员编号"];
                    row["vip_id"] = excelDt.Rows[i]["会员编号"];
                    row["retail_type"] = excelDt.Rows[i]["订单类型"];
                    row["source_type"] = excelDt.Rows[i]["销售类型"];
                    row["insert_time"] = DateTime.Now;
                    row["CardID"] = "";
                    row["DrewTime"] = "";
                    row["BillNo"] = excelDt.Rows[i]["订单编号"];
                    row["ShopID"] = "";
                    row["ModifyTime"] = DateTime.Now;
                    row["IsImport"] = 1;
                    dt.Rows.Add(row);
                }
                args = WebRepository.BulkCopyAdTransfer_Wif(dt);
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
            dt.Columns.Add(new DataColumn() { ColumnName = "retailitem_id", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "retail_id", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "product_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "size_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "bar_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "store_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "paytime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "billtime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "billdate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "qty", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "upt_quantity", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "pricelist", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "priceactual", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "tot_amt_list", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "tot_amt_actual", DataType = typeof(Decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "staff_code", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "vip_id", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "retail_type", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "source_type", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "insert_time", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "DrewTime", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BillNo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsImport", DataType = typeof(Boolean) });
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

                HandleArgs handle = WebRepository.TransferAdSaveAs_Wif(solutionId);
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
                HandleArgs handle = WebRepository.DeleteAdTransfer_Wif(solutionId);
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
                AdBll_OC bll = new AdBll_OC();
                var searchDic = bll.SelectPosSaleTransferPageList_Wif(page, pageSize, solutionId, strStartBillDate, strEndBillDate, shopCode, materialCode, sizeCode);
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

                HandleArgs handle = WebRepository.DeleteAdTransfer_Wif(solutionId, startBillDate, endBillDate, materialCode, sizeCode, shopCode);
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
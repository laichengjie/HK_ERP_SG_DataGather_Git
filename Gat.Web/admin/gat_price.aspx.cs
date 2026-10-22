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

namespace Gat.Web.admin
{
    public partial class gat_price : Core.ReportBase
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
                this.TopTitle = "货号结算价设置";
                this.SubTitle = "货号结算价设置";


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
                    case "TRANSFER_DELSELECT"://选择删除
                        DelSelectedData(Request["selectedIds"]);
                        break;
                    case "TRANSFER_DELALL"://全部删除
                        DelALL(Request);
                        break;
                    case "SEARCH"://查询
                        Search(Request);
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
        /// 加载数据
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="pageSize"></param>
        private void LoadTempData(string PageIndex, string pageSize)
        {
            try
            {

                int solutionID = 0;
                int currentPageIndex = 1;
                int currentPageSize = 15;
                int.TryParse(PageIndex, out currentPageIndex);
                int.TryParse(pageSize, out currentPageSize);

                // this.GridView = gvPos;
                var mats = WebRepository.QueryGatOnStocTransfer(solutionID, currentPageIndex, currentPageSize);
                var result = new DataSourceResult();

                result.Data = mats == null ? null : mats;
                result.Total = WebRepository.GetOnStockTransferTotalCount(solutionID, "");
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
            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
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

            string materialCode = request["materialCode"];
            string companyID = request["companyID"];
            string strPage = request["page"];
            string strPageSize = request["pageSize"];
            var result = new AJAXInterop();
            try
            {
                int page = string.IsNullOrEmpty(strPage) ? 1 : Convert.ToInt32(strPage);
                int pageSize = string.IsNullOrEmpty(strPageSize) ? 100 : Convert.ToInt32(strPageSize);
                var searchDic = WebRepository.SelectMaterialBalaPricePageList(page, pageSize, companyID, materialCode);
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
            fileNameStock = "货品结算价导入格式.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("公司代码");
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("结算价");
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


        private string Import(string fileName)
        {
            string Message = "";
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;
                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();
                string CompanyID = "";
                string MaterialCode = "";
                decimal BalaPrice = 0;
                int HH = 0;
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    try
                    {
                        HH++;
                        CompanyID = excelDt.Rows[i]["公司代码"].ToString();
                        MaterialCode = excelDt.Rows[i]["货号"].ToString();
                        if (string.IsNullOrEmpty(CompanyID)|| string.IsNullOrEmpty(MaterialCode) || string.IsNullOrEmpty(excelDt.Rows[i]["结算价"].ToString()))
                        {
                            Message += "第【"+ HH.ToString()+ "】行公司代码、货号、结算价都不允许\n";
                        }
                    }
                    catch (Exception ex)
                    {

                        Message += "第【" + HH.ToString() + "】公司代码【" + CompanyID + "】货号【" + MaterialCode + "】导入失败：" + ex.Message + "\n";
                    }
                }
                if (!string.IsNullOrEmpty(Message))
                {
                    return Message ;
                }

                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    try
                    {
                        CompanyID = excelDt.Rows[i]["公司代码"].ToString();
                        MaterialCode = excelDt.Rows[i]["货号"].ToString();
                        BalaPrice = Convert.ToDecimal(excelDt.Rows[i]["结算价"]);
                        args = WebRepository.MaterialBalaPriceSaveAll(CompanyID, MaterialCode, BalaPrice, "", 0);
                        if (args.Status == HandleStatus.Completed)
                        {
                        }
                        else
                        {
                            Message += "公司代码【" + CompanyID + "】货号【" + MaterialCode + "】导入失败：" + args.Message+"\n";
                        }
                    }
                    catch (Exception ex)
                    {

                        Message += "公司代码【" + CompanyID + "】货号【" + MaterialCode + "】导入失败：" + ex.Message + "\n";
                    }
                    if (string.IsNullOrEmpty(Message))
                    {
                        Message = "导入成功";
                    }
                }
            }
            catch (Exception ex)
            {
                Message+= "导入失败:" + ex.ToString();
            }
            return Message;
        }
        private DataTable InitImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "TransID", DataType = typeof(int), AutoIncrement = true });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BalaPrice", DataType = typeof(decimal) });
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
                
                HandleArgs handle = WebRepository.MaterialBalaPriceSaveAll("", "", 0, p, 1);
                if (handle.Status == HandleStatus.Fail)
                {
                    ai.Message = handle.Message;
                    ai.Html = handle.Message;
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
                else
                {
                    
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
               // HandleArgs handle = WebRepository.DeleteOnTransferAll(solutionId, "Stock");
                HandleArgs handle = WebRepository.MaterialBalaPriceSaveAll("", "", 0, "", 1);
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
using Gat.BLL;
using Gat.Utils;
using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nike_omni_channel.rpt_inv
{
    public partial class rpt_inv_transfer : Core.ReportBase
    {
        //页面传递的店铺代码参数
        private string id = string.Empty;

        protected override void OnLoad(EventArgs e) 
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "Nike全渠道报表";
                this.SubTitle = "点库存中转数据";
                this.BindMat("");
                LoadSolution();
            }

            #region ajax请求
            string method = Request["method"] ?? "";

            if (this.IsAJAXRequest || !string.IsNullOrEmpty(method))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (method.ToUpper())
                {
                    case "IMPORT":
                        HttpPostedFile file1 = Context.Request.Files["file"];
                        string fileFullName = FileHelper.uploadFile(file1, "~/FileUpload/");  //这里引用的是上面封装的方法
                        returnText = Import(fileFullName);
                        break;
                    case "EXPORTFILE":
                        byte[] fileArray = null;
                        string fileName = "";
                        returnText = ExportFile(ref fileArray, ref fileName);
                        if (returnText == "success")
                        {
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray);
                            this.Response.End();
                        }
                        break;
                    case "DELETEALL":
                        DelALL(Request);
                        break;
                    case "SAVETOMATERIALINV":
                        SaveToMaterialInv(Request);
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
        /// 初始化表格控件
        /// </summary>  
        protected void BindMat(string shopCode = null)
        {
            decimal totalQty = 0;
            decimal totalPathQty = 0;
            int totalCount = 0;
            this.GridView = gvInv;
            var mats = WebRepository.QueryNikeStockTransfer_OC(this.CurrentPage, this.ItemsPerPage, shopCode, ref totalQty, ref totalPathQty, ref totalCount);
            this.DataSource = mats;
            this.RecordsCount = totalCount;
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_pathqty.Text = totalPathQty.ToString();
            
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
        /// 全部删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelALL(HttpRequest request)
        {
            var result = new AJAXInterop();
            try
            {
                string strSolutionId = request["solutionId"];
                HandleArgs handle = WebRepository.TruncateNikeStockTransfer_OC(strSolutionId);
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
        /// 保存数据到库存表
        /// </summary>
        /// <param name="request"></param>
        void SaveToMaterialInv(HttpRequest request)
        {
            var result = new AJAXInterop();
            try
            {
                string strSolutionId = request["solutionId"];
                HandleArgs handle = WebRepository.SaveNikeStockTransfer_OC(strSolutionId);
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
        /// 导出导入模板
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string ExportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "库存中转导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("库存日期");
                dtSourceStock.Columns.Add("仓库代码");
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("尺码代码");
                dtSourceStock.Columns.Add("尺码名称");
                dtSourceStock.Columns.Add("条码");
                dtSourceStock.Columns.Add("在库数量");
                dtSourceStock.Columns.Add("在途数量");
                dtSourceStock.Columns.Add("零售价");
                dtSourceStock.Columns.Add("库存类型描述");
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
                    row["CompanyName"] = "";
                    row["CardID"] = "";
                    row["CardName"] = "";
                    row["ShopCode"] = "";
                    row["ShopName"] = "";
                    row["StockCode"] = excelDt.Rows[i]["仓库代码"];
                    row["StockName"] = "";
                    row["MaterialCode"] = excelDt.Rows[i]["货号"];
                    row["MaterialName"] = "";
                    row["BarCode"] = excelDt.Rows[i]["条码"];
                    row["SizeName"] = excelDt.Rows[i]["尺码名称"];
                    row["SizeCode"] = excelDt.Rows[i]["尺码代码"];
                    row["Qty"] = excelDt.Rows[i]["在库数量"];
                    row["PathQty"] = excelDt.Rows[i]["在途数量"];
                    row["UpdateDate"] = excelDt.Rows[i]["库存日期"];
                    row["RetailPrice"] = excelDt.Rows[i]["零售价"];
                    row["InventoryTypeDesc"] = excelDt.Rows[i]["库存类型描述"];
                    row["ModifyTime"] = DateTime.Now;
                    row["IsImport"] = 0;
                    dt.Rows.Add(row);
                }

                args = WebRepository.BulkCopyNikeStockTransfer_OC(dt);
                if (args.Status == HandleStatus.Completed)
                {
                    return "导入成功" + args.Message + datetimeStr;
                }
                else
                {
                    return "导入失败" + args.Message;
                }
            }
            catch (Exception ex)
            {
                return "导入失败:" + ex.Message + datetimeStr;
            }
        }

        private DataTable InitImportDataTable()
        {
            //必须与数据库表结构顺序一致
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionID", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CompanyName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardID", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CardName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "BarCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SizeName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "UpdateDate", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PathQty", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "InventoryTypeDesc", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "RetailPrice", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsImport", DataType = typeof(bool) });
            return dt;
        }
    }

}
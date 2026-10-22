using Gat.BLL;
using Gat.Utils;
using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.iker
{
    public partial class rpt_pos : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
        protected string shopCode = string.Empty;
        protected string shopName = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected string barCode = string.Empty;
        protected int SumQty { get; set; }
        protected decimal SumAmount { get; set; }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.TopTitle = "Iker报表";
                this.SubTitle = "销售数据";

                shopCode = Request["shopcode"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];
                shopName = Request["shopName"];
                sizeCode = Request["sizeCode"];
                materialCode = Request["materialCode"];
                barCode = Request["barCode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                if (!string.IsNullOrEmpty(shopName)) shopName = HttpUtility.UrlDecode(shopName);
                this.txtShopName.Text = shopName;

                if (!string.IsNullOrEmpty(startBillDate))
                {
                    startBillDate = HttpUtility.UrlDecode(startBillDate);
                    this.txtStarDate.Text = startBillDate;
                }
                if (!string.IsNullOrEmpty(endBillDate))
                {
                    endBillDate = HttpUtility.UrlDecode(endBillDate);
                    this.txtEndDate.Text = endBillDate;
                }
                if (!string.IsNullOrEmpty(materialCode))
                {
                    materialCode = HttpUtility.UrlDecode(materialCode);
                    this.txtMaterialCode.Text = materialCode;
                }
                if (!string.IsNullOrEmpty(sizeCode))
                {
                    sizeCode = HttpUtility.UrlDecode(sizeCode);
                    this.txtSizeCode.Text = sizeCode;
                }
                if (!string.IsNullOrEmpty(barCode))
                {
                    barCode = HttpUtility.UrlDecode(barCode);
                    this.txtBarCode.Text = barCode;
                }
                this.LoadSolution();
                this.BindMat();

                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }


            #region AJAX 请求

            string mode = Request["mode"] ?? "";
            if (this.IsAJAXRequest || !string.IsNullOrEmpty(mode))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (mode.ToUpper())
                {
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
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void BindMat()
        {
            var strSouleID = ddlSoule.SelectedValue;
            var startBillDate = txtStarDate.Text;
            var endBillDate = txtEndDate.Text;
            var shopName = txtShopName.Text;
            var shopCode = txtShopCode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            var barCode = txtBarCode.Text;


            this.GridView = gvPos;
            int Qty = 0;
            decimal Amoun = 0;
            int Count = 0;
            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            var mats = WebRepository.QueryGatIkerPosSales(SouleID, this.CurrentPage, this.ItemsPerPage, startBillDate, endBillDate, shopCode, shopName, materialCode, barCode, sizeCode, out Qty, out Amoun, out Count);
            this.DataSource = mats;
            this.SumQty = Qty;
            this.SumAmount = Math.Round(Amoun, 2);
            this.RecordsCount = Count;
            gvPos.DataBind();
        }
       
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bthSearch_Click(object sender, EventArgs e)
        {

            this.CurrentPage = 1;
            this.BindMat();

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
            var data = WebRepository.QueryGatIkerPosSalesQueryable(solutionId, startDate, endDate, shopName);
            System.Data.DataTable dtSourceStock = data.ToDataTable(new string[] {
                "CompanyName", "ShopCode", "CardName", "BillNo", "BillDate", "IsReturn", "MaterialCode", "SizeName", "BarCode",
                "SaleAmount", "Qty","RetailPrice", "SourceBillNo", "ModifyTime" });

            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["CardName"].ColumnName = "品牌";
                dtSourceStock.Columns["BillNo"].ColumnName = "单号";
                dtSourceStock.Columns["IsReturn"].ColumnName = "是否退货";
                dtSourceStock.Columns["BillDate"].ColumnName = "销售时间";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["SizeName"].ColumnName = "规格";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["Qty"].ColumnName = "销售数量";
                dtSourceStock.Columns["RetailPrice"].ColumnName = "零售价";
                dtSourceStock.Columns["SaleAmount"].ColumnName = "销售金额";
                dtSourceStock.Columns["SourceBillNo"].ColumnName = "来源单号";
                dtSourceStock.Columns["ModifyTime"].ColumnName = "提报时间";
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
        /// 按时间段删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bthDelSearch_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            var startBillDate = txtStarDate.Text;
            var endBillDate = txtEndDate.Text;
            var shopName = txtShopName.Text;
            var shopCode = txtShopCode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            var barCode = txtBarCode.Text;
            HandleArgs handle = new HandleArgs();
            if (string.IsNullOrEmpty(startBillDate) || string.IsNullOrEmpty(endBillDate))
            {
                handle.Status = HandleStatus.Fail;
                this.Alert("日期不允许为空", Core.DialogIcon.Error);
                return;
            }
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                handle = WebRepository.DeleteIkerPosSalesData(SouleID, "", "", startBillDate, endBillDate, shopCode, shopName, materialCode, barCode, sizeCode);

                if (handle.Status == HandleStatus.Fail)
                {
                    this.Alert(handle.Message, Core.DialogIcon.Error);
                }
                else
                {
                    this.Alert("删除成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Error);
            }
            finally
            {
                this.CurrentPage = 1;
                this.BindMat();
            }
        }

        /// <summary>
        /// 选择删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bthDelChecked_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvPos.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("CheckBox1");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvPos.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);

            }
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                HandleArgs handle = WebRepository.DeleteIkerPosSalesData(SouleID, strId);

                if (handle.Status == HandleStatus.Fail)
                {
                    this.Alert(handle.Message, Core.DialogIcon.Error);
                }
                else
                {
                    this.Alert("删除成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Error);
            }
            finally
            {
                this.CurrentPage = 1;
                this.BindMat();
            }
        }
    }
}
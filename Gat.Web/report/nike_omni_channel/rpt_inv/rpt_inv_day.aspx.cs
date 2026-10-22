using Gat.BLL;
using Gat.Utils;
using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nike_omni_channel.rpt_inv
{
    public partial class rpt_inv_day : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string barCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected string stockCode = string.Empty;
        protected string startBillTime = string.Empty;
        protected string endBillTime = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;

        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);


            if (!IsPostBack)
            {
                this.TopTitle = "Nike全渠道报表";
                this.SubTitle = "点库存日报表";
                stockCode = Request["stockcode"];
                barCode = Request["barCode"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = Request["startBillTime"];
                endBillTime = Request["endBillTime"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];
                if (!string.IsNullOrEmpty(stockCode))
                {
                    stockCode = HttpUtility.UrlDecode(stockCode);
                    this.txtStockCode.Text = stockCode;
                }
                if (!string.IsNullOrEmpty(materialCode))
                {
                    materialCode = HttpUtility.UrlDecode(materialCode);
                    this.txtMaterialCode.Text = materialCode;
                }
                if (!string.IsNullOrEmpty(barCode))
                {
                    barCode = HttpUtility.UrlDecode(barCode);
                    this.txtBarcode.Text = barCode;
                }
                if (!string.IsNullOrEmpty(sizeCode))
                {
                    sizeCode = HttpUtility.UrlDecode(sizeCode);
                    this.txtSizeCode.Text = sizeCode;
                }
                if (!string.IsNullOrEmpty(startBillTime))
                {
                    startBillTime = HttpUtility.UrlDecode(startBillTime);
                    this.txtStartBillTime.Text = startBillTime;
                }
                if (!string.IsNullOrEmpty(endBillTime))
                {
                    endBillTime = HttpUtility.UrlDecode(endBillTime);
                    this.txtEndBillTime.Text = endBillTime;
                }
                this.LoadSolution();
                this.BindMat(barCode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
            }

        }
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
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            decimal totalQty = 0;
            decimal totalYcQty = 0;
            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);

            this.GridView = gvInv;
            var mats = WebRepository.QueryGatNikeMatInvs_OC(SouleID,this.CurrentPage, this.ItemsPerPage, ref totalQty, ref totalYcQty, "", barcode, materialcode, sizecode, startBillTime, endBillTime, stockcode);
            this.DataSource = mats["records"];
            this.RecordsCount = Convert.ToInt32(mats["recordCount"] ?? 0);
            gvInv.DataBind();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_pathqty.Text = totalYcQty.ToString();
            
            //this.label_top_totoal_ycqty.Text = totalYcQty.ToString();
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var barcode = txtBarcode.Text;
                var materialCode = txtMaterialCode.Text;
                var sizeCode = txtSizeCode.Text;
                var startBillTime = txtStartBillTime.Text;
                var endBillTime = txtEndBillTime.Text;
                var stockCode = txtStockCode.Text;

                int SouleID = 0;
                var strSouleID = ddlSoule.SelectedValue;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);

                var salesData = WebRepository.QueryGatNikeMatInvs_OC(SouleID, "", barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] {"UpdateDate",
                    "StockCode",  "MaterialCode", "SizeCode","SizeName", "BarCode",  "Qty" , "PathQty" ,  "RetailPrice","InventoryTypeDesc", "IsImport", "ModifyTime"});
                string fileName = "Nike库存提报.xlsx";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "库存日期";
                dtSourceStock.Columns["StockCode"].ColumnName = "仓库代码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeCode"].ColumnName = "尺码代码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码名称";
                dtSourceStock.Columns["Qty"].ColumnName = "在库数量";
                dtSourceStock.Columns["PathQty"].ColumnName = "在途数量";
                dtSourceStock.Columns["RetailPrice"].ColumnName = "零售价";
                dtSourceStock.Columns["InventoryTypeDesc"].ColumnName = "库存类型描述";
                dtSourceStock.Columns["IsImport"].ColumnName = "是否导入";
                dtSourceStock.Columns["ModifyTime"].ColumnName = "提报时间";
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileName);
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + HttpUtility.UrlEncode(fileName));
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Warning);
            }
        }

        protected void bthSearch_Click(object sender, EventArgs e)
        {
            this.CurrentPage = 1;
            var shopCode = "";
            var barcode = txtBarcode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            var startBillTime = txtStartBillTime.Text;
            var endBillTime = txtEndBillTime.Text;
            var stockCode = txtStockCode.Text;
            this.BindMat(barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
        }
    }

}
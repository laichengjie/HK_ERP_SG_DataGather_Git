using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.adidas_wif_sale.rpt_inv
{
    public partial class rpt_inv_hour : Core.ReportBase
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
                this.TopTitle = "阿迪智慧零售";
                this.SubTitle = "点库存三小时报表";
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
                this.BindMat(barCode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
            }

        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            decimal totalQty = 0;
            this.GridView = gvInv;
            var mats = WebRepository.QueryGatAdMatInvs_WifTB(this.CurrentPage, this.ItemsPerPage, ref totalQty, "", barcode, materialcode, sizecode, startBillTime, endBillTime, stockcode);
            this.DataSource = mats["records"];
            this.RecordsCount = Convert.ToInt32(mats["recordCount"] ?? 0);
            gvInv.DataBind();
            this.label_top_totoal_qty.Text = totalQty.ToString();

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

                var salesData = WebRepository.QueryGatAdMatInvsQyeryable_WifTB("", barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);

                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] {"store_code","product_code","size_code","bar_code","qty","stats_date" });

                string fileName = "阿迪的库存提报.xlsx";
                dtSourceStock.Columns["store_code"].ColumnName = "店铺编号";
                dtSourceStock.Columns["product_code"].ColumnName = "产品编号";
                dtSourceStock.Columns["size_code"].ColumnName = "尺码名称";
                dtSourceStock.Columns["bar_code"].ColumnName = "条码";
                dtSourceStock.Columns["qty"].ColumnName = "数量";
                dtSourceStock.Columns["stats_date"].ColumnName = "库存快照日期";
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
            //var shopCode = txtShopCode2.Text;
            var shopCode = "";
            var barcode = txtBarcode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            var startBillTime = txtStartBillTime.Text;
            var endBillTime = txtEndBillTime.Text;
            var stockCode = txtStockCode.Text;
            this.BindMat(barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);


            //this.GridView = gvInv;
            //var invData = WebRepository.QueryGatAdMatInvs_OC(this.CurrentPage, this.ItemsPerPage, shopCode, barcode, materialCode, sizeCode,startBillTime , endBillTime, stockCode);
            //this.DataSource = invData["records"];
            //this.RecordsCount = Convert.ToInt32(invData["recordCount"] ?? 0);
            //gvInv.DataBind();
        }
    }
}
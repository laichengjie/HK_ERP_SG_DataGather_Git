using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.adidas_omni_channel.rpt_inv
{
    public partial class rpt_inv_hour : Core.ReportBase
    {
        //页面传递的店铺代码参数
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
          

            this.TopTitle = "阿迪全渠道报表";
            this.SubTitle = "点库存三小时报表";
            base.OnLoad(e);

            if (!IsPostBack)
            {

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
            decimal totalYcQty = 0;
            this.GridView = gvInv;
            var mats = WebRepository.QueryGatAdMatInvs_OCTB(this.CurrentPage, this.ItemsPerPage, ref totalQty, ref totalYcQty, "", barcode, materialcode, sizecode, startBillTime, endBillTime, stockcode);
            this.DataSource = mats["records"];
            this.RecordsCount = Convert.ToInt32(mats["recordCount"] ?? 0);
            gvInv.DataBind();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_ycqty.Text = totalYcQty.ToString();
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

                var salesData = WebRepository.QueryGatAdMatInvsQyeryable_OCTB("", barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
              
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] {"UpdateDate",
                    "StockCode",  "MaterialCode", "SizeName", "BarCode",  "Qty" ,  "YCQty",  "ModifyTime"});
                string fileName = "阿迪的库存提报.xlsx";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "库存日期";
                dtSourceStock.Columns["StockCode"].ColumnName = "仓库代码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码名称";
                dtSourceStock.Columns["Qty"].ColumnName = "实际库存";
                dtSourceStock.Columns["YCQty"].ColumnName = "云仓可用库存";
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
            //var shopCode = txtShopCode2.Text;
            var barcode = txtBarcode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            var startBillTime = txtStartBillTime.Text;
            var endBillTime = txtEndBillTime.Text;
            var stockCode = txtStockCode.Text;
            this.BindMat(barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);

            //this.GridView = gvInv;
            //var invData = WebRepository.QueryGatAdMatInvs_OCTB(this.CurrentPage, this.ItemsPerPage, shopCode, barcode, materialCode, sizeCode, startBillTime, endBillTime, stockCode);
            //this.DataSource = invData["records"];
            //this.RecordsCount = Convert.ToInt32(invData["recordCount"] ?? 0);
            //gvInv.DataBind();
        }
    }

}
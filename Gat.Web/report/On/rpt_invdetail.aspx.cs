using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.on 
{
    public partial class rpt_invdetail : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;
        protected string startBillTime  = string.Empty;
        protected string endBillTime = string.Empty;
        protected string barCode = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "On昂跑报表";
                this.SubTitle = "库存明细数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = HttpUtility.UrlDecode(shopCode);
                }
                this.txtShopCode.Text = shopCode;

                startBillTime = Request["startBillTime"];
                if (!string.IsNullOrEmpty(startBillTime))
                {
                    startBillTime = HttpUtility.UrlDecode(startBillTime);
                }
                this.txtStartBillTime.Text = startBillTime;


                endBillTime = Request["endBillTime"];
                if (!string.IsNullOrEmpty(endBillTime))
                {
                    endBillTime = HttpUtility.UrlDecode(endBillTime);
                }
                this.txtEndBillTime.Text = endBillTime;
                 

                barCode = Request["barCode"];
                if (!string.IsNullOrEmpty(barCode))
                {
                    barCode = HttpUtility.UrlDecode(barCode);
                }
                this.textBarCode.Text = barCode;

                materialCode = Request["materialCode"];
                if (!string.IsNullOrEmpty(materialCode))
                {
                    materialCode = HttpUtility.UrlDecode(materialCode);
                }
                this.txtMaterialCode.Text = materialCode;


                //sizeCode = Request["sizeCode"];
                //if (!string.IsNullOrEmpty(sizeCode))
                //{
                //    sizeCode = HttpUtility.UrlDecode(sizeCode);
                //}
                //this.txtSizeCode.Text = sizeCode;


                this.BindMat();
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string matID = null)
        {
            var startBillTime = txtStartBillTime.Text.Trim();
            var endBillTime = txtEndBillTime.Text.Trim(); 
            var shopCode = txtShopCode2.Text.Trim(); 
            var materialCode = txtMaterialCode.Text.Trim(); 
            //var sizeCode = txtSizeCode.Text.Trim();
            var sizeCode = "";
            var barCode = textBarCode.Text.Trim(); 
             
            this.GridView = gvInv;
            int RecordsCount = 0;
            decimal Qty = 0;
            var mats = WebRepository.QueryOnInvDetailData(this.CurrentPage, this.ItemsPerPage, shopCode, startBillTime, endBillTime, materialCode, sizeCode, barCode,out RecordsCount ,out Qty);
          
            this.DataSource = mats;
            this.RecordsCount = RecordsCount;
            this.Label1.Text = Qty.ToString();
        }


        protected void bthSearch_Click(object sender, EventArgs e)
        {
            this.CurrentPage = 1;
            BindMat();
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var startBillTime = txtStartBillTime.Text.Trim();
                var endBillTime = txtEndBillTime.Text.Trim();
                var shopCode = txtShopCode2.Text.Trim();
                var materialCode = txtMaterialCode.Text.Trim();
                //var sizeCode = txtSizeCode.Text.Trim();
                var sizeCode = "";
                var barCode = textBarCode.Text.Trim();
                var salesData = WebRepository.QueryOnInvDetailList( shopCode,  startBillTime,  endBillTime,  materialCode,  sizeCode,  barCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "store_code",
                    "MaterialCode", "sku", "Qty", "IsImport", "inventory_date", "ModifyDtm" });
                string fileName = "ON的库存提报.xlsx";
                dtSourceStock.Columns["store_code"].ColumnName = "品牌店铺编号";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["sku"].ColumnName = "条码";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["IsImport"].ColumnName = "是否导入";
                dtSourceStock.Columns["inventory_date"].ColumnName = "库存日期";
                dtSourceStock.Columns["ModifyDtm"].ColumnName = "上传时间";

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
    }
    
}
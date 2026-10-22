using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.iker
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
        protected string shopCode = string.Empty;
        public int SumCount { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "Iker报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                this.BindMat();
            }
        }
        protected void BindMat()
        {
            var start_date = txtStarDate.Text;
            var end_date = txtEndDate.Text;
            var shopCode = txtShopCode.Text;
            var matID = txtShopCode.Text;
            this.GridView = gvInv;
            int Count = 0;
            int Qty = 0;
            var mats = WebRepository.QueryGatIkerMatInvs(this.CurrentPage, this.ItemsPerPage, out Count, out Qty, shopCode, start_date, end_date);
            this.DataSource = mats;
            this.SumCount = Qty;
            this.RecordsCount = Count;
        }


        protected void bthSearch_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindMat();

        }
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var start_date = txtStarDate.Text;
                var end_date = txtEndDate.Text;
                var shopCode = txtShopCode.Text;
                int Count = 0;
                int Qty = 0;
                var salesData = WebRepository.QueryGatIkerMatInvsData(start_date, end_date, shopCode, out Count, out Qty);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "CompanyName",
                    "ShopCode", "ShopName","CardName", "MaterialCode", "SizeName","Barcode", "Qty","PathQty","UpdateDate","IsImport" });
                string fileName = "Iker库存提报.xlsx";
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["ShopName"].ColumnName = "店铺名称";
                dtSourceStock.Columns["CardName"].ColumnName = "品牌";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["SizeName"].ColumnName = "规格";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["Qty"].ColumnName = "在库数量";
                dtSourceStock.Columns["PathQty"].ColumnName = "在途数量";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "库存日期";
                dtSourceStock.Columns["IsImport"].ColumnName = "是否导入";
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
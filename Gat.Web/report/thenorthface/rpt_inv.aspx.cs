using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.thenorthface
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
                this.TopTitle = "北面报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                this.BindMat(shopCode);
            }
        }
        protected void BindMat(string matID = null)
        {
            this.GridView = gvInv;
            int Count = 0;
            int Qty = 0;
            var mats = WebRepository.QueryGatTheNorthFaceMatInvs(this.CurrentPage, this.ItemsPerPage, out Count, out Qty, matID);
            this.DataSource = mats;
            this.SumCount = Qty;
            this.RecordsCount = Count;
        }


        protected void bthSearch_Click(object sender, EventArgs e)
        {
            var start_date = txtStarDate.Text;
            var end_date = txtEndDate.Text;
            var shopName = txtShopName.Text;
            var matID = txtShopCode.Text;
            this.GridView = gvInv;
            int Count = 0;
            int Qty = 0;
            var list=WebRepository.QueryGatTheNorthFaceMatInvsData(start_date,end_date, shopName, matID, out Count, out Qty);
            this.SumCount = Qty;
            this.RecordsCount = Count;
            this.DataSource = list;

        }
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var start_date = txtStarDate.Text;
                var end_date = txtEndDate.Text;
                var shopName = txtShopName.Text;
                var matID = txtShopCode.Text;
                int Count = 0;
                int Qty = 0;
                var salesData = WebRepository.QueryGatTheNorthFaceMatInvsData(start_date, end_date, shopName, matID, out Count, out Qty);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "CompanyName",
                    "ShopCode", "CardName", "MaterialCode", "SizeName", "BarCode", "UpdateDate", "Qty" });
                string fileName = "北面库存提报.xlsx";
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["CardName"].ColumnName = "店铺经营品牌";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "产品编号";
                dtSourceStock.Columns["SizeName"].ColumnName = "规格";
                dtSourceStock.Columns["BarCode"].ColumnName = "产品条码";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "库存日期";
                dtSourceStock.Columns["Qty"].ColumnName = "库存数量";
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
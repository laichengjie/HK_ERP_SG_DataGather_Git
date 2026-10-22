using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GatNewBll.Blls;


namespace Gat.Web.report.vans
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected decimal Qty = 0;
        protected decimal Prices = 0;
        VansBill bll = new VansBill();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "VANS报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopCode"];
                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
               
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = HttpUtility.UrlDecode(shopCode);
                    this.txtShopCode.Text = shopCode;
                }
                if (!string.IsNullOrEmpty(shopCode2))
                {
                    shopCode2 = HttpUtility.UrlDecode(shopCode2);
                    this.txtShopCode2.Text = shopCode2;
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
                this.Label1.Text = Qty.ToString();
   
                this.label_top_totoal_qty.Text = Qty.ToString();
                this.BindMat(shopCode2,  materialCode, sizeCode);

            }
        }


        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode, string materialCode, string sizeCode, bool isNeedPage = true)
        {
            this.GridView = gvPos;
            int SouleID = 0;
            int totalCount = 0;
            decimal totalQty = 0;
            var salesData = bll.SelectInvPage(SouleID, shopCode, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, true);
            this.DataSource = salesData;
            this.RecordsCount = totalCount;
            gvPos.DataBind();
            this.Label1.Text = totalQty.ToString();
            this.label_top_totoal_qty.Text = totalQty.ToString();
           

        }

        protected void bthSearch_Click(object sender, EventArgs e)
        {
           
            var shopCode = txtShopCode2.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            this.CurrentPage = 1;
            this.BindMat(shopCode, materialCode, sizeCode);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var shopCode = txtShopCode2.Text;
                var materialCode = txtMaterialCode.Text;
                var sizeCode = txtSizeCode.Text;

                int SouleID = 0;
                int totalCount = 0;
                decimal totalQty = 0;
                var salesData = bll.SelectInvPage(SouleID, shopCode, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, false);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "Brand", "ShopCode", "ShopName", "itemSKU", "ItemNO", "WaistSize", "Inseam", "ItemBarcode", "Quantity", "InventoryType", "InventoryDate" });

                string fileName = "Vans库存提报.xlsx";
                dtSourceStock.Columns["Brand"].ColumnName = "品牌";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["ShopName"].ColumnName = "店铺名称";
                dtSourceStock.Columns["itemSKU"].ColumnName = "SKU";
                dtSourceStock.Columns["ItemNO"].ColumnName = "货号";
                dtSourceStock.Columns["WaistSize"].ColumnName = "尺码";
                dtSourceStock.Columns["Inseam"].ColumnName = "颜色";
                dtSourceStock.Columns["ItemBarcode"].ColumnName = "条形码";
                dtSourceStock.Columns["Quantity"].ColumnName = "数量";
                dtSourceStock.Columns["InventoryType"].ColumnName = "库存类型";
                dtSourceStock.Columns["InventoryDate"].ColumnName = "库存时间";
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
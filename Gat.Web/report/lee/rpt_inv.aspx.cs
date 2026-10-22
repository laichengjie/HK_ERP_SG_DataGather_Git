using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.lee
{  
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数123456
     
        public decimal SumCount { get; set; }
        protected string shopCode = string.Empty;
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected decimal Qty = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "LEE报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                shopCode = Request["shopCode"];
                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];

                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = HttpUtility.UrlDecode(shopCode);
                    this.txtShopCode.Text = shopCode;
                }
               
                //if (!string.IsNullOrEmpty(materialCode))
                //{
                //    materialCode = HttpUtility.UrlDecode(materialCode);
                //    this.txtMaterialCode.Text = materialCode;
                //}
                //if (!string.IsNullOrEmpty(sizeCode))
                //{
                //    sizeCode = HttpUtility.UrlDecode(sizeCode);
                //    this.txtSizeCode.Text = sizeCode;
                //}
                this.BindMat(shopCode2, materialCode, sizeCode);
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode, string materialCode, string sizeCode, bool isNeedPage = true)
        {
            this.GridView = gvInv;
            int SouleID = 0;
            int totalCount = 0;
            decimal totalQty = 0;
            var InvData = WebRepository.QueryGatLeeMatInvs(SouleID, shopCode, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, true);
            this.DataSource = InvData;
            this.SumCount = totalQty;
            this.RecordsCount = totalCount;
            gvInv.DataBind();
        }

        protected void bthSearch_Click(object sender, EventArgs e)
        {
            var shopCode = txtShopCode.Text;
            var materialCode = "";
            var sizeCode = "";
            this.CurrentPage = 1;
            this.BindMat(shopCode, materialCode, sizeCode);


        }
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var start_date = txtStarDate.Text;
                var end_date = txtEndDate.Text;
                var shopName = txtShopName.Text;
                var matID = txtShopCode.Text;

                int SouleID = 0;
                int totalCount = 0;
                decimal totalQty = 0;
                var InvData = WebRepository.QueryGatLeeMatInvs(SouleID, shopCode, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, true);
                System.Data.DataTable dtSourceStock = InvData.ToDataTable(new string[] { "CompanyName",
                    "ShopCode", "ShopName","CardName", "MaterialCode",  "BarCode", "ActualQty", "ModifyDtm", "IsImport" });
                string fileName = "LEE库存提报.xlsx";
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["ShopName"].ColumnName = "店铺名称";
                dtSourceStock.Columns["CardName"].ColumnName = "店铺经营品牌";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "产品编号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
               //dtSourceStock.Columns["PathQty"].ColumnName = "在途数量";
                dtSourceStock.Columns["ActualQty"].ColumnName = "库存数量";
                dtSourceStock.Columns["ModifyDtm"].ColumnName = "库存日期";
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
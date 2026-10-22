using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.lining
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "李宁报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                this.BindMat(shopCode);
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string matID = null)
        {
            this.GridView = gvInv;
            var mats = WebRepository.QueryGatLiNingMatInvs(this.CurrentPage, this.ItemsPerPage, matID);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatLiNingMatInvCount(matID);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var salesData = WebRepository.QueryGatLiNingMatInvsQyeryable(this.txtShopCode.Text);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "ShopCode",
                    "ShopName", "CardName", "MaterialCode", "SizeName", "BarCode", "Qty", "UpdateDate" });
                string fileName = "李宁的库存提报.xlsx";
                dtSourceStock.Columns["ShopCode"].ColumnName = "仓库代码";
                dtSourceStock.Columns["ShopName"].ColumnName = "仓库名称";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "产品编码";
                dtSourceStock.Columns["SizeName"].ColumnName = "产品尺码";
                dtSourceStock.Columns["BarCode"].ColumnName = "产品条码";
                dtSourceStock.Columns["Qty"].ColumnName = "产品数量";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "库存日期";
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
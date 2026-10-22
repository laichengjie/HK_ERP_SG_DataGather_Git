using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.adidas
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string method = Request["method"] ?? "";
            if (!IsPostBack && string.IsNullOrEmpty(method))
            {
                this.TopTitle = "阿迪报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                this.BindMat(shopCode);
            }
            #region ajax请求

            if (this.IsAJAXRequest || !string.IsNullOrEmpty(method))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (method.ToUpper())
                {
                    case "SEARCH":
                        Search(Request);
                        break;
                }

            }
            #endregion
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string matID = null)
        {
            this.GridView = gvInv;
            var mats = WebRepository.QueryGatAdMatInvs(this.CurrentPage, this.ItemsPerPage, matID);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatAdMatInvCount(matID);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var salesData = WebRepository.QueryGatAdMatInvsQyeryable(this.txtShopCode.Text);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "CompanyName",
                    "ShopCode", "CardName", "MaterialCode", "SizeName", "BarCode", "UpdateDate", "Qty" });
                string fileName = "阿迪的库存提报.xlsx";
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["CardName"].ColumnName = "店铺经营品牌";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["UpdateDate"].ColumnName = "期末库存日期";
                dtSourceStock.Columns["Qty"].ColumnName = "期末库存";
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

        protected void Search(HttpRequest request)
        {
            Dictionary<string ,object> searchResult=new Dictionary<string, object>();
            var result = new AJAXInterop();
            try
            {
                searchResult = WebRepository.QueryGatAdMatInvs(this.CurrentPage,this.ItemsPerPage,
                    request["shopcode"], request["barcode"],
                    request["materialcode"], request["sizecode"]);
                //重新加载店铺信息
                result.StatusCode = "1";
                result.Html = "查询完成";
                this.GridView = gvInv;
                this.ResponseToClient(result, true);
            }
            catch (Exception ex)
            {
                result.StatusCode = "2";
                result.Message = ex.Message;
                result.Html = ex.Message;
                this.ResponseToClient(result, true);
            }
            finally
            {
                this.Response.End();
            }
        }
    }

}
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.ua
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;
        protected string barCode = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string method = Request["method"] ?? "";
            if (!IsPostBack && string.IsNullOrEmpty(method))
            {
                shopCode = Request["shopcode"] ?? "";
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = HttpUtility.UrlDecode(shopCode);
                }
                this.txtShopCode.Text = shopCode;

                barCode = Request["barCode"] ?? "";
                if (!string.IsNullOrEmpty(barCode))
                {
                    barCode = HttpUtility.UrlDecode(barCode);
                }
                this.txtBarCode.Text = barCode;

                materialCode = Request["materialCode"];
                if (!string.IsNullOrEmpty(materialCode))
                {
                    materialCode = HttpUtility.UrlDecode(materialCode);
                }
                this.txtMaterialCode.Text = materialCode;


                sizeCode = Request["sizeCode"];
                if (!string.IsNullOrEmpty(sizeCode))
                {
                    sizeCode = HttpUtility.UrlDecode(sizeCode);
                }
                this.txtSizeCode.Text = sizeCode;
                this.TopTitle = "UA报表";
                this.SubTitle = "库存数据";
                this.BindMat();
            }

            #region AJAX 请求

            if (this.IsAJAXRequest || !string.IsNullOrEmpty(method))
            {
                string returnText = string.Empty;
                this.Response.Clear();
                switch (method)
                {
                    case "AllDelete":
                        DeleteData();
                        break;
                }
                if (returnText != "success")
                {
                    this.Response.ContentType = "application/text";
                    this.Response.Write(returnText);
                    this.Response.End();
                }
            }
            #endregion
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat()
        {
            string ShopCode = this.txtShopCode.Text;
            string BarCode = this.txtBarCode.Text;
            string MaterialCode = this.txtMaterialCode.Text;
            string SizeCode = this.txtSizeCode.Text;

            this.GridView = gvInv;
            int RecordsCount = 0;
            decimal ActualQty = 0;
            decimal PathQty = 0;
            var mats = WebRepository.QueryGatUaMats(this.CurrentPage, this.ItemsPerPage, ShopCode, BarCode, MaterialCode, SizeCode, out RecordsCount, out ActualQty, out PathQty);
            this.DataSource = mats;
            this.RecordsCount = RecordsCount;
            this.Label1.Text = ActualQty.ToString();
            this.Label2.Text = PathQty.ToString();
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            this.CurrentPage = 1;
            BindMat();
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                string ShopCode = this.txtShopCode.Text;
                string BarCode = this.txtBarCode.Text;
                string MaterialCode = this.txtMaterialCode.Text;
                string SizeCode = this.txtSizeCode.Text;
                var salesData = WebRepository.QueryGatUaMatInvsQyeryable(ShopCode, BarCode, MaterialCode, SizeCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] {"CompanyName", "ShopCode",
                    "BarCode", "SizeName", "MaterialCode", "ActualQty", "PathQty", "UpdateDate" });
                string fileName = "Ua的库存提报.xlsx";
                dtSourceStock.Columns["CompanyName"].ColumnName = "公司名称";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "SKU";
                dtSourceStock.Columns["ActualQty"].ColumnName = "库存数量";
                dtSourceStock.Columns["PathQty"].ColumnName = "在途数量";
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


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="str"></param>
        private void DeleteData()
        {
            var ai = new AJAXInterop();
            HandleArgs handle = new HandleArgs();
            handle = WebRepository.DeleteUaAllMaterial();
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }
            //重新加载店铺信息
            ai.StatusCode = "1";
            ai.Html = "删除成功";
            this.ResponseToClient(ai);
            BtnSearch_Click(null, null);

        }
    }
}
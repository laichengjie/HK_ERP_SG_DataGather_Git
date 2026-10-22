using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.asics
{
    public partial class rpt_pos : Core.ReportBase
    {//页面传递的店铺代码参数
        protected string shopCode = string.Empty;

        //页面传递的店铺代码参数
       
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected string billCode = string.Empty;
        protected string barCode = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "亚瑟士报表";//亚玛芬
                this.SubTitle = "销售数据";

                shopCode = Request["shopcode"];
                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                billCode = Request["billCode"];
                barCode = Request["barCode"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];
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
                if (!string.IsNullOrEmpty(barCode))
                {
                    barCode = HttpUtility.UrlDecode(barCode);
                    this.txtBarCode.Text = barCode;
                }
                if (!string.IsNullOrEmpty(billCode))
                {
                    billCode = HttpUtility.UrlDecode(billCode);
                    this.txtBillCode.Text = billCode;
                }
                if (!string.IsNullOrEmpty(startBillDate))
                {
                    startBillDate = HttpUtility.UrlDecode(startBillDate);
                    this.txtStartBillDate.Text = startBillDate;
                }
                if (!string.IsNullOrEmpty(endBillDate))
                {
                    endBillDate = HttpUtility.UrlDecode(endBillDate);
                    this.txtEndBillDate.Text = endBillDate;
                }
                this.BindMat( shopCode2, billCode, barCode, startBillDate,  endBillDate,  materialCode,  sizeCode);
               
            }
        }
         
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode, string billCode, string barCode, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            this.GridView = gvPos;
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;
            var mats = WebRepository.QueryGatAsicsPosSale(this.CurrentPage, this.ItemsPerPage,  shopCode,  billCode,  barCode,  startBillDate,  endBillDate,  materialCode,  sizeCode, ref totalCount, ref totalQty, ref totalAmount);
            this.DataSource = mats;
            this.RecordsCount = totalCount;
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_amount.Text = totalAmount.ToString();
        }
        protected void bthSearch_Click(object sender, EventArgs e)
        {
            var billCode = txtBillCode.Text;
            var barCode = txtBarCode.Text;
            var startBillDate = txtStartBillDate.Text; 
            var endBillDate = txtEndBillDate.Text;
            var shopCode2 = txtShopCode2.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            this.CurrentPage = 1;
            this.BindMat(shopCode2, billCode, barCode, startBillDate, endBillDate, materialCode, sizeCode);


        }
    }
}
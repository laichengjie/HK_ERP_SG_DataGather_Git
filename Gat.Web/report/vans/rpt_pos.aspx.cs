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
    public partial class rpt_pos : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected string startBillTime = string.Empty;
        protected string endBillTime = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;
        protected decimal Qty = 0;
        protected decimal Prices = 0;
        VansBill bll = new VansBill();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "VANS报表";
                this.SubTitle = "销售数据";

                shopCode = Request["shopCode"];
                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = Request["startBillTime"];
                endBillTime = Request["endBillTime"];
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

                this.Label1.Text = Qty.ToString();
                this.Label2.Text = Prices.ToString();
                this.label_top_totoal_qty.Text = Qty.ToString();
                this.label_top_totoal_amount.Text = Prices.ToString();
                this.LoadSolution();
                this.BindMat(shopCode2, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);

            }
        }


        private void LoadSolution()
        {

            DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            ConnnectType connType = context.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
            using (var gat = new GatRepository(connType))
            {
                //绑定供应商
                ddlSoule.DataSource = gat.QuerySolutionsForVendor(context.VendorID);
                ddlSoule.DataTextField = "Title";
                ddlSoule.DataValueField = "ID";
                ddlSoule.DataBind();
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode,bool isNeedPage = true)
        {
            this.GridView = gvPos;
            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;

            var salesData = bll.SelectPage(SouleID, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, ref totalAmount,true);
            this.DataSource = salesData;
            this.RecordsCount = totalCount;
            gvPos.DataBind();
            
            this.Label1.Text = totalQty.ToString();
            this.Label2.Text = totalAmount.ToString();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_amount.Text = totalAmount.ToString();
         
        }

        protected void bthSearch_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            var startBillTime = txtStartBillTime.Text;
            var endBillTime = txtEndBillTime.Text;
            var startBillDate = txtStartBillDate.Text;
            var endBillDate = txtEndBillDate.Text;
            var shopCode = txtShopCode2.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            this.CurrentPage = 1;
            this.BindMat(shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var strSouleID = ddlSoule.SelectedValue;
                var startBillTime = txtStartBillTime.Text;
                var endBillTime = txtEndBillTime.Text;
                var startBillDate = txtStartBillDate.Text;
                var endBillDate = txtEndBillDate.Text;
                var shopCode = txtShopCode2.Text;
                var materialCode = txtMaterialCode.Text;
                var sizeCode = txtSizeCode.Text;

                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                //var salesData = WebRepository.QueryGatAdPosSalesData(SouleID, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);

                int totalCount = 0;
                decimal totalQty = 0;
                decimal totalAmount = 0;

                var salesData = bll.SelectPage(SouleID, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage, ref totalCount, ref totalQty, ref totalAmount, false);



                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "Brand", "ShopCode", "ShopName", "BillNo", "BillDate", "SKU", "ItemNO", "WaistSize", "Inseam", "ItemBarcode", "PayAmount", "Qty", "PersonnelCode", "PersonnelName", "MemberCode", "MemberName", "PromotionCode", "PromotionDescription", "SalesType", "Remarks", "BillTime", "SettlementAmount" });

                string fileName = "Vans销售提报.xlsx";
                dtSourceStock.Columns["Brand"].ColumnName = "品牌";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["ShopName"].ColumnName = "店铺名称";
                dtSourceStock.Columns["BillNo"].ColumnName = "单号";
                dtSourceStock.Columns["BillDate"].ColumnName = "销售时间";
                dtSourceStock.Columns["SKU"].ColumnName = "SKU";
                dtSourceStock.Columns["ItemNO"].ColumnName = "货号";
                dtSourceStock.Columns["WaistSize"].ColumnName = "尺码";
                dtSourceStock.Columns["Inseam"].ColumnName = "颜色";
                dtSourceStock.Columns["ItemBarcode"].ColumnName = "条形码";
                dtSourceStock.Columns["PayAmount"].ColumnName = "销售金额";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["PersonnelCode"].ColumnName = "营业员代码";
                dtSourceStock.Columns["PersonnelName"].ColumnName = "营业员名称";
                dtSourceStock.Columns["MemberCode"].ColumnName = "会员卡号";
                dtSourceStock.Columns["MemberName"].ColumnName = "会员名称";
                dtSourceStock.Columns["PromotionCode"].ColumnName = "活动代码";
                dtSourceStock.Columns["PromotionDescription"].ColumnName = "活动名称";
                dtSourceStock.Columns["SalesType"].ColumnName = "销售类型";
                dtSourceStock.Columns["Remarks"].ColumnName = "备注";
                dtSourceStock.Columns["BillTime"].ColumnName = "制单时间";
                dtSourceStock.Columns["SettlementAmount"].ColumnName = "结算金额";
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
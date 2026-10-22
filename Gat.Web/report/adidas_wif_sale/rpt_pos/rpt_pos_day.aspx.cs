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


namespace Gat.Web.report.adidas_wif_sale.rpt_pos
{
    public partial class rpt_pos_day : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode2 = string.Empty;
        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;
        protected string startBillTime = string.Empty;
        protected string endBillTime = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;
        protected decimal Qty = 0;
        protected decimal Prices = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var mats = WebRepository.QueryGatTotal_Wif();
                Qty = mats[0];
                Prices = mats[1];
                this.TopTitle = "阿迪智慧零售报表";
                this.SubTitle = "销售日报表";

                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = Request["startBillTime"];
                endBillTime = Request["endBillTime"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];
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
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            this.GridView = gvPos;
            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            AdBll_OC bll = new AdBll_OC();
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;
            var saleData = bll.SelectPosSalePageList_Wif(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount,ref totalQty,ref totalAmount, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
            this.DataSource = saleData;
            this.RecordsCount = totalCount;
            gvPos.DataBind();
            this.Label1.Text = totalQty.ToString();
            this.Label2.Text = totalAmount.ToString();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_amount.Text = totalAmount.ToString();
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
            this.BindMat(shopCode2, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
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
                var salesData = WebRepository.QueryGatAdPosSalesData_Wif(SouleID, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "store_code", "retail_id", "retailitem_id", "billdate", "billtime", "paytime", "product_code", "size_code", "bar_code", "qty", "upt_quantity", "pricelist", "priceactual", "tot_amt_list", "tot_amt_actual", "staff_code", "vip_id", "retail_type",  "source_type", "insert_time", });
                string fileName = "阿迪的销售提报.xlsx";

                dtSourceStock.Columns["store_code"].ColumnName = "店铺代码";
                dtSourceStock.Columns["retail_id"].ColumnName = "订单编号";
                dtSourceStock.Columns["retailitem_id"].ColumnName = "订单明细编号";
                dtSourceStock.Columns["billdate"].ColumnName = "销售日期";
                dtSourceStock.Columns["billtime"].ColumnName = "订单时间";
                dtSourceStock.Columns["paytime"].ColumnName = "付款时间";
                dtSourceStock.Columns["product_code"].ColumnName = "产品编号";
                dtSourceStock.Columns["size_code"].ColumnName = "尺码名称";
                dtSourceStock.Columns["bar_code"].ColumnName = "条码";
                dtSourceStock.Columns["qty"].ColumnName = "数量";
                dtSourceStock.Columns["upt_quantity"].ColumnName = "除赠品总数量";
                dtSourceStock.Columns["pricelist"].ColumnName = "零售单价";
                dtSourceStock.Columns["priceactual"].ColumnName = "成交单价";
                dtSourceStock.Columns["tot_amt_list"].ColumnName = "零售总金额";
                dtSourceStock.Columns["tot_amt_actual"].ColumnName = "成交总金额";
                dtSourceStock.Columns["staff_code"].ColumnName = "营业员编号";
                dtSourceStock.Columns["vip_id"].ColumnName = "会员编号";
                dtSourceStock.Columns["retail_type"].ColumnName = "订单类型";
                dtSourceStock.Columns["source_type"].ColumnName = "销售类型";
                dtSourceStock.Columns["insert_time"].ColumnName = "插入时间";
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

        protected void bthDelSearch_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            var startBillTime = txtStartBillTime.Text;
            var endBillTime = txtEndBillTime.Text;
            var startBillDate = txtStartBillDate.Text;
            var endBillDate = txtEndBillDate.Text;
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                HandleArgs handle = WebRepository.DeleteAdPosSalesData_Wif(SouleID, startBillTime, endBillTime, startBillDate, endBillDate);

                if (handle.Status == HandleStatus.Fail)
                {
                    this.Alert(handle.Message, Core.DialogIcon.Error);
                }
                else
                {
                    this.Alert("删除成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Error);
            }
            finally
            {
                //this.GridView = gvPos;
                //var mats = WebRepository.QueryGatAdPosSales_OC(this.CurrentPage, this.ItemsPerPage, shopCode2);
                //this.DataSource = mats;
                //this.RecordsCount = WebRepository.GatAdPosSaleCount_OC(shopCode2);
                bthSearch_Click(sender, e);
            }
        }

        protected void bthDelChecked_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvPos.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("CheckBox1");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvPos.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);

            }
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                HandleArgs handle = WebRepository.DeleteAdPosSalesData_Wif(SouleID, strId);

                if (handle.Status == HandleStatus.Fail)
                {
                    this.Alert(handle.Message, Core.DialogIcon.Error);
                }
                else
                {
                    this.Alert("删除成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Error);
            }
            finally
            {
                //this.GridView = gvPos;
                //var mats = WebRepository.QueryGatAdPosSales_OC(this.CurrentPage, this.ItemsPerPage, shopCode2);
                //this.DataSource = mats;
                //this.RecordsCount = WebRepository.GatAdPosSaleCount_OC(shopCode2);

                bthSearch_Click(sender, e);
            }
        }
    }
}
using Gat.BLL;
using Gat.Utils;
using Gat.Web.Common;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nikeapi
{
    public partial class rpt_pos : Core.ReportBase
    {
        NikeApiBill bill = new NikeApiBill();
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.TopTitle = " Nike报表";
            this.SubTitle = "Nike 销售明细";
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.LoadSolution();
            }
        }
        private void LoadSolution()
        {

            DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            ConnnectType connType = context.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
            using (var gat = new GatRepository(connType))
            {
                //绑定供应商
                solution.DataSource = gat.QuerySolutionsForVendor(context.VendorID);
                solution.DataTextField = "Title";
                solution.DataValueField = "ID";
                solution.DataBind();
            }
        }
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var StrstartBillDate = txtStartBillDate.Text;
                var StrendBillDate = txtEndBillDate.Text;
                var Shopcode = shopCode.Text;
                var CardShopCode = cardShopCode.Text;
                var BillNo = billNo.Text;
                string statusValue = status.SelectedValue;

                int SouleID = 0;
                var strSouleID = solution.SelectedValue;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);

                DateTime? StartBillDate = null;
                DateTime? EndBillDate = null;

                // ★ 修复了原代码中 Convert.ToDateTime(null) 会报错的问题
                if (!string.IsNullOrEmpty(StrstartBillDate))
                    DateTime.TryParse(StrstartBillDate, out DateTime tempStart);
                if (!string.IsNullOrEmpty(StrstartBillDate) && DateTime.TryParse(StrstartBillDate, out DateTime tempStartDate))
                    StartBillDate = tempStartDate;

                if (!string.IsNullOrEmpty(StrendBillDate) && DateTime.TryParse(StrendBillDate, out DateTime tempEndDate))
                    EndBillDate = tempEndDate;

                int totalCount = 0;
                int? totalQty = 0;
                decimal? totalAmount = 0;

                // 获取数据 (注意最后一个参数 false 可能代表不分页，请根据实际接口调整)
                var salesData = bill.GatNikeApiPosSaleList(SouleID, 0, 0, StartBillDate, EndBillDate, Shopcode, CardShopCode, BillNo, statusValue, ref totalCount, ref totalQty, ref totalAmount, false);

                // ★ 关键修改：提取的字段必须与前端表格列字段名严格一致
                string[] exportColumns = new string[] {
            "BillDate",
            "STORE_CD",
            "BusinessType",
            "BillNo",
            "OrderType",
            "UPC",
            "PRODUCT_CD",
            "SIZE_CD",
            "NET_SALES_PRC_INCL_TAX",
            "SALES_QTY",
            "NET_SALES_AMT_INCL_TAX",
            "MSRP_LC",
            "TOTAL_PARTNER_MSRP_AMT",
            "ModifyDtm"
        };

                System.Data.DataTable dtSourceStock = salesData.ToDataTable(exportColumns);

                string fileName = "Nike销售明细.xlsx";

                // 重命名中文列头
                dtSourceStock.Columns["BillDate"].ColumnName = "单据日期";
                dtSourceStock.Columns["STORE_CD"].ColumnName = "店铺代码";
                dtSourceStock.Columns["BusinessType"].ColumnName = "业务类型";
                dtSourceStock.Columns["BillNo"].ColumnName = "单据编号";
                dtSourceStock.Columns["OrderType"].ColumnName = "订单类型";
                dtSourceStock.Columns["UPC"].ColumnName = "条码";
                dtSourceStock.Columns["PRODUCT_CD"].ColumnName = "货号";
                dtSourceStock.Columns["SIZE_CD"].ColumnName = "尺码代码";
                dtSourceStock.Columns["NET_SALES_PRC_INCL_TAX"].ColumnName = "结算价";
                dtSourceStock.Columns["SALES_QTY"].ColumnName = "数量";
                dtSourceStock.Columns["NET_SALES_AMT_INCL_TAX"].ColumnName = "结算金额";
                dtSourceStock.Columns["MSRP_LC"].ColumnName = "零售价";
                dtSourceStock.Columns["TOTAL_PARTNER_MSRP_AMT"].ColumnName = "零售金额";
                dtSourceStock.Columns["ModifyDtm"].ColumnName = "修改时间";

                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileName);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + HttpUtility.UrlEncode(fileName));
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Warning);
            }
        }
    }
}
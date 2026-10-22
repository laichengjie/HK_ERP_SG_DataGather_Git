using Gat.BLL;
using Gat.Utils;
using Gat.Web.Common;
using GatNewBll.Blls;
using GatNewBll.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nikeapi
{
    public partial class rpt_outin : Core.ReportBase
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
                int SouleID = 0;
                var strSouleID = solution.SelectedValue;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);

                DateTime? StartBillDate = null;
                DateTime? EndBillDate = null;
                if (!string.IsNullOrEmpty(StrstartBillDate) && DateTime.TryParse(StrstartBillDate, out DateTime tempStartDate))
                    StartBillDate = tempStartDate;

                if (!string.IsNullOrEmpty(StrendBillDate) && DateTime.TryParse(StrendBillDate, out DateTime tempEndDate))
                    EndBillDate = tempEndDate;
                QueryInputDto input = new QueryInputDto
                {
                    SolutionId = SouleID,
                    VendorId = 0,
                    Page = 0,
                    PageSize = 0,
                    StartBillDate = StartBillDate,
                    EndBillDate = EndBillDate,
                    //ShopCode = shopCode != null ? shopCode.Text.Trim() : "",
                    StockCode = stockCode != null ? stockCode.Text.Trim() : "",
                    BillNo = billNo != null ? billNo.Text.Trim() : "",
                    BarCode = barCode != null ? barCode.Text.Trim() : "",
                     MaterialCode = materialCode != null ? materialCode.Text.Trim() : "",
                     SizeCode = sizeCode != null ? sizeCode.Text.Trim() : "",
                    Status = status.SelectedValue,
                    //InterfaceType = InterfaceType.SelectedValue
                };
                var result = bill.GatNikeApiPosSaleList(input, false);
                string[] exportColumns = new string[] { "BillDate", "STORE_CD", "BusinessType", "BillNo", "OrderType", "UPC","PRODUCT_CD","SIZE_CD","NET_SALES_PRC_INCL_TAX", "SALES_QTY","NET_SALES_AMT_INCL_TAX","MSRP_LC", "TOTAL_PARTNER_MSRP_AMT","ModifyDtm" };

                System.Data.DataTable dtSourceStock = result.Data.ToDataTable(exportColumns);

                string fileName = "Nike出入库明细.xlsx";
                dtSourceStock.Columns["BillDate"].ColumnName = "单据日期";
                dtSourceStock.Columns["STORE_CD"].ColumnName = "店铺代码";
                dtSourceStock.Columns["BusinessType"].ColumnName = "业务类型";
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
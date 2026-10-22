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

namespace Gat.Web.report.burgeon
{
    public partial class rpt_pos : Core.ReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.TopTitle = " 伯俊报表";
            this.SubTitle = "伯俊Pos销售明细";
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
                if (!string.IsNullOrEmpty(StrstartBillDate))
                {
                    StartBillDate = Convert.ToDateTime(StartBillDate);
                }
                if (!string.IsNullOrEmpty(StrendBillDate))
                {
                    EndBillDate = Convert.ToDateTime(StrendBillDate);
                }


                int totalCount = 0;
                int? totalQty = 0;
                decimal? totalAmount = 0;
                
                

                BurgeonBill bll = new BurgeonBill();
                var salesData = bll.GetPageDetailList(SouleID, 0, 0, StartBillDate, EndBillDate, Shopcode, CardShopCode, BillNo, "", ref totalCount, ref totalQty, ref totalAmount,false);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "IsSuccess", "BillDate", "ShopCode", "Salesrep", "BillNo", "MaterialCode", "BarCode", "Type", "Qty", "BalaPrice", "PayAmount" });
                string fileName = "伯爵销售提报.xlsx";
                dtSourceStock.Columns["IsSuccess"].ColumnName = "是否推送";
                dtSourceStock.Columns["BillDate"].ColumnName = "销售日期";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["Salesrep"].ColumnName = "营业员";
                dtSourceStock.Columns["BillNo"].ColumnName = "单据编号";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["Type"].ColumnName = "销售类型";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["BalaPrice"].ColumnName = "成交单价";
                dtSourceStock.Columns["PayAmount"].ColumnName = "成交金额";
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
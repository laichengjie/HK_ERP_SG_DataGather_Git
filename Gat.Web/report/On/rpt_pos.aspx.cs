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

namespace Gat.Web.report.On 
{
    public partial class rpt_pos : Core.ReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.TopTitle = "On昂跑报表";
            this.SubTitle = "销售数据";
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


                var salesData = WebRepository.GetOnPosSalesData(SouleID,0,0,StartBillDate, EndBillDate, Shopcode, CardShopCode, BillNo, statusValue);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "sale_date", "store_code", "ShopCode", "BillNo", "Qty", "Amount", "order_type", "original_order_number", "IsImport",  "PostJaon", "ReturnJson", "CreateDtm", "ModifyDtm" });
                string fileName = "On销售提报.xlsx";

                dtSourceStock.Columns["sale_date"].ColumnName = "销售日期";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["store_code"].ColumnName = "品牌店铺代码";
                dtSourceStock.Columns["BillNo"].ColumnName = "单据编号";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["Amount"].ColumnName = "金额";
                dtSourceStock.Columns["order_type"].ColumnName = "销售类型";
                dtSourceStock.Columns["original_order_number"].ColumnName = "原单号";
                dtSourceStock.Columns["IsImport"].ColumnName = "是否导入";
                dtSourceStock.Columns["PostJaon"].ColumnName = "发送数据";
                dtSourceStock.Columns["ReturnJson"].ColumnName = "返回数据";
                dtSourceStock.Columns["CreateDtm"].ColumnName = "首次发送时间";
                dtSourceStock.Columns["ModifyDtm"].ColumnName = "最后修改时间";

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
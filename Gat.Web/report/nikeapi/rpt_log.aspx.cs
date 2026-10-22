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
    public partial class rpt_poslog : Core.ReportBase
    {
        NikeApiBill bill = new NikeApiBill();
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.TopTitle = " Nike报表";
            this.SubTitle = "Nike接口日志";
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
                    ShopCode = shopCode != null ? shopCode.Text.Trim() : "",
                    StockCode = stockCode != null ? stockCode.Text.Trim() : "",
                    BillNo = billNo != null ? billNo.Text.Trim() : "",
                    //BarCode = barCode != null ? barCode.Text.Trim() : "",
                   // MaterialCode = materialCode != null ? materialCode.Text.Trim() : "",
                   // SizeCode = sizeCode != null ? sizeCode.Text.Trim() : "",
                    Status = status.SelectedValue,
                    InterfaceType = InterfaceType.SelectedValue
                };
                var salesData = bill.GatNikeApiLoList(input, false);
              
                System.Data.DataTable dtSourceStock = salesData.Data.ToDataTable(new string[] {"Id", "BillDate", "BillTypeName", "BillNo", "ShopCode", "StockCode", "Qty", "Amount", "PostJaon", "ReturnJson", "ModifyDtm" });

                string fileName = "Nike接口日志.xlsx";

                dtSourceStock.Columns["Id"].ColumnName = "Id";
                dtSourceStock.Columns["BillDate"].ColumnName = "单据日期";
                dtSourceStock.Columns["BillTypeName"].ColumnName = "单据类型";
                dtSourceStock.Columns["BillNo"].ColumnName = "单号";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["StockCode"].ColumnName = "仓库代码";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["Amount"].ColumnName = "金额";
                dtSourceStock.Columns["PostJaon"].ColumnName = "发送数据";
                dtSourceStock.Columns["ReturnJson"].ColumnName = "返回数据";
                dtSourceStock.Columns["ModifyDtm"].ColumnName = "最后修改时间";

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
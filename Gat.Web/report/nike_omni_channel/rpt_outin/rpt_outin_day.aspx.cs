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


namespace Gat.Web.report.nike_omni_channel.rpt_outin
{
    public partial class rpt_outin_day : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string outStockCode = string.Empty;
        protected string inStockCode = string.Empty;
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

                this.TopTitle = "Nike全渠道报表";
                this.SubTitle = "出入库日报表";

                outStockCode = Request["outStockCode"];
                inStockCode = Request["inStockCode"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = Request["startBillTime"];
                endBillTime = Request["endBillTime"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];
                if (!string.IsNullOrEmpty(outStockCode))
                {
                    outStockCode = HttpUtility.UrlDecode(outStockCode);
                    this.txtOutStockCode.Text = outStockCode;
                }
                if (!string.IsNullOrEmpty(inStockCode))
                {
                    inStockCode = HttpUtility.UrlDecode(inStockCode);
                    this.txtInStockCode.Text = inStockCode;
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
                this.BindMat(outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);

            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            this.GridView = gvPos;

            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);

            NikeBll_OC bll = new NikeBll_OC();
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;
            var saleData = bll.SelectOutInPageList_OC(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, ref totalQty, ref totalAmount, outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
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
            var outStockCode = txtOutStockCode.Text;
            var inStockCode = txtInStockCode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;
            this.CurrentPage = 1;
            this.BindMat(outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
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
                var outStockCode = txtOutStockCode.Text;
                var inStockCode = txtInStockCode.Text;
                var materialCode = txtMaterialCode.Text;
                var sizeCode = txtSizeCode.Text;

                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                var salesData = WebRepository.QueryGatNikeOutInData_OC(SouleID, outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "BillDate", "BillNo", "OutStockCode", "InStockCode", "MaterialCode", "BarCode", "SizeCode", "SizeName", "Qty", "Amount","MovementType", "PackingListNumber", "BillTypeName", "ModifyTime" });

                string fileName = "Nike出入库提报.xlsx";
                dtSourceStock.Columns["BillDate"].ColumnName = "单据生效日期";
                dtSourceStock.Columns["BillNo"].ColumnName = "单号";
                dtSourceStock.Columns["OutStockCode"].ColumnName = "调出仓代码";
                dtSourceStock.Columns["InStockCode"].ColumnName = "入库仓代码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeCode"].ColumnName = "尺码代码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码名称";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["Amount"].ColumnName = "金额";
                dtSourceStock.Columns["MovementType"].ColumnName = "货物移动类型";
                dtSourceStock.Columns["PackingListNumber"].ColumnName = "装箱单号";
                dtSourceStock.Columns["BillTypeName"].ColumnName = "单据类型名称";
                dtSourceStock.Columns["ModifyTime"].ColumnName = "提报时间";

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
                HandleArgs handle = WebRepository.DeleteNikeOutInData_OC(SouleID, startBillTime, endBillTime, startBillDate, endBillDate);

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
                bthSearch_Click(null, null);
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
                HandleArgs handle = WebRepository.DeleteNikeOutInData_OC(SouleID, strId);

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
                bthSearch_Click(null, null);
            }
        }
    }
}
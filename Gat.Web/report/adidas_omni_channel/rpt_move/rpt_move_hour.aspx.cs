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


namespace Gat.Web.report.adidas_omni_channel.rpt_move
{
   
    public partial class rpt_move_hour : Core.ReportBase
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
                var mats = WebRepository.QueryGatTotal_Move_OCTB();
                Qty = mats[0];
                Prices = mats[1];
                this.TopTitle = "阿迪全渠道报表";
                this.SubTitle = "调拨日报表";

                outStockCode = Request["outStockCode"];
                inStockCode = Request["inStockCode"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = "";// Request["startBillTime"];
                endBillTime = "";// Request["startBillTime"];
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
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary> 
        protected void BindMat(string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            this.GridView = gvPos;
            //var mats = WebRepository.QueryGatAdPosSales(this.CurrentPage, this.ItemsPerPage, shopCode);
            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            //var salesData = WebRepository.QueryGatAdPosSalesData(SouleID, shopCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode, this.CurrentPage, this.ItemsPerPage);
            AdBll_OC bll = new AdBll_OC();
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;
            var saleData = bll.SelectMovePageList_OCTB(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, ref totalQty, ref totalAmount, outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
            this.DataSource = saleData;
            this.RecordsCount = totalCount;//Convert.ToInt32(salesData["dataCount"] ?? 0);
            gvPos.DataBind();


            this.Label1.Text = totalQty.ToString();
            this.Label2.Text = totalAmount.ToString();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_amount.Text = totalAmount.ToString();
            //this.DataSource = mats;
            //this.RecordsCount = WebRepository.GatAdPosSaleCount(shopCode);
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
            var startBillTime = "";//txtStartBillTime.Text;
            var endBillTime = "";// txtEndBillTime.Text;
            var startBillDate = txtStartBillDate.Text;
            var endBillDate = txtEndBillDate.Text;
            var outStockCode = txtOutStockCode.Text;
            var inStockCode = txtInStockCode.Text;
            var materialCode = txtMaterialCode.Text;
            var sizeCode = txtSizeCode.Text;

            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            this.GridView = gvPos;
            AdBll_OC bll = new AdBll_OC();
            int totalCount = 0;
            decimal totalQty = 0;
            decimal totalAmount = 0;
            this.CurrentPage = 1;
            var saleData = bll.SelectMovePageList_OCTB(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, ref totalQty, ref totalAmount, outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
            this.DataSource = saleData;
            this.RecordsCount = totalCount;//Convert.ToInt32(salesData["dataCount"] ?? 0);
            gvPos.DataBind();


            this.Label1.Text = totalQty.ToString();
            this.Label2.Text = totalAmount.ToString();
            this.label_top_totoal_qty.Text = totalQty.ToString();
            this.label_top_totoal_amount.Text = totalAmount.ToString();

            //this.RecordsCount = list.Count;
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                //var start_date = txtStartBillTime.Text;
                //var end_date = txtEndBillTime.Text;
                //var mats = WebRepository.QueryGatAdPosSalesALL();
                //var mats = WebRepository.QueryGatAdPosSalesALL(start_date, end_date);
                var strSouleID = ddlSoule.SelectedValue;
                var startBillTime = "";//txtStartBillTime.Text;
                var endBillTime = "";// txtEndBillTime.Text;
                var startBillDate = txtStartBillDate.Text;
                var endBillDate = txtEndBillDate.Text;
                var outStockCode = txtOutStockCode.Text;
                var inStockCode = txtInStockCode.Text;
                var materialCode = txtMaterialCode.Text;
                var sizeCode = txtSizeCode.Text;

                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                var salesData = WebRepository.QueryGatAdMoveData_OCTB(SouleID, outStockCode, inStockCode, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "BillDate", "BillNo", "ManualBillNo", "OutStocType", "OutStockCode", "InStocType", "InStockCode", "MaterialCode", "BarCode", "SizeName", "Qty", "State" });

                string fileName = "阿迪的调拨提报.xlsx";
                dtSourceStock.Columns["BillDate"].ColumnName = "单据生效日期";
                dtSourceStock.Columns["BillNo"].ColumnName = "单号";
                dtSourceStock.Columns["ManualBillNo"].ColumnName = "手工单号";
                dtSourceStock.Columns["OutStocType"].ColumnName = "发货仓类型";
                dtSourceStock.Columns["OutStockCode"].ColumnName = "发货仓代码";
                dtSourceStock.Columns["InStocType"].ColumnName = "入库仓类型";
                dtSourceStock.Columns["InStockCode"].ColumnName = "入库仓代码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "货号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码名称";
                dtSourceStock.Columns["Qty"].ColumnName = "数量";
                dtSourceStock.Columns["State"].ColumnName = "状态";
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
            var startBillTime = "";//txtStartBillTime.Text;
            var endBillTime = "";// txtEndBillTime.Text;
            var startBillDate = txtStartBillDate.Text;
            var endBillDate = txtEndBillDate.Text;
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                HandleArgs handle = WebRepository.DeleteAdMoveData_OCTB(SouleID, startBillTime, endBillTime, startBillDate, endBillDate);

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
                this.GridView = gvPos;
                var mats = WebRepository.QueryGatAdMove_OCTB(this.CurrentPage, this.ItemsPerPage, "");
                this.DataSource = mats;
                this.RecordsCount = WebRepository.GatAdMoveCount_OCTB("");
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
                HandleArgs handle = WebRepository.DeleteAdMoveData_OCTB(SouleID, strId);

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
                this.GridView = gvPos;
                var mats = WebRepository.QueryGatAdMove_OCTB(this.CurrentPage, this.ItemsPerPage, "");
                this.DataSource = mats;
                this.RecordsCount = WebRepository.GatAdMoveCount_OCTB("");
            }
        }
    }
}
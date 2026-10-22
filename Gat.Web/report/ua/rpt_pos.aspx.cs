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


namespace Gat.Web.report.ua
{
    public partial class rpt_pos : Core.ReportBase
    {
        //页面传递的店铺代码参数

        protected string shopCode2 = string.Empty;
        protected string startBillTime = string.Empty;
        protected string endBillTime = string.Empty;
        protected string startBillDate = string.Empty;
        protected string endBillDate = string.Empty;

        protected string materialCode = string.Empty;
        protected string sizeCode = string.Empty;


        //var start_date = txtStarDate.Text;
        //var end_date = txtEndDate.Text;
        //var billDateStartDate = txtBillDateStartDate.Text;
        //var billDateEndDate = txtBillDateEndDate.Text; 


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {

                this.TopTitle = "UA报表";
                this.SubTitle = "销售数据";

                shopCode2 = Request["shopCode2"];
                materialCode = Request["materialCode"];
                sizeCode = Request["sizeCode"];
                startBillTime = Request["startBillTime"];
                endBillTime = Request["endBillTime"];
                startBillDate = Request["startBillDate"];
                endBillDate = Request["endBillDate"];

                // 获取参数，若为空则设置默认值
               
                if (string.IsNullOrEmpty(startBillDate))
                {
                    startBillDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    // 如果需要带时间，可以使用 "yyyy-MM-dd HH:mm:ss"
                }
                if (string.IsNullOrEmpty(endBillDate))
                {
                    endBillDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrEmpty(shopCode2))
                {
                    shopCode2 = HttpUtility.UrlDecode(shopCode2);
                    this.txtShopCode2.Text = shopCode2;
                }
                if (!string.IsNullOrEmpty(startBillTime))
                {
                    startBillTime = HttpUtility.UrlDecode(startBillTime);
                    this.txtStarDate.Text = startBillTime;
                }
                if (!string.IsNullOrEmpty(endBillTime))
                {
                    endBillTime = HttpUtility.UrlDecode(endBillTime);
                    this.txtEndDate.Text = endBillTime;
                }
                if (!string.IsNullOrEmpty(startBillDate))
                {
                    startBillDate = HttpUtility.UrlDecode(startBillDate);
                    this.txtBillDateStartDate.Text = startBillDate;
                }
                if (!string.IsNullOrEmpty(endBillDate))
                {
                    endBillDate = HttpUtility.UrlDecode(endBillDate);
                    this.txtBillDateEndDate.Text = endBillDate;
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


                this.LoadSolution();
                this.BindMat(shopCode2, startBillTime, endBillTime, startBillDate, endBillDate, materialCode, sizeCode);
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode = null, string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string MaterialCode = "", string SizeCode = "")
        {
            this.GridView = gvPos;
            //var mats = WebRepository.QueryGatUaPosSales(this.CurrentPage, this.ItemsPerPage, shopCode);
            //this.DataSource = mats;
            //this.RecordsCount = WebRepository.GatUaPosSaleCount(shopCode);
            UaBll bll = new UaBll();
            var solutinId = ddlSoule.SelectedValue;
            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(solutinId))
                Int32.TryParse(solutinId, out SouleID);
            int totalCount = 0;
            decimal SumQty = 0;
            decimal SumAmount = 0;

            var list = bll.SelectPage(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, ref  SumQty, ref  SumAmount, startBillTime, endBillTime, startBillDate, endBillDate, shopCode, MaterialCode, SizeCode);
            this.DataSource = list;
            this.RecordsCount = totalCount;

            this.Label1.Text = SumQty.ToString();
            this.Label2.Text = SumAmount.ToString();
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
            var start_date = txtStarDate.Text.Trim();
            var end_date = txtEndDate.Text.Trim();
            var billDateStartDate = txtBillDateStartDate.Text.Trim();
            var billDateEndDate = txtBillDateEndDate.Text.Trim();

            var shopCode2 = this.txtShopCode2.Text.Trim();
            var materialCode = this.txtMaterialCode.Text.Trim();
            var sizeCode = this.txtSizeCode.Text.Trim();

            if (string.IsNullOrEmpty(billDateStartDate))
            {
                this.Alert("交易日期开始时间不能为空", Core.DialogIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(billDateEndDate))
            {
                this.Alert("交易日期结束时间不能为空", Core.DialogIcon.Warning);
                return;
            }
       

            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            this.GridView = gvPos;
            //var list = WebRepository.QueryGatUaPosSalesData(SouleID, start_date, end_date, billDateStartDate, billDateEndDate);
            //this.DataSource = list;

            UaBll bll = new UaBll();
            int totalCount = 0;
            this.CurrentPage = 1;
            this.BindMat(shopCode2, start_date, end_date, billDateStartDate, billDateEndDate, materialCode, sizeCode);
            //var list = bll.SelectPage(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, start_date, end_date, billDateStartDate, billDateEndDate);
            //this.DataSource = list;
            //this.RecordsCount = totalCount;

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //var start_date = txtStarDate.Text;
            //var end_date = txtEndDate.Text;
            //var mats = WebRepository.QueryGatUaPosSalesALL();
            //var mats = WebRepository.QueryGatAdPosSalesALL(start_date, end_date);
            var strSouleID = ddlSoule.SelectedValue;
            var start_date = txtStarDate.Text;
            var end_date = txtEndDate.Text;
            var billDateStartDate = txtBillDateStartDate.Text;
            var billDateEndDate = txtBillDateEndDate.Text;

            var shopCode2 = this.txtShopCode2.Text.Trim();
            var materialCode = this.txtMaterialCode.Text.Trim();
            var sizeCode = this.txtSizeCode.Text.Trim();

            if (string.IsNullOrEmpty(billDateStartDate))
            {
                this.Alert("交易日期开始时间不能为空", Core.DialogIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(billDateEndDate))
            {
                this.Alert("交易日期结束时间不能为空", Core.DialogIcon.Warning);
                return;
            }

            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);

            UaBll bll = new UaBll();
            //var list = WebRepository.QueryGatUaPosSalesData(SouleID, start_date, end_date, billDateStartDate, billDateEndDate);
            var list = bll.SelectAll(SouleID, start_date, end_date, billDateStartDate, billDateEndDate, shopCode2, materialCode, sizeCode);
            System.Data.DataTable dtSourceStock = list.AsQueryable().ToDataTable(new string[] { "BillDate", "BillNo", "ShopCode", "BillCode", "Sequence", "BarCode", "SizeName", "MaterialCode", "Qty", "BillDate", "Price", "Discount", "Amount", "ArriveDate", "PersonnelCode", "PersonnelName", "PromotionName", "BillTime" });
            string fileNameStock = "UA的提报仓库.xls";
            try
            {
                //交易时间  店铺代码 单号  行号 条码  SKU 销售数量    原价 折扣  零售额（含税）	到店日期 销售员代码   销售员名称 促销描述

                dtSourceStock.Columns["BillDate"].ColumnName = "交易时间";
                dtSourceStock.Columns["ShopCode"].ColumnName = "店铺代码";
                dtSourceStock.Columns["BillCode"].ColumnName = "单号";
                dtSourceStock.Columns["Sequence"].ColumnName = "行号";
                dtSourceStock.Columns["BarCode"].ColumnName = "条码";
                dtSourceStock.Columns["SizeName"].ColumnName = "尺码";
                dtSourceStock.Columns["MaterialCode"].ColumnName = "SKU";
                dtSourceStock.Columns["Qty"].ColumnName = "销售数量";
                dtSourceStock.Columns["Price"].ColumnName = "原价";
                dtSourceStock.Columns["Discount"].ColumnName = "折扣";
                dtSourceStock.Columns["Amount"].ColumnName = "零售额（含税）";
                dtSourceStock.Columns["ArriveDate"].ColumnName = "到店日期";
                dtSourceStock.Columns["PersonnelCode"].ColumnName = "销售员代码";
                dtSourceStock.Columns["PersonnelName"].ColumnName = "销售员名称";
                dtSourceStock.Columns["PromotionName"].ColumnName = "促销描述";
                dtSourceStock.Columns["BillTime"].ColumnName = "制单日期";

                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileNameStock);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, Core.DialogIcon.Warning);
            }
        }

        protected void bthDelUaSearch_Click(object sender, EventArgs e)
        {
            var strSouleID = ddlSoule.SelectedValue;
            var startBillTime = txtStarDate.Text;
            var endBillTime = txtEndDate.Text;
            var startBillDate = txtBillDateStartDate.Text;
            var endBillDate = txtBillDateEndDate.Text;
            try
            {
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                HandleArgs handle = WebRepository.DeleteUaPosSalesData(SouleID, startBillTime, endBillTime, startBillDate, endBillDate);

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
                //var mats = WebRepository.QueryGatUaPosSales(this.CurrentPage, this.ItemsPerPage, shopCode);
                //this.DataSource = mats;
                //this.RecordsCount = WebRepository.GatUaPosSaleCount(shopCode);
                bthSearch_Click(sender, e);
            }
        }

        protected void bthDelUaChecked_Click(object sender, EventArgs e)
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
                HandleArgs handle = WebRepository.DeleteUaPosSalesData(SouleID, strId);

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
                //var mats = WebRepository.QueryGatUaPosSales(this.CurrentPage, this.ItemsPerPage, shopCode);
                //this.DataSource = mats;
                //this.RecordsCount = WebRepository.GatUaPosSaleCount(shopCode);
                bthSearch_Click(sender, e);
            }
        }
    }
}
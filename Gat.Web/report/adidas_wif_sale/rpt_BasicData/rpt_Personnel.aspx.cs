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


namespace Gat.Web.report.adidas_wif_sale.rpt_BasicData
{
    public partial class rpt_Personnel : Core.ReportBase 
    {
        //页面传递的店铺代码参数
        protected string staffCode = string.Empty;
        protected string staffName = string.Empty;
        protected string shopCode = string.Empty;
        
        protected decimal Qty = 0;
        protected decimal Prices = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var mats = WebRepository.QueryGatTotal_Personnel_Wif();
                Qty = mats[0];
                Prices = 0;
                this.TopTitle = "阿迪智慧零售报表";
                this.SubTitle = "职员资料";
                staffCode = Request["staffCode"];
                staffName = Request["staffName"];
                shopCode = Request["shopCode"];
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = HttpUtility.UrlDecode(shopCode);
                    this.txtShopCode.Text = shopCode;
                }

                if (!string.IsNullOrEmpty(staffCode))
                {
                    staffCode = HttpUtility.UrlDecode(staffCode);
                    this.txtStaffCode.Text = staffCode;
                }
                if (!string.IsNullOrEmpty(staffName))
                {
                    staffName = HttpUtility.UrlDecode(staffName);
                    this.txtName.Text = staffName;
                }
                this.LoadSolution();
                this.BindMat(staffCode, staffName, shopCode);
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string staffCode, string staffName, string shopCode)
        { 
            this.GridView = gvPos;
            
            int SouleID = 0;
            var strSouleID = ddlSoule.SelectedValue;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            AdBll_OC bll = new AdBll_OC();
            int totalCount = 0;
            decimal totalQty = 0;
           
            var saleData = bll.SelectPersonnelPageList_Wif(this.CurrentPage, this.ItemsPerPage, SouleID, ref totalCount, ref totalQty, staffCode, staffName, shopCode);
            this.DataSource = saleData;
            this.RecordsCount = totalCount;//Convert.ToInt32(salesData["dataCount"] ?? 0);
            gvPos.DataBind();
            this.label_top_totoal_qty.Text = totalQty.ToString();
           
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
            var shopCode = txtShopCode.Text.Trim();
            var staffCode = txtStaffCode.Text.Trim();
            var staffName = txtName.Text.Trim();

            int SouleID = 0;
            if (!string.IsNullOrWhiteSpace(strSouleID))
                Int32.TryParse(strSouleID, out SouleID);
            this.CurrentPage = 1;
           this.BindMat(staffCode, staffName, shopCode);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var strSouleID = ddlSoule.SelectedValue;
                var shopCode = txtShopCode.Text.Trim();
                var staffCode = txtStaffCode.Text.Trim();
                var staffName = txtName.Text.Trim();
                int SouleID = 0;
                if (!string.IsNullOrWhiteSpace(strSouleID))
                    Int32.TryParse(strSouleID, out SouleID);
                var salesData = WebRepository.QueryGatAdPersonnel_Wif(SouleID,staffCode, staffName, shopCode);
                System.Data.DataTable dtSourceStock = salesData.ToDataTable(new string[] { "staff_code", "name", "store_code", "active", "saler", "entry_date", "sex", "age", "position", "stats_date" });
                string fileName = "职员资料.xlsx";
                dtSourceStock.Columns["staff_code"].ColumnName = "店员编号";
                dtSourceStock.Columns["name"].ColumnName = "姓名";
                dtSourceStock.Columns["store_code"].ColumnName = "店铺编号";
                dtSourceStock.Columns["active"].ColumnName = "在职状态";
                dtSourceStock.Columns["saler"].ColumnName = "是否导购";
                dtSourceStock.Columns["entry_date"].ColumnName = "入职时间";
                dtSourceStock.Columns["sex"].ColumnName = "性别";
                dtSourceStock.Columns["age"].ColumnName = "年龄";
                dtSourceStock.Columns["position"].ColumnName = "职位";
                dtSourceStock.Columns["stats_date"].ColumnName = "店员快照日期";
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

        //protected void bthDelSearch_Click(object sender, EventArgs e)
        //{
        //    var strSouleID = ddlSoule.SelectedValue;
        //    var shopCode = txtShopCode.Text.Trim();
        //    var staffCode = txtStaffCode.Text.Trim();
        //    var staffName = txtName.Text.Trim();
        //    try
        //    {
        //        int SouleID = 0;
        //        if (!string.IsNullOrWhiteSpace(strSouleID))
        //            Int32.TryParse(strSouleID, out SouleID);
        //        HandleArgs handle = WebRepository.DeleteAdOutInData_OCTB(SouleID, startBillTime, endBillTime, startBillDate, endBillDate);

        //        if (handle.Status == HandleStatus.Fail)
        //        {
        //            this.Alert(handle.Message, Core.DialogIcon.Error);
        //        }
        //        else
        //        {
        //            this.Alert("删除成功", Core.DialogIcon.Correct);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Alert(ex.Message, Core.DialogIcon.Error);
        //    }
        //    finally
        //    {
        //        this.GridView = gvPos;
        //        var mats = WebRepository.QueryGatAdOutIn_OCTB(this.CurrentPage, this.ItemsPerPage, "");
        //        this.DataSource = mats;
        //        this.RecordsCount = WebRepository.GatAdOutInCount_OCTB("");
        //    }
        //}

        //protected void bthDelChecked_Click(object sender, EventArgs e)
        //{
        //    var strSouleID = ddlSoule.SelectedValue;
        //    string strId = string.Empty;
        //    foreach (GridViewRow gvr in gvPos.Rows)
        //    {
        //        CheckBox cb = (CheckBox)gvr.FindControl("CheckBox1");  //CheckBoxId为选择框的id
        //        if (cb.Checked)
        //        {
        //            strId = strId + "," + gvPos.DataKeys[gvr.RowIndex].Value.ToString();
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(strId))
        //    {
        //        //去除第一个逗号
        //        strId = strId.Substring(1);

        //    }
        //    try
        //    {
        //        int SouleID = 0;
        //        if (!string.IsNullOrWhiteSpace(strSouleID))
        //            Int32.TryParse(strSouleID, out SouleID);
        //        HandleArgs handle = WebRepository.DeleteAdOutInData_OCTB(SouleID, strId);

        //        if (handle.Status == HandleStatus.Fail)
        //        {
        //            this.Alert(handle.Message, Core.DialogIcon.Error);
        //        }
        //        else
        //        {
        //            this.Alert("删除成功", Core.DialogIcon.Correct);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Alert(ex.Message, Core.DialogIcon.Error);
        //    }
        //    finally
        //    {
        //        this.GridView = gvPos;
        //        var mats = WebRepository.QueryGatAdOutIn_OCTB(this.CurrentPage, this.ItemsPerPage, "");
        //        this.DataSource = mats;
        //        this.RecordsCount = WebRepository.GatAdOutInCount_OCTB("");
        //    }
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using System.Text.RegularExpressions;
using Gat.Web.Common;
using System.IO;
using System.Data;
using GatNewBll.Blls;
using GatEntities.DbModels;
using GatNewBll.Models;

namespace Gat.Web.admin
{
    /* 说明：方案明细到品牌
     * 页面接收参数：
     * 1. company = CompanyID，公司ID
     * 2. vendor = VendorID，品牌ID
     * 3. card = CardID
     */

    public partial class gat_solution : Core.PageSingleBase
    {
        #region 公开属性

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID
        {
            get { return (ViewState["CompanyID"] ?? "").ToString(); }
            set { ViewState["CompanyID"] = value; }
        }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string CardID
        {
            get { return (ViewState["CardID"] ?? "").ToString(); }
            set { ViewState["CardID"] = value; }
        }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int VendorID
        {
            get { return Convert.ToInt32(ViewState["VendorID"] ?? "0"); }
            set { ViewState["VendorID"] = value; }
        }
        #endregion

        private int tabSelectIndex = 0;//0.店铺，1仓库
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);

            #region AJAX 请求

            string mode = Request["mode"] ?? "";
            if (this.IsAJAXRequest || !string.IsNullOrEmpty(mode))
            {
                string returnText = string.Empty;
                switch (mode)
                {
                    case "companysave":
                        //this.UpdateSolCompany(Request["solution"], Request["comcode"]);
                        break;

                    case "loadvendor": //为指定的供应商加载提报公司
                        this.LoadVendorForCompany(Request["company"]);
                        break;

                    case "loadcard": //为指定的供应商加载提报公司
                        this.LoadCardForCompanyAndVendor(Request["company"], Request["vendor"]);
                        break;

                    case "queryvs": //搜索供应商店铺
                        this.LoadVendShop(Request["vendor"], Request["shopcode"]);
                        break;
                    case "queryBasShop": //搜索店铺资料
                        this.LoadBasShop(Request["company"], Request["shopcode"], Request["shopMapMultipleSelect"]);
                        break;
                    case "queryvsStock"://搜索供应商仓库
                        this.LoadVendStock(Request["vendor"], Request["shopcode"]);
                        break;
                    case "savemap": //保存店铺与供应商店铺的关联信息
                        this.SaveShopMap(
                            Request["vendor"],
                            Request["company"],
                            Request["shop"],
                            Request["oldmap"],
                            Request["newmap"],
                            Request["card"],
                            Request["searchcode"]
                            );
                        break;
                    case "saveBasShopRightDS": //保存店铺与供应商店铺的关联信息
                        this.SaveBasShopRightDS(
                            Request["shopRightID"],
                            Request["vendor"],
                            Request["company"],
                            Request["shop"],
                            Request["oldmap"],
                            Request["newmap"],
                            Request["card"],
                            Request["searchcode"]
                            );
                        break;
                    case "saveStockMap"://保存仓库与供应商的关联关系
                        this.SaveStockMap(
                            Request["vendor"],
                            Request["company"],
                            Request["stock"],
                            Request["oldmap"],
                            Request["newmap"],
                            Request["card"],
                            Request["searchcode"]);
                        break;
                    case "queryright": //加载提报的店铺数据集
                        this.LoadShopRight(
                            Request["company"],
                            Request["vendor"],
                            Request["card"],
                            Request["shopcode"]);
                        break;
                    case "SelectShopData":
                        this.LoadMainShopRight(
                            Request["companyID"],
                            Request["vendorID"],
                            Request["cardID"],
                            Request["searchcode"]);
                        break;
                    case "SelectStockData":
                        this.LoadMainStockRight(
                           Request["companyID"],
                           Request["vendorID"],
                           Request["cardID"],
                           Request["searchcode"]);
                        break;
                    case "materialRangeImportFile":
                        byte[] fileArray = null;
                        string fileName = "";
                        returnText = this.MaterialRangeImportFile(ref fileArray, ref fileName);
                        if (returnText == "success")
                        {
                            Response.StatusCode = 200;
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray);
                            this.Response.End();
                        }
                        break;
                    case "materialRangeImport":
                        HttpPostedFile file1 = Context.Request.Files["file"];
                        string fileFullName = FileHelper.uploadFile(file1, "~/FileUpload/");  //这里引用的是上面封装的方法
                        int solutionId = Convert.ToInt32(Request["solutionId"]);
                        MaterialRangeImport(fileFullName, solutionId);
                        break;
                    case "specialMaterialRangeImportFile":
                        byte[] fileArray2 = null;
                        string fileName2 = "";
                        returnText = this.SpecialMaterialRangeImportFile(ref fileArray2, ref fileName2);
                        if (returnText == "success")
                        {
                            Response.StatusCode = 200;
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName2);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray2);
                            this.Response.End();
                        }
                        break;
                    case "specialMaterialRangeImport":
                        HttpPostedFile file2 = Context.Request.Files["file"];
                        string fileFullName2 = FileHelper.uploadFile(file2, "~/FileUpload/");  //这里引用的是上面封装的方法
                        int solutionId2 = Convert.ToInt32(Request["solutionId"]);
                        SpecialMaterialRangeImport(fileFullName2, solutionId2);
                        break;

                    case "materialDiscountImportFile":
                        byte[] fileArray3 = null;
                        string fileName3 = "";
                        returnText = this.MaterialDiscountImportFile(ref fileArray3, ref fileName3);
                        if (returnText == "success")
                        {
                            Response.StatusCode = 200;
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName3);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray3);
                            this.Response.End();
                        }
                        break;
                    case "materialDiscountImport":
                        HttpPostedFile file3 = Context.Request.Files["file"];
                        string fileFullName3 = FileHelper.uploadFile(file3, "~/FileUpload/");  //这里引用的是上面封装的方法
                        int solutionId3 = Convert.ToInt32(Request["solutionId"]);
                        MaterialDiscountImport(fileFullName3, solutionId3);
                        break;
                    case "stockSubModelSetImportFile": 
                        byte[] fileArray4 = null;
                        string fileName4 = "";
                        returnText = this.StockSubModelSetImportFile(ref fileArray4, ref fileName4);
                        if (returnText == "success")
                        {
                            Response.StatusCode = 200;
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName4);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray4);
                            this.Response.End();
                        }
                        break;
                    case "stockSubModelSetImport":
                        HttpPostedFile file4 = Context.Request.Files["file"];
                        string fileFullName4 = FileHelper.uploadFile(file4, "~/FileUpload/");  //这里引用的是上面封装的方法
                        int solutionId4 = Convert.ToInt32(Request["solutionId"]);
                        StockSubModelSetImport(fileFullName4, solutionId4);
                        break;

                    case "OnlineShopSetImportFile":
                        byte[] fileArray5 = null;
                        string fileName5 = "";
                        returnText = this.OnlineShopSetImportFile(ref fileArray5, ref fileName5);
                        if (returnText == "success")
                        {
                            Response.StatusCode = 200;
                            this.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName5);
                            this.Response.ContentType = "application/vnd.ms-excel";
                            this.Response.BinaryWrite(fileArray5);
                            this.Response.End();
                        }
                        break;
                    case "OnlineShopSetImport":
                        HttpPostedFile file5 = Context.Request.Files["file"];
                        string fileFullName5 = FileHelper.uploadFile(file5, "~/FileUpload/");  //这里引用的是上面封装的方法
                        int solutionId5 = Convert.ToInt32(Request["solutionId"]);
                        OnlineShopSetImport(fileFullName5, solutionId5);
                        break;
                }
                return;
            }

            #endregion

            #region 正常GET、POS请求

            if (!IsPostBack)
            {
                TopTitle = "提报方案";
                SubTitle = this.CUID == Core.CUID.Create ? "新增" : "修改";
                this.txtRepeatStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");       //指定的开始时间
                this.txtStartTime.Text = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss"); //补采开始时间
                this.txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");               // 补采结束时间
                txtShopCode.Text = HttpUtility.UrlDecode(Request["shopcode"]);
                //绑定供应商店铺资料
                if (this.CUID == Core.CUID.Create)
                {
                    this.CreateLoad();
                }
                else
                {
                    this.UpdateLoad();
                }



            }

            #endregion
        }

        protected void btnDoRefreshMaterialRangeGrid_Click(object sender, EventArgs e)
        {
            this.LoadMaterialRange(this.IdentityKey);
        }

        protected void btnDoRefreshMaterialDiscountGrid_Click(object sender, EventArgs e)
        {
            this.LoadMaterialDiscount(this.IdentityKey);
        }
       
        protected void btnDoRefreshSpecialMaterialRangeGrid_Click(object sender, EventArgs e)
        {
            this.LoadMaterialRange(this.IdentityKey);
        }

        #region 辅助方法

        /// <summary>
        /// 绑定基础资料中的公司
        /// </summary>
        private void BindBasCompany()
        {
            //绑定基础资料中的公司表格
            //List<CompanyB> dataSource = WebRepository.QueryCompanies();
            SolutionBll bll = new SolutionBll();
            var dataSource = bll.GetAllCompany();
            dataSource.ForEach(s => s.CompanyName = s.CompanyID + "-" + s.CompanyName);
            ddlCompany.DataSource = dataSource;
            ddlCompany.DataTextField = "CompanyName";
            ddlCompany.DataValueField = "CompanyID";
            ddlCompany.DataBind();
        }

        /// <summary>
        /// 呈现控件，输出 HTML
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private string RenderControl(Control control)
        {
            StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            control.RenderControl(htmlWriter);
            return stringWriter.ToString();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }

        /// <summary>
        /// 设置页面相关控件的状态
        /// </summary>
        /// <param name="state"></param>
        private void SetControlState(bool state)
        {
            this.gvShop.Visible = false;
            this.gvStock.Visible = false;
            this.btnSave.Visible = state;
            this.btnExport.Visible = state;
            this.btnSearch.Visible = state;
            this.ddlCompany.Enabled = state;
            this.ddlCard.Enabled = state;
            this.ddlVendor.Enabled = state;
        }

        /// <summary>
        /// 绑定店铺权限
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="vendorID"></param>
        /// <param name="cardID"></param>
        private void BindShopRight(string companyID, int vendorID, string cardID, string shopCode = "")
        {
            //List<ShopRightB> dataSource = WebRepository.QueryShopRights(vendorID, cardID, companyID, shopCode);
            SolutionBll bll = new SolutionBll();
            var shoprights = bll.GetShopRight(companyID, vendorID, cardID, shopCode);
            gvShop.DataSource = shoprights;
            gvShop.DataBind();
            gvShop.CssClass = shoprights.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            //List<StockRightB> dataStockSource = WebRepository.QueryStockRights(vendorID, cardID, companyID);
            var stockRights = bll.GetStockRight(companyID, vendorID, cardID);
            gvStock.DataSource = stockRights;
            gvStock.DataBind();
            gvStock.CssClass = stockRights.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 加载不上传货品范围
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadMaterialRange(int solutionId)
        {
            List<GatMaterialRangeB> data = WebRepository.GetMaterialRanges(solutionId);
            if (data == null)
            {
                data = new List<GatMaterialRangeB>();
            }
            gvMaterialRange.DataSource = data;
            gvMaterialRange.DataBind();
            gvMaterialRange.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 加载货品折扣
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadMaterialDiscount(int solutionId)
        {
            List<GatMaterialDiscountB> data = WebRepository.GetMaterialDiscount(solutionId);
            if (data == null)
            {
                data = new List<GatMaterialDiscountB>();
            }
            gvMaterialDiscount.DataSource = data;
            gvMaterialDiscount.DataBind();
            gvMaterialDiscount.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 加载特殊货品范围
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadSpecialMaterialRange(int solutionId)
        {
            List<GatSpecialMaterialRangeB> data = WebRepository.GetSpecialMaterialRanges(solutionId);
            if (data == null)
            {
                data = new List<GatSpecialMaterialRangeB>();
            }
            gvSpecialMaterialRange.DataSource = data;
            gvSpecialMaterialRange.DataBind();
            gvSpecialMaterialRange.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        private void BindStock()
        {
            string companyID = this.CompanyID;
            int vendor = this.VendorID;
            string card = this.CardID; ;
            string shopCode = txtShopCode.Text;
            List<StockRightB> dataStockSource = WebRepository.QueryStockRights(vendor, card, companyID, shopCode);
            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(card))
                return;
            Dictionary<string, string> sort = ViewState["SortedField"] as Dictionary<string, string>;
            foreach (var s in sort)
            {
                string s1 = s.Key.ToString() + "" + s.Value;
                if (s.Value == "DESC")
                {
                    dataStockSource = dataStockSource.OrderByDescending(p => GetPropertyValue(p, s.Key.ToString())).ToList();
                }
                else
                {
                    dataStockSource = dataStockSource.OrderBy(p => GetPropertyValue(p, s.Key.ToString())).ToList();
                }
            }
            gvStock.DataSource = dataStockSource;
            gvStock.DataBind();
            gvStock.CssClass = dataStockSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }
        private void BindShopSort()
        {

            string companyID = this.CompanyID;
            int vendor = this.VendorID;
            string card = this.CardID; ;
            string shopCode = txtShopCode.Text;

            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(card))
                return;

            //var  sortField=ViewState["SortedField"];
            Dictionary<string, string> sort = ViewState["SortedField"] as Dictionary<string, string>;
            List<ShopRightB> dataSource = WebRepository.QueryShopRights(vendor, card, companyID, HttpUtility.UrlDecode(shopCode ?? ""));
            foreach (var s in sort)
            {
                string s1 = s.Key.ToString() + "" + s.Value;
                if (s.Value == "DESC")
                {
                    dataSource = dataSource.OrderByDescending(p => GetPropertyValue(p, s.Key.ToString())).ToList();
                }
                else
                {
                    dataSource = dataSource.OrderBy(p => GetPropertyValue(p, s.Key.ToString())).ToList();
                }
            }

            gvShop.DataSource = dataSource;
            gvShop.DataBind();
            gvShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }
        public object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }

        private void BindMainShopRight(string companyID, int vendorID, string cardID, string shopCode = "")
        {
            //List<ShopRightB> dataSource = WebRepository.QueryShopRights(vendorID, cardID, companyID, shopCode);

            SolutionBll bll = new SolutionBll();
            var shoprights = bll.GetShopRight(companyID, vendorID, cardID, shopCode);
            gvShop.DataSource = shoprights;
            gvShop.DataBind();
            gvShop.CssClass = shoprights.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        private void BindMainStockRight(string companyID, int vendorID, string cardID, string stockCode = "")
        {
            //List<StockRightB> dataStockSource = WebRepository.QueryStockRights(vendorID, cardID, companyID, stockCode);

            SolutionBll bll = new SolutionBll();
            var stockRights = bll.GetStockRight(companyID, vendorID, cardID, stockCode);
            gvStock.DataSource = stockRights;
            gvStock.DataBind();
            gvStock.CssClass = stockRights.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 在页面处于编辑状态时初始化页面上的下拉列表控件
        /// </summary>
        /// <param name="solution"></param>
        private void InitDropDownList(SolutionModel solution)
        {
            //改变下拉列表的DataTextField和DataValueField
            //DropDownList[] dropDowns = new DropDownList[] { ddlCompany, ddlCard, ddlVendor };
            //foreach (DropDownList drop in dropDowns)
            //{
            //    drop.DataTextField = "Value";
            //    drop.DataValueField = "Key";
            //}

            //公司下拉列表
            var company = new List<CompanyB> { new CompanyB { CompanyID = solution.CompanyID, CompanyName = solution.CompanyID + "-" + solution.CompanyName } };
            ddlCompany.DataSource = company;
            ddlCompany.DataBind();

            //品牌下拉列表
            var card = new List<CardB> { new CardB { CardID = solution.CardID, CardName = solution.CardName } };
            ddlCard.DataSource = card;
            ddlCard.DataBind();

            //供应商下拉列表
            var vendor = new List<VendorB> { new VendorB { ID = solution.VendorID ?? 0, VendorName = solution.VendorName } };
            ddlVendor.DataSource = vendor;
            ddlVendor.DataBind();

            //this.BindShopRight(solution.CompanyID, solution.VendorID ?? 0, solution.CardID);
            //this.BindVendShop(solution.VendorID ?? -1);

        }

        /// <summary>
        /// 绑定供应商下拉列表
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="shopCode"></param>
        private void BindVendShop(int vendorID, string shopCode = "")
        {
            List<VendorShopB> dataSource = WebRepository.QueryVendorShops(vendorID, shopCode);
            gvVendShop.DataSource = dataSource;
            gvVendShop.DataBind();

            this.gvVendStock.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";


            List<VendorShopB> dataSourceStock = WebRepository.QueryVendorShops(vendorID, "");
            gvVendStock.DataSource = dataSourceStock;
            gvVendStock.DataBind();

            this.gvVendStock.CssClass = dataSourceStock.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 绑定店铺下拉列表
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="shopCode"></param>
        private void BindBasShop(string companyID, string shopCode = "", string shopMapMultipleSelect = "")
        {
            List<ShopB> dataSource = WebRepository.QueryBasShops(companyID, shopCode);
            if(!string.IsNullOrWhiteSpace(shopMapMultipleSelect))
            {
                var shopMapMultipleSelectList = shopMapMultipleSelect.Split(',');
                dataSource = dataSource.OrderBy(o => !shopMapMultipleSelectList.Contains(o.ShopID)).ToList();
            }
            gvBasShop.DataSource = dataSource;
            gvBasShop.DataBind();
            this.gvBasShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }


        private void BindVendStock(int vendorID, string stockCode = "")
        {
            List<VendorShopB> dataSourceStock = WebRepository.QueryVendorStocks(vendorID, stockCode);
            gvVendStock.DataSource = dataSourceStock;
            gvVendStock.DataBind();

            this.gvVendStock.CssClass = dataSourceStock.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 绑定店铺资料下拉列表 （疑似没有用注释掉）
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="shopCode"></param>
        //private void BindBasShop(int vendorID, string shopCode = "")
        //{
        //    List<VendorShopB> dataSource = WebRepository.QueryVendorShops(vendorID, shopCode);
        //    gvVendShop.DataSource = dataSource;
        //    gvVendShop.DataBind();

        //    this.gvVendStock.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";


        //    List<VendorShopB> dataSourceStock = WebRepository.QueryVendorShops(vendorID, "");
        //    gvVendStock.DataSource = dataSourceStock;
        //    gvVendStock.DataBind();

        //    this.gvVendStock.CssClass = dataSourceStock.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        //}


        #endregion

        #region AJAX 处理

        /// <summary>
        /// 为指定的公司加载供应商（指定的公司能为哪些供应商提报数据）
        /// </summary>
        /// <param name="companyID"></param>
        private void LoadVendorForCompany(string companyID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【COMPANY】";
                this.ResponseToClient(ai);
            }

            List<VendorB> dataSource = WebRepository.QueryVendorForCompany(companyID);
            ddlVendor.DataSource = dataSource;
            ddlVendor.DataBind();
            ddlVendor.Items.Insert(0, new ListItem("--请选择供应商--", "0"));

            //清空品牌
            ddlCard.DataSource = null;
            ddlCard.DataBind();

            string html = this.RenderControl(ddlVendor);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = this.RenderControl(ddlCard); ;
            //if (dataSource.Count == 0)
            //{
            //    ai.StatusCode = "1";
            //    ai.Message = "没有";
            //}
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 为指定的供应商和公司加载品牌
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="vendorID"></param>
        private void LoadCardForCompanyAndVendor(string companyID, string vendorID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【COMPANY|VENDOR】";
                this.ResponseToClient(ai);
            }

            List<CardB> dataSource = WebRepository.QueryCardForCompanyAndVendor(companyID, Convert.ToInt32(vendorID));
            ddlCard.DataSource = dataSource;
            ddlCard.DataBind();
            ddlCard.Items.Insert(0, new ListItem("--请选择品牌--", "0"));

            string html = this.RenderControl(ddlCard);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            //if (dataSource.Count == 0)
            //{
            //    ai.StatusCode = "1";
            //    ai.Message = "没有";
            //}
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 为指定的供应商和公司加载品牌
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="vendorID"></param>
        private void LoadVendShop(string vendorID, string shopCode)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR】";
                this.ResponseToClient(ai);
            }
            this.BindVendShop(Convert.ToInt32(vendorID), HttpUtility.UrlDecode(shopCode ?? ""));

            string html = this.RenderControl(gvVendStock);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }


        /// <summary>
        /// 为指定的公司加载店铺资料
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="shopCode"></param>
        private void LoadBasShop(string companyID, string shopCode, string shopMapMultipleSelect)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR】";
                this.ResponseToClient(ai);
            }
            this.BindBasShop(HttpUtility.UrlDecode(companyID ?? ""), HttpUtility.UrlDecode(shopCode ?? ""), HttpUtility.UrlDecode(shopMapMultipleSelect ?? ""));

            string html = this.RenderControl(gvBasShop);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 为指定的供应商和公司加载仓库
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="StopCode"></param>
        private void LoadVendStock(string vendorID, string StockCode)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR】";
                this.ResponseToClient(ai);
            }

            this.BindVendStock(Convert.ToInt32(vendorID), HttpUtility.UrlDecode(StockCode ?? ""));

            string html = this.RenderControl(gvVendStock);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }


        /// <summary>
        /// 保存店铺关联
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        /// <param name="oldMapID">之前关联的供应商店铺ID</param>
        /// <param name="newMapID">新关联的供应商店铺ID</param>
        /// <param name="newMapID">公司店铺的查询字符</param>
        /// <param name="cardID">品牌ID</param>
        private void SaveShopMap(string vendorID, string companyID, string shopID, string oldMapID, string newMapID, string cardID, string searchCode = "")
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(shopID) || string.IsNullOrEmpty(oldMapID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR|SHOP】";
                this.ResponseToClient(ai);
            }

            HandleArgs HANDLE = WebRepository.MapShop(Convert.ToInt32(vendorID), companyID, shopID, Convert.ToInt32(oldMapID), newMapID);
            if (HANDLE.Status == HandleStatus.Fail)
            {
                ai.StatusCode = "2";
                ai.Html = HANDLE.Message;
                this.ResponseToClient(ai);
            }

            this.BindShopRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(searchCode ?? ""));

            string html = this.RenderControl(gvShop);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 保存电商店铺关联
        /// </summary>
        /// <param name="shopRightID">当前行ID</param>
        /// <param name="vendorID">供应商</param>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        /// <param name="oldMapID">之前关联的供应商店铺ID</param>
        /// <param name="newMapID">新关联的供应商店铺ID</param>
        /// <param name="newMapID">公司店铺的查询字符</param>
        /// <param name="cardID">品牌ID</param>
        private void SaveBasShopRightDS(string shopRightID, string vendorID, string companyID, string shopID, string oldMapID, string newMapID, string cardID, string searchCode = "")
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(shopID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR|SHOP】";
                this.ResponseToClient(ai);
            }

            HandleArgs HANDLE = WebRepository.SaveShopRightDS(Convert.ToInt32(shopRightID), companyID, shopID, oldMapID, newMapID);
            if (HANDLE.Status == HandleStatus.Fail)
            {
                ai.StatusCode = "2";
                ai.Html = HANDLE.Message;
                this.ResponseToClient(ai);
            }

            this.BindShopRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(searchCode ?? ""));

            string html = this.RenderControl(gvShop);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }



        /// <summary>
        /// 保存仓库关联
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        /// <param name="oldMapID">之前关联的供应商店铺ID</param>
        /// <param name="newMapID">新关联的供应商店铺ID</param>
        /// <param name="newMapID">公司店铺的查询字符</param>
        /// <param name="cardID">品牌ID</param>
        private void SaveStockMap(string vendorID, string companyID, string stockID, string oldMapID, string newMapID, string cardID, string searchCode = "")
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(stockID) || string.IsNullOrEmpty(oldMapID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【VENDOR|SHOP】";
                this.ResponseToClient(ai);
            }

            //保存关联数据
            HandleArgs HANDLE = WebRepository.MapStock(Convert.ToInt32(vendorID), companyID, stockID, Convert.ToInt32(oldMapID), newMapID);
            if (HANDLE.Status == HandleStatus.Fail)
            {
                ai.StatusCode = "2";
                ai.Html = HANDLE.Message;
                this.ResponseToClient(ai);
            }

            this.BindMainStockRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(searchCode ?? ""));

            string html = this.RenderControl(gvStock);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = html;
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 加载提报的店铺集合
        /// </summary>
        /// <param name="companyID">公司</param>
        /// <param name="vendorID">供应商</param>
        /// <param name="cardID">品牌</param>
        /// <param name="shopCode">店铺代码</param>
        private void LoadShopRight(string companyID, string vendorID, string cardID, string shopCode)
        {
            //System.Threading.Thread.Sleep(100000000);
            if (NullList.Contains(companyID)) companyID = null;
            if (NullList.Contains(vendorID)) vendorID = null;
            if (NullList.Contains(cardID)) cardID = null;
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(cardID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【COMPANY|VENDOR|BRAND】";
                this.ResponseToClient(ai);
            }

            this.BindShopRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(shopCode ?? ""));
            this.BindVendShop(Convert.ToInt32(vendorID));
            string html = this.RenderControl(gvShop);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = this.RenderControl(gvVendShop);
            this.ResponseToClient(ai);
        }


        /// <summary>
        /// 加载提报的店铺集合
        /// </summary>
        /// <param name="companyID">公司</param>
        /// <param name="vendorID">供应商</param>
        /// <param name="cardID">品牌</param>
        /// <param name="shopCode">店铺代码</param>
        private void LoadMainShopRight(string companyID, string vendorID, string cardID, string shopCode)
        {
            //System.Threading.Thread.Sleep(100000000);
            if (NullList.Contains(companyID)) companyID = null;
            if (NullList.Contains(vendorID)) vendorID = null;
            if (NullList.Contains(cardID)) cardID = null;
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(cardID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【COMPANY|VENDOR|BRAND】";
                this.ResponseToClient(ai);
            }

            this.BindMainShopRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(shopCode ?? ""));
            this.BindVendShop(Convert.ToInt32(vendorID));
            string html = this.RenderControl(gvShop);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = this.RenderControl(gvVendShop);
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 加载提报的仓库集合
        /// </summary>
        /// <param name="companyID">公司</param>
        /// <param name="vendorID">供应商</param>
        /// <param name="cardID">品牌</param>
        /// <param name="shopCode">仓库代码</param>
        private void LoadMainStockRight(string companyID, string vendorID, string cardID, string shopCode)
        {
            //System.Threading.Thread.Sleep(100000000);
            if (NullList.Contains(companyID)) companyID = null;
            if (NullList.Contains(vendorID)) vendorID = null;
            if (NullList.Contains(cardID)) cardID = null;
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(companyID) || string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(cardID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【COMPANY|VENDOR|BRAND】";
                this.ResponseToClient(ai);
            }

            this.BindMainStockRight(companyID, Convert.ToInt32(vendorID), cardID, HttpUtility.UrlDecode(shopCode ?? ""));
            this.BindVendShop(Convert.ToInt32(vendorID));
            string html = this.RenderControl(gvStock);
            ai.StatusCode = "1";
            ai.Html = html;
            ai.Message = this.RenderControl(gvVendStock);
            this.ResponseToClient(ai);
        }

        #endregion

        #region 功能按钮

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
            //List<ShopRightB> shopRightUpdate = new List<ShopRightB>();
            int ShopNumberSelect = 0;
            List<GatShopright> shopRightUpdates = new List<GatShopright>();
            foreach (GridViewRow row in gvShop.Rows)
            {
                CheckBox cbAllowUsed = (CheckBox)row.FindControl("chkSelect");
                CheckBox cbDefaultDiscount = (CheckBox)row.FindControl("chkDiscount");
                TextBox txtDiscount = (TextBox)row.FindControl("txtDiscount");
                TextBox txtTradeCode = (TextBox)row.FindControl("txtTradeCode");
                TextBox txtTradeCode2 = (TextBox)row.FindControl("txtTradeCode2");
                CheckBox cbIsNoCalculationSale = (CheckBox)row.FindControl("chkIsNoCalculationSale");
                CheckBox cbIsECSale = (CheckBox)row.FindControl("chkIsECSale");
                var shopRightId = gvShop.DataKeys[row.RowIndex].Value;
                shopRightUpdates.Add(new GatShopright()
                {
                    Id = Convert.ToInt32(shopRightId),
                    AllowUsed = cbAllowUsed.Checked,
                    DefaultDiscount = cbDefaultDiscount.Checked,
                    Discount = Convert.ToDecimal(txtDiscount.Text.Trim()),
                    TradeCode = txtTradeCode.Text.Trim(),
                    TradeCode2 = txtTradeCode2.Text.Trim(),
                    IsNoCalculationSale = cbIsNoCalculationSale.Checked,
                    IsECSale= cbIsECSale.Checked,
                });
                //shopRightUpdate.Add(new ShopRightB
                //{
                //    ShopID = shopId.ToString(),
                //    AllowUsed = cbAllowUsed.Checked,
                //    DefaultDiscount = cbDefaultDiscount.Checked,
                //    Discount = Convert.ToDecimal(txtDiscount.Text.Trim()),
                //    TradeCode = txtTradeCode.Text
                //});
                if (cbAllowUsed.Checked)
                {
                    ShopNumberSelect = ShopNumberSelect + 1;
                }
            }
            List<GatStockright> stockRightUpdates = new List<GatStockright>();
            foreach (GridViewRow row in gvStock.Rows)
            {
                CheckBox cbAllowUsed = (CheckBox)row.FindControl("chkStockSelect");
                TextBox txtTradeCode = (TextBox)row.FindControl("txtStockTradeCode");
                var stockRightId = gvStock.DataKeys[row.RowIndex].Value;
                stockRightUpdates.Add(new GatStockright
                {
                    Id = Convert.ToInt32(stockRightId),
                    AllowUsed = cbAllowUsed.Checked,
                    TradeCode = txtTradeCode.Text.Trim()
                });
            }
            string company = hfCompany.Value;
            string card = hfCard.Value;
            int vendor = Convert.ToInt32(hfVendor.Value);
            HandleArgs HANDLE = null;
            //允许提报的店铺

            //拆分数据，被选中项和全部项，数据有符号“|”隔开
            //店铺
            string[] shopdataSplit = hfallowList.Value.Split('|');
            string shopList = shopdataSplit[0];
            string shopallList = shopdataSplit[1];
            //仓库
            string[] stockdataSplit = hfStockList.Value.Split('|');
            string stockList = stockdataSplit[0];//选中项
            string stockAllList = stockdataSplit[1];//全部
            //var hfDefaultDiscount = hfDefaultDiscountList.Value;
            //var hfDiscount = hfDiscountList.Value;

            List<string> allowList = shopList.Split(',').ToList();
            List<string> shopListSelect = shopallList.Split(',').ToList();


            List<string> allowListStock = stockList.Split(',').ToList();
            List<string> stockListSelect = stockAllList.Split(',').ToList();


            string shopCode = txtShopCode.Text.Trim();

            SolutionB solution = WebRepository.FetchSolution(company, vendor, card);
            if (solution != null)
            {
                if (solution.ID != this.IdentityKey) //检查指定的公司、供应商、品牌是否已经有提报方案
                {
                    this.Alert("已经存在【" + solution.CompanyName + "】为【" + solution.VendorName + "】【" + solution.CardName + "】提供的提报方案！", Core.DialogIcon.Warning);
                    //还原下拉列表状态
                    this.RevertPage(company, vendor, card, allowList, allowListStock);
                    return;
                }
            }
            solution = this.CUID == Core.CUID.Create ? new SolutionB() : WebRepository.FetchSolution(this.IdentityKey);
            if (solution == null)
            {
                this.Alert("保存失败，消息：不存在指定的提报方案！", Core.DialogIcon.Warning);
                return;
            }
            VendorB vendorB = WebRepository.FetchVendor(vendor);
            if (vendorB != null)
            {
                int ShopNumberCount = WebRepository.VendorShopNumberCount(vendor, card, 0);//已经设置店铺数量
                if (ShopNumberCount + ShopNumberSelect > vendorB.ShopNumber)
                {
                    this.Alert("已超过店铺数上限，不可保存，请联系客服！", Core.DialogIcon.Warning);
                    return;
                }
            }
            int SalesAmountType = 0;// --销售金额取数 0 结算金额  1 ActualAmount实际结算金额
            if (this.cbActualAmount.Checked)
            {
                SalesAmountType = 1;
            }
            //提报方式
            GatSolution dbSolution = new GatSolution()
            {
                Id = solution.ID,
                IsAuto = this.cbAuto.Checked,
                IsBackground = this.cbBackgroud.Checked,
                CronExpression = this.txtCron.Text.Trim(),
                GatType = Convert.ToByte(GatType.FromDatabase),
                RepeatInterval = null,
                Enabled = this.chkEnabled.Checked,
                VendorID = vendor,
                CardID = card,
                CompanyID = company,
                Directory = this.txtDictory.Text,
                ManualInterval = solution.ManualGatEndTime.Subtract(solution.ManualGatStartTime).Days,
                RepeatStartTime = Convert.ToDateTime(txtRepeatStartTime.Text),
                PastVersionVendorID = string.Empty,
                Title = solution.CompanyID + "&" + solution.VendorID + "&" + solution.CardID,
                ManualGatStartTime = DateTime.Now.AddDays(-2),
                ManualGatEndTime = DateTime.Now,
                SalesAmountType = SalesAmountType,
            };
            //solution.IsAuto = this.cbAuto.Checked;
            //solution.GatType = GatType.FromDatabase;
            //solution.RepeatInterval = null;
            //solution.Enabled = this.chkEnabled.Checked;
            //solution.VendorID = vendor;
            //solution.CardID = card;
            //solution.CompanyID = company;
            //solution.Directory = string.Empty;
            //solution.ManualInterval = solution.ManualGatEndTime.Subtract(solution.ManualGatStartTime).Days;
            //solution.RepeatStartTime = Convert.ToDateTime(txtRepeatStartTime.Text);
            //solution.Directory = string.Empty;
            //solution.PastVersionVendorID = string.Empty;
            //solution.Title = solution.CompanyID + "&" + solution.VendorID + "&" + solution.CardID;
            //solution.ManualGatStartTime = DateTime.Now.AddDays(-2);
            //solution.ManualGatEndTime = DateTime.Now;


            //HANDLE = this.CUID == Core.CUID.Create ?
            //    WebRepository.InsertSolution(solution, shopRightUpdate, allowList, allowListStock, shopListSelect, stockListSelect, shopCode) :
            //    WebRepository.UpdateSolution(solution, shopRightUpdate, allowList, allowListStock, shopListSelect, stockListSelect, shopCode); //插入或者更新记录
            SolutionBll bll = new SolutionBll();

            string message = string.Empty;
            string callback = string.Empty;
            Core.DialogIcon icon = Core.DialogIcon.Correct;
            try
            {
                string[] ignoreCloumns = { "AppKey", "ExpiryDate" };
                var solutionResult = bll.InsertOrUpdateSolution(dbSolution, out int id, ignoreCloumns);
                //更新店铺和仓库信息
                //"AllowUsed","DefaultDiscount","Discount","TradeCode"
                string[] shopRightUpdateColumns = { "AllowUsed", "DefaultDiscount", "Discount", "TradeCode", "TradeCode2", "IsNoCalculationSale", "IsECSale" };
                bll.UpdateShopRight(shopRightUpdates, shopRightUpdateColumns);
                string[] stockRightUpdateColumns = { "AllowUsed", "TradeCode" };
                bll.UpdateStockRight(stockRightUpdates, stockRightUpdateColumns);

                //保存客户
                SaveVendCust();
                if (solutionResult)
                {
                    message = "保存提报方案成功！";
                }
                if (id > 0)
                {
                    callback = $"location.href='{this.GetRouteUrl("solution-edit", new { id = id })}?shopcode={HttpUtility.UrlEncode(shopCode)}'";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                icon = Core.DialogIcon.Warning;
            }

            //string message = HANDLE.Status == HandleStatus.Completed ? "保存提报方案成功！" : HANDLE.Message;
            //string callback = HANDLE.Status == HandleStatus.Completed && this.CUID == Core.CUID.Create ?
            //        string.Format("location.href='{0}?shopcode={1}'", this.GetRouteUrl("solution-edit", new { id = solution.ID }), HttpUtility.UrlEncode(shopCode)) : string.Empty;
            //Core.DialogIcon icon = HANDLE.Status == HandleStatus.Completed ? Core.DialogIcon.Correct : Core.DialogIcon.Warning;

            this.Alert(message, icon, callback); //弹出提示框
            if (icon == Core.DialogIcon.Warning)
            {
                this.RevertPage(solution.CompanyID, solution.VendorID ?? 0, solution.CardID, allowList, allowListStock, shopCode);
            }
            if (dbSolution.Id > 0)
            {
                this.BindShopRight(solution.CompanyID, solution.VendorID ?? 0, solution.CardID); //如果是更新，不管成功失败都要重新绑定店铺
                this.LoadVendCust(solution.ID);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindShopRight(this.CompanyID, this.VendorID, this.CardID, txtShopCode.Text);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExport_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("/Files");
            string indexName = tabStripSelect.Value.ToString();
            tabSelectIndex = indexName.Contains("店铺") == true ? 0 : 1;
            switch (tabSelectIndex.ToString())
            {
                case "1":  //导出仓库excel
                    {
                        string fileNameStock = "【" + ddlCompany.SelectedItem.Text + "】" +
                                           "【" + ddlVendor.SelectedItem.Text + "】" +
                                           "【" + ddlCard.SelectedItem.Text + "】的提报仓库.xls";
                        fileNameStock = System.IO.Path.Combine(path, fileNameStock);
                        if (File.Exists(fileNameStock)) File.Delete(fileNameStock);

                        try
                        {
                            List<StockRightB> dataSourceStock = WebRepository.QueryStockRights(this.VendorID, this.CardID, this.CompanyID, true);
                            System.Data.DataTable dtSourceStock = dataSourceStock.AsQueryable().ToDataTable(new string[] { "StockID", "StockCode", "StockName", "VendorStockCode", "VendorStockName" });
                            dtSourceStock.Columns["StockID"].Caption = "仓库内码";
                            dtSourceStock.Columns["StockCode"].Caption = "仓库代码";
                            dtSourceStock.Columns["StockName"].Caption = "仓库名称";
                            dtSourceStock.Columns["VendorStockCode"].Caption = "供应商仓库代码";
                            dtSourceStock.Columns["VendorStockName"].Caption = "供应商仓库名称";
                            ExcelHelper Excel = new ExcelHelper();
                            Excel.ExportExcel(dtSourceStock, fileNameStock);
                            FileHelper.DownloadFile(fileNameStock, true);
                        }
                        catch (Exception ex)
                        {
                            this.Alert(ex.Message, Core.DialogIcon.Warning);
                        }
                    }
                    break;
                default: //默认都是导出店铺的
                    {
                        string fileName = "【" + ddlCompany.SelectedItem.Text + "】" +
                         "【" + ddlVendor.SelectedItem.Text + "】" +
                         "【" + ddlCard.SelectedItem.Text + "】的提报店铺.xls";
                        fileName = System.IO.Path.Combine(path, fileName);
                        if (File.Exists(fileName)) File.Delete(fileName);
                        try
                        {
                            List<ShopRightB> dataSource = WebRepository.QueryShopRights(this.VendorID, this.CardID, this.CompanyID, true);
                            System.Data.DataTable dtSource = dataSource.AsQueryable().ToDataTable(new string[] { "ShopID", "ShopCode", "ShopName", "VendorShopCode", "VendorShopName" });
                            dtSource.Columns["ShopID"].Caption = "店铺内码";
                            dtSource.Columns["ShopCode"].Caption = "店铺代码";
                            dtSource.Columns["ShopName"].Caption = "店铺名称";
                            dtSource.Columns["VendorShopCode"].Caption = "供应商店铺代码";
                            dtSource.Columns["VendorShopName"].Caption = "供应商店铺名称";

                            ExcelHelper Excel = new ExcelHelper();
                            Excel.ExportExcel(dtSource, fileName);
                            FileHelper.DownloadFile(fileName, true);
                        }
                        catch (Exception ex)
                        {
                            this.Alert(ex.Message, Core.DialogIcon.Warning);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 还原页面，在页面中很多记录是用AJAX做的，所以需要还原
        /// </summary>
        void RevertPage(string company, int vendor, string card, List<string> allowList, List<string> allowListStock, string shopCode = "")
        {
            //还原公司下拉列表状态
            ControlHelper.SetSelectedItem(ddlCompany, company);
            //供应商
            List<VendorB> dataSource = WebRepository.QueryVendorForCompany(ddlCompany.SelectedValue);
            ddlVendor.DataSource = dataSource;
            ddlVendor.DataBind();
            ddlVendor.Items.Insert(0, new ListItem("--请选择供应商--", "0"));
            ControlHelper.SetSelectedItem(ddlVendor, hfVendor.Value);

            //品牌
            List<CardB> dsCard = WebRepository.QueryCardForCompanyAndVendor(ddlCompany.SelectedValue, vendor);
            ddlCard.DataSource = dsCard;
            ddlCard.DataBind();
            ddlCard.Items.Insert(0, new ListItem("--请选择品牌--", "0"));
            ControlHelper.SetSelectedItem(ddlCard, hfCard.Value);

            this.BindShopRight(company, vendor, card, shopCode);
            //还原gv的选中状态
            CheckBox check = null;
            string shopID = null;
            foreach (GridViewRow gv in gvShop.Rows)
            {
                if (gv.RowType != DataControlRowType.DataRow) continue;
                check = gv.FindControl("chkSelect") as CheckBox;
                shopID = gvShop.DataKeys[gv.RowIndex].Value.ToString();
                if (allowList.Contains(shopID))
                    check.Checked = true;
                else
                    check.Checked = false;
            }
            //还原gvStock的选中状态，提交表单后触发
            try
            {
                string stockID = null;
                foreach (GridViewRow gv in gvStock.Rows)
                {
                    if (gv.RowType != DataControlRowType.DataRow) continue;
                    check = gv.FindControl("chkSelect") as CheckBox;
                    if (check != null)
                    {
                        stockID = gvStock.DataKeys[gv.RowIndex].Value.ToString();
                        if (allowListStock.Contains(stockID))
                            check.Checked = true;
                        else
                            check.Checked = false;
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 新增记录时的页面加载逻辑
        /// </summary>
        private void CreateLoad()
        {
            string message = string.Empty;//提示的消息
            string callback = string.Empty;//错误回调

            this.BindBasCompany(); //绑定公司下拉列表 
            //如果没有公司资料，则提示其使用资料同步
            if (ddlCompany.Items.Count == 0)
            {
                message = "没有公司基础资料，建议您使用【基础资料>>同步资料】功能更新基础资料，然后刷新本页面！";
                this.Alert(message, Core.DialogIcon.Warning);
                this.SetControlState(false); //隐藏保存按钮，禁用下拉列表控件
                return;
            }

            //检查是否有供应商
            //int vendorCount = WebRepository.VendorCount();
            SolutionBll bll = new SolutionBll();
            int vendorCount = bll.GetVendorCount();
            message = string.Format("请您先<a href='{0}'>添加</a>供应商资料！", this.GetRouteUrl("vendor-add", new { }));
            callback = string.Format("location.href = '{0}';", this.GetRouteUrl("vendor-add", new { }));

            if (vendorCount == 0)
            {
                this.Alert(message, Core.DialogIcon.Warning, callback);
                this.SetControlState(false); //隐藏保存按钮，禁用下拉列表控件                       
            }
            else
            {
                //插入提示项
                ddlCompany.Items.Insert(0, new ListItem("--请选择公司--", "0"));
                //公司默认选择第一项
                ControlHelper.SetSelectedItem(ddlCompany, "0");
            }
        }

        /// <summary>
        /// 修改记录时页面加载逻辑
        /// </summary>
        private void UpdateLoad()
        {
            this.SetControlState(false); //如果是修改，不可以更改下拉列表
            string message = string.Empty;//提示的消息
            string callback = string.Empty;//错误回调

            //SolutionB solution = WebRepository.FetchSolution(this.IdentityKey);
            SolutionBll bll = new SolutionBll();
            var solution = bll.GetSolutionById(this.IdentityKey);
            if (solution == null)
            {
                message = "抱歉，您所访问的提报方案不存在！";
                this.Alert(message, Core.DialogIcon.Warning);
                return;
            }


            //开始初始化UI==================================
            this.InitDropDownList(solution); //绑定下拉列表        
            chkEnabled.Checked = solution.Enabled;

            cbBackgroud.Checked = solution.IsBackground.Value;
            if (!solution.IsBackground.Value)
            {
                cbAuto.Checked = solution.IsAuto.Value;
                cbManual.Checked = !solution.IsAuto.Value;
            }
            if (solution.SalesAmountType == 1)
            {
                cbActualAmount.Checked = true;
            }
            btnSave.Visible = true;
            gvShop.Visible = true;
            gvStock.Visible = true;
            btnSearch.Visible = true;
            btnExport.Visible = true;
            hfCompany.Value = solution.CompanyID;
            hfCard.Value = solution.CardID;
            hfVendor.Value = (solution.VendorID ?? 0).ToString();
            txtCron.Text = solution.CronExpression;
            txtDictory.Text = solution.Directory;
            txtRepeatStartTime.Text = (solution.RepeatStartTime ?? DateTime.Parse("1910-00-00")).ToString("yyyy-MM-dd HH:mm:ss");
            this.CompanyID = solution.CompanyID;                    //存储公司、品牌和供应商
            this.VendorID = solution.VendorID ?? 0;
            this.CardID = solution.CardID;

            //绑定供应商店铺、提报店铺
            this.BindVendShop(this.VendorID);
            this.BindShopRight(this.CompanyID, this.VendorID, this.CardID, HttpUtility.UrlDecode(Request["shopcode"]));
            this.BindBasShop(this.CompanyID);
            //绑定不上传货品范围
            if (gvMaterialRange != null)
            {
                this.LoadMaterialRange(solution.Id);
            }
            //绑定货品折扣
            if (gvMaterialDiscount != null)
            {
                this.LoadMaterialDiscount(solution.Id);
            }
            //绑定仓库子款型
            if (gvStockSubModelSet != null)
            {
                this.LoadStockSubModelSet(solution.Id);
            }
            //绑定特殊货品范围
            if (gvSpecialMaterialRange != null)
            {
                this.LoadSpecialMaterialRange(solution.Id);
            }
            //网店归属线下店铺POS设置
            if (gvOnlineShopSet != null)
            {
                this.LoadOnlineShopSet(solution.Id);
            }
            //绑定客户范围
            if (gvVendCust != null)
            {
                this.LoadVendCust(solution.Id);
            }
        }

        #endregion

        protected void HeaderSort(GridViewRowEventArgs e)
        {
            //判断是否是表头
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //判断是否进行排序
                if (ViewState["SortedField"] != null)
                {
                    //恢复排序信息
                    Dictionary<string, string> order = (Dictionary<string, string>)ViewState["SortedField"];
                    //循环表头的单元格，判断是否有需要排序的单元格
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Controls.Count > 0)
                        {
                            //是否是排序字段信息
                            LinkButton lb = e.Row.Cells[i].Controls[0] as LinkButton;
                            if (lb != null)
                            {
                                //反序和降序
                                if (order.ContainsKey(lb.CommandArgument))
                                {
                                    Literal li = new Literal();
                                    if (order[lb.CommandArgument] == "ASC")
                                    {
                                        li.Text = "▲";
                                    }
                                    else
                                    {
                                        li.Text = "▼";
                                    }
                                    //
                                    e.Row.Cells[i].Controls.Add(li);
                                }
                            }
                        }
                    }
                }
            }
        }
        protected void gvStock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HeaderSort(e);
        }
        protected void gvShop_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            #region//加排序的头方法
            HeaderSort(e);
            #endregion 完成排序
        }
        protected void gv_Sorting(GridViewSortEventArgs e)
        {
            //
            Dictionary<string, string> sorted = new Dictionary<string, string>();
            //从ViewState读取上一次的排序信息。
            if (ViewState["SortedField"] == null)
            {
                //如果没有排序信息，则将当前点击的对象（即点击的排序字段）和排序信息添加到字典中
                sorted.Add(e.SortExpression, "ASC");
                //将排序信息放在字典中保存
                ViewState["SortedField"] = sorted;
            }
            else
            {
                //判断是否点击相同的排序，如果是相同的排序，
                //就是用反序
                //如果不是想通的排序，那就新的派讯，是用升序
                sorted = (Dictionary<string, string>)ViewState["SortedField"];
                if (sorted.ContainsKey(e.SortExpression))
                {
                    if (sorted[e.SortExpression] == "ASC")
                    {
                        sorted[e.SortExpression] = "DESC";
                    }
                    else
                    {
                        sorted[e.SortExpression] = "ASC";
                    }
                }
                else
                {
                    sorted.Clear();
                    sorted.Add(e.SortExpression, "ASC");
                    ViewState["SortedField"] = sorted;
                }
            }
        }
        protected void gvStock_Sorting(object sender, GridViewSortEventArgs e)
        {
            gv_Sorting(e);
            this.BindStock();
        }
        protected void gvShop_Sorting(object sender, GridViewSortEventArgs e)
        {
            gv_Sorting(e);
            this.BindShopSort();
        }
        #region


        #endregion

        protected void btnMaterialRangeDel_Click(object sender, EventArgs e)
        {
            var solutionId = this.IdentityKey;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvMaterialRange.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("MaterialRangeCheckBox");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvMaterialRange.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);
            }
            try
            {
                HandleArgs handle = WebRepository.DeleteMaterialRange(solutionId, strId);

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
                this.LoadMaterialRange(solutionId);
            }
        }

        protected void btnSpecialMaterialRangeDel_Click(object sender, EventArgs e)
        {
            var solutionId = this.IdentityKey;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvSpecialMaterialRange.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("SpecialMaterialRangeCheckBox");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvSpecialMaterialRange.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);
            }
            try
            {
                HandleArgs handle = WebRepository.DeleteSpecialMaterialRange(solutionId, strId);

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
                this.LoadSpecialMaterialRange(solutionId);
            }
        }

        private string MaterialRangeImportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "不上传货品范围导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("货品名称");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string SpecialMaterialRangeImportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "特殊货品范围导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("货品名称");
                dtSourceStock.Columns.Add("店铺代码");
                dtSourceStock.Columns.Add("店铺名称");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string MaterialDiscountImportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "货品折扣导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("货号");
                dtSourceStock.Columns.Add("货品名称");
                dtSourceStock.Columns.Add("折扣");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void MaterialRangeImport(string fileName, int solutionId)
        {
            var ai = new AJAXInterop();
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitMaterialRangeImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                //数据检查
                //已经设置了特殊货号的，不能设置到不上传货号里面
                string materialCodes = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    materialCodes += "'" + excelDt.Rows[i]["货号"] + "',";
                }
                if (materialCodes.Length > 3)
                {
                    materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                }
                var checkResult = WebRepository.GetExistsSpecialMaterialRangeMaterialCode(materialCodes, solutionId);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    ai.StatusCode = "2";
                    ai.Html = $"导入失败,特殊货品范围已设置以下货号：[{checkResult}]";
                    this.ResponseToClient(ai, true);
                    return;
                }

                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["Id"] = 0;
                    row["SolutionId"] = solutionId;
                    row["MaterialCode"] = excelDt.Rows[i]["货号"];
                    row["MaterialName"] = excelDt.Rows[i]["货品名称"];
                    row["CreateTime"] = DateTime.Now;
                    row["ModifyTime"] = DateTime.Now;
                    dt.Rows.Add(row);
                }

                WebRepository.DeleteAllMaterialRange(solutionId);
                args = WebRepository.BulkCopyMaterialRange(dt);
                if (args.Status == HandleStatus.Completed)
                {
                    this.LoadMaterialRange(solutionId);
                    string html = this.RenderControl(gvMaterialRange);
                    ai.StatusCode = "1";
                    ai.Html = html;
                    this.ResponseToClient(ai, true);
                }
                else
                {
                    ai.StatusCode = "2";
                    ai.Html = "导入失败" + args.Message;
                    this.ResponseToClient(ai, true);
                }
            }
            catch (Exception ex)
            {
                ai.StatusCode = "2";
                ai.Html = "导入失败:" + ex.Message + datetimeStr;
                this.ResponseToClient(ai, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        private DataTable InitMaterialRangeImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreateTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            return dt;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void SpecialMaterialRangeImport(string fileName, int solutionId)
        {
            var ai = new AJAXInterop();
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitSpecialMaterialRangeImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                //数据检查
                //已经设置了不上传的，不能设置到特殊货号里面

                string materialCodes = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    materialCodes += "'" + excelDt.Rows[i]["货号"] + "',";
                }
                if (materialCodes.Length > 3)
                {
                    materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                }
                var checkResult = WebRepository.GetExistsMaterialRangeMaterialCode(materialCodes, solutionId);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    ai.StatusCode = "2";
                    ai.Html = $"导入失败,不上传货品范围已设置以下货号：[{checkResult}]";
                    this.ResponseToClient(ai, true);
                    return;
                }

                for (int i = 0; i < excelDt.Rows.Count; i++)
                {

                    DataRow row = dt.NewRow();
                    row["Id"] = 0;
                    row["SolutionId"] = solutionId;
                    row["MaterialCode"] = excelDt.Rows[i]["货号"];
                    row["MaterialName"] = excelDt.Rows[i]["货品名称"];
                    row["ShopCode"] = excelDt.Rows[i]["店铺代码"];
                    row["ShopName"] = excelDt.Rows[i]["店铺名称"];
                    row["CreateTime"] = DateTime.Now;
                    row["ModifyTime"] = DateTime.Now;
                    dt.Rows.Add(row);
                }

                WebRepository.DeleteAllSpecialMaterialRange(solutionId);
                args = WebRepository.BulkCopySpecialMaterialRange(dt);
                if (args.Status == HandleStatus.Completed)
                {
                    this.LoadSpecialMaterialRange(solutionId);
                    var html = this.RenderControl(gvSpecialMaterialRange);
                    ai.StatusCode = "1";
                    ai.Html = html;
                    this.ResponseToClient(ai, true);
                }
                else
                {
                    ai.StatusCode = "2";
                    ai.Html = "导入失败" + args.Message;
                    this.ResponseToClient(ai, true);
                }
            }
            catch (Exception ex)
            {
                ai.StatusCode = "2";
                ai.Html = "导入失败:" + ex.Message + datetimeStr;
                this.ResponseToClient(ai, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        private DataTable InitSpecialMaterialRangeImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ShopName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreateTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            return dt;


        }


        /// <summary>
        /// 货品折扣 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void MaterialDiscountImport(string fileName, int solutionId)
        {
            var ai = new AJAXInterop();
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitMaterialDiscountImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                //数据检查
                //已经设置了特殊货号的，不能设置到不上传货号里面
                string materialCodes = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    materialCodes += "'" + excelDt.Rows[i]["货号"] + "',";
                }
                if (materialCodes.Length > 3)
                {
                    materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                }
               //List<GatMaterialDiscountB> data = WebRepository.GetMaterialDiscount(solutionId);
                string MaterialCode = "";
                string MaterialName = "";
                int Count = 0;
                string DeleteMaterialCode = "";
                decimal Discount = 0;
                string Msg = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    MaterialCode = excelDt.Rows[i]["货号"].ToString();
                    Discount = Convert.ToDecimal(excelDt.Rows[i]["折扣"].ToString());
                    if (Discount > 1 || Discount < 0)
                    {
                        Msg = "货号【" + MaterialCode + "】折扣【" + Discount.ToString() + "】异常。折扣取值只能是大于0并且小于1,";
                        break;
                    }

                    //Count = data.Where(s => s.MaterialCode == MaterialCode).Count();
                    //if (Count > 0)
                    //{
                    //    DeleteMaterialCode += "'" + MaterialCode + "',";
                    //}
                    DataRow row = dt.NewRow();
                    row["Id"] = 0;
                    row["SolutionId"] = solutionId;
                    row["MaterialCode"] = MaterialCode;
                    row["MaterialName"] = excelDt.Rows[i]["货品名称"];
                    row["Discount"] = excelDt.Rows[i]["折扣"];
                    row["CreateTime"] = DateTime.Now;
                    row["ModifyTime"] = DateTime.Now;
                    dt.Rows.Add(row);
                }
                if (!string.IsNullOrEmpty(Msg))
                {
                    ai.StatusCode = "2";
                    ai.Html = "导入失败" + Msg;
                    this.ResponseToClient(ai, true);
                }
                else
                {
                    //if (!string.IsNullOrEmpty(DeleteMaterialCode))
                    //{
                    //    DeleteMaterialCode = DeleteMaterialCode.TrimEnd(',');
                    //    WebRepository.DeleteAllMaterialDiscount(solutionId, DeleteMaterialCode);
                    //}
                    WebRepository.DeleteAllMaterialDiscount(solutionId,"");
                    args = WebRepository.BulkCopyMaterialDiscount(dt);
                    if (args.Status == HandleStatus.Completed)
                    {
                        this.LoadMaterialDiscount(solutionId);
                        string html = this.RenderControl(gvMaterialDiscount);
                        ai.StatusCode = "1";
                        ai.Html = html;
                        this.ResponseToClient(ai, true);
                    }
                    else
                    {
                        ai.StatusCode = "2";
                        ai.Html = "导入失败" + args.Message;
                        this.ResponseToClient(ai, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ai.StatusCode = "2";
                ai.Html = "导入失败:" + ex.Message + datetimeStr;
                this.ResponseToClient(ai, true);
            }
            finally
            {
                this.Response.End();
            }
        }

        private DataTable InitMaterialDiscountImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "MaterialName", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Discount", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreateTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            return dt;
        }

        protected void btnMaterialDiscountDel_Click(object sender, EventArgs e)
        {
            var solutionId = this.IdentityKey;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvMaterialDiscount.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("MaterialDiscountCheckBox");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvMaterialDiscount.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);
            }
            try
            {
                HandleArgs handle = WebRepository.DeleteMaterialDiscount(solutionId, strId);

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
                this.LoadMaterialDiscount(solutionId);
            }
        }

        protected void btnMaterialDiscountSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<GatMaterialDiscountB> materialDiscountUpdate = new List<GatMaterialDiscountB>();
                foreach (GridViewRow row in gvMaterialDiscount.Rows)
                {
                    TextBox txtMaterialCode = (TextBox)row.FindControl("txtMaterialCode");
                    TextBox txtMaterialName = (TextBox)row.FindControl("txtMaterialName");
                    TextBox txtMateriaDiscount = (TextBox)row.FindControl("txtMateriaDiscount");
                    var rowId = gvMaterialDiscount.DataKeys[row.RowIndex].Value;
                    materialDiscountUpdate.Add(new GatMaterialDiscountB
                    {
                        Id = (int)rowId,
                        SolutionId = this.IdentityKey,
                        MaterialCode = txtMaterialCode.Text,
                        MaterialName = txtMaterialName.Text,
                        Discount = decimal.Parse(txtMateriaDiscount.Text)

                    });
                }
                var materialCodes = "";
                if (materialDiscountUpdate.Count > 0)
                {
                    var codeList = materialDiscountUpdate.Select(s => s.MaterialCode);
                    foreach (var item in codeList)
                    {
                        materialCodes += "'" + item + "',";
                    }
                    if (materialCodes.Length > 3)
                    {
                        materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                    }
                }
                else
                {
                    return;
                }

                //var checkResult = WebRepository.GetExistsSpecialMaterialRangeMaterialCode(materialCodes, this.IdentityKey);
                //if (!string.IsNullOrEmpty(checkResult))
                //{
                //    this.Alert($"导入失败,特殊货品范围已设置以下货号：[{checkResult}]", Core.DialogIcon.Warning);
                //    return;
                //}
                HandleArgs result = WebRepository.UpdateMaterialDiscount(materialDiscountUpdate);
                if (result.Status == HandleStatus.Fail)
                {
                    this.Alert($"保存失败，信息：{result.Message}", Core.DialogIcon.Warning);
                }
                else
                {
                    this.Alert($"保存成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert($"保存失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadMaterialDiscount(this.IdentityKey);
            }
        }



        protected void btnMaterialRangeSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<GatMaterialRangeB> materialRangeUpdate = new List<GatMaterialRangeB>();
                foreach (GridViewRow row in gvMaterialRange.Rows)
                {
                    TextBox txtMaterialCode = (TextBox)row.FindControl("txtMaterialCode");
                    TextBox txtMaterialName = (TextBox)row.FindControl("txtMaterialName");
                    var rowId = gvMaterialRange.DataKeys[row.RowIndex].Value;
                    materialRangeUpdate.Add(new GatMaterialRangeB
                    {
                        Id = (int)rowId,
                        SolutionId = this.IdentityKey,
                        MaterialCode = txtMaterialCode.Text,
                        MaterialName = txtMaterialName.Text
                    });
                }
                var materialCodes = "";
                if (materialRangeUpdate.Count > 0)
                {
                    var codeList = materialRangeUpdate.Select(s => s.MaterialCode);
                    foreach (var item in codeList)
                    {
                        materialCodes += "'" + item + "',";
                    }
                    if (materialCodes.Length > 3)
                    {
                        materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                    }
                }
                else
                {
                    return;
                }

                var checkResult = WebRepository.GetExistsSpecialMaterialRangeMaterialCode(materialCodes, this.IdentityKey);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    this.Alert($"导入失败,特殊货品范围已设置以下货号：[{checkResult}]", Core.DialogIcon.Warning);
                    return;
                }
                HandleArgs result = WebRepository.UpdateMaterialRange(materialRangeUpdate);
                if (result.Status == HandleStatus.Fail)
                {
                    this.Alert($"保存失败，信息：{result.Message}", Core.DialogIcon.Warning);
                }
                else
                {
                    this.Alert($"保存成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert($"保存失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadMaterialRange(this.IdentityKey);
            }
        }

        protected void btnSpecialMaterialRangeSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<GatSpecialMaterialRangeB> specialMaterialRangeUpdate = new List<GatSpecialMaterialRangeB>();
                foreach (GridViewRow row in gvSpecialMaterialRange.Rows)
                {
                    TextBox txtMaterialCode = (TextBox)row.FindControl("txtMaterialCode");
                    TextBox txtMaterialName = (TextBox)row.FindControl("txtMaterialName");
                    TextBox txtShopCode = (TextBox)row.FindControl("txtShopCode");
                    TextBox txtShopName = (TextBox)row.FindControl("txtShopName");
                    var rowId = gvSpecialMaterialRange.DataKeys[row.RowIndex].Value;
                    specialMaterialRangeUpdate.Add(new GatSpecialMaterialRangeB
                    {
                        Id = (int)rowId,
                        SolutionId = this.IdentityKey,
                        MaterialCode = txtMaterialCode.Text,
                        MaterialName = txtMaterialName.Text,
                        ShopCode = txtShopCode.Text,
                        ShopName = txtShopName.Text
                    });
                }
                string materialCodes = "";
                if (specialMaterialRangeUpdate.Count > 0)
                {
                    var codeList = specialMaterialRangeUpdate.Select(s => s.MaterialCode);
                    foreach (var item in codeList)
                    {
                        materialCodes += "'" + item + "',";
                    }
                    if (materialCodes.Length > 3)
                    {
                        materialCodes = materialCodes.Substring(0, materialCodes.Length - 1);
                    }
                }
                else
                {
                    return;
                }
                var checkResult = WebRepository.GetExistsMaterialRangeMaterialCode(materialCodes, this.IdentityKey);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    this.Alert($"导入失败,不上传货品范围已设置以下货号：[{checkResult}]", Core.DialogIcon.Warning);
                    return;
                }

                HandleArgs result = WebRepository.UpdateSpecialMaterialRange(specialMaterialRangeUpdate);
                if (result.Status == HandleStatus.Fail)
                {
                    this.Alert($"保存失败，信息：{result.Message}", Core.DialogIcon.Warning);
                }
                else
                {
                    this.Alert($"保存成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert($"保存失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadSpecialMaterialRange(this.IdentityKey);
            }
        }

        #region 客户

        /// <summary>
        /// 加载客户列表
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadVendCust(int solutionId)
        {
            VendCustBll bll = new VendCustBll();
            var data = bll.GetVendCust(solutionId);
            if (data == null)
            {
                data = new List<GatVendcustright>();
            }
            gvVendCust.DataSource = data;
            gvVendCust.DataBind();
            gvVendCust.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 同步客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnVendCustAsync_Click(object sender, EventArgs e)
        {
            VendCustBll bll = new VendCustBll();
            try
            {
                bll.AyncErpCust(this.IdentityKey);
            }
            catch (Exception ex)
            {
                this.Alert($"同步失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadVendCust(this.IdentityKey);
            }
        }

        protected void SaveVendCust()
        {
            try
            {
                VendCustBll bll = new VendCustBll();
                List<GatVendcustright> vendcusts = new List<GatVendcustright>();
                foreach (GridViewRow row in gvVendCust.Rows)
                {
                    TextBox txtStockTradeCode = (TextBox)row.FindControl("txtStockTradeCode");
                    TextBox txtStockTradeCode2 = (TextBox)row.FindControl("txtStockTradeCode2");
                    CheckBox chkVendCustCheck = (CheckBox)row.FindControl("chkVendCustCheck");
                    var rowId = gvVendCust.DataKeys[row.RowIndex].Value;
                    vendcusts.Add(new GatVendcustright()
                    {
                        Id = Convert.ToInt32(rowId),
                        TradeCode = txtStockTradeCode.Text,
                        TradeCode2 = txtStockTradeCode2.Text,
                        AllowUsed = chkVendCustCheck.Checked
                    });
                }
                string[] vendcustRightUpdateColumns = { "TradeCode", "TradeCode2", "AllowUsed" };
                bll.Update(vendcusts, vendcustRightUpdateColumns);
            }
            catch (Exception ex)
            {
                throw new Exception($"报存客户异常：{ex.Message},stackTrace:{ex.StackTrace}");
            }
        }
        #endregion


        #region 仓库子款型设置

        protected void btnDoRefreshStockSubModelSetGrid_Click(object sender, EventArgs e)
        {
            this.LoadStockSubModelSet(this.IdentityKey);
        }

        /// <summary>
        /// 加载仓库子款型
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadStockSubModelSet(int solutionId)
        {
            List<GatStockSubModelSetB> data = WebRepository.GetStockSubModelSet(solutionId);
            if (data == null)
            {
                data = new List<GatStockSubModelSetB>();
            }
            gvStockSubModelSet.DataSource = data;
            gvStockSubModelSet.DataBind();
            gvStockSubModelSet.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 导出 仓库子款型格式
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string StockSubModelSetImportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "仓库子款型导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock);
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("ERP仓库编码");
                dtSourceStock.Columns.Add("商品子款型");
                dtSourceStock.Columns.Add("PCode");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        /// <summary>
        /// 仓库子款型 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void StockSubModelSetImport(string fileName, int solutionId)
        {
            var ai = new AJAXInterop();
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitStockSubModelSetImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                //数据检查
                int Count = 0;
                string DeleteMaterialCode = "";
                decimal Discount = 0;
                string Msg = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["Id"] = 0;
                    row["SolutionId"] = solutionId;
                    row["StockCode"] = excelDt.Rows[i]["ERP仓库编码"];
                    row["SubModel"] = excelDt.Rows[i]["商品子款型"];
                    row["PCode"] = excelDt.Rows[i]["PCode"];
                    row["CreateTime"] = DateTime.Now;
                    row["ModifyTime"] = DateTime.Now;
                    dt.Rows.Add(row);
                }
                if (!string.IsNullOrEmpty(Msg))
                {
                    ai.StatusCode = "2";
                    ai.Html = "导入失败" + Msg;
                    this.ResponseToClient(ai, true);
                }
                else
                {
                    //全部删除
                    WebRepository.DeleteAllStockSubModelSet(solutionId, "");
                    //全部导入
                    args = WebRepository.BulkCopyStockSubModelSet(dt);
                    if (args.Status == HandleStatus.Completed)
                    {
                        this.LoadStockSubModelSet(solutionId);
                        string html = this.RenderControl(gvStockSubModelSet);
                        ai.StatusCode = "1";
                        ai.Html = html;
                        this.ResponseToClient(ai, true);
                    }
                    else
                    {
                        ai.StatusCode = "2";
                        ai.Html = "导入失败" + args.Message;
                        this.ResponseToClient(ai, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ai.StatusCode = "2";
                ai.Html = "导入失败:" + ex.Message + datetimeStr;
                this.ResponseToClient(ai, true);
            }
            finally
            {
                this.Response.End();
            }
        }
        /// <summary>
        /// 构建导入数据表
        /// </summary>
        /// <returns></returns>
        private DataTable InitStockSubModelSetImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "StockCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SubModel", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreateTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            return dt;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnStockSubModelSetDel_Click(object sender, EventArgs e)
        {
            var solutionId = this.IdentityKey;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvStockSubModelSet.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("StockSubModelSetCheckBox");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvStockSubModelSet.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号
                strId = strId.Substring(1);
            }
            try
            {
                HandleArgs handle = WebRepository.DeleteStockSubModelSet(solutionId, strId);

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
                this.LoadStockSubModelSet(solutionId);
            }
        }

        protected void btnStockSubModelSetSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<GatStockSubModelSetB> stockSubModelSetUpdate = new List<GatStockSubModelSetB>();
                foreach (GridViewRow row in gvStockSubModelSet.Rows)
                {
                    TextBox txtStockCode = (TextBox)row.FindControl("txtStockCode_SS");
                    TextBox txtSubModel = (TextBox)row.FindControl("txtSubModel_SS");
                    TextBox txtPCode = (TextBox)row.FindControl("txtPCode_SS");
                    var rowId = gvStockSubModelSet.DataKeys[row.RowIndex].Value;
                    stockSubModelSetUpdate.Add(new GatStockSubModelSetB
                    {
                        Id = (int)rowId,
                        SolutionId = this.IdentityKey, 
                        StockCode = txtStockCode.Text,
                        SubModel = txtSubModel.Text,
                        PCode = txtPCode.Text,

                    });
                }
                if (stockSubModelSetUpdate.Count == 0)
                {
                    return;
                }

                //var checkResult = WebRepository.GetExistsSpecialMaterialRangeMaterialCode(materialCodes, this.IdentityKey);
                //if (!string.IsNullOrEmpty(checkResult))
                //{
                //    this.Alert($"导入失败,特殊货品范围已设置以下货号：[{checkResult}]", Core.DialogIcon.Warning);
                //    return;
                //}
                HandleArgs result = WebRepository.UpdateStockSubModelSet(stockSubModelSetUpdate);
                if (result.Status == HandleStatus.Fail)
                {
                    this.Alert($"保存失败，信息：{result.Message}", Core.DialogIcon.Warning);
                }
                else
                {
                    this.Alert($"保存成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert($"保存失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadStockSubModelSet(this.IdentityKey);
            }
        }


        #endregion

        #region 网店归属线下店铺POS设置

        protected void btnDoRefreshOnlineShopSetGrid_Click(object sender, EventArgs e)
        {
            this.LoadOnlineShopSet(this.IdentityKey);
        }
        /// <summary>
        /// 加载网店归属线下店铺POS设置
        /// </summary>
        /// <param name="solutionId"></param>
        private void LoadOnlineShopSet(int solutionId)
        {
            List<GatOnlineShopSetB> data = WebRepository.GatOnlineShopSet(solutionId);
            if (data == null)
            {
                data = new List<GatOnlineShopSetB>(); 
            }
            gvOnlineShopSet.DataSource = data;
            gvOnlineShopSet.DataBind();
            gvOnlineShopSet.CssClass = data.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 删除网店归属线下店铺POS设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOnlineShopSetDel_Click(object sender, EventArgs e)
        {
            var solutionId = this.IdentityKey;
            string strId = string.Empty;
            foreach (GridViewRow gvr in gvOnlineShopSet.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("OnlineShopSetCheckBox");  //CheckBoxId为选择框的id
                if (cb.Checked)
                {
                    strId = strId + "," + gvOnlineShopSet.DataKeys[gvr.RowIndex].Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(strId))
            {
                //去除第一个逗号 
                strId = strId.Substring(1);
            }
            try
            {
                HandleArgs handle = WebRepository.DeleteOnlineShopSet(solutionId, strId);

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
                this.LoadOnlineShopSet(solutionId);
            }
        }

        //保存网店归属线下店铺POS设置
        protected void btnOnlineShopSetSave_Click(object sender, EventArgs e)
        {  
            try
            {
                List<GatOnlineShopSetB> OnlineShopSetUpdate = new List<GatOnlineShopSetB>();
                foreach (GridViewRow row in gvOnlineShopSet.Rows)
                {
                    TextBox txtOnlineShopCode = (TextBox)row.FindControl("txtOnlineShopCode");
                    TextBox txtOfflineShopCode = (TextBox)row.FindControl("txtOfflineShopCode");
                    var rowId = gvOnlineShopSet.DataKeys[row.RowIndex].Value;
                    OnlineShopSetUpdate.Add(new GatOnlineShopSetB
                    {
                        Id = (int)rowId,
                        SolutionId = this.IdentityKey,
                        OnlineShopCode = txtOnlineShopCode.Text,
                        OfflineShopCode = txtOfflineShopCode.Text
                    });
                }
                if (OnlineShopSetUpdate.Count == 0)
                {
                    return;
                }
                HandleArgs result = WebRepository.UpdateOnlineShopSet(OnlineShopSetUpdate);
                if (result.Status == HandleStatus.Fail)
                {
                    this.Alert($"保存失败，信息：{result.Message}", Core.DialogIcon.Warning);
                }
                else
                {
                    this.Alert($"保存成功", Core.DialogIcon.Correct);
                }
            }
            catch (Exception ex)
            {
                this.Alert($"保存失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
            finally
            {
                this.LoadOnlineShopSet(this.IdentityKey);
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOnlineShopSetAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var solutionId = this.IdentityKey;
                List<GatOnlineShopSetB> OnlineShopList = WebRepository.GatOnlineShopSet(solutionId);
                OnlineShopList.Add(new GatOnlineShopSetB
                {
                    Id = 0,  // 或者 -1，用于标识这是新增行
                    SolutionId = solutionId,
                    OnlineShopCode = string.Empty,
                    OfflineShopCode = string.Empty
                });
                gvOnlineShopSet.DataSource = OnlineShopList;
                gvOnlineShopSet.DataBind();
                // 保持当前标签页（如果有 Tab 控件）
            }
            catch (Exception ex)
            {
                this.Alert($"新增失败，信息：{ex.Message}", Core.DialogIcon.Warning);
            }
        }

        // 保持当前标签页的方法
        private void KeepCurrentTab()
        {
            // 如果使用 Bootstrap Tab
            string activeTab = Request.Form["activeTab"] ?? "tab1";
            // 可以通过 HiddenField 或 QueryString 传递
            // 或者在客户端用 JavaScript 记住
        }

        /// <summary>
        /// 导出 仓库子款型格式
        /// </summary>
        /// <param name="fileArry"></param>
        /// <param name="fileNameStock"></param>
        /// <returns></returns>
        private string OnlineShopSetImportFile(ref byte[] fileArry, ref string fileNameStock)
        {
            string path = Server.MapPath("/Files");
            fileNameStock = "网店归属线下店铺导入.xls";
            if (File.Exists(fileNameStock)) File.Delete(fileNameStock); 
            try
            {
                System.Data.DataTable dtSourceStock = new System.Data.DataTable();
                dtSourceStock.Columns.Add("网店代码");
                dtSourceStock.Columns.Add("线下店铺代码");
                ExcelHelper Excel = new ExcelHelper();
                MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileNameStock);
                fileArry = ms.ToArray();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        /// <summary>
        /// 网店归属线下店铺POS设置 导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void OnlineShopSetImport(string fileName, int solutionId)
        {
            var ai = new AJAXInterop();
            string datetimeStr = string.Empty;
            try
            {

                string dir = Page.Server.MapPath("/FileUpload");
                string filePath = dir + "\\" + fileName;

                ExcelHelper Excel = new ExcelHelper();
                DataTable excelDt = Excel.Import(filePath);
                DataTable dt = InitOnlineShopSetImportDataTable();//dt1.Clone();
                HandleArgs args = new HandleArgs();

                //数据检查
                int Count = 0;
                string DeleteMaterialCode = "";
                decimal Discount = 0;
                string Msg = "";
                for (int i = 0; i < excelDt.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["Id"] = 0;
                    row["SolutionId"] = solutionId;
                    row["OnlineShopCode"] = excelDt.Rows[i]["网店代码"].ToString().Trim();
                    row["OfflineShopCode"] = excelDt.Rows[i]["线下店铺代码"].ToString().Trim();
                    row["CreateTime"] = DateTime.Now;
                    row["ModifyTime"] = DateTime.Now;
                    dt.Rows.Add(row);
                }
                if (!string.IsNullOrEmpty(Msg))
                {
                    ai.StatusCode = "2";
                    ai.Html = "导入失败" + Msg;
                    this.ResponseToClient(ai, true);
                }
                else
                {
                    //全部删除
                    //WebRepository.DeleteAllStockSubModelSet(solutionId, "");
                    //全部导入
                    args = WebRepository.OnlineShopSetImport(dt);
                    if (args.Status == HandleStatus.Completed)
                    {
                        this.LoadOnlineShopSet(solutionId);
                        string html = this.RenderControl(gvOnlineShopSet);
                        ai.StatusCode = "1";
                        ai.Html = html;
                        this.ResponseToClient(ai, true); 
                    } 
                    else
                    {
                        ai.StatusCode = "2";
                        ai.Html = "导入失败" + args.Message;
                        this.ResponseToClient(ai, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ai.StatusCode = "2";
                ai.Html = "导入失败:" + ex.Message + datetimeStr;
                this.ResponseToClient(ai, true); 
            }
            finally
            {
                this.Response.End();
            }
        }
        /// <summary>
        /// 构建导入数据表
        /// </summary>
        /// <returns></returns>
        private DataTable InitOnlineShopSetImportDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "SolutionId", DataType = typeof(Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "OnlineShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "OfflineShopCode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreateTime", DataType = typeof(DateTime) });
            dt.Columns.Add(new DataColumn() { ColumnName = "ModifyTime", DataType = typeof(DateTime) });
            return dt;
        }
        #endregion
    }
}
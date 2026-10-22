using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
using Gat.Utils;
using Newtonsoft.Json;
using System.IO;
using Gat.BLL.Modules;

namespace Gat.Web.admin
{
    public partial class gat_shop_map : Core.PageSingleBase
    {
        #region 公开属性

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName
        {
            get { return (ViewState["VendorName"] ?? "").ToString(); }
            set { ViewState["VendorName"] = value; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName
        {
            get { return (ViewState["CompanyName"] ?? "").ToString(); }
            set { ViewState["CompanyName"] = value; }
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyIdentity
        {
            get { return (ViewState["CompanyID"] ?? "").ToString(); }
            set { ViewState["CompanyID"] = value; }
        }


        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string identity = this["vid"];
            var regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (regex.IsMatch(identity ?? "")) this.IdentityKey = Convert.ToInt32(identity);

            #region AJAX 请求

            if (IsAJAXRequest)
            {
                string mode = Request["mode"] ?? "";
                switch (mode)
                {
                    case "map": //条件切换
                        this.LoadCompany(Request["vendor"]);
                        break;
                    case "loadmap": //可关联的店铺
                        this.LoadShopForMap(Request["vendor"], Request["company"], Request["shop"], "");
                        break;
                    case "shopquery": //查询
                        this.LoadShopForMap(Request["vendor"], Request["company"], Request["shop"], Request["shopcode"]);
                        break;
                    case "mapshop": //保存关联
                        this.MapShop(Request["vendor"], Request["shop"], Request["company"], Request["selected"], Request["scope"]);
                        break;
                    case "showmapping": //展示关联
                        this.ShopMapping(Request["shop"]);
                        break;
                }


                return;
            }

            #endregion

            if (!IsPostBack)
            {
                this.TopTitle = "店铺关联";

                if (this.IdentityKey > 0) //从站外传递参数
                {
                    VendorB vendor = WebRepository.FetchVendor(this.IdentityKey);
                    if (vendor == null)
                    {
                        this.Alert("抱歉，您所浏览的资源不存在！", Core.DialogIcon.Warning);
                        return;
                    }
                } //如果默认不传递参数，则算了


                //初始化下拉列表
                if (!this.BindVendor()) return;
                if (!this.BindSolCompany()) return;
                //初始化供应商店铺表格
                if (!this.BindVendorShop()) return;
                //this.BindShopRight(this.IdentityKey);
            }
        }

        #region 辅助方法

        /// <summary>
        /// 绑定供应商
        /// </summary>
        private bool BindVendor()
        {
            var dataSource = this.WebRepository.QueryVendorsByCode(null);
            this.ddlVendor.DataTextField = "VendorName";
            this.ddlVendor.DataValueField = "ID";
            this.ddlVendor.DataSource = dataSource;
            this.ddlVendor.DataBind();

            if (dataSource.Count != 0)
            {
                if (this.IdentityKey == 0) this.IdentityKey = dataSource[0].ID;
                ListItem li = this.ddlVendor.Items.FindByValue(this.IdentityKey + "");
                if (li != null) li.Selected = true;

                //供应商名称
                this.VendorName = ddlVendor.SelectedItem.Text;

                return true;
            }

            this.Alert("请您先添加供应商资料！", Core.DialogIcon.Warning);
            return false;
        }

        /// <summary>
        /// 绑定供应商店铺
        /// </summary>
        /// <returns></returns>
        private bool BindVendorShop()
        {
            var dataSource = this.WebRepository.QueryVendorShops(this.IdentityKey);
            this.gvVendorShop.DataSource = dataSource;
            this.gvVendorShop.DataBind();
            this.gvVendorShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            if (dataSource.Count == 0)
            {
                this.Alert("请您先添加供应商的店铺资料！", Core.DialogIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 提报公司
        /// </summary>
        private bool BindSolCompany()
        {
            ////根据供应商ID取它的提报方案
            //SolutionB solution = WebRepository.FetchSolutionByVendorID(this.IdentityKey);
            //if (solution == null)
            //{
            //    this.RegisterDialog("【" + this.VendorName + "】没有<a href=" + this.GetRouteUrl("solution-add", new { }) + ">提报方案</a>！", Core.DialogIcon.Warning);
            //    return false;
            //}//没有该供应商的提报方案

            ////绑定公司从资料
            //List<SolCompanyB> dataSource = solution.SolCompanies;
            //this.dllSolCompany.DataSource = dataSource;
            //this.dllSolCompany.DataTextField = "CompanyName";
            //this.dllSolCompany.DataValueField = "CompanyID";
            //this.dllSolCompany.DataBind();

            //if (dataSource.Count != 0)
            //{
            //    this.CompanyIdentity = Request["company"];
            //    if (string.IsNullOrEmpty(CompanyIdentity)) CompanyIdentity = dataSource[0].CompanyID; //默认选中第一项
            //    ListItem li = this.dllSolCompany.Items.FindByValue(this.CompanyIdentity + "");
            //    if (li != null) li.Selected = true;
            //    //this.dllSolCompany.Items.FindByValue(this.CompanyIdentity).Selected = true;

            //    //公司名称
            //    this.CompanyName = dllSolCompany.SelectedItem.Text;

            //    return true;
            //}

            //this.RegisterDialog("请您为【" + this.VendorName + "】<a href=" + this.GetRouteUrl("solution-edit", new { id = solution.ID }) + ">指定提报公司！</a>", Core.DialogIcon.Warning);
            return false;
        }

        /// <summary>
        /// 绑定可以采集的店铺
        /// </summary>
        /// <returns></returns>
        private bool BindShopForMap(string vendorID,string companyID,string vendorShopID,string shopCode="")
        {
            var dataSource = this.WebRepository.QueryShopsForMap(Convert.ToInt32(vendorID), companyID, Convert.ToInt32(vendorShopID), shopCode);
            this.gvShopForMap.DataSource = dataSource;
            this.gvShopForMap.DataBind();
            this.gvShopForMap.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            return true;
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

        #endregion

        #region AJAX 请求

        /// <summary>
        /// 根据供应商ID输出提报公司和提报品牌
        /// </summary>
        /// <param name="vendorID"></param>
        protected void LoadCompany(string vendorID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【vendorID】";
                this.ResponseToClient(ai);
            }

            //根据供应商ID取它的提报方案
            SolutionB solution = null; //WebRepository.FetchSolutionByVendorID(Convert.ToInt32(vendorID));
            if (solution == null)
            {
                ai.StatusCode = "2";
                ai.Html = "该供应商没有提报方案！";
                this.ResponseToClient(ai);
            }//没有该供应商的提报方案

            //公司
            List<SolCompanyB> company = solution.SolCompanies;
            this.dllSolCompany.DataSource = company;
            this.dllSolCompany.DataTextField = "CompanyName";
            this.dllSolCompany.DataValueField = "CompanyID";
            this.dllSolCompany.DataBind();

            ai.StatusCode = "1";
            ai.Message = this.RenderControl(dllSolCompany);
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 加载能关联的店铺
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="companyID"></param>
        /// <param name="vendershopID"></param>
        /// <param name="shopCode">查询时店铺代码</param>
        protected void LoadShopForMap(string vendorID,string companyID,string vendershopID,string shopCode)
        {
            //System.Threading.Thread.Sleep(2000);
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID) || string.IsNullOrEmpty(vendershopID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误！";
                this.ResponseToClient(ai);
            }

            

            //公司
            //List<ShopB> dataSource = WebRepository.QueryShopsForMap(Convert.ToInt32(vendorID), companyID, Convert.ToInt32(vendershopID), HttpUtility.UrlDecode(shopCode));
            //this.gvShopForMap.DataSource = dataSource;
            //this.gvShopForMap.DataBind();
            //this.gvShopForMap.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
            this.BindShopForMap(vendorID, companyID, vendershopID, HttpUtility.UrlDecode(shopCode));

            ai.StatusCode = "1";
            ai.Html = this.RenderControl(gvShopForMap);
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 关联店铺
        /// </summary>
        protected void MapShop(string vendorID, string vendorShopID, string companyID, string map, string scope)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【vendorID】";
                this.ResponseToClient(ai);
            }

            List<string> mapList = map.Split(',').ToList();
            List<string> scopeList = scope.Split(',').ToList();

            HandleArgs HANDLE = WebRepository.MapShops(Convert.ToInt32(vendorID),Convert.ToInt32(vendorShopID), companyID, mapList, scopeList);
            ai.StatusCode = "2";
            ai.Html = HANDLE.Message;
            if (HANDLE.Status == HandleStatus.Completed)
            {
                //绑定供应商的店铺
                var dataSource = this.WebRepository.QueryVendorShops(Convert.ToInt32(vendorID));
                this.gvVendorShop.DataSource = dataSource;
                this.gvVendorShop.DataBind();
                this.gvVendorShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

                ai.StatusCode = "1";
                ai.Html = this.RenderControl(gvVendorShop);
            }

            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 显示所关联到的ERP店铺
        /// </summary>
        /// <param name="vendorShopID"></param>
        protected void ShopMapping(string vendorShopID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorShopID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【供应商店铺标识】";
                this.ResponseToClient(ai);
            }

            //绑定供应商的店铺
            var dataSource = this.WebRepository.QueryMapShop(Convert.ToInt32(vendorShopID));
            this.gvBasShop.DataSource = dataSource;
            this.gvBasShop.DataBind();
            this.gvBasShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            ai.StatusCode = "1";
            ai.Html = this.RenderControl(gvBasShop);
            this.ResponseToClient(ai);
        }

        #endregion
    }
}
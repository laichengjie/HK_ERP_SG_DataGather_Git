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


/* ---------------------------------------------
 * 页面接收参数：
 * 1. company = CompanyID，公司ID
 * 2. vendor = VendorID，品牌ID
 * 3. card = CardID
 * 
 ---------------------------------------------*/

namespace Gat.Web.admin
{


    public partial class gat_shop_right : Core.PageSingleBase
    {
        #region 保护属性

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName
        {
            get { return (ViewState["VendorName"] ?? "").ToString(); }
            set { ViewState["VendorName"] = value; }
        }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string CardName
        {
            get { return (ViewState["CardName"] ?? "").ToString(); }
            set { ViewState["CardName"] = value; }
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
        /// 品牌ID
        /// </summary>
        public string CardIdentity
        {
            get { return (ViewState["CardID"] ?? "").ToString(); }
            set { ViewState["CardID"] = value; }
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
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);

            #region AJAX 请求

            if (IsAJAXRequest)
            {
                string mode = Request["mode"] ?? "";
                switch (mode)
                {
                    case "grt":
                        this.LoadCardAndCompany(Request["vendor"]);
                        break;
                }


                return;
            }

            #endregion



            string identity = this["vid"]; //供应商ID + 供应商ID + 品牌ID 
            var regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (regex.IsMatch(identity ?? "")) this.IdentityKey = Convert.ToInt32(identity);

            if (!IsPostBack)
            {
                this.TopTitle = "权限设置";
                this.btnSave.Visible = false;

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
                if (!this.BindVendorCard()) return;
                //初始化店铺列表
                if (!this.BindShops()) return;

                this.btnSave.Visible = true;
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

            this.Alert("请您先<a href='" + this.GetRouteUrl("vendor-add", new { }) + "'>添加</a>供应商资料！", Core.DialogIcon.Warning);
            return false;
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
        /// 供应商品牌
        /// </summary>
        private bool BindVendorCard()
        {
            //绑定店铺从资料
            List<VendorCardB> dataSource = WebRepository.QueryVendorCards(this.IdentityKey);
            this.ddlVendorCard.DataSource = dataSource;
            this.ddlVendorCard.DataTextField = "CardName";
            this.ddlVendorCard.DataValueField = "CardID";
            this.ddlVendorCard.DataBind();

            if (dataSource.Count != 0)
            {
                this.CardIdentity = Request["card"];
                if (string.IsNullOrEmpty(CardIdentity)) this.CardIdentity = dataSource[0].CardID;
                //this.ddlVendorCard.Items.FindByValue(this.CardIdentity).Selected = true;
                ListItem li = this.ddlVendorCard.Items.FindByValue(this.CardIdentity + "");
                if (li != null) li.Selected = true;

                //品牌名称
                this.CardName = ddlVendorCard.SelectedItem.Text;

                return true;
            }

            this.Alert("请您先添加供应商的<a href=" + this.GetRouteUrl("vendor-edit", new { id = this.IdentityKey }) + ">品牌资料</a>！", Core.DialogIcon.Warning);
            return false;
        }

        /// <summary>
        /// 绑定店铺数据源
        /// </summary>
        protected bool BindShops()
        {
            List<ShopRightB> dataSource = WebRepository.QueryShopRights(this.IdentityKey, this.CardIdentity, this.CompanyIdentity);
            gvShop.DataSource = dataSource;
            gvShop.DataBind();
            gvShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            if (dataSource.Count != 0) return true;

            this.Alert(
                string.Format(@"【{0}】没有为【{1}】的【{2}】品牌提供店铺权限设置<br />可能原因：<ul><li>【{0}】没有经营【{2}】品牌</li><li>【{0}】没有店铺资料</li><li>【{0}】的店铺资料没有仓库</li><li>建议您建议您使用【同步资料】功能更新基础资料</li></ul>",
                             this.CompanyName, this.VendorName, this.CardName),
                 Core.DialogIcon.Warning);
            return false;
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
        protected void LoadCardAndCompany(string vendorID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【vendorID】";
                this.ResponseToClient(ai);
            }

            //根据供应商ID取它的提报方案
            SolutionB solution = null;//WebRepository.FetchSolutionByVendorID(Convert.ToInt32(vendorID));
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
            //品牌
            List<VendorCardB> card = WebRepository.QueryVendorCards(Convert.ToInt32(vendorID));
            this.ddlVendorCard.DataSource = card;
            this.ddlVendorCard.DataTextField = "CardName";
            this.ddlVendorCard.DataValueField = "CardID";
            this.ddlVendorCard.DataBind();

            ai.StatusCode = "1";
            ai.Message = this.RenderControl(dllSolCompany);
            ai.Html = this.RenderControl(ddlVendorCard);
            this.ResponseToClient(ai);
        }

        #endregion

        #region 功能按钮

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //List<string> allowList = new List<string>(); //允许采集
            //List<string> scopeList = new List<string>(); //更新的范围

            //CheckBox check = null;
            //string shopID = null;
            //foreach (GridViewRow gvr in gvShop.Rows)
            //{
            //    if (gvr.RowType != DataControlRowType.DataRow) continue;

            //    shopID = gvShop.DataKeys[gvr.RowIndex].Value.ToString();
            //    scopeList.Add(shopID);

            //    check = gvr.FindControl("chkSelect") as CheckBox;
            //    if (check == null || !check.Checked) continue;

            //    allowList.Add(shopID);
            //}

            //HandleArgs handle = WebRepository.SetShosAllowToGat(this.IdentityKey, this.CompanyIdentity, this.CardIdentity, allowList, scopeList);
            //if (handle.Status == HandleStatus.Completed) handle.Message = "设置店铺权限成功！";
            //this.RegisterDialog(handle.Message, handle.Status == HandleStatus.Completed ? Core.DialogIcon.Correct : Core.DialogIcon.Warning);
        }

        //查询
        void btnSearch_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(this.txtShopCode.Text.Trim())) return;
            string shopCode = this.txtShopCode.Text.Trim();

            List<ShopRightB> dataSource =
                string.IsNullOrEmpty(shopCode) ? WebRepository.QueryShopRights(this.IdentityKey, this.CardIdentity, this.CompanyIdentity)
                : WebRepository.QueryShopRights(this.IdentityKey, this.CardIdentity, this.CompanyIdentity,shopCode);
            gvShop.DataSource = dataSource;
            gvShop.DataBind();
            gvShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";

            if (dataSource.Count == 0)
            {

                this.Alert("没有查询到符合条件的店铺权限资料！", Core.DialogIcon.Warning);
            }
        }

        #endregion
    }
}
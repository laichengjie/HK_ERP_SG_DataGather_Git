using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Gat.BLL;
using Gat.Utils;
using Gat.BLL.Modules;
using Gat.Web.Common;
using System.Text;

namespace Gat.Web.report
{
    public partial class Report_View : System.Web.UI.MasterPage
    {
        /// <summary>
        /// 导航
        /// </summary>
        protected string Navigate
        {
            get
            {
                //var context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
                //if (context != null)
                //{
                //    switch (context.VendorReport)
                //    {
                //        case ReportMap.Nike:
                //            return this.GetNikeNavigate();
                //        case ReportMap.Adidas:
                //            return this.GetAdidasNavigate();
                //        case ReportMap.Kappa:
                //            return this.GetKappaNavigate();
                //        case ReportMap.Puma:
                //            return this.GetPumaNavigate();
                //        case ReportMap.Northland:
                //            return this.GetNorthNavigate();
                //        case ReportMap.Amer:
                //            return this.GetAmerNavigate();
                //        case ReportMap.Columbia:
                //            return this.GetColumbiaNavigate();
                //        case ReportMap.Asics:
                //            return this.GetAsicsNavigate();
                //        case ReportMap.Ua:
                //            return this.GetUaNavigate();
                //        case ReportMap.JackWolfSkin:
                //            return this.GetJackWolfSkinNavigate();
                //        case ReportMap.Lining:
                //            return this.GetLiningNavigate();

                //    }
                //}
                //return string.Empty;

                return new CommonConfig(this).GetNavigate();
            }
        }

        protected string SecondaryNavigate
        {
            get
            {
                return new CommonConfig(this).GetSecondaryNavigate();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
                ConnnectType connType = context.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
                using (var gat = new GatRepository(connType))
                {
                    //绑定供应商
                    ddlVendor.DataSource = gat.QueryAllowUsedVendors();//CacheHelper.FetchCache<List<VendorB>>("Vendor");
                    ddlVendor.DataTextField = "VendorName";
                    ddlVendor.DataValueField = "ID";
                    ddlVendor.DataBind();

                    ListItem li = ddlVendor.Items.FindByValue(context.VendorID.ToString());
                    if (li != null) li.Selected = true;


                    //绑定供应商
                    ddlSoule.DataSource = gat.QuerySolutionsForVendor(context.VendorID);
                    ddlSoule.DataTextField = "Title";
                    ddlSoule.DataValueField = "ID";
                    ddlSoule.DataBind();
                    ListItem sli = ddlVendor.Items.FindByValue(context.VendorID.ToString());
                    if (sli != null) sli.Selected = true;

                }
            }
        }

        #region 辅助方法 

        /// <summary>
        /// 耐克的导航
        /// </summary>
        /// <returns></returns>
        private string GetNikeNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页","rpt-nike-mat|产品数据", "rpt-nike-shop|门店仓库", "rpt-nike-pos|销售数据", "rpt-nike-inv|库存数据", "rpt-nike-sum|销售汇总" };
            string[] temp; 
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 阿迪的导航
        /// </summary>
        /// <returns></returns>
        private string GetAdidasNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-ad-pos|销售数据", "rpt-ad-inv|库存数据", "rpt-ad-transfer|销售中转数据", "rpt-ad-stock-transfer|库存中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// Kappa的导航
        /// </summary>
        /// <returns></returns>
        private string GetKappaNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-kappa-pos|销售数据", "rpt-kappa-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// Puma的导航
        /// </summary>
        /// <returns></returns>
        private string GetPumaNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-puma-mat|产品数据", "rpt-puma-shop|门店仓库", "rpt-puma-pos|销售数据", "rpt-puma-inv|库存数据"};
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// North的导航
        /// </summary>
        /// <returns></returns>
        private string GetNorthNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-northland-pos|销售数据", "rpt-northland-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)导航
        /// </summary>
        /// <returns></returns>
        private string GetAmerNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-amer-pos|销售数据", "rpt-amer-goods|进货数据", "rpt-amer-inv|库存数据", "rpt-amer-other|其他出入库数据", "rpt-amer-product|产品数据", "rpt-amer-shop|门店数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 哥伦比亚的导航
        /// </summary>
        /// <returns></returns>
        private string GetColumbiaNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-columbia-pos|销售数据", "rpt-columbia-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        /// <summary>
        /// 亚瑟士 导航
        /// </summary>
        /// <returns></returns>
        private string GetAsicsNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-asics-pos|销售数据", "rpt-asics-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        /// <summary>
         /// UA的导航
         /// </summary>
         /// <returns></returns>
        private string GetUaNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-ua-pos|销售数据", "rpt-ua-inv|库存数据", "rpt-ua-transfer|中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 狼爪导航
        /// </summary>
        /// <returns></returns>
        private string GetJackWolfSkinNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-jackwolfskin-pos|销售数据", "rpt-jackwolfskin-inv|库存数据" };//, "rpt-jackwolfskin-transfer|中转数据"
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// 李宁导航
        /// </summary>
        /// <returns></returns>
        private string GetLiningNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-lining-pos|销售数据", "rpt-lining-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + this.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion
    }
}
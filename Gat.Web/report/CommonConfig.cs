using Gat.Utils;
using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;

namespace Gat.Web.report
{
    public class CommonConfig
    {
        private Control control;
        public CommonConfig(Control control)
        {
            this.control = control;
        }
        public string GetNavigate()
        {
            var context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            if (context != null)
            {
                switch (context.VendorReport)
                {
                    case ReportMap.Nike:
                        return this.GetNikeNavigate();
                    case ReportMap.Adidas:
                        return this.GetAdidasNavigate();
                    case ReportMap.Kappa:
                        return this.GetKappaNavigate();
                    case ReportMap.Puma:
                        return this.GetPumaNavigate();
                    case ReportMap.Northland:
                        return this.GetNorthNavigate();
                    case ReportMap.Amer:
                        return this.GetAmerNavigate();
                    case ReportMap.Columbia:
                        return this.GetColumbiaNavigate();
                    case ReportMap.Asics:
                        return this.GetAsicsNavigate();
                    case ReportMap.Ua:
                        return this.GetUaNavigate();
                    case ReportMap.JackWolfSkin:
                        return this.GetJackWolfSkinNavigate();
                    case ReportMap.Lining:
                        return this.GetLiningNavigate();
                    case ReportMap.Skechers:
                        return this.GatSkechersNavigate();
                    case ReportMap.Crocs:
                        return this.GatCrocsNavigate();
                    case ReportMap.LiningMember:
                        return this.GatLiningMemberNavigate();
                    case ReportMap.LiningFX:
                        return this.GatLiningFXNavigate();
                    case ReportMap.AdidasOmniChannel:
                        return this.GetAdidasOmniChannelNavigate();
                    case ReportMap.AdidasWifSale:
                        return this.GetAdidasWifSaleNavigate();
                    case ReportMap.TuMi:
                        return this.GetTuMiNavigate();
                    case ReportMap.On:
                        return this.GetOnNavigate();
                    case ReportMap.Vans:
                        return this.GetVansNavigate();
                    case ReportMap.ArcTeryx:
                        return this.GetArcTeryxNavigate();
                    case ReportMap.IceBreaker:
                        return this.GetIceBreakerNavigate();
                    case ReportMap.LEE:
                        return this.GetLeeNavigate();
                    case ReportMap.NikeOmniChannel:
                        return this.GetNikeOmniChannelNavigate();
                    case ReportMap.TheNorthFace:
                        return this.GetTheNorthFaceNavigate();
                    case ReportMap.Iker:
                        return this.GetIkerNavigate();
                    case ReportMap.Burgeon:
                        return this.GetBurgeonNavigate();
                    case ReportMap.NikeApi:
                        return this.GetNikeApiNavigate();


                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 耐克的导航
        /// </summary>
        /// <returns></returns>
        private string GetNikeNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-nike-mat|产品数据", "rpt-nike-shop|门店仓库", "rpt-nike-pos|销售数据", "rpt-nike-inv|库存数据", "rpt-nike-sum|销售汇总" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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

            string[] rptRoute = new string[] { "default|平台首页", "rpt-puma-mat|产品数据", "rpt-puma-shop|门店仓库", "rpt-puma-pos|销售数据", "rpt-puma-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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

            string[] rptRoute = new string[] { "default|平台首页", "rpt-columbia-pos|销售数据", "rpt-columbia-inv|库存数据", "rpt-columbia-transfer|销售中转数据", "rpt-columbia-stock-transfer|库存中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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

            string[] rptRoute = new string[] { "default|平台首页", "rpt-jackwolfskin-pos|销售数据", "rpt-jackwolfskin-inv|库存数据", "rpt-jackwolfskin-transfer|销售中转数据", "rpt-jackwolfskin-stock-transfer|库存中转数据" };//, "rpt-jackwolfskin-transfer|中转数据"
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
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
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private string GatSkechersNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-skechers-pos|销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        private string GatCrocsNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-crocs-pos|销售数据", "rpt-crocs-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        private string GatLiningMemberNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-liningmember-pos|销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private string GatLiningFXNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-liningfx-Wholesale|批发销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// 阿迪全渠道的导航
        /// </summary>
        /// <returns></returns>
        private string GetAdidasOmniChannelNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-adoc-pos-day|全渠道销售数据", "rpt-adoc-inv-day|点库存数据", "rpt-adoc-move-day|调拨数据", "rpt-adoc-stock-out-in-data-hour|出入库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        #region 二级导航

        public string GetSecondaryNavigate()
        {
            var context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            if (context != null)
            {
                switch (context.VendorReport)
                {
                    case ReportMap.Nike:
                    case ReportMap.Adidas:
                    case ReportMap.Kappa:
                    case ReportMap.Puma:
                    case ReportMap.Northland:
                    case ReportMap.Amer:
                    case ReportMap.Columbia:
                    case ReportMap.Asics:
                    case ReportMap.Ua:
                    case ReportMap.JackWolfSkin:
                    case ReportMap.Lining:
                    case ReportMap.Skechers:
                    case ReportMap.Crocs:
                    case ReportMap.LiningMember:
                    case ReportMap.LiningFX:
                    case ReportMap.TuMi:
                        return "";
                    case ReportMap.AdidasOmniChannel:
                        return this.GetAdidasOmniChannelSecondaryNavigate();
                    case ReportMap.AdidasWifSale:
                        return this.GetAdidasWifSaleSecondaryNavigate();
                    case ReportMap.NikeOmniChannel:
                        return this.GetNikeOmniChannelSecondaryNavigate();
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// 阿迪全渠道的导航
        /// </summary>
        /// <returns></returns>
        private string GetAdidasOmniChannelSecondaryNavigate()
        {
            // 将 HttpRequest 转换为 HttpContextBase
            HttpContextBase httpContextBase = new HttpContextWrapper(HttpContext.Current);
            // 获取当前路由数据
            RouteData routeData = RouteTable.Routes.GetRouteData(httpContextBase);
            string[] rptRoute = null;
            if (routeData != null)
            {
                string routeUrl = ((Route)routeData.Route).Url;
                switch (routeUrl)
                {
                    case "report/adoc/posday/{page}":
                        rptRoute = new string[] { "rpt-adoc-pos-day|日数据|1", "rpt-adoc-pos-transfer|日中转数据|0", "rpt-adoc-pos-hour|三小时数据|0" };
                        break;
                    case "report/adoc/postransfer":
                        rptRoute = new string[] { "rpt-adoc-pos-day|日数据|0", "rpt-adoc-pos-transfer|日中转数据|1", "rpt-adoc-pos-hour|三小时数据|0" };
                        break;
                    case "report/adoc/poshour/{page}":
                        rptRoute = new string[] { "rpt-adoc-pos-day|日数据|0", "rpt-adoc-pos-transfer|日中转数据|0", "rpt-adoc-pos-hour|三小时数据|1" };
                        break;
                    case "report/adoc/invday/{page}":
                        rptRoute = new string[] { "rpt-adoc-inv-day|日数据|1", "rpt-adoc-inv-transfer|日中转数据|0", "rpt-adoc-inv-hour|三小时数据|0" };
                        break;
                    case "report/adoc/invtransfer/{page}":
                        rptRoute = new string[] { "rpt-adoc-inv-day|日数据|0", "rpt-adoc-inv-transfer|日中转数据|1", "rpt-adoc-inv-hour|三小时数据|0" };
                        break;
                    case "report/adoc/invhour/{page}":
                        rptRoute = new string[] { "rpt-adoc-inv-day|日数据|0", "rpt-adoc-inv-transfer|日中转数据|0", "rpt-adoc-inv-hour|三小时数据|1" };
                        break;
                    case "report/adoc/moveday/{page}":
                        rptRoute = new string[] { "rpt-adoc-move-day|日数据|1", "rpt-adoc-move-transfer|日中转数据|0", "rpt-adoc-move-hour|三小时数据|0" };
                        break;
                    case "report/adoc/movetransfer":
                        rptRoute = new string[] { "rpt-adoc-move-day|日数据|0", "rpt-adoc-move-transfer|日中转数据|1", "rpt-adoc-move-hour|三小时数据|0" };
                        break;
                    case "report/adoc/movehour/{page}":
                        rptRoute = new string[] { "rpt-adoc-move-day|日数据|0", "rpt-adoc-move-transfer|日中转数据|0", "rpt-adoc-move-hour|三小时数据|1" };
                        break;
                    case "report/adoc/stock_out_in_data/{page}":
                        rptRoute = new string[] { "rpt-adoc-stock-out-in-data-hour|三小时数据|1" };
                        break;
                }
            }
            var sb = new StringBuilder("<ul>");
            string[] temp;
            if (rptRoute != null)
            {
                foreach (string route in rptRoute)
                {
                    temp = route.Split('|');
                    string cssclass = temp[2] == "1" ? "class=\"isselect select\"" : "";
                    sb.Append("<li " + cssclass + " ><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 阿迪智慧零售的导航
        /// </summary>
        /// <returns></returns>
        private string GetAdidasWifSaleNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-adoc-wifpos-day|智慧零售销售数据", "rpt-adoc-wifinv-day|智慧零售库存数据", "rpt-adoc-wifbasicdata-personnel|智慧零售职员资料" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        #region 二级导航


        /// <summary>
        /// 阿迪智慧零售的导航
        /// </summary>
        /// <returns></returns>
        private string GetAdidasWifSaleSecondaryNavigate()
        {
            // 将 HttpRequest 转换为 HttpContextBase
            HttpContextBase httpContextBase = new HttpContextWrapper(HttpContext.Current);
            // 获取当前路由数据
            RouteData routeData = RouteTable.Routes.GetRouteData(httpContextBase);
            string[] rptRoute = null;
            if (routeData != null)
            {
                string routeUrl = ((Route)routeData.Route).Url;
                switch (routeUrl)
                {
                    case "report/adoc/wifposday/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifpos-day|日数据|1", "rpt-adoc-wifpos-transfer|日中转数据|0", "rpt-adoc-wifpos-hour|三小时数据|0" };
                        break;
                    case "report/adoc/wifpostransfer":
                        rptRoute = new string[] { "rpt-adoc-wifpos-day|日数据|0", "rpt-adoc-wifpos-transfer|日中转数据|1", "rpt-adoc-wifpos-hour|三小时数据|0" };
                        break;
                    case "report/adoc/wifposhour/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifpos-day|日数据|0", "rpt-adoc-wifpos-transfer|日中转数据|0", "rpt-adoc-wifpos-hour|三小时数据|1" };
                        break;

                    case "report/adoc/wifinvday/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifinv-day|日数据|1", "rpt-adoc-wifinv-hour|三小时数据|0" };
                        break;
                    case "report/adoc/wifinvtransfer/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifinv-day|日数据|0", "rpt-adoc-wifinv-hour|三小时数据|0" };
                        break;
                    case "report/adoc/wifinvhour/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifinv-day|日数据|0", "rpt-adoc-wifinv-hour|三小时数据|1" };
                        break;
                    case "report/adoc/wifbasicdata_personnel/{page}":
                        rptRoute = new string[] { "rpt-adoc-wifbasicdata-personnel|职员资料|1" };
                        break;
                }
            }
            var sb = new StringBuilder("<ul>");
            string[] temp;
            if (rptRoute != null)
            {
                foreach (string route in rptRoute)
                {
                    temp = route.Split('|');
                    string cssclass = temp[2] == "1" ? "class=\"isselect select\"" : "";
                    sb.Append("<li " + cssclass + " ><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 途明 导航
        /// </summary>
        /// <returns></returns>
        private string GetTuMiNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-tumi-pos|销售数据", "rpt-tumi-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// On昂跑
        /// </summary>
        /// <returns></returns>
        private string GetOnNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-on-pos|销售数据", "rpt-on-inv|库存数据", "rpt-on-invdetail|库存明细数据", "rpt-on-transfer|销售中转数据", "rpt-on-stock-transfer|库存中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// VANS
        /// </summary>
        /// <returns></returns>
        private string GetVansNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-vans-pos|销售数据", "rpt-vans-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// ArcTeryx
        /// </summary>
        /// <returns></returns>
        private string GetArcTeryxNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-arcteryx-pos|销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// 拓冰者的导航
        /// </summary>
        /// <returns></returns>
        private string GetIceBreakerNavigate()
        {
            var sb = new StringBuilder("<ul>");

            //string[] rptRoute = new string[] { "default|平台首页", "rpt-icebreaker-pos|销售数据", "rpt-icebreaker-inv|库存数据", "rpt-icebreaker-transfer|销售中转数据", "rpt-icebreaker-stock-transfer|库存中转数据" };
            string[] rptRoute = new string[] { "default|平台首页", "rpt-icebreaker-pos|销售数据", "rpt-icebreaker-inv|库存数据", "rpt-icebreaker-transfer|销售中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// LEE的导航 
        /// </summary> 
        /// <returns></returns>
        private string GetLeeNavigate()
        {
            var sb = new StringBuilder("<ul>");

            //string[] rptRoute = new string[] { "default|平台首页", "rpt-icebreaker-pos|销售数据", "rpt-icebreaker-inv|库存数据", "rpt-icebreaker-transfer|销售中转数据", "rpt-icebreaker-stock-transfer|库存中转数据" };
            string[] rptRoute = new string[] { "default|平台首页", "rpt-lee-pos|销售数据", "rpt-lee-inv|库存数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        #region Nike全渠道的导航 

        /// <summary>
        /// Nike全渠道的导航 
        /// <returns></returns>
        private string GetNikeOmniChannelNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-nike-pos-day|全渠道销售数据", "rpt-nike-inv-day|库存数据", "rpt-nike-move-day|调拨数据", "rpt-nike-outin-day|出入库数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// Nike全渠道的导航
        /// </summary>
        /// <returns></returns>
        private string GetNikeOmniChannelSecondaryNavigate()
        {
            // 将 HttpRequest 转换为 HttpContextBase
            HttpContextBase httpContextBase = new HttpContextWrapper(HttpContext.Current);
            // 获取当前路由数据
            RouteData routeData = RouteTable.Routes.GetRouteData(httpContextBase);
            string[] rptRoute = null;
            if (routeData != null)
            {
                string routeUrl = ((Route)routeData.Route).Url;
                switch (routeUrl)
                {
                    case "report/nike/posday/{page}":
                        rptRoute = new string[] { "rpt-nike-pos-day|日数据|1", "rpt-nike-pos-transfer|日中转数据|0"};
                        break;
                    case "report/nike/postransfer":
                        rptRoute = new string[] { "rpt-nike-pos-day|日数据|0", "rpt-nike-pos-transfer|日中转数据|1" };
                        break;
                    case "report/nike/invday/{page}":
                        rptRoute = new string[] { "rpt-nike-inv-day|日数据|1", "rpt-nike-inv-transfer|日中转数据|0" };
                        break;
                    case "report/nike/invtransfer/{page}":
                        rptRoute = new string[] { "rpt-nike-inv-day|日数据|0", "rpt-nike-inv-transfer|日中转数据|1" };
                        break;
                    case "report/nike/moveday/{page}":
                        rptRoute = new string[] { "rpt-nike-move-day|日数据|1", "rpt-nike-move-transfer|日中转数据|0"};
                        break;
                    case "report/nike/movetransfer":
                        rptRoute = new string[] { "rpt-nike-move-day|日数据|0", "rpt-nike-move-transfer|日中转数据|1" };
                        break;
                    case "report/nike/outinday/{page}":
                        rptRoute = new string[] { "rpt-nike-outin-day|日数据|1" };
                        break;
                }
            }
            var sb = new StringBuilder("<ul>");
            string[] temp;
            if (rptRoute != null)
            {
                foreach (string route in rptRoute)
                {
                    temp = route.Split('|');
                    string cssclass = temp[2] == "1" ? "class=\"isselect select\"" : "";
                    sb.Append("<li " + cssclass + " ><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 北面导航 
        /// </summary>
        /// <returns></returns>
        private string GetTheNorthFaceNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-thenorthface-pos|销售数据", "rpt-thenorthface-inv|库存数据", "rpt-thenorthface-transfer|销售中转数据", "rpt-thenorthface-stock-transfer|库存中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// Iker导航 
        /// </summary>
        /// <returns></returns> 
        private string GetIkerNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-iker-pos|销售数据", "rpt-iker-inv|库存数据", "rpt-iker-transfer|销售中转数据", "rpt-iker-stock-transfer|库存中转数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 伯俊
        /// </summary>
        /// <returns></returns>
        private string GetBurgeonNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-burgeon-poslog|接口日志", "rpt-burgeon-pos|销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }


        /// <summary>
        /// Nike API 对接
        /// </summary>
        /// <returns></returns>
        private string GetNikeApiNavigate()
        {
            var sb = new StringBuilder("<ul>");

            string[] rptRoute = new string[] { "default|平台首页", "rpt-nikeapi-log|接口日志", "rpt-nikeapi-pos|销售数据" };
            string[] temp;
            foreach (string route in rptRoute)
            {
                temp = route.Split('|');
                sb.Append("<li><a href='" + control.GetRouteUrl(temp[0], new { page = 1 }) + "'>" + temp[1] + "</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}
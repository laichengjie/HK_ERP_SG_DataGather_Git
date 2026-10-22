
/* -----------------------------------------------
 * 说明：
 * 1. 从管理页进入报表时首先进入该页面，该页面负责将供应商
 *    信息写入到COOKIE，供以后的报表页面使用
 * 2. 
 * ----------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.Web.Core;
using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using Gat.Web.Common;

namespace Gat.Web.report
{
    public partial class _default : Core.PageSingleBase //接收供应商ID为参数
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string identity = this["uid"];
            var regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (regex.IsMatch(identity ?? "")) this.IdentityKey = Convert.ToInt32(identity);

            if (!IsPostBack)
            {
                VendorB vendor = null;
                //取供应商的ID，如果页面没有传递值，则默认使用缓存的供应商集合中的第一个
                //缓存老是不见，MB什么原因
                if (this.IdentityKey == 0)
                {
                    List<VendorB> cacheVendor = WebRepository.QueryAllowUsedVendors(); //CacheHelper.FetchCache<List<VendorB>>("Vendor");
                    if (cacheVendor != null && cacheVendor.Count > 0) vendor = cacheVendor[0];
                }
                else
                {
                    vendor = WebRepository.FetchVendor(this.IdentityKey);
                }

                if (vendor == null) //没有供应商，提示用户添加
                {
                    if (IdentityKey == 0)
                    {
                        this.Alert(
                            "请您先<a href='" + this.GetRouteUrl("vendor-add", new { }) + "'>添加</a>供应商资料！",
                            Core.DialogIcon.Warning,
                            "location.href = '" + this.GetRouteUrl("vendor-add", new { }) + "'");
                    }
                    else
                    {
                        this.Alert(
                            "您所访问的供应商不存在！",
                                Core.DialogIcon.Warning,
                            "location.href = '" + this.GetRouteUrl("default", new { }) + "'");
                    }
                    return;
                }

                //将供应商资料写入COOKIE，然后重定向到指定的页面 
                DGContext.VendorID = vendor.ID;
                DGContext.VendorCode = vendor.VendorCode;
                DGContext.VendorName = vendor.VendorName;
                DGContext.VendorReport = vendor.ReportPath;
                CookieHelper.SetCookie<DGContext>(CookieHelper.ContextCookieName, DGContext, 24 * 60);

                //重定向到到报表页面 
                switch (vendor.ReportPath)
                {
                    case ReportMap.Nike:
                        Response.Redirect(this.GetRouteUrl("rpt-nike-mat", new { }));
                        break;
                    case ReportMap.Adidas:
                        Response.Redirect(this.GetRouteUrl("rpt-ad-pos", new { }));
                        break;
                    case ReportMap.Kappa:
                        Response.Redirect(this.GetRouteUrl("rpt-kappa-pos", new { }));
                        break;
                    case ReportMap.Puma:
                        Response.Redirect(this.GetRouteUrl("rpt-puma-pos", new { }));
                        break;
                    case ReportMap.Northland:
                        Response.Redirect(this.GetRouteUrl("rpt-northland-pos", new { }));
                        break;
                    case ReportMap.Amer:
                        Response.Redirect(this.GetRouteUrl("rpt-amer-pos", new { }));
                        break;
                    case ReportMap.Columbia:
                        Response.Redirect(this.GetRouteUrl("rpt-columbia-pos", new { }));
                        break;
                    case ReportMap.Asics:
                        Response.Redirect(this.GetRouteUrl("rpt-asics-pos", new { }));
                        break;
                    case ReportMap.Ua:
                        Response.Redirect(this.GetRouteUrl("rpt-ua-pos", new { }));
                        break;
                    case ReportMap.JackWolfSkin:
                        Response.Redirect(this.GetRouteUrl("rpt-jackwolfskin-pos", new { }));
                        break;
                    case ReportMap.Lining:
                        Response.Redirect(this.GetRouteUrl("rpt-lining-pos", new { }));
                        break;
                    case ReportMap.Skechers:
                        Response.Redirect(this.GetRouteUrl("rpt-skechers-pos", new { }));
                        break;
                    case ReportMap.Crocs:
                        Response.Redirect(this.GetRouteUrl("rpt-crocs-pos", new { }));
                        break;
                    case ReportMap.LiningMember:
                        Response.Redirect(this.GetRouteUrl("rpt-liningmember-pos", new { }));
                        break;
                    case ReportMap.LiningFX:
                        Response.Redirect(this.GetRouteUrl("rpt-liningfx-Wholesale", new { }));
                        break;
                    case ReportMap.AdidasOmniChannel:
                        Response.Redirect(this.GetRouteUrl("rpt-adoc-pos-day", new { }));
                        break;
                    case ReportMap.AdidasWifSale:
                        Response.Redirect(this.GetRouteUrl("rpt-adoc-wifpos-day", new { }));
                        break;
                    case ReportMap.TuMi:
                        Response.Redirect(this.GetRouteUrl("rpt-tumi-pos", new { }));
                        break;
                    case ReportMap.On:
                        Response.Redirect(this.GetRouteUrl("rpt-on-pos", new { }));
                        break;
                    case ReportMap.Vans:
                        Response.Redirect(this.GetRouteUrl("rpt-vans-pos", new { }));
                        break;
                    case ReportMap.ArcTeryx:
                        Response.Redirect(this.GetRouteUrl("rpt-arcteryx-pos", new { }));
                        break;
                    case ReportMap.IceBreaker:
                        Response.Redirect(this.GetRouteUrl("rpt-icebreaker-pos", new { }));
                        break;
                    case ReportMap.LEE:
                        Response.Redirect(this.GetRouteUrl("rpt-lee-pos", new { }));
                        break;
                    case ReportMap.NikeOmniChannel:
                        Response.Redirect(this.GetRouteUrl("rpt-nike-pos-day", new { }));
                        break;
                    case ReportMap.TheNorthFace:
                        Response.Redirect(this.GetRouteUrl("rpt-thenorthface-pos", new { }));
                        break;
                    case ReportMap.Iker:
                        Response.Redirect(this.GetRouteUrl("rpt-iker-pos", new { }));
                        break;
                    case ReportMap.Burgeon:
                        Response.Redirect(this.GetRouteUrl("rpt-burgeon-poslog", new { }));
                        break;
                    case ReportMap.NikeApi:
                        Response.Redirect(this.GetRouteUrl("rpt-nikeapi-pos", new { }));
                        break;
                }
            }
        }
    }
}
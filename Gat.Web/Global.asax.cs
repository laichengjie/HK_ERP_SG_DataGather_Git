using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using System.IO;
using System.Diagnostics;
using Gat.BLL;
using System.Web.Routing;
using Gat.Web.App_Start;
using System.Web.Http;
using NLog;

namespace Gat.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            this.RegisterRouters(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ////缓存每一个品牌供应商的状态
            //using (var gat = new GatRepository())
            //{
            //    Application["Vendors"] = gat.QueryVendors();
            //}
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //' 从随请求一起传输的 cookie 集合中获取窗体身份验证 cookie：
            //' FormsAuthentication.FormsCookieName获取的Cookie的名字跟login.aspx加入的cookie名称是一致的
            //' 它也默认是根目录web.config的authentication下的name
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            //不知道为什么以FormsAuthentication.FormsCookieName名字存取的cookie与原来加入的不一致
            //UKAuthenCookie的cookie命名来自于login.aspx.vb中登录认证后创建的
            //Dim authCookie As HttpCookie = Context.Request.Cookies(".UKAuthenCookie")

            if (authCookie == null)
            {
                //没有身份验证(cookie)
                return;
            }

            //'从窗体身份验证 cookie 中提取身份验证票证并进行解密：
            FormsAuthenticationTicket authTicket = null;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                //记录异常情况详细信息(为简便起见, 已省略)
                return;
            }
            //无法解密 Cookie。
            if (authTicket == null) return;

            //'使用从票证名中获取的用户名创建一个 FormsIdentity 对象，
            //'并创建一个包含此标识以及用户角色列表的 CustomPrincipal 对象
            // 创建一个标识对象
            FormsIdentity fid = new FormsIdentity(authTicket);
            // 该主体将通过整个请求。

            //CustomPrincipal是UK.User一个用户重新自定义的Principal
            Gat.BLL.GatPrincipal principal = new Gat.BLL.GatPrincipal(fid, authTicket.UserData);

            // 将新的主体对象附加到当前的 HttpContext 对象
            Context.User = principal;

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 添加路由规则
        /// </summary>
        /// <param name="routes"></param>
        void RegisterRouters(RouteCollection routes)
        {
            ////参数含义:
            ////第一个参数：路由名称--随便自己起
            ////第二个参数：路由规则
            ////第三个参数：该路由规则交给哪一个页面来处理
            //routes.MapPageRoute("gatrout", "default/{id}", "~/default.aspx");
            ////...当然，您还可以添加更多路由规则
            ///
            //首页
            routes.MapPageRoute("default", "default", "~/admin/default.aspx");
            //错误页
            routes.MapPageRoute("error", "error/{error}", "~/admin/gat_error.aspx", true, new RouteValueDictionary { { "error", null } });
            //登录
            routes.MapPageRoute("login", "login/{p}", "~/userlogin.aspx", false, new RouteValueDictionary { { "p", null } });
            //方案|浏览
            routes.MapPageRoute("solution", "solution/browse", "~/admin/gat_solution_browse.aspx", true);
            //方案|添加
            routes.MapPageRoute("solution-add", "solution/add", "~/admin/gat_solution.aspx", true);
            //方案|修改
            routes.MapPageRoute("solution-edit", "solution/edit/{id}", "~/admin/gat_solution.aspx", true, new RouteValueDictionary { { "id", null } });
            //方案|供应商－品牌
            //routes.MapPageRoute("vendor-card", "vendor/card", "~/admin/gat_vendor_card.aspx", true);
            //店铺\授权，参数：供应商ID
            routes.MapPageRoute("shop", "shop/grt", "~/admin/gat_shop_right.aspx", true);
            //店铺\授权（现当做解决方案），参数：公司
            //routes.MapPageRoute("shop", "shop/grt/{cid}", "~/admin/gat_shop_right.aspx", true, new RouteValueDictionary { { "cid", null } });
            //routes.MapPageRoute("shop", "shop/{id}", "~/admin/gat_shop_right.aspx", true);
            //方案|修改
            routes.MapPageRoute("log", "log/{page}", "~/admin/gat_syslog.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("syscontrol", "syscontrol/{page}", "~/admin/gat_syscontrol.aspx", true, new RouteValueDictionary { { "page", null } });

            //供应商｜浏览
            routes.MapPageRoute("vendor", "vendor/browse", "~/admin/gat_vendor_bowse.aspx", true);
            //供应商｜添加
            routes.MapPageRoute("vendor-add", "vendor/add/{code}", "~/admin/gat_vendor.aspx", true, new RouteValueDictionary { { "code", null } });
            //供应商｜修改
            routes.MapPageRoute("vendor-edit", "vendor/edit/{id}/{code}", "~/admin/gat_vendor.aspx", true, new RouteValueDictionary { { "id", null }, { "code", null } });
            //供应商－品牌
            routes.MapPageRoute("vendor-card", "vendor/card/{id}", "~/admin/gat_vendor_card.aspx", true, new RouteValueDictionary { { "id", null } });
            //供应商－店铺
            routes.MapPageRoute("vendor-shop", "vendor/shop/{id}", "~/admin/gat_vendor_shop.aspx", true, new RouteValueDictionary { { "id", null } });

            routes.MapPageRoute("Material-price", "Material/price", "~/admin/gat_price.aspx", true);
            //object o = { };
            //Gat.BLL.GatRepository g = new GatRepository { { "k", "v" } };

            //店铺\关联，参数：供应商ID
            routes.MapPageRoute("shop-map", "shop/map/{vid}", "~/admin/gat_shop_map.aspx", true, new RouteValueDictionary { { "vid", null } });


            //报表：===================================

            //报表默认页面，参数：供应商ID
            routes.MapPageRoute("rpt-default", "report/default/{uid}", "~/report/default.aspx", true, new RouteValueDictionary { { "uid", null } });

            //Nike ===================================
            //货号
            routes.MapPageRoute("rpt-nike-mat", "report/nike/mat/{page}", "~/report/nike/rpt_material.aspx", true, new RouteValueDictionary { { "page", null } });
            //门店
            routes.MapPageRoute("rpt-nike-shop", "report/nike/shop/{page}", "~/report/nike/rpt_shop.aspx", true, new RouteValueDictionary { { "page", null } });
            //POS
            routes.MapPageRoute("rpt-nike-pos", "report/nike/pos/{page}", "~/report/nike/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-nike-inv", "report/nike/inv/{page}", "~/report/nike/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //汇总
            routes.MapPageRoute("rpt-nike-sum", "report/nike/sum/{page}", "~/report/nike/rpt_summary.aspx", true, new RouteValueDictionary { { "page", null } });

            //Adidas
            //POS
            routes.MapPageRoute("rpt-ad-pos", "report/ad/pos/{page}", "~/report/adidas/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-ad-inv", "report/ad/inv/{page}", "~/report/adidas/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //销售中转
            routes.MapPageRoute("rpt-ad-transfer", "report/adidas/transfer", "~/report/adidas/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-ad-stock-transfer", "report/ad/transfer/stock/{page}", "~/report/adidas/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });

            //Kappa
            //POS
            routes.MapPageRoute("rpt-kappa-pos", "report/kappa/pos/{page}", "~/report/kappa/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-kappa-inv", "report/kappa/inv/{page}", "~/report/kappa/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });

            //Puma ===================================
            //货号
            routes.MapPageRoute("rpt-puma-mat", "report/puma/mat/{page}", "~/report/puma/rpt_material.aspx", true, new RouteValueDictionary { { "page", null } });
            //门店
            routes.MapPageRoute("rpt-puma-shop", "report/puma/shop/{page}", "~/report/puma/rpt_shop.aspx", true, new RouteValueDictionary { { "page", null } });
            //POS
            routes.MapPageRoute("rpt-puma-pos", "report/puma/pos/{page}", "~/report/puma/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-puma-inv", "report/puma/inv/{page}", "~/report/puma/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });

            //Northland ===================================
            //POS
            routes.MapPageRoute("rpt-northland-pos", "report/northland/pos/{page}", "~/report/northland/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-northland-inv", "report/northland/inv/{page}", "~/report/northland/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });

            //Amer ===================================
            //POS
            routes.MapPageRoute("rpt-amer-pos", "report/amer/pos/{page}", "~/report/amer/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //进货
            routes.MapPageRoute("rpt-amer-goods", "report/amer/goods/{page}", "~/report/amer/rpt_reStock.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-amer-inv", "report/amer/inv/{page}", "~/report/amer/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //其他出入库
            routes.MapPageRoute("rpt-amer-other", "report/amer/others/{page}", "~/report/amer/rpt_other.aspx", true, new RouteValueDictionary { { "page", null } });
            //产品
            routes.MapPageRoute("rpt-amer-product", "report/amer/products/{page}", "~/report/amer/rpt_material.aspx", true, new RouteValueDictionary { { "page", null } });
            //门店
            routes.MapPageRoute("rpt-amer-shop", "report/amer/shop/{page}", "~/report/amer/rpt_shop.aspx", true, new RouteValueDictionary { { "page", null } });

            //Columbia ===================================
            //POS
            routes.MapPageRoute("rpt-columbia-pos", "report/columbia/pos/{page}", "~/report/columbia/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-columbia-inv", "report/columbia/inv/{page}", "~/report/columbia/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //中转
            routes.MapPageRoute("rpt-columbia-transfer", "report/columbia/transfer", "~/report/columbia/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-columbia-stock-transfer", "report/columbia/transfer/stock/{page}", "~/report/columbia/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });


            //Asics ===================================
            //POS
            routes.MapPageRoute("rpt-asics-pos", "report/asics/pos/{page}", "~/report/asics/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-asics-inv", "report/asics/inv/{page}", "~/report/asics/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });

            //ua
            //POS
            routes.MapPageRoute("rpt-ua-pos", "report/ua/pos/{page}", "~/report/ua/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-ua-inv", "report/ua/inv/{page}", "~/report/ua/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-ua-transfer", "report/ua/transfer", "~/report/ua/rpt_transfer.aspx");

            //狼爪
            routes.MapPageRoute("rpt-jackwolfskin-pos", "report/jackwolfskin/pos/{page}", "~/report/jackwolfskin/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-jackwolfskin-inv", "report/jackwolfskin/inv/{page}", "~/report/jackwolfskin/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //销售中转
            //routes.MapPageRoute("rpt-jackwolfskin-transfer", "report/jackwolfskin/transfer/{page}", "~/report/jackwolfskin/rpt_transfer.aspx" ,true, new RouteValueDictionary { { "page", null } });

            routes.MapPageRoute("rpt-jackwolfskin-transfer", "report/jackwolfskin/transfer", "~/report/jackwolfskin/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-jackwolfskin-stock-transfer", "report/jackwolfskin/transfer/stock/{page}", "~/report/jackwolfskin/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });


            //李宁
            routes.MapPageRoute("rpt-lining-pos", "report/lining/pos/{page}", "~/report/lining/rpt_pos_new.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-lining-inv", "report/lining/inv/{page}", "~/report/lining/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //李宁会员
            routes.MapPageRoute("rpt-liningmember-pos", "report/liningmember/pos/{page}", "~/report/liningmember/PosSale.aspx", true, new RouteValueDictionary { { "page", null } });
            //李宁分销
            routes.MapPageRoute("rpt-liningfx-Wholesale", "report/liningfx/Wholesale/{page}", "~/report/liningfx/Wholesale.aspx", true, new RouteValueDictionary { { "page", null } });



            //斯凯奇
            routes.MapPageRoute("rpt-skechers-pos", "report/skechers/pos/{page}", "~/report/skechers/PosSale.aspx", true, new RouteValueDictionary { { "page", null } });

            //Crocs
            routes.MapPageRoute("rpt-crocs-pos", "report/crocs/pos/{page}", "~/report/crocs/PosSale.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-crocs-inv", "report/crocs/inv/{page}", "~/report/crocs/Inventory.aspx", true, new RouteValueDictionary { { "page", null } });



            //AdidasOmniChannel
            //全渠道销售数据-日数据
            routes.MapPageRoute("rpt-adoc-pos-day", "report/adoc/posday/{page}", "~/report/adidas_omni_channel/rpt_pos/rpt_pos_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //全渠道销售数据-中转
            routes.MapPageRoute("rpt-adoc-pos-transfer", "report/adoc/postransfer", "~/report/adidas_omni_channel/rpt_pos/rpt_pos_transfer.aspx");
            //全渠道销售数据-三小时数据
            routes.MapPageRoute("rpt-adoc-pos-hour", "report/adoc/poshour/{page}", "~/report/adidas_omni_channel/rpt_pos/rpt_pos_hour.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-日数据
            routes.MapPageRoute("rpt-adoc-inv-day", "report/adoc/invday/{page}", "~/report/adidas_omni_channel/rpt_inv/rpt_inv_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-中转
            routes.MapPageRoute("rpt-adoc-inv-transfer", "report/adoc/invtransfer/{page}", "~/report/adidas_omni_channel/rpt_inv/rpt_inv_transfer.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-三小时数据
            routes.MapPageRoute("rpt-adoc-inv-hour", "report/adoc/invhour/{page}", "~/report/adidas_omni_channel/rpt_inv/rpt_inv_hour.aspx", true, new RouteValueDictionary { { "page", null } });
            
            //调拨数据-日数据
            routes.MapPageRoute("rpt-adoc-move-day", "report/adoc/moveday/{page}", "~/report/adidas_omni_channel/rpt_move/rpt_move_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //调拨数据-中转
            routes.MapPageRoute("rpt-adoc-move-transfer", "report/adoc/movetransfer", "~/report/adidas_omni_channel/rpt_move/rpt_move_transfer.aspx");
            //调拨数据-三小时数据
            routes.MapPageRoute("rpt-adoc-move-hour", "report/adoc/movehour/{page}", "~/report/adidas_omni_channel/rpt_move/rpt_move_hour.aspx", true, new RouteValueDictionary { { "page", null } });


            //出入库存数据-三小时数据
            routes.MapPageRoute("rpt-adoc-stock-out-in-data-hour", "report/adoc/stock_out_in_data/{page}", "~/report/adidas_omni_channel/rpt_stock_out_in_data/rpt_stock_out_in_data_hour.aspx", true, new RouteValueDictionary { { "page", null } });



            //智慧零售数据-日数据
            routes.MapPageRoute("rpt-adoc-wifpos-day", "report/adoc/wifposday/{page}", "~/report/adidas_wif_sale/rpt_pos/rpt_pos_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //智慧零售数据-中转
            routes.MapPageRoute("rpt-adoc-wifpos-transfer", "report/adoc/wifpostransfer", "~/report/adidas_wif_sale/rpt_pos/rpt_pos_transfer.aspx");
            //智慧零售数据-三小时数据
            routes.MapPageRoute("rpt-adoc-wifpos-hour", "report/adoc/wifposhour/{page}", "~/report/adidas_wif_sale/rpt_pos/rpt_pos_hour.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-日数据
            routes.MapPageRoute("rpt-adoc-wifinv-day", "report/adoc/wifinvday/{page}", "~/report/adidas_wif_sale/rpt_inv/rpt_inv_day.aspx", true, new RouteValueDictionary { { "page", null } });
            ////点库存数据-中转
            //routes.MapPageRoute("rpt-adoc-wifinv-transfer", "report/adoc/wifinvtransfer/{page}", "~/report/adidas_wif_sale/rpt_inv/rpt_inv_transfer.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-三小时数据
            routes.MapPageRoute("rpt-adoc-wifinv-hour", "report/adoc/wifinvhour/{page}", "~/report/adidas_wif_sale/rpt_inv/rpt_inv_hour.aspx", true, new RouteValueDictionary { { "page", null } });

            //职员数据-三小时数据
            routes.MapPageRoute("rpt-adoc-wifbasicdata-personnel", "report/adoc/wifbasicdata_personnel/{page}", "~/report/adidas_wif_sale/rpt_BasicData/rpt_Personnel.aspx", true, new RouteValueDictionary { { "page", null } });



            //途明
            routes.MapPageRoute("rpt-tumi-pos", "report/tumi/pos/{page}", "~/report/tumi/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-tumi-inv", "report/tumi/inv/{page}", "~/report/tumi/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });


            //On
            routes.MapPageRoute("rpt-on-pos", "report/on/pos/{page}", "~/report/on/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-on-inv", "report/on/inv/{page}", "~/report/on/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-on-invdetail", "report/on/invdetail/{page}", "~/report/on/rpt_invdetail.aspx", true, new RouteValueDictionary { { "page", null } });
            //销售中转
            routes.MapPageRoute("rpt-on-transfer", "report/on/transfer", "~/report/on/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-on-stock-transfer", "report/on/transfer/stock/{page}", "~/report/on/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });



            //VANS
            //POS
            routes.MapPageRoute("rpt-vans-pos", "report/vans/pos/{page}", "~/report/vans/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-vans-inv", "report/vans/inv/{page}", "~/report/vans/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });

            //ArcTeryx
            //POS
            routes.MapPageRoute("rpt-arcteryx-pos", "report/arcteryx/pos/{page}", "~/report/arcteryx/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });





            //IceBreaker ===================================
            //POS
            routes.MapPageRoute("rpt-icebreaker-pos", "report/icebreaker/pos/{page}", "~/report/icebreaker/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-icebreaker-inv", "report/icebreaker/inv/{page}", "~/report/icebreaker/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //中转
            routes.MapPageRoute("rpt-icebreaker-transfer", "report/icebreaker/transfer", "~/report/icebreaker/rpt_transfer.aspx");
            //库存中转
            //routes.MapPageRoute("rpt-icebreaker-stock-transfer", "report/icebreaker/transfer/stock/{page}", "~/report/icebreaker/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });



            //LEE ===================================
            //POS
            routes.MapPageRoute("rpt-lee-pos", "report/lee/pos/{page}", "~/report/lee/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-lee-inv", "report/lee/inv/{page}", "~/report/lee/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });




            //NikeOmniChannel
            //Nike全渠道销售数据-日数据
            routes.MapPageRoute("rpt-nike-pos-day", "report/nike/posday/{page}", "~/report/nike_omni_channel/rpt_pos/rpt_pos_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //Nike全渠道销售数据-中转
            routes.MapPageRoute("rpt-nike-pos-transfer", "report/nike/postransfer", "~/report/nike_omni_channel/rpt_pos/rpt_pos_transfer.aspx");

            //点库存数据-日数据
            routes.MapPageRoute("rpt-nike-inv-day", "report/nike/invday/{page}", "~/report/nike_omni_channel/rpt_inv/rpt_inv_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //点库存数据-中转
            routes.MapPageRoute("rpt-nike-inv-transfer", "report/nike/invtransfer/{page}", "~/report/nike_omni_channel/rpt_inv/rpt_inv_transfer.aspx", true, new RouteValueDictionary { { "page", null } });
          

            //调拨数据-日数据
            routes.MapPageRoute("rpt-nike-move-day", "report/nike/moveday/{page}", "~/report/nike_omni_channel/rpt_move/rpt_move_day.aspx", true, new RouteValueDictionary { { "page", null } });
            //调拨数据-中转
            routes.MapPageRoute("rpt-nike-move-transfer", "report/nike/movetransfer", "~/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx");


            //出入库存数据-日数据
            routes.MapPageRoute("rpt-nike-outin-day", "report/nike/outinday/{page}", "~/report/nike_omni_channel/rpt_outin/rpt_outin_day.aspx", true, new RouteValueDictionary { { "page", null } });


            //TheNorthFace 北面 ===================================
            //POS
            routes.MapPageRoute("rpt-thenorthface-pos", "report/thenorthface/pos/{page}", "~/report/thenorthface/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-thenorthface-inv", "report/thenorthface/inv/{page}", "~/report/thenorthface/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //中转
            routes.MapPageRoute("rpt-thenorthface-transfer", "report/thenorthface/transfer", "~/report/thenorthface/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-thenorthface-stock-transfer", "report/thenorthface/transfer/stock/{page}", "~/report/thenorthface/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });

            //Iker  ===================================
            //POS
            routes.MapPageRoute("rpt-iker-pos", "report/iker/pos/{page}", "~/report/iker/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });
            //库存
            routes.MapPageRoute("rpt-iker-inv", "report/iker/inv/{page}", "~/report/iker/rpt_inv.aspx", true, new RouteValueDictionary { { "page", null } });
            //中转
            routes.MapPageRoute("rpt-iker-transfer", "report/iker/transfer", "~/report/iker/rpt_transfer.aspx");
            //库存中转
            routes.MapPageRoute("rpt-iker-stock-transfer", "report/iker/transfer/stock/{page}", "~/report/iker/rpt_stock_transfer.aspx", true, new RouteValueDictionary { { "page", null } });


            //伯俊
            //POS
            routes.MapPageRoute("rpt-burgeon-poslog", "report/burgeon/poslog/{page}", "~/report/burgeon/rpt_poslog.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-burgeon-pos", "report/burgeon/pos/{page}", "~/report/burgeon/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });


            

            //Nike Api 对接
            //POS
            routes.MapPageRoute("rpt-nikeapi-log", "report/nikeapi/log/{page}", "~/report/nikeapi/rpt_log.aspx", true, new RouteValueDictionary { { "page", null } });
            routes.MapPageRoute("rpt-nikeapi-pos", "report/nikeapi/pos/{page}", "~/report/nikeapi/rpt_pos.aspx", true, new RouteValueDictionary { { "page", null } });



        }

    }
}
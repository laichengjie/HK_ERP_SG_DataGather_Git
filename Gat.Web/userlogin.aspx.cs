using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.Utils;
using Gat.BLL;
using Gat.BLL.Modules;
using System.Web.Security;
using Gat.Web.Common;
using Newtonsoft.Json;
using Gat.Web.Core;

using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace Gat.Web
{
    public partial class userlogin : System.Web.UI.Page
    {
        private GatRepository repository = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string code = Request["code"], password = Request["password"];
                RunEnvironment environment = (RunEnvironment)Convert.ToByte(Request["en"]);
                bool memorize = Request["memopwd"] != null; //记住密码memorize 
                var ai = new AJAXInterop();

                //是否使用dat配置作为数据库配置来源
                var useDat = true;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("ConnStrUseDAT"))
                {
                    useDat = ConfigurationManager.AppSettings["ConnStrUseDAT"] == "true" ? true : false;
                }
                ////还原以前的COOKIE
                var uc = CookieHelper.GetCookie<UserB>(CookieHelper.MemberCookieName);
                CookieUser = uc ?? new UserB();

                if (Page.RouteData.Values["p"] != null) FormsAuthentication.SignOut();

                var filePath = Server.MapPath(@"\bin\ConnectConfig.DAT");
                CommonSettings.ConnFilePath = filePath;
                //检查有没有配置
                HandleArgs HANDLE = XmlHelper.CheckConnectConfig();
                if (HANDLE.Status == HandleStatus.Fail)
                {
                    this.RegisterDialog(HANDLE.Message, DialogIcon.Warning, "jQuery('#btnLogin').remove();");
                }

                if (string.IsNullOrEmpty(code) ||
                    string.IsNullOrEmpty(password)) return;

                repository = new GatRepository(environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest); // 用户登录，使用测试库和正式库都可以
                //var x = new Gat.DAL.ServiceBase();
                //Gat.DAL.Vendor c = new DAL.Vendor { VendorCode = "CC", VendorName = "DD" };
                //var y = x.InsertEntity1(c);
                //var z = y.ToList();
                UserB user = repository.FetchUser(code);
                string result = string.Empty;
                if (user != null)
                {
                    result = user.UserName + "_";
                }
                bool login = user != null && user.Password.EqualTo(password);

                string message = login ? "" : "用户名或者密码错误，请重新输入！" + result;
                ai.StatusCode = login ? "1" : "3";
                ai.Message = message;

                if (login)
                {
                    //写COOKIE
                    string userData = GatPrincipal.BulidUserData(user);
                    CookieHelper.SetAuthenCookie(code, userData);
                    ai.Html = "";
                    if (!string.IsNullOrEmpty(Request["u"]))
                        ai.Html = Request["u"];
                    else
                        ai.Html = this.GetRouteUrl("default", new { });

                    if (memorize) CookieHelper.SetCookie(CookieHelper.MemberCookieName, userData, 7 * 24 * 60);//记住密码
                                                                                                               //写入当前网站上下文
                                                                                                               //将供应商资料写入缓存
                                                                                                               //   var vendor = repository.QueryVendors();
                    List<VendorB> vendor = repository.QueryAllowUsedVendors();
                    VendorB vendorb = new VendorB();

                    if (vendor != null && vendor.Count > 0) vendorb = vendor[0];
                    DGContext context = new DGContext();

                    context.Environment = environment;
                    context.VendorReport = vendorb.ReportPath;
                    CookieHelper.SetCookie<DGContext>(CookieHelper.ContextCookieName, context, 24 * 60);



                    CacheHelper.SetCache("Vendor", vendor);
                }

                this.ResponseToClient(ai);
            }
        }

        /// <summary>
        /// 向客户端返回响应信息
        /// </summary>
        protected void ResponseToClient(AJAXInterop ajax)
        {
            Context.Response.StatusCode = 200;
            Context.Response.Write(JsonConvert.SerializeObject(ajax));
            Context.Response.End();
        }

        /// <summary>
        /// 保存到COOKIE的用户
        /// </summary>
        public UserB CookieUser { get; set; }

        /// <summary>
        /// 记住密码选中状态
        /// </summary>
        /// <returns></returns>
        public string GetChecked()
        {
            if (string.IsNullOrEmpty(CookieUser.UserCode)) return string.Empty;
            return "checked=\"checked\"";
        }


        /// <summary>
        /// 向本页面注册提示窗口
        /// </summary>
        /// <param name="icon">图标</param>
        protected void RegisterDialog(string message, DialogIcon di, string closeCallback = "")
        {
            message = message.Replace("'", @"\'");
            message = message.Replace(Environment.NewLine, "<br />");
            string icon = di == DialogIcon.None ? "" : di.ToString().ToLower();
            string js = @"$.yuanyuan({icon:'" + icon + "',width:350,html:'" + message + "',buttons:{'确定':function(yuan){yuan.hide();}},callback : function(){" + closeCallback + "}});";
            this.RegisterJS(js);
        }

        /// <summary>
        /// 向本页面注册脚本
        /// </summary>
        protected void RegisterJS(string js, string jsKey = "yuanyuan")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), jsKey, js, true);
        }
    }
}
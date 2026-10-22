using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions;
using Gat.Utils;
using Gat.Web.Common;
using System.Configuration;
using Gat.BLL;
using Gat.Web.admin;
using Newtonsoft.Json;

namespace Gat.Web.Core
{
    /// <summary>
    /// 数据提报页面基类
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {
        #region 私有属性

        private readonly string cookieNmae = CookieHelper.ContextCookieName;//上下文的COOKIE名称

        private GatRepository _webRepository; //WEB使用的仓库
        private GatRepository _erpRepository; //ERP使用的仓库

        #endregion

        #region 公开属性

        /// <summary>
        /// 导航一级标题
        /// </summary>
        public string TopTitle
        {
            get
            {
                Label lblTitle = this.Master.FindControl("lblTopTitle") as Label;
                return lblTitle == null ? null : lblTitle.Text;
            }
            set
            {
                Label lblTitle = this.Master.FindControl("lblTopTitle") as Label;
                if (lblTitle != null) lblTitle.Text = value;
            }
        }

        /// <summary>
        /// 导航二级标题
        /// </summary>
        public string SubTitle
        {
            get
            {
                Label lblTitle = this.Master.FindControl("lblSubTitle") as Label;
                return lblTitle == null ? null : lblTitle.Text;
            }
            set
            {
                Label lblTitle = this.Master.FindControl("lblSubTitle") as Label;
                if (lblTitle != null) lblTitle.Text = value;
            }
        }

        /// <summary>
        /// 数据提报WEB上下文
        /// </summary>
        public DGContext DGContext { get; private set; }

        /// <summary>
        /// 报表专用上下文
        /// </summary>
        //public ReportContext RptContext { get;private set; }

        /// <summary>
        /// Web连接类型，测试或者正式帐套
        /// </summary>
        public ConnnectType WebConnect
        {
            get
            {
                if (DGContext == null) return ConnnectType.None;
                return DGContext.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
            }
        }

        /// <summary>
        /// ERP连接类型，测试或者正式帐套
        /// </summary>
        public ConnnectType ERPConnect
        {
            get
            {
                if (DGContext == null) return ConnnectType.None;
                return DGContext.Environment == RunEnvironment.Due ? ConnnectType.ERP : ConnnectType.ERPTest;
            }
        }

        /// <summary>
        /// 路由参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return this.RouteData.Values[key] != null ? this.RouteData.Values[key].ToString() : string.Empty; }

        }

        /// <summary>
        /// WEB使用的数据提报仓库
        /// </summary>
        public GatRepository WebRepository
        {
            get
            {
                if (_webRepository == null) _webRepository = new GatRepository(WebConnect);
                return _webRepository;
            }
        }

        /// <summary>
        /// ERP使用的数据提报仓库
        /// <remarks>ERP仓库只能使用公开的几个存储过程</remarks>
        /// </summary>
        public GatRepository ERPRepository
        {
            get
            {
                if (_erpRepository == null) _erpRepository = new GatRepository(ERPConnect);
                return _erpRepository;
            }
        }

        /// <summary>
        /// 判断当前请求是否为AJAX请求
        /// </summary>
        public bool IsAJAXRequest
        {
            get
            {
                string header = Request.Headers["X-Requested-With"];
                bool isAjaxRequest = (header != null && header.EqualTo("XMLHttpRequest")) ? true : false;

                return isAjaxRequest;
            }
        }

        /// <summary>
        /// AJAX传递的空值
        /// </summary>
        protected IList<string> NullList
        {
            get
            {
                return new List<string> { "null", "undefined" };
            }
        }

        #endregion

        #region 重写方法

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //初始化上下文
            this.DGContext = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            //this.RptContext = CookieHelper.GetCookie<ReportContext>(CookieHelper.ReportContextCookieName);
            if (DGContext == null) GotoError();
        }

        protected override void OnError(EventArgs e)
        {
            // At this point we have information about the error
            // HttpContext ctx = HttpContext.Current;

            //Exception exception = Server.GetLastError();

            //string errorInfo =
            //   "<br>Offending URL: " + Request.Url.ToString() +
            //   "<br>Source: " + exception.Source +
            //   "<br>Message: " + exception.Message +
            //   "<br>Stack trace: " + exception.StackTrace;

            //Response.Write(errorInfo);

            //// --------------------------------------------------
            //// To let the page finish running we clear the error
            //// --------------------------------------------------
            //ctx.Server.ClearError();

            base.OnError(e);

            //Response.Redirect(this.GetRouteUrl("error", new { }));
        }


        #endregion

        #region 辅助方法


        /// <summary>
        /// 转向错误页面
        /// </summary>
        public virtual void GotoError()
        {
            //Response.Redirect(this.GetRouteUrl("error", new object { }), true);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        protected virtual void RefreshCache()
        {
            string key = "Vendor";
            CacheHelper.RemoveCache(key);

            CacheHelper.SetCache(key, WebRepository.QueryVendors());
        }

        /// <summary>
        /// AJAX请求时返回给客户端的信息
        /// <remarks>数据格式为Json</remarks>
        /// </summary>
        /// <param name="response">响应的内容</param>
        /// <param name="hasExceptionOpera">是否包含异常操作，如果有异常处理，Response.End需要放置finally或者更后执行</param>
        public virtual void ResponseToClient(object response, bool hasExceptionOpera = false)
        {
            Response.Clear();
            Response.StatusCode = 200;
            Newtonsoft.Json.Converters.IsoDateTimeConverter timeFormat = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(response, Formatting.Indented, timeFormat));
            //Response.Flush();
            if (!hasExceptionOpera) Response.End();
        }

        /// <summary>
        /// 向本页面注册脚本
        /// </summary>
        protected void RegisterJS(string js, string jsKey = "yuanyuan")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), jsKey, js, true);
        }

        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="icon">图标</param>
        protected void Alert(string message, DialogIcon di, string closeCallback = "")
        {
            message = message.Replace("'", @"\'");
            message = message.Replace(Environment.NewLine, "<br />");
            string icon = di == DialogIcon.None ? "" : di.ToString().ToLower();
            string js = @"utils.dialog({icon:'" + icon + "',width:350,html:'" + message + "',buttons:{'确定':function(){utils.undialog();}},callback : function(){" + closeCallback + "}});";
            this.RegisterJS(js);
        }

        #endregion
    }

    /// <summary>
    /// 提示窗口的图标
    /// </summary>
    public enum DialogIcon
    {
        //icon : 'none' , //'none','correct','warning',loding,error
        /// <summary>
        /// 无
        /// </summary>
        None = 1,
        /// <summary>
        /// 正确
        /// </summary>
        Correct = 2,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 3,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 4
    }

}
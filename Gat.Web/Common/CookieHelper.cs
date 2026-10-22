using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using Gat.Utils;

namespace Gat.Web.Common
{
    public class CookieHelper
    {
        /// <summary>
        /// 设置COOKIE
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encrypt">是否加密cookie</param>
        /// <param name="expires">过期时间，分钟为单位</param>
        public static void SetCookie(string key, string value, int expires = 1440, bool encrypt = true)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (encrypt) value = SecurityHelper.EncryptDes(value);

            if (cookie != null)
            {
                cookie = new HttpCookie(key);//不能直接使用Request的cookie对象直接覆盖Value值，该操作在Chrome新版本浏览器中不支持
                cookie.Value = value;
                cookie.Secure = false;
                cookie.Path = "/";
                cookie.Expires = DateTime.Now.AddMinutes(expires);
                HttpContext.Current.Response.SetCookie(cookie);
                //HttpContext.Current.Response.Cookies.Set(cookie);
            }
            else
            {
                cookie = new HttpCookie(key);
                cookie.Value = value;
                cookie.Secure = false;
                cookie.Path = "/";
                cookie.Expires = DateTime.Now.AddMinutes(expires);
                HttpContext.Current.Response.AppendCookie(cookie);
                //HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 设置COOKIE
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires">过期时间，分钟为单位</param>
        public static void SetCookie<T>(string key, T cookieObj, int expires = 1440, bool encrypt = true)
            where T : class, new()
        {
            string json = JsonConvert.SerializeObject(cookieObj);
            CookieHelper.SetCookie(key, json, expires, encrypt);
        }

        /// <summary>
        /// 取指定KEY值的COOKIE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCookie<T>(string key, bool decrypt = true)
            where T : class, new()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return null;

            string cookieValue = cookie.Value;
            if (decrypt) cookieValue = SecurityHelper.DecryptDes(cookieValue);
            T TJson = JsonConvert.DeserializeObject<T>(cookieValue);
            return TJson;
        }

        /// <summary>
        /// 取指定KEY值的COOKIE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCookie(string key, bool decrypt = true)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return null;
            string cookieValue = cookie.Value;
            if (decrypt) cookieValue = SecurityHelper.DecryptDes(cookieValue);
            return cookieValue;
        }

        /// <summary>
        /// 删除COOKIE
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCookie(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        /// <summary>
        /// 记住密码的CookieName
        /// </summary>
        public static string MemberCookieName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["yuanyuan"]; }
        }

        /// <summary>
        /// 上下文
        /// </summary>
        public static string ContextCookieName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["deedy"]; }
        }

        /// <summary>
        /// 报表上下文
        /// </summary>
        //public static string ReportContextCookieName
        //{
        //    get { return System.Configuration.ConfigurationManager.AppSettings["report"]; }
        //}

        /// <summary>
        /// 写登录Cookie
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public static void SetAuthenCookie(string code, string data)
        {
            //创建新的窗体身份验证票证，它包含用户名、到期时间和该用户所属的角色列表。
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, code, DateTime.Now, DateTime.Now.AddMinutes(60), false, data, "/");
            //加密序列化验证票为字符串
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            //创建一个 cookie 并将加密的票证添加到该 cookie 作为数据。
            //FormsAuthentication.FormsCookieName是根目录web.config的authentication下的name
            HttpCookie loginCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            //输出Cookie,将cookie添加到返回给用户浏览器的cookie集合中
            HttpContext.Current.Response.Cookies.Add(loginCookie);
        }

        /// <summary>
        /// 清除验证COOKIE
        /// </summary>
        /// <param name="redirect"></param>
        public static void ClearAuthenCookie(string redirect)
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Response.Redirect(redirect, true);
        }


    }
}
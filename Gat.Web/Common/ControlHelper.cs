using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
namespace Gat.Web.Common
{
    public class ControlHelper
    {
        /// <summary>
        /// 设置下拉列表选中项
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="value"></param>
        public static void SetSelectedItem(DropDownList ddl, string value)
        {
            if (ddl == null) return;
            ListItem li = ddl.Items.FindByValue(value);
            if (li != null) li.Selected = true;
        }

        public static bool CheckToken(string token)
        {
            try
            {
                if (token == null)
                    return false;
                string userID = ConfigurationManager.AppSettings["userId"];
                string password = ConfigurationManager.AppSettings["userPwd"];
                string[] ckeckList = token.Split('|');
                var a = ckeckList.Length;
                var b = ckeckList[0];
                if (ckeckList.Length != 3 || ckeckList[0] != userID)
                {
                    return false;
                }
                string DM5First = Md5String(ckeckList[0] + password + ckeckList[1]);//编号+密码+时间 MD5加密
                string DM5Second = Md5String(DM5First + ckeckList[0]);//第一次加密+编号 再进行MD5加密
                if (DM5Second == ckeckList[2] || DM5Second.ToLower() == ckeckList[2])
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("安全令牌检验出错，出错原因:{0}", ex.Message));
            }
        }

        /// <summary>
        /// 获取指定的字符串 MD5 后的值
        /// </summary>
        /// <param name="sourceString">MD5 的值</param>
        /// <returns></returns>
        public static string Md5String(string sourceString)
        {
            string _Encrypassword = string.Empty;
            byte[] result = Encoding.UTF8.GetBytes(sourceString);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            _Encrypassword = BitConverter.ToString(output).Replace("-", "");
            return _Encrypassword;
        }
    }
}
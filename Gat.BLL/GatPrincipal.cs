using System;

using System.Security.Principal;
using Gat.Utils;
using Gat.BLL.Modules;
using Newtonsoft.Json;

namespace Gat.BLL
{
    public class GatPrincipal : IPrincipal
    {
        //接口的值
        private IIdentity _identity;

        #region 实现接口

        public System.Security.Principal.IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            //return (Array.BinarySearch(_roles, role) >= 0 ? true : false);
            return true;
        }

        #endregion 实现接口

        #region 自定义属性

        // <summary>
        /// 用户ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public RoleType Role { get; set; }

        #endregion 自定义属性

        #region 公共方法

        //用于在用户登陆时创建FormsAuthenticationTicket实例时提供构造UserData参数的方法
        //本方法尽用于Login页面
        public static string BulidUserData(UserB user)
        {
            if (user == null) user = new UserB();
            return JsonConvert.SerializeObject(user);
        }

        //构造函数
        //userData保存了登录后的用户数据
        //在需要获取用户登录信息的页面，可以实例化一个CustomPrincipal
        //通过它来获取用户登录信息
        public GatPrincipal(IIdentity identity, string userData)
        {
            _identity = identity;
            ExtractUserData(userData);
        }

        //'将BulidUserData构造的UserData还原到本实例中
        private void ExtractUserData(string userData)
        {
            UserB user = JsonConvert.DeserializeObject<UserB>(userData);
            this.Map(user);
        }

        /// <summary>
        /// 映射属性
        /// </summary>
        /// <param name="user"></param>
        private void Map(UserB user)
        {
            if (user == null) user = new UserB();
            this.ID = user.ID;
            this.Password = user.Password;
            this.UserName = user.UserName;
            this.UserCode = user.UserCode;
            this.Role = user.Role;
        }

        #endregion 公共方法
    }
}

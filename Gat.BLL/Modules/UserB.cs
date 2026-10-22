using System;

using Gat.DAL;
using System.Linq.Expressions;
using System.Configuration;
using Gat.Utils;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 用户业务逻辑层实现
    /// </summary>
    public class UserB : LogicBase
    {
        #region 受保护属性

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public UserB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public UserB(ModuleBase moduleBase) : base(moduleBase) { }
        
        ///// <summary>
        ///// 指定实体的主键实例化
        ///// <remarks>使用情景：UI层需要更新</remarks>
        ///// </summary>
        ///// <param name="keyValues">主键键值</param>
        ///// <param name="connType"></param>
        //public UserB(IDictionary<string, object> keyValues, ConnnectType connType) : base(keyValues, connType) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// </summary>
        ///// <param name="service">数据提报数据层服务</param>
        //public UserB(GatService service) : base(service) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// <remarks>此构造函数主要用在查询时实例化逻辑类使用</remarks>
        ///// </summary>
        ///// <param name="module">提供属性值的LINQ层模型</param>
        ///// <param name="service">数据提报数据层服务</param>
        //public UserB(User module, GatService service) : base(module, service) { }

        #endregion

        #region 公开属性

        /// <summary>
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
        /// 角色类型
        /// </summary>
        public RoleType Role { get; set; }


        #endregion

        #region 实体查询

        #endregion

        #region 增删改

        #endregion

        #region 重写方法

        /// <summary>
        /// 从LINQ模型拷贝属性
        /// </summary>
        /// <param name="TSource"></param>
        public override void CopyPropertyFrom(object TSource)
        {
            base.CopyPropertyFrom(TSource);
            User user = TSource as User;
            if (user != null)
            {
                this.Role = (RoleType)user.RoleType;
            }
        }

        /// <summary>
        /// 从LINQ模型拷贝属性
        /// </summary>
        /// <param name="TSource"></param>
        public override void CopyPropertyTo(object TSource)
        {
            base.CopyPropertyTo(TSource);
            User user = TSource as User;
            if (user != null)
            {
                user.RoleType = (byte)this.Role;
            }
        }

        #endregion

        #region 受保护方法

        #endregion

        #region 私有方法

        #endregion
    }
}

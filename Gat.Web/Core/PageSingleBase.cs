using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

namespace Gat.Web.Core
{
    /// <summary>
    /// 明细页面
    /// <remarks>仅实现单主键，多主键需要时再扩展</remarks>
    /// </summary>
    public class PageSingleBase : PageBase
    {
        #region 公开属性

        /// <summary>
        /// 主键键值，复合主键
        /// </summary>
        protected IDictionary<string, object> PrimaryKeyValues { get; private set; }

        /// <summary>
        /// 单主键，并且是自增列
        /// </summary>
        protected int IdentityKey
        {
            get { return Convert.ToInt32(ViewState["ID"]); }
            set { ViewState["ID"] = value; }
        }

        /// <summary>
        /// 新增或者删除记录
        /// </summary>
        protected CUID CUID
        {
            get { return this.IdentityKey == 0 ? Core.CUID.Create : Core.CUID.Update; }
        }

        #endregion

        #region 保护方法

        ///// <summary>
        ///// 设置实体ID
        ///// </summary>
        //protected virtual void SetIdentity(object value)
        //{
        //    if (value == null) throw new ArgumentNullException();
        //    Regex regex = new Regex(@"^\d+$");
        //    if (!regex.IsMatch(value.ToString()))
        //        throw new NotImplementedException();
        //    ViewState["ID"] = value;
        //}

        /// <summary>
        /// 初始化自增ID值
        /// </summary>
        /// <param name="key"></param>
        protected virtual void InitIdentityKey(string key = "id")
        {
            string identity = this[key];
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(identity ?? "")) this.IdentityKey = Convert.ToInt32(identity);
        }

        #endregion

        #region 重写方法

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitIdentityKey();
        }

        #endregion
    }

    /// <summary>
    /// 添加或者修改记录
    /// </summary>
    public enum CUID
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }
}
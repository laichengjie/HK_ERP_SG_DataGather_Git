using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 公司逻辑层
    /// </summary>
    public partial class CompanyB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string CompanyID { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        #endregion

                #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public CompanyB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public CompanyB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

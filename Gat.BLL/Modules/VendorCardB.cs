using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    public partial class VendorCardB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int VendorID { get; set; }

        public string VendorCode { get; set; }

        public string VendorName { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string CardID { get; set; }

        public bool AllowUsed { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDTM { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public VendorCardB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public VendorCardB(ModuleBase moduleBase) : base(moduleBase) { }
       

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

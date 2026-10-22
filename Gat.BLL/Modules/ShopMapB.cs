using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;


namespace Gat.BLL.Modules
{
    public partial class ShopMapB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int ID { get; set; }

        public int VendorID { get; set; }

        public int VendShopID { get; set; }

        public string ShopID { get; set; }

        public string StockID { get; set; }

        public string CompanyID { get; set; }

        public string ShopCode { get; set; }

        public string ShopName { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public string CompanyCode { get; set; }

        public string ComapnyName { get; set; }

        /// <summary>
        /// 是否已经被关联
        /// </summary>
        public bool Mapped { get; set; }

        #endregion

        

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public ShopMapB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public ShopMapB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public ShopMapB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;

namespace Gat.BLL.Modules
{
    public partial class ShopRightB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int ID { get; set; }

        public string PastVersionVendorID { get; set; }

        public string CompanyID { get; set; }

        /// <summary>
        /// 公司的店铺ID 
        /// </summary>
        public string ShopID { get; set; }

        public string ShopCode { get; set; }

        public string ShopName { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public string StockID { get; set; }

        public string CardID { get; set; }

        public bool AllowUsed { get; set; }

        public DateTime ModifyDTM { get; set; }

        public Nullable<int> VendorID { get; set; }

        public Nullable<System.DateTime> ManualGatStartTime { get; set; }

        public Nullable<System.DateTime> ManualGatEndTime { get; set; }



        /// <summary>
        /// 店铺的供应商店铺代码
        /// </summary>
        public string VendorShopCode { get; set; }

        /// <summary>
        /// 关联的供应商店铺名称
        /// </summary>
        public string VendorShopName { get; set; }

        /// <summary>
        /// 关联的供应商店铺ID
        /// </summary>
        public int VendorShopID { get; set; }

        /// <summary>
        /// 店铺关联ID
        /// </summary>
        public int ShopMapID { get; set; }

        /// <summary>
        /// 是否默认折扣
        /// </summary>
        public bool DefaultDiscount { get; set; }

        /// <summary>
        /// 店铺折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 品牌代码
        /// </summary>
        public string TradeCode { get; set; }
         
        /// <summary>
        /// 品牌代码
        /// </summary>
        public string TradeCode2 { get; set; }


        /// <summary>
        /// -是否不计算电商销售
        /// </summary>
        public bool IsNoCalculationSale { get; set; }

        /// <summary>
        /// ---是否正价电商销售
        /// </summary>
        public bool IsECSale { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public ShopRightB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public ShopRightB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public ShopRightB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

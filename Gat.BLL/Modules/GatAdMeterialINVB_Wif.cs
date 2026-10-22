using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatAdMeterialINVB_Wif : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        /// <summary>
        /// 店铺编号
        /// </summary>
        public string store_code { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string product_code { get; set; }

        /// <summary>
        /// 尺码代码
        /// </summary>
        public string size_code { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string bar_code { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public Nullable<int> qty { get; set; }

        /// <summary>
        /// 库存快照日期
        /// </summary>
        public Nullable<System.DateTime> stats_date { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public Nullable<System.DateTime> ModifyTime { get; set; }

        /// <summary>
        /// 提报时间点
        /// </summary>
        public string DrewTime { get; set; }

        /// <summary> 
        /// 店铺ID
        /// </summary>
        public string ShopID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string StockID { get; set; }

        /// <summary>
        /// 公司ID 
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string CardID { get; set; }



        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatAdMeterialINVB_Wif() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatAdMeterialINVB_Wif(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatAdMeterialINVB_Wif(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

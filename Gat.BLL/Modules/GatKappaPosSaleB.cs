using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatKappaPosSaleB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string BillNo { get; set; }

        public int RowNum { get; set; }

        public string OperateType { get; set; }

        public Nullable<System.DateTime> BillDate { get; set; }

        public string StockCode { get; set; }

        public string ShopCode { get; set; }

        public string BarCode { get; set; }

        public decimal Qty { get; set; }

        public Nullable<decimal> RetailPrice { get; set; }

        public Nullable<decimal> Discount { get; set; }

        public Nullable<decimal> PayAmount { get; set; }

        public string TicketNo { get; set; }

        public string CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string CardID { get; set; }

        public string CardName { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatKappaPosSaleB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatKappaPosSaleB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatKappaPosSaleB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

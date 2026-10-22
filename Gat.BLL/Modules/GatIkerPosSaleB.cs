using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatIkerPosSaleB : LogicBase
    {
        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int id { get; set; }
        public int SolutionID { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CardID { get; set; }
        public string CardName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string BillNo { get; set; }
        public Nullable<bool> IsReturn { get; set; }
        public string Sequence { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime BillTime { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string BarCode { get; set; }
        public string SizeCode { get; set; }
        public string SizeName { get; set; }
        public decimal Qty { get; set; }
        public Nullable<decimal> BalaPrice { get; set; }
        public Nullable<decimal> SaleAmount { get; set; }
        public Nullable<decimal> RetailPrice { get; set; }
        public Nullable<DateTime> ModifyTime { get; set; }
        public string SourceBillNo { get; set; }
        public Nullable<bool> IsImport { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造函数
        /// </summary>
        public GatIkerPosSaleB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatIkerPosSaleB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来初始化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatIkerPosSaleB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion
    }

}
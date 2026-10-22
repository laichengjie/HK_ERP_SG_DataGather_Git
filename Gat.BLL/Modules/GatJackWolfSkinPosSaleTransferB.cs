using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatJackWolfSkinPosSaleTransferB : LogicBase 
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public Int32 TransID { get; set; } 
        public string ShopCode { get; set; }

        public DateTime BillDate { get; set; }

        public DateTime BillTime { get; set; }

        public string MaterialCode { get; set; }

        public Nullable<decimal> Qty { get; set; }

        public string BillNo { get; set; }

        public Nullable<decimal> SaleAmount { get; set; }

        public string PersonelName { get; set; }

        public string BarCode { get; set; }

        public string SizeName { get; set; }

        public string CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string CardID { get; set; }

        public string CardName { get; set; }
        public string ShopName { get; set; }
        public string StockName { get; set; }
        public Nullable<decimal> BalaPrice { get; set; }
        public string PersonnelID { get; set; }
        public string SmallTicketNo { get; set; }

        public Boolean? IsImport { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatJackWolfSkinPosSaleTransferB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatJackWolfSkinPosSaleTransferB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatJackWolfSkinPosSaleTransferB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

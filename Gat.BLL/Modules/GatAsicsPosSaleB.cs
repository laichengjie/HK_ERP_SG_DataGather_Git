using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatAsicsPosSaleB : LogicBase
    {
        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string BillCode   { get; set; }

        public DateTime? BillDate { get; set; }

        public string BillType { get; set; }

        public string StoreName { get; set; }

        public string StoreCode { get; set; }

        public string ProdCode { get; set; }

        public string ProdName { get; set; }

        public string ProdUnit { get; set; }

        public decimal? ProdQuantity { get; set; }

        public decimal? ProdAmount { get; set; }


        public DateTime? CreateDTM { get; set; }

        public string StockCode { get; set; }

        public string Remark { get; set; }

        public DateTime? ModifyDTM { get; set; }

        public string CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string ProdBarCode { get; set; }
        public string ProdSpec { get; set; }

        public string CardID { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatAsicsPosSaleB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatAsicsPosSaleB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatAsicsPosSaleB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion
    }
}

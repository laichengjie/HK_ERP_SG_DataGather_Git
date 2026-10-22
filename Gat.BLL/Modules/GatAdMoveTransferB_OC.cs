using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatAdMoveTransferB_OC : LogicBase
    {
        #region 私有|保护属性



        #endregion

        #region 公开属性



        public int _TransID { get; set; }
        public int _SolutionID { get; set; }
        public string _CompanyID { get; set; }
        public string _CompanyName { get; set; }
        public string _BillNo { get; set; }
        public string _ShopCode { get; set; }
        public string _ShopName { get; set; }
        public string _OutStockCode { get; set; }
        public string _OutStockName { get; set; }
        public string _OutStocType { get; set; }
        public string _InStockCode { get; set; }
        public string _InStockName { get; set; }
        public string _InStocType { get; set; }
        public Nullable<System.DateTime> _BillDate { get; set; }
        public Nullable<System.DateTime> _BillTime { get; set; }
        public string _MaterialCode { get; set; }
        public string _BarCode { get; set; }
        public string _SizeCode { get; set; }
        public string _SizeName { get; set; }
        public decimal _Qty { get; set; }
        public Nullable<System.DateTime> _ModifyTime { get; set; }

        public Boolean _IsImport { get; set; }
        public string DrewTime { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatAdMoveTransferB_OC() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatAdMoveTransferB_OC(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatAdMoveTransferB_OC(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion
    }
}

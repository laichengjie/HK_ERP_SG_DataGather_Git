using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;

namespace Gat.BLL.Modules
{
    public partial class StockRightB : LogicBase
    {
        #region 私有|保护属性
        #endregion

        #region 公开属性
        public int ID { get; set; }

        public string PastVersionVendorID { get; set; }

        public string CompanyID { get; set; }

        /// <summary>
        /// 公司仓库
        /// </summary>
        public string StockID { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public string CardID { get; set; }

        public bool AllowUsed { get; set; }

        public DateTime ModifyDTM { get; set; }

        public Nullable<int> VendorID { get; set; }

        public Nullable<DateTime> MaunalGatStartTime { get; set; }



        /// <summary>
        /// 店铺的供应商店铺代码
        /// </summary>
        public string VendorStockCode { get; set; }

        /// <summary>
        /// 关联的供应商店铺名称
        /// </summary>
        public string VendorStockName { get; set; }

        /// <summary>
        /// 关联的供应商店铺ID
        /// </summary>
        public int VendorStockID { get; set; }

        /// <summary>
        /// 店铺关联ID
        /// </summary>
        public int StockMapID { get; set; }

        #region 扩展字段

        #endregion


        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public StockRightB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public StockRightB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public StockRightB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion
    }
}

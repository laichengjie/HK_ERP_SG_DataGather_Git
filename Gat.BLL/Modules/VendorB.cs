using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using System.Linq.Expressions;
using System.Configuration;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 供应商逻辑层
    /// </summary>
    public class VendorB : LogicBase
    {
        #region 公开属性

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 是否允许使用
        /// </summary>
        public bool AllowUsed { get; set; }

        /// <summary>
        /// 创建日间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDTM { get; set; }

        /// <summary>
        /// 供应商执行采集的存储过程名称
        /// </summary>
        public string ProcName { get; set; }

        /// <summary>
        /// 店铺数
        /// </summary>
        public int ShopNumber { get; set; }

        /// <summary>
        /// 运行环境
        /// </summary>
        public RunEnvironment Environment { get; set; }

        /// <summary>
        /// 首次运行时间
        /// </summary>
        public DateTime FirstStartTime { get; set; }

        /// <summary>
        /// 报表的路径 
        /// </summary>
        public ReportMap ReportPath { get; set; }


        private List<VendorCardB> _Cards = null;
        /// <summary>
        /// 供应商经营品牌
        /// </summary>
        public List<VendorCardB> Cards
        {
            get
            {
                if (_Cards != null) return _Cards;
                if (this.Owner == null || this.ID == 0) return null;
                return this.Owner.QueryVendorCards(this.ID);
            }
        }

        /// <summary>
        /// 供应商经营品牌
        /// </summary>
        public string CardNames
        {
            get
            {
                if (this.Cards == null || this.Cards.Count == 0) return null;
                string name = string.Empty;
                foreach (var c in Cards)
                {
                    name += string.IsNullOrEmpty(c.CardName) ? c.CardCode : c.CardName + ";";
                }
                return name;
            }
        }

        private List<VendorShopB> _Shops = null;
        /// <summary>
        /// 供应商的店铺
        /// </summary>
        public List<VendorShopB> Shops
        {
            get
            {
                if (_Shops != null) return _Shops;
                return null;
            }
            internal set { _Shops = value; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public VendorB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public VendorB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 指定数据服务初始化类
        /// </summary>
        public VendorB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// </summary>
        ///// <param name="service">数据提报数据层服务</param>
        //public VendorB(GatService service) : base(service) { }

        ///// <summary>
        ///// 指定实体的主键实例化
        ///// <remarks>使用情景：UI层需要更新</remarks>
        ///// </summary>
        ///// <param name="keyValues">主键键值</param>
        ///// <param name="connType"></param>
        //public VendorB(IDictionary<string, object> keyValues, ConnnectType connType) : base(keyValues, connType) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// <remarks>此构造函数主要用在查询时实例化逻辑类使用</remarks>
        ///// </summary>
        ///// <param name="module">提供属性值的LINQ层模型</param>
        ///// <param name="service">数据提报数据层服务</param>
        //public VendorB(Vendor module, GatService service) : base(module,service) { }

        #endregion

        #region 实体查询

        #endregion

        #region 增删改

        #endregion

        #region 重写方法

        /// <summary>
        /// 将指定类型的属性拷贝到本身的同名属性上
        /// <remarks>用于使用主键查询和更新实体时拷贝属性</remarks>
        /// </summary>
        /// <typeparam name="T">属性值来源类型</typeparam>
        /// <param name="pkcopy">是否拷贝主键值</param>
        /// <returns></returns>
        public override void CopyPropertyFrom(object TSource)
        {
            base.CopyPropertyFrom(TSource);
            Vendor vendorBase = TSource as Vendor;
            if (vendorBase != null)
            {
                this.Environment = (RunEnvironment)vendorBase.Environment;
                this.ReportPath = (ReportMap)(vendorBase.ReportPath ?? 1);
            }
        }

        /// <summary>
        /// 将本身的属性拷贝到指定类型的同名属性上
        /// <remarks>多用于BLL层属性复制</remarks>
        /// </summary>
        /// <typeparam name="T">属性的拷贝目标</typeparam>
        /// <param name="pkcopy">是否拷贝主键值</param>
        /// <returns></returns>
        public override void CopyPropertyTo(object TTarget)
        {
            base.CopyPropertyTo(TTarget);
            Vendor vendorBase = TTarget as Vendor;
            if (vendorBase != null)
            {
                vendorBase.Environment = (byte)this.Environment;
                vendorBase.ReportPath = (byte)this.ReportPath;
            }
        }

        #endregion

        #region 受保护方法

        #endregion

        #region 私有方法

        #endregion
    }
}

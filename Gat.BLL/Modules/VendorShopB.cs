using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    public partial class VendorShopB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int ID { get; set; }

        public int VendorID { get; set; }

        public string VendorCode { get; set; }

        public string VendorName { get; set; }

        public string VendorShopCode { get; set; }

        public string VendorShopName { get; set; }

        public string VendorStockCode { get; set; }

        public string VendorStockName { get; set; }

        public DateTime CreateDate { get; set; }

        #endregion

        #region 扩展属性


        private List<ShopB> _MapShops = null;
        /// <summary>
        /// 关联的店铺
        /// </summary>
        public List<ShopB> MapShops
        {
            get
            {
                if (_MapShops != null) return _MapShops;
                if (this.ID == 0 || this.Owner == null) return null;

                _MapShops = this.Owner.QueryMapShop(this.ID);
                return _MapShops;
            }
        }

        /// <summary>
        /// 映射的店铺名称
        /// </summary>
        public string MapShopCodes
        {
            get
            {
                if (this.MapShops == null || this.MapShops.Count == 0) return null;

                var temp = this.MapShops.Skip(0).Take(4);//取4条数据
                string codes = string.Empty;
                foreach (ShopB shop in temp)
                {
                    codes = codes + shop.ShopCode + ";";
                }
                codes += "&nbsp;&nbsp;<a href='javascript:;' onclick='javascript:showMapping(this);'>[详细]</a>";
                return codes;
                //return this.MapShops.Select(m => m.ShopCode).Aggregate((total, next) => total += next + ";");
            }
        }

        /// <summary>
        /// 映射的店铺ID
        /// </summary>
        public string MapShopIDs
        {
            get
            {
                return null;
                //if (this.MapShops == null || this.MapShops.Count == 0) return null;
                //return this.MapShops.Select(m => m.ShopID).Aggregate((total, next) => total += next + ";");
            }
        }

        #endregion 

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public VendorShopB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public VendorShopB(ModuleBase moduleBase) : base(moduleBase) { }
       
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public VendorShopB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }


        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法

        /// <summary>
        /// 拷贝属性值
        /// </summary>
        /// <param name="TSource"></param>
        public override void CopyPropertyFrom(object TSource)
        {
            base.CopyPropertyFrom(TSource);

            VendShop vendShopBase = TSource as VendShop;
            if (vendShopBase != null)
            {
                this.VendorShopCode = vendShopBase.ShopCode;
                this.VendorShopName = vendShopBase.ShopName;
                this.VendorStockCode = vendShopBase.StockCode;
                this.VendorStockName = vendShopBase.StockName;
            }
        }

        /// <summary>
        /// 将属性值拷贝至
        /// </summary>
        /// <param name="TTarget"></param>
        public override void CopyPropertyTo(object TTarget)
        {
            base.CopyPropertyTo(TTarget);

            VendShop vendShopBase = TTarget as VendShop;
            if (vendShopBase != null)
            {
                vendShopBase.ShopCode = this.VendorShopCode ;
               vendShopBase.ShopName= this.VendorShopName ;
               vendShopBase.StockCode = this.VendorStockCode;
               vendShopBase.StockName = this.VendorStockName;
            }
        }

        #endregion

        #region 公开方法



        #endregion

    }
}

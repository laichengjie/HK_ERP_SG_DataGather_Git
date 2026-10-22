using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;

namespace Gat.BLL.Modules
{
    public class GatMaterialB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string MaterialID { get; set; }

        public string BarCode { get; set; }

        public string MaterialCode { get; set; }

        public string KindName { get; set; }

        public string Series { get; set; }

        public string SubSeries { get; set; }

        public string SexCode { get; set; }

        public string SexName { get; set; }

        public string ModelCode { get; set; }

        public string ModelName { get; set; }

        public string ColorCode { get; set; }

        public string ColorName { get; set; }

        public string SizeCode { get; set; }

        public string SizeName { get; set; }

        public Nullable<decimal> RetailPrice { get; set; }

        public Nullable<System.Char> AutoFlag { get; set; }

        public Nullable<System.DateTime> StockDate { get; set; }

        public string SeasonName { get; set; }

        public Nullable<System.DateTime> SaleDate { get; set; }

        public DateTime CreateDTM { get; set; }


        public string CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string CardID { get; set; }

        public string CardName { get; set; }

        #endregion

        #region 私有|保护方法



        #endregion

         #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatMaterialB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatMaterialB(ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatMaterialB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

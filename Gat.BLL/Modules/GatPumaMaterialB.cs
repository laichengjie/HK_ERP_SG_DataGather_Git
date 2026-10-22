using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;

namespace Gat.BLL.Modules
{
    public class GatPumaMaterialB : LogicBase
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

        public string Sex { get; set; } 

        public string Model { get; set; } 

        public string Color { get; set; } 

        public string Size { get; set; } 

        public Nullable<decimal> RetailPrice { get; set; } 

        public Nullable<System.Char> AutoFlag { get; set; } 

        public Nullable<System.DateTime> StockDate { get; set; } 

        public Nullable<System.DateTime> SaleDate { get; set; } 

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
        public GatPumaMaterialB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatPumaMaterialB(ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatPumaMaterialB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

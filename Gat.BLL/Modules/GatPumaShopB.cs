using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gat.BLL.Modules
{
    public partial class GatPumaShopB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string ShopID { get; set; } 

        public string ShopName { get; set; } 

        public string Address { get; set; } 

        public string City { get; set; } 

        public string Province { get; set; } 

        public string Email { get; set; } 

        public string Country { get; set; } 

        public Nullable<System.DateTime> KDDate { get; set; } 

        public string OperationArea { get; set; } 

        public string ShopType { get; set; } 

        public string ShopCode { get; set; } 

        public string ZyFlag { get; set; } 

        public string DiscountFlag { get; set; } 

        public string FactoryFlag { get; set; } 

        public Nullable<decimal> ShopArea { get; set; } 

        public Nullable<decimal> StockArea { get; set; } 

        public Nullable<decimal> PumaSaleArea { get; set; } 

        public string Environment { get; set; } 

        public DateTime ModifyDTM { get; set; } 

        public string CompanyID { get; set; } 

        public string CompanyName { get; set; } 

        public string CardID { get; set; } 

        public string CardName { get; set; } 

        #endregion


        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatPumaShopB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatPumaShopB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatPumaShopB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}
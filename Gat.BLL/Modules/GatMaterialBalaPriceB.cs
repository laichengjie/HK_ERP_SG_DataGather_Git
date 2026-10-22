using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatMaterialBalaPriceB : LogicBase 
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性



        public int TransID { get; set; }
        public string CompanyID { get; set; }
        public string MaterialCode { get; set; }
        public Nullable<decimal> BalaPrice { get; set; }

        private System.DateTime _ModifyDtm;


        public System.DateTime ModifyDtm { get; set; }

         
        #endregion 

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatMaterialBalaPriceB() : base() { }

        /// <summary> 
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatMaterialBalaPriceB(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatMaterialBalaPriceB(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

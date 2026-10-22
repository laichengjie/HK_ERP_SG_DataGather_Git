using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 解决方案逻辑层
    /// </summary>
    public partial class SolutionB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int ID { get; set; }

        public string Title { get; set; }

        public string PastVersionVendorID { get; set; }

        public string CardID { get; set; }

        public string CompanyID { get; set; }

        public  GatType GatType { get; set; }

        public string Directory { get; set; }

        public ExecType ExecType { get; set; }

        public int ExecFrequency { get; set; }

        public DailyType DailyType { get; set; }

        public Nullable<System.DateTime> OnceStartTime { get; set; }

        public Nullable<RepeatType> RepeatType { get; set; }

        public Nullable<int> RepeatInterval { get; set; }

        public Nullable<System.DateTime> RepeatStartTime { get; set; }

        public DateTime ManualGatStartTime { get; set; }

        public DateTime ManualGatEndTime { get; set; }

        public int ManualInterval { get; set; }

        public bool Enabled { get; set; }

        public string Remark { get; set; }

        public Nullable<int> VendorID { get; set; }

        public string VendorName { get; set; }

        public string VendorCode { get; set; }

        public string CardName { get; set; }

        public string CardCode { get; set; }

        public string CompanyName { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 提报方案使用的存储过程名称
        /// </summary>
        public string ProcName { get; set; }
        /// <summary>
        /// 提取方式
        /// </summary>
        public bool IsAuto { get; set; }
        // public DateTime MyProperty { get; set; }

  


        #endregion

        #region 扩展属性

        private List<SolCardB> _SolCards = null;
        /// <summary>
        /// 提报品牌
        /// </summary>
        public List<SolCardB> SolCards
        {
            get
            {
                if (_SolCards != null) return _SolCards;
                if (this.Owner == null) return null;
                _SolCards = this.Owner.QuerySolCards(this.ID);
                return _SolCards;
            }
        }

        /// <summary>
        /// 店铺数 
        /// </summary>
        private int _ShopNumber = 0;
        /// <summary>
        /// 提报品牌
        /// </summary>
        public int ShopNumber
        {
            get
            {
                _ShopNumber = this.Owner.VendorShopNumberCount((int)this.VendorID,this.CardID,this.ID);
                return _ShopNumber;
            } 
        }



        private List<SolCompanyB> _SolCompanies = null;
        /// <summary>
        /// 提报品牌
        /// </summary>
        public List<SolCompanyB> SolCompanies
        {
            get
            {
                if (_SolCompanies != null) return _SolCompanies;
                if (this.Owner == null) return null;
                _SolCompanies = this.Owner.QuerySolCompanies(this.ID);
                return _SolCompanies;
            }
        }

        /// <summary>
        /// 品牌名称的字符表现形式
        /// </summary>
        public string CardNames
        {
            get
            {
                //if (this.SolCards == null) return null;

                //return this.SolCards
                //           .Select(sc => sc.CardName)
                //           .Aggregate((total, next) => total += next + ";");

                return string.Empty;
            }
        }

        /// <summary>
        /// 公司名称的字符表现形式
        /// </summary>
        public string CompanyNames
        {
            get
            {
                if (this.SolCompanies == null || this.SolCompanies.Count == 0) return null;
                string name = string.Empty;
                foreach (var sc in SolCompanies)
                {
                    name += string.IsNullOrEmpty(sc.CompanyName) ? sc.CompanyCode : sc.CompanyName + ";";
                }
                return name;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public SolutionB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public SolutionB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public SolutionB(ModuleBase moduleBase,GatRepository owner) : base(moduleBase,owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法

        /// <summary>
        /// 从源拷贝属性
        /// </summary>
        /// <param name="TSource"></param>
        public override void CopyPropertyFrom(object TSource)
        {
            base.CopyPropertyFrom(TSource);

            var solution = TSource as Solution;
            if (solution != null)
            {
                this.GatType = (GatType)solution.GatType;
                this.ExecType = (ExecType)solution.ExecType;
                this.DailyType = (DailyType)solution.DailyType;
                this.RepeatType = (RepeatType)(solution.RepeatType ?? 1);

                
            }
        }

        /// <summary>
        /// 将自身属性拷贝至目标
        /// </summary>
        /// <param name="TTarget"></param>
        public override void CopyPropertyTo(object TTarget)
        {
            base.CopyPropertyTo(TTarget);

            var solution = TTarget as Solution;
            if (solution != null)
            {
                solution.GatType = (byte)this.GatType;
                solution.ExecType = (byte)this.ExecType;
                solution.RepeatType = (byte)(this.RepeatType ?? Utils.RepeatType.Minute);
                solution.DailyType = (byte)this.DailyType;
            }
        }

        #endregion

        #region 公开方法



        #endregion

    }
}

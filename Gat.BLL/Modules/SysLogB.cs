using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.Utils;
using Gat.DAL;

namespace Gat.BLL.Modules
{
    public partial class SysLogB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int ID { get; set; }

        public string CompanyID { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public string CardID { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public bool IsSucceed { get; set; }

        public string Script { get; set; }

        public DateTime ModifyDTM { get; set; }

        public string Remark { get; set; }

        public LogType LogType { get; set; }

        public Nullable<int> SolutionID { get; set; }

        public string ShopID { get; set; }

        public string ShopCode { get; set; }

        public string ShopName { get; set; }

        public Nullable<int> VendorID { get; set; }

        public string VendorCode { get; set; }

        public string VendorName { get; set; }



        #endregion

         #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public SysLogB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public SysLogB(ModuleBase moduleBase) : base(moduleBase) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法

        /// <summary>
        /// 从目标拷贝属性
        /// </summary>
        /// <param name="TSource"></param>
        public override void CopyPropertyFrom(object TSource)
        {
            base.CopyPropertyFrom(TSource);

            SysLog logBase = TSource as SysLog;
            if (logBase != null)
            {
                this.LogType = (LogType)logBase.LogType;
            }
        }

        /// <summary>
        /// 将属性拷贝至源
        /// </summary>
        /// <param name="TTarget"></param>
        public override void CopyPropertyTo(object TTarget)
        {
            base.CopyPropertyTo(TTarget);

            SysLog logBase = TTarget as SysLog;
            if (logBase != null)
            {
               logBase.LogType = (byte) this.LogType ;
            }
        }

        #endregion

        #region 公开方法



        #endregion

    }
}

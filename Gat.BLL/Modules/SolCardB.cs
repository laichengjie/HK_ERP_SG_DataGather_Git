using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 提报品牌逻辑层
    /// </summary>
    public class SolCardB : LogicBase
    {
        
        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int SolutionID { get; set; }

        public string CardID { get; set; }

        public string  CardCode { get; set; }

        public string CardName { get; set; }

        #endregion

        #region 扩展属性

        private SolutionB _Solution = null;
        /// <summary>
        /// 属于哪一个提报方案
        /// </summary>
        public SolutionB Soluton
        {
            get
            {
                if (_Solution != null) return _Solution;
                
                if (this.Owner == null) return null;
                _Solution = this.Owner.FetchSolution(this.SolutionID);
                return _Solution;
            }
        }



        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public SolCardB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public SolCardB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public SolCardB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法

        #endregion

        #region 公开方法



        #endregion

    }
}

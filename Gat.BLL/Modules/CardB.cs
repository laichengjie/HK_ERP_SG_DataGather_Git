using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;

namespace Gat.BLL.Modules
{
    /// <summary>
    /// 品牌逻辑层
    /// </summary>
    public class CardB : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public string CardID { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public CardB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public CardB(ModuleBase moduleBase) : base(moduleBase) { }
        
        ///// <summary>
        ///// 指定实体的主键实例化
        ///// <remarks>使用情景：UI层需要更新</remarks>
        ///// </summary>
        ///// <param name="keyValues">主键键值</param>
        ///// <param name="connType"></param>
        //public CardB(IDictionary<string, object> keyValues, ConnnectType connType) : base(keyValues, connType) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// </summary>
        ///// <param name="service">数据提报数据层服务</param>
        //public CardB(GatService service) : base(service) { }

        ///// <summary>
        ///// 通过指定GatService来实例化
        ///// <remarks>此构造函数主要用在查询时实例化逻辑类使用</remarks>
        ///// </summary>
        ///// <param name="module">提供属性值的LINQ层模型</param>
        ///// <param name="service">数据提报数据层服务</param>
        //public CardB(ModuleBase module, GatService service) : base(module, service) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }

}

using Gat.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public partial class GatSpecialMaterialRangeB : LogicBase
    {
        public int Id { get; set; }

        public int SolutionId { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public string ShopCode { get; set; }

        public string ShopName { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatSpecialMaterialRangeB() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类
        /// </summary>
        public GatSpecialMaterialRangeB(ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatSpecialMaterialRangeB(ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }
    }
}

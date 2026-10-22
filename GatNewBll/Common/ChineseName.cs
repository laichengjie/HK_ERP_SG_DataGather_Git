using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Common
{
    /// <summary>
    /// 中文标识
    /// </summary>
    public class ChineseName : Attribute
    {
        /// <summary>
        /// 中文
        /// </summary>
        public string Name { get; set; }
    }
}

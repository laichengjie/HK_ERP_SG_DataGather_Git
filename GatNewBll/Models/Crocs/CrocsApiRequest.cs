using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.Crocs
{
    /// <summary>
    /// Crocs请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CrocsApiRequest<T>
    {
        /// <summary>
        /// 数据体
        /// </summary>
        public List<T> data { get; set; }
    }
}

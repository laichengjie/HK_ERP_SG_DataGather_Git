using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.Utils
{
    /// <summary>
    /// AJAX交互
    /// </summary>
    public  class AJAXInterop
    {
        /// <summary>
        /// 状态代码
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回的Html
        /// </summary>
        public string Html { get; set; }

        public Dictionary<string, object> list;
        

        public AJAXInterop()
        {
            StatusCode = "1";//成功
        }
    }
}

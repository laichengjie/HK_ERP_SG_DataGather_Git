using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// Nike 接口 HTTP 调用结果（同时保留 HTTP 状态码与响应原文，便于按文档 5.x 做重试判定）
    /// </summary>
    public class NikeApiHttpResult
    {
        /// <summary>HTTP 状态码（200/400/401/403/409/422/429/500/503 等）</summary>
        public int StatusCode { get; set; }

        /// <summary>响应原文（JSON）</summary>
        public string Content { get; set; }

        /// <summary>是否为网络层异常（未拿到任何 HTTP 响应，可重试）</summary>
        public bool IsNetworkError { get; set; }

        /// <summary>网络层异常描述</summary>
        public string ErrorMessage { get; set; }
    }
}

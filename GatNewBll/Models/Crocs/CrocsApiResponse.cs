using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models.Crocs
{
    /// <summary>
    /// 请求响应
    /// </summary>
    public class CrocsApiResponse
    {
        /// <summary>
        /// 正确信息
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}

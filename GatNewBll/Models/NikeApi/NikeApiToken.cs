using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GatNewBll.Models.NikeApi
{
    public class NikeApiToken
    {
        /// <summary>
     /// 访问令牌。用于后续请求 Header：Authorization: Bearer &lt;access_token&gt;
     /// 有效期见 expires_in（3600 秒 = 1 小时），建议提前 5 分钟刷新
     /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 签发令牌类型，固定值：urn:ietf:params:oauth:token-type:access_token
        /// </summary>
        public string issued_token_type { get; set; }

        /// <summary>
        /// 令牌类型，固定值：Bearer
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// 令牌有效期（秒），文档为 3600
        /// </summary>
        public long expires_in { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime ExpireAtUtc { get; set; }

        /// <summary>
        /// 授权范围，固定值：data_management:edagc.pde.ingest::create:
        /// </summary>
        public string scope { get; set; }
    }
}

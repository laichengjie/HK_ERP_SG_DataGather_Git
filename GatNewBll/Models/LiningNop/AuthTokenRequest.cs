using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models.LiningNop
{
    public class AuthTokenRequest
    {
        public AuthTokenRequest(bool isRefresh)
        {
            if (isRefresh)
            {
                this.GrantType = "refresh_token";
            }
            else
            {
                this.GrantType = "client_credentials";
            }
        }
        /// <summary>
        /// 授权码 非必填
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; } = "";

        /// <summary>
        /// 授权类型 client_credentials-客户端凭证，refresh_token-刷新token
        /// </summary>
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        /// <summary>
        /// 刷新token，当grant_type为refresh_token时使用，取值为获取token时接口返回refresh_token
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = "";

        public string GetMethod()
        {
            return "open.auth.token.app";
        }
    }
}

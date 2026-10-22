using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models.Burgeon
{ 
    public class BaseRequest
    {
        public BaseRequest()
        {
           // Version = "1.0";
            Charset = "utf-8";
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Format = "json"; 
            SignType = "md5";

        }
        ///// <summary>
        ///// 接入方appId
        ///// </summary>
        //[JsonProperty("app_id")]
        //public string AppId { get; set; }

        ///// <summary>
        ///// 接口名
        ///// </summary>
        //[JsonProperty("method")]
        //public string Method { get; set; }

        ///// <summary>
        ///// 版本号
        ///// </summary>
        //[JsonProperty("version")]
        //public string Version { get; set; }

        /// <summary>
        /// 字符编码 UTF-8
        /// </summary>
        [JsonProperty("charset")]
        public string Charset { get; set; }

        /// <summary>
        /// timestamp
        /// 时间戳，格式为yyyy-MM-dd HH:mm:ss，时区为GMT+8。服务端允许客户端请求最大时间误差为10分钟
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }


        /// <summary>
        /// 请求的业务参数
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("sign_type")]
        public string SignType { get; set; }

        /// <summary>
        /// 请求原始对象
        /// </summary>
        [JsonIgnore]
        public object DataObj { get; set; }


        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models.Skechers
{
    public class GetAppKeyRequest
    {
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}

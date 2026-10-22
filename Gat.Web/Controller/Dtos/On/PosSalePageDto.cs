using GatNewBll.Models;
using GatNewBll.Models.On;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Gat.Web.Controller.Dtos.On
{
    public class OnPosSalePageDto
    {
        [JsonProperty("message")] 
        public string Message { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("qty")]
        public int? Qty { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("data")]  
        public List<GatOnPosSalePageModel> Data { get; set; }
    }
}
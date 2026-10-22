using GatNewBll.Models;
using GatNewBll.Models.ArcTeryx;
using Newtonsoft.Json;
using System.Collections.Generic;
 

namespace Gat.Web.Controller.Dtos.ArcTeryx
{
    public class ArcTeryxPosSalePageDto
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
        public List<GatArcTeryxPosSalePageModel> Data { get; set; }
    }
}
using GatNewBll.Models;
using GatNewBll.Models.TuMi;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Gat.Web.Controller.Dtos.TuMi
{
    public class TuMiPosSalePageDto
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
        public List<GatTuMiPosSalePageModel> Data { get; set; }
    }
}
using GatNewBll.Models.LiningMember;
using GatNewBll.Models.LiningNop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.LiningFx
{
    public class PosSalePageDto
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
        public List<GatLiningnopWholesalePageModel> Data { get; set; }
    }
}
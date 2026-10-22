using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GatEntities.DbModels;
using GatNewBll.Models;

namespace Gat.Web.Controller.Dtos
{
    public class LiningPosSalePageDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("data")]
        public List<GatLiningPosSalePageModel> Data { get; set; }
    }
}
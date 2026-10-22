using GatEntities.DbModels;
using GatNewBll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Gat.Web.Controller.Dtos
{
    public class CrocsInvPageDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("qty")]
        public int Qty { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("data")]
        public List<GatCrocsInventory> Data { get; set; }
    }
}
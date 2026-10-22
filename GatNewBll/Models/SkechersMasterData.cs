using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models
{
    public class SkechersMasterData
    {
        public string refno { get; set; }

        [JsonIgnore]
        public DateTime billDate { get; set; }

        [JsonProperty("billDate")]
        public int billDateInt { get; set; }
        public string storeCode { get; set; }

        [JsonIgnore]
        public string ShopId { get; set; }
        public int sumQty { get; set; }
        public decimal sumTotAmtActual { get; set; }
        public List<SkechersDetailData> detail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models
{
    public class SkechersDetailData
    {
        [JsonIgnore]
        public string refno { get; set; }
        public string no { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
        public decimal priceActual { get; set; }
        public decimal totAmtActual { get; set; }
    }
}

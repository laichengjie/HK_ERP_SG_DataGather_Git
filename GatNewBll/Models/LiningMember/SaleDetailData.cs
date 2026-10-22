using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    /// <summary>
    /// 销售明细数据
    /// </summary>
    public class SaleDetailData
    {
        [JsonIgnore]
        public string tradeCode { get; set; }

        /// <summary>
        /// 退单原单号
        /// </summary>
        [JsonIgnore]
        public string refundCode { get; set; }
        public string productCode { get; set; }
        public string productCounts { get; set; }
        public double productAmount1 { get; set; }
        public double productAmount2 { get; set; }
    }
}

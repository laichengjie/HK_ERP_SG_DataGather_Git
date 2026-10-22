using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    /// <summary>
    /// 销售主数据
    /// </summary>
    public class SaleMasterData
    {
        public string fullName { get; set; }
        public DateTime birthday { get; set; }
        public string address { get; set; }
        public string memberId { get; set; }
        public string gender { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string tradeCode { get; set; }
        /// <summary>
        /// 退单原单号
        /// </summary>
        public string refundCode { get; set; }
        public string customerMobile { get; set; }
        public string tradeSource { get; set; }
        public double tradeAmount1 { get; set; }
        public double tradeAmount2 { get; set; }
        public string tradeStoreCode { get; set; }
        public string tradeType { get; set; }
        public string payTime { get; set; }
        /// <summary>
        /// 数量 
        ///</summary>
        [JsonIgnore]
        public int qty { get; set; }
    }
}

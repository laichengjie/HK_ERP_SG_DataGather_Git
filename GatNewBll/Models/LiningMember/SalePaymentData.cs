using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    /// <summary>
    /// 销售支付数据
    /// </summary>
    public class SalePaymentData
    {
        [JsonIgnore]
        public string tradeCode { get; set; }
        public string paymentType { get; set; }
        public double amount { get; set; }
        public string couponCode { get; set; }
    }
}

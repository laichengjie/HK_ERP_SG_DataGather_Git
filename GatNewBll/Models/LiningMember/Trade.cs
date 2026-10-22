using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    public class Trade
    {
        public string signdate { get; set; }
        public string secret { get; set; }
        public SaleMasterData tradeInfoJson { get; set; }
        public List<SaleDetailData> tradeDetailJson { get; set; }
        public List<SalePaymentData> paymentJson { get; set; }
    }
}

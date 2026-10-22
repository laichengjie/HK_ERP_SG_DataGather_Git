using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.LiningMember
{
    public class GetPageListQueryDto : BaseQueryPageDto
    {
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; }

        public string ShopCode { get; set; }

        public string Status { get; set; }
        public string TradeCode { get; set; }
        public string RefundCode { get; set; }
        public string IsRefund { get; set; }
        public string memberMobile { get; set; }
    }
}
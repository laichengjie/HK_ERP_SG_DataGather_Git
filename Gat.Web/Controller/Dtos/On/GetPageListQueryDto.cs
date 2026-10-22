using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.On
{
    public class GetPageListQueryDto : BaseQueryPageDto
    {
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; }

        public string ShopCode { get; set; }
        public string BillNo { get; set; }

        public string Status { get; set; }

        public string CardShopCode { get; set; }
    }
}
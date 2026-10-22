using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.LiningFx
{
    public class GetPageListQueryDto : BaseQueryPageDto
    {
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; }

        public string VendCustCode { get; set; }
        public string BillNo { get; set; }

        public string Status { get; set; }
    }
}
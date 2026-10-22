using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos
{
    public class LiningPosSaleQueryDto : BaseQueryPageDto
    {

        //"startBillTime": startBillTime,
        //"endBillTime": endBillTime,
        //"startBillDate": startBillDate,
        //"endBillDate": endBillDate,
        //"shopCode": shopCode,
        //"materialCode": materialCode,
        //"sizeCode": sizeCode
        public DateTime? StartBillTime { get; set; }
        public DateTime? EndBillTime { get; set; }
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; }

        public string ShopCode { get; set; }
        public string MaterialCode { get; set; }
        public string SizeName { get; set; }
    }
}
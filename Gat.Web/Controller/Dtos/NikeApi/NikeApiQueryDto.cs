using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.NikeApi
{
    public class NikeApiQueryDto : BaseQueryPageDto
    {
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; } 

        public string ShopCode { get; set; } 
        public string BillNo { get; set; }

        public string Status { get; set; }

        public string CardShopCode { get; set; }

        /// <summary>
        /// 接口类型
        /// </summary>
        public string InterfaceType { get; set; }
    }
}
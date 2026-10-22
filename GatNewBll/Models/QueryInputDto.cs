using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GatNewBll.Models
{
    public class QueryInputDto
    {

        public int? SolutionId { get; set; }

        public int? VendorId { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartBillDate { get; set; }
        public DateTime? EndBillDate { get; set; }

        /// <summary>
        /// 店铺代码
        /// </summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 仓库代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string BillNo { get; set; }


        /// <summary>
        /// 条码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        public string SizeCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary> 
        /// 接口类型
        /// </summary>
        public string InterfaceType { get; set; }



        /// <summary> 
        /// 总行数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary> 
        /// 总数量
        /// </summary>
        public int TotalQty { get; set; }

        /// <summary> 
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
       
    }
}
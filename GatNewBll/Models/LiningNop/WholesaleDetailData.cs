using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningNop
{
    public class WholesaleDetailData
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string externalOrderno { get; set; }

        /// <summary>
        /// 折扣（折后单价为空时，折扣必填，同时存在时优先取折后单价）最大值: 100最小值: 0
        /// </summary>
        public string disc { get; set; }

        /// <summary>
        /// 款号
        /// </summary>
        public string prodCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int qty { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        public string sizeCode { get; set; }

        /// <summary>
        /// 商品代码(传商品代码，或款号+尺码组合，或三个都传。传商品代码时，款号和尺码非必填，同时传款号和尺码时，商品代码非必填)
        /// </summary>
        public string sku { get; set; }

        /// <summary>
        /// 折后单价（折扣为空时，折后单价必填，同时存在时优先取折后单价）
        /// </summary>
        public decimal unitPrice { get; set; }
    }
}

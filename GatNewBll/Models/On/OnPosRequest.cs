using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.On
{

    public class OnPosRequest
    {

        /// <summary>
        /// 销售日期，日期格式可调整 
        ///</summary>
        public string sale_date { get; set; }

        /// <summary>
        /// 昂跑系统中的门店编号
        ///</summary>
        public string store_code { get; set; }

        /// <summary>
        /// "normal" | "exchange" | "return", // exchange & return需回传原订单值到”Original_order_number”
        ///</summary>
        public string order_type { get; set; }


        /// <summary>
        /// 单号
        ///</summary>
        public string order_number { get; set; }


        /// <summary>
        /// 单换货单的原单号,如order_type为normal，请返回空值””,不需要回传“null”
        ///</summary>
        public string original_order_number { get; set; }

        /// <summary>
        /// 更新时间
        ///</summary>
        public string updated_at { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<OnPosDetail> products  { get; set; }

        public class OnPosDetail
        {
            /// <summary>
            ///  SKU 
            ///</summary>
            public string sku { get; set; }

            /// <summary>
            /// 销售数量【qty】	销售数量 2
            ///</summary>
            public int quantity { get; set; }

            // <summary>
            /// 行支付金额，实际行支付金额
            ///</summary>
            public decimal total_paid { get; set; }

            // <summary>
            /// 行原价，产品吊牌价；如果为退单，或者换单，其中qty为-1的行，对应的total_paid,row_total请都使用负数
            ///</summary>
            public decimal row_total { get; set; }
        }

     
    }
}

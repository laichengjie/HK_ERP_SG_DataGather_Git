using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.TuMi.TuMiPosMaster;

namespace GatNewBll.Models.TuMi
{

    public class TuMiPosRequest
    {
        public List<TuMiPosMaster> jsondata { get; set; }
    }


    public class TuMiPosMaster
    {
        /// <summary>
        /// 单号  
        ///</summary>
        public string aicode { get; set; }

        /// <summary>
        /// 交易日期 
        ///</summary>
        public DateTime rq { get; set; }

        /// <summary>
        /// 发货时间 
        ///</summary>
        public DateTime deliveryTime { get; set; }

        /// <summary>
        /// 会员编号 
        ///</summary>
        public string memberCode { get; set; }

        /// <summary>
        /// 销售类型，0正常, 20退货	0正常, 20退货
        ///</summary>
        public string saletype { get; set; }


        /// <summary>
        /// 销售门店, 品牌提供的门店编号 ERP店铺资料的品牌门店编码
        ///</summary>
        public string store { get; set; }

        /// <summary>
        /// 原始订单,对应主订单的aicode 来源单号
        ///</summary>
        public string returnAicode { get; set; }

        /// <summary>
        /// 固定值: MA MA
        ///</summary>
        public string channel { get; set; }


        /// <summary>
        /// 顾客的年龄组（传code 1.2.3...）,默认值:0 
        ///</summary>
        public string nianLingZu { get; set; }

        /// <summary>
        /// 顾客的购物目的,默认值:0 	
        ///</summary>
        public string gouWuMuDi { get; set; }

        /// <summary>
        /// 优惠券id	
        ///</summary>
        public string couponid { get; set; }

        /// <summary>
        ///营业员ID 固定值"0"
        ///</summary>
        public string uid { get; set; }

        /// <summary>
        ///营业员姓名 固定值“”空字符串
        ///</summary>
        public string uName { get; set; }

        /// <summary>
        ///品牌营业员工号 固定值"0"
        ///</summary>
        public string uCode { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<TuMiPosDetail> orderdetail { get; set; }

        /// <summary>
        /// 支付明细
        /// </summary>
        public List<TuMiPosReceiptDetai> paymentdetail { get; set; }



        public string GetMethod()
        {
            return "3";
        }
        public class TuMiPosDetail
        {
            /// <summary>
            ///  SKU 
            ///</summary>
            public string SKU { get; set; }

            /// <summary>
            /// 商品的品牌, 固定值: Tumi Tumi 
            ///</summary>
            public string brand { get; set; }

            /// <summary>
            /// 商品的标准零售价，单价 商品档案的零售价 100
            ///</summary>
            public string product_unit_price { get; set; }

            /// <summary>
            /// 折扣金额    折扣 0.8
            ///</summary>
            public string freermb { get; set; }

            /// <summary>
            /// 销售数量【qty】	销售数量 2
            ///</summary>
            public string product_count { get; set; }

            // <summary>
            /// 明细实收金额【折扣后金额】	 
            ///</summary>
            public string amount { get; set; }

            // <summary>
            /// 明细总额【原总价】	  零售金额 200 
            ///</summary>
            public string order_detail_price { get; set; }

            // <summary>
            /// 是否是赠品,固定值0
            ///</summary>
            public string is_gift { get; set; }

            // <summary>
            /// 是促销活动说明
            ///</summary>
            public string promotion_remark { get; set; }
        }

        public class TuMiPosReceiptDetai
        {
            /// <summary>
            /// 支付方式名称
            ///</summary>
            public string payment_way { get; set; }

            /// <summary>
            /// 金额 
            ///</summary>
            public string payment_amount { get; set; }
        }
    }
}

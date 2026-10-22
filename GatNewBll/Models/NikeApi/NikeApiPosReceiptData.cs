using System;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// Gat_Burgeon_PosSale_Receipt 实体
    /// </summary>
    public class NikeApiPosReceiptData
    {
        /// <summary>
        /// ID（自增主键）
        /// </summary> 
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id
        /// </summary>
        public int SolutionId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 线上订单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternalBillNo { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// 支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 券号
        /// </summary>
        public string BalanceNumber { get; set; }
    }
}
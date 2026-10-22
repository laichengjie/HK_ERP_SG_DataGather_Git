using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.ArcTeryx
{ 
    public class ArcTeryxPosReceiptData
    {
        /// <summary>
        /// 提报方案Id  
        ///</summary>
        public int SolutionId { get; set; }
        /// <summary>
        /// 公司 
        ///</summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        public string BillNo { get; set; }

        /// <summary>
        ///  支付银行
        ///</summary>
        public string CardBank { get; set; }

        /// <summary>
        ///  银行卡卡号
        ///</summary>
        public string CardNumber { get; set; }

        /// <summary>
        ///  优惠金额
        ///</summary>
        public decimal DiscountAmt { get; set; }

        /// <summary>
        ///  应收金额
        ///</summary>
        public decimal PayAmt { get; set; }

        /// <summary>
        /// 支付金额（实收金额）
        ///</summary>
        public decimal value { get; set; }

         
        /// <summary>
        /// 支付方式
        ///</summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 支付时间 销售日期时间 格式：	yyyyMMddhhmmss
        ///</summary>
        public string Time { get; set; }

    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class OnPosDetailData
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

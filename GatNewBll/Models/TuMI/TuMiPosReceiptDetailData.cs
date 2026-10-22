using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi 
{
    public class TuMiPosReceiptDetailData
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
        /// 单号 
        ///</summary>
        public string aicode { get; set; }

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

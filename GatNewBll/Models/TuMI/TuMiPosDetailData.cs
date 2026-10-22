using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi 
{
    public class TuMiPosDetailData
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
}

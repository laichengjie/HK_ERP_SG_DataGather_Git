using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.ArcTeryx
{ 
    public class ArcTeryxPosDetailData
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
        ///  货号
        ///</summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 销售数量【qty】	销售数量 2
        ///</summary>
        public int Quantity { get; set; }

        // <summary>
        /// 商品价格（结算价）
        ///</summary>
        public decimal Price { get; set; }

    }
}

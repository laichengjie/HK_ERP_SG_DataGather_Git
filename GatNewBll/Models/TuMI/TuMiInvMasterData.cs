using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi 
{
    public class TuMiInvMasterData
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
        /// 销售门店, 品牌提供的门店编号 ERP店铺资料的品牌门店编码
        ///</summary>
        public string store { get; set; }

        /// <summary>
        /// 存结余时间 库存提交时间
        ///</summary>
        public DateTime rq { get; set; }


        /// <summary>
        /// 备注  
        ///</summary>
        public string remark { get; set; }

        /// <summary>
        ///品牌ID
        ///</summary>
        public string CardID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        public string ShopID  { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 退货来源单号
        ///</summary>
        public string StockID { get; set; }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi 
{
    public class TuMiInvDetailData
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
        /// SKU
        ///</summary>
        public string SKU { get; set; }

        /// <summary>
        /// 数量
        ///</summary>
        public string qty { get; set; }

       
    }
}

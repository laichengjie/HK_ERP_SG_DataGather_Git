using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.TuMi.TuMiPosMaster;

namespace GatNewBll.Models.TuMi
{

    public class TuMiInvRequest 
    {
        public List<TuMiInvMaster> jsondata { get; set; }
    }


    public class TuMiInvMaster
    {
        /// <summary>
        /// 销售门店, 品牌提供的门店编号(7位数字)
        ///</summary> 
        public string store { get; set; }

        /// <summary>
        /// 库存结余时间
        ///</summary>
        public DateTime rq { get; set; }

        /// <summary>
        /// 发货时间 
        ///</summary>
        public String remark { get; set; }
        
        /// <summary>
        /// 明细
        /// </summary>
        public List<TuMiInvDetai> Inventorydetail { get; set; }

        public class TuMiInvDetai
        {
            /// <summary>
            /// SKU
            ///</summary>
            public string SKU { get; set; }

            /// <summary>
            /// 结余数量 
            ///</summary>
            public string qty { get; set; }
        }
    }
}

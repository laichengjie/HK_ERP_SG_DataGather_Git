using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{

    public class OnInvRequest
    {

        /// <summary>
        /// 昂跑系统中的门店编号
        ///</summary> 
        public string store_code { get; set; }

        /// <summary>
        /// 库存结余时间
        ///</summary>
        public string inventory_date { get; set; }

     
        
        /// <summary>
        /// 明细
        /// </summary>
        public List<OnInvDetai> products { get; set; }

        public class OnInvDetai 
        { 
            /// <summary>
            /// SKU
            ///</summary>
            public string sku { get; set; } 

            /// <summary>
            /// 结余数量 
            ///</summary>
            public int quantity  { get; set; }
        }
    }
}

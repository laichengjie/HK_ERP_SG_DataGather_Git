using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class OnInvDetailData
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
        /// 品牌店铺编号
        ///</summary>
        public string store_code { get; set; }

        /// <summary>
        /// 存结余时间 库存提交时间
        ///</summary>
        public DateTime inventory_date { get; set; }

        /// <summary> 
        /// SKU
        ///</summary> 
        public string sku { get; set; }

        /// <summary>
        /// 数量
        ///</summary>
        public int quantity { get; set; }


        /// <summary> 
        /// 品牌ID
        ///</summary>
        public string CardID { get; set; }

        /// <summary> 
        /// 店铺ID
        ///</summary>
        public string ShopID { get; set; }

        /// <summary> 
        /// 提报时间点
        ///</summary>
        public string DrewTime { get; set; }
    }
}

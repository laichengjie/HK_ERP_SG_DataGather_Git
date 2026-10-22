using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;

namespace GatNewBll.Models
{
    public class GatSkechersPossalePageModel: GatSkechersPossale
    {
        /// <summary>
        /// 店铺编码
        /// </summary>
        public string ErpShopCode { get; set; }

        /// <summary>
        /// 品牌店铺编码
        /// </summary>
        public string TradeCode { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string ShopName { get; set; }
    }
}

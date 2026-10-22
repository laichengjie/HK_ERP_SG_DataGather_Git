using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;

namespace GatNewBll.Models
{
    public class GatShopRightModel:GatShopright
    {
        public string ShopCode { get; set; }
        public string ShopName { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }
        public int VendorShopID { get; set; }

        public string VendorShopName { get; set; }

        public string OMSShopId { get; set; }

        public string OMSShopName { get; set; }
    }
}

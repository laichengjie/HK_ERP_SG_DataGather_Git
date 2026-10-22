using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;

namespace GatNewBll.Models
{
    public class GatStockRightModel:GatStockright
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public int VendorStockID { get; set; }
        public string VendorStockName { get; set; }
    }
}

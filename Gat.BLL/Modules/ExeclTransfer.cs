using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
  public  class ExeclTransfer
    {
        public string CompanyName { get; set; }
        public string ShopCode { get; set; }
        public string CardName { get; set; }
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string MaterialCode { get; set; }
        public string SizeName { get; set; }
        public string BarCode { get; set; }
        public Decimal SalesAmount { get; set; }
        public int Qty { get; set; }
        public string PersonnelID { get; set; }

        public Decimal BalaPrice { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerModel.Request
{
    public class GetSalesRequest
    {
        public DateTime StartBillTime { get; set; }
        public DateTime EndBillTime { get; set; }
    }
}

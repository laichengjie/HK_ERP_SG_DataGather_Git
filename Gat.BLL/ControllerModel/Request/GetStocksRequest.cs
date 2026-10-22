using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerModel.Request
{
    public class GetStocksRequest
    {
        public string Stcode { get; set; } = "";

        public string Oldcode { get; set; } = "";
    }
}

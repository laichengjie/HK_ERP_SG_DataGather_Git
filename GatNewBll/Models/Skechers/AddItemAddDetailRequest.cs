using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.Skechers
{
    public class AddItemAddDetailRequest
    {
        public string ClientId { get; set; }

        public string Version { get; set; }

        public string Timestamp { get; set; }

        public string Sign { get; set; }

        public object Params { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    public class TradeResponse
    {
        /// <summary>
        /// 1正常 其他失败
        /// </summary>
        public string status { get; set; }
        public string message { get; set; }
        public string errorMsg { get; set; }
    }
}

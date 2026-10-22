using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    public class Member
    {
        /// <summary>
        /// 调用时间
        /// </summary>

        public string signdate { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string customerMobile { get; set; }
    }
}

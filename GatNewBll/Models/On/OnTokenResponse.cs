using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class OnTokenResponse
    {

        /// <summary> 
        ///  code 	200：操作成功
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OnToken data { get; set; }
    }

    public class OnToken
    {
        public string token { get; set; }
    }
}

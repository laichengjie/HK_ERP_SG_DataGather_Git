using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi
{
    public class TuMiPosResponse
    {
        /// <summary>
        ///  code String	0：操作成功 1：请求参数异常 2：请求业务异常 3：系统程序异常 40101：access-token过期 -1：订单号重复
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// msg String 成功或失败的相关的具体提示信息如：订单号为空、订单支付信息为空、订单明细为空等相关提示信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models
{
    /// <summary>
    /// 供应商参数列表行（参数表左连接参数值表）
    /// </summary>
    public class VendorParameterModel
    {
        /// <summary>参数代码</summary>
        public string ParameterCode { get; set; }

        /// <summary>参数名称</summary>
        public string ParameterName { get; set; }

        /// <summary>参数值</summary>
        public string ParameterValue { get; set; }

        /// <summary>修改时间（未设置过参数值时为空）</summary>
        public DateTime? ModifyDate { get; set; }
    }
}

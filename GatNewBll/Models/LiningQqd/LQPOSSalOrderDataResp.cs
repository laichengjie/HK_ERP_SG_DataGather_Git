using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningQqd
{

    public class ErrorList
    {
        /// <summary>
        /// 
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 门店编码不存在
        /// </summary>
        public string errorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primaryKey { get; set; }
    }

    public class LQPOSSalOrderData
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 全部失败
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ErrorList> errorList { get; set; }
    }

    public class LQPOSSalOrderDataResp
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LQPOSSalOrderData data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
    }

}

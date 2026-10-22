using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Gat.BLL;
using Gat.Utils;
using Gat.BLL.Modules;

using System.Runtime.Serialization;

namespace Gat.Web.Common
{
    /// <summary>
    /// 当前上下文
    /// </summary>
    [Serializable]
    public class DGContext : IDeserializationCallback
    {
        //字段
        [NonSerialized]
        [Newtonsoft.Json.JsonIgnore]
        public VendorB CurrentVendor; //当前供应商

        /// <summary>
        /// 当前用户
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public GatPrincipal User
        {
            get { return HttpContext.Current.User as GatPrincipal;}
        }

        /// <summary>
        /// 运行帐套，正式或者测试
        /// </summary>
        public RunEnvironment Environment { get; set; }

        #region 报表专用属性

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 报表
        /// </summary>
        public ReportMap VendorReport { get; set; }

        #endregion


        //然在反序列化过程中也不会被初始化，但是假如我们要在反序列化对象中得到total的结果怎么办呢？那我们就需要
        //IDeserializationCallback接口并实现IDeserializationCallback.OnDeserialization方法。

        public void OnDeserialization(object sender)
        {
            //throw new NotImplementedException();
        }
    }
}
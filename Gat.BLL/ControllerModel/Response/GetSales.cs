using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerModel.Response
{
    public class GetSales
    {
        /// <summary>
        /// 销售类型
        /// </summary>
        public string xstype { get; set; }

        /// <summary>
        /// 店铺编码
        /// </summary>
        public string stcode { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string stname { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        public string docdate { get; set; }

        /// <summary>
        /// 销售日期
        /// </summary>
        public string updatedat { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 销售人员名称
        /// </summary>
        public string salesclerk { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        public string last_update { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string Oldcode { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        public string boxcode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int basedigit { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal totalmoney { get; set; }
    }
}

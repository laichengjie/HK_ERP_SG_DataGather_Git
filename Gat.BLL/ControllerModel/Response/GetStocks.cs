using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerModel.Response
{
    public class GetStocks
    {
        /// <summary>
        /// 门店编码
        /// </summary>
        public string stcode { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string oldcode { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        public string boxcode { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal quantity { get; set; }
    }
}

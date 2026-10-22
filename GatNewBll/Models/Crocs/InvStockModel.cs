using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.Crocs
{
    /// <summary>
    /// 库存
    /// </summary>
    public class InvStockModel
    {
        //--DIScode 经销商编码   文本
        /// <summary>
        /// 经销商编码
        /// </summary>
        public string DIScode { get; set; }
        //--Doorcode 门店编码    文本
        /// <summary>
        /// 门店编码
        /// </summary>
        public string Doorcode { get; set; }
        //--SKUcode 款式+颜色 文本
        /// <summary>
        /// 款式+颜色
        /// </summary>
        public string SKUcode { get; set; }
        //--Size 尺寸  文本
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        //--InventoryDate 库存日期，当周周日或当天最后一天 日期
        /// <summary>
        /// 库存日期，当周周日或当天最后一天 日期
        /// </summary>
        public DateTime InventoryDate { get; set; }
        //--ReceivedQty 进货数据，记录一周进货合计 数值
        /// <summary>
        /// ReceivedQty
        /// </summary>
        public float ReceivedQty { get; set; }
        //--Qty 库存数量    数值
        /// <summary>
        /// 库存数量
        /// </summary>
        public float Qty { get; set; }
        //--MSRP 库存吊牌金额，库存数量* 单价  数值
        /// <summary>
        /// 库存吊牌金额，库存数量* 单价
        /// </summary>
        public float MSRP { get; set; }
        //--Updatetime 数据更新时间  日期时间
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public DateTime Updatetime { get; set; }
    }
}

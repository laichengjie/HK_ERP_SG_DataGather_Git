using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.Crocs
{
    /// <summary>
    /// 销售数据
    /// </summary>
    public class PosSaleModel
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
        //--TransactionID 单号编码    文本
        /// <summary>
        /// 单号编码
        /// </summary>
        public string TransactionID { get; set; }
        //--SKUcode 款式+颜色 文本
        /// <summary>
        /// SKUcode
        /// </summary>
        public string SKUcode { get; set; }
        //--Size 尺寸  文本
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        //--TransactionTime 交易时间，日期时分秒 日期时间
        /// <summary>
        /// TransactionTime
        /// </summary>
        public DateTime TransactionTime { get; set; }
        //--Date 业务日期    日期
        /// <summary>
        /// 业务日期
        /// </summary>
        public DateTime Date { get; set; }
        //--Qty 销售数量，退货为负数 数值
        /// <summary>
        /// 销售数量
        /// </summary>
        public float Qty { get; set; }
        //--Revenue 销售实际金额，退货为负数 数值
        /// <summary>
        /// 销售实际金额
        /// </summary>
        public float Revenue { get; set; }
        //--MSRP 零售价金额 = 销售数量 * 单价，退货为负 数值
        /// <summary>
        /// 零售价金额
        /// </summary>
        public float MSRP { get; set; }
        //--Traffic By Door By Date进行统计 数值
        /// <summary>
        /// 进货数量
        /// </summary>
        public string Traffic { get; set; }
        //--Notes 退货原因    文本
        /// <summary>
        /// 退货原因
        /// </summary>
        public string Notes { get; set; }
        //--Updatetime 数据更新时间  日期时间
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public DateTime Updatetime { get; set; }
    }
}

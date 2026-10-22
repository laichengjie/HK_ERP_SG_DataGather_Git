using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 李宁会员销售数据
    ///</summary>
    [SugarTable("gat_lining_member_possale")]
    public class GatLiningMemberPosSale
    {
        public GatLiningMemberPosSale()
        {
            if (CreateDate == null)
            {
                CreateDate = DateTime.Now;
            }
            ModifyDate = DateTime.Now;
        }
        public GatLiningMemberPosSale(int solutionId)
        {
            SolutionId = solutionId;
            if (CreateDate.Year == 1)
            {
                CreateDate = DateTime.Now;
            }
            ModifyDate = DateTime.Now;
        }
        /// <summary>
        /// 1 
        ///</summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "trade_code")]
        public string TradeCode { get; set; }
        /// <summary>
        /// 退单原单号 
        ///</summary>
        [SugarColumn(ColumnName = "refund_code")]
        public string RefundCode { get; set; }
        /// <summary>
        /// 客户名称 
        ///</summary>
        [SugarColumn(ColumnName = "full_name")]
        public string FullName { get; set; }
        /// <summary>
        /// 客户生日 
        ///</summary>
        [SugarColumn(ColumnName = "birthday")]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 客户地址 
        ///</summary>
        [SugarColumn(ColumnName = "address")]
        public string Address { get; set; }
        /// <summary>
        /// 会员Id 
        ///</summary>
        [SugarColumn(ColumnName = "member_id")]
        public string MemberId { get; set; }
        /// <summary>
        /// 客户性别 
        ///</summary>
        [SugarColumn(ColumnName = "gender")]
        public string Gender { get; set; }
        /// <summary>
        /// 客户手机号 
        ///</summary>
        [SugarColumn(ColumnName = "customer_mobile")]
        public string CustomerMobile { get; set; }
        /// <summary>
        /// 单据来源 
        ///</summary>
        [SugarColumn(ColumnName = "trade_source")]
        public string TradeSource { get; set; }
        /// <summary>
        /// 标准金额 
        ///</summary>
        [SugarColumn(ColumnName = "trade_amount1")]
        public decimal TradeAmount1 { get; set; }
        /// <summary>
        /// 结算金额 
        ///</summary>
        [SugarColumn(ColumnName = "trade_amount2")]
        public decimal TradeAmount2 { get; set; }
        /// <summary>
        /// 店铺编码 
        ///</summary>
        [SugarColumn(ColumnName = "trade_store_code")]
        public string TradeStoreCode { get; set; }
        /// <summary>
        /// 订单类型 
        ///</summary>
        [SugarColumn(ColumnName = "trade_type")]
        public string TradeType { get; set; }
        /// <summary>
        /// 支付时间 
        ///</summary>
        [SugarColumn(ColumnName = "pay_time")]
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 是否发送成功 
        ///</summary>
        [SugarColumn(ColumnName = "post_success")]
        public bool PostSuccess { get; set; }
        /// <summary>
        /// 返回消息 
        ///</summary>
        [SugarColumn(ColumnName = "post_return_json")]
        public string PostReturnJson { get; set; }
        /// <summary>
        /// 提报方案Id 
        ///</summary>
        [SugarColumn(ColumnName = "solution_id")]
        public int SolutionId { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
        [SugarColumn(ColumnName = "create_date")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        [SugarColumn(ColumnName = "modify_date")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// 数量 
        ///</summary>
        [SugarColumn(ColumnName = "qty")]
        public int Qty { get; set; }
    }
}

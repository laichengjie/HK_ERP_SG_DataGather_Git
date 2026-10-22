using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_LiningNop_Wholesale")]
    public class GatLiningnopWholesale
    {
        /// <summary>
        /// Id 
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }
        /// <summary>
        /// 提报方案Id 
        ///</summary>
        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }
        /// <summary>
        /// 单据日期 
        ///</summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }
        /// <summary>
        /// 上传数据json 
        ///</summary>
        [SugarColumn(ColumnName = "PostJaon")]
        public string PostJaon { get; set; }
        /// <summary>
        /// 返回数据json 
        ///</summary>
        [SugarColumn(ColumnName = "ReturnJson")]
        public string ReturnJson { get; set; }
        /// <summary>
        /// 是否成功 
        ///</summary>
        [SugarColumn(ColumnName = "IsSuccess")]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 上传时间 
        ///</summary>
        [SugarColumn(ColumnName = "CreateDtm")]
        public DateTime CreateDtm { get; set; }
        /// <summary>
        /// 最后上传时间 
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime ModifyDtm { get; set; }
        /// <summary>
        /// 数量 
        ///</summary>
        [SugarColumn(ColumnName = "Qty")]
        public int? Qty { get; set; }
        /// <summary>
        /// 金额 
        ///</summary>
        [SugarColumn(ColumnName = "Amount")]
        public decimal? Amount { get; set; }
        /// <summary>
        /// 客户编码 
        ///</summary>
        [SugarColumn(ColumnName = "VendCustCode")]
        public string VendCustCode { get; set; }
        /// <summary>
        /// 客户名称 
        ///</summary>
        [SugarColumn(ColumnName = "VendCustName")]
        public string VendCustName { get; set; }
    }
}

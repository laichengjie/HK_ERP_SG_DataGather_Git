using System;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// Nike 接口日志
    /// </summary>
    [SugarTable("Gat_NikeApi_Log")]
    public class GatNikeApiLog
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id
        /// </summary>
        [SugarColumn(ColumnName = "SolutionId", IsNullable = false)]
        public int SolutionId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [SugarColumn(ColumnName = "CompanyID", Length = 20, IsNullable = false)]
        public string CompanyID { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        [SugarColumn(ColumnName = "BillType", Length = 40, IsNullable = false)]
        public string BillType { get; set; }

        /// <summary>
        /// 单据类型ID
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeID", Length = 40, IsNullable = false)]
        public string BillTypeID { get; set; }

        /// <summary>
        /// 单据类型名称
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeName", Length = 40, IsNullable = false)]
        public string BillTypeName { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        [SugarColumn(ColumnName = "BillDate", IsNullable = false)]
        public DateTime BillDate { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(ColumnName = "BillNo", Length = 40, IsNullable = false)]
        public string BillNo { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        [SugarColumn(ColumnName = "CardID", Length = 20)]
        public string CardID { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [SugarColumn(ColumnName = "ShopID", Length = 50)]
        public string ShopID { get; set; }

        /// <summary>
        /// 店铺代码
        /// </summary>
        [SugarColumn(ColumnName = "ShopCode", Length = 50)]
        public string ShopCode { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        [SugarColumn(ColumnName = "StockID", Length = 50)]
        public string StockID { get; set; }

        /// <summary>
        /// 仓库代码
        /// </summary>
        [SugarColumn(ColumnName = "StockCode", Length = 50)]
        public string StockCode { get; set; }


        /// <summary>
        /// 上传数据json
        /// </summary>
        [SugarColumn(ColumnName = "PostJaon", ColumnDataType = "nvarchar(max)")]
        public string PostJaon { get; set; }

        /// <summary>
        /// API 接口的 URL
        /// </summary>
        [SugarColumn(ColumnName = "ApiUrl", Length = 500)]
        public string ApiUrl { get; set; }

        /// <summary>
        /// 返回数据json
        /// </summary>
        [SugarColumn(ColumnName = "ReturnJson", ColumnDataType = "nvarchar(max)")]
        public string ReturnJson { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [SugarColumn(ColumnName = "IsSuccess")]
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [SugarColumn(ColumnName = "CreateDtm")]
        public DateTime? CreateDtm { get; set; }

        /// <summary>
        /// 最后上传时间
        /// </summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime? ModifyDtm { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [SugarColumn(ColumnName = "Qty")]
        public int? Qty { get; set; }

        /// <summary>
        /// 订单总金额（POS单总结算金额）
        /// </summary>
        [SugarColumn(ColumnName = "Amount", ColumnDataType = "decimal(24,6)")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 是否导入
        /// </summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool? IsImport { get; set; }

        /// <summary>
        /// 推送单号
        /// </summary>
        [SugarColumn(ColumnName = "PushBillNo", Length = 40)]
        public string PushBillNo { get; set; }
    }
}
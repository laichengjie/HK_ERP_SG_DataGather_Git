using System;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// Nike 出入库流水接口数据
    /// </summary>
    [SugarTable("Gat_NikeApi_OutIn")]
    public class GatNikeApiOutIn
    {
        /// <summary>
        /// ID（自增）
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
        /// 单号
        /// </summary>
        [SugarColumn(ColumnName = "BillNo", Length = 40, IsNullable = false)]
        public string BillNo { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        [SugarColumn(ColumnName = "BillDate", IsNullable = false)]
        public DateTime BillDate { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        [SugarColumn(ColumnName = "CardID", Length = 20)]
        public string CardID { get; set; }

        /// <summary>
        /// 门店/仓库编码
        /// </summary>
        [SugarColumn(ColumnName = "SITE_CD", Length = 50, IsNullable = false)]
        public string SITE_CD { get; set; }

        /// <summary>
        /// 门店/仓库名称
        /// </summary>
        [SugarColumn(ColumnName = "SITE_NM", Length = 50)]
        public string SITE_NM { get; set; }

        /// <summary>
        /// 条形码（商品唯一识别码）
        /// </summary>
        [SugarColumn(ColumnName = "UPC", Length = 40, IsNullable = false)]
        public string UPC { get; set; }

        /// <summary>
        /// 货号ID
        /// </summary>
        [SugarColumn(ColumnName = "MaterialID", Length = 40)]
        public string MaterialID { get; set; }

        /// <summary>
        /// 货号（商品编码包含款号和颜色号）
        /// </summary>
        [SugarColumn(ColumnName = "PRODUCT_CD", Length = 40)]
        public string PRODUCT_CD { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(ColumnName = "PRODUCT_NM", Length = 60)]
        public string PRODUCT_NM { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        [SugarColumn(ColumnName = "SIZE_CD", Length = 40)]
        public string SIZE_CD { get; set; }

        /// <summary>
        /// 流转方向（出库或入库）
        /// </summary>
        [SugarColumn(ColumnName = "DIRECTION_DESC", Length = 50)]
        public string DIRECTION_DESC { get; set; }

        /// <summary>
        /// 仓库作业流水单号
        /// </summary>
        [SugarColumn(ColumnName = "WMS_ORDER_NO", Length = 40, IsNullable = false)]
        public string WMS_ORDER_NO { get; set; }

        /// <summary>
        /// 发货单号（EPL/RSO）
        /// </summary>
        [SugarColumn(ColumnName = "PROOF_OF_DELIVERY_NO", Length = 40)]
        public string PROOF_OF_DELIVERY_NO { get; set; }

        /// <summary>
        /// 状态类型描述（耐克发货、退还耐克、加盟发货、加盟退货）
        /// </summary>
        [SugarColumn(ColumnName = "STATUS_TYPE_DESC", Length = 40)]
        public string STATUS_TYPE_DESC { get; set; }

        /// <summary>
        /// 流水作业时间戳
        /// </summary>
        [SugarColumn(ColumnName = "TRANSACTION_TMST", ColumnDataType = "datetime2(0)", IsNullable = false)]
        public DateTime TRANSACTION_TMST { get; set; }

        /// <summary>
        /// 流水处理件数
        /// </summary>
        [SugarColumn(ColumnName = "TRANSACTION_QTY")]
        public int? TRANSACTION_QTY { get; set; }

        /// <summary>
        /// 源位置编码
        /// </summary>
        [SugarColumn(ColumnName = "SOURCE_FROM_SITE_CD", Length = 50)]
        public string SOURCE_FROM_SITE_CD { get; set; }

        /// <summary>
        /// 源位置名称
        /// </summary>
        [SugarColumn(ColumnName = "SOURCE_FROM_SITE_NM", Length = 50)]
        public string SOURCE_FROM_SITE_NM { get; set; }

        /// <summary>
        /// 目的位置编码
        /// </summary>
        [SugarColumn(ColumnName = "DESTINATION_TO_SITE_CD", Length = 50)]
        public string DESTINATION_TO_SITE_CD { get; set; }

        /// <summary>
        /// 目的位置名称
        /// </summary>
        [SugarColumn(ColumnName = "DESTINATION_TO_SITE_NM", Length = 50)]
        public string DESTINATION_TO_SITE_NM { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "LAST_UPDATED_TMST", ColumnDataType = "datetime2(0)")]
        public DateTime? LAST_UPDATED_TMST { get; set; }

        /// <summary>
        /// 是否导入
        /// </summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool? IsImport { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [SugarColumn(ColumnName = "IsSuccess")]
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime? ModifyDtm { get; set; }

        /// <summary>
        /// 推送单号
        /// </summary>
        [SugarColumn(ColumnName = "PushBillNo", Length = 40)]
        public string PushBillNo { get; set; }

        /// <summary>
        /// 单据类型名称
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeName", Length = 40, IsNullable = false)]
        public string BillTypeName { get; set; }

        /// <summary>
        /// 单据类型ID
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeID", Length = 20)]
        public string BillTypeID { get; set; }
    }
}
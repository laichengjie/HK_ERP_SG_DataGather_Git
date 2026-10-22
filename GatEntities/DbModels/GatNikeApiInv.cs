using System;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// Nike 库存快照接口数据
    /// </summary>
    [SugarTable("Gat_NikeApi_Inv")]
    public class GatNikeApiInv
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
        /// 品牌ID
        /// </summary>
        [SugarColumn(ColumnName = "CardID", Length = 20)]
        public string CardID { get; set; }

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
        /// 门店/仓库编码
        /// </summary>
        [SugarColumn(ColumnName = "SITE_CD", Length = 50, IsNullable = false)]
        public string SITE_CD { get; set; }

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
        /// 库存日期（库存快照日期）
        /// </summary>
        [SugarColumn(ColumnName = "INVENTORY_DT")]
        public DateTime? INVENTORY_DT { get; set; }

        /// <summary>
        /// 总库存
        /// </summary>
        [SugarColumn(ColumnName = "TOTAL_QTY")]
        public int? TOTAL_QTY { get; set; }

        /// <summary>
        /// 在库库存
        /// </summary>
        [SugarColumn(ColumnName = "ON_HAND_QTY")]
        public int? ON_HAND_QTY { get; set; }

        /// <summary>
        /// 在途库存
        /// </summary>
        [SugarColumn(ColumnName = "IN_TRANSIT_QTY")]
        public int? IN_TRANSIT_QTY { get; set; }

        /// <summary>
        /// 残次品库存
        /// </summary>
        [SugarColumn(ColumnName = "DEFECTIVE_PCS_QTY")]
        public int? DEFECTIVE_PCS_QTY { get; set; }

        /// <summary>
        /// 建议零售单价（吊牌价）
        /// </summary>
        [SugarColumn(ColumnName = "MSRP_LC", ColumnDataType = "numeric(24,6)")]
        public decimal? MSRP_LC { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "LAST_UPDATED_TMST", ColumnDataType = "datetime2(0)", IsNullable = false)]
        public DateTime LAST_UPDATED_TMST { get; set; }

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
    }
}
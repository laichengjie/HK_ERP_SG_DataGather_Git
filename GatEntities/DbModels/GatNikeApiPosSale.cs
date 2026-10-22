using System;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// Nike POS 销售明细接口数据
    /// </summary>
    [SugarTable("Gat_NikeApi_PosSale")]
    public class GatNikeApiPosSale
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
        /// 交易/订单号
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
        /// 门店编码
        /// </summary>
        [SugarColumn(ColumnName = "STORE_CD", Length = 50)]
        public string STORE_CD { get; set; }

        /// <summary>
        /// 业务类型（订单行级别业务分类）
        /// </summary>
        [SugarColumn(ColumnName = "BusinessType", Length = 50)]
        public string BusinessType { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime? BillDate { get; set; }

        /// <summary>
        /// 订单类型（销售、退货、换货）
        /// </summary>
        [SugarColumn(ColumnName = "OrderType", Length = 50)]
        public string OrderType { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        [SugarColumn(ColumnName = "UPC", Length = 40)]
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
        /// 行项目序号
        /// </summary>
        [SugarColumn(ColumnName = "LINE_ITEM_NBR", Length = 10)]
        public string LINE_ITEM_NBR { get; set; }

        /// <summary>
        /// 实际销售单价
        /// </summary>
        [SugarColumn(ColumnName = "NET_SALES_PRC_INCL_TAX", ColumnDataType = "numeric(24,6)")]
        public decimal? NET_SALES_PRC_INCL_TAX { get; set; }

        /// <summary>
        /// 商场补贴金额
        /// </summary>
        [SugarColumn(ColumnName = "REBATE_AMT", ColumnDataType = "numeric(24,6)")]
        public decimal? REBATE_AMT { get; set; }

        /// <summary>
        /// 实际销售金额（客户实收含税总额，退货为负）
        /// </summary>
        [SugarColumn(ColumnName = "NET_SALES_AMT_INCL_TAX", ColumnDataType = "numeric(24,6)")]
        public decimal? NET_SALES_AMT_INCL_TAX { get; set; }

        /// <summary>
        /// 销售数量（退货为负）
        /// </summary>
        [SugarColumn(ColumnName = "SALES_QTY")]
        public int? SALES_QTY { get; set; }

        /// <summary>
        /// 建议零售单价（吊牌价）
        /// </summary>
        [SugarColumn(ColumnName = "MSRP_LC", ColumnDataType = "numeric(24,6)")]
        public decimal? MSRP_LC { get; set; }

        /// <summary>
        /// 吊牌总金额
        /// </summary>
        [SugarColumn(ColumnName = "TOTAL_PARTNER_MSRP_AMT", ColumnDataType = "numeric(24,6)")]
        public decimal? TOTAL_PARTNER_MSRP_AMT { get; set; }

        /// <summary>
        /// 折扣总金额
        /// </summary>
        [SugarColumn(ColumnName = "TOTAL_DISCOUNT_AMT", ColumnDataType = "numeric(24,6)")]
        public decimal? TOTAL_DISCOUNT_AMT { get; set; }

        /// <summary>
        /// 退换货原始门店编码
        /// </summary>
        [SugarColumn(ColumnName = "ORIGINAL_STORE_CD", Length = 50)]
        public string ORIGINAL_STORE_CD { get; set; }

        /// <summary>
        /// 退换货原始交易/订单号
        /// </summary>
        [SugarColumn(ColumnName = "ORIGINAL_ORDER_NBR", Length = 40)]
        public string ORIGINAL_ORDER_NBR { get; set; }

        /// <summary>
        /// 退换货原始交易日期
        /// </summary>
        [SugarColumn(ColumnName = "ORIGINAL_TRANSACTION_DT")]
        public DateTime? ORIGINAL_TRANSACTION_DT { get; set; }

        /// <summary>
        /// 退换货原始行项目序号
        /// </summary>
        [SugarColumn(ColumnName = "ORIGINAL_LINE_ITEM_NBR", Length = 10)]
        public string ORIGINAL_LINE_ITEM_NBR { get; set; }

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
    }
}
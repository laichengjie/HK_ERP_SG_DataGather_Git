using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{

    [SugarTable("Gat_Nike_PosSale_OC")]
    public class GatNikePosSale_OC
    {
        /// <summary>
        /// 主键ID（自增）
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 解决方案ID（主键）
        /// </summary>
        [SugarColumn(ColumnName = "SolutionID", IsPrimaryKey = true)]
        public int SolutionID { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [SugarColumn(ColumnName = "CompanyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }

        /// <summary>
        /// 会员姓名
        /// </summary>
        [SugarColumn(ColumnName = "CardName")]
        public string CardName { get; set; }

        /// <summary>
        /// 门店编码（SHOP_CODE）
        /// </summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [SugarColumn(ColumnName = "ShopName")]
        public string ShopName { get; set; }

        /// <summary>
        /// 发货门店编码（fulfill_shop_code）
        /// </summary>
        [SugarColumn(ColumnName = "StockCode")]
        public string StockCode { get; set; }

        /// <summary>
        /// 发货门店名称
        /// </summary>
        [SugarColumn(ColumnName = "StockName")]
        public string StockName { get; set; }

        /// <summary>
        /// 交易流水号/单据号（主键，TRANSACTION_ID）
        /// </summary>
        [SugarColumn(ColumnName = "BillNo", IsPrimaryKey = true)]
        public string BillNo { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(ColumnName = "Sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// 销售日期（TRANSACTION_DT）
        /// </summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }

        /// <summary>
        /// 发货日期（shipment_DT）
        /// </summary>
        [SugarColumn(ColumnName = "OutBillDate")]
        public DateTime? OutBillDate { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        [SugarColumn(ColumnName = "BillTime")]
        public DateTime BillTime { get; set; }

        /// <summary>
        /// 货号（PRODUCT_ID）
        /// </summary>
        [SugarColumn(ColumnName = "MaterialCode")]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 商品名称（PRODUCT_NM）
        /// </summary>
        [SugarColumn(ColumnName = "MaterialName")]
        public string MaterialName { get; set; }

        /// <summary>
        /// 条形码（UPC）
        /// </summary>
        [SugarColumn(ColumnName = "BarCode")]
        public string BarCode { get; set; }

        /// <summary>
        /// 尺码编码（SIZE_CODE）
        /// </summary>
        [SugarColumn(ColumnName = "SizeCode")]
        public string SizeCode { get; set; }

        /// <summary>
        /// 尺码名称
        /// </summary>
        [SugarColumn(ColumnName = "SizeName")]
        public string SizeName { get; set; }

        /// <summary>
        /// 销售数量（QUANTITY）
        /// </summary>
        [SugarColumn(ColumnName = "Qty")]
        public decimal Qty { get; set; }

        /// <summary>
        /// 实际销售价（PRICE）
        /// </summary>
        [SugarColumn(ColumnName = "BalaPrice")]
        public decimal? BalaPrice { get; set; }

        /// <summary>
        /// 实际销售金额（SALES_AMOUNT）
        /// </summary>
        [SugarColumn(ColumnName = "SaleAmount")]
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 建议零售价/吊牌金额（MSRP_VALUE）
        /// </summary>
        [SugarColumn(ColumnName = "RetailPrice")]
        public decimal? RetailPrice { get; set; }

        /// <summary>
        /// 吊牌总金额（TOTAL_MSRP_VALUE）
        /// </summary>
        [SugarColumn(ColumnName = "RetailAmount")]
        public decimal? RetailAmount { get; set; }

        /// <summary>
        /// 折扣总金额（TOTAL_DISCOUNT_AMT）
        /// </summary>
        [SugarColumn(ColumnName = "DiscountAmount")]
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// 交易类型（默认'1'）
        /// </summary>
        [SugarColumn(ColumnName = "TradeType")]
        public string TradeType { get; set; }

        /// <summary>
        /// 数据处理时间戳（PROCESS_DT）
        /// </summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 是否导入（默认0）
        /// </summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool? IsImport { get; set; }
    }
}
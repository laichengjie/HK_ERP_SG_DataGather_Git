using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    [SugarTable("Gat_Nike_OutIn_OC")]
    public class GatNikeOutIn_OC
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
        /// 出入库移动凭证/单据号（Movement_Document_Number）
        /// </summary>
        [SugarColumn(ColumnName = "BillNo", IsPrimaryKey = true)]
        public string BillNo { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(ColumnName = "Sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }

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
        /// 发货店/仓编码（Departure_SITE_CODE）
        /// </summary>
        [SugarColumn(ColumnName = "OutStockCode")]
        public string OutStockCode { get; set; }

        /// <summary>
        /// 发货店/仓名称
        /// </summary>
        [SugarColumn(ColumnName = "OutStockName")]
        public string OutStockName { get; set; }

        /// <summary>
        /// 目的店/仓编码（Destination_SITE_CODE）
        /// </summary>
        [SugarColumn(ColumnName = "InStockCode")]
        public string InStockCode { get; set; }

        /// <summary>
        /// 目的店/仓名称
        /// </summary>
        [SugarColumn(ColumnName = "InStockName")]
        public string InStockName { get; set; }

        /// <summary>
        /// 数量（Quantity）
        /// </summary>
        [SugarColumn(ColumnName = "Qty")]
        public decimal Qty { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [SugarColumn(ColumnName = "Amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 货物移动类型
        /// </summary>
        [SugarColumn(ColumnName = "MovementType")]
        public string MovementType { get; set; }

        /// <summary>
        /// 装箱单号
        /// </summary>
        [SugarColumn(ColumnName = "PackingListNumber")]
        public string PackingListNumber { get; set; }

        /// <summary>
        /// 出入库日期（transfer_dt）
        /// </summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 是否导入（默认0）
        /// </summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool? IsImport { get; set; }

        /// <summary>
        /// 单据类型ID
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeID")]
        public string BillTypeID { get; set; }

        /// <summary>
        /// 单据类型名称
        /// </summary>
        [SugarColumn(ColumnName = "BillTypeName")]
        public string BillTypeName { get; set; }
    }
}

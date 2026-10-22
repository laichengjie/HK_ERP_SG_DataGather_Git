using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using static System.Net.Mime.MediaTypeNames;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Burgeon_PosSale_Detail")]
    public class GatBurgeonPosSaleDetail
    {
        /// <summary>
        /// ID（自增）
        /// </summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id
        /// </summary>
        [SugarColumn(ColumnName = "SolutionId", IsPrimaryKey = true)]
        public int SolutionId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [SugarColumn(ColumnName = "CompanyID", IsPrimaryKey = true, Length = 20)]
        public string CompanyID { get; set; }

        /// <summary>
        /// 线上订单号
        /// </summary>
        [SugarColumn(ColumnName = "BillNo", IsPrimaryKey = true, Length = 40)]
        public string BillNo { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        [SugarColumn(ColumnName = "ExternalBillNo", Length = 40)]
        public string ExternalBillNo { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }

        /// <summary>
        /// 营业员
        /// </summary>
        [SugarColumn(ColumnName = "Salesrep", Length = 40)]
        public string Salesrep { get; set; }

        /// <summary>
        /// 销售比例
        /// </summary>
        [SugarColumn(ColumnName = "Rate", Length = 10)]
        public string Rate { get; set; }

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
        /// 货号ID
        /// </summary>
        [SugarColumn(ColumnName = "MaterialID", Length = 40)]
        public string MaterialID { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        [SugarColumn(ColumnName = "MaterialCode", Length = 40)]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn(ColumnName = "BarCode", Length = 40)]
        public string BarCode { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        [SugarColumn(ColumnName = "Qty")]
        public int Qty { get; set; }

        /// <summary>
        /// 成交单价
        /// </summary>
        [SugarColumn(ColumnName = "BalaPrice")]
        public decimal BalaPrice { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        [SugarColumn(ColumnName = "PayAmount")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 1销售 2退货 3赠品
        /// </summary>
        [SugarColumn(ColumnName = "Type")]
        public int Type { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(ColumnName = "Sequence", Length = 10)]
        public string Sequence { get; set; }

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
    }


}
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
    [SugarTable("Gat_On_PosSale")]
    public class GatOnPosSale
    {
        /// <summary>
        /// Id 
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id 
        ///</summary>
        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }

        /// <summary>
        /// 公司 
        ///</summary>
       [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }

        /// <summary>
        /// 销售日期 
        ///</summary>
        [SugarColumn(ColumnName = "sale_date")]
        public DateTime sale_date { get; set; }

        /// <summary>
        /// 品牌店铺编号
        ///</summary>
        [SugarColumn(ColumnName = "store_code")]
        public string store_code { get; set; }

        /// <summary>
        /// 单据类型 normal 销售 exchange 换货  return 退货,
        ///</summary>
        [SugarColumn(ColumnName = "order_type")]
        public string order_type { get; set; }

        /// <summary>
        /// 单号
        ///</summary>
        [SugarColumn(ColumnName = "order_number")]
        public string order_number { get; set; }

        /// <summary>
        /// 原单号
        ///</summary>
        [SugarColumn(ColumnName = "original_order_number")]
        public string original_order_number { get; set; }

        /// <summary>
        ///品牌ID
        ///</summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        [SugarColumn(ColumnName = "ShopID")]
        public string ShopID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }
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
        /// 是否导入 
        ///</summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool IsImport { get; set; }


        
    }
}

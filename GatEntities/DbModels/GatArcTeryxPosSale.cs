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
    [SugarTable("Gat_ArcTeryx_PosSale")]
    public class GatArcTeryxPosSale
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
        /// 收银员编码
        ///</summary>
        [SugarColumn(ColumnName = "CashierId")]
        public string CashierId { get; set; }


        /// <summary>
        /// 店铺验证密钥(密码) 由IMPOS提供
        ///</summary>
        [SugarColumn(ColumnName = "CheckCode")]
        public string CheckCode { get; set; }

        /// <summary>
        /// 备注
        ///</summary>
        [SugarColumn(ColumnName = "Comments")]
        public string Comments { get; set; }

        /// <summary>
        /// 收银机编号
        ///</summary>
        [SugarColumn(ColumnName = "TillId")]
        public string TillId { get; set; }

        /// <summary>
        /// 订单时间 
        ///</summary>
        [SugarColumn(ColumnName = "SaleDate")]
        public DateTime SaleDate { get; set; }



        /// <summary>
        ///品牌店铺编号
        ///</summary>
        [SugarColumn(ColumnName = "StoreCode")]
        public string StoreCode { get; set; }

        /// <summary>
        ///销售类型
        ///</summary>
        [SugarColumn(ColumnName = "Saletype")]
        public string Saletype { get; set; }

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
        /// 订单总金额（POS单总结算金额）
        ///</summary>
        [SugarColumn(ColumnName = "Amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 是否导入 
        ///</summary>
        [SugarColumn(ColumnName = "IsImport")]
        public bool IsImport { get; set; }


        /// <summary>
        /// 商场编码 
        ///</summary>
        [SugarColumn(ColumnName = "Mall")]
        public string Mall { get; set; }


        /// <summary>
        /// 推送单号 
        ///</summary>
        [SugarColumn(ColumnName = "PushBillNo")]
        public string PushBillNo { get; set; }


    }
}

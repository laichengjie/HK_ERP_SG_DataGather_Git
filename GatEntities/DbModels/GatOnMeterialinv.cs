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
    [SugarTable("Gat_On_MeterialINV")] 
    public class GatOnMeterialinv
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
        /// 品牌店铺编号
        ///</summary>
        [SugarColumn(ColumnName = "store_code")]
        public string store_code { get; set; }

        /// <summary>
        /// 库存日期 
        ///</summary>
        [SugarColumn(ColumnName = "inventory_date")]
        public DateTime inventory_date { get; set; }

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
        ///提报时间点
        ///</summary>
        [SugarColumn(ColumnName = "DrewTime")]
        public string DrewTime { get; set; }

        /// <summary>
        /// 数量 
        ///</summary>
        [SugarColumn(ColumnName = "Qty")]
        public int Qty { get; set; }

    }
}

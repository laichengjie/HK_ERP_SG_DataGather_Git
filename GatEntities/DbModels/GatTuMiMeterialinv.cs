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
    [SugarTable("Gat_TuMi_MeterialINV")] 
    public class GatTuMiMeterialinv
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
        /// 销售门店, 品牌提供的门店编号 ERP店铺资料的品牌门店编码
        ///</summary>
        [SugarColumn(ColumnName = "store")]
        public string store { get; set; }


        /// <summary>
        /// 存结余时间 库存提交时间
        ///</summary>
        [SugarColumn(ColumnName = "rq")]
        public DateTime rq { get; set; }

        /// <summary>
        /// 备注 
        ///</summary>
        [SugarColumn(ColumnName = "remark")]
        public string remark { get; set; }

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
    }
}

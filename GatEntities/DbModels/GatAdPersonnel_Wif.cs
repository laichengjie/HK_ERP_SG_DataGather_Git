using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ad_Personnel_Wif")]
    public class GatAdPersonnel_Wif
    {
        /// <summary>
        ///  
        ///</summary>
        ///
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        ///  方案id
        ///</summary>
        [SugarColumn(ColumnName = "SolutionID")]
        public int SolutionID { get; set; }

        /// <summary>
        ///  店员编号
        ///</summary>
        [SugarColumn(ColumnName = "staff_code")]
        public string staff_code { get; set; }


        /// <summary>
        ///  姓名
        ///</summary>
        [SugarColumn(ColumnName = "name")]
        public string name { get; set; }

        /// <summary>
        ///  店铺编号
        ///</summary>
        [SugarColumn(ColumnName = "store_code")]
        public string store_code { get; set; }

        /// <summary>
        ///  在职状态
        ///</summary>
        [SugarColumn(ColumnName = "active")]
        public Boolean active { get; set; }

        /// <summary>
        ///  是否导购
        ///</summary>
        [SugarColumn(ColumnName = "saler")]
        public Boolean saler { get; set; }

        /// <summary>
        ///  入职时间
        ///</summary>
        [SugarColumn(ColumnName = "entry_date")]
        public DateTime? entry_date { get; set; }


        /// <summary>
        ///  性别
        ///</summary>
        [SugarColumn(ColumnName = "sex")]
        public string sex { get; set; }


        /// <summary>
        ///  年龄
        ///</summary>
        [SugarColumn(ColumnName = "age")]
        public string age { get; set; }


        /// <summary>
        ///  职位
        ///</summary>
        [SugarColumn(ColumnName = "position")]
        public string position { get; set; }

        /// <summary>
        ///  店员快照 日期
        ///</summary>
        [SugarColumn(ColumnName = "stats_date")]
        public DateTime stats_date { get; set; }


        /// <summary>
        ///  公司ID
        ///</summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        ///  品牌ID
        ///</summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }


        /// <summary>
        ///  职员ID
        ///</summary>
        [SugarColumn(ColumnName = "PersonnelID")]
        public string PersonnelID { get; set; }


        /// <summary>
        ///  修改时间
        ///</summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime ModifyTime { get; set; }



    }
}

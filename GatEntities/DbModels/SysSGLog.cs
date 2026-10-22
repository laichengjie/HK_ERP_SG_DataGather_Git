using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_SGLog")]
    public class SysSGLog
    {
        /// <summary>
        ///  id
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        ///  方案
        ///</summary>
        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }

        /// <summary>
        ///  公司
        ///</summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        ///  品牌
        ///</summary>
        [SugarColumn(ColumnName = "CardName")]
        public string CardName { get; set; }


        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

        /// <summary>
        ///  信息
        ///</summary>
        [SugarColumn(ColumnName = "Message")]
        public string Message { get; set; }

        /// <summary>
        ///  时间
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDTM")]
        public DateTime ModifyDTM { get; set; }


    }
}

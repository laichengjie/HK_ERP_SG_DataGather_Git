using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_Card")]
    public class BasCard
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID" ,IsPrimaryKey = true   )]
         public string CardID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardCode"    )]
         public string CardCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardName"    )]
         public string CardName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_Stock")]
    public class BasStock
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockID" ,IsPrimaryKey = true   )]
         public string StockID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockCode"    )]
         public string StockCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockName"    )]
         public string StockName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID" ,IsPrimaryKey = true   )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="AllowUsed"    )]
         public bool? AllowUsed { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="IsObject"    )]
         public bool? IsObject { get; set; }
    }
}

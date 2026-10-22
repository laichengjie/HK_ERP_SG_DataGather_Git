using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_CompanyCard")]
    public class BasCompanycard
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID" ,IsPrimaryKey = true   )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID" ,IsPrimaryKey = true   )]
         public string CardID { get; set; }
    }
}

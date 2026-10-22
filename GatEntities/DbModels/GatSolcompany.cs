using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_SolCompany")]
    public class GatSolcompany
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionID" ,IsPrimaryKey = true   )]
         public int SolutionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID" ,IsPrimaryKey = true   )]
         public string CompanyID { get; set; }
    }
}

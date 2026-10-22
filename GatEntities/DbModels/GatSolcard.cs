using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_SolCard")]
    public class GatSolcard
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionID" ,IsPrimaryKey = true   )]
         public int SolutionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID" ,IsPrimaryKey = true   )]
         public string CardID { get; set; }
    }
}

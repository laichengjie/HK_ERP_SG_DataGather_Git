using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("sysdiagrams")]
    public class Sysdiagrams
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="name"    )]
         public string Name { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="principal_id"    )]
         public int PrincipalId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="diagram_id" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int DiagramId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="version"    )]
         public int? Version { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="definition"    )]
         public string Definition { get; set; }
    }
}

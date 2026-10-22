using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_MaterialRange")]
    public class GatMaterialrange
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Id"  ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionId"    )]
         public int SolutionId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialCode"    )]
         public string MaterialCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialName"    )]
         public string MaterialName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CreateTime"    )]
         public DateTime CreateTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModifyTime"    )]
         public DateTime ModifyTime { get; set; }
    }
}

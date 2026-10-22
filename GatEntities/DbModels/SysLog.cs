using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Log")]
    public class SysLog
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID"    )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID"    )]
         public string CardID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="IsSucceed"    )]
         public bool IsSucceed { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Script"    )]
         public string Script { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModifyDTM"    )]
         public DateTime ModifyDTM { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Remark"    )]
         public string Remark { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="LogType"    )]
         public byte LogType { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionID"    )]
         public int? SolutionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopID"    )]
         public string ShopID { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((0))
        ///</summary>
         [SugarColumn(ColumnName="VendorID"    )]
         public int? VendorID { get; set; }
    }
}

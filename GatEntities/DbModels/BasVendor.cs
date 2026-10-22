using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_Vendor")]
    public class BasVendor
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendorCode"    )]
         public string VendorCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendorName"    )]
         public string VendorName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="AllowUsed"    )]
         public bool AllowUsed { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CreateDate"    )]
         public DateTime CreateDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModifyDTM"    )]
         public DateTime ModifyDTM { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ProcName"    )]
         public string ProcName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Environment"    )]
         public byte Environment { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="FirstStartTime"    )]
         public DateTime FirstStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ReportPath"    )]
         public byte? ReportPath { get; set; }
    }
}

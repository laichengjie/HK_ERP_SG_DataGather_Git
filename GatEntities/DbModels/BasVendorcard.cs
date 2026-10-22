using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_VendorCard")]
    public class BasVendorcard
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendorID" ,IsPrimaryKey = true   )]
         public int VendorID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID" ,IsPrimaryKey = true   )]
         public string CardID { get; set; }
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
    }
}

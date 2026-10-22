using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_VendCustRight")]
    public class GatVendcustright
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionID"    )]
         public int SolutionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID"    )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendCustID"    )]
         public string VendCustID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendCustCode"    )]
         public string VendCustCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendCustName"    )]
         public string VendCustName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="AllowUsed"    )]
         public bool AllowUsed { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModifyDTM"    )]
         public DateTime ModifyDTM { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="TradeCode"    )]
         public string TradeCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="TradeCode2"    )]
         public string TradeCode2 { get; set; }
    }
}

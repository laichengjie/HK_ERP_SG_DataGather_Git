using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_StockRight")]
    public class GatStockright
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PastVersionVendorID"    )]
         public string PastVersionVendorID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID"    )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockID"    )]
         public string StockID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID"    )]
         public string CardID { get; set; }
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
        /// 默认值: ((0))
        ///</summary>
         [SugarColumn(ColumnName="VendorID"    )]
         public int? VendorID { get; set; }
        /// <summary>
        ///  
        /// 默认值: (getdate())
        ///</summary>
         [SugarColumn(ColumnName="ManualGatStartTime"    )]
         public DateTime? ManualGatStartTime { get; set; }
        /// <summary>
        ///  
        /// 默认值: (getdate())
        ///</summary>
         [SugarColumn(ColumnName="ManualGatEndTime"    )]
         public DateTime? ManualGatEndTime { get; set; }

        /// <summary>
        ///  品牌代码（外部编码）
        ///</summary>
        [SugarColumn(ColumnName = "TradeCode")]
        public string TradeCode { get; set; }
    }
}

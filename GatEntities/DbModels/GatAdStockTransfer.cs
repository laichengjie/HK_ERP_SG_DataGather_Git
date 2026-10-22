using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ad_Stock_Transfer")]
    public class GatAdStockTransfer
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopCode"    )]
         public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockCode"    )]
         public string StockCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="UpdateDate"    )]
         public DateTime? UpdateDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BarCode"    )]
         public string BarCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialCode"    )]
         public string MaterialCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SizeCode"    )]
         public string SizeCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Qty"    )]
         public decimal? Qty { get; set; }
    }
}

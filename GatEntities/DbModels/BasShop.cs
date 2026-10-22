using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_Shop")]
    public class BasShop
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopID" ,IsPrimaryKey = true   )]
         public string ShopID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopCode"    )]
         public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopName"    )]
         public string ShopName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID" ,IsPrimaryKey = true   )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockID"    )]
         public string StockID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_VendShop")]
    public class BasVendshop
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="VendorID"    )]
         public int VendorID { get; set; }
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
         [SugarColumn(ColumnName="StockCode"    )]
         public string StockCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockName"    )]
         public string StockName { get; set; }
        /// <summary>
        ///  
        /// 默认值: (getdate())
        ///</summary>
         [SugarColumn(ColumnName="CreateDate"    )]
         public DateTime CreateDate { get; set; }
    }
}

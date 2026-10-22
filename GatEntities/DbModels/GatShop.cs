using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Shop")]
    public class GatShop
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopID"    )]
         public string ShopID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopName"    )]
         public string ShopName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Address"    )]
         public string Address { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="City"    )]
         public string City { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Province"    )]
         public string Province { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Email"    )]
         public string Email { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Country"    )]
         public string Country { get; set; }
        /// <summary>
        ///  
        /// 默认值: (CONVERT([varchar](10),getdate(),(120)))
        ///</summary>
         [SugarColumn(ColumnName="KDDate"    )]
         public DateTime? KDDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="OperationArea"    )]
         public string OperationArea { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopType"    )]
         public string ShopType { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopCode"    )]
         public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ZyFlag"    )]
         public string ZyFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="DiscountFlag"    )]
         public string DiscountFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="FactoryFlag"    )]
         public string FactoryFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopArea"    )]
         public decimal? ShopArea { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockArea"    )]
         public decimal? StockArea { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="NikeSaleArea"    )]
         public decimal? NikeSaleArea { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Environment"    )]
         public string Environment { get; set; }
        /// <summary>
        ///  
        /// 默认值: (getdate())
        ///</summary>
         [SugarColumn(ColumnName="ModifyDTM"    )]
         public DateTime ModifyDTM { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID"    )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyName"    )]
         public string CompanyName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID"    )]
         public string CardID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardName"    )]
         public string CardName { get; set; }
    }
}

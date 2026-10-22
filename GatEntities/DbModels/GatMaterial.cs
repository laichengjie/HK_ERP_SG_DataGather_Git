using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Material")]
    public class GatMaterial
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialID"    )]
         public string MaterialID { get; set; }
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
         [SugarColumn(ColumnName="KindName"    )]
         public string KindName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Series"    )]
         public string Series { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SubSeries"    )]
         public string SubSeries { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SexCode"    )]
         public string SexCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SexName"    )]
         public string SexName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModelCode"    )]
         public string ModelCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModelName"    )]
         public string ModelName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ColorCode"    )]
         public string ColorCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ColorName"    )]
         public string ColorName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SizeCode"    )]
         public string SizeCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SizeName"    )]
         public string SizeName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="RetailPrice"    )]
         public decimal? RetailPrice { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((0))
        ///</summary>
         [SugarColumn(ColumnName="AutoFlag"    )]
         public string AutoFlag { get; set; }
        /// <summary>
        ///  
        /// 默认值: (CONVERT([varchar](10),getdate(),(120)))
        ///</summary>
         [SugarColumn(ColumnName="StockDate"    )]
         public DateTime? StockDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SeasonName"    )]
         public string SeasonName { get; set; }
        /// <summary>
        ///  
        /// 默认值: (CONVERT([varchar](10),getdate(),(120)))
        ///</summary>
         [SugarColumn(ColumnName="SaleDate"    )]
         public DateTime? SaleDate { get; set; }
        /// <summary>
        ///  
        /// 默认值: (getdate())
        ///</summary>
         [SugarColumn(ColumnName="CreateDTM"    )]
         public DateTime CreateDTM { get; set; }
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

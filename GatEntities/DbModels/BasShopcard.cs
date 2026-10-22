using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Bas_ShopCard")]
    public class BasShopcard
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID" ,IsPrimaryKey = true   )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopID" ,IsPrimaryKey = true   )]
         public string ShopID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID" ,IsPrimaryKey = true   )]
         public string CardID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="AllowUsed"    )]
         public string AllowUsed { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="NetCardCode"    )]
         public string NetCardCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_ShopRight")]
    public class GatShopright
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
         [SugarColumn(ColumnName="ShopID"    )]
         public string ShopID { get; set; }
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
        ///  
        /// 默认值: ((1))
        ///</summary>
         [SugarColumn(ColumnName="DefaultDiscount"    )]
         public bool DefaultDiscount { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((1))
        ///</summary>
         [SugarColumn(ColumnName="Discount"    )]
         public decimal Discount { get; set; }
        /// <summary>
        ///  供应商编码
        ///</summary>
         [SugarColumn(ColumnName="TradeCode"    )]
         public string TradeCode { get; set; }
        /// <summary>
        ///  门店编码
        ///</summary>
        [SugarColumn(ColumnName = "TradeCode2")]
        public string TradeCode2 { get; set; }


        /// <summary> 
        /// 不计算电商销售: ((0))
        ///</summary>
        [SugarColumn(ColumnName = "IsNoCalculationSale")]
        public bool IsNoCalculationSale { get; set; }

        /// <summary> 
        /// 是否正价电商销售: ((0))
        ///</summary>
        [SugarColumn(ColumnName = "IsECSale")]
        public bool IsECSale { get; set; }
    }
}

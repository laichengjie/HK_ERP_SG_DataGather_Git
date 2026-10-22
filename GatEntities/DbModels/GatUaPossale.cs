using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ua_PosSale")]
    public class GatUaPossale
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Sequence"    )]
         public string Sequence { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillDate"    )]
         public DateTime BillDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillTime"    )]
         public DateTime BillTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopName"    )]
         public string ShopName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopCode"    )]
         public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillCode"    )]
         public string BillCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BarCode"    )]
         public string BarCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SizeName"    )]
         public string SizeName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialCode"    )]
         public string MaterialCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Qty"    )]
         public decimal Qty { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Price"    )]
         public decimal Price { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BalaPrice"    )]
         public decimal BalaPrice { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Discount"    )]
         public decimal? Discount { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Amount"    )]
         public decimal Amount { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ArriveDate"    )]
         public string ArriveDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PersonnelCode"    )]
         public string PersonnelCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PersonnelName"    )]
         public string PersonnelName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PromotionID"    )]
         public string PromotionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PromotionName"    )]
         public string PromotionName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CreateDTM"    )]
         public DateTime? CreateDTM { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Remark"    )]
         public string Remark { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ModifyDTM"    )]
         public DateTime? ModifyDTM { get; set; }
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
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SolutionID"    )]
         public int? SolutionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="TransID"    )]
         public int? TransID { get; set; }
    }
}

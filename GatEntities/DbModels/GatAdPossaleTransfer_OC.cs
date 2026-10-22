using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ad_PosSale_transfer_OC")]
    public class GatAdPossaleTransfer_OC
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName="TransID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int TransID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopCode"    )]
         public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillDate"    )]
         public DateTime BillDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillTime"    )]
         public DateTime? BillTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="MaterialCode"    )]
         public string MaterialCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Qty"    )]
         public decimal? Qty { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="BillNo"    )]
         public string BillNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="SaleAmount"    )]
         public decimal? SaleAmount { get; set; } 
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PersonnelID"    )]
         public string PersonnelID { get; set; }
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
         [SugarColumn(ColumnName="BalaPrice"    )]
         public decimal? BalaPrice { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ShopName"    )]
         public string ShopName { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="StockName"    )]
         public string StockName { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "StockCode")]
        public string StockCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName="SmallTicketNo"    )]
         public string SmallTicketNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Remark"    )]
         public string Remark { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "TradeType")]
        public string TradeType { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary> 
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "DrewTime")]
        public string DrewTime { get; set; }
    }
}

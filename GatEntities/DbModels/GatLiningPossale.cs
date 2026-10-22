using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_LiNing_PosSale")]
    public class GatLiningPossale
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillTime")]
        public DateTime BillTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "MaterialCode")]
        public string MaterialCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Qty")]
        public decimal Qty { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SaleAmount")]
        public decimal? SaleAmount { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "PersonelName")]
        public string PersonelName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BarCode")]
        public string BarCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SizeName")]
        public string SizeName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "CompanyName")]
        public string CompanyName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "CardName")]
        public string CardName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BalaPrice")]
        public decimal? BalaPrice { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ShopName")]
        public string ShopName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "StockName")]
        public string StockName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SmallTicketNo")]
        public string SmallTicketNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "PersonnelID")]
        public string PersonnelID { get; set; }
        /// <summary>
        ///  
        /// 默认值: ('')
        ///</summary>
        [SugarColumn(ColumnName = "SaleType")]
        public string SaleType { get; set; }
        /// <summary>
        ///  
        /// 默认值: ('')
        ///</summary>
        [SugarColumn(ColumnName = "MemberMobile")]
        public string MemberMobile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Skechers_PosSale")]
    public class GatSkechersPossale
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillNo", IsPrimaryKey = true)]
        public string BillNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "PostJaon")]
        public string PostJaon { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ReturnJson")]
        public string ReturnJson { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "CreateDtm")]
        public DateTime CreateDtm { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime ModifyDtm { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }

        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }

        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }

        [SugarColumn(ColumnName = "TotalQty")]
        public int TotalQty { get; set; }
        [SugarColumn(ColumnName = "TotalAmount")]
        public decimal TotalAmount { get; set; }
        [SugarColumn(ColumnName = "IsSuccess")]
        public bool IsSuccess { get; set; }


    }
}

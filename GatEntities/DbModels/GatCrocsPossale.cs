using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;

namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Crocs_PosSale")]
    public class GatCrocsPossale
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
        [SugarColumn(ColumnName = "DIScode")]
        public string DIScode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Doorcode")]
        public string Doorcode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "TransactionID")]
        public string TransactionID { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SKUcode")]
        public string SKUcode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Size")]
        public string Size { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "TransactionTime")]
        public DateTime TransactionTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Date")]
        public DateTime Date { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Qty")]
        public int Qty { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Revenue")]
        public decimal Revenue { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "MSRP")]
        public decimal Msrp { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((0))
        ///</summary>
        [SugarColumn(ColumnName = "Traffic")]
        public int Traffic { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Notes")]
        public string Notes { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Updatetime")]
        public DateTime Updatetime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ad_OutIn_OCTB")]
    public class GatAdOutIn_OCTB
    {
        /// <summary>
        ///  
        ///</summary>
        ///
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "SolutionID")]
        public int? SolutionID { get; set; }

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
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }


        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ShopName")]
        public string ShopName { get; set; }

        /// <summary>
        ///  出库仓类型/发货仓类型
        ///</summary>
        public string OutStocType { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "OutStockCode")]
        public string OutStockCode { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "OutStockName")]
        public string OutStockName { get; set; }


        /// <summary>
        ///  入库仓类型/入库货仓类型
        ///</summary>
        public string InStocType { get; set; }


        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "InStockCode")]
        public string InStockCode { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "InStockName")]
        public string InStockName { get; set; }




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
        ///
        [SugarColumn(ColumnName = "Qty")]
        public decimal Qty { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillTypeCode")]
        public string BillTypeCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "BillTypeName")]
        public string BillTypeName { get; set; }







    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary> 
    [SugarTable("Gat_Vans_MeterialINV")]
    public class GatVansMeterialinv
    {

        /// <summary>
        ///  ID
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        ///  提报方案ID
        ///</summary>
        [SugarColumn(ColumnName = "SolutionID")]
        public int? SolutionID { get; set; }

        /// <summary>
        ///  公司
        ///</summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }


        /// <summary>
        ///品牌全称
        ///</summary>
        [SugarColumn(ColumnName = "Brand")]
        public string Brand { get; set; }



        /// <summary>
        ///  品牌公司店铺ID
        ///</summary>
        [SugarColumn(ColumnName = "ShopID")]
        public string ShopID { get; set; }

        /// <summary>
        ///品牌公司店铺代码
        ///</summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        ///店铺简称
        ///</summary>
        [SugarColumn(ColumnName = "ShopName")]
        public string ShopName { get; set; }


        /// <summary>
        ///品牌全称
        ///</summary>
        [SugarColumn(ColumnName = "itemSKU")]
        public string itemSKU { get; set; }


        /// <summary>
        ///货号
        ///</summary>
        [SugarColumn(ColumnName = "ItemNO")]
        public string ItemNO { get; set; }

        /// <summary>
        ///尺码名称
        ///</summary>
        [SugarColumn(ColumnName = "WaistSize")]
        public string WaistSize { get; set; }

        /// <summary>
        ///颜色名称
        ///</summary>
        [SugarColumn(ColumnName = "Inseam")]
        public string Inseam { get; set; }

        /// <summary>
        ///条形码
        ///</summary>
        [SugarColumn(ColumnName = "ItemBarcode")]
        public string ItemBarcode { get; set; }


        

        /// <summary>
        ///  库存（实际库存数）
        ///</summary>
        [SugarColumn(ColumnName = "Quantity")]
        public decimal Quantity { get; set; }
        /// <summary>
        ///颜色名称
        ///</summary>
        [SugarColumn(ColumnName = "InventoryType")]
        public string InventoryType { get; set; }

        /// <summary>
        ///库存日期
        ///</summary>
        [SugarColumn(ColumnName = "InventoryDate")]
        public string InventoryDate { get; set; }

        /// <summary>
        ///  货号
        ///</summary>
        [SugarColumn(ColumnName = "MaterialCode")]
        public string MaterialCode { get; set; }

        /// <summary>
        ///  提报时间
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime ModifyDtm { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary> 
    [SugarTable("Gat_Vans_PosSale")]
    public class GatVansPosSale
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
        ///  单号
        ///</summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }

        /// <summary>
        ///  制单日期
        ///</summary>
        [SugarColumn(ColumnName = "BillTime")]
        public DateTime BillTime { get; set; }

        /// <summary>
        ///  单据日期
        ///</summary>
        [SugarColumn(ColumnName = "BillDate")]
        public DateTime BillDate { get; set; }

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
        [SugarColumn(ColumnName = "Brand")]
        public string Brand { get; set; }

        /// <summary>
        ///品牌全称
        ///</summary>
        [SugarColumn(ColumnName = "SKU")]
        public string SKU { get; set; }

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
        //条码
        ///</summary>
        [SugarColumn(ColumnName = "ItemBarcode")]
        public string ItemBarcode { get; set; }


        /// <summary>
        //金额
        ///</summary>
        [SugarColumn(ColumnName = "PayAmount")]
        public decimal PayAmount { get; set; }

        /// <summary>
        //结算金额（新增）
        ///</summary>
        [SugarColumn(ColumnName = "SettlementAmount")]
        public decimal SettlementAmount { get; set; }
  
        /// <summary>
        ///  数量
        ///</summary>
        [SugarColumn(ColumnName = "Qty")]
        public decimal Qty { get; set; }

        /// <summary>
        //营业员ID
        ///</summary>
        [SugarColumn(ColumnName = "PersonnelID")]
        public string PersonnelID { get; set; }

        /// <summary>
        //营业员代码
        ///</summary>
        [SugarColumn(ColumnName = "PersonnelCode")]
        public string PersonnelCode { get; set; }

        /// <summary>
        //营业员名称
        ///</summary>
        [SugarColumn(ColumnName = "PersonnelName")]
        public string PersonnelName { get; set; }

        /// <summary>
        //会员卡号
        ///</summary>
        [SugarColumn(ColumnName = "MemberCode")]
        public string MemberCode { get; set; }


        /// <summary>
        //会员名称
        ///</summary>
        [SugarColumn(ColumnName = "MemberName")]
        public string MemberName { get; set; }

        /// <summary>
        //活动代码
        ///</summary>
        [SugarColumn(ColumnName = "PromotionCode")]
        public string PromotionCode { get; set; }

        /// <summary>
        //活动名称
        ///</summary>
        [SugarColumn(ColumnName = "PromotionDescription")]
        public string PromotionDescription { get; set; }

        /// <summary>
        //销售类型
        ///</summary>
        [SugarColumn(ColumnName = "SalesType")]
        public string SalesType { get; set; }

        /// <summary>
        //单据主表备注
        ///</summary>
        [SugarColumn(ColumnName = "Remarks")]
        public string Remarks { get; set; }


        /// <summary>
        //商场ID
        ///</summary>
        [SugarColumn(ColumnName = "EmporiumID")]
        public string EmporiumID { get; set; }

        /// <summary>
        //商场代码
        ///</summary>
        [SugarColumn(ColumnName = "EmporiumCode")]
        public string EmporiumCode { get; set; }


        /// <summary>
        //商场门牌号
        ///</summary>
        [SugarColumn(ColumnName = "EDoorplate")]
        public string EDoorplate { get; set; }

        /// <summary>
        //收银机号
        ///</summary>
        [SugarColumn(ColumnName = "TillID")]
        public string TillID { get; set; }

        /// <summary>
        //收银员ID
        ///</summary>
        [SugarColumn(ColumnName = "CashierID")]
        public string CashierID { get; set; }

        /// <summary>
        //收银员名称
        ///</summary>
        [SugarColumn(ColumnName = "CashierName")]
        public string CashierName { get; set; }


        /// <summary>
        //收银员代码
        ///</summary>
        [SugarColumn(ColumnName = "CashierCode")]
        public string CashierCode { get; set; }

        /// <summary>
        //单据类型
        ///</summary>
        [SugarColumn(ColumnName = "BillType")]
        public string BillType { get; set; }

        /// <summary>
        //来源，来自POS销售明细表或者日志表
        ///</summary>
        [SugarColumn(ColumnName = "Source")]
        public string Source { get; set; }

        /// <summary>
        //货号
        ///</summary>
        [SugarColumn(ColumnName = "MaterialCode")]
        public string MaterialCode { get; set; }

        /// <summary>
        ///  提报时间
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDTM")]
        public DateTime? ModifyDTM { get; set; }
    }
}

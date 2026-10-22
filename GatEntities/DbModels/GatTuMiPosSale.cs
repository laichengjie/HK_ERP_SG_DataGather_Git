using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_TuMi_PosSale")]
    public class GatTuMiPosSale
    {
        /// <summary>
        /// Id 
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id 
        ///</summary>
        [SugarColumn(ColumnName = "SolutionId")]
        public int SolutionId { get; set; }


        /// <summary>
        /// 公司 
        ///</summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo  { get; set; }


        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "aicode")]
        public string aicode { get; set; }

        /// <summary>
        /// 交易日期 
        ///</summary>
        [SugarColumn(ColumnName = "rq")]
        public DateTime rq { get; set; }

        /// <summary>
        /// 发货时间 
        ///</summary>
        [SugarColumn(ColumnName = "deliveryTime")]
        public DateTime deliveryTime { get; set; }

        /// <summary>
        /// 会员编号 
        ///</summary>
        [SugarColumn(ColumnName = "memberCode")]
        public string memberCode { get; set; }

        /// <summary>
        /// 销售类型，0正常, 20退货	0正常, 20退货
        ///</summary>
        [SugarColumn(ColumnName = "saletype")]
        public string saletype { get; set; }


        /// <summary>
        /// 销售门店, 品牌提供的门店编号 ERP店铺资料的品牌门店编码
        ///</summary>
        [SugarColumn(ColumnName = "store")]
        public string store { get; set; }

        /// <summary>
        /// 原始订单,对应主订单的aicode 来源单号
        ///</summary>
        [SugarColumn(ColumnName = "returnAicode")]
        public string returnAicode { get; set; }

        /// <summary>
        /// 固定值: MA MA
        ///</summary>
        [SugarColumn(ColumnName = "channel")]
        public string channel { get; set; }


        /// <summary>
        /// 顾客的年龄组（传code 1.2.3...）,默认值:0 
        ///</summary>
        [SugarColumn(ColumnName = "nianLingZu")]
        public string nianLingZu { get; set; }

        /// <summary>
        /// 顾客的购物目的,默认值:0 	
        ///</summary>
        [SugarColumn(ColumnName = "gouWuMuDi")]
        public string gouWuMuDi { get; set; }

        /// <summary>
        /// 优惠券id	
        ///</summary>
        [SugarColumn(ColumnName = "couponid")]
        public string couponid { get; set; }

        /// <summary>
        ///营业员ID 固定值"0"
        ///</summary>
        [SugarColumn(ColumnName = "uid")]
        public string uid { get; set; }

        /// <summary>
        ///营业员姓名 固定值“”空字符串
        ///</summary>
        [SugarColumn(ColumnName = "uName")]
        public string uName { get; set; }

        /// <summary>
        ///品牌营业员工号 固定值"0"
        ///</summary>
        [SugarColumn(ColumnName = "uCode")]
        public string uCode { get; set; }

        /// <summary>
        ///品牌ID
        ///</summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        [SugarColumn(ColumnName = "ShopID")]
        public string ShopID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        [SugarColumn(ColumnName = "ShopCode")]
        public string ShopCode { get; set; }
        /// <summary>
        /// 上传数据json 
        ///</summary>
        [SugarColumn(ColumnName = "PostJaon")]
        public string PostJaon { get; set; }
        /// <summary>
        /// 返回数据json 
        ///</summary>
        [SugarColumn(ColumnName = "ReturnJson")]
        public string ReturnJson { get; set; }
        /// <summary>
        /// 是否成功 
        ///</summary>
        [SugarColumn(ColumnName = "IsSuccess")]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 上传时间 
        ///</summary>
        [SugarColumn(ColumnName = "CreateDtm")]
        public DateTime CreateDtm { get; set; }
        /// <summary>
        /// 最后上传时间 
        ///</summary>
        [SugarColumn(ColumnName = "ModifyDtm")]
        public DateTime ModifyDtm { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        [SugarColumn(ColumnName = "BillNo2")]
        public string BillNo2 { get; set; }


        /// <summary>
        /// 来源单 
        ///</summary>
        [SugarColumn(ColumnName = "SourceBillNo")]
        public string SourceBillNo { get; set; }

 

    }
}

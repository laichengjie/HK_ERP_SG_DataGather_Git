using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Ad_PosSale_Wif")]
    public class GatAdPossale_Wif 
    {

        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        [SugarColumn(ColumnName = "SolutionID")]
        public int? SolutionID { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [SugarColumn(ColumnName = "CompanyID")]
        public string CompanyID { get; set; }

        /// <summary>
        /// 订单明细编号
        /// </summary>
        [SugarColumn(ColumnName = "retailitem_id")]
        public string retailitem_id { get; set; }


        /// <summary>
        /// 订单编号
        /// </summary>
        [SugarColumn(ColumnName = "retail_id")]
        public string retail_id { get; set; }


        /// <summary>
        /// 产品编号
        /// </summary>
        [SugarColumn(ColumnName = "product_code")]
        public string product_code { get; set; }

        /// <summary>
        /// 尺码代码
        /// </summary>
        [SugarColumn(ColumnName = "size_code")]
        public string size_code { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn(ColumnName = "bar_code")]
        public string bar_code { get; set; }

        /// <summary>
        /// 店铺编号
        /// </summary>
        [SugarColumn(ColumnName = "store_code")]
        public string store_code { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [SugarColumn(ColumnName = "paytime")]
        public DateTime paytime { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        [SugarColumn(ColumnName = "billtime")]
        public DateTime billtime { get; set; }

        /// <summary>
        /// 销售日期
        /// </summary>
        [SugarColumn(ColumnName = "billdate")]
        public DateTime billdate { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [SugarColumn(ColumnName = "qty")]
        public int qty { get; set; }

        /// <summary>
        /// 订单商品去除赠品总数量
        /// </summary>
        [SugarColumn(ColumnName = "upt_quantity")]
        public int upt_quantity { get; set; }


        /// <summary>
        /// 零售单价
        /// </summary>
        [SugarColumn(ColumnName = "pricelist")]
        public decimal? pricelist { get; set; }

        /// <summary>
        /// 成交单价
        /// </summary>
        [SugarColumn(ColumnName = "priceactual")]
        public decimal? priceactual { get; set; }


        /// <summary>
        /// 零售总金额
        /// </summary>
        [SugarColumn(ColumnName = "tot_amt_list")]
        public decimal? tot_amt_list { get; set; }


        /// <summary>
        /// 成交总金额
        /// </summary>
        [SugarColumn(ColumnName = "tot_amt_actual")]
        public decimal? tot_amt_actual { get; set; }

        /// <summary>
        /// 营业员编号
        /// </summary>
        [SugarColumn(ColumnName = "staff_code")]
        public string staff_code { get; set; }

        /// <summary>
        /// 会员编号
        /// </summary>
        [SugarColumn(ColumnName = "vip_id")]
        public string vip_id { get; set; }

        /// <summary>
        /// 订单类型 SALE(销售） , RETURN（退货 ),EXCHANGE(换货）, OTHER（其他）
        /// </summary>
        [SugarColumn(ColumnName = "retail_type")]
        public string retail_type { get; set; }

        /// <summary>
        /// 销售类型 Y O2O/团购/线上/线下adidas FAYA仅传团购标签即可
        /// </summary>
        [SugarColumn(ColumnName = "source_type")]
        public string source_type { get; set; }

        /// <summary>
        ///插入时间
        /// </summary>
        [SugarColumn(ColumnName = "insert_time")]
        public DateTime insert_time { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        [SugarColumn(ColumnName = "CardID")]
        public string CardID { get; set; }


        /// <summary>
        /// 提报时间点
        /// </summary>
        [SugarColumn(ColumnName = "DrewTime")]
        public string DrewTime { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [SugarColumn(ColumnName = "ShopID")]
        public string ShopID { get; set; }


        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(ColumnName = "BillNo")]
        public string BillNo { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        [SugarColumn(ColumnName = "ModifyTime")]
        public DateTime ModifyTime { get; set; }

    }
}

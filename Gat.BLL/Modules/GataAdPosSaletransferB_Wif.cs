using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GataAdPosSaletransferB_Wif : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性

        public int TransID { get; set; }

        /// <summary>
        /// 方案ID 
        /// </summary>
        public int? SolutionID { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 订单明细编号
        /// </summary>
        public string retailitem_id { get; set; }


        /// <summary>
        /// 订单编号
        /// </summary>
        public string retail_id { get; set; }


        /// <summary>
        /// 产品编号
        /// </summary>
        public string product_code { get; set; }

        /// <summary>
        /// 尺码代码
        /// </summary>
        public string size_code { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string bar_code { get; set; }

        /// <summary>
        /// 店铺编号
        /// </summary>
        public string store_code { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime paytime { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime billtime { get; set; }

        /// <summary>
        /// 销售日期
        /// </summary>
        public DateTime billdate { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int qty { get; set; }

        /// <summary>
        /// 订单商品去除赠品总数量
        /// </summary>
        public int upt_quantity { get; set; }


        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal? pricelist { get; set; }

        /// <summary>
        /// 成交单价
        /// </summary>
        public decimal? priceactual { get; set; }


        /// <summary>
        /// 零售总金额
        /// </summary>
        public decimal? tot_amt_list { get; set; }


        /// <summary>
        /// 成交总金额
        /// </summary>
        public decimal? tot_amt_actual { get; set; }

        /// <summary>
        /// 营业员编号
        /// </summary>
        public string staff_code { get; set; }

        /// <summary>
        /// 会员编号
        /// </summary>
        public string vip_id { get; set; }

        /// <summary>
        /// 订单类型 SALE(销售） , RETURN（退货 ),EXCHANGE(换货）, OTHER（其他）
        /// </summary>
        public string retail_type { get; set; }

        /// <summary>
        /// 销售类型 Y O2O/团购/线上/线下adidas FAYA仅传团购标签即可
        /// </summary>
        public string source_type { get; set; }

        /// <summary>
        ///插入时间
        /// </summary>
        public DateTime insert_time { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string CardID { get; set; }


        /// <summary>
        /// 提报时间点
        /// </summary>
        public string DrewTime { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public string ShopID { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GataAdPosSaletransferB_Wif() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类 
        /// </summary>
        public GataAdPosSaletransferB_Wif(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GataAdPosSaletransferB_Wif(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}

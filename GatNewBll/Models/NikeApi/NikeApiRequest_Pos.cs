using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// 销售数据 records 明细（data_type = pos）
    /// </summary>
    public class NikeApiRequest_Pos
    {
        /// <summary>门店编码。订单发生地点的客户门店主数据编码</summary>
        public string store_cd { get; set; }

        /// <summary>业务类型。订单行级别业务分类。枚举：线下、临时特卖、团购、工服、赠品、线上、私域运营</summary>
        public string business_type { get; set; }

        /// <summary>交易时间。线下为收银台完成交易时间戳；线上为发货时间。格式 YYYY-MM-DD HH:mm:ss</summary>
        public string transaction_dt { get; set; }

        /// <summary>订单类型（选填）。枚举：销售、退货、换货</summary>
        public string order_type { get; set; }

        /// <summary>交易/订单号。交易唯一键</summary>
        public string order_nbr { get; set; }

        /// <summary>条形码，商品唯一识别码</summary>
        public string upc { get; set; }

        /// <summary>货号。商品编码包含款号和颜色号</summary>
        public string product_cd { get; set; }

        /// <summary>尺码。商品尺码代码</summary>
        public string size_cd { get; set; }

        /// <summary>商品名称（选填）</summary>
        public string product_nm { get; set; }

        /// <summary>行项目序号。商品的行项目序号</summary>
        public string line_item_nbr { get; set; }

        /// <summary>实际销售单价（含税）</summary>
        public decimal? net_sales_prc_incl_tax { get; set; }

        /// <summary>商场补贴金额（选填）。示例可能为空字符串 ""</summary>
        public string rebate_amt { get; set; }

        /// <summary>实际销售金额（含税）。客户实收金额 =（实际销售单价含税 × 数量）；退货为负数</summary>
        public decimal? net_sales_amt_incl_tax { get; set; }

        /// <summary>销售数量。消费者实际购买商品数量；退货为负数</summary>
        public int? sales_qty { get; set; }

        /// <summary>建议零售单价（吊牌价）（选填）</summary>
        public decimal? msrp_lc { get; set; }

        /// <summary>吊牌总金额（选填）</summary>
        public decimal? total_partner_msrp_amt { get; set; }

        /// <summary>折扣总金额（选填）。本币计的价格调整/折扣总金额</summary>
        public decimal? total_discount_amt { get; set; }

        /// <summary>退换货原始门店编码（选填）</summary>
        public string original_store_cd { get; set; }

        /// <summary>退换货原始交易/订单号（选填）</summary>
        public string original_order_nbr { get; set; }

        /// <summary>退换货原始交易日期（选填）。格式 YYYY-MM-DD HH:mm:ss（示例为 YYYY-MM-DD）</summary>
        public string original_transaction_dt { get; set; }

        /// <summary>退换货原始行项目序号（选填）</summary>
        public int original_line_item_nbr { get; set; }

        /// <summary>记录最后更新时间戳。格式 YYYY-MM-DD HH:mm:ss</summary>
        public string last_updated_tmst { get; set; }
    }

}
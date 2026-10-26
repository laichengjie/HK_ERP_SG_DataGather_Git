using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// 库存数据 records 明细（data_type = inventory）
    /// </summary>
    public class NikeApiRequest_Inv
    {
        /// <summary>门店/仓库编码。客户门店或仓库唯一标识编码</summary>
        public string site_cd { get; set; }

        /// <summary>条形码，商品唯一识别码</summary>
        public string upc { get; set; }

        /// <summary>货号。产品编码包含款号和颜色号</summary>
        public string product_cd { get; set; }

        /// <summary>尺码。商品尺码代码</summary>
        public string size_cd { get; set; }

        /// <summary>商品名称（选填），建议填</summary>
        public string product_nm { get; set; }

        /// <summary>库存日期。库存快照日期，格式 YYYY-MM-DD</summary>
        public string inventory_dt { get; set; }

        /// <summary>总库存</summary>
        public int total_qty { get; set; }

        /// <summary>在库库存</summary>
        public int on_hand_qty { get; set; }

        /// <summary>在途库存。仓库/门店未入库的在途库存数量</summary>
        public int in_transit_qty { get; set; }

        /// <summary>残次品库存</summary>
        public int defective_pcs_qty { get; set; }

        /// <summary>建议零售单价（吊牌价）（选填）</summary>
        public decimal msrp_lc { get; set; }

        /// <summary>记录最后更新时间戳。格式 YYYY-MM-DD HH:mm:ss</summary>
        public string last_updated_tmst { get; set; }
    }

}
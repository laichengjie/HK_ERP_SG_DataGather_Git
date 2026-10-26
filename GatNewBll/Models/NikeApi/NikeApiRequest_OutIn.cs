using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// 库存移动数据 records 明细（data_type = movement）
    /// </summary>
    public class NikeApiRequest_OutIn
    {
        /// <summary>门店/仓库编码。客户门店或仓库唯一标识编码</summary>
        public string site_cd { get; set; }

        /// <summary>门店/仓库名称。位置名称</summary>
        public string site_nm { get; set; }

        /// <summary>条形码，商品唯一识别码</summary>
        public string upc { get; set; }

        /// <summary>货号。商品编码包含款号和颜色号</summary>
        public string product_cd { get; set; }

        /// <summary>尺码。商品尺码代码</summary>
        public string size_cd { get; set; }

        /// <summary>商品名称（选填）。商品描述/名称</summary>
        public string product_nm { get; set; }

        /// <summary>流转方向。交易方向：出库或入库</summary>
        public string direction_desc { get; set; }

        /// <summary>仓库作业流水单号。仅应用于仓库</summary>
        public string wms_order_no { get; set; }

        /// <summary>发货单号。耐克发货为装箱单号 EPL，耐克退货为耐克退货订单号 RSO</summary>
        public string proof_of_delivery_no { get; set; }

        /// <summary>状态类型描述。枚举：耐克发货、退还耐克、加盟发货、加盟退货</summary>
        public string status_type_desc { get; set; }

        /// <summary>流水作业时间戳。格式 YYYY-MM-DD HH:mm:ss</summary>
        public string transaction_tmst { get; set; }

        /// <summary>流水处理件数</summary>
        public int transaction_qty { get; set; }

        /// <summary>源位置编码（选填）。入库产品源头仓库位置唯一标识（示例可能为 ""）</summary>
        public string source_from_site_cd { get; set; }

        /// <summary>源位置名称（选填）。入库产品源头仓库位置名称（示例可能为 ""）</summary>
        public string source_from_site_nm { get; set; }

        /// <summary>目的位置编码（选填）。出库产品目的仓库位置唯一标识</summary>
        public string destination_to_site_cd { get; set; }

        /// <summary>目的位置名称（选填）。出库产品目的仓库位置名称</summary>
        public string destination_to_site_nm { get; set; }

        /// <summary>记录最后更新时间戳。格式 YYYY-MM-DD HH:mm:ss</summary>
        public string last_updated_tmst { get; set; }
    }

}
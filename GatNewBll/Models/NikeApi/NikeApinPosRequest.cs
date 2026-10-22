using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.NikeApi
{



    public class NikeApiBillDate
    {
        /// <summary>
        /// 线上订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string w_docno { get; set; }

        /// <summary>
        /// 外部配货单号
        /// </summary>
       // public string w_ph_docno { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public string billdate { get; set; }

        /// <summary>
        /// 销售店仓
        /// </summary>
        public string storecode { get; set; }

        /// <summary>
        /// 发货店仓
        /// </summary>
       // public string origcode { get; set; }

        /// <summary>
        /// 收货店仓
        /// </summary>
      //  public string destcode { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
      // public string vipcode { get; set; }

        /// <summary>
        /// 会员手机号
        /// </summary>
      //  public string vipmobil { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
      //  public string consignee { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
     //   public string c_expresscompany { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        //public string zems { get; set; }

        /// <summary>
        /// 快递费用/元
        /// </summary>
      //  public decimal? deliverycosts { get; set; }

        /// <summary>
        /// 重量/体积
        /// </summary>
       // public string weightarea { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
       // public string sellerremarks { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
       // public string buyerremarks { get; set; }

        /// <summary>
        /// 单据备注
        /// </summary>
       // public string description { get; set; }

        /// <summary>
        /// 接口编号
        /// </summary>
        public string mesg_id { get; set; }

        /// <summary>
        /// 当前接口数据唯一ID
        /// </summary>
        public string mesg_guid { get; set; }

        /// <summary>
        /// 发送系统
        /// </summary>
        //public string source_sys { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
       //public string senddate { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
       // public string etr_date { get; set; }

        public List<NikeApiItemDetail> detail { get; set; }
        public List<NikeApiPayDetail> detail1 { get; set; }
        public List<NikeApiRateitem> detail2 { get; set; }


    }
    public class NikeApiItemDetail
    {
        /// <summary>
        /// 线上订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string w_docno { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string sku { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int qty { get; set; }

        /// <summary>
        /// 成交单价
        /// </summary>
        public decimal priceactual { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        public decimal tot_amt_actual { get; set; }

        /// <summary>
        /// 零售类型
        /// </summary>
        public string type { get; set; }
    }

    /// <summary>
    /// 收款明细
    /// </summary>

    public class NikeApiPayDetail
    {
        /// <summary>
        /// 线上订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string w_docno { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string c_payway { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? payamount { get; set; }

        /// <summary>
        /// 券号
        /// </summary>
        public string ticketno { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>

    public class NikeApiRateitem
    {
        /// <summary>
        /// 线上订单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 外部单号
        /// </summary>
        public string w_docno { get; set; }

        /// <summary>
        /// 营业员
        /// </summary>
        public string salesrep { get; set; }

        /// <summary>
        /// 销售比例
        /// </summary>
        public string rate { get; set; }
    }

}
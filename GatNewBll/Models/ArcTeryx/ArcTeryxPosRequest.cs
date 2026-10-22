using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GatNewBll.Models.On.OnPosRequest;

namespace GatNewBll.Models.ArcTeryx
{

    public class ArcTeryxPosRequest
    {
        public ArcTeryxPosDate REQUEST { get; set; }
    }
      
    public class ArcTeryxPosDate
    {
        
        public ArcTeryxBillDate REQUEST_DATA { get; set; }

        public HrtAttrs HRT_ATTRS { get; set; }
    }
   
    /// <summary> 
    /// Pos单
    /// </summary>
    public class ArcTeryxBillDate
    {
        /// <summary> 
        /// 收银员编码
        ///</summary> 
        public string cashierId { get; set; }

        /// <summary>
        /// 店铺验证密钥(密码) 由IMPOS提供
        ///</summary> 
        public string checkCode { get; set; }

        /// <summary>
        /// 备注
        ///</summary> 
        public string comments { get; set; }

        /// <summary>
        /// 商场编码
        ///</summary> 
        public string mall { get; set; }

        /// <summary>
        /// POS单号
        ///</summary> 
        public string orderId { get; set; }

        /// <summary>
        /// 店铺编号
        ///</summary> 
        public string store { get; set; }

        /// <summary>
        /// 收银机编号
        ///</summary> 
        public string tillId { get; set; }

        /// <summary>
        /// 订单时间
        ///</summary> 
        public string time { get; set; }

        /// <summary>
        /// 订单总金额（POS单总结算金额）
        ///</summary> 
        public decimal totalAmt { get; set; }


        /// <summary>
        /// 订单类型  销售：SALE  退货：ONLINEREFUND
        ///</summary> 
        public string type { get; set; }



        /// <summary>
        /// 明细
        /// </summary>
        public List<ArcTeryxItemDetail> itemList { get; set; }

        /// <summary>
        /// 收款
        /// </summary>
        public List<ArcTeryxPayDetail> payList { get; set; }

    }


    /// <summary>
    /// 货号明细
    /// </summary>
    public class ArcTeryxItemDetail
    {
        /// <summary>
        /// 商品编号(ERP货号)
        ///</summary> 
        public string itemCode { get; set; }

        /// <summary>
        /// 销售数量
        ///</summary>
        public decimal quantity { get; set; }

        // <summary>
        /// 商品价格（结算价）
        ///</summary>
        public decimal price { get; set; }

    }


    /// <summary>
    /// 收款明细
    /// </summary>

    public class ArcTeryxPayDetail
    {

        /// <summary>
        /// 支付银行
        ///</summary> 
        public string cardBank { get; set; }

        /// <summary>
        /// 银行卡卡号
        ///</summary> 
        public string cardNumber { get; set; }

        /// <summary>
        /// 优惠金额
        ///</summary>
        public decimal discountAmt { get; set; }

        /// <summary>
        /// 应收金额
        ///</summary>
        public decimal payAmt { get; set; }

        /// <summary>
        /// 支付方式
        ///</summary>
        public string paymentMethod { get; set; }

        /// <summary>
        ///支付时间  销售日期时间 格式：	yyyyMMddhhmmss	
        ///</summary>
        public string time { get; set; }

        // <summary>
        /// 支付金额（实收金额）
        ///</summary>
        public decimal value { get; set; }


    }

     
    public class HrtAttrs
    {

        /// <summary>
        /// 合作伙伴身份标识
        ///</summary> 
        public string Partner_ID { get; set; }

        /// <summary>
        /// 调用的API版本号
        ///</summary> 
        public string Api_Version { get; set; }

        /// <summary>
        /// API调用方应用的编码，同App_ID
        ///</summary> 
        public string App_Sub_ID { get; set; }

        /// <summary>
        ///  响应格式, 默认为json格式，可选值：xml或json
        ///</summary> 
        public string Format { get; set; }


        /// <summary>
        ///  API调用的API编码
        ///</summary> 
        public string Api_ID { get; set; }


        /// <summary>
        ///  API调用方授权令牌
        ///</summary> 
        public string App_Token { get; set; }


        /// <summary>
        ///  被调用API的应用编码
        ///</summary> 
        public string App_Pub_ID { get; set; }

        /// <summary>
        ///  
        ///</summary> 
        public string Time_Stamp { get; set; }

        /// <summary>
        ///  
        ///</summary> 
        public string Sys_ID { get; set; }

        

        /// <summary>
        ///  
        ///</summary> 
        public string Sign_Method { get; set; }

        /// <summary>
        ///   服务请求的签名字符串(采用32位大写)
        ///</summary> 
        public string Sign { get; set; }

    }

}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi 
{
    public class TuMiPosMasterData
    {
       

        /// <summary>
        /// 提报方案Id 
        ///</summary>
        public int SolutionId { get; set; }


        /// <summary>
        /// 公司 
        ///</summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 单号 
        ///</summary>
        public string aicode { get; set; }

        /// <summary>
        /// 交易日期 
        ///</summary>
        public DateTime rq { get; set; }

        /// <summary>
        /// 发货时间 
        ///</summary>
        public DateTime deliveryTime { get; set; }

        /// <summary>
        /// 会员编号 
        ///</summary>
        public string memberCode { get; set; }

        /// <summary>
        /// 销售类型，0正常, 20退货	0正常, 20退货
        ///</summary>
        public string saletype { get; set; }


        /// <summary>
        /// 销售门店, 品牌提供的门店编号 ERP店铺资料的品牌门店编码
        ///</summary>
        public string store { get; set; }

        /// <summary>
        /// 原始订单,对应主订单的aicode 来源单号
        ///</summary>
        public string returnAicode { get; set; }

        /// <summary>
        /// 固定值: MA MA
        ///</summary>
        public string channel { get; set; }


        /// <summary>
        /// 顾客的年龄组（传code 1.2.3...）,默认值:0 
        ///</summary>
        public string nianLingZu { get; set; }

        /// <summary>
        /// 顾客的购物目的,默认值:0 	
        ///</summary>
        public string gouWuMuDi { get; set; }

        /// <summary>
        /// 优惠券id	
        ///</summary>
        public string couponid { get; set; }

        /// <summary>
        ///营业员ID 固定值"0"
        ///</summary>
        public string uid { get; set; }

        /// <summary>
        ///营业员姓名 固定值“”空字符串
        ///</summary>
        public string uName { get; set; }

        /// <summary>
        ///品牌营业员工号 固定值"0"
        ///</summary>
        public string uCode { get; set; }

        /// <summary>
        ///品牌ID
        ///</summary>
        public string CardID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        public string ShopID  { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        public string ShopCode { get; set; }

        
        /// <summary>
        /// 上传数据json 
        ///</summary>
        public string PostJaon { get; set; }
        /// <summary>
        /// 返回数据json 
        ///</summary>
        public string ReturnJson { get; set; }
        /// <summary>
        /// 是否成功 
        ///</summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 上传时间 
        ///</summary>
        public DateTime CreateDtm { get; set; }
        /// <summary>
        /// 最后上传时间 
        ///</summary>
        public DateTime ModifyDtm { get; set; }

        /// <summary>
        /// 退货来源单号
        ///</summary>
        public string SourceBillNo { get; set; }


        /// <summary>
        /// 单号 
        ///</summary>
        public string BillNo2 { get; set; }
    }
}

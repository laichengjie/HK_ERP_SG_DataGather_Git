using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.ArcTeryx
{
    public class ArcTeryxPosMasterData
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
        /// 收银员编码
        ///</summary>
        public string CashierId { get; set; }


        /// <summary>
        /// 店铺验证密钥(密码) 由IMPOS提供
        ///</summary>
        public string CheckCode { get; set; }

        /// <summary>
        /// 备注
        ///</summary>
        public string Comments { get; set; }

        /// <summary>
        /// 收银机编号
        ///</summary>
        public string TillId { get; set; }

        /// <summary>
        /// 订单时间 
        ///</summary>
        public DateTime SaleDate { get; set; }



        /// <summary>
        ///品牌店铺编号
        ///</summary>
        public string StoreCode { get; set; }

        /// <summary>
        ///销售类型
        ///</summary>
        public string Saletype { get; set; }

        /// <summary>
        ///品牌ID
        ///</summary>

        public string CardID { get; set; }

        /// <summary>
        ///店铺ID
        ///</summary>
        public string ShopID { get; set; }

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
        /// 是否导入 
        ///</summary>
        public bool IsImport { get; set; }


        /// <summary>
        /// 数量
        ///</summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 金额 
        ///</summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 商场编码 
        ///</summary>
        public string Mall { get; set; }

        /// <summary>
        /// 推送单号 
        ///</summary>
        public string PushBillNo { get; set; }

        

    }

}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class OnPosMasterData
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
        /// 销售日期 
        ///</summary>
        public DateTime sale_date { get; set; }

        /// <summary>
        /// 店铺编号（品牌方）
        ///</summary>
        public string store_code { get; set; }


        /// <summary>
        /// 单据类型
        ///</summary>
        public string order_type { get; set; }


        /// <summary> 
        /// 单号
        ///</summary>
        public string order_number { get; set; }

        /// <summary> 
        /// 原单号
        ///</summary>
        public string original_order_number { get; set; }

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

    }

}

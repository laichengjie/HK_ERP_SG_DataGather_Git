using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class OnInvMasterData
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
        /// 品牌店铺编号
        ///</summary>
        public string store_code { get; set; }

        /// <summary>
        /// 库存日期 
        ///</summary>
        public DateTime inventory_date { get; set; }

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
        ///提报时间点
        ///</summary>
        public string DrewTime { get; set; }
    }
}

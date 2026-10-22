using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.Burgeon
{
    public class BurgeonPosMasterData
    {
        /// <summary>
        /// ID（自增主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 提报方案Id
        /// </summary>
        public int SolutionId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime BillDate { get; set; }

        /// <summary>
        /// 线上订单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternalBillNo { get; set; }

        /// <summary>
        /// 营业员
        /// </summary>
        public string Salesrep { get; set; }

        /// <summary>
        /// 销售比例
        /// </summary>
        public string Rate { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public string ShopID { get; set; }

        /// <summary>
        /// 店铺代码
        /// </summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 发货店仓
        /// </summary>
        public string OrigCode { get; set; }

        /// <summary>
        /// 收货店仓
        /// </summary>
        public string DestCode { get; set; }

        /// <summary>
        /// 上传数据json
        /// </summary>
        public string PostJaon { get; set; }

        /// <summary>
        /// 返回数据json
        /// </summary>
        public string ReturnJson { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? CreateDtm { get; set; }

        /// <summary>
        /// 最后上传时间
        /// </summary>
        public DateTime? ModifyDtm { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 订单总金额（POS单总结算金额）
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 是否导入
        /// </summary>
        public bool IsImport { get; set; }

        /// <summary>
        /// 推送单号
        /// </summary>
        public string PushBillNo { get; set; }
    }

}

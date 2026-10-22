using System;

namespace GatNewBll.Models.NikeApi
{
    /// <summary>
    /// Gat_Burgeon_PosSale_Detail 实体
    /// </summary>
    public class NikeApiPosDetailData
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
        /// 线上订单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternalBillNo { get; set; }

        /// <summary>
        /// 货号ID
        /// </summary>
        public string MaterialID { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 成交单价
        /// </summary>
        public decimal BalaPrice { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 1销售 2退货 3赠品
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// 是否导入
        /// </summary>
        public bool? IsImport { get; set; }
    }
}
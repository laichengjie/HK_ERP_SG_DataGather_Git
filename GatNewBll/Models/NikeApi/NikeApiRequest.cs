using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatNewBll.Models.NikeApi
{

    /// <summary>
    /// Partner → Nike 数据推送批次头（公共技术字段）
    /// </summary>
    public class NikeApiRequest<T>
    {
        /// <summary>批次号（UUID v4）。同一批次包含一个 data_type 当天传输的完整数据；分批时多个 request 共享同一 batch_no</summary>
        public string batch_no { get; set; }

        /// <summary>批次日期，Current Date - 1 Day，YYYY-MM-DD</summary>
        public string batch_date { get; set; }

        /// <summary>请求序号</summary>
        public int request_seq { get; set; }

        /// <summary>请求 ID，与 Header 的 X-Request-ID 一致，UUID v4</summary>
        public string request_id { get; set; }

        /// <summary>本批次预计的总请求数。接收方以每次请求携带的 total_requests 为准判断批次是否接收完成</summary>
        public int total_requests { get; set; }

        /// <summary>合作方唯一标识码（与 Header 的 X-Partner-Code 必须一致）</summary>
        public string partner_code { get; set; }

        /// <summary>数据类型。允许值：pos / inventory / movement / store / warehouse</summary>
        public string data_type { get; set; }

        /// <summary>records 数组行数，用于校验</summary>
        public int record_count { get; set; }

        /// <summary>本次请求 records 中金额字段的求和。pos 用 NET_SALES_AMT_INCL_TAX；其余类型传 0</summary>
        public decimal amount_sum { get; set; }

        /// <summary>本次请求 records 中数量字段的求和。pos 用 SALES_QTY；inventory 用 TOTAL_QTY；movement 用 transaction_qty；store/warehouse 传 0</summary>
        public int quantity_sum { get; set; }

        /// <summary>业务数据结构版本，如 1.0、2.1</summary>
        public string schema_version { get; set; }

        /// <summary>数据来源系统名称，自营/加盟来自不同数据库，通过"自营"、"加盟"区分</summary>
        public string source_system { get; set; }

        /// <summary>数据发送时间，ISO 8601 格式含时区（示例另有 yyyy-MM-dd HH:mm:ss，故用 string）</summary>
        public string send_timestamp { get; set; }

        /// <summary>业务数据数组，内容结构由 data_type 决定（每次请求最多 10000 条，总大小不超过 7.6MB）</summary>
        public List<T> records { get; set; }
    }
}

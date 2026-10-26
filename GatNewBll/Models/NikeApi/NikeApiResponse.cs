using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.NikeApi
{


    /// <summary>
    /// Nike 数据推送接口 响应体（成功/拒绝通用）
    /// 对应 POST /ades/partnerData/send 的返回
    /// </summary>
    public class NikeApiResponse
    {
        /// <summary>Nike 侧唯一响应标识，UUID v4</summary>
        public string response_id { get; set; }

        /// <summary>回传请求的 batch_no</summary>
        public string batch_no { get; set; }

        /// <summary>回传请求的 batch_date</summary>
        public string batch_date { get; set; }

        /// <summary>回传请求的 request_seq</summary>
        public int request_seq { get; set; }

        /// <summary>回传请求的 request_id，用于关联溯源</summary>
        public string request_id { get; set; }

        /// <summary>
        /// 200	请求受理成功（含部分拒绝场景）
        /// </summary>
        public string code { get; set; }

        /// <summary>状态描述信息，人类可读</summary>
        public string message { get; set; }

        /// <summary>响应生成时间，ISO 8601 格式含时区</summary>
        public string timestamp { get; set; }

        /// <summary>回传请求的 partner_code</summary>
        public string partner_code { get; set; }

        /// <summary>回传请求的 data_type</summary>
        public string data_type { get; set; }

        /// <summary>接口级失败时填的错误列表（成功时通常为 null 或不返回）</summary>
        public List<NikeApiResponseError> errors { get; set; }

        /// <summary>Nike 内部链路追踪 ID，用于问题排查</summary>
        public string trace_id { get; set; }
    }

    /// <summary>
    /// 响应 errors 数组子项（接口级/字段级错误）
    /// </summary>
    public class NikeApiResponseError
    {
        /// <summary>业务状态码。见错误码规范：0000 成功；1001/1002/2001/2002/2003/2004/3001/3002/4001/4002/5001 失败</summary>
      
        public string code { get; set; }

        /// <summary>错误描述</summary>
        public string message { get; set; }

        /// <summary>涉及字段（字段级错误时填充）</summary>
        public string field { get; set; }
    }


}

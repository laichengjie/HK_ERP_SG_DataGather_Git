using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.NikeApi
{
    public class NikeApiPosResponse
    { 

        public List<NikeApiPosResponseData> Data { get; set; }

    } 
    
    public class NikeApiPosResponseData
    { 
        /// <summary>
        /// 状态
        /// S = 成功 (Success)
        /// E = 失败 (Error)
        /// </summary>
        public string STATUS { get; set; }

        /// <summary>
        /// 当前接口数据唯一ID
        /// </summary>
        public string MESG_GUID { get; set; }

        /// <summary>
        /// 状态
        /// S = 成功 (Success)
        /// E = 失败 (Error)
        /// </summary>
        public string DESCRIPTION { get; set; }

        

    }



}

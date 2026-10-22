using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.ArcTeryx
{
    public class ArcTeryxPosResponse
    {

      
        public string RETURN_CODE { get; set; }
        public string OPENAPI_TC { get; set; }
        public string RETURN_DESC { get; set; }
        public string RETURN_STAMP { get; set; }
        public ArcTeryxPosReturnData RETURN_DATA { get; set; }

    } 
    
    public class ArcTeryxPosReturnData
    {
        public HeaderResponse header { get; set; }

        public BodyResponse body { get; set; }

    }
    public class HeaderResponse
    {
        /// <summary> 
        /// 0000：成功
        /// </summary>
        public string errcode { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string errmsg { get; set; }

      

    }


    public class BodyResponse
    {
        /// <summary> 
        /// 
        /// </summary>
        public string orderId { get; set; }
         
        /// <summary>
        /// 信息
        /// </summary>
        public string refOrderId { get; set; }

    }



}

using Gat.Utils;
using Gat.Web.Controller.Dtos;
using Gat.Web.Controller.Dtos.NikeApi;
using GatNewBll.Blls;
using GatNewBll.Models;
using GatNewBll.Models.NikeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    /// <summary>
    /// Nike
    /// </summary>
    public class NikeApiController : ApiController  
    {
        NikeApiBill bll = new NikeApiBill();
        [System.Web.Http.HttpPost] 
        public QueryOutputDto<GatNikeApiLogModel> GatNikeApiLoList(QueryInputDto input)
        {
            var result = bll.GatNikeApiLoList(input,true);
            return result;
        } 


        [System.Web.Http.HttpPost]
        public QueryOutputDto<GatNikeApiPosSaleModel> GatNikeApiPosSaleList(QueryInputDto input)
        {
            var result = bll.GatNikeApiPosSaleList(input, true);
            return result;
        }

        [System.Web.Http.HttpPost]
        public QueryOutputDto<GatNikeApiInvModel> GatNikeApiInvList(QueryInputDto input)
        {
            var result = bll.GatNikeApiInvList(input, true);
            return result;
        }
        [System.Web.Http.HttpPost]
        public QueryOutputDto<GatNikeApiOutInModel> GatNikeApiOutInList(QueryInputDto input)
        {
            var result = bll.GatNikeApiOutInList(input, true);
            return result;
        }
        public CommonResponseDto<object> Repost(RepostInputDto input)
        {
            CommonResponseDto<object> result = new CommonResponseDto<object>();
            if (input.SolutionId == 0)
            {
                result.Success = false;
                result.Message = "solutionId参数不能为空";
                return result;
            }
            if (input.Ids == null || input.Ids.Count() == 0)
            {
                result.Success = false;
                result.Message = "重传记录Id不能为空";
                return result;
            }
            try
            {
               
                string idsStr = "";
                if (input.Ids != null)
                {
                    idsStr = string.Join(",", input.Ids);
                }
                var HANDLE = bll.PushPostDataSave(input.SolutionId,null,null, idsStr,input.InterfaceType);
                if (HANDLE.Status == HandleStatus.Completed)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = HANDLE.Message;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        
    }
}
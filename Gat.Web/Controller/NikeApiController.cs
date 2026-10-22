using Gat.Utils;
using Gat.Web.Controller.Dtos;
using Gat.Web.Controller.Dtos.NikeApi;
using GatNewBll.Blls;
using GatNewBll.Models.NikeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    /// <summary>
    /// 伯俊
    /// </summary>
    public class NikeApiController : ApiController  
    {
        NikeApiBill bll = new NikeApiBill();
        [System.Web.Http.HttpPost] 
        public NikeApiDetailDto<GatNikeApiLogModel> GatNikeApiLoList(NikeApiQueryDto input)
        {
            NikeApiDetailDto<GatNikeApiLogModel> result = new NikeApiDetailDto<GatNikeApiLogModel>();
            int totalCount = 0;
            int? totalQty = 0;
            decimal? totalAmount = 0;
            int solutionIdValue = 0;
            if (input.SolutionId.HasValue)
            {
                solutionIdValue = input.SolutionId.Value;
            }
            else
            {
                SolutionBll solBll = new SolutionBll();
                var solution = solBll.GetFirstSolutionByVendorId(input.VendorId.Value);
                solutionIdValue = solution.Id;
            }
            var pageList = bll.GatNikeApiLoList(solutionIdValue, input.Page, input.PageSize,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.CardShopCode, input.BillNo, input.Status,input.InterfaceType, ref totalCount, ref totalQty, ref totalAmount,true);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        } 


        [System.Web.Http.HttpPost]
        public NikeApiDetailDto<GatNikeApiPosSaleModel> GatNikeApiPosSaleList(NikeApiQueryDto input)
        {
            NikeApiDetailDto<GatNikeApiPosSaleModel> result = new NikeApiDetailDto<GatNikeApiPosSaleModel>();
            int totalCount = 0;
            int? totalQty = 0;
            decimal? totalAmount = 0;
            int solutionIdValue = 0;
            if (input.SolutionId.HasValue)
            {
                solutionIdValue = input.SolutionId.Value;
            }
            else
            {
                SolutionBll solBll = new SolutionBll();
                var solution = solBll.GetFirstSolutionByVendorId(input.VendorId.Value);
                solutionIdValue = solution.Id;
            }
            var pageList = bll.GatNikeApiPosSaleList(solutionIdValue, input.Page, input.PageSize,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.CardShopCode, input.BillNo, input.Status, ref totalCount, ref totalQty, ref totalAmount, true);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
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
                var HANDLE = bll.PushPostDataSave(input.SolutionId,null,null, idsStr);
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
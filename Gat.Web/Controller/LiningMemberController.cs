using Gat.Utils;
using Gat.Web.Controller.Dtos;
using Gat.Web.Controller.Dtos.LiningMember;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    public class LiningMemberController : ApiController
    {
        [System.Web.Http.HttpPost]
        public PosSalePageDto GetPageList(GetPageListQueryDto input)
        {
            PosSalePageDto result = new PosSalePageDto();
            LiningMemberBll bll = new LiningMemberBll();
            int totalCount = 0;
            decimal totalQty = 0;
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
            var pageList = bll.GetPageList(solutionIdValue, input.Page, input.PageSize,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.Status, input.TradeCode, input.RefundCode, input.IsRefund, input.memberMobile, ref totalCount, ref totalQty, ref totalAmount);
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
                LiningMemberBll bll = new LiningMemberBll();
                var HANDLE = bll.Repost(input.SolutionId, input.Ids);
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
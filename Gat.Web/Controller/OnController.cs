using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gat.Web.Controller.Dtos.On;
using System.Web.Http;
using Gat.Utils;
using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;

namespace Gat.Web.Controller
{
    /// <summary>
    /// On昂跑
    /// </summary>
    public class OnController : ApiController
    { 
        [System.Web.Http.HttpPost]
        public OnPosSalePageDto GetPageList(GetPageListQueryDto input)
        {
            OnPosSalePageDto result = new OnPosSalePageDto();
            OnBill bll = new OnBill();
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
            var pageList = bll.GetPageList(solutionIdValue, input.Page, input.PageSize,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.CardShopCode, input.BillNo, input.Status, ref totalCount, ref totalQty, ref totalAmount);
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
                OnBill bll = new OnBill();
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

        [System.Web.Http.HttpPost]
        public InvPageDto GetPageInvList(GetPageListQueryDto input)
        {
            InvPageDto result = new InvPageDto();
            OnBill bll = new OnBill();
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
            var pageList = bll.GetPageInvList(solutionIdValue, input.Page, input.PageSize,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.CardShopCode, input.Status, ref totalCount, ref totalQty, ref totalAmount);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        }
    }
}
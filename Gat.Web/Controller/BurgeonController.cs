using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gat.Web.Controller.Dtos.Burgeon;
using System.Web.Http;
using Gat.Utils;
using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;

namespace Gat.Web.Controller
{
    /// <summary>
    /// 伯俊
    /// </summary>
    public class BurgeonController : ApiController  
    {
        BurgeonBill bll = new BurgeonBill();
        [System.Web.Http.HttpPost]
        public BurgeonPosSalePageDto GetPageList(GetPageListQueryDto input)
        {
            BurgeonPosSalePageDto result = new BurgeonPosSalePageDto();
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
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.CardShopCode, input.BillNo, input.Status, ref totalCount, ref totalQty, ref totalAmount,true);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        } 


        [System.Web.Http.HttpPost]
        public PosSaleDetailDto GetPosDetailList(GetPageListQueryDto input)
        {
            PosSaleDetailDto result = new PosSaleDetailDto();
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
            var pageList = bll.GetPageDetailList(solutionIdValue, input.Page, input.PageSize,
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
                BurgeonBill bll = new BurgeonBill();
                var HANDLE = bll.NormalPosPost (input.SolutionId,null,null,input.Ids);
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
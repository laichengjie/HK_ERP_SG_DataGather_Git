using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    public class SkechersController : ApiController
    {
        [HttpGet]
        public SkechersPosSalePageDto GetPageList(int? solutionId, int vendorId, int page, int pageSize, DateTime? startBillDate = null, DateTime? endBillDate = null, string shopCode = "", string status = "")
        {
            SkechersPosSalePageDto result = new SkechersPosSalePageDto();
            SkechersBll bll = new SkechersBll();
            int totalCount = 0;
            int totalQty = 0;
            decimal totalAmount = 0;
            int solutionIdValue = 0;
            if (solutionId.HasValue)
            {
                solutionIdValue = solutionId.Value;
            }
            else
            {
                SolutionBll solBll = new SolutionBll();
                var solution = solBll.GetFirstSolutionByVendorId(vendorId);
                solutionIdValue = solution.Id;
            }
            var pageList = bll.GetPageList(solutionIdValue, page, pageSize, startBillDate, endBillDate, shopCode, status, ref totalCount, ref totalQty, ref totalAmount);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        }
    }
}
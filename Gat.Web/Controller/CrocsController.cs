using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    public class CrocsController : ApiController
    {
        [HttpGet]
        public CrocsPosSalePageDto GetPosSalePageList(int? solutionId, int vendorId, int page, int pageSize, DateTime? startBillDate = null, DateTime? endBillDate = null, string shopCode = "")
        {
            CrocsPosSalePageDto result = new CrocsPosSalePageDto();
            CrocsBll bll = new CrocsBll();
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
            var pageList = bll.GetPosSalePageList(solutionIdValue, page, pageSize, startBillDate, endBillDate, shopCode, ref totalCount, ref totalQty, ref totalAmount);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        }

        [HttpGet]
        public CrocsInvPageDto GetInventoryPageList(int? solutionId, int vendorId, int page, int pageSize, string shopCode = "", string skuCode = "")
        {
            CrocsInvPageDto result = new CrocsInvPageDto();
            CrocsBll bll = new CrocsBll();
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
            var pageList = bll.GetInvPageList(solutionIdValue, page, pageSize, shopCode, skuCode, ref totalCount, ref totalQty, ref totalAmount);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        }
    }
}
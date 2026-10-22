using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using SqlSugar;

namespace GatNewBll.Blls
{
    public class AdBll
    {
        public List<GatAdPossale> SelectPosSalePageList(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdPossaleManager manager = new GatAdPossaleManager();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeName", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount) ?? 0m;
            return result.ToList();
        }

        public Dictionary<string, object> SelectPosSaleTransferPageList(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatAdPossaleTransferManager manager = new GatAdPossaleTransferManager();
            var p = new PageModel() { PageIndex = page, PageSize = pageSize };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeName", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var list = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            var totalCount = p.TotalCount;
            var qty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            var amount = manager.AsQueryable().Where(conModels).Sum(s => s.SalesAmount);
            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }
    }
}

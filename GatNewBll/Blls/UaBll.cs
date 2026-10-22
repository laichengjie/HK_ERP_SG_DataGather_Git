using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using SqlSugar;

namespace GatNewBll.Blls
{
    public class UaBll : BaseBll
    {
        public List<GatUaPossale> SelectPage(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal SumQty, ref decimal SumAmount,string start_date="", string end_date="", string billDateStartDate="", string billDateEndDate="", string ShopCode = "", string MaterialCode = "", string SizeCode = "")
        {
            SumQty = 0;
            SumAmount = 0;
            /*
            GatUaPossaleManager manager = new GatUaPossaleManager();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(start_date))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = start_date });
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = end_date });
            }
            if (!string.IsNullOrEmpty(billDateStartDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = billDateStartDate });
            }
            if (!string.IsNullOrEmpty(billDateEndDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = billDateEndDate });
            }
            if (!string.IsNullOrEmpty(ShopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = ShopCode });
            }

            if (!string.IsNullOrEmpty(MaterialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.LikeLeft, FieldValue = MaterialCode });
            }

            if (!string.IsNullOrEmpty(SizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeName", ConditionalType = ConditionalType.Like, FieldValue = SizeCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.ShopCode);
            totalCount = p.TotalCount;

            return result.ToList();
            */

            List<GatUaPossale> result = new List<GatUaPossale>();

            GatUaPossaleManager manager = new GatUaPossaleManager();
            var query = manager.AsQueryable()
                .Where(o => o.SolutionID == solutionID);
            if (!string.IsNullOrEmpty(start_date))
            {
                query= query.Where(s => s.BillTime >= Convert.ToDateTime(start_date));
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                query = query.Where(s => s.BillTime <= Convert.ToDateTime(end_date));
            }
            if (!string.IsNullOrEmpty(billDateStartDate))
            {
                query = query.Where(s => s.BillDate >= Convert.ToDateTime(billDateStartDate));
            }
            if (!string.IsNullOrEmpty(billDateEndDate))
            {
                query = query.Where(s => s.BillDate <= Convert.ToDateTime(billDateEndDate));
            }
            if (!string.IsNullOrEmpty(ShopCode))
            {
                query = query.Where(s => s.ShopCode.Contains(ShopCode));
            }

            if (!string.IsNullOrEmpty(MaterialCode))
            {
                query = query.Where(s => s.MaterialCode.StartsWith(MaterialCode));
            }

            if (!string.IsNullOrEmpty(SizeCode))
            {
                query = query.Where(s => s.SizeName.Contains(SizeCode));
            }
            totalCount = query.Count();
            if (totalCount > 0)
            {
                SumQty = query.Sum(o => o.Qty);
                SumAmount = query.Sum(o => o.Amount);
            }
            result = query.Select((o) => new GatUaPossale()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.BillTime, o.ShopCode, o.BillCode, o.MaterialCode })
              .ToPageList(currentPage, itemsPerPage, ref totalCount);
            return result;
        }

        public List<GatUaPossale> SelectAll(int solutionID, string start_date = "", string end_date = "", string billDateStartDate = "", string billDateEndDate = "", string ShopCode = "", string MaterialCode = "", string SizeCode = "")
        {
            GatUaPossaleManager manager = new GatUaPossaleManager();
            var query= manager.AsQueryable().Where(s=>s.SolutionID==solutionID);

            if (!string.IsNullOrEmpty(start_date))
            {
                query.Where(s => s.BillTime >= Convert.ToDateTime(start_date));
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                query.Where(s => s.BillTime <= Convert.ToDateTime(end_date));
            }
            if (!string.IsNullOrEmpty(billDateStartDate))
            {
                query.Where(s => s.BillDate >= Convert.ToDateTime(billDateStartDate));
            }
            if (!string.IsNullOrEmpty(billDateEndDate))
            {
                query.Where(s => s.BillDate <= Convert.ToDateTime(billDateEndDate));
            }
            if (!string.IsNullOrEmpty(ShopCode))
            {
                query.Where(s => s.ShopCode.Contains(ShopCode));
            }

            if (!string.IsNullOrEmpty(MaterialCode))
            {
                query.Where(s => s.MaterialCode.StartsWith(MaterialCode));
            }

            if (!string.IsNullOrEmpty(SizeCode))
            {
                query.Where(s => s.SizeName.Contains(SizeCode));
            }
            List<OrderByModel> orderByM = new List<OrderByModel>();
            orderByM.Add(new OrderByModel() { FieldName = "BillTime", OrderByType = OrderByType.Asc });
            orderByM.Add(new OrderByModel() { FieldName = "BillCode", OrderByType = OrderByType.Asc });
            return query.OrderBy(orderByM).ToList();
        }
    }
}

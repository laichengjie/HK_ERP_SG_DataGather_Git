using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GatEntities.DbModels;
using GatNewBll.Models;
using GatSqlSugarDAL.Services;
using SqlSugar;

namespace GatNewBll.Blls
{
    public class LiningBll : BaseBll
    {
        public List<GatLiningPosSalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillTime, DateTime? endBillTime, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string materialCode, string sizeName, ref int totalCount, ref decimal totalQty, ref decimal? totalAmount)
        {
            List<GatLiningPosSalePageModel> result = new List<GatLiningPosSalePageModel>();

            GatLiningPossaleManager manager = new GatLiningPossaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillTime.HasValue)
            {
                data = data.Where(o => o.BillTime >= startBillTime);
            }
            if (endBillTime.HasValue)
            {
                data = data.Where(o => o.BillTime <= endBillTime);
            }
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                data = data.Where(o => o.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrEmpty(sizeName))
            {
                data = data.Where(o => o.SizeName.Contains(sizeName));
            }
            totalQty = data.Sum(o => o.Qty);
            totalAmount = data.Sum(o => o.SaleAmount);
            result = data.Select((o) => new GatLiningPosSalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.BillTime, o.ShopCode, o.BillNo, o.MaterialCode })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }

        public bool Delete(List<int> ids = null, int solutionId = 0, DateTime? startBillTime = null, DateTime? endBillTime = null, DateTime? startBillDate = null, DateTime? endBillDate = null, string shopCode = null, string materialCode = null, string sizeName = null)
        {
            GatLiningPossaleManager manager = new GatLiningPossaleManager();
            if (ids != null && ids.Count() > 0)
            {
                return manager.DeleteById(ids);
            }
            else
            {
                var data = manager.AsQueryable()
                   .Where(o => o.SolutionId == solutionId);
                if (startBillTime.HasValue)
                {
                    data = data.Where(o => o.BillTime >= startBillTime);
                }
                if (endBillTime.HasValue)
                {
                    data = data.Where(o => o.BillTime <= endBillTime);
                }
                if (startBillDate.HasValue)
                {
                    data = data.Where(o => o.BillDate >= startBillDate);
                }
                if (endBillDate.HasValue)
                {
                    data = data.Where(o => o.BillDate <= endBillDate);
                }
                if (!string.IsNullOrEmpty(shopCode))
                {
                    data = data.Where(o => o.ShopCode.Contains(shopCode));
                }
                if (!string.IsNullOrEmpty(materialCode))
                {
                    data = data.Where(o => o.MaterialCode.Contains(materialCode));
                }
                if (!string.IsNullOrEmpty(sizeName))
                {
                    data = data.Where(o => o.SizeName.Contains(sizeName));
                }
                var deleteIds = data.Select(o => o.Id).ToList();

                return manager.DeleteById(deleteIds);
            }
        }

        public List<GatLiningPossale> GetExportList(int solutionId, DateTime? startBillTime, DateTime? endBillTime, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string materialCode, string sizeName)
        {
            GatLiningPossaleManager manager = new GatLiningPossaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillTime.HasValue)
            {
                data = data.Where(o => o.BillTime >= startBillTime);
            }
            if (endBillTime.HasValue)
            {
                data = data.Where(o => o.BillTime <= endBillTime);
            }
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                data = data.Where(o => o.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrEmpty(sizeName))
            {
                data = data.Where(o => o.SizeName.Contains(sizeName));
            }
            return data.OrderBy(o => new { o.BillTime, o.ShopCode, o.BillNo, o.MaterialCode }).ToList();
        }
    }
}

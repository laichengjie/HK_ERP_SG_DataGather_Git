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
    public class CrocsBll : BaseBll
    {
        public List<GatCrocsPossale> GetPosSalePageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode, ref int totalCount, ref int totalQty, ref decimal totalAmount)
        {
            List<GatCrocsPossale> result = new List<GatCrocsPossale>();

            var data = Db.Queryable<GatCrocsPossale>()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.TransactionTime >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.TransactionTime <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.Doorcode.Contains(shopCode));
            }
            totalQty = data.Sum(o => o.Qty);
            totalAmount = data.Sum(o => o.Revenue);
            result = data.ToPageList(page, pageSize, ref totalCount);

            return result;
        }

        public List<GatCrocsInventory> GetInvPageList(int solutionId, int page, int pageSize, string shopCode, string skuCode, ref int totalCount, ref int totalQty, ref decimal totalAmount)
        {
            List<GatCrocsInventory> result = new List<GatCrocsInventory>();

            var data = Db.Queryable<GatCrocsInventory>()
                .Where(o => o.SolutionId == solutionId);
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.Doorcode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(skuCode))
            {
                data = data.Where(o => o.SKUcode.Contains(skuCode));
            }
            totalQty = data.Sum(o => o.Qty);
            totalAmount = data.Sum(o => o.Msrp);
            result = data.ToPageList(page, pageSize, ref totalCount);

            return result;
        }
    }
}

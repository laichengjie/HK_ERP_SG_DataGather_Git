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
    public class SkechersBll : BaseBll
    {
        public List<GatSkechersPossalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string status, ref int totalCount, ref int totalQty, ref decimal totalAmount)
        {
            List<GatSkechersPossalePageModel> result = new List<GatSkechersPossalePageModel>();

            var data = Db.Queryable<GatSkechersPossale>()
                .LeftJoin<BasShop>((o, shop) => o.ShopCode == shop.ShopID)
                .LeftJoin<GatSolution>((o, shop, gs) => o.SolutionId == gs.Id)
                .LeftJoin<GatShopright>((o, shop, gs, gsr) => gs.CompanyID == gsr.CompanyID && gs.CardID == gsr.CardID && gs.VendorID == gsr.VendorID && o.ShopCode == gsr.ShopID)
                .Where(o => o.SolutionId == solutionId);
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
                data = data.Where((o, shop) => shop.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }
            totalQty = data.Sum(o => o.TotalQty);
            totalAmount = data.Sum(o => o.TotalAmount);
            result = data.Select((o, shop, gs, gsr) => new GatSkechersPossalePageModel()
            {
                BillNo = o.BillNo.SelectAll(),
                ErpShopCode = shop.ShopCode,
                TradeCode = gsr.TradeCode,
                ShopName = shop.ShopName
            }).ToPageList(page, pageSize, ref totalCount);

            return result;
        }
    }
}

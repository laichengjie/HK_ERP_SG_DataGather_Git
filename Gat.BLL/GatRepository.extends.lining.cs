using Gat.BLL.ControllerModel.Response;
using Gat.BLL.Modules;
using Gat.DAL;
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Gat.BLL
{
    public partial class GatRepository : IDisposable
    {

        #region 李宁品牌
        /// <summary>
        /// 查询李宁数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="isNeedPage"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatLiNingPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode, int currentPage, int itemsPerPage, bool isNeedPage = true)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatLiNingPosSaleB> posSalelist = new List<GatLiNingPosSaleB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.BillTime.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.BillTime.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.BillDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.BillDate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(e => e.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(e => e.SizeName.Contains(sizeCode));
            }
            var list = query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode);
            var dataCount = list.Count();
            if (isNeedPage)
            {
                posSalelist = list
                       .Select(g => new GatLiNingPosSaleB(g))
                       .Skip((currentPage - 1) * itemsPerPage)
                       .Take(itemsPerPage)
                       .ToList();
            }
            else
            {
                posSalelist = list
                                .Select(g => new GatLiNingPosSaleB(g))
                                .ToList();
            }
            var total = list.ToList();
            decimal? qty = total.Select(s => s.Qty)?.Sum();
            decimal? amount = total.Select(s => s.SaleAmount)?.Sum();
            result.Add("data", posSalelist);
            result.Add("dataCount", dataCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public IQueryable<GatLiNingPosSaleB> QueryGatLiNingPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.BillTime.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.BillTime.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.BillDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.BillDate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.SizeName.Contains(sizeCode));
            }
            var list = query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatLiNingPosSaleB(g));
            return list;
        }

        public List<GatLiNingPosSaleB> QueryGatLiNingPosSalesALL()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            return query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatLiNingPosSaleB(g))
                .ToList();
        }
        /// <summary>
        /// 李宁销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatLiNingPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        public HandleArgs DeleteLiNingPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)>='{startBillTime}'";
            }
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)<='{endBillTime}'";
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>='{startBillDate}'";
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<='{endBillDate}'";
            }
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_LiNing_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteLiNingPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_LiNing_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 查询李宁所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatLiNingMeterialINVB> QueryGatLiNingMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatLiNingMeterialINVB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询李宁所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatLiNingMeterialINVB> QueryGatLiNingMatInvsQyeryable(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Select(g => new GatLiNingMeterialINVB(g));
        }


        /// <summary>
        /// 查询李宁所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatLiNingMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 李宁销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatLiNingPosSaleB> QueryGatLiNingPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatLiNingPosSaleB(g))
                   .ToList();
        }
        public decimal?[] QueryLiNingGatTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLiNingPosSale>());
            decimal? qty = query.Sum(s => s.Qty) ;
            decimal? prices = query.Sum(s => s.BalaPrice);
            decimal?[] marks = new decimal?[] { qty, prices };
            return marks;
        }
        #endregion

        
    }
}

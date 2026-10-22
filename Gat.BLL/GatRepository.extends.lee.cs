using Gat.BLL.Modules;
using Gat.DAL;
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;

namespace Gat.BLL
{
    /// <summary>
    /// 数据提报逻辑层统管类
    /// </summary>
    public partial class GatRepository : IDisposable
    {
        #region POS单

        /// <summary> 
        /// Lee销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatLeePosSaleB> QueryGatLeePosSales(int solutionID, int currentPage, int itemsPerPage, string start_date, string end_date, string shopCode, string shopName, string materialCode, string barCode, string billNo, out int Qty, out decimal Amount, out int Count)
        {
            List<GatLeePosSaleB> Poslist = new List<GatLeePosSaleB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatLeePosSale>());
            query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));

            if (!string.IsNullOrWhiteSpace(start_date))
            {
                DateTime dtToday = Convert.ToDateTime(start_date);
                query = query.Where(s => s.BillDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(end_date))
            {
                DateTime dtendToday = Convert.ToDateTime(end_date); 
                query = query.Where(e => e.BillDate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(shopName))
            {
                query = query.Where(e => e.ShopName.Contains(shopName));
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(e => e.MaterialCode == materialCode);
            }
            
            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(e => e.BarCode == barCode);
            }
            if (!string.IsNullOrWhiteSpace(billNo))
            {
                query = query.Where(e => e.BillNo == billNo);
            }
            Qty = 0;
            Amount = 0;
            Count = 0;
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (int)(query.Sum(i => i.Qty));
                Amount = (decimal)query.Sum(i => i.PayAmount);
            }
            Poslist= query
                  .OrderByDescending(p => p.BillTime)
                 .ThenBy(p => p.ShopCode)
                 .ThenBy(p => p.BillNo)
                 .ThenBy(p => p.MaterialCode)
                 .Skip((currentPage - 1) * itemsPerPage)
                 .Take(itemsPerPage)
                 .Select(g => new GatLeePosSaleB(g))
                 .ToList();

            return Poslist;


        }


        /// <summary>
        /// 获取Lee销售数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public IQueryable<GatLeePosSaleB> QueryGatLeePosSalesQueryable(int solutionID, string start_date, string end_date, string shopName)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatLeePosSale>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(start_date))
            {
                DateTime dtToday = Convert.ToDateTime(start_date);
                query = query.Where(s => s.BillDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(end_date))
            {
                DateTime dtendToday = Convert.ToDateTime(end_date);
                query = query.Where(e => e.BillDate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(shopName))
            {
                query = query.Where(e => e.ShopName == shopName);
            }
            return query
                .OrderBy(p => p.BillDate)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
            .Select(g => new GatLeePosSaleB(g));
        }



        public HandleArgs DeleteLeePosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string shopCode, string shopName, string materialCode, string barCode, string sizeCode)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)>=CONVERT(varchar(10),'{startBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)<=CONVERT(varchar(10),'{endBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>=CONVERT(varchar(10),'{startBillDate}',121)";
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<=CONVERT(varchar(10),'{endBillDate}',121)";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                where += $" AND shopCode like'%{shopCode}%'";
            }
            if (!string.IsNullOrEmpty(shopName))
            {
                where += $" AND ShopName='{shopName}'";
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                where += $" AND materialCode='{materialCode}'";
            }

            if (!string.IsNullOrEmpty(barCode))
            {
                where += $" AND barCode='{barCode}'";
            }

            if (!string.IsNullOrEmpty(sizeCode))
            {
                where += $" AND SizeName='{sizeCode}'";
            }
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Lee_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }


        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteLeePosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Lee_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        #endregion

        #region 库存
        /// <summary>
        ///  查询Lee所有的提报库存 
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="isNeedPage"></param>
        /// <returns></returns>
        public List<GatLeeMeterialINVB> QueryGatLeeMatInvs(int solutionID, string shopCode, string materialCode, string sizeCode, int currentPage, int itemsPerPage, ref int totalCount, ref decimal totalQty, bool isNeedPage = true)
        {
            totalQty = 0;
            totalCount = 0;
            List<GatLeeMeterialINVB> Invlist = new List<GatLeeMeterialINVB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatLeeMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (!string.IsNullOrEmpty(materialCode)) query = query.Where(g => g.BarCode.Contains(materialCode));

            totalCount = query.Count();
            if (totalCount > 0)
            {
                totalQty = (int)(query.Sum(i => i.ActualQty));
            }
            if (isNeedPage)//分页
            {
                Invlist = query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatLeeMeterialINVB(g))
                   .ToList();
            }
            else
            {
                Invlist = query.OrderBy(m => m.ShopCode).Select(g => new GatLeeMeterialINVB(g)).ToList();
            }

            return Invlist;

        }

        #endregion
    }
}

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
        /// <summary>
        /// 查询Iker所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatIkerMaterialInvB> QueryGatIkerMatInvs(int currentPage, int itemsPerPage, out int Count, out int Qty, string shopCode = "",string start_date = "", string end_date = "")
        {
                    
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerMaterialInv>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(start_date))
            {
                DateTime dtToday = Convert.ToDateTime(start_date);
                query = query.Where(s => s.UpdateDate >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(end_date))
            {
                DateTime dtendToday = Convert.ToDateTime(end_date);
                query = query.Where(e => e.UpdateDate <= dtendToday);
            }


            Count = 0;
            Qty = 0;
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (int)query.Sum(m => m.Qty);
            }
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatIkerMaterialInvB(g))
                   .ToList();
        }
        /// <summary>
        /// 查询Iker所有的提报库存
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>

        public List<GatIkerMaterialInvB> QueryGatIkerMatInvsData(string start_date, string end_date, string shopCode, out int Count, out int Qty)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerMaterialInv>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode == shopCode);
            if (!string.IsNullOrWhiteSpace(start_date))
            {
                DateTime dtToday = Convert.ToDateTime(start_date);
                query = query.Where(s => s.UpdateDate >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(end_date))
            {
                DateTime dtendToday = Convert.ToDateTime(end_date);
                query = query.Where(e => e.UpdateDate <= dtendToday);
            }

            Count = 0;
            Qty = 0;
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (int)query.Sum(m => m.Qty);
            }
            return query
                   .OrderBy(m => m.UpdateDate)
                   .Select(g => new GatIkerMaterialInvB(g))
                   .ToList();
        }




        /// <summary>
        /// Iker销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatIkerPosSaleB> QueryGatIkerPosSales(int solutionID, int currentPage, int itemsPerPage, string start_date, string end_date, string shopCode, string shopName, string materialCode, string barCode, string sizeCode, out int Qty, out decimal Amount, out int Count)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSale>());
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
                query = query.Where(e => e.ShopName == shopName);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(e => e.MaterialCode == materialCode);
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(e => e.SizeName == sizeCode);
            }
            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(e => e.BarCode == barCode);
            }
            Qty = 0;
            Amount = 0;
            Count = 0;
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (int)(query.Sum(i => i.Qty));
                Amount = (decimal)query.Sum(i => i.SaleAmount);
            }

            return query
                  .OrderByDescending(p => p.BillTime)
                 .ThenBy(p => p.ShopCode)
                 .ThenBy(p => p.BillNo)
                 .ThenBy(p => p.MaterialCode)
                 .Skip((currentPage - 1) * itemsPerPage)
                 .Take(itemsPerPage)
                 .Select(g => new GatIkerPosSaleB(g))
                 .ToList();


        }

        /// <summary>
        /// 获取Iker销售数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public IQueryable<GatIkerPosSaleB> QueryGatIkerPosSalesQueryable(int solutionID, string start_date, string end_date, string shopName)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSale>());
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
            .Select(g => new GatIkerPosSaleB(g));
        }

        /// <summary>
        /// 中转数据（Iker的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatIkerPosSaleTransferB> QueryGatIkerPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage, string shopName, string start_date, string end_date, out int Qty, out decimal Amount, out int Count)
        { 
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrEmpty(shopName)) query = query.Where(g => g.ShopName == shopName);
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

            Qty = 0;
            Amount = 0;
            Count = 0;
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (int)(query.Sum(i => i.Qty));
                Amount = (decimal)query.Sum(i => i.SaleAmount);
            }

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatIkerPosSaleTransferB(g))
                   .ToList();
        }

        public HandleArgs ImportIkerPosSaletransfer(DataTable dt, string ddlSoule, string companyID)
        {
            List<GatIkerPosSaleTransfer> list = Utils.ConvertHelper.DataTableToList<GatIkerPosSaleTransfer>(dt);
            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                item.SolutionID = int.Parse(ddlSoule);
                item.ModifyTime = DateTime.Now;
                item.BillTime = DateTime.Now;
                item.ShopName = "";
                item.StockName = "";
                item.IsReturn = item.Qty < 0 ? true : false;
                handle = AddIkerPosSaletransfer(item);
            }
            return handle;
        }
        public HandleArgs AddIkerPosSaletransfer(GatIkerPosSaleTransfer entity)
        {
            HandleArgs handle = new HandleArgs();
            if (entity == null)
            {
                handle.Status = HandleStatus.Fail;
                handle.Message = "不能为空";
                return handle;
            }
            handle = _Service.InsertEntity(entity);
            if (handle.Status == HandleStatus.Completed)
                return handle;
            return handle;
        }
     
        /// <summary>
        /// 更新 POS销售数据的数量和销售金额
        /// </summary>
        /// <param name="Qty"></param>
        /// <param name="TransID"></param>
        /// <returns></returns>
        public HandleArgs UpdateIkertransfer(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Iker_PosSale_transfer SET SalesAmount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 选择删除中间表 POS销售
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns> 
        public HandleArgs DeleteSelectIkerSaletransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Iker_PosSale_transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }
        /// <summary>
        /// 全部 删除中间表 POS销售
        /// </summary>
        /// <returns></returns> 
        public HandleArgs DeleteAllIkerSaletransfer()
        {
            HandleArgs HANDLE = new HandleArgs();

            string command = @" DELETE Gat_Iker_PosSale_transfer ";
            HANDLE = _Service.ExecuteCommand(command);

            return HANDLE;
        }
        /// <summary>
        /// 中转数据总条数（Iker的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatIkerPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        /// <summary>
        /// 中转数据总条数（Iker的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public List<GatIkerPosSaleTransferB> GatIkerPosSaleTransfeSumCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Select(g => new GatIkerPosSaleTransferB(g)).ToList();
        }





        /// <summary>
        /// 查询所有符合条件的中转数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IQueryable<GatIkerPosSaleTransferB> QueryGatIkerPosSaleTransferData(int solutionID, string shopName, string startDate, string endDate)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrEmpty(shopName))
            {
                query = query.Where(g => g.ShopName == shopName);
            }
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                DateTime dtToday = Convert.ToDateTime(startDate);
                query = query.Where(s => s.BillDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endDate);
                query = query.Where(e => e.BillDate.Date <= dtendToday);
            }

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Select(g => new GatIkerPosSaleTransferB(g));
        }


        public HandleArgs DeleteIkerPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string shopCode, string shopName, string materialCode, string barCode, string sizeCode)
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
            string command = $" DELETE Gat_Iker_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteIkerPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Iker_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }



        #region
        /// <summary> 
        /// 获取库存中转数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatIkerMaterialInvB> QueryIkerStockTransfer(int pageIndex, int pageSize, string shopCode, out int Count, out decimal Qty)
        {
            Count = 0;
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatIkerMeterialTransfer>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(s => s.ShopCode == shopCode);
            }
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (decimal)query.Sum(s => s.Qty);
            }
            return query
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .Select(g => new GatIkerMaterialInvB(g))
                   .ToList();
        }

        /// <summary> 
        /// 库存中转数据全部删除
        /// </summary>
        /// <returns></returns>
        public HandleArgs TruncateIkerStockTransfer()
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = @" Truncate table Gat_Iker_Meterial_Transfer";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns>
        public HandleArgs SaveIkerTransfer(string strSolutionId,string TransIDstr, string OperationType)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(strSolutionId)) 
            {
                if (!HasDangerousContents(strSolutionId))
                {
                    string command = @" EXEC sp_Iker_TransferSave @SolutionId={0},@TransIDstr= '{1}',@OperationType= '{2}' ";
                    command = string.Format(command, strSolutionId, TransIDstr, OperationType);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

        public HandleArgs ImportIIkerPos(DataTable dt)
        {
            HandleArgs result = new HandleArgs(); 
            using (SqlConnection connection =
                   new SqlConnection(_Service.ConnectString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = 
                       "dbo.Gat_Iker_PosSale_transfer";
                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        result.Status = HandleStatus.Completed;
                    }
                    catch (Exception ex)
                    {
                        result.Status = HandleStatus.Fail;
                        result.Exception = ex;
                        result.Message = ex.Message;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyIkerStockTransfer(DataTable dt)
        {
            HandleArgs result = new HandleArgs();
            using (SqlConnection connection =
                   new SqlConnection(_Service.ConnectString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName =
                       "dbo.Gat_Iker_Meterial_Transfer";
                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        result.Status = HandleStatus.Completed;
                    }
                    catch (Exception ex)
                    {
                        result.Status = HandleStatus.Fail;
                        result.Exception = ex;
                        result.Message = ex.Message;
                    }
                }
            }
            return result;
        }


        #endregion
    }
}

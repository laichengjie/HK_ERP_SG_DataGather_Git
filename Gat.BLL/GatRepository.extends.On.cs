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


        #region On昂跑品牌
        public IQueryable<GatOnPosSaleB> GetOnPosSalesData(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string CardShopCode, string billNo, string status)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnPosSale>());
            if (solutionId > 0)
                query = query.Where(q => q.SolutionID == solutionId);

            if (startBillDate.HasValue)
            {
                query = query.Where(o => o.sale_date >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                query = query.Where(o => o.sale_date <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                query = query.Where(o => o.store_code.Contains(CardShopCode));
            }
            if (!string.IsNullOrEmpty(billNo))
            {
                query = query.Where(o => o.BillNo.Contains(billNo));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }

            var list = query
                .OrderBy(p => p.sale_date)
                .ThenBy(p => p.store_code)
                .ThenBy(p => p.BillNo)
                .Select(g => new GatOnPosSaleB(g));
            return list;
        }


        /// <summary>
        /// 查询On库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatOnMeterialINVDetailB> QueryOnInvDetailData(int currentPage, int itemsPerPage, string shopCode, string startBillTime, string endBillTime, string materialCode, string sizeCode, string barCode, out int Count, out decimal Qty)
        {
            Count = 0;
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatOnMeterialINVDetail>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.store_code.Contains(shopCode));


            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.inventory_date.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.inventory_date.Date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }

            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(s => s.sku.Contains(barCode));
            }

            Count = query.Count();
            if (Count > 0)
            {
                Qty = (decimal)query.Sum(s => s.Qty);
            }


            return query
                   .OrderBy(m => m.store_code)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatOnMeterialINVDetailB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询On库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatOnMeterialINVDetailB> QueryOnInvDetailList(string shopCode, string startBillTime, string endBillTime, string materialCode, string sizeCode, string barCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnMeterialINVDetail>());
            if (!string.IsNullOrEmpty(shopCode)) 
                query = query.Where(g => g.store_code.Contains(shopCode));

            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.inventory_date.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.inventory_date.Date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }

            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(s => s.sku.Contains(barCode));
            }
            return query
                   .OrderBy(m => m.store_code)
                   .Select(g => new GatOnMeterialINVDetailB(g));
        }


        /// <summary>
        /// 查询销售中转数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param> 
        /// <param name="solutionID"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> SelectOnPosSaleTransferPageList(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatOnPosSaleTransferB> posSalelist = new List<GatOnPosSaleTransferB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatOnPosSaleTransfer>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.sale_date.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.sale_date.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(e => e.sku.Contains(materialCode));
            }
            //if (!string.IsNullOrWhiteSpace(sizeCode))
            //{
            //    query = query.Where(e => e.SizeName.Contains(sizeCode));
            //}
            var list = query
                .OrderBy(p => p.sale_date)
                .ThenBy(p => p.store_code)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.sku);

            var totalCount = list.Count();

            posSalelist = list
                        .Select(g => new GatOnPosSaleTransferB(g))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            var total = list.ToList();
            decimal? qty = 0;
            decimal? amount = 0;
            if (totalCount > 0)
            {
                qty = list.Select(s => s.quantity)?.Sum();
                amount = list.Select(s => s.total_paid ?? 0m)?.Sum();
            }
            result.Add("data", posSalelist);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }


 
        /// <summary>
        /// 销售中转数据
        /// </summary> 
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatOnPosSaleTransferB> QueryGatOnPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.sku)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatOnPosSaleTransferB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询库存中转数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="solutionID"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param> 
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> SelectOnStockTransferPageList(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatOnMeterialINVB> posSalelist = new List<GatOnMeterialINVB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatOnMeterialTransfer>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.inventory_date.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.inventory_date.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(e => e.sku.Contains(sizeCode));
            }
            //if (!string.IsNullOrWhiteSpace(sizeCode))
            //{
            //    query = query.Where(e => e.SizeName.Contains(sizeCode));
            //}
            var list = query
                .OrderBy(p => p.inventory_date)
                .ThenBy(p => p.store_code)
                .ThenBy(p => p.sku);

            var totalCount = list.Count();

            posSalelist = list
                        .Select(g => new GatOnMeterialINVB(g))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            var total = list.ToList();
            decimal? qty = 0;
            decimal? amount = 0;
            if (totalCount > 0)
            {
                qty = list.Select(s => s.quantity)?.Sum();
                //amount = list.Select(s => s.total_paid ?? 0m)?.Sum();
            }
            result.Add("data", posSalelist);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }


        /// <summary>
        /// 库存中转数据
        /// </summary> 
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatOnMeterialINVB> QueryGatOnStocTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnMeterialTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.store_code)
                   .ThenBy(p => p.sku)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatOnMeterialINVB(g))
                   .ToList();
        }

        /// <summary> 
        /// 销售中转数据总条数
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatOnPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        /// <summary>
        /// 库存中转数据总条数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GetOnStockTransferTotalCount(int solutionID,string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnMeterialTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        ///  保存销售/库存中转数据（按单号）
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs TransferOnSave(string TransIDstr,string OperationType)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr)) 
            { 
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC sp_Gat_On_Transfer_PosStock  @SolutionId=0,@TransIDstr= '{0}',@Type=0,@OperationType='{1}' ";
                    command = string.Format(command, TransIDstr, OperationType);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 删除销售/库存中转数据（按单号） 
        /// </summary>
        /// <param name="TransIdstr"></param>
        /// <returns></returns>

        public HandleArgs TransferOnDeltet(string TransIdstr, string OperationType)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" EXEC sp_Gat_On_Transfer_PosStock  @SolutionId=0,@TransIDstr= '{0}',@Type=1,@OperationType='{1}' ";
                    command = string.Format(command, TransIdstr, OperationType);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 保存全部销售/库存中转数据到销售数据中
        /// </summary>
        /// <returns></returns> 
        public HandleArgs TransferOnSaveAll(int solutionId, string OperationType)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC sp_Gat_On_Transfer_PosStock  @solutionId='{solutionId}',@TransIDstr= '',@Type=0 ,@OperationType='{OperationType}' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }
        /// <summary>
        /// 删除全部销售/库存中转数据
        /// </summary> 
        /// <returns></returns>
        public HandleArgs DeleteOnTransferAll(int solutionId, string OperationType)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" EXEC sp_Gat_On_Transfer_PosStock  @solutionId='{solutionId}',@TransIDstr= '',@Type=1 ,@OperationType='{OperationType}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除符合条件销售/库存中转数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param> 
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteOnTransfer(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
        {
            string where = "";
            if (startBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),sale_date,121)>='{startBillDate.ToString("yyyy-MM-dd")}'";
            }
            if (endBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),sale_date,121)<='{endBillDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                where += $" AND order_number like '%{materialCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                where += $" AND sku like '%{sizeCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(shopCode))
            {
                where += $" AND store_code like '%{shopCode}%'";
            }

            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_On_transferAs_Pos WHERE  StateCode = 0 and  solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }




       /// <summary>
       /// 导入销售中转数据
       /// </summary>
       /// <param name="dt"></param>
       /// <returns></returns>

        public HandleArgs BulkCopyOnTransfer(DataTable dt)
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
                       "dbo.Gat_On_transferAs_Pos";
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
        /// 导入库存中转数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyOnStockTransfer(DataTable dt)
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
                       "dbo.Gat_On_transferAs_Material";
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

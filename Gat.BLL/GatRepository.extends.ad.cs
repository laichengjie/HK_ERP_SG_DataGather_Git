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
        //ad品牌数据相关操作

        /// <summary>
        /// 中转数据（Adidas的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GataAdPosSaletransferB> QueryGatAdPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GataAdPosSaletransferB(g))
                   .ToList();
        }

        /// <summary>
        /// 中转数据（Adidas的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GataAdPosSaletransferB> QueryGatAdPosSaleTransfer_OC (int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer_OC>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GataAdPosSaletransferB(g))
                   .ToList();
        }

        /// <summary>
        /// 中转数据（Adidas的调拨数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatAdMoveTransferB_OC> QueryGatAdMoveTransfer_OC(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMoveTransfer_OC>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMoveTransferB_OC(g))
                   .ToList();
        }
        /// <summary>
        /// 中转数据（Adidas的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GataAdPosSaletransferB_Wif> QueryGatAdPosSaleTransfer_Wif(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer_Wif>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.store_code)
                   .ThenBy(p => p.retail_id)
                   .ThenBy(p => p.product_code)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GataAdPosSaletransferB_Wif(g))
                   .ToList();
        }
        /// <summary>
        /// 中转数据（Adidas的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatAdPosSaleTransfer(int solutionID, DateTime startBillDate, DateTime endBillDate, string shopCode, string materialCode, string sizeCode, int page, int pageSize, bool isNeedPage = true)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GataAdPosSaletransferB> dataList = new List<GataAdPosSaletransferB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (startBillDate != null)
            {
                query = query.Where(q => q.BillDate >= startBillDate);
            }
            if (endBillDate != null)
            {
                query = query.Where(q => q.BillDate <= endBillDate.AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.SizeName.Contains(sizeCode));
            }
            if (!string.IsNullOrWhiteSpace(shopCode))
            {
                query = query.Where(s => s.ShopCode.Contains(shopCode));
            }


            var list = query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode);
            var dataCount = list.Count();
            if (isNeedPage)
            {
                dataList = list
                       .Select(g => new GataAdPosSaletransferB(g))
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                       .ToList();
            }
            else
            {
                dataList = list
                                .Select(g => new GataAdPosSaletransferB(g))
                                .ToList();
            }
            var total = list.ToList();
            decimal? qty = total.Select(s => s.Qty)?.Sum();
            decimal? amount = total.Select(s => s.SalesAmount ?? 0m)?.Sum();
            result.Add("data", dataList);
            result.Add("totalCount", dataCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public HandleArgs UpdateAdTransferQty(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Ad_PosSale_transfer SET SalesAmount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs UpdateAdTransferQty_OC(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Ad_PosSale_transfer_OC SET SaleAmount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        } 
        public HandleArgs UpdateAdTransferQty_Move_OC(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Ad_Move_transfer_OC SET Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs UpdateAdTransferQty_Wif(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Ad_PosSale_transfer_Wif SET tot_amt_actual=priceactual*{0},tot_amt_list=pricelist*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs InsertAdTransferQty(DataTable dt, string ddlSoule)
        {

            List<GatAdPosSaletransfer> list = Utils.ConvertHelper.DataTableToList<GatAdPosSaletransfer>(dt);
            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                item.SolutionID = int.Parse(ddlSoule);
                item.BillTime = DateTime.Now;

                handle = AddAdTransfer(item);

            }
            return handle;
        }

        public HandleArgs InsertAdTransfer(DataTable dt, string ddlSoule)
        {

            List<GatAdPosSaletransfer> list = Utils.ConvertHelper.DataTableToList<GatAdPosSaletransfer>(dt);
            HandleArgs handle = new HandleArgs();
            StringBuilder sql = new StringBuilder();
            sql.Append(@"INSERT INTO[dbo].[Gat_Ad_PosSale_transfer]
                        ([ShopCode]
                          ,[BillDate]
                          ,[BillTime]
                          ,[MaterialCode]
                          ,[Qty]
                          ,[BillNo]
                          ,[SalesAmount]
                          ,[PersonnelID]
                          ,[BarCode]
                          ,[SizeName]
                          ,[CompanyID]
                          ,[CompanyName]
                          ,[CardID]
                          ,[CardName]
                          ,[SolutionID]
                          ,[BalaPrice]
                          ,[ShopName]
                          ,[StockName]
                          ,[SmallTicketNo]) VALUES");
            foreach (var item in list)
            {
                item.SolutionID = int.Parse(ddlSoule);
                item.BillTime = DateTime.Now;
                sql.Append($@"('{item.ShopCode}'
           ,'{item.BillDate}'
           ,'{DateTime.Now}'
           ,'{item.MaterialCode}'
           ,'{item.Qty}'
           ,'{item.BillNo}'
           ,'{item.SalesAmount}'
           ,'{item.PersonnelID}'
           ,'{item.BarCode}'
           ,'{item.SizeName}'
           ,'{item.CompanyID}'
           ,'{item.CompanyName}'
           ,'{item.CardID}'
           ,'{item.CardName}'
           ,'{ddlSoule}'
           ,'{item.BalaPrice}'
           ,'{item.ShopName}'
           ,'{item.StockName}'
           ,'{item.SmallTicketNo}'),");
            }
            string insertSql = sql.ToString();
            insertSql = insertSql.Substring(0, insertSql.LastIndexOf(','));
            handle = _Service.ExecuteCommand(insertSql);
            return handle;
        }

        public HandleArgs BulkCopyAdTransfer(DataTable dt)
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
                       "dbo.Gat_Ad_PosSale_transfer";
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

        public HandleArgs BulkCopyAdTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Ad_PosSale_transfer_OC";
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

        public HandleArgs BulkCopyAdMoveTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Ad_Move_Transfer_OC";
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

        public HandleArgs BulkCopyAdTransfer_Wif(DataTable dt)
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
                       "dbo.Gat_Ad_PosSale_transfer_Wif";
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
        public HandleArgs DeleteAdTransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }
        public HandleArgs DeleteAdTransfer_OC(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

        public HandleArgs DeleteAdTransfer_Wif(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer_Wif WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

        public HandleArgs DeleteAdTransfer__Move_OC(string TransIDstr) 
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Ad_Move_transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }
        /// <summary>
        /// 删除全部中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除全部中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_OC(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer_OC WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除全部中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_Move_OC(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_Move_transfer_OC WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 删除全部中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_Wif(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer_Wif WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
        {
            string where = "";
            if (startBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>='{startBillDate.ToString("yyyy-MM-dd")}'";
            }
            if (endBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<='{endBillDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                where += $" AND MaterialCode like '%{materialCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                where += $" AND SizeName like '%{sizeCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(shopCode))
            {
                where += $" AND ShopCode like '%{shopCode}%'";
            }

            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_OC(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
        {
            string where = ""; 
            if (startBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>='{startBillDate.ToString("yyyy-MM-dd")}'";
            }
            if (endBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<='{endBillDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                where += $" AND MaterialCode like '%{materialCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                where += $" AND SizeName like '%{sizeCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(shopCode))
            {
                where += $" AND ShopCode like '%{shopCode}%'";
            }

            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_Move_OC(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string outStockCode, string inStockCode)
        {
            string where = "";
            if (startBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>='{startBillDate.ToString("yyyy-MM-dd")}'";
            }
            if (endBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<='{endBillDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                where += $" AND MaterialCode like '%{materialCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                where += $" AND SizeName like '%{sizeCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(outStockCode))
            {
                where += $" AND OutStockCode like '%{outStockCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(inStockCode))
            {
                where += $" AND InStockCode like '%{inStockCode}%'";
            }

            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_Move_transfer_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdTransfer_Wif(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
        {
            string where = "";
            if (startBillDate != null) 
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>='{startBillDate.ToString("yyyy-MM-dd")}'";
            }
            if (endBillDate != null)
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<='{endBillDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                where += $" AND product_code like '%{materialCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                where += $" AND size_code like '%{sizeCode}%'";
            }
            if (!string.IsNullOrWhiteSpace(shopCode))
            {
                where += $" AND store_code like '%{shopCode}%'";
            }

            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_transfer_Wif WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs AddAdTransfer(GatAdPosSaletransfer entity)
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

        public HandleArgs TransferAdSaveAs(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ad_transferAs_Pos '{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        public HandleArgs TransferAdSaveAs_OC(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr)) 
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ad_transferAs_Pos_OC '0','{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        public HandleArgs TransferAdSaveAs_Move_OC(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ad_transferAs_Move_OC '0','{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        public HandleArgs TransferAdSaveAs_Wif(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ad_transferAs_Pos_Wif '0','{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        /// <summary>
        /// 报错全部中转数据到销售数据中
        /// </summary>
        /// <returns></returns>
        public HandleArgs TransferAdSaveAs(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_Ad_transferAs_Pos '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        /// <summary>
        /// 保存全部中转数据到销售数据中
        /// </summary> 
        /// <returns></returns>
        public HandleArgs TransferAdSaveAs_OC(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_Ad_transferAs_Pos_OC '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        /// <summary>
        /// 保存全部中转数据到销售数据中
        /// </summary> 
        /// <returns></returns> 
        public HandleArgs TransferAdSaveAs_Move_OC(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_Ad_transferAs_Move_OC '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        /// <summary>
        /// 保存全部中转数据到销售数据中
        /// </summary> 
        /// <returns></returns>
        public HandleArgs TransferAdSaveAs_Wif(int solutionId) 
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_Ad_transferAs_Pos_Wif '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdMeterialINVB> QueryGatAdMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
        }
        /// <summary>
        /// 查询阿迪所有的提报库存 全渠道
        /// </summary>
        /// <param name="shopCode"></param> 
        /// <returns></returns>
        public List<GatAdMeterialINVB> QueryGatAdMatInvs_OC(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询阿迪所有的提报库存 全渠道 三小时库存
        /// </summary>
        /// <param name="shopCode"></param> 
        /// <returns></returns>
        public List<GatAdMeterialINVB> QueryGatAdMatInvs_OCTB(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB> QueryGatAdMatInvsQyeryable(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Select(g => new GatAdMeterialINVB(g));
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB> QueryGatAdMatInvsQyeryable_OC(string shopCode = "" ,string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.UpdateDate >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.UpdateDate <= dtendToday);
            }
           
            if (!string.IsNullOrWhiteSpace(materialcode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialcode));
            }
            if (!string.IsNullOrWhiteSpace(sizecode))
            {
                query = query.Where(s => s.SizeName.Contains(sizecode));
            }

            if (!string.IsNullOrWhiteSpace(stockcode))
            {
                query = query.Where(s => s.StockCode.Contains(stockcode));
            }
            return query
                   .OrderBy(m => m.StockCode)
                   .Select(g => new GatAdMeterialINVB(g));
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB> QueryGatAdMatInvsQyeryable_OCTB(string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.UpdateDate >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.UpdateDate <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialcode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialcode));
            }
            if (!string.IsNullOrWhiteSpace(sizecode))
            {
                query = query.Where(s => s.SizeName.Contains(sizecode));
            }

            if (!string.IsNullOrWhiteSpace(stockcode))
            {
                query = query.Where(s => s.StockCode.Contains(stockcode));
            }
            return query
                   .OrderBy(m => m.StockCode)
                   .Select(g => new GatAdMeterialINVB(g));
        }
        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB> QueryGatAdMatInvsQyeryable_OCTB(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.StockCode)
                   .Select(g => new GatAdMeterialINVB(g));
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB_Wif> QueryGatAdMatInvsQyeryable_Wif(string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_Wif>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.store_code.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.stats_date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.stats_date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialcode))
            {
                query = query.Where(s => s.product_code.Contains(materialcode));
            }
            if (!string.IsNullOrWhiteSpace(sizecode))
            {
                query = query.Where(s => s.size_code.Contains(sizecode));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                query = query.Where(s => s.bar_code.Contains(barcode));
            }
            return query
                   .OrderBy(m => m.store_code)
                   .Select(g => new GatAdMeterialINVB_Wif(g));
        }
        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMeterialINVB_Wif> QueryGatAdMatInvsQyeryable_WifTB(string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_WifTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.store_code.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.stats_date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.stats_date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialcode))
            {
                query = query.Where(s => s.product_code.Contains(materialcode));
            }
            if (!string.IsNullOrWhiteSpace(sizecode))
            {
                query = query.Where(s => s.size_code.Contains(sizecode));
            }

            //if (!string.IsNullOrWhiteSpace(stockcode))
            //{
            //    query = query.Where(s => s.StockCode.Contains(stockcode));
            //}
            return query
                   .OrderBy(m => m.store_code)
                   .Select(g => new GatAdMeterialINVB_Wif(g));
        }


        /// <summary> 
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary> 
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdMatInvCount_OC(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary> 
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdMatInvCount_OCTB(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        /// <summary>
        /// 阿迪销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdPosSaleB> QueryGatAdPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdPosSaleB(g))
                   .ToList();
        }
        /// <summary>
        /// 阿迪销售 全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdPosSaleB> QueryGatAdPosSales_OC(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdPosSaleB(g))
                   .ToList();
        }

        /// <summary>
        /// 阿迪销售 全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdPosSaleB> QueryGatAdPosSales_OCTB(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdPosSaleB(g))
                   .ToList();
        }

        /// <summary>
        /// 阿迪调拨 全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdMoveB> QueryGatAdMove_OC(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMoveB(g))
                   .ToList();
        }

        /// <summary>
        /// 阿迪调拨 全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdMoveB> QueryGatAdMove_OCTB(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMoveB(g))
                   .ToList();
        }


        /// <summary>
        /// 阿迪出入库 全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdOutInB_OC> QueryGatAdOutIn_OCTB(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdOutIn_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdOutInB_OC(g))
                   .ToList();
        }

        public decimal[] QueryGatTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }
        public decimal[] QueryGatTotal_OC()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OC>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }

        public decimal[] QueryGatTotal_OCTB()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OCTB>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                 Qty = query.Select(s => s.Qty).Sum();
                 prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }
        public decimal[] QueryGatTSTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer>());
            decimal Qty = query.Select(s => s.Qty).Sum();
            decimal prices = query.Select(s => s.SalesAmount).Sum() ?? 0m;
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }
         

        public decimal[] QueryGatTotal_Move_OC()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OC>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                //prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }

        public decimal[] QueryGatTotal_Move_OCTB()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OCTB>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                //prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }

        public decimal[] QueryGatTotal_OutIn_OCTB()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OCTB>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                //prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks; 
        }

        public decimal[] QueryGatTotal_Wif()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_Wif>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = (decimal) query.Select(s => s.qty).Sum();
                prices = (decimal)query.Select(s => s.tot_amt_actual).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }

        public decimal[] QueryGatTotal_WifTB()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_WifTB>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                Qty = (decimal)query.Select(s => s.qty).Sum();
                prices = (decimal)query.Select(s => s.tot_amt_actual).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }

        public decimal[] QueryGatTotal_Personnel_Wif ()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPersonnel_Wif>());
            decimal Qty = 0;
            Qty = query.Count();
            decimal[] marks = new decimal[] { Qty };
            return marks;
        }

        /// <summary>
        /// 
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
        public Dictionary<string, object> QueryGatAdPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode, int currentPage, int itemsPerPage, bool isNeedPage = true)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatAdPosSaleB> posSalelist = new List<GatAdPosSaleB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
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
                       .Select(g => new GatAdPosSaleB(g))
                       .Skip((currentPage - 1) * itemsPerPage)
                       .Take(itemsPerPage)
                       .ToList();
            }
            else
            {
                posSalelist = list
                                .Select(g => new GatAdPosSaleB(g))
                                .ToList();
            }
            var total = list.ToList();
            decimal? qty = total.Select(s => s.Qty)?.Sum();
            decimal? amount = total.Select(s => s.SaleAmount ?? 0m)?.Sum();
            result.Add("data", posSalelist);
            result.Add("dataCount", dataCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public IQueryable<GatAdPosSaleB> QueryGatAdPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
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
                .Select(g => new GatAdPosSaleB(g));
            return list;
        }

        /// <summary>
        /// 阿迪 全渠道 销售日数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdPosSaleB_OC> QueryGatAdPosSalesData_OC(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OC>());
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
                .Select(g => new GatAdPosSaleB_OC(g));
            return list;
        }

        /// <summary>
        ///  阿迪 全渠道 销售三小时数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdPosSaleB_OC> QueryGatAdPosSalesData_OCTB(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OCTB>());
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
                .Select(g => new GatAdPosSaleB_OC(g));
            return list;
        }

        /// <summary>
        /// 阿迪 全渠道 销售日数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMoveB> QueryGatAdMoveData_OC(int solutionID, string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OC>());
            if (!string.IsNullOrEmpty(outStockCode)) query = query.Where(g => g.OutStockCode.Contains(outStockCode));
            if (!string.IsNullOrEmpty(inStockCode)) query = query.Where(g => g.InStockCode.Contains(inStockCode));
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
                .Select(g => new GatAdMoveB(g));
            return list;
        }

   

        /// <summary>
        /// 阿迪 全渠道 销售日数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param> 
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdMoveB> QueryGatAdMoveData_OCTB(int solutionID, string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OCTB>());
            if (!string.IsNullOrEmpty(outStockCode)) query = query.Where(g => g.OutStockCode.Contains(outStockCode));
            if (!string.IsNullOrEmpty(inStockCode)) query = query.Where(g => g.InStockCode.Contains(inStockCode));
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
                .Select(g => new GatAdMoveB(g));
            return list;
        }

        /// <summary>
        ///阿迪 全渠道 出入库三小时数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param> 
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdOutInB_OC> QueryGatAdOutInData_OCTB(int solutionID, string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdOutIn_OCTB>());
            if (!string.IsNullOrEmpty(outStockCode)) query = query.Where(g => g.OutStockCode.Contains(outStockCode));
            if (!string.IsNullOrEmpty(inStockCode)) query = query.Where(g => g.InStockCode.Contains(inStockCode));
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
                .Select(g => new GatAdOutInB_OC(g));
            return list;
        }


        /// <summary>
        /// 阿迪 智慧零售 销售日数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdPosSaleB_Wif> QueryGatAdPosSalesData_Wif(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_Wif>());
            var dd = query.ToList();

            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.store_code.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.billtime.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.billtime.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.billdate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.billdate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.product_code.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.size_code.Contains(sizeCode));
            }
            
            var list = query
                .OrderBy(p => p.billtime)
                .ThenBy(p => p.store_code)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.product_code)
                .Select(g => new GatAdPosSaleB_Wif(g));
            return list;
        }


        /// <summary>
        /// 阿迪 智慧零售 销售日数据查询
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdPosSaleB_Wif> QueryGatAdPosSalesData_WifTB(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_WifTB>());
            var dd = query.ToList();

            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.store_code.Contains(shopCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.billtime.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.billtime.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime dtToday = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.billdate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.billdate.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.product_code.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.size_code.Contains(sizeCode));
            }

            var list = query
                .OrderBy(p => p.billtime)
                .ThenBy(p => p.store_code)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.product_code)
                .Select(g => new GatAdPosSaleB_Wif(g));
            return list;
        }


        /// <summary>
        ///阿迪 智慧零售 职员资料
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param> 
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public IQueryable<GatAdPersonnelB_Wif> QueryGatAdPersonnel_Wif (int solutionID, string staffCode , string staffName , string shopCode )
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPersonnel_Wif>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            if (!string.IsNullOrEmpty(shopCode)) 
                query = query.Where(g => g.store_code.Contains(shopCode));

            if (!string.IsNullOrWhiteSpace(staffCode))
            {
                query = query.Where(s => s.staff_code.Contains(staffCode));
            }
            if (!string.IsNullOrWhiteSpace(staffName))
            {
                query = query.Where(s => s.name.Contains(staffName));
            }
            var list = query
                .OrderBy(p => p.store_code)
                .ThenBy(p => p.staff_code)
                .ThenBy(p => p.name)
                .Select(g => new GatAdPersonnelB_Wif(g));
            return list;
        }

        public List<GatAdPosSaleB> QueryGatAdPosSalesALL()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
            return query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatAdPosSaleB(g))
                .ToList();
        }
        /// <summary>
        /// 阿迪销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        /// <summary>
        /// 阿迪销售计数  全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdPosSaleCount_OC(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 阿迪销售计数  全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdPosSaleCount_OCTB(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSale_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 阿迪销售计数  全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdMoveCount_OC(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OC>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 阿迪销售计数  全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdMoveCount_OCTB(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMove_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        /// <summary>
        /// 阿迪出入库计数  全渠道
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAdOutInCount_OCTB(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdOutIn_OCTB>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        public HandleArgs DeleteAdPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteAdPosSalesData_OC(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteAdPosSalesData_OCTB(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteAdMoveData_OC(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_Move_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteAdMoveData_OCTB(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)>=CONVERT(varchar(10),'{startBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(startBillTime))
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_Move_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteAdOutInData_OCTB(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)>=CONVERT(varchar(10),'{startBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(startBillTime))
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_OutIn_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        } 

        public HandleArgs DeleteAdPosSalesData_Wif(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_Wif WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs DeleteAdPosSalesData_WifTB(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ad_PosSale_Wif_TB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdPosSalesData_OC(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_PosSale_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdPosSalesData_OCTB(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_PosSale_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdMoveData_OC(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_Move_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdMoveData_OCTB(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_Move_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdOutInData_OCTB(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_OutIn_OCTB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪智慧零售
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdPosSalesData_Wif (int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_PosSale_Wif WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪智慧零售
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteAdPosSalesData_WifTB(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ad_PosSale_Wif_TB WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }


        #region 库存中转
        //导入
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyAdStockTransfer(DataTable dt)
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
                       "dbo.Gat_Ad_Stock_Transfer";
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
        public HandleArgs BulkCopyAdStockTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Ad_Stock_Transfer_OC";
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
        /// 删除全部
        /// </summary>
        /// <returns></returns>
        public HandleArgs TruncateAdStockTransfer()
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = @" Truncate table Gat_Ad_Stock_transfer";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <returns></returns>
        public HandleArgs TruncateAdStockTransfer_OC()
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = @" Truncate table Gat_Ad_Stock_transfer_OC";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns>
        public HandleArgs SaveAdStockTransfer(string strSolutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $@" declare @CompanyName NVARCHAR(20)
select @CompanyName=CompanyName FROM Gat_Solution a,Bas_Company b where a.CompanyID=b.CompanyID and a.id='{strSolutionId}';
delete a from gat_ad_meterialinv a,gat_ad_stock_transfer b where a.shopcode=b.shopcode and a.barcode=b.barcode;
insert into Gat_Ad_MeterialINV(ShopCode,CardName,BarCode,MaterialCode,SizeName,Qty,UpdateDate,CompanyName)
select ShopCode,StockCode,BarCode,MaterialCode,SizeCode,Qty,UpdateDate,@CompanyName from Gat_Ad_Stock_Transfer";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns>
        public HandleArgs SaveAdStockTransfer_OC(string strSolutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $@" declare @CompanyName NVARCHAR(20)
select @CompanyName=CompanyName FROM Gat_Solution a,Bas_Company b where a.CompanyID=b.CompanyID and a.id='{strSolutionId}';
delete a from gat_ad_meterialinv_OC a,gat_ad_stock_transfer_OC b where a.shopcode=b.shopcode and a.barcode=b.barcode;
insert into Gat_Ad_MeterialINV_OC(ShopCode,CardName,BarCode,MaterialCode,SizeName,Qty,UpdateDate,CompanyName,YCQty,StockCode,StockName,SizeCode,InventorySource,PCode)
select ShopCode,StockCode,BarCode,MaterialCode,SizeName,Qty,UpdateDate,@CompanyName,YCQty,StockCode,StockName,SizeCode,InventorySource,PCode from Gat_Ad_Stock_Transfer_OC";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        //查询
        /// <summary>
        /// 获取库存中转数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdStockTransferB> QueryAdStockTransfer(int pageIndex, int pageSize, string shopCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdStockTransfer>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(s => s.ShopCode == shopCode);
            }

            return query
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .Select(g => new GatAdStockTransferB(g))
                   .ToList();
        }
        //查询
        /// <summary>
        /// 获取库存中转数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatAdStockTransferB> QueryAdStockTransfer_OC(int pageIndex, int pageSize, string shopCode,ref decimal totalQty, ref decimal totalYcQty)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdStockTransfer_OC>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(s => s.ShopCode == shopCode);
            }
            var recordCount = query.Count();
            totalQty = 0;
            totalYcQty = 0;
            if (recordCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.Qty);
                totalYcQty = (decimal)query.Sum(s => s.YCQty);
            }

            return query
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .Select(g => new GatAdStockTransferB(g))
                   .ToList();
        }
        //统计数量
        public int GetAdStockTransferTotalCount(string shopCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdStockTransfer>());
            return query.Count();
        }

        //统计数量
        public int GetAdStockTransferTotalCount_OC(string shopCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdStockTransfer_OC>());
            return query.Count();
        }

        //统计条数
        #endregion

        #region 库存报表

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public Dictionary<string,object> QueryGatAdMatInvs(int currentPage,int itemsPerPage, string shopCode = "",string barcode="",string materialcode="",string sizecode="")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(g => g.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(materialcode))
            {
                query = query.Where(g => g.MaterialCode.Contains(materialcode));
            }
            if (!string.IsNullOrEmpty(sizecode))
            {
                query = query.Where(g => g.SizeName.Contains(sizecode));
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(g => g.BarCode.Contains(barcode));
            }
            var recordCount = query.Count();
            result.Add("recordCount", recordCount);
            var records= query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
            result.Add("records", records);
            return result;
        }


        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatAdMatInvs_OC(int currentPage, int itemsPerPage,ref decimal totalQty, ref decimal totalYcQty, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "",string stockcode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OC>());
            if (!string.IsNullOrEmpty(startBillTime)) 
            {
                DateTime DtStart = Convert.ToDateTime(startBillTime);
                query = query.Where(g => g.UpdateDate>= DtStart);
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                DateTime Dtend = Convert.ToDateTime(endBillTime);
                query = query.Where(g => g.UpdateDate <= Dtend);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(g => g.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(stockcode))
            {
                query = query.Where(g => g.StockCode.Contains(stockcode));
            }
            if (!string.IsNullOrEmpty(materialcode))
            {
                query = query.Where(g => g.MaterialCode.Contains(materialcode));
            }
            if (!string.IsNullOrEmpty(sizecode))
            {
                query = query.Where(g => g.SizeName.Contains(sizecode));
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(g => g.BarCode.Contains(barcode));
            }
            var recordCount = query.Count();
            result.Add("recordCount", recordCount);
            var records = query
                   .OrderBy(m => m.StockCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
            result.Add("records", records);
            totalQty = 0;
            totalYcQty = 0;
            if (recordCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.Qty);
                totalYcQty = (decimal)query.Sum(s => s.YCQty);
            }
            return result;
        }


        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatAdMatInvs_OCTB(int currentPage, int itemsPerPage, ref decimal totalQty, ref decimal totalYcQty, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_OCTB>());
            if (!string.IsNullOrEmpty(startBillTime))
            {
                DateTime DtStart = Convert.ToDateTime(startBillTime);
                query = query.Where(g => g.UpdateDate >= DtStart);
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                DateTime Dtend = Convert.ToDateTime(endBillTime);
                query = query.Where(g => g.UpdateDate <= Dtend);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(g => g.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(stockcode))
            {
                query = query.Where(g => g.StockCode.Contains(stockcode));
            }
            if (!string.IsNullOrEmpty(materialcode))
            {
                query = query.Where(g => g.MaterialCode.Contains(materialcode));
            }
            if (!string.IsNullOrEmpty(sizecode))
            {
                query = query.Where(g => g.SizeName.Contains(sizecode));
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(g => g.BarCode.Contains(barcode));
            }
            var recordCount = query.Count();
            result.Add("recordCount", recordCount);
            var records = query
                   .OrderBy(m => m.StockCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB(g))
                   .ToList();
            result.Add("records", records);
            totalQty = 0;
            totalYcQty = 0;
            if (recordCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.Qty);
                totalYcQty = (decimal)query.Sum(s => s.YCQty);
            }
            return result;
        }


        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param> 
        /// <returns></returns>
        public Dictionary<string, object> QueryGatAdMatInvs_Wif(int currentPage, int itemsPerPage, ref decimal totalQty, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_Wif>());
            if (!string.IsNullOrEmpty(startBillTime))
            {
                DateTime DtStart = Convert.ToDateTime(startBillTime);
                query = query.Where(g => g.stats_date >= DtStart);
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                DateTime Dtend = Convert.ToDateTime(endBillTime);
                query = query.Where(g => g.stats_date <= Dtend);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(g => g.store_code.Contains(shopCode));
            }
            //if (!string.IsNullOrEmpty(stockcode))
            //{
            //    query = query.Where(g => g.StockCode.Contains(stockcode));
            //}
            if (!string.IsNullOrEmpty(materialcode))
            {
                query = query.Where(g => g.product_code.Contains(materialcode));
            }
            if (!string.IsNullOrEmpty(sizecode))
            {
                query = query.Where(g => g.size_code.Contains(sizecode));
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(g => g.bar_code.Contains(barcode));
            }
            var recordCount = query.Count();
            result.Add("recordCount", recordCount);
            var records = query
                   .OrderBy(m => m.store_code)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB_Wif(g))
                   .ToList();
            result.Add("records", records);
            totalQty = 0;
            if (recordCount > 0)
            {
                totalQty = (int)query.Sum(s => s.qty);
               
            }
            return result;
        }

        /// <summary>
        /// 查询阿迪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param> 
        /// <returns></returns>
        public Dictionary<string, object> QueryGatAdMatInvs_WifTB(int currentPage, int itemsPerPage, ref decimal totalQty, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMeterialINV_WifTB>());
            if (!string.IsNullOrEmpty(startBillTime))
            {
                DateTime DtStart = Convert.ToDateTime(startBillTime);
                query = query.Where(g => g.stats_date >= DtStart);
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                DateTime Dtend = Convert.ToDateTime(endBillTime);
                query = query.Where(g => g.stats_date <= Dtend);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(g => g.store_code.Contains(shopCode));
            }
            //if (!string.IsNullOrEmpty(stockcode))
            //{
            //    query = query.Where(g => g.StockCode.Contains(stockcode));
            //}
            if (!string.IsNullOrEmpty(materialcode))
            {
                query = query.Where(g => g.product_code.Contains(materialcode));
            }
            if (!string.IsNullOrEmpty(sizecode))
            {
                query = query.Where(g => g.size_code.Contains(sizecode));
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(g => g.bar_code.Contains(barcode));
            }
            var recordCount = query.Count();
            result.Add("recordCount", recordCount);
            var records = query
                   .OrderBy(m => m.store_code)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAdMeterialINVB_Wif(g))
                   .ToList();
            result.Add("records", records);
            totalQty = 0;
            if (recordCount > 0)
            {
                totalQty = (int)query.Sum(s => s.qty);

            }
            return result;
        }
        #endregion
    }
}

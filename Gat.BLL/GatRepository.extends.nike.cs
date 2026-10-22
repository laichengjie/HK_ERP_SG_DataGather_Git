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
        #region 销售数据
        /// <summary>
        /// b、Nike 全渠道 销售日数据查询
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
        public IQueryable<GatNikePosSaleB_OC> QueryGatNikePosSalesData_OC(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNikePosSale_OC>());
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
                .Select(g => new GatNikePosSaleB_OC(g));
            return list;
        }


        public HandleArgs DeleteNikePosSalesData_OC(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            string command = $" DELETE Gat_Nike_PosSale_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 耐克全渠道
        /// </summary> 
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteNikePosSalesData_OC(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Nike_PosSale_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        #endregion

        #region 调拨数据
        /// <summary>
        /// Nike全渠道 调拨日数据查询
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
        public IQueryable<GatNikeMoveB_OC> QueryGatNikeMoveData_OC(int solutionID, string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNikeMove_OC>());
            if (!string.IsNullOrEmpty(outStockCode)) query = query.Where(g => g.OutStockCode.Contains(outStockCode));
            if (!string.IsNullOrEmpty(inStockCode)) query = query.Where(g => g.InStockCode.Contains(inStockCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            //if (!string.IsNullOrWhiteSpace(startBillTime))
            //{
            //    DateTime dtToday = Convert.ToDateTime(startBillTime);
            //    query = query.Where(s => s.BillTime.Date >= dtToday);
            //}
            //if (!string.IsNullOrWhiteSpace(endBillTime))
            //{
            //    DateTime dtendToday = Convert.ToDateTime(endBillTime);
            //    query = query.Where(e => e.BillTime.Date <= dtendToday);
            //}
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
                .OrderBy(p => p.BillDate)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatNikeMoveB_OC(g));
            return list;
        }


        public HandleArgs DeleteNikeMoveData_OC(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            string command = $" DELETE Gat_Nike_Move_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteNikeMoveData_OC(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Nike_Move_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        #endregion


        #region 出入库数据
        public IQueryable<GatNikeOutInB_OC> QueryGatNikeOutInData_OC(int solutionID, string outStockCode, string inStockCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNikeOutIn_OC>());
            if (!string.IsNullOrEmpty(outStockCode)) query = query.Where(g => g.OutStockCode.Contains(outStockCode));
            if (!string.IsNullOrEmpty(inStockCode)) query = query.Where(g => g.InStockCode.Contains(inStockCode));
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            //if (!string.IsNullOrWhiteSpace(startBillTime)) 
            //{
            //    DateTime dtToday = Convert.ToDateTime(startBillTime); 
            //    query = query.Where(s => s.BillTime.Date >= dtToday);
            //} 
            //if (!string.IsNullOrWhiteSpace(endBillTime))
            //{
            //    DateTime dtendToday = Convert.ToDateTime(endBillTime);
            //    query = query.Where(e => e.BillTime.Date <= dtendToday);
            //}
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
                .OrderBy(p => p.BillDate)
                .ThenBy(p => p.OutStockCode)
                .ThenBy(p => p.InStockCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatNikeOutInB_OC(g));
            return list;
        }


        public HandleArgs DeleteNikeOutInData_OC(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),ModifyTime,121)>=CONVERT(varchar(10),'{startBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                where += $" AND CONVERT(varchar(10),ModifyTime,121)<=CONVERT(varchar(10),'{endBillTime}',121)";
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
            string command = $" DELETE Gat_Nike_OutIn_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据 阿迪全渠道
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns> 
        public HandleArgs DeleteNikeOutInData_OC(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Nike_OutIn_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        #endregion

        #region 库存数据
        /// <summary>
        /// 查询Nike所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatNikeMatInvs_OC(int solutionID, int currentPage, int itemsPerPage, ref decimal totalQty, ref decimal totalYcQty, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var query = _Service.QueryEntity(ExpHelper.True<GatNikeMeterialInv_OC>());

            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);


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
                query = query.Where(g => g.SizeCode.Contains(sizecode));
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
                   .Select(g => new GatNikeMeterialInvB_OC(g))
                   .ToList();
            result.Add("records", records);
            totalQty = 0;
            totalYcQty = 0;
            if (recordCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.Qty);
                totalYcQty = (decimal)query.Sum(s => s.PathQty);
            }
            return result;
        }


        /// <summary>
        /// 查询Nike所有的提报库存
        /// </summary> 
        /// <param name="currentPage"></param> 
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param> 
        /// <returns></returns>
        public IQueryable<GatNikeMeterialInvB_OC> QueryGatNikeMatInvs_OC(int solutionID, string shopCode = "", string barcode = "", string materialcode = "", string sizecode = "", string startBillTime = "", string endBillTime = "", string stockcode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNikeMeterialInv_OC>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

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
                query = query.Where(s => s.SizeCode.Contains(sizecode));
            }

            if (!string.IsNullOrWhiteSpace(stockcode))
            {
                query = query.Where(s => s.StockCode.Contains(stockcode));
            }
            return query
                   .OrderBy(m => m.StockCode)
                   .Select(g => new GatNikeMeterialInvB_OC(g));
        }

        #endregion

        #region 销售导入


        /// <summary>
        /// Nike 全渠道銷售 中转数据
        /// </summary> 
        /// <param name="TransIdstr"></param>
        /// <returns></returns> 
        public HandleArgs DeleteNikePosSaleTransfer_OC(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Nike_PosSale_transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        public HandleArgs TransferNikeSaveAs_OC(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Nike_transferAs_Pos_OC '0','{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 保存全部中转数据到销售数据中 
        /// </summary>  
        /// <returns></returns>
        public HandleArgs TransferNikeSaveAs_OC(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_Nike_transferAs_Pos_OC '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        public HandleArgs UpdateNikeTransferQty_OC(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Nike_PosSale_transfer_OC SET SaleAmount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除全部中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteNikeTransfer_OC(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Nike_PosSale_transfer_OC WHERE solutionId='{solutionId}'";
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
        public HandleArgs DeleteNikeTransfer_OC(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
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
            string command = $" DELETE Gat_Nike_PosSale_transfer_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs BulkCopyNikeTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Nike_PosSale_transfer_OC";
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

        #region 库存导入
        //查询
        /// <summary>
        /// 获取库存中转数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatNikeStockTransferB_OC> QueryNikeStockTransfer_OC(int pageIndex, int pageSize, string shopCode, ref decimal totalQty, ref decimal totalPathQty, ref int totalCount)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNikeStockTransfer_OC>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(s => s.ShopCode == shopCode);
            } 
            var recordCount = query.Count();
            totalQty = 0;
            totalCount = 0;
            totalPathQty =0;
            if (recordCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.Qty);
                totalPathQty = (decimal)query.Sum(s => s.PathQty);
                totalCount = query.Count();
            }

            return query
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .Select(g => new GatNikeStockTransferB_OC(g))
                   .ToList();
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyNikeStockTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Nike_Stock_Transfer_OC";
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
        public HandleArgs TruncateNikeStockTransfer_OC(string strSolutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $@" delete Gat_Nike_Stock_Transfer_OC WHERE solutionId='{strSolutionId}'";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE; 
        }

        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns> 
        public HandleArgs SaveNikeStockTransfer_OC(string solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC sp_SGIS_Nike_Stock_Transfer_OC '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }

        #endregion

        #region 调拨导入

        /// <summary>
        /// 选择删除  中转数据
        /// </summary> 
        /// <param name="TransIdstr"></param>
        /// <returns></returns> 
        public HandleArgs DeleteNikeMoveTransfer_OC(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Nike_Move_Transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 删除全部  中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteNikeMoveTransfer_OC(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Nike_Move_Transfer_OC WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }


        /// <summary> 
        /// 删除符合条件的 中转数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteNikeMoveTransfer_OC(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
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
            string command = $" DELETE Gat_Nike_Move_Transfer_OC WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 选择保存 中转数据
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>

        public HandleArgs SaveNikeMoveTransfer_OC(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC sp_SGIS_Nike_Move_Transfer_OC '0','{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 全部保存  中转数据
        /// </summary>  
        /// <returns></returns>
        public HandleArgs SaveNikeMoveTransfer_OC(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC sp_SGIS_Nike_Move_Transfer_OC '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }
         
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs ImportNikeMoveTransfer_OC(DataTable dt)
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
                       "dbo.Gat_Nike_Move_Transfer_OC";
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

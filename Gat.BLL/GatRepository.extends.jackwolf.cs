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


        #region 狼爪品牌
        /// <summary>
        /// 查询狼爪数据
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
        public Dictionary<string, object> QueryGatJackWolfSkinPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode, int currentPage, int itemsPerPage, bool isNeedPage = true)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatJackWolfSkinPosSaleB> posSalelist = new List<GatJackWolfSkinPosSaleB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());
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
                       .Select(g => new GatJackWolfSkinPosSaleB(g))
                       .Skip((currentPage - 1) * itemsPerPage)
                       .Take(itemsPerPage)
                       .ToList();
            }
            else
            {
                posSalelist = list
                                .Select(g => new GatJackWolfSkinPosSaleB(g))
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

        public IQueryable<GatJackWolfSkinPosSaleB> QueryGatJackWolfSkinPosSalesData(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());
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
                .Select(g => new GatJackWolfSkinPosSaleB(g));
            return list;
        }


        /// <summary>
        /// 销售中转数据
        /// </summary> 
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatJackWolfSkinPosSaleTransferB> QueryGatJackWolfSkinPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatJackWolfSkinPosSaleTransferB(g))
                   .ToList();
        }
        /// <summary> 
        /// 中转数据总条数（销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatJackWolfSkinPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        public List<GatJackWolfSkinPosSaleB> QueryGatJackWolfSkinPosSalesALL()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());
            return query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillNo)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatJackWolfSkinPosSaleB(g))
                .ToList();
        }
        /// <summary>
        /// 狼爪销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatJackWolfSkinPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        public HandleArgs DeleteJackWolfSkinPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
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
            string command = $" DELETE Gat_JackWolfSkin_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteJackWolfSkinPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_JackWolfSkin_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 查询狼爪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatJackWolfSkinMeterialINVB> QueryGatJackWolfSkinMatInvs(int currentPage, int itemsPerPage, string shopCode,string startBillTime,string endBillTime,string materialCode , string sizeCode, string barCode,out int Count,out decimal Qty)
        {
            Count = 0;
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));


            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.UpdateDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.UpdateDate.Date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.SizeName.Contains(sizeCode));
            }

            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(s => s.BarCode.Contains(barCode));
            }

            Count = query.Count();
            if (Count > 0)
            {
                Qty =(decimal) query.Sum(s => s.Qty);
            }


            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatJackWolfSkinMeterialINVB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询狼爪所有的提报库存
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatJackWolfSkinMeterialINVB> QueryGatJackWolfSkinMatInvsQyeryable(string shopCode, string startBillTime, string endBillTime, string materialCode, string sizeCode, string barCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            if (!string.IsNullOrWhiteSpace(startBillTime))
            {
                DateTime dtToday = Convert.ToDateTime(startBillTime);
                query = query.Where(s => s.UpdateDate.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(endBillTime))
            {
                DateTime dtendToday = Convert.ToDateTime(endBillTime);
                query = query.Where(e => e.UpdateDate.Date <= dtendToday);
            }

            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(s => s.MaterialCode.Contains(materialCode));
            }
            if (!string.IsNullOrWhiteSpace(sizeCode))
            {
                query = query.Where(s => s.SizeName.Contains(sizeCode));
            }

            if (!string.IsNullOrWhiteSpace(barCode))
            {
                query = query.Where(s => s.BarCode.Contains(barCode));
            }
            return query
                   .OrderBy(m => m.ShopCode)
                   .Select(g => new GatJackWolfSkinMeterialINVB(g));
        }


        /// <summary>
        /// 查询狼爪所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatJackWolfSkinMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 狼爪销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatJackWolfSkinPosSaleB> QueryGatJackWolfSkinPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatJackWolfSkinPosSaleB(g))
                   .ToList();
        }
        public decimal[] QueryJackWolfSkinGatTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSale>());

            int ICount = query.Count();
            decimal Qty = 0;
            decimal prices = 0;
            if (ICount > 0)
            {
                 Qty =(decimal) query.Select(s => s.Qty).Sum();
                 prices = query.Select(s => s.BalaPrice).Sum();
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }





       
        /// <summary> 
        /// 获取库存中转数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatJackWolfSkinMeterialINVB> QueryJackWolfSkinStockTransfer(int pageIndex, int pageSize, string shopCode,out int Count, out decimal Qty)
        {
            Count = 0;
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinMeterialTransfer>());
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
                   .Select(g => new GatJackWolfSkinMeterialINVB(g))
                   .ToList(); 
        }

        public int GetJackWolfSkinStockTransferTotalCount(string shopCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinMeterialTransfer>());
            return query.Count();
        }

        /// <summary>
        /// 库存中转数据全部删除
        /// </summary>
        /// <returns></returns>
        public HandleArgs TruncateJackWolfSkinStockTransfer()
        { 
            HandleArgs HANDLE = new HandleArgs();
            string command = @" Truncate table Gat_JackWolfSkin_Meterial_Transfer";
            HANDLE = _Service.ExecuteCommand(command); 
            return HANDLE;
        }

        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns>
        public HandleArgs SaveJackWolfSkinStockTransfer(string strSolutionId)
        {
            //            HandleArgs HANDLE = new HandleArgs();
            //            string command = $@"declare @CompanyName NVARCHAR(20),@CardID NVARCHAR(20),@SolutionId int
            //select @CompanyName = CompanyName,@CardID = CardID,@SolutionId = id FROM Gat_Solution a,Bas_Company b where a.CompanyID = b.CompanyID and a.id='{strSolutionId}';
            //            delete a from Gat_JackWolfSkin_MeterialINV a,Gat_JackWolfSkin_Meterial_Transfer b where a.shopcode = b.shopcode and a.barcode = b.barcode;
            //            insert into Gat_JackWolfSkin_MeterialINV(SolutionId, ShopCode, ShopName, UpdateDate, BarCode, MaterialCode, SizeName, Qty, CompanyID, CompanyName, CardID, CardName, IsImport)
            //select @SolutionId, ShopCode, ShopName, UpdateDate, BarCode, MaterialCode, SizeName, Qty, CompanyID, @CompanyName, @CardID, CardName,1 IsImport from Gat_JackWolfSkin_Meterial_Transfer";
            //            HANDLE = _Service.ExecuteCommand(command);
            //            return HANDLE;

            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(strSolutionId))
            {
                if (!HasDangerousContents(strSolutionId))
                {
                    string command = @" EXEC Gat_JackWolfSkin_Transfer_Pos @SolutionId={0},@TransIDstr= '',@Type= 0,@OperationType= 'Stock' ";
                    command = string.Format(command, strSolutionId);
                     HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

       
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyJackWolfSkinStockTransfer(DataTable dt)
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
                       "dbo.Gat_JackWolfSkin_Meterial_Transfer";
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


        public HandleArgs TransferDelJackWolfSkin(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_JackWolfSkin_PosSale_Transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }


        public HandleArgs TransferJackWolfSkinSaveAs(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            { 
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_JackWolfSkin_Transfer_Pos @SolutionId=0,@TransIDstr= '{0}' ,@Type= 0,@OperationType= 'Pos' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
         

        public HandleArgs UpdateJackWolfSkinTransferQty(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_JackWolfSkin_PosSale_Transfer SET Qty=Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs DeleteJackWolfSkinTransfer(string TransIDstr)
        { 
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_JackWolfSkin_PosSale_Transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }


        public HandleArgs BulkCopyJackWolfSkinTransfer(DataTable dt)
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
                       "dbo.Gat_JackWolfSkin_PosSale_Transfer";
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
        /// 报错全部中转数据到销售数据中
        /// </summary>
        /// <returns></returns> 
        public HandleArgs TransferJackWolfSkinSaveAs(int solutionId)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC Gat_JackWolfSkin_Transfer_Pos '{solutionId}','' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }
        /// <summary>
        /// 删除全部销售中转数据
        /// </summary>
        /// <returns></returns>
        public HandleArgs DeleteJackWolfSkinTransfer(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_JackWolfSkin_PosSale_Transfer WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        } 


        public Dictionary<string, object> SelectJackWolfSkinPosSaleTransferPageList(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatJackWolfSkinPosSaleTransferB> posSalelist = new List<GatJackWolfSkinPosSaleTransferB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatJackWolfSkinPosSaleTransfer>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
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

            var totalCount = list.Count();

            posSalelist = list
                        .Select(g => new GatJackWolfSkinPosSaleTransferB(g))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            var total = list.ToList();
            decimal? qty = 0;
            decimal? amount = 0;
            if (totalCount > 0)
            {
                qty = list.Select(s => s.Qty)?.Sum();
                amount = list.Select(s => s.SaleAmount ?? 0m)?.Sum();
            }
            result.Add("data", posSalelist);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }


        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param> 
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteJackWolfSkinTransfer(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
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
            string command = $" DELETE Gat_JackWolfSkin_PosSale_Transfer WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        #endregion
    }
}

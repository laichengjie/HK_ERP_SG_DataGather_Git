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
        /// 查询Columbia所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatColumbiaMeterialINVB> QueryGatColumbiaMatInvs(int currentPage, int itemsPerPage, out int Count, out int Qty, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaMeterialINV>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            Count = 0;
            Qty = 0;
            Count = query.Count();
            if (Count>0)
            {
                Qty = (int) query.Sum(m => m.Qty);
            }
            return query
                   .OrderBy(m => m.ShopCode) 
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatColumbiaMeterialINVB(g))
                   .ToList();
        }
        /// <summary>
        /// 查询Columbia所有的提报库存
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>

        public List<GatColumbiaMeterialINVB> QueryGatColumbiaMatInvsData(string start_date, string end_date, string shopName,string shopcode,out int Count, out int Qty)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaMeterialINV>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopName)) query = query.Where(g => g.ShopName == shopName);
            if (!string.IsNullOrEmpty(shopcode)) query = query.Where(g => g.ShopCode == shopcode);
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
                   .Select(g => new GatColumbiaMeterialINVB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询Columbia所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatColumbiaMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaMeterialINV>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        /// <summary>
        /// 查询Columbia所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatColumbiaMatInvSumCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaMeterialINV>());
            query = query.Where(g => g.Qty > 0);
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return Convert.ToInt32(query.Sum(m => m.Qty));
        }
        /// <summary>
        /// Columbia销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatColumbiaPosSaleB> QueryGatColumbiaPosSales(int solutionID, int currentPage, int itemsPerPage, string start_date, string end_date, string shopCode ,string shopName, string materialCode , string barCode, string sizeCode ,out int Qty,out decimal Amount, out int Count)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSale>());
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
                Qty =(int)(query.Sum(i => i.Qty));
                Amount =(decimal) query.Sum(i => i.SalesAmount);
            }

            return query
                  .OrderByDescending(p => p.BillTime)
                 .ThenBy(p => p.ShopCode)
                 .ThenBy(p => p.BillNo)
                 .ThenBy(p => p.MaterialCode)
                 .Skip((currentPage - 1) * itemsPerPage)
                 .Take(itemsPerPage)
                 .Select(g => new GatColumbiaPosSaleB(g))
                 .ToList();


        }
        public List<GatColumbiaPosSaleB> QueryGatColumbiaPosSalesData(int solutionID, string start_date, string end_date, string shopName)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSale>());
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
                .Select(g => new GatColumbiaPosSaleB(g))
                .ToList();
        }


        /// <summary>
        /// Columbia销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatColumbiaPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }
        /// <summary>
        /// Columbia销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatColumbiaPosSaleB> GatColumbiaPosSaleSumCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Select(g => new GatColumbiaPosSaleB(g)).ToList();
        }
        /// <summary>
        /// 中转数据（Columbia的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatColumbiaPosSaletransferB> QueryGatColumbiaPosSaleTransferDate(int solutionID, string shopName, string start_date, string end_date)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSaletransfer>());
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
            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Select(g => new GatColumbiaPosSaletransferB(g))
                   .ToList();
        }
        /// <summary>
        /// 中转数据（Columbia的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatColumbiaPosSaletransferB> QueryGatColumbiaPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage, string shopName, string start_date, string end_date)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSaletransfer>());
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

            return query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatColumbiaPosSaletransferB(g))
                   .ToList();
        }

        public HandleArgs UpdateTransferQty(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Columbia_PosSale_transfer SET SalesAmount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        public HandleArgs InsertTransferQty(DataTable dt, string ddlSoule, string companyID)
        {
            List<GatColumbiaPosSaletransfer> list = Utils.ConvertHelper.DataTableToList<GatColumbiaPosSaletransfer>(dt);
            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                item.SolutionID = int.Parse(ddlSoule);
                item.BillTime = DateTime.Now;
                item.CompanyID = companyID;
                item.CardID = "";
                item.ShopName = "";
                item.StockName = "";
                handle = AddTransfer(item);
            }
            return handle;
        }
        public HandleArgs DeleteTransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_Columbia_PosSale_transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }
        public HandleArgs DeleteTransfer()
        {
            HandleArgs HANDLE = new HandleArgs();

            string command = @" DELETE Gat_Columbia_PosSale_transfer";
            HANDLE = _Service.ExecuteCommand(command);

            return HANDLE;
        }

        public HandleArgs AddTransfer(GatColumbiaPosSaletransfer entity)
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
        public HandleArgs TransferDelAs(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 安迪 全渠道銷售 中转数据
        /// </summary>
        /// <param name="TransIdstr"></param>
        /// <returns></returns>
        public HandleArgs TransferDelAs_OC(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr)) 
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary> 
        /// 安迪 全渠道銷售 中转数据
        /// </summary>
        /// <param name="TransIdstr"></param>
        /// <returns></returns>
        public HandleArgs TransferDelAs_Move_OC(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Ad_Move_Transfer_OC WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        /// <summary>
        ///阿迪智慧零售 中转数据
        /// </summary>
        /// <param name="TransIdstr"></param>
        /// <returns></returns>
        public HandleArgs TransferDelAs_Wif(string TransIdstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIdstr))
            {
                if (!HasDangerousContents(TransIdstr))
                {
                    string command = @" DELETE Gat_Ad_PosSale_transfer_Wif WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIdstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }
        public HandleArgs TransferSaveAs(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Columbia_transferAs_Pos '{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 中转数据总条数（Columbia的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatColumbiaPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSaletransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        /// <summary>
        /// 中转数据总条数（Columbia的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public List<GatColumbiaPosSaletransferB> GatColumbiaPosSaleTransfeSumCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSaletransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Select(g => new GatColumbiaPosSaletransferB(g)).ToList();
        }
        /// <summary>
        /// 中转数据总条数（Adidas的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatAdPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }
        /// <summary>
        /// 中转数据总条数（Adidas的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param> 
        /// <returns></returns>
        public int GatAdPosSaleTransferCount_OC(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer_OC>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        /// <summary>
        /// 中转数据总条数（Adidas的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>  
        /// <returns></returns>
        public int GatAdMoveTransferCount_OC(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdMoveTransfer_OC>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }
        /// <summary>
        /// 中转数据总条数（Adidas的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param> 
        /// <returns></returns>
        public int GatAdPosSaleTransferCount_Wif(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAdPosSaletransfer_Wif>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }


        /// <summary>
        /// 亚瑟士 库存
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<GatAsicsMaterialB> QueryGatAsicsINV(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAsicsMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatAsicsMaterialB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询所有符合条件的中转数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IQueryable<GatColumbiaPosSaletransferB> QueryGatColumbiaPosSaleTransferData(int solutionID, string shopName, string startDate, string endDate)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSaletransfer>());
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
                   .Select(g => new GatColumbiaPosSaletransferB(g));
        }

        /// <summary>
        /// 获取columbia销售数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public IQueryable<GatColumbiaPosSaleB> QueryGatColumbiaPosSalesQueryable(int solutionID, string start_date, string end_date, string shopName)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaPosSale>());
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
            .Select(g => new GatColumbiaPosSaleB(g));
        }
        public HandleArgs DeleteColumbiaPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate ,string  shopCode, string shopName, string materialCode, string barCode, string sizeCode)
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
            string command = $" DELETE Gat_Columbia_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteColumbiaPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Columbia_PosSale WHERE solutionId='{solutionId}'" + where;
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
        public List<GatColumbiaMeterialINVB> QueryColumbiaStockTransfer(int pageIndex, int pageSize, string shopCode, out int Count, out decimal Qty)
        {
            Count = 0; 
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatColumbiaMeterialTransfer>());
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
                   .Select(g => new GatColumbiaMeterialINVB(g))
                   .ToList();
        }

        /// <summary> 
        /// 库存中转数据全部删除
        /// </summary>
        /// <returns></returns>
        public HandleArgs TruncateColumbiaStockTransfer()
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = @" Truncate table Gat_Columbia_Meterial_Transfer";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 保存数据到库存表
        /// </summary>
        /// <returns></returns>
        public HandleArgs SaveColumbiaStockTransfer(string strSolutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(strSolutionId))
            {
                if (!HasDangerousContents(strSolutionId))
                {
                    string command = @" EXEC sp_SGIS_Columbia_GetMaterailINV @SolutionId={0},@TransIDstr= '',@Type= 0,@OperationType= 'Stock' ";
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
        public HandleArgs BulkCopyColumbiaStockTransfer(DataTable dt)
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
                       "dbo.Gat_Columbia_Meterial_Transfer";
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

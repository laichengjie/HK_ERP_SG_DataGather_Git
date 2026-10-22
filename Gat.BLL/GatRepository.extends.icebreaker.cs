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
        /// IceBreaker销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatIceBreakerPosSaleB> QueryGatIceBreakerPosSales(int solutionID, int currentPage, int itemsPerPage, string start_date, string end_date, string shopCode, string shopName, string materialCode, string barCode, string billNo, out int Qty, out decimal Amount, out int Count)
        {
            List<GatIceBreakerPosSaleB> Poslist = new List<GatIceBreakerPosSaleB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatIceBreakerPosSale>());
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
                 .Select(g => new GatIceBreakerPosSaleB(g))
                 .ToList();

            return Poslist;


        }


        /// <summary>
        /// 获取IceBreaker销售数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="shopName"></param>
        /// <returns></returns>
        public IQueryable<GatIceBreakerPosSaleB> QueryGatIceBreakerPosSalesQueryable(int solutionID, string start_date, string end_date, string shopName)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIceBreakerPosSale>());
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
            .Select(g => new GatIceBreakerPosSaleB(g));
        }



        public HandleArgs DeleteIceBreakerPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string shopCode, string shopName, string materialCode, string barCode, string sizeCode)
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
            string command = $" DELETE Gat_IceBreaker_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }


        /// <summary>
        /// 通过ID删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteIceBreakerPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_IceBreaker_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        #endregion

        #region 库存
        /// <summary>
        ///  查询IceBreaker所有的提报库存 
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
        public List<GatIceBreakerMeterialINVB> QueryGatIceBreakerMatInvs(int solutionID, string shopCode, string materialCode, string sizeCode, int currentPage, int itemsPerPage, ref int totalCount, ref decimal totalQty, bool isNeedPage = true)
        {
            totalQty = 0;
            totalCount = 0;
            List<GatIceBreakerMeterialINVB> Invlist = new List<GatIceBreakerMeterialINVB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatIceBreakerMeterialINV>());
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
                   .Select(g => new GatIceBreakerMeterialINVB(g))
                   .ToList();
            }
            else
            {
                Invlist = query.OrderBy(m => m.ShopCode).Select(g => new GatIceBreakerMeterialINVB(g)).ToList();
            }

            return Invlist;

        }

        #endregion


        #region 中转POS单
        /// <summary>
        ///  中转POS单 保存到正式pos
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs IceBreakerTransferSaveAs(int solutionId,string TransIDstr)
        { 
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC sp_SGIS_GaIceBreakertransferAsPos '{0}','{1}' ";
                    command = string.Format(command, solutionId, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            else
            {
                string command = @" EXEC sp_SGIS_GaIceBreakertransferAsPos '{0}','' ";
                command = string.Format(command, solutionId);
                HandleArgs HANDLE = _Service.ExecuteCommand(command);
            }

            return handle;
        }


        /// <summary>
        /// 修改中转POS单 数量
        /// </summary>
        /// <param name="Qty"></param>
        /// <param name="TransID"></param>
        /// <returns></returns>
        public HandleArgs UpdateIceBreakerTransferQty(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_IceBreaker_PosSale_transfer SET Qty={0} WHERE TransID={1} ";
            command = string.Format(command, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除中转POS单（按ID删除）
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs DeleteIceBreakerTransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" DELETE Gat_IceBreaker_PosSale_transfer WHERE TransID IN ({0}) ";
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

        /// <summary>
        /// 删除 所有中转POS单
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public HandleArgs DeleteIceBreakerTransferAll(int solutionId)
        {
            HandleArgs HANDLE = new HandleArgs();
            string command = @" DELETE Gat_IceBreaker_PosSale_transfer WHERE solutionId={0} ";
            command = string.Format(command, solutionId);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary> 
        /// 删除符合条件  中转POS单
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name=""></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteIceBreakerPosSaletransfer(int solutionId, DateTime startBillDate, DateTime endBillDate, string materialCode, string sizeCode, string shopCode)
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
            string command = $" DELETE Gat_IceBreaker_PosSale_transfer WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 查询 中转POS单
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="shopName"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <returns></returns>
        public Dictionary<string, object> QueryGatIceBreakerPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage, string shopName, string start_date, string end_date)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatIceBreakerPosSaletransfer>());
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

            var totalCount = query.Count();
            var qty = query.Sum(s => s.Qty);
            var amount = query.Sum(s => s.PayAmount);

            if (qty == null)
            {
                qty = 0;
            }
            var list= query
                    .OrderByDescending(p => p.TransID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatIceBreakerPosSaletransferB(g))
                   .ToList();



            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty);
            result.Add("amount", amount ?? 0);
            return result;
        }


        /// <summary>
        /// 导入 中转POS单
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ddlSoule"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public HandleArgs InsertIceBreakerTransferQty(DataTable dt, string ddlSoule, string companyID)
        {
            List<GatIceBreakerPosSaletransfer> list = Utils.ConvertHelper.DataTableToList<GatIceBreakerPosSaletransfer>(dt);
            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                item.SolutionID = int.Parse(ddlSoule);
                item.BillTime = DateTime.Now;
                item.CompanyID = companyID;
                //item.CardID = "";
                //item.ShopName = "";
                //item.StockName = "";
                handle = AddTransfer(item);
            }
            return handle;
        }
        public HandleArgs AddTransfer(GatIceBreakerPosSaletransfer entity)
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
        #endregion


        /// <summary> 
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyIceBreakerStockTransfer(DataTable dt)
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
                       "dbo.Gat_IceBreaker_MaterialInv_Transfer"; 
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
        /// 保存数据到库存表
        /// </summary> 
        /// <returns></returns>
        public HandleArgs SaveIceBreakerStockTransfer(string strSolutionId , int Type )
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(strSolutionId))
            {
                if (!HasDangerousContents(strSolutionId))
                {
                    //string command = @" EXEC sp_SGIS_Columbia_GetMaterailINV @SolutionId={0},@TransIDstr= '',@Type= 0,@OperationType= 'Stock' ";
                    string command = @" EXEC sp_SGIS_IceBreaker_GetMaterailINV_Transfer @SolutionId={0},@TransIDstr= '',@Type= {1},@OperationType= 'Stock' ";
                    command = string.Format(command, strSolutionId, Type);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        } 

        public List<GatIceBreakerMeterialINVB> QueryIceBreakerStockTransfer(int pageIndex, int pageSize, string shopCode, out int Count, out decimal Qty)
        {
            Count = 0;
            Qty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatIceBreakerMeterialTransfer>());
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where(s => s.ShopCode == shopCode);
            }
            Count = query.Count();
            if (Count > 0)
            {
                Qty = (decimal)query.Sum(s => s.ActualQty);
            }
            return query
                   .Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                   .Select(g => new GatIceBreakerMeterialINVB(g))
                   .ToList();
        }

    }
}

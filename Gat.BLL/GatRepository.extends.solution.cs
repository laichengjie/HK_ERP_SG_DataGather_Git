using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gat.BLL.Modules;
using Gat.Utils;
using Gat.DAL;
using System.Data;
using System.Data.SqlClient;

namespace Gat.BLL
{
    public partial class GatRepository : IDisposable
    {
        /// <summary>
        /// 获取不上传货品范围
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatMaterialRangeB> GetMaterialRanges(int solutionId)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMaterialRange>()).Where(s => s.SolutionId == solutionId);
            return query.Select(g => new GatMaterialRangeB(g)).ToList();
        }

        /// <summary>
        /// 获取货品折扣
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatMaterialDiscountB> GetMaterialDiscount(int solutionId)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMaterialDiscount>()).Where(s => s.SolutionId == solutionId);
            return query.Select(g => new GatMaterialDiscountB(g)).ToList();
        }

        /// <summary> 
        /// 删除不上传货品范围 
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteMaterialRange(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_MaterialRange WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 删除货品折扣
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteMaterialDiscount(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_MaterialDiscount WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 获取特殊货品范围
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatSpecialMaterialRangeB> GetSpecialMaterialRanges(int solutionId)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatSpecialMaterialRange>()).Where(s => s.SolutionId == solutionId);
            return query.Select(g => new GatSpecialMaterialRangeB(g)).ToList();
        }

        /// <summary>
        /// 删除特殊货品范围
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteSpecialMaterialRange(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_SpecialMaterialRange WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyMaterialRange(DataTable dt)
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
                       "dbo.Gat_MaterialRange";
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
        public HandleArgs BulkCopyMaterialDiscount(DataTable dt)
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
                       "dbo.Gat_MaterialDiscount";
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
     
        public void DeleteAllMaterialRange(int solutionId)
        {
            string command = $" delete Gat_MaterialRange WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            _Service.ExecuteCommand(command);
        }

        public void DeleteAllMaterialDiscount(int solutionId,string MaterialCode)
        {
            string command = "";
            if (string.IsNullOrEmpty(MaterialCode))
            {
                command = $" delete Gat_MaterialDiscount WHERE solutionId='{solutionId}'";
            }
            else
            {
                command = $" delete Gat_MaterialDiscount WHERE solutionId='{solutionId}' and MaterialCode in ({MaterialCode})   ";
            }
            command = string.Format(command);
            _Service.ExecuteCommand(command);
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopySpecialMaterialRange(DataTable dt)
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
                       "dbo.Gat_SpecialMaterialRange";
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
        
        public void DeleteAllSpecialMaterialRange(int solutionId)
        {
            string command = $" delete Gat_SpecialMaterialRange WHERE solutionId='{solutionId}'";
            command = string.Format(command);
            _Service.ExecuteCommand(command);
        }

        public string GetExistsSpecialMaterialRangeMaterialCode(string materialCode, int solutionId)
        {
            string data = "";
            var sql = $"select distinct materialcode from gat_specialmaterialrange where solutionId={solutionId} and materialcode in({materialCode})";
            var conn = new SqlConnection(_Service.ConnectString);
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data+=reader.GetString(0)+",";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader!=null)
                {
                    reader.Close();
                }
                conn.Close();
            }
            if (data.Length>1)
            {
                data = data.Substring(0, data.Length - 1);
            }
            return data;
        }

        public string GetExistsMaterialRangeMaterialCode(string materialCode, int solutionId)
        {
            string data = "";
            var sql = $"select distinct materialcode from Gat_MaterialRange where solutionId={solutionId} and materialcode in({materialCode})";
            var conn = new SqlConnection(_Service.ConnectString);
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data += reader.GetString(0) + ",";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
            if (data.Length > 1)
            {
                data = data.Substring(0, data.Length - 1);
            }
            return data;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateMaterialRange(List<GatMaterialRangeB> updates)
        {
            string command = string.Empty;
            foreach (var item in updates)
            {
                command += $@"Update Gat_MaterialRange set MaterialCode='{item.MaterialCode}',MaterialName='{item.MaterialName}',ModifyTime=getdate()
                        Where Id={item.Id};" + Environment.NewLine;
            }
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 更新 货号折扣
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateMaterialDiscount(List<GatMaterialDiscountB> updates)
        {
            string command = string.Empty;
            foreach (var item in updates)
            {
                command += $@"Update Gat_MaterialDiscount set MaterialCode='{item.MaterialCode}',MaterialName='{item.MaterialName}',Discount={item.Discount},ModifyTime=getdate()
                        Where Id={item.Id};" + Environment.NewLine;
            }
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateSpecialMaterialRange(List<GatSpecialMaterialRangeB> updates)
        {
            string command = string.Empty;
            foreach (var item in updates)
            {
                command += $@"Update Gat_SpecialMaterialRange set MaterialCode='{item.MaterialCode}',MaterialName='{item.MaterialName}',ShopCode='{item.ShopCode}',ShopName='{item.ShopName}',ModifyTime=getdate()
                        Where Id={item.Id};" + Environment.NewLine;
            }
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        #region 仓库子款型设置
        /// <summary>
        /// 更新 仓库子款型
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateStockSubModelSet(List<GatStockSubModelSetB> updates)
        {
            string command = string.Empty;
            foreach (var item in updates)
            {
                command += $@"Update Gat_StockSubModelSet set StockCode='{item.StockCode}',SubModel='{item.SubModel}',PCode='{item.PCode}',ModifyTime=getdate()
                        Where Id={item.Id};" + Environment.NewLine;
            }
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 获取仓库子款型
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatStockSubModelSetB> GetStockSubModelSet(int solutionId)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatStockSubModelSet>()).Where(s => s.SolutionId == solutionId);
            return query.Select(g => new GatStockSubModelSetB(g)).ToList();
        }

        /// <summary>
        /// 删除仓库子款型设置
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteStockSubModelSet(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_StockSubModelSet WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary> 
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs BulkCopyStockSubModelSet(DataTable dt)
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
                       "dbo.Gat_StockSubModelSet";
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


        public void DeleteAllStockSubModelSet(int solutionId, string StockCode)
        {
            string command = "";
            if (string.IsNullOrEmpty(StockCode))
            {
                command = $" delete Gat_StockSubModelSet WHERE solutionId='{solutionId}'";
            }
            else
            {
                command = $" delete Gat_StockSubModelSet WHERE solutionId='{solutionId}' and StockCode in ({StockCode})   ";
            }
            command = string.Format(command);
            _Service.ExecuteCommand(command);
        }
        #endregion


        /// <summary>
        /// 获取网店归属线下店铺POS设置
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatOnlineShopSetB> GatOnlineShopSet(int solutionId)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatOnlineShopSet>()).Where(s => s.SolutionId == solutionId);
            return query.Select(g => new GatOnlineShopSetB(g)).ToList();
        } 
        /// <summary>
        /// 删除网店归属线下店铺POS设置   
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteOnlineShopSet(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_OnlineShopSet WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary>
        /// 更新 网店归属线下店铺POS设置
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateOnlineShopSet(List<GatOnlineShopSetB> updates)
        {
            string command = string.Empty;
            foreach (var item in updates)
            {
                command += $@"Update Gat_OnlineShopSet set OnlineShopCode='{item.OnlineShopCode}',OfflineShopCode='{item.OfflineShopCode}',ModifyTime=getdate()
                        Where Id={item.Id};" + Environment.NewLine;
            }
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary> 
        /// 导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HandleArgs OnlineShopSetImport(DataTable dt)
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
                       "dbo.Gat_OnlineShopSet";
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
    }
}

using Gat.BLL.ControllerModel.Response;
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerBll
{
    public class BrandBll
    {
        #region 对外接口查询
        /// <summary>
        /// 获取销售
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="vendorId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<GetSales> GetSales(string connString, DateTime startDateTime, DateTime endDateTime, int vendorId, string companyId)
        {
            List<GetSales> data = new List<GetSales>();

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@VendorId",vendorId),
                new SqlParameter("@companyId",companyId),
                new SqlParameter("@startTime",startDateTime),
                new SqlParameter("@endTime",endDateTime)
            };

            SqlDataAdapter da = null;
            var conn = new SqlConnection(connString);
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "sp_SGIS_LiNing_QuerySales";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = new SqlConnection(connString);
                    command.CommandTimeout = 600;
                    command.Parameters.AddRange(paras);

                    da = new SqlDataAdapter();
                    da.SelectCommand = command;

                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    data = ConvertHelper.DataTableToList<GetSales>(ds.Tables[0]);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// 获取库存
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<GetStocks> GetStocks(string connString, string shopCode, string materialCode, int vendorId, string companyId)
        {
            List<GetStocks> data = new List<GetStocks>();

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@VendorId",vendorId),
                new SqlParameter("@companyId",companyId),
                new SqlParameter("@shopCode",shopCode),
                new SqlParameter("@materialCode",materialCode)
            };

            SqlDataAdapter da = null;
            var conn = new SqlConnection(connString);
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "sp_SGIS_LiNing_QueryStocks";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = conn;
                    command.Parameters.AddRange(paras);

                    da = new SqlDataAdapter();
                    da.SelectCommand = command;

                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    data = ConvertHelper.DataTableToList<GetStocks>(ds.Tables[0]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return data;
        }
        #endregion
    }
}

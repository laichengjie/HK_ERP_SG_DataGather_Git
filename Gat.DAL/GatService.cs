using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.Utils;
using System.Configuration;

namespace Gat.DAL
{
    /// <summary>
    /// 数据提报服务层 
    /// </summary>
    public class GatService : ServiceBase
    {

        #region 构造函数

        /// <summary>
        /// 默认WEB正式库
        /// </summary>
        public GatService()
            : base() { }

        /// <summary>
        /// 指定的运行环境
        /// </summary>
        /// <param name="connType"></param>
        public GatService(ConnnectType connType)
            : base(connType) { }

        #endregion

        #region ERP存储过程

        /// <summary>
        /// 数据同步
        /// </summary>
        /// <param name="vendorID">供应商ID </param>
        /// <returns></returns>
        public HandleArgs DataSynchron(int vendorID)
        {
            GatDataContext context = this.Context as GatDataContext;
            if (context == null) return new HandleArgs { Status = HandleStatus.Fail, Message = "将数据上下文转换为 GatDataContext 时失败！" };

            ReturnModule module = null;
            HandleArgs handle = null;
            string companyDB = ConfigurationManager.AppSettings["CompanyDB"].ToString();
            System.Data.Linq.ISingleResult<ReturnModule> results = null;
            try
            {
                results = context.DataSynchron(vendorID,companyDB);
                if (results != null)
                {
                    module = results.FirstOrDefault<ReturnModule>();
                    if (module != null)
                    {
                        handle = new HandleArgs();
                        if (module.Code == "1")
                        {
                            handle.Status = HandleStatus.Completed;
                            handle.Message = "同步数据成功！";
                        }
                        else
                        {
                            handle.Status = HandleStatus.Fail;
                            handle.Message = module.Message;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                handle = new HandleArgs();
                handle.Status = HandleStatus.Fail;
                handle.Message = ex.Message;
                handle.Exception = null;
            }

            if (handle == null)
            {
                handle = new HandleArgs();
                handle.Status = HandleStatus.Fail;
                handle.Message = "数据同步错误，原因：未知。";
            }

            return handle;
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        /// <param name="procName">执行的采集过程名称</param>
        /// <param name="vendorCode">品牌供应商代码</param>
        /// <param name="gatType">采集类型标志，1：正常采集；0：补采</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间，当采集类型为补采时，开始和结束时间方有效</param>
        /// <returns></returns>
        public HandleArgs ExecBatch(string vendorCode, int gatType,string companyID,string cardID, DateTime startTime, DateTime endTime)//(string procName, string  vendorCode, int gatType, DateTime startTime, DateTime endTime)
        {
            GatDataContext context = this.Context as GatDataContext;
            if (context == null) return new HandleArgs { Status = HandleStatus.Fail, Message = "将数据上下文转换为 GatDataContext 时失败！" };


            ReturnModule module = null;
            HandleArgs handle = null;
            System.Data.Linq.ISingleResult<ReturnModule> results = null;
            try
            {
                results = context.ExecBatch(vendorCode, gatType, companyID, cardID, startTime, endTime);
                if (results != null)
                {
                    module = results.FirstOrDefault<ReturnModule>();
                    if (module != null)
                    {
                        handle = new HandleArgs();
                        if (module.Code == "1")
                        {
                            handle.Status = HandleStatus.Completed;
                            handle.Message = "补采成功！";
                        }
                        else
                        {
                            handle.Status = HandleStatus.Fail;
                            handle.Message = module.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                handle = new HandleArgs();
                handle.Status = HandleStatus.Fail;
                handle.Message = ex.Message;
                handle.Exception = null;
            }

            if (handle == null)
            {
                handle = new HandleArgs();
                handle.Status = HandleStatus.Fail;
                handle.Message = "数据同步错误，原因：未知。";
            }

            return handle;
        }

        #endregion
    }
}

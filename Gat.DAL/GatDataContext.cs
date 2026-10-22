using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.ComponentModel;
using Gat.Utils;

namespace Gat.DAL
{
    /// <summary>
    /// 数据提报上下文，分部类
    /// </summary>
    public partial class GatDataContext : DataContextBase
    {
        #region 私有属性

        ConnnectType _connType; //连接类型
        //string _procName;       //将要执行的ERP中的存储过程

        #endregion

        #region 扩展构造函数

        /// <summary>
        /// 指定运行环境的初始化方式
        /// </summary>
        /// <param name="connType"></param>
        public GatDataContext(ConnnectType connType)
            : base(connType)
        {
            _connType = connType;            
        }
        
        #endregion

        #region ERP

        /// 注：当连接到ERP时，以下方法才可用 ===============

        /// <summary>
        /// 同步数据到服务平台
        /// </summary>
        /// <param name="vendorID">供应商ID</param>
        /// <returns></returns>
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.sp_SGIS_DataSynchron")]
        public ISingleResult<ReturnModule> DataSynchron(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "VendorID", DbType = "Int")] 
            System.Nullable<int> vendorID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "Company", DbType = "Nvarchar")]
             string companyDB)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), vendorID,companyDB);
            return ((ISingleResult<Gat.DAL.ReturnModule>)(result.ReturnValue));
        }

        /// <summary>
        /// 批量提取数据
        /// </summary>
        /// <param name="vendorCodde">供应商ID</param>
        /// <returns></returns>
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.sp_SGIS_ExecBatch")]
        public ISingleResult<ReturnModule> ExecBatch(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "VendorCode", DbType = "Nvarchar")] 
            string vendorCodde,
            
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GatType", DbType = "Int")] 
            System.Nullable<int> gatType,

            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CompanyID", DbType = "Nvarchar")] 
            string companyID,

            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CardID", DbType = "Nvarchar")] 
            string cardID,

            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "StartTime", DbType = "DateTime")] 
            System.Nullable<DateTime> startTime,
            
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "endTime", DbType = "DateTime")] 
            System.Nullable<DateTime> endTime)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), vendorCodde, gatType,companyID,cardID, startTime, endTime);
            return ((ISingleResult<Gat.DAL.ReturnModule>)(result.ReturnValue));
                //@VendorCode NVARCHAR(50), --供应商代码
                //@GatType Int = 1, --采集类型，1：正常采集；0：补采
                //@CompanyID NVARCHAR(20) = '',--公司ID ，补采的时候使用
                //@CardID NVARCHAR(20) = '',-- 品牌ID  ，补采的时候使用
                //@StartTime  DateTime , -- 补采时开始时间 ，补采的时候使用
                //@EndTime DateTime  --补采结束时间 ，补采的时候使用
        }

        

        #endregion
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Blls
{
    public class BaseBll
    {
        public SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.SqlServer,
            ConnectionString = ConfigurationManager.ConnectionStrings["WEBDB"].ToString(),
            IsAutoCloseConnection = true
        },
         db =>
         {
             //如果用单例配置要统一写在这儿
             db.Aop.OnLogExecuting = (s, p) =>
             {

             };

         });

        public SqlSugarScope ErpDb = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.SqlServer,
            ConnectionString = ConfigurationManager.ConnectionStrings["ERPDB"].ToString(),
            IsAutoCloseConnection = true
        },
         db =>
         {
             //如果用单例配置要统一写在这儿
             db.Aop.OnLogExecuting = (s, p) =>
             {

             };

         });

        /// <summary>
        /// 极限李宁会员提报
        /// </summary>
        public SqlSugarScope LiningMemberErpDb = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.SqlServer,
            ConnectionString = ConfigurationManager.ConnectionStrings["LININGMEMBERERPDB"].ToString(),
            IsAutoCloseConnection = true
        },
         db =>
         {
             //如果用单例配置要统一写在这儿
             db.Aop.OnLogExecuting = (s, p) =>
             {

             };

         });

        /// <summary>
        /// ERP提报库提报
        /// </summary>
        public SqlSugarScope ExtractorDB = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.SqlServer,
            ConnectionString = ConfigurationManager.ConnectionStrings["EXTRACTORDB"].ToString(),
            IsAutoCloseConnection = true
        },
         db =>
         {
             //如果用单例配置要统一写在这儿
             db.Aop.OnLogExecuting = (s, p) =>
             {

             };

         });
    }
}

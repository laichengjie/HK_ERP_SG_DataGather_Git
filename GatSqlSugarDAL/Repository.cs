using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace GatSqlSugarDAL
{
    /***这里面写的代码不会给覆盖,如果要重新生成请删除 Repository.cs ***/

    /// <summary>
    /// 仓储模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : SimpleClient<T> where T : class, new()
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public Repository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
        {
            if (context == null)
            {
                base.Context = Db;
            }
        }

        /// <summary>
        /// SqlSugarScope操作数据库是线程安的可以单例
        /// </summary>
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.SqlServer,
            ConnectionString = ConfigurationManager.ConnectionStrings["WEBDB"].ToString(),//@"server=192.168.100.21;uid=dev_user;pwd=hkdevdb@100.9;database=HK_ERP_SG_DataGather",
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
        /// 仓储扩展方法:单表查询通用分页 
        /// </summary>
        /// <returns></returns>
        public object CommonPage(QueryParameters pars, int pageIndex, int pagesize)
        {
            int tolCount = 0;
            var sugarParamters = pars.Parameters.Select(it => (IConditionalModel)new ConditionalModel()
            {
                ConditionalType = it.ConditionalType,
                FieldName = it.FieldName,
                FieldValue = it.FieldValue
            }).ToList();
            var query = Db.Queryable<T>();
            if (pars.OrderBys != null)
            {
                foreach (var item in pars.OrderBys)
                {
                    query.OrderBy(item.ToSqlFilter());//格式 id asc或者 id desc
                }
            }
            var result = query.Where(sugarParamters).ToPageList(1, 2, ref tolCount);
            return new
            {
                count = tolCount,
                data = result
            };
        }

        /// <summary>
        /// 仓储扩展方法:多表查询通用分页 
        /// 用法  CommonPage(db.Queryable<JoinTable1,JoinTable2>(...).Select(it=new class(){..}).MergeTable(),pars,orderbys,pageIndex,pagesize)
        /// </summary>
        /// <returns></returns>
        public object CommonPage<ViewModel>(ISugarQueryable<ViewModel> query, QueryParameters pars, int pageIndex, int pagesize)
        {
            int tolCount = 0;
            var sugarParamters = pars.Parameters.Select(it => (IConditionalModel)new ConditionalModel()
            {
                ConditionalType = it.ConditionalType,
                FieldName = it.FieldName,
                FieldValue = it.FieldValue
            }).ToList();
            if (pars.OrderBys != null)
            {
                foreach (var item in pars.OrderBys)
                {
                    query.OrderBy(item.ToSqlFilter());//格式 id asc或者 id desc
                }
            }
            var result = query.Where(sugarParamters).ToPageList(1, 2, ref tolCount);
            return new
            {
                count = tolCount,
                data = result
            };
        }


        /// <summary>
        /// 更新同时忽略部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">忽略字段集合</param>
        public bool UpdateWithIgnoreColumn(T data, params string[] columns)
        {
            var effectCount = Db.Updateable<T>(data).IgnoreColumns(columns).ExecuteCommand();
            if (effectCount > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 只更新部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">更新字段集合</param>
        /// <returns></returns>
        public bool UpdateWithColumn(T data, params string[] columns)
        {
            var effectCount = Db.Updateable<T>(data).UpdateColumns(columns).ExecuteCommand();
            if (effectCount > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新同时忽略部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">忽略字段集合</param>
        public bool BatchUpdateWithIgnoreColumn(List<T> data, params string[] columns)
        {
            var effectCount = Db.Updateable<T>(data).IgnoreColumns(columns).ExecuteCommand();
            if (effectCount > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 只更新部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">更新字段集合</param>
        /// <returns></returns>
        public bool BatchUpdateWithColumn(List<T> data, params string[] columns)
        {
            var effectCount = Db.Updateable<T>(data).UpdateColumns(columns).ExecuteCommand();
            if (effectCount > 0)
            {
                return true;
            }
            return false;
        }

    }
    /// <summary>
    /// 通用查询参数
    /// </summary>
    public class QueryParameters
    {
        public List<QueryParameter> Parameters { get; set; }
        public List<string> OrderBys { get; set; }

    }
    /// <summary>
    /// 通用查询参数
    /// </summary>
    public class QueryParameter
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public ConditionalType ConditionalType
        {
            get; set;
        }

    }
}
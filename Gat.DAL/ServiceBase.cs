using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gat.Utils;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data.Linq;

using System.Data.Common;

namespace Gat.DAL
{
    /// <summary>
    /// 数据提报服务类
    /// <remarks>1.注：BLL层一定要传递正确的T;2.事务使用TransactionExtension类来做</remarks>
    /// </summary>
    public class ServiceBase : IGatService,IDisposable
    {
        #region 公开属性

        /// <summary>
        /// 数据上下文
        /// </summary>
        internal DataContextBase Context = null;

        /// <summary>
        /// 连接环境
        /// </summary>
        public ConnnectType ConnnectType { get; private set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectString { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认WEB正式库（KEY=WEBDB）
        /// </summary>
        public ServiceBase() 
        {
            this.Context = new GatDataContext();//初始化数据上下文
            this.ConnnectType = ConnnectType.Web;
        }

        /// <summary>
        /// 指定的运行环境
        /// </summary>
        /// <param name="connType"></param>
        public ServiceBase(ConnnectType connType)
        {
            this.Context = new GatDataContext(connType);
            this.ConnectString = Context.Connection.ConnectionString;
            this.ConnnectType = connType;
        }

        #endregion

        #region IGatService成员

        /// <summary>
        /// 添加一条实体记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TTable">表映射说明</typeparam>
        /// <param name="TEntity">实体对象</param>
        /// <param name="submitChanges">true：自动保存；fase：调用SaveChanges保存</param>
        /// <returns></returns>
        public virtual HandleArgs InsertEntity<T>(T TEntity, bool submitChanges = true) where T : ModuleBase
        {
            HandleArgs args = this.ValidEntity(TEntity);
            if (args.Status == HandleStatus.Fail) return args;

            IList<string> primaryKeys = TEntity.PrimaryKeys;
            if (primaryKeys.Count == 0)
            {
                args.Status = HandleStatus.Fail;
                args.Message = string.Format("在类型【{0}】上找不到主键！", TEntity.GetType().Name);
                return args;
            }

            Regex regex = new Regex(@"IDENTITY", RegexOptions.IgnoreCase); //主键映射特性
            Type TType = TEntity.GetType() ;
            ColumnAttribute attribute = null;

            //去掉自增列的值
            PropertyInfo property = null;
            foreach (string key in primaryKeys)
            {
                property = TType.GetProperty(key);
                attribute = TEntity.FetchColumnAttribute(key);
                if (attribute != null &&
                    property != null &&
                    regex.IsMatch(attribute.DbType)) property.SetValue(TEntity, 0, null);
            }

            //添加到表
            this.Context.GetTable<T>().InsertOnSubmit(TEntity);
            if (submitChanges) return SaveChanges();
            
            
            args.Status = HandleStatus.Completed;
            args.Message = "尚未保存至数据库，请调用SaveChanges方法保存！";
            return args;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="TEntity">实体对象</param>
        /// <param name="submitChanges">true：自动保存；fase：调用SaveChanges保存</param>
        /// <returns></returns>
        public virtual HandleArgs UpdateEntity<T>(T TEntity, bool submitChanges = true) where T : ModuleBase
        {
            HandleArgs args = this.ValidEntity(TEntity);
            if (args.Status == HandleStatus.Fail) return args;

            //根据主键从数据库中查找出对应的实体
            T TTemp = this.FetchEntity(TEntity);
            if (TTemp == null)
            {
                args.Status = HandleStatus.Fail;
                args.Message = "数据库中不存在此记录！";
                return args;
            }

            //更新实体值
            ModuleBase moduleBase = TTemp as ModuleBase;
            moduleBase.CopyPropertyFrom(TEntity);

            args.Status = HandleStatus.Completed;
            args.Message = "尚未保存至数据库，请调用SaveChanges方法保存！";
            //args.Exception = null;
            return submitChanges ? this.SaveChanges() : args;
        }

        /// <summary>
        /// 批量更新记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="TEntity">实体对象</param>
        /// <param name="evaluator">更新属性的实体 </param>
        /// <returns></returns>
        public virtual HandleArgs UpdateEntity<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> evaluator) where T : ModuleBase
        {
            HandleArgs HANDLE = new HandleArgs();
            try
            {
                this.Context.GetTable<T>().UpdateEntity(predicate,evaluator);
                HANDLE.Status = HandleStatus.Completed;
            }
            catch (Exception ex)
            {

                HANDLE.Status = HandleStatus.Fail;
                HANDLE.Message = ex.Message;
                HANDLE.Exception = ex;
            }

            return HANDLE;

            //HandleArgs args = this.ValidEntity(TEntity);
            //if (args.Status == HandleStatus.Fail) return args;

            ////根据主键从数据库中查找出对应的实体
            //T TTemp = this.FetchEntity(TEntity);
            //if (TTemp == null)
            //{
            //    args.Status = HandleStatus.Fail;
            //    args.Message = "数据库中不存在此记录！";
            //    return args;
            //}

            ////更新实体值
            //ModuleBase moduleBase = TTemp as ModuleBase;
            //moduleBase.CopyPropertyFrom(TEntity);

            //args.Status = HandleStatus.Completed;
            //args.Message = "尚未保存至数据库，请调用SaveChanges方法保存！";
            //args.Exception = null;
            //return submitChanges ? this.SaveChanges() : args;

        }

        /// <summary>
        /// 单表记录查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual HandleArgs DeleteEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase
        {
            // 生成的删除语句： DELETE FROM TABLE WHERE CONDITION
            //var query = this.Context.GetTable<T>().Where(predicate);
            //DbCommand command = this.Context.GetCommand(query);
            //string selectPattern = @"From\s(?<Table>[^\s]+)\sAs\s(?<Alias>[^\s]+)\s(?<Condition>Where.+)"; //@"(?!=FROM)(?<From>FROM(?!=As)As\s(?<Alias>[^\s]+)(?!=Where)(?<Condition>Where.*))";
            //Regex rgxSql = new Regex(selectPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline );
            //var command = this.Context.GetCommand(query);

            ////　匹配命令
            //string cmd = command.CommandText.Replace(Environment.NewLine, " ");
            //Match match = rgxSql.Match(cmd);//(selectPattern);
            //HandleArgs result = new HandleArgs();
            //if (!match.Success) //return false; //匹配不成功
            //{
            //    result.Status = HandleStatus.Fail;
            //    result.Message = "生成的查询语句没有From关键字！";
            //    return result;
            //}

            //// 获取匹配到的From statement
            //string table = match.Groups["Table"].Value;
            //string alias = match.Groups["Alias"].Value;
            //string condition = match.Groups["Condition"].Value;

            //command.CommandText = string.Format("DELETE {0} {1}", alias, match.Groups[0]);
            //if (command.Connection.State != System.Data.ConnectionState.Open)
            //    command.Connection.Open();

            //try
            //{
            //    int effects = command.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    result.Status = HandleStatus.Fail;
            //    result.Exception = ex;
            //    result.Message = ex.Message;
            //}

            //return result;

            HandleArgs HANDLE = new HandleArgs();
            try
            {
                this.Context.GetTable<T>().DeleteEntity(predicate);
                HANDLE.Status = HandleStatus.Completed;
            }
            catch (Exception ex)
            {

                HANDLE.Status = HandleStatus.Fail;
                HANDLE.Message = ex.Message;
                HANDLE.Exception = ex;
            }

            return HANDLE;
        }

        /// <summary>
        /// 根据实体主键删除单个记录
        /// <remarks>实参只需要提供主键值即可</remarks>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="submitChanges">true：自动保存；fase：调用SaveChanges保存</param>
        /// <returns></returns>
        public virtual HandleArgs DeleteEntity<T>(T TEntity, bool submitChanges = true) where T : ModuleBase
        {
            //HandleArgs args = this.ValidEntity(TEntity);
            //if (args.Status == HandleStatus.Fail) return args;

            HandleArgs args = new HandleArgs();
            if (TEntity == null)
            {
                args.Message = "实体引用为空！";
                args.Status = HandleStatus.Fail;
                return args;
            }

            //ModuleBase mb = TEntity as ModuleBase;
            //if (mb == null)
            //{
            //    handle.Message = string.Format("不能从【{0}】到ModuleBase实现隐式转换！", TEntity.GetType().Name);
            //    handle.Status = HandleStatus.Fail;
            //    return handle;
            //}


            TEntity = this.FetchEntity(TEntity);
            if (TEntity == null)
            {
                args.Status = HandleStatus.Fail;
                args.Message = "数据库中不存在此记录！";
                return args;
            }

            this.Context.GetTable<T>().DeleteOnSubmit(TEntity);
            if (submitChanges) return this.SaveChanges();

            //handle.Status = HandleStatus.Completed;
            args.Message = "尚未保存至数据库，请调用SaveChanges方法保存！";
           // handle.Exception = null;
            return args;
        }

        /// <summary>
        /// 查询实体记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> QueryEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase
        {
            return this.Context.GetTable<T>().Where(predicate);
        }

        /// <summary>
        /// 取单个实体记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int CountEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase
        {
            return this.Context.GetTable<T>().Count<T>(predicate);
        }

        /// <summary>
        /// 根据实体主键集合从数据库中查找单个实体
        /// </summary>
        /// <param name="TEntity">提供主键值的实体</param>
        /// <returns>T</returns>
        public virtual T FetchEntity<T>(T TEntity) where T : ModuleBase
        {
            ModuleBase moduleBase = TEntity as ModuleBase;
            if (moduleBase == null || moduleBase.PrimaryKeys.Count == 0) return null; //没有主键映射不能从数据库中取值

            Expression<Func<T, bool>> predicate = ExpHelper.EqualChain<T>(moduleBase.PrimaryKeyValues);
            return this.QueryEntity(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 根据实体主键集合从数据库中查找单个实体
        /// </summary>
        /// <param name="primaryKeyValues">主键值集合</param>
        /// <returns>T</returns>
        public virtual T FetchEntity<T>(IDictionary<string, object> primaryKeyValues) where T : ModuleBase
        {
            if (primaryKeyValues == null || primaryKeyValues.Count == 0) return null; //没有主键映射不能从数据库中取值

            Expression<Func<T, bool>> predicate = ExpHelper.EqualChain<T>(primaryKeyValues);
            return this.QueryEntity(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 根据自增列ID获取单个实体
        /// </summary>
        /// <param name="TEntity">提供主键值的实体</param>
        /// <returns>T</returns>
        public virtual T FetchEntity<T>(int identity) where T : ModuleBase
        {
            T TEntity = default(T);
            var moduleBase = TEntity as ModuleBase;
            if (moduleBase == null ||
                string.IsNullOrEmpty(moduleBase.FetchIdentityName())) return null; //没有自增列主键

            Expression<Func<T, bool>> predicate = ExpHelper.EqualChain<T>(new Dictionary<string, object> { { moduleBase.PrimaryKeys[0], identity } });
            return this.QueryEntity(predicate).FirstOrDefault();
        }

        #endregion

        #region 辅助函数

        /// <summary>
        /// 保存改变值到数据库
        /// <remarks>公开此方法便于使用LINQ的隐式事务</remarks>
        /// </summary>
        /// <param name="commitTran">是否提交显式事务</param>
        /// <returns></returns>
        public virtual HandleArgs SaveChanges()
        {
            var result = new Gat.Utils.HandleArgs();
            try
            {
                this.Context.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);                
                //尝试提交事务
                //this.CommitTran();
                result.Status = Utils.HandleStatus.Completed;
            }
            catch (System.Data.Linq.ChangeConflictException)
            {
                //将当前值回写到数据库
                this.Context.ChangeConflicts.ResolveAll(System.Data.Linq.RefreshMode.KeepChanges);//RefreshMode.KeepCurrentValues
                this.Context.SubmitChanges();
                //尝试提交事务
                //this.CommitTran();
                result.Status = Utils.HandleStatus.Completed;
            }
            catch (Exception ex)
            {
                result.Status = Utils.HandleStatus.Fail;
                result.Exception = ex;
                result.Message = ex.Message;

                //尝试回滚事务
                //this.RollbackTran();
            }

            return result;
        }

        /// <summary>
        /// 在进行增删改时对实参进行检验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEntity"></param>
        /// <returns></returns>
        private HandleArgs ValidEntity<T>(T TEntity) where T :ModuleBase
        {
            var handle = new HandleArgs();
            if (TEntity == null)
            {
                handle.Exception = new ArgumentNullException("TEntity");
                handle.Message = handle.Exception.Message;
                return handle;
            }

            if (!TEntity.IsValid)
            {
                handle.Status = HandleStatus.Fail;
                foreach (KeyValuePair<string, string> kv in TEntity.Errors)
                {
                    handle.Message += kv.Value + Environment.NewLine;
                }
            }
            return handle;
        }

        /// <summary>
        /// 直接执行SQL脚本
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual HandleArgs ExecuteCommand(string command,params object[] parameters)
        {
            var args = new HandleArgs();
            try
            {
                this.Context.ExecuteCommand(command, parameters ?? new object[] { string.Empty });
                args.Message="Exccute Succeed！";
            }
            catch (Exception ex)
            {
                args.Status = HandleStatus.Fail;
                args.Message = "执行命令失败，消息：" + ex.Message;
            }

            return args;
        }

        

        #endregion

        #region IDisposable成员

        public void Dispose()
        {
            if (this.Context != null) this.Context.Dispose();
        }

        #endregion

        
    }
}

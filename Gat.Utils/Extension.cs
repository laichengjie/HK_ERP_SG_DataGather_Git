using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Expressions;
using System.Reflection;
using System.Data.Linq;
using System.ComponentModel;
using System.Data.Linq.Mapping;

using Gat.Utils.Expression;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Gat.Utils
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 反射公开属性
        /// </summary>
        public static BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;

        #region Expression

        /// <summary>
        /// 拼接真表达式
        /// </summary>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> TExp1,
            Expression<Func<T, bool>> TExp2) where T : class
        {
            var invokeExp = System.Linq.Expressions.Expression.Invoke(TExp2, TExp1.Parameters.Cast<System.Linq.Expressions.Expression>());
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>
                  (System.Linq.Expressions.Expression.AndAlso(TExp1.Body, invokeExp), TExp1.Parameters);
        }

        /// <summary>
        /// 拼接假表达式
        /// </summary>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> TExp1,
            Expression<Func<T, bool>> TExp2) where T : class
        {
            var invokeExp = System.Linq.Expressions.Expression.Invoke(TExp2, TExp1.Parameters.Cast<System.Linq.Expressions.Expression>());
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>
                  (System.Linq.Expressions.Expression.OrElse(TExp1.Body, invokeExp), TExp1.Parameters);
        }

        #endregion

        /// <summary>
        /// 从源对象中拷贝属性值到目标对象中
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="targetObj">目标对象</param>
        /// <param name="sourceObj">源对象</param>
        /// <param name="primaryKeyCopy">是否拷贝主键属性</param>
        public static T PropertyFrom<T>(this T targetObj, object sourceObj, bool primaryKeyCopy = true) where T : class
        {
            if (sourceObj == null) sourceObj = new object();

            Type sourceType = sourceObj.GetType();
            Type targetType = typeof(T);
            foreach (PropertyInfo pi in sourceType.GetProperties(Extension.Public))
            {
                if (!primaryKeyCopy) //不需要拷贝主键
                {
                    //检查属性是否被标志为主键
                    var columnAttribute = pi.GetColumnAttribute();
                    if (columnAttribute != null && columnAttribute.IsPrimaryKey) continue; //当前列是主键列，则不需要拷贝，继续下一轮迭代
                }

                var property = targetType.GetProperty(pi.Name, Extension.Public);
                if (property == null ||
                    property.PropertyType != pi.PropertyType ||
                    !property.CanWrite) continue;

                property.SetValue(targetObj, pi.GetValue(sourceObj, null), null);

               
            }

            return targetObj;
        }

        /// <summary>
        /// 从源对象中拷贝主键值到目标对象中，要求两个对象都具有TableAttribute特性
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="targetObj">目标对象</param>
        /// <param name="sourceObj">源对象</param>
        public static void PrimaryKeyFrom<T>(this T targetObj, object sourceObj) where T :class
        {
            if (sourceObj == null) sourceObj = new object();
            Type targetType = typeof(T),
                 sourceType = sourceObj.GetType();

            //检查两个对象是否有TableAttribute特性
            var attributes = targetType.GetCustomAttributes(typeof(TableAttribute), true);
            if (attributes == null || attributes.Length == 0) throw new NotImplementedException("类" + targetType.FullName + "上没有定义任何特性！");

            bool hasTableAttribute = false; //标志目标或者源类中是否定义表映射属性
            foreach (var attribute in attributes)
            {
                var tableAttribute = attribute as TableAttribute;
                if (tableAttribute == null) continue;

                hasTableAttribute = true;
                break;
            }
            if (!hasTableAttribute) throw new NotImplementedException("类" + targetType.FullName + "没有定义表映射特性！");

            
            attributes = sourceType.GetCustomAttributes(typeof(TableAttribute), true);
            if (attributes == null || attributes.Length == 0) throw new NotImplementedException("类" + sourceType.FullName + "上没有定义任何特性！");
            
            hasTableAttribute = false;
            foreach (var attribute in attributes)
            {
                var tableAttribute = attribute as TableAttribute;
                if (tableAttribute == null) continue;

                hasTableAttribute = true;
                break;
            }
            if (!hasTableAttribute) throw new NotImplementedException("类" + sourceType.FullName + "没有定义表映射特性！");

            //从源对象中拷贝主键属性值到目标对象的主键列中
            ColumnAttribute targetColumnAttr = null,sourceColumnAttr = null;
            bool targetHasPrimaryKey = false, sourceHasPrimaryKey = false;
            PropertyInfo targetProperty = null;
            foreach (PropertyInfo property in sourceType.GetProperties(Public))
            {
                sourceColumnAttr = property.GetColumnAttribute();
                if (sourceColumnAttr == null || !sourceColumnAttr.IsPrimaryKey) continue; //源列中没有定义主键属性，继续下一轮迭代
                sourceHasPrimaryKey = true;

                targetProperty = targetType.GetProperty(property.Name, Public);
                if (targetProperty == null ||
                   targetProperty.PropertyType != property.PropertyType ||
                   !targetProperty.CanWrite) continue; //目标对象中找不到同名属性或者目标属性类型与源属性类型不一致或者目标属性不可写，继续下一轮迭代

                targetColumnAttr = targetProperty.GetColumnAttribute();
                if (targetColumnAttr == null || !targetColumnAttr.IsPrimaryKey) continue;//目标列中没有定义主键属性，继续下一轮迭代
                targetHasPrimaryKey = true;

                //从源属性中拷贝值到目标属性中，主键属性
                targetProperty.SetValue(targetObj, property.GetValue(sourceObj, null), null);
            }

            if (!targetHasPrimaryKey || !sourceHasPrimaryKey)
                throw new NotImplementedException("类" + sourceType.FullName + "或者类" + targetType.FullName + "没有定义表映射特性！");

        }

        /// <summary>
        /// 获取单个公开属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="TEntity">实体</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="nonPublic">是否包含非公开成员</param>
        /// <returns></returns>
        public static object GetProperty<T>(this T TEntity, string propertyName,bool nonPublic) where T : class
        {
            var flag = nonPublic ? BindingFlags.Instance | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public;
            var property = typeof(T).GetProperty(propertyName, Extension.Public);
            if (property == null) return null;

            return property.GetValue(TEntity, null);
        }

        /// <summary>
        /// 设置单个公开属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="TEntity">实体</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static void SetProperty<T>(this T TEntity, string propertyName,object propertyValue) where T : class
        {
            var property = typeof(T).GetProperty(propertyName, Extension.Public);
            if (property == null || propertyValue.GetType() != property.PropertyType) return;

            property.SetValue(TEntity, propertyValue, null);
        }

        /// <summary>
        /// 获取类属性是否有ColumnAttribute特性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="TEntity">实体</param>
        /// <param name="property">成员</param>
        /// <returns></returns>
        public static ColumnAttribute GetColumnAttribute(this PropertyInfo property)
        {
            if (property == null) return null;

            return Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;
        }

        /// <summary>
        /// 比较两个字符串是否相等
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool EqualTo(this string A, string B, bool ignoreCase = true)
        {
            return string.Compare(A, B, ignoreCase) == 0;
        }

        /// <summary>
        /// 拼凑到字符串
        /// </summary>
        /// <param name="TSource"></param>
        /// <param name="joinChar"></param>
        /// <param name="needQuote"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> TSource, string joinChar = ",", bool needQuote = true)
        {
            //string join = TSource.Aggregate(string.Empty, (total, next) => total += "," + (needQuote ? "'" + next + "'" : next));
            //join = join.Substring(1);
            //return join;
            string join = string.Empty;
            foreach (string str in TSource)
            {
                join += joinChar + (needQuote ? "'" + str + "'" : str);
            }
            return join.Substring(1);
        }

        #region Linq2Sql

        /// <summary>
        /// 扩展映射表，批量删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="table"></param>
        /// <param name="predicate"></param>
        public static void DeleteEntity<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //取指定类型映射到数据库的元数据表
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));

            var query = table.Where(predicate);
            DbCommand command = table.Context.GetCommand(query);

            string batchQuery = @" DELETE {0}{1} {2} ";
            batchQuery = string.Format(batchQuery, metaTable.TableName, Environment.NewLine, table.FetchJoinSyntax(command));

            //开始执行命令
            var parameters =
                command
                .Parameters
                .Cast<DbParameter>()
                .Select(p => p.Value);
            table.Context.ExecuteCommand(batchQuery, parameters.ToArray());

            ////取指定类型映射到数据库的元数据表
            //var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));
            ////string command = String.Format("DELETE FROM {0}", tableName);

            ////ConditionBuilder conditionBuilder = new ConditionBuilder();
            ////conditionBuilder.Build(predicate.Body);

            ////if (!String.IsNullOrEmpty(conditionBuilder.Condition))
            ////{
            ////    command += " WHERE " + conditionBuilder.Condition;
            ////}

            ////return table.Context.ExecuteCommand(command, conditionBuilder.Arguments);

            //var query = table.Where(predicate);
            //DbCommand command = table.Context.GetCommand(query);
            //command.CommandText = string.Format("DELETE {0}{1}", metaTable.TableName, Environment.NewLine) + FetchJoinText(table, query);
            //var parameters =
            //    command
            //    .Parameters
            //    .Cast<DbParameter>()
            //    .Select(p => p.Value);
            //table.Context.ExecuteCommand(command.CommandText,parameters.ToArray());
        }

        /// <summary>
        /// 扩展映射表，批量更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="table"></param>
        /// <param name="predicate"></param>
        public static void UpdateEntity<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> evaluator)
            where TEntity : class
        {
            //取指定类型映射到数据库的元数据表
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));
            //取元数据表的主键集合
            var primaryKeys =
                metaTable
                .RowType
                .DataMembers
                .Where(m => m.IsPrimaryKey)
                .Select(m => m.MappedName)
                .ToArray();
            if (primaryKeys.Length == 0)
                throw new MissingPrimaryKeyException(string.Format("类型{0}上没有映射主键！", typeof(TEntity)));

            var query = table.Where(predicate);
            DbCommand command = table.Context.GetCommand(query);

            var set = new StringBuilder();
            int memberInitCount = 1;

            // Process the MemberInitExpression (there should only be one in the evaluator Lambda) to convert the expression tree
            // into a valid DbCommand.  The Visit<> method will only process expressions of type MemberInitExpression and requires
            // that a MemberInitExpression be returned - in our case we'll return the same one we are passed since we are building
            // a DbCommand and not 'really using' the evaluator Lambda.
            evaluator.Visit<MemberInitExpression>(delegate(MemberInitExpression expression)
            {
                if (memberInitCount > 1)
                {
                    throw new NotImplementedException("Currently only one MemberInitExpression is allowed for the evaluator parameter.");
                }
                memberInitCount++;

                set.Append(FetchSetSyntax<TEntity>(expression, table, command));

                return expression; // just return passed in expression to keep 'visitor' happy.
            });

            //拼凑JOIN语句的关联条件
            var join = new StringBuilder();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                join.Append(string.Format(@"([y0].[{0}] = [y1].[{0}])", primaryKeys[i]));
                if (i < primaryKeys.Length - 1) join.Append(@" AND ");
            }

            string batchQuery = @" UPDATE {0} {1}{2}{1} FROM {0} AS [y0] {1} INNER JOIN ( {1} {3} ) AS [y1] ON {4} ";
            batchQuery = string.Format(batchQuery, metaTable.TableName,Environment.NewLine, set.ToString(), command.CommandText, join.ToString());

            //开始执行命令
            var parameters =
                command
                .Parameters
                .Cast<DbParameter>()
                .Select(p => p.Value);
            table.Context.ExecuteCommand(batchQuery, parameters.ToArray());
        }

        /// <summary>
        /// 获取含JOIN的原生SQL
        /// <remarks>使用主键关联子查询的做法</remarks>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="table">映射的表</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static string FetchJoinSyntax<TEntity>(Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            //取指定类型映射到数据库的元数据表
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));
            //取元数据表的主键集合
            var primaryKeys =
                metaTable
                .RowType
                .DataMembers
                .Where(m => m.IsPrimaryKey)
                .Select(m => m.MappedName)
                .ToArray();
            if (primaryKeys.Length == 0)
                throw new MissingPrimaryKeyException(string.Format("类型{0}上没有映射主键！", typeof(TEntity)));

            //拼凑JOIN语句的关联条件
            var join = new StringBuilder();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                join.Append(string.Format(@"([y0].[{0}] = [y1].[{0}])", primaryKeys[i]));
                if (i < primaryKeys.Length - 1) join.Append(@" AND ");
            }

            string batchQuery = @" FROM {0} AS [y0] {1} INNER JOIN ( {1} {2} ) AS [y1] ON {3} ";
            batchQuery = string.Format(batchQuery,metaTable.TableName, Environment.NewLine, table.Context.GetCommand(entities).CommandText, join.ToString());

            return batchQuery;

            //var joinQuery = string.Format(@" From {0} AS [y0]");

            //var keys = from mdm in metaTable.RowType.DataMembers
            //           where mdm.IsPrimaryKey
            //           select new { mdm.MappedName };

            //var joinSB = new StringBuilder();
            //var subSelectSB = new StringBuilder();

            //foreach (var key in keys)
            //{
            //    joinSB.AppendFormat("j0.[{0}] = j1.[{0}] AND ", key.MappedName);
            //    // For now, always assuming table is aliased as t0.  Should probably improve at some point.
            //    // Just writing a smaller sub-select so it doesn't get all the columns of data, but instead
            //    // only the primary key fields used for joining.
            //    subSelectSB.AppendFormat("[t0].[{0}], ", key.MappedName);
            //}

            //var selectCommand = table.Context.GetCommand(entities);
            //var select = selectCommand.CommandText;

            //var join = joinSB.ToString();

            //if (join == "")
            //{
            //    throw new MissingPrimaryKeyException(string.Format("{0} does not have a primary key defined.  Batch updating/deleting can not be used for tables without a primary key.", metaTable.TableName));
            //}

            //join = join.Substring(0, join.Length - 5);											// Remove last ' AND '
            
            //#region - Better ExpressionTree Handling Needed -
            ///*
			
            //Below is a sample query where the let statement was used to simply the 'where clause'.  However, it produced an extra level
            //in the query.
			 
            //var manage =
            //    from u in User
            //    join g in Groups on u.User_Group_id equals g.gKey into groups
            //    from g in groups.DefaultIfEmpty()
            //    let correctGroup = groupsToManage.Contains( g.gName ) || ( groupsToManage.Contains( "_GLOBAL" ) && g.gKey == null )
            //    where correctGroup && ( users.Contains( u.User_Authenticate_id ) || userEmails.Contains( u.User_Email ) ) || userKeys.Contains( u.User_id )
            //    select u;
			 
            //Produces this SQL:
            //SELECT [t2].[User_id] AS [uKey], [t2].[User_Authenticate_id] AS [uAuthID], [t2].[User_Email] AS [uEmail], [t2].[User_Pin] AS [uPin], [t2].[User_Active] AS [uActive], [t2].[uAdminAuthID], [t2].[uFailureCount]
            //FROM (
            //    SELECT [t0].[User_id], [t0].[User_Authenticate_id], [t0].[User_Email], [t0].[User_Pin], [t0].[User_Active], [t0].[uFailureCount], [t0].[uAdminAuthID], 
            //        (CASE 
            //            WHEN [t1].[gName] IN (@p0) THEN 1
            //            WHEN NOT ([t1].[gName] IN (@p0)) THEN 0
            //            ELSE NULL
            //         END) AS [value]
            //    FROM [User] AS [t0]
            //    LEFT OUTER JOIN [Groups] AS [t1] ON [t0].[User_Group_id] = ([t1].[gKey])
            //    ) AS [t2]
            //WHERE (([t2].[value] = 1) AND (([t2].[User_Authenticate_id] IN (@p1)) OR ([t2].[User_Email] IN (@p2)))) OR ([t2].[User_id] IN (@p3))			 
			
            //If I put the entire where in one line...
            //where 	( groupsToManage.Contains( g.gName ) || ( groupsToManage.Contains( "_GLOBAL" ) && g.gKey == null ) ) && 
            //        ( users.Contains( u.User_Authenticate_id ) || userEmails.Contains( u.User_Email ) ) || userKeys.Contains ( u.User_id )

            //I get this SQL:
            //SELECT [t0].[User_id] AS [uKey], [t0].[User_Authenticate_id] AS [uAuthID], [t0].[User_Email] AS [uEmail], [t0].[User_Pin] AS [uPin], [t0].[User_Active] AS [uActive], [t0].[uAdminAuthID], [t0].[uFailureCount]
            //FROM [User] AS [t0]
            //LEFT OUTER JOIN [Groups] AS [t1] ON [t0].[User_Group_id] = ([t1].[gKey])
            //WHERE (([t1].[gName] IN (@p0)) AND (([t0].[User_Authenticate_id] IN (@p1)) OR ([t0].[User_Email] IN (@p2)))) OR ([t0].[User_id] IN (@p3))			
			
            //The second 'cleaner' SQL worked with my original 'string parsing' of simply looking for [t0] and stripping everything before it
            //to get rid of the SELECT and any 'TOP' clause if present.  But the first SQL introduced a layer which caused [t2] to be used.  So
            //I have to do a bit different string parsing.  There is probably a more efficient way to examine the ExpressionTree and figure out
            //if something like this is going to happen.  I will explore it later.
            //*/
            //#endregion
            
            //var endSelect = select.IndexOf("[t");													// Get 'SELECT ' and any TOP clause if present
            //var selectClause = select.Substring(0, endSelect);
            //var selectTableNameStart = endSelect + 1;												// Get the table name LINQ to SQL used in query generation
            //var selectTableName = select.Substring(selectTableNameStart,							// because I have to replace [t0] with it in the subSelectSB
            //                            select.IndexOf("]", selectTableNameStart) - (selectTableNameStart));

            //// TODO (Chris): I think instead of searching for ORDER BY in the entire select statement, I should examine the ExpressionTree and see
            //// if the *outer* select (in case there are nested subselects) has an orderby clause applied to it.
            //var needsTopClause = selectClause.IndexOf(" TOP ") < 0 && select.IndexOf("\r\nORDER BY ") > 0;

            //var subSelect = selectClause
            //                    + (needsTopClause ? "TOP 100 PERCENT " : "")							// If order by in original select without TOP clause, need TOP
            //                    + subSelectSB.ToString()												// Append just the primary keys.
            //                                 .Replace("[t0]", string.Format("[{0}]", selectTableName));
            //subSelect = subSelect.Substring(0, subSelect.Length - 2);									// Remove last ', '

            //subSelect += select.Substring(select.IndexOf("\r\nFROM ")); // Create a sub SELECT that *only* includes the primary key fields

            //var batchJoin = String.Format("FROM {0} AS j0 INNER JOIN (\r\n\r\n{1}\r\n\r\n) AS j1 ON ({2})\r\n", table.GetMapName(), subSelect, join);
            //return batchJoin;
        }

        /// <summary>
        /// 获取含JOIN的原生SQL
        /// <remarks>使用主键关联子查询的做法</remarks>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="table">映射的表</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static string FetchJoinSyntax<TEntity>(this Table<TEntity> table, DbCommand command) where TEntity : class
        {
            //取指定类型映射到数据库的元数据表
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));
            //取元数据表的主键集合
            var primaryKeys =
                metaTable
                .RowType
                .DataMembers
                .Where(m => m.IsPrimaryKey)
                .Select(m => m.MappedName)
                .ToArray();
            if (primaryKeys.Length == 0)
                throw new MissingPrimaryKeyException(string.Format("类型{0}上没有映射主键！", typeof(TEntity)));

            //拼凑JOIN语句的关联条件
            var join = new StringBuilder();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                join.Append(string.Format(@"([y0].[{0}] = [y1].[{0}])", primaryKeys[i]));
                if (i < primaryKeys.Length - 1) join.Append(@" AND ");
            }

            string batchQuery = @" FROM {0} AS [y0] {1} INNER JOIN ( {1} {2} ) AS [y1] ON {3} ";
            batchQuery = string.Format(batchQuery, metaTable.TableName, Environment.NewLine, command.CommandText, join.ToString());

            return batchQuery;
        }

        /// <summary>
        /// 获取含JOIN的原生SQL
        /// <remarks>使用主键关联子查询的做法</remarks>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="table">映射的表</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static string FetchJoinSyntax<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            //取指定类型映射到数据库的元数据表
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));
            //取元数据表的主键集合
            var primaryKeys =
                metaTable
                .RowType
                .DataMembers
                .Where(m => m.IsPrimaryKey)
                .Select(m => m.MappedName)
                .ToArray();
            if (primaryKeys.Length == 0)
                throw new MissingPrimaryKeyException(string.Format("类型{0}上没有映射主键！", typeof(TEntity)));

            var query = table.Where(predicate);
            DbCommand command = table.Context.GetCommand(query);

            //拼凑JOIN语句的关联条件
            var join = new StringBuilder();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                join.Append(string.Format(@"([y0].[{0}] = [y1].[{0}])", primaryKeys[i]));
                if (i < primaryKeys.Length - 1) join.Append(@" AND ");
            }

            string batchQuery = @" FROM {0} AS [y0] {1} INNER JOIN ( {1} {2} ) AS [y1] ON {3} ";
            batchQuery = string.Format(batchQuery, metaTable.TableName, Environment.NewLine, command.CommandText, join.ToString());

            return batchQuery;
        }

        /// <summary>
        /// 获取更新的SET原生SQL
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="memberInitExpression"></param>
        /// <param name="table"></param>
        /// <param name="updateCommand"></param>
        /// <returns></returns>
        private static string FetchSetSyntax<TEntity>(MemberInitExpression memberInitExpression, Table<TEntity> table, DbCommand updateCommand) where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (memberInitExpression.Type != entityType)
            {
                throw new NotImplementedException(string.Format("The MemberInitExpression is intializing a class of the incorrect type '{0}' and it should be '{1}'.", memberInitExpression.Type, entityType));
            }

            var set = new StringBuilder();

            //var tableName = table.GetDbName();
            var metaTable = table.Context.Mapping.GetTable(entityType);
            // Used to look up actual field names when MemberAssignment is a constant,
            // need both the Name (matches the property name on LINQ object) and the
            // MappedName (db field name).
            var dbCols = from mdm in metaTable.RowType.DataMembers
                         select new { mdm.MappedName, mdm.Name };

            // Walk all the expression bindings and generate SQL 'commands' from them.  Each binding represents a property assignment
            // on the TEntity object initializer Lambda expression.
            foreach (var binding in memberInitExpression.Bindings)
            {
                var assignment = binding as MemberAssignment;

                if (binding == null)
                {
                    throw new NotImplementedException("All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");
                }

                // TODO (Document): What is this doing?  I know it's grabbing existing parameter to pass into Expression.Call() but explain 'why'
                //					I assume it has something to do with fact we can't just access the parameters of assignment.Expression?
                //					Also, any concerns of whether or not if there are two params of type entity type?
                ParameterExpression entityParam = null;
                assignment.Expression.Visit<ParameterExpression>(delegate(ParameterExpression p) { if (p.Type == entityType) entityParam = p; return p; });

                // Get the real database field name.  binding.Member.Name is the 'property' name of the LINQ object
                // so I match that to the Name property of the table mapping DataMembers.
                string name = binding.Member.Name;
                var dbCol = (from c in dbCols
                             where c.Name == name
                             select c).FirstOrDefault();

                if (dbCol == null)
                {
                    throw new ArgumentOutOfRangeException(name, string.Format("The corresponding field on the {0} table could not be found.", metaTable.TableName));
                }

                // If entityParam is NULL, then no references to other columns on the TEntity row and need to eval 'constant' value...
                if (entityParam == null)
                {
                    // Compile and invoke the assignment expression to obtain the contant value to add as a parameter.
                    var constant = System.Linq.Expressions.Expression.Lambda(assignment.Expression, null).Compile().DynamicInvoke();

                    // use the MappedName from the table mapping DataMembers - that is field name in DB table.
                    if (constant == null)
                    {
                        set.AppendFormat("[{0}] = null, ", dbCol.MappedName);
                    }
                    else
                    {
                        // Add new parameter with massaged name to avoid clashes.
                        set.AppendFormat("[{0}] = @p{1}, ", dbCol.MappedName, updateCommand.Parameters.Count);
                        updateCommand.Parameters.Add(new SqlParameter(string.Format("@p{0}", updateCommand.Parameters.Count), constant));
                    }
                }
                else
                {
                    // TODO (Documentation): Explain what we are doing here again, I remember you telling me why we have to call but I can't remember now.
                    // Wny are we calling Expression.Call and what are we passing it?  Below comments are just 'made up' and probably wrong.

                    // Create a MethodCallExpression which represents a 'simple' select of *only* the assignment part (right hand operator) of
                    // of the MemberInitExpression.MemberAssignment so that we can let the Linq Provider do all the 'sql syntax' generation for
                    // us. 
                    //
                    // For Example: TEntity.Property1 = TEntity.Property1 + " Hello"
                    // This selectExpression will be only dealing with TEntity.Property1 + " Hello"
                    var selectExpression = System.Linq.Expressions.Expression.Call(
                                                typeof(Queryable),
                                                "Select",
                                                new Type[] { entityType, assignment.Expression.Type },

                    // TODO (Documentation): How do we know there are only 'two' parameters?  And what is Expression.Lambda
                        //						 doing?  I assume it's returning a type of assignment.Expression.Type to match above?

                                                System.Linq.Expressions.Expression.Constant(table),
                                                System.Linq.Expressions.Expression.Lambda(assignment.Expression, entityParam));

                    set.AppendFormat("[{0}] = {1}, ",
                                            dbCol.MappedName,
                                            FetchSetAssignment(table, selectExpression, updateCommand, name));
                }
            }

            var setStatements = set.ToString();
            return "SET " + setStatements.Substring(0, setStatements.Length - 2); // remove ', '
        }

        private static string FetchSetAssignment(ITable table, MethodCallExpression selectExpression, DbCommand updateCommand, string bindingName)
        {
            ValidateExpression(table, selectExpression);

            // Convert the selectExpression into an IQueryable query so that I can get the CommandText
            var selectQuery = (table as IQueryable).Provider.CreateQuery(selectExpression);

            // Get the DbCommand so I can grab relavent parts of CommandText to construct a field 
            // assignment and based on the 'current TEntity row'.  Additionally need to massage parameter 
            // names from temporary command when adding to the final update command.
            var selectCmd = table.Context.GetCommand(selectQuery);
            var selectStmt = selectCmd.CommandText;
            selectStmt = selectStmt.Substring(7,									// Remove 'SELECT ' from front ( 7 )
                                        selectStmt.IndexOf("\r\nFROM ") - 7)		// Return only the selection field expression
                                    .Replace("[t0].", "")							// Remove table alias from the select
                                    .Replace(" AS [value]", "")					// If the select is not a direct field (constant or expression), remove the field alias
                                    .Replace("@p", "@p" + bindingName);			// Replace parameter name so doesn't conflict with existing ones.

            foreach (var selectParam in selectCmd.Parameters.Cast<DbParameter>())
            {
                var paramName = string.Format("@p{0}", updateCommand.Parameters.Count);

                // DataContext.ExecuteCommand ultimately just takes a object array of parameters and names them p0-N.  
                // So I need to now do replaces on the massaged value to get it in proper format.
                selectStmt = selectStmt.Replace(
                                    selectParam.ParameterName.Replace("@p", "@p" + bindingName),
                                    paramName);

                updateCommand.Parameters.Add(new SqlParameter(paramName, selectParam.Value));
            }

            return selectStmt;
        }

        /// <summary>
        /// Some LINQ Query syntax is invalid because SQL (or whomever the provider is) can not translate it to its native language.  
        /// DataContext.GetCommand() does not detect this, only IProvider.Execute or IProvider.Compile call the necessary code to 
        /// check this.  This function invokes the IProvider.Compile to make sure the provider can translate the expression.
        /// </summary>
        /// <remarks>
        /// An example of a LINQ query that previously 'worked' in the *Batch methods but needs to throw an exception is something
        /// like the following:
        /// 
        /// var pay = 
        ///		from h in HistoryData
        ///		where h.his.Groups.gName == "Ochsner" && h.hisType == "pay"
        ///		select h;
        ///		
        /// HistoryData.UpdateBatchPreview( pay, h => new HistoryData { hisIndex = ( int.Parse( h.hisIndex ) - 1 ).ToString() } ).Dump();
        /// 
        /// The int.Parse is not valid and needs to throw an exception like: 
        /// 
        ///		Could not translate expression '(Parse(p.hisIndex) - 1).ToString()' into SQL and could not treat it as a local expression.
        ///		
        ///	Unfortunately, the IProvider.Compile is internal and I need to use Reflection to call it (ugh).  I've several e-mails sent into
        ///	MS LINQ team members and am waiting for a response and will correct/improve code as soon as possible.
        /// </remarks>
        private static void ValidateExpression(ITable table, System.Linq.Expressions.Expression expression)
        {
            var context = table.Context;
            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            var provider = providerProperty.GetValue(context, null);
            var compileMI = provider.GetType().GetMethod("System.Data.Linq.Provider.IProvider.Compile", BindingFlags.Instance | BindingFlags.NonPublic);

            // Simply compile the expression to see if it will work.
            compileMI.Invoke(provider, new object[] { expression });
        }

        #endregion

        #region IQuery转DataTable

        /// <summary>
        /// 将Linq查询的结果转换成DataTable
        /// </summary>
        /// <param name="columns">保留的列</param>
        /// <returns></returns>
        public static System.Data.DataTable ToDataTable(this IQueryable query, IList<string> columns)
        {
            DataTable dt = new DataTable();
            var props = query.ElementType.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in props)
            {
                if (columns != null && columns.Count > 0)
                {
                    if (!columns.Contains(pi.Name)) continue;
                }
                string ddd = pi.PropertyType.ToString();


                if ((pi.PropertyType == System.Type.GetType("System.Nullable`1[System.Int32]")) || (pi.PropertyType == System.Type.GetType("System.Int32")))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.Int32"));
                }
                else if (pi.PropertyType == System.Type.GetType("System.String"))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.String"));
                }
                else if ((pi.PropertyType == System.Type.GetType("System.Nullable`1[System.DateTime]")) || (pi.PropertyType == System.Type.GetType("System.DateTime")))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.DateTime"));
                }
                else
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.Decimal"));
                }

            }
            foreach (var item in query)
            {

                DataRow dr = dt.NewRow();
                foreach (System.Reflection.PropertyInfo pi in props)
                {
                    if (columns != null && columns.Count > 0)
                    {
                        if (!columns.Contains(pi.Name)) continue;
                    }

                    if (pi.GetValue(item, null) != null)
                    {
                        if (pi.GetValue(item, null) != null)
                        {
                            dr[pi.Name] = pi.GetValue(item, null);
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
            //return dataTable;
        }


        public static System.Data.DataTable ToDataTable<T>(this List<T> list, IList<string> columns)
        {
            DataTable dt = new DataTable();
            var props = typeof(T).GetProperties();
            foreach (System.Reflection.PropertyInfo pi in props)
            {
                if (columns != null && columns.Count > 0)
                {
                    if (!columns.Contains(pi.Name)) continue;
                }
                string ddd = pi.PropertyType.ToString();


                if ((pi.PropertyType == System.Type.GetType("System.Nullable`1[System.Int32]")) || (pi.PropertyType == System.Type.GetType("System.Int32")))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.Int32"));
                }
                else if (pi.PropertyType == System.Type.GetType("System.String"))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.String"));
                }
                else if ((pi.PropertyType == System.Type.GetType("System.Nullable`1[System.DateTime]")) || (pi.PropertyType == System.Type.GetType("System.DateTime")))
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.DateTime"));
                }
                else
                {
                    dt.Columns.Add(pi.Name, System.Type.GetType("System.Decimal"));
                }

            }
            foreach (var item in list)
            {

                DataRow dr = dt.NewRow();
                foreach (System.Reflection.PropertyInfo pi in props)
                {
                    if (columns != null && columns.Count > 0)
                    {
                        if (!columns.Contains(pi.Name)) continue;
                    }

                    if (pi.GetValue(item, null) != null)
                    {
                        if (pi.GetValue(item, null) != null)
                        {
                            dr[pi.Name] = pi.GetValue(item, null);
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
            //return dataTable;
        }
        #endregion

    }
}


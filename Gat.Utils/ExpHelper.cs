
#region 文件头描述信息

/* Copyright (C) 2011 恒康信息科技有限公司版权所有
 * 文件名：Logger.cs
 * 文件功能描述：表达式助手类
 * 
 * 创建标识：tian_naixiang 2011-0618 07:51
 * 
 * 
 * 修改标识：
 * 修改描述：
 * 
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Expressions;

namespace Gat.Utils
{
    /// <summary>
    /// Lambda 表达式助手类
    /// </summary>
    public class ExpHelper
    {
        /// <summary>
        /// 返回真表达式
        /// </summary>
        public static Expression<Func<T, bool>> True<T>()
            where T : class
        {
            return f => true;
        }

        /// <summary>
        /// 返回假表达式
        /// </summary>
        public static Expression<Func<T, bool>> False<T>()
            where T : class
        {
            return f => false;
        }

        /// <summary>
        /// 创建单个等于表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> EqualSingle<T>(KeyValuePair<string,object> keyValue)
            where T : class
        {
            Type TType = typeof(T);
            var paramExp = System.Linq.Expressions.Expression.Parameter(TType, "T");
            var propertyExp = System.Linq.Expressions.Expression.Property(paramExp, keyValue.Key);
            var constExp = System.Linq.Expressions.Expression.Constant(keyValue.Value);
            System.Linq.Expressions.Expression rightExp = constExp;
            System.Reflection.PropertyInfo property = TType.GetProperty(keyValue.Key, Extension.Public);
            if (property != null)
                rightExp = System.Linq.Expressions.Expression.Convert(constExp, property.PropertyType);//类型转换
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Equal(propertyExp, rightExp), 
                new ParameterExpression[] { paramExp });
        }

        /// <summary>
        /// 创建等于表达式链
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> EqualChain<T>(IDictionary<string, object> keyValues)
            where T : class
        {
            var TExp = ExpHelper.True<T>();
            foreach (KeyValuePair<string, object> kv in keyValues)
                TExp = TExp.AndAlso<T>(ExpHelper.EqualSingle<T>(kv));
            return TExp;
        }
    }
}

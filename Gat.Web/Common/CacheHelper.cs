using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Common
{
    /// <summary>
    /// 应用程序缓存
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 设置缓存，默认30天
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCache(string key, object value,int expires = 30)
        {
            HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(expires));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// 取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public static T FetchCache<T>(string key) where T : class
        {
            return HttpRuntime.Cache.Get(key) as T;
        }
    }
}
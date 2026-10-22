using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data.Linq.Mapping;
using System.Data;

using System.IO;
using System.Xml.Linq;
using System.Xml;
using Gat.Utils;

namespace Gat.DAL
{
    /// <summary>
    /// LINQ数据上下文基类
    /// </summary>
    public class DataContextBase : System.Data.Linq.DataContext
    {

        /// <summary>
        /// 映射源
        /// </summary>
        private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();

        /// <summary>
        /// 指定连接字符串的初始化方式
        /// </summary>
        /// <param name="connParam">忽略此值，使用web.config文件中的WEBDB配置</param>
        /// <param name="mapping"></param>
        public DataContextBase(string connParam, MappingSource mapping)
            : base(GetConnection(ConnnectType.Web), mapping)
        {
            this.CommandTimeout = 300;
        }

        public DataContextBase(IDbConnection connection, MappingSource mapping)
            : base(connection, mapping)
        {
            this.CommandTimeout = 300;
        }

        /// <summary>
        /// 指定运行环境的初始化方式
        /// </summary>
        /// <param name="ct">运行环境</param>
        public DataContextBase(ConnnectType ct)
            : base(GetConnection(ct), mappingSource)
        {
            this.CommandTimeout = 300;
        }

        ///// <summary>
        ///// 从web.config文件中取得配置
        ///// </summary>
        ///// <param name="configKey"></param>
        ///// <returns></returns>
        //protected static string GetConnection(string configKey)
        //{
        //    string config = XmlHelper.WebConfigConnection(configKey);
        //    string connect = string.Empty;
        //    try
        //    {
        //        connect = SecurityHelper.DecryptDes(config);
        //    }
        //    catch
        //    {

        //    }

        //    return connect;
        //}

        /// <summary>
        /// 从web.config文件中取得配置
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        protected static string GetConnection(ConnnectType conType)
        {
            string configKey = GetConfigKeyByConnectType(conType);
            //return GetConnection(configKey);

            return XmlHelper.WebConfigConnection(configKey);
        }

        /// <summary>
        /// 根据连接类型取得相应的在web.config文件配置的key值
        /// </summary>
        /// <param name="conType"></param>
        /// <returns></returns>
        protected static string GetConfigKeyByConnectType(ConnnectType conType)
        {
            switch(conType)
            {
                case ConnnectType.Web:
                    return "Web";
                case ConnnectType.ERP:
                    return "ERP";
                case ConnnectType.WebTest:
                    return "WebTest";
                case ConnnectType.ERPTest:
                    return "ERPTest";
                default: return string.Empty;
            }
        }

        
    }
}

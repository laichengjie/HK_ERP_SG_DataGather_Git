using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Gat.Utils
{
    /// <summary>
    /// xml助手
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 检查连接配置
        /// </summary>
        /// <param name="connFilePath">网络共享目录路径</param>
        /// <returns></returns>
        public static HandleArgs CheckConnectConfig()
        {
            HandleArgs HANDLE = new HandleArgs();

            //string configFile = string.Empty;
            //if (isNetPath)
            //{
            //    configFile = connFilePath;
            //}
            //else
            //{
            //    string assembly = Assembly.GetExecutingAssembly().CodeBase;
            //    string assemblyFile = assembly.Substring(8);
            //    FileInfo file = new FileInfo(assemblyFile);

            //    string executePath = file.Directory.FullName;//.Parent.FullName;
            //                                                 //executePath = executePath.Substring(0, executePath.LastIndexOf(@"\"));
            //                                                 //string configName = "web.config"; //
            //    string configName = "ConnectConfig.DAT"; //
            //    configFile = Path.Combine(executePath, configName);
            //}
            var connFilePath = CommonSettings.ConnFilePath;
            if (!File.Exists(connFilePath))
            {
                HANDLE.Status = HandleStatus.Fail;
                HANDLE.Message = "没有帐套连接信息！";
                return HANDLE;
            }

            try
            {
                XDocument xdoc = XDocument.Load(connFilePath);
                string message = string.Empty;
                foreach (Database data in Enum.GetValues(typeof(Database)))
                {
                    string nodeName = data.ToString();
                    if (xdoc.Root.Element(nodeName) == null ||
                        xdoc.Root.Element(nodeName).Element("Server") == null ||
                        string.IsNullOrEmpty(xdoc.Root.Element(nodeName).Element("Server").Value))
                    {
                        message += GetDatabaseString(data) + "、";
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    HANDLE.Status = HandleStatus.Fail;
                    HANDLE.Message = "没有" + message + "的帐套连接信息，请联系管理员！";
                    return HANDLE;
                }

                HANDLE.Status = HandleStatus.Completed;
                HANDLE.Message = "可以使用！";
                return HANDLE;
            }
            catch (Exception ex)
            {
                HANDLE.Status = HandleStatus.Fail;
                HANDLE.Message = "检查配置文件时出错，消息：" + ex.Message;
                return HANDLE;
            }
        }

        //XDocument xdoc = null;

        /// <summary>
        /// 取web.config文件 的连接字符串信息
        /// </summary>
        /// <returns></returns>
        public static string WebConfigConnection(string name, string connFile="")
        {
            //string assembly = Assembly.GetExecutingAssembly().CodeBase;
            //string assemblyFile = assembly.Substring(8);
            //FileInfo file = new FileInfo(assemblyFile);

            //string executePath = file.Directory.FullName;//.Parent.FullName;
            ////executePath = executePath.Substring(0, executePath.LastIndexOf(@"\"));
            ////string configName = "web.config"; //
            //string configName = "ConnectConfig.DAT"; //
            //string configFile = Path.Combine(executePath, configName);
            var configFile = string.IsNullOrEmpty(connFile) ? CommonSettings.ConnFilePath : connFile;
            try
            {
                XDocument xdoc = XDocument.Load(configFile);
                var xlNode = xdoc.Root.Element(name);
                if (xlNode == null) return null;

                string connect = "Data Source={0};Initial Catalog={1};Integrated Security=True";
                if (xlNode.Element("LoginType") != null &&
                   SecurityHelper.DecryptDes(xlNode.Element("LoginType").Value) == "Sql")
                {
                    connect = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3};pooling = true; Max Pool Size = 15; Min Pool Size = 10;Timeout = 2000";
                }
                string server = xlNode.Element("Server") != null ? xlNode.Element("Server").Value : null;
                string userName = xlNode.Element("UserID") != null ? xlNode.Element("UserID").Value : null;
                string password = xlNode.Element("Password") != null ? xlNode.Element("Password").Value : null;
                string database = xlNode.Element("GroupDB") != null ? xlNode.Element("GroupDB").Value : null;

                connect = string.Format(connect,
                    SecurityHelper.DecryptDes(server),
                    SecurityHelper.DecryptDes(database),
                    SecurityHelper.DecryptDes(userName),
                    SecurityHelper.DecryptDes(password));

                return connect;
            }
            catch (Exception ex)
            {
                return null;
            }



            //string connect = "Data Source={0};Initial Catalog=master;Integrated Security=True";


            //if (this.lueLoginType.EditValue != null && this.lueLoginType.EditValue.ToString().Trim() == "0")
            //{
            //    connString = string.Format("Data Source={0};Initial Catalog=master;Integrated Security=True", this.txtServer.Text);
            //}
            //else
            //{
            //    connString = string.Format("Data Source={0};Initial Catalog=master;User ID={1};Password={2};pooling = true; Max Pool Size = 15; Min Pool Size = 10;Timeout = 2000", new object[] { this.txtServer.Text, this.txtUser.Text, this.txtPwd.Text });
            //}

            //return xelCon.Value;

            //数据库连接节点
            //var connNode = xelCon.Elements("add").FirstOrDefault(xel => xel.Attribute("name").Value == name);
            //if (connNode == null) return null;

            //return connNode.Attribute("connectionString").Value;
        }

        /// <summary>
        /// 取web.config文件 的连接字符串信息
        /// </summary>
        /// <returns></returns>
        public static string AppSetting(string name)
        {
            string assembly = Assembly.GetExecutingAssembly().CodeBase;
            string assemblyFile = assembly.Substring(8);
            FileInfo file = new FileInfo(assemblyFile);

            string executePath = file.Directory.Parent.FullName;
            //executePath = executePath.Substring(0, executePath.LastIndexOf(@"\"));
            string configName = "web.config"; //
            string configFile = Path.Combine(executePath, configName);

            XDocument xdoc = XDocument.Load(configFile);
            var xelCon = xdoc.Root.Element("appSettings");
            if (xelCon == null) return null;

            var erpDB = xelCon.Elements("add").FirstOrDefault(xel => xel.Attribute("key").Value == name);
            if (erpDB == null) return null;

            return erpDB.Attribute("value").Value;
        }


        /// <summary>
        /// 取枚举字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetDatabaseString(Database data)
        {
            switch (data)
            {
                case Database.Web:
                    return "平台正式库";

                case Database.WebTest:
                    return "平台测试库";

                case Database.ERP:
                    return "ERP正式库";

                case Database.ERPTest:
                    return "ERP测试库";

                default:
                    return string.Empty;
            }
        }
    }
}

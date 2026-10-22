using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GatNewBll.Common
{
    /// <summary>
    /// 导出导入 助手类
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// 使用指定的分隔符导出数据
        /// </summary>
        /// <param name="dtExport">数据源</param>
        /// <param name="fullName">导出文件完全路径</param>
        /// <param name="separate">分隔符</param>
        public static void ExportSeparated(DataTable dtExport, string fullName, string separate = ",", bool exportHeader = true)
        {
            if (File.Exists(fullName)) File.Delete(fullName);

            StringBuilder contentBuilder = new StringBuilder();

            using (FileStream stream = new FileStream(fullName, FileMode.CreateNew))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                {
                    if (exportHeader)
                    {
                        //标头
                        for (int i = 0; i < dtExport.Columns.Count; i++)
                        {
                            contentBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                            if (i <= dtExport.Columns.Count - 1) contentBuilder.Append(separate);
                        }
                        writer.WriteLine(contentBuilder);
                    }

                    //内容
                    for (int i = 0; i < dtExport.Rows.Count; i++)
                    {
                        contentBuilder.Remove(0, contentBuilder.Length);//移出
                        for (int j = 0; j < dtExport.Columns.Count; j++)
                        {
                            contentBuilder.Append(dtExport.Rows[i][j].ToString());
                            if (j <= dtExport.Columns.Count - 1) contentBuilder.Append(separate);
                        }
                        writer.WriteLine(contentBuilder);
                    }
                }
            }

        }

        /// <summary>
        /// 导出到txt
        /// </summary>
        /// <param name="dtExport">数据源</param>
        /// <param name="fullName">导出文件完全路径</param>
        /// <param name="spliter">字段分割内容</param>
        /// <param name="exportHeader">是否需要标头</param>
        public static void ExportToText(DataTable dtExport, string fullName, string spliter = "", bool exportHeader = true)
        {
            if (File.Exists(fullName)) File.Delete(fullName);


            StringBuilder headBuilder = new StringBuilder();    ////先打印标头
            StringBuilder bodyBuilder = new StringBuilder();   ////行值

            using (FileStream stream = new FileStream(fullName, FileMode.CreateNew))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    if (exportHeader)
                    {
                        //标头
                        for (int i = 0; i < dtExport.Columns.Count; i++)
                        {
                            headBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                            if (i <= dtExport.Columns.Count - 1) headBuilder.Append(spliter);
                        }
                        writer.WriteLine(headBuilder);
                    }

                    //内容
                    for (int i = 0; i < dtExport.Rows.Count; i++)
                    {
                        bodyBuilder.Remove(0, bodyBuilder.Length);//移出

                        for (int j = 0; j < dtExport.Columns.Count; j++)
                        {
                            bodyBuilder.Append(dtExport.Rows[i][j].ToString());
                            if (j < dtExport.Columns.Count - 1) bodyBuilder.Append(spliter);
                        }
                        writer.WriteLine(bodyBuilder);
                    }
                }
            }
        }

        /// <summary>
        /// 导出到csv
        /// </summary>
        /// <param name="dtExport">数据源</param>
        /// <param name="fullName">导出文件完全路径</param>
        /// <param name="needTab">是否需要在值前插入tab以将字段作为文本格式显示在excel中</param>
        public static void ExportToCsv(DataTable dtExport, string fullName, bool needTab = false)
        {
            if (File.Exists(fullName)) File.Delete(fullName);

            StringBuilder headBuilder = new StringBuilder();    ////先打印标头
            StringBuilder bodyBuilder = new StringBuilder();   ////行值
            var prefix = "";
            if (needTab)
            {
                prefix = "\t";
            }
            using (FileStream stream = new FileStream(fullName, FileMode.CreateNew))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                {
                    //标头
                    for (int i = 0; i < dtExport.Columns.Count; i++)
                    {
                        headBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                        if (i < dtExport.Columns.Count - 1) headBuilder.Append(",");
                    }
                    writer.WriteLine(headBuilder);

                    //内容
                    for (int i = 0; i < dtExport.Rows.Count; i++)
                    {
                        bodyBuilder.Remove(0, bodyBuilder.Length);//移出

                        for (int j = 0; j < dtExport.Columns.Count; j++)
                        {
                            bodyBuilder.Append(prefix + dtExport.Rows[i][j].ToString());
                            if (j < dtExport.Columns.Count - 1) bodyBuilder.Append(",");
                        }
                        writer.WriteLine(bodyBuilder);
                    }
                }
            }
        }


        /// <summary>
        /// 导出到csv 追加模式
        /// </summary>
        /// <param name="dtExport">数据源</param>
        /// <param name="fullName">导出文件完全路径</param>
        /// <param name="needTab">是否需要在值前插入tab以将字段作为文本格式显示在excel中</param>
        public static void ExportToCsv2(DataTable dtExport, string fullName, bool needTab = false)
        {
            StringBuilder headBuilder = new StringBuilder();    ////先打印标头
            StringBuilder bodyBuilder = new StringBuilder();   ////行值
            var prefix = "";
            if (needTab)
            {
                prefix = "\t";
            }
            if (!File.Exists(fullName))
            {

                using (FileStream stream = new FileStream(fullName, FileMode.CreateNew))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                    {
                        //标头
                        for (int i = 0; i < dtExport.Columns.Count; i++)
                        {
                            headBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                            if (i < dtExport.Columns.Count - 1) headBuilder.Append(",");
                        }
                        writer.WriteLine(headBuilder);

                        //内容
                        for (int i = 0; i < dtExport.Rows.Count; i++)
                        {
                            bodyBuilder.Remove(0, bodyBuilder.Length);//移出

                            for (int j = 0; j < dtExport.Columns.Count; j++)
                            {
                                bodyBuilder.Append(prefix + dtExport.Rows[i][j].ToString());
                                if (j < dtExport.Columns.Count - 1) bodyBuilder.Append(",");
                            }
                            writer.WriteLine(bodyBuilder);
                        }
                    }
                }
            }
            else
            {
                // 追加模式打开文件
                using (FileStream stream = new FileStream(fullName, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                    {
                        ////标头
                        //for (int i = 0; i < dtExport.Columns.Count; i++)
                        //{
                        //    headBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                        //    if (i < dtExport.Columns.Count - 1) headBuilder.Append(",");
                        //}
                        //writer.WriteLine(headBuilder);

                        //内容
                        for (int i = 0; i < dtExport.Rows.Count; i++)
                        {
                            bodyBuilder.Remove(0, bodyBuilder.Length);//移出

                            for (int j = 0; j < dtExport.Columns.Count; j++)
                            {
                                bodyBuilder.Append(prefix + dtExport.Rows[i][j].ToString());
                                if (j < dtExport.Columns.Count - 1) bodyBuilder.Append(",");
                            }
                            writer.WriteLine(bodyBuilder);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 导出文件到txt,指定dateTime格式,wave0用到
        /// </summary>
        /// <param name="dtExport"></param>
        /// <param name="fullFileName"></param>
        /// <param name="spliter"></param>
        /// <param name="hasHead"></param>
        /// <param name="encoding"></param>
        public static void Export2Wave0File(DataTable dtExport, string fullFileName, string spliter = ",", bool hasHead = true, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            if (File.Exists(fullFileName)) File.Delete(fullFileName);


            StringBuilder headBuilder = new StringBuilder();    ////先打印标头
            StringBuilder bodyBuilder = new StringBuilder();   ////行值

            using (FileStream stream = new FileStream(fullFileName, FileMode.CreateNew))
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    if (hasHead)
                    {
                        //标头
                        for (int i = 0; i < dtExport.Columns.Count; i++)
                        {
                            headBuilder.Append(!string.IsNullOrEmpty(dtExport.Columns[i].Caption) ? dtExport.Columns[i].Caption : dtExport.Columns[i].ColumnName);
                            if (i < dtExport.Columns.Count - 1) headBuilder.Append(spliter);
                        }
                        writer.WriteLine(headBuilder);
                    }
                    //内容
                    for (int i = 0; i < dtExport.Rows.Count; i++)
                    {
                        bodyBuilder.Remove(0, bodyBuilder.Length);//移出

                        for (int j = 0; j < dtExport.Columns.Count; j++)
                        {
                            string cellValue = dtExport.Rows[i][j].ToString();
                            if (dtExport.Columns[j].DataType == typeof(DateTime) && !string.IsNullOrEmpty(dtExport.Rows[i][j].ToString()))
                            {

                                cellValue = new W3CDateTime(Convert.ToDateTime(dtExport.Rows[i][j])).ToString();
                            }
                            bodyBuilder.Append(cellValue);
                            if (j < dtExport.Columns.Count - 1) bodyBuilder.Append(spliter);
                        }
                        writer.WriteLine(bodyBuilder);
                    }
                }
            }
        }

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="savePath"></param>
        /// <param name="isEnd">是否结尾</param>
        /// <param name="endStr">收尾字符串</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool Export<T>(List<T> source, string savePath, bool isEnd, string endStr, out string errMsg)
        {
            errMsg = "";
            //int fileIndex = 1;
            //while (File.Exists(saveFileName))
            //{
            //    saveFileName = savePath.Substring(0, savePath.LastIndexOf('.') - 1) + "_" + fileIndex + savePath.Substring(savePath.LastIndexOf('.'), savePath.Length - savePath.LastIndexOf('.'));
            //    fileIndex++;
            //}
            if (!savePath.Contains(".csv"))
            {
                savePath = savePath + ".csv";
            }
            bool isExists = File.Exists(savePath);
            StreamWriter sw = null;
            FileStream fs = null;
            if (isExists)
            {
                sw = new StreamWriter(savePath, true, Encoding.Default);
            }
            else
            {
                fs = new FileStream(savePath, FileMode.OpenOrCreate);
                sw = new StreamWriter(fs, Encoding.Default);
            }
            try
            {
                //写入字段
                Type t = typeof(T);
                PropertyInfo[] infos = t.GetProperties();
                string firstline = "";
                if (!isExists)
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        object[] customerAttr = infos[i].GetCustomAttributes(typeof(ChineseName), false);
                        if (customerAttr != null && customerAttr.Length > 0 && customerAttr[0] is ChineseName)
                        {
                            ChineseName cName = customerAttr[0] as ChineseName;
                            firstline = firstline + cName.Name;
                            if (i < infos.Length - 1)
                            {
                                firstline = firstline + ",";
                            }
                        }
                        else
                        {
                            firstline = firstline + infos[i].Name;
                        }

                    }
                }
                //写入数值
                for (int row = 0; row < source.Count; row++)//一条数据代表一行
                {
                    string rowline = "";
                    for (int colome = 0; colome < infos.Length; colome++)//一条数据代表一列
                    {
                        rowline = rowline + infos[colome].GetValue(source[row], null).ToString();

                        if (colome < infos.Length - 1)
                        {
                            rowline = rowline + ",";
                        }
                    }
                    firstline = firstline + "\r\n" + rowline;
                }
                if (isEnd)
                {
                    firstline = firstline + endStr;
                }
                sw.Write(firstline);

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return true;
        }


        

    }
}

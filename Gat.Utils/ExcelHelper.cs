#region 文件头描述信息
/*----------------------------------------------------------------
           // Copyright (C) 2010 广州恒康信息科技有限公司
           // 版权所有。 
           //
           // 文件名：ExcelIO.cs
           // 文件功能描述：导入Excel数据或者导出数据到Excel
           //
           // 
           // 创建标识：Hernmmy  2011-05-16
           //
           // 修改标识：
           // 修改描述：
           //
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HPSF;
using System.Web;
using NPOI.SS.Util;

namespace Gat.Utils
{
    public class ExcelHelper
    {
        #region 属性
        private int _ReturnStatus;
        private string _ReturnMessage;
        private string _FileNotExist = "文件不存在";//GetRes.GetInfo("FileNotExist");//文件不存在
        private string _FileIsOpened = "导出文件时出错,文件可能正被打开";//GetRes.GetInfo("FileIsOpened");//导出文件时出错,文件可能正被打开
        private string _NotExistsExcel = "导出文件时出错,文件可能正被打开";// GetRes.GetInfo("NotExistsExcel");//导出文件时出错,文件可能正被打开
        private string _DSIsNull = "数据集为空"; //GetRes.GetInfo("DSIsNull");//数据集为空
        private static ExcelHelper eio;
        private Dictionary<string, int> columnIndexDir;
        private string connStr = string.Empty;
        private string connStr2007 = string.Empty;
        //private Dictionary<string, int> maxColumnIndexDir;
        private int maxColumnIndex;
        Microsoft.Office.Interop.Excel.Range range;
        /// <summary>
        /// 执行返回状态
        /// </summary>
        public int ReturnStatus
        {
            get { return _ReturnStatus; }
        }

        /// <summary>
        /// 执行返回信息
        /// </summary>
        public string ReturnMessage
        {
            get { return _ReturnMessage; }
        }

        #endregion

        #region 构造和实例化
        public ExcelHelper()
        {
        }
        public static ExcelHelper CreateInstace()
        {
            if (eio == null)
                eio = new ExcelHelper();
            return eio;
        }

        #endregion

        #region 获取连接excel的连接语句
        /// <summary>
        /// 获取连接excel的连接语句
        /// </summary>
        /// <returns></returns>
        private string GetConnExcelStr(string fileName)
        {
            //string connStr =   "Driver={Microsoft Excel Driver (*.xls)};" + "Driverid=790;" + "Dbq="+fileName+";";
            try
            {
                FileInfo file = new FileInfo(fileName);
                if (!file.Exists) { throw new Exception(_FileNotExist); }
                string extension = file.Extension;
                switch (extension)
                {
                    case ".xls":
                        connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                        connStr2007 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        break;
                    case ".xlsx":
                        connStr2007 = connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        //connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;\"";
                        break;
                    default:
                        connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                        connStr2007 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        break;
                }

            }
            catch
            {
                //"无法获取系统Office信息"
            }
            return connStr;
        }

        #endregion

        #region 导入EXCEL到DataSet

        /// <summary>
        /// 导入EXCEL到DataSet
        /// </summary>
        /// <param name="fileName">Excel全路径文件名</param>
        /// <param name="fieldNames">要查询的字段</param>
        /// <param name="whereSql">过滤语句</param>
        /// <param name="isOnlyGetFirstSheet">是否只取第一个工作表的数据</param>
        /// <returns>导入成功的DataSet</returns>
        public DataSet ImportExcel(string fileName, string fieldNames, string whereStr, bool isOnlyGetFirstSheet)
        {
            return ImportExcel(fileName, fieldNames, whereStr, false, isOnlyGetFirstSheet);
        }
        /// <summary>
        /// 导入EXCEL到DataSet
        /// </summary>
        /// <param name="fileName">Excel全路径文件名</param>
        /// <param name="fieldNames">要查询的字段</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="isOnlyGetOneData">是否只取一条记录(用于只取表架构)</param>
        /// <param name="isOnlyGetFirstSheet">是否只取第一个工作表的数据</param>
        /// <returns>导入成功的DataSet</returns>
        public DataSet ImportExcel(string fileName, string fieldNames, string whereStr, bool isOnlyGetOneData, bool isOnlyGetFirstSheet)
        {
            //判断是否安装EXCEL
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                _ReturnStatus = -1;
                _ReturnMessage = _NotExistsExcel;
                return null;
            }

            //判断文件是否被其他进程使用            
            Microsoft.Office.Interop.Excel.Workbook workbook;
            try
            {
                //workbook = xlApp.Workbooks.Open(fileName, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, 1, 0);
                workbook = xlApp.Workbooks.Open(fileName,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch
            {
                _ReturnStatus = -1;
                _ReturnMessage = _FileIsOpened;
                return null;
            }
            if (workbook.ReadOnly)
            {
                //HKMessageBox.Show(GetRes.GetInfo("FileIsOpened"), GetRes.GetInfo("formBaseTitle1"),
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisposeExcel(xlApp, xlApp.Workbooks, workbook, workbook.Worksheets[1]);
                return null;
            }

            //获得所有Sheet名称
            int n = workbook.Worksheets.Count;
            string[] SheetSet = new string[n];
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            if (isOnlyGetFirstSheet && n >= 1)
                SheetSet[0] = ((Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1]).Name;
            else
            {
                for (int i = 1; i <= n; i++)
                {
                    SheetSet[i - 1] = ((Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[i]).Name;
                }
            }

            //释放Excel相关对象
            #region
            //workbook.Close(null, null, null);
            //xlApp.Quit();
            //if (workbook != null)
            //{
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            //    workbook = null;
            //}
            //if (xlApp != null)
            //{
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            //    xlApp = null;
            //}
            //GC.Collect();
            #endregion

            DisposeExcel(xlApp, xlApp.Workbooks, workbook, workbook.Worksheets[1]);

            //把EXCEL导入到DataSet
            DataSet ds = new DataSet();
            GetConnExcelStr(fileName);
            if (string.IsNullOrEmpty(fieldNames))
                fieldNames = " * ";


            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                try
                {
                    FillData(isOnlyGetOneData, fieldNames, whereStr, SheetSet, n, ds, conn);
                }
                catch
                {
                    using (OleDbConnection exceptionConn = new OleDbConnection(connStr2007))
                    {
                        FillData(isOnlyGetOneData, fieldNames, whereStr, SheetSet, n, ds, exceptionConn);
                    }
                }

            }
            return ds;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="isOnlyGetOneData"></param>
        /// <param name="fieldNames"></param>
        /// <param name="whereStr"></param>
        /// <param name="SheetSet"></param>
        /// <param name="n"></param>
        /// <param name="ds"></param>
        /// <param name="conn"></param>
        private void FillData(bool isOnlyGetOneData, string fieldNames, string whereStr,
            string[] SheetSet, int n, DataSet ds, OleDbConnection conn)
        {
            StringBuilder strSql;
            OleDbDataAdapter da;
            conn.Open();
            for (int i = 1; i <= n; i++)
            {
                try
                {
                    strSql = new StringBuilder();
                    strSql.AppendLine("select ");
                    if (isOnlyGetOneData)
                        strSql.AppendLine(" top 1 ");
                    strSql.AppendLine(fieldNames + " from [" + SheetSet[i - 1] + "$]");
                    if (!string.IsNullOrEmpty(whereStr))
                        strSql.AppendLine("where " + whereStr);
                    da = new OleDbDataAdapter(strSql.ToString(), conn);
                    da.Fill(ds, SheetSet[i - 1]);
                    da.Dispose();
                }
                catch { }
            }
            conn.Close();
            conn.Dispose();
        }
        #endregion

        #region 把DataTable导出到EXCEL

        /// <summary>
        /// 把DataTable导出到EXCEL
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="dt">数据源表</param>
        /// <param name="saveFileName">Excel全路径文件名</param>
        /// <returns>导出是否成功</returns>
        public bool ExportExcel(DataTable dt, string saveFileName)
        {
            return ExportExcel(false, string.Empty, dt, saveFileName);
        }
        /// <summary>
        /// 把DataTable导出到EXCEL
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="dt">数据源表</param>
        /// <param name="saveFileName">Excel全路径文件名</param>
        /// <returns>导出是否成功</returns>
        public bool ExportExcel(bool isNeedWriteReportName, string reportName, DataTable dt, string saveFileName)
        {
            if (dt == null)
            {
                _ReturnStatus = -1;
                _ReturnMessage = _DSIsNull;
                return false;
            }

            bool fileSaved = false;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                _ReturnStatus = -1;
                _ReturnMessage = _NotExistsExcel;
                return false;
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet activesheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
            activesheet.Cells.Font.Size = 10;
            //Microsoft.Office.Interop.Excel.Range range;

            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;

            int iNeed = 0;
            if (isNeedWriteReportName)
            {
                iNeed += 1;
                activesheet.Cells[iNeed, 1] = reportName;//报名名称
                ((Microsoft.Office.Interop.Excel.Range)activesheet.Cells[iNeed, 1]).AutoFit();
                ((Microsoft.Office.Interop.Excel.Range)activesheet.Cells[iNeed, 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                ((Microsoft.Office.Interop.Excel.Range)activesheet.Cells[iNeed, 1]).Font.Size = 12;
                ((Microsoft.Office.Interop.Excel.Range)activesheet.Cells[iNeed, 1]).Font.Bold = true;
            }

            iNeed += 1;
            //写入字段
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                activesheet.Cells[iNeed, i + 1] = !string.IsNullOrEmpty(dt.Columns[i].Caption) ? dt.Columns[i].Caption : dt.Columns[i].ColumnName;
                range = (Microsoft.Office.Interop.Excel.Range)activesheet.Cells[iNeed, i + 1];
                range.Interior.ColorIndex = 15;
                range.Font.Bold = true;

            }
            iNeed += 1;
            //写入数值
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    activesheet.Cells[r + iNeed, i + 1] = dt.Rows[r][i].ToString();
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }
            //if (dt.Rows.Count > 1 && dt.Columns.Count > 1)
            //{
            //    range = activesheet.get_Range(activesheet.Cells[2, 1], activesheet.Cells[dt.Rows.Count + 2, dt.Columns.Count]);
            //    range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
            //}
            //if (dt.Rows.Count > 0)
            //{
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].ColorIndex = Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic;
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
            //}
            //if (dt.Columns.Count > 1)
            //{
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic;
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
            //}

            //保存文件
            if (!string.IsNullOrEmpty(saveFileName))
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                    fileSaved = true;
                }
                catch (Exception ex)
                {
                    fileSaved = false;
                    _ReturnStatus = -1;
                    _ReturnMessage = _FileIsOpened + "！\n" + ex.Message;
                }
            }
            else
            {
                fileSaved = false;
            }
            DisposeExcel(xlApp, workbooks, workbook, activesheet);
            return fileSaved;
        }
        #endregion
        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>   
        /// <param name="SheetName">要获取数据的工作表名称</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public DataTable RenderDataTableFromExcels(string strFileName, string SheetName, int HeaderRowIndex)
        {
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook;
                try
                {
                    workbook = new XSSFWorkbook(file);
                }
                catch (Exception ex)
                {
                    workbook = new HSSFWorkbook(file);
                }
                //  IWorkbook workbook = new HSSFWorkbook(file);
                ISheet sheet = workbook.GetSheet(SheetName);
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
        }
        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="workbook">要处理的工作薄</param>   
        /// <param name="SheetName">要获取数据的工作表名称</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public DataTable RenderDataTableFromExcel(IWorkbook workbook, string SheetName, int HeaderRowIndex)
        {
            ISheet sheet = workbook.GetSheet(SheetName);
            DataTable table = new DataTable();
            try
            {
                IRow headerRow = sheet.GetRow(HeaderRowIndex);
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum;

                #region 循环各行各列,写入数据到DataTable
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        ICell cell = row.GetCell(j);
                        if (cell == null)
                        {
                            dataRow[j] = null;
                        }
                        else
                        {
                            //dataRow[j] = cell.ToString();   
                            switch (cell.CellType)
                            {
                                case CellType.Blank:
                                    dataRow[j] = null;
                                    break;
                                case CellType.Boolean:
                                    dataRow[j] = cell.BooleanCellValue;
                                    break;
                                case CellType.Numeric:
                                    dataRow[j] = cell.ToString();
                                    break;
                                case CellType.String:
                                    dataRow[j] = cell.StringCellValue;
                                    break;
                                case CellType.Error:
                                    dataRow[j] = cell.ErrorCellValue;
                                    break;
                                case CellType.Formula:
                                default:
                                    dataRow[j] = "=" + cell.CellFormula;
                                    break;
                            }
                        }
                    }
                    table.Rows.Add(dataRow);
                    //dataRow[j] = row.GetCell(j).ToString();   
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                table.Clear();
                table.Columns.Clear();
                table.Columns.Add("出错了");
                DataRow dr = table.NewRow();
                dr[0] = ex.Message;
                table.Rows.Add(dr);
                return table;
            }
            finally
            {
                //sheet.Dispose();   
                workbook = null;
                sheet = null;
            }
            #region 清除最后的空行
            for (int i = table.Rows.Count - 1; i > 0; i--)
            {
                bool isnull = true;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j] != null)
                    {
                        if (table.Rows[i][j].ToString() != "")
                        {
                            isnull = false;
                            break;
                        }
                    }
                }
                if (isnull)
                {
                    table.Rows[i].Delete();
                }
            }
            #endregion
            return table;
        }
        /// <summary>读取excel      
        /// 默认第一行为表头，导入第一个工作表   
        /// </summary>      
        /// <param name="strFileName">excel文档路径</param>      
        /// <returns></returns>      
        public DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            // HSSFWorkbook hssfworkbook;
            IWorkbook hssfworkbook = null;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {

                try
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
                catch (Exception ex)
                {
                    file.Position = 0;
                    hssfworkbook = new HSSFWorkbook(file);
                }
                // hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            List<int> columns = new List<int>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                object obj = GetValueTypeForXLSX(header.GetCell(i) as ICell, hssfworkbook, i);
                if ((obj == null || obj.ToString() == string.Empty) && obj.ToString() != "Effective Date")
                {
                    dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    //continue;
                }
                else if (obj.ToString() == "Effective Date")
                { dt.Columns.Add(new DataColumn(obj.ToString(), typeof(DateTime))); }
                else
                { dt.Columns.Add(new DataColumn(obj.ToString())); }
                columns.Add(i);
            }
            //数据
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                foreach (int j in columns)
                {
                    if (sheet.GetRow(i) != null)
                    {
                        dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as ICell, hssfworkbook, j);
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                }
                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取单元格类型(xlsx)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLSX(ICell cell, IWorkbook hssfworkbook, int i)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC: 
                    short format = cell.CellStyle.DataFormat;
                    if (DateUtil.IsCellDateFormatted(cell))
                    //if(cell.ColumnIndex==4)
                    {
                        //npoi在2.51版本之前使用cell.DateCellValue读取日期可能会报未将对象引用设置到对象的实例
                        //升级到2.51版本后改为使用下面的方法来获取日期
                        DateTime date = DateTime.FromOADate(cell.NumericCellValue);
                        //DateTime date = cell.DateCellValue;
                        string re = date.ToString("yyyy-MM-dd HH:mm:ss");
                        return re;
                    }
                    else
                        return cell.NumericCellValue;

                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }


        /// <summary>
        /// HSSF导出数据到内存流 不支持一个sheet导出超过65535
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strExcelFileName"></param>
        /// <returns></returns>
        public MemoryStream GridToExcelByNPOI(DataTable dt, string strExcelFileName)
        {
            MemoryStream ms = new MemoryStream();
            HSSFWorkbook workbook = null;
            try
            {
                workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                //headerfont.Boldweight = (short)FontBoldWeight.Bold;
                headerfont.IsBold = true;
                HeadercellStyle.SetFont(headerfont);

                IDataFormat dataformat = workbook.CreateDataFormat();


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                foreach (DataColumn item in dt.Columns)
                {
                    ICell cell = headerRow.CreateCell(icolIndex);
                    cell.SetCellValue(item.ColumnName);
                    cell.CellStyle = HeadercellStyle;
                    icolIndex++;
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.IsBold = false;
                //cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;
                foreach (DataRow Rowitem in dt.Rows)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    foreach (DataColumn Colitem in dt.Columns)
                    {

                        ICell cell = DataRow.CreateCell(iCellIndex);
                        cell.SetCellValue(Rowitem[Colitem].ToString());
                        cell.CellStyle = cellStyle;
                        iCellIndex++;
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                //写Excel
                //  FileStream file = new FileStream(strExcelFileName, FileMode.Create,FileAccess.Write);
                // workbook.s
                workbook.Write(ms);
                ms.Flush();
                //  ms.Close();
                ms.Position = 0;
                //  MessageBox.Show(m_Common_ResourceManager.GetString("Export_to_excel_successfully"), m_Common_ResourceManager.GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //  ILog log = LogManager.GetLogger("Exception Log");
                //log.Error(ex.Message + Environment.NewLine + ex.StackTrace);
                //记录AuditTrail
                //  CCFS.Framework.BLL.AuditTrailBLL.LogAuditTrail(ex);

                //  MessageBox.Show(m_Common_ResourceManager.GetString("Export_to_excel_failed"), m_Common_ResourceManager.GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw ex;
            }
            finally
            {
                workbook = null;
            }
            return ms;
        }

        /// <summary>
        /// XSSF导出数据 可支持导出超过65535
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strExcelFileName"></param>
        /// <returns></returns>
        public MemoryStream GridToExcelXSSFByNPOI(DataTable dt, string strExcelFileName)
        {
            XSSFWorkbook workbook = null;
            MemoryStream ms = new MemoryStream();
            try
            {
                var count = dt.Rows.Count;
                var sheetMaxCount = 1048000;//每个sheet最大只能1048575
                var sheetCount = count / sheetMaxCount;
                workbook = new XSSFWorkbook();
                for (int i = 0; i < sheetCount + 1; i++)
                {

                    ISheet sheet = workbook.CreateSheet("Sheet" + (i + 1));

                    ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                    HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //字体
                    NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                    //headerfont.Boldweight = (short)FontBoldWeight.Bold;
                    headerfont.IsBold = true;
                    HeadercellStyle.SetFont(headerfont);

                    IDataFormat dataformat = workbook.CreateDataFormat();


                    //用column name 作为列名
                    int icolIndex = 0;
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn item in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(item.ColumnName);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }

                    ICellStyle cellStyle = workbook.CreateCellStyle();

                    //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                    NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                    cellfont.IsBold = false;
                    //cellfont.Boldweight = (short)FontBoldWeight.Normal;
                    cellStyle.SetFont(cellfont);

                    //建立内容行
                    int iRowIndex = 1;
                    int iCellIndex = 0;
                    var startRowIndex = i * sheetMaxCount;
                    var maxRowIndex = 0;
                    if (sheetCount > 0 && i < sheetCount)
                    {
                        maxRowIndex = (i + 1) * sheetMaxCount;
                    }
                    else
                    {
                        maxRowIndex = dt.Rows.Count;
                    }
                    for (int l = startRowIndex; l < maxRowIndex; l++)
                    {
                        DataRow item = dt.Rows[l];
                        IRow DataRow = sheet.CreateRow(iRowIndex);
                        foreach (DataColumn Colitem in dt.Columns)
                        {

                            ICell cell = DataRow.CreateCell(iCellIndex);
                            cell.SetCellValue(item[Colitem].ToString());
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                        iCellIndex = 0;
                        iRowIndex++;
                    }

                    //自适应列宽度
                    for (int j = 0; j < icolIndex; j++)
                    {
                        sheet.AutoSizeColumn(j);
                    }
                }
                ms.Position = 0;
                workbook.Write(ms);
                ms.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                workbook = null;
            }
            return ms;
        }
        //static void RenderToBrowser(MemoryStream ms, HttpContext context, string fileName)
        //{
        //    if (context.Request.Browser.Browser == "IE")
        //        fileName = HttpUtility.UrlEncode(fileName);
        //    context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
        //    context.Response.BinaryWrite(ms.ToArray());
        //}
        #region 回写Excel状态

        /// <summary>
        /// 回写Excel状态
        /// </summary>
        /// <param name="ExplortInfoDT"></param>
        /// <param name="fileName"></param>
        /// <param name="rowNoName">行号所在的列名</param>
        /// <param name="tagFieldName">标记列</param>
        public void SetNotExplortExcelBackColor(DataTable ExplortInfoDT, string fileName, string rowNoName, string tagFieldName, string errorMsgName)
        {
            if (columnIndexDir == null || columnIndexDir.Count == 0)
                GetColumnsIndex(fileName);
            ExplortInfoDT.DefaultView.RowFilter = "ErrorMsg<>''";
            if (ExplortInfoDT == null || ExplortInfoDT.DefaultView.Count == 0)
            {
                _ReturnStatus = -1;
                _ReturnMessage = _DSIsNull;
                return;
            }

            bool fileSaved = false;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            //判断文件是否被其他进程使用
            try
            {
                if (xlApp == null)
                {
                    _ReturnStatus = -1;
                    _ReturnMessage = _NotExistsExcel;
                }
                //Microsoft.Office.Interop.Excel.Workbook workbook = xlApp.Workbooks.Open(fileName, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, 1, 0);
                Microsoft.Office.Interop.Excel.Workbook workbook = xlApp.Workbooks.Open(fileName,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Microsoft.Office.Interop.Excel.Worksheet activesheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];

                if (workbook.ReadOnly)
                {
                    //HKMessageBox.Show(GetRes.GetInfo("FileIsOpened"), GetRes.GetInfo("formBaseTitle1"),
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisposeExcel(xlApp, xlApp.Workbooks, workbook, workbook.Worksheets[1]);
                    return;
                }
                string errorMessage = string.Empty;
                foreach (DataRowView drv in ExplortInfoDT.DefaultView)
                {
                    SetRangeBackground(activesheet, Convert.ToInt32(drv[rowNoName]) + 1, columnIndexDir[drv[tagFieldName].ToString()] + 1, maxColumnIndex + 2, ColorIndexEnum.红色.GetHashCode(), drv[errorMsgName].ToString());
                }
                //保存文件
                if (!string.IsNullOrEmpty(fileName))
                {
                    try
                    {
                        workbook.CheckCompatibility = false;//不弹出兼容性检查的提示
                        workbook.Saved = true;
                        workbook.Save();
                        fileSaved = true;
                    }
                    catch (Exception ex)
                    {
                        fileSaved = false;
                        _ReturnStatus = -1;
                        _ReturnMessage = _FileIsOpened + "！\n" + ex.Message;
                    }
                }
                else
                {
                    fileSaved = false;
                }
                DisposeExcel(xlApp, xlApp.Workbooks, workbook, activesheet);
            }
            catch
            {
                _ReturnStatus = -1;
                _ReturnMessage = _FileIsOpened;
            }

            columnIndexDir.Clear();//处理完之后记得清空，也可以不处理
        }

        #endregion

        #region 释放Excel资源

        /// <summary>
        /// 释放Excel资源
        /// </summary>
        /// <param name="xlApp"></param>
        /// <param name="worksheet"></param>
        /// <param name="workbooks"></param>
        /// <param name="workbook"></param>
        private void DisposeExcel(Microsoft.Office.Interop.Excel.Application xlApp, Microsoft.Office.Interop.Excel.Workbooks workbooks, Microsoft.Office.Interop.Excel.Workbook workbook, Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            try
            {
                int worksheetsCount = workbook.Worksheets.Count;
                workbook.Close(null, null, null);
                //释放Excel对应的对象
                if (range != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                    range = null;
                }
                if (worksheet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    worksheet = null;
                }
                for (int i = 1; i <= worksheetsCount; i++)
                {
                    if (workbook.Worksheets[i] != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook.Worksheets[i]);
                        //workbook.Worksheets[i] = null;
                        workbook.Worksheets.Delete();
                    }
                }
                if (workbook != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }
                if (workbooks != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
                    workbooks = null;
                }
                xlApp.Application.Workbooks.Close();
                xlApp.Quit();
                if (xlApp != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                    xlApp = null;
                }

            }
            catch
            { }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        #region 设置单元格的背景色

        ///<summary> 
        ///设置单元格的背景色
        ///</summary> 
        ///<param name="worksheet">当前Sheet</param>
        ///<param name="rowIndex">开始行</param>
        ///<param name="columnIndex">开始列</param>
        ///<param name="maxColumnIndex">当前sheet的列最大序号</param>
        ///<param name="columnIndex">颜色编号</param> 
        ///<param name="errorMessage">出错信息</param>
        public void SetRangeBackground(Microsoft.Office.Interop.Excel.Worksheet worksheet, int rowIndex, int columnIndex, int maxColumnIndex, int colorIndex, string errorMessage)
        {
            //设背景色
            range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, columnIndex];
            range.Font.ColorIndex = colorIndex;
            ////写错误信息
            worksheet.Cells[rowIndex, maxColumnIndex] = errorMessage;
        }

        #endregion

        #region 获取列名顺序及最大列号

        /// <summary>
        /// 获取列名顺序及最大列号
        /// </summary>
        /// <param name="fileName"></param>
        private void GetColumnsIndex(string fileName)
        {
            columnIndexDir = new Dictionary<string, int>();
            //maxColumnIndexDir = new Dictionary<string, int>();
            DataSet columnInfoDS = ImportExcel(fileName, string.Empty, string.Empty, true, true);
            if (columnInfoDS != null && columnInfoDS.Tables.Count > 0)
            {
                foreach (DataTable dt in columnInfoDS.Tables)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        columnIndexDir.Add(dt.Columns[i].ColumnName, i);
                    }
                    maxColumnIndex = dt.Columns.Count;
                    //maxColumnIndexDir.Add(dt.TableName, dt.Columns.Count);
                }
            }
        }

        #endregion

        /// <summary>
        /// 常用颜色定义,对就Excel中颜色名
        /// </summary>
        public enum ColorIndexEnum
        {
            无色 = -4142,
            自动 = -4105,
            黑色 = 1,
            褐色 = 53,
            橄榄 = 52,
            深绿 = 51,
            深青 = 49,
            深蓝 = 11,
            靛蓝 = 55,
            灰色80 = 56,
            深红 = 9,
            橙色 = 46,
            深黄 = 12,
            绿色 = 10,
            青色 = 14,
            蓝色 = 5,
            蓝灰 = 47,
            灰色50 = 16,
            红色 = 3,
            浅橙色 = 45,
            酸橙色 = 43,
            海绿 = 50,
            水绿色 = 42,
            浅蓝 = 41,
            紫罗兰 = 13,
            灰色40 = 48,
            粉红 = 7,
            金色 = 44,
            黄色 = 6,
            鲜绿 = 4,
            青绿 = 8,
            天蓝 = 33,
            梅红 = 54,
            灰色25 = 15,
            玫瑰红 = 38,
            茶色 = 40,
            浅黄 = 36,
            浅绿 = 35,
            浅青绿 = 34,
            淡蓝 = 37,
            淡紫 = 39,
            白色 = 2
        }

        /// <summary>      
        /// DataTable导出到Excel文件      
        /// </summary>      
        /// <param name="dtSource">源DataTable</param>      
        /// <param name="strHeaderText">表头文本</param>      
        /// <param name="strFileName">保存位置</param>   
        /// <param name="strSheetName">工作表名称</param>   
        /// <Author>CallmeYhz 2015-11-26 10:13:09</Author>      
        public void Export(DataTable dtSource, string strHeaderText, string strFileName, string strSheetName, string[] oldColumnNames, string[] newColumnNames)
        {
            if (strSheetName == "")
            {
                strSheetName = "Sheet";
            }
            using (MemoryStream ms = Export(dtSource, strHeaderText, strSheetName, oldColumnNames, newColumnNames))
            {
                using (FileStream fs = new FileStream(strFileName + "//" + strSheetName + ".xls", FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>      
        /// DataTable导出到Excel的MemoryStream      
        /// </summary>      
        /// <param name="dtSource">源DataTable</param>      
        /// <param name="strHeaderText">表头文本</param>      
        /// <param name="strSheetName">工作表名称</param>   
        /// <Author>CallmeYhz 2015-11-26 10:13:09</Author>      
        public MemoryStream Export(DataTable dtSource, string strHeaderText, string strSheetName, string[] oldColumnNames, string[] newColumnNames)
        {
            if (oldColumnNames.Length != newColumnNames.Length)
            {
                return new MemoryStream();
            }
            HSSFWorkbook workbook = new HSSFWorkbook();
            //HSSFSheet sheet = workbook.CreateSheet();// workbook.CreateSheet();   
            ISheet sheet = workbook.CreateSheet(strSheetName);

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "http://....../";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                if (HttpContext.Current.Session["realname"] != null)
                {
                    si.Author = HttpContext.Current.Session["realname"].ToString();
                }
                else
                {
                    if (HttpContext.Current.Session["username"] != null)
                    {
                        si.Author = HttpContext.Current.Session["username"].ToString();
                    }
                }                                       //填加xls文件作者信息      
                si.ApplicationName = "NPOI";            //填加xls文件创建程序信息      
                si.LastAuthor = "OA系统";           //填加xls文件最后保存者信息      
                si.Comments = "OA系统自动创建文件";      //填加xls文件作者信息      
                si.Title = strHeaderText;               //填加xls文件标题信息      
                si.Subject = strHeaderText;              //填加文件主题信息      
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            #region 取得列宽
            int[] arrColWidth = new int[oldColumnNames.Length];
            for (int i = 0; i < oldColumnNames.Length; i++)
            {
                arrColWidth[i] = Encoding.GetEncoding(936).GetBytes(newColumnNames[i]).Length;
            }
            /* 
            foreach (DataColumn item in dtSource.Columns) 
            { 
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length; 
            } 
             * */

            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < oldColumnNames.Length; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][oldColumnNames[j]].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
                /* 
                for (int j = 0; j < dtSource.Columns.Count; j++) 
                { 
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length; 
                    if (intTemp > arrColWidth[j]) 
                    { 
                        arrColWidth[j] = intTemp; 
                    } 
                } 
                 * */
            }
            #endregion
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet(strSheetName + ((int)rowIndex / 65535).ToString());
                    }

                    #region 表头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;
                        //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));   
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                    }
                    #endregion


                    #region 列头及样式
                    {
                        //HSSFRow headerRow = sheet.CreateRow(1);   
                        IRow headerRow = sheet.CreateRow(1);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);

                        //for (int i = 0; i < oldColumnNames.Length; i++)
                        //{
                        //      headerRow.CreateCell(i).SetCellValue(newColumnNames[i]);
                        //      headerRow.GetCell(i).CellStyle = headStyle;
                        //      设置列宽   
                        //    sheet.SetColumnWidth(i, (arrColWidth[i] + 1) * 256);
                        //}
                        for (int i = 0; i < oldColumnNames.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(newColumnNames[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            sheet.AutoSizeColumn(i);
                        }
                        /* 
                        foreach (DataColumn column in dtSource.Columns) 
                        { 
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName); 
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle; 
 
                            //设置列宽    
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256); 
                        } 
                         * */
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion


                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                //foreach (DataColumn column in dtSource.Columns)   
                for (int i = 0; i < oldColumnNames.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);

                    string drValue = row[oldColumnNames[i]].ToString();

                    switch (dtSource.Columns[oldColumnNames[i]].DataType.ToString())
                    {
                        case "System.String"://字符串类型      
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型      
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示      
                            break;
                        case "System.Boolean"://布尔型      
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型      
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型      
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理      
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();   
                sheet = null;
                workbook = null;
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet      
                return ms;
            }
        }
    }
}
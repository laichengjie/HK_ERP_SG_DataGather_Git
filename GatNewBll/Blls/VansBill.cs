using GatNewBll.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Net.FtpClient;
using FtpClientNet = System.Net.FtpClient.FtpClient;
using System.Text.RegularExpressions;
using Gat.Utils;
using GatSqlSugarDAL.Services;
using GatNewBll.Models.On;
using SqlSugar;
using GatEntities.DbModels;

namespace GatNewBll.Blls
{
    public class VansBill : BaseBll
    {
        #region 查询

        /// <summary>
        /// 销售数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="totalAmount"></param>
        /// <param name="isNeedPage"></param>
        /// <returns></returns>
        public List<GatVansPosSale> SelectPage(int solutionID, string shopCode, string startBillTime, string endBillTime, string startBillDate, string endBillDate, string materialCode, string sizeCode, int currentPage, int itemsPerPage, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, bool isNeedPage = true)
        {
            GatVansPossaleManager manager = new GatVansPossaleManager();
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }

            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "itemNo", ConditionalType = ConditionalType.LikeLeft, FieldValue = materialCode });
            }

            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "WaistSize", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            totalQty = 0;
            totalAmount = 0;
            List<GatVansPosSale> PosSalelist = new List<GatVansPosSale>();
            if (isNeedPage)
            {
                var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
                var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillDate).ThenBy(s => s.ShopCode);
                totalCount = p.TotalCount;
                if (totalCount > 0)
                {
                    totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
                    totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SettlementAmount);
                }
                PosSalelist = result.ToList();
            }
            else
            {
                List<OrderByModel> orderByM = new List<OrderByModel>();
                orderByM.Add(new OrderByModel() { FieldName = "ShopCode", OrderByType = OrderByType.Asc });
                orderByM.Add(new OrderByModel() { FieldName = "BillDate", OrderByType = OrderByType.Asc });
                var result = manager.AsQueryable().Where(conModels).OrderBy(orderByM);
                totalCount = result.Count();
                if (totalCount > 0)
                {
                    //totalQty = result.Sum(s => s.Qty);
                    //totalAmount = result.Sum(s => s.SettlementAmount);
                }
                PosSalelist = result.ToList();
            }
            return PosSalelist;
        }
        /// <summary>
        /// 库存数据
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="shopCode"></param>
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="isNeedPage"></param>
        /// <returns></returns>
        public List<GatVansMeterialinv> SelectInvPage(int solutionID, string shopCode, string materialCode, string sizeCode, int currentPage, int itemsPerPage, ref int totalCount, ref decimal totalQty, bool isNeedPage = true)
        {
            GatVansInvManager manager = new GatVansInvManager();
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            //conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });

            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }

            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "itemNo", ConditionalType = ConditionalType.LikeLeft, FieldValue = materialCode });
            }

            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "WaistSize", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            totalQty = 0;
            List<GatVansMeterialinv> PosSalelist = new List<GatVansMeterialinv>();
            if (isNeedPage)
            {
                var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
                var result = manager.GetPageList(conModels, p).OrderBy(s => s.ShopCode);
                totalCount = p.TotalCount;
                if (totalCount > 0)
                {
                    totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Quantity);

                }
                PosSalelist = result.ToList();
            }
            else
            {
                List<OrderByModel> orderByM = new List<OrderByModel>();
                orderByM.Add(new OrderByModel() { FieldName = "ShopCode", OrderByType = OrderByType.Asc });
                orderByM.Add(new OrderByModel() { FieldName = "itemNo", OrderByType = OrderByType.Asc });
                var result = manager.AsQueryable().Where(conModels).OrderBy(orderByM);
                totalCount = result.Count();
                if (totalCount > 0)
                {
                    //totalQty = result.Sum(s => s.Qty);
                    //totalAmount = result.Sum(s => s.SettlementAmount);
                }
                PosSalelist = result.ToList();
            }
            return PosSalelist;
        }
        #endregion




        #region 保护属性

        private static readonly object lockObj = new object(); //锁对象
        private static readonly object lockObj2 = new object();//上传文件时的销对象
        private string _Notupload, _Haveupload, _UnCompress; //待上传，已上传，未压缩的文件
        private IDictionary<string, string> columns4Export = null;   //导出列

        private string saleFileName = "SALES-{0}-{1}.txt";//{0}：每家经销商不同 {1}时间戳
        private string invFileName = "INVSNAP-{0}-{1}.txt";//{0}：每家经销商不同 {1}时间戳
        private string compressFileName = "\\{0}-{1}-auto.red";//{0}：每家经销商不同 {1}时间戳 Type：上传方式manual（表示手动上传）  auto（表示自动上传）
        private string spliterStr = string.Empty;
        public string OutputDirectory = string.Empty;

        SysSGLogManager SglogManager = new SysSGLogManager();

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void VFInitialization()
        {
            string devName = "VF";
            _Notupload = Path.Combine(Helper.ExecutePath, devName);
            _Haveupload = Path.Combine(Helper.ExecutePath, devName, "Uploaded");
            _UnCompress = Path.Combine(Helper.ExecutePath, devName, "UnCompress");
            if (!Directory.Exists(_Notupload)) Directory.CreateDirectory(_Notupload);
            if (!Directory.Exists(_Haveupload)) Directory.CreateDirectory(_Haveupload);
            if (!Directory.Exists(_UnCompress)) Directory.CreateDirectory(_UnCompress);
            spliterStr = GetCharEight();
            InitColumns4Export();
        }


        public ReportMap GetVendorReportPath(int solutionId)
        {
            GatSolutionManager manager = new GatSolutionManager();
            var solution = manager.GetById(solutionId);
            if (solution != null)
            {
                int? VendorID = solution.VendorID;
                BasVendorManager VendorManager = new BasVendorManager();
                var Vendor = VendorManager.GetById(VendorID);
                if (Vendor != null)
                { return (ReportMap)(Vendor.ReportPath ?? 1);
                }
            }
            return ReportMap.Nike;
        }

        /// <summary>
        /// 推送Vans品牌数据
        /// </summary>
        /// <param name="solutionId">方案</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="CollectMethod">采集类型（0正常采集，1补采，2重新上传）</param>
        /// <returns></returns>
        public HandleArgs VansPush(int solutionId, DateTime startTime, DateTime endTime, int CollectMethod)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                WriteSglogManager(0, "", "开始任务", "VansPush");
                VFInitialization();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                OutputDirectory = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报FTP上传参数"
                    };
                }
                WriteSglogManager(0, "", "开始获取数据", "PushService");
                DataSet dataList = GetInvData(solutionId, startTime, endTime, CollectMethod);
                WriteSglogManager(0, "", "获取数据成功", "PushService");
                if (dataList.Tables.Count > 0)
                {
                    PushService(dataList);
                }
                else
                {
                    WriteSglogManager(0, "", "无同步数据", "PushService");
                }

                SysLogManager logManager = new SysLogManager();
                //记录请求日志syslog
                SysLog log = new SysLog()
                {
                    CompanyID = solution.CompanyID,
                    CardID = solution.CardID,
                    IsSucceed = true,
                    LogType = Convert.ToByte(LogType.Gat),
                    SolutionID = solution.Id,
                    VendorID = solution.VendorID,
                    ModifyDTM = DateTime.Now,
                    Script = string.Format("EXEC sp_SGIS_Vans_GetPosSaleInv @SolutionId  = {0},@CollectMethod = {1},@StartTime ={1},@EndTime ={1}", solutionId, CollectMethod, startTime, endTime),
                    Remark = "VANS数据提报Repost"
                };
                logManager.Insert(log);
                return result;
            }
            catch (Exception ex)
            {
                result.Status = HandleStatus.Fail;
                result.Exception = ex;
                result.Message = ex.Message;
                return result;
            }
        }


        public void WriteSglogManager(int SolutionId, string CompanyID, string Remark, string Message)
        {
            try
            {
                SysSGLog log = new SysSGLog()
                {
                    SolutionId = SolutionId,
                    CompanyID = CompanyID,
                    Remark = Remark,
                    CardName = "VANS",
                    Message = Message,
                    ModifyDTM = DateTime.Now,
                };
                SglogManager.Insert(log);
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// 获取提报库存数据
        /// </summary>
        /// <param name="solutionId">方案</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="CollectMethod">采集类型（0正常采集，1补采，2重新上传）</param>
        /// <returns></returns>
        private DataSet GetInvData(int solutionId, DateTime startTime, DateTime endTime, int CollectMethod)
        {
            DataSet ds = new DataSet();
            try
            {
                List<SugarParameter> parameters = new List<SugarParameter>();
                string sql = "sp_SGIS_Vans_GetPosSaleInv";
                ds = ErpDb.Ado.UseStoredProcedure().GetDataSetAll(sql, new { SolutionId = solutionId, CollectMethod = CollectMethod, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取库存数据异常");
            }
            return ds;
        }

        public void PushService(DataSet dsInvData)
        {
            lock (lockObj)
            {
               
                string EmporiumName = "BJYM";
                DateTime dateNow = DateTime.Now; //当前服务器时间
                ////采集之前先将以前没上传的文件上传上去
                //this.PostFilesAsync();
                PostFiles(null);
                ////删除历史文件
                DeleteHisFiles(_Haveupload, null, dateNow);
                int PosCount = dsInvData.Tables[0].Rows.Count;//每日POS数据
                int quarterYearPosCount = dsInvData.Tables[1].Rows.Count;//季度POS数据
                int StockCount = dsInvData.Tables[2].Rows.Count;
               
                //if (LogOperation) this.Logger.InfoFormat("{0}查询数据结束，每日POS销售共{1}笔数据,季度POS销售共{2}笔数据,静态库存共{3}笔数据。", this.FullDevName, PosCount, quarterYearPosCount, StockCount);
                WriteSglogManager(0, "", "PushService", string.Format("每日POS销售共{0}笔数据,季度POS销售共{1}笔数据,静态库存共{2}笔数据。", PosCount, quarterYearPosCount, StockCount));
                if (PosCount > 0 || StockCount > 0 || quarterYearPosCount > 0)
                {
                    string clientCode = EmporiumName;
                    string dtstr = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string _saleFileName = Path.Combine(_UnCompress, string.Format(saleFileName, clientCode, dtstr));
                    string _quarterFileName = Path.Combine(_UnCompress, string.Format(saleFileName, clientCode, DateTime.Now.AddMinutes(1).ToString("yyyyMMddHHmmss")));
                    string _invFileName = Path.Combine(_UnCompress, string.Format(invFileName, clientCode, dtstr));
                    string _compressFileName = string.Format(compressFileName, clientCode, DateTime.Now.ToString("yyyyMMddHHmmss"));

                    if (PosCount > 0)
                        ExportToCsv(dsInvData, 0, "Pos", _saleFileName);
                    if (quarterYearPosCount > 0)
                        ExportToCsv(dsInvData, 1, "季度Pos", _invFileName);
                    if (StockCount > 0)
                        ExportToCsv(dsInvData, 2, "库存", _invFileName);

                    dsInvData.Clear();
                    dsInvData = null;
                    System.GC.Collect();

                    CompressFile compressFile = new CompressFile();
                    compressFile.ZIPDirectoryName = _Notupload;
                    compressFile.ZIPName = _compressFileName;
                    compressFile.ZIPFileList = new List<string> { _saleFileName, _invFileName, _quarterFileName };
                    string compressReturnMsg = compressFile.CompressReturnMsg(true);
                    WriteSglogManager(0, "", "PushService", string.Format("文件压缩结果{0}", compressReturnMsg));

                    //上传到FTP服务器
                    if (string.IsNullOrEmpty(OutputDirectory))
                    {
                        WriteSglogManager(0, "", "PushService", "没有配置Ftp，不上传文件");
                    }
                    else
                    {
                        WriteSglogManager(0, "", "PushService", "开始异步上传文件");
                        //this.PostFilesAsync();
                        PostFiles(null);
                    }
                }
            }
        }

        #region 辅助方法

        /// <summary>
        /// 获取 char(8) 
        /// </summary>
        /// <returns></returns>
        private string GetCharEight()
        {
            string result = string.Empty;
            try
            {
                result = ErpDb.Ado.SqlQuery<string>($@" SELECT  char(8) ").FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }



        //生成文件保存到本地
        private void ExportToCsv(DataSet dsInvData, int index, string dataName, string fileName)
        {
            WriteSglogManager(0, "", "ExportToCsv", string.Format("开始将{0}数据导出到txt...", dataName));
            this.FixDataTable(dsInvData.Tables[index]);
            ExportHelper.ExportToText(dsInvData.Tables[index], fileName, spliter: spliterStr, exportHeader: false);
            WriteSglogManager(0, "", "ExportToCsv", string.Format("导入{0}数据到txt完毕", dataName));
           
        }

        //导出数据到Excel之前，先给各列设置标题
        private void FixDataTable(DataTable dtExport)
        {
            if (dtExport != null)
            {
                foreach (KeyValuePair<string, string> kv in columns4Export)
                {
                    if (dtExport.Columns[kv.Key] != null) dtExport.Columns[kv.Key].Caption = kv.Value;
                }
                if (dtExport.Columns["LogFileName"] != null) dtExport.Columns.Remove("LogFileName");
            }
        }

        //文件名由存储过程返回
        private string GetSaveFileName(string FileName)
        {
            //根据LogType命名导出文件，1正常采集，3增量采集
            string tempfileName = string.Empty;
            //tempfileName += Plan != null ? Plan.CompanyID + "_" : "";
            tempfileName += FileName + "_" + DateTime.Now.ToString("yyyyMMddHH");
            tempfileName += ".csv";

            return tempfileName;
        }

        //初始化导出列
        private void InitColumns4Export()
        {
            if (columns4Export == null) columns4Export = new Dictionary<string, string>();
        }

        //同步上载指定文件到目标FTP
        private void PostFile(FtpClientNet client, string sourceFileName)
        {
            try
            {
                WriteSglogManager(0, "", "PostFile", string.Format("开始上传文件『{0}』...", sourceFileName));
                client.OpenUpload(sourceFileName, "/"); //上传文件
                MoveFile(sourceFileName);
                WriteSglogManager(0, "", "PostFile", string.Format("上传文件『{0}』完成", sourceFileName));
            }
            catch (Exception ex)
            {
                WriteSglogManager(0, "", "PostFile", string.Format("上传文件『{0}』失败，消息：{1}{2}{3}{4}", sourceFileName, ex.Message, Environment.NewLine, ex.InnerException != null ? ex.InnerException.Message + Environment.NewLine : string.Empty, ex.StackTrace));
            }
            finally
            {
                if (!client.IsDisposed)
                {
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// 将上传成功的文件移至已上传文件夹
        /// </summary>
        /// <param name="fileName"></param>
        private void MoveFile(string fileName)
        {
            string name = Path.GetFileName(fileName);
            string destFileName = Path.Combine(_Haveupload, name);
            if (File.Exists(destFileName)) File.Delete(destFileName);
            System.IO.File.Move(fileName, destFileName);
        }
        //同步上传未上传文件夹中所有的文件
        private void PostFiles(object state)
        {
            lock (lockObj2)
            {
                WriteSglogManager(0, "", "PostFiles", "开始上传文件");
                FtpClientNet client = null;
                try
                {
                    string[] ftpInfoes = OutputDirectory.Split(';'); //Plan.OutputDirectory.Split(';');
                    client = new FtpClientNet();
                    client.Host = ftpInfoes[0];
                    client.Credentials = new System.Net.NetworkCredential(ftpInfoes[2], ftpInfoes[3]);
                    if (ftpInfoes.Length > 4)
                    {
                        if (ftpInfoes[4] == "PORT") client.DataConnectionType = System.Net.FtpClient.FtpDataConnectionType.PORT;
                        if (ftpInfoes[4] == "PASV") client.DataConnectionType = System.Net.FtpClient.FtpDataConnectionType.PASV;

                    }
                    if (!string.IsNullOrEmpty(ftpInfoes[1])) client.Port = Convert.ToInt32(ftpInfoes[1]);

                    DirectoryInfo directory = new DirectoryInfo(_Notupload);
                    FileInfo[] files = directory.GetFiles();

                    if (files != null)
                    {
                        foreach (FileInfo fi in files) PostFile(client, fi.FullName);
                    }
                    WriteSglogManager(0, "", "PostFiles", "上传文件结束");
                }
                catch (Exception ex)
                {
                    WriteSglogManager(0, "", "PostFiles", string.Format("上传文件失败，消息：{0}", ex.Message));
                }
                finally
                {
                    if (client != null) client.Dispose();
                }
            }
        }

        //异步上传未上传文件夹中所有的文件
        private void PostFilesAsync()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(PostFiles), null);
        }
        /// <summary>
        /// 删除指定目录的历史数据
        /// </summary>
        /// <param name="path">指定目录</param>
        /// <param name="filePattern">文件匹配</param>
        /// <param name="dateNow">当前时间</param>
        public void DeleteHisFiles(string path, string filePattern = null, Nullable<DateTime> dateNow = null)
        {
            try
            {
                WriteSglogManager(0, "", "DeleteHisFiles", "开始删除历史文件");
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] files = directory.GetFiles();
                if (files == null || files.Length == 0) return;

                // 取删除文件的时间点，默认删除60天前的文件记录
                int ago = 60;
                //string s_Ago = PCBusiness.GetParameter("G0011");
                //int.TryParse(s_Ago, out ago);
                //if (ago == 0) ago = 60;
                ago = ago * -1;
                //将要删除的文件记录
                List<FileInfo> fileList = new List<FileInfo>();
                //服务器上的当前时间
                //dateNow = dateNow ?? PCBusiness.SrvDateTime;
                dateNow = dateNow ?? DateTime.Now;

                //添加要删除的文件 
                foreach (FileInfo file in files)
                {
                    if (dateNow.Value.AddDays(ago) > file.LastWriteTime)
                    {
                        bool canAdd = true;
                        if (!string.IsNullOrEmpty(filePattern)) canAdd = Regex.IsMatch(file.FullName, filePattern, RegexOptions.IgnoreCase);
                        if (canAdd) fileList.Add(file);
                    }
                }
                WriteSglogManager(0, "", "DeleteHisFiles", string.Format("删除历史文件，共{1}个文件...", fileList.Count));
                if (fileList.Count == 0) return;
                foreach (FileInfo file in fileList) File.Delete(file.FullName);
                WriteSglogManager(0, "", "DeleteHisFiles", "删除历史文件结束");
            }
            catch (Exception ex)
            {
                WriteSglogManager(0, "", "DeleteHisFiles", string.Format("删除历史文件结束失败，消息：{0} ", ex.Message + ex.StackTrace));
            }

        }


        #endregion
    }
}

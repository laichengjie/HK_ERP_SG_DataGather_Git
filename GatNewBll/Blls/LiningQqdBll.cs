using Gat.Utils;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.LiningNop;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Serializers;
using SqlSugar;
using System.Net;
using GatNewBll.Models.LiningQqd;

namespace GatNewBll.Blls
{
    /// <summary>
    /// 李宁全渠道
    /// </summary>
    public class LiningQqdBll : BaseBll
    {

        public HandleArgs NormalPost(int solutionId, DateTime startTime, DateTime endTime,bool isbu)
        {
            HandleArgs result = new HandleArgs();
            if (solutionId == 0)
            {
                GatSolutionManager manager = new GatSolutionManager();
                List<GatSolution> ListSolutin = null;
                BasVendorManager Vendormanager = new BasVendorManager();
                BasVendor Vendor = Vendormanager.GetList(s => s.AllowUsed == true && s.ReportPath == 22).FirstOrDefault();
                if (Vendor != null)
                {
                    int RepeatType = Vendor.Id;
                    ListSolutin = manager.GetList(s => s.VendorID == RepeatType && s.Enabled == true);
                    foreach (GatSolution solution in ListSolutin)
                    {
                        NormalPostDate(solution.Id, startTime, endTime, isbu);
                    }
                    result.Status = HandleStatus.Completed;
                    result.Message = "提报完成";
                }
            }
            else
            {
                result = NormalPostDate(solutionId, startTime, endTime, isbu);
            }
            return result;
        }
        public HandleArgs NormalPostDate(int solutionId, DateTime startTime, DateTime endTime, bool isbu)
        {
            HandleArgs result = new HandleArgs();
            try
            { 
                if (startTime > endTime)
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "开始时间不能小于结束时间"
                    };
                }

                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                var postConfig = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(postConfig))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报参数"
                    };
                }
                var url = postConfig;


                //
                var dataList = GetPostData(solutionId, startTime, endTime, isbu);
                if (dataList.Count > 0)
                {
                    bool isallsucc = true;
                    bool issucc = true;
                    var newdata = dataList.GroupBy(c => c.docDate);
                    foreach (var item in newdata)
                    {
                        LQPOSSalOrderDataReq req = new LQPOSSalOrderDataReq();
                        req.datas = item.AsEnumerable().ToList();
                        string reqstring = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                        //url = url.EndsWith("/")? url + "api/services/open/pushgmv": url + "/api/services/open/pushgmv";
                        int i = 0;
                    start:
                        var memberResult = webClient(reqstring, url);
                        LQPOSSalOrderDataResp resp = new LQPOSSalOrderDataResp();
                        issucc = true;
                        try
                        {
                            resp = JsonConvert.DeserializeObject<LQPOSSalOrderDataResp>(memberResult);
                            if (resp.code != "10000")
                            {
                                issucc = false;
                            }


                        }
                        catch (Exception ex)
                        {
                            if (i < 3)
                            {
                                i++;
                                goto start;
                            }
                            issucc = false;
                        }
                        if (issucc && resp != null)
                        {

                            string docdate = item.FirstOrDefault().docDate;
                            var listdate = item.AsEnumerable().ToList();
                            string allsql = "";
                            string insertsql = @"IF not EXISTS(SELECT TOP 1 1 FROM dbo.LingNi_QqdData WHERE ShopCode='{0}' AND SaleDate='{1}' )
                               BEGIN  INSERT INTO dbo.LingNi_QqdData(ShopCode, SaleDate, PayAmount, SendTime, SendStatus, ErrorReason, TryCount)VALUES('{0}', '{1}', '{2}', GETDATE(), '{3}', N'{4}', '{5}') END
                               ELSE BEGIN UPDATE a SET a.PayAmount ='{2}',a.SendStatus = '{3}',a.ErrorReason=N'{4}',TryCount = TryCount + 1,SendTime=GETDATE() FROM LingNi_QqdData a WHERE a.ShopCode = '{0}' AND a.SaleDate = '{1}' END ";

                            foreach (var shopdata in listdate)
                            {
                                if (resp.data != null && resp.data.errorList.Count > 0)
                                {
                                    var err = resp.data.errorList.Where(c => c.primaryKey == shopdata.shopCode).FirstOrDefault();
                                    if (err != null)
                                    {
                                        allsql += " " + string.Format(insertsql, shopdata.shopCode, shopdata.docDate, shopdata.gmv, "1", err.errorMsg, 1);
                                        isallsucc = false;
                                    }
                                    else
                                    {
                                        allsql += " " + string.Format(insertsql, shopdata.shopCode, shopdata.docDate, shopdata.gmv, "0", "", 1);
                                    }
                                }
                                else
                                {
                                    allsql += " " + string.Format(insertsql, shopdata.shopCode, shopdata.docDate, shopdata.gmv, "0", "", 1);
                                }



                            }
                            if (allsql.Length > 0)
                            {
                                int j = 0;
                            StarSql:
                                try
                                {
                                    int exeCount = ExtractorDB.Ado.ExecuteCommand(allsql);
                                }
                                catch (Exception ex)
                                {
                                    if (j < 2)
                                    {
                                        j++;
                                        goto StarSql;
                                    }
                                }
                            }
                        }

                        if (!issucc)
                        {
                            isallsucc = false;
                        }

                        SysLogManager logManager = new SysLogManager();
                        //记录请求日志syslog
                        SysLog log = new SysLog()
                        {
                            CompanyID = solution.CompanyID,
                            CardID = solution.CardID,
                            IsSucceed = isallsucc,
                            LogType = Convert.ToByte(LogType.Gat),
                            SolutionID = solution.Id,
                            VendorID = solution.VendorID,
                            ModifyDTM = DateTime.Now,
                            Script = $"{startTime.ToString()}-{endTime.ToString()}",
                            Remark = "ERP数据通过中台提交给李宁提报数据 请求:" + url + ";" + reqstring + ";" + (memberResult != null ? Newtonsoft.Json.JsonConvert.SerializeObject(memberResult) : "")
                        };
                        logManager.Insert(log);
                    }
                    if (!isallsucc)
                    {
                        result.Status = HandleStatus.Fail;
                        result.Message = "采集失败，请检查日志";
                    }
                    else
                    {
                        result.Status = HandleStatus.Completed;
                        result.Message = "提报成功";
                    }
                }
                else
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "未获取到数据";
                }
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
        /// <summary>
        /// 获取提报数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<LQdata> GetPostData(int solutionId, DateTime startTime, DateTime endTime,bool isbu)
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<LQdata>("sp_SGIS_LiNingQqd_GetPosSale",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Isbu= isbu?1:0 });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常"+ ex.Message+ex.StackTrace);
            }
        }

        /// <summary>
        /// 浏览器方式请求接口
        /// </summary>
        /// <param name="args"></param>
        /// <param name="url"></param>
        /// <param name="mediatype"></param>
        /// <param name="Medthod"></param>
        /// <returns></returns>
        private string webClient(string args, string url, string mediatype = "application/json", string Medthod = "POST")
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", mediatype);
                    Encoding code = Encoding.GetEncoding("utf-8");
                    //把数组转换成流中所需字节数组类型
                    byte[] bytesRequestData = code.GetBytes(args);
                    var Ret = client.UploadData(url, Medthod, bytesRequestData);
                    return System.Text.Encoding.UTF8.GetString(Ret);
                }
            }
            catch (Exception ex)
            {
                return ex.Message+ex.StackTrace;
            }

        }

    }
}

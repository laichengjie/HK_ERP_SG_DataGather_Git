using Gat.Utils;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.LiningMember;
using Newtonsoft.Json;
using System.Net;
using AutoMapper;
using SqlSugar;
using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GatNewBll.Blls
{
    public class LiningMemberBll : BaseBll
    {

        public LiningMemberBll()
        {
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        public List<GatLiningMemberPosSalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string status, string tradeCode, string refundCode, string isRefund,string memberMobile, ref int totalCount, ref decimal totalQty, ref decimal? totalAmount)
        {
            List<GatLiningMemberPosSalePageModel> result = new List<GatLiningMemberPosSalePageModel>();
            GatLiningMemberPosSaleManager manager = new GatLiningMemberPosSaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.PayTime >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.PayTime <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.TradeStoreCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.PostSuccess == (status == "0" ? false : true));
            }
            if (!string.IsNullOrEmpty(tradeCode))
            {
                data = data.Where(o => o.TradeCode.Contains(tradeCode));
            }
            if (!string.IsNullOrEmpty(refundCode))
            {
                data = data.Where(o => o.RefundCode.Contains(refundCode));
            }
            if (!string.IsNullOrEmpty(isRefund))
            {
                if (isRefund == "1")
                {
                    data = data.Where(o => o.TradeAmount2 < 0);
                }
                else
                {
                    data = data.Where(o => o.TradeAmount2 >= 0);
                }
            }
            if (!string.IsNullOrEmpty(memberMobile))
            {
                data = data.Where(o => o.CustomerMobile.Contains(memberMobile));
            }
            totalQty = data.Sum(o => o.Qty);
            totalAmount = data.Sum(o => o.TradeAmount2);
            result = data.Select((o) => new GatLiningMemberPosSalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.PayTime, o.TradeStoreCode, o.TradeCode })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }

        /// <summary>
        /// 批量数据重传
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs Repost(int solutionId, List<int> ids)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatLiningMemberPosSaleManager gatLMPSManager = new GatLiningMemberPosSaleManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                var postConfig = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(postConfig))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报路径(李宁会员提报为李宁接口地址及secrect参数)"
                    };
                }

                var configs = postConfig.Split(';');
                var url = configs[0];
                var secrectPara = configs[1];
                var idsStr = string.Join(",", ids);
                var dataList = GetPostData(solutionId, idsStr);
                foreach (var item in dataList.Item1)
                {
                    var liningMPS = gatLMPSManager.GetFirst(s => s.SolutionId == solutionId && s.TradeCode == item.tradeCode);

                    mapPosSale(liningMPS, item);
                    try
                    {

                        var details = dataList.Item2.FindAll(s => s.tradeCode == item.tradeCode);
                        var payments = dataList.Item3.FindAll(s => s.tradeCode == item.tradeCode);
                        var resultJson = string.Empty;
                        var postResult = PostData(url, secrectPara, item, details, payments, ref resultJson);
                        liningMPS.PostSuccess = postResult;
                        liningMPS.PostReturnJson = resultJson;
                    }
                    catch (Exception ex)
                    {
                        liningMPS.PostSuccess = false;
                        liningMPS.PostReturnJson = $"{ex.Message},{ex.StackTrace}";
                    }
                    liningMPS.ModifyDate = DateTime.Now;
                    gatLMPSManager.Update(liningMPS);
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
                    Script = string.Join(";", ids),
                    Remark = "李宁会员" + "重传"
                };
                logManager.Insert(log);
                result.Status = HandleStatus.Completed;
                result.Message = "重传成功";
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
        /// 正常数据发送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="positive"></param>
        /// <returns></returns>
        public HandleArgs NormalPost(int solutionId, DateTime startTime, DateTime endTime, string positive = null)
        {
            HandleArgs result = new HandleArgs();
            if (solutionId == 0)
            {
                GatSolutionManager manager = new GatSolutionManager();
                List<GatSolution> ListSolutin = null;
                BasVendorManager Vendormanager = new BasVendorManager();
                BasVendor Vendor = Vendormanager.GetList(s => s.AllowUsed == true && s.ReportPath == 14).FirstOrDefault();
                if (Vendor != null)
                {
                    int RepeatType = Vendor.Id;
                    ListSolutin = manager.GetList(s => s.VendorID == RepeatType && s.Enabled==true);
                    foreach (GatSolution solution in ListSolutin)
                    {
                        NormalPostDate(solution.Id, startTime, endTime, positive);
                    }
                    result.Status = HandleStatus.Completed;
                    result.Message = "提报完成";
                }
            }
            else
            {
                result = NormalPostDate(solutionId, startTime, endTime, positive);
            }
            return result;
        }

        public HandleArgs NormalPostDate(int solutionId, DateTime startTime, DateTime endTime, string positive = null)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatLiningMemberPosSaleManager gatLMPSManager = new GatLiningMemberPosSaleManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                var postConfig = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(postConfig))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报路径(李宁会员提报为李宁接口地址及secrect参数)"
                    };
                }

                var configs = postConfig.Split(';');
                var url = configs[0];
                var secrectPara = configs[1];

                var dataList = GetPostData(solutionId, startTime, endTime, positive);
                foreach (var item in dataList.Item1)
                {
                    var liningMPS = gatLMPSManager.GetFirst(s => s.SolutionId == solutionId && s.TradeCode == item.tradeCode);
                    if (liningMPS == null)
                    {
                        liningMPS = new GatLiningMemberPosSale(solutionId);
                    }
                    mapPosSale(liningMPS, item);
                    try
                    {

                        var details = dataList.Item2.FindAll(s => s.tradeCode == item.tradeCode);
                        var payments = dataList.Item3.FindAll(s => s.tradeCode == item.tradeCode);
                        var resultJson = string.Empty;
                        var postResult = PostData(url, secrectPara, item, details, payments, ref resultJson);
                        liningMPS.PostSuccess = postResult;
                        liningMPS.PostReturnJson = resultJson;
                    }
                    catch (Exception ex)
                    {
                        liningMPS.PostSuccess = false;
                        liningMPS.PostReturnJson = $"{ex.Message}";
                    }
                    liningMPS.ModifyDate = DateTime.Now;
                    gatLMPSManager.InsertOrUpdate(liningMPS);
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
                    Script = $"{startTime.ToString()}-{endTime.ToString()}",
                    Remark = "李宁会员" + "提报"
                };
                logManager.Insert(log);
                result.Status = HandleStatus.Completed;
                result.Message = "提报成功";
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

        private void mapPosSale(GatLiningMemberPosSale liningMPS, SaleMasterData item)
        {
            liningMPS.FullName = item.fullName;
            liningMPS.Birthday = item.birthday;
            liningMPS.Address = item.address;
            liningMPS.MemberId = item.memberId;
            liningMPS.Gender = item.gender;
            liningMPS.TradeCode = item.tradeCode;
            liningMPS.RefundCode = item.refundCode;
            liningMPS.CustomerMobile = item.customerMobile;
            liningMPS.TradeSource = item.tradeSource;
            liningMPS.TradeAmount1 = (decimal)item.tradeAmount1;
            liningMPS.TradeAmount2 = (decimal)item.tradeAmount2;
            liningMPS.TradeStoreCode = item.tradeStoreCode;
            liningMPS.TradeType = item.tradeType;
            liningMPS.PayTime = Convert.ToDateTime(item.payTime);
            liningMPS.Qty = item.qty;
        }

        /// <summary>
        /// 获取提报数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Tuple<List<SaleMasterData>, List<SaleDetailData>, List<SalePaymentData>> GetPostData(int solutionId, string ids)
        {
            try
            {
                return LiningMemberErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<SaleMasterData, SaleDetailData, SalePaymentData>("sp_sgis_lining_member_posdata",
                    new { SolutionId = solutionId, Ids = ids });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常");
            }
        }

        /// <summary>
        /// 获取提报数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<SaleMasterData>, List<SaleDetailData>, List<SalePaymentData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime, string positive)
        {
            try
            {
                return LiningMemberErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<SaleMasterData, SaleDetailData, SalePaymentData>("sp_sgis_lining_member_posdata",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Positive = positive });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常");
            }
        }

        /// <summary>
        /// 发送李宁数据请求，并返回成功失败结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="secrectPara"></param>
        /// <param name="master"></param>
        /// <param name="details"></param>
        /// <param name="payments"></param>
        /// <param name="resultJson">返回结果json</param>
        /// <returns></returns>
        private bool PostData(string url, string secrectPara, SaleMasterData master, List<SaleDetailData> details, List<SalePaymentData> payments, ref string resultJson)
        {
            var result = false;
            try
            {
                string signTime = signdate();
                string secrets = secret(secrectPara);
                string upData = string.Empty;
                var tradeCode = master.tradeCode;
                Trade trade = new Trade();
                trade.tradeDetailJson = details;
                var refundCode = details.Find(s => s.refundCode != null && s.refundCode != "")?.refundCode;
                master.refundCode = refundCode;
                trade.tradeInfoJson = master;
                trade.paymentJson = payments;

                Member member = new Member();
                member.secret = secrets;
                member.signdate = signTime;
                member.customerMobile = master.customerMobile;
                string memberJson = JsonConvert.SerializeObject(member);
                //var memberResult = webClient(memberJson, url + "/customer/getMemberInfoSingle");
                WebHeaderCollection webHeaderCollection = new WebHeaderCollection
                {
                    { ".AspNetCore.Culture", "zh-Hans" }
                };
                var memberResult = HttpPost(url + "/customer/getMemberInfoSingle", memberJson, webHeaderCollection);

                if (memberResult.Contains("无法连接到远程服务器"))
                {
                    throw new Exception($"发送销售数据失败,{memberResult}");
                }

                var memberResponse = JsonConvert.DeserializeObject<MemberResponse>(memberResult);
                if (memberResponse.memberJson.Length == 0)
                {
                    AddMember addMember = new AddMember();
                    addMember.secret = secrets;
                    addMember.signdate = signTime;
                    AddMemberInfo memberInfo = new AddMemberInfo();
                    memberInfo.fullName = master.fullName ?? "顾客";
                    memberInfo.mobileNum = master.customerMobile;
                    if (((Convert.ToDateTime(master.birthday).ToUniversalTime().Ticks - 621355968000000000) / 10000) < 0)
                    {
                        memberInfo.birthday = ((Convert.ToDateTime("1990-01-01 00:00:00").ToUniversalTime().Ticks - 621355968000000000) / 10000);
                    }
                    else
                    {
                        memberInfo.birthday = ((Convert.ToDateTime(master.birthday).ToUniversalTime().Ticks - 621355968000000000) / 10000);
                    }
                    memberInfo.gender = master.gender ?? "男";
                    memberInfo.address = master.address ?? "无";
                    memberInfo.storeCode = master.tradeStoreCode;
                    memberInfo.brand = "1";
                    addMember.memberJson = memberInfo;
                    string addMemberJson = JsonConvert.SerializeObject(addMember);
                    //var addMmeberResult = webClient(addMemberJson, url + "/customer/addMemberSTC");
                    var addMmeberResult = HttpPost(url + "/customer/addMemberSTC", addMemberJson, webHeaderCollection);
                    if (addMmeberResult.Contains("无法连接到远程服务器"))
                    {
                        throw new Exception($"新增会员失败,{addMmeberResult}");
                    }
                    var addMemberResponse = JsonConvert.DeserializeObject<AddMemberResponse>(addMmeberResult);
                    if (addMemberResponse.status == "-1")
                    {
                        throw new Exception($"新增会员失败,{addMmeberResult}");
                    }
                    trade.tradeInfoJson.memberId = addMemberResponse.memberJson.memberid;
                }
                else
                {
                    trade.tradeInfoJson.memberId = memberResponse.memberJson[0].memberid;
                }
                trade.secret = secrets;
                trade.signdate = signTime;
                string tradeJson = JsonConvert.SerializeObject(trade);
                //var tradeResult = webClient(tradeJson, url + "trade/sendTradeInfo");
                var tradeResult = HttpPost(url + "trade/sendTradeInfo", tradeJson, webHeaderCollection);
                if (tradeResult.Contains("无法连接到远程服务器"))
                {
                    throw new Exception($"发送销售数据失败,{tradeResult}");
                }
                var tradeResponse = JsonConvert.DeserializeObject<TradeResponse>(tradeResult);
                resultJson = tradeResult;
                if (tradeResponse.status == "-1")
                {
                    throw new Exception($"发送销售数据失败,{tradeResult}");
                }
                result = true;
                return result;
                //if (tradeResponse.status == "-1")
                //{
                //    tradeResponse.status = "4";
                //}
            }
            catch (Exception ex)
            {
                throw new Exception($"发送销售数据异常 {ex.Message}");
            }
        }

        private string secret(String appkey)
        {
            TimeSpan date = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            String md5Secret = md5String(((Convert.ToInt64(date.TotalMilliseconds) / 1000).ToString() + appkey)).ToLower();
            return md5Secret;
        }

        private string signdate()
        {
            String signdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return signdate;
        }

        private string md5String(string sourceString)
        {
            string _Encrypassword = string.Empty;
            byte[] result = Encoding.UTF8.GetBytes(sourceString);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            _Encrypassword = BitConverter.ToString(output).Replace("-", "");
            return _Encrypassword;
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
                return ex.Message +",请求地址："+ url;
            }

        }

        /// <summary>  
        /// 指定Post地址使用Get 方式获取全部字符串  
        /// </summary>  
        /// <param name="url">请求后台地址</param>  
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>  
        /// <returns></returns>  
        public string HttpPost(string url, string content, WebHeaderCollection HeaderValue)
        {
            HttpWebRequest req = null;
            HttpWebResponse resp = null;
            Stream stream = null;
            StreamReader reader = null;

            HttpWebResponse httpExResponse = null;
            StreamReader readerEx = null;

            string result = "";
            try
            {
                Uri uri = new Uri(url);
                if (uri.Scheme == "https")
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                // Set a default policy level for the "http:" and "https" schemes. 
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                HttpWebRequest.DefaultCachePolicy = policy;


                req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "POST";
                req.Accept = "application/json,text/plain,*/*";
                req.ContentType = "application/json;charset=UTF-8";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36";
                foreach (string key in HeaderValue.Keys)
                {
                    req.Headers.Add(key, HeaderValue[key]);
                }
                #region 添加Post 参数  
                byte[] data = Encoding.UTF8.GetBytes(content);
                //req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion

                resp = (HttpWebResponse)req.GetResponse();
                stream = resp.GetResponseStream();
                //获取响应内容  
                using (reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (WebException ex)
            {
                //服务器没响应，直接向调用方抛回
                if (ex.Response == null) throw;

                if (resp == null) throw;

                //读取服务器的错误响应信息
                httpExResponse = (System.Net.HttpWebResponse)ex.Response;//得到请求网站的详细错误提示
                readerEx = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                content = reader.ReadToEnd();

                //重新抛回给调用方
                throw new Exception(content);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (req != null) req.Abort();
                if (resp != null) resp.Close();
                if (stream != null) stream.Dispose();
                if (reader != null) reader.Dispose();

                if (httpExResponse != null) httpExResponse.Close();
                if (readerEx != null) readerEx.Dispose();
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}

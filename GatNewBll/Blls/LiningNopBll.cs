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

namespace GatNewBll.Blls
{
    /// <summary>
    /// 李宁NOP开放平台接口
    /// </summary>
    public class LiningNopBll : BaseBll
    {
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="vendCustCode"></param>
        /// <param name="billNo"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        public List<GatLiningnopWholesalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string vendCustCode, string billNo, string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatLiningnopWholesalePageModel> result = new List<GatLiningnopWholesalePageModel>();
            GatLiningnopWholesaleManager manager = new GatLiningnopWholesaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= endBillDate);
            }
            if (!string.IsNullOrEmpty(vendCustCode))
            {
                data = data.Where(o => o.VendCustCode.Contains(vendCustCode));
            }
            if (!string.IsNullOrEmpty(billNo))
            {
                data = data.Where(o => o.BillNo.Contains(billNo));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }
            totalQty = data.Sum(o => o.Qty);
            totalAmount = data.Sum(o => o.Amount);
            result = data.Select((o) => new GatLiningnopWholesalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.BillDate, o.VendCustCode, o.BillNo })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }

        public HandleArgs Repost(int solutionId, List<int> ids)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatLiningnopWholesaleManager gatlwManager = new GatLiningnopWholesaleManager();
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

                var configs = postConfig.Split(';');
                var url = configs[0];
                var appIdPara = configs[1];
                var secrectPara = configs[2];
                var idsStr = string.Join(",", ids);
                var dataList = GetPostData(solutionId, idsStr);
                if (dataList.Item1.Count > 0)
                {

                    AuthTokenRequest tokenReq = new AuthTokenRequest(false);
                    var tokenRes = PostData<AuthTokenResponse>(appIdPara, secrectPara, url, tokenReq, tokenReq.GetMethod());
                    string token = tokenRes.data.access_token;
                    foreach (var item in dataList.Item1)
                    {
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.externalOrderno);

                        try
                        {
                            var details = dataList.Item2.FindAll(s => s.externalOrderno == item.externalOrderno);
                            WholesaleSalesListRequest wReq = new WholesaleSalesListRequest()
                            {
                                externalOrderno = item.externalOrderno,
                                companyCode = item.companyCode,
                                receivingCode = item.receivingCode,
                                orderDate = item.orderDate.ToString("yyyyMMdd"),
                                remarks = "",
                                items = new List<WholesaleSalesListRequest.WholesaleSalesListItem>()
                            };
                            int totalQty = 0;
                            decimal totalAmount = 0;
                            foreach (var itemDetail in details)
                            {
                                wReq.items.Add(new WholesaleSalesListRequest.WholesaleSalesListItem()
                                {
                                    disc = itemDetail.disc,
                                    qty = itemDetail.qty.ToString(),
                                    prodCode=itemDetail.prodCode,
                                    sizeCode=itemDetail.sizeCode,
                                    //sku = itemDetail.sku,
                                    unitPrice = itemDetail.unitPrice.ToString()
                                });
                                totalQty += itemDetail.qty;
                                totalAmount += (itemDetail.qty * itemDetail.unitPrice);
                            }
                            sgData.Qty = totalQty;
                            sgData.Amount = totalAmount;
                            var postResult = PostData<WholesaleSalesListResponse>(appIdPara, secrectPara, url, wReq, wReq.GetMethod(), token);
                            if (postResult.code == "10000")
                            {
                                sgData.IsSuccess = postResult.data.state.ToUpper() == "S";
                            }
                            else
                            {
                                sgData.IsSuccess = false;
                            }
                            sgData.PostJaon = JsonConvert.SerializeObject(wReq);
                            sgData.ReturnJson = JsonConvert.SerializeObject(postResult);
                        }
                        catch (Exception ex)
                        {
                            sgData.IsSuccess = false;
                            sgData.ReturnJson = $"{ex.Message},{ex.StackTrace}";
                        }
                        sgData.ModifyDtm = DateTime.Now;
                        gatlwManager.Update(sgData);
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
                        Remark = "李宁开放平台批发销售出库数据提报Repost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "重传成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "重传成功:未获取到数据";
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

        public HandleArgs NormalPost(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs result = new HandleArgs();
            if (solutionId == 0)
            {
                GatSolutionManager manager = new GatSolutionManager();
                List<GatSolution> ListSolutin = null;
                BasVendorManager Vendormanager = new BasVendorManager();
                BasVendor Vendor = Vendormanager.GetList(s => s.AllowUsed == true && s.ReportPath == 15).FirstOrDefault();
                if (Vendor != null)
                {
                    int RepeatType = Vendor.Id;
                    ListSolutin = manager.GetList(s => s.VendorID == RepeatType && s.Enabled == true);
                    foreach (GatSolution solution in ListSolutin)
                    {
                        NormalPostDate(solution.Id, startTime, endTime);
                    }
                    result.Status = HandleStatus.Completed;
                    result.Message = "提报完成";
                }
            }
            else
            {
                result = NormalPostDate(solutionId, startTime, endTime);
            }
            return result;
        }

        public HandleArgs NormalPostDate(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs result = new HandleArgs(); 
            try
            {
                GatLiningnopWholesaleManager gatlwManager = new GatLiningnopWholesaleManager();
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

                var configs = postConfig.Split(';');
                var url = configs[0];
                var appIdPara = configs[1];
                var secrectPara = configs[2];
                var dataList = GetPostData(solutionId, startTime, endTime);
                if (dataList.Item1.Count > 0)
                {
                    AuthTokenRequest tokenReq = new AuthTokenRequest(false);
                    var tokenRes = PostData<AuthTokenResponse>(appIdPara, secrectPara, url, tokenReq, tokenReq.GetMethod());
                    string token = tokenRes.data.access_token;

                    foreach (var item in dataList.Item1)
                    {
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.externalOrderno);
                        if (sgData == null)
                        {
                            sgData = new GatLiningnopWholesale()
                            {
                                SolutionId = solutionId,
                                BillNo = item.externalOrderno,
                                CreateDtm = DateTime.Now
                            };
                        }
                        sgData.BillDate = item.orderDate;
                        sgData.VendCustCode = item.VendCustCode;
                        sgData.VendCustName = item.VendCustName;
                        try
                        {
                            var details = dataList.Item2.FindAll(s => s.externalOrderno == item.externalOrderno);
                            WholesaleSalesListRequest wReq = new WholesaleSalesListRequest()
                            {
                                externalOrderno = item.externalOrderno,
                                companyCode = item.companyCode,
                                receivingCode = item.receivingCode,
                                orderDate = item.orderDate.ToString("yyyyMMdd"),
                                remarks = "",
                                items = new List<WholesaleSalesListRequest.WholesaleSalesListItem>()
                            };
                            int totalQty = 0;
                            decimal totalAmount = 0;
                            foreach (var itemDetail in details)
                            {
                                wReq.items.Add(new WholesaleSalesListRequest.WholesaleSalesListItem()
                                {
                                    disc = itemDetail.disc,
                                    qty = itemDetail.qty.ToString(),
                                    prodCode = itemDetail.prodCode,
                                    sizeCode = itemDetail.sizeCode,
                                    //sku = itemDetail.sku,
                                    unitPrice = itemDetail.unitPrice.ToString()
                                });
                                totalQty += itemDetail.qty;
                                totalAmount += (itemDetail.qty * itemDetail.unitPrice);
                            }
                            sgData.Qty = totalQty;
                            sgData.Amount = totalAmount;
                            var postResult = PostData<WholesaleSalesListResponse>(appIdPara, secrectPara, url, wReq, wReq.GetMethod(), token);
                            if (postResult.code == "10000")
                            {
                                sgData.IsSuccess = postResult.data.state.ToUpper() == "S";
                            }
                            else
                            {
                                sgData.IsSuccess = false;
                            }
                            sgData.PostJaon = JsonConvert.SerializeObject(wReq);
                            sgData.ReturnJson = JsonConvert.SerializeObject(postResult);
                        }
                        catch (Exception ex)
                        {
                            sgData.IsSuccess = false;
                            sgData.ReturnJson = $"{ex.Message},{ex.StackTrace}";
                        }
                        sgData.ModifyDtm = DateTime.Now;
                        gatlwManager.InsertOrUpdate(sgData);
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
                        Remark = "李宁开放平台批发销售出库数据提报NormalPost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "重传成功:未获取到数据";
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
        /// <param name="ids"></param>
        /// <returns></returns>
        private Tuple<List<WholesaleMasterData>, List<WholesaleDetailData>> GetPostData(int solutionId, string ids)
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<WholesaleMasterData, WholesaleDetailData>("sp_sgis_lining_fx_wholesale",
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
        private Tuple<List<WholesaleMasterData>, List<WholesaleDetailData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<WholesaleMasterData, WholesaleDetailData>("sp_sgis_lining_fx_wholesale",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常");
            }
        }

        public void GetAuthToken()
        {
            try
            {
                AuthTokenRequest request = new AuthTokenRequest(false);

            }
            catch (Exception ex)
            {

                throw new Exception($"获取token异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }


        public BaseResponse<T> PostData<T>(string appId, string secrect, string url, object data, string method, string token = null)
        {
            try
            {
                BaseRequest requestData = new BaseRequest()
                {
                    AppId = appId,
                    AccessToken = token,
                    DataObj = data,
                    Data = JsonConvert.SerializeObject(data),
                    Method = method
                };
                requestData.Sign = Sign(requestData, secrect);
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(requestData, Formatting.Indented, jsetting);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                var client = new RestClient(url);
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.AddStringBody(json, ContentType.Json);

                var response = client.ExecutePostAsync<BaseResponse<T>>(request).GetAwaiter().GetResult();
                if (response.Data == null)
                {
                    throw new Exception($"请求{method}接口失败");
                }
                return response.Data;

            }
            catch (Exception ex)
            {
                throw new Exception($"发送请求异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }


        public string Sign(BaseRequest reqeust, string secrect)
        {
            try
            {
                string paramSrcStr = string.Empty;
                string paramMd5Str = string.Empty;

                if (reqeust != null)
                {
                    Dictionary<string, string> dicts = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(reqeust.AppId))
                    {
                        dicts.Add("app_id", reqeust.AppId);
                    }
                    if (!string.IsNullOrEmpty(reqeust.Method))
                    {
                        dicts.Add("method", reqeust.Method);
                    }
                    if (!string.IsNullOrEmpty(reqeust.Version))
                    {
                        dicts.Add("version", reqeust.Version);
                    }
                    if (!string.IsNullOrEmpty(reqeust.Charset))
                    {
                        dicts.Add("charset", reqeust.Charset);
                    }
                    if (!string.IsNullOrEmpty(reqeust.Timestamp))
                    {
                        dicts.Add("timestamp", reqeust.Timestamp);
                    }
                    if (!string.IsNullOrEmpty(reqeust.AccessToken))
                    {
                        dicts.Add("access_token", reqeust.AccessToken);
                    }
                    if (!string.IsNullOrEmpty(reqeust.Sign))
                    {
                        dicts.Add("sign", reqeust.Sign);
                    }
                    if (reqeust.DataObj != null)
                    {
                        dicts.Add("data", JsonConvert.SerializeObject(reqeust.DataObj));
                    }
                    dicts.Add("format", reqeust.Format);
                    dicts.Add("sign_type", reqeust.SignType);
                    paramSrcStr = GetParamSrc(dicts);
                }

                if (!string.IsNullOrEmpty(paramSrcStr))
                {
                    paramMd5Str = GetMd5Str(secrect + paramSrcStr + secrect);
                }
                return paramMd5Str;
            }
            catch (Exception ex)
            {
                throw new Exception($"sign生成异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }

        private string GetMd5Str(string str)
        {
            string byte32Str = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fromData = Encoding.UTF8.GetBytes(str);
                byte[] targetData = md5.ComputeHash(fromData);
                for (int i = 0; i < targetData.Length; i++)
                {
                    byte32Str += targetData[i].ToString("X2");
                }
            }

            return byte32Str.ToUpper();
        }

        private string GetParamSrc(Dictionary<string, string> pairs)
        {
            var dic = pairs.OrderBy(x => x.Key, new CompareString()).ToDictionary(x => x.Key, y => y.Value);

            var str = new StringBuilder();
            foreach (var kv in dic)
            {
                var pvalue = kv.Value;
                if (string.IsNullOrEmpty(pvalue))
                {
                    continue;
                }
                str.Append(kv.Key + pvalue);
            }

            return str.ToString();
        }

        public class CompareString : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.CompareOrdinal(x, y);
            }
        }
    }
}

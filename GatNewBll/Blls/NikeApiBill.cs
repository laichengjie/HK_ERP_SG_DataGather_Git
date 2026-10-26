using Gat.Utils;
using Gat.Web.Controller.Dtos.NikeApi;
using GatEntities.DbModels;
using GatNewBll.Models;
using GatNewBll.Models.Burgeon;
using GatNewBll.Models.NikeApi;
using GatNewBll.Models.On;
using GatSqlSugarDAL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers;
using SqlSugar;
using SqlSugar.DistributedSystem.Snowflake;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;

namespace GatNewBll.Blls
{
    /// <summary>
    /// nike开放平台接口
    /// </summary>
    public class NikeApiBill : BaseBll
    {

        string client_id ="";   // 客户端标识 / 客户端 ID
        string client_secret =""; // 客户端密钥 / 客户端密码
        string partner_code =""; // 合作方编码 / 合作伙伴代码
        string gettoken_url =""; // 获取访问令牌的 URL
        string api_url =""; // API 接口的 URL
        NikeApiToken Token = null;//

        /// <summary>
        /// 查询Nke接口日志
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
        /// <param name="IsPaging"> 是否分页</param>
        /// <returns></returns>
        public QueryOutputDto<GatNikeApiLogModel> GatNikeApiLoList(QueryInputDto input, bool IsPaging = false)
        {
            QueryOutputDto<GatNikeApiLogModel> result = new QueryOutputDto<GatNikeApiLogModel>()
            {
                TotalCount =0,
                Qty = 0,
                Amount = 0,
            };
            GatNikeApiLogManager manager = new GatNikeApiLogManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == input.SolutionId);
            if (input.StartBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= input.StartBillDate);
            }
            if (input.EndBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= input.EndBillDate);
            }
            if (!string.IsNullOrEmpty(input.ShopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(input.ShopCode));
            }
            if (!string.IsNullOrEmpty(input.StockCode))
            {
                data = data.Where(o => o.StockCode.Contains(input.StockCode));
            }

            if (!string.IsNullOrEmpty(input.InterfaceType))
            {
                string BillType = "POS";
                if (input.InterfaceType == "1")
                {
                    BillType = "INV";
                }
                else if (input.InterfaceType == "2")
                {
                    BillType = "OutIn";
                }
                data = data.Where(o => o.BillType== BillType);
            }
            if (!string.IsNullOrEmpty(input.BillNo))
            {
                data = data.Where(o => o.BillNo.Contains(input.BillNo));
            }
            if (!string.IsNullOrEmpty(input.Status))
            {
                data = data.Where(o => o.IsSuccess == (input.Status == "0" ? false : true));
            }
            int totalCount = data.Count();
            if (totalCount > 0)
            { 
                result.Qty = (int)data.Sum(o => o.Qty);
                result.Amount = (decimal)data.Sum(o => o.Amount);
            }

            if (IsPaging)
            {
                result.Data = data.Select((o) => new GatNikeApiLogModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.ShopCode, o.BillNo })
                  .ToPageList(input.Page, input.PageSize, ref totalCount);
               
            }
            else
            {
                result.Data = data.Select((o) => new GatNikeApiLogModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.ShopCode, o.BillNo })
                  .ToList();
            }
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询销售数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param>
        /// <param name="CardShopCode"></param>
        /// <param name="billNo"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="totalAmount"></param>
        /// <param name="IsPaging"></param>
        /// <returns></returns>
        public QueryOutputDto<GatNikeApiPosSaleModel> GatNikeApiPosSaleList(QueryInputDto input, bool IsPaging = false)
        {
            QueryOutputDto<GatNikeApiPosSaleModel> result = new QueryOutputDto<GatNikeApiPosSaleModel>()
            {
                TotalCount = 0,
                Qty = 0,
                Amount = 0,
            };
            GatNikeApiPosSaleManager manager = new GatNikeApiPosSaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == input.SolutionId);
            if (input.StartBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= input.StartBillDate);
            }
            if (input.EndBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= input.EndBillDate);
            }
            if (!string.IsNullOrEmpty(input.ShopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(input.ShopCode));
            }
            //if (!string.IsNullOrEmpty(input.StockCode))
            //{
            //    data = data.Where(o => o.StockCode.Contains(input.StockCode));
            //}

            if (!string.IsNullOrEmpty(input.BillNo))
            {
                data = data.Where(o => o.BillNo.Contains(input.BillNo));
            }

            if (!string.IsNullOrEmpty(input.MaterialCode))
            {
                data = data.Where(o => o.PRODUCT_CD.Contains(input.MaterialCode));
            }

            if (!string.IsNullOrEmpty(input.BarCode))
            {
                data = data.Where(o => o.UPC.Contains(input.BarCode));
            }

            if (!string.IsNullOrEmpty(input.SizeCode))
            {
                data = data.Where(o => o.SIZE_CD.Contains(input.SizeCode));
            }

            if (!string.IsNullOrEmpty(input.Status))
            {
                data = data.Where(o => o.IsSuccess == (input.Status == "0" ? false : true));
            }

            int totalCount = data.Count();
            if (totalCount > 0)
            {
                result.Qty = (int)data.Sum(o => o.SALES_QTY);
                result.Amount = (decimal)data.Sum(o => o.NET_SALES_AMT_INCL_TAX);
            }

            if (IsPaging)
            {
                result.Data = data.Select((o) => new GatNikeApiPosSaleModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.ShopCode, o.BillNo })
                  .ToPageList(input.Page, input.PageSize, ref totalCount);

            }
            else
            {
                result.Data = data.Select((o) => new GatNikeApiPosSaleModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.ShopCode, o.BillNo })
                  .ToList();
            }
            result.TotalCount = totalCount;
            return result;
        }



        /// <summary>
        /// 查询库存数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param>
        /// <param name="CardShopCode"></param>
        /// <param name="billNo"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param> 
        /// <param name="totalAmount"></param>
        /// <param name="IsPaging"></param>
        /// <returns></returns>
        public QueryOutputDto<GatNikeApiInvModel> GatNikeApiInvList(QueryInputDto input, bool IsPaging = false)
        {
            QueryOutputDto<GatNikeApiInvModel> result = new QueryOutputDto<GatNikeApiInvModel>()
            {
                TotalCount = 0,
                Qty = 0,
                Amount = 0,
            };
            GatNikeApiInvManager manager = new GatNikeApiInvManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == input.SolutionId);
            if (!string.IsNullOrEmpty(input.ShopCode))
            {
                data = data.Where(o => o.SITE_CD.Contains(input.ShopCode));
            }
            if (!string.IsNullOrEmpty(input.StockCode))
            {
                data = data.Where(o => o.StockCode.Contains(input.StockCode));
            }

            if (!string.IsNullOrEmpty(input.MaterialCode))
            {
                data = data.Where(o => o.PRODUCT_CD.Contains(input.MaterialCode));
            }

            if (!string.IsNullOrEmpty(input.BarCode))
            {
                data = data.Where(o => o.UPC.Contains(input.BarCode));
            }

            if (!string.IsNullOrEmpty(input.SizeCode))
            {
                data = data.Where(o => o.SIZE_CD.Contains(input.SizeCode));
            }

            if (!string.IsNullOrEmpty(input.Status))
            {
                data = data.Where(o => o.IsSuccess == (input.Status == "0" ? false : true));
            }
            if (!string.IsNullOrEmpty(input.Status))
            {
                data = data.Where(o => o.IsSuccess == (input.Status == "0" ? false : true));
            }

            int totalCount = data.Count();
            if (totalCount > 0)
            {
                result.Qty = (int)data.Sum(o => o.TOTAL_QTY);
               
            }

            if (IsPaging)
            {
                result.Data = data.Select((o) => new GatNikeApiInvModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.SITE_CD, o.PRODUCT_CD, o.UPC })
                  .ToPageList(input.Page, input.PageSize, ref totalCount);

            }
            else
            {
                result.Data = data.Select((o) => new GatNikeApiInvModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.SITE_CD, o.PRODUCT_CD, o.UPC })
                  .ToList();
            }
            result.TotalCount = totalCount;
            return result;
        }



        /// <summary>
        ///  查询出入库数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param>
        /// <param name="CardShopCode"></param>
        /// <param name="billNo"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalQty"></param>
        /// <param name="totalAmount"></param>
        /// <param name="IsPaging"></param>
        /// <returns></returns>
        public QueryOutputDto<GatNikeApiOutInModel> GatNikeApiOutInList(QueryInputDto input, bool IsPaging = false)
        {
            QueryOutputDto<GatNikeApiOutInModel> result = new QueryOutputDto<GatNikeApiOutInModel>()
            {
                TotalCount = 0,
                Qty = 0,
                Amount = 0,
            };
            GatNikeApiOutInManager manager = new GatNikeApiOutInManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == input.SolutionId);
            if (input.StartBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate >= input.StartBillDate);
            }
            if (input.EndBillDate.HasValue)
            {
                data = data.Where(o => o.BillDate <= input.EndBillDate);
            }
            if (!string.IsNullOrEmpty(input.ShopCode))
            {
                data = data.Where(o => o.SITE_CD.Contains(input.ShopCode));
            }
            if (!string.IsNullOrEmpty(input.StockCode))
            {
                data = data.Where(o => o.SITE_CD.Contains(input.StockCode));
            }

            if (!string.IsNullOrEmpty(input.BillNo))
            {
                data = data.Where(o => o.BillNo.Contains(input.BillNo));
            }

            if (!string.IsNullOrEmpty(input.MaterialCode))
            {
                data = data.Where(o => o.PRODUCT_CD.Contains(input.MaterialCode));
            }

            if (!string.IsNullOrEmpty(input.BarCode))
            {
                data = data.Where(o => o.UPC.Contains(input.BarCode));
            }

            if (!string.IsNullOrEmpty(input.SizeCode))
            {
                data = data.Where(o => o.SIZE_CD.Contains(input.SizeCode));
            }

            if (!string.IsNullOrEmpty(input.Status))
            {
                data = data.Where(o => o.IsSuccess == (input.Status == "0" ? false : true));
            }

            int totalCount = data.Count();
            if (totalCount > 0)
            {
                result.Qty = (int)data.Sum(o => o.TRANSACTION_QTY);
              
            }

            if (IsPaging)
            {
                result.Data = data.Select((o) => new GatNikeApiOutInModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.SITE_CD, o.BillNo }) 
                  .ToPageList(input.Page, input.PageSize, ref totalCount);

            }
            else
            {
                result.Data = data.Select((o) => new GatNikeApiOutInModel()
                {
                    Id = o.Id.SelectAll()
                }).OrderBy(o => new { o.BillDate, o.SITE_CD, o.BillNo })
                  .ToList();
            }
            result.TotalCount = totalCount;
            return result;
        }


        /// <summary>
        /// 自动推送 NIke（所有提报方案 ）
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="IsPushStock">是否推送库存</param>
        /// <returns></returns>
        public HandleArgs PushNikeApiAllData(int solutionId, DateTime startTime, DateTime endTime, bool IsPushStock = false)
        {
            GatSolutionManager SolutionManager = new GatSolutionManager();
            BasVendorManager VendorManager = new BasVendorManager();
            List<BasVendor> ListVendor = VendorManager.GetList(s => s.AllowUsed == true && s.ReportPath == 21);
            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            foreach (BasVendor vendor in ListVendor)
            {
                List<GatSolution> ListSolution = SolutionManager.GetList(s => s.Enabled == true && s.VendorID == vendor.Id);
                foreach (GatSolution Solution in ListSolution)
                {
                    try
                    {
                        HandleArgs Posresult = new HandleArgs();
                        Posresult = PushPostDataSave(Solution.Id, startTime, endTime, null);
                        if (Posresult.Status == HandleStatus.Fail)
                        {
                            result.Status = HandleStatus.Fail;
                        }
                        result.Message += Posresult.Message;
                    }
                    catch (Exception ex)
                    {
                        result.Message += ex.Message;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 推送Nik数据(按提报方案)
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="IsPushStock">是否推送库存</param>
        /// <returns></returns>
        public HandleArgs PushNikeApiPosData(int solutionId, DateTime startTime, DateTime endTime, bool IsPushStock = false)
        {
            HandleArgs Posresult = new HandleArgs();
            Posresult = PushPostDataSave(solutionId, startTime, endTime, "");

            HandleArgs Invresult = new HandleArgs();
            if (IsPushStock)
            {
                //Invresult = NormalInvPost(solutionId, startTime, endTime);
            }
            else
            {
                Invresult.Status = HandleStatus.Completed;
            }

            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            if (Posresult.Status == HandleStatus.Fail || Invresult.Status == HandleStatus.Fail)
            {
                result.Status = HandleStatus.Fail;
            }
            result.Message = Posresult.Message + Invresult.Message;
            return result;
        }

        /// <summary>
        /// 推送Nike数据接口
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="idsStr">单号ID字符串</param>
        /// <param name="InterfaceType">接口类型 0 销售 1 库存 2 出入库</param>
        /// <returns></returns>
        public HandleArgs PushPostDataSave(int solutionId, DateTime? startTime, DateTime? endTime, string idsStr,string InterfaceType="0")
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatNikeApiLogManager gatlwManager = new GatNikeApiLogManager();
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
                string url = configs[0];
                string AppKey = configs[1]; //接口认证ID
                string AppSecret = configs[2]; //接口认证口令
                string AppToken = configs[3];
                string SourceSys = configs[4];
                if (string.IsNullOrEmpty(url))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "接口地址不能为空"
                    };
                }
                
                var dataList = GetPosData(solutionId, startTime, endTime, idsStr);

                bool IsOk = true;
                bool IsUpdate = false;
                bool IsImport = false;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item2)
                    {
                        IsUpdate = false;
                        IsOk = true;
                        IsImport = false;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        if (sgData == null)
                        {
                            sgData = new GatNikeApiLog()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillNo = item.BillNo,//线上订单号

                                BillDate = item.BillDate,//订单时间

                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码

                                //Qty = item.Qty,//数量
                                //Amount = item.Amount,//金额
                                //IsImport = item.IsImport,//是否导入
                                //PushBillNo = item.PushBillNo,//推送单号
                                //Rate = item.Rate,
                                IsSuccess = false,
                            };
                        }
                        else
                        {

                            IsUpdate = true;
                            sgData.Qty = item.Qty;//数量
                            sgData.Amount = item.Amount;//数量
                        }

                        if (IsOk)
                        {
                            try
                            {
                                int totalQty = 0;
                                decimal totalAmount = 0;

                                List<NikeApiBillDate> MasteList = new List<NikeApiBillDate>();
                                NikeApiBillDate Maste = new NikeApiBillDate()
                                {
                                    order_no = item.BillNo.ToString(),//  线上订单号   string  是
                                  
                                    billdate = item.BillDate.ToString("yyyyMMdd"),// 单据日期 string  是
                                    storecode = item.ShopCode.ToString(),// 销售店仓  string  是
                                    mesg_id = "INT_3010R_BOS",//接口编号    string  是   值：INT_3010R_BOS
                                    mesg_guid = Guid.NewGuid().ToString(),// 当前接口数据唯一ID  string  是   GUID
                                };



                                MasteList.Add(Maste);

                                string appInterface = "INT_3010R_BOS";
                                string content = JsonConvert.SerializeObject(MasteList);
                                string Response = "";// WebServices.Request(appInterface, content);
                                var ResultData = JsonConvert.DeserializeObject<List<NikeApiPosResponseData>>(Response);
                                NikeApiPosResponseData FirstData = ResultData?.FirstOrDefault();
                                if (FirstData != null && FirstData.STATUS == "S")
                                {
                                    sgData.IsSuccess = true;
                                    UpdateNikeLogState(item.CompanyID, item.BillNo);
                                }
                                else
                                {
                                    sgData.IsSuccess = false;
                                }
                                sgData.PostJaon = "地址：" + url + "报文：" + JsonConvert.SerializeObject(content);
                                sgData.ReturnJson = Response;
                            }
                            catch (Exception ex)
                            {
                                sgData.IsSuccess = false;
                                sgData.ReturnJson = $"{ex.Message},{ex.StackTrace}";
                            }
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
                        Script = $"sp_SGIS_GatNikeApi_GetPosSale{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "NIke数据提报NormalPost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "Pos提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "Pos重传成功:未获取到数据";
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
        /// 获取POS数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<GatNikeApiPosSaleModel>, List<GatNikeApiLogModel>>  GetPosData(int solutionId, DateTime? startTime = null, DateTime? endTime = null, string ids = "")
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<GatNikeApiPosSaleModel, GatNikeApiLogModel>("sp_SGIS_NikeApi_GetData",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Ids = ids, Type = "POS" });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常" + ex.Message);
            }
        }
        /// <summary>
        /// 获取库存数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<GatNikeApiInvModel>, List<GatNikeApiLogModel>> GetInventoryData(int solutionId, DateTime? startTime = null, DateTime? endTime = null, string ids = "")
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<GatNikeApiInvModel, GatNikeApiLogModel>("sp_SGIS_NikeApi_GetData",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Ids = ids, Type = "INV" });
            }
            catch (Exception ex)
            {
                throw new Exception("获取库存数据异常" + ex.Message);
            }
        }
        /// <summary>
        /// 获取出入库数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<GatNikeApiOutInModel>, List<GatNikeApiLogModel>> GetOutInData(int solutionId, DateTime? startTime = null, DateTime? endTime = null, string ids = "")
        {
            try
            {
                return ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<GatNikeApiOutInModel, GatNikeApiLogModel>("sp_SGIS_NikeApi_GetData",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Ids = ids, Type = "OUTIN" });
            }
            catch (Exception ex)
            {
                throw new Exception("获取出入库数据异常" + ex.Message);
            }
        }


        /// <summary>
        /// 更新Nike日志状态为成功
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private int UpdateNikeLogState(string companyId, string billNo)
        {
            try
            {
                string sql = @"UPDATE a SET a.IsSuccess = 1 
                       FROM Gat_GatNikeApi_PosSale_Detail a 
                       WHERE a.CompanyID = @CompanyID AND a.BillNO = @BillNO";

                int rows = Db.Ado.ExecuteCommand(sql, new { CompanyID = companyId, BillNO = billNo });
                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception("更新请求单号异常", ex);
            }
        }


        public void test()
        {
            WebHeaderCollection headerValue = GetWebHeader();
            //string requestbody = JsonConvert.SerializeObject(input);
            string requestbody = "";
            string requestUrl = "";
            string responsebody = HttpPost(requestUrl, requestbody, headerValue);
            var result = JsonConvert.DeserializeObject<NikeApiPosMasterData>(responsebody);
        }
        /// <summary>
        /// 获取请求头部信息
        /// </summary>
        /// <returns></returns>
        public WebHeaderCollection GetWebHeader()
        {
            WebHeaderCollection headerValue = new WebHeaderCollection();
            headerValue.Add("Authorization", "Bearer " + GetAccessToken());
            return headerValue;
        }


        public string HttpPost(string url, string content, WebHeaderCollection HeaderValue)
        {
            try
            {
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                var client = new RestClient(url);
                RestRequest request = new RestRequest();
                foreach (string key in HeaderValue.Keys)
                {
                    request.AddHeader(key, HeaderValue[key]);
                }
                request.AddStringBody(content, ContentType.Json);

                var response = client.ExecutePostAsync(request).GetAwaiter().GetResult();
                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception($"请求接口失败");
                }
                return response.Content;
            }
            catch (Exception ex)
            {
                throw new Exception($"发送请求异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取接口参数
        /// </summary>
        /// <returns></returns>
        public HandleArgs GetInterfaceParameters(int solutionId)
        {
            HandleArgs result = new HandleArgs();
            GatSolutionManager manager = new GatSolutionManager();
            var solution = manager.GetById(solutionId);
            var postConfig = solution.Directory;
            if (string.IsNullOrEmpty(postConfig))
            {
                return new HandleArgs()
                {
                    Status = HandleStatus.Fail,
                    Message = "接口参数不能为空"
                };
            }
            var configs = postConfig.Split(';');

            gettoken_url = configs[0];// 获取访问令牌的 URL
            api_url = configs[1];// API 接口的 URL
            client_id = configs[2];  // 客户端标识 / 客户端 ID
            client_secret = configs[3]; // 客户端密钥 / 客户端密码
            partner_code = configs[4];// 合作方编码 / 合作伙伴代码

            result.Status = HandleStatus.Completed;
            return result;


        }

        /// <summary>
        /// 获取有效的 access_token。
        /// 若缓存有效则直接返回；否则请求新 Token。
        /// 提前 5 分钟刷新，避免传输中途失效（文档要求）。
        /// </summary>
        public string GetAccessToken()
        {
            // 原判断是 DateTime.UtcNow < ExpireAtUtc.AddMinutes(-5)，方向写反了：
            // 会导致「Token 还有效时去刷新，快过期了反而不刷新」。
            // 正确逻辑：进入「到期前 5 分钟」窗口即重新获取。
            var needRefresh = Token == null
                              || string.IsNullOrEmpty(Token.access_token)
                              || DateTime.UtcNow >= Token.ExpireAtUtc.AddMinutes(-5);

            if (needRefresh)
            {
                Token = RequestToken();
            }

            return Token.access_token;
        }
        private NikeApiToken RequestToken()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gettoken_url))
                {
                    throw new Exception("未配置获取Token的地址");
                }

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                // 1. Basic 认证：Base64(client_id:client_secret)
                var basic = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{client_id}:{client_secret}"));

                // 2. 组装请求（与 HttpPost 同一套 RestSharp 写法）
                var client = new RestClient(gettoken_url);
                RestRequest request = new RestRequest();

                request.AddHeader("Authorization", "Basic " + basic);
                request.AddHeader("Accept", "application/json");
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", partner_code);

                var response = client.ExecutePostAsync(request).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception($"请求Token接口失败");
                }
                if (!response.IsSuccessful)
                {
                    throw new Exception($"请求Token接口失败，HTTP {(int)response.StatusCode}，响应：{response.Content}");
                }

                var result = JsonConvert.DeserializeObject<NikeApiToken>(response.Content);
                if (result == null || string.IsNullOrEmpty(result.access_token))
                {
                    throw new Exception($"Token 响应解析失败或 access_token 为空，响应：{response.Content}");
                }
                var seconds = result.expires_in > 0 ? result.expires_in : 3600;
                result.ExpireAtUtc = DateTime.UtcNow.AddSeconds(seconds);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取Token异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }
    


}
}

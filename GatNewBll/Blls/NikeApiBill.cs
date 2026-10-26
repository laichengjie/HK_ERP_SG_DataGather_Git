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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services;

namespace GatNewBll.Blls
{
    /// <summary>
    /// nike开放平台接口
    /// </summary>
    public class NikeApiBill : BaseBll
    {
        #region 常量（依据《API数据对接_客户数据推送技术文档》）

        /// <summary>data_type：销售数据</summary>
        private const string DataTypePos = "pos";
        /// <summary>data_type：库存数据</summary>
        private const string DataTypeInventory = "inventory";
        /// <summary>data_type：库存移动数据</summary>
        private const string DataTypeMovement = "movement";

        /// <summary>日志单据类型：销售</summary>
        private const string BillTypePos = "POS";
        /// <summary>日志单据类型：库存</summary>
        private const string BillTypeInv = "INV";
        /// <summary>日志单据类型：出入库（库存移动）</summary>
        private const string BillTypeOutIn = "OutIn";

        /// <summary>业务数据结构版本</summary>
        private const string SchemaVersion = "1.0";

        /// <summary>业务状态码：全部记录受理成功</summary>
        private const string CodeSuccess = "0000";
        /// <summary>业务状态码：Token 无效或已过期</summary>
        private const string CodeTokenExpired = "3002";
        /// <summary>业务状态码：幂等冲突（相同 request_id 已成功处理）</summary>
        private const string CodeDuplicateRequest = "4001";

        /// <summary>每个 request 最多 10000 条 records（文档 3.2）</summary>
        private const int MaxRecordsPerRequest = 10000;

        /// <summary>最大重试次数（文档 5.3）</summary>
        private const int MaxRetryCount = 5;
        /// <summary>重试间隔（毫秒），文档 5.3 要求 5 分钟</summary>
        private const int RetryIntervalMs = 5 * 60 * 1000;
        /// <summary>单次 HTTP 请求超时（毫秒）</summary>
        private const int RequestTimeoutMs = 3 * 60 * 1000;

        /// <summary>OAuth scope 固定值（文档 第一步）</summary>
        private const string TokenScope = "data_management:edagc.pde.ingest::create:";

        /// <summary>X-Auth-Type 固定值（文档 3.1）</summary>
        private const string AuthType = "oscar";

        #endregion

        string client_id = "";   // 客户端标识 / 客户端 ID
        string client_secret = ""; // 客户端密钥 / 客户端密码
        string partner_code = ""; // 合作方编码 / 合作伙伴代码
        string source_system = ""; // 数据来源系统名称（自营/加盟）
        string gettoken_url = ""; // 获取访问令牌的 URL
        string api_url = ""; // API 接口的 URL
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
                TotalCount = 0,
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
                data = data.Where(o => o.BillType == BillType);
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
            List<BasVendor> ListVendor = VendorManager.GetList(s => s.AllowUsed == true && s.ReportPath == 28);
            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            foreach (BasVendor vendor in ListVendor)
            {
                List<GatSolution> ListSolution = SolutionManager.GetList(s => s.Enabled == true && s.VendorID == vendor.Id);
                if (solutionId > 0)
                {
                    ListSolution = ListSolution.Where(s => s.Id == solutionId).ToList();
                }
                foreach (GatSolution Solution in ListSolution)
                {
                    try
                    {
                        HandleArgs pushResult = PushNikeApiData(Solution.Id, startTime, endTime, IsPushStock);
                        if (pushResult.Status == HandleStatus.Fail)
                        {
                            result.Status = HandleStatus.Fail;
                        }
                        result.Message += pushResult.Message;
                    }
                    catch (Exception ex)
                    {
                        result.Status = HandleStatus.Fail;
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
        public HandleArgs PushNikeApiData(int solutionId, DateTime startTime, DateTime endTime, bool IsPushStock = false)
        {
            HandleArgs Posresult = new HandleArgs();
            Posresult = PushPosDataSave(solutionId, startTime, endTime, "");

            HandleArgs Invresult = new HandleArgs();
            if (IsPushStock)
            {
                Invresult = PushInvDataSave(solutionId, startTime, endTime, "");
            }
            else
            {
                Invresult.Status = HandleStatus.Completed;
            }
            HandleArgs OutInresult = new HandleArgs();
            OutInresult = PushOutInDataSave(solutionId, startTime, endTime, "");

            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            if (Posresult.Status == HandleStatus.Fail || Invresult.Status == HandleStatus.Fail || OutInresult.Status == HandleStatus.Fail)
            {
                result.Status = HandleStatus.Fail;
            }
            result.Message = Posresult.Message + Invresult.Message+ OutInresult.Message;
            return result;
        }

        /// <summary>
        /// 推送Nike  销售数据
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="idsStr">单号ID字符串</param>
        /// <param name="InterfaceType">接口类型 0 销售 1 库存 2 出入库</param>
        /// <returns></returns>
        public HandleArgs PushPosDataSave(int solutionId, DateTime? startTime, DateTime? endTime, string idsStr)
        {
            return PushDataSave(solutionId, startTime, endTime, idsStr,
                DataTypePos, BillTypePos, GetPosData, FilterPosDetails, BuildPosRecords);
        }

        /// <summary>
        /// 推送Nike  库存数据
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param> 
        /// <param name="endTime">结束时间</param>
        /// <param name="idsStr">单号ID字符串</param>
        /// <param name="InterfaceType">接口类型 0 销售 1 库存 2 出入库</param>
        /// <returns></returns>
        public HandleArgs PushInvDataSave(int solutionId, DateTime? startTime, DateTime? endTime, string idsStr)
        {
            return PushDataSave(solutionId, startTime, endTime, idsStr,
                DataTypeInventory, BillTypeInv, GetInventoryData, FilterInvDetails, BuildInvRecords);
        }


        /// <summary>
        /// 推送Nike  库存移动数据
        /// </summary>
        /// <param name="solutionId">提报方案ID</param>
        /// <param name="startTime">开始时间</param> 
        /// <param name="endTime">结束时间</param>
        /// <param name="idsStr">单号ID字符串</param>
        /// <param name="InterfaceType">接口类型 0 销售 1 库存 2 出入库</param>
        /// <returns></returns>
        public HandleArgs PushOutInDataSave(int solutionId, DateTime? startTime, DateTime? endTime, string idsStr)
        {
            return PushDataSave(solutionId, startTime, endTime, idsStr,
                DataTypeMovement, BillTypeOutIn, GetOutInData, FilterOutInDetails, BuildOutInRecords);
        }


        /// <summary>
        /// 推送数据到 Nike 的公共流程（文档 3：一个批次 = 一个 data_type 当天传输的完整数据）
        /// 1. 同一批次共用 batch_no，先规划出全部 request 以确定 total_requests；
        /// 2. 每个 request 不超过 10000 条 records，request_seq 递增、request_id 与 Header X-Request-ID 一致；
        /// 3. 按文档 2/5.3 处理 Token 失效重传与失败重试。
        /// </summary>
        private HandleArgs PushDataSave<T>(int solutionId, DateTime? startTime, DateTime? endTime, string idsStr,
            string dataType, string billType,
            Func<int, DateTime?, DateTime?, string, Tuple<List<T>, List<GatNikeApiLogModel>>> getData,
            Func<List<T>, GatNikeApiLogModel, List<T>> filterDetails,
            Func<List<T>, List<NikeApiRecordItem>> buildRecords)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatNikeApiLogManager gatlwManager = new GatNikeApiLogManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                if (solution == null)
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = $"未找到提报方案（SolutionId={solutionId}）";
                    return result;
                }
                result = GetInterfaceParameters(solution.Directory);
                if (result.Status == HandleStatus.Fail)
                {
                    return result;
                }
                GetAccessToken();
                if (Token == null || string.IsNullOrEmpty(Token.access_token))
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "获取Token失败";
                    return result;
                }

                var dataList = getData(solutionId, startTime, endTime, idsStr);
                List<T> allDetails = dataList == null ? new List<T>() : (dataList.Item1 ?? new List<T>());
                List<GatNikeApiLogModel> billList = dataList == null
                    ? new List<GatNikeApiLogModel>()
                    : (dataList.Item2 ?? new List<GatNikeApiLogModel>());

                if (allDetails.Count == 0 || billList.Count == 0)
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = $"{billType}提报成功：未获取到数据";
                    return result;
                }

                // 批次号（UUID v4），本批次所有 request 共用；batch_date = Current Date - 1 Day
                string batchNo = Guid.NewGuid().ToString();
                string batchDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                // 先切分出本批次全部 request，才能得到 total_requests（文档 3.2 / 3.4）
                List<NikeApiPushPlan> plans = new List<NikeApiPushPlan>();
                List<NikeApiBillContext> contexts = new List<NikeApiBillContext>();
                foreach (var bill in billList)
                {
                    var billDetails = filterDetails(allDetails, bill);
                    if (billDetails == null || billDetails.Count == 0)
                    {
                        continue;
                    }
                    var items = buildRecords(billDetails);
                    if (items == null || items.Count == 0)
                    {
                        continue;
                    }

                    NikeApiBillContext context = new NikeApiBillContext() { Bill = bill, IsSuccess = true };
                    contexts.Add(context);

                    for (int offset = 0; offset < items.Count; offset += MaxRecordsPerRequest)
                    {
                        var chunk = items.Skip(offset).Take(MaxRecordsPerRequest).ToList();
                        plans.Add(new NikeApiPushPlan()
                        {
                            Context = context,
                            Records = chunk.Select(o => o.Record).ToList(),
                            AmountSum = Math.Round(chunk.Sum(o => o.Amount), 2),
                            QuantitySum = chunk.Sum(o => o.Quantity)
                        });
                    }
                }

                if (plans.Count == 0)
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = $"{billType}提报成功：未获取到数据";
                    return result;
                }

                int totalRequests = plans.Count;

                for (int i = 0; i < plans.Count; i++)
                {
                    NikeApiPushPlan plan = plans[i];
                    // request_id：UUID v4，幂等性控制与链路追踪；重传时保持不变
                    string requestId = Guid.NewGuid().ToString();


                    var request = BuildNikeApiRequest(dataType, batchNo, batchDate, requestId, i + 1, totalRequests, plan);
                    string content = JsonConvert.SerializeObject(request);

                    NikeApiSendResult sendResult = SendWithRetry(content, requestId);

                    plan.Context.PostJson.AppendLine("地址：" + api_url + " 报文：" + content);
                    plan.Context.ReturnJson.AppendLine(sendResult.ReturnJson);
                    if (!sendResult.IsSuccess)
                    {
                        plan.Context.IsSuccess = false;
                        plan.Context.ErrorMessage = sendResult.Message;
                    }
                }

                int successCount = 0;
                int failCount = 0;
                foreach (var context in contexts)
                {
                    var bill = context.Bill;
                    var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId
                                                            && s.CompanyID == bill.CompanyID
                                                            && s.BillNo == bill.BillNo
                                                            && s.BillType == billType
                                                            && s.BillTypeID == bill.BillTypeID);
                    if (sgData == null)
                    {
                        sgData = new GatNikeApiLog()
                        {
                            SolutionId = solutionId,//提报方案
                            CompanyID = bill.CompanyID,//公司
                            BillType = billType,//单据类型
                            BillTypeID = bill.BillTypeID,//单据类型ID
                            BillTypeName = bill.BillTypeName,//单据类型名称
                            BillNo = bill.BillNo,//单号
                            BillDate = bill.BillDate,//单据时间
                            CardID = bill.CardID,//品牌ID
                            ShopID = bill.ShopID,//店铺ID
                            ShopCode = bill.ShopCode,//店铺代码
                            StockID = bill.StockID,//仓库ID
                            StockCode = bill.StockCode,//仓库代码
                            Qty = bill.Qty,//数量
                            Amount = bill.Amount,//金额
                            PushBillNo = bill.PushBillNo,//推送单号
                            IsSuccess = false,
                            CreateDtm = DateTime.Now,
                        };
                    }
                    else
                    {
                        sgData.Qty = bill.Qty;//数量
                        sgData.Amount = bill.Amount;//金额
                        sgData.BillTypeName = bill.BillTypeName;
                        sgData.PushBillNo = bill.PushBillNo;
                    }

                    sgData.PostJaon = context.PostJson.ToString();
                    sgData.ReturnJson = context.ReturnJson.ToString();
                    sgData.IsSuccess = context.IsSuccess;
                    sgData.ModifyDtm = DateTime.Now;
                    gatlwManager.InsertOrUpdate(sgData);

                    if (context.IsSuccess)
                    {
                        successCount++;
                        UpdateNikeLogState(bill.CompanyID, bill.BillNo, billType, bill.BillTypeID);
                    }
                    else
                    {
                        failCount++;
                    }
                }

                SysLogManager logManager = new SysLogManager();
                //记录请求日志syslog
                SysLog log = new SysLog()
                {
                    CompanyID = solution.CompanyID,
                    CardID = solution.CardID,
                    IsSucceed = failCount == 0,
                    LogType = Convert.ToByte(LogType.Gat),
                    SolutionID = solution.Id,
                    VendorID = solution.VendorID,
                    ModifyDTM = DateTime.Now,
                    Script = $"sp_SGIS_NikeApi_GetData{startTime.ToString()}-{endTime.ToString()}",
                    Remark = $"NIke {billType}推送"
                };
                logManager.Insert(log);

                result.Status = failCount == 0 ? HandleStatus.Completed : HandleStatus.Fail;
                result.Message = $"{billType}提报完成：共{totalRequests}个请求，成功{successCount}单，失败{failCount}单";
                if (failCount > 0)
                {
                    var errorContext = contexts.Find(o => !string.IsNullOrEmpty(o.ErrorMessage));
                    if (errorContext != null)
                    {
                        result.Message += "，首个错误：" + errorContext.ErrorMessage;
                    }
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
        /// 组装请求 Body（公共技术字段，文档 3.2）
        /// </summary>
        private NikeApiRequest<object> BuildNikeApiRequest(string dataType, string batchNo, string batchDate,
            string requestId, int requestSeq, int totalRequests, NikeApiPushPlan plan)
        {
            return new NikeApiRequest<object>()
            {
                batch_no = batchNo,
                batch_date = batchDate,
                request_seq = requestSeq,
                request_id = requestId,
                total_requests = totalRequests,
                partner_code = partner_code,
                data_type = dataType,
                record_count = plan.Records.Count,
                amount_sum = plan.AmountSum,
                quantity_sum = plan.QuantitySum,
                schema_version = SchemaVersion,
                source_system = source_system,
                send_timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                records = plan.Records
            };
        }

        #region 明细筛选

        private List<GatNikeApiPosSaleModel> FilterPosDetails(List<GatNikeApiPosSaleModel> details, GatNikeApiLogModel bill)
        {
            return details.FindAll(s => s.CompanyID == bill.CompanyID && s.BillNo == bill.BillNo);
        }

        private List<GatNikeApiInvModel> FilterInvDetails(List<GatNikeApiInvModel> details, GatNikeApiLogModel bill)
        {
            // 库存按「公司+仓库」分组，日志的 BillNo 存的是 StockID
            return details.FindAll(s => s.CompanyID == bill.CompanyID && s.StockID == bill.BillNo);
        }

        private List<GatNikeApiOutInModel> FilterOutInDetails(List<GatNikeApiOutInModel> details, GatNikeApiLogModel bill)
        {
            return details.FindAll(s => s.CompanyID == bill.CompanyID
                                        && s.BillNo == bill.BillNo
                                        && s.BillTypeID == bill.BillTypeID);
        }

        #endregion

        #region records 明细转换

        /// <summary>销售 records（data_type = pos），amount_sum 取 NET_SALES_AMT_INCL_TAX，quantity_sum 取 SALES_QTY</summary>
        private List<NikeApiRecordItem> BuildPosRecords(List<GatNikeApiPosSaleModel> details)
        {
            List<NikeApiRecordItem> list = new List<NikeApiRecordItem>();
            foreach (var detail in details)
            {
                int originalLineItemNbr = 0;
                if (!string.IsNullOrEmpty(detail.ORIGINAL_LINE_ITEM_NBR))
                {
                    int.TryParse(detail.ORIGINAL_LINE_ITEM_NBR, out originalLineItemNbr);
                }

                var record = new NikeApiRequest_Pos()
                {
                    store_cd = string.IsNullOrEmpty(detail.STORE_CD) ? detail.ShopCode : detail.STORE_CD,
                    business_type = detail.BusinessType,
                    transaction_dt = detail.BillDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    order_type = detail.OrderType,
                    order_nbr = detail.BillNo,
                    upc = detail.UPC,
                    product_cd = detail.PRODUCT_CD,
                    size_cd = detail.SIZE_CD,
                    product_nm = detail.PRODUCT_NM,
                    line_item_nbr = detail.LINE_ITEM_NBR,
                    net_sales_prc_incl_tax = detail.NET_SALES_PRC_INCL_TAX,
                    rebate_amt = detail.REBATE_AMT == null ? null : detail.REBATE_AMT.ToString(),
                    net_sales_amt_incl_tax = detail.NET_SALES_AMT_INCL_TAX,
                    sales_qty = detail.SALES_QTY,
                    msrp_lc = detail.MSRP_LC,
                    total_partner_msrp_amt = detail.TOTAL_PARTNER_MSRP_AMT,
                    total_discount_amt = detail.TOTAL_DISCOUNT_AMT,
                    original_store_cd = detail.ORIGINAL_STORE_CD,
                    original_order_nbr = detail.ORIGINAL_ORDER_NBR,
                    original_transaction_dt = detail.ORIGINAL_TRANSACTION_DT?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    original_line_item_nbr = originalLineItemNbr,
                    last_updated_tmst = (detail.LAST_UPDATED_TMST ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")
                };

                list.Add(new NikeApiRecordItem()
                {
                    Record = record,
                    Amount = detail.NET_SALES_AMT_INCL_TAX ?? 0,
                    Quantity = detail.SALES_QTY ?? 0
                });
            }
            return list;
        }

        /// <summary>库存 records（data_type = inventory），amount_sum 传 0，quantity_sum 取 TOTAL_QTY</summary>
        private List<NikeApiRecordItem> BuildInvRecords(List<GatNikeApiInvModel> details)
        {
            List<NikeApiRecordItem> list = new List<NikeApiRecordItem>();
            foreach (var detail in details)
            {
                var record = new NikeApiRequest_Inv()
                {
                    site_cd = detail.SITE_CD,
                    upc = detail.UPC,
                    product_cd = detail.PRODUCT_CD,
                    size_cd = detail.SIZE_CD,
                    product_nm = detail.PRODUCT_NM,
                    inventory_dt = detail.INVENTORY_DT?.ToString("yyyy-MM-dd") ?? "",
                    total_qty = detail.TOTAL_QTY ?? 0,
                    on_hand_qty = detail.ON_HAND_QTY ?? 0,
                    in_transit_qty = detail.IN_TRANSIT_QTY ?? 0,
                    defective_pcs_qty = detail.DEFECTIVE_PCS_QTY ?? 0,
                    msrp_lc = detail.MSRP_LC ?? 0,
                    last_updated_tmst = detail.LAST_UPDATED_TMST.ToString("yyyy-MM-dd HH:mm:ss")
                };

                list.Add(new NikeApiRecordItem()
                {
                    Record = record,
                    Amount = 0,
                    Quantity = detail.TOTAL_QTY ?? 0
                });
            }
            return list;
        }

        /// <summary>库存移动 records（data_type = movement），amount_sum 传 0，quantity_sum 取 TRANSACTION_QTY</summary>
        private List<NikeApiRecordItem> BuildOutInRecords(List<GatNikeApiOutInModel> details)
        {
            List<NikeApiRecordItem> list = new List<NikeApiRecordItem>();
            foreach (var detail in details)
            {
                var record = new NikeApiRequest_OutIn()
                {
                    site_cd = detail.SITE_CD,
                    site_nm = detail.SITE_NM,
                    upc = detail.UPC,
                    product_cd = detail.PRODUCT_CD,
                    size_cd = detail.SIZE_CD,
                    product_nm = detail.PRODUCT_NM,
                    direction_desc = detail.DIRECTION_DESC,
                    wms_order_no = detail.WMS_ORDER_NO,
                    proof_of_delivery_no = detail.PROOF_OF_DELIVERY_NO,
                    status_type_desc = detail.STATUS_TYPE_DESC,
                    transaction_tmst = detail.TRANSACTION_TMST.ToString("yyyy-MM-dd HH:mm:ss"),
                    transaction_qty = detail.TRANSACTION_QTY ?? 0,
                    source_from_site_cd = detail.SOURCE_FROM_SITE_CD,
                    source_from_site_nm = detail.SOURCE_FROM_SITE_NM,
                    destination_to_site_cd = detail.DESTINATION_TO_SITE_CD,
                    destination_to_site_nm = detail.DESTINATION_TO_SITE_NM,
                    last_updated_tmst = (detail.LAST_UPDATED_TMST ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")
                };

                list.Add(new NikeApiRecordItem()
                {
                    Record = record,
                    Amount = 0,
                    Quantity = detail.TRANSACTION_QTY ?? 0
                });
            }
            return list;
        }

        #endregion

        #region 发送（gzip + 重试 + Token 失效重传）

        /// <summary>
        /// 发送请求，按文档 5.3 重试：最大 5 次、间隔 5 分钟；
        /// 401 / code 3002 时重新获取 Token 后用同一个 X-Request-ID 立即重传。
        /// </summary>
        private NikeApiSendResult SendWithRetry(string content, string requestId)
        {
            NikeApiSendResult lastResult = new NikeApiSendResult()
            {
                IsSuccess = false,
                Message = "推送失败：未知错误",
                ReturnJson = "推送失败：未知错误"
            };

            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                NikeApiHttpResult http = HttpPost(api_url, content, GetWebHeader(requestId));

                if (!http.IsNetworkError)
                {
                    var rsp = DeserializeResponse(http.Content);
                    string code = rsp == null ? "" : (rsp.code ?? "");

                    // HTTP 401 / code 3002：Token 无效或已过期，重新获取 Token 后用同一个 X-Request-ID 重传
                    if (http.StatusCode == 401 || code == CodeTokenExpired)
                    {
                        lastResult = BuildFailResult(http, rsp, "Token 无效或已过期，刷新Token失败");
                        if (!RefreshToken())
                        {
                            return lastResult;
                        }
                        continue;
                    }

                    // 幂等冲突：相同 request_id 已成功处理，视为本条已受理
                    if (http.StatusCode == 409 || code == CodeDuplicateRequest)
                    {
                        return new NikeApiSendResult()
                        {
                            IsSuccess = true,
                            Message = rsp == null ? "" : rsp.message,
                            ReturnJson = BuildReturnJson(http, rsp)
                        };
                    }

                    if (http.StatusCode == 200 && code == CodeSuccess)
                    {
                        return new NikeApiSendResult()
                        {
                            IsSuccess = true,
                            Message = rsp == null ? "" : rsp.message,
                            ReturnJson = BuildReturnJson(http, rsp)
                        };
                    }

                    lastResult = BuildFailResult(http, rsp, rsp == null ? http.Content : rsp.message);

                    // 400 参数错误 / 403 权限不足 / 422 业务校验失败：重试无意义，直接返回
                    if (!(http.StatusCode == 408 || http.StatusCode == 429 || http.StatusCode >= 500))
                    {
                        return lastResult;
                    }
                }
                else
                {
                    lastResult = new NikeApiSendResult()
                    {
                        IsSuccess = false,
                        Message = http.ErrorMessage,
                        ReturnJson = http.ErrorMessage
                    };
                }

                if (attempt < MaxRetryCount)
                {
                    Thread.Sleep(RetryIntervalMs);
                }
            }

            return lastResult;
        }

        /// <summary>强制刷新 access_token</summary>
        private bool RefreshToken()
        {
            try
            {
                Token = null;
                GetAccessToken();
                return Token != null && !string.IsNullOrEmpty(Token.access_token);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private NikeApiSendResult BuildFailResult(NikeApiHttpResult http, NikeApiResponse rsp, string message)
        {
            return new NikeApiSendResult()
            {
                IsSuccess = false,
                Message = $"HTTP {http.StatusCode}，{message}",
                ReturnJson = BuildReturnJson(http, rsp)
            };
        }

        private string BuildReturnJson(NikeApiHttpResult http, NikeApiResponse rsp)
        {
            string summary = rsp == null
                ? ""
                : $"response_id:{rsp.response_id},code:{rsp.code},message:{rsp.message},trace_id:{rsp.trace_id}";
            return $"HTTP {http.StatusCode}；{summary}；原始报文：{http.Content}";
        }

        private NikeApiResponse DeserializeResponse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<NikeApiResponse>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>请求体 gzip 压缩（文档 3.1：Content-Encoding 固定 gzip）</summary>
        private byte[] GzipCompress(string content)
        {
            using (MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(content ?? "")))
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress, true))
                {
                    input.CopyTo(gzip);
                }
                return output.ToArray();
            }
        }

        #endregion

     

        /// <summary>
        /// 获取POS数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<GatNikeApiPosSaleModel>, List<GatNikeApiLogModel>> GetPosData(int solutionId, DateTime? startTime = null, DateTime? endTime = null, string ids = "")
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
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime, Ids = ids, Type = "OutIn" });
            }
            catch (Exception ex)
            {
                throw new Exception("获取出入库数据异常" + ex.Message);
            }
        }


        /// <summary>
        /// 更新Nike明细数据状态为推送成功
        /// </summary>
        /// <param name="companyId">公司</param>
        /// <param name="billNo">单号（库存类型传的是仓库ID）</param>
        /// <param name="billType">单据类型：POS / INV / OutIn</param>
        /// <param name="billTypeId">单据类型ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private int UpdateNikeLogState(string companyId, string billNo, string billType, string billTypeId = "")
        {
            try
            {
                string sql;
                object param;
                if (string.Equals(billType, BillTypeInv, StringComparison.OrdinalIgnoreCase))
                {
                    // 库存日志按「公司+仓库」分组，日志的 BillNo 存的是 StockID
                    sql = @"UPDATE a SET a.IsSuccess = 1 
                       FROM Gat_NikeApi_Inv a 
                       WHERE a.CompanyID = @CompanyID AND a.StockID = @BillNO";
                    param = new { CompanyID = companyId, BillNO = billNo };
                }
                else if (string.Equals(billType, BillTypeOutIn, StringComparison.OrdinalIgnoreCase))
                {
                    sql = @"UPDATE a SET a.IsSuccess = 1 
                       FROM Gat_NikeApi_OutIn a 
                       WHERE a.CompanyID = @CompanyID AND a.BillNO = @BillNO 
                             AND ISNULL(a.BillTypeID, '') = ISNULL(@BillTypeID, '')";
                    param = new { CompanyID = companyId, BillNO = billNo, BillTypeID = billTypeId };
                }
                else
                {
                    sql = @"UPDATE a SET a.IsSuccess = 1 
                       FROM Gat_NikeApi_PosSale a 
                       WHERE a.CompanyID = @CompanyID AND a.BillNO = @BillNO";
                    param = new { CompanyID = companyId, BillNO = billNo };
                }

                int rows = Db.Ado.ExecuteCommand(sql, param);
                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception("更新请求单号异常", ex);
            }
        }


        /// <summary>
        /// 获取请求头部信息（文档 3.1）
        /// </summary>
        /// <param name="requestId">X-Request-ID，与 Body 的 request_id 必须一致，UUID v4</param>
        /// <returns></returns>
        public WebHeaderCollection GetWebHeader(string requestId)
        {
            WebHeaderCollection headerValue = new WebHeaderCollection();
            headerValue.Add("Authorization", "Bearer " + (Token == null ? "" : Token.access_token));//Bearer Token
            headerValue.Add("Content-Type", "application/json");//固定值
            headerValue.Add("X-Auth-Type", AuthType);//固定值 oscar
            headerValue.Add("X-Request-ID", requestId);//请求唯一标识，UUID v4，用于幂等性控制与链路追踪
            headerValue.Add("X-Partner-Code", partner_code);//合作方唯一标识码，与 Body 的 partner_code 必须一致
            headerValue.Add("X-Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//请求发起时间
            headerValue.Add("Content-Encoding", "gzip");//固定值，请求体需 gzip 压缩
            return headerValue;
        }


        /// <summary>
        /// POST 报文到 Nike（请求体 gzip 压缩后发送）
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="content">请求体 JSON</param>
        /// <param name="headerValue">请求头</param>
        /// <returns>HTTP 状态码与响应原文</returns>
        public NikeApiHttpResult HttpPost(string url, string content, WebHeaderCollection HeaderValue)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                byte[] body = GzipCompress(content);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = RequestTimeoutMs;
                request.ReadWriteTimeout = RequestTimeoutMs;
                request.KeepAlive = false;
                request.ContentType = "application/json";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (HeaderValue != null)
                {
                    foreach (string key in HeaderValue.Keys)
                    {
                        string value = HeaderValue[key];
                        // Content-Type / Content-Length 属于受限标头，需通过属性设置
                        if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase))
                        {
                            request.ContentType = value;
                        }
                        else if (!string.Equals(key, "Content-Length", StringComparison.OrdinalIgnoreCase))
                        {
                            request.Headers[key] = value;
                        }
                    }
                }

                request.ContentLength = body.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return new NikeApiHttpResult()
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = reader.ReadToEnd()
                    };
                }
            }
            catch (WebException wex)
            {
                HttpWebResponse errorResponse = wex.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    string errorContent = "";
                    try
                    {
                        using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            errorContent = reader.ReadToEnd();
                        }
                    }
                    catch (Exception)
                    {
                        errorContent = "";
                    }
                    return new NikeApiHttpResult()
                    {
                        StatusCode = (int)errorResponse.StatusCode,
                        Content = errorContent
                    };
                }
                return new NikeApiHttpResult()
                {
                    IsNetworkError = true,
                    ErrorMessage = $"发送请求异常,message:{wex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new NikeApiHttpResult()
                {
                    IsNetworkError = true,
                    ErrorMessage = $"发送请求异常,message:{ex.Message},trace:{ex.StackTrace}"
                };
            }
        }

        /// <summary>
        /// 获取接口参数
        /// </summary>
        /// <returns></returns>
        public HandleArgs GetInterfaceParameters(string Directory)
        {
            HandleArgs result = new HandleArgs();
            if (string.IsNullOrEmpty(Directory))
            {
                return new HandleArgs()
                {
                    Status = HandleStatus.Fail,
                    Message = "接口参数不能为空"
                };
            }
            var configs = Directory.Split(';');
            if (configs.Length < 5)
            {
                return new HandleArgs()
                {
                    Status = HandleStatus.Fail,
                    Message = "接口参数格式不正确，应为：Token地址;接口地址;client_id;client_secret;partner_code[;source_system]"
                };
            }

            gettoken_url = configs[0].Trim();// 获取访问令牌的 URL
            api_url = configs[1].Trim();// API 接口的 URL
            client_id = configs[2].Trim();  // 客户端标识 / 客户端 ID
            client_secret = configs[3].Trim(); // 客户端密钥 / 客户端密码
            partner_code = configs[4].Trim();// 合作方编码 / 合作伙伴代码
            // 数据来源系统名称（自营/加盟），选填
            source_system = configs.Length > 5 ? configs[5].Trim() : "";

            result.Status = HandleStatus.Completed;
            return result;


        }

        /// <summary>
        /// 获取有效的 access_token。
        /// 若缓存有效则直接返回；否则请求新 Token。
        /// 提前 5 分钟刷新，避免传输中途失效（文档要求）。
        /// </summary>
        public void GetAccessToken()
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
                request.AddParameter("scope", TokenScope);

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

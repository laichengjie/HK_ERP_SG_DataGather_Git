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

        /// <summary>单次 HTTP 请求超时（毫秒）</summary>
        private const int RequestTimeoutMs = 3 * 60 * 1000;

        /// <summary>OAuth scope 固定值（文档 第一步）</summary>
        private const string TokenScope = "data_management:edagc.pde.ingest::create:";

        /// <summary>X-Auth-Type 固定值（文档 3.1）</summary>
        private const string AuthType = "oscar";

        #endregion

        string ClientId = "";   // 客户端标识 / 客户端 ID
        string ClientSecret = ""; // 客户端密钥 / 客户端密码
        string PartnerCode = ""; // 合作方编码 / 合作伙伴代码
        string SourceSystem = ""; // 数据来源系统名称（自营/加盟）
        string GetTokenUrl = ""; // 获取访问令牌的 URL
        string ApiUrl = ""; // API 接口的 URL

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
            List<BasVendor> ListVendor = VendorManager.GetList(s => s.AllowUsed == true && s.ReportPath == 29);
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
            HandleArgs result = new HandleArgs();
            try
            {
                GatNikeApiLogManager gatlwManager = new GatNikeApiLogManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                result = GetParameterValue(solution.VendorID);
                if (result.Status == HandleStatus.Fail)
                {
                    return result;
                }
                GetAccessToken();
                if (string.IsNullOrEmpty(Token.access_token))
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "获取Token失败";
                    return result;
                }

                var dataList = GetPosData(solutionId, startTime, endTime, idsStr);
                if (dataList.Item1.Count > 0)
                {
                    //公共技术字段：同一批次共用一个 batch_no（文档 3.2）
                    string batchNo = Guid.NewGuid().ToString();
                    string batchDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    int totalRequests = dataList.Item2.Count;
                    int requestSeq = 0;

                    foreach (var item in dataList.Item2)
                    {
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId
                                                                && s.CompanyID == item.CompanyID
                                                                && s.BillNo == item.BillNo
                                                                && s.BillType == BillTypePos);
                        if (sgData == null)
                        {
                            sgData = new GatNikeApiLog()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillType = BillTypePos,//单据类型
                                BillTypeID = item.BillTypeID,//单据类型ID
                                BillTypeName = item.BillTypeName,//单据类型名称
                                BillNo = item.BillNo,//线上订单号
                                BillDate = item.BillDate,//订单时间
                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                PushBillNo = item.PushBillNo,//推送单号
                                IsSuccess = false,
                            };
                        }
                        else
                        {
                            sgData.Qty = item.Qty;//数量
                            sgData.Amount = item.Amount;//金额
                        }

                        try
                        {
                            decimal totalAmount = 0;
                            int totalQty = 0;

                            NikeApiRequest<NikeApiRequest_Pos> Request = new NikeApiRequest<NikeApiRequest_Pos>();

                            List<NikeApiRequest_Pos> detailList = new List<NikeApiRequest_Pos>();

                            var details = dataList.Item1.FindAll(s => s.BillNo == item.BillNo && s.CompanyID == item.CompanyID);
                            foreach (var detail in details)
                            {
                                NikeApiRequest_Pos Pos = new NikeApiRequest_Pos()
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
                                    original_line_item_nbr = 0,
                                    last_updated_tmst = (detail.LAST_UPDATED_TMST ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")
                                };
                                if (!string.IsNullOrEmpty(detail.ORIGINAL_LINE_ITEM_NBR))
                                {
                                    int.TryParse(detail.ORIGINAL_LINE_ITEM_NBR, out int originalLineItemNbr);
                                    Pos.original_line_item_nbr = originalLineItemNbr;
                                }
                                totalAmount += detail.NET_SALES_AMT_INCL_TAX ?? 0;
                                totalQty += detail.SALES_QTY ?? 0;
                                detailList.Add(Pos);
                            }
                            Request.records = detailList;

                            #region 公共技术字段（文档 3.2）
                            requestSeq++;
                            string requestId = Guid.NewGuid().ToString();
                            Request.batch_no = batchNo;//批次号（UUID v4），本批次共用
                            Request.batch_date = batchDate;//Current Date - 1 Day
                            Request.request_seq = requestSeq;//请求序号
                            Request.request_id = requestId;//与 Header 的 X-Request-ID 一致
                            Request.total_requests = totalRequests;//本批次预计的总请求数
                            Request.partner_code = PartnerCode;//合作方唯一标识码
                            Request.data_type = DataTypePos;//pos
                            Request.record_count = detailList.Count;//records 数组行数
                            Request.amount_sum = Math.Round(totalAmount, 2);//NET_SALES_AMT_INCL_TAX 求和
                            Request.quantity_sum = totalQty;//SALES_QTY 求和
                            Request.schema_version = SchemaVersion;//业务数据结构版本
                            Request.source_system = SourceSystem;//数据来源系统名称（自营/加盟）
                            Request.send_timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");//ISO 8601 含时区
                            #endregion

                            string content = JsonConvert.SerializeObject(Request);
                            WebHeaderCollection headerValue = GetWebHeader(requestId);
                            string Response = HttpPost(ApiUrl, content, headerValue);
                            var ResultData = JsonConvert.DeserializeObject<NikeApiResponse>(Response);

                            if (ResultData != null && ResultData.code == CodeSuccess)
                            {
                                sgData.IsSuccess = true;
                                UpdateNikeLogState(item.CompanyID, item.BillNo, BillTypePos, item.BillTypeID);
                            }
                            else
                            {
                                sgData.IsSuccess = false;
                            }
                            sgData.PostJaon = "地址：" + ApiUrl + "报文：" + content;
                            sgData.ReturnJson = Response;
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
                        Script = $"sp_SGIS_NikeApi_GetData{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "NIke Pos推送"
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
            HandleArgs result = new HandleArgs();
            try
            {
                GatNikeApiLogManager gatlwManager = new GatNikeApiLogManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                result = GetParameterValue(solution.VendorID);
                if (result.Status == HandleStatus.Fail)
                {
                    return result;
                }
                GetAccessToken();
                if (string.IsNullOrEmpty(Token.access_token))
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "获取Token失败";
                    return result;
                }

                var dataList = GetInventoryData(solutionId, startTime, endTime, idsStr);
                if (dataList.Item1.Count > 0)
                {
                    //公共技术字段：同一批次共用一个 batch_no（文档 3.2）
                    string batchNo = Guid.NewGuid().ToString();
                    string batchDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    int totalRequests = dataList.Item2.Count;
                    int requestSeq = 0;

                    foreach (var item in dataList.Item2)
                    {
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId
                                                                && s.CompanyID == item.CompanyID
                                                                && s.BillNo == item.BillNo
                                                                && s.BillType == BillTypeInv);
                        if (sgData == null)
                        {
                            sgData = new GatNikeApiLog()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillType = BillTypeInv,//单据类型
                                BillTypeID = item.BillTypeID,//单据类型ID
                                BillTypeName = item.BillTypeName,//单据类型名称
                                BillNo = item.BillNo,//仓库ID
                                BillDate = item.BillDate,//库存日期
                                CardID = item.CardID,//品牌ID
                                StockID = item.StockID,//仓库ID
                                StockCode = item.StockCode,//仓库代码
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                PushBillNo = item.PushBillNo,//推送单号
                                IsSuccess = false,
                            };
                        }
                        else
                        {
                            sgData.Qty = item.Qty;//数量
                            sgData.Amount = item.Amount;//金额
                        }

                        try
                        {
                            int totalQty = 0;

                            NikeApiRequest<NikeApiRequest_Inv> Request = new NikeApiRequest<NikeApiRequest_Inv>();

                            List<NikeApiRequest_Inv> detailList = new List<NikeApiRequest_Inv>();

                            //库存按「公司+仓库」分组，日志的 BillNo 存的是 StockID
                            var details = dataList.Item1.FindAll(s => s.StockID == item.BillNo && s.CompanyID == item.CompanyID);
                            foreach (var detail in details)
                            {
                                NikeApiRequest_Inv Inv = new NikeApiRequest_Inv()
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
                                totalQty += detail.TOTAL_QTY ?? 0;
                                detailList.Add(Inv);
                            }
                            Request.records = detailList;

                            #region 公共技术字段（文档 3.2）
                            requestSeq++;
                            string requestId = Guid.NewGuid().ToString();
                            Request.batch_no = batchNo;//批次号（UUID v4），本批次共用
                            Request.batch_date = batchDate;//Current Date - 1 Day
                            Request.request_seq = requestSeq;//请求序号
                            Request.request_id = requestId;//与 Header 的 X-Request-ID 一致
                            Request.total_requests = totalRequests;//本批次预计的总请求数
                            Request.partner_code = PartnerCode;//合作方唯一标识码
                            Request.data_type = DataTypeInventory;//inventory
                            Request.record_count = detailList.Count;//records 数组行数
                            Request.amount_sum = 0;//inventory 不适用，传 0
                            Request.quantity_sum = totalQty;//TOTAL_QTY 求和
                            Request.schema_version = SchemaVersion;//业务数据结构版本
                            Request.source_system = SourceSystem;//数据来源系统名称（自营/加盟）
                            Request.send_timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");//ISO 8601 含时区
                            #endregion

                            string content = JsonConvert.SerializeObject(Request);
                            WebHeaderCollection headerValue = GetWebHeader(requestId);
                            string Response = HttpPost(ApiUrl, content, headerValue);
                            var ResultData = JsonConvert.DeserializeObject<NikeApiResponse>(Response);

                            if (ResultData != null && ResultData.code == CodeSuccess)
                            {
                                sgData.IsSuccess = true;
                                UpdateNikeLogState(item.CompanyID, item.BillNo, BillTypeInv, item.BillTypeID);
                            }
                            else
                            {
                                sgData.IsSuccess = false;
                            }
                            sgData.PostJaon = "地址：" + ApiUrl + "报文：" + content;
                            sgData.ReturnJson = Response;
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
                        Script = $"sp_SGIS_NikeApi_GetData{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "NIke Inv推送"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "Inv提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "Inv重传成功:未获取到数据";
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
            HandleArgs result = new HandleArgs();
            try
            {
                GatNikeApiLogManager gatlwManager = new GatNikeApiLogManager();
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                result = GetParameterValue(solution.VendorID);
                if (result.Status == HandleStatus.Fail)
                {
                    return result;
                }
                GetAccessToken();
                if (string.IsNullOrEmpty(Token.access_token))
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "获取Token失败";
                    return result;
                }

                var dataList = GetOutInData(solutionId, startTime, endTime, idsStr);
                if (dataList.Item1.Count > 0)
                {
                    //公共技术字段：同一批次共用一个 batch_no（文档 3.2）
                    string batchNo = Guid.NewGuid().ToString();
                    string batchDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    int totalRequests = dataList.Item2.Count;
                    int requestSeq = 0;

                    foreach (var item in dataList.Item2)
                    {
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId
                                                                && s.CompanyID == item.CompanyID
                                                                && s.BillNo == item.BillNo
                                                                && s.BillType == BillTypeOutIn);
                        if (sgData == null)
                        {
                            sgData = new GatNikeApiLog()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillType = BillTypeOutIn,//单据类型
                                BillTypeID = item.BillTypeID,//单据类型ID
                                BillTypeName = item.BillTypeName,//单据类型名称
                                BillNo = item.BillNo,//出入库单号
                                BillDate = item.BillDate,//单据时间
                                CardID = item.CardID,//品牌ID
                                StockID = item.StockID,//仓库ID
                                StockCode = item.StockCode,//仓库代码
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                PushBillNo = item.PushBillNo,//推送单号
                                IsSuccess = false,
                            };
                        }
                        else
                        {
                            sgData.Qty = item.Qty;//数量
                            sgData.Amount = item.Amount;//金额
                        }

                        try
                        {
                            int totalQty = 0;

                            NikeApiRequest<NikeApiRequest_OutIn> Request = new NikeApiRequest<NikeApiRequest_OutIn>();

                            List<NikeApiRequest_OutIn> detailList = new List<NikeApiRequest_OutIn>();

                            var details = dataList.Item1.FindAll(s => s.BillNo == item.BillNo
                                                                      && s.CompanyID == item.CompanyID
                                                                      && s.BillTypeID == item.BillTypeID);
                            foreach (var detail in details)
                            {
                                NikeApiRequest_OutIn OutIn = new NikeApiRequest_OutIn()
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
                                totalQty += detail.TRANSACTION_QTY ?? 0;
                                detailList.Add(OutIn);
                            }
                            Request.records = detailList;

                            #region 公共技术字段（文档 3.2）
                            requestSeq++;
                            string requestId = Guid.NewGuid().ToString();
                            Request.batch_no = batchNo;//批次号（UUID v4），本批次共用
                            Request.batch_date = batchDate;//Current Date - 1 Day
                            Request.request_seq = requestSeq;//请求序号
                            Request.request_id = requestId;//与 Header 的 X-Request-ID 一致
                            Request.total_requests = totalRequests;//本批次预计的总请求数
                            Request.partner_code = PartnerCode;//合作方唯一标识码
                            Request.data_type = DataTypeMovement;//movement
                            Request.record_count = detailList.Count;//records 数组行数
                            Request.amount_sum = 0;//movement 不适用，传 0
                            Request.quantity_sum = totalQty;//transaction_qty 求和
                            Request.schema_version = SchemaVersion;//业务数据结构版本
                            Request.source_system = SourceSystem;//数据来源系统名称（自营/加盟）
                            Request.send_timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");//ISO 8601 含时区
                            #endregion

                            string content = JsonConvert.SerializeObject(Request);
                            WebHeaderCollection headerValue = GetWebHeader(requestId);
                            string Response = HttpPost(ApiUrl, content, headerValue);
                            var ResultData = JsonConvert.DeserializeObject<NikeApiResponse>(Response);

                            if (ResultData != null && ResultData.code == CodeSuccess)
                            {
                                sgData.IsSuccess = true;
                                UpdateNikeLogState(item.CompanyID, item.BillNo, BillTypeOutIn, item.BillTypeID);
                            }
                            else
                            {
                                sgData.IsSuccess = false;
                            }
                            sgData.PostJaon = "地址：" + ApiUrl + "报文：" + content;
                            sgData.ReturnJson = Response;
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
                        Script = $"sp_SGIS_NikeApi_GetData{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "NIke OutIn推送"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "OutIn提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "OutIn重传成功:未获取到数据";
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
            headerValue.Add("X-Partner-Code", PartnerCode);//合作方唯一标识码，与 Body 的 PartnerCode 必须一致
            headerValue.Add("X-Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//请求发起时间
            headerValue.Add("Content-Encoding", "gzip");//固定值，请求体需 gzip 压缩
            return headerValue;
        }


        /// <summary>
        /// POST 报文到 Nike（请求体按 Header Content-Encoding: gzip 压缩后发送）
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="content">请求体 JSON</param>
        /// <param name="headerValue">请求头</param>
        /// <returns>响应原文</returns>
        public string HttpPost(string url, string content, WebHeaderCollection HeaderValue)
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
                    return reader.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                //401/403/422/429/500 等 HTTP 错误响应：仍返回响应体，便于解析业务状态码 code
                HttpWebResponse errorResponse = wex.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                throw new Exception($"发送请求异常,message:{wex.Message}");
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
        public HandleArgs GetParameterValue(int? VendorID)
        {
            HandleArgs result = new HandleArgs(); 
            BasVendorParameterValueManager ParameterValueManager = new BasVendorParameterValueManager();
            List<BasVendParameterValue> ListValue = ParameterValueManager.GetList(s => s.VendorID == VendorID);
            ClientId = ListValue.Where(s=>s.ParameterCode== "ClientId")?.FirstOrDefault()?.ParameterValue;   // 客户端标识 / 客户端 ID
            ClientSecret = ListValue.Where(s=>s.ParameterCode== "ClientSecret")?.FirstOrDefault()?.ParameterValue; // 客户端密钥 / 客户端密码
            PartnerCode = ListValue.Where(s=>s.ParameterCode== "PartnerCode")?.FirstOrDefault()?.ParameterValue; // 合作方编码 / 合作伙伴代码
            SourceSystem = ListValue.Where(s=>s.ParameterCode== "SourceSystem")?.FirstOrDefault()?.ParameterValue; // 数据来源系统名称（自营/加盟）
            GetTokenUrl = ListValue.Where(s=>s.ParameterCode== "GetTokenUrl")?.FirstOrDefault()?.ParameterValue; // 获取访问令牌的 URL
            ApiUrl = ListValue.Where(s=>s.ParameterCode== "ApiUrl")?.FirstOrDefault()?.ParameterValue; // API 接口的 URL
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
                if (string.IsNullOrWhiteSpace(GetTokenUrl))
                {
                    throw new Exception("未配置获取Token的地址");
                }

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                // 1. Basic 认证：Base64(ClientId:ClientSecret)
                var basic = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

                // 2. 组装请求（与 HttpPost 同一套 RestSharp 写法）
                var client = new RestClient(GetTokenUrl);
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

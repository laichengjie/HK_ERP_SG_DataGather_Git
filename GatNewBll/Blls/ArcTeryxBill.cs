using Gat.Utils;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.ArcTeryx;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Serializers;
using SqlSugar;
using GatNewBll.Models;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Globalization;

namespace GatNewBll.Blls
{
    /// <summary>
    /// 华润置地开放平台接口
    /// </summary>
    public class ArcTeryxBill : BaseBll
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
        public List<GatArcTeryxPosSalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode
            , string CardShopCode, string billNo, string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatArcTeryxPosSalePageModel> result = new List<GatArcTeryxPosSalePageModel>();
            GatArcTeryxPosSaleManager manager = new GatArcTeryxPosSaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.SaleDate >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.SaleDate <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                data = data.Where(o => o.StoreCode.Contains(CardShopCode));
            }
            if (!string.IsNullOrEmpty(billNo))
            {
                data = data.Where(o => o.BillNo.Contains(billNo));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }

            totalQty = 0;
            totalAmount = 0;
            if (data.Count() > 0)
            {
                totalQty = data.Sum(o => o.Qty);
                totalAmount = data.Sum(o => o.Amount);
            }

            result = data.Select((o) => new GatArcTeryxPosSalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.SaleDate, o.StoreCode, o.BillNo })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }


        /// <summary>
        ///  自动推送 华润置地 所有提报方案
        /// </summary>
        /// <param name="solutionId"></param> 
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>  
        /// <returns></returns>
        public HandleArgs PushAllArcTeryxPos(int solutionId, DateTime startTime, DateTime endTime, bool IsPushStock = false)
        {
            GatSolutionManager SolutionManager = new GatSolutionManager();
            BasVendorManager VendorManager = new BasVendorManager();
            List<BasVendor> ListVendor = VendorManager.GetList(s => s.AllowUsed == true && s.ReportPath == 21);
            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            foreach (BasVendor vendor in ListVendor)
            {
                List<GatSolution> ListSolution = SolutionManager.GetList(s => s.Enabled==true && s.VendorID == vendor.Id);
                foreach (GatSolution Solution in ListSolution)
                {
                    try
                    {
                        HandleArgs Posresult = new HandleArgs();
                        Posresult = NormalPosPost(Solution.Id, startTime, endTime);
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
        /// 销售库存数据推送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public HandleArgs NormalArcTeryxPost(int solutionId, DateTime startTime, DateTime endTime, bool IsPushStock = false)
        {
            HandleArgs Posresult = new HandleArgs();
            Posresult = NormalPosPost(solutionId, startTime, endTime);
            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            if (Posresult.Status == HandleStatus.Fail)
            {
                result.Status = HandleStatus.Fail;
            }
            result.Message = Posresult.Message;
            return result;
        }

        public HandleArgs Repost(int solutionId, List<int> ids)
        {


            HandleArgs result = new HandleArgs();
            try
            {
                GatArcTeryxPosSaleManager gatlwManager = new GatArcTeryxPosSaleManager();
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
                string url = configs[0];       //接口地址
                string signKey = configs[1];   //秘钥
                string Partner_ID = configs[2];//合作伙伴身份标识
                string Api_Version = configs[3]; //调用的API版本号
                string App_Sub_ID = configs[4]; //API调用方应用的编码，同App_ID
                string Api_ID = configs[5];      //API调用的API编码
                string App_Token = configs[6];// API调用方授权令牌
                string App_Pub_ID = configs[7]; //被调用API的应用编码
                string Sys_ID = configs[8];  //Sys_ID
                string Sign_Method = configs[9];//加密方式
                string Format = configs[10];//数据格式
                if (string.IsNullOrEmpty(url))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "接口地址不能为空"
                    };
                }

                var idsStr = string.Join(",", ids);
                var dataList = GetPostData(solutionId, idsStr);
                bool IsOk = true;
                bool IsUpdate = false;
                bool IsImport = false;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        IsUpdate = false;
                        IsOk = true;
                        IsImport = item.IsImport;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        if (sgData == null)
                        {
                            sgData = new GatArcTeryxPosSale()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillNo = item.BillNo,//单号
                                CashierId = item.CashierId,//收银员编码
                                CheckCode = item.CheckCode,//店铺验证密钥
                                Comments = item.Comments,//备注
                                TillId = item.TillId,//收银机编号
                                SaleDate = item.SaleDate,//销售日期
                                StoreCode = item.StoreCode,//品牌店铺编号
                                Saletype = item.Saletype,//销售类型
                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码
                                CreateDtm = DateTime.Now,
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                IsImport = item.IsImport,//是否导入
                                Mall = item.Mall,//是否导入
                                PushBillNo = item.PushBillNo,//推送单号
                            };
                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        if (IsOk)
                        {
                            try
                            {
                                int totalQty = 0;
                                decimal totalAmount = 0;
                                ArcTeryxBillDate Maste = new ArcTeryxBillDate()
                                {
                                    cashierId = item.CashierId.ToString(),//品牌方 收银员编码
                                    checkCode = item.CheckCode.ToString(),//品牌方 店铺验证密钥
                                    comments = item.Comments.ToString(),//备注
                                    mall = item.Mall.ToString(),//品牌方 商场编码
                                    orderId = item.BillNo.ToString(),//POS单号
                                    store = item.StoreCode.ToString(),//品牌方 店铺编号
                                    tillId = item.TillId.ToString(),//品牌方 收银机编号
                                    time = item.SaleDate.ToString("yyyyMMddHHmmss"),//销售日期
                                    totalAmt = Convert.ToDecimal(item.Amount),//订单总金额
                                    type = item.Saletype.ToString(),//订单类型
                                    itemList = new List<ArcTeryxItemDetail>(),
                                    payList = new List<ArcTeryxPayDetail>(),

                                };

                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo && s.CompanyID == item.CompanyID);
                                foreach (var itemDetail in details)
                                {
                                    Maste.itemList.Add(new ArcTeryxItemDetail()
                                    {
                                        itemCode = itemDetail.ItemCode,//商品编号(ERP货号)
                                        quantity = itemDetail.Quantity,//销售数量
                                        price = itemDetail.Price,//商品价格（结算价）
                                    });
                                }

                                var payList = dataList.Item3.FindAll(s => s.BillNo == item.BillNo && s.CompanyID == item.CompanyID);
                                foreach (var pay in payList)
                                {
                                    Maste.payList.Add(new ArcTeryxPayDetail()
                                    {
                                        cardBank = pay.CardBank,//支付银行
                                        cardNumber = pay.CardNumber,//银行卡卡号
                                        discountAmt = pay.DiscountAmt,//优惠金额
                                        payAmt = pay.PayAmt,//应收金额
                                        paymentMethod = pay.PaymentMethod,
                                        time = item.SaleDate.ToString("yyyyMMddHHmmss"),//销售日期,//支付时间  销售日期时间 格式：	yyyyMMddhhmmss	
                                        value = pay.value,//支付金额（实收金额）
                                    });
                                }

                                // 获取当前时间（UTC+8时区）
                                DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                                // 格式化为指定格式（yyyy-MM-dd HH:mm:ss:fff）
                                string Time_Stamp = now.ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);
                                string REQUEST_DATA = JsonConvert.SerializeObject(Maste);
                                string Sign = GetSign(signKey, App_Sub_ID, App_Token, Api_ID, Api_Version, Time_Stamp, Sign_Method, Format, Partner_ID, Sys_ID, App_Pub_ID, REQUEST_DATA);

                                HrtAttrs HRT_ATTRS = new HrtAttrs()
                                {
                                    Partner_ID = Partner_ID,// 合作伙伴身份标识
                                    Api_Version = Api_Version,// 调用的API版本号
                                    App_Sub_ID = App_Sub_ID,//API调用方应用的编码，同App_ID
                                    Format = Format,// 响应格式, 默认为json格式，可选值：xml或json
                                    Api_ID = Api_ID,// API调用的API编码
                                    App_Token = App_Token,// API调用方授权令牌
                                    App_Pub_ID = App_Pub_ID,//被调用API的应用编码
                                    Sign_Method = Sign_Method,
                                    Time_Stamp = Time_Stamp,
                                    Sys_ID = Sys_ID,
                                    Sign = Sign,//被调用API的应用编码

                                };

                                ArcTeryxPosDate PosDate = new ArcTeryxPosDate
                                {
                                    REQUEST_DATA = Maste,
                                    HRT_ATTRS = HRT_ATTRS,
                                };
                                ArcTeryxPosRequest wReq = new ArcTeryxPosRequest()
                                {
                                    REQUEST = PosDate
                                };


                                var postResult = PostData<ArcTeryxPosResponse>(url, wReq);
                                if (postResult.RETURN_CODE == "")
                                {
                                    sgData.IsSuccess = true;
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
                        Script = $"sp_SGIS_ArcTeryx_GetPosSale" + string.Join(";", ids),
                        Remark = "华润置地数据提报NormalPost"
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
        /// POS数据推送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public HandleArgs NormalPosPost(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatArcTeryxPosSaleManager gatlwManager = new GatArcTeryxPosSaleManager();
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
                string url = configs[0];//"http://ztopenapiuat.crland.com.cn:81/api-gateway/rs-service/";
                string signKey = configs[1];// "0bca40d57d1f44208d787a4e0a87957d";
                string Partner_ID = configs[2];// "70000006";
                string Api_Version = configs[3];// "1.0.0";
                string App_Sub_ID = configs[4];//"10000133334PY";
                string Api_ID = configs[5];//"mixc.imPOSWBJB.GLWXCJB.orderCollect";
                string App_Token = configs[6];//"c861e8a4be0f41b182abdb55b986444a";
                string App_Pub_ID = configs[7];// "10000133301US";
                string Sys_ID = configs[8];//"100001333";
                string Sign_Method = configs[9];//"md5";
                string Format = configs[10];//"json";
                if (string.IsNullOrEmpty(url))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "接口地址不能为空"
                    };
                }


                var dataList = GetPostData(solutionId, startTime, endTime);

                bool IsOk = true;
                bool IsUpdate = false;
                bool IsImport = false;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        IsUpdate = false;
                        IsOk = true;
                        IsImport = item.IsImport;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        if (sgData == null)
                        {
                            sgData = new GatArcTeryxPosSale()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillNo = item.BillNo,//单号
                                CashierId = item.CashierId,//收银员编码
                                CheckCode = item.CheckCode,//店铺验证密钥
                                Comments = item.Comments,//备注
                                TillId = item.TillId,//收银机编号
                                SaleDate = item.SaleDate,//销售日期
                                StoreCode = item.StoreCode,//品牌店铺编号
                                Saletype = item.Saletype,//销售类型
                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码
                                CreateDtm = DateTime.Now,
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                IsImport = item.IsImport,//是否导入
                                Mall = item.Mall,//是否导入
                                PushBillNo=item.PushBillNo,//推送单号
                            };
                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        if (IsOk)
                        {
                            try
                            {
                                int totalQty = 0;
                                decimal totalAmount = 0;
                                ArcTeryxBillDate Maste = new ArcTeryxBillDate()
                                {
                                    cashierId = item.CashierId.ToString(),//品牌方 收银员编码
                                    checkCode = item.CheckCode.ToString(),//品牌方 店铺验证密钥
                                    comments = item.Comments.ToString(),//备注
                                    mall = item.Mall.ToString(),//品牌方 商场编码
                                    orderId = item.BillNo.ToString(),//POS单号
                                    store = item.StoreCode.ToString(),//品牌方 店铺编号
                                    tillId = item.TillId.ToString(),//品牌方 收银机编号
                                    time = item.SaleDate.ToString("yyyyMMddHHmmss"),//销售日期
                                    totalAmt = Convert.ToDecimal(item.Amount),//订单总金额
                                    type = item.Saletype.ToString(),//订单类型
                                    itemList = new List<ArcTeryxItemDetail>(),
                                    payList = new List<ArcTeryxPayDetail>(),

                                };

                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo && s.CompanyID == item.CompanyID);
                                foreach (var itemDetail in details)
                                {
                                    Maste.itemList.Add(new ArcTeryxItemDetail()
                                    {
                                        itemCode = itemDetail.ItemCode,//商品编号(ERP货号)
                                        quantity = itemDetail.Quantity,//销售数量
                                        price = itemDetail.Price,//商品价格（结算价）
                                    });
                                }

                                var payList = dataList.Item3.FindAll(s => s.BillNo == item.BillNo && s.CompanyID == item.CompanyID);
                                foreach (var pay in payList)
                                {
                                    Maste.payList.Add(new ArcTeryxPayDetail()
                                    {
                                        cardBank = pay.CardBank,//支付银行
                                        cardNumber = pay.CardNumber,//银行卡卡号
                                        discountAmt = pay.DiscountAmt,//优惠金额
                                        payAmt = pay.PayAmt,//应收金额
                                        paymentMethod = pay.PaymentMethod,
                                        time = item.SaleDate.ToString("yyyyMMddHHmmss"),//销售日期,//支付时间  销售日期时间 格式：	yyyyMMddhhmmss	
                                        value = pay.value,//支付金额（实收金额）
                                    });
                                }

                                // 获取当前时间（UTC+8时区）
                                DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                                // 格式化为指定格式（yyyy-MM-dd HH:mm:ss:fff）
                                string Time_Stamp = now.ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);
                                string REQUEST_DATA = JsonConvert.SerializeObject(Maste);
                                string Sign = GetSign(signKey, App_Sub_ID, App_Token, Api_ID, Api_Version, Time_Stamp, Sign_Method, Format, Partner_ID, Sys_ID, App_Pub_ID, REQUEST_DATA);

                                HrtAttrs HRT_ATTRS = new HrtAttrs()
                                {
                                    Partner_ID = Partner_ID,// 合作伙伴身份标识
                                    Api_Version = Api_Version,// 调用的API版本号
                                    App_Sub_ID = App_Sub_ID,//API调用方应用的编码，同App_ID
                                    Format = Format,// 响应格式, 默认为json格式，可选值：xml或json
                                    Api_ID = Api_ID,// API调用的API编码
                                    App_Token = App_Token,// API调用方授权令牌
                                    App_Pub_ID = App_Pub_ID,//被调用API的应用编码
                                    Sign_Method = Sign_Method,
                                    Time_Stamp = Time_Stamp,
                                    Sys_ID = Sys_ID,
                                    Sign = Sign,//被调用API的应用编码

                                };

                                ArcTeryxPosDate PosDate = new ArcTeryxPosDate
                                {
                                    REQUEST_DATA = Maste,
                                    HRT_ATTRS = HRT_ATTRS,
                                };
                                ArcTeryxPosRequest wReq = new ArcTeryxPosRequest()
                                {
                                    REQUEST = PosDate
                                };


                                var postResult = PostData<ArcTeryxPosResponse>(url, wReq);
                                if (postResult != null && postResult.RETURN_DATA != null && postResult.RETURN_DATA.header != null && postResult.RETURN_DATA.header.errcode == "0000")
                                {
                                    sgData.IsSuccess = true;
                                }
                                else
                                {
                                    sgData.IsSuccess = false;
                                }
                                sgData.PostJaon = "地址："+ url+ "报文："+JsonConvert.SerializeObject(wReq);
                                sgData.ReturnJson = JsonConvert.SerializeObject(postResult);
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
                        Script = $"sp_SGIS_ArcTeryx_GetPosSale{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "华润置地数据提报NormalPost"
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
        /// 获取POS提报数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Tuple<List<ArcTeryxPosMasterData>, List<ArcTeryxPosDetailData>, List<ArcTeryxPosReceiptData>> GetPostData(int solutionId, string ids)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<ArcTeryxPosMasterData, ArcTeryxPosDetailData, ArcTeryxPosReceiptData>("sp_SGIS_ArcTeryx_GetPosSale",
                    new { SolutionId = solutionId, Ids = ids });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常");
            }
        }

        /// <summary>
        /// 获取提报POS数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<ArcTeryxPosMasterData>, List<ArcTeryxPosDetailData>, List<ArcTeryxPosReceiptData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<ArcTeryxPosMasterData, ArcTeryxPosDetailData, ArcTeryxPosReceiptData>("sp_SGIS_ArcTeryx_GetPosSale",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常" + ex.Message);
            }
        }








        /// <summary>
        /// 请求接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="username">店铺代码</param>
        /// <param name="SecretKey">秘钥</param>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <param name="type">类型 3:订单提交, 5:库存提交</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ArcTeryxPosResponse PostData<T>(string url, object data)
        {
            try
            {
                string StrUrl = url;
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(data, Formatting.Indented, jsetting);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                var client = new RestClient(StrUrl);
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json;charset=UTF-8");
                request.AddStringBody(json, ContentType.Json);
                var response = client.ExecutePostAsync<ArcTeryxPosResponse>(request).GetAwaiter().GetResult();
                if (response == null)
                {
                    throw new Exception($"请求接口失败");
                }
                return response.Data;

            }
            catch (Exception ex)
            {
                throw new Exception($"发送请求异常,message:{ex.Message},trace:{ex.StackTrace}");
            }
        }




        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="signKey"></param>
        /// <param name="App_Sub_ID"></param>
        /// <param name="App_Token"></param>
        /// <param name="Api_ID"></param>
        /// <param name="Api_Version"></param>
        /// <param name="Time_Stamp"></param>
        /// <param name="Sign_Method"></param>
        /// <param name="Format"></param>
        /// <param name="Partner_ID"></param>
        /// <param name="Sys_ID"></param>
        /// <param name="App_Pub_ID"></param>
        /// <param name="REQUEST_DATA"></param>
        /// <returns></returns>
        private string GetSign(string signKey, string App_Sub_ID, string App_Token, string Api_ID, string Api_Version, string Time_Stamp, string Sign_Method, string Format, string Partner_ID, string Sys_ID, string App_Pub_ID, string REQUEST_DATA)
        {
            Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            map.Add("App_Sub_ID", App_Sub_ID);
            map.Add("App_Token", App_Token);
            map.Add("Api_ID", Api_ID);
            map.Add("Api_Version", Api_Version);
            map.Add("Time_Stamp", Time_Stamp);
            map.Add("Sign_Method", Sign_Method);
            map.Add("Format", Format);
            map.Add("Partner_ID", Partner_ID);
            map.Add("Sys_ID", Sys_ID);
            map.Add("App_Pub_ID", App_Pub_ID);
            map.Add("REQUEST_DATA", REQUEST_DATA);
            List<string> keyList = new List<string>(map.Keys);
            keyList.Sort(StringComparer.OrdinalIgnoreCase);

            StringBuilder signStr = new StringBuilder();
            foreach (string key in keyList)
            {
                signStr.Append(key);
                signStr.Append("=");
                signStr.Append(map[key]);
                signStr.Append("&");
            }
            signStr.Append(signKey);

            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(signStr.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 获取更新时间
        /// </summary>
        /// <param name="SecretKey">秘钥</param>
        /// <returns></returns>
        public string GetUpdatedAt()
        {
            string UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return UpdatedAt;
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

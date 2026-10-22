using Gat.Utils;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.On;
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

namespace GatNewBll.Blls
{
    /// <summary>
    /// On昂跑开放平台接口
    /// </summary>
    public class OnBill : BaseBll
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
        public List<GatOnPosSalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode
            , string CardShopCode, string billNo, string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatOnPosSalePageModel> result = new List<GatOnPosSalePageModel>();
            GatOnPosSaleManager manager = new GatOnPosSaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.sale_date >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.sale_date <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                data = data.Where(o => o.store_code.Contains(CardShopCode));
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
         
            result = data.Select((o) => new GatOnPosSalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.sale_date, o.store_code, o.BillNo })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }



        /// <summary>
        /// 获取库存列表数据
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
        public List<GatOnInvPageModel> GetPageInvList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode, string CardShopCode, string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatOnInvPageModel> result = new List<GatOnInvPageModel>();
            GatOnInvManager manager = new GatOnInvManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.inventory_date >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.inventory_date <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                data = data.Where(o => o.store_code.Contains(CardShopCode));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }
            //totalQty = data.Sum(o => o.Qty); 
            //totalAmount = data.Sum(o => o.Amount); 
            totalQty = 0;
            totalAmount = 0;
            if (data.Count() > 0)
            {
                totalQty = data.Sum(o => o.Qty);
            }
            result = data.Select((o) => new GatOnInvPageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderByDescending(o => new { o.inventory_date, o.ShopCode })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }
        public HandleArgs Repost(int solutionId, List<int> ids)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatOnPosSaleManager gatlwManager = new GatOnPosSaleManager();
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
                string app_id = configs[1];
                string app_secret = configs[2];
                string Token = GetToken(app_id, app_secret, url);
                if (string.IsNullOrEmpty(Token))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "获取Token失败"
                    };
                }

                var idsStr = string.Join(",", ids);
                var dataList = GetPostData(solutionId, idsStr);
                string aicode = "";
                string returnAicode = "";
                bool IsOk = true;
                bool IsUpdate = false;
                bool IsImport = false;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        aicode = "";
                        returnAicode = "";
                        IsUpdate = false;
                        IsOk = true;
                        IsImport = item.IsImport;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        if (sgData == null)
                        {
                            sgData = new GatOnPosSale()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillNo = item.BillNo,//单号
                                sale_date = item.sale_date,//销售日期
                                store_code = item.store_code,//品牌店铺编号
                                order_type = item.order_type,//销售类型
                                order_number = item.order_number,//单号
                                original_order_number = item.original_order_number,//源单号
                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码
                                CreateDtm = DateTime.Now,
                                Qty= item.Qty,//数量
                                Amount = item.Amount,//金额
                                IsImport = item.IsImport,//是否导入
                            };
                        }
                        if (IsOk)
                        {
                            try
                            {
                                int totalQty = 0;
                                decimal totalAmount = 0;
                                OnPosRequest Maste = new OnPosRequest()
                                {
                                    sale_date = item.sale_date.ToString("yyyyMMdd"),//销售日期
                                    store_code = item.store_code,//品牌店铺编号
                                    order_type = item.order_type,// 销售类型，0正常, 20退货
                                    order_number = item.order_number,//单号
                                    original_order_number = item.original_order_number,//单号
                                    updated_at = GetUpdatedAt(),//更新时间
                                    products = new List<OnPosRequest.OnPosDetail>(),

                                };
                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var itemDetail in details)
                                {
                                    Maste.products.Add(new OnPosRequest.OnPosDetail()
                                    {
                                        sku = itemDetail.sku,//SKU 
                                        quantity = itemDetail.quantity,//销售数量
                                        total_paid = itemDetail.total_paid,//业绩金额
                                        row_total = itemDetail.row_total,//吊牌价
                                    });
                                }
                                List<OnPosRequest> wReq = new List<OnPosRequest>();
                                wReq.Add(Maste);
                                var postResult = PostData<OnPosResponse>(url, wReq, Token, "/sales_order");
                                if (postResult.code == 200)
                                {
                                    sgData.IsSuccess = true;
                                    if (IsImport)
                                    {
                                        UpdatePosImportState(solutionId, item.CompanyID, item.BillNo, "Pos");
                                    }
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
                        Script = "sp_SGIS_On_GetPosSale" + string.Join(";", ids),
                        Remark = "On数据提报Repost"
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
                GatOnPosSaleManager gatlwManager = new GatOnPosSaleManager();
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
                string app_id = configs[1];
                string app_secret = configs[2];
                string Token = GetToken(app_id, app_secret, url);
                if (string.IsNullOrEmpty(Token))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "获取Token失败"
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
                            sgData = new GatOnPosSale()
                            {
                                SolutionId = solutionId,//提报方案
                                CompanyID = item.CompanyID,//公司
                                BillNo = item.BillNo,//单号
                                sale_date = item.sale_date,//销售日期
                                store_code = item.store_code,//品牌店铺编号
                                order_type = item.order_type,//销售类型
                                order_number = item.order_number,//单号
                                original_order_number = item.original_order_number,//源单号
                                CardID = item.CardID,//品牌ID
                                ShopID = item.ShopID,//店铺ID
                                ShopCode = item.ShopCode,//店铺代码
                                CreateDtm = DateTime.Now,
                                Qty = item.Qty,//数量
                                Amount = item.Amount,//金额
                                IsImport = item.IsImport,//是否导入
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
                                OnPosRequest Maste = new OnPosRequest()
                                {
                                    sale_date = item.sale_date.ToString("yyyyMMdd"),//销售日期
                                    store_code = item.store_code,//品牌店铺编号
                                    order_type = item.order_type,// 销售类型，0正常, 20退货
                                    order_number = item.BillNo,//单号
                                    original_order_number = item.original_order_number,//单号
                                    updated_at = GetUpdatedAt(),//时间戳
                                    products = new List<OnPosRequest.OnPosDetail>(),
                                };
                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var itemDetail in details)
                                {
                                    Maste.products.Add(new OnPosRequest.OnPosDetail()
                                    {
                                        sku = itemDetail.sku,//SKU 
                                        quantity = itemDetail.quantity,//销售数量
                                        total_paid = itemDetail.total_paid,//业绩金额
                                        row_total = itemDetail.row_total,//吊牌价
                                    });
                                }
                                List<OnPosRequest> wReq = new List<OnPosRequest>();
                                wReq.Add(Maste);
                                var postResult = PostData<OnPosResponse>(url, wReq, Token, "/sales_order");
                                if (postResult.code == 200)
                                {
                                    sgData.IsSuccess = true;
                                    if (IsImport)
                                    {
                                        UpdatePosImportState(solutionId, item.CompanyID, item.BillNo, "Pos");
                                    }
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
                        Script = $"sp_SGIS_On_GetPosSale{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "On数据提报NormalPost"
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
        /// On销售库存数据推送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public HandleArgs NormalOnPost(int solutionId, DateTime startTime, DateTime endTime ,bool IsPushStock=false)
        {
            HandleArgs Posresult = new HandleArgs();
            Posresult = NormalPosPost(solutionId, startTime, endTime);

            HandleArgs Invresult = new HandleArgs();
            if (IsPushStock)
            {
                Invresult = NormalInvPost(solutionId, startTime, endTime);
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
        /// 库存数据推送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public HandleArgs NormalInvPost(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatOnInvManager gatlwManager = new GatOnInvManager();
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

                string app_id = configs[1];
                string app_secret = configs[2];
                string Token = GetToken(app_id, app_secret, url);
                if (string.IsNullOrEmpty(Token))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "获取Token失败"
                    };
                }
                var dataList = GetInvData(solutionId, startTime, endTime);

                bool IsOk = false;
                int Number = 0;
                int NumberMax = 500;
                int Count = 0;
                int Qty = 0;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        IsOk = false;
                        GatOnMeterialinv sgData = new GatOnMeterialinv()
                        {
                            SolutionId = solutionId,
                            CompanyID = item.CompanyID,
                            store_code = item.store_code,//品牌提供的门店编号
                            inventory_date = item.inventory_date,
                            CardID = item.CardID,
                            ShopID = item.ShopID,
                            ShopCode = item.ShopCode,
                            DrewTime = item.DrewTime,
                        };

                        OnInvRequest Maste = new OnInvRequest()
                        {
                            store_code = item.store_code,//品牌提供的门店编号
                            inventory_date = item.inventory_date.ToString("yyyyMMdd"),// 库存结余时间
                            products = new List<OnInvRequest.OnInvDetai>(),
                        };
                        var details = dataList.Item2.FindAll(s => s.store_code == item.store_code);
                        Count = details.Count();
                        Number = 1;
                        foreach (var itemDetail in details)
                        {
                            Maste.products.Add(new OnInvRequest.OnInvDetai()
                            {
                                sku = itemDetail.sku,//SKU
                                quantity = itemDetail.quantity,//数量
                            });
                            Qty = Qty+itemDetail.quantity;
                            if (Number == NumberMax || Number == Count)
                            {
                                sgData.Qty = Qty;
                                IsOk = PushInv(url, Token, sgData, Maste);
                                Maste.products = new List<OnInvRequest.OnInvDetai>();
                                Count = Count - Number;
                                Number = 1;
                                Qty = 0;

                            }
                            else
                            {
                                Number = Number + 1;
                            }

                           
                        }
                        UpdatePosImportState(solutionId, item.CompanyID, item.store_code, "Stock");
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
                        Script = $"sp_SGIS_On_GetMaterailINV{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "On库存数据提报NormalInvPost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "On库存提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "On库存重传成功:未获取到数据";
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

        public bool PushInv(string url, string Token, GatOnMeterialinv sgData, OnInvRequest Maste)
        {
            bool IsOk = false;
            GatOnInvManager gatlwManager = new GatOnInvManager();
            List<OnInvRequest> wReq = new List<OnInvRequest>();
            wReq.Add(Maste);
            try
            {
                var postResult = PostData<OnInvResponse>(url, wReq, Token, "/store_inventory");
                if (postResult.code == 200)
                {
                    sgData.IsSuccess = true;
                    IsOk = true;
                }
                else
                {
                    sgData.IsSuccess = false;
                    IsOk = false;
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
            sgData.CreateDtm = DateTime.Now;
            gatlwManager.InsertOrUpdate(sgData);
            return IsOk;

        }

        /// <summary>
        /// 获取POS提报数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Tuple<List<OnPosMasterData>, List<OnPosDetailData>> GetPostData(int solutionId, string ids)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<OnPosMasterData, OnPosDetailData>("sp_SGIS_On_GetPosSale",
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
        private Tuple<List<OnPosMasterData>, List<OnPosDetailData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<OnPosMasterData, OnPosDetailData>("sp_SGIS_On_GetPosSale",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取销售数据异常");
            }
        }

        /// <summary>
        /// 获取提报库存数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Tuple<List<OnInvMasterData>, List<OnInvDetailData>> GetInvData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<OnInvMasterData, OnInvDetailData>("sp_SGIS_On_GetMaterailINV",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取库存数据异常");
            }
        }

        /// <summary>
        /// 回写导入POS 单导入状态
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="CompanyID"></param> 
        /// <param name="BillNo"></param> 
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string UpdatePosImportState(int solutionId, string CompanyID, string BillNo, string OperationType)
        {
            string Msg = "";
            try
            {

                Msg = Db.Ado.SqlQuery<string>($@" EXEC sp_Gat_On_Transfer_PosStock  @solutionId='{solutionId}',@TransIDstr= '{BillNo}',@Type=2 ,@OperationType= '{OperationType}'").FirstOrDefault();
            }
            catch (Exception ex)
            {
                Msg = " 回写导入POS 单导入状态失败:" + ex.Message;
            }
            return Msg;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="app_id"></param>
        /// <param name="app_secret"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetToken(string app_id, string app_secret, string url)
        {
            string Token = "";
            try
            {
                OnTokenRequest data = new OnTokenRequest()
                {
                    app_id = app_id,
                    app_secret = app_secret
                };
                var response = PostData<OnToken>(url, data, Token, "/auth");
                if (response != null && response.code == 200)
                {
                    Token = response.data.token;
                }
            }
            catch (Exception ex)
            {

            }

            return Token;
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
        public BaseResponse<T> PostData<T>(string url, object data, string Token, string type)
        {
            try
            {
                string StrUrl = url + type;
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(data, Formatting.Indented, jsetting);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                var client = new RestClient(StrUrl);
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(Token))
                {
                    request.AddHeader("Authorization", Token);
                }
                request.AddStringBody(json, ContentType.Json);


                var response = client.ExecutePostAsync<BaseResponse<T>>(request).GetAwaiter().GetResult();
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

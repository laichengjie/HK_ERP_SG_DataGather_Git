using Gat.Utils;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.TuMi;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Serializers;
using SqlSugar;
using GatNewBll.Models;
using System.Data.SqlClient;
using System.Data;

namespace GatNewBll.Blls
{
    /// <summary>
    /// TuMi途明开放平台接口
    /// </summary>
    public class TuMiBll : BaseBll
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
        public List<GatTuMiPosSalePageModel> GetPageList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode
            , string CardShopCode, string billNo, string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatTuMiPosSalePageModel> result = new List<GatTuMiPosSalePageModel>();
            GatTuMiPosSaleManager manager = new GatTuMiPosSaleManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.rq >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.rq <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                data = data.Where(o => o.store.Contains(CardShopCode));
            }
            if (!string.IsNullOrEmpty(billNo))
            {
                data = data.Where(o => o.BillNo.Contains(billNo));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }
            //totalQty = data.Sum(o => o.Qty);
            //totalAmount = data.Sum(o => o.Amount);
            totalQty = 0;
            totalAmount = 0;
            result = data.Select((o) => new GatTuMiPosSalePageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderBy(o => new { o.rq, o.aicode })
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
        public List<GatTuMiInvPageModel> GetPageInvList(int solutionId, int page, int pageSize, DateTime? startBillDate, DateTime? endBillDate, string shopCode , string CardShopCode,  string status, ref int totalCount, ref int? totalQty, ref decimal? totalAmount)
        {
            List<GatTuMiInvPageModel> result = new List<GatTuMiInvPageModel>();
            GatTuMiInvManager manager = new GatTuMiInvManager();
            var data = manager.AsQueryable()
                .Where(o => o.SolutionId == solutionId);
            if (startBillDate.HasValue)
            {
                data = data.Where(o => o.rq >= startBillDate);
            }
            if (endBillDate.HasValue)
            {
                data = data.Where(o => o.rq <= endBillDate);
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                data = data.Where(o => o.ShopCode.Contains(shopCode));
            }
            if (!string.IsNullOrEmpty(CardShopCode))
            {
                data = data.Where(o => o.store.Contains(CardShopCode));
            }
            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(o => o.IsSuccess == (status == "0" ? false : true));
            }
            //totalQty = data.Sum(o => o.Qty); 
            //totalAmount = data.Sum(o => o.Amount);
            totalQty = 0;
            totalAmount = 0;
            result = data.Select((o) => new GatTuMiInvPageModel()
            {
                Id = o.Id.SelectAll()
            }).OrderByDescending(o => new { o.rq, o.ShopCode })
              .ToPageList(page, pageSize, ref totalCount);
            return result;
        }
        public HandleArgs Repost(int solutionId, List<int> ids)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatTuMiPosSaleManager gatlwManager = new GatTuMiPosSaleManager();
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
                var secrectPara = configs[1];
                var idsStr = string.Join(",", ids);
                var dataList = GetPostData(solutionId, idsStr);
                string aicode = "";
                string returnAicode = "";
                bool IsOk = true;
                bool IsUpdate = false;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        aicode = "";
                        returnAicode = "";
                        IsUpdate = false;
                        IsOk = true;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        if (sgData == null)
                        {
                            aicode = GetAicode(item.store);
                            sgData = new GatTuMiPosSale()
                            {
                                SolutionId = solutionId,
                                CompanyID = item.CompanyID,
                                BillNo = item.BillNo,
                                CreateDtm = DateTime.Now,
                                aicode = aicode,//店内订单编号 
                                rq = item.rq,//交易日  期格式: 2024 - 4 - 21 9:34:54
                                deliveryTime = item.deliveryTime,//发货时间  期格式: 2024 - 4 - 21 9:34:54
                                memberCode = item.memberCode,//会员编号
                                saletype = item.saletype,// 销售类型，0正常, 20退货
                                store = item.store,//销售门店, 品牌提供的门店编号
                                returnAicode = item.returnAicode,//原始订单, 对应主订单的aicode
                                channel = item.channel,//固定值: MA
                                nianLingZu = item.nianLingZu,// 顾客的年龄组（传code 1.2.3...）,默认值: 0
                                gouWuMuDi = item.gouWuMuDi,//顾客的购物目的, 默认值:0
                                couponid = item.couponid,//优惠券id
                                uid = item.uid,//营业员ID 固定值"0"
                                uName = item.uName,//营业员姓名 固定值“”空字符串
                                CardID = item.CardID,
                                ShopID = item.ShopID,
                                ShopCode = item.ShopCode,
                                BillNo2 = item.BillNo2,
                                SourceBillNo = item.SourceBillNo,
                            };
                        }
                        else
                        {
                            aicode = sgData.aicode;
                        }
                        returnAicode = item.returnAicode;
                        if (string.IsNullOrEmpty(returnAicode) && !string.IsNullOrEmpty(item.SourceBillNo))
                        {
                            returnAicode = GetSourceAicode(solutionId, item.CompanyID, item.SourceBillNo);
                        }
                        if (!string.IsNullOrEmpty(item.SourceBillNo) && string.IsNullOrEmpty(returnAicode))
                        {
                            //sgData.IsSuccess = false;
                            //sgData.PostJaon = "未调用接口";
                            //sgData.ReturnJson = $"退货单中的来源单号【" + item.SourceBillNo + "】未推送";
                            //IsOk = false;
                            returnAicode = item.SourceBillNo;
                        }
                        if (IsOk)
                        {
                            try
                            {

                                int totalQty = 0;
                                decimal totalAmount = 0;
                                TuMiPosMaster Maste = new TuMiPosMaster()
                                {
                                    aicode = aicode,//店内订单编号
                                    rq = item.rq,//交易日  期格式: 2024 - 4 - 21 9:34:54
                                    deliveryTime = item.deliveryTime,//发货时间  期格式: 2024 - 4 - 21 9:34:54
                                    memberCode = item.memberCode,//会员编号
                                    saletype = item.saletype,// 销售类型，0正常, 20退货
                                    store = item.store,//销售门店, 品牌提供的门店编号
                                    returnAicode = returnAicode,//原始订单, 对应主订单的aicode
                                    channel = item.channel,//固定值: MA
                                    nianLingZu = item.nianLingZu,// 顾客的年龄组（传code 1.2.3...）,默认值: 0
                                    gouWuMuDi = item.gouWuMuDi,//顾客的购物目的, 默认值:0
                                    couponid = item.couponid,//优惠券id
                                    uid = item.uid,//营业员ID 固定值"0"
                                    uName = item.uName,//营业员姓名 固定值“”空字符串
                                    uCode = item.couponid,//品牌营业员工号固定值"0"
                                    orderdetail = new List<TuMiPosMaster.TuMiPosDetail>(),
                                    paymentdetail = new List<TuMiPosMaster.TuMiPosReceiptDetai>()
                                };
                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var itemDetail in details)
                                {
                                    Maste.orderdetail.Add(new TuMiPosMaster.TuMiPosDetail()
                                    {
                                        SKU = itemDetail.SKU,//SKU
                                        brand = itemDetail.brand,//商品的品牌,固定值: Tumi
                                        product_unit_price = itemDetail.product_unit_price,//商品的标准零售价，单价
                                        freermb = itemDetail.freermb,//折扣金额
                                        product_count = itemDetail.product_count,//销售数量【qty】
                                        amount = itemDetail.amount,//明细实收金额【折扣后金额】
                                        order_detail_price = itemDetail.order_detail_price,//明细总额【原总价】
                                        is_gift = itemDetail.is_gift,//是否是赠品,固定值0
                                        promotion_remark = itemDetail.promotion_remark,//促销活动说明
                                    });
                                }


                                var ReceiptDetais = dataList.Item3.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var ReceiptDetai in ReceiptDetais)
                                {
                                    Maste.paymentdetail.Add(new TuMiPosMaster.TuMiPosReceiptDetai()
                                    {
                                        payment_way = ReceiptDetai.payment_way,//支付方式名称
                                        payment_amount = ReceiptDetai.payment_amount,// 支付金额

                                    });
                                }
                                TuMiPosRequest wReq = new TuMiPosRequest()
                                {
                                    jsondata = new List<TuMiPosMaster>()
                                };
                                wReq.jsondata.Add(Maste);
                                var postResult = PostData<TuMiPosResponse>(item.store, secrectPara, url, wReq, "3");
                                if (postResult.code == "0")
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
                        Script = "sp_SGIS_TuMi_GetPosSale" + string.Join(";", ids),
                        Remark = "TuMI数据提报Repost"
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
        /// TuMi销售库存数据推送
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public HandleArgs NormalTuMiPost(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs Posresult = new HandleArgs();
            Posresult = NormalPosPost(solutionId, startTime, endTime);

            HandleArgs Invresult = new HandleArgs();
            Invresult = NormalInvPost(solutionId, startTime, endTime);

            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;
            if (Posresult.Status == HandleStatus.Fail || Invresult.Status == HandleStatus.Fail)
            {
                result.Status = HandleStatus.Fail;
            }
            result.Message = Posresult.Message + Invresult.Message;
            return result;
        }
         



        public HandleArgs NormalPosPost(int solutionId, DateTime startTime, DateTime endTime)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatTuMiPosSaleManager gatlwManager = new GatTuMiPosSaleManager();
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
                var secrectPara = configs[1];
                var dataList = GetPostData(solutionId, startTime, endTime);
                string aicode = "";
                string returnAicode = "";
                bool IsOk = true;
                bool IsUpdate = false;
                
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {
                        aicode = "";
                        returnAicode = "";
                        IsUpdate = false;
                        IsOk = true;
                        var sgData = gatlwManager.GetFirst(s => s.SolutionId == solutionId && s.BillNo == item.BillNo);
                        aicode = GetAicode(item.store);
                        if (!string.IsNullOrEmpty(item.SourceBillNo))
                        {
                            returnAicode = GetSourceAicode(solutionId, item.CompanyID, item.SourceBillNo);
                        }
                        if (sgData == null)
                        {
                            sgData = new GatTuMiPosSale()
                            {
                                SolutionId = solutionId,
                                CompanyID = item.CompanyID,
                                BillNo = item.BillNo,
                                CreateDtm = DateTime.Now,
                                aicode = aicode,//店内订单编号 
                                rq = item.rq,//交易日  期格式: 2024 - 4 - 21 9:34:54
                                deliveryTime = item.deliveryTime,//发货时间  期格式: 2024 - 4 - 21 9:34:54
                                memberCode = item.memberCode,//会员编号
                                saletype = item.saletype,// 销售类型，0正常, 20退货
                                store = item.store,//销售门店, 品牌提供的门店编号
                                returnAicode = returnAicode,//原始订单, 对应主订单的aicode
                                channel = item.channel,//固定值: MA
                                nianLingZu = item.nianLingZu,// 顾客的年龄组（传code 1.2.3...）,默认值: 0
                                gouWuMuDi = item.gouWuMuDi,//顾客的购物目的, 默认值:0
                                couponid = item.couponid,//优惠券id
                                uid = item.uid,//营业员ID 固定值"0"
                                uName = item.uName,//营业员姓名 固定值“”空字符串
                                CardID = item.CardID,
                                ShopID = item.ShopID,
                                ShopCode = item.ShopCode,
                                BillNo2 = item.BillNo2,
                                SourceBillNo = item.SourceBillNo,
                            };
                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        
                        if (!string.IsNullOrEmpty(item.SourceBillNo) && string.IsNullOrEmpty(returnAicode))
                        {
                            //sgData.IsSuccess = false;
                            //sgData.PostJaon = "未调用接口";
                            //sgData.ReturnJson = $"退货单中的来源单号【" + item.SourceBillNo + "】未推送";
                            //IsOk = false;
                            returnAicode = item.SourceBillNo;
                        }
                        if (IsOk)
                        {
                            try
                            {
                                int totalQty = 0;
                                decimal totalAmount = 0;
                                TuMiPosMaster Maste = new TuMiPosMaster()
                                {
                                    aicode = aicode,//店内订单编号 
                                    rq = item.rq,//交易日  期格式: 2024 - 4 - 21 9:34:54
                                    deliveryTime = item.deliveryTime,//发货时间  期格式: 2024 - 4 - 21 9:34:54
                                    memberCode = item.memberCode,//会员编号
                                    saletype = item.saletype,// 销售类型，0正常, 20退货
                                    store = item.store,//销售门店, 品牌提供的门店编号
                                    returnAicode = returnAicode,//原始订单, 对应主订单的aicode
                                    channel = item.channel,//固定值: MA
                                    nianLingZu = item.nianLingZu,// 顾客的年龄组（传code 1.2.3...）,默认值: 0
                                    gouWuMuDi = item.gouWuMuDi,//顾客的购物目的, 默认值:0
                                    couponid = item.couponid,//优惠券id
                                    uid = item.uid,//营业员ID 固定值"0"
                                    uName = item.uName,//营业员姓名 固定值“”空字符串
                                    uCode = item.couponid,//品牌营业员工号固定值"0"
                                    orderdetail = new List<TuMiPosMaster.TuMiPosDetail>(),
                                    paymentdetail = new List<TuMiPosMaster.TuMiPosReceiptDetai>()
                                };
                                var details = dataList.Item2.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var itemDetail in details)
                                {
                                    Maste.orderdetail.Add(new TuMiPosMaster.TuMiPosDetail()
                                    {
                                        SKU = itemDetail.SKU,//SKU
                                        brand = itemDetail.brand,//商品的品牌,固定值: Tumi
                                        product_unit_price = itemDetail.product_unit_price,//商品的标准零售价，单价
                                        freermb = itemDetail.freermb,//折扣金额
                                        product_count = itemDetail.product_count,//销售数量【qty】
                                        amount = itemDetail.amount,//明细实收金额【折扣后金额】
                                        order_detail_price = itemDetail.order_detail_price,//明细总额【原总价】
                                        is_gift = itemDetail.is_gift,//是否是赠品,固定值0
                                        promotion_remark = itemDetail.promotion_remark,//促销活动说明
                                    });
                                }


                                var ReceiptDetais = dataList.Item3.FindAll(s => s.BillNo == item.BillNo);
                                foreach (var ReceiptDetai in ReceiptDetais)
                                {
                                    Maste.paymentdetail.Add(new TuMiPosMaster.TuMiPosReceiptDetai()
                                    {
                                        payment_way = ReceiptDetai.payment_way,//支付方式名称
                                        payment_amount = ReceiptDetai.payment_amount,// 支付金额

                                    });
                                }
                                TuMiPosRequest wReq = new TuMiPosRequest()
                                {
                                    jsondata = new List<TuMiPosMaster>()
                                };
                                wReq.jsondata.Add(Maste);
                                var postResult = PostData<TuMiInvResponse>(item.store, secrectPara, url, wReq, "3");
                                if (postResult.code == "0")
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
                        Script = $"sp_SGIS_TuMi_GetPosSale{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "TuMI数据提报NormalPost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "Pos 提报成功";
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
                GatTuMiInvManager gatlwManager = new GatTuMiInvManager();
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
                var secrectPara = configs[1];
                var dataList = GetInvData(solutionId, startTime, endTime);

                bool IsOk = true;
                if (dataList.Item1.Count > 0)
                {
                    foreach (var item in dataList.Item1)
                    {

                        GatTuMiMeterialinv sgData = new GatTuMiMeterialinv()
                        {
                            SolutionId = solutionId,
                            CompanyID = item.CompanyID,
                            store = item.store,//品牌提供的门店编号
                            rq = item.rq,//
                            remark = item.remark,//
                            CardID = item.CardID,
                            ShopID = item.ShopID,
                            ShopCode = item.ShopCode,
                        };
                        if (IsOk)
                        {
                            try
                            {
                                //int totalQty = 0;
                                //decimal totalAmount = 0;
                                TuMiInvMaster Maste = new TuMiInvMaster()
                                {
                                    store = item.store,//品牌提供的门店编号
                                    rq = item.rq,// 库存结余时间
                                    remark = item.remark,// 库存结余时间
                                    Inventorydetail = new List<TuMiInvMaster.TuMiInvDetai>(),
                                };
                                var details = dataList.Item2.FindAll(s => s.store == item.store);
                                foreach (var itemDetail in details)
                                {
                                    Maste.Inventorydetail.Add(new TuMiInvMaster.TuMiInvDetai()
                                    {
                                        SKU = itemDetail.SKU,//SKU
                                        qty = itemDetail.qty,//数量
                                    });
                                }
                                TuMiInvRequest wReq = new TuMiInvRequest()
                                {
                                    jsondata = new List<TuMiInvMaster>()
                                };
                                wReq.jsondata.Add(Maste);
                                var postResult = PostData<TuMiPosResponse>(item.store, secrectPara, url, wReq, "5");
                                if (postResult.code == "0")
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
                        sgData.CreateDtm = DateTime.Now;
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
                        Script = $"sp_SGIS_TuMi_GetMaterailINV{startTime.ToString()}-{endTime.ToString()}",
                        Remark = "TuMi库存数据提报NormalInvPost"
                    };
                    logManager.Insert(log);
                    result.Status = HandleStatus.Completed;
                    result.Message = "库存提报成功";
                }
                else
                {
                    result.Status = HandleStatus.Completed;
                    result.Message = "库存重传成功:未获取到数据";
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
        private Tuple<List<TuMiPosMasterData>, List<TuMiPosDetailData>, List<TuMiPosReceiptDetailData>> GetPostData(int solutionId, string ids)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<TuMiPosMasterData, TuMiPosDetailData, TuMiPosReceiptDetailData>("sp_SGIS_TuMi_GetPosSale",
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
        private Tuple<List<TuMiPosMasterData>, List<TuMiPosDetailData>, List<TuMiPosReceiptDetailData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<TuMiPosMasterData, TuMiPosDetailData, TuMiPosReceiptDetailData>("sp_SGIS_TuMi_GetPosSale",
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
        private Tuple<List<TuMiInvMasterData>, List<TuMiInvDetailData>> GetInvData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<TuMiInvMasterData, TuMiInvDetailData>("sp_SGIS_TuMi_GetMaterailINV",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception ex)
            {
                throw new Exception("获取库存数据异常");
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
        public BaseResponse<T> PostData<T>(string username, string SecretKey, string url, object data, string type)
        {
            try
            {
                string password = GetPassword(SecretKey);
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(data, Formatting.Indented, jsetting);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                var client = new RestClient(url);
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("type", type);
                request.AddHeader("username", username);
                request.AddHeader("password", password);
                request.AddStringBody(json, ContentType.Json);

                var response = client.ExecutePostAsync<BaseResponse<T>>(request).GetAwaiter().GetResult();
                if (response.Data == null)
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
        /// 生成请求单号  店内订单编号",//编码格式:tumi 门店编号+”-9+right((000000000+id),9)
        /// </summary>
        /// <param name="SecretKey">秘钥</param>
        /// <returns></returns> 
        public string GetAicode(string shopcode)
        {
            int id = GetID();
            string Aicode = shopcode + "-9" + id.ToString().PadLeft(9, '0');
            return Aicode;
        }
        private int GetID()
        {
            int id = 0;
            try
            {
                string strID = ErpDb.Ado.SqlQuery<string>($@"DECLARE @ID INT;EXEC sp_GetNextID @COMCODE = 'HK', @OBJID = 'TuMi', @ISUPDATE = 1, @ID = @ID OUTPUT; SELECT @ID VendCustId").FirstOrDefault();
                id = int.Parse(strID);
            }
            catch (Exception ex)
            {
                throw new Exception("获取请求单号异常");
            }
            return id;
        }

        private string GetSourceAicode(int solutionId, string CompanyID, string BillNo)
        {
            string aicode = "";
            try
            {
                aicode = Db.Ado.SqlQuery<string>($@" SELECT a.aicode FROM Gat_TuMi_PosSale a WHERE a.SolutionId = {solutionId} AND  a.CompanyID='{CompanyID}' AND a.BillNo='{BillNo}' ").FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("获取退货来源单异常");
            }
            return aicode;
        }
        public string GetExistsSpecialMaterialRangeMaterialCode(string materialCode, int solutionId)
        {
            string data = "";
            var sql = $"select distinct materialcode from gat_specialmaterialrange where solutionId={solutionId} and materialcode in({materialCode})";
            var conn = new SqlConnection("");
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data += reader.GetString(0) + ",";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
            if (data.Length > 1)
            {
                data = data.Substring(0, data.Length - 1);
            }
            return data;
        }

        /// <summary>
        /// 获取动态密码
        /// </summary>
        /// <param name="SecretKey">秘钥</param>
        /// <returns></returns>
        public string GetPassword(string SecretKey)
        {
            string Password = "";
            string DateStr = DateTime.Now.ToString("yyyyMMdd");
            string input = DateStr + SecretKey;
            Password = GetMd5Str(input, 11);
            return Password;
        }
        /// <summary>
        /// 生成密码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetMd5Str(string input, int length)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为16进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                string Output = sb.ToString();
                Output = Output.ToUpper();
                if (length >= Output.Length)
                {
                    return Output;
                }
                Output = Output.Substring(Output.Length - length);
                return Output;
            }
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

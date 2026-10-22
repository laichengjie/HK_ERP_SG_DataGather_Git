using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GatNewBll.Models;
using GatNewBll.Models.Skechers;
using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using SqlSugar;
using System.Configuration;
using Gat.Utils;
using RestSharp;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Converters;
using GatNewBll.Models.Crocs;
using RestSharp.Serializers.Json;
using RestSharp.Serializers;

namespace GatNewBll.Blls
{
    public class SolutionBll : BaseBll
    {
        private IMapper mapper;
        public SolutionBll()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GatSolution, SolutionModel>();
            });

            mapper = config.CreateMapper();
        }
        public List<BasCompany> GetAllCompany()
        {
            BasCompanyManager manager = new BasCompanyManager();
            return manager.GetList();
            //var dto = mapper.Map<List<CompanyModel>>(list);
            //return list;
        }

        public List<BasVendor> GetAllVendor()
        {
            BasVendorManager manager = new BasVendorManager();
            return manager.GetList(s => s.AllowUsed == true);
        }

        public int GetVendorCount()
        {
            BasVendorManager manager = new BasVendorManager();
            return manager.Count(s => s.AllowUsed == true);
        }

        public List<GatShopRightModel> GetShopRight(string companyID, int vendorID, string cardID, string shopCode = "")
        {
            var query = Db.Queryable<GatSolution>()
                   .LeftJoin<GatShopright>((o, gsr) => o.CompanyID == gsr.CompanyID && o.CardID == gsr.CardID && o.VendorID == gsr.VendorID)
                   .LeftJoin<BasShop>((o, gsr, shop) => gsr.ShopID == shop.ShopID && gsr.CompanyID == shop.CompanyID)
                   .LeftJoin<BasStock>((o, gsr, shop, stock) => gsr.CompanyID == stock.CompanyID && shop.StockID == stock.StockID)
                   .LeftJoin<BasShopmap>((o, gsr, shop, stock, bsm) => gsr.CompanyID == bsm.CompanyID && gsr.ShopID == bsm.ShopID && gsr.VendorID == bsm.VendorID)
                   .LeftJoin<BasVendshop>((o, gsr, shop, stock, bsm, bvs) => bsm.VendShopID == bvs.Id && gsr.VendorID == bvs.VendorID)
                   .Where(o => o.CompanyID == companyID && o.VendorID == vendorID && o.CardID == cardID);
            if (!string.IsNullOrEmpty(shopCode))
            {
                query = query.Where((o, gsr, shop, stock, bsm, bvs) => shop.ShopCode.Contains(shopCode));
            }
           
            var result = query.Select((o, gsr, shop, stock, bsm, bvs) => new GatShopRightModel
            {
                Id = gsr.Id.SelectAll(),
                ShopCode = shop.ShopCode,
                ShopName = shop.ShopName,
                StockCode = stock.StockCode,
                StockName = stock.StockName,
                VendorShopID = bvs.Id,
                VendorShopName = bvs.ShopName
            });

            var result2 = result.Distinct().ToList();

            if (result2 != null && result2.Count > 0)
            {
                var shoprightIds = result2.Select(s => s.Id).Distinct();

                var mapList = Db.Queryable<GatShoprightDS>()
                               .LeftJoin<BasShop>((o, shop) => o.CompanyID == shop.CompanyID && o.DSShopID == shop.ShopID)
                               .Where(o => o.CompanyID == companyID && shoprightIds.Contains(o.ShopRightID)).Select((o, shop) => new
                               {
                                   ShopRightID = o.ShopRightID,
                                   ShopID = shop.ShopID,
                                   ShopCode = shop.ShopCode,
                                   ShopName = shop.ShopName,
                               }).ToList();
                var shopIDDic = mapList.GroupBy(g => g.ShopRightID).ToDictionary(d => d.Key, d => string.Join(",", d.Select(s => s.ShopID)));
                var shopNameDic = mapList.GroupBy(g => g.ShopRightID).ToDictionary(d => d.Key, d => string.Join(",", d.Select(s => s.ShopName)));
                foreach(var item in result2)
                {
                    if(shopIDDic.ContainsKey(item.Id))
                    {
                        item.OMSShopId = shopIDDic[item.Id];
                    }
                    if (shopNameDic.ContainsKey(item.Id))
                    {
                        item.OMSShopName = shopNameDic[item.Id];
                    }
                }
            }
            return result2;
        }

        /// <summary>
        /// 获取提报方案-提报店铺
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatShopRightModel> GetShopRightBySolutionId(int solutionId)
        {
            var result = Db.Queryable<GatSolution>()
                .LeftJoin<GatShopright>((o, gsr) => o.CompanyID == gsr.CompanyID && o.CardID == gsr.CardID && o.VendorID == gsr.VendorID)
                .LeftJoin<BasShop>((o, gsr, shop) => gsr.ShopID == shop.ShopID && gsr.CompanyID == shop.CompanyID)
                .LeftJoin<BasStock>((o, gsr, shop, stock) => gsr.CompanyID == stock.CompanyID && shop.StockID == stock.StockID)
                .LeftJoin<BasShopmap>((o, gsr, shop, stock, bsm) => gsr.CompanyID == bsm.CompanyID && gsr.ShopID == bsm.ShopID && gsr.VendorID == bsm.VendorID)
                .LeftJoin<BasVendshop>((o, gsr, shop, stock, bsm, bvs) => bsm.VendShopID == bvs.Id && gsr.VendorID == bvs.VendorID)
                .Where(o => o.Id == solutionId)
                .Select((o, gsr, shop, stock, bsm, bvs) => new GatShopRightModel
                {
                    Id = gsr.Id.SelectAll(),
                    ShopCode = shop.ShopCode,
                    ShopName = shop.ShopName,
                    StockCode = stock.StockCode,
                    StockName = stock.StockName,
                    VendorShopID = bvs.Id,
                    VendorShopName = bvs.ShopName
                });
            return result.ToList();
        }

        public List<GatStockRightModel> GetStockRight(string companyID, int vendorID, string cardID, string stockCode = "")
        {
            var query = Db.Queryable<GatSolution>()
                .LeftJoin<GatStockright>((o, gsr) => o.CompanyID == gsr.CompanyID && o.CardID == gsr.CardID && o.VendorID == gsr.VendorID)
                .LeftJoin<BasStock>((o, gsr, stock) => gsr.CompanyID == stock.CompanyID && gsr.StockID == stock.StockID)
                .LeftJoin<BasStockmap>((o, gsr, stock, bsm) => gsr.CompanyID == bsm.CompanyID && gsr.StockID == bsm.StockID && gsr.VendorID == bsm.VendorID)
                .LeftJoin<BasVendshop>((o, gsr, stock, bsm, bvs) => bsm.VendStockID == bvs.Id && gsr.VendorID == bvs.VendorID)
                .Where(o => o.CompanyID == companyID && o.VendorID == vendorID && o.CardID == cardID);

            if (!string.IsNullOrEmpty(stockCode))
            {
                query = query.Where((o, gsr, stock, bsm, bvs) => stock.StockCode.Contains(stockCode));
            }
            var result = query.Select((o, gsr, stock, bsm, bvs) => new GatStockRightModel
            {
                Id = gsr.Id.SelectAll(),
                StockCode = stock.StockCode,
                StockName = stock.StockName,
                VendorStockID = bvs.Id,
                VendorStockName = bvs.StockName
            });
            return result.ToList();
        }

        public List<GatStockRightModel> GetStockRightBySolutionId(int solutionId)
        {
            var result = Db.Queryable<GatSolution>()
                .LeftJoin<GatStockright>((o, gsr) => o.CompanyID == gsr.CompanyID && o.CardID == gsr.CardID && o.VendorID == gsr.VendorID)
                .LeftJoin<BasStock>((o, gsr, stock) => gsr.CompanyID == stock.CompanyID && stock.StockID == stock.StockID)
                .LeftJoin<BasStockmap>((o, gsr, stock, bsm) => gsr.CompanyID == bsm.CompanyID && gsr.StockID == bsm.StockID && gsr.VendorID == bsm.VendorID)
                .LeftJoin<BasVendshop>((o, gsr, stock, bsm, bvs) => bsm.VendStockID == bvs.Id && gsr.VendorID == bvs.VendorID)
                .Where(o => o.Id == solutionId)
                .Select((o, gsr, stock, bsm, bvs) => new GatStockRightModel
                {
                    Id = gsr.Id.SelectAll(),
                    StockCode = stock.StockCode,
                    StockName = stock.StockName,
                    VendorStockID = bvs.Id,
                    VendorStockName = bvs.StockName
                });
            return result.ToList();
        }

        public SolutionModel GetSolutionById(int id)
        {
            var result = Db.Queryable<GatSolution>()
                .LeftJoin<BasCard>((o, card) => o.CardID == card.CardID)
                .LeftJoin<BasCompany>((o, card, company) => o.CompanyID == company.CompanyID)
                .LeftJoin<BasVendor>((o, card, company, vendor) => o.VendorID == vendor.Id)
                .Where(o => o.Id == id)
                .Select((o, card, company, vendor) => new SolutionModel
                {
                    Id = o.Id.SelectAll(),
                    CardName = card.CardName,
                    CompanyName = company.CompanyName,
                    VendorName = vendor.VendorName
                }).First();
            return result;
        }

        public bool InsertOrUpdateSolution(GatSolution data, out int id, params string[] ignoreColumns)
        {
            GatSolutionManager manager = new GatSolutionManager();
            if (data.Id == 0)
            {
                id = manager.InsertReturnIdentity(data);
                if (id > 0) return true;
                else return false;
            }
            else
            {
                var result = manager.UpdateWithIgnoreColumn(data, ignoreColumns);
                id = data.Id;
                return result;
            }

        }

        /// <summary>
        /// 更新店铺权限，只更新部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">需要更新的字段名</param>
        /// <returns></returns>
        public bool UpdateShopRight(List<GatShopright> data, params string[] columns)
        {
            GatShoprightManager manager = new GatShoprightManager();
            return manager.BatchUpdateWithColumn(data, columns);
        }

        /// <summary>
        /// 更新仓库权限，只更新部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">需要更新的字段名</param>
        /// <returns></returns>
        public bool UpdateStockRight(List<GatStockright> data, params string[] columns)
        {
            GatStockrightManager manager = new GatStockrightManager();
            return manager.BatchUpdateWithColumn(data, columns);
        }

        /// <summary>
        /// 通过供应商Id获取提报方案
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public List<SolutionBaseData> GetSolutionByVendorId(int vendorId)
        {
            var result = Db.Queryable<GatSolution>()
                .LeftJoin<BasCard>((o, card) => o.CardID == card.CardID)
                .LeftJoin<BasCompany>((o, card, company) => o.CompanyID == company.CompanyID)
                .Where(o => o.VendorID == vendorId && o.Enabled == true)
                .Select((o, card, company) => new SolutionBaseData
                {
                    Id = o.Id.SelectAll(),
                    CardName = card.CardName,
                    CompanyName = company.CompanyName
                }).ToList();
            return result;
        }

        /// <summary>
        /// 通过供应商Id获取第一个提报方案
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public GatSolution GetFirstSolutionByVendorId(int vendorId)
        {
            GatSolutionManager manager = new GatSolutionManager();
            return manager.GetFirst(s => s.VendorID == vendorId && s.Enabled == true);
        }

        /// <summary>
        /// 获取后台提报任务
        /// </summary>
        /// <returns></returns>
        public List<SolutionModel> GetOnlyBackRunSolution()
        {
            GatSolutionManager manager = new GatSolutionManager();
            var solutionList = manager.GetList(s => s.Enabled == true && s.IsBackground == true);
            return mapper.Map<List<SolutionModel>>(solutionList);
        }

        #region 斯凯奇提报请求
        public HandleArgs SkechersPost(int solutionId, DateTime startTime, DateTime endTime, bool isBackgroundRun = true)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                var directory = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(directory))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报路径"
                    };
                }
                var dirs = directory.Split(';');
                var requestUrl = dirs[0];
                var clientId = dirs[1];
                var password = dirs[2];

                var appKey = solution.AppKey;
                if (string.IsNullOrEmpty(solution.AppKey) || DateTime.Now > Convert.ToDateTime(solution.ExpiryDate))
                {
                    var appKeyResp = GetAppKey(requestUrl, clientId, password);
                    solution.AppKey = appKeyResp.Data;
                    solution.ExpiryDate = appKeyResp.ExpiryDate;
                    manager.Update(solution);
                }

                SysLogManager logManager = new SysLogManager();
                if (isBackgroundRun)
                {
                    //获取syslog最后一次提报时间作为开始时间
                }
                //获取提报数据Gat_SkechersPosData
                var dataList = GetPostData(solutionId, startTime, endTime);
                GatSkechersPosSaleManager spsManager = new GatSkechersPosSaleManager();
                //发送数据请求
                foreach (var item in dataList.Item1)
                {
                    var sps = spsManager.GetById(item.refno);
                    var isNew = false;
                    if (sps == null)
                    {
                        sps = new GatSkechersPossale()
                        {
                            SolutionId = solutionId,
                            BillDate = item.billDate,
                            BillNo = item.refno,
                            ShopCode = item.ShopId,
                            CreateDtm = DateTime.Now
                        };
                        isNew = true;
                    }
                    var hasDetail = false;
                    try
                    {
                        var detailList = dataList.Item2.Where(s => s.refno == item.refno).ToList();
                        if (detailList.Count() > 0)
                        {
                            hasDetail = true;
                            item.sumQty = detailList.Sum(s => s.qty);
                            item.billDateInt = Convert.ToInt32(item.billDate.ToString("yyyyMMdd"));
                            sps.TotalQty = item.sumQty;
                            item.sumTotAmtActual = detailList.Sum(s => s.totAmtActual);
                            sps.TotalAmount = item.sumTotAmtActual;
                            item.detail = detailList;
                            var postResult = PostService(requestUrl, clientId, appKey, item, out string reqJson);
                            if (postResult.Code != "0")
                            {
                                sps.IsSuccess = false;
                            }
                            else
                            {
                                sps.IsSuccess = true;
                            }
                            sps.PostJaon = reqJson;//JsonConvert.SerializeObject(item);
                            sps.ReturnJson = JsonConvert.SerializeObject(postResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        sps.ReturnJson = "发送请求异常：" + ex.Message;
                    }
                    if (hasDetail)
                    {
                        sps.ReturnJson=TruncateString(sps.ReturnJson,2000);
                        sps.ModifyDtm = DateTime.Now;
                        if (isNew)
                        {
                            spsManager.Insert(sps);
                        }
                        else
                        {
                            spsManager.Update(sps);
                        }
                    }
                }

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
                    Remark = "斯凯奇" + (isBackgroundRun ? "自动" : "补采") + "提报"
                };
                logManager.Insert(log);

                result.Status = HandleStatus.Completed;
                result.Message = "补采成功";
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
        /// 字符串截取
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string TruncateString(string input, int length = 1000)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length);
            }
        }

        private Tuple<List<SkechersMasterData>, List<SkechersDetailData>> GetPostData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {

                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<SkechersMasterData, SkechersDetailData>("sp_SGIS_SkechersPosData",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception)
            {
                throw new Exception("获取销售数据异常");
            }

        }
        private BaseResponse PostService(string url, string clientId, string appKey, object data, out string reqJson)
        {
            //https://eplatformtest.skechers.cn:9443/eplatform/ 测试环境
            //https://eplatform.skechers.cn:8443/eplatform/ 正式环境

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //生成sign clientid + appkey + timestamp 进行MD5加密
            var signStr = clientId + appKey + timestamp;
            var sign = GetMD5(signStr);

            //发送门店销售单（零售单）接口请求
            var itemDetailClient = new RestClient(url + "/erp/addItem/addRetail");
            RestRequest itemDetailReq = new RestRequest();
            AddItemAddDetailRequest request = new AddItemAddDetailRequest()
            {
                ClientId = clientId,
                Version = "v1",
                Timestamp = timestamp,
                Sign = sign,
                Params = data
            };
            var json = JsonConvert.SerializeObject(request);
            reqJson = json;
            itemDetailReq.AddHeader("Content-Type", "application/json");
            itemDetailReq.AddStringBody(json, ContentType.Json);
            //itemDetailReq.AddJsonBody(request);

            var itemDetailResp = itemDetailClient.ExecutePostAsync<BaseResponse>(itemDetailReq).GetAwaiter().GetResult();
            if (itemDetailResp.Data == null)
            {
                throw new Exception("请求零售单接口失败");
            }
            return itemDetailResp.Data;
        }

        private GetAppKeyResponse GetAppKey(string url, string clientId, string password)
        {
            //获取appkey 每个帐号每天访问getAppkey接口次数为30次
            //https://eplatformtest.skechers.cn:9443/eplatform/getAppkey
            var keyClient = new RestClient(url + "/getAppkey");
            GetAppKeyRequest keyReqParam = new GetAppKeyRequest()
            {
                ClientId = clientId,
                Password = password
            };
            RestRequest keyReq = new RestRequest();
            keyReq.AddJsonBody(keyReqParam);
            keyReq.AddHeader("Content-Type", "application/json");

            var keyResp = keyClient.ExecutePostAsync<GetAppKeyResponse>(keyReq).GetAwaiter().GetResult();
            if (keyResp.Data == null || keyResp.Data.Code != "0")
            {
                throw new Exception("获取appKey失败" + keyResp.Data?.Message);
            }
            return keyResp.Data;
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        public static string MD5Encrypt64(string str)
        {
            string cl = str;
            //string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }

        public static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);//
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                //这个是很常见的错误，你字节转换成字符串的时候要保证是2位宽度啊，某个字节为0转换成字符串的时候必须是00的，否则就会丢失位数啊。不仅是0，1～9也一样。
                //byte2String += targetData[i].ToString("x");//这个会丢失
                byte2String = byte2String + targetData[i].ToString("x2");
            }

            return byte2String;
        }
        #endregion

        #region Crocs提报请求
        public HandleArgs CrocsPost(int solutionId, DateTime startTime, DateTime endTime, bool isBackgroundRun = true)
        {
            HandleArgs result = new HandleArgs();
            try
            {

                GatSolutionManager manager = new GatSolutionManager();
                var solution = manager.GetById(solutionId);
                var directory = solution.Directory;
                //解析配置获取接口地址
                if (string.IsNullOrEmpty(directory))
                {
                    return new HandleArgs()
                    {
                        Status = HandleStatus.Fail,
                        Message = "基本方案未配置提报路径"
                    };
                }
                var dirs = directory.Split(';');
                var baseUrl = dirs[0];
                var secret = dirs[1];


                SysLogManager logManager = new SysLogManager();
                if (isBackgroundRun)
                {
                    //获取syslog最后一次提报时间作为开始时间
                }
                var tokenClient = new RestClient(baseUrl + "?t=token");
                RestRequest tokenRequest = new RestRequest();
                tokenRequest.Method = Method.Post;
                tokenRequest.AddParameter("secret", secret);
                //var tokenResponse1=tokenClient.ExecuteAsync<CrocsApiResponse>(tokenRequest).GetAwaiter().GetResult();
                //var tokenResponse = tokenClient.ExecuteAsync(tokenRequest).GetAwaiter().GetResult();
                var tokenResponse = tokenClient.ExecutePostAsync<CrocsApiResponse>(tokenRequest).GetAwaiter().GetResult();
                var token = "";
                if (tokenResponse.Data != null && !string.IsNullOrEmpty(tokenResponse.Data.Data))
                {
                    token = tokenResponse.Data.Data;

                    //获取销售数据
                    var posDataList = GetCrocsPosSaleData(solutionId, startTime, endTime);

                    IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                    if (posDataList != null && posDataList.Count() >= 0)
                    {
                        timeFormat.DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
                        //发送销售数据
                        var saleJson = JsonConvert.SerializeObject(new CrocsApiRequest<PosSaleModel> { data = posDataList }, Formatting.Indented, timeFormat);
                        var saleClient = new RestClient(baseUrl + "?t=doorslsbyjson");
                        saleClient.UseSystemTextJson();
                        var saleRequest = new RestRequest();
                        saleRequest.Method = Method.Post;
                        saleRequest.AddHeader("Authorization", token);
                        saleRequest.AddHeader("Content-Type", "application/json");
                        saleRequest.AddStringBody(saleJson, ContentType.Json);
                        var saleResponse = saleClient.ExecutePostAsync<CrocsApiResponse>(saleRequest).GetAwaiter().GetResult();
                        if (saleResponse.Data != null && !string.IsNullOrEmpty(saleResponse.Data.Data))
                        {
                            SysLog posLog = new SysLog()
                            {
                                CompanyID = solution.CompanyID,
                                CardID = solution.CardID,
                                IsSucceed = true,
                                LogType = Convert.ToByte(LogType.Gat),
                                SolutionID = solution.Id,
                                VendorID = solution.VendorID,
                                ModifyDTM = DateTime.Now,
                                Remark = "Crocs" + (isBackgroundRun ? "自动" : "补采") + "提报" + saleResponse.Data.Data
                            };
                            logManager.Insert(posLog);
                        }
                        else
                        {
                            throw new Exception("发送销售数据失败" + saleResponse.Content);
                        }
                    }

                    //获取库存数据
                    var invDataList = GetCrocsInvData(solutionId);

                    //发送库存数据
                    if (invDataList != null && invDataList.Count() >= 0)
                    {
                        var invJson = JsonConvert.SerializeObject(new CrocsApiRequest<InvStockModel> { data = invDataList }, Formatting.Indented, timeFormat);
                        var options = new RestClientOptions(baseUrl + "?t=doorinvbyjson")
                        {
                            ThrowOnAnyError = true,
                            Timeout = 600 * 1000
                        };
                        var invClient = new RestClient(options);
                        invClient.UseJson();
                        var invRequest = new RestRequest();
                        invRequest.Method = Method.Post;
                        invRequest.AddHeader("Authorization", token);
                        invRequest.AddHeader("Content-Type", "application/json");
                        invRequest.AddStringBody(invJson, ContentType.Json);
                        var invResponse = invClient.ExecutePostAsync<CrocsApiResponse>(invRequest).GetAwaiter().GetResult();
                        if (invResponse.Data != null && !string.IsNullOrEmpty(invResponse.Data.Data))
                        {
                            SysLog invLog = new SysLog()
                            {
                                CompanyID = solution.CompanyID,
                                CardID = solution.CardID,
                                IsSucceed = true,
                                LogType = Convert.ToByte(LogType.Gat),
                                SolutionID = solution.Id,
                                VendorID = solution.VendorID,
                                ModifyDTM = DateTime.Now,
                                Remark = "Crocs" + (isBackgroundRun ? "自动" : "补采") + "提报" + invResponse.Data.Data
                            };
                            logManager.Insert(invLog);
                        }
                        else
                        {
                            throw new Exception("发送库存数据失败" + invResponse.Content);
                        }
                    }
                }
                else
                {
                    throw new Exception("获取token失败" + tokenResponse.Content);
                }

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
                    Remark = "Crocs" + (isBackgroundRun ? "自动" : "补采") + "提报"
                };
                logManager.Insert(log);

                result.Status = HandleStatus.Completed;
                result.Message = "补采成功";
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

        private List<PosSaleModel> GetCrocsPosSaleData(int solutionId, DateTime startTime, DateTime endTime)
        {
            try
            {

                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<PosSaleModel>("sp_SGIS_CrocsPosData",
                    new { SolutionId = solutionId, StartTime = startTime, EndTime = endTime });
            }
            catch (Exception)
            {
                throw new Exception("获取销售数据异常");
            }

        }

        private List<InvStockModel> GetCrocsInvData(int solutionId)
        {
            try
            {

                return ErpDb.Ado.UseStoredProcedure()
                    .SqlQuery<InvStockModel>("sp_SGIS_CrocsInvData",
                    new { SolutionId = solutionId });
            }
            catch (Exception)
            {
                throw new Exception("获取销售数据异常");
            }

        }
        #endregion

        public void InsertSysLog(SysLog log)
        {
            SysLogManager manager = new SysLogManager();

            manager.Insert(log);
        }
    }
}

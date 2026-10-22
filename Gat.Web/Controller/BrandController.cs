using Gat.BLL;
using Gat.BLL.ControllerBll;
using Gat.BLL.ControllerModel.Request;
using Gat.BLL.ControllerModel.Response;
using Gat.Utils;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Gat.Web.Controller
{
    public class BrandController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetSales(string token, string content)
        {
            Logger logger = LogManager.GetLogger("GetSales");
            logger.Info($"GetSales请求参数：{content}");
            string resultJson = string.Empty;
            BaseResponse<List<GetSales>> response = null;
            try
            {
                if (!Common.ControlHelper.CheckToken(token))
                {
                    response = new BaseResponse<List<GetSales>>()
                    {
                        Status = 1,
                        ErrorMsg = "token验证失败"
                    };
                    return BuildResponse(response);
                }

                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"\bin\ConnectConfig.DAT");
                GetSalesRequest request = JsonConvert.DeserializeObject<GetSalesRequest>(content);
                var companyId = ConfigurationManager.AppSettings["BrandQueryCompanyID"];
                var vendorId = Convert.ToInt32(ConfigurationManager.AppSettings["BrandVendorID"]);
                BrandBll bll = new BrandBll();
                //string connString = XmlHelper.WebConfigConnection("ERP", filePath);
                string connString = ConfigurationManager.ConnectionStrings["EXTRACTORDB"].ToString();
                List<GetSales> data = bll.GetSales(connString, request.StartBillTime, request.EndBillTime, vendorId, companyId);

                logger.Info($"GetSales返回数据行数：{data.Count()}");
                response = new BaseResponse<List<GetSales>>()
                {
                    Status = 0,
                    Data = data
                };
                return BuildResponse(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetSales异常");
                response = new BaseResponse<List<GetSales>>()
                {
                    Status = 1,
                    ErrorMsg = "异常：" + ex.Message + ex.StackTrace
                };
                return BuildResponse(response);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetStocks(string token, string content)
        {
            Logger logger = LogManager.GetLogger("GetStocks");
            logger.Info($"GetStocks请求参数：{content}");
            string resultJson = string.Empty;
            BaseResponse<List<GetStocks>> response = null;
            try
            {
                if (!Common.ControlHelper.CheckToken(token))
                {
                    response = new BaseResponse<List<GetStocks>>()
                    {
                        Status = 1,
                        ErrorMsg = "token验证失败"
                    };
                    return BuildResponse(response);
                }
                if (string.IsNullOrEmpty(content))
                {
                    response = new BaseResponse<List<GetStocks>>()
                    {
                        Status = 1,
                        ErrorMsg = "查询条件不可为空"
                    };
                    return BuildResponse(response);
                }

                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"\bin\ConnectConfig.DAT");
                GetStocksRequest request = JsonConvert.DeserializeObject<GetStocksRequest>(content);
                var companyId = ConfigurationManager.AppSettings["BrandQueryCompanyID"];
                var vendorId = Convert.ToInt32(ConfigurationManager.AppSettings["BrandVendorID"]);
                BrandBll bll = new BrandBll();
                //string connString = XmlHelper.WebConfigConnection("ERP", filePath);
                string connString = ConfigurationManager.ConnectionStrings["EXTRACTORDB"].ToString();
                List<GetStocks> data = bll.GetStocks(connString, request.Stcode, request.Oldcode, vendorId, companyId);
                
                logger.Info($"GetStocks返回数据行数：{data.Count()}");
                response = new BaseResponse<List<GetStocks>>()
                {
                    Status = 0,
                    Data = data
                };
                return BuildResponse(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetStocks异常");
                response = new BaseResponse<List<GetStocks>>()
                {
                    Status = 1,
                    ErrorMsg = "异常：" + ex.Message
                };
                return BuildResponse(response);
            }
        }

        private HttpResponseMessage BuildResponse(object content)
        {
            var resultJson = JsonConvert.SerializeObject(content);
            return new HttpResponseMessage()
            {
                Content = new StringContent(resultJson, Encoding.UTF8, "application/json"),
            };
        }

        private string GetConnectionString()
        {
            return XmlHelper.WebConfigConnection("ERP");
        }

        private GatRepository GetRepository()
        {
            GatRepository repository = new GatRepository(ConnnectType.ERP);
            return repository;
        }
    }
}
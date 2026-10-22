using Bxh.Bj; // ← 服务引用 BosService_Zx 生成的代理命名空间
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace GatNewBll.Blls
{
    /// <summary>
    /// 伯俊 BOS WebService 调用工具。
    /// 签名规则：Key 升序 + 末尾追加 MD5(AppSecret) + 整体 MD5（大写）。
    /// </summary>
    public class BurgeonWebServices
    {


        private readonly string _endpoint;
        private readonly string _appkey;
        private readonly string _sourceSys;//接口调用方系统英文名称
        private readonly string _appsecret;
        private readonly string _apptoken;

        public BurgeonWebServices(string endpoint, string appkey, string sourceSys, string appsecret, string apptoken)
        {

            _endpoint = endpoint;
            _appkey = appkey;
            _sourceSys = sourceSys;
            _appsecret = appsecret;
            _apptoken = apptoken;
        }

        private BosService_ZxSoapClient CreateClient()
        {
            // 对应 WSDL 的 soap:binding transport="http://schemas.xmlsoap.org/soap/http"
            // 用 BasicHttpBinding 即可，SOAP 1.1
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                SendTimeout = TimeSpan.FromSeconds(60),
                ReceiveTimeout = TimeSpan.FromSeconds(60),
                OpenTimeout = TimeSpan.FromSeconds(30),
                CloseTimeout = TimeSpan.FromSeconds(30),
            };
            var address = new EndpointAddress(_endpoint);
            return new BosService_ZxSoapClient(binding, address);
        }

        /// <summary>
        ///  调用业务数据接口
        /// </summary>
        /// <param name="appInterface">接口编号</param>
        /// <param name="content">son格式数据</param>
        /// <returns></returns>
        public string Request(string appInterface, string content)
        {
            string result = null;
            try
            {
                using (var zx = CreateClient())
                {
                    var api = new ApiParameter
                    {
                        // 来自 HeadForm（父类）
                        SOURCE_SYS = _sourceSys,
                        SENDDATE = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        // 来自 ApiParameter 本身
                        APPKEY = _appkey,
                        APPSECRET = _appsecret,
                        APPTOKEN = _apptoken,
                        APPINTERFACE = appInterface,
                        METHOD = "CREATE|SUBMIT",
                        CONTENT = content
                    };
                    api.SIGN = CreateSign(api);
                    result = zx.ReceivingInterface(new ReceivingInterface { api = api })?.ReceivingInterfaceResult;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// 生成 SIGN。参数 Key 升序 → key=value&amp;... → 末尾追加 MD5(AppSecret) → 整体 MD5。
        /// 只签 8 个字段（APPINTERFACE / APPKEY / APPSECRET / APPTOKEN / CONTENT / METHOD / SENDDATE / SOURCE_SYS）。
        /// </summary>
        private string CreateSign(ApiParameter api)
        {
            // SortedDictionary 默认按 Key 升序（Ordinal 保证 ASCII 顺序）
            var signMap = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "APPINTERFACE", api.APPINTERFACE },
                { "APPKEY",       api.APPKEY },
                { "APPSECRET",    api.APPSECRET },
                { "APPTOKEN",     api.APPTOKEN },
                { "CONTENT",      api.CONTENT },
                { "METHOD",       api.METHOD },
                { "SENDDATE",     api.SENDDATE },
                { "SOURCE_SYS",   api.SOURCE_SYS }, // ← 在 HeadForm 里，子类实例可直接访问
            };

            var sb = new StringBuilder();
            foreach (var kv in signMap)
                sb.Append(kv.Key).Append('=').Append(kv.Value ?? string.Empty).Append('&');
            if (sb.Length > 0) sb.Length--; // 去掉末尾 '&'

            sb.Append(MD5(api.APPSECRET)); // 末尾追加 AppSecret 的 MD5
            return MD5(sb.ToString());      // 整体再 MD5
        }

        private static string MD5(string sourceStr)
        {
            using (var md = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md.ComputeHash(Encoding.UTF8.GetBytes(sourceStr ?? string.Empty));

                var sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
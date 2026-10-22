using Microsoft.VisualStudio.TestTools.UnitTesting;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatNewBll.Models.LiningNop;
using Newtonsoft.Json;

namespace GatNewBll.Blls.Tests
{
    [TestClass()]
    public class LiningNopBllTests
    {
        [TestMethod()]
        public void PostDataTest()
        {
            LiningNopBll bll = new LiningNopBll();
            string appId = "";
            string url = "";
            string secrect = "";
            AuthTokenRequest requst = new AuthTokenRequest(false);
            //var data = JsonConvert.SerializeObject(requst);
            var response = bll.PostData<AuthTokenResponse>(appId, secrect, url, "", requst.GetMethod());
        }
    }
}
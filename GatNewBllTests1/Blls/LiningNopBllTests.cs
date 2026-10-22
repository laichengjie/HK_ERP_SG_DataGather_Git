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
            AuthTokenRequest reqT = new AuthTokenRequest(false);
            //string url = "https://nopqas.lining.com/gateway";
            string url = "https://nop.lining.com/gateway";
            //string appid = "f928ff9031eb508c8ba64a23";
            string appid = "f854ee4776779979bcee4a3a";
            //string secrect = "5130fc69fe36d315cd6cab178ba941b9";
            string secrect = "c4c76e0a211545bcbe08817e90652b90";
            var resT = bll.PostData<AuthTokenResponse>(appid, secrect, url, reqT, reqT.GetMethod());
            if (!string.IsNullOrEmpty(resT.data.access_token))
            {
                WholesaleSalesListRequest reqW = new WholesaleSalesListRequest();
                reqW.companyCode = "A61034M";
                reqW.receivingCode = "I10T";
                reqW.externalOrderno = "CX221124JX036-001";
                reqW.orderDate = "20221124";
                reqW.items = new List<WholesaleSalesListRequest.WholesaleSalesListItem>();
                reqW.items.Add(new WholesaleSalesListRequest.WholesaleSalesListItem()
                {
                    disc = "0.510000",
                    //prodCode= "AMYS467-1",
                    qty= "2.000000",
                    //sizeCode= "000",
                    sku= "AMYS447-3000",
                    unitPrice= "50.490000"
                });
                var resW = bll.PostData<WholesaleSalesListResponse>(appid, secrect, url, reqW, reqW.GetMethod(), resT.data.access_token);
            }
        }
    }
}
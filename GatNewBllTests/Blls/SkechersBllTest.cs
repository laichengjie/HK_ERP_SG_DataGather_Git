using Microsoft.VisualStudio.TestTools.UnitTesting;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBllTests.Blls
{
    [TestClass]
    public class SkechersBllTest
    {
        [TestMethod]
        public void GetPageListTest()
        {
            SkechersBll bll = new SkechersBll();
            int totalCount = 0;
            int totalQty = 0;
            decimal totalAmount = 0;
            var data = bll.GetPageList(1, 1, 10, null, null, "", ref totalCount, ref totalQty, ref totalAmount);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Blls.Tests
{
    [TestClass()]
    public class SolutionBllTests
    {
        [TestMethod()]
        public void GetAllCompanyTest()
        {
            SolutionBll bll = new SolutionBll();
            var companyList = bll.GetAllCompany();
        }

        [TestMethod()]
        public void GetAllVendorTest()
        {
            SolutionBll bll = new SolutionBll();
            var data = bll.GetAllVendor();
            Console.Write(data.Count());
        }


        [TestMethod()]
        public void GetVendorCountTest()
        {
            SolutionBll bll = new SolutionBll();
            var data = bll.GetVendorCount();
            Console.Write(data);
        }

        [TestMethod()]
        public void GetSolutionByIdTest()
        {
            SolutionBll bll = new SolutionBll();
            var data = bll.GetSolutionById(16);
        }
    }
}
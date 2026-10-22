using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Linq.Expressions;

namespace CodeGenerate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //var e = ExpHelper.True<Company>();
            //e = e.AndAlso(c => c.CompanyCode == "sdds");
            //var service = new ServiceBase();

            //service.BeginTran();
            //service.DeleteEntity(e);
            //service.UpdateEntity(e, c => new Company { CompanyID = "KK", CompanyCode = "gg" });
            //Company c0 = new Company();
            //c0.CompanyCode = "事务测试公司";
            //service.InsertEntity(c0, false);

            //HandleArgs HANDLE = service.SaveChanges();

            //Expression<Func<Company,Company>> action = c => new Company { };
            //var xx = service.QueryEntity(e);
            ////service.TestUpdate<Company>(c =>  c.CompanyID; );
            //Action<Company> exp = (c) => 
            //{
            //    c.CompanyCode = "1";
            //    c.CompanyID = "2";
            //};


            //try
            //{
            //    service.BeginTran();
            //    service.DeleteEntity(e);
            //    Company c = new Company();
            //    c.CompanyCode = "事务测试公司";
            //    service.InsertEntity(c,false);
                
            //    HandleArgs HANDLE = service.SaveChanges();
            //    if (HANDLE.Status == HandleStatus.Fail) throw new Exception(HANDLE.Message);
            //    service.CommitTran();

            //}
            //catch (Exception)
            //{
            //    service.RollbackTran();
            //    //throw;
            //}

            Application.Run(new Form1());
        }
    }
}

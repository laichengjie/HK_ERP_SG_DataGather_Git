using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GatNewBll.Common
{
    public static class Helper
    {
        /// <summary> 
        /// 获取当前运行的程序集所在路径 
        /// </summary>
        public static string ExecutePath
        {
            get
            {
                string assembly = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                string assemblyFile = assembly.Substring(8);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(assemblyFile);

                return fileInfo.Directory.FullName;
            }
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.TuMi
{
    public class BaseResponse<T>
    {
        public string request_id { get; set; }

        public string code { get; set; }
        public string msg { get; set; }
        public string sub_code { get; set; }
        public string sub_msg { get; set; }
        public T data { get; set; }
    }
}

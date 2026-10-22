using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.On
{
    public class BaseResponse<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public string timestamp { get; set; }

        public string path { get; set; }
    }
}

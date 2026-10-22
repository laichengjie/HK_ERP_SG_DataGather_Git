using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.ControllerModel.Response
{
    public class BaseResponse<T>
    {
        public int Status { get; set; }

        public string ErrorMsg { get; set; }

        public T Data { get; set; }
    }
}

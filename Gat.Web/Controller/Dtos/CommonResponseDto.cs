using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos
{
    public class CommonResponseDto<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }


        public T Data { get; set; }
    }
}
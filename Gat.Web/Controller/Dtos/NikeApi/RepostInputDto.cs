using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.NikeApi
{  
    public class RepostInputDto
    {
        public int SolutionId { get; set; }

        /// <summary>
        /// 接口类型
        /// </summary>
        public string InterfaceType { get; set; }

        public List<int> Ids { get; set; }
    }
}
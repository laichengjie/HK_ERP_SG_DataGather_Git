using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller.Dtos.TuMi
{ 
    public class RepostInputDto
    {
        public int SolutionId { get; set; }

        public List<int> Ids { get; set; }
    }
}
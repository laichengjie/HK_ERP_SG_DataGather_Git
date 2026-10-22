using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace Gat.Web.Controller.Dtos
{
    public class BaseQueryPageDto
    {
        public int? SolutionId { get; set; }

        public int? VendorId { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
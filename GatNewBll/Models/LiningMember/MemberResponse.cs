using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    public class MemberResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public memberInfo[] memberJson { get; set; }
    }

    public class AddMemberResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public memberInfo memberJson { get; set; }
    }

    public class memberInfo
    {
        public string customerlevel { get; set; }
        public string customername { get; set; }
        public string gender { get; set; }
        public string customermobile { get; set; }
        public string customersource { get; set; }
        public string customeraddress { get; set; }
        public string customerbirthday { get; set; }
        public string memberid { get; set; }

    }
}

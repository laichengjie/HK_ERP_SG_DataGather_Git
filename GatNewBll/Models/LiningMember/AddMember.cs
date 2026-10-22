using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningMember
{
    public class AddMember
    {
        public string secret { get; set; }
        public string signdate { get; set; }
        public AddMemberInfo memberJson { get; set; }
    }

    public class AddMemberInfo
    {

        public string fullName { get; set; }
        public string mobileNum { get; set; }
        public long birthday { get; set; }
        public string address { get; set; }
        public string gender { get; set; }
        public string storeCode { get; set; }
        public string brand { get; set; }
    }
}

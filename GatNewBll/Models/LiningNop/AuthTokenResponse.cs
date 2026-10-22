using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningNop
{
    public class AuthTokenResponse
    {
        public string access_token { get; set; }
        public string client_id { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public long expires_in { get; set; }
        public long re_expires_in { get; set; }
        public string refresh_token { get; set; }
        public string user_id { get; set; }
    }
}

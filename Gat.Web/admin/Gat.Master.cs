using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
using Gat.BLL.Modules;
using Newtonsoft.Json;
using Gat.Web.Common;
using System.Runtime.Remoting.Contexts;

namespace Gat.Web.admin
{
    public partial class Gat : System.Web.UI.MasterPage
    {
        /// <summary>
        /// 
        /// </summary>
        private string userCode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string _UserCode
        {
            get { return userCode; }
        }


        /// <summary>
        /// 旧密码
        /// </summary>
        public string PasswordByPast
        {
            get { return this.ltlByPast.Text; }
            set { this.ltlByPast.Text = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = HttpContext.Current.User as GatPrincipal;
            if (currentUser != null)
            {
                ltlByPast.Text = currentUser.Password;
            }

            string mode = Request["mode"] ?? "";
            userCode = currentUser.UserCode.ToLower();
        }

        void btnReport_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Server.Transfer("~/report/default.aspx");
        }
    }
}
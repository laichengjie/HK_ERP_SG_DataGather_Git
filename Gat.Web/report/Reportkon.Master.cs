using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Gat.BLL;
using Gat.Utils;
using Gat.BLL.Modules;
using Gat.Web.Common;
using System.Text;

namespace Gat.Web.report
{
    public partial class Reportkon : System.Web.UI.MasterPage
    {
        /// <summary>
        /// 导航
        /// </summary>
        protected string Navigate
        {
            get
            {
                return new CommonConfig(this).GetNavigate();
            }
        }

        protected string SecondaryNavigate
        {
            get
            {
                return new CommonConfig(this).GetSecondaryNavigate();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
                ConnnectType connType = context.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest;
                HandleArgs HANDLE = XmlHelper.CheckConnectConfig();
                if (HANDLE.Status == HandleStatus.Fail)
                {
                    Response.RedirectToRoute("login");
                }
                using (var gat = new GatRepository(connType))
                {
                    // 绑定供应商
                    ddlVendor.DataSource = gat.QueryAllowUsedVendors();//CacheHelper.FetchCache<List<VendorB>>("Vendor");
                    ddlVendor.DataTextField = "VendorName";
                    ddlVendor.DataValueField = "ID";
                    ddlVendor.DataBind();

                    ListItem li = ddlVendor.Items.FindByValue(context.VendorID.ToString());
                    if (li != null) li.Selected = true;
                }
            }
        }

        #region 辅助方法 


        #endregion
    }
}
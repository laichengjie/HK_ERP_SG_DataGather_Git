using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.On
{
    public partial class rpt_inv : Core.ReportBase
    { 
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            this.TopTitle = "On昂跑报表";
            this.SubTitle = "库存数据";
            base.OnLoad(e);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nike
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string shopCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "耐克报表";
                this.SubTitle = "库存数据";

                shopCode = Request["shopcode"];
                if (!string.IsNullOrEmpty(shopCode)) shopCode = HttpUtility.UrlDecode(shopCode);
                this.txtShopCode.Text = shopCode;

                this.BindMat(shopCode);
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string matID = null)
        {
            this.GridView = gvInv;
            var mats = WebRepository.QueryGatMatInvs(this.CurrentPage, this.ItemsPerPage, matID);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatMatInvCount(matID);
        }
    }
    
}
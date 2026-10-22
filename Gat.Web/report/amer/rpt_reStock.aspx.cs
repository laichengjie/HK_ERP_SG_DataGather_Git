using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.amer
{
    public partial class rpt_reStock : Core.ReportBase
    {//页面传递的店铺代码参数
        protected string providerCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "萨洛蒙报表";//亚玛芬
                this.SubTitle = "进货数据";

                providerCode = Request["providerCode"];
                if (!string.IsNullOrEmpty(providerCode)) providerCode = HttpUtility.UrlDecode(providerCode);
                this.txtShopCode.Text = providerCode;

                this.BindMat(providerCode);
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode = null)
        {
            this.GridView = gvPos;
            var mats = WebRepository.QueryGatAmerReStockInfo(this.CurrentPage, this.ItemsPerPage, shopCode);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatAmerReStockInfoCount(shopCode);
        }
    }
}
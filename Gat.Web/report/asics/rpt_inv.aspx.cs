using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.asics
{
    public partial class rpt_inv : Core.ReportBase
    {
        //页面传递的店铺代码参数
        protected string productCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.TopTitle = "亚瑟士报表";//亚玛芬
                this.SubTitle = "库存数据";

                productCode = Request["productCode"];
                if (!string.IsNullOrEmpty(productCode)) productCode = HttpUtility.UrlDecode(productCode);
                this.txtShopCode.Text = productCode;

                this.BindMat(productCode);
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string shopCode = null)
        {
            this.GridView = gvPos;
            var mats = WebRepository.QueryGatAsicsINV(this.CurrentPage, this.ItemsPerPage, shopCode);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatAsicsINVCount(shopCode);

            
        }
    }
}
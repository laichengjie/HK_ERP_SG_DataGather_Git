using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
using Gat.Utils;

namespace Gat.Web.admin
{
    /// <summary>
    /// 不论供应商是否处于运行状态，供应商都从正式库取值，包括其经营品牌
    /// </summary>
    public partial class gat_vendor_bowse : Core.PageMultiBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            if (!IsPostBack)
            {
                TopTitle = "供应商资料";
                SubTitle = "浏览";

                BindVendors();
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindVendors(txtVendorCode.Text);
        }


        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindVendors(string vendCode = null)
        {
            this.GridView = gvVendor;
            //this.DataSource = WebRepository.QueryVendorsByCode(vendCode);
            var vendors = WebRepository.QueryVendorsByCode(vendCode);
            this.DataSource = vendors;
            gvVendor.CssClass = vendors.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
            //this.RecordsCount = WebRepository.VendorCount();
        }
    }
}
/*
 * 货品提报：
 * 接收参数：page,matcode
 */


using Gat.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gat.Web.report.nike
{
    public partial class rpt_material : Core.ReportBase
    {
        //页面传递的货号参数
        protected string matCode = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DGContext context = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);
            if (!IsPostBack)
            {
                this.TopTitle = "耐克报表";
                this.SubTitle = "产品数据";

                matCode = Request["matcode"];
                if (!string.IsNullOrEmpty(matCode)) matCode = HttpUtility.UrlDecode(matCode);
                this.txtMatCode.Text = matCode;

                this.BindMat(matCode);
                //this.gvMat.Width = new Unit(gvMat.Columns.Count * 120);
            }
        }


        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindMat(string matCode = null)
        {
            this.GridView = gvMat;
            var mats = WebRepository.QueryGatMaterials(this.CurrentPage, this.ItemsPerPage, matCode);
            this.DataSource = mats;
            this.RecordsCount = WebRepository.GatMaterialsCount(matCode);
        }
    }
}
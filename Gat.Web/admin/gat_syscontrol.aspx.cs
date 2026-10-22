using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gat.BLL;
using Gat.Utils;
using Gat.BLL.Modules;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Text;

namespace Gat.Web.admin
{
    public partial class gat_syscontrol : Core.PageMultiBase
    {
        protected override void OnLoad(EventArgs e)
        {
            var currentUser = HttpContext.Current.User as GatPrincipal;
            if (currentUser != null && currentUser.UserCode.ToLower() != "hkshop")
            {
                this.btnSave.Visible = false;
                return;
            }
            base.OnLoad(e);

            if (this.IsAJAXRequest)
            {
                string mode = Request["mode"] ?? "";
                string mode2 = Request["mode2"] ?? "";
                string id = Request["id"] ?? "";
                switch (mode)
                {
                    case "DeleteSolution":
                        //this.DeleteSolution(Request["id"]);
                        break;
                }

                return;
            }
            //this.btnSearch.Click += new EventHandler(btnSearch_Click);
            // this.btnSave.Click += new EventHandler(btnSave_Click);

            if (!IsPostBack)
            {
                this.TopTitle = "店铺管控设置";
                //this.SubTitle = "浏览方案";
                this.BindSolutions();
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindSolutions();
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindSolutions(string title = null)
        {
            this.GridView = this.gvSolution;
            //var solutions = WebRepository.QuerySolutions(title);
            var solutions = WebRepository.QueryVendors();
            this.DataSource = solutions;
            gvSolution.CssClass = solutions.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
            //this.RecordsCount = WebRepository.VendorCount();
        }

        /// <summary>
        /// 呈现控件，输出 HTML
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private string RenderControl(Control control)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page page = new Page();
            HtmlForm form = new HtmlForm();
            this.gvSolution.EnableViewState = false;
            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            form.Controls.Add(this.gvSolution);
            page.RenderControl(htw);
            return sw.ToString();
        }

        /// <summary>
        /// 更新供应商店铺数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var ai = new AJAXInterop();
            foreach (GridViewRow row in gvSolution.Rows)
            {
                try
                {
                    TextBox txtShopNumber = (TextBox)row.FindControl("txtShopNumber");
                    //Label labVendorCode = (Label)row.FindControl("labVendorCode");
                    //Label labVendorName = (Label)row.FindControl("labVendorName");
                    Label labID = (Label)row.FindControl("labID");
                    string StrId = labID.Text.Trim();
                    string StrShopNumber = txtShopNumber.Text.Trim();
                    HandleArgs handle = WebRepository.UpdateShopNumber(StrId, StrShopNumber);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            this.Alert("保存成功", Core.DialogIcon.Correct);
            //重新加载店铺信息
            this.BindSolutions();
        }


    }
}
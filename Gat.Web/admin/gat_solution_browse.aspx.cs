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
    public partial class gat_solution_browse : Core.PageMultiBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.IsAJAXRequest)
            {
                string mode = Request["mode"] ?? "";
                switch (mode)
                {
                    case "DeleteSolution":
                        this.DeleteSolution(Request["SolutionID"]);
                        break;
                    case "UpdateSolutionEnabled":
                        this.UpdateSolutionEnabled(Request["SolutionID"]);
                        break;
                }

                return;
            }
            this.btnSearch.Click += new EventHandler(btnSearch_Click);

            if (!IsPostBack)
            {
                this.TopTitle = "提报方案";
                this.SubTitle = "浏览方案";
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
            this.BindSolutions(txtSolutionCode.Text);
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        protected void BindSolutions(string title = null)
        {
            this.GridView = this.gvSolution;
            //this.DataSource = WebRepository.QueryVendorsByCode(vendCode);
            var solutions = WebRepository.QuerySolutions(title);
            this.DataSource = solutions;
            gvSolution.CssClass = solutions.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
            //this.RecordsCount = WebRepository.VendorCount();
        }

        /// <summary>
        /// 删除方案ID
        /// </summary>
        /// <param name="SolutionID"></param>
        private void DeleteSolution(string SolutionID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(SolutionID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【ID】";
                this.ResponseToClient(ai);
            }
            int varSolutionID = Convert.ToInt32(SolutionID);
            //保存到数据库
            HandleArgs handle = WebRepository.DeleteSolution(new SolutionB { ID = varSolutionID });
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }
            //重新加载店铺信息
            this.BindSolutions(txtSolutionCode.Text);
            string html = this.RenderControl(this.gvSolution);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);
        }
        private void UpdateSolutionEnabled(string SolutionID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(SolutionID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【ID】";
                this.ResponseToClient(ai);
            }
            int varSolutionID = Convert.ToInt32(SolutionID);
            SolutionB Solution = WebRepository.FetchSolution(varSolutionID);
            VendorB vendorB = WebRepository.FetchVendor((int)Solution.VendorID);
            if (vendorB != null && Solution.Enabled==false)
            {
                int ShopNumberCount = WebRepository.SolutionShopNumberCount((int)Solution.VendorID, "", varSolutionID);//已经设置店铺数量
                if (ShopNumberCount > vendorB.ShopNumber) 
                {
                    //this.Alert("已超过店铺数上限，不可保存，请联系客服！", Core.DialogIcon.Warning);
                    ai.StatusCode = "2";
                    ai.Message = "已超过店铺数上限，不可保存，请联系客服！";
                    ai.Html = "已超过店铺数上限，不可保存，请联系客服！";
                    this.ResponseToClient(ai);
                    return;
                }
            }

            //保存到数据库
            HandleArgs handle = WebRepository.UpdateSolutionEnabled(new SolutionB { ID = varSolutionID });
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }
            //重新加载店铺信息
            this.BindSolutions();
            string html = this.RenderControl(this.gvSolution);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);
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

    }
}
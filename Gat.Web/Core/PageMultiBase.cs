using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Gat.BLL.Modules;
namespace Gat.Web.Core
{
    /// <summary>
    /// 列表页面，主要用于浏览页
    /// </summary>
    public class PageMultiBase : PageBase
    {
        #region 公开属性

        /// <summary>
        /// 当前页码是否有效
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsPagerValid()
        {
            string page = this["page"];
            bool valid = true;
            if (string.IsNullOrEmpty(page)) return !valid;

            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(page);
        }

        /// <summary>
        /// 每页面显示的条数
        /// </summary>
        protected int ItemsPerPage { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        protected int PageCount
        {
            get { /*this.ItemsPerPage = 4;*/ return this.RecordsCount % this.ItemsPerPage == 0 ? this.RecordsCount / this.ItemsPerPage : (RecordsCount / this.ItemsPerPage + 1); }
        }

        /// <summary>
        /// 记录数
        /// </summary>
        protected int RecordsCount { get; set; }

        /// <summary>
        /// 当前页面
        /// </summary>
        protected int CurrentPage { get; private set; }

        /// <summary>
        /// 单数据源
        /// </summary>
        protected object DataSource
        {
            get
            {
                return GridView != null ? GridView.DataSource as List<LogicBase> : null;
            }
            set
            {
                if (GridView != null)
                {
                    this.GridView.DataSource = value;
                    this.GridView.DataBind();
                    this.GridView.CssClass = this.GridView.Rows.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
                    this.GridView.AlternatingRowStyle.CssClass = "alternate";
                }
            }
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        protected System.Web.UI.WebControls.GridView GridView { get; set; }

        #endregion 

        #region 重写方法

        protected override void OnInit(EventArgs e)
        {
            this.ItemsPerPage = 25;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            bool valid = this.IsPagerValid();
            this.CurrentPage = valid ? Convert.ToInt32(this["page"]) : 1;
        }

        #endregion

        #region 受保护方法

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Gat.Web.Core;
using Gat.Web.report;
using System.Text.RegularExpressions;

namespace Gat.Web.Core
{
    /// <summary>
    /// 报表基页面 
    /// </summary>
    public class ReportBase : Core.PageMultiBase
    {
        #region 公开属性

        private string _pageParam = "page";
        /// <summary>
        /// 分页参数
        /// </summary>
        protected string PageParam
        {
            get { return _pageParam; }
            set { _pageParam = value; }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public new int CurrentPage { get;  set; }

        #endregion

        #region 重写方法 
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            bool valid = this.IsPagerValid();
            this.CurrentPage = valid ? Convert.ToInt32(this[PageParam]) : 1;
        }

        protected override bool IsPagerValid()
        {
            //return base.IsPagerValid();

            string page = this[PageParam];
            bool valid = true;
            if (string.IsNullOrEmpty(page)) return !valid;

            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(page);
        }

        #endregion

        #region 受保护方法

        #endregion
    }
}
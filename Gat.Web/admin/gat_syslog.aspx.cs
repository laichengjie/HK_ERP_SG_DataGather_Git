

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
//using J.JQueryHelper;//UncleKing
using System.Text.RegularExpressions;

namespace Gat.Web.admin
{
    /*
     *  接收参数：page
     * 
     */

    public partial class gat_syslog : Core.PageMultiBase
    {
        protected new int ItemsPerPage
        {
            get { return 18; }
        }

        /// <summary>
        /// 页数
        /// </summary>
        protected new int PageCount
        {
            get { return this.RecordsCount % this.ItemsPerPage == 0 ? this.RecordsCount / this.ItemsPerPage : (RecordsCount / this.ItemsPerPage + 1); }
        }

        //页面传值
        protected string shopName, vendorCode, cardName, start, end, strartTimeString, endTimeString;
        string datePattern = @"^((((19|20)(([02468][048])|([13579][26]))-02-29))|((20[0-9][0-9])|(19[0-9][0-9]))-((((0[1-9])|(1[0-2]))-((0[1-9])|(1\d)|(2[0-8])))|((((0[13578])|(1[02]))-31)|(((01,3-9])|(1[0-2]))-(29|30)))))$";
        protected DateTime startTime = DateTime.Now.AddDays(-5).Date, endTime = DateTime.Now.Date;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //btnSearch.Click += new EventHandler(btnSearch_Click);

            if (!IsPostBack)
            {
                vendorCode = Request["vendor"]; //供应商代码
                shopName = Request["shop"]; //店铺名称
                cardName = Request["card"]; //品牌名称
                start = Request["start"]; //开始时间
                end = Request["end"];   //结束时间

                //初始化值
                if (!string.IsNullOrEmpty(vendorCode)) vendorCode = HttpUtility.UrlDecode(vendorCode);
                if (!string.IsNullOrEmpty(shopName)) vendorCode = HttpUtility.UrlDecode(shopName);
                if (!string.IsNullOrEmpty(cardName)) vendorCode = HttpUtility.UrlDecode(cardName);
                Regex regex = new Regex(datePattern);
                if (regex.IsMatch(start ?? "")) startTime = DateTime.Parse(start);
                if (regex.IsMatch(end ?? "")) endTime = DateTime.Parse(end);
                strartTimeString = startTime.ToString("yyyy-MM-dd");
                endTimeString = endTime.ToString("yyyy-MM-dd") + " 23:59:59";
                endTime = DateTime.Parse(endTimeString);

                //初始化UI
                txtVCSearch.Text = vendorCode;
                txtShopName.Text = shopName;
                txtCardName.Text = cardName;
                txtStartTime.Text = startTime.ToString("yyyy-MM-dd");
                txtEndTime.Text = endTime.ToString("yyyy-MM-dd");

                this.TopTitle = "日志管理";
                this.BindSyslogs(CurrentPage, startTime, endTime, shopName, cardName, vendorCode);
            }
        }

        /// <summary>
        /// 绑定提报方案
        /// </summary>
        void BindSyslogs(int currentPage, DateTime start, DateTime end, string shopName = "", string cardName = "", string vendorCode = "")
        {
            //int currentPage, int itemsPerPage, DateTime start, DateTime end, string shopName = "", string cardName = "", string vendorCode = ""
            this.GridView = gvLog;
            this.RecordsCount = WebRepository.LogsCount(start, end, shopName, cardName, vendorCode);
            this.DataSource = WebRepository.QueryLogs(currentPage, this.ItemsPerPage, start, end, shopName, cardName, vendorCode);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gat.BLL;
using Gat.BLL.Modules;
using Gat.Utils;
using System.Text.RegularExpressions;
using System.IO;

namespace Gat.Web.admin
{
    public partial class _default : Core.PageBase
    {
        /// <summary>
        /// 日志列表是否显示
        /// </summary>
        protected string LogDisplay
        {
            get
            {
                int itemCount = 0;
                foreach (RepeaterItem item in rptErrorLog.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        itemCount += 1;
                }
                return itemCount > 0 ? string.Empty : "hidden";
            }
        }

        /// <summary>
        /// 供应商列表是否显示
        /// </summary>
        protected string VendorDisplay
        {
            get
            {
                int itemCount = 0;
                foreach (RepeaterItem item in rptVendor.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        itemCount += 1;
                }
                return itemCount > 0 ? string.Empty : "hidden";
            }
        }

        /// <summary>
        /// 供应商列表是否显示
        /// </summary>
        protected string SolutionDisplay
        {
            get
            {
                int itemCount = 0;
                foreach (RepeaterItem item in rptSolution.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        itemCount += 1;
                }
                return itemCount > 0 ? string.Empty : "hidden";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region AJAX 补采请求

            if (this.IsAJAXRequest)
            {
                string mode = Request["mode"];
                switch (mode)
                {
                    case "gat":
                        this.ManualGat(Request["vendor"], Request["company"], Request["card"], Request["start"], Request["end"], Request["log"]);
                        break;
                }
            }

            #endregion

            if (!IsPostBack)
            {
                this.TopTitle = "首页";

                this.BindErrorLog();
                this.BindTopVendors();
                this.BindTopSolutions();
            }
        }

        /// <summary>
        /// 绑定错误日志
        /// </summary>
        protected void BindErrorLog()
        {
            List<SysLogB> errorLogs = WebRepository.QueryGatErrorLog();
            var dataSource =
                errorLogs
                .Select((log, index) => new
                {
                    Index = index + 1,
                    Html = log.ModifyDTM.ToString("yyyy-MM-dd") + "&nbsp;&nbsp日【" + log.CompanyName + "】" + "为【" + (string.IsNullOrEmpty(log.VendorName) ? log.VendorCode : log.VendorName) + "】的【" + log.CardName + "】品牌提报失败。&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<i style='padding:2px 8px;' class='ui-icon ui-icon-newwin'>&nbsp;</i> " +
                    "<span class='hidden end'>" + log.ModifyDTM + "</span><span class='hidden start'>" + log.ModifyDTM.AddDays(-1) + "</span><a href='javascript:;' onclick=\"javascript:manualGat(this," + log.ID + ",'" + log.CompanyID + "','" + log.VendorID + "','" + log.CardID + "');\" >补采</a>",
                });

            rptErrorLog.DataSource = dataSource;
            rptErrorLog.DataBind();
        }

        /// <summary>
        /// 绑定最新供应商，8条记录
        /// </summary>
        protected void BindTopVendors()
        {
            List<VendorB> vendors = WebRepository.QueryTopNewVendors();
            var dataSource =
                vendors
                .Select((v, index) => new
                {
                    Index = index + 1,
                    Html = "<a href=" + this.GetRouteUrl("vendor-edit", new { id = v.ID }) + ">" + (string.IsNullOrEmpty(v.VendorName) ? v.VendorCode : v.VendorName) + "</a>" + "（采集品牌：" + (string.IsNullOrEmpty(v.CardNames) ? "<span style='color:#757575'>无</span>" : v.CardNames) + "）"
                });

            rptVendor.DataSource = dataSource;
            rptVendor.DataBind();
        }


        /// <summary>
        /// 绑定最新解决方案，8条记录
        /// </summary>
        protected void BindTopSolutions()
        {
            List<SolutionB> vendors =
                WebRepository
                .QuerySolutions()
                .OrderBy(s => s.Title)
                .Skip(0)
                .Take(8)
                .ToList(); //方案应该不会很多，
            var dataSource =
                vendors
                .Select((s, index) => new
                {
                    Index = index + 1,
                    Html = "<a href=" + this.GetRouteUrl("solution-edit", new { id = s.ID }) + ">" + (s.Title) + "</a>"// + "（提报公司：" + (string.IsNullOrEmpty(s.CompanyNames) ? "<span style='color:#757575'>无</span>" : s.CompanyNames) + "）"
                });

            rptSolution.DataSource = dataSource;
            rptSolution.DataBind();
        }

        /// <summary>
        /// 手动采集数据
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        protected void ManualGat(string vendorID, string companyID, string cardID, string start, string end, string log)
        {
            AJAXInterop ai = new AJAXInterop();
            string datePattern = @"^((((((19|20)\d{2})-(0?(1|[3-9])|1[012])-(0?[1-9]|[12]\d|30))|(((19|20)\d{2})-(0?[13578]|1[02])-31)|(((19|20)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|((((19|20)([13579][26]|[2468][048]|0[48]))|(2000))-0?2-29)))\s((([0-1][0-9])|(2[0-3])):[0-5][0-9]:[0-5][0-9]))$";
            Regex dateRex = new Regex(datePattern);
            if (!dateRex.IsMatch(start) || !dateRex.IsMatch(end))
            {
                ai.StatusCode = "2";
                ai.Html = "时间格式不正确！";
                this.ResponseToClient(ai);
            }

            string numPattern = @"^\d+$";
            Regex numRex = new Regex(numPattern);
            if (!numRex.IsMatch(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "供应商参数错误！";
                this.ResponseToClient(ai);

            }

            if (string.IsNullOrEmpty(cardID) ||
                string.IsNullOrEmpty(companyID) ||
                cardID == "0" ||
                companyID == "0" ||
                vendorID == "0")
            {

                ai.StatusCode = "2";
                ai.Html = "参数错误：【CARD|COMPANY|VENDOR】！";
                this.ResponseToClient(ai);
            }

            //if (string.IsNullOrEmpty(procName))
            //{
            //    ai.StatusCode = "2";
            //    ai.Html = "PROC参数错误！";
            //    this.ResponseToClient(context, ai);
            //}

            //DateTime endParam = DateTime.Parse(end); //传递过来的结束时间值

            //如果传递过来的结束值大于等于当前日期，则取当前时间点
            //DateTime endTime = endParam.Date >= DateTime.Now.Date ? DateTime.Now : DateTime.Parse(end + " 23:59:59");
            DateTime endTime = DateTime.Parse(end);
            if (endTime > DateTime.Now) endTime = DateTime.Now;

            //取供应商的ID
            var webRepository = new GatRepository(DGContext.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest);
            VendorB vendor = webRepository.FetchVendor(Convert.ToInt32(vendorID));
            if (vendor == null)
            {
                ai.StatusCode = "2";
                ai.Html = "查找不到指定的供应商资料！";
                this.ResponseToClient(ai);
            }

            var gat = new GatRepository(DGContext.Environment == RunEnvironment.Due ? ConnnectType.ERP : ConnnectType.ERPTest);
            HandleArgs HANDLE = gat.ExecBatch(vendor.VendorCode, 0, companyID, cardID, DateTime.Parse(start), endTime);

            if (HANDLE.Status == HandleStatus.Completed)
            {
                //如果补采成功，并且传输Log，则更新该日志记录
                if (!string.IsNullOrEmpty(log))
                {
                    int logID = Convert.ToInt32(log);
                    SysLogB sysLog = webRepository.FetchSysLog(logID);
                    if (sysLog != null)
                    {
                        sysLog.LogType = LogType.Gat;
                        sysLog.IsSucceed = true;
                        sysLog.ModifyDTM = endTime;
                        webRepository.UpdateSysLog(sysLog);
                        this.BindErrorLog();
                    }
                }
            }


            ai.StatusCode = HANDLE.Status == HandleStatus.Completed ? "1" : "2";
            ai.Message = HANDLE.Status == HandleStatus.Completed ? "补采成功！" : HANDLE.Message;
            ai.Html = this.RenderControl(rptErrorLog);

            //HandleArgs gatResult = g

            //HandleArgs gatResult = GatSGLogic.ExecBatch("", ConfigHelper.CardID, "", "0", DateTime.Parse(start), endTime);
            //ajax.Message = gatResult.Message;
            //ajax.StatusCode = "2";
            //if (gatResult.Status == HandleStatus.Completed) ajax.StatusCode = "1";

            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 呈现控件，输出 HTML
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private string RenderControl(Control control)
        {
            StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            control.RenderControl(htmlWriter);
            return stringWriter.ToString();
        }
    }
}
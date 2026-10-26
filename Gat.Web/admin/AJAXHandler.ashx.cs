using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Gat.BLL;
using Gat.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web.Security;
using Gat.Web.Common;
using Gat.BLL.Modules;
using GatNewBll.Blls;
using System.Net.NetworkInformation;

namespace Gat.Web.admin
{
    /// <summary>
    /// AjaxHandler 的摘要说明
    /// </summary>
    public class AJAXHandler : IHttpHandler
    {
        DGContext dgContext = null; //存储用户信息的上下文

        #region 获取请求 
        public void ProcessRequest(HttpContext context)
        {
            string mode = context.Request["mode"];
            var ai = new AJAXInterop();
            dgContext = CookieHelper.GetCookie<DGContext>(CookieHelper.ContextCookieName);


            if (dgContext == null)
            {
                ai.StatusCode = "2";
                ai.Html = "您的连接信息丢失，请重登录！";
                this.ResponseToClient(context, ai);
            }

            switch (mode)
            {
                case "password":
                    this.PassworModify(context, context.Request["bypast"], context.Request["new"]);
                    break;

                case "sync":
                    this.DataSynchron(context);
                    break;
                case "gat":
                    this.ManualGat(context, context.Request["vendor"], context.Request["company"], context.Request["card"], context.Request["start"], context.Request["end"], context.Request["log"], context.Request["solutionId"]);
                    break;
            }
        }
        #endregion

        #region AJAX 请求

        #region 更新密码
        /// <summary>
        /// 更新密码 
        /// </summary>
        /// <param name="old"></param>
        /// <param name="_new"></param>
        protected void PassworModify(HttpContext context, string old, string _new)
        {
            var ai = new AJAXInterop();

            if (string.IsNullOrEmpty(_new))
            {
                ai.Html = "密码不能为空！";
                ai.StatusCode = "2";
                this.ResponseToClient(context, ai);
            }

            GatPrincipal principal = context.User as GatPrincipal;
            if (principal == null)
            {
                ai.Html = "您的登录信息丢失，请重登录！";
                ai.StatusCode = "2";
                this.ResponseToClient(context, ai);
            }

            if (old != principal.Password)
            {
                ai.StatusCode = "2";
                ai.Html = "旧密码错误！";
                this.ResponseToClient(context, ai);
            }

            //密码是否合法
            Regex regex = new Regex(@"^[0-9a-zA-Z_]+$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(_new))
            {
                ai.StatusCode = "2";
                ai.Html = "密码只能是数字、字母和&nbsp;_&nbsp;！";
                this.ResponseToClient(context, ai);
            }

            if (_new.Length > 16)
            {
                ai.StatusCode = "2";
                ai.Html = "密码的长度必须在1和16个字符之间！";
                this.ResponseToClient(context, ai);
            }

            var gat = new GatRepository(dgContext.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest);
            BLL.Modules.UserB user = new BLL.Modules.UserB();
            principal.Password = _new;
            user.CopyPropertyFrom(principal);

            HandleArgs handle = gat.UpdateUser(user);
            if (handle.Status == HandleStatus.Completed)
            {
                handle.Message = "修改密码成功！";

                //更新COOKIE
                FormsAuthentication.SignOut();
                string userData = GatPrincipal.BulidUserData(user);
                CookieHelper.SetAuthenCookie(user.UserCode, userData);


                if (context.Request.Cookies[CookieHelper.MemberCookieName] != null)
                {
                    Common.CookieHelper.SetCookie(CookieHelper.MemberCookieName, userData, 7 * 24 * 60);
                }

            }
            ai.StatusCode = handle.Status == HandleStatus.Completed ? "1" : "2";
            ai.Message = handle.Message;
            ai.Html = ai.Message;
            this.ResponseToClient(context, ai);
        }
        #endregion

        #region 数据同步
        /// <summary>
        /// 数据同步
        /// </summary>
        protected void DataSynchron(HttpContext context)
        {
            var gat = new GatRepository(dgContext.Environment == RunEnvironment.Due ? ConnnectType.ERP : ConnnectType.ERPTest);
            HandleArgs handle = gat.DataSynchron();
            var ai = new AJAXInterop();
            ai.Message = handle.Message;
            ai.Html = handle.Message;
            ai.StatusCode = handle.Status == HandleStatus.Completed ? "1" : "2";
            this.ResponseToClient(context, ai);
        }
        #endregion

        #region 手动采集数据
        /// <summary>
        /// 手动采集数据
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        protected void ManualGat(HttpContext context, string vendorID, string companyID, string cardID, string start, string end, string log, string solutionId)
        {
            AJAXInterop ai = new AJAXInterop();
            string datePattern = @"^((((((19|20)\d{2})-(0?(1|[3-9])|1[012])-(0?[1-9]|[12]\d|30))|(((19|20)\d{2})-(0?[13578]|1[02])-31)|(((19|20)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|((((19|20)([13579][26]|[2468][048]|0[48]))|(2000))-0?2-29)))\s((([0-1][0-9])|(2[0-3])):[0-5][0-9]:[0-5][0-9]))$";
            Regex dateRex = new Regex(datePattern);
            if (!dateRex.IsMatch(start) || !dateRex.IsMatch(end))
            {
                ai.StatusCode = "2";
                ai.Html = "时间格式不正确！";
                this.ResponseToClient(context, ai);
            }

            string numPattern = @"^\d+$";
            Regex numRex = new Regex(numPattern);
            if (!numRex.IsMatch(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "供应商参数错误！";
                this.ResponseToClient(context, ai);

            }

            if (string.IsNullOrEmpty(cardID) ||
                string.IsNullOrEmpty(companyID) ||
                cardID == "0" ||
                companyID == "0" ||
                vendorID == "0")
            {

                ai.StatusCode = "2";
                ai.Html = "参数错误：【CARD|COMPANY|VENDOR】！";
                this.ResponseToClient(context, ai);
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
            var webRepository = new GatRepository(dgContext.Environment == RunEnvironment.Due ? ConnnectType.Web : ConnnectType.WebTest);
            VendorB vendor = webRepository.FetchVendor(Convert.ToInt32(vendorID));
            
            if (vendor == null)
            {
                ai.StatusCode = "2";
                ai.Html = "查找不到指定的供应商资料！";
                this.ResponseToClient(context, ai);
            }
            HandleArgs HANDLE = new HandleArgs();
            int id = int.Parse(solutionId);
            SolutionBll bll = new SolutionBll();

            ReportMap Report = vendor.ReportPath;
            string VendorCode = vendor.VendorCode.ToLower();
            if (Report == ReportMap.Vans)
            {
                VendorCode = "vans_em".ToLower();
            }
            //斯凯奇提报为后台提报补采需要单独处理
            switch (VendorCode)
            {
                case "skechers":
                    HANDLE = bll.SkechersPost(id, DateTime.Parse(start), endTime, false);
                    break;
                case "crocs":
                    HANDLE = bll.CrocsPost(id, DateTime.Parse(start), endTime, false);
                    break;
                case "0001"://李宁
                    HANDLE = new AdBll_OC().GetExtractorData("sp_SGIS_ExecBatch", vendor.VendorCode, 0, companyID, cardID, start, end, 0);
                    break;
                case "lining_memeber":
                    HANDLE = new LiningMemberBll().NormalPost(id, DateTime.Parse(start), endTime);
                    break;
                case "lining_fx":
                    HANDLE = new LiningNopBll().NormalPost(id, DateTime.Parse(start), endTime);
                    break;
                case "ln_oms":
                    HANDLE = new LiningQqdBll().NormalPost(id, DateTime.Parse(start), endTime, true);
                    break;
                case "adidasomnichannel":
                    //tring VendorCode,int GatType, string CompanyID,string CardID,string StartTime,string EndTime, int IsTB = 0
                    HANDLE = new AdBll_OC().GetExtractorData("sp_SGIS_ExecBatch_OC", vendor.VendorCode, 0, companyID, cardID, start, end,0);
                    break;
                case "tumi":
                    HANDLE = new TuMiBll().NormalTuMiPost(id, DateTime.Parse(start), endTime);
                    break;
                case "on":
                    HANDLE = new OnBill().NormalOnPost(id, DateTime.Parse(start), endTime,false);
                    break;
                case "vans_em":
                    HANDLE = new VansBill().VansPush(id, DateTime.Parse(start), endTime, 1);
                    break;
                case "arcteryx":
                    HANDLE = new ArcTeryxBill().NormalArcTeryxPost(id, DateTime.Parse(start), endTime, false);
                    break;
                case "burgeon":
                    HANDLE = new BurgeonBill().NormalBurgeonPost(id, DateTime.Parse(start), endTime, false);
                    break;
                case "nikeapi":
                    HANDLE = new NikeApiBill().PushNikeApiAllData(id, DateTime.Parse(start), endTime, false);
                    break;
                case "nikeomnichannel":
                    HANDLE = new NikeBll_OC().GetExtractorData("sp_SGIS_ExecBatch_OC_Nike", vendor.VendorCode, 0, companyID, cardID, start, end, 0);
                    break;
                default:
                    var gat = new GatRepository(dgContext.Environment == RunEnvironment.Due ? ConnnectType.ERP : ConnnectType.ERPTest);
                    HANDLE = gat.ExecBatch(vendor.VendorCode, 0, companyID, cardID, DateTime.Parse(start), endTime);
                    break;
            }

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
                    }
                }
            }
            ai.StatusCode = HANDLE.Status == HandleStatus.Completed ? "1" : "2";
            ai.Html = HANDLE.Status == HandleStatus.Completed ? "补采成功！" : HANDLE.Message;
            ai.Message = HANDLE.Message;


            //HandleArgs gatResult = g

            //HandleArgs gatResult = GatSGLogic.ExecBatch("", ConfigHelper.CardID, "", "0", DateTime.Parse(start), endTime);
            //ajax.Message = gatResult.Message;
            //ajax.StatusCode = "2";
            //if (gatResult.Status == HandleStatus.Completed) ajax.StatusCode = "1";

            this.ResponseToClient(context, ai);
        }


        #endregion

        #region 提报方案



        #endregion

        #endregion

        #region 私有方法
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 向客户端返回响应信息
        /// </summary>
        protected void ResponseToClient(HttpContext context, AJAXInterop ajax)
        {
            context.Response.StatusCode = 200;
            context.Response.Write(JsonConvert.SerializeObject(ajax));
            context.Response.End();
            //context.Response.Flush();

            //context.Response.Close();
        }
        #endregion

    }
}
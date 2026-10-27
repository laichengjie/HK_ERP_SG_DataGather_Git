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
using Gat.Web.Common;
using System.IO;
using GatNewBll.Blls;
using GatNewBll.Models;

namespace Gat.Web.admin
{
    public partial class gat_vendor : Core.PageSingleBase
    {
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

            #region AJAX 选择品牌

            if (this.IsAJAXRequest)
            {
                string mode = Request["mode"] ?? "";
                switch (mode) 
                {
                    case  "cardquery":
                        this.QueryBasCard(Request["cardcode"]);
                        break;
                    case "cardsave":
                        this.UpdateVendorCard(Request["cardcode"], Request["vendor"], Request["scope"]);
                        break;
                    case "shopsave":
                        this.UpdateVendorShop(
                            Request["vendor"],
                            Request["shopcode"],
                            Request["shopname"],
                            Request["shopid"],
                            Request["stockcode"],
                            Request["stockname"]
                            );
                        break;

                    case "deleteshop":
                        this.DeleteVendorShop(Request["shop"],Request["vendor"]);
                        break;
                }

                return;
            }

            #endregion

            this.btnSave.Click += new EventHandler(btnSubmit_Click);
            this.btnDelete.Click += new EventHandler(btnDelete_Click);

            if (!Page.IsPostBack)
            {

                Title = "供应商资料";
                TopTitle = "供应商资料";
                lblCreateDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                SubTitle = this.CUID == Core.CUID.Create ? "新增" : "修改";

                //绑定基础资料中的品牌
                this.BindBasCard(null);
                //如果没有品牌资料，先提示用户更新基础资料
                if (this.gvCards.Rows.Count == 0)
                {
                    this.Alert("没有查询到品牌资料，建议您使用【基础资料>>同步资料】功能更新基础资料，然后刷新本页面！", Core.DialogIcon.Warning);
                    return;
                }

                if (this.CUID == Core.CUID.Update)
                {

                    //初始化UI
                    VendorB vendor = WebRepository.FetchVendor(IdentityKey);
                    if (vendor == null)
                    {
                        this.Alert(
                            "抱歉，您浏览的供应商不存在！",
                            Core.DialogIcon.Warning,
                            string.Format("location.href='{0}';", this.GetRouteUrl("vendor", new { }))
                            );
                    }
                    else
                    {
                        this.txtVendorCode.Text = vendor.VendorCode;
                        this.txtVendorName.Text = vendor.VendorName;
                        this.lblCreateDate.Text = vendor.CreateDate.ToString("yyyy-MM-dd");
                        ListItem li = this.ddlReport.Items.FindByValue(((byte)vendor.ReportPath).ToString());
                        if (li != null) li.Selected = true;
                        //this.txtReportPath.Text = vendor.ReportPath;
                        if (this.DGContext.User.Role == RoleType.Admin) this.txtVendorCode.Enabled = false;
                    }


                }
            }

            if (this.CUID == Core.CUID.Update)
            {
                //绑定供应商的店铺与品牌，因为界面有AJAX处理， 所以无论页面是否回发都要绑定
                this.BindVendorCard(this.IdentityKey);
                this.BindVendorShop(this.IdentityKey);
                //参数值是列表里可直接编辑的输入框，回发时不能重新绑定，否则会覆盖用户录入的值
                if (!Page.IsPostBack) this.BindVendorParameter(this.IdentityKey);
            }

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }

        #region 功能按钮

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.IdentityKey == 0) return;
            HandleArgs handle = WebRepository.DeleteVendor(new VendorB { ID = IdentityKey });
            if (handle.Status == HandleStatus.Completed) handle.Message = "供应商已成功删除！";

            this.Alert(handle.Message,
                handle.Status == HandleStatus.Completed ? Core.DialogIcon.Correct : Core.DialogIcon.Warning,
                string.Format("location.href='{0}'", this.GetRouteUrl("vendor", new {  })));
        }

        //保存 ==========================
        void btnSubmit_Click(object sender, EventArgs e)
        {
            //检查是否存在同名的供应商
            string vendorCode = txtVendorCode.Text;
            VendorB vendor = WebRepository.FetchVendor(vendorCode);
            if (vendor != null)
            {
                if (vendor.ID != this.IdentityKey)
                {
                    this.Alert(string.Format("已经存在代码为&nbsp;{0}&nbsp;的供应商", vendorCode), Core.DialogIcon.Warning);
                    return;
                }
            }

            vendor = this.CUID == Core.CUID.Create ? new VendorB() : WebRepository.FetchVendor(this.IdentityKey);
            if (vendor == null)
            {
                this.Alert("保存失败，消息：不存在指定的供应商资料！", Core.DialogIcon.Warning);
                return;
            }


            //WebRepository.FetchEntity(new VendorB { ID = this.IdentityKey });
            //if (this.CUID == Core.CUID.Create) vendor = new VendorB();

            //属性赋值
            vendor.ModifyDTM = DateTime.Now;
            vendor.VendorCode = txtVendorCode.Text.Trim();
            vendor.VendorName = txtVendorName.Text.Trim();
            vendor.AllowUsed = chkAllowUsed.Checked;
            vendor.CreateDate = DateTime.Parse(lblCreateDate.Text);
            vendor.Environment = DGContext.Environment;
            vendor.FirstStartTime = DateTime.Now;
            if (DGContext.User.Role == RoleType.Developer) //开发人员才可以更改下拉列表的值
                vendor.ReportPath = (ReportMap)Convert.ToByte(ddlReport.SelectedValue);

            Core.CUID orgCUID = this.CUID;

            //保存到数据库
            HandleArgs HANDLE = this.CUID == Core.CUID.Create ? WebRepository.InsertVendor(vendor) : WebRepository.UpdateVendor(vendor);
            if (HANDLE.Status == HandleStatus.Completed)
            {
                HANDLE.Message = "保存供应商资料成功！";
                this.IdentityKey = vendor.ID;
                this.UpdateCache(vendor);

                //参数值随界面一起保存
                HandleArgs paramHandle = this.SaveVendorParameterValues(vendor.ID);
                this.BindVendorParameter(vendor.ID);
                if (paramHandle.Status == HandleStatus.Fail)
                {
                    this.Alert("保存供应商资料成功，但" + paramHandle.Message, Core.DialogIcon.Warning);
                    return;
                }

                //保存成功，同步基础资料
                HandleArgs syncHandle = ERPRepository.DataSynchron();
                if (syncHandle.Status == HandleStatus.Fail)
                {
                    syncHandle.Message = "保存供应商成功，但同步资料时出错，请手动使用【基础资料>>同步资料】功能更新基础资料！";
                    this.Alert(syncHandle.Message, Core.DialogIcon.Warning);
                    return;
                }


            }


            this.Alert(HANDLE.Message,
                HANDLE.Status == HandleStatus.Completed ? Core.DialogIcon.Correct : Core.DialogIcon.Warning,
                orgCUID == Core.CUID.Create ? string.Format("location.href='{0}'", this.GetRouteUrl("vendor-edit", new { id = IdentityKey })) : string.Empty);
        }

        #endregion

        #region 辅助方法

       

        /// <summary>
        /// 绑定供应商品牌表格 
        /// </summary>
        private void BindVendorCard(int vendorID)
        {
            // 绑定品牌从资料
            List<VendorCardB> dataSource = WebRepository.QueryVendorCards(vendorID);
            this.gvVendorCard.DataSource = dataSource;
            this.gvVendorCard.DataBind();
            this.gvVendorCard.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 绑定店铺从资料
        /// </summary>
        /// <param name="vendorID"></param>
        private void BindVendorShop(int vendorID)
        {
            //绑定店铺从资料
            List<VendorShopB> dataSource = WebRepository.QueryVendorShops(vendorID);
            this.gvVendorShop.DataSource = dataSource;
            this.gvVendorShop.DataBind();
            this.gvVendorShop.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 绑定供应商参数表格（按供应商的关联报表 ReportPath 加载参数，再带出已设置的参数值）
        /// </summary>
        /// <param name="vendorID"></param>
        private void BindVendorParameter(int vendorID)
        {
            VendorB vendor = WebRepository.FetchVendor(vendorID);
            byte? reportPath = vendor == null ? (byte?)null : (byte)vendor.ReportPath;

            List<VendorParameterModel> dataSource = new VendorParameterBll().QueryVendorParameters(vendorID, reportPath);
            this.gvVendorParameter.DataSource = dataSource;
            this.gvVendorParameter.DataBind();
            this.gvVendorParameter.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        /// <summary>
        /// 保存供应商参数值：直接读取参数列表里每一行的输入框，随页面「保存」一起提交
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        private HandleArgs SaveVendorParameterValues(int vendorID)
        {
            HandleArgs result = new HandleArgs();
            result.Status = HandleStatus.Completed;

            if (this.gvVendorParameter.Rows.Count == 0)
            {
                return result;
            }

            VendorParameterBll parameterBll = new VendorParameterBll();
            foreach (GridViewRow row in this.gvVendorParameter.Rows)
            {
                HiddenField hdfParameterCode = row.FindControl("hdfParameterCode") as HiddenField;
                TextBox txtParameterValue = row.FindControl("txtParameterValue") as TextBox;
                if (hdfParameterCode == null || txtParameterValue == null) continue;

                HandleArgs handle = parameterBll.SaveVendorParameterValue(
                    vendorID, hdfParameterCode.Value, txtParameterValue.Text.Trim());
                if (handle.Status == HandleStatus.Fail)
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = handle.Message;
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// 绑定基础资料中的品牌
        /// </summary>
        /// <param name="cardCode"></param>
        private void BindBasCard(string cardCode)
        {
            //绑定所有的品牌资料
            List<CardB> dataSource = WebRepository.QueryCards(cardCode);
            this.gvCards.DataSource = dataSource;
            this.gvCards.DataBind();
            this.gvCards.CssClass = dataSource.Count == 0 ? "qp-gridview-empty" : "qp-gridview";
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

        /// <summary>
        /// 刷新缓存
        /// </summary>
        private void UpdateCache(VendorB vendor)
        {
            //更新缓存
            List<VendorB> vendors = CacheHelper.FetchCache<List<VendorB>>("Vendor");
            if (vendors != null)
            {
                VendorB cache = vendors.Find(v => v.ID == vendor.ID);//取出缓存中的供应商资料
                if (cache != null)
                {
                    vendors.Remove(cache);
                    vendors.Add(vendor);
                }
            }
        }

        #endregion

        #region AJAX 请求辅助

        /// <summary>
        /// 查询基础资料中的品牌
        /// </summary>
        private void QueryBasCard(string cardCode)
        {
            var ai = new AJAXInterop();

            cardCode = HttpUtility.UrlDecode(cardCode ?? string.Empty); //解码，否则可能会出现乱码的问题
            this.BindBasCard(cardCode);

            if (this.gvCards.Rows.Count == 0)
            {
                ai.StatusCode = "3";
                ai.Html = "没有查询到符合条件的品牌！";
                this.ResponseToClient(ai);
            }

            string html = this.RenderControl(this.gvCards);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);
        }

        /// <summary>
        /// 保存品牌供应商的品牌设置
        /// </summary>
        /// <param name="card"></param>
        /// <param name="vendorID"></param>
        /// <param name="scope">作用范围，只对该范围的品牌起作用</param>
        private void UpdateVendorCard(string card, string vendorID,string scope)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【vendorID】";
                ai.Message = "参数错误！";
                this.ResponseToClient(ai);
            }
            card = HttpUtility.UrlDecode(card ?? ""); //解码，否则可能会出现乱码的问题
            scope = HttpUtility.UrlDecode(scope ?? "");

            List<string> cardList = card.Split(',').ToList();
            List<string> scopeList = scope.Split(',').ToList();
            HandleArgs handle = WebRepository.UpdateVendorCards(Convert.ToInt32(vendorID), cardList, scopeList);
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }

            this.BindVendorCard(Convert.ToInt32(vendorID));
            string html = this.RenderControl(this.gvVendorCard);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);
        }

        ///// <summary>
        ///// 重新加载供应商品牌
        ///// </summary>
        ///// <param name="vendorID"></param>
        //private void ReloadVendCard(string vendorID)
        //{
        //    var ai = new AJAXInterop();
        //    if (string.IsNullOrEmpty(vendorID))
        //    {
        //        ai.StatusCode = "2";
        //        ai.Html = "参数错误，消息：【vendorID】";
        //        this.ReponseToClient(ai);
        //    }

        //    this.BindVendorCard(Convert.ToInt32(vendorID));
        //    string html = this.RenderControl(this.gvVendorCard);
        //    ai.StatusCode = "1";
        //    ai.Html = html;
        //    this.ReponseToClient(ai);
        //}

        /// <summary>
        /// 保存/更新供应商店铺
        /// </summary>
        /// <param name="vendorID"></param>
        private void UpdateVendorShop(string vendorID,
            string shopCode,
            string shopName,
            string shopID,
            string stockCode,
            string stockName)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【vendorID】";
                this.ResponseToClient(ai);
            }


            //初始化/更新店铺
            shopID = string.IsNullOrEmpty(shopID) ? "0" : shopID;
            int vendorShopID = Convert.ToInt32(shopID);
            VendorShopB vs = vendorShopID != 0 ? (WebRepository.FetchVendorShop(vendorShopID) ?? new VendorShopB()) : new VendorShopB();
            vs.ID = Convert.ToInt32(shopID);
            vs.VendorShopCode = shopCode;
            vs.VendorShopName = shopName;
            vs.VendorStockCode = stockCode;
            vs.VendorStockName = stockName;
            vs.VendorID = Convert.ToInt32(vendorID);
            vs.CreateDate = vs.ID == 0 ? DateTime.Now : vs.CreateDate;

            //检查是否已经存在相同的店铺代码
            VendorShopB vendorShop = WebRepository.FetchVendorShop(vs.VendorShopCode);
            if (vendorShop != null)
            {
                if (vendorShop.ID != vs.ID)
                {
                    ai.Message = string.Format("已经存在代码为&nbsp;{0}&nbsp;店铺", vs.VendorShopCode);
                    ai.Html = string.Format("已经存在代码为&nbsp;{0}&nbsp;店铺", vs.VendorShopCode);
                    ai.StatusCode = "2";
                    this.ResponseToClient(ai);
                }
            }

            //保存到数据库
            HandleArgs handle = vs.ID == 0 ? WebRepository.InsertVendorShop(vs) : WebRepository.UpdateVendorShop(vs);
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }


            //重新加载店铺信息
            this.BindVendorShop(vs.VendorID);
            string html = this.RenderControl(this.gvVendorShop);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);           
        }

        /// <summary>
        /// 删除店铺ID
        /// </summary>
        /// <param name="shopID"></param>
        private void DeleteVendorShop(string shopID,string vendorID)
        {
            var ai = new AJAXInterop();
            if (string.IsNullOrEmpty(shopID) || string.IsNullOrEmpty(vendorID))
            {
                ai.StatusCode = "2";
                ai.Html = "参数错误，消息：【shop&nbsp;vendor】";
                this.ResponseToClient(ai);
            }

            int vendShopID = Convert.ToInt32(shopID);
            //保存到数据库
            HandleArgs handle = WebRepository.DeleteVendorShop(new VendorShopB { ID = vendShopID });
            if (handle.Status == HandleStatus.Fail)
            {
                ai.Message = handle.Message;
                ai.Html = handle.Message;
                ai.StatusCode = "2";
                this.ResponseToClient(ai);
            }


            //重新加载店铺信息
            this.BindVendorShop(Convert.ToInt32(vendorID));
            string html = this.RenderControl(this.gvVendorShop);
            ai.StatusCode = "1";
            ai.Html = html;
            this.ResponseToClient(ai);           
        }

        #endregion
    }

    
}
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

namespace Gat.Web.admin
{
    public partial class gat_vendor_card : Core.PageSingleBase
    {
        /// <summary>
        /// 是否有品牌数据s
        /// </summary>
        protected bool HasData
        {
            get { return this.gvCards.Rows.Count != 0; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); //跟踪表示子类的ONLOAD比基类的ONLOAD事件要快，所以需要使用重写？
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);

            if (!IsPostBack)
            {
                this.TopTitle = "供应商资料";
                this.SubTitle = "品牌设置";

                VendorB vendor = WebRepository.FetchVendor(IdentityKey);
                if (vendor == null)
                {
                    this.Alert(
                        "抱歉，您浏览的供应商不存在！",
                        Core.DialogIcon.Warning,
                        string.Format("location.href='{0}';", this.GetRouteUrl("vendor", new { }))
                        );
                    return;
                }

                lblVendorName.Text = vendor.VendorName;

                //取所有品牌
                var cards = WebRepository.QueryCards();
                //取供应商品牌
                var vendCards = WebRepository.QueryVendorCards(this.IdentityKey);

                //整合数据源
                var dataSource =
                    cards.Select(c => new
                    {
                        CardID = c.CardID,
                        CardCode = c.CardCode,
                        CardName = c.CardName,

                        Checked = vendCards.Find(vc => vc.CardID == c.CardID) != null
                    });
                this.gvCards.DataSource = dataSource;
                this.gvCards.DataBind();
                this.gvCards.CssClass = dataSource.Count() == 0 ? "qp-gridview-empty" : "qp-gridview";
            }
        }

        #region 功能按钮

        void btnSearch_Click(object sender, EventArgs e)
        {
            //搜索 ==========

            //取所有品牌
            var cards = WebRepository.QueryCards(txtVendorCode.Text.Trim());
            //取供应商品牌
            var vendCards = WebRepository.QueryVendorCards(this.IdentityKey);

            //整合数据源
            var dataSource =
                cards.Select(c => new
                {
                    CardID = c.CardID,
                    CardCode = c.CardCode,
                    CardName = c.CardName,

                    Checked = vendCards.Find(vc => vc.CardID == c.CardID) != null
                });
            this.gvCards.DataSource = dataSource;
            this.gvCards.DataBind();
            this.gvCards.CssClass = dataSource.Count() == 0 ? "qp-gridview-empty" : "qp-gridview";
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            List<string> cardIDList = new List<string>();
            string tempCardID = null;
            CheckBox check = null;
            foreach (GridViewRow row in gvCards.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    check = row.FindControl("chkSelect") as CheckBox;
                    if (check == null || !check.Checked) continue;

                    tempCardID = gvCards.DataKeys[row.RowIndex].Value + "";
                    if (!string.IsNullOrEmpty(tempCardID)) cardIDList.Add(tempCardID);
                }
            }

            //HandleArgs handle = WebRepository.UpdateVendorCards(this.IdentityKey, cardIDList);
            //if (handle.Status == HandleStatus.Completed) handle.Message = "设置供应商品牌成功！";

            //this.RegisterDialog(handle.Message,
            //    handle.Status == HandleStatus.Completed ? Core.DialogIcon.Correct : Core.DialogIcon.Warning);
        }

        #endregion 
    }
}
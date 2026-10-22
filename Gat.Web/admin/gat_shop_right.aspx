<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true"
         CodeBehind="gat_shop_right.aspx.cs" Inherits="Gat.Web.admin.gat_shop_right" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>店铺权限设置</title>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/gat/shop.js" type="text/javascript"></script>
    
    <style type="text/css">
        
        .qp-gridview .template{
            width:100%;
            text-align:center;
            border:none;
        }
        .qp-gridview .template td{ border:none;padding:0; }
        .qp-gridview .template tr:hover td{ background:none; }
        .ui-icon-save{ margin-left:320px; }
        .layout span{ }
        .layout a.switch{
            float:left;
            outline:none;
            text-decoration:none;
            padding:2px 16px;
            margin:0px 0px 0px 220px;
            background:url(/images/icon_bg.png) no-repeat -5px -76px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="shopquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpBody" runat="server">
    <table cellspacing="0" cellpadding="0" class="mbox_form">
        
        <tr>
            <td class="field">
                提报公司：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlCompany" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="field">
                品牌供应商：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlVendor" runat="server">
                </asp:DropDownList>
                 <%--<span class="l"><%=this.VendorName%></span>
                 <%if (!string.IsNullOrEmpty(VendorName)) {%>
                    <a href="javascript:;" class="switch">选择</a>
               <% } %>--%>
            </td>
        </tr>
        <tr>
            <td class="field">
                提报品牌：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlCard" runat="server">
                </asp:DropDownList>
                <%--<%=this.CardName %>--%>
            </td>
        </tr>
        <%--<tr>
            <td class="none field">
                提报公司：
            </td>
            <td class="none layout">
                <%=this.CompanyName%>
               
            </td>
        </tr>--%>
    </table>
    <div style="color:#777;">
    说明：允许时表示此店铺或大仓将提报数据给品牌公司
    </div>
    <div class="gv-wrapper">
        <asp:GridView ID="gvShop" runat="server" AutoGenerateColumns = "False"  CssClass="qp-gridview" DataKeyNames="ShopID">
            <Columns>
           
                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkSelectAll" runat="server"/>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span style="display:none;">'<%#Eval("ShopID") %>'</span>
                        <asp:CheckBox ID="chkSelect" runat="server" Checked='<%# Bind("AllowUsed") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                    <ItemTemplate >
                       <span class="shop" style="display:none;"><%#Eval("ShopID") %></span>
                       <span ><%#Eval("ShopCode") %></span>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockCode" HeaderText="仓库代码" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockName" HeaderText="仓库名称" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="ModifyDTM" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center"  HeaderStyle-Width="150px"/>
                <asp:BoundField />
            </Columns>
            <EmptyDataTemplate>
                <div class="data-empty">
                    <i>&nbsp;&nbsp;</i>
                    没有查询到店铺资料，建议您使用【基础资料》同步资料】功能更新基础资料！
                </div>
            </EmptyDataTemplate>
            <AlternatingRowStyle CssClass="alternate" />
        </asp:GridView>
    </div>

    <!--选择框-->
    <div class="hidden" id="bas_gat">
        <div class="wrapper" >
            <table cellspacing="0" cellpadding="0" class="mbox_form">
                <tr>
                    <td class="field">
                        品牌供应商：
                    </td>                    
                    <td class="layout">
                      <asp:DropDownList ID="ddl_Vendor" runat="server" style="display:none;"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="field">
                        提报品牌：
                    </td>                    
                    <td class="layout">
                       <asp:DropDownList ID="ddlVendorCard" runat="server" style="display:none;">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="none field">
                        提报公司：
                    </td>
                    <td class="none layout">
                        <asp:DropDownList ID="dllSolCompany" runat="server" style="display:none;">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpAction" runat="server">
    <asp:Button ID="btnSave" runat="server" Text="保存" CssClass="btn-normal" Font-Names="微软雅黑" />
    <asp:Button ID="btnSearch" runat="server" Text="查询" CssClass="btn-normal" Font-Names="微软雅黑" />
    <% if (this.btnSave.Visible) {%>
        <button type="button" class="ui-icon-save">保存</button>
    <% } %>
</asp:Content>

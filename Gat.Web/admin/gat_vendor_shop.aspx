<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_vendor_shop.aspx.cs" Inherits="Gat.Web.admin.gat_vendor_shop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
        <!--主表-->
    <table cellspacing="0" cellpadding="0" class="mbox_form">        
        <tr>
            <td class="field">
                供应商名称：
            </td>
            <td class="layout">
                <asp:Label ID="lblVendorName" runat="server" />
            </td>    
        </tr>
        <tr>
            <td class="field">
                店铺代码：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtShopCode" runat="server" CssClass="text"></asp:TextBox>
            </td>            
        </tr>
        <tr>
            <td class="field">
                店铺名称：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtShopName" runat="server" CssClass="text"></asp:TextBox>
            </td>            
        </tr>
        <tr>
            <td class="field">
                仓库代码：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtStockCode" runat="server" CssClass="text"></asp:TextBox>
            </td>            
        </tr>
        <tr>
            <td class="field">
                仓库名称：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtStockName" runat="server" CssClass="text"></asp:TextBox>
            </td>            
        </tr>
         <tr>
            <td class="none field">
                创建日期：
            </td>
            <td class="none layout">
                <asp:Label ID="lblCreateDate" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
    
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_vendor_card.aspx.cs" Inherits="Gat.Web.admin.gat_vendor_card" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>供应商资料</title>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <!--文本水印-->
    <script src="/scripts/jquery.watermark-3.1.3/jquery.watermark.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        jQuery(function ($) {
            var $vc = $('#<%= txtVendorCode.ClientID%>');
            $vc
            .watermark("请输入品牌代码", "watermark2");

            $('#vendquery')
            .click(function (e) {
                if ($.trim($vc.val()) === '') {
                    e.preventDefault();
                    return false;
                }

                $(":submit[id$='Search']").trigger('click');
            });

            var gv = $('div.content')
                    .find("table[id$='gvCards']");

            var allSelect = true;
            var $checkboxs = gv.find(':checkbox');
            var $checkAll = $checkboxs.filter("[id*='chkSelectAll']");
            var $check = $checkboxs.filter("[id*='chkSelect']");

            $check.each(function () {
                if (!($(this).is(':checked'))) allSelect = false;
            });
            //alert(all_selected);
            if (allSelect) $checkAll.attr('checked', 'checked');
            $checkAll.change(function () {
                if ($(this).is(':checked')) {
                    $check.attr('checked', 'checked');
                } else {
                    $check.attr('checked', false);
                }
            });


            //jquery.ui.button
            var $uib = $('button.ui-icon-save');
            $uib
            .eq(0)
            .css("margin-left", "250px")
            .button({
                icons: {
                    primary: "ui-icon-arrowthickstop-1-w"
                }
            })
            .click(function (e) {
                //                
                //                e = e || window.event;
                //                e.preventDefault();
                location.href = '<%=this.IdentityKey %>' === '0' ? '<%=this.GetRouteUrl("vendor",new { }) %>' : '<%=this.GetRouteUrl("vendor-edit",new { id =this.IdentityKey })%>';
            })
            .next("button")
            .css("margin-left", "10px")
            .button({
                icons: {
                    primary: "ui-icon-disk"
                }
            })
            .click(function () {
                //if ($.trim($vc.val()) === '') return false;
                $(":submit[id$='Save']").trigger('click');
            })
            ;
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
       <div class="search">
            <asp:TextBox ID="txtVendorCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="vendquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <!--主表-->
    <table cellspacing="0" cellpadding="0" class="mbox_form">
        <tr>
            <td class="none field">
                供应商名称：
            </td>
            <td class="none layout">
                <asp:Label ID="lblVendorName" runat="server" Text="Label"></asp:Label>               
            </td>    
        </tr>        
    </table>

    <asp:GridView ID="gvCards" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" DataKeyNames="CardID">
        <Columns>
            <asp:TemplateField HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" />
                </HeaderTemplate>
                <ItemTemplate >
                    <asp:CheckBox ID="chkSelect" runat="server" Checked='<%#Eval("Checked") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="品牌代码" DataField="CardCode" HeaderStyle-Width="200px" />
            <asp:BoundField HeaderText="品牌名称" DataField="CardName" HeaderStyle-Width="200px" />
            <asp:BoundField />
        </Columns>
        <EmptyDataTemplate>
           <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到品牌资料，建议您使用<a href="javascript:;" class="datasync">【同步资料】</a>功能更新基础资料！
           </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
    <button type="button" class="ui-icon-save">返回</button>

    <%if (this.HasData) {%>
        <button type="button" class="ui-icon-save">保存</button>
        <asp:Button ID="btnSave" runat="server" Text="保存" CssClass="btn-normal"/>
    <% } %>
    <asp:Button ID="btnSearch" runat="server" Text="搜索" CssClass="btn-normal"/>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_syscontrol.aspx.cs" Inherits="Gat.Web.admin.gat_syscontrol" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>店铺管控设置</title>

    <!--jquery.validate-->
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/gat/syscontrol.js" type="text/javascript"></script>
    <script type="text/javascript">
       <%-- $(document).ready(function () {

            var $vc = $('#<%= txtSolutionCode.ClientID%>');
            $vc
                .watermark("请输入方案标题", "watermark2");

            $('#solutionquery')
                .click(function (e) {
                    if ($.trim($vc.val()) === '') {
                        e.preventDefault();
                        return false;
                    }

                    $(':submit').trigger('click');
                });
        });--%>

        function onGridDiscountValueChanged(e) {
            var sender = e;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpBody" runat="server">
    <div class="var-solution-lis">
        <asp:GridView ID="gvSolution" runat="server" AutoGenerateColumns="False"
            CssClass="qp-gridview">
            <Columns>
                <%-- <asp:TemplateField HeaderStyle-Width="5%" HeaderText="品牌供应商">
                    <ItemTemplate>
                        <asp:Label ID="Label" runat="server" Text='<%# Bind("VendorName") %>'></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="TxtBox" runat="server" Width="5%"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>--%>
                 <asp:TemplateField HeaderStyle-Width="5%" HeaderText="ID"  Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="labID" runat="server" Text='<%#Bind("ID") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                 <asp:TemplateField HeaderStyle-Width="5%" HeaderText="品牌供应商代码" >
                    <ItemTemplate>
                        <asp:Label ID="labVendorCode" runat="server" Text='<%#Bind("VendorCode") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-Width="5%" HeaderText="品牌供应商" >
                    <EditItemTemplate>
                        <asp:Label ID="labVendorName" runat="server"  Text='<%#Bind("VendorName") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="labVendorName" runat="server" Text='<%#Bind("VendorName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-Width="5%" HeaderText="店铺数">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtShopNumber" runat="server" Style="ime-mode: disabled" Width="50px" onkeydown="javascript:myKeyDown()" Text='<%#Bind("ShopNumber") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtShopNumber" runat="server" Style="ime-mode: disabled" Width="50px" onkeydown="javascript:myKeyDown(this)" onchange="javascript:onGridDiscountValueChanged(this)" Text='<%#Bind("ShopNumber") %>'></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
            <EmptyDataTemplate>
                <div class="data-empty">
                    <i>&nbsp;</i>
                    没有供应商记录！            
                </div>
            </EmptyDataTemplate>
            <AlternatingRowStyle CssClass="alternate" />
        </asp:GridView>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpAction" runat="server">
    <div style="display: none;">
        <asp:Button ID="btnSearch" runat="server" Text="Button" />
    </div>
    <span>
        <asp:Button ID="btnSave" runat="server" Text="保存店铺数量" OnClick="btnSave_Click" />
    </span>
</asp:Content>




<%--<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="cpHeaderNav" >
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtSolutionCode" runat="server"   />
            <a href="javascript:;" hidefocus="true" title="搜索" id="solutionquery">&nbsp;&nbsp;</a>
        </div>
        <a style="line-height: 35px;" href="<%= this.GetRouteUrl("solution-add", new { }) %>">新增方案</a>
    </div>
</asp:Content>--%>




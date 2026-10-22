<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_solution_browse.aspx.cs" Inherits="Gat.Web.admin.gat_solution_browse" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>提报方案</title>

    <!--jquery.validate-->
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/gat/solution_browse.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {

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
         });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpBody" runat="server">
    <div class="var-solution-lis">
    <asp:GridView ID="gvSolution" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:TemplateField HeaderText="方案名称" HeaderStyle-Width="300px">
                <ItemTemplate>
                    <span class="solutionID" style="display:none;"><%#Eval("ID") %></span>
                    <a href="<%# this.GetRouteUrl("solution-edit", new { id=Eval("ID") }) %>"><%# Eval("Title") %></a>
                </ItemTemplate>
                <HeaderStyle Width="340px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CompanyName" HeaderText="提报公司" HeaderStyle-Width="120px">
                <HeaderStyle Width="120px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="品牌供应商" DataField="VendorName" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <ItemStyle HorizontalAlign="Left" Width="120px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="CardName" HeaderText="提报品牌" HeaderStyle-Width="120px">
                <HeaderStyle Width="120px"></HeaderStyle>
            </asp:BoundField>
             <asp:BoundField DataField="ShopNumber" HeaderText="店铺数" HeaderStyle-Width="120px">
                <HeaderStyle Width="120px"></HeaderStyle>
            </asp:BoundField>
            <asp:TemplateField HeaderText="取数起始时间" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="150px">
                <ItemTemplate>
                    <%# Convert.ToDateTime(Eval("RepeatStartTime")).ToString("yyyy-MM-dd HH:mm:ss")%>
                </ItemTemplate>

                <HeaderStyle Width="150px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="状态" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <%# Convert.ToBoolean(Eval("Enabled"))==true?"使用中":"已停用" %>
                </ItemTemplate>

            </asp:TemplateField>


            <asp:TemplateField HeaderText="操作" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <a onclick="deleteSolution(this);" href="javascript:;">删除</a>
                    &nbsp;&nbsp;
                     <a onclick="setSolution(this);" href="javascript:;">
                         <%# Convert.ToBoolean(Eval("Enabled"))==true?"停用":"启用" %>
                     </a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有提报方案记录！            
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
</asp:Content>
<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="cpHeaderNav">
    <div class="nav">

        <div class="search">
            <asp:TextBox ID="txtSolutionCode" runat="server" />
            <a href="javascript:;" hidefocus="true" title="搜索" id="solutionquery">&nbsp;&nbsp;</a>
        </div>
        <a style="line-height: 35px;" href="<%= this.GetRouteUrl("solution-add", new { }) %>">新增方案</a>
    </div>
</asp:Content>


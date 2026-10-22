<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_vendor_bowse.aspx.cs" Inherits="Gat.Web.admin.gat_vendor_bowse" %>
<%@ Import Namespace="Gat.Utils" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>浏览供应商</title>
    <script type="text/javascript">
        $(document).ready(function () {
            /*$('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });*/
            
            var $vc = $('#<%= txtVendorCode.ClientID%>');
            $vc
            .watermark("请输入供应商代码", "watermark2");
            
            $('#vendquery')
            .click(function(e){
                 if($.trim($vc.val()) === ''){
                    e.preventDefault();
                    return false;
                 }

                 $(':submit').trigger('click');
            });    
        });
        /*PageClick = function (pageclickednumber) {
            window.location.href = '/log/' + pageclickednumber;
        }*/
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">        
        <div class="search">
            <asp:TextBox ID="txtVendorCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="vendquery">&nbsp;&nbsp;</a>     
        </div>
        <a href=" <%=this.GetRouteUrl("vendor-add", new { }) %>" style="line-height:35px;">新增供应商</a>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpBody" runat="server">
    <asp:GridView ID="gvVendor" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
            <asp:TemplateField HeaderText="供应商代码" HeaderStyle-Width="120px">
                <ItemTemplate >
                    <a href="<%# this.GetRouteUrl("vendor-edit", new { id=Eval("ID") }) %>"><%# Eval("VendorCode")%></a>
                </ItemTemplate>

<HeaderStyle Width="120px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField HeaderText="供应商名称" DataField="VendorName" HeaderStyle-Width="120px" >
<HeaderStyle Width="120px"></HeaderStyle>
            </asp:BoundField>
            <asp:TemplateField HeaderText="运行状态" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate >
                    <span><%# ((RunEnvironment)Convert.ToByte(Eval("Environment")) == RunEnvironment.Due ? "正式环境" : "测试环境") + (Convert.ToBoolean(Eval("AllowUsed")) ? "（<span style='color:#656565;'>允许<span>）" : "（<span style='color:#656565;'>禁用<span>）")%></span>
                </ItemTemplate>

<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField HeaderText="创建日期" DataField="CreateDate" HeaderStyle-Width="120px" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-HorizontalAlign="Center">
<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="修改时间" DataField="ModifyDTM" HeaderStyle-Width="180px" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center">
<HeaderStyle Width="180px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
           <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到供应商记录！
           </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>
<asp:Content ID="Content5" runat="server" contentplaceholderid="cpTop">
    <div style="display:none;">
        <asp:Button ID="btnSearch" runat="server" Text="Button" />
    </div>
</asp:Content>


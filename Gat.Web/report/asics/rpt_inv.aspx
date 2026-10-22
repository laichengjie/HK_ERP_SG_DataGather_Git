<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv.aspx.cs" Inherits="Gat.Web.report.asics.rpt_inv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入产品编码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/asics/inv/1';
                url = url + '?productCode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/asics/inv/' + pageclickednumber+ "?productCode=" + encodeURI('<%=productCode %>');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <asp:GridView ID="gvPos" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
          
            <asp:BoundField HeaderText="仓库名称" DataField="StockName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>  
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />        
            <asp:BoundField HeaderText="尺码" DataField="SizeName" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" /> 
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />     
            <asp:BoundField HeaderText="数量" DataField="Qty" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />              
         
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到库存提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

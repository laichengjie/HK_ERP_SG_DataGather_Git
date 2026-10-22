<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv.aspx.cs" Inherits="Gat.Web.report.kappa.rpt_inv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/kappa/inv/1';
                url = url + '?shopcode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/kappa/inv/' + pageclickednumber+ "?shopcode=" + encodeURI('<%=shopCode %>');
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
    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>            
            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="店铺经营品牌" DataField="CardName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>            
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />            
            <asp:BoundField HeaderText="期末库存" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right"/>                
            <asp:BoundField HeaderText="期末库存日期" DataField="UpdateDate" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center"  DataFormatString="{0:yyyy-MM-dd}"/>
            <asp:BoundField />
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

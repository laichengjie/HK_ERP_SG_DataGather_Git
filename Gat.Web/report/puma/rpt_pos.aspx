<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.puma.rpt_pos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/puma/pos/1';
                url = url + '?shopcode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/puma/pos/' + pageclickednumber+ "?shopcode=" + encodeURI('<%=shopCode %>');
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
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>            
            <asp:BoundField HeaderText="单号" DataField="BillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>            
            <asp:BoundField HeaderText="店铺代码" DataField="ShopID" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="店铺经营品牌" DataField="CardName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />        
            <asp:BoundField HeaderText="数量" DataField="Qty" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N0}"/> 
            <asp:BoundField HeaderText="销售金额" DataField="Amount" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}"/>           
            <asp:BoundField HeaderText="销售时间" DataField="BillDateTime" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd}"/>
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到销售提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

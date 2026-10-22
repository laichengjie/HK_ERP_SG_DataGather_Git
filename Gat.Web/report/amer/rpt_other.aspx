<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_other.aspx.cs" Inherits="Gat.Web.report.amer.rpt_other" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>其他出入库报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入产品编码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/amer/others/1';
                url = url + '?productCode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/amer/others/' + pageclickednumber+ "?shopcode=" + encodeURI('<%=productCode %>');
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
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>    
            <asp:BoundField HeaderText="单号" DataField="BillNo" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left" />  
            <asp:BoundField HeaderText="单据日期" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd }"/>            
            <asp:BoundField HeaderText="单据类型" DataField="BillType" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>        
            <asp:BoundField HeaderText="仓库名称" DataField="StoreName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left" />  
            <asp:BoundField HeaderText="产品编码" DataField="ProductCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />        
            <asp:BoundField HeaderText="产品名称" DataField="ProductName" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" /> 
            <asp:BoundField HeaderText="单位" DataField="ProductUnit" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />     
            <asp:BoundField HeaderText="数量" DataField="ProductQty" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />  
            <asp:BoundField HeaderText="仓库名称" DataField="StoreName" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>    
            <asp:BoundField HeaderText="备注" DataField="Remark" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />           
         
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到其他出入库提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

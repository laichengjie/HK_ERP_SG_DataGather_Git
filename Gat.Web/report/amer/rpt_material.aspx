<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_material.aspx.cs" Inherits="Gat.Web.report.amer.rpt_material" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>产品报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入产品编码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/amer/products/1';
                url = url + '?productCode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/amer/products/' + pageclickednumber+ "?productCode=" + encodeURI('<%=productCode %>');
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
            <asp:BoundField HeaderText="产品编码" DataField="ProductCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" /> 
            <asp:BoundField HeaderText="产品条码" DataField="ProductBar" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />  
            <asp:BoundField HeaderText="产品名称" DataField="ProductName" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />  
            <asp:BoundField HeaderText="产品规格" DataField="ProductStandard" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />         
            <asp:BoundField HeaderText="单位" DataField="ProductUnit" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
             <asp:BoundField HeaderText="单价" DataField="ProductPrice" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" /> 
             <asp:BoundField HeaderText="备注" DataField="Remark" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />             
         
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到产品提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

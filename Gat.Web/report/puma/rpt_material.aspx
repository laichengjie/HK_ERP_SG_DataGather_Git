<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_material.aspx.cs" Inherits="Gat.Web.report.puma.rpt_material" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
     <title>货品报表</title>

     <script type="text/javascript">
         jQuery(function ($) {
             var $mat = $('#<%=txtMatCode.ClientID %>');
             $mat.watermark("请输入货号", "watermark2");
             $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
             $('#matquery').click(function(){
                var url = '/report/puma/mat/1';
                url = url + '?matcode=' + encodeURI($mat.val());
                location.href = url;
             });
         });

         PageClick = function (pageclickednumber) {
             window.location.href = '/report/puma/mat/' + pageclickednumber+ "?matcode=" + encodeURI('<%=matCode %>');
         }
     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtMatCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <asp:GridView ID="gvMat" runat="server" AutoGenerateColumns = "False" 
            CssClass="qp-gridview" >
            <Columns>
                <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>                
                <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField HeaderText="条形码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField HeaderText="尺码" DataField="Size" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField HeaderText="品牌" DataField="CardName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField HeaderText="类别" DataField="KindName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField HeaderText="系列" DataField="Series" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/> 
                <asp:BoundField HeaderText="款型" DataField="Model" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left"/>             
                <asp:BoundField HeaderText="零售价" DataField="RetailPrice" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}"/>
            </Columns>
            <EmptyDataTemplate>
               <div class="data-empty">
                    <i>&nbsp;</i>
                    没有查询到货品提报记录！
               </div>
            </EmptyDataTemplate>
        </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

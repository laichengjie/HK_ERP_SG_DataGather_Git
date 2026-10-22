<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv_day.aspx.cs" Inherits="Gat.Web.report.adidas_wif_sale.rpt_inv.rpt_inv_day" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>阿迪智慧零售-点库存日报表</title>

    <script type="text/javascript">
        jQuery(function ($) {

            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });

           $('#cpBody_txtStartBillDate,#cpBody_txtStartBillTime').css('width', '150px')
               .bind('focus', function () {
                   $(this).css('border', 'none')
               }).kendoDatePicker({
                   format: 'yyyy-MM-dd 00:00:00',// HH:mm:ss
                   min: new Date(2000, 01, 01),
                   open: function () {
                       $(document.body).addClass('overflow');
                   },
                   close: function () {
                       $(document.body).removeClass('overflow');
                   }
               });
           $('#cpBody_txtEndBillTime,#cpBody_txtEndBillDate').css('width', '150px')
               .bind('focus', function () {
                   $(this).css('border', 'none')
               }).kendoDatePicker({
                   format: 'yyyy-MM-dd 23:59:59',// HH:mm:ss
                   min: new Date(2000, 01, 01),
                   open: function () {
                       $(document.body).addClass('overflow');
                   },
                   close: function () {
                       $(document.body).removeClass('overflow');
                   }
               });
       });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/adoc/wifinvday/' + pageclickednumber
                + '?shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2")?.value)
                + '&barCode=' + encodeURI(document.getElementById("cpBody_txtBarcode")?.value)
                + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode")?.value)
                + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode")?.value)
                + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime")?.value)
                + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime")?.value)
                + '&stockCode=' + encodeURI(document.getElementById("cpBody_txtStockCode")?.value)
                ;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div>
        <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />
    </div>
    <div>
        <span>
            <span style="color: #FF6600; font-weight: bold;">库存日期</span>：
                 <asp:TextBox ID="txtStartBillTime" runat="server" CssClass="datepicker"></asp:TextBox>
            -
                 <asp:TextBox ID="txtEndBillTime" runat="server" CssClass="datepicker"></asp:TextBox>

            <br />
            <label>店铺编号：</label><asp:TextBox ID="txtStockCode" runat="server"></asp:TextBox>
            <label>条码：</label><asp:TextBox ID="txtBarcode" runat="server"></asp:TextBox>
            <label>产品编号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>尺码名称：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            库存总数量:<asp:Label ID="label_top_totoal_qty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </span>
    </div>
    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:BoundField HeaderText="店铺编号" DataField="store_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="产品编号" DataField="product_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="尺码名称" DataField="size_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="条码" DataField="bar_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="数量" DataField="qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="库存快照日期" DataField="stats_date" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
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

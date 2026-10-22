<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_invdetail.aspx.cs" Inherits="Gat.Web.report.on.rpt_invdetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function () {
                var url = '/report/on/inv/1';
                url = url + '?shopcode=' + encodeURI($shop.val());
                location.href = url;
            });
            $('#cpBody_bthSearch').click(function () {
                var url = '/report/on/invdetail/<%=this.CurrentPage %>';
                url = url + '?shopcode=' + encodeURI(document.getElementById("cpBody_txtShopCode2")?.value)
                    + '&barCode=' + encodeURI(document.getElementById("cpBody_textBarCode")?.value)
                    + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode")?.value)
                    + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime")?.value)
                    + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime")?.value)
                    ;
                location.href = url;
            });

            $('input.datepicker')
                .eq(0)
                .css('width', '115px')
                .end()
                .bind('focus', function () {
                    $(this).css('border', 'none')
                }).kendoDatePicker({
                    format: 'yyyy-MM-dd',// HH:mm:ss
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
            window.location.href = '/report/on/invdetail/' + pageclickednumber
                + '?shopcode=' + encodeURI(document.getElementById("cpBody_txtShopCode2")?.value)
                + '&barCode=' + encodeURI(document.getElementById("cpBody_textBarCode")?.value)
                + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode")?.value)
                + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime")?.value)
                + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime")?.value)
                ;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server" />
            <a href="javascript:;" hidefocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div>

        <span style="color: #FF6600; font-weight: bold;">库存日期</span>：
                     <asp:TextBox ID="txtStartBillTime" runat="server" CssClass="datepicker"></asp:TextBox>
        -
                     <asp:TextBox ID="txtEndBillTime" runat="server" CssClass="datepicker"></asp:TextBox>
        &nbsp;&nbsp;
         <label>品牌店铺编号：</label><asp:TextBox ID="txtShopCode2" runat="server"></asp:TextBox>
        <label>货号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
        <%--<label>产品尺码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>--%>
        <label>条码：</label><asp:TextBox ID="textBarCode" runat="server"></asp:TextBox>
        <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
        <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />
    </div>
    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:BoundField HeaderText="品牌店铺编号" DataField="store_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="条码" DataField="sku" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="数量" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="是否导入" DataField="IsImport" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="库存日期" DataField="inventory_date" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField HeaderText="上传时间" DataField="ModifyDtm" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd}" />
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到库存提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
     <div id="pager1">
     <div style="float: right; margin-right: 60px;">总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
 </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

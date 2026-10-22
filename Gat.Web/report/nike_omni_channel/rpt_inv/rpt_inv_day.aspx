<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv_day.aspx.cs" Inherits="Gat.Web.report.nike_omni_channel.rpt_inv.rpt_inv_day" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>Nike全渠道-点库存日报表</title>

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
            window.location.href = '/report/nike/invday/' + pageclickednumber
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
    <div class="l">
        <span style="color: #FF6600; font-weight: bold;">方案</span>：
                    <asp:DropDownList ID="ddlSoule" runat="server" CssClass="k-picker-wrap">
                    </asp:DropDownList>
        &nbsp;&nbsp;
    </div>
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
            <label>仓库代码：</label><asp:TextBox ID="txtStockCode" runat="server"></asp:TextBox>
            <label>条码：</label><asp:TextBox ID="txtBarcode" runat="server"></asp:TextBox>
            <label>货号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>尺码代码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            在库总数量:<asp:Label ID="label_top_totoal_qty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
            在途总数量:<asp:Label ID="label_top_totoal_pathqty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </span>
    </div>
    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:BoundField HeaderText="库存日期" DataField="UpdateDate" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
            <asp:BoundField HeaderText="仓库代码" DataField="StockCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="尺码代码" DataField="SizeCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="尺码名称" DataField="SizeName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="在库数量" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="在途数量" DataField="PathQty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="零售价" DataField="RetailPrice" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />

            <asp:BoundField HeaderText="是否导入" DataField="IsImport" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="提报时间" DataField="ModifyTime" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
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

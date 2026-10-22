<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv.aspx.cs" Inherits="Gat.Web.report.iker.rpt_inv" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存报表</title>
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        /*datepicker*/
        .k-timepicker,
        .k-datetimepicker,
        .k-datepicker {
            border: 0 none;
            white-space: nowrap;
            width: 115px;
        }
    </style>
    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function () {
                var url = '/report/iker/inv/1';
                var shopCode = encodeURI($shop.val());
                if (shopCode != "") {
                    url = url + '?shopcode=' + encodeURI($shop.val());
                }
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
            var shopCode = encodeURI('<%=shopCode %>');
            if (shopCode != "") {
                window.location.href = '/report/iker/inv/' + pageclickednumber + "?shopcode=" + shopCode;
            } else {
                window.location.href = '/report/iker/inv/' + pageclickednumber;
            }


        }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>


<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">

    <div>
        <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />
    </div>

    <div class="l">
        <span style="color: #FF6600; font-weight: bold;">店铺代码</span>：
              <asp:TextBox ID="txtShopCode" runat="server"></asp:TextBox>
        <span style="color: #FF6600; font-weight: bold;">日期</span>：
                            <asp:TextBox ID="txtStarDate" runat="server" CssClass="datepicker"></asp:TextBox>
        -
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker"></asp:TextBox>
        &nbsp;&nbsp;
        <span>

            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
        </span>
    </div>

    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <%--<asp:BoundField HeaderText="店铺名称" DataField="ShopName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center"/>--%>
            <asp:BoundField HeaderText="品牌" DataField="CardName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="规格" DataField="SizeName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <%--<asp:BoundField HeaderText="仓库名称" DataField="StockName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center"/>--%>
            <asp:BoundField HeaderText="在库数量" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="在途数量" DataField="PathQty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="库存日期" DataField="UpdateDate" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField HeaderText="是否导入" DataField="IsImport" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到库存提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div>总库存数量:<%=this.SumCount %></div>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

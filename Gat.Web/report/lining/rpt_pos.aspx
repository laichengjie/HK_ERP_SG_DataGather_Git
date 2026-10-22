<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.lining.rpt_pos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function () {
                var url = '/report/lining/pos/<%=this.CurrentPage %>';
                url = url + '?shopcode=' + encodeURI($shop.val())
                    + '&shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                    + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                    + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                    + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime").value)
                    + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime").value)
                    + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStartBillDate").value)
                    + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndBillDate").value);
                location.href = url;
            });

            $('#cpBody_bthSearch').click(function () {
                var url = '/report/lining/pos/<%=this.CurrentPage %>';
                url = url + '?shopcode=' + encodeURI($shop.val())
                    + '&shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                    + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                    + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                    + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime").value)
                    + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime").value)
                    + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStartBillDate").value)
                    + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndBillDate").value);
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
            window.location.href = '/report/lining/pos/' + pageclickednumber + "?shopcode=" + encodeURI('<%=shopCode %>')
                + '&shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                + '&startBillTime=' + encodeURI(document.getElementById("cpBody_txtStartBillTime").value)
                + '&endBillTime=' + encodeURI(document.getElementById("cpBody_txtEndBillTime").value)
                + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStartBillDate").value)
                + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndBillDate").value);
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
    <div class="search">
        <div class="l">
            <span style="color: #FF6600; font-weight: bold;">方案</span>：
                            <asp:DropDownList ID="ddlSoule" runat="server" CssClass="k-picker-wrap">
                            </asp:DropDownList>
            &nbsp;&nbsp;
        </div>
        <div class="l">
            <span style="color: #FF6600; font-weight: bold;">导入日期</span>：
                            <asp:TextBox ID="txtStartBillTime" runat="server" CssClass="datepicker"></asp:TextBox>
            -
                            <asp:TextBox ID="txtEndBillTime" runat="server" CssClass="datepicker"></asp:TextBox>
            &nbsp;&nbsp;
            <span style="color: #FF6600; font-weight: bold;">销售日期</span>：
                            <asp:TextBox ID="txtStartBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
            -
                            <asp:TextBox ID="txtEndBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
            &nbsp;&nbsp;
        <span>
            <br />
            <label>店铺代码：</label><asp:TextBox ID="txtShopCode2" runat="server"></asp:TextBox>
            <label>货号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>尺码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            <asp:Button ID="btnDelSearch" runat="server" Text="删除时间段" OnClick="bthDelSearch_Click" Visible="false"  OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');"  />
            <asp:Button ID="benDelCehcked" runat="server" Text="删除选中" OnClick="bthDelChecked_Click" Visible="false"  OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </span>
        </div>

        <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />

        <br />
    </div>
    <asp:GridView ID="gvPos" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview" DataKeyNames="id">
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="门店代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="门店名称" DataField="ShopName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="交易单号" DataField="BillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="交易日期" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                <HeaderStyle Width="150px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="商品编码" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="产品尺码" DataField="SizeName" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="价格" DataField="BalaPrice" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="销售金额" DataField="SaleAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="销售数量" DataField="Qty" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="销售人员名称" DataField="PersonelName" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="销售类型" DataField="SaleType" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="会员手机号" DataField="MemberMobile" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到销售提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
    <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
        <div style="float: right; margin-right: 20px;">总金额;<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

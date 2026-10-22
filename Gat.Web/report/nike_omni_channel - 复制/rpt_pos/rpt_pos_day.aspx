<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos_day.aspx.cs" Inherits="Gat.Web.report.nike_omni_channel.rpt_pos.rpt_pos_day" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>阿迪全渠道-销售日报表</title>

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
            window.location.href = '/report/nike/posday/' + pageclickednumber
                + '?shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
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
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div class="search">
        <div class="l">
            <span style="color: #FF6600; font-weight: bold;">方案</span>：
                            <asp:DropDownList ID="ddlSoule" runat="server" CssClass="k-picker-wrap">
                            </asp:DropDownList>
            &nbsp;&nbsp;
        </div>
        <div class="l">
            <span style="color: #FF6600; font-weight: bold;">录单日期</span>：
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
            <label>销售店铺代码：</label><asp:TextBox ID="txtShopCode2" runat="server"></asp:TextBox>
            <label>货号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>尺码代码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            <asp:Button ID="btnDelSearch" runat="server" Text="删除时间段" OnClick="bthDelSearch_Click" OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            <asp:Button ID="benDelCehcked" runat="server" Text="删除选中" OnClick="bthDelChecked_Click" OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            总数量:<asp:Label ID="label_top_totoal_qty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
            总金额:<asp:Label ID="label_top_totoal_amount" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
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
                <HeaderStyle Width="20px"></HeaderStyle>
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="销售店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售日期" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="发货店铺代码" DataField="StockCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="发货时间" DataField="OutBillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="单号" DataField="BillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="货品全称" DataField="MaterialName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="尺码代码" DataField="SizeCode" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="尺码名称" DataField="SizeName" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售数量" DataField="Qty" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售价" DataField="BalaPrice" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售金额" DataField="SaleAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="零售价" DataField="RetailPrice" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="零售金额" DataField="RetailAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="折扣金额" DataField="DiscountAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="录单日期" DataField="BillTime" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="提报时间" DataField="ModifyTime" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="是否导入" DataField="IsImport" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Left" />

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

<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos_day.aspx.cs" Inherits="Gat.Web.report.adidas_wif_sale.rpt_pos.rpt_pos_day" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>阿迪智慧零售-销售日报表</title>

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
            window.location.href = '/report/adoc/wifposday/' + pageclickednumber
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
            <span style="color: #FF6600; font-weight: bold;">订单时间</span>：
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
            <label>店铺编号：</label><asp:TextBox ID="txtShopCode2" runat="server"></asp:TextBox>
            <label>产品编号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>尺码名称：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            <asp:Button ID="btnDelSearch" runat="server" Text="删除时间段" OnClick="bthDelSearch_Click"   OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            <asp:Button ID="benDelCehcked" runat="server" Text="删除选中" OnClick="bthDelChecked_Click"  OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
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


            <asp:BoundField HeaderText="店铺编号" DataField="store_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="订单编号" DataField="retail_id" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="订单明细编号" DataField="retailitem_id" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

      

            <asp:BoundField HeaderText="销售日期" DataField="BillDate" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="订单时间" DataField="billtime" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="付款时间" DataField="paytime" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="产品编号" DataField="product_code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="尺码名称" DataField="size_code" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="条码" DataField="bar_code" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="商品数量" DataField="qty" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="订单商品去除赠品总数量" DataField="upt_quantity" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>



            <asp:BoundField HeaderText="零售单价" DataField="pricelist" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="成交单价" DataField="priceactual" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="零售总金额" DataField="tot_amt_list" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="成交总金额" DataField="tot_amt_actual" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="营业员编号" DataField="staff_code" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="会员编号" DataField="vip_id" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="订单类型" DataField="retail_type" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售类型" DataField="source_type" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="80px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="插入时间" DataField="insert_time" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


    <%--        <asp:BoundField HeaderText="提报时间" DataField="ModifyTime" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>--%>

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

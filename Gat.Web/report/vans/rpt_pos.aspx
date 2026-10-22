<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.vans.rpt_pos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function () {
                var url = '/report/vans/pos/<%=this.CurrentPage %>';
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
                var url = '/report/vans/pos/<%=this.CurrentPage %>';
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
            window.location.href = '/report/vans/pos/' + pageclickednumber + "?shopcode=" + encodeURI('<%=shopCode %>')
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
            <span style="color: #FF6600; font-weight: bold;">制单日期</span>：
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

            <asp:BoundField DataField="Brand" HeaderStyle-Width="120px" HeaderText="品牌" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="店铺名称" DataField="ShopName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="120px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="单号" DataField="BillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售时间" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="SKU" DataField="SKU" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="货号" DataField="ItemNO" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="尺码" DataField="WaistSize" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="颜色" DataField="Inseam" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="条形码" DataField="ItemBarcode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="销售金额" DataField="PayAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="数量" DataField="Qty" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}">
                <HeaderStyle Width="60px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="结算金额" DataField="SettlementAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}">
                <HeaderStyle Width="80px"></HeaderStyle>

                <ItemStyle HorizontalAlign="Right"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="营业员代码" DataField="PersonnelCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="营业员名称" DataField="PersonnelName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="会员卡号" DataField="MemberCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="会员名称" DataField="MemberName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField HeaderText="活动代码" DataField="PromotionCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="活动名称" DataField="PromotionDescription" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="备注" DataField="Remarks" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="60px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="制单时间" DataField="BillTime" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="提报时间" DataField="ModifyDtm" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
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

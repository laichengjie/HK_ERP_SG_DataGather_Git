<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_Personnel.aspx.cs" Inherits="Gat.Web.report.adidas_wif_sale.rpt_BasicData.rpt_Personnel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>阿迪智慧零售-职员资料</title>

    <script type="text/javascript">
        jQuery(function ($) {

            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });

        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/adoc/wifbasicdata_personnel/' + pageclickednumber
                + '?&shopCode=' + encodeURI(document.getElementById("cpBody_txtShopCode").value)
                + '&staffCode=' + encodeURI(document.getElementById("cpBody_txtStaffCode").value)
                + '&staffName=' + encodeURI(document.getElementById("cpBody_txtName").value)
                ;
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
            <span>
                <br />
                <label>店铺编号：</label><asp:TextBox ID="txtShopCode" runat="server"></asp:TextBox>
                <label>店员编号：</label><asp:TextBox ID="txtStaffCode" runat="server"></asp:TextBox>
                <label>姓名：</label><asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
                总数量:<asp:Label ID="label_top_totoal_qty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
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


            <asp:BoundField HeaderText="店员编号" DataField="staff_code" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="姓名" DataField="name" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="店铺编号" DataField="store_code" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="在职状态" DataField="active" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="是否导购" DataField="saler" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="入职时间" DataField="entry_date" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="性别" DataField="sex" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField HeaderText="年龄" DataField="age" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="职位" DataField="position" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                <HeaderStyle Width="150px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField HeaderText="店员快照日期" DataField="stats_date" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}">
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
    <%-- <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
        <div style="float: right; margin-right: 20px;">总金额;<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></div>
    </div>--%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

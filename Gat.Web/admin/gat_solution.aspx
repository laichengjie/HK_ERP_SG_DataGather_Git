<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="gat_solution.aspx.cs" Inherits="Gat.Web.admin.gat_solution" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>提报方案</title>
    <!--jquery.validate-->
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .ui-icon {
            background-image: url(/images/ui-icons_ef8c08_256x240.png)
        }
        /*品牌*/
        .sol-cards, .sol-company {
            width: 50%;
        }
        /*gv
        .qp-gridview th {
            background: url() repeat-x scroll 0 0 transparent;
        }
        .qp-gridview tr:hover td {
            background: none repeat scroll 0 0 #fff;
        }*/

        /*datepicker*/
        .k-timepicker,
        .k-datetimepicker,
        .k-datepicker {
            border: 0 none;
            white-space: nowrap;
            width: 200px;
        }

        .map {
            outline: none;
            padding: 2px 18px;
            background: url(/images/icon_bg.png) no-repeat -0px -4px;
        }

        /*search*/
        .wrapper .search {
            background: url(/images/search_bg.png) no-repeat 0 0;
            width: 160px;
            height: 26px;
            float: right;
        }

            .wrapper .search input {
                width: 132px;
                border: 0;
                background: url() no-repeat 0 0 transparent;
                float: left;
                padding: 5px 0 5px 2px;
            }

            .wrapper .search a {
                height: 25px;
                float: right;
                width: 25px;
                text-decoration: none;
                outline: none;
            }
        /*店铺仓库查询按钮使用*/
        .wrapperCild .search {
            background: url(/images/search_bg.png) no-repeat 0 0;
            width: 160px;
            height: 26px;
            float: right;
        }

            .wrapperCild .search input {
                width: 132px;
                border: 0;
                background: url() no-repeat 0 0 transparent;
                float: left;
                padding: 5px 0 5px 2px;
            }

            .wrapperCild .search a {
                height: 25px;
                float: right;
                width: 25px;
                text-decoration: none;
                outline: none;
            }


        body {
            border: 0px solid #CCCCCC;
        }
    </style>
    <script type="text/javascript">
        /* -------------------
         * @全局变量声明
         * -----------------*/
        var solution = '<%=this.IdentityKey %>';
        var modified = false; //指示该页面是否已经有数据改变了，如果有，则需要提示用户保存再进行其它操作（导出、补采）
        var $txtShopCode = null;  //输入店铺代码以搜索店铺的文本框
        var $txtRepeatStartTime = null; //取数开始时间
        var $btnSave = null; //保存按钮
        var $hfCompany = null; //保存公司的隐域
        var $hfCard = null; //保存品牌的隐域
        var $hfVendor = null; //保存供应商的隐域
        var $ddlCompany = null; //公司下拉列表
        var $ddlCard = null; //品牌下拉列表
        var $ddlVendor = null; //供应商下拉列表
        var $hfallowList = null;// 选中的店铺列表
        var $chkEnabled = null;
        var $isAutoType = null;//提取方式类型
        var $SalesAmountType = null;//销售金额提取
        var body = null;
        var win = null;
        var loading = false; //是否正在加载内容
        var $hfStockList = null;//存放大仓选中的值
        var $tabStripSelect = null;//定位选择了那个标签0,店铺，1，大仓
        var $hfDefaultDiscountList = null;//选中的使用默认折扣的店铺
        var $hfDiscountList = null;
        var $hfTradeCodeList = null;

        jQuery(function ($) {

            //初始化全局变量值
            $txtShopCode = $('#<%=txtShopCode.ClientID %>');
            $txtRepeatStartTime = $('#<%=txtRepeatStartTime.ClientID %>');
            $btnSave = $('#<%=btnSave.ClientID %>');
            $hfCompany = $('#<%=hfCompany.ClientID %>');
            $hfCard = $('#<%=hfCard.ClientID %>');
            $hfVendor = $('#<%=hfVendor.ClientID %>');
            $ddlCompany = $('#<%=ddlCompany.ClientID %>');
            $ddlCard = $('#<%=ddlCard.ClientID %>');
            $ddlVendor = $('#<%=ddlVendor.ClientID %>');
            $chkEnabled = $('#<%=chkEnabled.ClientID %>');
            $isAutoType = $('#<%=cbAuto.ClientID %>');
            $SalesAmountType = $('#<%=cbPayAmount.ClientID %>');
            $hfallowList = $('#<%=hfallowList.ClientID%>');
            body = $(document.body);
            win = $(window);
            $hfStockList = $('#<%=hfStockList.ClientID%>');
            $tabStripSelect = $('#<%=tabStripSelect.ClientID%>');
            $hfDefaultDiscountList = $('#<%=hfDefaultDiscountList.ClientID%>');
            $hfDiscountList = $('#<%=hfDiscountList.ClientID%>');
            $hfTradeCodeList = $('#<%=hfTradeCodeList.ClientID%>');
            //tab标签
            $(document).ready(function () {
                $("#tabstrip").kendoTabStrip({
                    select: onSelect,
                    animation: {
                        open: {
                            effects: "fadeIn"
                        }
                    }
                });

                function onSelect(e) {
                    //选中的页签传送到后台
                    var selectIndex = $(e.item).find("> .k-link").text();
                    $tabStripSelect.val(selectIndex);
                    //$.ajax({
                    //    type: "POST",
                    //    url: '/admin/gat_solution.aspx',
                    //    timeout: 50000,
                    //    data: { mode: 'tabSelectIndex', tabSelectIndex: selectIndex }
                    //});


                }
            });
        });

        function myKeyDown(e) {
            var k = window.event.keyCode;
            var value = e.value + "";
            if ((k == 110 && value.indexOf(".") > 0) || (k == 109)) {//多小数点,负号处理
                window.event.returnValue = false;
            }
            if ((k == 46) || (k == 8) || (k == 189) || (k == 109) || (k == 190) || (k == 110) || (k >= 48 && k <= 57) || (k >= 96 && k <= 105) || (k >= 37 && k <= 40)) { }
            else if (k == 13) {
                window.event.keyCode = 9;
            }
            else {
                window.event.returnValue = false;
            }
        }

        function onGridDiscountValueChanged(e) {
            var sender = e;
        }

        
        
    </script>

    <script src="/scripts/gat/solution.js?v=20220726" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <%if (this.CUID == Gat.Web.Core.CUID.Update)
        { %>


    <%} %>
    <div class="nav">
        <%--    <div class="search">--%>
        <div>
            <asp:TextBox ID="txtShopCode" Visible="False" runat="server" /><!--公司的店铺代码-->
            <%-- <a href="javascript:;" hideFocus="true" title="搜索" id="vendquery" >&nbsp;&nbsp;</a>     --%>
        </div>
        <a href=" <%=this.GetRouteUrl("solution-add", new { }) %>" style="line-height: 35px;">新增提报方案</a>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <asp:LinkButton ID="doRefreshMaterialRangeGrid" runat="server" OnClick="btnDoRefreshMaterialRangeGrid_Click" Style="display: none" />
    <asp:LinkButton ID="doRefreshSpecialMaterialRangeGrid" runat="server" OnClick="btnDoRefreshSpecialMaterialRangeGrid_Click" Style="display: none" />
    <input id="solutionId" value="<%=this.IdentityKey %>" type="hidden" />
    <table cellspacing="0" cellpadding="0" class="mbox_form">

        <tr>
            <td class="field">提报公司：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlCompany" runat="server" CssClass="select" DataTextField="CompanyName" DataValueField="CompanyID">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="field">品牌供应商：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlVendor" runat="server" CssClass="select" DataTextField="VendorName" DataValueField="ID">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="field">提报品牌：
            </td>
            <td class="layout">
                <asp:DropDownList ID="ddlCard" runat="server" CssClass="select" DataTextField="CardName" DataValueField="CardID">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="field">取数起始时间：
            </td>
            <td class="layout valid">
                <asp:TextBox ID="txtRepeatStartTime" runat="server" CssClass="datepicker" />
            </td>
        </tr>
        <tr>
            <td class="field">提取方式：
            </td>
            <td class=" layout">
                <asp:RadioButton ID="cbAuto" GroupName="TiQu" runat="server" Checked="true" />自动提报&nbsp;&nbsp;
                <asp:RadioButton ID="cbManual" GroupName="TiQu" runat="server" />手动提报&nbsp;&nbsp;
                <asp:RadioButton ID="cbBackgroud" GroupName="TiQu" runat="server" />后台提报
            </td>
        </tr>
        <tr>
            <td class="field">销售金额取值：
            </td>
            <td class=" layout">
                <asp:RadioButton ID="cbPayAmount" GroupName="SalesAmountType" runat="server" Checked="true" />结算金额&nbsp;&nbsp;
                <asp:RadioButton ID="cbActualAmount" GroupName="SalesAmountType" runat="server" />实际结算金额&nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td class="field">Cron表达式：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtCron" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="field">提报路径：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtDictory" runat="server" Width="500px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="none field">允许使用：
            </td>
            <td class="none layout">
                <asp:CheckBox ID="chkEnabled" runat="server" Checked="true" />
            </td>
        </tr>
    </table>
    <%--       测试--%>
    <div id="tabstrip">
        <ul>
            <li class="k-state-active" id="shop">店铺仓库
            </li>
            <li id="stock">大仓
            </li>
            <li id="materialRange">不上传货品范围
            </li>
            <li id="specialMaterialRange">特殊货品范围
            </li>
            <li id="vendCustRight">客户
            </li>
            <li id="materialDiscount">货品折扣
            </li>
            <li id="stockSubModelSet">仓库子款型
            </li>
            <li id="onlineShopSet">网店归属线下店铺POS设置
            </li>
        </ul>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="bas_shopright" style="display: <% if(this.gvShop.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <div class="search">
                    <input id="txtShopSearchCode" type="text" /><!--供应商的店铺代码-->
                    <a href="javascript:;" hidefocus="true" title="搜索" id="searchButton" class="func-query">&nbsp;&nbsp;</a>
                </div>
                <div class="des">说明：允许时表示此店铺将提报数据给品牌公司</div>
            </div>
            <div class="gv-wrapper" id="gv-shop">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvShop" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ID" OnRowDataBound="gvShop_RowDataBound" OnSorting="gvShop_Sorting" AllowSorting="true">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <span class="hidden"><%#Eval("ShopID") %></span>
                                        <span class="shop" style="display: none;"><%#Eval("ShopID") %></span>
                                        <asp:CheckBox ID="chkSelect" runat="server" Checked='<%# Bind("AllowUsed") %>' />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" Checked='<%# Bind("AllowUsed") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ShopCode" HeaderText="店铺代码" HeaderStyle-Width="120px" SortExpression="ShopCode" />
                                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px" SortExpression="ShopName" />
                                <asp:BoundField DataField="StockCode" HeaderText="仓库代码" HeaderStyle-Width="120px" SortExpression="StockCode" />
                                <asp:BoundField DataField="StockName" HeaderText="仓库名称" HeaderStyle-Width="150px" SortExpression="StockName" />
                                <asp:BoundField DataField="VendorShopName" HeaderText="品牌店铺" HeaderStyle-Width="120px" SortExpression="VendorShopName" />
                                <asp:BoundField DataField="OMSShopName" HeaderText="不传店铺" HeaderStyle-Width="120px" SortExpression="OMSShopName" />
                                <asp:BoundField DataField="ModifyDTM" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100px" SortExpression="ModifyDTM" />
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="使用erp折扣" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkDiscount" runat="server" Checked='<%# Bind("DefaultDiscount") %>' />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkDiscount" runat="server" Checked='<%# Bind("DefaultDiscount") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="折扣" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDiscount" runat="server" Style="ime-mode: disabled" Width="50px" onkeydown="javascript:myKeyDown()" Text='<%#Bind("Discount") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtDiscount" runat="server" Style="ime-mode: disabled" Width="50px" onkeydown="javascript:myKeyDown(this)" onchange="javascript:onGridDiscountValueChanged(this)" Text='<%#Bind("Discount") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="品牌代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtTradeCode" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtTradeCode" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="品牌代码2" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtTradeCode2" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode2") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtTradeCode2" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode2") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="正价电商销售" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkIsECSale" runat="server" Checked='<%# Bind("IsECSale") %>' />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkIsECSale" runat="server" Checked='<%# Bind("IsECSale") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="不计算电商销售" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkIsNoCalculationSale" runat="server" Checked='<%# Bind("IsNoCalculationSale") %>' />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkIsNoCalculationSale" runat="server" Checked='<%# Bind("IsNoCalculationSale") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="操&nbsp;&nbsp;作" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <span class="hidden vendshop"><%#Eval("VendorShopID")%></span>
                                        <a hidefocus="true" class="map" href="javascript:;" onclick="javascript:shopMap(this);">品牌店铺</a>
                                        <span class="hidden company"><%#Eval("CompanyID")%></span>
                                        <span class="hidden shop"><%#Eval("ShopID")%></span>
                                        <span class="hidden vendor"><%#Eval("VendorID")%></span>
                                        <span class="hidden card"><%#Eval("CardID")%></span>
                                        <span class="hidden id"><%#Eval("ID")%></span>
                                        <a hidefocus="true" class="map" href="javascript:;" onclick="javascript:shopMapMultiple(this);">不传店铺</a>
                                        <span class="hidden OMSShopId"><%#Eval("OMSShopId")%></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到店铺资料，建议您使用【基础资料》同步资料】功能更新基础资料！
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_stockright" style="display: <% if (this.gvStock.Rows.Count == 0) { %>none; <%} %>">
            <div class="wrapperCild">
                <div class="search">
                    <input id="txtStockSearchCode" type="text" /><!--供应商的店铺代码-->
                    <a href="javascript:;" hidefocus="true" title="搜索" id="serachButtonStock" class="func-query">&nbsp;&nbsp;</a>
                </div>
                <div class="des">说明：允许时表示此大仓提报数据,大仓数据不区分品牌，请谨慎操作</div>
            </div>
            <div class="gv-stock" id="gv-stockID">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:GridView ID="gvStock" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ID" OnRowDataBound="gvStock_RowDataBound" OnSorting="gvStock_Sorting" AllowSorting="True">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkStockSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <span class="hidden"><%#Eval("StockID") %></span>
                                        <asp:CheckBox ID="chkStockSelect" runat="server" Checked='<%# Bind("AllowUsed") %>' />
                                    </ItemTemplate>

                                    <HeaderStyle Width="50px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="仓库代码" HeaderStyle-Width="120px" SortExpression="StockCode">
                                    <ItemTemplate>
                                        <span class="shop" style="display: none;"><%#Eval("StockID") %></span>
                                        <span><%#Eval("StockCode") %></span>
                                    </ItemTemplate>

                                    <HeaderStyle Width="120px"></HeaderStyle>
                                </asp:TemplateField>
                                <%--   <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>--%>
                                <asp:BoundField DataField="StockName" HeaderText="仓库名称" HeaderStyle-Width="150px" SortExpression="StockName">
                                    <HeaderStyle Width="150px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="品牌代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode" runat="server" Style="ime-mode: disabled" Width="50px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="VendorStockName" HeaderText="关联的仓库" HeaderStyle-Width="150px" SortExpression="VendorStockName">
                                    <HeaderStyle Width="150px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ModifyDTM" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" SortExpression="ModifyDTM">
                                    <HeaderStyle Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="操&nbsp;&nbsp;作" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <span class="hidden vendshop"><%#Eval("VendorStockID")%></span>
                                        <a hidefocus="true" class="map" href="javascript:;" onclick="javascript:stockMap(this);">关联</a>
                                        <span class="hidden company"><%#Eval("CompanyID")%></span>
                                        <span class="hidden stock"><%#Eval("StockID")%></span>
                                        <span class="hidden vendor"><%#Eval("VendorID")%></span>
                                        <span class="hidden card"><%#Eval("CardID")%></span>

                                        <%--                       <span class="hidden vendor"><%#Eval("VendorID")%></span>
                                <a hideFocus="true" class="map" href="javascript:;" onclick="javascript:stockMap(this);">关联</a>
                                <span class="hidden company"><%#Eval("CompanyID")%></span>
                                <span class="hidden vendshop"><%#Eval("VendorStockID")%></span>
                                <span class="hidden stock"><%#Eval("StockID")%></span>
                                <span class="hidden card"><%#Eval("CardID")%></span>--%>
                                    </ItemTemplate>

                                    <HeaderStyle Width="100px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到仓库资料，建议您使用【基础资料》同步资料】功能更新基础资料！
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>


                    </ContentTemplate>

                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_materialRangeRight" style="display: <% if(this.gvMaterialRange.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <input type="file" name="materialRangeFileName" id="materialRangeFileName" />
                <button id="btnMaterialRangeImport" type="button">导入</button>
                <button id="btnMaterialRangeImportFile" type="button">导出文件数据格式</button>
                <asp:Button ID="btnMaterialRangeDel" runat="server" Text="删除" OnClick="btnMaterialRangeDel_Click" />
                <asp:Button ID="btnMaterialRangeSave" runat="server" Text="保存" OnClick="btnMaterialRangeSave_Click" />
            </div>
            <div class="gv-wrapper" id="gv-MaterialRange">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvMaterialRange" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="MaterialRangeSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="MaterialRangeCheckBox" runat="server" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="MaterialRangeCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货号" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货品名称" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到不上传货品范围
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_specialMaterialRangeRight" style="display: <% if(this.gvSpecialMaterialRange.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <input type="file" name="specialMaterialRangeFileName" id="specialMaterialRangeFileName" />
                <button id="btnSpecialMaterialRangeImport" type="button">导入</button>
                <button id="btnSpecialMaterialRangeImportFile" type="button">导出文件数据格式</button>
                <asp:Button ID="btnSpecialMaterialRangeDel" runat="server" Text="删除" OnClick="btnSpecialMaterialRangeDel_Click" />
                <asp:Button ID="btnSpecialMaterialRangeSave" runat="server" Text="保存" OnClick="btnSpecialMaterialRangeSave_Click" />
            </div>
            <div class="gv-wrapper" id="gv-specialMaterialRange">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvSpecialMaterialRange" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="SpecialMaterialRangeSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="SpecialMaterialRangeCheckBox" runat="server" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="SpecialMaterialRangeCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货号" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货品名称" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="店铺代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("ShopCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("ShopCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="店铺名称" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtShopName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("ShopName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtShopName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("ShopName") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" />

                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到特殊货品范围
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_vendCustRight" style="display: <% if (this.gvStock.Rows.Count == 0) { %>none; <%} %>">
            <div class="wrapperCild">
                <asp:Button ID="btnVendCustAsync" runat="server" Text="同步客户" OnClick="BtnVendCustAsync_Click" />
            </div>
            <div class="gv-vendCust" id="gv-vendCustID">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:GridView ID="gvVendCust" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ID">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkVendCustSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkVendCustCheck" runat="server" Checked='<%# Bind("AllowUsed") %>'></asp:CheckBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkVendCustCheck" runat="server" Checked='<%# Bind("AllowUsed") %>' />
                                    </ItemTemplate>

                                    <HeaderStyle Width="50px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="客户代码" HeaderStyle-Width="120px" SortExpression="VendCustCode">
                                    <ItemTemplate>
                                        <span><%#Eval("VendCustCode") %></span>
                                    </ItemTemplate>

                                    <HeaderStyle Width="120px"></HeaderStyle>
                                </asp:TemplateField>
                                <%--   <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>--%>
                                <asp:BoundField DataField="VendCustName" HeaderText="客户名称" HeaderStyle-Width="150px" SortExpression="VendCustName">
                                    <HeaderStyle Width="150px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="品牌代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("TradeCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="50px" HeaderText="品牌代码2" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode2" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("TradeCode2") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtStockTradeCode2" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("TradeCode2") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ModifyDTM" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" SortExpression="ModifyDTM">
                                    <HeaderStyle Width="150px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:BoundField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到客户资料，建议您使用【同步客户】功能更新客户资料！
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>


                    </ContentTemplate>

                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_materialDiscount" style="display: <% if(this.gvMaterialDiscount.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <input type="file" name="materialDiscountFileName" id="materialDiscountFileName" />
                <button id="btnMaterialDiscountImport" type="button">导入</button>
                <button id="btnMaterialDiscountImportFile" type="button">导出文件数据格式</button>
                <asp:Button ID="Button1" runat="server" Text="删除" OnClick="btnMaterialDiscountDel_Click" />
                <asp:Button ID="Button2" runat="server" Text="保存" OnClick="btnMaterialDiscountSave_Click" />
            </div>
            <div class="gv-wrapper" id="gv-MaterialDiscount">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvMaterialDiscount" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="MaterialDiscountSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="MaterialDiscountCheckBox" runat="server" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="MaterialDiscountCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货号" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="货品名称" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialName" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("MaterialName") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="折扣" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtMaterialDiscount" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("Discount") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMaterialDiscount" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("Discount") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到货品折扣
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div class="bas_stockSubModelSet" style="display: <% if(this.gvStockSubModelSet.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <input type="file" name="stockSubModelSetFileName" id="stockSubModelSetFileName" />
                <button id="btnStockSubModelSetImport" type="button">导入</button>
                <button id="btnStockSubModelSetImportFile" type="button">导出文件数据格式</button>
                <asp:Button ID="Button3" runat="server" Text="删除" OnClick="btnStockSubModelSetDel_Click" />
                <asp:Button ID="Button4" runat="server" Text="保存" OnClick="btnStockSubModelSetSave_Click" />
            </div>
            <div class="gv-wrapper" id="gv-StockSubModelSet">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvStockSubModelSet" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="StockSubModelSetSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="StockSubModelSetCheckBox" runat="server" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="StockSubModelSetCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="ERP仓库编码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtStockCode_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("StockCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtStockCode_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("StockCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="商品子款型" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtSubModel_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("SubModel") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSubModel_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("SubModel") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="PCode" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtPCode_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("PCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtPCode_SS" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("PCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到货品折扣
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <%--网店归属线下店铺POS设置--%>
        <div class="bas_OnlineShopSet" style="display: <% if(this.gvOnlineShopSet.Rows.Count == 0){ %>none; <%} %>">
            <div class="wrapperCild">
                <input type="file" name="OnlineShopSetFileName" id="OnlineShopSetFileName" />
                <button id="btnOnlineShopSetImport" type="button">导入</button>
                <button id="btnOnlineShopSetImportFile" type="button">导出文件数据格式</button>
                <asp:Button ID="Button5" runat="server" Text="删除" OnClick="btnOnlineShopSetDel_Click"    OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
                <asp:Button ID="Button6" runat="server" Text="保存" OnClick="btnOnlineShopSetSave_Click"  OnClientClick="return confirm('确定要保存数据吗？');" />
            </div>
            <div class="gv-wrapper" id="gv-OnlineShopSet">
                <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvOnlineShopSet" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="OnlineShopSetSelectAll" runat="server" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="OnlineShopSetCheckBox" runat="server" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="OnlineShopSetCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="网店代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtOnlineShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("OnlineShopCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtOnlineShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("OnlineShopCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-Width="100px" HeaderText="线下店铺代码" ItemStyle-HorizontalAlign="Right">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtOfflineShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("OfflineShopCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtOfflineShopCode" runat="server" Style="ime-mode: disabled" Width="100px" Text='<%#Bind("OfflineShopCode") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="data-empty">
                                    <i>&nbsp;&nbsp;</i>
                                    没有查询到网店归属线下店铺POS设置
                                </div>
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="alternate" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>





    </div>

    <%--测试--%>

    <!--供应商店铺-->
    <div class="hidden" id="bas_vendshop">
        <div class="wrapper">
            <div class="search ng">
                <input id="txtVendShopCode" type="text" /><!--供应商的店铺代码-->
                <a href="javascript:;" hidefocus="true" title="搜索" class="func-query">&nbsp;&nbsp;</a>
            </div>
            <div class="des ng">
                说明：勾选时表示将店铺关联到品牌供应商的店铺
            </div>
            <asp:GridView ID="gvVendShop" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ID">
                <Columns>

                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span style="display: none;"><%#Eval("ID") %></span>
                            <asp:CheckBox ID="chkSelect" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                        <ItemTemplate>
                            <span class="shop" style="display: none;"><%#Eval("ID") %></span>
                            <span><%#Eval("VendorShopCode")%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="VendorShopName" HeaderText="店铺名称" HeaderStyle-Width="150px" />
                    <asp:BoundField DataField="VendorStockCode" HeaderText="仓库代码" HeaderStyle-Width="150px" />
                    <asp:BoundField DataField="VendorStockName" HeaderText="仓库名称" HeaderStyle-Width="150px" />
                </Columns>
                <EmptyDataTemplate>
                    <div class="data-empty">
                        <i>&nbsp;&nbsp;</i>
                        没有查询到供应商店铺资料！
                    </div>
                </EmptyDataTemplate>
                <AlternatingRowStyle CssClass="alternate" />
            </asp:GridView>
        </div>
    </div>

    <!--店铺资料-->
    <div class="hidden" id="bas_shop">
        <div class="wrapper">
            <div class="search ng">
                <input id="txtBasShopCode" type="text" /><!--店铺代码-->
                <a href="javascript:;" hidefocus="true" title="搜索" class="func-query">&nbsp;&nbsp;</a>
            </div>
            <div class="des ng">
                说明：勾选时表示将店铺关联到品牌供应商的电商店铺
            </div>
            <asp:GridView ID="gvBasShop" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ShopID">
                <Columns>

                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span style="display: none;"><%#Eval("ShopID") %></span>
                            <asp:CheckBox ID="chkSelect" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                        <ItemTemplate>
                            <span class="shop" style="display: none;"><%#Eval("ShopID") %></span>
                            <span><%#Eval("ShopCode")%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px" />
                </Columns>
                <EmptyDataTemplate>
                    <div class="data-empty">
                        <i>&nbsp;&nbsp;</i>
                        没有查询到店铺资料！
                    </div>
                </EmptyDataTemplate>
                <AlternatingRowStyle CssClass="alternate" />
            </asp:GridView>
        </div>
    </div>

    <!--供应商仓库-->
    <div class="hidden" id="bas_vendStock">
        <div class="wrapper">
            <div class="search ng">
                <input id="txtStockCode" type="text" /><!--供应商的店铺代码-->
                <a href="javascript:;" hidefocus="true" title="搜索" class="func-query">&nbsp;&nbsp;</a>
            </div>
            <div class="des ng">
                说明：勾选时表示将仓库关联到品牌供应商的仓库
            </div>
            <asp:GridView ID="gvVendStock" runat="server" AutoGenerateColumns="False" CssClass="qp-gridview" DataKeyNames="ID">
                <Columns>
                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span style="display: none;"><%#Eval("ID") %></span>
                            <asp:CheckBox ID="chkStockSelect" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="仓库代码" HeaderStyle-Width="120px">
                        <ItemTemplate>
                            <span class="shop" style="display: none;"><%#Eval("ID") %></span>
                            <span><%#Eval("VendorStockCode")%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="VendorStockName" HeaderText="仓库名称" HeaderStyle-Width="150px" />
                </Columns>
                <EmptyDataTemplate>
                    <div class="data-empty">
                        <i>&nbsp;&nbsp;</i>
                        没有查询到供应商仓库资料！
                    </div>
                </EmptyDataTemplate>
                <AlternatingRowStyle CssClass="alternate" />
            </asp:GridView>
        </div>
    </div>

    <!--公司、品牌、供应商，在保存时使用此值-->
    <div class="hidden">
        <asp:HiddenField ID="hfCompany" runat="server" />
        <asp:HiddenField ID="hfCard" runat="server" />
        <asp:HiddenField ID="hfVendor" runat="server" />
        <asp:HiddenField ID="hfallowList" runat="server" />
        <asp:HiddenField ID="hfStockList" runat="server" />
        <asp:HiddenField ID="tabStripSelect" runat="server" />
        <asp:HiddenField ID="hfDefaultDiscountList" runat="server" />
        <asp:HiddenField ID="hfDiscountList" runat="server" />
        <asp:HiddenField ID="hfTradeCodeList" runat="server" />
    </div>

    <!--补采条件-->
    <div id="bas_manual" class="hidden">
        <div class="wrapper">
            <table cellpadding="0" cellspacing="0" class="mbox_form">
                <tr>
                    <td class="field">开始时间：
                    </td>
                    <td class="layout">
                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="datepicker" Width="200px">
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="field none">结束时间：
                    </td>
                    <td class="layout none">
                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="datepicker" Width="200px">
                        </asp:TextBox>
                    </td>
                </tr>

            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
    <% if (this.CUID == Gat.Web.Core.CUID.Update)
        {%>
    <!--新增按钮-->
    <button type="button" class="ui-icon-save">新增提报方案</button>
    <!--补采-->
    <button type="button" class="ui-icon-save">补采 </button>

    <% }%>

    <button type="button" class="ui-icon-save save">保存提报方案</button>
    <%if (this.CUID == Gat.Web.Core.CUID.Update)
        { %>
    <button type="button" class="ui-icon-save" title="导出所有提报店铺到Excel">导出Excel</button>
    <%  } %>
    <asp:Button ID="btnSave" runat="server" Text="保存" CssClass="btn-normal" />
    <asp:Button ID="btnExport" runat="server" Text="导出" CssClass="btn-normal" />
    <asp:Button ID="btnSearch" runat="server" Text="查询" CssClass="btn-normal" />
</asp:Content>

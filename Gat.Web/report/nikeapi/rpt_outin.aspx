<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_outin.aspx.cs" Inherits="Gat.Web.report.nikeapi.rpt_outin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>Nike销售明细</title>
    <script src="/scripts/jquery-1.10.2.min.js"></script>
    <!--kendo-->
    <link href="/styles/kendo/2014.1.318/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.rtl.min.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/kendo/2014.1.318/kendo.web.min.js" type="text/javascript"></script>
    <script src="/scripts/kendo/2014.1.318/cultures/kendo.culture.zh-CN.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery-confirm.min.css" rel="stylesheet" />
    <script src="/scripts/jquery-confirm.min.js"></script>
    <link href="/styles/layui/css/layui.css" rel="stylesheet" />
    <script src="/styles/layui/layui.js"></script>

    <style type="text/css">
        body {
            font-size: 12px;
        }

        /* ★ 搜索区域整体样式优化 */
        .search {
            font-size: 14px;
            line-height: 30px;
            margin-bottom: 10px;
        }

            /* 统一表单控件（输入框、下拉框）的高度、字体、边框和对齐 */
            .search input[type="text"],
            .search select {
                height: 28px;
                line-height: 28px;
                font-size: 14px;
                padding: 0 6px;
                border: 1px solid #ccc;
                border-radius: 3px;
                vertical-align: middle;
                box-sizing: border-box;
            }

            /* 调整日期框宽度 */
            .search input[id$="txtStartBillDate"],
            .search input[id$="txtEndBillDate"] {
                width: 140px !important;
            }

            /* 调整普通文本框宽度 */
            .search input[id$="shopCode"],
            .search input[id$="stockCode"],
            .search input[id$="billNo"] {
                width: 200px;
            }

            /* 统一按钮样式 */
            .search button,
            .search input[type="submit"] {
                height: 28px;
                line-height: 26px;
                padding: 0 12px;
                font-size: 14px;
                vertical-align: middle;
                cursor: pointer;
                border: 1px solid #ccc;
                border-radius: 3px;
                background-color: #f8f8f8;
            }

        /* 查询按钮高亮 */
        #btnSearch {
            color: #fff;
            background-color: #009688;
            border-color: #009688;
        }

        /* 右侧统计数据样式 */
        .total-info {
            float: right;
            margin-right: 15px;
            font-size: 14px;
            font-weight: bold;
            color: #333;
            line-height: 28px;
        }

            .total-info label {
                color: #ff5722; /* 数值高亮 */
                margin-left: 5px;
            }
    </style>

    <script type="text/javascript">
        jQuery(function ($) {
            // ★ 修复：只初始化存在的日期框，去除高度限制，交由 CSS 控制
            $('#cpBody_txtStartBillDate').kendoDatePicker({
                format: 'yyyy-MM-dd 00:00:00',
                min: new Date(2000, 1, 1),
                open: function () { $('body').addClass('overflow'); },
                close: function () { $('body').removeClass('overflow'); }
            });

            $('#cpBody_txtEndBillDate').kendoDatePicker({
                format: 'yyyy-MM-dd 23:59:59',
                min: new Date(2000, 1, 1),
                open: function () { $('body').addClass('overflow'); },
                close: function () { $('body').removeClass('overflow'); }
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server"></asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div id="confirm"></div>

    <!-- 方案下拉 + 销售时间（同一行） -->
    <div class="search" style="padding-top: 10px;">
        方案：
        <asp:DropDownList ID="solution" runat="server"></asp:DropDownList>
        &nbsp;&nbsp;
        <span style="color: #FF6600; font-weight: bold;">单据时间</span>：
        <asp:TextBox ID="txtStartBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
        -
        <asp:TextBox ID="txtEndBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
    </div>

    <!-- 查询条件区域 -->
    <div class="search">
        <span>
            <%--<label>店铺代码：</label>
            <asp:TextBox ID="shopCode" runat="server"></asp:TextBox>
            &nbsp;--%>
            <label>仓库代码：</label>
            <asp:TextBox ID="stockCode" runat="server"></asp:TextBox>
            &nbsp;
            <label>条码：</label>
            <asp:TextBox ID="barCode" runat="server"></asp:TextBox>
            &nbsp;
            <label>货号：</label>
            <asp:TextBox ID="materialCode" runat="server"></asp:TextBox>
            &nbsp;
            <label>尺码：</label>
            <asp:TextBox ID="sizeCode" runat="server"></asp:TextBox>
            <label>单号：</label>
            <asp:TextBox ID="billNo" runat="server"></asp:TextBox>
            &nbsp;
            <label>上传状态：</label>
            <asp:DropDownList ID="status" runat="server">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="1">成功</asp:ListItem>
                <asp:ListItem Value="0">失败</asp:ListItem>
            </asp:DropDownList>
            &nbsp;
            <button type="button" id="btnSearch" class="ui-icon">查询</button>
            <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Text="导出数据" />

            <!-- 统计数据（修复了错别字和排版） -->
            <span class="total-info">总数量：<label id="LabelTotalQty">0</label>
            </span>
            <span class="total-info">总金额：<label id="LabelTotalAmount">0</label>
            </span>
        </span>
    </div>

    <!-- 表格区域 -->
    <div class="bas_shopright">
        <table class="layui-hide" id="grid"></table>
    </div>
    <div id="pager"></div>

    <script type="text/javascript">
        $(document).ready(function () {
            // 如果页面没有 ddlVendor，此处值为 undefined，接口需能兼容
            var vendorId = $('#ddlVendor').val();

            // 读取控件当前值（文本框 / 下拉框）
            function ctlVal(clientId) {
                var el = document.getElementById(clientId);
                return el ? el.value : '';
            }
            function ddlVal(clientId) {
                var el = document.getElementById(clientId);
                return (el && el.selectedIndex >= 0) ? el.options[el.selectedIndex].value : '';
            }

            // 统一的查询参数：首次加载、切方案、点击查询、分页都取当前界面上的实时值
            function getQueryParams() {
                return {
                    solutionId: ddlVal('<%= solution.ClientID %>'),
                    vendorId: vendorId || '',
                    startBillDate: ctlVal('<%= txtStartBillDate.ClientID %>'),
                    endBillDate: ctlVal('<%= txtEndBillDate.ClientID %>'),
                   <%-- shopCode: ctlVal('<%= shopCode.ClientID %>'),--%>
                    stockCode: ctlVal('<%= stockCode.ClientID %>'),
                    barCode: ctlVal('<%= barCode.ClientID %>'),
                    materialCode: ctlVal('<%= materialCode.ClientID %>'),
                    sizeCode: ctlVal('<%= sizeCode.ClientID %>'),
                    status: ddlVal('<%= status.ClientID %>')
                };
            }

            layui.use(['table'], function () {
                var table = layui.table;

                // 1. 初始化表格
                table.render({
                    elem: '#grid',
                    id: 'grid',
                    height: 'full-160',
                    url: window.location.origin + "/NikeApi/GatNikeApiOutInList",
                    method: "post",
                    request: { pageName: 'page', limitName: 'pageSize' },
                    where: getQueryParams(),
                    parseData: function (res) {
                        return {
                            code: 0,
                            msg: res.message,
                            count: res.totalCount,
                            data: res.data,
                            totalQty: res.qty,
                            totalAmount: res.amount
                        };
                    },
                    done: function (res) {
                        // 更新底部统计数据
                        $("#LabelTotalQty").text(res.totalQty == null ? '0' : res.totalQty);
                        $("#LabelTotalAmount").text(res.totalAmount == null ? '0' : res.totalAmount);
                    },
                    limit: 100,
                    limits: [50, 100, 500, 1000],
                    cols: [[
                        { type: 'numbers', fixed: true },
                        {
                            field: "IsSuccess", title: "成功", width: 65, align: "center",
                            templet: function (row) {
                                // 兼容 bool 类型
                                var on = (row.IsSuccess === true || row.IsSuccess === 1 || row.IsSuccess === "1" || row.IsSuccess === "true");
                                return "<input type='checkbox' disabled " + (on ? "checked" : "") + " />";
                            }
                        },

                        { field: "BillDate", title: "单据日期", width: 200 },
                        { field: "STORE_CD", title: "店铺代码", width: 120 },
                        { field: "BusinessType", title: "业务类型", width: 100 },
                        { field: "BillNo", title: "单据编号", width: 200 },
                        { field: "OrderType", title: "订单类型", width: 100 },
                        { field: "UPC", title: "条码", width: 120 },
                        { field: "PRODUCT_CD", title: "货号", width: 120 },
                        { field: "SIZE_CD", title: "尺码代码", width: 120 },
                        { field: "NET_SALES_PRC_INCL_TAX", title: "结算价", width: 100 },
                        { field: "SALES_QTY", title: "数量", width: 80 },
                        { field: "NET_SALES_AMT_INCL_TAX", title: "结算金额", width: 100 },
                        { field: "MSRP_LC", title: "零售价", width: 100 },
                        { field: "TOTAL_PARTNER_MSRP_AMT", title: "零售金额", width: 100 },
                        { field: "ModifyDtm", title: "修改时间", width: 200 },

                    ]],
                    page: true,
                    loading: true
                });

                // 日期格式化工具
                function fmtDate(v) {
                    if (!v) return '';
                    var d;
                    if (typeof v === 'string' && v.indexOf('/Date(') === 0) {
                        d = new Date(parseInt(v.replace(/\D/g, '')));
                    } else {
                        d = new Date(v);
                    }
                    if (isNaN(d.getTime())) return v;
                    var p = function (n) { return (n < 10 ? '0' : '') + n; };
                    return d.getFullYear() + '/' + p(d.getMonth() + 1) + '/' + p(d.getDate());
                }

                // 2. 异步获取方案下拉数据
                $.ajax({
                    type: "POST",
                    url: window.location.origin + "/solution/GetVendorSolution?vendorId=" + (vendorId || ''),
                    dataType: "json",
                    success: function (res) {
                        var $sel = $('#<%= solution.ClientID %>');
                        $(res).each(function (i, item) {
                            $sel.append('<option value="' + item.Id + '">' + item.CompanyName + '-' + item.CardName + '</option>');
                        });
                        // 填充完方案后重载表格，避免首屏空白（带上全部查询条件）
                        if ($sel.val()) {
                            table.reload('grid', {
                                where: getQueryParams(),
                                page: { curr: 1 }
                            });
                        }
                    }
                });

                // 3. 表格重新加载方法
                function gridReload() {
                    table.reload('grid', {
                        where: getQueryParams(),
                        page: { curr: 1 }
                    });
                }

                // 4. 绑定查询按钮（修复了只能点一次的问题）
                var btnSearchClick = false;
                $("#btnSearch").on("click", function () {
                    if (btnSearchClick) return false;
                    btnSearchClick = true;
                    try { gridReload(); } finally { btnSearchClick = false; }
                });

                // 5. 绑定重传按钮（若无此按钮则不会触发）
                var btnRepostClick = false;
                $("#btnRepost").on("click", function () {
                    if (btnRepostClick) return false;
                    btnRepostClick = true;

                    var checkStatus = table.checkStatus('grid');
                    var ids = [];
                    $(checkStatus.data).each(function (i, item) { ids.push(item.Id); });

                    var solutionId = $('#<%= solution.ClientID %>').val();

                    $.ajax({
                        url: window.location.origin + "/NikeApi/repost",
                        type: "post",
                        dataType: "json",
                        contentType: "application/json",
                        data: JSON.stringify({ "SolutionId": solutionId, "ids": ids }),
                        success: function (e) {
                            btnRepostClick = false;
                            if (e.Success) { gridReload(); alert('重传成功'); }
                            else { alert(e.Message); }
                        },
                        error: function (e) {
                            btnRepostClick = false;
                            alert(e.message || e.statusText);
                        }
                    });
                });
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server"></asp:Content>

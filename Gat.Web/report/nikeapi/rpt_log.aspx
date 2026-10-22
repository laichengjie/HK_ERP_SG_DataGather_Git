<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_log.aspx.cs" Inherits="Gat.Web.report.nikeapi.rpt_poslog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>Nike接口日志</title>
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
        body { font-size: 12px; }

        /* ★ 搜索区域整体样式优化 */
        .search {
            font-size: 14px; 
            line-height: 30px; 
            margin-bottom: 10px;
        }
        
        /* 统一表单控件的高度、字体、边框和对齐 */
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
        .search input[id$="cardShopCode"],
        .search input[id$="billNo"] {
            width: 120px;
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

        /* 重传按钮稍微突出 */
        #btnRepost {
            color: #fff;
            background-color: #ffb800;
            border-color: #ffb800;
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
    
    <!-- 方案下拉 -->
    <div class="search" style="height: 40px; padding-top: 10px;">
        <div>
            <div class="l">
                方案：
                <asp:DropDownList ID="solution" runat="server"></asp:DropDownList>
            </div>
            <br />
        </div>
    </div>
    
    <!-- 查询条件区域 -->
    <div class="search">
        <span style="color: #FF6600; font-weight: bold;">销售时间</span>：
        <asp:TextBox ID="txtStartBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
        -
        <asp:TextBox ID="txtEndBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
        &nbsp;&nbsp;
   
        <span>
            <label>店铺代码：</label>
            <asp:TextBox ID="shopCode" runat="server"></asp:TextBox>
            &nbsp;
            <label>营业员：</label>
            <asp:TextBox ID="cardShopCode" runat="server"></asp:TextBox>
            &nbsp;
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
            &nbsp;
<label>接口：</label>
<asp:DropDownList ID="InterfaceType" runat="server">
    <asp:ListItem Value="0">销售</asp:ListItem>
    <asp:ListItem Value="1">库存</asp:ListItem>
    <asp:ListItem Value="2">出入库</asp:ListItem>
</asp:DropDownList>
&nbsp;
            <button type="button" id="btnSearch" class="ui-icon">查询</button>
            <button type="button" id="btnRepost" class="ui-icon">重传</button>
            <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Text="导出数据" />

            <!-- 统计数据（修复了错别字和排版） -->
            <span class="total-info">
                总数量：<label id="LabelTotalQty">0</label>
            </span>
            <span class="total-info">
                总金额：<label id="LabelTotalAmount">0</label>
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

            layui.use(['table'], function () {
                var table = layui.table;

                // 1. 初始化表格
                table.render({
                    elem: '#grid',
                    id: 'grid',
                    height: 'full-160',
                    url: window.location.origin + "/NikeApi/GatNikeApiLoList",
                    method: "post",
                    request: { pageName: 'page', limitName: 'pageSize' },
                    where: { solutionId: '', vendorId: vendorId },
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
                        { type: 'checkbox', fixed: true },
                        {
                            field: "IsSuccess", title: "成功", width: 65, align: "center",
                            templet: function (row) {
                                // 兼容 bool 类型
                                var on = (row.IsSuccess === true || row.IsSuccess === 1 || row.IsSuccess === "1" || row.IsSuccess === "true");
                                return "<input type='checkbox' disabled " + (on ? "checked" : "") + " />";
                            }
                        },
                        { field: "Id", title: "Id", width: 80 },
                        { field: "BillDate", title: "单据日期", width: 200   },
                        { field: "BillTypeName", title: "单据类型", width: 130 },
                        { field: "BillNo", title: "单号", width: 200 },
                        { field: "ShopCode", title: "店铺代码", width: 120 },
                        { field: "StockCode", title: "仓库代码", width: 120 },
                        { field: "Qty", title: "数量", width: 80 },
                        { field: "Amount", title: "金额", width: 80 },
                        { field: "PostJaon", title: "发送数据", width: 300 },
                        { field: "ReturnJson", title: "返回数据", width: 300 },
                        { field: "ModifyDtm", title: "最后修改时间", width: 200 },
                        
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
                        // 填充完方案后重载表格，避免首屏空白
                        if ($sel.val()) {
                            table.reload('grid', {
                                where: { solutionId: $sel.val(), vendorId: vendorId },
                                page: { curr: 1 }
                            });
                        }
                    }
                });

                // 3. 表格重新加载方法
                function gridReload() {
                    var val = function (clientId) {
                        var el = document.getElementById(clientId);
                        return el ? el.value : '';
                    };
                    var ddlVal = function (clientId) {
                        var el = document.getElementById(clientId);
                        return el ? el.options[el.selectedIndex].value : '';
                    };

                    table.reload('grid', {
                        where: {
                            solutionId: ddlVal('<%= solution.ClientID %>'),
                            startBillDate: val('<%= txtStartBillDate.ClientID %>'),
                            endBillDate: val('<%= txtEndBillDate.ClientID %>'),
                            shopCode: val('<%= shopCode.ClientID %>'),
                            cardShopCode: val('<%= cardShopCode.ClientID %>'),
                            billNo: val('<%= billNo.ClientID %>'),
                            status: ddlVal('<%= status.ClientID %>'),
                            interfacetype: ddlVal('<%= InterfaceType.ClientID %>'),
                            vendorId: vendorId
                        },
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

                // 5. 绑定重传按钮（修复了 var 遮蔽导致的防抖失效）
                var btnRepostClick = false;
                $("#btnRepost").on("click", function () {
                    if (btnRepostClick) return false;
                    
                    var checkStatus = table.checkStatus('grid');
                    var ids = [];
                    $(checkStatus.data).each(function (i, item) { ids.push(item.Id); });

                    if (ids.length === 0) {
                        alert('请先勾选需要重传的数据');
                        return false;
                    }

                    btnRepostClick = true;

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
<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="PosSale.aspx.cs" Inherits="Gat.Web.report.crocs.PosSale" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>
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
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div id="confirm"></div>
    <div class="search" style="height: 40px;">
        <div>
            <div class="l">
                方案：
                <%--<asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>--%>
                <select lay-filter="solution" id="solution" name="solution">
                </select>
            </div>

            <br />
        </div>
    </div>
    <div class="search">
        <label>店铺代码：</label><input type="text" id="shopCode" />
        <label>Sku：</label><input type="text" id="skuCode" />
        <button type="button" id="btnSearch" class="ui-icon">查询</button>
    </div>
    <div class="bas_shopright">
        <table class="layui-hide" id="grid"></table>
    </div>
    <div id="pager"></div>
    <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<label id="LabelTotalQty"></label></div>
        <div style="float: right; margin-right: 20px;">总金额;<label id="LabelTotalAmount"></label></div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {

            var vendorId = $('#ddlVendor').val();
            $.ajax({
                type: "POST",
                url: window.location.origin + "/solution/GetVendorSolution?vendorId=" + vendorId,
                dataType: "json",
                success: function (res) {
                    $(res).each(function (i, item) {
                        $('#solution').append('<option value="' + item.Id + '">' + item.CompanyName + '-' + item.CardName + '</option>');
                    });
                }
            })

            //layui.use('laydate', function () {
            //    var laydate = layui.laydate;

            //    laydate.render({
            //        elem: '#txtStartBillDate'
            //    });
            //    laydate.render({
            //        elem: '#txtEndBillDate'
            //    });
            //});

            layui.use('table', function () {
                var solutionId = $("#solution").val();
                var table = layui.table;
                table.render({
                    elem: '#grid',
                    height: 500,
                    url: window.location.origin + "/crocs/GetInventoryPageList?solutionId=" + solutionId + '&vendorId=' + vendorId,
                    request: {
                        pageName: 'page' //页码的参数名称，默认：page
                        , limitName: 'pageSize'//每页数据量的参数名，默认：limit
                    },
                    parseData: function (res) {
                        //console.log(res);
                        return {
                            "code": 0, //解析接口状态
                            "msg": res.message, //解析提示文本
                            "count": res.totalCount, //解析数据长度
                            "data": res.data, //解析数据列表
                            "totalQty": res.qty,
                            "totalAmount": res.amount
                        };
                    },
                    done: function (res, curr, count) {
                        //如果是异步请求数据方式，res即为你接口返回的信息。
                        //如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度
                        $("#LabelTotalQty").text(res.totalQty);
                        $("#LabelTotalAmount").text(res.totalAmount);
                    },
                    limit: 100,
                    limits: [50, 100, 500, 1000],
                    cols: [[
                        { type: 'numbers', fixed: true },
                        { field: "DIScode", title: "经销商编码", width: 120 },
                        { field: "Doorcode", title: "门店编码", width: 120 },
                        { field: "TransactionID", title: "单号编码", width: 120 },
                        { field: "SKUcode", title: "款式+颜色", width: 200 },
                        { field: "Size", title: "尺寸", width: 120 },
                        { field: "InventoryDate", title: "库存日期", width: 130, type: "date", format: "{0:yyyy/MM/dd}" },
                        { field: "ReceivedQty", title: "进货数据", width: 120 },
                        { field: "Qty", title: "库存数量", width: 120 },
                        { field: "MSRP", title: "库存吊牌金额", width: 120 },
                        { field: "Updatetime", title: "数据更新时间", width: 130, type: "date", format: "{0:yyyy/MM/dd}" },
                    ]]
                    , page: true,
                    loading: true
                });

                /**
                 * 表格带参数重新加载
                 * */
                function gridReload() {
                    var shopCode = $("#shopCode").val();
                    var skuCode = $("#skuCode").val();
                    var solutionId = $("#solution").val();
                    var status = $('#status').val();
                    table.reload('grid', {
                        where: { //设定异步数据接口的额外参数
                            "solutionId": solutionId,
                            "skuCode": skuCode,
                            "shopCode": shopCode
                        }
                        , page: {
                            curr: 1 //重新从第 1 页开始
                        }
                    }); //只重载数据
                }

                var btnSearchClick = false;
                /**
                 * 查询
                 */
                $("#btnSearch").bind("click dbclick", function (e) {
                    if (btnSearchClick) {
                        return false;
                    }
                    btnSearchClick = true;
                    gridReload();
                    var btnSearchClick = false;
                });

            });
            //供应商下拉列表
            var $ddl = $("select[id$='cpBody_ddlSoule']");
            $ddl.kendoDropDownList({
                change: function () {
                    var value = $ddl.data('kendoDropDownList').value();

                    window.location.href = "/report/adidas/transfer?id=" + value
                }
            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

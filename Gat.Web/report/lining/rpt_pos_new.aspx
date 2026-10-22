<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_pos_new.aspx.cs" Inherits="Gat.Web.report.lining.rpt_pos_new" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>李宁销售报表</title>
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
        <label>导入日期：</label>
        <span style="margin-left: 15px;">
            <input type="text" id="txtStartBillTime" class="datepicker" /></span>
        -
        <span>
            <input type="text" id="txtEndBillTime" class="datepicker" /></span>
        <label>销售日期：</label>
        <span style="margin-left: 15px;">
            <input type="text" id="txtStartBillDate" class="datepicker" /></span>
        -
        <span>
            <input type="text" id="txtEndBillDate" class="datepicker" /></span>

        <label>店铺代码：</label><input type="text" id="txtShopCode2" />
        <label>货号：</label><input type="text" id="txtMaterialCode" />
        <label>尺码：</label><input type="text" id="txtSizeName" />
        <button type="button" id="btnSearch" class="ui-icon">查询</button>
        <button type="button" id="btnDelSearch" class="ui-icon">删除时间段</button>
        <button type="button" id="benDelChecked" class="ui-icon">删除选中</button>
        <button type="button" id="btnExport" class="ui-icon">导出数据</button>
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

            layui.use('laydate', function () {
                var laydate = layui.laydate;

                laydate.render({
                    elem: '#txtStartBillTime'
                });
                laydate.render({
                    elem: '#txtEndBillTime'
                });
                laydate.render({
                    elem: '#txtStartBillDate'
                });
                laydate.render({
                    elem: '#txtEndBillDate'
                });
            });

            layui.use('table', function () {
                var solutionId = $("#solution").val();
                var table = layui.table;
                table.render({
                    elem: '#grid',
                    id: 'grid',
                    height: 'full-160',
                    url: window.location.origin + "/lining/GetPageList",
                    method: "post",
                    request:{
                        pageName: 'page', //页码的参数名称，默认：page
                        limitName: 'pageSize'//每页数据量的参数名，默认：limit
                    },
                    where: {
                        solutionId: solutionId,
                        vendorId: vendorId,
                    },
                    contentType:"application/json;charset=utf-8",
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
                        { type: 'checkbox' },
                        { field: "Id", hide: true },
                        { field: "ShopCode", title: "门店代码", width: 90 },
                        {
                            field: "ShopName", title: "门店名称", width: 160 },
                        { field: "BillNo", title: "交易单号", width: 165 },
                        { field: "BillDate", title: "交易日期", width:165, type: "date", format: "{0:yyyy/MM/dd}" },
                        { field: "MaterialCode", title: "商品编码", width: 112 },
                        { field: "SizeName", title: "产品尺码", width: 86 },
                        { field: "BarCode", title: "条码", width: 135 },
                        { field: "BalaPrice", title: "价格", width: 80 },
                        { field: "SaleAmount", title: "销售金额", width: 100 },
                        { field: "Qty", title: "销售数量", width: 100 },
                        { field: "PersonelName", title: "销售人员名称", width: 115 },
                        { field: "SaleType", title: "销售类型", width: 100 },
                        { field: "MemberMobile", title: "会员手机号", width: 120 },
                    ]]
                    , page: true,
                    loading: true
                });

                /**
                 * 表格带参数重新加载
                 * */
                function gridReload() {
                    var startBillTime = $("#txtStartBillTime").val();
                    var endBillTime = $("#txtEndBillTime").val();
                    var startBillDate = $("#txtStartBillDate").val();
                    var endBillDate = $("#txtEndBillDate").val();
                    var shopCode = $("#txtShopCode2").val();
                    var solutionId = $("#solution").val();
                    var materialCode = $("#txtMaterialCode").val();
                    var sizeName = $("#txtSizeName").val();
                    table.reload('grid', {
                        where: { //设定异步数据接口的额外参数
                            "solutionId": solutionId,
                            "startBillTime": startBillTime,
                            "endBillTime": endBillTime,
                            "startBillDate": startBillDate,
                            "endBillDate": endBillDate,
                            "shopCode": shopCode,
                            "materialCode": materialCode,
                            "sizeName": sizeName
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
                    btnSearchClick = false;
                });

                var btnDelSearch = false;
                /**
                 * 删除查询范围内数据
                 */
                $("#btnDelSearch").bind("click dbclick", function (e) {

                    if (btnDelSearch) {
                        return false;
                    }
                    btnDelSearch = true;
                    var startBillTime = $("#txtStartBillTime").val();
                    var endBillTime = $("#txtEndBillTime").val();
                    var startBillDate = $("#txtStartBillDate").val();
                    var endBillDate = $("#txtEndBillDate").val();
                    var shopCode = $("#txtShopCode2").val();
                    var solutionId = $("#solution").val();
                    var materialCode = $("#txtMaterialCode").val();
                    var sizeName = $("#txtSizeCode").val();
                    var data = {
                        "startBillTime": startBillTime,
                        "endBillTime": endBillTime,
                        "startBillDate": startBillDate,
                        "endBillDate": endBillDate,
                        "shopCode": shopCode,
                        "solutionId": solutionId,
                        "materialCode": materialCode,
                        "sizeName": sizeName
                    };
                    //调用接口删除数据
                    $.ajax({
                        url: window.location.origin + "/lining/DeleteSearch",
                        type: "post",
                        dataType: "json",
                        contentType:"application/json",
                        data: JSON.stringify(data),
                        success: function (e) {
                            if (e.Success) {
                                gridReload();
                            }
                        },
                        error: function (e) { alert(e.message) }
                    });

                    btnDelSearch = false;
                });

                var benDelChecked = false;
                /**
                 * 删除勾选数据
                 */
                $("#benDelChecked").bind("click dbclick", function (e) {

                    if (benDelChecked) {
                        return false;
                    }
                    benDelChecked = true;

                    //调用接口删除数据
                    let checkStatus = table.checkStatus('grid');

                    console.log(checkStatus.data);
                    let ids = [];
                    $(checkStatus.data).each(function (i, item) {
                        ids.push(item.Id);
                    });
                    $.ajax({
                        url: window.location.origin + "/lining/DeleteById",
                        type: "post",
                        dataType: "json",
                        contentType: "application/json",
                        data: JSON.stringify(ids),
                        success: function (e) {
                            if (e.Success) {
                                gridReload();
                            }
                        },
                        error: function (e) { alert(e.message) }
                    });

                    gridReload();
                    benDelChecked = false;
                });


                var btnExport = false;
                /**
                 * 导出
                 */
                $("#btnExport").bind("click dbclick", function (e) {

                    if (btnExport) {
                        return false;
                    }
                    btnExport = true;
                    try {
                        var startBillTime = $("#txtStartBillTime").val();
                        var endBillTime = $("#txtEndBillTime").val();
                        var startBillDate = $("#txtStartBillDate").val();
                        var endBillDate = $("#txtEndBillDate").val();
                        var shopCode = $("#txtShopCode2").val();
                        var solutionId = $("#solution").val();
                        var materialCode = $("#txtMaterialCode").val();
                        var sizeName = $("#txtSizeCode").val();
                        var url = window.location.origin + "/lining/ExportExcel";
                        var solutionName = $("#solution option:selected").text();
                        var xhr = new XMLHttpRequest();
                        xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
                        xhr.responseType = "blob";  // 返回类型blob
                        xhr.setRequestHeader('content-type', 'application/json');
                        // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
                        xhr.onload = function () {
                            // 请求完成
                            if (this.status === 200) {
                                // 返回200
                                let blob = this.response;
                                let reader = new FileReader();
                                reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a标签href
                                reader.onload = function (e) {
                                    // 转换完成，创建一个a标签用于下载
                                    let a = document.createElement('a');
                                    let nowDate = new Date();
                                    a.download = solutionName + nowDate.toLocaleDateString() + '提报.xlsx';
                                    a.href = e.target.result;
                                    $("body").append(a);  // 修复firefox中无法触发click
                                    a.click();
                                    $(a).remove();
                                }
                            }
                        };
                        let data = {
                            "startBillTime": startBillTime,
                            "endBillTime": endBillTime,
                            "startBillDate": startBillDate,
                            "endBillDate": endBillDate,
                            "shopCode": shopCode,
                            "solutionId": solutionId,
                            "materialCode": materialCode,
                            "sizeName": sizeName
                        }
                        xhr.send(JSON.stringify(data));
                    } catch (e) {
                        alert(e.message);
                    }

                    btnExport = false;
                });
            });
            //供应商下拉列表
            var $ddl = $("select[id$='cpBody_ddlSoule']");
            $ddl.kendoDropDownList({
                change: function () {
                    let value = $ddl.data('kendoDropDownList').value();

                    window.location.href = "/report/adidas/transfer?id=" + value
                }
            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

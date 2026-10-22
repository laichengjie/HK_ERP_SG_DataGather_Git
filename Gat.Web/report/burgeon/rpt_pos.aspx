<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.burgeon.rpt_pos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>伯俊Pos销售明细</title>
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
    <script type="text/javascript">
        jQuery(function ($) {

            $('#cpBody_txtStartBillDate').css('width', '150px')
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

            $('#cpBody_txtEndBillDate').css('width', '150px')
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
    </script>

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
                <asp:DropDownList ID="solution" runat="server">
                </asp:DropDownList>
                <%--     <select lay-filter="solution" id="solution" name="solution">
                </select>--%>
            </div>

            <br />
        </div>
    </div>
    <div class="search">
        <span style="color: #FF6600; font-weight: bold;">销售时间</span>：
                 <asp:TextBox ID="txtStartBillDate" runat="server" CssClass="datepicker" Height="15px"></asp:TextBox>
        -
                 <asp:TextBox ID="txtEndBillDate" runat="server" CssClass="datepicker" Height="15px"></asp:TextBox>

        &nbsp;&nbsp;
   
        <span>
            <%--<input type="text" id="txtEndBillDate" class="datepicker" /></span>--%>
            <%--<label>店铺代码：</label><input type="text" id="shopCode" />--%>
            <%--<label>品牌店铺代码：</label><input type="text" id="cardShopCode" />--%>
            <%--<label>单号：</label><input type="text" id="billNo" />--%>

            <label>店铺代码：</label>
            <asp:TextBox ID="shopCode" runat="server" CssClass="datepicker"></asp:TextBox>
            <label>条码：</label>
            <asp:TextBox ID="cardShopCode" runat="server" CssClass="datepicker"></asp:TextBox>
            <label>单号：</label>
            <asp:TextBox ID="billNo" runat="server" CssClass="datepicker"></asp:TextBox>


            <label>上传状态：</label>
            <%-- <select lay-filter="status" id="status" name="status">
                <option vlaue=""></option>
                <option value="1">成功</option>
                <option value="0">失败</option>
            </select>--%>
            <asp:DropDownList ID="status" runat="server">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="1">成功</asp:ListItem>
                <asp:ListItem Value="0">失败</asp:ListItem>
            </asp:DropDownList>

            <button type="button" id="btnSearch" class="ui-icon">查询</button>
            <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />

            总数量:<label id="LabelTotalQty"></label>
            总金额;<label id="LabelTotalAmount"></label>


        </span>
    </div>
    <div class="bas_shopright">
        <table class="layui-hide" id="grid"></table>
    </div>
    <div id="pager"></div>
  <%--  <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<label id="LabelTotalQty"></label></div>
        <div style="float: right; margin-right: 20px;">总金额;<label id="LabelTotalAmount"></label></div>
    </div>--%>


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
                    elem: '#txtStartBillDate'
                });
                laydate.render({
                    elem: '#txtEndBillDate'
                });
            });

            layui.use('table', function () {
                var solutionId = $("#cpBody_solution").val();
                console.log(solutionId);
                var table = layui.table;
                table.render({
                    elem: '#grid',
                    id: 'grid',
                    height: 'full-160',
                    url: window.location.origin + "/Burgeon/GetPosDetailList",
                    method: "post",
                    request: {
                        pageName: 'page' //页码的参数名称，默认：page
                        , limitName: 'pageSize'//每页数据量的参数名，默认：limit
                    },
                    where: {
                        solutionId: solutionId,
                        vendorId: vendorId,
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
                        { field: "IsSuccess", title: "是否推送", width: 100 },
                        { field: "BillDate", title: "销售日期", width: 200, type: "date", format: "{0:yyyy/MM/dd}" },
                        { field: "ShopCode", title: "店铺代码", width: 120 },
                        { field: "Salesrep", title: "营业员", width: 120 },
                        { field: "BillNo", title: "单据编号", width: 200 },
                        { field: "MaterialCode", title: "货号", width: 120 },
                        { field: "BarCode", title: "条码", width: 120 },
                        { field: "Type", title: "销售类型", width: 80 },
                        { field: "Qty", title: "数量", width: 100 },
                        { field: "BalaPrice", title: "成交单价", width: 100 },
                        { field: "PayAmount", title: "成交金额", width: 100 },
                    ]]
                    , page: true,
                    loading: true
                });

                /**
                 * 表格带参数重新加载
                 * */
                function gridReload() {
                    /* var startBillDate = $("#txtStartBillDate").val();*/
                    /* var endBillDate = $("#txtEndBillDate").val();*/
                    /* var shopCode = $("#shopCode").val();*/
                    /*  var cardShopCode = $("#cardShopCode").val();*/
                    /* var billNo = $("#billNo").val();*/
                    /*  var solutionId = $("#solution").val();*/
                    /* var status = $('#status').val();*/

                    var startBillDate = document.getElementById('<%= txtStartBillDate.ClientID %>')?.value;
                    var endBillDate = document.getElementById('<%= txtEndBillDate.ClientID %>')?.value;
                    var shopCode = document.getElementById('<%= shopCode.ClientID %>')?.value;
                    var cardShopCode = document.getElementById('<%= cardShopCode.ClientID %>')?.value;
                    var billNo = document.getElementById('<%= billNo.ClientID %>')?.value;

                    var dropdown = document.getElementById('<%= status.ClientID %>');
                    var statusValue = dropdown.options[dropdown.selectedIndex].value;

                    var solutionIddropdown = document.getElementById('<%= solution.ClientID %>');
                    var solutionId = solutionIddropdown.options[solutionIddropdown.selectedIndex].value;

                    console.log(solutionId)


                    table.reload('grid', {
                        where: { //设定异步数据接口的额外参数
                            "solutionId": solutionId,
                            "startBillDate": startBillDate,
                            "endBillDate": endBillDate,
                            "shopCode": shopCode,
                            "cardShopCode": cardShopCode,
                            "billNo": billNo,
                            "status": statusValue
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

                var btnRepostClick = false;

                $("#btnRepost").bind("click dbclick", function (e) {

                    if (btnRepostClick) {
                        return false;
                    }
                    btnRepostClick = true;

                    //调用接口删除数据
                    let checkStatus = table.checkStatus('grid');

                    console.log(checkStatus.data);
                    let ids = [];
                    $(checkStatus.data).each(function (i, item) {
                        ids.push(item.Id);
                    });
                    /* */

                    var solutionId = $("#cpBody_solution").val();

                    $.ajax({
                        url: window.location.origin + "/Burgeon/repost",
                        type: "post",
                        dataType: "json",
                        contentType: "application/json",
                        data: JSON.stringify({ "SolutionId": solutionId, "ids": ids }),
                        success: function (e) {
                            btnRepostClick = false;
                            if (e.Success) {
                                gridReload();
                                alert('重传成功');
                            } else {
                                alert(e.Message);
                            }
                        },
                        error: function (e) {
                            btnRepostClick = false;
                            alert(e.message);
                        }
                    });
                    btnRepostClick = false;
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

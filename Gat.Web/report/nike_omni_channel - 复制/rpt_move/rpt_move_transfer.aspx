<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_move_transfer.aspx.cs" Inherits="Gat.Web.report.nike_omni_channel.rpt_move.rpt_move_transfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>>Nike全渠道-销售中转数据</title>
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
                <asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>
                <button type="button" id="SaveSeleted" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">选择保存</span></button>
                <button type="button" id="saveAll" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部保存</span></button>
                <button type="button" id="delALL" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部刪除</span></button>
                <button type="button" id="delChoosed" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">选择刪除</span></button>
            </div>
            <div class="r">
                <%--<asp:FileUpload ID="FileUpload" runat="server" CssClass="ui-icon" />--%>
                <input type="file" name="fileName" id="fileName" />
                <asp:Button ID="btnImport" runat="server" Text="导入明细" CssClass="ui-icon" OnClientClick="return false" />
                <asp:Button ID="btnExport" runat="server" Text="导出文件数据格式" CssClass="ui-icon" OnClientClick="return false" />
            </div>

            <br />
        </div>
    </div>
    <div class="search">
        <label>单据生效日期：</label>
        <span style="margin-left: 15px;">
            <input type="text" id="txtStartBillDate" class="datepicker" /></span>
        -
         <span>
             <input type="text" id="txtEndBillDate" class="datepicker" /></span>
        <label>调出仓代码：</label><input type="text" id="OutStockCode" />
        <label>调入仓代码：</label><input type="text" id="InStockCode" />
        <label>货号：</label><input type="text" id="ad_trans_materialCode" />
        <label>尺码代码：</label><input type="text" id="ad_trans_sizeCode" />
        <button type="button" id="btnSearch" class="ui-icon">查询</button>
        <button type="button" id="btnDelSearch" class="ui-icon">删除时间段数据</button>
    </div>

    <div class="bas_shopright">
        <table class="layui-hide" id="grid"></table>
    </div>
    <div id="pager"></div>
    <div id="pager1">
        <div style="float: right; margin-right: 60px;">调出数量:<label id="LabelTotalOutQty"></label></div>
        <div style="float: right; margin-right: 60px;">调入数量:<label id="LabelTotalinQty"></label></div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
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
                var table = layui.table;
                table.render({
                    elem: '#grid',
                    height: 500,
                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=SEARCH",
                    request: {
                        pageName: 'page' //页码的参数名称，默认：page
                        , limitName: 'pageSize'//每页数据量的参数名，默认：limit
                    },
                    parseData: function (res) {
                        //console.log(res);
                        return {
                            "code": 0, //解析接口状态
                            "msg": res.message, //解析提示文本
                            "count": res.list.totalCount, //解析数据长度
                            "data": res.list.data, //解析数据列表
                            "totalOutQty": res.list.totalOutQty,
                            "totalInQty": res.list.totalInQty
                        };
                    },
                    done: function (res, curr, count) {
                        //如果是异步请求数据方式，res即为你接口返回的信息。
                        //如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度
                        $("#LabelTotalOutQty").text(res.totalOutQty);
                        $("#LabelTotalinQty").text(res.totalInQty);
                    },
                    limit: 100,
                    limits: [50, 100, 500, 1000],
                    cols: [[
                        { type: 'checkbox', fixed: true },
                        { type: 'numbers', fixed: true },
                        { field: "TransID", title: "序号", width: 30, hide: true },
                        { field: "BillDate", title: "单据生效日期", width: 200, type: "date", format: "{0:yyyy/MM/dd}" },
                        { field: "BillNo", title: "单号", width: 200 },
                        { field: "OutStockCode", title: "调出仓代码", width: 120 },
                        { field: "InStockCode", title: "调入仓代码", width: 120 },
                        { field: "MaterialCode", title: "货号", width: 120 },
                        { field: "BarCode", title: "条码", width: 80 },
                        { field: "SizeCode", title: "尺码代码", width: 100 },
                        { field: "SizeName", title: "尺码名称", width: 100 },
                        { field: "Qty", title: "数量", width: 100 },
                        { field: "StockTransferStatus", title: "调拨状态", width: 120 },
                        { field: "MovementType", title: "货物移动类型", width: 120 },
                        { field: "BillTypeName", title: "单据类型名称", width: 200 },
                    ]]
                    , page: true,
                    loading: true
                });
                /**
                 * 表格带参数重新加载
                 * */
                function gridReload() {
                    var startBillDate = $("#txtStartBillDate").val();
                    var endBillDate = $("#txtEndBillDate").val();
                    var outStockCode = $("#OutStockCode").val();
                    var inStockCode = $("#InStockCode").val();
                    var materialCode = $("#ad_trans_materialCode").val();
                    var sizeCode = $("#ad_trans_sizeCode").val();
                    var solutionId = $("#cpBody_ddlSoule").val();
                    table.reload('grid', {
                        where: { //设定异步数据接口的额外参数，任意设
                            "solutionId": solutionId,
                            "startBillDate": startBillDate,
                            "endBillDate": endBillDate,
                            "outStockCode": outStockCode,
                            "inStockCode": inStockCode,
                            "materialCode": materialCode,
                            "sizeCode": sizeCode
                        }
                        , page: {
                            curr: 1 //重新从第 1 页开始
                        }
                    }); //只重载数据
                }


                var importClick = false;
                /**
                 * 导入
                 */
                $("#cpBody_btnImport").bind("click dbclick", function (e) {
                    if (importClick) {
                        return false;
                    }
                    importClick = true;
                    var $file1 = $("input[name='fileName']").val();//用户文件内容(文件)
                    // 判断文件是否为空 
                    if ($file1 == "") {
                        alert("请选择上传的目标文件! ")
                        return false;
                    }
                    //判断文件类型,我这里根据业务需求判断的是Excel文件
                    var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
                    if (fileName1 != "xls" && fileName1 != "xlsx") {
                        alert("请选择Execl文件!");
                        return false;
                    }
                    //判断文件大小
                    var size1 = $("input[name='fileName']")[0].files[0].size;
                    if (size1 > 104857600) {
                        alert("上传文件不能大于100M!");
                        return false;
                    }

                    boo1 = true;
                    var type = "file";
                    var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
                    formData.append(type, $("#fileName")[0].files[0]);
                    var solutionId = $("#cpBody_ddlSoule").val();
                    $.ajax({
                        url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=Import&solutionId=" + solutionId,
                        type: "POST",
                        data: formData,
                        contentType: false,
                        processData: false,
                        datatype: "text",
                        success: function (data) {
                            alert(data);
                            if (data.lastIndexOf("成功")) {
                                gridReload();
                            }
                        },
                        error: function (e) {
                            alert(e.message);
                        }
                    });
                    importClick = false;
                });


                var saveAllClick = false;
                /**
                 * 全部保存
                 */
                $("#saveAll").bind("click dbclick", function (e) {
                    if (saveAllClick) {
                        return false;
                    }
                    saveAllClick = true;

                    $.confirm({
                        useBootstrap: false,
                        title: '提示',
                        content: '是否确定要保存全部数据至销售数据?',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var solutionId = $("#cpBody_ddlSoule").val();
                                var postData = {
                                    "solutionId": solutionId,
                                };
                                $.ajax({
                                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=TRANSFER_SAVEALL",
                                    type: "POST",
                                    data: postData,
                                    datatype: "json",
                                    success: function (data) {
                                        gridReload();
                                    },
                                    error: function (e) {
                                        alert(e.message);
                                    }
                                });
                            },
                            '取消': function () {
                                return true;
                            }
                        }
                    });
                    saveAllClick = false;
                });

                var delALLClick = false;
                /**
                 * 全部删除
                 */
                $("#delALL").bind("click dbclick", function (e) {
                    if (delALLClick) {
                        return false;
                    }
                    delALLClick = true;

                    $.confirm({
                        useBootstrap: false,
                        title: '提示',
                        content: '是否确定要删除全部数据?',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var solutionId = $("#cpBody_ddlSoule").val();
                                var postData = {
                                    "solutionId": solutionId,
                                };
                                $.ajax({
                                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=TRANSFER_DELALL",
                                    type: "POST",
                                    data: postData,
                                    datatype: "json",
                                    success: function (data) {
                                        gridReload();
                                    },
                                    error: function (e) {
                                        alert(e.message);
                                    }
                                });
                            },
                            '取消': function () {
                                return true;
                            }
                        }
                    });
                    delALLClick = false;
                });

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

                var btnDelSearchClick = false;
                /**
                 * 删除查询结果
                 */
                $("#btnDelSearch").bind("click dbclick", function (e) {
                    if (btnDelSearchClick) {
                        return false;
                    }
                    btnDelSearchClick = true;
                    $.confirm({
                        useBootstrap: false,
                        title: '提示',
                        content: '是否确定要删除符合查询条件的数据?',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var solutionId = $("#cpBody_ddlSoule").val();
                                var startBillDate = document.getElementById("txtStartBillDate").value;
                                var endBillDate = document.getElementById("txtEndBillDate").value;
                                var postData = {
                                    "solutionId": solutionId,
                                    "startBillDate": startBillDate,
                                    "endBillDate": endBillDate
                                };
                                $.ajax({
                                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=DELETESEARCH",
                                    type: "POST",
                                    data: postData,
                                    datatype: "json",
                                    success: function (data) {
                                        gridReload();
                                    },
                                    error: function (e) {
                                        alert(e.message);
                                    }
                                });
                            },
                            '取消': function () {
                                return true;
                            }
                        }
                    });
                    btnDelSearchClick = false;
                });


                /**
                 * 选择保存
                 */
                $('#SaveSeleted').click(function (e) {
                    var selectedData = table.checkStatus('grid');
                    var selectedIds = [];

                    $(selectedData.data).each(function (i, item) {
                        selectedIds.push(item.TransID);
                    });

                    if (selectedIds.length === 0) {
                        $.alert({
                            useBootstrap: false,
                            title: '提示',
                            content: '请选择要保存的数据！',
                            boxWidth: '30%'
                        });
                        return false;
                    }

                    $.confirm({
                        useBootstrap: false,
                        title: '保存确认',
                        content: '是否确定要保存选中的 <strong style="color:blue;">' + selectedIds.length + '</strong> 条数据?',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var postData = {
                                    selectedIds: selectedIds
                                };
                                $.ajax({
                                    cache: false,
                                    type: "POST",
                                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=transfer_Save",
                                    data: postData,
                                    complete: function (data) {
                                        //reload grid
                                        gridReload();
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(thrownError);
                                    },
                                    traditional: true
                                });
                            },
                            '取消': function () {
                                return true;
                            }
                        }
                    });
                    return false;
                });

                /**
                 * 选择刪除
                 */
                $('#delChoosed').click(function (e) {
                    var selectedData = table.checkStatus('grid');
                    var selectedIds = [];

                    $(selectedData.data).each(function (i, item) {
                        selectedIds.push(item.TransID);
                    });

                    if (selectedIds.length === 0) {
                        $.alert({
                            useBootstrap: false,
                            title: '提示',
                            content: '请选择要删除的数据！',
                            boxWidth: '30%'
                        });
                        return false;
                    }

                    $.confirm({
                        useBootstrap: false,
                        title: '删除确认',
                        content: '是否确定要删除选中的 <strong style="color:red;">' + selectedIds.length + '</strong> 条数据?<br/><span style="color:red;">此操作不可恢复！</span>',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var postData = {
                                    selectedIds: selectedIds
                                };
                                $.ajax({
                                    cache: false,
                                    type: "POST",
                                    url: "/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=TRANSFER_DELSELECT",
                                    data: postData,
                                    complete: function (data) {
                                        gridReload();
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(thrownError);
                                    },
                                    traditional: true
                                });
                            },
                            '取消': function () {
                                return true;
                            }
                        }
                    });
                    return false;
                });
            });


            var exportClick = false;
            /**
             * 导出
             */
            $("#cpBody_btnExport").bind("click dbclick", function (e) {
                if (exportClick) {
                    return false;
                }
                exportClick = true;
                try {
                    var url = '/report/nike_omni_channel/rpt_move/rpt_move_transfer.aspx?mode=Export';
                    var xhr = new XMLHttpRequest();
                    xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
                    xhr.responseType = "blob";  // 返回类型blob
                    // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
                    xhr.onload = function () {
                        // 请求完成
                        if (this.status === 200) {
                            // 返回200
                            var blob = this.response;
                            var reader = new FileReader();
                            reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                            reader.onload = function (e) {
                                // 转换完成，创建一个a标签用于下载
                                var a = document.createElement('a');
                                a.download = 'Nike全渠道调拨单导入格式.xlsx';
                                a.href = e.target.result;
                                $("body").append(a);  // 修复firefox中无法触发click
                                a.click();
                                $(a).remove();
                            }
                        }
                    };
                    xhr.send();
                } catch (e) {
                    alert(e.message);
                }
                exportClick = false;
            });
            //供应商下拉列表
            var $ddl = $("select[id$='cpBody_ddlSoule']");
            $ddl.kendoDropDownList({
                change: function () {
                    var value = $ddl.data('kendoDropDownList').value();

                    window.location.href = "report/adoc/postransfer?id=" + value
                }
            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

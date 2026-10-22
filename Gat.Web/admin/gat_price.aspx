
<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_price.aspx.cs" Inherits="Gat.Web.admin.gat_price" EnableEventValidation="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>货号结算价设置</title>
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
                <%--方案：
                <asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>--%>
                <%--<button type="button" id="saveAll" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部转上传</span></button>--%>
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
        
        <label>公司代码：</label><input type="text" id="CompanyID"/>
        <label>货号：</label><input type="text" id="MaterialCode"/>
        <button type="button" id="btnSearch" class="ui-icon">查询</button>
        <%--<button type="button" id="btnDelSearch" class="ui-icon">删除时间段数据</button>--%>
    </div>

    <div class="bas_shopright">
        <table class="layui-hide" id="grid"></table>
    </div>
    <div id="pager"></div>
    <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<label id="LabelTotalQty"></label></div>
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
                    height: 700,
                    url: "/admin/gat_price.aspx?mode=SEARCH",
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
                            "totalQty": res.list.qty,
                            "totalAmount":res.list.amount
                        };
                    },
                    done: function (res, curr, count) {
                        //如果是异步请求数据方式，res即为你接口返回的信息。
                        //如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度
                        $("#LabelTotalQty").text(res.totalQty);
                      
                    },
                    limit: 100,
                    limits: [50, 100, 500, 1000],
                    cols: [[
                        { type: 'checkbox', fixed: true },
                        { type: 'numbers', fixed: true },
                        { field: "TransID", title: "序号", width: 30, hide: true },
                        { field: "CompanyID", title: "公司代码", width: 150 },
                        { field: "MaterialCode", title: "货号", width: 150 },
                        { field: "BalaPrice", title: "结算价", width: 80 },
                        { field: "ModifyDtm", title: "修改时间", width: 200, type: "date", format: "{0:yyyy/MM/dd hh:MM:ss}" }
                    ]]
                    , page: true,
                    loading: true
                });

                /**
                 * 表格带参数重新加载
                 * */
                function gridReload() {
                    var materialCode = $("#MaterialCode").val();
                    var companyID = $("#CompanyID").val();
                  
                    table.reload('grid', {
                        where: { //设定异步数据接口的额外参数，任意设
                            "companyID": companyID,
                            "materialCode": materialCode
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
                    
                    $.ajax({
                        url: "/admin/gat_price.aspx?mode=Import",
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
                        content: '是否确定要全部数据转上传吗?',
                        boxWidth: '30%',
                        buttons: {
                            '确定': function () {
                                var solutionId = $("#cpBody_ddlSoule").val();
                                var postData = {
                                    "solutionId": solutionId,
                                };
                                $.ajax({
                                    url: "/admin/gat_price.aspx?mode=TRANSFER_SAVEALL",
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
                                    url: "/admin/gat_price.aspx?mode=TRANSFER_DELALL",
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
                                    url: "/admin/gat_price.aspx?mode=DELETESEARCH",
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
                    if (selectedIds.length > 0) {
                        var postData = {
                            selectedIds: selectedIds
                        };

                        $.ajax({
                            cache: false,
                            type: "POST",
                            url: "/admin/gat_price.aspx?mode=transfer_Save",
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
                        return false;
                    }
                });

                /**
                 * 选择刪除
                 */
                $('#delChoosed').click(function (e) {
                    var selectedIds = [];
                    var selectedData = table.checkStatus('grid');
                    var selectedIds = [];
                    $(selectedData.data).each(function (i, item) {
                        selectedIds.push(item.TransID);
                    });
                    if (selectedIds.length > 0) {
                        var postData = {
                            selectedIds: selectedIds
                        };
                        $.ajax({
                            cache: false,
                            type: "POST",
                            url: "/admin/gat_price.aspx?mode=TRANSFER_DELSELECT",
                            data: postData,
                            complete: function (data) {
                                gridReload();
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(thrownError);
                            },
                            traditional: true
                        });
                        return false;
                    }
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
                    var url = '/admin/gat_price.aspx?mode=Export';
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
                                a.download = '提报.xlsx';
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

                    window.location.href = "/admin/transfer?id=" + value
                }
            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

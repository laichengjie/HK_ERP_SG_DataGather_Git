<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_transfer.aspx.cs" Inherits="Gat.Web.report.columbia.rpt_transfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>
    <script src="/scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <!--kendo-->
    <link href="/styles/kendo/2014.1.318/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.rtl.min.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/kendo/2014.1.318/kendo.web.min.js" type="text/javascript"></script>
    <script src="/scripts/kendo/2014.1.318/cultures/kendo.culture.zh-CN.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        jQuery(function ($) {
            $('input.datepicker')
                .eq(0)
                .css('width', '115px')
                .end()
                .bind('focus', function () {
                    $(this).css('border', 'none')
                }).kendoDatePicker({
                    format: 'yyyy-MM-dd',// HH:mm:ss
                    min: new Date(2000, 01, 01),
                    open: function () {
                        $(document.body).addClass('overflow');
                    },
                    close: function () {
                        $(document.body).removeClass('overflow');
                    }
                });

            var exportDataClick = false;
            $("#btnExportData").bind("click dbclick", function (e) {
                if (exportDataClick) {
                    return false;
                }
                exportDataClick = true;
                try {
                    var startDate = $("#cpHeaderNav_txtStarDate").val() || '';
                    var endDate = $("#cpHeaderNav_txtEndDate").val() || '';
                    var solutionId = $("#cpBody_ddlSoule").val() || '';
                    var shopName = $("#cpHeaderNav_txtShopName").val() || '';
                    var solutionName = $("#cpBody_ddlSoule option[selected='selected']").html() || '';
                    var url = '/report/columbia/rpt_transfer.aspx?mode=ExportData&solutionId=' + solutionId + '&startDate=' + startDate + '&endDate=' + endDate + '&shopName=' + shopName;
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
                                a.download = solutionName + startDate + '-' + endDate + shopName + '提报.xlsx';
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
                exportDataClick = false;
            });
        });


    </script>
    <style type="text/css">
        .k-picker-wrap {
            width: 300px;
        }

        .k-picker-wrap {
            display: inline;
        }
        /*datepicker*/
        .k-timepicker,
        .k-datetimepicker,
        .k-datepicker {
            border: 0 none;
            white-space: nowrap;
            width: 115px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="l">
        <span style="color: #FF6600; font-weight: bold;">店铺名称</span>：
              <asp:TextBox ID="txtShopName" runat="server"></asp:TextBox>
        <span style="color: #FF6600; font-weight: bold;">日期</span>：
                            <asp:TextBox ID="txtStarDate" runat="server" CssClass="datepicker"></asp:TextBox>
        &nbsp; &nbsp;
            -
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker"></asp:TextBox>
        &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;  
        <span>

            <input type="button" value="查询" id="bthSearch" />
            <%--  <asp:Button ID="bthSearch" runat="server" Text="查询" />--%>
        </span>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">

    <div class="search">
        <div class="l">
            方案：
                <asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>
            <button type="button" id="SaveSeleted" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">保存选择</span></button>
            <asp:Button ID="DeleteBut" runat="server" Text="全部删除" CssClass="ui-icon-save" />
            <asp:Button ID="SaveBut" runat="server" Text="保存全部" CssClass="ui-icon-save" />
        </div>
        <div class="r">
            <button type="button" id="btnExportData" class="ui-button">导出数据</button>
            <asp:FileUpload ID="FileUpload" runat="server" CssClass="ui-icon-save" />
            <asp:Button ID="btnImport" runat="server" Text="导入明细" CssClass="ui-icon-save" />
            <asp:Button ID="btnExport" runat="server" Text="导出文件数据格式" CssClass="ui-icon-save" />

        </div>
        <br />
    </div>

    <div class="bas_shopright">
        <div class="gv-wrapper" id="gv-shop">

            <table class="adminContent" style="width: 100%">
                <tr>
                    <td>
                        <div id="products-grid"></div>

                        <script type="text/javascript">
                            $(document).ready(function () {

                                $("#products-grid").kendoGrid({
                                    dataSource: {
                                        async: false,
                                        type: "json",
                                        transport: {
                                            read: {
                                                url: window.location.href,
                                                type: "POST",
                                                dataType: "json",
                                                data: addPostData
                                            },
                                            update: {
                                                url: "/report/columbia/rpt_transfer.aspx?mode=transfer_Update",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            destroy: {
                                                url: "/report/columbia/rpt_transfer.aspx?mode=transfer_Delete",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            parameterMap: function (data, operation) {
                                                console.info(data);
                                                if (operation != "read") {
                                                    var result = {};
                                                    var mr = "";

                                                    for (var i = 0; i < data.models.length; i++) {
                                                        var entity = data.models[i];
                                                        if (operation == "update") {
                                                            mr += entity["TransID"] + ',' + entity["Qty"] + '|'
                                                        }
                                                        if (operation == "destroy") {
                                                            mr += entity["TransID"] + ','
                                                        }
                                                        //for (var member in entity) {
                                                        //       result["trans[" + i + "]." + member] = entity[member];
                                                        //}
                                                    }
                                                    result.list = mr;
                                                    //console.info(result);
                                                    return result;
                                                } else {
                                                    return data;
                                                }
                                                return data;
                                            }
                                        },

                                        schema: {
                                            data: "Data",
                                            total: "Total",
                                            errors: "Errors",
                                            model: {
                                                id: "TransID",
                                                fields: {
                                                    TransID: { editable: false, type: "number" },
                                                    CompanyName: { editable: false, type: "string" },
                                                    ShopCode: { editable: false, type: "string" },
                                                    ShopName: { editable: false, type: "string" },
                                                    StockName: { editable: false, type: "string" },
                                                    CardName: { editable: false, type: "string" },
                                                    BillNo: { editable: false, type: "string" },
                                                    BillDate: { editable: false, type: "date" },
                                                    MaterialCode: { editable: false, type: "string" },
                                                    SizeName: { editable: false, type: "string" },
                                                    BarCode: { editable: false, type: "string" },
                                                    SalesAmount: { editable: false, type: "number" },
                                                    Qty: { editable: true, type: "number" },
                                                    PersonnelID: { editable: false, type: "string" },
                                                    Remark: { editable: false, type: "string" }
                                                }

                                            }


                                        },
                                        aggregate: [
                                            { field: "SalesAmount", aggregate: "sum" },
                                            { field: "Qty", aggregate: "sum" }],

                                        requestEnd: function (e) {
                                            if (e.type == "update") {
                                                this.read();

                                            }
                                        },
                                        error: function (e) {
                                            //display_kendoui_grid_error(e);
                                            this.cancelChanges();
                                        },
                                        batch: true,
                                        pageSize: 15,
                                        serverPaging: true,
                                        serverFiltering: true,
                                        serverSorting: true
                                    },
                                    pageable: {
                                        refresh: true,
                                        pageSizes: [10, 15, 20, 50, 100]
                                    },
                                    toolbar: [{ name: "save", text: "保存" }, { name: "cancel", text: "取消" }],
                                    editable: {
                                        confirmation: false,
                                        //bulk editing
                                        mode: "incell"
                                    },
                                    scrollable: false,
                                    dataBound: onDataBound,
                                    columns: [
                                        {
                                            field: "TransID",
                                            headerTemplate: "<input id='mastercheckbox' type='checkbox'/>",
                                            headerAttributes: { style: "text-align:center" },
                                            attributes: { style: "text-align:center" },
                                            template: "<input type='checkbox' value='#=TransID#' class='checkboxGroups'/>",
                                            width: 30
                                        },
                                        {
                                            field: "TransID",
                                            title: "序号",
                                            width: 30
                                        },
                                        {
                                            field: "CompanyName",
                                            title: "公司名称",
                                            width: 100
                                        },
                                        {
                                            field: "ShopCode",
                                            title: "店铺代码",
                                            width: 120
                                        },
                                        {
                                            field: "CardName",
                                            title: "经营品牌",
                                            width: 150
                                        },
                                        {
                                            field: "BillNo",
                                            title: "交易编号",
                                            width: 200
                                        },
                                        {
                                            field: "BillDate",
                                            title: "交易时间",
                                            width: 60,
                                            type: "date",
                                            format: "{0:yyyy/MM/dd HH:mm:ss}"

                                        },
                                        {
                                            field: "MaterialCode",
                                            title: "产品编号",
                                            width: 120
                                        },
                                        {
                                            field: "SizeName",
                                            title: "规格",
                                            width: 80
                                        },
                                        {
                                            field: "BarCode",
                                            title: "产品条码",
                                            width: 80
                                        },
                                        {
                                            field: "SalesAmount",
                                            title: "销售金额",
                                            width: 80,
                                            aggregates: ["sum"], footerTemplate: "总金额: #=sum#"
                                        },
                                        {
                                            field: "Qty",
                                            title: "销售数量",
                                            width: 60,
                                            aggregates: ["sum"], footerTemplate: "总数量: #=sum#",
                                            editor: function (container, options) {
                                                $('<input name="' + options.field + '"/>').appendTo(container).kendoNumericTextBox({
                                                    format: "#",
                                                    decimals: 0
                                                });
                                            }
                                        },
                                        {
                                            field: "PersonnelID",
                                            title: "销售员编号",
                                            width: 80
                                        },
                                        {
                                            field: "Remark",
                                            title: "备注",
                                            width: 80
                                        },
                                        {
                                            command: {
                                                name: "destroy",
                                                text: "删除"
                                            },
                                            title: "操作",
                                            width: 100
                                        }]

                                });
                                $('#bthSearch').click(function () {
                                    var grid = $('#products-grid').data('kendoGrid');
                                    grid.dataSource.page(1);
                                    return false;

                                    //$.ajax({
                                    //    cache: false,
                                    //    type: "POST",
                                    //    url: "/report/columbia/rpt_transfer.aspx?mode=transfer_List",
                                    //    data: postData,
                                    //    complete: function (data) {

                                    //        var grid = $('#products-grid').data('kendoGrid');
                                    //        grid.dataSource.page(1);
                                    //        return false;

                                    //    },
                                    //    error: function (xhr, ajaxOptions, thrownError) {
                                    //        alert(thrownError);
                                    //    },
                                    //    traditional: true
                                    //});

                                });
                                //保存提报方案
                                $('#SaveSeleted').click(function (e) {
                                    e.preventDefault();
                                    var selectedIds = [];
                                    $('#products-grid input[type=checkbox][id!=mastercheckbox]').each(function () {
                                        if ($(this).is(":checked") == true) {
                                            var checked = jQuery.inArray($(this).val(), selectedIds);
                                            if (checked == -1) {
                                                selectedIds.push($(this).val());
                                            }
                                        }
                                    });
                                    if (selectedIds.length > 0) {
                                        var postData = {
                                            selectedIds: selectedIds
                                        };
                                        console.info(postData);

                                        $.ajax({
                                            cache: false,
                                            type: "POST",
                                            url: "/report/columbia/rpt_transfer.aspx?mode=transfer_Save",
                                            data: postData,
                                            complete: function (data) {
                                                //reload grid
                                                var grid = $('#products-grid').data('kendoGrid');
                                                grid.dataSource.read();
                                            },
                                            error: function (xhr, ajaxOptions, thrownError) {
                                                alert(thrownError);
                                            },
                                            traditional: true
                                        });
                                        return false;
                                    }
                                });

                                $('#mastercheckbox').click(function () {
                                    var ischecked = $(this).is(":checked");
                                    $('.checkboxGroups').each(function () {
                                        $(this).prop("checked", ischecked);
                                    })
                                });

                                ////供应商下拉列表
                                var $ddl = $("select[id$='cpBody_ddlSoule']");
                                $ddl.kendoDropDownList({
                                    change: function () {
                                        var value = $ddl.data('kendoDropDownList').value();

                                        window.location.href = "/report/columbia/transfer?id=" + value
                                        // window.location.href = '/report/columbia/pos/' + pageclickednumber+ "?shopcode=" + shopCode;
                                    }
                                });


                            });

                            function onDataBound(e) {
                                updateMasterCheckbox();
                            }

                            function updateMasterCheckbox() {
                                var numChkBoxes = $('#products-grid input[type=checkbox][id!=mastercheckbox]').length;
                                var numChkBoxesChecked = $('#products-grid input[type=checkbox][checked][id!=mastercheckbox]').length;
                                $('#mastercheckbox').attr('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
                            }
                            function addPostData() {
                                var strat = $("#<%=txtStarDate.ClientID%>").val();
                                var end = $("#<%=txtEndDate.ClientID%>").val();
                                var shopName = $("#<%=txtShopName.ClientID%>").val();

                                return {
                                    strat: strat,
                                    end: end,
                                    shopName: shopName,
                                    mode: 'transfer_Reader'
                                };
                            }
                        </script>
                    </td>
                </tr>

            </table>

        </div>
    </div>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

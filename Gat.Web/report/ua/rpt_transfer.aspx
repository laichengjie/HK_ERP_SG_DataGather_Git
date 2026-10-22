<%@ Page Title="" Language="C#" MasterPageFile="~/report/Reportkon.Master" AutoEventWireup="true" CodeBehind="rpt_transfer.aspx.cs" Inherits="Gat.Web.report.ua.rpt_transfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>
    <script type="text/javascript" src="/scripts/jquery-1.10.2.min.js"></script>
    <!--kendo-->
    <link href="/styles/kendo/2014.1.318/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="/styles/kendo/2014.1.318/kendo.rtl.min.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/kendo/2014.1.318/kendo.web.min.js" type="text/javascript"></script>
    <script src="/scripts/kendo/2014.1.318/cultures/kendo.culture.zh-CN.min.js" type="text/javascript"></script>

    <style type="text/css">
        .k-picker-wrap {
            width: 300px;
        }

        .k-picker-wrap {
            display: inline;
        }

        ul.toptab {
            list-style: none;
            padding: 0 20px;
            margin: 0;
            float: left;
            background: #222;
            font-size: 1.2em;
            background: white;
            text-align: center;
            height: 100%;
        }

            ul.toptab li {
                float: left;
                width: 80px;
                height: 70%;
                margin: 0;
                position: relative;
                padding: 5px 0 0 0;
            }

                ul.toptab li a {
                    color: black;
                    display: block;
                    float: left;
                    width: 100%;
                    height: 80%;
                    text-decoration: none;
                    outline: none;
                }

                    ul.toptab li a:hover {
                        background: #199ED8;
                    }

        .bg {
            background: #199ED8;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div style="height: 40px;">
        <ul class="toptab">
            <li>
                <a class="bg" id="pos_sale" href="javascript:void(0)">销售</a>
            </li>
            <li>
                <a id="stock" href="javascript:void(0)">库存</a>
            </li>
        </ul>
    </div>
    <div class="search">
        <div class="l">
            方案：
                <asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>
            <button type="button" id="SaveSeleted" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">选择保存</span></button>
            <button type="button" id="SeletedDelete" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">选择刪除</span></button>
            <button type="button" id="SaveAll" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部保存</span></button>
             <button type="button" id="AllDelate" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部刪除</span></button>
        </div>
        <div class="r">
            <%--<asp:FileUpload ID="FileUpload" runat="server" CssClass="ui-icon-save" />--%>
            <input type="file" name="fileName" id="fileName" />
            <asp:Button ID="btnImport" runat="server" Text="导入明细" CssClass="ui-icon-save" OnClientClick="return false" />
            <asp:Button ID="btnExport" runat="server" Text="导出文件数据格式" CssClass="ui-icon-save" OnClientClick="return false" />
        </div>

        <br />
    </div>

    <div class="bas_shopright pos_sale">
        <div class="gv-wrapper" id="gv-shop">

            <table class="adminContent" style="width: 100%">
                <tr>
                    <td>
                        <div id="sale-grid"></div>

                        <script type="text/javascript">
                            function loadPosSale() {
                                $("#sale-grid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: {
                                                url: window.location.href,
                                                type: "POST",
                                                dataType: "json",
                                                data: {
                                                    mode: 'transfer_Reader'
                                                }
                                            },
                                            update: {
                                                url: "/report/ua/rpt_transfer.aspx?mode=transfer_Update",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            destroy: {
                                                url: "/report/ua/rpt_transfer.aspx?mode=transfer_Delete",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            parameterMap: function (data, operation) {
                                                //console.info(data);
                                                if (operation != "read") {
                                                    var result = { transferType: "sale" };
                                                    var mr = "";

                                                    for (var i = 0; i < data.models.length; i++) {
                                                        var entity = data.models[i];
                                                        if (operation == "update") {
                                                            mr += entity["ID"] + ',' + entity["Qty"] + '|'
                                                        }
                                                        if (operation == "destroy") {
                                                            mr += entity["ID"] + ','
                                                        }
                                                        //for (var member in entity) {
                                                        //       result["trans[" + i + "]." + member] = entity[member];
                                                        //}
                                                    }
                                                    result.list = mr;
                                                    //console.info(result);
                                                    return result;
                                                } else {
                                                    // var entit;
                                                    //entity["page"]
                                                    //console.info(operation["mode"]);
                                                    data.transferType = "sale";
                                                    return data;
                                                    // return { page: operation.page, pageSize: operation.pageSize };
                                                }
                                                //return data;
                                                // return { page: operation.page, pageSize: operation.pageSize };
                                            }
                                        },

                                        schema: {
                                            data: "Data",
                                            total: "Total",
                                            errors: "Errors",
                                            model: {
                                                id: "ID",
                                                fields: {
                                                    ID: { editable: false, type: "number" },
                                                    Sequence: { editable: false, type: "string" },
                                                    CompanyName: { editable: false, type: "string" },
                                                    ShopCode: { editable: false, type: "string" },
                                                    ShopName: { editable: false, type: "string" },
                                                    CardName: { editable: false, type: "string" },
                                                    BillCode: { editable: false, type: "string" },
                                                    BillTime: { editable: false, type: "date" },
                                                    MaterialCode: { editable: false, type: "string" },
                                                    BarCode: { editable: false, type: "string" },
                                                    //SizeCode: { editable: false, type: "string" },
                                                    SizeName: { editable: false, type: "string" },
                                                    Amount: { editable: false, type: "number" },
                                                    Qty: { editable: true, type: "number" },
                                                    Price: { editable: false, type: "number" },
                                                    BalaPrice: { editable: false, type: "number" },
                                                    PromotionName: { editable: false, type: "string" },
                                                    PersonnelName: { editable: false, type: "string" }
                                                }
                                            }
                                        },
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
                                        pageSize: 20,
                                        serverPaging: true,
                                        serverFiltering: true,
                                        serverSorting: true
                                    },
                                    pageable: {
                                        refresh: true,
                                        pageSizes: [20,50, 100, 200,500, 1000]
                                    },
                                    editable: {
                                        confirmation: false,
                                        //bulk editing
                                        mode: "incell"
                                    },
                                    scrollable: false,
                                  /*  toolbar: [{ name: "save", text: "保存" }, { name: "cancel", text: "取消" }],*/
                                    dataBound: onDataBound,
                                    columns: [
                                        {
                                            field: "ID",
                                            headerTemplate: "<input id='mastercheckbox-sale' type='checkbox'/>",
                                            headerAttributes: { style: "text-align:center" },
                                            attributes: { style: "text-align:center" },
                                            template: "<input type='checkbox' value='#=ID#' class='checkboxGroups checkboxGroups-sale'/>",
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
                                            field: "ShopName",
                                            title: "店铺名称",
                                            width: 120
                                        },
                                        {
                                            field: "CardName",
                                            title: "经营品牌",
                                            width: 150
                                        },
                                        {
                                            field: "BillCode",
                                            title: "单号",
                                            width: 200
                                        },
                                        {
                                            field: "Sequence",
                                            title: "行号",
                                            width: 30
                                        },
                                        {
                                            field: "BillTime",
                                            title: "交易时间",
                                            width: 60,
                                            type: "date",
                                            format: "{0:yyyy/MM/dd hh:MM:ss}"
                                        },
                                        {
                                            field: "BarCode",
                                            title: "条码",
                                            width: 80
                                        },
                                        //{
                                        //    field: "SizeCode",
                                        //    title: "尺码编码",
                                        //    width: 80
                                        //},
                                        {
                                            field: "SizeName",
                                            title: "尺码名称",
                                            width: 80
                                        },
                                        {
                                            field: "MaterialCode",
                                            title: "SKU",
                                            width: 120
                                        },
                                        {
                                            field: "Amount",
                                            title: "销售金额",
                                            width: 80
                                        },
                                        {
                                            field: "Qty",
                                            title: "销售数量",
                                            width: 60,
                                            editor: function (container, options) {
                                                $('<input name="' + options.field + '"/>').appendTo(container).kendoNumericTextBox({
                                                    format: "#",
                                                    decimals: 0
                                                });
                                            }
                                        },
                                        {
                                            field: "BalaPrice",
                                            title: "价格",
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
                            }
                        </script>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div class="bas_shopright stock" style="display: none;">
        <div class="gv-wrapper" id="gv-stock">

            <table class="adminContent" style="width: 100%">
                <tr>
                    <td>
                        <div id="stock-grid"></div>

                        <script type="text/javascript">
                            function loadStock() {

                                $("#stock-grid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: {
                                                url: window.location.href,
                                                type: "POST",
                                                dataType: "json",
                                                data: {
                                                    mode: 'transfer_Reader'
                                                }
                                            },
                                            update: {
                                                url: "/report/ua/rpt_transfer.aspx?mode=transfer_Update",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            destroy: {
                                                url: "/report/ua/rpt_transfer.aspx?mode=transfer_Delete",
                                                type: "POST",
                                                dataType: "json"
                                            },
                                            parameterMap: function (data, operation) {
                                                //console.info(data);
                                                if (operation != "read") {
                                                    var result = { transferType: "stock" };
                                                    var mr = "";

                                                    for (var i = 0; i < data.models.length; i++) {
                                                        var entity = data.models[i];
                                                        if (operation == "update") {
                                                            mr += entity["ID"] + ',' + entity["ActualQty"] + ',' + entity["PathQty"] + '|'
                                                        }
                                                        if (operation == "destroy") {
                                                            mr += entity["ID"] + ','
                                                        }
                                                        //for (var member in entity) {
                                                        //       result["trans[" + i + "]." + member] = entity[member];
                                                        //}
                                                    }
                                                    result.list = mr;
                                                    //console.info(result);
                                                    return result;
                                                } else {
                                                    // var entit;
                                                    //entity["page"]
                                                    //console.info(operation["mode"]);
                                                    data.transferType = "stock";
                                                    return data;
                                                    // return { page: operation.page, pageSize: operation.pageSize };
                                                }
                                                //return data;
                                                // return { page: operation.page, pageSize: operation.pageSize };
                                            }
                                        },

                                        schema: {
                                            data: "Data",
                                            total: "Total",
                                            errors: "Errors",
                                            model: {
                                                id: "ID",
                                                fields: {
                                                    ID: { editable: false, type: "number" },
                                                    CompanyName: { editable: false, type: "string" },
                                                    ShopCode: { editable: false, type: "string" },
                                                    ShopName: { editable: false, type: "string" },
                                                    UpdateDate: { editable: false, type: "date" },
                                                    MaterialCode: { editable: false, type: "string" },
                                                    BarCode: { editable: false, type: "string" },
                                                    SizeName: { editable: false, type: "string" },
                                                    ActualQty: { editable: true, type: "number" },
                                                    PathQty: { editable: true, type: "number" },
                                                }
                                            }
                                        },
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
                                        pageSize: 20,
                                        serverPaging: true,
                                        serverFiltering: true,
                                        serverSorting: true
                                    },
                                    pageable: {
                                        refresh: true,
                                        pageSizes: [20, 50, 100, 200, 500, 1000]
                                    },
                                    editable: {
                                        confirmation: false,
                                        //bulk editing
                                        mode: "incell"
                                    },
                                    scrollable: false,
                                    toolbar: [{ name: "save", text: "保存" }, { name: "cancel", text: "取消" }],
                                    dataBound: onDataBound,
                                    columns: [
                                        {
                                            field: "ID",
                                            headerTemplate: "<input id='mastercheckbox-stock' type='checkbox'/>",
                                            headerAttributes: { style: "text-align:center" },
                                            attributes: { style: "text-align:center" },
                                            template: "<input type='checkbox' value='#=ID#' class='checkboxGroups checkboxGroups-stock'/>",
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
                                            field: "ShopName",
                                            title: "店铺名称",
                                            width: 120
                                        },
                                        {
                                            field: "BarCode",
                                            title: "条码",
                                            width: 120
                                        },
                                        {
                                            field: "SizeName",
                                            title: "尺码",
                                            width: 120
                                        },
                                        {
                                            field: "MaterialCode",
                                            title: "SKU",
                                            width: 120
                                        },
                                        {
                                            field: "ActualQty",
                                            title: "库存数量",
                                            width: 60,
                                            editor: function (container, options) {
                                                $('<input name="' + options.field + '"/>').appendTo(container).kendoNumericTextBox({
                                                    format: "#",
                                                    decimals: 0
                                                });
                                            }
                                        },
                                        {
                                            field: "PathQty",
                                            title: "在途数量",
                                            width: 60,
                                            editor: function (container, options) {
                                                $('<input name="' + options.field + '"/>').appendTo(container).kendoNumericTextBox({
                                                    format: "#",
                                                    decimals: 0
                                                });
                                            }
                                        },
                                        {
                                            field: "UpdateDate",
                                            title: "库存日期",
                                            width: 120,
                                            type: "date",
                                            format: "{0:yyyy/MM/dd hh:MM:ss}"
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
                            }


                        </script>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="pager"></div>
    <div id="pager1">
        <div style="float: right; margin-right: 60px;">总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
        <div style="float: right; margin-right: 20px;">总金额;<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></div>
    </div>
    <script type="text/javascript">

        var transferType = "sale";
        $(document).ready(function () {
            $('ul.toptab li a').click(function (e) {
                var self = this;
                var id = self.id;
                $('ul.toptab li a.bg')[0].classList.remove("bg");
                this.classList.add("bg");
                if (id == "pos_sale") {
                    $('.pos_sale').css('display', '');
                    $('.stock').css('display', 'none');
                    loadPosSale();
                    bindSaleCheckBox();
                } else {
                    $('.pos_sale').css('display', 'none');
                    $('.stock').css('display', '');
                    transferType = "stock";
                    loadStock();
                    bindStockCheckBox();
                }
                $.ajax({
                    cache: false,
                    type: "POST",
                    url: "/report/ua/rpt_transfer.aspx?mode=changeCommandArgument",
                    data: { "transferType": transferType },
                });
                return false;
            });
            //保存提报方案
            $('#SaveSeleted').click(function (e) {
                e.preventDefault();
             if (!confirm("是否确定要选择保存数据？")) { return false; }
                var selectedIds = [];
                $('#' + transferType + '-grid input[type=checkbox][id!=mastercheckbox]').each(function () {
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
                        url: "/report/ua/rpt_transfer.aspx?mode=transfer_Save&transferType=" + transferType,
                        data: postData,
                        complete: function (data) {
                            //reload grid
                            var grid = $('#' + transferType + '-grid').data('kendoGrid');
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
             //保存全部
            $('#SaveAll').click(function (e) {
                e.preventDefault();

                   if (!confirm("是否确定要全部保存数据？")) { return false; }

                    $.ajax({
                        cache: false,
                        type: "POST",
                        url: "/report/ua/rpt_transfer.aspx?mode=transfer_SaveAll&transferType=" + transferType,
                        data: null,
                        complete: function (data) {
                            //reload grid
                            var grid = $('#' + transferType + '-grid').data('kendoGrid');
                            grid.dataSource.read();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(thrownError);
                        },
                        traditional: true
                    });
                    return false;
                
            });
            $('#SeletedDelete').click(function (e) {
                e.preventDefault();
                 if (!confirm("是否确定要选择删除数据？此操作不可恢复！")) { return false; }
                var selectedIds = [];
                $('#' + transferType + '-grid input[type=checkbox][id!=mastercheckbox]').each(function () {
                    if ($(this).is(":checked") == true) {
                        var selectedId = $(this).val();

                        var checked = jQuery.inArray(selectedId, selectedIds);
                        if (checked == -1 && selectedId != "on") {
                            selectedIds.push(selectedId);
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
                        url: "/report/ua/rpt_transfer.aspx?mode=transfer_ALLdel&transferType=" + transferType,
                        data: postData,
                        complete: function (data) {
                            //reload grid
                            var grid = $('#' + transferType + '-grid').data('kendoGrid');
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

          $('#AllDelate').click(function (e) {
                e.preventDefault();
                 if (!confirm("是否确定要全部删除数据？此操作不可恢复！")) { return false; }
                var selectedIds = [];
                selectedIds.push('C');
                if (selectedIds.length > 0) {
                    var postData = {
                        selectedIds: selectedIds
                    };
                    console.info(postData);

                    $.ajax({
                        cache: false,
                        type: "POST",
                        url: "/report/ua/rpt_transfer.aspx?mode=transfer_ALLdel&transferType=" + transferType,
                        data: postData,
                        complete: function (data) {
                            //reload grid
                            var grid = $('#' + transferType + '-grid').data('kendoGrid');
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

            $("#cpBody_btnImport").click(function (e) {
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
                var solutionId = $("#cpBody_ddlSoule").val() || '';
                $.ajax({
                    url: "/report/ua/rpt_transfer.aspx?mode=Import&transferType=" + transferType + "&solutionId=" + solutionId,
                    type: "POST",
                    data: formData,
                    contentType: false,
                    processData: false,
                    datatype: "text",
                    success: function (data) {
                        var grid = $('#' + transferType + '-grid').data('kendoGrid');
                        grid.dataSource.read();
                        alert(data);
                    },
                    error: function (e) {
                        alert(e.message);
                    }
                });
            });

            $("#cpBody_btnExport").click(function (e) {
                //$.ajax({
                //    url: "/report/ua/rpt_transfer.aspx?mode=Export",
                //    type: "POST",
                //    data: { "transferType": transferType },
                //    dataType:"application/json;charset=utf-8",
                //    success: function (data) {
                //        var blob = this.response;
                //        var a = document.createElement('a');
                //        var url = window.URL.createObjectURL(blob);
                //        a.href = url;
                //        //设置文件名称
                //        a.download = '导出.xls';
                //        a.click();
                //    },
                //    error: function (e) {
                //        alert(e.message);
                //    }
                //});
                var url = '/report/ua/rpt_transfer.aspx?mode=Export&transferType=' + transferType;
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
                            var transferTypeName = transferType == "sale" ? "销售" : "库存";
                            a.download = transferTypeName + '提报.xlsx';
                            a.href = e.target.result;
                            $("body").append(a);  // 修复firefox中无法触发click
                            a.click();
                            $(a).remove();
                        }
                    }
                };
                xhr.send();
            });


            loadPosSale();

            bindSaleCheckBox();

            ////供应商下拉列表
            var $ddl = $("select[id$='cpBody_ddlSoule']");
            $ddl.kendoDropDownList({
                change: function () {
                    var value = $ddl.data('kendoDropDownList').value();

                    window.location.href = "/report/ua/transfer?id=" + value
                    // window.location.href = '/report/columbia/pos/' + pageclickednumber+ "?shopcode=" + shopCode;
                }
            });
        });

        function bindSaleCheckBox() {
            $('#mastercheckbox-sale').click(function () {
                var ischecked = $(this).is(":checked");
                $('.checkboxGroups-sale').each(function () {
                    $(this).prop("checked", ischecked);
                })
            });
        }

        function bindStockCheckBox() {
            $('#mastercheckbox-stock').click(function () {
                var ischecked = $(this).is(":checked");
                $('.checkboxGroups-stock').each(function () {
                    $(this).prop("checked", ischecked);
                })
            });
        }

        function onDataBound(e) {
            updateMasterCheckbox();
        }

        function updateMasterCheckbox() {
            if (transferType == "sale") {
                var numChkBoxes = $('#sale-grid input[type=checkbox][id!=mastercheckbox-sale]').length;
                var numChkBoxesChecked = $('#sale-grid input[type=checkbox][checked][id!=mastercheckbox-sale]').length;
                $('#mastercheckbox-sale').attr('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
            } else {
                var numChkBoxes = $('#stock-grid input[type=checkbox][id!=mastercheckbox-stock]').length;
                var numChkBoxesChecked = $('#stock-grid input[type=checkbox][checked][id!=mastercheckbox-stock]').length;
                $('#mastercheckbox-stock').attr('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

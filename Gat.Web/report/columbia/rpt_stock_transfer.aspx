<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_stock_transfer.aspx.cs" Inherits="Gat.Web.report.columbia.rpt_stock_transfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存中转数据</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function () {
                var url = '/report/columbia/transfer/stock/1';
                url = url + '?shopcode=' + encodeURI($shop.val());
                location.href = url;
            });
        });

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/columbia/transfer/stock/' + pageclickednumber + "?shopcode=" + encodeURI('<%=shopCode %>');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server" />
            <a href="javascript:;" hidefocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div>
        方案：
                <asp:DropDownList ID="ddlSoule" runat="server">
                </asp:DropDownList>
        <input type="file" name="fileName" id="fileName" />
        <asp:Button ID="btnImport" runat="server" Text="导入" OnClientClick="return false" />
        <asp:Button ID="btnExportFile" runat="server" Text="导出文件数据格式" OnClientClick="return false" />
        <asp:Button ID="btnDeleteAll" runat="server" Text="删除全部"  OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');"  />
        <asp:Button ID="btnSaveToMaterialInv" runat="server" Text="保存数据" OnClientClick="return false" />
    </div>
    <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview">
        <Columns>
            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="店铺名称" DataField="ShopName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="产品编码" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="规格" DataField="SizeName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="产品条码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="库存数量" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="库存日期" DataField="UpdateDate" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:yyyy-MM-dd}" />

        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到库存中转记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
    <div style="float: right; margin-right: 60px;">总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
    <script type="text/javascript">
        $(document).ready(function () {

            function pageReload() {
                var url = '/report/columbia/transfer/stock/1';
                url = url + '?shopcode=';
                location.href = url;
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
                    url: "/report/columbia/rpt_stock_transfer.aspx?method=Import",
                    type: "POST",
                    data: formData,
                    contentType: false,
                    processData: false,
                    datatype: "text",
                    success: function (data) {
                        alert(data);
                        if (data.lastIndexOf("成功")) {
                            pageReload();
                        }
                    },
                    error: function (e) {
                        alert(e.message);
                    }
                });
                importClick = false;
            });

            var delALLClick = false;
            /**
             * 全部删除
             */
            $("#cpBody_btnDeleteAll").bind("click dbclick", function (e) {
                if (delALLClick) {
                    return false;
                }
                delALLClick = true;
                var delConfirm = confirm("确定删除全部库存中转数据?");
                if (delConfirm == true) {
                    $.ajax({
                        url: "/report/columbia/rpt_stock_transfer.aspx?method=DELETEALL",
                        type: "POST",
                        datatype: "json",
                        success: function (data) {
                            var result = eval('(' + data + ')');
                            alert(result.Html);
                            pageReload();
                        },
                        error: function (e) {
                            alert(e.message);
                        }
                    });
                }
                delALLClick = false;
            });

            var saveToMaterialInvClick = false;
            /**
             * 保存数据到库存表
             */
            $("#cpBody_btnSaveToMaterialInv").bind("click dbclick", function (e) {
                if (saveToMaterialInvClick) {
                    return false;
                }
                saveToMaterialInvClick = true;
                var delConfirm = confirm("确定保存全部库存中转数据至库存报表?");
                if (delConfirm == true) {
                    var solutionId = $("#cpBody_ddlSoule").val();
                    var postData = {
                        "solutionId": solutionId,
                    };
                    $.ajax({
                        url: "/report/columbia/rpt_stock_transfer.aspx?method=SAVETOMATERIALINV",
                        type: "POST",
                        data: postData,
                        datatype: "json",
                        success: function (data) {
                            var result = eval('(' + data + ')');
                            alert(result.Html);
                            pageReload();
                        },
                        error: function (e) {
                            alert(e.message);
                        }
                    });
                }
                saveToMaterialInvClick = false;
            });

            var exportClick = false;
            /**
             * 导出
             */
            $("#cpBody_btnExportFile").bind("click dbclick", function (e) {
                if (exportClick) {
                    return false;
                }
                exportClick = true;
                try {
                    var url = '/report/columbia/rpt_stock_transfer.aspx?method=ExportFile';
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
                                a.download = '库存中转.xlsx';
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
        })
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

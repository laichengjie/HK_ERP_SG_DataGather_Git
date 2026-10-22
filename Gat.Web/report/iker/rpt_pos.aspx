<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.iker.rpt_pos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>

    <!--jquery.validate   -->
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        /*datepicker*/
        .k-timepicker,
        .k-datetimepicker,
        .k-datepicker {
            border: 0 none;
            white-space: nowrap;
            width: 115px;
        }
    </style>
    <script type="text/javascript">
        jQuery(function ($) {

            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/iker/pos/1';
                var shopCode=encodeURI($shop.val());
                if(shopCode!=""){
                    url = url + '?shopcode=' + encodeURI($shop.val());
                }
                location.href = url;
            });
            //load date css
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
            /*导出*/
            $("#btnExport").bind("click dbclick", function (e) {
                if (exportDataClick) {
                    return false;
                }
                exportDataClick = true;
                try {
                    var startDate = $("#cpBody_txtStarDate").val() || '';
                    var endDate = $("#cpBody_txtEndDate").val() || '';
                    if (!startDate && !endDate) {
                        exportDataClick = false;
                        confirm("请先选择开始结束日期!");
                        return;
                    }
                    var solutionId = $("#cpBody_ddlSoule").val() || '';
                    var shopName = $("#cpHeaderNav_txtShopName").val() || '';
                    var solutionName = $("#cpBody_ddlSoule option[selected='selected']").html() || '';
                    var url = '/report/iker/rpt_pos.aspx?mode=ExportData&solutionId=' + solutionId + '&startDate=' + startDate + '&endDate=' + endDate + '&shopName=' + shopName;
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

      <%--  PageClick = function (pageclickednumber) {
            var shopCode= encodeURI('<%=shopCode %>');
            if(shopCode!=""){
                window.location.href = '/report/iker/pos/' + pageclickednumber+ "?shopcode=" + shopCode;
            }else{
                window.location.href = '/report/iker/pos/' + pageclickednumber;
            }
        }--%>

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/iker/pos/' + pageclickednumber + "?"
                + 'shopCode=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                 + '&shopName=' + encodeURI(document.getElementById("cpBody_txtShopName").value)
                + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                + '&barCode=' + encodeURI(document.getElementById("cpBody_txtBarCode").value)
                + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStarDate").value)
                + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndDate").value);
         }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div class="search">
        <div class="l">
            <span style="color: #FF6600; font-weight: bold;">方案</span>：
                            <asp:DropDownList ID="ddlSoule" runat="server" CssClass="k-picker-wrap">
                            </asp:DropDownList>
            &nbsp;&nbsp;
        </div>
        <div class="l">
              <span style="color: #FF6600; font-weight: bold;">店铺名称</span>：
              <asp:TextBox ID="txtShopName" runat="server" ></asp:TextBox>
            <span style="color: #FF6600; font-weight: bold;">日期</span>：
                            <asp:TextBox ID="txtStarDate" runat="server"  CssClass="datepicker"></asp:TextBox>
            -
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker" ></asp:TextBox>
            &nbsp;&nbsp;
        <span>
            <label>店铺代码：</label><asp:TextBox ID="txtShopCode" runat="server"></asp:TextBox>
            <label>货号：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
            <label>条码：</label><asp:TextBox ID="txtBarCode" runat="server"></asp:TextBox>
            <label>尺码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
            <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
            <asp:Button ID="btnDelSearch" runat="server" Text="删除时间段" OnClick="bthDelSearch_Click"   OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            <asp:Button ID="benDelCehcked" runat="server" Text="删除选中" OnClick="bthDelChecked_Click"   OnClientClick="return confirm('确定要删除数据吗？此操作不可恢复！');" />
            <button type="button" id="btnExport">导出</button>
        </span>
        </div>
       
        <br />
    </div>
    <asp:GridView ID="gvPos" runat="server" AutoGenerateColumns="False"
        CssClass="qp-gridview" DataKeyNames="id">
        <Columns>
             <asp:TemplateField>
     <HeaderStyle Width="20px"></HeaderStyle>
     <EditItemTemplate>
         <asp:CheckBox ID="CheckBox1" runat="server" />
     </EditItemTemplate>
     <ItemTemplate>
         <asp:CheckBox ID="CheckBox1" runat="server" />
     </ItemTemplate>
 </asp:TemplateField>
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="品牌" DataField="CardName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="单号" DataField="BillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="是否退货" DataField="IsReturn" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="销售时间" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField HeaderText="货号" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="规格" DataField="SizeName" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="零售价" DataField="RetailPrice" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:n2}" />
            <asp:BoundField HeaderText="销售数量" DataField="Qty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:n0}" />
            <asp:BoundField HeaderText="销售金额" DataField="SaleAmount" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:n2}" />
            <asp:BoundField HeaderText="来源单号" DataField="SourceBillNo" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="是否导入" DataField="IsImport" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="提报时间" DataField="ModifyTime" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到销售提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div>总销售金额:<%=this.SumAmount %> 总销售数量:<%=this.SumQty %></div>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

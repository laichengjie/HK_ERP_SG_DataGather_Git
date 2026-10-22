<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_inv.aspx.cs" Inherits="Gat.Web.report.ua.rpt_inv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>库存报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/ua/inv/1';
                url = url + '?shopcode=' + encodeURI($shop.val());
                location.href = url;
            });
        });
        PageClick = function (pageclickednumber) {
            window.location.href = '/report/ua/inv/' + pageclickednumber
                + '?shopcode=' + encodeURI(document.getElementById("cpBody_txtShopCode")?.value)
                + '&barCode=' + encodeURI(document.getElementById("cpBody_txtBarCode")?.value)
                + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode")?.value)
                + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode")?.value)
                 ;


        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<%--<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>--%>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <div>
    <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Style="margin-bottom: 0px" Text="导出数据" />
    <button type="button" id="AllDelate" class="ui-icon-save save ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false" style="margin-left: 10px;"><span class="ui-button-icon-primary ui-icon ui-icon-disk"></span><span class="ui-button-text">全部刪除</span></button>

   </div>
    <div>
    <asp:TextBox ID="txtShopCode" runat="server" type="text" placeholder="店铺编码" />
    <asp:TextBox ID="txtBarCode" runat="server" type="text" placeholder="条码" />
    <asp:TextBox ID="txtMaterialCode" runat="server" type="text" placeholder="货号" />
    <asp:TextBox ID="txtSizeCode" runat="server" type="text" placeholder="尺码" />
    <asp:Button  ID="btnSearch" runat="server" OnClick="BtnSearch_Click" Style="margin-bottom: 0px" Text="查询" />
   
</div>
   <asp:GridView ID="gvInv" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="店铺代码" DataField="ShopCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="条码" DataField="BarCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="尺码" DataField="SizeName" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField HeaderText="SKU" DataField="MaterialCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="库存数量" DataField="ActualQty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}"/>   
            <asp:BoundField HeaderText="在途数量" DataField="PathQty" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}"/>     
            <asp:BoundField HeaderText="库存日期" DataField="UpdateDate" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"  DataFormatString="{0:yyyy-MM-dd}"/>            
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到库存提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
    <div id="pager1">
    <div style="float: right; margin-right: 60px;">在途总数量:<asp:Label ID="Label2" runat="server" Text="Label1"></asp:Label></div>
    <div style="float: right; margin-right: 60px;">库存总数量:<asp:Label ID="Label1" runat="server" Text="Label1"></asp:Label></div>
    
          <script type="text/javascript">

              $(document).ready(function () {
                 
                  $('#AllDelate').click(function (e) {
                      e.preventDefault();
                      if (!confirm("是否确定要全部删除库存数据？此操作不可恢复！")) { return false; }
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
                              url: "/report/ua/rpt_inv.aspx?method=AllDelete",
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
                  
              });
          </script>

        <script type="text/javascript">
            $(document).ready(function () {
                $('#AllDelate').click(function (e) {
                    e.preventDefault();
                    if (!confirm("是否确定要全部删除库存数据？此操作不可恢复！")) {
                        return false;
                    }

                    // 禁用按钮，防止重复点击
                    var $btn = $(this);
                    $btn.prop('disabled', true).text('删除中...');

                    $.ajax({
                        cache: false,
                        type: "POST",
                        url: "/report/ua/rpt_inv.aspx?method=AllDelete",
                        data: { confirm: 'true' },
                        dataType: 'json',
                        success: function (response) {
                            if (response.StatusCode === "1") {
                                alert(response.Html || "删除成功！");
                                // 刷新当前页面，重新加载数据
                                window.location.href = window.location.pathname;
                            } else {
                                alert(response.Message || "删除失败，请重试！");
                                $btn.prop('disabled', false).text('全部刪除');
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("删除失败：" + thrownError);
                            $btn.prop('disabled', false).text('全部刪除');
                        }
                    });
                    return false;
                });
            });
        </script>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/report/Report.Master" AutoEventWireup="true" CodeBehind="rpt_pos.aspx.cs" Inherits="Gat.Web.report.asics.rpt_pos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>销售报表</title>

    <script type="text/javascript">
        jQuery(function ($) {
            var $shop = $('#<%=txtShopCode.ClientID %>');
            $shop.watermark("请输入店铺代码", "watermark2");
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
            $('#matquery').click(function(){
                var url = '/report/asics/pos/1';
                url = url + '?shopcode=' + encodeURI($shop.val())
                    + '&shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                    + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                    + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                    + '&billCode=' + encodeURI(document.getElementById("cpBody_txtBillCode").value)
                    + '&barCode=' + encodeURI(document.getElementById("cpBody_txtBarCode").value)
                    + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStartBillDate").value)
                    + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndBillDate").value);
                location.href = url;
            });

            $('#cpBody_bthSearch').click(function () {
                var url = '/report/asics/pos/<%=this.CurrentPage %>';
                url = url + '?shopcode=' + encodeURI($shop.val())
                    + '&shopCode2=' + encodeURI(document.getElementById("cpBody_txtShopCode2").value)
                    + '&materialCode=' + encodeURI(document.getElementById("cpBody_txtMaterialCode").value)
                    + '&sizeCode=' + encodeURI(document.getElementById("cpBody_txtSizeCode").value)
                    + '&billCode=' + encodeURI(document.getElementById("cpBody_txtBillCode").value)
                    + '&barCode=' + encodeURI(document.getElementById("cpBody_txtBarCode").value)
                    + '&startBillDate=' + encodeURI(document.getElementById("cpBody_txtStartBillDate").value)
                    + '&endBillDate=' + encodeURI(document.getElementById("cpBody_txtEndBillDate").value);
                location.href = url;
            });


            $('#cpBody_txtStartBillDate,#cpBody_txtStartBillTime').css('width', '150px')
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
            $('#cpBody_txtEndBillTime,#cpBody_txtEndBillDate').css('width', '150px')
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

        PageClick = function (pageclickednumber) {
            window.location.href = '/report/asics/pos/' + pageclickednumber+ "?shopcode=" + encodeURI('<%=shopCode %>');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search">
            <asp:TextBox ID="txtShopCode" runat="server"/>
            <a href="javascript:;" hideFocus="true" title="搜索" id="matquery">&nbsp;&nbsp;</a>     
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
     <div class="search">
     
     <div class="l">
         <span style="color: #FF6600; font-weight: bold;">单据日期</span>：
                         <asp:TextBox ID="txtStartBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
         -
                         <asp:TextBox ID="txtEndBillDate" runat="server" CssClass="datepicker"></asp:TextBox>
         &nbsp;&nbsp;
          <label>单号：</label><asp:TextBox ID="txtBillCode" runat="server"></asp:TextBox>
     <span>
         <br />
         <label>门店编码：</label><asp:TextBox ID="txtShopCode2" runat="server"></asp:TextBox>
         <label>产品编码：</label><asp:TextBox ID="txtMaterialCode" runat="server"></asp:TextBox>
         <label>产品尺码：</label><asp:TextBox ID="txtSizeCode" runat="server"></asp:TextBox>
         <label>产品条码：</label><asp:TextBox ID="txtBarCode" runat="server"></asp:TextBox>
         <asp:Button ID="bthSearch" runat="server" Text="查询" OnClick="bthSearch_Click" />
         
         总数量:<asp:Label ID="label_top_totoal_qty" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
         总金额:<asp:Label ID="label_top_totoal_amount" runat="server" Style="color: red; font-weight: bold;"></asp:Label>
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
     </span>
     </div>

     

     <br />
 </div>
    <asp:GridView ID="gvPos" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
            <asp:BoundField HeaderText="公司名称" DataField="CompanyName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>            
            <asp:BoundField HeaderText="单号" DataField="BillCode" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>  
            <asp:BoundField HeaderText="单据日期" DataField="BillDate" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"  DataFormatString="{0:yyyy-MM-dd HH:mm}"/>            
            <asp:BoundField HeaderText="门店名称" DataField="StoreName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="门店编码" DataField="StoreCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField HeaderText="产品编码" DataField="ProdCode" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />        
            <asp:BoundField HeaderText="产品名称" DataField="ProdName" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" /> 
            <asp:BoundField HeaderText="单位" DataField="ProdUnit" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Left" />     
            <asp:BoundField HeaderText="数量" DataField="ProdQuantity" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n0}" />    
            <asp:BoundField HeaderText="产品金额" DataField="ProdAmount" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:n2}" />     
            <asp:BoundField HeaderText="仓库名称" DataField="StockCode" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" />     
               <asp:BoundField HeaderText="产品条码" DataField="ProdBarCode" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" /> 
                <asp:BoundField HeaderText="产品尺码" DataField="ProdSpec" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left" />    
            <asp:BoundField HeaderText="备注" DataField="Remark" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left" />           
         
        </Columns>
        <EmptyDataTemplate>
            <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到销售提报记录！
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
    <div id="pager"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

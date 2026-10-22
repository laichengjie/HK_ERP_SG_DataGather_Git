<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_syslog.aspx.cs" Inherits="Gat.Web.admin.gat_syslog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>系统日志</title>
    <script src="/scripts/jquery.pager/jquery.pager.js" type="text/javascript"></script>
    <link href="/scripts/jquery.pager/pager.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.tooltip/jquery.tools.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.tooltip/jquery.tools.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/gat/syslog.js" type="text/javascript"></script>
    <style type="text/css">
        .detail{ text-indent:2em; }
        .more{  }
        .more a,.more a:hover{ float:right;padding:2px 5px; text-decoration:none;color:#656565;font-size:11px; }
        .k-datepicker{  width:110px;}
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#pager').pager({ pagenumber:  <%=this.CurrentPage %>, pagecount: <%=this.PageCount %>, buttonClickCallback: PageClick });
        
            var $vc = $('#<%= txtVendorCode.ClientID%>');
            $vc.watermark("请输入供应商代码", "watermark2");
                
        });
        PageClick = function (pageclickednumber) {
            window.location.href = '/log/' + pageclickednumber+ "?vendor=" + encodeURI('<%=vendorCode %>') + '&shop=' + encodeURI('<%=shopName %>') + '&card=' + encodeURI('<%=cardName %>') + '&start=' + encodeURI('<%=strartTimeString %>') + '&end=' + encodeURI('<%=endTimeString %>');
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">
        <div class="search" style="background-image:url(/images/search_bg1.png);">
            <asp:TextBox ID="txtVendorCode" runat="server" Width="140px"/>
            
            <a href="javascript:;" hideFocus="true" title="搜索" id="vendquery">&nbsp;&nbsp;</a>
            <a class="more condition" title="更多条件" href="javascript:;" hideFocus="true" >&nbsp;</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
    <!--<asp:BoundField DataField="CardName" HeaderText="品牌名称" HeaderStyle-Width="120px"/>
            <asp:BoundField DataField="CompanyName" HeaderText="公司名称" HeaderStyle-Width="120px"/>
            <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="120px"/>-->
    <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns = "False" 
        CssClass="qp-gridview" >
        <Columns>
            <asp:BoundField DataField="CompanyName" HeaderText="公司名称" HeaderStyle-Width="150px"/>
            <asp:BoundField DataField="VendorName" HeaderText="品牌供应商" HeaderStyle-Width="120px"/>
            <asp:BoundField DataField="CardName" HeaderText="品牌名称" HeaderStyle-Width="120px"/>
            <asp:TemplateField HeaderText="执行结果" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate >
                    <span><%# Convert.ToBoolean(Eval("IsSucceed")) ? "成功" : "失败"%></span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="执行脚本" HeaderStyle-Width="250px">
                <ItemTemplate >
                    <span style="display:none"><%#Eval("Script")%></span>
                    <span class="thick"><%#Eval("Script")%></span>      
                </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="备注" HeaderStyle-Width="200px">
                <ItemTemplate >
                    <span style="display:none"><%#Eval("Remark")%></span>
                    <span class="thick"><%#Eval("Remark")%></span>      
                    
                    <script type="text/javascript">
                        $('span.thick')
                        .each(function (i) {
                            var html = $(this).html();
                            if (html.length > 40) {
                                var temp = html.substr(0, 40);
                                temp = temp + "...&nbsp[<a href='javascript:;' onclick='showDetail(this)'>详细</a>]&nbsp;";
                                $(this).html(temp);
                            }
                        });

                        function showDetail(el) {
                            var body = $(el).parent().prev().html();
                            utils.dialog({
                                html: "<p class='detail'>" + body + "</p>",
                                width: 400,
                                title: '详细信息'
                            });
                        }
                    </script>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ModifyDTM" HeaderText="日志时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"  HeaderStyle-Width="150px"/>
        </Columns>
        <EmptyDataTemplate>
           <div class="data-empty">
                <i>&nbsp;</i>
                没有查询到日志记录！
           </div>
        </EmptyDataTemplate>
    </asp:GridView>
     <div id="pager"></div>


     <!--过滤条件-->
     <div class="hidden" id="bas_search">
        <div class="wrapper">
             <table cellspacing="0" cellpadding="0" class="mbox_form">
                <tr>
                    <td class="field">
                        供应商代码：
                    </td>
                    <td class="layout"><asp:TextBox ID="txtVCSearch" runat="server" CssClass="text title" 
                            Font-Names="微软雅黑"/>
                    </td>
                </tr>
                <tr>
                    <td class="field">
                        店铺名称：
                    </td>
                    <td class="layout valid">
                        <asp:TextBox ID="txtShopName" runat="server" CssClass="text title" 
                            Font-Names="微软雅黑"/>
                    </td>
                </tr>
                <tr>
                    <td class="field">
                        品牌名称：
                    </td>
                    <td class="layout valid">
                        <asp:TextBox ID="txtCardName" runat="server" CssClass="text title" 
                            Font-Names="微软雅黑"/>
                    </td>
                </tr>
                <tr>
                    <td class="none field multi">
                        日志时间：
                    </td>            
                    <td class="none layout">
                         <asp:TextBox ID="txtStartTime" runat="server" CssClass="datepicker" />
                         <asp:TextBox ID="txtEndTime" runat="server" CssClass="datepicker"/>
                         
                    </td>
                </tr>
            </table>
        </div>
     </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

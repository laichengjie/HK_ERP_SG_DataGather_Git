<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="gat_vendor.aspx.cs" Inherits="Gat.Web.admin.gat_vendor" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <!--jquery.validate-->
    <script src="/scripts/jquery.validate/jquery.validate.js" type="text/javascript"></script>
    <script src="/scripts/jquery.validate/jquery.validate.additional.js" type="text/javascript"></script>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />

    <script src="/scripts/gat/vendor.js" type="text/javascript"></script>

    <script type="text/javascript">
        var cardCode = '<%=txtCardCode.ClientID %>';
        var gvBasCard = '<%=gvCards.ClientID %>';
        //var url = '<%=this.GetRouteUrl("vendor-edit",new { id=this.IdentityKey })  %>';
        var vendor = '<%=this.IdentityKey %>';

        jQuery(function ($) {
            $('#form1').validate({
                errorElement: 'div',
                errorClass: 'errormt',
                rules: {
                    '<%=txtVendorCode.UniqueID %>': {
                        required: true,
                        maxlength: 20,
                        custcode: true
                    },
                    '<%=txtVendorName.UniqueID %>': { required: true, maxlength: 50 }
                },
                messages: {
                    '<%=txtVendorCode.UniqueID %>': {
                        required: "供应商代码不能为空！",
                        maxlength: "供应商代码不能长度不能大于20！",
                        remote: "该供应商已经存在！"
                    },
                    '<%=txtVendorName.UniqueID %>': { required: "供应商名称不能为空！", maxlength: "供应商名称长度不能大于50！" }
                }
            });
        });
    </script>
    
    <style type="text/css">
        /*search*/
        .search{
            background:url(/images/search_bg.png) no-repeat 0 0 ;
            width:160px;
            height:26px;
            float:right;
        }
        .search input{
            width:132px;
            border:0;
            background:url() no-repeat 0 0 transparent;
            float:left;
            padding:5px 0 5px 2px;
        }
        .search a{
            height:25px;
            float:right;
            width:25px;
            text-decoration:none;
            outline:none;
        }
        
        /*gv*/
        .qp-gridview th {
            background: url() repeat-x scroll 0 0 transparent;
        }
        .qp-gridview tr:hover td {
            background: none repeat scroll 0 0 #fff;
        }
        .ui-icon{ background-image: url(/images/ui-icons_ef8c08_256x240.png)}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpHeaderNav" runat="server">
    <div class="nav">        
        <%if (this.CUID == Gat.Web.Core.CUID.Update) { %>
          <a href=" <%=this.GetRouteUrl("vendor-add", new { }) %>" style="line-height:35px;">新增供应商</a>
          <%} %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpBody" runat="server">
    <!--主表-->
    <table cellspacing="0" cellpadding="0" class="mbox_form">
        <tr>
            <td class="field">
                供应商代码：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtVendorCode" runat="server" CssClass="text"></asp:TextBox>
            </td>            
        </tr>
        <tr>
            <td class="field">
                供应商名称：
            </td>
            <td class="layout">
                <asp:TextBox ID="txtVendorName" runat="server" CssClass="text"></asp:TextBox>
            </td>    
        </tr>
        <%if (DGContext.User.Role == Gat.Utils.RoleType.Developer)
          {%>
        <!--开发人员-->
        <tr>            
            <td class="field">
                关联报表：
            </td>
            <td class="layout valid">
                <asp:DropDownList ID="ddlReport" runat="server" CssClass="select">
                    <asp:ListItem Value="1">耐克报表</asp:ListItem>
                    <asp:ListItem Value="2">阿迪报表</asp:ListItem>
                    <asp:ListItem Value="3">卡帕报表</asp:ListItem>
                    <asp:ListItem Value="4">彪马报表</asp:ListItem>
                    <asp:ListItem Value="6">诺诗兰报表</asp:ListItem>
                    <asp:ListItem Value="7">萨洛蒙报表</asp:ListItem>
                     <asp:ListItem Value="8">哥伦比亚报表</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <%} %>
         <tr>
            <td class="field">
                创建日期：
            </td>
            <td class="layout valid">
                <asp:Label ID="lblCreateDate" runat="server" />
            </td>
        </tr>
       
        <tr>
            <td class="none field">
                允许使用：
            </td>            
            <td class="none layout">
                <asp:CheckBox ID="chkAllowUsed" runat="server" Checked="true" Enabled="false"/>
            </td>
        </tr>
    </table>

    <!--品牌明细-->
    <% if (this.CUID == Gat.Web.Core.CUID.Update){%>
    <div class="detail-wrapper vendor-card">
        <div class="detail-wrapper-header">
            <i>&nbsp;</i>
            <span class="detail-wrapper-title">
                供应商品牌
            </span>
            <b class="ui-icon ui-icon-calculator">&nbsp;</b>
            <!--<a class="vendor-card-action" href="<=this.GetRouteUrl("vendor-card",new { id=this.IdentityKey }) >" hideFocus="true">-->
            <a class="detail-wrapper-action vendcard" href="javascript:;" hideFocus="true">
               <span>选择</span>
            </a>
        </div>

        <div class="detail-wrapper-list vendor-card-list">            
            <asp:GridView ID="gvVendorCard" runat="server" AutoGenerateColumns = "False"  CssClass="qp-gridview">                
                <Columns>
                    <asp:TemplateField HeaderText="品牌代码" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <span class="card" style="display:none;"><%#Eval("CardID") %></span>
                            <span ><%#Eval("CardCode") %></span>
                        </ItemTemplate>

<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="品牌名称" DataField="CardName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
<HeaderStyle Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="创建日期" DataField="CreateDate" HeaderStyle-Width="150px" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-HorizontalAlign="Center">
<HeaderStyle Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:BoundField>
                </Columns>
                <EmptyDataTemplate>
                   <div class="data-empty">
                        <!--<i>&nbsp;</i>-->
                        您需要添加供应商品牌！
                   </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>       
    <% }%>

    <!--店铺明细-->
    <% if (this.CUID == Gat.Web.Core.CUID.Update){%>
    <div class="detail-wrapper vendor-shop" style="padding:20px 0; ">
        <div class="detail-wrapper-header">
            <i>&nbsp;</i>
            <span class="detail-wrapper-title">
                供应商店铺
            </span>
            <b class="ui-icon ui-icon-plus">&nbsp;</b>
            <a class="detail-wrapper-action vendshop" href="<%=this.GetRouteUrl("vendor-shop",new { id=this.IdentityKey }) %>" hideFocus="true">
               <span>添加</span>
            </a>
        </div>
        <div class="detail-wrapper-list vendor-shop-list">            
            <!--顺序千万不能变-->
            <asp:GridView ID="gvVendorShop" runat="server" AutoGenerateColumns = "False"  CssClass="qp-gridview" >                
                <Columns>                    
                    <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate >
                           <span class="shop" style="display:none;"><%#Eval("ID") %></span>
                           <span class="code"><%#Eval("VendorShopCode") %></span>
                        </ItemTemplate>

<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="店铺名称" DataField="VendorShopName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
<HeaderStyle Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="仓库代码" DataField="VendorStockCode" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Left">
<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="仓库名称" DataField="VendorStockName" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
<HeaderStyle Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="创建日期" DataField="CreateDate" HeaderStyle-Width="120px" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-HorizontalAlign="Center">
<HeaderStyle Width="120px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="操&nbsp;&nbsp;作" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href="javascript:;" onclick="updateShop(this);">修改</a> 
                            <i class="ui-icon ui-icon-pencil" style="display:inline;padding:2px 6px;background-position: -68px -110px;">&nbsp;</i>
                            &nbsp;
                            |
                            &nbsp;&nbsp;
                            <a onclick="deleteShop(this);" href="javascript:;" >删除</a>
                            <i class="ui-icon ui-icon-close" style="display:inline;padding:2px 6px;  background-position: -84px -126px;">&nbsp;</i>
                        </ItemTemplate>

<HeaderStyle Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                   <div class="data-empty">
                        <!--<i>&nbsp;</i>-->
                        您需要添加供应商店铺！
                   </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>       
    <% }%>
   
   <!--品牌基础资料-->
   <div id="bas_card" class="hidden">
    <div class="wrapper">
        <div class="search ng">
            <asp:TextBox ID="txtCardCode" runat="server" CssClass="card-code"/>
            <a href="javascript:;" hideFocus="true" title="搜索" class="func-query">&nbsp;&nbsp;</a>     
        </div>
        <div class="des ng">
            说明：勾选时表示品牌公司将提取该品牌的数据
        </div>
        <asp:GridView ID="gvCards" runat="server" AutoGenerateColumns = "False" 
            CssClass="qp-gridview" DataKeyNames="CardID">
            <Columns>
                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkSelectAll" runat="server" />
                    </HeaderTemplate>
                    <ItemTemplate >
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="品牌代码" HeaderStyle-Width="120px">
                    <ItemTemplate >
                       <span class="card" style="display:none;"><%#Eval("CardID") %></span>
                       <span ><%#Eval("CardCode") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="品牌名称" DataField="CardName" HeaderStyle-Width="150px" />
                <asp:BoundField />
            </Columns>
            <EmptyDataTemplate>
               <div class="data-empty">
                    <i>&nbsp;</i>
                    没有查询到品牌资料，建议您使用【同步资料】功能更新基础资料！
               </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
   </div>

   <!--店铺修改、增加ui-->
   <div id="vendor-shop-ui" class="hidden">
        <div class="wrapper">
            <p class="error"></p>
            <div class="ui-form">
                <label for="name">店铺代码&nbsp;&nbsp;&nbsp;</label>
		        <input type="text" name="shopcode" id="txtShopCode" class="text ui-widget-content ui-corner-all" />
                <p></p>
		        <label for="shopname">店铺名称&nbsp;&nbsp;&nbsp;</label>
		        <input type="text" name="shopname" id="txtShopName" class="text ui-widget-content ui-corner-all" />
                <p></p>
		        <label for="shopname">仓库代码&nbsp;&nbsp;&nbsp;</label>
		        <input type="text" name="stockcode" id="txtStockCode" class="text ui-widget-content ui-corner-all" />
                <p></p>
		        <label for="shopname">仓库名称&nbsp;&nbsp;&nbsp;</label>
		        <input type="text" name="stockname" id="txtStockName" class="text ui-widget-content ui-corner-all" />
            </div>
        </div>
   </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpAction" runat="server">
    <% if (this.CUID == Gat.Web.Core.CUID.Update){%>
        <!--新增按钮-->
        <button type="button" class="ui-icon-save">新增</button>
    <% }%>    
    
    <button type="button" class="ui-icon-save">保存</button>

    <% if (this.CUID == Gat.Web.Core.CUID.Update){%>        
        <button type="button" class="ui-icon-save">删除</button>
        <!--删除按钮-->
        <asp:Button ID="btnDelete" runat="server" Text="删除" CssClass="btn-normal"/>
    <% }%>

    <asp:Button ID="btnSave" runat="server" Text="保存" CssClass="btn-normal"/>
</asp:Content>



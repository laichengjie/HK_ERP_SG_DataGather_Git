<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" EnableEventValidation="false"
 CodeBehind="gat_shop_map.aspx.cs" Inherits="Gat.Web.admin.gat_shop_map" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>店铺关联</title>
    <!--jquery.ui.button-->
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui-1.8.16/development-bundle/ui/minified/jquery.ui.button.min.js" type="text/javascript"></script>
    <link href="/scripts/jquery.ui-1.8.16/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/gat/shopmap.js" type="text/javascript"></script>
    <script type="text/javascript">
        var vendor = '<%=this.IdentityKey %>', company = '<%=CompanyIdentity %>';
    </script>
    <style type="text/css">
        .layout span{ }
        .layout a.switch{
            float:left;
            outline:none;
            text-decoration:none;
            padding:2px 16px;
            margin:0px 0px 0px 220px;
            background:url(/images/icon_bg.png) no-repeat -5px -76px;
        }
        .map{
            outline:none;
            padding:2px 18px;
            background:url(/images/icon_bg.png) no-repeat -0px -4px;
        }

        /*品牌搜索*/
        .shop-search{
            background:url(/images/search_bg.png) no-repeat 0 0 ;
            width:160px;
            height:26px;
            float:right;
        }
        .shop-search input{
            width:132px;
            border:0;
            background:url() no-repeat 0 0 transparent;
            float:left;
            padding:5px 0 5px 2px;
        }
        .shop-search a{
            height:25px;
            float:right;
            width:25px;
            text-decoration:none;
            outline:none;
        }
        .des{
            color:#777;
            font-size:11px;
            /*clear:both;*/
            padding:5px;
        }
        .data-empty{text-align:left;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpHeaderNav" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpBody" runat="server">
     <table cellspacing="0" cellpadding="0" class="mbox_form">
        <tr>
            <td class="field">
                品牌供应商：
            </td>
            <td class="layout">
               <span class="l"><%=this.VendorName%></span>
               <%if (!string.IsNullOrEmpty(VendorName)) {%>
                    <a href="javascript:;" class="switch">选择</a>
               <% } %>
            </td>
           
        </tr> 
        <tr>
            <td class="none field">
                提报公司：
            </td>
            <td class="none layout">
                <%=this.CompanyName%>
            </td>
        </tr>
    </table>

    <div style="color:#777;">
    说明：品牌供应商的店铺（大仓）关联到ERP中的店铺（大仓）
    </div>
    <!--店铺关联管理-->
    <div class="gv-wrapper">
        <asp:GridView ID="gvVendorShop" runat="server" AutoGenerateColumns = "False"  CssClass="qp-gridview" DataKeyNames="ID">
            <Columns>           
                 <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                    <ItemTemplate >
                       <span class="shop" style="display:none;"><%#Eval("ID") %></span>
                       <span ><%#Eval("VendorShopCode")%></span>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="VendorShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="VendorStockCode" HeaderText="仓库代码" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="VendorStockName" HeaderText="仓库名称" HeaderStyle-Width="150px"/>
                <asp:TemplateField HeaderText="关联的ERP店铺">
                    <ItemTemplate>
                        <span class="hidden code"><%#Eval("ID")%></span>
                        <%#Eval("MapShopCodes")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操&nbsp;&nbsp;作" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <span class="hidden code"><%#Eval("ID")%></span>
                        <a hideFocus="true" class="map" href="javascript:;" onclick="javascript:shopMap(this);">关联</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        
            <AlternatingRowStyle CssClass="alternate" />
        </asp:GridView>
    </div>

     <!--选择框供应商和公司-->
    <div class="hidden" id="bas_gat">
        <div class="wrapper" >
            <table cellspacing="0" cellpadding="0" class="mbox_form">
                <tr>
                    <td class="field">
                        品牌供应商：
                    </td>                    
                    <td class="layout">
                      <asp:DropDownList ID="ddlVendor" runat="server" style="display:none;"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="none field">
                        提报公司：
                    </td>
                    <td class="none layout">
                        <asp:DropDownList ID="dllSolCompany" runat="server" style="display:none;">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <!--选择框店铺-->
    <div class="hidden" id="bas_shopright">
        <div class="wrapper" >
           <div class="shop-search ng">
                <asp:TextBox ID="txtCardCode" runat="server" CssClass="shop-code"/>
                <a href="javascript:;" hideFocus="true" title="搜索" class="func-query">&nbsp;&nbsp;</a>     
            </div>
           <div class="des ng">
                说明：允许时表示将该ERP的店铺关联到供应商的店铺，关联时供应商的店铺与ERP的店铺是一对多的关系
                <!--<br />
                允许时表示将该ERP的店铺关联到供应商的店铺
                <br />
                允许数据提报的店铺才能被关联到供应商的店铺
                <br />
                一个供应商的店铺可以关联多个EPR的店铺，
                <br />
                一个ERP店铺只能关联到一个供应商的店铺，-->
            </div>
           <asp:GridView ID="gvShopForMap" runat="server" AutoGenerateColumns = "False" 
                CssClass="qp-gridview" DataKeyNames="ShopID">
                <Columns>
                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkSelectAll" runat="server" />
                    </HeaderTemplate>
                    <ItemTemplate >
                        <asp:CheckBox ID="chkSelect" runat="server" Checked='<%#Eval("IsMapped") %>'/>
                        <span class="shop" style="display:none;"><%#Eval("ShopID") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                <ItemTemplate >
                    <span class="shop" style="display:none;"><%#Eval("ShopID") %></span>
                    <span ><%#Eval("ShopCode") %></span>
                </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockCode" HeaderText="仓库代码" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockName" HeaderText="仓库名称" HeaderStyle-Width="150px"/>

                </Columns>
                <EmptyDataTemplate>
                   <div class="data-empty">
                        <i>&nbsp;</i>
                        没有可关联的ERP店铺，可能原因：
                        <ul>
                            <li>您没有设置ERP店铺为允许提报数据</li>
                            <li>您已经将所有的允许提报的ERP店铺关联到供应商的其它店铺中</li>
                            <li>没有符合查询条件的店铺</li>
                        </ul>
                   </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

        <!--选择框店铺-->
    <div class="hidden" id="bas_shop">
        <div class="wrapper" >           
           <asp:GridView ID="gvBasShop" runat="server" AutoGenerateColumns = "False" 
                CssClass="qp-gridview" DataKeyNames="ShopID">
                <Columns>
                <asp:TemplateField HeaderText="店铺代码" HeaderStyle-Width="120px">
                <ItemTemplate >
                    <span class="shop" style="display:none;"><%#Eval("ShopID") %></span>
                    <span ><%#Eval("ShopCode") %></span>
                </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockCode" HeaderText="仓库代码" HeaderStyle-Width="150px"/>
                <asp:BoundField DataField="StockName" HeaderText="仓库名称" HeaderStyle-Width="150px"/>

                </Columns>
            </asp:GridView>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

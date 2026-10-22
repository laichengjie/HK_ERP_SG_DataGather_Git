<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Gat.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Gat.Web.admin._default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHeader" runat="server">
    <title>首页</title>
    <script type="text/javascript">
        //手动采集数据
        function manualGat(el,log,company,vendor,card) {

            var ui = $('#bas_manual .wrapper').clone();
            var body = $(document.body);
            var win = $(window);

            utils.dialog({
                html: ui,
                title: '补采数据',
                icon: 'loading',
                width: 350,
                create: function (yuan) {
                    $(':text', yuan.body)
                    .each(function (i) {

                        var value = $(el).prevAll('span'); //存储时间的值
                        $(this)
                        .css('border', 'none')
                        .kendoDatePicker({
                            format: 'yyyy-MM-dd HH:mm:ss',
                            min: new Date(2000, 01, 01),
                            value: new Date(value.eq(i).html()),
                            open: function () {
                                if (body.height() <= win.height()) body.addClass('overflow');
                            },
                            close: function () {
                                body.removeClass('overflow');
                            }
                        });
                        //.val(value.eq(i).html())

                    });


                },
                buttons: {
                    '采集': function (yuan) {
                        var patern = /^((((((19|20)\d{2})-(0?(1|[3-9])|1[012])-(0?[1-9]|[12]\d|30))|(((19|20)\d{2})-(0?[13578]|1[02])-31)|(((19|20)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|((((19|20)([13579][26]|[2468][048]|0[48]))|(2000))-0?2-29)))\s((([0-1][0-9])|(2[0-3])):[0-5][0-9]:[0-5][0-9]))$/i;
                        var kendostart = $(":text[id$='txtStartTime']", yuan.body);
                        var kendoend = $(":text[id$='txtEndTime']", yuan.body);
                        var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);

                        var start = kendostart.val();
                        var end = kendoend.val();

                        //alert(start + '|' + end); return;
                        if (!patern.test(start) || !patern.test(end)) {
                            $op
                            .html('')
                            .addClass("warning")
			                .html('<i class="warning">&nbsp;</i>日期格式不正确！');

                            return false;
                        }

                        //开始ajax保存
                        $.ajax({
                            type: "POST",
                            url: '/admin/default.aspx',
                            timeout: 50000,
                            data: "mode=gat&start=" + start + "&end=" + end + "&vendor=" + vendor + "&company=" + company + "&card=" + card + '&log=' + log,
                            beforeSend: function () {
                                $op
                                .html('')
                                .append($('<i class="loading">&nbsp;</i>'));
                                yuan.disable();
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $op.html('');
                                var message = '&nbsp;补采失败';
                                if (textStatus === 'timeout') {
                                    message += '，消息：加载超时，请稍后重试！';
                                } else {
                                    message += "，请稍后重试！";
                                }

                                //$op.addClass('warning').html(message);
                                $op
                                .addClass("warning")
				                .html('<i class="warning">&nbsp;</i>' + message);

                            },
                            success: function (html) {

                                var json = utils.toJson(html);

                                if (json.StatusCode == '1') {
                                    //更新日志提醒部分
                                    $op
                                    .removeClass("warning")
                                    .addClass("correct")
				                    .html('<i class="correct">&nbsp;</i>' + json.Message);


                                    yuan.opt.callback = function () {
                                        $('div.log')
                                        .empty()
                                        .append($(json.Html));

                                        if ($('div.log li').length === 0) {
                                            $('div.log').parent('div.detail-wrapper').css('display','none');
                                        }
                                    }
                                    $('a.yuan-botton', yuan.bottom).eq(0).unbind('click').remove();
                                    //utils.undialog();
                                    //$('a.yuan-botton', yuan.buttom).eq(0).fadeOut(200);  //.css('display', 'none');
                                }
                                else {
                                    var tip = json.html === '' ? '补采错误，原因：未知！' : json.Message;
                                    $op
                                    .addClass("warning")
				                    .html('<i class="warning">&nbsp;</i>' + tip);
                                }
                            },
                            complete: function () {
                                yuan.enable();
                            }
                        });
                    } /*,
                    '取消': utils.undialog*/
                }
            });

        }
    </script>
    <style type="text/css">
        .detail-wrapper { padding:20px 0; }
        .detail-wrapper-header a{ line-height:29px;outline:none; }
        .detail-wrapper-list li{    border-top: 1px dotted #C8C8C8;font: 12px/30px arial;text-align: left}
        .detail-wrapper-list li.none{ border:0px; }
        .ui-icon{ background-image: url(/images/ui-icons_ef8c08_256x240.png)}
        .ui-icon-newwin { background-position: -48px -80px; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpHeaderNav" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cpBody" runat="server">
    <!--日志通知-->
    <div class="detail-wrapper <%= this.LogDisplay %>">
        <div class="detail-wrapper-header">
            <i>&nbsp;</i>
            <span class="detail-wrapper-title">
                日志提醒
            </span>
            <a hideFocus="true" class="r" href='<%=this.GetRouteUrl("log",new{}) %>'>>>更多&nbsp;&nbsp;</a>
        </div>
        <div class="detail-wrapper-list log">   
            <asp:Repeater ID="rptErrorLog" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><i style="color:#777;"><%#Eval("Index") %>.&nbsp;</i><%#Eval("Html") %></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <script type="text/javascript">
                $('.detail-wrapper-list li:eq(0)').addClass('none');
            </script>
        </div>
       
    </div>

    <!--品牌供应商-->
    <div class="detail-wrapper <%= this.VendorDisplay %>">
        <div class="detail-wrapper-header">
            <i>&nbsp;</i>
            <span class="detail-wrapper-title">
                供应商快照
            </span>
            <a hideFocus="true" class="r" href='<%=this.GetRouteUrl("vendor",new{}) %>'>>>更多&nbsp;&nbsp;</a>
        </div>
        <div class="detail-wrapper-list v">   
            <asp:Repeater ID="rptVendor" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><i style="color:#777;"><%#Eval("Index") %>.&nbsp;</i><%#Eval("Html") %></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
                
            </asp:Repeater>
            <script type="text/javascript">
                $('.v li:eq(0)').addClass('none');
            </script>
        </div>

        <!--提报方案-->
    <div class="detail-wrapper <%= this.SolutionDisplay %>">
        <div class="detail-wrapper-header">
            <i>&nbsp;</i>
            <span class="detail-wrapper-title">
                提报方案快照
            </span>
            <a hideFocus="true" class="r" href='<%=this.GetRouteUrl("solution",new{}) %>'>>>更多&nbsp;&nbsp;</a>
        </div>
        <div class="detail-wrapper-list s">   
            <asp:Repeater ID="rptSolution" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><i style="color:#777;"><%#Eval("Index") %>.&nbsp;</i><%#Eval("Html") %></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <script type="text/javascript">
                $('.s li:eq(0)').addClass('none');
            </script>
        </div>
       
    </div>
    

    <!--补采条件-->
    <div id="bas_manual" class="hidden">
        <div class="wrapper">
            <table cellpadding="0" cellspacing="0" class="mbox_form">
                <tr>
                    <td class="field">
                        开始时间：
                    </td>
                    <td class="layout">
                       <asp:TextBox ID="txtStartTime" runat="server" CssClass="datepicker" Width="200px">
                        </asp:TextBox>
                    </td>
                </tr> 
                <tr>
                    <td class="field none">
                        结束时间：
                    </td>
                    <td class="layout none">
                       <asp:TextBox ID="txtEndTime" runat="server" CssClass="datepicker" Width="200px">
                        </asp:TextBox>
                    </td>
                </tr>
                
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cpAction" runat="server">
</asp:Content>

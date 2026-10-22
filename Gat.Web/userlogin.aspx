<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs" Inherits="Gat.Web.userlogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户登录</title>
    <link href="/styles/default.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.min.1.6.2.js" type="text/javascript"></script>

    <!--kendoui-->
    <link href="/scripts/kendoui/styles/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="/scripts/kendoui/styles/kendo.kendo.min.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/kendoui/js/kendo.all.min.js" type="text/javascript"></script>
    <script src="/scripts/kendoui/js/cultures/kendo.culture.zh-CN.js" type="text/javascript"></script>

    <script src="/scripts/jquery.yuan/jquery.yuanyuan.js?v=20220519" type="text/javascript"></script>
    <link href="/scripts/jquery.yuan/yuanyuan.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        .login-outer {
            padding-top: 10%;
            margin: 0 auto;
            width: 730px;
        }

        .login-com-logo {
            background: url(/images/login_left.png) no-repeat;
            height: 359px;
            width: 327px;
            float: left;
        }

        .login-inner {
            width: 400px;
            float: right;
            /*clear: both;
            content: ".";
            display: block;
            height: 0;
            visibility: hidden;*/
        }

            .login-inner .site-name {
                background: url(/images/index_font.jpg) no-repeat;
                height: 37px;
            }

            .login-inner .login-title {
                background: url(/images/login_font.png) no-repeat;
                height: 34px;
                width: 59px;
                margin: 10px 0;
            }

        #btnLogin {
            clear: both;
            width: 83px;
            height: 30px;
            background: #666666 url(/images/login_btn.png) no-repeat;
            text-align: center;
            line-height: 31px;
            color: #FFFFFF;
            border: none;
            font-size: 10pt;
            font-weight: bold;
            outline: none;
            border: none;
        }

        .login-tip {
            padding: 2px 4px;
            /*background:url(/images/null.png) no-repeat 8px center #F7F7F7;*/
            background-color: #F7F7F7;
            text-indent: 2em;
            color: #ed3636;
        }

            .login-tip span {
                background: url(/images/null.png) no-repeat 8px center #F7F7F7;
                padding: 2px 15px;
            }

        .logon-text {
            border: 1px solid #ccc;
            border-color: #888 #e7e7e7 #e7e7e7 #888;
            padding: 3px 1px;
            color: #003366;
            width: 200px;
        }

            .logon-text:focus {
                border: 1px solid #CC9900;
                border-color: #CC9900 #f8e6b1 #f8e6b1 #CC9900;
            }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            var dm = $('#dialog-modal');
            var $code = $("#userCode"), $password = $("#password");
            var $environment = $('select.environment:eq(0)');
            $environment.kendoDropDownList();

            $(document).keyup(function (e) {
                if (e.keyCode == 13) {
                    var nullCheck = /.+/;
                    if (nullCheck.test($code.val()) &&
                        nullCheck.test($password.val())) {
                        if ($('#btnLogin')[0] != null) $('#btnLogin')[0].focus();
                        $('#btnLogin').trigger('click');
                    }
                }
            });

            $('div.login-inner :radio').css({ 'padding': '0px', 'margin': '0px' });

            $('input.logon-text').bind('focus', function () {
                $('.login-tip').fadeOut('200', function () {
                    $(this).html('&nbsp;')
                        .css({
                            'display': 'block',
                        });
                });

                this.select();
            });

            $code[0].focus();

            var isLoginClick = false;
            $('#btnLogin').click(function () {
                var code = $('#userCode').val(),
                    password = $('#password').val();
                if (isLoginClick) {
                    return;
                }
                isLoginClick = true;
                var nullCheck = /.+/;

                if (!nullCheck.test($code.val())) {
                    $('.login-tip')
                        .css({
                            'opacity': '0.0'
                        })
                        .html("<span></span>请您输入用户名称！")
                        .fadeTo('200', '1');
                    isLoginClick = false;
                    return false;
                }
                if (!nullCheck.test($password.val())) {
                    $('.login-tip').css({
                        'opacity': '0.0'
                    })
                        .html("<span></span>请您输入用户密码！")
                        .fadeTo('200', '1');
                    isLoginClick = false;
                    return false;
                }

                var pattern = /returnurl=(.+)/i;
                var matches = pattern.exec(location.href);
                var u = matches !== null && matches.length > 0 ? matches[1] : '';
                // 数据保存
                $.ajax({
                    type: "POST",
                    url: 'userlogin.aspx',
                    timeout: 50000,
                    data: "type=logon&code=" + $code.val() + '&password=' + $password.val() + '&en=' + $environment.data("kendoDropDownList").value() + ($('#chkMemoPwd').attr('checked') ? "&memopwd=" : "") + (u !== '' ? "&u=" + u : ""),
                    beforeSend: function () {
                        $('#loading').css('display', 'block');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var msg = '加载失败';
                        if (textStatus === 'timeout') {
                            msg += '，消息：加载超时。请稍后重试！';
                        } else {
                            msg += "，请稍后重试！";
                        }

                        if (isLoginClick) {
                            $.yuanyuan({
                                html: msg,
                                icon: 'warning',
                                hClose: false,
                                buttons: {
                                    '确定': function (yuan) {
                                        yuan.hide();
                                        isLoginClick = false;
                                    }
                                }
                            });
                        }
                    },
                    success: function (html) {
                        var json = eval('(' + html + ')');
                        if (json.StatusCode === '1') {
                            /*if(json.Html !== ''){
                                 window.location.href =json.Html;
                            }else{
                                 window.location.href ='/admin/default.aspx' ;
                            }*/
                            //alert(json.Html);
                            window.location.href = json.Html;
                        } else {
                            /*$.dialog({
                                html: '<i></i>' + json.Message,
                                buttons: {
                                    '确定': function () {
                                        $(this).dialog('close');
                                    }
                                }
                            });*/
                            if (isLoginClick) {
                                $.yuanyuan({
                                    html: json.Message,
                                    icon: 'warning',
                                    hClose: false,
                                    buttons: {
                                        '确定': function (yuan) {
                                            yuan.hide();
                                            isLoginClick = false;
                                        }
                                    }

                                });
                            }
                            //alert(html);
                        }
                    },
                    complete: function (jqXHR, textStatus) {
                        $('#loading').css('display', 'none');
                    }
                });

            });
        });

        /*$.dialog = function (opt) {
            $('#dialog-modal')
                    .html(opt.html || '&nbsp;&nbsp;')
                    .dialog({
                        modal: true,
                        show: 'puff', //show:"blind",clip,drop,explode,fold,puff,slide,scale,size,pulsate
                        hide: 'puff',
                        resizable: opt.resizable || false,
                        minHeight: opt.minHeight || 80,
                        title: opt.title || '提示',
                        buttons: opt.buttons || {},
                        close: function () {
                            $('#dialog-modal').dialog('destroy');
                            $(document.body).removeClass('overflow');
                            if ($.isFunction(opt.close)) opt.close();
                        },
                        open: function () {
                            $(document.body).addClass('overflow');
                            var disable = opt.disabled || false;
                            if ($.isFunction(opt.open)) opt.open();
                        }
                    });
        }*/

//        function JsonToStr(o) {      
//            var arr = [];      
//            var fmt = function(s) {      
//            if (typeof s == 'object' && s != null) return JsonToStr(s);      
//                alert(typeof s);
//                return /^(string|number)$/.test(typeof s) ? "'" + s + "'" : s;      
//            }      
//            for (var i in o) arr.push("'" + i + "':" + fmt(o[i]));      
//            return '{' + arr.join(',') + '}';      
//       }

//       var x ={
//            name : '11',
//            age : 12,
//            sex : {
//                m:'man',
//                v:'woman'
//            }
//       };

//       alert(JsonToStr(x));
    </script>
</head>
<body>
    <form id="form1" runat="server">


        <div class="login-outer">
            <div class="login-com-logo">
                <img src="/images/hk_logo.gif" alt="" style='border: 0;' />
            </div>
            <div class="login-inner">
                <div class="site-name">&nbsp;</div>
                <div class="login-title">&nbsp;</div>
                <div>
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <div style="background: url(/images/login_top.png) no-repeat; height: 5px; width: 400px;">
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="background: #f7f7f7; width: 400px;">
                                    <div>
                                        <div style="width: 400px;">
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td style="width: 70px; height: 30px; text-align: right;">用户名：
                                                                </td>
                                                                <td>
                                                                    <input type="text" name="textfield" id="userCode" class="logon-text" value='<%=CookieUser.UserCode %>' />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 70px; height: 30px; text-align: right;">密码：
                                                                </td>
                                                                <td>
                                                                    <input type="password" name="textfield" id="password" class="logon-text" value='<%=CookieUser.Password %>' />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 70px; height: 30px; text-align: right;">帐套：
                                                                </td>
                                                                <td>
                                                                    <!--<table style="width:100%;">
                                                                <tr>                                                                    
                                                                    <td style="width:30px;"><label>正式</label></td>
                                                                    <td><asp:RadioButton ID="rbDue" runat="server" GroupName="Environment" Checked="true"/></td>
                                                                    <td style="width:30px;"><label>测试</label></td>
                                                                    <td style="width:60px;"><asp:RadioButton ID="rbTest" runat="server" GroupName="Environment"/></td>                        
                                                                </tr>
                                                            </table>-->
                                                                    <select class="environment" style="width: 100%;">
                                                                        <option selected="selected" value="1">正式</option>
                                                                        <option value="2">测试</option>
                                                                    </select>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2">
                                                                    <div class='login-tip'>&nbsp;</div>
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </td>
                                                    <td valign="top">
                                                        <div style="padding-left: 10px;">
                                                            <button id="btnLogin" type="button"></button>
                                                            <table border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <input id="chkMemoPwd" type="checkbox" value="一周免登录" name="check" <%=GetChecked() %>
                                                                            style="float: left; display: block; padding: 0; margin: 0; margin-top: 8px;" />
                                                                    </td>
                                                                    <td>
                                                                        <span style="float: left; display: block; margin-top: 8px; margin-left: 3px; color: #666666;">一周免登录</span></td>
                                                                </tr>
                                                            </table>
                                                            <img src="/images/loading.gif" id='loading' style="display: none; margin-top: 12px;" alt="正在登入" title="正在登录" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="background: url(/images/login_bottom.gif) no-repeat; height: 5px; width: 400px;">
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div id="dialog-modal" style="display: none">
            <p></p>
        </div>
        <div id="app"></div>
    </form>
</body>
</html>


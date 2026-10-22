
/*
 * Master 文件js
 *
 */

jQuery(function ($) {
    // kendo.ui汉化
    kendo.culture("zh-CN");

    //导航文件  ========================
    $("div.nav-bar")
    .css('display', 'block')
    .children('div.nav-inner')
    .children('ul')
    .addClass('topnav')
    .find('ul')
    .addClass('subnav')
    .end()
    .find('a')
    .attr('hideFocus', true);

    $("ul.subnav").parent().append("<span></span>");

    $("ul.topnav li ").click(function () {

        $(this).children().parent().find("ul.subnav").slideDown('fast').show();

        $(this).children().parent().hover(function () {
        }, function () {
            $(this).children().parent().find("ul.subnav").slideUp('slow');
        });

    }).hover(function () {
        $(this).addClass("subhover");
    }, function () {
        $(this).removeClass("subhover");
    });
    




    //单复选框没边框
    $(':radio').css('border', 'none');
    $(':checkbox').css('border', 'none');

    $('div.top-brand select').kendoDropDownList(); //供应商

    if ($('div.data-empty').length > 0) {
        $('#pager').css('display', 'none');
    }


    //密码修改
    $('#password-modify')
    .click(function (e) {
        e.preventDefault();
        var ui = $('#password-moidfy-ui .wrapper').clone();

        utils.dialog({
            html: ui,
            height: 200,
            width: 350,
            title: '修改密码',
            loading: true,
            buttons: {
                '确定': function (yuan) {
                    var _old = $('#txtOld', ui),
			            _new = $("#txtNew", ui)
                    var $op = yuan.bottom.find('.yuan-bottom-op:eq(0)');

                    function checkLength(o, n, min, max) {
                        if (o.val().length > max || o.val().length < min) {
                            o.addClass("ui-state-error");
                            updateTips(n + "的长度必须在" + min + '和' + max + '个字符之间！');
                            o[0].focus();
                            return false;
                        } else {
                            return true;
                        }
                    }

                    function updateTips(t) {
                        $op
                        .addClass("warning")
				        .html('<i class="warning">&nbsp;</i>' + t);
                    }

                    function checkRegexp(o, regexp, n) {
                        if (!(regexp.test(o.val()))) {
                            o.addClass("ui-state-error");
                            updateTips(n);
                            return false;
                        } else {
                            return true;
                        }
                    }

                    var bValid = true;
                    bValid = bValid && checkLength(_new, "密码", 1, 16);
                    if (!bValid) return false;

                    var equalTo = _old.val() === $('p.password', ui).html(); //alert(_old.val() + '|' + $('p.password', ui).html() + equalTo);
                    if (!equalTo) {
                        updateTips('旧密码不对！');
                    }
                    bValid = bValid && equalTo;

                    bValid = bValid && checkRegexp(_old, /^([0-9a-zA-Z_])+$/i, "密码只能是数字、字母和下划线！");
                    bValid = bValid && checkRegexp(_new, /^([0-9a-zA-Z_])+$/i, "密码只能是数字、字母和下划线！");

                    if (bValid) {
                        $.ajax({
                            type: "POST",
                            url: '/admin/AJAXHandler.ashx',
                            timeout: 50000,
                            data: 'mode=password&bypast=' + _old.val() + '&new=' + _new.val(),
                            beforeSend: function () {
                                $op
                                .html('')
                                .append($('<i class="loading">&nbsp;</i>'));
                                //.append($('<span></span>').html('正在更新密码...'));

                                yuan.disable();
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $op.html('');
                                var message = '&nbsp;查询失败';
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
                                    $('p.password', ui).html(_new.val());
                                    $op.html('<i class="correct">&nbsp;&nbsp;</i>更新密码成功，3 秒后自动关闭此对话框。');
                                    window.setTimeout(function () {
                                        yuan.hide();
                                    }, 3000);

                                } //else if (json.StatusCode == '2') {
                                //updateTips('<i></i>更新失败错误，原因：旧密码错误！');}
                                else {
                                    var tip = json.html === '' ? '更新错误，原因：未知！' : json.Message;
                                    //$op.addClass('warning').html('' + tip);
                                    $op
                                    .addClass("warning")
				                    .html('<i class="warning">&nbsp;</i>' + tip);
                                }
                            },
                            complete: function () {
                                yuan.enable();
                            }
                        });
                    }

                },
                '取消': utils.undialog
            },
            create: function (yuan) {
                $('#txtOld', ui).watermark("旧密码必填", "watermark2");
                $('#txtNew', ui).watermark("新密码必填", "watermark2");
                yuan.body.focus();


                //回车
                $(document).keyup(function (e) {
                    if (e.keyCode == 13) {

                        var ok =
                        yuan
                        .bottom
                        .find('a.yuan-botton:eq(0)');

                        //ok[0].focus();
                        ok.trigger('click');
                    }
                });
            }
        });
    }); // 密码修改完毕


    //数据同步
    $('#data-synchron')
    .bind('click', function (e) {
        e.preventDefault();

        utils.dialog({
            html: '正在同步数据，请稍候...',
            icon: 'loading',
            create: function (yuan) {
                yuan.disable();

                $.ajax({
                    type: "POST",
                    url: '/admin/AJAXHandler.ashx',
                    timeout: 50000,
                    data: "mode=sync",
                    beforeSend: function () {

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var message = '同步失败';
                        if (textStatus === 'timeout') {
                            message += '，消息：加载超时。请稍后重试！';
                        } else {
                            message += "，请稍后重试！";
                        }

                        yuan.setHtml({
                            icon: 'warning',
                            html: message
                        });
                    },
                    success: function (html) {
                        var json = utils.toJson(html);


                        if (json.StatusCode === '1') {

                            yuan.setHtml({
                                icon: 'correct',
                                html: json.Html
                            });

                        } else {

                            yuan.setHtml({
                                icon: 'warning',
                                html: json.Html
                            });
                        }


                    },
                    complete: function (jqXHR, textStatus) {
                        yuan.enable();
                    }

                }); //AJAX
            }
        });


    }); //同步结束
});
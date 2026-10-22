
jQuery(function ($) {

    $("select[id$='ddlReport']").kendoDropDownList();

    $('a.vendcard') // 品牌脚本 
    .bind('click', function (e) {
        var ui = $('#bas_card .wrapper').clone();
        e.preventDefault();
        utils.dialog({
            html: ui,
            height: 500,
            width: 400,
            title: '选择品牌',
            loading: true,
            buttons: {
                '确定': saveCards,
                '取消': utils.undialog
            },
            create: function (yuan) {
                $('.card-code', yuan.body).watermark("请输入品牌代码", "watermark2"); //水印

                var empty = $('div.data-empty', yuan.body);
                if (empty.length > 0) {
                    yuan.bottom.remove();
                }
                // 复选
                initCheck(yuan);

                //查询
                yuan.body.find('a.func-query')
                .click(function () {
                    var $vc = yuan.body.find('.card-code').eq(0); //品牌代码
                    if ($.trim($vc.val()) === '') {
                        return false;
                    }
                    gotoQuery(yuan, $.trim($vc.val())); //查询
                });
            }
        });
    });

    $('a.vendshop') // 店铺脚本 =============================================
    .bind('click', function (e) {
        e.preventDefault();
        createShopui();
    });

    //jquery.ui.button
    var $uib = $('button.ui-icon-save');
    $uib.eq(0)
    .css('margin-left', (function () {
        return $uib.length === 1 ? '320px' : '170px';
    })())
    .button({
        icons: {
            //primary: "ui-icon-disk"
            primary: ($uib.length === 1 ? "ui-icon-disk" : "ui-icon-plus")
        }
    })
    .click(function (ev) {
        //var e = ev || window.event;
        //e.preventDefault();
        //$(":submit[id$='save']").trigger('click');
        //location.href = '/vendor/add';
        if ($uib.length === 1) {
            $(":submit[id$='Save']").trigger('click');
        } else {
            location.href = '/vendor/add';
        }
    })
    .next("button")
    .css('margin-left', '10px')
    .button({
        icons: {
            primary: "ui-icon-disk"
        }
    })
    .click(function (ev) {
        $(":submit[id$='Save']").trigger('click');
    })
    .next("button")
    .css('margin-left', '10px')
    .button({
        icons: {
            primary: "ui-icon-trash "
        }
    })
    .click(function (ev) {
        utils.dialog({
            html: '您确定要删除这笔资料吗？',
            icon: 'warning',
            buttons: {
                '确定': function () {
                    $(":submit[id$='Delete']").trigger('click');
                },
                '取消': function () {
                    utils.undialog();
                }
            }
        });
    })
    ;

    $('.vendor-card-list,vendor-shop-list')
    .each(function () {
        setMaxHeight($(this));
    });
    //var y = $('div.vendor-shop-list:eq(0)');
    //setMaxHeight(y);

});

//窗口创建时初始化选中状态
function initCheck(yuan) {

    var gv = yuan.body.find('table.qp-gridview');

    //初始化选中状态
    var gvvc = $("div.vendor-card table.qp-gridview").eq(0); //供应商品牌
    var cid = []; //alert(gvvc.length);
    gvvc
    .find('span.card')
    .each(function () {
        cid.push($(this).html());
    });  //取出所有的供应商品牌代码

    gv
    .find('span.card')
    .each(function () {
        var code = $(this).html();
        if ($.inArray(code, cid) !== -1) {
            $(this)
            .parent()
            .prev()
            .find(':checkbox')
            .attr('checked', 'checked');

        }
    });    //取出所有的供应商品牌代码/**/

    var $checkboxs = gv.find(':checkbox');
    var $checkAll = gv.find(":checkbox[id$='chkSelectAll']");
    var $check = gv.find(":checkbox[name$='chkSelect']");


    if ($check.filter(':checked').length === $check.length) $checkAll.attr('checked', 'checked');

    $checkAll.change(function () {
        if ($(this).is(':checked')) {
            $check.attr('checked', 'checked');
        } else {
            $check.attr('checked', false);
        }
    });
}

// 查询
function gotoQuery(yuan, cardcode) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    //alert($op.length);
    $.ajax({
        type: "POST",
        url: '/admin/gat_vendor.aspx',
        timeout: 50000,
        data: { mode: 'cardquery', cardcode: encodeURI(cardcode) },
        beforeSend: function () {
            $op
            .html('')
            .append($('<i class="loading">&nbsp;</i>'));
            //.append($('<span></span>').html('正在查询...'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $op.html('');
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }

            $op
            .addClass("warning")
			.html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {
            var json = utils.toJson(html);
            if (json.StatusCode === '1') {
                $op.html('');

                yuan
                .body
                .find('table.qp-gridview')
                .parent()
                .remove();

                $(json.Html)
                .css({
                    'opacity': 0,
                    'background-color': '#fff'
                })
                .appendTo(yuan.body.children())
                .fadeTo('250', 1);

                initCheck(yuan);

            } else {
                $op
                .find('i')
                .remove();

                $op
                .addClass("warning")
			    .html('<i class="warning">&nbsp;</i>' + json.Html);

            }
        },
        complete: function (jqXHR, textStatus) {
            yuan.enable();
        }
    });
}

//保存品牌设置
function saveCards(yuan){
    var gv = $('table.qp-gridview:eq(0)', yuan.body);
    var $check = gv.find(":checkbox[name$='chkSelect']");

    var ccode = []; //选中的品牌代码数组
    var scope = []; //作用范围
    $check.each(function () {
        var me = $(this);
        var code = me.parent().next().children('.card:eq(0)').html() ;//品牌代码
        if (me.is(':checked')) ccode.push(code);
        scope.push(code);
    });


    var codestr = ccode.join(',');
    var scopestr = scope.join(',');

    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    //alert($op.length);
    $.ajax({
        type: "POST",
        url: '/admin/gat_vendor.aspx',
        timeout: 50000,
        data: { mode: 'cardsave', cardcode: encodeURI(codestr), vendor: vendor, scope: encodeURI(scopestr) },
        beforeSend: function () {
            $op
            .html('')
            .append($('<i class="loading">&nbsp;</i>'));
            //.append($('<span></span>').html('正在保存...'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $op.html('');
            var message = '&nbsp;保存失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时，请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }

            $op
            .addClass("warning")
			.html('<i class="warning">&nbsp;</i>' + message);
            yuan.enable();
        },
        success: function (html) {
            var json = utils.toJson(html);
            if (json.StatusCode === '1') {
                //reloadCards(yuan); //重新加载品牌
                utils.undialog();

                var vclist = $('div.vendor-card-list:eq(0)');
                vclist.html('');
                $(json.Html)
                .appendTo(vclist);

                setMaxHeight(vclist);

            } else {
                /*$op
                .find('i')
                .remove();*/

                $op
                .html('')
                .addClass("warning")
			    .html('<i class="warning">&nbsp;</i>' + json.Html);

                yuan.enable();
            }
        },
        complete: function (jqXHR, textStatus) {
            //yuan.enable();
        }
    });
}


//重新生成品牌列表
function reloadCards(yuan) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    $.ajax({
        type: "POST",
        url: '/admin/gat_vendor.aspx',
        timeout: 50000,
        data: { mode: 'cardreload', vendor: vendor },
        beforeSend: function () {
            $op
            .html('')
            .append($('<i>&nbsp;</i>'))
            .append($('<span></span>').html('正在加载供应商品牌...'));
            //yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $op.html('');
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时，请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }

            $op.find('span').html(message);
        },
        success: function (html) {
            var json = utils.toJson(html);
            if (json.StatusCode === '1') {
                utils.undialog();

                $(json.Html)
                .appendTo($('.vendor-card-list').html(''));

                $('div.vendor-card-list')
                .each(function () {
                    var maxHeight = 250;
                    if ($(this).height() > maxHeight) {
                        $(this)
                        .css({
                            'height': maxHeight,
                            'overflow-y': 'scroll'
                        });
                    } else {
                        $(this)
                        .css({
                            'height': 'auto',
                            'overflow-y': 'hidden'
                        });
                    }
                }); // 高度

            } else {
                $op
                .find('i')
                .remove();

                $op
                .find('span')
                .html('&nbsp;' + json.Html);
            }
        },
        complete: function (jqXHR, textStatus) {
            yuan.enable();
        }
    });
}

//创建供应商店铺界面，shop为空则创建，否则更新
function createShopui(opt) {
    var ui = $('#vendor-shop-ui .wrapper').clone();
    var shop = $.extend({
        shopid : '0',
        shopcode : '',
        shopname : '',
        stockcode : '',
        stockname: ''
    }, opt);
    if (shop.shopcode === '&nbsp;') shop.shopcode = '';
    if (shop.shopname === '&nbsp;') shop.shopname = '';
    if (shop.stockcode === '&nbsp;') shop.stockcode = '';
    if (shop.stockname === '&nbsp;') shop.stockname = '';

    var $shopCode = $('#txtShopCode', ui),
        $shopName = $('#txtShopName', ui),
        $stockCode = $('#txtStockCode', ui),
        $stockName = $('#txtStockName', ui);

    //赋初值
    $shopCode.val(shop.shopcode);
    $shopName.val(shop.shopname);
    $stockCode.val(shop.stockcode);
    $stockName.val(shop.stockname);

    utils.dialog({
        html: ui,
        height: 250,
        width: 350,
        title: '供应商店铺',
        loading: true,
        buttons: {
            '确定': function (yuan) {

                shop.shopcode = $shopCode.val();
                shop.shopname = $shopName.val();
                shop.stockcode = $stockCode.val();
                shop.stockname = $stockName.val();
                //alert(shop.shopcode);
                saveShop(yuan, shop);
            },
            '取消': utils.undialog
        },
        create: function (yuan) {
            $shopCode.watermark("店铺代码必填", "watermark2");
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
}

//保存供应商店铺
function saveShop(yuan,shop) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    if (shop === null || typeof shop === 'undefined') {
        $op
        .html('')
        .addClass("warning")
		.html('<i class="warning">&nbsp;</i>店铺参数错误！' );

        return false;
    };

    if (shop.shopcode === null || typeof shop.shopcode === 'undefined' || shop.shopcode === '') {
        $op
        .html('')
        .addClass("warning")
		.html('<i class="warning">&nbsp;</i>店铺代码不能为空！');

        $('#txtShopCode', yuan.body).focus();
        return false;
    }

    if (shop.shopcode.length > 20) {
        $op
        .html('')
        .addClass("warning")
		.html('<i class="warning">&nbsp;</i>代码长度不能大于20！');

        return false;
    }

    shop.mode = "shopsave"; //保存参数
    shop.vendor = vendor;

    var pattern = /^[0-9a-zA-z_]{1,20}$/i;
    if (!pattern.test(shop.shopcode)) {
        $op
        .html('')
        .addClass("warning")
		.html('<i class="warning">&nbsp;</i>代码必须由字母和数字组成！');

        return;
    }

    $.ajax({
        type: "POST",
        url: '/admin/gat_vendor.aspx',
        timeout: 50000,
        data: shop,
        beforeSend: function () {
            $op
            .html('')
            .append($('<i class="loading">&nbsp;</i>'));
            //.append($('<span></span>').html('正在保存...'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $op.html('');
            var message = '&nbsp;&nbsp;保存失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时，请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }

            $op
            .addClass("warning")
			.html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {
            var json = utils.toJson(html);
            if (json.StatusCode === '1') {
                utils.undialog();

                $(json.Html)
                .appendTo($('.vendor-shop-list').html(''));

                setMaxHeight($('div.vendor-shop-list:eq(0)'));

            } else {
                /*$op
                .find('i')
                .remove();

                $op
                .find('span')
                .html('&nbsp;&nbsp;' + json.Html);*/
                $op
                .html('')
                .addClass("warning")
			    .html('<i class="warning">&nbsp;</i>' + json.Html);
            }
        },
        complete: function (jqXHR, textStatus) {
            yuan.enable();
        }
    });
}

//设置高度
function setMaxHeight(element) {

    var maxHeight = 300;
    element
    .css({
        'height': 'auto',
        'overflow-y': 'hidden'
    });
    if (element.height() > maxHeight) {
        element
        .css({
            'height': maxHeight,
            'overflow-y': 'scroll'
        });
    }
}

//店铺修改锚点
function updateShop(anchor) {

    var $a = $(anchor);
    var $tr = $a.parent().parent();
    var $td = $tr.children('td:eq(0)');
    var shop = {};
    shop.shopid = $td.find('span.shop').html();
    shop.shopcode = $td.find('span.code').html();
    $td = $td.next();
    shop.shopname = $td.html();
    $td = $td.next();
    shop.stockcode = $td.html();
    $td = $td.next();
    shop.stockname = $td.html();
    createShopui(shop);
}

//删除店铺
function deleteShop(anchor) {
    utils.dialog({
        html: '您确定要删除该店铺吗？',
        icon: 'warning',
        create: function (yuan) {
            yuan.title.focus();
        },
        buttons: {
            '确定': function (yuan) {

                var $a = $(anchor);
                var shopid =
                $a
                .parent()
                .parent()
                .find('span.shop:eq(0)')
                .html();

                //var $body = $('.yuan-bottom-body:eq(0)', yuan.buttom);
                var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
                $.ajax({
                    type: "POST",
                    url: '/admin/gat_vendor.aspx',
                    timeout: 50000,
                    data: { mode: 'deleteshop', shop: shopid, vendor: vendor },
                    beforeSend: function () {
                        $op
                        .html('')
                        .append($('<i class="loading">&nbsp;</i>'));
                        //.append($('<span></span>').html('正在删除...'));
                        yuan.disable();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        var message = '&nbsp;&nbsp;保存失败';
                        if (textStatus === 'timeout') {
                            message += '，消息：加载超时，请稍后重试！';
                        } else {
                            message += "，请稍后重试！";
                        }

                        //$body.find('span').html(message);
                        $op
                        .html('')
                        .addClass("warning")
			            .html('<i class="warning">&nbsp;</i>' + message);
                    },
                    success: function (html) {
                        var json = utils.toJson(html);
                        if (json.StatusCode === '1') {
                            utils.undialog();


                            $(json.Html)
                            .appendTo($('.vendor-shop-list').html(''));

                            setMaxHeight($('div.vendor-shop-list:eq(0)'));

                            //var y = $('div.vendor-shop-list:eq(0)');


                        } else {
                            /*$body
                            .find('i')
                            .remove();

                            $body
                            .find('span')
                            .html('&nbsp;' + json.Html);*/
                            $op
                            .html('')
                            .addClass("warning")
			                .html('<i class="warning">&nbsp;</i>' + json.Html);
                        }
                    },
                    complete: function (jqXHR, textStatus) {
                        yuan.enable();
                    }
                });

            }, //确定结束


            '取消': utils.undialog
        }
    });    
    
}
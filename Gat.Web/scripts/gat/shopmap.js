
jQuery(function ($) {

    $('a.switch')
    .bind('click', function (e) {
        e.preventDefault();

        var kendo_vendor = null,
            kendo_company = null;
        var kendo_org = null; //change之前的值
        alert('s');
        utils.dialog({
            html: $('#bas_gat .wrapper').clone(),
            height: 230,
            width: 400,
            title: '关联',
            loading: true,
            buttons: {
                '确定': function (yuan) {
                    var id = kendo_vendor.val(),
                    company = kendo_company.val();
                    var href = '/shop/map' + '/' + id + '?company=' + company;
                    location.href = href;
                },
                '取消': utils.undialog
            },
            callback: function () {
                $("div.k-list-container,div.k-animation-container").remove();
            },
            create: function (yuan) {
                kendo_vendor = $("select[id$='ddlVendor']", yuan.body);
                kendo_company = $("select[id$='dllSolCompany']", yuan.body);

                kendo_vendor.kendoDropDownList({
                    change: function () {

                        var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
                        $.ajax({
                            type: "POST",
                            url: '/admin/gat_shop_map.aspx',
                            timeout: 50000,
                            data: "mode=map&vendor=" + kendo_vendor.val(),
                            beforeSend: function () {
                                $op
                                .html('')
                                .append($('<i class="loading">&nbsp;</i>'));
                                //.append($('<span></span>').html('正在查询...'));
                                yuan.disable();
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $op.html('');
                                var message = '&nbsp;加载数据失败';
                                if (textStatus === 'timeout') {
                                    message += '，消息：加载超时。请稍后重试！';
                                } else {
                                    message += "，请稍后重试！";
                                }

                                $op.find('span').html(message);
                                kendo_vendor.data("kendoDropDownList").value(kendo_org) //加载失败，还原下拉列表最初始值
                            },
                            success: function (html) {

                                var json = utils.toJson(html);


                                if (json.StatusCode === '1') {

                                    //更新 公司下拉列表
                                    $("div[id*='dllSolCompany']").remove();
                                    //var hold = [];
                                    $('div.k-animation-container')
                                    .each(function () {
                                        var c = $(this).children("div[id*='dllSolCompany']");
                                        if (c.length > 0) {
                                            //$(this).empty();
                                            $(this).remove();
                                        }
                                    });
                                    //取包含公司的TD
                                    var kendo_td_company = kendo_company.parent();
                                    kendo_td_company
                                    .empty()
                                    .append($(json.Message));

                                    kendo_company = kendo_td_company.find('select:eq(0)'); //指向新的引用
                                    kendo_company.kendoDropDownList().parent().css('display', ''); //重新绑定到KENDOUI

                                    //更新 初始值
                                    kendo_org = kendo_vendor.data("kendoDropDownList").value();

                                } else {

                                    $op.find('span').html(json.Html);
                                    kendo_vendor.data("kendoDropDownList").value(kendo_org) //加载失败，还原下拉列表最初始值
                                }

                            },
                            complete: function (jqXHR, textStatus) {
                                $op
                                .html('');
                                yuan.enable();
                            }

                        });  //AJAX


                    } //change

                }).parent().css('display', ''); //kenui
                kendo_org = kendo_vendor.data("kendoDropDownList").value(); //初始化下拉列表的最初值

                kendo_company
                .kendoDropDownList()
                .parent().css('display', '')
                ;
            }
        });
    }); //对话框 


});

//关联店铺
function shopMap(anchor) {
    var me = $(anchor);
    alert('1');
    utils.dialog({
        //html: '',
        height: 500,
        width: 400,
        title: '关联店铺',
        icon: 'none',
        buttons: {
            '确定': function (yuan) {
                var shop = me.parent().children('span.code:eq(0)').html();
                saveShopMap(yuan, shop);
            },
            '取消': utils.undialog
        },
        create: function (yuan) {

            //加载可关联的店铺
            loadShopForMap(yuan, me);
        }
    });
}

//加载可关联的店铺
function loadShopForMap(yuan,el) {
    //取店铺标识
    var shop = 
        el
        .parent()
        .find('span.code:eq(0)')
        .html();
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    //alert($op.length);
    $.ajax({
        type: "POST",
        url: '/admin/gat_shop_map.aspx',
        timeout: 50000,
        data: { mode: 'loadmap', shop: shop, vendor: vendor, company: company },
        beforeSend: function () {
            //$op
            //.html('')
            //.append($('<i class="loading">&nbsp;</i>'));
            //.append($('<span></span>').html('正在查询...'));
            yuan.bottom.stop().css('opacity', 0);
            yuan.disable();
            yuan.body.empty().css({
                'background': 'url(/scripts/jquery.yuan/ajax-loader.gif) no-repeat center center'
            });

        },
        error: function (jqXHR, textStatus, errorThrown) {
            //$op.html('');
            var message = '&nbsp;加载失败';
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
            //alert(html);
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                //yuan.opt.width = 400;

                var $wrapper = $('#bas_shopright div.wrapper');
                $wrapper.children('div:eq(2)').remove();

                $(json.Html)
                .appendTo($wrapper);

                yuan.setHtml({
                    html: $wrapper.clone()
                });


                //初始化搜索界面
                $txt = $('.shop-code', yuan.body);
                $txt.watermark("请输入店铺代码", "watermark2"); //水印

                var empty = $('div.data-empty', yuan.body);
                if (empty.length > 0) {
                    yuan.bottom.remove();
                    return false;
                } else {
                    //查询
                    yuan
                    .body
                    .find('a.func-query')
                    .click(function () {
                        gotoQuery(yuan, $txt,shop); //查询
                    });

                    initCheck(yuan, el);
                }



            } else {
                yuan.setHtml({
                    icon: 'warning',
                    html: json.Html
                });
            }
        },
        complete: function (jqXHR, textStatus) {
            yuan.bottom.fadeTo('200', 1);
            yuan.enable();
        }
    });
}

//窗口创建时初始化选中状态
function initCheck(yuan) {

    var gv = yuan.body.find('table.qp-gridview');

    //var allSelect = true;
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
function gotoQuery(yuan, el,shop) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    //alert($op.length);
    $.ajax({
        type: "POST",
        url: '/admin/gat_shop_map.aspx',
        timeout: 50000,
        data: { mode: 'shopquery', shopcode: encodeURI(el.val()), vendor: vendor, company: company, shop: shop },
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

            $op.find('span').html(message);
        },
        success: function (html) {

            //alert(html);
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {
                
                //yuan.opt.width = 400;

                var $wrapper = $('#bas_shopright div.wrapper');
                $wrapper.children('div:eq(2)').remove();

                $(json.Html)
                .appendTo($wrapper);

                yuan.setHtml({
                    html: $wrapper.clone()
                });


                //初始化搜索界面
                $txt = $('.shop-code', yuan.body);
                $txt.watermark("请输入店铺代码", "watermark2"); //水印

                /*var empty = $('div.data-empty', yuan.body);
                if (empty.length > 0) {
                    yuan.bottom.remove();
                    return false;
                } else {
                    //查询
                    yuan
                    .body
                    .find('a.func-query')
                    .click(function () {
                        gotoQuery(yuan, $txt, shop); //查询
                    });

                    initCheck(yuan, el);
                }*/
                //查询
                yuan
                .body
                .find('a.func-query')
                .click(function () {
                    gotoQuery(yuan, $txt, shop); //查询
                });

                initCheck(yuan, el);


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
    });
}


//保存店铺映射
function saveShopMap(yuan,shop) {
    var gv = yuan.body.find('table.qp-gridview');
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);

    //var allSelect = true;
    var $checkboxs = gv.find(':checkbox');
    var $checkAll = gv.find(":checkbox[id$='chkSelectAll']");
    var $check = gv.find(":checkbox[name$='chkSelect']");

    //选中的
    var selected = [];
    var scope = [];
    $check.each(function () {
        var me = $(this);
        var shop = me.next('span.shop').html();
        if (me.is(':checked')) selected.push(shop);
        scope.push(shop);
    });


    // 开始AJAX保存
    $.ajax({
        type: "POST",
        url: '/admin/gat_shop_map.aspx',
        timeout: 50000,
        data: { mode: 'mapshop', shop: shop, selected: selected.join(','), vendor: vendor, company: company, scope: scope.join(',') },
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

            $op.find('span').html(message);
        },
        success: function (html) {

            //alert(html);
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {
                $('div.gv-wrapper')
                .empty()
                .append($(json.Html));
                utils.undialog();
            } else {
                $op.find('span').html(json.Html);
            }
        },
        complete: function (jqXHR, textStatus) {
            yuan.enable();
        }
    });
}


//展示供应商店铺所关联的ERP店铺
function showMapping(anchor) {
    //取店铺标识
    var shop =
        $(anchor)
        .parent()
        .find('span.code:eq(0)')
        .html();

    utils.dialog({
        //html: '',
        height: 500,
        width: 400,
        title: '关联的ERP店铺',
        icon: 'none',
        /*buttons: {
            '确定': function (yuan) {

                

            },
            '取消': utils.undialog
        },*/
        create: function (yuan) {

            var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);

            $.ajax({
                type: "POST",
                url: '/admin/gat_shop_map.aspx',
                timeout: 50000,
                data: { mode: 'showmapping', shop: shop, vendor: vendor, company: company },
                beforeSend: function () {

                    yuan.bottom.stop().css('opacity', 0);
                    yuan.disable();
                    yuan.body.empty().css({
                        'background': 'url(/scripts/jquery.yuan/ajax-loader.gif) no-repeat center center'
                    });

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //$op.html('');
                    var message = '&nbsp;加载失败';
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
                    //alert(html);
                    var json = utils.toJson(html);
                    $op.html('');
                    if (json.StatusCode === '1') {

                        var $wrapper = $('#bas_shop div.wrapper');
                        $wrapper.children().remove();

                        $(json.Html).appendTo($wrapper);

                        yuan.setHtml({
                            html: $wrapper.clone()
                        });


                    } else {
                        yuan.setHtml({
                            icon: 'warning',
                            html: json.Html
                        });
                    }
                },
                complete: function (jqXHR, textStatus) {
                    yuan.bottom.fadeTo('200', 1);
                    yuan.enable();
                }
            });

        }
    });
    
    
}
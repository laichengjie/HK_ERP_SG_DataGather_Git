
jQuery(function ($) {
    //水印
    $("input[id$='txtShopCode']").watermark("请输入店铺代码", "watermark2");
    //kendoui.vendor
    /*$("select[id$='ddlVendor']").kendoDropDownList({        
        
    change: function () {
    location.href = '/shop/grt/' + $("select[id$='ddlVendor']").val()
    }
    });*/

    $('a.switch')
    .bind('click', function (e) {
        e.preventDefault();

        var kendo_vendor = null,
            kendo_card = null,
            kendo_company = null;
        var kendo_org = null; //change之前的值

        utils.dialog({
            html: $('#bas_gat .wrapper').clone(),
            height: 300,
            width: 400,
            title: '提报',
            loading: true,
            buttons: {
                '确定': function (yuan) {
                    var id = kendo_vendor.val(),
                    card = kendo_card.val(),
                    company = kendo_company.val();
                    var href = '/shop/grt' + '/' + id + '?card=' + card + '&company=' + company;
                    location.href = href;
                },
                '取消': utils.undialog
            },
            callback: function () {
                $("div.k-list-container,div.k-animation-container").remove();
            },
            create: function (yuan) {
                kendo_vendor = $("select[id$='ddlVendor']", yuan.body);
                kendo_card = $("select[id$='ddlVendorCard']", yuan.body);
                kendo_company = $("select[id$='dllSolCompany']", yuan.body);

                kendo_vendor.kendoDropDownList({
                    change: function () {

                        var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
                        $.ajax({
                            type: "POST",
                            url: '/admin/gat_shop_right.aspx',
                            timeout: 50000,
                            data: "mode=grt&vendor=" + kendo_vendor.val(),
                            beforeSend: function () {
                                $op
                                .html('')
                                .append($('<i class="loading">&nbsp;</i>'));
                                //.append($('<span></span>').html('正在查询...'));
                                yuan.disable();
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                //$op.html('');
                                var message = '&nbsp;加载数据失败';
                                if (textStatus === 'timeout') {
                                    message += '，消息：加载超时。请稍后重试！';
                                } else {
                                    message += "，请稍后重试！";
                                }

                                $op
                                .html('')
                                .addClass("warning")
			                    .html('<i class="warning">&nbsp;</i>' + message);
                                //op.find('span').html(message);
                                kendo_vendor.data("kendoDropDownList").value(kendo_org) //加载失败，还原下拉列表最初始值
                            },
                            success: function (html) {

                                var json = utils.toJson(html);


                                if (json.StatusCode === '1') {

                                    //更新 品牌下拉列表
                                    $("div[id*='ddlVendorCard']").remove();
                                    //var hold = [];
                                    $('div.k-animation-container')
                                    .each(function () {
                                        var c = $(this).children("div[id*='ddlVendorCard']");
                                        if (c.length > 0) {
                                            //$(this).empty();
                                            $(this).remove();
                                        }
                                    });

                                    //取包含品牌的TD
                                    var kendo_td = kendo_card.parent();
                                    kendo_td
                                    .empty()
                                    .append($(json.Html));

                                    kendo_card = kendo_td.find('select:eq(0)'); //指向新的引用
                                    kendo_card.kendoDropDownList().parent().css('display', ''); //重新绑定到KENDOUI

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

                                    $op
                                    .html('');

                                } else {
                                    $op
                                    .html('')
                                    .addClass("warning")
			                        .html('<i class="warning">&nbsp;</i>' + json.Html);
                                    //$op.find('span').html(json.Html);
                                    kendo_vendor.data("kendoDropDownList").value(kendo_org) //加载失败，还原下拉列表最初始值
                                }

                            },
                            complete: function (jqXHR, textStatus) {
                                //$op
                                //.html('');
                                yuan.enable();
                            }

                        });  //AJAX


                    } //change

                }).parent().css('display', '') ; //kenui
                kendo_org = kendo_vendor.data("kendoDropDownList").value(); //初始化下拉列表的最初值

                kendo_card.kendoDropDownList().parent().css('display', '') ;
                kendo_company.kendoDropDownList().parent().css('display', '');
            }
        });
    }); //对话框 


    //jquery.ui.botton
    var $uib = $('button.ui-icon-save');
    var $sumbmit = $(":submit[id$='btnSave']");
    $uib.eq(0)
    .css('margin-left', (function () {
        return $uib.length === 1 ? '320px' : '230px';
    })())
    .button({
        icons: {
            primary: "ui-icon-disk"
        }
    })
    .click(function (ev) {
        $sumbmit.trigger('click');
    });

    //jquery.watermark
    $('#shopquery')
    .click(function () {
        var $search = $(":submit[id$='Search']");
        if ($search.length > 0) {
            //            var $code = $("input[id$='txtShopCode']");
            //            if ($.trim($code.val()) !== '') {
            //                $search.trigger('click');
            //            }

            $search.trigger('click');
        }
    });


    // yuan.checkbox
    initCheck();

});


//窗口创建时初始化选中状态
function initCheck() {

    var gv =$('table.qp-gridview:eq(0)');

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


//加载指定供应商的提报公司和品牌,ddl:kendouidropdown,orgvalue原始值
function loadCardAndCompany(yuan,ddl,orgvalue) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    $.ajax({
        type: "POST",
        url: '/admin/gat_shop_right.aspx',
        timeout: 50000,
        data: "mode=grt&vendor=" + vendor,
        beforeSend: function () {
            $op
            .html('')
            .append($('<i class="loading">&nbsp;</i>'));
            //.append($('<span></span>').html('正在查询...'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //$op.html('');
            var message = '&nbsp;加载数据失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }

            //$op.find('span').html(message);
            $op
            .html('')
            .addClass("warning")
			.html('<i class="warning">&nbsp;</i>' + message);
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

    });  //AJAX
}

jQuery(function ($) {



    $('a.condition')
    .click(function (e) {
        e.preventDefault();

        utils.dialog({
            html: $('#bas_search .wrapper').clone(),
            //height: 300,
            width: 400,
            title: '日志条件',
            buttons: {
                '确定': function (yuan) {
                    var vendor = $("input[id$='txtVCSearch']", yuan.body).val();
                    var shop = $("input[id$='txtShopName']", yuan.body).val();
                    var card = $("input[id$='txtCardName']", yuan.body).val();
                    var start = $("input[id$='txtStartTime']", yuan.body).val();
                    var end = $("input[id$='txtEndTime']", yuan.body).val();

                    var url = '/log/1';
                    url = url + "?vendor=" + encodeURI(vendor) + '&shop=' + encodeURI(shop) + '&card=' + encodeURI(card) + '&start=' + encodeURI(start) + '&end=' + encodeURI(end);
                    location.href = url;
                },
                '取消': utils.undialog
            },
            create: function (yuan) {
                //日期选择
                $('input.datepicker', yuan.body)
                .bind('focus', function () {
                    $(this).css('border', 'none')

                }).kendoDatePicker({
                    format: 'yyyy-MM-dd',
                    min: new Date(2000, 01, 01),
                    open: function () {
                        //$(document.body).addClass('overflow');
                    },
                    close: function () {
                        //$(document.body).removeClass('overflow');
                    }
                });
            }
        });
    });

    $('#vendquery')
    .click(function () {
        var vendor = $("input[id$='txtVendorCode']").val();
        var url = '/log/1';
        url = url + "?vendor=" + encodeURI(vendor);
        location.href = url;
    });
});
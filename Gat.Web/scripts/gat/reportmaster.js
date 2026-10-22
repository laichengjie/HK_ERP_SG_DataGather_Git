
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

    $("ul.topnav li span").click(function () {
        $(this).parent().find("ul.subnav").slideDown('fast').show();

        $(this).parent().hover(function () {
        }, function () {
            $(this).parent().find("ul.subnav").slideUp('slow');
        });
    }).hover(function () {
        $(this).addClass("subhover");
    }, function () {
        $(this).removeClass("subhover");
    });

    if ($('div.data-empty').length > 0) {
        $('#pager').css('display', 'none');
    }

    //二次导航文件  ========================
    $("div.secondary-nav-bar")
        .css('display', 'block')
        .children('div.secondary-nav-inner')
        .children('ul')
        .addClass('secondnav')
        .find('ul')
        .addClass('secondsubnav')
        .end()
        .find('a')
        .attr('hideFocus', true);

    $("ul.secondsubnav").parent().append("<span></span>");

    $("ul.secondnav li span").click(function () {
        $(this).parent().find("ul.secondsubnav").slideDown('fast').show();

        $(this).parent().hover(function () {
        }, function () {
            $(this).parent().find("ul.secondsubnav").slideUp('slow');
        });
    }).hover(function () {
        $(this).addClass("secondsubhover");
    }, function () {
        $(this).removeClass("secondsubhover");
    });

    if ($('div.data-empty').length > 0) {
        $('#pager').css('display', 'none');
    }

    $("ul.secondnav li").hover(function () {
        if ($(this).hasClass('isselect'))
            $(this).removeClass("select");
    }, function () {
        if ($(this).hasClass('isselect'))
            $(this).addClass("select");
    });



    //供应商下拉列表
    var $ddl = $("#ddlVendor");
    $ddl.change(
        function () {
            var value = $(this).val();
            location.href = '/report/default/' + value;
        }
    );

});

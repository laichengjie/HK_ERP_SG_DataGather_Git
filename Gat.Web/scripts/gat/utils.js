
var utils = {}; // 公用对象

utils.dialog = function (opt) {
    $.yuanyuan(opt);
};
utils.undialog = function () {
    $.yuanyuan.hide();
}

utils.format = function (source, params) {
    if (arguments.length == 1)
        return function () {
            var args = $.makeArray(arguments);
            args.unshift(source);
            return $.validator.format.apply(this, args);
        };
    if (arguments.length > 2 && params.constructor != Array) {
        params = $.makeArray(arguments).slice(1);
    }
    if (params.constructor != Array) {
        params = [params];
    }
    $.each(params, function (i, n) {
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
    });
    return source;
};



utils.toJson = function (html) { 
    return eval('(' + html + ')');
} //字符串转json

utils.createTable = function (option) {
    //opt.colums(id,code,name),id may hide
    var opt = option || {};


}   //根据json数据生成表格

utils.kendo = {};
utils.kendo.setloading = function (el) {
    var x = el.data('kendoDropDownList');
    if (x && x.arrow) x.arrow.addClass('k-loading');
} //设置为加载状态

utils.kendo.setunload = function (el) {
    var x = el.data('kendoDropDownList');
    if (x && x.arrow) x.arrow.removeClass('k-loading');

}  //设置为不加载状态

utils.kendo.removedropdown = function (el) {
    $("div[id*='" + el.attr('id') + "']").remove();
    $('div.k-animation-container')
    .each(function () {
        var c = $(this).children("div[id*='" + el.attr('id') + "']");
        if (c.length > 0) {
            $(this).remove();
        }
    });
}
utils.kendo.setenable = function (set, enable) {
    set.each(function () {
        $(this).data('kendoDropDownList').enable(enable);
    });
}

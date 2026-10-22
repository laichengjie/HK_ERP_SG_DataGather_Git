jQuery(function ($) {
    $('.var-solution-lis')
  .each(function () {
      setMaxHeight($(this));
  });
});
//删除店铺
function deleteSolution(anchor) {
    utils.dialog({
        html: '您确定要删除该方案吗？',
        icon: 'warning',
        create: function (yuan) {
            yuan.title.focus();
        },
        buttons: {
            '确定': function (yuan) {

                var $a = $(anchor);
                var solutionID =
                $a
                .parent()
                .parent()
                .find('span.solutionID:eq(0)')
                .html();

                var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
                $.ajax({
                    type: "POST",
                    url: '/admin/gat_solution_browse.aspx',
                    timeout: 50000,
                    data: { mode: 'DeleteSolution', SolutionID: solutionID},
                    beforeSend: function () {
                        $op
                        .html('')
                        .append($('<i class="loading">&nbsp;</i>'));
                        yuan.disable();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        var message = '&nbsp;&nbsp;删除失败';
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
                            .appendTo($('.var-solution-lis').html(''));
                            setMaxHeight($('div.var-solution-lis:eq(0)'));
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
//停用 启用
function setSolution(anchor) {
    var $a = $(anchor);
    var show_text = $a.html();
    utils.dialog({
        html: '您确定要' + show_text + '该方案吗？',
        icon: 'warning',
        create: function (yuan) {
            yuan.title.focus();
        },
        buttons: {
            '确定': function (yuan) {
                var solutionID =$a
                .parent()
                .parent()
                .find('span.solutionID:eq(0)')
                .html();

                var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
                $.ajax({
                    type: "POST",
                    url: '/admin/gat_solution_browse.aspx',
                    timeout: 50000,
                    data: { mode: 'UpdateSolutionEnabled', SolutionID: solutionID },
                    beforeSend: function () {
                        $op
                        .html('')
                        .append($('<i class="loading">&nbsp;</i>'));
                        yuan.disable();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        var message = '&nbsp;&nbsp;' + show_text + '失败';
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
                            .appendTo($('.var-solution-lis').html(''));
                            setMaxHeight($('div.var-solution-lis:eq(0)'));
                        } else {
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


//设置高度
function setMaxHeight(element) {

    //var maxHeight = 9000;
    //element
    //.css({
    //    'height': 'auto',
    //    'overflow-y': 'hidden'
    //});
    //if (element.height() > maxHeight) {
    //    element
    //    .css({
    //        'height': maxHeight,
    //        'overflow-y': 'scroll'
    //    });
    //}
}
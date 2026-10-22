
/* 提报方案脚本
 * 功能：
 * author : @HK.tian_naixiang
 */

jQuery(function ($) {

    /* ----------------------------
    * @表单校验
    * --------------------------*/
    $('#form1').validate({
        errorElement: 'div',
        errorClass: 'errormt',
        errorPlacement: function (label, el) {
            el
                .parents('td.valid:eq(0)')
                .append(label);
        },
        rules: {
            'ctl00$cpBody$txtRepeatStartTime': {
                required: true,
                longdatetime: true
            }
        },
        messages: {
            'ctl00$cpBody$txtRepeatStartTime': {
                required: '取数开始时间不能为空'
            }
        }
    });

    /* -----------------------------------
    * @提交表单，检查是否选择公司、供应商和品牌
    * -----------------------------------*/
    $('#form1').bind('submit', function (e) {

        var company = $ddlCompany.data('kendoDropDownList').value();
        var vendor = $ddlVendor.data('kendoDropDownList').value();
        var card = $ddlCard.data('kendoDropDownList').value();

        var nullList = ['null', 'undefined', '0'];
        if ($.inArray(company, nullList) !== -1) {
            utils.dialog({
                html: '请先选择公司！',
                icon: 'warning'
            });
            return false;
        }

        if ($.inArray(vendor, nullList) !== -1) {
            utils.dialog({
                html: '请先选择供应商！',
                icon: 'warning'
            });
            return false;
        }

        if ($.inArray(card, nullList) !== -1) {
            utils.dialog({
                html: '请先选择品牌！',
                icon: 'warning'
            });
            return false;
        }

        //记下公司、供应商和品牌的值
        $hfCompany.val(company);
        $hfCard.val(card);
        $hfVendor.val(vendor);
        //alert(card);
        //sreturn false;

        //选中的店铺
        var shopcodeList = [];
        var shopcodeAllList = [];//存放所有的店铺ID，已减少范围
        var gv = $('.bas_shopright .qp-gridview');
        var $check = gv.find(":checkbox[name$='chkSelect']");
        $check.each(function () {
            var me = $(this);
            var code = me.prev().html(); //店铺ID
            if (me.is(':checked')) shopcodeList.push(code);//选择的数据
            shopcodeAllList.push(code);
            //scope.push(code);
        });
        var stockcodeList = [];
        var stockcodeAllList = [];//存放所有的店铺ID，已减少范围
        var gvStock = $('.bas_stockright .qp-gridview');
        var $checkStock = gvStock.find(":checkbox[name$='chkStockSelect']");
        $checkStock.each(function () {
            var meStock = $(this);
            var codeStock = meStock.prev().html(); //仓库ID
            if (meStock.is(':checked')) stockcodeList.push(codeStock);//选择的数据
            stockcodeAllList.push(codeStock);
            //scope.push(code);
        });
        //店铺
        $hfallowList.val(shopcodeList.join(',') + "|" + shopcodeAllList.join(','));
        //仓库
        $hfStockList.val(stockcodeList.join(',') + "|" + stockcodeAllList.join(','));
        return true;
    });

    /* 水印 */
    $txtShopCode.watermark("请输入店铺代码", "watermark2"); //水印



    //店铺查询

    $('#txtShopSearchCode').watermark("请输入店铺代码", "watermark2"); //水印
    $('#searchButton').click(function () {
        var company = $ddlCompany.data('kendoDropDownList').value();
        var vendor = $ddlVendor.data('kendoDropDownList').value();
        var card = $ddlCard.data('kendoDropDownList').value();
        var shopcode = $('#txtShopSearchCode').val();
        //调用ajax来刷新数据
        $.ajax({
            type: "POST",
            url: '/admin/gat_solution.aspx',
            timeout: 50000,
            //组合条件传到后台
            data: { mode: 'SelectShopData', companyID: company, vendorID: vendor, cardID: card, searchcode: encodeURI(shopcode) },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            success: function (html) {

                //刷新
                var json = utils.toJson(html);
                if (json.StatusCode === '1') {

                    $('#gv-shop')
                        .empty()
                        .append(json.Html);
                    utils.undialog();

                } else {
                    $op
                        .html('')
                        .addClass("warning")
                        .html('<i class="warning">&nbsp;</i>' + json.Html);
                }
            },
            complete: function (jqXHR, textStatus) {
                initCheck();
            }

        });


    });

    //仓库查询
    $('#txtStockSearchCode').watermark("请输入仓库代码", "watermark2"); //水印
    $('#serachButtonStock').click(function () {
        var company = $ddlCompany.data('kendoDropDownList').value();
        var vendor = $ddlVendor.data('kendoDropDownList').value();
        var card = $ddlCard.data('kendoDropDownList').value();
        var Stockcode = $('#txtStockSearchCode').val();
        //调用ajax来刷新数据
        $.ajax({
            type: "POST",
            url: '/admin/gat_solution.aspx',
            timeout: 50000,
            //组合条件传到后台
            data: { mode: 'SelectStockData', companyID: company, vendorID: vendor, cardID: card, searchcode: encodeURI(Stockcode) },
            beforeSend: function () {
                loading = true;
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            success: function (html) {

                //刷新
                var json = utils.toJson(html);
                if (json.StatusCode === '1') {

                    $('#gv-stockID')
                        .empty()
                        .append(json.Html);
                    utils.undialog();

                } else {
                    $op
                        .html('')
                        .addClass("warning")
                        .html('<i class="warning">&nbsp;</i>' + json.Html);
                }
            },
            complete: function (jqXHR, textStatus) {
                initCheck();
            }

        });



        //jquery.ui.botton
        var $uibutton = $('button.ui-icon-save');
        if ($uibutton.length > 0) {
            $uibutton
                .eq(0)
                .css('margin-left', $uibutton.length > 1 ? '100px' : '320px')
                .button({
                    icons: {
                        primary: ($uibutton.length > 1 ? 'ui-icon-plus' : 'ui-icon-disk')
                    }
                })
                .click(function () {
                    if ($uibutton.length > 1) {
                        location.href = '/solution/add';
                    } else {
                        $(":submit[id$='btnSave']").trigger('click');
                    }
                })
                .next('button.ui-icon-save')
                .css('margin-left', '10px')
                .button({
                    icons: {
                        primary: 'ui-icon-newwin'
                    }
                })
                .click(function (e) {
                    manualGat(e);
                })
                .next('button.ui-icon-save')
                .css('margin-left', '10px')
                .button({
                    icons: {
                        primary: 'ui-icon-disk'
                    }
                })
                .click(function () {
                    //alert('');
                    $(":submit[id$='btnSave']").trigger('click');
                })
                .next('button.ui-icon-save')
                .css('margin-left', '10px')
                .button({

                })
                .click(function (e) {
                    $(":submit[id$='btnExport']").trigger('click');
                });
        }

        //初始化复选框的选中状态
        initCheck();

    });

    /*$kendocompany.kendoDropDownList({
    open: function () {
    if (body.height() <= win.height()) $(document.body).addClass('overflow');
    },
    close: function () {
    body.removeClass('overflow');
    },
    change: function (e) {
    //保存按钮不可用
    //$('button.save').unbind('click', save);

    var company = $kendocompany.data('kendoDropDownList').value();
    if (company === '0') return false;

    $.ajax({
    type: "POST",
    url: '/admin/gat_solution.aspx',
    timeout: 50000,
    data: "mode=loadvendor&company=" + company,
    beforeSend: function () {
    utils.kendo.setloading($kendovendor);
    },
    error: function (jqXHR, textStatus, errorThrown) {

    },
    success: function (html) {
    var json = utils.toJson(html);
    var kendo_vendor = $kendovendor.data('kendoDropDownList');
    utils.kendo.removedropdown($kendovendor);

    var kendo_td_vendor = kendo_vendor.wrapper.parent();
    kendo_td_vendor
    .empty()
    .append(json.Html);

    $kendovendor = kendo_td_vendor.find('select:eq(0)'); //.css('display', 'none');
    $kendovendor.kendoDropDownList({
    open: function () {
    if (body.height() <= win.height()) $(document.body).addClass('overflow');
    },
    close: function () {
    body.removeClass('overflow');
    }, change: function (e) {
    var vendor = $kendovendor.data('kendoDropDownList').value();
    if (vendor === '0') return false;

    //开始加载品牌
    $.ajax({
    type: "POST",
    url: '/admin/gat_solution.aspx',
    timeout: 50000,
    data: "mode=loadcard&company=" + company + "&vendor=" + vendor,
    beforeSend: function () {
    utils.kendo.setloading($kendocard);
    },
    error: function (jqXHR, textStatus, errorThrown) {
    },
    success: function (ai) {
    var aiJson = utils.toJson(ai);
    //alert(aiJson.Html);
    var kendo_card = $kendocard.data('kendoDropDownList');
    utils.kendo.removedropdown($kendocard);

    var kendo_td_card = kendo_card.wrapper.parent();
    kendo_td_card
    .empty()
    .append(aiJson.Html);
    ///alert(aiJson.Html);

    $kendocard = kendo_td_card.find('select:eq(0)');
    $kendocard.kendoDropDownList({
    open: function () {
    if ($(document).height() <= $(window).height()) $(document.body).addClass('overflow');
    },
    close: function () {
    $(document.body).removeClass('overflow');
    },
    change: function () {
    }
    });
    },
    complete: function (jqXHR, textStatus) {
    utils.kendo.setunload($kendocard);
    }

    }); //AJAX 品牌

    } //change
    });

    //清空供应商的同时也清空品牌
    var kendo_card = $kendocard.data('kendoDropDownList');
    utils.kendo.removedropdown($kendocard);

    var kendo_td_card = kendo_card.wrapper.parent();
    kendo_td_card
    .empty()
    .append(json.Message)
    $kendocard = kendo_td_card.find('select:eq(0)');
    $kendocard.kendoDropDownList();
    }, //请求供应商完毕
    complete: function (jqXHR, textStatus) {
    utils.kendo.setunload($kendovendor);
    }
    }); //请求供应商
    }

    }); //公司下拉

    $kendovendor.kendoDropDownList({
    open: function () {
    if (body.height() <= win.height()) $(document.body).addClass('overflow');
    },
    close: function () {
    body.removeClass('overflow');
    }, change: function (e) {
    var company = $kendocompany.data('kendoDropDownList').value();
    var vendor = $kendovendor.data('kendoDropDownList').value();
    if (vendor === '0') return false;

    //开始加载品牌
    $.ajax({
    type: "POST",
    url: '/admin/gat_solution.aspx',
    timeout: 50000,
    data: "mode=loadcard&company=" + company + "&vendor=" + vendor,
    beforeSend: function () {
    utils.kendo.setloading($kendocard);
    },
    error: function (jqXHR, textStatus, errorThrown) {
    },
    success: function (ai) {
    var aiJson = utils.toJson(ai);
    //alert(aiJson.Html);
    var kendo_card = $kendocard.data('kendoDropDownList');
    utils.kendo.removedropdown($kendocard);

    var kendo_td_card = kendo_card.wrapper.parent();
    kendo_td_card
    .empty()
    .append(aiJson.Html);
    ///alert(aiJson.Html);

    $kendocard = kendo_td_card.find('select:eq(0)');
    $kendocard.kendoDropDownList({
    open: function () {
    if ($(document).height() <= $(window).height()) $(document.body).addClass('overflow');
    },
    close: function () {
    $(document.body).removeClass('overflow');
    },
    change: function () {
    }
    });
    },
    complete: function (jqXHR, textStatus) {
    utils.kendo.setunload($kendocard);
    }

    }); //AJAX 品牌

    } //change
    }); //供应商下拉*/

    createCompanyDropdown(); //公司下拉
    createVendDropdown();    //供应商下拉
    createBrandDropdown(); //品牌下拉

    $('input.datepicker')
        .eq(0)
        .css('width', '299px')
        .end()
        .bind('focus', function () {
            $(this).css('border', 'none')
        }).kendoDatePicker({
            format: 'yyyy-MM-dd HH:mm:ss',
            min: new Date(2000, 01, 01),
            open: function () {
                body.addClass('overflow');
            },
            close: function () {
                body.removeClass('overflow');
            }
        });

    //查询
    $('#vendquery').click(function () {
        var company = $ddlCompany.data('kendoDropDownList').value();
        var vendor = $ddlVendor.data('kendoDropDownList').value();
        var card = $ddlCard.data('kendoDropDownList').value();
        var shopcode = $txtShopCode.val();
        //alert(company + vendor + card);
        loadShopRight(company, vendor, card, shopcode);
    });


    //jquery.ui.botton
    var $uibutton = $('button.ui-icon-save');
    if ($uibutton.length > 0) {
        $uibutton
            .eq(0)
            .css('margin-left', $uibutton.length > 1 ? '100px' : '320px')
            .button({
                icons: {
                    primary: ($uibutton.length > 1 ? 'ui-icon-plus' : 'ui-icon-disk')
                }
            })
            .click(function () {
                if ($uibutton.length > 1) {
                    location.href = '/solution/add';
                } else {
                    $(":submit[id$='btnSave']").trigger('click');
                }
            })
            .next('button.ui-icon-save')
            .css('margin-left', '10px')
            .button({
                icons: {
                    primary: 'ui-icon-newwin'
                }
            })
            .click(function (e) {
                manualGat(e);
            })
            .next('button.ui-icon-save')
            .css('margin-left', '10px')
            .button({
                icons: {
                    primary: 'ui-icon-disk'
                }
            })
            .click(function () {
                //alert('');
                $(":submit[id$='btnSave']").trigger('click');
            })
            .next('button.ui-icon-save')
            .css('margin-left', '10px')
            .button({

            })
            .click(function (e) {
                $(":submit[id$='btnExport']").trigger('click');
            });
    }


    //不上传货品范围导入文件格式
    var materialRangeImportFileClick = false;
    $("#btnMaterialRangeImportFile").click(function () {
        if (materialRangeImportFileClick) {
            return false;
        }
        materialRangeImportFileClick = true;
        try {
            var url = '/admin/gat_solution.aspx?mode=materialRangeImportFile';
            var xhr = new XMLHttpRequest();
            xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
            xhr.responseType = "blob";  // 返回类型blob
            // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
            xhr.onload = function () {
                // 请求完成
                if (this.status === 200) {
                    // 返回200
                    var blob = this.response;
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                    reader.onload = function (e) {
                        // 转换完成，创建一个a标签用于下载
                        var a = document.createElement('a');
                        a.download = '不上传货品范围导入.xlsx';
                        a.href = e.target.result;
                        $("body").append(a);  // 修复firefox中无法触发click
                        a.click();
                        $(a).remove();
                    }
                }
            };
            xhr.send();
        } catch (e) {
            alert(e.message);
        }
        materialRangeImportFileClick = false;
    });
    //不上传货品范围导入
    var materialRangeImportClick = false;
    $("#btnMaterialRangeImport").click(function () {
        if (materialRangeImportClick) {
            return false;
        }
        materialRangeImportClick = true;
        var $file1 = $("input[name='materialRangeFileName']").val();//用户文件内容(文件)
        // 判断文件是否为空 
        if ($file1 == "") {
            alert("请选择上传的目标文件! ")
            return false;
        }
        //判断文件类型,我这里根据业务需求判断的是Excel文件
        var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
        if (fileName1 != "xls" && fileName1 != "xlsx") {
            alert("请选择Execl文件!");
            return false;
        }
        //判断文件大小
        var size1 = $("input[name='materialRangeFileName']")[0].files[0].size;
        if (size1 > 104857600) {
            alert("上传文件不能大于100M!");
            return false;
        }

        boo1 = true;
        var type = "file";
        var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
        formData.append(type, $("#materialRangeFileName")[0].files[0]);
        $.ajax({
            url: "/admin/gat_solution.aspx?mode=materialRangeImport&solutionId=" + $("#solutionId").val(),
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            datatype: "json",
            async: false,
            success: function (html) {
                //if (data.lastIndexOf("成功")) {
                //    $("#cpBody_doRefreshMaterialRangeGrid")[0].click();
                //}//刷新
                var json = utils.toJson(html);
                if (json.StatusCode === '1') {
                    $('#gv-MaterialRange')
                        .empty()
                        .append(json.Html);

                } else {
                    //$op
                    //    .html('')
                    //    .addClass("warning")
                    //    .html('<i class="warning">&nbsp;</i>' + json.Html);
                    alert(json.Html);
                }
                utils.undialog();
            },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            complete: function () {
                initCheck();
            }
        });
        materialRangeImportClick = false;
    })

    //特殊货品范围导入文件格式
    var specialMaterialRangeImportFileClick = false;
    $("#btnSpecialMaterialRangeImportFile").click(function () {
        if (specialMaterialRangeImportFileClick) {
            return false;
        }
        specialMaterialRangeImportFileClick = true;
        try {
            var url = '/admin/gat_solution.aspx?mode=specialMaterialRangeImportFile';
            var xhr = new XMLHttpRequest();
            xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
            xhr.responseType = "blob";  // 返回类型blob
            // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
            xhr.onload = function () {
                // 请求完成
                if (this.status === 200) {
                    // 返回200
                    var blob = this.response;
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                    reader.onload = function (e) {
                        // 转换完成，创建一个a标签用于下载
                        var a = document.createElement('a');
                        a.download = '特殊货品范围导入.xlsx';
                        a.href = e.target.result;
                        $("body").append(a);  // 修复firefox中无法触发click
                        a.click();
                        $(a).remove();
                    }
                }
            };
            xhr.send();
        } catch (e) {
            alert(e.message);
        }
        specialMaterialRangeImportFileClick = false;
    })

    //特殊货品范围导入
    var specialMaterialRangeImportClick = false;
    $("#btnSpecialMaterialRangeImport").click(function () {
        if (specialMaterialRangeImportClick) {
            return false;
        }
        specialMaterialRangeImportClick = true;
        var $file1 = $("input[name='specialMaterialRangeFileName']").val();//用户文件内容(文件)
        // 判断文件是否为空 
        if ($file1 == "") {
            alert("请选择上传的目标文件! ")
            return false;
        }
        //判断文件类型,我这里根据业务需求判断的是Excel文件
        var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
        if (fileName1 != "xls" && fileName1 != "xlsx") {
            alert("请选择Execl文件!");
            return false;
        }
        //判断文件大小
        var size1 = $("input[name='specialMaterialRangeFileName']")[0].files[0].size;
        if (size1 > 104857600) {
            alert("上传文件不能大于100M!");
            return false;
        }

        boo1 = true;
        var type = "file";
        var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
        formData.append(type, $("#specialMaterialRangeFileName")[0].files[0]);
        $.ajax({
            url: "/admin/gat_solution.aspx?mode=specialMaterialRangeImport&solutionId=" + $("#solutionId").val(),
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            datatype: "json",
            async: false,
            success: function (html) {
                //if (data.lastIndexOf("成功")) {
                //    $("#cpBody_doRefreshSpecialMaterialRangeGrid")[0].click();
                //}//刷新
                var json = utils.toJson(html);
                if (json.StatusCode === '1') {

                    $('#gv-specialMaterialRange')
                        .empty()
                        .append(json.Html);

                } else {
                    //$op
                    //    .html('')
                    //    .addClass("warning")
                    //    .html('<i class="warning">&nbsp;</i>' + json.Html);
                    alert(json.Html);
                }
                utils.undialog();
            },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            complete: function () {
                initCheck();
            }
        });
        specialMaterialRangeImportClick = false;
    })


    //货品折扣导入文件格式
    var materialDiscountImportFileClick = false;
    $("#btnMaterialDiscountImportFile").click(function () {
        if (materialDiscountImportFileClick) {
            return false;
        }
        materialDiscountImportFileClick = true;
        try {
            var url = '/admin/gat_solution.aspx?mode=materialDiscountImportFile';
            var xhr = new XMLHttpRequest();
            xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
            xhr.responseType = "blob";  // 返回类型blob
            // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
            xhr.onload = function () {
                // 请求完成
                if (this.status === 200) {
                    // 返回200
                    var blob = this.response;
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                    reader.onload = function (e) {
                        // 转换完成，创建一个a标签用于下载
                        var a = document.createElement('a');
                        a.download = '货品折扣导入.xlsx';
                        a.href = e.target.result;
                        $("body").append(a);  // 修复firefox中无法触发click
                        a.click();
                        $(a).remove();
                    }
                }
            };
            xhr.send();
        } catch (e) {
            alert(e.message);
        }
        materialDiscountImportFileClick = false;
    });
    //货品折扣导入
    var materialDiscountImportClick = false;
    $("#btnMaterialDiscountImport").click(function () {
        if (materialDiscountImportClick) {
            return false;
        }
        materialDiscountImportClick = true;
        var $file1 = $("input[name='materialDiscountFileName']").val();//用户文件内容(文件)
        // 判断文件是否为空 
        if ($file1 == "") {
            alert("请选择上传的目标文件! ")
            return false;
        }
        //判断文件类型,我这里根据业务需求判断的是Excel文件
        var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
        if (fileName1 != "xls" && fileName1 != "xlsx") {
            alert("请选择Execl文件!");
            return false;
        }
        //判断文件大小
        var size1 = $("input[name='materialDiscountFileName']")[0].files[0].size;
        if (size1 > 104857600) {
            alert("上传文件不能大于100M!");
            return false;
        }

        boo1 = true;
        var type = "file";
        var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
        formData.append(type, $("#materialDiscountFileName")[0].files[0]);
        $.ajax({
            url: "/admin/gat_solution.aspx?mode=materialDiscountImport&solutionId=" + $("#solutionId").val(),
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            datatype: "json",
            async: false,
            success: function (html) {
                //if (data.lastIndexOf("成功")) {
                //    $("#cpBody_doRefreshMaterialRangeGrid")[0].click();
                //}//刷新
                //console.info("success开始");
                //console.info(html);
                var json = utils.toJson(html);
                /*console.info(json);*/
                if (json.StatusCode === '1') {
                    $('#gv-MaterialDiscount')
                        .empty()
                        .append(json.Html);

                } else {
                    //$op
                    //    .html('')
                    //    .addClass("warning")
                    //    .html('<i class="warning">&nbsp;</i>' + json.Html);
                    alert(json.Html);
                }
                utils.undialog();
            },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            complete: function () {
                initCheck();
            }
        });
        materialDiscountImportClick = false;
    })


    //仓库子款型导入文件格式
    var stockSubModelSetImportFileClick = false;
    $("#btnStockSubModelSetImportFile").click(function () {
        if (stockSubModelSetImportFileClick) {
            return false;
        }
       
        stockSubModelSetImportFileClick = true;
        try {
            var url = '/admin/gat_solution.aspx?mode=stockSubModelSetImportFile';
            var xhr = new XMLHttpRequest();
            xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
            xhr.responseType = "blob";  // 返回类型blob
            // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
            //console.info("btnMaterialDiscountImportFile开始2222");
            xhr.onload = function () {
                // 请求完成
                if (this.status === 200) {
                    // 返回200
                    var blob = this.response;
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                    reader.onload = function (e) {
                        // 转换完成，创建一个a标签用于下载
                        var a = document.createElement('a');
                        a.download = '仓库子款型导入.xlsx';
                        a.href = e.target.result;
                        $("body").append(a);  // 修复firefox中无法触发click
                        a.click();
                        $(a).remove();
                    }
                }
            };
            xhr.send();
        } catch (e) {
            alert(e.message);
        }
        stockSubModelSetImportFileClick = false;
    });
    //仓库子款型导入
    var stockSubModelSetImportClick = false;
    $("#btnStockSubModelSetImport").click(function () {
        if (stockSubModelSetImportClick) {
            return false;
        }
        stockSubModelSetImportClick = true;
        var $file1 = $("input[name='stockSubModelSetFileName']").val();//用户文件内容(文件)
        // 判断文件是否为空 
        if ($file1 == "") {
            alert("请选择上传的目标文件! ")
            return false;
        }
        //判断文件类型,我这里根据业务需求判断的是Excel文件
        var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
        if (fileName1 != "xls" && fileName1 != "xlsx") {
            alert("请选择Execl文件!");
            return false;
        }
        //判断文件大小
        var size1 = $("input[name='stockSubModelSetFileName']")[0].files[0].size;
        if (size1 > 104857600) {
            alert("上传文件不能大于100M!");
            return false;
        }

        boo1 = true;
        var type = "file";
        var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
        formData.append(type, $("#stockSubModelSetFileName")[0].files[0]);
        $.ajax({
            url: "/admin/gat_solution.aspx?mode=stockSubModelSetImport&solutionId=" + $("#solutionId").val(),
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            datatype: "json",
            async: false,
            success: function (html) {
                //if (data.lastIndexOf("成功")) {
                //    $("#cpBody_doRefreshMaterialRangeGrid")[0].click();
                //}//刷新
                //console.info("success开始");
                //console.info(html);
                var json = utils.toJson(html);
                /*console.info(json);*/
                if (json.StatusCode === '1') {
                    $('#gv-StockSubModelSet')
                        .empty()
                        .append(json.Html);

                } else {
                    //$op
                    //    .html('')
                    //    .addClass("warning")
                    //    .html('<i class="warning">&nbsp;</i>' + json.Html);
                    alert(json.Html);
                }
                utils.undialog();
            },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            complete: function () {
                initCheck();
            }
        });
        stockSubModelSetImportClick = false;
    })


    //网店归属线下店铺POS设置文件格式
    var OnlineShopSetImportFileClick = false;
    $("#btnOnlineShopSetImportFile").click(function () {
        if (OnlineShopSetImportFileClick) {
            return false;
        }

        OnlineShopSetImportFileClick = true;
        try {
            var url = '/admin/gat_solution.aspx?mode=OnlineShopSetImportFile';
            var xhr = new XMLHttpRequest();
            xhr.open('post', url, true);    // 也可以使用POST方式，根据接口
            xhr.responseType = "blob";  // 返回类型blob
            // 定义请求完成的处理函数，请求前也可以增加加载框/禁用下载按钮逻辑
            //console.info("btnMaterialDiscountImportFile开始2222");
            xhr.onload = function () {
                // 请求完成
                if (this.status === 200) {
                    // 返回200
                    var blob = this.response;
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);  // 转换为base64，可以直接放入a表情href
                    reader.onload = function (e) {
                        // 转换完成，创建一个a标签用于下载
                        var a = document.createElement('a');
                        a.download = '网店归属线下店铺导入.xlsx';
                        a.href = e.target.result;
                        $("body").append(a);  // 修复firefox中无法触发click
                        a.click();
                        $(a).remove();
                    }
                }
            };
            xhr.send();
        } catch (e) {
            alert(e.message);
        }
        OnlineShopSetImportFileClick = false;
    });
    //网店归属线下店铺POS设置导入
    var OnlineShopSetImportClick = false;
    $("#btnOnlineShopSetImport").click(function () {
        if (OnlineShopSetImportClick) {
            return false;
        }
        OnlineShopSetImportClick = true;
        var $file1 = $("input[name='OnlineShopSetFileName']").val();//用户文件内容(文件)
        // 判断文件是否为空 
        if ($file1 == "") {
            alert("请选择上传的目标文件! ")
            return false;
        }
        //判断文件类型,我这里根据业务需求判断的是Excel文件
        var fileName1 = $file1.substring($file1.lastIndexOf(".") + 1).toLowerCase();
        if (fileName1 != "xls" && fileName1 != "xlsx") {
            alert("请选择Execl文件!");
            return false;
        }
        //判断文件大小
        var size1 = $("input[name='OnlineShopSetFileName']")[0].files[0].size;
        if (size1 > 104857600) {
            alert("上传文件不能大于100M!");
            return false;
        }

        boo1 = true;
        var type = "file";
        var formData = new FormData();//这里需要实例化一个FormData来进行文件上传
        formData.append(type, $("#OnlineShopSetFileName")[0].files[0]);
        $.ajax({
            url: "/admin/gat_solution.aspx?mode=OnlineShopSetImport&solutionId=" + $("#solutionId").val(),
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            datatype: "json",
            async: false,
            success: function (html) {
                //if (data.lastIndexOf("成功")) {
                //    $("#cpBody_doRefreshMaterialRangeGrid")[0].click();
                //}//刷新
                //console.info("success开始");
                //console.info(html);
                var json = utils.toJson(html);
                /*console.info(json);*/
                if (json.StatusCode === '1') {
                    $('#gv-OnlineShopSet')
                        .empty()
                        .append(json.Html);

                } else {
                    //$op
                    //    .html('')
                    //    .addClass("warning")
                    //    .html('<i class="warning">&nbsp;</i>' + json.Html);
                    alert(json.Html);
                }
                utils.undialog();
            },
            beforeSend: function () {
                utils.dialog({
                    showHeader: false,
                    html: '正在查询...',
                    icon: 'loading'
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '&nbsp;查询失败';
                if (textStatus === 'timeout') {
                    message += '，消息：加载超时。请稍后重试！';
                } else {
                    message += "，请稍后重试！";
                }
                $op
                    .html('')
                    .addClass("warning")
                    .html('<i class="warning">&nbsp;</i>' + message);
            },
            complete: function () {
                initCheck();
            }
        });
        OnlineShopSetImportClick = false;
    })

    //初始化复选框的选中状态
    initCheck();
});

//窗口创建时初始化选中状态
function initCheck() {

    var gv = $("table[id$='gvShop']");

    var $checkboxs = gv.find(':checkbox');
    var $checkAll = gv.find(":checkbox[id$='chkSelectAll']");
    var $check = gv.find(":checkbox[name$='chkSelect']");
    //alert(gv.length)

    if ($check.filter(':checked').length === $check.length) $checkAll.attr('checked', 'checked');

    $checkAll.change(function () {
        if ($(this).is(':checked')) {
            $check.attr('checked', 'checked');
        } else {
            $check.attr('checked', false);
        }
    });

    //初始化选择项 crb
    var gvStock = $("table[id$='gvStock']");

    var $checkStockboxs = gvStock.find(':checkbox');
    var $checkStockAll = gvStock.find(":checkbox[id$='chkStockSelectAll']");
    var $checkStock = gvStock.find(":checkbox[name$='chkStockSelect']");
    //alert(gv.length)

    if ($checkStock.filter(':checked').length === $checkStock.length) $checkStockAll.attr('checked', 'checked');

    $checkStockAll.change(function () {
        if ($(this).is(':checked')) {
            $checkStock.attr('checked', 'checked');
        } else {
            $checkStock.attr('checked', false);
        }
    });

    var gvMaterialRange = $("table[id$='gvMaterialRange']");

    var $checkboxMaterialRanges = gvMaterialRange.find(':checkbox');
    var $checkAllMaterialRange = gvMaterialRange.find(":checkbox[id$='MaterialRangeSelectAll']");
    var $checkMaterialRange = gvMaterialRange.find(":checkbox[name$='MaterialRangeCheckBox']");
    //alert(gv.length)

    if ($checkMaterialRange.filter(':checked').length === $check.length) $checkAllMaterialRange.attr('checked', 'checked');

    $checkAllMaterialRange.change(function () {
        if ($(this).is(':checked')) {
            $checkMaterialRange.attr('checked', 'checked');
        } else {
            $checkMaterialRange.attr('checked', false);
        }
    });

    var gvSpecialMaterialRange = $("table[id$='gvSpecialMaterialRange']");

    var $checkboxSpecialMaterialRanges = gvSpecialMaterialRange.find(':checkbox');
    var $checkAllSpecialMaterialRange = gvSpecialMaterialRange.find(":checkbox[id$='SpecialMaterialRangeSelectAll']");
    var $checkSpecialMaterialRange = gvSpecialMaterialRange.find(":checkbox[name$='SpecialMaterialRangeCheckBox']");
    //alert(gv.length)

    if ($checkSpecialMaterialRange.filter(':checked').length === $check.length) $checkAllMaterialRange.attr('checked', 'checked');

    $checkAllSpecialMaterialRange.change(function () {
        if ($(this).is(':checked')) {
            $checkSpecialMaterialRange.attr('checked', 'checked');
        } else {
            $checkSpecialMaterialRange.attr('checked', false);
        }
    });
}

//手动提报
function manualGat(el) {

    var ui = $('#bas_manual .wrapper').clone();

    var $kendovendor = $("select[name$='ddlVendor']");
    var $kendocompany = $("select[name$='ddlCompany']");
    var $kendocard = $("select[name$='ddlCard']");

    var company = $ddlCompany.data('kendoDropDownList').value();
    var vendor = $ddlVendor.data('kendoDropDownList').value();
    var card = $ddlCard.data('kendoDropDownList').value();

    utils.dialog({
        html: ui,
        title: '补采数据',
        icon: 'loading',
        width: 350,
        hClose: true,
        create: function (yuan) {
            $(':text', yuan.body)
                .each(function (i) {

                    var value = $(el).prevAll('span'); //存储时间的值
                    $(this)
                        .css('border', 'none')
                        .kendoDatePicker({
                            format: 'yyyy-MM-dd HH:mm:ss',
                            min: new Date(2000, 01, 01),
                            //value: new Date(value.eq(i).html()),
                            open: function () {
                                body.addClass('overflow');
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
                var solutionId = $("#solutionId").val();
                //开始ajax保存
                $.ajax({
                    type: "POST",
                    url: '/admin/AJAXHandler.ashx',
                    timeout: 1800000,
                    data: "mode=gat&start=" + start + "&end=" + end + "&vendor=" + vendor + "&company=" + company + "&card=" + card + "&solutionId=" + solutionId,
                    beforeSend: function () {
                        $op
                            .html('')
                            .append($('<i class="loading">&nbsp;</i>'));
                        yuan.disable();
                        //$('a.yuan-botton', yuan.bottom).eq(0).css('display','none');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $op.html('');
                        var message = '&nbsp;补采失败';
                        if (textStatus === 'timeout') {
                            message += '，消息3：加载超时，请稍后重试！';
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
                                .html('<i class="correct">&nbsp;</i>' + json.Html);

                            //utils.undialog();
                            //var x = $('a.yuan-botton', yuan.bottom).css({  display: 'none'});
                            //$('a.yuan-botton', yuan.bottom).eq(0).unbind('click').remove();
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
            }/*,
            '取消': utils.undialog*/
        }
    });

}




/// <summary>
/// 关联店铺
/// </summary>
/// <param>anchor:点击关联的链接</param>
function shopMap(anchor) {
    var me = $(anchor);
    var shop = me.prev('span.vendshop').eq(0).html();

    var ui = $('#bas_vendshop .wrapper').clone();
    utils.dialog({
        html: ui,
        height: 500,
        width: 400,
        title: '关联供应商的店铺',
        loading: true,
        buttons: {
            '确定': function (yuan) {
                saveShopMap(yuan, anchor);
            },
            '取消': utils.undialog
        },
        create: function (yuan) {
            //yuan.setHtml({
            //    html: ui
            //});

            var gv = $('table.qp-gridview:eq(0)', yuan.body);

            var $checks = gv.find(":checkbox[name$='chkSelect']");
            var tmp = 0;
            $checks.each(function () {
                var check = $(this);
                tmp = check.prev('span').eq(0).html();
                if (tmp === shop) check.attr('checked', 'checked');

                //绑定change事件
                check
                    .change(function () {
                        $checks.each(function () {
                            if ($(this).attr('id') !== check.attr('id')) {
                                $(this).attr('checked', false); //单选
                            }
                        });

                    });
            });



            $('#txtVendShopCode', yuan.body).watermark("请输入店铺代码", "watermark2"); //水印

            var empty = $('div.data-empty', yuan.body);
            if (empty.length > 0) {
                yuan.bottom.remove();
            }

            //查询
            yuan.body.find('a.func-query')
                .click(function () {
                    var $vc = $('#txtVendShopCode', yuan.body); //品牌代码
                    /*if ($.trim($vc.val()) === '') {
                        return false;
                    }*/
                    gotoQuery(yuan, anchor); //查询
                });
        }
    });
}

var shopMapMultipleSelect = [];

/// <summary>
/// 关联店铺(多选)
/// </summary>
/// <param>anchor:点击关联的链接</param>
function shopMapMultiple(anchor) {
    var me = $(anchor);
    var spanOMSShopId = me.next('span.OMSShopId').eq(0);
    var shop = spanOMSShopId.html();
    var shops = shop.split(',');
    shopMapMultipleSelect = shops;
    var ui = $('#bas_shop .wrapper').clone();
    utils.dialog({
        html: ui,
        height: 500,
        width: 400,
        title: '店铺资料',
        loading: true,
        buttons: {
            '确定': function (yuan) {
                saveBasShopRightDS(yuan, anchor);
            },
            '取消': utils.undialog
        },
        create: function (yuan) {
            //yuan.setHtml({
            //    html: ui
            //});

            var gv = $('table.qp-gridview:eq(0)', yuan.body);

            var $checks = gv.find(":checkbox[name$='chkSelect']");
            var tmp = 0;
            $checks.each(function () {
                var check = $(this);
                tmp = check.prev('span').eq(0).html();
                if (shopMapMultipleSelect.includes(tmp)) check.attr('checked', 'checked');

                //绑定change事件
                check
                    .change(function () {
                        if ($(this).attr('checked') === 'checked') {
                            shopMapMultipleSelect.push($(this).prev('span').eq(0).html());
                        }
                        else {
                            shopMapMultipleSelect = shopMapMultipleSelect.filter(fruit => fruit !== $(this).prev('span').eq(0).html());
                        }
                    });
            });

            $('#txtBasShopCode', yuan.body).watermark("请输入店铺代码", "watermark2"); //水印

            var empty = $('div.data-empty', yuan.body);
            if (empty.length > 0) {
                yuan.bottom.remove();
            }
            //查询
            yuan.body.find('a.func-query')
                .click(function () {
                    gotoQueryBasShop(yuan, anchor); //查询
                });
            //默认执行一次查询
            gotoQueryBasShop(yuan, anchor); //查询
        }
    });
   
}


/// <summary>
/// 查询店铺资料
/// </summary>
/// <param>anchor:点击关联的链接</param>
function gotoQueryBasShop(yuan, anchor) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    var company = $(anchor).prevAll('span.company').eq(0).html();
    var shopcode = $('#txtBasShopCode', yuan.body).val();
    var spanOMSShopId = $(anchor).next('span.OMSShopId').eq(0);
    var shop = spanOMSShopId.html();
    var shops = shop.split(',');
    var newmapid = shopMapMultipleSelect.join();
    newmapid = newmapid.replace(/^,|,$/g, '');

    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        data: { mode: 'queryBasShop', shopcode: encodeURI(shopcode), company: company, shopMapMultipleSelect: newmapid },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                var $wrapper = $('div.wrapper', yuan.body);
                $wrapper.children('div:eq(2)').remove();

                $(json.Html)
                    .appendTo($wrapper);

                //复选
                var gv = $('table.qp-gridview:eq(0)', yuan.body);

                var $checks = gv.find(":checkbox[name$='chkSelect']");
                var tmp = 0;
                $checks.each(function () {
                    var check = $(this);
                    tmp = check.prev('span').eq(0).html();
                    if (shopMapMultipleSelect.includes(tmp)) check.attr('checked', 'checked');

                    //绑定change事件
                    check
                        .change(function () {
                            if ($(this).attr('checked') === 'checked') {
                                shopMapMultipleSelect.push($(this).prev('span').eq(0).html());
                            }
                            else {
                                shopMapMultipleSelect = shopMapMultipleSelect.filter(fruit => fruit !== $(this).prev('span').eq(0).html());
                            }
                        });
                });

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
}


/// <summary>
/// 只在店铺关联
/// </summary>
/// <param>anchor:点击关联的链接</param>
function saveBasShopRightDS(yuan, anchor) {

    //如果没有可选的供应商店铺，则不给保存
    var me = $(anchor);
    var divMax = $("#bas_shop");
    if ($('div.data-empty', divMax).length > 0) {
        return false;
    }
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);

    var searchcode = $txtShopCode.val(); //公司店铺的查询条件

    var vendorid = $(anchor).prevAll('span.vendor').eq(0).html();
    var shopRightID = $(anchor).prevAll('span.id').eq(0).html();
    var companyid = $(anchor).prevAll('span.company').eq(0).html();
    var shopid = $(anchor).prevAll('span.shop').eq(0).html();
    var cardid = $(anchor).prevAll('span.card').eq(0).html();

    var oldmapid = $(anchor).next('span.OMSShopId').eq(0).html(); //之前关联的店铺
    //后期增加
    var Stockcode = $('#txtShopSearchCode').val();//获得店铺代码
    var newmapid = shopMapMultipleSelect.join();
    //$('table.qp-gridview:eq(0)', yuan.body)
    //    .find(":checkbox[name$='chkSelect']")
    //    .each(function () {
    //        if ($(this).is(':checked')) {
    //            newmapid = newmapid + ',' +  $(this).prev('span').eq(0).html();
    //        }
    //    });
    newmapid = newmapid.replace(/^,|,$/g, '');
    //alert(cardid);return false;
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        data: { mode: 'saveBasShopRightDS', shopRightID: shopRightID, vendor: vendorid, company: companyid, shop: shopid, oldmap: oldmapid, newmap: newmapid, card: cardid, searchcode: encodeURI(Stockcode) },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            //刷新
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                $('div.gv-wrapper')
                    .empty()
                    .append(json.Html);
                utils.undialog();

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
}

/// <summary>
/// 只在店铺关联
/// </summary>
/// <param>anchor:点击关联的链接</param>
function saveShopMap(yuan, anchor) {

    //如果没有可选的供应商店铺，则不给保存
    var me = $(anchor);
    var divMax = $("#bas_vendshop");
    if ($('div.data-empty', divMax).length > 0) {
        return false;
    }
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);

    var searchcode = $txtShopCode.val(); //公司店铺的查询条件

    var vendorid = $(anchor).nextAll('span.vendor').eq(0).html();
    var companyid = $(anchor).nextAll('span.company').eq(0).html();
    var shopid = $(anchor).nextAll('span.shop').eq(0).html();
    var cardid = $(anchor).nextAll('span.card').eq(0).html();

    var oldmapid = $(anchor).prev('span.vendshop').eq(0).html(); //之前关联的店铺
    //后期增加
    var Stockcode = $('#txtShopSearchCode').val();//获得店铺代码
    var newmapid = '';
    $('table.qp-gridview:eq(0)', yuan.body)
        .find(":checkbox[name$='chkSelect']")
        .each(function () {
            if ($(this).is(':checked')) {
                newmapid = $(this).prev('span').eq(0).html();
            }
        });
    //alert(cardid);return false;
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        //  data: { mode: 'savemap', vendor: vendorid, company: companyid, shop: shopid, oldmap: oldmapid, newmap: newmapid, card: cardid, searchcode: encodeURI(searchcode) },
        data: { mode: 'savemap', vendor: vendorid, company: companyid, shop: shopid, oldmap: oldmapid, newmap: newmapid, card: cardid, searchcode: encodeURI(Stockcode) },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            //刷新
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                $('div.gv-wrapper')
                    .empty()
                    .append(json.Html);
                utils.undialog();

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
}

/// <summary>
/// 查询供应商店铺
/// </summary>
/// <param>anchor:点击关联的链接</param>
function gotoQuery(yuan, anchor) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    var vendor = $(anchor).nextAll('span.vendor').eq(0).html();
    var vendshopid = $(anchor).prev('span.vendshop').eq(0).html();
    var shopcode = $('#txtShopCode', yuan.body).val();
    //alert(vendor);
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        data: { mode: 'queryvs', shopcode: encodeURI(shopcode), vendor: vendor },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                var $wrapper = $('div.wrapper', yuan.body);
                $wrapper.children('div:eq(2)').remove();

                $(json.Html)
                    .appendTo($wrapper);

                //复选
                var gv = $('table.qp-gridview:eq(0)', yuan.body);

                var $checks = gv.find(":checkbox[name$='chkSelect']");
                var tmp = 0;
                $checks.each(function () {
                    var check = $(this);
                    tmp = check.prev('span').eq(0).html();
                    if (tmp === vendshopid) check.attr('checked', 'checked');

                    //绑定change事件
                    check
                        .change(function () {
                            $checks.each(function () {
                                if ($(this).attr('id') !== check.attr('id')) {
                                    $(this).attr('checked', false); //单选
                                }
                            });

                        });
                });

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
}

/// <summary>
/// 根据公司、供应商和品牌查询能提报的店铺记录
/// </summary>
/// <param>anchor:点击关联的链接</param>
function loadShopRight(company, vendor, card, shopcode) {
    //alert(company + vendor + card);
    var nullList = ['null', 'undefined', '0'];
    if ($.inArray(company, nullList) !== -1) {
        utils.dialog({
            html: '请先选择公司！',
            icon: 'warning'
        });
        return false;
    }

    if ($.inArray(vendor, nullList) !== -1) {
        utils.dialog({
            html: '请先选择供应商！',
            icon: 'warning'
        });
        return false;
    }

    if ($.inArray(card, nullList) !== -1) {
        utils.dialog({
            html: '请先选择品牌！',
            icon: 'warning'
        });
        return false;
    }

    //alert('x');
    if (loading) return false;
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        data: { mode: 'queryright', shopcode: encodeURI(shopcode), vendor: vendor, company: company, card: card },
        beforeSend: function () {
            loading = true;
            utils.dialog({
                showHeader: false,
                html: '正在查询...',
                icon: 'loading'
            });
        },
        error: function (jqXHR, textStatus, errorThrown) { },
        success: function (html) {

            var json = utils.toJson(html);
            if (json.StatusCode === '1') {

                var $bs = $('#bas_shopright').css('display', '');

                var $wrapper = $('div.gv-wrapper', $bs);
                $wrapper
                    .empty()
                    .append(json.Html);

                var gv = $('table.qp-gridview:eq(0)', $wrapper);

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
                //加载店铺成功，清空以前的搜索关键字
                $txtShopCode.val(shopcode);
            }
        },
        complete: function (jqXHR, textStatus) {
            utils.undialog();
            loading = false;
        }
    });
}

/// <summary>
/// 初始化公司下拉列表
/// </summary>
/// <param name="selectEl">none</param>
function createCompanyDropdown() {
    var company, vendor, card; //公司、供应商、品牌
    var companyBeforeChange = '0'; //在改变之前，品牌的选中值

    $ddlCompany.kendoDropDownList({
        open: function () {
            $(document.body).addClass('overflow');
        },
        close: function () {
            body.removeClass('overflow');
        },
        change: function (e) {
            var me = this;
            var company = me.value();
            //alert(this.value());
            if (company === '0') return false;
            if (loading) { //如果正在加载内容，则不允许其它操作
                //$ddlCompany.data('kendoDropDownList').value(companyBeforeChange);
                me.value(companyBeforeChange)
                me._focus();
                return false;
            }

            $.ajax({
                type: "POST",
                url: '/admin/gat_solution.aspx',
                timeout: 50000,
                data: "mode=loadvendor&company=" + company,
                beforeSend: function () {
                    //禁用下拉列表
                    //utils.kendo.setenable($('select.select'),false);
                    utils.kendo.setloading($ddlVendor);
                    loading = true;
                    $(document.body).addClass('overflow');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    me.value(companyBeforeChange);
                    me._focus();
                },
                success: function (html) {
                    var json = utils.toJson(html);
                    companyBeforeChange = me.value();

                    //重新加载供应商下拉列表的kendoui
                    var kendo_vendor = $ddlVendor.data('kendoDropDownList');
                    //kendo_vendor.dataSource._data = [{
                    //    'text': '21',
                    //    'value': '22'
                    //}];
                    utils.kendo.removedropdown($ddlVendor);

                    var kendo_td_vendor = kendo_vendor.wrapper.parent();
                    kendo_td_vendor
                        .empty()
                        .append(json.Html);

                    $ddlVendor = kendo_td_vendor.find('select:eq(0)'); //.css('display', 'none');
                    createVendDropdown(); //===

                    //清空供应商的同时也清空品牌
                    var kendo_card = $ddlCard.data('kendoDropDownList');
                    utils.kendo.removedropdown($ddlCard);

                    var kendo_td_card = kendo_card.wrapper.parent();
                    kendo_td_card
                        .empty()
                        .append(json.Message)
                    $ddlCard = kendo_td_card.find('select:eq(0)');
                    createBrandDropdown();
                }, //请求供应商完毕
                complete: function (jqXHR, textStatus) {
                    //utils.kendo.setenable($('select.select'), true);
                    utils.kendo.setunload($ddlVendor);
                    loading = false;
                    $(document.body).removeClass('overflow');
                }
            }); //请求供应商
        }
    });

    //初始化值
    companyBeforeChange = $ddlCompany.data('kendoDropDownList').value();
}

/// <summary>
/// 初始化供应商下拉列表
/// </summary>
/// <param name="selectEl">none</param>
function createVendDropdown() {
    var company, vendor, card; //公司、供应商、品牌
    var vendorBeforeChange = '0'; //在改变之前，品牌的选中值

    $ddlVendor.kendoDropDownList({
        open: function () {
            $(document.body).addClass('overflow');
        },
        close: function () {
            body.removeClass('overflow');
        }, change: function (e) {
            var me = this;
            var vendor = me.value();
            var company = $ddlCompany.data('kendoDropDownList').value();
            if (vendor === '0') return false;
            if (company === '0' || loading) {
                me.value(vendorBeforeChange);
                me._focus();
                return false;
            };

            //开始加载品牌
            $.ajax({
                type: "POST",
                url: '/admin/gat_solution.aspx',
                timeout: 50000,
                data: "mode=loadcard&company=" + company + "&vendor=" + vendor,
                beforeSend: function () {
                    loading = true;
                    utils.kendo.setloading($ddlCard);
                    $(document.body).addClass('overflow');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    me.value(vendorBeforeChange); //还原品牌的值
                },
                success: function (ai) {
                    var aiJson = utils.toJson(ai);
                    vendorBeforeChange = me.value();

                    //重新加载品牌下拉列表的kendoui
                    var kendo_card = $ddlCard.data('kendoDropDownList');
                    utils.kendo.removedropdown($ddlCard);

                    var kendo_td_card = kendo_card.wrapper.parent();
                    kendo_td_card
                        .empty()
                        .append(aiJson.Html); //添加新的select元素

                    $ddlCard = kendo_td_card.find('select:eq(0)');
                    createBrandDropdown();
                },
                complete: function (jqXHR, textStatus) {
                    utils.kendo.setunload($ddlCard);
                    loading = false;
                    $(document.body).removeClass('overflow');
                }

            }); //AJAX 品牌

        } //change
    });

    //初始化值
    vendorBeforeChange = $ddlVendor.data('kendoDropDownList').value();
}


/// <summary>
/// 初始化品牌下拉列表
/// </summary>
function createBrandDropdown() {

    var company, vendor, card; //公司、供应商、品牌
    var cardBeforeChange = '0'; //在改变之前，品牌的选中值

    $ddlCard.kendoDropDownList({
        open: function () {
            $(document.body).addClass('overflow');
        },
        close: function () {
            $(document.body).removeClass('overflow');
        },
        change: function () {
            //在品牌下拉列表的值改变的时候，需要刷新提报的店铺列表
            var me = this;
            company = $ddlCompany.data('kendoDropDownList').value();
            vendor = $ddlVendor.data('kendoDropDownList').value();
            card = me.value();
            if (card === '0') return false;
            if (company === '0' || vendor === '0' || loading) {
                me.value(cardBeforeChange);
                me._focus();
                return false;
            }


            //var shopcode = $txtCode.val(); //店铺代码

            //开始AJAX请求店铺
            loadShopRight(company, vendor, card, '');
        }
    });     //品牌下拉    

    //初始化值
    cardBeforeChange = $ddlCard.data('kendoDropDownList').value();
}




/// <summary>
/// 关联仓库
/// </summary>
/// <param>anchor:点击关联的链接</param>
function stockMap(anchor) {
    var me = $(anchor);
    var vendor = me.prev('span.vendshop').eq(0).html();

    var ui = $('#bas_vendStock .wrapper').clone();
    utils.dialog({
        html: ui,
        height: 500,
        width: 400,
        title: '关联供应商的仓库',
        loading: true,
        buttons: {
            '确定': function (yuan) {
                saveStockMap(yuan, anchor);
            },
            '取消': utils.undialog
        },
        create: function (yuan) {
            //yuan.setHtml({
            //    html: ui
            //});

            var gv = $('table.qp-gridview:eq(0)', yuan.body);

            var $checks = gv.find(":checkbox[name$='chkStockSelect']");
            var tmp = 0;
            $checks.each(function () {
                var check = $(this);
                tmp = check.prev('span').eq(0).html();
                if (tmp === vendor) check.attr('checked', 'checked');

                //绑定change事件
                check
                    .change(function () {
                        $checks.each(function () {
                            if ($(this).attr('id') !== check.attr('id')) {
                                $(this).attr('checked', false); //单选
                            }
                        });

                    });
            });



            $('#txtStockCode', yuan.body).watermark("请输入仓库代码", "watermark2"); //水印

            var empty = $('div.data-empty', yuan.body);
            if (empty.length > 0) {
                yuan.bottom.remove();
            }

            //查询
            yuan.body.find('a.func-query')
                .click(function () {
                    var $vc = $('#txtStockCode', yuan.body); //仓库代码
                    gotoStockQuery(yuan, anchor); //查询
                });
        }
    });
}

/// <summary>
/// 只在仓库关联
/// </summary>
/// <param>anchor:点击关联的链接</param>
function saveStockMap(yuan, anchor) {

    //如果没有可选的供应商店铺，则不给保存
    var me = $(anchor);
    var divMax = $("#bas_vendstock");
    if ($('div.data-empty', divMax).length > 0) {
        return false;
    }

    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    //此操作只有仓库跟店铺无关
    // var searchcode = $TextVendStock.val(); //公司店铺的查询条件
    //供应商ID
    var vendorid = $(anchor).nextAll('span.vendor').eq(0).html();
    //公司ID
    var companyid = $(anchor).nextAll('span.company').eq(0).html();
    //仓库ID
    var stockid = $(anchor).nextAll('span.stock').eq(0).html();
    //品牌
    var cardid = $(anchor).nextAll('span.card').eq(0).html();
    var oldmapid = $(anchor).prev('span.vendshop').eq(0).html(); //之前关联的店铺

    //后期增加
    var Stockcode = $('#txtStockSearchCode').val();//获得仓库代码

    var newmapid = '';
    $('table.qp-gridview:eq(0)', yuan.body)
        .find(":checkbox[name$='chkStockSelect']")
        .each(function () {
            if ($(this).is(':checked')) {
                newmapid = $(this).prev('span').eq(0).html();
            }
        });
    //alert(cardid);return false;
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        //data: { mode: 'saveStockMap', vendor: vendorid, company: companyid, stock: stockid, oldmap: oldmapid, newmap: newmapid, card: cardid, searchcode: encodeURI('') },
        data: { mode: 'saveStockMap', vendor: vendorid, company: companyid, stock: stockid, oldmap: oldmapid, newmap: newmapid, card: cardid, searchcode: encodeURI(Stockcode) },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            //刷新
            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                $('div.gv-stock')
                    .empty()
                    .append(json.Html);
                utils.undialog();

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
}



/// <summary>
/// 关联小窗体查询仓库数据
/// </summary>
/// <param>anchor:点击关联的链接</param>
function gotoStockQuery(yuan, anchor) {
    var $op = $('.yuan-bottom-op:eq(0)', yuan.buttom);
    var vendor = $(anchor).nextAll('span.vendor').eq(0).html();
    var vendshopid = $(anchor).prev('span.vendshop').eq(0).html();
    var shopcode = $('#txtStockCode', yuan.body).val();
    //alert(vendor);
    $.ajax({
        type: "POST",
        url: '/admin/gat_solution.aspx',
        timeout: 50000,
        data: { mode: 'queryvsStock', shopcode: encodeURI(shopcode), vendor: vendor },
        beforeSend: function () {
            $op
                .html('')
                .append($('<i class="loading">&nbsp;</i>'));
            yuan.disable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var message = '&nbsp;查询失败';
            if (textStatus === 'timeout') {
                message += '，消息：加载超时。请稍后重试！';
            } else {
                message += "，请稍后重试！";
            }


            $op
                .html('')
                .addClass("warning")
                .html('<i class="warning">&nbsp;</i>' + message);
        },
        success: function (html) {

            var json = utils.toJson(html);
            $op.html('');
            if (json.StatusCode === '1') {

                var $wrapper = $('div.wrapper', yuan.body);
                $wrapper.children('div:eq(2)').remove();

                $(json.Html)
                    .appendTo($wrapper);

                //复选
                var gv = $('table.qp-gridview:eq(0)', yuan.body);

                var $checks = gv.find(":checkbox[name$='chkStockSelect']");
                var tmp = 0;
                $checks.each(function () {
                    var check = $(this);
                    tmp = check.prev('span').eq(0).html();
                    if (tmp === vendshopid) check.attr('checked', 'checked');

                    //绑定change事件
                    check
                        .change(function () {
                            $checks.each(function () {
                                if ($(this).attr('id') !== check.attr('id')) {
                                    $(this).attr('checked', false); //单选
                                }
                            });

                        });
                });

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
}
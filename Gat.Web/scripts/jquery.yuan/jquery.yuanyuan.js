

; (function ($) { 
    
    $.extend($.fn, {
        getCss: function(key) {
            var v = parseInt(this.css(key));
            if (isNaN(v))
                return false;
            return v;
        }
    });
    $.fn.drag = function(opts) {
        var ps = $.extend({
            zIndex: 20,
            opacity: .7,
            handler: null,
            onMove: function(e) { },
            onDrop: function(e) { }
        }, opts);

        var dragndrop = {
            drag: function(e) {
                var dragData = e.data.dragData;
                dragData.target.css({
                    left: dragData.left + e.pageX - dragData.offLeft,
                    top: dragData.top + e.pageY - dragData.offTop
                });
                dragData.handler.css({ cursor: 'move' });
                dragData.onMove(e);
            },
            drop: function(e) {
                var dragData = e.data.dragData;
                dragData.target.css(dragData.oldCss); //.css({ 'opacity': '' });
                dragData.handler.css('cursor', dragData.oldCss.cursor);
                dragData.onDrop(e);
                $(this).unbind('mousemove',dragndrop.drag);
                $(this).unbind('mouseup', dragndrop.drop);
            }
        }
        return this.each(function() {
            var me = this;
            var handler = null;
            if (typeof ps.handler == 'undefined' || ps.handler == null){
                handler = $(me);
            }
            else{
                handler = (typeof ps.handler == 'string' ? $(ps.handler, this) : ps.handler);
            }

            handler.bind('mousedown', { e: me }, function(ev) {
                var target = $(ev.data.e);
                var oldCss = {};
                /*if (target.css('position') !== 'absolute') {
                    try {
                        target.position(oldCss);
                    } catch (ex) { }
                    target.css('position', 'absolute');
                }*/

                var dragData = {
                    left: oldCss.left || target.getCss('left') || 0,
                    top: oldCss.top || target.getCss('top') || 0,
                    offLeft: ev.pageX,
                    offTop: ev.pageY,
                    oldCss: oldCss,
                    onMove: ps.onMove,
                    onDrop: ps.onDrop,
                    handler: handler,
                    target: target
                }
                $(document)
                .bind('mousemove',{dragData: dragData},dragndrop.drag)
                .bind('mouseup', { dragData: dragData }, dragndrop.drop);
            });
        });
    }
    
    $.yuanyuan = function(settings){  
        // 扩展缓冲动画
        $.extend($.easing, {
            easeOutBounce: function (x, t, b, c, d) {
                if ((t /= d) < (1 / 2.75)) {
                    return c * (7.5625 * t * t) + b;
                } else if (t < (2 / 2.75)) {
                    return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
                } else if (t < (2.5 / 2.75)) {
                    return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
                } else {
                    return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
                }
            } ,

            easeOutBack: function (x, t, b, c, d, s) {
		        if (s == undefined) s = 1.70158;
		        return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
	        },
            easeInOutQuint: function (x, t, b, c, d) {
		        if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
		        return c/2*((t-=2)*t*t*t*t + 2) + b;
	        }
        });

        
        var opt = $.extend({
            title : '提示' , //标题
            hclose : true , //标题头部的关闭按钮
            showHeader : true,
            html : '&nbsp;&nbsp;', //内容，如果指定iframe的话忽略该配置
            url : null, // iframe地址，目前暂不支持
            //loading : false,
            height : null,
            width : 300,
            icon : 'none' , //'none','correct','warning',loding,error
            create : null, //创建DOM时回高
            callback : null , //关闭时的回调 
            buttons : { } //按钮，以名称做为按钮文本
        },settings);

        if (!settings.hClose) {
            opt.hclose = false;
        }

        var browser = navigator.userAgent.toLowerCase();
        var ie    = browser.indexOf('msie') > -1 && browser.indexOf('opera') == -1;
        var version = browser.match(/(msie|firefox|webkit|opera)[\/:\s](\d+)/) ? RegExp.$2 : '0';
        var ie6     = ie && version < 7;

        var doc = $(document),body = $(document.body), win = $(window);
        var cssHidden = { 'overflow': 'hidden' },cssNormal = { 'overflow':'' };
        var docHeight = doc.height(),docWidth = doc.width(),winHeight = win.height(),windWidth = win.width();

        var elements = document.getElementsByTagName('script');
        var path = '';
        for (var i = 0, len = elements.length; i < len; i++) {
            if (elements[i].src && elements[i].src.match(/yuanyuan[\w\-\.]*\.js/)) {
                path =  elements[i].src.substring(0, elements[i].src.lastIndexOf('/') + 1);
            }
        }
        //alert(path);

        var minWidth = 250,minHeight = 100;
        if(opt.width < minWidth) opt.width = minWidth;
        
        if(opt.height && typeof(opt.height!=='undefined') && opt.height < minHeight) opt.height = minHeight;
        //alert(typeof(opt.height));
                
        $.func = {  };
        $.yuanyuan.create = function(){
            // #5742 - background

            var me = this;
            me.opt = opt;

            me.bg =
            $('<div />')
            .attr({ 'class' : 'yuan-bg' })
            .css({  
                //height : docHeight
            })
            .appendTo(body);

            // #5742 - outeropt.width，opt.height
            //var $outer = $('<div />')
            me.outer = $('<div />')
            .attr({ 'class' : 'yuan-outer' })
            .css({
                'left' : windWidth / 2,
                'top' :  winHeight / 2,
                'width' : 1,
                'height' : 1 
            })
            .appendTo(body);

            // #5742 - inner
            me.inner = $('<div />')
            .attr({ 'class' : 'yuan-inner' })
            .appendTo(me.outer);

            // #5742 - title
            me.title = $('<div />')
            .attr({ 'class' : 'yuan-title' })
            //.appendTo(me.inner)
            .append(
                $('<h4 />')
                .html(opt.title)    
            )
            .append(
                (function(){
                    if(opt.hclose){
                        return $('<a />')
                                .attr({
                                    hideFocus: true,
                                    href: 'javascript:;',
                                    title:'关闭',
                                    'class' : 'yuan-closer'
                                })
                                .bind('click',function(e){
                                    var e = e || window.event;
                                    e.preventDefault();
                                    // #5577 close
                                    me.hide();
                                });
                    }else{
                        return $('<span />');
                    }
                })()
            );
            if(opt.showHeader) me.title.appendTo(me.inner);

            // #5742 - body
            var gif = path + opt.icon + '.gif';
            if(opt.icon === 'loading') gif = path + 'ajax-loader.gif';
            var icon = "<i style='padding:12px 15px;background:url(" + gif + ") no-repeat center center;line-height:32px;'>&nbsp;</i>&nbsp;&nbsp;";
            me.body =
            $('<div />')
            .attr({ 
                'class' : 'yuan-body'
            })
            //.html()
            .append(
                (function(){
                    //opt.icon !== 'none' ? icon + opt.html  : opt.html
                    var content = null;
                    if(typeof opt.html === 'string'){
                        return $('<div />').html(opt.icon !== 'none' ? icon + opt.html  : opt.html);
                    }else{
                        return opt.html
                    }
                })()
            )
            .appendTo(me.inner);

            me.bottom = 
            $('<div />')
            .attr({ 'class' : 'yuan-bottom' })
            .append($('<div />').attr('class','yuan-bottom-op')) ;
            
            var bc = 0;
            
            $.each(opt.buttons || {},function(n,v){
                $('<a></a>')
                .attr({
                    'class' : 'yuan-botton',
                    'href' : 'javascript:;',
                    'hideFocus' : true
                })
                .html(n)
                .bind('click',function(e){
                    var e = e || window.event;
                    e.preventDefault();
                    //$.func.hide();
                    if($.isFunction(v)) v(me);
                })
                .appendTo(me.bottom);
                me.bottom.append('<span>&nbsp;</span>');
                bc += 1;
            });
            if(bc > 0 ) me.bottom.appendTo(me.inner);
            //if(bc > 0 && opt.icon !== 'loading') me.bottom.appendTo(me.inner);
            me.bc = bc;

            //if($.isFunction(opt.create)) opt.create(me);

        } //create all ui element

        $.yuanyuan.animate = function(set){
            /*var x = this.dialog;
            var y = $(this);
            var z = x.find('a.yuan-closer').click;
            x.find('a.yuan-closer')
             .unbind('click')
             .bind('click',z)
             .trigger('click');*/
            //var $bg = $('div.yuan-bg');
            //var $outer = $('div.yuan-outer');
            this
            .outer
            .animate({
                left : set.left,
                top : set.top,
                width : set.width,
                height : set.height
            },400,'easeInOutQuint',function(){
                if($.isFunction(set.callback)) set.callback();
            });
        } // # animate
        
        $.yuanyuan.show = function(){
            /*var $outer = $('div.yuan-outer'),
                $inner = $outer.children('div.yuan-inner'),
                $bottom =$inner.children('div.yuan-bottom').css('opacity','0'),
                $body = $inner.children('div.yuan-body'),
                $title = $inner.children('div.yuan-title');*/
            var me = this;
            
            //body.css(cssHidden);
            //if(opt.loading){
            //    me.body
                //.css('background','url('+path + 'ajax-loader.gif) no-repeat 20px center')
            //    .children()
            //    .css('display','none');
            //}
            //if(set.height && typeof(opt.height) !== 'undefined' && set.height < minHeight) set.height = minHeight;
            me.outer.css({
                //'visibility' : 'hidden',
                'width' : opt.width//,
                //height : 'auto'
            });
            //var innerHeight = $inner.height();//alert(innerHeight);
            if(opt.height === null || typeof(opt.height) === 'undefined'){
               me.outer.css('height','auto');
            }else{
                me.outer.css('height',opt.height);
            }
            //alert($inner.height());         
            var height = me.outer.height();
            //op.css
            me
            .bottom
            .css('opacity',0)
            .find('div.yuan-bottom-op')
            .css({
                'width':opt.width - 150,
                'overflow' : 'hidden'
            });

            me.outer.css({
                'height' :1,
                'width' : 1
            });

            var settings = {
                left : ($(window).width() - opt.width) / 2,
                top : ($(window).height() - height) / 2,
                width : opt.width,
                height : height,
                close : false,
                callback : function(){
                    /*if(opt.icon === 'loading' && me.bc > 0){
                        me.bottom.appendTo(me.inner);
                    }*/
                    
                    var ih = me.inner.height(),
                        hh = me.opt.showHeader ? me.title.outerHeight(true) : 0,
                        bh = me.bc > 0 ? me.bottom.outerHeight(true) : 0,
                        bp = window.parseInt(me.body.css('padding-left'));
                        //alert(ih +',' + hh +','+bh +','+bp)
                    me.body
                    .css({
                        'height' :ih  - hh - bh - 2 * bp,/**/
                        'background-position' : '-9999px -9999px'
                    })
                    .children()
                    .css('display','');
                    me.bottom.fadeTo('100',1);

                    me.title.focus();

                    if($.isFunction(opt.create)) opt.create(me);
                }
            };
            me.animate(settings);
        }

        $.yuanyuan.hide = function(){
            var me = this;

            var settings = {
                left : ($(window).width() - 1) / 2,
                top : ($(window).height() - 1) / 2,
                width : 1,
                height : 1,
                callback : function(){
                    if($.isFunction(opt.callback)) opt.callback(me);
                    me.outer.remove();
                    body.css(cssNormal);
                }
            };
            me.bg.remove();
            if(opt.loading) me.body.empty();
            me.animate(settings);

            $(document).unbind('keyup',_escape);
        }

        $.yuanyuan.enable = function(){
            var me = this;

            var dis =  me
            .inner
            .find("a[class^='yuan']:hidden").length > 0;

            if(dis){
                me
                .inner
                .find("a[class^='yuan']:visible")//a:hidden
                .each(function(){
                    $(this).remove();
                });

                me
                .inner
                .find("a[class^='yuan']:hidden")//a:hidden
                .each(function(){
                    $(this).css('display','');
                });            
            }


        }

        $.yuanyuan.disable = function(){
            var me = this;
            me
            .inner
            .find("a[class^='yuan']")
            .each(function(){
                var $close = $(this);
                $close
                .clone()
                .insertAfter($close);
                $close.css('display','none');
            });

            //me.title.click(function(){me.enable()});
        }

        $.yuanyuan.setHtml = function(set){
            var me = this;
            
            set = $.extend({
                icon : 'none',
                html : '&nbsp;'
            },set);
            
            var gif = path + set.icon + '.gif';
            var icon = "<i style='padding:12px 15px;background:url(" + gif + ") no-repeat center center;line-height:32px;'>&nbsp;</i>&nbsp;&nbsp;";
            me.body
            .html('')
            .append(
                (function(){
                    var content = null;
                    if(typeof set.html === 'string'){
                        return $('<div />').html(opt.icon !== 'none' ? icon + set.html  : set.html);
                    }else{
                        return set.html
                    }
                })()
            );

            var orgHeight = me.outer.height();
            me.outer.css('height','auto');
            me.body.css({
                'height':'auto',
                'background-position':'-888px -888px'
            });

            var h =  me.outer.height();
            if(opt.height){ //定义了窗口的高度
                h = h > orgHeight ? orgHeight : h;
            }

            me.outer.css('height',orgHeight);

            me.outer.animate({
                left : ($(window).width() - opt.width) / 2,
                top : ($(window).height() - h) / 2,
                width : opt.width,
                height : h - 2
            },400,'easeInOutQuint');
        }


        /*win.resize(function(){
            var $outer = $('div.yuan-outer')
            var settings = {
                left : ($(window).width() - opt.width) / 2,
                top : ($(window).height() - $outer.height()) / 2,
                width : opt.width,
                height : $outer.height(),
                close : false,
                callback : $.noop
            };
            $.func.animate(settings);
        });*/

        $.yuanyuan.create();
        $.yuanyuan.show();
        $.yuanyuan.outer.drag({
           handler: $.yuanyuan.title
        });

        $(document).bind('keyup',_escape);

        function _escape(e){
            if(e.keyCode === 27){
                $.yuanyuan.hide();
            }
        }

        //$.yuanyuan.disable();
        
    }; //提示插件

})(jQuery);
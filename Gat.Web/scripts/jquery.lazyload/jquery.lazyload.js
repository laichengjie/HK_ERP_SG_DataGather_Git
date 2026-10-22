

/*
 * Lazy Load - jQuery plugin for lazy loading images
 *
 * Copyright (c) 2007-2011 Mika Tuupola
 *
 * Licensed under the MIT license:
 *   http://www.opensource.org/licenses/mit-license.php
 *
 * Project home:
 *   http://www.appelsiini.net/projects/lazyload
 *
 * Version:  1.6.0-dev
 *
 */
(function ($) {

    $.fn.lazyload = function (options) {
        var settings = {
            threshold: 0,
            failurelimit: 0,
            event: "scroll",
            effect: "show",
            container: window
        };

        if (options) {
            $.extend(settings, options);
        }

        // browser detect
        var eff = $.checkbrowse();
        var showeffect = "";
        if ((eff['is'] == 'msie' && eff['ver'] < 8.0)) {
            showeffect = "show"
        } else {
            showeffect = "fadeIn"
        }
        settings['effect'] = showeffect;

        /* Fire one scroll event per scroll. Not one scroll event per image. */
        var elements = this;
        if ("scroll" == settings.event) {
            $(settings.container).bind("scroll", function (event) {
                var counter = 0;
                elements.each(function () {
                    if ($.abovethetop(this, settings) ||
                        $.leftofbegin(this, settings)) {
                        /* Nothing. */
                    } else if (!$.belowthefold(this, settings) &&
                        !$.rightoffold(this, settings)) {
                        $(this).trigger("appear");
                    } else {
                        if (counter++ > settings.failurelimit) {
                            return false;
                        }
                    }
                });

                /* Remove image from array so it is not looped next time. */
                var temp = $.grep(elements, function (element) {
                    return !element.loaded;
                });
                elements = $(temp);

            });
        }

        this.each(function () {
            var self = this;
            self.loaded = false;

            /* When appear is triggered load original image. */
            $(self).one("appear", function () {
                if (!this.loaded) {
                    $("<img />")
                        .bind("load", function () {
                            $(self)
                                .hide()
                                .attr("src", $(self).data("original"))
                                [settings.effect](settings.effectspeed);
                            self.loaded = true;
                        })
                        .attr("src", $(self).data("original"));
                };
            });

            /* When wanted event is triggered load original image */
            /* by triggering appear.                              */
            if ("scroll" != settings.event) {
                $(self).bind(settings.event, function (event) {
                    if (!self.loaded) {
                        $(self).trigger("appear");
                    }
                });
            }
        });

        /* Force initial check if images should appear. */
        $(settings.container).trigger(settings.event);

        return this;

    };

    /* Convenience methods in jQuery namespace.           */
    /* Use as  $.belowthefold(element, {threshold : 100, container : window}) */

    $.belowthefold = function (element, settings) {
        if (settings.container === undefined || settings.container === window) {
            var fold = $(window).height() + $(window).scrollTop();
        } else {
            var fold = $(settings.container).offset().top + $(settings.container).height();
        }
        return fold <= $(element).offset().top - settings.threshold;
    };

    $.checkbrowse = function () {
        var ua = navigator.userAgent.toLowerCase();
        var is = (ua.match(/\b(chrome|opera|safari|msie|firefox)\b/) || ['', 'mozilla'])[1];
        var r = '(?:' + is + '|version)[\\/: ]([\\d.]+)';
        var v = (ua.match(new RegExp(r)) || [])[1];
        jQuery.browser.is = is;
        jQuery.browser.ver = v;
        return {
            'is': jQuery.browser.is,
            'ver': jQuery.browser.ver
        }
    }

    $.rightoffold = function (element, settings) {
        if (settings.container === undefined || settings.container === window) {
            var fold = $(window).width() + $(window).scrollLeft();
        } else {
            var fold = $(settings.container).offset().left + $(settings.container).width();
        }
        return fold <= $(element).offset().left - settings.threshold;
    };

    $.abovethetop = function (element, settings) {
        if (settings.container === undefined || settings.container === window) {
            var fold = $(window).scrollTop();
        } else {
            var fold = $(settings.container).offset().top;
        }
        return fold >= $(element).offset().top + settings.threshold + $(element).height();
    };

    $.leftofbegin = function (element, settings) {
        if (settings.container === undefined || settings.container === window) {
            var fold = $(window).scrollLeft();
        } else {
            var fold = $(settings.container).offset().left;
        }
        return fold >= $(element).offset().left + settings.threshold + $(element).width();
    };
    /* Custom selectors for your convenience.   */
    /* Use as $("img:below-the-fold").something() */

    $.extend($.expr[':'], {
        "below-the-fold": "$.belowthefold(a, {threshold : 0, container: window})",
        "above-the-fold": "!$.belowthefold(a, {threshold : 0, container: window})",
        "right-of-fold": "$.rightoffold(a, {threshold : 0, container: window})",
        "left-of-fold": "!$.rightoffold(a, {threshold : 0, container: window})"
    });

})(jQuery);

//hoverclass
(function (a) { a.fn.hoverClass = function (b) { var a = this; a.each(function (c) { a.eq(c).hover(function () { $(this).addClass(b) }, function () { $(this).removeClass(b) }) }); return a }; a.fn.focusText = function (c) { var a = this; var b = (c == null) ? $(a).val() : c; a.val(b); a.focus(function () { if (a.val() == b) { a.val("") } }); a.blur(function () { if (a.val() == "") { a.val(b) } }); return a }; a.fn.soChange = function (b) { b = a.extend({ botPrev: null, botNext: null, changeType: '', slideTime: 1000, thumbObj: null, thumbNowClass: "now", changeClass: null, thumbOverEvent: true, autoChange: true, clickFalse: true, overStop: true, changeTime: 5000, delayTime: 300, start: 1 }, b || {}); var h = this; var i; var k = h.length; var e = 0; var g = 0; var c = []; var f; var hasthumb; var thumb; if (b.thumbObj != null) { hasthumb = true; thumb = a(b.thumbObj) } else { hasthumb = false } if (h.length <= 1) { thumb.remove(); } function d() { if (e != g) { if (hasthumb) { thumb.removeClass(b.thumbNowClass).eq(g).addClass(b.thumbNowClass) } if (b.changeClass != null) { h.eq(e).removeClass(b.changeClass); h.eq(g).addClass(b.changeClass) } else { if (b.changeType == 'fade' && b.slideTime > 0) { h.eq(g).fadeIn(b.slideTime); h.eq(e).fadeOut(b.slideTime) } else { h.eq(g).css("visibility", "visible"); h.eq(e).css("visibility", "hidden") } } e = g; if (b.autoChange == true) { stopAll(); start() } } } function stopAll() { for (var i = 0; i < c.length; i++) { clearInterval(c[i]) } } function start() { c.push(setInterval(j, b.changeTime)) } function j() { g = (e + 1) % k; d() } if (b.botNext != null) { a(b.botNext).click(function () { if (h.queue().length < 1) { runNext() } return false }) } if (b.botPrev != null) { a(b.botPrev).click(function () { if (h.queue().length < 1) { g = (e + 1) % k; d() } return false }) } if (hasthumb) { if (thumb.index(a("." + b.thumbNowClass)[0]) !== -1) { g = thumb.index(a("." + b.thumbNowClass)[0]) } else { g = b.start - 1; thumb.removeClass(b.thumbNowClass).eq(0).addClass(b.thumbNowClass) } thumb.click(function () { g = thumb.index(a(this)); d(); if (b.clickFalse == true) { return false } }); if (b.thumbOverEvent == true) { thumb.hover(function () { g = thumb.index(a(this)); f = setTimeout(function () { d(); stopAll() }, b.delayTime); return false }, function () { clearTimeout(f); return false }) } } if (b.changeClass != null) { h.removeClass(b.changeClass).eq(g).addClass(b.changeClass) } else { if (b.changeType == 'fade' && b.slideTime > 0) { h.css("display", "none").eq(g).css("display", "block") } else { h.css("visibility", "hidden").eq(g).css("visibility", "visible") } } if (b.autoChange == true) { c.push(setInterval(j, b.changeTime)); if (b.overStop == true) { h.mouseover(function () { stopAll(); return false }).mouseout(function () { start(); return false }) } } d(); this.stop = stopAll; this.start = start; h.bind("soChange", function (event, i) { g = i; d() }); return this } })(jQuery);
//lazyload
function lazyload(option) { var settings = { defObj: null, defHeight: -200 }; settings = $.extend(settings, option || {}); var defHeight = settings.defHeight, defObj = (typeof settings.defObj == "object") ? settings.defObj.find("img") : $(settings.defObj).find("img"); var pageTop = function () { return document.documentElement.clientHeight + Math.max(document.documentElement.scrollTop, document.body.scrollTop) - settings.defHeight }; var imgLoad = function () { defObj.each(function () { if ($(this).offset().top <= pageTop()) { var src2 = $(this).attr("src2"); if (src2) { $(this).attr("src", src2).removeAttr("src2") } } }) }; imgLoad(); $(window).bind("scroll", function () { imgLoad() }) }
function mlazyload(option) { var settings = { defObj: null, defHeight: 0, fn: null }; settings = $.extend(settings, option || {}); var defHeight = settings.defHeight, defObj = (typeof settings.defObj == "object") ? settings.defObj : $(settings.defObj); if (defObj.length < 1) { return }; var pageTop = function () { var d = document, y = (navigator.userAgent.toLowerCase().match(/iPad/i) == "ipad") ? window.pageYOffset : Math.max(d.documentElement.scrollTop, d.body.scrollTop); return d.documentElement.clientHeight + y - settings.defHeight }; var moduleLoad = function () { if (defObj.offset().top <= pageTop() && !defObj.attr("load")) { defObj.attr("load", "true"); if (settings.fn) { settings.fn(); } } }; moduleLoad(); $(window).bind("scroll", function () { moduleLoad(); }); } 
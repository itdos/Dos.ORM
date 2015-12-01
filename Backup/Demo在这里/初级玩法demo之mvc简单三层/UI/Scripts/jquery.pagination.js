(function ($) {
    /**
	 * @计算分页值的class类
	 */
    $.PaginationCalculator = function (maxentries, opts) {
        this.maxentries = maxentries;
        this.opts = opts;
    };

    $.extend($.PaginationCalculator.prototype, {
        /**
		 * 计算总页数
		 * @返回 {Number}
		 */
        numPages: function () {
            return Math.ceil(this.maxentries / this.opts.items_per_page);
        },
        /**
		 * 计算开始和结束点的分页
		 * 当前页码数和要显示的页码
		 * @返回数组 {Array}
		 */
        getInterval: function (current_page) {
            var ne_half = Math.floor(this.opts.num_display_entries / 2);
            var np = this.numPages();
            var upper_limit = np - this.opts.num_display_entries;
            var start = current_page > ne_half ? Math.max(Math.min(current_page - ne_half, upper_limit), 0) : 0;
            var end = current_page > ne_half ? Math.min(current_page + ne_half + (this.opts.num_display_entries % 2), np) : Math.min(this.opts.num_display_entries, np);
            return { start: start, end: end };
        }
    });

    // 初始化分页的jquery容器对象
    $.PaginationRenderers = {};

    /**
	 * @渲染分页链接的类
	 */
    $.PaginationRenderers.defaultRenderer = function (maxentries, opts) {
        this.maxentries = maxentries;
        this.opts = opts;
        this.pc = new $.PaginationCalculator(maxentries, opts);
    }
    $.extend($.PaginationRenderers.defaultRenderer.prototype, {
        /**
		 * 生成一个单独的链接的函数（如果是当前页，则生成span标签）
		 * @参数 {Number} page_id ：新页的页码
		 * @参数 {Number}current_page ：当前页页码
		 * @参数 {Object} appendopts ：新页的options：文本和类
		 * @返回 {jQuery} ：包含链接的jquery对象
		 */
        createLink: function (page_id, current_page, appendopts) {
            var lnk, np = this.pc.numPages();
            page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1);
            appendopts = $.extend({ text: page_id + 1, classes: "" }, appendopts || {});
            if (page_id == current_page) {
                lnk = $("<span class='current'>" + appendopts.text + "</span>");
            }
            else {
                lnk = $("<a>" + appendopts.text + "</a>")
					.attr('href', this.opts.link_to.replace(/__id__/, page_id));
            }
            if (appendopts.classes) { lnk.addClass(appendopts.classes); }
            lnk.data('page_id', page_id);
            return lnk;
        },
        // 生成数字范围内的页码链接
        appendRange: function (container, current_page, start, end, opts) {
            var i;
            for (i = start; i < end; i++) {
                this.createLink(i, current_page, opts).appendTo(container);
            }
        },
        getLinks: function (current_page, eventHandler) {
            var begin, end,
				interval = this.pc.getInterval(current_page),
				np = this.pc.numPages(),
				fragment = $("<div class='pagination'></div>");

            // 生成“上一页”的链接
            if (this.opts.prev_text && (current_page > 0 || this.opts.prev_show_always)) {
                fragment.append(this.createLink(current_page - 1, current_page, { text: this.opts.prev_text, classes: "prev" }));
            }
            // 开始项
            if (interval.start > 0 && this.opts.num_edge_entries > 0) {
                end = Math.min(this.opts.num_edge_entries, interval.start);
                this.appendRange(fragment, current_page, 0, end, { classes: 'sp' });
                if (this.opts.num_edge_entries < interval.start && this.opts.ellipse_text) {
                    jQuery("<span>" + this.opts.ellipse_text + "</span>").appendTo(fragment);
                }
            }
            // 链接之间的间隔
            this.appendRange(fragment, current_page, interval.start, interval.end);
            // 结束点
            if (interval.end < np && this.opts.num_edge_entries > 0) {
                if (np - this.opts.num_edge_entries > interval.end && this.opts.ellipse_text) {
                    jQuery("<span>" + this.opts.ellipse_text + "</span>").appendTo(fragment);
                }
                begin = Math.max(np - this.opts.num_edge_entries, interval.end);
                this.appendRange(fragment, current_page, begin, np, { classes: 'ep' });

            }
            // “下一页”链接
            if (this.opts.next_text && (current_page < np - 1 || this.opts.next_show_always)) {
                fragment.append(this.createLink(current_page + 1, current_page, { text: this.opts.next_text, classes: "next" }));
            }
            $('a', fragment).click(eventHandler);
            return fragment;
        }
    });

    // jquery扩展
    $.fn.pagination = function (maxentries, opts) {

        //初始化Options的默认值
        opts = jQuery.extend({
            items_per_page: 10,
            num_display_entries: 10,
            current_page: 0,
            num_edge_entries: 1,
            link_to: "#",
            prev_text: "上一页",
            next_text: "下一页",
            ellipse_text: "...",
            prev_show_always: true,
            next_show_always: true,
            renderer: "defaultRenderer",
            load_first_page: true,
            callback: function () { return false; }
        }, opts || {});

        var containers = this,
            renderer, links, current_page;

        /**
        * 分页链接事件处理函数
        * @参数 {int} page_id ：新的页码
        */

        function paginationClickHandler(evt) {
            var links,
                new_current_page = $(evt.target).data('page_id'),
                continuePropagation = selectPage(new_current_page);
            if (!continuePropagation) {
                evt.stopPropagation();
            }
            return continuePropagation;
        }

        /**
        * 内部事件处理函数
        * 在分页容器对象上设置新的分页
        * 根据分页链接和回调生成新的html
        * 回调函数
        */

        function selectPage(new_current_page) {
            // 更新容器中的所有链接显示
            containers.data('current_page', new_current_page);
            links = renderer.getLinks(new_current_page, paginationClickHandler);
            containers.empty();
            links.appendTo(containers);
            // 如果返回true，则调用回调函数以及一些事件。
            var continuePropagation = opts.callback(new_current_page, containers);
            return continuePropagation;
        }

        // -----------------------------------
        // 分页容器初始化
        // -----------------------------------
        current_page = opts.current_page;
        containers.data('current_page', current_page);
        // 为 maxentries 和 items_per_page赋值
        maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
        opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0) ? 1 : opts.items_per_page;

        if (!$.PaginationRenderers[opts.renderer]) {
            throw new ReferenceError("Pagination renderer '" + opts.renderer + "' was not found in jQuery.PaginationRenderers object.");
        }
        renderer = new $.PaginationRenderers[opts.renderer](maxentries, opts);

        // 给相应的dom元素绑定事件
        var pc = new $.PaginationCalculator(maxentries, opts);
        var np = pc.numPages();
        containers.bind('setPage', { numPages: np }, function (evt, page_id) {
            if (page_id >= 0 && page_id < evt.data.numPages) {
                selectPage(page_id);
                return false;
            }
        });
        containers.bind('prevPage', function (evt) {
            var current_page = $(this).data('current_page');
            if (current_page > 0) {
                selectPage(current_page - 1);
            }
            return false;
        });
        containers.bind('nextPage', { numPages: np }, function (evt) {
            var current_page = $(this).data('current_page');
            if (current_page < evt.data.numPages - 1) {
                selectPage(current_page + 1);
            }
            return false;
        });

        // 当所有的初始化完成后，输入链接
        links = renderer.getLinks(current_page, paginationClickHandler);
        containers.empty();
        links.appendTo(containers);
        // 回调函数
        if (opts.load_first_page) {
            opts.callback(current_page, containers);
        }
    }; // 分页插件结束

})(jQuery);

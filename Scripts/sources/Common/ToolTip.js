(function ($) {
    $.fn.ToolTip = function (vOptions) {

        var mSpan;

        // create only 1 span for all text tool tip
        if (!mSpan) {
            mSpan = $('<span>');
            mSpan.css('padding', '10px');
            mSpan.css('position', 'absolute');
            mSpan.css('z-index', 1002);
            mSpan.addClass('ui-widget-content ui-corner-all Shadow');
            mSpan.hide();
            $(document.body).append(mSpan);
        };

        this.each(function (vIndex, vItem) {
            var $this = $(this);
            var mTitle;

            if ($this.attr('title')) {
                mTitle = $this.attr('title');
                $this.mouseenter(function (e) {
                    mSpan.css('opacity', 1);
                    mSpan.text(mTitle);
                    $this.removeAttr('title');
                });
                $this.mousemove(function (e) {
                    mSpan.show(); // reposition the tooltip when user hovers over tooltip
                    mSpan.position({
                        my: 'left top',
                        of: e,
                        offset: '20 20',
                        collision: 'fit flip'
                    });
                });
                $this.mouseleave(function (e) {
                    $this.attr('title', mTitle);
                    mSpan.hide();
                });
                $this.click(function(e) {
                    $this.attr('title', mTitle);
                    mSpan.hide();
                });
            };
        });
    };

    $.ToolTip = function () {
        $('*').ToolTip();
    };
})(jQuery);
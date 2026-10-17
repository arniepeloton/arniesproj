(function ($) {
    $(function() {
        IeWarning = function() {
            if ($.browser.msie && $.browser.version < 8) {
                var pMessage = 'To properly view this website, please use a modern browser. ' + 
                    '<a href="http://www.google.com/chrome/" style="text-decoration:underline" target="_blank">Chrome</a>, ' + 
                    '<a href="http://www.mozilla.com" style="text-decoration:underline" target="_blank">Firefox</a>, or ' + 
                    '<a href="http://www.apple.com/safari/download/" style="text-decoration:underline" target="_blank">Safari</a>';

                $('<div>')
                    .html(pMessage)
                    .css({'position': 'absolute',
                        'top': '-35px',
                        'width': '100%',
                        'background-color': 'yellow',
                        'padding': '10px',
                        'border-bottom': '1px solid'
                    })
                    .appendTo($('body'))
                    .delay(3000)
                    .animate({'top': 0})
                    .delay(1000)
                    .effect('pulsate')
                    ;

                ShowMessage(pMessage, "Warning", null, true);
            };
        }();
    });
})(jQuery);
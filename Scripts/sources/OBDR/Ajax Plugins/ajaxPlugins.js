(function ($) {
    $.fn.extend({
        /* ajax with return value */
        ajaxSaveWithParameter: function (obj) {
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function () {
                //Ajax Call
            });
        }
    });

    function saveAjaxWithParameters(obj) {
        $.ajax({
            type: "POST",
            url: obj["url"],
            dataType: "json",
            data: JSON.stringify(obj["input-value"]),
            beforeSend: function () {
            }, success: function (data) {
            }, error: function () {
            }
        });
    }
})(jQuery);
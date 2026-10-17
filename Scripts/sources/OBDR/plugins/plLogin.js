(function ($) {
    $.fn.extend({
        UserLogin: function (objValue) {
            var defaults = objValue;

            objValue = $.extend(defaults, objValue);
            return $(this).each(function () {
                Login(objValue);
            });
        }
    });

    function Login(objValue) {
        $.ajax({
            type: "POST",
            url: "Verify/",
            dataType: "json",
            data: JSON.stringify(objValue),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.Text;
                var eData = data.d.Error;
                if (mData == "Ok") {
                    window.location = "../";
                } else {
                    //Nothing
                }
            }, error: function () {
            }
        });
    }
})(jQuery);
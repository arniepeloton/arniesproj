require([
    'common',
    ], function () {
        $.mobile.activePage.bind('pageshow', function() {
            PageShow();
        });
        
        $(document).ready(function () {
            PageShow();
        });

        function PageShow() {
            //var pUrlParams = GetUrlParameters();
            //if (pUrlParams.CustomerScreen) {
                require(['promise!/Scripts3/controller/CustomerScreen.js?crc='], function() {
                    // history.back(); // no need for history_back
                    var p = new App.controller.CustomerScreen();
                    p.popup();
                });
                return;
            //}
        }
    }
);


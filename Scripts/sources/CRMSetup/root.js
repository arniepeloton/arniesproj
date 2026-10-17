require([
    'common'
    ], function () {
        $(function () {
            $("#Teams").click(function () {
                window.open("Menu/", "_self");
            });

            $("#Done").click(function () {
                window.open("../", "_self");
            });
        });
    });


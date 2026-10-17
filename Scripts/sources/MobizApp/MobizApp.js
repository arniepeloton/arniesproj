require([
    'common'
    ], function () {

        $(function () {

            $('#lnkMobizAppUpdate').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/MobizApp/DownloadFile");

            });

            $('#btnMobizAppUpdateQR').click(function (e) {
                e.preventDefault();

                $('#lnkMobizAppUpdate').click();

            });

        });
    });

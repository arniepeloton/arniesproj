require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {


        $(function () {
        
            $('#lnkDownloadGenTemplate').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/PIMTemplate/DownloadTemplate");

            });



    });
});
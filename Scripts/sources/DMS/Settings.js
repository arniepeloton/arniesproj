require(['common'], function () {

    $(document).on("mobileinit", function () {
        $.mobile.ajaxEnabled = false;
        $.mobile.loadingMessage = false;
    });

    require(['/Content/bootstrap-5.0.0/js/bootstrap.js'], function () {
        $(document).ready(function () {
            $('#btnAddMetadata').click(function (e) {
                $('#popupMetadata').modal('show');
            })

            $('#btnAddDocumentCategory').click(function (e) {
                $('#popupDocumentCategory').modal('show');
            })

            $('#btnAddDocumentType').click(function (e) {
                $('#popupDocumentType').modal('show');
            })

            $('#btnAddArchiveLocation').click(function (e) {
                $('#popupArchiveLocation').modal('show');
            })
        })
    })
})
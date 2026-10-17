require(['common'], function () {
    $(document).on("mobileinit", function () {
        $.mobile.ajaxEnabled = false;
        $.mobile.loadingMessage = false;
    });
    require(['/Content/bootstrap-5.0.0/js/bootstrap.js'], function () {
        $(document).ready(function () {
            $('#btnAddbatch').click(function () {
                $('#staticBackdrop').modal('show');
            })
        })
    })    
})
require(['common'
    ], function() {

    $(function () {

        $('#formBomUpload').submit(function(e) {
            e.preventDefault();
            var pData = new FormData($(this)[0]);

            AjaxSend({
                url: '/PIMProduct/BomUploadFile',
                data: pData,
                success: function(vData) {
                    MessageBox('Success during upload: ' + JSON.stringify(arguments));
                },
                error: function() {
                    MessageBox('Error during upload: ' + JSON.stringify(arguments));
                }
            });
        });

    }); 
});
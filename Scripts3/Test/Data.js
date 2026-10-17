require(['/Scripts3/Test/common.js?crc='
    ], function() {
     
    // $.bind('pageshow', function() {
    $(function() {

        AjaxSend({
            url: '/Test/DataTest',
            success: function(vData) {
                Test.AppendResult($('#divTestData'), vData);
            }
        });

    }); // $.bind('pageshow', function() {

}); 
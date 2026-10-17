require(['common'
    ], function() {

    $(function () {

        var mCharStart;
        var mMessage = '';

        setInterval(GetStatus, 5000);

        function GetStatus() {

            AjaxSend({
                url: 'ServerAgent/GetStatus',
                data: {CharStart: mCharStart},
                success: function(vData) {
                    var pText;

                    if (CStr2(vData.Message) != '') {
                        mMessage += vData.Message;
                        mCharStart = mMessage.length;
                        pText = HtmlEncode(mMessage);
                        pText = pText.replace(/\r\n/g, '<br />');
                        $('#divLog').html(pText);
                        $('#spanLastResponse').text(new Date().toLocaleString());
                    };

                }, // success
                error: function(vData) {
                    console.log('Server error');
                    $('#spanError').text(new Date().toLocaleString());
                }
            }); // AjaxSend

        }; // GetStatus

    }); 
});
require(['common'], function () {

    var divLog = $('#divLog');

    $('#butLongProcess1').click(function() {
        AddLog('BEGIN LongProcessCall');
        AjaxSend({
            url: 'AndroidJsTest/LongProcessCall',
            success: function(vData) {
                AddLog('SUCCESS: AndroidJsTest/LongProcessCall');
            },
            error: function() {
                AddLog('ERROR: AndroidJsTest/LongProcessCall');
            }
        });
    });

    $('#butLongProcess2').click(function() {
        AddLog('BEGIN LongProcessCall2');
        AjaxSend({
            url: 'AndroidJsTest/LongProcessCall2',
            success: function(vData) {
                AddLog(vData.Result);
            },
            error: function() {
                AddLog('ERROR: LongProcessCall2');
            }
        });
    });

    $('#butFollowUp').click(function() {
        AddLog('BEGIN LongProcessCall2FollowUp');
        AjaxSend({
            url: 'AndroidJsTest/LongProcessCall2FollowUp',
            success: function(vData) {
                AddLog(vData.Result);
            },
            error: function() {
                AddLog('ERROR: LongProcessCall2FollowUp');
            }
        });
    });

    $('#butClear').click(function() {
        
        divLog.html('');

    });

    function AddLog(vText) {

        divLog.append(
            $('<div>').text($.format.date(new Date(), 'h:mm:ss a') + vText)
        );

    };

}); // define
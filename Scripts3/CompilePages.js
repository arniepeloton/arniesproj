require(['common'
    ], function () {

    var mLastTick = 0;
    var mCompile = true;
    var mDone;

    setTimeout(GetStatus, 1000);

    function GetStatus() {

        $('#divMessage').text('In progress');
        AjaxSend({
            url: '/CompilePages/GetStatus',
            data: {
                LastTick: mLastTick,
                Compile: mCompile
            },
            success: function(vData) {
                var pMessages = vData.Messages;

                if (pMessages == undefined)
                    return;

                mCompile = false;
                mLastTick = vData.LastTick;
                for (var p = 0; p <= pMessages.length - 1; p++) {
                    $('#divLog')
                        .append($('<div>')
                            .text(pMessages[p])
                        );
                }
                mDone = CBool(vData.Done);
                if (mDone) {
                    $('#divMessage').text('Done');
                    document.title = 'Done';
                }
            },
            error: function(vData) {
                vData = $.extend({}, vData);
                $('#divMessage').text('Error: ' + GetStringObject('statusText', vData));
                document.title = 'Error';
            },
            complete: function() {
                if (!mDone)
                    setTimeout(GetStatus, 5000);
            }
        });

    }

    $('#butClearLog').click(function() {
        $('#divLog').html('');
    });

}); // define
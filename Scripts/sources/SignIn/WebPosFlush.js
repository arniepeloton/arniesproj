require([
    'common'
    ], function () {

    var mLastTimeStamp;

    $(function () {
        mLastTimeStamp = new Date();
        GetRecent();
    });

    function GetRecent() {
        AjaxSend({
            url: '/WebPosFlush/GetRecentMessages',
            data: {
                LastTimeStamp: mLastTimeStamp
            },
            success: function(vData) {
                if ($.isArray(vData)) {
                    for (var pItem of vData) {
                        mLastTimeStamp = pItem.TimeStamp;
                        AddMessage(pItem);
                    }
                    if (vData.length > 0)
                        $('html,body').animate({scrollTop: document.body.scrollHeight},"slow"); // https://stackoverflow.com/a/35618752/334045
                } else
                    AddMessage({
                        Text: vData
                    });
            },
            complete: function() {
                setTimeout(GetRecent, 3000);
            }
        });
    }

    function AddMessage(vMsg) {
        var pParent = $('#divProgressLog');
        var pText = '[' + FormatDate(vMsg.TimeStamp, {Format: 'MM/dd/yyyy hh:mm:ss a'}) + '] ' + vMsg.Text;
        var pDiv = $('<div>')
            .text(pText);
        pParent.append(pDiv);
    }
});
    
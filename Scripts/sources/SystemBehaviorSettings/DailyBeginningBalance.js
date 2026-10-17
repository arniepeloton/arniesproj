require([
    'common',
], function () {

    var IsSaveDailyBeginningBalance = 0;

    $(function () {
        window.DailyBegBal = {
            GetSystemBehavior: function () {
                GetSystemBehavior()
            }
        }
        GetSystemBehavior();

        $('body')
        .on('change', '#selDailyBeginningBalance', function () {
            IsSaveDailyBeginningBalance = CInt2($('#selDailyBeginningBalance').is(':checked'));
        })
        .on('click', '#btnSaveDailyBeginningBalanceSettings', function () {
            SaveDailyBeginningBalanceSettings();
        })
        .on('click', '#btnDone', function () {
            location.hash = '';
        })
    });


    function SaveDailyBeginningBalanceSettings() {
        var data = {
            IsSaveDailyBeginningBalance: IsSaveDailyBeginningBalance,
        };

        AjaxSend({
            url: 'SystemBehaviorSettings/SaveDailyBeginningBalanceSettings',
            data: data,
            success: function (vData) {
                if (vData.Result == "Success") {
                    history.back();
                }
            }
        });

    }

    function GetSystemBehavior() {
        AjaxSend({
            url: 'SystemBehaviorSettings/GetDailyBeginningBalanceSettings',
            success: function (vData) {


                if (vData.IsSaveDailyBeginningBalance == 1) {
                    $('#selDailyBeginningBalance').prop('checked', true);
                } else {
                    $('#selDailyBeginningBalance').removeAttr('checked');
                }

                //try {
                //    $('#selDailyBeginningBalance').slider('refresh');
                //}
                //catch (e) {
                //    $('#selDailyBeginningBalance').slider();
                //}

            }
        });
    }
});
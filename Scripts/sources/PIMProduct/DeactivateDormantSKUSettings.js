require([ 'common' ], function () {

    $(document).ready(function () {
        PopulateDays();

        window.DormantSettings = {
            GetData: PopulateDays
        }

        $('body')
        .on('click', '#btnCancelDormant', function (e) {
            location.hash = 'LibraryPage';
        })
        .on('click', '#btnSaveDormant', function (e) {
            var dormantperiod = $('#selDormantPeriod').val();
            var isautocleanup = CInt2($('#selDormantSKUsIsAutoCleanUp').is(':checked'));

            if (dormantperiod == '') {
                NewMessageBox('Please select dormant period');
                return;
            }

            AjaxSend({
                url: 'PIMProduct/SaveDormantSettings',
                data: {
                    data: {
                        DormantPeriod: dormantperiod,
                        IsAutoCleanUp: isautocleanup
                    }
                }, success: function (e) {
                    if (e.Results == 'success') {
                        WebToast('Successfully saved', 'top');
                        location.hash = 'LibraryPage';
                    } else {
                        NewMessageBox(e.Results, 'Error');
                    }
                }
            })
        })

        function GetDormantSettings() {
            AjaxSend({
                url: 'PIMProduct/GetDormantSettings',
                success: function (e) {
                    $('#selDormantPeriod').val(e.DormantSKUsPeriod == '' ? 365 : e.DormantSKUsPeriod);
                    $('#selDormantSKUsIsAutoCleanUp').prop('checked', CInt2(e.IsAutoCleanUpDormantSKUs));
                }
            })
        }

        function PopulateDays() {
            //var days = 182;

            //for (var i = 0; i < 6; i++) {
            //    $('#selDormantPeriod').prepend(new Option(days + ' days', days));
            //    days += 182;
            //}

            //$('#selDormantPeriod').prepend(new Option('Please select', ''));
            //$('#selDormantPeriod').val('365');
            GetDormantSettings();
        }
    })
});
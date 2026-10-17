require(['common',
    GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc=')
    ], function () {

        $(function () {

            GetSettings();

            window.SalesReturn = {
                GetSettings: function () {
                    GetSettings();
                }
            }

            $('body').on('click', '#butSalesReturnSave', function () {
                if (Verify())
                    Save();
            })
            .on('click', '#butCancel', function () {
                window.location.hash = 'SystemAdministration';
            })

            function GetSettings() {

                AjaxSend({
                    url: 'SalesReturns/GetSettings',
                    success: function (vData) {
                        if (vData.Error != undefined)
                            MessageBox(vData.Error, 'Error');
                        else
                            LoadSettings(vData);
                    }
                }); // AjaxSend

            }; // function GetSettings

            function LoadSettings(vData) {

                $('#txtRestockingFeePercent').val(vData.RestockingFeePercent);
                $('#txtMaximumReturnablePeriod').val(vData.MaximumReturnablePeriod);
                $('#cbReturnedPrice')
                    .val(vData.ReturnedPrice)
                    //.selectmenu('refresh', true);

                $('#cbSRVUse')
                    .val(vData.SRVUseSettings)
                    //.selectmenu('refresh', true);

                $('#cbPrintRefundSignUp')
                    .val(vData.PrintRefundSignUp)
                    //.selectmenu('refresh', true);
                $('#chkHideMerchandiseExchange').prop('checked', vData.HideMerchandiseExchange);
                $('#chkRestrictedTransactionLookupForSalesReturn').prop('checked', vData.RestrictedTransactionLookupForSalesReturn);
                $('#txtAllowedReturnDuration').val(vData.AllowedReturnDuration);
            }; // function LoadSettings

            function Verify() {

                if (!IsNumeric($('#txtRestockingFeePercent').val())) {
                    MessageBox('Restocking Free Percent must be a valid number');
                    return false;
                };

                if (!IsNumeric($('#txtMaximumReturnablePeriod').val())) {
                    MessageBox('Maximum Returnable Period must be a valid number');
                    return false;
                };

                if ($('#cbReturnedPrice').val() == 'invalid') {
                    MessageBox('Please choose "When returned item has different price points (due to price breaks or volume pricing)"');
                    return false;
                };

                if ($('#cbSRVUse').val() == 'invalid') {
                    MessageBox('Please set SRV use settings.');
                    return false;
                };

                if ($('#cbPrintRefundSignUp').val() == 'invalid') {
                    MessageBox('Please choose "Print out a refund sign-up slip every time a refund transaction is made"');
                    return false;
                };

                if (!IsNumeric($('#txtAllowedReturnDuration').val()) 
                    || CDec2($('#txtAllowedReturnDuration').val()) < 0) {
                    MessageBox('Allowed Return Duration (Minutes) must be a valid number equal to or greater than zero.');
                    return false;
                };

                return true;

            }; // function Verify

            function Save() {

                AjaxSend({
                    url: 'SalesReturns/SaveSettings',
                    data: {
                        RestockingFeePercent: $('#txtRestockingFeePercent').val(),
                        MaximumReturnablePeriod: $('#txtMaximumReturnablePeriod').val(),
                        ReturnedPrice: $('#cbReturnedPrice').val(),
                        SRVSettings: $('#cbSRVUse').val(),
                        PrintRefundSignUp: $('#cbPrintRefundSignUp').val(),
                        HideMerchandiseExchange: $('#chkHideMerchandiseExchange').prop('checked'),
                        RestrictedTransactionLookupForSalesReturn: $('#chkRestrictedTransactionLookupForSalesReturn').prop('checked'),
                        AllowedReturnDuration: $('#txtAllowedReturnDuration').val(),
                    },
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            //MessageBox(vData.Error, 'Error');
                            $('#panSearchLibrary').popup('close');
                            $('#TitleOrgEmp').text('Error');
                            $('#MsgOrgEmp').text(vData.Error);
                            $('#panOrgEmpMessage').popup('open');
                            $('#btnOKOrgEmp').click(function (e) {
                                e.preventDefault();
                                $('#panOrgEmpMessage').popup('close');
                            })

                        } else if (vData.Result == 'ok') {
                            //MessageBox('Successfully saved');
                            $('#panSearchLibrary').popup('close');
                            $('#TitleOrgEmp').text('Sales Return');
                            $('#MsgOrgEmp').text('Successfully saved');
                            $('#panOrgEmpMessage').popup('open');
                            $('#btnOKOrgEmp').click(function (e) {
                                e.preventDefault();
                                window.location.hash = "SystemAdministration";
                                $('#panOrgEmpMessage').popup('close');
                            })
                        };
                    }
                }); // AjaxSend

            }; // function Save
        }); // $(function() {

    });
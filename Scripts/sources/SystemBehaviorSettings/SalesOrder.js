require([
    'common',
    ], function () {
        var IsShowTableNo = 0;
        var IsShowGasPumpNo = 0;

        $(function () {
            GetSystemBehavior();

            window.SystemBehavior = {
                GetSystemBehaviorData: function () {
                    GetSystemBehavior();
                }
            }

            $('body').on('click', '#btnDone', function () {
                //history.back();
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnSaveSalesOrderSetting', function () {
                SaveSystemBehavior();
            })
        });

        function GetSystemBehavior() {
            AjaxSend({
                url: 'SystemBehaviorSettings/GetSalesOrderBehavior',
                success: function (vData) {

                    if (vData.IsEnableInventoryCountMode == 1) {
                        $('#selEnableInventoryCountMode').prop('checked', true);
                    } else {
                        $('#selEnableInventoryCountMode').removeAttr('checked');
                    }
                    //try {
                    //    $('#selEnableInventoryCountMode').slider('refresh');
                    //}
                    //catch (e) {
                    //    $('#selEnableInventoryCountMode').slider();
                    //}

                    if (vData.RequireSignature == 1) {
                        $('#selRequireSignature').prop('checked', true);;
                    } else {
                        $('#selRequireSignature').removeAttr('checked');
                    }
                    //try {
                    //    $('#selRequireSignature').slider('refresh');
                    //}
                    //catch (e) {
                    //    $('#selRequireSignature').slider();
                    //}

                    $('#selRequireCustomerOutlet').prop('checked', CBool(vData.RequireCustomerOutlet) ? 1 : 0);
                    //try {
                    //    $('#selRequireCustomerOutlet').slider('refresh');
                    //}
                    //catch (e) {
                    //    $('#selRequireCustomerOutlet').slider();
                    //}

                    $('#selRequireCustomerDepartment').prop('checked', CBool(vData.RequireCustomerDepartment) ? 1 : 0);
                    //try {
                    //    $('#selRequireCustomerDepartment').slider('refresh');
                    //}
                    //catch (e) {
                    //    $('#selRequireCustomerDepartment').slider();
                    //}

                    $('#selShowQtyAvailableToCommit').prop('checked', CBool(vData.ShowQtyAvailableToCommit) ? 1 : 0);
                    //try {
                    //    $('#selShowQtyAvailableToCommit').slider('refresh');
                    //}
                    //catch (e) {
                    //    $('#selShowQtyAvailableToCommit').slider();
                    //}


                    //                    if (vData.Results.Rows.length > 0) {
                    //                        if (parseInt(vData.Results.Rows[0].DefaultValue)) {
                    //                            $('#selEnableInventoryCountMode').val('1');
                    //                        }
                    //                        else {
                    //                            $('#selEnableInventoryCountMode').val('0');
                    //                        }
                    //                    }

                    //                    try {
                    //                        $('#selEnableInventoryCountMode').slider('refresh');
                    //                    }
                    //                    catch (e) {
                    //                        $('#selEnableInventoryCountMode').slider();
                    //                    }


                }
            });
        }


        function validateNumber(number) {
            var re = /^[0-9]*$/;

            return re.test(number);
        }

        function SaveSystemBehavior() {

            var data = {
                IsEnableInventoryCountMode: $.trim($("#selEnableInventoryCountMode").is(':checked')),
                RequireSignature: $.trim($("#selRequireSignature").is(':checked')),
                RequireCustomerOutlet: $("#selRequireCustomerOutlet").is(':checked'),
                RequireCustomerDepartment: $("#selRequireCustomerDepartment").is(':checked'),
                ShowQtyAvailableToCommit: $("#selShowQtyAvailableToCommit").is(':checked')
            };

            AjaxSend({
                url: 'SystemBehaviorSettings/SaveSalesOrderBehavior',
                data: data,
                success: function (vData) {
                    if (vData.Result == "Success") {
                        //history.back();
                        WebToast('Successfully saved', 'top');
                        window.location.hash = 'SystemAdministration';
                    }
                    else {
                        NewMessageBox('An error occured while saving Sales Order settings.', 'Sales Order Settings');
                    }
                }
            });
        }
    });
require(['common'],
    function () {
        $(document).ready(function () {
            window.EcommerceVendorPortalSettings = {
                LoadData: GetEcommerceGeneralSettings
            }

            GetEcommerceGeneralSettings();

            $('body')
            .on('click', '#btnCancelEcommerceVendorPortalSettings', function () {
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnSaveEcommerceVendorPortalSettings', function () {
                if (!ValidateVendorPortalName())
                    return;
                AjaxSend({
                    url: 'Ecommerce/SaveEcommerceVendorPortalSettings',
                    data: {
                        EcommerceIsShowVendor3rdPartyCustomerName: $.trim(CInt2($('#selIsShowVendor3rdPartyCustomerName').is(':checked'))),
                        EcommerceVendorPortalName: $.trim($('#txtEcommerceVendorPortalName').val())
                    }, success: function (e) {
                        if (e.Result == 'Success') {
                            WebToast('Vendor Portal Settings has been saved', 'top');
                            window.location.hash = 'SystemAdministration';
                        }
                    }, error: function (e) {
                        NewMessageBox(e, 'Error');
                    }
                })
            })

            function ValidateVendorPortalName() {
                var pVal = $('#txtEcommerceVendorPortalName').val();
                var pVal2 = pVal.replace(/[^A-Za-z0-9]/gi, '');
                if (pVal != pVal2) {
                    var pMsg = 'Your vendor name contains invalid characters. Valid characters are alphanumeric. All other characters (symbols, spaces, underscores, etc.) are invalid.';
                    //$('#spanEcommerceVendorPortalNameError')
                    //    .text(pMsg)
                    //    .show();
                    NewMessageBox(pMsg, 'Please Check Your Vendor Portal Name');
                    return false;
                }
                $('#spanEcommerceVendorPortalNameError').hide();
                return true;
            }

            function GetEcommerceGeneralSettings() {
                AjaxSend({
                    url: 'Ecommerce/GetEcommerceVendorPortalSettings',
                    success: function (e) {
                        $('#selIsShowVendor3rdPartyCustomerName').prop('checked', CInt2(e.EcommerceIsShowVendor3rdPartyCustomerName));//.slider('refresh');
                        $('#txtEcommerceVendorPortalName').val(e.EcommerceVendorPortalName);
                        DisplayStoreUrl();
                    }
                })
            }

            function DisplayStoreUrl() {
                $('#divEcommerceVendorPortalUrl').text(location.origin + '/vportal/' + $('#txtEcommerceVendorPortalName').val());
            }
        })
    })
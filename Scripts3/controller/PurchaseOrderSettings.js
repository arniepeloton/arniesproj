define([
    'common',
    //'/Scripts3/Browser/LibraryOrganization.js?crc=',
    //'/Scripts3/Browser/LibraryFowst.js?crc=',
    //'/Scripts3/Browser/LibraryOrganizationSupplier.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

    })();

    $(function () {

        CreateStringObject('App.controller.PurchaseOrderSettings', null, function () {
            
            var mOut = {};
            var mSupplierID;
            var mSupplierFowstID;
            var mCustomerID;
            var mCustomerFowstID;

            mOut.Init = function() {

                $('#butPurchaseOrderSettingsSupplier').click(function () {
                    BrowsePopup('LibraryOrganization', 'Select Supplier', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                $('#butPurchaseOrderSettingsSupplier').text(e.CompanyName)//.button().button('refresh');
                                mSupplierID = e.ID;
                            }
                        }
                    ], {
                        FilterSupplier: true
                    })
                    //var pBrowser = Browser.LibraryOrganizationSupplier;
                    //pBrowser.Click = function(e) {
                        
                    //    setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                    //};
                    //pBrowser.Show();
                });

                $('#butPurchaseOrderSettingsSupplierFowst').click(function() {
                    var pBrowser;
                    if (!IsGuid(mSupplierID)) {
                        NewMessageBox('Please select a supplier first.');
                        return;
                    }

                    BrowsePopup('LibraryFowst', 'Select Supplier FOWST', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }, { Column: 'Address'}],
                            Click: function (e) {
                                $('#butPurchaseOrderSettingsSupplierFowst').text(e.FowstName)//.button().button('refresh');
                                mSupplierFowstID = e.ID;
                            }
                        }
                    ], {
                        OrganizationID: mSupplierID
                    })

                    //pBrowser = Browser.LibraryFowst;
                    //pBrowser.InputData.OrganizationID = mSupplierID;
                    //pBrowser.Click = function(e) {
                        
                    //    setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                    //};
                    //pBrowser.Show();
                });

                $("[name=radioPurchaseOrderSettings]").click(function() {
                    UpdateSupplierText();
                });

                $('#butPurchaseOrderSettingsCustomer').click(function () {
                    BrowsePopup('LibraryOrganization', 'Select Customer', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                $('#butPurchaseOrderSettingsCustomer').text(e.CompanyName)//.button().button('refresh');
                                mCustomerID = e.ID;
                            }
                        }
                    ], {
                        FilterCustomers: true
                    })

                    //var pBrowser = Browser.LibraryOrganization;
                    //pBrowser.InputData.FilterCustomers = true;
                    //pBrowser.Click = function (e) {

                    //    setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                    //};
                    //pBrowser.Show();
                });

                $('#butPurchaseOrderSettingsCustomerFowst').click(function() {
                    var pBrowser;
                    if (!IsGuid(mCustomerID)) {
                        NewMessageBox('Please select a customer first.');
                        return;
                    }

                    BrowsePopup('LibraryFowst', 'Select Customer FOWST', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                            Click: function (e) {
                                $('#butPurchaseOrderSettingsCustomerFowst').text(e.FowstName)//.button().button('refresh');
                                mCustomerFowstID = e.ID;
                            }
                        }
                    ], {
                        OrganizationID: mCustomerID
                    })

                    //pBrowser = Browser.LibraryFowst;
                    //pBrowser.InputData.OrganizationID = mCustomerID;
                    //pBrowser.Click = function(e) {
                        
                    //    setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                    //};
                    //pBrowser.Show();
                });

                $("[name=radioPurchaseOrderCustomerSettings]").click(function() {
                    UpdateCustomerText();
                });

                $('#butSavePurchaseOrderSettings').click(butSave_Click);
                GetData();
            };
            return mOut;

            function UpdateSupplierText() {
                var pVal = $("[name=radioPurchaseOrderSettings]:checked").val();
                if (pVal == 'default to') {
                    $('#divPurchaseOrderSettingsSupplierText').text('Please choose the pre-selected supplier:');
                    $('#divPurchaseOrderSettingsSupplierButtonsContainer').show();
                } else if (pVal == 'locked to') {
                    $('#divPurchaseOrderSettingsSupplierText').text('Please choose the locked-in supplier:');
                    $('#divPurchaseOrderSettingsSupplierButtonsContainer').show();
                } else
                    $('#divPurchaseOrderSettingsSupplierButtonsContainer').hide();
            }

            function UpdateCustomerText() {
                var pVal = $("[name=radioPurchaseOrderCustomerSettings]:checked").val();
                if (pVal == 'default to') {
                    $('#divPurchaseOrderSettingsCustomerText').text('Please choose the pre-selected customer:');
                    $('#divPurchaseOrderSettingsCustomerButtonsContainer').show();
                } else if (pVal == 'locked to') {
                    $('#divPurchaseOrderSettingsCustomerText').text('Please choose the locked-in customer:');
                    $('#divPurchaseOrderSettingsCustomerButtonsContainer').show();
                } else
                    $('#divPurchaseOrderSettingsCustomerButtonsContainer').hide();
            }

            function butSave_Click() {
                
                var pData = {
                    SupplierSetting: $("[name=radioPurchaseOrderSettings]:checked").val(),
                    SupplierID: mSupplierID,
                    SupplierFowstID: mSupplierFowstID,
                    CustomerSetting: $("[name=radioPurchaseOrderCustomerSettings]:checked").val(),
                    CustomerID: mCustomerID,
                    CustomerFowstID: mCustomerFowstID,
                    HideSomeSopaFields: CInt2($('#selHideSomeSopaFields').is(':checked')),
                    DeliveryMethodWhenAutoPO: $('#selDeliveryMethodWhenAutoPO').val()
                };

                AjaxSend({
                    url: '/SystemBehaviorSettings/SavePurchaseOrderSettings',
                    data: pData,
                    success: function(vData) {
                        vData = $.extend({}, vData);
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Result == 'ok')
                            NewMessageBox('Settings have been saved successfully.');
                    }
                });

            }

            function GetData() {
                AjaxSend({
                    url: '/SystemBehaviorSettings/GetPurchaseOrderSettings',
                    success: function(vData) {
                        vData = $.extend({}, vData);
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else
                            DisplayData(vData);
                    }
                });
            }

            function DisplayData(vData) {
                $("#selHideSomeSopaFields").prop('checked', CBool(vData.HideSomeSopaFields) ? 1 : 0);//.slider('refresh');
                $("#selDeliveryMethodWhenAutoPO").val(vData.DeliveryMethodWhenAutoPO);//.selectmenu('refresh');

                mSupplierID = vData.SupplierID;
                $(':radio[name=radioPurchaseOrderSettings][value="' + vData.SupplierSetting + '"]').prop('checked', true);
                $('#butPurchaseOrderSettingsSupplier').text(vData.SupplierName);//.button().button('refresh');
                mSupplierFowstID = vData.SupplierFowstID;
                $('#butPurchaseOrderSettingsSupplierFowst').text(vData.SupplierFowstName);//.button().button('refresh');

                mCustomerID = vData.CustomerID;
                $(':radio[name=radioPurchaseOrderCustomerSettings][value="' + vData.CustomerSetting + '"]').prop('checked', true);
                $('#butPurchaseOrderSettingsCustomer').text(vData.CustomerName);//.button().button('refresh');
                mCustomerFowstID = vData.CustomerFowstID;
                $('#butPurchaseOrderSettingsCustomerFowst').text(vData.CustomerFowstName);//.button().button('refresh');
                
                //$(':radio').checkboxradio('refresh');
                UpdateSupplierText();
                UpdateCustomerText();
            }

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
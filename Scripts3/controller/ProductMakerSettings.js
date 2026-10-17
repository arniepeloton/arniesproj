define([
    'common'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

    })();

    $(function () {

        CreateStringObject('App.controller.ProductMakerSettings', null, function () {
            
            var mOut = {};
            var mSupplierID;
            var mSupplierFowstID;
            var mCustomerID;
            var mCustomerFowstID;
            var mSelProductMakerSettingsDefaultHotButtonTemplate;

            mOut.Init = function() {

                //$('#butPurchaseOrderSettingsSupplier').click(function() {
                //    var pBrowser = Browser.LibraryOrganizationSupplier;
                //    pBrowser.Click = function(e) {
                //        $('#butPurchaseOrderSettingsSupplier').text(e.CompanyName).button().button('refresh');
                //        mSupplierID = e.ID;
                //        setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                //    };
                //    pBrowser.Show();
                //});

                //$('#butPurchaseOrderSettingsSupplierFowst').click(function() {
                //    var pBrowser;
                //    if (!IsGuid(mSupplierID)) {
                //        NewMessageBox('Please select a supplier first.');
                //        return;
                //    }
                //    pBrowser = Browser.LibraryFowst;
                //    pBrowser.InputData.OrganizationID = mSupplierID;
                //    pBrowser.Click = function(e) {
                //        $('#butPurchaseOrderSettingsSupplierFowst').text(e.FowstName).button().button('refresh');
                //        mSupplierFowstID = e.ID;
                //        setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                //    };
                //    pBrowser.Show();
                //});

                //$("[name=radioPurchaseOrderSettings]").click(function() {
                //    UpdateSupplierText();
                //});

                //$('#butPurchaseOrderSettingsCustomer').click(function() {
                //    var pBrowser = Browser.LibraryOrganization;
                //    pBrowser.InputData.FilterCustomers = true;
                //    pBrowser.Click = function(e) {
                //        $('#butPurchaseOrderSettingsCustomer').text(e.CompanyName).button().button('refresh');
                //        mCustomerID = e.ID;
                //        setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                //    };
                //    pBrowser.Show();
                //});

                //$('#butPurchaseOrderSettingsCustomerFowst').click(function() {
                //    var pBrowser;
                //    if (!IsGuid(mCustomerID)) {
                //        NewMessageBox('Please select a customer first.');
                //        return;
                //    }
                //    pBrowser = Browser.LibraryFowst;
                //    pBrowser.InputData.OrganizationID = mCustomerID;
                //    pBrowser.Click = function(e) {
                //        $('#butPurchaseOrderSettingsCustomerFowst').text(e.FowstName).button().button('refresh');
                //        mCustomerFowstID = e.ID;
                //        setTimeout(function(){window.location.hash = 'pageSetUpCheckList'}, 10);
                //    };
                //    pBrowser.Show();
                //});

                //$("[name=radioPurchaseOrderCustomerSettings]").click(function() {
                //    UpdateCustomerText();
                //});

                $('#butSaveProductMakerSettings').click(butSave_Click);
                GetData();
            };
            return mOut;

            function butSave_Click() {
                
                ;
                var pData = {
                    DefaultHotButtonTemplateID: $('#selProductMakerSettingsDefaultHotButtonTemplate').find('option:selected').prop('value')
                };

                AjaxSend({
                    url: '/SystemBehaviorSettings/SaveProductMakerSettings',
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
                    url: '/SystemBehaviorSettings/GetProductMakerSettings',
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
               
                FillCombo();

                function FillCombo() {
                    var pCb = $('#selProductMakerSettingsDefaultHotButtonTemplate');
                    var pOption;

                    pCb.empty();
                    for (var p = 0; p <= vData.HotButtonTemplates.length - 1; p++) {
                        var pDR = vData.HotButtonTemplates[p];
                        pOption = new Option(pDR.Name, pDR.ID);
                        pCb.append(pOption);
                    }

                    pOption = pCb.find('option[value=' + vData.DefaultHotButtonTemplateID + ']');
                    pOption.prop('selected', true);
                    pCb.selectmenu('refresh');
                }
            }

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
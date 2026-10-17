define([
    'common'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

    })();

    $(function () {

        CreateStringObject('App.controller.ErpSettings', null, function () {
            
            var mOut = {};
            var mDefaultPriceID;

            mOut.Init = function() {

                $('#butErpSettingsDefaultPriceLevel').click(function() {
                    BrowsePopup('LibraryPIMPriceLevel', 'Price Level',
                        [
                            {
                                //Search: mTxtCustomer.val(),
                                AutoSearch: true,
                                DisplayText: [{ Column: 'PriceLevel' }],
                                Click: function (e) {
                                    mDefaultPriceID = e.ID;
                                    $('#butErpSettingsDefaultPriceLevel').text(e.PriceLevel);//.button();//.button('refresh');
                                }
                            }
                        ]
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
                });

                $('#butSaveErpSettings').click(butSave_Click);
                GetData();
            };
            return mOut;

            function butSave_Click() {
                
                var pData = {
                    DefaultPriceLevelID: mDefaultPriceID,
                    AvailableCreditLimitComputation: $('#cbErpSettingsAvailableCreditLimitComputation').find('option:selected').prop('value')
                };

                AjaxSend({
                    url: '/SystemBehaviorSettings/SaveErpSettings',
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
                    url: '/SystemBehaviorSettings/GetErpSettings',
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
                mDefaultPriceID = vData.DefaultPriceLevel;
                $('#butErpSettingsDefaultPriceLevel').text(vData.DefaultPriceLevelName);//.button();//.button('refresh');
                $('#cbErpSettingsAvailableCreditLimitComputation').find('option[value=' + vData.AvailableCreditLimitComputation + ']').prop('selected', true);
                $('#cbErpSettingsAvailableCreditLimitComputation').selectmenu('refresh');
            }

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
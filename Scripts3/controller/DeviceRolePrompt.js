define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    $(function () {

        CreateStringObject('App.controller.DeviceRolePrompt', null, function () {

            var mOut = {};
            var mDelay = 10; // seconds

            mOut.model = new App.model.PeerConnect();

            mOut.PromptIfNeeded = function() {
                var pTimeoutHandle;

                if (mOut.model.GetSettings().localRole == 'customer screen') {
                    MessageBox('This device\'s role is customer screen. You will automatically be brought to customer screen in ' + mDelay + ' seconds.', null, [
                        {
                            Text: 'Go to customer screen',
                            Click: function() {
                                clearTimeout(pTimeoutHandle);
                                GotoCustomerScreen();
                            }
                        },
                        {
                            Text: 'Don\'t go to customer screen',
                            Click: function() {
                                clearTimeout(pTimeoutHandle);
                                history.back();
                            }
                        }
                    ]);
                    
                    pTimeoutHandle = setTimeout(GotoCustomerScreen, mDelay * 1000);
                }
            };

            function GotoCustomerScreen() {
                Android.Toast('GotoCustomerScreen');
                require(['promise!/Scripts3/controller/CustomerScreen.js?crc='], function() {
                    // history.back(); // no need for history_back
                    var p = new App.controller.CustomerScreen();
                    p.popup();
                });
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
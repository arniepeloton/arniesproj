define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/SecondScreenChangePin.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.SecondScreenChangePin', null, function () {

            var mOut = {};
            var mElement;
            var mTxtSecondScreenChangePinPin;
            var mTxtSecondScreenChangePinConfirmPin;
            var mButSecondScreenChangePinSave;

            mOut.model = new App.model.PeerConnect();
            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.PeerConnect();

            function Init() {
                mTxtSecondScreenChangePinPin = mElement.find('#txtSecondScreenChangePinPin');
                mTxtSecondScreenChangePinConfirmPin = mElement.find('#txtSecondScreenChangePinConfirmPin');
                mButSecondScreenChangePinSave = mElement.find('#butSecondScreenChangePinSave');
                mButSecondScreenChangePinSave.click(mButSecondScreenChangePinSave_Click);
            }
            
            function mButSecondScreenChangePinSave_Click() {
                var pPin = mTxtSecondScreenChangePinPin.val();
                if (pPin.length < 3) {
                    NewMessageBox('PIN must be longer than 2 characters.');
                    return;
                }
                if (pPin != mTxtSecondScreenChangePinConfirmPin.val()) {
                    NewMessageBox('PIN must be equal to confirm PIN.');
                    return;
                }
                mOut.model.SavePin(pPin);
                NewMessageBox('PIN was successfully saved.');
            }

            mOut.popup = function () {

                if (mElement.parent().length == 0) {
                    $('body').append(mElement);
                    Init();
                    //$('body').trigger('create');
                }

                $.mobile.changePage(mElement, {
                    allowSamePageTransition: true
                });
              
            };
            
            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
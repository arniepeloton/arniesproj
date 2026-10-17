define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/SecondScreenPin.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.SecondScreenPin', null, function () {

            var mOut = {};
            var mElement;
            var mTxtSecondScreenPinPin;
            var mPin;

            mOut.model = new App.model.PeerConnect();
            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.PeerConnect();

            function Init() {
                mPin = mOut.model.GetSettings().pin;
                mTxtSecondScreenPinPin = mElement.find('#txtSecondScreenPinPin');
                mTxtSecondScreenPinPin.bind('textchange', mTxtSecondScreenPinPin_TextChange);
            }
            
            function mTxtSecondScreenPinPin_TextChange() {
                if (mPin == mTxtSecondScreenPinPin.val()) {
                    mOut.success(); // success must be declared by the caller
                }
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
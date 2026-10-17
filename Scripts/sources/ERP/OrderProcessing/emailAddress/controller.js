define([
    // '/Scripts/sources/ERP/OrderProcessing/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/emailAddress/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.OrderProcessing.emailAddress.controller = ERP.OrderProcessing.emailAddress.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.OrderProcessing.emailAddress.controller', null, function () {

        var mOut = {};
        var mElement;
        var mOnOk;
        var mOnCancel;
        var mData;
        var mDontAutoClose;
        var mProductID;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        mElement = $(mHtml);
        mElement.attr('id', 'popPriceUpdateRequest' + NewID());
        $('body').append(mElement);

        mOut.element = mElement;
        

        mOut.popup = function(vParams) {
            
            mData = [];
            //mTransID = vParams.ID;  // delete this line 2022_03_14__08_31_45_9217
            mOnOk = vParams.OnOk;
            mOnCancel = vParams.OnCancel;
            mDontAutoClose = vParams.DontAutoClose;

            mElement.find('[name=butOk]').show();
            mElement.find('[name=lblMessage]').text('Please enter the recipients\'s email addresses?');
            mProductID = vParams.ID;
            mElement.find('[name=lblResult]').hide();
            mElement.find('[name=lblMessage]').show();
            mElement.find('[name=txtReason]').val('');

            function showPopup() {
                setTimeout(function() {
                    try {
                        mElement.modal('show');
                        //mElement.show().popup('open');
                    } catch (ex) {
                        // just ignore the error
                        showPopup();
                    }
                }, 500);
            }
            showPopup();
        };        

        mElement.find('[name=butOk]').click(function() {
            var pTxt = mElement.find('[name=txtData]').val();
            pTxt = pTxt.trim();
            if (pTxt == '') {
                mElement.find('[name=lblMessage]').text('Please enter at least 1 email address?');
                return;
            }
            mElement.modal('hide');
            mOnOk(pTxt);
        });

        mElement.find('[name=butCancel]').click(function(e) {
            e.preventDefault();
            mElement.modal('hide');
            return false;
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
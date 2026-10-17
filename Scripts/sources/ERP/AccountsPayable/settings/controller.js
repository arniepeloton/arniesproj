define([
    '/Scripts/sources/ERP/Invoicing/settings/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.settings.controller = ERP.Invoicing.settings.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.settings.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = new ERP.Invoicing.settings.model();

        var mChkAllowEncodingOfOrdersInInvoice = mElement.find('[name=AllowEncodingOfOrdersInInvoice]');
        var mChkAllowEncodingOfQuantitiesInInvoice = mElement.find('[name=AllowEncodingOfQuantitiesInInvoice]');

        mElement.find('[name=butSave]').click(function() {
            mOut.Model.Save({
                AllowEncodingOfOrdersInInvoice: mChkAllowEncodingOfOrdersInInvoice.prop('checked'),
                AllowEncodingOfQuantitiesInInvoice: mChkAllowEncodingOfQuantitiesInInvoice.prop('checked'),
                Success: function() {
                    NewMessageBox('Settings were successfully saved.');
                }
            });
        });

        mOut.Initialize = function() {
            $('#tabInvoicingSettings').html('').append(mElement);
            mOut.Load();
        };

        mOut.Load = function() {
            mOut.Model.Load({
                callback: function(vData) {
                    mChkAllowEncodingOfOrdersInInvoice.prop('checked', vData.AllowEncodingOfOrdersInInvoice);
                    mChkAllowEncodingOfQuantitiesInInvoice.prop('checked', vData.AllowEncodingOfQuantitiesInInvoice);
                }
            });
        };

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
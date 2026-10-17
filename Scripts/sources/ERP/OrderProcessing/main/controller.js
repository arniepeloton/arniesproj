define([
    'promise!/Scripts/sources/ERP/OrderProcessing/work/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/OrderProcessing/backorders/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/Invoicing/dashboard/controller.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                LoadDisbursement();
            }
        });

        function LoadDisbursement() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/OrderProcessing/main/disbursement.html',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mDisbursementHtml = vData;
                    ERP.OrderProcessing.main.controller = ERP.OrderProcessing.main.controller(); // singleton
                    mDeferred.resolve();
                }
            });
        }
    })();

    CreateStringObject('ERP.OrderProcessing.main.controller', null, function () {

        var mOut = {};
        var mElement;
        var mAllowedInInvoiceSettings;
        var mBackorders = new ERP.OrderProcessing.backorders.controller();
        var mInvoicingBackorders = new ERP.OrderProcessing.backorders.controller();
        var mQuoteBackorders = new ERP.OrderProcessing.backorders.controller();
        var mInvoicingDashboard;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpRevenue').append(mElement);
        $('#divErpDisbursement').append($(mDisbursementHtml));
        $('#tabOPBackorders').append(mBackorders.element);
        $('#tabInvoicingBackorders').append(mInvoicingBackorders.element);
        $('#tabQuoteBackorders').append(mQuoteBackorders.element);
        if (!CBool(GetUrlParameters().IsReadOnly)) {
            mInvoicingDashboard = ERP.Invoicing.dashboard.controller;
            $('#tabInvoicingDashboard').append(mInvoicingDashboard.element);
        }

        $('#divInvoicingTab').find('[custom-tab-content="#tabInvoicingSettings"]').click(function(e) {

            if (mAllowedInInvoiceSettings)
                return;

            var pThis = this;

            e.stopPropagation();
            PromptUserAccess('Invoice Settings (EAS)', {
                OnClickOKSuccess: function () {
                    mAllowedInInvoiceSettings = true;
                    $(pThis).trigger('click');
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        //$('#divOrderProcessingMain').append(ERP.OrderProcessing.work.controller.element);

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
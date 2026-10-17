define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ShowInventoryColumns/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ShowInventoryColumns.controller = ERP.ShowInventoryColumns.controller(); // singleton
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ShowInventoryColumns.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var mOnOk;

        $('body').append(mElement);//.trigger('create');
        // mElement.trigger('create'); // does not work

        mOut.Show = function(vParams) {
            vParams = $.extend({
                
            }, vParams);
            mOnOk = vParams.OnOk;
            //NewMessageBoxClose();
            $('#popShowInventoryColumns-bs').modal('show');
        };

        mElement.find('#butShowInventoryColumnsOk').click(function() {
            NewMessageBox('This might take a few seconds/minutes to process.', 'Quantity Display Options', [
                {
                    Text: 'Proceed to Display Quantities',
                    Click: function() {
                        $('#popShowInventoryColumns-bs').modal('hide');
                        mOnOk({
                            QuantityOnHand: $('#chkShowInventoryColumnsQuantityOnHand').is(':checked'),
                            QuantityCommitted: $('#chkShowInventoryColumnsQuantityCommitted').is(':checked'),
                            QuantityAvailableToCommit: $('#chkShowInventoryColumnsQuantityAvailableToCommit').is(':checked')
                        });
                    }
                },
                {
                    Text: 'Cancel'
                }
            ]);
        });
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
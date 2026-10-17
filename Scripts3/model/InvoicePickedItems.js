define(['common'
    ], function() {

    var mLastID = 0;

    CreateStringObject('App.model.InvoicePickedItems', null, function() {
        
        var mOut = {};
        mLastID++;
        var mID = 'obj' + mLastID;

        mOut.productID = null;
        mOut.baseUnit = null;
        mOut.productDescription = null;
        mOut.jit = 0;
        mOut.bomInstruction = null;

        mOut.getData = function(vParams) {
            /*
            vParams = {
                success: function(vData) {
                },
                error: function(vMessage) {
                }
            };
            */

            AjaxSend({
                url: '/InvoicePickedItems/InvoiceAndSections',
                data: {
                    InvoiceID: vParams.InvoiceID
                },
                success: function (vData) {
                    if (vData.Error != undefined) {
                        vParams.error(vData.Error);
                        return;
                    }
                        
                    vParams.success(vData);
                },
                error: function (vData) {
                    vParams.error(vData.statusText);
                }
            });
        };

        return mOut;
    });

    App.model.InvoicePickedItems = App.model.InvoicePickedItems(); // singleton
    
});
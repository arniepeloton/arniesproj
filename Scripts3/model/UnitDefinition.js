define(['common'
    ], function() {

    CreateStringObject('App.model.UnitDefinition', null, function() {
        
        var mOut = {};

        mOut.organizationID = null;
        mOut.productID = null;
        mOut.productDescription = null;

        // during saving
        mOut.qty = null;
        mOut.unitMeasureID = null;
        mOut.destinationMeasure = null;

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
                url: 'Stock/GetUnitDefinition',
                data: {
                    ProductID: mOut.productID,
                    OrgID: mOut.organizationID
                },
                success: function (vData) {
                    if (vData.Error != undefined) {
                        vParams.error(vData.Error);
                        return;
                    }

                    if (vData.content == undefined) {
                        mOut.qty = null;
                        mOut.destinationMeasure = null;
                    } else {
                        mOut.qty = vData.content.Quantity;
                        mOut.destinationMeasure = vData.content.DestinationMeasure;
                    }
                        
                    vParams.success(vData);
                },
                error: function (vData) {
                    vParams.error(vData.statusText);
                }
            });
        };

        mOut.save = function(vParams) {
            /*
            vParams = {
                success: function() {
                },
                error: function(vMessage) {
                }
            };
            */

            var pVerify = verify();
            if (!pVerify.success) {
                vParams.error(pVerify.error);
                return;
            }

            AjaxSend({
                url: 'Stock/SaveUnitDefinition',
                data: {
                    ProductID: mOut.productID,
                    SourceMeasureID: mOut.unitMeasureID,
                    OrgID: mOut.organizationID,
                    Quantity: mOut.qty,
                    DestinationMeasure: mOut.destinationMeasure
                },
                success: function (vData) {
                    if (vData.Error)
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok')
                        vParams.success(vData);
                    else
                        vParams.error('Unknown error jsBH7RG4935Y!');
                },
                error: function (vError) {
                    vParams.error(vError.statusText);
                }
            });
        };

        function verify() {
            
            if (CStr2(mOut.qty).trim() == '')
                return {
                    error: 'Qty must not be blank.'
                };

            if (!IsNumeric(mOut.qty)) 
                return {
                    error: 'Qty must be a valid number.'
                };

            if (CDec2(mOut.qty) <= 0) 
                return {
                    error: 'Qty must be greater than zero.'
                };

            return {
                success: true
            };

        }

        return mOut;
    });

});
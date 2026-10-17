define(['common'
    ], function() {

    var mLastID = 0;

    CreateStringObject('App.model.PriceChange', null, function() {
        
        var pOut = {};
        mLastID++;
        var mID = 'obj' + mLastID;

        pOut.discountAmount = 0;
        pOut.discountPercent = 0;
        pOut.baseAmount = 0;
        pOut.netAmount = 0;

        pOut.setDiscountAmount = function(vValue) {
            
            pOut.netAmount = pOut.baseAmount - vValue;
            pOut.discountPercent = vValue / pOut.baseAmount * 100;
            pOut.discountAmount = vValue;

        };

        pOut.setDiscountPercent = function(vValue) {
            
            pOut.discountAmount = pOut.baseAmount * vValue / 100;
            pOut.netAmount = pOut.baseAmount - pOut.discountAmount;
            pOut.discountPercent = vValue;

        };

        pOut.setNetAmount = function(vValue) {

            pOut.discountAmount = pOut.baseAmount - vValue;
            if (pOut.baseAmount == 0)
                pOut.discountPercent = 0;
            else
                pOut.discountPercent = pOut.discountAmount / pOut.baseAmount * 100;
            pOut.netAmount = vValue;

        };

        pOut.setBaseAmount = function(vValue, vStableProperty) {
            // PARAMS:
            //  vStableProperty = 1 of 'discountAmount', 'discountPercent', 'netAmount'

            if (['discountAmount',
                'discountPercent',
                'netAmount'
                ].indexOf(vStableProperty) == -1) {

                RaiseError("vStableProperty must be one of the following 'discountAmount', 'discountPercent', 'netAmount'");
            }
            pOut[vStableProperty] = parseFloat(pOut[vStableProperty]);

            if (vStableProperty == 'discountAmount') {
                pOut.discountPercent = pOut.discountAmount / vValue * 100;
                pOut.netAmount = vValue - pOut.discountAmount;
                
            } else if (vStableProperty == 'discountPercent') {
                pOut.discountAmount = vValue * pOut.discountPercent / 100;
                pOut.netAmount = vValue - pOut.discountAmount;
            } else if (vStableProperty == 'netAmount') {
                pOut.discountAmount = vValue - pOut.netAmount;
                pOut.discountPercent = pOut.discountAmount / vValue * 100;
            }
            pOut.baseAmount = vValue;

        };

        pOut.getID = function() {

            return mID;

        };

        pOut.validate = function() {

            if (IsNumeric(pOut.netAmount)) {
                if (CDec2(pOut.netAmount) < 0)
                    return {
                        error: 'Requested Price may not be negative.'
                    };
            } else
                return {
                    error: 'Requested Price must be a valid amount.'
                };

            if ((CDec2(pOut.baseAmount) < CDec2(pOut.netAmount))
                && (Business.SystemDefaults.DontAllowGreaterFreeTypePrices())) {

                return {
                    error: 'Requested Price must not be greater than the base price.'
                };
            }

            return {
                result: 'ok'
            };
            
        }; // END pOut.validate = function() {

        return pOut;
    });

});
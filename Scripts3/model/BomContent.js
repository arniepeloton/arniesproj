define(['common'
    ], function() {

    CreateStringObject('App.model.BomContent', null, function() {
        
        var mOut = {};

        mOut.organizationID = null;
        mOut.parentProductID = null;
        mOut.childProductID = null;
        mOut.productDescription = null;
        mOut.childProductBaseUnitMeasureID = null;

        // during saving
        mOut.qty = null;
        mOut.unitMeasureID = null;
        mOut.unitMeasure = null;

        mOut.getBomPageData = function(vParams) {
            /*
            vParams = {
                success: function(vData) {
                },
                error: function(vMessage) {
                }
            };
            */

            AjaxSend({
                url: 'Stock/GetBomPageData',
                data: {
                    ParentProductID: mOut.parentProductID,
                    ChildProductID: mOut.childProductID,
                    OrgID: mOut.organizationID
                },
                success: function (vData) {
                    if (vData.Error != undefined) {
                        vParams.error(vData.Error);
                        return;
                    }

                    mOut.childProductBaseUnitMeasureID = vData.child.BaseUnitMeasureID;
                    if (vData.contents == undefined) {
                        mOut.qty = null;
                        mOut.unitMeasureID = null;
                        mOut.unitMeasure = null;
                    } else {
                        mOut.qty = vData.contents.Quantity;

                        // only 1 of these 2 will be used.
                        // prioritize UnitMeasureID
                        mOut.unitMeasureID = vData.contents.UnitMeasureID;
                        mOut.unitMeasure = vData.contents.UnitMeasure; // used only for standard measure
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
                url: 'Stock/SaveBomPageData',
                data: {
                    ParentProductID: mOut.parentProductID,
                    ChildProductID: mOut.childProductID,
                    OrgID: mOut.organizationID,
                    Qty: mOut.qty,
                    UnitMeasureID: mOut.unitMeasureID,
                    UnitMeasure: mOut.unitMeasure
                },
                success: function (vData) {
                    if (vData.Error)
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok')
                        vParams.success(vData);
                    else
                        vParams.error('Unknown error jsH2QTTRCJVC!');
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

            if (CDec2(mOut.qty) < 0) 
                return {
                    error: 'Qty must be equal or greater than zero.'
                };

            return {
                success: true
            };

        }

        return mOut;
    });

});
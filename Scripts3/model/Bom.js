define(['common'
    ], function() {

    var mLastID = 0;

    CreateStringObject('App.model.Bom', null, function() {
        
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
                url: 'Stock/GetBomData',
                data: {
                    ProductID: mOut.productID
                },
                success: function (vData) {
                    if (vData.Error != undefined) {
                        vParams.error(vData.Error);
                        return;
                    }
                        
                    mOut.baseUnit = vData.parent.BaseUnit;
                    mOut.productDescription = vData.parent.ProductName;
                    mOut.jit = vData.parent.Jit;
                    mOut.bomInstruction = vData.parent.BomInstruction;
                    vParams.success(vData);
                },
                error: function (vData) {
                    vParams.error(vData.statusText);
                }
            });
        };

        mOut.saveInstruction = function (vParams) {
            /*
            vParams = {
                bomInstruction: 'string',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
            };
            */

            AjaxSend({
                url: 'Stock/SaveBomInstruction',
                data: {
                    ProductID: mOut.productID,
                    BomInstruction: vParams.bomInstruction,
                    Jit: vParams.jit
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

        mOut.deleteContent = function(vParams) {
            /*
            vParams = {
                childProductID: 'string',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
            };
            */

            AjaxSend({
                url: 'Stock/DeleteBomContent',
                data: {
                    ParentProductID: mOut.productID,
                    ChildProductID: vParams.childProductID
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

});
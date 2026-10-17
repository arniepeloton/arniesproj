define([
], function () {

    CreateStringObject('ERP.Receiving.work.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ID: null,
            ReceiveInOrgID: null,
            ReceiveInOutlet: null,
            ReceiveInBinID: null,
            SupplierID: null,
            ReceiveFromFowstID: null,
            ReceiveFromBinID: null,
            ProductVendorCost: null
        };
        
        mOut.Verify = function(vParams) {
            if (CStr2(mOut.Data.SupplierID).trim() == '')
                return vParams.Fail('Vendor must not be blank.');
            if (!IsGuid(mOut.Data.SupplierID))
                return vParams.Fail('SupplierID must be a valid GUID.');

            if (CStr2(mOut.Data.ReceivingPlaceID).trim() == '')
                return vParams.Fail('Receive In Fowst/Outlet must not be blank.');
            if (!IsGuid(mOut.Data.ReceivingPlaceID))
                return vParams.Fail('ReceivingPlaceID must be a valid GUID.');

            if (mOut.Data.TransType == 'StockTransfer') {
                mOut.Data.IsReceiving = 0;
            } else {
                mOut.Data.IsReceiving = 1;
                if (!IsDate(mOut.Data.InvoiceDate))
                    return vParams.Fail('Invoice Date must be a valid date.');
            } 
                
            return vParams.Success();
        }; // END Verify

        mOut.Save = function(vParams) {
            var pData = {
                SaveOnlyProduct: vParams.SaveOnlyProduct,
                Receiving: mOut.Data,
                Products: mOut.Data.Products,
                SaveSomeFields: vParams.SaveSomeFields
            };
            delete pData.Receiving.Products;
            AjaxSend({
                url: '/Receiving/Save',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else if (vData.Result == 'ok')
                        vParams.Success(vData);
                }
            });
        }; // END Save

        mOut.GetData = function(vParams) {
            var pData;
            if (vParams.Data)
                pData = vParams.Data;
            else
                pData = {
                    ID: vParams.ID
                };
            AjaxSend({
                url: '/Receiving/GetData',
                data: pData,
                success: function(vData) {
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.RemoveProduct = function(vParams) {
            AjaxSend({
                url: '/Receiving/DeleteItem',
                data: {
                    ReceivingID: mOut.Data.ID,
                    ID: vParams.ID,
                    ProductID: vParams.ProductID,
                    UnitMeasureID: vParams.UnitMeasureID
                },
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else if (vData.Result == 'ok')
                        vParams.Success();
                }
            });
        };

        mOut.AddAddress = function (vParams) {
            AjaxSend({
                url: '/ERP/AddAddress',
                data: {
                    CustomerID: mOut.Data.CustomerID,
                    Address: vParams.Address
                },
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    }
                }
            });
        };
       

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});
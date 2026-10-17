define([
], function () {

    CreateStringObject('ERP.OrderProcessing.work.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ID: null,
            CustomerID: null,
            FowstID: null,
            BillingAddressID: null,
            DeliveryAddressID: null,
            SalesPartnerOrgID: null,
            SalesRepPeopleID: null,
            DateNeeded: null,
            Remarks: null,
            InternalRemarks: null,
            CopyProductsFromInvoiceID: null,
            Products: [],
            TransType: null, // based on eTransType (C:\dev\debug\Mobiz\WebErp\vb\Business\TransInvoice.vb)
                // Quote
                // SalesOrder
                // Invoice
                // PurchaseOrder
            PurchaseOrderNo: null,
            PurchaseOrderDate: null,
            QuotationNo: null,
            QuotationID: null,
            CanvassID: null,
            TransDate: null
        }
        
        mOut.Verify = function(vParams) {
            if (CStr2(mOut.Data.CustomerID).trim() == '') {
                if (mOut.Data.TransType == 'PurchaseOrder')
                    return vParams.Fail('Mobiz cannot save your current transaction because: Vendor must not be blank.');
                else
                    return vParams.Fail('Mobiz cannot save your current transaction because: Customer must not be blank.');
            }
            if (!IsGuid(mOut.Data.CustomerID))
                return vParams.Fail('Mobiz cannot save your current transaction because: CustomerID must be a valid GUID.');
            if (mOut.Data.TransType == 'PurchaseOrder')
                if (CStr2(mOut.Data.DateNeeded).trim() == '')
                    return vParams.Fail('Mobiz cannot save your current transaction because: Date Needed is required.');
                else if (!IsDate(mOut.Data.DateNeeded))
                    return vParams.Fail('Mobiz cannot save your current transaction because: Date Needed must be a valid date.');

            if (CStr2(mOut.Data.BillingAddressID).trim() == '') 
                return vParams.Fail('Mobiz cannot save your current transaction because: Billing Address must not be blank.');

            if (CStr2(mOut.Data.DeliveryAddressID).trim() == '') 
                return vParams.Fail('Mobiz cannot save your current transaction because: Delivery Address must not be blank.');
            
            return vParams.Success();
        }; // END Verify

        mOut.Save = function(vParams) {
            var pData = $.extend(mOut.Data, vParams.Data);
            AjaxSend({
                url: '/ERP/SaveSalesOrder',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else // if (vData.Result == 'ok') // commented the condition 2019_01_31__16_18_14_2439
                        vParams.Success(vData);
                }
            });
        }; // END Save

        mOut.GetCart = function(vParams) {
            AjaxSend({
                url: '/SalesMenu/PosCart',
                data: {
                    OrderID: vParams.ID,
                    DontCheckCartValidity: 1,
                    IncludeZeroQty: vParams.IncludeZeroQty,
                    IncludeSuppliers: vParams.IncludeSuppliers,
                    AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                    IncludeChildOrderItems: vParams.IncludeChildOrderItems,
                    ShowBackOrderItems: vParams.ShowBackOrderItems || false,
                    TransType: mOut.Data.TransType
                },
                success: function(vData) {
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.GetCanvassitems = function(vParams) {
            AjaxSend({
                url: '/Browser/GetCanvassItems',
                data: {
                    CanvassID: vParams.CanvassID,
                    VendorID: vParams.VendorID
                },
                success: function(vData) {
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.RemoveProduct = function(vParams) {
            AjaxSend({
                url: '/SalesMenu/DeleteStockPos',
                data: {
                    OrderID: mOut.Data.ID,
                    ProductID: vParams.ProductID,
                    UnitMeasure: vParams.UnitMeasure,
                    ItemID: vParams.ItemID,
                    AddToItemRequisition: vParams.AddToItemRequisition,
                    PosMode: vParams.PosMode
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
                    //CustomerID: mOut.Data.CustomerID,
                    CustomerID: vParams.CustomerID,
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

        mOut.AddSalesRep = function (vParams) {
            AjaxSend({
                url: '/ERP/AddSalesRep',
                data: {
                    Name: vParams.Name
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
        
        mOut.SaveContactPerson = function (vParams) {
            AjaxSend({
                url: '/ERP/SaveContactPerson',
                data: {
                    LastName: vParams.LastName,
                    ClientOrgID: vParams.OrgID
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

        mOut.GetInvoiceTypes = function(vParams) {
            AjaxSend({
                url: '/Browser/LibraryDocumentControl',
                data: {
                    ModuleSource: 'Revenue > Invoicing'
                },
                success: function(vData) {  
                    vParams.success(vData);
                }
            });
        };

        mOut.GetCommonLineItemCount = function(vParams) {
            AjaxSend({
                url: '/SalesMenu/GetCommonLineItemCount',
                data: {
                    InvoiceID1: vParams.InvoiceID1,
                    InvoiceID2: vParams.InvoiceID2
                },
                success: function(vData) {  
                    vParams.success(vData);
                }
            });
        };

        mOut.SaveSomeFields = function(vParams) {
            AjaxSend({
                url: '/SalesMenu/SaveSomeFields',
                data: {
                    ID: mOut.Data.ID,
                    RefInvoiceNum: mOut.Data.RefInvoiceNum,
                    SalesRepText: mOut.Data.SalesRepText,
                    SalesInvoiceRef: mOut.Data.SalesInvoiceRef,
                    OfficialReceiptRef: mOut.Data.OfficialReceiptRef
                },
                success: function(vData) {  
                    vParams.success(vData);
                }
            });
        };

        mOut.VerifyStockTransfer = function(vParams) {
            if (!IsGuid(mOut.Data.TransferFromOrgID))
                return vParams.Fail('Mobiz cannot save your current transaction because Transfer From Org is required');
            if (!IsGuid(mOut.Data.TransferToOrgID))
                return vParams.Fail('Mobiz cannot save your current transaction because Transfer To Org is required');
            if (!IsGuid(mOut.Data.TransferFromOutletID))
                return vParams.Fail('Mobiz cannot save your current transaction because Transfer From Outlet is required');
            if (!IsGuid(mOut.Data.TransferToOutletID))
                return vParams.Fail('Mobiz cannot save your current transaction because Transfer To Outlet is required');
            return vParams.Success();
        }; // END Verify

        mOut.SaveStockTransfer = function(vParams) {
            var pData = $.extend(mOut.Data, vParams.Data);
            AjaxSend({
                url: '/ERP/SaveStockTransfer',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else // if (vData.Result == 'ok') // commented the condition 2019_01_31__16_18_14_2439
                        vParams.Success(vData);
                }
            });
        }; // END Save

        mOut.SaveOptions = function(vParams) {
            vParams = $.extend({
                ID: null,
                AccountsPayableFollowCost: null,
                Success: function() {},
                Fail: function() {}
            }, vParams);

            var pData = {
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost
            };

            AjaxSend({
                url: '/ERP/SaveOptions',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    }
                }
            });
        };

        mOut.LoadOptions = function(vParams) {
            vParams = $.extend({
                Success: function() {},
                Fail: function() {}
            }, vParams);

            AjaxSend({
                url: '/ERP/LoadOptions',
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    }
                }
            });
        };

        mOut.HandleWebPosDeviceFowst = function(vParams) {
            vParams = $.extend({
                Success: function() {},
                Fail: function() {}
            }, vParams);

            AjaxSend({
                url: '/ERP/HandleWebPosDeviceFowst',
                success: function(vData) {
                    if (vData.Error) {
                        if (vData.Error == 'UUID not in device library') {
                            NewMessageBox(localStorage.getItem('UUID') + ' is not registered. Do you want to send a device approval request?', 'Device Approval Request', [
                                {
                                    Text: 'Yes',
                                    Click: function() {
                                        RequestDeviceApproval();
                                    },
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        } else
                            vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    }
                }
            });
        };

        function RequestDeviceApproval() {
            var pData = {
                PhoneName: 'desktop via Mobiz Controller',
                PhoneModel: 'desktop via Mobiz Controller',
                IMEI: '-',
                //UUID: $.trim(Device.Serial()),
                OS: 'desktop via Mobiz Controller'
            };

            AjaxSend({
                url: "Device/RequestDeviceApproval",
                data: pData,
                success: function (vSaveData) {
                    NewMessageBox(vSaveData.Result,
                        "Device Approval Request",
                        [
                            { Text: 'Ok',
                                Click: function () {
                                    //window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                }
                            }
                        ]
                    );
                }
            });
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});
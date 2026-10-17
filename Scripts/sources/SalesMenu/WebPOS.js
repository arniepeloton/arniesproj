require(['common'], function () {
    require([
    //'promise!/Scripts/sources/ERP/Invoicing/settings/controller.js?v=' + assetversion,
    '/Scripts/sources/ERP/OrderProcessing/work/model.js',
    'promise!/Scripts/sources/ERP/OrderProcessing/product/controller.js?v=' + assetversion,
    //'promise!/Scripts/sources/ERP/OrderProcessing/relatedOrders/controller.js?v=' + assetversion,
    //'promise!/Scripts/sources/ERP/OrderProcessing/serialNumbers/controller.js',
    'promise!/Scripts/sources/ERP/QuickPim/controller.js',
    '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js?v=' + assetversion,
    'bootstrap-tooltip',
    'promise!/Scripts/sources/SetupChecklist/NewPromptUser.js'
    //'promise!/Scripts/sources/ERP/InventoryBalance/controller.js?v=' + assetversion,
    //'promise!/Scripts/sources/ERP/ChildSos/controller.js',
    //'promise!/Scripts/sources/ERP/ContactPerson/controller.js',
    //'promise!/Scripts/sources/PIMUpload/PIMUpload.js'
    ], function () {
        $(document).ready(function () {
            var mDeferred = $.Deferred();
            var mQuoteHtml;
            var mSalesOrderHtml;
            var mInvoiceHtml;
            var mStockReceivingHtml;
            var mStockTransferHtml;
            var mPurchaseOrderRequestHtml;
            var mPurchaseOrderHtml;
            var mRemoteServer;
            var mFowstGroupPriceName;

            (function staticInit() {

                AjaxSend({
                    ExactUrl: '/Scripts/sources/ERP/Quote/work/view.html',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        mQuoteHtml = vData;
                        //ERP.OrderProcessing.work.controller = ERP.OrderProcessing.work.controller(); // singleton
                        LoadSalesOrderHtml();
                    }
                });

                function LoadSalesOrderHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/OrderProcessing/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mSalesOrderHtml = vData;
                            //ERP.OrderProcessing.work.controller = ERP.OrderProcessing.work.controller(); // singleton
                            LoadStockReceivingHtml();
                        }
                    });
                }

                function LoadStockReceivingHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/Receiving/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mStockReceivingHtml = vData;
                            //ERP.OrderProcessing.work.controller = ERP.OrderProcessing.work.controller(); // singleton
                            LoadStockTransferHtml();
                        }
                    });
                }

                function LoadStockTransferHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/StockTransfer/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mStockTransferHtml = vData;
                            LoadPurchaseOrderRequestHtml();
                        }
                    });
                }

                function LoadPurchaseOrderRequestHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/PurchaseOrderRequest/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mPurchaseOrderRequestHtml = vData;
                            LoadPurchaseOrderHtml();
                        }
                    });
                }

                function LoadPurchaseOrderHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/PurchaseOrder/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mPurchaseOrderHtml = vData;
                            LoadInvoiceHtml();
                        }
                    });
                }

                function LoadInvoiceHtml() {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/Invoicing/work/view.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mInvoiceHtml = vData;
                            mDeferred.resolve();
                        }
                    });
                }

            })();

            //CreateStringObject('ERP.OrderProcessing.work.controller', null, function (vParams) {

            $('body').tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' }).on('click mousedown mouseup', '[data-toggle="tooltip"], [title]:not([data-toggle="popover"])', function () {
                $('[data-toggle="tooltip"], [title]:not([data-toggle="popover"])').tooltip('destroy');
            });;

            window.WebPOS = {};
            var mOut = {};
            var mElement = $('body');
            var mCustomerName;
            var mSavedAsFinal;
            var pProductPopup = new ERP.OrderProcessing.product.controller();
            //var mRelatedOrdersPopup = new ERP.OrderProcessing.relatedOrders.controller();
            //var mSerialNumbersPopup = new ERP.OrderProcessing.serialNumbers.controller();
            var mMaxPaymentTerm = 0;
            var mTargetOrgType; // customer or vendor
            var mInventoryBalance; // = new ERP.InventoryBalance.controller();
            //var mChildSos = new ERP.ChildSos.controller();
            //var mContactPerson = new ERP.ContactPerson.controller();
            var mInvoiceTypes;
            var mFromSalesOrder;
            var mHasCnCUnapprovedReason;
            var mFinalizedForApproval;
            var mUnapprovedProfitability;
            var mInvoiceTotal;
            var mZReadDate;
            var mServerTime;
            var mLocalTime;  // current local time while obtaining mServerTime 
            var mEnforceZRead;

            var vParams = $.extend({
                TransType: null,
                Callback: function () { },
                ShowProductSuppliersColumn: false
            }, vParams);
            mTargetOrgType = 'customer';
            mOut.TransType = 'Invoice'; //vParams.TransType;
            mOut.IsPos = true;
            mOut.isSaveReceipt = true;
            mOut.ShowProductSuppliersColumn = vParams.ShowProductSuppliersColumn;
            //if (mOut.TransType == 'Quote') {
            //    mElement = $(mQuoteHtml);
            //    $('#divQuotationMain').append(mElement);
            //} else if (mOut.TransType == 'SalesOrder') {
            //    mElement = $(mSalesOrderHtml);
            //    $('#divOrderProcessingMain').append(mElement);
            //} else if (mOut.TransType == 'Invoice') {
            //    mElement = $(mInvoiceHtml);

            //    //if ($('#divInvoicingMain > #tabInvoicingWork').length == 0) {
            //    //    $('#divInvoicingMain').append(mElement);
            //    //} else {
            //    //    $('#tabInvoicingWork').append($('#popupInvoiceContent'));
            //    //}
            //} else if (mOut.TransType == 'PurchaseOrderRequest') {
            //    mElement = $(mPurchaseOrderRequestHtml);

            //    if ($('#divPurchaseOrderRequestMain > #tabPurchaseOrderRequestWork').length == 0) {
            //        $('#divPurchaseOrderRequestMain').append(mElement);
            //    } else {
            //        $('#tabPurchaseOrderRequestWork').append($('#popupPurchaseOrderRequestContent'));
            //    }
            //} else if (mOut.TransType == 'PurchaseOrder') {
            //    //mElement = $('#divPurchasingMain');

            //    mElement = $(mPurchaseOrderHtml);

            //    $('#divPurchasingMain').append(mElement);

            //    mTargetOrgType = 'vendor';
            //    proj.GetOpenPurchaseOrders();
            //    // no need to append
            //} else if (mOut.TransType == 'AccountsPayable') {
            //    mElement = $(mPurchaseOrderHtml);
            //    $('#divPurchasingMain').append(mElement);
            //    mTargetOrgType = 'vendor';
            //    // no need to append
            //} else if (mOut.TransType == 'StockTransfer') {
            //    mElement = $(mStockTransferHtml);
            //    $('#divStockTransferMain').append(mElement);
            //} else {
            //    alert('Unknown TransType, ' + vParams.TransType + '. ref: js7VUG78QX3V!');
            //    debugger;
            //}

            mOut.element = mElement;

            mOut.Model = new ERP.OrderProcessing.work.model();
            var mButOptions = mElement.find('[name=butOptions]');
            if (mOut.TransType == 'AccountsPayable') {
                mButOptions.show();
            }

            mElement.find('[name="butHotButtons"]').click(function () {
                require(['promise!/Scripts/sources/SalesMenu/HotButton/controller.js?v=' + assetversion], function () {
                    ERP.WebPOS.hb.controller.popup({
                        WebPos: mOut,
                        success: function (vData) {
                            function AddProductHB() {
                                var pData = [];
                                for (var p = 0; p <= vData.length - 1; p++) {
                                    var vRows = vData[p];
                                    pData.push({
                                        ID: vRows.ProductID,
                                        Qty: vRows.OrderQty,
                                        EditPrice: vRows.RetailPrice,
                                        UnitMeasure: vRows.UnitMeasure,
                                        UnitQty: vRows.UnitQty
                                    });
                                }
                                PreSelectSave(pData);
                            }

                            if (CStr2(mOut.Model.Data.ID) == '') {
                                SaveAsDraft({
                                    NoPrompt: true,
                                    Success: AddProductHB
                                });
                            } else
                                AddProductHB();
                        }
                    });
                })
            })

            mElement.find('[name="btnWebPosBack"]').click(function () {
                if (sessionStorage.system == 'eas') {
                    location.href = '/erp?OrgID=' + sessionStorage.OrgID;
                } else {
                    location.href = '/People/PeopleMenu';
                }                
            })

            mElement.find('[name=divTransDate]').click(function () {
                SingleCalendar({
                    ShowTime: true,
                    Click: function (e) {
                        mOut.Model.Data.TransDate = e;
                        DisplayTransDate(e);
                    }
                });
            });

            function DisplayTransDate(vDate) {
                if (vDate == null)
                    vDate = new Date();

                var pLabel;
                switch (mOut.Model.Data.TransType) {
                    case 'SalesOrder':
                        pLabel = 'Sales Order Date: ';
                        break;

                    case 'Invoice':
                        pLabel = 'Invoice Date: ';
                        break;

                    default:
                        pLabel = 'Transaction Date: ';
                }

                mElement.find('[name=divTransDate]').text(pLabel + FormatDate(new Date(vDate), { Format: 'MMMM d, yyyy h:mm a' }));
            }

            var mTxtCustomerFowst = mElement.find('[name=txtCustomerFowst]');
            mTxtCustomerFowst.on('keypress', function (e) {
                if (e.key == 'Enter')
                    SearchCustomerFowst();
            }); // END mTxtBillingAddress.on('keypress', function(e) {
            mElement.find('[name=butCustomerFowst]').click(SearchCustomerFowst);
            function SearchCustomerFowst() {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (!IsGuid(mOut.Model.Data.CustomerID)) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }
                BrowsePopup('LibraryFowst', 'Customer Outlet',
                    [
                        {
                            // Search: mTxtCanvass.val(),
                            AutoSearch: true,
                            StyleFormat: 'table',
                            DisplayText: [
                                {
                                    Label: 'Outlet',
                                    Column: 'FowstName'
                                }
                            ],
                            Click: function (e) {
                                mOut.Model.Data.CustomerFowstID = e.ID;
                                mTxtCustomerFowst.val(e.FowstName);
                            }
                        }
                    ], {
                        IsThirdPartyOrganization: 1,
                        ThirdPartyOrganizationID: mOut.Model.Data.CustomerID
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            var mTxtCustomerBin = mElement.find('[name=txtCustomerBin]');
            mElement.find('[name=butCustomerBin]').click(SearchCustomerBin);
            function SearchCustomerBin() {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (!IsGuid(mOut.Model.Data.CustomerFowstID)) {
                    NewMessageBox('Please select a customer outlet first.');
                    return;
                }
                BrowsePopup('LibraryFowstBin', 'Customer Department',
                    [
                        {
                            // Search: mTxtCanvass.val(),
                            AutoSearch: true,
                            StyleFormat: 'table',
                            DisplayText: [
                                {
                                    Label: 'Department',
                                    Column: 'Name'
                                }
                            ],
                            Click: function (e) {
                                mOut.Model.Data.CustomerBinID = e.ID;
                                mTxtCustomerBin.val(e.Name);
                            }
                        }
                    ], {
                        FowstID: mOut.Model.Data.CustomerFowstID
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            var mDivDraftOrFinalized = mElement.find('[name=divDraftOrFinalized]');
            var mTxtInvoiceNumber = mElement.find('[name=txtInvoiceNumber]');
            var mTxtRefInvoiceNumber = mElement.find('[name=txtRefInvoiceNumber]');
            var mTxtSalesInvoiceRef = mElement.find('[name=txtSalesInvoiceRef]');
            var mTxtOfficialReceiptRef = mElement.find('[name=txtOfficialReceiptRef]');
            var mTxtPaymentTerms = mElement.find('[name=txtPaymentTerms]');
            var mTxtPaymentTermsText = mElement.find('[name=txtPaymentTermsText]');
            var mTxtCustomer = mElement.find('[name=txtCustomer]');
            mTxtCustomer.on('keypress', SearchCustomer); // END mTxtCustomer.on('keypress', function(e) {
            mTxtCustomer.click(function () {
                SearchCustomer({
                    key: 'Enter'
                });
            });
            mElement.find('[name=butCustomer]').click(function () {
                SearchCustomer({
                    key: 'Enter'
                });
            });

            proj.BeforeChangeFowst = function (vParams) {
                if (mSavedAsFinal || $.isEmptyObject(mOut.Model.Data.CustomerID)) {
                    vParams.Success();
                    return;
                }

                var pMsg = 'Changing FOWST will save the current transaction and create a new one. Do you still want to change FOWST?';
                NewMessageBox(pMsg, '', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            PreSave({
                                NoPrompt: true,
                                Success: function () {
                                    NewTrans();
                                    vParams.Success();
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function () {
                            // just do nothing
                        }
                    },
                    {
                        Text: 'Discard current cart',
                        Click: function () {
                            NewTrans();
                            vParams.Success();
                        }
                    }
                ]); // END NewMessageBox
            };

            function SearchCustomer(e) {
                if (!HandleEditingSavedAsFinal())
                    return;

                if (!HandleAllowEncodingOfQuantitiesInInvoice())
                    return;

                if (!$.isEmptyObject(mOut.Model.Data.CustomerID)) {
                    var pMsg = 'Changing ' + mTargetOrgType + '  will save the current transaction and create a new one. Do you still want to change ' + mTargetOrgType + '?';
                    NewMessageBox(pMsg, '', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                PreSave({
                                    NoPrompt: true,
                                    Success: function () {
                                        NewTrans();
                                        DoBrowsePopup();
                                    }
                                });
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                mTxtCustomer.val(mCustomerName);
                            }
                        },
                        {
                            Text: 'Discard current cart',
                            Click: function () {
                                NewTrans();
                                DoBrowsePopup();
                            }
                        }
                    ]);
                    return;
                }
                if (e.key == 'Enter') {
                    DoBrowsePopup();
                } // END if (e.key == 'Enter') {

                function DoBrowsePopup() {
                    var pTarget;
                    var pOptions;

                    if (mOut.TransType == 'PurchaseOrder') {
                        pTarget = 'Vendor';
                        pOptions = {
                            FilterSupplier: 1
                        };
                    } else {
                        pTarget = 'Customer';
                        pOptions = {
                            FilterCustomers: 1
                        };
                    }
                    proj.RemoteServer = mRemoteServer;
                    BrowsePopup('LibraryOrganization', pTarget,
                        [
                            {
                                // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                                Message: 'NOTE: Please search the customer\'s name first before you can add new customer.',
                                AllowToAddNewCustomer: true,
                                DisplayText: [{ Column: 'CompanyName' }],
                                Click: function (e) {
                                    mCustomerName = e.CompanyName;
                                    mTxtCustomer.val(e.CompanyName);
                                    $('#lblCustomerName').text(mCustomerName);
                                    sessionStorage.CustomerOrganizationID = e.ID;
                                    mOut.Model.Data.CustomerID = e.ID;
                                    mMaxPaymentTerm = CDec2(e.CreditTerms);
                                    mTxtPaymentTerms.val(CDec2(e.CreditTerms));
                                    mTxtSalesRep.val(e.SalesRepName);
                                    LoadAccountsDetails();
                                    LoadSingleAddress();
                                }
                            }
                        ], pOptions
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
                }
            } // END function SearchCustomer() {

            function LoadSingleAddress() {
                if (mOut.TransType == 'PurchaseOrder')
                    return;

                AjaxSend({
                    url: '/Browser/LibraryFowst',
                    data: {
                        OrganizationID: mOut.Model.Data.CustomerID,
                        HideNoAddress: 1
                    },
                    success: function (vData) {
                        var pRows = GetStringObject('Results.Rows', vData);
                        if (!$.isArray(pRows))
                            return;

                        if (pRows.length != 1)
                            return;

                        var pDR = pRows[0];
                        mTxtDeliveryAddress.val(pDR.Address);
                        mOut.Model.Data.DeliveryAddressID = pDR.ID;

                        mTxtBillingAddress.val(pDR.Address);
                        mOut.Model.Data.BillingAddressID = pDR.ID;
                    }
                });
            }

            mElement.find('[name=butCustomerShowAccount]').click(ShowAccountDetails);
            function ShowAccountDetails() {
                if (!IsGuid(mOut.Model.Data.CustomerID)) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }

                require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function () {
                    var pMode;

                    if (mTargetOrgType == 'customer')
                        pMode = 'AR';
                    else
                        pMode = 'AP';
                    ERP.AccountDetails.controller.Mode = pMode;
                    ERP.AccountDetails.controller.CustomerID = mOut.Model.Data.CustomerID;
                    ERP.AccountDetails.controller.CustomerName = mTxtCustomer.val();
                    $('#popupAccountDetails').show();
                    ERP.AccountDetails.controller.GetCustomerCreditDetails(mOut.Model.Data.CustomerID, 'has payable');
                });
            } // END ShowAccountDetails

            var mTxtCanvass = mElement.find('[name=txtCanvass]');
            mTxtCanvass.on('keypress', function (e) {
                if (e.key == 'Enter')
                    SearchCanvass();
            }); // END mTxtBillingAddress.on('keypress', function(e) {
            mElement.find('[name=butPoCanvass]').click(function () {
                SearchCanvass();
            });
            function SearchCanvass(vDontVerifyCurrentRecord) {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (!$.isEmptyObject(mOut.Model.Data.CustomerID))
                    if (!vDontVerifyCurrentRecord) {
                        var pMsg = 'Selecting a price canvass will create a new transaction. Any unsaved changes to the current record will be discarded. Do still want to select a price canvass?';
                        NewMessageBox(pMsg, '', [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    SearchCanvass(true);
                                }
                            },
                            {
                                Text: 'No',
                                Click: function () {
                                }
                            }
                        ]);
                        return;
                    }
                BrowsePopup('PurchaseOrderablePriceCanvass', 'Price Canvass',
                    [
                        {
                            Search: mTxtCanvass.val(),
                            StyleFormat: 'table',
                            DisplayText: [
                                {
                                    Label: 'Canvass #',
                                    Column: 'CanvassNumber'
                                },
                                {
                                    Label: 'Vendor',
                                    Column: 'CompanyName'
                                },
                                {
                                    Label: 'Last Updated',
                                    Column: 'LastUpdated'
                                }
                            ],
                            Click: function (e) {
                                NewTrans();
                                mTxtCanvass.val(e.CanvassNumber);
                                mOut.Model.Data.CanvassID = e.ID;
                                mTxtCustomer.val(e.CompanyName);
                                mOut.Model.Data.CustomerID = e.VendorID;

                                mOut.Model.Data.CopyFromCanvass = true;
                                PreSave({
                                    SaveACopy: true,
                                    NoPrompt: true,
                                    Success: function (vData) {
                                        mOut.Model.Data.CopyFromCanvass = false;
                                        LoadTrans({
                                            ID: vData.ID
                                        });
                                        //mOut.Model.Data.ID = vData.ID;
                                    }
                                });
                            }
                        }
                    ], { FilterCustomers: 1 }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            } // END function SearchCanvass() {

            var mTxtDateNeeded = mElement.find('[name=txtDateNeeded]');
            mTxtDateNeeded.click(BrowseDateNeeded);
            function BrowseDateNeeded() {
                if (!HandleEditingSavedAsFinal())
                    return;
                SingleCalendar({
                    ShowTime: true,
                    SetTime: FormatDate(new Date(), { Format: 'h:mm a' }),
                    Click: function (e) {
                        mTxtDateNeeded.val(e);
                    }
                });
            }
            mElement.find('[name=butDateNeeded]').click(BrowseDateNeeded);

            var mTxtBillingAddress = mElement.find('[name=txtBillingAddress]');
            mTxtBillingAddress.on('keypress', function (e) {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (e.key == 'Enter')
                    SearchBillingAddress();
            }); // END mTxtBillingAddress.on('keypress', function(e) {

            function SearchBillingAddress() {
                var pOrgID;

                if (!HandleEditingSavedAsFinal())
                    return;
                if (mOut.TransType == 'PurchaseOrder')
                    pOrgID = sessionStorage.OrgID;
                else
                    if (IsGuid(mOut.Model.Data.CustomerID))
                        pOrgID = mOut.Model.Data.CustomerID;
                    else {
                        NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                        return;
                    }

                BrowsePopup('LibraryFowst', 'Billing Address',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'Address' }],
                            Click: function (e) {
                                mTxtBillingAddress.val(e.Address);
                                mOut.Model.Data.BillingAddressID = e.ID;
                            }
                        }
                    ], {
                        OrganizationID: pOrgID,
                        HideNoAddress: 1
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }
            mElement.find('[name=butSearchBillingAddress]').click(SearchBillingAddress);

            var mTxtDeliveryAddress = mElement.find('[name=txtDeliveryAddress]');
            mTxtDeliveryAddress.on('keypress', function (e) {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (e.key == 'Enter')
                    SearchDeliveryAddress();
            }); // END mTxtBillingAddress.on('keypress', function(e) {

            function SearchDeliveryAddress() {
                var pOrgID;

                if (!HandleEditingSavedAsFinal())
                    return;

                if (!HandleEditingSavedAsFinal())
                    return;
                if (mOut.TransType == 'PurchaseOrder')
                    pOrgID = sessionStorage.OrgID;
                else
                    if (IsGuid(mOut.Model.Data.CustomerID))
                        pOrgID = mOut.Model.Data.CustomerID;
                    else {
                        NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                        return;
                    }

                BrowsePopup('LibraryFowst', 'Delivery Address',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'Address' }],
                            Click: function (e) {
                                mTxtDeliveryAddress.val(e.Address);
                                mOut.Model.Data.DeliveryAddressID = e.ID;
                            }
                        }
                    ], {
                        OrganizationID: pOrgID,
                        HideNoAddress: 1
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }
            mElement.find('[name=butSearchDeliveryAddress]').click(SearchDeliveryAddress);

            // // commendted 2021_08_13__14_25_06_9699
            //function SaveAndFinalizeInvoice(vParams) {
            //    if (!HandleEditingSavedAsFinal({
            //        AllowFinalizedForApproval: true,
            //        AllowSomeSaving: true,
            //        AllowSomeSavingCallback: function () {
            //            mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
            //            mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
            //            mOut.Model.Data.SalesInvoiceRef = mTxtSalesInvoiceRef.val();
            //            mOut.Model.Data.OfficialReceiptRef = mTxtOfficialReceiptRef.val();
            //            mOut.Model.SaveSomeFields({
            //                success: function (vData) {
            //                    if (vData.Error)
            //                        NewMessageBox(vData.Error, 'Failed');
            //                    else
            //                        NewMessageBox('Data were successfully saved.', 'Success');
            //                }
            //            });
            //        } // END AllowSomeSavingCallback: function
            //    })) {
            //        return;
            //    }
            //    mOut.Model.Data.CopyProductsFromInvoiceID = null;
            //    mOut.Model.Data.SetInvoiceID = null;
            //    PreSave({
            //        SaveAndFinalize: true,
            //        NoPrompt: true,
            //        Success: vParams.Success
            //    });
            //}

            mElement.find('[name=butSaveAsDraft]').click(function () {
                SaveAsDraft();
            });

            mOut.SaveAsDraft = SaveAsDraft;
            function SaveAsDraft(vParams) {
                if (!HandleEditingSavedAsFinal())
                    return;
                mOut.Model.Data.CopyProductsFromInvoiceID = null;
                mOut.Model.Data.SetInvoiceID = null;
                PreSave(vParams);
            };

            mElement.find('[name=butSaveACopy]').click(function () {
                NewMessageBox('This feature is under development.', 'Save a copy');
                return;

                mOut.Model.Data.CopyProductsFromInvoiceID = mOut.Model.Data.ID;
                mOut.Model.Data.ID = null;
                PreSave({
                    SaveACopy: true
                });
            });

            var mTxtRemarks = mElement.find('[name=txtRemarks]');
            mTxtRemarks.on('keypress', function () {
                if (!HandleEditingSavedAsFinal())
                    return;
            });
            function PreSave(vParams) {
                vParams = $.extend({
                    UseUnitPriceFromLibrary: false,
                    SaveAndFinalize: false,
                    SaveACopy: false,
                    NoPrompt: false,
                    RelatedSalesOrders: null,
                    Success: function () {
                        NewMessageBox('Transaction was successfully saved.');
                    }
                }, vParams);

                var pPaymentTerms = mTxtPaymentTerms.val();
                if ((CStr2(pPaymentTerms).trim() != '')
                    && !IsNumeric(pPaymentTerms)) {

                    NewMessageBox('Mobiz cannot save your current transaction because: Payment terms must be a valid number. You entered ' + pPaymentTerms + '.');
                    return;
                }
                pPaymentTerms = CDec2(pPaymentTerms);
                pPaymentTerms = parseInt(pPaymentTerms);
                if (pPaymentTerms < 0) {
                    NewMessageBox('Mobiz cannot save your current transaction because: Payment terms must be equal to or greater than zero. You entered ' + pPaymentTerms + '.');
                    return;
                }
                if (mMaxPaymentTerm < pPaymentTerms) {
                    NewMessageBox('Mobiz cannot save your current transaction because: The maximum allowed payment term for this ' + mTargetOrgType + ' is ' + mMaxPaymentTerm + ' but you entered ' + pPaymentTerms + '. Please enter a value that is equal to or lesser than ' + mMaxPaymentTerm + '.');
                    return;
                }
                if ((mOut.Model.Data.TransType == 'SalesOrder')
                    || (mOut.Model.Data.TransType == 'Invoice')) {

                    if (CBool(vParams.SaveAndFinalize)
                        && (CStr2(mTxtDateNeeded.val()).trim() == '')) {

                        NewMessageBox('Mobiz cannot save your current transaction because Date Needed is required.');
                        return;
                    }
                }

                mOut.Model.Data.FowstID = sessionStorage.FowstID;
                mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
                mOut.Model.Data.DateNeeded = mTxtDateNeeded.val();
                mOut.Model.Data.Remarks = mTxtRemarks.val();
                if (mOut.TransType == 'PurchaseOrder')
                    mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                else
                    mOut.Model.Data.PurchaseOrderText = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderDate = mTxtPoDate.val();
                mOut.Model.Data.QuotationNumText = mTxtQuotationNumText.val();
                mOut.Model.Data.QuotationDate = mTxtQuoteDate.val();
                mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                mOut.Model.Data.PaymentTermsText = pPaymentTerms;
                mOut.Model.Data.PaymentTerm = pPaymentTerms;
                mOut.Model.Data.DocumentNameID = null; // mCbInvoiceType.val();
                mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                mOut.Model.Data.DeliveryMethod = mCbDeliveryMethod.val();
                mOut.Model.Data.SalesInvoiceRef = mTxtSalesInvoiceRef.val();
                mOut.Model.Data.OfficialReceiptRef = mTxtOfficialReceiptRef.val();

                mOut.Model.Data.Products = [];

                var pMessage;
                var pTitle;

                if (!vParams.NoPrompt) {
                    if (vParams.SaveACopy) {
                        pTitle = 'Are you sure you want to save a copy of this transaction document?';
                        pMessage = 'This source transaction will not be updated.\n' +
                            'Instead, the changes you’ve made to this original transaction will get saved with the new transaction.';
                    } else if (vParams.SaveAndFinalize) {
                        var pForApproval = mFinalizedForApproval ? ' for approval' : '';
                        pTitle = 'Save ' + (vParams.SaveAndFinalize ? 'And Finalize' + pForApproval : 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize' + pForApproval + ' ' : '') + 'this transaction?<br /><br />';
                        pMessage += 'Finalizing this transaction means that you will not be able to edit this transaction\'s information later on.<br /><br />';
                        pMessage += 'Only finalized transactions can be printed out (subject to print number limits set by your system administrator).';
                    } else {
                        pTitle = 'Save ' + (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?';
                    }

                    mOut.Model.Verify({
                        Success: function () {
                            NewMessageBox(pMessage, pTitle, [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        Save({
                                            UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                                            SaveAndFinalize: vParams.SaveAndFinalize,
                                            Success: vParams.Success
                                        });
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        },
                        Fail: function (vMessage) {
                            NewMessageBox(vMessage);
                        }
                    });
                } else {
                    Save({
                        UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                        Success: vParams.Success
                    });
                }
            } // END function PreSave(vParams) {

            function Save(vParams) {
                vParams = $.extend({
                    NoPrompt: false,
                    Success: function () {
                        if (!vParams.NoPrompt)
                            NewMessageBox('Transaction was successfully saved.');
                    }
                }, vParams);
                mOut.Model.Save({
                    Data: {
                        ConsolidateSalesOrder: vParams.ConsolidateSalesOrder,
                        UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                        IsPos: 1
                    },
                    Success: function (vData) {
                        //if (CDec2(vData.OtherOrderCount) > 0) {
                        //    //&& !vParams.NoPrompt) {

                        //    NewMessageBox('Items from child orders are ready for invoicing. Do you want to include those items to this invoice?', 'Child orders?', [
                        //        {
                        //            Text: 'Yes',
                        //            Click: function() {
                        //                vParams.ConsolidateSalesOrder = true;
                        //                Save(vParams);
                        //            }
                        //        },
                        //        {
                        //            Text: 'No',
                        //            Click: function() {
                        //                vParams.ConsolidateSalesOrder = false;
                        //                Save(vParams);
                        //            }
                        //        },
                        //        {
                        //            Text: 'Cancel'
                        //        }
                        //    ]);
                        //    return;
                        //}
                        ERP.Payment.controller.WebPosInvoiceID = vData.ID;
                        LoadTrans({
                            ID: vData.ID,
                            AfterLoad: function () {
                                vParams.Success(vData);
                                if (!CBool(vData.FinalizedForApproval))
                                    mSavedAsFinal = vParams.SaveAndFinalize;
                                if (CBool(mSavedAsFinal))
                                    NewMessageBox('Do you want to print this transaction?', 'Print?', [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                HandlePrint({
                                                    NewTransAfterPrint: true
                                                });
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                    ]);
                            }
                        });
                        //if (CStr2(vData.QuotationNo) != '')
                        //    mTxtQuoteNumber.val(vData.QuotationNo);
                        //if (CStr2(vData.SalesOrderNo) != '')
                        //    mTxtSalesOrderNumber.val(vData.SalesOrderNo);
                        //if (CStr2(vData.InvoiceNumber) != '')
                        //    mTxtInvoiceNumber.val(vData.InvoiceNumber);
                        //if (CStr2(vData.PurchaseOrderNo) != '')
                        //    mTxtPoNumber.val(vData.PurchaseOrderNo);
                    },
                    Fail: function (vMessage) {
                        NewMessageBox('Mobiz cannot save your current transaction because: ' + vMessage);
                    }
                });
            }

            var mTxtQuotationNumText = mElement.find('[name=txtQuotationNumText]');
            var mTxtQuoteNumber = mElement.find('[name=txtQuoteNumber]');
            mTxtQuoteNumber.click(SearchQuotes);
            mTxtQuoteNumber.on('keypress', function (e) {
                if (mOut.TransType == 'SalesOrder')
                    if (e.key == 'Enter') {
                        SearchQuotes();
                    }
            });
            mElement.find('[name=butQuoteNumber]').click(SearchQuotes);
            function SearchQuotes() {
                if (!HandleEditingSavedAsFinal())
                    return;
                NewTrans();
                Find({
                    TransType: 'Quote',
                    DontReplaceCurrentID: true,
                    QueryParams: {
                        FilterQuotations: 1,
                        CustomerID: mOut.Model.Data.CustomerID
                    },
                    DontLoadCancelled: true,
                    AfterLoad: function (vData) {
                        mOut.Model.Data.CopyProductsFromInvoiceID = vData.ID;
                        mOut.Model.Data.QuotationID = vData.ID;
                        mOut.Model.Data.ID = null;
                        PreSave({
                            SaveACopy: true,
                            NoPrompt: true,
                            Success: function (vData) {
                                LoadTrans({
                                    ID: vData.ID
                                });
                                //mOut.Model.Data.ID = vData.ID;
                            }
                        });
                    } // END AfterLoad: function(vData) {
                }); // END Find({
            }

            var mTxtQuoteDate = mElement.find('[name=txtQuoteDate]');
            mTxtQuoteDate.click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                SingleCalendar({
                    Click: function (e) {
                        mTxtQuoteDate.val(e);
                    }
                });
            });

            function BrowseSalesOrder() {
                if (mOut.TransType == 'Invoice')
                    NewTrans();
                Find({
                    TransType: 'SalesOrder',
                    DontReplaceCurrentID: true,
                    QueryParams: {
                        FilterSalesOrders: 1,
                        ExcludeInvoiced: 1,
                        InvoiceFowstID: sessionStorage.FowstID
                    },
                    AfterLoad: function (vData) {
                        mRelatedOrdersPopup.popup({
                            ID: vData.ID,
                            OnOk: function (vRelatedSalesOrderData) {
                                mOut.Model.Data.CopyProductsFromInvoiceID = vRelatedSalesOrderData.IDs;
                                mOut.Model.Data.CopyProductsFromSalesOrderNumbers = vRelatedSalesOrderData.OrderNumbers;
                                mOut.Model.Data.ID = null;
                                mOut.Model.Data.SetInvoiceID = true;
                                mOut.Model.Data.SalesOrderID = vRelatedSalesOrderData.ID;
                                mMaxPaymentTerm = CDec2(mTxtPaymentTerms.val());
                                PreSave({
                                    SaveACopy: true,
                                    RelatedSalesOrders: vRelatedSalesOrderData.Data,
                                    NoPrompt: true,
                                    Success: function (vData2) {
                                        LoadTrans({
                                            ID: vData2.ID
                                        });
                                    }
                                });
                            },
                            OnCancel: function () {
                                NewTrans();
                            }
                        }); // for testing only

                    } // END AfterLoad: function(vData) {
                }); // END Find({
            }

            var mTxtSalesOrderNumber = mElement.find('[name=txtSalesOrderNumber]');
            mTxtSalesOrderNumber.click(BrowseSalesOrder); // END mTxtSalesOrderNumber.click(function() {
            mElement.find('[name=butSalesOrderNumber]').click(BrowseSalesOrder);

            var mTxtSalesOrderDate = mElement.find('[name=txtSalesOrderDate]');
            var mTxtPoNumber = mElement.find('[name=txtPoNumber]');
            var mTxtPoDate = mElement.find('[name=txtPoDate]');
            mTxtPoDate.click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                SingleCalendar({
                    Click: function (e) {
                        mTxtPoDate.val(e);
                    }
                });
            });

            var pProductTable = mElement.find('[name=tableProducts]');
            var pProductRow = pProductTable.find('tbody tr');
            pProductRow.remove();
            mElement.find('[name=butAddProductsFromPIM]').click(function (e) {
                e.preventDefault();
                sessionStorage.IsFromPOModule = true;

                //if (CStr2(mOut.Model.Data.ID) == '') {
                SaveAsDraft({
                    NoPrompt: true,
                    Success: $('#pagePIMUpload').popup('open')
                });
                //}

                return false;
            })

            mElement.find('[name=butCopyProducts]').click(function (e) {
                e.preventDefault();
                if (!HandleEditingSavedAsFinal())
                    return;

                mOut.Model.Verify({
                    Success: function () {
                        if (mOut.TransType == 'PurchaseOrder') {
                            NewMessageBox('', '', [
                                {
                                    Text: 'Load Products From Sales Order',
                                    Click: function () {
                                        FindTransForProductsSource({ TransType: 'SalesOrder' });
                                    }
                                },
                                {
                                    Text: 'Load Products From Purchase Order',
                                    Click: function () {
                                        FindTransForProductsSource();
                                    }
                                }
                            ]);
                        } else
                            FindTransForProductsSource();
                    },
                    Fail: function (vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
                return false;
            }); // END 

            function FindTransForProductsSource(vParams) {
                vParams = $.extend({
                    TransType: mOut.TransType
                }, vParams);

                Find({
                    TransType: vParams.TransType,
                    DontReplaceCurrentID: true,
                    //QueryParams: {
                    //    FilterQuotations: 1
                    //},
                    Click: function (e) {
                        if (mOut.Model.Data.ID == null) {
                            Proceed();
                            return;
                        }

                        mOut.Model.GetCommonLineItemCount({
                            InvoiceID1: mOut.Model.Data.ID,
                            InvoiceID2: e.ID,
                            success: function (vData) {
                                if (!IsNumeric(vData.Result)) {
                                    NewMessageBox(vData.Result);
                                    return;
                                }

                                var pCount = CInt2(vData.Result);
                                if (pCount > 0) {
                                    var pMessage = 'You are about to add ' + pCount + ' line items that already exist in this transaction. What do you want to do?';
                                    NewMessageBox(pMessage, 'Item already exist', [
                                        {
                                            Text: 'Merge',
                                            Click: function () {
                                                Proceed();
                                            }
                                        },
                                        {
                                            Text: 'Cancel'
                                        }
                                    ]);
                                } else
                                    Proceed();

                            }
                        });

                        function Proceed() {
                            mOut.Model.Data.CopyProductsFromInvoiceID = e.ID;
                            mOut.Model.Data.QuotationID = null;
                            mOut.Model.Data.SalesOrderID = null;
                            //mOut.Model.Data.ID = null;
                            PreSave({
                                UseUnitPriceFromLibrary: true,
                                SaveACopy: true,
                                NoPrompt: true,
                                Success: function (vData) {
                                    LoadTrans({
                                        ID: vData.ID
                                    });
                                    //mOut.Model.Data.ID = vData.ID;
                                }
                            });
                        }

                    }
                }); // END Find({
            }

            mElement.find('[name=butAddProduct]').click(function (e) {
                e.preventDefault();
                if (CStr2(mOut.Model.Data.ID) == '') {
                    SaveAsDraft({
                        NoPrompt: true,
                        Success: AddProduct
                    });
                } else
                    AddProduct();
                return false;
            }); // END mElement.find('[name=butAddProduct]').click(function() {

            mElement.find('[name=butAddProductFromSupplier]').click(function (e) {
                e.preventDefault();
                if (CStr2(mOut.Model.Data.ID) == '') {
                    SaveAsDraft({
                        NoPrompt: true,
                        Success: function () {
                            AddProduct({
                                FilterSupplierID: mOut.Model.Data.CustomerID,
                                AllowEmptySearch: true
                            });
                        }
                    });
                } else
                    AddProduct({
                        FilterSupplierID: mOut.Model.Data.CustomerID,
                        AllowEmptySearch: true
                    });
                return false;
            }); // END mElement.find('[name=butAddProduct]').click(function() {

            mElement.find('#formPunchItem').submit(function (e) {
                e.preventDefault();
                if (CStr2(mOut.Model.Data.ID) == '') {
                    SaveAsDraft({
                        NoPrompt: true,
                        Success: AddProduct
                    });
                } else
                    AddProduct();
                
            })

            function HandleAllowEncodingOfQuantitiesInInvoice() {
                //if ((mOut.TransType == 'Invoice')
                //    && (!ERP.Invoicing.settings.controller.Model.Data.AllowEncodingOfQuantitiesInInvoice)) {

                //    NewMessageBox('The current setting does not allow direct invocing. All orders need to be initiated by a sales order transaction first.');
                //    return false;
                //}
                return true;
            }

            function HandleFromSalesOrder() {
                if (mFromSalesOrder
                    && (mOut.TransType == 'Invoice')) {

                    NewMessageBox('You may not add products to, modify served quantities, or delete products from invoices that came from sales order.');
                    return false;
                }
                return true;
            }

            function AddProduct(vParams) {
                vParams = $.extend({
                    FilterSupplierID: null,
                    AutoSearch: false,
                    AllowEmptySearch: false
                }, vParams);
                if (!HandleEditingSavedAsFinal())
                    return;
                if (!HandleFromSalesOrder())
                    return;
                if (mOut.Model.Data.CustomerID == null) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }
                var pColumns = [
                    {
                        Column: 'ProductNo',
                        Label: 'Product No.',
                        Style: {
                            width: '100px'
                        }
                    },
                    {
                        Column: 'ProductName',
                        Label: 'Product Name'
                    }
                ];
                var pAdditionalData = {
                    IsAll: 1,
                    UseCacheProductSearch: 0,
                    //UseCacheProductSearch: 1, // old value 2021_06_04__10_50_24_5873
                    InventoryFowstID: sessionStorage.FowstID,
                    FilterSupplierID: vParams.FilterSupplierID,
                    IncludeBaseUnit: 1
                };

                if (mTargetOrgType == 'vendor') {
                    pAdditionalData.VendorID = mOut.Model.Data.CustomerID;
                    pColumns.push({
                        Label: 'Vendor Price',
                        Column: function (vData) {
                            return AccountingFormat(vData.VendorPrice);
                        }
                    });
                } else {
                    pAdditionalData.FilterCustomers = 1;
                    if (CStr2(mFowstGroupPriceName) != '')
                        pColumns.push({
                            Label: mFowstGroupPriceName,
                            Column: function (vData) {
                                return AccountingFormat(vData.FowstGroupPrice);
                            },
                            Style: {
                                width: '100px'
                            }
                        });
                    pColumns.push({
                        Label: 'Retail Price',
                        Column: function (vData) {
                            return AccountingFormat(vData.RetailPrice);
                        },
                        Style: {
                            width: '100px'
                        }
                    });
                }
                if (mOut.TransType == 'Invoice')
                    pColumns = pColumns.concat([
                        {
                            Label: 'Served Qty',
                            Render: function (v1, v2, vData) {
                                return '<div name="divQty" />';
                            },
                            Style: {
                                width: '100px'
                            }
                        }
                    ]);
                else
                    pColumns = pColumns.concat([
                    {
                        Label: 'Qty',
                        Render: function (v1, v2, vData) {
                            return '<div name="divQty" />';
                            //return '<input class="qty" style="width:70px" />';
                        }
                    }
                    ]);
                pColumns.push({
                    Label: 'Packing Size (in smallest unit)',
                    Render: function (v1, v2, vData) {
                        return 'Change';
                    }
                });
                pColumns.push({
                    Label: 'Qty Available to Commit',
                    Render: function (v1, v2, vData) {
                        var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
                        p = '<div style="text-align: center">' + p + '</div>';
                        return p;
                    }
                });
                pColumns.push(
                    {
                        Label: 'Action',
                        Render: function (v1, v2, vData) {
                            return '<div name="divAction"></div>';
                        }
                    }
                );
                ERP.QuickPim.controller.AfterSave = SelectProduct;

                function CacheProductSearch() {
                    var pData = {
                        keywords: $('#txtBrowseLibrary').val(),
                        FilterCustomers: 1,
                        IsDeactivated: 0,
                        CacheProductSearch: 1
                    };
                    AjaxSend({
                        url: 'Browser/LibraryPIMProduct',
                        data: pData
                    });
                }

                BrowsePopup('LibraryPIMProduct', 'Product - ' + sessionStorage.FowstName,
                    [
                        {
                            ModalSize: 'xl',
                            AutoDismiss: false,
                            AutoSearch: vParams.AutoSearch,
                            ShowOkButton: true,
                            DisplayText: pColumns,
                            Click: function (e) {
                                //if (!e.FromCacheProductSearch)
                                //    CacheProductSearch();
                                if ($.isArray(e)) {
                                    PreSelect(e);
                                } else {

                                    SelectProduct(e);
                                }
                            },
                            Search: $.trim($('#txtPunchItem').val()),
                            AllowEmptySearch: vParams.AllowEmptySearch,
                            StyleFormat: 'table',
                            InitComplete: function (vTable) {
                                vTable.find('.qty').click(function (e) {
                                    e.stopPropagation();
                                });
                            },
                            rowCallback: function (row, data, dataIndex) {
                                var pDiv = $(row).find('[name=divAction]');
                                if (pDiv.children().length > 0)
                                    return;
                                var pLink = $('<a>')
                                    .text('Info')
                                    .attr('href', '#')
                                    .click(function (e) {
                                        e.preventDefault();
                                        mInventoryBalance.Show({
                                            ProductID: data.ID,
                                            ProductNo: data.ProductNo,
                                            ProductName: data.ProductName,
                                            OrderController: mOut,
                                            AfterSave: function () {
                                                LoadTrans({
                                                    ID: mOut.Model.Data.ID,
                                                    LoadProductsOnly: true
                                                });
                                            }
                                        });
                                        return false;
                                    });
                                pDiv.append(pLink);
                                var pDiv = $(row).find('[name=divQty]');
                                pDiv.click(function (e) {
                                    e.preventDefault();
                                    return false;
                                });
                                var pDisabled = '';
                                if ((CDec2(data.FowstGroupPrice) == 0)
                                    && (CDec2(data.RetailPrice) == 0))
                                    pDisabled = ' disabled ';
                                pDiv.append('<input class="qty form-control" ' + pDisabled + ' type="number" />');
                            },
                            Close: function () {
                                $('#txtPunchItem').val('').focus();
                            }
                        }
                    ], pAdditionalData
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            function PreSelect() {
                var pTxts = $('#tblBrowsePopup .qty');
                var pData = [];
                var pIndex;
                var pBelowGpmThreshold = false;
                for (var p = 0; p <= pTxts.length - 1; p++) {
                    var pTxt = $(pTxts[p]);
                    var pQty = CDec2(pTxt.val());
                    if (pQty > 0) {
                        var pTR = pTxt.closest('tr');
                        var pDR = pTR.data('data');
                        var pPrice = pDR.RetailPrice;
                        if (CDec2(pDR.FowstGroupPrice) > 0)
                            pPrice = CDec2(pDR.FowstGroupPrice);
                        pData.push({
                            ID: pDR.ID,
                            Qty: pQty,
                            EditPrice: pPrice,
                            UnitMeasure: pDR.UnitMeasure,
                            UnitQty: pDR.UnitQty
                        });
                    } // todo: if qty = 0, delete at server
                }

                PreSelectSave(pData)
            }

            function PreSelectSave(pData) {
                var pBelowGpmThreshold = false;
                if (pData.length > 0) {
                    $.mobile.showPageLoadingMsg();
                    function Proceed() {
                        var pParams = $.extend({
                            Fail: function (vError) {
                                $.mobile.hidePageLoadingMsg();
                                NewMessageBox(vError);
                                AfterProductSave();
                            },
                            Success: function (vResult) {
                                if (pBelowGpmThreshold == false)
                                    pBelowGpmThreshold = vResult.BelowGpmThreshold;
                                pIndex++;
                                if (pIndex <= pData.length - 1)
                                    Proceed();
                                else {
                                    $.mobile.hidePageLoadingMsg();
                                    AfterProductSave({
                                        BelowGpmThreshold: pBelowGpmThreshold
                                    });
                                }
                            }
                        }, pData[pIndex]);
                        SavePreSelected(pParams);
                    }
                    pIndex = 0;
                    Proceed();
                }
            }

            mElement.find('[name=butAddSalesRep]').click(function () {
                //if (!HandleEditingSavedAsFinal())
                //    return;
                if (mOut.TransType == 'PurchaseOrder') {
                    if (IsGuid(mOut.Model.Data.CustomerID)) {
                        // just do nothing
                    } else {
                        NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                        return;
                    }
                }
                if (mTxtSalesRepText.val().trim() == '') {
                    NewMessageBox('Please enter the sales rep first.');
                    return;
                }

                NewMessageBox('Are you sure you want to add the sales rep, ' + mTxtSalesRepText.val().trim() + '?', 'Add Sales Rep', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            AddSalesRep({
                                Name: mTxtSalesRepText.val().substring(0, 500),
                                Success: function (vData) {
                                    //mOut.Model.Data.BillingAddressID = vData.ID;
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function () {

                        }
                    }
                ]);
            });

            function AddSalesRep(vParams) {
                if (mOut.TransType == 'PurchaseOrder') {
                    mOut.Model.SaveContactPerson({
                        LastName: vParams.Name,
                        OrgID: mOut.Model.Data.CustomerID,
                        Success: function (vData) {
                            NewMessageBox('Sales rep was successfully added.');
                            vParams.Success(vData);
                        },
                        Fail: function (vMessage) {
                            NewMessageBox(vMessage);
                        }
                    });
                } else {
                    mOut.Model.AddSalesRep({
                        Name: vParams.Name,
                        Success: function (vData) {
                            NewMessageBox('Sales rep was successfully added.');
                            vParams.Success(vData);
                        },
                        Fail: function (vMessage) {
                            NewMessageBox(vMessage);
                        }
                    });
                }
            }

            function SavePreSelected(vParams) {
                var pData = {
                    CustomerID: mOut.Model.Data.CustomerID,
                    TransType: mOut.TransType,
                    DeleteExistingProduct: 0,
                    DontCheckCartValidity: 1,
                    OrderID: mOut.Model.Data.ID,
                    NewCartMode: !IsGuid(mOut.Model.Data.ID),
                    ProductID: vParams.ID,
                    StockID: vParams.ID,
                    SalesRep: [],
                    ProductSerial: [],
                    UpdateOrderedQty: 1,
                    IsPos: 1,
                    Stocks: [
                        {
                            Qty: vParams.Qty,
                            UnitQty: vParams.UnitQty,
                            EditPrice: vParams.EditPrice,
                            UnitMeasure: vParams.UnitMeasure
                        }
                    ]
                };

                if (!IsGuid(pData.CustomerID)) {
                    NewMessageBox('Invalid/missing CustomerID: ' + pData.CustomerID + '. This known bug is still unresolved. Workaround: reload this page and try again. ref: jsFKWML635AP');
                    console.trace();
                    return;
                }
                Bs.Loading('show');
                AjaxSend({
                    url: '/Stock/UpdatePosLite',
                    data: pData,
                    NoSplash: true,
                    success: function (vData) {
                        if (vData.Error) {
                            vParams.Fail(vData.Error);
                        } else if (vData.Result == 'ok') {
                            mOut.Model.Data.ID = vData.OrderID;
                            pProductPopup.Model.Data.NewCartMode = false;
                            pProductPopup.Model.Data.OrderID = vData.OrderID;
                            vParams.Success(vData);
                        } else {
                            vParams.Fail('Unknown status. ref: jsCSLRU5TDDJ!');
                        }

                    }
                });
            }

            function SelectProduct(vData) {
                //pProductPopup.Model = new ERP.OrderProcessing.product.model();
                pProductPopup.Model.Data.TransType = mOut.TransType;
                //pProductPopup.AfterSave = AfterProductSave;
                console.log('selected product ID: ' + vData.ID);
                pProductPopup.popup({
                    ProductID: vData.ID,
                    ProductName: vData.ProductName,
                    ProductNo: vData.ProductNo,
                    CustomerID: mOut.Model.Data.CustomerID,
                    TransType: mOut.TransType,
                    ParentController: mOut,
                    MaxPaymentTerm: mMaxPaymentTerm,
                    TargetOrgType: mTargetOrgType,
                    Success: function () {

                    }
                });
            }

            function Item_Click(vProductID, vDR) {
                if (!HandleEditingSavedAsFinal())
                    return;
                vDR = $.extend({}, vDR);
                var pButtons = [];
                if (mOut.TransType != 'AccountsPayable')
                    pButtons.push(
                        {
                            Text: 'Add New Line Item',
                            Click: function () {
                                AddProduct();
                            }
                        }
                    );
                pButtons = pButtons.concat([
                    {
                        Text: 'Edit This Line Item',
                        Click: function () {
                            //pProductPopup.Model = new ERP.OrderProcessing.product.model();
                            pProductPopup.Model.Data.TransType = mOut.TransType;
                            //pProductPopup.AfterSave = AfterProductSave;

                            pProductPopup.popup({
                                EditMode: true,
                                InventoryBalance: mInventoryBalance,
                                ProductID: vProductID,
                                ProductName: vDR.ProductName,
                                ProductNo: vDR.ProductNo,
                                CustomerID: mOut.Model.Data.CustomerID,
                                TransType: mOut.TransType,
                                ParentController: mOut,
                                FromSalesOrder: mFromSalesOrder,
                                MaxPaymentTerm: mMaxPaymentTerm,
                                TargetOrgType: mTargetOrgType,
                                Success: function () {
                                }
                            });
                        }
                    },
                    {
                        Text: 'Enter Serial Numbers and IMEIs',
                        Click: function () {
                            mSerialNumbersPopup.popup({
                                InvoiceID: mOut.Model.Data.ID,
                                InventoryBalance: mInventoryBalance,
                                ProductID: vProductID,
                                ProductName: vDR.ProductName,
                                ProductNo: vDR.ProductNo,
                                CustomerID: mOut.Model.Data.CustomerID,
                                TransType: mOut.TransType,
                                ParentController: mOut,
                                Success: function () {
                                }
                            });
                        }
                    }
                ]);
                if (mOut.TransType != 'AccountsPayable')
                    pButtons.push(
                        {
                            Text: 'Remove This Line Item From Cart',
                            Click: function () {
                                RemoveFromCartPrompt(vProductID, vDR);
                            }
                        }
                    );
                pButtons.push(
                    //{
                    //    Text: 'View This Product\'s Information',
                    //    Click: function() {
                    //        NewMessageBox('This function is not yet implemented. ref: jsY7TDHS9SS3!');
                    //    }
                    //},
                    {
                        Text: 'View This Product\'s Inventory Balance(s)',
                        Click: function () {
                            mInventoryBalance.Show({
                                ProductID: vProductID,
                                ProductNo: vDR.ProductNo,
                                ProductName: vDR.ProductName,
                                OrderController: mOut,
                                AfterSave: function () {
                                    LoadTrans({
                                        ID: mOut.Model.Data.ID,
                                        LoadProductsOnly: true
                                    });
                                }
                            });
                        }
                    }
                );
                if (mOut.TransType == 'AccountsPayable') {
                    pButtons = pButtons.concat([
                        {
                            Text: 'Set Unit Price To Last Purchase Order (PO) Price',
                            Click: function () {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'LastPurchaseOrderPrice');
                            }
                        },
                        {
                            Text: 'Set Unit Price To Last Accounts Payable (AP) Price',
                            Click: function () {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'LastAccountsPayablePrice');
                            }
                        },
                        {
                            Text: 'Set Unit Price To AP or PO Price, Whichever Is Lower',
                            Click: function () {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'ApOrPoWhicheverIsLower');
                            }
                        },
                        {
                            Text: 'Set Unit Price To AP or PO Price, Whichever Is Newer',
                            Click: function () {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'ApOrPoWhicheverIsNewer');
                            }
                        }
                    ]);
                }
                PopupButtonList('Select Options', vDR.ProductName, pButtons);

            }

            function SetLineItemUnitPrice(vProductID, vItemID, vMethod) {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (IsGuid(mOut.Model.Data.ID) == false) {
                    NewMessageBox('Please save this transaction first. ref: jsvMethod!');
                    return;
                }
                AjaxSend({
                    url: 'ERP/SetLineItemPrice',
                    data: {
                        InvoiceID: mOut.Model.Data.ID,
                        VendorID: mOut.Model.Data.CustomerID,
                        SourceMethod: vMethod,
                        ItemID: vItemID,
                        ProductID: vProductID
                    },
                    success: function (vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error, 'Failed To Saved');
                        else if (vData.Result == 'ok') {
                            NewMessageBox('Prices were successfully saved.', 'Success');
                            LoadTrans({
                                ID: mOut.Model.Data.ID,
                                LoadProductsOnly: true,
                                IncludeZeroQty: true
                            });
                        } else
                            NewMessageBox('Done');
                    }
                });
            }

            function RemoveFromCartPrompt(vProductID, vDR) {
                if (!HandleFromSalesOrder())
                    return;
                NewMessageBox('Are you sure you want to remove ' + vDR.ProductName + ' from cart?', 'Remove from cart', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            mOut.Model.RemoveProduct({
                                ProductID: vProductID,
                                UnitMeasure: vDR.UnitMeasure,
                                ItemID: vDR.ID,
                                Success: function () {
                                    LoadTrans({
                                        ID: mOut.Model.Data.ID,
                                        LoadProductsOnly: true
                                    });
                                },
                                Fail: function (vMessage) {
                                    NewMessageBox(vMessage);
                                }
                            });
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);
            }

            mElement.find('[name=butLookup]').click(function () {
                PopupButtonList('', 'Search', [
                    {
                        Text: 'Sales Invoice', Click: function () {
                            if (!$.isEmptyObject(mOut.Model.Data.CustomerID)) {
                                var pMsg = 'Loading another record will discard the unsaved changes from the current record. Do still want to search for another record?';
                                NewMessageBox(pMsg, '', [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            Find({
                                                TransType: mOut.TransType
                                            });
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                        }
                                    }
                                ]);
                            } else
                                Find({
                                    TransType: mOut.TransType
                                });
                        }
                    },
                    {
                        Text: 'Sales Order', Click: function () {
                            BrowseSalesOrder();
                        }
                    }
                ])                
            });

            function Find(vParams) {
                var pTitle;
                var pColumns = [];

                if (vParams.TransType == 'Quote') {
                    pTitle = 'Quotations';
                    pColumns = [
                        {
                            Label: 'Quotation No.',
                            Column: 'QuotationNo'
                        },
                        {
                            Label: 'Customer',
                            Column: 'CompanyName'
                        },
                        {
                            Label: 'Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.Total);
                            }
                        }
                    ];
                } else if (vParams.TransType == 'SalesOrder') {
                    pTitle = 'Sales Order';
                    pColumns = [
                        {
                            Label: 'Sales Order No.',
                            Column: function (vDR) {
                                var pOut = CStr2(vDR.MIN).trim();
                                if (pOut != '')
                                    pOut = vDR.MIN + '-';
                                if (vDR.TransType == 'StockTransfer')
                                    pOut = 'STR-' + pOut + vDR.StrNum;
                                else if (CDec2(vDR.PurchaseOrderNo) > 0)
                                    pOut = 'PO-' + pOut + vDR.PurchaseOrderNo;
                                else
                                    pOut = 'SO-' + pOut + vDR.SalesOrderNo;

                                return pOut;
                            }
                        },
                        {
                            Label: 'Parent Sales Order No.',
                            Column: function (vDR) {
                                return CStr2(vDR.ParentSalesOrderNo);
                            }
                        },
                        {
                            Label: 'Serving Location',
                            Column: 'FOWSTName'
                        },
                        {
                            Label: 'Fulfillment Status',
                            Column: 'FulfillmentStatus'
                        },
                        {
                            Label: 'Order Entry Location',
                            Column: function (vDR) {
                                if (CStr2(vDR.EncodingFOWSTName).trim() == '')
                                    return vDR.FOWSTName;
                                else
                                    return vDR.EncodingFOWSTName;
                            }
                        },
                        {
                            Label: 'Customer',
                            Column: 'CompanyName'
                        },
                        {
                            Label: 'Ordered Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.OrderedAmount);
                            }
                        },
                        {
                            Label: 'Served Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.ServedAmount);
                            }
                        }
                    ];
                } else if (vParams.TransType == 'Invoice') {
                    pTitle = 'Invoice';
                    pColumns = [
                        {
                            Label: 'Invoice No.',
                            Column: function (vDR) {
                                var pOut = '';
                                if (CStr2(vDR.FowstWideInvoiceNumber).trim() != '')
                                    pOut = vDR.FOWSTNo + '-' + vDR.FowstWideInvoiceNumber;
                                else if (CStr2(vDR.MIN).trim() != '')
                                    pOut = vDR.MIN + '-' + vDR.InvoiceNumber;
                                else
                                    pOut = vDR.InvoiceNumber;
                                if (CStr2(vDR.DocumentName).trim() == '')
                                    return 'SI-' + pOut;
                                else
                                    return vDR.DocumentName + '-' + pOut;
                            }
                        },
                        {
                            Label: 'Sales Order No.',
                            Column: function (vDR) {
                                return CStr2(vDR.InvoiceSalesOrderNo);
                            }
                        },
                        {
                            Label: 'Manual Invoice No.',
                            Column: 'RefInvoiceNum'
                        },
                        {
                            Label: 'Serving Location',
                            Column: 'FOWSTName'
                        },
                        {
                            Label: 'Order Entry Location',
                            Column: function (vDR) {
                                if (CStr2(vDR.EncodingFOWSTName).trim() == '')
                                    return vDR.FOWSTName;
                                else
                                    return vDR.EncodingFOWSTName;
                            }
                        },
                        {
                            Label: 'Customer',
                            Column: 'CompanyName'
                        },
                        {
                            Label: 'Ordered Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.OrderedAmount);
                            }
                        },
                        {
                            Label: 'Served Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.ServedAmount);
                            }
                        }
                    ];
                } else if (vParams.TransType == 'PurchaseOrder') {
                    pTitle = 'Purchase Order';
                    pColumns = [
                        {
                            Label: 'PO No.',
                            Column: function (vDR) {
                                var pOut = '';
                                if (CStr2(vDR.MIN).trim() != '')
                                    pOut = vDR.MIN + '-';
                                if (!IsNumeric(vDR.PurchaseOrderNo))
                                    vDR.PurchaseOrderNo = 0;
                                return 'PO-' + pOut + vDR.PurchaseOrderNo;
                            }
                        },
                        {
                            Label: 'Vendor',
                            Column: 'CompanyName'
                        },
                        {
                            Label: 'Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.Total);
                            }
                        }
                    ];
                } else if (vParams.TransType == 'Canvass') {
                    pTitle = 'Price Canvass';
                    pColumns = [
                        {
                            Label: 'PC No.',
                            Column: function (vDR) {
                                var pOut = '';
                                if (CStr2(vDR.MIN).trim() != '')
                                    pOut = vDR.MIN + '-';
                                if (!IsNumeric(vDR.PurchaseOrderNo))
                                    vDR.PurchaseOrderNo = 0;
                                return 'PO-' + pOut + vDR.PurchaseOrderNo;
                            }
                        },
                        {
                            Label: 'Canvass Name',
                            Column: 'CanvassName'
                        },
                        {
                            Label: 'Vendor',
                            Column: 'CompanyName'
                        },
                        {
                            Label: 'Amount',
                            Column: function (vDR) {
                                return AccountingFormat(vDR.Total);
                            }
                        }
                    ];

                }

                pColumns = pColumns.concat([
                    {
                        Label: 'SKU Count',
                        Column: 'SkuCount'
                    },
                    {
                        Label: 'Date Created',
                        Column: 'DateCreated'
                    },
                    {
                        Label: 'Last Updated',
                        Column: 'LastUpdated'
                    },
                    {
                        Label: 'Updated By',
                        Column: 'EmployeeFullName'
                    }
                ]);

                if ((vParams.TransType == 'Invoice')
                    || (vParams.TransType == 'SalesOrder')) {
                    pColumns = pColumns.concat([
                        {
                            Label: 'Action',
                            Column: function () {
                                return '<div name="divAction" />';
                            }
                        }
                    ]);
                }

                if (vParams.Click == null)
                    vParams.Click = function (e) {
                        if (CBool(vParams.DontLoadCancelled))
                            if (CBool(e.IsCancelled)) {
                                NewMessageBox('The selected transaction was already cancelled. Please select a transaction that is still active.');
                                return;
                            }
                        if ((mOut.TransType == 'Invoice')
                            && (vParams.TransType == 'SalesOrder')) {

                            if ((CStr2(e.CnCUnapprovedReason).trim() != '')
                                && (CStr2(e.CncApprovedByID).trim() == '')) {
                                NewMessageBox('This transaction is awaiting approval from credit and collection and may not prepared and/or invoiced prior to said approval.');
                                return;
                            }

                            if (CStr2(e.FinalizePickListByID).trim() == '') {
                                NewMessageBox('This sales order cannot be invoiced at this time. It\'s pick list must be finalized via Fulfillment Center first.');
                                return;
                            }
                        }
                        LoadTrans({
                            ID: e.ID,
                            DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                            AfterLoad: function () {
                                if (vParams.AfterLoad)
                                    vParams.AfterLoad(e);
                            } // END AfterLoad: function() {
                        });
                    }; // END Click: function (e) {

                //if (vParams.Validation == null)
                //    vParams.Validation = function (e) {
                //        if (e.DataTableRowColumnIndex == 8) { // preview column
                //            var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                //                '_blank',
                //                'width=' + window.innerWidth + ',height=' + window.innerHeight);

                //            win.onload = function () {
                //                function LoadRecord() {
                //                    if (win.Z9Q3C9ASDA)
                //                        win.Z9Q3C9ASDA.ShowInvoice({
                //                            ID: e.ID,
                //                            ReadOnly: true
                //                        });
                //                    else
                //                        setTimeout(LoadRecord, 1000);
                //                }
                //                LoadRecord();    
                //            }

                //            return false; // para DILI magclose ang popup
                //        }

                //        return true; // para magclose ang popup
                //    }

                var pShowHideChildSos = false;
                if ((mOut.TransType == 'SalesOrder')
                    && (vParams.TransType == 'SalesOrder'))
                    pShowHideChildSos = true;
                BrowsePopup('TransInvoices', pTitle,
                    [
                        {
                            StyleFormat: 'table',
                            AutoSearch: false,
                            DisplayText: pColumns,
                            Click: vParams.Click,
                            Validation: vParams.Validation,
                            ShowHideChildSos: pShowHideChildSos,
                            rowCallback: function (row, data, dataIndex) {
                                var pCell = $(row).find('[name=divAction]');
                                if (pCell.length == 0)
                                    return;
                                var pLink = $('<a>')
                                    .text('Preview')
                                    .attr('href', '#')
                                    .click(function (e) {
                                        e.preventDefault();
                                        var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                                            '_blank',
                                            'width=' + window.innerWidth + ',height=' + window.innerHeight);

                                        win.onload = function () {
                                            function LoadRecord() {
                                                if (win.Z9Q3C9ASDA)
                                                    win.Z9Q3C9ASDA.ShowSalesOrder({
                                                        ID: data.ID,
                                                        ReadOnly: true
                                                    });
                                                else
                                                    setTimeout(LoadRecord, 1000);
                                            }
                                            LoadRecord();
                                        }

                                        return false; // para DILI magclose ang popup
                                    });
                                pCell.append(pLink);
                            },
                            Sorting: true,
                            NoInitialSorting: true,
                            ModalSize: 'xl' 
                        }
                    ], $.extend({
                        TransType: vParams.TransType,
                        InvoiceSource: 'pos',
                        AnyZero: true,
                        //NonZero: 0,
                        AllowNoItems: true,
                        ShowChildSos: true
                    }, vParams.QueryParams)
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            } // END function Find(vParams) {

            var pIncludeChildOrderItems = false;
            mOut.LoadTrans = LoadTrans;
            if (mOut.TransType == 'SalesOrder') {
                proj.LoadEasSalesOrderTrans = LoadTrans;
                pIncludeChildOrderItems = true;
            }
            function LoadTrans(vParams) {
                vParams = $.extend({
                    ID: null,
                    LoadProductsOnly: false,
                    DontReplaceCurrentID: false,
                    IncludeZeroQty: false,
                    AccountsPayableFollowCost: vParams.AccountsPayableFollowCost
                }, vParams);
                mOut.Model.GetCart({
                    ID: vParams.ID,
                    IncludeZeroQty: vParams.IncludeZeroQty,
                    IncludeSuppliers: mOut.ShowProductSuppliersColumn,
                    AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                    IncludeChildOrderItems: pIncludeChildOrderItems,
                    Success: function (vData) {
                        mFinalizedForApproval = CBool(vData.Master.FinalizedForApproval);
                        mUnapprovedProfitability = CBool(vData.Master.UnapprovedProfitability);
                        mHasCnCUnapprovedReason = (CStr2(vData.Master.CnCUnapprovedReason).trim() != '')
                            && (CStr2(vData.Master.CncApprovedByID) == '');
                        if (!vParams.LoadProductsOnly)
                            DisplayTrans(vData, {
                                DontReplaceCurrentID: vParams.DontReplaceCurrentID
                            });
                        DisplayProducts(vData);
                        if (vParams.AfterLoad)
                        vParams.AfterLoad(vData);
                    }
                });
            }

            function DisplayTrans(vData, vParams) {
                var pDate;
                var pStr;

                //InitInvoiceTypes();
                vParams = $.extend({
                    DontReplaceCurrentID: false
                }, vParams);
                mFromSalesOrder = IsGuid(vData.Master.SalesOrderID);
                mSavedAsFinal = CBool(vData.Master.SavedAsFinal);
                if (CBool(vData.Master.IsCancelled))
                    mDivDraftOrFinalized.text('CANCELLED');
                else if (mFinalizedForApproval)
                    mDivDraftOrFinalized.text('FINALIZED FOR APPROVAL');
                else
                    mDivDraftOrFinalized.text(mSavedAsFinal ? 'FINALIZED' : 'DRAFT');
                if (!vParams.DontReplaceCurrentID) {
                    mOut.Model.Data.ID = vData.Master.ID;
                    pProductPopup.Model.Data.OrderID = mOut.Model.Data.ID;
                    pProductPopup.Model.Data.NewCartMode = false;
                }
                mOut.Model.Data.CustomerID = vData.Master.CustomerID;
                mCustomerName = vData.Master.CustomerName;
                mTxtCustomer.val(mCustomerName);
                mOut.Model.Data.QuotationID = vData.Master.QuotationID;
                mTxtQuotationNumText.val(vData.Master.QuotationNumText);
                if (mOut.TransType == 'Quote') {
                    mTxtQuoteNumber.val(vData.Master.QuotationNo);
                    pDate = vData.Master.QuotationDate;
                    DisplayTransDate(pDate);
                } else {
                    mTxtQuoteNumber.val(vData.Master.RefQuotationNo || vData.Master.QuotationNo);
                    pDate = vData.Master.RefQuotationDate || vData.Master.QuotationDate;
                }
                pDate = ParseDate(pDate);
                if (pDate != null)
                    pDate = $.format.date(pDate, 'M/d/yyyy');
                mTxtQuoteDate.val(pDate);
                if (mOut.TransType == 'Invoice') {
                    pStr = CStr2(vData.Master.SoMIN).trim();
                    if (pStr != '')
                        pStr += '-';
                    pStr += vData.Master.RefSalesOrderNo;
                    mTxtSalesOrderNumber.val(pStr);
                    mTxtSalesOrderDate.val(vData.Master.RefSalesOrderDate);
                    DisplayTransDate(vData.Master.InvoiceDate);
                } else {
                    pStr = CStr2(vData.Master.MIN).trim();
                    if (pStr != '')
                        pStr += '-';
                    pStr += CInt2(vData.Master.SalesOrderNo);
                    mTxtSalesOrderNumber.val(pStr);
                    mTxtSalesOrderDate.val(vData.Master.SalesOrderDate);
                }
                if (mOut.TransType == 'SalesOrder')
                    DisplayTransDate(vData.Master.SalesOrderDate);

                if (CStr2(vData.Master.FowstWideInvoiceNumber).trim() != '')
                    pStr = vData.Master.FOWSTNo + '-' + vData.Master.FowstWideInvoiceNumber;
                else if (CStr2(vData.Master.MIN).trim() != '')
                    pStr = vData.Master.MIN + '-' + vData.Master.InvoiceNumber;
                else
                    pStr = vData.Master.InvoiceNumber;
                //pStr = CStr2(vData.Master.MIN).trim();
                //if (pStr != '')
                //    pStr += '-';
                //pStr += vData.Master.InvoiceNumber;
                mTxtInvoiceNumber.val(pStr);
                mTxtPaymentTerms.val(vData.Master.PaymentTerm);
                mTxtPaymentTermsText.val(vData.Master.PaymentTermsText);
                mOut.Model.Data.SalesPartnerOrgID = vData.Master.SalesPartnerOrgID;
                mTxtSalesPartner.val(vData.Master.SalesPartner);
                mOut.Model.Data.SalesRepPeopleID = vData.Master.SalesRepPeopleID;
                mTxtSalesRep.val(vData.Master.SalesRep);
                mTxtSalesRepText.val(vData.Master.SalesRepText);
                mOut.Model.Data.OrderedByID = vData.Master.OrderedByID;
                mTxtOrderedBy.val(vData.Master.OrderedByName);
                mTxtDateNeeded.val(FormatDate(vData.Master.DateNeeded, { DontDisplay12Am: true }));
                if (mOut.TransType == 'PurchaseOrder') {
                    pStr = vData.Master.MIN;
                    if (CStr2(pStr).trim() != '')
                        pStr += '-';
                    pStr += CInt2(vData.Master.PurchaseOrderNo);
                    mTxtPoNumber.val(pStr);
                    mMaxPaymentTerm = CDec2(vData.Master.PaymentTerm);
                } else {
                    mTxtPoNumber.val(vData.Master.PurchaseOrderText);
                    mMaxPaymentTerm = CDec2(vData.Master.CreditTerms);
                }
                pDate = ParseDate(vData.Master.PurchaseOrderDate);
                if (pDate != null)
                    pDate = $.format.date(pDate, 'M/d/yyyy');
                mTxtPoDate.val(pDate);
                mTxtRemarks.val(vData.Master.Remarks);
                mTxtRefInvoiceNumber.val(vData.Master.RefInvoiceNum);
                mTxtCanvass.val(vData.Master.CanvassNumber);
                mTxtSalesInvoiceRef.val(vData.Master.SalesInvoiceRef);
                mTxtOfficialReceiptRef.val(vData.Master.OfficialReceiptRef);

                mOut.Model.Data.CanvassID = vData.Master.CanvassID;
                mOut.Model.Data.BillingAddressID = vData.Master.BillingAddressID;
                mTxtBillingAddress.val(vData.Master.BillingAddress);
                mOut.Model.Data.DeliveryAddressID = vData.Master.DeliveryAddressID;
                mTxtDeliveryAddress.val(vData.Master.DeliveryAddress);
                //mCbInvoiceType.find('option[value=' + vData.Master.DocumentNameID + ']').attr('selected', true);
                mCbDeliveryMethod.val(vData.Master.DeliveryMethod);

                mOut.Model.Data.CustomerFowstID = vData.Master.CustomerFowstID;
                mTxtCustomerFowst.val(vData.Master.CustomerFowst);
                mOut.Model.Data.CustomerBinID = vData.Master.CustomerBinID;
                mTxtCustomerBin.val(vData.Master.CustomerBin);
                mElement.find('[name=divLastUpdated]').text(FormatDate(vData.Master.LastUpdated, { Format: 'MMMM d, yyyy h:mm a' }));
                mOut.Model.Data.ParentOrderID = vData.Master.ParentOrderID;
                mOut.Model.Data.InvoiceFowstID = vData.Master.InvoiceFowstID;
                mTxtInvoiceFowst.val(vData.Master.InvoiceFowstName);
                mOut.Model.Data.StockTransferID = vData.Master.StockTransferID;

                switch (mOut.TransType) {
                    case 'Quote':
                        mElement.find('[name=divTransNum]').text('Quote-' + mTxtQuoteNumber.val());
                        break;

                    case 'SalesOrder':
                        mElement.find('[name=divTransNum]').text('SO-' + mTxtSalesOrderNumber.val());
                        if (IsGuid(vData.Master.ParentOrderID))
                            mElement.find('[name=divParentOrder]').text('Derived from ' + vData.Master.ParentOrderMin).show();
                        else {
                            mElement.find('[name=divParentOrder]').hide();
                            mElement.find('[name=divChildSos]').show();
                        }

                        break;

                    case 'Invoice':
                        var pDocName = vData.Master.DocumentName;
                        if (CStr2(pDocName).trim() == '')
                            pDocName = 'SI';
                        mElement.find('[name=divTransNum]').text(pDocName + '-' + mTxtInvoiceNumber.val());
                        break;

                    case 'PurchaseOrder':
                        mElement.find('[name=divTransNum]').text('PO-' + mTxtPoNumber.val());
                        break;

                    case 'AccountsPayable':
                        pStr = CStr2(vData.Master.MIN).trim();
                        if (pStr != '')
                            pStr += '-';
                        pStr = 'AP-' + pStr + CDec2(vData.Master.AccountsPayableNum);
                        mElement.find('[name=divTransNum]').text(pStr);
                        break;

                }
                LoadAccountsDetails();
            } // END DisplayTrans

            function HandleEditingSavedAsFinal(vParams) {
                vParams = $.extend({
                    AllowFinalizedForApproval: false,
                    AllowSomeSaving: false,
                    AllowSomeSavingCallback: function () { }
                }, vParams);
                var pCondition = mSavedAsFinal;
                if (!vParams.AllowFinalizedForApproval)
                    pCondition = pCondition || mFinalizedForApproval;
                if (pCondition) {
                    if (ERP.Payment.controller.EmailReceipt)
                        require(['promise!/Scripts/sources/ERP/OrderProcessing/emailAddress/controller.js?v=' + assetversion], function() {
                            ERP.OrderProcessing.emailAddress.controller.popup({
                                OnOk: function(vEmailAddresses) {
                                    EmailReceipt(vEmailAddresses);
                                }
                            });
                    });    
                else
                    NewMessageBox('Do you want to reprint this transaction?', 'Reprint', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                CreatePdf({
                                    Template: 'PosReceipt'
                                });
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ]);

                    return false;
                }

                return true;
            }

            function EmailReceipt(vEmailAddresses) {
                var pData = {
                    InvoiceID: mOut.Model.Data.ID,
                    SendEmail: 1,
                    EmailAddresses: vEmailAddresses
                };
                    
                AjaxSend({
                    url: '/SalesMenu/EmailReceiptHtml',
                    data: pData,
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Result == 'ok')
                            NewMessageBox('Email was successfully sent', 'Email Sent');
                        else
                            NewMessageBox(vData.Result);
                    }
                });
            }

            function DisplayProducts(vData) {
                if (sessionStorage.FowstPOSView == 'Hot Buttons View' || sessionStorage.FowstPOSView == 'Hot Buttons with side cart') return;

                var pTotalServedQty = 0;
                var pTotalLineGp = 0;
                var pTotal;
                var pNumberOfLineItems = 0;
                var pNumberOfItems = 0;

                pProductTable.find('tbody tr').remove();
                for (var p = 0; p <= vData.Units.Rows.length - 1; p++) {
                    var pDR = vData.Units.Rows[p];
                    var pDrProduct = FilterByProductID(pDR.ProductID, vData, pDR.VendorID);
                    var pTR = pProductRow.clone();
                    var pUnit;
                    pTR.data('data', {
                        ID: pDR.ID,
                        ProductID: pDR.ProductID,
                        ProductName: pDrProduct.ProductName,
                        ProductNo: pDrProduct.ProductNo,
                        PriceLevelID: pDR.PriceLevelID,
                        UnitMeasure: pDR.UnitMeasure
                    });
                    pTR.find('[name=ProductNumber]').text(pDrProduct.ProductNo);
                    pTR.find('[name=ProductDescription]').text(pDrProduct.ProductName);
                    pTR.find('[name=OrderQty]').text($.format.number(CDec2(pDR.OrderedQuantity), '#,###.####'));
                    if (mOut.TransType == 'PurchaseOrder') {
                        if ((!CBool(pDR.ServedQtyIsEdited)
                            && (CDec2(pDR.Quantity) == 0)))
                            pTR.find('[name=VendorServedQty]').text('N/A');
                        else
                            pTR.find('[name=VendorServedQty]').text($.format.number(CDec2(pDR.Quantity), '#,###.####'));
                        pTR.find('[name=ServedQty]').text($.format.number(CDec2(pDR.QuantityGood), '#,###.####'));
                    } else
                        pTR.find('[name=ServedQty]').text($.format.number(CDec2(pDR.Quantity), '#,###.####'));
                    pTR.find('[name=ServedBadQty]').text($.format.number(CDec2(pDR.QuantityBad), '#,###.####'));
                    pTR.find('[name=UnitGP]').text($.format.number(CDec2(pDR.UnitGP), '#,###.00'));
                    pTR.find('[name=UnitGPM]').text($.format.number(CDec2(pDR.UnitGPM) * 100, '#,###.00') + '%');
                    pTR.find('[name=LineGP]').text($.format.number(CDec2(pDR.LineGP), '#,###.00'));
                    pTotalLineGp += CDec2(pDR.LineGP);
                    pTR.find('[name=UnitCogs]').text($.format.number(CDec2(pDR.Cost), '#,###.00'));
                    pTotalServedQty += CDec2(pDR.Quantity);
                    pUnit = pDR.UnitMeasure;
                    if (CDec2(pDR.ActualQuantity) != 1)
                        pUnit += ' (' + $.format.number(CDec2(pDR.ActualQuantity), '#,###.##') + ' ' + pDR.BaseUnit + ')';
                    pTR.find('[name=PackingSize]').text(pUnit);
                    pTR.find('[name=UnitPrice]').text(AccountingFormat(pDR.UnitPrice));
                    if ((mOut.TransType == 'SalesOrder')
                        || (mOut.TransType == 'Quote')
                        || (mOut.TransType == 'PurchaseOrder'))
                        pTR.find('[name=LineTotal]').text(AccountingFormat(pDR.OrderedAmount));
                    else
                        pTR.find('[name=LineTotal]').text(AccountingFormat(pDR.ServedAmount));
                    if (CStr2(pDR.AccountsPayablePoPrice) != '') {
                        pTR.find('[name=ApPoPrice]').text(AccountingFormat(pDR.AccountsPayablePoPrice));
                        pTR.find('[name=ApPoDate]').text(FormatDate(new Date(pDR.AccountsPayablePoDate), { Format: 'MMMM d, yyyy' }) + ' (' + pDR.AccountsPayablePoType + ')');
                    }
                    if (CStr2(pDR.LastApPrice) != '') {
                        pTR.find('[name=LastApPrice]').text(AccountingFormat(pDR.LastApPrice));
                        pTR.find('[name=LastApDate]').text(FormatDate(new Date(pDR.LastApDate), { Format: 'MMMM d, yyyy' }));
                    }
                    //pTR.find('[name=PaymentTerms]').text(pDR.TermNumericValue || vData.Master.CreditTerms || 'N/A');
                    //(pTR.click(function(vProductID) {
                    //    Item_Click(vProductID);
                    //}))(pDR.ProductID);
                    pTR.click(function (e) {
                        var pDR = $(this).data('data');
                        Item_Click(pDR.ProductID, pDR);
                    });

                    if (vData.ProductSuppliers) {
                        var pSupplierNames = '';
                        for (var pSupplierIndex = 0; pSupplierIndex <= vData.ProductSuppliers.Rows.length - 1; pSupplierIndex++) {
                            var pDrSupplier = vData.ProductSuppliers.Rows[pSupplierIndex];
                            if (ReadyGuid(pDR.ProductID) == ReadyGuid(pDrSupplier.ProductID)) {
                                if (pSupplierNames != '')
                                    pSupplierNames += ',\n ';
                                pSupplierNames += pDrSupplier.SupplierName;
                            }
                        }
                        pTR.find('[name=Supplier]').text(pSupplierNames);
                    }
                    pTR.show();
                    pProductTable.append(pTR);
                    pNumberOfLineItems++;
                    pNumberOfItems += CDec2(pDR.Quantity);
                }

                if (mOut.TransType == 'SalesOrder')
                    pTotal = CDec2(vData.Master.OrderedAmount);
                else
                    pTotal = CDec2(vData.Master.ServedAmount);
                pTotal = CDec2(pTotal);
                $('#webPosCartSubTotal').text(pTotal == 0 ? '0.00' : AccountingFormat(pTotal));
                pProductTable.find('tfoot tr td[name=LineTotal]').text(AccountingFormat(pTotal));
                ERP.Payment.controller.WebPosNetInvoiceBalance = pTotal;
                ERP.Payment.controller.WebPosInvoiceID = vData.Master.ID;
                ERP.Payment.controller.BeforePay = BeforePay;
                ERP.Payment.controller.ComputeTotalPaymentRcv();            
                $('#divNumberOfLineItems').text(AccountingFormat(pNumberOfLineItems, 0));
                $('#divNumberOfItems').text(AccountingFormat(pNumberOfItems, 0));
                mInvoiceTotal = pTotal;
                pProductTable.find('tfoot tr td[name=LineGP]').text(AccountingFormat(pTotalLineGp));
                pProductTable.find('tfoot tr td[name=UnitGPM]').text(AccountingFormat(pTotalLineGp / pTotal * 100) + '%');
                Bs.Loading('hide');
                $('[name="maincontainer"] > .table-fixed').scrollTop($('[name="maincontainer"] > .table-fixed')[0].scrollHeight);
                ShowHideColumns();
            } // END DisplayProducts           

            function ShowHideColumns() {

                if (CBool(vParams.AccountsPayable)) {
                    mElement.find('[name=butNew]').hide();
                    mElement.find('[name=butLookup]').hide();
                    mElement.find('[name=butSaveACopy]').hide();
                    mElement.find('[name=cbProductAction]').show();
                    DisplayProductAction();

                    pProductTable.find('[name="VendorServedQty"]').hide();
                    pProductTable.find('[name=thServedQty]').show();
                    pProductTable.find('[name=ServedQty]').show();
                    pProductTable.find('[name=thServedBadQty]').show();
                    pProductTable.find('[name=ServedBadQty]').show();
                    pProductTable.find('[name=thPoPrice]').text('Unit Price to be Paid');
                    pProductTable.find('[name="ApPoPrice"]').show();
                    pProductTable.find('[name="ApPoDate"]').show();
                    pProductTable.find('[name="LastApPrice"]').show();
                    pProductTable.find('[name="LastApDate"]').show();
                } else if ((mOut.TransType == 'Invoice')
                    || (mOut.TransType == 'PurchaseOrder')) {
                    mElement.find('[name=cbProductAction]').hide();
                    pProductTable.find('[name=thServedQty]').show();
                    pProductTable.find('[name=ServedQty]').show();
                    pProductTable.find('[name="ApPoPrice"]').hide();
                    pProductTable.find('[name="ApPoDate"]').hide();
                    pProductTable.find('[name="LastApPrice"]').hide();
                    pProductTable.find('[name="LastApDate"]').hide();
                } else {
                    mElement.find('[name=cbProductAction]').hide();
                    pProductTable.find('[name=thServedQty]').hide();
                    pProductTable.find('[name=ServedQty]').hide();
                    pProductTable.find('[name=thServedBadQty]').hide();
                    pProductTable.find('[name=ServedBadQty]').hide();
                    pProductTable.find('[name="ApPoPrice"]').hide();
                    pProductTable.find('[name="ApPoDate"]').hide();
                    pProductTable.find('[name="LastApPrice"]').hide();
                    pProductTable.find('[name="LastApDate"]').hide();
                }

                if (mOut.ShowProductSuppliersColumn)
                    pProductTable.find('[name=Supplier]').show();
                else
                    pProductTable.find('[name=Supplier]').hide();

                if (mOut.TransType == 'SalesOrder')
                    GetSecurityCurrentUserAcccess({
                        ModuleName: 'View Profitability Details in Sales Order',
                        Callback: function (vAllowed) {
                            if (vAllowed) {
                                pProductTable.find('.colProfit').show();
                            }
                        }
                    });
                else if (mOut.TransType == 'Invoice')
                    GetSecurityCurrentUserAcccess({
                        ModuleName: 'View Profitability Details in Sales Invoice',
                        Callback: function (vAllowed) {
                            if (vAllowed) {
                                pProductTable.find('.colProfit').show();
                            }
                        }
                    });
            }

            function FilterByProductID(vProductID, vData, vVendorID) {
                for (var p = 0; p <= vData.Stocks.Rows.length - 1; p++) {
                    var pDR = vData.Stocks.Rows[p];
                    if ((pDR.ProductID == vProductID)
                        && (ReadyGuid(pDR.VendorID) == ReadyGuid(vVendorID)))
                        return pDR;
                }
            } // END FilterByProductID

            var mTxtSalesPartner = mElement.find('[name=txtSalesPartner]');
            mTxtSalesPartner.click(SearchSalesPartner);
            mTxtSalesPartner.on('keypress', function (e) {
                if (e.key == 'Enter')
                    SearchSalesPartner();
            }); // END mElement.find('[name=txtSalesPartner]').click(function() {
            mElement.find('[name=butSalesPartner]').click(SearchSalesPartner);

            function SearchSalesPartner() {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowsePopup('LibraryOrganization', 'Sales Partner',
                    [
                        {
                            //Search: mTxtSalesPartner.val(),
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                mOut.Model.Data.SalesPartnerOrgID = e.ID;
                                mTxtSalesPartner.val(e.CompanyName);
                            }
                        }
                    ], { FilterCustomers: 1 }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            var mTxtSalesRepText = mElement.find('[name=txtSalesRepText]');
            var mTxtSalesRep = mElement.find('[name=txtSalesRep]');
            mTxtSalesRepText.on('keypress', function (e) {
                if (e.key == 'Enter')
                    SearchSalesRep();
            }); // END mElement.find('[name=txtSalesPartner]').click(function() {
            mElement.find('[name=butSearchSalesRep]').click(SearchSalesRep);

            function SearchSalesRep() {
                //if (!HandleEditingSavedAsFinal())
                //    return;
                var pOrgID;
                if (mOut.TransType == 'PurchaseOrder') {
                    if (IsGuid(mOut.Model.Data.CustomerID))
                        pOrgID = mOut.Model.Data.CustomerID;
                    else {
                        NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                        return;
                    }
                    BrowsePopup('LibraryOrganizationContactPerson', 'Sales Rep',
                        [
                            {
                                Search: mTxtSalesRepText.val(),
                                DisplayText: [{ Column: 'PeopleName' }],
                                Click: function (e) {
                                    mTxtSalesRepText.val(e.PeopleName);
                                }
                            }
                        ], {
                            OrgID: pOrgID,
                            HideNoAddress: 1
                        }
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',

                } else {
                    BrowsePopup('LibrarySalesRep', 'Sales Rep',
                        [
                            {
                                Search: mTxtSalesRepText.val(),
                                DisplayText: [{ Column: 'Name' }],
                                Click: function (e) {
                                    mTxtSalesRepText.val(e.Name);
                                }
                            }
                        ]
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
                }
            }

            var mTxtOrderedBy = mElement.find('[name=txtOrderedBy]');
            mTxtOrderedBy.click(SearchCustomerContactPerson);
            mTxtOrderedBy.on('keypress', function (e) {
                if (e.key == 'Enter')
                    SearchCustomerContactPerson();
            }); // END mTxtBillingAddress.on('keypress', function(e) {
            mElement.find('[name=butSearchCustomerContactPerson]').click(SearchCustomerContactPerson);

            mElement.find('[name=butAddCustomerContactPerson]').click(function () {
                if (!IsGuid(mOut.Model.Data.CustomerID)) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }
                mContactPerson.Show({
                    OrgID: mOut.Model.Data.CustomerID
                });
            });

            function SearchCustomerContactPerson() {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowsePopup('LibraryOrganizationContactPerson', 'Ordered By',
                    [
                        {
                            Search: mTxtOrderedBy.val(),
                            DisplayText: [{
                                Column: function (vData) {
                                    var p = vData.PeopleName;
                                    if (CStr2(vData.Position).trim() != '')
                                        p += ' - ' + vData.Position.trim();
                                    return p;
                                }
                            }],
                            Click: function (e) {
                                mOut.Model.Data.OrderedByID = e.ID;
                                var p = e.PeopleName;
                                if (CStr2(e.Position).trim() != '')
                                    p += ' - ' + e.Position.trim();
                                mTxtOrderedBy.val(p);
                            }
                        }
                    ], { OrgID: mOut.Model.Data.CustomerID }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            mElement.find('[name=butPay]').click(function () {
                NewMessageBox('This function is not yet implemented. ref: jsCTDD4VS4DR!', 'Pay');
            });

            mElement.find('[name=butNew]').click(function () {
                NewMessageBox('Are you sure to want to create a new transaction? Any unsaved data will be lost.', 'New', [
                    {
                        Text: 'Yes',
                        Click: NewTrans
                    },
                    {
                        Text: 'No'
                    }
                ]);
            });

            mOut.NewTrans = NewTrans;
            function NewTrans() {

                if (IsGuid(sessionStorage.FowstID))
                    Proceed();
                else {
                    function TryBrowsePopup() {
                        if (window.BrowsePopup == null) {
                            setTimeout(TryBrowsePopup, 100);
                            return;
                        }

                        BrowsePopup('LibraryFowst', 'Select FOWST', [
                            {
                                DisplayText: [{ Column: 'FowstName' }],
                                AutoSearch: true,
                                Click: function (e) {
                                    sessionStorage.FowstPOSView = e.DefaultPosView;
                                    sessionStorage.FowstName = e.FowstName;
                                    sessionStorage.FowstID = e.ID;
                                    $('#lblFowstName').text(sessionStorage.FowstName);
                                    Proceed();
                                }
                            }
                        ], {
                            FilterFowst: 1
                        });    
                    }
                    TryBrowsePopup();
                }

                function Proceed() {
                    if (ERP.Payment != null)
                        ERP.Payment.controller.ClearValues();
                    mFromSalesOrder = false;
                    mFinalizedForApproval = false;
                    mHasCnCUnapprovedReason = false;
                    mUnapprovedProfitability = false;
                    mSavedAsFinal = false;
                    mDivDraftOrFinalized.text('DRAFT');
                    mOut.Model = new ERP.OrderProcessing.work.model();
                    mOut.Model.Data.TransType = mOut.TransType;
                    mTxtCustomer.val('');
                    mTxtQuotationNumText.val('');
                    mTxtQuoteNumber.val('');
                    mTxtQuoteDate.val('');
                    mTxtSalesOrderNumber.val('');
                    mTxtSalesOrderDate.val('');
                    mTxtInvoiceNumber.val('');
                    mTxtPaymentTerms.val('');
                    mTxtPaymentTermsText.val('');
                    mTxtSalesPartner.val('');
                    mTxtSalesRep.val('');
                    mTxtSalesRepText.val('');
                    mTxtDateNeeded.val('');
                    mTxtBillingAddress.val('');
                    mTxtDeliveryAddress.val('');
                    mTxtRemarks.val('');
                    mTxtPoNumber.val('');
                    mTxtPoDate.val(FormatDate(new Date()));
                    mTxtRefInvoiceNumber.val('');
                    mTxtCanvass.val('');
                    pProductTable.find('tbody tr').remove();
                    pProductTable.find('tfoot tr td[name=LineTotal]').text('');
                    if (mOut.TransType == 'PurchaseOrder') {
                        mOut.Model.Data.BillingAddressID = sessionStorage.FowstID;
                        mOut.Model.Data.DeliveryAddressID = sessionStorage.FowstID;
                        mTxtBillingAddress.val(sessionStorage.FowstAddress);
                        mTxtDeliveryAddress.val(sessionStorage.FowstAddress);
                    }
                    mTxtCustomerFowst.val('');
                    mTxtCustomerBin.val('');
                    mElement.find('[name=divTransNum]').text('');
                    mElement.find('[name=divLastUpdated]').text('');
                    mElement.find('[name=divParentOrder]').hide();
                    mElement.find('[name=divChildSos]').hide();
                    mCbDeliveryMethod.val('');
                    mTxtOrderedBy.val('');
                    mOut.Model.Data.InvoiceFowstID = sessionStorage.FowstID;
                    mTxtInvoiceFowst.val(sessionStorage.FowstName);
                    DisplayTransDate();
                    mElement.find('tfoot tr td[name=LineTotal]').text('');
                    mElement.find('tfoot tr td[name=LineGP]').text('');
                    mElement.find('tfoot tr td[name=UnitGPM]').text('');

                    $('#webPosCartSubTotal, #webPosCartVATPercent, #webPosCartTotalVATExAmount, #webPosCartTotalDiscount, #lblVATBackPercent, #lblSalesPayCharges, #webPosCartTotal, #lblAmountLacking, #lblTotalPaymentRcv, #lblPaymentChange').text('0.00');
                    $('#divNumberOfLineItems, #divNumberOfItems').text('0');

                    $('#btnPaymentCash, #btnPaymentCheque, #btnPaymentDCCard, #btnPaymentSrv, #btnPaymentOnCredit, #btnPaymentOther, #btnPaymentDiscounts, #btnPaymentCharges, #btnPaymentRewards').removeClass('active');
                    $('.btnpaymentmethod > b').text('');
                    $('#txtPunchItem').val('').focus();

                    if (sessionStorage.FowstPOSView == 'Hot Buttons View' || sessionStorage.FowstPOSView == 'Hot Buttons with side cart') {
                        mElement.find('[name="butHotButtons"]').click();
                    } else {
                        mElement.find('[name="divRetail"]').show();
                        mElement.find('[name="divLine"]').show();
                        mElement.find('#formPunchItem').parent().show();
                        mElement.find('[name="divHotButtons"], [name="divPOSViewLoading"]').hide();
                    }

                    AjaxSend({
                        url: '/DashboardsAndReports/GetOrganizationName',
                        success: function (vData) {
                            if (vData.Results.Rows.length > 0) {
                                sessionStorage["OrgName"] = vData.Results.Rows[0].CompanyName;
                                $('#lblCompanyName').text(vData.Results.Rows[0].CompanyName);
                            }

                            AjaxSend({
                                url: '/SalesMenu/getOrgFOWSTPrinterType',
                                success: function (vData) {
                                    if (vData.Results.Rows.length > 0) {
                                        mOut.isSaveReceipt = false;
                                        mElement.find('[name="butPaymentSaveAndPrint"]').text('PRINT RECEIPT');
                                    } else {
                                        AjaxSend({
                                            url: '/WebPosDbDownloader/GetWebPosSettings',
                                            success: function(vWebPosSettings) {
                                                if (vWebPosSettings.Error) {
                                                    return;
                                                }
                                                mOut.isSaveReceipt = CInt2(vWebPosSettings.AfterSaving) == 1

                                                if (mOut.isSaveReceipt)
                                                    mElement.find('[name="butPaymentSaveAndPrint"]').text('SAVE RECEIPT');
                                                else
                                                    mElement.find('[name="butPaymentSaveAndPrint"]').text('PRINT RECEIPT');
                                            }
                                        });
                                    }
                                }
                            })
                        }
                    })                    

                    AjaxSend({
                        url: '/SalesMenu/GetCustomer',
                        data: {
                            GetOrAddDefaultAddresses: 1,
                            IncludeZRead: 1,
                            //CreateCacheDatabaseIfNeeded: 1,
                            GetRemoteServer: 1
                        },
                        success: function(vData) {
                            mCustomerName = vData.CompanyName;
                            mTxtCustomer.val(vData.CompanyName);
                            $('#lblCustomerName').text(mCustomerName);
                            sessionStorage.CustomerOrganizationID = vData.ID;
                            mOut.Model.Data.CustomerID = vData.ID;
                            mTxtDeliveryAddress.val(vData.DefaultDeliveryAddressName);
                            mOut.Model.Data.DeliveryAddressID = vData.DefaultDeliveryAddressID;
                            mFowstGroupPriceName = CStr2(vData.FowstGroupPriceName);
                            mTxtBillingAddress.val(vData.DefaultBillingAddressName);
                            mOut.Model.Data.BillingAddressID = vData.DefaultBillingAddressID;
                            //LoadAccountsDetails();

                            if (vData.ZReadDate)
                                vData.ZReadDate = new Date(vData.ZReadDate);
                            if (vData.ServerTime) {
                                vData.ServerTime = new Date(vData.ServerTime);
                                mLocalTime = new Date();
                                mServerTime = vData.ServerTime;
                            }
                            $('#bPosPermit').text(CStr2(vData.POSRegistrationNo));
                            $('#bPosSerial').text(CStr2(localStorage.UUID));
                            mEnforceZRead = CBool(vData.EnforceZRead);
                            mRemoteServer = CStr2(vData.RemoteServer);
                            function WaitForNewMessageBox() {
                                if (window.NewMessageBox) {
                                    if (vData.ZReadForwardable)
                                        PromptZRead(vData.ZReadDate, vData.ServerTime);
                                    $('#txtPunchItem').attr('placeholder', 'Punch Item Here');
                                } else
                                    setTimeout(WaitForNewMessageBox, 200);
                            }

                            WaitForNewMessageBox();
                        }
                    });
                    mTxtDateNeeded.val(FormatDate(new Date(), { Format: 'MMM d, yyyy h:mm a' }));
                } // END function Proceed() {
            }

            function PromptZRead(vZReadDate, vServerTime) {
                
                proj.SetZReadDate(vZReadDate);
                var pStr = 'Server date is ' + $.format.date(vServerTime, 'MMM d, yyyy') + ' but the Z-Read date is ' + $.format.date(vZReadDate, 'MMM d, yyyy');
                NewMessageBox(pStr, 'Closing Date', [
                    {
                        Text: 'Close the current Z-Read Date and move date forward by 1 day',
                        Click: function() {
                            NewMessageBox('You will no longer be able to post sales and other transactions to the current Z-Read Date after closing.', '', [
                                {
                                    Text: 'Yes',
                                    Click: function() {
                                        AjaxSend({
                                            url: '/SalesMenu/IncrementZReadDate',
                                            success: function(vData) {
                                                if (vData.Error)
                                                    NewMessageBox(vData.Error, 'Failed to forward the Z-Read date', null, {
                                                        Close: function() {
                                                            PromptZRead(vZReadDate, vServerTime);
                                                        }
                                                    });
                                                else if (vData.Result == 'ok') {
                                                    proj.SetZReadDate(vData.ZReadDate);
                                                    if (vData.ZReadForwardable)
                                                        PromptZRead(new Date(vData.ZReadDate), new Date(vData.ServerTime));
                                                }
                                            }
                                        });
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function() {
                                        PromptZRead(vZReadDate, vServerTime);
                                    }
                                }
                            ]);

                        }
                    },
                    {
                        Text: 'Use ' + $.format.date(vZReadDate, 'MMM d, yyyy') + ' as the current date',
                        Click: function() {
                            // just do nothing
                            // proj.SetZReadDate(vZReadDate);
                        } 
                    },
                    {
                        Text: 'Print report for ' + $.format.date(vZReadDate, 'MMM d, yyyy'),
                        Click: function () {
                            NewMessageBox('This feature is not yet implemented. jsM536VBPMNP!', 'Under construction');
                            return;

                            PopupButtonList('Select which report to print', 'POS Sales Reports', [
                                {
                                    Text: 'Cashiers Sales Report',
                                    Click: function () {                                                    

                                        var pData = {};
                                        var isPageReady = false;
                                        pData.OrgID = sessionStorage["OrgID"];
                                        pData.POSSalesReportType = 0;

                                        sessionStorage.POSSalesReportType = 0;

                                        AjaxSend({
                                            url: 'ReportsPOS/setPOSSalesReportType',
                                            data: pData,
                                            success: function (vData) {
                                                require([
                                                    GetPathFormat('promise!/Scripts/sources/ReportsPOS/SalesReport.js?crc=')
                                                ], function () {
                                                    var pData = {};

                                                    pData.RangeType = 'Day';
                                                    pData.RangeFrom = new Date(proj.GetZReadDateSql());
                                                    pData.RangeTo = new Date(proj.GetZReadDateSql());
                                                    pData.RangeYear = 'Day';

                                                    sessionStorage['ReportDate'] = Date(proj.GetZReadDateSql());
                                                    getSalesReportData(pData, Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()));
                                                });
                                            }
                                        });
                                    }
                                },
                                {
                                    Text: 'Managers Sales Report',
                                    Click: function () {
                                               
                                        var pData = {};
                                        var isPageReady = false;
                                        pData.OrgID = sessionStorage["OrgID"];
                                        pData.POSSalesReportType = 1;

                                        sessionStorage.POSSalesReportType = 1;

                                        AjaxSend({
                                            url: 'ReportsPOS/setPOSSalesReportType',
                                            data: pData,
                                            success: function (vData) {
                                                require([
                                                    GetPathFormat('promise!/Scripts/sources/ReportsPOS/SalesReport.js?crc=')
                                                ], function () {
                                                    var pData = {};

                                                    pData.RangeType = 'Day';
                                                    pData.RangeFrom = new Date(proj.GetZReadDateSql());
                                                    pData.RangeTo = new Date(proj.GetZReadDateSql());
                                                    pData.RangeYear = 'Day';

                                                    sessionStorage['ReportDate'] = Date(proj.GetZReadDateSql());
                                                    getSalesReportData(pData, Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()));
                                                });
                                            }
                                        })
                                                    
                                    }
                                },
                                {
                                    Text: 'Z-Read',
                                    Click: function () {
                                        //    sessionStorage.PrintingOfflineData = true;
                                        //    sessionStorage.OfflineType = 'zreport';
                                        //    sessionStorage.removeItem('IsRequestAndApproval');
                                        //    location = MergeUrlParameters({
                                        //        OrgID: sessionStorage.OrgID,
                                        //        IsMobile: CBool(IsMobile) ? 1 : 0
                                        //    }, '/DashBoardsAndReports');
                                        //}
                                        GetZreadReportPage();
                                        return;
                                        require([
                                                GetPathFormat('promise!/Scripts/sources/ReportsPOS/ZReport.js?v=' + assetversion)
                                        ], function () {
                                            sessionStorage.PrintingOfflineData = true; //para sa offline database sya magkuha ug zread data just incase naa pa unflushed data sa device
                                            PrintSalesZReport(false);
                                            sessionStorage.PrintingOfflineData = false; 
                                        });
                                    }
                                }
                            ])
                        }
                    }
                ]);
            } // END function PromptZRead() {

            var htmlzreport = '';
            function GetZreadReportPage() {
                Bs.Loading('show');

                if (CStr2(htmlzreport) == '') {
                    AjaxSend({
                        ExactUrl: '/ReportsPOS/ZReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlzreport = vData;
                            $('#popReports .modal-title').text('Z-Read Report');
                            $('#popReports .modal-body').append(vData);
                            $('#divZReportView > div').removeAttr('class').addClass('col');
                            $('#popReports').modal('show');
                        }
                    });
                } else {
                    proj.ViewZReport();
                    $('#popReports').modal('show');
                }                
            }

            function AfterProductSave(vParams) {
                vParams = $.extend({
                    BelowGpmThreshold: false
                }, vParams);
                if (vParams.BelowGpmThreshold) {
                    var pMsg = 'Some product(s) have a gross profit margin that is below the minimum threshold set by the company.<br /><br /> You will still be able to save this transaction but profitability approval needs to be made by an authorized person before this transaction can be processed further.<br /><br /> Please get in touch with your company’s authorized person for approvals to expedite this transaction.';
                    NewMessageBox(pMsg, 'Gross Profitability Warning');
                }
                mOut.Model.Data.ID = pProductPopup.Model.Data.OrderID;
                LoadTrans({
                    ID: mOut.Model.Data.ID,
                    LoadProductsOnly: true
                });                
            }
            mOut.AfterProductSave = AfterProductSave;

            mElement.find('[name=butAddBillingAddress]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (mOut.Model.Data.CustomerID == null) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }
                if (mTxtBillingAddress.val().trim() == '') {
                    NewMessageBox('Please enter the address first.');
                    return;
                }
                NewMessageBox('Are you sure you want to add the address, ' + mTxtBillingAddress.val().trim() + ', to ' + mTargetOrgType + ', ' + mTxtCustomer.val() + '?', 'Add Address', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            var pCustomerID;
                            if (mOut.TransType == 'PurchaseOrder') {
                                pCustomerID = sessionStorage.OrgID;
                            } else
                                pCustomerID = mOut.Model.Data.CustomerID;
                            AddAddress({
                                CustomerID: pCustomerID,
                                Address: mTxtBillingAddress.val().substring(0, 250),
                                Success: function (vData) {
                                    mOut.Model.Data.BillingAddressID = vData.ID;
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function () {

                        }
                    }
                ]);
            });

            function AddAddress(vParams) {

                mOut.Model.AddAddress({
                    Address: vParams.Address,
                    CustomerID: vParams.CustomerID,
                    Success: function (vData) {
                        NewMessageBox('Address was successfully added.');
                        vParams.Success(vData);
                    },
                    Fail: function (vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            }

            mElement.find('[name=butAddDeliveryAddress]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                if (mOut.Model.Data.CustomerID == null) {
                    NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                    return;
                }
                if (mTxtDeliveryAddress.val().trim() == '') {
                    NewMessageBox('Please enter the address first.');
                    return;
                }
                NewMessageBox('Are you sure you want to add the address, ' + mTxtDeliveryAddress.val().trim() + ', to ' + mTargetOrgType + ', ' + mTxtCustomer.val() + '?', 'Add Address', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            var pCustomerID;
                            if (mOut.TransType == 'PurchaseOrder') {
                                pCustomerID = sessionStorage.OrgID;
                            } else
                                pCustomerID = mOut.Model.Data.CustomerID;
                            AddAddress({
                                CustomerID: pCustomerID,
                                Address: mTxtDeliveryAddress.val().substring(0, 250),
                                Success: function (vData) {
                                    mOut.Model.Data.DeliveryAddressID = vData.ID;
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function () {

                        }
                    }
                ]);
            });

            mElement.find('[name=butOutput]').click(HandlePrint);

            function HandlePrint(vParams) {
                PopupButtonList('', 'Select which POS report to print', [
                    {
                        Text: 'Cashier\'s Sales Report (CSR)', Click: function () {
                            window.open('/DashboardsAndReports?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&View=CSR', '_blank');
                        }
                    }, {
                        Text: 'Manager\'s Sales Report (MSR)', Click: function () {
                            window.open('/DashboardsAndReports?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&View=MSR', '_blank');
                        }
                    }, {
                        Text: 'Z-Read Report', Click: function () {
                            GetZreadReportPage();
                            return;
                            window.open('/DashboardsAndReports?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&View=ZRead', '_blank');
                        }
                    }
                ])
                return
                vParams = $.extend({
                    NewTransAfterPrint: false
                }, vParams);
                var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
                if (pInvoiceID == null) {
                    NewMessageBox('Please select a transaction first.');
                    return;
                }

                LoadTrans({
                    ID: pInvoiceID,
                AfterLoad: function(vData) {
                        var pMsg = '';

                        if (mFinalizedForApproval) {
                            if (mUnapprovedProfitability) {
                                pMsg = 'This transaction is saved as final but profitability approval needs to be made by an authorized person before this transaction can be printed.<br /><br /> Please get in touch with your company’s authorized person for approvals to expedite this transaction.';
                            }

                            if (mHasCnCUnapprovedReason) {
                                if (pMsg != '')
                                    pMsg += '<br /><br />';

                                pMsg += 'This transaction is saved as final but is awaiting credit and collection approval due to credit limit and/or credit terms. A printed invoice cannot be generated before approval is made. Please coordinate with your credit and collection officer for approval.';
                            }

                            if (pMsg != '') {
                                NewMessageBox(pMsg, 'Saved but Awaiting Approval');
                                return;
                            }
                        }

                        if (!mSavedAsFinal) {
                            NewMessageBox('Only transactions that were saved as final can be printed.');
                            return;
                        }

                        if (mHasCnCUnapprovedReason
                            && (mOut.TransType == 'SalesOrder')) {

                            NewMessageBox('This transaction is awaiting approval from credit and collection and may not be prepared and/or invoiced prior to said approval.');
                            return;
                        }

                        var pMessageButtons = [];

                        switch (mOut.TransType) {
                            case 'SalesOrder':
                                pMessageButtons.push({
                                    Text: 'Sales Order',
                                    Click: function () {
                                        CreatePdf();
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Pick List',
                                    Click: function () {
                                        CreatePdf({
                                            Template: 'PickList'
                                        });
                                    }
                                });
                                break;

                            case 'Invoice':
                            if (CStr2(vData.Master.TransType).toLowerCase() == 'invoice') {
                                pMessageButtons.push({
                                    Text: 'Invoice',
                                    Click: function () {
                                        CreatePdf();
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Pick List',
                                    Click: function () {
                                        CreatePdf({
                                            Template: 'PickList'
                                        });
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Warehouse Releasing Slip',
                                    Click: function () {
                                        CreatePdf({
                                            Template: 'WithdrawalSlip'
                                        });
                                    }
                                });
                            } else
                                pMessageButtons.push({
                                    Text: 'POS Receipt',
                                    Click: function() {
                                        //PrintPosReceipt();
                                        CreatePdf({
                                            Template: 'PosReceipt'
                                        });
                                    }
                                });
                                break;

                            case 'PurchaseOrder':
                                pMessageButtons.push({
                                    Text: 'Purchase Order',
                                    Attr: { 'class': 'btn custom-btn-theme-c' },
                                    Click: function () {
                                        CreatePdf();
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'No-Price Purchase Order',
                                    Click: function () {
                                        CreatePdf({
                                            Template: 'No-Price Purchase Order'
                                        });
                                    }
                                });

                                break;

                            case 'AccountsPayable':
                                var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
                                AjaxSend({
                                    url: '/Receiving/CreatePdf',
                                    data: {
                                        AccountsPayableID: pInvoiceID,
                                        PrintHash: localStorage.getItem('PrintHash'),
                                        Template: 'StockReceivingAndPO'
                                    },
                                    success: function (vData) {
                                        if (vData.Error) {
                                            if (vData.Error.indexOf('PrintHash') == -1)
                                                NewMessageBox(vData.Error);
                                            else
                                                NoPrintHash();
                                        } else if (vData.Result == 'ok')
                                            NewMessageBox('Your document will be printed by your local print server.');

                                    }
                                });
                                return;
                        }

                        if (pMessageButtons.length > 0) {
                            NewMessageBox('What do you want to print?', 'Print', pMessageButtons, {
                                SameColorButtons: true
                            });
                            return;
                        }

                        CreatePdf();
                    } // END AfterLoad: function() {
                }); // END LoadTrans({
            } // END function HandlePrint(vParams) {

            function PrintPosReceipt() {
                var pPrintWindow = window.open();

                if (pPrintWindow == null || typeof (pPrintWindow) == 'undefined') {
                    alert('Please allow popup for this site and click the "Print" button again.');
                    return;
                }
                var pInvoiceID = GetStringObject('Model.Data.ID', mOut);

                AjaxSend({
                    url: '/SalesMenu/EmailReceiptHtml',
                    data: {
                        InvoiceID: pInvoiceID
                    },
                    success: function (vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else {
                            pPrintWindow.document.write(vData.Result);
                            setTimeout(function () {
                                pPrintWindow.print();
                            }, 1000);
                        }
                    }
                });
            }

            function CreatePdf(vParams) {
                vParams = $.extend({
                    Template: null
                }, vParams);

                var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
                var pData = {
                    InvoiceID: pInvoiceID,
                    PrintHash: localStorage.getItem('PrintHash'),
                    TransType: mOut.TransType,
                    Template: vParams.Template
                };
                if (CBool(vParams.AllowReprint))
                    pData.AllowReprint = 1;
                AjaxSend({
                    url: '/ERP/CreatePdf',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            if (vData.Error.indexOf('PrintHash') == -1) {
                                if (vData.Error == 'UUID is required.')
                                    NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                        'This is by design to enforce document print controls. <br /><br />\n\n' +
                                        'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                        'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                        [
                                            {
                                                Text: 'Download Mobiz Controller',
                                                Click: function () {
                                                    window.open('/Content/PrintApplet.zip', '_blank');
                                                }
                                            },
                                            {
                                                Text: 'Cancel'
                                            }
                                        ]);
                                else if (CStr2(vData.Error).indexOf('Transaction was already printed.') != -1)
                                    PromptUserAccess('POS Receipt Reprint', {
                                        OnClickOKSuccess: function () {
                                            NewMessageBox('Preparing to reprint.');
                                            vParams.AllowReprint = 1;
                                            CreatePdf(vParams);
                                        },
                                        OnFailed: function () {
                                            if (CBool(vData.HasUnapprovedReprintRequests))
                                                NewMessageBox('This document has an unapproved reprint request. Have your supervisor approve that request before you can print.');
                                            else
                                                require(['promise!/Scripts/sources/ERP/OrderProcessing/reprintRequest/controller.js?v=' + assetversion] , function() {
                                                    ERP.OrderProcessing.reprintRequest.controller.popup({ID: pData.InvoiceID});
                                                });
                                            //NewMessageBox('Sorry, you don\'t have access to reprint POS receipts', '');
                                        }
                                    });
                                else
                                    NewMessageBox(vData.Error);
                            } else
                                NoPrintHash();
                        } else if (vData.Result == 'ok')
                            NewMessageBox('Your document will be printed by your local print server.', '', null, {
                                Close: function () {
                                    if (vParams.NewTransAfterPrint)
                                        NewTrans();
                                }
                            });
                    }
                });
            }

            mElement.find('[name=butActivityTracker]').click(function () {
                require(['promise!/Scripts/sources/ERP/ActivityTracker/controller.js?v=' + assetversion], function () {
                    ERP.ActivityTracker.controller.popup({
                        ID: mOut.Model.Data.ID,
                        TransType: mOut.Model.Data.TransType
                    });
                });
            });

            mElement.find('[name=butCancel]').click(function () {
                var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
                if (pInvoiceID == null) {
                    NewMessageBox('Please select a transaction first.');
                    return;
                }
                PromptUserAccess('Cancel Invoice (EAS)', {
                    OnClickOKSuccess: function () {
                        Prompt();
                    },
                    OnFailed: function () {
                    }
                });

                function Prompt() {
                    NewMessageBox('Are you sure you want to cancel this transaction?', 'Cancel Transaction', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                Proceed();
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                            }
                        }
                    ]);
                }

                function Proceed() {
                    AjaxSend({
                        url: '/ERP/CancelInvoice',
                        data: {
                            InvoiceID: pInvoiceID
                        },
                        success: function (vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error);
                            } else if (vData.Result == 'ok') {
                                NewMessageBox('Transaction was successfully cancelled.');
                                NewTrans();
                            }

                        }
                    });
                }

            });

            function BeforePay() {
                //if (!mSavedAsFinal) {
                //    NewMessageBox('Please save and finalize this invoice before paying.', 'Not Finalized');
                //    return;
                //}

                var pData = {
                    InvoiceID: mOut.Model.Data.ID
                };

                pData.CustomerID = mOut.Model.Data.CustomerID;
                ERP.Payment.controller.Mode = mOut.Mode;
                ERP.Payment.controller.FromEcommerce = mOut.FromEcommerce;
                //ERP.Payment.controller.ClearValues();
                ERP.Payment.controller.WebPosInvoiceID = mOut.Model.Data.ID
                ERP.Payment.controller.DisplayPaymentInvoices({
                    CustomerID: mOut.Model.Data.CustomerID,
                    Results: {
                        Rows: [
                            {
                                ID: mOut.Model.Data.ID,
                                NetInvoiceBalance: mInvoiceTotal
                            }
                        ]
                    }
                });
                ERP.Payment.controller.BeforeSave = function(vParams) {
                    if (HandleEditingSavedAsFinal())
                        vParams.Success();
                };
                ERP.Payment.controller.NewTrans = function() {
                    NewTrans();
                    $('#divMainWebPOS').pageshow();
                    $(window).scrollTop(0);
                }
            }

            mElement.find('[name=btnBackToPOS]').click(function () {
                $('#divMainWebPOS').pageshow();
                $(window).scrollTop(0);
            })

            mElement.find('[name=butSalesReturn]').click(function () {
                location.href = '/SalesReturns?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&source=webpos';

                // BEGIN: delete this commented code after 1 year 2022_06_18__10_17_11_1196
                //else
                //    NewMessageBox('Select the location of your invoice.', 'Invoice Location', [
                //        {
                //            Text: 'Local server. Select this location first.',
                //            Click: function() {
                //                location.href = '/SalesReturns?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&source=webpos';
                //            }
                //        },
                //        {
                //            Text: 'Remote server. Select this location if you can\'t find the invoice in the local server. Requires Internet.',
                //            Click: function() {
                //                var pUrl = MergeUrlParameters({
                //                    IsMobile: 0,
                //                    OrgID: sessionStorage.OrgID,
                //                    UUID: localStorage.UUID,
                //                    Goto: 'SalesReturns'
                //                }, mRemoteServer + '/SignIn');
                //                location.href = pUrl;
                //            }
                //        }
                //    ]);
                // END: delete this commented code after 1 year 2022_06_18__10_17_11_1196
            })

            mElement.find('[name=butMoreOptions]').click(function() {
                PopupButtonList('', 'More Options', [
                    {
                        Text: 'Current FOWST Location: ' + sessionStorage.FowstName,
                        Description: 'Click to change',
                        Click: function() {
                            BrowsePopup('LibraryFowst', 'Select FOWST', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }],
                                    AutoSearch: true,
                                    Click: function (e) {
                                        sessionStorage.FowstPOSView = e.DefaultPosView;
                                        sessionStorage.FowstName = e.FowstName;
                                        sessionStorage.FowstID = e.ID;
                                        $('#lblFowstName').text(sessionStorage.FowstName);
                                        WebPOS.NewTrans();
                                    }
                                }
                            ], {
                                FilterFowst: 1
                            })
                        }
                    },
                    {
                        Text: 'Save Transaction and Email Receipt',
                        Click: function() {
                            ERP.Payment.controller.EmailReceipt = true;
                            ERP.Payment.controller.PreSavePayment({
                                Success: function(vData) {
                                    WebPOS.NewTrans();
                                    NewMessageBox('SI ' + vData.InvoiceMin + ' was successfully paid.', 'Paid');
                                }
                            });
                            //$('#butPaymentSaveAndEmail').click();
                        }
                    },
                    {
                        Text: 'Save Transaction Only',
                        Click: function() {
                            ERP.Payment.controller.EmailReceipt = false;
                            ERP.Payment.controller.PreSavePayment({
                                Success: function (vData) {
                                    WebPOS.NewTrans();
                                    NewMessageBox('SI ' + vData.InvoiceMin + ' was successfully paid.', 'Paid');
                                }
                            });
                        }
                    },
                    // temporary comment 2021_11_19__09_56_50_0213
                    {
                        Text: 'Show Flushing',
                        Click: function() {
                            window.open("/WebPosFlush", "_blank");
                        }
                    },
                    {
                        Text: 'Synchronize Database',
                        Click: function () {
                            NewMessageBox('Are you sure you want to synchronize database?', 'Data Sync', [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        var pUrl = MergeUrlParameters({
                                            IsMobile: 0,
                                            OrgID: sessionStorage.OrgID
                                        }, '/SignIn/DownloadDatabase');
                                        window.open(pUrl, "_blank");
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                            
                        }
                    },
                    // temporary comment 2021_11_19__09_56_50_0213
                    {
                        Text: 'Reconstruct Sales Data',
                        Click: function () {
                            location.href = location.href + '&rsd=1'
                        }
                    },
                ]);
            });

            function NoPrintHash() {
                var p = GetUrlParameters();
                if (CBool(p.IsReadOnly))
                    return;
                if (InAndroid)
                    return;
                var pMsg = 'Your Mobiz EAS (Enterprise Application System) needs to download the Mobiz Web Print Application (WPA) to securely print sensitive documents. This is a one-time download and installation process. After the initial installation of Mobiz WPA, you should be able to print documents from your Mobiz EAS quickly and easily.\n\n' +
                    'Would you like to download and install Mobiz Web Print Application?';
                NewMessageBox(pMsg, '', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            window.open('/Content/PrintApplet.zip', '_blank');
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);

            }

            if (CStr2(localStorage.getItem('UUID')).length < 1)
                var clear = setInterval(function () {
                    if (typeof NewMessageBox != 'undefined') {
                        NoPrintHash();
                        //InitInvoiceTypes();
                        clearInterval(clear);
                    }
                }, 200)

            //var mCbInvoiceType = mElement.find('[name=cbInvoiceType]');
            //function InitInvoiceTypes() {
            //    if (mInvoiceTypes == null)
            //        mOut.Model.GetInvoiceTypes({
            //            success: function (vData) {
            //                mInvoiceTypes = vData.Results.Rows;
            //                DisplayInvoiceTypes();
            //            }
            //        });
            //    else
            //        DisplayInvoiceTypes();

            //    function DisplayInvoiceTypes() {
            //        var pOption;
            //        var pDR;
            //        mCbInvoiceType.html('');
            //        for (var p = 0; p <= mInvoiceTypes.length - 1; p++) {
            //            pDR = mInvoiceTypes[p];
            //            if (IsGuid(mOut.Model.Data.StockTransferID)) {
            //                if (pDR.DocumentName != 'STI')
            //                    continue;
            //            } else {
            //                if (pDR.DocumentName == 'STI')
            //                    continue;
            //            }
            //            pOption = new Option(pDR.DocumentName, pDR.ID);
            //            mCbInvoiceType.append(pOption);
            //        }
            //    }
            //}

            function DisplayProductAction() {
                var pOption;
                var pDR;
                var pCB = mElement.find('[name=cbProductAction]');
                if (pCB.children().length > 0)
                    return;
                pCB.html('');

                var pOption = new Option('Unit Price To Be Paid');
                pCB.append(pOption);

                pOption = new Option('Last Purchase Order (PO) Price', 'LastPurchaseOrderPrice');
                pCB.append(pOption);

                pOption = new Option('Last Accounts Payable (AP) Price', 'LastAccountsPayablePrice');
                pCB.append(pOption);

                pOption = new Option('AP or PO Price, Whichever Is Lower', 'ApOrPoWhicheverIsLower');
                pCB.append(pOption);

                pOption = new Option('AP or PO Price, Whichever Is Newer', 'ApOrPoWhicheverIsNewer');
                pCB.append(pOption);

                pCB.change(function () {
                    var pVal = CStr2(pCB.val());
                    pCB.val('');
                    if (pVal == '')
                        return;
                    if (!HandleEditingSavedAsFinal())
                        return;
                    if (IsGuid(mOut.Model.Data.ID) == false) {
                        NewMessageBox('Please save this transaction first. ref: js396Y6YFTEW!');
                        return;
                    }
                    AjaxSend({
                        url: 'ERP/SetLineItemPrice',
                        data: {
                            InvoiceID: mOut.Model.Data.ID,
                            VendorID: mOut.Model.Data.CustomerID,
                            SourceMethod: pVal
                        },
                        success: function (vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error, 'Failed To Saved');
                            else if (vData.Result == 'ok') {
                                NewMessageBox('Prices were successfully saved.', 'Success');
                                LoadTrans({
                                    ID: mOut.Model.Data.ID,
                                    LoadProductsOnly: true,
                                    IncludeZeroQty: true
                                });
                            } else
                                NewMessageBox('Done');
                        }
                    });
                });
            }

            var mCbDeliveryMethod = mElement.find('[name=cbDeliveryMethod]');
            function InitDeliveryMethods() {
                var pOption;

                mCbDeliveryMethod.html('');
                pOption = new Option(''); mCbDeliveryMethod.append(pOption);
                pOption = new Option('Pick Up by Customer'); mCbDeliveryMethod.append(pOption);
                pOption = new Option('Pick Up by Internal Logistics'); mCbDeliveryMethod.append(pOption);
                pOption = new Option('Pick Up by 3rd Party Logistics'); mCbDeliveryMethod.append(pOption);
                pOption = new Option('Deliver to Source of Original Order'); mCbDeliveryMethod.append(pOption);
                pOption = new Option('Deliver to Customer'); mCbDeliveryMethod.append(pOption);
            }

            var mTxtInvoiceFowst = mElement.find('[name=txtInvoiceFowst]');
            mElement.find('[name=butInvoiceFowst]').click(function () {
                SelectInvoiceFowst();
            });

            function SelectInvoiceFowst() {
                BrowsePopup('LibraryFowst', 'FOWST',
                    [
                        {
                            AutoSearch: true,
                            Search: mTxtInvoiceFowst.val(),
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                mOut.Model.Data.InvoiceFowstID = e.ID;
                                mTxtInvoiceFowst.val(e.FowstName);
                            }
                        }
                    ], {
                        FilterFowst: 1
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            mOut.Initialize = function () {
                if (CBool(GetUrlParameters().rsd)) {
                    $('[name="divReconstructData"]').show();
                    $('[name="divReconstructData"]').find('a').click(function () {
                        var params = GetUrlParameters();
                        delete params.rsd;
                        location.href = MergeUrlParameters(params, location.pathname);
                    })
                } else {
                    $('[name="divReconstructData"]').remove();
                }

                if (mOut.TransType == 'Quote') {
                    mElement.find('[name=divSalesOrderNumber]').hide();
                    mElement.find('[name=divPoNumber]').hide();
                    mTxtQuoteNumber.attr('readonly', 'readonly')
                        .attr('placeholder', 'To be generated after saving as final');
                    mElement.find('[name=butSaveACopy]').show();
                    $('[custom-tab-content=#tabOPDashboard]').text('Quotation Dashboard');
                    $('[custom-tab-content=#tabOPWork]').text('Quotation');
                    $('[custom-tab-content=#tabOPSettings]').text('Quotation Settings');
                } else if (mOut.TransType == 'SalesOrder') {
                    mElement.find('[name=divSalesOrderNumber]').show();
                    mElement.find('[name=divPoNumber]').show();
                    mTxtQuoteNumber.attr('readonly', null)
                        .attr('placeholder', 'Select a quotation');
                    mElement.find('[name=butSaveACopy]').show();
                    $('[custom-tab-content=#tabOPDashboard]').text('Order Processing Dashboard');
                    $('[custom-tab-content=#tabOPWork]').text('Order Processing');
                    $('[custom-tab-content=#tabOPSettings]').text('Order Processing Settings');
                    InitDeliveryMethods();
                } else if (mOut.TransType == 'Invoice') {
                    mElement.find('[name=divSalesOrderNumber]').show();
                    mElement.find('[name=divPoNumber]').show();
                    mTxtQuoteNumber.attr('readonly', null)
                        .attr('placeholder', 'This field is read-only');
                    mElement.find('[name=butSaveACopy]').hide();
                    $('[custom-tab-content=#tabOPDashboard]').text('Invoicing Dashboard');
                    $('[custom-tab-content=#tabOPWork]').text('Invoicing');
                    $('[custom-tab-content=#tabOPSettings]').text('Invoicing Settings');
                    //InitInvoiceTypes();
                } else if (mOut.TransType == 'PurchaseOrder') {
                } else if (mOut.TransType == 'StockReceiving') {
                } else if (mOut.TransType == 'AccountsPayable') {
                } else if (mOut.TransType == 'StockTransfer') {
                } else {
                    throw 'Invalid vTransType. ref: js9S8L68QX4L!';
                }

                ShowHideColumns();

                $('#lblCashierName').text(sessionStorage.CurrentUserFullName);
                $('#lblFowstName').text(sessionStorage.FowstName);
                $('#lblCompanyName').text(sessionStorage.OrgName);                

                require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function () {
                    ERP.Payment.controller.FromWebPos = true;
                    if (!CBool(vParams.ViewInvFromCrm)) { // erp/accountdetails/controller.js
                        $('#tabOPWorkAccountDetails').append($('#popupAccountDetailsContent')).show();
                        $('#tabOPWorkAccountDetails').hide();
                    }
                });

                function DisplayTime() {
                    if (mLocalTime == null)
                        return;
                    if (mServerTime == null)
                        return;
                    var pElapsed = new Date() - mLocalTime;
                    var pEstimatedServerTime = new Date(mServerTime.getTime() + pElapsed);
                    var pZReadTime = proj.GetZReadDate(pEstimatedServerTime);
                    var pText = FormatDate(pZReadTime, { Format: 'MMM d, yyyy hh:mm a' });
                    sessionStorage.ZReadDate = pText;
                    pText = (mEnforceZRead ? 'Z' : 'C') + ' ' + pText;
                    $('#divClock').text(pText);
                }
                setInterval(DisplayTime, 1000);

                AjaxSend({
                    url: 'SalesMenu/PaymentOptions',
                    success: function (e) {
                        $('#btnDeliveryInfo').prop('disabled', !CBool(e.DeliveryInfo));
                        $('#btnPaymentCheque').prop('disabled', !CBool(e.Check));
                        $('#btnPaymentDCCard').prop('disabled', !CBool(e.Debit));
                        $('#btnPaymentSrv').prop('disabled', !CBool(e.SRV));
                        $('#btnPaymentOnCredit').prop('disabled', !CBool(e.OnCredit));
                        $('#btnPaymentOther').prop('disabled', !CBool(e.OtherPayment));
                        $('#btnPaymentDiscounts').prop('disabled', !CBool(e.Discounts));
                        $('#btnPaymentCharges').prop('disabled', !CBool(e.Charges));
                        $('#btnPaymentRewards').prop('disabled', !CBool(e.Rewards));
                        NewTrans();
                    }
                })
            };

            function LoadAccountsDetails() {
                require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function () {
                    var pMode;

                    if (mTargetOrgType == 'customer')
                        pMode = 'AR';
                    else
                        pMode = 'AP';
                    ERP.AccountDetails.controller.Mode = pMode;
                    ERP.AccountDetails.controller.CustomerID = mOut.Model.Data.CustomerID;
                    ERP.AccountDetails.controller.CustomerName = mTxtCustomer.val();
                    ERP.AccountDetails.controller.FromWebPos = true;
                    //$('#tabOPWorkAccountDetails').append($('#popupAccountDetails').find('.popup-content:first')).show();
                    //$('#popupAccountDetails').show();
                    //ERP.AccountDetails.controller.GetCustomerCreditDetails(mOut.Model.Data.CustomerID, 'has payable');
                });
            }

            mElement.find('[name=lnkChildSos]').click(function (e) {
                e.preventDefault();
                mChildSos.Show({
                    InvoiceID: mOut.Model.Data.ID
                });
                return false;
            });

            mElement.find('[name=lnkRelatedSos]').click(function (e) {
                e.preventDefault();
                var pSalesOrderID;
                if (mOut.TransType == 'SalesOrder')
                    pSalesOrderID = mOut.Model.Data.ID
                else if (mOut.TransType == 'Invoice')
                    pSalesOrderID = mOut.Model.Data.SalesOrderID
                else
                    return false;
                if (CStr2(pSalesOrderID) != '')
                    mRelatedOrdersPopup.popup({
                        ID: pSalesOrderID,
                        DontAutoClose: true,
                        IncludeInvoiced: true,
                        OnOk: function () {

                        },
                        OnCancel: function () {

                        }
                    });
                return false;
            });

            var mTxtTransferFromOrg = mElement.find('[name=txtTransferFromOrg]');
            mElement.find('[name=butTransferFromOrg]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowseTransferOrg({
                    Title: 'Transfer From Organization',
                    Success: function (vData) {
                        mTxtTransferFromOrg.val(vData.CompanyName);
                        mOut.Model.Data.TransferFromOrgID = vData.ID;
                    }
                });
            });

            var mTxtTransferToOrg = mElement.find('[name=txtTransferToOrg]');
            mElement.find('[name=butTransferToOrg]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowseTransferOrg({
                    Title: 'Transfer To Organization',
                    Success: function (vData) {
                        mTxtTransferToOrg.val(vData.CompanyName);
                        mOut.Model.Data.TransferToOrgID = vData.ID;
                    }
                });
            });

            function BrowseTransferOrg(vParams) {
                vParams = $.extend({
                    Title: null,
                    Success: function (vData) {

                    }
                }, vParams);
                BrowsePopup('LibraryOrganization', vParams.Title,
                    [
                        {
                            // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                vParams.Success(e);
                            }
                        }
                    ]
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            var mTxtTransferFromOutlet = mElement.find('[name=txtTransferFromOutlet]');
            mElement.find('[name=butTransferFromOutlet]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowseTransferOutlet({
                    Title: 'Transfer From Outlet',
                    FromOrTo: 'From',
                    Success: function (vData) {
                        mTxtTransferFromOutlet.val(vData.Data.FowstName);
                        mOut.Model.Data[vData.OutletFieldName] = vData.ID;
                    }
                });
            });

            var mTxtTransferToOutlet = mElement.find('[name=txtTransferToOutlet]');
            mElement.find('[name=butTransferToOutlet]').click(function () {
                if (!HandleEditingSavedAsFinal())
                    return;
                BrowseTransferOutlet({
                    Title: 'Transfer To Outlet',
                    FromOrTo: 'To',
                    Success: function (vData) {
                        mTxtTransferToOutlet.val(vData.Data.FowstName);
                        mOut.Model.Data[vData.OutletFieldName] = vData.ID;
                    }
                });
            });

            function BrowseTransferOutlet(vParams) {
                vParams = $.extend({
                    Title: null,
                    FromOrTo: '', // From or To (case-sensitive)
                    Success: function (vData) {

                    }
                }, vParams);
                if (!HandleEditingSavedAsFinal())
                    return;
                var pOutletFieldName = 'Transfer' + vParams.FromOrTo + 'OutletID';
                var pOrgFieldName = 'Transfer' + vParams.FromOrTo + 'OrgID';
                if (!IsGuid(mOut.Model.Data[pOrgFieldName])) {
                    var pOrgTitle = 'Transfer ' + vParams.FromOrTo + ' Organization';
                    NewMessageBox('Please select a ' + pOrgTitle + ' first.');
                    return;
                }
                BrowsePopup('LibraryFowst', vParams.Title,
                    [
                        {
                            // Search: mTxtCanvass.val(),
                            AutoSearch: true,
                            StyleFormat: 'table',
                            DisplayText: [
                                {
                                    Label: 'Outlet',
                                    Column: 'FowstName'
                                }
                            ],
                            Click: function (e) {
                                var pData = {
                                    OutletFieldName: pOutletFieldName,
                                    Data: e
                                };
                                vParams.Success(pData);
                            }
                        }
                    ], {
                        IsThirdPartyOrganization: 1,
                        ThirdPartyOrganizationID: mOut.Model.Data[pOrgFieldName]
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            mElement.find('[name=butSaveAndFinalizeStockTransfer]').click(function () {
                if (!HandleEditingSavedAsFinal({
                    AllowSomeSaving: true,
                    AllowSomeSavingCallback: function () {
                        mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                        mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                        mOut.Model.SaveSomeFields({
                    success: function (vData) {
                                if (vData.Error)
                                    NewMessageBox(vData.Error, 'Failed');
                else
                                    NewMessageBox('Data were successfully saved.', 'Success');
                }
                });
                } // END AllowSomeSavingCallback: function
                })) {
                    return;
                }
                mOut.Model.Data.CopyProductsFromInvoiceID = null;
                mOut.Model.Data.SetInvoiceID = null;
                PreSaveStockTransfer({
                    SaveAndFinalize: true
                });
            });

            mElement.find('[name=butSaveAsDraftStockTransfer]').click(function () {
                SaveAsDraftStockTransfer();
            });

            function SaveAsDraftStockTransfer(vParams) {
                if (!HandleEditingSavedAsFinal())
                    return;
                mOut.Model.Data.CopyProductsFromInvoiceID = null;
                mOut.Model.Data.SetInvoiceID = null;
                PreSaveStockTransfer(vParams);
            };

            function PreSaveStockTransfer(vParams) {
                vParams = $.extend({
                    UseUnitPriceFromLibrary: false,
                    SaveAndFinalize: false,
                    SaveACopy: false,
                    NoPrompt: false,
                    Success: function () {
                        NewMessageBox('Transaction was successfully saved.');
                    }
                }, vParams);

                mOut.Model.Data.FowstID = sessionStorage.FowstID;
                mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
                mOut.Model.Data.DateNeeded = mTxtDateNeeded.val();
                mOut.Model.Data.Remarks = mTxtRemarks.val();
                if (mOut.TransType == 'PurchaseOrder')
                    mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                else
                    mOut.Model.Data.PurchaseOrderText = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderDate = mTxtPoDate.val();
                mOut.Model.Data.QuotationNumText = mTxtQuotationNumText.val();
                mOut.Model.Data.QuotationDate = mTxtQuoteDate.val();
                mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                mOut.Model.Data.DocumentNameID = null; // mCbInvoiceType.val();
                mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                mOut.Model.Data.DeliveryMethod = mCbDeliveryMethod.val();

                mOut.Model.Data.Products = [];

                var pMessage;
                var pTitle;

                if (!vParams.NoPrompt) {
                    if (vParams.SaveACopy) {
                        pTitle = 'Are you sure you want to save a copy of this transaction document?';
                        pMessage = 'This source transaction will not be updated.\n' +
                            'Instead, the changes you’ve made to this original transaction will get saved with the new transaction.';
                    } else if (vParams.SaveAndFinalize) {
                        pTitle = 'Save ' + (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?<br /><br />';
                        pMessage += 'Finalizing this transaction means that you will not be able to edit this transaction\'s information later on.<br /><br />';
                        pMessage += 'Only finalized transactions can be printed out (subject to print number limits set by your system administrator).';
                    } else {
                        pTitle = 'Save ' + (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?';
                    }

                    mOut.Model.Verify({
                        Success: function () {
                            NewMessageBox(pMessage, pTitle, [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        Save({
                                            UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                                            SaveAndFinalize: vParams.SaveAndFinalize,
                                            Success: vParams.Success
                                        });
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        },
                        Fail: function (vMessage) {
                            NewMessageBox(vMessage);
                        }
                    });
                } else {
                    SaveStockTransfer({
                        UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                        Success: vParams.Success
                    });
                }
            } // END function PreSave(vParams) {

            function SaveStockTransfer(vParams) {
                vParams = $.extend({
                    NoPrompt: false,
                    Success: function () {
                        if (!vParams.NoPrompt)
                            NewMessageBox('Transaction was successfully saved.');
                    }
                }, vParams);
                mOut.Model.SaveStockTransfer({
                    Data: {
                        ConsolidateSalesOrder: vParams.ConsolidateSalesOrder,
                        UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary
                    },
                    Success: function (vData) {
                        if (CDec2(vData.OtherOrderCount) > 0) {
                            //&& !vParams.NoPrompt) {

                            NewMessageBox('Items from child orders are ready for invoicing. Do you want to include those items to this invoice?', 'Child orders?', [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        vParams.ConsolidateSalesOrder = true;
                                        Save(vParams);
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function () {
                                        vParams.ConsolidateSalesOrder = false;
                                        Save(vParams);
                                    }
                                },
                                {
                                    Text: 'Cancel'
                                }
                            ]);
                            return;
                        }
                        LoadTrans({
                            ID: vData.ID,
                            AfterLoad: function () {
                                vParams.Success(vData);
                                mSavedAsFinal = vParams.SaveAndFinalize;
                                if (CBool(mSavedAsFinal))
                                    NewMessageBox('Do you want to print this transaction?', 'Print?', [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                HandlePrint({
                                                    NewTransAfterPrint: true
                                                });
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                    ]);
                            }
                        });
                        //if (CStr2(vData.QuotationNo) != '')
                        //    mTxtQuoteNumber.val(vData.QuotationNo);
                        //if (CStr2(vData.SalesOrderNo) != '')
                        //    mTxtSalesOrderNumber.val(vData.SalesOrderNo);
                        //if (CStr2(vData.InvoiceNumber) != '')
                        //    mTxtInvoiceNumber.val(vData.InvoiceNumber);
                        //if (CStr2(vData.PurchaseOrderNo) != '')
                        //    mTxtPoNumber.val(vData.PurchaseOrderNo);
                    },
                    Fail: function (vMessage) {
                        NewMessageBox('Mobiz cannot save your current transaction because: ' + vMessage);
                    }
                });
            }

            mOut.ReadOnly = function () {
                if (mOut.TransType != 'AccountsPayable')
                    mElement.find('[name=divToolbar]').hide();
                mElement.find('[name=butAddProduct]').hide();
                mElement.find('[name=divAddproducts]').hide();
                mElement.find('.btn-xs').hide();
                $('#divEasHeader, #divQuotationTab, #divInvoicingTab, #divOrderProcessingTab').hide();
                $('#divInvoicingTab, #divOrderProcessingTab, #divPurchasingTab').addClass('hidetab').hide().css({ 'display': 'none' });

                // 2019-05-06 comment this
                //mElement.find('#divOPWorkTab').css({
                //    position: 'absolute',
                //    top: '25px'
                //})

                //mElement.find('[name=divDraftOrFinalized]').css({
                //    position: 'absolute',
                //    right: 0
                //});
                mElement.find('#accRevOrderHeader').customaccordion();
            };

            mElement.find('[name=butOptions]').click(function () {
                $('#divPopupAccountsPayablegOptions').show().popup('open');
                $('#divPopupAccountsPayablegOptions').find('[name=chkFollowVendorCost][value=vendor]').prop('checked', true).checkboxradio('refresh');
            });

            $('#divPopupAccountsPayablegOptions').find('[name=butOptionSave]').click(function () {
                LoadTrans({
                    ID: mOut.Model.Data.ID,
                    AccountsPayableFollowCost: $('#divPopupAccountsPayablegOptions').find('[name=chkFollowVendorCost]:checked').val()
                });
                $('#divPopupAccountsPayablegOptions').show().popup('close');
            });

            mOut.Initialize(mOut.TransType);            

            WebPOS = mOut;
            return mOut;
            //}); // END CreateStringObject('App.controller.PriceChange', null, function() {

            //return mDeferred.promise();
        })
    });
})
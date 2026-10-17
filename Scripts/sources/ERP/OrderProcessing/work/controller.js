define([
    '/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/OrderProcessing/product/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/OrderProcessing/relatedOrders/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/OrderProcessing/serialNumbers/controller.js',
    'promise!/Scripts/sources/ERP/QuickPim/controller.js',
    //'promise!/Scripts/sources/ERP/InventoryBalance/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/ChildSos/controller.js',
    'promise!/Scripts/sources/ERP/ContactPerson/controller.js',
    'promise!/Scripts/sources/PIMUpload/PIMUpload.js',
    '/Content/Ecommerce/js/lightbox-2.11.1.js',
    '/Scripts/sources/ERP/Invoicing/InvoiceImage/controller.js?crc=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mQuoteHtml;
    var mSalesOrderHtml;
    var mInvoiceHtml;
    var mStockReceivingHtml;
    var mStockTransferHtml;
    var mPurchaseOrderRequestHtml;
    var mPurchaseOrderHtml;
    var mUnitPriceSurchargePercent = 0;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Quote/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/OrderProcessing/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/Receiving/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/StockTransfer/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/PurchaseOrderRequest/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/PurchaseOrder/work/view.html?v=' + assetversion,
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
                ExactUrl: '/Scripts/sources/ERP/Invoicing/work/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mInvoiceHtml = vData;
                    mDeferred.resolve();
                }
            });
        }

    })();

    CreateStringObject('ERP.OrderProcessing.work.controller', null, function (vParams) {

        var mOut = {};
        var mElement;
        var mCustomerName;
        var mSavedAsFinal;
        var pProductPopup = new ERP.OrderProcessing.product.controller();
        var mRelatedOrdersPopup = new ERP.OrderProcessing.relatedOrders.controller();
        var mSerialNumbersPopup = new ERP.OrderProcessing.serialNumbers.controller();
        var mMaxPaymentTerm = 0;
        var mTargetOrgType; // customer or vendor
        var mInventoryBalance; // = new ERP.InventoryBalance.controller();
        var mChildSos = new ERP.ChildSos.controller();
        var mContactPerson = new ERP.ContactPerson.controller();
        var mInvoiceTypes;
        var mFromSalesOrder;
        var mHasCnCUnapprovedReason;
        var mFinalizedForApproval;
        var mUnapprovedProfitability;

        vParams = $.extend({
            TransType: null,
            Callback: function(){},
            ShowProductSuppliersColumn: false
        }, vParams);
        mTargetOrgType = 'customer';
        mOut.TransType = vParams.TransType;
        mOut.ShowProductSuppliersColumn = vParams.ShowProductSuppliersColumn;
        if (mOut.TransType == 'Quote') {
            mElement = $(mQuoteHtml);
            $('#divQuotationMain .tab-content').append(mElement);
            $('#tabQuoteWork').opentabmenu();
        } else if (mOut.TransType == 'SalesOrder') {
            mElement = $(mSalesOrderHtml);
            $('#divOrderProcessingMain .tab-content').append(mElement);
            $('#tabOPWork').opentabmenu();
        } else if (mOut.TransType == 'Invoice') {
            mElement = $(mInvoiceHtml);

            if ($('#divInvoicingMain #tabInvoicingWork').length == 0) {
                $('#divInvoicingMain .tab-content').append(mElement);
            } else {
                $('#tabInvoicingWork').append($('#popupInvoiceContent'));
            }
            $('#tabInvoicingWork').opentabmenu();
        } else if (mOut.TransType == 'PurchaseOrderRequest') {
            mElement = $(mPurchaseOrderRequestHtml);

            if ($('#divPurchaseOrderRequestMain > #tabPurchaseOrderRequestWork').length == 0) {
                $('#divPurchaseOrderRequestMain').append(mElement);
            } else {
                $('#tabPurchaseOrderRequestWork').append($('#popupPurchaseOrderRequestContent'));
            }
        } else if (mOut.TransType == 'PurchaseOrder') {
            //mElement = $('#divPurchasingMain');

            mElement = $(mPurchaseOrderHtml);

            if (proj.IsBootstrapDesign()){
                $('#divPurchasingMain .tab-content').append(mElement);
                $('#tabPurchasingWork').removeAttr('custom-tab-role style')
            } else {
                $('#divPurchasingMain').append(mElement);
            }            

            mTargetOrgType = 'vendor';
            proj.GetOpenPurchaseOrders();
            proj.GetUnapprovedPurchaseOrderCount();
            // no need to append
        } else if (mOut.TransType == 'AccountsPayable') {
            mElement = $(mPurchaseOrderHtml);
            $('#divPurchasingMain').append(mElement);
            mTargetOrgType = 'vendor';
            // no need to append
        } else if (mOut.TransType == 'StockTransfer') {
            mElement = $(mStockTransferHtml);
            $('#divStockTransferMain .tab-content').append(mElement);
        } else {
            alert('Unknown TransType, ' + vParams.TransType + '. ref: js7VUG78QX3V!');
            debugger;
        }
        
        mOut.element = mElement;

        mOut.Model = new ERP.OrderProcessing.work.model();
        var mButOptions = mElement.find('[name=butOptions]');
        if (mOut.TransType == 'AccountsPayable') {
            mButOptions.show();
            mElement.find('[name=butCopyToSo]').hide();
        }

        if (CBool(GetUrlParameters().IsSoaLink)){
            mElement.find('[name=butInvoiceImage]').show();
            mElement.find('[name=butOutput]').remove();
        } else {
            mElement.find('[name=butInvoiceImage]').remove();
        }

        if (CBool(GetUrlParameters().IsFollowUp)){
            mElement.find('[name=butMarkAsFollowedUp]').show();
        } else {
            mElement.find('[name=butMarkAsFollowedUp]').remove();
        }
        mElement.find('[name=divTransDate]').click(function() {
            SingleCalendar({
                ShowTime: true,
                Click: function(e){
                    mOut.Model.Data.TransDate = e;
                    DisplayTransDate(e);
                }
            });
        });

        function DisplayTransDate(vDate) {
            if (vDate == null)
                vDate = new Date();

            var pLabel;
            switch (mOut.Model.Data.TransType ) {
                case 'SalesOrder':
                    pLabel = 'Sales Order Date: ';
                    break;

                case 'Invoice':
                    pLabel = 'Invoice Date: ';
                    break;

                default:
                    pLabel = 'Transaction Date: ';
            }

            mElement.find('[name=divTransDate]').text(pLabel + FormatDate(new Date(vDate), {Format: 'MMMM d, yyyy h:mm a'}));
        }

        var mTxtCustomerFowst = mElement.find('[name=txtCustomerFowst]');
        mTxtCustomerFowst.on('keypress', function(e) {
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
        mTxtCustomer.click(function() {
            SearchCustomer({
                key: 'Enter'
            });
        });
        mElement.find('[name=butCustomer]').click(function() {
            SearchCustomer({
                key: 'Enter'
            });
        });

        proj.BeforeChangeFowst = function(vParams) {
            if (mSavedAsFinal || $.isEmptyObject(mOut.Model.Data.CustomerID)) {
                vParams.Success();
                return;
            }
                
            var pMsg = 'Changing FOWST will save the current transaction and create a new one. Do you still want to change FOWST?';
            NewMessageBox(pMsg, '', [
                {
                    Text: 'Yes',
                    Click: function() {
                        PreSave({
                            NoPrompt: true,
                            Success: function() {
                                NewTrans();
                                vParams.Success();
                            }
                        });
                    }
                },
                {
                    Text: 'No',
                    Click: function() {
                        // just do nothing
                    }
                },
                {
                    Text: 'Discard current cart',
                    Click: function() {
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

            if ((
                (mOut.Model.Data.TransType != 'PurchaseOrder')
                && (mOut.Model.Data.TransType != 'Quote')
                )
                && !$.isEmptyObject(mOut.Model.Data.CustomerID)) {
                var pMsg = 'Changing ' + mTargetOrgType + '  will save the current transaction and create a new one. Do you still want to change ' + mTargetOrgType + '?';
                NewMessageBox(pMsg, '', [
                    {
                        Text: 'Yes',
                        Click: function() {
                            PreSave({
                                NoPrompt: true,
                                Success: function() {
                                    NewTrans();
                                    DoBrowsePopup();    
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function() {
                            mTxtCustomer.val(mCustomerName);
                        }
                    },
                    {
                        Text: 'Discard current cart',
                        Click: function() {
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
                BrowsePopup('LibraryOrganization', pTarget,
                    [
                        {
                            // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                Bs.Loading('show');
                                mCustomerName = e.CompanyName;
                                mTxtCustomer.val(e.CompanyName);
                                mOut.Model.Data.CustomerID = e.ID;
                                mMaxPaymentTerm = CDec2(e.CreditTerms);
                                mTxtPaymentTerms.val(CDec2(e.CreditTerms));
                                mTxtSalesRep.val(e.SalesRepName);
                                mCustomerRequirePoNum = CBool(e.CustomerRequirePoNum);
                                LoadAccountsDetails();
                                LoadSingleAddress();
                                if ((mOut.TransType == 'PurchaseOrder') 
                                    && (IsGuid(mOut.Model.Data.ID)))
                                    SetLineItemUnitPrice(null, null, 'VendorPrice');
                                Bs.Loading('hide');
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
                success: function(vData) {
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

        mElement.find('[name=butInvoiceImageReference]').click(function () {
            ERP.InvoiceImage.main.controller.popup({
                mod: mOut.TransType,
                ID: mOut.Model.Data.ID,
                SupplierID: mOut.Model.Data.CustomerID,
                SupplierName: mTxtCustomer.val()
            });
        });
        mElement.find('[name=butCustomerShowAccount]').click(ShowAccountDetails);
        function ShowAccountDetails() {
            if (!IsGuid(mOut.Model.Data.CustomerID)) {
                NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                return;
            }

            require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function() {
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
        mTxtCanvass.on('keypress', function(e) {
            if (e.key == 'Enter')
                SearchCanvass();
        }); // END mTxtBillingAddress.on('keypress', function(e) {
        mElement.find('[name=butPoCanvass]').click(function() {
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
                            Click: function() {
                                SearchCanvass(true);
                            }
                        },
                        {
                            Text: 'No',
                            Click: function() {
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
                                Success: function(vData) {
                                    mOut.Model.Data.CopyFromCanvass = false;
                                    LoadTrans({
                                        ID: vData.ID
                                    });
                                    //mOut.Model.Data.ID = vData.ID;
                                }
                            });
                        }
                    }
                ], { FilterCustomers:1 }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function SearchCanvass() {

        var mTxtDateNeeded = mElement.find('[name=txtDateNeeded]');
        var mChkDontAutoInvoice = mElement.find('[name=chkDontAutoInvoice]');
        mTxtDateNeeded.click(BrowseDateNeeded);
        function BrowseDateNeeded() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                ShowTime: true,
                SetTime: FormatDate(new Date(), {Format: 'h:mm a'}),
                Click: function(e){
                    mTxtDateNeeded.val(e);
                }
            });
        }
        mElement.find('[name=butDateNeeded]').click(BrowseDateNeeded);

        var mTxtBillingAddress = mElement.find('[name=txtBillingAddress]');
        mTxtBillingAddress.on('keypress', function(e) {
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
        mTxtDeliveryAddress.on('keypress', function(e) {
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

        // "Freight and Handling" tab
        var mTxtShipper = mElement.find('[name=txtShipper]');
        mTxtShipper.click(BrowseShipper);

        var mTxtShipperAddress = mElement.find('[name=txtShipperAddress]');
        mTxtShipperAddress.click(SearchShipperAddress);

        mTxtShipperAddress.on('keypress', function(e) {
            if (!HandleEditingSavedAsFinal())
                return;
            if (e.key == 'Enter')
                SearchShipperAddress();
        }); // END mTxtBillingAddress.on('keypress', function(e) {

        function SearchShipperAddress() {
            if (!HandleEditingSavedAsFinal())
                return;
            if (!IsGuid(mOut.Model.Data.ShipperID)) {
                NewMessageBox('Please select a shipper first.');
                return;
            }

            BrowsePopup('LibraryFowst', 'Shipper Address',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Address' }],
                        Click: function (e) {
                            mTxtShipperAddress.val(e.Address);
                            mOut.Model.Data.ShipperAddressID = e.ID;
                        }
                    }
                ], { 
                    OrganizationID: mOut.Model.Data.ShipperID,
                    HideNoAddress: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        var mButShipper = mElement.find('[name=butShipper]');
        mButShipper.click(BrowseShipper);

        function BrowseShipper() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowsePopup('LibraryOrganization', 'Shipper',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mOut.Model.Data.ShipperID = e.ID;
                            mTxtShipper.val(e.CompanyName);
                        }
                    }
                ], { 
                    FilterSupplier:1 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }
        var mTxtWaybillNumber = mElement.find('[name=txtWaybillNumber]');
        var mTxtShipDate = mElement.find('[name=txtShipDate]');
        mElement.find('[name=butShipDate]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                Click: function(e){
                    mTxtShipDate.val(e);
                }
            });
        });
        var mTxtPackageQuantity = mElement.find('[name=txtPackageQuantity]');
        var mTxtGrossWeight = mElement.find('[name=txtGrossWeight]');
        var mTxtGrossVolume = mElement.find('[name=txtGrossVolume]');
        var mTxtFreight = mElement.find('[name=txtFreight]');
        var mTxtHandling = mElement.find('[name=txtHandling]');
        var mTxtArrastre = mElement.find('[name=txtArrastre]');
        var mTxtOtherCharges = mElement.find('[name=txtOtherCharges]');
        var mTxtFreightAndHandling = mElement.find('[name=txtFreightAndHandling]');

        mTxtFreight.on('textchange', charges_change);
        mTxtHandling.on('textchange', charges_change);
        mTxtArrastre.on('textchange', charges_change);
        mTxtOtherCharges.on('textchange', charges_change);

        function charges_change() {
            var pTotal = CDec2(mTxtFreight.val()) +
                CDec2(mTxtHandling.val()) +
                CDec2(mTxtArrastre.val()) +
                CDec2(mTxtOtherCharges.val());
            mTxtFreightAndHandling.val(AccountingFormat(pTotal));
        }

        mElement.find('[name=butSaveAndFinalize]').click(function() {
            if (!HandleEditingSavedAsFinal({
                AllowFinalizedForApproval: true,
                AllowSomeSaving: true,
                AllowSomeSavingCallback: function() {
                    mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                    mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                    mOut.Model.Data.SalesInvoiceRef = mTxtSalesInvoiceRef.val();
                    mOut.Model.Data.OfficialReceiptRef = mTxtOfficialReceiptRef.val();
                    mOut.Model.SaveSomeFields({
                        success: function(vData) {
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
            PreSave({
                SaveAndFinalize: true
            });
        });

        mElement.find('[name=butSaveAsDraft]').click(function() {
            SaveAsDraft();
        });

        function SaveAsDraft(vParams) {
            if (!HandleEditingSavedAsFinal())
                return;
            mOut.Model.Data.CopyProductsFromInvoiceID = null;
            mOut.Model.Data.SetInvoiceID = null;
            PreSave(vParams);
        };

        mElement.find('[name=butSaveACopy]').click(function() {
            if (!IsGuid(mOut.Model.Data.ID)) {
                NewMessageBox('Please load a transaction first.', 'Save a copy');
                return;
            }

            NewMessageBox('Are you sure you want to copy this transaction?', 'Save a copy', [
                {
                    Text: 'Yes',
                    Click: function() {
                        AjaxSend({
                            url: '/SalesMenu/CopyTransaction',
                            data: {
                                ID: mOut.Model.Data.ID
                            },
                            success: function(vData) {
                                if (vData.Error) {
                                    NewMessageBox(vData.Error);
                                } else if (IsGuid(vData.Result)) {
                                    NewMessageBox('The transaction was successfully copied.', 'Save a copy');
                                    LoadTrans({
                                        ID: vData.Result
                                    });
                                } else 
                                    NewMessageBox(vData);
                            }
                        });
                    }
                },
                {
                    Text: 'No'
                }
            ]);
        });

        mElement.find('[name=butCopyToItemRequisition]').click(function() {
            if (!IsGuid(mOut.Model.Data.ID)) {
                NewMessageBox('Please load a transaction first.', 'Save a copy');
                return;
            }

            NewMessageBox('Are you sure you want to copy this transaction to item requisition?', 'Copy To Item Requisition', [
                {
                    Text: 'Yes',
                    Click: function() {
                        AjaxSend({
                            url: '/SalesMenu/CopyToItemRequisition',
                            data: {
                                ID: mOut.Model.Data.ID,
                                FowstID: sessionStorage.FowstID
                            },
                            success: function(vData) {
                                if (vData.Error) {
                                    NewMessageBox(vData.Error);
                                } else if (vData.Result == 'ok') {
                                    NewMessageBox('The transaction was successfully copied to item requisition', 'Copy To Item Requisition');
                                } else 
                                    NewMessageBox(vData);
                            }
                        });
                    }
                },
                {
                    Text: 'No'
                }
            ]);
        });

        mElement.find('[name=butCopyToSo]').click(function() {
            if (!IsGuid(mOut.Model.Data.ID)) {
                NewMessageBox('Please load a transaction first.', 'Copy To Sales Order');
                return;
            }

            BrowsePopup('LibraryOrganization', 'Choose Customer',
                [
                    {
                        // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            SaveCopy(e.ID);
                        }
                    }
                ], { FilterCustomers:1 }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',

            function SaveCopy(vCustomerID) {
                AjaxSend({
                    url: '/SalesMenu/CopyTransaction',
                    data: {
                        ID: mOut.Model.Data.ID,
                        TransType: 'SalesOrder',
                        CustomerID: vCustomerID
                    },
                    success: function(vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                        } else if (IsGuid(vData.Result)) {
                            NewMessageBox('The transaction was successfully copied.', 'Save a copy');
                            //var win = window.open(location.origin + '/erp' + location.search + '&IsPreview=1&ID=' + vData.Result,
                            //    '_blank',
                            //    'width=' + screen.availWidth + ',height=' + screen.availHeight);
                        } else 
                            NewMessageBox(vData);
                    }
                });
            }
            
        });

        var mTxtRemarks = mElement.find('[name=txtRemarks]');
        mTxtRemarks.on('keypress', function() {
            if (!HandleEditingSavedAsFinal())
                return;
        });

        var mTxtInternalRemarks = mElement.find('[name=txtInternalRemarks]');
        mTxtInternalRemarks.on('keypress', function() {
            if (!HandleEditingSavedAsFinal())
                return;
        });

        mElement.find('[name="btnChooseQuickRemarks"], [name="btnChooseQuickInternalRemarks"]').click(function(){
            var btnName = $(this).attr('name');

            if (!HandleEditingSavedAsFinal())
                return;

            BrowsePopup('/ERP/GetQuickRemarks', (btnName == 'btnChooseQuickRemarks' ? 'Quick' : 'Internal') + ' Remarks', [{
                ModalSize: 'xl',
                AutoSearch: true,
                ServerSideProcessing: true,
                StyleFormat: 'table',
                Options: {
                    Fields: [
                        $('<tr>')
                            .append($('<td>')
                                .append('Show Tag: ')
                            )
                            .append($('<td>')
                                .append($('<select name="selWorkQuickRemarks" class="form-select fixed">')
                                    .append('<option value="">All Tags</option>')
                                    .append('<option value="Quotation">Quotation</option>')
                                    .append('<option value="Sales Order">Sales Order</option>')
                                    .append('<option value="Stock Transfer">Stock Transfer</option>')
                                    .append('<option value="Invoicing">Invoicing</option>')
                                    .append('<option value="Purchasing">Purchasing</option>')
                                    .append('<option value="Stock Receiving">Stock Receiving</option>')
                                    .change(function () {
                                        BrowsePopupNow();
                                    })
                                )
                            )
                    ],
                    OnChange: function () {
                        return {
                            Show: $('[name="selWorkQuickRemarks"]').val()
                        }
                    },
                    OnLoad: function() {
                        var selWorkQuickRemarks = $('[name="selWorkQuickRemarks"]');

                        if (mOut.TransType == 'Quote') {
                            selWorkQuickRemarks.val('Quotation');
                        } else if (mOut.TransType == 'SalesOrder') {
                            selWorkQuickRemarks.val('Sales Order');
                        } else if (mOut.TransType == 'StockTransfer') {
                            selWorkQuickRemarks.val('Stock Transfer');
                        } else if (mOut.TransType == 'Invoice') {
                            selWorkQuickRemarks.val('Invoicing');
                        } else if (mOut.TransType == 'PurchaseOrder') {
                            selWorkQuickRemarks.val('Purchasing');
                        } else if (mOut.TransType == 'StockReceiving') {
                            selWorkQuickRemarks.val('Stock Receiving');
                        }
                    }
                },
                DisplayText: [
                    {
                        Label: '#',
                        Column: function (data) {
                            return data.RowNum + '.';
                        }, Style: { width: '50px' }
                    },
                    {
                        Label: 'Message',
                        Column: function (data) {
                            return CStr2(data.Message);
                        }
                    },
                    {
                        Label: 'Tag',
                        Column: function (data) {
                            return CStr2(data.Tag);
                        }, Style: { width: '150px' }
                    },
                    {
                        Label: 'Updated By',
                        Column: function (data) {
                            return CStr2(data.UpdatedByName);
                        }, Style: { width: '150px' }
                    },
                    {
                        Label: 'Last Updated',
                        Column: function (data) {
                            return $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yyyy');
                        }, Style: { width: '150px' }, className: 'text-center'
                    }
                ],
                Click: function (e) {
                    if (btnName == 'btnChooseQuickRemarks'){
                        mTxtRemarks.val(mTxtRemarks.val() + '' + e.Message + ' \n\n');
                    } else {
                        mTxtInternalRemarks.val(mTxtInternalRemarks.val() + '' + e.Message + ' \n\n');
                    }                    
                }
            }], { IsInternalRemarks: (btnName == 'btnChooseQuickRemarks' ? 0 : 1)})
        })

        function PreSave(vParams) {
            vParams = $.extend({
                UseUnitPriceFromLibrary: false,
                SaveAndFinalize: false,
                SaveACopy: false,
                NoPrompt: false,
                RelatedSalesOrders: null,
                Success: function(){
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
                if ((mOut.TransType == 'PurchaseOrder')
                    || (mOut.TransType == 'AccountsPayable')) {

                    NewMessageBox('Warning: The maximum allowed payment term for this ' + mTargetOrgType + ' is ' + mMaxPaymentTerm + ' but you entered ' + pPaymentTerms + '. Do you want to continue?', 'Payment Term Warning', [
                        {
                            Text: 'Continue saving',
                            Click: Proceed2
                        },
                        {
                            Text: 'Do not save'
                        },
                    ]);
                    return;
                } else {
                    NewMessageBox('Mobiz cannot save your current transaction because: The maximum allowed payment term for this ' + mTargetOrgType + ' is ' + mMaxPaymentTerm + ' but you entered ' + pPaymentTerms + '. Please enter a value that is equal to or lesser than ' + mMaxPaymentTerm + '.');
                    return;
                }
            } else
                Proceed2();

            function Proceed2() {
                if (CBool(mCustomerRequirePoNum)
                    && CBool(vParams.SaveAndFinalize)) {
                    if ((CStr2(mTxtPoNumber.val()).trim() == '')
                        || !IsDate(mTxtPoDate.val())) {

                        if (mOut.Model.Data.TransType == 'SalesOrder') {
                            NewMessageBox('This customer/ client requires that PO numbers and PO dates be referenced in their Invoices.<br /><br />' +
                                'You may leave the PO number and date fields blank in the SO module.<br /><br />' +
                                'However, PO number and PO date references must be encoded in the invoicing module in order to save and finalize this customer/ client’s invoice.', 
                                'PO Number and Date Are Required', null, 
                                {
                                    Close: function() {
                                        Proceed();
                                    }
                                });
                            return;    
                        } else if (mOut.Model.Data.TransType == 'Invoice') {
                            NewMessageBox('This customer/ client requires that PO numbers and PO dates be referenced in their Invoices.<br /><br />' +
                                'A PO number and PO date must be encoded in order to save and finalize this customer/ client’s invoice.', 
                                'PO Number and Date Are Required');
                            return;    
                        }
                    }
                }

                Proceed();
            }

            function Proceed() {
                if ((mOut.Model.Data.TransType == 'SalesOrder')
                    || (mOut.Model.Data.TransType == 'Invoice')) {

                    if (CBool(vParams.SaveAndFinalize)
                        && (CStr2(mTxtDateNeeded.val()).trim() == '')) {

                        NewMessageBox('Mobiz cannot save your current transaction because Date Needed is required.');
                        return;
                    }

                    
                }
                if (mOut.Model.Data.TransType == 'Invoice') {
                    if (CBool(vParams.SaveAndFinalize)) {
                        if (CStr2(mCbInvoiceType.val()) == '') {
                            NewMessageBox('Mobiz cannot save your current transaction because Invoice Type is required.');
                            return;
                        }
                    }
                }
                
                mOut.Model.Data.FowstID = sessionStorage.FowstID;
                mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
                mOut.Model.Data.DateNeeded = mTxtDateNeeded.val();
                mOut.Model.Data.DontAutoInvoice = mChkDontAutoInvoice.prop('checked') ? 1 : 0;
                mOut.Model.Data.Remarks = mTxtRemarks.val();
                if (mOut.TransType == 'PurchaseOrder'){
                    mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                    mOut.Model.Data.InternalRemarks = mTxtInternalRemarks.val();
                }                    
                else
                    mOut.Model.Data.PurchaseOrderText = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderNo = mTxtPoNumber.val();
                mOut.Model.Data.PurchaseOrderDate = mTxtPoDate.val();
                mOut.Model.Data.QuotationNumText = mTxtQuotationNumText.val();
                mOut.Model.Data.QuotationDate = mTxtQuoteDate.val();
                mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                mOut.Model.Data.PaymentTermsText = pPaymentTerms;
                mOut.Model.Data.PaymentTerm = pPaymentTerms;
                mOut.Model.Data.DocumentNameID = mCbInvoiceType.val();
                mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                mOut.Model.Data.DeliveryMethod = mCbDeliveryMethod.val();
                mOut.Model.Data.SalesInvoiceRef = mTxtSalesInvoiceRef.val();
                mOut.Model.Data.OfficialReceiptRef = mTxtOfficialReceiptRef.val();
            
                // "Freight and Handling" tab
                mOut.Model.Data.WaybillNumber = mTxtWaybillNumber.val();
                mOut.Model.Data.ShipDate = mTxtShipDate.val();
                mOut.Model.Data.PackageQuantity = mTxtPackageQuantity.val();
                mOut.Model.Data.GrossWeight = mTxtGrossWeight.val();
                mOut.Model.Data.GrossVolume = mTxtGrossVolume.val();
                mOut.Model.Data.Freight = mTxtFreight.val();
                mOut.Model.Data.Handling = mTxtHandling.val();
                mOut.Model.Data.Arrastre = mTxtArrastre.val();
                mOut.Model.Data.OtherCharges = mTxtOtherCharges.val();

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
                        pTitle = 'Save '+ (vParams.SaveAndFinalize ? 'And Finalize' + pForApproval: 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize' + pForApproval + ' ' : '') + 'this transaction?<br /><br />';
                        pMessage += 'Finalizing this transaction means that you will not be able to edit this transaction\'s information later on.<br /><br />';
                        pMessage += 'Only finalized transactions can be printed out (subject to print number limits set by your system administrator).';
                    } else {
                        pTitle = 'Save '+ (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                        pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?';
                    }

                    mOut.Model.Verify({
                        Success: function() {
                            NewMessageBox(pMessage, pTitle, [
                                {
                                    Text: 'Yes',
                                    Click: function() {
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
                        Fail: function(vMessage) {
                            Bs.Loading('hide');
                            NewMessageBox(vMessage);
                        }
                    });
                } else {
                    Save({
                        UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary,
                        Success: vParams.Success
                    });
                }
            }
        } // END function PreSave(vParams) {

        function Save(vParams) {
            vParams = $.extend({
                NoPrompt: false,
                Success: function(){
                    if (!vParams.NoPrompt)
                        NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);
            Bs.Loading('show');
            mOut.Model.Save({
                Data: {
                    ConsolidateSalesOrder: vParams.ConsolidateSalesOrder,
                    UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary
                },
                Success: function(vData) {
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
                    Bs.Loading('hide');
                    LoadTrans({
                        ID: vData.ID,
                        AfterLoad: function(vData2) {
                            vParams.Success(vData);
                            if ((vData2.Master.TransType == 'PurchaseOrder')
                                && (CInt2(vData2.Master.PurchaseOrderNo) > 0)
                                && (CStr2(vData2.Master.ApprovedDate) == '')) {

                                mFinalizedForApproval = true;
                                vData2.Master.FinalizedForApproval = true;
                            } 
                            mSavedAsFinal = vParams.SaveAndFinalize;
                            if (!CBool(mFinalizedForApproval) && CBool(mSavedAsFinal)){
                                NewMessageBoxClose();
                                setTimeout(function(){
                                    NewMessageBox('Do you want to print this transaction?', 'Print?', [
                                    {
                                        Text: 'Yes',
                                        Click: function() {
                                            HandlePrint({
                                                NewTransAfterPrint: true
                                            });
                                        }
                                    },
                                    {
                                        Text: 'No'
                                    }
                                    ]);
                                }, 500);                                
                            }
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
                Fail: function(vMessage) {
                    Bs.Loading('hide');
                    NewMessageBox('Mobiz cannot save your current transaction because: ' + vMessage);
                }
            });
        }

        var mTxtQuotationNumText = mElement.find('[name=txtQuotationNumText]');
        var mTxtQuoteNumber = mElement.find('[name=txtQuoteNumber]');
        mTxtQuoteNumber.click(SearchQuotes);
        mTxtQuoteNumber.on('keypress', function(e) {
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
                AfterLoad: function(vData) {
                    mOut.Model.Data.CopyProductsFromInvoiceID = vData.ID;
                    mOut.Model.Data.QuotationID = vData.ID;
                    mOut.Model.Data.ID = null;
                    PreSave({
                        SaveACopy: true,
                        NoPrompt: true,
                        Success: function(vData) {
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
        mTxtQuoteDate.click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                Click: function(e){
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
                    AfterLoad: function(vData) {
                        mRelatedOrdersPopup.popup({
                            ID: vData.ID,
                            OnOk: function(vRelatedSalesOrderData) {
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
                                    Success: function(vData2) {
                                        LoadTrans({
                                            ID: vData2.ID
                                        });
                                    }
                                });
                            },
                            OnCancel: function() {
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
        mTxtPoDate.click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                Click: function(e){
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

        mElement.find('[name=butCopyProducts]').click(function(e) {
            e.preventDefault();
            if (!HandleEditingSavedAsFinal())
                return;

            mOut.Model.Verify({
                Success: function() {
                    if (mOut.TransType == 'PurchaseOrder') {
                        NewMessageBox('', '', [
                            {
                                Text: 'Load Products From Sales Order',
                                Click: function() {
                                    FindTransForProductsSource({TransType: 'SalesOrder'});
                                }
                            },
                            {
                                Text: 'Load Products From Purchase Order',
                                Click: function() {
                                    FindTransForProductsSource();
                                }
                            }
                        ]);
                    } else
                        FindTransForProductsSource();
                },
                Fail: function(vMessage) {
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
                Click: function(e) {
                    if (mOut.Model.Data.ID == null) {
                        Proceed();
                        return;
                    }

                    mOut.Model.GetCommonLineItemCount({
                        InvoiceID1: mOut.Model.Data.ID,
                        InvoiceID2: e.ID,
                        success: function(vData) {
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
                                        Click: function() {
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
                            Success: function(vData) {
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

        mElement.find('[name=butViewOrderHistory]').click(function(e) {
            e.preventDefault();
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/OrderProcessing/orderhistory/controller.js?v=' + assetversion], function() {;
                Bs.Loading('hide');
                if (!IsGuid(mOut.Model.Data.CustomerID)){
                    NewMessageBox('Please select customer first', 'No customer');
                    return
                }
                if (!HandleEditingSavedAsFinal())
                    return;

                if (CStr2(mOut.Model.Data.ID) == '') {
                    SaveAsDraft({
                        NoPrompt: true,
                        Success: CallOrderHistoryPopup
                    });
                } else
                    CallOrderHistoryPopup();

                function CallOrderHistoryPopup() {
                    ERP.OrderHistory.main.controller.popup({
                        CustomerName: mCustomerName,
                        CustomerID: mOut.Model.Data.CustomerID,
                        TransID: mOut.Model.Data.ID,
                        success: function(){
                            LoadTrans({
                                ID: mOut.Model.Data.ID,
                                LoadProductsOnly: true
                            });
                        }
                    });
                }
            });
        });

        mElement.find('[name=butAddProduct]').click(function(e) {
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

        mElement.find('[name=butAddProductFromSupplier]').click(function(e) {
            e.preventDefault();
            if (CStr2(mOut.Model.Data.ID) == '') {
                SaveAsDraft({
                    NoPrompt: true,
                    Success: function() {
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

        function HandleAllowEncodingOfQuantitiesInInvoice() {
            if ((mOut.TransType == 'Invoice')
                && (!ERP.Invoicing.settings.controller.Model.Data.AllowEncodingOfQuantitiesInInvoice)) {

                NewMessageBox('The current setting does not allow direct invocing. All orders need to be initiated by a sales order transaction first.');
                return false;
            }
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

        var mAddProductParams;
        function AddProduct(vParams) {
            vParams = $.extend({
                FilterSupplierID: null,
                AutoSearch: false,
                AllowEmptySearch: false,
                ShowInventoryColumnsQuantityOnHand: false,
                ShowInventoryColumnsQuantityCommitted: false,
                ShowInventoryColumnsQuantityAvailableToCommit: false
            }, vParams);
            mAddProductParams = vParams;
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
                    Render: function(v1, v2, vData) {
                        return '<div style="text-align: center">' + CStr2(vData.ProductNo) + '</div>';
                    }
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name',
                    CreatedCell: function(td, cellData, rowData, row, col){
                        $(td).css('min-width', '200px');
                    },
                    Render: function(v1, v2, vData) {
                        return '<div style="text-align: center">' + CStr2(vData.ProductName) + '</div>';
                    }
                }
            ];
            var pAdditionalData = {
                //IsAll:1, // commented on 2024_06_20__16_15_56_1273
                UseCacheProductSearch: 0,
                InventoryFowstID: sessionStorage.FowstID,
                ShowInventoryColumns: vParams.ShowInventoryColumnsQuantityOnHand || vParams.ShowInventoryColumnsQuantityCommitted || vParams.ShowInventoryColumnsQuantityAvailableToCommit,
                FilterSupplierID: vParams.FilterSupplierID,
                IncludeBaseUnit: 1
            };

            
            if (mOut.TransType == 'Invoice')
                pColumns = pColumns.concat([
                    {
                        Label: 'Served Qty',
                        Render: function(v1, v2, vData) {
                            return '<div name="divQty" />';
                        }, DontCopyToPlainText: true
                    }
                ]);
            else
                pColumns = pColumns.concat([
                {
                    Label: 'Qty',
                    Render: function(v1, v2, vData) {
                        return '<div name="divQty" />';
                        //return '<input class="qty" style="width:70px" />';
                    }, DontCopyToPlainText: true
                }
            ]);
            if (mOut.TransType == 'PurchaseOrder')
                pColumns = pColumns.concat([
                    {
                        Label: 'PO Price',
                        Render: function(v1, v2, vData) {
                            return '<div name="divPoPrice" />';
                            //return '<input class="qty" style="width:70px" />';
                        },
                        DontCopyToPlainText: true
                    }
                ]);

            pColumns.push({
                Label: 'Packing Size (in smallest unit)',
                Render: function(v1, v2, vData) {
                    return '<div style="text-align: center">' + CStr2(vData.UnitMeasure) + '</div>';
                }
            });
            pColumns.push({
                Label: 'Price Level',
                Column: null,
                CreatedCell: function(td, cellData, rowData, row, col){
                    var inp = $('<select data-role="none" class="form-select selPriceLevel">')
                                .append(new Option('Retail Price', '01000012-0005-0000-0000-000000000000'))
                                .data('data', rowData)
                                .click(function(e){
                                    e.stopPropagation();
                                    var dis = $(this), data = dis.data('data');

                                    if (!CBool(dis.attr('isloaded'))){
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: '/Stock/PriceTable',
                                            data: {ProductID: data.ID, CustomerID: mOut.Model.Data.CustomerID },
                                            success: function (e){
                                                Bs.Loading('hide');
                                                var vRows = e.Rows;

                                                for (var i = 0 in vRows){
                                                    if (dis.find('[value="'+ vRows[i].PriceLevelID +'"]').length == 0){
                                                        dis.append(new Option(vRows[i].PriceLevel, vRows[i].PriceLevelID))
                                                    }
                                                }

                                                dis.attr('isloaded', 1)
                                            }
                                        })
                                    }
                                })

                    $(td)
                        .css('min-width', '100px')
                        .click(function(e){
                            e.stopPropagation();
                        })
                        .html(inp)
                }
            });

            if (mTargetOrgType == 'vendor') {
                pAdditionalData.VendorID = mOut.Model.Data.CustomerID;
                pColumns.push({
                    Label: 'Vendor Price' ,
                    Column: function(vData) {
                        var pStr = AccountingFormat(vData.VendorPrice, 4);
                        if (IsDate(vData.VendorPriceLastUpdated))
                            pStr += '<br />' + FormatDate(vData.VendorPriceLastUpdated, {Format: 'M/d/yyyy'});
                        return pStr;
                    }
                });
            } else {
                pAdditionalData.FilterCustomers = 1;
                pColumns.push({
                    Label: 'Retail Price' ,
                    Column: function(vData) {
                        var pStr = '<b>' + AccountingFormat(vData.RetailPrice * (1 + mUnitPriceSurchargePercent / 100), 4) + '</b>';
                        if (IsDate(vData.RetailPriceLastUpdated)) {
                            pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.RetailPriceLastUpdated, { Format: 'M/d/yyyy' });
                            pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.RetailPriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                        }
                        pStr += '<br /><a name="lnkRequestPriceUpdate" href="#">Request Price Update</a>';
                        pStr = '<div style="text-align: center">' + pStr + '</div>';

                        return pStr;
                    }
                });
                pColumns.push({
                    Label: 'Wholesale Price' ,
                    Column: function(vData) {
                        var pStr = '<b>' + AccountingFormat(vData.WholesalePrice * (1 + mUnitPriceSurchargePercent / 100), 4) + '</b>';
                        if (IsDate(vData.WholesalePriceLastUpdated)) {
                            pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.WholesalePriceLastUpdated, { Format: 'M/d/yyyy' });
                            pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.WholesalePriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                        }
                        pStr += '<br /><a name="lnkRequestPriceUpdate" href="#">Request Price Update</a>';
                        pStr = '<div style="text-align: center">' + pStr + '</div>';
                        return pStr;
                    }
                });
            }

            pAdditionalData.LastInvCount = 1;
            pColumns.push({
                Label: 'Last Count Date / Counted By',
                Render: function(v1, v2, vData) {
                    return (CStr2(vData.LastInventoryCountDate) == '' ? '' : $.format.date(new Date(vData.LastInventoryCountDate), 'MM-dd-yyyy h:mm a') + ' / ' + vData.LastInventoryCountBy);
                }
            });
            pColumns.push({
                Label: 'Qty On Hand',
                Render: function(v1, v2, vData) {
                    if (vParams.ShowInventoryColumnsQuantityOnHand) {
                        var p = $.format.number(CDec2(vData.QtyOnHand), '#,###.##');
                        p = '<div style="text-align: center">' + p + '</div>';
                        return p;
                    } else 
                        return '<div style="text-align: center"><a name="lnkShowInventoryColumns" href="#">Click to Show</a></div>';
                }, DontCopyToPlainText: true
            });
            pColumns.push({
                Label: 'Qty Committed',
                Render: function(v1, v2, vData) {
                    if (vParams.ShowInventoryColumnsQuantityCommitted) {
                        var p = $.format.number(CDec2(vData.CommittedQty), '#,###.##');
                        p = '<div style="text-align: center">' + p + '</div>';
                        return p;
                    } else
                        return '<div style="text-align: center"><a name="lnkShowInventoryColumns" href="#">Click to Show</a></div>';

                }, DontCopyToPlainText: true
            });
            pColumns.push({
                Label: 'Qty Available to Commit',
                Render: function(v1, v2, vData) {
                    if (vParams.ShowInventoryColumnsQuantityAvailableToCommit) {
                        var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
                        p = '<div style="text-align: center">' + p + '</div>';
                        return p;
                    } else
                        return '<div style="text-align: center"><a name="lnkShowInventoryColumns" href="#">Click to Show</a></div>';
                }, DontCopyToPlainText: true
            });
            pColumns.push(
                {
                    Label: 'Action',
                    Render: function(v1, v2, vData) {
                        return '<div name="divAction"></div>';
                    }, DontCopyToPlainText: true
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
                        ModalSize: 'fullwidth',
                        AutoSearch: vParams.AutoSearch,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        CopyText: true,
                        DontCloseWhenClick: true,
                        ShowPic: true,
                        Search: vParams.Search,
                        Click: function (e) {
                            if (!e.FromCacheProductSearch)
                                CacheProductSearch();
                            if ($.isArray(e)) {
                                PreSelect(e);
                            } else {
                                SelectProduct(e);
                            }
                        },
                        AllowEmptySearch: vParams.AllowEmptySearch,
                        StyleFormat: 'table',
                        InitComplete: function(vTable) {
                            vTable.find('.qty').click(function(e) {
                                e.stopPropagation();
                            });
                        },
                        rowCallback: function(row, data, dataIndex){
                            var pDiv = $(row).find('[name=divAction]');
                            if (pDiv.children().length > 0)
                                return;
                            var pLink = $('<a>')
                                .text('Info')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    SelectProduct(data);

                                    //require(['promise!/Scripts/sources/ERP/InventoryBalance/controller.js?v=' + assetversion], function() {
                                    //    if (mInventoryBalance == null)
                                    //        mInventoryBalance = new ERP.InventoryBalance.controller();

                                    //    mInventoryBalance.Show({
                                    //        ProductID: data.ID,
                                    //        ProductNo: data.ProductNo,
                                    //        ProductName: data.ProductName,
                                    //        OrderController: mOut,
                                    //        AfterSave: function() {
                                    //            LoadTrans({
                                    //                ID: mOut.Model.Data.ID,
                                    //                LoadProductsOnly: true
                                    //            });
                                    //        }
                                    //    });
                                    //});
                                    return false;
                                });
                            pDiv.append(pLink);
                            pDiv = $(row).find('[name=divQty]');
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            var pDisabled = '';
                            // commented on 2021_09_02__09_42_32_5538
                            //if (CDec2(data.RetailPrice) == 0)
                            //    pDisabled = ' disabled ';
                            pDiv.append('<input type=number min=0 oninput="this.value = Math.abs(this.value)" class="qty form-control" ' + pDisabled + ' style="width:70px" />');

                            pDiv = $(row).find('[name=divPoPrice]');
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            var pVal = CDec2(data.VendorPrice);
                            if (pVal != 0) {
                                // pVal = AccountingFormat(pVal, 4); // do not format kay dli pwede ang comma sa textbox 2023_09_29__16_00_49_7805
                            } else
                                pVal = '';
                            pDiv.append('<input type=number min=0 onblur="this.value = Math.abs(this.value)" class="PoPrice form-control" style="width:90px; text-align:right" value="' + pVal + '" />');

                            $(row).find('[name=lnkRequestPriceUpdate]').click(function(e) {
                                e.stopPropagation();
                                PriceUpdateRequest(data);
                            });

                            $(row).find('[name=lnkShowInventoryColumns]').click(function(e) {
                                e.stopPropagation();
                                PopupShowInventoryColumns();
                            });
                        }
                    }
                ], pAdditionalData
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function PopupShowInventoryColumns() {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/ShowInventoryColumns/controller.js?v=' + assetversion], function() {
                Bs.Loading('hide');
                ERP.ShowInventoryColumns.controller.Show({
                    OnOk: PopupShowInventoryColumnsOnOk
                });
            });
        }

        function PopupShowInventoryColumnsOnOk(vData) {
            mAddProductParams.ShowInventoryColumnsQuantityOnHand = vData.QuantityOnHand;
            mAddProductParams.ShowInventoryColumnsQuantityCommitted = vData.QuantityCommitted;
            mAddProductParams.ShowInventoryColumnsQuantityAvailableToCommit = vData.QuantityAvailableToCommit;
            mAddProductParams.Search = $('#txtBrowseLibrary').val();
            mAddProductParams.AutoSearch = true;
            AddProduct(mAddProductParams);
        }

        function PriceUpdateRequest(vData) {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/OrderProcessing/priceUpdateRequest/controller.js?v=' + assetversion], function() {
                Bs.Loading('hide');
                ERP.OrderProcessing.priceUpdateRequest.controller.popup(vData);
            });
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
                    var pLineItem = {
                        ID: pDR.ID,
                        Qty: pQty,
                        PriceLevelID: pTxt.closest('tr').find('.selPriceLevel option:selected').val()
                    };
                    if (mOut.TransType == 'PurchaseOrder') {
                        pTxt = pTR.find('.PoPrice');
                        if (pTxt.length == 1) {
                            var pPrice = pTxt.val();
                            if (IsNumeric(pPrice))
                                pLineItem.UnitPrice = CDec2(pPrice);
                        }
                    } else if (mUnitPriceSurchargePercent > 0) {
                        pLineItem.UnitPriceSurchargePercent = mUnitPriceSurchargePercent;
                        // pLineItem.UnitPrice = CDec2(pDR.RetailPrice) * (1 + mUnitPriceSurchargePercent / 100);
                    }

                    pData.push(pLineItem);
                } // todo: if qty = 0, delete at server
            }
            
            if (pData.length > 0) {
                if (proj.IsBootstrapDesign()){
                    Bs.Loading('show');
                } else {
                    $.mobile.showPageLoadingMsg();
                }
                function Proceed() {
                    var pParams = $.extend({
                        Fail: function(vError) {
                    	    $.mobile.hidePageLoadingMsg();
                            NewMessageBox(vError);
                            AfterProductSave();
                        },
                        Success: function(vResult) {
                            if (pBelowGpmThreshold == false)
                                pBelowGpmThreshold = vResult.BelowGpmThreshold;
                            pIndex++;
                            if (pIndex <= pData.length - 1)
                                Proceed();
                            else {
                                if (proj.IsBootstrapDesign()){
                                    Bs.Loading('hide');
                                } else {
                                    $.mobile.hidePageLoadingMsg();
                                }                    	        
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

        mElement.find('[name=butAddSalesRep]').click(function() {
            //if (!HandleEditingSavedAsFinal())
            //    return;

            PromptUserAccess('Allow Edit of Sales Rep Field', {
                OnClickOKSuccess: function () {
                    Proceed();
                },
                OnFailed: function () {
                }
            });

            function Proceed() {
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
                        Click: function() {
                            AddSalesRep({
                                Name: mTxtSalesRepText.val().substring(0, 500),
                                Success: function(vData) {
                                    //mOut.Model.Data.BillingAddressID = vData.ID;
                                }
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function() {
                        
                        }
                    }
                ]);
            }
        });

        function AddSalesRep(vParams) {
            if (mOut.TransType == 'PurchaseOrder') {
                mOut.Model.SaveContactPerson({
                    LastName: vParams.Name,
                    OrgID: mOut.Model.Data.CustomerID,
                    Success: function(vData) { 
                        NewMessageBox('Sales rep was successfully added.');
                        vParams.Success(vData);
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            } else {
                mOut.Model.AddSalesRep({
                    Name: vParams.Name,
                    Success: function(vData) { 
                        NewMessageBox('Sales rep was successfully added.');
                        vParams.Success(vData);
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            }
        }

        function SavePreSelected (vParams) {
            var pData = {
                CustomerID: mOut.Model.Data.CustomerID,
                TransType: mOut.TransType,
                DeleteExistingProduct: 0,
                DontCheckCartValidity: 1,
                OrderID: mOut.Model.Data.ID,
                NewCartMode: !IsGuid(mOut.Model.Data.ID),
                StockID: vParams.ID,
                SalesRep: [],
                ProductSerial: [],
                UpdateOrderedQty: 1,
                UnitPriceSurchargePercent: vParams.UnitPriceSurchargePercent,
                Stocks: [
                    {
                        Qty: vParams.Qty,
                        UnitQty: 1,
                        EditPrice: vParams.UnitPrice,
                        PriceLevelID: vParams.PriceLevelID
                    }
                ]
            };

            if (!IsGuid(pData.CustomerID)) {
                NewMessageBox('Invalid/missing CustomerID: ' + pData.CustomerID + '. This known bug is still unresolved. Workaround: reload this page and try again. ref: jsFKWML635AP');
                console.trace();
                return;
            }

            AjaxSend({
                url: '/Stock/UpdatePos',
                data: pData,
                NoSplash: true,
                success: function(vData) {
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
                UnitPriceSurchargePercent: mUnitPriceSurchargePercent,
                Success: function() {
                    
                }
            });
        }

        function Item_Click(vProductID, vDR) {
            var pAllowEdit = HandleEditingSavedAsFinal({NoPrompt: true});
            vDR = $.extend({}, vDR);

            require(['promise!/Scripts/sources/ERP/InventoryBalance/controller.js?v=' + assetversion], function() {
                if (mInventoryBalance == null)
                    mInventoryBalance = new ERP.InventoryBalance.controller();
                var pButtons = [];

                pButtons.push({
                    Text: 'Edit And View More Info For This Line Item',
                    Click: function() {
                        //pProductPopup.Model = new ERP.OrderProcessing.product.model();
                        pProductPopup.Model.Data.TransType = mOut.TransType;
                        //pProductPopup.AfterSave = AfterProductSave;

                        pProductPopup.popup({
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
                            Success: function() {
                            },
                            ReadOnly: !pAllowEdit
                        });
                    }
                });

                if (pAllowEdit) {
                    pButtons.push({
                        Text: 'Add New Line Item To This Cart',
                        Visible: mOut.TransType != 'AccountsPayable',
                        Click: function () {
                            AddProduct();
                        }
                    });

                    pButtons.push({
                        Text: 'Enter Serial Numbers and IMEIs',
                        Click: function() {
                            mSerialNumbersPopup.popup({
                                InvoiceID: mOut.Model.Data.ID,
                                InventoryBalance: mInventoryBalance,
                                ProductID: vProductID,
                                ProductName: vDR.ProductName,
                                ProductNo: vDR.ProductNo,
                                CustomerID: mOut.Model.Data.CustomerID,
                                TransType: mOut.TransType,
                                ParentController: mOut,
                                Success: function() {
                                }
                            });
                        }
                    });

                    if (mOut.TransType != 'AccountsPayable')
                        pButtons.push(
                            {
                                Text: 'Remove This Line Item From Cart',
                                Click: function() {
                                    RemoveFromCartPrompt(vProductID, vDR);
                                }
                            }
                        );
                    if (mOut.TransType != 'Purchaseorder')
                        pButtons.push(
                            {
                                Text: 'Remove This Line Item From Cart and Return SKU to Item Requisition List',
                                Click: function() {
                                    RemoveFromCartPrompt(vProductID, vDR, true, 'po');
                                }
                            }
                        );
                } // END if (pAllowEdit) {
                pButtons.push(
                    //{
                    //    Text: 'View This Product\'s Information',
                    //    Click: function() {
                    //        NewMessageBox('This function is not yet implemented. ref: jsY7TDHS9SS3!');
                    //    }
                    //},
                    {
                        Text: 'View This Product\'s Inventory Balance(s)',
                        Visible: true,
                        Click: function() {
                            mInventoryBalance.Show({
                                ProductID: vProductID,
                                ProductNo: vDR.ProductNo,
                                ProductName: vDR.ProductName,
                                OrderController: mOut,
                                AfterSave: function() {
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
                            Click: function() {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'LastPurchaseOrderPrice');
                            }
                        },
                        {
                            Text: 'Set Unit Price To Last Accounts Payable (AP) Price',
                            Click: function() {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'LastAccountsPayablePrice');
                            }
                        },
                        {
                            Text: 'Set Unit Price To AP or PO Price, Whichever Is Lower',
                            Click: function() {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'ApOrPoWhicheverIsLower');
                            }
                        },
                        {
                            Text: 'Set Unit Price To AP or PO Price, Whichever Is Newer',
                            Click: function() {
                                SetLineItemUnitPrice(vDR.ProductID, vDR.ID, 'ApOrPoWhicheverIsNewer');
                            }
                        }
                    ]);
                }
                PopupButtonList('', vDR.ProductName, pButtons);
            });
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
                success: function(vData) {
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

        function RemoveFromCartPrompt(vProductID, vDR, vAddToItemRequisition, vPosMode) {
            if (!HandleFromSalesOrder())
                return;
            NewMessageBox('Are you sure you want to remove ' + vDR.ProductName + ' from cart?', 'Remove from cart', [
                {
                    Text: 'Yes',
                    Click: function() {
                        if (mOut.TransType == 'AccountsPayable')
                            AjaxSend({
                                url: '/SalesMenu/HideServedQuantity',
                                data: {
                                    InvoiceItemID: vDR.ID,
                                    HideQuantity: true
                                },
                                success: function(vData) {
                                    if (vData.Result == 'ok') {
                                        // NewMessageBox('Line item was successfully hidden.'); // hide "success" result dialog 2018_03_02__09_19_11_5219
                                        LoadTrans({
                                            ID: mOut.Model.Data.ID,
                                            LoadProductsOnly: true
                                        });
                                    } else {
                                        NewMessageBox('Failed to hide line item.');
                                    }
                                },
                                error: function(vError) {
                                    NewMessageBox(vError);
                                }
                            });
                        else
                            mOut.Model.RemoveProduct({
                                ProductID: vProductID,
                                UnitMeasure: vDR.UnitMeasure,
                                ItemID: vDR.ID,
                                AddToItemRequisition: vAddToItemRequisition,
                                PosMode: vPosMode,
                                Success: function() {
                                    LoadTrans({
                                        ID: mOut.Model.Data.ID,
                                        LoadProductsOnly: true
                                    });
                                },
                                Fail: function(vMessage) {
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

        mElement.find('[name=butLookup]').click(function() {
            
            if (!$.isEmptyObject(mOut.Model.Data.CustomerID)) {
                var pMsg = 'Loading another record will discard the unsaved changes from the current record. Do still want to search for another record?';
                NewMessageBox(pMsg, '', [
                    {
                        Text: 'Yes',
                        Click: function() {
                            Find({
                                TransType: mOut.TransType
                            });
                        }
                    },
                    {
                        Text: 'No',
                        Click: function() {
                        }
                    }
                ]);
            } else
                Find({
                    TransType: mOut.TransType
                });
        });

        function Find(vParams) {
            var pTitle;
            var pColumns = [];
            var pShowOkButton = false;

            if (vParams.TransType == 'Quote') {
                pTitle = 'Quotations';
                pColumns = [
                    {
                        Label: 'Quotation No.',
                        Column: '',
                        Column: function(vDR) {
                            var pOut = CStr2(vDR.MIN).trim();
                            if (pOut != '')
                                pOut += '-';
                            pOut += vDR.QuotationNo;
                            
                            return '<a href="#">' + pOut + '</a>';
                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    PeekView({
                                        ID: $(this).data('data').ID,
                                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                                        TransType: vParams.TransType
                                    });
                                })
                        }
                    },
                    {
                        Label: 'Customer',
                        Column: 'CompanyName'
                    },
                    {
                        Label: 'Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total, 4);
                        }
                    }
                ];
            } else if (vParams.TransType == 'SalesOrder') {
                pTitle = 'Sales Order';
                pColumns = [
                    {
                        Label: 'Sales Order No.',
                        Column: function(vDR) {
                            var pOut = CStr2(vDR.MIN).trim();
                            if (pOut != '')
                                pOut = vDR.MIN + '-';
                            if (vDR.TransType == 'StockTransfer')
                                pOut = 'STR-' + pOut + vDR.StrNum;
                            else if (CDec2(vDR.PurchaseOrderNo) > 0)
                                pOut = 'PO-' + pOut + vDR.PurchaseOrderNo;
                            else
                                pOut = 'SO-' + pOut + vDR.SalesOrderNo;
                            
                            return '<a href="#">' + pOut + '</a>';
                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    PeekView({
                                        ID: $(this).data('data').ID,
                                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                                        TransType: vParams.TransType
                                    });
                                })
                        }
                    },
                    {
                        Label: 'Parent Sales Order No.',
                        Column: function(vDR) {
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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            return AccountingFormat(vDR.OrderedAmount, 4);
                        }
                    },
                    {
                        Label: 'Served Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.ServedAmount, 4);
                        }
                    }
                ];
            } else if (vParams.TransType == 'Invoice') {
                pTitle = 'Invoice';
                pColumns = [
                    {
                        Label: 'Invoice No.',
                        Column: function(vDR) {
                            var pOut = '';
                            if (CStr2(vDR.FowstWideInvoiceNumber).trim() != '')
                                pOut = vDR.FOWSTNo + '-' + vDR.FowstWideInvoiceNumber;
                            else if (CStr2(vDR.MIN).trim() != '') 
                                pOut = vDR.MIN + '-' + vDR.InvoiceNumber;
                            else
                                pOut = vDR.InvoiceNumber;
                            if (CStr2(vDR.DocumentName).trim() == '')
                                return '<a href="#">' + 'SI-' + pOut + '</a>';
                            else
                                return '<a href="#">' + vDR.DocumentName + '-' + pOut + '</a>';
                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    PeekView({
                                        ID: $(this).data('data').ID,
                                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                                        TransType: vParams.TransType
                                    });
                                })
                        }
                    },
                    {
                        Label: 'Sales Order No.',
                        Column: function(vDR) {
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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            return AccountingFormat(vDR.OrderedAmount, 4);
                        }
                    },
                    {
                        Label: 'Served Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.ServedAmount, 4);
                        }
                    }
                ];
            } else if (vParams.TransType == 'PurchaseOrder') {
                pTitle = 'Purchase Order';
                pShowOkButton = true;
                pColumns = [
                    {
                        Label: 'Merge',
                        Render: function(v1, v2, vData) {
                            return '<div class=class=form-check style="text-align: center;"><input type=checkbox class=form-check-input name=chkMerge></div>';
                        },
                        className: 'dt-head-body-center'
                    },
                    {
                        Label: 'PO No.',
                        Column: function(vDR) {
                            var pOut = '';
                            if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-';
                            if (!IsNumeric(vDR.PurchaseOrderNo))
                                vDR.PurchaseOrderNo = 0;
                            return '<a href="#">' + 'PO-' + pOut + vDR.PurchaseOrderNo + '</a>';
                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    PeekView({
                                        ID: $(this).data('data').ID,
                                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                                        TransType: vParams.TransType
                                    });
                                })
                        }
                    },
                    {
                        Label: 'Vendor',
                        Column: 'CompanyName'
                    },
                    {
                        Label: 'Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total, 4);
                        }
                    },
                    {
                        Label: 'Date Approved',
                        Column: 'ApprovedDate'
                    },
                    {
                        Label: 'Received Percent',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.PoReceivedPercent, 4);
                        }
                    },
                    {
                        Label: 'Internal Remarks',
                        Column: 'InternalRemarks'
                    },
                ];
            } else if (vParams.TransType == 'Canvass') {
                pTitle = 'Price Canvass';
                pColumns = [
                    {
                        Label: 'PC No.',
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total, 4);
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
                || (vParams.TransType == 'SalesOrder')){
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
                    if ((mOut.TransType == 'PurchaseOrder')
                        && $.isArray(e)) {

                        HandleMergePurchaseOrder();
                        return;
                    }
                    LoadTrans({
                        ID: e.ID,
                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                        AfterLoad: function() {
                            if (vParams.AfterLoad)
                                vParams.AfterLoad(e);
                        } // END AfterLoad: function() {
                    });
                }; // END Click: function (e) {

            function HandleMergePurchaseOrder() {
                var pChks = $('#tblBrowsePopup [name=chkMerge]:checked');
                if (pChks.length <= 1) {
                    NewMessageBox('You must select at least 2 transactions to merge.', 'Invalid user input');
                    return;
                }

                NewMessageBox('Are you sure you want to merge the selected purchase orders?', 'Merge Purchase Orders', [
                    {
                        Text: 'Yes',
                        Click: Proceed
                    },
                    {
                        Text: 'No'
                    }
                ]);

                function Proceed() {
                    var pData = [];
                    for (var pChk of pChks.toArray()) {
                        pChk = $(pChk);
                        var pTR = pChk.closest('tr');
                        var pDR = pTR.data('data');
                        pData.push(pDR);
                    }
                    AjaxSend({
                        url: '/ERP/MergePurchaseOrders',
                        data: {
                            PurchaseOrders: pData
                        },
                        success: function(vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error, 'Fail');
                                return;
                            } else if (vData.Result == 'ok')
                                NewMessageBox('The selected purchase orders were successfully merged.', 'Success');
                        }
                    });
                }
            }
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
                        ModalSize: 'fullwidth',
                        StyleFormat: 'table',
                        AutoSearch: false,
                        DisplayText: pColumns,
                        Click: vParams.Click,
                        ShowOkButton: pShowOkButton,
                        Validation: vParams.Validation,
                        ShowHideChildSos: pShowHideChildSos,
                        //ShowLastActionTakenFilter: mOut.TransType == 'PurchaseOrder',
                        ShowRecordStatusAsDropDown: mOut.TransType == 'PurchaseOrder',
                        ShowDraftFinalizedFilter: mOut.TransType == 'PurchaseOrder',
                        ShowApprovedFilter: mOut.TransType == 'PurchaseOrder',
                        InitComplete: function(vTable) {
                            //vTable.find('input').click(function(e) {
                            //    e.stopPropagation();
                            //});
                        },
                        rowCallback: function(row, data, dataIndex) {
                            $(row).find('input').click(function(e) {
                                e.stopPropagation();
                            });
                            if (vParams.TransType == 'PurchaseOrder') {
                                if (CInt2(data.PurchaseOrderNo) != 0)
                                    $(row).find('[name=chkMerge]').remove();
                            }

                            var pCell = $(row).find('[name=divAction]');
                            if (pCell.length == 0)
                                return;
                            var pLink = $('<a>')
                                .text('Preview')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    var win = window.open(location.origin + '/erp' + location.search + '&IsReadOnly=1&IsPreview=1&ID=' + data.ID,
                                        '_blank',
                                        'width=' + screen.availWidth + ',height=' + screen.availHeight);

                                    win.onload = function () {
                                        function LoadRecord() {
                                            if (vParams.TransType == 'SalesOrder'){
                                                win.location.href = '/Erp/OrderProcessing' + win.location.search;
                                            } else if (vParams.TransType == 'Invoice'){
                                                win.location.href = '/Erp/Invoicing' + win.location.search;
                                            }
                                        }
                                        LoadRecord();    
                                    }
                        
                                    return false; // para DILI magclose ang popup
                                });
                            pCell.append(pLink);
                        },
                        Sorting: true,
                        NoInitialSorting: true
                    }
                ], $.extend({ 
                    TransType: vParams.TransType,
                    AnyZero: 1,
                    AllowNoItems: true,
                    LimitSearchFields: true
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
                IncludeZeroQty: vParams.IncludeZeroQty,
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost
            }, vParams);
            mOut.Model.GetCart({
                ID: vParams.ID,
                IncludeZeroQty: vParams.IncludeZeroQty,
                IncludeSuppliers: mOut.ShowProductSuppliersColumn,
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                IncludeChildOrderItems: pIncludeChildOrderItems,
                Success: function(vData) {
                    mFinalizedForApproval = CBool(vData.Master.FinalizedForApproval);
                    if (!mFinalizedForApproval
                        && (vData.Master.TransType == 'PurchaseOrder')
                        && (CInt2(vData.Master.PurchaseOrderNo) > 0)
                        && (CStr2(vData.Master.ApprovedDate) == '')) {

                        mFinalizedForApproval = true;
                        vData.Master.FinalizedForApproval = true;
                    }
                    if (mOut.Model.Data.TransType != 'PurchaseOrder') {
                        mUnapprovedProfitability = CBool(vData.Master.UnapprovedProfitability);
                        mHasCnCUnapprovedReason = (CStr2(vData.Master.CnCUnapprovedReason).trim() != '')
                            && (CStr2(vData.Master.CncApprovedByID) == '');
                    }
                    if (!vParams.LoadProductsOnly)
                        DisplayTrans(vData, {
                            DontReplaceCurrentID: vParams.DontReplaceCurrentID
                        });
                    DisplayProducts(vData);
                    if (vParams.AfterLoad)
                        vParams.AfterLoad(vData);
                    if (mOut.Model.Data.TransType == 'AccountsPayable' || mOut.Model.Data.TransType == 'PurchaseOrder'){
                        ERP.InvoiceImage.main.controller.GetInvoiceRefImages(function(e){
                            var vRows = e.Results.Rows;
                            if (vRows.length > 0) {
                                mElement.find('[name="txtRefInvoiceNumber"]').val(vRows.map(function (elem) {
                                    return elem.InvoiceRefNo;
                                }).join(', '));
                            } else {
                                mElement.find('[name="txtRefInvoiceNumber"]').val('');
                            }
                        }, {
                            mod: mOut.TransType,
                            ID: mOut.Model.Data.ID
                        })
                    }
                }
            });
        }

        function PeekView(vParams){
            Bs.Loading('show');
            vParams = $.extend({
                ID: null,
                LoadProductsOnly: false,
                DontReplaceCurrentID: false,
                IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                AccountsPayableFollowCost: CBool(vParams.AccountsPayableFollowCost)
            }, vParams);
            mOut.Model.GetCart({
                ID: vParams.ID,
                IncludeZeroQty: vParams.IncludeZeroQty,
                IncludeSuppliers: mOut.ShowProductSuppliersColumn,
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                IncludeChildOrderItems: pIncludeChildOrderItems,
                ShowBackOrderItems: vParams.TransType == 'Invoice',
                Success: function (vData) {
                    Bs.Loading('hide');
                    var msg = CopyTransToPlainText(vParams.TransType, vData, true);
                    NewMessageBox(msg.replace(/(?:\r\n|\r|\n)/g, '<br>'), 'Peek View');
                }
            });
        }

        var mCustomerRequirePoNum; // bool

        function DisplayTrans(vData, vParams) {
            var pDate;
            var pStr;

            InitInvoiceTypes();
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
            mCustomerRequirePoNum = CBool(vData.Master.CustomerRequirePoNum);
            mCustomerName = vData.Master.CustomerName;
            mTxtCustomer.val(mCustomerName);
            mOut.Model.Data.QuotationID = vData.Master.QuotationID;
            mOut.Model.Data.SalesOrderID = vData.Master.SalesOrderID;
            mTxtQuotationNumText.val(vData.Master.QuotationNumText);
            pStr = CStr2(vData.Master.MIN).trim();
            if (pStr != '')
                pStr += '-';
            if (mOut.TransType == 'Quote') {
                mTxtQuoteNumber.val(pStr + CInt2(vData.Master.QuotationNo));
                pDate = vData.Master.QuotationDate;
                DisplayTransDate(pDate);
            } else {
                mTxtQuoteNumber.val(vData.Master.RefQuotationNo || pStr + CInt2(vData.Master.QuotationNo));
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
                pStr += CStr2(vData.Master.RefSalesOrderNo);
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
                pStr = vData.Master.FOWSTNo + '-' +  vData.Master.FowstWideInvoiceNumber;
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
            mChkDontAutoInvoice.prop('checked', CBool(vData.Master.DontAutoInvoice));
            mTxtDateNeeded.val(FormatDate(vData.Master.DateNeeded, {DontDisplay12Am: true}));
            if ((mOut.TransType == 'PurchaseOrder') 
                || (mOut.TransType == 'AccountsPayable')) {

                pStr = vData.Master.MIN;
                if (CStr2(pStr).trim() != '')
                    pStr += '-';
                pStr += CInt2(vData.Master.PurchaseOrderNo);
                mTxtPoNumber.val(pStr);
                mMaxPaymentTerm = Math.max(CDec2(vData.Master.PaymentTerm), CDec2(vData.Master.SupplierCreditTerms));

                if (mOut.TransType == 'AccountsPayable') {
                    mTxtPoNumber.val(CStr2(vData.Master.AccountsPayableFullPoNum + ' (' + FormatDate(vData.Master.PurchaseOrderDate, {Format: 'MMM d, yyyy'}) + ')')); // AccountsPayablePurchaseOrderID
                }
            } else {
                mTxtPoNumber.val(vData.Master.PurchaseOrderText);
                mMaxPaymentTerm = CDec2(vData.Master.CreditTerms);
            }
            pDate = ParseDate(vData.Master.PurchaseOrderDate);
            if (pDate != null)
                pDate = $.format.date(pDate, 'M/d/yyyy');
            mTxtPoDate.val(pDate);
            mTxtRemarks.val(vData.Master.Remarks);
            mTxtInternalRemarks.val(CStr2(vData.Master.InternalRemarks));
            mTxtRefInvoiceNumber.val(vData.Master.RefInvoiceNum);
            mTxtCanvass.val(vData.Master.CanvassNumber); 
            mTxtSalesInvoiceRef.val(vData.Master.SalesInvoiceRef);
            mTxtOfficialReceiptRef.val(vData.Master.OfficialReceiptRef);

            mOut.Model.Data.CanvassID = vData.Master.CanvassID;
            mOut.Model.Data.BillingAddressID = vData.Master.BillingAddressID;
            mTxtBillingAddress.val(vData.Master.BillingAddress);
            mOut.Model.Data.DeliveryAddressID = vData.Master.DeliveryAddressID;
            mTxtDeliveryAddress.val(vData.Master.DeliveryAddress);
            mCbInvoiceType.find('option[value=' + vData.Master.DocumentNameID + ']').attr('selected', true);
            mCbDeliveryMethod.val(vData.Master.DeliveryMethod);

            mOut.Model.Data.CustomerFowstID = vData.Master.CustomerFowstID;
            mTxtCustomerFowst.val(vData.Master.CustomerFowst);
            mOut.Model.Data.CustomerBinID = vData.Master.CustomerBinID;
            mTxtCustomerBin.val(vData.Master.CustomerBin);
            mElement.find('[name=divLastUpdated]').text(FormatDate(vData.Master.LastUpdated, {Format: 'MMMM d, yyyy h:mm a'}));
            mOut.Model.Data.ParentOrderID = vData.Master.ParentOrderID;
            mOut.Model.Data.InvoiceFowstID = vData.Master.InvoiceFowstID;
            mTxtInvoiceFowst.val(vData.Master.InvoiceFowstName);
            mOut.Model.Data.StockTransferID = vData.Master.StockTransferID;
            
            // "Freight and Handling" tab
            mTxtShipper.val(vData.Master.ShipperName);
            mTxtWaybillNumber.val(vData.Master.WaybillNumber);
            if (CStr2(vData.Master.ShipDate).trim() != '')
                mTxtShipDate.val(FormatDate(vData.Master.ShipDate, {DontDisplay12Am: true}));
            mTxtGrossWeight.val(vData.Master.GrossWeight);
            mTxtPackageQuantity.val(vData.Master.PackageQuantity);
            mTxtGrossVolume.val(vData.Master.GrossVolume);
            mTxtFreight.val(AccountingFormat(vData.Master.Freight, null, ''));
            mTxtHandling.val(AccountingFormat(vData.Master.Handling, null, ''));
            mTxtArrastre.val(AccountingFormat(vData.Master.Arrastre, null, ''));
            mTxtOtherCharges.val(AccountingFormat(vData.Master.OtherCharges, null, ''));
            mTxtFreightAndHandling.val(AccountingFormat(vData.Master.FreightAndHandling, null, ''));

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
                    LoadArPayments(null, mOut.Model.Data.ID);
                    break;
                
                case 'Invoice':
                    var pDocName = vData.Master.DocumentName;
                    if (CStr2(pDocName).trim() == '')
                        pDocName = 'SI';
                    mElement.find('[name=divTransNum]').text(pDocName + '-' + mTxtInvoiceNumber.val());
                    LoadArPayments(mOut.Model.Data.ID);
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
                    LoadApPayments();
                    break;

            }
            mCbInvoiceType_change();
            LoadAccountsDetails();
        } // END DisplayTrans

        function LoadArPayments(vInvoiceID, vSalesOrderID) {

            AjaxSend({
                url: '/ERP/GetAccountsReceivablePaymentData',
                data: {
                    InvoiceID: vInvoiceID,
                    SalesOrderID: vSalesOrderID
                },
                success: function (vData) {
                    DisplayArPayments(vData);
                }
            });
        }

        function DisplayArPayments(vData) {
            var pDiv = $('#divPayments');
            pDiv.empty();
            if (vData.payments == null)
                return;
            for (var pPaymentIndex = 0; pPaymentIndex <= vData.payments.length - 1; pPaymentIndex++) {
                var pPayment = vData.payments[pPaymentIndex];
                pPayment.Master = pPayment.Master.Rows[0];
                var pTable = $('<table class="mb-3">');
                pTable.append($('<tr>')
                    .append($('<td>').text('Payment Date: '))
                    .append($('<td class="pl-2">').text(pPayment.Master.PaymentDate))
                ).append($('<tr>')
                    .append($('<td>').text('Receipt Number: '))
                    .append($('<td class="pl-2">').text(pPayment.Master.ReceiptNumber))
                ).append($('<tr>')
                    .append($('<td>').text('Total Paid Amount: '))
                    .append($('<td class="pl-2">').text(AccountingFormat(pPayment.Master.TotalPayment)))
                ).append($('<tr>')
                    .append($('<td>').text('Cash Amount: '))
                    .append($('<td class="pl-2">').text(AccountingFormat(pPayment.Master.CashAmount)))
                )
                pDiv.append(pTable);
                pDiv.append($('<div class="mb-3">').text('Invoices that were paid by this payment'));
                pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                var pTR = $('<tr>');
                pTR.append($('<th>').text('Invoice No.'));
                pTR.append($('<th>').text('Amount Paid'));
                pTable.append(pTR);

                for (var pInvoiceIndex = 0; pInvoiceIndex <= pPayment.Invoices.Rows.length - 1; pInvoiceIndex++) {
                    var pInvoice = pPayment.Invoices.Rows[pInvoiceIndex];
                    pTR = $('<tr>');
                    pTR.append($('<td>').text(pInvoice.PrintedInvoiceNumber));
                    pTR.append($('<td>').text(pInvoice.AmountPaid));
                    pTable.append(pTR);
                }
                pDiv.append(pTable);

                var pCheckCount = 0;
                for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                    var pCheck = pPayment.Checks.Rows[pCheckIndex];
                    if (!CBool(pCheck.IsOnlinePayment))
                        pCheckCount++;
                }
                if (pCheckCount > 0) {
                    pDiv.append($('<div class="mb-3 font-weight-bold">').text('Cheques'));
                    pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                    var pTR = $('<tr>');
                    pTR.append($('<th>').text('Account Name'));
                    pTR.append($('<th>').text('Account Number'));
                    pTR.append($('<th>').text('Bank Name'));
                    pTR.append($('<th>').text('Bank Branch'));
                    pTR.append($('<th>').text('Check Number'));
                    pTR.append($('<th>').text('Check Amount'));
                    pTable.append(pTR);
                    for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                        var pCheck = pPayment.Checks.Rows[pCheckIndex];
                        if (CBool(pCheck.IsOnlinePayment))
                            continue;
                        pTR = $('<tr>');
                        pTR.append($('<td>').text(pCheck.AccountName));
                        pTR.append($('<td>').text(pCheck.AccountNumber));
                        pTR.append($('<td>').text(pCheck.BankName));
                        pTR.append($('<td>').text(pCheck.BankBranchName));
                        pTR.append($('<td>').text(pCheck.CheckNumber));
                        pTR.append($('<td>').text(pCheck.CheckAmount));
                        pTable.append(pTR);
                    }
                    pDiv.append(pTable);
                }

                pCheckCount = 0;
                for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                    var pCheck = pPayment.Checks.Rows[pCheckIndex];
                    if (CBool(pCheck.IsOnlinePayment))
                        pCheckCount++;
                }
                if (pCheckCount > 0) {
                    pDiv.append($('<div class="mb-3 font-weight-bold">').text('Online Payment'));
                    pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                    var pTR = $('<tr>');
                    pTR.append($('<th>').text('From Account Name'));
                    pTR.append($('<th>').text('From Account Number'));
                    pTR.append($('<th>').text('From Bank Name'));
                    pTR.append($('<th>').text('From Bank Branch'));
                    pTR.append($('<th>').text('Online Transaction Number'));
                    pTR.append($('<th>').text('Online Transaction Amount'));
                    pTR.append($('<th>').text('To Account Name'));
                    pTR.append($('<th>').text('To Account Number'));
                    pTR.append($('<th>').text('To Bank Name'));
                    pTR.append($('<th>').text('To Bank Branch'));
                    pTable.append(pTR);
                    for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                        var pCheck = pPayment.Checks.Rows[pCheckIndex];
                        if (!CBool(pCheck.IsOnlinePayment))
                            continue;
                        pTR = $('<tr>');
                        pTR.append($('<td>').text(pCheck.AccountName));
                        pTR.append($('<td>').text(pCheck.AccountNumber));
                        pTR.append($('<td>').text(pCheck.BankName));
                        pTR.append($('<td>').text(pCheck.BankBranchName));
                        pTR.append($('<td>').text(pCheck.CheckNumber));
                        pTR.append($('<td>').text(pCheck.CheckAmount));
                        pTR.append($('<td>').text(pCheck.AccountName2));
                        pTR.append($('<td>').text(pCheck.AccountNumber2));
                        pTR.append($('<td>').text(pCheck.BankName2));
                        pTR.append($('<td>').text(pCheck.BankBranchName2));
                        pTable.append(pTR);
                    }
                    pDiv.append(pTable);
                }

                pDiv.append($('<hr>'));
            } // END for (var pPaymentIndex = 0; pPaymentIndex <= vData.payments.length - 1; pPaymentIndex++) {
        }

        function LoadApPayments() {

            AjaxSend({
                url: '/ERP/GetAccountsPayablePaymentData',
                data: {
                    AccountsPayableID: mOut.Model.Data.ID
                },
                success: function(vData) {
                    DisplayPayments(vData);
                }
            });
        }

        function DisplayPayments(vData) {
            var pDiv = $('#divPayments'), pTable = $('<table class="mb-3">');
            pDiv.empty();
            for (var pPaymentIndex = 0; pPaymentIndex <= vData.payments.length - 1; pPaymentIndex++) {
                var pPayment = vData.payments[pPaymentIndex];
                pPayment.Master = pPayment.Master.Rows[0];
                pTable.append($('<tr>')
                        .append($('<td>').text('Payment Date: '))
                        .append($('<td class="pl-2">').text(pPayment.Master.PaymentDate))
                    ).append($('<tr>')
                        .append($('<td>').text('Receipt Number: '))
                        .append($('<td class="pl-2">').text(pPayment.Master.ReceiptNumber))
                    ).append($('<tr>')
                        .append($('<td>').text('Total Paid Amount: '))
                        .append($('<td class="pl-2">').text(AccountingFormat(pPayment.Master.TotalPayment)))
                    ).append($('<tr>')
                        .append($('<td>').text('Cash Amount: '))
                        .append($('<td class="pl-2">').text(AccountingFormat(pPayment.Master.CashAmount)))
                    )
                pDiv.append(pTable);
                pDiv.append($('<div class="mb-3">').text('Accounts Payables that were paid by this payment'));
                pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                var pTR = $('<tr>');
                pTR.append($('<th>').text('Invoice No.'));
                pTR.append($('<th>').text('Amount Paid'));
                pTable.append(pTR);

                for (var pInvoiceIndex = 0; pInvoiceIndex <= pPayment.Invoices.Rows.length - 1; pInvoiceIndex++) {
                    var pInvoice = pPayment.Invoices.Rows[pInvoiceIndex];
                    pTR = $('<tr>');
                    pTR.append($('<td>').text(pInvoice.InvoiceNumber));
                    pTR.append($('<td>').text(pInvoice.AmountPaid));
                    pTable.append(pTR);
                }
                pDiv.append(pTable);

                var pCheckCount = 0;
                for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                    var pCheck = pPayment.Checks.Rows[pCheckIndex];
                    if (!CBool(pCheck.IsOnlinePayment))
                        pCheckCount++;
                }
                if (pCheckCount > 0) {
                    pDiv.append($('<div class="mb-3 font-weight-bold">').text('Cheques'));
                    pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                    var pTR = $('<tr>');
                    pTR.append($('<th>').text('Account Name'));
                    pTR.append($('<th>').text('Account Number'));
                    pTR.append($('<th>').text('Bank Name'));
                    pTR.append($('<th>').text('Bank Branch'));
                    pTR.append($('<th>').text('Check Number'));
                    pTR.append($('<th>').text('Check Amount'));
                    pTable.append(pTR);
                    for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                        var pCheck = pPayment.Checks.Rows[pCheckIndex];
                        if (CBool(pCheck.IsOnlinePayment))
                            continue;
                        pTR = $('<tr>');
                        pTR.append($('<td>').text(pCheck.AccountName));
                        pTR.append($('<td>').text(pCheck.AccountNumber));
                        pTR.append($('<td>').text(pCheck.BankName));
                        pTR.append($('<td>').text(pCheck.BankBranchName));
                        pTR.append($('<td>').text(pCheck.CheckNumber));
                        pTR.append($('<td>').text(pCheck.CheckAmount));
                        pTable.append(pTR);
                    }
                    pDiv.append(pTable);
                }

                pCheckCount = 0;
                for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                    var pCheck = pPayment.Checks.Rows[pCheckIndex];
                    if (CBool(pCheck.IsOnlinePayment))
                        pCheckCount++;
                }
                if (pCheckCount > 0) {
                    pDiv.append($('<div class="mb-3 font-weight-bold">').text('Online Payment'));
                    pTable = $('<table class="table table-light table-striped table-hover mb-3">');
                    var pTR = $('<tr>');
                    pTR.append($('<th>').text('From Account Name'));
                    pTR.append($('<th>').text('From Account Number'));
                    pTR.append($('<th>').text('From Bank Name'));
                    pTR.append($('<th>').text('From Bank Branch'));
                    pTR.append($('<th>').text('Online Transaction Number'));
                    pTR.append($('<th>').text('Online Transaction Amount'));
                    pTR.append($('<th>').text('To Account Name'));
                    pTR.append($('<th>').text('To Account Number'));
                    pTR.append($('<th>').text('To Bank Name'));
                    pTR.append($('<th>').text('To Bank Branch'));
                    pTable.append(pTR);
                    for (var pCheckIndex = 0; pCheckIndex <= pPayment.Checks.Rows.length - 1; pCheckIndex++) {
                        var pCheck = pPayment.Checks.Rows[pCheckIndex];
                        if (!CBool(pCheck.IsOnlinePayment))
                            continue;
                        pTR = $('<tr>');
                        pTR.append($('<td>').text(pCheck.AccountName));
                        pTR.append($('<td>').text(pCheck.AccountNumber));
                        pTR.append($('<td>').text(pCheck.BankName));
                        pTR.append($('<td>').text(pCheck.BankBranchName));
                        pTR.append($('<td>').text(pCheck.CheckNumber));
                        pTR.append($('<td>').text(pCheck.CheckAmount));
                        pTR.append($('<td>').text(pCheck.AccountName2));
                        pTR.append($('<td>').text(pCheck.AccountNumber2));
                        pTR.append($('<td>').text(pCheck.BankName2));
                        pTR.append($('<td>').text(pCheck.BankBranchName2));
                        pTable.append(pTR);
                    }
                    pDiv.append(pTable);
                }

                pDiv.append($('<hr>'));
            } // END for (var pPaymentIndex = 0; pPaymentIndex <= vData.payments.length - 1; pPaymentIndex++) {
        }

        function HandleEditingSavedAsFinal(vParams) {
            vParams = $.extend({
                AllowFinalizedForApproval: false,
                AllowSomeSaving: false,
                AllowSomeSavingCallback: function() {},
                NoPrompt: false
            }, vParams);
            if (mFinalizedForApproval && vParams.AllowFinalizedForApproval)
                return true;
            var pCondition = mSavedAsFinal;
            if (!vParams.AllowFinalizedForApproval)
                pCondition = pCondition || mFinalizedForApproval;
            if (pCondition) {
                if (vParams.AllowSomeSaving) {
                    PromptUserAccess('Modify Finalized Invoice', {
                        OnClickOKSuccess: function () {
                            NewMessageBox('This transaction was saved as final. Some fields may still be updated. Do you want to update manual invoice number and sales rep?', 'Saved As Final', [
                                {
                                    Text: 'Save',
                                    Click: function() {
                                        vParams.AllowSomeSavingCallback();
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        },
                        OnFailed: function () {
                            console.log('jsCFDTPD3GXC!');
                        }
                    });
                    
                } else {
                    if (!vParams.NoPrompt)
                        NewMessageBox('This transaction was saved as final. This transaction may not be edited.', 'Saved As Final');
                }
                return false;
            }

            return true;
        }

        function DisplayProducts(vData) {
            var pTotalServedQty = 0;
            var pTotalLineGp = 0;
            var pTotal = 0;
            var pStr;
            var pLineCount = 0;
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
                if (pDrProduct.Image != null){
                    pTR.find('[name=ProductImage]').append($('<img src="/Content/MobizImagePIM/'+ sessionStorage.OrgID +'/Small/'+ pDrProduct.Image +'" />')
                            .data('data', pDR)
                            .click(function(e){
                                var rowData = $(this).data('data');
                                if (rowData.Image != null) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    ShowProductImage(rowData.ProductID);
                                }
                            })
                        );
                }
                pTR.find('[name=LineNumber]').text(CStr2(pDR.LineNumber));
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
                pTR.find('[name=UnitGP]').text($.format.number(CDec2(pDR.UnitGP), '#,###.0000'));
                pTR.find('[name=UnitGPM]').text($.format.number(CDec2(pDR.UnitGPM) * 100, '#,###.0000') + '%');
                pTR.find('[name=LineGP]').text($.format.number(CDec2(pDR.LineGP), '#,###.0000'));
                pTR.find('[name=Srp]').text($.format.number(CDec2(pDR.Srp), '#,###.0000'));
                var p2ndLine = '';
                if (CDec2(pDR.PurchaseOrderQuantity) > 0) {
                    p2ndLine = AccountingFormat(CDec2(pDR.OriginalUnitPrice), '#,###.0000') + '<br />PO ' + pDR.PurchaseOrderNumber +
                        '<br />' + FormatDate(new Date(pDR.PurchaseOrderDate), { Format: 'MMM d, yyyy' });
                } else
                    p2ndLine = 'No PO';

                pTR.find('[name=PoPriceOrig]').html(p2ndLine);
                pTotalLineGp += CDec2(pDR.LineGP);
                pTR.find('[name=UnitCogs]').text($.format.number(CDec2(pDR.Cost), '#,###.0000'));
                pTotalServedQty += CDec2(pDR.Quantity);
                pUnit = pDR.UnitMeasure;
                if (CDec2(pDR.ActualQuantity) != 1)
                    pUnit += ' (' + $.format.number(CDec2(pDR.ActualQuantity), '#,###.####') + ' ' + pDR.BaseUnit + ')';
                pTR.find('[name=PackingSize]').text(pUnit);

                p2ndLine = AccountingFormat(pDR.UnitPrice, 4);
                if ((CStr2(pDR.AccountsPayablePoPrice) != '')
                    && (CDec2(pDR.OrderedQuantity) == 0)){

                    var pPrices = [];
                    if ((IsDate(pDR.LastApDate))
                        && (CDec2(pDR.LastApPrice) > 0)) 
                        pPrices.push({
                            Date: new Date(pDR.LastApDate),
                            Text: 'Latest AP Price/Date: ' + AccountingFormat(pDR.LastApPrice, 4) + '<br />' +
                                FormatDate(new Date(pDR.LastApDate), { Format: 'MMM d, yyyy' })
                        });

                    if ((IsDate(pDR.AccountsPayablePoDate))
                        && (CDec2(pDR.AccountsPayablePoPrice) > 0)) 
                        pPrices.push({
                            Date: new Date(pDR.AccountsPayablePoDate),
                            Text: 'Latest PO/Canvass Price/Date: ' + AccountingFormat(pDR.AccountsPayablePoPrice, 4) + '<br />' +
                                FormatDate(new Date(pDR.AccountsPayablePoDate), { Format: 'MMM d, yyyy' })
                        });
                    if (pPrices.length > 0) {
                        pPrices = pPrices.sort(function(vA, vB) {
                            if (vA.Date < vB.Date)
                                return 1; // sort descending by date
                            else if (vA.Date > vB.Date)
                                return -1; // sort descending by date
                            else
                                return 0;
                        });
                    p2ndLine += '<br />' + pPrices[0].Text;
                    }
                }
                pTR.find('[name=UnitPrice]').html(p2ndLine);
                if ((mOut.TransType == 'SalesOrder')
                    || (mOut.TransType == 'Quote')
                    || (mOut.TransType == 'PurchaseOrder'))
                    pTR.find('[name=LineTotal]').text(AccountingFormat(pDR.OrderedAmount, 4));
                else
                    pTR.find('[name=LineTotal]').text(AccountingFormat(pDR.ServedAmount, 4));
                if (CStr2(pDR.AccountsPayablePoPrice) != '') {
                    p2ndLine = AccountingFormat(pDR.AccountsPayablePoPrice, 4) + ' <br>' + FormatDate(new Date(pDR.AccountsPayablePoDate), { Format: 'MMM d, yyyy' });
                    if (pDR.AccountsPayablePoType == 'PO')
                        p2ndLine += '<br /> (PO ' + pDR.AccountsPayablePoNum + ')';
                    else
                        p2ndLine += ' (' + pDR.AccountsPayablePoType + ')';
                    pTR.find('[name=ApPoPrice]').html(p2ndLine);
                    //pTR.find('[name=ApPoDate]').text(FormatDate(new Date(pDR.AccountsPayablePoDate), {Format: 'MMMM d, yyyy'}) + ' (' + pDR.AccountsPayablePoType + ')');
                }
                if (CStr2(pDR.LastApPrice) != '') {
                    pTR.find('[name=LastApPrice]').html(AccountingFormat(pDR.LastApPrice, 4) + ' <br>' + FormatDate(new Date(pDR.LastApDate), { Format: 'MMM d, yyyy' }));
                    //pTR.find('[name=LastApDate]').text(FormatDate(new Date(pDR.LastApDate), {Format: 'MMMM d, yyyy'}));
                }
                pTR.find('[name=PaymentTerms]').text(pDR.TermNumericValue || vData.Master.CreditTerms || 'N/A');
                //(pTR.click(function(vProductID) {
                //    Item_Click(vProductID);
                //}))(pDR.ProductID);

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
                if (CStr2(pDR.ServedDate) == '')
                    pStr = 'Click';
                else
                    pStr = FormatDate(pDR.ServedDate, {Format: 'MM/dd/yyyy'});
                if (CStr2(pDR.CancelledByID) != '')
                    pTR.find('[name=Cancelled] input').prop('checked', true);
                (function(vDR, vTR) {
                    pTR.click(function(e) {
                        if (!$(e.target).parent().is(vTR))
                            return;
                        var pDR = $(this).data('data');
                        Item_Click(pDR.ProductID, pDR);
                    });
                    vTR.find('[name=ServedDate] a').text(pStr).click(function(vElement) {
                        SingleCalendar({
                            Click: function(e){
                                Bs.Loading('show');
                                $(vElement.target).text('Saving...');
                                AjaxSend({
                                    url: 'SalesMenu/SaveServedDate',
                                    data: {
                                        ID: vDR.ID,
                                        ServedDate: e
                                    },
                                    success: function(vData) {
                                        if (vData.Result == 'ok') {
                                            var pStr = FormatDate(e, {Format: 'MM/dd/yyyy'});
                                            $(vElement.target).text(pStr);
                                        } else
                                            NewMessageBox(vData.Error);
                                    },
                                    complete: function() {
                                        Bs.Loading('hide');
                                    }
                                });
                            }
                        });
                    });

                    vTR.find('[name=Cancelled] input').change(function(vElement) {
                        var pChk = $(vElement.target);
                        var pChecked = pChk.prop('checked');
                        pChk.hide();
                        Bs.Loading('show');
                        AjaxSend({
                            url: 'SalesMenu/SaveLineItemCancel',
                            data: {
                                ID: vDR.ID,
                                Cancel: pChecked
                            },
                            success: function(vData) {
                                if (vData.Result == 'ok') {
                                    
                                } else
                                    NewMessageBox(vData.Error);
                            },
                            complete: function() {
                                Bs.Loading('hide');
                                pChk.show();
                            }
                        });
                    });

                    var pShippingRef;
                    vTR.find('[name=ShippingRef] input').on('focus', function(vElement) {
                        var pTextBox = $(vElement.target);
                        pShippingRef = pTextBox.val();
                        //pTextBox.removeAttr('readonly');
                    }).val(CStr2(vDR.ShippingRef));
                    
                    vTR.find('[name=ShippingRef] input').on('blur', function(vElement) {
                        var pTextBox = $(vElement.target);
                        if (pShippingRef == pTextBox.val())
                            return;
                        pTextBox.hide();
                        Bs.Loading('show');
                        AjaxSend({
                            url: 'SalesMenu/SaveLineItemShippingRef',
                            data: {
                                ID: vDR.ID,
                                ShippingRef: pTextBox.val()
                            },
                            success: function(vData) {
                                pShippingRef = pTextBox.val();
                                if (vData.Result == 'ok') {
                                    
                                } else
                                    NewMessageBox(vData.Error);
                            },
                            complete: function() {
                                Bs.Loading('hide');
                                pTextBox.show();
                            }
                        });
                    });

                    //setTimeout(function() {
                    //    vTR.find('[name=ShippingRef] input').removeAttr('readonly');
                    //}, 10000);
                })(pDR, pTR);
                pProductTable.append(pTR);
                pLineCount++;
            }


            if ((mOut.TransType == 'SalesOrder')
                || (mOut.TransType == 'Quote')
                || (mOut.TransType == 'PurchaseOrder'))
                pTotal += CDec2(vData.Master.OrderedAmount);
            else
                pTotal += CDec2(vData.Master.ServedAmount);
            pTotal = CDec2(pTotal);
            pProductTable.find('tfoot tr td[name=ProductDescription]').text('No. of line items: ' + AccountingFormat(pLineCount, 0));
            pProductTable.find('tfoot tr td[name=LineTotal]').text(AccountingFormat(pTotal, 4));
            pProductTable.find('tfoot tr td[name=LineGP]').text(AccountingFormat(pTotalLineGp, 4));
            pProductTable.find('tfoot tr td[name=UnitGPM]').text(AccountingFormat(pTotalLineGp / pTotal * 100, 4) + '%');

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
                pProductTable.find('[name="PoPriceOrig"]').show(); 
                pProductTable.find('[name="ApPoDate"]').show(); 
                pProductTable.find('[name="LastApPrice"]').show(); 
                pProductTable.find('[name="LastApDate"]').show(); 
            } else if ((mOut.TransType == 'Invoice')
                || (mOut.TransType == 'PurchaseOrder')) {
                mElement.find('[name=cbProductAction]').hide();
                pProductTable.find('[name=thServedQty]').show();
                pProductTable.find('[name=ServedQty]').show();
                pProductTable.find('[name="ApPoPrice"]').hide(); 
                pProductTable.find('[name="PoPriceOrig"]').hide(); 
                pProductTable.find('[name="ApPoDate"]').hide(); 
                pProductTable.find('[name="LastApPrice"]').hide(); 
                pProductTable.find('[name="LastApDate"]').hide();

                if (mOut.TransType == 'PurchaseOrder'){
                    var POcols = pProductTable.find('[name="VendorServedQty"], [name="ServedDate"], [name="Cancelled"], '+
                        '[name="ShippingRef"], [name="ServedQty"], [name="thServedQty"], ' +
                        '[name="thServedBadQty"], [name="ServedBadQty"]');

                    if (!mSavedAsFinal){
                        POcols.hide();
                    } else {
                        POcols.show();
                    }
                }
            } else {
                mElement.find('[name=cbProductAction]').hide();
                pProductTable.find('[name=thServedQty]').hide();
                pProductTable.find('[name=ServedQty]').hide();
                pProductTable.find('[name=thServedBadQty]').hide();
                pProductTable.find('[name=ServedBadQty]').hide();
                pProductTable.find('[name="ApPoPrice"]').hide(); 
                pProductTable.find('[name="PoPriceOrig"]').hide(); 
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
                    Callback: function(vAllowed) {
                        if (vAllowed) {
                            pProductTable.find('.colProfit').show();
                        }
                    }
                });            
            else if (mOut.TransType == 'Invoice') 
                if (CBool(sessionStorage.IsSOALink)){
                    return
                }
                GetSecurityCurrentUserAcccess({
                    ModuleName: 'View Profitability Details in Sales Invoice',
                    Callback: function(vAllowed) {
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
        mTxtSalesPartner.on('keypress', function(e) {
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
                ], { FilterCustomers:1 }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        var mTxtSalesRepText = mElement.find('[name=txtSalesRepText]');
        var mTxtSalesRep = mElement.find('[name=txtSalesRep]');
        mTxtSalesRepText.on('keypress', function(e) {
            if (e.key == 'Enter')
                SearchSalesRep();
        }); // END mElement.find('[name=txtSalesPartner]').click(function() {
        mElement.find('[name=butSearchSalesRep]').click(SearchSalesRep);

        function SearchSalesRep() {
            PromptUserAccess('Allow Edit of Sales Rep Field', {
                OnClickOKSuccess: function () {
                    Proceed();
                },
                OnFailed: function () {
                }
            });

            function Proceed() {
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
        }

        var mTxtOrderedBy = mElement.find('[name=txtOrderedBy]');
        mTxtOrderedBy.click(SearchCustomerContactPerson);
        mTxtOrderedBy.on('keypress', function(e) {
            if (e.key == 'Enter')
                SearchCustomerContactPerson();
        }); // END mTxtBillingAddress.on('keypress', function(e) {
        mElement.find('[name=butSearchCustomerContactPerson]').click(SearchCustomerContactPerson);

        mElement.find('[name=butAddCustomerContactPerson]').click(function() {
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
                            Column: function(vData) {
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

        mElement.find('[name=butPay]').click(function() {
            NewMessageBox('This function is not yet implemented. ref: jsCTDD4VS4DR!', 'Pay');
        });

        mElement.find('[name=butXml]').click(function() {
            require(['promise!/Scripts/sources/ERP/Invoicing/uploadPo/controller.js?v=' + assetversion], function() {
                ERP.Invoicing.uploadPo.controller.popup({
                    AfterSave: function(vID) {
                        LoadTrans({
                            ID: vID
                        });
                    }
                });
            });
            
        });

        mElement.find('[name=butNew]').click(function() {
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

        function NewTrans() {
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
            mChkDontAutoInvoice.prop('checked', 0);
            mTxtBillingAddress.val('');
            mTxtDeliveryAddress.val('');
            mTxtRemarks.val('');
            mTxtPoNumber.val('');
            mTxtPoDate.val(''); // FormatDate(new Date())
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
            mCbInvoiceType.val('');

            // "Freight and Handling" tab
            mTxtShipper.val('');
            mTxtWaybillNumber.val('');
            mTxtShipDate.val('');
            mTxtPackageQuantity.val('');
            mTxtGrossWeight.val('');
            mTxtGrossVolume.val('');
            mTxtFreight.val('');
            mTxtHandling.val('');
            mTxtArrastre.val('');
            mTxtOtherCharges.val('');
            mTxtFreightAndHandling.val('');
        }

        function AfterProductSave(vParams) {
            vParams = $.extend({
                BelowGpmThreshold: false
            }, vParams);
            if (vParams.BelowGpmThreshold) {
                var pMsg = `Some product(s) have a gross profit margin that is below the minimum threshold set by the company.<br /><br />
                    You will still be able to save this transaction but profitability approval needs to be made by an authorized person before this transaction can be processed further.<br /><br />
                    Please get in touch with your company’s authorized person for approvals to expedite this transaction.`;
                NewMessageBox(pMsg, 'Gross Profitability Warning');
            }
            mOut.Model.Data.ID = pProductPopup.Model.Data.OrderID;
            LoadTrans({
                ID: mOut.Model.Data.ID,
                LoadProductsOnly: true
            });
        }
        mOut.AfterProductSave = AfterProductSave;

        mElement.find('[name=butAddBillingAddress]').click(function() {
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
                    Click: function() {
                        var pCustomerID;
                        if (mOut.TransType == 'PurchaseOrder') {
                            pCustomerID = sessionStorage.OrgID;
                        } else
                            pCustomerID = mOut.Model.Data.CustomerID;
                        AddAddress({
                            CustomerID: pCustomerID,
                            Address: mTxtBillingAddress.val().substring(0, 250),
                            Success: function(vData) {
                                mOut.Model.Data.BillingAddressID = vData.ID;
                            }
                        });
                    }
                },
                {
                    Text: 'No',
                    Click: function() {
                        
                    }
                }
            ]);
        });

        function AddAddress(vParams) {
            
            mOut.Model.AddAddress({
                Address: vParams.Address,
                CustomerID: vParams.CustomerID,
                Success: function(vData) { 
                    NewMessageBox('Address was successfully added.');
                    vParams.Success(vData);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }

        mElement.find('[name=butAddDeliveryAddress]').click(function() {
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
                    Click: function() {
                        var pCustomerID;
                        if (mOut.TransType == 'PurchaseOrder') {
                            pCustomerID = sessionStorage.OrgID;
                        } else
                            pCustomerID = mOut.Model.Data.CustomerID;
                        AddAddress({
                            CustomerID: pCustomerID,
                            Address: mTxtDeliveryAddress.val().substring(0, 250),
                            Success: function(vData) {
                                mOut.Model.Data.DeliveryAddressID = vData.ID;
                            }
                        });
                    }
                },
                {
                    Text: 'No',
                    Click: function() {
                        
                    }
                }
            ]);
        });

        mElement.find('[name=butOutput]').click(HandlePrint);

        function HandlePrint(vParams) {
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
                            pMsg = `This transaction is saved as final but profitability approval needs to be made by an authorized person before this transaction can be printed.<br /><br />
                                Please get in touch with your company’s authorized person for approvals to expedite this transaction.`;
                        }    

                        if (mHasCnCUnapprovedReason) {
                            if (pMsg != '')
                                pMsg += '<br /><br />';

                            pMsg += 'This transaction is saved as final but is awaiting credit and collection approval due to credit limit and/or credit terms. A printed invoice cannot be generated before approval is made. Please coordinate with your credit and collection officer for approval.';
                        }

                        if ((pMsg == '')
                            && (mOut.Model.Data.TransType == 'PurchaseOrder')) {

                            pMsg = 'This transaction needs to be approved first.';
                        }

                        if (pMsg != '') {
                            NewMessageBox(pMsg, 'Saved but Awaiting Approval');
                            return;
                        }
                    }

                    if (!mSavedAsFinal 
                        && (mOut.TransType != 'AccountsPayable')
                        && (mOut.TransType != 'PurchaseOrder')
                        && (mOut.TransType != 'SalesOrder')) {
                        CopyTransToPlainText(mOut.TransType, vData);
                        NewMessageBox('This transaction was successfully copied to your clipboard. <br><br>' + 
                            'You can now paste this transaction\'s details to any of your preferred messaging or office productivity application. <br><br>' +
                            'However, you can only print out this transaction after you\'ve saved this as final [version] -- after which no more edits may be made to this transaction.');
                        return;
                    }

                    if (mSavedAsFinal
                        && mHasCnCUnapprovedReason
                        && (mOut.TransType == 'SalesOrder')) {

                        NewMessageBox('This transaction is awaiting approval from credit and collection and may not be prepared and/or invoiced prior to said approval.');
                        return;
                    }

                    if (CBool(GetUrlParameters().IsReadOnly)){
                        CopyTransToPlainText(mOut.TransType, vData);
                        return
                    }
                    var pMessageButtons = [];

                    switch (mOut.TransType) {
                        case 'Quote':
                            pMessageButtons.push({
                                Text: 'Sales Quotation',
                                Click: function() {
                                    CreatePdf();
                                }
                            });

                            break;

                        case 'SalesOrder':
                            if (mSavedAsFinal) {
                                pMessageButtons.push({
                                    Text: 'Sales Order',
                                    Click: function() {
                                        CreatePdf();
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Pick List',
                                    Click: function() {
                                        CreatePdf({
                                            Template: 'PickList'
                                        });
                                    }
                                });
                            } else {
                                pMessageButtons.push({
                                    Text: 'Stock Availability Check',
                                    Click: function() {
                                        CreatePdf({
                                            Template: 'PickList',
                                            PrintAsDraft: true
                                        });
                                    }
                                });
                            }
                            break;

                        case 'Invoice':
                            if (CStr2(vData.Master.TransType).toLowerCase() == 'invoice') {
                                pMessageButtons.push({
                                    Text: 'Invoice',
                                    Click: function() {
                                        CreatePdf();
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Invoice\'s Supplemental Blind Copy',
                                    Click: function() {
                                        CreatePdf({
                                            PrintBlindCopy: true,
                                            Template: 'Invoice - BlindCopy'
                                        });
                                    }
                                });
                                pMessageButtons.push({
                                    Text: 'Pick List',
                                    Click: function() {
                                        CreatePdf({
                                            Template: 'PickList'
                                        });
                                    }
                                });

                                pMessageButtons.push({
                                    Text: 'Warehouse Releasing Slip',
                                    Click: function() {
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
                                Attr: {'class': 'btn custom-btn-theme-c'},
                                Click: function() {
                                    CreatePdf({
                                        Template: 'Purchase Order'
                                    });
                                }
                            });

                            pMessageButtons.push({
                                Text: 'No-Price Purchase Order',
                                Click: function() {
                                    CreatePdf({
                                        Template: 'No-Price Purchase Order'
                                    });
                                }
                            });

                            break;
                    
                        case 'AccountsPayable':
                            pMessageButtons.push({
                                Text: 'Stock Receiving + PO + AP',
                                Click: function() {
                                    var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
                                    AjaxSend({
                                        url: '/Receiving/CreatePdf',
                                        data: {
                                            AccountsPayableID: pInvoiceID,
                                            PrintHash: localStorage.getItem('PrintHash'),
                                            Template: 'StockReceivingAndPO'
                                        },
                                        success: function(vData) {
                                            if (vData.Error) {
                                                if (vData.Error.indexOf('PrintHash') == -1)
                                                    NewMessageBox(vData.Error);
                                                else
                                                    NoPrintHash();
                                            } else if (vData.Result == 'ok')
                                                ViewReport(vData.BaseFilename);

                                        }
                                    });
                                }
                            });
                            break;
                    }

                    if (pMessageButtons.length > 0) {
                        PopupButtonList('', 'Output Options', [
                            {
                                Text: 'Print Output', Click: function(){
                                    NewMessageBox('What do you want to print?', 'Print', pMessageButtons, {
                                        SameColorButtons: true
                                    });
                                }
                            },{
                                Text: 'Copy transaction as plain text', Click: function(){
                                    CopyTransToPlainText(mOut.TransType, vData);
                                }
                            }
                        ])                        
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
                Template: null,
                PrintBlindCopy: false,
                PrintAsDraft: null // boolean
            }, vParams);

            var pInvoiceID = GetStringObject('Model.Data.ID', mOut);
            Bs.Loading('show');
            AjaxSend({
                url: '/ERP/CreatePdf',
                data: {
                    InvoiceID: pInvoiceID,
                    PrintHash: localStorage.getItem('PrintHash'),
                    TransType: mOut.TransType,
                    Template: vParams.Template,
                    PrintBlindCopy: vParams.PrintBlindCopy,
                    PrintAsDraft: vParams.PrintAsDraft
                },
                success: function(vData) {
                    if (vData.Error) {
                        if (vData.Error == 'Transaction was already printed.') {
                            if (CBool(vData.HasUnapprovedReprintRequests))
                                NewMessageBox('This document has an unapproved reprint request. Have your supervisor approve that request before you can print.');
                            else
                                require(['promise!/Scripts/sources/ERP/OrderProcessing/reprintRequest/controller.js?v=' + assetversion] , function() {
                                    ERP.OrderProcessing.reprintRequest.controller.popup({ID: pInvoiceID});
                                });
                        } else if (vData.Error.indexOf('PrintHash') == -1) {
                            if (vData.Error == 'UUID is required.')
                                NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                    'This is by design to enforce document print controls. <br /><br />\n\n' + 
                                    'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                    'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                    [
                                        {
                                            Text: 'Download Mobiz Controller',
                                            Click: function() {
                                                window.open('/Content/PrintApplet.zip', '_blank');
                                            }
                                        },
                                        {
                                            Text: 'Cancel'
                                        }
                                    ]);
                            else
                                NewMessageBox(vData.Error);
                        } else
                            NoPrintHash();
                    } else if (vData.Result == 'ok') {
                        if ((vData.BaseFilenames != null)
                            && (vData.BaseFilenames.length > 0)) {
                            
                            for (var pIndex = 0; pIndex <= vData.BaseFilenames.length - 1; pIndex++) {
                                ViewReport(vData.BaseFilenames[pIndex]);
                            }
                            NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                        } else
                            NewMessageBox('Your document will be printed by your local print server.', '', null, {
                                Close: function() {
                                    if (vParams.NewTransAfterPrint)
                                        NewTrans();
                                }
                            });
                    }
                },
                complete: function() {
                    Bs.Loading('hide');
                }
            });
        }

        function ViewReport(vFilename) {
            var pUrl = MergeUrlParameters({
                Filename: vFilename
            }, '/Erp/DownloadPrintablePdf');
            window.open(pUrl, '_blank', 
                'width=' + window.innerWidth + 
                ',height=' + window.innerHeight + 
                ',scrollbars=auto');
        } // END function ViewReport() {

        mElement.find('[name=butActivityTracker]').click(function() {
            require(['promise!/Scripts/sources/ERP/ActivityTracker/controller.js?v=' + assetversion], function() {
                ERP.ActivityTracker.controller.popup({
                    ID: mOut.Model.Data.ID,
                    TransType: mOut.Model.Data.TransType
                });
            });
        });

        mElement.find('[name=butInvoiceImage]').click(function() {
            AjaxSend({
                url: 'CreditAndCollection/GetInvoiceFiles',
                data: { InvoiceID: mOut.Model.Data.ID },
                success: function (e) {
                    //Cac.ShowGallery(e.Results.Rows);
                    var vRows = e.Results.Rows;
                    var divInvoiceImagePreview = $('#divInvoiceImagePreview');

                    divInvoiceImagePreview.empty();

                    for (var i = 0; i < vRows.length; i++) {
                        var src = location.origin + '/InvoicesFiles/' + sessionStorage.OrgID + '/' + vRows[i].InvoiceID + '/' + vRows[i].FileName;

                        divInvoiceImagePreview.append($('<div>')
                                .append($('<a data-role="none" data-lightbox="invimg">')
                                    .attr('id', vRows[i].ID)
                                    .attr('href', src)
                                    .append($('<img>')
                                    .data('data', vRows[i]).attr('src', src)
                                )
                                )
                            )
                    }
                    $('#' + vRows[0].ID).click();
                }
            })
        });

        mElement.find('[name=butMarkAsFollowedUp]').click(function() {
            AjaxSend({
                url: 'ERP/MarkTransAsFollowedUp',
                data: { InvoiceID: mOut.Model.Data.ID },
                success: function (e) {
                    if (e.Results == 'ok'){
                        NewMessageBox('Marked as followed up');
                    }
                }
            })
        });
        mElement.find('[name=butCancel]').click(function() {
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
                        Click: function() {
                            Proceed();
                        }
                    },
                    {
                        Text: 'No',
                        Click: function() {
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
                    success: function(vData) {
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

        mElement.find('[name=butInteractionHistory]').click(function() {
            if (mOut.Model.Data.CustomerID == null){
                NewMessageBox('Please select '+ (mOut.TransType == 'PurchaseOrder' ? 'vendor' : 'customer') + ' first.');
                return;
            }
            Bs.Loading('show');
            require(['promise!/Scripts/sources/OrganizationShort/notes/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.Company.main.controller.Load({
                    module: mOut.TransType,
                    CompanyID: mOut.Model.Data.CustomerID,
                });
            })
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
                    Click: function() {
                        window.open('/Content/PrintApplet.zip', '_blank');
                    }
                },
                {
                    Text: 'No'
                }
            ]);

        }

        if ((CStr2(localStorage.getItem('UUID')).length < 1)
            && (mOut.TransType != 'Quote')
            && (mOut.TransType != 'AccountsPayable')
            && (mOut.TransType != 'PurchaseOrder')
            )
            NoPrintHash();

        var mCbInvoiceType = mElement.find('[name=cbInvoiceType]');
        mCbInvoiceType.change(mCbInvoiceType_change);
        function mCbInvoiceType_change()
        {
            var pSelectedInvoiceTypeID = mCbInvoiceType.val();
            if (CStr2(pSelectedInvoiceTypeID) == '') {
                mUnitPriceSurchargePercent = 0;
                return;
            }
            pSelectedInvoiceTypeID = ReadyGuid(pSelectedInvoiceTypeID);
            for (var p = 0; p <= mInvoiceTypes.length - 1; p++) {
                var pDR = mInvoiceTypes[p];
                if (ReadyGuid(pDR.ID) == pSelectedInvoiceTypeID) {
                    mUnitPriceSurchargePercent = CDec2(pDR.UnitPriceSurchargePercent);
                    return;
                }
            }
        }

        function InitInvoiceTypes() {   
            if (mInvoiceTypes == null) 
                mOut.Model.GetInvoiceTypes({
                    success: function(vData) {
                        mInvoiceTypes = vData.Results.Rows;
                        DisplayInvoiceTypes();
                    }
                });
            else
                DisplayInvoiceTypes();

            function DisplayInvoiceTypes() {
                var pOption;
                var pDR;
                mCbInvoiceType.html('');
                pOption = new Option('Please select', '');
                mCbInvoiceType.append(pOption);    
                for (var p = 0; p <= mInvoiceTypes.length - 1; p++) {
                    pDR = mInvoiceTypes[p];
                    if (IsGuid(mOut.Model.Data.StockTransferID)) {
                        if (pDR.DocumentName != 'STI')
                            continue;
                    } else {
                        if (pDR.DocumentName == 'STI')
                            continue;
                    }
                    pOption = new Option(pDR.DocumentName, pDR.ID);
                    mCbInvoiceType.append(pOption);    
                }
            }
        }

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

            pCB.change(function() {
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
                    success: function(vData) {
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
        function InitPurchaseOrderDeliveryMethods() {   
            var pOption;

            mCbDeliveryMethod.html('');
            pOption = new Option('Deliver to Buyer\'s Location'); mCbDeliveryMethod.append(pOption);
            pOption = new Option('Pick Up by Buyer\'s Internal Logistics'); mCbDeliveryMethod.append(pOption);
            pOption = new Option('Pick Up by 3rd Party Logistics (3PL)'); mCbDeliveryMethod.append(pOption);
            pOption = new Option('Deliver to Buyer\'s Shipping Service Provider/Agent'); mCbDeliveryMethod.append(pOption);
            pOption = new Option('Dropship to Buyer\'s Customer'); mCbDeliveryMethod.append(pOption);
        }

        var mTxtInvoiceFowst = mElement.find('[name=txtInvoiceFowst]');
        mElement.find('[name=butInvoiceFowst]').click(function() {
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
        
        mOut.Initialize = function() {
            mCustomerRequirePoNum = false;
            if (mOut.TransType == 'Quote') {
                mElement.find('[name=divSalesOrderNumber]').hide();
                mElement.find('[name=divPoNumber]').hide();
                mTxtQuoteNumber.attr('readonly', 'readonly')
                    .attr('placeholder', 'To be generated after saving as final');
                mElement.find('[name=butSaveACopy]').show();
                $('[data-target=#tabOPDashboard]').text('Quotation Dashboard');
                $('[data-target=#tabOPWork]').text('Quotation');
                $('[data-target=#tabOPSettings]').text('Quotation Settings');
                InitInvoiceTypes();
            } else if (mOut.TransType == 'SalesOrder') {
                mElement.find('[name=divSalesOrderNumber]').show();
                mElement.find('[name=divPoNumber]').show();
                mTxtQuoteNumber.attr('readonly', null)
                    .attr('placeholder', 'Select a quotation');
                mElement.find('[name=butSaveACopy]').show();
                $('[data-target=#tabOPDashboard]').text('Dashboard');
                $('[data-target=#tabOPWork]').text('Order Processing');
                $('[data-target=#tabOPSettings]').text('Settings');
                InitDeliveryMethods();
                InitInvoiceTypes();
            } else if (mOut.TransType == 'Invoice') {
                mElement.find('[name=divSalesOrderNumber]').show();
                mElement.find('[name=divPoNumber]').show();
                mTxtQuoteNumber.attr('readonly', null)
                    .attr('placeholder', 'This field is read-only');
                mElement.find('[name=butSaveACopy]').hide();
                $('[data-target=#tabOPDashboard]').text('Dashboard');
                $('[data-target=#tabOPWork]').text('Invoicing');
                $('[data-target=#tabOPSettings]').text('Settings');
                InitDeliveryMethods();
                InitInvoiceTypes();
            } else if (mOut.TransType == 'PurchaseOrder') {
                InitPurchaseOrderDeliveryMethods();
            } else if (mOut.TransType == 'StockReceiving') {
            } else if (mOut.TransType == 'AccountsPayable') {
            } else if (mOut.TransType == 'StockTransfer') {
            } else {
                throw 'Invalid vTransType. ref: js9S8L68QX4L!';
            }

            ShowHideColumns();
            
            require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function () {
                if (!CBool(vParams.ViewInvFromCrm)) { // erp/accountdetails/controller.js
                    $('#tabOPWorkAccountDetails').append((proj.IsBootstrapDesign() ? $('#popupAccountDetailsContent-bs > *') : $('#popupAccountDetailsContent')));//.show();
                    //$('#tabOPWorkAccountDetails').hide();
                }                
            });
        };

        function LoadAccountsDetails() {
            require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function() {
                var pMode;

                if (mTargetOrgType == 'customer')
                    pMode = 'AR';
                else {
                    pMode = 'AP';
                    return; // 2023_03_28__13_45_05_9005
                }
                ERP.AccountDetails.controller.Mode = pMode;
                ERP.AccountDetails.controller.CustomerID = mOut.Model.Data.CustomerID;
                ERP.AccountDetails.controller.CustomerName = mTxtCustomer.val();
                //$('#tabOPWorkAccountDetails').append($('#popupAccountDetails').find('.popup-content:first')).show();
                //$('#popupAccountDetails').show();
                ERP.AccountDetails.controller.GetCustomerCreditDetails(mOut.Model.Data.CustomerID, 'has payable');
            });
        }

        mElement.find('[name=lnkChildSos]').click(function(e) {
            e.preventDefault();
            mChildSos.Show({
                InvoiceID: mOut.Model.Data.ID
            });
            return false;
        });

        mElement.find('[name=lnkRelatedSos]').click(function(e) {
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
                    OnOk: function() {
                    
                    },
                    OnCancel: function() {

                    }
                });
            return false;
        });

        var mTxtTransferFromOrg = mElement.find('[name=txtTransferFromOrg]');
        mElement.find('[name=butTransferFromOrg]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowseTransferOrg({
                Title: 'Transfer From Organization',
                Success: function(vData) {
                    mTxtTransferFromOrg.val(vData.CompanyName);
                    mOut.Model.Data.TransferFromOrgID = vData.ID;
                }
            });
        });

        var mTxtTransferToOrg = mElement.find('[name=txtTransferToOrg]');
        mElement.find('[name=butTransferToOrg]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowseTransferOrg({
                Title: 'Transfer To Organization',
                Success: function(vData) {
                    mTxtTransferToOrg.val(vData.CompanyName);
                    mOut.Model.Data.TransferToOrgID = vData.ID;
                }
            });
        });
        
        function BrowseTransferOrg(vParams) {
            vParams = $.extend({
                Title: null,
                Success: function(vData) {
                    
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
        mElement.find('[name=butTransferFromOutlet]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowseTransferOutlet({
                Title: 'Transfer From Outlet',
                FromOrTo: 'From',
                Success: function(vData) {
                    mTxtTransferFromOutlet.val(vData.Data.FowstName);
                    mOut.Model.Data[vData.OutletFieldName] = vData.ID;
                }
            });
        });

        var mTxtTransferToOutlet = mElement.find('[name=txtTransferToOutlet]');
        mElement.find('[name=butTransferToOutlet]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowseTransferOutlet({
                Title: 'Transfer To Outlet',
                FromOrTo: 'To',
                Success: function(vData) {
                    mTxtTransferToOutlet.val(vData.Data.FowstName);
                    mOut.Model.Data[vData.OutletFieldName] = vData.ID;
                }
            });
        });

        function BrowseTransferOutlet(vParams) {
            vParams = $.extend({
                Title: null,
                FromOrTo: '', // From or To (case-sensitive)
                Success: function(vData) {
                    
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

        mElement.find('[name=butSaveAndFinalizeStockTransfer]').click(function() {
            if (!HandleEditingSavedAsFinal({
                AllowSomeSaving: true,
                AllowSomeSavingCallback: function() {
                    mOut.Model.Data.RefInvoiceNum = mTxtRefInvoiceNumber.val();
                    mOut.Model.Data.SalesRepText = mTxtSalesRepText.val();
                    mOut.Model.SaveSomeFields({
                        success: function(vData) {
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

        mElement.find('[name=butSaveAsDraftStockTransfer]').click(function() {
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
                Success: function(){
                    NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);

            mOut.Model.Data.FowstID = sessionStorage.FowstID;
            mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
            mOut.Model.Data.DateNeeded = mTxtDateNeeded.val();
            mOut.Model.Data.DontAutoInvoice = mChkDontAutoInvoice.prop('checked') ? 1 : 0;
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
            mOut.Model.Data.DocumentNameID = mCbInvoiceType.val();
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
                    pTitle = 'Save '+ (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                    pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?<br /><br />';
                    pMessage += 'Finalizing this transaction means that you will not be able to edit this transaction\'s information later on.<br /><br />';
                    pMessage += 'Only finalized transactions can be printed out (subject to print number limits set by your system administrator).';
                } else {
                    pTitle = 'Save '+ (vParams.SaveAndFinalize ? 'And Finalize' : 'As Draft');
                    pMessage = 'Are you sure you want to save ' + (vParams.SaveAndFinalize ? 'and finalize ' : '') + 'this transaction?';
                }

                mOut.Model.Verify({
                    Success: function() {
                        NewMessageBox(pMessage, pTitle, [
                            {
                                Text: 'Yes',
                                Click: function() {
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
                    Fail: function(vMessage) {
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
                Success: function(){
                    if (!vParams.NoPrompt)
                        NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);
            mOut.Model.SaveStockTransfer({
                Data: {
                    ConsolidateSalesOrder: vParams.ConsolidateSalesOrder,
                    UseUnitPriceFromLibrary: vParams.UseUnitPriceFromLibrary
                },
                Success: function(vData) {
                    if (CDec2(vData.OtherOrderCount) > 0) {
                        //&& !vParams.NoPrompt) {

                        NewMessageBox('Items from child orders are ready for invoicing. Do you want to include those items to this invoice?', 'Child orders?', [
                            {
                                Text: 'Yes',
                                Click: function() {
                                    vParams.ConsolidateSalesOrder = true;
                                    Save(vParams);
                                }
                            },
                            {
                                Text: 'No',
                                Click: function() {
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
                        AfterLoad: function() {
                            vParams.Success(vData);
                            mSavedAsFinal = vParams.SaveAndFinalize;
                            if (CBool(mSavedAsFinal))
                                NewMessageBox('Do you want to print this transaction?', 'Print?', [
                                    {
                                        Text: 'Yes',
                                        Click: function() {
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
                Fail: function(vMessage) {
                    NewMessageBox('Mobiz cannot save your current transaction because: ' + vMessage);
                }
            });
        }

        mOut.ReadOnly = function() {
            //mElement.find('[name="butOutput"]').find('> span').removeAttr('class').addClass('bi-clipboard');
            //mElement.find('[name="butOutput"]').find('> div').html('Copy transaction to <br> plain text');

            if (mOut.TransType != 'AccountsPayable'){
                if (mOut.TransType == 'Invoice' || mOut.TransType == 'SalesOrder'){
                    mElement.find('[name=divToolbar]').find('button:not(:eq(6))').remove(); // naa lang sa isa ka parent div ang action button sa invoice.
                    mElement.find('[name=divToolbar]').removeClass('w-100').show();
                } else if (mOut.TransType == 'PurchaseOrder'){
                    mElement.find('[name=divToolbar]').find('button:not(:eq(5)):not(:eq(7))').remove();
                    mElement.find('[name=divToolbar]').removeClass('w-100');
                } else if (mOut.TransType == 'Quote'){
                    mElement.find('[name=divToolbar]').find('button:not(:eq(5))').remove();
                    mElement.find('[name=divToolbar]').removeClass('w-100');
                } else {
                    mElement.find('[name=divToolbar]').hide();
                }                
            }
            
            mElement.find('[name=butAddProduct]').hide();
            mElement.find('[name=divAddproducts]').hide();
            mElement.find('.btn-xs').hide(); 
            $('#divEasHeader, #divQuotationTab, #divInvoicingTab, #divOrderProcessingTab').hide();
	        $('#divInvoicingTab, #divOrderProcessingTab, #divPurchasingTab').addClass('hidetab').hide().css({'display': 'none'});

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

        mElement.find('[name=butOptions]').click(function() {
            $('#divPopupAccountsPayablegOptions').show().popup('open');
            $('#divPopupAccountsPayablegOptions').find('[name=chkFollowVendorCost][value=vendor]').prop('checked', true).checkboxradio('refresh');
        });

        $('#divPopupAccountsPayablegOptions').find('[name=butOptionSave]').click(function() {
            LoadTrans({
                ID: mOut.Model.Data.ID,
                AccountsPayableFollowCost: $('#divPopupAccountsPayablegOptions').find('[name=chkFollowVendorCost]:checked').val()
            });
            $('#divPopupAccountsPayablegOptions').show().popup('close');
        });

        $('[name=butSalesReturn]').click(function() {
            if (CStr2(mOut.Model.Data.ID) == '') {
                NewMessageBox('Please lookup an invoice first.', 'Sales Return');
                return;
            }
            require(['promise!/Scripts/sources/ERP/SalesReturn/work/controller.js'], function() {
                if ($('#popupSalesReturn').length == 0)
                    console.log('#popupSalesReturn does not exist. ref: jsDCYMVWAJK8!');
                $('#popupSalesReturn').modal('show');                            
                if (mOut.Model.Data.ID) {
                    ERP.SalesReturn.work.controller.RepopupAdjustment = false;
                    ERP.SalesReturn.work.controller.LoadTrans({ID: mOut.Model.Data.ID});
                }
                //ERP.SalesReturn.controller();
            });
        });

        $('[name=butScrollToTop]').click(function() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        mOut.Initialize(mOut.TransType);
        NewTrans();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
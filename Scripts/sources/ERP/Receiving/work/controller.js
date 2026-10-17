define([
    '/Scripts/sources/ERP/Receiving/work/model.js',
    'promise!/Scripts/sources/ERP/OrderProcessing/serialNumbers/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/Receiving/product/controller.js?v=' + assetversion,
    '/Scripts/sources/ERP/Invoicing/InvoiceImage/controller.js?crc=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mQuoteHtml;
    var mSalesOrderHtml;
    var mInvoiceHtml;
    var mStockReceivingHtml;
    var mStockTransferHtml;
    var mPopupHtml;

    (function staticInit() {

        function LoadStockReceivingHtml() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Receiving/work/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mStockReceivingHtml = vData;
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
                    mDeferred.resolve();
                }
            });
        }

        AjaxSend({
            ExactUrl: '/Scripts3/view/PoPopup.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mPopupHtml = vData;
                LoadStockReceivingHtml();
            }
        });

    })();

    CreateStringObject('ERP.Receiving.work.controller', null, function (vParams) {

        vParams = $.extend({
            TransType: null
        }, vParams);
        var mOut = {};
        mOut.Model = new ERP.Receiving.work.model();
        mOut.TransType = vParams.TransType;
        var mElement;
        mOut.element = mElement;
        var mInputData;
        var mPopupElement = $(mPopupHtml);
        var mReceiveInOrgID;
        var mReceiveIn;
        var mSavedAsFinal;
        var pProductPopup = ERP.Receiving.product.controller;
        var mSerialNumbersPopup = new ERP.OrderProcessing.serialNumbers.controller();
        var mCanShowLandedCost = false;

        if (mOut.TransType == 'StockTransfer') {
            mElement = $(mStockTransferHtml);
            $('#divStockTransferMain .tab-content').append(mElement);
            $('#tabStockTransferWork').opentabmenu();
        } else {
            mElement = $(mStockReceivingHtml);
            $('#divStockReceivingMain .tab-content').append(mElement);
        }

        var mTxtPaymentTerms = mElement.find('[name=txtPaymentTerms]');
        var mDivDraftOrFinalized = mElement.find('[name=divDraftOrFinalized]');
        var mTxtCanvassNumber = mElement.find('[name=txtCanvassNumber]');
        var mTxtCanvassDate = mElement.find('[name=txtCanvassDate]');
        var mTxtBillingAddress = mElement.find('[name=txtBillingAddress]');
        mTxtBillingAddress.on('keypress', function(e) {
            if (!HandleEditingSavedAsFinal())
                return;
            if (e.key == 'Enter')
                SearchBillingAddress();
        }); // END mTxtBillingAddress.on('keypress', function(e) {

        function SearchBillingAddress() {
            if (!HandleEditingSavedAsFinal())
                return;

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
                    OrganizationID: sessionStorage.OrgID,
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
            if (!HandleEditingSavedAsFinal())
                return;

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
                    OrganizationID: sessionStorage.OrgID,
                    HideNoAddress: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }
        mElement.find('[name=butSearchDeliveryAddress]').click(SearchDeliveryAddress);

        var mTxtPoNumber = mElement.find('[name=txtPoNumber]');
        var mTxtPoDate = mElement.find('[name=txtPoDate]');
        mElement.find('[name=butLookupPo]').click(function() {
            FindPurchaseOrder({
                TransTypes: 'PurchaseOrderAndInvoice'
            });
        });
        mTxtPoNumber.click(function() {
            FindPurchaseOrder({
                TransTypes: 'PurchaseOrderAndInvoice'
            });
        });
        mElement.find('[name=butPoNumber]').click(function() {
            FindPurchaseOrder({
                TransTypes: 'PurchaseOrderAndInvoice'
            });
        });

        var mTxtInvoiceNumber = mElement.find('[name=txtInvoiceNumber]');
        var mTxtInvoiceDate = mElement.find('[name=txtInvoiceDate]');
        mElement.find('[name=butInvoiceDate]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                Click: function(e){
                    mTxtInvoiceDate.val(e);
                }
            });
        });

        var mTxtSalesOrderNumber = mElement.find('[name=txtSalesOrderDate]');
        mElement.find('[name=butSalesOrderDate]').click(function () {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                Click: function (e) {
                    mTxtSalesOrderNumber.val(e);
                }
            });
        });

        mElement.find('[name=butInvoiceImageReference]').click(function () {
            ERP.InvoiceImage.main.controller.popup({
                mod: 'Receiving',
                ID: mOut.Model.Data.ID,
                SupplierID: mOut.Model.Data.SupplierID,
                SupplierName: mElement.find('[name=txtSupplier]').val()
            });
        });

        function HandleEditingSavedAsFinal() {
            if (mSavedAsFinal) {
                NewMessageBox('This transaction was saved as final. This transaction may not be edited.', 'Saved As Final');
                return false;
            }
            return true;
        }

        var mButReceiveInOrg = mElement.find('[name=butReceiveInOrg]');
        mButReceiveInOrg.click(function() {
            BrowsePopup('LibraryOrganization', 'Receive in organization',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mOut.Model.Data.ReceiveInOrgID = e.ID;
                            mButReceiveInOrg.text(e.CompanyName).button('refresh');
                        }
                    }
                ], { 
                //    FilterCustomers:1 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        var mButReceiveInOutlet = mElement.find('[name=butReceiveInOutlet]');
        mButReceiveInOutlet.click(function() {
            BrowsePopup('LibraryFOWST', 'Receive in outlet',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mOut.Model.Data.ReceiveInOutlet = e.ID;
                            mButReceiveInOutlet.text(e.FowstName).button('refresh');
                        }
                    }
                ], { 
                    IsThirdPartyOrganization: 1,
                    ThirdPartyOrganizationID: mOut.Model.Data.ReceiveInOrgID
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        var mButReceiveInDept = mElement.find('[name=butReceiveInDept]');
        mButReceiveInDept.click(function() {
            BrowsePopup('LibraryFowstBin', 'Receive in department',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mOut.Model.Data.ReceiveInBinID = e.ID;
                            mButReceiveInDept.text(e.FowstName).button('refresh');
                        }
                    }
                ], { 
                    FowstID: mOut.Model.Data.ReceiveInOutlet
                }
            );
        });

        var mTxtSupplier = mElement.find('[name=txtSupplier]');
        mTxtSupplier.click(BrowseSupplier);

        var mButSupplier = mElement.find('[name=butSupplier]');
        mButSupplier.click(BrowseSupplier);

        function BrowseSupplier() {
            BrowsePopup('LibraryOrganization', 'Vendor',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mOut.Model.Data.SupplierID = e.ID;
                            mTxtSupplier.val(e.CompanyName);
                        }
                    }
                ], { 
                    FilterSupplier: 1 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        var mButReceiveFromOutlet = mElement.find('[name=butReceiveFromOutlet]');
        mButReceiveFromOutlet.click(function() {
            BrowsePopup('LibraryFOWST', 'Receive from outlet',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mOut.Model.Data.ReceiveFromFowstID = e.ID;
                            mButReceiveFromOutlet.text(e.FowstName).button('refresh');
                        }
                    }
                ], { 
                    IsThirdPartyOrganization: 1,
                    ThirdPartyOrganizationID: mOut.Model.Data.SupplierID
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        var mButReceiveFromDept = mElement.find('[name=butReceiveFromDept]');
        mButReceiveFromDept.click(function() {
            BrowsePopup('LibraryFowstBin', 'Receive from department',
                [
                    {
                        //Search: mTxtSalesPartner.val(),
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mOut.Model.Data.ReceiveFromBinID = e.ID;
                            mButReceiveFromDept.text(e.FowstName).button('refresh');
                        }
                    }
                ], { 
                    FowstID: mOut.Model.Data.ReceiveFromFowstID
                }
            );
        });

        mElement.find('[name=butSaveAndFinalize]').click(function() {
            mOut.Model.Data.CopyProductsFromInvoiceID = null;
            if (mSavedAsFinal) {
                NewMessageBox('This transaction was saved as final. Some fields may still be updated. Do you want to update invoice reference?', 'Saved As Final', [
                    {
                        Text: 'Save',
                        Click: function() {
                            PreSave({
                                SaveAndFinalize: true,
                                SaveSomeFields: true,
                                NoPrompt: true
                            });
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);
                return;
            }
            PreSave({
                SaveAndFinalize: true
            });
        });

        mElement.find('[name=butSaveAsDraft]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            PreSave();
        });

        var pProductTable = mElement.find('[name=tableProducts]');
        var pProductRow = pProductTable.find('tbody tr');
        pProductRow.remove();
        mElement.find('[name=butAddProduct]').click(function(e) {
            e.preventDefault();

            ReceivingUpdateProductPrices();
            if (!HandleEditingSavedAsFinal())
                return;
            if (mOut.Model.Data.SupplierID == null) {
                if (CStr2(mOut.TransType) == 'StockTransfer') 
                    NewMessageBox('Please select a Transfer From Organization first.');
                else
                    NewMessageBox('Please select a vendor first.');
                return;
            }
            AddProduct();
            return false;
        }); // END mElement.find('[name=butAddProduct]').click(function() {

        function ReceivingUpdateProductPrices() {
            var pUpdate;
            pUpdate = false;
            if (pUpdate == false)
                return;
            AjaxSend({
                url: 'CreditAndCollection/ReceivingUpdateProductPrices',
                data: {
                    ReceivingID: mOut.Model.Data.ID
                },
                success: function(vData) {
                    console.log(vData);
                }
            });
        }

        var mTargetOrgType = 'vendor';
        function AddProduct(vParams) {
            vParams = $.extend({
                FilterSupplierID: null
            }, vParams);
            if (!HandleEditingSavedAsFinal())
                return;
            if (mOut.Model.Data.SupplierID == null) {
                NewMessageBox('Please select a ' + mTargetOrgType + ' first.');
                return;
            }
            var pColumns = [
                { 
                    Column: 'ProductNo', 
                    Label: 'Product No.' 
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name',
                    CreatedCell: function (td, cellData, rowData, row, col) {
                        $(td).css('min-width', '300px');
                    }
                }
            ];
            var pAdditionalData = {
                IsAll:1,
                UseCacheProductSearch: 0,
                InventoryFowstID: sessionStorage.FowstID,
                FilterSupplierID: vParams.FilterSupplierID,
                DontIncludeRetailPrice: 1
            };

            if (mTargetOrgType == 'vendor') {
                pAdditionalData.VendorID = mOut.Model.Data.SupplierID;
                pColumns.push({
                    Label: 'Vendor Price' ,
                    Column: function(vData) {
                        return AccountingFormat(vData.VendorPrice);
                    }
                });
            } else {
                pAdditionalData.FilterCustomers = 1;
                pColumns.push({
                    Label: 'Retail Price' ,
                    Column: function(vData) {
                        return AccountingFormat(vData.RetailPrice);
                    }
                });
            }
            if (mOut.TransType == 'Invoice')
                pColumns = pColumns.concat([
                    {
                        Label: 'Served Qty',
                        Render: function(v1, v2, vData) {
                            return '<div name="divQty" />';
                        }
                    }
                ]);
            else
                pColumns = pColumns.concat([
                {
                    Label: 'Qty',
                    Render: function(v1, v2, vData) {
                        return '<div name="divQty" />';
                        //return '<input class="qty" style="width:70px" />';
                    }
                }
            ]);
            pColumns.push({
                Label: 'Packing Size (in smallest unit)',
                Render: function(v1, v2, vData) {
                    return CStr2(vData.UnitMeasure);
                }
            });
            pColumns.push({
                Label: 'Qty Available to Commit',
                Render: function(v1, v2, vData) {
                    var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
                    p = '<div style="text-align: center">' + p + '</div>';
                    return p;
                }
            });
            pColumns.push(
                {
                    Label: 'Action',
                    Render: function(v1, v2, vData) {
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

            BrowsePopup('LibraryPIMProduct', 'Product',
                [
                    {
                        //we put SelectMultiple: true para dili maclose ang pop-up pagnaclick
                        ModalSize: 'xl',
                        DontCloseWhenClick: true,
                        AutoSearch: false,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            if ($.isArray(e)) {
                                PreSelect({
                                    done: function() {
                                        if (!e.FromCacheProductSearch)
                                            CacheProductSearch();
                                    }
                                });
                            }
                            //else {
                            //    SelectProduct(e);
                            //}

                            //if (!e.FromCacheProductSearch)
                            //    CacheProductSearch();
                        },
                        AllowEmptySearch: false,
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
                                    mInventoryBalance.Show({
                                        ProductID: data.ID,
                                        ProductNo: data.ProductNo,
                                        ProductName: data.ProductName,
                                        OrderController: mOut,
                                        AfterSave: function() {
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
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            pDiv.append('<input class="qty" style="width:70px" />');
                        }
                    }
                ], pAdditionalData
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function AddProduct(vParams) {

        function SelectProduct(e) {
            ERP.Receiving.product.controller.popup({
                TransID: mOut.Model.Data.ID,
                ProductID: e.ID,
                ProductName: e.ProductName,
                ProductNo: e.ProductNo,
                CustomerID: mOut.Model.Data.CustomerID,
                TransType: mOut.TransType,
                OnSave: function(vProducts) {
                    mOut.Model.Data.Products = vProducts;
                    PreSave({
                        SaveOnlyProduct: false,
                        NoPrompt: true,
                        Success: function() {
                            NewMessageBox('Products were successfully saved.');
                            LoadTrans({
                                ID: mOut.Model.Data.ID,
                                LoadProductsOnly: true
                            });
                        }
                    });
                },
                Success: function() {
                    
                }
            });
        }

        function PreSelect(vParams) {
            vParams = $.extend({
                done: function() {
                }
            }, vParams);

            var pTxts = $('#tblBrowsePopup .qty');
            var pData = [];
            var pIndex;
            for (var p = 0; p <= pTxts.length - 1; p++) {
                var pTxt = $(pTxts[p]);
                var pQty = CDec2(pTxt.val());
                if (pQty > 0) {
                    var pTR = pTxt.closest('tr');
                    var pDR = pTR.data('data');
                    pData.push({
                        ID: pDR.ID,
                        Qty: pQty
                    });
                } // todo: if qty = 0, delete at server
            }
            
            if (pData.length > 0) {
            	$.mobile.showPageLoadingMsg();
                function Proceed() {
                    var pParams = $.extend({
                        Fail: function(vError) {
                    	    $.mobile.hidePageLoadingMsg();
                            NewMessageBox(vError);
                            AfterProductSave();
                            vParams.done();
                        },
                        Success: function() {
                            pIndex++;
                            if (pIndex <= pData.length - 1)
                                Proceed();
                            else {
                    	        $.mobile.hidePageLoadingMsg();
                                AfterProductSave();
                                vParams.done();
                            }
                        }
                    }, pData[pIndex]);
                    SavePreSelected(pParams);
                }
                pIndex = 0;
                Proceed();
            }
        }

        function SavePreSelected(vParams) {
            mOut.Model.Data.Products = {    
                ProductID: vParams.ID,
                Items: [
                    {
                        UnitQty: 1,
                        GoodQty: vParams.Qty
                    }
                ]
            };
            PreSave({
                SaveOnlyProduct: false,
                NoPrompt: true,
                Success: function() {
                    vParams.Success();
                    
                    //NewMessageBox('Products were successfully saved.');
                    //LoadTrans({
                    //    ID: mOut.Model.Data.ID,
                    //    LoadProductsOnly: true
                    //});
                }
            });
        }

        function AfterProductSave() {
            LoadTrans({
                ID: mOut.Model.Data.ID,
                LoadProductsOnly: true
            });
        }

        function AddProduct_old() {
            //if (!HandleEditingSavedAsFinal())
            //    return;
            //if (mOut.Model.Data.CustomerID == null) {
            //    NewMessageBox('Please select a customer first.');
            //    return;
            //}

            BrowsePopup('LibraryPIMProduct', 'Product',
                [
                    {
                        AutoSearch: false,
                        DisplayText: [
                            { 
                                Column: 'ProductNo', 
                                Label: 'Product No.' 
                            },
                            { 
                                Column: 'ProductName', 
                                Label: 'Product Name' 
                            }
                        ],
                        Click: function (e) {
                            ERP.Receiving.product.controller.popup({
                                TransID: mOut.Model.Data.ID,
                                ProductID: e.ID,
                                ProductName: e.ProductName,
                                ProductNo: e.ProductNo,
                                CustomerID: mOut.Model.Data.CustomerID,
                                TransType: mOut.TransType,
                                OnSave: function(vProducts) {
                                    mOut.Model.Data.Products = vProducts;
                                    PreSave({
                                        SaveOnlyProduct: false,
                                        NoPrompt: true,
                                        Success: function() {
                                            NewMessageBox('Products were successfully saved.');
                                            LoadTrans({
                                                ID: mOut.Model.Data.ID,
                                                LoadProductsOnly: true
                                            });
                                        }
                                    });
                                },
                                Success: function() {
                    
                                }
                            });
                        },
                        StyleFormat: 'table',
                        AllowEmptySearch: false
                    }
                ], { FilterCustomers:1, IsAll: 1 }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function PreSave(vParams) {
            vParams = $.extend({
                SaveOnlyProduct: false,
                SaveAndFinalize: false,
                SaveACopy: false,
                NoPrompt: false,
                Success: function(){
                    NewMessageBox('Transaction was successfully saved.');
                },
                SaveSomeFields: false
            }, vParams);

            mOut.Model.Data.TransType = mOut.TransType;
            mOut.Model.Data.ReceivingPlaceID = sessionStorage.FowstID;
            mOut.Model.Data.InvoiceNumber = mTxtInvoiceNumber.val();
            mOut.Model.Data.InvoiceDate = mTxtInvoiceDate.val();
            mOut.Model.Data.SalesRepText = mTxtSalesRep.val();
            mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
            mOut.Model.Data.Remarks = mTxtRemarks.val();
            
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
                                        SaveOnlyProduct: vParams.SaveOnlyProduct,
                                        SaveAndFinalize: vParams.SaveAndFinalize,
                                        Success: vParams.Success
                                    });
                                }
                            },
                            {
                                Text: 'No'
                            }
                        ]);
                        return; // debug only
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            } else {
                Save({
                    SaveOnlyProduct: vParams.SaveOnlyProduct,
                    Success: vParams.Success,
                    SaveSomeFields: vParams.SaveSomeFields
                });
            }
        } // END function PreSave(vParams) {

        function Save(vParams) {
            vParams = $.extend({
                Success: function() {
                    NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);
            mOut.Model.Save({
                SaveOnlyProduct: vParams.SaveOnlyProduct,
                SaveSomeFields: vParams.SaveSomeFields,
                Success: function(vData) {
                    if (vData.Result != 'ok')
                        return;
                    var pID = mOut.Model.Data.ID;
                    if (vData.Receiving)
                        pID = vData.Receiving.ID;
                    if (IsGuid(pID))
                        mOut.Model.Data.ID = pID;
                    LoadTrans({
                        ID: pID,
                        AfterLoad: function() {
                            vParams.Success(vData);
                            mSavedAsFinal = vParams.SaveAndFinalize;
                            //if (CBool(mSavedAsFinal))
                            //    NewMessageBox('Do you want to print this transaction?', 'Print?', [
                            //        {
                            //            Text: 'Yes',
                            //            Click: function() {
                            //                HandlePrint({
                            //                    NewTransAfterPrint: true
                            //                });
                            //            }
                            //        },
                            //        {
                            //            Text: 'No'
                            //        }
                            //    ]);
                        }
                    });
                    //if (vData.Receiving) {
                    //    if (CStr2(vData.Receiving.Num) != '')
                    //        $('body').attr('title', vData.Receiving.Num);
                    //    if (CStr2(vData.Receiving.ID) != '')
                    //        mOut.Model.Data.ID = vData.Receiving.ID;
                    //}
                    //vParams.Success(vData);
                    //mSavedAsFinal = vParams.SaveAndFinalize;
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }

        mElement.find('[name=butLookup]').click(function() {
            Find({
                TransType: mOut.TransType
            });
        });

        function Find(vParams) {
            var pTitle;
            var pColumns = [];
            var pIsReceiving;

            if ((vParams.TransType == 'Receiving')
                || (vParams.TransType == 'StockReceiving')) {
                pTitle = 'Stock Receivings';
                pIsReceiving = 1;
                pColumns = [
                    {
                        Label: 'SR No.',
                        Column: function(vDR) {
                            //var pOut = '';
                            //if (CStr2(vDR.MIN).trim() != '')
                            //    pOut = vDR.MIN + '-';
                            return 'SR-' + vDR.MIN;
                        }
                    }, 
                    {
                        Label: 'SR Date Created',
                        Column: 'DateReceived'
                    },
                    { // 2
                        Label: 'SR Last Updated',
                        Column: 'LastUpdated'
                    },
                    {
                        Label: 'SR Updated By',
                        Column: 'UpdatedBy'
                    },
                    {
                        Label: 'PO No.',
                        Column: function(vDR) {
                            return CStr2(vDR.FullPurchaseOrderNum);
                        }
                    },
                    {
                        Label: 'PO Last Updated',
                        Column: 'PoLastUpdated'
                    },
                    {
                        Label: 'Vendor',
                        Column: 'SourceOfSKU2'
                    },
                    {
                        Label: 'Vendor Invoice Ref.',
                        Column: function(vDR) {
                            return CStr2(vDR.InvoiceNumber);
                        }
                    },
                    {
                        Label: 'SKU Count',
                        Column: 'SKUCount'
                    }
                ];
            } else if (vParams.TransType == 'StockTransfer') {
                pTitle = 'Stock Transfers';
                pColumns = [
                    {
                        Label: 'STR No.',
                        Column: function(vDR) {
                            //var pOut = '';
                            //if (CStr2(vDR.MIN).trim() != '')
                            //    pOut = vDR.MIN + '-';
                            return 'STR-' + vDR.MIN;
                        }
                    },
                    {
                        Label: 'Vendor',
                        Column: 'SourceOfSKU2'
                    },
                    {
                        Label: 'Date Created',
                        Column: 'DateReceived'
                    },
                    {
                        Label: 'Vendor Invoice',
                        Column: 'InvoiceNumber'
                    }
                ];
            }

            pColumns = pColumns.concat([
                {
                    Label: 'TransportedBy',
                    Column: 'Transported By'
                }
            ]);

            if (vParams.Click == null)
                vParams.Click = function (e) {
                    LoadTrans({
                        ID: e.ID,
                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                        AfterLoad: function() {
                            if (vParams.AfterLoad)
                                vParams.AfterLoad({
                                    ID: e.ID
                                });
                        } // END AfterLoad: function() {
                    });
                }; // END Click: function (e) {

            BrowsePopup('TransReceivingAndTransfers', pTitle,
                [
                    {
                        ModalSize: 'xl',
                        StyleFormat: 'table',
                        AutoSearch: false,
                        DisplayText: pColumns,
                        Click: vParams.Click,
                        Sorting: false
                    }
                ], $.extend({ 
                    IsReceiving: pIsReceiving
                }, vParams.QueryParams)
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function Find(vParams) {

        mOut.LoadTrans = LoadTrans;
        function LoadTrans(vParams) {
            vParams = $.extend({
                ID: null,
                LoadProductsOnly: false,
                DontReplaceCurrentID: false,
                Data: null
            }, vParams);
            mOut.Model.GetData({
                ID: vParams.ID,
                Data: vParams.Data,
                Success: function(vData) {
                    if (!vParams.LoadProductsOnly)
                        DisplayTrans(vData, {
                            DontReplaceCurrentID: vParams.DontReplaceCurrentID
                        });
                    DisplayProducts(vData);
                    if (vParams.AfterLoad)
                        vParams.AfterLoad();

                    ERP.InvoiceImage.main.controller.GetInvoiceRefImages(function (e) {
                        var vRows = e.Results.Rows;
                        var pInvRefOld = mElement.find('[name="txtInvoiceNumber"]').val().trim();
                        var pInvRef = vRows.map(function(elem) {
                            return elem.InvoiceRefNo;
                        }).join(', ');
                        if ((pInvRefOld != '')
                            && (pInvRef != ''))
                            pInvRef = pInvRefOld + ', ' + pInvRef;
                        else if (pInvRefOld != '')
                            pInvRef = pInvRefOld;
                        mElement.find('[name="txtInvoiceNumber"]').val(pInvRef); 
                        //if (vRows.length > 0) {
                        //    mElement.find('[name="txtInvoiceNumber"]').val(pInvRef); 
                        //} else {
                        //     mElement.find('[name="txtInvoiceNumber"]').val('');
                        //}
                    }, {
                        mod: 'Receiving',
                        ID: mOut.Model.Data.ID
                    });
                }
            });
        }

        var mTxtSrNum = mElement.find('[name=txtSrNum]');
        var mTxtSrDate = mElement.find('[name=txtSrDate]');
        var mTxtSalesRep = mElement.find('[name=txtSalesRep]');
        function DisplayTrans(vData) {
            var pStr;

            mSavedAsFinal = IsGuid(vData.Master.SaveAndFinalizedByID);
            mDivDraftOrFinalized.text(mSavedAsFinal ? 'FINALIZED' : 'DRAFT');
            mOut.Model.Data.ID = vData.Master.ID;
            mOut.Model.Data.SupplierID = vData.Master.SupplierID;
            mTxtSupplier.val(vData.Master.SupplierName);
            mTxtTransferFromOrg.val(vData.Master.SupplierName); // stock transfer HTML
            mTxtTransferFromOutlet.val(vData.Master.ReceiveFromOutletName); // stock transfer HTML
            mTxtTransferFromDepartment.val(vData.Master.TransferFromFowstBinName); // stock transfer HTML
            mTxtTransferToOrg.val(vData.Master.ReceivingPlaceOrg); // stock transfer HTML
            mTxtTransferToOutlet.val(vData.Master.ReceivingPlace); // stock transfer HTML
            mTxtTransferToDepartment.val(vData.Master.TransferToFowstBinName); // stock transfer HTML

            mTxtSrNum.val(vData.Master.MIN || vData.Master.ReceivingNo);
            mTxtSalesRep.val(vData.Master.SalesRepText);
            mTxtSrDate.val($.format.date(new Date(vData.Master.DateReceived), 'M/d/yyyy h:mm a'));
            mTxtInvoiceNumber.val(vData.Master.InvoiceNumber);
            mTxtPaymentTerms.val(vData.Master.PaymentTerm);
            if (IsDate(vData.Master.InvoiceDate))
                mTxtInvoiceDate.val($.format.date(new Date(vData.Master.InvoiceDate), 'M/d/yyyy h:mm a'));
            else
                mTxtInvoiceDate.val('');
            mOut.Model.Data.BillingAddressID = vData.Master.BillingAddressID;
            mTxtBillingAddress.val(vData.Master.BillingAddress);
            mOut.Model.Data.DeliveryAddressID = vData.Master.DeliveryAddressID;
            mTxtDeliveryAddress.val(vData.Master.DeliveryAddress);
            pStr = CInt2(vData.Master.PurchaseOrderNo);
            if ((vData.Master.PoMin == null) 
                && (pStr == 0))
                pStr = '';
            else if (vData.Master.PoMin != null)
                pStr = vData.Master.PoMin + '-' + pStr;
            mTxtPoNumber.val(pStr);
            if (CStr2(vData.Master.PurchaseOrderDate).trim() != '')
                mTxtPoDate.val($.format.date(new Date(vData.Master.PurchaseOrderDate), 'M/d/yyyy h:mm a'));
            else
                mTxtPoDate.val('');
            mTxtRemarks.val(vData.Master.Remarks);
            mTxtCanvassNumber.val(vData.Master.CanvassNumber);
            if (CStr2(vData.Master.CanvassDate).trim() != '')
                mTxtCanvassDate.val($.format.date(new Date(vData.Master.CanvassDate), 'M/d/yyyy h:mm a'));
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
            if (mOut.TransType == 'StockTransfer')
                mElement.find('[name=divTransNum]').text('STR-' + vData.Master.MIN);
            else
                mElement.find('[name=divTransNum]').text('SR-' + vData.Master.MIN);
            mElement.find('[name=divUpdatedBy]').text('Updated By: ' + CStr2(vData.Master.UpdatedBy));
            mElement.find('[name=divLastUpdated]').text('Last Updated: ' + FormatDate(vData.Master.LastUpdated, {Format: 'MM/dd/yyyy h:mm a'}));
        }

        function DisplayProducts(vData) {
            var pTotal = 0;
            var pLineFreightCost = 0;
            var pTotalSubtotalBad = 0;
            var pTotalSubtotalGoodBad = 0;
            var pTotalLineLandedCost = 0;
            pProductTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Products.Rows.length - 1; p++) {
                var pUnit;
                var pDR = vData.Products.Rows[p];
                var pTR = pProductRow.clone();
                pTR.data('data', pDR);
                if (pDR.Image != null) {
                    pTR.find('[name=ProductImage]').append($('<img src="/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Small/' + pDR.Image + '" />')
                            .data('data', pDR)
                            .click(function (e) {
                                var rowData = $(this).data('data');
                                if (rowData.Image != null) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    ShowProductImage(rowData.ProductID);
                                }
                            })
                        );
                }
                pTR.find('[name=ProductNumber]').text(pDR.ProductNo);
                pTR.find('[name=ProductDescription]').text(pDR.ProductName);
                pTR.find('[name=QtyPo]').text(AccountingFormat(pDR.PoQty, null, ''));
                pTR.find('[name=QtyGood]').text(AccountingFormat(pDR.QuantityGood));
                var p2ndLine = AccountingFormat(pDR.QuantityDamaged);
                if (CStr2(pDR.BadOrderReasonName).trim() != '')
                    p2ndLine += '<br />' + pDR.BadOrderReasonName;
                pTR.find('[name=QtyBad]').html(p2ndLine);
                pUnit = pDR.UnitMeasure;
                if (CDec2(pDR.ActualQuantityGood) != 1)
                    pUnit += ' (' + $.format.number(CDec2(pDR.ActualQuantityGood), '#,###.##') + ' ' + pDR.BaseUnit + ')';
                pTR.find('[name=PackingSize]').text(pUnit);
                var pUnitCost = pDR.PoUnitPrice; // pDR.LandedCost;
                pUnitCost = CDec2(pUnitCost);
                if (pUnitCost == 0) {
                    pTR.find('[name=UnitCost]').text('');
                    pTR.find('[name=Subtotal]').text('');
                } else {
                    //pUnitCost = pUnitCost * CDec2(pDR.ActualQuantityGood);
                    pTR.find('[name=UnitCost]').text(AccountingFormat(pUnitCost));
                    pUnitCost *= CDec2(pDR.QuantityGood);
                    pTR.find('[name=Subtotal]').text(AccountingFormat(pUnitCost));
                    pTotal += pUnitCost;
                }
                pTR.find('[name=LineFreightCost]').text(AccountingFormat(pDR.LineFreightCost));
                pLineFreightCost += CDec2(pDR.LineFreightCost);
                pTR.find('[name=UnitFreightCost]').text(AccountingFormat(pDR.UnitFreightCost));
                pTR.find('[name=SubtotalBad]').text(AccountingFormat(pDR.SubtotalBad));
                pTotalSubtotalBad += CDec2(pDR.SubtotalBad);
                pTR.find('[name=SubtotalGoodBad]').text(AccountingFormat(pDR.SubtotalGoodBad));
                pTotalSubtotalGoodBad += CDec2(pDR.SubtotalGoodBad);
                pTR.find('[name=LineLandedCost]').text(AccountingFormat(pDR.LineLandedCost));
                pTotalLineLandedCost += CDec2(pDR.LineLandedCost);
                pTR.find('[name=FreightPercent]').text(AccountingFormat(pDR.FreightPercent) + '%');
                pTR.find('[name=UnitLandedCost]').text(AccountingFormat(pDR.UnitLandedCost));
                //(pTR.click(function(vProductID) {
                //    Item_Click(vProductID);
                //}))(pDR.ProductID);
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    Item_Click(pDR);
                });
                pProductTable.append(pTR);
            }
            pProductTable.find('tfoot tr td[name=Subtotal]').text(AccountingFormat(pTotal));
            pProductTable.find('tfoot tr td[name=LineFreightCost]').text(AccountingFormat(pLineFreightCost));
            pProductTable.find('tfoot tr td[name=SubtotalBad]').text(AccountingFormat(pTotalSubtotalBad));
            pProductTable.find('tfoot tr td[name=SubtotalGoodBad]').text(AccountingFormat(pTotalSubtotalGoodBad));
            pProductTable.find('tfoot tr td[name=LineLandedCost]').text(AccountingFormat(pTotalLineLandedCost));
            mOut.Model.Data.ProductVendorCost = pTotal;
            HandleShowLandedCost();
        }

        function Item_Click(vDR) {
            if (!HandleEditingSavedAsFinal())
                return;
            vDR = $.extend({}, vDR);
            PopupButtonList('', vDR.ProductName, [
                {
                    Text: 'Edit Stock Receiving Quantity',
                    Click: function() {
                        ERP.Receiving.product.controller.popup({
                            ReceivingStockID: vDR.ID,
                            TransID: mOut.Model.Data.ID,
                            ProductID: vDR.ProductID,
                            ProductName: vDR.ProductName,
                            ProductNo: vDR.ProductNo,
                            CustomerID: mOut.Model.Data.CustomerID,
                            TransType: mOut.TransType,
                            OnSave: function(vProducts) {
                                var pOldID = mOut.Model.Data.ID;
                                mOut.Model.Data.Products = vProducts;
                                PreSave({
                                    SaveOnlyProduct: false,
                                    NoPrompt: true,
                                    Success: function() {
                                        NewMessageBox('Products were successfully saved.');
                                        LoadTrans({
                                            LoadProductsOnly: CStr2(pOldID) != '',
                                            ID: mOut.Model.Data.ID
                                        });
                                    }
                                });
                            },
                            Success: function() {
                    
                            }
                        });
                    }
                },
                {
                    Text: 'Add Product',
                    Click: function() {
                        AddProduct();
                    }
                },
                {
                    Text: 'Enter Serial Numbers and IMEIs',
                    Click: function () {
                        mSerialNumbersPopup.popup({
                            InvoiceID: mOut.Model.Data.ID,
                            ProductID: vDR.ProductID,
                            ProductName: vDR.ProductName,
                            ProductNo: vDR.ProductNo,
                            CustomerID: mOut.Model.Data.CustomerID,
                            TransType: mOut.TransType,
                            ParentController: mOut,
                            Success: function () {
                            }
                        });
                    }
                },
                {
                    Text: 'Remove From Cart',
                    Click: function() {
                        RemoveFromCartPrompt(vDR);
                    }
                }
            ]);
        }

        function RemoveFromCartPrompt(vDR) {
            NewMessageBox('Are you sure you want to remove ' + vDR.ProductName + ' from cart?', 'Remove from cart', [
                {
                    Text: 'Yes',
                    Click: function() {
                        mOut.Model.RemoveProduct({
                            ReceivingID: mOut.Model.Data.ID,
                            ID: vDR.ID,
                            ProductID: vDR.ProductID,
                            UnitMeasureID: vDR.UnitMeasureGood,
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

        function FindPurchaseOrder(vParams) {

            var pTitle;
            var pColumns = [];

            NewTrans();
            if (vParams.TransType == 'Quote') {
                pTitle = 'Quotations';
                pColumns = [
                    {
                        Label: 'Quotation No.',
                        Column: 'QuotationNo'
                    }
                ];
            } else if (vParams.TransType == 'SalesOrder') {
                pTitle = 'Sales Order';
                pColumns = [
                    {
                        Label: 'Sales Order No.',
                        Column: 'SalesOrderNo'
                    }
                ];
            } else if (vParams.TransType == 'Invoice') {
                pTitle = 'Invoice';
                pColumns = [
                    {
                        Label: 'Invoice No.',
                        Column: function(vDR) {
                            if (CStr2(vDR.DocumentName).trim() == '')
                                return 'SI-' + vDR.InvoiceNum;
                            else
                                return vDR.DocumentName + '-' + vDR.InvoiceNum;
                        }
                    },
                    {
                        Label: 'Manual Invoice No.',
                        Column: 'RefInvoiceNum'
                    }
                ];
            } else if ((vParams.TransType == 'PurchaseOrder')
                || (vParams.TransTypes == 'PurchaseOrderAndInvoice')) {
                pTitle = 'Purchase Order';
                pColumns = [
                    {
                        Label: 'PO No.',
                        Column: function(vDR) {
                            var pOut = '';
                            var pNum = '';
                            var pPrefix = 'PO-';
                            if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-';
                            if (vDR.TransType == 'Invoice') {
                                pNum = vDR.InvoiceNumber;
                                pPrefix = 'SI-';
                            } else
                                pNum = vDR.PurchaseOrderNo;
                            if (!IsNumeric(pNum))
                                pNum = 0;
                            return '<a href="#">' + pPrefix + pOut + pNum + '</a>';
                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    Bs.Loading('show');
                                    var rowData = $(this).data('data');
                                    mOut.Model.GetData({
                                        ID: rowData.ID,
                                        Data: {
                                            CreateFromPoID: rowData.ID,
                                            FowstID: sessionStorage.FowstID
                                        },
                                        Success: function (vData) {
                                            Bs.Loading('hide');
                                            var msg = '', grandtotal = 0;
                                            var headerdata = vData.Master, skus = vData.Products.Rows;

                                            msg += '<b>Vendor:</b> ' + headerdata.SupplierName + '<br>';
                                            msg += '<b>PO No.:</b> ';

                                            var pStr = headerdata.MIN;
                                            if (CStr2(pStr).trim() != '')
                                                pStr += '-';
                                            pStr += CInt2(headerdata.PurchaseOrderNo);

                                            msg += pStr + '<br>';

                                            if (CStr2((headerdata.PurchaseOrderDate)) != '')
                                                msg += '<b>PO Date:</b> ' + $.format.date(new Date(headerdata.PurchaseOrderDate), 'MM-dd-yy') + '<br>';

                                            msg += '<br><b>ITEMS:</b> <br><br>';

                                            for (var i = 0; i < skus.length; i++) {
                                                var pDR = skus[i];
                                                var linetotal = CDec2(pDR.PoQty) * CDec2(pDR.PoUnitPrice);
                                                grandtotal += linetotal
                                                //var pDrProduct = FilterByProductID(pDR.ProductID, vData, pDR.VendorID);
                                                msg += (i + 1) + '.<br>';
                                                msg += 'PN: ' + pDR.ProductNo + '<br>';
                                                msg += 'Brand: ' + pDR.Brand + '<br>';
                                                msg += 'Desc: ' + pDR.ProductNameOnly + '<br>';
                                                msg += '= ' + pDR.PoQty + ' ' + pDR.UnitMeasure + ' @ ' + AccountingFormat(pDR.PoUnitPrice, 4) + '<br>';
                                                msg += '= Line Total: ' + AccountingFormat(linetotal, 4) + '<br><br>';
                                            }

                                            msg += '*** <br><br>';
                                            msg += '= GRAND TOTAL: ' + AccountingFormat(grandtotal, 4) + '<br><br>';

                                            if (CStr2(headerdata.Remarks) != '') {
                                                msg += '***<br><br>REMARKS: <br>';
                                                msg += headerdata.Remarks;
                                            }

                                            NewMessageBox(msg, 'Peek View');
                                        }
                                    });
                                })
                        }
                    }
                ];
            }

            pColumns = pColumns.concat([
                {
                    Label: 'Vendor',
                    Column: 'CompanyName'
                },
                {
                    Label: 'Amount',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.Total);
                    }
                },
                {
                    Label: 'SKU Count',
                    Column: 'SkuCount'
                },
                {
                    Label: 'Last Updated',
                    Column: 'LastUpdated'
                },
                {
                    Label: 'Updated By',
                    Column: 'EmployeeFullName'
                },
                {
                    Label: 'Received Percent',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.PoReceivedPercent);
                    }
                }
            ]);

            if (vParams.Click == null)
                vParams.Click = function (e) {
                    LoadTrans({
                        ID: e.ID,
                        Data: {
                            CreateFromPoID: e.ID,
                            FowstID: sessionStorage.FowstID
                        },
                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                        AfterLoad: function() {
                            if (vParams.AfterLoad)
                                vParams.AfterLoad({
                                    ID: e.ID
                                });
                        } // END AfterLoad: function() {
                    });
                }; // END Click: function (e) {
                
            BrowsePopup('TransInvoices', pTitle,
                [
                    {
                        ModalSize: 'fullwidth',
                        StyleFormat: 'table',
                        AutoSearch: false,
                        DisplayText: pColumns,
                        Click: vParams.Click,
                        ShowPoAndInvoiceRadios: true
                    }
                ], $.extend({ 
                    TransType: vParams.TransType,
                    TransTypes: vParams.TransTypes,
                    IncludeFinalizedInvoices: 1,
                    AnyZero: 1,
                    FinalizedPoOnly: 1,
                    NoReceiving: 1
                }, vParams.QueryParams)
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function Find(vParams) {

        var mTxtRemarks = mElement.find('[name=txtRemarks]');
        mTxtRemarks.on('keypress', function() {
            if (!HandleEditingSavedAsFinal())
                return;
        });

        mElement.find('[name="btnChooseQuickRemarks"]').click(function () {
            if (!HandleEditingSavedAsFinal())
                return;

            BrowsePopup('/ERP/GetQuickRemarks', 'Quick Remarks', [{
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
                    OnLoad: function () {
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
                    mTxtRemarks.val(mTxtRemarks.val() + '' + e.Message + ' \n\n');
                }
            }])
        })

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

        function NewTrans() {
            mSavedAsFinal = false;
            mDivDraftOrFinalized.text('DRAFT');
            mOut.Model = new ERP.Receiving.work.model();
            mTxtSupplier.val('');
            mTxtInvoiceNumber.val('');
            mTxtInvoiceDate.val(FormatDate(new Date()));
            mTxtSrNum.val('');
            mTxtSrDate.val('');
            //mTxtPaymentTermsText.val('');
            //mTxtSalesPartner.val('');
            mTxtSalesRep.val('');
            //mTxtDateNeeded.val('');
            mTxtBillingAddress.val('');
            mTxtDeliveryAddress.val('');
            mTxtRemarks.val('');
            mTxtPoNumber.val('');
            mTxtPoDate.val('');
            pProductTable.find('tbody tr').remove();
            pProductTable.find('tfoot tr td[name=LineTotal]').text('');
            mTxtCanvassNumber.val('');
            mTxtCanvassDate.val('');
            mOut.Model.Data.BillingAddressID = sessionStorage.FowstID;
            mOut.Model.Data.DeliveryAddressID = sessionStorage.FowstID;
            mTxtBillingAddress.val(sessionStorage.FowstAddress);
            mTxtDeliveryAddress.val(sessionStorage.FowstAddress);
            mTxtPaymentTerms.val('');
            mElement.find('[name=divTransNum]').text('');
            mTxtTransferFromOrg.val(sessionStorage.OrgName);
            mOut.Model.Data.SupplierID = sessionStorage.OrgID;
            mTxtTransferFromOutlet.val(sessionStorage.FowstName);
            mOut.Model.Data.ReceiveFromOutlet = sessionStorage.FowstID;

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

            // stock transfer
            mTxtTransferToOrg.val('');
            mTxtTransferToOutlet.val('');
            mTxtTransferFromDepartment.val('');
            mTxtTransferToDepartment.val('');
        }

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

        mElement.find('[name=butOutput]').click(function() {

            var pCaption = 'Stock Receiving Report'
            if (mOut.TransType == 'StockTransfer')
                pCaption = 'Stock Transfer Report';
            NewMessageBox('What do you want to print?', 'Print?', [
                {
                    Text: pCaption,
                    Click: function() {
                        Proceed();
                    }
                },
                //{
                //    Text: 'Stock Receiving + PO',
                //    Click: function() {
                //        Proceed({
                //            Template: 'StockReceivingAndPO'
                //        });
                //    }
                //}
            ]);

            function Proceed(vParams) {
                vParams = $.extend({}, vParams);
                var pReceivingID = GetStringObject('Model.Data.ID', mOut);
                if (pReceivingID == null) {
                    NewMessageBox('Please select a transaction first.');
                    return;
                }
                AjaxSend({
                    url: '/Receiving/CreatePdf',
                    data: {
                        ReceivingID: pReceivingID,
                        PrintHash: localStorage.getItem('PrintHash'),
                        Template: vParams.Template,
                        TransType: mOut.TransType
                    },
                    success: function(vData) {
                        if (vData.Error) {
                            if (vData.Error.indexOf('PrintHash') == -1)
                                NewMessageBox(vData.Error);
                            else
                                NoPrintHash();
                        } else if (vData.Result == 'ok') {
                            ViewReport(vData.BaseFilename);
                            NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                        }
                    }
                });
            }
        });

        function ViewReport(vFilename) {
            var pUrl = MergeUrlParameters({
                Filename: vFilename
            }, '/Erp/DownloadPrintablePdf');
            window.open(pUrl, '_blank', 
                'width=' + window.innerWidth + 
                ',height=' + window.innerHeight + 
                ',scrollbars=auto');
        } // END function ViewReport() {

        function NoPrintHash() {
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

        mElement.find('[name=butCancel]').click(function() {
            var pReceivingID = GetStringObject('Model.Data.ID', mOut);
            if (pReceivingID == null) {
                NewMessageBox('Please select a transaction first.');
                return;
            }
            PromptUserAccess('Cancel Stock Receiving (EAS)', {
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
                    url: '/Receiving/CancelReceiving',
                    data: {
                        ReceivingID: pReceivingID
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

        mOut.ReadOnly = function() {
            mElement.find('[name=divToolbar]').hide();
            mElement.find('[name=butAddProduct]').hide();
            mElement.find('.btn-xs').hide();
            $('#divEasHeader, #divInvoicingTab, #divOrderProcessingTab').hide();
	        $('#divInvoicingTab, #divOrderProcessingTab').addClass('hidetab');
            mElement.find('#divOPWorkTab').css({
                position: 'absolute',
                top: '25px'
            })
            mElement.find('[name=divDraftOrFinalized]').css({
                position: 'absolute',
                right: 0
            });
            mElement.find('#accRevOrderHeader').customaccordion();
        };

        var mTxtTransferFromOrg = mElement.find('[name=txtTransferFromOrg]');
        mElement.find('[name=butTransferFromOrg]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowseTransferOrg({
                Title: 'Transfer From Organization',
                Success: function(vData) {
                    mTxtTransferFromOrg.val(vData.CompanyName);
                    mOut.Model.Data.SupplierID = vData.ID;
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
                    mOut.Model.Data.ReceivingPlaceID = vData.ID;
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
            if (!IsGuid(mOut.Model.Data.SupplierID)) {
                var pOrgTitle = 'Transfer From Organization';
                NewMessageBox('Please select a ' + pOrgTitle + ' first.');
                return;
            }
            BrowseTransferOutlet({
                Title: 'Transfer From Outlet',
                FromOrTo: 'From',
                OrgFieldName: 'SupplierID',
                Success: function(vData) {
                    mTxtTransferFromOutlet.val(vData.Data.FowstName);
                    mOut.Model.Data[vData.OutletFieldName] = vData.Data.ID;
                }
            });
        });

        var mTxtTransferToOutlet = mElement.find('[name=txtTransferToOutlet]');
        mElement.find('[name=butTransferToOutlet]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            if (!IsGuid(mOut.Model.Data.ReceivingPlaceID)) {
                var pOrgTitle = 'Transfer ' + vParams.FromOrTo + ' Organization';
                NewMessageBox('Please select a ' + pOrgTitle + ' first.');
                return;
            }
            BrowseTransferOutlet({
                Title: 'Transfer To Outlet',
                FromOrTo: 'To',
                OrgFieldName: 'ReceivingPlaceID',
                Success: function(vData) {
                    mTxtTransferToOutlet.val(vData.Data.FowstName);
                    mOut.Model.Data[vData.OutletFieldName] = vData.Data.ID;
                }
            });
        });

        function BrowseTransferOutlet(vParams) {
            vParams = $.extend({
                Title: null,
                FromOrTo: '', // From or To (case-sensitive)
                OrgFieldName: null,
                Success: function(vData) {
                    
                }
            }, vParams);
            if (!HandleEditingSavedAsFinal())
                return;
            var pOutletFieldName;
            if (vParams.FromOrTo == 'From') 
                pOutletFieldName = 'ReceiveFromOutlet';
            else
                pOutletFieldName = 'ReceiveInOutlet';
            
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
                    ThirdPartyOrganizationID: mOut.Model.Data[vParams.OrgFieldName]
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }
        
        var mTxtTransferFromDepartment = mElement.find('[name=txtTransferFromDepartment]');
        mElement.find('[name=butTransferFromDepartment]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            if (!IsGuid(mOut.Model.Data.ReceiveFromOutlet)) {
                NewMessageBox('Please select a Transfer From Outlet first.');
                return;
            }
            BrowseTransferDepartment({
                Title: 'Transfer From Department',
                FowstFieldName: 'ReceiveFromOutlet',
                Success: function(vData) {
                    mTxtTransferFromDepartment.val(vData.Name);
                    mOut.Model.Data.ReceiveFromDepartment = vData.ID;
                }
            });
        });

        var mTxtTransferToDepartment = mElement.find('[name=txtTransferToDepartment]');
        mElement.find('[name=butTransferToDepartment]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            if (!IsGuid(mOut.Model.Data.ReceiveInOutlet)) {
                NewMessageBox('Please select a Transfer To Outlet first.');
                return;
            }
            BrowseTransferDepartment({
                Title: 'Transfer To Department',
                FowstFieldName: 'ReceiveInOutlet',
                Success: function(vData) {
                    mTxtTransferToDepartment.val(vData.Name);
                    mOut.Model.Data.ReceiveInDepartment = vData.ID;
                }
            });
        });

        var mTxtTransportedBy = mElement.find('[name=txtTransportedBy]');
        mElement.find('[name=butTransportedBy]').click(function() {
            BrowsePopup('LibraryPeople', 'Employee', [
                {
                    DisplayText: [{ Column: 'PeopleName' }],
                    Click: function (e) {
                        mTxtTransportedBy.val(e.PeopleName);
                    },
                    Close: function () {
                        //$('#popupTotalNumberOfCustomer').popup('open');
                    },
                    AutoSearch: true,
                    Offline: true
                }
            ]);
        }); // END mElement.find('[name=butTransportedBy]').click(function() {

        function BrowseTransferDepartment(vParams) {
            vParams = $.extend({
                Title: null,
                FowstFieldName: null,
                Success: function(vData) {
                    
                }
            }, vParams);
            if (!HandleEditingSavedAsFinal())
                return;
            
            BrowsePopup('LibraryFowstBin', vParams.Title,
                [
                    {
                        AutoSearch: true,
                        StyleFormat: 'table',
                        DisplayText: [
                            {
                                Label: 'Department',
                                Column: 'Name'
                            }
                        ],
                        Click: function (e) {
                            vParams.Success(e);
                        }
                    }
                ], { 
                    FowstID: mOut.Model.Data[vParams.FowstFieldName]
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function HandleShowLandedCost() {
            if (mCanShowLandedCost)
                $('.receiving-landed-cost').show();
        }

        mOut.Initialize = function() {
            GetSecurityCurrentUserAcccess({
                ModuleName: 'Show cost details in stock receiving',
                Callback: function(vAllowed) {
                    if (vAllowed) {
                        mCanShowLandedCost = true;
                        HandleShowLandedCost();
                    }
                }
            });
        };

        mOut.Initialize();
        NewTrans();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
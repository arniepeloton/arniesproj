define([
    '/Scripts/sources/ERP/SalesReturn/work/model.js',
    'promise!/Scripts/sources/ERP/OrderProcessing/serialNumbers/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/SalesReturn/product/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/SalesReturn/work/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.SalesReturn.work.controller = ERP.SalesReturn.work.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.SalesReturn.work.controller', null, function (vParams) {

        var mOut = {
            RepopupAdjustment: false
        };
        mOut.Model = new ERP.SalesReturn.work.model();

        var mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = new ERP.SalesReturn.work.model();
        var mSerialNumbersPopup = new ERP.OrderProcessing.serialNumbers.controller();
        var mSavedAsFinal;
        var mDivDraftOrFinalized = mElement.find('[name=divDraftOrFinalized]');
        var mTxtInvoiceNumber = mElement.find('[name=txtInvoiceNumber]');
        var mTxtRefInvoiceNumber = mElement.find('[name=txtRefInvoiceNumber]');
        var mTxtPaymentTerms = mElement.find('[name=txtPaymentTerms]');
        var mTxtPaymentTermsText = mElement.find('[name=txtPaymentTermsText]');
        var mTxtCustomer = mElement.find('[name=txtCustomer]');
        var mCustomerName;
        var mTxtQuotationNumText = mElement.find('[name=txtQuotationNumText]');
        var mTxtQuoteNumber = mElement.find('[name=txtQuoteNumber]');
        var mTxtQuoteDate = mElement.find('[name=txtQuoteDate]');
        var mTxtSalesOrderNumber = mElement.find('[name=txtSalesOrderNumber]');
        var mMaxPaymentTerm = 0;
        var mTxtSalesOrderDate = mElement.find('[name=txtSalesOrderDate]');
        var mTxtPoNumber = mElement.find('[name=txtPoNumber]');
        var mTxtPoDate = mElement.find('[name=txtPoDate]');
        var mTxtSalesPartner = mElement.find('[name=txtSalesPartner]');
        var mTxtSalesRepText = mElement.find('[name=txtSalesRepText]');
        var mTxtSalesRep = mElement.find('[name=txtSalesRep]');
        var mTxtDateNeeded = mElement.find('[name=txtDateNeeded]');
        var mTxtRemarks = mElement.find('[name=txtRemarks]');
        var mTxtCanvass = mElement.find('[name=txtCanvass]');
        var mTxtBillingAddress = mElement.find('[name=txtBillingAddress]');
        var mTxtDeliveryAddress = mElement.find('[name=txtDeliveryAddress]');
        var mCbInvoiceType = mElement.find('[name=cbInvoiceType]');
        var mTxtCustomerFowst = mElement.find('[name=txtCustomerFowst]');
        var mTxtCustomerBin = mElement.find('[name=txtCustomerBin]');
        var pProductTable = mElement.find('[name=tableProducts]');
        var pProductRow = pProductTable.find('tbody tr');
        pProductRow.remove();
        var pProductPopup = new ERP.SalesReturn.product.controller();
        var mInvoiceID;

        $('#popupSalesReturn .modal-body').empty().append(mElement)//.trigger('create');
        mElement.show();

        mElement.find('[name=butInvoiceNumber]').click(function() {
            BrowsePopup('TransInvoicesReturnables', 'Invoices',
                [
                    {
                        StyleFormat: 'table',
                        AutoSearch: false,
                        DisplayText: [
                            {
                                Label: 'Customer',
                                Column: 'CompanyName'
                            },
                            {
                                Label: 'Invoice No.',
                                Column: 'InvoiceNum'
                            },
                            {
                                Label: 'Invoice Date',
                                Column: function(vDR) {
                                    return FormatDate(vDR.InvoiceDate, {Format: 'M/d/yyyy m:hh a'});
                                }
                            },
                            {
                                Label: 'Invoice Amount',
                                Column: function(vDR) {
                                    return AccountingFormat(vDR.Total);
                                }
                            },
                            {
                                Label: 'SKU Count',
                                Column: function(vDR) {
                                    return AccountingFormat(vDR.SkuCount, 0);
                                }
                            },
                            {
                                Label: 'Returnables',
                                Column: function(vDR) {
                                    return AccountingFormat(vDR.Returnables, 0);
                                }
                            }
                        ],
                        Click: function(vData) {
                            mTxtInvoiceNumber.text(vData.InvoiceNum);
                            mOut.LoadTrans({
                                ID: vData.ID
                            });
                        }
                    }
                ]
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        mOut.LoadTrans = function (vParams) {
            vParams = $.extend({
                ID: null,
                LoadProductsOnly: false,
                DontReplaceCurrentID: false
            }, vParams);
            mInvoiceID = vParams.ID;
            mOut.Model.GetInvoice({
                InvoiceID: mInvoiceID,
                Success: function(vData) {
                    if (!vParams.LoadProductsOnly)
                        DisplayTrans(vData, {
                            DontReplaceCurrentID: vParams.DontReplaceCurrentID
                        });
                    LoadProducts(vParams.ID);
                }
            });
        };

        function LoadProducts(vInvoiceID) {
            mOut.Model.GetContents({
                InvoiceID: vInvoiceID,
                Success: function(vData) {
                    DisplayProducts(vData);
                }
            });
        }

        function DisplayTrans(vData, vParams) {
            var pDate;
            var pStr;

            vParams = $.extend({
                DontReplaceCurrentID: false
            }, vParams);
            mSavedAsFinal = CBool(vData.Master.SavedAsFinal);
            // mDivDraftOrFinalized.text(mSavedAsFinal ? 'FINALIZED' : 'DRAFT');
            if (!vParams.DontReplaceCurrentID) {
                mOut.Model.Data.ID = vData.Master.ID;
                pProductPopup.Model.Data.OrderID = mOut.Model.Data.ID;
                pProductPopup.Model.Data.NewCartMode = false;
            }
            mOut.Model.Data.InvoiceID = vData.Master.ID;
            mOut.Model.Data.CustomerID = vData.Master.CustomerID;
            mCustomerName = vData.Master.CustomerName;
            mTxtCustomer.val(mCustomerName);
            mOut.Model.Data.QuotationID = vData.Master.QuotationID;
            mTxtQuotationNumText.val(vData.Master.QuotationNumText);
            if (mOut.TransType == 'Quote') {
                mTxtQuoteNumber.val(vData.Master.QuotationNo);
                pDate = vData.Master.QuotationDate;
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
            } else {
                pStr = CStr2(vData.Master.MIN).trim();
                if (pStr != '')
                    pStr += '-';
                pStr += CInt2(vData.Master.SalesOrderNo);
                mTxtSalesOrderNumber.val(pStr);
                mTxtSalesOrderDate.val(vData.Master.SalesOrderDate);
            }
            pStr = CStr2(vData.Master.MIN).trim();
            if (pStr != '')
                pStr += '-';
            pStr += vData.Master.InvoiceNumber;
            mTxtInvoiceNumber.val(pStr);
            mMaxPaymentTerm = CDec2(vData.Master.CreditTerms);
            mTxtPaymentTerms.val(vData.Master.PaymentTerm);
            mTxtPaymentTermsText.val(vData.Master.PaymentTermsText);
            mOut.Model.Data.SalesPartnerOrgID = vData.Master.SalesPartnerOrgID;
            mTxtSalesPartner.val(vData.Master.SalesPartner);
            mOut.Model.Data.SalesRepPeopleID = vData.Master.SalesRepPeopleID;
            mTxtSalesRep.val(vData.Master.SalesRep);
            mTxtSalesRepText.val(vData.Master.SalesRepText);
            mTxtDateNeeded.val(FormatDate(vData.Master.DateNeeded, {DontDisplay12Am: true}));
            if (mOut.TransType == 'PurchaseOrder') {
                pStr = vData.Master.MIN;
                if (CStr2(pStr).trim() != '')
                    pStr += '-';
                pStr += CInt2(vData.Master.PurchaseOrderNo);
                mTxtPoNumber.val(pStr);
            } else
                mTxtPoNumber.val(vData.Master.PurchaseOrderText);
            pDate = ParseDate(vData.Master.PurchaseOrderDate);
            if (pDate != null)
                pDate = $.format.date(pDate, 'M/d/yyyy');
            mTxtPoDate.val(pDate);
            mTxtRemarks.val(vData.Master.Remarks);
            mTxtRefInvoiceNumber.val(vData.Master.RefInvoiceNum);
            mTxtCanvass.val(vData.Master.CanvassNumber); 

            mOut.Model.Data.CanvassID = vData.Master.CanvassID;
            mOut.Model.Data.BillingAddressID = vData.Master.BillingAddressID;
            mTxtBillingAddress.val(vData.Master.BillingAddress);
            mOut.Model.Data.DeliveryAddressID = vData.Master.DeliveryAddressID;
            mTxtDeliveryAddress.val(vData.Master.DeliveryAddress);
            mCbInvoiceType.find('option[value=' + vData.Master.DocumentNameID + ']').attr('selected', true);

            mOut.Model.Data.CustomerFowstID = vData.Master.CustomerFowstID;
            mTxtCustomerFowst.val(vData.Master.CustomerFowst);
            mOut.Model.Data.CustomerBinID = vData.Master.CustomerBinID;
            mTxtCustomerBin.val(vData.Master.CustomerBin);
            //LoadAccountsDetails();
        } // END DisplayTrans

        function DisplayProducts(vData) {
            var pBaseUnit;
            pProductTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Units.Rows.length - 1; p++) {
                var pDR = vData.Units.Rows[p];
                var pDrProduct = FilterByProductID(pDR.ProductID, vData);
                var pTR = pProductRow.clone();
                var pUnit;
                pTR.data('data', {
                    ProductID: pDR.ProductID,
                    ProductName: pDrProduct.ProductName,
                    ProductNo: pDrProduct.ProductNo,
                    PriceLevelID: pDR.PriceLevelID,
                    UnitMeasure: pDR.UnitMeasure
                });
                pTR.find('[name=ProductNumber]').text(pDrProduct.ProductNo);
                pTR.find('[name=ProductDescription]').text(pDrProduct.ProductName);
                pTR.find('[name=OrderQty]').text($.format.number(CDec2(pDR.OrderedQuantity), '#,###.##'));
                pTR.find('[name=ServedQty]').text($.format.number(CDec2(pDR.Quantity), '#,###.##'));
                pTR.find('[name=QtyReturnedOthers]').text($.format.number(CDec2(pDR.QtyReturnedOthers), '#,###.##'));
                pTR.find('[name=QtyReturnedOwn]').text($.format.number(CDec2(pDR.QtyReturnedOwn), '#,###.##'));
                pTR.find('[name=QtyReturnedOwnBad]').text($.format.number(CDec2(pDR.QtyReturnedOwnBad), '#,###.##'));
                pUnit = pDR.UnitMeasure;
                //if (CDec2(pDR.ActualQuantity) != 1)
                //    pUnit += ' (' + $.format.number(CDec2(pDR.ActualQuantity), '#,###.##') + ' ' + pBaseUnit + ')';
                pTR.find('[name=PackingSize]').text(pUnit);
                pTR.find('[name=UnitPrice]').text(AccountingFormat(pDR.UnitPrice));
                pDR.SubTotal = (CDec2(pDR.QtyReturnedOwn) + CDec2(pDR.QtyReturnedOwnBad)) * CDec2(pDR.UnitPrice);
                pTR.find('[name=LineTotal]').text(AccountingFormat(pDR.SubTotal));
                //(pTR.click(function(vProductID) {
                //    Item_Click(vProductID);
                //}))(pDR.ProductID);
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    Item_Click(pDR.ProductID, pDR);
                });
                pProductTable.append(pTR);
            }

            //pProductTable.find('tfoot tr td[name=LineTotal]').text(AccountingFormat(vData.Master.Subtotal));
        } // END DisplayProducts

        function FilterByProductID(vProductID, vData) {
            for (var p = 0; p <= vData.Stocks.Rows.length - 1; p++) {
                var pDR = vData.Stocks.Rows[p];
                if (pDR.ProductID == vProductID)
                    return pDR;
            }
        } // END FilterByProductID

        function Item_Click(vProductID, vDR) {
            if (!HandleEditingSavedAsFinal())
                return;
            vDR = $.extend({}, vDR);
            PopupButtonList('', vDR.ProductName, [
                {
                    Text: 'Edit This Line Item',
                    Click: function() {
                        pProductPopup.AfterSave = AfterProductSave;
                        pProductPopup.popup({
                            ProductID: vProductID,
                            ProductName: vDR.ProductName,
                            ProductNo: vDR.ProductNo,
                            CustomerID: mOut.Model.Data.CustomerID,
                            InvoiceID: mOut.Model.Data.InvoiceID,
                            Success: function() {
                            }
                        });
                    }
                },
                {
                    Text: 'Enter Serial Numbers',
                    Click: function() {
                        mSerialNumbersPopup.popup({
                            InvoiceID: mOut.Model.Data.ID,
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
                }
            ]);
            
        }

        function HandleEditingSavedAsFinal() {
            return true; // always editable
            if (mSavedAsFinal) {
                NewMessageBox('This transaction was saved as final. This transaction may not be edited.', 'Saved As Final');
                return false;
            }

            return true;
        }

        function PreSave(vParams) {
            vParams = $.extend({
                SaveAndFinalize: false,
                SaveACopy: false,
                NoPrompt: false,
                Success: function(){
                    NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);

            mOut.Model.Data.FowstID = sessionStorage.FowstID;
            mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
            mOut.Model.Data.Remarks = mTxtRemarks.val();
            
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
                        mOut.Model.VerifyOnline({
                            MemoID: null,
                            success: function(vData) {
                                if (vParams.SaveAndFinalize)
                                    pMessage = vData.Message + '<br />' + pMessage;
                                NewMessageBox(pMessage, pTitle, [
                                    {
                                        Text: 'Yes',
                                        Click: function() {
                                            Save({
                                                SaveAndFinalize: vParams.SaveAndFinalize,
                                                Success: function () {
                                                    vParams.Success();
                                                    // $('#popupNewAdjustment').modal('show'); // commented 2023_03_30__13_00_04_0539
                                                }
                                            });

                                        }
                                    },
                                    {
                                        Text: 'No', Click: function () {
                                            $('#popupSalesReturn').modal('show');
                                        }
                                    }
                                ]);
                            } // END success: function(vData) {
                        }); // END mOut.Model.VerifyOnline({
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            } else {
                Save({
                    Success: vParams.Success
                });
            }
        } // END function PreSave(vParams) {

        function Save(vParams) {
            vParams = $.extend({
                Success: function(){
                    NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);
            mOut.Model.CheckOut({
                data: {
                    FromEas: 1,
                    InvoiceID: mInvoiceID
                },
                Success: function(vData) {
                    vParams.Success(vData);
                    mSavedAsFinal = vParams.SaveAndFinalize;
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }

        mElement.find('[name=butSaveAndFinalize]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            PreSave({
                SaveAndFinalize: true,
                Success: function() {
                    $('#popupSalesReturn').modal('hide');
                    Cac.LoadAdjustments();
                }
            });
        });

        mElement.find('[name=butSaveAsDraft]').click(function() {
            if (!HandleEditingSavedAsFinal())
                return;
            PreSave();
        });

        mElement.find('[name=butOutput]').click(function () {
            PopupButtonList('Select Print Options', 'Options', [
                {
                    Text: 'Make A Merchandise Exchange',
                    Click: function () {
                        AjaxSend({
                            url: 'SalesReturns/MakeMerchandiseExchange',
                            data: {
                                Template: 'MerchandiseExchange',
                                TransType: 'MerchandiseExchange',
                                UUID: '162220421536',
                                InvoiceID: mInvoiceID
                            }
                        })
                    }
                }, {
                    Text: 'Issue a Refund',
                    Click: function () {

                    }
                }
            ])
        });

        function AfterProductSave() {
            mOut.LoadTrans({
                ID: mInvoiceID,
                LoadProductsOnly: true
            });
        }

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

        $('#popupSalesReturn').find('[name=btnCloseSalesReturn]').click(function (e) {
            // commented 2023_03_30__13_00_04_0539
            //if (mOut.RepopupAdjustment)
            //    $('#popupNewAdjustment').modal('show');
        });

        function AddProduct() {
            if (!HandleEditingSavedAsFinal())
                return;
            var pColumns = [
                { 
                    Column: 'ProductNo', 
                    Label: 'Product No.' 
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name' 
                }
            ];
            var pAdditionalData = {
                IsAll:1,
                UseCacheProductSearch: 0,
                InventoryFowstID: sessionStorage.FowstID // disable temporarily
            };
            pAdditionalData.FilterCustomers = 1;
            pColumns.push({
                Label: 'Retail Price' ,
                Column: function(vData) {
                    return AccountingFormat(vData.RetailPrice);
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
                        AutoSearch: false,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            if (!e.FromCacheProductSearch)
                                CacheProductSearch();
                            if ($.isArray(e)) {
                                //PreSelect(e);
                            } else {
                                Item_Click(e.ID, e);
                            }
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
        }

        mOut.Initialize = function() {

        };

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
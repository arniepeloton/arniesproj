define([
    'promise!/Scripts/sources/ERP/ProductMaker/work/model.js',
    'promise!/Scripts/sources/ERP/ProductMaker/product/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ProductMaker/work/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ProductMaker.work.controller', null, function () {

        var mOut = {};
        var mElement;
        var mSavedAsFinal;
        var mDivDraftOrFinalized;
        
        mOut.Model = new ERP.ProductMaker.work.model();

        mElement = $(mHtml);
        mOut.element = mElement;
        var mTxtAssignedTo = mElement.find('[name=txtAssignedTo]');
        var mTxtDateNeeded = mElement.find('[name=txtDateNeeded]');
        var mTxtRemarks = mElement.find('[name=txtRemarks]');
        var mDivDraftOrFinalized = mElement.find('[name=divDraftOrFinalized]');
        var pProductTable = mElement.find('[name=tableProducts]');
        var pProductRow = pProductTable.find('tbody tr');
        var pProductPopup = ERP.ProductMaker.product.controller;

        $('#tabProductMakerWork').append(mElement).trigger('create');

        $('#tabProductMakerWork').opentabmenu();

        mTxtAssignedTo.on('keypress', function(e) {
            if (e.key == 'Enter')
                BrowseAssignTo();
        }); // END mTxtAssignedTo.on('keypress', function(e) {

        mElement.find('[name=butAssignedTo]').click(BrowseAssignTo);

        mElement.find('[name=butOutput]').click(function() {
            var pTransID = GetStringObject('Model.Data.ID', mOut);
            if (pTransID == null) {
                NewMessageBox('Please select a transaction first.');
                return;
            }
            AjaxSend({
                url: '/ProductMaker/CreatePdf',
                data: {
                    TransID: pTransID,
                    PrintHash: localStorage.getItem('PrintHash')
                },
                success: function(vData) {
                    if (vData.Error) {
                        if (vData.Error.indexOf('PrintHash') == -1)
                            NewMessageBox(vData.Error);
                        else
                            NoPrintHash();
                    } else if (vData.Result == 'ok')
                        NewMessageBox('Your document will be printed by your local print server.');
                }
            });
        });

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

        function BrowseAssignTo() {
            if (!HandleEditingSavedAsFinal())
                return;
            BrowsePopup('LibraryOrganizationEmployee', 'Assign To',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        StyleFormat: 'table',
                        DisplayText: [
                            {
                                Label: 'Employee',
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            mOut.Model.Data.AssignedToID = e.ID;
                            mTxtAssignedTo.val(e.PeopleName);
                        }
                    }
                ], { 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mTxtDateNeeded.click(BrowseDateNeeded);
        function BrowseDateNeeded() {
            if (!HandleEditingSavedAsFinal())
                return;
            SingleCalendar({
                ShowTime: true,
                Click: function(e){
                    mTxtDateNeeded.val(e);
                }
            });
        }
        mElement.find('[name=butDateNeeded]').click(BrowseDateNeeded);

        function HandleEditingSavedAsFinal(vParams) {
            vParams = $.extend({
                AllowFinalizedForApproval: false,
                AllowSomeSaving: false,
                AllowSomeSavingCallback: function() {}
            }, vParams);
            var pCondition = mSavedAsFinal;
            if (pCondition) {
                if (vParams.AllowSomeSaving) {
                    PromptUserAccess('Modify Finalized Invoice', {
                        OnClickOKSuccess: function () {
                            NewMessageBox('This transaction was saved as final.', 'Saved As Final', [
                                {
                                    Text: 'OK'
                                }
                            ]);
                        },
                        OnFailed: function () {
                            console.log('jsCFDTPD3GXC!');
                        }
                    });
                    
                } else {
                    NewMessageBox('This transaction was saved as final. This transaction may not be edited.', 'Saved As Final');
                }
                return false;
            }

            return true;
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

        function AddProduct(vParams) {
            vParams = $.extend({
                FilterSupplierID: null
            }, vParams);
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
                UseCacheProductSearch: 1,
                WithContentsOnly: 1
            };

            
            pAdditionalData.FilterCustomers = 1;
            pColumns.push({
                Label: 'Retail Price' ,
                Column: function(vData) {
                    return AccountingFormat(vData.RetailPrice);
                }
            });
            
            //pColumns = pColumns.concat([
            //    {
            //        Label: 'Qty',
            //        Render: function(v1, v2, vData) {
            //            return '<div name="divQty" />';
            //            //return '<input class="qty" style="width:70px" />';
            //        }
            //    }
            //]);
            pColumns.push({
                Label: 'Packing Size (in smallest unit)',
                Render: function(v1, v2, vData) {
                    return 'Change';
                }
            });
            //pColumns.push({
            //    Label: 'Qty Available to Commit',
            //    Render: function(v1, v2, vData) {
            //        var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
            //        p = '<div style="text-align: center">' + p + '</div>';
            //        return p;
            //    }
            //});
            //pColumns.push(
            //    {
            //        Label: 'Action',
            //        Render: function(v1, v2, vData) {
            //            return '<div name="divAction"></div>';
            //        }
            //    }
            //);
            
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
                        AutoSearch: false,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            if (!e.FromCacheProductSearch)
                                CacheProductSearch();
                            if ($.isArray(e)) {
                                PreSelect(e);
                            } else {
                                
                                SelectProduct(e);
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
                            var pDisabled = '';
                            // commented on 2021_09_02__09_42_32_5538
                            //if (CDec2(data.RetailPrice) == 0)
                            //    pDisabled = ' disabled ';
                            pDiv.append('<input class="qty" ' + pDisabled + ' style="width:70px" />');
                        }
                    }
                ], pAdditionalData
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function AddProduct(vParams) {

        function SaveAsDraft(vParams) {
            if (!HandleEditingSavedAsFinal())
                return;
            mOut.Model.Data.CopyProductsFromInvoiceID = null;
            PreSave(vParams);
        };

        function PreSave(vParams) {
            vParams = $.extend({
                UseUnitPriceFromLibrary: false,
                SaveAndFinalize: false,
                SaveACopy: false,
                NoPrompt: false,
                Success: function(){
                    NewMessageBox('Transaction was successfully saved.');
                }
            }, vParams);

            //if (CStr2(mTxtDateNeeded.val()).trim() == '') {
            //    NewMessageBox('Mobiz cannot save your current transaction because Date Needed is required.');
            //    return;
            //}

            mOut.Model.Data.FowstID = sessionStorage.FowstID;
            mOut.Model.Data.SaveAndFinalize = vParams.SaveAndFinalize;
            mOut.Model.Data.DateNeeded = mTxtDateNeeded.val();
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
                    var pForApproval = '';
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
                Save({
                    Success: vParams.Success
                });
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
            mOut.Model.Save({
                Data: {
                },
                Success: function(vData) {
                    LoadTrans({
                        ID: vData.ID,
                        AfterLoad: function() {
                            vParams.Success(vData);
                            if (!CBool(vData.FinalizedForApproval))
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
                Success: function(vData) {
                    if (!vParams.LoadProductsOnly)
                        DisplayTrans(vData, {
                            DontReplaceCurrentID: vParams.DontReplaceCurrentID
                        });
                    DisplayProducts(vData);
                    if (vParams.AfterLoad)
                        vParams.AfterLoad();
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
            if (CBool(vData.Master.IsCancelled))
                mDivDraftOrFinalized.text('CANCELLED');
            else
                mDivDraftOrFinalized.text(mSavedAsFinal ? 'FINALIZED' : 'DRAFT');
            if (!vParams.DontReplaceCurrentID) {
                mOut.Model.Data.ID = vData.Master.ID;
                //pProductPopup.Model.Data.OrderID = mOut.Model.Data.ID;
                //pProductPopup.Model.Data.NewCartMode = false;
            }
            mOut.Model.Data.AssignedToID = vData.Master.AssignedToID;
            mTxtAssignedTo.val(vData.Master.AssignedToName);
            pStr = vData.Master.FowstDeviceNum + '-' + CInt2(vData.Master.Num);
            mElement.find('[name=divTransNum]').text(pStr);
            
            mTxtDateNeeded.val(FormatDate(vData.Master.DateNeeded, {DontDisplay12Am: true}));
            mTxtRemarks.val(vData.Master.Remarks);
        } // END DisplayTrans

        function DisplayProducts(vData) {
            var pTotalServedQty = 0;
            pProductTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Products.Rows.length - 1; p++) {
                var pDR = vData.Products.Rows[p];
                var pTR = pProductRow.clone();
                pTR.data(pDR);
                pTR.find('[name=ProductNumber]').text(pDR.ProductNo);
                pTR.find('[name=ProductDescription]').text(pDR.ProductName);
                pTR.find('[name=Qty]').text($.format.number(CDec2(pDR.Quantity), '#,###.####'));
                pUnit = pDR.UnitMeasure;
                //if (CDec2(pDR.ActualQuantity) != 1)
                //    pUnit += ' (' + $.format.number(CDec2(pDR.ActualQuantity), '#,###.##') + ' ' + pDR.BaseUnit + ')';
                pTR.find('[name=PackingSize]').text(pUnit);
                //(pTR.click(function(vProductID) {
                //    Item_Click(vProductID);
                //}))(pDR.ProductID);
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    Item_Click(pDR.ProductID, pDR);
                });

                pProductTable.append(pTR);
            }
        } // END DisplayProducts

        function SelectProduct(e) {
            ERP.ProductMaker.product.controller.popup({
                TransID: mOut.Model.Data.ID,
                ProductID: e.ID,
                ProductName: e.ProductName,
                ProductNo: e.ProductNo,
                CustomerID: mOut.Model.Data.CustomerID,
                TransType: mOut.TransType,
                OnSave: function(vProducts) {
                    NewMessageBox('Products were successfully saved.');
                    LoadTrans({
                        ID: mOut.Model.Data.ID,
                        LoadProductsOnly: true
                    });
                },
                Success: function() {
                    
                }
            });
        }

        mElement.find('[name=butLookup]').click(function() {
            
            Find({});
            return;
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
        });

        function Find(vParams) {
            var pColumns = [];
            var pIsReceiving;

            pColumns = [
                {
                    Label: 'Trans No.',
                    Column: function(vDR) {
                        //var pOut = '';
                        //if (CStr2(vDR.MIN).trim() != '')
                        //    pOut = vDR.MIN + '-';
                        return 'BOM-' + CStr2(vDR.FowstDeviceNum) + '-' + CStr2(vDR.Num);
                    }
                },
                {
                    Label: 'Assigned To',
                    Column: 'AssignedToName'
                }
            ];

            pColumns = pColumns.concat([
                {
                    Label: 'Last Updated',
                    Column: 'LastUpdated'
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

            BrowsePopup('TransBomMaker', 'Product Maker',
                [
                    {
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
            PreSave({
                SaveAndFinalize: true
            });
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
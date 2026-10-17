require(['common', 'business', 'promise!/Scripts3/Browser/TransInvoices.js?crc=', 'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
   'promise!/Scripts3/Browser/LibraryFowstBin.js?crc=',
   'promise!/Scripts3/Browser/LibraryOrganizationFOWSTSection.js?crc=',
   GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
    ], function () {
        //        require([(OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : '')], function () {

        $(function () {
            //                if (OfflineMode) {
            //                    Browser.OfflineMode = true;
            //                }
            Browser.OfflineMode = false;


            startclock('#clock');
            getFOWST();

            var newPrinterIP;
            var salesOrderNo = '';
            var deliveryDate = '';
            var lastUpdated = '';
            var lastUpdatedBy = '';
            var soDate = '';
            var clientName = '';
            var deliveryAddress = '';
            var mLvCart;
            var CounterOrderCentral = true;
            var isPrint = false;
            //            var isApplyFilter = 0;
            var isHistory = false;

            proj.PLMode = true;
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.location = '/People/PeopleMenu';

            });


            $('#btnFowst').click(function (e) {
                e.preventDefault();
                BrowseAddresses();
                return false;
            });


            $('#btnAddBasket').click(function (e) {
                e.preventDefault();

                var pData = {};
                pData.CartOrderID = sessionStorage.CartOrderID;
                pData.BasketName = $('#txtBasketName').val();

                var result = PickProducts_offline().SaveBasketInfo(pData);
                if (result == 'ok')
                    LoadBasketList(PickProducts_offline().GetBasketInfo(sessionStorage.CartOrderID));
            });

            $('#btnRefresh').click(function (e) {
                ShowPendingOrders();
                //                //                MessageBox('Needs internet connection');

                //                // import invoice data from server. sales orders
                //                if (OfflineMode) {
                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            GetPendingOrders();
                //                        },
                //                        error: function (vError) {

                //                            Android.Toast("Can't establish a connection to server. Please make sure you have internet connection.");
                //                        }
                //                    });
                //                }

            });

            $('#btnViewHistory').click(function (e) {

                e.preventDefault();

                isHistory = true;

                ShowTransInvoiceBrowser(true);
            });


            $('#btnStart').click(function (e) {

                e.preventDefault();
                isHistory = false;
                ShowTransInvoiceBrowser(false);

            });

            $('#lnkPageBinBack').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesOrder');
                LoadCart();
            });

            $('#lnkSalesOrderBack').click(function (e) {
                e.preventDefault();


                ShowTransInvoiceBrowser(isHistory);


            });


            $('#butBinName').click(function (e) {
                e.preventDefault();

                if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                    mobile.Scan({
                        Success: function (vData) {
                            $('#txtBinName').val(vData.Value);
                            var pBrowser;

                            pBrowser = Browser.LibraryFowstBin;
                            pBrowser.InputData.FowstID = sessionStorage.FowstID;

                            pBrowser.Click = function (e) {
                                //                                                if (OfflineMode) {
                                //                                                    var result = PickProducts_offline().saveFowstBinProduct(e.ID, proj.ProductID);
                                //                                                    if (result = 'ok')
                                //                                                        LoadBin(PickProducts_offline().getProductBinLocation(proj.ProductID));

                                //                                                } else {
                                AjaxSend({
                                    url: 'PickProducts/saveFowstBinProduct',
                                    data: { BinID: e.ID,
                                        ProductID: proj.ProductID,
                                        FowstID: sessionStorage.FowstID,
                                        InvoiceID: sessionStorage.CartOrderID,
                                        Quantity: 0
                                    },
                                    success: function (vData) {
                                        if (vData.Result == 'ok')
                                            getProductBinLocation();
                                    },
                                    error: function (vError) {
                                        var pDummy;
                                    }
                                }); // AjaxSend
                                //                                                }

                            };

                            pBrowser.Show($('#txtBinName').val());
                        }
                    });
                } else {
                    MessageBox('Scan Bin QR Code feature is only available on the mobile app',
                                    'Scan Bin QR Code',
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                }
            });



            function ShowTransInvoiceBrowser(isReadyForInvoice) {
                var pBrowser;

                pBrowser = Browser.TransInvoices;
                pBrowser.InputData.FowstID = sessionStorage.FowstID;
                //                pBrowser.InputData.CustomerID = sessionStorage.CustomerOrganizationID;
                pBrowser.InputData.NonZero = 0;
                pBrowser.InputData.SalesOrder = 1;
                //                pBrowser.InputData.ApplyFilter = isApplyFilter;
                pBrowser.InputData.NoMaxRecord = 0;
                if (isReadyForInvoice)
                    pBrowser.InputData.IsReadyForInvoice = 1;
                else
                    pBrowser.InputData.IsReadyForInvoice = 0;
                pBrowser.InputData.ForPickingList = 1;
                pBrowser.InputData.SectionID = proj.SectionID;

                pBrowser.Click = function (e) {

                    $.mobile.changePage('#pageSalesOrder');

                    //                    if (location.hash == '#&ui-state=dialog')
                    //                        history.back();
                    sessionStorage.CustomerOrganizationID = e.CustomerID;
                    sessionStorage.CartOrderID = e.ID;
                    //                                sessionStorage.FowstID = e.FowstID;
                    salesOrderNo = '';
                    salesOrderNo = e.MIN + '-' + (e.SalesOrderNo || '');


                    lastUpdatedBy = '';
                    lastUpdatedBy = e.EmployeeFullName;

                    clientName = '';
                    clientName = e.CompanyName;

                    deliveryAddress = '';
                    deliveryAddress = e.DeliveryAddress;


                    sessionStorage.SalesRepID = '';
                    sessionStorage.SalesRepID = e.CurrentUserID;

                    sessionStorage.SalesOrderDate = '';
                    sessionStorage.SalesOrderDate = e.InvoiceDate;

                    orderStatus = isReadyForInvoice;

                    if (CounterOrderCentral == true) {
                        AjaxSend({
                            ExactUrl: '/PickProducts/SalesOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                $('#divSalesOrderProcessing').empty();

                                $('#divSalesOrderProcessing').append(vData).trigger('create');

                                App.Deferred.fulfill('FQ26E5TP76');


                                //                                    $.mobile.changePage('#pageScanBasket');
                                //                                    LoadBasketList(PickProducts_offline().GetBasketInfo(sessionStorage.CartOrderID));

                                //                                    ShowBasketList();
                                $('#btnPrint').click(function (e) {
                                    e.preventDefault();
                                    PrintPickingList(sessionStorage.CartOrderID);
                                    //                                    MessageBox('This sales order will be tagged as ready for invoicing.' +
                                    //                                        ' Once tagged, you are not allowed to make any changes ' +
                                    //                                        'to the order. Click "Ok" to confirm.', 'Ready for Invoicing',
                                    //                                        [
                                    //                                            {
                                    //                                                Text: 'Ok',
                                    //                                                Click: function () {
                                    //                                                    isPrint = true;

                                    //                                                    SaveAsReadyForInvoicing();



                                    //                                                }
                                    //                                            },
                                    //                                            {
                                    //                                                Text: 'Cancel',
                                    //                                                Click: function () {
                                    //                                                    isPrint = false;
                                    //                                                    history.back();
                                    //                                                    return;
                                    //                                                }
                                    //                                            }
                                    //                                        ]
                                    //                                    );


                                });
                                $('#btnOrderFulfillmentDone').click(function (e) {
                                    e.preventDefault();

                                    MessageBox('This sales order will be tagged as ready for invoicing.' +
                                        ' Once tagged, you are not allowed to make any changes ' +
                                        'to the order. Click "Ok" to confirm.', 'Ready for Invoicing',
                                        [
                                            {
                                                Text: 'Ok',
                                                Click: function () {
                                                    isPrint = false;
                                                    SaveAsReadyForInvoicing();



                                                }
                                            },
                                            {
                                                Text: 'Cancel',
                                                Click: function () {
                                                    isPrint = false;
                                                    history.back();
                                                    return;
                                                }
                                            }
                                        ]
                                    );




                                });
                                $('#btnHomeMain').click(function (e) {
                                    e.preventDefault();

                                    $.mobile.changePage('');
                                    ShowPendingOrders();
                                });

                                $('#butDone').click(function(e) {
                                    e.preventDefault();
                                    NewMessageBox('[Not yet implemented] Are you done? Are you sure?', 'Done?', [
                                        {
                                            Text: 'Yes, done',
                                            Click: function() {
                                                Android.Toast('done');
                                            }
                                        },
                                        {
                                            Text: 'Not yet done.',
                                            Click: function() {
                                                Android.Toast('ok, not yet done.');
                                            }
                                        }
                                    ]); // END NewMessageBox('Are you done? Are you sure?', 'Done?', [
                                }); // END $('#butDone').click(function(e) {

                                LoadCart();

                            }
                        });

                        CounterOrderCentral = false;

                    } else {

                        LoadCart();
                    }


                };
                pBrowser.Show();
            }

            proj.RefreshCart = function () {

                //                if (CDec2(sessionStorage.QtyLeft) == 0) {
                //                    $.mobile.changePage('#pageSalesOrder');
                //                    LoadCart();
                //                } else {
                if (CStr2(sessionStorage.IsReplacement) == 'true')
                    MarkAsDone();

                $.mobile.changePage('#pageBin');
                getProductBinLocation();
                LoadCart();


                //                }

            }; // proj.RefreshCart

            function SaveAsReadyForInvoicing() {

                var pData = {};
                pData.InvoiceID = sessionStorage.CartOrderID;


                AjaxSend({
                    url: 'PickProducts/SaveAsReadyForInvoicing',
                    data: pData,
                    success: function (vData) {


                        if (vData.Result == 'ok') {
                            if (isPrint) {
                                PrintPickingList(sessionStorage.CartOrderID);

                            } else {
                                $.mobile.changePage('');
                                ShowPendingOrders();
                            }



                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend

            }

            function LoadBin(vData, isDisable) {
                var pLV = $('#lstBin');
                var pItem;
                sessionStorage.TotalPickedQty = 0;

                pLV.html('');
                if (CStr2(vData) != '') {

                    for (var i = 0; i < vData.Rows.length; i++) {
                        sessionStorage.TotalPickedQty = CDec2(sessionStorage.TotalPickedQty) + CDec2(vData.Rows[i].PreparedQuantity);

                        pItem = $('<li>')
                            .append($('<a>')
                              .data('Data', vData)
                              .data('Index', i)
                                .append($('<h3>')
                                    .attr("style", "white-space:normal")
                                    .text(vData.Rows[i].Name)
                                )
                                .append($('<p>')
                                    .text('Qty Picked : ' + vData.Rows[i].PreparedQuantity)
                                )
                                .click(function (e) {
                                    e.preventDefault();

                                    if (!isDisable) {
                                        var data = $(this).data('Data');
                                        sessionStorage.BinID = data.Rows[parseInt($(this).data('Index'))].BinID;
                                        sessionStorage.BinName = data.Rows[parseInt($(this).data('Index'))].Name;
                                        sessionStorage.CurrentPickedQty = data.Rows[parseInt($(this).data('Index'))].PreparedQuantity;

                                        if (proj.ProductID != '') {

                                            window.NewStockBrowser = true;
                                            startclock('#clockStock');
                                            window.pageSourcePage = $.mobile.activePage;

                                            sessionStorage.IsReplacement = 'false';
                                            ChangePage('/PickProducts/SalesOrderStockViewer');

                                        } else
                                            return;


                                    }



                                })

                            ).append($('<a>')
                                .data('Data', vData)
                                .data('Index', i)
                                .attr('href', "#")
                                .attr('data-icon', "minus")
                                .click(function (e) {
                                    e.preventDefault();

                                    if (!isDisable) {

                                        var newData = $(this).data('Data');
                                        var i = $(this).data('Index');

                                        MessageBox("You may only remove the quantity picked for this bin. Continue?",
                                        'Remove',
                                        [
                                            { Text: 'Yes',
                                                Click: function (e) {
                                                    e.preventDefault();

                                                    AjaxSend({
                                                        url: 'PickProducts/removeFowstBinProduct',
                                                        data: { ProductID: proj.ProductID,
                                                            FowstID: sessionStorage.FowstID,
                                                            BinID: newData.Rows[parseInt(i)].BinID,
                                                            InvoiceID: sessionStorage.CartOrderID
                                                        },
                                                        success: function (vData) {
                                                            if (vData.Result == 'ok') {
                                                                history.back();

                                                                getProductBinLocation();
                                                            }
                                                        },
                                                        error: function (vError) {
                                                            var pDummy;
                                                        }
                                                    }); // AjaxSend

                                                } //Click
                                            }
                                            ,
                                            { Text: 'No'
                                            }
                                        ]
                                    );



                                    }

                                })
                           );

                        pLV.append(pItem);

                    }


                }


                pLV.listview('refresh');
                
            };

            function LoadBasketList(vRows) {
                var pLV = $('#lstBasket');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .data('Data', vRows[p])
                        .click(function (e) {
                            e.preventDefault();

                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].BasketName)
                        )
                    );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };



            //            function GetPendingOrders() {


            //                var pData = {};
            //                pData.FowstID;

            //                AjaxSend({
            //                    url: 'PickProducts/GetPendingSalesOrders',
            //                    data: pData,
            //                    success: function (vData) {

            //                        if (vData.Data != undefined) {
            //                            var vRows = vData.Data.Rows;

            //                            for (var i = 0; i < vRows.length; i++) {

            //                                var pData = {};
            //                                pData.CustomerID = vRows[i].CustomerID;
            //                                pData.OrderID = vRows[i].ID;
            //                                pData.FowstID = vRows[i].FowstID;
            //                                pData.PosMode = "so";

            //                                if (OfflineMode) {

            //                                    AjaxSend({
            //                                        url: 'SalesMenu/PosCart',
            //                                        data: pData,
            //                                        success: function (vDataPos) {

            //                                            // import invoice data from server
            //                                            PickProducts_offline().Import(vDataPos);

            //                                            if (i == vRows.length)
            //                                                ShowPendingOrders();
            //                                        },
            //                                        error: function (vError) {
            //                                            var pDummy;
            //                                        }
            //                                    }); // AjaxSend

            //                                }
            //                            }


            //                        } else {
            //                            ShowPendingOrders();
            //                        }

            //                    },
            //                    error: function (vError) {
            //                        var pDummy;
            //                    }
            //                }); // AjaxSend

            //            }


            function ShowPendingOrders() {
                var pData = {};

                //                pData.CustomerID = sessionStorage.CustomerOrganizationID;
                pData.NonZero = 0;
                pData.SalesOrder = 1;
                pData.FowstID = sessionStorage.FowstID;
                //                    pData.FowstID = sessionStorage.FowstID;

                pData.IsReadyForInvoice = 0;
                //                pData.ApplyFilter = isApplyFilter;
                pData.NoMaxRecord = 1;
                pData.ForPickingList = 1;
                pData.SectionID = proj.SectionID;
                //                if (OfflineMode) {

                //                    var vData = OfflineBrowser.TransInvoices.GetData(pData);
                //                    if (vData != undefined) {
                //                        if (vData.Rows.length > 0) {
                //                            $('#btnStart').show();

                //                            if (vData.Rows.length == 1)
                //                                $('#lblOrderTxt').text('order');
                //                            else
                //                                $('#lblOrderTxt').text('orders');
                //                        }


                //                        $('#lblPendingOrders').text(vData.Rows.length);
                //                    }
                //                } else {

                AjaxSend({
                    url: '/Browser/TransInvoices',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            $('#btnStart').show();

                            if (vData.Results.Rows.length == 1)
                                $('#lblOrderTxt').text('order');
                            else
                                $('#lblOrderTxt').text('orders');
                        }

                        $('#lblPendingOrders').text(vData.Results.Rows.length);
                    }
                });
                //                }
            }

            function getFOWST() {

                //                if (OfflineMode) {
                //                    BrowseAddresses();
                //                } else {
                var pData = { FilterFowst: 1 };
                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: pData,
                    success: function (vFowstData) {

                        if (vFowstData.Results.Rows.length == 1) {
                            sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                            $('#lnkPosBranch').html(HtmlEncode(vFowstData.Results.Rows[0].FowstName) + '&#x25BC;');
                            BrowseSections(vFowstData.Results.Rows[0]);
                        } else {
                            BrowseAddresses();
                        }



                    }
                });
                //                }; // if (OfflineMode) {
            }

            function BrowseAddresses() {

                Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                Browser.LibraryFowst.InputData.FilterFowst = 1;
                Browser.LibraryFowst.AfterClick = function (e) {
                    BrowseSections(e);
                };
                Browser.LibraryFowst.Show();
            };

            function BrowseSections(vFowstData) {
                Browser.LibraryOrganizationFOWSTSection.InputData.FowstID = vFowstData.ID;
                Browser.LibraryOrganizationFOWSTSection.AfterClick = function(e2) {
                    var pHtml;

                    proj.SectionID = e2.ID;
                    proj.SectionText = e2.Code + ' - ' + e2.Name;
                    pHtml = HtmlEncode(vFowstData.FowstName) + '</br>';
                    pHtml += HtmlEncode('Section: ' + proj.SectionText);
                    sessionStorage.FowstID = vFowstData.ID;
                    proj.FowstName = vFowstData.FowstName;
                    $('#lnkPosBranch').html(pHtml);
                    ShowPendingOrders();
                };
                Browser.LibraryOrganizationFOWSTSection.Show();
            }

            function LoadCart(vDontFetchIfBlank) {
                // PARAMS:
                //   vDontFetchIfBlank: boolean. if true, dont get the lastest sales order if CartOrderID is blank
                //additional by janine
                $('#divRecentlyBoughtPanel').hide();
                $('#divCartPanel').show();


                //                    isOnCartPage = true;

                var pUrl;
                var pData = {};

                //additional by janine
                $('#divCartPanel').show();

                $('#divCartSection').hide();
                $('#divCartEmpty').show();


                //additional by janine sept 5 2014 for sales order taker
                pData.PosMode = proj.PosMode ? "pos" : "so";
                //end of additional

                //                pData.CustomerID = sessionStorage.CustomerID;
                pData.OrderID = sessionStorage.CartOrderID;
                pData.FowstID = sessionStorage.FowstID;
                pData.PosMode = "so";
                if (vDontFetchIfBlank)
                    pData.DontFetchIfBlank = 1;
                pData.ForPickingList = 1;
                pData.SectionID = proj.SectionID;
                localStorage.removeItem('NewCartMode');

                //                if (OfflineMode) {
                //                    if (Browser.TransInvoices.OfflineMode == false) {
                //                        Browser.TransInvoices.OfflineMode = true;
                //                    } else {



                //                        var vData = PickProducts_offline().PosCart(pData);

                //                        deliveryDate = '';
                //                        deliveryDate = $.format.date(new Date(vData.DeliveryDate), 'MMM d, yyyy h:mm a');

                //                        $('#lblOrderNo').text(salesOrderNo);
                //                        $('#headerDeliveryDate').text(deliveryDate);
                //                        $('#headerSalesRep').text(salesrep);
                //                        $('#headerInvoiceDate').text(invoiceDate);
                //                        $('#headerClientName').text(clientName);


                //                        Display(vData);

                //                    }
                //                } else {

                //                        if (proj.PosMode) {
                pUrl = 'SalesMenu/PosCart';
                //                        } else {
                //                            pUrl = 'SalesMenu/Cart';
                //                        };

                AjaxSend({
                    url: pUrl,
                    data: pData,
                    success: function (vData) {
                        deliveryDate = '';

                        if (vData.Master == null) {
                            NewMessageBox('No pickable items for this sales order.');
                            return;
                        }

                        if (CStr2(vData.Master.DeliveryDate) != '')
                            deliveryDate = $.format.date(new Date(vData.Master.DeliveryDate), 'MMM d, yyyy h:mm a');

                        lastUpdated = '';
                        if (CStr2(vData.Master.LastUpdated) != '')
                            lastUpdated = $.format.date(new Date(vData.Master.LastUpdated), 'MMM d, yyyy h:mm a');

                        soDate = '';
                        if (CStr2(vData.Master.SalesOrderDate) != '')
                            soDate = $.format.date(new Date(vData.Master.SalesOrderDate), 'MMM d, yyyy h:mm a');

                        $('#spanPickProductsFowst').text(proj.FowstName);
                        $('#spanPickProductsSection').text(proj.SectionText);
                        $('#lblOrderNo').text(salesOrderNo);
                        $('#headerDeliveryDate').text(deliveryDate);
                        $('#headerLastUpdated').text(lastUpdated + ' by ' + lastUpdatedBy);
                        $('#headerSODate').text(soDate);
                        $('#headerClientName').text(clientName);
                        $('#headerClientDeliveryAddress').text(deliveryAddress);

                        if (orderStatus) {
                            $('#lblStatus').text('Ready for invoicing');
                            $('#btnOrderFulfillmentDone').hide();
                            $('#btnPrint').hide();
                        } else {
                            $('#lblStatus').text('Open');
                            $('#btnOrderFulfillmentDone').hide();
                            $('#btnPrint').show();
                        }
                        Display(vData, vData.PeopleID);
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend

                //                }; // if (OfflineMode) {

                function Display(vData, peopleID) {
                    var arrayProductIndex = [];
                    var arrayQty = [];

                    if (vData.Error != undefined) {
                        if (CDec2(vData.ReloadBrowser) != 0)
                            MessageBox(vData.Error, 'Error', null, {
                                OnClose: function () {
                                    location.reload();
                                }
                            });
                        else
                            MessageBox(vData.Error);
                        return;
                    };

                    var pDivTemplate = $('#divCartStock').hide();
                    var pTrTemplate = pDivTemplate.find('[name=trTemplate]').hide();
                    var pTrTemplatePrep = pDivTemplate.find('[name=trTemplatePrep]').hide();
                    var pTrTemplateRemarks = pDivTemplate.find('[name=trTemplateRemarks]').hide();


                    if ((vData.Stocks == undefined)
                                || (vData.Stocks.Rows.length == 0)) {

                        //additional by janine mar 6. if cart is empty, empty the session storage
                        sessionStorage.removeItem('InvoiceProductIDList');
                        //end of additional by janine

                        if (mLvCart != undefined)
                            mLvCart.Element.hide();
                        //additional by janine
                        $('#divCartPanel').show();

                        $('#divCartSection').hide();
                        $('#divCartEmpty').show();

                        $('#spanCartTotal').text('0.00'); //additional by janine nov 11

                        isCartEmpty = true;
                        return;
                    };
                    //ADDITIONAL BY JANINE FEB 18
                    isCartEmpty = false;

                    sessionStorage.CartOrderID = vData.Master.ID;
                    //additional by janine nov 7
                    if (window.location.pathname.toLowerCase() == '/salesmenu')
                        DisplaySalesOrder(vData.Master);


                    pDivTemplate.hide();

                    if (mLvCart == undefined) {
                        mLvCart = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                        mLvCart.Element.find('ul')
                .attr('data-split-icon', 'minus');
                        mLvCart.Element.insertAfter(pDivTemplate);
                        mLvCart.OnItem = function (vDataRow, vItem, e) {

                            var pTR;
                            var pTRPrep;
                            var pTRRemarks;
                            var pDiv;
                            var pDrUnit;
                            var pItemTotal = 0;

                            var pLnk = $('<a>')
                            .attr('href', '../Stock?id=' + vDataRow.ProductID)
                            .attr('StockItemID', vDataRow.ID)
                            .attr('ProductID', vDataRow.ProductID)
                            .attr('PeopleID', vDataRow.PeopleID)
                            .attr('BinCount', vDataRow.BinCount)
                            .attr('PickerCount', vDataRow.PickerCount)
                            .attr('ProductNo', vDataRow.ProductNo)
                            .attr('ProductName', vDataRow.ProductName)
                            .attr('RefStockID', vDataRow.RefStockID)
                            .data('data', vDataRow)
                            .click(function (e) {
                                e.preventDefault();
                                sessionStorage.ProductName = $(this).attr('ProductNo') + ' ' + $(this).attr('ProductName');
                                sessionStorage.RefStockID = CStr2($(this).attr('RefStockID'));

                                sessionStorage.StockItemID = CStr2($(this).attr('StockItemID'));

                                if (CDec2($(this).attr('PickerCount')) > 0 && $(this).attr('PeopleID') != peopleID && $(this).attr('PeopleID') != 'NoFilter' && CDec2($(this).attr('BinCount')) > 0)
                                    return;

                                if (orderStatus) {
                                    MessageBox("You are not allowed to modify this invoice. Please contact your manager.",
                                                "Ready for Invoicing",
                                                [
                                                    { Text: 'Ok',
                                                        Click: function () {
                                                            history.back();
                                                            $('.ui-btn-active').removeClass('ui-btn-active');

                                                        }
                                                    }
                                                ]
                                    );


                                } else {

                                    for (var i = 0; i < arrayProductIndex.length; i++) {

                                        if (arrayProductIndex[i] == $(this).attr('ProductID')) {

                                            sessionStorage.AllowableQty = mLvCart.OnItem.Units[i].OrderedQuantity;
                                            break;
                                        }
                                    }

                                    sessionStorage.FowstBinID = '';



                                    proj.ProductID = $(this).attr('ProductID');
                                    sessionStorage.RefProductID = $(this).attr('ProductID');
                                    $.mobile.changePage('#pageBin');
                                    getProductBinLocation();

                                    $('#txtBinName').keydown(function (e) {

                                        if (e.which == 13) {
                                            var pBrowser;

                                            pBrowser = Browser.LibraryFowstBin;
                                            pBrowser.InputData.FowstID = sessionStorage.FowstID;

                                            pBrowser.Click = function (e) {
                                                //                                                if (OfflineMode) {
                                                //                                                    var result = PickProducts_offline().saveFowstBinProduct(e.ID, proj.ProductID);
                                                //                                                    if (result = 'ok')
                                                //                                                        LoadBin(PickProducts_offline().getProductBinLocation(proj.ProductID));

                                                //                                                } else {
                                                AjaxSend({
                                                    url: 'PickProducts/saveFowstBinProduct',
                                                    data: { BinID: e.ID,
                                                        ProductID: proj.ProductID,
                                                        FowstID: sessionStorage.FowstID,
                                                        InvoiceID: sessionStorage.CartOrderID,
                                                        Quantity: 0
                                                    },
                                                    success: function (vData) {
                                                        if (vData.Result == 'ok')
                                                            getProductBinLocation();
                                                    },
                                                    error: function (vError) {
                                                        var pDummy;
                                                    }
                                                }); // AjaxSend
                                                //                                                }

                                            };

                                            pBrowser.Show($('#txtBinName').val());
                                        }
                                    });

                                }

                            });
                            pDiv = pDivTemplate.clone()
                        .attr('id', '');

                            pDiv.find('[name=spanStockDescription]').html('&nbsp;' + (CDec2(vDataRow.PickerCount) > 0 && vDataRow.PeopleID != peopleID && vDataRow.PeopleID != 'NoFilter' && CDec2(vDataRow.BinCount) > 0 ? '<font color="grey">' : '<font color="black">') + vDataRow.ProductNo + ' ' + vDataRow.ProductName + ' </font>');


                            for (var p = 0; p <= mLvCart.vData.Units.Rows.length - 1; p++) {
                                pDrUnit = mLvCart.vData.Units.Rows[p];
                                if (pDrUnit.ProductID == vDataRow.ProductID) {
                                    arrayProductIndex[p] = pDrUnit.ProductID;

                                    pTR = pTrTemplate.clone();
                                    pTR.find('[name=tdQty]').text('Qty Ordered : ' + $.format.number(CDec2(pDrUnit.OrderedQuantity), proj.QuantityFormat) + ' ' + pDrUnit.UnitMeasure);
                                    pTR.find('[name=tdUnit]').text('; Qty withdrawn: ' + $.format.number(CDec2(pDrUnit.PreparedQuantity), proj.QuantityFormat));
                                    pDiv.find('[name=trTemplate]').parent().append(pTR);
                                    pTR.show();

                                    //pTRPrep = pTrTemplatePrep.clone();
                                    //pTRPrep.find('[name=tdQtyPrep]').html('Qty Picked &nbsp;&nbsp;: ' + $.format.number(CDec2(pDrUnit.Quantity), proj.QuantityFormat));
                                    //pTRPrep.find('[name=tdUnitPrep]').text(pDrUnit.UnitMeasure);
                                    //pDiv.find('[name=trTemplatePrep]').parent().append(pTRPrep);
                                    //pTRPrep.show();

                                    //if (CDec2(vDataRow.BinCount) < 1) {
                                    //    pTRRemarks = pTrTemplateRemarks.clone();
                                    //    pTRRemarks.find('[name=tdRemarks]').html('<font color="red">No bin assigned</font>');
                                    //    pDiv.find('[name=trTemplateRemarks]').parent().append(pTRRemarks);
                                    //    pTRRemarks.show();
                                    //} else {

                                    //    if (CDec2(vDataRow.PickerCount) < 1) {
                                    //        pTRRemarks = pTrTemplateRemarks.clone();
                                    //        pTRRemarks.find('[name=tdRemarks]').html('<font color="red">Product bin does not belong to a section</font>');
                                    //        pDiv.find('[name=trTemplateRemarks]').parent().append(pTRRemarks);
                                    //        pTRRemarks.show();
                                    //    } else {

                                    //        if (vDataRow.PeopleID != peopleID && vDataRow.PeopleID != 'NoFilter') {
                                    //            pTRRemarks = pTrTemplateRemarks.clone();
                                    //            pTRRemarks.find('[name=tdRemarks]').html('<font color="red">Assigned to a different section</font>');
                                    //            pDiv.find('[name=trTemplateRemarks]').parent().append(pTRRemarks);
                                    //            pTRRemarks.show();
                                    //        }
                                    //    }
                                    //}

                                    pItemTotal += CDec2(pDrUnit.SubTotal);
                                };
                            }; // for (var p = 0; p <= vData.Units.length - 1; p++) {
                            //   pDiv.find('[name=spanTotal]').text(AccountingFormat(pItemTotal));
                            mLvCart.OnItem.CartTotal += pItemTotal;



                            pLnk.append($('<img>')
                                .attr('src', '../Content/images/box.png'));
                            pLnk.append(pDiv);
                            pDiv.show();
                            vItem.append(pLnk);

                            //                    ); // .append($('<a>')

                        }; // mLvCart.OnItem
                    };  // if (mLvCart == undefined) {
                    mLvCart.vData = vData;
                    mLvCart.OnItem.CartTotal = 0;
                    mLvCart.SetData(vData.Stocks.Rows);
                    mLvCart.OnItem.Units = vData.Units.Rows;

                    $('#spanCartTotal').text(AccountingFormat(mLvCart.OnItem.CartTotal));
                    $('#spanSkuCount').text(AccountingFormat(vData.Stocks.Rows.length, 0));
                    //additional by janine
                    $('#divCartPanel').show();

                    $('#divCartEmpty').hide();
                    $('#divCartSection').show();
                    if (window.location.hash.substring(1).toLowerCase() == 'cart')
                        $('[data-href=Nav1d]').click();




                    $('[name=lblItemTotal]').hide();

                    return true;
                }; // Display

            }; // function LoadCart()



            $('#btnImNotDone').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'PickProducts/CheckIsQtyConfirmed',
                    data: {
                        ProductID: proj.ProductID,
                        InvoiceID: sessionStorage.CartOrderID,
                        IsConfirmed: 0,
                        SectionID: proj.SectionID
                    },
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        }

                        if (vData.Result == 'ok') {
                            $.mobile.changePage('#pageSalesOrder');
                            LoadCart();
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend

            });


            function MarkAsDone() {
                AjaxSend({
                    url: 'PickProducts/CheckIsQtyConfirmed',
                    data: {
                        ProductID: (CStr2(sessionStorage.IsReplacement) == 'true') ? CStr2(sessionStorage.RefProductID) : proj.ProductID,
                        InvoiceID: sessionStorage.CartOrderID,
                        IsConfirmed: 1,
                        SectionID: proj.SectionID
                    },
                    success: function (vData) {
                        if (vData == null) {
                            NewMessageBox('No reply from server.');
                        }
                        if (CStr2(vData.Error).trim() != '') {
                            NewMessageBox(vData.Error);
                            return;
                        }
                        if (vData.Result == 'ok') {
                            $.mobile.changePage('#pageSalesOrder');
                            LoadCart();
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend


            }


            $('#btnBinListDone').click(function (e) {
                e.preventDefault();

                // 

                MessageBox("This product will be marked as ready (done with picking). Do you want to continue? Please click cancel if you are not yet done picking for this product.",
                                            "Confirm Picking Qty",
                                            [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        MarkAsDone();
                                                        //           
                                                    }
                                                },
                                                {
                                                    Text: "Cancel",
                                                    Click: function () {
                                                        $.mobile.changePage('#pageBin');
                                                        getProductBinLocation();
                                                    }
                                                }
                                            ], null, true
                                        );




            });



            function getProductBinLocation() {

                $('.ui-btn-active').removeClass('ui-btn-active');
                //                if (OfflineMode) {
                //                    LoadBin(PickProducts_offline().getProductBinLocation(proj.ProductID));
                //                } else {

                AjaxSend({
                    url: 'PickProducts/getProductBinLocation',
                    data: {ProductID: proj.ProductID, 
                        FowstID: sessionStorage.FowstID, 
                        InvoiceID: sessionStorage.CartOrderID,
                        SectionID: proj.SectionID
                    },
                    success: function (vData) {
                        LoadAlternativeItems(vData.Data);


                        $('#hdrProductName').text(sessionStorage.ProductName);
                        var qty = CDec2(sessionStorage.AllowableQty);

                        $('#lblQtyOrdered').text(vData.OrderedQty);
                        $('#lblQtyPicked').text(vData.WithdrawnQty);

                        if (!isVisited) {
                            isVisited = true;
                            searchAlternativeItems();
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend
                //                }
            }

            var isVisited = false;

            function searchAlternativeItems() {
                $('#txtAlternativeItemSearch').keydown(function (e) {

                    if (e.which == 13) {
                        sessionStorage.IsInventoryCountEditSKUMode = false;
                        window.IsBrowserloaded = false;
                        Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;

                        Browser.LibraryPIMProduct.Click = function (e, vOpt) {

                            if (sessionStorage.RefProductID == e.ID) {
                                MessageBox("The item you selected already exists in the cart. Please select a different item.",
                                    "Replace Item",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                history.back();
                                                return;
                                            }
                                        }
                                    ]
                                );

                            } else {

                                proj.ReplacedProductID = e.ID;
                                vOpt.DontClose = true;

                                window.NewStockBrowser = true;
                                sessionStorage.IsReplacement = 'true';

                                ChangePage('/PickProducts/SalesOrderStockViewer');

                            }
                        };
                        Browser.LibraryPIMProduct.Show($('#txtAlternativeItemSearch').val());

                        $('#txtAlternativeItemSearch').val('');



                    };
                }); // pTxtSalesRep.keydown(function(e) {
            }


            function LoadAlternativeItems(binData) {
                AjaxSend({
                    url: 'PickProducts/getAlternativeItems',
                    data: { ProductID: proj.ProductID, InvoiceID: sessionStorage.CartOrderID },
                    success: function (vData) {

                        var pLV = $('#lstAlternativeItem');
                        var pItem;

                        pLV.html('');
                        if (CStr2(vData.Data) != '') {

                            if (vData.Data.Rows.length > 0) {
                                $('#lblRemarksReplacement').html('<font color = "red"> Item has been replaced. You are not allowed to modify the quantity for this item.</font>');
                                LoadBin(binData, true);
                            } else {
                                $('#lblRemarksReplacement').html('');
                                LoadBin(binData, false);
                            }

                            for (var i = 0; i < vData.Data.Rows.length; i++) {

                                pItem = $('<li>')
                                .append($('<a>')
                                .data('Data', vData.Data)
                                .data('Index', i)
                                .append($('<h3>')
                                    .attr("style", "white-space:normal")
                                    .text(vData.Data.Rows[i].ProductName)
                                 )

                                );
                                pLV.append(pItem);

                            }


                        }


                        pLV.listview('refresh');



                    },
                    error: function (vError) {
                        var pDummy;

                        return true;
                    }
                }); // AjaxSend


            }

            //additional for warehouse fulfillment
            function PrintPickingList(pInvoiceID) {
                var vDataPrint;
                var vDataHeader;
                var vDataInvHeader;
                var vDataItem;
                var vDataItemSerial;
                var vDataPayment;
                var vDataFooter;
                var vDataMsg;
                var pData = {};
                pData.InvoiceID = pInvoiceID;

                //                    //Get Printer IP Address
                //                    if (OfflineMode) {
                //                        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
                //                        //Get Header
                //                        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
                //                        //Get Invoice Header
                //                        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
                //                        //Get Invoice Items
                //                        vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID);
                //                        //Get Invoice Items Serial
                //                        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(pInvoiceID);
                //                        //Get Footer Amounts
                //                        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID);
                //                        //Get Footer Messages
                //                        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();


                //                        if (window.InAndroid) { //Android Mobile, query to offline
                //                            PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg);
                //                        } else { //Mobile (window), query to offline
                //                            var japplet = document.jzebra;
                //                            var blnIsPrinterFound = false;

                //                            if (japplet != null) {
                //                                while (!blnIsPrinterFound) {

                //                                    if (FindDesktopPrinter("")) {
                //                                        blnIsPrinterFound = true;

                //                                        PrintBillingNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                //                                    };

                //                                }
                //                            } else {
                //                                MessageBox("Jzebra applet was not loaded.",
                //                                "Jzebra",
                //                                [
                //                                    { Text: 'Ok'
                //                                    }
                //                                ]
                //                            );
                //                            }
                //                        }

                //                    } else {


                AjaxSend({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    success: function (vRetDataPrint) {
                        vDataPrint = vRetDataPrint.OrganizationFOWSTData;

                        //Get Header
                        AjaxSend({
                            url: "SalesPayments/getReceiptHeaderInfo",
                            data: pData,
                            success: function (vRetDataHeader) {
                                vDataHeader = vRetDataHeader.SalesPaymentData;

                                //Get Invoice Header
                                AjaxSend({
                                    url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                    data: pData,
                                    success: function (vRetDataInvHeader) {
                                        vDataInvHeader = vRetDataInvHeader.SalesPaymentData;

                                        //Get Invoice Items
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                            data: pData,
                                            success: function (vRetDataItem) {
                                                vDataItem = vRetDataItem.SalesPaymentData;

                                                //Get Invoice Items Serial
                                                AjaxSend({
                                                    url: "SalesPayments/getSalesPaymentReceiptSerial",
                                                    data: pData,
                                                    success: function (vRetDataItemSerial) {
                                                        vDataItemSerial = vRetDataItemSerial.SalesPaymentData;

                                                        //Get Payment Info
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                            data: pData,
                                                            success: function (vRetDataPayment) {
                                                                vDataPayment = vRetDataPayment.SalesPaymentData;

                                                                //Get Footer Amounts
                                                                AjaxSend({
                                                                    url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                                    data: pData,
                                                                    success: function (vRetDataFooter) {
                                                                        vDataFooter = vRetDataFooter.SalesPaymentData;

                                                                        //Get Footer Messages
                                                                        AjaxSend({
                                                                            url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                            data: pData,
                                                                            success: function (vRetDataMsg) {
                                                                                vDataMsg = vRetDataMsg.SalesPaymentData;

                                                                                //                                                                                        if (window.InAndroid || InWinRt) { //Android Mobile Or Mobile (window)
                                                                                PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vRetDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                //                                                                                        } else { //Web App
                                                                                //                                                                                            var japplet = document.jzebra;
                                                                                //                                                                                            var blnIsPrinterFound = false;

                                                                                //                                                                                            if (japplet != null) {
                                                                                //                                                                                                while (!blnIsPrinterFound) {

                                                                                //                                                                                                    if (FindDesktopPrinter("")) {
                                                                                //                                                                                                        blnIsPrinterFound = true;

                                                                                //                                                                                                        PrintBillingNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                                                                //                                                                                                    };

                                                                                //                                                                                                }
                                                                                //                                                                                            } else {
                                                                                //                                                                                                MessageBox("Jzebra applet was not loaded.",
                                                                                //                                                                                                    "Jzebra",
                                                                                //                                                                                                    [
                                                                                //                                                                                                        { Text: 'Ok',
                                                                                //                                                                                                            Click: function () {

                                                                                //                                                                                                                $.mobile.changePage('');

                                                                                //                                                                                                                if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                                                                //                                                                                                                    window.pageHeader.ShowPaymentsPage(false);

                                                                                //                                                                                                            }
                                                                                //                                                                                                        }
                                                                                //                                                                                                    ]
                                                                                //                                                                                                );
                                                                                //                                                                                            }
                                                                                //                                                                                        }

                                                                            }
                                                                        });

                                                                    }
                                                                });
                                                            }
                                                        });

                                                    }
                                                });
                                            }
                                        });
                                    }
                                });

                            }
                        });

                    }
                });




                //                    }



            } // PrintPickingList

            function PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var strStockID = '';
                var IsShowSalesRep = false;
                var pData = {};
                var intFAFCTotalLenght = 40;
                var intFBTotalLenght = 27;
                var intFCDoubleTotalLenght = 18;

                pData.InvoiceID = pInvoiceID;

                //Get Printer IP Address
                if (vDataPrint == undefined) {
                    MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                    return;
                }

                //additional by janine jan 31 2015
                originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
                if (vDataPrint.Rows.length > 0) {
                    if (CStr2(newPrinterIP) != '')
                        strPrinterIP = newPrinterIP;
                    else
                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                    //New set of text to print
                    NewPrintBuilder();

                    //Get Header
                    IsShowSalesRep = false;
                    if (vDataHeader.Rows.length > 0) {

                        IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        strPrint = '';
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim('Picking List'), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);


                        //Get Items
                        if (vDataInvHeader.Rows.length > 0) {
                            strPrint = '\n';
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Delivery Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].DeliveryAddress || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Delivery Date', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].DeliveryDate || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                            if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {
                                var prefix = vDataInvHeader.Rows[0].CustomerTableNo.split('_');

                                if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((prefix[1] || '')), intFAFCTotalLenght, '', 0, 0);
                                    strPrint = strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                } //if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                            } //  if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SO ' + salesOrderNo, intFCDoubleTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFCDoubleTotalLenght, $.format.date(new Date(), 'MMM dd, yyyy HH:mm'), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Prepared By ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                            ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            //Get Items
                            var strProductID = "";
                            var intLimitPerBatchPrint = 0;
                            if (vDataItem.Rows.length > 0) {
                                strPrint = '';
                                var intSKUCnt = 0;
                                for (var intSKU = 0; intSKU <= vDataItem.Rows.length - 1; intSKU++) {

                                    var strStockDescription = $.trim((vDataItem.Rows[intSKU].StockDescription || ''));
                                    var strOrderedQty = SetPrefixPostfix(AccountingFormat((vDataItem.Rows[intSKU].OrderedQuantity || '0'), 0), 4, ' ', 0);
                                    var strUnitMeasure = $.trim((vDataItem.Rows[intSKU].UnitMeasure || ''));
                                    var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.Rows[intSKU].Quantity || '0'), 0), 4, ' ', 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, intFAFCTotalLenght - 14, 'Prepared Qty', intFAFCTotalLenght, '', 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, strOrderedQty, 4, strUnitMeasure, 11, strQty, intFCDoubleTotalLenght, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    intSKUCnt = intSKUCnt + 1;
                                }; //for

                                var strSKUCount = SetPrefixPostfix(AccountingFormat((intSKUCnt || 0), 0), intFAFCTotalLenght - 14, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                    PrintText(strPrinterIP);
                                    ClearCommandBuffer();

                                    //New set of text to print because it causes error when you print directly all items at once.
                                    NewPrintBuilder();
                                    intLimitPerBatchPrint = 0;
                                } // if (intLimitPerBatchPrint 

                            }; //if (vDataItem.Rows.length > 0) {


                            strPrint = 'SO|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix(salesOrderNo, 12, '0', 0)
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                            PrintText(strPrinterIP);
                            ClearCommandBuffer();

                            setTimeout(VerifyPrintReceipt(pInvoiceID), 2000)



                        } // if (vDataInvHeader.Rows.length > 0) {
                    } // if (vDataHeader.Rows.length > 0) {

                } else {
                    MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                                "Printer",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                }

            };


            var theInvoiceID;
            function VerifyPrintReceipt(pInvoiceID) {
                MessageBox("Your transaction has been saved. Were you able to successfully print out a picking list?",
                            "Print Confirmation",
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        location.hash = '';
                                        ShowPendingOrders();
                                    }
                                },
                                {
                                    Text: "No, it didn't print",
                                    Click: function () {
                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                        PromptUserAccess('POS Receipt Reprint', {
                                            OnClickOKSuccess: function () {
                                                PrintPickingList(pInvoiceID);
                                            }
                                        });
                                    }
                                }
                            ], null, true
                        );

                theInvoiceID = pInvoiceID;

                //                    $.mobile.changePage("#dialogPrintConfirmationBill");
            }

            //
            
        }); // END $(function () {
        //        }); // require PickProducts_offline.js
    });






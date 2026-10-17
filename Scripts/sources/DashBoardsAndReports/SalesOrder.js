define([
    'common', 'tablesort',
     '/Scripts3/Browser/LibraryPIMProduct.js?crc=',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=',
    'promise!/Scripts/sources/ERP/OrderProcessing/product/controller.js?v=' + assetversion
    ], function () {
        var mDeferred = $.Deferred();
        var selectedInvoiceID = '';
        var selectedFowstID = '';
        var selectedDownloadCount = 0;
        var actualStatus = '';
        var selectedPrintCount = 0;
        var selectedViewCount = 0;
        var selectedStatus = '';
        var selectedStatusID = '';
        var selectedSalesOrderNo = '';
        var lastSearchString = '';
        var maxDownload = 0;
        var maxPrint = 0;
        var image;
        var signatory;

        var selectedCustomerID = '';
        var selectedCustomerName = '';
        var selectedTotalAmount = '';
        var selectedSKUCount = '';
        var selectedDeliveryAddress = '';
        var selectedBillingAddress = '';
        var selectedSalesRep = '';
        var mSelectedInvoiceItemID;
        var mSelectedQtyMode; // order or serve
        var mSelectedTd;
        var mSelectedTr;
        var dataTableSalesOrder;
        var orderdateFrom = $.format.date(new Date(new Date() -5 * 24 * 60 * 60 * 1000), 'MM-dd-yy');
        var orderdateTo = $.format.date(new Date(), 'MM-dd-yy');
        var mFinalized;
        var mIsPrintSOPA = true;
        var mSelectedAreas;
        var pProductPopup;

        SetSelectedAreasFromLocalStorage();
        HideColumns();

        $(function () {
            if (!window.InAndroid) {
                $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
            }
            pProductPopup = new ERP.OrderProcessing.product.controller();
            //            $("[data-role=header]").fixedtoolbar({ tapToggle: false });


            //            var pPage = $.mobile.activePage;
            //            var pHeader = pPage.find("[data-role=header]");
            //            var pHeight = pHeader.height();

            //            pPage.css('padding-top', pHeight + 'px');

            //            pHeader.fixedtoolbar({ tapToggle: false,
            //                updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
            //                // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
            //            });


            breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
            arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
            //            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
            //                ui.prevPage.addClass("ui-dialog-background ");
            //            });

            //            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
            //                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
            //            });

            // start tab
            // http://stackoverflow.com/a/6850059/334045
            $(document).delegate('.ui-navbar ul li > a', 'click', function () {

                //un-highlight and highlight only the buttons in the same navbar widget
                $(this).closest('.ui-navbar').find('a').removeClass('ui-navbar-btn-active');

                //this bit is the same, you could chain it off of the last call by using two `.end()`s
                $(this).addClass('ui-navbar-btn-active');

                //this starts the same but then only selects the sibling `.content_div` elements to hide rather than all in the DOM
                $('#' + $(this).attr('data-href')).show().siblings('.content_div').hide();
            });
            $('#pageOrderCentral').show();

            if (proj.IsBootstrapDesign()) {
                $('[name=divToolbar] .btn-trail').addClass('btn btn-secondary');
                $('#popupOrderStatuscontent').contents().appendTo('#popupOrderStatuscontent-bs');
                $('#popProductQtyEditcontent').contents().appendTo('#popProductQtyEditcontent-bs');
            }

            GetMaxLimit();

            GetSalesOrderReport('');

            NoPrintHash();

            window.Sopa = {
                LoadData: function () {
                    $('#btnSopaOrderDate').text(orderdateFrom + ' to ' + orderdateTo);
                    GetMaxLimit();
                    GetSalesOrderReport('');
                    ClickEvent(); // call the click events
                }
            }
            //            $('#btnViewReport').click(function () {
            //                GetSalesOrderReport();
            //            });
            
            ClickEvent();

            $('#tabOrderFulfillmentModule').off('click').click(function () {
                NewPromptUserAccess('Access Order Fulfillment Module*', {
                    OnClickOKSuccess: function () {
                        $('#tabOFWork').opentabmenu();
                    }
                });
            });

            $('#tabOrderFulfillmentSettings').off('click').click(function () {
                NewPromptUserAccess('Access Order Fulfillment Settings*', {
                    OnClickOKSuccess: function () {
                        $('#tabOFSettings').opentabmenu();
                    }
                });
            });

            $('body')
            .on('click', '#table-custom-2 tbody tr td', function () {
                var data = dataTableSalesOrder.row(this).data();
                if (data != undefined) {
                    if (!IsInVendorPortal()) {
                        // removed from condition  2020_09_29__12_15_31_5029:
                        //(CStr2(data.CnCUnapprovedReason).trim() != '')
                        //&& (CStr2(data.CncApprovedByID).trim() == '')
                        //&& 

                        AjaxSend({
                            url: '/ERP/IsOkForFC',
                            data: {
                                ID: data.ID
                            },
                            success: function(vData) {
                                var pMsg = '';

                                if (CBool(vData.ProfitNotOk))
                                    pMsg = 'This transaction cannot be processed further. Profitability approval/ validation needs to made before further processing. ';

                                if (CBool(vData.CncNotOk)) {
                                    if (pMsg != '')
                                        pMsg += '<br /><br />';
                                    pMsg += 'This transaction cannot be processed further. Credit and Collection approval/ validation needs to made before further processing.';
                                }

                                if (pMsg != '') {
                                    NewMessageBox(pMsg, 'Transaction Cannot Be Processed Prior to Approvals');
                                    return;
                                }
                                Proceed();
                            }
                        });
                        return;                        
                    }
                    
                    Proceed();

                    function Proceed() {
                        selectedInvoiceID = '';
                        selectedInvoiceID = data.ID;


                        sessionStorage.SalesOrderDate = '';
                        sessionStorage.SalesOrderDate = $.format.date(new Date(data.SalesOrderDate), 'MMM d, yyyy h:mm a');

                        sessionStorage.DeliveryDate = '';
                        sessionStorage.DeliveryDate = $.format.date(new Date(data.DeliveryDate), 'MMM d, yyyy h:mm a');


                        selectedCustomerID = '';
                        selectedCustomerID = data.CustomerID;

                        selectedCustomerName = '';
                        selectedCustomerName = data.CustomerName;

                        selectedFowstID = '';
                        selectedFowstID = data.FowstID;

                        selectedTotalAmount = '';
                        selectedTotalAmount = AccountingFormat(data.Total);


                        selectedSKUCount = '';
                        selectedSKUCount = data.SkuCount;

                        selectedSalesOrderNo = '';
                        selectedSalesOrderNo = data.SalesOrderNo;

                        selectedDeliveryAddress = '';
                        selectedDeliveryAddress = data.Address;

                        selectedBillingAddress = '';
                        selectedBillingAddress = data.BillingAddress;

                        selectedSalesRep = '';
                        sessionStorage.SalesRepID = '';


                        if (CStr2(data.SalesRepresentative) == '') {
                            selectedSalesRep = data.UserName;
                            sessionStorage.SalesRepID = data.UserID;
                        } else {
                            selectedSalesRep = data.SalesRepresentative;
                        }

                        selectedPrintCount = 0;
                        selectedDownloadCount = 0;


                        actualStatus = CStr2(data.Status);

                        selectedStatus = '';
                        selectedStatus = (CStr2(data.Status) == '' || CStr2(data.Status) == 'Created' || CStr2(data.Status).indexOf('New') != -1) ? 'Viewed' : data.Status;
                        ShowPreview();
                    }
                }
            })
                        
            function ClickEvent() {
                //gibutang nko diri kay dili mogana ang click event sa eas pag 2nd or more call sa page - by arnie 8717
                // diri ibutang ang mga event sa mga html tags
                $('#btnSopaOrderDate').text(orderdateFrom + ' to ' + orderdateTo);

                $('#btnDownloadFile').click(function () {

                    //                if (selectedDownloadCount < maxDownload) {
                    GetSalesOrderDetails();
                    //                } else {
                    //                    MessageBox("The maximum download limit for this document has been reached. Please contact your super administrator.",
                    //                            "Download Sales Order",
                    //                            [
                    //                                { Text: 'Ok'
                    //                                }
                    //                            ]
                    //                        );
                    //                }

                });
                
                $('#btnOrderCentralHome').click(function () {
                    window.location = "/People/PeopleMenu";

                });

                $('#formSearch').submit(function (e) {
                    e.preventDefault();
                    selectedInvoiceID = '';
                    Search();
                    return false;
                });

                $('#btnSearch').click(function (e) {

                    e.preventDefault();
                    selectedInvoiceID = '';
                    Search();
                    return false;
                });

                $('#butSearchFromCache').click(function (e) {

                    e.preventDefault();
                    selectedInvoiceID = '';
                    Search('QueryFromCache');
                    return false;
                });

                $('#butSearchRecache').click(function (e) {

                    e.preventDefault();
                    selectedInvoiceID = '';
                    Search('Recache');
                    return false;
                });


                $('#selInvoiceType').change(function (e) {
                    if ($(this).val() == 'Add New') {
                        //$('#divAddInvoiceType').show();
                        $('#pageInvoiceType').popup('open');
                    } else
                        $('#divAddInvoiceType').hide();
                });


                $('#btnOrderDetailsBack').click(function (e) {

                    e.preventDefault();
                    $.mobile.changePage('#pageMain');
                    GetSalesOrderReport('');
                });


                $('#btnSignature').click(function (e) {

                    e.preventDefault();

                    GetSignature();

                });

                $('#btnInvoice').click(function (e) {
                    e.preventDefault();
                    GetInvoiceTypes();
                });


                $('#btnSaveInvoiceDetails').click(function (e) {

                    e.preventDefault();

                    if ($('#selInvoiceType').val() == 'Add New') {

                        MessageBox("Please select an invoice type.",
                                    "Invoice Details",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );

                        return;
                    }


                    if ($.trim($('#txtInvoiceNo').val()) == '') {

                        MessageBox("Please enter an invoice number.",
                                    "Invoice Details",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }

                    var data = {
                        ID: selectedInvoiceID,
                        InvoiceType: $('#selInvoiceType').val(),
                        InvoiceNumber: $('#txtInvoiceNo').val(),
                        Remarks: $('#txtRemarks').val()
                    };

                    SaveSalesInvoice(data);
                    //                MessageBox("Not yet implemented",
                    //                                "Invoice Details",
                    //                                [
                    //                                    { Text: 'Ok' }
                    //                                ]
                    //                            );

                });

                $('#btnPrint').click(function (e) {

                    e.preventDefault();


                    //                if (selectedPrintCount < maxPrint) {
                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (japplet != null) {
                        while (!blnIsPrinterFound) {

                            if (FindDesktopPrinter("")) {
                                blnIsPrinterFound = true;

                                PrintSalesOrderDetails();

                            };

                        }
                    } else {
                        MessageBox("Jzebra applet was not loaded.",
                                "Jzebra",
                                [
                                    {
                                        Text: 'Ok'
                                    }
                                ]
                            );
                    }

                    //                } else {
                    //                    MessageBox("The maximum print limit for this document has been reached. Please contact your super administrator.",
                    //                            "Print Sales Order",
                    //                            [
                    //                                { Text: 'Ok'
                    //                                }
                    //                            ]
                    //                        );
                    //                }
                });


                $('#btnAddInvoiceType').click(function () {
                    var data = {
                        InvoiceType: $('#txtInvoiceType').val(),
                        InvoiceAbbreviation: $('#txtInvoiceTypeAbbr').val()
                    };

                    SaveInvoiceType(data);
                });
                HandleHideSomeSopaFields();
                
                 $('#chkShowRemovedItems').click(function () {
                    ShowPreview();
                });

                $('#butPrintSopa').click(function () {

                    if (!window.InAndroid) {
                        NewMessageBox('What do you want to do?', 'Output', [
                            {
                                Text: 'Print Picking List<br />(Letter Size Paper)',
                                Click: function() {
                                    CreatePdf({
                                        ID: selectedInvoiceID,
                                        Template: 'PickList'
                                    });
                                }
                            },
                            {
                                Text: 'Print Picking List<br />(76-80mm width for POS Printer)',
                                Click: function() {
                                    mIsPrintSOPA = true;
                                    PrintPreviewInHtml();
                                }
                            },
                            {
                                Text: 'Print Warehouse Withdrawal Slip<br />(Letter Size Paper)',
                                Click: function() {
                                    CreatePdf({
                                        ID: selectedInvoiceID,
                                        Template: 'WithdrawalSlip'
                                    });
                                }
                            },
                            {
                                Text: 'Print Warehouse Withdrawal Slip<br />(76-80mm width for POS Printer)',
                                Click: function() {
                                    mIsPrintSOPA = false;
                                    PrintPreviewInHtml();
                                }
                            },
                            {
                                Text: 'Export Data to 3rd Party Application',
                                Click: function() {
                                    NewMessageBox('Do you want to approve this transaction?', 'Approve', [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                AjaxSend({
                                                    data: {InvoiceID: selectedInvoiceID},
                                                    url: 'SalesMenu/Approve',
                                                    success: function (vData) {
                                                        if (vData.Error)
                                                            NewMessageBox(vData.Error, 'Error', null, {
                                                                Close: function () {
                                                                    if (proj.IsBootstrapDesign()) {
                                                                        $('#popupOrderStatus-bs').modal('show');
                                                                    }
                                                                }
                                                            });
                                                        else if (vData.Result == 'ok') {
                                                            WebToast('Success', 'top');
                                                            //$('#butApprove').parent().hide();
                                                        } else
                                                            NewMessageBox('Please try searching for the invoice again.', 'Message', null, {
                                                                Close: function () {
                                                                    if (proj.IsBootstrapDesign()) {
                                                                        $('#popupOrderStatus-bs').modal('show');
                                                                    }
                                                                }
                                                            });
                                                    } // end of success
                                                });
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                    ], {
                                        Close: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#popupOrderStatus-bs').modal('show');
                                            }
                                        }
                                    }); // END NewMessageBox
                                }
                            },
                        ], {
                            Close: function () {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popupOrderStatus-bs').modal('show');
                                }
                            }
                        });
                        
                        return;
                    }

                    function PrintPreviewInHtml() {
                        AjaxSend({
                            url: '/DashboardsAndReports/GetSopaDataToPrint',
                            data: {
                                InvoiceID: selectedInvoiceID,
                                mIsPrintSOPA: mIsPrintSOPA
                            },
                            success: function (vData) {
                                if (vData.Error) {
                                    NewMessageBox(vData.Error);
                                } else {
                                    var headerData = vData.Invoices.Rows[0];
                                    var stocks = vData.Stocks.Rows;

                                    if (window.InAndroid) {
                                        if (mIsPrintSOPA) {
                                            PrintSopaInAndroid(headerData, stocks);
                                        } else {
                                            PrintWithdrawalItemsInAndroid(headerData, stocks);
                                        }
                                        return;
                                    }
                                    var PrintWindow = window.open();

                                    if (PrintWindow == null || typeof (PrintWindow) == 'undefined') {
                                        alert('Please allow popup for this site and click the "Print" button again.');
                                        return;
                                    }

                                    PrintWindow.document.write('<html><head><title>Print</title>');
                                    PrintWindow.document.write('<link rel="stylesheet" href="/Content/flatui-css-custom.css" type="text/css" />');
                                    PrintWindow.document.write('<style type="text/css">');
                                    PrintWindow.document.write('@page{ margin:10px; font-size:30px; } @media print{ .footer{ display: table-footer-group; bottom:0; position:fixed; } .header { font-size: 25px; } .body { font-size: 15px;} }');
                                    PrintWindow.document.write('</style>');
                                    PrintWindow.document.write('</head><body style="-webkit-print-color-adjust:exact; font-family: sans-serif;">');
                                    PrintWindow.document.write('<div style="width:340px;">');

                                    if (mIsPrintSOPA) {
                                        PrintWindow.document.write('<div style="width:100%; text-align:center; font-weight:bold;" class="header">WAREHOUSE PICK LIST</div>');
                                    } else {
                                        PrintWindow.document.write('<div style="width:100%; text-align:center; font-weight:bold;" class="header">WAREHOUSE WITHDRAWAL SLIP</div>');
                                    }
                                
                                    PrintWindow.document.write('<br><table style="width:100%;">');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Serving FOWST:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.OrgFowstName || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">NetSuite Ref:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.NetSuiteNum || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Mobiz PO #:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.MIN || '') + '-' + CDec2(headerData.PurchaseOrderNo) + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">PO Date:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + (CStr2(headerData.PurchaseOrderDate) == '' ? '' : $.format.date(new Date(headerData.PurchaseOrderDate), 'MMM dd, yyyy')) + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Date of Flush:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + FormatDate(headerData.DateCreated, {
                                            DontDisplay12Am: true,
                                            Format: 'MMM dd, yyyy hh:mm a',
                                            FormatNoTime: 'MMM dd, yyyy'
                                        }) + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Customer:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.CustomerName || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Outlet:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.CustomerFowst || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Department:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.CustomerBin || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Remarks:</td>');
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + CStr2(headerData.Remarks || '') + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td class="body" style="width:140px; vertical-align:top;">Sales Rep.:</td>');
                                    var pSalesRepText = CStr2(headerData.SalesRepText).trim();
                                    if (pSalesRepText != '') 
                                        pSalesRepText += '; ';
                                    pSalesRepText += CStr2(headerData.EmployeeFullName);
                                    PrintWindow.document.write('<td class="body" style="font-weight:bold;">' + pSalesRepText + '</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('</table>');

                                    PrintWindow.document.write('<br><table style="width:100%;">');
                                    PrintWindow.document.write('<thead><tr>');
                                    PrintWindow.document.write('<th class="body" style="text-align:left;">Product Name</th>');
                                    PrintWindow.document.write('<th class="body" style="width:50px; text-align:center;">Ordered</th>');
                                    PrintWindow.document.write('<th class="body" style="width:50px; text-align:center;">Served</th>');
                                    PrintWindow.document.write('</tr></thead>');
                                    PrintWindow.document.write('<tbody>');

                                    var pTotalOrderedQty = 0;
                                    var pTotalServedQty = 0;
                                    for (var i = 0; i < stocks.length; i++) {
                                        PrintWindow.document.write('<tr>');
                                        PrintWindow.document.write('<td class="body" style="width:135px; vertical-align:top; word-break:break-word;">' + CStr2(stocks[i].ProductName || '') + '</td>');
                                        PrintWindow.document.write('<td class="body" style="text-align:center; vertical-align:top; word-break:break-word;">' + CDec2(stocks[i].OrderedQuantity || '') + '</td>');
                                        pTotalOrderedQty += CDec2(stocks[i].OrderedQuantity);
                                        PrintWindow.document.write('<td class="body" style="text-align:center; vertical-align:top; word-break:break-word;">' + CDec2(stocks[i].Quantity || '') + '</td>');
                                        pTotalServedQty += CDec2(stocks[i].Quantity);
                                        PrintWindow.document.write('</tr>');
                                    }
                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td colspan=2>');
                                    PrintWindow.document.write('Total ordered qty. (in basic unit)');
                                    PrintWindow.document.write('</td>');
                                    PrintWindow.document.write('<td>');
                                    PrintWindow.document.write($.format.number(pTotalOrderedQty, '#,###.##'));
                                    PrintWindow.document.write('</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('<tr>');
                                    PrintWindow.document.write('<td colspan=2>');
                                    PrintWindow.document.write('Total served qty. (in basic unit)');
                                    PrintWindow.document.write('</td>');
                                    PrintWindow.document.write('<td>');
                                    PrintWindow.document.write($.format.number(pTotalServedQty, '#,###.##'));
                                    PrintWindow.document.write('</td>');
                                    PrintWindow.document.write('</tr>');

                                    PrintWindow.document.write('</tbody>');
                                    PrintWindow.document.write('</table>');
                                    PrintWindow.document.write('<br><br><div style="text-align:center;">*****</div>');
                                    PrintWindow.document.write('</div>');
                                    PrintWindow.document.write('</body></html>');
                                    setTimeout(function () {
                                        PrintWindow.print();
                                        mIsPrintSOPA = true;
                                    }, 1000);
                                }
                            }
                        });
                    } // function PrintPreviewInHtml
                });

                $('#pageOrderStatus [name=butFinalizePickList]').click(function () {

                    if (mFinalized) {
                        WebToast('This transaction was already finalized.', 'top');
                        return;
                    }

                    var pTitle = 'Finalize';
                    var pMessage = 'Are you sure you want to finalize this pick list?<br /><br />';
                    pMessage += 'Finalizing this transaction means that you will not be able to edit this transaction\'s information later on.<br /><br />';
                    //pMessage += 'Only finalized transactions can be printed out (subject to print number limits set by your system administrator).';

                    NewMessageBox(pMessage, pTitle, [
                        {
                            Text: 'Yes',
                            Click: function() {
                                AjaxSend({
                                    url: '/SalesMenu/FinalizePickList',
                                    data: {
                                        InvoiceID: selectedInvoiceID
                                    },
                                    success: function (vData) {
                                        if (vData.Error) {
                                            NewMessageBox(vData.Error, 'Error', null, {
                                                Close: function () {
                                                    if (proj.IsBootstrapDesign()) {
                                                        $('#popupOrderStatus-bs').modal('show');
                                                    }
                                                }
                                            });
                                        } else {
                                            NewMessageBox('Pick list was successfully finalized. Do you want to print withdrawal slip for this order?', '',
                                               [
                                                   {
                                                       Text: 'Yes',
                                                       Click: function () {
                                                           mIsPrintSOPA = false;
                                                           $('#butPrintSopa').click();
                                                       }
                                                   },
                                                   {
                                                       Text: 'No'
                                                   }
                                               ], {
                                                   Close: function () {
                                                       if (proj.IsBootstrapDesign()) {
                                                           $('#popupOrderStatus-bs').modal('show');
                                                       }
                                                   }
                                               });
                                            mFinalized = true;
                                        }
                                    }
                                });
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ], {
                        Close: function () {
                            if (proj.IsBootstrapDesign()) {
                                $('#popupOrderStatus-bs').modal('show');
                            }
                        }
                    });
                });

                function PrintSopaInAndroid(vHeader, vStocks) {

                    AjaxSend({
                        url: '/Erp/GetPrinterByFowstIdAndUuid',
                        data: {
                            FowstID: sessionStorage.FowstID,
                            Uuid: Device.Serial()
                        },
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Result != null)
                                Proceed(vData.Result);
                            else
                                NewMessageBox('Unexpected result.');
                        }
                    });

                    function Proceed(vPrinter) {
                        NewPrintBuilder();
                        var pText = FormatTextToPrint(20, 'WAREHOUSE PICK LIST', 20, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(30, 'Serving FOWST:' + CStr2(vHeader.OrgFowstName), 30, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(20, 'Mobiz PO #:' + CStr2(vHeader.MIN || '') + '-' + CDec2(vHeader.PurchaseOrderNo), 20, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(32, 'Customer:' + CStr2(vHeader.CustomerName), 32, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = '\n';

                        if (CStr2(vHeader.NetSuiteNum).trim() != '')
                            pText += FormatTextToPrint(40, 'NetSuite Ref::' + CStr2(vHeader.NetSuiteNum), 40, '', 0, '', 0, 1);
                        
                        pText += FormatTextToPrint(40, 'Outlet:' + CStr2(vHeader.CustomerFowst), 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Department:' + CStr2(vHeader.CustomerBin), 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Remarks:' + CStr2(vHeader.Remarks), 40, '', 0, '', 0, 1);

                        pText += '\n';

                        pText += FormatTextToPrint(40, 'Product Name', 20, 'Ordered', 30, 'Served', 40, 1);

                        for (var p = 0; p <= vStocks.length - 1; p++) {
                            var pStock = vStocks[p];
                            pText += FormatTextToPrint(40, CStr2(pStock.ProductName), 20, ' ' + $.format.number(CDec2(pStock.OrderedQuantity), '#,###.##'), 30, '____', 40, 1);;
                        }

                        ReadyTextToPrint(pText, 3, 1, 1, 0, 0, 0, 0, 0, 1);
                        PrintText(vPrinter.PrinterIPAddress);
                    }
                }

                function PrintWithdrawalItemsInAndroid(vHeader, vStocks) {

                    AjaxSend({
                        url: '/Erp/GetPrinterByFowstIdAndUuid',
                        data: {
                            FowstID: sessionStorage.FowstID,
                            Uuid: Device.Serial()
                        },
                        success: function (vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Result != null)
                                Proceed(vData.Result);
                            else
                                NewMessageBox('Unexpected result.');
                        }
                    });

                    function Proceed(vPrinter) {
                        NewPrintBuilder();
                        var pText = FormatTextToPrint(26, 'WAREHOUSE WITHDRAWAL SLIP', 26, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(30, 'Serving FOWST:' + CStr2(vHeader.OrgFowstName), 30, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(20, 'Mobiz PO #:' + CStr2(vHeader.MIN || '') + '-' + CDec2(vHeader.PurchaseOrderNo), 20, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        pText = FormatTextToPrint(32, 'Customer:' + CStr2(vHeader.CustomerName), 32, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 1, 1, 1, 0, 0, 0);


                        pText = '\n';

                        if (CStr2(vHeader.NetSuiteNum).trim() != '')
                            pText += FormatTextToPrint(40, 'NetSuite Ref::' + CStr2(vHeader.NetSuiteNum), 40, '', 0, '', 0, 1);
                        //pText += FormatTextToPrint(40, 'Mobiz PO #:' + CStr2(vHeader.MIN || '') + '-' + CDec2(vHeader.PurchaseOrderNo), 40, '', 0, '', 0, 1);
                        //pText += FormatTextToPrint(40, 'Customer:' + CStr2(vHeader.CustomerName), 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Outlet:' + CStr2(vHeader.CustomerFowst), 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Department:' + CStr2(vHeader.CustomerBin), 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Remarks:' + CStr2(vHeader.Remarks), 40, '', 0, '', 0, 1);

                        pText += '\n';

                        pText += FormatTextToPrint(40, 'Product Name', 20, 'Ordered', 30, 'Served', 40, 1);

                        for (var p = 0; p <= vStocks.length - 1; p++) {
                            var pStock = vStocks[p];
                            pText += FormatTextToPrint(40, CStr2(pStock.ProductName), 20, ' ' + $.format.number(CDec2(pStock.OrderedQuantity), '#,###.##'), 30, ' ' + $.format.number(CDec2(pStock.Quantity), '#,###.##'), 40, 1);;
                        }

                        pText += '\n\n';
                        pText += FormatTextToPrint(40, '____________', 40, '', 0, '', 0, 1);
                        pText += FormatTextToPrint(40, 'Prepared By', 40, '', 0, '', 0, 1);
                        ReadyTextToPrint(pText, 3, 1, 1, 0, 0, 0, 0, 0, 1);
                        PrintText(vPrinter.PrinterIPAddress);

                        mIsPrintSOPA = true;
                    }
                }

                $('#selSopaFilter').change(function (e) {
                    Search();
                    // dataTableSalesOrder.column(11).search($(this).val()).draw();
                })

                $('#butSopaFilterArea').click(function (e) {
                    ShowAreas();
                    // dataTableSalesOrder.column(11).search($(this).val()).draw();
                })

                $('#selSopaSort').change(function (e) {
                    Search();
                });

                $('#btnSopaOrderDate').click(function () {
                    DualCalendar({
                        Format: 'MM-dd-yy',
                        Click: function (e) {
                            orderdateFrom = e.From;
                            orderdateTo = e.To;
                            $('#btnSopaOrderDate').text(e.From + ' to ' + e.To);
                            GetSalesOrderReport();
                        }
                    })
                });

                ResetAutoRefresh();
            }
            mDeferred.resolve();
        }); //End Inner function

        function HandleHideSomeSopaFields() {
            AjaxSend({
                url: '/SystemBehaviorSettings/GetPurchaseOrderSettings',
                success: function(vData) {
                    if (CBool(vData.HideSomeSopaFields)) {
                        $('#divSopaApprovedBy').hide();
                        $('#divSopaDeliveryAddress').hide();
                        $('#divSopaDeliveryDate').hide();
                        $('#divSopaBillingAddress').hide();
                    }
                }
            });
        }

        function GetInvoiceTypes() {
            AjaxSend({
                url: 'DashboardsAndReports/GetInvoiceTypes',
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayInvoiceTypes(vData.Results.Rows);
                    }
                } // end of success
            });
        }

        function DisplayInvoiceTypes(vRows) {
            $('#selInvoiceType').empty();
            $('#selInvoiceType').append(new Option('Select Type of Invoice', '', false));

            for (var i = 0; i <= vRows.length - 1; i++) {
                $('#selInvoiceType').append(new Option(vRows[i].InvoiceType + ' (' + vRows[i].InvoiceAbbreviation + ')', vRows[i].ID, false));
            }

            $('#selInvoiceType').append(new Option('Add New Type of Invoice', 'Add New', false));

            try {
                $('#selInvoiceType').selectmenu('refresh');
            }
            catch (e) {
                $('#selInvoiceType').selectmenu();
            }

        }

        function SaveInvoiceType(data) {
            //            if ($('#selInvoiceType').val() == '' || $('#selInvoiceType').val() == 'Add New') {
            //                MessageBox('Please select invoice type');
            //                return;
            //            }

            //            if ($('#txtInvoiceNo').val() == '' || parseInt($('#txtInvoiceNo').val()) == 0) {
            //                MessageBox('Please enter valid invoice number');
            //                return;
            //            }

            AjaxSend({
                url: 'DashboardsAndReports/SaveInvoiceType',
                data: data,
                success: function (vData) {
                    $('#pageInvoiceType').popup('close');
                    GetInvoiceTypes();
                } // end of success
            });
        }

        function SaveSalesInvoice(data) {
            if ($('#selInvoiceType').val() == '' || $('#selInvoiceType').val() == 'Add New') {
                MessageBox('Please select invoice type');
                return;
            }

            if ($('#txtInvoiceNo').val() == '' || parseInt($('#txtInvoiceNo').val()) == 0) {
                MessageBox('Please enter valid invoice number');
                return;
            }

            AjaxSend({
                url: 'DashboardsAndReports/SaveSalesInvoice',
                data: data,
                success: function (vData) {
                    //                    $.mobile.changePage('#pageMain');
                    MessageBox('Invoice has been successfully saved.', 'Save Invoice');
                    GetSalesOrderReport('');
                    $('#btnSalesOrderProcessingAnalyticsPage').click();

                } // end of success
            });
        }

        function PrintSalesOrderDetails() {

            AjaxSend({
                url: 'DashboardsAndReports/GetSalesOrderDetails',
                data: { FowstID: selectedFowstID, InvoiceID: selectedInvoiceID },
                success: function (vData) {
                    if (vData.Result == "error") {
                        //                        DisplayRawData(vData.POSTransactionDetail.Rows, data);
                        MessageBox("No record(s) found.",
                                "Sales Order",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    } else {
                        PrintForWarehouse(vData);


                    }

                } // end of success
            });
        }

        function PrintInvoice(vData) {

            var strTemp = '';
            var strPrint = '';
            var IsShowSalesRep = false;

            var intDesktopPrintLength = 320;

            var japplet = document.jzebra;

            var dateToday = new Date();
            var copyCount = 1;

            //            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Invoice No. ', 26, selectedSalesOrderNo, intDesktopPrintLength, '', 0);
            //            japplet.append(strPrint + "\n");

            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Order No. ', 26, selectedSalesOrderNo, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Order Date: ', 26, $.format.date(new Date(vData.InvoiceDate)), intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Representative: ', 26, selectedSalesRep, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n\n");

            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Customer: ', 26, vData.CustomerName, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Delivery Address: ', 26, vData.DeliveryAddress, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Billing Address: ', 26, vData.BillingAddress, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n\n");

            strPrint = FormatTextToTable(intDesktopPrintLength, '#', 6, 'Stock Number', 26, 'Stock Description', 65, SetPrefixPostfix('Qty', 8, ' ', 0), 80, 'Unit Measure', 100, SetPrefixPostfix('Unit Price', 14, ' ', 0), 120, SetPrefixPostfix('Sub Total', 14, ' ', 0), intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n\n");
            //Get Items
            if (vData.StockData.Rows.length > 0) {

                strPrint = '';
                for (var i = 0; i <= vData.StockData.Rows.length - 1; i++) {

                    var strStockNumber = $.trim(vData.StockData.Rows[i].StockNumber || '');
                    var strStockDescription = $.trim(vData.StockData.Rows[i].StockDescription || '');
                    var strUnitPrice = SetPrefixPostfix(AccountingFormat(vData.StockData.Rows[i].UnitPrice || '0'), 14, ' ', 0);
                    var strQty = SetPrefixPostfix(AccountingFormat((vData.StockData.Rows[i].Quantity || '0'), 0), 8, ' ', 0);
                    var strUnitMeasure = $.trim(vData.StockData.Rows[i].UnitMeasure || '');
                    var strSubTotal = SetPrefixPostfix(AccountingFormat(vData.StockData.Rows[i].SubTotal || '0'), 14, ' ', 0);

                    strPrint = FormatTextToTable(intDesktopPrintLength, (i + 1).toString(), 6, strStockNumber, 26, strStockDescription, 65, strQty, 80, strUnitMeasure, 100, strUnitPrice, 120, strSubTotal, intDesktopPrintLength, '', 0);
                    japplet.append(strPrint + "\n");
                }

                strPrint = FormatTextToTable(intDesktopPrintLength, ' ', 6, ' ', 26, ' ', 65, ' ', 80, ' ', 100, ' ', 120, ' ', intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");

                strPrint = FormatTextToTable(intDesktopPrintLength, ' ', 6, ' ', 26, ' ', 65, ' ', 80, ' ', 100, 'SKU Count: ', 120, SetPrefixPostfix(AccountingFormat((vData.SKUCount || '0'), 0), 14, ' ', 0), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");
                strPrint = FormatTextToTable(intDesktopPrintLength, ' ', 6, ' ', 26, ' ', 65, ' ', 80, ' ', 100, 'Sub Total: ', 120, SetPrefixPostfix(AccountingFormat(vData.TotalAmountDue || '0'), 14, ' ', 0), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");
                strPrint = FormatTextToTable(intDesktopPrintLength, ' ', 6, ' ', 26, ' ', 65, ' ', 80, ' ', 100, 'Total Discount: ', 120, SetPrefixPostfix(AccountingFormat(vData.DiscountAmount || '0'), 14, ' ', 0), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");
                strPrint = FormatTextToTable(intDesktopPrintLength, ' ', 6, ' ', 26, ' ', 65, ' ', 80, ' ', 100, 'Amount Due: ', 120, SetPrefixPostfix(AccountingFormat(vData.NetAmountDue || '0'), 14, ' ', 0), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");


                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Buyer/Purchaser: ', 26, vData.PurchaserName, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Purchase Order No.: ', 26, vData.PurchaseOrderNo, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Purchase Order Date: ', 26, (vData.PurchaseOrderDate == '' ? '' : $.format.date(new Date(vData.PurchaseOrderDate))), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");



                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Ordered By: ', 26, selectedSalesRep, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Prepared By: ', 26, vData.PeopleName, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Date Issued: ', 26, $.format.date(dateToday), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'No. of Copies: ', 26, copyCount.toString(), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");


                strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, 138);
                japplet.append(strPrint + "\n");

                strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, 138);
                japplet.append(strPrint + "\n");

                japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                japplet.print();



            } else {
                MessageBox('No items to print.');
            };

            setTimeout(VerifyPrintOrder(selectedInvoiceID), 5000);
            return;




        }



        function PrintForWarehouse(vData) {

            var strTemp = '';
            var strPrint = '';
            var IsShowSalesRep = false;

            var intDesktopPrintLength = 320;

            var japplet = document.jzebra;

            var dateToday = new Date();
            var copyCount = 1;

            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Order No. ', 26, selectedSalesOrderNo, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Order Date: ', 26, $.format.date(new Date(vData.SalesOrderDate)), intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n");
            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Sales Representative: ', 26, selectedSalesRep, intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n\n");

            strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Customer: ', 26, vData.CustomerName, intDesktopPrintLength, '', 0);

            japplet.append(strPrint + "\n\n");

            strPrint = FormatTextToTable(intDesktopPrintLength, '#', 6, 'Stock Number', 26, 'Stock Description', 65, SetPrefixPostfix('Qty Ordered', 8, ' ', 0), 78, 'Unit Measure', 95, SetPrefixPostfix('Qty Prepared', 8, ' ', 0), 110, SetPrefixPostfix('Remarks', 12, ' ', 0), intDesktopPrintLength, '', 0);
            japplet.append(strPrint + "\n\n");
            //Get Items

            if (vData.StockData.Rows.length > 0) {

                strPrint = '';
                for (var i = 0; i <= vData.StockData.Rows.length - 1; i++) {

                    var strStockNumber = $.trim(vData.StockData.Rows[i].StockNumber || '');
                    var strStockDescription = $.trim(vData.StockData.Rows[i].StockDescription || '');
                    var strUnitPrice = SetPrefixPostfix(AccountingFormat(vData.StockData.Rows[i].UnitPrice || '0'), 14, ' ', 0);
                    var strQty = SetPrefixPostfix(AccountingFormat((vData.StockData.Rows[i].OrderedQuantity || '0'), 0), 8, ' ', 0);
                    var strUnitMeasure = $.trim(vData.StockData.Rows[i].UnitMeasure || '');
                    var strSubTotal = SetPrefixPostfix(AccountingFormat(vData.StockData.Rows[i].SubTotal || '0'), 14, ' ', 0);

                    strPrint = FormatTextToTable(intDesktopPrintLength, (i + 1).toString(), 6, strStockNumber, 26, strStockDescription, 65, strQty, 78, strUnitMeasure, 95, '', intDesktopPrintLength, '', 0, '', 0);
                    japplet.append(strPrint + "\n");
                }

                strPrint = "\n" + FormatTextToDesktopPrint(intDesktopPrintLength, 'SKU Count: ', 26, vData.SKUCount, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");

                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Printed By: ', 26, vData.PeopleName, intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n");
                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Date: ', 26, $.format.date(dateToday), intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");


                strPrint = FormatTextToDesktopPrint(intDesktopPrintLength, 'Prepared By: ', 26, '', intDesktopPrintLength, '', 0);
                japplet.append(strPrint + "\n\n");


                strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, 138);
                japplet.append(strPrint + "\n");

                strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, 138);
                japplet.append(strPrint + "\n");

                japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                japplet.print();


            } else {
                MessageBox('No items to print.');
            };

            setTimeout(VerifyPrintOrder(selectedInvoiceID), 5000);
            return;
        }

        function VerifyPrintOrder(pInvoiceID) {
            MessageBox("Were you able to successfully print a picking list?",
                            "Print Confirmation",
                            [
                                { Text: 'Yes',
                                    Click: function () {

                                        selectedStatus = '';
                                        selectedStatus = 'Printed';

                                        selectedPrintCount = selectedPrintCount + 1;

                                        proj.UpdateOrderStatus();

                                        history.back();
                                    }
                                },
                                { Text: "No, it didn't print",
                                    Click: function () {

                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                        PromptUserAccess('Reprint Picking List', {
                                            OnClickOKSuccess: function () {

                                                //                                                $.mobile.changePage(window.pageSourcePage);
                                                PrintSalesOrderDetails(pInvoiceID);
                                            }
                                        });
                                    }
                                }
                            ], null, true
                );
        }


        function FormatTextToTable(intMaxChar, strText1, intLimit1, strText2, intLimit2, strText3, intLimit3, strText4, intLimit4, strText5, intLimit5, strText6, intLimit6, strText7, intLimit7, strText8, intLimit8) {
            var strReturn = '';
            var intStart1 = 0;
            var intText1Index = 0;
            var intStart2 = intLimit1 + 1;
            var intText2Index = 0;
            var intStart3 = intLimit2 + 1;
            var intText3Index = 0;
            var intStart4 = intLimit3 + 1;
            var intText4Index = 0;
            var intStart5 = intLimit4 + 1;
            var intText5Index = 0;
            var intStart6 = intLimit5 + 1;
            var intText6Index = 0;
            var intStart7 = intLimit6 + 1;
            var intText7Index = 0;
            var intStart8 = intLimit7 + 1;
            var intText8Index = 0;

            for (var p = 0; p <= intMaxChar; p++) {

                //Text1
                if (0 <= p && p <= intLimit1) {
                    if (intText1Index <= (strText1.length - 1)) {
                        strReturn = strReturn + strText1.substring(intText1Index, intText1Index + 1);
                        intText1Index = intText1Index + 1;
                    } else {
                        if (strText2.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit1 + 1) < p && p <= intLimit2) {
                    if (intText2Index <= (strText2.length - 1)) {
                        strReturn = strReturn + strText2.substring(intText2Index, intText2Index + 1);
                        intText2Index = intText2Index + 1;
                    } else {
                        if (strText3.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit2 + 1) < p && p <= intLimit3) {
                    if (intText3Index <= (strText3.length - 1)) {
                        strReturn = strReturn + strText3.substring(intText3Index, intText3Index + 1);
                        intText3Index = intText3Index + 1;
                    } else {
                        if (strText4.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit3 + 1) < p && p <= intLimit4) {
                    if (intText4Index <= (strText4.length - 1)) {
                        strReturn = strReturn + strText4.substring(intText4Index, intText4Index + 1);
                        intText4Index = intText4Index + 1;
                    } else {
                        if (strText5.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit4 + 1) < p && p <= intLimit5) {
                    if (intText5Index <= (strText5.length - 1)) {
                        strReturn = strReturn + strText5.substring(intText5Index, intText5Index + 1);
                        intText5Index = intText5Index + 1;
                    } else {
                        if (strText6.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit5 + 1) < p && p <= intLimit6) {
                    if (intText6Index <= (strText6.length - 1)) {
                        strReturn = strReturn + strText6.substring(intText6Index, intText6Index + 1);
                        intText6Index = intText6Index + 1;
                    } else {
                        if (strText7.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit6 + 1) < p && p <= intLimit7) {
                    if (intText7Index <= (strText7.length - 1)) {
                        strReturn = strReturn + strText7.substring(intText7Index, intText7Index + 1);
                        intText7Index = intText7Index + 1;
                    } else {
                        if (strText8.length > 0) {
                            strReturn = strReturn + " ";
                        }
                    }
                }
                else if ((intLimit7 + 1) < p && p <= intLimit8) {
                    if (intText8Index <= (strText8.length - 1)) {
                        strReturn = strReturn + strText8.substring(intText8Index, intText8Index + 1);
                        intText8Index = intText8Index + 1;
                    } else {
                        strReturn = strReturn + " ";
                    }
                } else {
                    strReturn = strReturn + " ";
                }

                //Check If All text has been loaded, if not, do in new line
                if (p == (intMaxChar - 1)) {
                    if (intText1Index < (strText1.length - 1) || intText2Index < (strText2.length - 1) || intText3Index < (strText3.length - 1) || intText4Index < (strText4.length - 1) || intText5Index < (strText5.length - 1) || intText6Index < (strText6.length - 1) || intText7Index < (strText7.length - 1) || intText8Index < (strText8.length - 1)) {
                        p = 0;
                        strReturn = strReturn + "\n";
                    }

                }
            };

            return strReturn;
        }



        function Search(vCacheMode) {
            lastSearchString = $.trim($('#txtSearch').val());
            GetSalesOrderReport($.trim($('#txtSearch').val()), vCacheMode) ;
            ResetAutoRefresh();
        };

        var mAutoRefreshTimer;
        function ResetAutoRefresh() {
            if (mAutoRefreshTimer)
                clearInterval(mAutoRefreshTimer);
            //mAutoRefreshTimer = setInterval(Search, 10 * 1000);
            mAutoRefreshTimer = setInterval(Search, 3 * 1000 * 60);
        }

        function GetMaxLimit() {
            AjaxSend({
                url: 'DashBoardsAndReports/GetMaxLimit',
                success: function (vData) {
                    maxDownload = vData.MaxDownload;
                    maxPrint = vData.MaxPrint;
                }
            });
        }

        function SetSelectedAreasFromLocalStorage() {
            try {
                mSelectedAreas = JSON.parse(localStorage.FcSelectedAreas);
                if ($.isArray(mSelectedAreas) == false)
                    mSelectedAreas = [];
            } catch (e) {
                mSelectedAreas = [];
                console.warn(e);
            }
        }

        function GetSalesOrderReport(sString, vCacheMode) {
            //            $('#MainNav1b').css('display', 'none');
            //            $('#MainNav1a').css('display', 'block');

            //            $('#navGroup').addClass('ui-navbar-btn-active');
            //            $('#navGroup').addClass('ui-btn-active');

            //            $('#navCutOff').removeClass('ui-navbar-btn-active');
            //            $('#navCutOff').removeClass('ui-btn-active');
            var pAreas = '';
            var pAreaText = '';
            for (var pIndex = 0; pIndex <= mSelectedAreas.length - 1; pIndex++) {
                if (mSelectedAreas[pIndex] == '[All Areas]') {
                    pAreas = '';
                    pAreaText = '[All Areas]';
                    break;
                }
                if (pAreas != '') {
                    pAreas += ', ';
                    pAreaText += ', ';
                }
                pAreas += ReadyString(mSelectedAreas[pIndex]);
                pAreaText += mSelectedAreas[pIndex];
            }
            localStorage.FcSelectedAreas = JSON.stringify(mSelectedAreas);

            var data = {
                SearchString: sString,
                OrderDateFrom: orderdateFrom,
                OrderDateTo: orderdateTo,
                SortBy: $('#selSopaSort').val(),
                Status: $('#selSopaFilter').val(),
                Areas: pAreas,
                CacheMode: vCacheMode
            }

            if (pAreaText == '')
                pAreaText = '[All Areas]';
            $('#butSopaFilterArea').text(pAreaText);
            if (IsGuid(sessionStorage.SupplierID))
                data.VendorID = sessionStorage.SupplierID;
            if (IsInVendorPortal())
                data.IsInVendorPortal = 1;
            AjaxSend({
                url: 'DashBoardsAndReports/GetSalesOrderReport',
                data: data,
                success: function (vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error, 'Error');
                        return;
                    }
                        
                    DisplaySalesOrderReport(vData.SalesOrderReport.Rows);
                    $("#lblColumnName").text('Creation Date');
                    handlePlaySound();
                }
            });
        }
        
        function ShowAreas() {
          
            var pColumns = [
                {
                    Label: 'Include',
                    Render: function(v1, v2, vData) {
                        return '<div name="divInclude" />';
                    }
                },
                { 
                    Column: 'Area', 
                    Label: 'Area' 
                }
            ];

            BrowsePopup('Areas', 'Area',
                [
                    {
                        AutoSearch: true,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            if ($.isArray(e)) {
                                PreSelectArea(e);
                            } else {
                                mSelectedAreas = [e.Area];
                                Search();
                            }
                        },
                        AllowEmptySearch: true,
                        StyleFormat: 'table',
                        InitComplete: function(vTable) {
                            vTable.find('.divInclude').click(function(e) {
                                e.stopPropagation();
                            });
                        },
                        rowCallback: function(row, data, dataIndex){
                            var pDiv = $(row).find('[name=divInclude]');
                            var pChk = $('<input type=checkbox name=chkInclude>');
                            pDiv.append(pChk);
                            pChk.click(function(e) {
                                //e.preventDefault();
                                e.stopImmediatePropagation();
                                //return false;
                            });
                        }
                    }
                ], {
                    IncludeAllAreas: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function PreSelectArea() {
            var pChks = $('#tblBrowsePopup [name=chkInclude]');
            var pData = [];
            var pIndex;
            for (var p = 0; p <= pChks.length - 1; p++) {
                var pChk = $(pChks[p]);
                if (pChk.prop('checked')) {
                    var pTR = pChk.closest('tr');
                    var pDR = pTR.data('data');
                    pData.push(pDR.Area);
                } // todo: if qty = 0, delete at server
            }
            
            if (pData.length > 0) {
            	mSelectedAreas = pData;
                Search();
            }
        }

        var mHasNewStatus;

        function DisplaySalesOrderReport(vRows) {
            var tblSalesOrder = $('#table-custom-2');
            var ctr = 0;
            var pHasNetSuiteResponse;

            if ($.fn.DataTable.isDataTable(tblSalesOrder)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblSalesOrder.empty();
                dataTableSalesOrder.destroy();
            }

            mHasNewStatus = false;
            dataTableSalesOrder = tblSalesOrder.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                pageLength: 25,
                "ordering": true,
                data: vRows,
                columns: [
                    { // 0
                        data: function () {
                            return ctr += 1;
                        }
                    },
                    { // 1
                        data: function (data) {
                            return CStr2(data.FowstName);
                        }
                    },
                    { // 1
                        data: function (data) {
                            return CStr2(data.Area);
                        }
                    },
                    { // 2
                        data: function (data) {
                            return CStr2(data.EncodingFowstName);
                        }
                    },
                    { // 3
                        data: function (data) {
                            return $.format.date(new Date(data.LastUpdated));
                        }
                    },
                    { // 4
                        data: function (data) {
                            return FormatDate(data.DateNeeded, {
                                DontDisplay12Am: true,
                                Format: 'MMM dd, yyyy hh:mm a',
                                FormatNoTime: 'MMM dd, yyyy'
                            }) || '';
                        }
                    },
                    { // 5
                        data: function (data) {
                            return CStr2(data.SalesOrderNo)
                        }
                    },
                    { // 6
                        data: function (data) {
                            if (IsInVendorPortal())
                                return CStr2(data.SalesOrderNo);
                            else
                                return CStr2(data.PurchaseOrderText);
                        }
                    },
                    { // 6a // Customer PO Date
                        data: function (data) {
                            if (IsInVendorPortal())
                                return FormatDate(data.SalesOrderDate, {
                                    DontDisplay12Am: true,
                                    Format: 'MMM dd, yyyy hh:mm a',
                                    FormatNoTime: 'MMM dd, yyyy'
                                }) || '';
                            else
                                return '';
                        }
                    },
                    { // 7
                        data: function (data) {
                            var pTransNum = CStr2(data.SalesOrderNo);
                            // commented on 2020-08-14
                            //if (pTransNum.indexOf('PO ') == 0)
                            //    return CStr2(sessionStorage.OrgName);
                            var pOut = data.CustomerName;
                            if (CStr2(data.DeliveryFowstName) != '')
                                pOut += data.DeliveryFowstName;
                            return CStr2(pOut);
                        }
                    },
                    { // 8
                        data: function (data) {
                            return CStr2(data.CustomerFowst)
                        }
                    },
                    { // 9
                        data: function (data) {
                            return CStr2(data.CustomerBin)
                        }
                    },
                    { // 10
                        data: function (data) {
                            return CStr2(data.SalesRepresentative) != '' ? data.SalesRepresentative : data.UserName;
                        }
                    },
                    { // 11
                        data: function (data) {
                            return CStr2(data.Address)
                        }
                    },
                    { // 12
                        data: function (data) {
                            return CStr2(data.DeliveryMethod);
                        }
                    },
                    { // 13
                        data: function (data) {
                            return CStr2(data.SupplierName) + ' - ' + CStr2(data.SupplierFowstName);
                        }
                    },
                    { // 14
                        data: function (data) {
                            return CStr2(data.Remarks)
                        }
                    },
                    { // 15
                        width: '370px',
                        data: function (data) {
                            if (CStr2(data.Status).toLowerCase().indexOf('new') != -1)
                                mHasNewStatus = true;
                            return (CStr2(data.Status) != '' && CStr2(data.Status) != 'New') ? '<strong><font color="green">' + data.Status + '</font></strong>' : '<strong><font color="red">New</font></strong>';
                        }
                    },
                    {
                        data: function (data) {
                            var pText = CStr2(data.NetSuiteReponse);
                            if (pText.trim().length > 0)
                                pHasNetSuiteResponse = true;
                            return HtmlEncode(pText);
                        }
                    }
                ],
                initComplete: function () {
                    tblSalesOrder.css('width', '100%').removeClass('dataTable');
                    if (!proj.IsBootstrapDesign()) {
                        tblSalesOrder.table('refresh');
                    }
                    //PopupReposition()
                },
                "aaSorting": [],
                //: [{ extend: 'excel', text: 'Download Excel File' }],
                sDom: '<"top">rt<"bottom"ip><"clear">' //hide search label and field
            })
            // dataTableSalesOrder.column(13).search($('#selSopaFilter').val()).draw();
            if (pHasNetSuiteResponse)
                tblSalesOrder.find('[name=netsuite]').show();
            else
                tblSalesOrder.find('[name=netsuite]').hide();
            HideColumns();
            return;
            var pLV = $("#tblSalesOrderReportBody");

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr style="cursor: pointer;">')

                         .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text($.format.date(new Date(vRows[i].SalesOrderDate)))
                        )
                        .append($('<td>')
                            .text(vRows[i].SalesOrderNo)

                        )
                        .append($('<td>')
                            .text(vRows[i].CustomerName + ' - ' + vRows[i].DeliveryFowstName)
                            // .text(vRows[i].CustomerName + ' - ' + vRows[i].Address)
                        )
                        .append($('<td>')
                            .text(vRows[i].CustomerFowst)
                        )
                        .append($('<td>')
                            .text(vRows[i].CustomerBin)
                        )
                        .append($('<td>')
                            .text(CStr2(vRows[i].SalesRepresentative) != '' ? vRows[i].SalesRepresentative : vRows[i].UserName)
                        )
                        .append($('<td>')
                            .text(vRows[i].Address)
                        )
                        .append($('<td>')
                            .text(CStr2(vRows[i].SupplierName) + ' - ' + CStr2(vRows[i].SupplierFowstName))
                        )
                        .append($('<td>')
                            .text(CStr2(vRows[i].Remarks))
                        )
                        .append($('<td>')
                            .html(CStr2(vRows[i].Status) != '' ? '<strong><font color="green">' + vRows[i].Status + '</font></strong>' : '<strong><font color="red">New</font></strong>')
                        )
                        .attr('ID', vRows[i].ID)
                            .attr('SalesOrderNo', vRows[i].SalesOrderNo)
                            .attr('CustomerID', vRows[i].CustomerID)
                            .attr('CustomerName', vRows[i].CustomerName)
                            .attr('UserName', vRows[i].UserName)
                            .attr('UserID', vRows[i].UserID)
                            .attr('SalesRepresentative', vRows[i].SalesRepresentative)
                            .attr('Address', vRows[i].Address)
                            .attr('BillingAddress', vRows[i].BillingAddress)
                            .attr('FowstID', vRows[i].FowstID)
                            .attr('Total', vRows[i].Total)
                            .attr('SkuCount', vRows[i].SkuCount)
                            .attr('SalesOrderDate', vRows[i].SalesOrderDate)
                            .attr('DeliveryDate', vRows[i].DeliveryDate)
                            .attr('Status', vRows[i].Status)
                            .click(function (e) {

                                e.preventDefault();

                                //                                if (CDec2($(this).attr('Total')) > 0) {
                                selectedInvoiceID = '';
                                selectedInvoiceID = $(this).attr('ID');


                                sessionStorage.SalesOrderDate = '';
                                sessionStorage.SalesOrderDate = $.format.date(new Date($(this).attr('SalesOrderDate')), 'MMM d, yyyy h:mm a');

                                sessionStorage.DeliveryDate = '';
                                sessionStorage.DeliveryDate = $.format.date(new Date($(this).attr('DeliveryDate')), 'MMM d, yyyy h:mm a');


                                selectedCustomerID = '';
                                selectedCustomerID = $(this).attr('CustomerID');

                                selectedCustomerName = '';
                                selectedCustomerName = $(this).attr('CustomerName');

                                selectedFowstID = '';
                                selectedFowstID = $(this).attr('FowstID');

                                selectedTotalAmount = '';
                                selectedTotalAmount = AccountingFormat($(this).attr('Total'));


                                selectedSKUCount = '';
                                selectedSKUCount = $(this).attr('SkuCount');

                                selectedSalesOrderNo = '';
                                selectedSalesOrderNo = $(this).attr('SalesOrderNo');

                                selectedDeliveryAddress = '';
                                selectedDeliveryAddress = $(this).attr('Address');

                                selectedBillingAddress = '';
                                selectedBillingAddress = $(this).attr('BillingAddress');

                                selectedSalesRep = '';
                                sessionStorage.SalesRepID = '';


                                if (CStr2($(this).attr('SalesRepresentative')) == '') {
                                    selectedSalesRep = $(this).attr('UserName');
                                    sessionStorage.SalesRepID = $(this).attr('UserID');
                                } else {
                                    selectedSalesRep = $(this).attr('SalesRepresentative');
                                }

                                selectedPrintCount = 0;
                                selectedDownloadCount = 0;


                                actualStatus = CStr2($(this).attr('Status'));

                                selectedStatus = '';
                                selectedStatus = CStr2($(this).attr('Status')) == '' ? 'Viewed' : $(this).attr('Status');




                                //                                selectedStatusID = '';
                                //                                selectedStatusID = CStr2($(this).attr('StatusID'));

                                ShowPreview();
                                //                                } else {
                                //                                    MessageBox('No ordered quantity', 'Invalid Order');
                                //                                }


                            })

                pLV.append(pItem)

            }

            if (!proj.IsBootstrapDesign()) {
                pLV.parent().table("refresh");
            }
        }

        function ShowPreview() {
            
            actualStatus = CStr2(actualStatus);
            //if (actualStatus == '' 
            //    || actualStatus.indexOf('New') != -1
            //    || actualStatus == 'Created') {

            //}
            proj.UpdateOrderStatus();

            sessionStorage.CartOrderID = selectedInvoiceID;
            sessionStorage.FowstID = selectedFowstID;
            sessionStorage.CustomerOrganizationID = selectedCustomerID;

            var pData = {};

            pData.CustomerID = selectedCustomerID;
            pData.OrderID = selectedInvoiceID;
            pProductPopup.Model.Data.OrderID = pData.OrderID;
            pProductPopup.Model.Data.NewCartMode = false;
            pData.FowstID = selectedFowstID;
            //additional by janine sept 5 2014 for sales order taker
            pData.PosMode = proj.PosMode ? "pos" : "so";
            //end of additional
            pData.IncludeZeroQty = 1; // 2017_04_20__11_15_49_1734 zeroed order qty: once the order qty has been zeroed out, it is no longer possible for the product to be viewed on the PO dashboard.
            AjaxSend({
                url: 'SalesMenu/PosCart',
                data: pData,
                success: function (vData) {

                    mFinalized = IsGuid(vData.Master.FinalizePickListByID);
                    $('#MainNav1a').css('display', 'block');
                    $('#MainNav1b').css('display', 'none');
                    $('#MainNav1c').css('display', 'none');
                    $('#MainNav1d').css('display', 'none');

                    $('#navViewOrders').addClass('ui-navbar-btn-active');
                    $('#navViewOrders').addClass('ui-btn-active');

                    $('#btnSignature').removeClass('ui-navbar-btn-active');
                    $('#btnSignature').removeClass('ui-btn-active');

                    $('#btnInvoice').removeClass('ui-navbar-btn-active');
                    $('#btnInvoice').removeClass('ui-btn-active');

                    $('#btnStatusLog').removeClass('ui-navbar-btn-active');
                    $('#btnStatusLog').removeClass('ui-btn-active');

                    //                    $.mobile.changePage('#pageOrderStatus');
                    $('#pageOrderStatus').show();

                    if (proj.IsBootstrapDesign()) {
                        $('#popupOrderStatus-bs').modal('show');
                    } else {
                        $('#popupOrderStatus').show();
                    }
                    
                    //$('#pageOrderCentral').hide();

                    ////                    $('#divSalesReplenishmentProcessing').show();

                    ////                    $('.ItemsBreadCrumbs').text('');
                    ////                    breadcrumbs.prepend(arrowR, 'Order Details');
                    ////                    breadcrumbs.prepend(arrowR, '<a id="btnBCStoreReplenishmentProcessing">Store Replenishment Processing & Analytics</a>');
                    ////                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                    ////                    BreadCrumbsClickEvent();

                    //////                    TempValueBtnBack = "StoreReplenish";

                    //                    $('#divMainDashboard, #divSalesAnalytics,' +
                    //                    '#divOnPrintPOSSalesReport,' +
                    //                    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings').hide();

                    //                    $('#divSalesReplenishmentProcessing').show();

                    //$('.ItemsBreadCrumbs').text('');
                    //breadcrumbs.prepend(arrowR, 'Order Details');
                    //breadcrumbs.prepend(arrowR, '<a id="btnSalesOrderProcessingAnalyticsPage">Sales Order Processing & Analytics</a>');
                    //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                    //BreadCrumbsClickEvent();
                    
                    var pTotal = 0;
                    var pDR;
                    for (var p = 0; p <= vData.Units.Rows.length - 1; p++) {
                        pDR = vData.Units.Rows[p];
                        pTotal += CDec2(pDR.Quantity) * CDec2(pDR.UnitPrice);
                    }
                    if (vData.Master.TransType == 'PurchaseOrder'
                        || (CInt2(vData.Master.PurchaseOrderNo) > 0))
                        selectedCustomerName = vData.Master.OrgName;
                    selectedCustomerName = vData.Master.CustomerName;
                    selectedSKUCount = vData.Stocks.Rows.length;
                    if (CStr2(selectedSalesOrderNo) == '')
                        selectedSalesOrderNo = 'SO ' + vData.Master.MIN + '-' + vData.Master.SalesOrderNo;
                    if (CStr2(selectedDeliveryAddress) == '')
                        selectedDeliveryAddress = vData.Master.DeliveryAddress;
                    if (CStr2(selectedBillingAddress) == '')
                        selectedBillingAddress = vData.Master.BillingAddress;
                    if (CStr2(selectedSalesRep) == '')
                        selectedSalesRep = vData.Master.SalesRep;
                    sessionStorage.DeliveryDate = vData.Master.DeliveryDate;
                    DisplayInvoice(vData.Stocks.Rows, vData.Units.Rows, sessionStorage.SalesOrderDate, selectedCustomerName, pTotal, selectedSKUCount, selectedSalesOrderNo, selectedDeliveryAddress, selectedBillingAddress, selectedSalesRep, sessionStorage.DeliveryDate, vData.Master);

                    GetUpdatedStatus();

                    // commented by Arvin 2017_04_11__18_50_10_9064
                    //if (ReadyGuid(vData.Master.FowstID) != ReadyGuid(sessionStorage.PoServerFowstID))
                    //    BrowseFowst();
                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }

        proj.ShowFulfillmentCenterTransaction = function(vParams) {
            selectedCustomerID = vParams.CustomerID;
            selectedInvoiceID = vParams.InvoiceID;
            selectedFowstID = vParams.FowstID;
            ShowPreview();
        };

        function BreadCrumbsClickEvent() {
            if (sessionStorage.IsShowFirstPage == 'false') {
                $('#btnMainDashboard').click(function () {

                    $('#divSalesAnalytics, .btnPeriod, .lblPeriod').fadeIn();
                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                    $('#divMainDashboard, #divOnPrintPOSSalesReport,' +
                    ' #divOrderCentral,' +
                    '#divSpecialReports, #divAuditTrail,' +
                    '#divSettings, .divSearchFilterDateRange,#panTransactionDetails').hide();
                    $('#divFranchiseRequest').hide();
                    $('#divSalesReplenishmentProcessing').hide();
                    $('#divSalesOrderProcessing').hide();
                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                    TempValueBtnBack = null;
                    sessionStorage.SearchMode = 'false';
                    GetData();
                })
            }
            else {
                $('#btnMainDashboard').click(function () {

                    $('#divMainDashboard, .btnPeriod, .lblPeriod').fadeIn();
                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                    $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                        ' #divOrderCentral,' +
                        '#divSpecialReports, #divAuditTrail,' +
                        '#divSettings, .divSearchFilterDateRange,#panTransactionDetails').hide();
                    $('#divFranchiseRequest').hide();
                    $('#divSalesReplenishmentProcessing').hide();
                    $('#divSalesOrderProcessing').hide();
                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                    TempValueBtnBack = null;
                    sessionStorage.SearchMode = 'false';
                    GetData();
                })
            }

            //            $('#btnMainDashboard').click(function () {
            //                $('#divMainDashboard').fadeIn();
            //                $('#pageOrderStatus').hide();
            //                $('#pageOrderCentral').hide();



            //                $('.ItemsBreadCrumbs').text('Main Dashboard');
            //            });

            $('#btnSalesOrderProcessingAnalyticsPage').click(function () {
                //$('#pageOrderStatus').hide();
                //$('#pageOrderCentral').show();

                //$('.ItemsBreadCrumbs').text('');
                //breadcrumbs.prepend(arrowR, 'Sales Order Processing & Analytics');
                //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                //BreadCrumbsClickEvent();

                //TempValueBtnBack = "BackToDashboard";
                GetSalesOrderReport('');
            });
        }

        function GetSalesOrderDetails() {
            var pUrl;

            AjaxSend({
                url: 'DashboardsAndReports/GetRawSalesOrderData',
                data: { FowstID: selectedFowstID, InvoiceID: selectedInvoiceID, SalesOrderGroupNo: '' },
                success: function (vData) {
                    if (vData.SalesOrderDetail == "") {

                        MessageBox("No record(s) found.",
                                "Sales Order Data",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    } else {
                        pUrl = MergeUrlParameters({
                            SourceFileName: $.trim(vData.SalesOrderDetail),
                            DestinationFilename: $('#lblOrderNo').text()
                        }, '/DashboardsAndReports/DownloadSalesOrderCsvFile');
                        window.open(pUrl, '_blank');


                        selectedStatus = '';
                        selectedStatus = 'Downloaded';

                        selectedDownloadCount = selectedDownloadCount + 1;
                        proj.UpdateOrderStatus();

                    }

                }
            });



        }



        proj.UpdateOrderStatus = function () {
            var pData = {};

            pData.OrderID = selectedInvoiceID;
            pData.DownloadCount = selectedDownloadCount;
            pData.PrintCount = selectedPrintCount;
            pData.Status = selectedStatus;
            pData.StatusID = selectedStatusID;
            AjaxSend({
                url: 'DashBoardsAndReports/UpdateOrderStatus',
                data: pData,
                success: function (vData) {

                                    GetSalesOrderReport(lastSearchString);
                    //                    $("#lblColumnName").text('Creation Date');


                    GetUpdatedStatus();
                },
                error: function (vError) {
                    var pDummy;
                }
            }); // UpdateOrderStatus
        }; // proj.UpdateOrderStatus

        function GetUpdatedStatus() {

            $('#lblDownloadCount').text(selectedDownloadCount);
            $('#lblPrintCount').text(selectedPrintCount);


            AjaxSend({
                url: 'DashBoardsAndReports/GetUpdatedStatus',
                data: { InvoiceID: selectedInvoiceID },
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        if (vData.StatusCount.Rows.length > 0) {
                            for (var i = 0; i < vData.StatusCount.Rows.length; i++) {
                                if (vData.StatusCount.Rows[i].Status == 'Downloaded') {

                                    selectedDownloadCount = CDec2(vData.StatusCount.Rows[i].Count);
                                    $('#lblDownloadCount').text(selectedDownloadCount);
                                }

                                if (vData.StatusCount.Rows[i].Status == 'Printed') {

                                    selectedPrintCount = CDec2(vData.StatusCount.Rows[i].Count);
                                    $('#lblPrintCount').text(selectedPrintCount);
                                }



                            }

                        }

                        DisplayStatus(vData.Status.Rows);

                    }

                },
                error: function (vError) {
                    var pDummy;
                }
            }); // 

        }
        function GetSignature() {

            AjaxSend({
                url: 'SalesMenu/GetSalesOrderSignature',
                data: { OrgID: sessionStorage.OrgID, Filename: selectedInvoiceID },
                success: function (vData) {

                    if (vData.Error == undefined) {



                        document.getElementById('imgSignature').src = vData.ImageURL;



                        $('#lblSignatory').text(vData.SignatoryName);

                    }


                },
                error: function (vError) {
                    var pDummy;
                }
            }); // 

        }
        function DisplayInvoice(vRowsStocks, vRowsUnits, invoiceDate, clientName, totalValue, skuCount, orderNo, deliveryaddress, billingaddress, salesrep, deliveryDate, vInvoiceData) {

            var pTotalOrderedQty = 0;
            var pTotalServedQty = 0;
            totalValue = 0; // dont use the parameter value
            var pLV = $('#tblInvoiceBody');
            skuCount = 0;

            pLV.html('');
            $('#divRemarksLabel').show();
            $('#divRemarksTextbox').hide();
            $('#headerEncodingFowst').text(vInvoiceData.EncodingFowstName);
            $('#headerFowst').text(vInvoiceData.OrgFowstName);
            $('#headerEncodedBy').text(CStr2(vInvoiceData.CreatedbyName));
            $('#headerDateCreated').text(FormatDate(vInvoiceData.DateCreated, {
                DontDisplay12Am: true,
                Format: 'MMM dd, yyyy hh:mm a',
                FormatNoTime: 'MMM dd, yyyy'
            }));
            $('#lblOrderNo').text(orderNo);
            $('#headerSalesRep').text(salesrep);
            var pOrderDate;
            if (vInvoiceData.TransType == 'PurchaseOrder'
                || (CInt2(vInvoiceData.PurchaseOrderNo) > 0))
                pOrderDate = vInvoiceData.PurchaseOrderDate;
            else
                pOrderDate = vInvoiceData.SalesOrderDate;
            $('#headerInvoiceDate').text(FormatDate(pOrderDate, {
                DontDisplay12Am: true,
                Format: 'MMM dd, yyyy hh:mm a',
                FormatNoTime: 'MMM dd, yyyy'
            }));
            $('#headerDateNeeded').text(FormatDate(vInvoiceData.DateNeeded, {
                DontDisplay12Am: true,
                Format: 'MMM dd, yyyy hh:mm a',
                FormatNoTime: 'MMM dd, yyyy'
            }));
            $('#headerDeliveryDate').text(deliveryDate);
            $('#headerClientName').text(clientName);
            $('#headerClientDeliveryAddress').text(vInvoiceData.DeliveryFowstName + ' - ' + deliveryaddress);
            $('#headerClientBillingAddress').text(billingaddress);

            $('#lblPrintCount').text(selectedPrintCount);
            $('#lblDownloadCount').text(selectedDownloadCount);
            $('#lblRemarks').text(vInvoiceData.Remarks);
            $('#txtRemarks').text(vInvoiceData.Remarks);
            $('#lblRemarks').text(vInvoiceData.Remarks);
            $('#lblApprovedBy').text(vInvoiceData.SopaApproverName);
            $('#lblDucSoNum').text(vInvoiceData.DucSoNum);
            if (CStr2(vInvoiceData.DucSoNum) == '')
                $('#divDucSoNum').hide();
            else
                $('#divDucSoNum').show();

            //if (CStr2(vInvoiceData.SopaApproverName).trim() == '')
            //    $('#butApprove').parent().show();
            //else
            //    $('#butApprove').parent().hide();
            $('#headerCustomerFowst').text(vInvoiceData.CustomerFowst);
            $('#headerCustomerBin').text(CStr2(vInvoiceData.CustomerBin));
            var pShowRemovedItems = $('#chkShowRemovedItems').prop('checked');
            for (var i = 0; i <= vRowsUnits.length - 1; i++) {
                var pDrUnit = vRowsUnits[i];
                if ((CDec2(pDrUnit.Quantity) == 0)
                    && (CDec2(pDrUnit.HiddenServedQuantity) > 0))
                    pDrUnit.Quantity = pDrUnit.HiddenServedQuantity;
                if (!pShowRemovedItems) {
                    if (CDec2(pDrUnit.HiddenServedQuantity) > 0)
                        continue;
                }
                skuCount ++;
                for (var j = 0; j <= vRowsStocks.length - 1; j++) {
                    var pMatch = false;
                    if (pDrUnit.ProductID != vRowsStocks[j].ProductID)
                        continue;
                    if (pDrUnit.VendorID != vRowsStocks[j].VendorID)
                        continue;
                    if (pDrUnit.ProductID == vRowsStocks[j].ProductID) {
                        var pServedQty = CDec2(vRowsUnits[i].Quantity);
                        if (!CBool(vRowsUnits[i].ServedQtyIsEdited))
                            pServedQty = CDec2(vRowsUnits[i].OrderedQuantity);
                        pTotalOrderedQty += CDec2(vRowsUnits[i].OrderedQuantity);
                        pTotalServedQty += pServedQty;
                        var pLineTotal = vRowsUnits[i].UnitPrice * pServedQty;
                        totalValue += pLineTotal;
                        var pPurchaseOrderStatus = CStr2(pDrUnit.PurchaseOrderStatus);
                        if (CBool(pDrUnit.PurchaseOrderServedByVendor))
                            pPurchaseOrderStatus = 'Finalized';
                        var pItem = $('<tr style="cursor: pointer;">')
                            .data('data', vRowsStocks[j])
                            .append($('<td>')
                                .text(vRowsStocks[j].ProductNo)
                            )
                            .append($('<td>')
                                .text(vRowsStocks[j].ProductName)
                            )
                            .append($('<td>')
                                .text(CStr2(pDrUnit.VendorName2))
                            )
                            .append($('<td>')
                                .text(pPurchaseOrderStatus)
                            )
                            .append($('<td style="text-align: center">')
                                 .html(CDec2(vRowsUnits[i].OrderedQuantity) > 0 ? $.format.number(CDec2(vRowsUnits[i].OrderedQuantity), '#,###') : '<font color="red">-</font>')
                                 // Order Qty is read-only 2017_04_25__14_58_56_1456
                                //.click(function(e) {
                                //    Qty_Click.call(this, e, 'order');
                                //    return false;
                                //}) // END .click(function(e) {
                            )
                            .append($('<td style="text-align: center">')
                                 .html(CDec2(pServedQty) > 0 ? $.format.number(pServedQty, '#,###') : '<font color="red">-</font>')
                                 
                            )
                            .append($('<td>')
                                .text(vRowsUnits[i].UnitMeasure)
                            )
                            //.append($('<td style="text-align: right">')
                            //    .text(AccountingFormat(vRowsUnits[i].UnitPrice))
                            //)
                            //.append($('<td style="text-align: right">')
                            //    .text(AccountingFormat(pLineTotal))
                            //)
                            ;
                        if (CDec2(pDrUnit.HiddenServedQuantity) == 0)
                            pItem.append($('<td style="width:50px;">').append($('<button data-role="none" class="btn custom-btn-theme-d btn-danger btn-sm btn-block">')
                                .append('<span class="bi bi-x-lg"></span>')
                                .click(function (e) {
                                    var pLink = $(this);
                                    var pInvoiceItemID = pLink.closest('tr').data('ID');
                                    HandleOnHideItem(pInvoiceItemID);
                                    return false;
                                })
                            ));
                        else
                            pItem.append($('<td style="width:50px;">').append($('<button data-role="none" class="btn custom-btn-theme-d btn-danger btn-sm btn-block">')
                                .append('<span class="bi bi-x-lg"></span>')
                                .click(function (e) {
                                    if (!HandleEditingSavedAsFinal())
                                        return;

                                    var pLink = $(this);
                                    NewMessageBox('Are you sure you want to re-add this line item to the cart?', 'Re-add',
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                var pInvoiceItemID = pLink.closest('tr').data('ID');
                                                AjaxSend({
                                                    url: '/SalesMenu/ReaddHideServedQuantity',
                                                    data: {
                                                        InvoiceItemID: pInvoiceItemID
                                                    },
                                                    success: function (vData) {
                                                        if (vData.Result == 'ok') {
                                                            WebToast('Line item was successfully readded to cart.', 'top');
                                                        } else {
                                                            WebToast('Failed to readd line item.', 'top');
                                                        }
                                                        ShowPreview();
                                                    }
                                                });
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                    ], {
                                        Close: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#popupOrderStatus-bs').modal('show');
                                            }
                                        }
                                    });
                                    return false;
                                })
                            ));
                        if (CDec2(pDrUnit.HiddenServedQuantity) > 0)
                            pItem.css('background-color', '#f5d8d8');
                        pItem.data('ID', vRowsUnits[i].ID)
                            .attr('ProductID', vRowsUnits[i].ProductID)
                            .attr('Quantity', vRowsUnits[i].Quantity)
                            .attr('OrderedQuantity', pServedQty)
                            //.click(function(e) {
                            //    e.preventDefault();
                            //    NewMessageBox('Please click the "Order Qty" or "Served Qty" value to edit quantity.');
                            //    return false;
                            //});
                            .click(function(e) {
                                Qty_Click.call(this, e, 'serve');
                                return false;
                            }) // END .click(function(e) {
                        pLV.append(pItem);
                    }
                }
            }

            if (!proj.IsBootstrapDesign()) {
                pLV.parent().table("refresh");
            }

            $('#lblTotalOrderedQty').text('Total ordered qty. (in basic unit): ' + $.format.number(pTotalOrderedQty, '#,###.##'));
            $('#lblTotalServedQty').text('Total served qty. (in basic unit): ' + $.format.number(pTotalServedQty, '#,###.##'));
            $('#lblSKUCount').text('No. of SKU: ' + skuCount);
            $('#lblTotal').text('Total Amount Due: ' + AccountingFormat(totalValue));
        }

        function HandleOnHideItem(vInvoiceItemID, vParams) {

            if (!HandleEditingSavedAsFinal())
                return;

            vParams = $.extend({
                success: function(){}
            }, vParams);
            NewMessageBox('Are you sure you want to remove this line item from the cart?', 'Remove',
            [
                {
                    Text: 'Yes',
                    Click: function() {
                        console.log('jsP92C9BSUTZ! Are you sure you want to remove this line item from the cart? Yes');
                        AjaxSend({
                            url: '/SalesMenu/HideServedQuantity',
                            data: {
                                InvoiceItemID: vInvoiceItemID
                            },
                            success: function(vData) {
                                if (vData.Result == 'ok') {
                                    // NewMessageBox('Line item was successfully hidden.'); // hide "success" result dialog 2018_03_02__09_19_11_5219
                                } else {
                                    NewMessageBox('Failed to hide line item.');
                                }
                                ShowPreview();
                                vParams.success();
                            }
                        });
                    }
                },
                {
                    Text: 'No'
                }
            ], {
                Close: function () {
                    if (proj.IsBootstrapDesign()) {
                        $('#popupOrderStatus-bs').modal('show');
                    }
                }
            });
        }

        function Qty_Click(e, vMode) {
            var pAttr;
            var pLabel;

            if (!HandleEditingSavedAsFinal())
                return;

            if (vMode == 'order') {
                pAttr = 'OrderedQuantity';
                pLabel = 'Order Qty:';
            } else if (vMode == 'serve') {
                pAttr = 'Quantity';
                pLabel = 'Served Qty:';
            } else {
            }
                
            e.preventDefault();
            mSelectedTd = $(this);
            mSelectedTr = mSelectedTd.closest('tr');
            sessionStorage.AllowableQty = mSelectedTr.attr(pAttr);
            proj.ProductID = mSelectedTr.attr('ProductID');
            mSelectedQtyMode = vMode;
            if (proj.ProductID != '') {
                mSelectedInvoiceItemID = mSelectedTr.data('ID');
                selectedStatus = 'Modified';

                //window.NewStockBrowser = true;
                //startclock('#clockStock');
                //window.pageSourcePage = $.mobile.activePage;
                ////                                    ChangePage('/Stock?ref=SalesOrder');

                //proj.SalesMenuPage = $.mobile.activePage;
                //ChangePage('/DashBoardsAndReports/SalesOrderStockViewer'); // commented by Arvin 2017_03_21__10_39_00_2264

                pProductPopup.Model.Data.TransType = 'Invoice';
                pProductPopup.Model.Data.DontSaveTransType = 1;
                pProductPopup.Model.Data.ForceUpdateOrderedQty = 1;
                pProductPopup.Model.Data.ServedQtyIsEdited = 1;
                //pProductPopup.Model.Data.DontUpdateOrderedQty = 1;
                //pProductPopup.AfterSave = AfterProductSave;

                pProductPopup.popup({
                    //InventoryBalance: mInventoryBalance,
                    ProductID: proj.ProductID,
                    ProductName: mSelectedTr.data('data').ProductName,
                    ProductNo: mSelectedTr.data('data').ProductNo,
                    CustomerID: selectedCustomerID,
                    TransType: 'Invoice',
                    HidePrices: true,
                    ParentController: {
                        TransType: 'Invoice',
                        IsPos: false,
                        AfterProductSave: function () {
                           ShowPreview();
                        }
                    },
                    DeleteExistingProduct: 0,
                    //FromSalesOrder: mFromSalesOrder,
                    //MaxPaymentTerm: mMaxPaymentTerm,
                    //TargetOrgType: mTargetOrgType,
                    Success: function () {
                    }
                });

                return;

                $('#popProductQtyEdit, #popProductQtyEdit-bs').find('[name=lblQty]').text(pLabel);
                $('#popProductQtyEdit, #popProductQtyEdit-bs').find('[name=txtQtyEdit]').val($.format.number(CDec2(mSelectedTr.attr(pAttr)), '#,###.##'));
                if (proj.IsBootstrapDesign()) {
                    $('#popProductQtyEdit-bs').modal('show');
                } else {
                    $('#popProductQtyEdit').popup('open');
                }
            } 
            return false;
        }

        function DisplayStatus(vRows) {


            var pLV = $('#tblStatusTable');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                var pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].LastUpdated)
                            )
                            .append($('<td>')
                                .text(vRows[i].Status)
                            )
                            .append($('<td>')
                                .text(vRows[i].UserName)
                            )

                pLV.append(pItem)
            }
            try {
                if (!proj.IsBootstrapDesign()) {
                    pLV.parent().table("refresh");
                }
            } catch (ex) {
            }
        }











        //stupid table js


        // Helper function to convert a string of the form "Mar 15, 1987" into
        // a Date object.
        var date_from_string = function (str) {
            var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul",
                          "aug", "sep", "oct", "nov", "dec"];
            var pattern = "^([a-zA-Z]{3})\\s*(\\d{2}),\\s*(\\d{4})$";
            var re = new RegExp(pattern);
            var DateParts = re.exec(str).slice(1);

            var Year = DateParts[2];
            var Month = $.inArray(DateParts[0].toLowerCase(), months);
            var Day = DateParts[1];
            return new Date(Year, Month, Day);
        }

        var moveBlanks = function (a, b) {

            if (a < b) {
                if (a == "")
                    return 1;
                else
                    return -1;
            }
            if (a > b) {
                if (b == "")
                    return -1;
                else
                    return 1;
            }
            return 0;
        };
        var moveBlanksDesc = function (a, b) {
            // Blanks are by definition the smallest value, so we don't have to
            // worry about them here
            if (a < b)
                return 1;
            if (a > b)
                return -1;
            return 0;
        };

        var table = $("table").stupidtable({

            "date": function (a, b) {
                // Get these into date objects for comparison.

                aDate = new Date(a);
                bDate = new Date(b);

                return aDate - bDate;
            },
            "moveBlanks": moveBlanks,
            "moveBlanksDesc": moveBlanksDesc
        });

        table.on("beforetablesort", function (event, data) {
            // data.column - the index of the column sorted after a click
            // data.direction - the sorting direction (either asc or desc)
            $("#msg").text("Sorting index " + data.column)
        });

        table.on("aftertablesort", function (event, data) {

            var th = $(this).find("th");
            th.find(".arrow").remove();
            var dir = $.fn.stupidtable.dir;

            var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
            th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');



            switch (data.column) {
                case 1:
                    $("#lblColumnName").text('Creation Date');
                    break;
                case 2:
                    $("#lblColumnName").text('Order No.');
                    break;
                case 3:
                    $("#lblColumnName").text('Customer');
                    break;
                case 4:
                    $("#lblColumnName").text('Sales Representative');
                    break;
                case 5:
                    $("#lblColumnName").text('Delivery Address');
                    break;
                case 6:
                    $("#lblColumnName").text('Status');
                    break;
                default:
                    $("#lblColumnName").text('Creation Date');
                    break;
            }



        });

        $("tr").slice(1).click(function () {

            $(".awesome").removeClass("awesome");
            $(this).addClass("awesome");
        });


        //end of stupid table
    
        $('#butAddProduct').click(function () {
            sessionStorage.removeItem('AllowableQty');
            SearchProduct();            
        }); // END $('#butAddProduct').click(function() {

        function SearchProduct() {
            proj.SalesMenuPage = $.mobile.activePage;
            sessionStorage.IsInventoryCountEditSKUMode = false;
            window.IsBrowserloaded = false;
            BrowsePopup('LibraryPIMProduct', 'Search Product', [
                {
                    DisplayText: [
                        { Column: 'ProductName' },
                        {
                            Label: 'Status: ', Column: function (e) {
                                return (!CBool(e.IsDeactivated) ? 'Active' : 'Inactive')
                            }
                        },
                        //{ Label: 'Published by: ', Column: 'PublishedBy' },
                        //{ Label: 'Uploaded by:', Column: 'UploadedBy' }
                    ],
                    Click: function (e) {
                        OnClickSearchProduct(e);
                    }
                }
            ], {
                WithContentsOnly: (window.location.pathname.toLowerCase() == '/billofmaterials' ? true : false),
                IsSearchMore: 0
            })
            return;
            Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;

            if (window.location.pathname.toLowerCase() == '/billofmaterials')
                Browser.LibraryPIMProduct.InputData.WithContentsOnly = true; // debug only

            Browser.LibraryPIMProduct.Click = function (e, vOpt) {                
                vOpt.DontClose = true;
                OnClickSearchProduct(e);
            };
            Browser.LibraryPIMProduct.Show($('#txtSearch').val());

            $('#txtSearch').val('');

        }; // END function SearchProduct() {

        function OnClickSearchProduct(e) {
            proj.ProductID = e.ID;
            sessionStorage.IsDraft = 1;
            window.NewStockBrowser = true;
            startclock('#clockStock');

            proj.RebateData = null;
            proj.ModifierGroupID = null;
            if (!CBool(proj.POMode)
                && window.OfflineMode) {

                if (window.Business.LibraryItemModifiers.HasModifier(e.ID))
                    ItemModifier({
                        ProductName: e.ProductName,
                        ProductID: e.ID,
                        OnClickNoModification: function () {
                            ShowStockPage();
                        },
                        OnClickRebates: function () {
                            ItemModifier_Rebates();
                        },
                        OnClickRebatesDone: function (vData) {
                            if (vData != null) {
                                proj.RebateData = vData;
                                proj.ModifierGroupID = NewID();
                            }
                            ShowStockPage();
                        }
                    });
                else
                    ShowStockPage();
            } else
                ShowStockPage();

            function ShowStockPage() {
                pProductPopup.Model.Data.TransType = 'Invoice';
                pProductPopup.Model.Data.DontSaveTransType = 1;
                pProductPopup.Model.Data.ForceUpdateOrderedQty = 1;

                pProductPopup.popup({
                    ProductID: proj.ProductID,
                    ProductName: e.ProductName,
                    ProductNo: e.ProductNo,
                    CustomerID: selectedCustomerID,
                    TransType: 'Invoice',
                    HidePrices: true,
                    ParentController: {
                        TransType: 'Invoice',
                        IsPos: false,
                        AfterProductSave: function () {
                            ShowPreview();
                        }
                    },
                    Success: function () {
                    }
                });

                return;
                proj.ShowStockInfo();
                return
                ChangePage('/DashBoardsAndReports/SalesOrderStockViewer');
                //if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                //    ChangePage('../Stock');
                //else
                //    ChangePage('Stock');

                $('.lblPosBranch').text(sessionStorage.FowstName);
            } // END function ShowStockPage() {
        }

        proj.ShowCart = function (vDontFetchIfBlank) {
            $.mobile.changePage(proj.SalesMenuPage);
            proj.UpdateOrderStatus();
            ShowPreview();
            //sessionStorage.PosMode = 'Retail Mode';
            //startclock('#clock');
            //LoadCart(vDontFetchIfBlank);

            //if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
            //    window.pageHeader.LoadHotButtons();
            //else if (proj.SalesMenuPage != undefined) {
            //    sessionStorage.removeItem('IsHotButtonVisited');
            //    $.mobile.changePage(proj.SalesMenuPage); // dont use GetWindowPathFormat here
            //}

            //                    $('[data-href=Nav1d]').click();
        }; // proj.ShowCart = function() {

        $('#butChangeFowst').click(function(e) {
            e.preventDefault();
            BrowseFowst(true);
        });

        $('#butChangeRemarks').click(function(e) {
            if (!HandleEditingSavedAsFinal())
                return;
            e.preventDefault();
            $('#divRemarksLabel').hide();
            $('#divRemarksTextbox').show();
        });
        $('#divRemarksTextbox').hide();

        $('#butSaveRemarks').click(function(e) {
            e.preventDefault();

            var pUrl;
            var pData;

            pUrl = 'SalesMenu/SaveRemarksPos';
            pData = { 
                InvoiceID: selectedInvoiceID,
                Remarks: $('#txtRemarks').val()
            };
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function (vData) {
                    if (vData == undefined) return;
                    if (vData.Remarks == 'ok') {
                        WebToast('Remarks was successfully saved', 'top');
                        $('#lblRemarks').text($('#txtRemarks').val());
                        $('#divRemarksLabel').show();
                        $('#divRemarksTextbox').hide();
                    } else if (vData.Error != undefined) {
                        MessageBox(vData.Error, 'Error');
                    };
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        });

        function BrowseFowst(vSaveFowst) {
            if (!HandleEditingSavedAsFinal())
                return;

            BrowsePopup('LibraryFowst', 'Select FOWST', [
                {
                    AutoSearch: true,
                    Message: 'Please choose the FOWST (business location) that will serve this Purchase Order',
                    DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                    Click: function (e) {
                        selectedFowstID = e.ID;
                        sessionStorage.FowstID = e.ID;
                        sessionStorage.PoServerFowstID = e.ID;
                        $('#headerFowst').text(e.FowstName);
                        if (vSaveFowst)
                            AjaxSend({
                                url: '/SalesMenu/SaveFowst',
                                data: {
                                    InvoiceID: selectedInvoiceID,
                                    FowstID: selectedFowstID
                                },
                                success: function (vData) {
                                    setTimeout(function () {
                                        if (vData == undefined) return;
                                        if (vData.Result == 'ok') {
                                            WebToast('FOWST was successfully changed.', 'top');
                                        } else if (vData.Error != undefined) {
                                            WebToast(vData.Error, 'top');
                                        } else if (vData.Fail != undefined) {
                                            WebToast('Nothing was updated. ref: jsRYD6DEM4EE!', 'top');
                                        };
                                    }, 1000);
                                },
                                error: function (vError) {
                                    var pDummy;
                                }
                            });
                    }, Close: function () {
                        if (proj.IsBootstrapDesign()) {
                            $('#popupOrderStatus-bs').modal('show');
                        }
                    }
                }
            ], {
                OrganizationID: sessionStorage.OrgID,
                FilterFowst: 1
            })
        }

        $('#popProductQtyEdit, #popProductQtyEdit-bs').find('[name=butSave]').click(function () {
            SaveQty();            
        });

        $('#popProductQtyEdit, #popProductQtyEdit-bs').find('[name=txtQtyEdit]').keydown(function (e) {
            if (e.which == 13)
                SaveQty();            
        });

        function SaveQty() {
            var pQty = $('#popProductQtyEdit, #popProductQtyEdit-bs').find('[name=txtQtyEdit]').val();
            if (pQty == 0) {
                HandleOnHideItem(mSelectedInvoiceItemID, {
                    success: function () {
                        if (proj.IsBootstrapDesign()) {
                            $('#popProductQtyEdit-bs').modal('hide');
                        } else {
                            $('#popProductQtyEdit').popup('close');
                        }                        
                    }
                });
                return;
            }
            if (CDec2(pQty) < 0) {
                NewMessageBox('Quantity must be a valid number equal to or greater than zero.');
                return;
            }
            
            AjaxSend({
                url: 'SalesMenu/SaveQty',
                data: {
                    InvoiceItemID: mSelectedInvoiceItemID,
                    QtyMode: mSelectedQtyMode,
                    Value: pQty
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Result == 'ok') {
                        //mSelectedTr.attr('OrderedQuantity', pQty);
                        //mSelectedTd.text($.format.number(pQty, '#,###'));
                        ShowPreview();
                        // NewMessageBox('Quantity was successfully updated.'); // reduce the number of clicks daw ingon ni Marvin / Trail / Darlington; 2018_03_06__17_18_08_9690
                        if (proj.IsBootstrapDesign()) {
                            $('#popProductQtyEdit-bs').modal('hide');
                        } else {
                            $('#popProductQtyEdit').popup('close');
                        }
                    } else
                        NewMessageBox('Unknown server response. ref jsDLPM2S4VW4!');
                }
            }); // END AjaxSend({
        }

        function LoadSetting() {
            AjaxSend({
                url: 'SalesMenu/GetSettings',
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else {
                        $('#tabOFSettings [name=txtPickListPrintCount]').val(vData.PickListPrintCount);
                        $('#chkFcSettingExcludeVendorPoInFc')
                            .prop('checked', vData.FcSettingExcludeVendorPoInFc)
                            //.checkboxradio('refresh');
                    }
                }
            }); // END AjaxSend({
        }

        LoadSetting();

         $('#tabOFSettings [name=butSave]').click(function() {
            var pData = {
                PickListPrintCount: $('#tabOFSettings [name=txtPickListPrintCount]').val(),
                FcSettingExcludeVendorPoInFc: $('#chkFcSettingExcludeVendorPoInFc').prop('checked')
            };
            AjaxSend({
                url: 'SalesMenu/SaveSettings',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Result == 'ok') {
                        NewMessageBox('Settings were successfully saved.');
                    } else
                        NewMessageBox('Unknown server response. ref jsXRMAHHT56L!');
                }
            }); // END AjaxSend({
         });

        function ViewSopaPdf(vFilename) {
            var pUrl = MergeUrlParameters({
                Basefilename: vFilename
            }, '/DashboardsAndReports/GetSopaPdf');
            window.open(pUrl, '_blank',
                'width=' + window.innerWidth +
                ',height=' + window.innerHeight +
                ',scrollbars=auto');
            NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
        } // END function ViewReport() {

         $('#pageOrderStatus [name=butDownloadDocument]').click(function() {
            NewMessageBox('What do you want to download?', 'Download', [
                {
                    Text: 'Download CSV',
                    Click: function() {
                        GetSalesOrderDetails();
                    }
                },
                //{
                //    Text: 'Download PDF (Plain)',
                //    Click: function() {
                //        AjaxSend({
                //            url: '/DashboardsAndReports/CreateSopaPdf',
                //            data: {
                //                InvoiceID: selectedInvoiceID
                //            },
                //            success: function (vData) {
                //                if (vData.Error)
                //                    NewMessageBox(vData.Error);
                //                else
                //                    ViewSopaPdf(vData.Basefilename);
                //            }
                //        });
                //    }
                //},
                {
                    Text: 'Download PDF',
                    Click: function() {
                        CreatePdf()
                    }
                }
            ]);
            
         });

        // deletable
        //function CreatePdf() {
        //    AjaxSend({
        //        url: '/ERP/CreatePdf',
        //        data: {
        //            InvoiceID: selectedInvoiceID,
        //            PrintHash: localStorage.getItem('PrintHash'),
        //            TransType: 'SalesOrder',
        //            Template: 'WithdrawalSlip'
        //        },
        //        success: function(vData) {
        //            if (vData.Error) {
        //                if (vData.Error.indexOf('PrintHash') == -1)
        //                    NewMessageBox(vData.Error);
        //                else
        //                    NoPrintHash();
        //            } else if (vData.Result == 'ok') 
        //                NewMessageBox('Your document will be printed by your local print server.', '', null, {
        //                    Close: function() {
        //                    }
        //                });
        //        }
        //    });
        //}

        function NoPrintHash() {
            if (CStr2(localStorage.getItem('UUID')).length > 1)
                return;
            var p = GetUrlParameters();
            if (CBool(p.IsReadOnly))
                return;
            if (InAndroid)
                return;
            if (IsInVendorPortal())
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

        } // END function NoPrintHash()

        function HandleEditingSavedAsFinal() {
            
            if (mFinalized) {
                NewMessageBox('This transaction was saved as final. This transaction may not be edited.', 'Saved As Final');
                return false;
            }

            return true;
        }

        function CreatePdf(vParams) {
            // partial duplicate from \Scripts\sources\ERP\OrderProcessing\work\controller.js 
            vParams = $.extend({
                Template: null,
                ID: null
            }, vParams);

            var pInvoiceID = vParams.ID;
            AjaxSend({
                url: '/ERP/CreatePdf',
                data: {
                    InvoiceID: pInvoiceID,
                    PrintHash: localStorage.getItem('PrintHash'),
                    TransType: 'SalesOrder',
                    Template: vParams.Template
                },
                success: function(vData) {
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
                                            Click: function() {
                                                window.open('/Content/PrintApplet.zip', '_blank');
                                            }
                                        },
                                        {
                                            Text: 'Cancel'
                                        }
                                    ], {
                                        Close: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#popupOrderStatus-bs').modal('show');
                                            }
                                        }
                                    });
                            else
                                NewMessageBox(vData.Error, 'Error', null, {
                                    Close: function () {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#popupOrderStatus-bs').modal('show');
                                        }
                                    }
                                });
                        } else
                            NoPrintHash();
                    } else if (vData.Result == 'ok') 
                        NewMessageBox('Your document will be printed by your local print server.', '', null, {
                            Close: function() {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popupOrderStatus-bs').modal('show');
                                }

                                if (vParams.NewTransAfterPrint)
                                    NewTrans();
                            }
                        });
                }
            });
        }

        var mAudio;
        var mCanPlayAudio;

        function handlePlaySound() {

            if (mAudio == null) {
                mAudio = new Audio('/Content/Audio/promise.mp3');
                mAudio.addEventListener('canplaythrough', function() {
                    try {
                        mCanPlayAudio = true;
                        mAudio.play()
                    } catch (e) {
                        console.log(e);
                    }
                });
            } else if (mCanPlayAudio && mHasNewStatus)
                try {
                    mAudio.play()
                } catch (e) {
                    console.log(e);
                }

        }

        function HideColumns() {
            if (IsInVendorPortal()) {
                $('[name=hideInVendorPortal]').hide();
                var pTableRows = $('#table-custom-2 tr');
                ['d', 'h'].forEach(function(pChar) {
                    pTableRows.find('t' + pChar + ':nth-child(2)').hide();
                    pTableRows.find('t' + pChar + ':nth-child(7)').hide();
                    pTableRows.find('t' + pChar + ':nth-child(10)').hide();
                    pTableRows.find('t' + pChar + ':nth-child(11)').hide();
                    pTableRows.find('t' + pChar + ':nth-child(15)').hide();
                    pTableRows.find('t' + pChar + ':nth-child(19)').hide();
                });

                $('#divFcServingFowst').hide();
                $('#divFcDateOfFlush').hide();
                $('#divSopaDeliveryDate').hide();
                $('#divFcCustomerOutlet').hide();
                $('#divFcCustomerDepartment').hide();
                $('#divSopaApprovedBy').hide();

                // not columns
                
            }
        }

        return mDeferred.promise();

    });
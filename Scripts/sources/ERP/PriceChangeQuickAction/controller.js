define(['/Content/Ecommerce/js/lightbox-2.11.1.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceChangeQuickAction/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.AccountDetails.controller = ERP.AccountDetails.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.PriceChangeQuickAction.controller', null, function () {

        var mOut = {};
        var mElement;
        var pDataTable;

        mElement = $(mHtml);
        if (location.pathname.toLowerCase() != '/salesmenu/webpos' && location.pathname.toLowerCase() != '/salesmenu/webposnew') {
            var pAdjustments = $.mobile.activePage.find('#popupAdjustment');
            if (pAdjustments.length)
                mElement.insertBefore(pAdjustments);
            else
                $.mobile.activePage.append(mElement);
            if (proj.IsBootstrapDesign()) {
                if (!IsEcommerce) {
                    if (location.pathname.toLowerCase() == '/creditandcollection/onlinesoa') {
                        mElement = $('#popupAccountDetailsContent > *').appendTo('#divMAAccountsPayable');
                    } else {
                        $('#popupAccountDetailsContent > *').appendTo('#popupAccountDetailsContent-bs');
                        $('#popupAccountDetails').removeAttr('id');
                        $('#popupAccountDetails-bs').attr('id', 'popupAccountDetails');
                    }                    
                } else {
                    mElement = $('#popupAccountDetailsContent > *').appendTo('#divMAAccountsPayable');
                }
            }            
        }
        
        //var pCustomerPayment = $.mobile.activePage.find('#popupCustomerPayment');
        //if (pCustomerPayment.length)
        //    mElement.insertBefore(pCustomerPayment);
        //else
        //    $.mobile.activePage.append(mElement);
        mOut.element = mElement;
        mOut.Mode = null;
        mOut.CustomerName = null;
        mOut.CustomerID = null;
        mOut.FromEcommerce = false;
        mOut.FromWebPos = false;
        var tbodyInvoices = mElement.find('#tbodyInvoices');
        mElement.find('#cbInvoiceFilter').on('change', function() {
            ERP.AccountDetails.controller.GetCustomerCreditDetails(mOut.CustomerID, $('#cbInvoiceFilter').val());
        });
        mOut.GetCustomerCreditDetails = function(id, vFilter) {
            var pUrl;
            var pData = {
                Filter: vFilter
            };
            $('.lblCustomerName').text(CStr2(mOut.CustomerName));
            mOut.CustomerID = id;

            if (mOut.Mode == 'AP') {
                pUrl = '/CreditAndCollection/GetSupplierReceivablesDetails';
                pData.SupplierID = id;
                tbodyInvoices.find('[name=thStockReceivingNum]').show();
                tbodyInvoices.find('[name=tfootStockReceivingNum]').show();
            } else {
                pUrl = '/CreditAndCollection/GetCustomerCreditDetails';
                pData.CustomerID = id;
                tbodyInvoices.find('[name=thStockReceivingNum]').hide();
                tbodyInvoices.find('[name=tfootStockReceivingNum]').hide();
            }
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function (vData) {
                    var pCreditBegBalance = CDec2(GetStringObject('Data.CreditBegBalance', vData));
                    var pCreditTerm = CInt2(GetStringObject('Data.CreditTerm', vData));
                    var pCreditBalanceAsOf = CStr2(GetStringObject('Data.CreditBalanceAsOf', vData));
                    var pCreditLimit = CDec2(GetStringObject('Data.CreditLimit', vData));
                    var pAvailableCreditLimitComputation = CStr2(vData.AvailableCreditLimitComputation);
                    var pAvailableCredit = pCreditLimit - vData.NetInvoiceBalance;
                    if (pAvailableCreditLimitComputation == 'NoPdc')
                        pAvailableCredit -= CDec2(vData.CheckPaymentAmountPdc);
                    var vRows = vData.Results.Rows;
                    var pStr;

                    arrCreditDetails = vRows;
                    DisplayInvoices({
                        BeginningBalance: pCreditBegBalance,
                        Overpayment: vData.Overpayment
                    });
                    $('#lblBeginningBalance').text(AccountingFormat(pCreditBegBalance));
                    $('#lblBeginningBalanceAsOf').text(pCreditBalanceAsOf);
                    $('#lblUnappliedPayments').text(AccountingFormat(vData.Overpayment));
                    $('#lblExcessPayments').text(AccountingFormat(vData.ExcessPayments));
                    $('#lblTotalOfUnappliedCreditAndExcessPayments').text(AccountingFormat(CDec2(vData.Overpayment) + CDec2(vData.ExcessPayments)));

                    $('[name=spanCustomerCreditLimit]').text(AccountingFormat(pCreditLimit));
                    pStr = AccountingFormat(pCreditTerm, 0);
                    if (pCreditTerm == 1)
                        pStr += ' Day';
                    else if (pCreditTerm > 1)
                        pStr += ' Days';
                    $('[name=spanCustomerCreditTerms]').text(pStr);
                    pAvailableCredit = CDec2(pAvailableCredit);
                    pStr = $.format.number(Math.abs(pAvailableCredit), '#,##0.00');
                    if (pAvailableCredit > 0)
                        pStr += ' available credit';
                    else if (pAvailableCredit < 0)
                        pStr += ' over the credit limit';
                    $('[name=spanCustomerAvailableCredit]').text(pStr);
                    $('[name=spanCustomerAgingOfAr]').text(AccountingFormat(vData.AgingOfAr) + ' Days Overdue (based on Moving Weighted Average)');
                }
            });
        }

        var isTHAPPPendingApprovalDeleted = false, inxTHAPPPendingApproval;
        function DisplayInvoices(vData) {
                    
            var totalamount = 0;
            var pTotalNetAdjustments = 0;
            var pTotalNetInvoiceAmount = 0;
            var pTotalAccumPrevPaymentsPendingApproval = 0;
            var pTotalAccumPrevPayments = 0;
            var pNetInvoiceBalance = 0;
            var pTotaNetAdjustments = 0;
            var pWeightedAverageDaysOverdue = 0;
            var pWeightRelativeToTotalAr = 0;

            //tbodyInvoices.empty();
                    
            //francis H. 04-26-17
                    

            if ($.fn.DataTable.isDataTable(tbodyInvoices)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tbodyInvoices.empty();
                pDataTable.destroy();
            }

            var buttons = [];
            if (!IsMobile && !mOut.FromEcommerce) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'CAC of ' + mOut.CustomerName });
            }

            if (CBool(sessionStorage.IsSOALink)) {
                $('[name="thAPPPendingApproval"]').show();
            }

            var pColumns = [
                {
                    title: '',
                    'className': 'text-center',
                    'data': function (data, type, full, meta) {
                        return (CInt2(data.NoOfAttachments) == 0 ? '' : '<button data-role="none" class="btn btn-warning btn-sm" title="View Invoice Image"><span class="bi-paperclip"></span></button>'); // attachment cell
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:50px;');
                    }
                },
                {
                    title: 'Invoice Date',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.InvoiceDate, {
                            DontDisplay12Am: true
                        });
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    name: 'InvoiceNumber',
                    title: 'Invoice No.',
                    data: function(vData) {
                        return vData.InvoiceNumber;
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            if (mOut.Mode == 'AP') 
                pColumns.push(
                    {
                        title: 'Stock Receiving',
                        data: function(vData) {
                            return CStr2(vData.FullReceivingNum);
                        }
                    }
                );

            Array.prototype.push.apply(pColumns, 
            [
                {
                    name: 'GrossInvoiceAmount',
                    title: 'Gross Invoice Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.GrossInvoiceAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    name: 'AdjustmentAmount',
                    title: 'Net Adjustments',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.AdjustmentAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 5
                    name: 'NetInvoiceAmount',
                    title: 'Net Invoice  Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.NetInvoiceAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 6
                    name: 'TotalAPPPendingApproval',
                    title: 'Accum. Customer Payments Pending Approval',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.APPPendingApproval);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    name: 'TotalPayment',
                    title: 'Accum. Prev.  Payments',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.TotalPayment);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    name: 'NetInvoiceBalance',
                    title: 'Net Invoice  Balance',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.NetInvoiceBalance);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {   title: 'Invoice Terms',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.CreditTerms, 0);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:150px;');
                    }
                },
                {
                    title: 'Invoice Due Date',
                    'className': 'text-center',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.InvoiceDueDate);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'AR Age',
                    'className': 'text-center',
                    'data': function (data, type, full, meta) {
                        var pOut = AccountingFormat(data.ArAge, 0);
                        if (CDec2(data) > 0)
                            pOut = '<span style="color:red">' + pOut + '<span>';
                        return pOut;
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:80px;');
                    }
                },
                {
                    name: 'WeightRelativeToTotalAr',
                    title: 'Weight Of Transaction Relative to Total AR of Customer',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        var pOut = AccountingFormat(data.WeightRelativeToTotalAr) + '%';
                        return pOut;
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:300px;');
                    }
                },
                {
                    name: 'WeightedAverageDaysOverdue',
                    title: 'Weighted Days Overdue',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        var pOut = AccountingFormat(data.WeightedAverageDaysOverdue);
                        return pOut;
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {   
                    title: 'Last Modified',
                    'className': 'text-center',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.LastUpdated);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Remarks',
                    'className': 'text-center',
                    'data': function (data, type, full, meta) {
                        return CStr2(data.Remarks);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'PO No.',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return CStr2(data.PurchaseOrderText);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:100px;');
                    }
                },
                {
                    title: 'PO Date',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.PurchaseOrderDate, {
                            DontDisplay12Am: true
                        });
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:100px;');
                    }
                }                
            ]);

            if (mOut.Mode == 'AP') {
                pColumns = pColumns.concat([
                    {
                        title: 'Accounts Payable Ref No',
                        'className': 'dt-body-left',
                        'data': function (data, type, full, meta) {
                            return CStr2(data.ApNum);
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'min-width:200px;');
                        }
                    },
                    {
                        title: 'Accounts Payable Date',
                        'className': 'dt-body-left',
                        'data': function (data, type, full, meta) {
                            return FormatDate(data.ApDate, {
                                DontDisplay12Am: true
                            });
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'min-width:200px;');
                        }
                    },
                    {
                        title: 'Accounts Payable Status',
                        'className': 'dt-body-left',
                        'data': function (data, type, full, meta) {
                            return data.ApStatus + '';
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'min-width:200px;');
                        }
                    }
                ]);
            }

            tbodyInvoices.html('');
            var pFooter = $('<tfoot>');
            var pTrFooter = $('<tr>');            
            pFooter.append(pTrFooter);
            tbodyInvoices.append(pFooter);
            pColumns.every(function () {
                pTrFooter.append($('<th style="text-align:right;">'));
                return true;
            });

            pDataTable = tbodyInvoices.DataTable({
                "data": arrCreditDetails,
                order: [],
                "autoWidth": false,
                columns: pColumns,
                "language": {
                    "search": "Search Invoice "
                },
                sDom: '<"top"Bf>rt<"bottom"ip><"clear">',
                buttons: buttons,
                initComplete: function () {
                    tbodyInvoices.removeClass('dataTable').wrap('<div style="width:100%; overflow:auto;"></div>');
                    tbodyInvoices.find('thead tr').css({ 'font-size': '12px', 'vertical-align': 'top', 'text-align': 'center' });
                    
                    if (!CBool(sessionStorage.IsSOALink)) {
                        if (!isTHAPPPendingApprovalDeleted) {
                            isTHAPPPendingApprovalDeleted = true;
                            inxTHAPPPendingApproval = tbodyInvoices.find('th:contains("APP Pending Approval")').index();
                            tbodyInvoices.find('thead th:eq(' + inxTHAPPPendingApproval + ')').remove();
                        }

                        tbodyInvoices.find('td:eq(' + inxTHAPPPendingApproval + ')').remove();
                    }                    
                }
            });
            tbodyInvoices.addClass('table-stripe').css('width', '100%')
            tbodyInvoices.find('thead tr').addClass('custom-theme-f');

            for (var i = 0; i < arrCreditDetails.length; i++) {
                var pDR = arrCreditDetails[i];
                totalamount += CDec2(pDR.GrossInvoiceAmount);
                pTotaNetAdjustments += CDec2(pDR.AdjustmentAmount);
                pTotalNetInvoiceAmount += CDec2(pDR.NetInvoiceAmount);
                pTotalAccumPrevPaymentsPendingApproval += CDec2(pDR.APPPendingApproval);
                pTotalAccumPrevPayments += CDec2(pDR.TotalPayment);
                pNetInvoiceBalance += CDec2(pDR.NetInvoiceBalance);
                pWeightedAverageDaysOverdue += CDec2(pDR.WeightedAverageDaysOverdue);
                pWeightRelativeToTotalAr += CDec2(pDR.WeightRelativeToTotalAr);
            }

            $(pDataTable.column('InvoiceNumber:name').footer()).text(AccountingFormat(arrCreditDetails.length, 0) + ' records');
            $(pDataTable.column('GrossInvoiceAmount:name').footer()).text(AccountingFormat(totalamount));
            $(pDataTable.column('AdjustmentAmount:name').footer()).text(AccountingFormat(pTotalNetAdjustments));
            $(pDataTable.column('NetInvoiceAmount:name').footer()).text(AccountingFormat(pTotalNetInvoiceAmount));
            $(pDataTable.column('TotalAPPPendingApproval:name').footer()).text(AccountingFormat(pTotalAccumPrevPaymentsPendingApproval));
            $(pDataTable.column('TotalPayment:name').footer()).text(AccountingFormat(pTotalAccumPrevPayments + CDec2(vData.Overpayment)));
            $(pDataTable.column('NetInvoiceBalance:name').footer()).text(AccountingFormat(pNetInvoiceBalance));
            $(pDataTable.column('WeightedAverageDaysOverdue:name').footer()).text(AccountingFormat(pWeightedAverageDaysOverdue));
            $(pDataTable.column('WeightRelativeToTotalAr:name').footer()).text(AccountingFormat(pWeightRelativeToTotalAr));

            // footer 2nd row
            pTrFooter = $('<tr>');
            pFooter.append(pTrFooter);
            tbodyInvoices.append(pFooter);
            var pColIndex = pDataTable.column('NetInvoiceBalance:name').index();
            pTrFooter.append($('<th colspan=' + (pColIndex) + ' style="text-align:right;">').text('Net Invoice Total with Beginning Balance:'));
            pTrFooter.append($('<th name=TotalPayment2ndRow style="text-align:right;">'));
            tbodyInvoices.find('[name=TotalPayment2ndRow]').text(AccountingFormat(pNetInvoiceBalance  + CDec2(vData.BeginningBalance)));
            pTrFooter.append($('<th colspan=' + (pColumns.length - pColIndex - 1) + ' style="text-align:right;">').text(''));
        }

        function AccDetOptions(data) {
            var pButtons = [];
            var pText;

            if (proj.CacMode == 'AP')
                pText = 'Process Accounts Payable For This Transaction';
            else
                pText = 'View Document';
            pButtons.push({
                Text: pText,
                data: data,
                Click: function (e) {
                    var pID;
                    if (proj.CacMode == 'AP')
                        //pID = e.ID;
                        pID = e.AccountsPayableID;
                    else
                        pID = e.ID;

                    var pParams = {
                        IsReadOnly: 1,
                        ID: pID,
                        OrgID: sessionStorage.OrgID
                    };
                    if (proj.CacMode == 'AP')
                        pParams.ShowAccountsPayable = 1;
                    else
                        pParams.ShowInvoice = 1;

                    if (CBool(sessionStorage.IsSOALink)) {
                        pParams.IsSoaLink = 1
                    }

                    var pUrl = MergeUrlParameters(pParams, location.origin + (proj.IsBootstrapDesign() ? '/erp' : location.pathname));
                    var win = window.open(pUrl,
                        '_blank',
                        'width=' + screen.width + ',height=' + screen.height);

                    //win.onload = function () {
                    //    function LoadRecord() {
                    //        if (win.Z9Q3C9ASDA) {
                    //            var pData = {
                    //                ID: pID,
                    //                ReadOnly: true
                    //            };
                    //            if (proj.CacMode == 'AP') {
                    //                pData.AccountsPayable = true;
                    //                pData.ReadOnly = false;
                    //                win.Z9Q3C9ASDA.ShowAccountsPayable(pData);
                    //            } else
                    //                win.Z9Q3C9ASDA.ShowInvoice(pData);
                    //        } else
                    //            setTimeout(LoadRecord, 1000);
                    //    }
                    //    LoadRecord();
                    //}
                }
            });

            if (proj.CacMode == 'AP') {
                pButtons.push({
                    Text: 'View Stock Receiving Document',
                    data: data,
                    Click: function (e) {
                        var pID;
                        var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                            '_blank',
                            'width=' + window.innerWidth + ',height=' + window.innerHeight);
                                
                        if (proj.CacMode == 'AP')
                            pID = e.ID;
                        else
                            pID = e.ID;
                        win.onload = function () {
                            function LoadRecord() {
                                if (win.Z9Q3C9ASDA) {
                                    var pData = {
                                        ID: pID,
                                        ReadOnly: true
                                    };
                                    if (proj.CacMode == 'AP')
                                        win.Z9Q3C9ASDA.ShowStockReceiving(pData);
                                    else
                                        win.Z9Q3C9ASDA.ShowInvoice(pData);
                                } else
                                    setTimeout(LoadRecord, 1000);
                            }
                            LoadRecord();
                        }
                    }
                });
            } else {
            }

            
            pButtons.push({
                Text: 'Attach File',
                data: data,
                Click: function (e) {
                    PopupButtonList('', 'Options', [
                        {
                            Text: 'Upload image from computer',
                            Click: function () {
                                invoiceID = e.ID;
                                sessionStorage.CacInvoiceID = invoiceID;
                                $('#fileAttach').click();
                            }
                        }, {
                            Text: 'Take Invoice Image using computer camera',
                            Click: function () {
                                if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                                    NewMessageBox("getUserMedia() not supported.");
                                    return;
                                }

                                $('#popupInvoiceImageWebCam').modal('show');
                                $('#btnTakePhoto').text('Take Photo');
                                $('#btnUploadPhoto').attr('disabled', 1);

                                function StartCamera() {
                                    var video = document.querySelector('#campreview');
                                    var canvas = document.querySelector('#camcaptured');
                                    var isRetake = false, isforUpload = false;

                                    navigator.mediaDevices.getUserMedia({ audio: false, video: true })
                                        .then(function (stream) {
                                            if ("srcObject" in video) {
                                                video.srcObject = stream;
                                            } else {
                                                video.src = window.URL.createObjectURL(stream);
                                            }

                                            video.onloadedmetadata = function (e) {
                                                video.play();
                                            };

                                            $('#campreview').pageshow();

                                            $('#btnTakePhoto').unbind('click').click(function () {
                                                if (!isRetake) {
                                                    canvas.height = video.offsetHeight;
                                                    canvas.width = video.offsetWidth;
                                                    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);
                                                    StopCamera(stream);
                                                    $('#camcaptured').pageshow();
                                                    $(this).text('Retake');
                                                    $('#btnUploadPhoto').removeAttr('disabled');
                                                    isRetake = true;
                                                    isforUpload = true;
                                                } else {
                                                    $(this).text('Take Photo');
                                                    $('#btnTakePhoto').attr('disabled', 1);
                                                    isRetake = false;
                                                    isforUpload = false;
                                                    StartCamera();
                                                }
                                            })

                                            $('#btnUploadPhoto').unbind('click').click(function () {
                                                if (isforUpload) {
                                                    Bs.Loading('show', 'Uploading... please wait');
                                                    $.ajax({
                                                        url: "/CreditAndCollection/UploadInvoiceImageWebCam",
                                                        data: JSON.stringify({
                                                            OrgID: sessionStorage.OrgID,
                                                            InvID: e.ID,
                                                            imageData: canvas.toDataURL('image/png', 1).replace('data:image/png;base64,', '')
                                                        }),
                                                        type: 'POST',
                                                        async: false,
                                                        cache: false,
                                                        processData: false,
                                                        contentType: false,
                                                        success: function (e) {
                                                            var d = JSON.parse(e);
                                                            Bs.Loading('hide');
                                                            if (d.d.Results == 'saved') {
                                                                WebToast('Invoice image was successfully uploaded', 'top');
                                                                mOut.GetCustomerCreditDetails(mOut.CustomerID, 'has payable');
                                                                $('#popupInvoiceImageWebCam').modal('hide');
                                                            } else {
                                                                NewMessageBox(d.d.Results);
                                                            }
                                                        }
                                                    });
                                                }
                                            })

                                            $('#popupInvoiceImageWebCam').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                                StopCamera(stream);
                                            })
                                        })
                                        .catch(function (err) {
                                            console.log(err.name + ": " + err.message);
                                        });
                                }

                                function StopCamera(stream) {
                                    stream.getTracks().forEach(function (track) {
                                        if (track.readyState == 'live') {
                                            track.stop();
                                        }
                                    });
                                }

                                StartCamera();
                            }
                        }
                    ])                        
                }
            });
            pButtons.push({
                Text: 'Pay This Invoice',
                data: data,
                Click: function (e) {
                    function Load() {
                        var pData = {
                            InvoiceID: e.ID
                        };
                        if (mOut.Mode == 'AP')
                            pData.CustomerID = data.SupplierID;
                        else
                            pData.CustomerID = data.CustomerID;
                        ERP.Payment.controller.Mode = mOut.Mode;
                        ERP.Payment.controller.FromEcommerce = mOut.FromEcommerce;
                        ERP.Payment.controller.FromWebPos = mOut.FromWebPos;
                        ERP.Payment.controller.ClearValues();
                        ERP.Payment.controller.LoadPaymentInvoices(pData);
                        //mElement.hide();
                        if (proj.IsBootstrapDesign()) {
                            $('#popupCustomerPayment').modal('show').on('hidden.bs.modal', function () {
                                if (!IsEcommerce) {
                                    $('#popupCustomerPayment').unbind('hidden.bs.modal');
                                    //$('#popupAccountDetails').modal('show');  after daw masave, no need to show na daw ni na popup. ingon sir - Sep 1, 2021
                                }
                            });
                        } else {
                            $('#popupCustomerPayment').show();
                        }
                    }

                    if (mOut.FromEcommerce) {
                        Load()
                    } else {
                        PromptUserAccess('Payments (EAS)', {
                            OnClickOKSuccess: function () {
                                Load()
                            },
                            OnFailed: function () {
                                console.log('jsCFDTPD3GXC!');
                            }
                        });
                    }                    
                }
            });

            if (!mOut.FromEcommerce)
                pButtons.push({
                    Text: 'Enter Account Adjustments',
                    data: data,
                    Click: function (e) {
                        var pID;
                        if (proj.CacMode == 'AP')
                            pID = e.AccountsPayableID;
                        else
                            pID = e.ID;
                        var pAccess = 'Adjustment (EAS)';
                        if (CDec2(data.TotalPayment) > 0) 
                            pAccess = 'Adjustment to Invoices With Payments (EAS)';
                        PromptUserAccess(pAccess, {
                            OnClickOKSuccess: function () {                                                                                        
                                Cac.LoadAdjustments({
                                    ID: e.ID,
                                    InvoiceID: pID,
                                    InvoiceNumber: e.InvoiceNumber,
                                    InvoiceDate: e.InvoiceDate,
                                    GrossInvoiceAmount: e.GrossInvoiceAmount,
                                    NetInvoiceAmount: e.NetInvoiceAmount,
                                    NetInvoiceBalance: e.NetInvoiceBalance,
                                    SupplierID: mOut.CustomerID,
                                    AccountsPayableID: e.AccountsPayableID
                                });
                                //mElement.hide(); // commented on 2017_11_09__09_06_50_5349
                                $('#popupAdjustment').modal('show');
                            },
                            OnFailed: function () {
                                console.log('jsCFDTPD3GXC!');
                            }
                        });
                    }
                });
            PopupButtonList('', 'Action Menu', pButtons);
        }

        mOut.GetInvoiceFiles = GetInvoiceFiles;
        function GetInvoiceFiles(invoiceID) {
            AjaxSend({
                url: 'CreditAndCollection/GetInvoiceFiles',
                data: { InvoiceID: invoiceID },
                success: function (e) {
                    //Cac.ShowGallery(e.Results.Rows);
                    var vRows = e.Results.Rows;

                    if (vRows.length == 0) {
                        return;
                    }

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
        }

        $('body')
        .on('click', '#butPayments', function() {
            var pNeededAccess = 'Payments (EAS)';
            if (mOut.Mode == 'AP')
                pNeededAccess = 'Accounts Payable Verification > Access to Payments';
            if (mOut.FromEcommerce)
                Proceed();
            else
                PromptUserAccess(pNeededAccess, {
                    OnClickOKSuccess: function () {                                                                                        
                        Proceed();
                    },
                    OnFailed: function () {
                        console.log('jsCFDTPD3GXC!');
                    }
                });
            function Proceed() {
                ERP.Payment.controller.Mode = mOut.Mode;
                ERP.Payment.controller.FromEcommerce = mOut.FromEcommerce;
                ERP.Payment.controller.FromWebPos = mOut.FromWebPos;
                ERP.Payment.controller.CustomerName = mOut.CustomerName;
                ERP.Payment.controller.ClearValues();
                ERP.Payment.controller.LoadPaymentInvoices({
                    CustomerID: mOut.CustomerID
                });
                $('.lblCustomerName').text(CStr2(mOut.CustomerName));
                if (proj.IsBootstrapDesign()) {
                    $('#popupCustomerPayment').modal('show').on('hidden.bs.modal', function () {
                        if (!IsEcommerce) {
                            $('#popupCustomerPayment').unbind('hidden.bs.modal');
                            //$('#popupAccountDetails').modal('show'); after daw masave, no need to show na daw ni na popup. ingon sir - Sep 1, 2021
                        }                        
                    });
                } else {
                    //mElement.hide();
                    $('#popupCustomerPayment').show();
                }
            }
        })
        .on('click', '#tbodyInvoices tbody tr td', function () {

            invoiceID = pDataTable.row(this).data().ID;

            if ($(this).is(':first-child') && CInt2(pDataTable.row(this).data().NoOfAttachments) != 0) { // for ATTACHMENT COLUMN
                GetInvoiceFiles(invoiceID)
                return;
            }
            
            AccDetOptions(pDataTable.row(this).data());

            // $(this).data('invoicedetails').ID;
            //AccDetOptions($(this).data('invoicedetails'));
        });

        (function Init() {
            AjaxSend({
                ExactUrl: '/SalesMenu/FMCGPopup?cache=1&OrgID=' + sessionStorage.OrgID,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $(vData).hide();

                    if (location.pathname.toLowerCase() == '/salesmenu/webpos') {
                        $('#divWebPOS').append(vData).trigger('create');
                    } else if (location.pathname.toLowerCase() == '/salesmenu/webposnew' || proj.IsBootstrapDesign()) {
                        $.mobile.activePage.append(vData);

                        $('#popFMCGCashcontent > *').appendTo('#popFMCGCashcontent-bs');
                        $('#popFMCGChequeGCcontent > *').appendTo('#popFMCGChequeGCcontent-bs');
                        $('#popFinancialDetails > [data-role=content] > *').appendTo('#popFinancialDetailsContent-bs');
                        $('#popFMCGCardcontent > *').appendTo('#popFMCGCardcontent-bs');
                        $('#popFMCGSrvcontent > *').appendTo('#popFMCGSrvcontent-bs');
                        $('#popFMCGOtherPaymentcontent > *').appendTo('#popFMCGOtherPaymentcontent-bs');
                        $('#popFMCGDiscountcontent > *').appendTo('#popFMCGDiscountcontent-bs');
                        $('#popFMCGChargescontent > *').appendTo('#popFMCGChargescontent-bs');
                        $('#popFMCGRewardsProgramcontent > *').appendTo('#popFMCGRewardsProgramcontent-bs');
                        $('#popFMCGOnCreditcontent > *').appendTo('#popFMCGOnCreditcontent-bs');
                    } else {
                        $('#popupAccountDetails').after(vData).trigger('create');
                        $('#popupAccountDetails').nextAll('[data-role="popup"]').popup().trigger('create');
                    }                    
                }
            });
        })();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
define([
    'promise!/Scripts/sources/ERP/DeliveryTrackingEdit/controller.js?v=' + assetversion,
    '/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion,
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/DeliveryTracking/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.DeliveryTracking.controller', null, function () {

        var mOut = {};
        var mElement;
        var mTable;
        var mTableBody;
        var mDataTable;
        var mCityID;
        var mInvoiceDateFrom;
        var mInvoiceDateTo;
        var arrInvoices = [];
        var DeliveryTruckFowstID;
        var Delivery3PlCompanyID;
        var mAllRows;
        var mDeliveryEncodingDateFrom = $.format.date(new Date('03-01-2023'), 'MMM dd, yyyy'), mDeliveryEncodingDateTo = $.format.date(new Date(), 'MMM dd, yyyy');
        var rsStatus, rsID, isRSOldProgram, ctrInvoice = 1;        

        mElement = $(mHtml);
        mOut.element = mElement;

        var listInfoDeliveryChecker = mElement.find('[name="listInfoDeliveryChecker"]');
        var listInfoDeliveryDriver = mElement.find('[name="listInfoDeliveryDriver"]');
        var listInfoDeliveryHelper = mElement.find('[name="listInfoDeliveryHelper"]');
        var DeliveryChecker = [], DeliveryDriver = [], DeliveryHelper = [];

        $('#divErpRevenue').append(mElement);
        $('#divDeliveryTracking, #divDeliveryTrackingMain').pageshow();
        var tbodyInfoInvoices = $('[name="tbodyInfoInvoices"]');
        mTable = mElement.find('[name=myTable]');
        mTableBody = mTable.find('tbody');

        mElement.find('[name=btnMarkAsDelivered]').click(function () {
            if (arrInvoices.length > 0) {
                var arrInvoiceIDs = [], arrInvoiceNumber = [];

                $.grep(arrInvoices, function (e) {
                    arrInvoiceIDs.push(e.ID);

                    var pStr;
                    e.DocumentName;
                    if (e.DocumentName == null)
                        pStr = 'SI';
                    else
                        pStr = e.DocumentName;
                    pStr += '-' + e.MIN + '-' + e.InvoiceNumber;

                    arrInvoiceNumber.push(pStr)
                })

                if (mElement.find('[name=cbDelivered]').val() == 'Undelivered') {
                    ERP.DeliveryTrackingEdit.controller.popup({
                        InvoiceIDs: arrInvoiceIDs,
                        OnSave: LoadData
                    }, (arrInvoices.length > 1 ? null : arrInvoices[0]));
                } else {
                    NewMessageBox('Do you want to mark as undelivered ' + (arrInvoices.length > 1 ? 'these invoices' : 'this invoice') + '? <br><br>' + arrInvoiceNumber.join('<br>') + ' <br><br>' +
                        '<div><label class="form-label">Enter the reason why it was not delivered:</label><textarea data-role="none" class="form-control" name="txtUndeliveredReason" style="resize:none; height:150px;"></textarea></div>', 'Confirmation Dialog',
                        [{
                            Text: 'Yes', Click: function () {
                                ERP.DeliveryTrackingEdit.controller.SetUndeliveredAgain({
                                    InvoiceIDs: arrInvoiceIDs,
                                    Reason: $('[name="txtUndeliveredReason"]').val()
                                })
                            },
                            Validation: function (e) {
                                var txtReason = $('[name="txtUndeliveredReason"]');
                                if ($.trim(txtReason.val()) == '') {
                                    txtReason.addClass('is-invalid');
                                    WebToast('Please enter the reason why it was not delivered');
                                    return false;
                                } return true;
                            }
                        }, { Text: 'No' }
                    ])
                }                
            } else NewMessageBox('Please select at least one invoice');
        })

        mElement.find('[name=btnOutput]').click(function () {
            $('[name="popupOuput"]').modal('show');
            mElement.find('[name=listDeliveredInvoices]').empty();
            mElement.find('[name=cbOutputDeliveryMode]').val('Inhouse Delivery').change();
            mElement.find('.modal-footer').hide();
        })

        mElement.find('[name=btnRoutingSheet]').click(function () {
            $('[name="popupRoutingSheet"]').modal('show');
            mElement.find('[name=cbInfoDeliveryMode]').val('Inhouse Delivery').change();
            mElement.find('[name="popupRoutingSheet"] .modal-footer, [name="divRSInfo"]').hide();
            mElement.find('[name="divUndeliveredReason"]').hide().val('');
            mElement.find('[name="txtRoutingSheetNumber"]').val('');
            mElement.find('[name="lblRSStatus"]').text('');
        })

        mElement.find('[name=cbOutputDeliveryMode], [name=cbInfoDeliveryMode]').change(function () {
            mElement.find('[name=divOutputInhouseDelivery], [name=divInfoInhouseDelivery]').hide();
            mElement.find('[name=divOutput3rdPartyDelivery], [name=divInfo3rdPartyDelivery]').hide();
            mElement.find('[name=divOutputCustomerPickup], [name=divInfoCustomerPickup]').hide();
            mElement.find('[name=listDeliveredInvoices]').empty();
            //mElement.find('.modal-footer').hide();

            ClearFilter();

            switch ($(this).val()) {
                case 'Inhouse Delivery':
                    mElement.find('[name=divOutputInhouseDelivery], [name=divInfoInhouseDelivery]').show();
                    break;

                case '3rd Party Delivery':
                    mElement.find('[name=divOutput3rdPartyDelivery], [name=divInfo3rdPartyDelivery]').show();
                    break;

                case 'Customer Pickup':
                    mElement.find('[name=divOutputCustomerPickup], [name=divInfoCustomerPickup]').show();
                    break;

                default:

            }
        })

        mElement.find('[name="txtInfoEncodingDate"]').click(function () {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            SingleCalendar({
                ShowTime: true,
                SetTime: $.format.date(new Date(), 'hh:mm a'),
                Click: function (e) {
                    mElement.find('[name="txtInfoEncodingDate"]').val(e)
                }
            })
        })

        mElement.find('[name="btnAddInvoice"]').click(function () {
            BrowsePopup('TransInvoices', 'Search Invoice', [
                {
                    Message: 'You can select multiple invoices. Click OK when you\'re done.',
                    StyleFormat: 'table',
                    SelectMultiple: true,
                    Sorting: false,
                    DisplayText: [
                        {
                            Label: 'Invoice Date', Column: function (data) {
                                return $.format.date(new Date(data.InvoiceDate), 'MM-dd-yyyy hh:mm a');
                            }
                        }, {
                            Label: 'Invoice #', Column: function (data) {
                                var pStr;

                                if (CStr2(data.DocumentName) == '')
                                    pStr = 'SI';
                                else
                                    pStr = data.DocumentName;
                                pStr += '-' + data.MIN + '-' + data.InvoiceNumber;
                                return pStr;
                            }
                        }, {
                            Label: 'Customer Name', Column: function (data) {
                                return data.CompanyName;
                            }
                        }
                    ],
                    Click: function (e) {
                        for (var i in e) {
                            if (tbodyInfoInvoices.find('tr[invid="' + e[i].ID + '"]').length == 0) {
                                e[i].DeliveryStatus = 'For Delivery';
                                e[i].IsNewlyAddedInvoice = true;
                                DisplayInvoices(e[i])
                                ctrInvoice += 1;
                            }
                        }
                        ComputeInvoice();
                    }
                }
            ], {
                "NonZero": 1, "ExcludeCashPaymentCustomer": true , "DeliveryStatus": "Undelivered"
            })
        })

        mElement.find('[name="btnRetrieve"]').click(function () {
            var delivery = mElement.find('[name=cbOutputDeliveryMode]').val();

            if (mElement.find('[name=txtRoutingSheetNumber]').val() == '') {
                NewMessageBox('Please select Routing Sheet number');
                return;
            }

            AjaxSend({
                url: 'SalesMenu/GetRoutingSheetDetails',
                data: {
                    //NonZero: 1,
                    //ExcludeUndelivered: 1,
                    //DeliveryTruckFowstID: DeliveryTruckFowstID,
                    //Delivery3PlCompanyID: Delivery3PlCompanyID,
                    //DeliveryCustomerRepresentative: mElement.find('[name="txtOutputCustomerRepresentative"]').val(),
                    DeliveryEncodingDate: mElement.find('[name=txtOutputEncodingDate]').val(),
                    RoutingSheetNumber: mElement.find('[name=txtRoutingSheetNumber]').val()
                }, success: function (e) {
                    var header = e.Header.Rows[0];
                    var invoices = e.Invoices.Rows;
                    DeliveryChecker = e.Checker || [];
                    DeliveryDriver = e.Driver || [];
                    DeliveryHelper = e.Helper || [];
                    ctrInvoice = 1;
                    tbodyInfoInvoices.empty();                  
                    rsStatus = CBool(header.IsDraft) ? 'draft' : 'finalized';
                    $('[name="lblRSStatus"]').text(rsStatus.toUpperCase());
                    $('[name="cbInfoDeliveryMode"]').val(header.DeliveryMethod).prop('disabled', (CBool(header.IsDraft) ? 0 : 1)).change();
                    $('[name="txtInfoTruck"]').val(CStr2(header.DeliveryTruckFowstName));
                    $('[name="txtInfo3plCompanyName"]').val(CStr2(header.Delivery3PlCompanyName)).prop('disabled', (CBool(header.IsDraft) ? 0 : 1));
                    $('[name="txtInfo3plContactPerson"]').val(CStr2(header.Delivery3PlContactPerson)).prop('disabled', (CBool(header.IsDraft) ? 0 : 1));
                    $('[name="txtInfoWaybillNumber"]').val(CStr2(header.DeliveryWaybillNumber)).prop('disabled', (CBool(header.IsDraft) ? 0 : 1));
                    $('[name="txtInfoCustomerRepresentative"]').val(CStr2(header.DeliveryCustomerRepresentative)).prop('disabled', (CBool(header.IsDraft) ? 0 : 1));
                    $('[name="txtInfoDeliveryRemarks"]').val(CStr2(header.DeliveryNotes)).prop('disabled', (CBool(header.IsDraft) ? 0 : 1));
                    $('[name="txtInfoEncodingDate"]').val($.format.date(new Date(header.DeliveryEncodingDate), 'MMM dd, yyyy hh:mm a'));
                    DeliveryTruckFowstID = CStr2(header.DeliveryTruckFowstID);
                    Delivery3PlCompanyID = CStr2(header.Delivery3PlCompanyID);

                    for (var i in invoices) {
                        var data = invoices[i];
                        DisplayInvoices(data);
                        ctrInvoice += 1;
                    }
                    ComputeInvoice();
                    ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryChecker, DeliveryChecker, 'checker', rsStatus);
                    ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryDriver, DeliveryDriver, 'driver', rsStatus);
                    ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryHelper, DeliveryHelper, 'helper', rsStatus);

                    mElement.find('.modal-footer, [name="divRSInfo"]').show();

                    if (CBool(header.IsDraft)) {
                        mElement.find('[name="butSaveAsDraft"], [name="butSaveAsFinalized"]').parent().show();
                    } else {
                        mElement.find('[name="butSaveAsDraft"], [name="butSaveAsFinalized"]').parent().hide();
                    }
                    return
                    var vRows = e.Results.Rows;
                    var list = mElement.find('[name=listDeliveredInvoices]').empty();
                    var btn = mElement.find('[name="butCopy"]').unbind('click');                    

                    mElement.find('.modal-footer').hide();

                    if (vRows.length > 0) {
                        mElement.find('.modal-footer').show();

                        for (var i in vRows) {
                            var data = vRows[i];
                            var InvoiceNumber;

                            if (CStr2(data.DocumentName) == '')
                                InvoiceNumber = 'SI';
                            else
                                InvoiceNumber = data.DocumentName;
                            InvoiceNumber += '-' + data.MIN + '-' + data.InvoiceNumber;
                            data.ActualInvoiceNumber = InvoiceNumber;

                            list.append($('<li class="list-group-item">')
                                .append($('<div class="row align-items-center">')
                                    .append($('<div class="col-auto">').text((CInt2(i) + 1) + '.'))
                                    .append($('<div class="col">').html('Invoice No.: ' + data.ActualInvoiceNumber + ' <br>' +
                                        'Customer: ' + data.CompanyName))
                                )
                            )                            
                        }

                        btn.click(function () {
                            var msg = 'Route Sheet Details \n\n', txtarea = $('<textarea id="txtCopyTransToPlainText" style="margin-left:-1000px;">');

                            if ($('body').find('#txtCopyTransToPlainText').length == 0) {
                                $('body').append(txtarea);
                            } else {
                                txtarea = $('#txtCopyTransToPlainText');
                            }

                            if (delivery == 'Inhouse Delivery') {
                                msg += 'Truck: ' + mElement.find('[name="txtOutputTruck"]').val();
                            } else if (delivery == '3rd Party Delivery') {
                                msg += '3PL Company Name: ' + mElement.find('[name="txtOutput3plCompanyName"]').val();
                            } else if (delivery == 'Customer Pickup') {
                                msg += 'Customer Representative: ' + mElement.find('[name="txtOutputCustomerRepresentative"]').val();
                            }

                            msg += '\n\n';

                            for (var i in vRows) {
                                var data = vRows[i];
                                msg += (CInt2(i) + 1) + '. \n';
                                msg += 'Invoice No.:\t' + data.ActualInvoiceNumber + ' \n';
                                msg += 'Customer Name:\t' + data.CompanyName  + ' \n\n';
                            }

                            txtarea.val(msg).select();
                            document.execCommand("copy");
                            WebToast('Copied to clipboard');
                        })
                    }
                }
            })
        })

        mElement.find('[name="butPrint"]').click(function () {
            if (!Validation()) return;

            window.location = MergeUrlParameters({
                RoutingSheetNumber: mElement.find('[name=txtRoutingSheetNumber]').val(),
                OrgID: sessionStorage.OrgID,
                FowstID: sessionStorage.FowstID
            }, '/SalesMenu/PrintDelivery');
        })

        mElement.find('[name=butInfoSearch3plCompanyName], [name=txtInfo3plCompanyName]').click(function () {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            BrowsePopup('LibraryOrganizationSupplier', '3PL Company',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        DisplayText: [
                            {
                                Column: 'SupplierName'
                            }
                        ],
                        Click: function (e) {
                            Delivery3PlCompanyID = e.ID;
                            mElement.find('[name=txtInfo3plCompanyName]').val(e.SupplierName);
                        }
                    }
                ]
            );
        });

        mElement.find('[name="butSaveAsDraft"]').click(function () {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            Resave(1);
        })

        mElement.find('[name="butSaveAsFinalized"]').click(function () {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            NewMessageBox('When saving as finalized, you won\'t be able to edit this routing sheet in the future. Do you want to continue?', 'Confirmation Dialog', [
                {
                    Text: 'Yes', Click: function () {
                        Resave(0);
                    }
                }, { Text: 'No' }
            ])            
        })

        mElement.find('[name="chbMark"]').change(function () {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            mElement.find('[name="tbodyInfoInvoices"] tr select').val($(this).val())

            if ($(this).val() == 'Undelivered') mElement.find('[name="divUndeliveredReason"]').show()
            else mElement.find('[name="divUndeliveredReason"]').hide()
        })

        function ComputeInvoice() {
            var total = 0;

            tbodyInfoInvoices.find('tr').each(function () {
                total += CDec2($(this).data('data').ServedAmount);
            })

            mElement.find('[name="lblInvoiceTotal"]').text('TOTAL: ' +AccountingFormat(total));
        }

        function DisplayInvoices(data) {
            var pStr;

            if (CStr2(data.DocumentName) == '')
                pStr = 'SI';
            else
                pStr = data.DocumentName;
            pStr += '-' + data.MIN + '-' + data.InvoiceNumber;
            data.ActualInvoiceNumber = pStr;
            tbodyInfoInvoices.append($('<tr>')
                .attr('invid', data.ID)
                .attr('isnewlyaddedinvoice', CBool(data.IsNewlyAddedInvoice))
                .data('data', data)
                .append($('<td>').text(ctrInvoice + '.'))
                .append($('<td>').append($('<a href="#">')
                    .text(pStr)
                    .data('data', data)
                    .click(function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        var data = $(this).data('data');
                        PeekView({
                            ID: data.ID,
                            DontReplaceCurrentID: true,
                            IncludeZeroQty: data.TransType == 'PurchaseOrder',
                            TransType: data.TransType
                        });
                    })
                    )
                )
                .append($('<td>').text(data.CompanyName))
                .append($('<td class="text-right">').text(AccountingFormat(CDec2(data.ServedAmount))))
                .append($('<td>').append($('<select data-role="none" class="form-select">')
                        .prop('disabled', (rsStatus == 'draft' ? 0 : 1))
                        .append('<option value="Undelivered">Undelivered</option>')
                        .append('<option value="Delivered">Delivered</option>')
                        .append('<option value="For Delivery">For Delivery</option>')
                        .val(data.DeliveryStatus)
                        .change(function () {
                            var cnt = 0;
                            tbodyInfoInvoices.find('tr select').each(function () {
                                if ($(this).find('option:selected').val() == 'Undelivered') {
                                    cnt += 1;
                                }
                            })
                            if (cnt > 0) mElement.find('[name="divUndeliveredReason"]').show()
                        })
                    )
                )
                .append($('<td>').append($('<button data-role="none" class="btn btn-danger" title="Remove Invoice">')
                        .prop('disabled', (rsStatus == 'draft' ? 0 : 1))
                        .data('data', data)
                        .append('<span class="bi-trash"></span>')
                        .click(function () {
                            var dis = $(this), data = dis.data('data');
                            NewMessageBox('Are you sure you want to remove this invoice number ' + data.ActualInvoiceNumber + '?', 'Confirmation Dialog', [
                                {
                                    Text: 'Yes', Click: function () {
                                        dis.parent().closest('tr').hide();
                                        dis.parent().closest('tr').find('select').val('Undelivered');
                                        data.InvoiceStatusInRS = 'remove';                                        
                                        ctrInvoice -= 1;
                                    }
                                }, { Text: 'No' }
                            ])
                        })
                    )
                )
            )
        }

        mElement.find('[name=butInfoSearchCheckedBy], [name=txtInfoCheckedBy]').click(BrowseCheckedBy);

        function BrowseCheckedBy() {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            BrowsePopup('LibraryOrganizationEmployee', 'Checked By',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            DeliveryChecker = $.merge(DeliveryChecker || [], e);

                            ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryChecker, DeliveryChecker, 'checker');

                            //mOutgoingData.DeliveryCheckedByID = e.ID;
                            //mElement.find('[name=txtCheckedBy]').val(e.PeopleName);
                        }
                    }
                ], {
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butInfoSearchDriver], [name=txtInfoDriver]').click(BrowseDriver);

        function BrowseDriver() {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            BrowsePopup('LibraryOrganizationEmployee', 'Driver',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            DeliveryDriver = $.merge(DeliveryDriver || [], e);

                            ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryDriver, DeliveryDriver, 'driver');

                            //mOutgoingData.DeliveryDriverID = e.ID;
                            //mElement.find('[name=txtDriver]').val(e.PeopleName);
                        }
                    }
                ], {
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butInfoSearchHelper], [name=txtInfoHelper]').click(BrowseHelper);

        function BrowseHelper() {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            BrowsePopup('LibraryOrganizationEmployee', 'Helper',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            DeliveryHelper = $.merge(DeliveryHelper || [], e);

                            ERP.DeliveryTrackingEdit.controller.DisplayDeliveryDetails(listInfoDeliveryHelper, DeliveryHelper, 'helper');

                            //mOutgoingData.DeliveryHelperID = e.ID;
                            //mElement.find('[name=txtHelper]').val(e.PeopleName);
                        }
                    }
                ], {
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function Resave(IsDraft) {
            if (!ResaveValidation()) return;

            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            var arrInvoices = [], cntUndelivered = 0, cntForDelivery = 0;
            var data = {
                DeliveryLoggerID: rsID,
                RoutingSheetNumber: mElement.find('[name="txtRoutingSheetNumber"]').val(),
                DeliveryMethod: mElement.find('[name="cbInfoDeliveryMode"]').val(),
                DeliveryTruckFowstID: DeliveryTruckFowstID,
                Delivery3PlCompanyID: Delivery3PlCompanyID,
                Delivery3PlCompanyName: mElement.find('[name="txtInfo3plCompanyName"]').val(),
                Delivery3PlContactPerson: mElement.find('[name=txtInfo3plContactPerson]').val(),
                DeliveryWaybillNumber: mElement.find('[name=txtInfoWaybillNumber]').val(),
                DeliveryCustomerRepresentative: mElement.find('[name="txtInfoCustomerRepresentative"]').val(),
                DeliveryEncodingDate: mElement.find('[name="txtInfoEncodingDate"]').val(),
                DeliveryRemarks: mElement.find('[name="txtInfoDeliveryRemarks"]').val(),
                IsDraft: IsDraft,
                Checker: DeliveryChecker,
                Driver: DeliveryDriver,
                Helper: DeliveryHelper
            };

            mElement.find('[name="tbodyInfoInvoices"] tr').each(function () {
                var dis = $(this), data = dis.data('data');
                var status = dis.find('select option:selected').val();

                if (status == 'Undelivered' && CStr2(data.InvoiceStatusInRS) == '') { // wala giremove ang invoice
                    cntUndelivered += 1;
                }

                if (status == 'For Delivery') {
                    cntForDelivery += 1;
                }

                arrInvoices.push({ InvoiceID: data.ID, Status: status, IsNewlyAddedInvoice: CBool(dis.attr('isnewlyaddedinvoice')), InvoiceStatusInRS: CStr2(data.InvoiceStatusInRS) })
            })

            if (!CBool(IsDraft)) {
                if (cntForDelivery > 0) {
                    NewMessageBox('Saving as finalized means all invoices will be tagged as delivered. Do you want to tag all invoices as "delivered"?', 'Confirmation Dialog',
                        [
                            {
                                Text: 'Yes, tag as "Delivered"', Click: function () {
                                    mElement.find('[name="tbodyInfoInvoices"] tr select').val('Delivered');
                                    Resave(IsDraft);
                                }
                            }, { Text: 'No, maybe next time' }
                        ])
                    return;
                }
            }

            data.Invoices = arrInvoices;

            if (cntUndelivered > 0) {
                if ($.trim(mElement.find('[name="txtInfoUndeliveredReason"]').val()) == '') {
                    WebToast('Please enter the reason why Invoice(s) was not delivered');
                    return;
                } else {
                    data.Reason = $.trim(mElement.find('[name="txtInfoUndeliveredReason"]').val());
                }
            }
            Bs.Loading('show', 'Saving ...');
            AjaxSend({
                url: '/SalesMenu/ResaveRoutingSheet',
                data: data,
                success: function (e) {
                    Bs.Loading('hide');
                    if (e.Result == 'saved') {
                        mElement.find('[name="popupRoutingSheet"]').modal('hide');
                        NewMessageBox('Routing sheet has been saved. <br><br> Do you want to download this routing sheet?', 'Message',
                            [
                                {
                                    Text: 'Yes', Click: function () {
                                        data.OrgID = sessionStorage.OrgID;
                                        data.DownloadReport = 1;
                                        data.NonZero = 1;
                                        data.ExcludeUndelivered = 1;
                                        data.IsOldProgram = 0;

                                        window.location = MergeUrlParameters(data, '/SalesMenu/PrintDelivery');                                     
                                    }
                                }, { Text: 'No' }
                            ])
                    } else {
                        NewMessageBox(e.Error, 'Error');
                    }
                }
            })
        }

        function ResaveValidation() {
            var delivery = mElement.find('[name=cbInfoDeliveryMode]').val();

            if (delivery == 'Inhouse Delivery' && CStr2(DeliveryTruckFowstID) == '') {
                WebToast('Please select truck');
                return false;
            } else if (delivery == '3rd Party Delivery' && CStr2(Delivery3PlCompanyID) == '') {
                WebToast('Please select 3PL Company');
                return false;
            } else if (delivery == 'Customer Pickup' && $.trim(mElement.find('[name="txtInfoCustomerRepresentative"]').val()) == '') {
                WebToast('Please enter Customer Representative name');
                return false;
            }

            if (mElement.find('[name="txtRoutingSheetNumber"]').val() == '') {
                WebToast('Please select Routing Sheet number');
                return false;
            }

            return true;
        }

        function Validation() {
            var delivery = mElement.find('[name=cbOutputDeliveryMode]').val();

            if (delivery == 'Inhouse Delivery' && CStr2(DeliveryTruckFowstID) == '') {
                NewMessageBox('Please select truck');
                return false;
            } else if (delivery == '3rd Party Delivery' && CStr2(Delivery3PlCompanyID) == '') {
                NewMessageBox('Please select 3PL Company');
                return false;
            } else if (delivery == 'Customer Pickup' && $.trim(mElement.find('[name="txtOutputCustomerRepresentative"]').val()) == '') {
                NewMessageBox('Please enter Customer Representative name');
                return false;
            }

            return true;
        }

        mElement.find('[name=butOutputSearchTruck], [name=txtOutputTruck], [name=butInfoSearchTruck], [name=txtInfoTruck]').click(BrowseTruck);

        function BrowseTruck(vParams) {
            if (rsStatus == 'finalized') {
                NewMessageBox('Sorry! This Routing Sheet has already been saved as finalized. Modification is not allowed.');
                return
            }

            BrowsePopup('LibraryFowst', 'Select Truck',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [
                            {
                                Column: 'FowstName'
                            }
                        ],
                        Click: function (e) {
                            DeliveryTruckFowstID = e.ID;
                            mElement.find('[name=txtOutputTruck]').val(e.FowstName);
                            mElement.find('[name=txtInfoTruck]').val(e.FowstName);
                        }
                    }
                ], {
                FowstTypeID: 'CE791943-9D80-42FC-BEBE-5BB52A5792E8'
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butRoutingSheetNumber], [name=txtRoutingSheetNumber]').click(BrowseRoutingSheetNumbers);

        function BrowseRoutingSheetNumbers() {
            //if (!Validation()) return;

            BrowsePopup('RoutingSheetNumbers', 'Select Routing Sheet Number',
                [
                    {
                        StyleFormat: 'table',
                        AutoSearch: true,
                        Sorting: false,
                        Options: {
                            Fields: [
                                $('<tr>')
                                    .append($('<td>')
                                        .append('Show Status: ')
                                    )
                                    .append($('<td class="pl-3">')
                                        .append($('<div class="form-check">')
                                            .append($('<label class="form-check-label">')
                                                .append('Draft')
                                                .append($('<input type="radio" name="radRSStatus" value="1" class="form-check-input" checked>')
                                                    .change(function () {
                                                        BrowsePopupNow();
                                                    })
                                                )
                                            )
                                        )
                                    )
                                    .append($('<td class="pl-3">')
                                        .append($('<div class="form-check">')
                                            .append($('<label class="form-check-label">')
                                                .append('Finalized')
                                                .append($('<input type="radio" name="radRSStatus" value="0" class="form-check-input">')
                                                    .change(function () {
                                                        BrowsePopupNow();
                                                    })
                                                )
                                            )
                                        )
                                    )
                            ],
                            OnChange: function () {
                                return {
                                    Status: $('[name="radRSStatus"]:checked').val()
                                }
                            }
                        },
                        DisplayText: [
                            {
                                Label: 'Routing Sheet No.',
                                Column: 'RoutingSheetNumber'
                            },
                            {
                                Label: 'Details',
                                Column: function (e) {
                                    return CStr2(e.TruckName) + '' + CStr2(e.Delivery3PlCompanyName) + '' + CStr2(e.DeliveryCustomerRepresentative)
                                }
                            },
                            {
                                Label: 'Delivery Encoding Date',
                                Column: 'DeliveryEncodingDate'
                            },
                            {
                                Label: 'Status',
                                Column: function (e) {
                                    return CBool(e.IsDraft) ? 'Draft' : 'Finalized'
                                }
                            }
                        ],
                        Click: function (e) {
                            rsID = e.ID;
                            isRSOldProgram = e.IsOldProgram;
                            mElement.find('[name=txtRoutingSheetNumber]').val(e.RoutingSheetNumber);
                            mElement.find('[name="btnRetrieve"]').click();
                        }
                    }
                ],
                {
                    DeliveryTruckFowstID: DeliveryTruckFowstID,
                    DeliveryEncodingDateFrom: mDeliveryEncodingDateFrom,
                    DeliveryEncodingDateTo: mDeliveryEncodingDateTo,
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butOutputSearch3plCompanyName], [name=txtOutput3plCompanyName]').click(BrowseSupplier);

        mElement.find('[name="chkExcludeCashPaymentCustomer"]').change(function () {
            LoadData();
        })

        mElement.find('[name="txtOutputEncodingDate"]').val(mDeliveryEncodingDateFrom + ' to ' + mDeliveryEncodingDateTo).click(function () {
            DualCalendar({
                Click: function (e) {
                    mDeliveryEncodingDateFrom = e.From;
                    mDeliveryEncodingDateTo = e.To;
                    mElement.find('[name="txtOutputEncodingDate"]').val(e.From + ' to ' + e.To);
                }
            })
        })

        function BrowseSupplier() {
            BrowsePopup('LibraryOrganizationSupplier', '3PL Company',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        DisplayText: [
                            {
                                Column: 'SupplierName'
                            }
                        ],
                        Click: function (e) {
                            Delivery3PlCompanyID = e.ID;
                            mElement.find('[name=txtOutput3plCompanyName]').val(e.SupplierName);
                        }
                    }
                ], {
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function ClearFilter() {
            DeliveryTruckFowstID = null;
            Delivery3PlCompanyID = null;

            mElement.find('[name="txtInfoTruck"], [name="txtInfo3plCompanyName"], [name="txtInfo3plContactPerson"], [name="txtInfoWaybillNumber"], [name="txtInfoCustomerRepresentative"]').val('');
        }

        function LoadData() {
            arrInvoices = [];
            var pData = {
                NonZero: 1,
                DateFrom: mInvoiceDateFrom,
                DateTo: mInvoiceDateTo,
                DeliveryCityID: mCityID,
                keywords: mElement.find('[name=txtSearch]').val().trim(),
                MaxRecords: 0,
                ExcludeCashPaymentCustomer: mElement.find('[name="chkExcludeCashPaymentCustomer"]').is(':checked'),
                DeliveryStatus: mElement.find('[name=cbDelivered]').val()
            };
            if (mElement.find('[name=cbDelivered]').val() == 'Undelivered') {
                //pData.ExcludeDelivered = 1; pData.ExcludeDelivered = 1;
                mElement.find('[name=btnMarkAsDelivered] > .menu-description').html('Mark <br> For Delivery');
            } else {
                //pData.ExcludeUndelivered = 1;
                mElement.find('[name=btnMarkAsDelivered] > .menu-description').html('Mark as <br> Undelivered');
            }
            mElement.find('[name=divLoading]').show();
            Bs.Loading('show');
            AjaxSend({
                url: '/Browser/TransInvoices',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else {
                        mAllRows = vData.Results.Rows
                        DisplayData(vData);
                    }
                },
                complete: function () {
                    Bs.Loading('hide');
                    mElement.find('[name=divLoading]').hide();
                }
            });
        }

        function DisplayData(vData) {
            
            //if (s != undefined) {
            //    tblSysAdminDataTable.search($.trim(s)).draw();
            //    return;
            //}
            var ctr = 0;
            var pData = {
                columns: [
                    {
                        title: '',
                        data: null,
                        "createdCell": function (td, cellData, rowData, row, col) {
                            return $(td).html($('<input data-role="none" type="checkbox" class="form-check-input">')
                                .prop('checked', CInt2(rowData.IsChecked))
                                .data('data', rowData)
                                .click(function (e) {
                                    e.stopPropagation();
                                    var data = $(this).data('data'), index = null;
                                    data.IsChecked = $(this).is(':checked');

                                    $.grep(arrInvoices, function (e, i) {
                                        if (e.ID == data.ID) {
                                            index = i
                                            return false; //stop
                                        }
                                    })

                                    if (index != null) {
                                        arrInvoices.splice(index, 1);
                                    } else arrInvoices.push(data);

                                    var pCount = 0;
                                    var pTotal = 0;
                                    for (var p = 0; p <= mAllRows.length - 1; p++) {
                                        var pDR = mAllRows[p];
                                        if (pDR.IsChecked) {
                                            pCount++;
                                            pTotal += CDec2(pDR.Total);
                                        }
                                    }
                                    $('#spanValueOfSelectedInvoices').text(AccountingFormat(pTotal));
                                    $('#spanNumberOfSelectedInvoices').text(AccountingFormat(pCount, 0));
                                })
                            )
                            .click(function (e) {
                                e.stopPropagation();
                            })
                        }, width: 'auto'
                    },
                    {
                        title: '#',
                        "data": function (data, type, full, meta) {
                            ctr += 1;
                            return ctr + '.';
                        }
                    },
                    {
                        title: 'Routing Sheet #',
                        visible: (mElement.find('[name="cbDelivered"]').val() == 'Undelivered' ? false : true),
                        "data": function (data, type, full, meta) {
                            return CStr2(data.RoutingSheetNumber);
                        }, className: 'text-center'
                    },
                    {
                        title: 'Invoice No.',
                        "data": function (data, type, full, meta) {
                            var pStr;
                            data.DocumentName;
                            if (data.DocumentName == null)
                                pStr = 'SI';
                            else
                                pStr = data.DocumentName;
                            pStr += '-' + data.MIN + '-' + data.InvoiceNumber;

                            data.ActualInvoiceNumber = pStr;
                            return '<a href="#">' + pStr + '</a>';
                        }, createdCell: function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData)
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    PeekView({
                                        ID: rowData.ID,
                                        DontReplaceCurrentID: true,
                                        IncludeZeroQty: rowData.TransType == 'PurchaseOrder',
                                        TransType: rowData.TransType
                                    });
                                })
                        }
                    },
                    {
                        title: 'Invoice Date',
                        "data": function (data, type, full, meta) {
                            return data.InvoiceDate + '';
                        }
                    },
                    {
                        title: 'Customer',
                        "data": function (data, type, full, meta) {
                            return data.CompanyName + '';
                        }
                    },
                    //{
                    //    title: 'Delivery Address',
                    //    "data": function (data, type, full, meta) {
                    //        return CStr2(data.DeliveryAddress);
                    //    }
                    //},
                    {
                        title: 'Street Address',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.DeliveryStreet);
                        }
                    },
                    {
                        title: 'City / Town',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.DeliveryCity);
                        }
                    },
                    {
                        title: 'Delivery Status',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.DeliveryStatus);
                            //if (CBool(data.IsDelivered))
                            //    return 'Delivered';
                            //else
                            //    return 'Undelivered';
                        }, className: 'text-center'
                    },
                    {
                        title: 'Invoice Print Status',
                        "data": function (data, type, full, meta) {
                            return (CInt2(data.PrintCount) > 0 ? 'Printed' : 'Unprinted');
                        }, className: 'text-center'
                    }
                ]
            };
            pData.data = vData.Results.Rows;

            SysAdminDataTable(pData);
        }

        mElement.find('[name=butCity]').click(BrowseCity);
        
        function BrowseCity() {
            BrowsePopup('LibraryCities', 'City / Town',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        AllowToSelectAll: true,
                        StyleFormat: 'table',
                        DisplayText: [
                            {
                                Label: 'City / Town',
                                Column: 'Name'
                            }
                        ],
                        Click: function (e) {
                            if (e.length > 0) {
                                mCityID = '';
                                mElement.find('[name=butCity]').text('All Cities');
                            } else {
                                mCityID = e.ID;
                                mElement.find('[name=butCity]').text(e.Name);
                            }
                            
                            LoadData();
                        }
                    }
                ], { 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function SysAdminDataTable(vData) {
            mTableBody.empty()
            
            if ($.fn.DataTable.isDataTable(mTable)) {
                //check if datatable is created 
                mTable.DataTable().clear().destroy();
            }
            Bs.Loading('show');
            mDataTable = mTable.DataTable({
                //retrieve: true,
                data: vData.data,
                "bdestroy": true,
                "pageLength": 50,
                "order": [[0, "asc"]],
                "bProcessing": true,
                //"bServerSide": true,
                //"ajax": vData.ajax,
                "autoWidth": false,
                "columns": vData.columns,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    Bs.Loading('hide');
                    mTable.removeClass('dataTable');
                    mTable.find('thead > tr').addClass('align-top text-center').css('font-size', '14px');
                }
                //"stateSave": true
            })

            mTableBody.unbind('click').on('click', 'tr', function () {
                var pData = mDataTable.row(this).data();
                if (CStr2(pData) != '') {
                    ERP.DeliveryTrackingEdit.controller.popup({
                        InvoiceIDs: [pData.ID],
                        OnSave: LoadData
                    }, pData);
                }
            });     

            mElement.find('[name=txtSearch]').unbind('keydown').keydown(function (e) {
                if (e.which == 13) {
                    mDataTable.search($(this).val()).draw();
                }
            })
        }

        mElement.find('[name=cbDelivered]').change(LoadData);
        mInvoiceDateFrom = new Date(new Date() - 1000 * 60 * 60 * 24 * 7);
        mInvoiceDateTo = new Date();
        mElement.find('[name=butInvoiceDate]').text(FormatDate(mInvoiceDateFrom, {Format: 'MMM d, yyyy'}) + ' to ' + FormatDate(mInvoiceDateTo, {Format: 'MMM d, yyyy'}));
        mElement.find('[name=butInvoiceDate]').click(BrowseInvoiceDate);

        function BrowseInvoiceDate() {
            DualCalendar({
                Click: function(vData) {
                    mInvoiceDateFrom = vData.From;
                    mInvoiceDateTo = vData.To;
                    mElement.find('[name=butInvoiceDate]').text(vData.From + ' to ' + vData.To);
                    LoadData();
                }
            });
        }

        function PeekView(vParams) {
            Bs.Loading('show');
            vParams = $.extend({
                ID: null,
                LoadProductsOnly: false,
                DontReplaceCurrentID: false,
                IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                AccountsPayableFollowCost: CBool(vParams.AccountsPayableFollowCost)
            }, vParams);
            new ERP.OrderProcessing.work.model().GetCart({
                ID: vParams.ID,
                IncludeZeroQty: vParams.IncludeZeroQty,
                IncludeSuppliers: mOut.ShowProductSuppliersColumn,
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                IncludeChildOrderItems: false,
                Success: function (vData) {
                    Bs.Loading('hide');
                    var msg = CopyTransToPlainText(vParams.TransType, vData, true);
                    NewMessageBox(msg.replace(/(?:\r\n|\r|\n)/g, '<br>'), 'Peek View');
                }
            });
        }

        mElement.find('#chkShowInvoicesAll').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            ClearTable();
            DisplayData({
                Results: {
                    Rows: mAllRows
                }
            });
            //ActivateTab.call(this, e);
        });

        mElement.find('#chkShowInvoicesMarked').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            var pNewRows = [];

            //ActivateTab.call(this, e);
            for (var p = 0; p <= mAllRows.length - 1; p++) {
                var pDR = mAllRows[p];
                if (pDR.IsChecked)
                    pNewRows.push(pDR);
            }
            ClearTable();
            DisplayData({
                Results: {
                    Rows: pNewRows
                }
            });
        });

        mElement.find('#chkShowInvoicesUnmarked').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            var pNewRows = [];

            //ActivateTab.call(this, e);
            for (var p = 0; p <= mAllRows.length - 1; p++) {
                var pDR = mAllRows[p];
                if (!pDR.IsChecked)
                    pNewRows.push(pDR);
            }
            ClearTable();
            DisplayData({
                Results: {
                    Rows: pNewRows
                }
            });
        });

        LoadData();

        mOut.LoadData = LoadData

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    

    return mDeferred.promise();

});
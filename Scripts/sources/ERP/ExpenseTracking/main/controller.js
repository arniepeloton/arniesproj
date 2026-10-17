define(['/Scripts/sources/ERP/Invoicing/InvoiceImage/controller.js?crc=' + assetversion]
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/ExpenseTracking/main/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.ExpenseTracking.main.controller = ERP.ExpenseTracking.main.controller()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.ExpenseTracking.main.controller', null, function () {

            var mOut = {};
            var mElement, isDoneLoadCentersPage = false;

            mElement = $(mHtml);
            mOut.element = mElement;

            $('#divErpExpenseTracking').append(mElement);//.trigger('create');

            var expenseID = '', empID = '', associatedTo = '', costcenterID = '', expenseData = null, isApproved = 0, isRejected = 0, reimburseToEmp = '', disburseToVendor = '', isFromUnfinishedExpense = 0;
            var txtWebLogExpenseEmpName = mOut.element.find('#txtWebLogExpenseEmpName');
            var selWebLogExpenseType = mOut.element.find('#selWebLogExpenseType');
            var txtWebLogExpenseDate = mOut.element.find('#txtWebLogExpenseDate');
            var txtWebLogExpenseCostCenter = mOut.element.find('#txtWebLogExpenseCostCenter'); //aka FOWST
            var selWebLogExpenseCostCenterLocation = mOut.element.find('#selWebLogExpenseCostCenterLocation');
            var txtWebLogExpenseAssociatedTo = mOut.element.find('#txtWebLogExpenseAssociatedTo');
            var txtWebLogExpenseFromLocation = mOut.element.find('#txtWebLogExpenseFromLocation');
            var txtWebLogExpenseToLocation = mOut.element.find('#txtWebLogExpenseToLocation');
            var txtWebLogExpenseAmount = mOut.element.find('#txtWebLogExpenseAmount');
            var chbExpenseAmountToBeFollow = mOut.element.find('#chbExpenseAmountToBeFollow');
            var txtWebLogExpenseParticulars = mOut.element.find('#txtWebLogExpenseParticulars');
            var txtWebLogExpenseRemarks = mOut.element.find('#txtWebLogExpenseRemarks');
            var expensepaymentmethod = mOut.element.find('[name="expensepaymentmethod"]');
            var selWebLogExpenseRequestedBy = mOut.element.find('#selWebLogExpenseRequestedBy');
            var btnWebLogExpenseNew = mOut.element.find('#btnWebLogExpenseNew');
            var btnWebLogExpenseLookup = mOut.element.find('#btnWebLogExpenseLookup');
            var btnWebLogExpenseSave = mOut.element.find('#btnWebLogExpenseSave');
            var btnWebLogExpensePrint = mOut.element.find('#btnWebLogExpensePrint');
            var btnWebLogExpenseActions = mOut.element.find('#btnWebLogExpenseActions');
            var btnWebLogExpenseHistory = mOut.element.find('#btnWebLogExpenseHistory');
            var txtInvoiceNumber = mOut.element.find('[name="txtInvoiceNumber"]');
            var butInvoiceImageReference = mOut.element.find('[name="butInvoiceImageReference"]');

            LoadRequiredData();

            $('#btnETWork').off('click').click(function () {
                PromptUserAccess('Access to Expense Tracker Module', {
                    Message: 'You are not allowed to create Expense Report via online.',
                    OnClickOKSuccess: function () {
                        $('#tabETWork').opentabmenu();
                    }
                });
            })

            $('#btnETSettings').off('click').click(function () {
                PromptUserAccess('Access to Expense Tracker Settings', {
                    Message: 'You don\'t have access to Expense Tracker Settings',
                    OnClickOKSuccess: function () {
                        $('#tabETSettings').opentabmenu();
                        LoadSettings();
                    }
                });
            })

            $('#selETCostCenterStatus').change(function () {
                mOut.GetCostCenters();
            })

            $('#selETCostCenterStatus').change(function () {
                mOut.GetRevenueCenters();
            })

            $('#btnETAddCostCenters').click(function () {
                sessionStorage.ViewID = '';
                sessionStorage.LibraryName = 'Cost Centers';
                CallCentersPage('Cost Center');
            })

            $('#btnETAddRevenueCenters').click(function () {
                sessionStorage.ViewID = '';
                sessionStorage.LibraryName = 'Revenue Centers';
                CallCentersPage('Revenue Center');
            })

            $('#btnETAddMakers').click(function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Select Makers', [
                    {
                        AutoSearch: true,
                        Message: 'Note: You can select multiple users.',
                        SelectMultiple: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            AjaxSend({
                                url: 'Expenses/SaveExpenseTicket',
                                data: { data:  e, Type: 'maker' },
                                success: function (e) {
                                    if (e.Results == 'saved') {
                                        WebToast('Successfully added', 'top');
                                        mOut.GetExpenseTicket('maker');
                                    } else {
                                        NewMessageBox(e.Error, 'Error');
                                    }
                                }
                            })
                        }
                    }
                ])
            })

            $('#btnETAddAuthorizer').click(function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Select Authorizers', [
                    {
                        AutoSearch: true,
                        Message: 'Note: You can select multiple users.',
                        SelectMultiple: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            AjaxSend({
                                url: 'Expenses/SaveExpenseTicket',
                                data: { data: e, Type: 'authorizer' },
                                success: function (e) {
                                    if (e.Results == 'saved') {
                                        WebToast('Successfully added', 'top');
                                        mOut.GetExpenseTicket('authorizer');
                                    } else {
                                        NewMessageBox(e.Error, 'Error');
                                    }
                                }
                            })
                        }
                    }
                ])
            })

            $('#divETPMOnCreditFromVendor').click(function () {
                BrowseVendor(function (e) {
                    $('[name="expensepaymentmethod"]').prev().removeClass('active');
                    $('#divETPMOnCreditFromVendor').addClass('active');
                    $('#radETOnCreditFromVendor').attr('checked', 'checked');
                    disburseToVendor = e.ID;
                    reimburseToEmp = '';
                    $('#spanETPDVendorName, #spanETReimburseTo').text('-');
                    $('#spanETOCFromVendorName').html(e.SupplierName + '<br>' + (e.CreditTermsInDays == undefined ? '' : e.CreditTermsInDays + ' Days'));
                })
            })

            $('#divETPMForPaymentDisbursement').click(function () {
                BrowseVendor(function (e) {
                    $('[name="expensepaymentmethod"]').prev().removeClass('active');
                    $('#divETPMForPaymentDisbursement').addClass('active');
                    $('#radETForPaymentDisbursement').attr('checked', 'checked');
                    disburseToVendor = e.ID;
                    reimburseToEmp = '';
                    $('#spanETOCFromVendorName, #spanETReimburseTo').text('-');
                    $('#spanETPDVendorName').html(e.SupplierName);
                })
            })

            $('#divETPMForReimbursement').click(function () {
                if (expenseData != null) {
                    if (CBool(expenseData.IsApproved) || CBool(expenseData.IsRejected)) {
                        WebToast('Modification is not allowed', 'top');
                        return;
                    }
                }

                BrowsePopup('LibraryOrganizationEmployee', 'Select Person To Reimburse', [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            $('[name="expensepaymentmethod"]').prev().removeClass('active');
                            $('#divETPMForReimbursement').addClass('active');
                            $('#radETForReimbursement').attr('checked', 'checked');
                            disburseToVendor = '';
                            reimburseToEmp = e.ID;
                            $('#spanETOCFromVendorName, #spanETPDVendorName').text('-');
                            $('#spanETReimburseTo').html('Reimburse To: <br>' + e.PeopleName);
                        }
                    }
                ])
            })

            $('#divETPMForExpenseRecordingOnly').click(function () {
                if (expenseData != null) {
                    if (CBool(expenseData.IsApproved) || CBool(expenseData.IsRejected)) {
                        WebToast('Modification is not allowed', 'top');
                        return;
                    }
                }

                $('[name="expensepaymentmethod"]').prev().removeClass('active');
                $(this).addClass('active');
                $('#radETForExpenseRecordingOnly').attr('checked', 'checked');
                disburseToVendor = '';
                reimburseToEmp = '';
                $('#spanETOCFromVendorName, #spanETPDVendorName, #spanETReimburseTo').text('-');
            })

            $('#selLogExpenseRemarksField, #chbAllowCustomLocationName').change(function () {
                SaveOptionalSettings();
            })

            $('#selETCostCenterStatus').change(function () {
                mOut.GetCostCenters();
            })

            $('#selETRevenueCenterStatus').change(function () {
                mOut.GetRevenueCenters();
            })

            $('#btnETAddRouteSchedule').click(function () {
                NewPromptUserAccess('Allow to add/edit/delete Employee\'s Route Schedule', {
                    Message: 'You\'re not allowed to add Route Schedule.',
                    OnClickOKSuccess: function () {
                        ExpenseTracking.RouteSchedule();
                    }
                });                
            })

            $('#txtETRouteScheduleSearch').keydown(function (e) {
                if (e.which == 13) {
                    mOut.GetRouteSchedules();
                }
            })

            chbExpenseAmountToBeFollow.change(function () {
                if ($(this).is(':checked')) {
                    txtWebLogExpenseAmount.attr('disabled', 'disabled').val('');
                    txtWebLogExpenseAmount.parent().removeClass('has-error');
                } else {
                    txtWebLogExpenseAmount.removeAttr('disabled');
                }
            })

            txtWebLogExpenseEmpName.click(function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Select Person', [{
                    AutoSearch: true,
                    DisplayText: [{ Column: 'PeopleName' }],
                    Click: function (e) {
                        empID = e.ID;
                        txtWebLogExpenseEmpName.val(e.PeopleName);
                    }
                }])
            })

            txtWebLogExpenseCostCenter.click(function () {
                BrowsePopup('Expenses/GetCostCenters', 'Select Cost Center', [{
                    AutoSearch: true,
                    DisplayText: [{ Column: 'Name' }, { Column: 'Description' }],
                    Click: function (e) {
                        costcenterID = e.ID;
                        txtWebLogExpenseCostCenter.val(e.Name);
                    }
                }])
            })

            selWebLogExpenseType.change(function () {
                var expensetype = $(this).val();

                if (expensetype == 'Jeepney Fare' ||
                    expensetype == 'Bus Fare' ||
                    expensetype == 'Taxi Fare' ||
                    expensetype == 'Van Fare' ||
                    expensetype == 'Tricycle Fare' ||
                    expensetype == 'Air Fare' ||
                    expensetype == 'Sea Fare' ||
                    expensetype == 'Train Fare') {
                    $('.lblExpenseTypeTranspo').show();
                } else {
                    txtWebLogExpenseFromLocation.val('');
                    txtWebLogExpenseToLocation.val('');
                    $('.lblExpenseTypeTranspo').hide();
                }
            })

            txtWebLogExpenseAssociatedTo.click(function () {
                PopupButtonList('Get data from:', 'Options', [
                    {
                        Text: 'Customer',
                        Size: 2,
                        Click: function () {
                            BrowsePopup('LibraryOrganizationCustomer', 'Select Customer', [
                                {
                                    DisplayText: [{ Column: 'CustomerName' }],
                                    Click: function (e) {
                                        associatedTo = e.ID;
                                        txtWebLogExpenseAssociatedTo.val(e.CustomerName);
                                    }
                                }
                            ])
                        }
                    },
                    {
                        Text: 'Supplier',
                        Size: 2,
                        Click: function () {
                            BrowsePopup('LibraryOrganizationSupplier', 'Select Supplier', [
                                {
                                    DisplayText: [{ Column: 'SupplierName' }],
                                    Click: function (e) {
                                        associatedTo = e.ID;
                                        txtWebLogExpenseAssociatedTo.val(e.SupplierName);
                                    }
                                }
                            ])
                        }
                    },
                    {
                        Text: 'No Association',
                        Size: 2,
                        Click: function () {
                            associatedTo = '';
                            txtWebLogExpenseAssociatedTo.val('No Association');
                        }
                    }
                ])
            })

            txtWebLogExpenseAmount.keydown(function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })

            btnWebLogExpenseNew.click(function () {
                ClearFields();
                WebToast('You can now add new expense', 'top')
            })

            btnWebLogExpenseLookup.click(function () {
                BrowsePopup('/Expenses/GetAllLogExpenses', 'List Of Expense Tickets', [{
                    ModalSize: 'fullscreen',
                    AutoSearch: true,
                    ServerSideProcessing: true,
                    StyleFormat: 'table',
                    Sorting: false,
                    Options: {
                        Fields: [
                            $('<tr>')
                                .append($('<td>')
                                    .append('Show Tickets: ')
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Unapproved')
                                            .append($('<input type="radio" name="radETBrowse" value="unapproved" class="form-check-input" checked>')
                                                .change(function () {
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )                                        
                                    )
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Approved')
                                            .append($('<input type="radio" name="radETBrowse" value="approved" class="form-check-input">')
                                                .change(function () {
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )                                
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Rejected')
                                            .append($('<input type="radio" name="radETBrowse" value="rejected" class="form-check-input">')
                                                .change(function () {
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Without Amount')
                                            .append($('<input type="radio" name="radETBrowse" value="unfinished" class="form-check-input">')
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
                                ShowTickets: $('[name="radETBrowse"]:checked').val()
                            }
                        }
                    },
                    DisplayText: [
                        {
                            Label: 'ET No.', Column: 'ExpenseTicketNo',
                            Style: {
                                width: '150px'
                            }
                        },
                        {
                            Label: 'Expense Date', Column: function (e) {
                                return $.format.date(new Date(parseInt((e.ExpenseDate || e.DateCreated).substr(6))), 'MM-dd-yyyy')
                            }
                        },
                        {
                            Label: 'Location From', Column: function (e) {
                                return CStr2(e.FromLocation)
                            }
                        },
                        {
                            Label: 'Location To', Column: function (e) {
                                return CStr2(e.ToLocation)
                            }
                        },
                        {
                            Label: 'Date Created', Column: function (e) {
                                return $.format.date(CJsonDate(e.DateCreated), 'MM/dd/yyyy hh:mm a')
                            }
                        },
                        {
                            Label: 'Expense Type', Column: 'ExpenseType'
                        },
                        {
                            Label: 'Associated To', Column: function (e) {
                                return (CBool(e.IsAssociatedACustomer) ? '<span class="badge bg-success" title="Customer">C</span> ' : '') + (CBool(e.IsAssociatedASupplier) ? '<span class="badge bg-warning" title="Supplier">S</span> ' : '') + CStr2(e.AssociatedToName)
                            }
                        },
                        {
                            Label: 'Payment Method', Column: function (e) {
                                return CStr2(e.PaymentMethod)
                            }
                        },
                        {
                            Label: 'Disburse / Reimburse To', Column: function (e) {
                                return CStr2(e.DisburseToVendorName || e.ReimburseToEmployeeName)
                            }
                        },
                        {
                            Label: 'Amount', Column: function (e) {
                                return e.Amount == null ? '' : AccountingFormat(e.Amount)
                            }, className: 'text-right'
                        },
                        {
                            Label: 'Particulars', Column: 'Particulars'
                        },
                        {
                            Label: 'Remarks', Column: 'Remarks'
                        }
                    ],
                    Click: function (e) {
                        ClearFields();

                        expenseData = e;
                        expenseID = e.ID;
                        empID = CStr2(e.PeopleID);
                        reimburseToEmp = CStr2(e.ReimburseToEmployee);
                        disburseToVendor = CStr2(e.DisburseToVendor);
                        costcenterID = (e.CostCenter || '817B4C2A-AF4B-4F58-BA21-C3907320AA32'.toLowerCase())
                        associatedTo = CStr2(e.AssociatedTo);
                        txtWebLogExpenseAmount.val(e.Amount == null ? '' : CDec2(e.Amount));
                        txtWebLogExpenseAssociatedTo.val(CStr2(e.AssociatedToName))
                        txtWebLogExpenseDate.val($.format.date(ConvertDate(e.ExpenseDate || e.DateCreated), 'yyyy-MM-dd'));
                        txtWebLogExpenseEmpName.val(e.EmployeeName);
                        txtWebLogExpenseFromLocation.val(e.FromLocation);
                        txtWebLogExpenseToLocation.val(e.ToLocation);
                        txtWebLogExpenseRemarks.val(e.Remarks);
                        txtWebLogExpenseParticulars.val(e.Particulars);
                        txtWebLogExpenseCostCenter.val(CStr2(e.CostCenterName));
                        selWebLogExpenseCostCenterLocation.val(e.FowstID);
                        selWebLogExpenseType.val(e.ExpenseType);
                        //selWebLogExpenseRequestedBy.val(e.RequestedBy);
                        chbExpenseAmountToBeFollow.attr('checked', CBool(e.IsUnfinishedExpense));//.checkboxradio('refresh');
                        $('[name="expensepaymentmethod"][value="' + e.PaymentMethod + '"]').attr('checked', true);
                        $('[name="expensepaymentmethod"][value="' + e.PaymentMethod + '"]').prev().addClass('active');

                        if (CBool(e.IsUnfinishedExpense)) {
                            txtWebLogExpenseAmount.attr('disabled', 'disabled');
                        }

                        $('#lblETReferenceNo').text('ET-' + e.ExpenseTicketNo);
                        $('#lblETEncodedBy').text('Encoded By: ' + e.EncodedByName + ' on ' + $.format.date(ConvertDate(e.DateCreated), 'MM/dd/yyyy hh:mm a'));

                        if (CBool(e.IsApproved) && !CBool(e.IsRejected)) {
                            $('#lblETStatus').text('APPROVED');
                            $('#lblETOtherInfo').html('Approved By: ' + e.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(e.ApprovedOrRejectedOn), 'MM/dd/yyyy hh:mm a') + '<br>');
                        } else if (!CBool(e.IsApproved) && CBool(e.IsRejected)) {
                            $('#lblETStatus').text('REJECTED');
                            $('#lblETOtherInfo').html('Rejected By: ' + e.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(e.ApprovedOrRejectedOn), 'MM/dd/yyyy hh:mm a') + '<br>');
                        } else if (!CBool(e.IsApproved) && !CBool(e.IsRejected)) {
                            $('#lblETStatus').text('UNAPPROVED');
                        } else {
                            $('#lblETStatus').text('UNFINISHED');
                        }

                        if (e.LastUpdatedByName != null) {
                            $('#lblETOtherInfo').append('Last Updated By: ' + e.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(e.LastUpdated), 'MM/dd/yyyy hh:mm a') + '<br>');
                        }

                        if (CBool(e.IsCancelled)) {
                            $('#lblETOtherInfo').append('Cancelled By: ' + e.CancelledByName + ' on ' + $.format.date(ConvertDate(e.CancelledOn), 'MM/dd/yyyy hh:mm a'));
                        }

                        if (e.PaymentMethod == 'For Reimbursement') {
                            $('#spanETReimburseTo').html('Reimburse To: <br>' + e.ReimburseToEmployeeName);
                        } else if (e.PaymentMethod == 'For Payment Disbursement') {
                            $('#spanETPDVendorName').text(e.DisburseToVendorName);
                        } else if (e.PaymentMethod == 'On Credit From Vendor') {
                            $('#spanETOCFromVendorName').html(e.DisburseToVendorName + '<br>' + (e.CreditTermsInDays == undefined ? '' : e.CreditTermsInDays + ' Days'));
                        }                        

                        if (CBool(e.IsApproved) && CBool(e.IsUnfinishedExpense) && CStr2(e.Remarks) != '') {
                            $('#lblETStatus').append(' but without amount');
                            $('#divCurrentRemarks').html('<b>Previous remarks:</b> <br><br><div style="border: 1px solid #CCC; border-radius: 3px; padding:10px;">' + CStr2(e.Remarks).replace(/(?:\r\n|\n|\r)/g, '<br>') + '</div><br><br> <b>NOTE:</b> The encoded remarks will be merge with the previous remarks. To avoid redundancy, please do not re-enter the previous remarks to the Remarks field below.<br><br>');
                            chbExpenseAmountToBeFollow.removeAttr('checked');//.checkboxradio('refresh');
                            txtWebLogExpenseAmount.removeAttr('disabled');
                            txtWebLogExpenseRemarks.val('').removeAttr('disabled');
                        }

                        if (CBool(e.IsApproved) && CBool(e.IsRejected)) {
                            $('#tabETWorkBasicInfo').find('input[type="text"], input[type="date"], select, textarea').attr('disabled', 'disabled');
                        }

                        if (disburseToVendor != '') {
                            ERP.InvoiceImage.main.controller.GetInvoiceRefImages(function (e) {
                                txtInvoiceNumber.val('');
                                var vRows = e.Results.Rows;
                                if (vRows.length > 0) {
                                    txtInvoiceNumber.val(vRows.map(function (elem) {
                                        return elem.InvoiceRefNo;
                                    }).join(', '));
                                }
                            }, {
                                mod: 'Expense',
                                ID: expenseID,
                                ExpenseNumber: expenseData.ExpenseReferenceNo
                            })
                        }
                    }
                }])
            })

            btnWebLogExpenseSave.click(function () {
                if (expenseData != null){
                    if (CBool(expenseData.IsApproved)) {
                        NewMessageBox('This Expense Ticket has already been <b>approved</b>', 'Message');
                        return;
                    }

                    if (CBool(expenseData.IsRejected)) {
                        NewMessageBox('This Expense Ticket has already been <b>rejected</b>', 'Message');
                        return;
                    }
                }                

                var isValid = true, expensetype = selWebLogExpenseType.val();

                $('.form-group').removeClass('has-error');

                //if (empID == '') {
                //    txtWebLogExpenseEmpName.parent().addClass('has-error');
                //    isValid = false;
                //}

                if (selWebLogExpenseType.val() == '') {
                    selWebLogExpenseType.parent().addClass('has-error');
                    isValid = false;
                }

                if (txtWebLogExpenseDate.val() == '') {
                    txtWebLogExpenseDate.parent().addClass('has-error');
                    isValid = false;
                }

                if (txtWebLogExpenseCostCenter.val() == '') {
                    txtWebLogExpenseCostCenter.parent().addClass('has-error');
                    isValid = false;
                }

                if (expensetype == 'Jeepney Fare' ||
                    expensetype == 'Bus Fare' ||
                    expensetype == 'Taxi Fare' ||
                    expensetype == 'Van Fare' ||
                    expensetype == 'Tricycle Fare' ||
                    expensetype == 'Air Fare' ||
                    expensetype == 'Sea Fare' ||
                    expensetype == 'Train Fare') {
                    if ($.trim(txtWebLogExpenseFromLocation.val()) == '') {
                        txtWebLogExpenseFromLocation.parent().addClass('has-error');
                        isValid = false;
                    }

                    if ($.trim(txtWebLogExpenseToLocation.val()) == '') {
                        txtWebLogExpenseToLocation.parent().addClass('has-error');
                        isValid = false;
                    }
                }

                if ($.trim(txtWebLogExpenseAmount.val()) == '' && !chbExpenseAmountToBeFollow.is(':checked')) {
                    txtWebLogExpenseAmount.parent().addClass('has-error');
                    isValid = false;
                } else {

                }

                if (!IsNumeric(txtWebLogExpenseAmount.val()) && !chbExpenseAmountToBeFollow.is(':checked')) {
                    txtWebLogExpenseAmount.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(txtWebLogExpenseParticulars.val()) == '') {
                    txtWebLogExpenseParticulars.parent().addClass('has-error');
                    isValid = false;
                }

                //if ($.trim(selWebLogExpenseRequestedBy.val()) == '') {
                //    selWebLogExpenseRequestedBy.parent().addClass('has-error');
                //    isValid = false;
                //}

                if ($('[name="expensepaymentmethod"]:checked').length == 0) {
                    //expensepaymentmethod.parent().addClass('has-error');
                    isValid = false;
                } else {
                    if ($('[name="expensepaymentmethod"]:checked').val() == 'For Reimbursement') {
                        if (reimburseToEmp == '') {
                            NewMessageBox('Please select employee to reimbursement payment', 'Message');
                        }
                    }
                }

                if (!isValid) {
                    WebToast('Please enter required fields (<span class="required">*</span>)', 'top');
                    return;
                }

                if (expenseData != null) {
                    if (!CBool(expenseData.IsApproved) && !CBool(expenseData.IsRejected)) { // UNAPPROVED
                        if (sessionStorage.PeopleID != expenseData.EncodedBy) {
                            PromptUserAccess('Allow to edit expenses made by other people', {
                                Message: 'You are not allowed to edit expense tickets made by other people',
                                OnClickOKSuccess: function () {
                                    SaveNow();
                                }
                            });
                        } else {
                            SaveNow();
                        }
                    }
                } else {
                    AjaxSend({
                        url: 'Expenses/CheckTicketUserIfHasAccess',
                        data: { UserID: sessionStorage.PeopleID, type: 'maker' },
                        success: function (e) {
                            if (CBool(e.Results)) {
                                SaveNow();
                            } else {
                                NewMessageBox('Sorry! You\'re not allowed to create Expense Ticket. Please ask your administrator for assistance.', 'Message');
                            }
                        }
                    })                    
                }
            })

            btnWebLogExpensePrint.click(function () {
                if (CStr2(expenseID) == '') {
                    NewMessageBox('Please select approved Expense Ticket', 'Message');
                    return;
                }

                if (!CBool(expenseData.IsApproved)){
                    NewMessageBox('Only approved Expense Ticket can be downloaded', 'Message');
                    return;
                }

                window.location = MergeUrlParameters({
                    ID: expenseID,
                    OrgID: sessionStorage.OrgID
                }, '/Expenses/DownloadExpenseTicketReport');
            })

            btnWebLogExpenseActions.click(function () {
                if (expenseData != null) {
                    if (!CBool(expenseData.IsApproved) && !CBool(expenseData.IsRejected)) { // UNAPPROVED
                        NewMessageBox('What would you like to do for this Unapproved Expense Ticket?', 'Actions', [
                            {
                                Text: 'Approve',
                                Theme: 'btn-success',
                                Size: 2,
                                Click: function () {
                                    CheckIfAuthorizer('approve', function () {
                                        isApproved = 1;
                                        isRejected = 0;
                                        SaveNow();
                                    })                                 
                                }
                            },
                            {
                                Text: 'Reject',
                                Theme: 'btn-danger',
                                Size: 2,
                                Click: function () {
                                    CheckIfAuthorizer('reject', function () {
                                        isApproved = 0;
                                        isRejected = 1;
                                        SaveNow();
                                    })
                                }
                            },
                            {
                                Text: 'Close',
                                Size: 2,
                                Click: function () {

                                }
                            }
                        ])
                    } else if (CBool(expenseData.IsApproved) && !CBool(expenseData.IsRejected) && CBool(expenseData.IsUnfinishedExpense)) {
                        NewMessageBox('Would you like to save as finalized Expense Ticket no. <b>' + expenseData.ExpenseTicketNo + '</b>?', 'Actions', [
                            {
                                Text: 'Yes, save as finalized now',
                                Theme: 'b',
                                Size: 2,
                                Click: function () {
                                    var msg = '';

                                    if ($.trim(txtWebLogExpenseAmount.val()) == '') {
                                        NewMessageBox('Please enter exact amount', 'Message');
                                        return;
                                    }

                                    if ($.trim(txtWebLogExpenseRemarks.val()) == '') {
                                        NewMessageBox('Please enter remarks', 'Message');
                                        return;
                                    }

                                    isApproved = 1;
                                    isRejected = 0;
                                    isFromUnfinishedExpense = 1;
                                    SaveNow();
                                }
                            },
                            {
                                Text: 'Maybe later',
                                Size: 2,
                                Click: function () {

                                }
                            }
                        ])
                    } else {
                        NewMessageBox('The selected Expense Ticket (ET) was already finalized. Modification is not allowed ' +
                            'but you can Cancel this ET if you seems there\'s something wrong with the data.', 'Message', [
                                {
                                    Text: 'Yes, I will cancel this ET', Click: function () {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: '/Expenses/CancelET',
                                            data: { ID: expenseData.ID },
                                            success: function (e) {
                                                if (e.Results == 'saved') {
                                                    NewMessageBox('ET-' + expenseData.ExpenseTicketNo + ' has been cancelled.', 'Message');
                                                    $('#lblETOtherInfo').append('Cancelled By: ' + sessionStorage.CurrentUserFullName + ' on ' + $.format.date(ConvertDate(new Date()), 'MM/dd/yyyy hh:mm a'));
                                                } else {
                                                    NewMessageBox('There\'s something wrong while cancelling the ET. Please ask assistance from any Mobiz representative.');
                                                }
                                            }
                                        })
                                    }
                                }, { Text: 'Just do nothing' }
                            ]);
                    }
                } else {
                    NewMessageBox('This feature is only available for Unapproved Expense Tickets', 'Message');
                }
            })

            btnWebLogExpenseHistory.click(function () {
                if (expenseData == null) {
                    NewMessageBox('Please select Expense Ticket', 'Message');
                    return;
                }

                ExpenseTracking.GetLogExpenseHistory(expenseData);
            })

            butInvoiceImageReference.click(function () {
                if (expenseData != null) {
                    ERP.InvoiceImage.main.controller.popup({
                        mod: 'Expense',
                        ID: expenseID,
                        ExpenseNumber: expenseData.ExpenseReferenceNo,
                        SupplierID: disburseToVendor,
                        SupplierName: expenseData.DisburseToVendorName
                    });
                } else {
                    NewMessageBox('Please save as draft before you can attach Invoice reference.')
                }
                return;
                if ($.trim(disburseToVendor) != '') {
                                        
                } else {
                    NewMessageBox('Attaching Invoice image is only for Payment method: On Credit from Vendor or For Payment Disbursement.')
                }
            });

            function SaveOptionalSettings() {
                var data = {
                    LogExpenseRemarksField: CInt2($('#selLogExpenseRemarksField').is(':checked')),
                    AllowCustomLocationName: CInt2($('#chbAllowCustomLocationName').is(':checked'))
                }

                AjaxSend({
                    url: 'Expenses/SaveOptionalSettings',
                    data: { data: data },
                    success: function (e) {
                        if (e.Results == 'saved') {
                            WebToast('Optional settings has been successfully!', 'top');
                            mOut.GetOptionalSettings();
                        }
                    }
                })
            }

            function BrowseVendor(callback) {
                if (expenseData != null) {
                    if (CBool(expenseData.IsApproved) || CBool(expenseData.IsRejected)) {
                        WebToast('Modification is not allowed', 'top');
                        return;
                    }
                }

                BrowsePopup('LibraryOrganizationSupplier', 'Select Vendor', [
                    {
                        DisplayText: [{ Column: 'SupplierName' }],
                        Click: function (e) {
                            callback(e);
                        }
                    }
                ])
            }

            function CheckIfAuthorizer(action, callback) {
                AjaxSend({
                    url: 'Expenses/CheckTicketUserIfHasAccess',
                    data: { UserID: sessionStorage.PeopleID, type: 'authorizer' },
                    success: function (e) {
                        if (CBool(e.Results)) {
                            callback()
                        } else {
                            NewMessageBox('Sorry! You\'re not allowed to ' + action + ' Expense Ticket.', 'Message');
                        }
                    }
                })
            }

            function SaveNow() {
                var data = {
                    PeopleID: empID,
                    ExpenseDate: $.trim(txtWebLogExpenseDate.val()),
                    ExpenseType: selWebLogExpenseType.val(),
                    FromLocation: $.trim(txtWebLogExpenseFromLocation.val()),
                    ToLocation: $.trim(txtWebLogExpenseToLocation.val()),
                    Amount: txtWebLogExpenseAmount.val(),
                    FowstID: selWebLogExpenseCostCenterLocation.val(),
                    CostCenter: costcenterID,
                    Remarks: $.trim(txtWebLogExpenseRemarks.val()),
                    Particulars: $.trim(txtWebLogExpenseParticulars.val()),
                    AssociatedTo: associatedTo,
                    ID: expenseID,
                    IsUnfinishedExpense: CInt2(chbExpenseAmountToBeFollow.is(':checked')),
                    IsApproved: isApproved,
                    IsRejected: isRejected,
                    IsFromUnfinishedExpense: isFromUnfinishedExpense,
                    PaymentMethod: $('[name="expensepaymentmethod"]:checked').val(),
                    ReimburseToEmployee: reimburseToEmp,
                    DisburseToVendor: disburseToVendor,
                    //RequestedBy: selWebLogExpenseRequestedBy.val(),
                    IsCreatedInWebApp: 1
                }

                if (isFromUnfinishedExpense) {
                    data.Remarks = expenseData.Remarks + '\n\n' + $.trim(txtWebLogExpenseRemarks.val());
                }

                if (CBool(isApproved)) {
                    data.StatusExpenseTicket = 'approved';
                }

                if (CBool(isRejected)) {
                    data.StatusExpenseTicket = 'rejected';
                }

                AjaxSend({
                    url: 'Expenses/SaveLogExpense',
                    data: {
                        data: data
                    },
                    success: function (e) {
                        if (e.Results == 'saved') {
                            if (CBool(isApproved)) {
                                NewMessageBox('Expense Ticket has been approved! Would you like to download the report?', 'Message', [
                                    {
                                        Text: 'Yes, download the report now!', Click: function () {
                                            window.location = MergeUrlParameters({
                                                ID: e.ID,
                                                OrgID: sessionStorage.OrgID
                                            }, '/Expenses/DownloadExpenseTicketReport');
                                        }
                                    }, { Text: 'No, maybe later.' }
                                ]);
                            } 
                            
                            if (CBool(isRejected)) {
                                NewMessageBox('Expense Ticket has been rejected', 'Message');
                            }

                            if (CStr2(data.ID) == '') { // for new
                                NewMessageBox('Expense Ticket has been successfully saved', 'Message');
                            }
                            
                            ClearFields();
                            ExpenseTracking.GetLogExpenseByMonth();
                            ExpenseTracking.CountUnfinishedExpenseReport();
                            ExpenseTracking.CountUnapprovedExpenseTicket();
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            }

            function LoadRequiredData() {
                ClearFields();
                ExpenseTracking.GetLogExpenseByMonth();
                ExpenseTracking.CountUnfinishedExpenseReport();
                ExpenseTracking.CountUnapprovedExpenseTicket();

                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: {
                        IsAll: 1
                    },
                    success: function (e) {
                        var rows = e.Results.Rows;

                        selWebLogExpenseCostCenterLocation.empty();

                        selWebLogExpenseCostCenterLocation.append(new Option('Please select', ''));

                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++){
                                selWebLogExpenseCostCenterLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                            }

                            if (rows.length == 1) {
                                selWebLogExpenseCostCenterLocation.val(rows[0].ID);
                            }

                            selWebLogExpenseCostCenterLocation.val(sessionStorage.FowstID);
                        }
                    }
                })                

                AjaxSend({
                    url: 'Expenses/GenerateDefaultCostCenter',
                    success: function (e) {
                        if (e.Error != undefined) {
                            NewMessageBox(e.Error, 'Error');
                        }

                        //AjaxSend({
                        //    url: 'Expenses/GetCostCenters',
                        //    success: function (e) {
                        //        var rows = e.Results.Rows;

                        //        txtWebLogExpenseCostCenter.empty();

                        //        txtWebLogExpenseCostCenter.append(new Option('Please select', ''));

                        //        if (rows.length > 0) {
                        //            for (var i = 0; i < rows.length; i++) {
                        //                txtWebLogExpenseCostCenter.append(new Option(rows[i].Name, rows[i].ID));
                        //            }
                        //        }
                        //    }
                        //})
                    }
                })

                //AjaxSend({
                //    url: 'Browser/LibraryOrganizationEmployee',
                //    success: function (e) {
                //        var rows = e.Results.Rows;

                //        selWebLogExpenseRequestedBy.empty();

                //        selWebLogExpenseRequestedBy.append(new Option('Please select', ''));

                //        if (rows.length > 0) {
                //            for (var i = 0; i < rows.length; i++) {
                //                selWebLogExpenseRequestedBy.append(new Option(rows[i].PeopleName, rows[i].ID));
                //            }
                //        }
                //    }
                //})
            }

            function ClearFields() {
                expenseData = null;
                expenseID = '';
                empID = '';
                associatedTo = '';
                costcenterID = '';
                reimburseToEmp = '';
                disburseToVendor = '';
                isApproved = 0;
                isRejected = 0;
                isFromUnfinishedExpense = 0;
                txtWebLogExpenseAmount.removeAttr('disabled').val('');
                txtWebLogExpenseAssociatedTo.val('');                
                txtWebLogExpenseEmpName.val('');
                txtWebLogExpenseFromLocation.val('');
                txtWebLogExpenseToLocation.val('');
                txtWebLogExpenseParticulars.val('');
                txtWebLogExpenseRemarks.val('');
                txtWebLogExpenseCostCenter.val('');
                selWebLogExpenseCostCenterLocation.val(sessionStorage.FowstID);
                selWebLogExpenseType.val('');
                //selWebLogExpenseRequestedBy.val('');
                chbExpenseAmountToBeFollow.removeAttr('checked');//.checkboxradio('refresh');
                expensepaymentmethod.removeAttr('checked');
                $('#divCurrentRemarks').empty();
                $('[name="expensepaymentmethod"]').prev().removeClass('active')
                $('.lblExpenseTypeTranspo').hide();
                $('#lblETReferenceNo').text('NEW');
                $('#lblETStatus, #lblETEncodedBy, #lblETOtherInfo').text('');
                $('#spanETOCFromVendorName, #spanETPDVendorName, #spanETReimburseTo').text('-');
                $('#tabETWorkBasicInfo').find('input[type="text"], input[type="date"], select, textarea').removeAttr('disabled');

                txtWebLogExpenseDate.attr({
                    min: $.format.date(new Date(), 'yyyy-MM-01'),
                    max: $.format.date(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0), 'yyyy-MM-dd'),
                    value: $.format.date(new Date(), 'yyyy-MM-dd')
                })
            }

            function CallCentersPage(type) {
                //require([GetPathFormat('/Scripts/sources/Centers/Centers.js?crc=')

                //]);
                sessionStorage.IsCenterFromExpenseTracker = true;

                AddBreadCrumb({
                    text: type, Click: function () {
                        $('#divETCentersPage').pageshow();
                    }
                }, '.ItemsBreadCrumbs')

                $('#divETCentersPage').pageshow();

                if (!isDoneLoadCentersPage) {
                    AjaxSend({
                        ExactUrl: '/Centers?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isDoneLoadCentersPage = true;
                            $('#divETCentersPage').append(vData).trigger('create');
                        }
                    });
                } else if (CStr2(sessionStorage.ViewID) != '') {
                    Centers.View();
                } else {
                    Centers.Add();
                }
            }

            function LoadSettings() {
                mOut.GetExpenseTicket('maker');
                mOut.GetExpenseTicket('authorizer');
                mOut.GetCostCenters();
                mOut.GetRevenueCenters();
                mOut.GetOptionalSettings();
                mOut.GetRouteSchedules();
            }

            function ConvertDate(date) {
                if (isNaN(new Date(date))) {
                    date = new Date(parseInt(date.substr(6)));
                } else {
                    date = new Date(date);
                }

                return date;
            }

            mOut.GetOptionalSettings = function () {
                AjaxSend({
                    url: 'Expenses/GetOptionalSettings',
                    success: function (e) {
                        $('#selLogExpenseRemarksField').prop('checked', CInt2(e.LogExpenseRemarksField[0].DefaultValue));
                        $('#chbAllowCustomLocationName').prop('checked', CInt2(e.AllowCustomLocationName));
                    }
                })
            }

            var tblETMakers;
            mOut.GetExpenseTicket = function (type) {
                var tbl = type == 'maker' ? '#tblETMakers' : '#tblETAuthorizer';
                //$('#tbodyETMakers').empty()

                if ($.fn.DataTable.isDataTable(tbl)) {
                    //check if datatable is created 
                    $(tbl).DataTable().clear().destroy();
                }

                tblETMakers = $(tbl).DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 10,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/Expenses/GetExpenseTicket?OrgID=" + sessionStorage.OrgID + '&type=' + type,
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.MakerName;
                            }
                        },
                        {
                            "data": null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                $(td).css('text-align', 'center').text($.format.date(new Date(parseInt(rowData.DateAdded.substr(6))), 'MMM dd, yyyy hh:mm a'));
                            }
                        },
                        {
                            "data": null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                $(td).css('text-align', 'center').text(rowData.AddedByName);
                            }
                        },
                        {
                            data: null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                var button = $('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .data('data', rowData)
                                                .click(function () {
                                                    var data = $(this).data('data');
                                                    
                                                    NewMessageBox('Are you sure you want to remove <b>' + data.MakerName + '</b>?', 'Confirmation Message', [
                                                        {
                                                            Text: 'Yes', Click: function (e) {
                                                                AjaxSend({
                                                                    url: 'Expenses/RemoveExpenseTicket',
                                                                    data: { ID: data.ID, type: type },
                                                                    success: function (e) {
                                                                        if (e.Results == 'deleted') {
                                                                            WebToast('Successfully removed', 'top');
                                                                            mOut.GetExpenseTicket(type);
                                                                        } else {
                                                                            NewMessageBox(e.Results, 'Error');
                                                                        }
                                                                    }
                                                                })
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                })

                                $(td).css('text-align', 'right').html(button);
                            },
                            "width": "50px"
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [4] }],
                    "initComplete": function () {
                        $(tbl).removeClass('dataTable');
                    },
                    "responsive": true,
                    "columnDefs": [{ "targets": 4, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })
            }

            var tblETCostCenters;
            mOut.GetCostCenters = function() {
                $('#tbodyETCostCenters').empty()

                if ($.fn.DataTable.isDataTable("#tblETCostCenters")) {
                    //check if datatable is created 
                    $('#tblETCostCenters').DataTable().clear().destroy();
                }

                tblETCostCenters = $('#tblETCostCenters').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 10,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=centers&type=Cost Centers&isactive=' + $('#selETCostCenterStatus').val(),
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.DateCreated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ModifiedDate == null) {
                                    return '';
                                }
                                return $.format.date(new Date(parseInt(data.ModifiedDate.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.LastModifiedBy;
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "initComplete": function(){
                        $('#tblETCostCenters').removeClass('dataTable');
                    },
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })

                $('#tblETCostCenters tbody').unbind('click').on('click', 'tr', function () {
                    if (CStr2(tblETCostCenters.row(this).data()) != '') {
                        sessionStorage.ViewID = tblETCostCenters.row(this).data().ID;
                        CallCentersPage('Cost Center');
                    }
                });
            }

            var tblETRevenueCenters;
            mOut.GetRevenueCenters = function () {
                $('#tbodyETRevenueCenters').empty()

                if ($.fn.DataTable.isDataTable("#tblETRevenueCenters")) {
                    //check if datatable is created 
                    $('#tblETRevenueCenters').DataTable().clear().destroy();
                }

                tblETRevenueCenters = $('#tblETRevenueCenters').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 10,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=centers&type=Revenue Centers&isactive=' + $('#selETRevenueCenterStatus').val(),
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.DateCreated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ModifiedDate == null) {
                                    return '';
                                }
                                return $.format.date(new Date(parseInt(data.ModifiedDate.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.LastModifiedBy;
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "initComplete": function () {
                        $('#tblETRevenueCenters').removeClass('dataTable');
                    },
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })

                $('#tblETRevenueCenters tbody').unbind('click').on('click', 'tr', function () {
                    if (CStr2(tblETRevenueCenters.row(this).data()) != '') {
                        sessionStorage.ViewID = tblETRevenueCenters.row(this).data().ID;
                        CallCentersPage('Revenue Center');
                    }
                });
            }

            var tblETRouteSchedule;
            mOut.GetRouteSchedules = function () {
                $('#tblETRouteSchedule tbody').empty()

                if ($.fn.DataTable.isDataTable("#tblETRouteSchedule")) {
                    //check if datatable is created 
                    $('#tblETRouteSchedule').DataTable().clear().destroy();
                }

                tblETRouteSchedule = $('#tblETRouteSchedule').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 20,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/Expenses/GetRouteSchedules?OrgID=" + sessionStorage.OrgID + "&keywords=" + $.trim($('#txtETRouteScheduleSearch').val()),
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.EmployeeName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return ((data.Frequency || 'monthly') == 'monthly' ? 'Monthly' : 'Weekly');
                            }, className: 'text-center', width: '150px'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CBool(data.StrictlyFollowSched) ? 'Fixed' : 'Flexible';
                            }, className: 'text-center', width: '170px'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.LastUpdatedBy + ' <br> <span style="font-size: 12px;">on ' + $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MM-dd-yyyy hh:mm a') + '</span>';
                            }, width: '150px'
                        },
                        //{
                        //    "data": function (data, type, full, meta) {
                        //        return '';
                        //    }
                        //}
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "initComplete": function () {
                        $('#tblETRouteSchedule').removeClass('dataTable');
                    },
                    "rowCallback": function (row, data, displayNum, displayIndex, dataIndex) {
                        $(row)
                            .data('data', data)
                            .click(function (e) {
                                e.stopPropagation();
                                var data = $(this).data('data');
                                NewPromptUserAccess('Allow to add/edit/delete Employee\'s Route Schedule', {
                                    Message: 'You\'re not allowed to edit Route Schedule.',
                                    OnClickOKSuccess: function () {
                                        ExpenseTracking.RouteSchedule(data);
                                    }
                                });
                            })
                    },
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })
            }

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })
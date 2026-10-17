define(['']
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/GatePass/main/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.GatePass.main.controller = ERP.GatePass.main.controller()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.GatePass.main.controller', null, function () {

            var mOut = {};
            var mElement, isDoneLoadCentersPage = false;

            mElement = $(mHtml);
            mOut.element = mElement;

            $('#divGatePass').empty().append(mElement).trigger('create');

            var gatepassID = '', empID = '', requestedby = '', associatedTo = '', costcenterID = '', gatepassData = {}, isApproved = 0, isRejected = 0, isCancelled = 0, isTagAsReturned = 0, reimburseToEmp = '', disburseToVendor = '';
            var txtGPEmpName = mOut.element.find('#txtGPEmpName');
            var txtGPAssociatedTo = mOut.element.find('#txtGPAssociatedTo');
            var txtGPFromLocation = mOut.element.find('#txtGPFromLocation');
            var txtGPToLocation = mOut.element.find('#txtGPToLocation');
            var txtGPOtherToLocation = mOut.element.find('#txtGPOtherToLocation');
            var txtGPParticulars = mOut.element.find('#txtGPParticulars');
            var txtGPRemarks = mOut.element.find('#txtGPRemarks');
            var selGPRequestedBy = mOut.element.find('#selGPRequestedBy');
            var btnGPNew = mOut.element.find('#btnGPNew');
            var btnGPLookup = mOut.element.find('#btnGPLookup');
            var btnGPSave = mOut.element.find('#btnGPSave');
            var btnGPPrint = mOut.element.find('#btnGPPrint');
            var btnGPActions = mOut.element.find('#btnGPActions');
            var selROGPEmployeeName = mOut.element.find('#selROGPEmployeeName');
            var selROGPFromLocation = mOut.element.find('#selROGPFromLocation');
            var selROGPToLocation = mOut.element.find('#selROGPToLocation');
            
            var selROGPRequestedBy = mOut.element.find('#selROGPRequestedBy');
            var txtGPReturnDate = mOut.element.find('#txtGPReturnDate');

            LoadRequiredData();

            $('#btnGPSettings').unbind('click').click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                PromptUserAccess('Access to Gate Pass Settings', {
                    Message: 'You don\'t have access to Gate Pass Settings',
                    OnClickOKSuccess: function () {
                        $('#tabGPSettings').opentabmenu();
                        LoadSettings();
                    }
                });
            })

            $('#btnGPAddAuthorizer').click(function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Select Approvers', [
                    {
                        AutoSearch: true,
                        Message: 'Note: You can select multiple users.',
                        SelectMultiple: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            AjaxSend({
                                url: 'GatePass/SaveGatePassUsers',
                                data: { data: e, Type: 'approver' },
                                success: function (e) {
                                    if (e.Results == 'saved') {
                                        WebToast('Successfully added', 'top');
                                        mOut.GetGatePassUsers('approver');
                                    } else {
                                        NewMessageBox(e.Error, 'Error');
                                    }
                                }
                            })
                        }
                    }
                ])
            })

            $('#btnGatePassReport').click(function () {
                $('#popupGatePass').modal('show');
                GetGatePasses();
            })

            $('#selGPStatus').change(function () {
                GetGatePasses();
            })

            $('#chbReturnDateNA').change(function () {
                if ($(this).is(':checked')) {
                    txtGPReturnDate.attr('disabled', 'disabled').val('');
                } else {
                    txtGPReturnDate.removeAttr('disabled');
                }
            })

            $('#chbROGPReturnDateNA').change(function () {
                if ($(this).is(':checked')) {
                    $('#txtROGPReturnDate').attr('disabled', 'disabled').val('');
                } else {
                    $('#txtROGPReturnDate').removeAttr('disabled');
                }
            })

            $('#txtROGPSearchReport').keydown(function (e) {
                if (e.which == 13) {
                    GetGatePasses();
                }
            })

            $('#selROGPToLocation').change(function () {
                if ($(this).val() == 'Other Location') {
                    $('#txtROGPOtherToLocation').show();
                } else {
                    $('#txtROGPOtherToLocation').hide();
                }
            })

            txtGPToLocation.change(function () {
                if ($(this).val() == 'Other Location') {
                    txtGPOtherToLocation.val('').show();
                } else {
                    txtGPOtherToLocation.val('').hide();
                }
            })

            txtGPEmpName.click(function () {
                if (gatepassData.Status == 'unapproved' || gatepassData.Status == undefined) {
                    BrowsePopup('LibraryOrganizationEmployee', 'Select Employee (Bearer of Gate Pass)', [{
                        AutoSearch: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            empID = e.ID;
                            txtGPEmpName.val(e.PeopleName);
                        }
                    }])
                }
            })

            selGPRequestedBy.click(function () {
                if (gatepassData.Status == 'unapproved' || gatepassData.Status == undefined) {
                    BrowsePopup('LibraryOrganizationEmployee', 'Requested By', [{
                        Message: 'Please select employee',
                        AutoSearch: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            requestedby = e.ID;
                            selGPRequestedBy.val(e.PeopleName);
                        }
                    }])
                }
            })

            txtGPAssociatedTo.click(function () {
                if (gatepassData.Status == 'unapproved' || gatepassData.Status == undefined) {
                    NewMessageBox('Get data from:', 'Options', [
                    {
                        Text: 'Customer',
                        Size: 2,
                        Click: function () {
                            BrowsePopup('LibraryOrganizationCustomer', 'Select Customer', [
                                {
                                    AutoSearch: true,
                                    DisplayText: [{ Column: 'CustomerName' }],
                                    Click: function (e) {
                                        associatedTo = e.ID;
                                        txtGPAssociatedTo.val(e.CustomerName);
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
                                    AutoSearch: true,
                                    DisplayText: [{ Column: 'SupplierName' }],
                                    Click: function (e) {
                                        associatedTo = e.ID;
                                        txtGPAssociatedTo.val(e.SupplierName);
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
                            txtGPAssociatedTo.val('No Association');
                        }
                    }
                    ])
                }               
            })

            btnGPNew.click(function () {
                ClearFields();
                WebToast('You can now add new Gate Pass', 'top')
            })

            btnGPLookup.click(function () {
                BrowsePopup('GatePass/GetAllGatePasses', 'List Of Gate Passes', [{
                    AutoSearch: true,
                    ServerSideProcessing: true,
                    StyleFormat: 'table',
                    Options: {
                        Fields: [
                            $('<tr>')
                                .append($('<td>')
                                    .append('Show Gate Passes: ')
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .text('Unapproved')
                                            .append($('<input type="radio" name="radGPBrowse" value="unapproved" class="form-check-input" checked>')
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
                                            .text('Approved')
                                            .append($('<input type="radio" name="radGPBrowse" value="approved" class="form-check-input">')
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
                                            .text('Cancelled')
                                            .append($('<input type="radio" name="radGPBrowse" value="cancelled" class="form-check-input">')
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
                                            .text('Returned')
                                            .append($('<input type="radio" name="radGPBrowse" value="returned" class="form-check-input">')
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
                                ShowPass: $('[name="radGPBrowse"]:checked').val()
                            }
                        }
                    },
                    DisplayText: [
                        {
                            Label: 'GP No.', Column: 'ReferenceNo'
                        },
                        {
                            Label: 'Bearer Name', Column: 'PeopleName'
                        },
                        {
                            Label: 'Date Created', Column: function (e) {
                                return $.format.date(ConvertDate(e.DateCreated), 'MM-dd-yyyy hh:mm a')
                            }
                        }
                    ],
                    Click: function (e) {
                        ClearFields();

                        gatepassData = e;
                        gatepassID = e.ID;
                        empID = CStr2(e.PeopleID);
                        requestedby = e.RequestedBy;
                        associatedTo = CStr2(e.AssociatedTo);
                        txtGPAssociatedTo.val(CStr2(e.AssociatedToName))
                        txtGPEmpName.val(e.PeopleName);
                        txtGPFromLocation.attr('disabled', 'disabled').val(e.FromLocation);
                        txtGPToLocation.attr('disabled', 'disabled').val(e.ToLocation || 'Other Location');
                        txtGPOtherToLocation.val(CStr2(e.OtherToLocation));
                        txtGPRemarks.attr('readonly', 'readonly').val(e.Remarks);
                        txtGPParticulars.attr('readonly', 'readonly').val(e.Particulars);
                        selGPRequestedBy.val(e.RequestedByName);

                        if (CStr2(e.ToLocation) == '') {
                            txtGPOtherToLocation.show();
                        } else {
                            txtGPOtherToLocation.hide();
                        }

                        txtGPReturnDate.attr('readonly', 'readonly');
                        $('#chbReturnDateNA').attr('disabled', 'disabled').checkboxradio('refresh');

                        if (CStr2(e.ReturnDate) == '') {
                            $('#chbReturnDateNA').attr('checked', 'checked').checkboxradio('refresh');
                            txtGPReturnDate.val('').attr('disabled', 'disabled');
                        } else {
                            txtGPReturnDate.val($.format.date(ConvertDate(e.ReturnDate), 'yyyy-MM-dd'));
                        }

                        $('#lblGPReferenceNo').text('GP-' + e.ReferenceNo);
                        $('#lblGPEncodedBy').text('Encoded By: ' + e.EncodedByName + ' on ' + $.format.date(ConvertDate(e.DateCreated), 'MM/dd/yyyy hh:mm a'));

                        if (CBool(e.IsApproved) && !CBool(e.IsRejected) && !CBool(e.IsCancelled) && !CBool(e.IsTagAsReturned)) {
                            gatepassData.Status = 'approved';
                            $('#lblGPStatus').text('APPROVED');
                            $('#lblGPOtherInfo').text('Approved By: ' + e.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(e.DateApprovedOrRejectedOrCancelled), 'MM/dd/yyyy hh:mm a'));
                        } else if (!CBool(e.IsApproved) && !CBool(e.IsRejected) && CBool(e.IsCancelled) && !CBool(e.IsTagAsReturned)) {
                            gatepassData.Status = 'cancelled';
                            $('#lblGPStatus').text('CANCELLED');
                            $('#lblGPOtherInfo').text('Cancelled By: ' + e.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(e.DateApprovedOrRejectedOrCancelled), 'MM/dd/yyyy hh:mm a'));
                        } else if (CBool(e.IsApproved) && !CBool(e.IsRejected) && !CBool(e.IsCancelled) && CBool(e.IsTagAsReturned)) {
                            gatepassData.Status = 'returned';
                            $('#lblGPStatus').text('RETURNED');
                            $('#lblGPOtherInfo').text('Approved By: ' + e.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(e.DateApprovedOrRejectedOrCancelled), 'MM/dd/yyyy hh:mm a'))
                                .append('<br>')
                                .append('Returned on ' + $.format.date(ConvertDate(e.ItemReturnedOn), 'MM/dd/yyyy hh:mm a'));
                        } else if (!CBool(e.IsApproved) && !CBool(e.IsRejected) && !CBool(e.IsCancelled) && !CBool(e.IsTagAsReturned)) {
                            txtGPFromLocation.removeAttr('disabled');
                            txtGPToLocation.removeAttr('disabled');
                            txtGPParticulars.removeAttr('readonly');
                            txtGPRemarks.removeAttr('readonly');
                            txtGPReturnDate.removeAttr('readonly');
                            $('#chbReturnDateNA').removeAttr('disabled').checkboxradio('refresh');
                            gatepassData.Status = 'unapproved';
                            $('#lblGPStatus').text('UNAPPROVED');                            
                            if (e.LastUpdatedByName != null) {
                                $('#lblGPOtherInfo').text('Last Updated By: ' + e.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(e.LastUpdated), 'MM/dd/yyyy hh:mm a'));
                            }
                        }
                    }
                }])
            })

            btnGPSave.click(function () {
                if (gatepassData.Status != 'unapproved' && gatepassData.Status != undefined) {
                    NewMessageBox('Sorry! This Gate Pass was already ' + gatepassData.Status + ', modification is not allowed.', 'Message');
                    return;
                }

                var isValid = true;

                $('.form-group').removeClass('has-error');

                if (empID == '') {
                    txtGPEmpName.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(txtGPFromLocation.val()) == '') {
                    txtGPFromLocation.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(txtGPToLocation.val()) == '') {
                    txtGPToLocation.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(txtGPParticulars.val()) == '') {
                    txtGPParticulars.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(requestedby) == '') {
                    selGPRequestedBy.parent().addClass('has-error');
                    isValid = false;
                }

                if ($.trim(txtGPToLocation.val()) == 'Other Location') {
                    if ($.trim(txtGPOtherToLocation.val()) == '') {
                        txtGPOtherToLocation.parent().addClass('has-error');
                        isValid = false;
                    }                    
                }

                if (new Date(txtGPReturnDate.val()) < new Date()) {
                    txtGPReturnDate.parent().addClass('has-error');
                    isValid = false;
                }

                if (!isValid) {
                    WebToast('Please enter required fields (<span class="required">*</span>)', 'top');
                    return;
                }

                var data = {
                    PeopleID: empID,
                    FromLocation: $.trim(txtGPFromLocation.val()),
                    ToLocation: $.trim(txtGPToLocation.val()),
                    OtherToLocation: $.trim(txtGPOtherToLocation.val()),
                    Remarks: $.trim(txtGPRemarks.val()),
                    Particulars: $.trim(txtGPParticulars.val()),
                    AssociatedTo: associatedTo,
                    ID: gatepassID,
                    IsApproved: isApproved,
                    IsRejected: isRejected,
                    IsCancelled: isCancelled,
                    IsTagAsReturned: isTagAsReturned,
                    RequestedBy: requestedby,
                    ReturnDate: txtGPReturnDate.val()
                }

                SaveNow(data);
            })

            btnGPPrint.click(function () {
                if (CStr2(gatepassID) == '') {
                    NewMessageBox('Please select approved Gate Pass', 'Message');
                    return;
                }

                if (gatepassData.Status != 'approved'){
                    NewMessageBox('Only approved Gate Pass can be downloaded', 'Message');
                    return;
                }

                window.location = MergeUrlParameters({
                    ID: gatepassID,
                    OrgID: sessionStorage.OrgID
                }, '/GatePass/DownloadGatePassReport');
            })

            btnGPActions.click(function () {
                var data = {
                    PeopleID: empID,
                    FromLocation: $.trim(txtGPFromLocation.val()),
                    ToLocation: $.trim(txtGPToLocation.val()),
                    OtherToLocation: $.trim(txtGPOtherToLocation.val()),
                    Remarks: $.trim(txtGPRemarks.val()),
                    Particulars: $.trim(txtGPParticulars.val()),
                    AssociatedTo: associatedTo,
                    ID: gatepassID,
                    IsApproved: isApproved,
                    IsRejected: isRejected,
                    IsCancelled: isCancelled,
                    IsTagAsReturned: isTagAsReturned,
                    RequestedBy: requestedby,
                    ReturnDate: txtGPReturnDate.val()
                }

                if (gatepassData != null) {
                    if (gatepassData.Status == 'unapproved') { // UNAPPROVED
                        PopupButtonList('What would you like to do for this Unapproved Gate Pass?', 'Actions', [
                            {
                                Text: 'Approve Gate Pass',
                                Theme: 'b',
                                Size: 2,
                                Click: function () {
                                    CheckIfAuthorizer('approver', function () {
                                        data.IsApproved = 1;
                                        data.IsRejected = 0;
                                        data.IsCancelled = 0;
                                        data.IsTagAsReturned = 0;
                                        SaveNow(data);
                                    })                                 
                                }
                            },
                            {
                                Text: 'Cancel Gate Pass',
                                Theme: 'd',
                                Size: 2,
                                Click: function () {
                                    CheckIfAuthorizer('approver', function () {
                                        data.IsApproved = 0;
                                        data.IsRejected = 0;
                                        data.IsCancelled = 1;
                                        data.IsTagAsReturned = 0;
                                        SaveNow(data);
                                    })
                                }
                            },
                            {
                                Text: 'Maybe next time',
                                Size: 2,
                                Click: function () {

                                }
                            }
                        ])
                    } else if (gatepassData.Status == 'approved') {
                        NewMessageBox('Please select an action', 'Actions', [
                            {
                                Text: 'Tag as Returned',
                                Theme: 'b',
                                Size: 2,
                                Click: function () {
                                    CheckIfAuthorizer('approver', function () {
                                        data.IsApproved = 0; // para dili machange kung when giapproved
                                        data.IsRejected = 0;
                                        data.IsCancelled = 0;
                                        data.IsTagAsReturned = 1;
                                        SaveNow(data);
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
                    } else {
                        NewMessageBox('This feature is only available for Unapproved or Approved Gate Pass', 'Message');
                    }
                } else {
                    NewMessageBox('This feature is only available for Unapproved or Approved Gate Pass', 'Message');
                }
            })

            function GetGatePasses() {
                var tbodyGatePass = $('#tbodyGatePass');
                tbodyGatePass.empty();

                if ($.fn.DataTable.isDataTable("#tblGatePassReport")) {
                    //check if datatable is created 
                    $('#tblGatePassReport').DataTable().clear().destroy();
                }

                var tblGatePassReport = $('#tblGatePassReport').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 25,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            ShowPass: $('#selGPStatus').val(),
                            keywords: $.trim($('#txtROGPSearchReport').val())
                        }, 'GatePass/GetAllGatePasses'),
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum + '.';
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ReferenceNo;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.PeopleName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<div class="line-clamp">' + data.Particulars + '</div>';
                            },
                            "width": "200px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<div class="line-clamp">' + CStr2(data.Remarks) + '</div>';
                            },
                            "width": "200px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.RequestedByName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(ConvertDate(data.DateCreated), 'MM/dd/yyyy hh:mm a');
                            }
                        }
                    ],
                    "createdRow": function (row, data, index) {
                        $(row).data('data', data).click(function () {
                            var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                            gatepassData = data;
                            
                            $('#divContentGPMoreDetails').find('input, select, textarea').attr('disabled', 'disabled');
                            $('#lblROGPEncodedBy').text(data.EncodedByName);
                            $('#lblROGPReferenceNo').text('GP-' + data.ReferenceNo);
                            $('#selROGPEmployeeName').val(data.PeopleID);
                            $('#selROGPFromLocation').val(data.FromLocation);
                            $('#selROGPToLocation').val(data.ToLocation || 'Other Location');
                            $('#txtROGPOtherToLocation').val(CStr2(data.OtherToLocation));
                            $('#lblROGPAssociatedTo').text(data.AssociatedToName || 'N/A');
                            $('#lblROGPParticulars').val(data.Particulars);
                            $('#lblROGPRemarks').val(data.Remarks);
                            $('#selROGPRequestedBy').val(data.RequestedBy);

                            if (CStr2(data.ToLocation) == '') {
                                $('#txtROGPOtherToLocation').show();
                            } else {
                                $('#txtROGPOtherToLocation').hide();
                            }

                            if (CStr2(data.ReturnDate) == '') {
                                $('#chbROGPReturnDateNA').attr('checked', 'checked');
                                $('#txtROGPReturnDate').val('').attr('disabled', 'disabled');
                            } else {
                                $('#chbROGPReturnDateNA').removeAttr('checked');
                                $('#txtROGPReturnDate').val($.format.date(ConvertDate(data.ReturnDate), 'yyyy-MM-dd'));
                            }

                            $('#forROGPTagAsReturned, .forROGPUnapproved').hide();

                            $('#btnROGPTagAsReturned, #btnROGPApprove, #btnROGPCancel').unbind('click');

                            if (CBool(data.IsApproved) && !CBool(data.IsRejected) && !CBool(data.IsCancelled) && !CBool(data.IsTagAsReturned)) {
                                $('#lblROGPStatus').text('APPROVED');
                                $('#lblROGPOtherInfo').text('By: ' + data.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(data.DateApprovedOrRejectedOrCancelled), 'MM/dd/yyyy hh:mm a'));
                                $('#forROGPTagAsReturned').pageshow();                                

                                $('#btnROGPTagAsReturned').click(function () {
                                    CheckIfAuthorizer('approver', function () {
                                        SaveNow({
                                            PeopleID: data.PeopleID,
                                            FromLocation: $.trim($('#selROGPFromLocation').val()),
                                            ToLocation: $.trim($('#selROGPToLocation').val()),
                                            OtherToLocation: $.trim($('#txtROGPOtherToLocation').val()),
                                            Remarks: $.trim($('#lblROGPRemarks').val()),
                                            Particulars: $.trim($('#lblROGPParticulars').val()),
                                            AssociatedTo: CStr2(data.AssociatedTo),
                                            ID: data.ID,
                                            IsApproved: 0,
                                            IsRejected: 0,
                                            IsCancelled: 0,
                                            IsTagAsReturned: 1,
                                            RequestedBy: $.trim($('#selROGPRequestedBy').val()),
                                            ReturnDate: $('#txtROGPReturnDate').val()
                                        });
                                    })
                                })
                            } else if (!CBool(data.IsApproved) && !CBool(data.IsRejected) && CBool(data.IsCancelled) && !CBool(data.IsTagAsReturned)) {
                                $('#lblROGPStatus').text('CANCELLED');
                                $('#lblROGPOtherInfo').text('By: ' + data.ApprovedOrRejectedByName + ' on ' + $.format.date(ConvertDate(data.DateApprovedOrRejectedOrCancelled), 'MM/dd/yyyy hh:mm a'));
                            } else if (CBool(data.IsApproved) && !CBool(data.IsRejected) && !CBool(data.IsCancelled) && CBool(data.IsTagAsReturned)) {
                                $('#lblROGPStatus').text('RETURNED');
                                $('#lblROGPOtherInfo').text('on ' + $.format.date(ConvertDate(data.ItemReturnedOn), 'MM/dd/yyyy hh:mm a'));
                            } else if (!CBool(data.IsApproved) && !CBool(data.IsRejected) && !CBool(data.IsCancelled) && !CBool(data.IsTagAsReturned)) {
                                $('#lblROGPStatus').text('UNAPPROVED');
                                if (data.LastUpdatedByName != null) {
                                    $('#lblROGPOtherInfo').text('Last Updated By: ' + data.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(data.LastUpdated), 'MM/dd/yyyy hh:mm a'));
                                }
                                $('.forROGPUnapproved').pageshow();
                                $('#divContentGPMoreDetails').find('input, select, textarea').removeAttr('disabled');

                                $('#btnROGPApprove').click(function () {
                                    CheckIfAuthorizer('approver', function () {
                                        SaveNow({
                                            PeopleID: data.PeopleID,
                                            FromLocation: $.trim($('#selROGPFromLocation').val()),
                                            ToLocation: $.trim($('#selROGPToLocation').val()),
                                            OtherToLocation: $.trim($('#txtROGPOtherToLocation').val()),
                                            Remarks: $.trim($('#lblROGPRemarks').val()),
                                            Particulars: $.trim($('#lblROGPParticulars').val()),
                                            AssociatedTo: CStr2(data.AssociatedTo),
                                            ID: data.ID,
                                            IsApproved: 1,
                                            IsRejected: 0,
                                            IsCancelled: 0,
                                            IsTagAsReturned: 0,
                                            RequestedBy: $.trim($('#selROGPRequestedBy').val()),
                                            ReturnDate: $('#txtROGPReturnDate').val()
                                        });
                                    })                                    
                                })

                                $('#btnROGPCancel').click(function () {
                                    CheckIfAuthorizer('approver', function () {
                                        SaveNow({
                                            PeopleID: data.PeopleID,
                                            FromLocation: $.trim($('#selROGPFromLocation').val()),
                                            ToLocation: $.trim($('#selROGPToLocation').val()),
                                            OtherToLocation: $.trim($('#txtROGPOtherToLocation').val()),
                                            Remarks: $.trim($('#lblROGPRemarks').val()),
                                            Particulars: $.trim($('#lblROGPParticulars').val()),
                                            AssociatedTo: CStr2(data.AssociatedTo),
                                            ID: data.ID,
                                            IsApproved: 0,
                                            IsRejected: 0,
                                            IsCancelled: 1,
                                            IsTagAsReturned: 0,
                                            RequestedBy: $.trim($('#selROGPRequestedBy').val()),
                                            ReturnDate: $('#txtROGPReturnDate').val()
                                        });
                                    })                                    
                                })
                            }
                            //$('#chbROGPReturnDateNA').checkboxradio('refresh');
                            Bs.NestedModal('#popupGatePass', '#popGatePassMoreDetails');
                        })
                    },
                    "initComplete": function () {
                        $('#tblGatePassReport').removeClass('dataTable');
                    },
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                    "responsive": true,
                    //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })
            }

            function CheckIfAuthorizer(action, callback) {
                AjaxSend({
                    url: 'GatePass/CheckGatePassUserIfHasAccess',
                    data: { UserID: sessionStorage.PeopleID, type: 'approver' },
                    success: function (e) {
                        if (CBool(e.Results)) {
                            callback()
                        } else {
                            NewMessageBox('Sorry! You\'re not allowed to approve, cancel or tag as returned the selected Gate Pass.', 'Message');
                        }
                    }
                })
            }

            function SaveNow(data) {

                AjaxSend({
                    url: 'GatePass/SaveGatePass',
                    data: {
                        data: data
                    },
                    success: function (e) {
                        if (e.Results == 'saved') {
                            var refno = gatepassData == null ? '' : gatepassData.ReferenceNo;
                            GetGatePasses();
                            CountGatePasses();
                            ClearFields();

                            if (CBool(data.IsApproved)) {
                                NewMessageBox('Gate Pass no. <b>' + refno + '</b> has been approved! Would you like to download the report?', 'Message', [
                                    {
                                        Text: 'Yes, download the report now!', Click: function () {
                                            window.location = MergeUrlParameters({
                                                ID: e.ID,
                                                OrgID: sessionStorage.OrgID
                                            }, '/GatePass/DownloadGatePassReport');
                                        }
                                    }, { Text: 'No, maybe later.' }
                                ], {
                                    Close: function () {
                                        $('#popupGatePass').modal('show');
                                    }
                                });
                                return;
                            } 
                            
                            if (CBool(data.IsCancelled)) {
                                NewMessageBox('Gate Pass no. <b>' + refno + '</b> has been cancelled', 'Message', undefined, {
                                    Close: function () {
                                        $('#popupGatePass').modal('show');
                                    }
                                });
                                return;
                            }

                            if (CBool(data.IsTagAsReturned)) {
                                NewMessageBox('Gate Pass no. <b>' + refno + '</b> has been returned', 'Message', undefined, {
                                    Close: function () {
                                        $('#popupGatePass').modal('show');
                                    }
                                });
                                return;
                            }

                            NewMessageBox('Gate Pass has been successfully saved', 'Message', undefined, {
                                Close: function () {
                                    $('#popupGatePass').modal('show');
                                }
                            });
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            }

            function LoadRequiredData() {
                ClearFields();

                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: {
                        IsAll: 1
                    },
                    success: function (e) {
                        var rows = e.Results.Rows;

                        txtGPFromLocation.empty();
                        txtGPToLocation.empty();
                        selROGPFromLocation.empty();
                        selROGPToLocation.empty();

                        txtGPFromLocation.append(new Option('Please select', ''));
                        txtGPToLocation.append(new Option('Please select', ''));
                        selROGPFromLocation.append(new Option('Please select', ''));
                        selROGPToLocation.append(new Option('Please select', ''));

                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++){
                                txtGPFromLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                                txtGPToLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                                selROGPFromLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                                selROGPToLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                            }

                            if (rows.length == 1) {
                                txtGPFromLocation.val(rows[0].ID);
                                txtGPToLocation.val(rows[0].ID);
                                selROGPFromLocation.val(rows[0].ID);
                                selROGPToLocation.val(rows[0].ID);
                            }

                            //selROGPFromLocation.val(sessionStorage.FowstID);
                            //txtGPToLocation.val(sessionStorage.FowstID);
                            //txtGPFromLocation.val(sessionStorage.FowstID);
                            //selROGPToLocation.val(sessionStorage.FowstID);

                            txtGPToLocation.append(new Option('Other Location', 'Other Location'));
                            selROGPToLocation.append(new Option('Other Location', 'Other Location'));
                        }
                    }
                })

                AjaxSend({
                    url: 'Browser/LibraryOrganizationEmployee',
                    success: function (e) {
                        var rows = e.Results.Rows;

                        selGPRequestedBy.empty();
                        selROGPEmployeeName.empty();
                        selROGPRequestedBy.empty();

                        selGPRequestedBy.append(new Option('Please select', ''));
                        selROGPEmployeeName.append(new Option('Please select', ''));
                        selROGPRequestedBy.append(new Option('Please select', ''));

                        if (rows.length > 0) {
                            for (var i = 0; i < rows.length; i++) {
                                selGPRequestedBy.append(new Option(rows[i].PeopleName, rows[i].ID));
                                selROGPEmployeeName.append(new Option(rows[i].PeopleName, rows[i].ID));
                                selROGPRequestedBy.append(new Option(rows[i].PeopleName, rows[i].ID));
                            }
                        }
                    }
                })

                CountGatePasses();
            }

            function ClearFields() {
                gatepassData = {};
                gatepassID = '';
                empID = '';
                associatedTo = '';
                requestedby = '';
                isApproved = 0;
                isRejected = 0;
                isCancelled = 0;
                txtGPAssociatedTo.val('');                
                txtGPEmpName.val('');
                txtGPFromLocation.removeAttr('disabled').val('');
                txtGPToLocation.removeAttr('disabled').val('');
                txtGPParticulars.removeAttr('readonly').val('');
                txtGPRemarks.removeAttr('readonly').val('');
                selGPRequestedBy.val('');
                txtGPOtherToLocation.val('').hide();
                txtGPReturnDate.removeAttr('disabled readonly').attr('min', $.format.date(new Date(), 'yyyy-MM-dd')).val($.format.date(new Date(new Date().setDate(new Date().getDate() + 7)), 'yyyy-MM-dd'));
                $('#lblGPReferenceNo').text('NEW');
                $('#lblGPStatus, #lblGPEncodedBy, #lblGPOtherInfo').text('');
                $('#chbReturnDateNA').removeAttr('checked disabled').checkboxradio('refresh');
            }

            function LoadSettings() {
                mOut.GetGatePassUsers('approver');
            }

            function ConvertDate(date) {
                if (isNaN(new Date(date))) {
                    date = new Date(parseInt(date.substr(6)));
                } else {
                    date = new Date(date);
                }

                return date;
            }

            function CountGatePasses() {
                AjaxSend({
                    url: 'GatePass/CountGatePasses',
                    success: function (e) {
                        $('#lblCountUnapprovedGP').html('<b>' + CInt2(e.Unapproved) + '</b>');
                        $('#lblCountApprovedGP').html('<b>' + CInt2(e.Approved) + '</b>');
                        $('#lblCountCancelledGP').html('<b>' + CInt2(e.Cancelled) + '</b>');
                        $('#lblCountReturnedGP').html('<b>' + CInt2(e.Returned) + '</b>');
                    }
                })
            }

            var tblGPMakers;
            mOut.GetGatePassUsers = function (type) {
                var tbl = '#tblGPApprover';
                //$('#tbodyGPMakers').empty()

                if ($.fn.DataTable.isDataTable(tbl)) {
                    //check if datatable is created 
                    $(tbl).DataTable().clear().destroy();
                }

                tblGPMakers = $(tbl).DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 10,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/GatePass/GetGatePassUsers?OrgID=" + sessionStorage.OrgID + '&type=approver',
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
                                $(td).text($.format.date(new Date(parseInt(rowData.DateAdded.substr(6))), 'MMM dd, yyyy hh:mm a'));
                            }
                        },
                        {
                            "data": null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                $(td).text(rowData.AddedByName);
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
                                                                    url: 'GatePass/RemoveGatePassUser',
                                                                    data: { ID: data.ID, type: type },
                                                                    success: function (e) {
                                                                        if (e.Results == 'deleted') {
                                                                            WebToast('Successfully removed', 'top');
                                                                            mOut.GetGatePassUsers(type);
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

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })
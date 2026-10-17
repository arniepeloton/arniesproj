define([''], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ComplianceReminders/work/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ComplianceReminders.work.controller = ERP.ComplianceReminders.work.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ComplianceReminders.work.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabComplianceRemindersWork').append(mElement).find('> .divspinner').remove();

        var id = '';
        var txtCRSearch = mElement.find('#txtCRSearch');
        var btnCRAddAlert = mElement.find('#btnCRAddAlert');
        var selCRComplianceOrg = mElement.find('#selCRComplianceOrg');
        var txtCROrgOthers = mElement.find('#txtCROrgOthers');
        var txtCRTaxType = mElement.find('#txtCRTaxType');
        var txtCRFormType = mElement.find('#txtCRFormType');
        var txtCRFilingStartDate = mElement.find('#txtCRFilingStartDate');
        var selCRFilingFrequency = mElement.find('#selCRFilingFrequency');
        var txtCRFilingDueDateDescription = mElement.find('#txtCRFilingDueDateDescription');
        var selCRFilingDueDateMonth = mElement.find('#selCRFilingDueDateMonth');
        var selCRFilingDueDateDay = mElement.find('#selCRFilingDueDateDay');
        var selCRFilingWeekly = mElement.find('#selCRFilingWeekly');
        var selCRFilingMonthly = mElement.find('#selCRFilingMonthly');
        var selCRFilingQuarterlyDay = mElement.find('#selCRFilingQuarterlyDay');
        var txtCRComplianceTaskTitle = mElement.find('#txtCRComplianceTaskTitle');
        var txtCRComplianceDescription = mElement.find('#txtCRComplianceDescription');
        var txtCRComplianceDueDateDescription = mElement.find('#txtCRComplianceDueDateDescription');
        var selCRComplianceFrequency = mElement.find('#selCRComplianceFrequency');
        var selCRComplianceDueDateMonth = mElement.find('#selCRComplianceDueDateMonth');
        var selCRComplianceDueDateDay = mElement.find('#selCRComplianceDueDateDay');
        var selCRComplianceWeekly = mElement.find('#selCRComplianceWeekly');
        var selCRComplianceMonthly = mElement.find('#selCRComplianceMonthly');
        var btnCRSaveAlert = mElement.find('#btnCRSaveAlert');

        btnCRAddAlert.click(function () {
            ClearFields();
            $('#popupCRAlert').modal('show');
        })

        selCRComplianceOrg.change(function () {
            var val = $(this).val();
            if (val == 'BIR') {
                $('#divCRForBIR').show();
                $('#divCRForOthers').hide();
            } else {
                $('#divCRForBIR').hide();
                $('#divCRForOthers').show();
            }

            txtCROrgOthers.val('');
            if (val == 'Others') {
                $('#divCROrgOther').show();
            } else {
                $('#divCROrgOther').hide();
            }
        })

        btnCRSaveAlert.click(function () {
            var isValid = true, pFields = null, complianceOrg = selCRComplianceOrg.val();
            mElement.find('input, textarea, select').removeClass('is-invalid');

            if (complianceOrg == '') {
                isValid = false;
                selCRComplianceOrg.addClass('is-invalid');
            } else if (complianceOrg == 'BIR') {
                pFields = mElement.find('#divCRForBIR').find('input, textarea, select:not(#divFilingFrequencyOpts select)');
            } else {
                pFields = mElement.find('#divCRForOthers').find('input, textarea, select:not(#divComplianceFrequencyOpts select)');
            }

            if (pFields != null)
                pFields.each(function () {
                    var dis = $(this);
                    if (dis.val().trim() == '') {
                        isValid = false;
                        dis.addClass('is-invalid');
                    }
                })

            if (complianceOrg == 'Others' && txtCROrgOthers.val().trim() == '') {
                isValid = false;
                txtCROrgOthers.addClass('is-invalid');
            }            

            var duedate, freq;
            if (complianceOrg == 'BIR') {
                freq = selCRFilingFrequency.val();

                if (freq == 'Annually') {
                    if (selCRFilingDueDateMonth.val() == '') {
                        isValid = false;
                        selCRFilingDueDateMonth.addClass('is-invalid');
                    }

                    if (selCRFilingDueDateDay.val() == '') {
                        isValid = false;
                        selCRFilingDueDateDay.addClass('is-invalid');
                    }

                    duedate = selCRFilingDueDateMonth.val() + ' ' + selCRFilingDueDateDay.val();
                } else if (freq == 'Weekly') {
                    if (selCRFilingWeekly.val() == '') {
                        isValid = false;
                        selCRFilingWeekly.addClass('is-invalid');
                    }

                    duedate = selCRFilingWeekly.val();
                } else {
                    if (selCRFilingMonthly.val() == '') {
                        isValid = false;
                        selCRFilingMonthly.addClass('is-invalid');
                    }

                    duedate = selCRFilingMonthly.val();
                }
            } else {
                freq = selCRComplianceFrequency.val();

                if (freq == 'Annually') {
                    if (selCRComplianceDueDateMonth.val() == '') {
                        isValid = false;
                        selCRComplianceDueDateMonth.addClass('is-invalid');
                    }

                    if (selCRComplianceDueDateDay.val() == '') {
                        isValid = false;
                        selCRComplianceDueDateDay.addClass('is-invalid');
                    }

                    duedate = selCRComplianceDueDateMonth.val() + ' ' + selCRComplianceDueDateDay.val();
                } else if (freq == 'Weekly') {
                    if (selCRComplianceWeekly.val() == '') {
                        isValid = false;
                        selCRComplianceWeekly.addClass('is-invalid');
                    }

                    duedate = selCRComplianceWeekly.val();
                } else {
                    if (selCRComplianceMonthly.val() == '') {
                        isValid = false;
                        selCRComplianceMonthly.addClass('is-invalid');
                    }

                    duedate = selCRComplianceMonthly.val();
                }
            }

            if (!isValid) {
                WebToast('Please fill out all fields marked with an <span class="required">*</span>', 'top');
                return
            }

            Bs.Loading('show');
            AjaxSend({
                url: '/Alerts/SaveComplianceReminders',
                data: {
                    data: {
                        ID: id,
                        ComplianceOrg: complianceOrg,
                        OtherComplianceOrg: txtCROrgOthers.val(),
                        TaxType: txtCRTaxType.val(),
                        FormType: txtCRFormType.val(),
                        FilingStartDate: txtCRFilingStartDate.val(),
                        FilingDueDateDescription: txtCRFilingDueDateDescription.val(),
                        FilingDueDate: duedate,
                        FilingFrequency: freq,
                        ComplianceTaskTile: txtCRComplianceTaskTitle.val(),
                        ComplianceFrequency: freq,
                        ComplianceDescription: txtCRComplianceDescription.val(),
                        ComplianceDueDateDescription: txtCRComplianceDueDateDescription.val(),
                        ComplianceDueDate: duedate
                    }
                }, success: function (e) {
                    Bs.Loading('hide');
                    
                    if (e.Results != undefined) {
                        WebToast('Successfully saved', 'top');
                        $('#popupCRAlert').modal('hide');
                        GetComplianceReminders();
                    } else {
                        NewMessageBox(e.Error, 'Error');
                    }
                }
            })
        })

        txtCRSearch.keydown(function (e) {
            if (e.which == 13) {
                GetComplianceReminders();
            }
        })

        selCRFilingFrequency.change(function () {
            mElement.find('#divFilingFrequencyOpts > div').hide();
            var val = $(this).val();
            if (val == 'Weekly') {
                selCRFilingWeekly.parent().show();
            } else if (val == 'Monthly') {
                selCRFilingMonthly.parent().show();
            } else if (val == 'Quarterly') {
                selCRFilingQuarterlyDay.parent().parent().show();
            } else {
                selCRFilingDueDateMonth.parent().show();
            }
        })

        selCRComplianceFrequency.change(function () {
            mElement.find('#divComplianceFrequencyOpts > div').hide();
            var val = $(this).val();
            if (val == 'Weekly') {
                selCRComplianceWeekly.parent().show();
            } else if (val == 'Monthly') {
                selCRComplianceMonthly.parent().show();
            } else {
                selCRComplianceDueDateMonth.parent().show();
            }
        })

        function GetComplianceReminders() {
            mElement.find('tbody').empty();
            var tbl = mElement.find('table');

            if ($.fn.DataTable.isDataTable(tbl)) {
                //check if datatable is created 
                tbl.DataTable().clear().destroy();
            }

            tbl.DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 10,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        keywords: $.trim(txtCRSearch.val())
                    }, '/Alerts/GetComplianceReminders'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        title: '#',
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "30px"
                    },
                    {
                        title: 'Compliance Organization',
                        "data": function (data, type, full, meta) {
                            var str;

                            if (data.ComplianceOrg == 'Others') {
                                str = data.OtherComplianceOrg;
                            } else {
                                str = data.ComplianceOrg;
                            }

                            return str;
                        }
                    },
                    {
                        title: 'Description',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.TaxType) + '' + CStr2(data.ComplianceTaskTile);
                        }
                    },
                    {
                        title: 'Updated By',
                        "data": function (data, type, full, meta) {
                            return data.UpdatedByName;
                        }
                    },
                    {
                        title: 'Last Updated',
                        "data": function (data, type, full, meta) {
                            return $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yy hh:mm a');
                        }, className: 'text-center'
                    }
                ],
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = JSON.parse(JSON.stringify(($(this).data('data'))));                        

                        ClearFields();

                        id = data.ID;
                        selCRComplianceOrg.val(data.ComplianceOrg);
                        txtCROrgOthers.val(CStr2(data.OtherComplianceOrg));
                        txtCRTaxType.val(CStr2(data.TaxType));
                        txtCRFormType.val(CStr2(data.FormType));
                        txtCRFilingStartDate.val($.format.date(CJsonDate(data.FilingStartDate), 'yyyy-MM-dd'));

                        if (data.ComplianceOrg == 'BIR') {
                            selCRFilingFrequency.val(CStr2(data.Frequency));
                            txtCRFilingDueDateDescription.val(data.Description);                            
                            if (data.Frequency == 'Annually') {
                                var duedate = data.DueDate.split(' ');
                                selCRFilingDueDateMonth.val(duedate[0]).parent().show();
                                selCRFilingDueDateDay.val(duedate[1]);
                            } else if (data.Frequency == 'Monthly') {
                                selCRFilingMonthly.val(data.DueDate).parent().show();
                            } else {
                                selCRFilingWeekly.val(data.DueDate).parent().show();
                            }
                        } else {
                            selCRComplianceFrequency.val(CStr2(data.Frequency));
                            txtCRComplianceTaskTitle.val(CStr2(data.ComplianceTaskTile));
                            txtCRComplianceDescription.val(data.ComplianceDescription);
                            txtCRComplianceDueDateDescription.val(data.Description);
                            if (data.Frequency == 'Annually') {
                                var duedate = data.DueDate.split(' ');
                                selCRComplianceDueDateMonth.val(duedate[0]).parent().show();
                                selCRComplianceDueDateDay.val(duedate[1]);
                            } else if (data.Frequency == 'Monthly') {
                                selCRComplianceMonthly.val(data.DueDate).parent().show();
                            } else {
                                selCRComplianceWeekly.val(data.DueDate).parent().show();
                            }
                        }

                        if (data.ComplianceOrg == 'BIR') {
                            $('#divCRForBIR').show();
                            $('#divCRForOthers').hide();
                        } else {
                            $('#divCRForBIR').hide();
                            $('#divCRForOthers').show();
                        }

                        if (data.ComplianceOrg == 'Others') {
                            $('#divCROrgOther').show();
                        } else {
                            $('#divCROrgOther').hide();
                        }

                        $('#popupCRAlert').modal('show');
                    })
                },
                "initComplete": function () {
                    tbl.removeClass('dataTable');
                    tbl.find('thead tr').addClass('align-top text-center').css('font-size', '14px')
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function ClearFields() {
            id = '';
            mElement.find('input, textarea, select').removeClass('is-invalid').val('');
            mElement.find('#divCROrgOther, #divCRForBIR, #divCRForOthers, #divFilingFrequencyOpts > div, #divComplianceFrequencyOpts > div').hide();
        }

        function Initialize() {
            var sel = mElement.find('.CRday');
            sel.append(new Option('Day', ''))
            for (var i = 0; i < 31; i++) {
                var day = i + 1;
                sel.append(new Option(day, day))
            }

            sel = mElement.find('.CRweekday');
            var weekdays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
            for (var i = 0; i < weekdays.length; i++) {
                sel.append(new Option(weekdays[i], (i + 1)))
            }

            sel = mElement.find('.CRdayofmonth');
            for (var i = 0; i < 31; i++) {
                var day = i + 1;
                var lbl;

                if (day == 1) {
                    lbl = 'st ';
                } else if (day == 2) {
                    lbl = 'nd ';
                } else {
                    lbl = 'th ';
                }

                lbl = day + lbl + 'day';

                selCRFilingQuarterlyDay.append(new Option(lbl, day))

                lbl = day + lbl + 'of the month';

                sel.append(new Option(lbl, day))
            }

            GetComplianceReminders();
        }

        Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
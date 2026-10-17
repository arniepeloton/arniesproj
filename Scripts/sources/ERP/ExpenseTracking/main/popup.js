require(['common',
    'promise!/Scripts/PrintPDF/jspdf.min.js',
    'promise!/Scripts/jquery.filedownload.min.js',
    'moment'], function () {
    require(['promise!/Scripts/PrintPDF/jspdf.plugin.autotable.js'], function () {
        var employeeid = sessionStorage.PeopleID, expenseFromDate = '', expenseToDate = '', checkinFromDate = '', checkinToDate = '', statusExpenseTicket = '';
        var isFromUnfinishedExpense = 0, rsParentID = '', rsID = '', rsEditDay = '', rsEmployeeID = null, rsLocationToVisit = null, rsBranchOfSelectedLocation = null;
        var arrDayName = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
        var hasWeeklySched = false, hasMonthlySched = false;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/ExpenseTracking/main/popup.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#divErpDisbursement').append(vData)//.trigger('create');

                    LoadRequiredData();
                }
            });
        })();

        window.ExpenseTracking = {
            GetLogExpenseByMonth: GetLogExpenseByMonth,
            CountUnfinishedExpenseReport: CountUnfinishedExpenseReport,
            CountUnapprovedExpenseTicket: CountUnapprovedExpenseTicket,
            GetLogExpenseHistory: GetLogExpenseHistory,
            RouteSchedule: function (data, isView) {
                ClearEmployeeSchedule();

                if (CStr2(data) != '') {
                    Bs.Loading('show');
                    rsEmployeeID = data.EmployeeID;
                    rsParentID = CStr2(data.ParentID);
                    $('#txtRSEmployee').val(data.EmployeeName);
                    AjaxSend({
                        url: '/Expenses/GetEmployeeRouteSchedules',
                        data: { EmployeeID: data.EmployeeID },
                        success: function (e) {
                            Bs.Loading('hide');
                            var vSched = e.Schedules.Rows, vInfo = e.Info.Rows, vSchedFrequency = 'monthly';

                            if (vInfo.length > 0) {
                                vSchedFrequency = (vInfo[0].Frequency || 'monthly');
                                $('#txtRSEmployee').val(vInfo[0].EmployeeName);
                                $('#txtRSRouteName').val(vInfo[0].RouteName);
                                $('#txtRSRouteDescription').val(CStr2(vInfo[0].RouteDescription));
                                $('[name="radRSSchedFreq"][value="' + vSchedFrequency + '"]').attr('checked', 1);
                                $('[name="radRSSchedType"][value="' + CInt2(vInfo[0].StrictlyFollowSched) + '"]').attr('checked', 1);
                            }                           

                            if (vSched.length > 0) {
                                for (var i = 0; i < vSched.length; i++) {
                                    rsEmployeeID = vSched[i].EmployeeID;

                                    if (vSchedFrequency == 'weekly') {
                                        DisplayWeeklySched(vSched[i]);
                                    }

                                    DisplaySchedule(vSched[i]);
                                }

                                $('#popEmployeeRouteSchedule').modal('show');
                            } else {
                                NewMessageBox('No route schedules found');
                            }
                            
                            (CBool(isView) ? $('.RSDeleteSched').remove() : '');
                        }
                    })
                } else {
                    $('#popEmployeeRouteSchedule').modal('show');
                }

                var txtRSEmployee = $('#txtRSEmployee').unbind('click');

                if (CBool(isView)) {
                    $('#popEmployeeRouteSchedule .modal-footer, #btnRSAdd').hide();
                } else {
                    $('#popEmployeeRouteSchedule .modal-footer, #btnRSAdd').show();
                    txtRSEmployee.click(function () {
                        var dis = $(this);
                        BrowsePopup('LibraryOrganizationEmployee', 'Search Employee', [{
                            DisplayText: [{ Column: 'PeopleName' }],
                            Click: function (e) {
                                rsEmployeeID = e.ID;
                                dis.val(e.PeopleName);
                            }
                        }])
                    })
                }
            }
        }

        $('body')
        .on('click', '.btnLogExpenseReport', function () {
            $('#popupLogExpenseReport').modal('show');
            $('#tabExpenses').opentabmenu();
            GetEmployeeTransData();
        })
        .on('click', '#btnExpenseUnfinishedReport', function () {
            $('#popupUnapprovedExpenseTicket').modal('show');
            $('#selFilterTickets').val('unfinished');
            GetUnapprovedExpenseTicket();
            GetCostCenterLocation();
        })
        .on('click', '#btnExpenseUnapprovedTicket', function () {
            $('#popupUnapprovedExpenseTicket').modal('show');
            $('#selFilterTickets').val('unapproved');
            $('#selExpenseEmployee').val('na');
            GetUnapprovedExpenseTicket();
            GetCostCenterLocation();
        })
        .on('click', '#btnLogExpenseDateRange', function () {
            DualCalendar({
                MaxFromDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                MaxToDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                Click: function (e) {
                    expenseFromDate = e.From;
                    expenseToDate = e.To;
                    $('#btnLogExpenseDateRange').text(expenseFromDate + ' to ' + expenseToDate);
                    GetEmployeeTransData();
                }
            })
        })
        .on('click', '#btnCheckinDateRange', function () {
            DualCalendar({
                MaxFromDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                MaxToDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                Click: function (e) {
                    checkinFromDate = e.From;
                    checkinToDate = e.To;
                    $('#btnCheckinDateRange').text(checkinFromDate + ' to ' + checkinToDate);
                    GetExpenseCheckInOutByEmployee();
                }
            })
        })
        .on('click', '#btnExpenseCheckInOutReport', function () {
            $('#tabCHUnplannedRoute').opentabmenu();
            $('#popupExpenseCheckInOutReport').modal('show').on('shown.bs.modal', function () {
                document.getElementById($.format.date(new Date(), "MM-dd-yyyy")).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                $('#popupExpenseCheckInOutReport').unbind('shown.bs.modal');
            })
            GetExpenseCheckInOutByEmployee();
        })
        .on('change', '#selLogExpenseType, #selLogExpenseMonthCovered, #selLogExpenseYearCovered, #selExpenseCostCenter, #selExpenseCostCenterLocation', function () {
            GetEmployeeTransData();
        })
        .on('change', '#selExpenseCheckInOutMonthCovered, #selExpenseCheckInOutYearCovered, #selExpenseCheckInOutEmployee', function () {
            if ($('#btnCHUnplannedRoute').hasClass('active')) {
                GetExpenseCheckInOutByEmployee();
            } else {
                GetRouteSchedulesReportByEmployeeV2();
            }
        })
        .on('click', '#btnLogExpenseEmployee', function () {
            BrowsePopup('LibraryOrganizationEmployee', 'Select Employee', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'PeopleName' }],
                    Click: function (e) {
                        $('#btnLogExpenseEmployee').text(e.PeopleName);
                        employeeid = e.ID;
                        GetLogExpenseByEmployee();
                    }
                }
            ])
        })
        .on('change', '#selExpenseEmployee', function () {
            employeeid = $(this).val();
            GetEmployeeTransData();
        })
        .on('click', '#btnDownloadReportByEmployee', function () {
            $('#popExpenseFundDownloadOption').modal('show');
        })
        .on('click', '#btnExpenseReportDLSeparate', function () {
            DownloadExpenseReports('separate');
        })
        .on('click', '#btnExpenseReportDLSingle', function () {
            DownloadExpenseReports('single');
        })
        .on('click', '#btnFundReportDL', function () {
            $.fileDownload(MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                PeopleID: employeeid,
                FromDate: expenseFromDate,
                ToDate: expenseToDate,
            }, '/Expenses/DownloadFundReports'), {
                prepareCallback: function () {
                    NewMessageBox('Preparing your report. Please wait ...');
                },
                successCallback: function () {
                    NewMessageBoxClose();
                },
                failCallback: function (responseHtml, url, error) {
                    var res = JSON.parse(responseHtml).d.Results;
                    NewMessageBox(res, 'Message');
                }
            });
        })
        .on('click', '#btnDownloadCheckInReportByEmployee', function () {
            $.fileDownload(MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                PeopleID: $('#selExpenseCheckInOutEmployee').val(),
                FromDate: checkinFromDate,
                ToDate: checkinToDate,
            }, '/Expenses/DownloadCheckinReports'), {
                prepareCallback: function () {
                    Bs.Loading('show', 'Preparing your report. Please wait ...');
                },
                successCallback: function () {
                    Bs.Loading('hide');
                },
                failCallback: function (responseHtml, url, error) {
                    var res = JSON.parse(responseHtml).d.Results;
                    NewMessageBox(res, 'Message');
                }
            });
        })
        .on('click', '#btnViewEmployeeRouteSchedule', function () {
            var employeeID = $('#selExpenseCheckInOutEmployee').val();

            if (employeeID == '') {
                NewMessageBox('Please select employee');
                return;
            }

            ExpenseTracking.RouteSchedule({ EmployeeID: employeeID }, true);
        })
        .on('click', '#btnRefreshDisbursementDashboard', function () {
            GetLogExpenseByMonth();
            GetFundByMonth();
            CountUnfinishedExpenseReport();
            CountUnapprovedExpenseTicket();
            GetOpenPurchaseOrders();
        })
        .on('click', '#btnAddExpense', function () {
            $('#popLogExpenseMoreDetails').modal('show');
        })
        .on('change', '#selExpensesSummaryMonthCovered, #selExpensesSummaryYearCovered', function (e) {
            GetEmployeeTransData();
        })
        .on('click', '#btnViewDetailsBeginningBalance', function () {
            if (IsGuid($('#selExpenseEmployee').val())) {
                $('#divDetailForBeginningBalance').modal('show');
                $('.lblEmployeeName').text((CStr2(data.EmployeeName) == '' ? '' : 'of ' + $('#selExpenseEmployee option:selected').text()));
                $('.lblDetailMonthCovered').text($.format.date(new Date($('#selExpensesSummaryMonthCovered').val() + ' 1 ' + $('#txtExpensesSummaryYearCovered').val()), 'MMMM yyyy'))
            }
        })
        .on('click', '#btnViewDetailsExpenses', function () {
            if (IsGuid($('#selExpenseEmployee').val())) {
                $('#divDetailForExpenses').modal('show');
                $('.lblEmployeeName').text((CStr2(data.EmployeeName) == '' ? '' : 'of ' + $('#selExpenseEmployee option:selected').text()));
                $('.lblDetailMonthCovered').text($.format.date(new Date($('#selExpensesSummaryMonthCovered').val() + ' 1 ' + $('#txtExpensesSummaryYearCovered').val()), 'MMMM yyyy'))
            }
        })
        .on('click', '#radRFCoEmployee', function () {
            $('#selReceiveFundFromCoEmployee').removeAttr('disabled');
            $('#txtReceiveFundFromOther').attr('disabled', true).val('');
        })
        .on('click', '#radRFOther', function () {
            $('#txtReceiveFundFromOther').removeAttr('disabled');
            $('#selReceiveFundFromCoEmployee').attr('disabled', true).val('');
        })
        .on('click', '#btnyMonthCoveredPrev', function () {
            NavigateDate('prev');
        })
        .on('click', '#btnyMonthCoveredNext', function () {
            NavigateDate('next');
        })
        .on('change', '#txtExpensesSummaryYearCovered', function () {
            GetEmployeeTransData();
        })
        .on('click', '#divForAdjustedNetFundBalance', function () {
            GetAdjustedFundHistory({
                Month: $('#selExpensesSummaryMonthCovered').val(),
                Year: $('#txtExpensesSummaryYearCovered').val()
            });
        })
        .on('click', '#divForAdjustedCarryOverFundBalance', function () {
            var month = $('#selExpensesSummaryMonthCovered').val(), year = $('#txtExpensesSummaryYearCovered').val();
            
            if (month == 1) {
                month = 12;
                year = year - 1;
            } else {
                month = month - 1;
            }

            GetAdjustedFundHistory({
                Month: month,
                Year: year
            });
        })
        .on('click', '#btnAdjustCarryOverFund', function () {
            $('#lblCOFEmployeeName').text($('#selExpenseEmployee option:selected').text());
            $('.lblCOFPeriod').text($.format.date(new Date(new Date().setDate(0)), 'MMMM yyyy'));
            $('#selCOFFowst').val('');
            $('#lblCOFRemainingFund').text(CDec2($('#lblCarryOverFund').text()));
            $('#txtCOFAdjustedAmount').val('');
            $('#txtCOFRemarks').val('');
            $('#popCarryOverFund').modal('show');
        })
        .on('click', '#btnCOFSave', function (e) {
            e.preventDefault();
            e.stopPropagation();

            var selCOFFowst = $('#selCOFFowst').val();
            var txtCOFAdjustedAmount = $('#txtCOFAdjustedAmount').val();
            var txtCOFRemarks = $('#txtCOFRemarks').val();

            if (selCOFFowst == '') {
                WebToast('Please select branch to be adjusted', 'top');
                return;
            }

            if ($.trim(txtCOFAdjustedAmount) == '') {
                WebToast('Please enter adjusted amount', 'top');
                return;
            }

            if ($.trim(txtCOFRemarks) == '') {
                WebToast('Briefly describe your fund adjustment', 'top');
                return;
            }

            AjaxSend({
                url: '/Expenses/SaveCarryOverFund',
                data: {
                    data: {
                        EmployeeID: $('#selExpenseEmployee').val(),
                        Month: new Date(new Date().setDate(0)).getMonth() + 1,
                        Year: new Date(new Date().setDate(0)).getFullYear(),
                        FowstID: selCOFFowst,
                        OriginalFund: CDec2($('#lblCarryOverFund').text()),
                        AdjustedAmount: txtCOFAdjustedAmount,
                        Remarks: txtCOFRemarks
                    }
                }, success: function (e) {
                    if (e.Results == 'saved') {
                        $('#popCarryOverFund').modal('hide');
                        WebToast('Fund adjustment has been saved', 'top');
                        GetFundAdjustmentByEmployee();
                        GetNetFundBalanceOfEmployee();
                    } else {
                        NewMessageBox(e.Results, 'Error');
                    }
                }
            })
        })
        .on('change', '#selFilterTickets', function () {
            GetUnapprovedExpenseTicket();
        })
        .on('keydown', '#txtSearchExpenseTickets', function (e) {
            if (e.which == 13) {
                GetUnapprovedExpenseTicket();
            }            
        })
        .on('click', '#btnDownloadStoreVisitedReport', function () {
            $('#selSVRFromMonth, #selSVRToMonth').val(new Date().getMonth() + 1);
            $('#txtSVRFromYear, #txtSVRToYear').attr('max', new Date().getFullYear()).val(new Date().getFullYear());
            $('[name="chbSVREmployee"]').removeAttr('checked');//.checkboxradio('refresh');
            $('#popStoreVisitedReport').modal('show');
        })
        .on('click', '#btnSVRSelectAll', function () {
            $('[name="chbSVREmployee"]').prop('checked', 1);//.checkboxradio('refresh');
        })
        .on('click', '#btnSVRDeselectAll', function () {
            $('[name="chbSVREmployee"]').removeAttr('checked');//.checkboxradio('refresh');
        })
        .on('click', '#btnDownloadSVR', function () {
            var from = $('#selSVRFromMonth').val() + '/1/' + $('#txtSVRFromYear').val();
            var to = $('#selSVRToMonth').val() + '/1/' + $('#txtSVRToYear').val();
            var arrEmployeeID = [];

            if (new Date(from) > new Date(to)) {
                WebToast('Invalid date range', 'top');
                return;
            }

            $('[name="chbSVREmployee"]:checked').each(function () {
                arrEmployeeID.push($(this).attr('value'));
            })

            if (arrEmployeeID.length == 0) {
                WebToast('Please check at least one employee', 'top');
                return;
            }

            NewMessageBox('Please wait while generating the report');

            AjaxSend({
                url: '/Expenses/DownloadStoreVisitedReports',
                data: {
                    data: {
                        Start: from,
                        End: to,
                        Employees: arrEmployeeID
                    }
                }, success: function (e) {
                    if (e.success != undefined) {
                        $.fileDownload(MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            name: e.success
                        }, '/Expenses/DownloadSVRNow'), {
                            prepareCallback: function () {
                                NewMessageBox('Preparing your report. Please wait ...');
                            },
                            successCallback: function () {
                                NewMessageBox('Please save the generated file. Thank you!');
                            },
                            failCallback: function (responseHtml, url, error) {
                                var res = JSON.parse(responseHtml).d.Results;
                                NewMessageBox(res, 'Message');
                            }
                        });
                    } else {
                        NewMessageBox(e.error, 'Error');
                    }
                }
            })
        })
        .on('click', '#btnRSCopySchedule', function () {
            ValidateRouteSchedule(function () {
                function SchedList(list, daynumber, weeknumber) {
                    var emp = list.find('a.empty'), ctrSched = 1;

                    if (emp.length > 0) {
                        return;
                    }

                    emp.remove();

                    msg += '======  ' + (weeknumber == undefined ? '' : 'WEEK ' + weeknumber + ' - ') + (daynumber == 'any' ? 'ANY DAY' : arrDayName[daynumber].toUpperCase()) + '  ======\n\n';

                    list.find('a').each(function () {
                        var data = JSON.parse(JSON.stringify($(this).data('data')));

                        //para makuha ang duration
                        data.CheckIn = '/Date(' + new Date('1/1/2022 ' + data.StartTime).getTime() + ')/';
                        data.CheckOut = '/Date(' + new Date('1/1/2022 ' + data.EndTime).getTime() + ')/';

                        msg += ctrSched + '.\n'
                        msg += 'Company\t: ' + data.LocationName + '\n';
                        msg += 'Outlet\t: ' + data.BranchName + '\n';

                        if (CStr2(data.StartTime) != '' && CStr2(data.EndTime) != '') {
                            msg += 'Time\t: ' + $.format.date(new Date('1/1/2022 ' + data.StartTime), 'hh:mm a') + ' to ' + $.format.date(new Date('1/1/2022 ' + data.EndTime), 'hh:mm a') + '\n';
                            msg += 'Duration\t: ' + GetTimeDuration(data) + '\n';
                        }
                        
                        msg += '\n';
                        ctrSched += 1;
                    })
                }
                var msg = '';

                msg += 'Emp. Name\t: ' + $('#txtRSEmployee').val().trim();
                msg += '\nRoute Name\t: ' + $('#txtRSRouteName').val().trim();
                msg += '\nRoute Desc\t: ' + $('#txtRSRouteDescription').val().trim();
                msg += '\nFrequency\t: ' + ($('[name="radRSSchedFreq"]:checked').val() == 'monthly' ? 'Monthly' : 'Weekly');
                msg += '\nSched Type\t: ' + (CBool($('[name="radRSSchedType"]:checked').val()) ? 'Fixed' : 'Flexible');
                msg += '\n\n';
                msg += 'Your Planned Route Schedule';
                msg += '\n\n';

                if ($('[name="radRSSchedFreq"]:checked').val() == 'weekly') {
                    for (var w = 0; w <= 4; w++) {
                        if ($('#tabRSWeek' + w).length == 0) continue;

                        SchedList($('#listRSSchedWeek'+ w +'-any'), 'any', w);

                        for (var i = 0; i <= 6; i++) {
                            var list = $('#listRSSchedWeek' + w + '-' + i);
                            SchedList(list, i, w);
                        }
                    }
                } else {
                    SchedList($('#listRSSchedany'), 'any');

                    for (var i = 0; i <= 6; i++) {
                        var list = $('#listRSSched' + i);
                        SchedList(list, i);
                    }
                }                

                $('#txtRSCopyToClipboard').val(msg).select();
                document.execCommand("copy");
                WebToast('Copied to clipboard');
            })
        })
        .on('click', '#btnRSAdd', function () {
            ClearSchedule();
            $('#popRouteSchedule').modal('show');

            $('#btnRSScheduleOk').unbind('click').click(function () {
                PopulateSchedule();
            })
        })        
        .on('click', '#txtRSLocationToVisit', function () {
            var dis = $(this);
            BrowsePopup('LibraryOrganizationCustomer', 'Location to visit', [{
                DisplayText: [{ Column: 'CustomerName' }, { Column: 'LastUpdated', Label: 'Last Updated' }],
                Click: function (e) {
                    if (rsLocationToVisit != e.ID) {
                        rsBranchOfSelectedLocation = null;
                        $('#txtRSBranch').text('Search Branch');
                    }

                    rsLocationToVisit = e.ID;
                    dis.text(e.CustomerName);

                    if (rsBranchOfSelectedLocation == null) {
                        $('#txtRSBranch').click();
                    }
                }
            }])
        })
        .on('click', '#txtRSBranch', function () {
            var dis = $(this);

            if (rsLocationToVisit == null) {
                WebToast('Search first the location to visit', 'top');
                return;
            }

            BrowsePopup('/OrganizationShort/GetCustomerOrgOutlets', 'Branch of ' + $('#txtRSLocationToVisit').text(), [{
                DisplayText: [{ Column: 'Name' }],
                ReturnVariable: 'Results',
                Click: function (e) {
                    rsBranchOfSelectedLocation = e.ID;
                    dis.text(e.Name);
                }
            }], { ID: rsLocationToVisit })
        })
        .on('click', '.RSSetTime', function () {
            var dis = $(this);

            CustomSetTime({
                SetTime: CStr2(dis.val()),
                Click: function (e) {
                    dis.val(e.time);
                }
            })
        })
        .on('click', '#btnRSSave', function () {
            var monthlyscheds = $('#listRSEmployeeMonthlySchedules a:not(.empty)'), employeeSched = [];
            var weeklyscheds = $('.weeklyscheds a:not(.empty)')
            var txtRSRouteName = $('#txtRSRouteName').val().trim();
            var txtRSRouteDescription = $('#txtRSRouteDescription').val().trim();
            var switchRSStrictlyFollowSched = $('[name="radRSSchedType"]:checked').val();
            var radRSSchedFreq = $('[name="radRSSchedFreq"]:checked').val();

            ValidateRouteSchedule(function () {
                monthlyscheds.each(function () {
                    var dis = $(this), data = dis.data('data');
                    data.EmployeeID = rsEmployeeID;
                    data.ModifiedBy = sessionStorage.PeopleID;
                    data.IsDeleted = dis.hasClass('deleted');
                    employeeSched.push(data);
                })

                weeklyscheds.each(function () {
                    var dis = $(this), data = dis.data('data');
                    data.EmployeeID = rsEmployeeID;
                    data.ModifiedBy = sessionStorage.PeopleID;
                    data.IsDeleted = dis.hasClass('deleted');
                    employeeSched.push(data);
                })
                //console.log(employeeSched)
                //return
                Bs.Loading('show');

                AjaxSend({
                    url: '/Expenses/SaveRouteSchedules',
                    data: {
                        data: {
                            ParentID: CStr2(rsParentID),
                            EmployeeID: rsEmployeeID,
                            RouteName: txtRSRouteName,
                            RouteDescription: txtRSRouteDescription,
                            ModifiedBy: sessionStorage.PeopleID,
                            Schedules: employeeSched,
                            StrictlyFollowSched: switchRSStrictlyFollowSched,
                            Frequency: radRSSchedFreq
                        }
                    },
                    success: function (e) {
                        Bs.Loading('hide');

                        if (e.Results != undefined) {
                            WebToast('New schedule has been successfully saved', 'top');
                            $('#popEmployeeRouteSchedule').modal('hide');
                            ERP.ExpenseTracking.main.controller.GetRouteSchedules();
                        } else {
                            NewMessageBox(e.Error, 'Error');
                        }
                    }
                })
            })
        })
        .on('change', '[name="radRSSchedFreq"]', function () {
            if ($(this).val() == 'weekly') {
                $('#divRSSchedFreqWeekly').show();
            } else {
                $('#divRSSchedFreqWeekly').hide();
            }
            $('#selRSSchedFreqWeek').val('');
        })
        .on('change', '#switchRSSetTime', function () {
            if ($(this).is(':checked')) {
                $('#divRSSetTime').show();
            } else {
                $('#divRSSetTime').hide();
            }
            $('#txtRSStartTime, #txtRSEndTime').val('');
        })
        .on('keydown', '#txtExpenseCheckInOutReportSearch', function (e) {
            if (e.which == 13) {
                GetExpenseCheckInOutByEmployee();
            }
        })
        .on('change', '#selExpenseCheckInOutDateRangeMonth, #selExpenseCheckInOutDateRangeYear', function () {            
            GetRouteSchedulesReportByEmployeeV2();
        })
        .on('click', '#btnCHv2PlannedRoute', function () {
            GetRouteSchedulesReportByEmployeeV2();
        })
        .on('click', '#btnCHUnplannedRoute', function () {
            GetExpenseCheckInOutByEmployee();
        })

        function ValidateRouteSchedule(callback) {
            var monthlyscheds = $('#listRSEmployeeMonthlySchedules a:not(.empty)');
            var weeklyscheds = $('.weeklyscheds a:not(.empty)');
            var txtRSRouteName = $('#txtRSRouteName').val().trim();
            var txtRSRouteDescription = $('#txtRSRouteDescription').val().trim();

            if (rsEmployeeID == null) {
                WebToast('Please select employee', 'top');
                return;
            }

            if (txtRSRouteName == '') {
                WebToast('Please enter route name', 'top');
                return;
            }

            if (monthlyscheds.length == 0 && weeklyscheds.length == 0) {
                WebToast('Please add at least one schedule', 'top');
                return;
            }

            callback();
        }

        function PopulateSchedule(callback) {
            var radRSSchedFreq = $('[name="radRSSchedFreq"]:checked').val();
            var selRSSchedFreqWeek = $('#selRSSchedFreqWeek').val();
            var txtRSLocationToVisit = $('#txtRSLocationToVisit').text();
            var txtRSBranch = $('#txtRSBranch').text();
            var chbRSDay = $('[name="chbRSDay"]:checked');
            var txtRSStartTime = $('#txtRSStartTime').val();
            var txtRSEndTime = $('#txtRSEndTime').val();
            var selRSTag = $('#selRSTag option:selected').val();

            if (radRSSchedFreq == 'weekly' && selRSSchedFreqWeek == '') {
                WebToast('Please select week', 'top');
                return;
            }

            if (rsLocationToVisit == null || txtRSLocationToVisit == '') {
                WebToast('Please select location to visit', 'top');
                return;
            }

            if (rsBranchOfSelectedLocation == null || txtRSBranch == '') {
                WebToast('Please select branch of selected location', 'top');
                return;
            }            

            if ($('#switchRSSetTime').is(':checked')) {
                if (txtRSStartTime == '' || txtRSEndTime == '') {
                    WebToast('Please enter time duration', 'top');
                    return;
                }

                if (!(new Date('1/1/2022 ' + txtRSEndTime) > new Date('1/1/2022 ' + txtRSStartTime))) {
                    WebToast('Invalid time duration', 'top');
                    return;
                }
            }

            function ValidateDay(day) {
                var isInvalidNewStart = false, isInvalidNewEnd = false;
                var monthlyscheds = $('#listRSSched' + day + ' a:not(.empty):not(.editing)');
                var weeklyscheds = $('#listRSSchedWeek' + selRSSchedFreqWeek + '-' + day + ' a:not(.empty):not(.editing)');

                function ValidateTime(data) {
                    if (CStr2(txtRSStartTime) == '' && CStr2(txtRSEndTime) == '') {
                        return true;
                    }

                    var start = new Date('1/1/2022 ' + data.StartTime);
                    var end = new Date('1/1/2022 ' + data.EndTime);
                    var newStart = new Date('1/1/2022 ' + txtRSStartTime);
                    var newEnd = new Date('1/1/2022 ' + txtRSEndTime);

                    isInvalidNewStart = start <= newStart && end >= newStart;
                    isInvalidNewEnd = start <= newEnd && end >= newEnd;

                    if (isInvalidNewStart || isInvalidNewEnd) return false;
                }

                monthlyscheds.each(function () {
                    var data = $(this).data('data');
                    ValidateTime(data);
                })

                weeklyscheds.each(function () {
                    var data = $(this).data('data');
                    ValidateTime(data);
                })

                if (isInvalidNewStart || isInvalidNewEnd) {
                    NewMessageBox('New ' + arrDayName[day] + ' schedule is conflict with an existing schedule.');
                    return false;
                }

                var data = {
                    ID: (rsEditDay == day ? rsID : ''),
                    Frequency: radRSSchedFreq,
                    Week: selRSSchedFreqWeek,
                    LocationID: rsLocationToVisit,
                    LocationName: txtRSLocationToVisit,
                    FowstID: rsBranchOfSelectedLocation,
                    BranchName: txtRSBranch,
                    Day: day,
                    StartTime: txtRSStartTime,
                    EndTime: txtRSEndTime,
                    Tag: selRSTag
                }

                if (radRSSchedFreq == 'weekly') {
                    DisplayWeeklySched(data);
                }

                DisplaySchedule(data);

                return true;
            }

            var isValid = true;

            if (chbRSDay.length == 0) {
                isValid = ValidateDay('any');
            }

            chbRSDay.each(function () {
                isValid = ValidateDay($(this).val())
                return isValid;
            })

            if (!isValid) return;

            if (typeof callback == 'function') {
                callback();
            }

            $('#popRouteSchedule').modal('hide');
        }        

        function DisplayWeeklySched(data) {
            var divRSWeeklyTab = $('#divRSWeeklyTab');

            if (divRSWeeklyTab.find('li button[id="btnWeek' + data.Week + '"]').length == 0) {
                divRSWeeklyTab.find('ul').append($('<li class="nav-item">')
                        .append($('<button data-role="none" class="nav-link" data-toggle="tab" id="btnWeek' + data.Week + '" data-target="#tabRSWeek' + data.Week + '">').text('Week ' + data.Week))
                    )
                divRSWeeklyTab.next().append($('<div class="tab-pane fade" id="tabRSWeek' + data.Week + '">'))

                var tab = $('#tabRSWeek' + data.Week), ul = $('<ul data-role="none" class="list-group list-group-flush weeklyscheds">');

                ul.append($('<li class="list-group-item mb-3">')
                            .append($('<h6 class="mb-3">').text('Any Day'))
                            .append($('<div data-role="none" class="list-group mb-3" id="listRSSchedWeek' + data.Week + '-any">')
                                .append('<a href="#" class="list-group-item list-group-item-action empty">No schedules</a>')
                            )
                        )

                for (var i = 0; i < 7; i++) {
                    ul.append($('<li class="list-group-item mb-3">')
                            .append($('<h6 class="mb-3">').text(arrDayName[i]))
                            .append($('<div data-role="none" class="list-group mb-3" id="listRSSchedWeek'+ data.Week +'-'+ i +'">')
                                .append('<a href="#" class="list-group-item list-group-item-action empty">No schedules</a>')
                            )
                        )
                }

                tab.append(ul)
            }

            if (!divRSWeeklyTab.find('li button').hasClass('active')) {
                divRSWeeklyTab.find('li:first-child button').click();
            }
        }

        function DisplaySchedule(data) {
            var vDay = (data.Day || 'any');
            var list = ((data.Frequency || 'monthly') == 'monthly' ? $('#listRSSched' + vDay) : $('#listRSSchedWeek' + data.Week + '-' + vDay));
            var key = data.LocationID + data.FowstID + data.StartTime + data.EndTime;
            list.find('a.empty').remove();

            //para makuha ang duration
            data.CheckIn = '/Date(' + new Date('1/1/2022 ' + data.StartTime).getTime() + ')/';
            data.CheckOut = '/Date(' + new Date('1/1/2022 ' + data.EndTime).getTime() + ')/';

            list.append($('<a href="#" class="list-group-item list-group-item-action">')
                    .data('data', data)
                    .append($('<div class="row">')
                        .append($('<div class="col">')
                            .html('<i>Company:</i> ' + data.LocationName + ' <br><i>Outlet:</i> ' + data.BranchName)
                        )
                        .append($('<div class="col-auto">')
                            .html((CStr2(data.StartTime) != '' && CStr2(data.EndTime) != '' ? $.format.date(new Date('1/1/2022 ' + data.StartTime), 'hh:mm a') + ' to ' + $.format.date(new Date('1/1/2022 ' + data.EndTime), 'hh:mm a') + '<br>' +
                                '= ' + GetTimeDuration(data) : ''))
                        )
                        .append($('<div class="col-auto">')
                            .append($('<button data-role="none" class="btn btn-danger RSDeleteSched">')
                                .data('data', data)
                                .append('<span class="bi bi-trash"></span>')
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var dis = $(this), data = dis.data('data');

                                    NewPromptUserAccess('Allow to add/edit/delete Employee\'s Route Schedule', {
                                        Message: 'You\'re not allowed to delete schedule.',
                                        OnClickOKSuccess: function () {
                                            NewMessageBox('Are you sure you want to remove this schedule? If Yes, it cannot be undone', 'Confirmation Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        var a = dis.parents().closest('a');
                                                        if (data.ID == '') {
                                                            a.remove();
                                                        } else {
                                                            a.addClass('deleted').hide();
                                                        }
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        }
                                    });                                    
                                })
                            )
                        )
                    )
                    .click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        var dis = $(this), data = dis.data('data');

                        NewPromptUserAccess('Allow to add/edit/delete Employee\'s Route Schedule', {
                            Message: 'You\'re not allowed to edit Route Schedule.',
                            OnClickOKSuccess: function () {
                                ClearSchedule();

                                dis.addClass('editing');
                                rsID = CStr2(data.ID);
                                rsEditDay = (data.Day || 'any');
                                rsLocationToVisit = data.LocationID;
                                rsBranchOfSelectedLocation = data.FowstID;
                                if (CStr2(data.Frequency || 'monthly') == 'monthly') {
                                    $('#radRSSchedFreqMonthly').attr('checked', 1);
                                    $('#divRSSchedFreqWeekly').hide();
                                    $('#selRSSchedFreqWeek').val('');
                                } else {
                                    $('#radRSSchedFreqWeekly').attr('checked', 1);
                                    $('#selRSSchedFreqWeek').val(CStr2(data.Week));
                                }

                                $('#txtRSLocationToVisit').text(data.LocationName);
                                $('#txtRSBranch').text(data.BranchName);

                                if (rsEditDay != 'any') $('[name="chbRSDay"][value="' + data.Day + '"]').attr('checked', 1);

                                if (CStr2(data.StartTime) != '' && CStr2(data.EndTime) != '') {
                                    $('#switchRSSetTime').attr('checked', 1);
                                    $('#divRSSetTime').show();
                                    $('#txtRSStartTime').val($.format.date(new Date('1/1/2022 ' + data.StartTime), 'hh:mm a'));
                                    $('#txtRSEndTime').val($.format.date(new Date('1/1/2022 ' + data.EndTime), 'hh:mm a'));
                                }
                                
                                $('#selRSTag').val(CStr2(data.Tag));

                                $('#popRouteSchedule').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                    dis.removeClass('editing');
                                    rsEditDay = '';
                                    $('#popRouteSchedule').unbind('hidden.bs.modal');
                                });

                                $('#btnRSScheduleOk').unbind('click').click(function () {
                                    PopulateSchedule(function () {
                                        dis.remove();
                                    });
                                })
                            }
                        });                        
                    })
                )

            if ((data.Frequency || 'monthly') == 'monthly') {
                hasMonthlySched = true;
                $('#tabRSSchedFreqMonthly').opentabmenu()
            }

            if ((data.Frequency || 'monthly') == 'weekly') {
                hasWeeklySched = true;
                $('#tabRSSchedFreqWeekly').opentabmenu()
            }

            if (hasMonthlySched && hasWeeklySched) {
                $('#divRSTab').hide();
            }
        }

        function ClearSchedule() {            
            rsID = '';
            rsEditDay = '';
            rsLocationToVisit = null;
            rsBranchOfSelectedLocation = null;
            $('#txtRSLocationToVisit').text('Search Location');
            $('#txtRSBranch').text('Search Branch');
            $('[name="chbRSDay"], #switchRSSetTime').removeAttr('checked');
            $('#txtRSStartTime, #txtRSEndTime, #selRSTag, #selRSSchedFreqWeek').val('');            
            $('#divRSSetTime').hide();

            if ($('[name="radRSSchedFreq"]:checked').val() == 'monthly') {
                $('#divRSSchedFreqWeekly').hide();
            } else {
                $('#divRSSchedFreqWeekly').show();
            }
        }

        function ClearEmployeeSchedule() {
            rsEmployeeID = null;
            rsParentID = '';
            $('#popEmployeeRouteSchedule').find('input:not([type="radio"], [type="checkbox"]), textarea').val('');
            $('#listRSEmployeeMonthlySchedules div.list-group').empty().append('<a href="#" class="list-group-item list-group-item-action empty">No schedules</a>');
            $('#tabRSSchedFreqWeekly').find('ul:eq(0), .tab-content').empty();
            $('#radRSSchedTypeFixed, #radRSSchedFreqMonthly').attr('checked', 1);
            $('#divRSTab').hide();
        }

        function GetAdjustedFundHistory(data) {
            AjaxSend({
                url: '/Expenses/GetAdjustedFundHistory',
                data: {
                    data: {
                        EmployeeID: $('#selExpenseEmployee').val(),
                        Month: data.Month,
                        Year: data.Year,
                        FowstID: $('#selExpenseCostCenterLocation').val()
                    }
                }, success: function (e) {
                    var vRows = e.Results.Rows;
                    var tbodyAdjustedFundHistory = $('#tbodyAdjustedFundHistory');

                    tbodyAdjustedFundHistory.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            tbodyAdjustedFundHistory.append($('<tr>')
                                    .append($('<td>').text(i + 1 + '.'))
                                    .append($('<td>').text(vRows[i].FowstName))
                                    .append($('<td>').text($.format.date(new Date(vRows[i].MonthCovered + '-1-' + vRows[i].YearCovered), 'MMMM yyyy')))
                                    .append($('<td>').text(AccountingFormat(vRows[i].OriginalAmount)))
                                    .append($('<td>').text(AccountingFormat(vRows[i].AdjustedAmount)))
                                    .append($('<td>').text($.format.date(new Date(vRows[i].RequestedDate), 'MM-dd-yyyy hh:mm a')))
                                    .append($('<td>').text(vRows[i].Remarks))
                                    .append($('<td>').text(vRows[i].Status + ' on ' + $.format.date(new Date(vRows[i].LastUpdated), 'MM-dd-yyyy hh:mm a') + ' by ' + vRows[i].UpdatedBy))
                                    .append($('<td>').text(vRows[i].Status == 'Approved' ? AccountingFormat(vRows[i].NetFundBalance) : ''))
                                )
                        }
                    } else {
                        tbodyAdjustedFundHistory.append($('<tr>')
                                    .append($('<td colspan="9">').text('No results found'))
                                )
                    }

                    $('#divAdjustedFundHistory').modal('show');
                    $('.lblEmployeeName').text((CStr2(data.EmployeeName) == '' ? '' : 'of ' + $('#selExpenseEmployee option:selected').text()));
                }
            })
        }

        function NavigateDate(nav) {
            var selExpensesSummaryMonthCovered = $('#selExpensesSummaryMonthCovered');
            var txtExpensesSummaryYearCovered = $('#txtExpensesSummaryYearCovered');
            var curMonth = CInt2(selExpensesSummaryMonthCovered.val());
            var curYear = CInt2(txtExpensesSummaryYearCovered.val());

            if (nav == 'prev') {
                curMonth -= 1;

                if (curMonth == 0) {
                    curMonth = 12; // go to december
                    curYear -= 1; // go to prev year
                    txtExpensesSummaryYearCovered.val(curYear);
                }

                selExpensesSummaryMonthCovered.val(curMonth);
            } else {
                curMonth += 1;

                if (curMonth == 13) {
                    curMonth = 1; // go to january
                    curYear += 1; // go to next year
                    txtExpensesSummaryYearCovered.val(curYear);
                }

                selExpensesSummaryMonthCovered.val(curMonth);
            }
            GetEmployeeTransData();
        }

        function GetNetFundBalanceOfEmployee() {
            if (IsGuid($('#selExpenseEmployee').val())) {
                AjaxSend({
                    url: '/Expenses/GetNetFundBalanceOfEmployee',
                    data: { EmployeeID: $('#selExpenseEmployee').val(), Month: $('#selExpensesSummaryMonthCovered').val(), Year: $('#txtExpensesSummaryYearCovered').val() },
                    success: function (e) {
                        var carryover = e.CarryOverFund.Rows;
                        var expenses = e.Expenses.Rows;
                        var receivedfunds = e.ReceivedFunds.Rows;
                        var adjustednetfundbalance = e.CurrentMonthAdjustedNetFundBalance.Rows;
                        var adjustedcarryoverfundbalance = e.CarryOverNetFundBalance.Rows;
                        var totalReceiveFunds = 0, carryoverfund = 0, totalbegfundbal = 0, totalexpenses = 0, netfund = 0;
                        var tbodyReceivedFundsDetails = $('#tbodyReceivedFundsDetails');
                        var tbodyExpensesDetails = $('#tbodyExpensesDetails');

                        tbodyReceivedFundsDetails.empty();
                        tbodyExpensesDetails.empty();

                        PopulateFundByEmployee('#tblDetailForBeginningBalance', {
                            data: receivedfunds,
                            "bdestroy": true,
                            paging: false
                        })

                        if (receivedfunds.length > 0) {                            
                            for (var i = 0; i < receivedfunds.length; i++) {
                                totalReceiveFunds += CDec2(receivedfunds[i].Amount);
                                //tbodyReceivedFundsDetails.append($('<tr>')
                                //    .append($('<td>').text(i + 1 + '.'))
                                //    .append($('<td>').text(receivedfunds[i].ReceiveFromBy))
                                //    .append($('<td>').text($.format.date(new Date(receivedfunds[i].DateCreated), 'MM/dd/yyyy hh:mm a')))
                                //    .append($('<td style="text-align:right;">').text(AccountingFormat(receivedfunds[i].Amount)))
                                //)
                            }
                            //$('#lblReceivedFundsDetailsTotal').text(AccountingFormat(totalReceiveFunds));
                        } else {
                            $('#tblDetailForBeginningBalance').find('.tfFooterFund').text('Total: -');
                        }

                        if (carryover.length > 0) {
                            for (var i = 0; i < carryover.length; i++) {
                                carryoverfund += CDec2(carryover[i].AdjustedAmount || carryover[i].NetFundBalance);
                            }
                        }

                        if ($.fn.DataTable.isDataTable("#tblDetailForExpenses")) {
                            //check if datatable is created 
                            $('#tblDetailForExpenses').DataTable().clear().destroy();
                        }

                        var tblDetailForExpenses = $('#tblDetailForExpenses').DataTable({
                            "bdestroy": true,
                            paging: false,
                            "order": [[1, "asc"]],
                            "data": expenses,
                            "autoWidth": false,
                            "columns": LogExpenseRow(false),
                            "createdRow": function (row, data, index) {
                                $(row).data('data', data).click(function () {
                                    var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                                    data.IsFinishedLogExpense = true;
                                    EditLogExpense(data);
                                })
                            },
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                            "responsive": true,
                            //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">'
                            //"stateSave": true
                        })

                        if (expenses.length > 0) {                            
                            for (var i = 0; i < expenses.length; i++) {
                                var data = expenses[i];
                                totalexpenses += CDec2(data.Amount);
                            }
                            $('#lblExpensesDetailsTotal').text(AccountingFormat(totalexpenses));
                        } else {
                            $('#lblExpensesDetailsTotal').text(AccountingFormat(0));
                        }

                        totalbegfundbal = totalReceiveFunds + carryoverfund;
                        netfund = totalbegfundbal - totalexpenses;

                        $('.lblTotalReceiveFundsHeader').text(AccountingFormat(totalReceiveFunds));
                        $('#lblCarryOverFund').text(AccountingFormat(carryoverfund));
                        $('#lblTotalBegFundBal').text(AccountingFormat(totalbegfundbal));
                        $('.lblTotalExpenseHeader').text(AccountingFormat(totalexpenses));
                        $('#lblNetFundBalance').text(AccountingFormat(netfund));

                        if (adjustednetfundbalance.length > 0) {
                            $('#lblNetFundBalance').text(AccountingFormat(adjustednetfundbalance[0].NetFundBalance));
                            $('#divForAdjustedNetFundBalance').show();
                        } else {
                            $('#divForAdjustedNetFundBalance').hide();
                        }

                        if (adjustedcarryoverfundbalance.length > 0) {
                            $('#lblCarryOverFund').text(AccountingFormat(adjustedcarryoverfundbalance[0].NetFundBalance));
                            $('#divForAdjustedCarryOverFundBalance').show();
                        } else {
                            $('#divForAdjustedCarryOverFundBalance').hide();
                        }
                    }
                })
            } else {
                $('.lblTotalReceiveFundsHeader').text(AccountingFormat(0));
                $('#lblCarryOverFund').text(AccountingFormat(0));
                $('#lblTotalBegFundBal').text(AccountingFormat(0));
                $('.lblTotalExpenseHeader').text(AccountingFormat(0));
                $('#lblNetFundBalance').text(AccountingFormat(0));
                $('#divForAdjustedNetFundBalance').hide();
                $('#divForAdjustedCarryOverFundBalance').hide();
            }
        }

        function GetEmployeeTransData() {
            GetLogExpenseByEmployee();
            GetFundByEmployee('#tblFundReport');
            GetFundAdjustmentByEmployee();
            GetCostCenterLocation();
            GetNetFundBalanceOfEmployee();
        }

        function GetExpenseDetailsForDownload() {
            AjaxSend({
                url: '/Expenses/GetExpenseDetailsForDownload',
                data: {
                    OrgID: sessionStorage.OrgID,
                    PeopleID: employeeid,
                    MonthCovered: $('#selLogExpenseMonthCovered').val(),
                    YearCovered: $('#selLogExpenseYearCovered').val()
                },
                success: function (e) {
                    var vRowsHeader = $.merge([
                        { ExpenseType: '#' },
                        { ExpenseType: 'From' },
                        { ExpenseType: 'To' }
                    ], e.Header.Rows);
                    var headerPos = [];
                    var vRowsBody = e.Body.Rows;
                    var thExpenseHeader = $('#thExpenseHeader');
                    var tbExpenseBody = $('#tbExpenseBody');
                    var expenseHeaderStyle = {};

                    thExpenseHeader.empty();
                    tbExpenseBody.empty();

                    for (var i = 0; i < vRowsHeader.length; i++) {
                        thExpenseHeader.append($('<th>').text(vRowsHeader[i].ExpenseType))
                        headerPos.push(vRowsHeader[i].ExpenseType);
                    }

                    thExpenseHeader.append($('<th>').text('Remarks'))

                    for (var i = 0; i < vRowsBody.length; i++) {
                        tbExpenseBody.append($('<tr>')
                                .attr('id', vRowsBody[i].ID)
                                .append($('<td>').text(i + 1))
                                .append($('<td>').text(vRowsBody[i].FromLocation))
                                .append($('<td>').text(vRowsBody[i].ToLocation))
                            )
                    }

                    tbExpenseBody.find('tr').each(function () {
                        var dis = $(this);

                        for (var i = 0; i < e.Header.Rows.length; i++) {
                            dis.append($('<td style="text-align:right;">'));
                            expenseHeaderStyle[i + 3] = { halign: 'right' };
                        }

                        dis.append($('<td>'))
                    })

                    for (var i = 0; i < vRowsBody.length; i++) {
                        tbExpenseBody.find('tr[id="' + vRowsBody[i].ID + '"]').find('td:eq(' + headerPos.indexOf(vRowsBody[i].ExpenseType) + ')').text(AccountingFormat(vRowsBody[i].Amount))
                        tbExpenseBody.find('tr[id="' + vRowsBody[i].ID + '"]').find('td:last').text(vRowsBody[i].Remarks)
                    }

                    generate(expenseHeaderStyle);
                }
            })
        }

        function DownloadExpenseReports(format) {
            $.fileDownload(MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                PeopleID: employeeid,
                FromDate: expenseFromDate,
                ToDate: expenseToDate,
                Format: format
            }, '/Expenses/DownloadExpenseReports'), {
                prepareCallback: function () {
                    NewMessageBox('Preparing your report. Please wait ...');
                },
                successCallback: function () {
                    NewMessageBoxClose();
                },
                failCallback: function (responseHtml, url, error) {
                    var res = JSON.parse(responseHtml).d.Results;
                    NewMessageBox(res, 'Message');
                }
            });
        }

        function generate(expenseHeaderStyle) {

            var doc = new jsPDF('p', 'pt');

            var res = doc.autoTableHtmlToJson(document.getElementById("tblExpenseReport"));

            var pageHeight = doc.internal.pageSize.height || doc.internal.pageSize.getHeight();
            var pageWidth = doc.internal.pageSize.width || doc.internal.pageSize.getWidth();

            var header = function (data) {
                doc.setFontSize(18);
                doc.setTextColor(40);
                doc.setFontStyle('normal');
                //doc.addImage(headerImgData, 'JPEG', data.settings.margin.left, 20, 50, 50);
                doc.text("Expense Report", pageWidth - 170, 50, 'right');
                //doc.addText('Expense Report', { textAlign: 'right'})

                doc.setFontSize(12);
                doc.text("Employee Name: " + $('#btnLogExpenseEmployee').text(), data.settings.margin.left, 75); //
            };

            var options = {
                beforePageContent: header,
                margin: {
                    top: 95
                },
                startY: 95,
                styles: { "overflow": "linebreak", "cellWidth": "wrap", "rowPageBreak": "avoid", "halign": "justify" },
                columnStyles: expenseHeaderStyle
                //drawCell: function (cell, data) {
                //    if (data.column.dataKey == 3) {
                //        doc.autoTableText(cell.text, cell.x + cell.width / 2, cell.y + cell.height / 2, {
                //            halign: 'left',
                //            valign: 'middle'
                //        });
                //    } else {
                //        doc.autoTableText(cell.text, cell.textPos.x, cell.y + cell.height / 2, {
                //            halign: 'left',
                //            valign: 'middle'
                //        });
                //    }
                //    return false;
                //},
            };

            //doc.autoTable(res.columns, res.data, { margin: { top: 80 } });               

            doc.autoTable(res.columns, res.data, options);

            doc.save("table.pdf");
        }

        function LoadRequiredData() {
            expenseFromDate = $.format.date(new Date(), 'MMM 01, yyyy');
            expenseToDate = $.format.date(new Date(), 'MMM dd, yyyy');
            checkinFromDate = $.format.date(new Date(), 'MMM dd, yyyy');
            checkinToDate = $.format.date(new Date(), 'MMM dd, yyyy');
            $('#btnLogExpenseEmployee').text(sessionStorage.CurrentUserFullName);
            $('#btnExpenseCheckInOutEmployee').text(sessionStorage.CurrentUserFullName);            
            $('#btnLogExpenseDateRange').text(expenseFromDate + ' to ' + expenseToDate);
            $('#btnCheckinDateRange').text(checkinFromDate + ' to ' + checkinToDate);
            $('#selExpensesSummaryMonthCovered').val(new Date().getMonth() + 1);
            $('#txtExpensesSummaryYearCovered').val(new Date().getFullYear());
            setMonthCollection('selExpenseCheckInOutDateRangeMonth');
            setYearCollection('selExpenseCheckInOutDateRangeYear');
            $('#selExpenseCheckInOutDateRangeMonth').val(new Date().getMonth() + 1);
            $('#selExpenseCheckInOutDateRangeYear').val(new Date().getFullYear());
            
            GetAllFOWST();
            
            AjaxSend({
                url: '/Expenses/GenerateDefaultCostCenter',
                success: function (e) {
                    if (e.Error != undefined) {
                        NewMessageBox(e.Error, 'Error');
                    }

                    GetCostCenters();
                }
            })

            for (var i = 2019; i <= new Date().getFullYear() ; i++) {
                $('#selExpensesSummaryYearCovered').prepend(new Option(i, i));
            }

            $('#selExpensesSummaryYearCovered').val(new Date().getFullYear());

            if (location.pathname.toLowerCase() == '/erp/expensetracking') {
                require(['promise!/Scripts/sources/ERP/ExpenseTracking/main/controller.js?crc=' + assetversion], function () {
                    $('#divErpExpenseTracking, #divETMainDiv').pageshow();
                })
            }
        }

        function GetAllFOWST() {
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: {
                    IsAll: 1
                },
                success: function (e) {
                    var selExpenseCostCenterLocation = $('#selExpenseCostCenterLocation');
                    var rows = e.Results.Rows;

                    selExpenseCostCenterLocation.empty();

                    selExpenseCostCenterLocation.append(new Option('All Cost Center Locations', 'all'));

                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            selExpenseCostCenterLocation.append(new Option(rows[i].FowstName, rows[i].ID));
                        }

                        selExpenseCostCenterLocation.val(sessionStorage.FowstID);
                    }
                }
            })
        }

        function GetListOfEmployee() {
            AjaxSend({
                url: 'Browser/LibraryOrganizationEmployee',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var selExpenseEmployee = $('#selExpenseEmployee');
                    var selExpenseCheckInOutEmployee = $('#selExpenseCheckInOutEmployee');
                    var selReceiveFundFromCoEmployee = $('#selReceiveFundFromCoEmployee');

                    selExpenseEmployee.empty();
                    selExpenseCheckInOutEmployee.empty();
                    selReceiveFundFromCoEmployee.empty();

                    if (vRows.length > 0) {
                        selExpenseEmployee.append(new Option('Not Applicable', 'na'))
                        selExpenseEmployee.append(new Option('All Employees', 'all'))
                        selExpenseCheckInOutEmployee.append(new Option('All Employees', ''))
                        selReceiveFundFromCoEmployee.append(new Option('Please select Co-Employee', ''))
                        for (var i = 0; i < vRows.length; i++) {
                            selExpenseEmployee.append(new Option(vRows[i].PeopleName, vRows[i].ID));
                            selExpenseCheckInOutEmployee.append(new Option(vRows[i].PeopleName, vRows[i].ID));
                            selReceiveFundFromCoEmployee.append(new Option(vRows[i].PeopleName, vRows[i].ID));

                            $('#divSVREmployeeList').append($('<div class="form-check">')
                                    .append($('<label class="form-check-label">')
                                        .append(vRows[i].PeopleName)
                                        .append($('<input type="checkbox" name="chbSVREmployee" class="form-check-input">')
                                            .attr('value', vRows[i].ID)
                                        )
                                    )
                                )
                        }
                    }

                    selExpenseEmployee.val(employeeid);
                    selExpenseCheckInOutEmployee.val(employeeid);
                    GetLogExpenseByMonth();
                    GetFundByMonth();
                    CountUnfinishedExpenseReport();
                    CountUnapprovedExpenseTicket();
                }
            })
        }

        function GetCostCenterLocation() {
            //aka FOWST per employee

            AjaxSend({
                url: (!IsGuid($('#selExpenseEmployee').val()) ? 'Browser/LibraryFowst' : 'Organization/GetEmployeeFowst'),
                data: {
                    EmployeeID: $('#selExpenseEmployee').val(),
                    IsAll: 1
                },
                success: function (e) {
                    var lblROExpenseFowst = $('#lblROExpenseFowst');
                    var selReceiveFundFowst = $('#selReceiveFundFowst');
                    var selCOFFowst = $('#selCOFFowst');
                    var vRows = e.Results.Rows;

                    lblROExpenseFowst.empty();
                    selReceiveFundFowst.empty();
                    selCOFFowst.empty();

                    selCOFFowst.append(new Option('Please select', ''));

                    for (var i = 0; i < vRows.length; i++) {
                        lblROExpenseFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                        selReceiveFundFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                        selCOFFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                    }
                }
            })
        }

        function GetCostCenters() {
            AjaxSend({
                url: '/Expenses/GetCostCenters',
                success: function (e) {
                    var lblROExpenseCostCenter = $('#lblROExpenseCostCenter');
                    var selExpenseCostCenter = $('#selExpenseCostCenter');
                    var vRows = e.Results.Rows;

                    selExpenseCostCenter.append(new Option('Not Applicable', 'na'));
                    selExpenseCostCenter.append(new Option('All Cost Centers', 'all'));

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            lblROExpenseCostCenter.append(new Option(vRows[i].Name, vRows[i].ID));
                            selExpenseCostCenter.append(new Option(vRows[i].Name, vRows[i].ID));
                        }
                    }

                    selExpenseCostCenter.val('all');

                    GetListOfEmployee();
                }
            })
        }

        function GetLogExpenseByMonth() {
            AjaxSend({
                url: '/Expenses/GetLogExpenseByMonth',
                success: function (e) {
                    var total = e.Results.Rows[0].Total;

                    $('.lblTotalExpense').text(AccountingFormat(total));
                    $('.lblExpenseMonthCovered').text($.format.date(new Date(), 'MMMM yyyy'));
                    $('.lblExpenseCheckInOutMonthCovered').text($.format.date(new Date(), 'MMMM yyyy'));
                }
            })
        }

        function CountUnfinishedExpenseReport() {
            AjaxSend({
                url: '/Expenses/CountUnfinishedExpenseReport',
                success: function (e) {
                    $('.lblCountUnfinishedExpenseReport').text(e.Results.Rows[0].Count);
                }
            })
        }

        function CountUnapprovedExpenseTicket() {
            AjaxSend({
                url: '/Expenses/CountUnapprovedExpenseTicket',
                success: function (e) {
                    $('.lblCountUnapprovedExpenseTicket').text(e.Results.Rows[0].Count);
                }
            })
        }

        function GetFundByMonth() {
            AjaxSend({
                url: '/Expenses/GetFundByMonth',
                success: function (e) {
                    var total = e.Results.Rows[0].Total;

                    $('#lblTotalFunds').text(AccountingFormat(total));
                }
            })
        }

        function GetLogExpenseByEmployee() {
            var tbodyLogExpense = $('#tbodyLogExpense'), totalamount = 0;
            tbodyLogExpense.empty();

            $('#tfFooterExpenseExtra').attr('colspan', 2)

            if ($.fn.DataTable.isDataTable("#tblLogExpenseReport")) {
                //check if datatable is created 
                $('#tblLogExpenseReport').DataTable().clear().destroy();
            }

            var tblLogExpenseReport = $('#tblLogExpenseReport').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        ExpenseType: $('#selLogExpenseType').val(),
                        PeopleID: $('#selExpenseEmployee').val(),
                        //MonthCovered: $('#selLogExpenseMonthCovered').val(),
                        //YearCovered: $('#selLogExpenseYearCovered').val(),
                        FromDate: expenseFromDate,
                        ToDate: expenseToDate,
                        OrgID: sessionStorage.OrgID,
                        CostCenter: $('#selExpenseCostCenter').val(),
                        CostCenterLocation: $('#selExpenseCostCenterLocation').val()
                    }, '/Expenses/GetLogExpenseByEmployee'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": LogExpenseRow(false),
                "createdRow": function (row, data, index) {
                    totalamount += data.Amount;
                    $(row).data('data', data).click(function () {
                        var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                        data.IsFinishedLogExpense = true;
                        EditLogExpense(data);
                    })
                },
                "initComplete": function(){
                    $('#tblLogExpenseReport').removeClass('dataTable');
                },
                "footerCallback": function (row, data, start, end, display) {
                    var api = this.api(), data, colIndex = (IsGuid($('#selExpenseEmployee').val()) ? 8 : 7);

                    $('#tfFooterExpense').attr('colspan', (!IsGuid($('#selExpenseEmployee').val()) ? 8 : 7));
                    $('#tfFooterExpenseExtra').attr('colspan', (!IsGuid($('#selExpenseEmployee').val()) ? 3 : 2));

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    // Total over this page
                    pageTotal = api
                        .column(6, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(CDec2(b));
                        }, 0);

                    // Update footer
                    $('#tfFooterExpense').html(
                        'Total: ' + AccountingFormat(totalamount)
                    );
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function LogExpenseRow(isHideCol) {
            return [
                {
                    title: '#',
                    "data": function (data, type, full, meta) {
                        return data.RowNum + '.';
                    },
                    "width": "30px"
                },
                {
                    title: 'Employee Name',
                    "visible": !IsGuid($('#selExpenseEmployee').val()),
                    "data": function (data, type, full, meta) {
                        return CStr2(data.EmployeeName);
                    }
                },
                {
                    title: 'Expense Date',
                    "data": function (data, type, full, meta) {
                        return $.format.date(ConvertDate((CStr2(data.ExpenseDate) == '' ? data.DateCreated : data.ExpenseDate)), 'MM/dd/yyyy');
                    }
                },
                {
                    title: 'Location From',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.FromLocation);
                    }
                },
                {
                    title: 'Location To',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.ToLocation);
                    }
                },
                {
                    title: 'Date Created',
                    "data": function (data, type, full, meta) {
                        return $.format.date(ConvertDate(data.DateCreated), 'MM/dd/yyyy hh:mm a');
                    }
                },
                {
                    title: 'Expense Type',
                    "data": function (data, type, full, meta) {
                        return data.ExpenseType;
                    }
                },
                {
                    title: 'Associated To',
                    "data": function (data, type, full, meta) {
                        return (CBool(data.IsAssociatedACustomer) ? '<span class="badge bg-success" title="Customer">C</span> ' : '') + (CBool(data.IsAssociatedASupplier) ? '<span class="badge bg-warning" title="Supplier">S</span> ' : '') + CStr2(data.AssociatedToName);
                    }, "visible": CBool(isHideCol)
                },                
                {
                    title: 'Payment Method',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.PaymentMethod);
                    }, "visible": CBool(isHideCol)
                },
                {
                    title: 'Disburse / Reimburse To',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.DisburseToVendorName || data.ReimburseToEmployeeName);
                    }, "visible": CBool(isHideCol)
                },
                {
                    title: 'Amount',
                    'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'text-align:right');
                    },
                    "data": function (data, type, full, meta) {
                        return AccountingFormat(data.Amount);
                    }
                },
                {
                    title: 'Particulars',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.Particulars);
                    }
                },
                {
                    title: 'Remarks',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.Remarks);
                    }
                },
                {
                    title: '',
                    data: null,
                    createdCell: function (td, cellData, rowData, row, col) {
                        var dropdown = $('<div class="dropdown" style="position:relative;">')
                                .append($('<button data-role="none" class="btn btn-secondary btnCircle-xs dropdown-toggle" type="button" id="dd-' + rowData.ID + '" data-toggle="dropdown">')
                                    .append('<span class="bi-gear"></span>')
                                    .click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();

                                        var isOpen = $(this).parent().hasClass('open');

                                        if (CBool(isOpen)) {
                                            $(this).parent().removeClass('open');
                                        } else {
                                            $('.dropdown').removeClass('open');
                                            $(this).parent().addClass('open');
                                        }
                                    })
                                )

                        var options = $('<ul class="dropdown-menu" aria-labelledby="dd-' + rowData.ID + '" style="left:unset; right:0;">')

                        options.append($('<li>')
                                            .append($('<a class="dropdown-item">').data('data', rowData).append('<span class="bi-pencil" style="color: #f1c40f;"></span> &nbsp; Edit')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                                                    EditLogExpense(data);
                                                })
                                            )
                                        )
                                        .append($('<li>')
                                            .append($('<a class="dropdown-item">').data('data', rowData).append('<span class="bi-trash" style="color: #c1392b;"></span> &nbsp; Delete')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = $(this).data('data');
                                                    DeleteLogExpense(data);
                                                    $('.dropdown').removeClass('open');
                                                })
                                            )
                                        )
                                        .append($('<li>')
                                            .append($('<a class="dropdown-item">').data('data', rowData).append('<span class="bi-clock" style="color: #009688;"></span> &nbsp; View History')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = $(this).data('data');
                                                    GetLogExpenseHistory(data);
                                                    $('.dropdown').removeClass('open');
                                                })
                                            )
                                        )

                        dropdown.append(options)

                        $(td).attr({
                            'data-toggle': 'tooltip',
                            'data-placement': 'left',
                            'data-title': 'More Options'
                        }).css('text-align', 'right').html(dropdown);
                    },
                    "width": "50px"
                }
            ]
        }

        function GetLogExpenseHistory(data) {
            $('#divDetailForExpenses').modal('hide');
            var tbodyLogExpenseHistory = $('#tbodyLogExpenseHistory');

            tbodyLogExpenseHistory.empty();

            AjaxSend({
                url: '/Expenses/GetLogExpenseHistory',
                data: { ExpenseID: data.ID },
                success: function (e) {
                    var vRows = $.merge([data], e.Results.Rows);
                    var totalamount = 0;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var vData = vRows[i];
                            totalamount += CDec2(vData.Amount);

                            tbodyLogExpenseHistory.append($('<tr>')
                                    .append($('<td>').text(i + 1 + '.'))
                                    .append($('<td>').text($.format.date(ConvertDate((CStr2(vData.ExpenseDate) == '' ? vData.DateCreated : vData.ExpenseDate)), 'MM/dd/yyyy')))
                                    .append($('<td>').text(CStr2(vData.FromLocation)))
                                    .append($('<td>').text(CStr2(vData.ToLocation)))
                                    .append($('<td>').text($.format.date(ConvertDate(vData.DateCreated), 'MM/dd/yyyy hh:mm a')))
                                    .append($('<td>').text(CStr2(vData.ExpenseType)))
                                    .append($('<td>').text(CStr2(vData.GPSLocation)))
                                    .append($('<td>').text(CStr2(vData.CompanyName)))
                                    .append($('<td>').text(CStr2(vData.FowstName)))
                                    .append($('<td>').text(CStr2(vData.CostCenterName)))
                                    .append($('<td>').text(CStr2(AccountingFormat(vData.Amount))))
                                    .append($('<td>').text(CStr2(vData.Remarks)))
                                    .append($('<td>').text(CStr2(vData.LastUpdatedByName) == '' ? '' : vData.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(vData.LastUpdated), 'MM/dd/yyyy hh:mm a')))
                                )
                        }
                    }

                    $('#lblLogExpenseHistoryTotal').text(AccountingFormat(totalamount));
                    $('.lblEmployeeName').text((CStr2(data.EmployeeName) == '' ? '' : 'of ' + data.EmployeeName));
                    $('#divLogExpenseHistory').modal('show');
                }
            })
        }

        function DeleteLogExpense(data) {
            if (sessionStorage.PeopleID == data.PeopleID) {
                NewMessageBox('Are you sure you want to remove record row number <b>"' + data.RowNum + '"</b>? <br><br>' +
                        'If Yes, you cannot undo this deletion.', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                DeleteNowLogExpense(data);
                            }
                        }, { Text: 'No' }
                        ])
            } else {
                NewMessageBox('Oops! You\'re not allowed to remove expenses of other employees.', 'Message');
            }
        }

        function EditLogExpense(data) {
            if (CBool(data.IsFromExpenseTicket)) {
                $('#divROExpenseParticulars').show();
            } else {
                $('#divROExpenseParticulars').hide();
            }

            EditNow();

            function EditNow() {
                $('#lblROExpenseEncodedBy').text((data.EncodedByName || data.EmployeeName) + ' on ' + $.format.date(ConvertDate(data.DateCreated), 'MM/dd/yyyy hh:mm a'));

                if (CStr2(data.LastUpdatedByName) != '') {
                    $('#lblROExpenseLastUpdatedByName').text(data.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(data.LastUpdated), 'MM/dd/yyyy hh:mm a'));
                    $('.divROExpenseLastUpdatedBy').show();
                } else {
                    $('#lblROExpenseLastUpdatedByName').text('');
                    $('.divROExpenseLastUpdatedBy').hide();
                }

                if (CStr2(data.EmployeeName) != '') {
                    $('#lblROExpenseEmployeeName').text(data.EmployeeName);
                    $('.divROExpenseEmployee').show();
                } else {
                    $('#lblROExpenseEmployeeName').text('');
                    $('.divROExpenseEmployee').hide();
                }

                isFromUnfinishedExpense = 0;
                $('#lblROExpenseTicketNo').text(CStr2(data.ExpenseTicketNo));
                $('#lblROExpenseDate').val($.format.date(ConvertDate((CStr2(data.ExpenseDate) == '' ? data.DateCreated : data.ExpenseDate)), 'yyyy-MM-dd'));
                $('#lblROExpenseType').val(data.ExpenseType);
                $('#lblROExpenseFromLocation').val(CStr2(data.FromLocation));
                $('#lblROExpenseToLocation').val(CStr2(data.ToLocation));
                $('#lblROExpenseGPSLocation').text(CStr2(data.GPSLocation));
                $('#lblROExpenseAmount').removeAttr('disabled').val(CStr2(data.Amount) == '' ? '' : AccountingFormat(data.Amount));
                $('#lblROExpenseAssociatedTo').text(CStr2(data.CompanyName || 'No Association'));
                $('#lblROExpenseFowst').val(data.FowstID);
                $('#lblROExpenseParticulars').val(CStr2(data.Particulars));
                $('#lblROExpenseCostCenter').val(CStr2(data.CostCenter || '817b4c2a-af4b-4f58-ba21-c3907320aa32')); // default to General Expense
                $('#lblROExpenseRemarks').val(''); //do not show current remarks, editor must describe his/her changes

                $('[subpage="divExpenseLogButtons"]').hide();

                if (data.IsFromExpenseTicket) {
                    $('#lblROExpenseRemarks').val(data.Remarks);
                    if (!CBool(data.IsApproved) && !CBool(data.IsRejected)) {
                        $('.forUnapprovedExpenseTicket').pageshow();
                    } else if (CBool(data.IsApproved) && !CBool(data.IsRejected) && CBool(data.IsUnfinishedExpense)) {
                        $('#forWithoutAmount').pageshow();
                    }

                    if (CBool(data.IsUnfinishedExpense)) {
                        $('#chbROExpenseAmountToBeFollow').removeAttr('disabled').attr('checked', 'checked');//.checkboxradio('refresh');
                        $('#lblROExpenseAmount').attr('disabled', 'disabled');
                        $('#divAmountToBeEncodedNextTime').show();                        

                        if (CBool(data.IsApproved)) {
                            $('#chbROExpenseAmountToBeFollow').removeAttr('checked').attr('disabled', 'disabled');//.checkboxradio('refresh');
                            $('#lblROExpenseAmount').removeAttr('disabled');

                            $('#divROExpensePreviousRemarks').show();
                            $('#txtROExpensePreviousRemarks').html('<textarea disabled style="height:150px; resize:none; width:100%;">' + CStr2(data.Remarks) + '</textarea><br><br> <b>NOTE:</b> The encoded remarks will be merge with the previous remarks. To avoid redundancy, please do not re-enter the previous remarks to the Remarks field below.<br><br>');
                            $('#lblROExpenseRemarks').val('');
                        }
                    } else {
                        $('#divROExpensePreviousRemarks, #divAmountToBeEncodedNextTime').hide();
                    }
                } else {
                    $('#forLogExpense').pageshow();
                }

                $('.dropdown').removeClass('open');
                $('#popLogExpenseMoreDetails').modal('show');

                $('#btnSaveLogExpense').unbind('click').click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    if (CBool(data.IsApproved)) {
                        NewMessageBox('Sorry! Modification is not allowed because the Expense Ticket was already approved and made via web app.', 'Message');
                        return;
                    }

                    if (sessionStorage.PeopleID == (data.PeopleID || data.EncodedBy)) {
                        SaveLogExpense(data, false, function () {
                            var isfinished = data.IsFinishedLogExpense == undefined ? true : data.IsFinishedLogExpense; // IsFinishedLogExpense means if nakaencode ang amount or wala
                            if (isfinished) {
                                GetLogExpenseByEmployee();
                                GetNetFundBalanceOfEmployee();
                            } else {
                                GetGeneralExpenses();
                            }
                        })
                    } else {
                        NewPromptUserAccess('Allow to edit expenses made by other people', {
                            Message: 'You are not allowed to edit expenses made by other people',
                            OnClickOKSuccess: function () {
                                SaveLogExpense(data, false, function () {
                                    var isfinished = data.IsFinishedLogExpense == undefined ? true : data.IsFinishedLogExpense;
                                    if (isfinished) {
                                        GetLogExpenseByEmployee();
                                        GetNetFundBalanceOfEmployee();
                                    } else {
                                        GetGeneralExpenses();
                                    }
                                })
                            }
                        });
                    }
                })

                $('#btnApproveExpenseTicket').unbind('click').click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    statusExpenseTicket = 'approved';
                    AjaxSend({
                        url: '/Expenses/CheckTicketUserIfHasAccess',
                        data: { UserID: sessionStorage.PeopleID, type: 'authorizer' },
                        success: function (e) {
                            if (CBool(e.Results)) {
                                SaveLogExpense(data, true, function () {
                                    NewMessageBox('Expense Ticket has been approved! Would you like to download the report?', 'Message', [
                                    {
                                        Text: 'Yes, download the report now!', Click: function () {
                                            window.location = MergeUrlParameters({
                                                ID: data.ID,
                                                OrgID: sessionStorage.OrgID
                                            }, '/Expenses/DownloadExpenseTicketReport');
                                        }
                                    }, { Text: 'No, maybe later.' }
                                    ]);
                                    GetUnapprovedExpenseTicket();
                                    CountUnapprovedExpenseTicket();
                                })
                            } else {
                                NewMessageBox('Sorry! You\'re not allowed to approve or reject Expense Ticket.', 'Message');
                            }
                        }
                    })
                })

                $('#btnSaveLogExpenseWithoutAmount').unbind('click').click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    statusExpenseTicket = 'approved';
                    isFromUnfinishedExpense = 1;
                    SaveLogExpense(data, true, function () {
                        NewMessageBox('Expense Ticket has been saved! Would you like to download the report?', 'Message', [
                        {
                            Text: 'Yes, download the report now!', Click: function () {
                                window.location = MergeUrlParameters({
                                    ID: data.ID,
                                    OrgID: sessionStorage.OrgID
                                }, '/Expenses/DownloadExpenseTicketReport');
                            }
                        }, { Text: 'No, maybe later.' }
                        ]);
                        GetUnapprovedExpenseTicket();
                        CountUnapprovedExpenseTicket();
                        GetGeneralExpenses();
                        CountUnfinishedExpenseReport();
                    })
                })

                $('#btnRejectExpenseTicket').unbind('click').click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    statusExpenseTicket = 'rejected';
                    AjaxSend({
                        url: '/Expenses/CheckTicketUserIfHasAccess',
                        data: { UserID: sessionStorage.PeopleID, type: 'authorizer' },
                        success: function (e) {
                            if (CBool(e.Results)) {
                                SaveLogExpense(data, true, function () {
                                    GetUnapprovedExpenseTicket();
                                    CountUnapprovedExpenseTicket();
                                })
                            } else {
                                NewMessageBox('Sorry! You\'re not allowed to approve or reject Expense Ticket.', 'Message');
                            }
                        }
                    })
                })

                $('#lblROInvoiceReference, #btnROInvoiceReference').unbind('click');

                if (CStr2(data.DisburseToVendor) != '') {
                    ERP.InvoiceImage.main.controller.GetInvoiceRefImages(function (e) {
                        $('#lblROInvoiceReference').val('');
                        var vRows = e.Results.Rows;
                        if (vRows.length > 0) {
                            $('#lblROInvoiceReference').val(vRows.map(function (elem) {
                                return elem.InvoiceRefNo;
                            }).join(', '));
                        }
                    }, {
                        mod: 'Expense',
                        ID: data.ID,
                        ExpenseNumber: data.ExpenseReferenceNo
                    })

                    $('#lblROInvoiceReference, #btnROInvoiceReference').click(function () {
                        if (CStr2(data.DisburseToVendor) != '') {
                            ERP.InvoiceImage.main.controller.popup({
                                mod: 'Expense',
                                ID: data.ID,
                                ExpenseNumber: data.ExpenseReferenceNo,
                                SupplierID: data.DisburseToVendor,
                                SupplierName: data.DisburseToVendorName
                            });
                        } else {
                            NewMessageBox('Attaching Invoice image is only for Payment method: On Credit from Vendor or For Payment Disbursement.')
                        }
                    })
                }
            }
        }

        function SaveLogExpense(data, IsFromExpenseTicket, callback) {
            if ($('#lblROExpenseType').val() == '') {
                WebToast('Please select Expense Type', 'top');
                return;
            }

            if (data.IsFromExpenseTicket) {
                if (!$('#chbROExpenseAmountToBeFollow').is(':checked') && $('#lblROExpenseAmount').val() == '') {
                    WebToast('Please enter Amount', 'top');
                    return;
                }
            } else {
                if ($('#lblROExpenseAmount').val() == '') {
                    WebToast('Please enter Amount', 'top');
                    return;
                }
            }            

            if ($('#lblROExpenseFowst').val() == '') {
                WebToast('Please select Cost Center Location', 'top');
                return;
            }

            if ($('#lblROExpenseCostCenter').val() == '') {
                WebToast('Please select Cost Center', 'top');
                return;
            }

            if ($.trim($('#lblROExpenseRemarks').val()) == '') {
                WebToast('Briefly describe your changes in the Remarks field', 'top');
                return;
            }

            var editData = {
                ID: data.ID,
                PeopleID: data.PeopleID,
                ExpenseDate: CStr2($('#lblROExpenseDate').val()),
                ExpenseType: CStr2($('#lblROExpenseType').val()),
                FromLocation: CStr2($('#lblROExpenseFromLocation').val()),
                ToLocation: CStr2($('#lblROExpenseToLocation').val()),
                Amount: $('#lblROExpenseAmount').val(),
                FowstID: CStr2($('#lblROExpenseFowst').val()),
                CostCenter: CStr2($('#lblROExpenseCostCenter').val()),
                AssociatedTo: CStr2(data.AssociatedTo),
                Remarks: CStr2($('#lblROExpenseRemarks').val()),
                Particulars: CStr2($('#lblROExpenseParticulars').val())
            }

            if (IsFromExpenseTicket) {
                editData.StatusExpenseTicket = statusExpenseTicket;
                editData.IsFromUnfinishedExpense = isFromUnfinishedExpense;

                if (isFromUnfinishedExpense) {
                    editData.Remarks = data.Remarks + '\n\n' + $.trim($('#lblROExpenseRemarks').val());
                }
            }
            Bs.Loading('show');
            AjaxSend({
                url: '/Expenses/SaveLogExpense',
                data: {
                    data: editData
                },
                success: function (e) {
                    Bs.Loading('hide');
                    if (e.Results == 'saved') {
                        $('#popLogExpenseMoreDetails').modal('hide');
                        WebToast('Successfully saved', 'top');
                        callback();
                    } else {
                        NewMessageBox(e.Results, 'Message');
                    }
                }
            })
        }

        function GetGeneralExpenses() {
            var tbodyUnfinishedReport = $('#tbodyUnfinishedReport');
            tbodyUnfinishedReport.empty();

            if ($.fn.DataTable.isDataTable("#tblUnfinishedReport")) {
                //check if datatable is created 
                $('#tblUnfinishedReport').DataTable().clear().destroy();
            }

            var tblUnfinishedReport = $('#tblUnfinishedReport').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID
                    }, '/Expenses/GetGeneralExpenses'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": LogExpenseRow(false),
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                        data.IsFinishedLogExpense = false;
                        data.IsFromExpenseTicket = true;
                        EditLogExpense(data);
                    })
                },
                "initComplete": function () {
                    $('#tblUnfinishedReport').removeClass('dataTable');
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function GetUnapprovedExpenseTicket() {
            var tbodyUnapprovedExpenseTicket = $('#tbodyUnapprovedExpenseTicket');
            tbodyUnapprovedExpenseTicket.empty();

            if ($.fn.DataTable.isDataTable("#tblUnapprovedExpenseTicket")) {
                //check if datatable is created 
                $('#tblUnapprovedExpenseTicket').DataTable().clear().destroy();
            }

            var tblrow = LogExpenseRow(true);
            tblrow.splice(1, 0, {
                "data": function (data, type, full, meta) {
                    return data.ExpenseTicketNo
                }, width: '100px'
            })
            var tblUnapprovedExpenseTicket = $('#tblUnapprovedExpenseTicket').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        ShowTickets: $('#selFilterTickets').val(),
                        keywords: $.trim($('#txtSearchExpenseTickets').val())
                    }, '/Expenses/GetAllLogExpenses'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": tblrow,
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                        data.IsFromExpenseTicket = true;
                        EditLogExpense(data);
                    })
                },
                "initComplete": function () {
                    $('#tblUnapprovedExpenseTicket').removeClass('dataTable');
                    $('#tblUnapprovedExpenseTicket thead tr').addClass('align-top text-center').css('font-size', '14px')
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function GetFundByEmployee(table) {
            PopulateFundByEmployee(table, {
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        PeopleID: $('#selExpenseEmployee').val(),
                        //MonthCovered: $('#selLogExpenseMonthCovered').val(),
                        //YearCovered: $('#selLogExpenseYearCovered').val(),
                        FromDate: expenseFromDate,
                        ToDate: expenseToDate,
                        OrgID: sessionStorage.OrgID,
                        CostCenterLocation: $('#selExpenseCostCenterLocation').val() // aka FOWST
                    }, '/Expenses/GetFundByEmployee'),
                    type: 'POST'
                }
            })
        }

        function PopulateFundByEmployee(table, options) {
            var tbodyFunds = $(table).find('tbody');
            tbodyFunds.empty();

            if ($.fn.DataTable.isDataTable(table)) {
                //check if datatable is created 
                $(table).DataTable().clear().destroy();
            }

            var tblFundReport = $(table).DataTable($.extend(options, {
                "order": [[1, "asc"]],
                "autoWidth": true,
                "columns": ReceiveFundRow(),
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                        EditReceivedFund(data);
                    })
                },
                "initComplete": function () {
                    $(table).removeClass('dataTable');
                },
                "footerCallback": function (row, data, start, end, display) {
                    var api = this.api(), data;

                    $(table).find('.tfFooterFund').attr('colspan', (4));

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    // Total over this page
                    pageTotal = api
                        .column(4, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    // Update footer
                    $(table).find('.tfFooterFund').html(
                        'Total: ' + AccountingFormat(pageTotal)
                    );
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                "sDom": '<"top">rt<"bottom"ip><"clear">'
            }))
        }

        function ReceiveFundRow() {
            return [
                {
                    "data": function (data, type, full, meta) {
                        return data.RowNum + '.';
                    },
                    "width": "30px"
                },
                {
                    "visible": ($('#selExpenseEmployee').val() == 'all' ? true : false),
                    "data": function (data, type, full, meta) {
                        return data.EmployeeName;
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return $.format.date(ConvertDate(data.ReceiveDate), 'MM/dd/yyyy');
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return (CStr2(data.ReceiveFrom) == '' ? data.OtherPerson : data.ReceiveFromName);
                    }
                },
                {
                    'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'text-align:right');
                    },
                    "data": function (data, type, full, meta) {
                        return AccountingFormat(data.Amount);
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return CStr2(data.Comment);
                    }
                },
                {
                    data: null,
                    createdCell: function (td, cellData, rowData, row, col) {
                        var dropdown = $('<div class="dropdown" style="position:relative;">')
                                .append($('<button data-role="none" class="btn btn-secondary btnCircle-xs dropdown-toggle" type="button" id="dd-' + rowData.ID + '" data-toggle="dropdown">')
                                    .append('<span class="bi-gear"></span>')
                                    .click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();

                                        var isOpen = $(this).parent().hasClass('open');

                                        if (CBool(isOpen)) {
                                            $(this).parent().removeClass('open');
                                        } else {
                                            $('.dropdown').removeClass('open');
                                            $(this).parent().addClass('open');
                                        }
                                    })
                                )

                        var options = $('<ul class="dropdown-menu" aria-labelledby="dd-' + rowData.ID + '" style="left:unset; right:0;">')

                        options.append($('<li>')
                                            .append($('<a class="dropdown-item">').data('data', rowData).append('<span class="bi-pencil" style="color: #f1c40f;"></span> &nbsp; Edit')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                                                    EditReceivedFund(data);
                                                })
                                            )
                                        )
                                        //.append($('<li>')
                                        //    .append($('<a>').data('data', rowData).append('<span class="glyphicon glyphicon-remove" style="color: #c1392b;"></span> &nbsp; Delete')
                                        //        .click(function (e) {
                                        //            e.preventDefault();
                                        //            e.stopPropagation();
                                        //            var data = $(this).data('data');
                                        //            DeleteReceiveFund(data);
                                        //            $('.dropdown').removeClass('open');
                                        //        })
                                        //    )
                                        //)
                                        .append($('<li>')
                                            .append($('<a class="dropdown-item">').data('data', rowData).append('<span class="bi-clock" style="color: #009688;"></span> &nbsp; View History')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = $(this).data('data');
                                                    GetReceiveFundHistory(data);
                                                    $('.dropdown').removeClass('open');
                                                })
                                            )
                                        )

                        dropdown.append(options)

                        $(td).attr({
                            'data-toggle': 'tooltip',
                            'data-placement': 'left',
                            'data-title': 'More Options'
                        }).css('text-align', 'right').html(dropdown);
                    },
                    "width": "50px"
                }
            ]
        }

        function GetReceiveFundHistory(data) {
            $('#divDetailForBeginningBalance').modal('hide');
            var tbodyReceiveFundHistory = $('#tbodyReceiveFundHistory');

            tbodyReceiveFundHistory.empty();

            AjaxSend({
                url: '/Expenses/GetReceiveFundHistory',
                data: { ReceiveFundID: data.ID },
                success: function (e) {
                    var vRows = $.merge([data], e.Results.Rows);
                    var totalamount = 0;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var vData = vRows[i];
                            totalamount += CDec2(vData.Amount);

                            tbodyReceiveFundHistory.append($('<tr>')
                                    .append($('<td>').text(i + 1 + '.'))
                                    .append($('<td>').text($.format.date(ConvertDate(vData.ReceiveDate), 'MM/dd/yyyy')))
                                    .append($('<td>').text(CStr2(vData.ReceiveFromName || vData.OtherPerson)))
                                    .append($('<td class="text-right">').text(CStr2(AccountingFormat(vData.Amount))))
                                    .append($('<td>').text(CStr2(vData.Comment)))
                                    .append($('<td>').text(CStr2(vData.LastUpdatedByName) == '' ? '' : vData.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(vData.LastUpdated), 'MM/dd/yyyy hh:mm a')))
                                )
                        }
                    }

                    $('#lblReceiveFundHistoryTotal').text(AccountingFormat(totalamount));
                    $('.lblEmployeeName').text((CStr2(data.EmployeeName) == '' ? '' : 'of ' + data.EmployeeName));
                    $('#divReceiveFundHistory').modal('show');
                }
            })
        }

        function DeleteReceiveFund(data) {
            if (sessionStorage.PeopleID == data.PeopleID) {
                NewMessageBox('Are you sure you want to remove record row number <b>"' + data.RowNum + '"</b>? <br><br>' +
                        'If Yes, you cannot undo this deletion.', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                DeleteNowLogExpense(data);
                            }
                        }, { Text: 'No' }
                        ])
            } else {
                NewMessageBox('Oops! You\'re not allowed to remove funds of other employees.', 'Message');
            }
        }

        function EditReceivedFund(data) {
            $('#lblROReceiveFundEncodedBy').text((data.EncodedByName || data.EmployeeName) + ' on ' + $.format.date(ConvertDate(data.DateCreated), 'MM/dd/yyyy hh:mm a'));

            if (CStr2(data.LastUpdatedByName) != '') {
                $('#lblROReceiveFundLastUpdatedByName').text(data.LastUpdatedByName + ' on ' + $.format.date(ConvertDate(data.LastUpdated), 'MM/dd/yyyy hh:mm a'));
                $('.divROReceiveFundLastUpdatedBy').show();
            } else {
                $('#lblROReceiveFundLastUpdatedByName').text('');
                $('.divROReceiveFundLastUpdatedBy').hide();
            }

            if (CStr2(data.EmployeeName) != '') {
                $('#lblROReceiveFundEmployeeName').text(data.EmployeeName);
                $('.divROReceiveFundEmployee').show();
            } else {
                $('#lblROReceiveFundEmployeeName').text('');
                $('.divROReceiveFundEmployee').hide();
            }

            $('#lblROReceiveFundDate').val($.format.date(ConvertDate(data.ReceiveDate), 'yyyy-MM-dd'));
            $('#lblROReceiveFundAmount').val(AccountingFormat(data.Amount));
            //$('#lblROReceiveFundComment').val(CStr2(data.Comment));
            $('#selReceiveFundFowst').val(CStr2(data.FowstID));

            if (CStr2(data.ReceiveFrom) != '') {
                $('#selReceiveFundFromCoEmployee').removeAttr('disabled').val(data.ReceiveFrom);
                $('#txtReceiveFundFromOther').attr('disabled', true);
                $('[name="radROReceiveFundFrom"][value="coemployee"]').prop('checked', 'checked');
            } else {
                $('#txtReceiveFundFromOther').removeAttr('disabled').val(data.OtherPerson);
                $('#selReceiveFundFromCoEmployee').attr('disabled', true);
                $('[name="radROReceiveFundFrom"][value="others"]').prop('checked', 'checked');
            }

            $('.dropdown').removeClass('open');
            $('#popReceiveFundMoreDetails').modal('show');

            $('#btnSaveReceiveFund').unbind('click').click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                if (!IsDate($('#lblROReceiveFundDate').val())) {
                    WebToast('Please select date', 'top');
                    return;
                }

                if ($('#radRFCoEmployee').is(':checked') && $('#selReceiveFundFromCoEmployee').val() == '') {
                    WebToast('Please select Co-Employee', 'top');
                    return;
                }

                if ($('#radRFOther').is(':checked') && $('#txtReceiveFundFromOther').val() == '') {
                    WebToast('Please enter other person\'s name', 'top');
                    return;
                }

                if (CStr2($('#lblROReceiveFundAmount').val()) == '') {
                    WebToast('Please enter amount', 'top');
                    return;
                }

                if (CStr2($('#lblROReceiveFundComment').val()) == '') {
                    WebToast('Briefly describe your changes in the Comment field', 'top');
                    return;
                }

                AjaxSend({
                    url: '/Expenses/SaveReceiveFund',
                    data: {
                        data: {
                            ID: data.ID,
                            PeopleID: data.PeopleID,
                            ReceiveDate: CStr2($('#lblROReceiveFundDate').val()),
                            FowstID: CStr2($('#selReceiveFundFowst').val()),
                            ReceiveFrom: CStr2($('#selReceiveFundFromCoEmployee').val()),
                            OtherPerson: CStr2($('#txtReceiveFundFromOther').val()),
                            Amount: CDec2($('#lblROReceiveFundAmount').val()),
                            Comment: CStr2($('#lblROReceiveFundComment').val())
                        }
                    },
                    success: function (e) {
                        if (e.Results == 'saved') {
                            $('#popReceiveFundMoreDetails').popup('close');
                            WebToast('Successfully saved', 'top');
                            GetFundByEmployee('#tblFundReport');
                            GetNetFundBalanceOfEmployee();
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            })
        }

        function GetFundAdjustmentByEmployee() {
            var tbodyFundAdjustment = $('#tbodyFundAdjustment');
            var ctrPending = 0;
            tbodyFundAdjustment.empty();

            if ($.fn.DataTable.isDataTable("#tblFundAdjustment")) {
                //check if datatable is created 
                $('#tblFundAdjustment').DataTable().clear().destroy();
            }

            $('#badgePendingRAF').hide();

            var tblFundAdjustment = $('#tblFundAdjustment').DataTable({
                retrieve: true,
                "bdestroy": true,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        PeopleID: $('#selExpenseEmployee').val(),
                        //MonthCovered: $('#selLogExpenseMonthCovered').val(),
                        //YearCovered: $('#selLogExpenseYearCovered').val(),
                        FromDate: expenseFromDate,
                        ToDate: expenseToDate,
                        OrgID: sessionStorage.OrgID,
                        FowstID: $('#selExpenseCostCenterLocation').val()
                    }, '/Expenses/GetFundAdjustmentByEmployee'),
                    type: 'POST'
                },
                "autoWidth": true,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">#</b>' + data.RowNum + '.';
                        },
                        "width": "30px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Branch to be adjusted</b>' + data.FowstName;
                        }
                    },
                    {
                        "visible": ($('#selExpenseEmployee').val() == 'all' ? true : false),
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Requested Name</b>' + data.RequestedBy;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Adjusted Period</b>' + $.format.date(new Date(data.MonthCovered + ' 01, ' + data.YearCovered), 'MMM yyyy');
                        }
                    },
                    {
                        'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:right');
                        },
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Original Amount</b>' + AccountingFormat(data.OriginalAmount);
                        }
                    },
                    {
                        'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:right');
                        },
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Adjusted Amount</b>' + data.AdjustedAmount == 0 ? 0 : AccountingFormat(data.AdjustedAmount);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Requested Date</b>' + $.format.date(new Date(parseInt(data.RequestedDate.substr(6))), 'MM/dd/yyyy hh:mm a');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Remarks</b>' + CStr2(data.Remarks);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            var lbl;

                            if (CStr2(data.Status) == 'Pending') {
                                lbl = 'Pending and waiting for approval';
                            } else {
                                if (CStr2(data.Status) == 'Approved') {
                                    lbl = '<span style="color:#006b04;">Approved</span>';
                                } else {
                                    lbl = '<span style="color:#f00;">Denied</span>';
                                }

                                lbl = lbl + ' on ' + $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MM/dd/yyyy hh:mm a') + ' by ' + data.UpdatedBy;
                            }
                            return '<b class="ui-table-cell-label">Status</b>' + lbl
                        }
                    },
                    {
                        'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:right');
                        },
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Approved Amount</b>' + AccountingFormat(data.NetFundBalance);
                        }
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "initComplete": function (settings, json) {
                    $('#tblFundAdjustment').removeClass('dataTable');
                    for (var i = 0; i < json.data.length; i++) {
                        if (json.data[i].Status == 'Pending') {
                            ctrPending += 1;
                            $('#badgePendingRAF').show();
                            $('#badgePendingRAF > span').text(ctrPending);
                        }
                    }
                },
                //"columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })

            $('#tblFundAdjustment tbody').unbind('click').on('click', 'td', function () {
                if (CStr2(tblFundAdjustment.row(this).data()) != '') {
                    var data = JSON.parse(JSON.stringify(tblFundAdjustment.row(this).data()));

                    if (data.Status != 'Pending') {
                        NewMessageBox('This request has already been ' + (data.Status).toLowerCase(), 'Message');
                        return;
                    }

                    CheckAccess(data, sessionStorage.PeopleID);
                }
            });
        }

        function CheckAccess(data, accessuserid) {
            AjaxSend({
                url: '/Expenses/CheckAccess',
                data: { PeopleID: accessuserid },
                success: function (e) {
                    if (e.Results.Rows.length > 0) {
                        ShowRequestFundAdjustment(data);
                    } else {
                        CustomPromptUserAccess('You\'re not allowed to process request fund adjustment.', {
                            OnClickOKSuccess: function (e) {
                                CheckAccess(data, e);
                            }
                        })
                    }
                }
            })
        }

        function ShowRequestFundAdjustment(data) {
            $('#lblFARequestedDate').text($.format.date(new Date(parseInt(data.RequestedDate.substr(6))), 'MMMM dd, yyyy hh:mm a'));
            $('#lblFARequestedBy').text(data.RequestedBy);
            $('#lblFAFowst').text(CStr2(data.FowstName));
            $('#lblFAOriginalAmount').text(AccountingFormat(data.OriginalAmount));
            $('#txtFAAdjustedAmount').val(data.AdjustedAmount);
            $('#lblFARemarks').text(CStr2(data.Remarks));

            $('#popFundAdjustmentDetails').modal('show');

            $('#btnFAApprove').unbind('click').click(function () {
                data.Status = 'Approved';
                UpdateAdjustment(data);
            })

            $('#btnFADeny').unbind('click').click(function () {
                data.Status = 'Denied';
                UpdateAdjustment(data);
            })
        }

        function UpdateAdjustment(data) {
            AjaxSend({
                url: '/Expenses/UpdateAdjustment',
                data: {
                    data: {
                        Status: data.Status,
                        Amount: CDec2($('#txtFAAdjustedAmount').val()),
                        ID: data.ID,
                        PeopleID: data.PeopleID,
                        MonthCovered: data.MonthCovered,
                        YearCovered: data.YearCovered,
                        FowstID: data.FowstID
                    }
                },
                success: function (e) {
                    if (e.Results == 'saved') {
                        $('#popFundAdjustmentDetails').modal('hide');
                        WebToast('Fund Adjustment has been successfully updated', 'top');
                        GetFundAdjustmentByEmployee();
                        GetNetFundBalanceOfEmployee();
                    } else {
                        NewMessageBox(e.Results, 'Error Message');
                        AfterPopupClose(function () {
                            $('#popFundAdjustmentDetails').popup('open');
                        })
                    }
                }
            })
        }

        function DeleteNowLogExpense(data) {
            AjaxSend({
                url: '/Expenses/DeleteLogExpense',
                data: {
                    ExpenseID: data.ID,
                    IsFromCheckInOut: CBool(data.IsFromCheckInOut),
                    EmployeeID: employeeid,
                    FowstID: data.FowstID,
                    MonthCovered: $.format.date(ConvertDate(data.ExpenseDate || data.DateCreated), 'M'),
                    YearCovered: $.format.date(ConvertDate(data.ExpenseDate || data.DateCreated), 'yyyy')
                },
                success: function (e) {
                    if (e.Results == 'saved') {
                        WebToast('Successfully deleted', 'top');
                        GetLogExpenseByEmployee();
                        GetNetFundBalanceOfEmployee();
                    } else {
                        NewMessageBox(e.Results, 'Message');
                    }
                }
            })
        }

        function GetExpenseCheckInOutByEmployee() {
            var tbodyExpenseCheckInOut = $('#tbodyExpenseCheckInOut');
            tbodyExpenseCheckInOut.empty();

            if ($.fn.DataTable.isDataTable("#tblExpenseCheckInOutReport")) {
                //check if datatable is created 
                $('#tblExpenseCheckInOutReport').DataTable().clear().destroy();
            }
            Bs.Loading('show');
            var tblExpenseCheckInOutReport = $('#tblExpenseCheckInOutReport').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        PeopleID: $('#selExpenseCheckInOutEmployee').val(),
                        FromDate: checkinFromDate,
                        ToDate: checkinToDate,
                        OrgID: sessionStorage.OrgID,
                        keywords: $.trim($('#txtExpenseCheckInOutReportSearch').val())
                    }, '/Expenses/GetCheckInOutDetailsByEmployee'),
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
                        "visible": ($('#selExpenseCheckInOutEmployee').val() == '' ? true : false),
                        "data": function (data, type, full, meta) {
                            return data.EmployeeName;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.ExactCheckInLocation);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return (CStr2(data.CheckIn) == '' ? '' : $.format.date(new Date(parseInt(data.CheckIn.substr(6))), 'MM/dd/yyyy hh:mm a'));
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return (CStr2(data.CheckOut) == '' ? '' : $.format.date(new Date(parseInt(data.CheckOut.substr(6))), 'MM/dd/yyyy hh:mm a'));
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return GetTimeDuration(data);
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Remarks);
                        }
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                "initComplete": function(){
                    $('#tblExpenseCheckInOutReport').removeClass('dataTable');
                    Bs.Loading('hide');
                },
                //"columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })

            $('#tblExpenseCheckInOutReport tbody').unbind('click').on('click', 'tr', function () {
                if (CStr2(tblExpenseCheckInOutReport.row(this).data()) != '') {
                    var data = tblExpenseCheckInOutReport.row(this).data();
                    ViewCheckInOutDetails(data);
                }
            });            
        }

        function ViewCheckInOutDetails(data) {
            if (CStr2(data.CheckIn) != '') {
                $('#lblROExpenseCheckInLocation').text(data.CheckInLocationName);
                $('#lblROExpenseCheckInTime').text($.format.date(new Date(parseInt(data.CheckIn.substr(6))), 'MM/dd/yyyy hh:mm a'));
                $('#lblROExpenseCheckInGPS').text(data.CheckInGPSLocation);
                setGPSMap(data.CheckInGPSLocation, 'divGMapCheckInGPS');
                $('#divExpenseCheckInDetails').show();
            } else {
                $('#divExpenseCheckInDetails').hide();
            }

            if (CStr2(data.CheckInAmount) != '') {
                $('#lblROExpenseCheckInAmount').text(AccountingFormat(data.CheckInAmount));
                $('#divCheckInExpense').show();
            } else {
                $('#divCheckInExpense').hide();
            }

            if (CStr2(data.CheckInFromLocation) != '' && CStr2(data.CheckInToLocation) != '') {
                $('#lblROExpenseCheckInDestination').text(data.CheckInFromLocation + ' to ' + data.CheckInToLocation);
                $('#divCheckInExpenseDestination').show();
            } else {
                $('#divCheckInExpenseDestination').hide();
            }

            if (CStr2(data.CheckOut) != '') {
                $('#lblROExpenseCheckOutLocation').text(data.CheckOutLocationName);
                $('#lblROExpenseCheckOutTime').text($.format.date(new Date(parseInt(data.CheckOut.substr(6))), 'MM/dd/yyyy hh:mm a'));
                $('#lblROExpenseCheckOutGPS').text(data.CheckOutGPSLocation);
                setGPSMap(data.CheckOutGPSLocation, 'divGMapCheckOutGPS');
                $('#divExpenseCheckOutDetails').show();
            } else {
                $('#divExpenseCheckOutDetails').hide();
            }

            if (CStr2(data.CheckOutAmount) != '') {
                $('#lblROExpenseCheckOutAmount').text(AccountingFormat(data.CheckOutAmount));
                $('#divCheckOutExpense').show();
            } else {
                $('#divCheckOutExpense').hide();
            }

            if (CStr2(data.CheckOutFromLocation) != '' && CStr2(data.CheckOutToLocation) != '') {
                $('#lblROExpenseCheckOutDestination').text(data.CheckOutFromLocation + ' to ' + data.CheckOutToLocation);
                $('#divCheckOutExpenseDestination').show();
            } else {
                $('#divCheckOutExpenseDestination').hide();
            }

            if (CStr2(data.Remarks) != '') {
                $('#lblROExpenseCheckInOutRemarks').text(CStr2(data.Remarks));
                $('#divExpenseCheckInOutRemarks').show();
            } else {
                $('#divExpenseCheckInOutRemarks').hide();
            }

            if (CStr2(data.CheckIn) != '' && CStr2(data.CheckOut) != '') {
                $('#lblROExpenseCheckInOutDuration').text(GetTimeDuration(data));
                $('#divExpenseCheckInOutDuration').show();
            } else {
                $('#divExpenseCheckInOutDuration').hide();
            }

            $('#popExpenseCheckInOutMoreDetails').modal('show');
        }

        function GetRouteSchedulesReportByEmployee() {
            Bs.Loading('show');

            AjaxSend({
                url: MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    PeopleID: $('#selExpenseCheckInOutEmployee').val(),
                    FromDate: checkinFromDate,
                    ToDate: checkinToDate,
                }, '/Expenses/GetRouteSchedulesReportByEmployee'),
                success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Results.Rows;
                    var divRSByEmployee = $('#divRSByEmployee').empty();
                    
                    var curDay = null, noactualsched = true;

                    if (vRows.length > 0) {
                        for (var i in vRows) {
                            var data = vRows[i], container = $('#divRouteSchedByDay' + CInt2(data.Day));

                            //para makuha ang duration
                            data.CheckIn = '/Date(' + new Date('1/1/2022 ' + data.StartTime).getTime() + ')/';
                            data.CheckOut = '/Date(' + new Date('1/1/2022 ' + data.EndTime).getTime() + ')/';
                            curDay = CInt2(data.Day);

                            if (container.length == 0) {
                                container = $('[name="divRouteSchedByDay"]').clone().removeClass('d-none').removeAttr('name');
                                container.attr('id', 'divRouteSchedByDay' + CInt2(data.Day));
                                divRSByEmployee.append(container);
                            }

                            container.find('[name="lblDay"]').text(arrDayName[CInt2(data.Day)]);

                            var actualRoute = JSON.parse(data.Actual);
                            var tbody = container.find('tbody');
                            var trLoc = $('<tr class="text-center">')
                                            .append($('<td class="text-left">').html('Company: ' + data.LocationName + '<br>Outlet: ' + data.BranchName))
                                            .append($('<td class="table-active" style="min-width:100px">').html(CStr2(data.StartTime) == '' ? '' : $.format.date(new Date('1/1/2022 ' + data.StartTime), 'hh:mm a') + ' to ' + $.format.date(new Date('1/1/2022 ' + data.EndTime), 'hh:mm a')))
                                            .append($('<td class="table-active" style="min-width:100px">').text(GetTimeDuration(data)))

                            tbody.append(trLoc);

                            if (actualRoute.length > 0) {
                                noactualsched = false;
                                trLoc.find('td').attr('rowspan', actualRoute.length + 1);
                                for (var a in actualRoute) {
                                    var vActualRouteData = actualRoute[a];
                                    tbody.append($('<tr class="text-center">')
                                            .css('cursor', 'pointer')
                                            .data('data', vActualRouteData)
                                            .append($('<td style="min-width:170px">').html((CStr2(vActualRouteData.CheckIn) == '' ? '' : $.format.date(CJsonDate(vActualRouteData.CheckIn), 'MM-dd-yyyy hh:mm a'))))
                                            .append($('<td style="min-width:170px">').html((CStr2(vActualRouteData.CheckOut) == '' ? '' : $.format.date(CJsonDate(vActualRouteData.CheckOut), 'MM-dd-yyyy hh:mm a'))))
                                            .append($('<td style="min-width:100px">').html(GetTimeDuration(vActualRouteData)))
                                            .click(function () {
                                                var data = $(this).data('data');
                                                ViewCheckInOutDetails(data);
                                            })
                                        )
                                }
                            } else {
                                trLoc.find('td').attr('rowspan', 2);
                                tbody.append($('<tr class="text-center">')
                                            .append($('<td colspan="3">').text('No data found'))
                                        )
                            }
                        }                        
                    } else {
                        divRSByEmployee.append($('<div class="text-center text-secondary" style="font-size:30px">').text('No data available'))
                    }
                }
            })
        }

        function GetRouteSchedulesReportByEmployeeV2() {
            Bs.Loading('show');
            var monthperiod = new Date();
            monthperiod.setMonth(CInt2($('#selExpenseCheckInOutDateRangeMonth').val()) - 1);
            monthperiod.setFullYear(CInt2($('#selExpenseCheckInOutDateRangeYear').val()));

            AjaxSend({
                url: MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    PeopleID: $('#selExpenseCheckInOutEmployee').val(),
                    FromDate: $.format.date(new Date(monthperiod.getFullYear(), monthperiod.getMonth(), 1), 'MMM dd, yyyy'),
                    ToDate: $.format.date(new Date(monthperiod.getFullYear(), monthperiod.getMonth() + 1, 0), 'MMM dd, yyyy')
                }, '/Expenses/GetRouteSchedulesReportByEmployeeV2'),
                success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Details;
                    var thPlannedRouteDate = $('#thPlannedRouteDate');
                    var tblRouteScheduleByEmployeeV2 = $('[name="tblRouteScheduleByEmployeeV2"]');
                    var tblRouteScheduleByEmployeeV2Body = tblRouteScheduleByEmployeeV2.find('tbody');
                    var tblRouteScheduleByEmployeeV2Footer = tblRouteScheduleByEmployeeV2.find('tfoot > tr');
                    var tds = '', arrActualRoutes = [], arrVisitedDates = [];
                    var dateCovered = new Date();
                    dateCovered.setMonth(CInt2($('#selExpenseCheckInOutDateRangeMonth').val()) - 1);
                    dateCovered.setFullYear(CInt2($('#selExpenseCheckInOutDateRangeYear').val()));
                    var lastDayOfMonth = new Date(dateCovered.getFullYear(), dateCovered.getMonth() + 1, 0).getDate();

                    thPlannedRouteDate.empty().append('<th style="min-width:200px; max-width:400px;">Location Name</th>');
                    thPlannedRouteDate.append('<th>Schedule</th>');
                    tblRouteScheduleByEmployeeV2Body.empty();
                    tblRouteScheduleByEmployeeV2Footer.empty().append('<td style="min-width:200px; max-width:400px; text-align:center; font-weight:bold; z-index:6;">Total Duration</td><td></td>');

                    for (var i = 1; i <= lastDayOfMonth; i++) {
                        var RPdate = $.format.date(dateCovered, 'MM-' + i + '-yyyy');
                        var daynow = new Date(dateCovered.setDate(i)).getDay()
                        tds += '<td class="text-truncate text-center" date="' + RPdate + '"></td>';
                        thPlannedRouteDate.append('<th id="' + RPdate + '" date="' + RPdate + '">' + i + '<br><span style="font-size:12px">' + arrDayName[(daynow == 0 ? 6 : daynow - 1)] + '</span></th>');
                    }

                    thPlannedRouteDate.find('th[date="' + $.format.date(new Date(), 'MM-d-yyyy') + '"]').addClass('bg-success text-white');

                    tblRouteScheduleByEmployeeV2Footer.append(tds)

                    for (var i in vRows) {
                        var data = vRows[i], tr;
                        var tdloc = '<td>' + data.CheckInLocationName + '</td>';
                        var actualRoute = data.VisitedLocations;

                        if (CStr2(data.LocationID) != '' && CStr2(data.FowstID) != '') {
                            tr = '<tr locationid="' + data.LocationID + '" fowstid="' + data.FowstID + '">';
                        } else {
                            tr = '<tr locationname="' + data.CheckInLocationName + '">';
                        }

                        actualRoute.length > 0 ? arrActualRoutes.push(actualRoute) : '';
                        tr += tdloc;
                        tr += '<td class="text-truncate">';
                        for (var s in data.Schedule) {
                            var sched = data.Schedule[s];
                            tr += (sched.Week == null ? '' : 'W' + sched.Week + '-') + sched.DayName + (sched.StartTime == null ? ' (Anytime)' : ' (' + sched.StartTime + ' - ' + sched.EndTime + ')') + '<br>';
                        }
                        tr += '</td>';
                        tr += tds;
                        
                        tblRouteScheduleByEmployeeV2Body.append(tr);
                    }

                    if (arrActualRoutes.length > 0) {
                        for (var i in arrActualRoutes) {
                            var ar = arrActualRoutes[i];
                            for (var a in ar) {
                                var data = ar[a];
                                var cInDate = $.format.date(CJsonDate(data.CheckIn), 'MM-d-yyyy');
                                var td, txt = '<div>', isExist = false, indexVisitedDate = null;

                                if (CStr2(data.CheckInCustomerOrgID) != '' || CStr2(data.CheckInCustomerOrgOutletID) != '') {
                                    td = $('tr[locationid="' + data.CheckInCustomerOrgID + '"][fowstid="' + data.CheckInCustomerOrgOutletID + '"] > td[date="' + $.format.date(CJsonDate(data.CheckIn), 'MM-d-yyyy') + '"]');
                                } else {
                                    td = $('tr[locationname="' + data.CheckInLocationName + '"] > td[date="' + $.format.date(CJsonDate(data.CheckIn), 'MM-d-yyyy') + '"]');
                                }

                                $.grep(arrVisitedDates, function (e, i) {
                                    if (e.Date == cInDate) {
                                        isExist = true;
                                        indexVisitedDate = i;
                                        return false;
                                    }
                                })

                                if (CStr2(data.CheckIn) != '') {
                                    txt += $.format.date(CJsonDate(data.CheckIn), 'hh:mm a');
                                }

                                if (CStr2(data.CheckOut) != '') {
                                    txt += ' - ';
                                    txt += $.format.date(CJsonDate(data.CheckOut), 'hh:mm a');
                                    txt += '<br>= ' + GetTimeDuration(data);
                                    var inMinutes = CJsonDate(data.CheckOut) - CJsonDate(data.CheckIn);

                                    if (isExist) {
                                        arrVisitedDates[indexVisitedDate].Duration += inMinutes;
                                    } else {
                                        arrVisitedDates.push({
                                            Date: cInDate,
                                            Duration: inMinutes
                                        })
                                    }
                                }

                                txt += '</div>';

                                td.append($(txt).css('cursor', 'pointer').data('data', data)
                                    .click(function () {
                                        var data = $(this).data('data');
                                        ViewCheckInOutDetails(data);
                                    }))
                            }
                        }

                        for (var i in arrVisitedDates) {
                            tblRouteScheduleByEmployeeV2Footer.find('td[date="' + arrVisitedDates[i].Date + '"]').text(GetTimeDuration(arrVisitedDates[i].Duration));
                        }

                        document.getElementById($.format.date(new Date(), "MM-dd-yyyy")).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                    }
                }
            })
        }

        function ComputeAllDuration(totalmins)  {
            if(Math.sign(totalmins) != -1){
                var mins= totalmins % 60;
                var hours = Math.floor(totalmins / 60);
                var days= Math.floor(hours / 24);
                var hourss = hours % 24;
                return days+ ':'+hourss + ':'+ mins;
            }
            else{
                var absTotal= Math.abs(totalmins);
                var mins= absTotal % 60;
                var hours = Math.floor(absTotal / 60);
                var days= Math.floor(hours / 24);
                var hourss = hours % 24;
                return days+ ':'+hourss + ':'+ mins;
            }
        }

        function setGPSMap(gpsLocation, div) {
            var lat;
            var long;

            if (gpsLocation == undefined || gpsLocation == '') {
                //the paper tree sta ana GPS
                lat = 7.076466;
                long = 125.614671;
            } else {
                var latlong = gpsLocation.split(',');
                lat = latlong[0];
                long = latlong[1];
            }

            centerpoint = new google.maps.LatLng(lat, long);

            var mapProp = {
                center: centerpoint,
                zoom: 17
            };

            var map = new google.maps.Map(document.getElementById(div), mapProp);

            var marker = new google.maps.Marker({
                position: centerpoint,
                animation: google.maps.Animation.DROP
            });
            marker.setMap(map);
        }

        function GetTimeDuration(data) {
            var duration = '', secs = '', mins = '', hours = '', days = '';

            if (typeof data == 'object') {
                secs = CJsonDate(data.CheckOut) - CJsonDate(data.CheckIn);
            } else if (typeof data == 'number') {
                secs = data;
            }

            mins = Math.floor(secs / 60000);
            hours = Math.floor(mins / 60);
            days = Math.floor(hours / 24);

            if (days > 0) {
                duration = days + 'd & ';
                hours = hours % 24;
            }

            if (hours > 0) {
                duration += hours + 'h ';
                mins = mins % 60;
            } else if (hours == 0) {
                mins = mins % 60;
            }

            if (hours > 0 && mins > 0) {
                duration += '& '
            }

            if (mins > 0) {
                duration += mins + 'm';
            }

            return duration;
        }

        function ConvertDate(date) {
            if (isNaN(new Date(date))) {
                date = new Date(parseInt(date.substr(6)));
            } else {
                date = new Date(date);
            }

            return date;
        }
    })
})
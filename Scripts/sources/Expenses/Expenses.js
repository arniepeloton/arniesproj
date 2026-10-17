require(['common',
    'promise!/Scripts/sources/Expenses/ExpensesOffline.js',
    'promise!/Scripts3/Offline/Browser/LibraryOrganizationCustomer.js',
    'promise!/Scripts3/Offline/Browser/LibraryOrganizationSupplier.js',
    'promise!/Scripts3/Offline/Browser/LibraryFowst.js',
    'promise!/Scripts3/Offline/Browser/LibraryPeople.js',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='],
function () {
    $(document).ready(function () {
        var Expenses = new App.model.Expenses();
        var checkinID = '', checktype, expenseID = '', associatedTo = '', isEditLogExpense = false, isEditFund = false;
        var fundid = '', receivefundfrom = '', arrCheckIn, isLogAnExpenseFromCheckInOut = false;
        var customerorgid = '', customerorgoutletid = '';
        var isLogExpenseRemarksFieldRequired = Business.SystemDefaults.GetSystemDefault('LogExpenseRemarksField');

        GetCostCenters();
        GetFowst();

        if (CBool(isLogExpenseRemarksFieldRequired)) {
            $('#lblLogExpenseRemarksField').show();
        } else {
            $('#lblLogExpenseRemarksField').hide();
        }

        if (!CBool(Business.SystemDefaults.GetSystemDefault('AllowCustomLocationName'))) {
            $('.btnUserDefinedLocation').text('If you are having trouble finding the location you searched for, kindly contact your system administrator for assistance. Thank you!')
        }

        if (!Expenses.CheckIfMyDataExistInSqlite() && !CBool(Business.SystemDefaults.GetSystemDefault('GotMyRouteExpenseData'))) {
            Bs.Loading('show', 'It seems you have a new database installed on your device. Your data is currently being downloaded. Please wait a moment.');
            setTimeout(function () {
                Bs.Loading('hide');
                SyncNow();
            }, 5000);
        }

        $('#lblCurrentUserFullName').text(sessionStorage.CurrentUserFullName);
        $('#txtFundReceivedBy').val(sessionStorage.CurrentUserFullName);

        $('#txtExpenseDate, #txtFundReceivingDate').attr({
            min: $.format.date(new Date(), 'yyyy-MM-0' + 1),
            max: $.format.date(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0), 'yyyy-MM-dd')
        })

        $('#btnExpenseBackToMenu').click(function () {
            location = MergeUrlParameters({
                IsMobile: 1
            }, '/People/PeopleMenu')
        })

        $('#btnCkeckInOutBack, #btnFundsReportsBack, #btnLogExpenseBack, #btnExpenseCheckinReportsBack, #btnSyncDataBack').click(function () {
            $('#pageExpenseMain').pageshow();
            ClearFieldsCheckinout();
        })

        $('#btnFundReceivedReportsHeaderDetailsBack, #btnExpenseReportsBack, #btnCarryOverFundsBack').click(function () {
            $('#pageFundsReports').pageshow();
        })

        $('#btnCarryOverFundAdjustmentBack, #btnCancelCarryOverFund').click(function () {
            $('#pageCarryOverFunds').pageshow();
        })
        
        $('.btnLogExpenseBack').click(function () {
            if (isEditLogExpense) {
                $('#pageExpenseReportsHeaderDetails').pageshow();
            } else {
                $('#pageLogExpenseMain').pageshow();
            }
        })

        $('#btnExpenseReceiveFundsBack, #btnCancelReceiveFund').click(function () {
            if (isEditFund) {
                $('#pageFundReceivedReportsHeaderDetails').pageshow();
            } else {
                $('#pageExpenseMain').pageshow();
            }
        })

        $('#btnLogExpenseMainBack').click(function () {
            if (isLogAnExpenseFromCheckInOut) {
                $('#pageCheckInOut').pageshow();
            } else {
                $('#pageExpenseMain').pageshow();
            }
        })

        $('#btnExpenseReportsHeaderDetailsBack').click(function () {
            $('#pageExpenseReports').pageshow();
        })

        $('#btnExpenseCheckIn').click(function () {
            if (Expenses.CheckLastCheckInOut()) {
                NewMessageBox('Please check-out the location where you last checked in. Thank you!', 'Message', [
                    {
                        Text: 'Check-out Now', Click: function () {
                            $('#btnExpenseCheckOut').click();
                        }
                    }, { Text: 'Cancel' }
                ])
            } else {
                ClearFieldsCheckinout();
                checktype = 'in';
                GetGPSLocation();

                $('#lblCheckType').text('In');
                $('#pageCheckInOut').pageshow();
                $('#txtExpenseCheckIn').val($.format.date(new Date(), 'MMM dd, yyyy hh:mm a'));
            }            
        })

        $('#btnExpenseCheckOut').click(function () {
            if (!Expenses.CheckLastCheckInOut()) {
                NewMessageBox('There isn\'t any checked-in location that needs to be checked out. Instead, would you like to check-in?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            $('#btnExpenseCheckIn').click();
                        }
                    }, { Text: 'No' }
                ])
            } else {
                ClearFieldsCheckinout();
                checktype = 'out';
                GetGPSLocation();

                $('#lblCheckType').text('Out');
                $('#pageCheckInOut').pageshow();

                var prevCheckin = Expenses.GetLastCheckIn().Rows;

                if (prevCheckin.length > 0) {
                    checkinID = prevCheckin[0].ID;
                    customerorgid = prevCheckin[0].CheckInCustomerOrgID;
                    customerorgoutletid = prevCheckin[0].CheckInCustomerOrgOutletID;
                    $('#txtExpenseLocationName').val(prevCheckin[0].CheckInLocationName);
                    $('#txtExpenseCheckIn').val($.format.date(new Date(prevCheckin[0].CheckIn), 'MMM dd, yyyy hh:mm a'));
                    $('#txtExpenseCheckinoutRemarks').val(prevCheckin[0].Remarks);
                }

                $('#txtExpenseCheckOut').val($.format.date(new Date(), 'MMM dd, yyyy hh:mm a'));
            }            
        })

        $('#btnExpenseCheckinReports').click(function () {
            $('#pageExpenseCheckinReports').pageshow();
            GetCheckinDetails();
        })

        $('#txtExpenseLocationName').click(function () {
            if (checktype == 'out') {
                NewMessageBox('Sorry! During the checkout process, you cannot modify the location.');
                return
            }

            if ($.trim($(this).val()) == '') {
                PredefinedLocation();
            } else {
                PopupButtonList('', 'Edit Options', [
                    {
                        Text: 'Select from Pre-defined location', Click: function () {
                            PredefinedLocation();
                        }
                    }, {
                        Text: 'Enter / Edit my exact location', Click: function () {
                            $('.btnUserDefinedLocation').click();

                            if (customerorgid == '' && customerorgoutletid == '') {
                                $('#txtUserDefinedExactLocation').val($.trim($('#txtExpenseLocationName').val()));
                            }
                        }
                    }
                ])
            }
            return;
            BrowsePopup('LibraryOrganizationCustomer', 'Select Location', [
                {
                    Offline: true,
                    Message: 'Where are you right now?',
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (c) {
                        BrowsePopup('new App.model.Expenses().GetCustomerOutlet(' + ReadyString(c.ID) + ')', 'Select Branch', [
                            {
                                AutoSearch: true,
                                Offline: true,
                                OfflineBrowser: false,
                                Message: 'In what branch of ' + c.CompanyName + ' are you right now?',
                                DisplayText: [{ Column: 'Name' }],
                                Click: function (b) {
                                    customerorgid = c.ID;
                                    customerorgoutletid = b.ID;
                                    $('#txtExpenseLocationName').val(c.CompanyName + ' - ' + b.Name).removeClass('is-invalid');
                                }
                            }
                        ])
                    }
                }
            ])
        })

        $('#txtSearchLocation').keydown(function (e) {
            if (e.which == 13) {
                var dis = $(this);
                var listSearchedLocation = $('#listSearchedLocation');

                listSearchedLocation.empty();
                dis.blur();

                $('#divSearchLocationLoading').show();
                $('#divForUnsearchedLocation, #divSearchedList, #divSchedules').hide();

                setTimeout(function () {
                    var vRows = OfflineBrowser.LibraryOrganizationCustomer.GetData({ keywords: $.trim(dis.val()) }).Rows;                    

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            listSearchedLocation.append($('<a href="" style="padding:1em;" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">')
                                    .append($('<div style="width:100%;">').text(vRows[i].CompanyName))
                                    .append($('<span class="bi-chevron-right">'))
                                    .data('data', vRows[i])
                                    .click(function (e) {
                                        var location = $(this).data('data');
                                        GetLocationOutlet(location);
                                    })
                                )
                        }
                    } else {
                        listSearchedLocation.append('<a data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">No Result(s) Found</a>');
                    }

                    $('#divSearchLocationLoading').hide();
                    $('#divForUnsearchedLocation').show();
                    $('#divSearchedList').pageshow();
                }, 500);
            }
        })

        $('.btnUserDefinedLocation').click(function () {
            if (!CBool(Business.SystemDefaults.GetSystemDefault('AllowCustomLocationName'))) {
                return;
            }
            $('#popSelectLocation, #popSelectLocationOutlet').modal('hide');
            $('#popUserDefinedLocation').modal('show');
            $('#txtUserDefinedExactLocation').val('');
        })

        $('#btnOkUDExactLocation').click(function () {
            customerorgid = '';
            customerorgoutletid = '';
            $('#txtExpenseLocationName').val($.trim($('#txtUserDefinedExactLocation').val()));
            $('#popUserDefinedLocation').modal('hide');
        })

        $('.btnLogExpense').click(function () {
            ClearFieldsLogExpense();
            isLogAnExpenseFromCheckInOut = false;
            $('#pageLogExpenseMain').pageshow();
        })

        $('#btnExpenseReceiveFunds').click(function () {
            ClearFieldReceiveFunds();
            $('#pageExpenseReceiveFunds').pageshow();
        })

        $('#btnReceivedFundDetails').click(function () {
            $('#pageFundReceivedReportsHeaderDetails').pageshow();
            GetFundsHeaderDetails();
        })

        $('#btnCarryOverFunds').click(function () {
            ShowCarryOverFund();
        })

        $('#btnAdjustFund').click(function () {
            if (Expenses.RequestAdjustFund().Rows.length > 0) {
                NewMessageBox('Sorry! You still have pending request fund adjustment.');
                return;
            }

            var carryoverfund = Expenses.CarryOverFunds().Rows;

            if (carryoverfund.length > 0) {
                carryoverfund = CDec2(carryoverfund[0].TotalBalance);
            } else {
                carryoverfund = 0;
            }

            $('#pageCarryOverFundAdjustment').pageshow();
            $('#txtOriginalCarryOverFund').val(carryoverfund);
            $('#txtAdjustedCarryOverFund, #txtRemarksCarryOverFund').val('');
        })

        $('#btnSaveCarryOverFund').click(function () {
            var origamt = CDec2($('#txtOriginalCarryOverFund').val());
            var adjustedamt = CDec2($('#txtAdjustedCarryOverFund').val());
            var remarks = CStr2($.trim($('#txtRemarksCarryOverFund').val()));
            var selFowstCarryOverFund = $('#selFowstCarryOverFund').val();

            if (selFowstCarryOverFund == '') {
                NewMessageBox('Please select branch to be adjusted', 'Message');
                return;
            }

            if (CDec2(adjustedamt) == CDec2(origamt)) {
                NewMessageBox('Adjusted amount should be greater or less than ' + AccountingFormat(origamt), 'Message');
                return;
            }

            if (remarks == '') {
                NewMessageBox('Briefly describe your fund adjustment', 'Message');
                return;
            }

            var res = Expenses.SaveCarryOverFunds({
                OrigAmount: origamt,
                AdjustedAmount: adjustedamt,
                Remarks: remarks,
                FowstID: selFowstCarryOverFund
            })

            if(IsGuid(res)){
                ShowCarryOverFund();
            } else {
                NewMessageBox(res, 'Error Message');
            }
        })        

        $('#btnFundReceivedFrom').click(function () {
            BrowsePopup('LibraryPeople', 'Search Employee', [
                {
                    AddButton: [
                        {
                            Text: 'Not on the list? Click here', Click: function () {                                
                                $('#popReceiveFundOtherPerson').modal('show');
                                $('#txtFundReceivingOtherPerson').val('').focus();
                            }
                        }
                    ],
                    DisplayText: [
                        { Column: 'PeopleName' }
                    ],
                    Offline: true,
                    Click: function (e) {
                        receivefundfrom = e.ID;
                        DisplayReceiveFrom(e);
                    }
                }
            ], { EmployeesOnly: true })
        })

        $('#btnOkFundOtherPerson').click(function () {
            receivefundfrom = null;
            DisplayReceiveFrom({ PeopleName: $.trim($('#txtFundReceivingOtherPerson').val()) });
            $('#popReceiveFundOtherPerson').modal('hide');
        })

        $('#btnSaveReceiveFund').click(function () {
            SaveReceiveFund();
        })

        $('.btnLogExpenseType').click(function () {
            ClearFieldsLogExpense();
            $('#pageLogExpense').pageshow();
            $('#selExpenseType').val($(this).attr('value')).change();

            var currentLocation = GetLatestGPSLocation();

            if (CStr2(currentLocation) != '') {
                currentLocation = currentLocation.split('-');
                $('#txtExpenseGPSCoordinates').val(currentLocation[0] + ',' + currentLocation[1]);
            }
        })

        $('#selExpenseType').change(function () {
            var val = $(this).val()
            if (val == 'Jeepney Fare' || val == 'Bus Fare' || val == 'Taxi Fare' || val == 'Van Fare' ||
                val == 'Tricycle Fare' || val == 'Air Fare' || val == 'Sea Fare' || val == 'Train Fare') {
                $('.divforExpenseTypeFare').show();
                $('#txtExpenseFrom, #txtExpenseTo').removeClass('is-invalid');
            } else {
                $('.divforExpenseTypeFare').hide();
            }
        })

        $('#btnExpensePicture').click(function () {
            $('#fileExpensePicture').click()
        })

        $('#btnExpenseAddClient').click(function () {
            PopupButtonList('Search from:', 'Options', [
                {
                    Text: 'Client Lists',
                    Size: 3,
                    Click: function () {
                        AddClientVendor('customer');
                    }
                },
                {
                    Text: 'Vendor Lists',
                    Size: 3,
                    Click: function () {
                        AddClientVendor('supplier');
                    }
                },
                {
                    Text: 'No Association',
                    Size: 3,
                    Click: function () {
                        AddClientVendor('');
                    }
                }
            ])            
        })

        $('#btnCheckInOutLogExpense').click(function () {
            isLogAnExpenseFromCheckInOut = true;
            SaveCheckInOut(function (data) {
                arrCheckIn = data;
                $('#pageLogExpenseMain').pageshow();
            });
        })

        $('#btnSaveCheckinout').click(function () {
            isLogAnExpenseFromCheckInOut = false;
            SaveCheckInOut();
        })

        $('#btnSaveLogExpense').click(function () {
            SaveLogExpense();
        })

        $('#btnFundsReports').click(function () {
            $('#pageFundsReports').pageshow();
            GetFundsHeader();
        })

        $('#btnSyncData').click(function () {
            $('#pageSyncData').pageshow();
        })

        $('#btnSyncDataNow').click(function () {
            SyncNow();
        })

        $('#btnExpenseReports').click(function () {
            $('#pageExpenseReports').pageshow();
            GetLogExpenseHeader();
        })

        $('#btnReceivedFundMoreDetails').unbind('click').click(function () {
            NewMessageBox('To save on storage space, some received fund details are no longer stored in your local storage. <br> <br> Received Fund details are saved in server storage whenever you sync your database. These received fund details are not saved on your new local database after each sync to conserve your local storage and preserve your device\'s data lookup performance.', 'Message', [
                {
                    Text: 'View Details Online',
                    Size: 2,
                    Click: function (e) {
                        Bs.Loading('show', 'Please wait ... getting data from server', 'Message');

                        AjaxSend({
                            url: 'Expenses/GetMoreDetailsOfReceivedFunds',
                            data: {
                                data: {
                                    PeopleID: sessionStorage.PeopleID,
                                    OrgID: sessionStorage.OrgID
                                }
                            }, success: function (e) {
                                Bs.Loading('hide');

                                var vRows = e.Results.Rows;
                                var listReceivedFundHeaderMoreDetails = $('#listReceivedFundHeaderMoreDetails');
                                var totalamount = 0;

                                listReceivedFundHeaderMoreDetails.empty();

                                if (vRows.length > 0) {
                                    for (var i = 0; i < vRows.length; i++) {
                                        totalamount += CDec2(vRows[i].Amount)
                                        listReceivedFundHeaderMoreDetails.append(FundList(vRows[i], (i + 1), true)
                                                .data('data', vRows[i]))
                                    }
                                } else {
                                    listReceivedFundHeaderMoreDetails.append($('<li class="list-group-item">')
                                        .text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy'))
                                    )
                                }                                

                                $('#lblTotalReceivedFundMoreDetails').text(AccountingFormat(totalamount));
                                $('#popReceivedFundMoreDetails').modal('show');
                            }, error: function (e) {
                                Bs.Loading('hide');
                                NewMessageBox('Internet connection required', 'Message');
                            }
                        })
                    }
                }, {
                    Text: 'Cancel', Size: 2
                }
            ])
        })

        $('#btnCheckinoutMoreDetails').unbind('click').click(function () {
            NewMessageBox('To save on storage space, some check in/out details are no longer stored in your local storage. <br> <br> Check in/out details are saved in server storage whenever you sync your database. These check in/out details are not saved on your new local database after each sync to conserve your local storage and preserve your device\'s data lookup performance.', 'Message', [
                {
                    Text: 'View Details Online',
                    Size: 2,
                    Click: function (e) {
                        Bs.Loading('show', 'Please wait ... getting data from server', 'Message');

                        GetMoreDetailsOfCheckInOut();
                    }
                }, {
                    Text: 'Cancel', Size: 2
                }
            ])
        })

        $('#selCheckinSorting').change(function () {
            GetCheckinDetails();
        })        

        $('#selMoreDetailsCheckinSorting').change(function () {
            GetMoreDetailsOfCheckInOut();
        })

        $('#selFundReportFowst').change(function () {
            GetFundsHeader();
        })

        $('#btnscheduletoday').click(function () {
            $('#scheduletoday').pageshow();
        })

        $('#btncheckedinhistory').click(function () {
            $('#checkedinhistory').pageshow();
        })

        function GetLocationOutlet(location) {
            var listLocationBranch = $('#listLocationBranch');

            $('#popSelectLocation').modal('hide');
            $('#lblSelectedLocation').text(location.CompanyName);
            $('#divSearchLocationOutletLoading, #divSearchedListOutlet, #divForUnsearchedLocationOutlet').hide();
            $('#popSelectLocationOutlet').modal('show');

            $('#txtSearchLocationOutlet').val('').unbind('keydown').keydown(function (e) {
                if (e.which == 13) {
                    var keywords = $.trim($(this).val());

                    $('#divSearchLocationOutletLoading').show();
                    $('#divSearchedListOutlet, #divForUnsearchedLocationOutlet').hide();

                    setTimeout(function () {
                        vRows = Expenses.GetCustomerOutlet(location.ID, keywords).Rows;

                        listLocationBranch.empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                listLocationBranch.append($('<a href="" style="padding:1em;" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">')
                                        .append($('<div style="width:100%;">').text(vRows[i].Name))
                                        .append($('<span class="bi-chevron-right">'))
                                        .data('data', vRows[i])
                                        .click(function (e) {
                                            var locationbranch = $(this).data('data');
                                            customerorgid = location.ID;
                                            customerorgoutletid = locationbranch.ID;
                                            $('#txtExpenseLocationName').val(location.CompanyName + ' - ' + locationbranch.Name).removeClass('is-invalid');
                                            $('#popSelectLocationOutlet').modal('hide');
                                        })
                                    )
                            }
                        } else {
                            listLocationBranch.append('<a data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">No branches found for ' + location.CompanyName + '</a>');
                        }

                        $('#divSearchLocationOutletLoading').hide();
                        $('#divSearchedListOutlet, #divForUnsearchedLocationOutlet').show();
                    }, 500)
                }
            })
        }

        function PredefinedLocation() {
            $('#divForUnsearchedLocation, #divSearchLocationLoading, #divSearchedList').hide();
            $('#listLocation, #listTodaysSchedule').empty();
            $('#txtSearchLocation').val('');
            $('#divSchedules').pageshow();
            $('#btnscheduletoday').click();

            if (checktype == 'in') {
                $('#lblCheckTypeLabel').text('checked-in');
            } else {
                $('#lblCheckTypeLabel').text('checked-out');
            }

            var recentlyvisitedloc = Expenses.RecentlyVisitedLocation(checktype).Rows;
            var ScheduleInfo = Expenses.EmployeeScheduleForToday();
            var todaysschedule = ScheduleInfo.Schedules.Rows;
            
            if (recentlyvisitedloc.length > 0) {
                for (var i = 0; i < recentlyvisitedloc.length; i++) {
                    $('#listLocation').append($('<a href="" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center p-2">')
                                    .append($('<div style="width:100%;">').text(recentlyvisitedloc[i].CompanyName))
                                    .append($('<span class="bi-chevron-right">'))
                                    .data('data', recentlyvisitedloc[i])
                                    .click(function (e) {
                                        var location = $(this).data('data');
                                        
                                        customerorgid = CStr2(location.OrgID);
                                        customerorgoutletid = CStr2(location.OutletID);

                                        $('#txtExpenseLocationName').val(location.CompanyName);
                                        $('#popSelectLocation').modal('hide');
                                    })
                                )
                }
            } else {
                $('#listLocation').append('<a data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">No recently visited locations</a>');
            }

            if (todaysschedule.length > 0) {
                for (var i = 0; i < todaysschedule.length; i++) {
                    var data = todaysschedule[i];
                    $('#listTodaysSchedule').append($('<a href="" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center p-2">')
                                    .append($('<div style="width:100%;">').html(data.CompanyName + '<br>' + (CStr2(data.Time) == '' ? '' : '- ' + data.Time + '<br>') + '- ' + (CStr2(data.Week) == '' ? '' : 'Week ' + data.Week + ' - ') + data.DayName))
                                    .append($('<span class="bi-chevron-right">'))
                                    .data('data', data)
                                    .click(function (e) {
                                        var location = $(this).data('data');

                                        customerorgid = CStr2(location.OrgID);
                                        customerorgoutletid = CStr2(location.OutletID);

                                        $('#txtExpenseLocationName').val(location.CompanyName);
                                        $('#popSelectLocation').modal('hide');
                                    })
                                )
                }
            } else {
                $('#listTodaysSchedule').append('<a data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">No schedule for today</a>');
            }

            $('#popSelectLocation').modal('show');
        }

        function SyncNow() {
            var countUnflushedData = Expenses.CountUnflushedData();
            
            if (countUnflushedData > 0){
                NewMessageBox('You have ' + countUnflushedData + ' unflushed data. Please flush the data first before you can sync data from server.', 'Message');
            } else {
                Bs.Loading('show', 'Please wait while getting data from server', 'Message');
                AjaxSend({
                    url: 'Expenses/GetLatestData',
                    data: {
                        data: {
                            PeopleID: sessionStorage.PeopleID,
                            OrgID: sessionStorage.OrgID
                        }
                    }, success: function (e) {
                        var expenses = e.Expenses.Rows;
                        var receivefunds = e.ReceiveFunds.Rows;
                        var expensescache = e.ExpensesCache.Rows;
                        var receivefundscache = e.ReceiveFundsCache.Rows;
                        var netfundbalance = e.NetFundBalances.Rows;
                        var checkinout = e.CheckInOut.Rows;
                        var routeschedule = e.RouteSchedules.Rows;
                        var routescheduleemployee = e.RouteSchedulesEmployees.Rows;
                        var isError = false;

                        Sql.BeginTransaction();

                        for (var i = 0; i < expenses.length; i++) {
                            var data = expenses[i];
                            var result = Expenses.SaveLogExpense({
                                ID: CStr2(data.ID),
                                ExpenseDate: $.format.date(new Date(data.ExpenseDate), 'yyyy-MM-dd'),
                                ExpenseType: CStr2(data.ExpenseType),
                                From: CStr2(data.FromLocation),
                                To: CStr2(data.ToLocation),
                                Amount: CDec2(data.Amount),
                                FowstID: CStr2(data.FowstID),
                                CostCenter: CStr2(data.CostCenter),
                                AssociatedTo: CStr2(data.AssociatedTo),
                                Remarks: CStr2(data.Remarks),
                                GPSLocation: CStr2(data.GPSLocation),
                                IsFromCheckInOut: CBool(data.IsFromCheckInOut),
                                DateImported: CStr2(data.DateImported),
                                EncodedBy: CStr2(data.EncodedBy),
                                LastUpdatedBy: CStr2(data.LastUpdatedBy),
                                DateCreated: CStr2(data.DateCreated),
                                LastUpdated: CStr2(data.LastUpdated),
                                IsDelete: CInt2(data.IsDelete),
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < receivefunds.length; i++) {
                            var data = receivefunds[i];
                            var result = Expenses.SaveReceiveFund({
                                ID: CStr2(data.ID),
                                ReceiveDate: CStr2(data.ReceiveDate),
                                ReceiveFrom: CStr2(data.ReceiveFrom),
                                OtherPerson: CStr2(data.OtherPerson),
                                Amount: CDec2(data.Amount),
                                Comment: CStr2(data.Comment),
                                IsDelete: CInt2(CBool(data.IsDelete)), // added on August 1, 2020 by arnie
                                DateCreated: CStr2(data.DateCreated),
                                LastUpdated: CStr2(data.LastUpdated),
                                DateImported: CStr2(data.DateImported),
                                PeopleID: CStr2(data.PeopleID),
                                LastUpdatedBy: CStr2(data.LastUpdatedBy),
                                EncodedBy: CStr2(data.EncodedBy),
                                FowstID: CStr2(data.FowstID)
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < checkinout.length; i++) {
                            var data = checkinout[i];
                            var result = Expenses.SaveCheckInOut({
                                ID: CStr2(data.ID),
                                CheckInLocationName: CStr2(data.CheckInLocationName),
                                CheckInGPSLocation: CStr2(data.CheckInGPSLocation),
                                CheckIn: CStr2(data.CheckIn) == '' ? '' : $.trim($.format.date(new Date(CStr2(data.CheckIn)), 'yyyy-MM-dd HH:mm')),
                                CheckInLogExpenseID: CStr2(data.CheckInLogExpenseID),
                                CustomerOrgID: CStr2(data.CheckInCustomerOrgID),
                                CustomerOrgOutletID: CStr2(data.CheckInCustomerOrgOutletID),
                                CheckOutLocationName: CStr2(data.CheckOutLocationName),
                                CheckOutGPSLocation: CStr2(data.CheckOutGPSLocation),
                                CheckOut: CStr2(data.CheckOut) == '' ? '' : $.trim($.format.date(new Date(CStr2(data.CheckOut)), 'yyyy-MM-dd HH:mm')),
                                CheckOutLogExpenseID: CStr2(data.CheckOutLogExpenseID),
                                CustomerOrgID: CStr2(data.CheckOutCustomerOrgID),
                                CustomerOrgOutletID: CStr2(data.CheckOutCustomerOrgOutletID),
                                Remarks: CStr2(data.Remarks),
                                LastUpdated: CStr2(data.LastUpdated),
                                DateImported: CStr2(data.DateImported),
                                PeopleID: CStr2(data.PeopleID)
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < netfundbalance.length; i++) {
                            var data = netfundbalance[i];
                            var result = Expenses.SaveNetFundBalance({
                                ID: CStr2(data.ID),
                                NetFundBalance: CStr2(data.NetFundBalance),
                                MonthCovered: CStr2(data.MonthCovered),
                                YearCovered: CStr2(data.YearCovered),
                                IsAdjustment: CBool(data.IsAdjustment),
                                OriginalAmount: CDec2(data.OriginalAmount),
                                AdjustedAmount: CDec2(data.AdjustedAmount),
                                Status: CStr2(data.Status),
                                RequestUpdatedBy: CStr2(data.RequestUpdatedBy),
                                AdjustmentType: CStr2(data.AdjustmentType),
                                RequestedDate: CStr2(data.RequestedDate),
                                IsCancelRequest: CBool(data.AdjustmentType),
                                Remarks: CStr2(data.Remarks),
                                LastUpdated: CStr2(data.LastUpdated),
                                DateImported: CStr2(data.DateImported),
                                PeopleID: CStr2(data.PeopleID),
                                FowstID: CStr2(data.FowstID),
                                EncodedBy: CStr2(data.EncodedBy)
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < expensescache.length; i++) {
                            var data = expensescache[i];
                            var result = Expenses.SaveLogExpenseCache({
                                ID: CStr2(data.ID),
                                ExpenseType: CStr2(data.ExpenseType),
                                Amount: CStr2(data.Amount),
                                MonthCovered: CStr2(data.MonthCovered),
                                YearCovered: CStr2(data.YearCovered),
                                LastUpdated: CStr2(data.LastUpdated),
                                NoOfRecords: CStr2(data.NoOfRecords),
                                PeopleID: CStr2(data.PeopleID),
                                FowstID: CStr2(data.FowstID)
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < receivefundscache.length; i++) {
                            var data = receivefundscache[i];
                            var result = Expenses.SaveReceiveFundCache({
                                ID: CStr2(data.ID),
                                MonthCovered: CStr2(data.MonthCovered),
                                YearCovered: CStr2(data.YearCovered),
                                LastUpdated: CStr2(data.LastUpdated),
                                Amount: CStr2(data.Amount),
                                PeopleID: CStr2(data.PeopleID),
                                FowstID: CStr2(data.FowstID)
                            }, true)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        for (var i = 0 ; i < routeschedule.length; i++) {
                            var data = routeschedule[i];
                            var result = Expenses.SaveRouteSchedules(data)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        if (routeschedule.length > 0) {
                            var ids = [];
                            for (var i in routeschedule) {
                                ids.push('\'' + routeschedule[i].ID + '\'')
                            }
                            Expenses.DeleteRouteSchedules(ids.join(','));
                        }

                        for (var i = 0 ; i < routescheduleemployee.length; i++) {
                            var data = routescheduleemployee[i];
                            var result = Expenses.SaveRouteSchedulesEmployees(data)

                            if (!IsGuid(result)) {
                                Bs.Loading('hide');
                                isError = true;
                                NewMessageBox('There\'s an error while updating your data in your local database. Please ask assistant from Mobiz representative.', 'Message');
                                break
                            }
                        }

                        if (isError) {
                            Sql.RollbackTransaction()
                            return
                        }

                        Sql.CommitTransaction();

                        Bs.Loading('hide');
                        NewMessageBox('Your local database has been updated.', 'Success', undefined, {
                            Close: function () {
                                $('#pageExpenseMain').pageshow();
                            }
                        });
                    },
                    error: function (e) {
                        Bs.Loading('hide');
                        NewMessageBox('Internet connection required', 'Message');
                    }
                })
            }
        }

        function ShowCarryOverFund() {
            var prevmonth = new Date(new Date().setMonth(new Date().getMonth() - 1));
            var carryoverfund = Expenses.CarryOverFunds($('#selFundReportFowst').val()).Rows;

            if (carryoverfund.length > 0) {
                carryoverfund = CDec2(carryoverfund[0].TotalBalance);
            } else {
                carryoverfund = 0;
            }

            $('.lblCarryOverDateCovered').text($.format.date(prevmonth, 'MMM yyyy'));
            $('#lblRemainingFund').text(AccountingFormat(carryoverfund));
            $('.lblSelectedBranchReport').text($('#selFundReportFowst option:selected').text());
            $('#pageCarryOverFunds').pageshow();

            var req = Expenses.RequestAdjustFund().Rows;

            if (req.length > 0) {
                $('#lblRAFStatus').text(req[0].Status);
                $('#lblRAFRequestedDate').text($.format.date(new Date(req[0].RequestedDate), 'MMM dd, yyyy'));
                $('#lblRAFAmount').text(req[0].AdjustedAmount == 0 ? 0 : AccountingFormat(req[0].AdjustedAmount));
                $('#divRAF').show();

                if(CStr2(req[0].Remarks) != ''){
                    $('#lblRAFRemarks').text(req[0].Remarks);
                    $('#lblRAFRemarks').parents().closest('div.divTable').show();
                } else {
                    $('#lblRAFRemarks').parents().closest('div.divTable').hide();
                }

                $('#btnCheckUpdatesRAF').click(function () {
                    Bs.Loading('show', 'Getting updates from server. Please wait ...', 'Message');

                    AjaxSend({
                        url: 'Expenses/GetUpdatesRAF',
                        data: {
                            data: {
                                PeopleID: sessionStorage.PeopleID,
                                OrgID: sessionStorage.OrgID,
                                ReqID: req[0].ID
                            }
                        }, success: function (e) {
                            Bs.Loading('hide');
                            var vRows = e.Results;

                            if (vRows.Rows != undefined) {
                                if (vRows.Rows.length == 0) {
                                    NewMessageBox('Your request is still pending for approval.', 'Message');
                                    return;
                                }

                                var res = Expenses.UpdateRAF(vRows.Rows[0]);

                                if (res == 'saved') {
                                    ShowCarryOverFund();
                                    GetFundsHeader();
                                    NewMessageBox('Your request adjusted fund for ' + $.format.date(prevmonth, 'MMM yyyy')
                                        + ' has been ' + vRows.Rows[0].Status + ' by ' + vRows.Rows[0].RequestUpdatedByName + ' on ' + $.format.date(new Date(vRows.Rows[0].LastUpdated), 'MM/dd/yyyy hh:mm a'),
                                        'Success')                                    
                                } else {
                                    NewMessageBox(res, 'Error Message');
                                }                                
                            } else {
                                NewMessageBox(vRows, 'Server Error Messgae');
                            }
                        }, error: function (e) {
                            Bs.Loading('hide');
                            NewMessageBox('Internet connection required', 'Message');
                        }
                    })
                })

                $('#btnCancelRAF').click(function () {
                    NewMessageBox('Are you sure you want to cancel your request fund adjustment?', 'Message', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                var res = Expenses.CancelRequestFA(req[0].ID);

                                if (res == 'saved') {
                                    Android.Toast('Request has been successfully canceled');
                                    ShowCarryOverFund();
                                } else {
                                    NewMessageBox(res, 'Message');
                                }
                            }
                        },
                        { Text: 'No' }
                    ]);
                })
            } else {
                $('#divRAF').hide();
            }
        }

        function GetMoreDetailsOfCheckInOut() {
            AjaxSend({
                url: 'Expenses/GetMoreDetailsOfCheckInOut',
                data: {
                    data: {
                        PeopleID: sessionStorage.PeopleID,
                        OrgID: sessionStorage.OrgID,
                        Order: $('#selMoreDetailsCheckinSorting').val()
                    }
                }, success: function (e) {
                    Bs.Loading('hide');

                    var vRows = e.Results.Rows;
                    var listCheckinoutHeaderMoreDetails = $('#listCheckinoutHeaderMoreDetails');

                    listCheckinoutHeaderMoreDetails.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            listCheckinoutHeaderMoreDetails.append(CheckInOutList(vRows[i], (i + 1))
                                    .data('data', vRows[i]))
                        }
                    } else {
                        listCheckinoutHeaderMoreDetails.append($('<li class="list-group-item">')
                            .text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy'))
                        )
                    }

                    $('#popCheckinoutMoreDetails').modal('show');
                }, error: function (e) {
                    Bs.Loading('hide');
                    NewMessageBox('Internet connection required', 'Message');
                }
            })
        }

        function GetFundsHeaderDetails() {
            var vRows = Expenses.GetReceiveFundsHeaderDetails($('#selFundReportFowst').val()).Rows;
            var totalamount = 0;
            var listFundReceivedReportHeaderDetails = $('#listFundReceivedReportHeaderDetails');

            listFundReceivedReportHeaderDetails.empty();

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    totalamount += CDec2(vRows[i].Amount);
                    listFundReceivedReportHeaderDetails.append(FundList(vRows[i], (i + 1), false)
                            .data('data', vRows[i])
                            .click(function () {
                                var data = $(this).data('data');

                                ClearFieldReceiveFunds();
                                $('#pageExpenseReceiveFunds').pageshow();

                                isEditFund = true;
                                fundid = data.ID;

                                $('#txtFundReceivingDate').val($.format.date(new Date(data.ReceiveDate), 'yyyy-MM-dd'));
                                $('#txtFundReceivedAmount').val(data.Amount);
                                $('#txtFundReceivingComment').val(data.Comment);
                                $('#selFundReceivingFowst').val(data.FowstID);

                                if (CStr2(data.ReceiveFrom) == '') {
                                    receivefundfrom = null;
                                    $('#txtFundReceivingOtherPerson').val(data.PeopleName);
                                } else {
                                    receivefundfrom = data.ReceiveFrom;
                                }

                                DisplayReceiveFrom(data);

                                $('.form-control').removeClass('is-invalid');
                            })
                        )
                }
            } else {
                listFundReceivedReportHeaderDetails.append($('<li class="list-group-item">')
                        .text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy'))
                    )
            }

            $('#lblSubTotalReceivedFunds').text(AccountingFormat(totalamount));
            $('#lblNetTotalReceivedFunds').text(AccountingFormat(Expenses.GetReceiveFundsHeader($('#selFundReportFowst').val()).Rows[0].TotalAmount));
        }

        function FundList(data, orderno, ismoredetails) {
            data.Title = $.format.date(new Date(data.ReceiveDate), 'MMM dd, yyyy') + ' from ' + data.PeopleName;
            return $('<li class="list-group-item">')
                    .append($('<div style="display:table; width:100%;">')
                        .append($('<div style="display:table-cell;">')
                            .append($('<div style="color: #909090;">')
                                .append(orderno + '. ')
                                .append(data.Title + '<br>')
                            )
                            .append($('<div style="font-size:14px; color: #909090; display: ' + (CStr2(data.Comment) == '' ? 'none' : 'block') + '">')
                                .text('Comment: ' + data.Comment)
                            )
                        )
                        .append($('<div style="display:table-cell; text-align:right; vertical-align:middle;">')
                            .append(AccountingFormat(data.Amount))
                        )
                        .append($('<div style="display: ' + (ismoredetails ? 'none' : 'table-cell') + '; text-align:right; vertical-align:middle; width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .data('data', data)
                                .append($('<span class="bi-x">'))
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var data = $(this).data('data');

                                    NewMessageBox('Are you sure you want to remove record <b>"' + data.Title + '"</b>? <br><br>' +
                                        'If Yes, you cannot undo this deletion.', 'Message', [
                                        {
                                            Text: 'Yes', Size: 2, Click: function () {
                                                if (Expenses.DeleteFund(data)) {
                                                    Android.Toast('Successfully deleted');
                                                    GetFundsHeaderDetails();
                                                    GetFundsHeader();
                                                } else {
                                                    Android.Toast('There\'s an error in deleting a record');
                                                }
                                            }
                                        }, { Text: 'No', Size: 2 }
                                    ])
                                })
                            )
                        )
                    )
        }

        function GetFundsHeader() {
            var fowstid = $('#selFundReportFowst').val();
            var vRows = Expenses.GetReceiveFundsHeader(fowstid).Rows;
            var totalReceiveFunds = 0, carryoverfund = 0, totalbegfundbal = 0, totalexpenses = 0;

            $('.lblFundsDateCovered').text($.format.date(new Date(), 'MMM. yyyy'));

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    totalReceiveFunds += CDec2(vRows[i].TotalAmount);
                }

                $('.lblTotalReceiveFundsHeader').text(AccountingFormat(totalReceiveFunds));
            }

            carryoverfund = Expenses.CarryOverFunds(fowstid).Rows;

            if (carryoverfund.length > 0) {
                carryoverfund = CDec2(carryoverfund[0].TotalBalance);
            } else {
                carryoverfund = 0;
            }

            $('#lblCarryOverFund').text(AccountingFormat(carryoverfund));

            totalbegfundbal = totalReceiveFunds + carryoverfund;
            $('#lblTotalBegFundBal').text(AccountingFormat(totalbegfundbal));

            vRows = Expenses.GetLogExpenseHeader(fowstid).Rows;

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    totalexpenses += CDec2(vRows[i].TotalAmount);
                }

                $('.lblTotalExpenseHeader').text(AccountingFormat(totalexpenses));
            } else {
                $('.lblTotalExpenseHeader').text(AccountingFormat(0));
            }

            var netfund = totalbegfundbal - totalexpenses;
            $('#lblNetFundBalance').text(AccountingFormat(netfund));
        }

        function SaveReceiveFund() {
            var isValid = true;
            var txtFundReceivingDate = $('#txtFundReceivingDate');
            var btnFundReceivedFrom = $('#btnFundReceivedFrom');
            var txtFundReceivedAmount = $('#txtFundReceivedAmount');
            var txtFundReceivingComment = $('#txtFundReceivingComment');
            var txtFundReceivingOtherPerson = $('#txtFundReceivingOtherPerson');
            var selFundReceivingFowst = $('#selFundReceivingFowst');

            $('.form-control').removeClass('is-invalid');
            $('[for="btnFundReceivedFrom"]').css('color', 'inherit');

            if ($.trim(txtFundReceivingDate.val()) == '') {
                txtFundReceivingDate.addClass('is-invalid');
                isValid = false;
            }

            if ($.trim(selFundReceivingFowst.val()) == '') {
                selFundReceivingFowst.addClass('is-invalid');
                isValid = false;
            }

            if (receivefundfrom == '') {
                $('[for="btnFundReceivedFrom"]').css('color', '#dc3545');
                btnFundReceivedFrom.addClass('is-invalid');
                isValid = false;
            }

            if ($.trim(txtFundReceivedAmount.val()) == '') {
                txtFundReceivedAmount.addClass('is-invalid');
                isValid = false;
            }

            if (!isValid) {
                Android.Toast('Please fill in the required fields');
                return false;
            }

            var data = {
                ID: fundid,
                ReceiveDate: $.trim(txtFundReceivingDate.val()),
                ReceiveFrom: receivefundfrom,
                OtherPerson: $.trim(txtFundReceivingOtherPerson.val()),
                Amount: CDec2(txtFundReceivedAmount.val()),
                Comment: $.trim(txtFundReceivingComment.val()),
                FowstID: $.trim(selFundReceivingFowst.val()), // added on August 1, 2020 by arnie
                IsDelete: 0
            }

            var receiveFundResult = Expenses.SaveReceiveFund(data);

            if (IsGuid(receiveFundResult)) {
                Android.Toast('Successfully saved');

                if (isEditFund) {
                    GetFundsHeaderDetails();
                    GetFundsHeader();
                    $('#pageFundReceivedReportsHeaderDetails').pageshow();
                } else {
                    $('#pageExpenseMain').pageshow();
                }
            } else {
                Android.Toast('There\'s an error while saving your received fund. Please ask assistance from Mobiz representive');
            }
        }

        function DisplayReceiveFrom(e) {
            var divReceiveFrom = $('#divReceiveFrom');
            divReceiveFrom.empty();

            divReceiveFrom.append($('<div class="alert alert-warning" role="alert">')
                    .append($('<table style="width:100%">')
                        .append($('<tr>')
                            .append($('<td>').text(e.PeopleName))
                            .append($('<td style="width:50px;">')
                                .append($('<button data-role="none" class="btn btn-danger btn-close" style="float:right;">')
                                    .data('data', e)
                                    .append($('<span class="bi-x">'))
                                    .click(function () {
                                        var dis = $(this);
                                        var data = dis.data('data');

                                        NewMessageBox('Are you sure you want to remove <b>' + data.PeopleName + '</b>?', 'Message', [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    dis.parent().remove();
                                                    receivefundfrom = '';
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )
                        )
                    )                    
                )

            $('[for="btnFundReceivedFrom"]').css('color', 'inherit');
        }

        function GetGPSLocation() {
            var timer = 6;
            var currentLocation = GetLatestGPSLocation();

            $('#txtExpenseCheckinoutGPSCoordinates').attr('isgps', 0);

            if (CStr2(currentLocation) != '') {
                currentLocation = currentLocation.split('-');
                $('#txtExpenseCheckinoutGPSCoordinates').attr('isgps', 1).val(currentLocation[0] + ',' + currentLocation[1]).removeClass('is-invalid');
            } else {
                var gpsInterval = setInterval(function (e) {
                    timer -= 1

                    if (timer < 0) {
                        currentLocation = GetLatestGPSLocation();

                        if (CStr2(currentLocation) != '') {
                            currentLocation = currentLocation.split('-');
                            $('#txtExpenseCheckinoutGPSCoordinates').attr('isgps', 1).val(currentLocation[0] + ',' + currentLocation[1]).removeClass('is-invalid');
                            clearInterval(gpsInterval);
                        } else {
                            timer = 6;
                        }
                    } else {
                        $('#txtExpenseCheckinoutGPSCoordinates').attr('isgps', 0).val('Getting GPS location in ' + timer + ' sec(s), please wait ...')
                    }
                }, 1000)
            }       
        }

        function GetCheckinDetails() {
            var vRows = Expenses.GetCheckinDetails($('#selCheckinSorting').val()).Rows;
            var listExpenseCheckinReports = $('#listExpenseCheckinReports');

            listExpenseCheckinReports.empty();

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    listExpenseCheckinReports.append(CheckInOutList(vRows[i], (i + 1)))
                }
            } else {
                listExpenseCheckinReports.append($('<h3>').text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy')))
            }
        }

        function CheckInOutList(data, orderno) {
            var duration = '', secs = '', mins = '', hours = '', days = '';

            if (CStr2(data.CheckOut) != '') {
                secs = new Date(CStr2(data.CheckOut)) - new Date(CStr2(data.CheckIn));
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
            }

            return $('<li class="list-group-item">')
                    .append($('<div style="display:table; width: 100%;">')
                        .append($('<div style="display:table-row;">')
                            .append($('<div style="display:table-cell; width:40px; vertical-align:middle;">')
                                .append(orderno + '. ')
                            )
                            .append($('<div style="display:table-cell;">')                                
                                .append((CStr2(data.CheckIn) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">').append($('<span class="bi-geo-alt-fill" style="color:#dc3545;">')))
                                    .append($('<div style="width:100%;">').append(CStr2(data.CheckInLocationName)))
                                ))
                                .append((CStr2(data.CheckIn) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">').append($('<span class="bi-arrow-down-square-fill" style="color:#4CAF50;">')))
                                    .append($('<div style="width:100%;">').append($.format.date(new Date(CStr2(data.CheckIn)), 'MM/dd/yyyy hh:mm a')))
                                ))
                                .append((CStr2(data.CheckinAmount) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">'))
                                    .append($('<div style="width:100%;">').append(' - ' + (CStr2(data.CheckinFromLocation) == '' ? '' : 'From ' + data.CheckinFromLocation) +
                                            (CStr2(data.CheckinToLocation) == '' ? '' : ' to ' + data.CheckinToLocation) +
                                            ' for only ' + AccountingFormat(data.CheckinAmount)))
                                ))
                                //.append((CStr2(data.CheckOut) == '' ? '' : $('<div style="display:flex;">')
                                //    .append($('<div style="width:30px;">').append($('<span class="bi-arrow-up-square-fill" style="color:#FF9800;">')))
                                //    .append($('<div style="width:100%;">').append(CStr2(data.CheckOutLocationName)))
                                //))
                                .append((CStr2(data.CheckOut) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">').append($('<span class="bi-arrow-up-square-fill" style="color:#FF9800;">')))
                                    .append($('<div style="width:100%;">').append($.format.date(new Date(CStr2(data.CheckOut)), 'MM/dd/yyyy hh:mm a')))
                                ))
                                .append((CStr2(data.CheckoutAmount) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">'))
                                    .append($('<div style="width:100%;">').append(' - ' + (CStr2(data.CheckoutFromLocation) == '' ? '' : 'From ' + data.CheckoutFromLocation) +
                                            (CStr2(data.CheckoutToLocation) == '' ? '' : ' to ' + data.CheckoutToLocation) +
                                            ' for only ' + AccountingFormat(data.CheckoutAmount)))
                                ))
                                .append((duration == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">').append($('<span class="bi-clock-fill" style="color:#FF5722;">')))
                                    .append($('<div style="width:100%;">').append(duration))
                                ))
                                .append((CStr2(data.Remarks) == '' ? '' : $('<div style="display:flex;">')
                                    .append($('<div style="width:30px;">').append($('<span class="bi-chat-text-fill" style="color:#2196f3;">')))
                                    .append($('<div style="width:100%;">').append(CStr2(data.Remarks)))
                                ))
                            )
                        )
                    )                
        }

        function GetLogExpenseHeader() {
            var divExpenseReportHeader = $('#divExpenseReportHeader');
            var vRows = Expenses.GetLogExpenseHeader($('#selFundReportFowst').val()).Rows;
            var totalamount = 0;

            $('#lblExpenseDateCovered').text($.format.date(new Date(), 'MMM. yyyy'));
            $('.lblSelectedBranchReport').text($('#selFundReportFowst option:selected').text());
            divExpenseReportHeader.empty();

            if(vRows.length > 0){
                for (var i = 0; i < vRows.length; i++) {
                    totalamount += CDec2(vRows[i].TotalAmount);
                    divExpenseReportHeader.append($('<div class="alert alert-info" role="alert">')
                            .append($('<strong>').text(vRows[i].ExpenseType))
                            .append($('<span style="float:right;">')
                                .append(AccountingFormat(vRows[i].TotalAmount))
                                .append(' ')
                                .append($('<span class="glyphicon glyphicon-chevron-right">'))
                            )
                            .data('data', vRows[i])
                            .click(function (e) {
                                var data = $(this).data('data');
                                ViewExpenseReportsHeaderDetails(data);                                
                                $('#selExpensePeriod').unbind('change').change(function () {
                                    ViewExpenseReportsHeaderDetails(data);

                                    if($(this).val() == 'today'){
                                        $('#lblExpensePeriod').text('Today\'s Total:');
                                    } else if ($(this).val() == 'week') {
                                        $('#lblExpensePeriod').text('Week-To-Date Total:');
                                    } else if ($(this).val() == 'month') {
                                        $('#lblExpensePeriod').text('Month-To-Date Total:');
                                    }
                                })
                            })
                        )
                }

                $('.lblTotalExpenseHeader').text(AccountingFormat(totalamount));                
            } else {
                divExpenseReportHeader.append($('<h2>').text('No reports found'));
                $('.lblTotalExpenseHeader').text('-');
            }
        }

        function ViewExpenseReportsHeaderDetails(data) {
            $('#pageExpenseReportsHeaderDetails').pageshow();
            $('#lblExpenseTypeHeaderDetails').text(data.ExpenseType);            

            data.Period = $('#selExpensePeriod').val();
            data.Sort = $('#selExpenseSorting').val();
            data.FowstID = $('#selFundReportFowst').val();
            var vRowsCache = Expenses.ViewExpenseReportsCacheHeaderDetails(data).Rows;
            var vRows = Expenses.ViewExpenseReportsHeaderDetails(data).Rows;
            var listExpenseReportHeaderDetails = $('#listExpenseReportHeaderDetails');
            var totalcache = 0, totalamount = 0;

            listExpenseReportHeaderDetails.empty();

            $('#lblMonthName').text($.format.date(new Date(), 'MMMM yyyy'));            

            if (vRowsCache.length > 0) {
                for (var i = 0; i < vRowsCache.length; i++){
                    totalcache += CDec2(vRowsCache[i].Amount);
                }                
            }

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    totalamount += CDec2(vRows[i].Amount);
                    listExpenseReportHeaderDetails.append(ExpenseList(vRows[i], (i + 1), false)
                            .data('data', vRows[i])
                            .click(function () {
                                var data = $(this).data('data');

                                ClearFieldsLogExpense();
                                $('#pageLogExpense').pageshow();

                                isEditLogExpense = true;
                                expenseID = data.ID;
                                associatedTo = data.AssociatedTo;
                                $('#selExpenseType').val(data.ExpenseType);
                                $('#txtExpenseDate').val($.format.date(new Date(data.ExpenseDate), 'yyyy-MM-dd'));
                                $('#txtExpenseFrom').val(data.FromLocation);
                                $('#txtExpenseTo').val(data.ToLocation);
                                $('#txtExpenseAmount').val(data.Amount);
                                $('#selExpenseCostCenter').val(data.CostCenter);
                                $('#txtExpenseRemarks').val(data.Remarks);
                                $('#txtExpenseGPSCoordinates').val(data.GPSLocation);
                                $('#selExpenseCostCenterLocation').val(data.FowstID);

                                if(associatedTo != ''){
                                    DisplayAssociatedTo(data);
                                } else {
                                    associatedTo = '';
                                    DisplayAssociatedTo('');
                                }
                                
                                $('.form-control').removeClass('is-invalid');
                            })
                        )
                }                
            } else {
                listExpenseReportHeaderDetails.append($('<li class="list-group-item">')
                        .text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy'))
                    )
            }

            $('#lblTotalPerExpenseTypePerPeriod').text(AccountingFormat(totalamount));
            $('#lblTotalPerExpenseType').text(AccountingFormat(totalcache + totalamount));

            $('#btnLogExpenseMoreDetails').unbind('click').click(function () {
                NewMessageBox('To save on storage space, some expense details are no longer stored in your local storage. <br> <br> Expense details are saved in server storage whenever you sync your database. These expense details are not saved on your new local database after each sync to conserve your local storage and preserve your device\'s data lookup performance.', 'Message', [
                    {
                        Text: 'View Details Online',
                        Size: 2,
                        Click: function (e) {
                            Bs.Loading('show', 'Please wait ... getting data from server', 'Message');

                            GetMoreDetailsOfExpenseType(data);                            
                        }
                    }, {
                        Text: 'Cancel', Size: 2
                    }
                ])
            })

            $('#selExpenseSorting').unbind('change').change(function () {
                ViewExpenseReportsHeaderDetails(data);
            })
        }

        function GetMoreDetailsOfExpenseType(data) {
            AjaxSend({
                url: 'Expenses/GetMoreDetailsOfExpenseType',
                data: {
                    data: {
                        Period: data.Period,
                        ExpenseType: data.ExpenseType,
                        PeopleID: sessionStorage.PeopleID,
                        OrgID: sessionStorage.OrgID,
                        Sort: data.Sort
                    }
                }, success: function (e) {
                    Bs.Loading('hide');

                    var vRows = e.Results.Rows;
                    var listExpenseReportHeaderMoreDetails = $('#listExpenseReportHeaderMoreDetails');
                    var totalamount = 0;

                    listExpenseReportHeaderMoreDetails.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            totalamount += CDec2(vRows[i].Amount)
                            listExpenseReportHeaderMoreDetails.append(ExpenseList(vRows[i], (i + 1), true)
                                    .data('data', vRows[i]))
                        }
                    } else {
                        listExpenseReportHeaderMoreDetails.append($('<li class="list-group-item">')
                            .text('No result(s) found for the month of ' + $.format.date(new Date(), 'MMMM yyyy'))
                        )
                    }

                    $('#lblTotalPerExpenseTypeMoreDetails').text(AccountingFormat(totalamount));
                    
                    $('#selExpenseMoreDetailsPeriod, #selExpenseMoreDetailsSorting').unbind('change').change(function () {
                        data.Period = $('#selExpenseMoreDetailsPeriod').val();
                        data.Sort = $('#selExpenseMoreDetailsSorting').val();
                        GetMoreDetailsOfExpenseType(data);
                    })

                    $('#popLogExpenseMoreDetails').modal('show');
                }, error: function (e) {
                    Bs.Loading('hide');
                    NewMessageBox('Internet connection required', 'Message');
                }
            })
        }

        function ExpenseList(data, orderno, ismoredetails) {
            data.Title = (CStr2(data.AssociatedTo) == '' ? '' : 'Associated to ' + data.AssociatedTo + ', ') + (CStr2(data.FromLocation) == '' ? '' : (CStr2(data.AssociatedTo) == '' ? 'From ' : 'from ') + data.FromLocation) + (CStr2(data.ToLocation) == '' ? '' : ' to ' + data.ToLocation + ' on ') + $.format.date(new Date(data.ExpenseDate || data.DateCreated), 'MM/dd/yyyy');
            return $('<li class="list-group-item">')
                    .append($('<div style="display:table; width:100%;">')
                        .append($('<div style="display:table-cell; vertical-align:middle;">')
                        .append($('<span style="color: #909090;">')
                                .append(orderno + '. ')
                                .append(data.Title + '<br>')
                            )
                        )
                        .append($('<div style="display:table-cell; text-align:right; vertical-align:middle;">')
                            .append(AccountingFormat(data.Amount))
                        )
                        .append($('<div style="display: ' + (ismoredetails ? 'none' : 'table-cell') + '; text-align:right; vertical-align:middle; width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .data('data', data)
                                .append($('<span class="bi-x">'))
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var data = $(this).data('data');

                                    NewMessageBox('Are you sure you want to remove record <b>"' + data.Title + '"</b>? <br><br>' +
                                        'If Yes, you cannot undo this deletion.', 'Message', [
                                        {
                                            Text: 'Yes', Size: 2, Click: function () {
                                                if (Expenses.DeleteLogExpense(data)) {
                                                    data.ExpenseType = $('#lblExpenseTypeHeaderDetails').text();
                                                    data.Period = $('#selExpensePeriod').val();
                                                    $('#pageExpenseReportsHeaderDetails').pageshow();
                                                    ViewExpenseReportsHeaderDetails(data);
                                                    GetLogExpenseHeader();
                                                } else {
                                                    Android.Toast('There\'s an error in deleting a record')
                                                }
                                            }
                                        }, { Text: 'No', Size: 2 }
                                    ])
                                })
                            )
                        )
                    )
        }

        function AddClientVendor(type) {
            if(type == ''){
                associatedTo = '';
                DisplayAssociatedTo('');
            } else {
                BrowsePopup((type == 'customer' ? 'LibraryOrganizationCustomer' : 'LibraryOrganizationSupplier'), 'Select ' + type,
                [
                    {
                        DisplayText: [{ Column: 'CompanyName' }],
                        OfflineBrowser: (type == 'customer' ? OfflineBrowser.LibraryOrganizationCustomer : OfflineBrowser.LibraryOrganizationSupplier),
                        Click: function (e) {
                            associatedTo = e.ID;
                            DisplayAssociatedTo(e);
                        }
                    }
                ])
            }            
        }

        function DisplayAssociatedTo(e) {
            var divAssociatedTo = $('#divAssociatedTo');
            divAssociatedTo.empty();
            
            divAssociatedTo.append($('<div class="alert alert-warning" role="alert">')
                    .append($('<table style="width:100%">')
                        .append($('<tr>')
                            .append($('<td>').text((e == '' ? 'No Association' : e.CompanyName)))
                            .append($('<td style="width:50px;">')
                                .append($('<button data-role="none" class="btn btn-danger btn-close" style="float:right;">')
                                    .data('data', e)
                                    .append($('<span class="bi-x">'))
                                    .click(function () {
                                        var dis = $(this);
                                        var data = dis.data('data');

                                        NewMessageBox('Are you sure you want to remove <b>' + (data.CompanyName || 'this record') + '</b>?', 'Message', [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    dis.parent().remove();
                                                    associatedTo = '';
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )
                        )
                    )
                )
        }

        function SaveLogExpense() {
            var isValid = true;
            var selExpenseType = $('#selExpenseType').val();
            var txtExpenseFrom = $('#txtExpenseFrom').val();
            var txtExpenseTo = $('#txtExpenseTo').val();
            var txtExpenseAmount = $('#txtExpenseAmount');
            var selExpenseCostCenterLocation = $('#selExpenseCostCenterLocation');
            var selExpenseCostCenter = $('#selExpenseCostCenter').val();
            var txtExpenseRemarks = $('#txtExpenseRemarks');
            var txtExpenseGPSCoordinates = $('#txtExpenseGPSCoordinates');
            var txtExpenseDate = $('#txtExpenseDate');

            $('.form-control, .form-select').removeClass('is-invalid');

            if ($.trim(txtExpenseDate.val()) == '') {
                txtExpenseDate.addClass('is-invalid');
                isValid = false;
            }

            if (selExpenseType == 'Jeepney Fare' || selExpenseType == 'Bus Fare' || selExpenseType == 'Taxi Fare' || selExpenseType == 'Van Fare' ||
                selExpenseType == 'Tricycle Fare' || selExpenseType == 'Air Fare' || selExpenseType == 'Sea Fare' || selExpenseType == 'Train Fare') {
                if ($.trim(txtExpenseFrom) == '') {
                    $('#txtExpenseFrom').addClass('is-invalid');
                    isValid = false;
                }

                if ($.trim(txtExpenseTo) == '') {
                    $('#txtExpenseTo').addClass('is-invalid');
                    isValid = false;
                }

                if ($.trim(txtExpenseFrom) != '' && $.trim(txtExpenseTo) == '') {
                    $('#txtExpenseTo').addClass('is-invalid');
                    isValid = false;
                }

                if ($.trim(txtExpenseTo) != '' && $.trim(txtExpenseFrom) == '') {
                    $('#txtExpenseFrom').addClass('is-invalid');
                    isValid = false;
                }
            }            

            if ($.trim(txtExpenseAmount.val()) == '') {
                txtExpenseAmount.addClass('is-invalid');
                isValid = false;
            }

            if ($.trim(selExpenseCostCenterLocation.val()) == '') {
                selExpenseCostCenterLocation.addClass('is-invalid');
                isValid = false;                
            }

            if ($.trim(txtExpenseGPSCoordinates.val()) == '') {
                txtExpenseGPSCoordinates.addClass('is-invalid');
                isValid = false;
            }

            if (CBool(isLogExpenseRemarksFieldRequired)) {
                if ($.trim(txtExpenseRemarks.val()) == '') {
                    txtExpenseRemarks.addClass('is-invalid');
                    isValid = false;
                }
            }

            //if ($.trim(associatedTo) == '') {
            //    $('#btnExpenseAddClient').addClass('is-invalid');
            //    isValid = false;
            //} else if ($.trim(associatedTo) == 'no') {
            //    associatedTo = ''; // iempty lang
            //}

            if(!isValid){
                Android.Toast('Please fill in the required fields');
                return false;
            }
            
            var data = {
                ID: expenseID,
                ExpenseDate: $.trim(txtExpenseDate.val()),
                ExpenseType: $.trim(selExpenseType),
                From: $.trim(txtExpenseFrom),
                To: $.trim(txtExpenseTo),
                Amount: $.trim(txtExpenseAmount.val()),
                FowstID: $.trim(selExpenseCostCenterLocation.val()),
                CostCenter: $.trim(selExpenseCostCenter),
                AssociatedTo: associatedTo,
                Remarks: $.trim(txtExpenseRemarks.val()),
                GPSLocation: $.trim(txtExpenseGPSCoordinates.val()),
                IsFromCheckInOut: isLogAnExpenseFromCheckInOut,
                IsDelete: 0
            }
            var expenseResult = Expenses.SaveLogExpense(data);

            if (IsGuid(expenseResult)) {
                Transfers.DeleteFile("LastKnownLocation.flg");

                if (CBool(isLogAnExpenseFromCheckInOut)) {

                    if(checktype == 'in'){
                        arrCheckIn.CheckInLogExpenseID = expenseResult;
                    } else {
                        arrCheckIn.CheckOutLogExpenseID = expenseResult;
                    }

                    if (IsGuid(Expenses.SaveCheckInOut(arrCheckIn))) {
                        Android.Toast('Successfully checked-' + checktype);
                        $('#pageExpenseMain').pageshow();
                    } else {
                        Android.Toast('There\'s an error while saving your check in/out data. Please ask assistance from Mobiz representative.');
                    }
                } else {
                    Android.Toast('Successfully saved');

                    if (isEditLogExpense) {
                        $('#pageExpenseReportsHeaderDetails').pageshow();
                        ViewExpenseReportsHeaderDetails(data);
                        GetLogExpenseHeader();
                        GetFundsHeader();
                    } else {
                        $('#pageExpenseMain').pageshow();
                    }
                }

                SalesPaymentsExport_offline().FlushDeviceData({
                    Success: function (vData) {
                        //do nothing
                    }
                });
            } else {
                Android.Toast('There\'s an error while saving your log expense. Please ask assistance from Mobiz representative.');
            }
        }

        function SaveCheckInOut(callback) {
            var isValid = true;
            var txtExpenseLocationName = $('#txtExpenseLocationName');
            var txtExpenseCheckinoutGPSCoordinates = $('#txtExpenseCheckinoutGPSCoordinates');
            var txtExpenseCheckIn = $('#txtExpenseCheckIn');
            var txtExpenseCheckOut = $('#txtExpenseCheckOut');
            var txtExpenseCheckinoutRemarks = $('#txtExpenseCheckinoutRemarks');

            $('.form-control').removeClass('is-invalid');

            if ($.trim(txtExpenseLocationName.val()) == '') {
                isValid = false;
                txtExpenseLocationName.addClass('is-invalid');
            }

            if (!CBool(txtExpenseCheckinoutGPSCoordinates.attr('isgps'))) {
                isValid = false;
                txtExpenseCheckinoutGPSCoordinates.addClass('is-invalid');
                NewMessageBox('GPS Coordinates are not valid. Please make sure your phone GPS is on or you must connected to wifi or mobile network.', 'Message');
            }

            if(!isValid){
                Android.Toast('Please fill in the required fields');
                return false;
            }

            var data = {
                ID: checkinID,                
                CheckIn: $.trim($.format.date(new Date(txtExpenseCheckIn.val()), 'yyyy-MM-dd HH:mm')),
                CheckOut: $.trim($.format.date(new Date(txtExpenseCheckOut.val()), 'yyyy-MM-dd HH:mm')),
                Remarks: $.trim(txtExpenseCheckinoutRemarks.val()),
                Type: checktype,
                CustomerOrgID: customerorgid,
                CustomerOrgOutletID: customerorgoutletid
            }

            if (checktype == 'in') {
                data.CheckInLocationName = $.trim(txtExpenseLocationName.val());
                data.CheckInGPSLocation = $.trim(txtExpenseCheckinoutGPSCoordinates.val());
            } else {
                data.CheckOutLocationName = $.trim(txtExpenseLocationName.val());
                data.CheckOutGPSLocation = $.trim(txtExpenseCheckinoutGPSCoordinates.val());
            }

            if (CBool(Expenses.ScheduleInfo().StrictlyFollowSched) && checktype == 'in') {
                Expenses.ValidateCheckIn(data, function () {
                    Go();
                })
            } else {
                Go();
            }
            
            function Go() {
                if (typeof callback == 'function') callback(data);

                if (CBool(isLogAnExpenseFromCheckInOut)) {
                    return data;
                } else {
                    var res = Expenses.SaveCheckInOut(data);

                    if (IsGuid(res)) {
                        Transfers.DeleteFile("LastKnownLocation.flg");
                        Android.Toast('Successfully checked-' + checktype);
                        $('#pageExpenseMain').pageshow();
                        SalesPaymentsExport_offline().FlushDeviceData({
                            Success: function (vData) {
                                //do nothing
                            }
                        });
                    } else {
                        Android.Toast('There\'s an error while saving your Check-in/out data. Please ask assistance from Mobiz representative.');
                    }
                }
            }
        }

        function GetCostCenters() {
            var selExpenseCostCenter = $('#selExpenseCostCenter');
            var vRows = Expenses.GetCostCenters().Rows;

            selExpenseCostCenter.empty();

            if (vRows.length > 0) {
                selExpenseCostCenter.append(new Option('Please select', ''));

                for (var i = 0; i < vRows.length; i++){
                    selExpenseCostCenter.append(new Option(vRows[i].Name, vRows[i].ID));
                }
            }
        }

        function GetFowst() {
            var vRows = OfflineBrowser.LibraryFowst.GetData({ keywords: '' }).Rows;
            var selExpenseCostCenterLocation = $('#selExpenseCostCenterLocation');
            var selFundReceivingFowst = $('#selFundReceivingFowst');
            var selFowstCarryOverFund = $('#selFowstCarryOverFund');
            var selFundReportFowst = $('#selFundReportFowst');

            selExpenseCostCenterLocation.empty();
            selFundReceivingFowst.empty();
            selFowstCarryOverFund.empty();
            selFundReportFowst.empty();

            if (vRows.length > 1) {
                selExpenseCostCenterLocation.append(new Option('Please select', ''));
                selFundReceivingFowst.append(new Option('Please select', ''));
                selFowstCarryOverFund.append(new Option('Please select', ''));

                for (var i = 0; i < vRows.length; i++) {
                    selExpenseCostCenterLocation.append(new Option(vRows[i].FowstName, vRows[i].ID));
                    selFundReceivingFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                    selFowstCarryOverFund.append(new Option(vRows[i].FowstName, vRows[i].ID));
                    selFundReportFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                }
            } else {
                selExpenseCostCenterLocation.append(new Option(vRows[0].FowstName, vRows[0].ID));
                selFundReceivingFowst.append(new Option(vRows[0].FowstName, vRows[0].ID));
                selFowstCarryOverFund.append(new Option(vRows[0].FowstName, vRows[0].ID));
                selFundReportFowst.append(new Option(vRows[0].FowstName, vRows[0].ID));
            }
        }

        function ClearFieldsLogExpense() {
            expenseID = '';
            associatedTo = '';
            isEditLogExpense = false;
            $('#txtExpenseFrom').val('');
            $('#txtExpenseTo').val('');
            $('#txtExpenseAmount').val('');
            $('#selExpenseCostCenter').val('');
            $('#txtExpenseRemarks').val('');
            $('#txtExpenseGPSCoordinates').val('');
            $('#selExpenseCostCenterLocation').val('');
            $('#divAssociatedTo').empty();
            $('.form-control').removeClass('is-invalid');
        }

        function ClearFieldsCheckinout() {
            checkinID = '';
            customerorgid = '';
            customerorgoutletid = '';
            $('#txtExpenseLocationName').val('').removeClass('is-invalid');
            $('#txtExpenseCheckinoutGPSCoordinates').val('').removeClass('is-invalid');
            $('#txtExpenseCheckIn').val('');
            $('#txtExpenseCheckOut').val('');
            $('#txtExpenseCheckinoutRemarks').val('');
            $('.form-control').removeClass('is-invalid');
        }

        function ClearFieldReceiveFunds() {
            receivefundfrom = '';
            fundid = '';
            isEditFund = false;
            $('#txtFundReceivingDate').val($.format.date(new Date(), 'yyyy-MM-dd'));
            $('#divReceiveFrom').empty();
            $('#txtFundReceivedAmount, #txtFundReceivingOtherPerson, #txtFundReceivingComment').val('');
            $('#selFundReceivingFowst').val('');
        }
    })
})
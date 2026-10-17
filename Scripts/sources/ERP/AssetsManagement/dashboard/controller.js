define([
    //'promise!/Scripts/sources/ERP/AssetsManagement/dashboard/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AssetsManagement/dashboard/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.AssetsManagement.dashboard.controller', null, function () {

        var mOut = {}, dtAssetsDashboard, dtPreviousCheckedIn;
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;        

        $('#tabAssetsMgmtDashboard').empty().append(mHtml).trigger('create');

        $('#txtDashboardAsofDate').val($.format.date(new Date(), 'yyyy-MM-dd'));

        $('#btnAssetDashboardRefresh').click(function () {
            GetDashboardData();
        })

        $('#btnAssetScheduledCheckout').click(function () {
            $('#lblDashboardGridTitle').text('Scheduled Check-out for Today')
            DashboardGridDetails('scheduledcheckout', $(this).data('data'), 'ScheduledCheckout')
        })

        $('#btnAssetCheckIn').click(function () {
            $('#lblDashboardGridTitle').text('Checked In Assets Details')
            DashboardGridDetails('checkedin', $(this).data('data'), 'NoOfCheckedIn')
        })

        $('#btnAssetReservation').click(function () {
            $('#lblDashboardGridTitle').text('Reservation Details')
            DashboardGridDetails('reserved', $(this).data('data'), 'NoOfReservation')
        })

        $('#btnAssetCancelled').click(function () {
            $('#lblDashboardGridTitle').text('Cancelled Assets Details')
            DashboardGridDetails('cancelled', $(this).data('data'), 'NoOfCancelled')
        })

        $('#btnAssetOccupied').click(function () {
            $('#lblDashboardGridTitle').text('Occupied / Used Assets Details')
            DashboardGridDetails('occupied', $(this).data('data'), 'NoOfOccupied')
        })

        $('#btnAssetUnoccupied').click(function () {
            $('#lblDashboardGridTitle').text('Unoccupied / Unused Assets Details')
            DashboardGridDetails('unoccupied', $(this).data('data'), 'NoOfUnoccupiedUnused')
        })

        $('#btnAssetReserved').click(function () {
            $('#lblDashboardGridTitle').text('Reserved Assets Details')
            DashboardGridDetails('reserved', $(this).data('data'), 'NoOfReserved')
        })

        $('#btnViewPastCheckIns').click(function () {
            var date = $.format.date(new Date(), 'MMM dd, yyyy');
            $('#btnPrevCheckedInDate').text(date);
            $('#txtSearchClientPrevCheckedIn').val('');
            GetPreviousCheckedIn({
                Date: date,
                Search: ''
            })
        })

        $('#btnPrevCheckedInDate').click(function () {
            SingleCalendar({
                MaxDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                Click: function (e) {
                    $('#btnPrevCheckedInDate').text(e);
                    $('#txtSearchClientPrevCheckedIn').val('');
                    GetPreviousCheckedIn({
                        Date: e,
                        Search: ''
                    })
                }
            })
        })

        $('#txtSearchClientPrevCheckedIn').keydown(function (e) {
            if(e.keyCode == 13){
                if($.trim($(this).val())){
                    GetPreviousCheckedIn({
                        Date: '',
                        Search: $.trim($(this).val())
                    })
                }
            }
        })

        GetDashboardData();

        function GetPreviousCheckedIn(data) {
            AjaxSend({
                url: 'AssetsManagement/GetPreviousCheckedIn',
                data: data,
                success: function (e) {
                    DisplayPreviousCheckedIn(e.Results.Rows);
                }
            })
        }

        function GetDashboardData() {
            AjaxSend({
                url: 'AssetsManagement/GetDashboardData',
                data: { date:  $.format.date(new Date(), 'yyyy-MM-dd') },
                success: function (e) {
                    var NoOfScheduledCheckoutToday = e.ScheduledCheckout.Rows;
                    var NoOfOccupiedToday = e.NoOfOccupiedToday.Rows;
                    var NoOfUnoccupiedUnusedToday = e.NoOfUnoccupiedUnusedToday.Rows;
                    var NoOfReservedToday = e.NoOfReservedToday.Rows;
                    var NoOfCheckedInToday = e.NoOfCheckedInToday.Rows;
                    var NoOfReservationMadeToday = e.NoOfReservationMadeToday.Rows;
                    var NoOfCancelledToday = e.NoOfCancelledToday.Rows;

                    //NoOfUnoccupiedUnusedToday.push.apply(NoOfUnoccupiedUnusedToday, NoOfReservedToday); // add the reserved room today

                    $('#btnAssetScheduledCheckout').removeData('data').data('data', NoOfScheduledCheckoutToday);
                    $('#lblScheduledCheckoutBody').text(NoOfScheduledCheckoutToday.length);
                    $('#btnAssetOccupied').removeData('data').data('data', NoOfOccupiedToday);
                    $('#lblOccupiedBody').text(NoOfOccupiedToday.length);
                    $('#btnAssetUnoccupied').removeData('data').data('data', NoOfUnoccupiedUnusedToday);
                    $('#lblUnoccupiedBody').text(NoOfUnoccupiedUnusedToday.length);
                    $('#btnAssetReserved').removeData('data').data('data', NoOfReservedToday);
                    $('#lblReservedBody').text(NoOfReservedToday.length);
                    $('#btnAssetCheckIn').removeData('data').data('data', NoOfCheckedInToday);
                    $('#lblCheckInUsedBody').text(NoOfCheckedInToday.length);
                    $('#btnAssetReservation').removeData('data').data('data', NoOfReservationMadeToday);
                    $('#lblReservationBody').text(NoOfReservationMadeToday.length);
                    $('#btnAssetCancelled').removeData('data').data('data', NoOfCancelledToday);
                    $('#lblCancelledBody').text(NoOfCancelledToday.length);
                }
            })
        }

        function GetGridData(data, stat) {
            AjaxSend({
                url: 'AssetsManagement/GetGridData',
                data: data,
                success: function (e) {
                    DisplayGridDetails(e.Results.Rows, stat);
                }
            })
        }

        function DashboardGridDetails(stat, vRows, type) {
            //if (vRows.length == 0) {
            //    WebToast('No result(s) found', 'top');
            //    return;
            //}
            $('#btnDashboardAsof').text('Today').unbind('click').click(function () {
                PopupPeriod({
                    Click: function (e) {
                        var data = {
                            type: type,
                            DateFrom: e.From,
                            DateTo: e.To
                        }
                        if (e.Period == '2 days ago' || e.Period == 'Yesterday' || e.Period == 'Today') {
                            $('#btnDashboardAsof').text(e.Period);
                        } else {
                            $('#btnDashboardAsof').text(e.From + ' to ' + e.To);
                        }                        
                        GetGridData(data, stat);
                    }
                })
            })

            var data = {
                type: type,
                DateFrom: $.format.date(new Date(), 'MMM dd, yyyy'),
                DateTo: $.format.date(new Date(), 'MMM dd, yyyy')
            }

            GetGridData(data, stat);
        }

        function DisplayGridDetails(vRows, stat) {
            var tblAssetsDashboard = $('#tblAssetsDashboard');

            if ($.fn.DataTable.isDataTable(tblAssetsDashboard)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblAssetsDashboard.empty();
                dtAssetsDashboard.destroy();
            }

            var columns = [
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Asset Name</b>' + data.AssetType;
                    }, width: '200px'
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Asset Name</b>' + data.AssetName;
                    }, width: '200px'
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Client Name</b>' + data.CustomerName;
                    }, width: '200px'
                },
                {
                    "data": function (data) {
                        var from;
                        if (stat == 'checkedin' || stat == 'occupied' || stat == 'scheduledcheckout') {
                            from = FormatDate(new Date(data.CheckedInUsedOn), { Format: 'MMM dd, yyyy hh:mm a' });
                        } else {
                            from = (CStr2(data.ReservedFrom) == '' ? '' : FormatDate(new Date(data.ReservedFrom), { Format: 'MMM dd, yyyy' }));
                        }
                        return '<b class="ui-table-cell-label">Date / Time From</b>' + from;
                    }
                },
                {
                    "data": function (data) {
                        var to;
                        if (stat == 'scheduledcheckout' && $('#btnDashboardAsof').text().toLowerCase() != 'today') {
                            to = FormatDate(new Date(data.ActualCheckedOutReturnedOn), { Format: 'MMM dd, yyyy hh:mm a' });
                        } else if (stat == 'checkedin' || stat == 'occupied' || stat == 'scheduledcheckout') {
                            to = FormatDate(new Date((data.ActualCheckedOutReturnedOn || data.CheckedOutReturnedOn)), { Format: 'MMM dd, yyyy hh:mm a' });
                        } else {
                            to = (CStr2(data.ReservedUntil) == '' ? '' : FormatDate(new Date(data.ReservedUntil), { Format: 'MMM dd, yyyy' }));
                        }
                        return '<b class="ui-table-cell-label">Date / Time Until</b>' + to;
                    }
                }
            ]

            //if (stat == 'scheduledcheckout') {
            //    $('#divAMAsofDate').hide();
            //} else {
            //    $('#divAMAsofDate').show();
                
            //}

            if (stat == 'occupied') {
                columns.push({
                    "data": function (data) {
                        if (data.ActualCheckedOutReturnedOn == undefined) {
                            return '';
                        }
                        return '<b class="ui-table-cell-label">Actual Checked Out</b>' + FormatDate(new Date(data.ActualCheckedOutReturnedOn), { Format: 'MMM dd, yyyy HH:mm a' });
                    }
                })
                $('.thExtra').text('Actual Checked Out').show();
            } else {
                $('.thExtra').hide();
            }

            dtAssetsDashboard = tblAssetsDashboard.DataTable({
                "data": vRows,
                "pageLength": 50,
                "columns": columns,
                "ordering": false,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": '_all' }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                initComplete: function () {
                    tblAssetsDashboard.removeClass('dataTable');
                }
            });

            $('#popupDashboardGridDetails').modal('show');
        }

        function DisplayPreviousCheckedIn(vRows) {
            var tblPreviousCheckedIn = $('#tblPreviousCheckedIn');

            if ($.fn.DataTable.isDataTable(tblPreviousCheckedIn)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblPreviousCheckedIn.empty();
                dtPreviousCheckedIn.destroy();
            }

            var columns = [
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Asset Name</b>' + data.AssetType;
                    }, width: '100px'
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Asset Name</b>' + data.AssetName;
                    }, width: '100px'
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Client Name</b>' + data.CustomerName;
                    }
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Checked-in Date</b>' + FormatDate(new Date(data.CheckedInUsedOn), { Format: 'MMM. dd, yyyy hh:mm a' });
                    }, width: '160px'
                },
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Checked-out Date</b>' + FormatDate(new Date(data.ActualCheckedOutReturnedOn), { Format: 'MMM. dd, yyyy hh:mm a' });
                    }, width: '160px'
                }
                ,
                {
                    "data": function (data) {
                        return '<b class="ui-table-cell-label">Option</b> <div style="text-align:center;"><button data-role="none" data-toggle="tooltip" data-placement="left" title="Download Options" class="btn custom-btn-theme-e btnCircle-xs"><span class="glyphicon glyphicon-paperclip"></span></button></div>'
                    }, width: '50px'
                }
            ]

            dtPreviousCheckedIn = tblPreviousCheckedIn.DataTable({
                "data": vRows,
                paging: false,
                "columns": columns,
                "ordering": false,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": '_all' }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                initComplete: function () {
                    tblPreviousCheckedIn.removeClass('dataTable');
                }
            });

            $('#tblPreviousCheckedIn tr td').unbind('click').click(function () {
                if (dtPreviousCheckedIn.row(this).data() != undefined) {
                    var data = dtPreviousCheckedIn.row(this).data();
                    if (dtPreviousCheckedIn.cell(this).index().columnVisible == 5) {
                        PopupButtonList('Asset Name: <b>' + data.AssetName + '</b><br><br>' +
                            'Customer Name: <b>' + data.CustomerName + '</b>', 'Download Options', [
                            {
                                Text: 'Download Guest Registration Card',
                                Click: function () {
                                    window.location = MergeUrlParameters({
                                        TransID: data.ID,
                                        FileName: 'GuestRegistrationCard',
                                        OrgID: sessionStorage.OrgID
                                    }, '/AssetsManagement/DownloadAssetFile');
                                }
                            },
                            {
                                Text: 'Download Guest Folio',
                                Click: function () {
                                    window.location = MergeUrlParameters({
                                        TransID: data.ID,
                                        FileName: 'GuestFolio',
                                        OrgID: sessionStorage.OrgID
                                    }, '/AssetsManagement/DownloadAssetFile');
                                }
                            }
                            ], { AutoDismiss: true, Close: function() {
                                $('#popupDashboardPreviousCheckedIn').modal('show');
                            }
                            });
                    }
                }
            })

            $('#popupDashboardPreviousCheckedIn').modal('show');
        }

        mOut.GetDashboardData = function () {
            GetDashboardData();
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
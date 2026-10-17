require(['common', 'promise!/Scripts/jquery.filedownload.min.js'], function () {
    $(document).ready(function () {
        var dtDeviceUtilDetails = false;
        var tblVouchersReport;

        $('#selVRStatus').change(function () {
            GetData()
        })

        $('#txtVRSearch').keydown(function (e) {
            if (e.which == 13) {
                tblVouchersReport.search($.trim($(this).val())).draw();
            }
        })

        $('#btnVRDownload').unbind().click(function () {
            $.fileDownload(MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                Status: $('#selVRStatus').val()
            }, '/DashboardsAndReports/DownloadVouchers'), {
                prepareCallback: function () {
                    Bs.Loading('show', 'Preparing your report. Please wait ...');
                },
                successCallback: function () {
                    Bs.Loading('hide');
                },
                failCallback: function (responseHtml, url, error) {
                    var res = JSON.parse(responseHtml).d.error;
                    NewMessageBox(res, 'Error');
                }
            });
            //$('.btnExportVouchersReport').trigger("click");
        })

        function GetData() {

            if ($.fn.DataTable.isDataTable("#tblVouchersReport")) {
                //check if datatable is created 
                $('#tblVouchersReport').DataTable().clear().destroy();
            }
            
            tblVouchersReport = $('#tblVouchersReport').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        Status: $('#selVRStatus').val()
                    }, '/DashboardsAndReports/GetVouchersReport'),
                    type: 'POST',
                    //"dataSrc": function (json) {
                    //    Bs.Loading('hide');
                    //    if (CStr2(json.d.Error) != '') {
                    //        NewMessage('It\'s taking longer than usual. Please try again.', '', [
                    //            {
                    //                Text: 'Retry Now', Click: function () {
                    //                    GetData();
                    //                }
                    //            }, { Text: 'Cancel' }
                    //        ])
                    //    }
                    //}
                },
                "autoWidth": false,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "30px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.TransactionDate) == '' ? '' : $.format.date(new Date(parseInt(data.TransactionDate.substr(6))), 'MM/dd/yyyy hh:mm a');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.ReferenceNo);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.VoucherType);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.VoucherStatus);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Region);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.StoreName);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.StoreCode);
                        }
                    },
                    //{
                    //    "data": function (data, type, full, meta) {
                    //        return CStr2(data.SupercardNumber);
                    //    }
                    //}
                ],
                dom: '<"clear"><"table-fixed-container"<"table-fixed"t>><"bottom"ip><"clear">',
                buttons: [
                    {
                        "extend": 'csvHtml5', text: 'Download CSV', title: 'Vouchers Report as of ' + $.format.date(new Date(), 'MM-dd-yyyy'),
                        "action": function (e, dt, button, config) {
                            var self = this;
                            var oldStart = dt.settings()[0]._iDisplayStart;
                            dt.one('preXhr', function (e, s, data) {
                                // Just this once, load all data from the server...
                                data.start = 0;
                                data.length = 100000;
                                dt.one('preDraw', function (e, settings) {
                                    // Call the original action function
                                    if (button[0].className.indexOf('buttons-copy') >= 0) {
                                        $.fn.dataTable.ext.buttons.copyHtml5.action.call(self, e, dt, button, config);
                                    } else if (button[0].className.indexOf('buttons-excel') >= 0) {
                                        $.fn.dataTable.ext.buttons.excelHtml5.available(dt, config) ?
                                            $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config) :
                                            $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
                                    } else if (button[0].className.indexOf('buttons-csv') >= 0) {
                                        $.fn.dataTable.ext.buttons.csvHtml5.available(dt, config) ?
                                            $.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config) :
                                            $.fn.dataTable.ext.buttons.csvFlash.action.call(self, e, dt, button, config);
                                    } else if (button[0].className.indexOf('buttons-pdf') >= 0) {
                                        $.fn.dataTable.ext.buttons.pdfHtml5.available(dt, config) ?
                                            $.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config) :
                                            $.fn.dataTable.ext.buttons.pdfFlash.action.call(self, e, dt, button, config);
                                    } else if (button[0].className.indexOf('buttons-print') >= 0) {
                                        $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
                                    }
                                    dt.one('preXhr', function (e, s, data) {
                                        // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                                        // Set the property to what it was before exporting.
                                        settings._iDisplayStart = oldStart;
                                        data.start = oldStart;
                                    });
                                    // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
                                    setTimeout(dt.ajax.reload, 0);
                                    // Prevent rendering of the full data to the DOM
                                    return false;
                                });
                            });
                            // Requery the server with the new one-time export settings
                            dt.ajax.reload();
                        }, className: 'btnExportVouchersReport d-none'
                    }
                ],
                initComplete: function () {
                    Bs.Loading('hide');
                    $('#tblVouchersReport').removeClass('dataTable');
                    dtDeviceUtilDetails = true;
                },
                fnPreDrawCallback: function (oSettings) {
                    Bs.Loading('show');
                },
                drawCallback: function (oSettings) {
                    Bs.Loading('hide');
                },
            });
        }

        GetData();
    })
})
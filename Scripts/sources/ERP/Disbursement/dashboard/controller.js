require(['common'], function () {
    $(document).ready(function () {

        LoadRequiredData();

        function GetNumberContactPersonForApproval() {
            AjaxSend({
                url: 'OrganizationShort/GetNumberContactPersonForApproval',
                success: function (e) {
                    if (e.Error != undefined) {
                        NewMessageBox(e.Error, 'Message');
                    } else {
                        $('#lblVendorAdminBody').empty()
                            .append($('<span style="font-size:12px">').text('to Vendor Portal'))
                            .append('<br>')
                            .append(e.Supplier)
                    }
                }
            })
        }

        $('body').on('click', '#btnReportVendorAdmin', function () {
            $('#popupVendorAdminApproval').modal('show');
            $('#tabVendorPortal').opentabmenu();
            GetVendorContactPersonRequestList();
        })

        $('body').on('click', '[name=txtSearchVendorAdmin]', function (e) {
            if (e.which == 13) {
                GetVendorContactPersonRequestList($(this).val())
            }
        })

        var tableVendorContactPersonAdmin;
        function GetVendorContactPersonRequestList(keywords) {
            var tbodyVendorContactPersonAdmin = $('#tbodyVendorContactPersonAdmin');

            tbodyVendorContactPersonAdmin.empty().append($('<tr>').append($('<td colspan="5" style="text-align:center">').text('Getting data ... please wait')))
            AjaxSend({
                url: 'OrganizationShort/GetVendorContactPersonRequestList',
                data: { keywords: $.trim(CStr2(keywords)) },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var ctr = 0;

                    tbodyVendorContactPersonAdmin.empty()

                    if ($.fn.DataTable.isDataTable("#tableVendorContactPersonAdmin")) {
                        //check if datatable is created 
                        $('#tableVendorContactPersonAdmin').DataTable().clear().destroy();
                    }

                    tableVendorContactPersonAdmin = $('#tableVendorContactPersonAdmin').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 20,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.OrgName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.AdminName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return $.format.date(new Date(data.StatusLastUpdated), 'MM-dd-yyyy hh:mm a');
                                }
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none" class="btn btn-success" title="Approve">')
                                        .append('<span class="bi bi-hand-thumbs-up-fill"></span>')
                                        .click(function () {
                                            rowData.Site = 'Vendor Portal';
                                            SubmitContactPersonRequest(rowData, 'Approved');
                                        });
                                    $(td).css('width', '50px').html(b);
                                }
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none" class="btn btn-danger" title="Disapprove">')
                                        .append('<span class="bi bi-hand-thumbs-down-fill"></span>')
                                        .click(function () {
                                            rowData.Site = 'Vendor Portal';
                                            SubmitContactPersonRequest(rowData, 'Disapproved');
                                        });
                                    $(td).css('width', '50px').html(b);
                                }
                            }
                        ],
                        "initComplete": function (row, data) {
                            $('#tableVendorContactPersonAdmin').removeClass('dataTable');
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "columnDefs": [{ "orderable": false }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })
                }
            })
        }

        function SubmitContactPersonRequest(data, status) {
            NewPromptUserAccess('Allow approval of contact person\'s request to access Vendor Portal?', {
                OnClickOKSuccess: function () {
                    NewMessageBox('Are you sure want to <b>' + status.replace('d', '').toLowerCase() + '</b> request of <b>' + data.AdminName + '</b> for access to ' + data.Site + '?', 'Message', [
                        {
                            Text: 'Yes', Click: function (e) {
                                AjaxSend({
                                    url: 'OrganizationShort/ApproveContactPersonRequest',
                                    data: { ID: data.ID, Status: status },
                                    success: function (e) {
                                        if (e.Results == 'saved') {
                                            WebToast('Request of ' + data.AdminName + ' has been ' + status.toLowerCase(), 'top');
                                            GetVendorContactPersonRequestList();
                                            GetNumberContactPersonForApproval();
                                        } else {
                                            NewMessageBox(e.Results, 'Error');
                                        }
                                    }
                                })
                            }
                        }, { Text: 'No' }
                    ])
                },
                Message: 'You\'re not allowed to approve contact person\'s request to access ' + data.Site
            });
        }

        function LoadRequiredData() {
            GetNumberContactPersonForApproval();
        }

        var mOpenPurchaseOrderTable = $('#tableOpenPurchaseOrders');
        var mOpenPurchaseOrderTableBody = mOpenPurchaseOrderTable.find('tbody');;
        var mOpenPurchaseOrderDataTable, mIsCountPQFollowUp = false;

        $('body').on('click', '[name=butDisbursementOpenPurchaseOrders]', function () {
            $('[name="chbPQFollowUp"]').prop('checked', 0);
            GetOpenPurchaseOrders();
            if (proj.IsBootstrapDesign()) {
                $('#popDisbursementOpenPurchaseOrders').modal('show');
            } else {
                $('#popDisbursementOpenPurchaseOrders').show();
            }            
        });

        $('body').on('click', '[name="butDisbursementPQFollowUp"]', function () {
            $('[name="chbPQFollowUp"]').prop('checked', 1);
            GetOpenPurchaseOrders();
            if (proj.IsBootstrapDesign()) {
                $('#popDisbursementOpenPurchaseOrders').modal('show');
            } else {
                $('#popDisbursementOpenPurchaseOrders').show();
            }
        });

        var mButPoDate = $('#popDisbursementOpenPurchaseOrders [name=butPoDate]');
        var mPoDateFrom = new Date();
        mPoDateFrom = new Date(mPoDateFrom.getFullYear(), mPoDateFrom.getMonth() - 2, mPoDateFrom.getDate());
        var mPoDateTo = new Date();
        mButPoDate.text('PO Date: ' + FormatDate(mPoDateFrom, {Format: 'MMM dd, yyyy'}) + ' to ' + FormatDate(mPoDateTo, {Format: 'MMM dd, yyyy'}));
        mButPoDate.click(function() {
            DualCalendar({
                Click: function (e) {
                    mPoDateFrom = e.From;
                    mPoDateTo = e.To;
                    mButPoDate.text('PO Date: ' + FormatDate(mPoDateFrom, {Format: 'MMM dd, yyyy'}) + ' to ' + FormatDate(mPoDateTo, {Format: 'MMM dd, yyyy'}));
                    GetOpenPurchaseOrders();
                }
            }, new Date(), new Date());    
        });

        function GetOpenPurchaseOrders() {
            if (!proj.IsBootstrapDesign())
                $('[name="chbPQFollowUp"]').checkboxradio('refresh');
                
            var pData = {
                "TransType": "PurchaseOrder",
                "AnyZero": 1,
                "FinalizedPoOnly": 1,
                "NoReceiving": 1,
                "IsCancelled": "0",
                ShowCustomerContactInfo: 1,
                LastUpdatedDateFrom: mPoDateFrom,
                LastUpdatedDateTo: mPoDateTo,
                MaxRecords: 5000
            };
            if (CBool($('#popDisbursementOpenPurchaseOrders [name="rbReceivedByCompany"]:checked').val()))
                pData.HasReceiving = 1;
            else
                pData.NoReceiving = 1;
            if (CBool($('#popDisbursementOpenPurchaseOrders [name="rbServedByVendor"]:checked').val()))
                pData.FinalizedFcOnly = 1;
            else
                pData.ExcludeFinalizedFc = 1;
            if (CBool($('#popDisbursementOpenPurchaseOrders [name="chbPQFollowUp"]').is(':checked')))
                pData.IsForFollowUp = 1;
            else
                pData.IsForFollowUp = 0;
            return
            AjaxSend({
                url: 'Browser/TransInvoices',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else {
                        if (mIsCountPQFollowUp) {
                            mIsCountPQFollowUp = false;
                            $('[name=lblDisbursementPQFollowUp]').text(vData.Results.Rows.length);
                            $('[name="chbPQFollowUp"]').prop('checked', 0);
                        } else {
                            $('[name=lblDisbursementOpenPurchaseOrders]').text(vData.Results.Rows.length);
                            DisplayOpenPurchaseOrders(vData);
                        }
                    }
                }
            });

        }
        proj.GetOpenPurchaseOrders = GetOpenPurchaseOrders;
        $('#popDisbursementOpenPurchaseOrders').detach().appendTo($('body'));        

        $('body').on('change', '#popDisbursementOpenPurchaseOrders [name="chbPQFollowUp"]', function () {
            GetOpenPurchaseOrders();
        });

        $('body').on('change', '#popDisbursementOpenPurchaseOrders [name="rbServedByVendor"]', function () {
            GetOpenPurchaseOrders();
        });

        $('body').on('change', '#popDisbursementOpenPurchaseOrders [name="rbReceivedByCompany"]', function () {
            GetOpenPurchaseOrders();
        });

        $('body').on('click', '[name=butDisbursementVendorPriceHistory]', function() {
            require(['/Scripts/sources/ERP/VendorPriceHistory.js?crc=' + assetversion], function () {
                ERP.VendorPriceHistory.show();
            })
        });

        $('body').on('click', '[name=butDisbursementVendorPaymentReport]', function () {
            //$('#tabRevenueWork').opentabmenu();
            //$('#btnRevCreditAndCollection').click();
            require(['promise!/Scripts/sources/ERP/CollectionReport/controller.js?v=' + assetversion], function () {
                ERP.CollectionReport.Popup({
                    Mode: 'AP'
                });
            });
        })

        $('#butDisbursementVendorPaymentsHistory').click(function () {
            Bs.Loading('show');
            require(['/Scripts/sources/ERP/Payment/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.Payment.controller.Mode = 'AP';
                ERP.Payment.controller.CustomerID = null;
                ERP.Payment.controller.VendorPaymentsHistory();
            })
        });

        function DisplayOpenPurchaseOrders(vParams) {

            var pData = {
                columns: [
                    {
                        title: 'PO Date',
                        data: function(vData) {
                            return CStr2(FormatDate(vData.PurchaseOrderDate, {
                                    
                            }));
                        }
                    },
                    {
                        title: 'PO Number',
                        data: function(vData) {
                            var pOut = '';
                            if (CStr2(vData.MIN).trim() != '')
                                pOut = vData.MIN + '-';
                            if (!IsNumeric(vData.PurchaseOrderNo))
                                vData.PurchaseOrderNo = 0;
                            vData.PurchaseOrderFullNumber = 'PO-' + pOut + vData.PurchaseOrderNo;
                            return vData.PurchaseOrderFullNumber;
                        }
                    },
                    {
                        title: 'Vendor Name',
                        data: function(vData) {
                            return vData.CompanyName;
                        }
                    },
                    {
                        title: 'Time Elapsed Since PO',
                        data: function(vData, vRenderType) {
                            var pPoDate = ParseDate(vData.LastUpdated);
                            if (vRenderType == 'sort') {
                                if (pPoDate == null)
                                    return 0;
                                else
                                    return pPoDate.getTime();
                            }

                            if (pPoDate == null)
                                return '-';
                            var pDiff = new Date().getTime() - pPoDate.getTime();
                            pDiff = pDiff / 1000;
                            pDiff = Math.floor(pDiff);
                            pDiff = FormatSeconds(pDiff);
                            return pDiff;
                        }
                    },
                    {
                        title: 'Latest Remarks',
                        data: function(vData) {
                            return vData.Remarks;
                        }
                    },
                    {
                        title: 'Vendor Contact Details',
                        data: function (vData) {
                            var str = '';

                            if (CStr2(vData.CustomerTelephoneNumbers) != '')
                                str += 'T:' + vData.CustomerTelephoneNumbers + '<br>';

                            if (CStr2(vData.CustomerMobileNumbers) != '')
                                str += 'M: ' + vData.CustomerMobileNumbers + '<br>';                            

                            if (CStr2(vData.CustomerEmails) != '')
                                str += 'E: ' + vData.CustomerEmails;

                            return str;
                        }
                    },
                    {
                        title: 'Served Date and Time by Vendor',
                        data: function(vData) {
                            return CStr2(FormatDate(vData.FinalizePickListTime, {
                                    
                            }));
                        }
                    },
                    {
                        title: 'Served By Vendor',
                        data: function(vData) {
                            return (CStr2(vData.FinalizePickListByID) != '') ? 'Yes' : 'No';
                        }
                    },
                    {
                        title: 'Received by Company',
                        data: function(vData) {
                            var pValue = CDec2(vData.PoReceivedPercent);
                            if (pValue == 100)
                                return 'Complete';
                            else if (pValue == 0)
                                return 'Non Delivered';
                            else
                                return 'Partial';
                        }
                    }
                ]
            };
            pData.data = vParams.Results.Rows;

            SysAdminDataTable(pData);

            function SysAdminDataTable(vData) {
                mOpenPurchaseOrderTableBody.empty()
            
                if ($.fn.DataTable.isDataTable(mOpenPurchaseOrderTable)) {
                    //check if datatable is created 
                    mOpenPurchaseOrderTable.DataTable().clear().destroy();
                }

                mOpenPurchaseOrderDataTable = mOpenPurchaseOrderTable.DataTable({
                    //retrieve: true,
                    data: vData.data,
                    "bdestroy": true,
                    "pageLength": 50,
                    "order": [[3, "asc"]],
                    "bProcessing": true,
                    //"bServerSide": true,
                    //"ajax": vData.ajax,
                    "autoWidth": false,
                    "columns": vData.columns,
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": vData.disablecols }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function () {
                        if (proj.IsBootstrapDesign()) {
                            mOpenPurchaseOrderTable.removeClass('dataTable');
                            mOpenPurchaseOrderTable.find('thead tr').css('font-size', '12px');
                        }                        
                    }
                    //"stateSave": true
                })

                mOpenPurchaseOrderTableBody.unbind('click').on('click', 'tr', function () {
                    var pData = mOpenPurchaseOrderDataTable.row(this).data();
                    var win = window.open(location.origin + '/PurchaseOrder/Preview' + location.search + '&IsReadOnly=1&ID=' + pData.ID + '&IsFollowUp=' + CInt2($('[name="chbPQFollowUp"]').is(':checked')),
                        '_blank',
                        'width=' + screen.availWidth + ',height=' + screen.availHeight);

                    var timer = setInterval(function () {
                        if (win.closed) {
                            clearInterval(timer);
                            GetOpenPurchaseOrders();
                        }
                    }, 500);
                    //var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                    //    '_blank',
                    //    'width=' + window.innerWidth + ',height=' + window.innerHeight);

                    //win.onload = function () {
                    //    function LoadRecord() {
                    //        if (win.Z9Q3C9ASDA)
                    //            win.Z9Q3C9ASDA.ShowPurchaseOrder({
                    //                ID: pData.ID,
                    //                ReadOnly: true
                    //            });
                    //        else
                    //            setTimeout(LoadRecord, 1000);
                    //    }
                    //    LoadRecord();    
                    //}
                    //if (CStr2(pData) != '') {
                    //    NewMessageBox('', pData.PurchaseOrderFullNumber, [
                    //        {
                    //            Text: 'View PO',
                    //            Click: function() {
                    //                var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                    //                    '_blank',
                    //                    'width=' + window.innerWidth + ',height=' + window.innerHeight);

                    //                win.onload = function () {
                    //                    function LoadRecord() {
                    //                        if (win.Z9Q3C9ASDA)
                    //                            win.Z9Q3C9ASDA.ShowPurchaseOrder({
                    //                                ID: pData.ID,
                    //                                ReadOnly: true
                    //                            });
                    //                        else
                    //                            setTimeout(LoadRecord, 1000);
                    //                    }
                    //                    LoadRecord();    
                    //                }
                    //            }
                    //        }
                    //    ]);
                    //}
                });
            }

        }

        GetOpenPurchaseOrders();
        proj.GetUnapprovedPurchaseOrderCount = GetUnapprovedPurchaseOrderCount;
        function GetUnapprovedPurchaseOrderCount() {
                
            var pData = {
            };

            AjaxSend({
                url: 'Erp/GetUnapprovedPurchaseOrderCount',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else {
                        $('[name=lblDisbursementUnapprovedPurchaseOrders]').text(AccountingFormat(vData.Result, 0));
                        
                    }

                    mIsCountPQFollowUp = true;
                    $('[name="chbPQFollowUp"]').prop('checked', 1);
                    GetOpenPurchaseOrders();
                }
            });

        }
        GetUnapprovedPurchaseOrderCount();

        $('body').on('click', '[name=butDisbursementUnapprovedPurchaseOrders]', function () {
            require([
                'promise!/Scripts/sources/ERP/UnapprovedPurchaseOrders/controller.js?v=' + assetversion
            ], function() {
                ERP.UnapprovedPurchaseOrders.controller.show();
            });
        })
    })
})
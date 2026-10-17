define([
    '/Content/Ecommerce/js/lightbox-2.11.1.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml, isLoad = false;
    var mOut = {};

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ScanVendorInvoices/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ScanVendorInvoices.main.controller = ERP.ScanVendorInvoices.main.controller()
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.ScanVendorInvoices.main.controller', null, function () {

        var mElement = $(mHtml);
        mOut.element = mElement;

        $('body').append(mHtml);

        $('[name="txtSearchScanVendorInvoices"]').keydown(function (e) {
            if (e.which == 13) {
                GetUnapprovedExpenseTicket();
            }
        })

        $('[name="btnAddVendorInvoice"]').click(function () {
            require(['promise!/Scripts/sources/ERP/Invoicing/InvoiceImage/controller.js?crc=' + assetversion], function () {
                ERP.InvoiceImage.main.controller.add({
                    success: function () {
                        GetUnapprovedExpenseTicket();
                    }
                });
            })
        })

        mOut.popup = function (vParams) {
            GetUnapprovedExpenseTicket();
            $('#popupScanVendorInvoices').modal('show');
        }

        function GetUnapprovedExpenseTicket() {
            var tbodyScanVendorInvoices = $('#tbodyScanVendorInvoices');
            var divScanVendorInvoicesViewer = $('[name="divScanVendorInvoicesViewer"]');

            tbodyScanVendorInvoices.empty();
            divScanVendorInvoicesViewer.empty();

            if ($.fn.DataTable.isDataTable("#tblScanVendorInvoices")) {
                //check if datatable is created 
                $('#tblScanVendorInvoices').DataTable().clear().destroy();
            }

            var tblScanVendorInvoices = $('#tblScanVendorInvoices').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        keywords: $.trim($('[name="txtSearchScanVendorInvoices"]').val())
                    }, '/Vendor/GetAllScanVendorInvoices'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        title: '#',
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "50px"
                    },
                    {
                        title: 'Vendor Name',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.VendorName);
                        }
                    },
                    {
                        title: 'Invoice Ref. No',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.InvoiceRefNo);
                        }
                    },
                    {
                        title: 'Invoice Date',
                        "data": function (data, type, full, meta) {
                            return $.format.date(CJsonDate(data.InvoiceDate), 'MM-dd-yyyy');
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:center');
                        }
                    },
                    {
                        title: 'No. of Line Items',
                        "data": function (data, type, full, meta) {
                            return CDec2(data.NoOfLineItems);
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:center');
                        }
                    },
                    {
                        title: 'Invoice Amount',
                        "data": function (data, type, full, meta) {
                            return AccountingFormat(data.InvoiceAmount);
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:right');
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
                            var button = $('<button data-role="none" class="btn btn-secondary">')
                                            .append('View')
                                            .data('data', rowData)
                                            .click(function () {
                                                var data = $(this).data('data');
                                                $('#' + data.ID).click();
                                            })

                            $(td).css('text-align', 'right').html(button);
                        },
                        "width": "50px"
                    }
                ],
                "createdRow": function (row, data, index) {
                    var src = location.origin + '/FileTemplates/' + sessionStorage.OrgID + '/CustomerSupplierFiles/' + data.ClientID + '/' + data.FileName;
                    divScanVendorInvoicesViewer.append($('<div>')
                                    .append($('<a data-role="none" data-lightbox="scaninvref" data-title="'+ data.InvoiceRefNo +'">')
                                        .attr('id', data.ID)
                                        .attr('href', src)
                                        //.append($('<img>')
                                        //    .data('data', data).attr('src', src)
                                        //)
                                    )
                                )
                },
                "initComplete": function () {
                    $('#tblScanVendorInvoices').removeClass('dataTable');
                    $('#tblScanVendorInvoices thead tr').addClass('align-top text-center').css('font-size', '14px')

                    lightbox.option({
                        'resizeDuration': 200,
                        'wrapAround': true,
                        'imageFadeDuration': 100,
                        'alwaysShowNavOnTouchDevices': true
                    })
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
require(['common'], function () {
    $(document).ready(function () {
        var invid, vendorid = null;

        $('#formSearch').submit(function (e) {
            e.preventDefault();
            
            Bs.Loading('show', 'Searching... please wait');
            $('#txtSearch').blur();

            AjaxSend({
                url: '/Browser/TransInvoices',
                data: {
                    keywords: $.trim($('#txtSearch').val()),
                    TransType: 'Invoice',
                    AnyZero: 1,
                    AllowNoItems: true
                }, success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Results.Rows;
                    var divResults = $('#divResults');

                    divResults.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var vDR = vRows[i], pOut = '';
                            if (CStr2(vDR.FowstWideInvoiceNumber).trim() != '')
                                pOut = vDR.FOWSTNo + '-' + vDR.FowstWideInvoiceNumber;
                            else if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-' + vDR.InvoiceNumber;
                            else
                                pOut = vDR.InvoiceNumber;
                            if (CStr2(vDR.DocumentName).trim() == '')
                                pOut = 'SI-' + pOut;
                            else
                                pOut = vDR.DocumentName + '-' + pOut;

                            vDR.CompleteInvoice = pOut;
                            var btn = '<button data-role="none" class="list-group-item list-group-item-action">' +
                                        '<div class="row align-items-center">' +
                                            '<div class="col"> ' +
                                                '<b>' + pOut + '</b> <br> ' + $.format.date(new Date(vDR.InvoiceDate), 'MM/dd/yy') + '<br>' +
                                                $.trim(vDR.CompanyName) +
                                            '</div>' +
                                            '<div class="col-1"> ' +
                                                '<span class="bi-chevron-right"></span>' +
                                            '</div>' +
                                        '</button>'

                            divResults.append($('<div class="mb-3">')
                                .append(btn)
                                .data('data', vDR)
                                .click(function () {
                                    var data = $(this).data('data');
                                    invid = data.ID;
                                    $('#lblInvoice').text(data.CompleteInvoice);
                                    $('#pageUpload').pageshow();
                                })
                            )
                        }
                    } else {
                        divResults.append($('<div class="text-center text-secondary">')
                                .append('<span class="bi-search" style="font-size:50px;"></span>')
                                .append('<br>')
                                .append('No results found')
                            )
                    }
                }
            })
        })

        $('#btnSearchAgain').click(function () {
            $('#txtSearch').val('');
            $('#divResults').empty();
            $('#pageSearch').pageshow();
        })

        $('#btnUpload').click(function () {
            $('#fileAttach').click();
        })

        $('#fileAttach').change(function () {
            var formdata = new FormData();
            var url = MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                InvoiceID: invid
            }, '/CreditAndCollection/UploadInvoiceAttachments')

            for (var i = 0; i < $('#fileAttach')[0].files.length; i++) {
                formdata.append($('#fileAttach')[0].files[i].name, $('#fileAttach')[0].files[i]);
            }

            Bs.Loading('show', 'Please wait while uploading file...');

            AjaxSend({
                url: url,
                type: 'POST',
                data: formdata,
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    Bs.Loading('hide');
                    if (vData.Results == 'saved') {
                        NewMessageBox('File was successfully uploaded', 'Message');
                        $('#btnSearchAgain').click();
                    } else {
                        NewMessageBox('Thers\'s an error uploading attachment(s).', 'Message');
                    }
                }
            })
        });

        $('#btnCustomerSI').click(function () {
            $('#pageCustomerSI, #pageSearch').pageshow();
        })

        $('.btnBackHome').click(function () {
            $('#pageHome').pageshow();
        })

        $('#btnVendorSI').click(function () {
            ClearVendorFields();
            $('#pageVendorSI').pageshow();
        })

        $('#btnVendor').click(function () {
            BrowsePopup('LibraryOrganizationSupplier', 'Search Vendor', [
                {
                    DisplayText: [{ Column: 'SupplierName' }],
                    Click: function (e) {
                        vendorid = e.ID;
                        $('#btnVendor').text(e.SupplierName);
                    }
                }
            ])
        })

        $('#btnVendorUpload').click(function () {
            if (!ValidateVendorSI()) {
                return;
            }

            $('#VendorfileAttach').click();
        })

        $('#VendorfileAttach').change(function () {
            if (!ValidateVendorSI()) {
                return;
            }

            var formdata = new FormData();
            var url = MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                id: vendorid,
                InvRefNo: $('#txtVendorInvoiceRefNo').val(),
                InvRefDate: $('#txtVendorInvoiceRefDate').val(),
                InvNoOfLineItems: $('#txtVendorNoOfLineItems').val(),
                InvRefAmount: $('#txtVendorInvoiceRefAmount').val(),
                Remarks: $('#txtVendorInvoiceRefRemarks').val(),
            }, '/OrganizationShort/UploadFiles')
            var VendorfileAttach = $('#VendorfileAttach')[0].files;

            if (VendorfileAttach.length == 0) {
                NewMessageBox('Please select Invoice Image.');
                return false;
            }

            for (var i = 0; i < VendorfileAttach.length; i++) {
                formdata.append(VendorfileAttach[i].name, VendorfileAttach[i]);
            }

            Bs.Loading('show', 'Please wait while uploading file...');

            AjaxSend({
                url: url,
                type: 'POST',
                data: formdata,
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    Bs.Loading('hide');
                    if (vData.Results == 'uploaded') {
                        NewMessageBox('File was successfully uploaded', 'Message');
                        $('#btnVendorSI').click();
                    } else {
                        NewMessageBox('Thers\'s an error uploading attachment(s).', 'Message');
                    }
                }
            })
        });

        function ValidateVendorSI() {
            if (vendorid == null) {
                NewMessageBox('Please search Vendor');
                return false;
            }

            if ($.trim($('#txtVendorInvoiceRefNo').val()) == '') {
                NewMessageBox('Please enter Invoice Reference No.');
                return false;
            }

            if ($.trim($('#txtVendorInvoiceRefDate').val()) == '') {
                NewMessageBox('Please enter Invoice Date');
                return false;
            }

            if ($.trim($('#txtVendorNoOfLineItems').val()) == '') {
                NewMessageBox('Please enter Invoice Line Items');
                return false;
            }

            if ($.trim($('#txtVendorInvoiceRefAmount').val()) == '') {
                NewMessageBox('Please enter Invoice Amount');
                return false;
            }

            return true;
        }

        function ClearVendorFields() {
            vendorid = null;
            $('#btnVendor').text('Search Vendor');
            $('#pageVendorSI').find('input, textarea').val('');
            $('#txtVendorInvoiceRefDate').attr('max', $.format.date(new Date(), 'yyyy-MM-dd'));
            $(window).scrollTop(0);
        }
    })
})
define(['/Content/Ecommerce/js/lightbox-2.11.1.js']
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Invoicing/InvoiceImage/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.InvoiceImage.main.controller = ERP.InvoiceImage.main.controller()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.InvoiceImage.main.controller', null, function () {

            var mOut = {}, data;
            var mElement, isAddOnly = false;

            mElement = $(mHtml);
            mOut.element = mElement;            

            $('body').append(mElement);

            mOut.popup = function (vParams) {
                data = vParams;

                if (CStr2(data.ID) == '') {
                    NewMessageBox('Please select draft or finalized transaction');
                    return
                }

                //if (CStr2(data.SupplierID) == '') {
                //    NewMessageBox('Please select Vendor name first');
                //    return
                //}

                DisplayReceivingInvoiceRefImages();
                isAddOnly = false;                
            }

            mOut.add = function (vParams) {
                data = vParams;
                isAddOnly = true;
                mElement.find('#btnIIUploadInvRef').click()
            }

            mElement.find('#btnIIBrowseVendorInv').click(function () {
                BrowsePopup('/OrganizationShort/GetFiles', 'Select Invoice', [
                    {
                        SelectMultiple: true,
                        Message: 'You can select multiple files. Click OK when you\'re done.',
                        DisplayText: [{ Column: 'FileName' }],
                        AutoSearch: true,
                        Click: function (e) {
                            Bs.Loading('show');

                            AttachReceivingInvoiceRefImages(e);
                        }
                    }
                ], { ID: CStr2(data.SupplierID), InvoiceOnly: true })
            })

            mElement.find('#btnIIUploadInvRef').click(function () {
                $('#btnIIVendor').text(data.SupplierName || 'Search Vendor');
                $('#popupUploadInvoiceRefImage').find('input, textarea').val('');
                $('#txtIIVendorInvoiceRefDate').attr('max', $.format.date(new Date(), 'yyyy-MM-dd'));
                $('#popupUploadInvoiceRefImage').modal('show');
            })

            mElement.find('#btnIIVendor').click(function () {
                BrowsePopup('LibraryOrganization', 'Search Vendor', [
                    {
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            data.SupplierID = e.ID;
                            data.SupplierName = e.CompanyName;
                            mElement.find('#btnIIVendor').text(data.SupplierName);
                        }
                    }
                ], { FilterSupplier: 1 })
            })

            mElement.find('#btnIIUpload').click(function () {
                if ($.trim($('#txtIIVendorInvoiceRefNo').val()) == '') {
                    WebToast('Please enter Invoice Reference No.', 'top');
                    return false;
                }

                if ($.trim($('#txtIIVendorInvoiceRefDate').val()) == '') {
                    WebToast('Please enter Invoice Date', 'top');
                    return false;
                }

                if ($.trim($('#txtIIVendorNoOfLineItems').val()) == '') {
                    WebToast('Please enter Invoice Line Items', 'top');
                    return false;
                }

                if ($.trim($('#txtIIVendorInvoiceRefAmount').val()) == '') {
                    WebToast('Please enter Invoice Amount', 'top');
                    return false;
                }

                var formdata = new FormData();
                var url = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    id: data.SupplierID,
                    InvRefNo: $('#txtIIVendorInvoiceRefNo').val(),
                    InvRefDate: $('#txtIIVendorInvoiceRefDate').val(),
                    InvNoOfLineItems: $('#txtIIVendorNoOfLineItems').val(),
                    InvRefAmount: $('#txtIIVendorInvoiceRefAmount').val(),
                    Remarks: $('#txtIIVendorInvoiceRefRemarks').val(),
                }, '/OrganizationShort/UploadFiles')
                var txtIIVendorfileAttach = $('#txtIIVendorfileAttach')[0].files;

                if (txtIIVendorfileAttach.length == 0) {
                    NewMessageBox('Please select Invoice Image.');
                    return false;
                }

                for (var i = 0; i < txtIIVendorfileAttach.length; i++) {
                    formdata.append(txtIIVendorfileAttach[i].name, txtIIVendorfileAttach[i]);
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
                        if (vData.Results == 'uploaded') {
                            if (typeof data.success != 'undefined') {
                                data.success();
                            }

                            if (!isAddOnly) {
                                AttachReceivingInvoiceRefImages([{ ID: vData.ID }]);
                            } else {
                                Bs.Loading('hide');
                            }

                            $('#popupUploadInvoiceRefImage').modal('hide');
                        } else {
                            NewMessageBox('Thers\'s an error uploading attachment(s).', 'Message');
                        }
                    }
                })
            })

            function DisplayReceivingInvoiceRefImages() {
                var tbodyInvoiceImage = $('#tbodyInvoiceImage');
                var divInvoiceRefImage = $('[name="divInvoiceRefImage"]');

                tbodyInvoiceImage.empty();
                divInvoiceRefImage.empty();

                mOut.GetInvoiceRefImages(function (e) {
                    if (e.Results == '') {
                        NewMessageBox('This transaction is not yet invoiced');
                        return;
                    }

                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var vData = vRows[i];
                            var src = location.origin + '/FileTemplates/' + sessionStorage.OrgID + '/CustomerSupplierFiles/' + data.SupplierID + '/' + vData.FileName;
                            vData.src = src;
                            tbodyInvoiceImage.append($('<tr>')
                                .append($('<td>').text((i + 1) + '.'))
                                .append($('<td>').text(vData.InvoiceRefNo))
                                .append($('<td class="text-center">').text($.format.date(new Date(vData.InvoiceDate), 'MM-dd-yyyy')))
                                .append($('<td class="text-center">').text(vData.NoOfLineItems))
                                .append($('<td class="text-right">').text(AccountingFormat(vData.InvoiceAmount)))
                                .append($('<td>').text(CStr2(vData.Remarks)))
                                .append($('<td>')
                                    .append($('<button data-role="none" class="btn btn-secondary">').text('View')
                                        .data('data', vData)
                                        .click(function () {
                                            var data = $(this).data('data');
                                            PopupButtonList('', 'Open image', [
                                                {
                                                    Text: 'In pop-up window', Click: function () {
                                                        $('#' + data.ID).click();
                                                    }
                                                }, {
                                                    Text: 'In new browser window', Click: function () {
                                                        window.open(data.src, '_blank', 'width=' + screen.width + ',height=' + screen.height);
                                                    }
                                                }
                                            ])                                            
                                        })
                                    )
                                )
                            )

                            divInvoiceRefImage.append($('<div>')
                                .append($('<a data-role="none" data-lightbox="invrefimg" data-title="' + vData.InvoiceRefNo + '">')
                                    .attr('id', vData.ID)
                                    .attr('href', src)
                                    //.append($('<img>')
                                    //    .data('data', vData).attr('src', src)
                                    //)
                                )
                            )
                        }

                        lightbox.option({
                            'resizeDuration': 200,
                            'wrapAround': true,
                            'imageFadeDuration': 100,
                            'alwaysShowNavOnTouchDevices': true
                        })
                    } else {
                        tbodyInvoiceImage.append($('<tr>')
                            .append($('<td class="text-center" colspan="8">').text('No results found'))
                        )
                    }

                    $('#popupInvoiceRefImage').modal('show');
                }, data);
            }

            mOut.GetInvoiceRefImages = function (callback, vData) {
                Bs.Loading('show');
                AjaxSend({
                    url: '/Receiving/GetReceivingInvoiceRefImages',
                    data: { ReceivingID: vData.ID, TransType: vData.mod, ExpenseNumber: CStr2(vData.ExpenseNumber) },
                    success: function (e) {
                        Bs.Loading('hide');
                        if (vData.mod == 'Expense') {
                            vData.ID = e.ReceivingID;
                        }

                        callback(e);
                    }
                })
            }

            function AttachReceivingInvoiceRefImages(e) {
                AjaxSend({
                    url: '/Receiving/AttachReceivingInvoiceRefImages',
                    data: { ReceivingID: CStr2(data.ID), TransType: data.mod, SupplierID: CStr2(data.SupplierID), Files: e },
                    success: function (e) {
                        Bs.Loading('hide');

                        if (e.Results == 'saved') {
                            DisplayReceivingInvoiceRefImages();
                            WebToast('Successfully added', 'top');
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            }

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })
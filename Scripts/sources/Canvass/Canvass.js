require(['common'], function () {
    require(['promise!/Scripts/jquery.filedownload.min.js'])
    $(document).ready(function () {
        var tbodyCanvassLink, isValidTradeDisc = false, itemData, arrVendor, indVendor = null, OrgID, canvassData, vendorData;
        var IsCreateNewSKU = false, isFirstUploadCanvass = true, isFromAdjustPrices = false;

        $(function () {
            AjaxSend({
                url: 'Canvass/GetLinkData',
                data: { code: GetUrlParameters().c },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        OrgID = vRows[0].OrgID;
                        sessionStorage.OrgID = OrgID;

                        AjaxSend({
                            url: 'Canvass/CheckLinkIfValid',
                            data: {
                                OrgID: OrgID,
                                cid: vRows[0].PriceCanvassID,
                                vcode: GetUrlParameters().v
                            },
                            success: function (e) {
                                if (e.Results.Rows.length > 0) {
                                    if (!CBool(e.Results.Rows[0].NeverExpireLink)) {
                                        $('#lstNoteExpiration').show();
                                        $('#lblEndDate').text($.format.date(new Date(e.Results.Rows[0].LinkExpirationDate), 'MMMM dd, yyyy'));
                                    } else {
                                        $('#lstNoteExpiration').remove();
                                    }

                                    $('#pageCanvassForm').pageshow();
                                    AjaxSend({
                                        url: 'Canvass/GetLinkCanvassData',
                                        data: {
                                            OrgID: OrgID,
                                            PriceCanvassID: vRows[0].PriceCanvassID,
                                            vcode: GetUrlParameters().v
                                        },
                                        success: function (e) {
                                            tbodyCanvassLink = $('#tbodyCanvassLink');
                                            var data = e.data.Item;
                                            arrVendor = e.data.Vendor;

                                            canvassData = e.data;
                                            vendorData = e.data.Vendor[0];

                                            tbodyCanvassLink.empty();

                                            if (data.length > 0) {
                                                for (var i = 0; i < data.length; i++) {
                                                    if ($('#selAdjustPricesCategory option[value="' + CStr2(data[i].Category) + '"]').length == 0) {
                                                        $('#selAdjustPricesCategory').append(new Option(CStr2(data[i].Category), CStr2(data[i].Category)));
                                                    }

                                                    if ($('#selAdjustPricesBrand option[value="' + CStr2(data[i].Brand) + '"]').length == 0) {
                                                        $('#selAdjustPricesBrand').append(new Option(CStr2(data[i].Brand), CStr2(data[i].Brand)));
                                                    }

                                                    tbodyCanvassLink.append($('<tr>')
                                                        .attr({
                                                            dbid: data[i].DBID,
                                                            category: CStr2(data[i].Category),
                                                            brand: CStr2(data[i].Brand)
                                                        })
                                                        .data('data', data[i])
                                                        .append($('<th scope="row">').text(i + 1 + '.'))
                                                        .append($('<td>').text(data[i].ProductName || data[i].Description))
                                                        .append($('<td>').text(CStr2(data[i].UomName)))
                                                        .append($('<td>').append($('<input type="textbox" data-role="none" class="form-control txtSRP">')
                                                                .keydown(function (e) {
                                                                    if (!NumberDotOnly($(this), e)) {
                                                                        return;
                                                                    }

                                                                    var currentTR = $(this).parents().closest('tr');

                                                                    if (e.which == 38) { // arrow up
                                                                        var prevTR = currentTR.prev();

                                                                        if (prevTR.length > 0) {
                                                                            prevTR.find('.txtSRP').focus();
                                                                        }
                                                                    } else if (e.which == 40 || e.which == 13) { // arrow down and ENTER
                                                                        var nextTR = currentTR.next();

                                                                        if (nextTR.length > 0) {
                                                                            nextTR.find('.txtSRP').focus();
                                                                        }
                                                                    }
                                                                })
                                                                .blur(function () {
                                                                    itemData = $(this).parents().closest('tr').data('data');
                                                                    PopulateVendorData('srp', {
                                                                        Price: $(this).val()
                                                                    })
                                                                })
                                                            )
                                                        )
                                                        .append($('<td>').append($('<input type="textbox" data-role="none" class="form-control txtTradeDiscount" readonly>')
                                                                .data('data', [])
                                                                .click(function (e) {
                                                                    $('#popCanvassTradeDiscount').popup('open');
                                                                    var dis = $(this);
                                                                    var data = dis.data('data');
                                                                    itemData = $(this).parents().closest('tr').data('data');

                                                                    $('#txtTradeDiscountLess, #txtTradeDiscountRemarks').val('');
                                                                    $('#lblTradeDiscountFactor, #lblTradeDiscountRate').text('');
                                                                    $('#txtTradeDiscountLess').focus();

                                                                    if (data.length > 0) {
                                                                        $('#txtTradeDiscountLess').val(data[0].TradeDiscountLess);
                                                                        $('#txtTradeDiscountRemarks').val(data[0].TradeDiscountRemarks);
                                                                        TradeDiscount(data[0].TradeDiscountLess);
                                                                    }

                                                                    $('#btnOkCanvassTradeDiscount').unbind('click').click(function () {
                                                                        var txtTradeDiscountLess = $.trim($('#txtTradeDiscountLess').val());
                                                                        var txtTradeDiscountRemarks = $.trim($('#txtTradeDiscountRemarks').val());

                                                                        if (txtTradeDiscountLess == '' || !isValidTradeDisc) {
                                                                            WebToast('Please enter valid trade discount', 'top');
                                                                            return;
                                                                        }

                                                                        if (data.length == 0) {
                                                                            data.push({
                                                                                TradeDiscountLess: txtTradeDiscountLess,
                                                                                TradeDiscountRemarks: txtTradeDiscountRemarks
                                                                            })
                                                                        } else {
                                                                            data[0].TradeDiscountLess = txtTradeDiscountLess;
                                                                            data[0].TradeDiscountRemarks = txtTradeDiscountRemarks;
                                                                        }

                                                                        PopulateVendorData('tradediscount', {
                                                                            TradeDiscountLess: txtTradeDiscountLess,
                                                                            TradeDiscountRemarks: txtTradeDiscountRemarks
                                                                        })

                                                                        if (CDec2(txtTradeDiscountLess) != 0) {
                                                                            dis.val(txtTradeDiscountLess + ' (' + TradeDiscount(txtTradeDiscountLess).NetRate + '% NDR)')
                                                                        } else {
                                                                            dis.val('');
                                                                        }

                                                                        $('#popCanvassTradeDiscount').popup('close');
                                                                    })
                                                                })
                                                            )
                                                        )
                                                        .append($('<td>').append($('<input type="textbox" data-role="none" class="form-control txtPriceNetOfTradeDiscount">')
                                                                .keydown(function (e) {
                                                                    if (!NumberDotOnly($(this), e)) {
                                                                        return;
                                                                    }

                                                                    var currentTR = $(this).parents().closest('tr');

                                                                    if (e.which == 38) { // arrow up
                                                                        var prevTR = currentTR.prev();

                                                                        if (prevTR.length > 0) {
                                                                            prevTR.find('.txtPriceNetOfTradeDiscount').focus();
                                                                        }
                                                                    } else if (e.which == 40 || e.which == 13) { // arrow down and ENTER
                                                                        var nextTR = currentTR.next();

                                                                        if (nextTR.length > 0) {
                                                                            nextTR.find('.txtPriceNetOfTradeDiscount').focus();
                                                                        }
                                                                    }
                                                                })
                                                                .blur(function () {
                                                                    itemData = $(this).parents().closest('tr').data('data');

                                                                    if (CStr2($.trim($(this).val())) != '') {
                                                                        $(this).parents().closest('tr').find('.txtTradeDiscount').val('');
                                                                        PopulateVendorData('tradediscount', {
                                                                            TradeDiscountLess: '',
                                                                            TradeDiscountRemarks: '',
                                                                            PriceNetOfTradeDiscount: CDec2($(this).val())
                                                                        })
                                                                    }
                                                                })
                                                            )
                                                        )
                                                        .append($('<td>').append($('<input type="textbox" data-role="none" class="form-control txtCashDiscount" readonly>')
                                                                .data('data', [])
                                                                .click(function (e) {
                                                                    itemData = $(this).parents().closest('tr').data('data');
                                                                    isFromAdjustPrices = false;
                                                                    CashDiscount($(this), false);
                                                                })
                                                            )
                                                        )
                                                        .append($('<td>').append($('<textarea data-role="none" class="form-control txtVendorComment">')
                                                                .click(function () {
                                                                    var dis = $(this);
                                                                    itemData = dis.parents().closest('tr').data('data');
                                                                    $('#popCanvassComment').popup('open');
                                                                    $('#txtCanvassComment').val(dis.val()).focus();

                                                                    $('#formCanvassComment').unbind('submit').submit(function (e) {
                                                                        e.preventDefault();

                                                                        dis.val($('#txtCanvassComment').val());
                                                                        PopulateVendorData('comment', {
                                                                            VendorComment: $('#txtCanvassComment').val()
                                                                        })
                                                                        $('#popCanvassComment').popup('close');
                                                                    })
                                                                })
                                                                //.blur(function () {
                                                                //    itemData = $(this).parents().closest('tr').data('data');
                                                                //    PopulateVendorData('comment', {
                                                                //        VendorComment: $(this).val()
                                                                //    })
                                                                //})
                                                            )
                                                        )
                                                    )
                                                }

                                                $('#tableCanvass').table('refresh')
                                            }

                                            if (arrVendor.length > 0) {
                                                $.grep(arrVendor, function (e, i) {
                                                    if (e.CanvassVendorCode == GetUrlParameters().v) {
                                                        indVendor = i;
                                                        return false;
                                                    }
                                                })

                                                if (indVendor != null) {
                                                    $('.lblVendorName').text((CBool(arrVendor[indVendor].IsOrganization) ? arrVendor[indVendor].CompanyName : $.trim(arrVendor[indVendor].FirstName + ' ' + arrVendor[indVendor].MiddleName + ' ' + arrVendor[indVendor].Surname)));
                                                    for (var i = 0; i < arrVendor[indVendor].data.length; i++) {
                                                        var data = arrVendor[indVendor].data[i];
                                                        var tr = $('tr[dbid="' + data.ItemDBID + '"]');

                                                        tr.find('.txtSRP').val(CStr2(data.Price));

                                                        if (CStr2(data.TradeDiscountLess) != '') {
                                                            tr.find('.txtTradeDiscount').data('data', [{
                                                                TradeDiscountLess: data.TradeDiscountLess,
                                                                TradeDiscountRemarks: data.TradeDiscountRemarks
                                                            }]).val(CStr2(data.TradeDiscountLess) + ' (' + TradeDiscount(CStr2(data.TradeDiscountLess)).NetRate + '% NDR)');
                                                        }

                                                        if (CDec2(data.PriceNetOfTradeDiscount) != 0) {
                                                            tr.find('.txtPriceNetOfTradeDiscount').val(CDec2(data.PriceNetOfTradeDiscount));
                                                        }

                                                        if (CStr2(data.CashDiscountIfPaidIn) != '') {
                                                            tr.find('.txtCashDiscount').data('data', [{
                                                                CashDiscountRate: data.CashDiscountRate,
                                                                CashDiscountIfPaidIn: data.CashDiscountIfPaidIn,
                                                                CashDiscountNetAmountToBePaidIn: data.CashDiscountNetAmountToBePaidIn
                                                            }]).val(data.CashDiscountRate + ' % ' + data.CashDiscountIfPaidIn + ', Net ' + data.CashDiscountNetAmountToBePaidIn);
                                                        }

                                                        tr.find('.txtVendorComment').val(CStr2(data.VendorComment));
                                                    }
                                                }
                                            }

                                            var tempinterval = setInterval(function () {
                                                if (typeof $.fn.DataTable != 'undefined') {
                                                    clearInterval(tempinterval)
                                                    GetFiles();
                                                }
                                            }, 100)
                                            
                                        }
                                    })
                                } else {
                                    $('#pageBrokenLink').pageshow();
                                }
                            }
                        })

                        if (GetUrlParameters().v == undefined) {
                            $('.divNeedVendorUser').show();
                        }                  
                    } else {
                        $('#pageBrokenLink').pageshow();
                    }
                }
            })
        })

        $('#btnAdjustPricesOk').click(function () {
            var selAdjustPricesCategory = $('#selAdjustPricesCategory').val();
            var selAdjustPricesBrand = $('#selAdjustPricesBrand').val();
            var apTradeDiscounts = $.trim($('#txtAdjustPricesTradeDiscounts').val());
            var apCashDiscounts = $('#txtAdjustPricesCashDiscounts').data('data');
            var trow;

            if (selAdjustPricesCategory == '' && selAdjustPricesBrand == '') {
                trow = tbodyCanvassLink.find('tr');
            } else if (selAdjustPricesCategory == '' || selAdjustPricesBrand == '') {
                trow = tbodyCanvassLink.find('tr[category="' + selAdjustPricesCategory + '"],[brand="' + selAdjustPricesBrand + '"]');
            } else if (selAdjustPricesCategory != '' && selAdjustPricesBrand != '') {
                trow = tbodyCanvassLink.find('tr[category="' + selAdjustPricesCategory + '"][brand="' + selAdjustPricesBrand + '"]')
            }

            if (apTradeDiscounts == '' && apCashDiscounts.length == 0) {
                WebToast('Please enter at least one discount', 'top');
                return;
            }

            trow.each(function () {
                var dis = $(this);
                var td, tddata;
                itemData = dis.data('data');

                //tbodyCanvassLink.find('tr').removeClass('activeRow');
                //dis.addClass('activeRow');

                if (apTradeDiscounts != '') {
                    td = dis.find('.txtTradeDiscount');
                    tddata = td.data('data');
                    var newtddata = {
                        TradeDiscountLess: apTradeDiscounts
                    }
                    if (tddata.length > 0) {
                        newtddata = $.extend(tddata[0], newtddata)
                    } else {
                        tddata.push(newtddata)
                    }

                    PopulateVendorData('tradediscount', newtddata)
                    td.val(apTradeDiscounts + ' (' + TradeDiscount(apTradeDiscounts).NetRate + '% NDR)');
                }

                if (apCashDiscounts.length > 0) {
                    var cdRate = apCashDiscounts[0].CashDiscountRate, cdifpaid = apCashDiscounts[0].CashDiscountIfPaidIn,
                        cdnet = apCashDiscounts[0].CashDiscountNetAmountToBePaidIn;
                    td = dis.find('.txtCashDiscount');
                    tddata = td.data('data');
                    var newtddata = {
                        CashDiscountRate: cdRate,
                        CashDiscountIfPaidIn: cdifpaid,
                        CashDiscountNetAmountToBePaidIn: cdnet
                    }
                    if (tddata.length > 0) {
                        newtddata = $.extend(tddata[0], newtddata)
                    } else {
                        tddata.push(newtddata)
                    }

                    PopulateVendorData('cashdiscount', newtddata)
                    td.val(cdRate + ' % ' + cdifpaid + ', Net ' + cdnet);
                }
            })

            $('#popCanvassAdjustPrices').hide();
        })

        $('#txtAdjustPricesCashDiscounts').click(function () {
            CashDiscount($('#txtAdjustPricesCashDiscounts'));
        })

        $('#btnAdjustPrice').click(function () {
            if (GetUrlParameters().v == undefined) {
                NewMessageBox('Please sign in as vendor before you can adjust your prices.', 'Message');
                return;
            }

            $('#popCanvassAdjustPrices').show();

            isFromAdjustPrices = true;
            $('#selAdjustPricesCategory').val('');
            $('#selAdjustPricesBrand').val('');
            $('#txtAdjustPricesTradeDiscounts, #txtAdjustPricesCashDiscounts, #txtAdjustPricesVolumeDiscount').val('');
            $('#txtAdjustPricesCashDiscounts').data('data', []);
        })

        $('#btnLoginVendor').click(function () {
            location.href = '/VendorPortal/Signin?OrgID=' + sessionStorage.OrgID + '&redirect=canvassform&c=' + GetUrlParameters().c
        })

        $('#btnDownloadCanvassForm').click(function () {
            if (GetUrlParameters().v == undefined) {
                NewMessageBox('Please sign in as vendor before you can download the canvass form.', 'Message');
                return;
            }
            location.href = GetPathFormat('/Canvass/DownloadCanvassVendor?OrgID=' + sessionStorage.OrgID + '&cid=' + canvassData.CanvassID + '&vid=' + vendorData.ID + '&displayprices=1');
        })

        $('#btnUploadCanvassForm').click(function () {
            NewMessageBox('If you upload a canvass form template, the existing prices will be overwritten by the data of the template.', 'Message', [
                {
                    Text: 'Ok and select a file', Click: function () {
                        $('#fileUploadCanvassFormTemplate').val('')
                        $('#fileUploadCanvassFormTemplate').click()
                    }
                }, { Text: 'Cancel' }
            ])            
        })

        $('#formUploadCanvassFormTemplate').submit(function (e) {
            e.stopPropagation();
            e.preventDefault();

            SecurityValidation(function () {
                var pData = {
                    CanvassID: canvassData.CanvassID,
                    Name: canvassData.Name,
                    IsCreateNewSKU: IsCreateNewSKU,
                    IsFirstUploadCanvass: isFirstUploadCanvass,
                    IsFinalize: true,
                    Type: 'vendor'
                }
                var formdata = new FormData();
                var fileUpload = $('#fileUploadCanvassFormTemplate').get(0);
                var files = fileUpload.files;
                formdata.append('data', JSON.stringify(pData));
                formdata.append(files[0].name, files[0]);

                NewMessageBox('Please wait while uploading your file', 'Message');

                AjaxSend({
                    url: 'Canvass/UploadTemplate?OrgID=' + sessionStorage.OrgID + '&UID=' + sessionStorage.PeopleID,
                    data: formdata,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    success: function (vData) {
                        if (vData.Uploading != undefined) {
                            setTimeout(FollowUpRequestFromServer, 1000);
                            NewMessageBox(vData.Uploading, 'Message');
                        } else if (vData.Error != undefined) {
                            NewMessageBox(vData.Error, 'Message');
                        } else if (vData.NoSKURecords != undefined) {
                            NewMessageBox('Some line item records in your Excel template have no corresponding SKU records in your database.' +
                                '<br><br> Would you like to create SKU database records for these line items from your Excel template?' +
                                '<br><br> If you select No, Mobiz will not load price canvass information for line items that have no existing SKU records in your database. Canvass information for line items that have existing SKU records will be uploaded.', 'Message', [
                                    {
                                        Text: 'Yes, let Mobiz auto create SKUs for these line items that are not yet in my SKU database records',
                                        Validation: function () {
                                            WebToast('This feature is not yet implemented', 'top');
                                            return false;
                                        },
                                        Click: function () {
                                            IsCreateNewSKU = true;
                                            isFirstUploadCanvass = false;
                                        }
                                    }, {
                                        Text: 'No, I will upload new SKU records on my own', Click: function () {
                                            IsCreateNewSKU = false;
                                            isFirstUploadCanvass = false;
                                            $('#formUploadCanvass').submit();
                                        }
                                    }
                                ]);
                        }
                    }
                })
            })            
        })

        $('#fileUploadCanvassFormTemplate').change(function () {
            $('#formUploadCanvassFormTemplate').submit()
        })

        $('#btnUploadAttachments').click(function () {
            $('#fileUploadAttachments').val('')
            $('#fileUploadAttachments').click()
        })

        $('#formUploadAttachments').submit(function (e) {
            e.stopPropagation();
            e.preventDefault();

            var formdata = new FormData();
            var fileUpload = $('#fileUploadAttachments').get(0);
            var files = fileUpload.files;

            NewMessageBox('Please wait while uploading your file', 'Message');

            for (var i = 0; i < files.length; i++) {
                formdata.append(files[i].ID, files[i]);
            }            

            AjaxSend({
                url: 'OrganizationShort/UploadFiles?OrgID=' + sessionStorage.OrgID + '&id=' + vendorData.ID,
                data: formdata,
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                dataType: 'json',
                success: function (vData) {
                    if (vData.Results == 'uploaded') {
                        NewMessageBoxClose();
                        WebToast('Successfully uploaded', 'top');
                        GetFiles()
                    } else {
                        NewMessageBox(vData.Results, 'Error');
                    }
                }
            })
        })

        $('#fileUploadAttachments').change(function () {
            $('#formUploadAttachments').submit()
        })

        $('#txtTradeDiscountLess').keydown(function (e) {
            var kCode = e.keyCode;
            // allow only number, comma, backspace, tab, enter, period, paste, undo
            if (((kCode < 48 || kCode > 57) && (kCode < 96 || kCode > 105) && (kCode != 188 && kCode != 8 && kCode != 9 && kCode != 13 && kCode != 190 && kCode != 110) && (kCode < 37 || kCode > 40))
                || (e.ctrlKey == true && (kCode != 86 && kCode != 90))) {
                e.preventDefault();
            }
        }).keyup(function () {
            TradeDiscount($(this).val());
        })

        $('#txtCanvassCDRate').keydown(function (e) {
            if(!NumberDotOnly($(this), e)){
                return;
            }
        })

        $('#txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        })

        $('#btnCanvassLinkSave').click(function (e) {
            var isEmpty = true;
            
            $('#tbodyCanvassLink tr').each(function () {
                $.each($(this).find('input[type="textbox"], textarea'), function () {
                    if ($.trim($(this).val()) != '') {
                        isEmpty = false;
                    }
                })
            })

            if(isEmpty){
                NewMessageBox('Empty fields');
                return;
            }

            SecurityValidation(function () {
                AjaxSend({
                    url: 'Canvass/SaveLinkedVendorData',
                    data: {
                        OrgID: OrgID,
                        vcode: GetUrlParameters().v,
                        data: arrVendor[indVendor]
                    },
                    success: function (e) {
                        if (e.Results != undefined) {
                            $('#pageTY').pageshow();
                        } else if (e.Error != undefined) {
                            console.log(e.Error);
                            $('#pageError').pageshow();
                        }
                    }
                })
            });
        })

        var tableAttachments;
        function GetFiles() {
            AjaxSend({
                url: 'OrganizationShort/GetFiles',
                data: { ID: vendorData.ID },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var ctr = 0;

                    $('#tbodyAttachments').empty()

                    if ($.fn.DataTable.isDataTable("#tableAttachments")) {
                        //check if datatable is created 
                        $('#tableAttachments').DataTable().clear().destroy();
                    }

                    tableAttachments = $('#tableAttachments').DataTable({
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
                                    return data.FileName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                                }
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none" class="btn custom-btn-theme-h btn-sm">')
                                        .text('Download')
                                        .click(function () {
                                            $.fileDownload(MergeUrlParameters({
                                                OrgID: sessionStorage.OrgID,
                                                ClientID: vendorData.ID,
                                                FileName: rowData.FileName,
                                                DownloadFileName: rowData.FileName
                                            }, 'OrganizationShort/DownloadFile'), {
                                                prepareCallback: function () {
                                                    NewMessageBox('Preparing your file. Please wait ...');
                                                },
                                                successCallback: function () {
                                                    NewMessageBoxClose();
                                                },
                                                failCallback: function (responseHtml, url, error) {
                                                    var res = JSON.parse(responseHtml).d.Results;
                                                    NewMessageBox(res, 'Message');
                                                }
                                            });
                                        });
                                    $(td).html(b);
                                },
                                "width": "100px"
                            }
                        ],
                        "initComplete": function (row, data) {
                            $('#tableAttachments').table('refresh');
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

        function SecurityValidation(callback) {
            $('#popUserLogin').popup('open');

            $('#txtUsername, #txtPassword').val('');
            $('#errLogin').hide();

            $('#formCanvassLogin').unbind('submit').submit(function (e) {
                e.preventDefault();
                e.stopPropagation();

                $('#errLogin').hide();

                if ($.trim($('#txtUsername').val()) == '' || $.trim($('#txtPassword').val()) == '') {
                    $('#errLogin').show();
                    return;
                }

                AjaxSend({
                    url: 'VendorPortal/Login',
                    data: {
                        uname: $.trim($('#txtUsername').val()),
                        pword: $.trim($('#txtPassword').val()),
                        iscanvass: 1,
                        canvasscode: GetUrlParameters().c,
                        OrgID: sessionStorage.OrgID
                    }, success: function (e) {
                        if(e.Results == 'invalid'){
                            $('#errLogin').show();
                        } else {
                            $('#popUserLogin').popup('close');
                            callback();
                        }
                    }
                })
            })
        }

        function FollowUpRequestFromServer() {
            AjaxSend({
                url: 'Canvass/UploadRequestFollowUp',
                success: function (vData) {
                    if (CStr2(vData.Error) != '') {
                        NewMessageBox(vData.Error,
                                    "Excel Data",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                    } else if (CStr2(vData.Uploading) != '') {
                        setTimeout(FollowUpRequestFromServer, 1000);
                        NewMessageBox(vData.Uploading,
                                "Excel Data",
                                [
                                    { Text: 'Ok' }
                                ]
                        );
                    } else if (CStr2(vData.Success) != '') {
                        NewMessageBox('Successfully Uploaded! Please wait while retrieving data',
                                    "Excel Data",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                    } else {
                        $('#pageTY').pageshow();
                        NewMessageBoxClose();
                    }
                }
            });
        }

        function PopulateVendorData(column, data) {
            var descData = itemData;
            var selCanvassVendorData = arrVendor[indVendor].data;
            var pData = {
                ItemDBID: descData.DBID,
                SkuID: descData.ID
            }
            indVendorData = null;

            $.extend(pData, data);

            if (selCanvassVendorData.length == 0) {
                indVendorData = 0;
                selCanvassVendorData.push(pData)
                if (column != 'comment') {
                    ComputePrices(pData);
                }
            } else {
                var ind = null;

                $.grep(selCanvassVendorData, function (e, i) {
                    indVendorData = i;
                    if (e.ItemDBID == descData.DBID) {
                        ind = i;
                        return false;
                    }
                })

                if (ind == null) {
                    indVendorData += 1;
                    selCanvassVendorData.push(pData);
                    if (column != 'comment') {
                        ComputePrices(pData);
                    }
                } else {
                    indVendorData = ind;
                    if (column == 'srp') {
                        selCanvassVendorData[ind].Price = data.Price;
                    } else if (column == 'tradediscount') {
                        selCanvassVendorData[ind].TradeDiscountLess = data.TradeDiscountLess;
                        selCanvassVendorData[ind].TradeDiscountRemarks = data.TradeDiscountRemarks;
                        selCanvassVendorData[ind].PriceNetOfTradeDiscount = data.PriceNetOfTradeDiscount;
                    } else if (column == 'cashdiscount') {
                        selCanvassVendorData[ind].CashDiscountRate = data.CashDiscountRate;
                        selCanvassVendorData[ind].CashDiscountIfPaidIn = data.CashDiscountIfPaidIn;
                        selCanvassVendorData[ind].CashDiscountNetAmountToBePaidIn = data.CashDiscountNetAmountToBePaidIn;
                    } else if (column == 'volumediscount') { // to be implemented
                        //selCanvassVendorData[ind].VolumeDiscount = data.VolumeDiscount;
                    } else if (column == 'comment') {
                        selCanvassVendorData[ind].VendorComment = data.VendorComment;
                    }

                    if (column != 'comment') {
                        ComputePrices(selCanvassVendorData[ind]);
                    }
                }
            }
        }

        function ComputePrices(data) {
            if (CDec2(data.Price) != 0 && CStr2(data.TradeDiscountLess) != '') {
                data.PriceNetOfTradeDiscount = ((CDec2(TradeDiscount(CStr2(data.TradeDiscountLess)).Factor) / 100) * CDec2(data.Price)).toFixed(4);
                $('tr[dbid="' + data.ItemDBID + '"]').find('.txtPriceNetOfTradeDiscount').val(data.PriceNetOfTradeDiscount);
            }

            if (CStr2(data.PriceNetOfTradeDiscount) != '') {
                data.PriceNetOfTradeAndVolumeDiscount = (CDec2(data.PriceNetOfTradeDiscount) * (1 - (CDec2(data.VolumeDiscount) / 100))).toFixed(4)
            }

            if (CStr2(data.PriceNetOfTradeAndVolumeDiscount) != '') {
                data.PriceNetOfTradeVolumeAndCashDiscount = (CDec2(data.PriceNetOfTradeAndVolumeDiscount) * (1 - (CDec2(data.CashDiscountRate) / 100))).toFixed(4)
            }

            //dili pa ni mao about sa VolumeDiscount
            //if (CStr2(data.PriceNetOfTradeAndVolumeDiscount) != '' && CStr2(data.VolumeDiscount) != '') {
            //    data.PriceNetOfTradeVolumeAndCashDiscount = CDec2(data.PriceNetOfTradeAndVolumeDiscount) * (1 - (CDec2(data.VolumeDiscount) / 100))
            //}

            //formula sa Lowest price = Net of Trade Discount * Volume Discount * Cash Discount (in order daw)
            data.LowestPrice = (CDec2(data.PriceNetOfTradeDiscount) * (1 - (CDec2(data.VolumeDiscount) / 100)) * (1 - (CDec2(data.CashDiscountRate) / 100))).toFixed(4) // 4 decimals lang
            console.log(data);
        }

        function TradeDiscount(val) {
            var less = val.split(',');
            var factor = 0, rate = 0;

            for (var i = 0; i < less.length; i++) {
                if (IsNumeric(less[i])) {
                    if (factor == 0) {
                        factor = 1 - (CDec2(less[i]) / 100);
                    } else {
                        factor = factor * (1 - (CDec2(less[i]) / 100));
                    }
                    isValidTradeDisc = true;
                } else {
                    isValidTradeDisc = false;
                    factor = 0;
                    break;
                }
            }
            var realfactor = factor * 100;
            var realrate = (1 - factor) * 100;
            var netfactor = CDec2(realfactor.toFixed(4));
            var netrate = CDec2(realrate.toFixed(4));
            $('#lblTradeDiscountFactor').text(netfactor + '%');
            $('#lblTradeDiscountRate').text(netrate + '%');

            return {
                Factor: realfactor,
                Rate: realrate,
                NetFactor: netfactor,
                NetRate: netrate
            }
        }

        function CashDiscount(dis, isFromAdjustPrices) {
            var data = dis.data('data');
            var txtCanvassCDRate = $('#txtCanvassCDRate');
            var txtCanvassCDIfPaidIn = $('#txtCanvassCDIfPaidIn');
            var txtCanvassCDNetAmount = $('#txtCanvassCDNetAmount');

            itemData = data;

            $('#popCanvassCashDiscount').popup('open');
            $('#txtCanvassCDRate, #txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').val('');

            if(data.length > 0){
                txtCanvassCDRate.val(data[0].CashDiscountRate);
                txtCanvassCDIfPaidIn.val(data[0].CashDiscountIfPaidIn);
                txtCanvassCDNetAmount.val(data[0].CashDiscountNetAmountToBePaidIn);
            }

            txtCanvassCDRate.focus();

            $('#formCanvassCashDiscount').unbind('submit').submit(function (e) {
                e.preventDefault();
                
                if (txtCanvassCDRate.val() == '' || txtCanvassCDIfPaidIn.val() == '' || txtCanvassCDNetAmount.val() == '') {
                    WebToast('Please fill the required fields', 'top');
                    return;
                }

                var cd = txtCanvassCDRate.val() + ' % ' + txtCanvassCDIfPaidIn.val() + ', Net ' + txtCanvassCDNetAmount.val()

                $('#popCanvassCashDiscount').popup('close');

                if (isFromAdjustPrices) {
                    var apcd = $('#txtAdjustPricesCashDiscounts');

                    if (CDec2(txtCanvassCDRate.val()) != 0) {
                        apcd.data('data', [
                            {
                                CashDiscountRate: txtCanvassCDRate.val(),
                                CashDiscountIfPaidIn: txtCanvassCDIfPaidIn.val(),
                                CashDiscountNetAmountToBePaidIn: txtCanvassCDNetAmount.val()
                            }
                        ]).val(cd);
                    } else {
                        apcd.data('data', []).val('');
                    }

                    return;
                }

                var newData = {
                    CashDiscountRate: CDec2(txtCanvassCDRate.val()),
                    CashDiscountIfPaidIn: CDec2(txtCanvassCDIfPaidIn.val()),
                    CashDiscountNetAmountToBePaidIn: CDec2(txtCanvassCDNetAmount.val())
                }

                if(data.length == 0){
                    data.push(newData)
                } else {
                    $.extend(data[0], newData);
                }

                PopulateVendorData('cashdiscount', {
                    CashDiscountRate: txtCanvassCDRate.val(),
                    CashDiscountIfPaidIn: txtCanvassCDIfPaidIn.val(),
                    CashDiscountNetAmountToBePaidIn: txtCanvassCDNetAmount.val()
                })

                if (CDec2(txtCanvassCDRate.val()) != 0) {
                    dis.val(cd);
                }

                if (isFromAdjustPrices) {
                    $('#popCanvassAdjustPrices').popup('open');
                }
            })
        }
    })
})
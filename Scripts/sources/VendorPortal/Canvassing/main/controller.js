define([GetPathFormat('/Scripts/jquery.bxslider.min.js')], function () {
    var mDeferred = $.Deferred();
    var mHtml;
    var isAccessThroughCanvassLink = location.pathname.toLowerCase().includes("canvass");

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/VendorPortal/Canvassing/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.VendorCanvassing.main.controller = ERP.VendorCanvassing.main.controller();
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.VendorCanvassing.main.controller', null, function () {

        var mOut = {}, tblVendorCanvass;
        var isValidTradeDisc = false, itemData, arrVendor, indVendor = null, OrgID;
        var canvassData, IsCreateNewSKU = false, isFirstUploadCanvass = true, isFromAdjustPrices = false;
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        if (isAccessThroughCanvassLink) {
            $('#pageCanvassForm').pageshow();
            $('#divCanvassFormLink').empty().append(mElement).trigger('create');
            setTimeout(function () {
                var tempinterval = setInterval(function () {
                    if (typeof AddBreadCrumb != 'undefined') {
                        clearInterval(tempinterval)
                        $('.ItemsBreadCrumbs').empty();
                        AddBreadCrumb({
                            text: 'Canvass Form', Click: function () {
                                $('#divVendorCanvassDetails').pageshow();
                            }
                        }, '.ItemsBreadCrumbs')
                    }
                }, 100)
                $('#lblWelcomeForCanvassLink').show();
                $('#btnCanvassLinkCancel').remove();
                $('#divVendorCanvassDetails').pageshow();
                GetCanvassLinkData();
            }, 100);
        } else {
            $('#divVendorCanvass').empty().append(mElement).trigger('create');
            $('#divVendorCanvassList').pageshow();
        }

        $('#btnCanvassInfoNotes').click(function () {
            NewMessageBox('1. Please enter your prices for each item. <br><br>' +
                '2. If ever the resulting value of the "SRP" less "Trade Discount" is not equal to the "Price Net of Trade Discount", the system will follow the values encoded in the "Price Net of Trade Discount" column. <br><br>' +
                '3. You can use arrow up/down or ENTER keys to navigate to the next field.<br><br>' +
                '4. Click DONE button below to submit your prices.',
                'Important Note')
        })

        $('#formVendorCanvassSearch').submit(function (e) {
            e.preventDefault();
            e.stopPropagation();
            mOut.GetSupplierPriceCanvass();
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
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })

        $('#txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        })

        $('#btnCanvassLinkCancel').click(function () {
            AddBreadCrumb({
                text: 'Canvass Forms', Click: function () {
                    $('#divVendorCanvassList').pageshow();
                }
            }, '.ItemsBreadCrumbs')
            $('#divVendorCanvassList').pageshow();
        })

        $('#btnCanvassLinkSave').click(function (e) {
            var isEmpty = true;

            $('#tbodyVendorCanvassSku tr').each(function () {
                $.each($(this).find('input[type="textbox"], textarea'), function () {
                    if ($.trim($(this).val()) != '') {
                        isEmpty = false;
                    }
                })
            })

            if (isEmpty) {
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
                            if (isAccessThroughCanvassLink) {
                                $('#pageTY').pageshow();
                            } else {
                                mOut.GetSupplierPriceCanvass();
                                $('#divVendorCanvassList').pageshow();
                                WebToast('Canvass Form has been saved', 'top');                                
                            }
                        } else if (e.Error != undefined) {
                            NewMessageBox(e.Error, 'Error Message');
                        }
                    }
                })
            })            
        })

        $('#btnCanvassSkuPicCancel').click(function () {
            if (!isAccessThroughCanvassLink){
                AddBreadCrumb({
                    text: canvassData.CanvassName, Click: function () {
                        $('#divVendorCanvass, #divVendorCanvassDetails').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            } else {
                AddBreadCrumb({
                    text: 'Canvass Form', Click: function () {
                        $('#divVendorCanvassDetails').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            }

            $('#divVendorCanvassDetails').pageshow();
        })

        $('#btnAdjustPrice').click(function () {
            $('#popCanvassAdjustPrices').modal('show');

            isFromAdjustPrices = true;
            $('#selAdjustPricesCategory').val('');
            $('#selAdjustPricesBrand').val('');
            $('#txtAdjustPricesTradeDiscounts, #txtAdjustPricesCashDiscounts, #txtAdjustPricesVolumeDiscount').val('');
            $('#txtAdjustPricesCashDiscounts').data('data', []);
        })

        $('#btnAdjustPricesOk').click(function () {
            var selAdjustPricesCategory = $('#selAdjustPricesCategory').val();
            var selAdjustPricesBrand = $('#selAdjustPricesBrand').val();
            var apTradeDiscounts = $.trim($('#txtAdjustPricesTradeDiscounts').val());
            var apCashDiscounts = $('#txtAdjustPricesCashDiscounts').data('data');
            var tbodyVendorCanvassSku = $('#tbodyVendorCanvassSku');
            var trow;

            if (selAdjustPricesCategory == '' && selAdjustPricesBrand == '') {
                trow = tbodyVendorCanvassSku.find('tr');
            } else if (selAdjustPricesCategory == '' || selAdjustPricesBrand == '') {
                trow = tbodyVendorCanvassSku.find('tr[category="' + selAdjustPricesCategory + '"],[brand="' + selAdjustPricesBrand + '"]');
            } else if (selAdjustPricesCategory != '' && selAdjustPricesBrand != '') {
                trow = tbodyVendorCanvassSku.find('tr[category="' + selAdjustPricesCategory + '"][brand="' + selAdjustPricesBrand + '"]')
            }

            if (apTradeDiscounts == '' && apCashDiscounts.length == 0) {
                WebToast('Please enter at least one discount', 'top');
                return;
            }

            trow.each(function () {
                var dis = $(this);
                var td, tddata;
                itemData = dis.data('data');

                //tbodyVendorCanvassSku.find('tr').removeClass('activeRow');
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

            $('#popCanvassAdjustPrices').modal('hide');
        })

        $('#btnCloseCashDiscount').click(function () {
            $('#popCanvassCashDiscount').modal('hide');

            if (isFromAdjustPrices) {
                $('#popCanvassAdjustPrices').modal('show');
            }            
        })

        $('#txtAdjustPricesCashDiscounts').click(function () {
            CashDiscount($('#txtAdjustPricesCashDiscounts'));
        })

        $('#btnDownloadCanvassForm').click(function () {
            NewMessageBox('You\'re about to download this canvass form to excel format. You can edit the prices through the excel file then you can re-upload the edited file later on.',
                        'Message', [
                            {
                                Text: 'Download Canvass to Excel', Click: function () {
                                    location.href = GetPathFormat('/Canvass/DownloadCanvassVendor?OrgID=' + sessionStorage.OrgID + '&cid=' + canvassData.ID + '&vid=' + sessionStorage.SupplierID + '&displayprices=1');
                                }
                            }, { Text: 'Cancel' }
                        ])            
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
                    CanvassID: canvassData.ID,
                    Name: canvassData.CanvassName,
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

            WebToast('Please wait while uploading your file', 'top');

            for (var i = 0; i < files.length; i++) {
                formdata.append(files[i].ID, files[i]);
            }

            AjaxSend({
                url: 'OrganizationShort/UploadFiles?OrgID=' + sessionStorage.OrgID + '&id=' + sessionStorage.SupplierID,
                data: formdata,
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                dataType: 'json',
                success: function (vData) {
                    if (vData.Results == 'uploaded') {
                        NewMessageBox('Files has been successfully uploaded', 'Message');
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

        $('#txtCanvassAttachmentsSearch').keydown(function (e) {
            if (e.which == 13) {
                GetFiles();
            }
        })

        $('#btnSkuPicAttachments').click(function () {
            $('#fileSkuPicAttachments').val('')
            $('#fileSkuPicAttachments').click()
        })

        $('#fileSkuPicAttachments').change(function (e) {
            e.stopPropagation();
            e.preventDefault();

            var input = e.target;
            var reader = new FileReader();
            var img = $('#canvasImage');
            var clippedImageH, clippedImageW;
            var canvasArea;

            reader.onload = function () {
                var dataURL = reader.result;
                var compressOutput = document.getElementById('compressImage');
                compressOutput.src = dataURL;

                var source_image = document.getElementById('compressImage');
                var result_image = $('#canvasImage');
                var thumbnail_image = $('#thumbnail');
                var final_image = $('#finalImage');
                var quality = parseInt(99);
                var canvasorig, cSmall, cMedium, cBig, resizedImage;
                output_format = input.files[0].type;
                source_image.onload = function () {
                    result_image.attr('src', imageCompressor(source_image, quality, output_format).src);
                }

                result_image.on('load', function () {
                    AddBreadCrumb({
                        text: 'Cropping', Click: function () {
                            $('#divVendorCanvass, #divVendorCanvassSkuCropPic').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')

                    result_image.show();
                    $('#divVendorCanvassSkuCropPic').pageshow();
                    $('.imgareaselect-outer').show();
                    $('.imgareaselect-selection').show();
                    $('.imgareaselect-border1').show();
                    $('.imgareaselect-border2').show();
                    $('.imgareaselect-border3').show();
                    $('.imgareaselect-border4').show();
                    $('#fileSkuPicAttachments').val('');

                    if (canvasArea != undefined) {
                        $('#canvasImage').imgAreaSelect({ remove: true })
                        canvasArea.setOptions({ hide: false })
                        canvasArea.update();
                    }

                    canvasArea = $('#canvasImage').imgAreaSelect({
                        instance: true,
                        minWidth: 100,
                        minHeight: 100,
                        resizable: true,
                        handles: true,
                        persistent: true,
                        show: true,
                        aspectRatio: "1:1",
                        parent: $('#divVendorCanvassSkuCropPic'),
                        x1: 0, y1: 0, x2: 100, y2: 100,
                        onSelectEnd: function (img, selection) {
                            x1 = selection.x1;
                            y1 = selection.y1;
                            x2 = selection.x2// - selection.x1;
                            y2 = selection.y2// - selection.y1;
                            clippedImageH = selection.height;
                            clippedImageW = selection.width;
                        },
                    });
                })

                $("#btnCancelCrop, .btnPIMMenu").unbind('click').click(function () {
                    AddBreadCrumb({
                        text: 'Picture Gallery', Click: function () {
                            $('#divVendorCanvass, #divVendorCanvassSkuPic').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                    $('#divVendorCanvassSkuPic').pageshow();
                    canvasArea.cancelSelection();
                    canvasArea.setOptions({ hide: false })
                    canvasArea.update();
                });

                $("#btnDoneCrop").unbind('click').click(function () {

                    /* Saving the images */
                    canvasorig = $('<canvas>')[0] //document.getElementById('canvasorig');
                    cSmall = $('<canvas>')[0];
                    cMedium = $('<canvas>')[0];
                    cBig = $('<canvas>')[0];

                    cSmall.width = "50";
                    cSmall.height = "50";
                    cMedium.width = "100";
                    cMedium.height = "100";
                    cBig.width = "400";
                    cBig.height = "400";

                    canvasorig.width = CDec2(img[0].width);
                    canvasorig.height = CDec2(img[0].height);

                    var wRatio = canvasorig.width / CDec2(img[0].naturalWidth);
                    var hRatio = canvasorig.height / CDec2(img[0].naturalHeight);
                    var ratio = Math.min(wRatio, hRatio);

                    if (x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0) {
                        x1 = 120;
                        y1 = 90;
                        x2 = 220;
                        y2 = 190;
                        clippedImageH = 100;
                        clippedImageW = 100;
                    }

                    canvasorig.getContext("2d").drawImage(img[0], 0, 0, CDec2(img[0].naturalWidth), CDec2(img[0].naturalHeight), 0, 0, Math.round(CDec2(img[0].naturalWidth) * ratio), Math.round(CDec2(img[0].naturalHeight) * ratio));

                    resizedImage = document.getElementById("resizedImage");
                    resizedImage.src = canvasorig.toDataURL(output_format, 1);
                    //drawRotated();
                    setTimeout(function () {
                        //drawRotated();
                        cSmall.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 50, 50);
                        cMedium.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 100, 100);
                        cBig.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 400, 400);
                        thumbnail_image.src = cSmall.toDataURL(output_format, 1);

                        //if ($('#imgSelection img').length == 0) { // display picture if there's no default
                        //    final_image.src = cBig.toDataURL(output_format, 1);
                        //}

                        /* Send image src to save */
                        //var imageString = final_image.src.replace('data:' + output_format + ';base64,', '');
                        var prefixID = NewID();

                        SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: itemData.ID, imageData: cBig.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'big', VendorID: sessionStorage.SupplierID, isFromVendor: true }), function () {
                            SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: itemData.ID, imageData: cMedium.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'medium', VendorID: sessionStorage.SupplierID, isFromVendor: true }), function () {
                                SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: itemData.ID, imageData: cSmall.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'small', VendorID: sessionStorage.SupplierID, isFromVendor: true }), function () {
                                    $("#btnCancelCrop").click();
                                    GetPIMPictures();
                                    WebToast('Successfully uploaded', 'top');                                    
                                })
                            })
                        })

                        $('#fileUploadProductImage').val('');
                    }, 500)

                    /* Refresh current page to refresh the Cropping libraries */
                    //location.reload();
                });
            };
            reader.readAsDataURL(input.files[0]);            
        })

        mOut.GetSupplierPriceCanvass = function () {
            AddBreadCrumb({
                text: 'Canvass Forms', Click: function () {
                    $('#divVendorCanvass, #divVendorCanvassList').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            AjaxSend({
                url: 'Canvass/GetSupplierPriceCanvass',
                data: { VendorID: sessionStorage.SupplierID, keywords: $.trim($('#txtVendorCanvassSearch').val()) },
                success: function (e) {
                    var tblBodyVendorCanvass = $('#tblBodyVendorCanvass');
                    tblBodyVendorCanvass.empty();

                    if (e.Error != undefined) {
                        NewMessageBox(e.Error, 'Error Message');
                    } else {
                        var vRows = e.Results.Rows, ctr = 0;

                        if ($.fn.DataTable.isDataTable("#tblVendorCanvass")) {
                            //check if datatable is created 
                            $('#tblVendorCanvass').DataTable().clear().destroy();
                        }

                        tblVendorCanvass = $('#tblVendorCanvass').DataTable({
                            retrieve: true,
                            "bdestroy": true,
                            "pageLength": 50,
                            "data": vRows,
                            "autoWidth": false,
                            "ordering": false,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return '<b class="ui-table-cell-label">#</b>' + ctr + '.';
                                    },
                                    "width": "30px"
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<b class="ui-table-cell-label">Canvass Name</b>' + data.CanvassName;
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<b class="ui-table-cell-label">Last Updated</b>' + $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                                    },
                                    "width": "200px"
                                }
                            ],
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                            "responsive": true,
                            "columnDefs": [{ "orderable": false }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">'
                            //"stateSave": true
                        })

                        $('#tblVendorCanvass tbody').unbind('click').on('click', 'tr', function () {
                            if (CStr2(tblVendorCanvass.row(this).data()) != '') {
                                canvassData = tblVendorCanvass.row(this).data();

                                AddBreadCrumb({
                                    text: canvassData.CanvassName, Click: function () {
                                        $('#divVendorCanvass, #divVendorCanvassDetails').pageshow();
                                    }
                                }, '.ItemsBreadCrumbs')
                                mOut.GetPriceCanvassData(canvassData.ID);
                            }
                        });
                    }
                }
            })
        }

        mOut.GetPriceCanvassData = function (canvassID) {
            AjaxSend({
                url: 'Canvass/GetLinkCanvassData',
                data: { PriceCanvassID: canvassID, VendorID: sessionStorage.SupplierID },
                success: function (e) {
                    var vRows = e.data.Item, ctr = 0;
                    arrVendor = e.data.Vendor;

                    $('#divVendorCanvassDetails').pageshow();
                    $('#tabCanvassForm').opentabmenu();
                    $('#tbodyVendorCanvassSku').empty();

                    if (CStr2(sessionStorage.SupplierID) != '') {
                        $('.lblVendorName').text((CBool(arrVendor[0].IsOrganization) ? arrVendor[0].CompanyName : $.trim(arrVendor[0].FirstName + ' ' + arrVendor[0].MiddleName + ' ' + arrVendor[0].Surname)));
                    }

                    if ($.fn.DataTable.isDataTable("#tblVendorCanvassSku")) {
                        //check if datatable is created 
                        $('#tblVendorCanvassSku').DataTable().clear().destroy();
                    }

                    tblVendorCanvassSku = $('#tblVendorCanvassSku').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 50,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return '<b class="ui-table-cell-label">#</b>' + ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none"  data-toggle="tooltip" data-placement="top" title="Picture Gallery" class="btn custom-btn-theme-b">')
                                        .data('data', rowData)
                                        .append($('<span class="glyphicon glyphicon-camera">'))
                                        .click(function () {
                                            itemData = $(this).data('data');
                                            AddBreadCrumb({
                                                text: 'Picture Gallery', Click: function () {
                                                    $('#divVendorCanvass, #divVendorCanvassSkuPic').pageshow();
                                                }
                                            }, '.ItemsBreadCrumbs')
                                            $('#lblCanvassSkuName').text((itemData.ProductName || itemData.Description));
                                            $('#divVendorCanvassSkuPic').pageshow();
                                            GetPIMPictures();
                                        });
                                    $(td).html(b);
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Item Description:</b>' + (data.ProductName || data.Description);
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.UomName;
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">SRP:</b> <input type="textbox" data-role="none" class="form-control unicase-form-control text-input txtSRP">';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Trade Discount (%):</b> <input type="textbox" data-role="none" readonly class="form-control unicase-form-control text-input txtTradeDiscount">';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Price Net Of Trade Discount:</b> <input type="textbox" data-role="none" class="form-control unicase-form-control text-input txtPriceNetOfTradeDiscount">';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Cash Discount:</b> <input type="textbox" data-role="none" readonly class="form-control unicase-form-control text-input txtCashDiscount">';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Comment:</b> <textarea data-role="none" class="form-control unicase-form-control text-input txtVendorComment" style="height:42px; resize:none;"></textarea>';
                                }
                            }
                        ],
                        "rowCallback": function (row, data) {
                            var tr = $(row);

                            if ($('#selAdjustPricesCategory option[value="' + CStr2(data.Category) + '"]').length == 0) {
                                $('#selAdjustPricesCategory').append(new Option(CStr2(data.Category), CStr2(data.Category)));
                            }

                            if ($('#selAdjustPricesBrand option[value="' + CStr2(data.Brand) + '"]').length == 0) {
                                $('#selAdjustPricesBrand').append(new Option(CStr2(data.Brand), CStr2(data.Brand)));
                            }

                            tr.data('data', data);
                            tr.attr('dbid', data.DBID);

                            tr.find('.txtSRP')
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
                                .change(function () {
                                    itemData = tr.data('data');
                                    PopulateVendorData('srp', {
                                        Price: $(this).val()
                                    })
                                })

                            tr.find('.txtTradeDiscount').data('data', []).click(function () {
                                $('#popCanvassTradeDiscount').modal('show');
                                var dis = $(this);
                                var data = dis.data('data');
                                itemData = tr.data('data');

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

                                    $('#popCanvassTradeDiscount').modal('hide');
                                })
                            })

                            tr.find('.txtPriceNetOfTradeDiscount')
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
                                .change(function () {
                                    itemData = tr.data('data');

                                    tr.find('.txtTradeDiscount').val('');
                                    PopulateVendorData('tradediscount', {
                                        TradeDiscountLess: '',
                                        TradeDiscountRemarks: '',
                                        PriceNetOfTradeDiscount: CDec2($(this).val())
                                    })
                                })

                            tr.find('.txtCashDiscount').data('data', []).click(function () {
                                itemData = tr.data('data');
                                isFromAdjustPrices = false;
                                CashDiscount($(this));
                            })

                            tr.find('.txtVendorComment').click(function () {
                                itemData = tr.data('data');
                                var dis = $(this);

                                $('#popCanvassComment').modal('show');
                                $('#txtCanvassComment').val(dis.val()).focus();

                                $('#btnOkCanvassComment').unbind('click').click(function (e) {
                                    e.preventDefault();

                                    dis.val($('#txtCanvassComment').val());
                                    PopulateVendorData('comment', {
                                        VendorComment: $('#txtCanvassComment').val()
                                    })
                                    $('#popCanvassComment').modal('hide');
                                })
                            })
                        },
                        "initComplete": function (row, data) {

                            $.grep(arrVendor, function (e, i) {
                                indVendor = i;
                                return false;
                            })

                            if (indVendor != null) {
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
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "columnDefs": [{ "orderable": false }],
                        "sDom": '<"top">rt<"bottom"><"clear">'
                        //"stateSave": true
                    })

                    //$('#tblVendorCanvassSku tbody').unbind('click').on('click', 'tr', function () {
                    //    if (CStr2(tblVendorCanvassSku.row(this).data()) != '') {
                    //        mOut.GetPriceCanvassData(tblVendorCanvassSku.row(this).data().ID);
                    //    }
                    //});

                    GetFiles();
                }
            })
        }

        if (!isAccessThroughCanvassLink) {
            mOut.GetSupplierPriceCanvass();
        }

        function GetCanvassLinkData() {
            AjaxSend({
                url: 'Canvass/GetLinkData',
                data: { code: GetUrlParameters().c },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        OrgID = vRows[0].OrgID;
                        sessionStorage.OrgID = OrgID;

                        if (GetUrlParameters().v == undefined) {
                            $('.divNeedVendorUser').show();
                            sessionStorage.SupplierID = '';
                            mOut.GetPriceCanvassData(vRows[0].PriceCanvassID);
                            return;
                        }

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
                                    
                                    sessionStorage.SupplierID = e.Results.Rows[0].VendorID;

                                    mOut.GetPriceCanvassData(e.Results.Rows[0].ID);
                                } else {
                                    $('#pageBrokenLink').pageshow();
                                }
                            }
                        })                        
                    } else {
                        $('#pageBrokenLink').pageshow();
                    }
                }
            })
        }

        var carousel = null;
        function GetPIMPictures() {
            var imgSelection = $("#imgSelection");
            imgSelection.empty();

            AjaxSend({
                url: 'Organization/GetPIMPictures',
                data: { ProductID: itemData.ID, IsFromVendor: true, VendorID: sessionStorage.SupplierID },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    
                    var divPIMPicPreview = $('#divPIMPicPreview');
                    var divPIMPicPreviewThumbnails = $('#divPIMPicPreviewThumbnails');
                    var table = $('<table>');
                    
                    divPIMPicPreview.empty();
                    divPIMPicPreviewThumbnails.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            vRows[i].Index = i;
                            vRows[i].isFromVendor = true;
                            vRows[i].OrgID = sessionStorage.OrgID;
                            vRows[i].ViewID = itemData.ID;
                            vRows[i].VendorID = sessionStorage.SupplierID;

                            var td = $('<div class="col-sm-1">');
                            var srcMed = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + sessionStorage.SupplierID + '/' + vRows[i].FileName;
                            var srcBig = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + sessionStorage.SupplierID + '/' + vRows[i].GroupImageID + '_big.jpg';
                            var img = $('<img>');
                            var dropdown = $('<div class="dropdown" data-toggle="tooltip" data-placement="right" title="Options">')
                                    .append($('<button data-role="none" class="btn custom-btn-theme-m btn-less-width dropdown-toggle" data-toggle="dropdown" id="dd-' + vRows[i].ID + '">')
                                        .append('<span class="glyphicon glyphicon-option-vertical"></span>')
                                        .unbind('click')
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            $('.dropdown').removeClass('open');
                                            var isOpen = $(this).parent().hasClass('open');

                                            if (CBool(isOpen)) {
                                                $(this).parent().removeClass('open');
                                            } else {
                                                $(this).parent().addClass('open');
                                            }
                                        })
                                    )

                            var options = $('<ul class="dropdown-menu" aria-labelledby="dd-' + vRows[i].ID + '">')

                            options.append($('<li>')
                                        .append($('<a href="#">').data('data', vRows[i]).append('<span class="glyphicon glyphicon-picture" style="color: #f1c40f;"></span> &nbsp; Make as default picture')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                SaveImagePIM(JSON.stringify(data), function () {
                                                    $('.tooltip').remove();
                                                    GetPIMPictures(sessionStorage.ViewID);
                                                    WebToast('Selected as default picture', 'top');                                                    
                                                })
                                            })
                                        )
                                    )
                                    .append($('<li>')
                                        .append($('<a href="#">').data('data', vRows[i]).append('<span class="glyphicon glyphicon-remove" style="color: #c1392b;"></span> &nbsp; Delete')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                NewMessageBox('Are you sure you want to remove this picture?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            data.State = 'remove';
                                                            SaveImagePIM(JSON.stringify(data), function () {
                                                                GetPIMPictures(sessionStorage.ViewID);
                                                                WebToast('Deleted successfully', 'top');                                                                
                                                            })
                                                        }
                                                    }, { Text: 'No' }
                                                ]);
                                            })
                                        )
                                    )

                            dropdown.append(options)

                            if (CBool(vRows[i].IsDefault)) {
                                img.addClass('selected');
                            } else {
                                img.addClass('unselected');
                            }

                            img.data('data', vRows[i]).attr('src', srcMed)
                                .click(function () {
                                    $('#popCanvassSkuPic').modal('show');
                                    var data = $(this).data('data');
                                    carousel.goToSlide(data.Index);
                                    $('.bx-viewport').css('height', '390px');
                                    $('.bxslider > div').css('width', '390px');
                                })

                            td.append($('<div>').append(img)
                                    .append('<br>')
                                    .append(dropdown)
                                )
                            imgSelection.append(td);

                            divPIMPicPreview.append($('<div>')
                                    .append($('<img>')
                                        .data('data', vRows[i]).attr('src', srcBig)
                                    )
                                )

                            //divPIMPicPreviewThumbnails.append($('<img>')
                            //    .data('data', vRows[i]).attr('src', srcMed)
                            //    .click(function () {
                            //        var data = $(this).data('data');
                            //        carousel.goToSlide(data.Index);
                            //    })
                            //)
                            //.append('&nbsp;')
                        }

                        if (carousel) {
                            carousel.destroySlider()
                        }

                        carousel = $('.bxslider').bxSlider({
                            auto: false,
                            autoControls: true,
                            stopAutoOnClick: true,
                            pager: true,
                            slideWidth: 400,
                            adaptiveHeight: true,
                            touchEnabled: true,
                            responsive: true,
                            onSliderLoad: function () {
                                $('.bx-viewport').css('height', '390px');
                                $('.bxslider > div').css('width', '390px');
                            }
                        });                       

                        imgSelection.append(table);
                    } else {
                        imgSelection.append($('<div style="color:#cacaca; text-align:center;">')
                                .append($('<span class="glyphicon glyphicon-picture" style="font-size: 25vh;">'))
                                .append('<br>')
                                .append($('<span style="font-size:30px;">').text('Gallery is empty'))
                            );
                    }
                }
            })
        }

        function SaveImagePIM(data, callback) {
            $.ajax({
                url: "/Organization/SaveImagePIM",
                data: data,
                type: 'POST',
                async: false,
                cache: false,
                processData: false,
                contentType: false,
                success: function (e) {
                    var d = JSON.parse(e);
                    if (d.d.Results == 'saved') {
                        callback()
                    } else {
                        NewMessageBox(d.d.Results);
                    }
                }
            });
        }

        function imageCompressor(source_img_obj, quality, output_format) {
            var mime_type = "image/jpeg";
            if (typeof output_format !== "undefined" && output_format == "png") {
                mime_type = "image/png";
            }

            var cvs = document.createElement('canvas');
            cvs.width = source_img_obj.naturalWidth;
            cvs.height = source_img_obj.naturalHeight;
            var ctx = cvs.getContext("2d").drawImage(source_img_obj, 0, 0);
            var newImageData = cvs.toDataURL(output_format, quality / 100);
            var result_image_obj = new Image();
            result_image_obj.src = newImageData;
            return result_image_obj;

        }

        var tableAttachments;
        function GetFiles() {
            if (CStr2(sessionStorage.SupplierID) == '') {
                return;
            }

            AjaxSend({
                url: 'OrganizationShort/GetFiles',
                data: { ID: sessionStorage.SupplierID, keywords: $.trim($('#txtCanvassAttachmentsSearch').val()) },
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
                        "pageLength": 10,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": true,
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
                                                ClientID: sessionStorage.SupplierID,
                                                FileName: rowData.FileName,
                                                DownloadFileName: rowData.FileName
                                            }, '/OrganizationShort/DownloadFile'), {
                                                prepareCallback: function () {
                                                    WebToast('Preparing your file. Please wait ...', 'top');
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
                        //"aoColumnDefs": [{ "bSortable": true, "aTargets": ['_all'] }],
                        "responsive": true,
                        "order": [[ 2, "asc" ]],
                        //"columnDefs": [{ "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })
                }
            })
        }

        function SecurityValidation(callback) {
            if (!isAccessThroughCanvassLink) {
                callback();
                return;
            }

            $('#popUserLogin').modal('show');

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

                $('#btnCanvassUserLoginSubmit').attr('disabled', 'disabled').text('Submitting ...');

                AjaxSend({
                    url: 'VendorPortal/Login',
                    data: {
                        uname: $.trim($('#txtUsername').val()),
                        pword: $.trim($('#txtPassword').val()),
                        iscanvass: 1,
                        canvasscode: GetUrlParameters().c,
                        OrgID: sessionStorage.OrgID
                    }, success: function (e) {
                        if (e.Results == 'invalid') {
                            $('#errLogin').show();
                        } else {
                            $('#popUserLogin').modal('hide');
                            callback();
                        }

                        $('#btnCanvassUserLoginSubmit').removeAttr('disabled').text('Submit');
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
                        mOut.GetPriceCanvassData(canvassData.ID);
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

        function CashDiscount(dis) {
            var data = dis.data('data');
            var txtCanvassCDRate = $('#txtCanvassCDRate');
            var txtCanvassCDIfPaidIn = $('#txtCanvassCDIfPaidIn');
            var txtCanvassCDNetAmount = $('#txtCanvassCDNetAmount');

            itemData = data;

            $('#popCanvassAdjustPrices').modal('hide');
            $('#popCanvassCashDiscount').modal('show');
            $('#txtCanvassCDRate, #txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').val('');

            if (data.length > 0) {
                txtCanvassCDRate.val(data[0].CashDiscountRate);
                txtCanvassCDIfPaidIn.val(data[0].CashDiscountIfPaidIn);
                txtCanvassCDNetAmount.val(data[0].CashDiscountNetAmountToBePaidIn);
            }

            txtCanvassCDRate.focus();

            $('#btnOkCanvassPriceTradeDiscount').unbind('click').click(function (e) {
                e.preventDefault();

                if (txtCanvassCDRate.val() == '' || txtCanvassCDIfPaidIn.val() == '' || txtCanvassCDNetAmount.val() == '') {
                    WebToast('Please fill the required fields', 'top');
                    return;
                }

                var newData = {
                    CashDiscountRate: CDec2(txtCanvassCDRate.val()),
                    CashDiscountIfPaidIn: CDec2(txtCanvassCDIfPaidIn.val()),
                    CashDiscountNetAmountToBePaidIn: CDec2(txtCanvassCDNetAmount.val())
                }

                if (data.length == 0) {
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
                    dis.val(txtCanvassCDRate.val() + ' % ' + txtCanvassCDIfPaidIn.val() + ', Net ' + txtCanvassCDNetAmount.val());
                }

                $('#popCanvassCashDiscount').modal('hide');

                if (isFromAdjustPrices) {
                    $('#popCanvassAdjustPrices').modal('show');
                }
            })
        }

        return mOut;

    });

    return mDeferred.promise();
})
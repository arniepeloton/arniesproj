define([], function () {
    var mDeferred = $.Deferred(), isLoad = false;
    var mHtml, mMeasurements, arrPriceCanvass, prevDescription, isCanvassDraft = true, IsCreateNewSKU = false, isFirstUploadCanvass = true;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceCanvassing/work/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
                //setTimeout(function () {
                //    $('#tabPriceCanvassWork').empty().append(vData).trigger('create');
                //    Load()
                //}, 1500)
            }
        });

    })();

    CreateStringObject('ERP.PriceCanvassing.work.controller', null, function () {
        var mOut = {};
        var mElement = $('body'), tbodyCanvass, currentCellIndex, isNewCanvass = true, modifiedIndex, canvassID = '', canvassState = 'New';
        var cItemDescription, cItemQty, cItemUom, cName, cNo, cFilterVendor, cShareableLink, cStatus;
        var arrLinkToSku = [], arrVendor = [], arrCanvassItem = [], indVendor, indVendorData = 0;
        var curVendor, curVendorData, curSkuPrices, isPriceCanvassPIMPageLoaded = '';
        var isPopupOpen = false, isFocusOut = false, isValidTradeDisc = false, isFromAdjustPrices = false;
        var txtMobileDescriptionField;

        $('#tabPriceCanvassWork').empty().append(mHtml).trigger('create');        

        setTimeout(function () {
            if (!isLoad) {
                //isLoad = true;
                Load();
                if (sessionStorage.PriceCanvassType == "vendor") {
                    $('.lblPCType').text('Vendor');
                } else {
                    $('.lblPCType').text('Competitor');
                }
            }
        }, 1000)

        mOut.SavePriceCanvass = function (saveas) {
            if ($.trim(cName.val()) == '') {
                NewMessageBox('Please enter canvass name', 'Message');
                $('#tabPCWorkBasicInfo').opentabmenu();
                return
            }
            arrCanvassItem = [];

            tbodyCanvass.find('tr:not(:last)').each(function () {
                var dis = $(this);
                var desc = dis.find('.tdProjProductDescription > div');
                var qty = dis.find('.tdProjProductReqQty > div').text();
                var uomid = dis.find('.tdProjProductUom > div').attr('uomid');
                var frequency = dis.find('.tdProjProductFrequency > div');
                var nettradedisc = dis.find('.tdProjProductNetTradeDiscount > div').text();
                //var srp = dis.find('.tdProjProductSRP > div').text();
                //var tradedisc = dis.find('.tdProjProductTradeDiscount > div').data('data');
                //var cashdisc = dis.find('.tdProjProductCashDiscount > div').data('data');

                arrCanvassItem.push({
                    RowNumber: dis.attr('id'),
                    DBID: (desc.data('data').DBID || desc.attr('dbid')),
                    SkuID: (desc.data('data').ID || ''),
                    State: desc.attr('status'),
                    Description: $.trim(desc.text()),
                    Qty: qty,
                    Uom: uomid,
                    Frequency: frequency.text(),
                    IsOtherFrequency: CInt2(frequency.attr('isotherfreq')),
                    PriceNetOfTradeDiscount: nettradedisc
                    //Price: srp,
                    //TradeDiscount: tradedisc,
                    //CashDiscount: cashdisc
                })
            })

            if (arrCanvassItem.length == 0) {
                NewMessageBox('Please add at least one item');
                return;
            }

            var pData = {
                CanvassID: canvassID,
                Name: $.trim(cName.val()),
                Items: arrCanvassItem,
                NeverExpireLink: $('#chbNeverExpireLink').is(':checked'),
                LinkExpirationDate: $('#txtLinkExpirationDate').val(),
                Vendor: arrVendor,
                State: canvassState,
                SaveAs: saveas,
                Type: sessionStorage.PriceCanvassType,
                IsActive: cStatus.val()
            }

            if (saveas == 'copy') {
                //pag mag SAVE A COPY, ichange item DBID and ItemDBID in Vendor
                var arrNewID = [];

                for (var i = 0; i < pData.Items.length; i++) {
                    var newID = NewID();
                    arrNewID.push({
                        OldID: CStr2(pData.Items[i].DBID),
                        NewID: newID //new DBID
                    })

                    pData.Items[i].DBID = newID; // change the DBID
                }

                for (var i = 0; i < pData.Vendor.length; i++) {
                    for (var m = 0; m < pData.Vendor[i].data.length; m++) {
                        var newID;

                        $.grep(arrNewID, function (e) {
                            if (e.OldID == pData.Vendor[i].data[m].ItemDBID) {
                                pData.Vendor[i].data[m].VendorPriceID = '';
                                pData.Vendor[i].data[m].ItemDBID = e.NewID;
                                return false;
                            }
                        })
                    }
                }
            }

            var arrGroupVendorItemTradeDiscount = [];

            for (var i = 0; i < pData.Vendor.length; i++) {
                var vVendor = pData.Vendor[i];
                for (var m = 0; m < vVendor.data.length; m++) {
                    var ind = null, vItem = vVendor.data[m];
                    var arrTradeDiscount = undefined, arrTradeVolumeDiscount = undefined, arrTradeVolumeCashDiscount = undefined;

                    $.grep(arrGroupVendorItemTradeDiscount, function (e, ii) {
                        if (e.ItemID == vItem.ItemDBID) {
                            ind = ii;
                            return false; // stop loop
                        }
                    })

                    if (CStr2(vItem.PriceNetOfTradeDiscount) != '') {
                        arrTradeDiscount = {
                            VendorIndex: i,
                            VendorDataIndex: m,
                            Price: CDec2(vItem.PriceNetOfTradeDiscount)
                        }
                    }

                    if (CStr2(vItem.PriceNetOfTradeAndVolumeDiscount) != '') {
                        arrTradeVolumeDiscount = {
                            VendorIndex: i,
                            VendorDataIndex: m,
                            Price: CDec2(vItem.PriceNetOfTradeAndVolumeDiscount)
                        }
                    }

                    if (CDec2(vItem.PriceNetOfTradeVolumeAndCashDiscount) != 0) {
                        arrTradeVolumeCashDiscount = {
                            VendorIndex: i,
                            VendorDataIndex: m,
                            Price: CDec2(vItem.PriceNetOfTradeVolumeAndCashDiscount)
                        }
                    }

                    if (ind == null) {
                        var arrDiscount = {
                            ItemID: vItem.ItemDBID
                        }

                        if (arrTradeDiscount != undefined) {
                            arrDiscount.TradeDiscounts = [arrTradeDiscount];
                        }

                        if (arrTradeVolumeDiscount != undefined) {
                            arrDiscount.TradeVolumeDiscounts = [arrTradeVolumeDiscount];
                        }

                        if (arrTradeVolumeCashDiscount != undefined) {
                            arrDiscount.TradeVolumeCashDiscount = [arrTradeVolumeCashDiscount];
                        }

                        arrGroupVendorItemTradeDiscount.push(arrDiscount);
                    } else {
                        if (arrTradeDiscount != undefined) {
                            if (arrGroupVendorItemTradeDiscount[ind].TradeDiscounts == undefined) {
                                arrGroupVendorItemTradeDiscount[ind].TradeDiscounts = [arrTradeDiscount];
                            } else {
                                arrGroupVendorItemTradeDiscount[ind].TradeDiscounts.push(arrTradeDiscount);
                            }
                        }

                        if (arrTradeVolumeDiscount != undefined) {
                            if (arrGroupVendorItemTradeDiscount[ind].TradeVolumeDiscounts == undefined) {
                                arrGroupVendorItemTradeDiscount[ind].TradeVolumeDiscounts = [arrTradeVolumeDiscount];
                            } else {
                                arrGroupVendorItemTradeDiscount[ind].TradeVolumeDiscounts.push(arrTradeVolumeDiscount);
                            }
                        }

                        if (arrTradeVolumeCashDiscount != undefined) {
                            if (arrGroupVendorItemTradeDiscount[ind].TradeVolumeCashDiscount == undefined) {
                                arrGroupVendorItemTradeDiscount[ind].TradeVolumeCashDiscount = [arrTradeVolumeCashDiscount];
                            } else {
                                arrGroupVendorItemTradeDiscount[ind].TradeVolumeCashDiscount.push(arrTradeVolumeCashDiscount);
                            }
                        }
                    }
                }
            }

            if (arrGroupVendorItemTradeDiscount.length > 0) {
                for (var i = 0; i < arrGroupVendorItemTradeDiscount.length; i++) {
                    var tc = [], min = null, vindex = null, vdataind = null;

                    if (arrGroupVendorItemTradeDiscount[i].TradeVolumeDiscounts != undefined) {
                        for (var m = 0; m < arrGroupVendorItemTradeDiscount[i].TradeVolumeDiscounts.length; m++) {
                            tc.push(CDec2(arrGroupVendorItemTradeDiscount[i].TradeVolumeDiscounts[m].Price))
                        }
                    }

                    if (tc.length > 0) {
                        min = Math.min.apply(Math, tc);
                        $.grep(arrGroupVendorItemTradeDiscount[i].TradeVolumeDiscounts, function (e) {
                            if (CDec2(e.Price) == min) {
                                vindex = e.VendorIndex;
                                vdataind = e.VendorDataIndex;
                                return false;
                            }
                        })

                        pData.Vendor[vindex].data[vdataind].IsNewLowestNetTradeAndVolumeDiscount = true;
                        pData.Vendor[vindex].data[vdataind].IsLowestNetTradeAndVolumeDiscount = true;
                    }

                    tc = [], min = null, vindex = null, vdataind = null;

                    if (arrGroupVendorItemTradeDiscount[i].TradeDiscounts != undefined) {
                        for (var m = 0; m < arrGroupVendorItemTradeDiscount[i].TradeDiscounts.length; m++) {
                            tc.push(CDec2(arrGroupVendorItemTradeDiscount[i].TradeDiscounts[m].Price))
                        }
                    }

                    if (tc.length > 0) {
                        min = Math.min.apply(Math, tc);
                        $.grep(arrGroupVendorItemTradeDiscount[i].TradeDiscounts, function (e) {
                            if (CDec2(e.Price) == min) {
                                vindex = e.VendorIndex;
                                vdataind = e.VendorDataIndex;
                                return false;
                            }
                        })

                        pData.Vendor[vindex].data[vdataind].IsNewLowestNetTradeDiscount = true;
                        pData.Vendor[vindex].data[vdataind].IsLowestNetTradeDiscount = true;
                    }

                    tc = [], min = null, vindex = null, vdataind = null;

                    if (arrGroupVendorItemTradeDiscount[i].TradeVolumeCashDiscount != undefined) {
                        for (var m = 0; m < arrGroupVendorItemTradeDiscount[i].TradeVolumeCashDiscount.length; m++) {
                            tc.push(CDec2(arrGroupVendorItemTradeDiscount[i].TradeVolumeCashDiscount[m].Price))
                        }
                    }

                    if (tc.length > 0) {
                        min = Math.min.apply(Math, tc);
                        $.grep(arrGroupVendorItemTradeDiscount[i].TradeVolumeCashDiscount, function (e) {
                            if (CDec2(e.Price) == min) {
                                vindex = e.VendorIndex;
                                vdataind = e.VendorDataIndex;
                                return false;
                            }
                        })

                        pData.Vendor[vindex].data[vdataind].IsNewLowestNetTradeVolumeAndCashDiscount = true;
                        pData.Vendor[vindex].data[vdataind].IsLowestNetTradeVolumeAndCashDiscount = true;
                    }
                }
            }

            //console.log(pData)
            //return;
            AjaxSend({
                url: 'Canvass/SavePriceCanvass',
                data: pData,
                success: function (vData) {
                    if (vData.Results != undefined) {
                        NewMessageBox('ERROR: ' + vData.Results, 'Message')
                    } else {
                        ClearWorkFields();
                        WebToast('Successfully Saved', 'top');
                        mOut.GetPriceCanvass();
                        mOut.DisplayCanvassDetails(JSON.parse(JSON.stringify(vData.data)), false);
                    }
                }
            })
        }

        mOut.DisplayCanvassDetails = function (data, isInputPrice) {
            $('#popListOfCanvass').modal('hide');
            WebToast('Displaying data ...  please wait', 'top');
            ClearWorkFields();
            canvassID = data.CanvassID;
            canvassState = 'Edit';
            cName.val(data.Name);
            cNo.val(data.No);
            cShareableLink.val(location.origin + MergeUrlParameters({
                c: data.CanvassCode
            }, '/Canvass'));
            cStatus.val(CInt2(data.IsActive));
            arrCanvassItem = data.Item;
            arrVendor = data.Vendor;
            if (CBool(data.IsSaveAsFinal)) {
                isCanvassDraft = false;
                $('#lblCanvassSaveAs').text('FINALIZED');
            }

            if (data.NeverExpireLink) {
                $('#chbNeverExpireLink').attr('checked', 'checked')//.checkboxradio('refresh');
            } else {
                $('#chbNeverExpireLink').removeAttr('checked')//.checkboxradio('refresh');

                if (data.LinkExpirationDate != '') {
                    $('#txtLinkExpirationDate').val($.format.date(new Date(data.LinkExpirationDate), 'yyyy-MM-dd'));
                    $('#divExpirationDate').show();
                }
            }

            setTimeout(function () {
                AddCanvassItem();
                //AddCanvassItemForInputPrice();
                for (var i = 0; i < arrVendor.length; i++) {
                    AddVendorToFilter(arrVendor[i]);
                    $('#divCanvassVendorLink').append($('<div class="mb-3">')
                        .attr('id', 'divlinkvendor-' + arrVendor[i].DBID)
                        .append($('<div class="row align-items-center">')
                            .append($('<div class="col-sm-9">').append((CBool(arrVendor[i].IsOrganization) ? arrVendor[i].CompanyName : $.trim(arrVendor[i].FirstName + ' ' + arrVendor[i].MiddleName + ' ' + arrVendor[i].Surname))))
                            .append($('<div class="col-sm-3">')
                                .append($('<button data-role="none" class="btn btn-light border">')
                                    .data('data', arrVendor[i])
                                    .text('Show Link')
                                    .click(function () {
                                        if ($(this).text() == 'Show Link') {
                                            $(this).text('Hide Link');
                                            $('#divlink-' + $(this).data('data').DBID).show();
                                        } else {
                                            $(this).text('Show Link');
                                            $('#divlink-' + $(this).data('data').DBID).hide();
                                        }
                                    })
                                )
                            )
                        )
                        .append($('<div style="display:none;">')
                            .attr('id', 'divlink-' + arrVendor[i].DBID)
                            .append('<br>')
                            .append($('<div class="input-group">')
                                .append($('<input data-role="none" type="text" class="form-control" readonly>')
                                    .attr('id', 'vendor-' + arrVendor[i].DBID)
                                    .val(location.origin + MergeUrlParameters({
                                        v: arrVendor[i].CanvassVendorCode,
                                        c: data.CanvassCode
                                    }, '/Canvass'))
                                )
                                .append($('<button data-role="none" class="btn btn-secondary">')
                                    .data('data', arrVendor[i])
                                    .text('Copy Link')
                                    .click(function () {
                                        CopyToClipboard('#vendor-' + $(this).data('data').DBID);
                                    })
                                )
                            )
                            //.append('<br>')
                            //.append($('<div class="divTable" style="font-size: inherit !important; width: auto !important;">')
                            //    .append($('<div class="divTableRow">')
                            //        .append($('<div class="divTableCell" style="width:115px;">')
                            //            .append($('<div class="custom-checkbox">')
                            //                .append($('<label class="checkbox-only">')
                            //                    .append($('<input type="checkbox" class="chbLinkActive" data-iconpos="notext" value="1" />')
                            //                        .data('data', arrVendor[i])
                            //                        .attr('checked', CBool(arrVendor[i].IsActiveLink))
                            //                        .change(function () {
                            //                            $(this).data('data').IsActiveLink = $(this).is(':checked');
                            //                        })
                            //                    )
                            //                )
                            //                .append($('<div>').text('Active'))
                            //            )
                            //        )
                            //        .append($('<div class="divTableCell" style="vertical-align:top;">')
                            //            .append($('<div style="display:inline-flex">')
                            //                .append('Link will expire on: ')
                            //                .append($('<input type="date" onkeydown="return false" data-role="none" class="form-control" />')
                            //                    .data('data', arrVendor[i])
                            //                    .attr('min', $.format.date(new Date(), 'yyyy-MM-dd'))
                            //                    .attr('value', $.format.date(new Date((CStr2(arrVendor[i].EndDate) == '' ? new Date().setDate(new Date().getDate() + 3) : CStr2(arrVendor[i].EndDate))), 'yyyy-MM-dd'))
                            //                    .change(function () {
                            //                        $(this).data('data').EndDate = $(this).val();
                            //                    })
                            //                )
                            //            )
                            //        )
                            //    )
                            //)
                            .append('<hr>')
                        )
                    )
                }
                $('.chbLinkActive')//.checkboxradio()//.checkboxradio('refresh');
                DisplayInputPrices();
                PopulatePricesOfCurrentSelectedVendor();
                VendorPriceComparison();
                $('#divCanvassSku').show();

                if (CBool(isInputPrice)) {
                    $('#tabPCWorkVendor').opentabmenu();
                    $('.thProductInfo').hide();
                    $('.thVendor').show();
                }
            }, 1000);
        }

        mOut.LoadWorkPage = function () {
            $('#tabPriceCanvassWork').empty().append(mHtml).trigger('create');
        }

        mOut.DisplayPage = function () {
            ClearWorkFields();
            //LoadMeasurements();
        }

        mOut.GetPriceCanvass = function (s) {
            LoadPriceCanvass(s)
            return
            AjaxSend({
                url: 'Canvass/GetPriceCanvass',
                data: { keywords: CStr2(s), type: sessionStorage.PriceCanvassType, IsActive: mElement.find('[name="selCanvassListStatus"]').val() },
                success: function (vData) {
                    if (typeof vData.data == 'object') {
                        arrPriceCanvass = vData.data;
                        LoadPriceCanvass();
                    } else {
                        NewMessageBox(vData.data, 'Message');
                    }
                }
            })
        }

        mOut.GetPriceCanvass();

        mOut.LoadPriceCanvass = function () {
            LoadPriceCanvass();
        }

        mOut.LoadPage = function () {
            $('#tabPriceCanvassWork').empty().append(mHtml).find('[data-role="popup"]').remove().trigger('create');
            mElement = $('body');

            mElement.find('#tabPriceCanvassWork *').undelegate();

            if (sessionStorage.PriceCanvassType == "vendor") {
                $('.lblPCType').text('Vendor');
            } else {
                $('.lblPCType').text('Competitor');
            }

            Load();
            mOut.GetPriceCanvass();
        }

        window.PriceCanvass = mOut;

        //mElement = $(mHtml);
        if (mHtml == undefined) {
            mOut.Init();
        } else {
            //$('#tabPriceCanvassWork').empty().append(mHtml).trigger('create');
            //$('#popVendorListEdit').popup(); // init popup
            //$('#popPriceCanvassSearchVendor').popup();
        }

        function Load() {
            mOut.element = mElement;
            tbodyCanvass = mElement.find('#tbodyCanvass');
            cItemDescription = mElement.find('[name="txtCanvassItemDescription"]');
            cItemQty = mElement.find('[name="txtCanvassItemQty"]');
            cItemUom = mElement.find('[name="selCanvassMeasurement"]');
            cName = mElement.find('[name="txtCanvassName"]');
            cNo = mElement.find('[name="txtCanvassNo"]');
            cStatus = mElement.find('[name="selCanvassStatus"]');
            cFilterVendor = mElement.find('[name="selCanvassVendor"]');
            txtMobileDescriptionField = $('#txtMobileDescriptionField');
            cShareableLink = mElement.find('#txtCanvassShareLink');

            ClearWorkFields();

            mElement.find('#tbodyCanvass').sortable({
                list: '#tbodyCanvass',
                handle: '.tddraggable',
                delay: 150,
                start: function (event, ui) {
                    isOnDrag = true;
                    ui.item.css('border', '1px solid #CCC');
                    $('#tbodyCanvass tr:last').hide();
                },
                stop: function (event, ui) {
                    isOnDrag = false;
                    ui.item.css('border', '1px solid #FFF');
                    $('#tbodyCanvass tr:last').show();
                },
                update: function (event, ui) {
                    UpdateItemNumber();
                }
            });

            mElement.find('#divCanvassSku').attachDragger();

            try { // check if using mobile
                document.createEvent("TouchEvent");
                $('#formMobileDescriptionField').show();
            }
            catch (e) {
                $('#formMobileDescriptionField').hide();
            }

            mElement.off('change', '[name="selCanvassListStatus"]').on('change', '[name="selCanvassListStatus"]', function () {
                mOut.GetPriceCanvass();
            })

            mElement.off('click', '[name="butPriceCanvassNew"]').on('click', '[name="butPriceCanvassNew"]', function () {
                var isEmpty = true;

                tbodyCanvass.find('tr:not(:last)').each(function () {
                    var dis = $(this);
                    var desc = dis.find('.tdProjProductDescription > div');
                    if ($.trim(desc.text()) != '') {
                        isEmpty = false;
                        return false;
                    }
                })
                if (CStr2(canvassID) != '' || !isEmpty) {
                    NewMessageBox('Are you sure to want to create a new transaction? Any unsaved data will be lost.', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                CreateNewCanvass();
                            }
                        }, { Text: 'No' }
                    ])
                } else {
                    CreateNewCanvass()
                }
            })

            mElement.off('click', '[name="butPriceCanvassLookUp"]').on('click', '[name="butPriceCanvassLookUp"]', function () {
                $('#popListOfCanvass').modal('show');
            })

            mElement.off('click', '[name="butAddCanvassItem"]').on('click', '[name="butAddCanvassItem"]', function (e) {
                e.stopPropagation();
                e.preventDefault();
                if (ValidateCanvass()) {
                    return;
                }

                BrowsePopup('LibraryPIMProduct', 'Select Product', [
                    {
                        SelectMultiple: true,
                        DisplayText: [{ Label: 'Product No:', Column: 'ProductNo' }, { Column: 'ProductName' }],
                        Click: function (e) {
                            $('#tbodyCanvass tr:last').remove();

                            for (var i = 0; i < e.length; i++) {
                                if (tbodyCanvass.find('.tdProjProductDescription > div[skuid="' + e[i].ID + '"]').length > 0) {
                                    continue;
                                }
                                e[i].DBID = NewID();
                                e[i].State = 'New';
                                AddNewRow(e[i]);
                            }

                            AddNewRow();
                        }
                    }
                ])
                return;
                ClearCanvassItemsField();
                $('#popCanvassItem').show();
                cItemDescription.focus();
                isNewCanvass = true;
            })

            mElement.off('click', '[name="btnLinkToSku"]').on('click', '[name="btnLinkToSku"]', function (e) {
                e.stopPropagation();
                e.preventDefault();

                BrowsePopup('LibraryPIMProduct', 'Search SKU', [
                    {
                        DisplayText: [{
                            Label: 'No.', Column: 'ProductNo', Style: {
                                width: '100px'
                            }
                        }, {
                            Label: 'Description:', Column: 'ProductName', Style: {
                                maxWidth: '300px'
                            }
                        }],
                        Click: function (pInfo) {
                            for (var i = 0; i < pInfo.length; i++) {
                                if (arrLinkToSku.length == 0) {
                                    PushToListSku(pInfo[i])
                                } else {
                                    var isExist = false;
                                    for (var m = 0; m < arrLinkToSku.length; m++) {
                                        if (pInfo[i].ID == arrLinkToSku[m].ID) {
                                            isExist = true;
                                        }
                                    }

                                    if (!isExist) {
                                        PushToListSku(pInfo[i])
                                    }
                                }
                            }
                            DisplayLinkSku();
                        },
                        StyleFormat: 'table',
                        SelectMultiple: true
                    }
                ])
            })

            mElement.off('click', '[name="btnVendorOfItem"]').on('click', '[name="btnVendorOfItem"]', function (e) {
                e.stopPropagation();
                e.preventDefault();
                var title = sessionStorage.PriceCanvassType.charAt(0).toUpperCase() + '' + sessionStorage.PriceCanvassType.slice(1);
                var url, arrSelectedVendor = [];                

                BrowsePopup(function () {
                    if (sessionStorage.PriceCanvassType == 'competitor' && CBool($('[name="radPriceCanvassVendor"]:checked').val())) {
                        url = 'LibraryOrganizationCustomerAndSupplier';
                    } else {
                        url = 'LibraryOrganization';
                    }

                    return url;
                }, 'Search ' + title, [{
                    Options: {
                        Fields: [
                            $('<tr>')
                                .append($('<td>')
                                    .append('Search in: ')
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Company\'s ' + sessionStorage.PriceCanvassType + ' list')
                                            .append($('<input type="radio" name="radPriceCanvassVendor" value="1" class="form-check-input" checked>')
                                                .change(function () {
                                                    WebToast('Cleared all selected vendors', 'top');
                                                    arrSelectedVendor = [];
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Public ' + sessionStorage.PriceCanvassType + ' list')
                                            .append($('<input type="radio" name="radPriceCanvassVendor" value="0" class="form-check-input">')
                                                .change(function () {
                                                    WebToast('Cleared all selected vendors', 'top');
                                                    arrSelectedVendor = [];
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )
                        ],
                        OnChange: function () {                            
                            return {
                                FilterSupplier: $('[name="radPriceCanvassVendor"]:checked').val()
                            }
                        }
                    },
                    SelectMultiple: true,
                    DisplayText: [{ Column: 'CompanyName' }],
                    Validation: function (e){
                        if (e.length == 0) {
                            WebToast('Please select at least one vendor', 'top');
                            return false;
                        }

                        return true;
                    },
                    Click: function (e) {
                        arrSelectedVendor = e;
                        for (var i = 0; i < arrSelectedVendor.length; i++) {
                            arrSelectedVendor[i].State = 'New';
                            arrSelectedVendor[i].SkuPrices = [];
                            arrSelectedVendor[i].IsFromPrivateVendor = $('[name="radPriceCanvassVendor"]:checked').val();
                            arrSelectedVendor[i].data = [];
                            if (arrVendor.length == 0) {
                                arrVendor.push(arrSelectedVendor[i]);
                                AddVendorToFilter(arrSelectedVendor[i]);
                                DisplayInputPrices(arrSelectedVendor[i]);
                            } else {
                                var isExist = false;
                                for (var m = 0; m < arrVendor.length; m++) {
                                    if (arrSelectedVendor[i].ID == arrVendor[m].ID) {
                                        isExist = true;
                                    }
                                }

                                if (!isExist) {
                                    arrVendor.push(arrSelectedVendor[i]);
                                    AddVendorToFilter(arrSelectedVendor[i]);
                                    DisplayInputPrices(arrSelectedVendor[i]);
                                }
                            }
                        }
                        PopulatePricesOfCurrentSelectedVendor();
                    }
                }])
            })

            mElement.off('click', '[name="btnOkCanvassItem"]').on('click', '[name="btnOkCanvassItem"]', function (e) {
                e.stopPropagation();
                e.preventDefault();
                var cDescription = $.trim(cItemDescription.val());
                var cQty = $.trim(cItemQty.val());
                var cUom = $.trim(cItemUom.val());
                var cUomName = $.trim(cItemUom.find('option:selected').text());

                if (cDescription == '') {
                    WebToast('Please enter description', 'top');
                    return;
                }

                var pData = {};
                if (isNewCanvass) {
                    pData.ItemID = '';
                    pData.Description = cDescription;
                    pData.Qty = cQty;
                    pData.Uom = cUom;
                    pData.UomName = cUomName;
                    pData.Sku = arrLinkToSku;
                    pData.State = 'New';
                    arrCanvassItem.push(pData);
                } else {
                    arrCanvassItem[CInt2(modifiedIndex)].Description = cDescription;
                    arrCanvassItem[CInt2(modifiedIndex)].Qty = cQty;
                    arrCanvassItem[CInt2(modifiedIndex)].Uom = cUom;
                    arrCanvassItem[CInt2(modifiedIndex)].UomName = cUomName;
                    arrCanvassItem[CInt2(modifiedIndex)].Sku = arrLinkToSku;
                }
                AddCanvassItem();
                AddCanvassItemForInputPrice();
                PopulatePricesOfCurrentSelectedVendor();
                $('#popCanvassItem').hide();
            })

            mElement.off('click', '[name="butPriceCanvassAdjustPrices"]').on('click', '[name="butPriceCanvassAdjustPrices"]', function () {
                if (ValidateCanvass()) {
                    return;
                }

                if ($('#selAdjustPricesVendor option').length == 0) {
                    NewMessageBox('No vendor found. Please add at least one vendor', 'Message');
                    return;
                }

                if (arrCanvassItem.length == 0) {
                    NewMessageBox('No item found. Please add at least one item', 'Message');
                    return;
                }

                $('#popCanvassAdjustPrices').modal('show');
                isFromAdjustPrices = true;
                $('#selAdjustPricesVendor').val($('[name="selCanvassVendor"] option:selected').val());
                $('#selAdjustPricesCategory').val($('[name="selFilterCanvassCategory"] option:selected').val());
                $('#selAdjustPricesBrand').val($('[name="selFilterCanvassBrand"] option:selected').val());
                $('#txtAdjustPricesTradeDiscounts, #txtAdjustPricesCashDiscounts, #txtAdjustPricesVolumeDiscount').val('');
                $('#txtAdjustPricesCashDiscounts').data('data', []);
            })

            mElement.off('click', '[name="butPriceCanvassSaveAsDraft"]').on('click', '[name="butPriceCanvassSaveAsDraft"]', function () {
                if (ValidateCanvass()) {
                    return;
                }
                if (!isCanvassDraft) {
                    NewMessageBox('You can\'t save finalized canvass as draft.', 'Message');
                } else {
                    mOut.SavePriceCanvass('draft');
                }
            })

            mElement.off('click', '[name="butPriceCanvassSaveAsFinal"]').on('click', '[name="butPriceCanvassSaveAsFinal"]', function () {
                NewMessageBox('Are you sure you want to save and finalize this transaction? ' +
                    '<br><br> If Yes, after “Saving as Final”, you:<br><ul>' +
                        '<li>Can no longer edit descriptions for this canvass form</li>' +
                        '<li>Can no longer edit linked SKUs to descriptions for this canvass form</li>' +
                        '<li>Can still add new vendor to vendor lists for this canvass form</li>' +
                        '<li>Can still add/ edit vendor price for this canvass form</li>' +
                        '<li>Can link SKUs to item descriptions that have no SKU link (security access required)</li>' +
                    '</ul>' +
                    'Only finalized transactions can be shared.', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                mOut.SavePriceCanvass('final');
                            }
                        }, { Text: 'No' }
                    ])
            })

            mElement.off('click', '[name="butPriceCanvassSaveACopy"]').on('click', '[name="butPriceCanvassSaveACopy"]', function () {
                if (isCanvassDraft) {
                    NewMessageBox('Only finalized transactions can be copied', 'Message');
                    return;
                }

                mOut.SavePriceCanvass('copy');
            })

            mElement.off('click', '[name="butPriceCanvassShare"]').on('click', '[name="butPriceCanvassShare"]', function () {
                $('#popCanvassShare').modal('show');
            })

            mElement.off('change', '#chbNeverExpireLink').on('change', '#chbNeverExpireLink', function () {
                if ($(this).is(':checked')) {
                    $('#divExpirationDate').hide();
                } else {
                    $('#divExpirationDate').show();
                }
            })

            mElement.off('click', '#btnDownloadCanvassVendor').on('click', '#btnDownloadCanvassVendor', function () {
                if (arrVendor.length == 0) {
                    WebToast('No vendor found.', 'top');
                    return;
                }

                location.href = GetPathFormat('/Canvass/DownloadCanvassVendor?OrgID=' + sessionStorage.OrgID + '&cid=' + canvassID);
            })

            mElement.off('click', '#btnCopyCanvassLink').on('click', '#btnCopyCanvassLink', function () {
                CopyToClipboard('#txtCanvassShareLink');
            })

            mElement.off('click', '#btnTabWorkVendor').on('click', '#btnTabWorkVendor', function () {
                DisplayInputPrices();
                $('.thProductInfo').hide();
                $('.thVendor').show();
            })

            mElement.off('click', '#btnVendorListEdit').on('click', '#btnVendorListEdit', function () {
                var listVendorEdit = $('#listVendorEdit');
                listVendorEdit.empty();
                for (var i = 0; i < arrVendor.length; i++) {
                    arrVendor[i].arrIndex = i;
                    if (arrVendor[i].State != 'Remove') {
                        listVendorEdit.append($('<li class="list-group-item d-flex align-items-center">')
                            .append($('<p class="p-0 m-0 flex-grow-1">')
                                .text((CBool(arrVendor[i].IsOrganization) ? arrVendor[i].CompanyName : $.trim(CStr2(arrVendor[i].FirstName) + ' ' + CStr2(arrVendor[i].MiddleName) + ' ' + CStr2(arrVendor[i].Surname))))
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', arrVendor[i])
                                .click(function (e) {
                                    var dis = $(this);
                                    var data = JSON.parse(JSON.stringify(dis.data('data')));
                                    var id = data.ID;
                                    NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                if (data.State == 'New') {
                                                    arrVendor = $.grep(arrVendor, function (val) {
                                                        return val.ID != id;
                                                    })
                                                }
                                                else {
                                                    arrVendor[data.arrIndex].State = "Remove";
                                                }
                                                dis.parent().remove();
                                                $('[name="selCanvassVendor"]').find('option[value="' + id + '"]').remove()
                                                $('#selAdjustPricesVendor').find('option[value="' + id + '"]').remove()
                                                $('#divlinkvendor-' + data.DBID).remove();
                                                DisplayInputPrices();
                                                PopulatePricesOfCurrentSelectedVendor();
                                                $('#popVendorListEdit').modal((listVendorEdit.find('li').length > 0 ? 'show' : 'hide'));
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                        )
                    }
                }

                //listVendorEdit.listview().listview('refresh');
                $('#popVendorListEdit').modal('show');
            })

            mElement.off('click', '#btnTabWorkCanvassProductList').on('click', '#btnTabWorkCanvassProductList', function () {
                $('#divCanvassSku').show();
                $('.thProductInfo').show();
                $('.thVendor').hide();
            })

            mElement.off('click', '#btnTabWorkVendorPriceComparison').on('click', '#btnTabWorkVendorPriceComparison', function () {
                $('#divCanvassSku').hide();
            })

            mElement.off('click', '#btnOkCanvassFrequency').on('click', '#btnOkCanvassFrequency', function () {
                var activeRow = tbodyCanvass.find('tr.activeRow');
                var freq = activeRow.find('.tdProjProductFrequency > div');
                var selCanvassFrequency = $.trim($('#selCanvassFrequency').val());
                var txtCanvassFreqOthers = $.trim($('#txtCanvassFreqOthers').val());

                //if (selCanvassFrequency == '') {
                //    WebToast('Please select frequency', 'top');
                //    return;
                //}

                if (selCanvassFrequency == 'Others') {
                    if (txtCanvassFreqOthers == '') {
                        WebToast('Please select other frequency', 'top');
                        return;
                    }

                    freq.attr('isotherfreq', 1).text(txtCanvassFreqOthers);
                } else {
                    freq.attr('isotherfreq', 0).text(selCanvassFrequency);
                }

                $('#popCanvassFrequency').modal('hide');
            })

            mElement.off('change', '#selCanvassFrequency').on('change', '#selCanvassFrequency', function () {
                if ($(this).val() == 'Others') {
                    $('#txtCanvassFreqOthers').show().focus();
                } else {
                    $('#txtCanvassFreqOthers').hide();
                }
            })

            mElement.off('click', '#txtTradeDiscountLess, #txtAdjustPricesTradeDiscounts').on('keydown', '#txtTradeDiscountLess, #txtAdjustPricesTradeDiscounts', function (e) {
                var kCode = e.keyCode;
                // allow only number, comma, backspace, tab, enter, period, paste, undo
                if (((kCode < 48 || kCode > 57) && (kCode < 96 || kCode > 105) && (kCode != 188 && kCode != 8 && kCode != 9 && kCode != 13 && kCode != 190 && kCode != 110) && (kCode < 37 || kCode > 40))
                    || (e.ctrlKey == true && (kCode != 86 && kCode != 90))) {
                    e.preventDefault();
                }
            }).on('keyup', '#txtTradeDiscountLess, #txtAdjustPricesTradeDiscounts', function () {
                TradeDiscount($(this).val());
            })

            mElement.off('click', '#txtAdjustPricesCashDiscounts').on('click', '#txtAdjustPricesCashDiscounts', function () {
                PopupCashDiscounts($('#txtAdjustPricesCashDiscounts').data('data'));
            })

            mElement.off('click', '#btnOkCanvassTradeDiscount').on('click', '#btnOkCanvassTradeDiscount', function () {
                var txtTradeDiscountLess = $.trim($('#txtTradeDiscountLess').val());
                var txtTradeDiscountRemarks = $.trim($('#txtTradeDiscountRemarks').val());

                if (txtTradeDiscountLess == '' || !isValidTradeDisc) {
                    WebToast('Please enter valid trade discount', 'top');
                    return;
                }

                var div = $('.activeCell > div');
                var data = div.data('data');

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
                    div.text(txtTradeDiscountLess + ' (' + TradeDiscount(txtTradeDiscountLess).NetRate + '% NDR)')
                } else {
                    div.text('');
                }

                $('#popCanvassTradeDiscount').modal('hide');
            })

            mElement.find('#btnOkCanvassCashDiscount').unbind('click').click(function (e) {
                e.preventDefault();

                var txtCanvassCDRate = $.trim($('#txtCanvassCDRate').val());
                var txtCanvassCDIfPaidIn = $.trim($('#txtCanvassCDIfPaidIn').val());
                var txtCanvassCDNetAmount = $.trim($('#txtCanvassCDNetAmount').val());
                var cd = txtCanvassCDRate + ' % ' + txtCanvassCDIfPaidIn + ', Net ' + txtCanvassCDNetAmount

                if (txtCanvassCDRate == '' || txtCanvassCDIfPaidIn == '' || txtCanvassCDNetAmount == '') {
                    WebToast('Please fill the required fields', 'top');
                    return;
                }

                $('#popCanvassCashDiscount').modal('hide');

                if (isFromAdjustPrices) {
                    var apcd = $('#txtAdjustPricesCashDiscounts');

                    if (CDec2(txtCanvassCDRate) != 0) {
                        apcd.data('data', [
                            {
                                CashDiscountRate: txtCanvassCDRate,
                                CashDiscountIfPaidIn: txtCanvassCDIfPaidIn,
                                CashDiscountNetAmountToBePaidIn: txtCanvassCDNetAmount
                            }
                        ]).val(cd);
                    } else {
                        apcd.data('data', []).val('');
                    }

                    return;
                }

                var div = $('.activeCell > div');
                var data = div.data('data');
                isFromAdjustPrices = false;

                if (data.length == 0) {
                    data.push({
                        CashDiscountRate: txtCanvassCDRate,
                        CashDiscountIfPaidIn: txtCanvassCDIfPaidIn,
                        CashDiscountNetAmountToBePaidIn: txtCanvassCDNetAmount
                    })
                } else {
                    data[0].CashDiscountRate = txtCanvassCDRate;
                    data[0].CashDiscountIfPaidIn = txtCanvassCDIfPaidIn;
                    data[0].CashDiscountNetAmountToBePaidIn = txtCanvassCDNetAmount;
                }

                PopulateVendorData('cashdiscount', {
                    CashDiscountRate: txtCanvassCDRate,
                    CashDiscountIfPaidIn: txtCanvassCDIfPaidIn,
                    CashDiscountNetAmountToBePaidIn: txtCanvassCDNetAmount
                })

                if (CDec2(txtCanvassCDRate) != 0) {
                    div.text(cd);
                }
            })

            mElement.off('click', '#btnRefreshPriceCanvass').on('click', '#btnRefreshPriceCanvass', function () {
                mOut.GetPriceCanvass();
            })

            mElement.off('keypress', '#txtSearchPriceCanvass').on('keypress', '#txtSearchPriceCanvass', function (e) {
                if (e.which == 13) {
                    mOut.GetPriceCanvass($.trim($(this).val()));
                }
            })

            mElement.off('keydown keyup', '#txtCanvassCDRate').on('keydown', '#txtCanvassCDRate', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            }).on('keyup', '#txtCanvassCDRate', function () {
                $(this).val($(this).val().replace('%', ''))
            })

            mElement.off('keydown', '#txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').on('keydown', '#txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })

            mElement.off('click', '[name="butPriceCanvassUpload"]').on('click', '[name="butPriceCanvassUpload"]', function () {
                $('#popCanvassUpload').modal('show');
                $('#fileCanvass').val('');
            })

            mElement.off('click', '#fileCanvass').on('click', '#fileCanvass', function () {
                $(this).val('');
            })

            mElement.off('click', '#btnDownloadCanvassTemplate').on('click', '#btnDownloadCanvassTemplate', function () {
                location.href = GetPathFormat('/Canvass/DownloadCanvassTemplate');
            })

            mElement.find('#formUploadCanvass').unbind('submit').submit(function (e) {
                e.stopPropagation();
                e.preventDefault();

                //if (ValidateCanvass()) {
                //    return;
                //}

                if ($.trim(cName.val()) == '') {
                    WebToast('Please enter canvass name', 'top');
                    return;
                }

                if (CStr2($('#fileCanvass').val()) == '') {
                    WebToast('Please select canvass template', 'top');
                    return;
                }

                var pData = {
                    CanvassID: canvassID,
                    Name: $.trim(cName.val()),
                    State: canvassState,
                    IsCreateNewSKU: IsCreateNewSKU,
                    IsFirstUploadCanvass: isFirstUploadCanvass,
                    IsFinalize: !isCanvassDraft,
                    Type: sessionStorage.PriceCanvassType
                }
                var formdata = new FormData();
                var fileUpload = $('#fileCanvass').get(0);
                var files = fileUpload.files;
                formdata.append('data', JSON.stringify(pData));
                formdata.append(files[0].name, files[0]);

                $('#btnCanvassUpload').attr('disabled', true).text('Uploading');
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
                        $('#btnCanvassUpload').removeAttr('disabled').text('Upload');

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

            mElement.off('click', '#btnCanvassUpload').on('click', '#btnCanvassUpload', function () {
                mElement.find('#formUploadCanvass').submit();
            })

            mElement.off('change', '[name="selFilterCanvassCategory"], [name="selFilterCanvassBrand"]').on('change', '[name="selFilterCanvassCategory"], [name="selFilterCanvassBrand"]', function () {
                FilterList();
            })

            mElement.off('click', '#butPriceCanvassDownloadPIM').on('click', '#butPriceCanvassDownloadPIM', function () {
                BrowsePopup('PimCategories', 'Categories', [
                    {
                        Message: 'Please select categories that you want to canvass using excel template. Then click OK button to download the excel template.',
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Name' }],
                        Click: function (e) {
                            window.location = GetPathFormat("/Canvass/DownloadPIMByCategories?OrgID=" + sessionStorage["OrgID"] + "&categories=" + JSON.stringify(e));
                        },
                        SelectMultiple: true
                    }
                ], { IsAll: true })
            })

            mElement.off('click', '#btnPriceCanvassUploadPIM').on('click', '#btnPriceCanvassUploadPIM', function () {
                sessionStorage.IsUploadPIMViaPriceCanvass = true;
                NewMessageBoxClose();
                AddBreadCrumb({
                    text: 'Upload PIM Template',
                    Click: function () {
                        $('#divPriceCanvassUploadPIMPage').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                $('#divPriceCanvassUploadPIMPage').pageshow();

                if (isPriceCanvassPIMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMUpload?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPriceCanvassPIMPageLoaded = vData;
                            $('#UniversalDiv, #divPriceCanvassUploadPIMPage').empty();
                            $('#divPriceCanvassUploadPIMPage').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv, #divPriceCanvassUploadPIMPage').empty();
                    $('#divPriceCanvassUploadPIMPage').append(isPriceCanvassPIMPageLoaded).trigger('create');
                }
            })

            mElement.off('click', '#btnAdjustPricesOk').on('click', '#btnAdjustPricesOk', function () {
                var selAdjustPricesCategory = $('#selAdjustPricesCategory').val();
                var selAdjustPricesBrand = $('#selAdjustPricesBrand').val();
                var apTradeDiscounts = $.trim($('#txtAdjustPricesTradeDiscounts').val());
                var apCashDiscounts = $('#txtAdjustPricesCashDiscounts').data('data');
                var trow;

                if (selAdjustPricesCategory == '' && selAdjustPricesBrand == '') {
                    trow = tbodyCanvass.find('tr:not(:last)');
                } else if (selAdjustPricesCategory != '' || selAdjustPricesBrand != '') {
                    trow = tbodyCanvass.find('tr:contains(' + selAdjustPricesCategory + selAdjustPricesBrand + ')')
                } else if (selAdjustPricesCategory != '' && selAdjustPricesBrand != '') {
                    trow = tbodyCanvass.find('tr:contains(' + selAdjustPricesCategory + '):contains(' + selAdjustPricesBrand + ')')
                }

                if (apTradeDiscounts == '' && apCashDiscounts.length == 0) {
                    WebToast('Please enter at least one discount', 'top');
                    return;
                }

                trow.each(function () {
                    var dis = $(this);
                    var td, tddata;

                    tbodyCanvass.find('tr').removeClass('activeRow');
                    dis.addClass('activeRow');

                    if (apTradeDiscounts != '') {
                        td = dis.find('.tdProjProductTradeDiscount > div');
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
                        td.text(apTradeDiscounts + ' (' + TradeDiscount(apTradeDiscounts).NetRate + '% NDR)');
                    }

                    if (apCashDiscounts.length > 0) {
                        var cdRate = apCashDiscounts[0].CashDiscountRate, cdifpaid = apCashDiscounts[0].CashDiscountIfPaidIn,
                            cdnet = apCashDiscounts[0].CashDiscountNetAmountToBePaidIn;
                        td = dis.find('.tdProjProductCashDiscount  > div');
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
                        td.text(cdRate + ' % ' + cdifpaid + ', Net ' + cdnet);
                    }
                })

                $('#popCanvassAdjustPrices').modal('hide');
            })

            mElement.off('change', '#selAdjustPricesVendor').on('change', '#selAdjustPricesVendor', function () {
                cFilterVendor.val($(this).val());
                PopulatePricesOfCurrentSelectedVendor();
            })

            mElement.off('click', '#btnCanvassSearchClear').on('click', '#btnCanvassSearchClear', function () {
                $('#txtCanvassSearch').val('');
                tbodyCanvass.find('tr').show();
            })

            mElement.off('keyup', '#txtCanvassSearch').on('keyup', '#txtCanvassSearch', function () {
                var val = $.trim($(this).val());

                tbodyCanvass.find('tr').show();
                if (val != '') {
                    tbodyCanvass.find('.tdProjProductDescription').each(function () {
                        var desc = $.trim($(this).text());

                        if (desc.toLowerCase().indexOf(val) < 0) {
                            $(this).parent().hide();
                        }
                    })
                }
            })

            mElement.off('change', '[name="selCanvassVendor"]').on('change', '[name="selCanvassVendor"]', function (e) {
                PopulatePricesOfCurrentSelectedVendor();
            })

            mElement.find('#btnCanvassCommentOk').unbind('click').click(function (e) {
                e.preventDefault();
                var comment = $.trim($('#txtCanvassComment').val());
                tbodyCanvass.find('td.activeCell > div').html((comment.length > 50 ? comment.substring(0, 50) + '... (see more)' : comment));
                PopulateVendorData('comment', {
                    VendorComment: comment
                })
                $('#popCanvassComment').modal('hide');
            })

            mElement.off('keydown', '#txtMobileDescriptionField').on('keydown', '#txtMobileDescriptionField', function (e) {
                if ($(this).attr('datatype') == 'numeric') {
                    if (!NumberDotOnly($(this), e)) {
                        return;
                    }
                }
            })

            mElement.find('#formMobileDescriptionField').unbind('submit').submit(function (e) {
                e.preventDefault();

                if (ValidateCanvass()) {
                    return;
                }

                var col = txtMobileDescriptionField.attr('col');
                var tdActiveCell = $('#tbodyCanvass tr td.activeCell');
                tdActiveCell.find('div').text(txtMobileDescriptionField.val());

                if (col == 'srp') {
                    PopulateVendorData('srp', {
                        Price: txtMobileDescriptionField.val()
                    })
                }

                if ($.trim($('#tbodyCanvass tr:last').find('td.tdProjProductDescription').text()) != "") {
                    AddNewRow();
                }
            })

            mElement.off('contextmenu', '.tdProjProductDescription').on('contextmenu', '.tdProjProductDescription', function (e) {
                e.preventDefault();
                e.stopPropagation();
                ContextMenuDescription($(this), e);
            })

            mElement.off('click', '.tableCell').on('click', '.tableCell', function (e) {
                if (e.ctrlKey) {
                    //$('#tBodyExcelLike tr td.activeCell').parent().addClass('rowSelected');
                    $(this).parent().addClass('rowSelected');
                    $('#tbodyCanvass tr td').removeClass('activeCell');
                }
                else {
                    $('#tbodyCanvass tr td').removeClass('activeCell');
                    $('#tbodyCanvass tr').removeClass('activeRow');
                    $('#tbodyCanvass tr').removeClass('rowSelected');
                    $(this).addClass('activeCell');
                    $(this).parent().addClass('activeRow');
                    $('.context-menu').remove();
                    SetAttributeMobileDescriptionField($(this));
                }
            })

            mElement.off('dblclick', '.InputTextOnly').on('dblclick', '.InputTextOnly', function () {
                var currentVal = $(this).text();
                var replaceWithTextbox = $('<input type="text" class="InputText">');

                $(this).replaceWith(replaceWithTextbox.val(currentVal));
                replaceWithTextbox.focus(function () {
                    $(this).select();
                });;
            })

            mElement.off('blur', '.InputText').on('blur', '.InputText', function () {
                var newVal = $('.InputText').val();
                $(this).replaceWith('<a class="InputTextOnly">' + newVal + '</a>');
            })

            mElement.off('dblclick', '.tdProjProductDescription').on('dblclick', '.tdProjProductDescription', function () {
                if (ValidateCanvass()) {
                    return;
                }
                var div = $(this).find('div');
                var thisVal = div.text();
                prevDescription = thisVal;
                div.replaceWith($('<input>')
                                        .attr('type', 'text')
                                        .attr('value', $.trim(thisVal))
                                        .addClass('typeText')
                                        .addClass('inputTextInExcel')
                                        .attr('data-role', 'none')
                                        .attr('skuid', div.attr('skuid'))
                                        .attr('dbid', div.attr('dbid'))
                                        .attr('datatype', div.attr('datatype'))
                                        .attr('status', div.attr('status'))
                                        .data('data', div.data('data'))
                                    );
                $('.typeText').focus();
            })

            mElement.off('blur', '.typeText').on('blur', '.typeText', function () {
                if (isFocusOut == false) {
                    SubmitValue($(this));

                    if ($.trim($('#tbodyCanvass tr:last').find('td.tdProjProductDescription').text()) != "") {
                        AddNewRow();
                    }
                }
                isFocusOut = false;
            })

            mElement.off('keydown', '.typeText').on('keydown', '.typeText', function (e) {
                e.stopPropagation();
                if (e.which == 13 || e.which == 38 || e.which == 40 || e.which == 27) {
                    isFocusOut = true;
                    SubmitValue($(this), (e.which == 27));

                    if ($.trim($('#tbodyCanvass tr:last').find('td.tdProjProductDescription').text()) != "") {
                        AddNewRow();
                    }

                    if (e.which == 39) {
                        $('#tbodyCanvass tr td.activeCell').closest('td').next().addClass('activeCell');
                        $('#tbodyCanvass tr td.activeCell').closest('td').prev().removeClass('activeCell');
                    }
                    else {
                        currentCellIndex = $('#tbodyCanvass tr td.activeCell').closest('td').index();
                        $('#tbodyCanvass tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                        $('#tbodyCanvass tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                        $('#tbodyCanvass tr td.activeCell').closest('tr').prev().removeClass('activeRow');
                        $('#tbodyCanvass tr td.activeCell').closest('tr').addClass('activeRow');
                    }
                }

                if ($(this).attr('datatype') == 'numeric' && !NumberDotOnly($(this), e)) {
                    return;
                }
            })

            mElement.off('click', '.tdProjProductUom').on('click', '.tdProjProductUom', function () {
                if (ValidateCanvass()) {
                    return;
                }

                var activeRow = tbodyCanvass.find('tr.activeRow');
                var desc = activeRow.find('.tdProjProductDescription > div');
                var skuid = desc.attr('skuid');
                var library;
                var data = {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'Measurement' }]
                }

                if (IsGuid(skuid)) {
                    library = 'LibraryPIMProductMeasure';
                    data.Click = function (e) {
                        activeRow.find('.tdProjProductUom > div').attr('uomid', e.UnitMeasureID)
                            .text(e.Measurement);
                    }
                } else {
                    library = 'LibraryPIMMeasure';
                    data.Click = function (e) {
                        activeRow.find('.tdProjProductUom > div').attr('uomid', e.MeasureNameID)
                            .text(e.Measurement);
                    }
                }

                if (CStr2($.trim(desc.text())) != '') {
                    BrowsePopup(library, 'Select unit of measure', [
                    data
                    ], { ProductID: skuid })
                }
            })

            mElement.off('click', '.tdProjProductFrequency').on('click', '.tdProjProductFrequency', function () {
                if (ValidateCanvass()) {
                    return;
                }

                var desc = $.trim(tbodyCanvass.find('tr.activeRow .tdProjProductDescription > div').text());
                if (desc == '') {
                    return;
                }
                $('#lblFrequencyProductDesc').text();
                var freq = tbodyCanvass.find('.activeCell > div');

                if (CBool(freq.attr('isotherfreq'))) {
                    $('#txtCanvassFreqOthers').val(freq.text()).show();
                    $('#selCanvassFrequency').val('Others');
                } else {
                    $('#txtCanvassFreqOthers').val('').hide();
                    $('#selCanvassFrequency').val(freq.text());
                }

                $('#popCanvassFrequency').modal('show');
            })

            mElement.off('click', '.tdProjVendorComment').on('click', '.tdProjVendorComment', function () {
                $('#popCanvassComment').modal('show');
                var data = tbodyCanvass.find('td.activeCell > div').data('data');
                var comment = '';

                if (data.length > 0) {
                    comment = data[0].VendorComment;
                }

                $('#txtCanvassComment').val(comment).focus();
            })

            mElement.off('click', '.tdProjProductTradeDiscount').on('click', '.tdProjProductTradeDiscount', function () {
                if (ValidateCanvass()) {
                    return;
                }

                if ($(this).hasClass('tableCellDisable')) {
                    return;
                }

                $('#popCanvassTradeDiscount').modal('show');

                var data = tbodyCanvass.find('td.activeCell > div').data('data');
                $('#txtTradeDiscountLess').val('').focus();
                $('#txtTradeDiscountRemarks').val('');

                if (data.length > 0) {
                    $('#txtTradeDiscountLess').val(data[0].TradeDiscountLess);
                    $('#txtTradeDiscountRemarks').val(data[0].TradeDiscountRemarks);
                    TradeDiscount(data[0].TradeDiscountLess)
                }
            })

            mElement.off('click', '.tdProjProductCashDiscount').on('click', '.tdProjProductCashDiscount', function () {
                if (ValidateCanvass()) {
                    return;
                }

                if ($(this).hasClass('tableCellDisable')) {
                    return;
                }

                var data = tbodyCanvass.find('td.activeCell > div').data('data');
                isFromAdjustPrices = false;
                PopupCashDiscounts(data);
            })

            mElement.off('submit', '#formCanvassSecurityAccess').on('submit', '#formCanvassSecurityAccess', function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Canvass/CheckUserAccountToLinkSku',
                    data: {
                        Username: $.trim($('#txtLinkSKUUsername').val()),
                        UserPassword: $.trim($('#txtLinkSKUPassword').val()),
                    },
                    success: function (e) {
                        if (e.Results == 'ok') {
                            $('#popCanvassSecurityAccess').modal('hide');
                            LinkSku();
                        } else {
                            WebToast('Invalid password', 'top');
                        }
                    }
                })
            })
        }

        function VendorPriceComparison() {
            if (isCanvassDraft) {
                //return;
            }

            var tbodyCanvassVendorPriceComparison = $('#tbodyCanvassVendorPriceComparison');
            var trowVendorPriceComparison = $('#trowVendorPriceComparison');

            tbodyCanvassVendorPriceComparison.empty();
            $('.thVendorPriceComparison').remove();
            //$('#tabPCWorkVendorPriceComparison').opentabmenu();

            for (var i = 0; i < arrCanvassItem.length; i++) {
                var trRegTrade = $('<tr id="regtrade-' + arrCanvassItem[i].DBID + '" style="font-weight:bold;">')
                        .append($('<td rowspan="2" style="border-bottom:2px solid #CCC;">').append((i + 1) + '. ' + arrCanvassItem[i].Description))
                        .append($('<td>').text('Regular Trade'))
                var trLowestPrice = $('<tr id="lowestprice-' + arrCanvassItem[i].DBID + '" style="font-size:20px; border-bottom:2px solid #CCC;">')
                        .append($('<td>').text('Lowest Price'))

                tbodyCanvassVendorPriceComparison.append(trRegTrade).append(trLowestPrice);
            }

            var vendorIndex, thCompanyName = 2;
            for (var i = 0; i < arrVendor.length; i++) {
                vendorIndex = i + 1;
                trowVendorPriceComparison.append($('<th style="text-align:center;" class="custom-theme-f thVendorPriceComparison">').text(arrVendor[i].CompanyName))
                if (arrVendor[i].data.length > 0) {
                    for (var v = 0; v < arrVendor[i].data.length; v++) {
                        var vData = arrVendor[i].data[v];
                        var trRegTrade = $('#regtrade-' + vData.ItemDBID)
                        var trLowestPrice = $('#lowestprice-' + vData.ItemDBID)
                        var islowestregtrade = (vData.IsLowestNetTradeDiscount ? 'background-color: #d6ffa7' : '');
                        var islowestprice = (vData.IsLowestNetTradeVolumeAndCashDiscount ? 'background-color: #eaffd1' : '');

                        if (trRegTrade.find('td').eq(thCompanyName - 1).length == 0) {
                            while (trRegTrade.find('td').length < thCompanyName) {
                                trRegTrade.append($('<td>'));
                            }
                        }

                        trRegTrade.append($('<td class="regtrade" style="text-align:center;' + islowestregtrade + '">').text(CStr2(vData.PriceNetOfTradeDiscount)));

                        if (trLowestPrice.find('td').eq((thCompanyName - 1) - 1).length == 0) {
                            while (trLowestPrice.find('td').length < thCompanyName - 1) {
                                trLowestPrice.append($('<td>'));
                            }
                        }

                        trLowestPrice.append($('<td class="lowestprice" style="text-align:center;' + islowestprice + '">').text(CStr2(vData.PriceNetOfTradeVolumeAndCashDiscount)));
                    }
                } else {
                    tbodyCanvassVendorPriceComparison.find('tr').each(function () {
                        $(this).append($('<td>'))
                    })
                }

                thCompanyName += 1
            }

            //fill row with empty cells
            var thlength = trowVendorPriceComparison.find('th').length;
            tbodyCanvassVendorPriceComparison.find('tr').each(function () {
                var arrRegTradePrice = [], arrLowestPrice = [];
                var tdlength = $(this).find('td').length;

                if (thlength != tdlength) {
                    var noofcolumn = ($(this).index() % 2 == 0 ? (thlength - tdlength) : (thlength - tdlength) - 1)

                    for (var i = 1; i <= noofcolumn; i++) {
                        $(this).append($('<td>'));
                    }
                }
            })
        }

        function SetAttributeMobileDescriptionField(field) {
            var div = field.find('> div');
            var datatype = div.attr('datatype');
            var thisDataSkuID = div.attr('skuid');
            var thisDataDBID = div.attr('dbid');
            var thisDataStatus = div.attr('status');
            var val = $.trim(div.text());
            var isTextField = false, attr;

            if (field.hasClass('tdProjProductDescription')) {
                $('#lblMobileField').text('Enter Description:');
                isTextField = true;
                attr = {
                    col: 'description',
                    datatype: CStr2(datatype),
                    skuid: CStr2(thisDataSkuID),
                    dbid: CStr2(thisDataDBID),
                    status: CStr2(thisDataStatus)
                }
            } else if (field.hasClass('tdProjProductReqQty')) {
                $('#lblMobileField').text('Enter Quantity:');
                isTextField = true;
                attr = {
                    col: 'qty',
                    datatype: CStr2(datatype)
                }
            } else if (field.hasClass('tdProjProductSRP')) {
                $('#lblMobileField').text('Enter SRP:');
                isTextField = true;
                attr = {
                    col: 'srp',
                    datatype: CStr2(datatype)
                }
            }

            if (isTextField) {
                txtMobileDescriptionField.attr(attr).val(val);
            }
        }

        function PopupCashDiscounts(data) {
            $('#popCanvassCashDiscount').modal('show');
            $('#txtCanvassCDRate').val('').focus();
            $('#txtCanvassCDIfPaidIn, #txtCanvassCDNetAmount').val('');

            if (data.length > 0) {
                $('#txtCanvassCDRate').val(data[0].CashDiscountRate);
                $('#txtCanvassCDIfPaidIn').val(data[0].CashDiscountIfPaidIn);
                $('#txtCanvassCDNetAmount').val(data[0].CashDiscountNetAmountToBePaidIn);
            }
        }

        function CreateNewCanvass() {
            ClearWorkFields();
            $('#tabPriceCanvassWork, #tabPCWorkBasicInfo').opentabmenu();
            $('#tbodyCanvass tr:first td.tdProjProductDescription').addClass('activeCell');
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
                        NewMessageBox('Successfully Uploaded', 'Message');
                        mOut.GetPriceCanvass();
                        mOut.DisplayCanvassDetails(vData.data, false);
                    }
                }
            });
        }

        function FilterList() {
            var selFilterCanvassCategory = $('[name="selFilterCanvassCategory"]').val();
            var selFilterCanvassBrand = $('[name="selFilterCanvassBrand"]').val();

            if ($('[name="selFilterCanvassCategory"]').val() == '' && $('[name="selFilterCanvassBrand"]').val() == '') {
                tbodyCanvass.find('tr').show();
            } else if ($('[name="selFilterCanvassCategory"]').val() != '' && $('[name="selFilterCanvassBrand"]').val() != '') {

            } else {
                tbodyCanvass.find('tr:visible').hide();

                if (selFilterCanvassCategory != '') {
                    if (selFilterCanvassBrand == '') {
                        tbodyCanvass.find('tr:visible').hide();
                    }
                    tbodyCanvass.find('.tdProjProductCategory > div:contains(' + selFilterCanvassCategory + ')').parents().closest('tr').show();
                }

                if (selFilterCanvassBrand != '') {
                    if (selFilterCanvassCategory == '') {
                        tbodyCanvass.find('tr:visible').hide();
                    }
                    tbodyCanvass.find('.tdProjProductBrand > div:contains(' + selFilterCanvassBrand + ')').parents().closest('tr').show();
                }
            }
        }

        var activeCell;
        function ContextMenuDescription(elem, e) {
            $('#tbodyCanvass tr td').removeClass('activeCell');
            $('#tbodyCanvass tr').removeClass('activeRow');
            $(elem).addClass('activeCell');
            $(elem).parent().addClass('activeRow');
            $('.context-menu').remove();
            activeCell = $('.activeCell > div');

            if (isCanvassDraft && $.trim(activeCell.text()) == '') {
                WebToast('Please enter description first', 'top');
                return;
            }

            var ul = $('<div class="list-group list-group-flush">');

            if ((!isCanvassDraft && CStr2($.trim(activeCell.attr('skuid'))) == '') || isCanvassDraft) {
                ul.append($('<a class="list-group-item">')
                    .html(($.trim(activeCell.attr('skuid')) == '' ? 'Link to SKU' : 'Change Linked SKU'))
                    .click(function (e) {
                        e.stopPropagation();
                        $('.context-menu').remove();

                        if (!isCanvassDraft && CStr2($.trim(activeCell.attr('skuid'))) == '') {
                            AjaxSend({
                                url: 'Canvass/CheckIfUserCanLinkSku',
                                success: function (e) {
                                    if (e.Results == 'ok') {
                                        LinkSku();
                                    } else {
                                        $('#popCanvassSecurityAccess').modal('show');
                                        $('#txtLinkSKUUsername').val('').focus();
                                        $('#txtLinkSKUPassword').val('');
                                    }
                                }
                            })
                        } else {
                            LinkSku();
                        }
                    })
                )
            }

            if (isCanvassDraft) {
                ul.append($('<a class="list-group-item">')
                    .html('Delete Item')
                    .click(function (e) {
                        e.stopPropagation();
                        $('.context-menu').remove();
                        NewMessageBox('Are you sure you want to remove <b>' + activeCell.text() + '</b>?', 'Message', [
                            {
                                Text: 'Yes', Click: function () {
                                    activeCell.data('data').State = 'Remove';
                                    activeCell.parents().closest('tr').addClass('deleteditem').hide();
                                    activeCell.removeAttr('skuid');
                                    UpdateItemNumber();
                                }
                            }, { Text: 'No' }
                        ])
                    })
                )
            }

            if (ul.find('a').length > 0) {
                $('body').append($('<div>')
                    .append(ul)
                    .addClass('context-menu')
                    .css({ top: e.pageY + "px", left: e.pageX + "px" })
                )
            }
        }

        function LinkSku() {
            BrowsePopup('LibraryPIMProduct', 'Search SKU',
                [
                    {
                        AddUsingWebForm: true,
                        //SelectMultiple: true,
                        Message: 'Please choose specific product or service (SKU) to associate with <b>' + name + '</b>',
                        DisplayText: [{ Column: 'ProductName' }],
                        Click: function (e) {
                            if ($('#tbodyCanvass .tdProjProductDescription:not(.activeCell) > div[skuid="' + e.ID + '"]').length > 0) {
                                NewMessageBox('The selected SKU has already been added in the list.', 'Message');
                                return;
                            }
                            e.DBID = activeCell.attr('dbid');
                            e.State = 'Edit';
                            activeCell.parent().addClass('hasAssociatedSKU').attr('data-original-title', e.ProductName);
                            activeCell.attr('skuid', e.ID).data('data', e);
                            activeCell.parents().closest('tr').find('.tdProjProductCategory > div').text(e.Category);
                        }
                    }
                ]
            )
        }

        function SubmitValue(dis, isEsc) {
            var val = $.trim((isEsc ? prevDescription : dis.val()));
            var cellstobedisabled = dis.parent().closest('tr').find('.tdProjProductSRP, .tdProjProductTradeDiscount, .tdProjProductNetTradeDiscount, .tdProjProductVolumeDiscount, .tdProjProductCashDiscount, ' +
                    '.tdProjProductVolumeDiscount, .tdProjProductLowestPrice')
            var desc = $('.activeCell').parent().find('.tdProjProductDescription > *');

            //if (CStr2(desc.attr('skuid')) == '') {
            //    cellstobedisabled.addClass('tableCellDisable');
            //} else {
            //    cellstobedisabled.removeClass('tableCellDisable');
            //}

            if (dis.parent().hasClass('tdProjProductSRP')) {
                PopulateVendorData('srp', {
                    Price: val
                })
            }

            if (dis.parent().hasClass('tdProjProductNetTradeDiscount')) {
                dis.parents().closest('tr').find('.tdProjProductTradeDiscount').html('');
                PopulateVendorData('tradediscount', {
                    TradeDiscountLess: '',
                    TradeDiscountRemarks: '',
                    PriceNetOfTradeDiscount: CDec2(val)
                })
                //arrVendor[indVendor].data[indVendorData].PriceNetOfTradeDiscount = val;
            }

            //if (dis.parent().hasClass('tdProjProductVolumeDiscount')) {
            //    PopulateVendorData('volumediscount', {
            //        VolumeDiscount: val
            //    })
            //}

            dis.replaceWith($('<div>')
                .attr('datatype', dis.attr('datatype'))
                .attr('skuid', dis.attr('skuid'))
                .attr('dbid', dis.attr('dbid'))
                .attr('status', dis.attr('status'))
                .attr('data-role', 'none')
                .data('data', dis.data('data'))
                .html(val)
            );
        }

        function PopulateVendorData(column, data) {
            var descData = $('.activeRow > .tdProjProductDescription > div').data('data');
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

        function UpdateItemNumber() {
            var ctr = 0;
            $('#tbodyCanvass tr:not(.deleteditem)').each(function () {
                $(this).attr('id', ctr);
                ctr += 1;
                $(this).find('.tdProjItemNo > div').text(ctr);
            })
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

        function PopulatePricesOfCurrentSelectedVendor() {
            curVendor = cFilterVendor.find('option:selected');
            curVendorData = curVendor.data('data');
            indVendor = curVendor.index();

            if (curVendorData == undefined) {
                return;
            }

            var details = curVendorData.data;

            tbodyCanvass.find('.tdProjVendorComment > div').each(function () {
                $(this).html('&nbsp;').data('data', []);
            })

            tbodyCanvass.find('.tdProjProductSRP > div').each(function () {
                $(this).html('&nbsp;');
            })

            tbodyCanvass.find('.tdProjProductTradeDiscount > div').each(function () {
                $(this).html('&nbsp;').data('data', []);
            })

            tbodyCanvass.find('.tdProjProductNetTradeDiscount > div').each(function () {
                $(this).html('&nbsp;').data('data', []);
            })

            tbodyCanvass.find('.tdProjProductCashDiscount > div').each(function () {
                $(this).html('&nbsp;').data('data', []);
            })

            tbodyCanvass.find('.tdProjProductLowestPrice > div').each(function () {
                $(this).html('&nbsp;').data('data', []);
            })

            if (details.length > 0) {
                for (var i = 0; i < details.length; i++) {
                    var vData = details[i]
                    var tr = $('div[dbid="' + vData.ItemDBID + '"]').parents().closest('tr');
                    var divVendorComment = tr.find('.tdProjVendorComment > div');
                    var divSrp = tr.find('.tdProjProductSRP > div');
                    var divTradeDiscount = tr.find('.tdProjProductTradeDiscount > div');
                    var divNetTradeDiscount = tr.find('.tdProjProductNetTradeDiscount > div');
                    var divVolumeDiscount = tr.find('.tdProjProductVolumeDiscount > div');
                    var divCashDiscount = tr.find('.tdProjProductCashDiscount > div');

                    divVendorComment.html((CStr2(vData.VendorComment).length > 50 ? CStr2(vData.VendorComment).substring(0, 50) + '... (see more)' : CStr2(vData.VendorComment)))
                        .data('data', [{ VendorComment: CStr2(vData.VendorComment) }])
                    divSrp.html(vData.Price || '&nbsp;');

                    if (CStr2(vData.TradeDiscountLess) != '') {
                        divTradeDiscount.text(vData.TradeDiscountLess + ' (' + TradeDiscount(vData.TradeDiscountLess).NetRate + '% NDR)')
                            .data('data', [{
                                TradeDiscountLess: vData.TradeDiscountLess,
                                TradeDiscountRemarks: vData.TradeDiscountRemarks
                            }])
                    } else {
                        divTradeDiscount.html('&nbsp;').data('data', [])
                    }

                    if (CStr2(vData.CashDiscountRate) != '') {
                        divCashDiscount.text(vData.CashDiscountRate + ' % ' + vData.CashDiscountIfPaidIn + ', Net ' + vData.CashDiscountNetAmountToBePaidIn)
                            .data('data', [{
                                CashDiscountRate: vData.CashDiscountRate,
                                CashDiscountIfPaidIn: vData.CashDiscountIfPaidIn,
                                CashDiscountNetAmountToBePaidIn: vData.CashDiscountNetAmountToBePaidIn
                            }])
                    } else {
                        divCashDiscount.html('&nbsp;').data('data', [])
                    }

                    //if (CStr2(vData.VolumeDiscount) != '') {
                    //    divVolumeDiscount.text(vData.VolumeDiscount)
                    //}

                    ComputePrices(vData);

                    divNetTradeDiscount.html(vData.PriceNetOfTradeDiscount);
                }
            } else {

            }

            //curSkuPrices = curVendorData.SkuPrices;

            //$('.txtVendorPrice').val('');
            //$('#lblVendorName').text(curVendor.text());

            //for (var i = 0; i < curSkuPrices.length; i++){
            //    $('[skuid="' + curSkuPrices[i].ID + '"]').val(curSkuPrices[i].VendorPrice);
            //}
        }

        function ComputePrices(data) {
            var tr = $('div[dbid="' + data.ItemDBID + '"]').parents().closest('tr');
            var tdnettradedisc = tr.find('.tdProjProductNetTradeDiscount > div');
            var tdlowestprice = tr.find('.tdProjProductLowestPrice > div');

            var tradediscountfactor = 1;
            var volumediscountfactor = 1;
            var cashdiscountfactor = 1;

            if (CStr2(data.TradeDiscountLess) != '') {
                tradediscountfactor = 1 - (CDec2(TradeDiscount(CStr2(data.TradeDiscountLess)).Rate) / 100);
            }

            //if (CDec2(data.VolumeDiscount) != 0) {
            //    volumediscountfactor = 1 - (CDec2(data.VolumeDiscount) / 100);
            //}

            if (CDec2(data.CashDiscountRate) != 0) {
                cashdiscountfactor = 1 - (CDec2(data.CashDiscountRate) / 100);
            }

            if (CDec2(data.Price) != 0 && CStr2(data.TradeDiscountLess) != '') {
                tdnettradedisc.text(((CDec2(TradeDiscount(CStr2(data.TradeDiscountLess)).Factor) / 100) * CDec2(data.Price)).toFixed(4))
                data.PriceNetOfTradeDiscount = $.trim(tdnettradedisc.text());
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
            tdlowestprice.text(data.LowestPrice);
        }

        function AddCanvassItemForInputPrice() {
            var tbodyInputPrices = $('#tbodyInputPrices');
            tbodyInputPrices.empty();

            if (arrCanvassItem.length > 0) {
                for (var i = 0; i < arrCanvassItem.length; i++) {
                    if (arrCanvassItem[i].State == 'Remove') {
                        return
                    }
                    tbodyInputPrices.append($('<tr>').append($('<td>')
                                    .text(arrCanvassItem[i].Description)
                                )
                                .append($('<td>')
                                    //.append($('<input type="text" class="form-control">'))
                                )
                            )
                    if (arrCanvassItem[i].Sku.length > 0) {
                        for (var s = 0; s < arrCanvassItem[i].Sku.length; s++) {
                            if (arrCanvassItem[i].Sku[s].State == 'Remove') {
                                return
                            }
                            tbodyInputPrices.append($('<tr class="skurow">').append($('<td>')
                                        .css('padding-left', '50px')
                                        .text(arrCanvassItem[i].Sku[s].ProductName)
                                    )
                                    .append($('<td>')
                                        .data('data', arrCanvassItem[i].Sku[s])
                                        .append($('<input type="text" class="form-control txtVendorPrice">')
                                            .attr('skuid', arrCanvassItem[i].Sku[s].ID)
                                            .data('data', arrCanvassItem[i].Sku[s])
                                            .keydown(function (e) {
                                                if (!NumberDotOnly($(this), e)) {
                                                    return;
                                                }
                                            })
                                            .unbind('blur')
                                            .blur(function (e) {
                                                e.stopPropagation();
                                                e.preventDefault();

                                                var disDataString = JSON.stringify($(this).data('data')), isExist = false, curIndex = 0;
                                                var disData = JSON.parse(disDataString);
                                                curSkuPrices = arrVendor[indVendor].SkuPrices;

                                                for (var i = 0; i < curSkuPrices.length; i++) {
                                                    if (disData.ID == curSkuPrices[i].ID) {
                                                        curIndex = i;
                                                        isExist = true;
                                                        break;
                                                    }
                                                }

                                                if (!isExist) {
                                                    disData.VendorPrice = $.trim($(this).val());
                                                    curSkuPrices.push(disData)
                                                } else {
                                                    curSkuPrices[curIndex].VendorPrice = $.trim($(this).val());
                                                }
                                                arrVendor[indVendor].SkuPrices = curSkuPrices;
                                            })
                                        )
                                    )
                                )
                        }
                    }
                }
            } else {

            }
        }

        function DisplayInputPrices(data) {
            $('#divCanvassSku').hide();
            for (var i = 0; i < arrVendor.length; i++) {
                if (arrVendor[i].State != 'Remove') {
                    $('#divCanvassSku').show();
                }
            }
        }

        function AddVendorToFilter(data) {
            cFilterVendor
                .append($('<option>')
                    .data('data', data)
                    .attr('value', data.ID)
                    .text((CBool(data.IsOrganization) ? data.CompanyName : $.trim(CStr2(data.FirstName) + ' ' + CStr2(data.MiddleName) + ' ' + CStr2(data.Surname))))
                )

            $('#selAdjustPricesVendor')
                .append($('<option>')
                    .data('data', data)
                    .attr('value', data.ID)
                    .text((CBool(data.IsOrganization) ? data.CompanyName : $.trim(CStr2(data.FirstName) + ' ' + CStr2(data.MiddleName) + ' ' + CStr2(data.Surname))))
                )
        }

        function AddCanvassItem() {
            tbodyCanvass.empty();

            if (arrCanvassItem.length > 0) {
                txtMobileDescriptionField.val(arrCanvassItem[0].Description);
                for (var i = 0; i < arrCanvassItem.length; i++) {
                    arrCanvassItem[i].Index = i;
                    AddNewRow(arrCanvassItem[i]);
                }
            }
            AddNewRow();
            $('#tbodyCanvass tr:first td.tdProjProductDescription').addClass('activeCell');
            $('.thProductInfo').show();
            $('.thVendor').hide();
        }

        function PushToListSku(pInfo) {
            var data = {
                ProductName: pInfo.ProductName,
                ID: pInfo.ID,
                State: 'New',
                DBID: NewID()
            }
            arrLinkToSku.push(data);
        }

        function DisplayLinkSku() {
            var tbody = $('#tbodyLinkSku');
            tbody.empty();
            for (var i = 0; i < arrLinkToSku.length; i++) {
                arrLinkToSku[i].arrindex = i;
                if (arrLinkToSku[i].State != "Remove") {
                    tbody.append($('<tr>')
                            .data('data', arrLinkToSku[i])
                            .append($('<td>')
                                .text($.trim(arrLinkToSku[i].ProductName))
                            )
                            //.append($('<td>')
                            //    .append($('<input class="form-control" onclick="this.select()">').data('data', arrLinkToSku[i])
                            //        .val(arrLinkToSku[i].VendorPrice)
                            //        .keydown(function (e) {
                            //            if (!NumberDotOnly($(this), e)) {
                            //                return;
                            //            }
                            //        })
                            //        .keyup(function () {
                            //            arrLinkToSku[CInt2($(this).data('data').arrindex)].VendorPrice = $(this).val()
                            //        })
                            //    )
                            //)
                            .append($('<td>')
                                .data('data', arrLinkToSku[i])
                                .append($('<a style="margin-top:8px;" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Remove">')
                                    .text('Remove')
                                    .button()
                                )
                                .click(function (e) {
                                    var data = $(this).data('data');
                                    var id = data.ID;
                                    NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                if (data.State == 'New') {
                                                    arrLinkToSku = $.grep(arrLinkToSku, function (val) {
                                                        return val.ID != id;
                                                    })
                                                }
                                                else {
                                                    arrLinkToSku[data.arrindex].State = "Remove";
                                                }

                                                //remove also the sku in arrVendor
                                                for (var i = 0; i < arrVendor.length; i++) {
                                                    for (var s = 0; s < arrVendor[i].SkuPrices.length; s++) {
                                                        if (arrVendor[i].SkuPrices[s].ID == id) {
                                                            if (arrVendor[i].SkuPrices[s].State == 'New') {
                                                                (arrVendor[i].SkuPrices).splice(s, 1);
                                                            } else {
                                                                arrVendor[i].SkuPrices[s].State = 'Remove';
                                                            }
                                                        }
                                                    }
                                                }

                                                DisplayLinkSku();
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                        )
                }
            }
        }

        function ClearCanvassItemsField() {
            cItemDescription.val('');
            cItemQty.val('');
            cItemUom.val('');
            $('#tbodyLinkSku').empty().append('<tr><td colspan="5" style="text-align:center;">No data found. Click "+" to add item.</td></tr>');;
            arrLinkToSku = [];
        }

        function LoadMeasurements() {
            var selCanvassMeasurement = $('[name="selCanvassMeasurement"]');

            selCanvassMeasurement.empty();
            selCanvassMeasurement.append(new Option('Please select unit of measure', ''));

            for (var i = 0; i < mMeasurements.length; i++) {
                selCanvassMeasurement.append(new Option(mMeasurements[i].Measurement, mMeasurements[i].ID));
            }
        }

        function LoadPriceCanvass(s) {
            if ($.fn.DataTable.isDataTable(mElement.find('#tblDataTablePriceCanvass'))) {
                mElement.find('#tblDataTablePriceCanvass').empty();
                dtPriceCanvass.destroy();
                //return;
            }

            dtPriceCanvass = mElement.find('#tblDataTablePriceCanvass').DataTable({
                //"data": arrPriceCanvass,
                ajax: {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        keywords: CStr2(s),
                        type: sessionStorage.PriceCanvassType,
                        IsActive: mElement.find('[name="selCanvassListStatus"]').val()
                    }, '/Canvass/GetPriceCanvass'),
                    type: 'POST'
                },
                "bProcessing": true,
                "bServerSide": true,
                "pageLength": 10,
                "columns": [
                    {
                        title: 'Canvass No.',
                        "data": function (data, type, full, meta) {
                            return data.CanvassNumber;
                        },
                        "width": "150px"
                    },
                    {
                        title: 'Name',
                        "data": function (data, type, full, meta) {
                            return data.CanvassName;
                        }
                    },
                    {
                        title: 'Date',
                        "data": function (data, type, full, meta) {
                            return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                        },
                        width: '200px'
                    },
                    {
                        title: 'No. of Vendors Invited',
                        "data": function (data, type, full, meta) {
                            return data.Vendor
                        },
                        width: '200px'
                    }
                    //{
                    //    "data": function (data, type, full, meta) {
                    //        return $($('<a style="margin-top:8px;" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Remove">')
                    //                .text('Remove')
                    //                .button())[0].outerHTML
                    //    }
                    //}
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [] }],
                bSort: false,
                "responsive": false,
                "columnDefs": [{ "targets": 1, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    $('#tblDataTablePriceCanvass').removeClass('dataTable');
                }
                //"stateSave": true
            })

            mElement.find('#tblDataTablePriceCanvass tbody').unbind('click').on('click', 'td', function () {
                if (dtPriceCanvass.row(this).data() != undefined) {
                    var data = JSON.parse(JSON.stringify(dtPriceCanvass.row(this).data()));
                    if (dtPriceCanvass.cell(this).index().columnVisible == 4) { // delete button
                        NewMessageBox('Are you sure you want to delete this record', 'Message', [
                            {
                                Text: 'Yes', Click: function () {
                                    AjaxSend({
                                        url: 'Canvass/RemoveCanvass',
                                        data: { CanvassID: data.CanvassID },
                                        success: function (e) {
                                            if (e.Results == 'deleted') {
                                                WebToast('Successfully Deleted', 'top');
                                                mOut.GetPriceCanvass();
                                            } else {
                                                NewMessageBox(e.Results, 'Message');
                                            }
                                        }
                                    })
                                }
                            }, { Text: 'No' }
                        ])
                    } else {
                        AjaxSend({
                            url: 'Canvass/GetLinkCanvassData',
                            data: { PriceCanvassID: data.ID },
                            success: function (e) {
                                mOut.DisplayCanvassDetails(e.data, false);
                            }
                        })
                    }
                }
            });
        }

        function ClearWorkFields() {
            isFirstUploadCanvass = true;
            IsCreateNewSKU = false
            isCanvassDraft = true;
            isNewCanvass = true;
            canvassID = '';
            canvassState = 'New';
            arrLinkToSku = [];
            arrVendor = [];
            arrCanvassItem = [];
            cName.val('');
            cNo.val('');
            cFilterVendor.empty();
            $('#selAdjustPricesVendor, #divCanvassVendorLink').empty();
            $('#tabPCWorkBasicInfo').opentabmenu();
            tbodyCanvass.empty();
            AddNewRow();
            txtMobileDescriptionField.attr('col', 'description').val('');
            $('#lblMobileField').text('Enter Description:');
            $('#lblCanvassSaveAs').text('DRAFT');
            $('#txtCanvassShareLink').val('');
            $('#tbodyCanvass tr:last td.tdProjProductDescription').addClass('activeCell');
            $('[name="selFilterCanvassCategory"], #selAdjustPricesCategory').empty().append(new Option('All Categories', ''));
            $('[name="selFilterCanvassBrand"], #selAdjustPricesBrand').empty().append(new Option('All Brands', ''));
            $('#chbNeverExpireLink').attr('checked', 'checked')//.checkboxradio('refresh');
            $('#divExpirationDate').hide();
            $('#txtLinkExpirationDate')
                .attr('min', $.format.date(new Date(), 'yyyy-MM-dd'))
                .val($.format.date(new Date(new Date().setDate(new Date().getDate() + 3)), 'yyyy-MM-dd'))
            //tbodyCanvass.empty().append('<tr><td colspan="5" style="text-align:center;">No data found. Click "+" to add item.</td></tr>');
            //$('#popVendorListEdit').popup(); // init popup
            //$('#popPriceCanvassSearchVendor').popup();
        }

        function AddNewRow(productdata) {
            var i, newItemDBID = NewID();

            if (tbodyCanvass.find('tr').length == 0) {
                i = 0;
            }
            else {
                i = parseInt(tbodyCanvass.find('tr:last').attr('id')) + 1;
            }

            tbodyCanvass.append($('<tr id="' + i + '" rowtype="normal">')
                .append(
                    $('<td class="tddraggable">')
                    .append($('<span class="bi-list">'))
                    .css({
                        backgroundColor: '#fff',
                        width: '36px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html(i + 1)
                        .attr('data-role', 'none')
                        .data('data', [])
                    )
                    .addClass('tableCellDisable tdProjItemNo')
                    .css({
                        width: '90px'
                    })
                )
                .append(
                    $('<td title="' + (productdata == undefined ? '' : CStr2(productdata.ProductName)) + '">') // data-toggle="tooltip" data-placement="top"
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : (productdata.Description || productdata.ProductName)))
                        .attr('data-role', 'none')
                        .attr('skuid', (productdata == undefined ? '' : productdata.ID))
                        .attr('dbid', (productdata == undefined ? newItemDBID : productdata.DBID))
                        .data('data', (productdata || { DBID: newItemDBID }))
                        .attr('status', (productdata == undefined ? 'New' : productdata.State))
                    )
                    .addClass('tableCell tdProjProductDescription ' + (productdata == undefined ? '' : (CStr2(productdata.ID) == '' ? '' : ' hasAssociatedSKU')))
                    .css({
                        width: '305px',
                        left: '137px',
                        backgroundColor: '#fff'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.Qty)))
                        .attr('data-role', 'none')
                        .attr('datatype', 'numeric')
                        .data('data', (productdata || []))
                    )
                    .addClass('tableCell thProductInfo tdProjProductReqQty')
                    .css({
                        width: '100px', textAlign: 'center'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.UomName)))
                        .attr('data-role', 'none')
                        .attr('uomid', (productdata == undefined ? '' : CStr2(productdata.Uom)))
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell tdProjProductUom')
                    .css({
                        width: '100px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.Frequency)))
                        .attr('data-role', 'none')
                        .attr('isotherfrequency', (productdata == undefined ? '&nbsp;' : CInt2(productdata.IsOtherFrequency)))
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thProductInfo tdProjProductFrequency')
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjVendorComment ') // + (productdata == undefined ? '' : (CStr2(productdata.ID) == '' ? 'tableCellDisable' : ''))
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.Category)))
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell tdProjProductCategory')
                    .css({
                        width: '100px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.Brand)))
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell tdProjProductBrand')
                    .css({
                        width: '100px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .attr('datatype', 'numeric')
                        .data('data', [])
                    )
                    .addClass('tableCell thVendor tdProjProductSRP ') // + (productdata == undefined ? '' : (CStr2(productdata.ID) == '' ? 'tableCellDisable' : ''))
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjProductTradeDiscount ')
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .data('data', [])
                        .attr('datatype', 'numeric')
                        //.addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjProductNetTradeDiscount ')
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html((productdata == undefined ? '&nbsp;' : CStr2(productdata.VolumeDiscount)))
                        .attr('data-role', 'none')
                        .attr('datatype', 'numeric')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjProductVolumeDiscount ')
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjProductCashDiscount ')
                    .css({
                        width: '150px'
                    })
                )
                .append(
                    $('<td>')
                    .append($('<div>')
                        .css({ width: '100%' })
                        .html('&nbsp;')
                        .attr('data-role', 'none')
                        .data('data', [])
                        .addClass('tableCellReadOnly')
                    )
                    .addClass('tableCell thVendor tdProjProductLowestPrice ')
                    .css({
                        width: '150px'
                    })
                )
            )

            if ($('#tabPCWorkBasicInfo').css('display') == 'block') {
                $('.thProductInfo').show();
                $('.thVendor').hide();
            } else {
                $('.thProductInfo').hide();
                $('.thVendor').show();
            }

            if (productdata != undefined) {
                AddToFilter(productdata);
            }
        }

        function AddToFilter(data) {
            var selFilterCanvassCategory = $('[name="selFilterCanvassCategory"]');
            var selAdjustPricesCategory = $('#selAdjustPricesCategory');
            var selFilterCanvassBrand = $('[name="selFilterCanvassBrand"]');
            var selAdjustPricesBrand = $('#selAdjustPricesBrand');

            if ((selFilterCanvassCategory.find('option[value="' + CStr2(data.Category) + '"]').length &&
                selAdjustPricesCategory.find('option[value="' + CStr2(data.Category) + '"]').length) == 0) {
                selFilterCanvassCategory.append(new Option(CStr2(data.Category), CStr2(data.Category)));
                selAdjustPricesCategory.append(new Option(CStr2(data.Category), CStr2(data.Category)));
            }

            if ((selFilterCanvassBrand.find('option[value="' + CStr2(data.Brand) + '"]').length &&
                selAdjustPricesBrand.find('option[value="' + CStr2(data.Brand) + '"]').length) == 0) {
                selFilterCanvassBrand.append(new Option(CStr2(data.Brand), CStr2(data.Brand)));
                selAdjustPricesBrand.append(new Option(CStr2(data.Brand), CStr2(data.Brand)));
            }
        }

        function ValidateCanvass() {
            if (!isCanvassDraft && !$('#tabPCWorkVendor').is(':visible') && sessionStorage.PriceCanvassType != 'competitor') {
                WebToast('This transaction was saved as final. This transaction may not be edited.', 'top');
                return true;
            }
            return false;
        }

        $(document).keydown(function (e) {
            if (IsPopupOpen() || $('input[type="text"]:focus').length > 0) {
                return;
            }

            if ($('#divPriceCanvass').css('display') == 'none' && $('#divRevenuePriceCanvass').css('display') == 'none') {
                return;
            }

            var tdActiveCell = mElement.find('#tbodyCanvass tr td.activeCell');
            currentCellIndex = tdActiveCell.closest('td').index();

            //navigate active cell using arrow keys
            if (e.which == 39 || e.which == 9) { // right arrow
                $('#tbodyCanvass tr td.activeCell').closest('td').next().addClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').removeClass('activeRow');
                $('#tbodyCanvass tr td.activeCell').closest('td').prev().removeClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').addClass('activeRow');
            }
            else if (e.which == 37 || (e.which === 9 && e.shiftKey)) { // left arrow
                $('#tbodyCanvass tr td.activeCell').closest('td').prev().addClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').removeClass('activeRow');
                $('#tbodyCanvass tr td.activeCell').closest('td').next().removeClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').addClass('activeRow');
            }
            else if (e.which == 40) { // down arrow
                $('#tbodyCanvass tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').removeClass('activeRow');
                $('#tbodyCanvass tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').addClass('activeRow');
            }
            else if (e.which == 38) { // up arrow
                $('#tbodyCanvass tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').removeClass('activeRow');
                $('#tbodyCanvass tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                $('#tbodyCanvass tr td.activeCell').closest('tr').addClass('activeRow');
            }

            else {
                e.stopPropagation();

                if (!isCanvassDraft && tdActiveCell.hasClass('tdProjProductDescription')) {
                    setTimeout(function () {
                        WebToast('This transaction was saved as final. This transaction may not be edited.', 'top');
                    }, 10)
                    return;
                }

                if (isPopupOpen == false) {
                    if (tdActiveCell.find('> div').hasClass('disableCell') ||
                        tdActiveCell.find('> div').hasClass('tableCellReadOnly') ||
                        tdActiveCell.hasClass('tableCellDisable') ||
                        e.which == 20 || // Caps Lock
                        e.which == 16 || // Shift key
                        e.which == 17 || // Ctrl Key
                        e.which == 91 || // Windows Command Key
                        e.which == 19 || // Pause Break
                        e.which == 18 || // Alt Key
                        (e.which >= 144 && e.which <= 145) // Num Lock, Scroll Lock
                        ) {
                        return false;
                    }

                    var thisVal = tdActiveCell.find('div').text();
                    var thisDataType = tdActiveCell.find('div').attr('datatype');
                    var thisDataSkuID = tdActiveCell.find('div').attr('skuid');
                    var thisDataDBID = tdActiveCell.find('div').attr('dbid');
                    var thisDataStatus = tdActiveCell.find('div').attr('status');
                    var thisData = (tdActiveCell.find('div').data('data') || []);

                    if (e.which == 46) {
                        thisVal = '';
                    }

                    if (thisDataType == "numeric") {
                        if (e.which != 8) {
                            if (!NumberOnly(e)) {
                                return;
                            }
                        }
                    }
                    prevDescription = thisVal;
                    var input = $('<input>')
                        .attr('type', 'text')
                        .attr('value', $.trim(thisVal))
                        .attr('datatype', thisDataType)
                        .attr('skuid', thisDataSkuID)
                        .attr('dbid', thisDataDBID)
                        .attr('status', thisDataStatus)
                        .addClass('typeText')
                        .addClass('inputTextInExcel')
                        .data('data', thisData)
                    tdActiveCell.find('div').replaceWith(input);
                    input.focus().select();
                }
                else {
                    return true;
                }
            }
        })

        return mOut;
    })

    return mDeferred.promise();
})
define([
    'promise!/Scripts/sources/ERP/AssetsManagement/settings/model.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/ContractDesigner/main/main.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AssetsManagement/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.AssetsManagement.settings.controller', null, function () {

        var mOut = {}, invmethodID = '', dtInvoicingMethod, arrCategory = [], arrTransferCat = [];
        var mElement, settingsModel = new ERP.AssetsManagement.settings.model(), workModel = new ERP.AssetsManagement.work.model();;

        mElement = $(mHtml);
        mOut.element = mElement;

        LoadRequiredData();

        $('#tabAssetsMgmtSettings').empty().append(mElement).trigger('create');

        Contract.Populate('#divTableAMContract', 'Asset Management');

        $('[name="labelsetting"]').change(function () {
            if ($(this).val() != 'custom label') {
                $('#txtLabelForAssets, #txtLabelForCustomers').val('');
            }            
        })

        $('#btnSaveAssetSettings').unbind('click').click(function () {
            if ($('[name="labelsetting"]:checked').val() == 'custom label') {
                if ($.trim($('#txtLabelForAssets').val()) == '') {
                    NewMessageBox('Please enter label for "Assets"');
                    return;
                }

                if ($.trim($('#txtLabelForCustomers').val()) == '') {
                    NewMessageBox('Please enter label for "Customers"');
                    return;
                }
            }

            settingsModel.SaveSettings({
                data: {
                    LabelSetting: $('[name="labelsetting"]:checked').val(),
                    CustomLableForAssets: $.trim($('#txtLabelForAssets').val()),
                    CustomLableForCustomers: $.trim($('#txtLabelForCustomers').val()),
                    CheckOutTimeID: $.trim($('#txtCheckOutTimeID').val()),
                    CheckOutTime: $('#selCOhour').val() + ":" + $('#selCOminute').val() + " " + $('#selCOmeredian').val(),
                    CheckInTime: mElement.find('[name=selCheckInTimeHour]').val() + ":" + mElement.find('[name=selCheckInTimeMinute]').val() + " " + mElement.find('[name=selCheckInTimeMeridian]').val(),
                    EarlyCheckInTime: mElement.find('[name=selEarlyCheckInTimeHour]').val() + ":" + mElement.find('[name=selEarlyCheckInTimeMinute]').val() + " " + mElement.find('[name=selEarlyCheckInTimeMeridian]').val(),
                    LateCheckOutTime: mElement.find('[name=selLateCheckOutTimeHour]').val() + ":" + mElement.find('[name=selLateCheckOutTimeMinute]').val() + " " + mElement.find('[name=selLateCheckOutTimeMeridian]').val(),
                    EarlyCheckInProductID: mEarlyCheckInProductID,
                    LateCheckOutProductID: mLateCheckOutProductID,
                    OutdatedResID: $.trim($('#txtOutdatedResID').val()),
                    IsAllowOutdatedRes: $('[name="radOutdatedRes"]:checked').val(),
                    FutureDatedCheckInsID: $.trim($('#txtFutureDatedCheckInsID').val()),
                    IsAllowFutureDatedCheckIns: $('[name="radFutureDated"]:checked').val(),
                    MaxNumberOfGuestCardsID: $('#txtNoOfGuestCardsID').val(),
                    MaxNumberOfGuestCards: $('#txtMaxNumberOfGuestCards').val(),

                    AllowLostCardCreationID: $('#txtAllowLostCardCreationID').val(),
                    AllowLostCardCreation: $('[name="radLostCard"]:checked').val(),
                    MaximumNumberOfLostCardID: $('#txtMaxNumberOfLostCardsID').val(),
                    MaximumNumberOfLostCard: $('#txtMaxNumberOfLostCards').val(),
                    DefaultChargeOfLostCardID: $('#txtChargeforLostCardsID').val(),
                    DefaultChargeOfLostCard: $('#selChargeforLostCards').val(),
                    CreateKeyCardAfterCheckInID: $('#txtCreateKeyCardAfterCheckInID').val(),
                    CreateKeyCardAfterCheckIn: $('[name="radCreateKeyCardAfterCheckIn"]:checked').val(),
                    DisplayProductNoInAssetsID: $('#txtDisplayProductNoInAssetsID').val(),
                    DisplayProductNoInAssets: $('[name="radDisplayProductNoInAssets"]:checked').val(),
                    DisplayAssetPrice: $('[name="radAssetPrice"]:checked').val(),
                    DownpaymentRequired: $('[name="radDownpayment"]:checked').val(),

                },
                success: function (e) {
                    WebToast('Settings has been successfully saved', 'top');
                    GetAssetSetting(function () {
                        AssetMgmtWorkController.CheckOudatedReservationDate();
                        //AssetMgmtWorkController.CheckOudatedReservationDate(CDec2($('#txtAvailabilityYear').val()), CDec2($('#selAvailabilityMonth').val())); // update the grid date
                    });
                }
            })
        })

        $('#btnSaveDoorLockSettings').click(function () {
            settingsModel.SaveDoorLockSettings({
                data: {
                    DoorLockID: $('#txtDoorLockSystemID').val(),
                    DoorLock: $('[name="radDoorLock"]:checked').val()
                },
                success: function (e) {
                    NewMessageBox('Door Lock Settings has been saved', 'Door Lock Settings');
                }
            })
        })

        $('#btnAddInvoicingMethod').unbind('click').click(function () {
            $('#popInvoicingMethod').modal('show');
            GetAssetsCategory();
            mOut.ClearInvoiceMethod();
        })

        $('#btnInvMethodSave').unbind('click').click(function () {
            SaveInvoicingMethod(false);
        })

        $('#btnInvMethodContinueSave').click(function () {
            SaveInvoicingMethod(true);
        })

        $('#btnInvMethodBack').click(function () {
            $('#popInvoicingMethodCategoryExisting').modal('hide');
            $('#popInvoicingMethod').modal('show');
        })

        $('#txtSearchInvoicingMethod').keypress(function (e) {
            if (e.which == 13) {
                GetInvoicingMethod()
            }
        })

        $('.btnInfoDailyPrice').click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            WebToast('When this option is selected, the rentable asset’s unit is treated as a <b>daily/ nightly</b> price', 'top');
        })

        $('.btnInfoMonthlyPrice').click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            WebToast('When this option is selected, the rentable asset’s unit is treated as a <b>monthly</b> price', 'top');
        })

        $('[name="radInvoiceMethod"]').change(function () {
            if ($(this).val() == 'bynthday') {
                $('#divMonthlyBillType').show();
            } else {
                $('#divMonthlyBillType').hide();
            }
        })

        $('#selIMStatus').change(function () {
            GetInvoicingMethod();
        })

        $('#btnAddOtherCharges').click(function () {
            mOut.ClearOtherCharges();
            $('#popAMOtherCharges').modal('show');
        });

        var mEarlyCheckInProductID;
        mElement.find('[name=txtEarlyCheckInChargeSku]').on('click', function(e) {
            BrowseProduct({
                title: 'Select Early Check In / Rent Out Time SKU',
                OnSelect: function (vData) {
                    mEarlyCheckInProductID = vData.ID;
                    mElement.find('[name=txtEarlyCheckInChargeSku]').val(vData.ProductName);
                }
            });
        });

        var mLateCheckOutProductID;
        mElement.find('[name=txtLateCheckOutChargeSku]').on('click', function(e) {
            BrowseProduct({
                title: 'Select Late Check Out / Return Time SKU',
                OnSelect: function (vData) {
                    mLateCheckOutProductID = vData.ID;
                    mElement.find('[name=txtLateCheckOutChargeSku]').val(vData.ProductName);
                }
            });
        });

        function BrowseProduct(vParams) {
            vParams = $.extend({
                OnSelect: function() {}
            }, vParams);

            var pColumns = [
                { 
                    Column: 'ProductNo', 
                    Label: 'Product No.' 
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name' 
                }
            ];
            pColumns.push({
                Label: 'Retail Price' ,
                Column: function(vData) {
                    return AccountingFormat(vData.RetailPrice);
                }
            });

            BrowsePopup('LibraryPIMProduct', 'Product',
                [
                    {
                        AutoSearch: false,
                        //ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            vParams.OnSelect(e);
                        },
                        AllowEmptySearch: false,
                        StyleFormat: 'table'
                    }
                ], {}
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        $('#txtAMOtherChargesSku').click(function () {
            BrowsePopup('AssetsManagement/SearchSku', 'Search SKU', [
                {
                    DisplayText: [{ Column: 'ProductNo', Label: 'Product No:' }, { Column: 'ProductName' }],
                    Click: function (e) {
                        GetSkuDetails(e);
                    },
                    Close: function () {
                        $('#popAMOtherCharges').modal('show');
                    }
                }
            ])
        })

        $('#btnAMOtherChargesSave').click(function () {
            var txtAMOtherChargesID = $('#txtAMOtherChargesID');
            var txtAMOtherChargesSkuID = $('#txtAMOtherChargesSkuID');
            var txtAMOtherChargesUomID = $('#txtAMOtherChargesUomID');
            var txtAMOtherChargesQty = $('#txtAMOtherChargesQty');
            var selAMOtherChargesInvoicingType = $('#selAMOtherChargesInvoicingType');
            var otherchargesprice = $('[name="otherchargesprice"]:checked');
            var selAMOtherChargesIsRequired = $('#selAMOtherChargesIsRequired');
            var selAMOtherChargesIsActive = $('#selAMOtherChargesIsActive');

            if (txtAMOtherChargesSkuID.val() == '') {
                WebToast('Please select SKU', 'top');
                return;
            }

            if (otherchargesprice.length == 0) {
                WebToast('Please select price', 'top');
                return;
            }

            var data = {
                ID: $.trim(txtAMOtherChargesID.val()),
                SkuID: $.trim(txtAMOtherChargesSkuID.val()),
                UomID: $.trim(txtAMOtherChargesUomID.val()),
                Qty: 1, //$.trim(txtAMOtherChargesQty.val()),
                InvoicingType: $.trim(selAMOtherChargesInvoicingType.val()),
                PriceLevelID: otherchargesprice.val(),
                IsRequired: CInt2(selAMOtherChargesIsRequired.is(':checked')),
                IsActive: CInt2(selAMOtherChargesIsActive.is(':checked')),
            }

            $(this).prop('disabled', true).text('Saving ...');

            AjaxSend({
                url: 'AssetsManagement/SaveOtherCharges',
                data: { data: data },
                success: function (e) {
                    $('#btnAMOtherChargesSave').removeAttr('disabled').text('Save');

                    if (e.Results == 'saved') {
                        WebToast('Other charges has been successfully saved', 'top');
                        $('#popAMOtherCharges').modal('hide');
                        GetListOtherCharges();
                    } else {
                        NewMessageBox(e.Results);
                    }
                }
            })
        })

        $('#txtSearchOtherCharges').keydown(function (e) {
            if (e.which == 13) {
                GetListOtherCharges();
            }
        })

        $('#selOCStatus').change(function () {
            GetListOtherCharges();
        })

        $('#chkLostCardYes').click(function () {
            $('#dvCreateLostCardOptions').css('visibility', 'visible');
            $('#dvCreateLostCardOptions').css('display', 'block');
        });

        $('#chkLostCardNo').click(function () {
            $('#dvCreateLostCardOptions').css('visibility', 'hidden');
            $('#dvCreateLostCardOptions').css('display', 'none');
        });

        $('#btnAMAddDocument').click(function () {
            BrowsePopup('LibraryContractDesignerTemplate', 'Select Document', [
                {
                    Message: 'You can select multiple documents. Click OK when you\'re done.',
                    SelectMultiple: true,
                    AutoSearch: true,
                    DisplayText: [{
                        Column: 'TemplateName'
                    }],
                    Click: function (e) {
                        if(e.length > 0){
                            Contract.AddToModule(e, 'Asset Management', function () {
                                Contract.Populate('#divTableAMContract', 'Asset Management');
                            })
                        }
                    }
                }
            ], { Module: 'Asset Management'})
        })        

        function GetListOtherCharges() {
            var tbodyAMOtherCharges = $('#tbodyAMOtherCharges');
            tbodyAMOtherCharges.empty();

            AjaxSend({
                url: 'AssetsManagement/GetListOtherCharges',
                data: { keywords: $.trim($('#txtSearchOtherCharges').val()), Status: $('#selOCStatus').val() },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            tbodyAMOtherCharges.append($('<tr>')
                                    .data('data', vRows[i])
                                    .append($('<td>').text(i + 1 + '.'))
                                    .append($('<td>').text(vRows[i].ProductName))
                                    //.append($('<td>').text(vRows[i].Qty))
                                    .append($('<td>').text(vRows[i].UomName))
                                    .append($('<td>').text(vRows[i].InvoicingType))
                                    .append($('<td>').text((CBool(vRows[i].IsRequired) ? 'Yes' : 'No')))
                                    .append($('<td>').text(AccountingFormat(vRows[i].Price)))
                                    .append($('<td>').text(vRows[i].ModifiedBy))
                                    .append($('<td>').text((CBool(vRows[i].IsActive) ? 'Active' : 'Inactive')))
                                    .click(function () {
                                        var data = $(this).data('data');
                                        mOut.ClearOtherCharges();
                                        $('#popAMOtherCharges').modal('show');
                                        GetSkuDetails(data);
                                    })
                                )
                        }
                    } else {
                        tbodyAMOtherCharges.append($('<tr>')
                            .append($('<td colspan="10" class="text-center">').text('No result(s) found'))
                        )
                    }
                }
            })
        }

        function GetSkuDetails(data) {
            var tbodyOtherChargesRates = $('#tbodyOtherChargesRates');
            tbodyOtherChargesRates.empty();

            $('#txtAMOtherChargesID').val(CStr2(data.ID));
            $('#txtAMOtherChargesSkuID').val(data.ProductID);
            $('#txtAMOtherChargesSku').val('(' + data.ProductNo + ') ' + data.ProductName);
            $('#txtAMOtherChargesUomID').val(data.UomID);
            $('#lblOtherChargesSkuUom').text(data.UomName);
            $('#selAMOtherChargesIsRequired').prop('checked', (CInt2(data.IsRequired) || 0));
            $('#selAMOtherChargesIsActive').prop('checked', (CInt2(data.IsActive) || 1));

            workModel.GetAssetRates({
                data: { ProductID: data.ProductID },
                success: function (e) {
                    var vRows = e.OrgProductPriceData.Rows;
                    var excludePrices = ['01000012-0003-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', '01000012-0001-0000-0000-000000000000'] // Landed Cost, Vendor Price, Freight Cost

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            if ($.inArray(vRows[i].PriceLevelID, excludePrices) == -1) {
                                tbodyOtherChargesRates.append($('<tr>')
                                    .append($('<td>')
                                        .append($('<div class="form-check">')
                                            .append($('<label for="' + vRows[i].PriceLevelID + '" class="form-check-label">')
                                                .append($('<input type="radio" name="otherchargesprice" id="' + vRows[i].PriceLevelID + '" class="otherchargesprice form-check-input" data-iconpos="notext" value="' + vRows[i].PriceLevelID + '">'))
                                            )
                                            //.append($('<div>').text('asd'))
                                        )
                                    )
                                    .append($('<td>').text(vRows[i].PriceLevel))
                                    .append($('<td>').text(CStr2(GetStringObject('Unit.UnitMeasure', e))))
                                    .append($('<td>').text(AccountingFormat(vRows[i].Price)))
                                    .click(function () {
                                        $('[name="otherchargesprice"]').removeAttr('checked')//.checkboxradio;('refresh')
                                        $(this).find('input[type="radio"]').prop('checked', true)//.checkboxradio;('refresh')
                                    })
                                )
                            }
                        }

                        if (data.PriceLevelID != undefined) {
                            $('[value="' + data.PriceLevelID + '"]').prop('checked', true);
                        }

                        $('.otherchargesprice')//.checkboxradio;()//.checkboxradio;('refresh');
                    }

                }
            })
        }

        function GetInvoicingMethod() {
            settingsModel.GetInvoicingMethod({
                data: { keywords: CStr2($.trim($('#txtSearchInvoicingMethod').val())), status: $('#selIMStatus').val() },
                success: function (e) {
                    DisplayInvoicingMethod(e.Results);
                }
            })
        }

        function DisplayInvoicingMethod(e) {
            var tblDataTableInvoicingMethod = $('#tblDataTableInvoicingMethod');

            if ($.fn.DataTable.isDataTable(tblDataTableInvoicingMethod)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblDataTableInvoicingMethod.empty();
                dtInvoicingMethod.destroy();
            }

            dtInvoicingMethod = tblDataTableInvoicingMethod.DataTable({
                "data": e,
                "pageLength": 50,
                "columns": [
                    {
                        data: function (e) {
                            return e.MethodName;
                        }
                    },
                    {
                        data: function (e) {
                            var cat = [];

                            for (var i = 0; i < e.Category.length; i++) {
                                cat.push(e.Category[i].Category);
                            }

                            return cat.join(", ");
                        }
                    },
                    {
                        data: function (e) {
                            return e.ModifiedBy
                        }
                    },
                    {
                        data: function (e) {
                            return FormatDate(AssetMgmtWorkController.ConvertDate(e.LastUpdated), { Format: 'MMM dd, yyyy hh:mm a' });
                        }
                    },
                    {
                        data: function (e) {
                            return (CBool(e.IsActive) ? 'Active' : 'Inactive');
                        }
                    }
                ],
                "ordering": false,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": '_all' }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                initComplete: function () {
                    $('#tblDataTableInvoicingMethod').removeClass('dataTable');
                }
            });

            $('#tblDataTableInvoicingMethod tbody').on('click', 'tr', function () {
                var data = dtInvoicingMethod.row(this).data();
                if (data != undefined) {
                    $('#popInvoicingMethod').modal('show');
                    mOut.ClearInvoiceMethod();
                    invmethodID = data.ID;
                    arrCategory = JSON.parse(JSON.stringify(data.Category)); // clone

                    $('[name="radInvoiceMethod"][value="' + data.MethodType + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    $('[name="radMonthlyBillType"][value="' + (CStr2(data.MonthlyBillType) == '' ? 'postpaid' : data.MonthlyBillType) + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    $('#selInvMethodStatus').prop('checked', CInt2(data.IsActive));

                    if (data.MethodType == 'bynthday') {
                        $('#divMonthlyBillType').show();
                    } else {
                        $('#divMonthlyBillType').hide();
                    }

                    if (data.MethodType != 'uponcheckin' || data.MethodType != 'uponcheckout') {
                        $('[name="radInvoiceMethod"][value="' + data.MethodType + '"]').parent().find('input[type="number"]').val(CDec2(data.NoOfDays));
                    }

                    for (var i = 0; i < arrCategory.length; i++) {
                        $('[name="chbAssetsCategory"][value="' + arrCategory[i].Category + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    }
                }
            });
        }

        function SaveInvoicingMethod(isFromContinue) {
            var radInvoiceMethod = $('[name="radInvoiceMethod"]:checked');
            var noofdays = radInvoiceMethod.parent().find('input[type="number"]');
            var chbAssetsCategory = $('[name="chbAssetsCategory"]:checked');
            var radMonthlyBillType = $('[name="radMonthlyBillType"]:checked');
            var pData = {}, IncMethod;

            if (radInvoiceMethod.length == 0) {
                WebToast('Please select invoicing method', 'top');
                return;
            }

            if (radInvoiceMethod.val() == 'uponcheckin') {
                IncMethod = 'Invoice upon checkin';
            } else if (radInvoiceMethod.val() == 'uponcheckout') {
                IncMethod = 'Invoice upon checkout';
            } else if (radInvoiceMethod.val() == 'bydays') {
                IncMethod = 'Invoice every ' + noofdays.val() + ' day(s) from check-in date';
            } else {
                var suf = '';
                if (noofdays.val() == 1) {
                    suf = 'st';
                } else if (noofdays.val() == 2) {
                    suf = 'nd';
                } else if (noofdays.val() == 3) {
                    suf = 'rd';
                } else {
                    suf = 'th';
                }
                IncMethod = 'Invoice every ' + noofdays.val() + suf + ' day of the month';
            }

            if (radInvoiceMethod.val() == 'bydays' || radInvoiceMethod.val() == 'bynthofmonth') {
                if (CDec2(noofdays.val()) == 0) {
                    WebToast('Please enter no. of days', 'top');
                    noofdays.focus();
                    return;
                }
            }

            if (!isFromContinue) {

                if (chbAssetsCategory.length == 0 && CBool($('#selInvMethodStatus').is(':checked'))) {
                    WebToast('Please select at least one category of rentable assets.', 'top');
                    return;
                }

                if (arrCategory.length == 0) {
                    chbAssetsCategory.each(function () {
                        arrCategory.push({ Category: $(this).val(), State: 'add' });
                    })
                }
            }

            pData = {
                ID: invmethodID,
                MethodName: IncMethod,
                MethodType: radInvoiceMethod.val(),
                NoOfDays: CDec2(noofdays.val()),
                Category: arrCategory,
                TransferCategory: arrTransferCat,
                IsActive: CInt2($('#selInvMethodStatus').is(':checked')),
                MonthlyBillType: radMonthlyBillType.val()
            }

            settingsModel.SaveInvoicingMethod({
                data: pData,
                success: function (e) {
                    if ($.isArray(e)) {
                        $('#popInvoicingMethod').modal('hide');
                        $('#lblInvoiceMethodType').text(pData.MethodName);

                        var tbodyExistingIMCategory = $('#tbodyExistingIMCategory');
                        tbodyExistingIMCategory.empty();

                        for (var i = 0; i < e.length; i++) {
                            var method;

                            arrCategory = $.grep(arrCategory, function (v) {
                                if (v.Category != e[i].Category) {
                                    return v;
                                } else {
                                    $('[name="chbAssetsCategory"][value="' + v.Category + '"]').removeAttr('checked')//.checkboxradio;('refresh');
                                }
                            })
                            if (e[i].MethodType == 'uponcheckin') {
                                method = 'Invoice upon checkin';
                            } else if (e[i].MethodType == 'uponcheckout') {
                                method = 'Invoice upon checkout';
                            } else if (e[i].MethodType == 'bydays') {
                                method = 'Invoice every ' + e[i].NoOfDays + ' day(s) from check-in date';
                            } else {
                                method = 'Invoice every ' + e[i].NoOfDays + 'th day of the month';
                            }

                            tbodyExistingIMCategory.append($('<tr>')
                                .append($('<td>')
                                    .text(e[i].Category)
                                )
                                .append($('<td>')
                                    .text(method)
                                )
                                .append($('<td>')
                                    .append($('<button data-role="none" class="btn btn-danger">').append('<span class="bi-x"></span>'))
                                    .data('data', e[i])
                                    .click(function (v) {
                                        v.stopPropagation();
                                        v.preventDefault();
                                        var data = $(this).data('data');
                                        $('[name="chbAssetsCategory"][value="' + data.Category + '"]').prop('checked', true)//.checkboxradio;('refresh');
                                        arrTransferCat.push(data);
                                        $(this).parent().remove();
                                    })
                                )
                            )
                        }
                        $('#popInvoicingMethodCategoryExisting').modal('show');
                        return;
                    }

                    if (e == "saved") {
                        WebToast('Successfully saved', 'top');
                        GetInvoicingMethod();
                    } else {
                        WebToast(e, 'top');
                    }

                    NewMessageBoxClose(); // close current popup;
                }
            })
        }

        function LoadRequiredData() {
            GetAssetSetting();
            GetOtherCharges();
            GetAssetsCategory();            
        }

        function GetAssetsCategory() {
            AjaxSend({
                url: 'AssetsManagement/GetAssetsCategory',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var divAssetCategories = $('#divAssetCategories');
                    var rows = $('<div class="row">');
                    var ctrCol = 0;

                    divAssetCategories.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var data = vRows[i];

                            if (ctrCol == 2) {
                                ctrCol = 0;
                                rows = $('<div class="row">');
                            }

                            rows.append($('<div class="col-sm-6 mb-3">')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append($('<input class="chbAssetsCategory form-check-input" data-theme="c" type="checkbox" name="chbAssetsCategory" value="' + data.Category + '" data-iconpos="notext">')
                                                .data('data', data)
                                                .change(function (e) {
                                                    var data = $(this).data('data');
                                                    var ind;

                                                    if (arrCategory.length > 0) {
                                                        $.grep(arrCategory, function (e, i) {
                                                            if (e.Category == data.Category) {
                                                                ind = i;
                                                                return false;
                                                            }
                                                        })

                                                        if (ind == undefined) {
                                                            arrCategory.push({
                                                                Category: data.Category,
                                                                State: 'new'
                                                            })
                                                        } else {
                                                            arrCategory[ind].State = ($(this).is(':checked') ? 'edit' : 'remove');
                                                        }
                                                    }
                                                })
                                            ).append(data.Category)
                                        )
                                    )
                                )

                            ctrCol += 1;

                            if (ctrCol == 2 || (i + 1) == vRows.length) {
                                divAssetCategories.append(rows);
                            }
                        }

                        $('.chbAssetsCategory')//.checkboxradio;()//.checkboxradio;('refresh');
                    }
                }
            })
        }

        function GetAssetSetting(callback) {
            settingsModel.GetAssetSetting({
                success: function (e) {
                    mOut.Clear();
                    var defaulttime, time, mer, isAllowOutdatedRes, isAllowFutureDatedCheckIns, CreateKeyCardAfterCheckIn, MaxNumberOfGuestCards, AllowLostCardCreation, MaximumNumberOfLostCard, DefaultChargeOfLostCard;
                    var isDisplayProductNoInAssets;

                    if (e.LabelSetting.length > 0) {
                        mOut.LabelSetting = e.LabelSetting[0].DefaultValue;
                    } else {
                        mOut.LabelSetting = 'hotel industry';
                    }

                    $('[name="labelsetting"][value="' + mOut.LabelSetting + '"]').prop('checked', true)//.checkboxradio;('refresh');

                    if (e.CustomLableForAssets.length > 0) {
                        mOut.AssetsLabel = e.CustomLableForAssets[0].DefaultValue;
                        $('#txtLabelForAssets').val(mOut.AssetsLabel);
                    }

                    if (e.CustomLableForCustomers.length > 0) {
                        mOut.CustomersLabel = e.CustomLableForCustomers[0].DefaultValue;
                        $('#txtLabelForCustomers').val(mOut.CustomersLabel);
                    }

                    if (mOut.LabelSetting == 'real estate industry') {
                        mOut.AssetsLabel = 'Space';
                        mOut.CustomersLabel = 'Tenant';
                    }

                    if (e.DefaultCheckOutTime.length > 0) {
                        defaulttime = e.DefaultCheckOutTime[0].DefaultValue;
                        time = (e.DefaultCheckOutTime[0].DefaultValue).split(':');
                        mer = (e.DefaultCheckOutTime[0].DefaultValue).split(' ')[1];
                        $('#txtCheckOutTimeID').val(e.DefaultCheckOutTime[0].ID);
                    } else {
                        defaulttime = "12:00 PM";
                        time = ["12", "00 PM"];
                        mer = "PM";
                    }

                    mOut.DefaultCheckOutTime = {
                        Time: defaulttime,
                        Hour: time[0],
                        Minute: (time[1]).split(' ')[0],
                        Meredian: mer
                    };

                    $('#selCOhour').val(mOut.DefaultCheckOutTime.Hour);
                    $('#selCOminute').val(mOut.DefaultCheckOutTime.Minute);
                    $('#selCOmeredian').val(mOut.DefaultCheckOutTime.Meredian);

                    mElement.find('[name=selCheckInTimeHour]').val(e.CheckInTime.split(':')[0]);
                    mElement.find('[name=selCheckInTimeMinute]').val((e.CheckInTime.split(':')[1]).split(' ')[0]);
                    mElement.find('[name=selCheckInTimeMeridian]').val(e.CheckInTime.split(' ')[1].toUpperCase());

                    mElement.find('[name=selEarlyCheckInTimeHour]').val(e.EarlyCheckInTime.split(':')[0]);
                    mElement.find('[name=selEarlyCheckInTimeMinute]').val((e.EarlyCheckInTime.split(':')[1]).split(' ')[0]);
                    mElement.find('[name=selEarlyCheckInTimeMeridian]').val(e.EarlyCheckInTime.split(' ')[1].toUpperCase());

                    mElement.find('[name=selLateCheckOutTimeHour]').val(e.LateCheckOutTime.split(':')[0]);
                    mElement.find('[name=selLateCheckOutTimeMinute]').val((e.LateCheckOutTime.split(':')[1]).split(' ')[0]);
                    mElement.find('[name=selLateCheckOutTimeMeridian]').val(e.LateCheckOutTime.split(' ')[1].toUpperCase());

                    mEarlyCheckInProductID = e.EarlyCheckInProductID;
                    mElement.find('[name=txtEarlyCheckInChargeSku]').val(e.EarlyCheckInProductName);
                    mLateCheckOutProductID = e.LateCheckOutProductID;
                    mElement.find('[name=txtLateCheckOutChargeSku]').val(e.LateCheckOutProductName);
                    $('[name="radAssetPrice"][value="' + e.DisplayAssetPrice + '"]').prop('checked', true);
                    $('[name="radDownpayment"][value="' + CInt2(e.AssetMgmtDownpaymentRequired) + '"]').prop('checked', true);
                    mOut.IsDownpaymentRequired = CBool(e.AssetMgmtDownpaymentRequired);

                    //mElement.find('[name=]').val(
                    if (e.OutdatedReservationSettings.length > 0) {
                        isAllowOutdatedRes = CBool(e.OutdatedReservationSettings[0].DefaultValue);
                        $('#txtOutdatedResID').val(e.OutdatedReservationSettings[0].ID);
                        $('[name="radOutdatedRes"][value="' + e.OutdatedReservationSettings[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    } else {
                        isAllowOutdatedRes = false;
                        $('[name="radOutdatedRes"][value="0"]').prop('checked', true)//.checkboxradio;('refresh');
                    }

                    mOut.IsAllowOutdatedReservation = isAllowOutdatedRes;

                    if (e.IsAllowFutureDatedCheckIns.length > 0) {
                        isAllowFutureDatedCheckIns = CBool(e.IsAllowFutureDatedCheckIns[0].DefaultValue);
                        $('#txtFutureDatedCheckInsID').val(e.IsAllowFutureDatedCheckIns[0].ID);
                        $('[name="radFutureDated"][value="' + e.IsAllowFutureDatedCheckIns[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    } else {
                        isAllowFutureDatedCheckIns = false;
                        $('[name="radFutureDated"][value="0"]').prop('checked', true)//.checkboxradio;('refresh');
                    }

                    mOut.IsAllowFutureDatedCheckIns = isAllowFutureDatedCheckIns;

                    if (e.DisplayProductNoInAssets.length > 0) {
                        isDisplayProductNoInAssets = CBool(e.DisplayProductNoInAssets[0].DefaultValue);
                        $('#txtDisplayProductNoInAssetsID').val(e.DisplayProductNoInAssets[0].ID);
                        $('[name="radDisplayProductNoInAssets"][value="' + e.DisplayProductNoInAssets[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    } else {
                        isDisplayProductNoInAssets = false;
                        $('[name="radDisplayProductNoInAssets"][value="0"]').prop('checked', true)//.checkboxradio;('refresh');
                    }

                    mOut.IsDisplayProductNoInAssets = isDisplayProductNoInAssets;

                    if (e.MaxNumberOfGuestCards.length > 0) {
                        MaxNumberOfGuestCards = CInt2(e.MaxNumberOfGuestCards[0].DefaultValue);
                        $('#txtNoOfGuestCardsID').val(e.MaxNumberOfGuestCards[0].ID);
                        $('#txtMaxNumberOfGuestCards').val(CInt2(e.MaxNumberOfGuestCards[0].DefaultValue));

                        var txtMaxNumberOfGuestCards = document.getElementById("txtNoOfGuestCards");
                        txtMaxNumberOfGuestCards.max = CInt2(e.MaxNumberOfGuestCards[0].DefaultValue);

                    } else {
                        MaxNumberOfGuestCards = 1;
                        $('#txtMaxNumberOfGuestCards').val(1);
                    }

                    mOut.MaxNumberOfGuestCards = MaxNumberOfGuestCards;

                    //Create Key Card After Check In

                    if (e.CreateKeyCardAfterCheckIn.length > 0) {
                        if (CBool(e.CreateKeyCardAfterCheckIn[0].DefaultValue)) {
                            CreateKeyCardAfterCheckIn = CBool(e.CreateKeyCardAfterCheckIn[0].DefaultValue);
                            $('#txtCreateKeyCardAfterCheckInID').val(e.CreateKeyCardAfterCheckIn[0].ID);
                        } else {
                            CreateKeyCardAfterCheckIn = CBool(e.CreateKeyCardAfterCheckIn[0].DefaultValue);
                            $('#txtCreateKeyCardAfterCheckInID').val(e.CreateKeyCardAfterCheckIn[0].ID);
                        }
                        $('[name="radCreateKeyCardAfterCheckIn"][value="' + e.CreateKeyCardAfterCheckIn[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    } else {
                        CreateKeyCardAfterCheckIn = false;
                        $('#dvCreateLostCardOptions').css('visibility', 'hidden');
                        $('#dvCreateLostCardOptions').css('display', 'none');
                    }

                    mOut.CreateKeyCardAfterCheckIn = CreateKeyCardAfterCheckIn;

                    //Door Lock Settings

                    if (e.DoorLockSystem.length > 0) {
                        $('#txtDoorLockSystemID').val(e.DoorLockSystem[0].ID);
                        $('[name="radDoorLock"][value="' + e.DoorLockSystem[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    }

                    //mOut.DoorLockSystem = DoorLockSystem;

                    //Allow Lost Card Creation

                    if (e.AllowLostCardCreation.length > 0) {
                        if (CBool(e.AllowLostCardCreation[0].DefaultValue)) {
                            AllowLostCardCreation = CBool(e.AllowLostCardCreation[0].DefaultValue);
                            $('#txtAllowLostCardCreationID').val(e.AllowLostCardCreation[0].ID);
                            $('#dvCreateLostCardOptions').css('visibility', 'visible');
                            $('#dvCreateLostCardOptions').css('display', 'block');
                        } else {
                            AllowLostCardCreation = CBool(e.AllowLostCardCreation[0].DefaultValue);
                            $('#txtAllowLostCardCreationID').val(e.AllowLostCardCreation[0].ID);
                            $('#dvCreateLostCardOptions').css('visibility', 'hidden');
                            $('#dvCreateLostCardOptions').css('display', 'none');
                        }
                        $('[name="radLostCard"][value="' + e.AllowLostCardCreation[0].DefaultValue + '"]').prop('checked', true)//.checkboxradio;('refresh');
                    } else {
                        AllowLostCardCreation = false;
                        $('#dvCreateLostCardOptions').css('visibility', 'hidden');
                        $('#dvCreateLostCardOptions').css('display', 'none');
                        $('[name="radLostCard"][value="0"]').prop('checked', true)//.checkboxradio;('refresh');
                    }

                    mOut.AllowLostCardCreation = AllowLostCardCreation;

                    //Max Number of Lost Card

                    if (e.MaximumNumberOfLostCard.length > 0) {
                        MaximumNumberOfLostCard = CInt2(e.MaximumNumberOfLostCard[0].DefaultValue);
                        $('#txtMaxNumberOfLostCardsID').val(e.MaximumNumberOfLostCard[0].ID);
                        $('#txtMaxNumberOfLostCards').val(CInt2(e.MaximumNumberOfLostCard[0].DefaultValue));

                    } else {
                        MaximumNumberOfLostCard = 1;
                        $('#txtMaxNumberOfLostCards').val(1);
                    }

                    mOut.MaximumNumberOfLostCard = MaximumNumberOfLostCard;

                    //Max Number of Lost Card

                    if (e.DefaultChargeOfLostCard.length > 0) {
                        DefaultChargeOfLostCard = e.DefaultChargeOfLostCard[0].DefaultValue;
                        $('#txtChargeforLostCardsID').val(e.DefaultChargeOfLostCard[0].ID);
                        $('#selChargeforLostCards').val(e.DefaultChargeOfLostCard[0].DefaultValue);

                        try {
                            $('#selChargeforLostCards')//.selectmenu('refresh');
                        } catch (e) {
                            $('#selChargeforLostCards')//.selectmenu();
                        }
                    }

                    mOut.DefaultChargeOfLostCard = DefaultChargeOfLostCard;

                    GetListOtherCharges();

                    if ($.isFunction(callback)) { // use for updating grid date color
                        callback();
                    }

                    DisplayInvoicingMethod(e.InvoiceMethod);
                }
            })
        }

        function GetOtherCharges() {
            var data = { Status: 1 };

            AjaxSend({
                url: 'AssetsManagement/GetListOtherCharges',
                data: data,
                success: function (vData) {
                    var selCharges = $('#selChargeforLostCards');
                    selCharges.empty();

                    vRows = vData.Results.Rows;

                    for (var i = 0; i <= vRows.length - 1; i++) {
                        selCharges.append(new Option(vRows[i].ProductName + " - " + AccountingFormat(vRows[i].Price), vRows[i].ID, false, false));
                    }

                    try {
                        selCharges//.selectmenu('refresh');
                    } catch (e) {
                        selCharges//.selectmenu();
                    }
                }
            });
        }

        mOut.Clear = function () {
            $('#txtCheckOutTimeID').val('');
            $('#selCOhour').val('12');
            $('#selCOminute').val('00');
            $('#selCOmeredian').val('PM');
        }

        mOut.ClearInvoiceMethod = function () {
            invmethodID = '';
            arrCategory = [];
            arrTransferCat = [];
            $('[name="radInvoiceMethod"], .chbAssetsCategory, [name="radMonthlyBillType"]').removeAttr('checked')//.checkboxradio;('refresh');
            $('.txtinvmethoddays').val('');
            $('#selInvMethodStatus').prop('checked', 1);
            $('#divMonthlyBillType').hide();
        }

        mOut.ClearOtherCharges = function () {
            $('#txtAMOtherChargesID, #txtAMOtherChargesSkuID, #txtAMOtherChargesSku').val('');
            $('#txtAMOtherChargesQty').val('1');
            $('#selAMOtherChargesUom, #tbodyOtherChargesRates').empty();
            $('#selAMOtherChargesInvoicingType').val('Recurring');
            $('#lblOtherChargesSkuUom').text('');
            $('#selAMOtherChargesIsRequired').prop('checked', 0);
            $('#selAMOtherChargesIsActive').prop('checked', 1);
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
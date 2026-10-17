define([
    'common',
    GetPathFormat('/Scripts3/Browser/TransReceivingAndTransfers.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowstBin.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js'),
    GetPathFormat('/Scripts3/Browser/LibrarySupplier.js'),
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    GetPathFormat('/Scripts3/model/InventoryCount_Offline.js?crc='),
    'promise!/Scripts3/model/Receiving.js?crc=',
    '/Scripts/sources/Receiving/Bs_Cart.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    (OfflineMode ? GetPathFormat('/Scripts3/Offline/Browser/LibraryPeople.js?crc=') : ''),
    (OfflineMode ? GetPathFormat('/Scripts3/Offline/Browser/LibraryFowst.js?crc=') : '')
    ], function () {
        var OrgID = '';
        var ID = '';
        var IsNew = 1;
        var IsIntraOrganization = 0;
        var IsReceiving = 1;
        var Is3PL = 0;
        var CustomerID = '';
        var IsCustomer = 0;
        var IsFromTransferOutOrDraftReceiving = false;
        var IsSearchFromTransFerOut = false;
        var IsGetDataFromServer = false
        var IsAutomaticReceived = false;
        var IsShowQuantityOfStocks = false;
        var IsMainPage = true;
        var POID = '';
        var IsDraft = 1;

        var ThirdPartyOutlet = '';
        var ThirdPartyOutletDepartment = '';

        //either both (below comment) and to determine what button is click
        var isRecInTransFrom = true;

        //vars used for RecIn Or Trans From button
        var isThirdPartyRecInTransFrom = false;
        var RecInTransFromDefault = '';        
        var RecInTransFromOrgID = '';
        var RecInTransFromOrgName = '';
        var RecInTransFromDepartment = '';
        var RecInTransFromFowstID = '';
        var RecInTransFromFowstName = '';

        //vars used for RecFrom Or Trans To button
        var isThirdPartyRecFromTransTo = false;        
        var RecFromTransToDefault = '';        
        var RecFromTransToOrgID = '';
        var RecFromTransToOrgName = '';
        var RecFromTransToDepartment = '';        
        var RecFromTransToFowstID = '';        
        var RecFromTransToFowstName = '';
        var arrFowst = [];

        var InventoryCount = new App.model.InventoryCount_Offline();
        var AllowSRBadCount = CBool(Business.SystemDefaults.GetSystemDefault('AllowSRBadCount'));
        var ProductUnitMeasureByCategoryTable = $('#tblProductBinUnitMeasuresByCategory');
        var productUnitMeasureByCategoryDatatable;
        var arrCountItems = [], vRowsRawMats;

        var SelectedOrgName = undefined, SelectedOrgID = undefined, SelectedOutlet = undefined, SelectedOutletName = undefined, SelectedDepartment = undefined, isSelectedThirdParty = undefined;

        App.Deferred.done(function () {
            App.model.Receiving.isPageIdle = true;
        });
        App.Deferred.expect([
        '4UHHJWNPB9',
        '4VL3L6F7FZ',
        'KWXCETE5XN'
    ]);
        $(function () {

            //            if (window.location.toString().indexOf('#') < 0) {
            //                GetAllReceiving();
            //            }

            (function () {
                var pMessage;

                if (OfflineMode) {
                    Browser.OfflineMode = OfflineMode;
                    App.model.Receiving.OfflineMode = true;
                    App.model.Receiving_offline.FlushData();
                    App.model.Receiving_offline.DuplicateTable(); // use for saving DRAFT trans
                    //SalesMenu_offline().HandleEnforceZRead({
                    //    callback: function () { }
                    //});
                } else
                    App.Deferred.fulfill('KWXCETE5XN');

                if (!CBool(App.model.Receiving.OfflineMode)) {
                    AjaxSend({
                        url: '/Login3/TestCookies',
                        success: function (vData) {
                        },
                        error: function (vError) {
                            NewMessageBox("This feature requires internet connection jsXZ2LAXXRE3!",
                            "Requires Internet Connection",
                            null,
                            {
                                OnClose: function () {
                                    history.back();
                                }
                            }

                        );
                            return;
                        }

                    }); // END AjaxSend
                } // END if (!CBool(App.model.Receiving.OfflineMode)) {
            })();

            //set receive items by default
            $('#selReceiveOrTransfer').val('ReceiveItems');

            try {
                $('#selReceiveOrTransfer');//.selectmenu('refresh');
            } catch (e) {
                $('#selReceiveOrTransfer');//.selectmenu();
            }

            $('#lblReceivingPlace').text('Receive In: ');
            $('#lblSKUSource').text('Receive From: ');

            ClearFields();
            LoadRequiredData();

            //GetFowst();            

            if (IsMobile) {
                GetDefaultSoureOfSKU();
            }           
            
            startclock('#clockStock');
            startclock('#clockReceivingCart');
            //UpdateContent();

            var pPage = $.mobile.activePage;
            var pHeader = pPage.find("[data-role=header]");
            var pHeight = pHeader.height() + 10;

            //pPage.css('padding-top', pHeight + 'px');

            if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                pHeader.fixedtoolbar({ tapToggle: false,
                    updatePagePadding: false
                });
            App.Deferred.fulfill('R87MBPXGSQ');

            var m_Date = new Date();

            setMonthCollection("optMonthValue");
            setYearCollection("optYearValue");

            setMonthCollection("optMonthBLValue");
            setYearCollection("optYearBLValue");

            setMonthCollection("optMonthInvoiceValue");
            setYearCollection("optYearInvoiceValue");

            $('#optMonthValue').val(m_Date.getMonth() + 1);
            $('#optYearValue').val(m_Date.getFullYear());
            setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false);
            $('#optDayValue').val(m_Date.getDate());

            $('#optMonthBLValue').val(m_Date.getMonth() + 1);
            $('#optYearBLValue').val(m_Date.getFullYear());
            setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false);
            $('#optDayBLValue').val(m_Date.getDate());

            $('#optMonthInvoiceValue').val(m_Date.getMonth() + 1);
            $('#optYearInvoiceValue').val(m_Date.getFullYear());
            setDayCollection('optMonthInvoiceValue', 'optDayInvoiceValue', 'optYearInvoiceValue', false);
            $('#optDayInvoiceValue').val(m_Date.getDate());

            try {
                $('#optMonthValue');//.selectmenu('refresh');
                $('#optYearValue');//.selectmenu('refresh');
                $('#optDayValue');//.selectmenu('refresh');

                $('#optMonthBLValue');//.selectmenu('refresh');
                $('#optYearBLValue');//.selectmenu('refresh');
                $('#optDayBLValue');//.selectmenu('refresh');

                $('#optMonthInvoiceValue');//.selectmenu('refresh');
                $('#optDayInvoiceValue');//.selectmenu('refresh');
                $('#optYearInvoiceValue');//.selectmenu('refresh');
            }
            catch (e) {
                $('#optMonthValue');//.selectmenu();
                $('#optYearValue');//.selectmenu();
                $('#optDayValue');//.selectmenu();

                $('#optMonthBLValue');//.selectmenu();
                $('#optYearBLValue');//.selectmenu();
                $('#optDayBLValue');//.selectmenu();

                $('#optMonthInvoiceValue');//.selectmenu();
                $('#optDayInvoiceValue');//.selectmenu();
                $('#optYearInvoiceValue');//.selectmenu();
            }

            $('#btnNewReceiptsAndTransfers').click(function () {
                IsNew = 1;
                ClearFields();
            });


            $('.btnBack').click(function (e) {
                e.preventDefault();

                window.location = '/People/PeopleMenu';

            });

            $('#selReceivingPlace').change(function () {


                if ($('#selReceivingPlace').val() == 'Others') {
                    //e.preventDefault();
                    IsCustomer = 1;

                    Browser.LibraryOrganization.FilterCustomers = 1;
                    Browser.LibraryOrganization.Click = function (e) {
                        CustomerID = e.ID;
                        var CustomerName = e.CompanyName;
                        setTimeout(function () {
                            Browser.LibraryFowst.InputData.IsThirdPartyOrganization = 1;
                            Browser.LibraryFowst.InputData.ThirdPartyOrganizationID = CustomerID;
                            Browser.LibraryFowst.Click = function (e) {
                                $('#selReceivingPlace').val('Select');

                                CustomerID = e.ID;

                                $('#selReceivingPlace :selected').text(CustomerName + '(' + e.FowstName + ')');

                                try {
                                    $('#selReceivingPlace');//.selectmenu('refresh');
                                }
                                catch (e) {
                                    $('#selReceivingPlace');//.selectmenu();
                                }
                            };
                            Browser.LibraryFowst.Show();
                        }, 500);
                    };
                    Browser.LibraryOrganization.Show();
                }
                else {
                    IsCustomer = 0;
                    //show all fowst
                    $('#selIntraOrganization option').each(function () {
                        $(this).show();
                    });
                    //hide already selected fows

                    if ($('#selReceivingPlace').val() != '') {
                        $('#selIntraOrganization option[value=' + $('#selReceivingPlace').val() + ']').hide();
                    }

                    try {
                        $('#selIntraOrganization');//.selectmenu('refresh');
                    } catch (e) {
                        $('#selIntraOrganization');//.selectmenu();
                    }


                    sessionStorage.FowstID = $('#selReceivingPlace').val();
                }
            });

            $('#txtResaleableNo').keypress(function (e) {
                if (!e.keyCode == 8 && $('#txtResaleableNo').length > 1) {
                    $('#lblResalableCount').text(parseInt($('#lblResalableCount').text()) + 1 + ' characters left')
                    $('#txtResaleableNo').val($('#txtResaleableNo').val().substr($('#txtResaleableNo').length - 1))
                }
                else if (e.keyCode != 13) {
                    $('#lblResalableCount').text(parseInt($('#lblResalableCount').text()) - 1 + ' characters left')
                }
            });

            //$('#txtSearchReceiving').keypress(function (e) {
            //    if (e.keyCode == 13) {
            //        e.preventDefault();
            //        GetAllReceiving($('#txtSearchReceiving').val());
            //    }
            //});

            $('#btnReceivingCart').click(function () {
                $('#pageCart').pageshow();
            })

            $('#btnSearchTransportedBy').click(function (e) {
                BrowsePopup('LibraryPeople', 'Search Person Name', [
                    {
                        Offline: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            $('#txtTransportedBy').val(e.PeopleName);
                        }
                    }
                ])
            });

            $('#btnPlusOne').css('height', '75%');

            $('#btnInputItemDetails').click(function () {

                if ($('#selReceivingPlace').val() == '') {
                    if (CustomerID == '') {
                        if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
                            NewMessageBox("Please select the location to receive items.",
                        "Receive In",
                        [
                            { Text: 'Ok' }
                        ]
                    );
                            return;
                        } else {
                            NewMessageBox("Please select the source of items.",
                        "Transfer From",
                        [
                            { Text: 'Ok' }
                        ]
                    );
                            return;
                        }
                    }
                }

                if (($('#lblSourceOfSKU').data('ID') == undefined || $('#lblSourceOfSKU').data('ID') == null) && $('#selIntraOrganization').children('option:selected').val() == '') {

                    if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
                        NewMessageBox("Please select the source of items.",
                                "Receive From",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        return;
                    } else {
                        NewMessageBox("Please select the location to transfer items.",
                                "Transfer To",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        return;
                    }
                }

                //2019-10-02
                //if (!IsMainPage) {
                    if (IsReceiving == 1) {
                        if (!IsDate($('#optMonthValue').val() + '/' + $('#optDayValue').val() + '/' + $('#optYearValue').val())) {
                            NewMessageBox("Please put Document Date",
                                        "Error",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                            return;
                        }

                        if ($.trim($('#txtDocumentType').val()) == '') {
                            NewMessageBox("Please enter document type.",
                                    "Document Type",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                            return;
                        }

                        if ($.trim($('#txtDocumentNo').val()) == '') {
                            NewMessageBox("Please enter document number.",
                                    "Document Number",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                            return;
                        }

                    }
                    if ($.trim($('#txtTransportedBy').val()) == '') {
                        NewMessageBox("Please enter transported by.",
                            "Document Type",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        return;
                    }

                    //2019-10-02
                //    $('#btnDone').click();
                //    //SaveReceiving(false, null)
                //    return;
                //}

                // sessionStorage.FowstID = $('#selReceivingPlace').val(); // commented by Arvin 2021_05_08__10_11_09_5369
                sessionStorage.FowstID = RecInTransFromFowstID;
                $('#pageCart').pageshow();

                $("#lstReceiving").empty();
                $('#txtSearchItem').val('');
                $('#selRawMatShow').val('all');

                if (arrCountItems.length == 0) arrCountItems = []; //no need to clear kapag iedit lang ang receiving/transfer details

                AllowSRBadCount = CBool(Business.SystemDefaults.GetSystemDefault('AllowSRBadCount'));
                vRowsRawMats = InventoryCount.GetAllProducts('', '', true, 'all', 'ByInventoryCountItems', true).Rows;
                var listICCategories = $('#listICCategories').empty();
                var listICCategorySku = $('#listICCategorySku').empty();
                var pCategories = $.merge([{ Category: 'All Categories' }], App.model.Receiving_offline.GetAllProductCategories());
                var noofICItems = 0;
                console.log(pCategories);
                for (var i in pCategories) {
                    listICCategories.append($('<li style="flex: 0 0 auto;">')
                        .append($('<button data-role="none" class="btn btn-outline-dark rounded-pill btnCat">')
                            .text(pCategories[i].Category)
                            .data('data', pCategories[i])
                            .click(function () {
                                var dis = $(this), data = dis.data('data');

                                $('.btnCat').removeClass('active');
                                dis.addClass('active');

                                if (data.Category == 'All Categories') {
                                    $('[subpage="iccategories"]').pageshow();
                                } else {
                                    $('[catid="' + data.ID + '"]').pageshow();
                                }
                            })
                        )
                    )

                    if (pCategories[i].Category != 'All Categories') {
                        var parent = $('<div>');

                        parent.attr('catid', pCategories[i].ID);
                        parent.attr('subpage', 'iccategories');
                        parent.hide();

                        parent.append($('<div class="text-secondary border-primary p-2 pl-3 pr-3 font-weight-bold" style="background-color: #F5F7FA;">').text(pCategories[i].Category));

                        for (var s in pCategories[i].Sku) {
                            noofICItems += 1;
                            var skuData = pCategories[i].Sku[s], isCntData = skuData.Full != '' || skuData.Loose != '';
                            var uom = InventoryCount.GetStockUnitMeasures({ ProductID: skuData.ID }, true).Rows;
                            console.log(uom)
                            var itemRow = $('<div class="p-3 border bg-secondary-light listProduct" iscounted="0" status="' + skuData.Status + '">')
                                .attr('productid', skuData.ID)
                                .append($('<div class="row align-items-center">')
                                    .append($('<div class="col divProductInfo">')
                                        .append('<div class="text-dark font-weight-bold">' + skuData.ProductNo + '</div>')
                                        .append('<div class="text-secondary">' + skuData.ProductName + '</div>')
                                    )
                                    .append($('<div class="col-auto divUom">'))
                                    .append($('<div class="col-auto divStatus">')
                                        .append($('<span class="badge rounded-pill ' + (skuData.Status == 'Submitted' ? 'bg-success-transparent text-success' : (skuData.Status == 'Ready' ? 'bg-primary-transparent text-primary' : 'bg-secondary-transparent text-secondary')) + ' px-2 py-1 d-inline-flex align-items-center gap-1">')
                                            .append((skuData.Status == 'Submitted' ? '<i class="bi bi-check-circle-fill mr-2"></i> Submitted' : (skuData.Status == 'Ready' ? '<i class="bi bi-check-circle mr-2"></i> Ready' : '<i class="bi bi-circle mr-2"></i> Pending')))
                                        )
                                    )
                                )
                                .data('data', skuData)
                            //skuData.UnitMeasures = Sql.Execute('SELECT UnitMeasureID as ID, UnitMeasureName as UnitMeasure, Qty as Quantity, UomState, Count FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) + ' AND FowstID = ' + ReadyString(selectedFowstID) + ' AND ProductID = ' + ReadyString(skuData.ID) + ' AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy'))).Rows;
                            var divUom = itemRow.find('.divUom');

                            if (uom.length > 0) {
                                if ((isThirdPartyRecFromTransTo || uom.length == 1) &&
                                    (CBool(IsReceiving) && !CBool($('#radPSDShortLanded').is(':checked')) && !CBool($('#radPSDOverLanded').is(':checked')))) {
                                    itemRow.attr('noofuom', 1)
                                    var firstuom = uom[0];
                                    firstuom.ProductID = skuData.ID;
                                    PopulateCountField(divUom, firstuom, (isThirdPartyRecFromTransTo && CBool(IsReceiving) ? 'Full UOM' : 'Loose UOM'));

                                    if (CStr2(firstuom.Count) != '') {
                                        itemRow.removeClass('bg-secondary-light').addClass('bg-success-light');
                                        itemRow.attr('iscounted', 1)
                                        itemRow.find('.divStatus').empty().append($('<span class="badge rounded-pill bg-success-transparent text-success px-2 py-1 d-inline-flex align-items-center gap-1">')
                                            .append('<i class="bi bi-check-circle-fill mr-2"></i> Confirmed')
                                        )
                                    }
                                } else {
                                    itemRow.attr('noofuom', uom.length)
                                    for (var u in uom) {
                                        uom[u].ProductID = skuData.ID;
                                        PopulateCountField(divUom, uom[u], ((u == 0 && CBool(IsReceiving) || (u == 0 && uom.length > 1 && !CBool(IsReceiving))) ? 'Full UOM' : (u <= 1 ? 'Loose UOM' : '')));

                                        if (CStr2(uom[u].Count) != '') {
                                            itemRow.removeClass('bg-secondary-light').addClass('bg-success-light');
                                            itemRow.attr('iscounted', 1)
                                            itemRow.find('.divStatus').empty().append($('<span class="badge rounded-pill bg-success-transparent text-success px-2 py-1 d-inline-flex align-items-center gap-1">')
                                                .append('<i class="bi bi-check-circle-fill mr-2"></i> Confirmed')
                                            )
                                        }
                                    }
                                }
                            } else {
                                divUom.text('No assigned UOM')
                            }                                                        

                            parent.append(itemRow)
                        }

                        listICCategorySku.append(parent);
                    }
                }

                $('[subpage="iccategories"]').pageshow();
                $('.btnCat:eq(0)').addClass('active');

                DisplayRawMats(vRowsRawMats, function () {
                    if (IsFromTransferOutOrDraftReceiving) {
                        if (ReceivingSKU != undefined) {
                            Bs.Loading('show');
                            setTimeout(function () {
                                GetSystemBehaviorSettings();
                                //DisplayReceivingSKU(ReceivingSKU, IsAutomaticReceived, IsShowQuantityOfStocks, ID); commented by Arnie 1-17-2023 - NEW UI na gamit like sa Inventory Count
                                Bs.Loading('hide');
                            }, 200)
                        }
                    }
                });

                //if (CBool(IsReceiving)) {
                //    try {
                //        $('#btnDone div').text('View Receiving Cart');

                //    } catch (e) {
                //        //$('#btnDone .ui-btn-text');//.button();
                //    }
                //}
                //else {
                //    try {
                //        $('#btnDone div').text('View Transfer-Out Cart');
                //    }
                //    catch (e) {
                //        //$('#btnDone');//.button();
                //    }

                //}

                //try {
                //    //$('#btnDone');//.button('refresh');
                //    $('#lstItems');//.listview('refresh');
                //}
                //catch (e) {
                //    //$('#btnDone');//.button();
                //    $('#lstItems');//.listview();
                //}
            });            

            $('#selIsResaleable').change(function () {
                SetIsResaleable();
            })

            $('#selReceiveOrTransfer').change(function () {
                //ClearFields();
                UpdateDisplay();
            });

            $('#txtShipper').keypress(function (e) {
                if (e.keyCode == 13) {
                    GetShippers();
                }
            });

            $('#btnShipper').click(function () {
                GetShippers();
            })

            $('#selSourceOfSKU').change(function () {
                ChangeSourceOfSKU();
            });

            $('#selIntraOrganization').change(function (e) {
                if ($('#selIntraOrganization option:selected').val() == '') {
                    $('#lblSourceOfSKU').data('ID', null);
                }
                else {
                    data = { ID: $('#selIntraOrganization').children('option:selected').val() };
                    $('#lblSourceOfSKU').data(data);
                }
            });

            $('#optMonthValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false);
            });

            $('#optYearValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false);
            });

            $('#optMonthBLValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false);
            });

            $('#optYearBLValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false);
            });

            $('#rcChoiceYes').click(function (e) {
                $('#dv3pl').css('visibility', 'visible');
                $('#dv3pl').css('display', 'block');
                Is3PL = 1;
            })

            $('#rcChoiceNo').click(function (e) {
                $('#dv3pl').css('visibility', 'hidden');
                $('#dv3pl').css('display', 'none');
                Is3PL = 0;
            })

            $('#btnDone').click(function () {
                //if (CStr2($.trim($('#txtTransportedBy').val())) == '') {
                //    NewMessageBox("Please enter Transported By.",
                //           "Transported By",
                //            [
                //                { Text: 'Ok'
                //                }
                //            ]
                //            );
                //    return false;
                //}

                if (CStr2($.trim($('#lblSourceOfSKU').data('ID'))) == '') {
                    NewMessageBox("Please select source of SKU.",
                           "Source of SKU",
                            [
                                { Text: 'Ok'
                                }
                            ]
                            );
                    return false;
                }

                if (true) { // mShowWithQtyOnly - no need to show summary, by Arnie 09032026
                    PopupButtonList('', 'Save Options', [
                        {
                            HasImage: true,
                            Text: 'Save and Finalize Transaction',
                            Description: 'Save and send to head office. Transaction can be searched but not edited after saving.',
                            Click: function () {
                                SaveReceiving(false, null, 0);
                            }
                        },
                        {
                            HasImage: true,
                            Text: 'Save as Draft Transaction',
                            Description: 'Will not be sent to the head office. Transaction can still be searched and edited.',
                            Click: function () {
                                SaveReceiving(false, null, 1);
                            }
                        }
                    ], { AutoDismiss: true, TextAlign: 'center' })
                } else {
                    if (arrCountItems.length == 0) {
                        NewMessageBox('No items selected.');
                    } else
                        DisplayRawMats(vRowsRawMats, null, true);
                }
            });

            $('#btnScanTransferOutQrCode').click(function () {
                if (IsMobile) {
                    mobile.Scan({
                        Success: function (vData) {
                            if (IsGuid(vData.Value)) {
                                ID = vData.Value;
                                GetReceivingOrTransferOutInfo();
                                IsFromTransferOutOrDraftReceiving = true;
                            }
                        }
                    });
                } else {
                    NewMessageBox('This feature is for mobile device only')
                }

            });

            $('#txtSearchStockTransfer').keypress(function (e) {
                if (e.keyCode = 13) {
                    IsSearchFromTransFerOut = true;
                    SearchTransferOut();                    
                }
            });

            function SearchTransferOut(vIssueHistoryBack) {
                SearchReceivingTransfers(true, function (e) {
                    ID = e.ID;
                    IsGetDataFromServer = true
                    GetReceivingOrTransferOutInfo(e);
                    IsFromTransferOutOrDraftReceiving = true;
                }, {
                    IsReceiving: 0
                })
            }

            $('#btnRecordLookup').click(function () {
                //var options = [], firstopt;

                //if (CBool(IsReceiving)) {
                //    firstopt = {
                //        Text: 'Receiving',
                //        Description: 'View Finalized Receiving Transactions or Continue working on Draft Receiving Transactions.'
                //    }
                //} else {
                //    firstopt = {
                //        Text: 'Transfer Out',
                //        Description: 'View Finalized Transfer Out Transactions or Continue working on Draft Transfer Out Transactions.'
                //    }
                //    $('#lblRecordSearchType').html('');
                //    $('#lblRecordSearchDescription').html('')
                //}

                //firstopt.Click = function () {
                    POID = '';
                    SearchReceivingTransfers(false, function (e) {
                        ID = e.ID;
                        IsDraft = e.IsDraft;
                        IsGetDataFromServer = false;                        
                        GetReceivingOrTransferOutInfo(e);
                        IsFromTransferOutOrDraftReceiving = true;
                        IsNew = false;
                    }, {
                        IsReceiving: IsReceiving
                    })
                //}

                //options.push(firstopt);
                //options.push({
                //    Text: 'Finalized PO',
                //    Description: 'Receive Finalized POs.',
                //    Click: function () {
                //        BrowsePopup('TransInvoices', 'Select P.O', [
                //            {
                //                Offline: true,
                //                AutoSearch: true,
                //                DisplayText: [
                //                    {
                //                        Label: 'Purchase Order No.',
                //                        Column: function (e) {
                //                            return e.MIN + '-' + (e.PurchaseOrderNo || '')
                //                        }
                //                    },
                //                    {
                //                        Label: 'Status',
                //                        Column: function (e) {
                //                            if (e.IsDraft == undefined)
                //                                e.IsDraft = 1;
                //                            return ((CBool(e.IsDraft)) ? 'Draft' : 'Finalized');
                //                        }
                //                    },
                //                    {
                //                        Label: 'Customer',
                //                        Column: 'CompanyName'
                //                    },
                //                    {
                //                        Label: 'Supplier',
                //                        Column: 'SupplierName'
                //                    },
                //                    {
                //                        Label: 'Date',
                //                        Column: function (e) {
                //                            return $.format.date(new Date(e.InvoiceDate), 'MMM dd, yyyy hh:mm a');
                //                        }
                //                    },
                //                    {
                //                        Label: 'FOWST',
                //                        Column: 'FOWSTName'
                //                    },
                //                    {
                //                        Label: 'Device',
                //                        Column: 'DeviceName'
                //                    },
                //                    {
                //                        Label: 'Total Value',
                //                        Column: function (e) {
                //                            return AccountingFormat(e.Total);
                //                        }
                //                    },
                //                    {
                //                        Label: 'SKU Count',
                //                        Column: 'SkuCount'
                //                    },
                //                    {
                //                        Label: 'Created By',
                //                        Column: 'EmployeeFullName'
                //                    }
                //                ],
                //                Click: function (e) {
                //                    POID = e.ID;
                //                    IsNew = 1;

                //                    $('#selReceivingPlace').val(e.FOWSTID);//.selectmenu('refresh');

                //                    $('#selSourceOfSKU').val('valThirdPartyVendor');//.selectmenu('refresh');
                //                    SourceOfSKUID = e.CustomerID;
                //                    SourceOfSKU = e.CompanyName;
                //                    $('#dvVendor').css('visibility', 'visible');
                //                    $('#dvVendor').css('display', 'block')
                //                    $('#dvIntraOrganization').css('visibility', 'hidden');
                //                    IsIntraOrganization = 0;

                //                    data = { ID: e.CustomerID };
                //                    $('#lblSourceOfSKU').val(SourceOfSKU)
                //                    $('#lblSourceOfSKU').data(data);

                //                    $('#txtDocumentType').val('PO');
                //                    $('#txtDocumentNo').val(CStr2(e.MIN) + '-' + e.PurchaseOrderNo);
                //                    $('#txtPONo').val(CStr2(e.MIN) + '-' + e.PurchaseOrderNo);

                //                    m_Date = new Date(e.InvoiceDate);

                //                    $('#optMonthValue').val(m_Date.getMonth() + 1)
                //                    $('#optYearValue').val(m_Date.getFullYear())
                //                    setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
                //                    $('#optDayValue').val(m_Date.getDate())

                //                    $('#optMonthValue');//.selectmenu('refresh');
                //                    $('#optYearValue');//.selectmenu('refresh');
                //                    $('#optDayValue');//.selectmenu('refresh');

                //                    var pData = {};

                //                    pData.PosMode = "so";
                //                    //end of additional

                //                    pData.CustomerID = sessionStorage.OrgID;
                //                    pData.OrderID = e.ID;
                //                    pData.FowstID = sessionStorage.FowstID;

                //                    vRetDataReceivingDetails = PosCart(pData);

                //                    ReceivingSKU = {};
                //                    ReceivingSKU.Rows = [];

                //                    isThirdPartyRecInTransFrom = ReadyGuid(e.CustomerID) != ReadyGuid(sessionStorage.OrgID);
                //                    RecInTransFromOrgID = e.CustomerID;
                //                    RecInTransFromOrgName = e.CompanyName;
                //                    RecInTransFromFowstID = e.CustomerFowstID;
                //                    RecInTransFromFowstName = e.CustomerFowst;

                //                    isThirdPartyRecFromTransTo = ReadyGuid(e.SupplierID) != ReadyGuid(sessionStorage.OrgID);
                //                    RecFromTransToOrgID = e.SupplierID;
                //                    RecFromTransToOrgName = e.SupplierName;
                //                    RecFromTransToFowstID = e.SupplierFowstID;
                //                    RecFromTransToFowstName = e.SupplierFowst;

                //                    for (var i = 0; i <= vRetDataReceivingDetails.Units.Rows.length - 1; i++) {
                //                        ReceivingSKU.Rows.push({
                //                            ID: vRetDataReceivingDetails.Units.Rows[i].ProductID,
                //                            ProductNo: vRetDataReceivingDetails.Units.Rows[i].ProductNo,
                //                            UnitMeasureGood: vRetDataReceivingDetails.Units.Rows[i].ItemID,
                //                            ActualQuantityGood: CDec2(vRetDataReceivingDetails.Units.Rows[i].ActualQuantity), // * CDec2(vRetDataReceivingDetails.Units.Rows[i].OrderedQuantity),
                //                            ProductName: vRetDataReceivingDetails.Units.Rows[i].ProductName,
                //                            QuantityGood: CDec2(vRetDataReceivingDetails.Units.Rows[i].OrderedQuantity),
                //                            Label: vRetDataReceivingDetails.Units.Rows[i].UnitMeasure
                //                        });
                //                    }

                //                    ReceivingSKU = ReceivingSKU.Rows;

                //                    IsFromTransferOutOrDraftReceiving = true;
                //                }
                //            }
                //        ], {
                //            CustomerID: sessionStorage.FowstID,
                //            PoMode: true,
                //            IsFinalizeOnly: true,
                //            IsFilterReceived: true
                //        })
                //    }
                //})
                if (CBool(IsReceiving)) {
                    options.push({
                        Text: 'Transfer Out',
                        Description: 'Receive items from Suppliers or FOWST.',
                        Click: function () {
                            IsSearchFromTransFerOut = true;
                            SearchTransferOut(true);
                        }
                    })
                }                
                
                //PopupButtonList('', 'Record Search Options', options, {
                //    TextAlign: 'center'
                //})
            });

            $('#btnRORecordSearch').click(function () {
                POID = '';
                SearchReceivingTransfers(false, function (e) {
                    ID = e.ID;
                    IsDraft = e.IsDraft;                    
                    GetReceivingOrTransferOutInfo();
                    IsFromTransferOutOrDraftReceiving = true;
                    IsNew = false;
                }, {
                    IsReceiving: IsReceiving
                })
            });

            $('#btnNewReceivingCart').click(function () {
                $('#pageMain').pageshow();
                IsDraft = 1;
                IsNew = 1;
            });

            $('#lblSourceOfSKU').keypress(function (e) {
                if (e.keyCode == 13) {
                    GetLibraryOrganization();
                }
            });

            $('#lblThirdPartyOutlet').keypress(function (e) {
                if (e.keyCode == 13 && CustomerID != '') {
                    GetThirdPartyOutlet(CustomerID);
                } else {
                    NewMessageBox("Please select vendor before adding an outlet.")
                }
            });

            $('#lblThirdPartyOutletDepartment').keypress(function (e) {
                if (e.keyCode == 13 && ThirdPartyOutlet != '') {
                    GetThirdPartyOutletDepartment();
                } else {
                    NewMessageBox("Please select outlet before adding a department.")
                }
            });

            $('#btnAddOutlet').click(function () {
                $('#panAddOutlet').modal('show');
            });
            
            $('#btnRecInTransFromPlace').click(function () {
                isRecInTransFrom = true;
                $('#txtTitlePopup').text($('#lblReceivingPlace').text().replace(/:/g, ''));
                if (RecInTransFromDefault == 'internal') {
                    $('#popReceivingTransferOption').modal('show');
                    if (isThirdPartyRecInTransFrom) {
                        $('#selOrganization').val('third');
                        RecInTransFromOrgID = RecInTransFromOrgID;
                        RecInTransFromOrgName = RecInTransFromOrgName;
                        GetOrgFowst(RecInTransFromOrgID, true);
                        $('#selOrganization option[value="third"]').text('3rd Party (' + RecInTransFromOrgName + ')');
                        $('#selOrganization option[value="anothird"]').css('display', 'block');
                    }
                    else {
                        $('#selOrganization').val(RecInTransFromOrgID);
                        RecInTransFromOrgID = $('#selOrganization').val();
                        RecInTransFromOrgName = $('#selOrganization').find('option:selected').text();
                        GetOrgFowst(RecInTransFromOrgID, false);
                        $('#selOrganization option[value="third"]').text('3rd Party');
                        $('#selOrganization option[value="anothird"]').css('display', 'hide');
                    }
                    $('#selReceivingPlaceOption').text(CStr2(RecInTransFromFowstName) || 'Please select');//.selectmenu('refresh');
                } else {
                    if (RecInTransFromOrgID != '') {
                        //if user already select a third party org, show popup instead browser search page
                        $('#popReceivingTransferOption').modal('show');
                        if (isThirdPartyRecInTransFrom) {
                            GetOrgFowst(RecInTransFromOrgID, true)
                            $('#selOrganization').val('third');
                            $('#selOrganization option[value="third"]').text('3rd Party (' + RecInTransFromOrgName + ')');
                        }
                        else {
                            GetOrgFowst(RecInTransFromOrgID, false);
                            $('#selOrganization').val(RecInTransFromOrgID);
                            $('#selOrganization option[value="third"]').text('3rd Party');
                        }
                        $('#selReceivingPlaceOption').text(CStr2(RecInTransFromFowstName) || 'Please select');//.selectmenu('refresh');
                    } else {
                        ThirdPartyOrg();
                    }                    
                }               

                if (RecInTransFromFowstID != '') {
                    if (IsMobile) {
                        GetDepartment(RecInTransFromFowstID);
                    }
                }
                $('#selOrganization');//.selectmenu('refresh');
                $('#selDepartment').val(RecInTransFromDepartment);//.selectmenu('refresh');
                $('#msgOrganization, #msgOutlet').hide();
            })

            $('#btnRecFromTransToPlace').click(function () {
                isRecInTransFrom = false;
                $('#txtTitlePopup').text($('#lblSKUSource').text().replace(/:/g, ''));
                if (RecFromTransToDefault == 'internal') {
                    $('#popReceivingTransferOption').modal('show');
                    if (isThirdPartyRecFromTransTo) {
                        $('#selOrganization').val('third');//.selectmenu('refresh');
                        RecFromTransToOrgID = RecFromTransToOrgID;
                        RecFromTransToOrgName = RecFromTransToOrgName;
                        GetOrgFowst(RecFromTransToOrgID, true);
                        $('#selOrganization option[value="third"]').text('3rd Party (' + RecFromTransToOrgName + ')');
                    }
                    else {
                        $('#selOrganization').val(RecFromTransToOrgID);
                        RecFromTransToOrgID = $('#selOrganization').val();
                        RecFromTransToOrgName = $('#selOrganization').find('option:selected').text();
                        GetOrgFowst(RecFromTransToOrgID, false);
                        $('#selOrganization option[value="third"]').text('3rd Party');
                    }
                    $('#selReceivingPlaceOption').text(CStr2(RecFromTransToFowstName) || 'Please select');//.selectmenu('refresh');
                } else {
                    if (RecFromTransToOrgID != '') {
                        //if user already select a third party org, show popup instead browser search page
                        $('#popReceivingTransferOption').modal('show');
                        if (isThirdPartyRecFromTransTo) {
                            GetOrgFowst(RecFromTransToOrgID, true)                            
                            $('#selOrganization option[value="third"]').text('3rd Party (' + RecFromTransToOrgName + ')');
                            $('#selOrganization').val('third');
                        }
                        else {
                            GetOrgFowst(RecFromTransToOrgID, false);
                            $('#selOrganization').val(RecFromTransToOrgID);
                            $('#selOrganization option[value="third"]').text('3rd Party');
                        }
                        $('#selReceivingPlaceOption').text(CStr2(RecFromTransToFowstName) || 'Please select');//.selectmenu('refresh');
                    } else {
                        ThirdPartyOrg();
                    }
                }                

                if (RecFromTransToFowstID != '') {
                    if (IsMobile) {
                        GetDepartment(RecFromTransToFowstID);
                    }
                }
                $('#selOrganization');//.selectmenu('refresh');
                $('#selDepartment').val(RecFromTransToDepartment);//.selectmenu('refresh');
                $('#msgOrganization, #msgOutlet').hide();
            })

            $('#selOrganization').change(function (e) {
                if ($(this).val() == "third" || $(this).val() == "anothird") {
                    $(this).find('option:first').attr('selected', 'selected');
                    $(this);//.selectmenu('refresh');                    
                    $('#popReceivingTransferOption').modal('hide');
                    $('#selOrganization option[value="anothird"]').css('display', 'block');
                    ThirdPartyOrg();
                } else {                   
                    IsCustomer = 0;
                    isSelectedThirdParty = false;
                    SelectedOrgID = ($(this).val() == '' ? undefined : $(this).val());
                    SelectedOrgName = $(this).find('option:selected').text();
                    GetOrgFowst($(this).val(), false) // load fowst of selected organization
                    $('#selOrganization option[value="third"]').text('3rd Party');                    
                    $('#selOrganization option[value="anothird"]').css('display', 'none');                    
                }
                $('#selReceivingPlaceOption').text('Please select');
                PopupReposition();
            })

            $('#selReceivingPlaceOption')
                .click(function () {
                    PopupButtonList('', 'Select Outlet', arrFowst, { WithSearchbox: true });
                })
            //    .change(function () {
            //    SelectedOutlet = ($(this).val() == '' ? undefined : $(this).val());
            //    SelectedOutletName = $(this).find('option:selected').text();
            //    PopupReposition();
            //    if (IsMobile) {
            //        GetDepartment($(this).val()) // get department or bin of selected fowst
            //    }                
            //})

            $('#selDepartment').change(function () {
                SelectedDepartment = $(this).val();
                PopupReposition();
            })

            $('#btnReceivingTransferOptionOk').click(function () {
                if (isRecInTransFrom) {
                    //Receive In or Transfer From
                    RecInTransFromOrgID = (SelectedOrgID == undefined ? RecInTransFromOrgID : SelectedOrgID);
                    RecInTransFromOrgName = (SelectedOrgName == undefined ? RecInTransFromOrgName : SelectedOrgName);
                    RecInTransFromFowstID = SelectedOutlet;
                    RecInTransFromFowstName = (SelectedOutletName == undefined ? RecInTransFromFowstName : SelectedOutletName);
                    RecInTransFromDepartment = (SelectedDepartment == undefined ? RecInTransFromDepartment : SelectedDepartment);
                    isThirdPartyRecInTransFrom = (isSelectedThirdParty == undefined ? isThirdPartyRecInTransFrom : isSelectedThirdParty);

                    if (RecInTransFromOrgID == '') {
                        $('#msgOrganization').pageshow();
                        return
                    }

                    if (isThirdPartyRecInTransFrom == false) {
                        //FOWST or outlet is required when it is not a third party org
                        if (RecInTransFromFowstID == undefined || RecInTransFromFowstID == '') {
                            $('#msgOutlet').pageshow();
                            return
                        }
                        $('#btnRecInTransFromPlace').text(RecInTransFromOrgName + ' (' + RecInTransFromFowstName + ')');//.button('refresh');
                    } else {
                        $('#btnRecInTransFromPlace').text('3rd Party (' + RecInTransFromOrgName + ')');//.button('refresh')
                    }
                } else {
                    //Receive From or Transfer To
                    var data;
                    RecFromTransToOrgID = (SelectedOrgID == undefined ? RecFromTransToOrgID : SelectedOrgID);
                    RecFromTransToOrgName = (SelectedOrgName == undefined ? RecFromTransToOrgName : SelectedOrgName);
                    RecFromTransToFowstID = SelectedOutlet;
                    RecFromTransToFowstName = (SelectedOutletName == undefined ? RecFromTransToFowstName : SelectedOutletName);
                    RecFromTransToDepartment = (SelectedDepartment == undefined ? RecFromTransToDepartment : SelectedDepartment);
                    isThirdPartyRecFromTransTo = (isSelectedThirdParty == undefined ? isThirdPartyRecFromTransTo : isSelectedThirdParty);

                    if (RecFromTransToOrgID == '') {
                        $('#msgOrganization').pageshow();
                        return
                    }

                    if (isThirdPartyRecFromTransTo == false) {
                        //FOWST or outlet is required when it is not a third party org
                        if (RecFromTransToFowstID == undefined || RecFromTransToFowstID == '') {
                            $('#msgOutlet').pageshow();
                            return
                        }
                        $('#btnRecFromTransToPlace').text(RecFromTransToOrgName + ' (' + RecFromTransToFowstName + ')');//.button('refresh');
                        data = { ID: SelectedOutlet, Name: SelectedOutletName };
                    } else {
                        $('#btnRecFromTransToPlace').text('3rd Party (' + RecFromTransToOrgName + ')');//.button('refresh')
                        data = { ID: RecFromTransToOrgID, Name: RecFromTransToOrgName };
                    }

                    $('#lblSourceOfSKU').data(data);
                }

                //var data;
                //if(IsReceiving == 1){
                //    data = { ID: RecFromTransToOrgID, Name: RecFromTransToOrgName };
                //} else {
                //    data = { ID: RecInTransFromOrgID, Name: RecInTransFromOrgName };
                //}
                //$('#lblSourceOfSKU').data(data);
                $('#popReceivingTransferOption').modal('hide');
                //$('#btnReceivingTransferOptionCancel').click(); do not use this event because it will go back to prev page :)

                SelectedOrgID = undefined;
                SelectedOrgName = undefined;
                SelectedOutlet = undefined;
                SelectedOutletName = undefined;
                SelectedDepartment = undefined;
                isSelectedThirdParty = undefined;
            })

            $('#btnReceivingTransferOptionCancel').click(function () {
                SelectedOrgID = undefined;
                SelectedOrgName = undefined;
                SelectedOutlet = undefined;
                SelectedOutletName = undefined;
                SelectedDepartment = undefined;
                isSelectedThirdParty = undefined;
            })
            
            $('#btnNewCart').click(function () {
                if(RecInTransFromOrgID != ''){
                    NewMessageBox('Are you sure you want to create new cart?', 'New Cart', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                ClearFields();
                            }
                        }, { Text: 'No' }
                    ])
                } else { ClearFields() }
            })
        });

    function PopulateCountField(container, data, label) {
        var count = '';

        if (IsGuid(ID)) {
            count = Sql.ExecuteScalar('SELECT Count FROM cache.TransReceivingStockCountPerUom WHERE ReceivingID = ' + ReadyString(ID) +
                '\n AND ProductID = ' + ReadyString(data.ProductID) +
                '\n AND UnitMeasureID = ' + ReadyString(data.ID))
        }

        data.Count = count;

        container.append('<div class="w-100 text-center text-secondary" style="font-size:12px;">' + label + '</div>')
            .append($('<div class="input-group mb-1">')
                .append($('<input data-role="none" type="number" class="form-control txtCount" value="' + count +'" style="width:80px;">')
                    .data('data', data)
                    .keyup(function () {
                        var parent = $(this).parents().closest('.listProduct'), row = parent.find('.row'), status = row.find('.divStatus').empty(), isNoCount = true;

                        parent.find('.txtCount').each(function () {
                            if ($.trim($(this).val()) != '') {
                                isNoCount = false;
                            }
                        })

                        if (isNoCount) {
                            parent.removeClass('bg-success-light').addClass('bg-secondary-light');
                            parent.attr('iscounted', 0);
                            status.append($('<span class="badge rounded-pill bg-secondary-transparent text-secondary px-2 py-1 d-inline-flex align-items-center gap-1">')
                                .append('<i class="bi bi-circle mr-2"></i> Pending')
                            )
                        } else {
                            $(this).data('data').Count = $(this).val();
                            parent.removeClass('bg-secondary-light').addClass('bg-success-light');
                            parent.attr('iscounted', 1);
                            status.append($('<span class="badge rounded-pill bg-success-transparent text-success px-2 py-1 d-inline-flex align-items-center gap-1">')
                                .append('<i class="bi bi-check-circle-fill mr-2"></i> Confirmed')
                            )
                        }
                    })
                )
                .append('<span class="input-group-text justify-content-center" style="font-size:12px; width:80px;">' + data.UnitMeasure +'</span>')
            )            
    }

        var tblRawMatItemsDT, vRowsAllRawMats;
        var mShowWithQtyOnly = false;
        function DisplayRawMats(data, callback, vWithQty) {
            var tblRawMatItems = $('#tblRawMatItems');            

            if ($.fn.DataTable.isDataTable(tblRawMatItems)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblRawMatItems.empty();
                tblRawMatItemsDT.destroy();
            }
            mShowWithQtyOnly = CBool(vWithQty);
            if (true) { //mShowWithQtyOnly - no need to show summary, by Arnie 09032026
                data = $.grep(data, function(vItem) {
                    var pResult = $.grep(arrCountItems, function(vItemWithQty) {
                        if ((ReadyGuid(vItem.ID) == ReadyGuid(vItemWithQty.ID))
                            && (ReadyGuid(vItem.ID) == ReadyGuid(vItemWithQty.ID)))
                            return true;
                    });
                    if (pResult.length > 0)
                        return true;
                });
                if (IsReceiving)
                    $('#btnDone div').text('Save Receiving Cart');
                else
                    $('#btnDone div').text('Save Transfer Out Cart');
                //$('.summary').removeClass('d-none').show();
                //$('.not-summary').hide();
            } else {
                //if (IsReceiving)
                //    $('#btnDone div').text('View Receiving Cart');
                //else
                //    $('#btnDone div').text('View Transfer Out Cart');
                //$('.summary').hide();
                //$('.not-summary').show();
            }

            if (ReceivingSKU != undefined) arrCountItems = ReceivingSKU;

            var isShortLanded = CBool($('#radPSDShortLanded').is(':checked'))
            var isOverLanded = CBool($('#radPSDOverLanded').is(':checked'))

            tblRawMatItemsDT = tblRawMatItems.DataTable({
                pageLength: 25,
                "ordering": false,
                "data": data,
                "initComplete": function () {
                    $('#tblRawMatItems').removeClass('dataTable').css('width', '100%');

                    if (callback != undefined) callback();

                    $('#txtSearchRawMat').unbind('keyup').keyup(function () {
                        tblRawMatItemsDT.search($(this).val()).draw();
                    })

                    $('#selRawMatShow').unbind('change').change(function () {
                        if ($(this).val() == 'all') {
                            //$.fn.dataTable.ext.search.pop();
                            DisplayRawMats(vRowsRawMats);
                        } else {
                            DisplayRawMats(arrCountItems);
                            //$.fn.dataTable.ext.search.push(
                            //    function (settings, data, dataIndex) {
                            //        return $(tblRawMatItemsDT.row(dataIndex).node()).hasClass('counted');
                            //    }
                            //);
                        }
                        
                        //tblRawMatItemsDT.draw();
                    })
                },
                rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
                    function onblur(dis) {
                        var isExist = false, index = null;
                        var tr = dis.parent().closest('tr'), rowData = dis.data('data');
                        var valgoodcount = CDec2(tr.find('input.goodcount').val()), valbadcount = CDec2(tr.find('input.badcount').val());

                        $.grep(arrCountItems, function (e, i) {
                            if (e.ID == rowData.ID) {
                                isExist = true;
                                index = i;
                                return false;
                            }
                        })

                        rowData = {
                            ID: rowData.ID,
                            UnitMeasureID: rowData.UnitMeasureID,
                            ActualQuantity: CDec2(tr.find('button').attr('actualqty')),
                            BadOrderReason: rowData.BadOrderReason || '',
                            AdditionalInfo: "",
                            IsResaleable: 0,
                            ProductName: rowData.ProductName,
                            ProductNo: rowData.ProductNo,
                            ProductSerials: rowData.ProductSerials || [],
                            UnitMeasure: rowData.UnitMeasure
                        }

                        if (valgoodcount == '' && valbadcount == '') {
                            if (isExist) {
                                arrCountItems.splice(index, 1);
                                tr.removeClass('counted');
                            }
                        } else {
                            if (isExist) {
                                arrCountItems[index].ActualQuantity = rowData.ActualQuantity;
                                arrCountItems[index].UnitMeasure = rowData.UnitMeasure;
                                arrCountItems[index].UnitMeasureID = rowData.UnitMeasureID;
                                arrCountItems[index].Quantity = valgoodcount;
                                arrCountItems[index].QuantityBad = valbadcount;
                            } else {
                                rowData.Quantity = valgoodcount;
                                rowData.QuantityBad = valbadcount;
                                arrCountItems.push(rowData);
                            }
                            tr.addClass('counted');
                        }

                        //console.log(arrCountItems)
                    }

                    var btn = $(row).find('button'), curUom = '';
                    var inpgoodcount = $(row).find('input.goodcount'), currentGoodCount = '';
                    var inpbadcount = $(row).find('input.badcount'), currentBadCount = '';

                    inpgoodcount.data('data', data);
                    inpbadcount.data('data', data);

                    $.grep(arrCountItems, function (e, i) {
                        if (e.ID == data.ID) {
                            arrCountItems[i] = $.extend(arrCountItems[i], {
                                ID: e.ID,
                                ProductName: e.ProductName,
                                UnitMeasureID: e.UnitMeasureGood,
                                ActualQuantity: e.ActualQuantityGood,
                                Quantity: CDec2(e.Quantity),
                                QuantityBad: CDec2(e.QuantityBad),
                                BadOrderReason: '',
                                AdditionalInfo: '',
                                IsResaleable: 0,
                                ProductSerials: [],
                                UnitMeasure: e.UnitMeasure
                            })

                            //if (e.Quantity == undefined && e.QuantityBad == undefined) {
                            //    e.Quantity = e.QuantityGood;
                            //    e.QuantityBad = e.QuantityDamaged;
                            //}
                            currentGoodCount = arrCountItems[i].Quantity;
                            currentBadCount = arrCountItems[i].QuantityBad;
                            curUom = arrCountItems[i].UnitMeasure;
                            return false;
                        }
                    })

                    if (curUom != '') btn.text(curUom);
                    inpgoodcount.val(currentGoodCount);
                    inpbadcount.val(currentBadCount);

                    if (currentGoodCount != 0 || currentBadCount != 0) {
                        $(row).addClass('counted');
                    }

                    inpgoodcount.unbind('blur').blur(function () {
                        onblur($(this));
                    })

                    inpbadcount.unbind('blur').blur(function () {
                        onblur($(this));
                    })
                },
                "columns": [
                    {
                        "data": function (data) {
                            return data.ProductNo + ' <br> ' + data.ProductName
                        }
                    },
                    {
                        "data": function (data) {
                            return "<button data-role='none' actualqty='" + data.ActualQuantityGood + "' maxactualqty='" + data.ActualQuantityGood + "' maxuom='" + data.UnitMeasureID + "' maxuomname='" + data.UnitMeasure + "' class='btn " + (CInt2(data.NoOfUnitMeasure) > 1 && (!CBool(GetUrlParameters().IsReceiving) || isShortLanded || isOverLanded) ? 'btn-warning' : 'btn-success') + " btn-block'>" + data.UnitMeasure + "</button>";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            var btn = $(td).find('button');                            

                            rowData.RowIndex = row;
                            rowData.MaxUom = btn.attr('maxuom');
                            rowData.MaxUomName = btn.attr('maxuomname');
                            rowData.MaxActualQuantity = btn.attr('maxactualqty');
                            
                            btn.data('data', rowData).unbind('click').click(function () {
                                //remove popup for stock receiving para dili maconfuse ang crew 10/18/2023

                                if (!CBool(IsReceiving) || isShortLanded || isOverLanded) {
                                    var data = $(this).data('data');
                                    var actualqty = CDec2(data.ActualQuantityGood), defaultUnitMeasureID = CStr2(data.MaxUom);
                                    //console.log(data)
                                    if (CInt2(data.NoOfUnitMeasure) > 1) {
                                        getAllProductsInBinUnitMeasuresByCategory(data.ID);

                                        $('#lblProductBinUnitMeasuresLabelByCategory').text(data.ProductName);

                                        $('#popProductBinUnitMeasuresByCategory').modal('show');

                                        $('#btnDoneProductBinUnitMeasuresByCategory').unbind('click').click(function () {

                                            //convert values to base unit
                                            var TotalGoodCount, TotalBadCount, curSelectedUom;

                                            if (productUnitMeasureByCategoryDatatable.data().count() > 0) {
                                                var cntSelectedUom = 0;

                                                $('#tblProductBinUnitMeasuresByCategory tbody tr').each(function () {
                                                    var dis = $(this);
                                                    var txtGoodCount = CStr2(dis.find('.goodcount').val());
                                                    var txtBadCount = CStr2(dis.find('.badcount').val());

                                                    if (txtGoodCount != '' || txtBadCount != '') {
                                                        cntSelectedUom += 1;
                                                        curSelectedUom = dis.attr('uomid');
                                                    }
                                                })

                                                if (cntSelectedUom >= 1 && defaultUnitMeasureID != curSelectedUom) {
                                                    data.UnitMeasure = CStr2(data.MinUnitMeasure)
                                                    data.UnitMeasureID = CStr2(data.MinUnitMeasureID)
                                                    data.ActualQuantity = CDec2(data.MinActualQuantityGood)
                                                } else {
                                                    data.UnitMeasure = data.MaxUomName;
                                                    data.UnitMeasureID = data.MaxUom;
                                                    data.ActualQuantity = CDec2(data.MaxActualQuantity);
                                                }

                                                $('#tblProductBinUnitMeasuresByCategory tbody tr').each(function () {
                                                    var dis = $(this);
                                                    var txtGoodCount = CStr2(dis.find('.goodcount').val());
                                                    var txtBadCount = CStr2(dis.find('.badcount').val());
                                                    var qty = dis.data('data').Quantity

                                                    if (txtGoodCount != '' || txtBadCount != '') {
                                                        UnitMeasureID = dis.attr('uomid');

                                                        if (txtGoodCount != '') {
                                                            if (cntSelectedUom >= 1 && defaultUnitMeasureID != curSelectedUom) {
                                                                TotalGoodCount = CDec2(TotalGoodCount) + (CDec2(txtGoodCount) * qty);
                                                            } else if (cntSelectedUom == 1) {
                                                                TotalGoodCount = CDec2(TotalGoodCount) + (CDec2(txtGoodCount));
                                                            }
                                                        }

                                                        if (txtBadCount != '') {
                                                            if (cntSelectedUom >= 1 && defaultUnitMeasureID != curSelectedUom) {
                                                                TotalBadCount = CDec2(TotalBadCount) + (CDec2(txtBadCount) * qty);
                                                            } else if (cntSelectedUom == 1) {
                                                                TotalBadCount = CDec2(TotalBadCount) + (CDec2(txtBadCount));
                                                            }
                                                        }
                                                    }
                                                })

                                                btn.text(data.UnitMeasure);
                                                btn.attr('actualqty', data.ActualQuantity);

                                                //display value to main table
                                                $('#tblRawMatItems tbody tr').each(function () {
                                                    if (tblRawMatItemsDT.row($(this)).index() == data.RowIndex) {
                                                        if (TotalGoodCount != undefined) $(this).find('.goodcount').val(TotalGoodCount);
                                                        if (TotalBadCount != undefined) $(this).find('.badcount').val(TotalBadCount);
                                                        $(this).find('.goodcount').blur(); //para ma-add sa array for saving ang data
                                                        return false;
                                                    }
                                                })
                                            }

                                            $('#popProductBinUnitMeasuresByCategory').modal('hide');
                                        });
                                    }
                                }
                                            
                            })
                        }
                    },
                    {
                        "data": function (data) {
                            return "<input type=number data-role='none' class='form-control goodcount' />";
                        }, width: '150px',
                        createdCell: function (td, cellData, rowData, row, col) {
                            var input = $(td).find('input');

                            rowData.RowIndex = row;
                            rowData.MaxUom = rowData.UnitMeasureID;
                            rowData.MaxUomName = rowData.UnitMeasure;
                            rowData.MaxActualQuantity = rowData.ActualQuantityGood;

                            if (CBool(IsReceiving)) {
                                input.data('data', rowData).unbind('keyup').keyup(function () {
                                    var data = $(this).data('data');

                                    if (CDec2($(this).val()) > CDec2(data.MaxQtyToBeRcvTfer) && CDec2(data.MaxQtyToBeRcvTfer) > 0) {
                                        NewMessageBox('You have exceeded the maximum allowed quantity for this product. The maximum allowable quantity for this product is ' + data.MaxQtyToBeRcvTfer + ' ' + data.MaxUomName + '.', data.ProductName);
                                        $(this).val('');
                                        return;
                                    }
                                })
                            }                            
                        }
                    },
                    {
                        visible: AllowSRBadCount,
                        "data": function (data) {
                            return "<input type=number data-role='none' class='form-control badcount' />"
                        }, width: '150px'
                    }
                ],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
            });
        }

        function getAllProductsInBinUnitMeasuresByCategory(ProductID) {
            var data = {
                ProductID: ProductID
            }

            var mData = InventoryCount.GetStockUnitMeasures(data, true);

            DisplayAllProductsInBinUnitMeasuresByCategory(mData.Rows);
        }

        function DisplayAllProductsInBinUnitMeasuresByCategory(vData) {
            if ($.fn.DataTable.isDataTable(ProductUnitMeasureByCategoryTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                ProductUnitMeasureByCategoryTable.empty();
                productUnitMeasureByCategoryDatatable.destroy();
            }

            productUnitMeasureByCategoryDatatable = ProductUnitMeasureByCategoryTable.DataTable({
                "data": vData,
                "columns": [
                    { "data": "UnitMeasure" },
                    {
                        "data": function (data) {
                            return "<input type=number data-role='none' class='form-control goodcount' />";
                        }
                    },
                    {
                        visible: AllowSRBadCount,
                        "data": function (data) {
                            return "<input type=number data-role='none' class='form-control badcount' />"
                        }
                    }
                    //{ "data": "Quantity" }
                ],
                rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
                    $(row).data('data', data).attr('uomid', data.ID);
                },
                "order": [[1, "asc"]],
                "language": {
                    "search": "Search Unit Measure"
                },
                "initComplete": function () {
                    $('#tblProductBinUnitMeasuresByCategory').removeClass('dataTable').css('width', '100%');
                },
                "sDom": '<"top">rt<"bottom"><"clear">',
                "pageLenght": 25
            });

            //carry over qty
            //if (productQty != undefined) {
            //    $('#tblProductBinUnitMeasures tbody tr td input').each(function () {
            //        if (productUnitMeasureByCategoryDatatable.row($(this).closest('tr')).data().Quantity == 1) {
            //            $(this).val(productQty);
            //        }
            //    })
            //}
        }

        // not used. commented sep 2, 2014
        //    function GetOrganizationID() {
        //        AjaxSend({
        //            url: 'Receiving/GetOrganizationID',
        //            success: function (vData) {
        //                OrgID = vData.OrganizationID;
        //                GetFowst();
        //            }
        //        });
        //    }

        function SearchReceivingTransfers(isearchonline, callback, param) {            
            param.IsCache = 1;
            var draftData = OfflineBrowser.TransReceivingAndTransfers.GetData(param).Rows;
            param.IsCache = 0;
            var finalizedData = OfflineBrowser.TransReceivingAndTransfers.GetData(param).Rows;
            var recordLookup = $.merge(draftData, finalizedData);
            var listRecordLookup = $('#listRecordLookup').empty();
            var colRoute = window.innerWidth >= 500 ? 'p-3 col' : 'p-1 text-center col-sm';
            var otherdetailsColLabel = window.innerWidth >= 500 ? 'col-12 mb-2' : 'col-auto';
            var otherdetailsColVal = window.innerWidth >= 500 ? 'col-12' : 'col text-right';
            console.log(recordLookup)
            if (recordLookup.length > 0) {
                for (var i in recordLookup) {
                    var e = recordLookup[i];

                    listRecordLookup.append($('<li class="list-group-item">')
                        .append($('<div class="row align-items-center mb-3">')
                            .append('<div class="col">' + (CInt2(e.IsReceiving) == 0 ? '<b>TRF #' : '<b>RCV #') + ': ' + e.ReceivingNo + '</div>')
                            .append('<div class="col text-right">' + (CBool(e.IsDraft) ? '<span class="badge rounded-pill bg-warning-transparent text-warning px-2 py-1 d-inline-flex align-items-center gap-1"><i class="bi bi-exclamation-circle mr-2"></i> Draft</span>' : '<span class="badge rounded-pill bg-success-transparent text-success px-2 py-1 d-inline-flex align-items-center gap-1"><i class="bi bi-check-circle-fill mr-2"></i> Finalized</span>') + '</div>')
                            .append((CBool(e.IsDraft) ? $('<div class="col-auto">').append($('<button data-role="none" class="btn bg-danger-light text-danger d-inline-flex align-items-center">')
                                .append('<i class="bi bi-trash"></i>')
                                .data('data', e)
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var draftID = $(this).data('data').ID;
                                    NewMessageBox('Are you sure you want to delete this draft? It will be permanently removed from your device.', 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                Sql.Execute('DELETE FROM cache.TransReceiving WHERE ID = ' + ReadyString(draftID));
                                                Sql.Execute('DELETE FROM cache.TransReceivingStockCountPerUom WHERE ReceivingID = ' + ReadyString(draftID));
                                                SearchReceivingTransfers(isearchonline, callback, param)
                                            }
                                        }, { Text: 'No, maybe later'}
                                    ])
                                })
                                )
                                : ''))
                        )
                        .append($('<div class="row align-items-center mb-3 border g-0 rounded-1 bg-light shadow-sm mb-3">')
                            .append($('<div class="' + colRoute +'">')
                                .append('<span class="text-secondary" style="font-size:14px">' + (CInt2(e.IsReceiving) == 0 ? 'To' : 'From') + '</span>: ' + (e.SourceOfSKU1 || e.SourceOfSKU2))
                            )
                            .append($('<div class="' + colRoute +'-1 text-center">')
                                .append('<span class="' + (window.innerWidth >= 500 ? 'd-block' : 'd-none') + '"><i class="bi bi-arrow-right-circle-fill bg-primary-light p-2 text-primary rounded-pill"></i></span>')
                                .append('<span class="' + (window.innerWidth >= 500 ? 'd-none' : 'd-block') + '"><i class="bi bi-arrow-down-circle-fill bg-primary-light p-2 text-primary rounded-pill"></i></span>')
                            )
                            .append($('<div class="text-right ' + colRoute +'">')
                                .append('<span class="text-secondary" style="font-size:14px">' + (CInt2(e.IsReceiving) == 0 ? 'From' : 'To') + '</span>: ' + (e.ReceivingPlaceFowst || e.ReceivingPlaceThirdParty || e.ReceivingPlace))
                            )
                        )
                        .append($('<div class="row ' + (window.innerWidth >= 500 ? 'row-cols-2' : 'row-cols-1') + ' gy-3">')
                            .append($('<div class="col">')
                                .append($('<div class="row align-items-center">')
                                    .append('<div class="' + otherdetailsColLabel +' text-secondary" style="font-size:14px">Date Received:</div>')
                                    .append('<div class="' + otherdetailsColVal +'">' + $.format.date(new Date(e.DateReceived), 'MMM dd, yyyy') +'</div>')
                                )
                            )
                            .append($('<div class="col">')
                                .append($('<div class="row align-items-center">')
                                    .append('<div class="' + otherdetailsColLabel +'  text-secondary" style="font-size:14px">SKU Count:</div>')
                                    .append('<div class="' + otherdetailsColVal +'">' + e.SKUCount +'</div>')
                                )
                            )
                            .append($('<div class="col">')
                                .append($('<div class="row align-items-center">')
                                    .append('<div class="' + otherdetailsColLabel +'  text-secondary" style="font-size:14px">Transported By:</div>')
                                    .append('<div class="' + otherdetailsColVal +'">' + e.TransportedBy +'</div>')
                                )
                            )
                            .append($('<div class="col">')
                                .append($('<div class="row align-items-center">')
                                    .append('<div class="' + otherdetailsColLabel +'  text-secondary" style="font-size:14px">Last Updated:</div>')
                                    .append('<div class="' + otherdetailsColVal +'">' + $.format.date(new Date(e.LastUpdated), 'MMM dd, yyyy hh:mm a') +'</div>')
                                )
                            )
                        )
                        .data('data', e)
                        .click(function () {
                            $('#popHistory').modal('hide');
                            callback($(this).data('data'));
                        })
                    )
                }

                $('#popHistory').modal('show');
            }

            return
            var settings = [
                {
                    AutoSearch: true,
                    DisplayText: [
                        {
                            Label: function (e) {
                                return 'EAS #'
                            }, Column: 'MIN'
                        }, {
                            Label: function (e) {
                                return e.IsReceiving == 0 ? 'T-Out #' : 'Receiving #'
                            }, Column: 'ReceivingNo'
                        }, {
                            Label: function (e) {
                                return e.IsReceiving == 0 ? 'To' : 'From'
                            }, Column: function (e) {
                                return (e.ReceivingPlaceFowst || e.ReceivingPlaceThirdParty || e.ReceivingPlace)
                            }
                        }, {
                            Label: function (e) {
                                return e.IsReceiving == 0 ? 'From' : 'To'
                            }, Column: function (e) {
                                return (e.SourceOfSKU1 || e.SourceOfSKU2)
                            }
                        }, {
                            Label: 'Date Received', Column: 'DateReceived'
                        }, {
                            Label: 'SKU Count', Column: 'SKUCount'
                        }, {
                            Label: 'Transported By', Column: 'TransportedBy'
                        }, {
                            Label: 'Status', Column: function (e) {
                                return (CBool(e.IsDraft) == true ? '<span style="color:#F00;">Draft</span>' : '<span style="color:#4CAF50">Finalized</span>')
                            }
                        }, {
                            Label: 'Last Updated', Column: function (e) {
                                return $.format.date(new Date(e.LastUpdated), 'MMM dd, yyyy hh:mm a')
                            }
                        }
                    ], Click: function (e) {
                        callback(e);
                    }
                }
            ]
            if (!isearchonline) {
                settings[0].Offline = true;
            }

            BrowsePopup('TransReceivingAndTransfers', 'Receipts and Transfers', settings, param, draftData)
        }

        function GetDepartment(id) {
            //Department is also equal to BIN Location
            var vData = Business.LibraryOrganizationFOWST.GetFowstBin(id).Rows;
            var selDepartment = $('#selDepartment');
            selDepartment.empty();
            selDepartment.append(new Option('Please Select', ''))

            for (var i = 0; i < vData.length; i++){
                selDepartment.append(new Option(vData[i].Name, vData[i].ID))
            }

            selDepartment;//.selectmenu('refresh');

            if(isRecInTransFrom){
                $('#selDepartment').val(RecInTransFromDepartment);//.selectmenu('refresh');
            } else {
                $('#selDepartment').val(RecFromTransToDepartment);//.selectmenu('refresh');
            }
        }

        function ThirdPartyOrg() {
            BrowsePopup('LibraryOrganizationSupplier', 'Search', [
                {
                    Offline: true,
                    AutoSearch: true,
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (e) {
                        $('#selOrganization').val('third');//.selectmenu('refresh');
                        // append the Company Name to third party option, so that the user will know what 3rd party org he/she selected
                        $('#selOrganization option[value="third"]').text('3rd Party (' + e.CompanyName + ')');
                        $('#selOrganization').val('third');//.selectmenu('refresh');

                        IsCustomer = 1;

                        SelectedOrgID = e.ID;
                        SelectedOrgName = e.CompanyName;                        
                        isSelectedThirdParty = true;

                        if (IsMobile) {
                            GetOrgFowst(e.ID, true)
                        }

                        setTimeout(function () {
                            $('#popReceivingTransferOption').modal('show');
                        }, 100)
                    }
                }
            ])
        }

        function GetOrgFowst(id, isThird) {
            var vData;

            if (isThird) {
                if (IsMobile) {
                    // if it is 3rd party org, must lookup to its own fowst
                    vData = OfflineBrowser.LibraryFowst.GetData({
                        IsThirdPartyOrganization: 1,
                        ThirdPartyOrganizationID: id
                    }).Rows
                    DisplayFOWST(vData);
                } else {
                    App.model.Receiving.GetFowst({
                        successFiltered: function (vData) {
                            if (!OfflineMode) {
                                DisplayFOWST(vData.Results.Rows);
                            }
                        },
                        success: function (vData) {
                            DisplayFOWST(vData.Results.Rows);
                        }
                    });
                }
            } else {
                //used only for internal default
                //if not, must lookup to its own fowst but the fowst listed must depend on what fowst assigned to the device
                if (isRecInTransFrom) {
                    if (IsMobile) {
                        vData = Business.LibraryOrganizationFOWST.GetData().Rows;
                        DisplayFOWST(vData);
                    } else {
                        App.model.Receiving.GetFowst({
                            successFiltered: function (vData) {
                                if (!OfflineMode) {
                                    DisplayFOWST(vData.Results.Rows);
                                }
                            },
                            success: function (vData) {
                                DisplayFOWST(vData.Results.Rows);
                            }
                        });
                    }
                } else {
                    if (IsMobile) {
                        vData = Business.LibraryOrganizationFOWST.GetOrgAllFowst().Rows;
                        DisplayFOWST(vData);
                    } else {
                        App.model.Receiving.GetFowst({
                            successFiltered: function (vData) {
                                if (!OfflineMode) {
                                    DisplayFOWST(vData.Results.Rows);
                                }
                            },
                            success: function (vData) {
                                DisplayFOWST(vData.Results.Rows);
                            }
                        });
                    }
                    
                }                
            }

        }

        function DisplayFOWST(vData){
        
            //var selReceivingPlaceOption = $('#selReceivingPlaceOption');
            //selReceivingPlaceOption.empty();
            //selReceivingPlaceOption.append(new Option('Please Select', ''))

            //for (var i = 0; i < vData.length; i++) {
            //    selReceivingPlaceOption.append(new Option(vData[i].FowstName, (vData[i].ID || vData[i].FowstID)))
            //}           

            //selReceivingPlaceOption;//.selectmenu('refresh');
            arrFowst = [];
            for (var i in vData){
                arrFowst.push({ 
                    ID: (vData[i].ID || vData[i].FowstID), 
                    Text: vData[i].FowstName,
                    Click: function(e) {
                        SelectedOutlet = e.ID;
                        SelectedOutletName = e.Text;
                        $('#selReceivingPlaceOption').text(SelectedOutletName);
                        if (IsMobile) {
                            GetDepartment(SelectedOutlet) // get department or bin of selected fowst
                        }
                    }
                })
            }

            $('#selDepartment').empty().append(new Option('Please Select', ''));//.selectmenu('refresh');

            if (vData.length == 1) {
                //set default if one fowst
                SelectedOutlet = (vData[0].ID || vData[0].FowstID);
                SelectedOutletName = vData[0].FowstName;
                if (!isRecInTransFrom) {
                    // Receive From or Transfer To only
                    var data = { ID: (vData[0].ID || vData[0].FowstID), Name: vData[0].FowstName };
                    $('#lblSourceOfSKU').data(data);
                }
                $('#selReceivingPlaceOption').text((vData[0].FowstName));//.selectmenu('refresh');

                if (IsMobile) {
                    GetDepartment((vData[0].ID || vData[0].FowstID));
                }
                
            }
            else if (vData.length > 1) {
                //if multiple fowst assign the current of clicked button to temp vars for outlet (fowst)
                if (isRecInTransFrom) {
                    SelectedOutlet = RecInTransFromFowstID;
                    SelectedOutletName = RecInTransFromFowstName;
                }
                else {
                    SelectedOutlet = RecFromTransToFowstID;
                    SelectedOutletName = RecFromTransToFowstName;
                }                
            }
            else {
                SelectedOutlet = undefined;
                SelectedOutletName = undefined;
            }
        }
        function UpdateContent() {
            $('#divReceivingContent').css('margin-top', $('#divHeaderReceiving').height() - 100 + 'px');
            $('#divCartContent').css('margin-top', $('#divHeaderCart').height() - 100 + 'px');
        }

        function GetSystemBehaviorSettings() {
            var pSql = 'Select DefaultValue from private.SystemDefaults where Description = \'Automatically Received Stocks\' ';
            IsAutomaticReceived = CInt2(Sql.ExecuteScalar(pSql));

            pSql = 'Select DefaultValue from private.SystemDefaults where Description = \'Show Stock Quantities\' ';
            IsShowQuantityOfStocks = CInt2(Sql.ExecuteScalar(pSql));
        }

        function UpdateDisplay() {

            //if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
            if (CBool(IsReceiving)) {
                //IsReceiving = 1;

                $('#lblReceivingPlace').text('Receive In: ');
                $('#lblSKUSource').text('Receive From: ');

                $('#dvReceiving').css('visibility', 'visible')
                $('#dvReceiving').css('display', 'block')

                $('#dvTransferOut').css("visibility", "hidden");
                $('#dvTransferOut').css("display", "none");

                $('#btnInputItemDetails > div').html('Go To Receiving Cart');

                $('#ReceivingTitle').text('Receiving Cart');
                $('#dvGoodCount').html('Good Count');
                $('#dvpageCartHeader').html('Receiving Cart');

                //$('.headerInformation').css('visibility', 'visible');
                $('.headerInformation').css('display', 'block');
            }
            else {

                $('#lblReceivingPlace').text('Transfer From: ');
                $('#lblSKUSource').text('Transfer To: ');
                IsReceiving = 0;

                $('#dvReceiving').css('visibility', 'hidden')
                $('#dvReceiving').css('display', 'none')

                $('#dvTransferOut').css("visibility", "visible")
                $('#dvTransferOut').css("display", "inline")

                $('#btnInputItemDetails > div').html('Go To Transfer Out Cart');

                $('#selResaleableAnswer').css('visibility', 'hidden');
                //$('#selResaleableAnswer').css('display', 'none');

                $('#ReceivingTitle').text('Transfer-Out Cart');
                $('#dvGoodCount').html('Count');
                $('#dvpageCartHeader').html('Transfer-Out Cart');

                //$('.headerInformation').css('visibility', 'hidden');
                $('.headerInformation').css('display', 'none');

            }
        }

        function SetIsResaleable() {
            $('#selResaleableAnswer').empty();
            $('#dvUnitMeasureIsResaleable').css('visibility', 'visible');
            $('#dvUnitMeasureIsResaleable').css('display', 'block');
            $('#dvUnitMeasureIsResaleableNotes').css('visibility', 'visible');
            $('#dvUnitMeasureIsResaleableNotes').css('display', 'block');

            if ($('#selIsResaleable option:selected').val() == 'ResaleableYes') {
                $('#selResaleableAnswer').append(new Option('Change of mind', 'ChangeofMind', true));
                $('#selResaleableAnswer').append(new Option('Excess quantity ordered', 'ExcessQuantityOrdered', true));
                $('#selResaleableAnswer').append(new Option('Goods no longer needed', 'GoodsNoLongerNeeded', true));
                $('#selResaleableAnswer').append(new Option('Pull out request by main office', 'PullOutRequestbyMainOffice', true));
                $('#selResaleableAnswer').append(new Option('Pull out request by vendor', 'PullOutRequestbyVendor', true));
                $('#selResaleableAnswer').append(new Option('Requested by another branch or store', 'RequestedbyAnotherBranchorStore', true));
                $('#selResaleableAnswer').append(new Option('Quality of goods does not meet expectations', 'QualityofGoodsDoesNotMeetExpectations', true));
                $('#selResaleableAnswer').append(new Option('Wrong goods ordered', 'WrongGoodsOrdered', true));
                $('#selResaleableAnswer').append(new Option('Wrong goods delivered', 'WrongGoodsDelivered', true));
                $('#lblResaleableNo').css('visibility', 'hidden');
                $('#txtResaleableNo').css('visibility', 'hidden');
                $('#dvResaleable').css('display', 'none');
                $('#lblResalableCount').css('visibility', 'hidden');

                //                $('#dvUnitMeasureIsResaleable').css('visibility', 'visible');
                //                $('#dvUnitMeasureIsResaleableNotes').css('visibility', 'visible');
                $('#selResaleableAnswer').css('visibility', 'visible');
                $('#lblUnitMeasureIsResaleable').text('Resaleable');
                $('#lblUnitMeasureIsResaleableNote').text('Please encode only the quantities of resaleable items (goods that are fit to be resold to customers).' +
                'If you would like to receive or transfer out bad stocks (defective, obsolete, expired, etc.), please do so in a separate transaction.');

            }
            else if ($('#selIsResaleable option:selected').val() == 'ResaleableNo') {
                $('#selResaleableAnswer').append(new Option('Defective goods', 'DefectiveGoods', true));
                $('#selResaleableAnswer').append(new Option('Expired item', 'Expireditem', true));
                $('#selResaleableAnswer').append(new Option('About-to-expire item', 'About-to-expireitem', true));
                $('#selResaleableAnswer').append(new Option('Factory-defective packaging', 'Factory-defectivepackaging', true));
                $('#selResaleableAnswer').append(new Option('Factory-defective product', 'Factory-defectiveproduct', true));
                $('#selResaleableAnswer').append(new Option('Non-Factory-Damaged Goods', 'Non-Factory-DamagedGoods', true));
                $('#selResaleableAnswer').append(new Option('Non-Factory-Damaged Packaging', 'Non-Factory-DamagedPackaging', true));
                $('#selResaleableAnswer').append(new Option('Obsolete item', 'Obsoleteitem', true));
                $('#selResaleableAnswer').append(new Option('Product recall', 'Productrecall', true));

                $('#lblResaleableNo').css('visibility', 'visible');
                $('#txtResaleableNo').css('visibility', 'visible');
                $('#lblResalableCount').css('visibility', 'visible');
                $('#dvResaleable').css('display', 'block');
                //                $('#dvUnitMeasureIsResaleable').css('visibility', 'visible');
                //                $('#dvUnitMeasureIsResaleableNotes').css('visibility', 'visible');
                $('#selResaleableAnswer').css('visibility', 'visible');
                $('#lblUnitMeasureIsResaleable').text('Non-Resaleable');
                $('#lblUnitMeasureIsResaleableNote').text('Please encode only the quantities of non-resaleable items (damaged, defective, obsolete, expired, etc.).' +
                'If you would like to receive or transfer out good stocks (goods that are fit to be resold to customers), please do so in a separate transaction.');
            }
            else {

                $('#selResaleableAnswer').css('visibility', 'hidden');

                $('#lblResaleableNo').css('visibility', 'hidden');
                $('#txtResaleableNo').css('visibility', 'hidden');
                $('#lblResalableCount').css('visibility', 'hidden');
                $('#dvResaleable').css('display', 'none');
            }

            try {
                $('#selResaleableAnswer');//.selectmenu('refresh');
            }
            catch (e) {
                $('#selResaleableAnswer');//.selectmenu();
            }

        }

        function GetReceivingOrTransferOutInfo(vPram) {            

            if (OfflineMode && !IsGetDataFromServer) {

                //vPram = { ID: ID };

                var vRetDataReceivingInfo = App.model.Receiving.GetReceivingOrTransfersInfo(vPram);
                var vRetDataReceivingDetails = CBool(vPram.IsDraft) ? { Rows: [] } : App.model.Receiving.GetReceivingDetails(vPram);;            
                
                ClearFields(); // Clear all fields when getting the draft trans

                ReceivingSKU = vRetDataReceivingDetails.Rows;
                DisplayReceivingInformation(vRetDataReceivingInfo.Rows[0]);
                console.log(vRetDataReceivingInfo.Rows[0])
            } else {
                AjaxSend({
                    url: "Receiving/GetAllReceiving",
                    data: { ID: ID },
                    success: function (vRetDataReceivingInfo) {
                        ClearFields(); // Clear all fields when getting the draft trans

                        if (IsSearchFromTransFerOut) {
                            IsFromTransferOutOrDraftReceiving = true;
                        }

                        DisplayReceivingInformation(vRetDataReceivingInfo.Results.Rows[0]);

                        AjaxSend({
                            url: "Receiving/GetAllReceivingSKU",
                            data: { ID: ID },
                            success: function (vRetDataReceivingDetails) {
                                ReceivingSKU = vRetDataReceivingDetails.Results.Rows;
                                //DisplayReceivingSKU(vRetDataReceivingDetails.Results.Rows)
                            }
                        });
                    }
                });
            }

        }

        function DisplayReceivingInformation(vRows) {

            var data = [];
            var m_Date;

            var SourceOfSKUID = "";
            var SourceOfSKU = "";

            ID = vRows.ID;
            POID = (vRows.POID || '');
            //if (vRows.IsReceiving == 0) {

            //    $('#selReceiveOrTransfer').val('TransferItems');

            //    $('#dvTransferOut').css("visibility", "visible");
            //    $('#dvTransferOut').css("display", "inline")

            //    $('#dvReceiving').css('visibility', 'hidden')
            //    $('#dvReceiving').css('display', 'none')

            //    if (vRows.Resaleable == 1) {
            //        $('#selIsResaleable').val('ResaleableYes');
            //        IsResaleable = 1;
            //    }
            //    else {
            //        $('#selIsResaleable').val('ResaleableNo');
            //        IsResaleable = 0;
            //    }

            //    SetIsResaleable();
            //    $('#selIsResaleable');//.selectmenu('refresh');

            //}
            //else {

            if (CBool(IsDraft)) {
                $('#pageMain').pageshow();

                $('#selReceiveOrTransfer').val('ReceiveItems');

                $('#dvReceiving').css('visibility', 'visible')
                $('#dvReceiving').css('display', 'block')

                $('#dvTransferOut').css("visibility", "hidden");
                $('#dvTransferOut').css("display", "none")
                //}

                $('#selReceiveOrTransfer');//.selectmenu('refresh');

                if (IsReceiving == 1) {
                    data = { ID: (vRows.SourceOfSKU1ID || vRows.SourceOfSKU2ID), Name: (vRows.SourceOfSKU1 || vRows.SourceOfSKU2) };                    
                    $('#selIntraOrganization').val(vRows.SourceOfSKU1ID);
                    $('#selIntraOrganization');//.selectmenu('refresh');

                } else {
                    //data = { ID: vRows.ReceivingPlaceID, Name: vRows.ReceivingPlace };
                    data = { ID: vRows.SourceOfSKU1ID, Name: vRows.SourceOfSKU1 };
                    $('#selIntraOrganization').val(vRows.ReceivingPlaceID);
                    $('#selIntraOrganization');//.selectmenu('refresh');
                }

                RecFromTransToOrgID = $('#selOrganization').val();
                RecFromTransToOrgName = $('#selOrganization').find('option:selected').text();
                RecFromTransToFowstID = (vRows.SourceOfSKU1ID || vRows.ReceiveFromOutlet)
                RecFromTransToFowstName = (vRows.SourceOfSKU1 || vRows.ReceiveFromOutletName)
                RecFromTransToDepartment = (vRows.ReceiveFromDepartment || '')
                isThirdPartyRecInTransFrom = false;

                $('#btnRecFromTransToPlace').text(RecFromTransToOrgName + ' (' + RecFromTransToFowstName + ')');//.button('refresh');

                //data = { ID: SourceOfSKUID };
                $('#lblSourceOfSKU').val(SourceOfSKU)
                $('#lblSourceOfSKU').data(data);

                //Check if ReceiveInOrgID is equal to current OrgID
                //if equal then the value of RecInTransFromOrgName must be the current orgname and RecInTransFromOrgID must be the current orgid
                if ((vRows.ReceiveInOrgID || vRows.SourceOfSKU2ID) == sessionStorage.OrgID) {
                    RecInTransFromOrgID = $('#selOrganization').val();
                    RecInTransFromOrgName = $('#selOrganization').find('option:selected').text();
                    RecInTransFromFowstID = vRows.ReceivingPlaceID;
                    RecInTransFromFowstName = (vRows.ReceivingPlaceFowst || vRows.ReceivingPlace);
                    RecInTransFromDepartment = vRows.ReceiveInDepartment;
                    isThirdPartyRecInTransFrom = false;
                    $('#btnRecInTransFromPlace').text(RecInTransFromOrgName + ' (' + (vRows.ReceivingPlaceFowst || vRows.ReceivingPlace) + ')');//.button('refresh');
                } else {
                    //if not, it is a 3rd party org name and org id
                    RecInTransFromOrgID = vRows.ReceivingPlaceID;
                    RecInTransFromOrgName = (vRows.ReceivingPlace || vRows.ReceivingPlaceFowst);
                    RecInTransFromFowstID = "";
                    RecInTransFromFowstName = "";
                    RecInTransFromDepartment = "";
                    isThirdPartyRecInTransFrom = true;
                    $('#btnRecInTransFromPlace').text('3rd Party (' + RecInTransFromOrgName + ')');//.button('refresh');
                    $('#selOrganization option[value="third"]').text('3rd Party (' + RecInTransFromOrgName + ')')
                    $('#selOrganization').val('third');//.selectmenu('refresh');
                }

                if (CStr2(vRows.SourceOfSKU2ID) == "") {

                    $('#selSourceOfSKU').val('valIntraOrganization');
                    if (IsReceiving == 1) {
                        SourceOfSKUID = vRows.ReceivingPlaceID;
                        $('#selReceivingPlace').val(SourceOfSKUID);                        
                        
                    } else {
                        SourceOfSKUID = vRows.SourceOfSKU1ID;
                        SourceOfSKU = vRows.SourceOfSKU1;
                        $('#selIntraOrganization').val(SourceOfSKUID);
                    }                    

                    $('#dvIntraOrganization').css('visibility', 'visible');
                    $('#dvIntraOrganization').css('display', 'block');
                    $('#dvVendor').css('visibility', 'hidden');
                    $('#dvVendor').css('display', 'none');
                    IsIntraOrganization = 1;

                    $('#selIntraOrganization');//.selectmenu('refresh');
                    $('#selReceivingPlace');//.selectmenu('refresh');
                }
                else {
                    SourceOfSKUID = vRows.SourceOfSKU2ID;
                    SourceOfSKU = vRows.SourceOfSKU2;
                    $('#selSourceOfSKU').val('valThirdPartyVendor');
                    $('#dvVendor').css('visibility', 'visible');
                    $('#dvVendor').css('display', 'block');
                    $('#dvIntraOrganization').css('visibility', 'hidden');
                    $('#selReceivingPlace').val(vRows.ReceivingPlaceID);//.selectmenu('refresh');
                    IsIntraOrganization = 0;

                    data = { ID: SourceOfSKUID };
                    $('#lblSourceOfSKU').val(SourceOfSKU)
                    $('#lblSourceOfSKU').data(data);

                    RecFromTransToOrgID = SourceOfSKUID;
                    RecFromTransToOrgName = SourceOfSKU;
                    RecFromTransToFowstID = '';
                    RecFromTransToFowstName = '';
                    RecFromTransToDepartment = (vRows.RecFromTransToDepartment || '');
                    isThirdPartyRecFromTransTo = true;

                    $('#btnRecFromTransToPlace').text('3rd Party (' + RecFromTransToOrgName + ')');//.button('refresh');
                }

                $('#selSourceOfSKU');//.selectmenu('refresh');

                m_Date = new Date(vRows.DocumentDate);

                $('#optMonthValue').val(m_Date.getMonth() + 1)
                $('#optYearValue').val(m_Date.getFullYear())
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
                $('#optDayValue').val(m_Date.getDate())

                $('#optMonthValue');//.selectmenu('refresh');
                $('#optYearValue');//.selectmenu('refresh');
                $('#optDayValue');//.selectmenu('refresh');

                $('#txtDocumentType').val(vRows.DocumentType)
                $('#txtDocumentNo').val((CInt2(vRows.ReceivingNo) == 0 ? vRows.DocumentNo : vRows.ReceivingNo))
                $('#txtPONo').val(vRows.PONo)
                $('#txtTransportedBy').val(vRows.TransportedBy)

                $('#rcChoiceYes');//.checkboxradio('refresh')

                $('#rcChoiceNo');//.checkboxradio('refresh')

                if (CInt2(vRows.Is3PL) == 1) {
                    Is3PL = 1;
                    $('#rcChoiceYes').prop('checked', 1);

                    $('#dv3pl').css('visibility', 'visible').show();
                    data = { ID: vRows.ShipperID };
                    $('#txtShipper').val(vRows.Shipper);
                    $('#txtShipper').data(data);
                    $('#txtWayBill').val(vRows.WaybillNumber);
                    $('#txtVessel').val(vRows.VesselID);

                    if (IsDate(vRows.DatePrepared)) {
                        $('#optMonthBLValue').val($.format.date(new Date(vRows.DatePrepared), 'M'));
                        $('#optYearBLValue').val($.format.date(new Date(vRows.DatePrepared), 'yyyy'));
                        setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false)
                        $('#optDayBLValue').val($.format.date(new Date(vRows.DatePrepared), 'd'));                        
                    }
                    
                    $('#txtVoyageNo').val(vRows.VoyageNumber);

                    if (IsDate(vRows.InvoiceDate)) {
                        $('#optMonthInvoiceValue').val($.format.date(new Date(vRows.InvoiceDate), 'M'));
                        $('#optYearInvoiceValue').val($.format.date(new Date(vRows.InvoiceDate), 'yyyy'));
                        setDayCollection('optMonthInvoiceValue', 'optDayInvoiceValue', 'optYearInvoiceValue', false);
                        $('#optDayInvoiceValue').val($.format.date(new Date(vRows.InvoiceDate), 'd'));                        
                    }
                    
                    $('#txtInvoiceNo').val(vRows.InvoiceNumber);
                    $('#txtFreightAndHandling').val(vRows.FreightAndHandling);
                    $('#txtArrastre').val(vRows.Arrastre);
                    $('#txtTotal').val(vRows.Total);
                    $('#txtTotalCC').val(vRows.TotalCC);
                    $('#txtTotalCBM').val(vRows.TotalCBM);
                    $('#txtTotalWeightInkg').val(vRows.TotalWeightInKg);
                    $('#txtTotalCostofBundle').val(vRows.TotalCostOfBundle);
                    $('#txtBundles').val(vRows.Bundles);
                }
                else {
                    $('#rcChoiceNo').checked = true;
                }

                //m_Date = new Date(vRows.WaybillDate);

                //$('#optMonthBLValue').val(m_Date.getMonth() + 1);
                //$('#optYearBLValue').val(m_Date.getFullYear())
                
                //$('#optDayBLValue').val(m_Date.getDate())

                $('#txtTransportedBy').val(vRows.TransportedBy);

                //$('#optMonthBLValue');//.selectmenu('refresh');
                //$('#optYearBLValue');//.selectmenu('refresh');
                //$('#optDayBLValue');//.selectmenu('refresh');
            }
            else {
                $('#pageReadOnly').pageshow();
                startclock('#clockReadOnly');

                var mDate = $.format.date(new Date(vRows.DocumentDate), 'MMM dd, yyyy');

                if (!IsDate(mDate)){
                    mDate = '';
                }

                $('#txtROReceiveIn').text(vRows.ReceivingPlace || vRows.ReceivingPlaceFowst);
                $('#txtROReceiveFrom').text((vRows.SourceOfSKU1 || vRows.SourceOfSKU2));
                $('#txtRODocDate').text(mDate);
                $('#txtRODocType').text(vRows.DocumentType);
                $('#txtRODocNo').text(vRows.DocumentNo);
                $('#txtROPONo').text(vRows.PONo);
                $('#txtROTransportedBy').text(vRows.TransportedBy);

                if (CInt2(vRows.Is3PL) == 1) {
                    $('#divRO3PL').show();
                    Is3PL = 1;

                    $('#txtROShipper').text(vRows.Shipper);
                    $('#txtROWayBill').text(vRows.WaybillNumber);
                    $('#txtROVessel').text(vRows.VesselID);

                    if (IsDate(vRows.DatePrepared)) {
                        $('#txtRODatePrepared').text($.format.date(new Date(vRows.DatePrepared), 'MMMM dd, yyyy'));
                    } else {
                        $('#txtRODatePrepared').text('N/A');
                    }

                    $('#txtROVoyageNo').text(vRows.VoyageNumber);

                    if (IsDate(vRows.InvoiceDate)) {
                        $('#txtROInvoiceDate').text($.format.date(new Date(vRows.InvoiceDate), 'MMMM dd, yyyy'));
                    } else {
                        $('#txtROInvoiceDate').text('N/A');
                    }

                    $('#txtROInvoiceNo').text(vRows.InvoiceNumber);
                    $('#txtROFreightAndHandling').text(vRows.FreightAndHandling);
                    $('#txtROArrastre').text(vRows.Arrastre);
                    $('#txtROTotal').text(vRows.Total);
                    $('#txtROTotalCC').text(vRows.TotalCC);
                    $('#txtROTotalCBM').text(vRows.TotalCBM);
                    $('#txtROTotalWeightInkg').text(vRows.TotalWeightInKg);
                    $('#txtROTotalCostofBundle').text(vRows.TotalCostOfBundle);
                    $('#txtROBundles').text(vRows.Bundles);
                }
                else {
                    $('#divRO3PL').hide();
                }

                $('#lstROReceivingCart').empty();

                for (var i = 0; i < ReceivingSKU.length; i++) {
                    $('#lstROReceivingCart')
                        .append($('<li data-role="none" class="list-group-item list-group-item-action">')
                            .append($('<b>').append((ReceivingSKU[i].ProductNo == "" ? '' : '(' + ReceivingSKU[i].ProductNo + ') ') + ReceivingSKU[i].ProductName))
                            .append('<br>')
                            .append(ReceivingSKU[i].QuantityGood + ' ' + ReceivingSKU[i].Label)
                        )
                }
                $('#lstROReceivingCart').trigger('create');
                ClearFields();
            }

        }
        //function GetAllReceiving(m_data) {
        //    App.model.Receiving.GetAllReceiving({
        //        data: m_data,
        //        success: function (vData) {
        //            DisplayReceiving(vData.Receiving.Rows);
        //            //GetFowst();
        //        }
        //    });
        //}

        function ClearFields() {
            IsNew = 1;
            IsIntraOrganization = 0;

            IsReceiving = CInt2(GetUrlParameters().IsReceiving);
            Is3PL = 0;
            IsMainPage = true;
            POID = '';
            ID = '';
            ReceivingSKU = undefined;
            arrCountItems = [];

            isThirdPartyRecInTransFrom = false;
            isThirdPartyRecFromTransTo = false;
            isRecInTransFrom = true;
            RecInTransFromOrgID = '';
            RecInTransFromOrgName = '';
            RecFromTransToOrgID = '';
            RecFromTransToOrgName = '';
            RecFromTransToDepartment = '';
            RecInTransFromDepartment = '';
            RecFromTransToFowstID = '';
            RecInTransFromFowstID = '';
            RecInTransFromFowstName = '';
            RecFromTransToFowstName = '';

            SelectedOrgID = undefined;
            SelectedOrgName = undefined;
            SelectedOutlet = undefined;
            SelectedOutletName = undefined;
            SelectedDepartment = undefined;
            isSelectedThirdParty = undefined;

            if (CBool(IsReceiving)) {
                $('#MainPageTitle').text('Receiving')
                $('#selReceiveOrTransfer').val('ReceiveItems');
                //$('#txtDocumentType').val('');
                //$('#txtDocumentNo').val('');

            } else {
                $('#MainPageTitle').text('Transfers')
                $('#selReceiveOrTransfer').val('TransferItems');
                $('#txtDocumentType').val('Transfer-Out');
                $('#txtDocumentNo').val('');
            }

            try {
                $('#selReceiveOrTransfer');//.selectmenu('refresh');
            }
            catch (e) {
                $('#selReceiveOrTransfer');//.selectmenu();
            }

            UpdateDisplay();

            IsFromTransferOutOrDraftReceiving = false;

            //$('#txtTransportedBy').val('');
            $('#btnSearchItem').val('');

            $('#lblSourceOfSKU').data('ID', null);

            //$('#btnInputItemDetails .ui-btn-text').text('Go to Receiving Cart');


            $('#txtShipper').val('');
            $('#txtWayBill').val('');

            $('#txtPONo').val('');
            $('#txtVessel').val('');
            $('#txtVoyageNo').val('');
            $('#txtInvoiceNo').val('');
            $('#txtArrastre').val('');
            $('#txtFreightAndHandling').val('');
            $('#txtTotal').val('');
            $('#txtTotalCC').val('');
            $('#txtTotalCBM').val('');
            $('#txtTotalWeightInkg').val('');
            $('#txtTotalCostofBundle').val('');
            $('#txtBundles').val('');

            $('#dvIntraOrganization').css('visibility', 'hidden');
            $('#dvVendor').css('visibility', 'hidden');

            //$('#dvReceiving').css('visibility', 'visible')
            //$('#dvReceiving').css('display', 'block')

            $('#dvTransferOut').css("visibility", "hidden");
            $('#dvTransferOut').css("display", "none")

            $('#selIntraOrganization').val('');
            $('#selSourceOfSKU').val('');
            $('#selReceivingPlace').val('');
            //$('#selReceiveOrTransfer').val('');
            $('#selIsResaleable').val('');

            $('#rcChoiceNo').prop('checked', true);
            $('#rcChoiceYes').prop('checked', false);
            $('#dv3pl').hide();

            try {
                $("#rcChoiceNo");//.checkboxradio("refresh");
                $('#selIntraOrganization');//.selectmenu('refresh');
                $('#selSourceOfSKU');//.selectmenu('refresh');
                $('#selReceivingPlace');//.selectmenu('refresh');
                $('#selIsResaleable');//.selectmenu('refresh');
            }
            catch (e) {
                $("#rcChoiceNo");//.checkboxradio();
                $('#selIntraOrganization');//.selectmenu();
                $('#selSourceOfSKU');//.selectmenu();
                $('#selReceivingPlace');//.selectmenu();
                $('#selIsResaleable');//.selectmenu();
            }


            $('#lstItems').empty().append('<li class="emptylist"><div style="text-align:center; color: #a9afb5; font-size:25px;"><span class="bi-cart4" style="font-size:100px;"></span><br />Cart is empty</div></li>');

            //$('#selReceiveOrTransfer');//.selectmenu('refresh');

            $('#btnRecInTransFromPlace, #btnRecFromTransToPlace, #selReceivingPlaceOption').text('Please Select');//.button('refresh');
            $('#selOrganization')
                .empty()
                .append(new Option(sessionStorage.OrgName, sessionStorage.OrgID))
                .append(new Option('3rd Party', 'third'))
                .append(new Option('Select another 3rd Party', 'anothird'))
                ;//.selectmenu('refresh')
            $('#selOrganization').val(sessionStorage.OrgID);//.selectmenu('refresh');
            SRReceiveFromTransferToDefault();
            ReceiveInInternalSetToDefault();
        }        

        function GetAllReceivingSKU() {
            data = {
                ID: ID
            }
            AjaxSend({
                url: 'Receiving/GetAllReceivingSKU',
                data: data,
                success: function (vData) {
                    ReceivingSKU = vData.Receiving.Rows;
                    //DisplayReceivingSKU(vData.Receiving.Rows)
                }
            });
        }

        var ReceivingSKU;

        //function DisplayReceivingSKU() {
        //    var pLV = $("#lstItems");
        //    var StockNumber = "";
        //    var StockDescription = "";

        //    vRows = ReceivingSKU;

        //    pLV.empty();

        //    for (var i = 0; i <= vRows.length - 1; i++) {

        //        //StockNumber = vRows[i].StockNumber;
        //        StockDescription = vRows[i].ProductName;

        //        pItem = $('<li>')
        //        .attr('ID', vRows[i].ID)
        //        .append($('<a>')
        //            .attr('ID', vRows[i].ID)
        //            //.data('units', getStockData(vRows[i].ID))
        //            .click(function (e) {
        //                //$.mobile.changePage("#pageUnitMeasure");
        //                $('#lblSKUNo').text(StockNumber);
        //                $('#lblSKUDescription').text(StockDescription);        
        //                GetStockUnitMeasures($(this).attr('ID'), ID)
        //            })
        //            .append($('<h3>')
        //                .text(StockDescription)
        //            )
        //        )

        //        .append($('<a>')
        //            .attr('href', '#')
        //            .attr('ID', 'close' + vRows[i].ID)
        //            .click(function () {
        //                $(this).remove();
        //                $("#" + $(this).attr("ID").substr(5, 36)).remove();
        //                $("#lstItems");//.listview('refresh');
        //            })
        //        )
        //        pLV.append(pItem);
        //    }

        //    try {
        //        pLV;//.listview('refresh');
        //    }
        //    catch (e) {                
        //    }
        //}

        function getStockData(m_ID) {
            var data = { ProductID: m_ID, ReceivingID: ID }

            AjaxSend({
                url: 'Receiving/GetStockUnitMeasures',
                data: data,
                success: function (vData) {
                    var dataUnits = {};

                    dataUnits.units = [];

                    for (var i = 0; i <= vData.UnitMeasure.Rows.length - 1; i++) {

                        dataUnits.units.push({ ID: vData.UnitMeasure.Rows[i].ID,
                            UnitMeasure: vData.UnitMeasure.Rows[i].UnitMeasure,
                            ActualQuantity: vData.UnitMeasure.Rows[i].Quantity,
                            Quantity: vData.UnitMeasure.Rows[i].QuantityGood,
                            QuantityBad: vData.UnitMeasure.Rows[i].QuantityDamaged,
                            BadOrderReason: vData.UnitMeasure.Rows[i].BadOrderReason,
                            AdditionalInfo: vData.UnitMeasure.Rows[i].AdditionalInfo,
                            IsResaleable: vData.UnitMeasure.Rows[i].IsResaleable
                        });
                        $('#' + m_ID).data(dataUnits);
                    };
                }
            });
        }

        //function DisplayReceiving(vRows) {
        //    var pLV = $("#lstReceiving");

        //    pLV.empty();

        //    for (var i = 0; i <= vRows.length - 1; i++) {
        //        var IsReceivingLabel = "Receiving";
        //        var SKUSource = vRows[i].SourceOfSKU1;

        //        if (vRows[i].IsReceiving == "0") {
        //            IsReceivingLabel = "Transfer";
        //        }

        //        if (SKUSource == "") {
        //            SKUSource = vRows[i].SourceOfSKU2;
        //        }

        //        pItem = $('<li>')
        //        .append($('<a>')
        //            .attr('ID', vRows[i].ID)
        //            .data('data', vRows[i])
        //            .click(function (e) {

        //                $.mobile.changePage('#pageMain')

        //                IsNew = 0;
        //                var data = [];
        //                var m_Date;

        //                var SourceOfSKUID = "";
        //                var SourceOfSKU = "";

        //                ID = $(this).attr('ID');

        //                if ($(this).data('data').IsReceiving == 0) {

        //                    $('#selReceiveOrTransfer').val('TransferItems');

        //                    $('#dvTransferOut').css("visibility", "visible");
        //                    $('#dvTransferOut').css("display", "inline")

        //                    $('#dvReceiving').css('visibility', 'hidden')
        //                    $('#dvReceiving').css('display', 'none')

        //                    if ($(this).data('data').Resaleable == 1) {
        //                        $('#selIsResaleable').val('ResaleableYes');
        //                        IsResaleable = 1;
        //                    }
        //                    else {
        //                        $('#selIsResaleable').val('ResaleableNo');
        //                        IsResaleable = 0;
        //                    }

        //                    SetIsResaleable();
        //                    $('#selIsResaleable');//.selectmenu('refresh');

        //                }
        //                else {
        //                    $('#selReceiveOrTransfer').val('ReceiveItems');

        //                    $('#dvReceiving').css('visibility', 'visible')
        //                    $('#dvReceiving').css('display', 'block')

        //                    $('#dvTransferOut').css("visibility", "hidden");
        //                    $('#dvTransferOut').css("display", "none")
        //                }

        //                $('#selReceiveOrTransfer');//.selectmenu('refresh');

        //                data = { ID: $(this).data('data').ReceivingPlaceID };
        //                $('#selReceivingPlace').val($(this).data('data').ReceivingPlaceID);

        //                $('#selReceivingPlace');//.selectmenu('refresh');

        //                if (!$(this).data('data').SourceOfSKU1ID == "") {
        //                    SourceOfSKUID = $(this).data('data').SourceOfSKU1ID;
        //                    SourceOfSKU = $(this).data('data').SourceOfSKU1;
        //                    $('#selSourceOfSKU').val('valIntraOrganization');
        //                    $('#selIntraOrganization').val(SourceOfSKUID);
        //                    $('#dvIntraOrganization').css('visibility', 'visible');
        //                    $('#dvVendor').css('visibility', 'hidden');
        //                    IsIntraOrganization = 1;

        //                    $('#selIntraOrganization');//.selectmenu('refresh');
        //                }
        //                else {
        //                    SourceOfSKUID = $(this).data('data').SourceOfSKU2ID;
        //                    SourceOfSKU = $(this).data('data').SourceOfSKU2;
        //                    $('#selSourceOfSKU').val('valThirdPartyVendor');
        //                    $('#dvVendor').css('visibility', 'visible');
        //                    $('#dvIntraOrganization').css('visibility', 'hidden');
        //                    IsIntraOrganization = 0;
        //                }

        //                $('#selSourceOfSKU');//.selectmenu('refresh');

        //                data = { ID: SourceOfSKUID };
        //                $('#lblSourceOfSKU').text(SourceOfSKU)
        //                $('#lblSourceOfSKU').data(data);

        //                m_Date = new Date($(this).data('data').DocumentDate);

        //                $('#optMonthValue').val(m_Date.getMonth() + 1)
        //                $('#optYearValue').val(m_Date.getFullYear())
        //                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
        //                $('#optDayValue').val(m_Date.getDate())

        //                $('#optMonthValue');//.selectmenu('refresh');
        //                $('#optYearValue');//.selectmenu('refresh');
        //                $('#optDayValue');//.selectmenu('refresh');

        //                $('#txtDocumentType').val($(this).data('data').DocumentType)
        //                $('#txtDocumentNo').val($(this).data('data').DocumentNo)

        //                $('#rcChoiceYes');//.checkboxradio('refresh')

        //                $('#rcChoiceNo');//.checkboxradio('refresh')

        //                if ($(this).data('data').Is3PL == 1) {
        //                    $('#rcChoiceYes').checked = true;

        //                    $('#dv3pl').css('visibility', 'visible');
        //                    data = { ID: $(this).data('data').ShipperID };
        //                    $('#txtShipper').val($(this).data('data').Shipper)
        //                    $('#txtShipper').data(data)
        //                    $('#txtWayBill').val($(this).data('data').WaybillNumber)
        //                }
        //                else {
        //                    $('#rcChoiceNo').checked = true;
        //                }

        //                m_Date = new Date($(this).data('data').WaybillDate);

        //                $('#optMonthBLValue').val(m_Date.getMonth() + 1);
        //                $('#optYearBLValue').val(m_Date.getFullYear())
        //                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false)
        //                $('#optDayBLValue').val(m_Date.getDate())

        //                $('#optMonthBLValue');//.selectmenu('refresh');
        //                $('#optYearBLValue');//.selectmenu('refresh');
        //                $('#optDayBLValue');//.selectmenu('refresh');

        //                GetAllReceivingSKU();

        //            })
        //            .append($('<h3>')
        //                .text('Transaction No.: ' + vRows[i].ReceivingNo)
        //            )
        //            .append($('<p>')
        //                .text("Source of SKU: " + SKUSource)
        //            )
        //            .append($('<p>')
        //                .text("Transaction Type: " + IsReceivingLabel)
        //            )
        //        )
        //        pLV.append(pItem);
        //    }

        //    try {
        //        pLV;//.listview('refresh');
        //    }
        //    catch (e) {
        //        pLV;//.listview();
        //    }

        //}


        function GetShippers() {
            BrowsePopup('LibraryOrganization', 'Search Shipper', [
                {
                    Offline: true,
                    Search: $.trim($('#txtShipper').val()),
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (e) {
                        $('#txtShipper').val(e.CompanyName)
                        $('#txtShipper').data(e)
                    }
                }
            ])
        }

        function LoadRequiredData() {
            $('#selOrganization')
                .empty()
                .append(new Option(sessionStorage.OrgName, sessionStorage.OrgID))
                .append(new Option('3rd Party', 'third'))
                .append(new Option('Select another 3rd Party', 'anothird'))
                ;//.selectmenu('refresh')

            if (RecInTransFromDefault == 'internal') {
                CustomerID = sessionStorage.OrgID;
                CustomerName = sessionStorage.OrgName;

                if (Business.LibraryOrganizationFOWST.GetData().Rows.length == 1) {                    
                    $('#btnReceivingPlace').text(sessionStorage.OrgName);//.button('refresh');
                } else {
                    $('#btnReceivingPlace').text('Please Select');//.button('refresh');
                }
            } else {
                CustomerID = sessionStorage.OrgID;
                CustomerName = sessionStorage.OrgName;
            }

            $('#selReceivingPlaceOption, #selDepartment').empty()

            if (!AllowSRBadCount) {
                $('#dvStockViewerUnitMeasure').remove();
            }

            if (CBool(Business.SystemDefaults.GetSystemDefault('RecTransPackSizeDiscprepancy', 1))) {
                $('#divPackSizeDiscrepency').show();
            }

            ReceiveInInternalSetToDefault();
        }

        //remove OrganizationID when deployed
        function GetFowst(m_PropertyAndPlant) {

            if (OfflineMode) {
                vData = Business.LibraryOrganizationFOWST.GetData()

                $('#selReceivingPlace').empty();
                $('#selReceivingPlaceOption').empty();
                //$('#selReceivingPlace').append(new Option('Select', '', false));
                $('#selReceivingPlaceOption').append(new Option('Please Select', ''))

                for (var i = 0; i <= vData.Rows.length - 1; i++) {
                    $('#selReceivingPlace').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].FowstID, true));
                    $('#selReceivingPlace').val(vData.Rows[i].FowstID);
                    $('#selReceivingPlaceOption').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].FowstID, true));
                }

                $('#selReceivingPlace').append(new Option('Others', 'Others', false));

                try {
                    $('#selReceivingPlace');//.selectmenu('refresh');
                    $('#selReceivingPlaceOption');//.selectmenu('refresh');
                }
                catch (e) {
                    $('#selReceivingPlace');//.selectmenu();
                    $('#selReceivingPlaceOption');//.selectmenu();
                }

                //sessionStorage.FowstID = $('#selReceivingPlace').val();

                App.Deferred.fulfill('4UHHJWNPB9');                
            }

            App.model.Receiving.GetFowst({
                successFiltered: function (vData) {
                    if (!OfflineMode) {
                        $('#selReceivingPlace').empty();
                        $('#selReceivingPlace').append(new Option('Select', '', false));

                        for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {
                            $('#selReceivingPlace').append(new Option(vData.Results.Rows[i].FowstName, vData.Results.Rows[i].ID, false));
                        }

                        $('#selReceivingPlace').append(new Option('Others', 'Others', false));

                        try {
                            $('#selReceivingPlace');//.selectmenu('refresh');
                        }
                        catch (e) {
                            $('#selReceivingPlace');//.selectmenu();
                        }
                        App.Deferred.fulfill('4UHHJWNPB9');
                    }
                },
                success: function (vData) {
                    $('#selIntraOrganization').empty();
                    $('#selIntraOrganization').append(new Option('Select', '', false));

                    for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {
                        $('#selIntraOrganization').append(new Option(vData.Results.Rows[i].FowstName, vData.Results.Rows[i].ID, false));
                    }

                    try {
                        $('#selIntraOrganization');//.selectmenu('refresh');
                    }
                    catch (e) {
                        $('#selIntraOrganization');//.selectmenu();
                    }
                    App.Deferred.fulfill('4VL3L6F7FZ');
                }
            });

        }

        function GetLibraryOrganization() {
            IsCustomer = 1;

            BrowsePopup('LibraryOrganization', 'Search', [
                {
                    Offline: true,
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (e) {
                        $('#lblSourceOfSKU').val(e.CompanyName);
                        $('#lblSourceOfSKU').data(e);

                        CustomerID = e.ID;

                        setTimeout(function () {
                            GetThirdPartyOutlet(CustomerID)
                        }, 500);
                    }
                }
            ], {
                FilterCustomers: 1
            })
        };

        function GetThirdPartyOutlet(CustomerID) {
            Browser.LibraryFowst.InputData.IsThirdPartyOrganization = 1;
            Browser.LibraryFowst.InputData.ThirdPartyOrganizationID = CustomerID;
            Browser.LibraryFowst.Click = function (e) {

                $('#lblThirdPartyOutlet').val(e.FowstName);
                ThirdPartyOutlet = e.ID;

                setTimeout(function () {
                    GetThirdPartyOutletDepartment();
                }, 500);

            };
            Browser.LibraryFowst.Show();
        }

        function GetThirdPartyOutletDepartment() {
            //Browser.LibraryFowstBin.InputData.IsThirdPartyOrganization = 1;
            //Browser.LibraryFowstBin.InputData.ThirdPartyOrganizationID = ThirdPartyFOWST;
            Browser.OfflineMode = false;

            Browser.LibraryFowstBin.InputData.FowstID = ThirdPartyOutlet;
            Browser.LibraryFowstBin.Click = function (e) {

                $('#lblThirdPartyOutletDepartment').val(e.Name);
                ThirdPartyOutletDepartment = e.ID;
            };
            Browser.LibraryFowstBin.Show('');
        }

        function GetLibraryAndPlant() {
            Browser.LibraryFowst.Click = function (e) {
                $('#lblSourceOfSKU').val(e.Name)
                $('#lblSourceOfSKU').data(e)
                IsIntraOrganization = 1;
            };
            Browser.LibraryFowst.Show();
        };


        function GetDefaultSoureOfSKU() {
            var IsThirdPartyCompanyOrOrganization;
            var pSql = '';

            pSql = 'Select DefaultValue from private.SystemDefaults where Description = \'Receive From/Transfer To\' ';
            IsThirdPartyCompanyOrOrganization = CInt2(Sql.ExecuteScalar(pSql));

            if (CBool(IsThirdPartyCompanyOrOrganization)) {
                RecFromTransToDefault = 'external';
                $('#selSourceOfSKU').val('valThirdPartyVendor');//.selectmenu('refresh');
                isThirdPartyRecFromTransTo = true;


            } else {
                RecFromTransToDefault = 'internal';
                $('#selSourceOfSKU').val('valIntraOrganization');//.selectmenu('refresh');
                isThirdPartyRecInTransFrom = false;
            }

            pSql = 'Select DefaultValue from private.SystemDefaults where Description = \'Receive In/Transfer From\' ';
            
            if (CBool(CInt2(Sql.ExecuteScalar(pSql)))) {
                RecInTransFromDefault = 'external';
                isThirdPartyRecInTransFrom = true;
            } else {
                RecInTransFromDefault = 'internal';
                isThirdPartyRecInTransFrom = false;
            }

            ChangeSourceOfSKU();
            SRReceiveFromTransferToDefault();
        }

        function ChangeSourceOfSKU() {
            $('#selIntraOrganization').val('');
            $('#selIntraOrganization');//.selectmenu('refresh');
            $('#lblSourceOfSKU').data('ID', null);

            if ($('#selSourceOfSKU option:selected').val() == 'valIntraOrganization') {
                $('#dvIntraOrganization').css('visibility', 'visible');
                $('#dvIntraOrganization').css('display', 'block');
                $('#dvVendor').css('visibility', 'hidden');
                $('#dvVendor').css('display', 'none');
                IsIntraOrganization = 1;

                $('#lblSourceOfSKU').val('');
                $('#lblThirdPartyOutlet').val('');
                $('#lblThirdPartyOutletDepartment').val('');

                CustomerID = '';
                ThirdPartyOutlet = '';
                ThirdPartyOutletDepartment = '';

            }
            else if ($('#selSourceOfSKU option:selected').val() == 'valThirdPartyVendor') {
                $('#dvIntraOrganization').css('visibility', 'hidden');
                $('#dvIntraOrganization').css('display', 'none');
                $('#dvVendor').css('visibility', 'visible');
                $('#dvVendor').css('display', 'block');
                $('#lblSourceOfSKU').val('');
                //GetLibraryOrganization();
                IsIntraOrganization = 0;
            }
            else {
                $('#dvIntraOrganization').css('visibility', 'hidden');
                $('#dvIntraOrganization').css('display', 'none');
                $('#dvVendor').css('visibility', 'hidden');
                $('#dvVendor').css('display', 'none');
                //$('#dvIntraOrganization').css('visibility', 'hidden');
                //$('#dvVendor').css('visibility', 'hidden');
            }
        }

        function SRReceiveFromTransferToDefault() {
            if (CBool(Business.SystemDefaults.GetSystemDefault('SRUseThirdPartyAsSource'))) {
                RecFromTransToOrgID = Business.SystemDefaults.GetSystemDefault('SRThirdPartySupplier');
                RecFromTransToOrgName = Sql.ExecuteScalar("SELECT CompanyName FROM LibraryOrganization WHERE ID = " + ReadyString(RecFromTransToOrgID));
                $('#btnRecFromTransToPlace').text('3rd Party (' + RecFromTransToOrgName + ')');

                RecFromTransToFowstID = Business.SystemDefaults.GetSystemDefault('SRThirdPartySupplierOutlet');
                RecFromTransToFowstName = Sql.ExecuteScalar("SELECT Name FROM LibraryOrganizationFOWST WHERE ID = " + ReadyString(RecFromTransToFowstID));
                $('#selReceivingPlaceOption').text(RecFromTransToFowstName);

                $('#lblSourceOfSKU').data({ ID: RecFromTransToOrgID, Name: RecFromTransToOrgName });
            }
        }

        function ReceiveInInternalSetToDefault() {
            $('#txtDocumentType').val(Business.SystemDefaults.GetSystemDefault('SRDefaultSourceDocType'));

            if (CBool(Business.SystemDefaults.GetSystemDefault('IsReceiveInInternalSetToDefault'))) {
                var vFowst = Business.LibraryOrganizationFOWST.GetData().Rows[0];
                RecInTransFromOrgID = sessionStorage.OrgID;
                RecInTransFromOrgName = sessionStorage.OrgName;
                RecInTransFromFowstID = vFowst.FowstID;
                RecInTransFromFowstName = vFowst.FowstName;
                isThirdPartyRecInTransFrom = false;
                $('#btnRecInTransFromPlace').text(RecInTransFromOrgName + ' (' + RecInTransFromFowstName + ')');
            }
        }

        window.SaveReceiving = function (IsFromMultipleSizes, vItems, IsDraft) {
            var Items = [];
            var ctr = 0;
            var ctr2 = 0;
            IsResaleable = 0;

            //if (IsFromMultipleSizes) {
            //    ctr2 = 1;
            //    Items = vItems;
            //}

            if ($('#selIsResaleable').val() == 'ResaleableYes') {
                IsResaleable = 1;
            }

            if (IsDraft) {
                $('.txtCount').filter(function () {
                    return $.trim(this.value);
                }).each(function name(params) {
                    var data = $(this).data('data');
                    data.Count = $(this).val();
                    Items.push(data)
                })
            } else {
                $('.listProduct[iscounted="1"]').each(function () {
                    var disParent = $(this), uomcount, uomdata;

                    if (CInt2(disParent.attr('noofuom')) == 1) {
                        uomcount = disParent.find('.divUom input')
                        uomdata = uomcount.data('data');
                        Items.push({
                            ID: uomdata.ProductID,
                            UnitMeasureID: uomdata.ID,
                            ActualQuantity: uomdata.Quantity,
                            Quantity: CDec2(uomcount.val()),
                            QuantityBad: 0,
                            BadOrderReason: '',
                            AdditionalInfo: '',
                            IsResaleable: false,
                            ProductSerials: null,
                        })
                    } else {
                        var totalOtherUomCount = 0;
                        if (CBool(IsReceiving)) {
                            //for receiving, convert all UOMs count to largest UOM
                            uomcount = disParent.find('.divUom input:eq(0)');
                            uomdata = uomcount.data('data');                            

                            //exclude the largest UOM
                            disParent.find('.divUom input:not(:eq(0))').each(function () {
                                var dt = $(this).data('data');
                                totalOtherUomCount += (CDec2($(this).val()) * CDec2(dt.Quantity)) / CDec2(uomdata.Quantity);
                            })

                            Items.push({
                                ID: uomdata.ProductID,
                                UnitMeasureID: uomdata.ID,
                                ActualQuantity: uomdata.Quantity,
                                Quantity: CDec2(uomcount.val()) + CDec2(totalOtherUomCount),
                                QuantityBad: 0,
                                BadOrderReason: '',
                                AdditionalInfo: '',
                                IsResaleable: false,
                                ProductSerials: null,
                            })
                        } else {
                            //for transfer, convert all UOMs count to smallest UOM
                            uomcount = disParent.find('.divUom input:last');
                            uomdata = uomcount.data('data');

                            //exclude the largest UOM
                            disParent.find('.divUom input:not(:last)').each(function () {
                                var dt = $(this).data('data');
                                totalOtherUomCount += (CDec2($(this).val()) * CDec2(dt.Quantity));
                            })

                            Items.push({
                                ID: uomdata.ProductID,
                                UnitMeasureID: uomdata.ID,
                                ActualQuantity: uomdata.Quantity,
                                Quantity: CDec2(uomcount.val()) + CDec2(totalOtherUomCount),
                                QuantityBad: 0,
                                BadOrderReason: '',
                                AdditionalInfo: '',
                                IsResaleable: false,
                                ProductSerials: null,
                            })
                        }
                    }                    
                })
            }
            //$('#lstItems li').each(function (m_index) {
            //    if ($(this).data('units') != undefined) {
            //        while (ctr < $(this).data('units').length) {
            //            Items[ctr2] = { ID: $(this).attr('ID'),
            //                UnitMeasureID: $(this).data('units')[ctr].UnitMeasureID,
            //                ActualQuantity: $(this).data('units')[ctr].ActualQuantity, // * $(this).data('units')[ctr].Quantity,
            //                Quantity: $(this).data('units')[ctr].Quantity,
            //                QuantityBad: $(this).data('units')[ctr].QuantityBad,
            //                BadOrderReason: $(this).data('units')[ctr].BadOrderReason,
            //                AdditionalInfo: $(this).data('units')[ctr].AdditionalInfo,
            //                IsResaleable: $(this).data('units')[ctr].IsResaleable,
            //                ProductSerials: $(this).data('units')[ctr].ProductSerials,
            //            };
            //            ctr++;
            //            ctr2++;
            //        };
            //        ctr = 0;
            //    };

            //});

            for (var i in arrCountItems) {
                var isNew = true;

                $.grep(Items, function (e) {
                    if (e.ID == arrCountItems[i].ID) {
                        isNew = false;
                        e.Quantity = arrCountItems[i].Quantity;
                        e.QuantityBad = arrCountItems[i].QuantityBad;
                        return false;
                    }
                })

                if (isNew) {
                    Items.push(arrCountItems[i]);
                    ctr2++;
                }
            }

            //if (ctr2 == 0) {
            //    //AddToList();
            //    NewMessageBox("Please add item(s) to the cart before saving.",
            //    "Message",
            //    [
            //        { Text: 'Ok'
            //        }
            //    ]);
            //    return;
            //}
            if (CBool(IsReceiving)) {
                if ($('#txtDocumentType').val() == '' || $('#txtDocumentNo').val() == '' || $('#txtTransportedBy').val() == '') {
                    //$.mobile.changePage('#');
                    IsMainPage = false;
                    if (CBool(IsReceiving)) {
                        $('#btnInputItemDetails div').text('Save Receiving');
                    } else {
                        $('#btnInputItemDetails div').text('Save Transfer-Out');
                    }

                    return false;
                }
            }
            console.log(Items)
            //return
            var ReceivingPlace = '';

            //if (IsCustomer) {
            //    ReceivingPlace = CustomerID;
            //}
            //else {
            //    ReceivingPlace = $('#selReceivingPlace').val();
            //}

            var data = {
                ID: ID,
                IsNew: IsNew,
                IsReceiving: IsReceiving,
                ReceivingPlace: (CStr2(RecInTransFromFowstID) == "" ? RecInTransFromOrgID : RecInTransFromFowstID), // if there's a fowst save it, if non save orgid instead
                ReceivingPlaceName: RecInTransFromOrgName,
                SourceOfSKU: $('#lblSourceOfSKU').data('ID'),
                DocumentDate: $('#optMonthValue').val() + '/' + $('#optDayValue').val() + '/' + $('#optYearValue').val(),
                DocumentType: $('#txtDocumentType').val(),
                DocumentNo: $('#txtDocumentNo').val(),
                PONo: $('#txtPONo').val(),
                Is3PL: Is3PL,
                ShipperCompany: $('#txtShipper').data('ID'),
                Vessel: $('#txtVessel').val(),
                DatePrepared: $('#optMonthBLValue').val() + '/' + $('#optDayBLValue').val() + '/' + $('#optYearBLValue').val(),
                VoyageNumber: $('#txtVoyageNo').val(),
                BLNo: $('#txtWayBill').val(),
                InvoiceDate: $('#optMonthInvoiceValue').val() + '/' + $('#optDayInvoiceValue').val() + '/' + $('#optYearInvoiceValue').val(),
                InvoiceNo: $('#txtInvoiceNo').val(),
                FAndH: $('#txtFreightAndHandling').val(),
                Arrastre: $('#txtArrastre').val(),
                Total: $('#txtTotal').val(),
                TotalCC: $('#txtTotalCC').val(),
                TotalCBM: $('#txtTotalCBM').val(),
                TotalWeightInKg: $('#txtTotalWeightInkg').val(),
                TotalCostBundle: $('#txtTotalCostofBundle').val(),
                Bundles: $('#txtBundles').val(),
                Items: Items,
                IsResaleable: IsResaleable,
                IsResaleableAnswer: $('#selResaleableAnswer').children('option:selected').text(),
                IsResaleableRemarks: $('#txtResaleableNo').val(),
                TransportedBy: $('#txtTransportedBy').val(),
                IsFromTransferOut: CInt2(IsFromTransferOutOrDraftReceiving),
                IsDraft: IsDraft,
                POID: POID,
                ReceiveInOrgID: (CStr2(RecInTransFromFowstID) == "" ? "" : RecInTransFromOrgID),
                ReceiveInOutlet : '',
                ReceiveInDepartment: RecInTransFromDepartment,
                ReceiveFromOutlet: RecFromTransToFowstID,
                ReceiveFromDepartment: RecFromTransToDepartment,
                IsShortLanded: CBool($('#radPSDShortLanded').is(':checked')),
                IsOverLanded: CBool($('#radPSDOverLanded').is(':checked'))
            };

            Bs.Loading('show', 'Saving... please wait');
            setTimeout(function () {
                App.model.Receiving.Save({
                    data: data,
                    success: function (vData) {
                        var msg;
                        var RefNum;
                        Bs.Loading('hide');
                        if (IsGuid(vData.Save.split('|')[1])) {
                            ID = vData.Save;

                            try {
                                $('#selReceiveOrTransfer').val('ReceiveItems');//.selectmenu('refresh');
                            } catch (e) {
                                $('#selReceiveOrTransfer').val('ReceiveItems');//.selectmenu();
                            }


                            if (IsMobile) {
                                if (IsReceiving == 1) {
                                    RefNum = 'RCV #' + vData.Save.split('|')[0]
                                } else {
                                    RefNum = 'T-OUT #' + vData.Save.split('|')[0]
                                }
                                if (IsDraft == 1) {
                                    //draft message
                                    if (IsReceiving == 1) {
                                        NewMessageBox('Receiving transaction has been successfully saved as \'Draft\'.', '', [{
                                            Text: 'Ok' }], {
                                            Close: function () {
                                                IsSearchFromTransFerOut = false;
                                                ClearFields();
                                                $('#pageMain').pageshow();
                                            }
                                        });
                                    } else {
                                        NewMessageBox('Transfer Out transaction has been successfully saved as \'Draft\'.', '', [{
                                            Text: 'Ok', }], {
                                            Close: function () {
                                                IsSearchFromTransFerOut = false;
                                                ClearFields();
                                                $('#pageMain').pageshow();
                                            }
                                        });
                                    }

                                } else {
                                    NewMessageBox('Your transaction has been successfully saved. ' + RefNum +
                                        '. This transaction record will be flushed to the Mobiz Cloud automatically when you have an Internet connection. '
                                    , 'Save',
                                    [
                                        {
                                            Text: 'Ok'
                                        }
                                    ], {
                                        Close: function () {
                                            ClearFields();
                                            $('#pageMain').pageshow();
                                        }
                                    }); // END NewMessageBox
                                }

                                LoadRequiredData();
                            } else {
                                if (IsReceiving == 1) {
                                    //msg = 'Your Receiving transaction has been saved.  Reference for this transaction is RCV #' + vData.Save;
                                    msg = 'Your Receiving transaction has been saved. Reference for this transaction is RCV #' + vData.Save.split('|')[0] + '. Do you want to download PDF file of RCV #' + vData.Save.split('|')[0] + '?' //'. Do you want to print receiving report?'
                                }
                                else {
                                    //msg = 'Your Transfer Out transaction has been saved.  Reference for this transaction is T-OUT #' + vData.Save;
                                    msg = 'Your Receiving transaction has been saved. Reference for this transaction is T-OUT #' + vData.Save.split('|')[0] + '. Do you want to download PDF file of T-OUT #' + vData.Save.split('|')[0] + '?' //'. Do you want to print transfers\' report?'
                                }


                                NewMessageBox(msg,
                                "Save",
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            GetReportInfo();
                                            $('#pageMain').pageshow();
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            ClearFields();
                                            $('#pageMain').pageshow();
                                        }
                                    }
                                ]); // END NewMessageBox

                            }

                            if (OfflineMode)
                                App.model.Receiving_offline.FlushData();

                        }
                        else {
                            msg = 'An error occurred while saving this transaction.';
                            NewMessageBox(msg,
                            "Message",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]); // END NewMessageBox
                        }
                    } // END success
                }); // END App.model.Receiving.Save({
            }, 200)            

        }; // END window.SaveReceiving

        function GetReportInfo() {
            if (InAndroid) {

                var vRetDataHeader = GetReportHeaderInfo();
                var vRetDataReceivingInfo = GetAllReceiving();
                var vRetDataReceivingDetails = GetAllReceivingSKU();

                PrintReport(vRetDataHeader.Results, vRetDataReceivingInfo.Results, vRetDataReceivingDetails.Results);
            } else {
                //Get Header
                AjaxSend({
                    url: "Receiving/GetReportHeaderInfo",
                    data: { OrgID: sessionStorage.OrgID, FowstID: $('#selReceivingPlace').val() },
                    success: function (vRetDataHeader) {
                        //Get Receiving Info
                        var mID = ID.split('|');

                        if (mID.length > 1) {
                            ID = mID[1];
                        }

                        AjaxSend({
                            url: "Receiving/GetAllReceiving",
                            data: { ID: ID },
                            success: function (vRetDataReceivingInfo) {
                                AjaxSend({
                                    url: "Receiving/GetAllReceivingSKU",
                                    data: { ID: ID },
                                    success: function (vRetDataReceivingDetails) {
                                        //PrintReport(vRetDataHeader.Results, vRetDataReceivingInfo.Results, vRetDataReceivingDetails.Results);
                                        GeneratePDFReport(vRetDataHeader.Results, vRetDataReceivingInfo.Results, vRetDataReceivingDetails.Results)

                                        NewMessageBox('Were you able to download the PDF file?',
                                                   "Save",
                                                   [
                                                       {
                                                           Text: 'Yes',
                                                           Click: function () {
                                                               ClearFields();
                                                               ID = ''; //place here para makuha muna ang report info before maclear ang Receiving ID
                                                               //$.mobile.changePage('#');
                                                           }
                                                       },
                                                       {
                                                           Text: 'No',
                                                           Click: function () {
                                                               GetReportInfo();
                                                               //$.mobile.changePage('#');
                                                           }
                                                       }
                                                    ]); // END NewMessageBox


                                    }
                                });
                            }
                        });
                    }
                });
            }
            //                }
            //            });
        }

        function GetReportHeaderInfo() {
            //return App.model.Receiving.
        }

        function GeneratePDFReport(vRetDataHeader, vRetDataReceivingInfo, vRetDataReceivingDetails) {

            sessionStorage.mHeader = JSON.stringify(vRetDataHeader);
            sessionStorage.mReceivingInfo = JSON.stringify(vRetDataReceivingInfo);
            sessionStorage.mReceivingDetails = JSON.stringify(vRetDataReceivingDetails);

            var f = window.open('Receiving/GeneratePDF', '_blank');
            //f.Test

            //window.open('Receiving/GeneratePDF', '_blank');
        }

        function PrintReport(vRetDataHeader, vRetDataReceivingInfo, vRetDataReceivingDetails) {

            var intFAFCTotalLenght = 40;

            NewPrintBuilder();

            vPrintString = FormatTextToPrint(intFAFCTotalLenght, $.trim((vRetDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            ReadyTextToPrint(vPrintString, 3, 1, 1, 1, 1, 1, 0, 0, 0);

            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vRetDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString = strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vRetDataHeader.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vRetDataHeader.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vRetDataHeader.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 1, 0, 0, 0);

            if (CBool(vRetDataReceivingInfo.Rows[0].IsReceiving)) {
                strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nReceiving No.' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vRetDataReceivingInfo.Rows[0].DateReceived || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;
                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 1, 1, 0, 0, 0);
                //5th para = isbold
                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Receive In: ' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingPlace || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                if (vRetDataReceivingInfo.Rows[0].SourceOfSKU1 != '') {
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Receive From: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU1 || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    vPrintString += strTemp;
                }
                else {
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Receive From: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU2 || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    vPrintString += strTemp;
                }
            }
            else {
                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Transfer No.' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date Transfered: ' + $.trim((vRetDataReceivingInfo.Rows[0].DateReceived || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString += strTemp;
                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 1, 1, 0, 0, 0);
                //5th para = isbold
                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Transfer From: ' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingPlace || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString += strTemp;

                if (vRetDataReceivingInfo.Rows[0].SourceOfSKU1 != '') {
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Transfer To: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU1 || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    vPrintString += strTemp;
                }
                else {
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Transfer To: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU2 || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    vPrintString += strTemp;
                }

            }

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Document Date: ' + $.trim(($.format.date(new Date(vRetDataReceivingInfo.Rows[0].DocumentDate), 'MM/dd/yyyy') || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Document Type: ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentType || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Document No: ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString += strTemp;

            if (CBool(vRetDataReceivingInfo.Rows[0].Is3PL)) {
                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Shipper: ' + $.trim((vRetDataReceivingInfo.Rows[0].CompanyName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString += strTemp;
            }

            ReadyTextToPrint('\n' + vPrintString, 3, 1, 1, 0, 0, 0, 0, 0);

            strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
            vPrintString = strTemp;

            ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 0, 0, 0);
            //5th para = isbold
            vPrintString = '';

            for (i = 0; i <= vRetDataReceivingDetails.Rows.length - 1; i++) {
                strTemp = FormatTextToPrint(intFAFCTotalLenght, vRetDataReceivingDetails.Rows[i].ProductName, 28, '', intFAFCTotalLenght, '', 0, 0);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, vRetDataReceivingDetails.Rows[i].QuantityGood, 6, vRetDataReceivingDetails.Rows[i].Label, 10, '', intFAFCTotalLenght, 0);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 1, 1, 0, 0, 0, 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                vPrintString = strTemp;

                ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 0, 0, 0, 0);
            }

            //            strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nReceived By: ' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivedBy || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            //            vPrintString = strTemp;

            //            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Transported By: ' + $.trim((vRetDataReceivingInfo.Rows[0].TransportedBy || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            //            vPrintString += strTemp;

            //            ReadyTextToPrint(vPrintString, 3, 1, 1, 0, 0, 1, 0, 0, 0);

            //            strPrint = SetPrefixPostfix('ID: ' + $.trim((vRetDataReceivingInfo.Rows[0].ID || '')) + '|OrgID: ' + $.trim((sessionStorage.OrgID || '')), 12, '0', 0)
            strPrint = '';
            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 1);

            PrintText('192.168.1.188');
            PrintText(GetFowstPrinter());
        }

        function GetFowstPrinter() {
            AjaxSend({
                url: '/Organization/getOrgFOWSTPrinterIP',
                data: { FowstID: $('#selReceivingPlace').val() },
                success: function (vData) {
                    return vData.OrganizationFOWSTData.Rows[0].PrinterIPAddress
                }
            });
        }

        function PosCart(vData) {

            var pInvoiceID;
            var pDsCart;
            var pOut;

            pInvoiceID = vData.OrderID; // yes, OrderID is not a typo

            if (IsGuid(pInvoiceID)) {
                if (!Business.TransInvoice.IsValidCart(pInvoiceID))
                    return { 'text': 'No item' };
            } else {
                if (CBool(vData.DontFetchIfBlank))
                    return { 'text': 'No item' };
            }

            pDsCart = Business.TransInvoice.GetCart(pInvoiceID);
            if (pDsCart != undefined) {
                pOut = {};
                pOut.Master = pDsCart.Invoices.Rows[0];
                pOut.Units = pDsCart.Stocks;
                pOut.Stocks = JSON.parse(JSON.stringify(pOut.Units)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/
                pOut.Stocks.Rows = GetDistinct(pOut.Stocks.Rows, ['ProductID', 'ProductNo', 'ProductName', 'ModifierGroupID']);
                pOut.Modifiers = pDsCart.Modifiers;
                //AttachModifers(pDsCart, pOut);
                proj.CartData = pOut;

                return pOut;
            } else
                return { 'text': 'No item' };

        }; // PosCart

        proj.ReceivingBackButton_Click = function() {
            if (mShowWithQtyOnly)
                DisplayRawMats(vRowsRawMats, null, false);
            else
                $('#pageMain').pageshow();

        }
    });

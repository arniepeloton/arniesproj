define([
    'common',
    GetPathFormat('/Scripts3/Browser/TransReceivingAndTransfers.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowstBin.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js'),
    GetPathFormat('/Scripts3/Browser/LibrarySupplier.js'),
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    'promise!/Scripts3/model/Receiving.js?crc=',
    '/Scripts/sources/Receiving/Cart.js?crc=',
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
                            MessageBox("This feature requires internet connection jsXZ2LAXXRE3!",
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
                $('#selReceiveOrTransfer').selectmenu('refresh');
            } catch (e) {
                $('#selReceiveOrTransfer').selectmenu();
            }

            $('#lblReceivingPlace').text('Receive In: ');
            $('#lblSKUSource').text('Receive From: ');

            ClearFields();
            LoadRequiredData();

            GetFowst();            

            if (IsMobile) {
                GetDefaultSoureOfSKU();
            }           
            
            startclock('#clockStock');
            startclock('#clockReceivingCart');
            //UpdateContent();

            var pPage = $.mobile.activePage;
            var pHeader = pPage.find("[data-role=header]");
            var pHeight = pHeader.height() + 10;

            pPage.css('padding-top', pHeight + 'px');

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
            setDayCollection('optMonthValue', 'optDayValue', 'optYearValue');
            $('#optDayValue').val(m_Date.getDate());

            $('#optMonthBLValue').val(m_Date.getMonth() + 1);
            $('#optYearBLValue').val(m_Date.getFullYear());
            setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue');
            $('#optDayBLValue').val(m_Date.getDate());

            $('#optMonthInvoiceValue').val(m_Date.getMonth() + 1);
            $('#optYearInvoiceValue').val(m_Date.getFullYear());
            setDayCollection('optMonthInvoiceValue', 'optDayInvoiceValue', 'optYearInvoiceValue');
            $('#optDayInvoiceValue').val(m_Date.getDate());

            try {
                $('#optMonthValue').selectmenu('refresh');
                $('#optYearValue').selectmenu('refresh');
                $('#optDayValue').selectmenu('refresh');

                $('#optMonthBLValue').selectmenu('refresh');
                $('#optYearBLValue').selectmenu('refresh');
                $('#optDayBLValue').selectmenu('refresh');

                $('#optMonthInvoiceValue').selectmenu('refresh');
                $('#optDayInvoiceValue').selectmenu('refresh');
                $('#optYearInvoiceValue').selectmenu('refresh');
            }
            catch (e) {
                $('#optMonthValue').selectmenu();
                $('#optYearValue').selectmenu();
                $('#optDayValue').selectmenu();

                $('#optMonthBLValue').selectmenu();
                $('#optYearBLValue').selectmenu();
                $('#optDayBLValue').selectmenu();

                $('#optMonthInvoiceValue').selectmenu();
                $('#optDayInvoiceValue').selectmenu();
                $('#optYearInvoiceValue').selectmenu();
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
                                    $('#selReceivingPlace').selectmenu('refresh');
                                }
                                catch (e) {
                                    $('#selReceivingPlace').selectmenu();
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
                        $('#selIntraOrganization').selectmenu('refresh');
                    } catch (e) {
                        $('#selIntraOrganization').selectmenu();
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
                $.mobile.changePage('#pageCart');
            })

            $('#txtTransportedBy').keypress(function (e) {
                if (e.keyCode == 13) {
                    e.preventDefault();
                    Browser.LibraryPeople.Click = function (e) {
                        $('#txtTransportedBy').val(e.PeopleName);
                    };
                    Browser.LibraryPeople.Show($(this).val());
                }
            });

            $('#btnPlusOne').css('height', '75%');

            $('#btnInputItemDetails').click(function () {

                if ($('#selReceivingPlace').val() == '') {
                    if (CustomerID == '') {
                        if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
                            MessageBox("Please select the location to receive items.",
                        "Receive In",
                        [
                            { Text: 'Ok' }
                        ]
                    );
                            return;
                        } else {
                            MessageBox("Please select the source of items.",
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
                        MessageBox("Please select the source of items.",
                                "Receive From",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        return;
                    } else {
                        MessageBox("Please select the location to transfer items.",
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
                            MessageBox("Please put Document Date",
                                        "Error",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                            return;
                        }

                        if ($.trim($('#txtDocumentType').val()) == '') {
                            MessageBox("Please enter document type.",
                                    "Document Type",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                            return;
                        }

                        if ($.trim($('#txtDocumentNo').val()) == '') {
                            MessageBox("Please enter document number.",
                                    "Document Number",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                            return;
                        }

                    }
                    if ($.trim($('#txtTransportedBy').val()) == '') {
                        MessageBox("Please enter transported by.",
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

                sessionStorage.FowstID = $('#selReceivingPlace').val();
                $.mobile.changePage('#pageCart');

                $("#lstReceiving").empty();
                $('#txtSearchItem').val('');


                if (IsFromTransferOutOrDraftReceiving) {
                    if (ReceivingSKU != undefined) {
                        GetSystemBehaviorSettings();
                        DisplayReceivingSKU(ReceivingSKU, IsAutomaticReceived, IsShowQuantityOfStocks, ID);
                    }
                }



                if (CBool(IsReceiving)) {
                    try {
                        $('#btnDone .ui-btn-text').text('Save Receiving Cart');

                    } catch (e) {
                        //$('#btnDone .ui-btn-text').button();
                    }
                }
                else {
                    try {
                        $('#btnDone .ui-btn-text').text('Save Transfer-Out Cart');
                    }
                    catch (e) {
                        //$('#btnDone').button();
                    }

                }

                try {
                    //$('#btnDone').button('refresh');
                    $('#lstItems').listview('refresh');
                }
                catch (e) {
                    //$('#btnDone').button();
                    $('#lstItems').listview();
                }
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
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', true);
            });

            $('#optYearValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', true);
            });

            $('#optMonthBLValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', true);
            });

            $('#optYearBLValue').change(function (e) {
                e.preventDefault();
                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', true);
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
                //    MessageBox("Please enter Transported By.",
                //           "Transported By",
                //            [
                //                { Text: 'Ok'
                //                }
                //            ]
                //            );
                //    return false;
                //}

                if (CStr2($.trim($('#lblSourceOfSKU').data('ID'))) == '') {
                    MessageBox("Please select source of SKU.",
                           "Source of SKU",
                            [
                                { Text: 'Ok'
                                }
                            ]
                            );
                    return false;
                }

                PopupButtonList('', 'Save Options', [
                {
                    HasImage: true,
                    Text: '<img src="/Content/images/savefinalized.jpg" align="absmiddle" class="ui-li-thumb" /> <b>Save and Finalize Transaction</b> <br /> <div style="text-align:left !important; font-size:14px; font-weight:normal;">Save and send to head office. Transaction can be searched but not edited after saving.</div>',
                    Click: function () {
                        SaveReceiving(false, null, 0);
                    }
                },
                {
                    HasImage: true,
                    Text: '<img src="/Content/images/savedraft.jpg" align="absmiddle" class="ui-li-thumb" /> <b>Save as Draft Transaction</b> <br /> <div style="text-align:left !important; font-size:14px; font-weight:normal;">Will not be sent to the head office. Transaction can still be searched and edited.</div>',
                    Click: function () {
                        SaveReceiving(false, null, 1);
                    }
                }
                ], { AutoDismiss: true })

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
                    MessageBox('This feature is for mobile device only')
                }

            });

            $('#txtSearchStockTransfer').keypress(function (e) {
                if (e.keyCode = 13) {
                    IsSearchFromTransFerOut = true;
                    SearchTransferOut();                    
                }
            });

            function SearchTransferOut(vIssueHistoryBack) {
                Browser.OfflineMode = false;
                Browser.TransReceivingAndTransfers.Click = function (e) {
//                    if (vIssueHistoryBack)
//                        history.back();
                    ID = e.ID;
                    IsGetDataFromServer = true
                    GetReceivingOrTransferOutInfo();
                    IsFromTransferOutOrDraftReceiving = true;
                    
                };
                Browser.TransReceivingAndTransfers.Show($('#txtSearchStockTransfer').val());
            }

            $('#btnRecordLookup').click(function () {
                if (CBool(IsReceiving)) {
                    $('#lblRecordSearchType').html('Receiving');
                    $('#lblRecordSearchDescription').html('View Finalized Receiving Transactions or Continue working on Draft Receiving Transactions.')
                } else {
                    $('#lblRecordSearchType').html('Transfer Out');
                    $('#lblRecordSearchDescription').html('View Finalized Transfer Out Transactions or Continue working on Draft Transfer Out Transactions.')
                }
                $('#popupMoreOptions').popup('open');
            });

            $('#btnPOPopup').click(function () {
                var pBrowser = Browser.TransInvoices;

                pBrowser.InputData.CustomerID = sessionStorage.FowstID;
                pBrowser.InputData.PoMode = true;
                pBrowser.InputData.IsFinalizeOnly = true
                pBrowser.InputData.IsFilterReceived = true
                pBrowser.Click = function (e) {
                    POID = e.ID;
                    IsNew = 1;

                    $('#selReceivingPlace').val(e.FOWSTID).selectmenu('refresh');

                    $('#selSourceOfSKU').val('valThirdPartyVendor').selectmenu('refresh');
                    SourceOfSKUID = e.CustomerID;
                    SourceOfSKU = e.CompanyName;
                    $('#dvVendor').css('visibility', 'visible');
                    $('#dvVendor').css('display', 'block')
                    $('#dvIntraOrganization').css('visibility', 'hidden');
                    IsIntraOrganization = 0;

                    data = { ID: e.CustomerID };
                    $('#lblSourceOfSKU').val(SourceOfSKU)
                    $('#lblSourceOfSKU').data(data);

                    $('#txtDocumentType').val('PO');
                    $('#txtDocumentNo').val(e.PurchaseOrderNo);
                    $('#txtPONo').val(e.PurchaseOrderNo);

                    m_Date = new Date(e.InvoiceDate);

                    $('#optMonthValue').val(m_Date.getMonth() + 1)
                    $('#optYearValue').val(m_Date.getFullYear())
                    setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
                    $('#optDayValue').val(m_Date.getDate())

                    $('#optMonthValue').selectmenu('refresh');
                    $('#optYearValue').selectmenu('refresh');
                    $('#optDayValue').selectmenu('refresh');

                    var pData = {};

                    pData.PosMode = "so";
                    //end of additional

                    pData.CustomerID = sessionStorage.OrgID;
                    pData.OrderID = e.ID;
                    pData.FowstID = sessionStorage.FowstID;

                    vRetDataReceivingDetails = PosCart(pData);

                    ReceivingSKU = {};
                    ReceivingSKU.Rows = [];

                    for (var i = 0; i <= vRetDataReceivingDetails.Units.Rows.length - 1; i++) {
                        ReceivingSKU.Rows.push({
                            ID: vRetDataReceivingDetails.Units.Rows[i].ProductID,
                            ProductNo: vRetDataReceivingDetails.Units.Rows[i].ProductNo,
                            UnitMeasureGood: vRetDataReceivingDetails.Units.Rows[i].ItemID,
                            ActualQuantityGood: CDec2(vRetDataReceivingDetails.Units.Rows[i].ActualQuantity) * CDec2(vRetDataReceivingDetails.Units.Rows[i].OrderedQuantity),
                            ProductName: vRetDataReceivingDetails.Units.Rows[i].ProductName,
                            QuantityGood: CDec2(vRetDataReceivingDetails.Units.Rows[i].OrderedQuantity),
                            Label: vRetDataReceivingDetails.Units.Rows[i].UnitMeasure
                        });
                    }

                    ReceivingSKU = ReceivingSKU.Rows;

                    IsFromTransferOutOrDraftReceiving = true;

                    history.back();
                };

                pBrowser.Show();

            })

            $('#btnReceivingPopup').click(function () {
                POID = '';

                Browser.OfflineMode = true;
                Browser.TransReceivingAndTransfers.InputData.IsReceiving = IsReceiving;
                Browser.TransReceivingAndTransfers.AfterClick = function (e) {
                    ID = e.ID;
                    IsDraft = e.IsDraft;
                    IsFromTransferOutOrDraftReceiving = false;
                    GetReceivingOrTransferOutInfo();
                    IsFromTransferOutOrDraftReceiving = true;
                    IsNew = false;
                };
                Browser.TransReceivingAndTransfers.Show();

            });

            $('#butTransferOutSearchType').click(function () {
                IsSearchFromTransFerOut = true;
                SearchTransferOut(true);
            });

            $('#btnRORecordSearch').click(function () {
                POID = '';

                Browser.OfflineMode = true;
                Browser.TransReceivingAndTransfers.InputData.IsReceiving = IsReceiving;
                Browser.TransReceivingAndTransfers.AfterClick = function (e) {
                    ID = e.ID;
                    IsDraft = e.IsDraft;
                    GetReceivingOrTransferOutInfo();
                    IsFromTransferOutOrDraftReceiving = true;
                    IsNew = false;
                };
                Browser.TransReceivingAndTransfers.Show();
            });

            $('#btnNewReceivingCart').click(function () {
                $.mobile.changePage('#pageMain');
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
                    MessageBox("Please select vendor before adding an outlet.")
                }
            });

            $('#lblThirdPartyOutletDepartment').keypress(function (e) {
                if (e.keyCode == 13 && ThirdPartyOutlet != '') {
                    GetThirdPartyOutletDepartment();
                } else {
                    MessageBox("Please select outlet before adding a department.")
                }
            });

            $('#btnAddOutlet').click(function () {
                $('#panAddOutlet').popup('open');
            });
            
            $('#btnRecInTransFromPlace').click(function () {
                isRecInTransFrom = true;
                $('#txtTitlePopup').text($('#lblReceivingPlace').text().replace(/:/g, ''));
                if (RecInTransFromDefault == 'internal') {
                    $('#popReceivingTransferOption').popup('open');
                    if (isThirdPartyRecInTransFrom) {
                        $('#selOrganization').val('third');
                        RecInTransFromOrgID = RecInTransFromOrgID;
                        RecInTransFromOrgName = RecInTransFromOrgName;
                        GetOrgFowst(RecInTransFromOrgID, true);
                        $('#selOrganization option[value="third"]').text('3rd Party (' + RecInTransFromOrgName + ')');
                    }
                    else {
                        $('#selOrganization').val(RecInTransFromOrgID);
                        RecInTransFromOrgID = $('#selOrganization').val();
                        RecInTransFromOrgName = $('#selOrganization').find('option:selected').text();
                        GetOrgFowst(RecInTransFromOrgID, false);
                        $('#selOrganization option[value="third"]').text('3rd Party');
                    }
                    $('#selReceivingPlaceOption').val(SelectedOutlet).selectmenu('refresh');
                } else {
                    if (RecInTransFromOrgID != '') {
                        //if user already select a third party org, show popup instead browser search page
                        $('#popReceivingTransferOption').popup('open');
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
                        $('#selReceivingPlaceOption').val(SelectedOutlet).selectmenu('refresh');
                    } else {
                        ThirdPartyOrg();
                    }                    
                }               

                if (RecInTransFromFowstID != '') {
                    if (IsMobile) {
                        GetDepartment(RecInTransFromFowstID);
                    }
                }
                $('#selOrganization').selectmenu('refresh');
                $('#selDepartment').val(RecInTransFromDepartment).selectmenu('refresh');
                $('#msgOrganization, #msgOutlet').hide();
            })

            $('#btnRecFromTransToPlace').click(function () {
                isRecInTransFrom = false;
                $('#txtTitlePopup').text($('#lblSKUSource').text().replace(/:/g, ''));
                if (RecFromTransToDefault == 'internal') {
                    $('#popReceivingTransferOption').popup('open');
                    if (isThirdPartyRecFromTransTo) {
                        $('#selOrganization').val('third').selectmenu('refresh');
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
                    $('#selReceivingPlaceOption').val(SelectedOutlet).selectmenu('refresh');
                } else {
                    if (RecFromTransToOrgID != '') {
                        //if user already select a third party org, show popup instead browser search page
                        $('#popReceivingTransferOption').popup('open');
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
                        $('#selReceivingPlaceOption').val(SelectedOutlet).selectmenu('refresh');
                    } else {
                        ThirdPartyOrg();
                    }
                }                

                if (RecFromTransToFowstID != '') {
                    if (IsMobile) {
                        GetDepartment(RecFromTransToFowstID);
                    }
                }
                $('#selOrganization').selectmenu('refresh');
                $('#selDepartment').val(RecFromTransToDepartment).selectmenu('refresh');
                $('#msgOrganization, #msgOutlet').hide();
            })

            $('#selOrganization').change(function (e) {
                if ($(this).val() == "third") {
                    $(this).find('option:first').attr('selected', 'selected');
                    $(this).selectmenu('refresh');

                    $('#popReceivingTransferOption').popup('close');
                    ThirdPartyOrg();
                } else {                   
                    IsCustomer = 0;
                    isSelectedThirdParty = false;
                    SelectedOrgID = ($(this).val() == '' ? undefined : $(this).val());
                    SelectedOrgName = $(this).find('option:selected').text();
                    GetOrgFowst($(this).val(), false) // load fowst of selected organization
                    $('#selOrganization option[value="third"]').text('3rd Party')
                }
                PopupReposition();
            })

            $('#selReceivingPlaceOption').change(function () {
                SelectedOutlet = ($(this).val() == '' ? undefined : $(this).val());
                SelectedOutletName = $(this).find('option:selected').text();
                PopupReposition();
                if (IsMobile) {
                    GetDepartment($(this).val()) // get department or bin of selected fowst
                }                
            })

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
                        $('#btnRecInTransFromPlace').text(RecInTransFromOrgName + ' (' + RecInTransFromFowstName + ')').button('refresh');
                    } else {
                        $('#btnRecInTransFromPlace').text('3rd Party (' + RecInTransFromOrgName + ')').button('refresh')
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
                        $('#btnRecFromTransToPlace').text(RecFromTransToOrgName + ' (' + RecFromTransToFowstName + ')').button('refresh');
                        data = { ID: $('#selReceivingPlaceOption').val(), Name: $('#selReceivingPlaceOption option:selected').text() };
                    } else {
                        $('#btnRecFromTransToPlace').text('3rd Party (' + RecFromTransToOrgName + ')').button('refresh')
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
                $('#popReceivingTransferOption').popup('close');
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
                } else { ClearFields }
            })
        });

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

        function GetDepartment(id) {
            //Department is also equal to BIN Location
            var vData = Business.LibraryOrganizationFOWST.GetFowstBin(id).Rows;
            var selDepartment = $('#selDepartment');
            selDepartment.empty();
            selDepartment.append(new Option('Please Select', ''))

            for (var i = 0; i < vData.length; i++){
                selDepartment.append(new Option(vData[i].Name, vData[i].ID))
            }

            selDepartment.selectmenu('refresh');

            if(isRecInTransFrom){
                $('#selDepartment').val(RecInTransFromDepartment).selectmenu('refresh');
            } else {
                $('#selDepartment').val(RecFromTransToDepartment).selectmenu('refresh');
            }
        }

        function ThirdPartyOrg() {
            //Browser.LibraryOrganization.FilterCustomers = 1;
            Browser.LibraryOrganizationSupplier.Click = function (e) {
                $('#selOrganization').val('third').selectmenu('refresh');
                // append the Company Name to third party option, so that the user will know what 3rd party org he/she selected
                $('#selOrganization option[value="third"]').text('3rd Party (' + e.CompanyName + ')');
                $('#selOrganization').val('third').selectmenu('refresh');

                IsCustomer = 1;

                SelectedOrgID = e.ID;
                SelectedOrgName = e.CompanyName;
                isSelectedThirdParty = true;

                if (IsMobile) {
                    GetOrgFowst(e.ID, true)
                }
                
                setTimeout(function () {
                    $('#popReceivingTransferOption').popup('open');                    
                    PopupReposition();
                }, 100)
            };
            Browser.LibraryOrganizationSupplier.Show();
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
        
            var selReceivingPlaceOption = $('#selReceivingPlaceOption');
            selReceivingPlaceOption.empty();
            selReceivingPlaceOption.append(new Option('Please Select', ''))

            for (var i = 0; i < vData.length; i++) {
                selReceivingPlaceOption.append(new Option(vData[i].FowstName, (vData[i].ID || vData[i].FowstID)))
            }           

            selReceivingPlaceOption.selectmenu('refresh');
            $('#selDepartment').empty().append(new Option('Please Select', '')).selectmenu('refresh');

            if (vData.length == 1) {
                //set default if one fowst
                SelectedOutlet = (vData[0].ID || vData[0].FowstID);
                SelectedOutletName = vData[0].FowstName;
                if (!isRecInTransFrom) {
                    // Receive From or Transfer To only
                    var data = { ID: (vData[0].ID || vData[0].FowstID), Name: vData[0].FowstName };
                    $('#lblSourceOfSKU').data(data);
                }
                selReceivingPlaceOption.val((vData[0].ID || vData[0].FowstID)).selectmenu('refresh');

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

                $('#btnInputItemDetails .ui-btn-text').html('<div><img alt="home" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></div><div style="font-size:13px; margin-top:10px;">Go To Receiving Cart</div>');

                $('#ReceivingTitle').text('Receiving Cart');
                $('#dvGoodCount').html('Good Count');
                $('#dvpageCartHeader').html('Receiving Cart');

                $('#headerInformation').css('visibility', 'visible');
                $('#headerInformation').css('display', 'block');
            }
            else {

                $('#lblReceivingPlace').text('Transfer From: ');
                $('#lblSKUSource').text('Transfer To: ');
                IsReceiving = 0;

                $('#dvReceiving').css('visibility', 'hidden')
                $('#dvReceiving').css('display', 'none')

                $('#dvTransferOut').css("visibility", "visible")
                $('#dvTransferOut').css("display", "inline")

                $('#btnInputItemDetails .ui-btn-text').html('<div><img alt="home" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></div><div style="font-size:13px; margin-top:10px;">Go To Transfer Out Cart</div>');

                $('#selResaleableAnswer').css('visibility', 'hidden');
                //$('#selResaleableAnswer').css('display', 'none');

                $('#ReceivingTitle').text('Transfer-Out Cart');
                $('#dvGoodCount').html('Count');
                $('#dvpageCartHeader').html('Transfer-Out Cart');

                $('#headerInformation').css('visibility', 'hidden');
                $('#headerInformation').css('display', 'none');

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
                $('#selResaleableAnswer').selectmenu('refresh');
            }
            catch (e) {
                $('#selResaleableAnswer').selectmenu();
            }

        }

        function GetReceivingOrTransferOutInfo() {            

            if (OfflineMode && !IsGetDataFromServer) {

                vPram = { ID: ID };

                var vRetDataReceivingInfo = App.model.Receiving.GetReceivingOrTransfersInfo(vPram);
                var vRetDataReceivingDetails = App.model.Receiving.GetReceivingDetails(vPram);                
                
                ClearFields(); // Clear all fields when getting the draft trans

                ReceivingSKU = vRetDataReceivingDetails.Rows;
                DisplayReceivingInformation(vRetDataReceivingInfo.Rows[0]);

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
            //    $('#selIsResaleable').selectmenu('refresh');

            //}
            //else {

            if (CBool(IsDraft)) {
                $.mobile.changePage('#pageMain');

                $('#selReceiveOrTransfer').val('ReceiveItems');

                $('#dvReceiving').css('visibility', 'visible')
                $('#dvReceiving').css('display', 'block')

                $('#dvTransferOut').css("visibility", "hidden");
                $('#dvTransferOut').css("display", "none")
                //}

                $('#selReceiveOrTransfer').selectmenu('refresh');

                if (IsReceiving == 1) {
                    data = { ID: vRows.SourceOfSKU1ID, Name: vRows.SourceOfSKU1 };                    
                    $('#selIntraOrganization').val(vRows.SourceOfSKU1ID);
                    $('#selIntraOrganization').selectmenu('refresh');

                } else {
                    //data = { ID: vRows.ReceivingPlaceID, Name: vRows.ReceivingPlace };
                    data = { ID: vRows.SourceOfSKU1ID, Name: vRows.SourceOfSKU1 };
                    $('#selIntraOrganization').val(vRows.ReceivingPlaceID);
                    $('#selIntraOrganization').selectmenu('refresh');
                }

                RecFromTransToOrgID = $('#selOrganization').val();
                RecFromTransToOrgName = $('#selOrganization').find('option:selected').text();
                RecFromTransToFowstID = vRows.SourceOfSKU1ID
                RecFromTransToFowstName = vRows.SourceOfSKU1
                RecFromTransToDepartment = (vRows.ReceiveFromDepartment || '')
                isThirdPartyRecInTransFrom = false;

                $('#btnRecFromTransToPlace').text(RecFromTransToOrgName + ' (' + vRows.SourceOfSKU1 + ')').button('refresh');

                //data = { ID: SourceOfSKUID };
                $('#lblSourceOfSKU').val(SourceOfSKU)
                $('#lblSourceOfSKU').data(data);

                //Check if ReceiveInOrgID is equal to current OrgID
                //if equal then the value of RecInTransFromOrgName must be the current orgname and RecInTransFromOrgID must be the current orgid
                if (vRows.ReceiveInOrgID == sessionStorage.OrgID) {
                    RecInTransFromOrgID = $('#selOrganization').val();
                    RecInTransFromOrgName = $('#selOrganization').find('option:selected').text();
                    RecInTransFromFowstID = vRows.ReceivingPlaceID;
                    RecInTransFromFowstName = vRows.ReceivingPlaceFowst;
                    RecInTransFromDepartment = vRows.ReceiveInDepartment;
                    isThirdPartyRecInTransFrom = false;
                    $('#btnRecInTransFromPlace').text(RecInTransFromOrgName + ' (' + vRows.ReceivingPlaceFowst + ')').button('refresh');
                } else {
                    //if not, it is a 3rd party org name and org id
                    RecInTransFromOrgID = vRows.ReceivingPlaceID;
                    RecInTransFromOrgName = (vRows.ReceivingPlace || vRows.ReceivingPlaceFowst);
                    RecInTransFromFowstID = "";
                    RecInTransFromFowstName = "";
                    RecInTransFromDepartment = "";
                    isThirdPartyRecInTransFrom = true;
                    $('#btnRecInTransFromPlace').text('3rd Party (' + RecInTransFromOrgName + ')').button('refresh');
                    $('#selOrganization option[value="third"]').text('3rd Party (' + RecInTransFromOrgName + ')')
                    $('#selOrganization').val('third').selectmenu('refresh');
                }

                if (!vRows.SourceOfSKU1ID == "") {

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

                    $('#selIntraOrganization').selectmenu('refresh');
                    $('#selReceivingPlace').selectmenu('refresh');
                }
                else {
                    SourceOfSKUID = vRows.SourceOfSKU2ID;
                    SourceOfSKU = vRows.SourceOfSKU2;
                    $('#selSourceOfSKU').val('valThirdPartyVendor');
                    $('#dvVendor').css('visibility', 'visible');
                    $('#dvVendor').css('display', 'block');
                    $('#dvIntraOrganization').css('visibility', 'hidden');
                    $('#selReceivingPlace').val(vRows.ReceivingPlaceID).selectmenu('refresh');
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

                    $('#btnRecFromTransToPlace').text('3rd Party (' + RecFromTransToOrgName + ')').button('refresh');
                }

                $('#selSourceOfSKU').selectmenu('refresh');

                m_Date = new Date(vRows.DocumentDate);

                $('#optMonthValue').val(m_Date.getMonth() + 1)
                $('#optYearValue').val(m_Date.getFullYear())
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
                $('#optDayValue').val(m_Date.getDate())

                $('#optMonthValue').selectmenu('refresh');
                $('#optYearValue').selectmenu('refresh');
                $('#optDayValue').selectmenu('refresh');

                $('#txtDocumentType').val(vRows.DocumentType)
                $('#txtDocumentNo').val((CInt2(vRows.ReceivingNo) == 0 ? vRows.DocumentNo : vRows.ReceivingNo))
                $('#txtPONo').val(vRows.PONo)
                $('#txtTransportedBy').val(vRows.TransportedBy)

                $('#rcChoiceYes').checkboxradio('refresh')

                $('#rcChoiceNo').checkboxradio('refresh')

                if (vRows.Is3PL == 1) {
                    $('#rcChoiceYes').checked = true;

                    $('#dv3pl').css('visibility', 'visible');
                    data = { ID: vRows.ShipperID };
                    $('#txtShipper').val(vRows.Shipper)
                    $('#txtShipper').data(data)
                    $('#txtWayBill').val(vRows.WaybillNumber)
                }
                else {
                    $('#rcChoiceNo').checked = true;
                }

                m_Date = new Date(vRows.WaybillDate);

                $('#optMonthBLValue').val(m_Date.getMonth() + 1);
                $('#optYearBLValue').val(m_Date.getFullYear())
                setDayCollection('optMonthBLValue', 'optDayBLValue', 'optYearBLValue', false)
                $('#optDayBLValue').val(m_Date.getDate())

                $('#txtTransportedBy').val(vRows.TransportedBy);

                $('#optMonthBLValue').selectmenu('refresh');
                $('#optYearBLValue').selectmenu('refresh');
                $('#optDayBLValue').selectmenu('refresh');
            }
            else {
                $.mobile.changePage('#pageReadOnly');
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

                $('#lstROReceivingCart').empty();

                for (var i = 0; i < ReceivingSKU.length; i++) {
                    $('#lstROReceivingCart')
                    .append('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-first-child ui-last-child ui-btn-up-d">' +
                                '<div style="background-color:#F2F2F2 !important;" class="ui-btn-inner ui-li ui-li-static ui-btn-up-d">' +
                                    '<div class="ui-btn-text">' +
                                        '<a>' +
                                            (ReceivingSKU[i].ProductNo == "" ? '' : '(' + ReceivingSKU[i].ProductNo + ') ') + ReceivingSKU[i].ProductName +
                                            '<br/> <br/>' +
                                            '<div style="font-weight:normal !important;">' +
                                                ReceivingSKU[i].QuantityGood + ' ' + ReceivingSKU[i].Label +
                                            '</div>' +
                                        '</a>' +
                                    '</div>' +
                                    '<span class="ui-icon ui-icon-arrow-r ui-icon-shadow"></span>' +
                                '</div>' +
                            '</li>'
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
                $('#txtDocumentType').val('');
                $('#txtDocumentNo').val('');

            } else {
                $('#MainPageTitle').text('Transfers')
                $('#selReceiveOrTransfer').val('TransferItems');
                $('#txtDocumentType').val('Transfer-Out');
                $('#txtDocumentNo').val('');
            }

            try {
                $('#selReceiveOrTransfer').selectmenu('refresh');
            }
            catch (e) {
                $('#selReceiveOrTransfer').selectmenu();
            }

            UpdateDisplay();

            IsFromTransferOutOrDraftReceiving = false;

            $('#txtTransportedBy').val('');
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

            try {
                $("#rcChoiceNo").checkboxradio("refresh");
                $('#selIntraOrganization').selectmenu('refresh');
                $('#selSourceOfSKU').selectmenu('refresh');
                $('#selReceivingPlace').selectmenu('refresh');
                $('#selIsResaleable').selectmenu('refresh');
            }
            catch (e) {
                $("#rcChoiceNo").checkboxradio();
                $('#selIntraOrganization').selectmenu();
                $('#selSourceOfSKU').selectmenu();
                $('#selReceivingPlace').selectmenu();
                $('#selIsResaleable').selectmenu();
            }


            $('#lstItems li').each(function (m_index) {
                $(this).remove();
            });

            //$('#selReceiveOrTransfer').selectmenu('refresh');

            $('#btnRecInTransFromPlace, #btnRecFromTransToPlace').text('Please Select').button('refresh');
            $('#selOrganization')
                .empty()
                .append(new Option(sessionStorage.OrgName, sessionStorage.OrgID))
                .append(new Option('3rd Party', 'third'))
                .selectmenu('refresh')
            $('#selOrganization').val(sessionStorage.OrgID).selectmenu('refresh');
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
        //                $("#lstItems").listview('refresh');
        //            })
        //        )
        //        pLV.append(pItem);
        //    }

        //    try {
        //        pLV.listview('refresh');
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
        //                    $('#selIsResaleable').selectmenu('refresh');

        //                }
        //                else {
        //                    $('#selReceiveOrTransfer').val('ReceiveItems');

        //                    $('#dvReceiving').css('visibility', 'visible')
        //                    $('#dvReceiving').css('display', 'block')

        //                    $('#dvTransferOut').css("visibility", "hidden");
        //                    $('#dvTransferOut').css("display", "none")
        //                }

        //                $('#selReceiveOrTransfer').selectmenu('refresh');

        //                data = { ID: $(this).data('data').ReceivingPlaceID };
        //                $('#selReceivingPlace').val($(this).data('data').ReceivingPlaceID);

        //                $('#selReceivingPlace').selectmenu('refresh');

        //                if (!$(this).data('data').SourceOfSKU1ID == "") {
        //                    SourceOfSKUID = $(this).data('data').SourceOfSKU1ID;
        //                    SourceOfSKU = $(this).data('data').SourceOfSKU1;
        //                    $('#selSourceOfSKU').val('valIntraOrganization');
        //                    $('#selIntraOrganization').val(SourceOfSKUID);
        //                    $('#dvIntraOrganization').css('visibility', 'visible');
        //                    $('#dvVendor').css('visibility', 'hidden');
        //                    IsIntraOrganization = 1;

        //                    $('#selIntraOrganization').selectmenu('refresh');
        //                }
        //                else {
        //                    SourceOfSKUID = $(this).data('data').SourceOfSKU2ID;
        //                    SourceOfSKU = $(this).data('data').SourceOfSKU2;
        //                    $('#selSourceOfSKU').val('valThirdPartyVendor');
        //                    $('#dvVendor').css('visibility', 'visible');
        //                    $('#dvIntraOrganization').css('visibility', 'hidden');
        //                    IsIntraOrganization = 0;
        //                }

        //                $('#selSourceOfSKU').selectmenu('refresh');

        //                data = { ID: SourceOfSKUID };
        //                $('#lblSourceOfSKU').text(SourceOfSKU)
        //                $('#lblSourceOfSKU').data(data);

        //                m_Date = new Date($(this).data('data').DocumentDate);

        //                $('#optMonthValue').val(m_Date.getMonth() + 1)
        //                $('#optYearValue').val(m_Date.getFullYear())
        //                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', false)
        //                $('#optDayValue').val(m_Date.getDate())

        //                $('#optMonthValue').selectmenu('refresh');
        //                $('#optYearValue').selectmenu('refresh');
        //                $('#optDayValue').selectmenu('refresh');

        //                $('#txtDocumentType').val($(this).data('data').DocumentType)
        //                $('#txtDocumentNo').val($(this).data('data').DocumentNo)

        //                $('#rcChoiceYes').checkboxradio('refresh')

        //                $('#rcChoiceNo').checkboxradio('refresh')

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

        //                $('#optMonthBLValue').selectmenu('refresh');
        //                $('#optYearBLValue').selectmenu('refresh');
        //                $('#optDayBLValue').selectmenu('refresh');

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
        //        pLV.listview('refresh');
        //    }
        //    catch (e) {
        //        pLV.listview();
        //    }

        //}


        function GetShippers() {
            Browser.LibraryOrganization.Click = function (e) {
                $('#txtShipper').val(e.CompanyName)
                $('#txtShipper').data(e)
            };
            Browser.LibraryOrganization.Show($('#txtShipper').val());
        }

        function LoadRequiredData() {
            $('#selOrganization')
                .empty()
                .append(new Option(sessionStorage.OrgName, sessionStorage.OrgID))
                .append(new Option('3rd Party', 'third'))
                .selectmenu('refresh')

            if (RecInTransFromDefault == 'internal') {
                CustomerID = sessionStorage.OrgID;
                CustomerName = sessionStorage.OrgName;

                if (Business.LibraryOrganizationFOWST.GetData().Rows.length == 1) {                    
                    $('#btnReceivingPlace').text(sessionStorage.OrgName).button('refresh');
                } else {
                    $('#btnReceivingPlace').text('Please Select').button('refresh');
                }
            } else {
                CustomerID = sessionStorage.OrgID;
                CustomerName = sessionStorage.OrgName;
            }

            $('#selReceivingPlaceOption, #selDepartment').empty()
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
                    $('#selReceivingPlace').selectmenu('refresh');
                    $('#selReceivingPlaceOption').selectmenu('refresh');
                }
                catch (e) {
                    $('#selReceivingPlace').selectmenu();
                    $('#selReceivingPlaceOption').selectmenu();
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
                            $('#selReceivingPlace').selectmenu('refresh');
                        }
                        catch (e) {
                            $('#selReceivingPlace').selectmenu();
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
                        $('#selIntraOrganization').selectmenu('refresh');
                    }
                    catch (e) {
                        $('#selIntraOrganization').selectmenu();
                    }
                    App.Deferred.fulfill('4VL3L6F7FZ');
                }
            });

        }

        function GetLibraryOrganization() {
            IsCustomer = 1;

            Browser.LibraryOrganization.FilterCustomers = 1;
            Browser.LibraryOrganization.Click = function (e) {
                $('#lblSourceOfSKU').val(e.CompanyName);
                $('#lblSourceOfSKU').data(e);

                CustomerID = e.ID;
                
                setTimeout(function () {
                    GetThirdPartyOutlet(CustomerID)
                }, 500);
            };
            Browser.LibraryOrganization.Show();

            //Browser.LibraryOrganization.Click = function (e) {
            //    $('#lblSourceOfSKU').val(e.CompanyName);
            //    $('#lblSourceOfSKU').data(e);
            //};
            //Browser.LibraryOrganization.Show();
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
                $('#selSourceOfSKU').val('valThirdPartyVendor').selectmenu('refresh');
                isThirdPartyRecFromTransTo = true;
            } else {
                RecFromTransToDefault = 'internal';
                $('#selSourceOfSKU').val('valIntraOrganization').selectmenu('refresh');
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
        }

        function ChangeSourceOfSKU() {
            $('#selIntraOrganization').val('');
            $('#selIntraOrganization').selectmenu('refresh');
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
        window.SaveReceiving = function (IsFromMultipleSizes, vItems, IsDraft) {
            var Items = [];
            var ctr = 0;
            var ctr2 = 0;
            IsResaleable = 0;

            if (IsFromMultipleSizes) {
                ctr2 = 1;
                Items = vItems;
            }

            if ($('#selIsResaleable').val() == 'ResaleableYes') {
                IsResaleable = 1;
            }

            $('#lstItems li').each(function (m_index) {
                if ($(this).data('units') != undefined) {
                    while (ctr < $(this).data('units').length) {
                        Items[ctr2] = { ID: $(this).attr('ID'),
                            UnitMeasureID: $(this).data('units')[ctr].UnitMeasureID,
                            ActualQuantity: $(this).data('units')[ctr].ActualQuantity,
                            Quantity: $(this).data('units')[ctr].Quantity,
                            QuantityBad: $(this).data('units')[ctr].QuantityBad,
                            BadOrderReason: $(this).data('units')[ctr].BadOrderReason,
                            AdditionalInfo: $(this).data('units')[ctr].AdditionalInfo,
                            IsResaleable: $(this).data('units')[ctr].IsResaleable,
                            ProductSerials: $(this).data('units')[ctr].ProductSerials,
                        };
                        ctr++;
                        ctr2++;
                    };
                    ctr = 0;
                };

            });

            if (ctr2 == 0) {
                //AddToList();
                MessageBox("Please add item(s) to the cart before saving.",
                "Save",
                [
                    { Text: 'Ok'
                    }
                ]);
                return;
            }
            if (CBool(IsReceiving)) {
                if ($('#txtDocumentType').val() == '' || $('#txtDocumentNo').val() == '' || $('#txtTransportedBy').val() == '') {
                    $.mobile.changePage('#');
                    IsMainPage = false;
                    if (CBool(IsReceiving)) {
                        $('#btnInputItemDetails .ui-btn-text').text('Save Receiving');
                    } else {
                        $('#btnInputItemDetails .ui-btn-text').text('Save Transfer-Out');
                    }

                    return false;
                }
            }

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
                TotalWeightKg: $('#txtTotalWeightInkg').val(),
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
                ReceiveFromDepartment: RecFromTransToDepartment

            };

            App.model.Receiving.Save({
                data: data,
                success: function (vData) {
                    var msg;
                    var RefNum;

                    if (IsGuid(vData.Save.split('|')[1])) {
                        ID = vData.Save;

                        try {
                            $('#selReceiveOrTransfer').val('ReceiveItems').selectmenu('refresh');
                        } catch (e) {
                            $('#selReceiveOrTransfer').val('ReceiveItems').selectmenu();
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
                                    MessageBox('Receiving transaction has been successfully saved as \'Draft\'.', '', [{
                                        Text: 'Ok',
                                        Click: function () {
                                            IsSearchFromTransFerOut = false;
                                            ClearFields();
                                            $.mobile.changePage('#');
                                        }
                                    }]);
                                } else {
                                    MessageBox('Transfer Out transaction has been successfully saved as \'Draft\'.', '', [{
                                        Text: 'Ok',
                                        Click: function () {
                                            IsSearchFromTransFerOut = false;
                                            ClearFields();
                                            $.mobile.changePage('#');
                                        }
                                    }]);
                                }
                                
                            } else {
                                MessageBox('Your transaction has been successfully saved. ' + RefNum +
                                    '. This transaction record will be flushed to the Mobiz Cloud automatically when you have an Internet connection. '
                                , 'Save',
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function () {
                                            ClearFields();
                                            $.mobile.changePage('#');
                                        }
                                    }
                                ]); // END MessageBox
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


                            MessageBox(msg,
                            "Save",
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        GetReportInfo();
                                        $.mobile.changePage('#');
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function () {
                                        ClearFields();
                                        $.mobile.changePage('#');
                                    }
                                }
                            ]); // END MessageBox

                        }

                        if (OfflineMode)
                            App.model.Receiving_offline.FlushData();

                    }
                    else {
                        msg = 'An error occurred while saving this transaction.';
                        MessageBox(msg,
                        "Save",
                        [
                            { Text: 'Ok'
                            }
                        ]); // END MessageBox
                    }
                } // END success
            }); // END App.model.Receiving.Save({

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

                                        MessageBox('Were you able to download the PDF file?',
                                                   "Save",
                                                   [
                                                       {
                                                           Text: 'Yes',
                                                           Click: function () {
                                                               ClearFields();
                                                               ID = ''; //place here para makuha muna ang report info before maclear ang Receiving ID
                                                               $.mobile.changePage('#');
                                                           }
                                                       },
                                                       {
                                                           Text: 'No',
                                                           Click: function () {
                                                               GetReportInfo();
                                                               $.mobile.changePage('#');
                                                           }
                                                       }
                                                    ]); // END MessageBox


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
    });

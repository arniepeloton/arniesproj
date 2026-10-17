define([ // /Scripts3/controller/PrePos.js
    'common', 'business',
    (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''),
    // 'promise!/Scripts3/controller/PurchaseOrder.js?crc=', // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    '/Scripts3/Browser/LibraryFowst.js?crc='
], function () {

    var mDeferred = $.Deferred();
    mDeferred.resolve();
    
    $(function () {

        CreateStringObject('App.controller.PrePos', null, function () {

            var mOut = {};
            var mOldCartID = null;
            mOut.AutoLoadDefaultPosView = false;
            mOut.UpdateOrderedQuantity = function(){};
            mOut.UrlParams; // string
            mOut.callback = function(){};

            mOut.GetUser = function () {

                var pUser;

                if (OfflineMode) {
                    pUser = SalesMenu_offline().GetUser();
                    if ((pUser != undefined)
                        && (pUser.FowstID == undefined)) {
                        if (pUser.FowstCount == undefined) {
                            //Edited By: Francis
                            //Date: June 24, 2014
                            //                                MessageBox('The device you are currently using is not registered to any FOWST in the current organization.', null,

                            //                                        [
                            //                                            { Text: 'Update',
                            //                                                Click: function () {
                            //                                                    GetDeviceInformation();
                            //                                                }
                            //                                            },
                            //                                            { Text: 'Cancel',
                            //                                                Click: function () {
                            //                                                    var pParam = {};

                            //                                                    if (IsMobile)
                            //                                                        pParam.IsMobile = 1;
                            //                                                    pParam = $.param(pParam);
                            //                                                    window.location = "/People/PeopleMenu?" + pParam;
                            //                                                }
                            //                                            }
                            //                                        ]
                            //                                ); // MessageBox

                            MessageBox('The device you are currently using is not registered to any FOWST in the current organization.', null, null, {
                                OnClose: function () {
                                    var pParam = {};
                                    if (IsMobile)
                                        pParam.IsMobile = 1;
                                    pParam = $.param(pParam);
                                    window.location = "/People/PeopleMenu?" + pParam;
                                } // OnClose
                            }); // MessageBox

                            return false;
                        }; // if (pUser.FowstCount == undefined) {
                    }; // if (pUser.FowstID == undefined) {

                    LoadUser(pUser);
                    SalesMenu_offline().UpdateCounterFromOnline();
                } else { // if (OfflineMode) {
                    AjaxSend({
                        url: 'Login3/GetUser',
                        data: { FowstID: sessionStorage.FowstID },
                        success: function (vData) {
                            LoadUser(vData);
                        },
                        error: function (vError) {
                            console.log('Error getting Login/GetUser. Retrying...');
                            GetUser();
                        }
                    });
                }; // if (OfflineMode) {

                return true;
            };

            function LoadUser(vData) {

                var pUserMenu;
                var pLink;

                if (proj.POMode)
                    require(['promise!/Scripts3/controller/PurchaseOrder.js?crc='], function() {
                        // breakpoint here if you want
                    });

                if (CStr2(vData).length == 0) return;
                if (vData.Error) {
                    MessageBox(vData.Error);
                    MessageBox(vData.Error, '', null, {
                        OnClose: function () {
                            window.location = '/People/PeopleMenu';
                        }
                    });
                    return;
                };
                sessionStorage.AllowPriceAdjustment = CBool(vData.AllowPriceAdjustment);
                if (vData.FirstName != undefined) {
                    pUserMenu = $('#divUserMenu');
                    pUserMenu.html('');

                    pLink = $('<a>')
                        .attr('href', '#pageUserMenu')
                        .css('text-decoration', 'none')
                        .html('Hi, ' + HtmlEncode(vData.FirstName) + '&#x25BC;');
                    pUserMenu.append(pLink);

                    //////////////////                        if ((CDec2(vData.IsSalesAgent) != 0) && (proj.PosMode == false)) {

                    //////////////////                            $('#divCustomer').show();
                    //////////////////                        };
                    //////////////////                         if (proj.PosMode) {
                    if (window.pageCustomerDetails
                        && $.isFunction(window.pageCustomerDetails.LoadCustomer)
                        && window.pageCustomerDetails.LoadCustomer()) {
                        //ADDITIONAL BY JANINE FEB 18
                        //                            LoadRecentlyBought({ DateRangeType: mRecentlyBoughtDateRangeType });

                        NewCart(); // march 26, 2013
                        isCartEmpty = true;


                        //                            pageCustomerDetails.setCustomerInfo();
                    }
                    $('#divPosBranch').show();
                    if (CStr2(vData.FowstID).length > 0) {
                        onSelectFowst(vData);
                        //sessionStorage.EnableHotButtonsContextualMenu = CBool(vData.EnableHotButtonsContextualMenu);
                        //sessionStorage.FowstID = vData.FowstID;
                        //sessionStorage.FowstTin = vData.TIN;
                        //sessionStorage.FowstAddress = vData.Address;
                        //SalesMenu_offline().HandleEnforceZRead({
                        //    callback: function () {
                        //        LoadSupplier();
                        //        HandleMallLessor();
                        //        // #lnkPosBranchHotButtons = button in product maker
                        //        $('#lnkPosBranch, #lnkPosBranchHotButtons').html(HtmlEncode('Branch: ' + vData.FowstName) + '&#x25BC;');
                        //        $('.lblPosBranch').text(vData.FowstName);
                        //        //if (proj.POMode)
                        //        //    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + vData.FowstName);
                        //        PredeterminePrinter();

                        //        VerifyDevice();

                        //        //added by janine
                        //        sessionStorage.FowstName = vData.FowstName;
                        //        WriteLastFowst();
                        //        //addeed by francis
                        //        if (IsMobile) {  // condition added by Arvin
                        //            try {
                        //                Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                        //            } catch(e) {
                        //                console.error('jsVQWQHN6UG3! ' + e);
                        //            }
                        //        }
                        //        //$.mobile.changePage('#dialogCheckerLogIn');
                        //    }
                        //});

                        ///////end of added by janine
                    } else { // if (CStr2(vData.FowstID).length > 0) {
                        if (OfflineMode) {
                            BrowseAddresses();
                        } else {
                            var pData = { FilterFowst: 1 };
                            AjaxSend({
                                url: 'Browser/LibraryFowst',
                                data: pData,
                                success: function (vFowstData) {

                                    if (vFowstData.Results.Rows.length == 1) {
                                        sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                                        LoadSupplier();
                                        HandleMallLessor();
                                        $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + vFowstData.Results.Rows[0].FowstName) + '&#x25BC;');
                                        $('.lblPosBranch').text(vFowstData.Results.Rows[0].FowstName);
                                        PredeterminePrinter();
                                        VerifyDevice();
                                        //added by janine
                                        sessionStorage.FowstName = vFowstData.Results.Rows[0].FowstName;
                                        WriteLastFowst();
                                        ///////end of added by janine

                                        //////                                            VerifyPrinterAndReceiptSettings();

                                    } else {
                                        BrowseAddresses();
                                    }
                                }
                            });
                        }; // if (OfflineMode) {
                    }; // if (CStr2(vData.FowstID).length > 0) {

                    ////////////////////                         };
                    //                        LoadCustomer();
                    

                };

                if (CDec2(vData.IsSalesAgent) != 1)
                    $('#divCompany').hide();

            }; // LoadUser

            function LoadSupplier() {

                if (proj.POMode) {
                    $('#divBlockCustomer').hide();
                    $('#divBlockSupplier').show();

                    if (OfflineMode) {
                        var vSupplier;
                        var vData;
                        vData = SalesMenu_offline().getSupplier(sessionStorage.FowstID);

                        if (GetStringObject('Rows.length', vData) > 0) {

                            vSupplier = vData.Rows[0];

                            if (CBool(vSupplier.IsPOExclusiveToPrincipal)) {

                                $('#lblSupplierName').html(HtmlEncode(vSupplier.CompanyName) + '&#x25BC;')
                        .data('id', vSupplier.ID);


                                sessionStorage.SupplierOrganizationID = CStr2(vSupplier.ID);
                                sessionStorage.SupplierOrganizationName = CStr2(vSupplier.CompanyName);
                                sessionStorage.SupplierPriceLevelID = CStr2(vSupplier.POPriceLevel);
                                sessionStorage.SupplierPriceLevelName = CStr2(vSupplier.PriceLevel);
                            } else
                                BrowseSupplier();

                        } else
                            BrowseSupplier();
                    } else {
                        MessageBox('This feature is only available in the mobile app',
                    'Purchase Order',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                location = '/People/PeopleMenu';
                            }
                        }

                    ]
            );
                    }
                } else {
                    sessionStorage.removeItem('SupplierOrganizationID');
                    sessionStorage.removeItem('SupplierOrganizationName');
                    sessionStorage.removeItem('SupplierPriceLevelID');
                    sessionStorage.removeItem('SupplierPriceLevelName');
                    $('#divBlockCustomer').show();
                    $('#divBlockSupplier').hide();
                }


            }

            mOut.BrowseSupplier = BrowseSupplier;
            function BrowseSupplier() {

                require(['promise!/Scripts3/controller/PurchaseOrder.js?crc='], function() {
                    if (mOut.PurchaseOrder == null)
                        mOut.PurchaseOrder = new App.controller.PurchaseOrder();
                    mOut.PurchaseOrder.ok = function(vData) {
                        $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons')
                            .text('Supplier: ' + vData.CompanyName + ' - ' + vData.FowstName)
                            .data('id', vData.OrgID);

                        sessionStorage.SupplierOrganizationID = CStr2(vData.OrgID);
                        sessionStorage.SupplierOrganizationName = CStr2(vData.CompanyName);
                        sessionStorage.SupplierAddress = CStr2(vData.Address);
                        if (vData.FowstID == undefined)
                            sessionStorage.removeItem('SupplierFowstID');
                        else {
                            sessionStorage.SupplierFowstID = vData.FowstID;
                            Android.UpdateSessionData({
                                FowstID: vData.FowstID
                            }); // disconnected checkout
                        }
                        if (vData.FowstName == undefined)
                            sessionStorage.removeItem('SupplierFowstName');
                        else
                            sessionStorage.SupplierFowstName = vData.FowstName;
                        if (IsGuid(vData.BinID)) {
                            sessionStorage.SupplierBinID = vData.BinID;
                            sessionStorage.SupplierBinName = vData.BinName;
                        } else {
                            sessionStorage.removeItem('SupplierBinID');
                            sessionStorage.removeItem('SupplierBinName');
                        }
                        mOut.PurchaseOrder.SaveSupplier(sessionStorage.CartOrderID, null, true);

                        var pData = SalesMenu_offline().getSupplierPriceLevel(vData.OrgID, sessionStorage.FowstID);

                        if (GetStringObject('Rows.length', pData) > 0) {
                            var pSupplier = pData.Rows[0];
                            sessionStorage.SupplierPriceLevelID = CStr2(pSupplier.POPriceLevel);
                            sessionStorage.SupplierPriceLevelName = CStr2(pSupplier.PriceLevel);
                        } else {
                            sessionStorage.SupplierPriceLevelID = Business.SystemDefaults.GetLandedCostID(); //'EEF716C6-B290-4B28-B2A6-0A4F52CF4989';
                            sessionStorage.SupplierPriceLevelName = 'Landed Cost'; // 'SRP (Suggested Retail Price)';
                        }
                    }; // END mOut.PurchaseOrder.ok = function(vData) {

                    (function() {
                        var pPoInput = {
                            OrgID: sessionStorage.SupplierOrganizationID,
                            FowstID: sessionStorage.SupplierFowstID,
                            BinName: sessionStorage.SupplierBinName,
                            Title: 'Supplier'
                        };
                        var pSetting = Business.SystemDefaults.PurchaseOrderSupplierSetting();
                        var pDefaultOrgID;
                        var pDefaultFowstID;

                        if (pSetting == 'locked to') {
                            pDefaultOrgID = Business.SystemDefaults.PurchaseOrderSupplierID();
                            if (IsGuid(pDefaultOrgID)) {
                                pPoInput.LockedOrg = true;
                                pPoInput.OrgID = pDefaultOrgID;
                            }
                            pDefaultFowstID = Business.SystemDefaults.PurchaseOrderSupplierFowstID();
                            if (IsGuid(pDefaultFowstID)) {
                                pPoInput.LockedFowst = true;
                                pPoInput.FowstID = pDefaultFowstID;
                            }
                        }
                        if (pSetting == 'default to') {
                            if (!IsGuid(pPoInput.OrgID)) {
                                pDefaultOrgID = Business.SystemDefaults.PurchaseOrderSupplierID();
                                if (IsGuid(pDefaultOrgID)) {
                                    pPoInput.OrgID = pDefaultOrgID;
                                    pDefaultFowstID = Business.SystemDefaults.PurchaseOrderSupplierFowstID();
                                    if (IsGuid(pDefaultFowstID)) {
                                        pPoInput.FowstID = pDefaultFowstID;
                                        pPoInput.LockedFowst = pPoInput.LockedOrg;
                                    }
                                }
                            }
                        }
                        mOut.PurchaseOrder.popup(pPoInput);
                    })();
                }); // END require(['promise!/Scripts3/controller/PurchaseOrder.js?crc='], function() {
                
            }

            mOut.BrowseAddresses = BrowseAddresses;
            function BrowseAddresses() {
                if (proj.IsBootstrapDesign()) {
                    BrowsePopup('LibraryFowst', 'FOWST', [
                    {
                        Offline: true,
                        AutoSearch: true,
                        WithSearchbox: false,
                        Message: proj.POMode ? 'Please choose the FOWST (business location) that you are doing a Purchase Order for' : '',
                        DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                        Click: function (e) {
                            onSelectFowst($.extend({
                                FowstID: e.ID
                            }, e));
                        }
                    }
                    ], {
                        OrganizationID: sessionStorage.OrgID,
                        FilterFowst: 1
                    })
                } else {
                    Browser.OfflineMode = OfflineMode;
                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.AfterClick = function (e) {
                        onSelectFowst($.extend({
                            FowstID: e.ID
                        }, e));
                        //sessionStorage.FowstTin = e.TIN;
                        //sessionStorage.FowstAddress = e.Address;
                        //sessionStorage.FowstID = e.ID;
                        //sessionStorage.EnableHotButtonsContextualMenu = CBool(e.EnableHotButtonsContextualMenu);
                        ////if (proj.POMode)
                        ////    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + e.FowstName);
                        //// LoadSupplier(); // commented by Arvin 2016-08-11
                        //HandleMallLessor();
                        //$('#lnkPosBranch').html(HtmlEncode('Fowst: ' + e.FowstName) + '&#x25BC;');
                        //$('.lblPosBranch').text(e.FowstName);
                        //PredeterminePrinter();
                        //VerifyDevice();
                        ////added by janine
                        //sessionStorage.FowstName = e.FowstName;
                        //WriteLastFowst();
                        /////////end of added by janine
                        //if (OfflineMode) {
                        //    SalesMenu_offline().HandleEnforceZRead();
                        //    Android.UpdateSessionData({
                        //        FowstID: sessionStorage.FowstID
                        //    }); // disconnected checkout
                        //    SalesMenu_offline().UpdateCounterFromOnline();
                        //}

                        //if (window.location.pathname.toLowerCase() == '/billofmaterials')
                        //    $('#lblCurrentFOWST').html(HtmlEncode(e.FowstName) + '&#x25BC;');
                        ////////                        VerifyPrinterAndReceiptSettings();

                        ////$.mobile.changePage('#dialogCheckerLogIn');
                    };
                    Browser.LibraryFowst.Show(null, {
                        description: proj.POMode ? 'Please choose the FOWST (business location) that you are doing a Purchase Order for' : ''
                    });
                }
            };

            function onSelectFowst(vData) {
                sessionStorage.EnableHotButtonsContextualMenu = CBool(vData.EnableHotButtonsContextualMenu);
                sessionStorage.FowstID = vData.FowstID;
                sessionStorage.FowstTin = vData.TIN;
                sessionStorage.FowstAddress = vData.Address;
                if (OfflineMode) {
                    var pValue = Business.SystemDefaults.GetSystemDefault('CompanyOwnedOrFranchisee');
                    Transfers.WriteFile('CompanyOwnedOrFranchisee.txt', null, pValue);
                    Transfers.WriteFile('DataImportationPort.txt', null, CStr2(vData.DataImportationPort));
                    SalesMenu_offline().HandleEnforceZRead({
                        IsFOWSTInsideAMall: vData.IsFOWSTInsideAMall,
                        FOWSTMallName: vData.FOWSTMallName,
                        Operates247: vData.Operates247,
                        callback: function (vResult) {
                            if (!vResult)
                                return;
                            LoadSupplier();
                            HandleMallLessor();
                            // #lnkPosBranchHotButtons = button in product maker
                            $('#lnkPosBranch, #lnkPosBranchHotButtons').html(HtmlEncode('Branch: ' + vData.FowstName) + '&#x25BC;');
                            $('.lblPosBranch').text(vData.FowstName);
                            //if (proj.POMode)
                            //    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + vData.FowstName);
                            PredeterminePrinter();

                            VerifyDevice();

                            //added by janine
                            sessionStorage.FowstName = vData.FowstName;
                            WriteLastFowst();
                            //addeed by francis
                            if (IsMobile) {  // condition added by Arvin
                                try {
                                    Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                                } catch(e) {
                                    console.error('jsVQWQHN6UG3! ' + e);
                                }
                            }
                            mOut.callback();
                            if (mOut.AutoLoadDefaultPosView)
                                LoadDefaultPosView(vData.DefaultPosView);
                            else if (mOldCartID) {
                                sessionStorage.setItem('CartOrderID', mOldCartID);
                                mOut.UpdateOrderedQuantity();
                            }
                        }
                    });
                } else
                    HandleMallLessor();
            }

            function LoadDefaultPosView(vDefaultPosView) {

                var pLocation;
                var pBaseLocation = '/SalesMenu';
                if (window.InAndroid) {
                    if ($(window).width() > proj.WideWidth) //
                        pBaseLocation += '/POSViewAll';
                    pLocation = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0,
                        // TableView: 'true', 
                        TableNo: 'js998QYHQHXH!'
                    }, pBaseLocation);
                }

                localStorage.PosView = CStr2(vDefaultPosView).trim();
                vDefaultPosView = CStr2(vDefaultPosView).trim().toLowerCase();

                var pOptions = {
                    FieldName: 'POSLastORNo',
                    InvoiceFieldName: 'InvoiceNumber',
                    DontUseFileCounter: false,
                    DontUseTableCounter: false
                };
                var pCounter = 1//Business.TransInvoice.GetCounterFileContent(sessionStorage.FowstID, Web.DeviceID(), pOptions);
                if (!IsNumeric(pCounter)) {
                    SalesMenu_offline().UpdateCounterFromOnline({
                        OnExit: function() {
                            NewMessageBox('The current version of this app requires a connection to Mobiz server during this 1-time initialization.', '', null, {
                                Close: function() {
                                    location.reload();
                                }
                            });
                        }
                    });
                    return;
                }
                switch (vDefaultPosView) {
                    case 'Table View'.toLowerCase():
                        // USE NEW UI BY DEFAULT STARTING JAN. 1, 2021 by Arnie
                        window.location = '/TableView/BsNewTable?theme=bs&' + mOut.UrlParams;
                        //pLocation = MergeUrlParameters({
                        //    OrgID: sessionStorage["OrgID"],
                        //    mode: 'pos',
                        //    IsMobile: IsMobile ? 1 : 0
                        //}, '/SalesMenu/POSViewAll');
                        break;

                    case 'Hot Buttons View'.toLowerCase():
                        // USE NEW UI BY DEFAULT STARTING JAN. 1, 2021 by Arnie
                        if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                            location = MergeUrlParameters({
                                OrgID: sessionStorage["OrgID"],
                                mode: 'pos',
                                IsMobile: IsMobile ? 1 : 0,
                                theme: 'bs',
                                rsd: (new URLSearchParams(mOut.UrlParams).get('rsd') || 0)
                            }, '/SalesMenu/BsPOSViewAll');
                        } else {
                            window.location = '/SalesMenu/BsIndex?HotButtons=1&theme=bs&' + mOut.UrlParams;
                            // goToHotButtonPage();
                        }

                        //NewMessageBox('We have a new User Interface (UI) design for Hot Button mode, would you like to use it?', 'Message', [
                        //    {
                        //        Text: 'Yes, I want to use the new UI',
                        //        Click: function () {
                        //            if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                        //                location = MergeUrlParameters({
                        //                    OrgID: sessionStorage["OrgID"],
                        //                    mode: 'pos',
                        //                    IsMobile: IsMobile ? 1 : 0,
                        //                    theme: 'bs'
                        //                }, '/SalesMenu/BsPOSViewAll');
                        //            } else {
                        //                window.location = '/SalesMenu/BsIndex?HotButtons=1&theme=bs&' + mOut.UrlParams;
                        //                // goToHotButtonPage();
                        //            }
                        //        }
                        //    },
                        //    {
                        //        Text: 'No, I want to use the old UI',
                        //        Click: function () {
                        //            if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                        //                location = MergeUrlParameters({
                        //                    OrgID: sessionStorage["OrgID"],
                        //                    mode: 'pos',
                        //                    IsMobile: IsMobile ? 1 : 0
                        //                }, '/SalesMenu/POSViewAll');
                        //            } else {
                        //                window.location = '/SalesMenu?HotButtons=1&' + mOut.UrlParams;
                        //                // goToHotButtonPage();
                        //            }
                        //        }
                        //    }
                        //]);                        
                        break;

                    case 'Hot Buttons with side cart'.toLowerCase():
                        // USE NEW UI BY DEFAULT STARTING JAN. 1, 2021 by Arnie
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0,
                            theme: 'bs',
                            rsd: (new URLSearchParams(mOut.UrlParams).get('rsd') || 0)
                        }, '/SalesMenu/BsPOSViewAll');
                        break;

                    case 'FMCG View (DOS Look)'.toLowerCase():
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0
                        }, '/SalesMenu/FMCGBasic');
                        break;

                    case 'FMCG View (Modern Look)'.toLowerCase():
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0
                        }, '/SalesMenu/FMCG');
                        break;
                    case 'Vending Machine'.toLowerCase():
                        //if (!Android.DebugMode())
                        //    Android.PinApp();
                        setTimeout(function () {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage["OrgID"],
                                mode: 'pos',
                                IsMobile: IsMobile ? 1 : 0
                            }, '/VendingMachine');
                        }, 2000)                        
                        break;

                    default:
                        // USE NEW UI BY DEFAULT STARTING JAN. 1, 2021 by Arnie

                        window.location = '/SalesMenu/BsIndex?theme=bs&' + mOut.UrlParams;

                        //NewMessageBox('We have a new User Interface (UI) design for Retail View mode, would you like to use it?', 'Message', [
                        //    {
                        //        Text: 'Yes, I want to use the new UI',
                        //        Click: function () {
                        //            window.location = '/SalesMenu/BsIndex?theme=bs&' + mOut.UrlParams;
                        //        }
                        //    },
                        //    {
                        //        Text: 'No, I want to use the old UI',
                        //        Click: function () {
                        //            window.location = '/SalesMenu?' + mOut.UrlParams;
                        //        }
                        //    }
                        //]);
                }

                if (window.InAndroid) {
                    //Android.SetSessionStorage({
                    //    WebViewIndex: 2
                    //});
                    //if (pLocation != null)
                    //    Android.LoadToWebView2({
                    //        Url: pLocation
                    //    });
                }
                
            } // END LoadDefaultPosView

            function HandleMallLessor() {

                if (localStorage.LoginHash != undefined) {
                    AjaxSend({
                        url: 'MallLessor/SetMall',
                        data: {
                            LoginHash: localStorage.LoginHash
                            // ,Mall: $('#cbMall > option:selected').val()
                        },
                        success: function (vData) {
                            if (vData.Error) {
                                MessageBox(vData.Error);
                                return;
                            } else if (vData.Result) {
                                localStorage.removeItem('LoginHash');
                                MessageBox('Mall Report Setup was successful.', 'Success');
                            };
                        }
                    }); // AjaxSend({
                }; // if (localStorage.LoginHash != undefined) {
            }; // HandleMallLessor

            function PredeterminePrinter() {
                if (!IsMobile) return;
                if (OfflineMode) return; // todo
                AjaxSend({
                    url: '/Organization/getOrgFOWSTPrinterIP',
                    success: function (vData) {
                        try {
                            if (vData.OrganizationFOWSTData.Rows.length == 0) {
                                MessageBox('This device has no assigned printer in the selected FOWST');
                            };
                        } catch (e) {
                            MessageBox('Unable to determine "vData.OrganizationFOWSTData.Rows.length"');
                            console.error(e);
                        };
                    }
                });

            }; // PredeterminePrinter

            function VerifyDevice() {

                if (OfflineMode) return; // todo
                AjaxSend({
                    url: '/Login3/VerifyDevice',
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            MessageBox('This device is not assigned to this FOWST',
                                'FOWST',
                                [
                                    { Text: 'Ok',
                                        Click: function () {
                                            //redirect to home. kick out of POS
                                            window.location = '/People/PeopleMenu';
                                        }
                                    }
                                ]
                    );

                        } else {
                            NewCart(); //additional by janine
                            isCartEmpty = true;
                        }

                    }
                }); // AjaxSend

            }; // VerifyDevice

            function WriteLastFowst() {
                if (IsMobile) {  // condition added by Arvin
                    try {
                        Transfers.WriteFile('LastFowst.txt', null, sessionStorage.FowstName);
                        Transfers.WriteFile('LastFowstID.txt', null, sessionStorage.FowstID);
                    } catch(e) {
                        console.error('jsVQWQHN6UG3! ' + e);
                    }
                }

                try {
                    var pMobizAppVersion = Business.LibraryDevice.GetMobizAppVersion();
                    Transfers.WriteFile('MobizAppVersion.txt', null, pMobizAppVersion);
                    Android.SetMobizAppVersion(pMobizAppVersion);
                } catch(e) {
                    console.error('js7ZF72WAGU9! ' + e);
                }
            } // END function WriteLastFowst() {

            mOut.NewCart = NewCart;
            function NewCart() {
                localStorage.removeItem('CartOrderID'); // use for new retail view since onpage ang pagload sa payment page, ginadelete ang SESSIONSTORAGE sa payments page pag mag clear sa proj.SalesPayments.ClearSalesPaymentsInfoFields by arnie
                localStorage.NewCartMode = 1;
                mOldCartID = sessionStorage.getItem('CartOrderID');
                sessionStorage.removeItem('CartOrderID');
                                
                //additional by janine
                sessionStorage.removeItem('InvoiceProductIDList');
                proj.ProductID = '';
                sessionStorage.LastQty = 1;
                sessionStorage.ProductName = '';
                sessionStorage.ProductUnitQty = 0;
                sessionStorage.ProductUnitMeasure = '';
                sessionStorage.ProductPrice = 0;
                sessionStorage.removeItem('SupplierOrganizationID');
                sessionStorage.removeItem('SupplierOrganizationName');
                sessionStorage.removeItem('SupplierFowstID');
                sessionStorage.removeItem('SupplierFowstName');
                sessionStorage.removeItem('SupplierBinID');
                sessionStorage.removeItem('SupplierBinName');
                if (proj.POMode) 
                    sessionStorage.removeItem('CustomerOrganizationID'); // clear only if PO
                sessionStorage.removeItem('CustomerOrgName');
                sessionStorage.removeItem('CustomerAddress');
                sessionStorage.removeItem('CustomerFowstID');
                // sessionStorage.removeItem('FowstID'); // dont clear FowstID
                sessionStorage.removeItem('CustomerFowstName');
                sessionStorage.removeItem('CustomerBinID');
                sessionStorage.removeItem('CustomerBinName');

                //                    sessionStorage.PosMode = 'Retail Mode'; //temporarily set retail mode as default
                // sessionStorage.IsHotButtonVisited = 'false';
                if (CStr2(sessionStorage.IsEnableDiningMode) == 'true')
                    sessionStorage.DeliveryMethod = 'Dine In';
                else
                    sessionStorage.DeliveryMethod = 'PickUp';

                sessionStorage.OrderMethod = 'Walk In';
                sessionStorage.Gender = 'NA';
                sessionStorage.Age = 'Mixed Ages/ NA';
                //                    ClearCustomerFreeTypeSession();

                $('#txtOrder').html('<label style="font-size:16px;">Your cart is empty</label>');
                $('#txtQtyUnit').html('');

                $('#divCartPanel').show();

                $('#divCartSection').hide();
                $('#divCartEmpty').show();
                window.popSalesMenuOrderTaker.Clear();

                $('#btnHotButtonMain').click(); // for hot buttons, display first button
                if (proj.NewCartHotButtons)
                    proj.NewCartHotButtons();
                if (proj.IsBootstrapDesign()) { //added by arnie nov 3, 2020
                    $('#txtOrder').html('<div style="text-align:center; color: #a9afb5; font-size:20px;"><span class="bi-cart4" style="font-size:90px;"></span><br />Cart is empty</div>');
                    $('#divCartRetail').empty().append('<li><div style="text-align:center; color: #a9afb5; font-size:25px;"><span class="bi-cart4" style="font-size:100px;"></span><br />Cart is empty</div></li>');
                    $('#listEmptyCart').show();
                    $('#pushTemplate').hide();
                } else {
                    $('#pushTemplate').show();
                }

                //Inserted by Francis A. to load the Invoice Details from Sales Payment due to Overpayment of Check 
                //09/18/2013
                if (sessionStorage.CartOnHoldCheckPayment != undefined) {
                    if (sessionStorage.CartOnHoldCheckPayment == 1 && sessionStorage.CartOnHoldCheckPaymentOrderID) {
                        sessionStorage.setItem('CartOrderID', sessionStorage.CartOnHoldCheckPaymentOrderID);
                        proj.ShowCart();
                    }
                }
                //End Inserted by Francis A. ------------------------
                $('#spanCartTotal').text('0.00'); //additional by janine nov 11
                //$('#pushTemplate').parent().nextAll().remove();
                $('#pushSection').empty();
                $('#tblSideCartItems tbody').remove();

                if (proj.POMode) {
                    $('#lblCustomerName, #lblSupplierName').text('Supplier');
                    $('#lblCurrentPOSMode').text('Customer');
                }
                proj.CartStock = [];
                proj.CartData = null;
                startclock('#clock');
               
                if (localStorage.getItem('PosView') == 'Hot Buttons View'
                    && sessionStorage.PosView == 'Hot Buttons Mode')
                    proj.goToHotButtonPage();

                // commented for debugging 2020-02-15
                if (localStorage.PosView != 'Table View') {
                    var pLocation = MergeUrlParameters({
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, "/SalesPayments");
                    if (window.InAndroid) {
                        //Android.SetSessionStorage({
                        //    WebViewIndex: 2
                        //});
                        //Android.LoadToWebView2({
                        //    Url: pLocation
                        //});
                    }
                }
                
            };

            mOut.CanTransactWithUnflushedData = function () {
                AjaxSend({
                    url: 'POSSettings/CanTransactWithUnflushedData',
                    success: function (e) {
                        Transfers.WriteFile('CanTransactWithUnflushedData.txt', null, CBool(e.CanTransactWithUnflushedData))
                    }, error: function () {
                        //silent error
                    }
                })
            }

            mOut.CheckIfNoUnflushedDataFromPrevDays = function (param) {
                var CanTransactWithUnflushedData = Transfers.ReadFile('CanTransactWithUnflushedData.txt') || true;

                if (!CBool(CanTransactWithUnflushedData)) {
                    var cntUnflushedInvoice = Sql.Execute('SELECT ID FROM TransInvoice WHERE InvoiceNumber > 0 AND DateImported IS NULL AND Date(InvoiceDate) < Date(' + proj.GetZReadDateSql() + ') LIMIT 1').Rows.length;

                    if (cntUnflushedInvoice > 0) {
                        NewMessageBox('<div class="text-center mb-3"><span class="bi-exclamation-circle-fill text-warning" style="font-size: 70px;"></span></div>You cannot process new transactions because you have unflushed data from previous days. Please flush all pending data to continue.', 'Transaction Restricted', [
                            {
                                Text: 'Got it', Click: function () {
                                    SalesPaymentsExport_offline().FlushDeviceData({
                                        Success: function (vData) {

                                        },
                                        Fail: function () {                                                  
                                        }
                                    });
                                }
                            }
                        ]);

                        return false;
                    }
                }

                param.success();
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
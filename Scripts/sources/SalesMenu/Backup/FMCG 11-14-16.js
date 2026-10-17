require([
    'common',
    'business',
    '/Scripts/sources/SalesMenu/FMCGPopup.js',
    '/Scripts/sources/SalesMenu/FMCGPayment.js',
    '/Scripts/sources/SalesMenu/FMCGCart.js',
    '/Scripts/sources/SalesMenu/FMCGPayment.js',
    '/Scripts/sources/SalesMenu/FMCGHeader.js',
    '/Scripts3/SalesMenu_offline.js',
    '/Scripts3/CustomerDetails.js'
    //'/Scripts3/Stock_offline.js',
    //'/Scripts3/Offline/Browser/LibraryPIMProduct.js'    
], function () {
    require([
        (OfflineMode ? '/Scripts3/Offline/Business/LibraryPeople.js' : '')
    ], function () {
        require([
            (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''),
            '/Scripts/jquery.scrollTo.js'
        ], function () {
            $(document).ready(function () {
                Browser.OfflineMode = true;
                LoadRequiredSection();

                window.BlinkMessage = function (msg) {
                    $('#txtMessage').html(msg).blink();
                    $('#divPayment').scrollTop($('#divPayment')[0].scrollHeight)
                }

                window.NewCart = function () {
                    NewCart();
                }

                window.BrowseFowst = function () {
                    BrowseFowst();
                }

                window.ScrollToMiddle = function (scrollableDiv, el) {
                    $(scrollableDiv).scrollTo('#' + el.attr('id'), { offset: -$(scrollableDiv).height() / 2 });
                }

                NewCart();

                FmcgBasicCart.init($('#divCart'))

                var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                if (vDataPrinterType != undefined) {
                    if (vDataPrinterType.Rows.length > 0) {
                        if (vDataPrinterType.Rows[0].ConnectionType == "USB") {
                            setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, true)
                        }
                    }
                }

                function NewCart() {
                    sessionStorage.removeItem('CustomerOrganizationID');
                    window.pageCustomerDetails.LoadCustomer() // CustomerDetails.js

                    $('#txtCurrentCustomerName').text(sessionStorage.CustomerOrganizationName);
                    FmcgBasicCart.CartTotal = 0;
                    FmcgBasicCart.CartTotalQty = 0;
                    sessionStorage.removeItem('CartOrderID')
                    sessionStorage.CustomerTableNo = '';
                    sessionStorage.GasPumpNo = '';
                    sessionStorage.SupplierOrganizationID = '';
                    sessionStorage.SupplierOrganizationName = '';
                    sessionStorage.SupplierAddress = '';
                    sessionStorage.SupplierPriceLevelID = '';
                    sessionStorage.SupplierPriceLevelName = '';

                    if (CStr2(sessionStorage.IsEnableDiningMode) == 'true')
                        sessionStorage.DeliveryMethod = 'Dine In';
                    else
                        sessionStorage.DeliveryMethod = 'PickUp';

                    sessionStorage.OrderMethod = 'Walk In';
                    sessionStorage.Gender = 'NA';
                    sessionStorage.Age = 'Mixed Ages/ NA';
                    sessionStorage.DeliveryAddressID = '';
                    sessionStorage.BillingAddressID = '';
                    sessionStorage.DeliveryDate = '';
                    sessionStorage.IsPaymentJSLoaded = 'false';
                    localStorage.NewCartMode = 1;
                    sessionStorage.IsNewCart = false;
                    var pParams = GetUrlParameters();

                    proj.PosMode = (pParams.mode == 'pos');
                    proj.ReconstructSalesDataMode = (pParams.rsd == '1');
                    proj.POMode = (pParams.mode == 'po');
                    proj.SOMode = (pParams.mode == 'so');
                    proj.CartStock = [];

                    CartTotal = 0;
                    CartTotalQty = 0;
                    $('.lblCurrentProdQty, .lblCurrentProdQty2').text('1');
                    $('#ulSelectedProduct').empty();
                    $('#txtSubTotal, #lblVATPercent, #lblTotalAmount, #txtLessDiscount, #txtTotal, #txtCash, .lblCurrentProdTotal, #txtBalChange').text('0.00')
                    $('[name="txtQtyPlu"]').focus();
                    $('#divPLU').pageshow();
                    $('#rowVATPercent, #rowAmountLessVAT').hide();

                    FmcgBasicCart.Clear();
                    Popup.Cash.Clear();
                    Popup.ChequeGC.Clear();
                    Popup.Srv.Clear();
                    Popup.Card.Clear();
                    Popup.OtherPayment.Clear();
                    Popup.Discount.Clear();
                }

                function LoadRequiredSection() {
                    LoadText();

                    AjaxSend({
                        ExactUrl: '/SalesMenu/FMCGHeader?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divFMCGHeader').empty();
                            $('#divFMCGHeader').append(vData).trigger('create');
                        }
                    });

                    AjaxSend({
                        ExactUrl: '/SalesMenu/FMCGPopup?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divFMCGPopup').empty();
                            $('#divFMCGPopup').append(vData).trigger('create');
                            $('#txtCurrentCustomerName').text(sessionStorage.CustomerOrganizationName);
                        }
                    });

                    AjaxSend({
                        ExactUrl: '/SalesMenu/FMCGPayment?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divFMCHPayment').empty();
                            $('#divFMCHPayment').append(vData).trigger('create');
                        }
                    });

                    AjaxSend({
                        ExactUrl: '/SalesMenu/FMCGCart?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            var pElement = $(vData);
                            $('#divFMCGCart').empty();
                            $('#divFMCGCart').append(pElement).trigger('create');
                            window.FMCGCart.init(pElement);
                        }
                    });
                }

                function LoadText() {
                    $('#txtOrgName').text(sessionStorage.OrgName.toUpperCase());                    
                    $('#txtFMCGSerialNum').text(Web.DeviceSerial());
                    $('#txtFMCGCashierName').text(Business.LibraryPeople.ByID(Web.PeopleUserID()).FullName);
                    $('#txtFMCGCurDate').text($.format.date(proj.GetZReadDate(), 'MMM dd, yyyy'))
                    setInterval(function () {
                        $('#txtFMCGCurTime').text($.format.date(proj.GetZReadDate(), 'hh:mm:ss a'))
                    }, 1000)

                    if (sessionStorage.FowstID == 'undefined' || sessionStorage.FowstID == undefined) {
                        var pUser = SalesMenu_offline().GetUser();
                        if (pUser.FowstID != undefined) {
                            sessionStorage.EnableHotButtonsContextualMenu = CBool(pUser.EnableHotButtonsContextualMenu);
                            sessionStorage.FowstID = pUser.FowstID;
                            sessionStorage.FowstTin = pUser.TIN;
                            sessionStorage.FowstAddress = pUser.Address;
                        }
                        else {
                            BrowseFowst();
                        }
                    }
                    else {
                        $('#txtFowstName').text(sessionStorage.FowstName.toUpperCase())
                        $('#txtFowstAddress').text(sessionStorage.FowstAddress)
                    }
                }

                function BrowseFowst() {
                    BrowsePopup('LibraryFowst', 'Select FOWST', [
                            {
                                DisplayText: [
                                    { Column: 'FowstName' },
                                    { Column: 'Address' }
                                ],
                                Click: function (e) {
                                    FmcgBasicCart.txtQtyPlu.focus();
                                    sessionStorage.FowstName = e.FowstName;
                                    sessionStorage.FowstAddress = e.Address;
                                    sessionStorage.FowstID = e.ID;
                                    sessionStorage.FowstTin = e.TIN;
                                    sessionStorage.EnableHotButtonsContextualMenu = CBool(e.EnableHotButtonsContextualMenu);
                                    LoadText();
                                    LoadSupplier();
                                    HandleMallLessor();
                                    PredeterminePrinter();
                                    VerifyDevice();
                                    WriteLastFowst();
                                    if (OfflineMode) {
                                        SalesMenu_offline().HandleEnforceZRead();
                                        Android.UpdateSessionData({
                                            FowstID: sessionStorage.FowstID
                                        }); // disconnected checkout
                                        SalesMenu_offline().UpdateCounterFromOnline();
                                    }
                                }, AutoSearch: true,
                                Offline: true
                            }
                    ], { FilterFowst: 1, OrganizationID: sessionStorage.OrgID })
                }

                function GetUser() {
                    var pUser;

                    if (OfflineMode) {
                        pUser = SalesMenu_offline().GetUser();
                        if ((pUser != undefined)
                            && (pUser.FowstID == undefined)) {
                            if (pUser.FowstCount == undefined) {
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
                }

                function LoadUser(vData) {

                    var pUserMenu;
                    var pLink;

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

                        $('#divPosBranch').show();
                        if (CStr2(vData.FowstID).length > 0) {
                            sessionStorage.EnableHotButtonsContextualMenu = CBool(vData.EnableHotButtonsContextualMenu);
                            sessionStorage.FowstID = vData.FowstID;
                            SalesMenu_offline().HandleEnforceZRead({
                                callback: function () {
                                    LoadSupplier();
                                    HandleMallLessor();
                                    $('#lnkPosBranch').html(HtmlEncode('Branch: ' + vData.FowstName) + '&#x25BC;');
                                    $('.lblPosBranch').text(vData.FowstName);
                                    if (proj.POMode)
                                        $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + vData.FowstName);
                                    PredeterminePrinter();

                                    VerifyDevice();

                                    //added by janine
                                    sessionStorage.FowstName = vData.FowstName;
                                    WriteLastFowst();
                                    //addeed by francis
                                    if (IsMobile) {  // condition added by Arvin
                                        try {
                                            Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                                        } catch (e) {
                                            console.error('jsVQWQHN6UG3! ' + e);
                                        }
                                    }

                                }
                            });

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
                        if (window.pageCustomerDetails.LoadCustomer()) {
                            //ADDITIONAL BY JANINE FEB 18
                            //                            LoadRecentlyBought({ DateRangeType: mRecentlyBoughtDateRangeType });

                            NewCart(); // march 26, 2013
                            isCartEmpty = true;


                            //                            pageCustomerDetails.setCustomerInfo();
                        }

                    };

                    if (CDec2(vData.IsSalesAgent) != 1)
                        $('#divCompany').hide();

                }

                function LoadSupplier() {

                    if (proj.POMode) {

                    } else {
                        sessionStorage.removeItem('SupplierOrganizationID');
                        sessionStorage.removeItem('SupplierOrganizationName');
                        sessionStorage.removeItem('SupplierPriceLevelID');
                        sessionStorage.removeItem('SupplierPriceLevelName');
                    }
                }

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
                }

                function PredeterminePrinter() {
                    if (!IsMobile) return;
                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Organization/getOrgFOWSTPrinterIP',
                        success: function (vData) {
                            try {
                                if (vData.OrganizationFOWSTData.Rows.length == 0) {
                                    NewMessageBox('This device has no assigned printer in the selected FOWST');
                                };
                            } catch (e) {
                                NewMessageBox('Unable to determine "vData.OrganizationFOWSTData.Rows.length"');
                                console.error(e);
                            };
                        }
                    });

                }

                function VerifyDevice() {
                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Login3/VerifyDevice',
                        success: function (vData) {
                            if (vData.Error != undefined) {
                                NewMessageBox('This device is not assigned to this FOWST',
                                    'FOWST',
                                    [
                                        {
                                            Text: 'Ok',
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
                    });
                }

                function WriteLastFowst() {
                    if (IsMobile) {  // condition added by Arvin
                        try {
                            Transfers.WriteFile('LastFowst.txt', null, sessionStorage.FowstName);
                        } catch (e) {
                            console.error('jsVQWQHN6UG3! ' + e);
                        }
                    }
                }
            })
        })
        
    })    
})
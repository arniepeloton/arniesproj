define([GetPathFormat('/Scripts3/common/common.js?v=' + assetversion)], function () {

    if (window.proj == undefined)
        window.proj = {};

    var mZReadDate;
    var pParams = GetUrlParameters();
    if (pParams.OrgID != undefined) {
        sessionStorage.OrgID = pParams.OrgID;
    }; 
    proj.DateFormat = 'MMM d, yyyy h:mm a';
    proj.MaxNumber = Math.pow(10, 13) - 1;
    // proj.SqliteNow = 'datetime(\'now\', \'localtime\')';
    proj.SqliteNow = 'STRFTIME(\'%Y-%m-%d %H:%M:%f\', \'NOW\', \'localtime\')';
    proj.AjaxTimeout = 5000;
    proj.QuantityDecimals = 6; // maximum number of decimals for "quantity" fields
    proj.QuantityFormat = '#,##0';
    if (CDec2(proj.QuantityDecimals) > 0) {
        proj.QuantityFormat += '.' + '#'.repeat(proj.QuantityDecimals);
        proj.QuantityStep = '0.' + '0'.repeat(proj.QuantityDecimals - 1) + '1';
    }
    proj.WideWidth = 550;
    // proj.WideWidth = 5500; // for debugging only. to simulate cellphone screen size
    proj.CustomPriceLevelID = '62E1E267-AE0A-4EDC-9AC3-B2CC3991159C';
    proj.RetailPriceID = '01000012-0005-0000-0000-000000000000';
    proj.RebateData = null; // reserve this property name 
    proj.ModifierGroupID = null; // reserve this property name
    proj.CartData = null; // reserve this property name
    proj.ShowSalesReps = null; // reserve this property name
    proj.CartStock = null; // reserve this property name
    proj.SOMode = null; // reserve this property name
    proj.PLMode = null; // picking list mode. reserve this property name
    proj.SectionID = null; // reserve this property name
    proj.SectionText = null; // reserve this property name
    proj.FowstName = null; // reserve this property name
    proj.ReplacedProductID = null; // during picking list
    proj.CustomerScreenTransmitter = null; // reserve this property name
    proj.NewCartPos = null; // reserve this property name
    proj.NewCartBOM = null; // reserve this property name
    proj.BomMakerMode = null;  // reserve this property name
    proj.BomMakerSave = null;  // reserve this property name
    proj.DisplayCart = null; // reserve this property name
    proj.PrintStickers = null; // reserve this property name
    proj.LogInvoiceNumberCounter = false;
    proj.OnPinch = function(){}; // reserve this property name
    proj.OnFullScreen = function(){}; // reserve this property name
    proj.RetransmitToSecondScreenTimeoutHandle = null; // reserve this property name
    proj.RetransmitToSecondScreen = function(){}; // reserve this property name
    proj.SalesMenuOrderTakerLoadView = function(){}; // reserve this property name
    proj.SupplierDisplayData = function(){}; // reserve this property name
    proj.GetSupplierInfoFromHtml = function(){}; // reserve this property name
    proj.SupplierClearFields = function(){}; // reserve this property name
    proj.VerifySupplier = function(){}; // reserve this property name
    proj.PrintQueueNumberIfNeeded = null; // reserve this property name
    proj.PeerReceivers = [];
    proj.HotButtonsUpdateCartStock = null;
    proj.CustomerPaymentID = null; // reserve this property name
    proj.CacMode = null; // AR (default) or AP
    proj.LoadEasSalesOrderTrans = null; // reserve this property name
    proj.SalesMenuInit = null; // reserve this property name
    proj.ReloadTableView = null; // reserve this property name
    proj.BeforeChangeFowst = null; // reserve this property name
    proj.goToHotButtonPage = null; // reserve this property name
    proj.ShowFulfillmentCenterTransaction = null; // reserve this property name
    proj.SalesReturnFromOffline = false;
    proj.SalesReturnStock_PageShow = null; // function
    proj.SrvFowstID = null; // reserve this property name
    proj.FowstTemplateID = null; // reserve this property name
    proj.NewCartHotButtons = null; // reserve this property name
    proj.GetUnapprovedPurchaseOrderCount = null; // reserve this property name
    proj.DownloadDatabasePageShowError = null; // reserve this property name
    proj.LoadPaymentInvoices = null; // \Scripts\sources\ERP\Payment\controller.js
    proj.IsStockLoadedFromServer = null;
    proj.RemoteServer = null;

    var mPeerConnect; // App.controller.SecondScreenConnect();
    
    proj.LoadBirAccreditation = function(vAccreditation, vVersionNo) {
        vAccreditation = CStr2(vAccreditation).trim().toLowerCase();
        vVersionNo = vVersionNo || '1';
        proj.MobizVersion = vVersionNo;
        if (vAccreditation == 'duc') { // change to "false" if DUC accre
            proj.AccreditedSupplier = 'Davao Unified Corporation'; // Accredited Supplier
            proj.RegisteredAddress = 'STA ANA AVENUE FORMERLY Y GYM DAVAO CITY'; // Registered Address
            proj.TinBranchCode = '005-161-114-000'; // TIN-Branch Code
            proj.PosAccreditation = '13200516111400019366756'; // POS Accreditation
            proj.PosAccreditationDateIssued = 'July 31, 2013';
            proj.PosAccreditationValidUntil = 'Aug 30, 2025';
        } else {
            proj.AccreditedSupplier = '1MOBIZ, INC.'; // Accredited Supplier
            proj.RegisteredAddress = 'BOLTON ST., DAVAO CITY'; // Registered Address
            proj.TinBranchCode = '462756815-000'; // TIN-Branch Code

            if (vVersionNo == '1') {
                proj.PosAccreditation = '1134627568152015050299'; // POS Accreditation
                proj.PosAccreditationDateIssued = 'May 4, 2015';
                proj.PosAccreditationValidUntil = '';
            } else {
                proj.PosAccreditation = '1134627568152026022882'; // POS Accreditation
                proj.PosAccreditationDateIssued = 'Mar 24, 2026';
                proj.PosAccreditationValidUntil = '';
            }            
        }
    };
    //proj.LoadBirAccreditation();
    

    proj.ClockPrefix = '';

    proj.HandleSessionExpired = function (vData) {
        // PARAMS:
        //  vData: from Ajax. Detect if vData.d.Error == 'Access denied'
        // RETURNS:
        //  false if vData.d.Error == 'Access denied'. Otherwise, true

        if (vData && vData.Error == 'Access denied') {
            proj.PromptLogin();
            return false;
        } if (vData && vData.Error == 'Invalid organization') {
            MessageBox(vData.Error, '', null, {
                OnClose: function() {
                    ChangePage('People/PeopleMenu');
                }
            });
        } else
            return true;
    }; // proj.HandleSessionExpired = function (vData) {

    proj.PromptLogin = function(vMessage) {
        // PARAMS:
        //  vMessage: optional

        var pTitle;

        if (CStr2(vMessage).trim() == '') {
            vMessage = 'Your web browsing session has expired due to extended period of inactivity or the webapp was upgraded/restarted. Please log in again. <br><br>' +
                    '<div class="mb-3">' +
                        '<div class="input-group">' +
                            '<span class="bi-person-fill input-group-text border-primary bg-primary text-white"></span>' +
                            '<input type="text" placeholder="Username" class="form-control form-control-lg border-primary" data-role="none" name="username" id="txtPromptUsername" />' +
                        '</div>' +
                    '</div>' +
                    '<div class="mb-0">' +
                        '<div class="input-group">' +
                            '<span class="bi-key-fill input-group-text border-primary bg-primary text-white"></span>' +
                            '<input type="password" placeholder="Password" class="form-control form-control-lg border-primary" data-role="none" name="password" id="txtPromptPassword" />' +
                        '</div>' +
                    '</div>';
            pTitle = 'You\'ve been logged out';
        }
   
        if (window.NewMessageBox)
            var msgbox = NewMessageBox(vMessage, pTitle,
                [
                    {
                        Text: 'Log In', Validation: function(){
                            var txtPromptUsername = $.trim($('#txtPromptUsername').val()), txtPromptPassword = $.trim($('#txtPromptPassword').val());

                            if (txtPromptUsername == '') {
                                WebToast('Please enter your username', 'top');
                                return false;
                            }

                            if (txtPromptPassword == '') {
                                WebToast('Please enter your password', 'top');
                                return false;
                            }

                            return true;
                        }, Click: function () {
                            var pData = {}, txtPromptUsername = $.trim($('#txtPromptUsername').val()), txtPromptPassword = $.trim($('#txtPromptPassword').val());
                            
                            pData.Username = txtPromptUsername;
                            pData.UserPassword = txtPromptPassword;

                            if (proj.IsCookieless == 1) {
                                pData.IsCookieless = 1;
                                docCookies.removeItem('ASP.NET_SessionId');
                            };
                            pData.Uuid = localStorage.UUID;

                            Bs.Loading('show');
                            AjaxSend({
                                url: 'People/CheckPeopleUserAccount',
                                data: pData,
                                // online mode = dont put timeout
                                success: function (vData) {
                                    if ($.trim(vData.PeopleUserAccountData) != '') {
                                        sessionStorage.PeopleUserAccountData = vData.PeopleUserAccountData;
                                        localStorage.PeopleUserAccountData = vData.PeopleUserAccountData;
                                        if (vData.HashedPassword) {
                                            localStorage.Username = vData.Username;
                                            localStorage.HashedPassword = vData.HashedPassword;
                                        }
                                        sessionStorage.RemoteServer = vData.RemoteServer;
                                        if (proj.IsCookieless == 1) {
                                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                                        } else {
                                            localStorage.removeItem('ASP.NET_SessionId');
                                        };

                                        pData = {};
                                        pData.IsNew = 0;
                                        pData.ID = vData.PeopleUserAccountData;

                                        AjaxSend({
                                            url: 'People/setCreateNew',
                                            data: pData,
                                            success: function (vData) {
                                                var pParam;
                                                var pUrl;

                                                var pDataPeopleID = {};
                                                pDataPeopleID.ID = pData.ID;

                                                AjaxSend({
                                                    url: 'SignIn/CheckPeopleIsFirstPartySignUp',
                                                    data: pDataPeopleID,
                                                    success: function (vData) {
                                                        sessionStorage.PeopleID = pData.ID;
                                                        sessionStorage.CurrentUserFullName = vData.PeopleFirstPartySignUpData.Rows[0].FullName;
                                                        $('#txtLoggedUserName').text((sessionStorage.CurrentUserFullName).split(' ')[0]);
                                                        WebToast('Your session has been restored', 'top');
                                                        Bs.Loading('hide');
                                                    } // success
                                                })
                                            }
                                        });
                                    } else {
                                        Bs.Loading('hide');
                                        WebToast('Invalid username/password', 'top');
                                        msgbox.modal('show');
                                    };
                                } // success
                            })
                        }
                    },
                    {
                        Text: 'Go to login page', Click: function () {

                        }
                    }
                ], {
                    OnClose: function () {
                        return;
                        if (IsMobile)
                            // ChangePage('People?IsMobile=1');
                            window.location = '/SignIn?IsMobile=1';
                        else if (CStr2(localStorage.getItem('LoginUrl')).trim() != '')
                            window.location = localStorage.getItem('LoginUrl');
                        else
                            if (location.pathname.toLowerCase() == '/erp') {
                                window.location = '/signin/erp?IsMobile=0';
                            } else if (location.pathname.toLowerCase().includes("ecommerce")) {
                                window.location = '/Ecommerce/SignIn?OrgID=' + sessionStorage.OrgID;
                            } else if (location.pathname.toLowerCase().includes("vendorportal")) {
                                window.location = '/VendorPortal/SignIn?OrgID=' + sessionStorage.OrgID;
                            } else if (location.pathname.toLowerCase().includes("sidocscanner")) {
                                window.location = '/SIDocScanner/Signin';
                            } else
                            window.location.reload();
                    }
                }
            ); // END NewMessageBox(vMessage, pTitle,

    };

    proj.ClearSessionStorage = function() {

        var p = 0;
        var pPreservedItems = [
            'FowstID',
            'OrgID',
            'CustomerOrganizationID',
            'IsMobile',
            'HotButtonMenuID',
            'HotButtonData',
            'TotalNumberOfCustomer',
            'NoOfSeniorCitizens',
            'CurrentUserFullName',
            'UUID',
            'PeopleID',
            'OrgName',
            'DefaultOtherPaymentID',
            'HotButtonDataCacheTemplateID',
            'HotButtonDataCacheWithContentsOnly',
            'HotButtonDataCacheBatchProductOnly',
            'HotButtonDataCacheCustomerID',
            'HotButtonData',
	    'AdditionalMaxAllowableDiscount'
            // put additional items here
        ];
        var pPreserve;

        do {
            pPreserve = false;
            for (var pIndex = 0; pIndex <= pPreservedItems.length - 1; pIndex++) {
                if (pPreservedItems[pIndex] == sessionStorage.key(p)) {
                    pPreserve = true;
                    break;
                };
            };

            if (pPreserve) {
                p++; // try next sessionStorage item
            } else {
                sessionStorage.removeItem(sessionStorage.key(p));
            };
        } while (p <= sessionStorage.length - 1);

    }; // ClearSessionStorage

    proj.PromptRequiresInternet = function(vParams) {
        // OPTION:
        //  Retry

        var pButtons = [];
        var pOptions;

        vParams = $.extend({
        }, vParams);
        if ($.isFunction(vParams.Retry)) {
//            pOptions = {
//                OnClose: vParams.Retry
//            };
            pButtons.push({
                Text: 'Retry',
                Click: vParams.Retry
            });   
        };
        pButtons.push({
            Text: 'Go to menu page',
            Click: function() {
                window.location = '/People/PeopleMenu?IsMobile=1';
            }
        });

        MessageBox('This feature requires an Internet connection.', 'Requires Internet', pButtons, pOptions);

    }; // END PromptRequiresInternet

    proj.PromptTestInternet = function(vParams) {
        // PARAMS:
        // vParams = function() {}
        // - or -
        //vParams = {
        //    method: function() {},
        //    timeout: 0, // in milliseconds
        //    context: this // set this if you use "this" in your function
        //    error: function() {} // if set, called when cannot connect and but another button "Use offline-only features"
        //}

        var pHasError;

        if ($.isFunction(vParams)) {
            vParams = {
                method: vParams,
                timeout: 5000
            }
        } else 
            vParams = $.extend({
                timeout: 5000,
                AllowOffline: null // function
            }, vParams);

        Bs.Loading('show');
        AjaxSend({
            url: 'Login3/TestCookies',
            timeout: vParams.timeout,
            success: function (vData) {
                Bs.Loading('hide');
                if (vParams.method != null)
                    try {
                        vParams.method.call(vParams.context);
                    } catch (e) {
                        pHasError = true;
                        throw e;
                    }
            },
            error: function (vError) {
                Bs.Loading('hide');
                var pButtons;

                if (pHasError)
                    return;

                pButtons = [
                    { 
                        Text: 'Try again later'
                        //,Click: function() {
                        //    window.location = '/People/PeopleMenu';
                        //}
                    },
                    {
                        Text: 'Wait for 10 seconds more',
                        Click: function() {
                            Android.SetAutoOfflineMode(false);
                            proj.PromptTestInternet($.extend(vParams, {
                                timeout: 10000
                            }));
                        }
                    }
                ];

                if ($.isFunction(vParams.AllowOffline))
                    pButtons.push({
                        Text: 'Use offline data. Only data created in this device after the latest data sync will be available for use.',
                        Click: vParams.AllowOffline
                    });

                if ($.isFunction(vParams.error))
                    pButtons.push({
                        Text: 'See offline reports',
                        Click: vParams.error
                    });

                if (vParams.Message == null)
                    vParams.Message = "The connection to the server is not available or taking more time than expected.";
                if (vParams.MessageSuffix != null)
                    vParams.Message += vParams.MessageSuffix;
                MessageBox(vParams.Message,
                        "", pButtons
                );
            } // END error

        }); // END AjaxSend
    }; // END PromptTestInternet

    proj.EnsureMobile = function()  {
        if (CDec2(GetUrlParameters().IsMobile) == 0) {
            location = MergeUrlParameters({
                IsMobile: 1
            });

            return false;
        };

        return true;

    }; // END EnsureMobile

    proj.SetZReadDate = function(vDate) {
        mZReadDate = vDate;
    }; // END SetZReadDate

    proj.GetZReadDate = function(vTime) {
        var pDate;
        if (vTime == null)
            pDate = new Date();
        else
            pDate = vTime;
        var pOut;

        if (IsDate(mZReadDate)) {
            pOut = RemoveTime(mZReadDate);
            pOut.setHours(pDate.getHours());
            pOut.setMinutes(pDate.getMinutes());
            pOut.setSeconds(pDate.getSeconds());
            pOut.setMilliseconds(pDate.getMilliseconds());
        } else
            pOut = pDate;

        return pOut;

    }; // END GetZReadDate

    proj.GetZReadDateSql = function() {
        return ReadyDate(proj.GetZReadDate());
    };

    proj.SalesMenu = proj.SalesMenu || {};
    proj.SalesMenu.goToSalesMenu = function () {
        if (proj.IsBootstrapDesign()) { // based sa url params
            return;
        }
        if ((window.location.pathname.toLowerCase() == '/salespayments')
            || (window.location.pathname.toLowerCase() == '/salesmenu')) {
            //window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
            window.location = location = MergeUrlParameters({
                mode: 'pos',
                OrgID: sessionStorage.OrgID,
                IsMobile: CBool(IsMobile) ? 1 : 0
            }, '/SalesMenu');

        } else {
            if ($.isFunction(GetStringObject('proj.SalesPayments.ClearSalesPaymentsInfoFields')))
                proj.SalesPayments.ClearSalesPaymentsInfoFields();

            location = MergeUrlParameters({
                OrgID: sessionStorage["OrgID"],
                mode: 'pos',
                IsMobile: IsMobile ? 1 : 0
            }, '/SalesMenu/POSViewAll');
             
        } //end if location
    }; // END proj.SalesMenu.goToSalesMenu = function() {

    proj.OnSettings = function(){
        require(['promise!/Scripts3/controller/SecondScreenPin.js?crc='], function() {
            var p = new App.controller.SecondScreenPin();
            p.success = function() {
                history.back();
                setTimeout(Show, 200);
            };
            p.popup();
        });

        function Show() {
            NewMessageBox('', null, [
                {
                    Text: 'About',
                    Click: function() {
                        Android.ShowAbout();
                    }
                },
                {
                    Text: 'Check for Updates',
                    Click: function() {
                        Android.UpdateApk();
                    }

                },
                {
                    Text: 'Restart Customer Display App',
                    Click: function() {
                        Android.PromptRestart();
                    }
                },
                {
                    Text: 'Connect to POS unit',
                    Click: function() {
                        require(['promise!/Scripts3/controller/SecondScreenConnect.js?crc='], function() {
                            if (mPeerConnect == null)
                                mPeerConnect =  new App.controller.SecondScreenConnect();
                            mPeerConnect.popup();
                        });
                        return false;
                    }
                },
                {
                    Text: 'Change PIN',
                    Click: function() {
                        require(['promise!/Scripts3/controller/SecondScreenChangePin.js?crc='], function() {
                            var p = new App.controller.SecondScreenChangePin();
                            p.popup();
                        });
                    }

                },
                {
                    Text: 'Exit Customer Display App',
                    Click: function() {
                        Android.ExitApp();
                    }
                }
            ], {
                CloseButton: 'enable'
            }); // END NewMessageBox('', null, [
        } // END function Show() {

        
    };  // END proj.OnSettings = function(){

    proj.CanChangeTableNumber = function() {
        // this is not table view.
        // this is dine-in -> table number

        if (CStr2(sessionStorage.CustomerTableNo).trim() == '')
            return true;

        if (IsNumeric(sessionStorage.CustomerTableNo))
            return true;

        return false;
    };

    proj.HandlePosIsEditable = function(vForPrinting) {
        var pMaster = GetStringObject('proj.CartData.Master');
        var pTransDeviceID = GetStringObject('DeviceID', pMaster);
        pTransDeviceID = ReadyGuid(pTransDeviceID);
        if (pTransDeviceID == '')
            return true;
        var pDeviceID = Web.DeviceID();
        pDeviceID = ReadyGuid(pDeviceID);
        if (pTransDeviceID == pDeviceID)
            return true;

        var pMsg = 'You may not ' + (vForPrinting ? 'print' : 'edit') + ' this transaction because this transaction did not originate from this device.';
        try {
            var pLastUpdated = pMaster.LastUpdated;
            pLastUpdated = FormatDate(pLastUpdated, {Format: proj.DateFormat});
            var pUser = GetStringObject('UserID', pMaster);
            pUser = Business.LibraryPeople.ByID(pUser);
            var pDevice = GetStringObject('DeviceID', pMaster); 
            pDevice = Business.LibraryDevice.ById(pDevice);
            Business.LibraryDevice.ById(pDevice);
            pMsg += 'Last saved on ' + pLastUpdated + ' by ' + pUser.FullName + ' using ' + pDevice.PhoneName + ' ' + pDevice.PhoneModel + ' with serial number, ' + pDevice.UUID + '. ';
        } catch (e) {
            Android.AddLog('jsG23TJ5K67P! ' + e.message + '\n' + e.stack);
        }
        pMsg += 'Please ' + (vForPrinting ? 'print' : 'edit') + ' this transaction using that device.';
        NewMessageBox(pMsg);
    };

    proj.IsBootstrapDesign = function () {
        if (typeof IsBootstrap != 'undefined') { //BootstrapFiveLayout.vbhtml
            return CBool(IsBootstrap);
        }
        if (GetUrlParameters().theme == 'bs') {
            return true;
        } else return false;
    };

    proj.AutoPrintEnrollment = function(vParams) {
        
        require(['promise!/Scripts3/NewMessageBox.js?crc=' + assetversion], function() {
            vParams = $.extend({
                done: function(){}
            }, vParams);
            if (CBool(localStorage.DontPromptAutoPrint)) {
                vParams.done();
                return false;
            }
            if (CStr2(localStorage.UUID).trim() == '') {
                vParams.done();
                return false;
            }
            if (location.href.indexOf('/erp?OrgID=') == -1) {
                vParams.done();
                return false;
            }
            AjaxSend({
                url: '/ERP/GetAutoPrintUuid',
                data: {
                    Uuid: CStr2(localStorage.UUID).trim()
                },
                success: function(vData) {
                    vData.AutoPrintUuid = CStr2(vData.AutoPrintUuid).trim();
                    if (vData.AutoPrintApproval == 'approved') {
                        if (vData.AutoPrintUuid == CStr2(localStorage.UUID).trim()) {
                            AutoPrintLoop();
                            NewMessageBox('This workstation is assigned to print auto created invoice', 'Auto Print Workstation', null, {
                                Close: function() {
                                    vParams.done();
                                }
                            });
                        } else 
                            NewMessageBox('Another workstation (UUID: ' + CStr2(vData.AutoPrintUuid).trim() + ') is assigned to print auto created invoices.<br /><br />' +
                                'Do you want your workstation to auto print invoices?', 'Auto Print Workstation', 
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function() {
                                            AssignAutoPrint();
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function() {
                                        }
                                    }
                                ], {
                                    Close: function() {
                                        vParams.done();
                                    }
                                }
                            );
                    } else if (vData.AutoPrintApproval == 'none') {
                        NewMessageBox('Another workstation (UUID: ' + CStr2(vData.AutoPrintUuid).trim() + ') is assigned to print auto created invoices.<br /><br />' +
                            'This workstation does not have a pending request to auto print invoice. Do you want to submit a request to auto print invoices in this workstation?', 'Submit Auto Print Request', 
                            [
                                {
                                    Text: 'Yes',
                                    Click: function() {
                                        SubmitAutoPrintRequest();
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function() {
                                    }
                                }
                            ], {
                                Close: function() {
                                    vParams.done();
                                }
                            }
                        );
                    } else {
                        NewMessageBox('Another workstation (UUID: ' + CStr2(vData.AutoPrintUuid).trim() + ') is assigned to print auto created invoices.<br /><br />' +
                            'This workstation has a pending request to auto print invoice and it was ' + vData.AutoPrintApproval + '.', 'Pending Auto Print Request', 
                            [
                                {
                                    Text: 'OK',
                                    Click: function() {
                                    }
                                }
                            ], {
                                Close: function() {
                                    vParams.done();
                                }
                            }
                        );
                    }
                },
                error: function(vError) {
                    vParams.done();
                }
            }); // END AjaxSend({
        }); // END require(['promise!/Scripts3/NewMessageBox.js?crc=' + assetversion], function() {
    };

    var isLoadStockInfo = false;
    proj.ShowStockInfo = function (callback) {
        if (!isLoadStockInfo) {
            AjaxSend({
                ExactUrl: '/DashBoardsAndReports/SalesOrderStockViewer?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    isLoadStockInfo = true;
                    $('body').append(vData);
                }
            });
        } else {
            StockInfo.View();
        }
    }

    function SubmitAutoPrintRequest() {
        
        AjaxSend({
            url: '/ERP/RequestAutoPrint',
            data: {
                Uuid: CStr2(localStorage.UUID).trim()
            },
            success: function(vData) {
                if (vData.Result == 'ok') {
                    NewMessageBox('Your request was submitted successfully', 'Auto Print Request', null,
                    {
                        Close: function() {
                            //vParams.done();
                        }
                    });
                } else if (vData.Status == 'pending') {
                    NewMessageBox('You already have a pending request.', 'Auto Print Request', null,
                    {
                        Close: function() {
                            //vParams.done();
                        }
                    });
                } else if (vData.Status == 'approved') {
                    NewMessageBox('This workstation already have a previous request and it was approved.', 'Auto Print Request', null,
                    {
                        Close: function() {
                            //vParams.done();
                        }
                    });
                } else if (vData.Status == 'disapproved') {
                    NewMessageBox('This workstation already have a previous request and it was disapproved.', 'Auto Print Request', null,
                    {
                        Close: function() {
                            //vParams.done();
                        }
                    });
                } 
            },
            error: function(vError) {
                //vParams.done();
            }
        });
    }

    function AssignAutoPrint() {
        AjaxSend({
            url: '/ERP/SetAutoPrintUuid',
            data: {
                AutoPrintUuid: CStr2(localStorage.UUID).trim()
            },
            success: function(vData) {
                if (vData.Result == 'ok') {
                    AutoPrintLoop();
                    NewMessageBox('This workstation will print auto created invoices', 'Auto Print Workstation', null,
                    {
                        Close: function() {
                            vParams.done();
                        }
                    });
                }
            },
            error: function(vError) {
                vParams.done();
            }
        });
    }

    function AutoPrintLoop() {
        
        AjaxSend({
            url: '/ERP/GetAutoPrintableInvoice',
            data: {
                Uuid: CStr2(localStorage.UUID).trim()
            },
            success: function(vData) {
                if (vData.Data)
                    CreatePdf(vData.Data);
                else
                    setTimeout(function() {
                        AutoPrintLoop();
                    }, 1000 * 30);
            },
            error: function(vError) {
                setTimeout(function() {
                    AutoPrintLoop();
                }, 1000 * 30);
            }
        });
    }

    function CreatePdf(vDR) {
        
        AjaxSend({
            url: 'ERP/CreatePdf',
            data: {
                InvoiceID: vDR.ID,
                TransType: 'Invoice'
            },
            success: function(vData) {
                setTimeout(function() {
                    AutoPrintLoop();
                }, 1000 * 30);
            },
            error: function(vError) {
                setTimeout(function() {
                    AutoPrintLoop();
                }, 1000 * 30);
            }
        });

    }

    proj.ChangePriceLevelDisplay = function(vText) {
        var pText = CStr2(vText).trim().toLowerCase();
        if ((pText == 'FACTORY PRICE'.toLowerCase())
            || (pText == 'FACTORY'.toLowerCase()))
            return 'Factory / Vendor Cost';
        else if ((pText == 'FREIGHT COST'.toLowerCase())
            || (pText == 'Freight'.toLowerCase()))
            return 'Freight / Shipping Cost';
        else if (pText == 'LANDED COST'.toLowerCase())
            return 'Landed Cost';
        return vText;
    };

    proj.CheckIfThereAreDraftStockRecTrans = function (callback, page) {
        if (!IsMobile) {
            callback();
            return;
        }

        if (!CBool(Business.SystemDefaults.GetSystemDefault('RemindUserAboutTheDraftTransactionsStockRecTrans', 0))) {
            callback();
            return;
        }

        var draftSR = Sql.Execute("SELECT * FROM TransReceiving WHERE IsDraft = 1 AND IsReceiving = 1").Rows.length;
        var draftST = Sql.Execute("SELECT * FROM TransReceiving WHERE IsDraft = 1 AND IsReceiving = 0").Rows.length;

        if (draftSR > 0) {
            NewMessageBox('You have ' + draftSR + ' pending Draft Stock Receiving transaction' + (draftSR > 1 ? 's' : '') + '. Your administrator requires you to finish your draft transaction' + (draftSR > 1 ? 's' : '') + ' before you can ' + (page == 'inventory' ? 'proceed to Inventory Count' : 'view your Z-Read') +'.<br><br>Would you like to continue the Draft Stock Receiving transaction' + (draftSR > 1 ? 's' : '') + '?', 'Message', [
                {
                    Text: 'Yes', Click: function () {
                        PromptUserAccess('Receiving and Transfers', {
                            OnClickOKSuccess: function () {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsReceiving: 1,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/Receiving' + (CBool(IsMobile) ? '/BsIndex' : ''));
                            }
                        });
                    }
                },
                {
                    Text: 'No', Click: function () {
                        //callback();
                    }
                }
            ])
        //} else if (draftST > 0) {
        //    NewMessageBox('You have ' + draftST + ' pending Draft Stock Transfer transaction' + (draftST > 1 ? 's' : '') + '. Would you like to continue the Stock Transfer transaction' + (draftST > 1 ? 's' : '') + '?', 'Message', [
        //        {
        //            Text: 'Yes', Click: function () {
        //                PromptUserAccess('Receiving and Transfers', {
        //                    OnClickOKSuccess: function () {
        //                        location = MergeUrlParameters({
        //                            OrgID: sessionStorage.OrgID,
        //                            IsReceiving: 0,
        //                            IsMobile: CBool(IsMobile) ? 1 : 0
        //                        }, '/Receiving' + (CBool(IsMobile) ? '/BsIndex' : ''));
        //                    }
        //                });
        //            }
        //        },
        //        {
        //            Text: 'No', Click: function () {
        //                //callback();
        //            }
        //        }
        //    ])
        } else {
            callback();
        }
    }

    proj.CheckIfInventoryIsDone = function (callback, page) {
        if (!IsMobile) {
            callback();
            return;
        }

        if (CBool(Business.SystemDefaults.GetSystemDefault('DontAllowViewZreadWhenNoInventory', 0))) {
            if (Sql.Execute('SELECT * FROM TransInventoryProductCount WHERE date(InventoryCountDate) = ' + ReadyString($.format.date(new Date(proj.GetZReadDateSql()), 'yyyy-MM-dd'))).Rows.length == 0) {
                NewMessageBox('You are required by your administrator to perform an Inventory Count before viewing/printing the Z-Read report as part of the closing procedure.<br><br>Would you like to perform the Inventory Count?', 'Message', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            PromptUserAccess('Inventory Count', {
                                OnClickOKSuccess: function () {
                                    proj.CheckIfThereAreDraftStockRecTrans(function () {
                                        location = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        }, '/InventoryCount' + (CBool(IsMobile) ? '/BsIndex' : ''));
                                    })
                                }
                            });
                        }
                    },
                    {
                        Text: 'No, maybe later', Click: function () {

                        }
                    }
                ])
            } else callback();
        } else callback();
    };

    proj.InitDoneSchema = function (vParams) {
        vParams = $.extend({
            DontKick: false // if true, prevents: 'This store is inside a Robinsons Mall. Cashiering transactions are not accepted from 4:00 AM to 9:00 AM.'
        }, vParams);

        if (!IsMobile)
            return;

        // commented 2024_12_04__15_15_09_7498
        //if (!CBool(Transfers.ReadFile('enable rlc.txt')))
        //    return;

        if (CBool(Transfers.ReadFile('disable rlc.txt'))) // useful for troubleshooting
            return;

        var pOrgID;
        var pFowstID;
        var pDebuggerIsAttached = true;
        Loop();
        function Loop() {
            setTimeout(function() {
                if (!pDebuggerIsAttached)
                    Loop();
                else if (window.Android == null) 
                    Loop();
                else if (Android.DoneSchema()) 
                    Proceed();
                else
                    Loop();
            }, 1000);
        }

        function Proceed() {
            pOrgID = Transfers.ReadFile('LastOrgID.txt');
            if (!IsGuid(pOrgID))
                return;
            pFowstID = Transfers.ReadFile('LastFowstID.txt');
            if (!IsGuid(pFowstID))
                return;

            Proceed3();
        }
                 
        function Proceed3() {
            Web.SetOrgID(pOrgID);
            if (!Web.HandleOrganization()) {
                return;
            }

            require(['promise!/Scripts3/Browser/LibraryFowst.js?crc=',
                '/Scripts3/SalesMenu_offline.js?crc='], function() {
                var pDtFowsts = OfflineBrowser.LibraryFowst.GetData({
                    ID: pFowstID,
                    IsThirdPartyOrganization: true,
                    ThirdPartyOrganizationID: pOrgID
                });
                if (pDtFowsts.Rows.length == 0)
                    return;
                var pDrFowst = pDtFowsts.Rows[0];
                if (!CBool(pDrFowst.IsFOWSTInsideAMall))
                    return;
                if (pDrFowst.FOWSTMallName != 'Robinson')
                    return;
                sessionStorage.FowstID = pFowstID;
                SalesMenu_offline().HandleEnforceZRead({
                    IsFOWSTInsideAMall: pDrFowst.IsFOWSTInsideAMall,
                    FOWSTMallName: pDrFowst.FOWSTMallName,
                    Operates247: pDrFowst.Operates247,
                    DontKick: vParams.DontKick
                });
            });
        }
    };

    proj.CheckIfVDMFeaturesIsActivated = function (callback) {
        AjaxSend({
            url: '/SetupChecklist/CheckIfVDMFeaturesIsActivated',
            success: function (e) {
                callback(e);
            }
        })
    }    
}); // end
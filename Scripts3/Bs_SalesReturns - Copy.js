require(['common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc='),
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''),
    'promise!/Scripts3/NewMessageBox.js?crc=' + assetversion
    ], function () {

        //        $(window).on('hashchange', function (event, data) {
        //            var pDummy;
        //        });

        $(function () {

            var intDesktopPrintLenght = 45;

            var mLvContents;
            var mPage;
            var mLvCart;
            var mCheckOutVerify;
            var mIsRefund;
            var mApproved; // boolean
            var mDenied; // boolean
            var mUseOfflineData = false;
            var mAllowOfflineSrvPayments;
            var mHideMerchandiseExchange;
            var browserDeviceID;
            var mRemoteServer = null;
            var mIsStockLoadedFromServer;
            var mLatestCheckOutTime;

            $(document).ready(function () {

                if (!window.InAndroid) {
                    $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                }

            });

            (function Init() {
                if (location.pathname.toLowerCase() != '/vendingmachine') {

                    mPage = $.mobile.activePage;
                    EnsureFirstPage();
                    $('#popPeopleMenuPeopleInfoSummary').click(function (e) {
                        e.preventDefault();

                        location = "/People/PeopleInfoSummary";

                    });
                    $('#btnSignOut').click(function () {

                        SignOut();

                    });

                    $('.btnHome').click(function () {
                        if (GetUrlParameters().source == 'webpos') {
                            history.back();
                        } else {
                            location = "/People/PeopleMenu";
                        }
                    });

                    $('#butGotoSalesReturn').click(function (e) {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/SalesReturns/BsIndex');
                    });

                    $('#butGotoPos').click(function (e) {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0,
                            mode: 'pos',
                        }, '/SalesMenu/BsIndex');
                    });

                    $('#butGotoMainMenu').click(function (e) {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/People/PeopleMenu');
                    });

                    $('#btnDateFrom').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (v) {
                                $('#btnDateFrom').text(v);//.button('refresh');
                            }
                        })
                    })

                    $('#btnDateTo').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (v) {
                                $('#btnDateTo').text(v);//.button('refresh');
                            }
                        })
                    })

                    $('#btnFOWST').click(function (e) {
                        //e.preventDefault();
                        BrowseAddresses();
                        //return false;
                    });

                    InitUser();

                    startclock('#clockSalesReturn');

                    var pPage = $.mobile.activePage;
                    var pHeader = pPage.find("[data-role=header]");
                    var pHeight = pHeader.height() + 10;

                    //pPage.css('padding-top', pHeight + 'px');

                    if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                        pHeader.fixedtoolbar({
                            tapToggle: false,
                            updatePagePadding: false
                        });
                    $('#btnDateFrom').text($.format.date(new Date(), 'MMM dd, yyyy'));//.button('refresh');
                    $('#btnDateTo').text($.format.date(new Date(), 'MMM dd, yyyy'));//.button('refresh');

                    $('#txtDateFrom').DateCombo('val', '1/1/1900');
                    $('#txtDateTo').DateCombo('val', new Date());

                    Ajax_SalesReturns_GetSettings({
                        success: function (vSRVSettingsData) {
                            mAllowOfflineSrvPayments = CBool(vSRVSettingsData.AllowOfflineSrvPayments);
                            mHideMerchandiseExchange = CBool(vSRVSettingsData.HideMerchandiseExchange);
                            if (mHideMerchandiseExchange)
                                $('#butCheckOutNow').hide();
                            else
                                $('#butCheckOutNow').show();
                        }
                    });

                    if (!IsMobile) {
                        AjaxSend({
                            url: '/SalesMenu/GetCustomer',
                            data: {
                                CreateCacheDatabaseIfNeeded: 1,
                                GetRemoteServer: 1
                            },
                            success: function (vData) {
                                if (!IsMobile)
                                    mRemoteServer = CStr2(vData.RemoteServer);
                            }
                        });
                    }
                    
                    App.Deferred.fulfill('R87MBPXGSQ');
                }
            })(); // Init

            function SignOut() {

                AjaxSend({
                    url: "People/setSignOut",
                    success: function (vData) {
                        window.open(GetPathFormat("/SignIn?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                    }
                });

            }; // SignOut

            function InitFromQueryString() {
                
                var pMemoID;
                var pParameters;

                pParameters = GetUrlParameters();
                pMemoID = pParameters.SalesReturnMemoID;
                
                if (IsGuid(pMemoID)) {
                    proj.SalesReturnMemoID = pMemoID;
                    LoadStocks();
                    LoadCart();
                    return true;
                } else
                    $('#butCheckOut').closest('.ui-btn').show();
                
            } // END InitFromQueryString

            function InitFowst() {

                //Identify FOWST
                var pData = {FilterFowst: 1};
                if (IsMobile)
                    pData.DeviceSerial = Web.DeviceSerial();
                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: pData,
                    success: function (vFowstData) {

                        if (vFowstData.Results.Rows.length == 1) {
                            sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                            $('#lnkPosBranch').html(HtmlEncode('Branch: ' + vFowstData.Results.Rows[0].FowstName));
                            $('.lblPosBranch').text(vFowstData.Results.Rows[0].FowstName);

                            InitUser();
                        } else {
                            //BrowseAddresses();
                            var lastIndex = vFowstData.Results.Rows.length - 1;
                            sessionStorage.FowstID = vFowstData.Results.Rows[lastIndex].ID;
                            $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + vFowstData.Results.Rows[lastIndex].FowstName));
                            $('.lblPosBranch').text(vFowstData.Results.Rows[lastIndex].FowstName);
                        }
                    }
                }); // END AjaxSend

            }; // END InitFowst

            function InitUser() {
                var pParameters = GetUrlParameters();
                mUseOfflineData = CBool(pParameters.UseOfflineData);
                if (!mUseOfflineData)
                    GetUser();
                else
                    BrowseAddresses();

                $('#divPosBranch').hide();
                
                if (!InAndroid)
                    $('#butBarCode').closest('.ui-btn').hide();

                // start tab
                // http://stackoverflow.com/a/6850059/334045
                $(document).delegate('.ui-navbar ul li > a', 'click', function () {

                    //un-highlight and highlight only the buttons in the same navbar widget
                    $(this).closest('.ui-navbar').find('a').removeClass('ui-navbar-btn-active');

                    //this bit is the same, you could chain it off of the last call by using two `.end()`s
                    $(this).addClass('ui-navbar-btn-active');

                    //this starts the same but then only selects the sibling `.content_div` elements to hide rather than all in the DOM
                    $('#' + $(this).attr('data-href')).show().siblings('.content_div').hide();
                });
                $('.ui-navbar ul li > a:eq(0)').click();

                if (!IsMobile) {
                    AjaxSend({
                        url: 'SalesReturns/GetDeviceID',
                        data: {
                            UUID: localStorage.UUID
                        }, success: function (e) {
                            browserDeviceID = e.Results;
                        }
                    })
                }                
                // end tab
            }


            function GetUser() {

                AjaxSend({
                    url: 'Login3/GetUser',
                    success: function (vData) {
                        if (vData.Error != undefined)
                            MessageBox(vData.Error);
                        else
                            LoadUser(vData);
                    },
                    error: function (vError) {
                        if (IsMobile)
                            proj.PromptRequiresInternet({
                                Retry: GetUser
                            });
                        else {
                            console.log('Error getting Login/GetUser. Retrying...');
                            GetUser();
                        };
                    }
                });

            }

            function LoadUser(vData) {

                var pUserMenu;
                var pLink;

                if (vData.FirstName != undefined) {
                    pUserMenu = $('#divUserMenu');
                    pUserMenu.html('');

                    pLink = $('<a>')
                        .attr('href', '#')
                        .css('text-decoration', 'none')
                        .html('Hi, ' + HtmlEncode(vData.FirstName) + '&#x25BC;')
                        .click(function (e) {
                            e.preventDefault();
                            $("#divPeopleMenuPopUp").popup("open");
                        });
                    pUserMenu.append(pLink);

                    proj.CustomerID = vData.PosSubscriberCustomerID;
                    $('#divPosBranch').show();
                    if (InitFromQueryString())  {
                        // just do nothing
                    } else if (CStr2(vData.FowstID).length > 0) {
                        sessionStorage.FowstID = vData.FowstID;
                        $('#lnkPosBranch').html(HtmlEncode('Branch: ' + vData.FowstName));
                        $('.lblPosBranch').text(vData.FowstName);
                    } else  {
                        if (sessionStorage.FowstID == undefined) {
                            InitFowst();
                        }
                    }
                } // END if (vData.FirstName != undefined) {

                if (CDec2(vData.IsSalesAgent) != 1)
                    $('#divCompany').hide();

            }; // LoadUser

            function BrowseAddresses() {

                            //if (CStr2(GetStringObject('data.DeviceSerial', vParams)).trim() == '')
                if (IsMobile)
                    Browser.LibraryFowst.InputData.DeviceSerial = Web.DeviceSerial();
                Browser.OfflineMode = mUseOfflineData;
                BrowsePopup('LibraryFowst', 'Select FOWST', [
                    {
                        DisplayText: [{ Column: 'FowstName' }],
                        AutoSearch: true,
                        Offline: mUseOfflineData,
                        Click: function(e) {
                            sessionStorage.FowstID = e.ID;
                            $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + e.FowstName));
                            $('.lblPosBranch').text(e.FowstName);
                            //                AjaxSend({
                            //                    url: 'SalesMenu/SetPosCustomerAddressID',
                            //                    data: {CustomerAddressID: localStorage.CustomerAddressID},
                            //                    success: function (vData) {
                            //                        if ((vData) && (vData.Result == 'ok')) {
                            //                            $('#lnkPosBranch').html(HtmlEncode('Branch: ' + e.BranchName) + '&#x25BC;');
                            //                        } else if (vData.Error)
                            //                            MessageBox(vData.Error);
                            //                        else    
                            //                            MessageBox('An error ocurred while setting FOWST. Please try again');
                            //                    }
                            //                });
                        }
                    }
                ], {
                    OrganizationID: sessionStorage.OrgID,
                    FilterFowst: 1,
                    DeviceSerial: (IsMobile ? Web.DeviceSerial() : '')
                });
            };

            $('#txtSearch').keypress(function(e){
                if(e.which == 13){
                    if (mUseOfflineData)
                        BrowseOfflineInvoices();
                    else      
                        BrowseOnlineInvoices();
                    //NewMessageBox('Where do want to search for invoices?', 'Choose source', [
                    //    {
                    //        Text: 'Browse Offline Invoices',
                    //        Click: function() {
                    //            BrowseOfflineInvoices();
                    //        }
                    //    },
                    //    {
                    //        Text: 'Browse Online Invoices',
                    //        Click: function() {
                    //            BrowseOnlineInvoices();
                    //        }
                    //    }
                    //]);
                    
                 
                    return false;
                }
            })

            function BrowseOnlineInvoices() {

                if (!IsGuid(sessionStorage.FowstID)) {
                    NewMessageBox('Please select a FOWST first.', 'No Selected FOWST');
                    return;
                }

                BrowsePopup('TransInvoicesReturnables', 'Sales Invoices', [
                    {
                        Offline: false,
                        Search: $('#txtSearch').val(),
                        Message: mAllowOfflineSrvPayments ? 'Due to the ability to generate and consume offline SRVs, invoices shown here are limited to those that have been directly transacted in your specific POS machine+Business Location Data' : '',
                        RemoteServer: mRemoteServer,
                        //BeforeAjaxSend: function(vOrigParams, vAjaxData) {
                        //    if (vAjaxData.OfflineOnline == 'online')
                        //        vOrigParams.Server = mRemoteServer;
                        //    else
                        //        vOrigParams.Server = '';
                        //},
                        DisplayText: [
                            {
                                Label: 'Invoice No.',
                                //Column: 'SearchSO'
                                Column: 'InvoiceNum'
                            },
                            {
                                Label: 'Invoice Date',
                                Column: function (e) {
                                    return $.format.date(new Date(e.InvoiceDate), 'MMM dd, yyyy hh:mm a');
                                }
                            },
                            {
                                Label: 'Customer',
                                Column: 'CompanyName'
                            },
                            {
                                Label: 'FOWST',
                                Column: 'FOWSTName'
                            },
                            {
                                Label: 'Device',
                                Column: 'DeviceName'
                            },
                            {
                                Label: 'Total Value',
                                Column: function (e) {
                                    return AccountingFormat(e.Total);
                                }
                            }
                        ],
                        Click: function (e, vIsResultFromBarcodeScanner, vRemoteServerMode) {
                            proj.SalesReturnFromOffline = false;
                            if (IsMobile)
                                HandleInvoiceDownloadJson(e.ID);
                            else if (!CBool(vRemoteServerMode))
                                LoadStocks(e.ID);
                            else
                                HandleInvoiceDownload(e.ID);
                                
                        },
                        //NoRecordCallback: function() {
                        //    if (vRemoteServer != null)
                        //        return;

                        //    if (mRemoteServer == null) {
                        //        AjaxSend({
                        //            url: '/SalesMenu/GetCustomer',
                        //            data: {
                        //                GetOrAddDefaultAddresses: 1,
                        //                CreateCacheDatabaseIfNeeded: 1,
                        //                GetRemoteServer: 1
                        //            },
                        //            success: function(vData) {
                        //                mRemoteServer = CStr2(vData.RemoteServer);
                        //                if (mRemoteServer.trim() != '')
                        //                    PromptServerSearchIfNeeded();
                        //            }
                        //        });
                        //    } else if (mRemoteServer.trim() != '')
                        //        PromptServerSearchIfNeeded();

                        //    function PromptServerSearchIfNeeded() {
                        //        NewMessageBox('The invoice you are looking may not be on your local server. Do you want to search the remote server?', 'Remote Server?', [
                        //            {
                        //                Text: 'Yes. Search in the remote server',
                        //                Click: function() {
                        //                    BrowseOnlineInvoices(mRemoteServer);
                        //                }
                        //            }
                        //        ]);
                        //    }
                        //} // END NoRecordCallback: function() {
                    }
                ], {
                    FastMode: 1,
                    DateFrom: FormatDate($('#btnDateFrom').text()),
                    DateTo: FormatDate($('#btnDateTo').text()),
                    DeviceID: (IsMobile ? Web.DeviceID() : browserDeviceID)
                });
            }

            function HandleInvoiceDownload(vInvoiceID) {
                NewMessageBox('Downloading invoice data from remote server. Please wait.', 'Getting Data');
                var pUrl = 'SalesReturns/ExportInvoiceToXmlZip';
                pUrl = MergeUrlParameters({}, pUrl, mRemoteServer);
                AjaxSend({
                    url: pUrl,
                    data: {
                        InvoiceID: vInvoiceID  
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Results == 'ok')
                            DownloadXmlZip();
                        else
                            NewMessageBox('Unexpected result. ref: jsZH3B2NDFQ7!');
                    }
                })

                function DownloadXmlZip() {
                    AjaxSend({
                        url: 'SalesReturns/DownloadInvoiceXmlZip',
                        data: {
                            Server: mRemoteServer,
                            InvoiceID: vInvoiceID
                        },
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Results == 'ok') {
                                LoadStocks(vInvoiceID);
                                NewMessageBox('Invoice data was successfully loaded.', 'Success');
                            } else
                                NewMessageBox('Unexpected result. ref: jsNYKY3FEY7J!');
                        }
                    });
                } // END function DownloadXmlZip() {
            } // END HandleInvoiceDownload

            function HandleInvoiceDownloadJson(vInvoiceID) {
                NewMessageBox('Downloading invoice data from remote server. Please wait.', 'Getting Data');
                var pUrl = '/SalesReturns/ExportInvoiceToXmlZip';
                AjaxSend({
                    url: pUrl,
                    data: {
                        InvoiceID: vInvoiceID  
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Results == 'ok')
                            DownloadXmlZipAsJson();
                        else
                            NewMessageBox('Unexpected result. ref: jsDYHC29JHTZ!');
                    }
                })

                function DownloadXmlZipAsJson() {
                    AjaxSend({
                        url: '/SalesReturns/GetInvoiceXmlZipAsJson',
                        data: {
                            InvoiceID: vInvoiceID
                        },
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Results == 'ok') {
                                ImportInvoiceFromDataSet(vData);
                                LoadStocks(vInvoiceID);
                                NewMessageBox('Invoice data was successfully loaded.', 'Success');
                            } else
                                NewMessageBox('Unexpected result. ref: jsNYKY3FEY7J!');
                        }
                    });
                } // END function DownloadXmlZip() {
            } // END HandleInvoiceDownload

            function ImportInvoiceFromDataSet(vDS) {
                for (var pDtName in vDS) {
                    if (vDS.hasOwnProperty(pDtName) == false)
                        continue;
                    var pDT = vDS[pDtName];
                    if (pDT.Rows == null)
                        continue;
                    for (var pDrIndex = 0; pDrIndex <= pDT.Rows.length - 1; pDrIndex++) {
                        var pDR = pDT.Rows[pDrIndex];
                        var pSql = 'SELECT COUNT(*)\n' +
                            ' FROM ' + pDtName + '\n' +
                            ' WHERE ID = ' + ReadyString(pDR.ID);
                        var pCount = Sql.ExecuteScalar(pSql);
                        if (CInt2(pCount) > 0)
                            continue;
                        var pIU = new InsertUpdate(pDtName);
                        var pHasDateImported = false;
                        pIU.GetItems(pDR, function(vArgs) {
                            if ((vArgs.FieldName.toLowerCase() == 'dateimported')
                                && (!vArgs.Cancel))
                                pHasDateImported = true;
                        });
                        if (!pHasDateImported
                            && pDR.hasOwnProperty('DateImported'))
                            pIU.Add('DateImported', proj.SqliteNow);
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);
                    }
                }
            }

            function BrowseOfflineInvoices() {

                if (!IsGuid(sessionStorage.FowstID)) {
                    NewMessageBox('Please select a FOWST first.', 'No Selected FOWST');
                    return;
                }

                BrowsePopup('TransInvoices', 'Sales Invoices', [
                    {
                        Offline: true,
                        AutoSearch: true,
                        Search: $('#txtSearch').val(),
                        TopText: 'Due to the ability to generate and consume offline SRVs, invoices shown here are limited to those that have been directly transacted in your specific POS machine+Business Location Data',
                        DisplayText: [
                            {
                                Label: 'Invoice No.',
                                //Column: 'SearchSO'
                                Column: function(vData) {
                                    // return 'SI ' + vData.MIN + '-' + vData.InvoiceNum;
                                    return 'SI ' + ('0000000' + CStr2(vData.InvoiceNum)).substr(-7);
                                }
                            },
                            {
                                Label: 'Customer',
                                Column: 'CompanyName'
                            },
                            {
                                Label: 'Date',
                                Column: function (e) {
                                    return $.format.date(new Date(e.InvoiceDate), 'MMM dd, yyyy hh:mm a');
                                }
                            },
                            {
                                Label: 'FOWST',
                                Column: 'FOWSTName'
                            },
                            {
                                Label: 'Device',
                                Column: 'DeviceName'
                            },
                            {
                                Label: 'Total Value',
                                Column: function (e) {
                                    return AccountingFormat(e.Total);
                                }
                            },
                            {
                                Label: 'SKU Count',
                                Column: 'SkuCount'
                            },
                            {
                                Label: 'Created By',
                                Column: 'EmployeeFullName'
                            }
                        ],
                        Click: function (e) {
                            proj.SalesReturnFromOffline = true;
                            proj.SalesReturnMemoID = undefined;
                            proj.SalesReturnInvoiceID = e.ID;
                            var pDS = Business.TransSalesReturn.GetContent({
                                InvoiceID: e.ID
                            });
                            ShowStocks(pDS);
                            LoadCart();
                        }
                    }
                    ], {
                        PoMode: false,
                        NonZero: true,
                        FOWSTID: sessionStorage.FowstID,
                        DeviceID: (IsMobile ? Web.DeviceID() : browserDeviceID)
                    }
                ); // END BrowsePopup('TransInvoices', 'Sales Invoices', [
            }

            $('#formSearch2').submit(function (e) {
                e.preventDefault();

                Browser.TransInvoicesReturnables.Click = function (e) {
                    LoadStocks(e.ID);
                };
                Browser.TransInvoicesReturnables.InputData.DateFrom = FormatDate($('#btnDateFrom').text()); //FormatDate($('#txtDateFrom').DateCombo('val'));
                Browser.TransInvoicesReturnables.InputData.DateTo = FormatDate($('#btnDateTo').text()); //FormatDate($('#txtDateTo').DateCombo('val'));
                Browser.TransInvoicesReturnables.Show($('#txtSearch').val());
                return false;
            });

            $('#formSearchCustomer').submit(function (e) {
                e.preventDefault();
                Browser.LibraryOrganization.Click = function (e) {
                    $('#txtSearchCustomer').val(e.CompanyName)
                    .data('id', e.ID);
                };
                Browser.LibraryOrganization.Show($('#txtSearchCustomer').val());
                return false;
            });

            $('#formSearchItem').submit(function (e) {
                var pCustomerID = $('#txtSearchCustomer').data('id');
                e.preventDefault();
                if (CStr2(pCustomerID).length > 0) {
                    AjaxSend({
                        url: 'SalesReturns/GetContents',
                        data: { CustomerID: pCustomerID,
                            Keywords: $('#txtSearchItem').val()
                        },
                        success: function (vData) {
                            ShowStocks(vData);
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });
                } else {
                    $('#txtSearchCustomer').focus();
                    MessageBox('Please choose a customer first.');
                };
                return false;
            });

            function LoadStocks(vInvoiceID, vServer) {

                var pUrl = '/SalesReturns/GetContents';
                pUrl = MergeUrlParameters({}, pUrl, vServer);
                AjaxSend({
                    url: pUrl,
                    data: { 
                        InvoiceID: vInvoiceID, // this takes precedence over MemoID
                        MemoID: proj.SalesReturnMemoID
                    },
                    success: function (vData) {
                        proj.RemoteServer = vServer;
                        proj.IsStockLoadedFromServer = CStr2(vServer).trim() != '';
                        mIsStockLoadedFromServer = (vServer != null);
                        proj.SalesReturnMemoID = undefined;
                        proj.SalesReturnInvoiceID = vInvoiceID;
                        ShowStocks(vData);
                        LoadCart();
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }; // function LoadStocks

            function ShowStocks(vData) {

                $('#divInvoiceContents > .invoice-row').remove(); 
                $('[data-href=Nav1a]').click();
                if (vData.Result == 'no data') {
                    if (mLvContents != undefined)
                        mLvContents.Element.hide();
                    $('#divInvoiceContents').hide();
                    $('#divInvoiceContentsEmpty').show();
                    return;
                };
                $('#txtSearch').val(CStr2(vData.Units.Rows[0].InvoiceNum));
                var pDivTemplate = $('#divReturnables').hide();
                var pTrTemplate = pDivTemplate.find('[name=trTemplate]').hide();

                pDivTemplate.hide();

                for (var i = 0; i < vData.Stocks.Rows.length; i++) {
                    var vDataRow = vData.Stocks.Rows[i], pItemTotal = 0;
                    var divrow =  $('<div class="bg-light border rounded mb-3 invoice-row">')
                                    .append($('<table style="width:100%">')
                                        .append($('<td class="align-middle p-1" style="width:100px;">').append($('<img style="width:100px;">')
                                            .attr('src', '/Content/images/box.png'))
                                        )
                                        .append($('<td class="align-middle p-1">')
                                            .append($('<div class="mb-3 font-weight-bold">').text(vDataRow.ProductNo + ' ' + vDataRow.ProductName))
                                            .append($('<div class="unitdetails">'))
                                            .append($('<div>').append('<b>Item Total: <span class="itemtotal"></span></b>'))
                                        )
                                        .append($('<td class="align-middle p-1" style="width:40px; text-align:center;">')
                                            .append($('<span class="bi-chevron-right">'))
                                        )
                                    )
                    for (var u = 0; u < vData.Units.Rows.length; u++) {
                        var pDrUnit = vData.Units.Rows[u];
                        if (vDataRow.ProductID == pDrUnit.ProductID) {
                            vDataRow.InvoiceID = pDrUnit.InvoiceID;
                            pDrUnit.SubTotal = CDec2(pDrUnit.Quantity) - CDec2(pDrUnit.QuantityReturned);
                            pDrUnit.SubTotal = pDrUnit.SubTotal * CDec2(pDrUnit.UnitPrice);

                            divrow.find('.unitdetails').append($('<div>').append($.format.number(CDec2(pDrUnit.Quantity), proj.QuantityFormat) + ' ' +
                                                pDrUnit.UnitMeasure + ' ' +
                                                AccountingFormat(pDrUnit.UnitPrice) + ' ' +
                                                AccountingFormat(pDrUnit.SubTotal) + ' ' +
                                                $.format.number(CDec2(pDrUnit.QuantityReturned), proj.QuantityFormat))
                                            )
                            pItemTotal += CDec2(pDrUnit.SubTotal);
                        }

                        divrow.data('data', vDataRow).find('.itemtotal').text(AccountingFormat(pItemTotal));
                    }

                    divrow.click(function () {
                        var pDR = $(this).data('data');

                        proj.ProductID = pDR.ProductID;
                        proj.SalesReturnInvoiceID = pDR.InvoiceID;
                        ShowStockPage();
                        if (proj.SalesReturnStock_PageShow != null) {
                            proj.RemoteServer = mRemoteServer;
                            proj.IsStockLoadedFromServer = mIsStockLoadedFromServer;
                            proj.SalesReturnStock_PageShow();
                        }
                    })

                    $('#divInvoiceContents').append(divrow);
                }

                $('#divInvoiceContentsEmpty, #divInvoiceContents > div:eq(1)').hide();
                $('#divInvoiceContents').show();

                return;

                if (mLvContents == undefined) {
                    mLvContents = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                    mLvContents.Element.find('ul')
                    .attr('data-split-icon', 'minus');
                    mLvContents.Element.appendTo('#divInvoiceContents');
                    mLvContents.OnItem = function (vDataRow, vItem, e) {

                        var pTR;
                        var pDiv;
                        var pDrUnit;
                        var pItemTotal = 0;

                        var pLnk = $('<a>')
                            .attr('href', GetPathFormat('/SalesReturns/Stock') + '?id=' + vDataRow.ProductID)
                        .data('data', vDataRow)
                        .click(function (e) {
                            e.preventDefault();
                            var pDR = $(this).data('data');
                            if (mDenied) {
                                MessageBox('This sales return request was denied.');
                                return;
                            }
                            if (mApproved) {
                                MessageBox('This sales return request was already approved.');
                                return;
                            }
                            proj.ProductID = pDR.ProductID;
                            proj.SalesReturnInvoiceID = pDR.InvoiceID;
                            ChangePage('SalesReturns/Stock');
                            if (proj.SalesReturnStock_PageShow != null) 
                                proj.SalesReturnStock_PageShow();
                            return false;
                        });
                        pDiv = pDivTemplate.clone()
                            .attr('id', '');
                        pDiv.find('[name=spanStockDescription]').text(vDataRow.ProductNo + ' ' + vDataRow.ProductName);

                        for (var p = 0; p <= mLvContents.vData.Units.Rows.length - 1; p++) {
                            pDrUnit = mLvContents.vData.Units.Rows[p];
                            if (pDrUnit.ProductID == vDataRow.ProductID) {
                                var pMasterData = pLnk.data('data')
                                pMasterData.InvoiceID = pDrUnit.InvoiceID;
                                pLnk.data('data', pMasterData);
                                pTR = pTrTemplate.clone();
                                pTR.data('data', pDrUnit);
                                pTR.find('[name=tdQty]').text($.format.number(CDec2(pDrUnit.Quantity), proj.QuantityFormat));
                                pTR.find('[name=tdUnit]').text(pDrUnit.UnitMeasure);
                                pTR.find('[name=tdPrice]').text(AccountingFormat(pDrUnit.UnitPrice));
                                pDrUnit.SubTotal = CDec2(pDrUnit.Quantity) - CDec2(pDrUnit.QuantityReturned);
                                pDrUnit.SubTotal = pDrUnit.SubTotal * CDec2(pDrUnit.UnitPrice);
                                pTR.find('[name=tdLineTotal]').text(AccountingFormat(pDrUnit.SubTotal));
                                pTR.find('[name=tdQuantityReturned]').text($.format.number(CDec2(pDrUnit.QuantityReturned), proj.QuantityFormat));
                                pTR.click(function (e) {
                                    var pDR = $(this).data('data');

                                    proj.ProductID = pDR.ProductID;
                                    proj.SalesReturnInvoiceID = pDR.InvoiceID;
                                    ChangePage('SalesReturns/Stock');
                                    if (proj.SalesReturnStock_PageShow != null) 
                                        proj.SalesReturnStock_PageShow();
                                    return false;
                                });
                                pDiv.find('[name=trTemplate]').parent().append(pTR);
                                pTR.show();
                                pItemTotal += CDec2(pDrUnit.SubTotal);
                            };
                        }; // for (var p = 0; p <= vData.Units.length - 1; p++) {
                        pDiv.find('[name=spanTotal]').text(AccountingFormat(pItemTotal));
                        mLvContents.OnItem.Total += pItemTotal;

                        pLnk.append($('<img>')
                        .attr('src', '/Content/images/box.png'));
                        pLnk.append(pDiv);
                        pDiv.show();
                        vItem.append(pLnk);
                        //                        .append($('<a>')
                        //                            .attr('href', '#')
                        //                            .data('data', vDataRow)
                        //                            .data('li', vItem)
                        //                            .click(function(e) {
                        //                                var pDR = $(this).data('data');
                        //                                var pLI = $(this).data('li');

                        //                                proj.ProductID = pDR.ProductID;
                        //                                proj.SalesReturnInvoiceID = pDR.InvoiceID;
                        //                                ChangePage(GetWindowPathFormat('StockReturns/Stock'));
                        //                                return false;
                        //                            })
                        //                            .text('Remove')
                        //                        ); // .append($('<a>')

                    }; // mLvContents.OnItem
                }; // if (mLvContents == undefined) {
                mLvContents.vData = vData;
                mLvContents.OnItem.Total = 0;
                mLvContents.SetData(vData.Stocks.Rows);

                //                $('#spanCartTotal').text(AccountingFormat(mLvCart.OnItem.CartTotal));
                $('#divInvoiceContentsEmpty').hide();
                $('#divInvoiceContents').show();
                //                if (window.location.hash.substring(1).toLowerCase() == 'cart') 
                //                    $('[data-href=Nav1d]').click();

            }; // function ShowStocks

            var htmlstock = '';
            function ShowStockPage() {
                if (htmlstock == '') {
                    AjaxSend({
                        ExactUrl: '/SalesReturns/BsStock?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlstock = vData;
                            $('div[data-role="page"]:first').append(vData);
                            $('#pageMain').pageshow();
                        }
                    });
                } else {
                    $('#pageMain').pageshow();
                }
            }

            function LoadCart() {

                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pMemoID;
                    if (CStr2(proj.SalesReturnMemoID).length == 0)
                        pMemoID = Business.TransSalesReturn.GetCartMemoByInvoiceID(proj.SalesReturnInvoiceID);
                    else
                        pMemoID = proj.SalesReturnMemoID;
                    var pCartData = Business.TransSalesReturn.GetStocks(pMemoID);
                    ShowCart(pCartData);
                } else 
                    AjaxSend({
                        url: 'SalesReturns/Cart',
                        data: { InvoiceID: proj.SalesReturnInvoiceID,
                            MemoID: proj.SalesReturnMemoID
                        },
                        success: function (vData) {
                            ShowCart(vData);
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                function ShowCart(vData) {

                    if (vData == undefined) return;
                    if ((vData.Stocks == undefined)
                        || (vData.Stocks.Rows.length == 0)) {

                        if (mLvCart != undefined)
                            mLvCart.Element.hide();
                        $('#divCartSection').hide();
                        $('#divCartEmpty').show();
                        return;
                    }

                    (function SetInvoiceNumber() {
                        var pDRs;
                        var pInvoiceNums = '';

                        if (vData.Units == undefined) return;
                        if (vData.Units.Rows == undefined) return;

                        pDRs = GetDistinct(vData.Units.Rows, ['InvoiceNum']);
                        for (var p = 0; p <= pDRs.length - 1; p++) {
                            if (pInvoiceNums != '')
                                pInvoiceNums += ', ';
                            pInvoiceNums += CStr2(pDRs[p].InvoiceNum);
                        }
                        if (pInvoiceNums != '')
                            $('#txtSearch').val(pInvoiceNums);
                    })(); // END SetInvoiceNumber
    
                    var pTitle;

                    var pCheckedOut;

                     if (vData.Memo != null) {
                        mApproved = (vData.Memo.ApprovedBy != null);
                        mDenied = (vData.Memo.DeniedBy != null)

                        if (vData.Memo.FowstName != null) {
                            $('#lnkPosBranch').text(vData.Memo.FowstName);
                            $('.lblPosBranch').text(vData.Memo.FowstName);
                            sessionStorage.FowstID = vData.Memo.FowstID;
                        }
                        if (vData.Memo.CheckOutMethod != null) {
                            pCheckedOut= true;
                            if (vData.Memo.CheckOutMethod == 'Refund') {
                                pTitle = 'Sales Returns (Refund)';
                                mIsRefund = vData.Memo.CheckOutMethod == 'Refund';
                            } else
                                pTitle = 'Sales Returns (Merchandise Exchange)';
                            if (mDenied) // 'denied' takes precendence
                                pTitle += ' Disapproved';
                            else if (mApproved)
                                pTitle += ' Approved';
                            else
                                pTitle += ' Not yet approved';
                            $('#divTitle').text(pTitle);
                        }

                        if (mApproved) {
                            // hide a button http://stackoverflow.com/a/7054037
                            $('#butCheckOut').closest('.ui-btn').hide();
                            $('#butPrint').closest('.ui-btn').show();
                            $('#txtSearch').attr('readonly', 1);
                        }
                    }

                    if (mDenied) {
                        $('#butCheckOut').closest('.ui-btn').hide();
                        $('#butPrint').closest('.ui-btn').hide();
                    } else if (mApproved)
                        $('#butCheckOut').closest('.ui-btn').hide();
                    else
                        $('#butCheckOut').closest('.ui-btn').show();
                    if (mApproved || mDenied) {
                        $('#lnkPosBranch').unbind('click');
                    } else if (pCheckedOut) {
                        $('#butApprove').closest('.ui-btn').show();
                        $('#butDeny').closest('.ui-btn').show();
                    }

                    proj.SalesReturnMemoID = vData.MemoID;
                    var pDivTemplate = $('#divCartStock').hide();
                    var pTrTemplate = pDivTemplate.find('[name=trTemplate]').hide();
                    var carttotal = 0;

                    pDivTemplate.hide();
                    $('#divCartList').empty();

                    for (var i = 0; i < vData.Stocks.Rows.length; i++) {
                        var vDataRow = vData.Stocks.Rows[i], pItemTotal = 0;
                        var divrow = $('<div class="bg-light border rounded mb-3">')
                                    .append($('<table style="width:100%">')
                                        .append($('<td class="align-middle p-1" style="width:100px;">').append($('<img style="width:100px;">')
                                            .attr('src', '/Content/images/box.png'))
                                        )
                                        .append($('<td class="align-middle p-1">')
                                            .append($('<div class="mb-3 font-weight-bold">').text(vDataRow.ProductNo + ' ' + vDataRow.ProductName))
                                            .append($('<div class="unitdetails">'))
                                            .append($('<div>').append('<b>Item Total: <span class="itemtotal"></span></b>'))
                                        )
                                        .append($('<td class="align-middle p-1 bg-danger" style="width:40px; text-align:center;">')
                                            .data('data', vDataRow)
                                            .append($('<span class="bi-trash text-white">'))
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                var data = $(this).data('data');
                                                console.log(data);
                                            })
                                        )
                                    )
                        for (var u = 0; u < vData.Units.Rows.length; u++) {
                            var pDrUnit = vData.Units.Rows[u];
                            if (vDataRow.ProductID == pDrUnit.ProductID) {
                                divrow.find('.unitdetails').append($('<div>').append($.format.number(CDec2(pDrUnit.QuantityReturned), proj.QuantityFormat) + ' ' +
                                                    pDrUnit.UnitMeasure + ' ' +
                                                    AccountingFormat(pDrUnit.UnitPrice) + ' ' +
                                                    AccountingFormat(pDrUnit.TotalAmount))
                                                )
                                pItemTotal += CDec2(pDrUnit.TotalAmount);
                                carttotal += pItemTotal;
                            }

                            divrow.data('data', vDataRow).find('.itemtotal').text(AccountingFormat(pItemTotal));
                        }

                        divrow.click(function () {
                            var pDR = $(this).data('data');

                            proj.ProductID = pDR.ProductID;
                            proj.SalesReturnInvoiceID = pDR.InvoiceID;
                            ShowStockPage();
                            if (proj.SalesReturnStock_PageShow != null)
                                proj.SalesReturnStock_PageShow();
                        })

                        $('#divCartList').append(divrow);
                    }

                    $('#spanCartTotal').text(AccountingFormat(carttotal));
                    $('#divCartEmpty').hide();
                    $('#divCartSection').show();

                    return;

                    if (mLvCart == undefined) {
                        mLvCart = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                        mLvCart.Element.find('ul')
                            .attr('data-split-icon', 'minus');
                        mLvCart.Element.insertAfter(pDivTemplate);
                        mLvCart.OnItem = function (vDataRow, vItem, e) {

                            var pTR;
                            var pDiv;
                            var pDrUnit;
                            var pItemTotal = 0;

                            var pLnk = $('<a>')
                                .attr('href', 'SalesReturns/Stock?id=' + vDataRow.ProductID)
                                .data('data', vDataRow)
                                .click(function (e) {
                                    e.preventDefault();
                                    if (mDenied) {
                                        MessageBox('This sales return request was denied.');
                                        return;
                                    }
                                    if (mApproved) {
                                        MessageBox('This sales return request was already approved.');
                                        return;
                                    }

                                    var pData = $(this).data('data');
                                    proj.ProductID = pData.ProductID;
                                    proj.SalesReturnInvoiceID = pData.InvoiceID;
                                    ChangePage('SalesReturns/Stock');
                                    if (proj.SalesReturnStock_PageShow != null) 
                                        proj.SalesReturnStock_PageShow();

                                    return false;
                                });
                            pDiv = pDivTemplate.clone()
                                .attr('id', '');
                            pDiv.find('[name=spanStockDescription]').text(vDataRow.ProductNo + ' ' + vDataRow.ProductName);

                            for (var p = 0; p <= mLvCart.vData.Units.Rows.length - 1; p++) {
                                pDrUnit = mLvCart.vData.Units.Rows[p];
                                if (pDrUnit.ProductID == vDataRow.ProductID) {
                                    pTR = pTrTemplate.clone();
                                    pTR.find('[name=tdQty]').text($.format.number(CDec2(pDrUnit.QuantityReturned), proj.QuantityFormat));
                                    pTR.find('[name=tdUnit]').text(pDrUnit.UnitMeasure);
                                    pTR.find('[name=tdPrice]').text(AccountingFormat(pDrUnit.UnitPrice));
                                    pTR.find('[name=tdLineTotal]').text(AccountingFormat(pDrUnit.TotalAmount));
                                    pDiv.find('[name=trTemplate]').parent().append(pTR);
                                    pTR.show();
                                    pItemTotal += CDec2(pDrUnit.TotalAmount);
                                }
                            }; // for (var p = 0; p <= vData.Units.length - 1; p++) {
                            pDiv.find('[name=spanTotal]').text(AccountingFormat(pItemTotal));
                            mLvCart.OnItem.CartTotal += pItemTotal;

                            pLnk.append($('<img>')
                                .attr('src', 'Content/images/box.png'));
                            pLnk.append(pDiv);
                            pDiv.show();
                            vItem.append(pLnk)
                                .append($('<a>')
                                    .attr('href', '#')
                                    .data('data', vDataRow)
                                    .data('li', vItem)
                                    .click(function (e) {
                                        e.preventDefault();

                                        if (mDenied) {
                                            MessageBox('This sales return request was denied.');
                                            return;
                                        }
                                        if (mApproved) {
                                            MessageBox('This sales return request was already approved.');
                                            return;
                                        }
                                        var pDR = $(this).data('data');

                                        MessageBox('Are you sure you want to remove ' + pDR.ProductNo + ' ' + pDR.ProductName + ' from your returns cart?',
                                            'Remove',
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {
                                                        AjaxSend({
                                                            url: 'SalesReturns/DeleteStock',
                                                            data: {
                                                                InvoiceID: proj.SalesReturnInvoiceID,
                                                                StockID: pDR.ProductID,
                                                                MemoID: proj.SalesReturnMemoID
                                                            },
                                                            success: function (vData) {
                                                                history.back();
                                                                if (vData.Result == 'ok') {
                                                                    if (vData.NewMemo == '1')
                                                                        proj.SalesReturnMemoID = undefined;
                                                                    LoadCart();
                                                                } else if (vData.Error) {
                                                                    MessageBox(vData.Error);
                                                                };
                                                            },
                                                            error: function (vError) {
                                                                var pDummy;
                                                            }
                                                        });
                                                    }
                                                }
                                            ,
                                                { Text: 'No' }
                                            ]);
                                        return false;
                                    })
                                    .text('Remove')
                                ); // .append($('<a>')

                        }; // mLvCart.OnItem
                    };  // if (mLvCart == undefined) {
                    mLvCart.vData = vData;
                    mLvCart.OnItem.CartTotal = 0;
                    mLvCart.SetData(vData.Stocks.Rows);

                    $('#spanCartTotal').text(AccountingFormat(mLvCart.OnItem.CartTotal));
                    $('#divCartEmpty').hide();
                    $('#divCartSection').show();
                    // if (window.location.hash.substring(1).toLowerCase() == 'cart') 
                    $('[data-href=Nav1b]').click();

                }; // function ShowCart(vData) {

            }; // function LoadCart(vData) {

            proj.ShowReturns = function () {
                LoadCart();
                $('#pageSalesReturn').pageshow();
                $('[data-target="#Nav1b"]').tab('show');
            }; // proj.ShowCart = function() {

            $('#butCheckOutHeader').click(function () {
                //IminPrintPOSsalesRefundReceipt('4ec40703-76fe-42ab-a7eb-a106109dc6ff')
                if (CStr2(proj.SalesReturnMemoID).length == 0) {
                    MessageBox('Please return at least 1 item');
                    return;
                };

                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    mCheckOutVerify = {
                        OtherPayments: []
                    };
                    var pStocks = Business.TransSalesReturn.GetStocks(proj.SalesReturnMemoID);
                    mCheckOutVerify.TotalAmount = pStocks.TotalAmount;
                    mCheckOutVerify.SkuCount = pStocks.SkuCount;
                    var pDtOtherPayements = Business.TransInvoice.GetOtherPayments(proj.SalesReturnInvoiceID);
                    var pOtherPaymentTypes = '';
                    var pRemainingValues = mCheckOutVerify.TotalAmount;
                    for (var pOtherPaymentIndex = 0; pOtherPaymentIndex <= pDtOtherPayements.Rows.length - 1; pOtherPaymentIndex++) {
                        if (pRemainingValues <= 0)
                            break;
                        var pDrOtherPaymentType = pDtOtherPayements.Rows[pOtherPaymentIndex];
                        var pActualValue;
                        if (pDrOtherPaymentType.Amount <= pRemainingValues) {
                            pActualValue = pDrOtherPaymentType.Amount;
                        } else {
                            pActualValue = pRemainingValues;
                        }
                        mCheckOutVerify.OtherPayments.push({
                            PaymentType: pDrOtherPaymentType.PaymentType,
                            Amount: pActualValue
                        });
                        pRemainingValues -= pActualValue;
                        pOtherPaymentTypes += CStr2(pDrOtherPaymentType.PaymentType) + ' = ' + AccountingFormat(pActualValue) + '; ';
                    }
                    console.log(mCheckOutVerify)
                    //return
                    var pMsg = 'You are about to check out this sales return cart. Please verify the following info: Total Returned Amount: ' + $.format.number(CDec2(mCheckOutVerify.TotalAmount), '#,###.00') + '; SKU Count: ' + mCheckOutVerify.SkuCount + '. ';
                    if (pOtherPaymentTypes != '')
                        pMsg += ' In case of return, please the following values to the customer: ' + pOtherPaymentTypes;
                    $('#divCheckOutMessage').text(pMsg);
                    $('#divCheckOutResultMessage').hide().text('');
                    $('#divCheckOutPrintMessage').hide().text('');
                    $('#divCheckOut').pageshow();
                } else 
                    AjaxSend({
                        url: 'SalesReturns/Verify',
                        data: { MemoID: proj.SalesReturnMemoID },
                        success: function (vData) {
                            if ((vData) && (vData.Result == 'ok')) {
                                mCheckOutVerify = vData;
                                $('#divCheckOutMessage').html('You are about to check out this sales return cart. Please verify the following info: <br><br> Total Returned Amount: ' + $.format.number(CDec2(vData.TotalAmount), '#,###.00') + ' <br> SKU Count: ' + vData.SkuCount);
                                $('#divCheckOutResultMessage').hide().text('');
                                $('#divCheckOutPrintMessage').hide().text('');
                                $('#divCheckOut').pageshow();
                            } else
                                MessageBox(vData.Error);
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    }); // AjaxSend

            })

            $('#butCheckOut').click(function () {
                
                if (CStr2(proj.SalesReturnMemoID).length == 0) {
                    MessageBox('Please return at least 1 item');
                    return;
                };

                AjaxSend({
                    url: 'SalesReturns/Verify',
                    data: { MemoID: proj.SalesReturnMemoID },
                    success: function (vData) {
                        if ((vData) && (vData.Result == 'ok')) {
                            mCheckOutVerify = vData;
                            $('#divCheckOutMessage').text('You are about to check out this sales return cart. Please verify the following info: Total Returned Amount: ' + $.format.number(CDec2(vData.TotalAmount), '#,###.00') + '; SKU Count: ' + vData.SkuCount);
                            $('#divCheckOutResultMessage').hide().text('');
                            $('#divCheckOutPrintMessage').hide().text('');
                            $('#divCheckOut').pageshow();
                        } else
                            MessageBox(vData.Error);
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend

            })
            .closest('.ui-btn').hide(); // $('#butCheckOut').click(function() {

            $('#butPrint').click(Print).closest('.ui-btn').hide();

            $('#butApprove').click(function(e) {
                Approve({
                    MemoID: proj.SalesReturnMemoID,
                    Approve: true
                });
            }).closest('.ui-btn').hide();

            $('#butDeny').click(function(e) {
                Approve({
                    MemoID: proj.SalesReturnMemoID, 
                    Approve: false
                });
            }).closest('.ui-btn').hide();

            // BEGIN if this function is modified, update change to all copies
            // js7DVC6QTAUU!
            function Approve(vParams) {
                /*
                vParams =  {
                    MemoID: 'string-guid',
                    Approve: boolean,
                    Username: 'optional',
                    Password: 'optional',
                    success: function() {} // optional callback
                }
                */
                var pParams = vParams;

                AjaxSend({
                    url: '/SalesReturns/Approve',
                    data: {
                        SalesReturnMemoID: vParams.MemoID,
                        Approve: CBool(vParams.Approve) ? 1 : 0,
                        Username: vParams.Username,
                        Password: vParams.Password
                    },
                    success: function(vData) {
                        if (vData.Error) {
                            MessageBox(vData.Error);
                            return;
                        } else if (vData.Result == 'approved') {
                            SaveToInventoryLedger()
                            MessageBox('Request was successfully approved.');
                            if ($.isFunction(vParams.success))
                                vParams.success();
                            UpdateSalesTextFile();
                        } else if (vData.Result == 'denied') {
                            MessageBox('Request was successfully disapproved.');
                            if ($.isFunction(vParams.success))
                                vParams.success();
                        } else if (vData.NoAccess) {
                            PromptUserAccess('[Approve Request]', {
                                OtherData: {
                                    FowstID: sessionStorage.FowstID
                                },
                                PromptOnly: true,
                                PromptCallback: function(vData) {
                                    Approve($.extend(vParams, vData));
                                }
                            });
                        }
                    }
                });
            } // END function Approve() {
            // END if this function is modified, update change to all copies

            function Clear() {

                if (mLvContents != undefined)
                    mLvContents.Element.hide();
                $('#divInvoiceContents').hide();
                $('#divInvoiceContentsEmpty').show();

                proj.SalesReturnMemoID = undefined;
                proj.SalesReturnInvoiceID = undefined;

                if (mLvCart != undefined)
                    mLvCart.Element.hide();
                $('#divCartSection').hide();
                $('#divCartEmpty').show();

                $('#txtSearch').val('');
                $('#pageSalesReturn').pageshow();


            }; // function Clear() {

            $('[name=butCheckOutNow]').click(function () {

                if (!IsGuid(sessionStorage.FowstID)) {
                    NewMessageBox('Please select a FOWST first.', 'No Selected FOWST');
                    return;
                }

                var pIsRefund = $(this).attr('id') == 'butCheckOutRefund' ? 1 : 0;
                //if (pIsRefund && mUseOfflineData) {
                //    NewMessageBox('Refund is not yet implemented while using offline data. To issue a refund, this device must be connected to Mobiz server.', 'Refund not available');
                //    return;
                //}

                if (!CBool(pIsRefund)) {
                    PromptUserAccess('Allow Merchandise Exchange', {
                        OnClickOKSuccess: function () {
                            ContinueAfterCheckingUserAccess();
                        }
                    });
                } else {
                    PromptUserAccess('Allow Sales Refund', {
                        OnClickOKSuccess: function () {
                            ContinueAfterCheckingUserAccess();
                        }
                    });
                }
                function ContinueAfterCheckingUserAccess() {
                    $('[name=butCheckOutNow]').hide();

                    var pData = {
                        TotalAmount: mCheckOutVerify.TotalAmount,
                        SkuCount: mCheckOutVerify.SkuCount, 
                        OtherPayments: mCheckOutVerify.OtherPayments,
                        Refund: pIsRefund
                    };
                    if (IsMobile)
                        pData.DeviceSerial = Device.Serial();

                    mIsRefund = pIsRefund;
                    //if (proj.SalesReturnFromOffline || CBool(mIsRefund)) {
                    if (IsMobile) {
                        $('[name=butCheckOutNow]').show();
                        //proj.SalesReturnFromOffline = true;
                        Business.TransSalesReturn.Approve(proj.SalesReturnMemoID, pIsRefund);
                        Business.TransSalesReturn.SaveToSalesReturnLedger(proj.SalesReturnMemoID, pIsRefund, mCheckOutVerify);
                        UpdateSalesTextFile();
                        UpdateZReadCache();
                        $('[name=butCheckOutNow]').hide();
                        Print();
                    } else 
                        CheckOut(pData);
                }

                function CheckOut(vData) {

                    mIsRefund = vData.Refund;
                    vData.MemoID = proj.SalesReturnMemoID;
                    vData.UUID = CStr2(localStorage.getItem('UUID'));

                    mLatestCheckOutTime = new Date();
                    AjaxSend({
                        url: 'SalesReturns/CheckOut',
                        data: vData,
                        success: function (vData) {
                            //$('[name=butCheckOutNow]').show();
                            if ((vData) && (vData.Result == 'ok')) {
                                try {                               
                                    if ((CStr2(vData.SrvNum) != '')
                                        && (CStr2(vData.Pin) != ''))
                                        $('#divCheckOutResultMessage').html('SRV Num: ' + vData.SrvNum + '<br />' +
                                            'Pin: ' + vData.Pin).show();
                                
                                    if (CBool(vData.SalesReturnRequiresApproval)) {
                                        $('#spanMemoNum').text(vData.Num);
                                        $('#divSalesReturnRequiresApproval').pageshow();
                                        setTimeout(GetApproval, 10000);
                                        return;
                                    } else {
                                        if (IsMobile) {
                                            var pSeriesFieldName;
                                            if (vData.Refund)
                                                pSeriesFieldName = 'POSLastRefNo';
                                            else
                                                pSeriesFieldName = 'POSLastSRVNo';
                                            Business.TransInvoice.UpdateCounter(vData.Num, {
                                                FieldName: pSeriesFieldName,
                                                InvoiceFieldName: 'dont use',
                                                DontUseTableCounter: true
                                            });
                                            UpdateSalesTextFile();
                                            UpdateZReadCache();
                                            SaveToInventoryLedger();
                                        }
                                    }
                                    
                                    Print(vData);
                                } catch (e) {
                                    alert('An error occured while printing SRV: ' + e);
                                };

                            } else
                                MessageBox(vData.Error);
                        },
                        error: function(vData) {
                            $('[name=butCheckOutNow]').show();
                            NewMessageBox(vData);
                        }
                    }); // AjaxSend                    

                }; // function CheckOut() {
            });

            function SaveToInventoryLedger() {
                AjaxSend({
                    url: 'SalesReturns/GetMemoStocks',
                    data: { MemoID: proj.SalesReturnMemoID },
                    success: function (vData) {
                        vRows = vData.Results.Rows;
                        if (vRows.length > 0) {
                            for (var i = 0; i <= vRows.length - 1; i++) {
                                Business.TransInventoryLedger.Add(vRows[i].ProductID, CStr2(sessionStorage.FowstID),
                                                                proj.SalesReturnMemoID, 'SalesReturn', Web.PeopleUserID(), vRows[i].QuantityReturned * vRows[i].ActualQuantity,
                                                                undefined, undefined, undefined, undefined, undefined, vRows[i].TotalAmount);

                                var mISJIT;
                                var mIsSeperateIntoComponentDuringSalesReturns;

                                pSql = " SELECT IFNULL(IsSeperateIntoComponentDuringSalesReturns, 0) AS IsSeperateIntoComponentDuringSalesReturns" +
                                                " ,Instructions" +
                                                " FROM LibraryPIMProductParent" +
                                                " WHERE LibraryPIMProductParent.ProductID = " + ReadyString(vRows[i].ProductID) +
                                                " AND LibraryPIMProductParent.OrganizationID = " + ReadyString(sessionStorage.OrgID)

                                mIsSeperateIntoComponentDuringSalesReturns = Sql.Execute(pSql);

                                if (mIsSeperateIntoComponentDuringSalesReturns.Rows.length > 0) {
                                    if (CBool(mIsSeperateIntoComponentDuringSalesReturns.Rows[0].IsSeperateIntoComponentDuringSalesReturns)) {
                                        pSql = " SELECT IFNULL(Jit, 0) AS Jit " +
                                                " ,Instructions " +
                                                " FROM LibraryPIMProductParent " +
                                                " WHERE LibraryPIMProductParent.ProductID = " + ReadyString(vRows[i].ProductID) +
                                                " AND LibraryPIMProductParent.OrganizationID = " + ReadyString(sessionStorage.OrgID)

                                        mIsJIT = Sql.Execute(pSql);

                                        if (mIsJIT.Rows.length > 0) {
                                            if (CBool(mIsJIT.Rows[0].Jit)) {

                                                Business.TransInventoryLedger.Add(vRows[i].ProductID, CStr2(sessionStorage.FowstID),
                                                        vRows[i].ProductID, 'SalesReturn', Web.PeopleUserID(), vRows[i].QuantityReturned * vRows[i].ActualQuantity * -1,
                                                        undefined, undefined, undefined, 'Production', undefined, undefined, undefined);

                                                var mContents;

                                                pSql = 'SELECT LibraryPIMProductContents.ChildProductID ' +
                                                ' ,LibraryPIMProduct.ProductNo ' +
                                                ' ,LibraryPIMProduct.ProductName ' +
                                                ' ,LibraryPIMProductContents.UnitMeasureID ' +
                                                ' ,IFNULL(LibraryCountriesLanguageLabel.Label, LibraryPIMProductContents.UnitMeasure) AS UnitMeasure ' +
                                                ' ,LibraryPIMProductContents.Quantity ' +
                                                ' FROM LibraryPIMProductContents ' +
                                                ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryPIMProductContents.ChildProductID ' +
                                                ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductContents.UnitMeasureID ' +
                                                ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID ' +
                                                ' WHERE LibraryPIMProductContents.ParentProductID = ' + ReadyString(vRows[i].ProductID) +
                                                ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                                                ' ORDER BY LibraryPIMProduct.ProductName';

                                                mContents = Sql.Execute(pSql);

                                                for (var j = 0; j <= mContents.Rows.length - 1; j++) {
                                                    Business.TransInventoryLedger.Add(mContents.Rows[j].ChildProductID, CStr2(sessionStorage.FowstID),
                                                                vRows[i].ProductID, 'Sales Return', Web.PeopleUserID(), mContents.Rows[j].Quantity * vRows[i].QuantityReturned,
                                                                undefined, undefined, undefined, undefined, undefined, undefined);
                                                }
                                            }
                                        }
                                    }
                                }

                                //pSql = 'SELECT ModifierGroupID ' +
                                //        ',SUM(Quantity * ActualQuantity) AS Quantity ' +
                                //        ' FROM TransInvoiceItems ' +
                                //        ' WHERE InvoiceID = ' + ReadyString(vData.InvoiceID) +
                                //        ' AND ModifierGroupID IS NOT NULL' +
                                //        ' GROUP BY ModifierGroupID'

                                //var pDtInvoiceItems;

                                //pDtInvoiceItems = Sql.Execute(pSql);


                                //if (pDtInvoiceItems.Rows.length > 0) {
                                //    pSql = 'Select * from TransInvoiceItemsRebates where ModifierGroupID = ' + ReadyString(pDtInvoiceItems.Rows[0].ModifierGroupID)

                                //    var pDvModifers;
                                //    pDvModifers = Sql.Execute(pSql);

                                //    for (var j = 0; j <= pDvModifers.Rows.length - 1; j++) {
                                //        Business.TransInventoryLedger.Add(pDvModifers.Rows[j].ProductID, pDataInvoiceItems.Rows[i].FowstID,
                                //                                    pDataInvoiceItems.Rows[i].ID, 'Return', Web.PeopleUserID(), pDvModifers.Rows[j].UnitQuantity * pDvModifers.Rows[j].Quantity * pDataInvoiceItems.Rows[i].Quantity,
                                //                                    undefined, undefined, undefined, undefined, undefined, undefined);
                                //    }
                                //}
                            }
                        }


                        //save rebates

                        AjaxSend({
                            url: 'SalesReturns/GetMemoRebatesItems',
                            data: { MemoID: proj.SalesReturnMemoID },
                            success: function (vData) {
                                vRowsRebates = vData.Results.Rows;
                                if (vRowsRebates.length > 0) {
                                    for (var k = 0; k <= vRowsRebates.length - 1; k++) {
                                        Business.TransInventoryLedger.Add(vRowsRebates[k].ProductID, CStr2(sessionStorage.FowstID),
                                                        proj.SalesReturnMemoID, 'Sales Return', Web.PeopleUserID(), vRowsRebates[k].Quantity,
                                                        undefined, undefined, undefined, undefined, undefined, undefined);
                                    }
                                }
                            }
                        })

                    }
                });
            }

            function Print(vParams) {
                vParams = $.extend({
                    DontQueryForUnflushedData: false
                }, vParams);
                var pUrl = 'SalesReturns/CreatePdf';
                if ((proj.IsStockLoadedFromServer || CBool(proj.SalesReturnFromOffline) == false) && !window.IsMobile ) {
                    $('#divCheckOutPrintMessage').text('Sales return data was successfully saved. SRV receipt will begin printing after all sales return data are flushed.').show();
                    if (!vParams.DontQueryForUnflushedData) {
                        if (window.IsMobile) {
                            if (SalesPaymentsExport_offline().HasUnflushedRecords({SrvOnly:true})) {
                                setTimeout(function() {
                                    Print(vParams);
                                }, 5000);
                            } else {
                                vParams.DontQueryForUnflushedData = true;
                                Print(vParams);
                            }
                        } else {
                            AjaxSend({
                                url: 'WebPosFlush/GetUnflushedInfo',
                                success: function(vData) {
                                    if (CInt2(vData.UnflushedSalesReturnRowCount) > 0) {
                                        setTimeout(function() {
                                            Print(vParams);
                                        }, 5000);
                                        return;
                                    }
                                    var pDate = ParseDate(vData.LastQueryTime);
                                    if (pDate.getTime() > mLatestCheckOutTime.getTime()) {
                                        vParams.DontQueryForUnflushedData = true;
                                        Print(vParams);
                                        return;
                                    }

                                    setTimeout(function() {
                                        Print(vParams);
                                    }, 5000);
                                },
                                error: function() {
                                    setTimeout(function() {
                                        Print(vParams);
                                    }, 5000);
                                }
                            });
                        }
                        return;
                    }
                    $('#divCheckOutPrintMessage').text('All sales return data are flushed.').show();
                    pUrl = MergeUrlParameters({}, pUrl, proj.RemoteServer);
                }
                if (CBool(mIsRefund)) {                                        
                    if (InAndroid || InWinRt) {
                    	var vDataPrinterType = getOrgFOWSTPrinterType();
                        if (vDataPrinterType.Rows.length > 0) {
                            if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {

                            } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                            } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                sessionStorage.Printer = 'GPrinter';
                                PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                            } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                WindowsPrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                            } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                //Clear();
                                //location = "/People/PeopleMenu";
                                IminPrintPOSsalesRefundReceipt(proj.SalesReturnMemoID); //commented 2023_11_21__15_59_09_2735
                            } else {
                                PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                            }
                        } else {
                            PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                        }                                 
                    } else {
                        AjaxSend({
                            url: pUrl,
                            data: {
                                Template: 'IssueARefund',
                                UUID: localStorage.UUID,
                                MemoID: proj.SalesReturnMemoID,
                                InvoiceID: proj.SalesReturnInvoiceID
                            },
                            success: function(vData) {
                                if (vData.Error) {
                                    if (vData.Error.indexOf('PrintHash') == -1) {
                                        if (vData.Error == 'UUID is required.')
                                            NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                                'This is by design to enforce document print controls. <br /><br />\n\n' +
                                                'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                                'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                                [
                                                    {
                                                        Text: 'Download Mobiz Controller',
                                                        Click: function() {
                                                            window.open('/Content/PrintApplet.zip', '_blank');
                                                        }
                                                    },
                                                    {
                                                        Text: 'Cancel'
                                                    }
                                                ]);
                                        else
                                            NewMessageBox(vData.Error);
                                    } else
                                        NoPrintHash();
                                } else if (vData.Result == 'ok') {
                                    NewMessageBox('Your document will be printed by your local print server.', '', null, {
                                        Close: function() {
                                            Clear();
                                        }
                                    });
                                    if (proj.IsStockLoadedFromServer) {
                                        $('#divCheckOutPrintMessage').text('Downloading PDF from remote server.').show();
                                        AjaxSend({
                                            url: 'ERP/ImportPrintablePdf',
                                            data: {
                                                Filename: vData.BaseFilename + '.pdf',
                                                Server: proj.RemoteServer
                                            },
                                            success: function(vData2) {
                                                if (vData2.Error)
                                                    NewMessageBox(vData2.Error, 'Fail');
                                                else
                                                    $('#divCheckOutPrintMessage').text('PDF was downloaded from remote server. Mobiz Controller will now print the sales return receipt.').show();
                                            }
                                        });
                                    }
                                }
                            }
                        });
                        return;
                        var japplet = document.jzebra;
                        var blnIsPrinterFound = false;

                        if (japplet != null) {
                            while (!blnIsPrinterFound) {

                                if (FindDesktopPrinter("")) {
                                    blnIsPrinterFound = true;
                                    PrintPOSDesktopSalesRefundReceipt(proj.SalesReturnMemoID);
                                }

                            }
                        } else {
                            MessageBox("Jzebra applet was not loaded.",
                                    "Jzebra",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }
                    }
                } else {
                    if (InAndroid || InWinRt) {
                    	var vDataPrinterType = getOrgFOWSTPrinterType();
                        if (vDataPrinterType.Rows.length > 0) {
                            if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {

                            } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                            } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                sessionStorage.Printer = 'GPrinter';
                                PrintPOSSRVReceipt(proj.SalesReturnMemoID);
                            } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                WindowsPrintPOSSRVReceipt(proj.SalesReturnMemoID);
                            } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                IminPrintPOSSRVReceipt(proj.SalesReturnMemoID);
                            } else {
                                PrintPOSSRVReceipt(proj.SalesReturnMemoID);
                            }
                        } else {
                            PrintPOSSRVReceipt(proj.SalesReturnMemoID);
                        } 
                    } else {
                        AjaxSend({
                            url: pUrl,
                            data: {
                                Template: 'MerchandiseExchange',
                                UUID: localStorage.UUID,
                                MemoID: proj.SalesReturnMemoID,
                                InvoiceID: proj.SalesReturnInvoiceID
                            }, success: function (vData) {
                                if (vData.Error) {
                                    if (vData.Error.indexOf('PrintHash') == -1) {
                                        if (vData.Error == 'UUID is required.')
                                            NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                                'This is by design to enforce document print controls. <br /><br />\n\n' +
                                                'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                                'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                                [
                                                    {
                                                        Text: 'Download Mobiz Controller',
                                                        Click: function () {
                                                            window.open('/Content/PrintApplet.zip', '_blank');
                                                        }
                                                    },
                                                    {
                                                        Text: 'Cancel'
                                                    }
                                                ]);
                                        else
                                            NewMessageBox(vData.Error);
                                    } else
                                        NoPrintHash();
                                } else if (vData.Result == 'ok') {
                                    var pMsg = 'Your document will be printed by your local print server.';
                                    if ((CStr2(vParams.SrvNum) != '')
                                        && (CStr2(vParams.Pin) != ''))
                                        pMsg += '<br />SRV No.: ' + vParams.SrvNum +
                                            '<br />Pin: ' + vParams.Pin;
                                    NewMessageBox(pMsg, '', null, {
                                        Close: function () {
                                            Clear();
                                        }
                                    });

                                    if (proj.IsStockLoadedFromServer) {
                                        $('#divCheckOutPrintMessage').text('Downloading PDF from remote server.').show();
                                        AjaxSend({
                                            url: 'ERP/ImportPrintablePdf',
                                            data: {
                                                Filename: vData.BaseFilename + '.pdf',
                                                Server: proj.RemoteServer
                                            },
                                            success: function(vData2) {
                                                if (vData2.Error)
                                                    NewMessageBox(vData2.Error, 'Fail');
                                                else
                                                    $('#divCheckOutPrintMessage').text('PDF was downloaded from remote server. Mobiz Controller will now print the sales return receipt.').show();

                                            }
                                        });
                                    }
                                }
                            }
                        })
                        return
                        var japplet = document.jzebra;
                        var blnIsPrinterFound = false;

                        if (japplet != null) {
                            while (!blnIsPrinterFound) {

                                if (FindDesktopPrinter("")) {
                                    blnIsPrinterFound = true;
                                    PrintPOSDesktopSRVReceipt(proj.SalesReturnMemoID);
                                }

                            }
                        } else {
                            MessageBox("Jzebra applet was not loaded.",
                                    "Jzebra",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }

                    }
                }
            } // END function Print() {

            $('#butApproveNow').click(function(e) {
                Approve({
                    MemoID: proj.SalesReturnMemoID, 
                    Approve: true,
                    success: Print
                });
            });

            $('#butGetApprovalStatus').click(function(e) {
                
                GetApproval();

            }); // END $('#butGetApprovalStatus').click(function(e) {

            var mApprovalTimer;

            function GetApproval() {
                
                clearTimeout(mApprovalTimer);
                $('#divGetApprovalResult').text('Connecting to server for latest status');
                $('#butGetApprovalStatus');//.button('disable'); // http://stackoverflow.com/a/5875143
                AjaxSend({
                    url: 'SalesReturns/GetApprovalStatus',
                    timeout: 30 * 1000,
                    data: {
                        MemoID: proj.SalesReturnMemoID
                    },
                    success: function(vData) {
                        $('#butGetApprovalStatus');//.button('enable')
                        if (vData.Error) {
                            $('#divGetApprovalResult').text(vData.Error);
                            LaunchTimer();
                        } else if (vData.Result == 'denied')  {
                            clearTimeout(mApprovalTimer);
                            $('#butGetApprovalStatus');//.button('disable'); // http://stackoverflow.com/a/5875143
                            $('#butApproveNow');//.button('disable');
                            $('#divGetApprovalResult').text('Success. This transaction has been disapproved.');
                        } else if (vData.Result == 'approved')  {
                            clearTimeout(mApprovalTimer);
                            $('#butGetApprovalStatus');//.button('disable'); // http://stackoverflow.com/a/5875143
                            $('#butApproveNow');//.button('disable');
                            $('#divGetApprovalResult').text('Success. This transaction has been approved.');
                            if (IsMobile) {
                                UpdateSalesTextFile();
                            }
                            Print();
                        } else if (CStr2(vData.Result).trim() != '') {
                            if (vData.Result == 'pending')
                                vData.Result = 'No action was taken by manager yet';
                            $('#divGetApprovalResult').text(vData.Result);
                            LaunchTimer();
                        } else {
                            $('#divGetApprovalResult').text('Unexpected result from the server. jsNHT8NJ23F6!');
                            LaunchTimer();
                        }
                    },
                    error: function() {
                        $('#butGetApprovalStatus');//.button('enable')
                        $('#divGetApprovalResult').text('Cannot connect to server. Please try again later.');
                        LaunchTimer();
                    }
                });

                function LaunchTimer() {
                    
                    mApprovalTimer = setTimeout(GetApproval, 10000);

                } // END function LaunchTimer() {
            } // END function GetApproval() {

            $('#divCheckOutBack').click(function () {
                $('#pageSalesReturn').pageshow();
            });

            function PrintPOSSRVReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};

                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                pData.MemoID = pMemoID;
                if (IsMobile)
                    pData.DeviceSerial = Web.DeviceSerial();

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP", // not used
                    data: pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;
                            if (IsMobile)
                                pData.DeviceSerial = Web.DeviceSerial();

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo", // not used
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        //Get SRV Deatils
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                    if (parseFloat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                                        return;
                                                    }

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'SRV ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print QR
                                                    strPrint = SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'Pin ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print Barcode 1D
                                                    strPrint = $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Invoice Payment', 22, strInvoicePayment, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net of Payment', 15, strInvoiceBalance, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strPrint = strPrint + '\n';

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Value of Returns', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of Returns', 20, strNetValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strPrint = strPrint + '\n';

                                                    var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                                    var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intFAFCTotalLenght - 22, ' ', 0);

                                                    if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                                        strInvoiceBalanceNetPaymentAndReturns = 'NONE';
                                                    }

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net  Payments and Returns', 20, strInvoiceBalanceNetPaymentAndReturns, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    //Get SRV Deatils
                                                    Ajax_SalesPayments_getSalesPaymentSRVReceiptInvoices({
                                                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices", // not used
                                                        data: pData,
                                                        success: function (vDataSRVInv) {

                                                            //Sales Return Voucher Settings 
                                                            Ajax_SalesReturns_GetSettings({
                                                                url: 'SalesReturns/GetSettings', // not used
                                                                success: function (vSRVSettingsData) {

                                                                    if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                                        strPrint = '';
                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Used Portion of SRV', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                            var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                            var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intFAFCTotalLenght - 30, ' ', 0);


                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, strInvAndDate, 28, strUsedAmount, intFAFCTotalLenght, '', 0, 0);
                                                                            strPrint = strPrint + strTemp;

                                                                        };

                                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    }

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of SRV', 16, strNetSRV, intFAFCTotalLenght, '', 0, 0);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This SRV is as good as cash.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                    if (vSRVSettingsData.SRVUseSettings != undefined) {
                                                                        if (vSRVSettingsData.SRVUseSettings == '1-Time Use of SRV') {
                                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "1-TIME USE ONLY. Any unused portion of this SRV will be forfeited and is non-refundable. Please use up the entire value of this SRV.", intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                        }
                                                                    }

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Present and surrender this SRV to       cashier as form of payment.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    if (mAllowOfflineSrvPayments) {
                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Please use this SRV where it was        issued.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;
                                                                    }

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '* To get electronic copy of this SRV,     please register with Customer Service.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = '\n';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    //Print Barcode
                                                                    strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                                    PrintText(strPrinterIP);

                                                                    setTimeout(VerifyPrintReceipt(pMemoID, true, {
                                                                        PinCode: vDataSRV.SalesPaymentData.Rows[0].PinCode,
                                                                        SRVNumber: vDataSRV.SalesPaymentData.Rows[0].SRVNumber
                                                                    }), 2000)

                                                                }
                                                            });

                                                        }
                                                    });
                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function(vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        { 
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }
                                }
                            });
                        }
                        else {
                            //Get SRV Deatils
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // URL is not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo

                            
                        }

                    }
                });

            }; // END function PrintPOSSRVReceipt(pMemoID) {

            function Ajax_Organization_getOrgFOWSTPrinterIP(vParams) {
                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pResult = SalesPayment_offline().getOrgFOWSTPrinterIP();
                    vParams.success({
                        OrganizationFOWSTData: pResult
                    });
                } else
                    AjaxSend({
                        url: "Organization/getOrgFOWSTPrinterIP",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            } // END function Ajax_Organization_getOrgFOWSTPrinterIP(vParams) {

            function Ajax_SalesPayments_getReceiptHeaderInfo(vParams) {
                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pResult = SalesPayment_offline().getReceiptHeaderInfo();
                    vParams.success({
                        SalesPaymentData: pResult
                    });
                } else
                    AjaxSend({
                        url: "SalesPayments/getReceiptHeaderInfo",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            } // END function Ajax_SalesPayments_getReceiptHeaderInfo(vParams) {

            function Ajax_SalesPayments_getSalesPaymentSRVRefundHeaderInfo(vParams) {
                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pResult = Business.TransSalesReturn.getSalesPaymentSRVRefundHeaderInfo(vParams.data.MemoID);
                    vParams.success({
                        SalesPaymentData: pResult
                    });
                } else
                    AjaxSend({
                        url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            } // END function Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo(vParams) {

            function Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo(vParams) {
                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pResult = Business.TransSalesReturn.getSalesPaymentSRVReceiptHeaderInfo(vParams.data.MemoID);
                    vParams.success({
                        SalesPaymentData: pResult
                    });
                } else
                    AjaxSend({
                        url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            } // END function Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo(vParams) {

            function Ajax_SalesPayments_getSalesPaymentSRVReceiptInvoices(vParams) {
                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var pResult = Business.TransSalesReturn.getSalesPaymentSRVReceiptInvoices(vParams.data.MemoID);
                    vParams.success({
                        SalesPaymentData: pResult
                    });
                } else
                    AjaxSend({
                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            } // END function Ajax_SalesPayments_getSalesPaymentSRVReceiptInvoices(vParams) {

            function Ajax_SalesReturns_GetSettings(vParams) {
                //if (mUseOfflineData) {
                if (IsMobile) {
                    var pResult = Business.TransSalesReturn.GetSettings();
                    vParams.success(pResult);
                } else
                    AjaxSend({
                        url: "SalesReturns/GetSettings",
                        data: vParams.data,
                        success: function (vDataPrint) {
                            vParams.success(vDataPrint);
                        }
                    });
            }

			function WindowsPrintPOSSRVReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};

                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP", // not used
                    data:pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;
                            if (IsMobile)
                                pData.DeviceSerial = Web.DeviceSerial();

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo", // not used
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

										WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);
                                        
                                        WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), [1, 0, 0, 1, 0, 36], null, null, null, null);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), [1, 0, 0, 1, 0, 36], null, null, null, null);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;
                                        
                                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        //Get SRV Deatils
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                    if (parseFloat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                                        return;
                                                    }
                                                    
                                                    WindowsPrinter('Customer : ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Address : ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('TIN : '+$.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    WindowsPrinter('SRV '+ SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'SRV ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print QR
                                                    //strPrint = SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    WindowsPrinter('Pin '+ $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'Pin ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print Barcode 1D
                                                    //strPrint = $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    WindowsPrinter('Date and Time : '+ ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), [0, 0, 0, 1, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    WindowsPrinter('Bought From : '+ ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Returned To : '+ ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Processed By : '+ ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Approved By : '+ ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    
													WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    WindowsPrinter('Invoice No.', [0, 0, 0, 0, 0, 36], SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Invoice Amount', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter('Less: Invoice Payment', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Invoice Payment', 22, strInvoicePayment, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    
													WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    WindowsPrinter('Invoice Balance Net of Payment'+ AccountingFormat(dblInvBalance), [1, 16, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net of Payment', 15, strInvoiceBalance, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    //strPrint = strPrint + '\n';

                                                    
													WindowsPrinter('Value of Returns', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Value of Returns', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    
													WindowsPrinter('Less: One-Time Restocking Fee', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    WindowsPrinter('Net Value of Returns', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of Returns', 20, strNetValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    //strPrint = strPrint + '\n';

                                                    var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                                    var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intFAFCTotalLenght - 22, ' ', 0);

                                                    if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                                        strInvoiceBalanceNetPaymentAndReturns = 'NONE';
                                                    }

                                                    WindowsPrinter('Invoice Balance Net Payments and Returns', [1, 20, 0, 0, 0, 36], AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), [0, 0, 0, 0, 1, 36], null, null);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net  Payments and Returns', 20, strInvoiceBalanceNetPaymentAndReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    //Get SRV Deatils
                                                    Ajax_SalesPayments_getSalesPaymentSRVReceiptInvoices({
                                                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices", // not used
                                                        data: pData,
                                                        success: function (vDataSRVInv) {

                                                            //Sales Return Voucher Settings 
                                                            Ajax_SalesReturns_GetSettings({
                                                                url: 'SalesReturns/GetSettings', // not used
                                                                success: function (vSRVSettingsData) {

                                                                    if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                                        WindowsPrinter('Less: Used Portion of SRV', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                                        //strPrint = '';
                                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Used Portion of SRV', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        //strPrint = strPrint + strTemp;

                                                                        for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                            var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                            var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intFAFCTotalLenght - 30, ' ', 0);

																			WindowsPrinter('SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || ''), [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strInvAndDate, 28, strUsedAmount, intFAFCTotalLenght, '', 0, 0);
                                                                            //strPrint = strPrint + strTemp;

                                                                        };

                                                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    }

                                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);

                                                                    WindowsPrinter('Net Value of SRV', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of SRV', 16, strNetSRV, intFAFCTotalLenght, '', 0, 0);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    WindowsPrinter('This SRV is as good as cash.', [0, 0, 0, 1, 0, 36], null, null, null, null);
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This SRV is as good as cash.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                    if (vSRVSettingsData.SRVUseSettings != undefined) {
                                                                        if (vSRVSettingsData.SRVUseSettings == '1-Time Use of SRV') {
                                                                            WindowsPrinter('1-TIME USE ONLY. Any unused portion of this SRV will be forfeited and is non-refundable. Please use up the entire value of this SRV.', [0, 0, 0, 1, 0, 36], null, null, null, null);
                                                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, "1-TIME USE ONLY. Any unused portion of this SRV will be forfeited and is non-refundable. Please use up the entire value of this SRV.", intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                        }
                                                                    }

                                                                    WindowsPrinter('* Present and surrender this SRV to cashier as form of payment.', [1, 0, 0, 0, 0, 36], null, null, null, null);
                                                                    //strPrint = '';
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Present and surrender this SRV to cashier as form of payment.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    WindowsPrinter('* Do not lose or crumple this receipt.', [1, 0, 0, 0, 0, 36], null, null, null, null);
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Do not lose or crumple this receipt.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    WindowsPrinter('* To get electronic copy of this SRV, please register with Customer Service.', [1, 0, 0, 0, 0, 36], null, null, null, null);
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* To get electronic copy of this SRV, please register with Customer Service.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                                    //strPrint = '\n';
                                                                    WindowsPrinter('This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', [1, 0, 0, 1, 0, 36], null, null, null, null);
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //strPrint = strPrint + strTemp;

                                                                   // ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    //Print Barcode
                                                                    //strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                                    //PrintText(strPrinterIP);
                                                                    WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                                					WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);


                                                                    setTimeout(VerifyPrintReceipt(pMemoID, true, {
                                                                        PinCode: vDataSRV.SalesPaymentData.Rows[0].PinCode,
                                                                        SRVNumber: vDataSRV.SalesPaymentData.Rows[0].SRVNumber
                                                                    }, 'wps'), 2000);

                                                                }
                                                            });

                                                        }
                                                    });
                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function(vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        { 
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }

                                }
                            });

                        }
                        else {
                            //Get SRV Deatils
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            });
                        }

                    }
                });

            };

            function IminPrintPOSSRVReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};

                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                try {
                    window.pPrint.InitializeBluetoothPrinter();
                } catch (e) {
                    console.log('InitializeBluetoothPrinter: ' + e);
                };

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP", // not used
                    data: pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;
                            if (IsMobile)
                                pData.DeviceSerial = Web.DeviceSerial();

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo", // not used
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", "VAT TIN: " + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", "Tel No.: " + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                        IminfeedPaper();
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        //Get SRV Deatils
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                    if (parseFloat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                                        return;
                                                    }

                                                    IminPrintInColumns('Customer : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);
                                                    IminPrintInColumns('Address : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);
                                                    IminPrintInColumns('TIN : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);

                                                    IminfeedPaper();

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('SRV : ', "", $.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 4, 0, 6, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'SRV ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print QR
                                                    //strPrint = SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    IminPrintInColumns('Pin : ', "", $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), 4, 0, 6, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'Pin ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //Print Barcode 1D
                                                    //strPrint = $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                    IminPrintInColumns('Date/Time : ', "", ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), 4, 0, 6, 0, 0, 2, 24, 24, 24);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                    IminfeedPaper();

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('Bought From : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Returned From : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Processed By : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Approved By : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //ReadyTextToPrint('\n' + strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);


                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    IminPrintInColumns('Invoice No. : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Invoice Amount : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Less: Invoice Payment ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Invoice Payment', 22, strInvoicePayment, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);


                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    IminPrintInColumns('Invoice Balance Net of Payment ', "", AccountingFormat(dblInvBalance), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net of Payment', 15, strInvoiceBalance, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminfeedPaper();
                                                    //strPrint = strPrint + '\n';

                                                    IminPrintInColumns('Value of Returns : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Value of Returns', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Less: One-Time Restocking Fee ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    IminPrintInColumns('Net Value of Returns : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of Returns', 20, strNetValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminfeedPaper();
                                                    //strPrint = strPrint + '\n';

                                                    var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                                    var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intFAFCTotalLenght - 22, ' ', 0);

                                                    if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                                        strInvoiceBalanceNetPaymentAndReturns = 'NONE';
                                                    }

                                                    IminPrintInColumns('Invoice Balance Net Payments and Returns : ', "", AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), 5, 0, 5, 0, 0, 2, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net  Payments and Returns', 20, strInvoiceBalanceNetPaymentAndReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    //Get SRV Deatils
                                                    Ajax_SalesPayments_getSalesPaymentSRVReceiptInvoices({
                                                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices", // not used
                                                        data: pData,
                                                        success: function (vDataSRVInv) {

                                                            //Sales Return Voucher Settings 
                                                            Ajax_SalesReturns_GetSettings({
                                                                url: 'SalesReturns/GetSettings', // not used
                                                                success: function (vSRVSettingsData) {

                                                                    if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                                        IminPrintInColumns('Less: Used Portion of SRV ', "", "", 10, 0, 0, 0, 0, 2, 24, 24, 24);
                                                                        //strPrint = '';
                                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Used Portion of SRV', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        //strPrint = strPrint + strTemp;

                                                                        for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                            var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                            var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intFAFCTotalLenght - 30, ' ', 0);

                                                                            IminPrintInColumns("SI " + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || ''), "", AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strInvAndDate, 28, strUsedAmount, intFAFCTotalLenght, '', 0, 0);
                                                                            //strPrint = strPrint + strTemp;

                                                                        };

                                                                        //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                                    }

                                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                                    IminfeedPaper();
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);

                                                                    IminPrintInColumns('Net Value of SRV : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of SRV', 16, strNetSRV, intFAFCTotalLenght, '', 0, 0);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                                    IminfeedPaper();
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    IminPrintInColumns('', "This SRV is as good as cash.", '', 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                                    IminfeedPaper();
                                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This SRV is as good as cash.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                    if (vSRVSettingsData.SRVUseSettings != undefined) {
                                                                        if (vSRVSettingsData.SRVUseSettings == '1-Time Use of SRV') {
                                                                            IminPrintInColumns('', "1-TIME USE ONLY. Any unused portion of this SRV will be forfeited and is non-refundable. Please use up the entire value of this SRV.", '', 0, 10, 0, 0, 0, 2, 24, 24, 24);
                                                                            IminfeedPaper();
                                                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, "1-TIME USE ONLY. Any unused portion of this SRV will be forfeited and is non-refundable. Please use up the entire value of this SRV.", intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                        }
                                                                    }

                                                                    IminPrintInColumns('', "* Present and surrender this SRV to cashier as form of payment.", '', 0, 10, 0, 0, 0, 2, 24, 24, 24);
                                                                    IminfeedPaper();
                                                                    //strPrint = '';
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Present and surrender this SRV to cashier as form of payment.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    IminPrintInColumns('', "* Do not lose or crumple this receipt.", '', 0, 10, 0, 0, 0, 2, 24, 24, 24);
                                                                    IminfeedPaper();

                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Do not lose or crumple this receipt.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    IminPrintInColumns('', "* To get electronic copy of this SRV, please register with Customer Service.", '', 0, 10, 0, 0, 0, 2, 24, 24, 24);
                                                                    IminfeedPaper();

                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* To get electronic copy of this SRV, please register with Customer Service.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    //strPrint = strPrint + strTemp;

                                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                                    //strPrint = '\n';
                                                                    IminPrintInColumns("", "This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                                    IminfeedPaper();
                                                                    IminfeedPaper();
                                                                    IminfeedPaper();
                                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    //strPrint = strPrint + strTemp;

                                                                    // ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    //Print Barcode
                                                                    //strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                                    //PrintText(strPrinterIP);

                                                                    setTimeout(VerifyPrintReceipt(pMemoID, true, {
                                                                        PinCode: vDataSRV.SalesPaymentData.Rows[0].PinCode,
                                                                        SRVNumber: vDataSRV.SalesPaymentData.Rows[0].SRVNumber
                                                                    }, 'imin'), 2000);

                                                                }
                                                            });

                                                        }
                                                    });
                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function (vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        {
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }

                                }
                            });

                        }
                        else {
                            //Get SRV Deatils
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            {
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            });
                        }

                    }
                });

            };
            
            function getOrgFOWSTPrinterType() {

            pSql = "SELECT devfowstpntr.ID, " +
                    " fowstpntr.PrinterIPAddress, " +
                    " dp.PrinterType " +
                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                    " INNER JOIN private.LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " +
                    " INNER JOIN private.LibraryDevicePrinter dp on dp.ID = fowstpntr.PrinterID " +
                    " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

            };

            window.IminPrintPOSsalesRefundReceipt = IminPrintPOSsalesRefundReceipt
            function IminPrintPOSsalesRefundReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};
                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                try {
                    window.pPrint.InitializeBluetoothPrinter();
                } catch (e) {
                    console.log('InitializeBluetoothPrinter: ' + e);
                };

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    data: pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;
                            if (IsMobile)
                                pData.DeviceSerial = Web.DeviceSerial();

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", "VAT TIN: " + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                        IminPrintInColumns("", "Tel No.: " + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                        IminfeedPaper();

                                        //Get SRV Deatils
                                        Ajax_SalesPayments_getSalesPaymentSRVRefundHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                    IminPrintInColumns('Customer : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);
                                                    IminPrintInColumns('Address : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);
                                                    IminPrintInColumns('TIN : ', $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), "", 4, 6, 0, 0, 0, 0, 24, 24, 24);

                                                    IminfeedPaper();

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('Refund No. : ', "", $.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 7, 0, 3, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Refund No. " + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('Date/Time : ', "", ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), 4, 0, 6, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('Bought From : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Returned From : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Processed By : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Approved By : ', $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), "", 5, 5, 0, 0, 0, 0, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint('\n' + strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    IminfeedPaper();

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    //var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    //var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    //var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    IminPrintInColumns('Invoice No. : ', "", $.trim(vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || ''), 5, 0, 5, 0, 0, 2, 24, 24, 24);

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    IminPrintInColumns('Gross Refund : ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Refund', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    IminPrintInColumns('Less: One-Time Restocking Fee ', "", AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), 5, 0, 5, 0, 0, 2, 24, 24, 24);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns('Net Refund Amount : ', "", AccountingFormat((CDec2(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount) - CDec2(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee)) || '0'), 6, 0, 4, 0, 0, 2, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Refund Amount', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    var strNetValueOfRefund = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, strNetValueOfRefund, intFCDoubleTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //ReadyTextToPrint('\n', 3, 24, 1, 0, 0, 1, 0, 0, 0);
                                                    //ReadyTextToPrint('\n', 3, 24, 1, 0, 0, 1, 0, 0, 0);
                                                    //ReadyTextToPrint('\n', 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                                    IminfeedPaper();
                                                    IminfeedPaper();
                                                    IminfeedPaper();

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminPrintInColumns("", "________________________________", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                                    IminPrintInColumns("", "Please sign over printed name", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Please sign over printed name', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    IminfeedPaper();

                                                    IminPrintInColumns("", "This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com", "", 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                                    IminfeedPaper();
                                                    IminfeedPaper();
                                                    IminfeedPaper();
                                                    IminfeedPaper();
                                                    IminfeedPaper();
                                                    IminfeedPaper();

                                                    setTimeout(VerifyPrintReceipt(pMemoID, false, null, 'imin'), 2000)

                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function (vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        {
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }
                                }
                            });

                        }
                        else {
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // URL is not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            {
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo
                        }

                    }
                });

            };

            function WindowsPrintPOSsalesRefundReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};
                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    data: pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;
                            if (IsMobile)
                                pData.DeviceSerial = Web.DeviceSerial();

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

										WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);

                                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

										WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);

                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;
                                        
                                        WindowsPrinter('TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

										WindowsPrinter($.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), [1, 0, 0, 1, 0, 36], null, null, null, null);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

										WindowsPrinter('Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), [1, 0, 0, 1, 0, 36], null, null, null, null);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;
                                        
                                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                        
                                        WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                        //Get SRV Deatils
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

													WindowsPrinter('Customer : '+$.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Address : ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('TIN : ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('Sales Refund No. ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), [0, 0, 0, 1, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Refund No. " + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('Date and Time ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), [0, 0, 0, 1, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('Bought From : ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || ''), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Returned To : ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || ''), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Processed By : ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Approved By : ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || ''), [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    //var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    //var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    //var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    WindowsPrinter('Invoice No.', [0, 0, 0, 0, 0, 36], $.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), [0, 0, 0, 0, 1, 36], null, null);
                                                    
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Invoice No.', [0, 0, 0, 0, 0, 36], $.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), [0, 0, 0, 0, 1, 36], null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    WindowsPrinter('Gross Refund', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);
                                                    
                                                    //strPrint = '';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Refund', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                    
                                                    WindowsPrinter('Less: One-Time Restocking Fee', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), [0, 0, 0, 0, 1, 36], null, null);

                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('Net Refund Amount', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), [0, 0, 0, 0, 1, 36], null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Refund Amount', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    var strNetValueOfRefund = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    //strPrint = FormatTextToPrint(intFCDoubleTotalLenght, strNetValueOfRefund, intFCDoubleTotalLenght, '', 0, '', 0, 0);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36], null, null, null, null);
                                                    
                                                    WindowsPrinter('Please sign over printed name', [0, 0, 0, 0, 0, 36], null, null, null, null);

                                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Please sign over printed name', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                                    
                                                    WindowsPrinter('This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', [1, 0, 0, 1, 0, 36], null, null, null, null);

                                                    //strPrint = '\n';
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    //strPrint = strPrint + strTemp;

                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    //Print Barcode
                                                    //strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                    //PrintText(strPrinterIP);
                                                    
                                                    WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                                	WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                                    setTimeout(VerifyPrintReceipt(pMemoID, false, null, 'wps'), 2000)

                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function(vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        { 
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }
                                }
                            });

                        }
                        else {
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // URL is not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo
                        }

                    }
                });

            };
            
            function PrintPOSsalesRefundReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};
                var pData2 = {};
                if (IsMobile)
                    pData2.DeviceSerial = Device.Serial();

                pData.MemoID = pMemoID;
                if (IsMobile)
                    pData.DeviceSerial = Web.DeviceSerial();

                Ajax_Organization_getOrgFOWSTPrinterIP({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    data: pData2,
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            //Get Header
                            Ajax_SalesPayments_getReceiptHeaderInfo({
                                url: "SalesPayments/getReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataRHeader) {

                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

										strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

										strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

										strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

										strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                        //Get SRV Deatils
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                    if (CStr2(vDataSRV.SalesPaymentData.Rows[0].RefundNum).trim() != '')
                                                        vDataSRV.SalesPaymentData.Rows[0].SRVNumber = vDataSRV.SalesPaymentData.Rows[0].RefundNum;
													strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Refund No. " + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                    var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                    var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Refund', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                    
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Refund Amount', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    var strNetValueOfRefund = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, strNetValueOfRefund, intFCDoubleTotalLenght, '', 0, '', 0, 0);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Please sign over printed name', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                                    
                                                    strPrint = '\n';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                    //Print Barcode
                                                    //strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                    PrintText(strPrinterIP);

                                                    setTimeout(VerifyPrintReceipt(pMemoID, false), 2000);

                                                }
                                            }
                                        });

                                    } else {
                                        Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                            data: pData,
                                            success: function(vDataSRV) {
                                                var pMessage = '';
                                                if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                                    pMessage = 'Sales refund was successfully saved.<br />';
                                                else
                                                    pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                                        ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                                pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                                NewMessageBox(pMessage, "Printer",
                                                    [
                                                        { 
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                Clear();
                                                                location = "/People/PeopleMenu";
                                                            }
                                                        }
                                                    ]
                                                );
                                            }
                                        }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                    }

                                }
                            });

                        }
                        else {
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // URL is not used
                                data: pData,
                                success: function (vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo
                        }

                    }
                });

            };
            
            function WindowsPrinter(cols1, args1, cols2, args2, cols3, args3) {
                    if ((cols3 == null || cols3 == undefined) && (cols2 != null || cols2 != undefined)) {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                              { "cols2": cols2 },
                                                              { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] }
												    ]
												}
						);
                    } else if ((cols3 == undefined || cols3 == null) && (cols2 == null || cols2 == undefined)) {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] }
												    ]
												}
						);
                    } else if (cols1 == "newLine") {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "newLine": "newLine" }
												    ]
												}
						);
                    } else if (cols1 == "startPrint") {
                        try {
                            Android.WindowsPrinting(
                                                    {
                                                        "data": [
                                                                  { "startPrint": "startPrint" }
                                                        ]
                                                    }
                            );
                        } catch (e) {
                            //get ip address from settings
                            var MobizSettingsIPAddress = '';

                            MobizSettingsIPAddress = Transfers.ReadFile('printerip.txt', null, true)

                            if (MobizSettingsIPAddress != undefined && MobizSettingsIPAddress != '') {

                                Android.AddLog("Overriding printing IP with " + MobizSettingsIPAddress);

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], MobizSettingsIPAddress, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                try {
                                    Android.WindowsPrinting(
                                                                {
                                                                    "data": [
                                                                              { "startPrint": "startPrint" }
                                                                    ]
                                                                }
                                                            );
                                } catch (e) {
                                    Android.AddLog("A problem occured while printing receipt using WPS method.")
                                }
                            } else {
                                Android.AddLog("A problem occured while printing receipt using WPS method.")
                            }
                        }
                    } else if (cols1 == "address") {
                        Android.AddLog("WPS IP address.: " + cols2);

                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "address": cols2 }
												    ]
												}
						);
                    } else {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                              { "cols2": cols2 },
                                                              { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] },
                                                              { "cols3": cols3 },
                                                              { "args3": [args3[0].toString(), args3[1].toString(), args3[2].toString(), args3[3].toString(), args3[4].toString(), args3[5].toString()] }
												    ]
												}
						);
                    }
                };

            function PrintPOSDesktopSRVReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var pData = {};
                var japplet = document.jzebra;

                pData.MemoID = pMemoID;
                if (IsMobile)
                    pData.DeviceSerial = Web.DeviceSerial();

                //Get Header
                AjaxSend({
                    url: "SalesPayments/getReceiptHeaderInfo",
                    data: pData,
                    success: function (vDataHeader) {

                        if (vDataHeader.SalesPaymentData.Rows.length > 0) {

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].BusinessTradeName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CorporateName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("TIN " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TIN || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CompleteAddress || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("Tel. No. " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TelNo || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            //Get SRV Deatils
                            AjaxSend({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataSRV) {

                                    if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                        if (parseFloat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                            return;
                                        }

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Address  ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('TIN      ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV No. ', 0, 8), 8, ': ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Pin     ', 0, 8), 8, ': ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].PinCode || ''), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Bought From ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Returned To ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Processed By', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Approved By ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intDesktopPrintLenght - 13, ' ', 0);
                                        var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intDesktopPrintLenght - 16, ' ', 0);
                                        var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intDesktopPrintLenght - 23, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice No.', 0, 11), 11, strInvoiceNo, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Amount', 0, 14), 14, strInvoiceAmount, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: Invoice Payment', 0, 21), 21, strInvoicePayment, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                        var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intDesktopPrintLenght - 32, ' ', 0);
                                        var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intDesktopPrintLenght - 18, ' ', 0);
                                        var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intDesktopPrintLenght - 31, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Balance Net of Payment', 0, 30), 30, strInvoiceBalance, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        japplet.append("\n");
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Value of Returns', 0, 16), 16, strValueOfReturns, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: One-Time Restocking Fee', 0, 29), 29, strRestockingFee, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intDesktopPrintLenght - 22, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Net Value of Returns', 0, 20), 20, strNetValueOfReturns, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");
                                        japplet.append("\n");

                                        var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                        var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intDesktopPrintLenght - 21, ' ', 0);

                                        if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                            strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix('NONE', intDesktopPrintLenght - 21, ' ', 0);
                                        }

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Balance Net', 0, 19), 19, strInvoiceBalanceNetPaymentAndReturns, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Payments and Returns', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                        japplet.append(strPrint + "\n");

                                        //Get SRV Deatils
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentSRVReceiptInvoices",
                                            data: pData,
                                            success: function (vDataSRVInv) {

                                                //Sales Return Voucher Settings 
                                                AjaxSend({
                                                    url: 'SalesReturns/GetSettings',
                                                    success: function (vSRVSettingsData) {

                                                        if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: Used Portion of SRV', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intDesktopPrintLenght - 30, ' ', 0);

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strInvAndDate, 28, strUsedAmount, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                            };

                                                        }

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intDesktopPrintLenght - 18, ' ', 0);

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Net Value of SRV', 0, 16), 16, strNetSRV, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('This SRV is as good as cash.', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        if (vSRVSettingsData.SRVUseSettings != undefined) {
                                                            if (vSRVSettingsData.SRVUseSettings == '1-Time Use of SRV') {
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('1-TIME USE ONLY.', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Any unused portion of this SRV will be', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('forfeited and is non-refundable.', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Please use the entire value of this SRV.', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                            }
                                                        }

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* Present and surrender this SRV to cashier.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('  as form of payment.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* Do not lose or crumple this receipt.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* To get electronic copy of this SRV, please', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('  register with Customer Service.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");

                                                        japplet.print();
                                                        setTimeout(VerifyPrintReceipt(pMemoID, true, {
                                                            PinCode: vDataSRV.SalesPaymentData.Rows[0].PinCode,
                                                            SRVNumber: vDataSRV.SalesPaymentData.Rows[0].SRVNumber
                                                        }), 2000)
                                                    }
                                                });

                                            }
                                        });
                                    }
                                }
                            });

                        } else {
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                data: pData,
                                success: function(vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({

                        }

                    }
                });
            }

            function PrintPOSDesktopSalesRefundReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var pData = {};
                var japplet = document.jzebra;

                pData.MemoID = pMemoID;
                if (IsMobile)
                    pData.DeviceSerial = Web.DeviceSerial();

                //Get Header
                AjaxSend({
                    url: "SalesPayments/getReceiptHeaderInfo",
                    data: pData,
                    success: function (vDataHeader) {

                        if (vDataHeader.SalesPaymentData.Rows.length > 0) {

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].BusinessTradeName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CorporateName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("TIN " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TIN || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CompleteAddress || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("Tel. No. " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TelNo || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            //Get SRV Deatils
                            AjaxSend({
                                url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                                data: pData,
                                success: function (vDataSRV) {

                                    if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                        if (parseFloat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                            return;
                                        }

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Address  ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('TIN      ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign("Sales Refund No. " + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Bought From ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Returned To ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Processed By', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Approved By ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intDesktopPrintLenght - 13, ' ', 0);
                                        var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intDesktopPrintLenght - 16, ' ', 0);
                                        var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intDesktopPrintLenght - 23, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice No.', 0, 11), 11, strInvoiceNo, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Amount', 0, 14), 14, strInvoiceAmount, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                        var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intDesktopPrintLenght - 12, ' ', 0);
                                        var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intDesktopPrintLenght - 18, ' ', 0);
                                        var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intDesktopPrintLenght - 31, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Refund', 0, 12), 12, strInvoiceBalance, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: One-Time Restocking Fee', 0, 29), 29, strRestockingFee, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var strNetValueOfRefund = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intDesktopPrintLenght - 15, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Refund Amount', 0, 13), 13, strNetValueOfRefund, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n\n\n\n\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('Please sign over printed name', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n\n");


                                        strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");

                                        japplet.print();
                                        setTimeout(VerifyPrintReceipt(pMemoID, false), 2000)
                                    }
                                }
                            });

                        } else {
                            Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo", // not used
                                data: pData,
                                success: function(vDataSRV) {
                                    var pMessage = '';
                                    if (vDataSRV.SalesPaymentData.Rows[0].SRVNumber == null)
                                        pMessage = 'Sales refund was successfully saved.<br />';
                                    else
                                        pMessage = 'Merchandise exchange was successfully saved.<br />' +
                                            ' SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "This branch has no receipt settings. Please contact your system adminstrator.";
                                    NewMessageBox(pMessage, "Printer",
                                        [
                                            { 
                                                Text: 'Ok',
                                                Click: function () {
                                                    Clear();
                                                    location = "/People/PeopleMenu";
                                                }
                                            }
                                        ]
                                    );
                                }
                            }); // END Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo({
                        }

                    }
                });


            };

            function VerifyPrintReceipt(pMemoID, IsSRV, vData, printer) {


                window.pageSalesReturn = $.mobile.activePage;

                var pMessage = '';

                if (vData != null) {
                    if (IsSRV)
                        pMessage = 'SRV#: ' + vData.SRVNumber + ';<br />Pin Code: ' + vData.PinCode + ';<br />';
                }
                pMessage += "Your sales return cart was successfully checked out. Were you able to successfully print out a " + (IsSRV ? " SRV Voucher" : " Refund Voucher") + "?";
                NewMessageBox(pMessage,
                            "Print Confirmation",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        Clear();
                                        // window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage.OrgID, "_self");
                                        location = "/People/PeopleMenu";
                                    }
                                },
                                { Text: "No, it didn't print",
                                    Click: function () {

                                        window.pageSecuritySourcePage = $.mobile.activePage;

                                        if (IsSRV == true) {
                                            PromptUserAccess('POS Receipt Reprint', {
                                                OnClickOKSuccess: function () {

                                                    $('#divCheckOut').pageshow();

                                                    if (InAndroid || InWinRt) {
                                                        if (printer == 'imin') {
                                                            IminPrintPOSSRVReceipt(pMemoID);
                                                        } else if (printer == 'wps') {
                                                            WindowsPrintPOSSRVReceipt(pMemoID);
                                                        } else {
                                                            PrintPOSSRVReceipt(pMemoID);
                                                        }                                                        
                                                    } else {

                                                        var japplet = document.jzebra;
                                                        var blnIsPrinterFound = false;

                                                        if (japplet != null) {
                                                            while (!blnIsPrinterFound) {

                                                                if (FindDesktopPrinter("")) {
                                                                    blnIsPrinterFound = true;

                                                                    PrintPOSDesktopSRVReceipt(pMemoID)
                                                                };

                                                            }
                                                        } else {
                                                            MessageBox("Jzebra applet was not loaded.",
                                                                "Jzebra",
                                                                [
                                                                    { Text: 'Ok'
                                                                    }
                                                                ]
                                                        );
                                                        }
                                                    }

                                                }
                                            });

                                        } else {
                                            PromptUserAccess('POS Receipt Reprint', {
                                                OnClickOKSuccess: function () {

                                                    $('#divCheckOut').pageshow();

                                                    if (InAndroid || InWinRt) {
                                                        if (printer == 'imin') {
                                                            IminPrintPOSsalesRefundReceipt(pMemoID);
                                                        } else if (printer == 'wps') {
                                                            WindowsPrintPOSsalesRefundReceipt(pMemoID);
                                                        } else {
                                                            PrintPOSsalesRefundReceipt(pMemoID);
                                                        }
                                                    } else {

                                                        var japplet = document.jzebra;
                                                        var blnIsPrinterFound = false;

                                                        if (japplet != null) {
                                                            while (!blnIsPrinterFound) {

                                                                if (FindDesktopPrinter("")) {
                                                                    blnIsPrinterFound = true;

                                                                    PrintPOSDesktopSalesRefundReceipt(pMemoID)
                                                                };

                                                            }
                                                        } else {
                                                            MessageBox("Jzebra applet was not loaded.",
                                                                "Jzebra",
                                                                [
                                                                    { Text: 'Ok'
                                                                    }
                                                                ]
                                                        );
                                                        }
                                                    }

                                                }
                                            });
                                        }

                                    }
                                }
                            ], {
                                AutoDismiss: false,
                                Close: function() {
                                    // dont do these. 2023_11_21__08_39_59_1773
                                    //Clear();
                                    //// window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage.OrgID, "_self");
                                    //location = "/People/PeopleMenu";
                                }
                            }, true
                );


            }

            window.UpdateSalesTextFile = UpdateSalesTextFile
            function UpdateSalesTextFile() {
                
                //I use getCashierSalesTransactionSummary so that i dnt have to create another query from the server

                //note: check dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0); AND
                //dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                //its value comes from 'TOTALS' this will cause descrepancy

                var pSql = '';
                var pData = {};


                var dblTotalSales = 0;
                var dblTotalSalesDiscount = 0;
                var dblAdjRefund = 0;
                var dblAdjSRV = 0;
                var dblNetSales = 0;

                var dblCash = 0;
                var dblCheck = 0;
                var dblDebitCard = 0;
                var dblCreditCard = 0;
                var dblSRV = 0;
                var dblTotalPayment = 0;

                var dblVAT = 0;
                var dblVATAmount = 0;
                var dblNonVAT = 0;
                var dblVATExempt = 0;
                var dblZeroRatedVAT = 0;
                var dblTotalVAT = 0;

                var dblTotalVATSales = 0;
                var dblTotalVATAmount = 0;
                var dblTotalVATSalesSalesReturn = 0;
                var dblTotalVATAmountSalesReturn = 0;

                var mDate = new Date();

                pData.FowstID = CStr2(sessionStorage.FowstID);
                pData.RangeType = $.trim('Day');
                pData.RangeFrom = (mDate.getMonth() + 1) + '/' + mDate.getDate() + '/' + mDate.getFullYear();
                pData.RangeTo = (mDate.getMonth() + 1) + '/' + mDate.getDate() + '/' + mDate.getFullYear();;
                pData.RangeYear = mDate.getFullYear;

                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    var CurTotalVATSalesSalesReturn;
                    var CurTotalVATAmountSalesReturn;
                    var CurSalesReturnRefund;
                    var CurSalesReturnExchange;

                    var pSql = "SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0)) CurTotalVATSalesSalesReturn " +
                        " FROM TransMemoSalesReturn memosr " +
                        " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " +
                        " WHERE memosr.MemoID = " + ReadyString(proj.SalesReturnMemoID);

                    //" WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                    //" AND memo.FOWSTID = devf.FowstID " &
                    //" AND memo.DeviceID = devf.DeviceID " &
                    //" AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                    //",0)

                    CurTotalVATSalesSalesReturn = CDec2(Sql.ExecuteScalar(pSql));

                    pSql = "SELECT SUM(memosr.TaxAmount) CurTotalVATAmountSalesReturn " +
                        " FROM TransMemoSalesReturn memosr " +
                        " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " +
                        " WHERE memosr.MemoID = " + ReadyString(proj.SalesReturnMemoID);

                    //" WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                    //" AND memo.FOWSTID = devf.FowstID " &
                    //" AND memo.DeviceID = devf.DeviceID " &
                    //" AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                    //",0)"

                    CurTotalVATAmountSalesReturn = CDec2(Sql.ExecuteScalar(pSql));

                    pSql = "SELECT SUM(srledger.GrossRefundAmount) CurSalesReturnRefund " +
                        " FROM TransSalesReturnLedger srledger " +
                        " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                        " WHERE srledger.SourceMemoID = " + ReadyString(proj.SalesReturnMemoID);

                    //" WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                    //" AND memo.FOWSTID = devf.FowstID " &
                    //" AND memo.DeviceID = devf.DeviceID) " &
                    //",0)"

                    CurSalesReturnRefund = CDec2(Sql.ExecuteScalar(pSql));

                    pSql = "SELECT SUM(srledger.GrossSrvAmount) CurSalesReturnExchange " +
                        " FROM TransSalesReturnLedger srledger " +
                        " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                        " WHERE srledger.SourceMemoID = " + ReadyString(proj.SalesReturnMemoID);

                    //" WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                    //" AND memo.FOWSTID = devf.FowstID " &
                    //" AND memo.DeviceID = devf.DeviceID) " &
                    //",0)"

                    CurSalesReturnExchange = CDec2(Sql.ExecuteScalar(pSql));

                    dblTotalVATSalesSalesReturn += CDec2(CurTotalVATSalesSalesReturn);
                    dblTotalVATAmountSalesReturn += CDec2(CurTotalVATAmountSalesReturn);

                    dblAdjRefund += CDec2(CurSalesReturnRefund);
                    dblAdjSRV += CDec2(CurSalesReturnExchange);

                    var LastTotalData = Sql.Execute('SELECT * FROM cache.CacheCSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                        ' AND UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                        ' AND SalesDate = ' + ReadyString($.format.date(mDate, 'yyyy-MM-dd'))).Rows //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                    if (LastTotalData.length > 0) {
                        var LastTotal = LastTotalData[0];

                        dblTotalSales = CDec2(LastTotal.TotalSales);
                        dblTotalSalesDiscount = CDec2(LastTotal.TotalSalesDiscount);

                        dblCash = CDec2(LastTotal.Cash);
                        dblCheck = CDec2(LastTotal.Check);
                        dblDebitCard = CDec2(LastTotal.DebitCard);
                        dblCreditCard = CDec2(LastTotal.CreditCard);
                        dblSRV = CDec2(LastTotal.SRV);

                        dblNonVAT = CDec2(LastTotal.NonVAT);
                        dblVATExempt = CDec2(LastTotal.VATExempt);
                        dblZeroRatedVAT = CDec2(LastTotal.ZeroRatedVAT);

                        dblVAT = CDec2(LastTotal.VAT);
                        dblVATAmount = CDec2(LastTotal.VATAmount);
                    }

                    dblVAT = Math.abs(CDec2(dblVAT) - CDec2(dblTotalVATSalesSalesReturn));
                    dblVATAmount = Math.abs(CDec2(dblVATAmount) - CDec2(dblTotalVATAmountSalesReturn));

                    var cachedata = {
                        TotalSales: dblTotalSales,
                        TotalSalesDiscount: dblTotalSalesDiscount,
                        AdjRefund: dblAdjRefund,
                        AdjSRV: dblAdjSRV,
                        Cash: dblCash,
                        Check: dblCheck,
                        DebitCard: dblDebitCard,
                        CreditCard: dblCreditCard,
                        SRV: dblSRV,
                        VAT: dblVAT,
                        VATAmount: dblVATAmount,
                        NonVAT: dblNonVAT,
                        VATExempt: dblVATExempt,
                        ZeroRatedVAT: dblZeroRatedVAT,
                        TotalVATSales: dblTotalVATSales,
                        TotalVATAmount: dblTotalVATAmount,
                        TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturn,
                        TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturn,
                        SalesDate: $.format.date(mDate, 'yyyy-MM-dd')
                    }
                    console.log(cachedata)
                    //SaveToCacheTable(cachedata, 'CacheCSR');

                    //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                    //                    dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                    //                        dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                    //                        dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);

                    //MCR

                    LastTotalData = Sql.Execute('SELECT * FROM cache.CacheMSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                        ' AND SalesDate = ' + ReadyString($.format.date(mDate, 'yyyy-MM-dd'))).Rows //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                    if (LastTotalData.length > 0) {
                        var LastTotal = LastTotalData[0];

                        dblTotalSales = CDec2(LastTotal.TotalSales);
                        dblTotalSalesDiscount = CDec2(LastTotal.TotalSalesDiscount);

                        dblCash = CDec2(LastTotal.Cash);
                        dblCheck = CDec2(LastTotal.Check);
                        dblDebitCard = CDec2(LastTotal.DebitCard);
                        dblCreditCard = CDec2(LastTotal.CreditCard);
                        dblSRV = CDec2(LastTotal.SRV);

                        dblNonVAT = CDec2(LastTotal.NonVAT);
                        dblVATExempt = CDec2(LastTotal.VATExempt);
                        dblZeroRatedVAT = CDec2(LastTotal.ZeroRatedVAT);

                        dblVAT = CDec2(LastTotal.VAT);
                        dblVATAmount = CDec2(LastTotal.VATAmount);
                    }

                    dblVAT = Math.abs(CDec2(dblVAT) - CDec2(dblTotalVATSalesSalesReturn));
                    dblVATAmount = Math.abs(CDec2(dblVATAmount) - CDec2(dblTotalVATAmountSalesReturn));

                    cachedata = {
                        TotalSales: dblTotalSales,
                        TotalSalesDiscount: dblTotalSalesDiscount,
                        AdjRefund: dblAdjRefund,
                        AdjSRV: dblAdjSRV,
                        Cash: dblCash,
                        Check: dblCheck,
                        DebitCard: dblDebitCard,
                        CreditCard: dblCreditCard,
                        SRV: dblSRV,
                        VAT: dblVAT,
                        VATAmount: dblVATAmount,
                        NonVAT: dblNonVAT,
                        VATExempt: dblVATExempt,
                        ZeroRatedVAT: dblZeroRatedVAT,
                        TotalVATSales: dblTotalVATSales,
                        TotalVATAmount: dblTotalVATAmount,
                        TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturn,
                        TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturn,
                        SalesDate: $.format.date(mDate, 'yyyy-MM-dd')
                    }
                    //console.log(cachedata)
                    //return
                    //SaveToCacheTable(cachedata, 'CacheMSR');

                    //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                    //                    dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                    //                        dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                    //                        dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                }
                //} else { // comment out
                //    AjaxSend({
                //        url: "ReportsPOS/getCashierSalesTransactionSummary",
                //        data: pData,
                //        success: function (vDataInfo) {

                //            vDataInfo = vDataInfo.ReportPOS;


                //            for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                //                dblTotalVATSalesSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                //                dblTotalVATAmountSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);

                //                dblAdjRefund += CDec2(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                //                dblAdjSRV += CDec2(vDataInfo.Rows[p].CurSalesReturnExchange || 0);
                //            };
                                                        
                //            var LastTotalData = Sql.Execute('SELECT * FROM cache.CacheCSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                //                                ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                //                                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                //                                ' AND UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                //                                ' AND SalesDate = ' + ReadyString($.format.date(mDate, 'yyyy-MM-dd'))).Rows //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                //            if (LastTotalData.length > 0) {
                //                var LastTotal = LastTotalData[0];

                //                dblTotalSales += CDec2(LastTotal.TotalSales);
                //                dblTotalSalesDiscount += CDec2(LastTotal.TotalSalesDiscount);
                //                //dblAdjRefund = CDec2(LastTotal[2]); //value of this variable is being set above
                //                //dblAdjSRV = CDec2(LastTotal[3]); //value of this variable is being set above

                //                dblCash += CDec2(LastTotal.Cash) - dblAdjRefund;
                //                dblCheck += CDec2(LastTotal.Check);
                //                dblDebitCard += CDec2(LastTotal.DebitCard);
                //                dblCreditCard += CDec2(LastTotal.CreditCard);
                //                dblSRV += CDec2(LastTotal.SRV);

                //                //dblVAT -= CDec2(LastTotal[9]);
                //                //dblVATAmount -= CDec2(LastTotal[10]);
                //                dblNonVAT += CDec2(LastTotal.NonVAT);
                //                dblVATExempt += CDec2(LastTotal.VATExempt);
                //                dblZeroRatedVAT += CDec2(LastTotal.ZeroRatedVAT);

                //                dblTotalVATSales += CDec2(LastTotal.TotalVATSales);
                //                dblTotalVATAmount += CDec2(LastTotal.TotalVATAmount);

                //                //dblTotalVATSalesSalesReturn //value of this variable is being set above
                //                //dblTotalVATAmountSalesReturn //value of this variable is being set above
                //            }

                //            dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                //            dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                //            var cachedata = {
                //                TotalSales: dblTotalSales,
                //                TotalSalesDiscount: dblTotalSalesDiscount,
                //                AdjRefund: dblAdjRefund,
                //                AdjSRV: dblAdjSRV,
                //                Cash: dblCash,
                //                Check: dblCheck,
                //                DebitCard: dblDebitCard,
                //                CreditCard: dblCreditCard,
                //                SRV: dblSRV,
                //                VAT: dblVAT,
                //                VATAmount: dblVATAmount,
                //                NonVAT: dblNonVAT,
                //                VATExempt: dblVATExempt,
                //                ZeroRatedVAT: dblZeroRatedVAT,
                //                TotalVATSales: dblTotalVATSales,
                //                TotalVATAmount: dblTotalVATAmount,
                //                TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturn,
                //                TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturn,
                //                SalesDate: $.format.date(mDate, 'yyyy-MM-dd')
                //            }

                //            SaveToCacheTable(cachedata, 'CacheCSR');

                //            //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                //            //                    dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                //            //                        dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                //            //                        dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                //        }
                //    });

                //    AjaxSend({
                //        url: "ReportsPOS/getManagerSalesTransactionSummary",
                //        data: pData,
                //        success: function (vDataInfo) {

                //            vDataInfo = vDataInfo.ReportPOS;

                //            var dblTotalSales = 0;
                //            var dblTotalSalesDiscount = 0;
                //            var dblAdjRefund = 0;
                //            var dblAdjSRV = 0;
                //            var dblNetSales = 0;

                //            var dblCash = 0;
                //            var dblCheck = 0;
                //            var dblDebitCard = 0;
                //            var dblCreditCard = 0;
                //            var dblSRV = 0;
                //            var dblTotalPayment = 0;

                //            var dblVAT = 0;
                //            var dblVATAmount = 0;
                //            var dblNonVAT = 0;
                //            var dblVATExempt = 0;
                //            var dblZeroRatedVAT = 0;
                //            var dblTotalVAT = 0;

                //            var dblTotalVATSales = 0;
                //            var dblTotalVATAmount = 0;
                //            var dblTotalVATSalesSalesReturn = 0;
                //            var dblTotalVATAmountSalesReturn = 0;

                //            for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                //                dblTotalVATSalesSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                //                dblTotalVATAmountSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);

                //                dblAdjRefund += CDec2(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                //                dblAdjSRV += CDec2(vDataInfo.Rows[p].CurSalesReturnExchange || 0);
                //            };

                //            var LastTotalData = Sql.Execute('SELECT * FROM cache.CacheMSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                //                                ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                //                                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                //                                ' AND SalesDate = ' + ReadyString($.format.date(mDate, 'yyyy-MM-dd'))).Rows //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                //            if (LastTotalData.length > 0) {
                //                var LastTotal = LastTotalData[0];

                //                dblTotalSales += CDec2(LastTotal.TotalSales);
                //                dblTotalSalesDiscount += CDec2(LastTotal.TotalSalesDiscount);
                //                //dblAdjRefund = CDec2(LastTotal[2]); //value of this variable is being set above
                //                //dblAdjSRV = CDec2(LastTotal[3]); //value of this variable is being set above

                //                dblCash += CDec2(LastTotal.Cash) - dblAdjRefund;
                //                dblCheck += CDec2(LastTotal.Check);
                //                dblDebitCard += CDec2(LastTotal.DebitCard);
                //                dblCreditCard += CDec2(LastTotal.CreditCard);
                //                dblSRV += CDec2(LastTotal.SRV);

                //                //dblVAT -= CDec2(LastTotal[9]);
                //                //dblVATAmount -= CDec2(LastTotal[10]);
                //                dblNonVAT += CDec2(LastTotal.NonVAT);
                //                dblVATExempt += CDec2(LastTotal.VATExempt);
                //                dblZeroRatedVAT += CDec2(LastTotal.ZeroRatedVAT);

                //                dblTotalVATSales += CDec2(LastTotal.TotalVATSales);
                //                dblTotalVATAmount += CDec2(LastTotal.TotalVATAmount);

                //                //dblTotalVATSalesSalesReturn //value of this variable is being set above
                //                //dblTotalVATAmountSalesReturn //value of this variable is being set above
                //            }

                //            dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                //            dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                //            var cachedata = {
                //                TotalSales: dblTotalSales,
                //                TotalSalesDiscount: dblTotalSalesDiscount,
                //                AdjRefund: dblAdjRefund,
                //                AdjSRV: dblAdjSRV,
                //                Cash: dblCash,
                //                Check: dblCheck,
                //                DebitCard: dblDebitCard,
                //                CreditCard: dblCreditCard,
                //                SRV: dblSRV,
                //                VAT: dblVAT,
                //                VATAmount: dblVATAmount,
                //                NonVAT: dblNonVAT,
                //                VATExempt: dblVATExempt,
                //                ZeroRatedVAT: dblZeroRatedVAT,
                //                TotalVATSales: dblTotalVATSales,
                //                TotalVATAmount: dblTotalVATAmount,
                //                TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturn,
                //                TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturn,
                //                SalesDate: $.format.date(mDate, 'yyyy-MM-dd')
                //            }

                //            SaveToCacheTable(cachedata, 'CacheMSR');

                //            //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                //            //                    dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                //            //                        dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                //            //                        dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                //        }
                //    });
                //}123abc
            }

            function SaveToCacheTable(data, table) {
                var pIU = new InsertUpdate('cache.' + table);

                pIU.Add('TotalSales', CDec2(data.TotalSales));
                pIU.Add('TotalSalesDiscount', CDec2(data.TotalSalesDiscount));
                pIU.Add('AdjRefund', 'AdjRefund + ' + CDec2(data.AdjRefund));
                pIU.Add('AdjSRV', 'AdjSRV + ' + CDec2(data.AdjSRV));
                pIU.Add('Cash', CDec2(data.Cash));
                pIU.Add('[Check]', CDec2(data.Check));
                pIU.Add('DebitCard', CDec2(data.DebitCard));
                pIU.Add('CreditCard', CDec2(data.CreditCard));
                pIU.Add('TotalSales', CDec2(data.TotalSales));
                pIU.Add('TotalSalesDiscount', CDec2(data.TotalSalesDiscount));
                pIU.Add('SRV', CDec2(data.SRV));
                pIU.Add('VAT', CDec2(data.VAT));
                pIU.Add('VATAmount', CDec2(data.VATAmount));
                pIU.Add('NonVAT', CDec2(data.NonVAT));
                pIU.Add('VATExempt', CDec2(data.VATExempt));
                pIU.Add('ZeroRatedVAT', CDec2(data.ZeroRatedVAT));
                pIU.Add('TotalVATSales', 'TotalVATSales + ' + CDec2(data.TotalVATSales));
                pIU.Add('TotalVATAmount', 'TotalVATAmount + ' + CDec2(data.TotalVATAmount));
                pIU.Add('TotalSalesDiscount', CDec2(data.TotalSalesDiscount));
                pIU.Add('TotalVATSalesSalesReturn', 'TotalVATSalesSalesReturn + ' + CDec2(data.TotalVATSalesSalesReturn));
                pIU.Add('TotalVATAmountSalesReturn', 'TotalVATAmountSalesReturn + ' + CDec2(data.TotalVATAmountSalesReturn));
                pIU.Add('SalesDate', ReadyString(data.SalesDate));
                pIU.Add('OrgID', ReadyString(sessionStorage.OrgID));
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('DeviceID', ReadyString(Web.DeviceID()));

                if (table == 'CacheCSR') {
                    pSql = pIU.GetUpdate(' OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                                        ' AND UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                                        ' AND SalesDate = ' + ReadyString(data.SalesDate));

                    if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                        pIU.Add('AdjRefund', CDec2(data.AdjRefund));
                        pIU.Add('AdjSRV', CDec2(data.AdjSRV));
                        pIU.Add('TotalVATSales', CDec2(data.TotalVATSales));
                        pIU.Add('TotalVATAmount', CDec2(data.TotalVATAmount));
                        pIU.Add('TotalVATSalesSalesReturn', CDec2(data.TotalVATSalesSalesReturn));
                        pIU.Add('TotalVATAmountSalesReturn', CDec2(data.TotalVATAmountSalesReturn));
                        pIU.Add("ID", ReadyString(NewID()));
                        pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql)
                    }
                } else {
                    pSql = pIU.GetUpdate(' OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                                        ' AND SalesDate = ' + ReadyString(data.SalesDate));

                    if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                        pIU.Add('AdjRefund', CDec2(data.AdjRefund));
                        pIU.Add('AdjSRV', CDec2(data.AdjSRV));
                        pIU.Add('TotalVATSales', CDec2(data.TotalVATSales));
                        pIU.Add('TotalVATAmount', CDec2(data.TotalVATAmount));
                        pIU.Add('TotalVATSalesSalesReturn', CDec2(data.TotalVATSalesSalesReturn));
                        pIU.Add('TotalVATAmountSalesReturn', CDec2(data.TotalVATAmountSalesReturn));
                        pIU.Add("ID", ReadyString(NewID()));
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql)
                    }
                }
            }

            window.UpdateZReadCache = UpdateZReadCache;
            function UpdateZReadCache() {
                var pData = {
                    RangeType: 'Today',
                    ZDate: new Date(),
                    DeviceID: Web.DeviceID()
                }

                //if (proj.SalesReturnFromOffline || mIsRefund) {                   
                if (IsMobile) {                   
                    var dtMemoData;

                    var pSql = '';

                    pSql = "SELECT " +
                            "(SELECT SUM(srledger.GrossRefundAmount) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID) CurSalesReturnRefund " +

                            ",(SELECT SUM(srledger.GrossSrvAmount) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID) CurSalesReturnExchange " +

                            ",(SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0)) " +
                               " FROM TransMemoSalesReturn memosr " +
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID) CurTotalVATSalesSalesReturn " +
                               
                            ",(SELECT SUM(memosr.TaxAmount) " +
                               " FROM TransMemoSalesReturn memosr " +
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID) CurTotalVATAmountSalesReturn " +

                            ",(SELECT DISTINCT COUNT(memo.MemoNumber) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID " +
                               " AND srledger.DestinationPaymentID IS NULL " +
                               " AND srledger.SrvNum > 0) CurTransCountMemoReturnExchange " +

                            ",(SELECT COUNT(memoret.ProductID) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID " +
                               " AND srledger.DestinationPaymentID IS NULL " +
                               " AND srledger.SrvNum > 0) CurSalesReturnExchangeItemCount " +

                            ",(SELECT DISTINCT COUNT(memo.MemoNumber) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID " +
                               " AND srledger.DestinationPaymentID IS NULL " +
                               " AND srledger.RefundNum > 0) CurTransCountMemoRefund " +
                               
                            ",(SELECT COUNT(memoret.ProductID) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " +
                               " WHERE memo.ID = " + ReadyString(proj.SalesReturnMemoID) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID " +
                               " AND srledger.DestinationPaymentID IS NULL " +
                               " AND srledger.RefundNum > 0) CurSalesRefundItemCount " + 

                            ", dev.DeviceNo CurTermNum " +
                            ", fowst.FOWSTNo " +
                            " FROM LibraryDeviceFowst devf " +
                            " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) " +
                            " INNER JOIN LibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) " +
                            " WHERE devf.FOWSTID = " + ReadyString(sessionStorage.FowstID) + " AND devf.DeviceID = " + ReadyString(Web.DeviceID())

                    dtMemoData = Sql.Execute(pSql);

                    SaveZReadCacheData(dtMemoData);

                } else {

                    AjaxSend({
                        url: "SalesReturns/GetSalesReturnDetails",
                        data: {MemoID: proj.SalesReturnMemoID},
                        success: function (vDataInfo) {
                            SaveZReadCacheData(vDataInfo.Results);
                        }
                    });

                    //AjaxSend({
                    //    url: "ReportsPOS/getSalesZReportSummary",
                    //    data: pData,
                    //    success: function (vDataInfo) {

                    //        vDataInfo = vDataInfo.ReportPOS;

                    //        var pIU = new InsertUpdate('CacheZRead');
                    //        var pSql;

                    //        pSql = "Delete from CacheZRead where SalesDate = " + ReadyString($.format.date(new Date(), 'yyyy-MM-dd'));
                    //        Sql.Execute(pSql);

                    //        pIU.Add('ID', ReadyString(vDataInfo.Rows[0].ID))
                    //        pIU.Add('PrevGrossSales', CDec2(vDataInfo.Rows[0].PrevGrossSales))
                    //        pIU.Add('PrevTotalDiscount', CDec2(vDataInfo.Rows[0].PrevTotalDiscount))
                    //        pIU.Add('PrevSalesReturnRefund', CDec2(vDataInfo.Rows[0].PrevSalesReturnRefund))
                    //        pIU.Add('PrevSalesReturnExchange', CDec2(vDataInfo.Rows[0].PrevSalesReturnExchange))
                    //        pIU.Add('PrevTotalVAT', CDec2(vDataInfo.Rows[0].PrevTotalVAT))
                    //        pIU.Add('PrevServiceCharge', CDec2(vDataInfo.Rows[0].PrevServiceCharge))
                    //        pIU.Add('PrevChargeAmount', CDec2(vDataInfo.Rows[0].PrevChargeAmount))
                    //        pIU.Add('SalesDate', ReadyString($.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'yyyy-MM-dd')));
                    //        pIU.Add('PresentationCurrGrossSales', CDec2(vDataInfo.Rows[0].CurGrossSales));
                    //        pIU.Add('CurGrossSales', CDec2(vDataInfo.Rows[0].CurGrossSales));
                    //        pIU.Add('CurTotalDiscount', CDec2(vDataInfo.Rows[0].CurTotalDiscount));
                    //        pIU.Add('CurTotalVATSales', CDec2(vDataInfo.Rows[0].CurTotalVATSales));
                    //        pIU.Add('CurTotalVAT', CDec2(vDataInfo.Rows[0].CurTotalVAT));
                    //        pIU.Add('CurLocalTax', CDec2(vDataInfo.Rows[0].CurLocalTax));
                    //        pIU.Add('CurServiceCharge', CDec2(vDataInfo.Rows[0].CurServiceCharge));
                    //        pIU.Add('CurNoTAXSales', CDec2(vDataInfo.Rows[0].CurNoTAXSales));
                    //        pIU.Add('CurSalesReturnRefund', CDec2(vDataInfo.Rows[0].CurSalesReturnRefund));
                    //        pIU.Add('CurSalesReturnExchange', CDec2(vDataInfo.Rows[0].CurSalesReturnExchange));
                    //        pIU.Add('CurSCDiscount', CDec2(vDataInfo.Rows[0].CurSCDiscount));
                    //        pIU.Add('CurVATExempt', CDec2(vDataInfo.Rows[0].CurVATExempt));
                    //        pIU.Add('CurTotalVATSalesSalesReturn', CDec2(vDataInfo.Rows[0].CurTotalVATSalesSalesReturn));
                    //        pIU.Add('CurTotalVATAmountSalesReturn', CDec2(vDataInfo.Rows[0].CurTotalVATAmountSalesReturn));
                    //        pIU.Add('OnCreditAmount', CDec2(vDataInfo.Rows[0].OnCreditAmount));
                    //        pIU.Add('CurBeginningInvoiceNo', CDec2(vDataInfo.Rows[0].CurBeginningInvoiceNo));
                    //        pIU.Add('CurEndingInvoiceNo', CDec2(vDataInfo.Rows[0].CurEndingInvoiceNo));
                    //        pIU.Add('AmountBeforeAdjustment', CDec2(vDataInfo.Rows[0].AmountBeforeAdjustment));
                    //        pIU.Add('DateOfAdjustment', (CStr2(vDataInfo.Rows[0].DateOfAdjustment)) == '' ? 'NULL' : $.format.date(new Date(vDataInfo.Rows[0].DateOfAdjustment), 'yyyy-MM-dd'));
                    //        pIU.Add('EditedBy', ReadyString(CStr2(vDataInfo.Rows[0].EditedBy)));
                    //        pIU.Add('SRVAmount', CDec2(vDataInfo.Rows[0].SRVAmount));
                    //        pIU.Add('CurVATExemptSales', CDec2(vDataInfo.Rows[0].CurVATExemptSales));
                    //        pIU.Add('CurZeroRatedVATSales', CDec2(vDataInfo.Rows[0].CurZeroRatedVATSales));
                    //        pIU.Add('CurOthers', CDec2(vDataInfo.Rows[0].CurOthers));
                    //        pIU.Add('CashAmount', CDec2(vDataInfo.Rows[0].CashAmount));
                    //        pIU.Add('DebitCardAmount', CDec2(vDataInfo.Rows[0].DebitCardAmount));
                    //        pIU.Add('CreditCardAmount', CDec2(vDataInfo.Rows[0].CreditCardAmount));
                    //        pIU.Add('SMACAmount', CDec2(vDataInfo.Rows[0].SMACAmount));
                    //        pIU.Add('CheckPaymentAmount', CDec2(vDataInfo.Rows[0].CheckPaymentAmount));
                    //        pIU.Add('OtherTypeAmount', CDec2(vDataInfo.Rows[0].OtherTypeAmount));
                    //        pIU.Add('EPlusAmount', CDec2(vDataInfo.Rows[0].EPlusAmount));
                    //        pIU.Add('VoucherAmount', CDec2(vDataInfo.Rows[0].VoucherAmount));
                    //        pIU.Add('CurBegInvoiceNo', vDataInfo.Rows[0].CurBegInvoiceNo);
                    //        pIU.Add('CurEndInvoiceNo', vDataInfo.Rows[0].CurEndInvoiceNo);
                    //        pIU.Add('CurTransCountInv', CDec2(vDataInfo.Rows[0].CurTransCountInv));
                    //        pIU.Add('CurTransInvItemCount', CDec2(vDataInfo.Rows[0].CurTransInvItemCount));
                    //        pIU.Add('CurTransCountMemoReturnExchange', CDec2(vDataInfo.Rows[0].CurTransCountMemoReturnExchange));
                    //        pIU.Add('CurSalesReturnExchangeItemCount', CDec2(vDataInfo.Rows[0].CurSalesReturnExchangeItemCount));
                    //        pIU.Add('CurTransCountMemoRefund', CDec2(vDataInfo.Rows[0].CurTransCountMemoRefund));
                    //        pIU.Add('CurSalesRefundItemCount', CDec2(vDataInfo.Rows[0].CurSalesRefundItemCount));
                    //        pIU.Add('CurTermNum', CDec2(vDataInfo.Rows[0].CurTermNum));
                    //        pIU.Add('FowstNo', CDec2(vDataInfo.Rows[0].FowstNo));
                    //        pIU.Add('DeviceID', ReadyString(vDataInfo.Rows[0].DeviceID));
                    //        pIU.Add('FOWSTID', ReadyString(vDataInfo.Rows[0].FOWSTID));

                    //        pSql = pIU.GetInsert();
                    //        Sql.Execute(pSql);
                    //    }
                    //});
                }

                function SaveZReadCacheData(dtMemoData) {
                    var IsNew = false;
                    var dtCurrZRead;
                    var dtPrevZRead;


                    var pIU = new InsertUpdate('CacheZRead');

                    pSql = "Select * FROM CacheZRead " +
                            " WHERE date(SalesDate) = " + ReadyString($.format.date(new Date(), 'yyyy-MM-dd')) +
                            " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                            " AND DeviceID = " + ReadyString(Web.DeviceID())


                    dtCurrZRead  = Sql.Execute(pSql);

                    if(dtCurrZRead.Rows.length == 0){
                        IsNew = true;

                        pSql = "Select * FROM CacheZRead " +
                                    " WHERE date(SalesDate) < " + ReadyString($.format.date(new Date(), 'yyyy-MM-dd')) +
                                    " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                    " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                                    " Order by SalesDate desc limit 1"

                        dtPrevZRead  = Sql.Execute(pSql);

                    }
                    
                    if (dtPrevZRead == null)
                        dtPrevZRead = {
                            Rows: [{}]
                        };

                    if (dtPrevZRead.Rows[0] == null)
                        dtPrevZRead.Rows[0] = {};

                    if (dtPrevZRead.Rows.length == 0)
                        dtPrevZRead.Rows.push({});

                    if (IsNew) {

                        pIU.Add('ID', ReadyString(NewID()));
                        pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                        pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));

                        if (CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) == 0){
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        }
                        else{
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) + CDec2(dtPrevZRead.Rows[0].PresentationCurrGrossSales))
                        }                    

                        pIU.Add('PrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales));
                        pIU.Add('PrevTotalDiscount', CDec2(dtPrevZRead.Rows[0].PrevTotalDiscount) + CDec2(dtPrevZRead.Rows[0].CurTotalDiscount));
                        pIU.Add('PrevSalesReturnRefund', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnRefund) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnRefund));
                        pIU.Add('PrevSalesReturnExchange', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnExchange) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnExchange));
                        pIU.Add('PrevTotalVAT', CDec2(dtPrevZRead.Rows[0].PrevTotalVAT) + CDec2(dtPrevZRead.Rows[0].CurTotalVAT));
                        pIU.Add('PrevServiceCharge', CDec2(dtPrevZRead.Rows[0].PrevServiceCharge) + CDec2(dtPrevZRead.Rows[0].CurServiceCharge));
                        pIU.Add('PrevChargeAmount', CDec2(dtPrevZRead.Rows[0].PrevChargeAmount) + CDec2(dtPrevZRead.Rows[0].ChargeAmount));

                        pIU.Add('SalesDate', ReadyString($.format.date(new Date(), 'yyyy-MM-dd')));

                        pIU.Add('CurSalesReturnRefund', CDec2(dtMemoData.Rows[0].CurSalesReturnRefund));
                        pIU.Add('CurSalesReturnExchange', CDec2(dtMemoData.Rows[0].CurSalesReturnExchange));
                        pIU.Add('CurTotalVATSalesSalesReturn', CDec2(dtMemoData.Rows[0].CurTotalVATSalesSalesReturn));
                        pIU.Add('CurTotalVATAmountSalesReturn', CDec2(dtMemoData.Rows[0].CurTotalVATAmountSalesReturn));
                        pIU.Add('CurTransCountMemoReturnExchange', CDec2(dtMemoData.Rows[0].CurTransCountMemoReturnExchange));
                        pIU.Add('CurSalesReturnExchangeItemCount', CDec2(dtMemoData.Rows[0].CurSalesReturnExchangeItemCount));
                        pIU.Add('CurTransCountMemoRefund', CDec2(dtMemoData.Rows[0].CurTransCountMemoRefund));
                        pIU.Add('CurSalesRefundItemCount', CDec2(dtMemoData.Rows[0].CurSalesRefundItemCount));
                        pIU.Add('CurTermNum', CInt2(dtMemoData.Rows[0].CurTermNum));
                        pIU.Add('FowstNo', CInt2(dtMemoData.Rows[0].FowstNo));
                        
                    }
                    else{

                        pIU.Add('CurSalesReturnRefund', CDec2(dtCurrZRead.Rows[0].CurSalesReturnRefund) + CDec2(dtMemoData.Rows[0].CurSalesReturnRefund));
                        pIU.Add('CurSalesReturnExchange', CDec2(dtCurrZRead.Rows[0].CurSalesReturnExchange) + CDec2(dtMemoData.Rows[0].CurSalesReturnExchange));
                        pIU.Add('CurTotalVATSalesSalesReturn', CDec2(dtCurrZRead.Rows[0].CurTotalVATSalesSalesReturn) + CDec2(dtMemoData.Rows[0].CurTotalVATSalesSalesReturn));
                        pIU.Add('CurTotalVATAmountSalesReturn', CDec2(dtCurrZRead.Rows[0].CurTotalVATAmountSalesReturn) + CDec2(dtMemoData.Rows[0].CurTotalVATAmountSalesReturn));
                        pIU.Add('CurTransCountMemoReturnExchange', CDec2(dtCurrZRead.Rows[0].CurTransCountMemoReturnExchange) + CDec2(dtMemoData.Rows[0].CurTransCountMemoReturnExchange));
                        pIU.Add('CurSalesReturnExchangeItemCount', CDec2(dtCurrZRead.Rows[0].CurSalesReturnExchangeItemCount) + CDec2(dtMemoData.Rows[0].CurSalesReturnExchangeItemCount));
                        pIU.Add('CurTransCountMemoRefund', CDec2(dtCurrZRead.Rows[0].CurTransCountMemoRefund) + CDec2(dtMemoData.Rows[0].CurTransCountMemoRefund));
                        pIU.Add('CurSalesRefundItemCount', CDec2(dtCurrZRead.Rows[0].CurSalesRefundItemCount) + CDec2(dtMemoData.Rows[0].CurSalesRefundItemCount));
                    }

                    if (IsNew) {
                        pSql = pIU.GetInsert()
                    }
                    else{
                        pSql = pIU.GetUpdate("ID=" + ReadyString(dtCurrZRead.Rows[0].ID));
                    }

                    Sql.Execute(pSql);
                }
            }


        }); // $(function() {

    });
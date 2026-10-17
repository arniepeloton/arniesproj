require(['common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc='),
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''),
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

            $(document).ready(function () {

                if (!window.InAndroid) {
                    $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                }

            });

            (function Init() {
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

                    location = "/People/PeopleMenu";

                });

                $('#butGotoSalesReturn').click(function(e) {
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/SalesReturns');
                });

                $('#butGotoPos').click(function(e) {
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0,
                        mode: 'pos',
                    }, '/SalesMenu');
                });

                $('#butGotoMainMenu').click(function(e) {
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/People/PeopleMenu');
                });

		        $('#btnDateFrom').click(function(e){
                    e.preventDefault();
                    SingleCalendar({
                        Click: function(v){
                            $('#btnDateFrom').text(v).button('refresh');
                        }
                    })
                })

                $('#btnDateTo').click(function(e){
                    e.preventDefault();
                    SingleCalendar({
                        Click: function(v){
                            $('#btnDateTo').text(v).button('refresh');
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

                pPage.css('padding-top', pHeight + 'px');

                if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                    pHeader.fixedtoolbar({ tapToggle: false,
                        updatePagePadding: false
                    });
                $('#btnDateFrom').text($.format.date(new Date(), 'MMM dd, yyyy')).button('refresh');
                $('#btnDateTo').text($.format.date(new Date(), 'MMM dd, yyyy')).button('refresh');

                $('#txtDateFrom').DateCombo('val', '1/1/1900');
                $('#txtDateTo').DateCombo('val', new Date());

                Ajax_SalesReturns_GetSettings({
                    success: function (vSRVSettingsData) {
                        mAllowOfflineSrvPayments = vSRVSettingsData.AllowOfflineSrvPayments;
                    }
                });
                App.Deferred.fulfill('R87MBPXGSQ');
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
                Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                Browser.LibraryFowst.InputData.FilterFowst = 1;
                Browser.LibraryFowst.Click = function (e) {
                    sessionStorage.FowstID = e.ID;
                    proj.SrvFowstID = e.ID;
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
                };
                Browser.LibraryFowst.Show();

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

                if (CStr2(sessionStorage.FowstID).trim() == '') {
                    BrowseAddresses();
                    return;
                }
                
                Browser.OfflineMode = false;
                Browser.TransInvoicesReturnables.Click = function (e) {
                    proj.SalesReturnFromOffline = false;
                    LoadStocks(e.ID);
                };
                Browser.TransInvoicesReturnables.InputData.FastMode = 1;
                Browser.TransInvoicesReturnables.InputData.DateFrom = FormatDate($('#btnDateFrom').text()); //FormatDate($('#txtDateFrom').DateCombo('val'));
                Browser.TransInvoicesReturnables.InputData.DateTo = FormatDate($('#btnDateTo').text()); //FormatDate($('#txtDateTo').DateCombo('val'));
                Browser.TransInvoicesReturnables.InputData.DeviceID = Web.DeviceID();
                Browser.TransInvoicesReturnables.Show($('#txtSearch').val(), {
                    description: mAllowOfflineSrvPayments ? 'Due to the ability to generate and consume offline SRVs, invoices shown here are limited to those that have been directly transacted in your specific POS machine+Business Location Data' : ''
                });
            }

            function BrowseOfflineInvoices() {

                if (CStr2(sessionStorage.FowstID).trim() == '') {
                    BrowseAddresses();
                    return;
                }

                BrowsePopup('TransInvoices', 'Sales Invoices', [
                    {
                        Offline: true,
                        AutoSearch: true,
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
                        DeviceID: Web.DeviceID()
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

            function LoadStocks(vInvoiceID) {

                AjaxSend({
                    url: 'SalesReturns/GetContents',
                    data: { 
                        InvoiceID: vInvoiceID, // this takes precedence over MemoID
                        MemoID: proj.SalesReturnMemoID
                    },
                    success: function (vData) {
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

            function LoadCart() {

                if (proj.SalesReturnFromOffline) {
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

                    pDivTemplate.hide();

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
                $.mobile.changePage(mPage);
                $('[data-href=Nav1b]').click();
            }; // proj.ShowCart = function() {

            $('#butCheckOutHeader').click(function () {

                if (CStr2(proj.SalesReturnMemoID).length == 0) {
                    MessageBox('Please return at least 1 item');
                    return;
                };

                if (proj.SalesReturnFromOffline) {
                    mCheckOutVerify = {};
                    $('#divCheckOutMessage').text('You are about to check out this sales return cart. Please verify the following info: Total Returned Amount: ' + $.format.number(CDec2(mCheckOutVerify.TotalAmount), '#,###.00') + '; SKU Count: ' + mCheckOutVerify.SkuCount);
                    //$('#divCheckOutMessage').text('You are about to check out this sales return cart.');
                    $.mobile.changePage('#divCheckOut');
                } else 
                    AjaxSend({
                        url: 'SalesReturns/Verify',
                        data: { MemoID: proj.SalesReturnMemoID },
                        success: function (vData) {
                            if ((vData) && (vData.Result == 'ok')) {
                                mCheckOutVerify = vData;
                                $('#divCheckOutMessage').text('You are about to check out this sales return cart. Please verify the following info: Total Returned Amount: ' + $.format.number(CDec2(vData.TotalAmount), '#,###.00') + '; SKU Count: ' + vData.SkuCount);
                                $.mobile.changePage('#divCheckOut');
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
                            $.mobile.changePage('#divCheckOut');
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

            }; // function Clear() {

            $('[name=butCheckOutNow]').click(function () {

                var pIsRefund = $(this).attr('id') == 'butCheckOutRefund' ? 1 : 0;
                //if (pIsRefund && mUseOfflineData) {
                //    NewMessageBox('Refund is not yet implemented while using offline data. To issue a refund, this device must be connected to Mobiz server.', 'Refund not available');
                //    return;
                //}

                $('[name=butCheckOutNow]').hide();

                var pData = { TotalAmount: mCheckOutVerify.TotalAmount,
                    SkuCount: mCheckOutVerify.SkuCount,
                    Refund: pIsRefund
                };
                if (IsMobile)
                    pData.DeviceSerial = Device.Serial();

                if (proj.SalesReturnFromOffline) {
                    $('[name=butCheckOutNow]').show();
                    Business.TransSalesReturn.Approve(proj.SalesReturnMemoID);
                    Business.TransSalesReturn.SaveToSalesReturnLedger(proj.SalesReturnMemoID, pIsRefund);
                    UpdateSalesTextFile();
                    Print();
                } else 
                    CheckOut(pData);

                function CheckOut(vData) {

                    mIsRefund = vData.Refund;
                    vData.MemoID = proj.SalesReturnMemoID;
                    AjaxSend({
                        url: 'SalesReturns/CheckOut',
                        data: vData,
                        success: function (vData) {
                            $('[name=butCheckOutNow]').show();
                            if ((vData) && (vData.Result == 'ok')) {
                                try {                                    

                                    if (CBool(vData.SalesReturnRequiresApproval)) {
                                        $('#spanMemoNum').text(vData.Num);
                                        $.mobile.changePage('#divSalesReturnRequiresApproval');
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
                                        }
                                        SaveToInventoryLedger();
                                    }
                                    
                                    Print();
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

            function Print() {
                if (mIsRefund == 1) {                                        
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
                            } else {
                                PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                            }
                        } else {
                            PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);
                        }                                 
                    } else {
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
                            } else {
                                PrintPOSSRVReceipt(proj.SalesReturnMemoID);
                            }
                        } else {
                            PrintPOSSRVReceipt(proj.SalesReturnMemoID);
                        } 
                    } else {

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
                $('#butGetApprovalStatus').button('disable'); // http://stackoverflow.com/a/5875143
                AjaxSend({
                    url: 'SalesReturns/GetApprovalStatus',
                    timeout: 30 * 1000,
                    data: {
                        MemoID: proj.SalesReturnMemoID
                    },
                    success: function(vData) {
                        $('#butGetApprovalStatus').button('enable')
                        if (vData.Error) {
                            $('#divGetApprovalResult').text(vData.Error);
                            LaunchTimer();
                        } else if (vData.Result == 'denied')  {
                            clearTimeout(mApprovalTimer);
                            $('#butGetApprovalStatus').button('disable'); // http://stackoverflow.com/a/5875143
                            $('#butApproveNow').button('disable');
                            $('#divGetApprovalResult').text('Success. This transaction has been disapproved.');
                        } else if (vData.Result == 'approved')  {
                            clearTimeout(mApprovalTimer);
                            $('#butGetApprovalStatus').button('disable'); // http://stackoverflow.com/a/5875143
                            $('#butApproveNow').button('disable');
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
                        $('#butGetApprovalStatus').button('enable')
                        $('#divGetApprovalResult').text('Cannot connect to server. Please try again later.');
                        LaunchTimer();
                    }
                });

                function LaunchTimer() {
                    
                    mApprovalTimer = setTimeout(GetApproval, 10000);

                } // END function LaunchTimer() {
            } // END function GetApproval() {

            $('#divCheckOutBack').click(function () {
                history.back();
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
                                        MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                               "Receipt Settings",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
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
                                    var pMessage = 'SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage,
                                           "Printer",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                }
                            });

                            
                        }

                    }
                });

            }; // END function PrintPOSSRVReceipt(pMemoID) {

            function Ajax_Organization_getOrgFOWSTPrinterIP(vParams) {
                if (proj.SalesReturnFromOffline) {
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
                if (proj.SalesReturnFromOffline) {
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

            function Ajax_SalesPayments_getSalesPaymentSRVReceiptHeaderInfo(vParams) {
                if (proj.SalesReturnFromOffline) {
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
                if (proj.SalesReturnFromOffline) {
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
                if (mUseOfflineData) {
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
                                                                    }), 2000);

                                                                }
                                                            });

                                                        }
                                                    });
                                                }
                                            }
                                        });

                                    } else {
                                        MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                               "Receipt Settings",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
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
                                    var pMessage = 'SRV#: ' + vDataSRV.SalesPaymentData.Rows[0].SRVNumber + ';<br />Pin Code: ' + vDataSRV.SalesPaymentData.Rows[0].PinCode + ';<br />';
                                    pMessage += "No available printer for this branch. Please contact your system administrator.";
                                    NewMessageBox(pMessage,
                                           "Printer",
                                            [
                                                { Text: 'Ok'
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

                                                    setTimeout(VerifyPrintReceipt(pMemoID, false), 2000)

                                                }
                                            }
                                        });

                                    } else {
                                        MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                               "Receipt Settings",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
                                    }

                                }
                            });

                        }
                        else {
                            MessageBox("No available printer for this branch. Please contact your system administrator.",
                                   "Printer",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
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

                AjaxSend({
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
                            AjaxSend({
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
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentSRVRefundHeaderInfo",
                                            data: pData,
                                            success: function (vDataSRV) {

                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

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
                                                    
                                                    setTimeout(VerifyPrintReceipt(pMemoID, false), 2000)

                                                }
                                            }
                                        });

                                    } else {
                                        MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                               "Receipt Settings",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
                                    }

                                }
                            });

                        }
                        else {
                            MessageBox("No available printer for this branch. Please contact your system administrator.",
                                   "Printer",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
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
                            MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                   "Receipt Settings",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );

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
                            MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                    "Receipt Settings",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }

                    }
                });


            };

            function VerifyPrintReceipt(pMemoID, IsSRV, vData) {


                window.pageSalesReturn = $.mobile.activePage;

                var pMessage = '';

                if (vData != null)
                    pMessage = 'SRV#: ' + vData.SRVNumber + ';<br />Pin Code: ' + vData.PinCode + ';<br />';
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

                                                    $.mobile.changePage(window.pageSalesReturn);

                                                    if (InAndroid || InWinRt) {
                                                        PrintPOSSRVReceipt(pMemoID)
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

                                                    $.mobile.changePage(window.pageSalesReturn);

                                                    if (InAndroid || InWinRt) {
                                                        PrintPOSsalesRefundReceipt(pMemoID)
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
                            ], null, true
                );


            }

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

                if (proj.SalesReturnFromOffline) {
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

                    var LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                    if (LastTotalData != undefined) {
                        var LastTotal = LastTotalData.split('|');

                        dblTotalSales += CDec2(LastTotal[0]);
                        dblTotalSalesDiscount += CDec2(LastTotal[1]);

                        dblCash = CDec2(LastTotal[4]) - dblAdjRefund;
                        dblCheck += CDec2(LastTotal[5]);
                        dblDebitCard += CDec2(LastTotal[6]);
                        dblCreditCard += CDec2(LastTotal[7]);
                        dblSRV += CDec2(LastTotal[8]);

                        dblNonVAT += CDec2(LastTotal[11]);
                        dblVATExempt += CDec2(LastTotal[12]);
                        dblZeroRatedVAT += CDec2(LastTotal[13]);

                        dblTotalVATSales += CDec2(LastTotal[14]);
                        dblTotalVATAmount += CDec2(LastTotal[15]);

                    }

                    dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                    dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                    Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                        dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                            dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                            dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);

                    //MCR
                    
                    LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                    if (LastTotalData != undefined) {
                        var LastTotal = LastTotalData.split('|');

                        dblTotalSales += CDec2(LastTotal[0]);
                        dblTotalSalesDiscount += CDec2(LastTotal[1]);

                        dblCash = CDec2(LastTotal[4]) - dblAdjRefund;
                        dblCheck += CDec2(LastTotal[5]);
                        dblDebitCard += CDec2(LastTotal[6]);
                        dblCreditCard += CDec2(LastTotal[7]);
                        dblSRV += CDec2(LastTotal[8]);

                        dblNonVAT += CDec2(LastTotal[11]);
                        dblVATExempt += CDec2(LastTotal[12]);
                        dblZeroRatedVAT += CDec2(LastTotal[13]);

                        dblTotalVATSales += CDec2(LastTotal[14]);
                        dblTotalVATAmount += CDec2(LastTotal[15]);

                    }

                    dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                    dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                    Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                        dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                            dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                            dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                
                } else {
                    AjaxSend({
                        url: "ReportsPOS/getCashierSalesTransactionSummary",
                        data: pData,
                        success: function (vDataInfo) {

                            vDataInfo = vDataInfo.ReportPOS;


                            for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                dblTotalVATSalesSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                dblTotalVATAmountSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);

                                dblAdjRefund += CDec2(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                dblAdjSRV += CDec2(vDataInfo.Rows[p].CurSalesReturnExchange || 0);
                            };

                            var LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                            var LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                            if (LastTotalData != undefined) {
                                var LastTotal = LastTotalData.split('|');

                                dblTotalSales += CDec2(LastTotal[0]);
                                dblTotalSalesDiscount += CDec2(LastTotal[1]);
                                //dblAdjRefund = CDec2(LastTotal[2]); //value of this variable is being set above
                                //dblAdjSRV = CDec2(LastTotal[3]); //value of this variable is being set above

                                dblCash = CDec2(LastTotal[4]) - dblAdjRefund;
                                dblCheck += CDec2(LastTotal[5]);
                                dblDebitCard += CDec2(LastTotal[6]);
                                dblCreditCard += CDec2(LastTotal[7]);
                                dblSRV += CDec2(LastTotal[8]);

                                //dblVAT -= CDec2(LastTotal[9]);
                                //dblVATAmount -= CDec2(LastTotal[10]);
                                dblNonVAT += CDec2(LastTotal[11]);
                                dblVATExempt += CDec2(LastTotal[12]);
                                dblZeroRatedVAT += CDec2(LastTotal[13]);

                                dblTotalVATSales += CDec2(LastTotal[14]);
                                dblTotalVATAmount += CDec2(LastTotal[15]);

                                //dblTotalVATSalesSalesReturn //value of this variable is being set above
                                //dblTotalVATAmountSalesReturn //value of this variable is being set above
                            }

                            dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                            dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                            Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                                dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                                    dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                                    dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                        }
                    });

                    AjaxSend({
                        url: "ReportsPOS/getManagerSalesTransactionSummary",
                        data: pData,
                        success: function (vDataInfo) {

                            vDataInfo = vDataInfo.ReportPOS;

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

                            for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                dblTotalVATSalesSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                dblTotalVATAmountSalesReturn += CDec2(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);

                                dblAdjRefund += CDec2(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                dblAdjSRV += CDec2(vDataInfo.Rows[p].CurSalesReturnExchange || 0);
                            };

                            var LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');

                            if (LastTotalData != undefined) {
                                var LastTotal = LastTotalData.split('|');

                                dblTotalSales += CDec2(LastTotal[0]);
                                dblTotalSalesDiscount += CDec2(LastTotal[1]);
                                //dblAdjRefund = CDec2(LastTotal[2]); //value of this variable is being set above
                                //dblAdjSRV = CDec2(LastTotal[3]); //value of this variable is being set above

                                dblCash = CDec2(LastTotal[4]) - dblAdjRefund;
                                dblCheck += CDec2(LastTotal[5]);
                                dblDebitCard += CDec2(LastTotal[6]);
                                dblCreditCard += CDec2(LastTotal[7]);
                                dblSRV += CDec2(LastTotal[8]);

                                //dblVAT -= CDec2(LastTotal[9]);
                                //dblVATAmount -= CDec2(LastTotal[10]);
                                dblNonVAT += CDec2(LastTotal[11]);
                                dblVATExempt += CDec2(LastTotal[12]);
                                dblZeroRatedVAT += CDec2(LastTotal[13]);

                                dblTotalVATSales += CDec2(LastTotal[14]);
                                dblTotalVATAmount += CDec2(LastTotal[15]);

                                //dblTotalVATSalesSalesReturn //value of this variable is being set above
                                //dblTotalVATAmountSalesReturn //value of this variable is being set above
                            }

                            dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
                            dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

                            Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                                dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                                    dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                                    dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn);
                        }
                    });
                }
            }

            function UpdateZReadCache() {
                var pData = {
                    RangeType: 'Today',
                    ZDate: new Date(),
                    DeviceID: Web.DeviceID()
                }

                AjaxSend({
                    url: "ReportsPOS/getSalesZReportSummary",
                    data: pData,
                    success: function (vDataInfo) {

                        vDataInfo = vDataInfo.ReportPOS;

                        var pIU = new InsertUpdate('TransZReadCache');
                        var pSql;

                        pSql = "Delete from TransZReadCache where SalesDate = " + ReadyString($.format.date(new Date(), 'yyyy-MM-dd'));
                        Sql.Execute(pSql);

                        pIU.Add('ID', ReadyString(vDataInfo.Rows[0].ID))
                        pIU.Add('PrevGrossSales', CDec2(vDataInfo.Rows[0].PrevGrossSales))
                        pIU.Add('PrevTotalDiscount', CDec2(vDataInfo.Rows[0].PrevTotalDiscount))
                        pIU.Add('PrevSalesReturnRefund', CDec2(vDataInfo.Rows[0].PrevSalesReturnRefund))
                        pIU.Add('PrevSalesReturnExchange', CDec2(vDataInfo.Rows[0].PrevSalesReturnExchange))
                        pIU.Add('PrevTotalVAT', CDec2(vDataInfo.Rows[0].PrevTotalVAT))
                        pIU.Add('PrevServiceCharge', CDec2(vDataInfo.Rows[0].PrevServiceCharge))
                        pIU.Add('PrevChargeAmount', CDec2(vDataInfo.Rows[0].PrevChargeAmount))
                        pIU.Add('SalesDate', ReadyString($.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'yyyy-MM-dd')));
                        pIU.Add('PresentationCurrGrossSales', CDec2(vDataInfo.Rows[0].CurGrossSales));
                        pIU.Add('CurGrossSales', CDec2(vDataInfo.Rows[0].CurGrossSales));
                        pIU.Add('CurTotalDiscount', CDec2(vDataInfo.Rows[0].CurTotalDiscount));
                        pIU.Add('CurTotalVATSales', CDec2(vDataInfo.Rows[0].CurTotalVATSales));
                        pIU.Add('CurTotalVAT', CDec2(vDataInfo.Rows[0].CurTotalVAT));
                        pIU.Add('CurLocalTax', CDec2(vDataInfo.Rows[0].CurLocalTax));
                        pIU.Add('CurServiceCharge', CDec2(vDataInfo.Rows[0].CurServiceCharge));
                        pIU.Add('CurNoTAXSales', CDec2(vDataInfo.Rows[0].CurNoTAXSales));
                        pIU.Add('CurVATExemptSales', CDec2(vDataInfo.Rows[0].CurVATExemptSales));
                        pIU.Add('CurZeroRatedVATSales', CDec2(vDataInfo.Rows[0].CurZeroRatedVATSales));
                        pIU.Add('CurOthers', CDec2(vDataInfo.Rows[0].CurOthers));
                        pIU.Add('CashAmount', CDec2(vDataInfo.Rows[0].CashAmount));
                        pIU.Add('DebitCardAmount', CDec2(vDataInfo.Rows[0].DebitCardAmount));
                        pIU.Add('CreditCardAmount', CDec2(vDataInfo.Rows[0].CreditCardAmount));
                        pIU.Add('SMACAmount', CDec2(vDataInfo.Rows[0].SMACAmount));
                        pIU.Add('CheckPaymentAmount', CDec2(vDataInfo.Rows[0].CheckPaymentAmount));
                        pIU.Add('OtherTypeAmount', CDec2(vDataInfo.Rows[0].OtherTypeAmount));
                        pIU.Add('EPlusAmount', CDec2(vDataInfo.Rows[0].EPlusAmount));
                        pIU.Add('VoucherAmount', CDec2(vDataInfo.Rows[0].VoucherAmount));
                        pIU.Add('CurBegInvoiceNo', vDataInfo.Rows[0].CurBegInvoiceNo);
                        pIU.Add('CurEndInvoiceNo', vDataInfo.Rows[0].CurEndInvoiceNo);
                        pIU.Add('CurTransCountInv', CDec2(vDataInfo.Rows[0].CurTransCountInv));
                        pIU.Add('CurTransInvItemCount', CDec2(vDataInfo.Rows[0].CurTransInvItemCount));
                        pIU.Add('CurTransCountMemoReturnExchange', CDec2(vDataInfo.Rows[0].CurTransCountMemoReturnExchange));
                        pIU.Add('CurSalesReturnExchangeItemCount', CDec2(vDataInfo.Rows[0].CurSalesReturnExchangeItemCount));
                        pIU.Add('CurTransCountMemoRefund', CDec2(vDataInfo.Rows[0].CurTransCountMemoRefund));
                        pIU.Add('CurSalesRefundItemCount', CDec2(vDataInfo.Rows[0].CurSalesRefundItemCount));
                        pIU.Add('CurTermNum', CDec2(vDataInfo.Rows[0].CurTermNum));
                        pIU.Add('FowstNo', CDec2(vDataInfo.Rows[0].FowstNo));
                        pIU.Add('DeviceID', ReadyString(vDataInfo.Rows[0].DeviceID));
                        pIU.Add('FOWSTID', ReadyString(vDataInfo.Rows[0].FOWSTID));

                        pSql = pIU.GetInsert();
                        Sql.Execute(pSql);
                    }
                });
            }
        }); // $(function() {

    });
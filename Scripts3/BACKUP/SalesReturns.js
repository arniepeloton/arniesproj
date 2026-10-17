require(['common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc='),
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
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
		
                $('#lnkPosBranch').click(function (e) {
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
                            BrowseAddresses();
                        }
                    }
                }); // END AjaxSend

            }; // END InitFowst

            function InitUser() {
                GetUser();

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
                Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                Browser.LibraryFowst.InputData.FilterFowst = 1;
                Browser.LibraryFowst.Click = function (e) {
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
                };
                Browser.LibraryFowst.Show();

            };

            $('#txtSearch').keypress(function(e){
                if(e.which == 13){
                    Browser.TransInvoicesReturnables.Click = function (e) {
                        LoadStocks(e.ID);
                    };
                    Browser.TransInvoicesReturnables.InputData.DateFrom = FormatDate($('#btnDateFrom').text()); //FormatDate($('#txtDateFrom').DateCombo('val'));
                    Browser.TransInvoicesReturnables.InputData.DateTo = FormatDate($('#btnDateTo').text()); //FormatDate($('#txtDateTo').DateCombo('val'));
                    Browser.TransInvoicesReturnables.Show($('#txtSearch').val());
                    return false;
                }
            })

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
                $('#txtSearch').val(vData.Units.Rows[0].InvoiceNum);
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
                            ChangePage('/SalesReturns/Stock');
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
                                pTR.find('[name=tdLineTotal]').text(AccountingFormat(pDrUnit.SubTotal));
                                pTR.find('[name=tdQuantityReturned]').text($.format.number(CDec2(pDrUnit.QuantityReturned), proj.QuantityFormat));
                                pTR.click(function (e) {
                                    var pDR = $(this).data('data');

                                    proj.ProductID = pDR.ProductID;
                                    proj.SalesReturnInvoiceID = pDR.InvoiceID;
                                    ChangePage('SalesReturns/Stock');
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
                            pInvoiceNums += pDRs[p].InvoiceNum;
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
                                    ChangePage('/SalesReturns/Stock');
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
                CheckOut({ TotalAmount: mCheckOutVerify.TotalAmount,
                    SkuCount: mCheckOutVerify.SkuCount,
                    Refund: $(this).attr('id') == 'butCheckOutRefund' ? 1 : 0
                });

                function CheckOut(vData) {

                    mIsRefund = vData.Refund;
                    vData.MemoID = proj.SalesReturnMemoID;
                    AjaxSend({
                        url: 'SalesReturns/CheckOut',
                        data: vData,
                        success: function (vData) {
                            if ((vData) && (vData.Result == 'ok')) {
                                try {
                                    if (IsMobile) {
                                        UpdateSalesTextFile();
                                    }

                                    if (CBool(vData.SalesReturnRequiresApproval)) {
                                        $('#spanMemoNum').text(vData.Num);
                                        $.mobile.changePage('#divSalesReturnRequiresApproval');
                                        setTimeout(GetApproval, 10000);
                                        return;
                                    } else {
                                        SaveToInventoryLedger();
                                    }
                                    
                                    Print();
                                } catch (e) {
                                    alert('An error occured while printing SRV: ' + e);
                                };

                            } else
                                MessageBox(vData.Error);
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
                            }
                        }
                    }
                });
            }

            function Print() {
                if (mIsRefund == 1) {                                        
                    if (InAndroid || InWinRt) {
                        PrintPOSsalesRefundReceipt(proj.SalesReturnMemoID);                                            
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
                        PrintPOSSRVReceipt(proj.SalesReturnMemoID);
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

                AjaxSend({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;

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
                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
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
                                                    AjaxSend({
                                                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices",
                                                        data: pData,
                                                        success: function (vDataSRVInv) {

                                                            //Sales Return Voucher Settings 
                                                            AjaxSend({
                                                                url: 'SalesReturns/GetSettings',
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

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Do not lose or crumple this receipt.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

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

                                                                    setTimeout(VerifyPrintReceipt(pMemoID, true), 2000)

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

                AjaxSend({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    success: function (vDataPrint) {
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        var intDesktopPrintLenght = 45;

                        var IsPrintReceipt = false;

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;

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
                                                    strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

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

            function PrintPOSDesktopSRVReceipt(pMemoID) {
                var strTemp = '';
                var strPrint = '';
                var pData = {};
                var japplet = document.jzebra;

                pData.MemoID = pMemoID;

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
                                                        setTimeout(VerifyPrintReceipt(pMemoID, true), 2000)
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

            function VerifyPrintReceipt(pMemoID, IsSRV) {


                window.pageSalesReturn = $.mobile.activePage;

                MessageBox("Your sales return cart was successfully checked out. Were you able to successfully print out a " + (IsSRV ? " SRV Voucher" : " Refund Voucher") + "?",
                            "Print Confirmation",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        Clear();
                                        window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage.OrgID, "_self");
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

                var mDate = new Date();

                pData.FowstID = CStr2(sessionStorage.FowstID);
                pData.RangeType = $.trim('Day');
                pData.RangeFrom = mDate;
                pData.RangeTo = mDate;
                pData.RangeYear = mDate.getFullYear;

                AjaxSend({
                    url: "ReportsPOS/getCashierSalesTransactionSummary",
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

            }

        }); // $(function() {

    });
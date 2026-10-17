require(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    $(function () {

        var mTestPos = {
            // settable variables
            MaxSalesRepresentatives: 0,
            MaxSkuCount: 3,
            OrgNames: [
                'Davao Unified Corporation'
                //, 'ABC Company Inc.'
            ],
            ProductKeywords: [
                '"this key word will yield no result"',
                'book paper',
                'pencil',
                'glue',
                'ballpen',
                'candy',
                'egg',
                'asado',
                'rice',
                'pan'
            ],

            // used internally by this test
            SearchIndex: 0,
            SkuCount: 0,

            Start: function () {
                setTimeout(this.Login, 1000);
            },

            Login: function () {
                MasterFrame.Contents.find('#txtUsername').val('dd');
                MasterFrame.Contents.find('#txtPassword').val('dd');
                setTimeout(function () {
                    // MasterFrame.Contents.find('#txtUsername').closest('form').submit(); // not working
                    MasterFrame.Contents.find('#btnSignIn').click();
                    MasterFrame.OnLoad = function () {
                        setTimeout(mTestPos.ClickPos, 2000);
                    };
                }, 1000);
                // MasterFrame.Contents.find('#txtUsername').closest('form').submit();
            },

            ClickPos: function () {
                var pOrgName;
                var pElement;
                var pFound = false;

                for (var pIndex in mTestPos.OrgNames) {
                    pOrgName = mTestPos.OrgNames[pIndex];
                    pElement = MasterFrame.Child$('[data-role="collapsible"]').children('h2:contains("' + pOrgName + '"):first');
                    if (pElement.length > 0) {
                        if (!pElement.closest('[data-role="collapsible"]').hasClass('ui-collapsible-collapsed')) {
                            pFound = true;
                            break;
                        }
                    }
                }
                if (!pFound) {
                    MessageBox('Org name, ' + pOrgName + ', not found. Test incomplete js2J47BE7NUH!.');
                    return;
                }

                MasterFrame.Child$('html, body').animate({
                    scrollTop: pElement.offset().top + 'px'
                }, 'slow').promise().done(ScrollComplete);

                function ScrollComplete() {

                    pElement = pElement.closest('[data-role=collapsible]').find('a:contains("Point of Sales (POS)")')[0];

                    if ((pElement == null)
                        || (pElement.length == 0)) {

                        MessageBox('POS not found. Test incomplete js5U845H2KJY.');
                        return;
                    }

                    setTimeout(function () {
                        MasterFrame.OnLoad = function () {
                            setTimeout(mTestPos.LoadPos, 2000);
                        };

                        UiTest.ClickLink(pElement);
                        
                        //pElement.click();
                    }, 1000);

                } // END ScrollComplete

            },

            LoadPos: function () {

                var pDataUrl = MasterFrame.Child$.mobile.activePage.attr('data-url');
                var pFowst = 'The Paper Tree - Bolton';
                var pElement;

                if (pDataUrl == 'LibraryFowst') {
                    pElement = MasterFrame.Child$.mobile.activePage.find('[name=divBrowserResults]').find('a:contains("' + pFowst + '")');
                    if (pElement.length == 0) {
                        MessageBox('Fowst, ' + pFowst + ', not found. Test incomplete js5WWXVV6BHA.');
                        return;
                    }
                    // setTimeout(pElement.click, 1000); // does not work
                    setTimeout(function () {
                        UiTest.ClickLink(pElement);
                        setTimeout(mTestPos.SearchProduct, 1000);
                    }, 1000);
                } else {
                    setTimeout(mTestPos.SearchProduct, 1000);
                }

            },

            SearchProduct: function () {

                var pTxtSearch = MasterFrame.Child$('#txtSearch');

                if (pTxtSearch.length == 0) {
                    console.warn('#txtSearch not found. Test incomplete jsL7588A7898!.');
                    setTimeout(mTestPos.SearchProduct, 1000);
                    return;
                };

                // todo: verify cart here
                if (mTestPos.SkuCount >= mTestPos.MaxSkuCount) {
                    if (mTestPos.MaxSalesRepresentatives == 0)
                        mTestPos.CheckOut();
                    else
                        mTestPos.AssignSalesRepresentativeAll();
                    return;
                } // END if (mTestPos.SkuCount <= 7) {

                pTxtSearch.val(mTestPos.ProductKeywords[mTestPos.SearchIndex]);
                if (mTestPos.ProductKeywords.length - 1 == mTestPos.SearchIndex)
                    mTestPos.SearchIndex = 0;
                else
                    mTestPos.SearchIndex++;

                setTimeout(function () {
                    pTxtSearch.closest('form').submit();
                    setTimeout(function() {
                        UiTest.DisplaySearch({
                            Done: mTestPos.StockPage,
                            OnNoResult: function() {
                                history.back();
                                setTimeout(mTestPos.SearchProduct, 1000);
                            }
                        });
                    }, 2000);
                }, 1000);

            }, // END SearchProduct

            StockPage: function () {

                UiTest.DelayedLoop(function() {
                    var pDataUrl = MasterFrame.Child$.mobile.activePage.attr('data-url');

                    if (pDataUrl != 'Stock') {
                        console.trace('data-url of active page must be LibraryFowst. retrying jsK7PQ59FW4V!');
                        return;
                    }

                    return {};
                            
                }, function() {
                    var pTxtQtys = MasterFrame.Child$.mobile.activePage.find('[name=txtQty]');
                    if (pTxtQtys.length == 0) {
                        console.trace('qty txtboxes not found. retrying. jsQRT7ZRGHTJ.');
                        setTimeout(mTestPos.StockPage, 1200);
                        return;
                    }
                    var pIndex = 0;

                    function SetQty() {

                        var pQty;

                        if (pIndex <= pTxtQtys.length - 1) {
                            // todo: find a better random qty algo
                            pQty = RandomInteger(1, 54);
                            pQty = Math.log(pQty);
                            pQty = parseInt(pQty);
                            pQty = RandomInteger(Math.pow(10, pQty), Math.pow(10, pQty + 1));

                            $(pTxtQtys[pIndex])
                                .val(pQty)
                                .trigger('keyup.textchange');
                            setTimeout(SetQty, 1000);
                        } else {
                            var pButtons = MasterFrame.Child$.find('[name=butUnitUpdateCart]');

                            if (pButtons.length == 0) {
                                MessageBox('butUnitUpdateCart buttons not found. jsWRN776EE2M.');
                                return;
                            }
                            UiTest.ClickLink(pButtons[0]);
                            setTimeout(function () {
                                mTestPos.SkuCount++;
                                UiTest.TestPos.SearchProduct();
                            }, 2000);
                        }
                        pIndex++;

                    } // END SetQty
                    setTimeout(SetQty, 1000);
                }, 0, 1000);

            }, // END StockPage

            AssignSalesRepresentativeAll: function() {

                var butSalesRepresentatives;
                var popupSalesRepresentatives;
                var txtSearch;
                var pSalesRepresentativeKeywords = ['a', 'b', 'c', 'd', 'e'];
                var pSalesRepresentativeCount = 0;

                var btnMoreOptions = MasterFrame.Child$('#btnMoreOptions');
                if (btnMoreOptions.length == 0) {
                    MessageBox('#btnMoreOptions not found. js3R8HW5T9U5!');
                    return;
                }
                UiTest.ClickLink(btnMoreOptions[0]);

                var popupMoreOptions = MasterFrame.Child$.find('#popupMoreOptions');
                if (popupMoreOptions.length == 0) {
                    MessageBox('#popupMoreOptions not found. jsY8BELXJWVE!');
                    return;
                }
                popupMoreOptions = MasterFrame.Child$(popupMoreOptions);
                if (popupMoreOptions.closest(".ui-popup-container").hasClass('ui-popup-active') == false) {
                    MessageBox('#popupMoreOptions is not active. jsD7G33N8NZZ!');
                    return;
                }

                setTimeout(popupMoreOptionsShown, 1000);

                function popupMoreOptionsShown() {
                
                    butSalesRepresentatives = popupMoreOptions.find('#butSalesRepresentatives');
                    if (butSalesRepresentatives.length == 0) {
                        MessageBox('#butSalesRepresentatives not found. jsXZ27B6HTCV!');
                        return;
                    }

                    MasterFrame.Child$('html, body').animate({
                        scrollTop: butSalesRepresentatives.offset().top + 'px'
                    }, 'slow').delay(1000).promise().done(ClickButSalesRepresentatives);    

                } // END popupMoreOptionsShown

                function ClickButSalesRepresentatives() {
                    UiTest.ClickLink(butSalesRepresentatives[0]);
                
                    popupSalesRepresentatives = MasterFrame.Child$.find('#popupSalesRepresentatives');
                    if (popupSalesRepresentatives.length == 0) {
                        MessageBox('#popupSalesRepresentatives not found. jsNRY7ATLPCG!');
                        return;
                    }
                    popupSalesRepresentatives = MasterFrame.Child$(popupSalesRepresentatives);
                    if (popupSalesRepresentatives.closest(".ui-popup-container").hasClass('ui-popup-active') == false) {
                        MessageBox('#popupSalesRepresentatives is not active. jsUWXDAGWZ9M!');
                        return;
                    }    

                    txtSearch = popupSalesRepresentatives.find('[name=txtSearch]');
                    if (txtSearch.length == 0) {
                        MessageBox('#txtSearch not found. jsTGBJAZP8WF!');
                        return;
                    }
                    txtSearch = MasterFrame.Child$(txtSearch);
                    setTimeout(StartSearch, 1000);
                } // END ClickButSalesRepresentatives

                function StartSearch() {
                    var pIndex = RandomInteger(0, pSalesRepresentativeKeywords.length - 1);
                    txtSearch.val(pSalesRepresentativeKeywords[pIndex]);
                    setTimeout(PressSearch, 1000);
                } // END StartSearch

                function PressSearch() {
                    txtSearch.closest('form').submit();
                    setTimeout(PickSalesRepresentative, 1000);
                } // END PressSearch

                function PickSalesRepresentative() {
                    UiTest.DisplaySearch({
                        Done: function() {
                            pSalesRepresentativeCount++;
                            if (pSalesRepresentativeCount < mTestPos.MaxSalesRepresentatives)
                                StartSearch();
                            else
                                ClickApply();
                        }
                    });
                } // END PickSalesRepresentative

                function ClickApply() {
                    var butApplyAll = popupSalesRepresentatives.find('button[name=butApplyAll]');
                    if (butApplyAll.length == 0) {
                        MessageBox('#butApplyAll not found. jsLACCNX4M89!');
                        return;
                    }
                    UiTest.ClickLink(butApplyAll[0]);

                    setTimeout(function() {
                        mTestPos.CheckOut();
                    }, 1000);
                } // END ClickApply


            }, // END AssignSalesRepresentativeAll

            CheckOut: function() {
                var pButPay = MasterFrame.Child$('#lnkCheckout');

                if (pButPay.length == 0) {
                    MessageBox('#lnkCheckout not found. js8X9HTZEYS9');
                    return;
                }
                MasterFrame.OnLoad = function () {
                    setTimeout(mTestPos.Payment, 2000);
                };
                UiTest.ClickLink(pButPay[0]);
            }, // END CheckOut

            Payment: function () {

                var pAmountDue;
                var lblSalesPayAmountDue;

                if (frames[0].location.pathname.toLowerCase().indexOf('salespayments') == -1) {
                    MessageBox('Unexpected URL. must be salespayments. jsT6W4UY6ZLS');
                    return;
                }

                UiTest.DelayedLoop(function() {
                    lblSalesPayAmountDue = MasterFrame.Child$('#lblSalesPayAmountDue');
                    if (lblSalesPayAmountDue.length == 0) {
                        MessageBox('#lblSalesPayAmountDue not found. js5LG58WTKZM.');
                        return;
                    }
                    pAmountDue = lblSalesPayAmountDue.text();
                    if (!IsNumeric(pAmountDue)) {
                        MessageBox('#lblSalesPayAmountDue must be numeric. jsJXGNQA8LV7.');
                        return;
                    }
                    pAmountDue = CDec2(pAmountDue);
                    if (pAmountDue > 0) 
                        return {};
                    else
                        return; // optional else. just here for breakpointing
                }, function(vResult) {
                    MasterFrame.Contents.find('html, body').animate({
                        scrollTop: lblSalesPayAmountDue.offset().top + 'px'
                    }, 'slow').promise().done(ScrollComplete);
                }, 0, 1000);

                function ScrollComplete() {
                    UiTest.DelayedLoop(function() {
                        var txtSalesPaymentCash = MasterFrame.Child$('#txtSalesPaymentCash');

                        if (txtSalesPaymentCash.length == 0) {
                            MessageBox('#txtSalesPaymentCash not found. js4E92G4PQ9V.');
                            return;
                        }

                        pAmountDue = pAmountDue + pAmountDue * RandomInteger(0, 10) / 100;
                        txtSalesPaymentCash.val(pAmountDue);
                        txtSalesPaymentCash.trigger({
                            type: 'keyup'
                        });

                        if (CDec2(txtSalesPaymentCash.val()) > 0)
                            return {};
                        else
                            return; // optional else. just here for breakpointing
                    }, function(vResult) {
                        var butSaveReceipt = MasterFrame.Child$('#butSaveReceipt');
                        if (butSaveReceipt.length == 0) {
                            MessageBox('#butSaveReceipt not found. jsQLHSPAL3UU.');
                            return;
                        }
                        UiTest.ClickLink(butSaveReceipt[0]);

                        UiTest.DelayedLoop(function() {
                            var pDialog = UiTest.CurrentDialog();
                            if (pDialog == null) {
                                console.trace('dialog message box is expected here. jsN5WQKMRHM2!');
                                return;
                            }
                            if (pDialog.Text != 'Your transaction has been saved successfully.') {
                                console.trace('a different dialog message is expected here. js7QSZ8PK2Q3!');
                                return;
                            }

                            return pDialog;
                            
                        }, function(vDialog) {
                            MasterFrame.OnLoad = mTestPos.LoadPos;
                            mTestPos.SkuCount = 0;
                            setTimeout(vDialog.OkButtonClick, 2000);
                        }, 1000, 1000);
                    }, 0, 1000);

                } // END ScrollComplete

            } // END Payment

        }; // END mTestPost

        window.UiTest = {
            TestPos: mTestPos,
            CurrentDialog: function () {
                var pPage = MasterFrame.Child$.mobile.activePage;
                var pRole = pPage.attr('data-role');
                if (pRole != 'dialog')
                    return null;

                return function (vPage) {
                    var pOut = {
                        Text: vPage.find('[data-role=content] div:first').text(),
                        OkButton: vPage.find('button:contains("Ok")'),
                        OkButtonClick: function () {
                            UiTest.ClickLink(pOut.OkButton);
                        }
                    };
                    return pOut;
                } (pPage);
            }, // END CurrentDialog
            ClickLink: function (vElement) {
                // http://stackoverflow.com/a/14669335/334045
                var pEvent = document.createEvent('HTMLEvents');
                pEvent.initEvent('click', true, true);
                if ($.isFunction(vElement.dispatchEvent))
                    vElement.dispatchEvent(pEvent);
                else
                    vElement[0].dispatchEvent(pEvent);
            }, // END ClickLink
            DisplaySearch: function(vParams) {
                // PARAMS:
                //  Done: function
                //  OnNoResult: (optional) function

                UiTest.DelayedLoop(function() {
                    if (MasterFrame.Child$.mobile == undefined)
                        return;
                    var pBrowser = MasterFrame.Child$.mobile.activePage.find('[name=divBrowserResults]');

                    if (pBrowser.length == 0) {
                        console.trace('Browser window not found. retrying. jsXD9ANREEUN!.');
                        return;
                    }

                    return pBrowser;
                            
                }, function(vBrowser) {
                    var pList = vBrowser.find('[name=ulBrowser]');

                    if (pList.length == 0) {
                        MessageBox('Results list not found. Test incomplete jsXBHKX4YA8L.');
                        return;
                    }

                    var pChildren = pList.children('li');

                    if (pChildren.length == 0) {
                        if ($.isFunction(vParams.OnNoResult))
                            vParams.OnNoResult();
                        else
                            MessageBox('Search result is blank. Recommendation: OnNoResult. Test incomplete jsZKE5UDCKLC.');
                        return;
                    }

                    var pIndex = RandomInteger(0, pChildren.length - 1);

                    var pLink = MasterFrame.Child$(pChildren[pIndex]).find('a');

                    if (pLink.length == 0) {
                        MessageBox('Link of search result item was not found. Test incomplete jsA7PVHZKHYX.');
                        return;
                    }

                    MasterFrame.Child$('html, body').animate({
                        scrollTop: pLink.offset().top + 'px'
                    }, 'slow').delay(1000).promise().done(ScrollComplete);

                    function ScrollComplete() {
                        try {
                            UiTest.ClickLink(pLink[0]);
                        } catch (e) {
                            throw e;
                        }

                        if (MasterFrame.Child$.isFunction(vParams.Done))
                            setTimeout(vParams.Done, 1200);
                    } // END ScrollComplete
                }, 1000, 1000);
                
            }, // END DisplaySearch

            DelayedLoop: function(vFunction, vOnSuccess, vStartDelay, vEndDelay) {
                // PARAMS:
                //  vFunction: must return true else will be called again after vRetryDelay milliseconds

                var pResult;

                if (!$.isFunction(vFunction))
                    throw 'vFunction must be a function';
                    
                if (CDec2(vStartDelay) >= 1) {
                    setTimeout(function() {
                        UiTest.DelayedLoop(vFunction, vOnSuccess, 0, vEndDelay);
                    }, vStartDelay);
                } else {
                    pResult = vFunction();
                    if (pResult == undefined) 
                        setTimeout(function() {
                            UiTest.DelayedLoop(vFunction, vOnSuccess, 0, vEndDelay);
                        }, vEndDelay);
                    else 
                        setTimeout(function() {
                            vOnSuccess(pResult);
                        }, vEndDelay);
                }

            } // END DelayedLoop
        };

        (function LoadPage() {

            AjaxSend({
                ExactUrl: '/AndroidJsTest/TestControl?cache=1',
                dataType: 'text',
                success: function (vData) {
                    var pPage = $(vData);
                    $('#divTestControl')
                        .append(pPage)
                        .trigger('create');
                    InitPage();
                }
            });

        })(); // END LoadPage

        function InitPage() {

            $('#butStartTest').click(function () {

                $('.ui-navbar ul li > a:eq(0)').click();
                UiTest.TestPos.Start();

            }); // END $('#butStartTest').click

        } // END InitPage

    }); // END $(function () {

});                               // END require
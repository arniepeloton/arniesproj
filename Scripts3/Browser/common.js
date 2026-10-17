define(['common'], function () {

    if (window.Browser == undefined) {

        window.Browser = {
            OfflineMode: false,
            Init: function (vKey, vOptions) {

                if (Browser[vKey] != undefined) throw 'Browser key, ' + vKey + ', already exist';
                Browser[vKey] = new BrowserClass(vKey, vOptions);

                function BrowserClass(vKey, vOptions) {

                    var mPage;
                    var mSearchResults;
                    var mParentPage;
                    var mKeywords;  // set on this.Show
                    var mParams;  // set on this.Show
                    // get on mPage.pageShow
                    var mDivResults;
                    var mMyClass = this;
                    var mInitialized = false;
                    var mSelectedData;
                    var butBarcode;

                    // "results" page
                    var mLV;

                    mMyClass.InputData = {};
                    mMyClass.InitInputData = $.extend({}, vOptions.InputData);
                    mMyClass.InternalInputData = {};
                    mMyClass.AutoSearch = vOptions.AutoSearch;      // property: boolean
                    mMyClass.AutoSearchIfBlank = vOptions.AutoSearchIfBlank;      // property: boolean
                    mMyClass.AfterResult = vOptions.AfterResult;    // callback
                    mMyClass.PageShow = vOptions.PageShow;          // callback
                    mMyClass.DontGoBack = vOptions.DontGoBack;

                    this.Show = function(vKeywords, vParams) {
                        // PARAMS:
                        //  vKeywords: if set, this.Show will dislay it the the textbox and call AjaxSend
                        //vParams: {
                        //    description: 'text',
                        //    beforeShow: function(vArgs) {
                        //        vArgs = {
                        //            page: object
                        //        }
                        //    }
                        //}
                        vParams = $.extend({}, vParams);
                        mParams = vParams;
                        mKeywords = vKeywords;
                        mMyClass.InternalInputData = $.extend(mMyClass.InitInputData, mMyClass.InputData);
                        if (mInitialized) {
                            this.HideRightButton();
                            if (vParams.Title != undefined)
                                mMyClass.Title(vParams.Title);
                            else if (vOptions.Title != undefined)
                                mMyClass.Title(vOptions.Title);
                            else if (mMyClass.Title().length == 0)
                                mMyClass.Title('Search');
                            mParentPage = $.mobile.activePage;
                            ClearResults();
                            mPage.find('[name=txtBrowser]').val(CStr2(mKeywords));
                            if ($.isFunction(vParams.beforeShow))
                                vParams.beforeShow({
                                    page: mPage
                                });
                            mPage.find('.divBrowserContentTop').text(vParams.description);
                            ChangePage2(mPage);
                            if (mMyClass.AutoSearch)
                                Search();
                            else if ((mMyClass.AutoSearchIfBlank)
                                && (CStr2(vKeywords).trim().length == 0))
                                Search();
                        } else {
                            Init();
                        };

                    }; // this.Show

                    function Init() {
                        if (Browser.Html == undefined)
                            AjaxSend({
                                ExactUrl: '/Browser?cache=1',
                                //data: { keywords: $('#txtSearch').val() },
                                dataType: 'text',
                                type: 'GET',
                                success: function (vData) {
                                    Browser.Html = vData;
                                    InitPage(Browser.Html);
                                    mInitialized = true;
                                    mMyClass.Show(mKeywords, mParams);
                                }
                            });
                        else {
                            mInitialized = true;
                            InitPage(Browser.Html);
                            mMyClass.Show(mKeywords, mParams);
                        };
                    }; // Init

                    function InitPage(vHtml) {

                        mPage = $(vHtml);
                        
                        try {
                            if (IsMobile) {
                                if (Business.SystemDefaults.HideSearchWithinResults()) {
                                    var pFilterable = mPage.find('[data-filter=true]');
                                    pFilterable.attr('data-filter', 'false');
                                }
                                if (Business.SystemDefaults.HideBarcodeScan())
                                    // mPage.find('[name=butBarcode]').closest('.ui-btn').hide(); // hide a button http://stackoverflow.com/a/7054037
                                    mPage.find('[name=butBarcode]').remove();
                            }
                        } catch (e) {
                            debugger;
                        }

                        mPage.attr('data-url', vKey); // this is required to append hash tag on the address bar
                        mPage.find('[name=formBrowser]').submit(function (e) {
                            e.preventDefault();
                            window.IsBrowserloaded = false;
                            Search();
                            return false;
                        });

                        mPage.bind('pageshow', function () {
                             if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                                if (butBarcode == undefined) {
                                    butBarcode = mPage.find('[name=butBarcode]');
                                    butBarcode.show();
                                    butBarcode.click(function () {
                                        // dont hide button here. button will still show.
                                        // hide button after page show
                                        mobile.Scan({
                                            Success: function (vData) {
                                                mPage.find('[name=txtBrowser]').val(CStr2(vData.Value));
                                                Search();
                                            }
                                        }); // scan
                                    }); // click
                                }; // if (butBarcode == undefined) {
                            }; // if (IsMobile) {
                            if (!ValidPrevPage(arguments)) return;
                            mPage.find('[name=txtBrowser]').val(CStr2(mKeywords)).focus();
                            if (CStr2(mKeywords).length > 0)
                                Search();
                            if ($.isFunction(mMyClass.PageShow))
                                mMyClass.PageShow();

                            function ValidPrevPage(args) {
                                try {
                                    if (args[1].prevPage.attr('data-role') != 'dialog') return true;
                                    if (args[1].prevPage.data('CanRefreshStockPage')) return true;
                                } catch (e) {
                                    return true;
                                };

                            }; // function ValidPrevPage(args) {
                        }); // mPage.bind('pageshow', function () {

                        mPage.bind('pagehide', function () {
                            setTimeout(function() {
                                if (mSelectedData != undefined)
                                    if ($.isFunction(mMyClass.AfterClick)) {
                                        mMyClass.AfterClick(mSelectedData);
                                        mSelectedData = undefined;
                                    };
                            }, 500);
                        }); // mPage.bind('pagehide', function() {

                        $('body').append(mPage);
                        mMyClass.Page = mPage;
                    };

                    function Search() {

                        var pData = mMyClass.InternalInputData || {};
                        var pBrowser;
                        var pStr;

                        mMyClass.InputData = {};
                        pData.keywords = mPage.find('[name=txtBrowser]').val();
                        mMyClass.OutputData = undefined;

                        var m_Keywords = pData.keywords.split('|');
                        if (CBool(mMyClass.InternalInputData.SearchOnline)) {
                            SearchOnline(pData);
                            return;
                        }

                        if (m_Keywords[1] != undefined && m_Keywords[0] == 'so' && vKey == 'TransInvoices') {
                            pData.keywords = m_Keywords[1];
                            AjaxSend({
                                url: 'Browser/' + (vOptions.Url || vKey),
                                data: pData,
                                success: function (vData) {
                                    if (vData.Error)
                                        MessageBox(vData.Error, 'Error');
                                    else {
                                        mSearchResults = vData.Results;
                                        mMyClass.OutputData = vData;
                                        mMyClass.OfflineMode = false;
                                        ShowResults(vData);
                                    };
                                },
                                complete: function () {
                                    mMyClass.InputData = {};
                                }
                            }); // AjaxSend
                        }

                        if (
                            (
                                (mMyClass.OfflineMode == undefined)
                                || (CBool(mMyClass.OfflineMode))
                            )
                            && (window.Browser.OfflineMode)) {

                            try {
                                pBrowser = window.OfflineBrowser[vOptions.Url || vKey];
                                if (pBrowser == undefined) {
                                    pStr = 'OfflineBrowser.' + (vOptions.Url || vKey) + ' was not found';
                                    console.log(pStr);
                                    MessageBox(pStr);
                                    return;
                                };
                                mSearchResults = pBrowser.GetData(pData);

                                if (mSearchResults.Rows.length <= 0 && vKey == 'TransInvoices') {

                                    MessageBox("Unable to find Sales Order " + $.trim(pData.keywords) + " in your device. Do you want to check it online?",
                                            "Sales Order",
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {
                                                        $('.ui-dialog').dialog('close');
                                                        SearchOnline(pData);
                                                        return;
                                                    }
                                                }
                                            ,
                                                { Text: 'No'
                                                }
                                            ]
                                    );
                                    return;
                                } else {
                                    mMyClass.InputData = {};
                                    mMyClass.OfflineMode = true;
                                    ShowResults({});
                                }

                            } catch (e) {
                                MessageBox(e);
                                console.error(e);
                            };
                        } else { // if (OfflineMode) {
                            SearchOnline(pData);
////////////////                            AjaxSend({
////////////////                                url: 'Browser/' + (vOptions.Url || vKey),
////////////////                                data: pData,
////////////////                                success: function (vData) {
////////////////                                    if (vData.Error)
////////////////                                        MessageBox(vData.Error, 'Error');
////////////////                                    else {
////////////////                                        mSearchResults = vData.Results;
////////////////                                        mMyClass.OutputData = vData;
////////////////                                        mMyClass.OfflineMode = false;
////////////////                                        ShowResults(vData);
////////////////                                    };
////////////////                                },
////////////////                                complete: function () {
////////////////                                    mMyClass.InputData = {};
////////////////                                }
////////////////                            }); // AjaxSend
                        }; // if (OfflineMode) {
                        //                        } //window.Browser.OfflineMode && GetUrlParameters().mode == 'so' && vKey == 'TransInvoices'

                    }; // function Search() {

                    function SearchOnline(pData) {
                        AjaxSend({
                            url: 'Browser/' + (vOptions.Url || vKey),
                            data: pData,
                            success: function (vData) {
                                if (vData.Error)
                                    MessageBox(vData.Error, 'Error');
                                else {
                                    mSearchResults = vData.Results;
                                    mMyClass.OutputData = vData;
                                    mMyClass.OfflineMode = false;
                                    ShowResults(vData);
                                };
                            },
                            complete: function () {
                                mMyClass.InputData = {};
                            }
                        }); // AjaxSend
                    }

                    function InitAfterResults() {

                        var pTxtFilter;

                        if (mLV != undefined) return;
                        mLV = mPage.find('[name=ulBrowser]');
                        mDivResults = mPage.find('[name=divBrowserResults]');
                        mDivResults.show();

                        pTxtFilter = mDivResults.find('input[data-type="search"]');
                        pTxtFilter.on('keyup', function () {
                            DisplayCount();
                        });

                    };

                    function ShowResults(vData) {

                        var pItem;
                        var pLink;
                        var pE;

                        InitAfterResults();
                        mLV.html('');
                        mDivResults.show();
                        for (var p = 0; p <= mSearchResults.Rows.length - 1; p++) {
                            pE = {
                                Index: p
                            };
                            pItem = $('<li>');
                            if ($.isFunction(vOptions.OnLink)) {
                                pLink = $('<a>')
                                    .attr('href', '#')
                                    .data('data', mSearchResults.Rows[p])
                                    .click(function () {
                                        mSelectedData = $(this).data('data');
                                        if ($.isFunction(mMyClass.Click)) {
                                            var pOpt = {
                                                DontClose: false
                                            };

                                            mMyClass.Click(mSelectedData, pOpt);
                                            mSelectedData = undefined;
                                            if (!pOpt.DontClose)
                                                history.back();
                                        } else if ($.isFunction(mMyClass.AfterClick)) {
                                            mSelectedData = $(this).data('data');
                                            history.back();
                                        };
                                        return false;
                                    });
                                vOptions.OnLink(mSearchResults.Rows[p], pLink, pE);
                                pItem.append(pLink);
                            } else
                                vOptions.OnItem(mSearchResults.Rows[p], pItem, pE);
                            mLV.append(pItem);
                        };

                        if (mSearchResults.Rows.length == 1 && vKey == "LibraryPIMProduct") {

                            if (window.IsBrowserloaded == undefined) {
                                var pOptTest = {
                                    DontClose: false
                                };

                                mMyClass.Click(mSearchResults.Rows[0], pOptTest);
                                window.IsBrowserloaded = true;
                            } else {
                                if (!window.IsBrowserloaded) {
                                    var pOptTest = {
                                        DontClose: false
                                    };

                                    if ($.isFunction(mMyClass.Click))
                                        mMyClass.Click(mSearchResults.Rows[0], pOptTest);

                                    window.IsBrowserloaded = true;
                                }
                            }
                        }

                        try {
                            mLV.listview('refresh');
                        } catch(e) {
                            console.log ('jsB22A56TBKY! ' + e.message);
                        }
                        //$('#pageResults input[data-type="search"]').focus();
                        DisplayCount(mSearchResults.Rows.length);
                        if ($.isFunction(mMyClass.AfterResult))
                            mMyClass.AfterResult(vData);

                        if (vKey == 'LibraryPIMProduct' && Business.SystemDefaults.ShowCondensedProductInformationInSearchResult()) {
                            AjaxSend({
                                url: 'Organization/GetBulkPIMPictures',
                                data: { data: mSearchResults.Rows, size: 'medium', IsDefault: true },
                                dontHandleSessionExpired: true, // prevent invalid org
                                success: function (e) {
                                    if (e.Error) {
                                        console.warn(e.Error);
                                        return; // silent error lang
                                    }
                                    var vRows = e.data;
                                    var sourceimg = Android.GetServer();

                                    for (var i = 0; i < vRows.length; i++) {
                                        if (vRows[i].Pics != '') {
                                            vRows[i].Ctr = 1;
                                            var pSrc = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Pics[0].FileName;

                                            $('#sr_' + vRows[i].ID).attr('src', pSrc)
                                                .attr('onerror', 'skupicerror(this)')
                                                .attr('isimgerror', 0)
                                                .attr('isimgviewer', 0)
                                                .data('data', vRows[i])
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = $(this).data('data');                                                    

                                                    $(this).parents().closest('li').removeClass('ui-btn-active');

                                                    if (CInt2($(this).attr('isimgerror')) == 1) {
                                                        NewMessageBox('Can\'t load images for SKU: <i>' + data.ProductName + '</i> or the server not responding.');
                                                        return;
                                                    }

                                                    AjaxSend({
                                                        url: 'Organization/GetBulkPIMPictures',
                                                        data: { data: [data], size: 'big' },
                                                        success: function (e) {
                                                            vRows = e.data[0].Pics;

                                                            var div = $('[name="formBrowser"]');

                                                            if ($('#skupics').length == 0){
                                                                div.append($('<div id="skupics" style="display:none;">'));                                                                
                                                            } else {
                                                                $('#skupics').empty();
                                                            }

                                                            div = $('#skupics');

                                                            for (var i = 0; i < vRows.length; i++) {
                                                                vRows[i].Ctr = i + 1;
                                                                vRows[i].ID = data.ID;
                                                                vRows[i].ProductName = data.ProductName;
                                                                var srcBig = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].FileName;

                                                                div.append($('<a data-role="none" data-lightbox="skupictures">')
                                                                            .attr('id', 'pic_' + vRows[i].ID)
                                                                            .data('data', vRows[i])
                                                                            .attr('href', srcBig)
                                                                            .append($('<img>')
                                                                                .attr('isimgerror', 0)
                                                                                .attr('isimgviewer', 1)
                                                                                .attr('onerror', 'skupicerror(this)')
                                                                                .data('data', vRows[i]).attr('src', srcBig)
                                                                            )
                                                                        )
                                                            }

                                                            var firstpic = $('#pic_' + vRows[0].ID);
                                                            firstpic.on('click', function () {
                                                                var dis = $(this);
                                                                setTimeout(function () {
                                                                    if (CInt2(dis.find('> img').attr('isimgerror')) == 1) {
                                                                        NewMessageBox('Can\'t load images for SKU: <i>' + firstpic.data('data').ProductName + '</i> or the server not responding.');
                                                                    }
                                                                }, 100)
                                                            })
                                                            firstpic.trigger('click');
                                                        }
                                                    })
                                                })
                                        } else {
                                            $('#sr_' + vRows[i].ID).attr('src', '../Content/images/box.png')
                                        }
                                    }
                                },
                                error: function (e) {
                                    Android.AddLog('SKU Search: Can\'t connect to the server, SKU image not successfully loaded');
                                }
                            })
                        }
                    };

                    function DisplayCount(vCount) {
                        // PARAMS:
                        //  vCount: [optional]

                        var pCount;

                        if (vCount == undefined)
                            pCount = mLV.children(':visible').not('#no-results').length;
                        else
                            pCount = vCount;
                        if (pCount == 0) {
                            mPage.find('[name=divRecordCount]').text('No records');
                            var pParams = GetUrlParameters();
                            if (pParams.mode == 'wrt' && localStorage.getItem("wrt") == 1) {
                                $('#personFreeType').show();
                                $('#personSearch').val('');
                            }
                        } else {
                            mPage.find('[name=divRecordCount]').text('No. of records: ' + pCount);
                            var pParams = GetUrlParameters();
                            if (pParams.mode == 'wrt' && localStorage.getItem("wrt") == 1) {
                                $('#personFreeType').hide();
                                $('#personName').val('');
                            }
                        };

                    };

                    function ClearResults() {

                        if (mDivResults == undefined) return;
                        mLV.html(''); // clean the results before changing. dont show previous results
                        mDivResults.hide();
                    };

                    this.GetPage = function () {

                        return mPage;

                    };

                    this.Title = function (vTitle) {

                        var pLabel = mPage.find('[data-role=header] h1:eq(0)'); // mPage.find('[data-role=header] h1:nth-child(1)') does not work

                        if (vTitle == null)
                            return pLabel.text();
                        else
                            pLabel.text(vTitle);

                    };

                    this.ShowRightButton = function () {

                        var pButton = mPage
                            .find('[name=butRight]');

                        pButton.closest('div[name=divButRight]')
                            .show();

                        return pButton;

                    }; // this.ShowRightButton = function() {

                    this.HideRightButton = function () {

                        var pButton = mPage
                            .find('[name=butRight]');

                        pButton.closest('div[name=divButRight]')
                            .hide();

                        return pButton;

                    }; // END HideRightButton

                    this.ShowSearchMore = function () {

                        var pButton = mPage.find('[name=butSearchMore]')
                            .show();

                        return pButton;

                    }; // this.ShowSearchButton = function() {

                    this.ShowManualEntry = function () {

                        var pButton = mPage.find('[name=butManualEntry]')
                            .show();

                        return pButton;

                    }; // this.ShowSearchButton = function() {

                    this.doSearch = function () {

                        Search();

                    }; // this.doSearch = function() {

                    return this;

                }; // Browser._class = function(vKey, vOptions) {

            } // Init: function(vKey, vOptions) {

        }; // Browser = {

    }; // if (window.Browser == undefined) {
});
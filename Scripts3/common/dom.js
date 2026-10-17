define(['jqm', 'jquery.format',
    GetPathFormat('/Scripts3/common/common.js?v=' + assetversion)
    ], function () {

    window.MessageBox = function (vText, vTitle, vButtons,
        vParams, vPreservePage) {

        if (typeof Bs != 'undefined') {
            //\Content\bootstrap\js\bootstrap_modal.js
            Bs.MessageBox(vText, vTitle, vButtons, vParams)
            return;
        }

        // vParams: OnClose (function)

        vTitle = vTitle || '';

        var pPage = $('<div>');
        pPage.attr('data-role', 'dialog');
        pPage.attr('id', 'randomID');

        var pHeader = $('<div>');
        pHeader.attr('data-role', 'header')
            .append(
                $('<h1>').text(vTitle)
            );
        pPage.append(pHeader);

        vParams = $.extend({
            EncodeHtml: true
        }, vParams);

        var pContent = $('<div>');
        pContent.attr('data-role', 'content');

        var pDivText = $('<div>');
        pContent.append(pDivText);

        if (vText instanceof jQuery) {
            pDivText.append(vText);
        } else {
            if (vParams.EncodeHtml) 
                pDivText.text(vText);
            else
                pDivText.html(vText);
        }; // if (vText instanceof jQuery) {

        var pButton;
        var pItem;

        if (vButtons == undefined) {
            vButtons = [
                {
                    Text: 'Ok'
                }
            ];
        };

        if ($.isArray(vButtons)) {
            for (var pIndex = 0; pIndex <= vButtons.length - 1; pIndex++) {
                pItem = $.extend({ CloseOnClick: true }, vButtons[pIndex]);
                pButton = $('<button>');
                pButton.text(pItem.Text);
                if (pItem.Click != null) {
                    pButton.click(pItem.Click);
                } else if (pItem.CloseOnClick) {
                    pButton.click(function (e) {
                        history.back();
                    });
                };
                pContent.append(pButton);
            };
        } else if (vButtons != undefined) {
            throw ('if vButton is not null, it must be an array');
        };

        pPage.append(pContent);
        $('body').append(pPage);
        pPage.bind('pagehide', function (e) {
            if(vPreservePage == undefined){
                pPage.remove();
            }else if(vPreservePage == false){
                pPage.remove();
            }

            if ($.isFunction(vParams.OnClose))
                vParams.OnClose();
        });
        $.mobile.changePage(pPage);
        $('html, body').animate({
            scrollTop: pPage.offset().top + 'px'
        }, 'fast');

        pPage.AddText = function (vText, vParams) {

            var pDiv;

            vParams = $.extend({
                EncodeHtml: true,
                RemoveExisting: false
            }, vParams);

            if (vText instanceof jQuery) {
                pDiv = vText;
            } else {
                pDiv = $('<div>');
                if (vParams.EncodeHtml) 
                    pDiv.text(vText);
                else
                    pDiv.html(vText);
            }; // if (vText instanceof jQuery) {

            if (CBool(vParams.RemoveExisting))
                pDivText.html();
            pDivText.append(pDiv);
        }; // END AddText

        pPage.SetText = function (vText, vParams) {

            vParams = $.extend({}, vParams);
            vParams.RemoveExisting = true;

            pPage.AddText(vText, vParams);
            
        }; // END SetText

        pPage.dialog();

        return pPage;
    }; // MessageBox

    //    window.PageChange = function(vFunction) {

    //        // pageinit fail
    //        // pageshow fail
    //        // pagechange success
    //        $(document).bind('pagechange', function() {
    //            if (init.DataUrl == undefined)
    //                init.DataUrl = $.mobile.activePage.attr('data-url');
    //            else 
    //                if (init.DataUrl != $.mobile.activePage.attr('data-url'))
    //                    return;
    //            vFunction();
    //        });    

    //    }; // window.PageChange = function(vFunction) {

    window.ChangePage = function (vUrl) {
        // alternative to $.mobile.changePage

        var pPage = $('[data-role=page][data-url="' + vUrl.replace(/\//g, '^') + '"]');
        var pUrl;

        if (pPage.length == 0) {
            pUrl = MergeUrlParameters({
                IsMobile: IsMobile ? 1 : 0,
                Partial: 1 // dont load the whole page. OfflineMode will be lost. weird.
            }, vUrl);
            AjaxSend({
                ExactUrl: pUrl,
                dataType: 'text',
                type: 'GET',
                success: function (vData) {
                    InitPage(vData);
                },
                error: function (vError) {
                    var pDummy;
                }
            });

            function InitPage(vHtml) {

                var pPages, pPage;

                pPages = $('<div>');
                pPages.html(vHtml);  
                // pPages.get(0).innerHTML = vHtml; // dont use this. error on IE11
                pPages = pPages.find('[data-role=page]'); 
                pPage = $(pPages[0]);
                pPage.attr('data-url', vUrl.replace(/\//g, '^')); // this is required to append hash tag on the address bar
                $('body').append(pPages);
                
                ChangePage2(pPage);

            }; // InitPage
        } else {
            
            $.mobile.changePage(pPage);
        };

    }; // ChangePage

    window.ChangePage2 = function(vPage) {
        
        vPage.append($('#divFooterAjax'));
        $.mobile.changePage(vPage);

    };

    window.CreateListView = function (vOptions) {

        var mLV;
        var mMyClass = this;
        var mDivRecordCount;
        var mData;
        var pDataFilter = true;

        try {
            if (OfflineMode && Business.SystemDefaults.HideSearchWithinResults()) {
                pDataFilter = false;
            }
        } catch (e) {
            debugger;
        }

        vOptions = $.extend({
            // DataFilter: true
            DataFilter: pDataFilter
            }, vOptions);

        (function Init() {

            var pDiv = $('<div>');
            var pDivCountTop = $('<div>')
                    .addClass('ListViewMarginBottom');
            var pDivCountBottom = $('<div>')
                    .addClass('ListViewMarginTop');
            mLV = $('<ul>')
                    .attr('data-role', 'listview')
                    .attr('data-filter', vOptions.DataFilter);

            pDiv.append(pDivCountTop)
                    .append(mLV)
                    .append(pDivCountBottom);
            mDivRecordCount = pDivCountTop.add(pDivCountBottom);
            mMyClass.Element = pDiv;

        })(); // function Init()

        function InitAfterResults() {

            var pTxtFilter;

            pTxtFilter = mLV.parent().find('input[data-type="search"]');
            pTxtFilter.on('keyup', function () {
                DisplayCount();
            });

        }; // function InitAfterResults() {

        this.SetData = function (vRows) {

            var pItem;
            var pLink;
            var pE;
            
            mData = vRows;
            mLV.html('');

            if (vRows != undefined) {
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pE = {
                        Index: p
                    };
                    pItem = $('<li>');
                    mMyClass.OnItem(vRows[p], pItem, pE);
                    mLV.append(pItem);
                }
            }

            try {
                mLV.listview('refresh');
            } catch (e) {
                try {
                    mLV.listview(); // 1st time
                } catch (e) {
                    // just ignore this error
                };
            };
            if (vRows != undefined) 
                DisplayCount(vRows.length);
            else
                DisplayCount(0);
            InitAfterResults();
            mMyClass.Element.show();

        }; // this.SetData = function

        this.GetData = function() {
            
            return mData;

        }; // this.GetData = function() {

        this.DisplayCount = DisplayCount;
        function DisplayCount(vCount) {
            // PARAMS:
            //  vCount: [optional]

            var pCount;

            if (vCount == undefined)
                pCount = mLV.children(':visible').length;
            else
                pCount = vCount;
            if (pCount == 0) {
                mDivRecordCount.text('No records');
            } else {
                mDivRecordCount.text('No. of records: ' + pCount);
            };

        };

        function ClearResults() {

            mLV.html(''); // clean the results before changing. dont show previous results
            DisplayCount(0);

        };

        this.ClearResults = ClearResults;

        return this;

    }; // window.CreateListView = function() {

    window.CreateTable = function (vData, vOptions) { 

        var pTable;
        var pRow, pCell;
        var pBody;
        var pKey;
        var pText;
        var pColumn;
        var pBeforeCellArgs;

        vOptions = $.extend({
            Element: null, // this is a table element. if set, CreateTable will not create a new table
            BeforeCell: function(e) {
                return true;
            },
            BeforeRow: function(e) {
                return true;
            },
            AfterRow: function(e) {
            }
        }, vOptions);

        if (vOptions.Element)
            pTable = $(vOptions.Element);
        else
            pTable = $('<table data-role="table">')
                .addClass('ui-responsive table-stroke');
        if (vOptions.attr)
            pTable.attr(vOptions.attr);

        if (vOptions.Columns == undefined) {
            vOptions.Columns = {};
            for (pKey in vData.Columns) {
                if (!vData.Columns.hasOwnProperty(pKey))
                    continue;
                vOptions.Columns[pKey] = {};
            }
        }

        // create header
        pBody = pTable.children('thead');
        if (pBody.length == 0)
            pBody = $('<thead>');
        if (vOptions.theadAttr)
            pBody.attr(vOptions.theadAttr);
        pRow = pBody.children('tr');
        if (pRow.length == 0)
            pRow = $('<tr>');
        pRow.html('');
        for (pKey in vOptions.Columns) {
            if (!vOptions.Columns.hasOwnProperty(pKey))
                continue;
            pColumn = vOptions.Columns[pKey];
            if (pColumn.Hidden == true) continue;
            pColumn.Text = pColumn.Text || pColumn.Name || pKey; // header text
            pCell = $('<th>').text(pColumn.Text);
            pRow.append(pCell);
        };
        pBody.append(pRow);
        pTable.append(pBody);

        // create body
        pBody = pTable.children('tbody');
        if (pBody.length == 0)
            pBody = $('<tbody>');
        pBody.html('');
        for (var pY = 0; pY <= vData.Rows.length - 1; pY++) {
            pRow = $('<tr>');
            pRow.data('data', vData.Rows[pY]);
            pCell = null;
            for (pKey in vOptions.Columns) {
                if (!vOptions.Columns.hasOwnProperty(pKey))
                    continue;
                pColumn = vOptions.Columns[pKey];
                if (pColumn.Hidden == true) continue;
                if (pCell == null)
                    pCell = $('<th>');
                else
                    pCell = $('<td>');
                pCell.css('text-align', pColumn.HAlign);
                pText = vData.Rows[pY][pKey];
                switch (CStr2(pColumn.DataType).toLowerCase()) {
                case 'number':
                case 'numeric':
                    if (IsNumeric(pText))
                        pText = $.format.number(CDec2(pText), pColumn.Format);
                    break;

                case 'date':
                    if (IsDate(pText)) {
                        if (pColumn.WrapDate)
                            pText = $.format.date(new Date(pText), 'MMM d, yyyy') + '<br />' +
                                $.format.date(new Date(pText), 'h:mm a');
                        else
                            pText = $.format.date(new Date(pText), pColumn.Format);
                    }
                    break;

                };
                pBeforeCellArgs = {
                    Key: pKey,
                    Cell: pCell,
                    Row: pRow,
                    DataRow: vData.Rows[pY],
                    RowNumber: pY,
                    Text: pText
                };
                if (vOptions.BeforeCell(pBeforeCellArgs)) {
                    var pDiv = $('<span>');
                    if ((CStr2(pColumn.DataType).toLocaleLowerCase() == 'date')
                        && (pColumn.WrapDate)
                        && (pBeforeCellArgs.Text == pText)) {

                        pDiv.css('white-space', 'nowrap');
                        pDiv.html(pBeforeCellArgs.Text);
                    } else if (pColumn.AsHtml)
                        pDiv.html(pBeforeCellArgs.Text);
                    else
                        pDiv.text(pBeforeCellArgs.Text);
                    pCell.append(pDiv);
                };
                pRow.append(pCell);
            }; // for pY
            pBody.append(pRow);
            vOptions.AfterRow({
                Row: pRow,
                DataRow: vData.Rows[pY],
                RowNumber: pY
            });
        }; // for pY
        pTable.append(pBody);

        return pTable;

    }; // CreateTable

    (function AjaxFooter() {

        var mHasAjax = false;
        var mFooterVisible = false;
        var mFooterExpanded = false;
        var mTimer;
        var spanFooterElapsedTime = $('#spanFooterElapsedTime');
        var mOldestAjax;

        $('#divFooterAjax').hide();
        $('#divFooterAjaxDetails').hide();

        require(['jqm'], function() {
            AppendFooter();
            function AppendFooter() {
                var pDiv;
                if ($.mobile.activePage != undefined) {
                    $.mobile.activePage.append($('#divFooterAjax').hide());
                    pDiv = $('#divFooterAjax')
                        .addClass('ui-footer ui-bar-d');
                    if ($.isFunction(pDiv.fixedtoolbar))
                        pDiv.fixedtoolbar(); // jqm 1.4.4 doesn't have fixedtoolbar function
                    pDiv.trigger('create');
                } else 
                    setTimeout(AppendFooter, 1000);
            };
        });

        common.AjaxCallback = function() {

            // dont show bottom bar during ajax. may 12, 2015

            //var pHasDelayed = false;
            //var pElapse;
            //var pTopElapse = 0;

            //if (common.AjaxCalls.length == 0) {
            //    if (mHasAjax == false) return;
            //    mHasAjax = false;
            //    clearInterval(mTimer);
            //    $('#divFooterAjax').slideUp();
            //} else {
            //    if (mHasAjax == false) {
            //        mHasAjax = true;
            //        mTimer = setInterval(common.AjaxCallback, 1000);
            //    };
            //};

            //for (var p = 0; p <= common.AjaxCalls.length - 1; p++) {
            //    pElapse = new Date() - common.AjaxCalls[p].Start;
            //    if (pElapse > 2000) {
            //        pHasDelayed = true;
            //        if (pTopElapse < pElapse) {
            //            pTopElapse = pElapse;
            //            mOldestAjax = common.AjaxCalls[p];
            //        };
            //    } else if (pElapse < 0) {
            //        // user set the computer time to a past date/time
            //        common.AjaxCalls[p].Start = new Date();
            //    };
            //};

            //if (pHasDelayed) {
            //    if (mFooterVisible == false) {
            //        mFooterVisible = true;
            //        $('#divFooterAjax').slideDown();
            //    };
            //    if (mFooterExpanded) {
            //        spanFooterElapsedTime.text(Math.round(pElapse / 1000));
            //    };
            //} else {
            //    if (mFooterVisible) {
            //        mFooterVisible = false;
            //        $('#divFooterAjax').slideUp();
            //    };
            //};

        }; // AjaxCallback

        $('#butFooterAjax').click(function() {

            if (mFooterExpanded) {
                mFooterExpanded = false
                $(this).buttonMarkup({icon:'arrow-u'})
                $('#divFooterAjaxDetails').slideUp();
            } else {
                mFooterExpanded = true;
                $(this).buttonMarkup({icon:'arrow-d'})
                $('#divFooterAjaxDetails').slideDown();
            };

        });

        $('#butFooterAjaxAbort').click(function() {
            mOldestAjax.abort();
        });

    })(); // AjaxFooter

    $.widget("mobile.TimeCombo", $.mobile.controlgroup, {

        options: {
            seconds: false,
            type: 'horizontal',
            usecurrent: false // show current date / time
        },

        /** Mandatory method - automatically called by jQuery Mobile to initialise the widget. */
        _create: function() {
            
            
            this.CbHour = $('<select>')
                .data('TimeCombo', this);
            this.element.append(this.CbHour); // must append element before calling selectmenu
            this.CbHour.change(function() {
            });
            InitHour(this);

            this.CbMinute = $('<select>')
                .data('TimeCombo', this);
            this.element.append(this.CbMinute); // must append element before calling selectmenu
            this.CbMinute.change(function() {
            });

            InitMinute(this);

            
            if (this.options.seconds) {
                this.CbSecond = $('<select>')
                    .data('TimeCombo', this);
                this.element.append(this.CbSecond); // must append element before calling selectmenu
                this.CbSecond.change(function() {
                });
                InitSeconds(this);
            };

            this.CbAm = $('<select>')
                .data('TimeCombo', this)
                .append(new Option('AM/PM', ''))
                .append(new Option('AM', 0))
                .append(new Option('PM', 1));
            this.element.append(this.CbAm); // must append element before calling selectmenu
            this.CbAm.change(function() {
                
            });

            function InitHour(e) {

                e.CbHour.append(new Option('h', ''));
                for (var p = 1; p <= 12; p++) 
                    e.CbHour.append(new Option(p));
  
            };

            function InitMinute(e) {
            
                e.CbMinute.append(new Option('m', ''));
                for (var p = 0; p <= 59; p++)
                    e.CbMinute.append(new Option(p));

            };

            function InitSeconds(e) {
                
                e.CbSecond.append(new Option('s', ''));
                for (var p = 0; p <= 59; p++)
                    e.CbSecond.append(new Option(p));

            };
        },
        /** Custom method to handle updates. */
        _update: function() {
            this.element.trigger('create')
                .controlgroup()
                .controlgroup('option', 'type', this.options.type);
            if (this.options.usecurrent)
                this.val(new Date());
        },
        /* Externally callable method cto force a refresh of the widget. */
        refresh: function() {
            return this._update();
        },
        val: function(vValue) {

            var pHour;

            if (vValue == undefined) {
                // getter
                pHour = this.CbHour.val();
                var pMinute = this.CbMinute.val();
                var pAm = this.CbAm.val();
                var pSecond;

                if (this.options.seconds)
                    pSecond = this.CbSecond.val();
                else
                    pSecond = 0;

                if (IsNumeric(pHour)
                    && IsNumeric(pMinute) 
                    && IsNumeric(pAm)
                    && IsNumeric(pSecond))

                    return new Date(1, 1, 1, 
                        parseInt(this.CbHour.val()) + 12 * parseInt(pAm),
                        pMinute, 
                        pSecond);
                else
                    return null;
            } else {
                // setter
                if (!IsDate(vValue)) {
                    this.CbHour.val('');
                    this.CbMinute.val('');
                    this.CbAm.val('');
                    if (this.options.seconds)
                        this.CbSecond.val('');
                } else {
                    vValue = new Date(vValue);
                    pHour = vValue.getHours();

                    if (pHour == 0) {
                        this.CbHour.val(12);
                        this.CbAm.val(0);
                    } else if (pHour == 12) {
                        this.CbHour.val(12);
                        this.CbAm.val(1);
                    } else {
                        this.CbHour.val(pHour % 12);
                        this.CbAm.val(parseInt(pHour / 12));
                    };
                    this.CbMinute.val(vValue.getMinutes());
                    if (this.options.seconds) 
                        this.CbSecond
                            .val(vValue.getSeconds())
                            .selectmenu('refresh');

                    this.CbHour.selectmenu('refresh');
                    this.CbMinute.selectmenu('refresh');
                    this.CbAm.selectmenu('refresh');

                };
            };
                
        }
    }); // $.widget("mobile.TimeCombo", $.mobile.controlgroup, {
    // init TimeCombo before DateCombo
    $('[data-role="TimeCombo"]').TimeCombo(); // dont wait for pagecreate event, it already passed

    $.widget("mobile.DateCombo", $.mobile.controlgroup, {

        options: {
            type: 'horizontal',
            usecurrent: false, // show current date / time
            timecombo: null
        },

        /** Mandatory method - automatically called by jQuery Mobile to initialise the widget. */
        _create: function() {
            
            this.CbMonth = $('<select>')
                .data('DateCombo', this);
            this.element.append(this.CbMonth); // must append element before calling selectmenu
            this.CbMonth.change(function() {
                var pDateCombo = $(this).data('DateCombo');
                pDateCombo.InitDay();
            });
            InitMonth(this);

            this.CbDay = $('<select>')
                .data('DateCombo', this);
            this.element.append(this.CbDay); // must append element before calling selectmenu
            this.CbDay.append(new Option('DD'));
            this.CbDay.change(function() {
                var pDateCombo = $(this).data('DateCombo');

                $(pDateCombo).trigger('DateChanged', [new Date(pDateCombo.CbYear.val(), pDateCombo.CbMonth.val(), pDateCombo.CbDay.val())]);
            });

            this.CbYear = $('<select>')
                .data('DateCombo', this);
            this.element.append(this.CbYear); // must append element before calling selectmenu
            this.CbYear.change(function() {
                var pDateCombo = $(this).data('DateCombo');
                
                pDateCombo.InitDay(pDateCombo);
            });
            InitYear(this);

            // InitDay(this);

            function InitMonth(e) {

                e.CbMonth.append(new Option('MMM'));
                for (var p = 0; p <= 11; p++) 
                    e.CbMonth.append(new Option($.format.locale().date.monthsShort[p], p));
  
            };

           

            function InitYear(e) {
            
                var pYear = new Date().getFullYear();

                e.CbYear.empty();
                e.CbYear.append(new Option('YYYY'));
                for (var p = 1900; p <= pYear; p++)
                    e.CbYear.append(new Option(p));

            };

        },

        InitDay: function() {
            
            var pVal = this.CbDay.val();
            this.CbDay.empty();
            var pDays = new Date(CDec2(this.CbYear.val()), parseInt(this.CbMonth.val()) + 1, 0).getDate();

            this.CbDay.append(new Option('DD'));
            if (pDays.toString() == 'NaN') {
                this.CbDay.selectmenu().selectmenu('refresh');
                return;
            };

            for (var p = 1; p <= pDays; p++) 
                this.CbDay.append(new Option(p));

            if (parseInt(pVal).toString != 'NaN') {
                if (pDays < parseInt(pVal))
                    this.CbDay.val(pDays);
                else
                    this.CbDay.val(pVal);
            };
            this.CbDay.selectmenu().selectmenu('refresh');

        },

        /** Custom method to handle updates. */
        _update: function() {
            this.element.trigger('create')
                .controlgroup()
                .controlgroup('option', 'type', this.options.type);
            if (this.options.timecombo) 
                this.TimeCombo('#' + this.options.timecombo);
            if (this.options.usecurrent)
                this.val(new Date());
        },
        /* Externally callable method to force a refresh of the widget. */
        refresh: function() {
            return this._update();
        },
        val: function(vValue) {

            var pMonth = this.CbMonth.val();
            var pDay = this.CbDay.val();
            var pYear = this.CbYear.val();
            var pHour = 0;
            var pMinute = 0;
            var pSeconds = 0;
            var pDate, pTime;
            
            if (vValue == undefined) {
                // getter
                if (this._TimeCombo != undefined) {
                    pTime = this._TimeCombo.TimeCombo('val');
                    if (!IsDate(pTime)) return;
                    pHour = pTime.getHours();
                    pMinute = pTime.getMinutes();
                    pSeconds = pTime.getSeconds();
                };

                pDate = new Date(pYear, pMonth, pDay, pHour, pMinute, pSeconds);
                return pDate;
            } else {
                // setter
                if (!IsDate(vValue)) return;
                vValue = new Date(vValue);
                this.CbMonth.val(vValue.getMonth());
                this.CbYear.val(vValue.getFullYear());
                this.InitDay();
                this.CbDay.val(vValue.getDate());

                this.CbMonth.selectmenu('refresh');
                this.CbDay.selectmenu('refresh');
                this.CbYear.selectmenu('refresh');

                if (this._TimeCombo != undefined) 
                    this._TimeCombo.TimeCombo('val', vValue);
            };

        },
        TimeCombo: function(vValue) {
            
            if (vValue == undefined)
                return this._TimeCombo;
            else {
                if (vValue == '')
                    this._TimeCombo = null;
                else
                    this._TimeCombo = $(vValue);
            }

        }
    }); // $.widget("mobile.ComboDate", $.mobile.controlgroup, {

    $('[data-role="DateCombo"]').DateCombo(); // dont wait for pagecreate event, it already passed

    var mIsPageShown = false;
    var mShowGotoFirstPage = false;

    (function() {

        if (mIsPageShown)
            return;
              
        if (document.location.hash != '')
            mShowGotoFirstPage = true;

        mIsPageShown = true;

    })();

    window.EnsureFirstPage = function() {
        
        if (!mShowGotoFirstPage)
            return false;

        GotoFirstPage();
        mShowGotoFirstPage = false;
        return true;

    };

    window.ChangeButtonText = function(vButton, vText) {
        vButton = $(vButton);
        vButton.prev('.ui-btn-inner').children('.ui-btn-text').text(vText);
        return vButton;
    };
}); // require


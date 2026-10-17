require([
    'common', 'tablesort',
    '/Scripts3/Offline/Browser/TransInvoiceItems.js?crc='
], function () {
    $(function () {
        var mIsSearched;
        var divOfflineProductHistoryList;

        if (!window.InAndroid) {
            $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
        }
        //            $("[data-role=header]").fixedtoolbar({ tapToggle: false });


        //            var pPage = $.mobile.activePage;
        //            var pHeader = pPage.find("[data-role=header]");
        //            var pHeight = pHeader.height();

        //            pPage.css('padding-top', pHeight + 'px');

        //            pHeader.fixedtoolbar({ tapToggle: false,
        //                updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
        //                // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
        //            });


        breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
        arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
        //            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
        //                ui.prevPage.addClass("ui-dialog-background ");
        //            });

        //            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
        //                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
        //            });

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

        $('#butOfflineProductHistorySearch').click(function(e) {
            Search($('#txtSearchSalesReturn').val());
        });

        $('#formOfflineProductHistorySearch').submit(function(e) {
            e.preventDefault();
            Search();
        }); // END $('#formRequestAndApprovalSearch').click(function(e) {

        divOfflineProductHistoryList = $('#divOfflineProductHistoryList');

        function Search() {
            
            //var pData = {
            //    keywords: $('#txtSearchSalesReturn').val(),
            //    Fowst: cbApprovalFilterFowst.val(),
            //    Status: cbApprovalFilterStatus.val(),
            //    Type: cbApprovalFilterType.val()
            //};

            divOfflineProductHistoryList.html('');

            var vData = {
                        FOWSTID: $('#cbOfflineProductHistoryFilterFowst').val(),
                        Filter: $('#cbOfflineProductHistoryFilterProduct').val()
                        }
            var pDT = OfflineBrowser.TransInvoiceItems.GetData({NonZero:true});
            mIsSearched = true;

            //if (vData.NoResult) {
            //    divOfflineTransactionHistoryList.text('Nothing was found');
            //    return;
            //}
                    
            var pTable = CreateTable(pDT, {
                attr: {
                    'class': 'ui-body-d ui-shadow table-stripe ui-responsive table table-light table-hover table-striped',
                    'data-column-btn-theme': 'b'
                },
                theadAttr: {
                    'class': 'custom-theme-f'
                },
                AddAllColumns: false,
                Columns: {
                    ProductNo: {
                        Text: 'Product No.'
                    },
                    ProductName: {
                        Text: 'Product Name'
                    },
                    Quantity: {
                        Text: 'Quantity',
                        DataType: 'number',
                        Format: '#,###',
                        HAlign: 'right'
                    },
                    Subtotal: {
                        Text: 'Subtotal',
                        DataType: 'number',
                        HAlign: 'right'
                    }
                },
                BeforeCell: function (e) {
                switch (e.Key) {
                    case 'Subtotal':
                        // e.Text = AccountingFormat(e.DataRow.Total);
                        e.Text = AccountingFormat(e.Text);
                        return true;
                        break;
               
                    default:
                        return true;
                }
            },
                //,AfterRow: function(e) {
                //    e.Row.css('cursor', 'pointer');
                //    e.Row.click(function(e1) {
                //        mContextData = $($(e1.target).closest('tr')).data('data');
                //        $('#popApprove').popup('open');
                //    });
                //}
            });
            divOfflineProductHistoryList.append(pTable);
            //pTable.table();
            //pTable.trigger('create');

            var table = $("table").stupidtable({

                "date": function (a, b) {
                    // Get these into date objects for comparison.

                    aDate = new Date(a);
                    bDate = new Date(b);

                    return aDate - bDate;
                }
            });
            

        } // END Search

        (function InitPopup() {
            var pPop = $('#popApprove');

            pPop.find('[name=Approve]').click(function(e) {
                Approve({
                    MemoID:mContextData.ID, 
                    Approve: true
                });
                $('#popApprove').popup('close');
            });

            pPop.find('[name=Disapprove]').click(function(e) {
                Approve({
                    MemoID:mContextData.ID, 
                    Approve: false
                });
                $('#popApprove').popup('close');
            });

            pPop.find('[name=View]').click(function(e) {
                var pUrl;
                pUrl = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    SalesReturnMemoID: mContextData.ID
                }, '/SalesReturns');
                window.open(pUrl, '_blank');
                $('#popApprove').popup('close');
            });

        })(); // END (function InitPopup() {

        function DisplayFowstFilter() {
            cbOfflineProductHistoryFilterFowst = $('#cbOfflineProductHistoryFilterFowst');
            var pOption;

            cbOfflineProductHistoryFilterFowst.html('');
            pOption = new Option('Loading FOWSTs');
            cbOfflineProductHistoryFilterFowst.append(pOption);
            cbOfflineProductHistoryFilterFowst.change(function(e) {
                Search();
            });

            var pDT = OfflineBrowser.LibraryFowst.GetData({});
            var pDR;

            cbOfflineProductHistoryFilterFowst.html('');
            pOption = new Option('All FOWST in my Group', 'all');
            cbOfflineProductHistoryFilterFowst.append(pOption);
            for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                pDR = pDT.Rows[p];
                pOption = new Option(pDR.FowstName, pDR.ID);
                cbOfflineProductHistoryFilterFowst.append(pOption);
            }
            if (!mIsSearched)
                Search();
        }

        DisplayFowstFilter();

    }); // END $(function () {

});
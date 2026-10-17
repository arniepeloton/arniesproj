require(['common', 'tablesort'
    //,GetPathFormat('/Scripts/jquery.dataTables.min.js?crc=')
    //GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js')
], function () {

    var ProductID = '';
    var ProductName = '';
    var UnitMeasureID = '';
    var FowstBinID = '';
    var mFilter = 0; //0 - all, 1 - product, 2 - raw materials
    var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
        arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
    var data;
    var isFinishedLoadIL = false;
    
    var balancesDatatable;

    $(function () {

        //stupid table js

        // Helper function to convert a string of the form "Mar 15, 1987" into
        // a Date object.
        var date_from_string = function (str) {
            var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul",
                          "aug", "sep", "oct", "nov", "dec"];
            var pattern = "^([a-zA-Z]{3})\\s*(\\d{2}),\\s*(\\d{4})$";
            var re = new RegExp(pattern);
            var DateParts = re.exec(str).slice(1);

            var Year = DateParts[2];
            var Month = $.inArray(DateParts[0].toLowerCase(), months);
            var Day = DateParts[1];
            return new Date(Year, Month, Day);
        }

        var moveBlanks = function (a, b) {

            if (a < b) {
                if (a == "")
                    return 1;
                else
                    return -1;
            }
            if (a > b) {
                if (b == "")
                    return -1;
                else
                    return 1;
            }
            return 0;
        };
        var moveBlanksDesc = function (a, b) {
            // Blanks are by definition the smallest value, so we don't have to
            // worry about them here
            if (a < b)
                return 1;
            if (a > b)
                return -1;
            return 0;
        };

        var table = $("table").stupidtable({

            "date": function (a, b) {
                // Get these into date objects for comparison.

                aDate = new Date(a);
                bDate = new Date(b);

                return aDate - bDate;
            },
            "moveBlanks": moveBlanks,
            "moveBlanksDesc": moveBlanksDesc,
            "accounting": function (a, b) {
                if (CDec2(a) < CDec2(b))
                    return 1;
                if (CDec2(a) > CDec2(b))
                    return -1;
                return 0;
            }
        });

        table.on("beforetablesort", function (event, data) {
            // data.column - the index of the column sorted after a click
            // data.direction - the sorting direction (either asc or desc)
            $("#msg").text("Sorting index " + data.column)
            $(this).find('tbody').find('tr').each(function () {
                $(this).find("td:eq(" + data.column + ")").find('b').detach();
            })
        });

        table.on("aftertablesort", function (event, data) {

            var th = $(this).find("th");
            th.find(".arrow").remove();
            var dir = $.fn.stupidtable.dir;

            var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
            th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');

            try {
                $(this).table();
            }
            catch (e) {
                $(this).table().table('refresh');
            }
        });

        $("tr").slice(1).click(function () {

            $(".awesome").removeClass("awesome");
            $(this).addClass("awesome");
        });


        //end of stupid table

        //if we use $('#btnInventoryBalanceLedgerGO'), the page wont load the 2nd time you access it
        DisplayFowst()
        $('body').on('click', '#btnInventoryBalanceOptions', function () {

            ProductID = '';
            sessionStorage.IsFromInventoryBalance = false;

            $('#panInventoryBalanceOptions').popup('open').trigger('create');
            //if (sessionStorage.SelectedFOWST == undefined) {

            //}
            //else {
            //    var data = {
            //        FOWSTID: sessionStorage.SelectedFOWST,
            //        DisplayType: 'fowst',
            //        ProductID: ProductID
            //    }

            //    GetInventoryBalance(data);
            //}
        })
        .on('click', '.btnSelectFowst', function (e) {
            e.preventDefault();
            DisplayFowst();
            return;
            Display.GetAllFowst();
            $('#panHeaderListOfGroupFowst').popup('open');
        }).on('click', '#tblBalances tbody tr td', function () {
            ProductID = balancesDatatable.row(this).data().ProductID;
            ProductName = balancesDatatable.row(this).data().ProductName;
            $('#panTableClick').popup('open');
        }).on('click', '#tblBalances thead tr th', function () {
            
        });

        //$('#txtSearch').keypress(function (e) {
        //    if (e.keyCode == 13) {
        //        BrowsePopup('LibraryPIMProduct', 'Search Product', [
        //            {
        //                Search: $.trim($('#txtSearch').val()),
        //                DisplayText: [{ Column: 'ProductName' }],
        //                Click: function (e) {
        //                    ProductID = e.ID;
        //                    $('#txtSearch').val(e.ProductName);

        //                    var data;

        //                    switch (mFilter) {
        //                        case 0:
        //                            data = {
        //                                FOWSTID: sessionStorage.SelectedFOWST,
        //                                DisplayType: 'fowst',
        //                                ProductID: ProductID
        //                            };
        //                            break;
        //                        case 1:
        //                            data = {
        //                                FOWSTID: sessionStorage.SelectedFOWST,
        //                                DisplayType: 'fowst',
        //                                ProductID: ProductID,
        //                                WithComponentsOnly: 1
        //                            };
        //                            break;
        //                        case 2:
        //                            data = {
        //                                FOWSTID: sessionStorage.SelectedFOWST,
        //                                DisplayType: 'fowst',
        //                                ProductID: ProductID,
        //                                SubcomponentsOnly: 1
        //                            };
        //                            break;
        //                    }

        //                    GetInventoryBalance(data);
        //                }
        //            }
        //        ])
        //    }
        //});

        $('#btnInventoryBalanceOptions').click(function () {
            $('#panInventoryBalanceOptions').popup('open');
        });

        $('#liViewAll').click(function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID
            }

            $('.lblSelectedInventoryType').text('All Inventory Types')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 0;

            GetInventoryBalance(data);

            $('#panInventoryBalanceOptions').popup('close');

        });

        $('#liViewProducts').click(function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID,
                WithComponentsOnly: 1
            }

            $('.lblSelectedInventoryType').text('Finished Goods')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 1;

            GetInventoryBalance(data);

            $('#panInventoryBalanceOptions').popup('close');

        });

        $('#liViewRawMaterials').click(function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID,
                SubcomponentsOnly: 1
            }

            $('.lblSelectedInventoryType').text('Intermediate Goods and Raw Materials')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 2;

            GetInventoryBalance(data);

            $('#panInventoryBalanceOptions').popup('close');

        });

        $('#liViewAllByCategory').click(function () {

            $('.lblSelectedInventoryType').text('All Inventory Types Filtered By Category')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            $('#panInventoryBalanceOptions').popup('close');

            GetProductCategories();
        });

        $('#liViewInventoryLedger').click(function () {
            //            $('.btnMoreOptionInventoryLedger').click();
            sessionStorage.IsFromInventoryBalance = true;
            sessionStorage.InventoryBalanceProductID = ProductID;
            sessionStorage.InventoryBalanceProductName = ProductName;
            //            $('#panTableClick').popup('close');
            //            setTimeout(function () {
            //                $('#panHeaderListOfGroupFowst').popup('close');
            //            }, 1500);
            //$('#divSecond').hide();
            $('#popupViewInventoryLedger').show();
            $('#panTableClick').popup('close');
            if(location.pathname != '/erp'){
                //$('.ItemsBreadCrumbs').text('');
                //breadcrumbs.prepend(arrowR, 'View Inventory Ledger');
                //breadcrumbs.prepend(arrowR, '<a id="btnBCInventoryBalances">Inventory Balance</a>');
                //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                //BreadCrumbsClickEvent();
            } else {
                //for erp
                $('#tabInventoryLedger').empty();
            }
            ViewInventoryLedger();
        });

        $('#liViewSetBeginningBalance').click(function () {
            $('#panTableClick').popup('close');
            setTimeout(function () {
                $('#panSetBegBalance').popup('open');
                GetProductUnitMeasure();
                $('#dtBegBalanceDateTime').val($.format.date(new Date, 'yyyy-MM-ddTHH:mm:ss'))
            }, 500);
        });

        $('#liViewSetInventoryCount').click(function () {
            $('#panTableClick').popup('close');
            setTimeout(function () {
                $('#lblPopUpProductName').text(ProductName);
                $('#panSelectInventoryBalance').popup('open');
                GetBinsBalances()
                GetProductUnitMeasure();
                //GetLibraryFowstBin();
            }, 500);
        });

        $('#liViewSetInventoryCountApproval').click(function () {
            $('#panTableClick').popup('close');
            $('#lblPopUpProductName2').text(ProductName);
            $('#panInventoryCountApproval').show();
            GetInventoryCountForApproval();
        });

        $('#liViewSetInventoryCountApprovalHistory').click(function () {
            $('#panTableClick').popup('close');
            $('#panInventoryCountApprovalHistory').show();
            $('#lblPopUpProductNameAppHistory').text(ProductName);
            GetInventoryCountForApprovalHistory();
        })

        $('#btnInventoryCountApprovalHistoryClose').click(function () {
            $('#panInventoryCountApprovalHistory').hide();
        })


        $('#btnSetInventoryCount').click(function () {
            SaveInventoryCount();
        });

        $('#btnSetBeginningBalance').click(function () {
            SaveBeginningBalance();
        });


        $('#selUnitMeasureForInventoryCount').click(function () {
            UnitMeasureID = $('#selUnitMeasureForInventoryCount').val();
        });

        $('#btnInventoryBalanceClosePopUp').click(function () {
            ProductID = '';
            switch (mFilter) {
                case 0: $('#liViewAll').click(); break
                case 1: $('#liViewProducts').click(); break
                case 2: $('#liViewRawMaterials').click(); break
            }
        });

        $('#btnInventoryCountApprovalDone').click(function () {
            $('#panInventoryCountApproval').hide();
        });

        //$('#selBins').click(function () {
        //    FowstBinID = $('#selBins').val();
        //});

        //$('#btnInventoryBalanceLedgerGO').click(function () {
        //    if (sessionStorage.SelectedFOWST == undefined) {

        //    }
        //    else {
        //        GetInventoryBalance();
        //    }            
        //});       

        (function Init() {
            var pParams = GetUrlParameters();
            var pData;
            // bookmark: arvin, parked

            if ((pParams.View == 'Inventory')
                && (IsGuid(pParams.FowstID))) {

                $('#panHeaderListOfGroupFowst').popup('close');
                $('#panOption').popup('close');
                sessionStorage.SelectedFOWST = pParams.FowstID;
                $('.lblSelectedInventoryType').text('All Inventory Types');

                pData = {
                    FOWSTID: sessionStorage.SelectedFOWST,
                    DisplayType: 'fowst'
                }

                GetInventoryBalance(pData);
            }
            if(location.pathname == '/erp'){
                DisplayFowst()
            }            
        })(); // END Init
    });

    function DisplayFowst() {
        BrowsePopup('LibraryFowst', 'Select FOWST', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'FowstName' }],
                    Click: function (e) {
                        sessionStorage.SelectedFOWST = e.ID;
                        sessionStorage.SelectedFOWSTName = e.FowstName;
                        $('#btnInventoryBalanceFowst > .lblSelectedFowstGroup').text(e.FowstName);
                        $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        $('#btnInventoryLedgerFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        $('button[id="btnInventoryLedgerFowst"]').removeClass('btnRequired').addClass('btnNotRequired');

                        if ($('.lblSelectedInventoryType').text() == "All Inventory Types") {
                            $('#liViewAll').click();
                        }
                        else if ($('.lblSelectedInventoryType').text() == "Finished Goods") {
                            $('#liViewProducts').click();
                        }
                        else if ($('.lblSelectedInventoryType').text() == "Intermediate Goods and Raw Materials") {
                            $('#liViewRawMaterials').click();
                        }
                    }
                }
        ], {IsAll: true})
    }

    function GetInventoryCountForApprovalHistory() {
        var tblInventoryCountApprovalHistoryBody = $('#tblInventoryCountApprovalHistoryBody');

        tblInventoryCountApprovalHistoryBody.empty().append('<tr><td colspan="10" style="text-align:center">Please wait while loading ...</td></tr>');
        AjaxSend({
            url: 'InventoryCount/GetInventoryCountForApprovalHistory',
            data: { ProductID: ProductID, FowstID: sessionStorage.SelectedFOWST },
            success: function (vData) {
                var vRows = vData.Results.Rows;
                tblInventoryCountApprovalHistoryBody.empty()

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {
                        tblInventoryCountApprovalHistoryBody
                            .append($('<tr>')
                                .append($('<td>')
                                    .text((i + 1) + '.')
                                )
                                .append($('<td>')
                                    .text(vRows[i].FOWST)
                                )
                                .append($('<td>')
                                    .text(vRows[i].Bin)
                                )
                                .append($('<td>')
                                    .text($.format.date(new Date(vRows[i].CountDate), 'MMM dd, yyyy hh:mm a'))
                                )
                                .append($('<td>')
                                    .text(vRows[i].Quantity)
                                )
                                .append($('<td>')
                                    .text(vRows[i].UnitMeasure)
                                )
                                .append($('<td>')
                                    .text(vRows[i].Variance)
                                )
                                .append($('<td>')
                                    .css('color', (vRows[i].Status == 'Approved' ? '#168E03' : '#F00'))
                                    .text(vRows[i].Status)
                                )
                                .append($('<td>')
                                    .text(vRows[i].ModifiedBy)
                                )
                                .append($('<td>')
                                    .text($.format.date(new Date(vRows[i].ModifiedDate), 'MMM dd, yyyy hh:mm a'))
                                )
                            )
                    }
                }
                else {
                    tblInventoryCountApprovalHistoryBody.append('<tr><td colspan="10" style="text-align:center">No Result(s) Found</td></tr>')
                }

                tblInventoryCountApprovalHistoryBody.parent().table('refresh');
            }
        })
    }

    function GetProductCategories() {
        var mdata = { ProductID: ProductID };

        AjaxSend({
            url: 'InventoryLedger/GetCategories',
            data: mdata,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    var vRows = vData.Results.Rows;

                    var mCategories = [];


                    for (var i = 0; i <= vRows.length - 1; i++) {
                        mCategories.push({
                            Text: vRows[i].Category,
                            Click: function () {
                                data = {
                                    FOWSTID: sessionStorage.SelectedFOWST,
                                    DisplayType: 'fowst',
                                    ProductID: ProductID,
                                    Category: $(this)[0].Text
                                }

                                GetInventoryBalance(data);

                            }
                        })
                    }

                    PopupButtonList('', 'Categories',
                            mCategories, { AutoDismiss: true })

                }
            }
        });
    }

    function ViewInventoryLedger() {
        AjaxSend({
            url: 'Login3/TestCookies',
            success: function (vData) {

                PromptUserAccess('Group Settings', {
                    OnClickOKSuccess: function () {
                        if (location.hash == '#&ui-state=dialog')
                            history.back();

                        AjaxSend({
                            ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                sessionStorage["Inventory"] = "ledger";
                                $('#divForInventoryLedger').empty();
                                $('#divForInventoryLedger').append(vData).trigger('create');
                                App.Deferred.fulfill('FQ26E5TP76');

                                if (isFinishedLoadIL) {
                                    InventoryLedger.CheckSource();
                                }
                                isFinishedLoadIL = true;
                            }
                        });

                        //$('#divSecond').hide();
                        $('#popupViewInventoryLedger').show();
                    }
                });
            },
            error: function (vError) {
                NewMessageBox("This feature requires internet connection.",
                    "Requires Internet Connection",
                    [
                        {
                            Text: 'Ok'
                        }
                    ]
                );
                return;
            }
        });
    }

    function BreadCrumbsClickEvent() {
        $('#btnMainDashboard').click(function () {

            $('#divMainDashboard, .btnPeriod, .lblPeriod').show();
            //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
            $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                                                ' #divOrderCentral,' +
                                                '#divSpecialReports, #divAuditTrail,' +
                                                '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage, #divInventoryProductBreakdown').hide();
            $('#divFranchiseRequest').hide();
            $('#divSalesReplenishmentProcessing').hide();
            $('#divSalesOrderProcessing').hide();
            $('.ItemsBreadCrumbs').text('Main Dashboard');
            TempValueBtnBack = null;
            sessionStorage.SearchMode = 'false';
            GetData();
            $('.btnSelectFowst, #divBtnPeriod2').hide();
        });

        $('#btnBCInventoryBalances').click(function () {
            //$('#divSecond').show();
            $('#popupViewInventoryLedger').hide();
            $('.ItemsBreadCrumbs').text('');
            breadcrumbs.prepend(arrowR, 'Inventory Balances');
            $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
            BreadCrumbsClickEvent();
        });
    }

    function GetBinsBalances() {
        var data = {
            ProductID: ProductID,
            FOWSTID: sessionStorage.SelectedFOWST
        }

        AjaxSend({
            url: 'InventoryCount/GetBinBalances',
            data: data,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayBinsBalances(vData.Results.Rows);
                }
            }
        });
    }

    function DisplayBinsBalances(vRows) {

        var pLV = $('#tblInventoryCountBody');

        pLV.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {

            pItem = $('<tr>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .css('cursor', 'pointer')
                        .append($('<td>')
                            .text(vRows[i].BinName)
                        )
            //.append($('<td>')
            //    .text(vRows[i].Qty)
            //)
                        .append($('<td>')
                            .text(vRows[i].Balance)
                        )
                        .append($('<td>')
                            .html('<a href="#" data-role="button">Edit Balance</a>')
                        )
                        .click(function () {
                            //$('#dvInventoryCount').css('visibility', 'visible')
                            //$('#dvInventoryCount').css('display', 'block')

                            $('#lblBin').text('Bin Name: ' + $(this).data('data').BinName);

                            FowstBinID = $(this).data('data').ID;

                            $('#panSelectInventoryBalance').popup('close');
                            $('#panSetInventoryBalance').popup('open');
                        })

            pLV.append(pItem);
        }

    }

    function GetProductUnitMeasure() {
        var mdata = { ProductID: ProductID };

        AjaxSend({
            url: 'InventoryLedger/GetStockUnitMeasures',
            data: mdata,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    var vRows = vData.Results.Rows;
                    var pLV = $('#selUnitMeasure');
                    var pLV2 = $('#selUnitMeasureForInventoryCount');

                    pLV.empty();
                    pLV2.empty();

                    UnitMeasureID = vRows[0].ID;
                    //pLV.append(new Option('Select Unit Measure', '', false));

                    for (var i = 0; i <= vRows.length - 1; i++) {
                        pLV.append(new Option(vRows[i].UnitMeasure, vRows[i].ID, false));
                        pLV2.append(new Option(vRows[i].UnitMeasure, vRows[i].ID, false));
                    }

                    try {
                        pLV.selectmenu('refresh');
                        pLV2.selectmenu('refresh');
                    }
                    catch (e) {
                        pLV.selectmenu();
                        pLV2.selectmenu();
                    }
                }
            }
        });
    }

    function GetInventoryCountForApproval() {
        var mdata = { ProductID: ProductID };

        $('#tblInventoryCountApprovalBody').empty().append('<tr><td colspan="10" style="text-align:center">Please wait while loading ...</td></tr>')

        AjaxSend({
            url: 'InventoryCount/GetInventoryCountForApproval',
            data: mdata,
            success: function (vData) {
                $('#tblInventoryCountApprovalBody').empty();

                if (vData.Results.Rows.length > 0) {
                    DisplayInventoryCountForApproval(vData.Results.Rows);
                }
                else {
                    $('#tblInventoryCountApprovalBody').append('<tr><td colspan="10" style="text-align:center">No Result(s) Found</td></tr>')
                }
            }
        });
    }

    function DisplayInventoryCountForApproval(vRows) {
        var pLV = $('#tblInventoryCountApprovalBody');

        pLV.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {

            pItem = $('<tr>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .css('cursor', 'pointer')
                        .append($('<td>')
                            .text((i + 1) + '.')
                        )
                        .append($('<td>')
                            .text(vRows[i].FOWST)
                        )
                        .append($('<td>')
                            .text(vRows[i].Bin)
                        )
                        .append($('<td>')
                            .text(vRows[i].CountDate)
                        )
                        .append($('<td>')
                            .text(vRows[i].Quantity)
                        )
                        .append($('<td>')
                            .text(vRows[i].UnitMeasure)
                        )
                        .append($('<td>')
                            .text(vRows[i].Variance)
                        )
                        .append($('<td>')
                            .html('<a href="#" data-role="button">Approve Count </a>')
                            .attr('ID', vRows[i].ID)
                            .data('data', vRows[i])
                            .click(function (e) {
                                e.preventDefault();

                                var data = {
                                    ID: $(this).attr('ID'),
                                    CountDate: $(this).data('data').CountDate,
                                    InventoryCountID: $(this).data('data').InventoryProductCountID,
                                    FOWSTID: sessionStorage.SelectedFOWST,
                                    ProductID: ProductID,
                                    BinID: $(this).data('data').BinID,
                                    Variance: $(this).data('data').Variance,
                                    Count: $(this).data('data').Quantity,
                                    IsVarianceDisApproved: 0
                                }

                                ApprovedInventoryCount(data);
                            })
                        )
                        .append($('<td>')
                            .html('<a href="#" data-role="button">Disapprove Count </a>')
                            .attr('ID', vRows[i].ID)
                            .data('data', vRows[i])
                            .click(function (e) {
                                e.preventDefault();

                                var data = {
                                    ID: $(this).attr('ID'),
                                    CountDate: $(this).data('data').CountDate,
                                    InventoryCountID: $(this).data('data').InventoryProductCountID,
                                    FOWSTID: sessionStorage.SelectedFOWST,
                                    ProductID: ProductID,
                                    BinID: $(this).data('data').BinID,
                                    Variance: $(this).data('data').Variance,
                                    Count: $(this).data('data').Quantity,
                                    IsVarianceDisApproved: 1
                                }

                                ApprovedInventoryCount(data);
                            })
                        )
            //.append($('<td>')
            //    .html('<a href="#" data-role="button">Reject Count</a>')
            //)
            //.click(function () {
            //    $('#dvInventoryCount').css('visibility', 'visible')
            //    $('#dvInventoryCount').css('display', 'block')

            //    $('#lblBin').text('Bin Name: ' + $(this).data('data').BinName);

            //    FowstBinID = $(this).data('data').ID;
            //})

            pLV.append(pItem);
        }

        pLV.parent().table('refresh');
    }

    function ApprovedInventoryCount(mdata) {

        AjaxSend({
            url: 'InventoryCount/ApprovedInventoryCount',
            data: mdata,
            success: function (vData) {
                if (vData.Results == 'True') {
                    $('#panInventoryCountApproval').hide();

                    data = {
                        FOWSTID: sessionStorage.SelectedFOWST,
                        DisplayType: 'fowst'
                        //,ProductID: ProductID
                    }

                    GetInventoryBalance(data);
                }
            }
        });
    }

    function GetLibraryFowstBin() {
        var data = {
            FowstID: sessionStorage.SelectedFOWST,
            NoFilter: 1
        }

        AjaxSend({
            url: 'Browser/LibraryFowstBin',
            data: data,
            success: function (vData) {
                DisplayFowstBin(vData.Results);
            }
        });
    }

    function DisplayFowstBin(vData) {
        $('#selBins').empty();

        if (vData.Rows.length > 0) {
            FowstBinID = vData.Rows[0].ID;

            for (var i = 0; i <= vData.Rows.length - 1; i++) {
                if (vData.Rows[i].Name != 'Unnamed Bin') {
                    $('#selBins').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));
                }
            }

            try {
                $('#selBins').selectmenu('refresh');

            } catch (e) {
                $('#selBins').selectmenu();
            }
        }
    }

    function SaveInventoryCount() {
        var Count = [];

        if (IsNumeric($('#txtInventoryCount').val())) {
            var data = {
                ID: '',
                IsNew: 1,
                FowstID: sessionStorage.SelectedFOWST,
                ProductID: ProductID,
                Count: Count
            }

            Count[0] = {
                BinID: FowstBinID,
                ExpirationDate: 'N/A',
                UnitMeasureID: UnitMeasureID,
                Quantity: $('#txtInventoryCount').val(),
                ActualQuantity: 1,
                IsGoodStock: 1
            };

            AjaxSend({
                url: 'InventoryCount/SaveInventoryCount',
                data: data,
                success: function (vData) {
                    if (vData.Save == "True") {

                        $('#panSetInventoryBalance').popup('close');

                        data = {
                            FOWSTID: sessionStorage.SelectedFOWST,
                            DisplayType: 'fowst',
                            ProductID: ProductID
                        }

                        GetInventoryBalance(data);


                    } else if (CStr2(vData.Save) != '') {

                    } //end if
                }
            });
        } else {
            //error here
        }
    }

    function SaveBeginningBalance() {

        if ($('#txtBeginningBalance').val() == '') {
            NewMessageBox("Please input Balance", "Beginning Balance");
            return
        }

        if (!IsDate($('#dtBegBalanceDateTime').val())) {
            NewMessageBox("Please input proper Date and Time", "Beginning Balance");
            return;
        }

        var mData = {
            ProductID: ProductID,
            BegBalance: $('#txtBeginningBalance').val(),
            DateFrom: $('#dtBegBalanceDateTime').val(),
            FOWSTID: sessionStorage.SelectedFOWST
        }

        AjaxSend({
            url: 'InventoryLedger/SetBeginningBalance',
            data: mData,
            success: function (vData) {

                //reset product id to prevent filtering the table when requery is called
                ProductID = '';

                if (vData.Results == 'Success') {
                    switch (mFilter) {
                        case 0: $('#liViewAll').click(); break
                        case 1: $('#liViewProducts').click(); break
                        case 2: $('#liViewRawMaterials').click(); break
                    }
                }
                $('#panSetBegBalance').popup('close');
            }
        });
    }

    function GetInventoryBalance(data) {

        AjaxSend({
            url: 'InventoryLedger/GetReport',
            data: data,
            success: function (vData) {

                var vRows = vData.data.Rows;

                //if (vRows.length > 0) {
                //    $('#dvDetails').css('visibility', 'visible');
                //    $('#dvSummary').css('visibility', 'visible');
                //    $('#dvDetails').css('display', 'block');
                //    $('#dvSummary').css('display', 'block');
                //    $('#btnBeginningBalance').css('visibility', 'visible');
                //    $('#divNoPage').css('visibility', 'hidden');

                    DisplayBalances(vRows);

                //}

                //else {
                //    $('#dvDetails').css('display', 'none');
                //    $('#dvSummary').css('display', 'none');
                //    $('#dvDetails').css('visibility', 'hidden');
                //    $('#dvSummary').css('visibility', 'hidden');
                //    $('#btnBeginningBalance').css('visibility', 'hidden');
                //    $('#divNoPage').css('visibility', 'visible');
                //}
            }
        });
    }

    function DisplayBalances(vRows) {
        //var pLV = $('#tblBody');

        //pLV.empty();

        //for (var i = 0; i <= vRows.length - 1; i++) {

        //    //.append($('<td>')
        //    //            .text(vRows[i].InventoryCountApproval)
        //    //        )

        //    if (CInt2(vRows[i].InventoryCountApproval) != 0) {
        //        var backGroundColor = 'orange';

        //        pItem = $('<tr>')
        //            .attr('ID', vRows[i].ProductID)
        //            .data('data', vRows[i])
        //            .css('cursor', 'pointer')
        //            .append($('<td style="background-color:orange">')
        //                .text(vRows[i].ProductNo)
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(vRows[i].ProductName)
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(AccountingFormat(CDec2(vRows[i].Balance)))
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(vRows[i].UnitMeasure)
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(AccountingFormat(vRows[i].COGS))
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(AccountingFormat(vRows[i].SRP))
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(vRows[i].LastUpdated)
        //            )
        //            .append($('<td style="background-color:orange">')
        //                .text(vRows[i].InventoryCountApproval)
        //            )
        //            .click(function () {
        //                ProductID = $(this).attr('ID');
        //                ProductName = $(this).data('data').ProductName;
        //                $('#panTableClick').popup('open');
        //            })

        //    } else {
        //        pItem = $('<tr>')
        //            .attr('ID', vRows[i].ProductID)
        //            .data('data', vRows[i])
        //            .css('cursor', 'pointer')
        //            .append($('<td>')
        //                .text(vRows[i].ProductNo)
        //            )
        //            .append($('<td>')
        //                .text(vRows[i].ProductName)
        //            )
        //            .append($('<td>')
        //                .text(AccountingFormat(CDec2(vRows[i].Balance)))
        //            )
        //            .append($('<td>')
        //                .text(vRows[i].UnitMeasure)
        //            )
        //            .append($('<td>')
        //                .text(AccountingFormat(vRows[i].COGS))
        //            )
        //            .append($('<td>')
        //                .text(AccountingFormat(vRows[i].SRP))
        //            )
        //            .append($('<td>')
        //                .text(vRows[i].LastUpdated)
        //            )
        //            .append($('<td>')
        //                .text(vRows[i].InventoryCountApproval)
        //            )
        //            .click(function () {
        //                ProductID = $(this).attr('ID');
        //                ProductName = $(this).data('data').ProductName;
        //                $('#panTableClick').popup('open');
        //            })
        //    }


        //    pLV.append(pItem);
        //}
        //pLV.parent().addClass('custom-tbl-responsive').table('refresh')
        var BalancesTable = $('#tblBalances'); // diri ni ibutang kay kung sa taas kay dili na magload ang datatable
        if ($.fn.DataTable.isDataTable(BalancesTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            BalancesTable.empty();
            balancesDatatable.destroy();
        }

        var buttons = [];
        if(!IsMobile){
            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inventory Balance' })
        }

        balancesDatatable = BalancesTable.DataTable({
            "data": vRows,
            "columns": [                
                { "data": "ProductNo" },
                { "data": "ProductName" },
                { "data": "Balance" },
                { "data": "UnitMeasure" },
                { "data": "COGS" },
                { "data": "SRP" },
                { "data": "LastUpdated" },
                { "data": "InventoryCountApproval" }
            ],
            "columnDefs": [
            {
                'targets': [ 2 ],                
                'render': function (data, type, full, meta) {
                    return AccountingFormat(data);
                }
            },
            {
                'targets': [2],
                'render': function (data, type, full, meta) {
                    return AccountingFormat(data);
                }
            },
            {
                'targets': [4],
                'render': function (data, type, full, meta) {
                    return AccountingFormat(data);
                }
            },
            {
                'targets': [5],
                'render': function (data, type, full, meta) {
                    return AccountingFormat(data);
                }
            },
            ],
            //change background color
            "fnRowCallback": function (nRow, aData) {
                if (CInt2(aData.InventoryCountApproval) > 0) {
                    $(nRow).attr('style', 'background: orange !important');
                }
            },
            //para dili machange ang color sa selected column
            "orderClasses" : false,
            "lengthMenu": [[25, 50, -1], [25, 50, "All"]],
            "language": {
                "search": "Search Product "
            },
            dom: 'Bfrtip',
            buttons: buttons,
            fnPreDrawCallback: function (oSettings) { // call every click onto pagination
                //BalancesTable.table('refresh');
            }
        });
    }

});
require(['common', 'tablesort',
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js')
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
    var isFilterInvBal = false;
    var categoryname = '';
    var isFinishedLoadCategory = false;

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
            "moveBlanksDesc": moveBlanksDesc
        });

        table.on("beforetablesort", function (event, data) {
            // data.column - the index of the column sorted after a click
            // data.direction - the sorting direction (either asc or desc)
            $("#msg").text("Sorting index " + data.column)
        });

        table.on("aftertablesort", function (event, data) {

            var th = $(this).find("th");
            th.find(".arrow").remove();
            var dir = $.fn.stupidtable.dir;

            var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
            th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');




        });

        $("tr").slice(1).click(function () {

            $(".awesome").removeClass("awesome");
            $(this).addClass("awesome");
        });


        //end of stupid table

        //if we use $('#btnInventoryBalanceLedgerGO'), the page wont load the 2nd time you access it

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
            $('#panHeaderListOfGroupFowst').popup('open');
        })
        .on('click', '#btnInventoryBalanceCategory', function () {
            $('#panInventoryBalanceCategory').popup('open');
            if (isFinishedLoadCategory == false) {
                GetCategories();
            }
            isFinishedLoadCategory = true;
        })

        $('#txtSearch').keypress(function (e) {
            if (e.keyCode == 13) {
                Browser.LibraryPIMProduct.Click = function (e) {
                    ProductID = e.ID;
                    $('#txtSearch').val(e.ProductName);

                    var data;

                    switch (mFilter) {
                        case 0:
                            data = {
                                FOWSTID: sessionStorage.SelectedFOWST,
                                DisplayType: 'fowst',
                                ProductID: ProductID
                            };
                            break;
                        case 1:
                            data = {
                                FOWSTID: sessionStorage.SelectedFOWST,
                                DisplayType: 'fowst',
                                ProductID: ProductID,
                                WithComponentsOnly: 1
                            };
                            break;
                        case 2:
                            data = {
                                FOWSTID: sessionStorage.SelectedFOWST,
                                DisplayType: 'fowst',
                                ProductID: ProductID,
                                SubcomponentsOnly: 1
                            };
                            break;
                    }

                    GetInventoryBalance(data);
                }
                Browser.LibraryPIMProduct.Show($('#txtSearch').val());

            }
        });

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

        $('#liViewInventoryLedger').click(function () {
            //            $('.btnMoreOptionInventoryLedger').click();
            sessionStorage.IsFromInventoryBalance = true;
            sessionStorage.InventoryBalanceProductID = ProductID;
            sessionStorage.InventoryBalanceProductName = ProductName;
            //            $('#panTableClick').popup('close');
            //            setTimeout(function () {
            //                $('#panHeaderListOfGroupFowst').popup('close');
            //            }, 1500);
            $('#divSecond').hide();
            $('#divForInventoryLedger').show();
            $('#panTableClick').popup('close');
            $('.ItemsBreadCrumbs').text('');
            breadcrumbs.prepend(arrowR, 'View Inventory Ledger');
            breadcrumbs.prepend(arrowR, '<a id="btnBCInventoryBalances">Inventory Balance</a>');
            $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
            BreadCrumbsClickEvent();
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
            setTimeout(function () {
                $('#lblPopUpProductName2').text(ProductName);
                $('#panInventoryCountApproval').popup('open');
                GetInventoryCountForApproval();
            }, 500);
        });

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
            $('#panInventoryCountApproval').popup('close');
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
    });

    function GetCategories() {
        var pLV = $('#lstInvBalCategory');

        pLV.empty();
        pLV.append($('<li>').append($('<a>').text('Please wait...'))).listview('refresh');
        AjaxSend({
            url: 'InventoryLedger/GetCategories',
            success: function (vData) {
                pLV.empty();
                pLV.append($('<li>').append($('<a>').text('None').click(function () {
                    categoryname = $(this).text();
                    $('#lblSelectedCategory').text(categoryname);
                    if (categoryname == "None") {
                        isFilterInvBal = false;
                    }

                    if ($('.lblSelectedFowstGroup').text() != 'FOWST Location' || $('.lblSelectedInventoryType').text() != 'Choose Inventory Type') {
                        if ($('.lblSelectedInventoryType').text() == 'All Inventory Types') {
                            $('#liViewAll').click();
                        }
                        else if ($('.lblSelectedInventoryType').text() == 'Finished Goods') {
                            $('#liViewProducts').click();
                        }
                        else {
                            $('#liViewRawMaterials').click();
                        }
                    }
                    $('#panInventoryBalanceCategory').popup('close');
                })));
                if (vData.Results.Rows.length > 0) {
                    for (var i = 0; i < vData.Results.Rows.length; i++) {
                        pLV.append($('<li>')
                                    .append($('<a>')
                                        .text(vData.Results.Rows[i].Category)
                                        .click(function (e) {
                                            isFilterInvBal = true;
                                            categoryname = $(this).text();
                                            $('#lblSelectedCategory').text(categoryname);

                                            if (categoryname == "None") {
                                                isFilterInvBal = false;
                                            }

                                            if ($('.lblSelectedFowstGroup').text() != 'FOWST Location' || $('.lblSelectedInventoryType').text() != 'Choose Inventory Type') {
                                                if ($('.lblSelectedInventoryType').text() == 'All Inventory Types') {
                                                    $('#liViewAll').click();
                                                }
                                                else if ($('.lblSelectedInventoryType').text() == 'Finished Goods') {
                                                    $('#liViewProducts').click();
                                                }
                                                else {
                                                    $('#liViewRawMaterials').click();
                                                }
                                            }
                                            $('#panInventoryBalanceCategory').popup('close');
                                        })
                                    )
                        )
                    }
                }
                else {
                    pLV.append($('<li>').append($('<a>').text('No Result(s) Found')));
                }

                pLV.listview('refresh');
                PopupReposition();
            }
        })
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

                        $('#divSecond').hide();
                        $('#divForInventoryLedger').show();
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
            $('#divSecond').show();
            $('#divForInventoryLedger').hide();
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

        AjaxSend({
            url: 'InventoryCount/GetInventoryCountForApproval',
            data: mdata,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayInventoryCountForApproval(vData.Results.Rows);
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
                                    Count: $(this).data('data').Quantity
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
    }

    function ApprovedInventoryCount(mdata) {

        AjaxSend({
            url: 'InventoryCount/ApprovedInventoryCount',
            data: mdata,
            success: function (vData) {
                if (vData.Results == 'True') {
                    $('#panInventoryCountApproval').popup('close');

                    data = {
                        FOWSTID: sessionStorage.SelectedFOWST,
                        DisplayType: 'fowst',
                        ProductID: ProductID
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

        if (isFilterInvBal) {
            data.FilterCategory = categoryname;
        }
        else {
            data.FilterCategory = '';
        }

        AjaxSend({
            url: 'InventoryLedger/GetReport',
            data: data,
            success: function (vData) {

                var vRows = vData.data.Rows;

                if (vRows.length > 0) {
                    $('#dvDetails').css('visibility', 'visible');
                    $('#dvSummary').css('visibility', 'visible');
                    $('#dvDetails').css('display', 'block');
                    $('#dvSummary').css('display', 'block');
                    $('#btnBeginningBalance').css('visibility', 'visible');
                    $('#divNoPage').css('visibility', 'hidden');

                    DisplayBalances(vRows);

                }

                else {
                    $('#dvDetails').css('display', 'none');
                    $('#dvSummary').css('display', 'none');
                    $('#dvDetails').css('visibility', 'hidden');
                    $('#dvSummary').css('visibility', 'hidden');
                    $('#btnBeginningBalance').css('visibility', 'hidden');
                    $('#divNoPage').css('visibility', 'visible');
                }
            }
        });
    }

    function DisplayBalances(vRows) {
        var pLV = $('#tblBody');

        pLV.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {

            //.append($('<td>')
            //            .text(vRows[i].InventoryCountApproval)
            //        )

            if (CInt2(vRows[i].InventoryCountApproval) != 0) {
                var backGroundColor = 'orange';

                pItem = $('<tr>')
                    .attr('ID', vRows[i].ProductID)
                    .data('data', vRows[i])
                    .css('cursor', 'pointer')
                    .append($('<td style="background-color:orange">')
                        .text(vRows[i].ProductNo)
                    )
                    .append($('<td style="background-color:orange">')
                        .text(vRows[i].ProductName)
                    )
                    .append($('<td style="background-color:orange">')
                        .text(AccountingFormat(CDec2(vRows[i].Balance)))
                    )
                    .append($('<td style="background-color:orange">')
                        .text(vRows[i].UnitMeasure)
                    )
                    .append($('<td style="background-color:orange">')
                        .text(AccountingFormat(vRows[i].COGS))
                    )
                    .append($('<td style="background-color:orange">')
                        .text(AccountingFormat(vRows[i].SRP))
                    )
                    .append($('<td style="background-color:orange">')
                        .text(vRows[i].LastUpdated)
                    )
                    .append($('<td style="background-color:orange">')
                        .text(vRows[i].InventoryCountApproval)
                    )
                    .click(function () {
                        ProductID = $(this).attr('ID');
                        ProductName = $(this).data('data').ProductName;
                        $('#panTableClick').popup('open');
                    })

            } else {
                pItem = $('<tr>')
                    .attr('ID', vRows[i].ProductID)
                    .data('data', vRows[i])
                    .css('cursor', 'pointer')
                    .append($('<td>')
                        .text(vRows[i].ProductNo)
                    )
                    .append($('<td>')
                        .text(vRows[i].ProductName)
                    )
                    .append($('<td>')
                        .text(AccountingFormat(CDec2(vRows[i].Balance)))
                    )
                    .append($('<td>')
                        .text(vRows[i].UnitMeasure)
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].COGS))
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].SRP))
                    )
                    .append($('<td>')
                        .text(vRows[i].LastUpdated)
                    )
                    .append($('<td>')
                        .text(vRows[i].InventoryCountApproval)
                    )
                    .click(function () {
                        ProductID = $(this).attr('ID');
                        ProductName = $(this).data('data').ProductName;
                        $('#panTableClick').popup('open');
                    })
            }


            pLV.append(pItem);
        }

    }

});
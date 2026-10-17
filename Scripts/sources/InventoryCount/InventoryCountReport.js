require([
    'common',
    'tablesort',
    GetPathFormat('/Scripts3/model/InventoryCountReport_Offline.js?crc='),
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
    ], function () {

        var InventoryCountReportOffline = {};
        var isDoneClicked = false;
        var isInventoryLedgerLoaded = '';

        InventoryCountReportOffline = new App.model.InventoryCountReport_Offline();

        require([(OfflineMode ? '/Scripts3/model/InventoryCountReport_Offline.js?crc=' : '')], function () {
            var ProductID = '';
            var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
                arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';

            $(function () {
                GetNamedFowst();

                window.Report = function () {
                    GetNamedFowst();
                    $('#divInventoryReport').attachDragger();
                }

                window.InventoryCountReport = {
                    Report: Report(),
                    ClearFields: function () {
                        $('#selReportRange, #selFowst').val('');
                        $('#tblInventoryReportBody').empty();
                    }
                }

                $('#divInventoryReport').attachDragger();

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

                $('body').on('click', '#btnViewReport', function () {
                    if ($('#selReportRange').val() == "") {
                        $('#selReportRange').focus();
                        return false;
                    }

                    if ($('#selFowst').val() == "") {
                        $('#selFowst').focus();
                        return false;
                    }
                    sessionStorage.SelectedFOWSTName = $('#selFowst option:selected').text();
                    sessionStorage.SelectedFOWST = $('#selFowst').val();
                    GetInventoryCountReport();
                })

            }); //End Inner function        

            function GetNamedFowst() {
                if (OfflineMode) {
                    //Sql.AttachDatabase(sessionStorage.OrgID);
                    if (Web.HandleOrganization() == false)
                        return;
                    var vData = OfflineBrowser.LibraryFowst.GetData({
                        FilterFowst: 1
                    });
                    DisplayFowst(vData);
                }
                else {
                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        success: function (vData) {
                            DisplayFowst(vData.Results);
                        }
                    });
                }
            }

            function DisplayFowst(vRows) {
                $('#selFowst').empty();
                $('#selFowst').append(new Option('-Select FOWST-', '', false));

                for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                    if (vRows.Rows[i].Name != 'Unnamed Bin') {
                        $('#selFowst').append(new Option(vRows.Rows[i].FowstName, vRows.Rows[i].ID, false));
                    }
                }

                $('#selFowst').append(new Option("All FOWST", 'AllFowst', false));

                if (vRows.Rows.length == 1) {
                    $('#selFowst').val(vRows.Rows[0].ID)
                }

                try {
                    //$('#selFowst').selectmenu('refresh');
                }
                catch (e) {
                    //$('#selFowst').selectmenu();
                }
            }

            function GetInventoryCountReport() {
                data = {
                    Within: $('#selReportRange option:selected').val(),
                    FowstID: $('#selFowst option:selected').val()
                    //ProductID: ProductID
                }

                if (CBool(sessionStorage.PrintingOfflineData)) {
                    //if (OfflineMode) {
                    var vData;
                    vData = InventoryCountReportOffline.GetInventoryCountReport(data);

                    DisplayInventoryCountReport(vData.Rows);
                }
                else {
                    AjaxSend({
                        url: 'InventoryCount/GetInventoryCountReport',
                        data: data,
                        success: function (vData) {
                            DisplayInventoryCountReport(vData.InventoryCountReport.Rows)
                        }
                    });
                }
            }

            function DisplayInventoryCountReport(vRows) {
                var pLV = $("#tblInventoryReportBody");
                var pLV2 = $('#tblInventoryReportTotalsBody');

                pLV.html('');
                pLV2.html('');

                if (vRows.length > 0) {
                    for (var i = 0; i <= vRows.length - 1; i++) {

                        //if (i < (vRows.length - 3)) {

                        pItem = $('<tr>')
                        //.append($('<th>')
                        //    .text(vRows[i].BIN)
                        //)
                            .append($('<td>')
                                .text(vRows[i].ProductNo)
                            )
                            .append($('<td>')
                                .text(vRows[i].ProductGroupNo)
                            )
                            .append($('<td>')
                                .text(vRows[i].Category)
                            )
                            .append($('<td>')
                                .append($('<a>')
                                    .attr('ID', vRows[i].ProductID)
                                    .data('data', vRows[i])
                                    .attr('href', '#')
                                    .text(vRows[i].ProductName)
                                    .click(function () {

                                        $('#lblWithin').text($('#selReportRange option:selected').text());

                                        sessionStorage.IsFromInventoryBalance = true;
                                        sessionStorage.InventoryBalanceProductID = $(this).data('data').ProductID;
                                        sessionStorage.InventoryBalanceProductName = $(this).data('data').ProductName;
                                        sessionStorage.SelectedFOWST = $('#selFowst').val();

                                        $('#divForInventoryLedger').pageshow();
                                        $('#panTableClick').popup('close');

                                        AddBreadCrumb({
                                            text: 'View Inventory Ledger', Click: function () {
                                                $('#divForPhysicalInventory').pageshow();
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        if (OfflineMode) {
                                            ViewInventoryLedgerOffline();                                            
                                        }
                                        else {
                                            ViewInventoryLedger();
                                        }

                                        Display.GetAllFowst();
                                    })
                                )
                            )
                            .append($('<td>')
                                .text(vRows[i].UnitMeasure)
                            )
                            .append($('<td data-sort-value="' + CDec2(vRows[i].CountOnRecord) + '">')
                                .text(CDec2(vRows[i].CountOnRecord))
                            )
                            .append($('<td data-sort-value="' + CDec2(vRows[i].PhysicalCount) + '">')
                                .text(CDec2(vRows[i].PhysicalCount))
                            )
                            .append($('<td>')
                                .text(vRows[i].CountDate)
                            )
                            .append($('<td data-sort-value="' + CDec2(vRows[i].Absolute) + '">')
                                .text(CDec2(vRows[i].Absolute))
                            )
                            .append($('<td data-sort-value="' + CDec2(vRows[i].Percentage) + '">')
                                .text(vRows[i].Percentage + '%')
                            )
                            .append($('<td data-sort-value="' + CDec2(vRows[i].Value) + '">')
                                .text(AccountingFormat(vRows[i].Value))
                            )
                            .append($('<td>')
                                .text(vRows[i].CountedBy)
                            )
                            .append($('<td>')
                                .text(vRows[i].TurnedOverTo)
                            )
                            .append($('<td>')
                                .text(vRows[i].TurnOverDate)
                            )
                            .append($('<td>')
                                    .text(vRows[i].Remarks)
                                )
                        pLV.append(pItem)
                        //}
                        //else {
                        //    pItem = $('<tr>')
                        //                .append($('<td>')
                        //                    .text(AccountingFormat(vRows[i].ProductName))
                        //                )
                        //                .append($('<td>')
                        //                    .text(AccountingFormat(vRows[i + 1].ProductName))
                        //                )
                        //                .append($('<td>')
                        //                    .text(AccountingFormat(vRows[i + 2].ProductName))
                        //                )
                        //    pLV2.append(pItem)
                        //}
                    }
                }
                else {
                    $("#tblInventoryReportBody").append($('<tr>')
                                                    .append($('<td>')
                                                        .text('No Result(s) Found')
                                                        .css('text-align', 'center')
                                                        .attr('colspan', 16)
                                                    )
                )
                }


                //$('#tblInventoryReport').table("refresh");
                //$('#tblInventoryReportTotals').table("refresh");
            }

            function ViewInventoryLedgerOffline() {
                if (isInventoryLedgerLoaded == '') {
                    AjaxSend({
                        ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            sessionStorage["Inventory"] = "ledger";
                            isInventoryLedgerLoaded = vData;
                            $('#divForInventoryLedger').empty();
                            $('#divForInventoryLedger').append(vData).trigger('create');
                            isDoneClicked = true;
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    sessionStorage["Inventory"] = "ledger";
                    $('#divForInventoryLedger').empty();
                    $('#divForInventoryLedger').append(isInventoryLedgerLoaded).trigger('create');
                    InventoryLedger.CheckSource();
                }

                $('#divForInventoryLedger').pageshow();
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
                                    }
                                });

                                $('#divSecond').hide();
                                $('#divForInventoryLedger').show();
                            }
                        });
                    },
                    error: function (vError) {
                        MessageBox("This feature requires internet connection.",
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

            function GetProductLedger(ProductID) {
                data = {
                    ProductID: ProductID,
                    Within: $('#selReportRange option:selected').val(),
                    Fowst: $('#selFowst option:selected').val()
                }
                AjaxSend({
                    url: 'InventoryCount/GetInventoryCountReportByProduct',
                    data: data,
                    success: function (vData) {
                        DisplayInventoryCountReportByProduct(vData.InventoryCountReport.Rows)
                    }
                });
            }

            function DisplayInventoryCountReportByProduct(vRows) {
                pLV = $('#tblProductLedgerBody')

                pLV.html('');

                for (var i = 0; i <= vRows.length - 1; i++) {
                    pItem = $('<tr>')
                        .append($('<th>')
                            .append($('<span>')
                                .text(vRows[i].ProductNo + '-' + vRows[i].ProductName)
                            )
                        )
                        .append($('<td>')
                            .append($('<span>')
                                .text(vRows[i].Quantity)
                            )
                        )
                        .append($('<td>')
                            .append($('<span>')
                                .text(vRows[i].Balance)
                            )
                        )
                        .append($('<td>')
                            .append($('<span>')
                                .text(vRows[i].LastUpdated)
                            )
                        )
                        .append($('<td>')
                            .append($('<span>')
                                .text(vRows[i].Source)
                            )
                        )
                    pLV.append(pItem)
                }

                $('#tblProductLedger').table("refresh");
            }

            function ViewInventoryLedger() {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        PromptUserAccess('Group Settings', {
                            OnClickOKSuccess: function () {
                                if (location.hash == '#&ui-state=dialog')
                                    history.back();

                                if (isInventoryLedgerLoaded == '') {
                                    AjaxSend({
                                        ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            sessionStorage["Inventory"] = "ledger";
                                            isInventoryLedgerLoaded = vData;
                                            $('#divForInventoryLedger').empty();
                                            $('#divForInventoryLedger').append(vData).trigger('create');
                                            isDoneClicked = true;
                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });
                                }
                                else {
                                    sessionStorage["Inventory"] = "ledger";
                                    $('#divForInventoryLedger').empty();
                                    $('#divForInventoryLedger').append(isInventoryLedgerLoaded).trigger('create');
                                    InventoryLedger.CheckSource();
                                }

                                $('#divForInventoryLedger').pageshow();
                            }
                        });
                    },
                    error: function (vError) {
                        MessageBox("This feature requires internet connection.",
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

                $('#btnBCPhysicalInventory').click(function () {
                    $('#divForPhysicalInventory').pageshow();
                    $('.ItemsBreadCrumbs').text('');
                    if (OfflineMode) {
                        $('.ItemsBreadCrumbs').text('Inventories Accountability');
                    }
                    else {
                        breadcrumbs.prepend(arrowR, 'Physical Inventory And Variance Report');
                        $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                    }

                    BreadCrumbsClickEvent();
                });
            }

        });
    });

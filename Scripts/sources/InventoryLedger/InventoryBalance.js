require(['common', 'tablesort'
    , '/Scripts3/InventoryLedger.js?v=' + assetversion
    //,GetPathFormat('/Scripts/jquery.dataTables.min.js?crc=')
    //GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js')
], function () {

    var ProductID = '';
    var ProductName = '';
    var ProductNo = '';
    var UnitMeasureID = '';
    var FowstBinID = '';
    var mFilter = 0; //0 - all, 1 - product, 2 - raw materials
    var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
        arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
    var data;
    var isFinishedLoadIL = false;
    
    var balancesDatatable, dataUrl;
    window.InventoryBalance = {};

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

        if (proj.IsBootstrapDesign()) {
            $('#panSetBegBalancecontent').contents().appendTo('#panSetBegBalancecontent-bs');
            $('#panSelectInventoryBalancecontent').contents().appendTo('#panSelectInventoryBalancecontent-bs');
            $('#panInventoryCountApprovalcontent').contents().appendTo('#panInventoryCountApprovalcontent-bs');
            $('#panInventoryCountApprovalHistorycontent').contents().appendTo('#panInventoryCountApprovalHistorycontent-bs');
            $('#divFOWSTInventoryBalance').contents().appendTo('#divFOWSTInventoryBalance-bs');
            $('#divCompayWideInventoryBalance').contents().appendTo('#divCompayWideInventoryBalance-bs');
            $('#divCompanyWideInventoryBalancePriceLevel').contents().appendTo('#panCompanyWideInventoryBalancecontent-bs');
        }


        //end of stupid table

        //if we use $('#btnInventoryBalanceLedgerGO'), the page wont load the 2nd time you access it
        if (CStr2(sessionStorage.SelectedFOWST) == ''){
            DisplayFowst()
        }
        
        $('body').on('click', '#btnInventoryBalanceOptions', function () {

            ProductID = '';
            sessionStorage.IsFromInventoryBalance = false;

            PopupButtonList('', 'Choose Inventory Type', [
                {
                    Text: 'All Inventory Types', Click: function(){
                        $('#liViewAll').click()
                    }
                },{
                    Text: 'Finished Goods', Click: function(){
                        $('#liViewProducts').click()
                    }
                },{
                    Text: 'Intermediate Goods and Raw Materials', Click: function(){
                        $('#liViewRawMaterials').click()
                    }
                },{
                    Text: 'All Inventory Types Filtered By Category', Click: function(){
                        $('#liViewAllByCategory').click()
                    }
                },{
                    Text: 'By Vendors Product', Click: function(){
                        GetAllVendors();
                    }
                }
            ])
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
        .on('click', '#btnInventoryBalanceFowst', function (e) {
            e.preventDefault();
            DisplayFowst();
            return;
            Display.GetAllFowst();
            $('#panHeaderListOfGroupFowst').popup('open');
        })
        .on('click', '#tblBalances tbody tr td', function () {
            ProductID = balancesDatatable.row(this).data().ProductID;
            ProductName = balancesDatatable.row(this).data().ProductName;
            ProductNo = balancesDatatable.row(this).data().ProductNo;

            PopupButtonList('', 'Select Action', [
                {
                    Text: 'View Inventory Ledger', Click: function(){
                        sessionStorage.IsFromInventoryBalance = true;
                        sessionStorage.InventoryBalanceProductID = ProductID;
                        sessionStorage.InventoryBalanceProductName = ProductName;

                        if(location.pathname != '/erp'){                
                        } else {
                            $('#tabInventoryLedger').empty();
                        }
                        ViewInventoryLedger();
                    }
                },{
                    Text: 'Set Beginning Balance', Click: function(){
                        $('#txtBeginningBalance').val('');
                        if (proj.IsBootstrapDesign()) {
                            $('#panSetBegBalance-bs').modal('show');
                        } else {
                            $('#panSetBegBalance').popup('open');
                        }
                        
                        GetProductUnitMeasure();
                        $('#dtBegBalanceDateTime').val($.format.date(new Date, 'yyyy-MM-ddTHH:mm:ss'))
                    }
                },{
                    Text: 'Inventory Count', Click: function(){
                        $('#lblPopUpProductName').text(ProductName);
                        if (proj.IsBootstrapDesign()) {
                            $('#panSelectInventoryBalance-bs').modal('show');
                        } else {
                            $('#panSelectInventoryBalance').popup('open');
                        }

                        GetBinsBalances()
                        GetProductUnitMeasure();
                    }
                },{
                    Text: 'Inventory Count Approval/Disapproval', Click: function(){
                        $('#lblPopUpProductName2').text(ProductName);
                        if (proj.IsBootstrapDesign()) {
                            $('#panInventoryCountApproval-bs').modal('show');
                        } else {
                            $('#panInventoryCountApproval').show();
                        }
                        
                        GetInventoryCountForApproval();
                    }
                },{
                    Text: 'Inventory Count Approval/Disapproval History', Click: function(){
                        if (proj.IsBootstrapDesign()) {
                            $('#panInventoryCountApprovalHistory-bs').modal('show');
                        } else {
                            $('#panInventoryCountApprovalHistory').show();
                        }

                        $('#lblPopUpProductNameAppHistory').text(ProductName);
                        GetInventoryCountForApprovalHistory();
                    }
                },{
                    Text: 'Company Wide Inventory Balance', Click: function(){
                        $('#lblCompanyWideProduct').text(ProductName)
                        $('#lblProductNo').text(ProductNo);
                        $('#lblFOWSTName').text(sessionStorage.SelectedFOWSTName || sessionStorage.FowstName);

                        //$('#divCompayWideInventoryBalance').hide();
                        //$('#divFOWSTInventoryBalance').show();
            
                        $('#lblAvailableToCommit').text('');
                        $('#lblOnHand').text('');
                        $('#lblCommited').text('');

                        GetProductBalances(true);
                        GetAvailableToCommitProducts(true);
                        //GetProductPriceList(ProductID);

                        if (proj.IsBootstrapDesign()) {
                            $('#panCompanyWideInventoryBalance-bs').modal('show');
                        } else {
                            $('#panCompanyWideInventoryBalance').show();
                        }
                    }
                }
            ]);
        })
        .on('click', '#btnInventoryBalanceStatus', function () {
            PopupButtonList('', 'Filter Product Status', [
                {
                    Text: 'Active Products',
                    Click: function (e) {
                        $('.lblInventoryProductStatus').text('Active');
                        $('#btnInventoryBalanceStatus').attr('value', 0);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                },
                {
                    Text: 'Inactive Products',
                    Click: function (e) {
                        $('.lblInventoryProductStatus').text('Inactive');
                        $('#btnInventoryBalanceStatus').attr('value', 1);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                }
            ]);
        })
        .on('click', '#btnInventoryBalanceMovementYear', function () {
            PopupButtonList('', 'Filter Product by Last Posting Date', [
                {
                    Text: 'All SKU with Movement for the Last 1 Year',
                    Click: function (e) {
                        $('.lblInventoryProductMovementYear').text('All SKU with movement for the last 1 Year');
                        $('#btnInventoryBalanceMovementYear').attr('value', 1);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                },
                {
                    Text: 'All SKU with Movement for the Last 2 Years',
                    Click: function (e) {
                        $('.lblInventoryProductMovementYear').text('All SKU with movement for the last 2 Years');
                        $('#btnInventoryBalanceMovementYear').attr('value', 2);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                },
                {
                    Text: 'All SKU with Movement for the Last 3 Years',
                    Click: function (e) {
                        $('.lblInventoryProductMovementYear').text('All SKU with movement for the last 3 Years');
                        $('#btnInventoryBalanceMovementYear').attr('value', 3);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                },
                {
                    Text: 'All SKU with Movement',
                    Click: function (e) {
                        $('.lblInventoryProductMovementYear').text('All SKU with movement');
                        $('#btnInventoryBalanceMovementYear').attr('value', 4);
                        delete dataUrl.SearchString;
                        GetInventoryBalance(dataUrl);
                    }
                }
            ]);
        })
        .on('click', '#liViewAll', function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID
            }

            $('.lblSelectedInventoryType').text('All Inventory Types')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 0;
            delete data.SearchString;
            GetInventoryBalance(data);
        })
        .on('click', '#liViewProducts', function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID,
                WithComponentsOnly: 1
            }

            $('.lblSelectedInventoryType').text('Finished Goods')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 1;

            delete data.SearchString;
            GetInventoryBalance(data);
        })
        .on('click', '#liViewRawMaterials', function () {
            data = {
                FOWSTID: sessionStorage.SelectedFOWST,
                DisplayType: 'fowst',
                ProductID: ProductID,
                SubcomponentsOnly: 1
            }

            $('.lblSelectedInventoryType').text('Intermediate Goods and Raw Materials')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            mFilter = 2;

            delete data.SearchString;
            GetInventoryBalance(data);
        })
        .on('click', '#liViewAllByCategory', function () {
            $('.lblSelectedInventoryType').text('All Inventory Types Filtered By Category')
            $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

            $('#panInventoryBalanceOptions').popup('close');

            GetProductCategories();
        })
        .on('click', '.btnFOWSTInventoryBalance', function () {
            //$('#divCompayWideInventoryBalance').hide();
            //$('#divFOWSTInventoryBalance').show();

            $('#lblAvailableToCommit').text('');
            $('#lblOnHand').text('');
            $('#lblCommited').text('');

            //$('#lblProductNo').text(ProductNo);
            $('#lblFOWSTName').text(sessionStorage.SelectedFOWSTName || sessionStorage.FowstName);

            GetProductBalances(true);
            GetAvailableToCommitProducts(true);
            //GetProductPriceList(ProductID);
        })
        .on('click', '.btnCompanyWideInventoryBalance', function () {
            $('#lblCompanyWideProduct').text(ProductName)

            //$('#divCompayWideInventoryBalance').show();
            //$('#divFOWSTInventoryBalance').hide();

            GetProductBalances(false);            
            //GetProductPriceList(ProductID);
        })
        .on('click', '#btnInventoryCountApprovalHistoryClose', function () {
            $('#panInventoryCountApprovalHistory').hide();
        })
        .on('click', '#btnSetInventoryCount', function () {
            SaveInventoryCount();
        })
        .on('click', '.btnSetBeginningBalance', function () {
            SaveBeginningBalance();
        })
        .on('click', '#selUnitMeasureForInventoryCount', function () {
            UnitMeasureID = $('#selUnitMeasureForInventoryCount').val();
        })
        .on('click', '#btnInventoryBalanceClosePopUp', function () {
            ProductID = '';
            switch (mFilter) {
                case 0: $('#liViewAll').click(); break
                case 1: $('#liViewProducts').click(); break
                case 2: $('#liViewRawMaterials').click(); break
            }
        })
        .on('click', '#btnInventoryCountApprovalDone', function () {
            $('#panInventoryCountApproval').hide();
        })
        .on('click', '.btnSaveInventoryCountAndProductCap', function () {
            SaveMultipleBinInventoryCount();
        })
        .on('click', '#btnSearchMoreProducts', function () {
                SearchMoreProducts();
            })
        .on('click', '#butScanQr', function() {
            mobile.Scan({
                Success: function (vData) {
                    $('#txtInventoryBalSearch').val(vData.Value);
                    SearchMoreProducts();
                }
            });
        })

        function SearchMoreProducts() {
            BrowsePopup('LibraryPIMProduct', 'Search Product', [
                {
                    Search: $.trim($('#txtInventoryBalSearch').val()),
                    DisplayText: [{ Column: 'ProductName' }],
                    Click: function (e) {
                        ProductID = e.ID;
                        ProductName = e.ProductName;
                        ProductNo = e.ProductNo;
                        
                        $('#txtBeginningBalance').val('');
                        if (proj.IsBootstrapDesign()) {
                            $('#panSetBegBalance-bs').modal('show');
                        } else {
                            $('#panSetBegBalance').popup('open');
                        }
                        
                        GetProductUnitMeasure();
                        $('#dtBegBalanceDateTime').val($.format.date(new Date, 'yyyy-MM-ddTHH:mm:ss'))
                    }
                }
            ]);
        }
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

        $(function() {
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
            if(location.pathname == '/erp' && CStr2(sessionStorage.SelectedFOWST) == ''){
                DisplayFowst()
            }            
        }); // END Init
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
        ], {IsAll: true, FilterFowst: 1});
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
    InventoryBalance.GetInventoryCountForApprovalHistory = function(productid){
        ProductID = productid;
        GetInventoryCountForApprovalHistory()
    }

    InventoryBalance.GetCompanyWideInventoryBalance = function(productid, ProductName, ProductNo){
        ProductID = productid;
        ProductNo = ProductNo;
        $('#lblCompanyWideProduct').text(ProductName)
        $('#lblProductNo').text(ProductNo);
        $('#lblFOWSTName').text(sessionStorage.SelectedFOWSTName || sessionStorage.FowstName);
        GetProductBalances(true);
        GetAvailableToCommitProducts(true);
        //GetProductPriceList(ProductID);
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

                                delete data.SearchString;
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

    var htmlinventoryledger = '';
    function ViewInventoryLedger() {
        AjaxSend({
            url: 'Login3/TestCookies',
            success: function (vData) {

                PromptUserAccess('Inventory Ledger', {
                    OnClickOKSuccess: function () {
                        if (location.hash == '#&ui-state=dialog')
                            history.back();

                        function Go(vData, isloaded) {
                            if (!isloaded) {
                                htmlInventoryLedger = vData;
                                //sessionStorage["Inventory"] = "ledger";
                                $('#popupViewInventoryLedgercontent').empty();
                                $('#popupViewInventoryLedgercontent').append(vData);
                                App.Deferred.fulfill('FQ26E5TP76');

                                if (proj.IsBootstrapDesign() && !isloaded) {
                                    $('#popupViewInventoryLedgercontent').contents().appendTo('#popupViewInventoryLedgercontent-bs');
                                    $('#panSetBegBalanceInventoryLedger-bs').appendTo('#divDBInventoryBalance');
                                }
                            }

                            if (isloaded) {
                                $('#divDBInventoryLedger').contents().appendTo('#popupViewInventoryLedgercontent-bs');
                                $('#panSetBegBalanceInventoryLedger-bs').appendTo('#divDBInventoryBalance');                                
                            }

                            if (proj.IsBootstrapDesign()) {
                                $('#popupViewInventoryLedger-bs').modal('show').on('shown.bs.modal', function () {
                                    InventoryLedger.CheckSource();
                                });
                            } else {
                                $('#popupViewInventoryLedger').show();
                                InventoryLedger.CheckSource();
                            }
                        }

                        if (htmlinventoryledger == '') {
                            AjaxSend({
                                ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1&v=' + assetversion,
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    Go(vData, false);
                                    htmlinventoryledger = vData;
                                }
                            });
                            //CounterFranchiseRequest = false;

                        } else {
                            Go(htmlinventoryledger, true);
                        }
                        //$('#divSecond').hide();                        
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
    InventoryBalance.GetBinsBalances = function(productid){
        ProductID = productid;
        GetBinsBalances();
        GetProductUnitMeasure();
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
                            .data('data', vRows[i])
                            .click(function () {

                                $('#lblBin').text('Bin Name: ' + $(this).data('data').BinName);

                                FowstBinID = $(this).data('data').ID;

                                $('#panSelectInventoryBalance').popup('close');
                                $('#panSetInventoryBalance').popup('open');
                            })
                        )           
                        .append($('<td style="text-align:right">')
                            .text(CDec2(vRows[i].Balance))
                            .data('data', vRows[i])
                            .click(function () {

                                $('#lblBin').text('Bin Name: ' + $(this).data('data').BinName);

                                FowstBinID = $(this).data('data').ID;

                                $('#panSelectInventoryBalance').popup('close');
                                $('#panSetInventoryBalance').popup('open');
                            })
                        )
                        .append($('<td>')
                            .html('<input type="text" class="ObservedCount form-control" data-role="none" />')
                        )
                        .append($('<td>')
                            .html('<input type="text" class="MaxCap form-control" data-role="none" value=' + CDec2(vRows[i].MaxPerProduct) + ' />')
                        )
                        .append($('<td>')
                            .html('<input type="text" class="ReorderPoint form-control" data-role="none" value=' + CDec2(vRows[i].MinPerProduct) + ' />')
                        )
                        .append($('<td>')
                            .html('<input type="text" class="ReorderQty form-control" data-role="none" value=' + CDec2(vRows[i].ReorderQty) + ' />')
                        )
                        //.append($('<td>')
                        //    .html('<a href="#" data-role="button">Edit Balance</a>')
                        //)
            pLV.append(pItem);
        }

        PopupReposition();
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

                    //try {
                    //    pLV.selectmenu('refresh');
                    //    pLV2.selectmenu('refresh');
                    //}
                    //catch (e) {
                    //    pLV.selectmenu();
                    //    pLV2.selectmenu();
                    //}
                }
            }
        });
    }
    InventoryBalance.GetProductUnitMeasure = function(productid){
        ProductID = productid;
        GetProductUnitMeasure()
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
    InventoryBalance.GetInventoryCountForApproval = function(productid){
        ProductID = productid;
        GetInventoryCountForApproval()
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
                    WebToast('SKU count has been ' + (CBool(mdata.IsVarianceDisApproved) ? 'disapproved' : 'approved'));

                    if (proj.IsBootstrapDesign()){
                        $('#panInventoryCountApproval-bs').modal('hide');
                    } else {
                        $('#panInventoryCountApproval').hide();
                    }                    

                    data = {
                        FOWSTID: sessionStorage.SelectedFOWST,
                        DisplayType: 'fowst'
                        //,ProductID: ProductID
                    }

                    delete data.SearchString;
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

    function SaveMultipleBinInventoryCount() {
        var ObservedCount = 0;
        var MaxCap = 0;
        var ReorderPoint = 0;
        var ReorderQty = 0;
        var TotalCount = 0;

        $('#tblInventoryCountBody tr').each(function () {
            var Count = [];

            ObservedCount = CDec2($(this).find('.ObservedCount').val());
            MaxCap = CDec2($(this).find('.MaxCap').val());
            ReorderPoint = CDec2($(this).find('.ReorderPoint').val());
            ReorderQty = CDec2($(this).find('.ReorderQty').val());
            UnitMeasureID = $(this).data().data.UnitMeasureID;
            FowstBinID = $(this).data().data.ID;

            TotalCount += ObservedCount;

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
                Quantity: ObservedCount,
                ActualQuantity: 1,
                IsGoodStock: 1
            };

            if ($(this).find('.ObservedCount').val() != '') {
                AjaxSend({
                    url: 'InventoryCount/SaveInventoryCount',
                    data: data,
                    success: function (vData) {
                        $('#panSelectInventoryBalance-bs').modal('hide');
                        ProductID = '';
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
                });
            }

            SaveBinCaps(FowstBinID, MaxCap, ReorderPoint, ReorderQty);
        })

        $('#panSelectInventoryBalance').popup('close');

        data = {
            FOWSTID: sessionStorage.SelectedFOWST,
            DisplayType: 'fowst'
        }

        $('#tblBalances tbody tr').each(function(){
            if(balancesDatatable.row(this).data().ProductID == ProductID){
                balancesDatatable.row(this).data().Balance = AccountingFormat(TotalCount);
                return;
            }
        });

        balancesDatatable.clear().rows.add(InventoryProductRows).draw();
        //GetInventoryBalance(data);

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

                        delete data.SearchString;
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
            WebToast("Please enter beginning balance", "top");
            return
        }

        if (!IsDate($('#dtBegBalanceDateTime').val())) {
            WebToast("Please input proper Date and Time", "top");
            return;
        }

        var mData = {
            ProductID: ProductID,
            BegBalance: $('#txtBeginningBalance').val(),
            DateFrom: $('#dtBegBalanceDateTime').val(),
            UnitMeasureID: $('#selUnitMeasure').val(),
            FOWSTID: sessionStorage.SelectedFOWST
        }

        AjaxSend({
            url: 'InventoryLedger/SetBeginningBalance',
            data: mData,
            success: function (vData) {

                //reset product id to prevent filtering the table when requery is called
                ProductID = '';

                if (vData.Results == 'Success') {
                    WebToast('Beginning balance has been set', 'top');
                    switch (mFilter) {
                        case 0: $('#liViewAll').click(); break
                        case 1: $('#liViewProducts').click(); break
                        case 2: $('#liViewRawMaterials').click(); break
                    }
                }
                if (proj.IsBootstrapDesign()) {
                    $('#panSetBegBalance-bs').modal('hide');
                } else {
                    $('#panSetBegBalance').popup('close');
                }
            }
        });
    }

    var InventoryProductRows;
    var InventoryDataCount = 0;

    function GetInventoryBalance(data) {
        InventoryDataCount = 0;
        data.IsDeactivated = $('#btnInventoryBalanceStatus').attr('value');
        
        var YearCount = $('#btnInventoryBalanceMovementYear').attr('value');
        var MovementYear;

        if(CInt2(YearCount)==4){
            data.MovementYear = '';
        }else {
            MovementYear = new Date();
            MovementYear.setDate(MovementYear.getDate() - (365 * YearCount))

            //convert integer to date
            MovementYear = new Date(MovementYear);
            data.MovementYear = $.format.date(MovementYear, 'yyyy-MM-dd');
        }

        dataUrl = data;

        AjaxSend({
            url: 'InventoryLedger/GetReport',
            data: data,
            success: function (vData) {

                if (vData.RowCount != undefined) {
                    if (InventoryDataCount == 0) {
                        InventoryProductRows = vData.data.Rows;
                        DisplayBalances(InventoryProductRows);
                    }else{
                        InventoryProductRows = [...InventoryProductRows, ...vData.data.Rows];
                    }
                    
                    //InventoryDataCount * 1

                    if (CInt2(InventoryProductRows.length) == CInt2(vData.RowCount)) {
                        //DisplayBalances(InventoryProductRows);

                        //redraw datatable to get new data
                        //balancesDatatable.ajax.reload();
                        //$('#tblBalances').DataTable().data.reload();

                        //refresh data in datatable
                        $('#lblResultCount').text('Loaded ' + InventoryProductRows.length + ' out of ' + vData.RowCount + ' records ')

                        balancesDatatable.clear().rows.add(InventoryProductRows).draw();

                        InventoryProductRows = null;
                        InventoryDataCount = 0;
                        data.InventoryDataCount = InventoryDataCount;

                    } else {
                        balancesDatatable.clear().rows.add(InventoryProductRows).draw();

                        $('#lblResultCount').text('Loading ' + InventoryProductRows.length + ' out of ' + vData.RowCount + ' records ')

                        InventoryDataCount++;
                        data.InventoryDataCount = InventoryDataCount;
                        GetInventoryBalance(data)
                    }

                } else {

                    InventoryProductRows = vData.data.Rows;

                    $('#lblResultCount').text('Loaded ' + InventoryProductRows.length + ' out of ' + vData.data.Rows.length + ' records ')

                    DisplayBalances();
                }
               

            }
        });
    }

    function SaveBinCaps(BinID, MaxCap, ReorderPoint, ReorderQty) {
        var data = {
            FowstID: sessionStorage.SelectedFOWST,
            BinID: BinID,
            ProductID: ProductID,
            MaxCap: MaxCap,
            MinCap: ReorderPoint,
            ReorderQty: ReorderQty
        }

        AjaxSend({
            url: 'InventoryCount/UpdateFowstBinProductMaxMinCap',
            data: data,
            success: function (vData) {
                if (vData.Results != 'Success') {
                }
            }
        });
        
    }

    function DisplayBalances() {
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

        var buttons = [];
        if (!IsMobile) {
            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inventory Balance' })
        }

        if ($.fn.DataTable.isDataTable(BalancesTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            $('#tblBalances').empty();
            balancesDatatable.destroy();
        }

        balancesDatatable = $('#tblBalances').DataTable({
            "data": InventoryProductRows,
            "columns": [   
                //{
                //    data: function(data) {
                //        if(new Date(data.LastInventoryCountDate) - new Date()>30){
                //            return '<img class="ui-icon-flat-cross" />'
                //        }else{
                //            return '<img class="bs-checkbox" />'
                //        }
                //    }
                //},
                { "data": "ProductNo" },
                { "data": "ProductName" },
                { "data": "Brand" },
                //{
                //    data: function(data) {
                //        return !CBool(data.IsDeactivated);
                //    }
                //},
                { "data": "Balance" },
                { "data": "UnitMeasure" },
                {
                    data: function(data) {
                        return AccountingFormat(data.COGS);
                    }
                },{
                    data: function(data) {
                        return AccountingFormat(data.SRP);
                    }
                },
                { "data": "LastUpdated" },
                { "data": "InventoryCountApproval" },
                {
                    data: function(data) {
                        if(data.LastInventoryCountDate=='1/1/1900 12:00:00 AM'){
                            return '';
                        }else{
                            return data.LastInventoryCountDate;
                        }
                        
                    }
                },
                //{ "data": "LastInventoryCountBy" },
                //{
                //    data: function(vData) {
                //        return '<input type="text" ID="txt'+ vData.ProductID +'">';
                //    }
                //},
                //{
                //    data: function(vData) {
                //        return '<button ID="'+ vData.ProductID +'" data-role="none"> Submit Count </button> '
                //    }
                //}
            ],
            "columnDefs": [                
                {
                    'targets': [4,6,7,9],
                    "className": "dt-body-right"
                },
               
            //{
            //    'targets': [ 2 ],                
            //    'render': function (data, type, full, meta) {
            //        return CBool(data) ? '<span style="color:#F00">Inactive</span>' : '<span style="color:#4CAF50">Active</span>';
            //    }
            //},
            //{
            //    'targets': [3],
            //    'render': function (data, type, full, meta) {
            //        return AccountingFormat(data);
            //    }
            //},
            ////{
            ////    'targets': [4],
            ////    'render': function (data, type, full, meta) {
            ////        return AccountingFormat(data);
            ////    }
            ////},
            //{
            //    'targets': [6],
            //    'render': function (data, type, full, meta) {
            //        return AccountingFormat(data);
            //    }
            //},
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
            //sDom: '<"top">rt<"bottom"ip><"clear">',
            //buttons: [{ extend: 'excel', text: 'Download Excel File' }],
            dom: 'rtip',
            //buttons: buttons,
            fnPreDrawCallback: function (oSettings) { // call every click onto pagination
                //BalancesTable.table('refresh');
            }, initComplete: function(){
                if($('#txtInventoryBalSearch').val()!=''){
                    setTimeout(function(){
                        var pSearchString = $('#txtInventoryBalSearch').val();
                        balancesDatatable.search(pSearchString).draw();
                    },500);                    
                }

                $('#tblBalances').removeClass('dataTable');               
            }
        });



        $('#txtInventoryBalSearch').unbind('keydown').on('keydown', function(e) {
            var pSearchString = $(this).val();
            balancesDatatable.search(pSearchString).draw();

            //if(CInt2(balancesDatatable.rows( {search:'applied'} ).count())==0){
            //    $('#btnSearchMoreProducts').css('visibility','visible');
            //}else{
            //    $('#btnSearchMoreProducts').css('visibility','hidden');
            //}
        });

    }


    function GetProductBalances(IsByFOWST) {

        var isByFOWST = IsByFOWST;

        pData = {
            DisplayType: 'fowst',
            ProductID: ProductID
        }

        if (isByFOWST) { //if single fowst
            pData.FOWSTID = sessionStorage.SelectedFOWST;
            
            AjaxSend({
                url: 'InventoryLedger/GetReport',
                data: pData,
                success: function (vData) {
                    if (isByFOWST) {
                        var AvailableToCommit = CDec2($('#lblOnHand').text()) - CDec2($('#lblCommited').text());;
                        $('#lblOnHand').text(AccountingFormat(vData.data.Rows[0].Balance));

                        if ($('#lblCommited').text() != '') {
                            $('#lblAvailableToCommit').text(AccountingFormat((AvailableToCommit >= 0 ? AvailableToCommit : '0')));
                        }
                    } 
                }
            });
        } else {
            Bs.Loading('show');
            AjaxSend({
                url: 'Stock/GetInventoryBalance',
                data: { OrgID: sessionStorage.OrgID, StockID: ProductID },
                success: function (vData) {

                    var TotalAdjusted = 0;
                    var Total = 0;

                    $('#tblBodyCompayWideInventoryBalance').empty();

                    for (i = 0; i < vData.Results.Rows.length; i++) {

                        TotalAdjusted += (parseInt(vData.Results.Rows[i].Balance) > 0 ? parseInt(vData.Results.Rows[i].Balance) : 0);
                        Total += parseInt(vData.Results.Rows[i].Balance);

                        tData = $('<tr>')
                            .append($('<td>')
                                .text(vData.Results.Rows[i].FOWST)
                            )
                            .append($('<td id=lblBalance' + vData.Results.Rows[i].FowstID + '>')
                                .text(AccountingFormat(vData.Results.Rows[i].Balance))
                            )
                            .append($('<td id=lblCommited' + vData.Results.Rows[i].FowstID + '>')
                                .text(AccountingFormat('0'))
                            )
                            .append($('<td id=lblAvailableToCommit' + vData.Results.Rows[i].FowstID + '>')
                                .text(CInt2(vData.Results.Rows[i].Balance) > 0 ? AccountingFormat(vData.Results.Rows[i].Balance) : 'NONE')
                            )

                        $('#tblBodyCompayWideInventoryBalance').append(tData);
                    }

                    $('#tblBodyCompayWideInventoryBalance').append(tData);

                    tData = $('<tr style="font-weight:bold; font-size:large">')
                            .append($('<td>')
                                .text('Total :')
                            )
                            .append($('<td id=lblOnHandTotal>')
                                .text(AccountingFormat(TotalAdjusted))
                            )
                            .append($('<td id=lblCommitedTotal>')
                                .text(AccountingFormat('0'))
                            )
                            .append($('<td id=lblAvailableToCommitTotal>')
                                .text(AccountingFormat(Total))
                            )

                    $('#tblBodyCompayWideInventoryBalance').append(tData);

                    tData = $('<tr style="font-weight:bold; font-size:large">')
                            .append($('<td>')
                                .text('Adjusted Total (rows with  negative on-hand quantities are not included) :')
                            )
                            .append($('<td id=lblAdjustedOnHandTotal>')
                                .text(AccountingFormat(TotalAdjusted))
                            )
                            .append($('<td id=lblAdjustedCommitedTotal>')
                                .text(AccountingFormat('0'))
                            )
                            .append($('<td id=lblAdjustedAvailableToCommitTotal>')
                                .text(AccountingFormat('0'))
                            )

                    Bs.Loading('hide');
                }
            });

            GetAvailableToCommitProducts(false);

        }
    }

    function GetAvailableToCommitProducts(IsByFOWST) {

        var isByFOWST = IsByFOWST;

        pData = {
            ProductID: ProductID
        }

        if (isByFOWST) {
            pData.FOWSTID = sessionStorage.SelectedFOWST;
        }

        AjaxSend({
            url: 'SalesMenu/GetAvailableToCommitProducts',
            data: pData,
            success: function (vData) {
                if (isByFOWST) {
                    if (vData.Results.Rows.length > 0) {
                        $('#lblCommited').text(AccountingFormat(vData.Results.Rows[0].Quantity));
                    } else {
                        $('#lblCommited').text(AccountingFormat('0'));
                    }
                    
                    var AvailableToCommit = CDec2($('#lblOnHand').text()) - CDec2($('#lblCommited').text());

                    if ($('#lblOnHand').text() != '') {
                        $('#lblAvailableToCommit').text(AccountingFormat((AvailableToCommit >= 0 ? AvailableToCommit : '0')));
                    }

                } else {

                    var TotalAdjusted = 0;
                    var Total = 0;
                    var AvailableToCommitAdjusted;
                    var AvailableToCommit = 0;

                    for (i = 0; i < vData.Results.Rows.length; i++) {

                        TotalAdjusted += (parseInt(vData.Results.Rows[i].Quantity) > 0 ? parseInt(vData.Results.Rows[i].Quantity) : 0);
                        Total += parseInt(vData.Results.Rows[i].Quantity);

                        $('#lblCommited' + vData.Results.Rows[i].FOWSTID).text(AccountingFormat(vData.Results.Rows[i].Quantity))

                        AvailableToCommitAdjusted = CDec2($('#lblBalance' + vData.Results.Rows[i].FOWSTID).text()) -
                                    CDec2(vData.Results.Rows[i].Quantity)
                        
                        if (CDec2($('#lblBalance' + vData.Results.Rows[i].FOWSTID).text()) < 0 || AvailableToCommitAdjusted < 0) {
                            AvailableToCommitAdjusted = 'NONE';
                        }

                        $('#lblAvailableToCommit' + vData.Results.Rows[i].FOWSTID).text(
                                (AvailableToCommitAdjusted == 'NONE' ? AvailableToCommitAdjusted : AccountingFormat(AvailableToCommitAdjusted))
                            )
                    }

                    AvailableToCommitAdjusted = CDec2($('#lblAdjustedOnHandTotal').text()) - CDec2(TotalAdjusted);
                    AvailableToCommit = CDec2($('#lblOnHandTotal').text()) - CDec2(Total);


                    $('#lblAdjustedCommitedTotal').text(AccountingFormat(TotalAdjusted));
                    $('#lblAdjustedAvailableToCommitTotal').text(
                            AccountingFormat(
                                    (AvailableToCommitAdjusted)
                                )
                        );

                    $('#lblCommitedTotal').text(AccountingFormat(Total));
                    $('#lblAvailableToCommitTotal').text(
                            AccountingFormat(AvailableToCommit)
                        );
                }
            }
        });
    }

    function GetProductPriceList(strProductID) {
        var pData = [];

        pData = {
            ProductID: strProductID
        };

        AjaxSend({
            url: 'Organization/getOrganizationProductPriceList',
            data: pData,
            success: function (vData) {
                var count = 1;

                $('#tblBodyProductPrice').empty();

                if (vData.OrgProductPriceData.Rows.length == 0) {
                    $('#tblBodyProductPrice').append('<tr><td colspan="3" style="text-align:center;">No Result(s) Found</td></tr>')
                }
                else {
                    for (var i = 0; i < vData.OrgProductPriceData.Rows.length; i++) {
                        $('#tblBodyProductPrice').append($('<tr style="cursor:pointer;">')
                                                        .attr('ID', vData.OrgProductPriceData.Rows[i].ID)
                                                        .append($('<td>')
                                                            .text(count + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(proj.ChangePriceLevelDisplay(vData.OrgProductPriceData.Rows[i].PriceLevel))
                                                        )
                                                        .append($('<td>')
                                                            .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].Price))
                                                        )
                                                        .data('data', vData.OrgProductPriceData.Rows[i])
                                                        .click(function (e) {
                                                            
                                                        })
                        )
                        count += 1;
                    }
                }
                
            }
        });
    }

    function GetAllVendors(){
        
        $('.lblSelectedInventoryType').text('By Vendor')
        $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');

        $('#panInventoryBalanceOptions').popup('close');

        var pTarget;
        var pOptions;

        pTarget = 'Vendor';
        pOptions = { 
            FilterSupplier: 1
        };
       
        BrowsePopup('LibraryOrganization', pTarget,
            [
                {
                    // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (e) {
                        data = {
                            FOWSTID: sessionStorage.SelectedFOWST,
                            DisplayType: 'fowst',
                            ProductID: ProductID,
                            VendorID: e.ID
                        }

                        delete data.SearchString;
                        GetInventoryBalance(data);
                    }
                }
            ], pOptions
        ); // END BrowsePopup('LibraryFowst', 'Fowst',
    }
});
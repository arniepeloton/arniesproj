require(['common', , 'tablesort',
    //GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js')
    //'promise!/Scripts3/Browser/LibraryFowst.js?crc='
], function () {
    //require([
    //        '/Scripts/jquery.widget.min.js'
    //],
    //function () {
    //    require([
    //        '/Scripts/metro.min.js'
    //    ],
    //    function () {
    //        require([
    //            '/Scripts/metro-calendar.js',
    //            '/Scripts/metro-datepicker.js'
    //        ],
    //function () {
        var mData;
        var ProductID = '';
        var FOWSTID = '';
        var mDateFrom = new Date();
        var mDateTo = new Date();
        var dateFrom;
        var dateTo;
        var countBinRow = 0;
        var fowstTotal = 0;
        var begbal = [];
        var datetime = [];
        var binid = [];
        var counterSetDateTime = 0;

        $(function () {
            CheckSource();
            //InventoryLedger naa sa Bs_SalesDashboardIndex.js
            if (typeof InventoryLedger == 'undefined') {
                window.InventoryLedger = {};
            }

            InventoryLedger.CheckSource = CheckSource

            InventoryLedger.ClearData = ClearData

            InventoryLedger.HideSearch = function() {
                $('#divILSearch').hide();
            };

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

            //GetDateRange();
                                    
            $(".fromCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtDateFrom").text(data);
                }
            });

            $(".toCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtDateTo").text(data);
                }
            });

            $(".InventoryLedgerfromCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtInventoryLedgerDateFrom").text(data);
                }
            });

            $(".InventoryLedgertoCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtInventoryLedgerDateTo").text(data);
                }
            });

            $(".InventoryLedgerSetDateTimefromCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtInventoryLedgerSetDateTimeFrom").text(data);
                }
            });


            //$('#btnDone').click(function () {

            //    var newDateFrom = new Date($('.fromCalendar').calendar('getDates'));
            //    var newDateTo = new Date($('.toCalendar').calendar('getDates'));
                            
            //    if (!IsDate(newDateFrom)) {
            //        $('#txtDateFrom').text('Please select date from');
            //        return;
            //    } else if (!IsDate(newDateTo)) {
            //        $('#txtDateTo').text('Please select date to');
            //        return;
            //    } else {
            //        $('#panCustomDates').popup('close');
            //        mDateFrom = (newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
            //        mDateTo = (newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();
            //    }

            //    $('#lblPeriod').text(mDateFrom + ' - ' + mDateTo);

            //    if (ProductID != '' && FOWSTID != '') {
            //        DisplayInventoryLedger();
            //    }                            
            //});


            //if we use $('#btnInventoryLedgerGO'), the page wont load the 2nd time you access it
            if (!CBool(sessionStorage.IsFromInventoryBalance)) {
                if (CStr2(sessionStorage.SelectedFOWST) == '') {
                    if (IsGuid(sessionStorage.FowstID))
                        sessionStorage.SelectedFOWST = sessionStorage.FowstID;
                    else
                        DisplayFowst();
                }
            }
            
            $('body').on('click', '#btnInventoryLedgerGO', function (e) {
                GetInventoryLedger();
            })
            .on('click', '#btnInventoryLedgerDone', function (e) {
                dateFrom = new Date($('#txtInventoryLedgerDateFrom').text());
                dateTo = new Date($('#txtInventoryLedgerDateTo').text());
                var dateNow = new Date();

                if ((dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) || $('#txtInventoryLedgerDateFrom').text() == "" || $('#txtInventoryLedgerDateTo').text() == "") {
                    $('#spanILDateRangeError').show();
                }
                else {
                    $('#spanILDateRangeError').hide();
                    $('*[id="btnInventoryLedgerDateRange"]').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtInventoryLedgerDateFrom').text() + ' to ' + $('#txtInventoryLedgerDateTo').text())
                    $('*[id="ILDateRangeFrom"]').text($('#txtInventoryLedgerDateFrom').text());
                    $('*[id="ILDateRangeTo"]').text($('#txtInventoryLedgerDateTo').text());
                    $('*[id="ILDateRangeSeparator"]').show();
                    $('#panInventoryLedgerDate').popup('close');

                    if($('#txtILSearch').val() == "" && $('#btnInventoryLedgerFowst').text() != "FOWST Location"){
                        //$('#txtILSearch').val("This field is required").css('color', '#F00');
                        //setTimeout(function(){
                        //    $('#txtILSearch').val("").css('color', 'inherit');
                        //}, 2000)
                    }
                    else {
                        if(sessionStorage.Inventory == "ledger" && $('#txtILSearch').val() != "This field is required"){
                            DisplayInventoryLedger();
                        }
                    }
                }
            })
            .on('click', '#txtILSearch', function (e) {
                var option = [
                    {
                        StyleFormat: 'table',
                        DisplayText: [{ Column: 'ProductNo', Label: 'Product No.' }, { Column: 'ProductName', Label: 'Description' }],
                        //Search: $('#txtILSearch').val(),
                        Click: function (e) {
                            ProductID = e.ID;
                            $('[id=txtILSearch]').val(e.ProductName);
                            $('.lblSkuName').text(e.ProductName);
                            $('[id=txtProductDescription]').text(e.ProductName);

                            if ($('#btnInventoryLedgerFowst').text() != "FOWST Location" && $('#btnInventoryLedgerDateRange').text() != "Date Range") {
                                DisplayInventoryLedger();
                            }

                            if (sessionStorage.Inventory == 'balance') {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popupViewInventoryLedger-bs').modal('show');
                                } else {
                                    $('#popupViewInventoryLedger').show();
                                }
                            }
                        }
                    }
                ]

                if (OfflineMode) {
                    option[0].Offline = true;
                }

                BrowsePopup('LibraryPIMProduct', 'Select Product', option);
            })
            .on('click', '#btnInventoryLedgerFowst', function (e) {
                DisplayFowst();
                return;
                $('#panHeaderListOfGroupFowst').popup('open');
                Display.GetAllFowst();                
            })
            .on('click', '#btnInventoryLedgerDateRange', function (e) {
                DualCalendar({
                    Click: function (e) {
                        dateFrom = e.From;
                        dateTo = e.To;
                        $('*[id="btnInventoryLedgerDateRange"]').removeClass('btnRequired').addClass('btnNotRequired').attr('title', e.From + ' to ' + e.To).text(e.From + ' to ' + e.To);                        

                        if (IsGuid(ProductID))
                            DisplayInventoryLedger();
                        else if ($('#txtILSearch').val() == "" && $('#btnInventoryLedgerFowst').text() != "FOWST Location") {
                            //$('#txtILSearch').val("This field is required").css('color', '#F00');
                            //setTimeout(function () {
                            //    $('#txtILSearch').val("").css('color', 'inherit');
                            //}, 2000)
                        }
                        else {
                            if (sessionStorage.Inventory == "ledger" && $('#txtILSearch').val() != "This field is required") {
                                DisplayInventoryLedger();
                            }
                        }

                        if (sessionStorage.Inventory == 'balance') {
                            if (proj.IsBootstrapDesign()) {
                                $('#popupViewInventoryLedger-bs').modal('show');
                            } else {
                                $('#popupViewInventoryLedger').show();
                            }
                        }                        
                    }
                })
                return;
                $('#panInventoryLedgerDate').popup('open');
            })
            .on('click', '#btnInventoryLedgerCancel', function (e) {
                $('#panInventoryLedgerDate').popup('close');
            })
            .on('click', '.btnSetBeginningBalance', function (e) {
                //SetBeginningBalance($('#txtBeginningBalance').val(), $('#selBins').val());
                var e = 0;
                var isValid = true;

                for(var i = 0; i < countBinRow; i++){
                    if(countBinRow == 1){
                        if($('#setbal-' + i).val() == ""){
                            var balcount = i;
                            $('#setbal-' + i).css('border', '1px solid #F00');
                            setTimeout(function(){
                                $('#setbal-' + balcount).css('border', 'none');
                            }, 1000)
                            isValid = false;
                        }
                        else if($('#setdatetime-' + i).text() == "Set Date and Time") {
                            var dtcount = i;
                            $('#setdatetime-' + i).css('color', '#F00');
                            setTimeout(function(){
                                $('#setdatetime-' + dtcount).css('color', '#335F8E');
                            }, 1000)
                            isValid = false;
                        }
                        else {
                            binid.push({id: $('#binid-' + i).val()});
                            begbal.push({bal: $('#setbal-' + i).val()});
                            datetime.push({datetime: $('#setdatetime-' + i).text()});
                            isValid = true;;
                        }
                    }
                    else {
                        if($('#setbal-' + i).val() != ""){
                            e++;
                        }

                        if(e == 0){
                            $('#spanILError').show();
                            isValid = false;
                        }
                        else {
                            $('#spanILError').hide();
                            if($('#setbal-' + i).val() != ""){
                                if($('#setdatetime-' + i).text() == "Set Date and Time") {
                                    $('#setdatetime-' + i).css('color', '#F00');
                                    setTimeout(function(){
                                        $('#setdatetime-' + dtcount).css('color', '#335F8E');
                                    }, 1000)
                                    isValid = false;
                                }
                                else {
                                    binid.push({id: $('#binid-' + i).val()});
                                    begbal.push({bal: $('#setbal-' + i).val()});
                                    datetime.push({datetime: $('#setdatetime-' + i).text()});
                                    isValid = true;
                                }
                            }
                            else {
                                binid.push({id: $('#binid-' + i).val()});
                                begbal.push({bal: $('#setbal-' + i).val()});
                                datetime.push({datetime: $('#setdatetime-' + i).text()});
                                isValid = true;
                            }
                        }
                    }
                }

                if(isValid == true){
                    SetBeginningBalance(binid, begbal, datetime);
                }
                
            })
            .on('click', '#btnBeginningBalance', function (e) {
                SetBins();
                if (proj.IsBootstrapDesign()) {
                    $('#popupViewInventoryLedger-bs').modal('hide');
                    $('#panSetBegBalanceInventoryLedger-bs').modal('show');

                    if (sessionStorage.Inventory == 'balance') {
                        $('#panSetBegBalanceInventoryLedger-bs').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                            $('#popupViewInventoryLedger-bs').modal('show');
                            $('#panSetBegBalanceInventoryLedger-bs').unbind('hidden.bs.modal');
                        })
                    }                    
                } else {
                    $('#panSetBegBalanceInventoryLedger').popup('open');
                }
                $('.txtBegBalDateTime').val($.format.date(new Date, 'yyyy-MM-ddTHH:mm:ss'));
            })
            .on('change', 'input[name="rdoSetBegBalOption"]:checked', function (e) {
                if($(this).val() == "bin"){
                    $('.txtBal').removeAttr('disabled').textinput('enable');
                    $('.txtAll').attr('disabled', 'disabled').textinput('disable');
                }
                else {
                    $('.txtBal').attr('disabled', 'disabled').textinput('disable');
                    $('.txtAll').removeAttr('disabled').textinput('enable');
                }
            })
            
            $('#selHeaderListOfFowstGroup').on('click', '.btnLIListofFowstGroup', function (e) {
                e.preventDefault();
                FOWSTID = $(this).attr('id');                
                $('.lblSelectedFowstGroup').text($(this).text());
                $('#panHeaderListOfGroupFowst').popup('close');
                $('#btnInventoryLedgerFowst').removeClass('btnRequired');
                
                if($('#txtILSearch').val() == "" && $('#btnInventoryLedgerDateRange').text() == "Date to Range"){
                    $('#txtILSearch').val("This field is required").css('color', '#F00');
                    setTimeout(function(){
                        $('#txtILSearch').val("").css('color', 'inherit');
                    }, 2000)
                }
                else {
                    if(sessionStorage.Inventory == "ledger"){
                        DisplayInventoryLedger();
                    }
                }
            })

            //$('#btnInventoryLedgerGO').click(function () {
            //    GetInventoryLedger();
            //});

            $('#lblPeriod').click(function () {
                $('#panCustomDates').popup('open');
            });

            $('#txtSearchItem').keypress(function (e) {
                if (e.keyCode == 13) {
                    Browser.LibraryPIMProduct.Click = function (e) {
                        ProductID = e.ID;
                        $('.txtILSearch').val(e.ProductName)
                    }
                    Browser.LibraryPIMProduct.Show($('.txtILSearch').val());
                }
            });

            //$('#txtFOWST').keypress(function (e) {
            //    if (e.keyCode == 13) {
            //        Browser.LibraryFowst.AfterClick = function (e) {
            //            FOWSTID = e.ID;
            //            $('#txtFOWST').val(e.FowstName)
            //        }
            //        Browser.LibraryFowst.Show($('#txtFOWST').val());
            //    }
            //});
            
            $('[name="mFilter"]').click(function (e) {
                var vData = {};

                vData.Rows = [];

                var filter = '';

                switch (e.target.id){
                    case 'lnkGrossSales': 
                        filter = 'Invoice';
                        break;
                    case 'lnkStockReceiving': 
                        filter = 'Receiving'; 
                        break;
                    case 'lnkSalesReturns': 
                        filter = 'Sales Return';
                        break;
                    case 'lnkStockWidrawal':
                        filter = 'TransferOut';                        
                        break;
                    case 'lnkInventoryCountVariances': 
                        filter = 'Inventory Count'
                        break;
                }

                for (i = 0; i <= mData.Rows.length - 1; i++) {
                    if (mData.Rows[i].TransName.indexOf(filter) >= 0) {
                        vData.Rows.push({
                            BinName: mData.Rows[i].BinName,
                            TransName: mData.Rows[i].TransName,
                            Quantity: mData.Rows[i].Quantity,
                            Balance: mData.Rows[i].Balance,
                            LastUpdated: mData.Rows[i].LastUpdated,
                            Source: mData.Rows[i].Source,
                            UpdatedBy: mData.Rows[i].UpdatedBy,
                        })
                    }
                }

                if (vData.Rows.length > 0) {
                    var vRows = vData.Rows;
                    DisplayRows(vRows);
                }

            })

            
            $('#selFilter').change(function () {
                var vData = {};

                vData.Rows = [];
                $('#divTable').html('');

                if ($('#selFilter').val() != 'None') {
                    for (i = 0; i <= mData.Rows.length - 1; i++) {
                        if (mData.Rows[i].TransName.indexOf($('#selFilter').val()) >= 0) {
                            vData.Rows.push({
                                BinName: mData.Rows[i].BinName,
                                TransName: mData.Rows[i].TransName,
                                Quantity: mData.Rows[i].Quantity,
                                Balance: mData.Rows[i].Balance,
                                LastUpdated: mData.Rows[i].LastUpdated,
                                Source: mData.Rows[i].Source,
                            })
                        }
                    }

                    if (vData.Rows.length > 0) {

                        var pTable = CreateTable(vData, {
                            AddAllColumns: false,
                            Columns: {
                                BinName: {
                                    Text: 'Bin',
                                    Hidden: (pDisplayType == 'fowst')
                                },
                                TransName: {
                                    Text: 'Trans Name',
                                    Hidden: (pDisplayType == 'transname')
                                },
                                Quantity: {
                                    DataType: 'number',
                                    Format: '#,###',
                                    HAlign: 'right',
                                    Hidden: (pDisplayType == 'fowst')
                                },
                                Balance: {
                                    DataType: 'number',
                                    Format: '#,###',
                                    HAlign: 'right'
                                },
                                LastUpdated: {
                                    Text: 'Last Updated',
                                    DataType: 'date',
                                    Format: 'M/d/yyyy h:mm a',
                                    Hidden: (pDisplayType == 'fowst')
                                },
                                Source: {
                                    Hidden: (pDisplayType != 'ledger')
                                }
                            },
                            BeforeCell: function (e) {
                                switch (e.Key) {
                                    case 'StockDescription':
                                        e.Text = e.DataRow.ProductNo + ' - ' + e.DataRow.ProductName;
                                        return true;
                                        break;

                                    case 'Quantity':
                                        //if (CBool(e.DataRow.AllowDecimal))
                                        if (IsNumeric(e.DataRow.Quantity))
                                            e.Text = $.format.number(CDec2(e.DataRow.Quantity), proj.QuantityFormat);
                                        return true;
                                        break;

                                    case 'Balance':
                                        if (IsNumeric(e.DataRow.Balance))
                                            e.Text = $.format.number(CDec2(e.DataRow.Balance), proj.QuantityFormat);
                                        return true;
                                        break;

                                    default:
                                        return true;
                                }
                            },
                            BeforeRow: function (e) {
                                return true; // add this row to the table
                            }
                        });
                        $('#divTable').append(pTable);
                        pTable.table();
                        pTable.trigger('create');
                    }
                }
                else {

                    var pTable = CreateTable(mData, {
                        AddAllColumns: false,
                        Columns: {
                            BinName: {
                                Text: 'Bin',
                                Hidden: (pDisplayType == 'fowst')
                            },
                            TransName: {
                                Text: 'Trans Name',
                                Hidden: (pDisplayType == 'transname')
                            },
                            Quantity: {
                                DataType: 'number',
                                Format: '#,###',
                                HAlign: 'right',
                                Hidden: (pDisplayType == 'fowst')
                            },
                            Balance: {
                                DataType: 'number',
                                Format: '#,###',
                                HAlign: 'right'
                            },
                            LastUpdated: {
                                Text: 'Last Updated',
                                DataType: 'date',
                                Format: 'M/d/yyyy h:mm a',
                                Hidden: (pDisplayType == 'fowst')
                            },
                            Source: {
                                Hidden: (pDisplayType != 'ledger')
                            }
                        },
                        BeforeCell: function (e) {
                            switch (e.Key) {
                                case 'StockDescription':
                                    e.Text = e.DataRow.ProductNo + ' - ' + e.DataRow.ProductName;
                                    return true;
                                    break;

                                case 'Quantity':
                                    //if (CBool(e.DataRow.AllowDecimal))
                                    if (IsNumeric(e.DataRow.Quantity))
                                        e.Text = $.format.number(CDec2(e.DataRow.Quantity), proj.QuantityFormat);
                                    return true;
                                    break;

                                case 'Balance':
                                    if (IsNumeric(e.DataRow.Balance))
                                        e.Text = $.format.number(CDec2(e.DataRow.Balance), proj.QuantityFormat);
                                    return true;
                                    break;

                                default:
                                    return true;
                            }
                        },
                        BeforeRow: function (e) {
                            return true; // add this row to the table
                        }
                    });
                    $('#divTable').append(pTable);
                    pTable.table();
                    pTable.trigger('create');
                }
            });

            function ClearData() {
                dateFrom = '';
                dateTo = '';
                ProductID = '';
                $('.lblSelectedFowstGroup').text('FOWST Location');
                $('#btnInventoryLedgerDateRange').text('Date Range');
                $('#txtILSearch').val('');
                $('#divSecond, #btnBeginningBalance').hide();
                $('#divNoPageIL').pageshow();
            }

            function DisplayFowst() {
                BrowsePopup('LibraryFowst', 'Select FOWST', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                sessionStorage.SelectedFOWST = e.ID;
                                sessionStorage.SelectedFOWSTName = e.FowstName;
                                $('#btnInventoryLedgerFowst > .lblSelectedFowstGroup').text(e.FowstName);
                                $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                $('#btnInventoryLedgerFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                $('button[id="btnInventoryLedgerFowst"]').removeClass('btnRequired').addClass('btnNotRequired');

                                if (CStr2(dateFrom) != "" && CStr2(dateTo) != "" && ProductID != "") {
                                    DisplayInventoryLedger();
                                }

                                if (sessionStorage.Inventory == 'balance') {
                                    if (proj.IsBootstrapDesign()) {
                                        $('#popupViewInventoryLedger-bs').modal('show');
                                    } else {
                                        $('#popupViewInventoryLedger').show();
                                    }
                                }
                            }
                        }
                ], { IsAll: true, FilterFowst: 1 });
            }

            function SetBeginningBalance(BinID, Begbalance, DateTime) {
                
                for(var i=0; i < countBinRow; i++){
                    if(Begbalance[i] != undefined){
                        var data = {
                            ProductID: ProductID,
                            FOWSTID: FOWSTID,
                            BegBalance: Begbalance[i].bal,
                            BinID: binid[i].id,
                            DateFrom: DateTime[i].datetime
                        }

                        AjaxSend({
                            url: 'InventoryLedger/FirstTimeBeginningBalance',
                            data: data,
                            success: function (vData) {
        //                        if (vData.Results == 'Success') {
        //                            
        //                            
        //                        } else {
        //                            NewMessageBox('This product has assigned beginning balance. You can set beginning balance per FOWST-Product-Bin once.')
        //                        }
                                AjaxSend({
                                    url: 'InventoryLedger/SetBeginningBalance',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results == 'Success') {
                                            WebToast('You’ve successfully set the beginning balance for this SKU.', 'top');
                                            DisplayInventoryLedger();
                                            begbal = [];
                                            datetime = [];
                                            binid = [];

                                            if (proj.IsBootstrapDesign()) {
                                                $('#panSetBegBalanceInventoryLedger-bs').modal('hide');
                                            } else {
                                                $('#panSetBegBalanceInventoryLedger').popup('close');
                                            }
                                        } else {
                                            NewMessageBox('An error occured while saving beginning balance. Please contact Mobiz asap.')
                                        }

                                    }
                                });
                            }
                        });
                    }
                }                
            }

            function SetBins() {
                var prevBin = '';
                var pLV = $('#selBins');
                var count = 0;

                pLV.empty();
                $('.binrow').remove();

                $('#tBodySetBegBal').empty();

                for (i = 0; i <= mData.Rows.length - 1; i++) {
                    if (prevBin != mData.Rows[i].BinName) {
                        if (i == 0) {
                            pLV.text(mData.Rows[i].BinName);
                        } 
                        pLV.append(new Option(mData.Rows[i].BinName, mData.Rows[i].BinID, false));
                        prevBin = mData.Rows[i].BinName;

                        $('#tBodySetBegBal').append($('<tr>')
                                                    .addClass('binrow')
                                                    .append($('<td>')
                                                        .append($('<input type="number" class="form-control" data-role="none" />')
                                                                .attr('ID', 'setbal-' + count)
                                                                .addClass('txtBal')
                                                                .addClass('numBal')
                                                        )
                                                    )
                                                    .append($('<td>')
                                                        .text('<Unit Measure Here>')
                                                    )
//                                                                .append($('<td>')
//                                                                    .append($('<input type="datetime-local" />')
//                                                                            .attr('ID', 'datetime-' + count)
//                                                                            .attr('data-theme', 'd')
//                                                                            .addClass('ui-input-text')
//                                                                            .addClass('ui-body-d')
//                                                                            .addClass('txtBal')
//                                                                            .addClass('txtBegBalDateTime')
//                                                                            .trigger('create')
//                                                                    )
//                                                                )
                                                    .append($('<td>')
                                                        .append($('<a count="'+ count +'" id="setdatetime-'+ count +'">')
                                                            .text('Set Date and Time')
                                                            .addClass('link-primary')
                                                            .click(function(){
                                                                counterSetDateTime = $(this).attr('count');
                                                                if (!proj.IsBootstrapDesign()) {
                                                                    $('#panSetBegBalanceInventoryLedger').popup('close');
                                                                } else {
                                                                    $('#panSetBegBalanceInventoryLedger-bs').unbind('hidden.bs.modal');
                                                                }
                                                                SingleCalendar({
                                                                    ShowTime: true,
                                                                    Click: function (e) {
                                                                        $('#setdatetime-' + counterSetDateTime).css('color', '#335F8E');
                                                                        $('#setdatetime-' + counterSetDateTime).text(e);
                                                                    }, Close: function () {
                                                                        if (proj.IsBootstrapDesign()) {
                                                                            $('#panSetBegBalanceInventoryLedger-bs').modal('show');
                                                                            if (sessionStorage.Inventory == 'balance') {
                                                                                $('#panSetBegBalanceInventoryLedger-bs').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                                                                    $('#popupViewInventoryLedger-bs').modal('show');
                                                                                    $('#panSetBegBalanceInventoryLedger-bs').unbind('hidden.bs.modal');
                                                                                })
                                                                            }                                                                            
                                                                        } else {
                                                                            $('#panSetBegBalanceInventoryLedger').popup('open');
                                                                        }
                                                                    }
                                                                })
                                                            })
                                                        )
                                                    )
                                                    .append($('<td>')
                                                        .attr('align', 'center')
                                                        .text((mData.Rows[i].BinName == "Unnamed Bin" ? "All " : mData.Rows[i].BinName))
                                                        .append($('<input type="text" id="binid-'+ count +'" style="display:none;" value="' + mData.Rows[i].BinID +'"/>'))
                                                    )
                                                )
                        count = count + 1;
                        countBinRow = count;                         
                    }
                }

                try{
                    //pLV.selectmenu('refresh');
                    //$('.txtBal').textinput(); 
                    //$('select').selectmenu();
                }
                catch (e) {
                    //pLV.selectmenu();
                }
            }

            function GetInventoryLedger() {

                FOWSTID = sessionStorage.SelectedFOWST;

                var m_DateFrom = new Date();
                var m_DateTo = new Date();

                //$('.fromCalendar').calendar('unsetDate');
                //$('.toCalendar').calendar('unsetDate');
                
                switch (sessionStorage.Period) {
                    case "2DaysAgo":
                        //back date from
                        m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                        m_DateTo.setDate(m_DateTo.getDate() - 2);
                        break;
                        ;
                    case "Yesterday":
                        //back date from    
                        m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                        m_DateTo.setDate(m_DateTo.getDate() - 1);
                        break;

                    case "Today":
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear();
                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;

                    case "WeekToDate":
                        m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;

                    case "MonthToDate":
                        GroupDateBy = 'Month';
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;

                        var newDateFrom = new Date($('.fromCalendar').calendar('getDates'));
                        var newDateTo = new Date($('.toCalendar').calendar('getDates'));
                        GroupDateBy = 'Month';
                        m_DateFrom = (newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
                        m_DateTo = (newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();

                    default:
                        var newDateFrom = new Date($('.fromCalendar').calendar('getDates'));
                        var newDateTo = new Date($('.toCalendar').calendar('getDates'));
                        
                        m_DateFrom = (newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
                        m_DateTo = (newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();
                }
                
                mDateFrom = m_DateFrom;
                mDateTo = m_DateTo;

                if (ProductID == '') {
                    NewMessageBox('Please select a product', 'Error');
                    return;
                }

                if (FOWSTID == '' || FOWSTID == undefined) {
                    NewMessageBox('Please select FOWST', 'Error');
                    return;
                }

                if (!IsDate(mDateFrom)) {
                    NewMessageBox('Please put valid \'Date From\'', 'Error');
                    return;
                }

                if (!IsDate(mDateTo)) {
                    NewMessageBox('Please put valid \'Date To\'', 'Error');
                    return;
                }

                DisplayInventoryLedger();
            }

            function GetDateRange() {
                var mDate = new Date();

                mDateTo = mDate;
                mDateFrom.setDate(mDate.getDate() - 15);

                $('#lblPeriod').text($.format.date(mDateFrom, "MM/dd/yyyy") + ' - ' + $.format.date(mDateTo, "MM/dd/yyyy"));
            }

            function DisplayInventoryLedger() {

                //var pDisplayType = $('[name=optDisplayType]:checked').val();
                pDisplayType = 'product';
                //$('#divTable').html('');
                $('#LastBalance').text($.format.date(dateFrom || '', 'MM/dd/yyyy'));
                //$('#divSecond').show();
                
                if (OfflineMode) {
                    var vData = Business.TransInventoryLedger.GetReport(ProductID, sessionStorage.SelectedFOWST, dateFrom, dateTo);

                    if (vData.Rows.length > 0) {
                        //$('#dvDetailsIL').css('visibility', 'visible');
                        //$('#dvSummaryIL').css('visibility', 'hidden'); //hide muna kung offline kasi wala pang computation
                        $('#dvDetailsIL').css('display', 'block');
                        $('#dvSummaryIL').css('display', 'none');//hide muna kung offline kasi wala pang computation
                        $('#btnBeginningBalance').css('display', 'none'); //hide kasi bawal pa ang setting ng beg balance sa offline.
                        $('#divNoPageIL').css('display', 'none');

                        DisplayRows(vData.Rows);
                    } else {
                        $('#dvDetailsIL').css('display', 'none');
                        $('#dvSummaryIL').css('display', 'none');
                        $('#dvDetailsIL').css('visibility', 'hidden');
                        $('#dvSummaryIL').css('visibility', 'hidden');
                        $('#btnBeginningBalance').css('display', 'none');
                        $('#divNoPageIL').css('display', 'block');
                    }

                } else {
                    AjaxSend({
                        url: 'InventoryLedger/GetReport',
                        data: {
                            DisplayType: pDisplayType,
                            ProductID: ProductID,
                            FOWSTID: sessionStorage.SelectedFOWST,
                            DateFrom: dateFrom,
                            DateTo: dateTo
                        },
                        success: function (vData) {
                            var vRows = vData.Details.Rows;
                            mData = vData.Details;

                            if (vRows.length > 0) {
                                //$('#dvDetailsIL').css('visibility', 'visible');
                                //$('#dvSummaryIL').css('visibility', 'visible');
                                $('#dvDetailsIL').css('display', 'block');
                                $('#dvSummaryIL').css('display', 'block');
                                $('#btnBeginningBalance').css('display', 'block');
                                $('#divNoPageIL').css('display', 'none');

                                DisplayRows(vRows);
                            }
                            else {
                                $('#dvDetailsIL').css('display', 'none');
                                $('#dvSummaryIL').css('display', 'none');
                                //$('#dvDetailsIL').css('visibility', 'hidden');
                                //$('#dvSummaryIL').css('visibility', 'hidden');
                                $('#btnBeginningBalance').css('display', 'none');
                                $('#divNoPageIL').css('display', 'block');
                            }


                            //if (vData.Error != undefined)
                            //    NewMessageBox(vData.Error, 'Error');
                            //else {
                            //    mData = vData.Details;

                            //    var pTable = CreateTable(vData.Details, {
                            //        AddAllColumns: false,
                            //        Columns: {
                            //            BinName: {
                            //                Text: 'Bin',
                            //                Hidden: (pDisplayType == 'fowst')
                            //            },
                            //            TransName: {
                            //                Text: 'Trans Name',
                            //                Hidden: (pDisplayType == 'transname')
                            //            },
                            //            Quantity: {
                            //                DataType: 'number',
                            //                Format: '#,###',
                            //                HAlign: 'right',
                            //                Hidden: (pDisplayType == 'fowst')
                            //            },
                            //            Balance: {
                            //                DataType: 'number',
                            //                Format: '#,###',
                            //                HAlign: 'right'
                            //            },
                            //            LastUpdated: {
                            //                Text: 'Last Updated',
                            //                DataType: 'date',
                            //                Format: 'M/d/yyyy h:mm a',
                            //                Hidden: (pDisplayType == 'fowst')
                            //            },
                            //            Source: {
                            //                Hidden: (pDisplayType != 'ledger')
                            //            }
                            //        },
                            //        BeforeCell: function (e) {
                            //            switch (e.Key) {
                            //                case 'StockDescription':
                            //                    e.Text = e.DataRow.ProductNo + ' - ' + e.DataRow.ProductName;
                            //                    return true;
                            //                    break;

                            //                case 'Quantity':
                            //                    //if (CBool(e.DataRow.AllowDecimal))
                            //                    if (IsNumeric(e.DataRow.Quantity))
                            //                        e.Text = $.format.number(CDec2(e.DataRow.Quantity), proj.QuantityFormat);
                            //                    return true;
                            //                    break;

                            //                case 'Balance':
                            //                    if (IsNumeric(e.DataRow.Balance))
                            //                        e.Text = $.format.number(CDec2(e.DataRow.Balance), proj.QuantityFormat);
                            //                    return true;
                            //                    break;

                            //                default:
                            //                    return true;
                            //            }
                            //        },
                            //        BeforeRow: function (e) {
                            //            return true; // add this row to the table


                            vRows = vData.Summary.Rows;

                            var mGrossSales = 0;
                            var mReturns = 0;
                            var pSubtransactionType;

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                if (vRows[i].Transaction == 'Sales Invoice') {
                                    pSubtransactionType = CStr2(vRows[i].SubtransactionType);
                                    if (pSubtransactionType == 'Usage') {
                                        $('#spUsage').text(CDec2(vRows[i].Total));
                                    } else if (pSubtransactionType == 'Production') {
                                        $('#spProduction').text(CDec2(vRows[i].Total));
                                    } else {
                                        $('#spGrossSales').text(CDec2(vRows[i].Total));
                                        mGrossSales = CDec2(vRows[i].Total);
                                    }
                                }
                                else if (vRows[i].Transaction == 'SR') {
                                    $('#spStockReceiving').text(CDec2(vRows[i].Total));
                                }
                                else if (vRows[i].Transaction == 'Sales Return') {
                                    $('#spSalesReturns').text(CDec2(vRows[i].Total));
                                    mReturns = CDec2(vRows[i].Total);
                                }
                                else if (vRows[i].Transaction == 'Inventory Count Positive') {
                                    $('#spInventoryCountVariancesPositive').text(CDec2(vRows[i].Total));
                                }
                                else if (vRows[i].Transaction == 'Inventory Count Negative') {
                                    $('#spInventoryCountVariancesNegative').text(CDec2(vRows[i].Total));
                                }
                                else if (vRows[i].Transaction == 'Transfer Out') {
                                    $('#spStockTransfers').text(CDec2(vRows[i].Total));
                                }

                                //pItem = $('<tr>')
                                //            .append($('<td style="margin-top:3px">')
                                //                .text('Total (Qty) ' + vRows[i].Transaction)
                                //            )
                                //            .append($('<td>')
                                //                .text(CDec2(vRows[i].Total))
                                //            )
                                //            .append($('<br />')
                                //            )
                                //pLV.append(pItem);
                            }

                            $('#spNetSales').text(mGrossSales - mReturns);
                        }
                    });
                }
                //    $('#divTable').append(pTable);
                //    pTable.table();
                //    pTable.trigger('create');
                //};

                //        var pLV = $('#tblSummary');

                //        pLV.html('');

            //}
                //}); // AjaxSend

                //$('#dvDetails').css('visibility', 'visible');
                //$('#dvDetails').css('display', 'block');
            }; // Display

            function DisplayRows(vRows) {
                var pLV = $('#tblBodyIL');
                var BegBalance = 0;
                var EndingBalance = 0;
                var prevBin = '';
                

                pLV.empty();

                
                var BegBalanceBins = [];
                var EndBalanceBins = [];

                var AllBinsEndingBalance = {};
                var AllBinsBeginningBalance = {};

                EndBalanceBins = [];
                var IsExist = false;

                for (var i = 0; i <= vRows.length - 1; i++) {
                    //beg balance
                    //if (i == 0) {
                    //    BegBalanceBins[BegBalanceBins.length] = vRows[i].BinName;
                    //    BegBalance = vRows[i].Balance;                        
                    //}
                    //else {
                    IsExist = false;

                    for (j = 0; j < BegBalanceBins.length; j++) {
                        if (BegBalanceBins[j] == vRows[i].BinName) {
                            IsExist = true;
                            break;
                        }
                    }

                    if (!IsExist) {
                        BegBalanceBins[BegBalanceBins.length] = vRows[i].BinName;

                        if(vRows[i].TransName.indexOf('Sales Invoice')>=0){
                            BegBalance = BegBalance + CDec2(vRows[i].Balance) - CDec2(vRows[i].Quantity);
                        } else if (vRows[i].TransName.indexOf('Sales Return') >= 0) {
                            BegBalance = BegBalance + CDec2(vRows[i].Balance) - CDec2(vRows[i].Quantity);
                        } else if (vRows[i].TransName.indexOf('Receiving') >= 0) {
                            BegBalance = BegBalance + CDec2(vRows[i].Balance) - CDec2(vRows[i].Quantity);
                        } else if (vRows[i].TransName.indexOf('Transfer Out') >= 0) {
                            BegBalance = BegBalance + CDec2(vRows[i].Balance) - CDec2(vRows[i].Quantity);
                        } else if (vRows[i].TransName.indexOf('Inventory Count') >= 0) {
                            BegBalance = BegBalance + CDec2(vRows[i].Balance) - CDec2(vRows[i].Quantity);
                        }
                    }
                    //}

                    //ending balance
                    IsExist = false;

                    for (k = 0; k < EndBalanceBins.length; k++) {
                        if (EndBalanceBins[k][0] == vRows[i].BinName) {
                            EndBalanceBins[k][1] = vRows[i].Balance
                            IsExist = true;
                        }
                    }

                    if (!IsExist) {
                        var l = EndBalanceBins.length;
                        EndBalanceBins[l] = [];
                        EndBalanceBins[l][0] = vRows[i].BinName;
                        EndBalanceBins[l][1] = vRows[i].Balance;
                    }

                    //var key = vRows[i].BinName;

                    //AllBins[key] = vRows[i].Balance;

                    //if (!AllBinsBeginningBalance.hasOwnProperty(vRows[i].BinName)) {
                    //    AllBins[key] = vRows[i].Balance;
                    //}

                    //if (vRows[i].BinName != prevBin) {

                    //    BegBalance = BegBalance + CDec2(vRows[i].Balance);
                        
                    //    if (i != 0) {
                    //        EndingBalance = EndingBalance + CDec2(vRows[i - 1].Balance);
                    //    }                       

                    //    prevBin = vRows[i].BinName;
                    //}

                    pItem = $('<tr>')
                            .attr('ID', vRows[i].ID)
                            .append($('<td>')
                                .text(vRows[i].BinName)
                            )
                            .append($('<td>')
                                .text(vRows[i].TransName)
                            )
                            .append($('<td>')
                                .text(vRows[i].CustomerOrVendorName)
                            )
                            .append($('<td>')
                                .text(vRows[i].SubtransactionType)
                            )
                            .append($('<td>')
                                .text($.format.number(CDec2(vRows[i].Quantity),'#,###.########'))
                            )
                            .append($('<td>')
                                .text(AccountingFormat(vRows[i].Price))
                            )
                            .append($('<td>')
                                .text($.format.number(CDec2(vRows[i].Balance),'#,###.########'))
                            )
                            .append($('<td>')
                                .text(vRows[i].TransactionDate)
                            )
                            .append($('<td>')
                                .text(vRows[i].UpdatedBy)
                            )

                    pLV.append(pItem);
                }

                //for last bin's ending balance
                //EndingBalance = EndingBalance + CDec2(vRows[vRows.length - 1].Balance);

                //compute ending balance
                for (p = 0; p < EndBalanceBins.length; p++) {
                    EndingBalance += CDec2(EndBalanceBins[p][1]);
                }

                $('#spBeginningBalance').text(BegBalance);
                $('#spEndingBalance').text(EndingBalance);
            }

            function CheckSource() {
                if (CBool(sessionStorage.IsFromInventoryBalance)) {
                    var todayDate = new Date();
                    var priorDate = new Date(todayDate.getFullYear(), todayDate.getMonth() - 2, todayDate.getDate());
                    ProductID = sessionStorage.InventoryBalanceProductID;
                    FOWSTID = sessionStorage.SelectedFOWST;
                    if (FOWSTID == null) {
                        FOWSTID = sessionStorage.FowstID;
                        sessionStorage.SelectedFOWST = sessionStorage.FowstID;
                    }
                    $('#txtILSearch').val(sessionStorage.InventoryBalanceProductName);
                    $('#btnInventoryLedgerFowst').removeClass('btnRequired').addClass('btnNotRequired');
                    $('.lblSelectedFowstGroup').text(sessionStorage.SelectedFOWSTName);
                    $('#btnInventoryLedgerDateRange').removeClass('btnRequired').addClass('btnNotRequired').text($.format.date(new Date(priorDate), 'MMM dd, yyyy') + ' to ' + $.format.date(new Date(), 'MMM dd, yyyy'));
                    dateFrom = $.format.date(new Date(priorDate), 'MMM dd, yyyy');
                    dateTo = $.format.date(new Date(), 'MMM dd, yyyy');

                    if ((FOWSTID != undefined) 
                        && (IsDate(dateFrom))
                        && (IsDate(dateTo))) {

                        DisplayInventoryLedger();
                    }

                    if (FOWSTID == undefined) {
                        //DisplayFowst();
                    }
                    
                }

                if (proj.IsBootstrapDesign()) {
                    $('#divSetBagContent').contents().appendTo('#panSetBegBalanceInventoryLedgercontent-bs');
                } else {
                    $('#divSetBagContent').contents().appendTo('#panSetBegBalanceInventoryLedgercontent');
                }
            }

        }); // $(function() {

        
    });
//});
//    });
//});
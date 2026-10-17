require(['common', 'tablesort'
    //,GetPathFormat('/Scripts/jquery.dataTables.min.js?crc=')
    //GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js')
], function () {

    var mFilter = 0; //0 - all, 1 - product, 2 - raw materials
    var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
        arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
    
    
    var inventorylevelDatatable;

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


        $('body').on('click', '#btnInventoryLevelFowst', function (e) {
            e.preventDefault();
            DisplayFowst();
        })
    });
    
    function DisplayFowst() {
        BrowsePopup('LibraryFowst', 'Select FOWST', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'FowstName' }],
                    Click: function (e) {
                        sessionStorage.SelectedFOWST = e.ID;
                        sessionStorage.SelectedFOWSTName = e.FowstName;
                        $('#btnInventoryLevelFowst > .lblSelectedFowstGroup').text(e.FowstName);
                        //$('#btnInventoryLevelFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        //$('#btnInventoryLedgerFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        //$('button[id="btnInventoryLedgerFowst"]').removeClass('btnRequired').addClass('btnNotRequired');
                        GetAllFowstBinProduct();
                    }
                }
        ], { IsAll: true, FilterFowst: 1 });
    }

    function GetAllFowstBinProduct() {
        data = {
            FowstID: sessionStorage.SelectedFOWST
        }

        AjaxSend({
            url: 'InventoryCount/GetAllFowstBinProduct',
            data: data,
            success: function (vData) {
                DisplayInventoryLevels(vData.Results.Rows);
            }
        });
    }

    function DisplayInventoryLevels(vRows) {
        var InventoryLevelTable = $('#tblInventoryLevel');
        if ($.fn.DataTable.isDataTable(InventoryLevelTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            InventoryLevelTable.empty();
            inventorylevelDatatable.destroy();
        }

        inventorylevelDatatable = InventoryLevelTable.DataTable({
            "data": vRows,
            "columns": [                
                { "data": "ProductName" },
                { "data": "BinName" },
                { "data": "UnitMeasure" },
                { "data": "Balance" },
                { "data": "MaxPerProduct" },
                { "data": "MinPerProduct" },
                { "data": "ReorderQty" },
                { "data": "MaxMinDiff" }
            ],
            "columnDefs": [
            {
                'targets': [3],
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
            {
                'targets': [6],
                'render': function (data, type, full, meta) {
                    return AccountingFormat(data);
                }
            },
            {
                'targets': [7],
                'render': function (data, type, full, meta) {
                    return (CDec2(data) <= 0 ? 'Insufficient' : 'Sufficient');
                }
            },
            ],            
            "lengthMenu": [[25, 50, -1], [25, 50, "All"]],
            "language": {
                "search": "Search Product "
            },
            sDom: '<"top"Bf>rt<"bottom"ip><"clear">',
            buttons: [{ extend: 'csvHtml5', text: 'Download as CSV File' }],
            initComplete: function () {
                InventoryLevelTable.removeClass('dataTable');
            }
        });
    }
});

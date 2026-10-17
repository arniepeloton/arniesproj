$(function () {

    setLevel0("Employees", "../welcome", 1);
    setLevel1("Drp", "../StockSched/", 1);
    setLevel2("Planned Orders", "../StockSched/", 1);

    var mDiv = $('#divStockSched');
    var divStockNunber = mDiv.find('[name=divStockNunber]');
    var divStocks = mDiv.find('[name=divStocks]');
    var mGridStocks;
    var divStockDescription = mDiv.find('[name=divStockDescription]');
    var spanStockOnHand = mDiv.find('[name=spanStockOnHand]');
    var spanDateLastApprovedCount = mDiv.find('[name=spanDateLastApprovedCount]');
    var spanSafetyStock = mDiv.find('[name=spanSafetyStock]');

    var txtWeeksCount = mDiv.find('[name=txtWeeksCount]');
    var butWeeksCount = mDiv.find('[name=butWeeksCount]')
        .click(butWeeksCount_Click);
    var divSupplierReason = mDiv.find('[name=divSupplierReason]')
        .hide();

    // tableSupplier
    var tableSupplier = mDiv.find('[name=tableSupplier]');
    var trSupplier = tableSupplier.find('[name=trSupplier]');
    var trPrice = tableSupplier.find('[name=trPrice]');
    var trLeadTime = tableSupplier.find('[name=trLeadTime]');
    var trMoq = tableSupplier.find('[name=trMoq]');
    var trMoqUnit = tableSupplier.find('[name=trMoqUnit]');
    var trMoqTotal = tableSupplier.find('[name=trMoqTotal]');
    var trLotSize = tableSupplier.find('[name=trLotSize]');
    var trLotSizeUnit = tableSupplier.find('[name=trLotSizeUnit]');
    var trLotSizeTotal = tableSupplier.find('[name=trLotSizeTotal]');

    // table and rows
    var tableSched = mDiv.find('[name=tableSched]');
    var tdAdvanceOrders = tableSched
        .find('[name=tdAdvanceOrders]')
        .css('text-align', 'right');
    var tdBackOrders = tableSched
        .find('[name=tdBackOrders]')
        .css('text-align', 'right');
    var tdForecast = tableSched
        .find('[name=tdForecast]')
        .css('text-align', 'right');
    var tdGrossRequirements = tableSched
        .find('[name=tdGrossRequirements]')
        .css('text-align', 'right');
    var tdScheduledReceipts = tableSched
        .find('[name=tdScheduledReceipts]')
        .css('text-align', 'right');
    var tdProjectedAvaialbletoCommitQty = tableSched
        .find('[name=tdProjectedAvaialbletoCommitQty]')
        .css('text-align', 'right');

    var mStockID;
    var mBestSupplier;
    var mWeeksCount;
    var mData; // data (w/o d) from Ajax
    var mSafetyStock; // datatype: single

    (function () { // Init

        mDiv.find('div[name=divDetails],div[name=divTables]').Poper({ AutoPop: true });
        mWeeksCount = txtWeeksCount.val();
        InitGrid(mWeeksCount);
        GetGrid();

    })();  // END Init

    function GetGrid() {

        mDiv.block();
        AjaxSend({
            url: '/Drp/StockSchedData',
            data: {
                WeeksCount: mWeeksCount
            },
            success: function (vData) {
                mDiv.unblock();
                if (!HandleSessionExpired(vData)) return;
                LoadGrid(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                mDiv.unblock();
                ShowMessage(XMLHttpRequest.responseText);
            }
        });

    }; // END GetGrid

    function LoadGrid(vData) {

        vData.Data.Columns = {
            StockNumber: {
                Label: 'Stock number'
            },
            StockDescription: {
                Label: 'Stock description'
            }
        };
        for (var p = 0; p <= vData.Dates.Rows.length - 1; p++) {
            vData.Data.Columns[vData.Dates.Rows[p].NowDateRange] = {
                Label: 'Week #' + (p + 1) + ': ' + vData.Dates.Rows[p].NowDateRange,
                Format: '#,##0',
                MajorDataType: 'Numeric',
                Width: 50,
                Alignment: 'Right'
            };
        };
        mGridStocks = divStocks.ProWebGrid(vData.Data)
            .AfterRowActivate(mGridStocks_AfterRowActivate);

    }; // END LoadGrid

    function mGridStocks_AfterRowActivate(e) {

        var pDr;
        var pText;

        pDr = mGridStocks.RowData(e.SelectedRow);
        mStockID = pDr.StockID;
        divStockNunber.text(pDr.StockNumber);
        divStockDescription.text(pDr.StockDescription);
        GetDetails();

    }; // Grid_AfterRowActivate

    function GetDetails() {

        mDiv.block();
        AjaxSend({
            url: '/Drp/StockSchedDetails',
            data: {
                StockId: mStockID,
                WeeksCount: mWeeksCount
            },
            success: function (vData) {
                mDiv.unblock();
                if (!HandleSessionExpired(vData)) return;
                LoadDetails(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                mDiv.unblock();
                ShowMessage(XMLHttpRequest.responseText);
            }
        });

    }; // END GetDetails

    function LoadDetails(vData) {

        mData = vData;
        mSafetyStock = CDec2(mData.SafetyStock);
        tdProjectedAvaialbletoCommitQty.text($.format.number(CDec2(vData.StockOnHand), '#,##0'));
        tdBackOrders.next().children().text(vData.BackOrders);
        spanDateLastApprovedCount.text(mData.DateLastApprovedCount + '');
        spanSafetyStock.text(parseInt(mSafetyStock * mWeeksCount));
        spanStockOnHand.text(vData.StockOnHand);
        mBestSupplier = vData.BestSupplier;
        LoadSupplierGrid(mBestSupplier);
        InitGrid(mWeeksCount);
        UpdateGrid('tdAdvanceOrders', 0);

    }; // END LoadDetails

    function butWeeksCount_Click() {

        if (IsNumeric(txtWeeksCount.val()) == false) {
            ShowMessage('Please enter a number from 1 to 30');
            txtWeeksCount.focus();
            return;
        };
        mWeeksCount = parseInt(txtWeeksCount.val());
        if (mWeeksCount <= 0 || mWeeksCount > 30) {
            ShowMessage('Please enter a number from 1 to 30');
            txtWeeksCount.focus();
            return;
        };

        InitGrid(mWeeksCount);

    }; // END butWeeksCount_Click

    function LoadSupplierGrid(vData) {

        var pTd;

        tableSupplier.find('.SupplierCell').remove();
        for (var p = 0; p <= vData.Rows.length - 1; p++) {
            tableSupplier.children('tbody').children('tr').each(function (vIndex, vItem) {
                pTd = $('<td>')
                    .addClass('SupplierCell')
                    .appendTo($(vItem));
                switch ($(vItem).attr('name')) {
                    case 'trSupplier':
                        pTd.append($('<div>').text(vData.Rows[p].SupplierName))
                            .append($('<button>')
                                .click(function () {
                                    var pIndex = $(this).closest('td.SupplierCell').index(); // base-1
                                    PromptReason(pIndex);
                                })
                                .text('Choose this supplier')
                            );
                        break;

                    case 'trPrice':
                        pTd.text($.format.number(CDec2(vData.Rows[p].SupplierPrice), '#,##0') + ' / ' + vData.Rows[p].SupplierUnitMeasure2);
                        break;

                    case 'trLeadTime':
                        if (IsNumeric(vData.Rows[p].LeadTime))
                            pTd.text($.format.number(CDec2(vData.Rows[p].LeadTime), '#,##0') + ' day[s]');
                        else
                            pTd.text('');
                        break;

                    case 'trMoq':
                        pTd.text($.format.number(CDec2(vData.Rows[p].MinimumOrderQty), '#,##0'));
                        break;

                    case 'trMoqUnit':
                        pTd.text(vData.Rows[p].MinimumOrderUnit + ' [' + vData.Rows[p].MinimumOrderUnitQty + ']');
                        break;

                    case 'trMoqTotal':
                        pTd.text($.format.number(CDec2(vData.Rows[p].MinimumOrderQty) * CDec2(vData.Rows[p].MinimumOrderUnitQty), '#,##0'));
                        break;

                    case 'trLotSize':
                        pTd.text($.format.number(CDec2(vData.Rows[p].LotSize), '#,##0'));
                        break;

                    case 'trLotSizeUnit':
                        pTd.text(vData.Rows[p].LotSizeUnit + ' [' + vData.Rows[p].LotSizeUnitQty + ']');
                        break;

                    case 'trLotSizeTotal':
                        pTd.text($.format.number(CDec2(vData.Rows[p].LotSize) * CDec2(vData.Rows[p].LotSizeUnitQty), '#,##0'));
                        break;
                };
            });
        };
        SelectSupplier(1);

    }; // END LoadSupplierGrid

    function SelectSupplier(vIndex) {
        // PARAMS:
        //  vIndex: 1-based

        tableSupplier.find('td.SupplierCell').removeClass('SelectedSupplier');
        tableSupplier.children('tbody').children('tr').find('td:eq(' + vIndex + ')').addClass('SelectedSupplier');

    }; // END SelectSupplier

    function PromptReason(vIndex) {

        divSupplierReason.dialog({
            title: 'Under construction',
            modal: true,
            buttons: {
                'Ok': function () {
                    SelectSupplier(vIndex);
                    $(this).dialog('close');
                },
                'Close': function () {
                    $(this).dialog('close');
                }
            }
        });

    }; // END PromptReason

    function InitGrid(vWeeksCount) {

        var pTrForecast;

        spanSafetyStock.text(parseInt(mSafetyStock * mWeeksCount));
        tableSched.find('.WeekColumn').remove();
        tableSched.children('tbody').children('tr[name^="trHistoricalDemand_"]').remove(); // remove historical data from "3 years ago"
        if (mData) {
            pTrForecast = tableSched.children('tbody').children('tr[name="trForecast"]');
            pTrForecast.children('td:eq(0)').text('Forecast');
            for (var p = 1; p <= mData.HistoricalDemandCount; p++) {
                pTrForecast = pTrForecast.clone().insertAfter(pTrForecast);
                pTrForecast.attr('name', 'trHistoricalDemand_' + p);
                pTrForecast.children('td:eq(0)').text('Historical demand ' + (mData.HistoricalDemandYear - p + 1))
                    .next().attr('name', 'tdHistoricalDemand_' + p);
            };
        };
        tableSched.children('tbody').children('tr').each(function (vIndex, vItem) {

            var pTrName = $(vItem).children('td:eq(1), th:eq(1)').attr('name');
            var pTd; // or TH
            var pDiv;
            var pNameArr;

            for (var p = 0; p <= vWeeksCount - 1; p++) {
                pDiv = $('<div>')
                    .text(' ') // put text here
                    .html('&nbsp') // a simple .text(' ') wont work
                    .css('padding', '0 5px')
                    .css('text-align', 'right');

                if (pTrName != null) {
                    pDiv.attr('name', pTrName + '_' + (p + 1));
                    switch (pTrName) {
                        case 'tdAdvanceOrders':
                            pDiv.prop('contentEditable', true);
                            break;

                        case 'tdBackOrders':
                            pDiv.prop('contentEditable', true);
                            break;

                        case 'tdForecast':
                            if (mData && mData.Forecast && mData.Forecast.Rows[p]['QtySold']) {
                                pDiv.prop('contentEditable', true);
                                pDiv.text($.format.number(CDec2(mData.Forecast.Rows[p]['QtySold']), '#,##0'));
                                pDiv.prop('title', mData.Forecast.Rows[p]['DateRange']).tipsy();
                            };
                            break;

                        case 'tdScheduledReceipts':
                            pDiv.prop('contentEditable', true);
                            break;

                        case 'thTopRow':
                            pDiv.html('<b>' + (p + 1) + '</b>');
                            break;

                        case 'tdGrossRequirements':
                            pDiv.css('font-weight', 'bold');
                            break;

                        default:
                            pNameArr = $(vItem).attr('name');
                            if (pNameArr) {
                                pNameArr = pNameArr.split('_');
                                if (pNameArr.length == 2) {
                                    if (pNameArr[0] == 'trHistoricalDemand') {
                                        pDiv.prop('contentEditable', true);
                                        pDiv.text($.format.number(CDec2(mData['HistoricalDemand_' + pNameArr[1]].Rows[p]['QtySold']), '#,##0'));
                                        pDiv.prop('title', mData['HistoricalDemand_' + pNameArr[1]].Rows[p]['DateRange']).tipsy();
                                    }
                                };
                            };

                    }; // END switch (pTrName)
                }; // END pTrName != null

                if (pTrName == 'thTopRow')
                    pTd = $('<th>');
                else
                    pTd = $('<td>');
                pTd.addClass('WeekColumn').append(pDiv);
                $(vItem).append(pTd);

            }; // END for(var p = 0; p <= vWeeksCount - 1; p++) {

        });

        tableSched.find('.WeekColumn').find('[contenteditable]')
            .focus(function () {
                var $this = $(this);
                $this.data('before', $this.html());
            }).bind('blur keyup', function () {
                var $this = $(this);
                if ($this.data('before') !== $this.html()) {
                    $this.data('before', $this.html());
                    $this.trigger('change');
                };
            }).change(function () {

                var pName = $(this).attr('name');
                var pResult;
                var pRowName;
                var pColIndex;

                if (pName != undefined) {
                    pResult = /(\w+)_(\d+)/ig.exec(pName);
                    if (pResult != null && pResult.length == 3) {
                        pRowName = pResult[1]; // index-0 is the entire string
                        pColIndex = parseInt(pResult[2]);
                        UpdateGrid(pRowName, pColIndex);
                    };
                };
            });

    }; // END InitGrid

    function UpdateGrid(vRowName, vColIndex) {
        // vColIndex: 1-based
        var pElement;
        var pName;
        var pInt;

        switch (vRowName) {
            case 'tdAdvanceOrders':
            case 'tdBackOrders':
            case 'tdForecast':
            case 'tdScheduledReceipts':
                pInt = CDec2(tableSched.find('[name=tdAdvanceOrders_' + vColIndex + ']').text()) +
                    CDec2(tableSched.find('[name=tdBackOrders_' + vColIndex + ']').text()) +
                    CDec2(tableSched.find('[name=tdForecast_' + vColIndex + ']').text());
                pElement = tableSched.find('[name=tdGrossRequirements_' + vColIndex + ']');
                pElement.text(AccountingFormat(pInt));
                if (pInt < 0)
                    pElement.addClass('NegativeNumber');
                else
                    pElement.removeClass('NegativeNumber');

                // update Projected Avaialble to Commit Qty 
                if (vColIndex == 1)
                    pName = 'tdProjectedAvaialbletoCommitQty';
                else
                    pName = 'tdProjectedAvaialbletoCommitQty_' + (vColIndex - 1);
                pInt = CDec2(tableSched.find('[name=' + pName + ']').text()) +
                    CDec2(tableSched.find('[name=tdScheduledReceipts_' + vColIndex + ']').text()) -
                    CDec2(tableSched.find('[name=tdGrossRequirements_' + vColIndex + ']').text());
                pElement = tableSched.find('[name=tdProjectedAvaialbletoCommitQty_' + vColIndex + ']');
                pElement.text(AccountingFormat(pInt));
                if (pInt < 0)
                    pElement.addClass('NegativeNumber');
                else
                    pElement.removeClass('NegativeNumber');

                // check next column (if any)
                if (vColIndex < mWeeksCount) UpdateGrid('tdAdvanceOrders', vColIndex + 1);
                break;

        }; // END switch(vRowName) {

    }; // END UpdateGrid

});
$(function () {
    setLevel0("Employees", "../welcome", 1);
    setLevel1("Drp", "../drp/", 1);
    new Drp();

});

function Drp() {

    // controls
    var formMain = $('#formMain')
        .submit(formMain_Submit);
    var txtDateFrom = $('#txtInvoiceDateFrom')
        .width(100)
        .datepicker({
            changeMonth: true,
            changeYear: true
        });
    var txtDateTo = $('#txtInvoiceDateTo')
        .width(100)
        .datepicker({
            changeMonth: true,
            changeYear: true
        });
    var mFindSupplier;
    var divResult = $('#divResult');
    var divData = $('#divData');
    var mDataGrid;  
    var divDrpStocks = $('#divDrpStocks');
    var cbPurchasingFrequency = $('#cbPurchasingFrequency')
        .width(112);
    var divDetails = $('#divDetails');
    var mData;
    var mExtraColumns;
    var spanStockNumber = $('#spanStockNumber');
    var spanStockDescription = $('#spanStockDescription');
    var butGenerate = $('#butGenerate').click(butGenerate_Click);
    var butStockCheckAll = $('#butStockCheckAll').click(butStockCheckAll_Click);
    var butStockCheckNone = $('#butStockCheckNone').click(butStockCheckNone_Click);
    var divTasks = $('#divTasks');
    var divTasksResult = $('#divTasksResult');
    var mDateFrom, mDateTo, mDurationType;
    var txtDateNeeded = $('#txtDateNeeded')
        .width(100)
        .datepicker({
            changeMonth: true,
            changeYear: true
        });
    var spanItemCount = $('#spanItemCount');

    function formMain_Submit(e) {

        var pResult;

        e.preventDefault();
        divResult.hide();
        pResult = Validator.ValidateDate({
            Value1: txtDateFrom.val(),
            Value2: txtDateTo.val(),
            Field1: 'Date From',
            Field2: 'Date To'
        });
        if (pResult != null) {
            ShowMessage(pResult, 'Invalid');
            return;
        };

        divDrpStocks.block();
        AjaxSend({
            url: '/Drp/Stocks',
            data: formMain.serializeObject(),
            success: function (vData) {
                var pMsg = '';

                mData = vData.d;
                if (mData.Error != undefined) {
                    divDrpStocks.unblock();
                    divResult.text(vData.d.Error).MakeError();
                    HandleSessionExpired(vData);
                    return;
                } else if (mData.Text) {
                    divResult.text(mData.Text).MakeHighlight();
                    divDrpStocks.unblock();
                    return;
                };
                mDateFrom = mData.DateFrom;
                mDateTo = mData.DateTo;
                mDurationType = cbPurchasingFrequency.val();
                txtDateFrom.val(mDateFrom);
                txtDateTo.val(mDateTo);
                mData = vData.d.Data;
                mExtraColumns = vData.d.Columns;
                if (mData) {
                    spanItemCount.text(mData.Rows.length);
                    mData.Columns = {
                        RowNum: {
                            Label: 'Row#',
                            MajorDataType: 'Numeric',
                            ContentType: Enum.ProWebGrid.ContentType.RowNumber
                        },
                        'Include': {
                            'Control': 'Checkbox',
                            'MajorDataType': 'Numeric'
                        },
                        'StockNumber': {
                            'Label': 'Stock Number'
                        },
                        'StockDescription': {
                            'Label': 'Stock Description'
                        },
                        'StocksOnHand': {
                            'Label': 'Stocks on hand',
                            Format: '#,##0',
                            'MajorDataType': 'Numeric',
                            Alignment: 'Right'
                        },
                        Qty: {
                            'Label': 'Qty sold',
                            Format: '#,##0',
                            'MajorDataType': 'Numeric',
                            Alignment: 'Right'
                        },
                        SupplierName: {
                            Label: 'Supplier'
                        },
                        BestPrice: {
                            Label: 'Supplier Price',
                            Format: '#,##0.00',
                            MajorDataType: 'Numeric',
                            Alignment: 'Right'
                        },
                        BestPriceUnit2: {
                            Label: 'Supplier Unit Measure'
                        }
                        //                    ,Test: {
                        //                        Control: 'Button',
                        //                        Events: {
                        //                            click: function() {
                        //                                var pDr;

                        //                                pDr=mDataGrid.DataProps(this).DataRow;
                        //                                ShowMessage('Please put stock viewer for stock number '+pDr.StockNumber+'. '+pDr.StockDescription,'Stock Viewer');
                        //                            }
                        //                        }
                        //                    }

                    }; // END vData.d.Columns

                    // add the weekly columns
                    //                for (var p = 0; p <= vData.d.Columns.Rows.length - 1; p++) {
                    //                    mData.Columns['Qty' + vData.d.Columns.Rows[p].DateText] = {
                    //                        Label: 'Qty Sold ' + vData.d.Columns.Rows[p].Text,
                    //                        Format: '#,##0',
                    //                        MajorDataType: 'Numeric',
                    //                        Width: 50
                    //                    };
                    //                };

                    var pTable = divDetails.children('table.TableForm')
                    var pTr;

                    pTable.children('tbody').children('tr.Columns').remove();
                    for (var p = 0; p <= mExtraColumns.Rows.length - 1; p++) {
                        pTr = $('<tr>');
                        pTr.addClass('Columns');
                        pTr.append($('<td>').text(mExtraColumns.Rows[p].Text));
                        pTr.append($('<td>', { id: 'Qty' + mExtraColumns.Rows[p].DateText }));
                        //.css('text-align', 'right');

                        pTable.append(pTr);
                    };

                    mDataGrid = $('#divData').ProDataGrid(mData);
                    mDataGrid.AfterRowActivate(mDataGrid_AfterRowActivate);
                } else {
                    spanItemCount.text('N/A');
                    pMsg = 'The data is too big to be displayed on the page.<br />';
                };

                divResult
                    .html(pMsg + '<a href="/download/' + vData.d.Zip + '">Please click here to download the Excel workbook, ' + HtmlEncode(vData.d.Zip) + '</a>')
                    .MakeHighlight();
                divDrpStocks.unblock();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divDrpStocks.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });               // AjaxSend
    }; // END formMain_Submit

    function mDataGrid_AfterRowActivate(e) {

        var pDr;
        var pText;

        pDr = mDataGrid.RowData(e.SelectedRow);
        spanStockNumber.text(pDr.StockNumber);
        spanStockDescription.text(pDr.StockDescription);

        for (var p = 0; p <= mExtraColumns.Rows.length - 1; p++) {
            pText = pDr['Qty' + mExtraColumns.Rows[p].DateText];
            if (IsNumeric(pText))
                pText = $.format.number(
                    pText,
                    '#,##0'
                );
            $('#' + 'Qty' + mExtraColumns.Rows[p].DateText).text(pText);
        };

        divDetails.dialog({
            width: 500,
            height: 400
        });

    }; // END mDataGrid_AfterRowActivate

    function butStockCheckAll_Click(e) {

        mDataGrid.CheckAll('Include');

    };

    function butStockCheckNone_Click(e) {

        mDataGrid.CheckAll('Include', 0);

    };

    function butGenerate_Click(e) {

        var pCheckedData;

        divTasksResult.hide();

        if (!mDataGrid) {
            divTasksResult.text('Please click Show Stocks first').MakeError();
            return;
        };
        pCheckedData = mDataGrid.GetCheckedRows('Include', ['StockID']);
        if (pCheckedData.length == 0) {
            divTasksResult.text('Please check at least 1 SKU').MakeError();
        } else {
            if (IsDate(txtDateNeeded.val()) == false) {
                divTasksResult.text('Date Needed must be a valid date').MakeError();
                return;
            };
            ShowMessage('Do you want to generate proposed PO\'s for the checked SKU\'s?', 'Generate', {
                'Yes': function () {
                    var pData = {};
                    var pStocks = [];
                    var p;

                    for (p = 0; p <= pCheckedData.length - 1; p++)
                        pStocks.push(pCheckedData[p].StockID);
                    pData.DateFrom = mDateFrom;
                    pData.DateTo = mDateTo;
                    pData.DurationType = mDurationType;
                    pData.DateNeeded = txtDateNeeded.val();
                    pData.StockIDs = pStocks;
                    $(this).dialog('close');
                    divTasks.block();
                    AjaxSend({
                        url: '/drp/GenerateProposedPOs',
                        data: pData,
                        success: function (vData) {
                            divTasks.unblock();
                            if (vData.d.Text == 'Please login') {
                                window.location = '/login';
                                return;
                            } else if (vData.d.Error) {
                                divTasksResult.text(vData.d.Error).MakeError();
                                return;
                            };
                            if (vData.d.Text) {
                                divTasksResult.text(vData.d.Text).MakeHighlight();
                            };
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            divTasks.unblock();
                            divTasksResult.text(XMLHttpRequest.responseText).MakeError();
                        }
                    });
                },
                'No': function () {
                    $(this).dialog('close');
                }
            });
        };

    }; // END butGenerate_Click

}; // END Drp
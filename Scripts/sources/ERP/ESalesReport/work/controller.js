define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ESalesReport/work/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ESalesReport.work.controller', null, function () {

        var mOut = {};

        mElement = $(mHtml);
        mOut.element = mElement;
        var mCbYear = mElement.find('[name=cbYear]').on('change', OnYearChange);
        var mTotalRow;

        $('#tabESalesReportWork').empty().append(mElement); //.trigger('create');
        
        (function PopulateYears() {
            var pCurrentYear = new Date().getFullYear();
            for (var pYear = pCurrentYear - 20; pYear <= pCurrentYear; pYear++) {
                var pOption = new Option(pYear, pYear);
                mCbYear.append(pOption);
            }
            mCbYear.val(pCurrentYear);
        })();

        var pTemplateRow, pJanuaryRow;
        function PopulateMonths(vData) {
            vData = $.extend({
                VatRate: 12,
                OutputTaxRate: 1
            }, vData);
            mElement.find('[name=cbYear]').val(vData.ZReadYear);
            mElement.find('[name=txtMachineNumber]').val(vData.MachineNumber);
            mElement.find('[name=txtVatRate]').val(vData.VatRate);
            mElement.find('[name=txtOutputTaxRate]').val(vData.OutputTaxRate);
            var pTBody = mElement.find('[name=table] tbody');
            if (pTemplateRow == null) {
                pTemplateRow = pTBody.find('tr');
                pJanuaryRow = pTemplateRow.clone();
                pTemplateRow.find('[name=month]').text('Previous Total for the Period');
                pTemplateRow.find('[name=invoice-number]').html('');
                pTemplateRow.find('[name=gross-sales] input').attr('readonly', 'readonly');
                pTemplateRow.find('[name=gross-sales]').append('<div>as of 12/31/9999</div>');
                pTemplateRow.find('[name=accumulated-gross-sales]').html('');
                pTemplateRow.find('[name=vat-sales] input').attr('readonly', 'readonly');
                pTemplateRow.find('[name=vat-sales]').append('<div>as of 12/31/9999</div>');
                pTemplateRow.find('[name=accumulated-vat-sales]').html('');
                pTemplateRow.find('[name=output-tax]').html('');
            }
            pTBody.html('');
            pTBody.append(pTemplateRow);
            pTemplateRow.find('[name=gross-sales] input').val(AccountingFormat(vData.PreviousGrossSales));
            if (vData.PreviousYear)
                pTemplateRow.find('[name=gross-sales] div').text('as of 12/31/' + CInt2(vData.PreviousYear));
            else
                pTemplateRow.find('[name=gross-sales] div').text('');
            pTemplateRow.find('[name=vat-sales] input').val(AccountingFormat(vData.PreviousVatSales));
            if (vData.PreviousYear)
                pTemplateRow.find('[name=vat-sales] div').text('as of 12/31/' + CInt2(vData.PreviousYear));
            else
                pTemplateRow.find('[name=vat-sales] div').text('');

            for (var pMonth = 1; pMonth <= 12; pMonth++) {
                var pRow = pJanuaryRow.clone();
                var pMonthNumber = $.format.number(pMonth, '00');
                pRow.attr('name', 'month' + pMonthNumber);
                pRow.find('[name=month]').text($.format.locale().date.monthsFull[pMonth - 1]);
                pRow.find('[name=invoice-number] input').val(AccountingFormat(vData['InvoiceNumber' + pMonthNumber], 0));
                pRow.find('[name=gross-sales] input').on('keyup paste', RecomputeValues);
                pRow.find('[name=gross-sales] input').val(AccountingFormat(vData['GrossSales' + pMonthNumber], 2));
                pRow.find('[name=accumulated-gross-sales] input').val(AccountingFormat(vData['AccumulatedGrossSales' + pMonthNumber], 2));
                pRow.find('[name=vat-sales] input').on('keyup paste', RecomputeValues);
                pRow.find('[name=vat-sales] input').val(AccountingFormat(vData['VatSales' + pMonthNumber], 2));
                pRow.find('[name=accumulated-vat-sales] input').val(AccountingFormat(vData['GrossSales' + pMonthNumber], 2));
                //pRow.find('[name=output-tax] input').on('keyup paste', RecomputeValues);
                pRow.find('[name=output-tax] input').val(AccountingFormat(vData['GrossSales' + pMonthNumber], 2));
                pTBody.append(pRow);
            }

            mTotalRow = mElement.find('[name=table] tfoot tr');
            mTotalRow.find('[name=month]').text('GRAND TOTALS');
            mTotalRow.find('[name=invoice-number]').html('');
            mTotalRow.find('[name=gross-sales]').text('9,876,543.21');
            mTotalRow.find('[name=accumulated-gross-sales]').html('');
            mTotalRow.find('[name=vat-sales]').text('9,876,543.21');
            mTotalRow.find('[name=accumulated-vat-sales]').html('');
            mTotalRow.find('[name=output-tax]').text('9,876,543.21');
            RecomputeValues();
        }
        PopulateMonths({});

        function OnYearChange() {
            var pMachineNumber = mElement.find('[name=txtMachineNumber]').val();
            pMachineNumber = pMachineNumber.trim();
            if (pMachineNumber == '') {
                PopulateMonths({});
                return;
            }
            AjaxSend({
                url: '/Browser/TransESales',
                data: {
                    ZReadYear: mCbYear.val(),
                    MachineNumber: pMachineNumber
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    if (vData.Results.Rows.length == 0)
                        PopulateMonths({
                            MachineNumber: pMachineNumber
                        });
                    else if (vData.Results.Rows.length > 0) {
                        if (vData.Results.Rows[0].ID)
                            PopulateMonths(vData.Results.Rows[0]);
                        else 
                            PopulateMonths({
                                ZReadYear: mCbYear.val(),
                                MachineNumber: pMachineNumber,
                                PreviousGrossSales: vData.Results.Rows[0].PreviousGrossSales,
                                PreviousVatSales: vData.Results.Rows[0].PreviousVatSales,
                                PreviousYear: vData.Results.Rows[0].ZReadYear
                            });
                    }
                },
                error: function(vError) {
                    NewMessageBox(vError);
                }
            });
        }

        function BrowseMachineNumber() {
            BrowsePopup('TransESales', 'Machine Number',
                [
                    {
                        Search: mElement.find('[name=txtMachineNumber]').val(),
                        StyleFormat: 'table',
                        DisplayText: [
                            { 
                                Column: 'ZReadYear',
                                Label: 'Year'
                            }, {
                                Column: 'MachineNumber',
                                Label: 'Machine Number'
                            }
                        ],
                        Click: function (e) {
                            mElement.find('[name=cbYear]').val(e.ZReadYear);
                            mElement.find('[name=txtMachineNumber]').val(e.MachineNumber);
                            OnYearChange();
                        }
                    }
                ], { 
                    
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butMachineNumber]').click(function() {
            BrowseMachineNumber();
        });

        mElement.find('[name=butSave]').click(function() {
            WebToast('Saving. Please wait.');
            try {
                pData = {
                    ZReadYear: mElement.find('[name=cbYear]').val(),
                    MachineNumber: mElement.find('[name=txtMachineNumber]').val(),
                    VatRate: mElement.find('[name=txtVatRate]').val(),
                    OutputTaxRate: mElement.find('[name=txtOutputTaxRate]').val(),
                };
                if (!IsNumeric(pData.ZReadYear)) {
                    NewMessageBox('ZRead Year must be a valid number.');
                    return;
                }
                if (pData.MachineNumber.trim() == '') {
                    NewMessageBox('Machine Number must not be blank.');
                    return;
                }
                if (!IsNumeric(pData.VatRate)) {
                    NewMessageBox('VAT Rate must be a valid number.');
                    return;
                }
                if (!IsNumeric(pData.OutputTaxRate)) {
                    NewMessageBox('Output Tax Rate must be a valid number.');
                    return;
                }
                
                if (RecomputeValues(pData, true) == false)
                    return;

                AjaxSend({
                    url: '/ESales/Save',
                    data: pData,
                    success: function(vData) {
                        if (vData.Result == 'ok')
                            NewMessageBox('Data was saved successfully.');
                        else if (vData.Error)
                            NewMessageBox(vData.Error);
                    },
                    error: function(vError) {
                        NewMessageBox(vError);
                    }
                });
            } catch(e) {
                NewMessgeBox(e.message);
            }
        });

        function RecomputeValues(pData, vShowPrompts) {
            if (pData == null)
                pData = {};
            var pTBody = mElement.find('[name=table] tbody');
            var pLastInvoiceNumber = 0;
            
            var pAccumulatedGrossSales = CDec2(pTemplateRow.find('[name=gross-sales] input').val());
            var pAccumulatedVatSales = CDec2(pTemplateRow.find('[name=vat-sales] input').val());
            var pAccumulatedOutputTax = 0;
            var pOutputTaxRate = CDec2(mElement.find('[name=txtOutputTaxRate]').val());
            for (var pMonth = 1; pMonth <= 12; pMonth++) {
                var pCellValue;
                var pMonthNumber = $.format.number(pMonth, '00');
                var pTR = pTBody.find('tr[name=month' + pMonthNumber + ']');

                if (vShowPrompts) {
                    pCellValue = pTR.find('[name=invoice-number] input').val();
                    if (pCellValue == '-' || pCellValue == '')
                        pCellValue = 0;
                    if (!IsNumeric(pCellValue)) {
                        NewMessageBox($.format.locale().date.monthsFull[pMonth - 1] + ' Invoice Number must be numeric.');
                        return false;
                    }
                    pCellValue = CInt2(pCellValue);
                    if (pCellValue < pLastInvoiceNumber) {
                        NewMessageBox($.format.locale().date.monthsFull[pMonth - 1] + ' Invoice Number must not be lesser than the previous month\'s invoice number.');
                        return false;
                    }
                    pLastInvoiceNumber = pCellValue;
                    pData['InvoiceNumber' + pMonthNumber] = pCellValue;
                }

                pCellValue = pTR.find('[name=gross-sales] input').val();
                if (pCellValue == '-' || pCellValue == '')
                    pCellValue = 0;
                if (vShowPrompts)
                    if (!IsNumeric(pCellValue)) {
                        NewMessageBox($.format.locale().date.monthsFull[pMonth - 1] + ' Gross Sales must be numeric.');
                        return false;
                    }
                pCellValue = CDec2(pCellValue);
                pAccumulatedGrossSales += pCellValue;
                pData['GrossSales' + pMonthNumber] = pCellValue;

                pTR.find('[name=accumulated-gross-sales] input').val(AccountingFormat(pAccumulatedGrossSales));
                pData['AccumulatedGrossSales' + pMonthNumber] = pAccumulatedGrossSales;

                pCellValue = pTR.find('[name=vat-sales] input').val();
                if (pCellValue == '-' || pCellValue == '')
                    pCellValue = 0;
                if (vShowPrompts)
                    if (!IsNumeric(pCellValue)) {
                        NewMessageBox($.format.locale().date.monthsFull[pMonth - 1] + ' VAT Sales must be numeric.');
                        return false;
                    }
                pCellValue = CDec2(pCellValue);
                pAccumulatedVatSales += pCellValue;
                pData['VatSales' + pMonthNumber] = pCellValue;

                pTR.find('[name=accumulated-vat-sales] input').val(AccountingFormat(pAccumulatedVatSales));
                pData['AccumulatedVatSales' + pMonthNumber] = pAccumulatedVatSales;

                pCellValue = pOutputTaxRate / 100 * CDec2(pTR.find('[name=vat-sales] input').val());
                pAccumulatedOutputTax += pCellValue;
                pData['OutputTax' + pMonthNumber] = pCellValue;
                pTR.find('[name=output-tax] input').val(AccountingFormat(pCellValue));
            }
        
            //mTotalRow.find('[name=invoice-number]').html('');
            mTotalRow.find('[name=gross-sales]').text(AccountingFormat(pAccumulatedGrossSales));
            //mTotalRow.find('[name=accumulated-gross-sales]').html('');
            mTotalRow.find('[name=vat-sales]').text(AccountingFormat(pAccumulatedVatSales));
            //mTotalRow.find('[name=accumulated-vat-sales]').html('');
            mTotalRow.find('[name=output-tax]').text(AccountingFormat(pAccumulatedOutputTax));
            return true;
        }

        mElement.find('[name=butExportToPdf]').click(function() {
            NewMessageBox('This function is not yet implemented. js2UKTCX7C6K!');
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});
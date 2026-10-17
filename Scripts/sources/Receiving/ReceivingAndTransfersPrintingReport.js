require([
    'common'
    ], function () {

        var FowstVendorID = '';
        var FowstID = '';
        var countIfHasTo = 0;

        $('document').ready(function () {

            //MarkedAsRequired();
            GetAllFOWST();
            //SearchOrganizationPopUp();

            window.ReceivingTransfer = {
                GetRequiredData: function () {
                    //MarkedAsRequired();
                    GetAllFOWST();
                }
            }

            $(window).resize(function () {
                if ($(window).width() >= 545) {
                    $('#tableReceivingAndTransfers > thead').show();
                }
                else {
                    $('#tableReceivingAndTransfers > thead').hide();
                }
            })

            $('body').on('click', '#btnReceivingTransferDateRange', function () {
                DualCalendar({
                    Click: function (e) {
                        $('#btnReceivingTransferDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', e.From + ' to ' + e.To);
                        $('#DateRangeFrom').text(e.From);
                        $('#DateRangeTo').text(e.To);
                        $('#DateRangeSeparator').show();

                        if ($('#selReceivingTransferTransType').val() != "" && $('#txtReceivingTransferFowstID').val() != "") {
                            FowstID = $('#txtReceivingTransferFowstVendorID').val();
                            FowstVendorID = $('#txtReceivingTransferFowstID').val();
                            GetReceivingAndTransfersReport();
                        }
                    }
                })
                return;
                $('#panReceivingTransferDateRange').popup('open');
            })
            .on('change', '#selReceivingTransferTransType', function () {
                //MarkedAsRequired();
                if ($(this).val() == "") {
                    $(this).removeClass('btnNotRequired').addClass('btnRequired');
                }
                else {
                    $(this).removeClass('btnRequired').addClass('btnNotRequired');
                }

                if ($(this).val() != "" && $('#btnReceivingTransferDateRange').text() != "Date to Range" && $('#txtReceivingTransferFowstID').val() != "") {
                    FowstID = $('#txtReceivingTransferFowstVendorID').val();
                    FowstVendorID = $('#txtReceivingTransferFowstID').val();
                    GetReceivingAndTransfersReport();
                }
            })
            .on('click', '#btnRecevingTransferFowst', function () {
                $('#panReceivingTransferFowst').popup('open');
            })
            .on('click', '#btnReceivingTransferDone', function (e) {
                e.preventDefault();
                var dateFrom = new Date($('#txtReceivingTransferDateFrom').text());
                var dateTo = new Date($('#txtReceivingTransferDateTo').text());
                var dateNow = new Date();

                if (dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) {
                    $('#spanDateRangeError').show();
                }
                else {
                    $('#spanDateRangeError').hide();
                    $('#btnReceivingTransferDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtReceivingTransferDateFrom').text() + ' to ' + $('#txtReceivingTransferDateTo').text())
                    $('#DateRangeFrom').text($('#txtReceivingTransferDateFrom').text());
                    $('#DateRangeTo').text($('#txtReceivingTransferDateTo').text());
                    $('#DateRangeSeparator').show();
                    $('#panReceivingTransferDateRange').popup('close');

                    if ($('#selReceivingTransferTransType').val() != "" && $('#txtReceivingTransferFowstID').val() != "") {
                        FowstID = $('#txtReceivingTransferFowstVendorID').val();
                        FowstVendorID = $('#txtReceivingTransferFowstID').val();
                        GetReceivingAndTransfersReport();
                    }
                }
            })
            .on('click', '#btnReceivingTransferCancel', function (e) {
                e.preventDefault();
                $('#panReceivingTransferDateRange').popup('close');
            })
            .on('change', '#selFowst1', function () {
                if ($(this).val() == "") {
                    $('#spanRecTransFowst1Error').show();
                }
                else {
                    $('#spanRecTransFowst1Error').hide();
                }
            })
            .on('change', '#selFowst2', function () {
                $('#divThirdParty').hide();
                if ($('#selFowst2').val() != "") {
                    $('#txtReceivingTransferFowstVendorID').val($('#selFowst2').val());
                    FowstID = $('#txtReceivingTransferFowstVendorID').val();
                    FowstVendorID = $('#txtReceivingTransferFowstID').val();
                    if ($('#selFowst2').val() == "thirdparty") {
                        $('#selFowst2').val('').selectmenu('refresh');
                        BrowsePopup('LibraryOrganization', 'Organization',
                            [
                                {
                                    DisplayText: [{ Column: 'CompanyName' }],
                                    Click: function (e) {
                                        $('#selFowst2').val('thirdparty').selectmenu('refresh');
                                        $('#divThirdParty').show();
                                        $('#txtVendorName').text(e.CompanyName);
                                        FowstVendorID = e.ID;
                                    },
                                    Close: function () {
                                        $('#panReceivingTransferFowst').popup('open');
                                    }
                                }
                            ])
                        //$('#panReceivingTransferFowst').popup('close');
                        //$('#panReceivingTransferThirdParty').popup('open');
                    }
                }
            })
            .on('click', '#btnReceivingTransferFowstDone', function () {
                if ($('#selFowst1').val() == "") {
                    $('#spanRecTransFowst1Error').show();
                    return;
                }

                //                if ($('#selFowst2').val() != "") {
                //                    if ($('#selFowst2').val() == $('#selFowst1').val()) {
                //                        $('#spanRecTransFowst2Error').show();
                //                        return;
                //                    }
                //                }


                $('#spanRecTransFowst1Error').hide();

                $('#btnRecevingTransferFowst').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#selFowst1 option:selected').text());
                $('#txtRecTransFowstName').text($('#selFowst1 option:selected').text())
                $('#txtReceivingTransferFowstID').val($('#selFowst1').val());

                if ($('#selFowst2').val() != "") {
                    if ($('#selFowst2').val() == "thirdparty") {
                        $('#txtReceivingTransferFowstVendorID').val(FowstVendorID);
                        $('#btnRecevingTransferFowst').attr('title', $('#selFowst1 option:selected').text() + ' to ' + $('#selFowst2 option:selected').text());
                        $('#txtRecTransFowstName').text($('#selFowst1 option:selected').text() + ' to ' + $('#txtVendorName').text())
                    }
                    else {
                        $('#txtReceivingTransferFowstVendorID').val($('#selFowst2').val());
                        $('#btnRecevingTransferFowst').attr('title', $('#selFowst1 option:selected').text() + ' to ' + $('#selFowst2 option:selected').text());
                        $('#txtRecTransFowstName').text($('#selFowst1 option:selected').text() + ' to ' + $('#selFowst2 option:selected').text())
                    }
                }
                else {
                    $('#txtReceivingTransferFowstVendorID').val('');
                }

                if ($('#btnReceivingTransferDateRange').text() != "Date to Range" && $('#selReceivingTransferTransType').val() != "") {
                    FowstID = $('#txtReceivingTransferFowstVendorID').val();
                    FowstVendorID = $('#txtReceivingTransferFowstID').val();
                    GetReceivingAndTransfersReport();
                }
                $('#panReceivingTransferFowst').popup('close');
            })
            .on('click', '#btnReceivingTransferFowstCancel', function () {
                $('#txtRecTransFowstName').text('FOWST');
                $('#txtReceivingTransferFowst1ID').val('');

                if ($('#txtReceivingTransferFowst1ID').val() == "") {
                    $('#btnRecevingTransferFowst').removeClass('btnNotRequired').addClass('btnRequired');
                }
                $('#panReceivingTransferFowst').popup('close');
            })
            .on('click', '#btnSearchRecTrans', function () {
                $('#divSearchRecTrans').css({ visibility: 'visible', display: 'block' });
                $('#txtSearchRecTrans').focus();
                $(this).hide();
            })
            .on('blur', '#txtSearchRecTrans', function () {
                if ($(this).val() == "") {
                    $('#btnSearchRecTrans').show();
                    $('#divSearchRecTrans').css({ visibility: 'hidden', display: 'none' });
                }
            })
            .on('click', '#btnBackToReceivingTransferFowst', function () {
                $('#panReceivingTransferThirdParty').popup('close');
                $('#selFowst2').val('').selectmenu('refresh');
                $('#panReceivingTransferFowst').popup('open');
            })
            .on('keypress', '#txtSearchReceivingTransferOrg', function (e) {
                if (e.keyCode == 13) {
                    SearchOrganizationPopUp($(this).val());
                }
            })
            .on('click', '.btnSubReportTransT1', function () {
                var pData = {};

                pData.DateFrom = $('#DateRangeFrom').text();
                pData.DateTo = $('#DateRangeTo').text();
                pData.TransactionType = $(this).parents().attr('name');

                pData.Fowst1ID = $('#txtReceivingTransferFowstID').val();
                pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val();

                $('#txtStockType').text('Stock Transfer Details')

                GetReceivingAndTransfersSubReport(pData);
            })
            .on('click', '.btnSubReportRecT1', function () {
                var pData = {};

                pData.DateFrom = $('#DateRangeFrom').text();
                pData.DateTo = $('#DateRangeTo').text();
                pData.TransactionType = $(this).parents().attr('name');

                pData.Fowst1ID = $('#txtReceivingTransferFowstID').val();
                pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val();

                $('#txtStockType').text('Stock Receiving Details')

                GetReceivingAndTransfersSubReport(pData);
            })
            .on('click', '.btnSubReportTransT2', function () {
                var pData = {};

                pData.DateFrom = $('#DateRangeFrom').text();
                pData.DateTo = $('#DateRangeTo').text();
                pData.TransactionType = $(this).parents().attr('name');

                pData.Fowst1ID = $('#txtReceivingTransferFowstVendorID').val();
                pData.FowstVendorID = $('#txtReceivingTransferFowstID').val();

                $('#txtStockType').text('Stock Transfer Details')

                GetReceivingAndTransfersSubReport(pData);
            })
            .on('click', '.btnSubReportRecT2', function () {
                var pData = {};

                pData.DateFrom = $('#DateRangeFrom').text();
                pData.DateTo = $('#DateRangeTo').text();
                pData.TransactionType = $(this).parents().attr('name');

                pData.Fowst1ID = $('#txtReceivingTransferFowstID').val();
                pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val();

                $('#txtStockType').text('Stock Receiving Details')

                GetReceivingAndTransfersSubReport(pData);
            })

        })//END Document

        function MarkedAsRequired() {
            var RequiredButton = $('.btnRequired');

            $(RequiredButton).each(function (i, e) {
                var glow = $(this);

                var glowTime = setInterval(function () {
                    if (glow.is(':button')) {
                        if (glow.text() == "Date to Range" || glow.text() == "FOWST") {
                            glow.hasClass('glow') ? glow.removeClass('glow') : glow.addClass('glow');
                        }
                        else {
                            clearInterval(glowTime);
                            glow.removeClass('glow');
                        }
                    }
                    else {
                        if ($('#selReceivingTransferTransType option:selected').val() == "") {
                            glow.hasClass('glow') ? glow.removeClass('glow') : glow.addClass('glow');
                        }
                        else {
                            clearInterval(glowTime);
                            glow.removeClass('glow');
                        }
                    }

                }, 1000);
            })
        }

        function GetReceivingAndTransfersReport() {
            var pData = {};

            pData.DateFrom = $('#DateRangeFrom').text();
            pData.DateTo = $('#DateRangeTo').text();
            pData.TransactionType = $('#selReceivingTransferTransType').val();
            pData.Fowst1ID = $('#txtReceivingTransferFowstID').val(); //$('#selFowst2 option:selected').text()
            pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val(); // $('#selFowst1 option:selected').text()

            if ($(window).width() >= 545) {
                $('#tableReceivingAndTransfers > thead').show();
            }
            else {
                $('#tableReceivingAndTransfers > thead').hide();
            }

            AjaxSend({
                url: 'Receiving/GetReceivingAndTransfersReport',
                data: pData,
                success: function (vData) {
                    $('#divNoPage').hide();
                    $('#divSecond').show();

                    DisplayResult(vData.Results.Rows);
                    NewDisplayResult(vData.Results.Rows);

                    $('#tbFOWSTAndOrganizationSubReportSummary').empty();

                    if (vData.Results.Rows.length > 0) {

                        if (FowstID != "") {
                            if (countIfHasTo == 0) {
                                $('#txtReceivingTransferFowstID').val(FowstID);
                                $('#txtReceivingTransferFowstVendorID').val(FowstVendorID);
                                countIfHasTo = 1;
                                GetReceivingAndTransfersReport();
                            }
                            else {
                                $('#txtReceivingTransferFowstID').val(FowstVendorID);
                                $('#txtReceivingTransferFowstVendorID').val(FowstID);
                                countIfHasTo = 0;
                            }
                        }
                    }
                    else {
                        if (countIfHasTo == 1) {
                            $('.table2').hide();
                            $('#divNoPage').hide();
                            $('#txtReceivingTransferFowstID').val(FowstVendorID);
                            $('#txtReceivingTransferFowstVendorID').val(FowstID);
                            countIfHasTo = 0;
                        }
                        else {
                            $('#divFortableReceivingAndTransfers').hide();
                            $('#dvSummaryFromTo3').hide();
                            $('#divNoPage').show();
                        }
                    }
                },
                complete: function () {
                    //                    $('#tBodyFowstReportSummary > tr').each(function () {
                    //                        $(this).find('td').find('b').remove()
                    //                    })
                    //                    $('#tBodyFowstReportSummary').parent().table('refresh')
                }
            })
        }

        function NewDisplayResult(vRows) {
            $('#divFortableReceivingAndTransfers').show();
            $('#divNoPage').hide();

            var AprevFOWST = '';
            var AFowstCount = 0;
            var BprevFOWST = '';
            var BFowstCount = 0;

            if (countIfHasTo == 0) {
                $('.table2').hide();

                var AcountReceivingTransCount = 0;
                var AcountReceivingSKUCount = 0;
                var AcountReceivingCogs = 0;
                var AcountTransferTransCount = 0;
                var AcountTransferSKUCount = 0;
                var AcountTransferCogs = 0;

                if ($('#txtReceivingTransferFowstVendorID').val() != "") {
                    if ($('#selFowst2').val() == "thirdparty") {
                        $('#lblFowst').text('FROM ' + $('#selFowst1 option:selected').text() + ' TO ' + $('#txtVendorName').text());
                    }
                    else {
                        $('#lblFowst').text('FROM ' + $('#selFowst1 option:selected').text() + ' TO ' + $('#selFowst2 option:selected').text());
                    }
                }
                else {
                    $('#lblFowst').text('FROM ' + $('#selFowst1 option:selected').text())
                }

                for (var i = 0; i < vRows.length; i++) {

                    //(vRows[i].TransactionTo == "" ? $('#lblFowst').text('FROM ' + $('#selFowst1 option:selected').text()) : $('#lblFowst').text('FROM ' + $('#selFowst1 option:selected').text() + ' TO ' + $('#selFowst2 option:selected').text()));
                    AprevFOWST = vRows[i].TransactionFROM;
                    AFowstCount++;

                    if (vRows[i].TransactionType == 'Receiving') {
                        TransType = 'Stocks Receipts';
                        TransTypeFilter = 'Stock Receiving';

                        AcountReceivingTransCount = AcountReceivingTransCount + CInt2(vRows[i].TransCount);
                        AcountReceivingSKUCount = AcountReceivingSKUCount + CInt2(vRows[i].SKUCount);
                        AcountReceivingCogs = AcountReceivingCogs + CDec2(vRows[i].COGS);

                    } else {
                        TransType = 'Stock Transfers';
                        TransTypeFilter = TransType;
                        AcountTransferTransCount = AcountTransferTransCount + CInt2(vRows[i].TransCount);
                        AcountTransferSKUCount = AcountTransferSKUCount + CInt2(vRows[i].SKUCount);
                        AcountTransferCogs = AcountTransferCogs + CDec2(vRows[i].COGS);
                    }
                }

                $('#colgroupARecTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + CInt2(AcountReceivingTransCount));
                $('#colgroupARecSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + CInt2(AcountReceivingSKUCount));
                $('#colgroupARecCOGS').html(AccountingFormat(CDec2(AcountReceivingCogs)));

                $('#colgroupATransTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + CInt2(AcountTransferTransCount));
                $('#colgroupATransSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + CInt2(AcountTransferSKUCount));
                $('#colgroupATransCOGS').html('<b class="ui-table-cell-label">Trans Value in COGS Count</b>' + AccountingFormat(CDec2(AcountTransferCogs)));

                $('#colgroupAVarianceTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + AccountingFormat(CDec2(AcountTransferTransCount - AcountReceivingTransCount)));
                $('#colgroupAVarianceSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + AccountingFormat(CDec2(AcountTransferSKUCount - AcountReceivingSKUCount)));
                $('#colgroupAVarianceCOGS').html('<b class="ui-table-cell-label">Trans Value in COGS Count</b>' + AccountingFormat(CDec2(AcountTransferCogs - AcountReceivingCogs)));
            }
            else {
                $('.table2').show();

                var BcountReceivingTransCount = 0;
                var BcountReceivingSKUCount = 0;
                var BcountReceivingCogs = 0;
                var BcountTransferTransCount = 0;
                var BcountTransferSKUCount = 0;
                var BcountTransferCogs = 0;

                if ($('#txtReceivingTransferFowstVendorID').val() != "") {
                    if ($('#selFowst2').val() == "thirdparty") {
                        $('#lblFowst2').text('FROM ' + $('#txtVendorName').text() + ' TO ' + $('#selFowst1 option:selected').text());
                    }
                    else {
                        $('#lblFowst2').text('FROM ' + $('#selFowst2 option:selected').text() + ' TO ' + $('#selFowst1 option:selected').text());
                    }
                }
                else {
                    $('#lblFowst2').text('FROM ' + $('#selFowst2 option:selected').text())
                }

                for (var i = 0; i < vRows.length; i++) {

                    //(vRows[i].TransactionTo == "" ? $('#lblFowst2').text('FROM ' + $('#selFowst2 option:selected').text()) : $('#lblFowst2').text('FROM ' + $('#selFowst2 option:selected').text() + ' TO ' + $('#selFowst1 option:selected').text()));
                    BprevFOWST = vRows[i].TransactionFROM;
                    BFowstCount++;

                    if (vRows[i].TransactionType == 'Receiving') {
                        TransType = 'Stocks Receipts';
                        TransTypeFilter = 'Stock Receiving';

                        BcountReceivingTransCount = BcountReceivingTransCount + CInt2(vRows[i].TransCount);
                        BcountReceivingSKUCount = BcountReceivingSKUCount + CInt2(vRows[i].SKUCount);
                        BcountReceivingCogs = BcountReceivingCogs + CDec2(vRows[i].COGS);

                    } else {
                        TransType = 'Stock Transfers';
                        TransTypeFilter = TransType;
                        BcountTransferTransCount = BcountTransferTransCount + CInt2(vRows[i].TransCount);
                        BcountTransferSKUCount = BcountTransferSKUCount + CInt2(vRows[i].SKUCount);
                        BcountTransferCogs = BcountTransferCogs + CDec2(vRows[i].COGS);
                    }
                }

                $('#colgroupBRecTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst2').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + CInt2(BcountReceivingTransCount));
                $('#colgroupBRecSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + CInt2(BcountReceivingSKUCount));
                $('#colgroupBRecCOGS').html('<b class="ui-table-cell-label">Trans Value in COGS Count</b>' + AccountingFormat(CDec2(BcountReceivingCogs)));

                $('#colgroupBTransTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst2').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + CInt2(BcountTransferTransCount));
                $('#colgroupBTransSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + CInt2(BcountTransferSKUCount));
                $('#colgroupBTransCOGS').html('<b class="ui-table-cell-label">Trans Value in COGS Count</b>' + AccountingFormat(CDec2(BcountTransferCogs)));

                $('#colgroupBVarianceTransCount').html('<b class="ui-table-cell-label ui-table-cell-label-top">' + $('#lblFowst2').text() + '</b><b class="ui-table-cell-label">Trans Count</b>' + AccountingFormat(CDec2(BcountTransferTransCount - BcountReceivingTransCount)));
                $('#colgroupBVarianceSKUCount').html('<b class="ui-table-cell-label">SKU Count</b>' + AccountingFormat(CDec2(BcountTransferSKUCount - BcountReceivingSKUCount)));
                $('#colgroupBVarianceCOGS').html('<b class="ui-table-cell-label">Trans Value in COGS Count</b>' + AccountingFormat(CDec2(BcountTransferCogs - BcountReceivingCogs)));

            }
        }


        function DisplayResult(vRows) {
            var pLV = $('#tbFOWSTReportSummary');
            var prevFOWST = '';
            var FowstCount = 0;
            var TransType = '';
            var TransTypeFilter = '';

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {

                if (prevFOWST != vRows[i].TransactionFROM) {
                    if (FowstCount == 0) {
                        $('#lblFOWST').text('FROM ' + vRows[i].TransactionFROM + ' To ' + vRows[i].TransactionTo);
                        prevFOWST = vRows[i].TransactionFROM;
                        FowstCount++;
                    }
                    else {
                        pLV = $('#tbFOWSTAndOrganizationReportSummary');
                        pLV.empty();
                        $('#lblFOWST2').text('FROM ' + vRows[i].TransactionFROM + ' To ' + vRows[i].TransactionTo);
                        prevFOWST = vRows[i].TransactionFROM;
                        FowstCount++;
                    }

                }

                if (vRows[i].TransactionType == 'Receiving') {
                    TransType = 'Stocks Receipts';
                    TransTypeFilter = 'Stock Receiving';
                } else {
                    TransType = 'Stock Transfers';
                    TransTypeFilter = TransType;
                }

                pItem = $('<tr>')
                            .attr('fowstcount', FowstCount)
                            .attr('name', TransTypeFilter)
                            .append($('<td>')
                                .text(TransType)
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].TransCount))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].SKUCount))
                            )
                            .append($('<td>')
                                .text(CDec2(vRows[i].COGS))
                            )
                            .click(function () {
                                var pData = {};

                                pData.DateFrom = $('#DateRangeFrom').text();
                                pData.DateTo = $('#DateRangeTo').text();
                                pData.TransactionType = $(this).attr('name');

                                //FowstCount == 1 means it belongs to the first table
                                if (CInt2($(this).attr('fowstcount')) == 1) {

                                    pData.Fowst1ID = $('#txtReceivingTransferFowstID').val();
                                    pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val();


                                } else {
                                    pData.Fowst1ID = $('#txtReceivingTransferFowstVendorID').val();
                                    pData.FowstVendorID = $('#txtReceivingTransferFowstID').val();
                                }

                                //pData.Fowst1ID = $('#txtReceivingTransferFowstID').val();
                                //pData.FowstVendorID = $('#txtReceivingTransferFowstVendorID').val();                                
                                GetReceivingAndTransfersSubReport(pData);
                            })

                pLV.append(pItem);

            }

        }

        function GetReceivingAndTransfersSubReport(pData) {
            $('#dvSummaryFromTo3').show();
            $('#tbFOWSTAndOrganizationSubReportSummary').empty();

            //url: 'Receiving/GetReceivingAndTransfersSubReport',
            AjaxSend({
                url: 'Receiving/GetReceivingAndTransfersDiscrepancyReport',
                data: pData,
                success: function (vData) {
                    $('#divNoPage').hide();
                    $('#divSecond').show();
                    if (vData.Results.Rows.length > 0) {
                        if (vData.Results.Rows.length > 0) {
                            DisplayReceivingAndTransfersSubReport(vData.Results.Rows);
                        }
                    }
                    else {
                        $('#tbFOWSTAndOrganizationSubReportSummary').append('<tr><td colspan="8" style="text-align:center;">No Result(s) Found</td></tr>')
                    }
                }
            })
        }

        function DisplayReceivingAndTransfersSubReport(vRows) {
            var pLV = $('#tbFOWSTAndOrganizationSubReportSummary');
            var prevFOWST = '';
            var FowstCount = 0;
            var TransType = '';
            var TransTypeFilter = '';

            pLV.empty();

            //pItem = $('<tr>')
            //                .append($('<td>')
            //                   .text('Transaction Reference')
            //               )
            //               .append($('<td>')
            //                   .text('Transaction Date')
            //               )
            //               .append($('<td>')
            //                   .text('Stock Transfer')
            //               )
            //                .append($('<td>')
            //                   .text('Stock Receiving')
            //               )
            //pLV.append(pItem);

            var vFrom = '';
            var vTo = '';

            for (var i = 0; i <= vRows.length - 1; i++) {


                if (CBool(vRows[i].TransactionType) == false) {
                    TransType = 'Stock Transfers';
                    vFrom = vRows[i].TransactionFROM;
                    vTo = vRows[i].TransactionTo;
                    //$('#lblReceivingAndTransFersSummaryFrom').text(vRows[i].TransactionTo)
                    //$('#lblReceivingAndTransFersSummaryTo').text(vRows[i].TransactionFROM)
                } else {
                    TransType = 'Stocks Receipts';
                    vFrom = vRows[i].TransactionTo;
                    vTo = vRows[i].TransactionFROM;
                    //$('#lblReceivingAndTransFersSummaryFrom').text(vRows[i].TransactionFROM)
                    //$('#lblReceivingAndTransFersSummaryTo').text(vRows[i].TransactionTo)
                }

                pItem = $('<tr>')
                            .attr('ID', vRows[i].ID)
                            .data(vRows[i])
                             .append($('<td>')
                                .text(TransType)
                            )
                            .append($('<td>')
                                .text((CBool(vRows[i].TransactionType) == false ? 'T-Out #' + vRows[i].ReceivingNo : 'RCV #' + vRows[i].ReceivingNo))
                            )
                            .append($('<td>')
                                .text(vRows[i].DateReceived)
                            )
                            .append($('<td>')
                                .text(vFrom)
                            )
                            .append($('<td>')
                                .text(vTo)
                            )
                            .append($('<td>')
                                .text(vRows[i].IssueRemarks)
                            )
                            .append($('<td>')
                                .text(vRows[i].NetSuiteNum)
                            )
                            .append($('<td>')
                                .text(vRows[i].NetSuiteErrorReponse)
                            )
                            .append($('<td>')
                                .text(vRows[i].ProcessedForNetSuite)
                            )
                            .click(function () {
                                $('#divFortableReceivingAndTransfersDetails').popup('open');
                                $('#lblReceivingAndTransFersSummaryFrom').text($(this).data().TransactionFROM);
                                $('#lblReceivingAndTransFersSummaryTo').text($(this).data().TransactionTo);
                                GetReceivingDetails($(this).attr('ID'));
                            })
                pLV.append(pItem);

            }

            pLV.parent().table('refresh');
        }

        function GetReceivingDetails(ID) {
            data = { ID: ID }

            AjaxSend({
                url: "Receiving/GetAllReceiving",
                data: { ID: ID },
                success: function (vRetDataReceivingInfo) {
                    AjaxSend({
                        url: "Receiving/GetAllReceivingSKU",
                        data: { ID: ID },
                        success: function (vRetDataReceivingDetails1) {

                            $('#divFortableReceivingAndTransfersDetails').popup('open');
                            DisplayReceivingDetails(vRetDataReceivingInfo.Results.Rows, vRetDataReceivingDetails1.Results.Rows, true);

                            if (vRetDataReceivingInfo.Results.Rows[0].TransferOutToReceivingID != undefined) {
                                AjaxSend({
                                    url: "Receiving/GetAllReceivingSKU",
                                    data: { ID: vRetDataReceivingInfo.Results.Rows[0].TransferOutToReceivingID },
                                    success: function (vRetDataReceivingDetails2) {

                                        DisplayReceivingDetails(vRetDataReceivingInfo.Results.Rows, vRetDataReceivingDetails2.Results.Rows, false);
                                        //GeneratePDFReport(vRetDataHeader.Results, vRetDataReceivingInfo.Results, vRetDataReceivingDetails.Results)
                                        $('#tableReceivingAndTransfersDetails2').css('visibility', 'visible');
                                    }
                                });
                            } else {
                                $('#tableReceivingAndTransfersDetails2').css('visibility', 'hidden');
                            }
                        }
                    });
                }
            });
        }

        function DisplayReceivingDetails(vReceivingInfo, vReceivingDetails, IsFirstTable) {

            var pLV;

            if (IsFirstTable) {
                var pLV = $('#tBodyReceivingAndTransfersDetails1');
            } else {
                var pLV = $('#tBodyReceivingAndTransfersDetails2');
            }


            pLV.empty();

            for (var i = 0; i <= vReceivingDetails.length - 1; i++) {

                pItem = $('<tr>')

                             .append($('<td>')
                                .text(vReceivingDetails[i].ProductName)
                            )
                            .append($('<td>')
                                .text(vReceivingDetails[i].Label)
                            )
                            .append($('<td>')
                                .text(vReceivingDetails[i].QuantityGood)
                            )
                pLV.append(pItem);

            }

            pLV.parent().table('refresh');
        }

        function GetAllFOWST() {
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: {IsAll: true},
                success: function (vData) {

                    pSel1 = $('#selFowst1');
                    pSel2 = $('#selFowst2');
                    pSel1.empty();
                    pSel2.empty();

                    pSel1.append(new Option('Please Select Fowst (Required)', ''));
                    pSel2.append(new Option('Please Select Fowst or Vendor (Optional)', ''));

                    var vRows = vData.Results;

                    for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                        pSel1.append(new Option(vRows.Rows[i].FowstName, vRows.Rows[i].ID));
                        pSel2.append(new Option(vRows.Rows[i].FowstName, vRows.Rows[i].ID));
                    }

                    pSel2.append(new Option("Third party Company or Organization", 'thirdparty'))

                    pSel1.selectmenu('refresh', true);
                    pSel2.selectmenu('refresh', true);
                }
            });
        }

        function SearchOrganizationPopUp(s) {
            var pData = {};

            pData.keywords = s;

            AjaxSend({
                url: 'Browser/LibraryOrganization',
                data: pData,
                success: function (vData) {
                    $('#lstReceivingTransferOrganization').empty();

                    if (vData.Results.Rows.length > 0) {
                        for (var p = 0; p < vData.Results.Rows.length; p++) {
                            $('#lstReceivingTransferOrganization').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .attr('title', vData.Results.Rows[p].CompanyName)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#divThirdParty').show();
                                            $('#txtVendorName').text($(this).data('data').CompanyName);
                                            FowstVendorID = $(this).attr('ID');
                                            $('#panReceivingTransferThirdParty').popup('close');
                                            $('#panReceivingTransferFowst').popup('open');
                                        })
                                    ));
                        }
                    }
                    else {
                        $('#lstReceivingTransferOrganization').append($('<li>').text('No Result(s) Found'));
                    }

                    $('#lstReceivingTransferOrganization').listview('refresh');
                }
            });
        }
    })
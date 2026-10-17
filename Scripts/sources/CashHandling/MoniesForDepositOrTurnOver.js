require([
    'common',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    (OfflineMode ? GetPathFormat('/Scripts3/model/CashHandling_offline.js?crc=') : ''),
    '/Scripts/sources/ReportsPOS/SalesReport_offline.js?crc='
    //, 'promise!/Scripts3/NewMessageBox.js?crc='
], function () {

    require([
               '/Scripts/jquery.widget.min.js'
    ],
           function () {
               require([
                       '/Scripts/metro.min.js'
               ],
                   function () {
                       //metro-calendar.js, metro-datepicker.js needs jquery.widget.min.js andmetro.min.js  to load first before it can load itself
                       require([
                               '/Scripts/metro-calendar.js',
                               '/Scripts/metro-datepicker.js'
                       ],
                       function () {
                           var CashHandling = {};
                           var dateFrom;
                           var dateTo;
                           var newdateFrom = new Date();
                           var newdateTo = new Date();
                           var newRangeType = "";

                           CashHandling = new App.model.CashHandling_offline();

                           //if (!Business.SystemDefaults.GetEnforceZRead()) {
                           //    startclock('#clockMonies', 'C');
                           //} else {
                           //    startclock('#clockMonies, #clock', 'Z ');
                           //}

                           $('body')
                            .on('click', '.btnCashierSelectFowst', function () {
                                BrowseFowst();
                           })
                           .on('click', '#btnCashierDateRange', function () {
                               $('#panCashierDate').popup('open');
                           })
                           .on('click', '.btnCashierCancel', function () {
                               $('#panCashierDate').popup('close');
                           })
                            .on('click', '#btnCashierDone', function (e) {
                                dateFrom = new Date($('#txtCashierDateFrom').text());
                                dateTo = new Date($('#txtCashierDateTo').text());
                                var dateNow = new Date();

                                //                                if ((dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) || $('#txtCashierDateFrom').text() == "" || $('#txtCashierDateTo').text() == "") {
                                //                                    $('#spanCashierDateRangeError').show();
                                //                                }
                                //                                else {
                                $('#spanCashierDateRangeError').hide();
                                $('#btnCashierDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text())
                                $('#CashierDateRangeFrom').text($('#txtCashierDateFrom').text());
                                $('#CashierDateRangeTo').text($('#txtCashierDateTo').text());
                                $('#CashierDateRangeSeparator').show();
                                $('.lblReportDate2').text($('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text()) //for Cash Handling Module
                                $('#panCashierCustomDate').popup('close');
                                //dateFrom = $('#txtCashierDateFrom').text();
                                //dateTo = $('#txtCashierDateTo').text();
                                //dateYear = '';
                                //newRangeType = "Day";
                                //$('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');

                                if ($('#lblCashierSelectedFowstGroup').text() != "FOWST Location") {
                                    //IsViewOnly = 1;
                                    HandleEnforceZRead();
                                    GetReport(false);
                                    GetReport(true); // to get the accumulated gross variance
                                }
                                //}
                            })
                            .on('click', '#liYesterday', function () {
                                dateYear = "This year.";
                                newRangeType = "Yesterday";
                                $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                                dateFrom = new Date((newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate() - 1) + "/" + newdateFrom.getFullYear());
                                dateTo = new Date((newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate() - 1) + "/" + newdateTo.getFullYear()); //newdateTo.getMonth() + " " + (newdateTo.getDate() - 1);
                                IsViewOnly = 1;
                                GetReport(false);
                                GetReport(true); // to get the accumulated gross variance
                                $('#CashierDateRangeFrom').text('Yesterday');
                                $('#CashierDateRangeSeparator').hide();
                                $('#CashierDateRangeTo').text('');
                                $('.lblReportDate2').text('Yesterday') //for Cash Handling Module
                                $('#panCashierDate').popup('close');
                            })
                            .on('click', '#liToday', function () {
                                dateYear = "This year.";
                                newRangeType = "Today";
                                $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                                dateFrom = newdateFrom;
                                dateTo = newdateTo;
                                IsViewOnly = 1;
                                GetReport(false);
                                GetReport(true); // to get the accumulated gross variance
                                $('#CashierDateRangeFrom').text('Today');
                                $('#CashierDateRangeSeparator').hide();
                                $('#CashierDateRangeTo').text('');
                                $('.lblReportDate2').text('Today') //for Cash Handling Module
                                $('#panCashierDate').popup('close');
                            })
                            .on('click', '#liCustomPeriod', function () {
                                $('#panCashierDate').popup('close');
                                $('#panCashierCustomDate').popup('open');
                            })
                            .on('click', '#btnCashierCancel', function () {
                                $('#panCashierCustomDate').popup('close');
                            })
                            .on('click', '#btnViewMoreDetails', function () {
                                ViewMoreDetails();
                            })
                            .on('click', '#btnBackMonies', function () {
                                $(window).scrollTop(0);
                                $('#divViewDetails').hide();
                                $('#divMoniesHeader').show();
                            })
                            .on('click', '#btnSalesDetails', function () {
                                $('#divMoniesHeader').hide();
                                $('#divViewDetails, #divInvoiceDetails').hide();
                                $('#divViewSalesDetails, #divMainSalesDetails').show();
                                ViewSalesDetails();
                            })
                            .on('click', '#btnBackSalesDetails', function () {
                                $(window).scrollTop(0);
                                $('#divMoniesHeader').show();
                                $('#divViewSalesDetails').hide();
                            })
                            .on('click', '#btnBackInvoiceDetails', function () {
                                $(window).scrollTop(0);
                                $('#divMainSalesDetails').show();
                                $('#divInvoiceDetails').hide();
                            })
                               .on('click', '#btnPrint', function () {
                                   var intFAFCTotalLenght = 40;
                                   var vDataHeader = SalesReport_offline().getReportHeader(0).Rows[0];
                                   var IsVAT = CBool(Sql.ExecuteScalar('Select IFNULL(IsVAT, 1) from LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID)));

                                   try {
                                       window.pPrint.InitializeBluetoothPrinter();
                                   } catch (e) {
                                       console.log('InitializeBluetoothPrinter: ' + e);
                                   };

                                   NewPrintBuilder();

                                   var strPrint = '';
                                   IminPrintInColumns('', 'Monies Turnover Slip', '', 0, 10, 0, 0, 1, 0, 24, 36, 24);
                                   IminfeedPaper();

                                   //Get Header
                                   IminPrintInColumns('', vDataHeader.OrgBusinessTradeName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                   IminPrintInColumns('', vDataHeader.SubscriberOrganizationName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24); 
                                   IminPrintInColumns('', vDataHeader.OrgBranchName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                   IminPrintInColumns('', (!CBool(IsVAT) ? 'NON-' : '') + 'VAT TIN ' + $.trim((vDataHeader.TIN || '')), '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                   IminPrintInColumns('', vDataHeader.OrgCompleteAddress, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);

                                   IminfeedPaper();

                                   IminPrintInColumns('Turned Over Amt:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);

                                   IminfeedPaper();
                                   IminPrintInColumns('', '***** BREAKDOWN *****', '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 0.05', 'x 10', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 0.10', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 0.25', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 1.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 5.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 10.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 20.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 50.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 100.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 200.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 500.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   if (true) IminPrintInColumns('P 1000.00', 'x 1', '99,999.00', 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                   IminPrintInColumns('Cash:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);
                                   IminPrintInColumns('Check/GC:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);
                                   IminPrintInColumns('Debit Card:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);
                                   IminPrintInColumns('Credit Card:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);
                                   IminPrintInColumns('SRV:', '', '99,999.00', 6, 1, 5, 0, 0, 2, 24, 24, 24);

                                   IminfeedPaper();
                                   IminfeedPaper();
                                   IminfeedPaper();

                                   IminPrintInColumns('__________________________________', '', '', 12, 0, 0, 0, 0, 0, 24, 24, 24);
                                   IminPrintInColumns('From: Arnie Loste Peloton From Davao City, Philippines', '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                                   IminfeedPaper();
                                   IminfeedPaper();
                                   IminfeedPaper();

                                   IminPrintInColumns('__________________________________', '', '', 12, 0, 0, 0, 0, 0, 24, 24, 24);
                                   IminPrintInColumns('To: Juan Dela Cruz', '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                                   IminfeedPaper();
                                   IminfeedPaper();
                                   IminfeedPaper();
                                   IminfeedPaper();
                                   IminfeedPaper();
                               })

                           $(function () {                               

                               $(".CashierfromCalendar").calendar({
                                   format: "mmmm dd, yyyy",
                                   multiSelect: false,
                                   otherDays: true,
                                   getDates: function (data) {
                                       $("#txtCashierDateFrom").text(data);
                                   }
                               });

                               $(".CashiertoCalendar").calendar({
                                   format: "mmmm dd, yyyy",
                                   multiSelect: false,
                                   otherDays: true,
                                   getDates: function (data) {
                                       $("#txtCashierDateTo").text(data);
                                   }
                               });

                               $('#btnDepositMonies').click(function () {
                                   if (IsDate(dateFrom)) {
                                       sessionStorage['DepositDateFrom'] = dateFrom;
                                   } else {
                                       if (newRangeType == 'Today') {
                                           sessionStorage['DepositDateFrom'] = (newdateFrom.getMonth() - 1) + "/" + (newdateFrom.getDate()) + "/" + (newdateFrom.getFullYear());
                                       } else if (newRangeType == 'Yesterday') {
                                           sessionStorage['DepositDateFrom'] = (newdateFrom.getMonth() - 1) + "/" + (newdateFrom.getDate() - 1) + "/" + (newdateFrom.getFullYear());
                                       }
                                   }


                                   window.open('/CashHandling/DepositMonies' + (IsMobile ? "?IsMobile=1" : ""), '_self')
                               });

                               $('#btnPOSSalesReportCancel').click(function (e) {

                                   e.preventDefault();

                                   window.open(GetPathFormat("/People/PeopleMenu") + (IsMobile ? "?IsMobile=1" : ""), "_self");
                               });
                                
                               $('#txtCurrentUserName').text(sessionStorage.CurrentUserFullName);

                               GetAllFOWST();
                           });

                           function ViewSalesDetails() {
                               var tbodySalesDetails = $('#tbodySalesDetails');
                               var vRows = CashHandling.getSalesDetails({ NonZero: true, FOWSTID: sessionStorage.FowstID, dateNow: $.format.date(new Date(proj.GetZReadDateSql().replace(/'/g, '')), 'yyyy-MM-dd') }).Rows;

                               tbodySalesDetails.empty();

                               if (vRows.length > 0) {
                                   for (var i = 0; i < vRows.length; i++) {
                                       tbodySalesDetails.append($('<tr>')
                                        .append($('<td>')
                                            .text(vRows[i].InvoiceNum)
                                        )
                                        .append($('<td>')
                                            .text($.format.date(new Date(vRows[i].InvoiceDate), 'MMM dd, yyyy'))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].CompanyName)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].FOWSTName)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].SkuCount)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].Total)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].EmployeeFullName)
                                        )
                                        .data('data', vRows[i])
                                        .css('cursor', 'pointer')
                                        .click(function (e) {
                                            var vRowsItems = CashHandling.GetInvoiceItems({ InvoiceID: $(this).data('data').ID }).Rows;
                                            DisplayInvoiceItems(vRowsItems);
                                            $('#divMainSalesDetails').hide();
                                            $('#divInvoiceDetails').show();
                                            $(window).scrollTop(0);
                                        })
                                       )
                                   }
                               }
                               else {
                                   tbodySalesDetails.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 8)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                               }

                               tbodySalesDetails.parent();//.table('refresh');
                           }

                           function DisplayInvoiceItems(vRows) {
                               var tbodyInvoiceDetails = $('#tbodyInvoiceDetails');

                               tbodyInvoiceDetails.empty();

                               if (vRows.length > 0) {
                                   for (var i = 0; i < vRows.length; i++) {
                                       $('#txtInvoiceNo').text(vRows[i].InvoiceNo);
                                       tbodyInvoiceDetails.append($('<tr>')
                                        .append($('<td>')
                                            .text(vRows[i].InvoiceNo)
                                        )
                                        .append($('<td>')
                                            .text($.format.date(new Date(vRows[i].InvoiceDate), 'MMM dd, yyyy'))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].ProductDescription)
                                        )
                                        .append($('<td>')
                                            .css('text-align', 'right')
                                            .text(vRows[i].Qty)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].UnitMeasure)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].RebatesProduct)
                                        )
                                        .append($('<td>')
                                            .css('text-align', 'right')
                                            .text((vRows[i].QtyRebates || ''))
                                        )
                                        .append($('<td>')
                                            .text((vRows[i].UnitMeasureRebates || ''))
                                        )
                                        .append($('<td>')
                                            .css('text-align', 'right')
                                            .text(AccountingFormat(vRows[i].Price))
                                        )
                                        .append($('<td>')
                                            .css('text-align', 'right')
                                            .text(AccountingFormat(vRows[i].SubTotal))
                                        )
                                        .append($('<td>')
                                            .css('text-align', 'right')
                                            .text(AccountingFormat(vRows[i].Discount))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].Customer)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].UpdatedByName)
                                        )
                                       )
                                   }
                               }
                               else {
                                   tbodyInvoiceDetails.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 15)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                               }

                               tbodyInvoiceDetails.parent();//.table('refresh');
                           }

                           function ViewMoreDetails() {
                               var pData = {}, mData;

                               pData.DateFrom = new Date('JAN 1, 1900'); ;
                               pData.DateTo = proj.GetZReadDateSql().replace(/'/g, '');
                               pData.PeopleID = Web.PeopleUserID();
                               pData.FowstID = sessionStorage.FowstID;

                               mData = CashHandling.GetAllDetails(pData, 0);

                               DisplayAllDetails(mData[0], mData[1].Rows, mData[2].Rows);
                               DisplayTurnover(mData[1].Rows);
                               DisplayDeposit(mData[2].Rows);

                               $('#divMoniesHeader').hide();
                               $('#divViewSalesDetails').hide();
                               $('#divViewDetails').show();
                               $(window).scrollTop(0);
                           }

                           function DisplayTurnover(vRows) {
                               var pLV = $('tbody[id="tBodyTurnOverDetails"]');

                               pLV.empty();

                               if (vRows.length > 0) {
                                   for (var i = 0; i < vRows.length; i++) {
                                       pLV.append($('<tr>')
                                                .append($('<td>')
                                                    .text($.format.date(new Date(vRows[i].DateToTurnover), 'MMM dd, yyyy HH:mm'))
                                                )
                                                .append($('<td>')
                                                    .text(vRows[i].TurnoverBy)
                                                )
                                                .append($('<td>')
                                                    .text(vRows[i].TurnoverTo)
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(vRows[i].AmountTurnover))
                                                )
                                            )
                                   }
                               }
                               else {
                                   pLV.append($('<tr>')
                                            .append($('<td>')
                                                .text('No Result(s) Found')
                                                .css('text-align', 'center')
                                                .attr('colspan', 10)
                                            )
                                        )
                               }

                               pLV.parent();//.table('refresh');
                           }

                           function DisplayAllDetails(vRows, vRowsTurnOver, vRowsDeposits) {
                               var pLV = $('#tBodyAllDetails');
                               var lv = $('tbody[id="tbodyDetails"]');

                               pLV.empty();
                               lv.empty();

                               if (vRows.length > 0) {
                                   $('#txtPersonName').text(vRows[0].Cashier);
                                   for (var i = 0; i < vRows.length; i++) {

                                       var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRows[i].SalesTransactionDate), 'MMM dd, yyyy') + '"]').index();
                                       var thisRow = lv.find('tr:eq(' + rowInd + ')');

                                       //if (vRows[i].GeneratedSales != undefined) {
                                       var TotalGeneratedSalesAndAcceptedMonies = CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies);
                                       var TotalDisbursedAndTuronedOverMonies = 0 + CDec2(vRows[i].TurnedOverMonies);
                                       var TotalMoniesRemainingOnHand = (CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies)) - (0 + CDec2(vRows[i].TurnedOverMonies));
                                       var isTurnOver = '-';

                                       if (vRows[i].IsTurnOver != undefined) {
                                           isTurnOver = (vRows[i].IsTurnOver == 1 ? "Turnover" : "Deposit");
                                       }

                                       if (lv.find('tr[pplid="' + vRows[i].PeopleID + '"]').length > 0 && $.format.date(new Date(thisRow.find('td:eq(0)').text()), 'MMM dd, yyyy') == $.format.date(new Date(vRows[i].SalesTransactionDate), 'MMM dd, yyyy')) {
                                           //thisRow.find('td:eq(3)').text(AccountingFormat(CDec2(thisRow.find('td:eq(3)').text()) + CDec2(vRows[i].AcceptedMonies)))
                                           thisRow.find('td:eq(0)').text(vRows[i].SalesTransactionDate)
                                           //thisRow.find('td:eq(8)').text(AccountingFormat((CDec2(thisRow.find('td:eq(5)').text()) + CDec2(thisRow.find('td:eq(6)').text())) - CDec2(thisRow.find('td:eq(5)').text())))
                                       }
                                       else {
                                           var mDate;
                                           if (IsDate(vRows[i].SalesTransactionDate)) {
                                               mDate = vRows[i].SalesTransactionDate;
                                           } else {
                                               mDate = vRows[i].DepositDate;
                                           }
                                           lv.append($('<tr>')
                                                .attr('pplid', vRows[i].PeopleID)
                                                .attr('salestract', $.format.date(new Date(mDate), 'MMM dd, yyyy'))
                                                .append($('<td>')
                                                    .css('border-left', '1px solid #757575')
                                                    .text($.format.date(new Date(mDate), 'MMM dd, yyyy'))
                                                )
                                                .append($('<td>')
                                                    .css('display', 'none')
                                                    .text(vRows[i].Cashier)
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .css('border-left', '1px solid #757575')
                                                    .text(AccountingFormat(CDec2(vRows[i].GeneratedSales)))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(CDec2(vRows[i].DiscountAmount)))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(CDec2(vRows[i].VatExemptTax)))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(CDec2(vRows[i].TotalReturns)))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(CDec2(vRows[i].GeneratedSales) - (CDec2(vRows[i].DiscountAmount) + CDec2(vRows[i].VatExemptTax) + CDec2(vRows[i].TotalReturns))))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat(vRows[i].AcceptedMonies))
                                                )
                                                .append($('<td>')
                                                    .text(vRows[i].AcceptedMoniesFrom)
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text(AccountingFormat((CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies)) - CDec2(vRows[i].DiscountAmount) - CDec2(vRows[i].VatExemptTax) - CDec2((vRows[i].TotalReturns))))
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .css('border-left', '1px solid #757575')
                                                    .text('-')
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text('-')
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text('-')
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text('-')
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text('-')
                                                )
                                                .append($('<td>')
                                                    .attr('pplid', vRows[i].PeopleID)
                                                    .data('data', vRows[i])
                                                    .text('')
                                                    .click(function (e) {
                                                        e.stopPropagation();

                                                        if (CBool($(this).attr('ismulti')) == false) {
                                                            return false;
                                                        }

                                                        var pData = {};
                                                        var data = $(this).data('data');
                                                        pData.FowstID = sessionStorage.SelectedFOWST;
                                                        pData.PeopleID = $(this).attr('pplid');

                                                        var vData = CashHandling.GetTurnover(pData, new Date(data.DepositDate), new Date(data.DepositDate));
                                                        var details = '';

                                                        for (var i = 0; i < vData.Rows.length; i++) {
                                                            if (i == 0) {
                                                                details = '<div style="float:left">' + vData.Rows[i].TurnoverTo + '</div> <div style="float:right">' + AccountingFormat(vData.Rows[i].AmountTurnover) + '</div> <div style="clear:both"></div>';
                                                            }
                                                            else {
                                                                details += '<br> <div style="float:left">' + vData.Rows[i].TurnoverTo + '</div> <div style="float:right">' + AccountingFormat(vData.Rows[i].AmountTurnover) + '</div> <div style="clear:both"></div>';
                                                            }
                                                        }

                                                        NewMessageBox(details, 'Turnedover Details');
                                                    })
                                                )
                                                .append($('<td>')
                                                    .css('text-align', 'right')
                                                    .text('-')
                                                )
                                            )
                                       }
                                       //}
                                   }
                               }
                               else {
                                   lv.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 16)
                                   //.css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                   )
                               }

                               if (vRowsDeposits.length > 0) {
                                   for (var d = 0; d < vRowsDeposits.length; d++) {
                                       var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRowsDeposits[d].DepositDate), 'MMM dd, yyyy') + '"]').index();
                                       var thisRow = lv.find('tr:eq(' + rowInd + ')');

                                       thisRow.find('td:eq(10)').text(AccountingFormat(CDec2(thisRow.find('td:eq(10)').text()) + CDec2(vRowsDeposits[d].AmountDeposit)));
                                       thisRow.find('td:eq(11)').text(AccountingFormat(CDec2(thisRow.find('td:eq(11)').text()) + CDec2(vRowsDeposits[d].ServiceCharge)));
                                       thisRow.find('td:eq(12)').text(AccountingFormat(CDec2(thisRow.find('td:eq(10)').text()) + CDec2(thisRow.find('td:eq(11)').text())));
                                   }
                               }

                               if (vRowsTurnOver.length > 0) {
                                   var isMultiLoadFinished = false;
                                   for (var t = 0; t < vRowsTurnOver.length; t++) {
                                       var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRowsTurnOver[t].DateToTurnover), 'MMM dd, yyyy') + '"]').index();
                                       var thisRow = lv.find('tr:eq(' + rowInd + ')');
                                       var isMultipleTurnedover = false;

                                       if (thisRow.find('td:eq(15)').text() != '') {
                                           if (vRowsTurnOver[t].TurnoverTo != thisRow.find('td:eq(15)').text()) {
                                               isMultipleTurnedover = true;
                                           }
                                       }                                       

                                       thisRow.find('td:eq(14)').css('border-left', '1px solid #757575').text(AccountingFormat(CDec2(thisRow.find('td:eq(14)').text()) + CDec2(vRowsTurnOver[t].AmountTurnover)));
                                       //if (isMultiLoadFinished == false) {
                                           thisRow.find('td:eq(15)')
                                            .attr('ismulti', isMultipleTurnedover)
                                            .html((isMultipleTurnedover == true ? 'Multiple Person <br> <a class="custom-link" data-role="none">View Details</a>' : vRowsTurnOver[t].TurnoverTo));
                                       //}
                                       thisRow.find('td:eq(13)').text(AccountingFormat(CDec2(thisRow.find('td:eq(9)').text()) - CDec2(thisRow.find('td:eq(12)').text())));
                                       thisRow.find('td:eq(16)').css('border-left', '1px solid #757575').text(AccountingFormat(CDec2(thisRow.find('td:eq(14)').text()) - CDec2(thisRow.find('td:eq(13)').text())));

                                       isMultiLoadFinished = true;
                                   }

                                   $('#tbodyDetails tr').each(function (i) {

                                       $(this).find('td:eq(13)').text(AccountingFormat(CDec2($(this).find('td:eq(9)').find('b').remove('b').end().text()) - CDec2($(this).find('td:eq(12)').text())));

                                       if ($(this).find('td:eq(13)').text() == '-' && $(this).find('td:eq(14)').text() == '-' && $(this).find('td:eq(12)').text() == '-') {
                                           //if with sales with no turnover
                                           $(this).find('td:eq(16)').text($(this).find('td:eq(9)').find('b').remove('b').end().text());
                                       } else {
                                           //if with sales and turnover
                                           $(this).find('td:eq(16)').text(AccountingFormat(CDec2($(this).find('td:eq(14)').text()) - CDec2($(this).find('td:eq(13)').text())));
                                       }
                                   })
                               }
                               else {
                                   $('#tbodyDetails tr').each(function (i) {

                                       $(this).find('td:eq(13)').text(AccountingFormat(CDec2($(this).find('td:eq(9)').find('b').remove('b').end().text()) - CDec2($(this).find('td:eq(12)').text())));

                                       if ($(this).find('td:eq(13)').text() == '-' && $(this).find('td:eq(13)').text() == '-' && $(this).find('td:eq(12)').text() == '-') {
                                           //if with sales with no turnover
                                           $(this).find('td:eq(16)').text($(this).find('td:eq(9)').find('b').remove('b').end().text());
                                       } else {
                                           //if with sales and turnover
                                           $(this).find('td:eq(16)').text(AccountingFormat(CDec2($(this).find('td:eq(14)').text()) - CDec2($(this).find('td:eq(13)').text())));
                                       }
                                   })
                               }

                               lv.parent();//.table('refresh');
                           }

                           function DisplayDeposit(vRows) {
                               var pLV = $('tbody[id="tBodyDepositDetails"]');

                               pLV.empty();

                               if (vRows.length > 0) {
                                   for (var i = 0; i < vRows.length; i++) {
                                       pLV.append($('<tr>')
                                            .append($('<td>')
                                                .text($.format.date(new Date(vRows[i].DepositDate), 'MMM dd, yyyy HH:mm'))
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].DepositedBy)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].DepositedTo)
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].AmountDeposit))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(CDec2(vRows[i].ServiceCharge)))
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].DepositReferenceNo)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].BankName)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].BankBranchName)
                                            )
                                        )
                                   }
                               }
                               else {
                                   pLV.append($('<tr>')
                                        .append($('<td>')
                                            .text('No Result(s) Found')
                                            .css('text-align', 'center')
                                            .attr('colspan', 10)
                                        )
                                    )
                               }

                               pLV.parent();//.table('refresh');
                           }

                           function GetAllFOWST() {


                               if (OfflineMode) {
                                   if (Web.HandleOrganization() == false)
                                       return;

                                   var vData = OfflineBrowser.LibraryFowst.GetData({
                                       FilterFowst: 1
                                   });

                                   ShowFOWST(vData);

                               } else {

                                   AjaxSend({
                                       url: 'Browser/LibraryFowst',
                                       data: { FilterFowst: 1 },
                                       success: function (vData) {

                                           ShowFOWST(vData);
                                       }
                                   });
                               }

                               function ShowFOWST(vData) {


                                   pLV = $('#listCashierFowst');
                                   pLV.empty();

                                   var vRows;

                                   if (OfflineMode)
                                       vRows = vData;
                                   else
                                       vRows = vData.Results;

                                   countFowst = vRows.Rows.length;

                                   if (vRows.Rows.length == 1) {

                                       if (vRows.Rows[0].IsFOWSTInsideAMall == '1' && vRows.Rows[0].FOWSTMallName == 'SM Mall') {
                                           IsFowstInSMMall = true;
                                       } else {
                                           IsFowstInSMMall = false;
                                       }

                                       sessionStorage.FowstID = vRows.Rows[0].ID;
                                       sessionStorage.SelectedFOWST = vRows.Rows[0].ID;

                                       dateFrom = newdateFrom;
                                       dateTo = newdateTo;

                                       GetReport(false);
                                       GetReport(true); // to get the accumulated gross variance
                                       $('.lblCashierSelectedFowstGroup').text(vRows.Rows[0].FowstName);
                                       //$('#panCashierFowst').popup('close');
                                       //$('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                       //dateYear = "This year.";
                                       //newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                                       //$('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                                       //IsViewOnly = 1;
                                       //DoPrintSalesReport();
                                       HandleEnforceZRead();

                                   }
                                   else {
                                       BrowseFowst();
                                   }
                               }
                           }

                           function BrowseFowst() {
                               var clr = setInterval(function () {
                                   if (typeof BrowsePopup != 'undefined') {
                                       BrowsePopup('LibraryFowst', 'Select FOWST', [
                                   {
                                       DisplayText: [{ Column: 'FowstName' }],
                                       Offline: true,
                                       AutoSearch: true,
                                       Click: function (e) {
                                           sessionStorage.FowstID = e.ID;
                                           sessionStorage.SelectedFOWST = e.ID;

                                           HandleEnforceZRead();
                                           dateFrom = proj.GetZReadDateSql().replace(/'/g, '')
                                           dateTo = proj.GetZReadDateSql().replace(/'/g, '')
                                           GetReport(false);
                                           GetReport(true); // to get the accumulated gross variance
                                           $('.lblCashierSelectedFowstGroup').text(e.FowstName);
                                       }
                                   }
                                       ], { FilterFowst: 1 });
                                       clearInterval(clr);
                                   }
                               }, 100)                               
                           }

                           function GetReport(isAccumulated) {
                               var mData;
                               var pData = {};
                               var vRowsAll, vRowsTurned, vRowsDeposits;
                               var GrossSales = 0, SalesDiscounts = 0, VatExemptTax = 0, SalesReturns = 0, NetSales = 0,
                                    MoniesReceived = 0, MoniesReceivedFrom = '', TotalNetSalesAndMoniesReceived = 0,
                                    DepositsToBank3rd = 0, ServiceCharges = 0, TotalDepositsServiceCharge = 0,
                                    TotalAvailableMoniesAfter = 0, MoniesTurnedOver = 0,
                                    TurnedOverT0 = '', GrossVariance = 0;


                               //NEW
                               if (isAccumulated) {
                                   pData.DateFrom = new Date('JAN 1, 1900');
                                   //pData.DateTo = new Date();
                                   pData.DateTo = proj.GetZReadDateSql().replace(/'/g, '')
                               }
                               else {
                                   pData.DateFrom = dateFrom;
                                   //pData.DateTo = dateTo;
                                   pData.DateTo = proj.GetZReadDateSql().replace(/'/g, '')
                               }

                               pData.PeopleID = Web.PeopleUserID();
                               pData.FowstID = sessionStorage.FowstID;

                               mData = CashHandling.GetAllDetails(pData, 0);

                               vRowsAll = mData[0];
                               vRowsTurned = mData[1].Rows;
                               vRowsDeposits = mData[2].Rows;

                               for (var i = 0; i < vRowsAll.length; i++) {
                                   GrossSales = GrossSales + CDec2(vRowsAll[i].GeneratedSales);
                                   SalesDiscounts = SalesDiscounts + CDec2(vRowsAll[i].DiscountAmount);
                                   VatExemptTax = VatExemptTax + CDec2(vRowsAll[i].VatExemptTax);
                                   SalesReturns = SalesReturns + CDec2(vRowsAll[i].TotalReturns);
                                   MoniesReceived = MoniesReceived + CDec2(vRowsAll[i].AcceptedMonies);
                               }

                               NetSales = GrossSales - SalesDiscounts - VatExemptTax - SalesReturns;
                               //TotalNetSalesAndMoniesReceived = (GrossSales + MoniesReceived) - SalesDiscounts - VatExemptTax - SalesReturns;
                               TotalNetSalesAndMoniesReceived = (GrossSales) - SalesDiscounts - VatExemptTax - SalesReturns;

                               for (var m = 0; m < vRowsDeposits.length; m++) {
                                   DepositsToBank3rd = DepositsToBank3rd + CDec2(vRowsDeposits[m].AmountDeposit);
                                   ServiceCharges = ServiceCharges + CDec2(vRowsDeposits[m].ServiceCharge);
                               }

                               TotalDepositsServiceCharge = DepositsToBank3rd + ServiceCharges;
                               TotalAvailableMoniesAfter = TotalNetSalesAndMoniesReceived - TotalDepositsServiceCharge;

                               for (var n = 0; n < vRowsTurned.length; n++) {
                                   MoniesTurnedOver = MoniesTurnedOver + CDec2(vRowsTurned[n].AmountTurnover);
                               }

                               GrossVariance = MoniesTurnedOver - TotalAvailableMoniesAfter;

                               if (isAccumulated) {
                                   //$('#txtAccumulatedGrossVariance').text(AccountingFormat(GrossVariance));
                                   $('#txtAccumulatedGrossVariance').text(AccountingFormat(0));
                               }
                               else {
                                   $('#txtGrossSales').text(AccountingFormat(GrossSales));
                                   $('#txtSalesDiscounts').text(AccountingFormat(SalesDiscounts));
                                   $('#txtVatExemptTax').text(AccountingFormat(VatExemptTax));
                                   $('#txtSalesReturns').text(AccountingFormat(SalesReturns));
                                   $('#txtNetSales').text(AccountingFormat(NetSales));
                                   $('#txtMoniesReceived').text(AccountingFormat(MoniesReceived));
                                   $('#txtTotalNetSalesMoniesReceived').text(AccountingFormat(TotalNetSalesAndMoniesReceived));

                                   $('#txtDepositsToBank3rd').text(AccountingFormat(DepositsToBank3rd));
                                   $('#txtServiceCharge').text(AccountingFormat(ServiceCharges));
                                   $('#txtTotalDepositsAndServiceCharge').text(AccountingFormat(TotalDepositsServiceCharge));

                                   $('#txtTotalAvailableMoniesAfter').text(AccountingFormat(TotalAvailableMoniesAfter));

                                   $('#txtTurnedOverToCoEmployees').text(AccountingFormat(MoniesTurnedOver));
                                   $('#txtGrossVariance').text(AccountingFormat(GrossVariance));
                               }

                               return;
                               //OLD

                               dateFrom = newdateFrom;
                               dateTo = newdateTo;
                               IsViewOnly = 1;

                               pData.From = dateFrom;
                               pData.To = dateTo;
                               pData.FowstID = sessionStorage.FowstID;

                               mData = CashHandling.getUndepositedMonies(pData);

                               vRows = mData.Rows;

                               if (mData.Rows.length > 0) {
                                   $('#lblTotalUndepositedAmountofMoniesFromSales').text(AccountingFormat(CDec2(vRows[0].GrossSales) - CDec2(vRows[0].TotalDiscounts) - CDec2(vRows[0].TotalReturns)));
                                   $('#lblTotalUndepositedAmountofMoniesFromCoEmployees').text(AccountingFormat(vRows[0].AcceptedMonies));
                                   $('#lblTotalUndepositedAmountofMoniesFromSalesAndAcceptedMonies').text(AccountingFormat(CDec2(vRows[0].GrossSales) - CDec2(vRows[0].TotalDiscounts) - CDec2(vRows[0].TotalReturns) + CDec2(vRows[0].AcceptedMonies)));

                                   $('#lblGrossSales').text(AccountingFormat(vRows[0].GrossSales));
                                   $('#lblReturns').text(AccountingFormat(vRows[0].TotalReturns));
                                   $('#lblDiscounts').text(AccountingFormat(vRows[0].TotalDiscounts));
                                   $('#lblNetSales').text(AccountingFormat(CDec2(vRows[0].GrossSales) - CDec2(vRows[0].TotalDiscounts) - CDec2(vRows[0].TotalReturns)));
                               }

                               mData = CashHandling.getBreakDownofMoniesAccepted(pData);

                               var pLV = $('#tbBreakDownofMoniesAccepted');

                               pLV.empty();

                               if (mData.Rows.length > 0) {

                                   vRows = mData.Rows;

                                   for (var i = 0; i <= vRows.length - 1; i++) {

                                       pItem = $('<tr>')
                                               .attr('id', vRows[i].ID)
                                               .append($('<td>')
                                                   .html('<br />' + vRows[i].DepositedByName)
                                                   )
                                               .append($('<td>')
                                                   .html('<br />' + $.format.date(new Date(vRows[i].DepositDate), 'HH:mm'))
                                                   )
                                               .append($('<td style="text-align:right">')
                                                   .html('<br />' + AccountingFormat(vRows[i].TotalAmount))
                                                   )

                                       pLV.append(pItem);
                                   }
                               }
                           }

                           function HandleEnforceZRead() {

                               if (!Business.SystemDefaults.GetEnforceZRead()) {
                                   // orginal startclock('#clockHotButtons, #clock', '');
                                   startclock('#clockMonies, #clock', 'C ');
                                   return;
                               } else {
                                   startclock('#clockMonies, #clock', 'Z ');
                               }                               

                               var pZReadDate = GetZReadDay();
                               proj.SetZReadDate(pZReadDate);

                               var pToday = RemoveTime(new Date());
                               var pStr;

                               if (+pZReadDate != +pToday) {
                                   // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                   //if (CBool(vParams.silent)) {
                                   //    proj.SetZReadDate(pZReadDate);
                                   //    // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                   //    pCallback();
                                   //} else {
                                   pStr = 'Your device date is ' + $.format.date(pToday, 'MMM d, yyyy') + ' but the Z-Read date is ' + $.format.date(pZReadDate, 'MMM d, yyyy');
                                   NewMessageBox(pStr, 'Closing Date', [
                                           {
                                               Text: 'Close the current Z-Read Date and move date forward by 1 day',
                                               Click: function () {
                                                   NewMessageBox('You will no longer be able to post sales and other transactions to the current Z-Read Date after closing.', '', [
                                                       {
                                                           Text: 'Yes',
                                                           Click: function () {
                                                               IncrementZReadDay();
                                                               HandleEnforceZRead();
                                                               GetReport(false);
                                                               GetReport(true); // to get the accumulated gross variance
                                                           }
                                                       },
                                                       {
                                                           Text: 'No',
                                                           Click: function () {
                                                               HandleEnforceZRead();
                                                               GetReport(false);
                                                               GetReport(true); // to get the accumulated gross variance
                                                           }
                                                       }
                                                   ]);

                                               }
                                           },
                                           {
                                               Text: 'Use ' + $.format.date(pZReadDate, 'MMM d, yyyy') + ' as the current date',
                                               Click: function () {
                                                   proj.SetZReadDate(pZReadDate);
                                                   // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                                   //pCallback();
                                               }
                                           }
                                       ]);
                                   //} // END if (CBool(vParams.silent)) {
                               }
                           }; // END HandleEnforceZRead

                           function GetZReadDay() {
                               if (!IsGuid(sessionStorage.OrgID)) {
                                   throw 'sessionStorage.OrgID must be a valid GUID before calling GetZReadDay. jsWZ3WVGHB7Y!';
                               }

                               if (!IsGuid(sessionStorage.SelectedFOWST)) {
                                   throw 'sessionStorage.FowstID must be a valid GUID before calling GetZReadDay. jsFXQVFSTGHL!';
                               }

                               var pData = LoadZReadDay();
                               try {
                                   pData = JSON.parse(pData);
                               } catch (e) {
                                   pData = {};
                               }
                               var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
                               if (IsDate(pDate)) {

                               } else {
                                   pDate = Android.AppInstallDate();
                               }
                               pDate = RemoveTime(pDate);

                               return pDate;
                           };

                           function LoadZReadDay() {
                               return Transfers.ReadFile('ZReadDay.txt');
                           };

                           function IncrementZReadDay() {
                               var pDate = GetZReadDay();
                               pDate.setDate(pDate.getDate() + 1);
                               pDate = FormatDate(pDate);
                               var pData = LoadZReadDay();
                               try {
                                   pData = JSON.parse(pData);
                               } catch (e) {
                                   pData = {};
                               }
                               var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
                               CreateStringObject(pPath, pData, pDate);
                               SaveZReadDay(pData);
                           };

                           function SaveZReadDay(vData) {
                               Transfers.WriteFile('ZReadDay.txt', null, JSON.stringify(vData, null, 4));
                           };
                       })
                   });
           });
});
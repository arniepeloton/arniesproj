define(['common'
], function () {

    CreateStringObject('App.model.CashHandling_offline', null, function () {

        var pOut = {};
        var mDateFrom;
        var mDateTo;

        pOut.GetAllDetails = function (pData, isHeaderDetails) {
            var pSql = '';
            //var pFinder = new Finder();
            //var pFields = pFinder.Fields;
            //var pField;
            var pDT;
            mDateFrom = new Date(pData.DateFrom);
            mDateTo = new Date(pData.DateTo);

            //pField = pFields.Add('ID');
            //pField.Expression = 'private.TransCashHandling.ID';
            //pField.Searchable = false;

            //pField = pFields.Add('FOWSTID');
            //pField.Searchable=false

            //pField = pFields.Add('FOWST');
            //pField.Expression = 'LibraryOrganizationFOWST.Name';

            //pField = pFields.Add('IsFromTotalAmount');
            //pField = pFields.Add('DepositDate');

            //pField = pFields.Add('DepositedTo');
            //pField.Expression = '(Select DepositedToByName || \' of \' || CompanyName ' +
            //                    ' from LibraryOrganization inner join TransCashHandling on LibraryOrganization.ID = TransCashHandling.DepositedTo  ' +
            //                    ' where LibraryOrganization.ID = TransCashHandling.DepositedTo ' +
            //                    ' UNION ' +
            //                    ' Select Surname || \', \' || FirstName || \' of \' || LibraryOrganizationFOWST.Name ' +
            //                    ' from LibraryPeople inner join TransCashHandling on LibraryPeople.ID = TransCashHandling.DepositedTo  ' +
            //                    ' where LibraryPeople.ID = TransCashHandling.DepositedTo)';

            //pField = pFields.Add('DepositedToByName');
            //pField = pFields.Add('DepositedBy');
            //pField = pFields.Add('DepositedByName');

            //pField = pFields.Add('TotalAmount');
            //pField = pFields.Add('TotalCash');
            //pField = pFields.Add('TotalCheck');
            //pField = pFields.Add('TotalCreditCard');
            //pField = pFields.Add('TotalDebitCard');
            //pField = pFields.Add('TotalSRV');
            //pField = pFields.Add('TotalPettyCash');

            //pField = pFields.Add('NoOfFiveCents');
            //pField = pFields.Add('NoOfTenCents');
            //pField = pFields.Add('NoOfTwentyFiveCents');
            //pField = pFields.Add('NoOfOnePeso');
            //pField = pFields.Add('NoOfFivePesos');
            //pField = pFields.Add('NoOfTenPesos');
            //pField = pFields.Add('NoOfTwentyPesos');
            //pField = pFields.Add('NoOfFiftyPesos');
            //pField = pFields.Add('NoOfOneHundredPesos');
            //pField = pFields.Add('NoOfTwoHundredPesos');
            //pField = pFields.Add('NoOfFiveHundredPesos');
            //pField = pFields.Add('NoOfOneThousandPesos');

            //pFinder.SqlFrom = "FROM private.TransCashHandling INNER JOIN " +
            //            "LibraryOrganizationFOWST on LibraryOrganizationFOWST.ID = private.TransCashHandling.FOWSTID INNER JOIN " +
            //            "LibraryPeople on LibraryPeople.ID = TransCashHandling.DepositedBy " 

            //pFinder.SqlWhere = "WHERE date(DepositDate) BETWEEN " + ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //            " AND " + ReadyString(mDateTo.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateTo.getDate().toString()) +
            //            " AND FOWSTID = " + ReadyString(pData.FowstID)

            //pSql = 'Select  ' +
            //        'DepositedBy as PeopleID,  ' +
            //        ' IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier,  ' +
            //        'SUM(TransInvoiceItems.SubTotal) GeneratedSales,  ' +
            //        'TotalAcceptedFromPrevTurnOver as AcceptedMonies,  ' +
            //        'TransCashHandling.ID ID,  ' +
            //        'DepositDate,   ' +
            //        'ppl2.SurName || \', \' || ppl2.FirstName TurnedOverTo,  ' +
            //        'TransCashHandling.TotalAmount TurnedOverMonies, ' +
            //        'TransInvoice.InvoiceDate as SalesTransactionDate ' +
            //        'from TransInvoice   ' +
            //        'inner join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID  ' +
            //        'inner join  LibraryPeople ppl1 on ppl1.ID = TransInvoice.UserID ' +
            //        'left join TransCashHandling on TransCashHandling.DepositedBy = TransInvoice.UserID  AND date(TransInvoice.InvoiceDate) = date(TransCashHandling.DepositDate)  ' +
            //        'left join LibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo ' +
            //        'WHERE date(InvoiceDate) BETWEEN ' + ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //        ' AND ' + ReadyString(mDateTo.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateTo.getDate().toString()) +
            //        ' AND TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) +
            //        ' Group By TransInvoice.UserID ' +
            //        'union  ' +
            //        'select ' +
            //        'DepositedBy as PeopleID,  ' +
            //        'DepositedByName Cashier,  ' +
            //        'SUM(TransInvoiceItems.SubTotal) GeneratedSales,  ' +
            //        'TotalAcceptedFromPrevTurnOver AcceptedMonies,  ' +
            //        'TransCashHandling.ID ID,  ' +
            //        'DepositDate, ' +
            //        'ppl1.SurName || \', \' || ppl1.FirstName TurnedOverTo,  ' +
            //        'TransCashHandling.TotalAmount TurnedOverMonies, ' +
            //        'TransInvoice.InvoiceDate as SalesTransactionDate ' +
            //        'from TransCashHandling ' +
            //        'left join TransInvoice on TransCashHandling.DepositedBy = TransInvoice.UserID  ' +
            //        'AND date(TransInvoice.InvoiceDate) = date(TransCashHandling.DepositDate)  ' +
            //        'left join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID ' +
            //        'left join LibraryPeople ppl1 on ppl1.ID = TransCashHandling.depositedTo ' +
            //        'WHERE date(InvoiceDate) BETWEEN ' + ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //        ' AND ' + ReadyString(mDateTo.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateTo.getDate().toString()) +
            //        ' AND TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) +
            //        'union  ' +
            //        'Select DepositedBy as PeopleID,  ' +
            //        'DepositedByName Cashier,  ' +
            //        '0 GeneratedSales,  ' +
            //        'TotalAcceptedFromPrevTurnOver AcceptedMonies,  ' +
            //        'TransCashHandling.ID, ' +
            //        'DepositDate, ' +
            //        'org1.CompanyName TurnedOverTo ' +
            //        ',TransCashHandling.TotalAmount TurnedOverMonies, ' +
            //        '\'\' as SalesTransactionDate ' +
            //        'from TransCashHandling ' +
            //        'inner join LibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo ' +
            //         'WHERE date(DepositDate) BETWEEN ' + ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //         ' AND ' + ReadyString(mDateTo.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateTo.getDate().toString()) +
            //         ' AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID)

            //2015-08-25
            //pSql = 'Select IFNULL(DepositedBy, TransInvoice.UserID) as PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier,  ' +
            //        'SUM(TransInvoiceItems.SubTotal) GeneratedSales, ' +
            //        'SUM(TransInvoiceItems.DiscountAmount) as DiscountAmount, ' +
            //        '(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger where UserID = DepositedBy AND FOWSTID = ' + ReadyString(pData.FowstID) +
            //        ' AND date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) +
            //        ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') TotalReturns,' +
            //        '(SELECT SUM(TransCashHandling.TotalAmount) FROM TransCashHandling WHERE TransCashHandling.DepositedTo = TransInvoice.UserID AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as AcceptedMonies, ' +
            //        '(SELECT DepositedByName FROM TransCashHandling WHERE TransCashHandling.DepositedTo = TransInvoice.UserID AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as AcceptedMoniesFrom, ' +
            //        'TransCashHandling.ID ID,  DepositDate,   IFNULL(org1.CompanyName,ppl2.SurName || \', \' || ppl2.FirstName) TurnedOverTo,  ' +
            //        'TransCashHandling.TotalAmount TurnedOverMonies, TransInvoice.InvoiceDate as SalesTransactionDate, TransCashHandling.BankBranch, TransCashHandling.ServiceCharge, TransCashHandling.DepositReferenceNo, TransCashHandling.IsTurnOver, TransCashHandling.DepositedToByName ' +
            //        'from TransInvoice ' +
            //        'inner join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID   ' +
            //        'inner join  LibraryPeople ppl1 on ppl1.ID = TransInvoice.UserID  ' +
            //        'left join TransCashHandling on TransCashHandling.DepositedBy = TransInvoice.UserID   ' +
            //        'AND date(TransInvoice.InvoiceDate) = date(TransCashHandling.DepositDate)   ' +
            //        'left join LibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo  ' +
            //        'left join LibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo ' +
            //        'WHERE (TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) +
            //        ' OR TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) + ') ' +
            //        ' AND (date(DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + //ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //        ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + //ReadyString(mDateTo.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateTo.getDate().toString()) +
            //        ' OR date(TransInvoice.InvoiceDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + //ReadyString(mDateFrom.getFullYear().toString() + '-' + (((mDateFrom.getMonth() + 1) < 10) ? ("0" + (mDateFrom.getMonth() + 1)) : (mDateFrom.getMonth() + 1)).toString() + '-' + mDateFrom.getDate().toString()) +
            //        ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ')'

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ' AND (DepositedBy = ' + ReadyString(pData.PeopleID) + ' OR TransInvoice.UserID = ' + ReadyString(pData.PeopleID) + ')'
            //}
            //pSql += ' Group By TransInvoice.UserID, TransCashHandling.IsTurnOver, DepositDate '


            pSql = 'select TransInvoice.Userid PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransInvoice.ID ID, SUM(SubTotal) GeneratedSales, \n' +
            'SUM(TransInvoiceItems.DiscountAmount) as DiscountAmount,  \n' +
            '(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger  \n' +
            'where TransInventoryLedger.UserID = IFNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date \n' +
            '(TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ' \n' +
            ' AND TransInventoryLedger.TransactionType = "Sales Returns" AND TransInventoryLedger.SubtransactionType IS NULL AND TransInventoryLedger.ParentID IS NULL AND TransInventoryLedger.BinID = "10d4672a-0358-4e2d-9471-2ee42e464300" \n'

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND date(TransInvoice.InvoiceDate) = date(TransInventoryLedger.LastUpdated) \n '
            }

            pSql += ') TotalReturns \n' +
            ',(SELECT SUM(TransCashHandling.TotalAmount) FROM TransCashHandling \n' +
            'WHERE TransCashHandling.DepositedTo = ' + ReadyString(pData.PeopleID) + ' AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' + '\n' +
            '(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ')  AcceptedMonies ' + '\n' +
            ',null DepositDate \n' +
            ',null TurnedOverTo \n' +
            ',null TurnedOverMonies \n' +
            ',TransInvoice.InvoiceDate as SalesTransactionDate \n' +
            ',null BankBranch \n' +
            ', null ServiceCharge \n' +
            ', null DepositReferenceNo \n' +
            ', null IsTurnOver \n' +
            ',null DepositedToByName \n' +
            'from TransInvoiceItems \n' +
            'inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID \n' +
            'left join TransCashHandling on TransInvoice.CashHandlingID = TransCashHandling.ID \n' +
            'inner join  LibraryPeople ppl1 on ppl1.ID = TransInvoice.UserID  \n' +
            'where TransCashHandling.ID is null and TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) + '\n' +
            ' and date(TransInvoice.InvoiceDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n'

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND TransInvoice.UserID = ' + ReadyString(pData.PeopleID) + '\n'
            }

            pSql += ' group by TransInvoice.CashHandlingID \n'

            pSql += 'UNION ALL \n' +
            'select distinct TransCashHandling.Depositedby PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransCashHandling.ID  ID, null GeneratedSales,  null DiscountAmount,  \n' +
            '(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger  \n' +
            'where TransInventoryLedger.UserID = IFNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' + '\n' + 
            '(TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n' +
            ' AND TransInventoryLedger.TransactionType = "Sales Returns" AND TransInventoryLedger.SubtransactionType IS NULL AND TransInventoryLedger.ParentID IS NULL AND TransInventoryLedger.BinID = "10d4672a-0358-4e2d-9471-2ee42e464300" \n'

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND date(TransInvoice.InvoiceDate) = date(TransInventoryLedger.LastUpdated) \n'
            }

            pSql += ') TotalReturns \n' +
            ',TotalAcceptedFromPrevTurnOver as AcceptedMonies, \n' +
            'DepositDate, \n' +
            'IFNULL(org1.CompanyName,ppl2.SurName || \', \' || ppl2.FirstName) TurnedOverTo, \n' +
            'TransCashHandling.TotalAmount TurnedOverMonies, \n' +
            'null as SalesTransactionDate, \n' +
            'TransCashHandling.BankBranch,  \n' +
            'TransCashHandling.ServiceCharge, \n ' +
            'TransCashHandling.DepositReferenceNo,\n ' +
            'TransCashHandling.IsTurnOver, \n ' +
            'TransCashHandling.DepositedToByName \n' +
            'from TransCashHandling  \n' +
            'left join TransInvoice on TransInvoice.CashHandlingID = TransCashHandling.ID \n ' +
            'left join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID \n' +
            'left join  LibraryPeople ppl1 on ppl1.ID = TransCashHandling.Depositedby \n' +
            'left join LibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo  \n' +
            'left join LibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo \n' +
            'where TransInvoice.CashHandlingID is null and TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + '\n' +
            ' and date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd'))

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND TransCashHandling.Depositedby = ' + ReadyString(pData.PeopleID) + '\n'
            }

            pSql += ' UNION ALL \n' +
            ' select TransCashHandling.Depositedby PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransCashHandling.ID  ID, SUM(SubTotal) GeneratedSales,  \n' +
            ' SUM(TransInvoiceItems.DiscountAmount) as DiscountAmount,\n ' +
            '(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger \n ' +
            'where TransInventoryLedger.UserID = IFNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date \n' +
            '(TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n' +
            ' AND TransInventoryLedger.TransactionType = "Sales Returns" AND TransInventoryLedger.SubtransactionType IS NULL AND TransInventoryLedger.ParentID IS NULL AND TransInventoryLedger.BinID = "10d4672a-0358-4e2d-9471-2ee42e464300"' + '\n'

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND date(TransInvoice.InvoiceDate) = date(TransInventoryLedger.LastUpdated) \n'
            }

            pSql += ') TotalReturns \n' +
            ',TotalAcceptedFromPrevTurnOver as AcceptedMonies, \n' +
            'DepositDate, \n' +
            'IFNULL(org1.CompanyName,ppl2.SurName || \', \' || ppl2.FirstName) TurnedOverTo, \n' +
            'TransCashHandling.TotalAmount TurnedOverMonies, \n' +
            'TransInvoice.InvoiceDate as SalesTransactionDate, \n' +
            'TransCashHandling.BankBranch,  \n' +
            'TransCashHandling.ServiceCharge, \n ' +
            'TransCashHandling.DepositReferenceNo,\n ' +
            'TransCashHandling.IsTurnOver, \n ' +
            'TransCashHandling.DepositedToByName \n' +
            'from TransCashHandling  \n' +
            'inner join TransInvoice on TransInvoice.CashHandlingID = TransCashHandling.ID  \n' +
            'inner join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID \n' +
            'left join  LibraryPeople ppl1 on ppl1.ID = TransCashHandling.Depositedby \n' +
            'left join LibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo \n ' +
            'left join LibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo \n ' +
            'where TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + '\n' +
            ' and date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n' +
            ' and date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd'))

            if (!CBool(isHeaderDetails)) {
                pSql += ' AND DepositedBy = ' + ReadyString(pData.PeopleID) + '\n'
            }

            pSql += ' group by TransInvoice.CashHandlingID '

            if (!CBool(isHeaderDetails)) {
                pSql += ',date(TransInvoice.InvoiceDate) '
            }

            pSql += ' ORDER BY TransInvoice.InvoiceDate DESC'

            pDT = Sql.Execute(pSql);

            return pOut.ProcessCashHandling(pData, pDT, isHeaderDetails);
        };

        pOut.ProcessCashHandling = function (pData, pDT, isHeaderDetails) {
            var newpDT = new Array();

            if (CBool(isHeaderDetails)) {
                var newpDTRows = [];

                for (var i = 0; i <= pDT.Rows.length - 1; i++) {

                    var isExist = false;
                    var ind = 0;

                    for (var m = 0; m < newpDTRows.length; m++) {
                        if (pDT.Rows[i].PeopleID == newpDTRows[m].PeopleID) {
                            ind = $.map(newpDTRows, function (obj, index) {
                                if (obj.PeopleID == pDT.Rows[i].PeopleID) {
                                    return index;
                                }
                            })
                            isExist = true;
                        }
                    }

                    var mDeposit = 0;
                    var mTurnOver = 0;

                    if (!isExist) {

                        //if (pDT.Rows[i].GeneratedSales != undefined) {

                        if (CBool(pDT.Rows[i].IsTurnOver)) {
                            mTurnOver = pDT.Rows[i].TurnedOverMonies;
                        } else {
                            mDeposit = pDT.Rows[i].TurnedOverMonies;
                        }

                        newpDTRows.push({
                            'Index': i,
                            'SalesAmount': pDT.Rows[i].GeneratedSales,
                            'Cashier': pDT.Rows[i].Cashier,
                            'TotalAmount': pDT.Rows[i].TurnedOverMonies,
                            'TotalAcceptedFromPrevTurnOver': pDT.Rows[i].AcceptedMonies,
                            'TurnOverVariance': CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                            'PeopleID': pDT.Rows[i].PeopleID,
                            'DepositDate': pDT.Rows[i].DepositDate,
                            'ServiceCharge': pDT.Rows[i].ServiceCharge,
                            'DiscountAmount': pDT.Rows[i].DiscountAmount,
                            'SalesTransactionDate': $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy'),
                            'TotalReturns': pDT.Rows[i].TotalReturns,
                            'IsTurnOver': pDT.Rows[i].IsTurnOver,
                            'TotalTurnOver': mTurnOver,
                            'TotalDeposit': mDeposit
                        });
                        newpDT.push({ 'Rows': newpDTRows });
                    }
                    else {

                        if (CBool(pDT.Rows[i].IsTurnOver)) {
                            newpDTRows[ind].TotalTurnOver = CDec2(newpDTRows[ind].TotalTurnOver) + CDec2(pDT.Rows[i].TurnedOverMonies);
                            //mTurnOver = pDT.Rows[i].TurnedOverMonies;
                        } else {
                            newpDTRows[ind].TotalDeposit = CDec2(newpDTRows[ind].TotalDeposit) + CDec2(pDT.Rows[i].TurnedOverMonies);
                            //mDeposit = pDT.Rows[i].TurnedOverMonies;
                        }

                        newpDTRows[ind].SalesAmount = CDec2(newpDTRows[ind].SalesAmount) + CDec2(pDT.Rows[i].GeneratedSales);
                        newpDTRows[ind].TotalAmount = CDec2(newpDTRows[ind].TotalAmount) + CDec2(pDT.Rows[i].TurnedOverMonies);
                        //newpDTRows[ind].TotalAcceptedFromPrevTurnOver = CDec2(newpDTRows[ind].TotalAcceptedFromPrevTurnOver) + CDec2(pDT.Rows[i].AcceptedMonies);
                        newpDTRows[ind].TurnOverVariance = CDec2(newpDTRows[ind].TurnOverVariance) + CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                        newpDTRows[ind].ServiceCharge = CDec2(newpDTRows[ind].ServiceCharge) + CDec2(pDT.Rows[i].ServiceCharge);
                        newpDTRows[ind].DiscountAmount = CDec2(newpDTRows[ind].DiscountAmount) + CDec2(pDT.Rows[i].DiscountAmount);
                        newpDTRows[ind].SalesTransactionDate = $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy');

                        //newpDTRows[ind].AcceptedMonies = CDec2(newpDTRows[ind].AcceptedMonies) + CDec2(pDT.Rows[i].AcceptedMonies);

                    }
                    //}
                }

                return newpDT;
                //return newpDTRows;
            }
            else {
                var newpDTRows = [];

                for (var i = 0; i < pDT.Rows.length; i++) {

                    //if (pDT.Rows[i].GeneratedSales != undefined) {
                    if (newpDTRows.length == 0) {
                        newpDTRows.push({
                            'Index': i,
                            'GeneratedSales': pDT.Rows[i].GeneratedSales,
                            'Cashier': pDT.Rows[i].Cashier,
                            'TotalAmount': pDT.Rows[i].TurnedOverMonies,
                            'TotalAcceptedFromPrevTurnOver': pDT.Rows[i].AcceptedMonies,
                            'TurnOverVariance': CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                            'PeopleID': pDT.Rows[i].PeopleID,
                            'DepositDate': pDT.Rows[i].DepositDate,
                            'ServiceCharge': pDT.Rows[i].ServiceCharge,
                            'DiscountAmount': pDT.Rows[i].DiscountAmount,
                            'SalesTransactionDate': $.format.date(new Date((pDT.Rows[i].SalesTransactionDate || pDT.Rows[i].DepositDate)), 'MMM dd, yyyy'),
                            'AcceptedMonies': pDT.Rows[i].AcceptedMonies,
                            'AcceptedMoniesFrom': pDT.Rows[i].AcceptedMoniesFrom,
                            'TotalReturns': pDT.Rows[i].TotalReturns
                        })
                    }
                    else {
                        var isExist = false;
                        var ind = 0;

                        for (var m = 0; m < newpDTRows.length; m++) {
                            if(OfflineMode){
                                if (pDT.Rows[i].SalesTransactionDate != undefined) {
                                    if (
                                $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy') == newpDTRows[m].SalesTransactionDate

                                && pDT.Rows[i].PeopleID == newpDTRows[m].PeopleID) {
                                        ind = $.map(newpDTRows, function (obj, index) {
                                            if (obj.PeopleID == pDT.Rows[i].PeopleID &&
                                        (obj.SalesTransactionDate == $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy') ||
                                        obj.SalesTransactionDate == $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') ||
                                        obj.DepositDate == $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') ||
                                        obj.DepositDate == $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy')
                                        )) {
                                                return index;
                                            }
                                        })
                                        isExist = true;
                                    }
                                }
                                else {
                                    ind = $.map(newpDTRows, function (obj, index) {
                                        if (obj.PeopleID == pDT.Rows[i].PeopleID &&
                                        obj.SalesTransactionDate == $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy')) {
                                            return index;
                                        }
                                    })
                                    isExist = true;
                                }
                            }
                            else {
                                if (
                                ($.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy') == newpDTRows[m].SalesTransactionDate ||
                                $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') == newpDTRows[m].DepositDate ||
                                $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') == newpDTRows[m].SalesTransactionDate)
                                && pDT.Rows[i].PeopleID == newpDTRows[m].PeopleID) {
                                    ind = $.map(newpDTRows, function (obj, index) {
                                        if (obj.PeopleID == pDT.Rows[i].PeopleID &&
                                        (obj.SalesTransactionDate == $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy') ||
                                        obj.SalesTransactionDate == $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') ||
                                        obj.DepositDate == $.format.date(new Date(pDT.Rows[i].DepositDate), 'MMM dd, yyyy') ||
                                        obj.DepositDate == $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy')
                                        )) {
                                            return index;
                                        }
                                    })
                                    isExist = true;
                                }
                            }                            
                        }

                        if (!isExist) {
                            newpDTRows.push({
                                'Index': i,
                                'GeneratedSales': pDT.Rows[i].GeneratedSales,
                                'Cashier': pDT.Rows[i].Cashier,
                                'TotalAmount': pDT.Rows[i].TurnedOverMonies,
                                'TotalAcceptedFromPrevTurnOver': pDT.Rows[i].AcceptedMonies,
                                'TurnOverVariance': CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                                'PeopleID': pDT.Rows[i].PeopleID,
                                'DepositDate': pDT.Rows[i].DepositDate,
                                'ServiceCharge': pDT.Rows[i].ServiceCharge,
                                'DiscountAmount': pDT.Rows[i].DiscountAmount,
                                'SalesTransactionDate': $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy'),
                                'AcceptedMonies': pDT.Rows[i].AcceptedMonies,
                                'AcceptedMoniesFrom': pDT.Rows[i].AcceptedMoniesFrom,
                                'TotalReturns': pDT.Rows[i].TotalReturns
                            })
                        }
                        else {

                            newpDTRows[ind].GeneratedSales = CDec2(newpDTRows[ind].GeneratedSales) + CDec2(pDT.Rows[i].GeneratedSales);
                            newpDTRows[ind].TotalAmount = CDec2(newpDTRows[ind].TotalAmount) + CDec2(pDT.Rows[i].TurnedOverMonies);
                            //newpDTRows[ind].TotalAcceptedFromPrevTurnOver = CDec2(newpDTRows[ind].TotalAcceptedFromPrevTurnOver) + CDec2(pDT.Rows[i].AcceptedMonies);
                            newpDTRows[ind].TurnOverVariance = CDec2(newpDTRows[ind].TurnOverVariance) + CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                            //newpDTRows[ind].SalesTransactionDate = $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy');
                            newpDTRows[ind].ServiceCharge = CDec2(newpDTRows[ind].ServiceCharge) + CDec2(pDT.Rows[i].ServiceCharge);
                            newpDTRows[ind].DiscountAmount = CDec2(newpDTRows[ind].DiscountAmount) + CDec2(pDT.Rows[i].DiscountAmount);
                            //newpDTRows[ind].AcceptedMonies = CDec2(newpDTRows[ind].AcceptedMonies) + CDec2(pDT.Rows[i].AcceptedMonies);'comment since subquery ni sya therefore, total na ni sya na accepted

                        }
                    }
                    //}
                    //else {

                    //}
                }
                //newpDT.push({ 'Rows': newpDTRows });

                if (OfflineMode) {
                    newpDT.push(newpDTRows, pOut.GetTurnover(pData, mDateFrom, mDateTo), pOut.GetDeposit(pData, mDateFrom, mDateTo))
                    return newpDT;
                } else {
                    return newpDTRows;
                }
            }
        }

        pOut.GetTurnover = function (pData, mDateFrom, mDateTo) {
            var pSql = '';
            var pDT;

            pSql = 'SELECT ' +
                    ' ppl2.FirstName || " " || ppl2.SurName || " of " || orgfowst.Name as TurnoverBy, ' +
                    ' ppl1.FirstName || " " || ppl1.SurName as TurnoverTo, ' +
                    ' ch.TotalAmount as AmountTurnover, ' +
                    ' ch.DepositDate as DateToTurnover, ' +
                    ' ppl2.ID as PeopleID ' +
                    ' FROM ' +
                    '   TransCashHandling ch ' +
                    '   LEFT JOIN LibraryPeople ppl1 ON ch.DepositedTo = ppl1.ID ' +
                    '   LEFT JOIN LibraryPeople ppl2 ON ch.DepositedBy = ppl2.ID ' +
                    '   LEFT JOIN LibraryOrganizationFOWST orgfowst ON orgfowst.ID = ch.FowstID ' +
            //'   LEFT JOIN LibrarySecurityRoleUser roleusr ON roleusr.PeopleID = ppl1.ID ' +
            //'   LEFT JOIN LibrarySecurityRole role ON role.ID = roleusr.RoleID ' +
                    '   WHERE ch.FowstID = ' + ReadyString(pData.FowstID) +
                    '       AND ch.DepositedBy = ' + ReadyString(pData.PeopleID) +
                    '       AND (date(ch.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) +
                    '       AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') ' +
                    '       AND ch.IsTurnOver = 1 ' +
                    '       GROUP BY ppl1.ID, ch.DepositDate '

            pDT = Sql.Execute(pSql);
            return pDT;
        }

        pOut.GetDeposit = function (pData, mDateFrom, mDateTo) {
            var pSql = '';
            var pDT;

            pSql = 'SELECT ' +
                    ' ppl1.FirstName || " " || ppl1.SurName || " of " || orgfowst.Name as DepositedBy, ' +
                    ' IFNULL(ch.DepositedToByName, "") as DepositedTo, ' +
                    ' ch.ServiceCharge, ' +
                    ' org.CompanyName as BankName, ' +
                    ' ch.BankBranch as BankBranchName, ' +
                    ' ch.TotalAmount as AmountDeposit, ' +
                    ' ch.DepositReferenceNo, ' +
                    ' ch.DepositDate, ' +
                    ' ppl1.ID as PeopleID ' +
                    ' FROM ' +
                    '   TransCashHandling ch ' +
                    '   LEFT JOIN LibraryOrganization org ON ch.DepositedTo = org.ID ' +
                    '   LEFT JOIN LibraryPeople ppl1 ON ch.DepositedBy = ppl1.ID ' +
                    '   LEFT JOIN LibraryOrganizationFOWST orgfowst ON orgfowst.ID = ch.FowstID ' +
                    '   WHERE ch.FowstID = ' + ReadyString(pData.FowstID) +
                    '       AND ch.DepositedBy = ' + ReadyString(pData.PeopleID) +
                    '       AND (date(ch.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) +
                    '       AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') ' +
                    '       AND ch.IsTurnOver = 0' +
                    '   GROUP BY ch.DepositDate'
            pDT = Sql.Execute(pSql);
            return pDT;
        }

        pOut.getUndepositedMonies = function (pData) {
            var pSql = '';
            var pDT;

            pSql = 'Select SUM(SubTotal) GrossSales, ' +
            'SUM(IFNULL(TransInvoiceItems.DiscountAmount,0)) TotalDiscounts, ' +
            '(SELECT SUM(TransCashHandling.TotalAmount) FROM TransCashHandling ' +
            'WHERE TransCashHandling.DepositedTo = ' + ReadyString(Web.PeopleUserID()) + ' AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' +
            '(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(pData.From, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(pData.To, 'yyyy-MM-dd')) + ')  AcceptedMonies, ' +
            '(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger  where UserID = ' + ReadyString(Web.PeopleUserID()) + ' AND CashHandlingID IS NULL AND FOWSTID = ' + ReadyString(pData.FowstID) +
            ' AND date (TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(pData.From, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(pData.To, 'yyyy-MM-dd')) + ') TotalReturns ' +
            'from TransInvoiceItems ' +
            'inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID ' +
            'where TransInvoice.CashHandlingID IS NULL AND TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) +
            ' AND date(TransInvoice.InvoiceDate) BETWEEN ' + ReadyString($.format.date(pData.From, 'yyyy-MM-dd')) +
            ' AND ' + ReadyString($.format.date(pData.To, 'yyyy-MM-dd')) +
            ' AND TransInvoice.UserID = ' + ReadyString(Web.PeopleUserID())

            pDT = Sql.Execute(pSql);
            return pDT;
        }

        pOut.getBreakDownofMoniesAccepted = function (pData) {
            var pSql = '';
            var pDT;

            pSql = 'SELECT TransCashHandling.TotalAmount, DepositedByName, DepositDate FROM TransCashHandling ' +
            'WHERE TransCashHandling.DepositedTo = ' + ReadyString(Web.PeopleUserID()) + ' AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' +
            '(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(pData.From, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(pData.To, 'yyyy-MM-dd'))

            pDT = Sql.Execute(pSql);
            return pDT;
        }

        //pOut.getBreakDownofMoniesFromSales = function (pData) {
        //    var pSql = '';
        //    var pDT;

        //    pSql = 'Select TransInvoice.ID, \'SI#\' || TransInvoice.InvoiceNumber InvoiceNumber, SUM(SubTotal) - SUM(IFNULL(TransInvoiceItems.DiscountAmount,0)) GeneratedSales ' +
        //    'from TransInvoiceItems ' +
        //    'inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID ' +
        //    'where TransInvoice.CashHandlingID IS NULL AND TransInvoice.FOWSTID = ' + ReadyString(pData.FowstID) +
        //    ' AND date(TransInvoice.InvoiceDate) BETWEEN ' + ReadyString($.format.date(pData.From, 'yyyy-MM-dd')) +
        //    ' AND ' + ReadyString($.format.date(pData.To, 'yyyy-MM-dd')) +
        //    ' AND TransInvoice.UserID = ' + ReadyString(Web.PeopleUserID())

        //    pDT = Sql.Execute(pSql);
        //    return pDT;
        //}

        return pOut;

    })
})
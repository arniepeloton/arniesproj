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

            //comment on feb 8 2022
            //pSql = 'select TransInvoice.Userid PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransInvoice.ID ID, SUM(SubTotal) GeneratedSales, \n' +
            //'SUM(TransInvoiceItems.DiscountAmount) + (SELECT SUM(SCDiscount) FROM TransInvoice WHERE date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as DiscountAmount,  \n' +
            //' (SELECT SUM(ForegoneVat) FROM TransInvoice WHERE date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as VatExemptTax, \n' +
            //'(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger  \n' +
            //'where TransInventoryLedger.UserID = IFNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date \n' +
            //'(TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ' \n' +
            //' AND TransInventoryLedger.TransactionType = "Sales Returns" AND (TransInventoryLedger.SubtransactionType IS NULL OR TransInventoryLedger.SubtransactionType = \'\') AND (TransInventoryLedger.ParentID IS NULL OR TransInventoryLedger.ParentID = \'\') AND TransInventoryLedger.BinID = "10d4672a-0358-4e2d-9471-2ee42e464300" \n'

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ' AND date(TransInvoice.InvoiceDate) = date(TransInventoryLedger.LastUpdated) \n '
            //}

            //pSql += ') TotalReturns \n' +

            pSql = "Select " + ReadyString(sessionStorage.PeopleID) + " PeopleID, IFNULL(DepositedByName , ppl1.SurName || ', ' || ppl1.FirstName) as Cashier, cache.CacheZRead.ID ID, CurGrossSales GeneratedSales, CurSCDiscount DiscountAmount, CurVATExempt VatExemptTax, CurSalesReturnExchange + CurSalesReturnRefund TotalReturns " +
            ', 0 AcceptedMonies \n' +
            //(SELECT SUM(TransCashHandling.TotalAmount) FROM TransCashHandling \n' +
            //'WHERE TransCashHandling.DepositedTo = ' + ReadyString(pData.PeopleID) + ' AND TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' + '\n' +
            //'WHERE TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date ' + '\n' +
            //'(TransCashHandling.DepositDate) = date(CacheZRead.SalesDate)) AcceptedMonies \n' +
            //'(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ')  AcceptedMonies ' + '\n' +
            ',null DepositDate \n' +
            ',null TurnedOverTo \n' +
            ',null TurnedOverMonies \n' +
            ',CacheZRead.SalesDate as SalesTransactionDate \n' +
            ',null BankBranch \n' +
            ', null ServiceCharge \n' +
            ', null DepositReferenceNo \n' +
            ', null IsTurnOver \n' +
            ',null DepositedToByName \n' +
            "FROM cache.CacheZRead inner join LibraryPeople ppl1 on ppl1.ID = " + ReadyString(sessionStorage.PeopleID) + " " +
            //'from TransInvoiceItems \n' +
            //'inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID \n' +
            'left join TransCashHandling on cache.CacheZRead.ID = TransCashHandling.ID \n' + //cache.CacheZRead.ID is temporary, need to add CacheZReadID in Z-Read Table
            'where TransCashHandling.ID is null and cache.CacheZRead.FOWSTID = ' + ReadyString(pData.FowstID) + '\n' +
            ' and date(cache.CacheZRead.SalesDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n'

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ' AND TransInvoice.UserID = ' + ReadyString(pData.PeopleID) + '\n'
            //}

            pSql += ' group by TransCashHandling.ID \n'

            pSql += 'UNION ALL \n' +
            'select distinct TransCashHandling.Depositedby PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransCashHandling.ID  ID, 0 GeneratedSales,  null DiscountAmount, 0 as VatExemptTax, \n' +
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

            //pSql += ' UNION ALL \n' +
            //' select TransCashHandling.Depositedby PeopleID, IFNULL(DepositedByName , ppl1.SurName || \', \' || ppl1.FirstName) as Cashier, TransCashHandling.ID  ID, SUM(SubTotal) GeneratedSales,  \n' +
            //' SUM(TransInvoiceItems.DiscountAmount) + (SELECT SUM(SCDiscount) FROM TransInvoice WHERE date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as DiscountAmount,\n ' +
            //' (SELECT SUM(ForegoneVat) FROM TransInvoice WHERE date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + ') as VatExemptTax, \n' +
            //'(SELECT SUM(IFNULL(Value, 0)) from TransInventoryLedger \n ' +
            //'where TransInventoryLedger.UserID = IFNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = ' + ReadyString(pData.FowstID) + ' AND date \n' +
            //'(TransInventoryLedger.LastUpdated) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n' +
            //' AND TransInventoryLedger.TransactionType = "Sales Returns" AND TransInventoryLedger.SubtransactionType IS NULL AND TransInventoryLedger.ParentID IS NULL AND TransInventoryLedger.BinID = "10d4672a-0358-4e2d-9471-2ee42e464300"' + '\n'

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ' AND date(TransInvoice.InvoiceDate) = date(TransInventoryLedger.LastUpdated) \n'
            //}

            //pSql += ') TotalReturns \n' +
            //',TotalAcceptedFromPrevTurnOver as AcceptedMonies, \n' +
            //'DepositDate, \n' +
            //'IFNULL(org1.CompanyName,ppl2.SurName || \', \' || ppl2.FirstName) TurnedOverTo, \n' +
            //'TransCashHandling.TotalAmount TurnedOverMonies, \n' +
            //'TransInvoice.InvoiceDate as SalesTransactionDate, \n' +
            //'TransCashHandling.BankBranch,  \n' +
            //'TransCashHandling.ServiceCharge, \n ' +
            //'TransCashHandling.DepositReferenceNo,\n ' +
            //'TransCashHandling.IsTurnOver, \n ' +
            //'TransCashHandling.DepositedToByName \n' +
            //'from TransCashHandling  \n' +
            //'inner join TransInvoice on TransInvoice.CashHandlingID = TransCashHandling.ID  \n' +
            //'inner join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID \n' +
            //'left join  LibraryPeople ppl1 on ppl1.ID = TransCashHandling.Depositedby \n' +
            //'left join LibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo \n ' +
            //'left join LibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo \n ' +
            //'where TransCashHandling.FOWSTID = ' + ReadyString(pData.FowstID) + '\n' +
            //' and date(TransCashHandling.DepositDate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd')) + '\n' +
            //' and date(Transinvoice.invoicedate) BETWEEN ' + ReadyString($.format.date(mDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(mDateTo, 'yyyy-MM-dd'))

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ' AND DepositedBy = ' + ReadyString(pData.PeopleID) + '\n'
            //}

            //pSql += ' group by TransInvoice.CashHandlingID '

            //if (!CBool(isHeaderDetails)) {
            //    pSql += ',date(TransInvoice.InvoiceDate) '
            //}

            //pSql += ' ORDER BY TransInvoice.InvoiceDate DESC'

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
                            'DiscountAmount': CDec2(AccountingFormat(pDT.Rows[i].DiscountAmount)),
                            'VatExemptTax': CDec2(AccountingFormat(pDT.Rows[i].VatExemptTax)),
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
                        newpDTRows[ind].DiscountAmount = CDec2(newpDTRows[ind].DiscountAmount) + CDec2(AccountingFormat(pDT.Rows[i].DiscountAmount));
                        newpDTRows[ind].VatExemptTax = CDec2(newpDTRows[ind].VatExemptTax) + CDec2(AccountingFormat(pDT.Rows[i].VatExemptTax));
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
                            'DiscountAmount': CDec2(AccountingFormat(pDT.Rows[i].DiscountAmount)),
                            'VatExemptTax': CDec2(AccountingFormat(pDT.Rows[i].VatExemptTax)),
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
                            if (OfflineMode) {
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
                                            isExist = true;
                                            return index;
                                        }
                                    })
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
                                'DiscountAmount': CDec2(AccountingFormat(pDT.Rows[i].DiscountAmount)),
                                'VatExemptTax': CDec2(AccountingFormat(pDT.Rows[i].VatExemptTax)),
                                'SalesTransactionDate': $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy'),
                                'AcceptedMonies': pDT.Rows[i].AcceptedMonies,
                                'AcceptedMoniesFrom': pDT.Rows[i].AcceptedMoniesFrom,
                                'TotalReturns': pDT.Rows[i].TotalReturns
                            })
                        }
                        else {
                            if (newpDTRows[ind] != undefined) {
                                newpDTRows[ind].GeneratedSales = CDec2(newpDTRows[ind].GeneratedSales) + CDec2(pDT.Rows[i].GeneratedSales);
                                newpDTRows[ind].TotalAmount = CDec2(newpDTRows[ind].TotalAmount) + CDec2(pDT.Rows[i].TurnedOverMonies);
                                //newpDTRows[ind].TotalAcceptedFromPrevTurnOver = CDec2(newpDTRows[ind].TotalAcceptedFromPrevTurnOver) + CDec2(pDT.Rows[i].AcceptedMonies);
                                newpDTRows[ind].TurnOverVariance = CDec2(newpDTRows[ind].TurnOverVariance) + CDec2(pDT.Rows[i].GeneratedSales) + CDec2(pDT.Rows[i].AcceptedMonies) - CDec2(pDT.Rows[i].TurnedOverMonies),
                                //newpDTRows[ind].SalesTransactionDate = $.format.date(new Date(pDT.Rows[i].SalesTransactionDate), 'MMM dd, yyyy');
                                newpDTRows[ind].ServiceCharge = CDec2(newpDTRows[ind].ServiceCharge) + CDec2(pDT.Rows[i].ServiceCharge);
                                newpDTRows[ind].DiscountAmount = CDec2(newpDTRows[ind].DiscountAmount) + CDec2(AccountingFormat(pDT.Rows[i].DiscountAmount));
                                newpDTRows[ind].VatExemptTax = CDec2(newpDTRows[ind].VatExemptTax) + CDec2(AccountingFormat(pDT.Rows[i].VatExemptTax));
                                //newpDTRows[ind].AcceptedMonies = CDec2(newpDTRows[ind].AcceptedMonies) + CDec2(pDT.Rows[i].AcceptedMonies);'comment since subquery ni sya therefore, total na ni sya na accepted
                            }
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

        pOut.getSalesDetails = function (vData) {
            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;
            var pFOWSTID;

            // options:
            // vData.PoMode = 1;

            pNonZero = vData.NonZero;
            pFOWSTID = vData.FOWSTID;

            pField = pFields.Add('ID');
            pField.Expression = 'TransInvoice.ID';
            pField.Searchable = false;

            pField = pFields.Add('CustomerID');
            pField.Expression = 'TransInvoice.CustomerID';
            pField.Searchable = false;

            pField = pFields.Add('FOWSTID');
            pField.Expression = 'TransInvoice.FOWSTID';
            pField.Searchable = false;

            pField = pFields.Add('InvoiceNum');
            pField.Expression = 'TransInvoice.InvoiceNumber';

            pField = pFields.Add('CompanyName');
            pField.Expression = " (CASE " +
                "     WHEN LibraryOrganization.IsOrganization = 1 THEN LibraryOrganization.CompanyName " +
                "     WHEN LibraryOrganization.IsOrganization = 0 THEN " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonSurname) || ', ',', '), '') || " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonFirstName) || ' ',' '), '') || " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonMiddleName) || ' ',' '), '') || " +
                "         IFNULL(NULLIF(', ' || TRIM(LibraryOrganization.PersonSuffixName),', '), '') " +
                " END)";

            pField = pFields.Add('InvoiceDate');
            pField.Expression = 'TransInvoice.InvoiceDate';

            pField = pFields.Add('Total');
            pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            pField = pFields.Add('SkuCount');
            pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            //additional by janine sept 5 2014 for sales order taker
            pField = pFields.Add('TotalOrder');
            pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            pField = pFields.Add('SkuCountOrder');
            pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;
            ///////////////
            pField = pFields.Add('SalesOrderNo');
            pField.Expression = 'IFNULL(SalesOrderNo,0)';

            pField = pFields.Add('UserID');
            pField.Expression = 'TransInvoice.UserID';
            pField.Searchable = false;

            pField = pFields.Add('EmployeeFullName');
            pField.Expression = 'LibraryPeople.Surname || \',\' || LibraryPeople.FirstName';

            pField = pFields.Add('FOWSTName');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('DeviceName');
            pField.Expression = 'LibraryDevice.PhoneName';

            pField = pFields.Add('CurrentUserID');
            pField.Expression = "'" + Web.PeopleUserID() + "'";
            pField.Searchable = false;

            pField = pFields.Add('MIN');
            pField.Expression = 'LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo';

            pField = pFields.Add('TableNo');
            pField.Expression = 'TransInvoice.CustomerTableNo';

            pField = pFields.Add('DeliveryMethod');
            pField.Expression = 'TransInvoice.DeliveryMethod';

            pField = pFields.Add('OrderMethod');
            pField.Expression = 'TransInvoice.OrderMethod';

            pField = pFields.Add('CustomerDemographicGender');
            pField.Expression = 'TransInvoice.CustomerDemographicGender';

            pField = pFields.Add('CustomerDemographicAge');
            pField.Expression = 'TransInvoice.CustomerDemographicAge';

            pField = pFields.Add('SearchSO');
            pField.Expression = " LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo || \'-\' || IFNULL(SalesOrderNo,0) ";

            pField = pFields.Add('SalesOrderNoRemoveDash');
            pField.Expression = " LibraryOrganizationFOWST.FOWSTNo ||  LibraryDevice.DeviceNo ||  IFNULL(SalesOrderNo,0) ";

            pField = pFields.Add('PurchaserName');
            pField.Expression = 'TransInvoice.PurchaserName';

            pField = pFields.Add('IsDraft');
            pField.Expression = 'TransInvoice.IsDraft';

            pField = pFields.Add('PurchaseOrderNo');
            pField.Expression = 'TransInvoice.PurchaseOrderNo';
            //            pFinder.SqlFrom = 'FROM TransInvoice' +
            //                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID';
            pFinder.SqlFrom = ' FROM TransInvoice INNER JOIN' +
                ' LibraryPeople ON TransInvoice.UserID = LibraryPeople.ID INNER JOIN' +
                ' LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN' +
                ' LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID' +
                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID';
            pFinder.SqlWhere = ' WHERE TransInvoice.IsCancelled = 0' +
                ' AND EXISTS(SELECT TransInvoiceItems.ID' +
                ' FROM TransInvoiceItems' +
                ' WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)';

            pFinder.SqlWhere += ' AND TransInvoice.InvoiceNumber ' + (CBool(pNonZero) ? '<>' : '=') + ' 0\n' +
                    ' AND IFNULL(TransInvoice.PurchaserName, \'\') = \'\'\n';


            if (pFOWSTID != undefined) {
                pFinder.SqlWhere += ' AND TransInvoice.FOWSTID = ' + ReadyString(pFOWSTID);
            }

            pFinder.SqlWhere += ' AND TransInvoice.UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                                //' AND TransInvoice.CashHandlingID IS NULL \n' +
                                ' AND date(TransInvoice.InvoiceDate) = ' + ReadyString(vData.dateNow)
            pFinder.SqlOrder = ' ORDER BY TransInvoice.LastUpdated DESC';

            return pFinder.Requery();
        }

        pOut.GetInvoiceItems = function (vData) {
            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pInvoiceID;

            pInvoiceID = vData.InvoiceID;

            pField = pFields.Add('FOWSTID');
            pField.Expression = 'LibraryOrganizationFOWST.ID';
            pField.Searchable = false;

            pField = pFields.Add('FOWST');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('InvoiceID');
            pField.Expression = 'TransInvoice.ID';

            pField = pFields.Add('InvoiceNo');
            pField.Expression = 'TransInvoice.InvoiceNumber';

            pField = pFields.Add('InvoiceDate');
            pField.Expression = 'TransInvoice.InvoiceDate';

            pField = pFields.Add('ProductDescription');
            pField.Expression = 'IFNULL(LibraryPIMProduct.ProductGroupNo,\'\') || \' - \' || IFNULL(LibraryPIMProduct.ProductNo,\'\') || \' - \' || IFNULL(LibraryPIMProduct.ProductName,\'\')';

            pField = pFields.Add('Qty');
            pField.Expression = 'TransInvoiceItems.Quantity';

            pField = pFields.Add('UnitMeasure');
            pField.Expression = 'TransInvoiceItems.UnitMeasure';

            pField = pFields.Add('Price');
            pField.Expression = 'TransInvoiceItems.UnitPrice';

            pField = pFields.Add('SubTotal');
            pField.Expression = 'TransInvoiceItems.SubTotal';

            pField = pFields.Add('Discount');
            pField.Expression = 'TransInvoiceItems.DiscountAmount';

            //pField = pFields.Add('SalesRepresentative');
            //pField.Expression = '(SELECT DISTINCT STUFF((SELECT DISTINCT \',\'+ IFNULL(CASE WHEN LibraryPeople.SurName = \'\' THEN \'N/A\' ' +
            //                    'ELSE LibraryPeople.FirstName + \' \' + LibraryPeople.SurName END,\'N/A\') ' +
            //                    'FROM TransInvoiceItemSalesReps LEFT join LibraryPeople on LibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID ' +
            //                    'WHERE TransInvoiceItemSalesReps.ProductID=TransInvoiceItems.ProductID and TransInvoiceItemSalesReps.InvoiceID =TransInvoice.ID ' +
            //                    'FROM TransInvoiceItemSalesReps)';
            //FOR XML PATH(\'\')), 1, 1,\'\')
            pField = pFields.Add('Customer');
            pField.Expression = '(CASE WHEN LibraryOrganization.CompanyName  = \'\' THEN LibraryOrganization.PersonSurname || \', \' || LibraryOrganization.PersonFirstName ELSE LibraryOrganization.CompanyName END)';

            pField = pFields.Add('RebatesID');
            pField.Expression = 'TransInvoiceItemsRebates.ProductID';

            pField = pFields.Add('RebatesProduct');
            pField.Expression = 'IFNULL(p1.ProductGroupNo,\'\') || \' - \' || IFNULL(p1.ProductNo,\'\') || \' - \' || IFNULL(p1.ProductName,\'\')';

            pField = pFields.Add('QtyRebates');
            pField.Expression = 'TransInvoiceItemsRebates.Quantity';

            pField = pFields.Add('UnitMeasureRebates');
            pField.Expression = 'TransInvoiceItemsRebates.UnitMeasure';

            pField = pFields.Add('UnitMeasureRebates');
            pField.Expression = 'TransInvoiceItemsRebates.UnitMeasure';

            pField = pFields.Add('UpdatedByName');
            pField.Expression = 'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdaterSuffix.Code)) || \' \', \' \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.Surname)) || \', \',\', \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.FirstName)) || \' \',\' \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.MiddleName)) || \' \',\' \'), \'\') || ' +
                                'IFNULL(NULLIF(\', \' || LTRIM(RTRIM(tblUpdater.PersonalSuffixName)),\', \'), \'\')';

            pFinder.SqlFrom = '  FROM TransInvoiceItems INNER JOIN ' +
                                'LibraryPIMProduct ON TransInvoiceItems.ProductID = LibraryPIMProduct.ID INNER JOIN  ' +
                                'TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID INNER JOIN  ' +
                                'LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID  ' +
                                'LEFT join TransInvoiceItemSalesReps on transinvoiceitems.InvoiceID = TransInvoiceItemSalesReps.InvoiceID  ' +
                                'AND transinvoiceitems.ProductID = TransInvoiceItemSalesReps.ProductID  ' +
                                'LEFT join LibraryPeople on LibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID  ' +
                                'LEFT join LibraryOrganization on LibraryOrganization.ID = TransInvoice.CustomerID  ' +
                                'left join TransInvoiceItemsRebates on TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID  ' +
                                'LEFT JOIN LibraryPIMProduct p1 ON TransInvoiceItemsRebates.ProductID = p1.ID  ' +
                                'LEFT JOIN LibraryPeople tblUpdater ON tblUpdater.ID = TransInvoice.UserID ' +
                                'LEFT JOIN LibraryPeoplePreNominalSuffix tblUpdaterSuffix ON tblUpdater.TitleID = tblUpdaterSuffix.ID  '

            pFinder.SqlWhere = 'WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(pInvoiceID)

            return pFinder.Requery();
        }

        return pOut;

    })
})
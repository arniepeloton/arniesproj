Public Class CashHandling

    Public Function GetDepositsDetails() As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT  t.TransactionDate SalesTransactionDate " & vbCrLf &
                ",ISNULL(sum(invoiceamount), 0) as GeneratedSales " & vbCrLf &
                ",(t.Cashier) Cashier " & vbCrLf &
                ",t.DepositDate " & vbCrLf &
                ",sum(depositamount) as TurnedOverMonies " & vbCrLf &
                ",ISNULL(t.DepositTo, '') as TurnedOverTo " & vbCrLf &
                ",ISNULL(t.TotalAcceptedFromPrevTurnOver, 0) AcceptedMonies" & vbCrLf &
                ", ISNULL(SUM(invoiceamount) + t.TotalAcceptedFromPrevTurnOver - sum(depositamount), 0) as TurnOverVariance " & vbCrLf &
                ", t.PeopleID PeopleID " & vbCrLf &
                ", fowst.Name FowstName " & vbCrLf &
                ", t.BankBranch" & vbCrLf &
                ", t.DepositReferenceNo" & vbCrLf &
                ", t.ServiceCharge" & vbCrLf &
                ", t.IsTurnOver" & vbCrLf &
                "from ( " & vbCrLf &
                "select convert(date, transinvoice.InvoiceDate) AS TransactionDate " & vbCrLf &
                ",sum(transinvoice.TotalAmount) as InvoiceAmount " & vbCrLf &
                ", ppl1.Surname + ', ' + ppl1.FirstName Cashier " & vbCrLf &
                ",ppl1.ID PeopleID " & vbCrLf &
                ",0  as depositamount " & vbCrLf &
                ",p2.DepositDate " & vbCrLf &
                ",p2.TotalAmount " & vbCrLf &
                ",p2.DepositTo " & vbCrLf &
                ",p2.TotalAcceptedFromPrevTurnOver " & vbCrLf &
                ",TransInvoice.FowstID" & vbCrLf &
                ",p2.BankBranch " & vbCrLf &
                ",p2.DepositReferenceNo " & vbCrLf &
                ",p2.ServiceCharge " & vbCrLf &
                ",p2.IsTurnOver " & vbCrLf &
                "from transinvoice  " & vbCrLf &
                "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
                "outer apply( " & vbCrLf &
                "SELECT TransCashHandling.DepositDate " & vbCrLf &
                ",TransCashHandling.TotalAmount  " & vbCrLf &
                ",t2.DepositTo " & vbCrLf &
                ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
                ",BankBranch " & vbCrLf &
                ",DepositReferenceNo " & vbCrLf &
                ",ServiceCharge " & vbCrLf &
                ",IsTurnOver " & vbCrLf &
                "from TransCashHandling " & vbCrLf &
                "OUTER APPLY(   " & vbCrLf &
                "Select DepositedToByName + ' of ' + CompanyName DepositTo  " & vbCrLf &
                "from MobizLibraryOrganization where ID = TransCashHandling.DepositedTo   " & vbCrLf &
                " UNION " & vbCrLf &
                "Select ppl2.Surname + ', ' + ppl2.FirstName DepositTo  " & vbCrLf &
                "from MobizLibraryPeople ppl2 where ppl2.ID =  TransCashHandling.DepositedTo  " & vbCrLf &
                ")t2 " & vbCrLf &
                "where TransCashHandling.DepositedBy = TransInvoice.UserID And " & vbCrLf &
                "CONVERT(date,transinvoice.InvoiceDate) = CONVERT(date,TransCashHandling.TransactionDateAndTimeFrom) " & vbCrLf &
                ")p2 " & vbCrLf &
                "where convert(date, transinvoice.InvoiceDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
                " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
                " AND TransInvoice.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                "group by convert(date, transinvoice.InvoiceDate) " & vbCrLf &
                ", transinvoice.UserID " & vbCrLf &
                ",ppl1.Surname ,ppl1.FirstName  " & vbCrLf &
                ",p2.DepositDate, p2.TotalAmount " & vbCrLf &
                ",p2.DepositTo, p2.TotalAcceptedFromPrevTurnOver,ppl1.ID,TransInvoice.FowstID " & vbCrLf &
                ",p2.BankBranch, p2.ServiceCharge, p2.DepositReferenceNo, p2.IsTurnOver" & vbCrLf &
                " UNION " & vbCrLf &
                "SELECT convert(date, TransCashHandling.DepositDate) AS DATE " & vbCrLf &
                ",0 as invoiceamount " & vbCrLf &
                ",p1.cashier " & vbCrLf &
                ",p1.PeopleID" & vbCrLf &
                ",TransCashHandling.TotalAmount as depositamount " & vbCrLf &
                ",DepositDate " & vbCrLf &
                ",TransCashHandling.TotalAmount as DepositedAmount " & vbCrLf &
                ",t1.DepositTo " & vbCrLf &
                ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
                ",TransCashHandling.FOWSTID " & vbCrLf &
                ",TransCashHandling.BankBranch " & vbCrLf &
                ",TransCashHandling.DepositReferenceNo " & vbCrLf &
                ",TransCashHandling.ServiceCharge " & vbCrLf &
                ",TransCashHandling.IsTurnOver " & vbCrLf &
                "from TransCashHandling " & vbCrLf &
                "outer apply( " & vbCrLf &
                " select ppl1.Surname + ', ' + ppl1.FirstName Cashier " & vbCrLf &
                " ,ppl1.ID PeopleID" & vbCrLf &
                " from transinvoice  " & vbCrLf &
                "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
                "where TransCashHandling.DepositedBy = TransInvoice.UserID And " & vbCrLf &
                "CONVERT(date,transinvoice.InvoiceDate) = CONVERT(date,TransCashHandling.TransactionDateAndTimeFrom) " & vbCrLf &
                ")p1 " & vbCrLf &
                "OUTER APPLY(   " & vbCrLf &
                "Select DepositedToByName + ' of ' + CompanyName DepositTo  " & vbCrLf &
                "from MobizLibraryOrganization where ID = TransCashHandling.DepositedTo   " & vbCrLf &
                "UNION " & vbCrLf &
                "Select ppl2.Surname + ', ' + ppl2.FirstName DepositTo  " & vbCrLf &
                "from MobizLibraryPeople ppl2 where ppl2.ID =  TransCashHandling.DepositedTo  " & vbCrLf &
                ")t1 " & vbCrLf &
                "WHERE convert(date, TransCashHandling.DepositDate) between " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
                " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
                " AND TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                "group by convert(date, TransCashHandling.DepositDate),  " & vbCrLf &
                "p1.cashier, DepositDate, TotalAmount, t1.DepositTo " & vbCrLf &
                ",TotalAcceptedFromPrevTurnOver, p1.PeopleID, TransCashHandling.FOWSTID " & vbCrLf &
                ",TransCashHandling.BankBranch " & vbCrLf &
                ",TransCashHandling.DepositReferenceNo " & vbCrLf &
                ",TransCashHandling.ServiceCharge " & vbCrLf &
                ",TransCashHandling.IsTurnOver " & vbCrLf &
                ") t LEFT JOIN MobizLibraryOrganizationFOWST fowst ON fowst.ID = t.FowstID" & vbCrLf &
                "WHERE t.PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & vbCrLf &
                "group by   " & vbCrLf &
                "t.TransactionDate, t.Cashier, " & vbCrLf &
                "t.DepositDate, t.DepositTo, t.TotalAcceptedFromPrevTurnOver, t.PeopleID, fowst.Name, " & vbCrLf &
                "t.BankBranch, t.DepositReferenceNo, t.ServiceCharge, t.IsTurnOver"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    <HttpPost()>
    Public Function GetDeposits() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDT2 As New DataTable
        Dim pSql As String

        'pSql = "SELECT  t.TransactionDate SalesTransactionDate " & vbCrLf &
        '        ",sum(invoiceamount) as SalesAmount " & vbCrLf &
        '        ",(t.Cashier) Cashier " & vbCrLf &
        '        ",t.DepositDate " & vbCrLf &
        '        ",depositamount as TotalAmount " & vbCrLf &
        '        ",t.DepositTo " & vbCrLf &
        '        ",t.TotalAcceptedFromPrevTurnOver TotalAcceptedFromPrevTurnOver" & vbCrLf &
        '        ", SUM(invoiceamount) + t.TotalAcceptedFromPrevTurnOver - sum(depositamount) as TurnOverVariance " & vbCrLf &
        '        ", t.PeopleID PeopleID " & vbCrLf &
        '        "from ( " & vbCrLf &
        '        "select convert(date, transinvoice.InvoiceDate) AS TransactionDate " & vbCrLf &
        '        ",sum(transinvoice.TotalAmount) as InvoiceAmount " & vbCrLf &
        '        ", ppl1.Surname + ', ' + ppl1.FirstName Cashier " & vbCrLf &
        '        ",ppl1.ID PeopleID " & vbCrLf &
        '        ",0  as depositamount " & vbCrLf &
        '        ",p2.DepositDate " & vbCrLf &
        '        ",p2.TotalAmount " & vbCrLf &
        '        ",p2.DepositTo " & vbCrLf &
        '        ",p2.TotalAcceptedFromPrevTurnOver " & vbCrLf &
        '        "from transinvoice  " & vbCrLf &
        '        "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
        '        "outer apply( " & vbCrLf &
        '        "SELECT TransCashHandling.DepositDate " & vbCrLf &
        '        ",TransCashHandling.TotalAmount  " & vbCrLf &
        '        ",t2.DepositTo " & vbCrLf &
        '        ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
        '        "from TransCashHandling " & vbCrLf &
        '        "OUTER APPLY(   " & vbCrLf &
        '        "Select DepositedToByName + ' of ' + CompanyName DepositTo  " & vbCrLf &
        '        "from MobizLibraryOrganization where ID = TransCashHandling.DepositedTo   " & vbCrLf &
        '        " UNION " & vbCrLf &
        '        "Select ppl2.Surname + ', ' + ppl2.FirstName DepositTo  " & vbCrLf &
        '        "from MobizLibraryPeople ppl2 where ppl2.ID =  TransCashHandling.DepositedTo  " & vbCrLf &
        '        ")t2 " & vbCrLf &
        '        "where TransCashHandling.DepositedBy = TransInvoice.UserID And " & vbCrLf &
        '        "CONVERT(date,transinvoice.InvoiceDate) = CONVERT(date,TransCashHandling.TransactionDateAndTimeFrom) " & vbCrLf &
        '        ")p2 " & vbCrLf &
        '        "where convert(date, transinvoice.InvoiceDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        '        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
        '        " AND TransInvoice.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        '        "group by convert(date, transinvoice.InvoiceDate) " & vbCrLf &
        '        ", transinvoice.UserID " & vbCrLf &
        '        ",ppl1.Surname ,ppl1.FirstName  " & vbCrLf &
        '        ",p2.DepositDate, p2.TotalAmount " & vbCrLf &
        '        ",p2.DepositTo, p2.TotalAcceptedFromPrevTurnOver,ppl1.ID " & vbCrLf &
        '        " UNION " & vbCrLf &
        '        "SELECT convert(date, TransCashHandling.DepositDate) AS DATE " & vbCrLf &
        '        ",0 as invoiceamount " & vbCrLf &
        '        ",p1.cashier " & vbCrLf &
        '        ",p1.PeopleID" & vbCrLf &
        '        ",TransCashHandling.TotalAmount as depositamount " & vbCrLf &
        '        ",DepositDate " & vbCrLf &
        '        ",TransCashHandling.TotalAmount as DepositedAmount " & vbCrLf &
        '        ",t1.DepositTo " & vbCrLf &
        '        ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
        '        "from TransCashHandling " & vbCrLf &
        '        "outer apply( " & vbCrLf &
        '        " select ppl1.Surname + ', ' + ppl1.FirstName Cashier " & vbCrLf &
        '        " ,ppl1.ID PeopleID" & vbCrLf &
        '        " from transinvoice  " & vbCrLf &
        '        "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
        '        "where TransCashHandling.DepositedBy = TransInvoice.UserID And " & vbCrLf &
        '        "CONVERT(date,transinvoice.InvoiceDate) = CONVERT(date,TransCashHandling.TransactionDateAndTimeFrom) " & vbCrLf &
        '        ")p1 " & vbCrLf &
        '        "OUTER APPLY(   " & vbCrLf &
        '        "Select DepositedToByName + ' of ' + CompanyName DepositTo  " & vbCrLf &
        '        "from MobizLibraryOrganization where ID = TransCashHandling.DepositedTo   " & vbCrLf &
        '        "UNION " & vbCrLf &
        '        "Select ppl2.Surname + ', ' + ppl2.FirstName DepositTo  " & vbCrLf &
        '        "from MobizLibraryPeople ppl2 where ppl2.ID =  TransCashHandling.DepositedTo  " & vbCrLf &
        '        ")t1 " & vbCrLf &
        '        "WHERE convert(date, TransCashHandling.DepositDate) between " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        '        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
        '        " AND TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        '        "group by convert(date, TransCashHandling.DepositDate),  " & vbCrLf &
        '        "p1.cashier, DepositDate, TotalAmount, t1.DepositTo " & vbCrLf &
        '        ",TotalAcceptedFromPrevTurnOver, p1.PeopleID " & vbCrLf &
        '        ") t " & vbCrLf &
        '        "group by   " & vbCrLf &
        '        "t.TransactionDate, t.Cashier, " & vbCrLf &
        '        "t.DepositDate, t.DepositTo, t.TotalAcceptedFromPrevTurnOver, t.PeopleID, depositamount "

        pSql = "SELECT " & vbCrLf &
        "TransInvoice.Userid PeopleID " & vbCrLf &
        ", ISNULL(DepositedByName , ppl1.SurName + ', ' + ppl1.FirstName) as Cashier" & vbCrLf &
        ", TransInvoice.ID ID, SUM(SubTotal) GeneratedSales" & vbCrLf &
        ", SUM(TransInvoiceItems.DiscountAmount) + (SELECT SUM(SCDiscount) FROM TransInvoice WHERE convert(date, Transinvoice.invoicedate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & ") as DiscountAmount" & vbCrLf &
        ", TransInvoice.ForegoneVAT as VatExemptTax" & vbCrLf &
        ",  (SELECT SUM(ISNULL(Value, 0)) " & vbCrLf &
        "from TransInventoryLedger" & vbCrLf &
        "where TransInventoryLedger.UserID = ISNULL(DepositedBy, TransInvoice.UserID) AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        " AND CashHandlingID IS NULL AND convert(date,TransInventoryLedger.LastUpdated) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
        " AND TransInventoryLedger.TransactionType = 'Sales Returns' AND TransInventoryLedger.SubtransactionType IS NULL AND TransInventoryLedger.ParentID IS NULL" & vbCrLf

        If Not CBool2(Common.WebRequest.Data("isHeaderDetails")) Then
            pSql += " AND convert(date, TransInvoice.InvoiceDate) = CONVERT(date, TransInventoryLedger.LastUpdated) " & vbCrLf
        End If

        '"WHERE TransCashHandling.DepositedTo IS NULL AND TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        'ORIG CODE LINE 231
        '"AND convert(date,TransCashHandling.DepositDate) = " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        '" AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf & ")  AcceptedMonies " & vbCrLf &

        pSql += ") TotalReturns " & vbCrLf &
        ",(SELECT SUM(TransCashHandling.TotalAmount) " & vbCrLf &
        "FROM TransCashHandling" & vbCrLf &
        "WHERE TransCashHandling.DepositedTo = TransInvoice.Userid AND TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        "AND convert(date,TransCashHandling.DepositDate) = convert(date, TransInvoice.InvoiceDate))  AcceptedMonies " & vbCrLf &
        ",null DepositDate " & vbCrLf &
        ",null TurnedOverTo " & vbCrLf &
        ",null TurnedOverMonies " & vbCrLf &
        ",TransInvoice.InvoiceDate as SalesTransactionDate " & vbCrLf &
        ",null BankBranch " & vbCrLf &
        ", null ServiceCharge " & vbCrLf &
        ", null DepositReferenceNo " & vbCrLf &
        ", null IsTurnOver " & vbCrLf &
        ",null DepositedToByName " & vbCrLf &
        "from TransInvoiceItems" & vbCrLf &
        "inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
        "left join TransCashHandling on TransInvoice.CashHandlingID = TransCashHandling.ID " & vbCrLf &
        "inner join  MobizLibraryPeople ppl1 on ppl1.ID = TransInvoice.UserID  " & vbCrLf &
        "where TransCashHandling.ID is null and TransInvoice.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        "and convert(date,TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf

        If Not CBool2(Common.WebRequest.Data("isHeaderDetails")) Then
            pSql += "AND TransInvoice.UserID = " & ReadyString(Common.WebRequest.Data("PeopleID"))
        End If

        pSql += " group by TransInvoice.CashHandlingID, TransInvoice.ForegoneVAT " & vbCrLf &
        ",TransInvoice.ID ,TransInvoice.UserID, TransCashHandling.DepositedByName, ppl1.Surname, ppl1.FirstName " & vbCrLf &
        ",TransCashHandling.DepositedBy, TransInvoice.InvoiceDate " & vbCrLf &
        "UNION ALL" & vbCrLf &
        "Select distinct" & vbCrLf &
        "TransCashHandling.Depositedby PeopleID" & vbCrLf &
        ",ISNULL(DepositedByName " & vbCrLf &
        ", ppl1.SurName + ', ' + ppl1.FirstName) as Cashier" & vbCrLf &
        ", TransCashHandling.ID  ID" & vbCrLf &
        ", null GeneratedSales" & vbCrLf &
        ",  null DiscountAmount" & vbCrLf &
        ",  null VatExemptTax" & vbCrLf &
        ",  (SELECT SUM(ISNULL(Value, 0)) from TransInventoryLedger  " & vbCrLf &
        "where UserID = DepositedBy AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        "AND CashHandlingID IS NULL AND convert(date,TransInventoryLedger.LastUpdated) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf & ") TotalReturns" & vbCrLf &
        ", TotalAcceptedFromPrevTurnOver as AcceptedMonies" & vbCrLf &
        ", DepositDate" & vbCrLf &
        ", ISNULL(org1.CompanyName,ppl2.SurName + ', ' + ppl2.FirstName) TurnedOverTo" & vbCrLf &
        ", TransCashHandling.TotalAmount TurnedOverMonies" & vbCrLf &
        ", null as SalesTransactionDate" & vbCrLf &
        ", TransCashHandling.BankBranch" & vbCrLf &
        ",  TransCashHandling.ServiceCharge" & vbCrLf &
        ",  TransCashHandling.DepositReferenceNo" & vbCrLf &
        ", TransCashHandling.IsTurnOver" & vbCrLf &
        ",  TransCashHandling.DepositedToByName " & vbCrLf &
        "from TransCashHandling" & vbCrLf &
        "left join TransInvoice on TransInvoice.CashHandlingID = TransCashHandling.ID  " & vbCrLf &
        "left join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
        "left join  MobizLibraryPeople ppl1 on ppl1.ID = TransCashHandling.Depositedby " & vbCrLf &
        "left join MobizLibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo  " & vbCrLf &
        "left join MobizLibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo " & vbCrLf &
        "where TransInvoice.CashHandlingID is null and TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        "and convert(date,TransCashHandling.DepositDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf

        If Not CBool2(Common.WebRequest.Data("isHeaderDetails")) Then
            pSql += "AND TransCashHandling.Depositedby = " & ReadyString(Common.WebRequest.Data("PeopleID"))
        End If

        pSql += " UNION ALL" & vbCrLf &
        "select TransCashHandling.Depositedby PeopleID" & vbCrLf &
        ", ISNULL(DepositedByName , ppl1.SurName + ', ' + ppl1.FirstName) as Cashier" & vbCrLf &
        ", TransCashHandling.ID  ID, SUM(SubTotal) GeneratedSales" & vbCrLf &
        ",   SUM(TransInvoiceItems.DiscountAmount) + (SELECT SUM(SCDiscount) FROM TransInvoice WHERE convert(date, Transinvoice.invoicedate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & ") as DiscountAmount" & vbCrLf &
        ", TransInvoice.ForegoneVAT as VatExemptTax" & vbCrLf &
        ", (SELECT SUM(ISNULL(Value, 0)) from TransInventoryLedger " & vbCrLf &
        "where UserID = DepositedBy AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & " AND CashHandlingID IS NULL " & vbCrLf &
        "AND convert(date,TransInventoryLedger.LastUpdated) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf & ") TotalReturns" & vbCrLf &
        ", TotalAcceptedFromPrevTurnOver as AcceptedMonies, DepositDate" & vbCrLf &
        ", ISNULL(org1.CompanyName,ppl2.SurName + ', ' + ppl2.FirstName) TurnedOverTo" & vbCrLf &
        ", TransCashHandling.TotalAmount TurnedOverMonies" & vbCrLf &
        ", TransInvoice.InvoiceDate as SalesTransactionDate" & vbCrLf &
        ", TransCashHandling.BankBranch" & vbCrLf &
        ",  TransCashHandling.ServiceCharge" & vbCrLf &
        ",  TransCashHandling.DepositReferenceNo" & vbCrLf &
        ", TransCashHandling.IsTurnOver" & vbCrLf &
        ",  TransCashHandling.DepositedToByName " & vbCrLf &
        "from TransCashHandling  inner join TransInvoice on TransInvoice.CashHandlingID = TransCashHandling.ID  " & vbCrLf &
        "inner join TransInvoiceItems on TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
        "left join  MobizLibraryPeople ppl1 on ppl1.ID = TransCashHandling.Depositedby " & vbCrLf &
        "left join MobizLibraryPeople ppl2 on ppl2.ID = TransCashHandling.depositedTo  " & vbCrLf &
        "left join MobizLibraryOrganization org1 on org1.ID = TransCashHandling.depositedTo " & vbCrLf &
        "where TransCashHandling.FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
        "and convert(date,TransCashHandling.DepositDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
        "and convert(date,Transinvoice.invoicedate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf

        If Not CBool2(Common.WebRequest.Data("isHeaderDetails")) Then
            pSql += "AND TransCashHandling.DepositedBy = " & ReadyString(Common.WebRequest.Data("PeopleID"))
        End If

        pSql += "group by TransInvoice.CashHandlingID, TransInvoice.ForegoneVAT " & vbCrLf &
        ",TransCashHandling.DepositedBy, TransCashHandling.DepositedByName, ppl1.Surname, ppl1.FirstName " & vbCrLf &
        ", TransCashHandling.ID,TransCashHandling.TotalAcceptedFromPrevTurnOver, TransInvoice.InvoiceDate " & vbCrLf &
        ",TransCashHandling.DepositDate, org1.CompanyName, ppl2.Surname, ppl2.FirstName, TransCashHandling.TotalAmount " & vbCrLf &
        ",TransCashHandling.BankBranch, TransCashHandling.ServiceCharge, TransCashHandling.DepositReferenceNo " & vbCrLf &
        ",TransCashHandling.IsTurnOver, TransCashHandling.DepositedToByName ORDER BY TransInvoice.InvoiceDate DESC"

        pDT = Common.WebRequest.ProData.Execute(pSql)


        Return pDT

    End Function

    Function GetTurnover() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDT2 As New DataTable
        Dim pSql As String

        pSql = "SELECT " & vbCrLf &
                "ppl2.FirstName + ' ' + ppl2.SurName + ' of ' + orgfowst.Name as TurnoverBy " & vbCrLf &
                ",  ppl1.FirstName + ' ' + ppl1.SurName as TurnoverTo " & vbCrLf &
                ",  ch.TotalAmount as AmountTurnover " & vbCrLf &
                ",  ch.DepositDate as DateToTurnover " & vbCrLf &
                ",  ppl2.ID as PeopleID   " & vbCrLf &
                "FROM    TransCashHandling ch     " & vbCrLf &
                "LEFT JOIN MobizLibraryPeople ppl1 ON ch.DepositedTo = ppl1.ID     " & vbCrLf &
                "LEFT JOIN MobizLibraryPeople ppl2 ON ch.DepositedBy = ppl2.ID     " & vbCrLf &
                "LEFT JOIN MobizLibraryOrganizationFOWST orgfowst ON orgfowst.ID = ch.FowstID    " & vbCrLf &
                "WHERE ch.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                " AND ch.DepositedBy = " & ReadyString(Common.WebRequest.Data("PeopleID")) & vbCrLf &
                "AND (convert(date,ch.DepositDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
                " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
                ") AND ch.IsTurnOver = 1 " & vbCrLf &
                "GROUP BY ppl1.ID, ch.DepositDate " & vbCrLf &
                ",ppl2.FirstName, ppl2.SurName , orgfowst.Name, ppl1.FirstName, ppl1.SurName, ch.TotalAmount, ppl2.ID"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetAllDeposits() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDT2 As New DataTable
        Dim pSql As String

        pSql = "SELECT  ppl1.FirstName + ' ' + ppl1.SurName + ' of ' + orgfowst.Name as DepositedBy " & vbCrLf &
            ",  ISNULL(ch.DepositedToByName, '') as DepositedTo " & vbCrLf &
            ",  ch.ServiceCharge " & vbCrLf &
            ",  org.CompanyName as BankName" & vbCrLf &
            ",  ch.BankBranch as BankBranchName " & vbCrLf &
            ",  ch.TotalAmount as AmountDeposit " & vbCrLf &
            ",  ch.DepositReferenceNo " & vbCrLf &
            ",  ch.DepositDate " & vbCrLf &
            ",  ppl1.ID as PeopleID   " & vbCrLf &
            "FROM    TransCashHandling ch     " & vbCrLf &
            "LEFT JOIN MobizLibraryOrganization org ON ch.DepositedTo = org.ID     " & vbCrLf &
            "LEFT JOIN MobizLibraryPeople ppl1 ON ch.DepositedBy = ppl1.ID     " & vbCrLf &
            "LEFT JOIN MobizLibraryOrganizationFOWST orgfowst ON orgfowst.ID = ch.FowstID " & vbCrLf &
            "WHERE ch.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
            IIf(Common.WebRequest.Data("PeopleID") = "", "", " AND ch.DepositedBy = " & ReadyString(Common.WebRequest.Data("PeopleID"))) & vbCrLf &
            " AND convert(date,ch.DepositDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
            " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
            " AND ch.IsTurnOver = 0 " & vbCrLf &
            " GROUP BY ch.DepositDate, org.CompanyName, ch.DepositedToByName, orgfowst.Name, ppl1.FirstName, ppl1.SurName, ch.TotalAmount, ppl1.ID " & vbCrLf &
            ",  ch.ServiceCharge,  ch.BankBranch,  ch.TotalAmount ,  ch.DepositReferenceNo,  ch.DepositDate "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function
End Class

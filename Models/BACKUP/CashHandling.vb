Public Class CashHandling

    Public Function GetInventoryAccountability() As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM TransInventoryProductCount " & vbCrLf &
                "inner join TransInventoryProductCountStock on transinventoryproductcount.ID = TransInventoryProductCountStock.InventoryProductCountID" & vbCrLf &
                "inner join MobizLibraryPIMProduct on transinventoryproductcount.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "INNER JOIN MobizLibraryPIMProductMeasurement on MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "INNER JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
                "INNER JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
                "inner join MobizLibraryPIMproductPriceLevel on MobizLibraryPIMproductPriceLevel.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
                "inner join MobizLibraryPIMPriceLevel on MobizLibraryPIMPriceLevel.ID = MobizLibraryPIMproductPriceLevel.PriceLevelID"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

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
                "from transinvoice  " & vbCrLf &
                "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
                "outer apply( " & vbCrLf &
                "SELECT TransCashHandling.DepositDate " & vbCrLf &
                ",TransCashHandling.TotalAmount  " & vbCrLf &
                ",t2.DepositTo " & vbCrLf &
                ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
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
                ") t LEFT JOIN MobizLibraryOrganizationFOWST fowst ON fowst.ID = t.FowstID" & vbCrLf &
                "WHERE t.PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & vbCrLf &
                "group by   " & vbCrLf &
                "t.TransactionDate, t.Cashier, " & vbCrLf &
                "t.DepositDate, t.DepositTo, t.TotalAcceptedFromPrevTurnOver, t.PeopleID, fowst.Name "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    <HttpPost()>
    Public Function GetDeposits() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDT2 As New DataTable
        Dim pSql As String

        pSql = "SELECT  t.TransactionDate SalesTransactionDate " & vbCrLf &
                ",sum(invoiceamount) as SalesAmount " & vbCrLf &
                ",(t.Cashier) Cashier " & vbCrLf &
                ",t.DepositDate " & vbCrLf &
                ",depositamount as TotalAmount " & vbCrLf &
                ",t.DepositTo " & vbCrLf &
                ",t.TotalAcceptedFromPrevTurnOver TotalAcceptedFromPrevTurnOver" & vbCrLf &
                ", SUM(invoiceamount) + t.TotalAcceptedFromPrevTurnOver - sum(depositamount) as TurnOverVariance " & vbCrLf &
                ", t.PeopleID PeopleID " & vbCrLf &
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
                "from transinvoice  " & vbCrLf &
                "inner join MobizLibraryPeople ppl1 on transinvoice.UserID = ppl1.ID " & vbCrLf &
                "outer apply( " & vbCrLf &
                "SELECT TransCashHandling.DepositDate " & vbCrLf &
                ",TransCashHandling.TotalAmount  " & vbCrLf &
                ",t2.DepositTo " & vbCrLf &
                ",TotalAcceptedFromPrevTurnOver " & vbCrLf &
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
                ",p2.DepositTo, p2.TotalAcceptedFromPrevTurnOver,ppl1.ID " & vbCrLf &
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
                ",TotalAcceptedFromPrevTurnOver, p1.PeopleID " & vbCrLf &
                ") t " & vbCrLf &
                "group by   " & vbCrLf &
                "t.TransactionDate, t.Cashier, " & vbCrLf &
                "t.DepositDate, t.DepositTo, t.TotalAcceptedFromPrevTurnOver, t.PeopleID, depositamount "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        pDT2.Columns.Add("SalesAmount")
        pDT2.Columns.Add("Cashier")
        pDT2.Columns.Add("TotalAmount")
        pDT2.Columns.Add("TotalAcceptedFromPrevTurnOver")
        pDT2.Columns.Add("TurnOverVariance")
        pDT2.Columns.Add("PeopleID")

        For i As Integer = 0 To pDT.Rows.Count - 1

            Dim TotalAcceptedFromPrevTurnOver As Decimal = CDec2(pDT.Compute("SUM(TotalAcceptedFromPrevTurnOver)", "Cashier = " & ReadyString(pDT.Rows(i).Item("Cashier"))))
            Dim SalesAmount As Decimal = CDec2(pDT.Compute("SUM(SalesAmount)", "Cashier = " & ReadyString(pDT.Rows(i).Item("Cashier"))))
            Dim TotalAmount As Decimal = CDec2(pDT.Compute("SUM(TotalAmount)", "Cashier = " & ReadyString(pDT.Rows(i).Item("Cashier"))))
            Dim TurnOverVariance As Decimal = CDec2(pDT.Compute("SUM(TurnOverVariance)", "Cashier = " & ReadyString(pDT.Rows(i).Item("Cashier"))))

            Dim datarow As DataRow

            datarow = pDT2.NewRow

            If ReadyString(pDT.Rows(i).Item("Cashier")) <> "''" Then
                datarow(0) = SalesAmount
                datarow(1) = pDT.Rows(i).Item("Cashier")
                datarow(2) = TotalAmount
                datarow(3) = TotalAcceptedFromPrevTurnOver
                datarow(4) = TurnOverVariance
                datarow(5) = pDT.Rows(i).Item("PeopleID")

                pDT2.DefaultView.RowFilter = "Cashier = " & ReadyString(pDT.Rows(i).Item("Cashier"))

                If pDT2.DefaultView.Count = 0 Then
                    pDT2.Rows.Add(datarow)
                End If
            End If

        Next

        Return pDT2

        'pFinder = New Finder

        'With pFinder
        '    .ProData = Common.WebRequest.ProData
        '    .MaxRecords = 50
        '    .SearchString = WebRequest.Instance.Data("data")

        '    With .Fields
        '        With .Add("ID", SqlDbType.UniqueIdentifier)
        '            .Searchable = False
        '            .Expression = "dbo.TransCashHandling.ID"
        '        End With

        '        With .Add("SalesTransactionDate", SqlDbType.VarChar)
        '            .Searchable = False
        '            .Expression = "convert(date,TransInvoice.InvoiceDate)"
        '        End With

        '        With .Add("SalesAmount", SqlDbType.Float)
        '            .Expression = "SUM(AmountPaid)"
        '        End With

        '        With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
        '            .Searchable = False
        '            .Expression = "TransInvoice.FowstID"
        '        End With

        '        With .Add("FOWST", SqlDbType.VarChar)
        '            .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
        '        End With

        '        .Add("IsFromTotalAmount", SqlDbType.Bit)

        '        .Add("DepositDate", SqlDbType.DateTime)

        '        With .Add("DepositedTo", SqlDbType.VarChar)
        '            .Expression = "t.DepositTo"
        '        End With

        '        .Add("DepositedToByName", SqlDbType.VarChar)

        '        .Add("DepositedBy", SqlDbType.VarChar)

        '        With .Add("Cashier", SqlDbType.VarChar)
        '            .Expression = "ppl1.Surname + ', ' + ppl1.FirstName "
        '        End With

        '        '.Add("DepositedByName", SqlDbType.UniqueIdentifier)

        '        .Add("TotalCash", SqlDbType.Float)
        '        .Add("TotalCheck", SqlDbType.Float)
        '        .Add("TotalCreditCard", SqlDbType.Float)
        '        .Add("TotalDebitCard", SqlDbType.Float)
        '        .Add("TotalSRV", SqlDbType.Float)
        '        .Add("TotalPettyCash", SqlDbType.Float)

        '        .Add("NoOfFiveCents", SqlDbType.Int)
        '        .Add("NoOfTenCents", SqlDbType.Int)
        '        .Add("NoOfTwentyFiveCents", SqlDbType.Int)
        '        .Add("NoOfOnePeso", SqlDbType.Int)
        '        .Add("NoOfFivePesos", SqlDbType.Int)
        '        .Add("NoOfTenPesos", SqlDbType.Int)
        '        .Add("NoOfTwentyPesos", SqlDbType.Int)
        '        .Add("NoOfFiftyPesos", SqlDbType.Int)
        '        .Add("NoOfOneHundredPesos", SqlDbType.Int)
        '        .Add("NoOfTwoHundredPesos", SqlDbType.Int)
        '        .Add("NoOfFiveHundredPesos", SqlDbType.Int)
        '        .Add("NoOfOneThousandPesos", SqlDbType.Int)

        '        With .Add("TotalAmount", SqlDbType.Float)
        '            .Expression = "TransCashHandling.TotalAmount"
        '        End With

        '        .Add("TotalAcceptedFromPrevTurnOver", SqlDbType.Float)

        '        With .Add("TurnOverVariance", SqlDbType.Float)
        '            .Expression = "ISNULL(SUM(AmountPaid),0) + ISNULL(TotalAcceptedFromPrevTurnOver,0) - ISNULL(TransCashHandling.TotalAmount,0)"
        '        End With
        '    End With

        '    .SqlFrom = "FROM TransCustomerPaymentInvoices " & vbCrLf &
        '    "inner join TransInvoice on TransInvoice.ID = TransCustomerPaymentInvoices.InvoiceID " & vbCrLf &
        '    "left join TransCashHandling on TransCashHandling.DepositedBy = TransInvoice.UserID " & vbCrLf &
        '    "AND CONVERT(date,transinvoice.InvoiceDate) = CONVERT(date,TransCashHandling.DepositDate) " & vbCrLf &
        '    "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID  " & vbCrLf &
        '    "INNER JOIN MobizLibraryPeople ppl1 on ppl1.ID = TransInvoice.UserID " & vbCrLf &
        '    "OUTER APPLY(  " & vbCrLf &
        '    "Select ID, DepositedToByName + ' of ' + CompanyName DepositTo from MobizLibraryOrganization where ID = TransCashHandling.DepositedTo  " & vbCrLf &
        '    "UNION " & vbCrLf &
        '    "Select ID, ppl2.Surname + ', ' + ppl2.FirstName + ' of ' + dbo.MobizLibraryOrganizationFOWST.Name DepositTo from MobizLibraryPeople ppl2 where ppl2.ID =  TransCashHandling.DepositedTo " & vbCrLf &
        '    ") t "

        '    .SqlWhere = "WHERE convert(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
        '                " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
        '                " AND TransInvoice.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID"))

        '    .SqlGroup = "GROUP BY TransCashHandling.ID ,convert(date,TransInvoice.InvoiceDate) ,TransInvoice.UserID ,TransInvoice.FowstID ,MobizLibraryOrganizationFOWST.Name " & vbCrLf &
        '                " ,TransCashHandling.IsFromTotalAmount ,TransCashHandling.DepositDate ,t.DepositTo ,TransCashHandling.DepositedToByName ,TransCashHandling.DepositedBy " & vbCrLf &
        '                " ,TransCashHandling.DepositedByName ,TransCashHandling.TotalCash ,TransCashHandling.TotalCheck ,TransCashHandling.TotalCreditCard " & vbCrLf &
        '                " ,TransCashHandling.TotalDebitCard ,TransCashHandling.TotalSRV ,TransCashHandling.TotalPettyCash , [NoOfFiveCents], [NoOfTenCents] " & vbCrLf &
        '                " , [NoOfTwentyFiveCents], [NoOfOnePeso], [NoOfFivePesos], [NoOfTenPesos], [NoOfTwentyPesos], [NoOfFiftyPesos], [NoOfOneHundredPesos] " & vbCrLf &
        '                " , [NoOfTwoHundredPesos], [NoOfFiveHundredPesos], [NoOfOneThousandPesos], TransCashHandling.TotalAmount, [TotalAcceptedFromPrevTurnOver] " & vbCrLf &
        '                " ,ppl1.Surname, ppl1.FirstName"

        '    If Not Common.WebRequest.Data("EmployeeID") Is Nothing Then
        '        .SqlWhere += " AND TransInvoice.UserID = " & ReadyString(Common.WebRequest.Data("EmployeeID"))
        '    End If

        '    .SqlOrder = "Order by DepositDate desc"

        '    pDT = .Requery
        'End With        
    End Function

End Class

Imports System.IO
Imports Aspose.Cells
Imports Newtonsoft.Json

Public Class DashBoardsAndReports
    Public Property OrgID As String
    Public Property InvoiceID As String
    Public Property FOWSTID As String
    Public Property DateFrom As DateTime
    Public Property DateTo As DateTime
    Public Property IsRawData As Int16
    Public Property GroupBy As String
    Public Property ProductID As String
    Public Property BinID As String
    Public Property SearchString As String
    Public Property ShowSKU As String
    Public Property BatchID As String

    Public Property HasUngroupedInvoice = False
    Public Property lastDay As Date
    Public Property theLimitDate As Date
    Public Property HasExistingCutOff = False
    Public Property prevWeekday As Integer
    Public Property prevTime As String

    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property IsCountPage As Boolean
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String
    Public Property Status As String

#Region "Dashboard"

    'Function GetDashboardsAndReportsByDateAndTime() As DataTable
    '    Dim pFinder As Finder
    '    Dim pDT As DataTable
    '    Dim SqlWhere As String
    '    Dim Fowst As String()
    '    Dim CtrFowst As Integer = 1

    '    pFinder = New Finder

    '    With pFinder
    '        .ProData = Common.WebRequest.ProData

    '        With .Fields
    '            With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
    '                .Searchable = False
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
    '            End With
    '            With .Add("FOWST", SqlDbType.VarChar)
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
    '            End With
    '            With .Add("GrossSale", SqlDbType.VarChar)
    '                .Expression = "SUM(dbo.TransInvoice.TotalAmount)"
    '            End With

    '            If Not WebRequest.Instance.Data("SortedBy") = "Time" Then
    '                With .Add("InvoiceDate", SqlDbType.VarChar)
    '                    .Expression = "CONVERT(date, dbo.TransInvoice.InvoiceDate)"
    '                End With
    '            End If
    '        End With

    '        .SqlFrom = "FROM dbo.TransInvoice INNER JOIN " &
    '                  "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID"

    '        If Not WebRequest.Instance.Data("SortedBy") = "Time" Then
    '            Select Case WebRequest.Instance.Data("Val")
    '                Case "Today"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(Today)
    '                Case "Yesterday"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(DateAdd(DateInterval.Day, -1, Today))
    '                Case "Day"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To"))
    '            End Select
    '        Else
    '            Select Case WebRequest.Instance.Data("Val")
    '                Case "Today"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND dbo.TransInvoice.InvoiceDate BETWEEN " & ReadyString(Today & " " & WebRequest.Instance.Data("From")) & " AND " & ReadyString(Today & " " & WebRequest.Instance.Data("To"))
    '                Case "Yesterday"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND dbo.TransInvoice.InvoiceDate BETWEEN " & ReadyString(DateAdd(DateInterval.Day, -1, Today) & " " & WebRequest.Instance.Data("From")) & " AND " & ReadyString(DateAdd(DateInterval.Day, -1, Today) & " " & WebRequest.Instance.Data("To"))
    '                Case "Day"
    '                    SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND dbo.TransInvoice.InvoiceDate BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To"))
    '            End Select
    '        End If

    '        Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

    '        SqlWhere &= "AND (FowstID = " & ReadyString(Fowst(0))

    '        While CtrFowst <= UBound(Fowst)
    '            If Not Fowst(CtrFowst) = vbNullString Then
    '                SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst))
    '            End If
    '            CtrFowst += 1
    '        End While

    '        .SqlWhere = SqlWhere & ")"

    '        If Not WebRequest.Instance.Data("SortedBy") = "Time" Then
    '            .SqlOrder = "ORDER BY dbo.MobizLibraryOrganizationFOWST.Name, CONVERT(date, dbo.TransInvoice.InvoiceDate)"
    '            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.Name, CONVERT(date, dbo.TransInvoice.InvoiceDate), dbo.MobizLibraryOrganizationFOWST.ID"
    '        Else
    '            .SqlOrder = "ORDER BY dbo.MobizLibraryOrganizationFOWST.Name"
    '            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.Name, dbo.MobizLibraryOrganizationFOWST.ID"
    '        End If

    '        pDT = .Requery

    '    End With

    '    Return pDT

    'End Function

    'by sales value/ sales volume/ hits
    'Function GetPOSSalesReportBySlice() As DataTable
    '    Dim Slice As String
    '    Dim pFinder As Finder
    '    Dim pDT As DataTable
    '    Dim Fowst As String()
    '    Dim CtrFowst As Integer = 1
    '    Dim SqlWhere As String
    '    Dim SqlGroupBy As String

    '    pFinder = New Finder

    '    With pFinder
    '        .ProData = Common.WebRequest.ProData
    '        .Distinct = True

    '        With .Fields
    '            With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
    '                .Searchable = False
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
    '            End With
    '            With .Add("FOWST", SqlDbType.VarChar)
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
    '            End With

    '            Select Case WebRequest.Instance.Data("SliceBy")
    '                Case "optSalesValue"
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
    '                    End With
    '                Case "optSalesVolume"
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
    '                    End With
    '                Case "optHits"
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "Count(dbo.TransInvoice.ID)"
    '                    End With
    '            End Select

    '            Select Case WebRequest.Instance.Data("DiceBy")
    '                Case "optBrand"
    '                    With .Add("ProductFeature", SqlDbType.VarChar)
    '                        .Expression = "dbo.MobizLibraryPIMProduct.Brand"
    '                    End With
    '                    SqlGroupBy += ", dbo.MobizLibraryPIMProduct.Brand"
    '                Case "optDivisionDescription"
    '                    With .Add("ProductFeature", SqlDbType.VarChar)
    '                        .Expression = "dbo.MobizLibraryPIMProduct.DivisionDescription"
    '                    End With
    '                    SqlGroupBy += ", dbo.MobizLibraryPIMProduct.DivisionDescription"
    '            End Select
    '        End With

    '        .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
    '                  "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
    '                  "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
    '                  "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

    '        Select Case WebRequest.Instance.Data("Val")
    '            Case "Today"
    '                SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(Today) & SqlWhere
    '            Case "Yesterday"
    '                SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(DateAdd(DateInterval.Day, -1, Today)) & SqlWhere
    '            Case "Day"
    '                SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere
    '        End Select

    '        Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

    '        SqlWhere += " AND FowstID = " & ReadyString(Fowst(0))

    '        While CtrFowst <= UBound(Fowst)
    '            If Not Fowst(CtrFowst) = vbNullString Then
    '                SqlWhere += " OR FowstID = " & ReadyString(Fowst(CtrFowst))
    '            End If
    '            CtrFowst += 1
    '        End While

    '        .SqlWhere = SqlWhere

    '        .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & SqlGroupBy
    '        pDT = .Requery

    '    End With

    '    Return pDT

    '    Slice = WebRequest.Instance.Data("SliceBy")

    'End Function

    Function GetInventoryLevelApproveDisapproveDetails() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.ID"
                End With
                With .Add("FowstID", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCount.FOWSTID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("Bin", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With
                With .Add("BinID", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.ID"
                End With
                With .Add("CountDate", SqlDbType.Date)
                    .Expression = "TransInventoryProductCount.InventoryCountDate"
                End With
                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.Quantity"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With
                With .Add("InventoryProductCountID", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCount.ID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "MobizLibraryCountriesLanguageLabel.Label"
                End With
                With .Add("Variance", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.Variance"
                End With
            End With

            .SqlFrom = "FROM TransInventoryProductCount " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProduct " & vbCrLf &
                        "   ON MobizLibraryPIMProduct.ID = TransInventoryProductCount.ProductID " & vbCrLf &
                        "LEFT JOIN TransInventoryProductCountStock " & vbCrLf &
                        "   ON TransInventoryProductCountStock.InventoryProductCountID = TransInventoryProductCount.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWST.ID = TransInventoryProductCount.FowstID " & vbCrLf &
                        "LEFT JOIN LibraryFowstBin " & vbCrLf &
                        "   ON LibraryFowstBin.ID = TransInventoryProductCountStock.BinID " & vbCrLf &
                        "LEFT JOIN dbo.MobizLibraryPIMProductMeasurement " & vbCrLf &
                        "   ON TransInventoryProductCount.ProductID = dbo.MobizLibraryPIMProductMeasurement.ProductID " & vbCrLf &
                        "LEFT JOIN dbo.MobizLibraryPIMMeasure " & vbCrLf &
                        "   ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID " & vbCrLf &
                        "LEFT JOIN dbo.MobizLibraryCountriesLanguageLabel " & vbCrLf &
                        "   ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID " & vbCrLf

            .SqlWhere = "WHERE TransInventoryProductCount.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND TransInventoryProductCount.BatchID = " & ReadyString(BatchID) & vbCrLf &
                        " AND IsActionTaken = 0 "

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetInventoryLevelApproveDisapprove() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("BatchID", SqlDbType.Date)
                    .Expression = "TransInventoryProductCount.BatchID"
                End With
                With .Add("InventoryCountDate", SqlDbType.Date)
                    .Expression = "convert(varchar(10),TransInventoryProductCount.InventoryCountDate, 101) + right(convert(varchar(32),TransInventoryProductCount.InventoryCountDate,100),8)"
                End With
                With .Add("NoOfUnapprovedCounts", SqlDbType.VarChar)
                    .Expression = "COUNT(TransInventoryProductCountStock.ID)"
                End With
                With .Add("CountedBy", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName"
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 tipc.Remarks FROM TransInventoryProductCountStock as tipc " & vbCrLf &
                                "INNER JOIN TransInventoryProductCount tip ON tip.ID = tipc.InventoryProductCountID  " & vbCrLf &
                                "WHERE (tipc.Remarks IS NOT NULL OR tipc.Remarks <> '') AND tip.BatchID = TransInventoryProductCount.BatchID " & vbCrLf &
                                "GROUP BY tip.BatchID , tipc.Remarks " & vbCrLf &
                                "ORDER BY tipc.Remarks DESC) "
                End With
            End With

            .SqlFrom = "FROM TransInventoryProductCount " & vbCrLf &
                        "INNER JOIN TransInventoryProductCountStock " & vbCrLf &
                        "   ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWST.ID = TransInventoryProductCount.FowstID " & vbCrLf &
                        "INNER JOIN MobizLibraryPeople " & vbCrLf &
                        "   ON MobizLibraryPeople.ID = TransInventoryProductCount.UserID " & vbCrLf

            .SqlWhere = "WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND IsActionTaken = 0 AND TransInventoryProductCountStock.Variance <> 0 "

            .SqlGroup = "GROUP BY TransInventoryProductCount.BatchID, convert(varchar(10),TransInventoryProductCount.InventoryCountDate, 101) + right(convert(varchar(32),TransInventoryProductCount.InventoryCountDate,100),8), MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName"

            .SqlOrder = "ORDER BY convert(varchar(10),TransInventoryProductCount.InventoryCountDate, 101) + right(convert(varchar(32),TransInventoryProductCount.InventoryCountDate,100),8) DESC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetPOSSalesInvoiceHeader() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        '", SUM(tblItems.Returnables) AS [Returnables] " & vbCrLf &

        pSql = "SELECT TransInvoice.ID AS [ID] " & vbCrLf &
                ", RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12) AS [InvoiceNum] " & vbCrLf &
                ", (CASE MobizLibraryOrganization.IsOrganization WHEN 1 THEN  MobizLibraryOrganization.CompanyName ELSE ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [CompanyName] " & vbCrLf &
                ", TransInvoice.InvoiceDate AS [InvoiceDate] " & vbCrLf &
                ", SUM(tblItems.Total) AS [Total] " & vbCrLf &
                ", SUM(tblItems.SkuCount) AS [SkuCount] " & vbCrLf &
                ", (SELECT COUNT(*) FROM TransMemoSalesReturn WHERE InvoiceID = TransInvoice.ID) as QuantityReturned " & vbCrLf &
                ", MobizLibraryOrganizationFOWST.Name AS [FOWSTName] " & vbCrLf &
                ", LibraryDevice.PhoneName AS [DeviceName] " & vbCrLf &
                ", CASE TransInvoice.FOWSTID WHEN " & ReadyString(FOWSTID) & " THEN 0 ELSE 1 END AS [OrderByFowst] " & vbCrLf &
                ", (SELECT DISTINCT STUFF((SELECT DISTINCT ','+ ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.SurName END,'N/A')FROM TransInvoiceItemSalesReps LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID WHERE TransInvoiceItemSalesReps.InvoiceID =dbo.TransInvoice.ID   FOR XML PATH('')), 1, 1,'')FROM TransInvoiceItemSalesReps) AS [SalesRep] " & vbCrLf &
                ",TransCustomerPayment.CashAmount " & vbCrLf &
                ",TransCustomerPayment.CreditCardAmount " & vbCrLf &
                ",TransCustomerPayment.DebitCardAmount " & vbCrLf &
                ",TransCustomerPayment.CheckPaymentAmount " & vbCrLf &
                ",TransCustomerPayment.SRVAmount " & vbCrLf &
                ",TransCustomerPayment.OtherTypeAmount OtherPayments " & vbCrLf &
                ",TransCustomerPayment.OnCreditAmount " & vbCrLf &
                ", cashier.Surname + ', ' + cashier.FirstName Cashier " & vbCrLf &
                ", ISNULL(TransCustomerPaymentOther.PaymentType,'-') PaymentType  " & vbCrLf

        'If CBool2(Common.WebRequest.Data("IsOtherPayments")) Then
        '    pSql &= ", TransCustomerPaymentOther.PaymentType  " & vbCrLf
        'End If

        pSql &= "FROM TransInvoice " & vbCrLf &
        "   INNER JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
        "       ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
        "   INNER JOIN LibraryDevice " & vbCrLf &
        "       ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
        "   JOIN MobizLibraryOrganization " & vbCrLf &
        "       ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
        " LEFT JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = dbo.TransInvoice.ID " & vbCrLf &
        " LEFT JOIN TransCustomerPayment on TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID " & vbCrLf &
        " LEFT JOIN MobizLibraryPeople cashier on cashier.ID = TransInvoice.UserID " & vbCrLf &
        " LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID " & vbCrLf

        'If CBool2(Common.WebRequest.Data("IsOtherPayments")) Then
        '    pSql &= " LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID " & vbCrLf
        'End If

        If Not CStr2(Common.WebRequest.Data("CashierID")) = vbNullString Then
            pSql &= " AND TransInvoice.UserID = " & ReadyString(CStr2(Common.WebRequest.Data("CashierID")))
        End If

        pSql &= "   CROSS APPLY (" & vbCrLf &
         "           SELECT SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) AS Total " & vbCrLf &
         "           , COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount " & vbCrLf &
         "           FROM TransInvoiceItems " & vbCrLf &
         "       WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
         "       AND TransInvoiceItems.Quantity > 0 " & vbCrLf &
         "   ) AS tblItems" & vbCrLf &
         "WHERE TransInvoice.IsCancelled = 0 " & vbCrLf

        ' change subtotal to GrossSalesNetOfVATExemptions because of item modifiers 09-20-17 francis h.

        '"           , SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) - ISNULL(tblReturns.QuantityReturned, 0) AS Returnables " & vbCrLf &
        '"           OUTER APPLY (" & vbCrLf &
        '"               SELECT SUM(TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity) AS QuantityReturned " & vbCrLf &
        '"               FROM TransMemoSalesReturn " & vbCrLf &
        '"               JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID " & vbCrLf &
        '"               WHERE TransMemo.IsCancelled = 0 " & vbCrLf &
        '"               AND TransMemoSalesReturn.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
        '"               AND TransMemoSalesReturn.InvoiceID = TransInvoiceItems.InvoiceID " & vbCrLf &
        '"               AND TransMemo.ApprovedBy IS NOT NULL " & vbCrLf &
        '"           ) AS tblReturns" & vbCrLf &

        '"       GROUP BY tblReturns.QuantityReturned " & vbCrLf &
        '"       HAVING SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) > ISNULL(tblReturns.QuantityReturned, 0) " & vbCrLf &

        If CBool2(Common.WebRequest.Data("IsOtherPayments")) Then
            pSql &= " AND TransCustomerPaymentOther.PaymentType IS NOT NULL " & vbCrLf
        End If

        If SearchString <> "" Then
            pSql += "AND (TransInvoice.InvoiceNumber LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " OR MobizLibraryOrganization.CompanyName LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " OR MobizLibraryOrganization.PersonSurname LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " OR MobizLibraryOrganization.PersonFirstName LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " OR MobizLibraryOrganization.PersonMiddleName LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " OR MobizLibraryOrganization.PersonSuffixName LIKE " & ReadyStringWild(SearchString) & vbCrLf &
                    " )" & vbCrLf
        End If

        '"AND tblItems.Returnables > 0 " & vbCrLf &
        If Not CStr2(Common.WebRequest.Data("CashierID")) = vbNullString Then
            pSql += "AND TransInvoice.UserID = " & ReadyString(CStr2(Common.WebRequest.Data("CashierID"))) & " "
        End If

        pSql += "AND convert(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(CDate(DateFrom)) & " AND " & ReadyString(CDate(DateTo)) & vbCrLf &
                "AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf &
                "AND TransInvoice.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                " GROUP BY TransInvoice.ID " & vbCrLf &
                "   , RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12) " & vbCrLf &
                "   , (CASE MobizLibraryOrganization.IsOrganization WHEN 1 THEN  MobizLibraryOrganization.CompanyName ELSE ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) " & vbCrLf &
                "   , TransInvoice.FowstID " & vbCrLf &
                "   , TransInvoice.DeviceID " & vbCrLf &
                "   , TransInvoice.InvoiceDate " & vbCrLf &
                "   , MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                "   , LibraryDevice.PhoneName " & vbCrLf &
                "   , TransInvoice.LastUpdated " & vbCrLf &
                ",TransCustomerPayment.CashAmount  " & vbCrLf &
                ",TransCustomerPayment.CreditCardAmount  " & vbCrLf &
                ",TransCustomerPayment.DebitCardAmount  " & vbCrLf &
                ",TransCustomerPayment.CheckPaymentAmount  " & vbCrLf &
                ",TransCustomerPayment.SRVAmount  " & vbCrLf &
                ",TransCustomerPayment.OnCreditAmount  " & vbCrLf &
                ",TransCustomerPayment.OtherTypeAmount " & vbCrLf &
                ",cashier.Surname " & vbCrLf &
                ",cashier.FirstName " & vbCrLf &
                " ,TransCustomerPaymentOther.PaymentType " & vbCrLf

        'If CBool2(Common.WebRequest.Data("IsOtherPayments")) Then
        '    pSql &= " ,TransCustomerPaymentOther.PaymentType " & vbCrLf
        'End If

        pSql &= "ORDER BY " & vbCrLf &
                "   (CASE TransInvoice.FOWSTID  WHEN " & ReadyString(FOWSTID) & " THEN 0 ELSE 1 END) ASC, " & vbCrLf &
                "   TransInvoice.InvoiceDate DESC " & vbCrLf &
                "   , TransInvoice.LastUpdated DESC" & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPOSCashiers() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "Select DISTINCT TransInvoice.UserID, cashier.Surname + ', ' + cashier.FirstName Cashier  FROM TransInvoice " & vbCrLf &
                " INNER JOIN MobizLibraryPeople cashier on cashier.ID = TransInvoice.UserID " & vbCrLf &
                "WHERE TransInvoice.IsCancelled = 0 " & vbCrLf &
                "AND convert(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(CDate(DateFrom)) & " AND " & ReadyString(CDate(DateTo)) & vbCrLf &
                "AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf &
                "AND TransInvoice.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPOSSalesReportBySlice() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim Slice As String
        Dim pColumn As String
        Dim pTableName As String
        Dim IsSalesRep As Boolean

        Slice = WebRequest.Instance.Data("SliceBy")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                If CInt(WebRequest.Instance.Data("IsReport") = 1) Then
                    With .Add("ProductName", SqlDbType.Decimal)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                    End With
                    With .Add("Quantity", SqlDbType.Decimal)
                        .Expression = "dbo.TransInvoiceItems.Quantity"
                    End With
                    With .Add("TotalAmount", SqlDbType.Decimal)
                        .Expression = "(CASE WHEN ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0) = 0 THEN dbo.TransInvoiceItems.SubTotal ELSE dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions END)"
                    End With
                    With .Add("InvoiceNumber", SqlDbType.Decimal)
                        .Expression = "dbo.TransInvoice.InvoiceNumber"
                    End With
                Else
                    With .Add("Total", SqlDbType.Decimal)
                        .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                    End With
                    With .Add("TransactionCount", SqlDbType.Int)
                        .Expression = "COUNT(dbo.TransInvoice.ID)"
                    End With
                    With .Add("ProductCount", SqlDbType.Decimal)
                        .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
                    End With
                    With .Add("ProductCountSO", SqlDbType.Decimal)
                        .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
                    End With
                End If


                Select Case Slice
                    Case "optBrand"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "dbo.MobizLibraryPIMProduct.Brand"
                        End With
                        SqlGroupBy &= ", dbo.MobizLibraryPIMProduct.Brand"
                        '12-15-2022 Francis H.
                    Case "optCustomerIndustry"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.MobizLibraryIndustryClass.Class,'N/A')"
                        End With

                        SqlFrom &= "INNER JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = Transinvoice.CustomerID " & vbCrLf &
                                    "LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID"

                        SqlGroupBy &= ", dbo.MobizLibraryIndustryClass.Class"
                    Case "optSalesChannel"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.MobizLibrarySalesChannels.Name,'N/A')"
                        End With

                        SqlFrom &= "INNER JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = Transinvoice.CustomerID " & vbCrLf &
                                    "INNER JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = MobizLibraryorganization.ID " & vbCrLf &
                                    "LEFT JOIN MobizLibrarySalesChannels ON MobizLibrarySalesChannels.ID = LibraryOrganizationCustomer.SalesChannelID " & vbCrLf

                        SqlGroupBy &= ", dbo.MobizLibrarySalesChannels.Name"

                    Case "optDivisionDescription"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "dbo.MobizLibraryPIMProduct.DivisionDescription"
                        End With
                        SqlGroupBy &= ", dbo.MobizLibraryPIMProduct.DivisionDescription"
                    Case "optSalesRepresentative"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName END,'N/A')"
                        End With
                        SqlGroupBy &= ", MobizLibraryPeople.SurName, MobizLibraryPeople.FirstName "
                        IsSalesRep = True
                        'additional by janine oct 30 2014
                    Case "optProduct"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.MobizLibraryPIMProduct.ProductName, 'N/A')"
                        End With

                        With .Add("Category", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0020, 'N/A')"
                        End With

                        SqlFrom &= " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 ON dbo.TransInvoiceItems.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID "

                        SqlGroupBy &= ", dbo.MobizLibraryPIMProduct.ProductName, dbo.MobizLibraryPIMProductVariableFeatures001.C0020 "
                    Case "optDeliveryMethod"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.TransInvoice.DeliveryMethod, 'Not Provided')"
                        End With
                        SqlGroupBy &= ", dbo.TransInvoice.DeliveryMethod"
                    Case "optOrderMethod"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.TransInvoice.OrderMethod, 'Not Provided')"
                        End With
                        SqlGroupBy &= ", dbo.TransInvoice.OrderMethod"
                    Case "optGender"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.TransInvoice.CustomerDemographicGender, 'Not Provided')"
                        End With
                        SqlGroupBy &= ", dbo.TransInvoice.CustomerDemographicGender"
                    Case "optAge"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(dbo.TransInvoice.CustomerDemographicAge, 'Not Provided')"
                        End With
                        SqlGroupBy &= ", dbo.TransInvoice.CustomerDemographicAge"
                        ' end of additional by janine oct 30 2014
                    Case "optTime"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00'"
                        End With

                        SqlGroupBy = ", DATEPART(hh, InvoiceDate)"
                    Case "Encoder"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "EncodedBy.Surname + ', ' + EncodedBy.FirstName"
                        End With

                        SqlFrom = "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                        SqlGroupBy = ", EncodedBy.Surname + ', ' + EncodedBy.FirstName "
                    Case "OtherPayments"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "TransCustomerPaymentOther.PaymentType"
                        End With

                        SqlFrom = "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                        SqlGroupBy = ", TransCustomerPaymentOther.PaymentType "
                    Case "PaymentType"

                        SqlFrom = "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                        SqlGroupBy = ", EncodedBy.Surname + ', ' + EncodedBy.FirstName "
                    Case Else

                        pColumn = GetFeatureColumnName(Slice, pTableName)

                        pTableName = "Mobiz" & pTableName

                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')" '"ISNULL(dbo." & pTableName & "." & pColumn & ",'N/A')"
                        End With
                        SqlFrom = "LEFT JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                        SqlGroupBy &= ", " & "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')"

                End Select
            End With

            SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere

            If Not WebRequest.Instance.Data("Filter1") = vbNullString Then
                Select Case WebRequest.Instance.Data("Filter1Option")
                    Case "Time"
                        SqlWhere &= " AND DATEPART(hh, InvoiceDate) = " & ReadyString(Replace(WebRequest.Instance.Data("Filter1"), ":00", ""))
                    Case "Brand"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                    Case "Division"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.DivisionDescription = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                    Case "SalesRepresentative"
                        If IsSalesRep = True Then
                            If WebRequest.Instance.Data("Filter1") = "N/A" Then
                                SqlWhere &= " AND (MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName IS NULL) "
                            Else
                                SqlWhere &= " AND (MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName = " & ReadyString(WebRequest.Instance.Data("Filter1")) & ")"
                            End If
                        End If
                        'additional by janine oct 30
                    Case "Product"
                        SqlWhere &= " AND ISNULL(dbo.MobizLibraryPIMProduct.ProductName, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'N/A' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Delivery Method"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.DeliveryMethod, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Order Method"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.OrderMethod, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Gender"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicGender, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Age"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicAge, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                        'end of additional
                    Case "Encoder"

                        SqlWhere &= " AND EncodedBy.Surname + ', ' + EncodedBy.FirstName  LIKE " & ReadyStringWild(WebRequest.Instance.Data("Filter1"))

                        SqlFrom &= "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID"

                    Case "OtherPayments"

                        SqlWhere &= " AND TransCustomerPaymentOther.PaymentType like " & ReadyStringWild(WebRequest.Instance.Data("Filter1"))

                        SqlFrom &= "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                    Case Else
                        pColumn = GetFeatureColumnName(WebRequest.Instance.Data("Filter1Option"), pTableName)

                        pTableName = "Mobiz" & pTableName

                        If SqlFrom Is Nothing Then
                            SqlFrom = "LEFT JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                        End If

                        If WebRequest.Instance.Data("Filter1") = "N/A" Then
                            SqlWhere &= " AND (" & pColumn & " = " & ReadyString(WebRequest.Instance.Data("Filter1")) & " OR " & pColumn & " = '')"
                        Else
                            SqlWhere &= " AND " & pColumn & " = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                        End If
                End Select
            End If

            If IsSalesRep Then
                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                          "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                "LEFT join TransInvoiceItemSalesReps on transinvoiceitems.InvoiceID = TransInvoiceItemSalesReps.InvoiceID " & vbCrLf &
                "AND transinvoiceitems.ProductID = TransInvoiceItemSalesReps.ProductID " & vbCrLf &
                "LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID " & vbCrLf &
                SqlFrom & vbCrLf
            Else
                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                    "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                    "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                    "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                SqlFrom & vbCrLf
            End If

            Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

            SqlWhere &= " AND TransInvoice.FowstID = " & ReadyString(Fowst(0))

            While CtrFowst <= UBound(Fowst)
                If Not Fowst(CtrFowst) = vbNullString Then
                    SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst)) & vbCrLf
                End If
                CtrFowst += 1
            End While

            .SqlWhere = SqlWhere

            If CInt(WebRequest.Instance.Data("IsReport") = 1) Then

            Else
                .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & vbCrLf & SqlGroupBy
                .SqlOrder = "Order by Total"
            End If

            pDT = .Requery

            If CInt(WebRequest.Instance.Data("IsReport") = 1) Then

            Else
                Return pDT
            End If
        End With

    End Function

    Public Function GetPOSSalesReportByCustomer() As DataTable

        Dim pFinder As New Finder()
        Dim pCompanyNameSelect As String
        Dim pCompanyNameFrom As String

        BrowserController.ComposeCustomer(pCompanyNameSelect, pCompanyNameFrom)

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransInvoice.FowstID"
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With
                .Add("CompanyName", SqlDbType.VarChar, pCompanyNameSelect)
                .Add("TotalAmount", SqlDbType.Money, "SUM(TransInvoice.TotalAmount)")
                .Add("AdjustmentAmount", SqlDbType.Money, "SUM(CacheInvoices.AdjustmentAmount)")
                .Add("NetInvoiceAmount", SqlDbType.Money, "SUM(CacheInvoices.NetInvoiceAmount)")
                .Add("TotalPayment", SqlDbType.Money, "SUM(CacheInvoices.TotalPayment)")
                .Add("NetInvoiceBalance", SqlDbType.Money, "SUM(CacheInvoices.NetInvoiceBalance)")
            End With

            .SqlFrom = " FROM CacheInvoices" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = CacheInvoices.ID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
                pCompanyNameFrom
            .SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0)" & vbCrLf &
                " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & vbCrLf &
                ""
            .SqlGroup = " GROUP BY TransInvoice.FowstID" &
                ",MobizLibraryOrganizationFOWST.Name" & vbCrLf &
                ",MobizLibraryOrganization.IsOrganization" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName" & vbCrLf &
                ",MobizLibraryPeoplePreNominalSuffix.Code" & vbCrLf &
                ",MobizLibraryOrganization.PersonSurname" & vbCrLf &
                ",MobizLibraryOrganization.PersonFirstName" & vbCrLf &
                ",MobizLibraryOrganization.PersonMiddleName" & vbCrLf &
                ",MobizLibraryOrganization.PersonSuffixName" & vbCrLf

            Return .Requery()
        End With

    End Function

    Function GetFeatureColumnName(ByVal pLabel As String, ByRef TableName As String)
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                .Add("PIMFeatureTable", SqlDbType.VarChar)
                .Add("PIMFeatureColumn", SqlDbType.VarChar)
                .Add("Label", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM dbo.MobizLibraryCountriesLanguageLabel INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID "

            .SqlWhere = "where Label = " & ReadyString(pLabel)

            pDt = .Requery
        End With

        If pDt.Rows.Count > 0 Then
            TableName = CStr(pDt.Rows(0).Item("PIMFeatureTable"))

            If CStr(pDt.Rows(0).Item("PIMFeatureColumn")) = vbNullString Then
                Return pLabel
            End If
        End If

        Return CStr(pDt.Rows(0).Item("PIMFeatureColumn"))


    End Function

    Function GetPOSSalesReportByTrendOrSalesDistribution() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        pFinder = New Finder

        With pFinder

            If CBool(IsRawData) Then
                .Distinct = True
            End If

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                If CBool(IsRawData) Then
                    With .Add("InvoiceItemsID", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItems.ID"
                    End With
                    With .Add("InvoiceID", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoice.ID"
                    End With
                    With .Add("InvoiceNo", SqlDbType.VarChar)
                        .Expression = "RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12)"
                    End With
                    With .Add("InvoiceDate", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoice.InvoiceDate"
                    End With
                    With .Add("ProductDescription", SqlDbType.VarChar)
                        .Expression = "ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')"
                    End With
                    With .Add("Qty", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItems.Quantity"
                    End With
                    With .Add("UnitMeasure", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItems.UnitMeasure"
                    End With
                    With .Add("Price", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItems.UnitPrice"
                    End With
                    With .Add("SubTotal", SqlDbType.VarChar)
                        '.Expression = "dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions"
                        'change to this expression because GrossSalesNetOfVATExemptions is computed together with item modifiers which will make computation for line items wrong
                        .Expression = "dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.Quantity"
                    End With
                    With .Add("Discount", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItems.DiscountAmount"
                    End With
                    With .Add("SalesRepresentative", SqlDbType.VarChar)
                        .Expression = "(SELECT DISTINCT STUFF((SELECT DISTINCT ','+ ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.SurName END,'N/A')" &
                            "FROM TransInvoiceItemSalesReps LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" &
                            " WHERE TransInvoiceItemSalesReps.ProductID=dbo.TransInvoiceItems.ProductID and TransInvoiceItemSalesReps.InvoiceID =dbo.TransInvoice.ID " &
                            "  FOR XML PATH('')" &
                            "), 1, 1,'')" &
                            "FROM TransInvoiceItemSalesReps)"
                    End With
                    With .Add("Customer", SqlDbType.VarChar)
                        .Expression = "(CASE WHEN MobizLibraryOrganization.CompanyName  = '' THEN MobizLibraryOrganization.PersonSurname + ', ' + MobizLibraryOrganization.PersonFirstName ELSE MobizLibraryOrganization.CompanyName END)"
                    End With
                    With .Add("RebatesID", SqlDbType.VarChar)
                        .Expression = "TransInvoiceItemsRebates.ProductID"
                    End With
                    With .Add("RebatesProduct", SqlDbType.VarChar)
                        .Expression = "ISNULL(p1.ProductGroupNo,'') + ' - ' + ISNULL(p1.ProductNo,'') + ' - ' + ISNULL(p1.ProductName,'')"
                    End With
                    With .Add("QtyRebates", SqlDbType.VarChar)
                        .Expression = "TransInvoiceItemsRebates.Quantity"
                    End With
                    With .Add("UnitMeasureRebates", SqlDbType.VarChar)
                        .Expression = "TransInvoiceItemsRebates.UnitMeasure"
                    End With
                    With .Add("MemoNumber", SqlDbType.VarChar)
                        .Expression = "TransMemo.MemoNumber"
                    End With
                    With .Add("QuantityReturned", SqlDbType.VarChar)
                        .Expression = "TransMemoSalesReturn.QuantityReturned"
                    End With
                    With .Add("UpdatedByName", SqlDbType.VarChar)
                        .Expression = BrowserController.ComposePeopleName("tblUpdater", "tblUpdaterSuffix")
                    End With

                    SqlFrom = "LEFT join TransInvoiceItemSalesReps on transinvoiceitems.InvoiceID = TransInvoiceItemSalesReps.InvoiceID " & vbCrLf &
                        "AND transinvoiceitems.ProductID = TransInvoiceItemSalesReps.ProductID " & vbCrLf &
                        "LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID " &
                        "LEFT join MobizLibraryOrganization on MobizLibraryOrganization.ID = TransInvoice.CustomerID " &
                        "left join TransInvoiceItemsRebates on TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID " &
                        "LEFT JOIN dbo.MobizLibraryPIMProduct p1 ON dbo.TransInvoiceItemsRebates.ProductID = p1.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople tblUpdater ON tblUpdater.ID = TransInvoice.UserID" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblUpdaterSuffix ON tblUpdater.TitleID = tblUpdaterSuffix.ID" & vbCrLf &
                        " LEFT JOIN TransMemoSalesReturn ON TransMemoSalesReturn.InvoiceID = TransInvoice.ID AND TransMemoSalesReturn.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                        "   LEFT JOIN TransMemo ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf

                    If Not Common.WebRequest.Data("IsSearch") Is Nothing Then
                        If CBool(Common.WebRequest.Data("IsSearch")) Then
                            SqlWhere = " AND ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')  " &
                                "  + ' - ' + CAST(TransInvoice.InvoiceNumber AS VARCHAR) + ' - ' + ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName END,'N/A') " &
                                "  + ' - ' +  (CASE WHEN MobizLibraryOrganization.CompanyName  = '' THEN MobizLibraryOrganization.PersonSurname + ', ' + MobizLibraryOrganization.PersonFirstName ELSE MobizLibraryOrganization.CompanyName END) like " & ReadyStringWild(Common.WebRequest.Data("SearchString"))
                        End If
                    End If

                    'SqlOrderBy = "ORDER BY InvoiceNumber DESC, InvoiceDate DESC" ' edited by janine 
                    SqlOrderBy = "ORDER BY RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12) DESC, InvoiceDate DESC" ' edited by francis
                Else
                    Select Case GroupBy
                        Case "Time"
                            With .Add("InvoiceDate", SqlDbType.VarChar)
                                .Expression = "DATEPART(hour, InvoiceDate)"
                                SqlOrderBy = "ORDER BY DATEPART(hour, InvoiceDate)"
                            End With
                            With .Add("Total", SqlDbType.Decimal)
                                .Expression = "SUM(TotalAmount)"
                            End With
                            SqlGroupBy = ", DATEPART(hour, InvoiceDate)"
                        Case "Day"
                            With .Add("InvoiceDate", SqlDbType.VarChar)
                                .Expression = "CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                                SqlOrderBy = "ORDER BY CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                            End With
                            With .Add("Total", SqlDbType.Decimal)
                                .Expression = "SUM(dbo.TransInvoiceItems.Subtotal)"
                            End With
                            With .Add("DiscountTotal", SqlDbType.Decimal)
                                .Expression = "SUM(dbo.TransInvoiceItems.DiscountAmount) + tblSCDiscount.SCDiscount "
                            End With
                            With .Add("ForegoneVAT", SqlDbType.VarChar)
                                .Expression = "VATExempt.ForegoneVAT"
                            End With
                            With .Add("SalesReturn", SqlDbType.VarChar)
                                .Expression = "ISNULL(tblReturns.TotalReturns,0)"
                            End With
                            With .Add("ChargeAmount", SqlDbType.VarChar)
                                .Expression = "ISNULL(Charges.ChargeAmount,0)"
                            End With
                            SqlGroupBy = ", Charges.ChargeAmount"
                            SqlGroupBy += ", CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                        Case "Month"
                            With .Add("InvoiceDate", SqlDbType.VarChar)
                                .Expression = "Month(dbo.TransInvoice.InvoiceDate)"
                                SqlOrderBy = "ORDER BY Month(dbo.TransInvoice.InvoiceDate)"
                            End With
                            With .Add("Total", SqlDbType.Decimal)
                                .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                            End With
                            With .Add("DiscountTotal", SqlDbType.Decimal)
                                .Expression = "SUM(dbo.TransInvoiceItems.DiscountAmount) + tblSCDiscount.SCDiscount"
                            End With
                            With .Add("ForegoneVAT", SqlDbType.VarChar)
                                .Expression = "VATExempt.ForegoneVAT"
                            End With
                            SqlGroupBy = ", Month(dbo.TransInvoice.InvoiceDate)"

                    End Select
                End If
            End With

            Select Case WebRequest.Instance.Data("GroupBy")
                Case "Time"
                    .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                                "dbo.TransInvoice ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInvoice.FowstID"
                Case Else
                    .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                              "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                              "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                              "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & SqlFrom &
                              " outer apply (Select SUM(SCDiscount) SCDiscount from Transinvoice inv " & vbCrLf &
                              " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                              " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                              " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ") tblSCDiscount " & vbCrLf &
                              " outer apply (" & vbCrLf &
                              "Select SUM(ForegoneVAT) as ForegoneVAT from TransInvoice INV " & vbCrLf &
                              " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                              " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                              " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) &
                                ") VATExempt outer apply (  " & vbCrLf &
                                "SELECT SUM(TotalAmount) AS [TotalReturns], COUNT(TransMemo.ID) [SalesReturnsCount] " & vbCrLf &
                                "FROM TransSalesReturnLedger " & vbCrLf &
                                "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                                "inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                                "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND " & vbCrLf &
                                " convert(date,TransMemo.MemoDate) = convert(date,TransInvoice.InvoiceDate) " & vbCrLf &
                                " and TransMemo.FowstID = TransInvoice.FowstID " & vbCrLf &
                                ") tblReturns " & vbCrLf &
                                "outer apply (" & vbCrLf &
                                "SELECT SUM(ROUND(ISNULL(custpay.ChargeAmount,0), 2)) AS ChargeAmount " & vbCrLf &
                                " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                                " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                                " INNER JOIN dbo.CacheInvoices inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                                " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                                " AND inv.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                                ") Charges "

                    '" convert(date,TransMemo.MemoDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
            End Select

            SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & SqlWhere

            'Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

            strWhereFowst = " AND (dbo.TransInvoice.FowstID = " & ReadyString(FOWSTID)


            'While CtrFowst <= UBound(Fowst)
            '    If Not Fowst(CtrFowst) = vbNullString Then
            '        strWhereFowst &= " OR dbo.TransInvoice.FowstID = " & ReadyString(Fowst(CtrFowst))
            '    End If
            '    CtrFowst += 1
            'End While
            strWhereFowst &= ")"

            If ShowSKU = "No" Then
                strWhereFowst = " AND dbo.TransInvoice.ID = " & ReadyString(InvoiceID)
            End If

            SqlWhere &= strWhereFowst

            .SqlWhere = SqlWhere

            If Not SqlOrderBy = vbNullString Then
                .SqlOrder = SqlOrderBy
            End If

            If Not CBool(IsRawData) Then
                .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name, tblSCDiscount.SCDiscount, VATExempt.ForegoneVAT, tblReturns.TotalReturns " & SqlGroupBy
            End If

            Try
                pDT = .Requery
            Catch ex As Exception
                AppendToDebug("vb3FJ79X6APU! " & ex.Message & vbCrLf & "SQL: " & .CreateSql())
                Throw
            End Try

        End With

        Return pDT
    End Function

    Function GetPOSSalesInvoiceItemModifiers(vDTInvoiceItems As DataTable) As DataTable
        Dim pFinder As Finder
        Dim pDTModifiers As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String

        Dim dtMainTable As DataTable

        dtMainTable = vDTInvoiceItems.Clone

        For i As Integer = 0 To vDTInvoiceItems.Rows.Count - 1

            Dim dr As DataRow

            dr = dtMainTable.NewRow

            dr(0) = vDTInvoiceItems.Rows(i).Item(0)
            dr(1) = vDTInvoiceItems.Rows(i).Item(1)
            dr(2) = vDTInvoiceItems.Rows(i).Item(2)
            dr(3) = vDTInvoiceItems.Rows(i).Item(3)
            dr(4) = vDTInvoiceItems.Rows(i).Item(4)
            dr(5) = vDTInvoiceItems.Rows(i).Item(5)
            dr(6) = vDTInvoiceItems.Rows(i).Item(6)
            dr(7) = vDTInvoiceItems.Rows(i).Item(7)
            dr(8) = vDTInvoiceItems.Rows(i).Item(8)
            dr(9) = vDTInvoiceItems.Rows(i).Item(9)
            dr(10) = vDTInvoiceItems.Rows(i).Item(10)
            dr(11) = vDTInvoiceItems.Rows(i).Item(11)
            dr(12) = vDTInvoiceItems.Rows(i).Item(12)
            dr(13) = vDTInvoiceItems.Rows(i).Item(13)
            dr(14) = vDTInvoiceItems.Rows(i).Item(14)
            dr(15) = vDTInvoiceItems.Rows(i).Item(15)
            dr(16) = vDTInvoiceItems.Rows(i).Item(16)
            dr(17) = vDTInvoiceItems.Rows(i).Item(17)
            dr(18) = vDTInvoiceItems.Rows(i).Item(18)
            dr(19) = vDTInvoiceItems.Rows(i).Item(19)
            dr(20) = vDTInvoiceItems.Rows(i).Item(20)


            dtMainTable.Rows.Add(dr)

            pFinder = New Finder

            With pFinder

                .ProData = Common.WebRequest.ProData

                With .Fields

                    With .Add("ProductDescription", SqlDbType.VarChar)
                        .Expression = "ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')"
                    End With
                    With .Add("Qty", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItemsModifier.OrderedQty"
                    End With
                    With .Add("UnitMeasure", SqlDbType.VarChar)
                        .Expression = "dbo.LibraryItemModifierInventoryAffecting.UnitMeasure"
                    End With
                    With .Add("Price", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItemsModifier.ModifierPrice" 'dbo.LibraryItemModifierInventoryAffecting.UnitPrice updated by arnie 6-9-2020, price mismo sa natransact na modifier
                    End With
                    With .Add("SubTotal", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoiceItemsModifier.SubTotal"
                    End With

                End With

                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                          "TransInvoiceItemsModifier ON TransInvoiceItemsModifier.InvoiceItemID = TransInvoiceItems.ID INNER JOIN " & vbCrLf &
                          "LibraryItemModifierInventoryAffecting ON LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryPIMProduct ON dbo.LibraryItemModifierInventoryAffecting.ProductID = dbo.MobizLibraryPIMProduct.ID "

                SqlWhere = " WHERE TransInvoiceItems.ID IN (" & ReadyString(CStr2(vDTInvoiceItems.Rows(i).Item("InvoiceItemsID"))) & ")"

                .SqlWhere = SqlWhere

                pDTModifiers = .Requery

            End With

            If pDTModifiers.Rows.Count > 0 Then

                For j As Integer = 0 To pDTModifiers.Rows.Count - 1
                    dr = dtMainTable.NewRow

                    dr(0) = vDTInvoiceItems.Rows(i).Item(0)
                    dr(1) = vDTInvoiceItems.Rows(i).Item(1)
                    dr(2) = vDTInvoiceItems.Rows(i).Item(2)
                    dr(3) = vDTInvoiceItems.Rows(i).Item(3)
                    dr(4) = vDTInvoiceItems.Rows(i).Item(4)
                    dr(5) = vDTInvoiceItems.Rows(i).Item(5)
                    dr(6) = "*** " & pDTModifiers.Rows(j).Item(0)
                    dr(7) = pDTModifiers.Rows(j).Item(1)
                    dr(8) = pDTModifiers.Rows(j).Item(2)
                    dr(9) = pDTModifiers.Rows(j).Item(3)
                    dr(10) = pDTModifiers.Rows(j).Item(4)
                    dr(11) = vDTInvoiceItems.Rows(i).Item(11)
                    dr(12) = vDTInvoiceItems.Rows(i).Item(12)
                    dr(13) = vDTInvoiceItems.Rows(i).Item(13)
                    dr(14) = vDTInvoiceItems.Rows(i).Item(14)
                    dr(15) = vDTInvoiceItems.Rows(i).Item(15)
                    dr(16) = vDTInvoiceItems.Rows(i).Item(16)
                    dr(17) = vDTInvoiceItems.Rows(i).Item(17)
                    dr(18) = vDTInvoiceItems.Rows(i).Item(18)
                    dr(19) = vDTInvoiceItems.Rows(i).Item(19)
                    dr(20) = vDTInvoiceItems.Rows(i).Item(20)

                    dtMainTable.Rows.Add(dr)
                Next
            End If
        Next

        Return dtMainTable
    End Function

    Function GetDetailPOSSalesReportByDateAndTime() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("FOWST", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("InvoiceNumber", SqlDbType.VarChar)
                    .Expression = "dbo.TransInvoice.InvoiceNumber"
                End With
                With .Add("Date", SqlDbType.Date)
                    .Expression = "dbo.TransInvoice.InvoiceDate"
                End With
                With .Add("Price", SqlDbType.Float)
                    .Expression = "dbo.TransInvoiceItems.UnitPrice"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "dbo.TransInvoiceItems.Quantity"
                End With
                With .Add("SubTotal", SqlDbType.Float)
                    .Expression = "dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions"
                End With
                With .Add("SalesPerson", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPeople.Surname + ', ' + dbo.MobizLibraryPeople.FirstName"
                End With

                'If WebRequest.Instance.Data("SortedBy") = "ProductClass" Then
                '    With .Add("ProductName", SqlDbType.VarChar)
                '        .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                '    End With
                'End If
            End With

            If WebRequest.Instance.Data("SortedBy") = "ProductClass" Then
                .SqlFrom = "FROM dbo.TransInvoiceItemSalesReps INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPeople ON dbo.TransInvoiceItemSalesReps.PeopleID = dbo.MobizLibraryPeople.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoice INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoiceItems ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = dbo.TransInvoiceItems.ProductID ON " & vbCrLf &
                      "dbo.TransInvoiceItemSalesReps.InvoiceItemID = dbo.TransInvoiceItems.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProductFeature ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductFeature.ProductID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryUNSPSCCommodityFeature ON dbo.MobizLibraryPIMProductFeature.CommodityFeatureID = dbo.MobizLibraryUNSPSCCommodityFeature.ID"
            Else
                .SqlFrom = "FROM dbo.TransInvoiceItemSalesReps INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPeople ON dbo.TransInvoiceItemSalesReps.PeopleID = dbo.MobizLibraryPeople.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoice INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoiceItems ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = dbo.TransInvoiceItems.ProductID ON " & vbCrLf &
                      "dbo.TransInvoiceItemSalesReps.InvoiceItemID = dbo.TransInvoiceItems.ID"
            End If

            SqlWhere = "WHERE dbo.TransInvoice.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))

            If WebRequest.Instance.Data("SortedBy") = "Time" Then
                SqlWhere &= " AND dbo.TransInvoice.InvoiceDate BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To"))
            Else
                SqlWhere &= " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(CDate(WebRequest.Instance.Data("ReportDate")))
            End If

            .SqlWhere = SqlWhere

            .SqlOrder = "ORDER BY dbo.TransInvoice.InvoiceDate"

            pDT = .Requery

        End With

        Return pDT
    End Function

    'Function GetPOSSalesReportBySalesDistributionByTime() As DataTable
    '    Dim pFinder As Finder
    '    Dim pDT As DataTable
    '    Dim SqlWhere As String
    '    Dim SqlGroupBy As String
    '    Dim Fowst As String()
    '    Dim CtrFowst As Integer = 1

    '    pFinder = New Finder

    '    With pFinder
    '        .ProData = Common.WebRequest.ProData
    '        .Distinct = True

    '        With .Fields
    '            With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
    '                .Searchable = False
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
    '            End With

    '            With .Add("FOWST", SqlDbType.VarChar)
    '                .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
    '            End With

    '            Select Case WebRequest.Instance.Data("GroupBy")
    '                Case "Time"
    '                    With .Add("InvoiceDate", SqlDbType.VarChar)
    '                        .Expression = "DATEPART(hh, InvoiceDate)"
    '                    End With
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "SUM(TotalAmount)"
    '                    End With
    '                    SqlGroupBy = ", DATEPART(hh, InvoiceDate)"
    '                Case "Day"
    '                    With .Add("InvoiceDate", SqlDbType.VarChar)
    '                        .Expression = "CONVERT(date, dbo.TransInvoice.InvoiceDate)"
    '                    End With
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
    '                    End With
    '                    SqlGroupBy = ", CONVERT(date, dbo.TransInvoice.InvoiceDate)"
    '                Case "Month"
    '                    With .Add("InvoiceDate", SqlDbType.VarChar)
    '                        .Expression = "Month(dbo.TransInvoice.InvoiceDate)"
    '                    End With
    '                    With .Add("Total", SqlDbType.Decimal)
    '                        .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
    '                    End With
    '                    SqlGroupBy = ", Month(dbo.TransInvoice.InvoiceDate)"

    '            End Select

    '            'Select Case WebRequest.Instance.Data("DataType")
    '            '    Case "SalesValue"
    '            '        With .Add("Total", SqlDbType.Decimal)
    '            '            .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
    '            '        End With
    '            '    Case "SalesVolume"
    '            '        With .Add("Total", SqlDbType.Decimal)
    '            '            .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
    '            '        End With
    '            '    Case "Hits"
    '            '        With .Add("Total", SqlDbType.Decimal)
    '            '            .Expression = "Count(dbo.TransInvoice.ID)"
    '            '        End With
    '            'End Select



    '        End With

    '        .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
    '                  "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
    '                  "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
    '                  "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

    '        'Select Case WebRequest.Instance.Data("Val")
    '        '    Case "Today"
    '        '        SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(Today) & SqlWhere
    '        '    Case "Yesterday"
    '        '        SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) = " & ReadyString(DateAdd(DateInterval.Day, -1, Today)) & SqlWhere
    '        '    Case "Day"
    '        '        SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere
    '        'End Select

    '        SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere

    '        Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

    '        SqlWhere &= " AND FowstID = " & ReadyString(Fowst(0))

    '        'For Each strFowst As String In Fowst
    '        '    If Not strFowst = vbNullString Then
    '        '        SqlWhere += " OR FowstID = " & ReadyString(strFowst)
    '        '    End If
    '        'Next

    '        While CtrFowst <= UBound(Fowst)
    '            If Not Fowst(CtrFowst) = vbNullString Then
    '                SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst))
    '            End If
    '            CtrFowst += 1
    '        End While

    '        .SqlWhere = SqlWhere

    '        .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & SqlGroupBy
    '        pDT = .Requery

    '    End With

    '    Return pDT
    'End Function

    Function DownloadSliceRawData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim Slice As String
        Dim pColumn As String
        Dim pTableName As String

        Slice = WebRequest.Instance.Data("SliceBy")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("InvoiceNo", SqlDbType.Decimal)
                    .Expression = "dbo.TransInvoice.InvoiceNo"
                End With

                With .Add("Total", SqlDbType.Decimal)
                    .Expression = "dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions"
                End With

                Select Case Slice
                    Case "optBrand"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "dbo.MobizLibraryPIMProduct.Brand"
                        End With
                    Case "optDivisionDescription"
                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "dbo.MobizLibraryPIMProduct.DivisionDescription"
                        End With
                    Case Else

                        pColumn = GetFeatureColumnName(Slice, pTableName)

                        pTableName = "Mobiz" & pTableName

                        With .Add("ProductFeature", SqlDbType.VarChar)
                            .Expression = "dbo." & pTableName & "." & pColumn
                        End With
                        SqlFrom = "INNER JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                        SqlWhere &= " AND " & pColumn & " IS NOT NULL"
                End Select
            End With

            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                      SqlFrom

            SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere
            'End Select

            If Not WebRequest.Instance.Data("Filter1") = vbNullString Then
                Select Case WebRequest.Instance.Data("Filter1Option")
                    Case "optBrand"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                    Case "optDivisionDescription"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.DivisionDescription = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                End Select
            End If

            Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

            SqlWhere &= " AND FowstID = " & ReadyString(Fowst(0))

            While CtrFowst <= UBound(Fowst)
                If Not Fowst(CtrFowst) = vbNullString Then
                    SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst))
                End If
                CtrFowst += 1
            End While

            .SqlWhere = SqlWhere

            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & SqlGroupBy
            .SqlOrder = "Order by Total"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetSqlWhereString(ByVal Dice As String, ByVal Mince As String) As String
        If Not Dice = vbNullString Then
            Select Case Dice
                Case "optBrand"
                    Return " AND dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(Mince)
                Case "optDivisionDescription"
                    Return " AND dbo.MobizLibraryPIMProduct.DivisionDescription = " & ReadyString(Mince)
            End Select
        End If
    End Function

    Function GetTotalGrossAndDiscounts() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTSalesReturn As DataTable
        Dim SqlWhere As String
        Dim SqlWhereSalesReturns As String
        Dim SqlGroupBySalesReturns As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()

        pFinder = New Finder

        If IsFranchisor() Then
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields

                    Select Case GroupBy
                        Case "ByOrganizationGroup"
                            With .Add("GroupID", SqlDbType.VarChar)
                                .Expression = "FranchiseGroupID"
                            End With
                            With .Add("GroupName", SqlDbType.VarChar)
                                .Expression = "ISNULL(GroupName,'Unassigned')"
                            End With
                            SqlGroupBy = " FranchiseGroupID, GroupName"
                    End Select

                    With .Add("GrossSales", SqlDbType.Decimal)
                        .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                    End With

                    With .Add("TotalDiscounts", SqlDbType.Decimal)
                        .Expression = "SUM(ROUND(Discount, 2))"
                    End With

                    With .Add("TotalSrv", SqlDbType.VarChar)
                        .Expression = "0.00"
                    End With

                    With .Add("TotalRefund", SqlDbType.Decimal)
                        .Expression = "0.00"
                    End With

                    With .Add("TotalReturns", SqlDbType.VarChar)
                        .Expression = "0.00"
                    End With

                    With .Add("ForegoneVAT", SqlDbType.VarChar)
                        .Expression = "SUM(ROUND(ForegoneVAT, 2))"
                    End With

                    With .Add("Charges", SqlDbType.VarChar)
                        .Expression = "Charges.ChargeAmount"
                    End With

                    'With .Add("InvoiceCount", SqlDbType.VarChar)
                    '    .Expression = "COUNT(transinvoice.ID)"
                    'End With

                    With .Add("InvoiceCount", SqlDbType.VarChar)
                        .Expression = "COUNT(distinct ID)"
                    End With

                    With .Add("SalesReturnsCount", SqlDbType.VarChar)
                        .Expression = "0"
                    End With
                End With

                .SqlFrom = "FROM CacheInvoices " & vbCrLf



                .SqlFrom += "outer apply (" & vbCrLf &
                       "SELECT SUM(ROUND(ISNULL(custpay.ChargeAmount,0), 2)) AS ChargeAmount " & vbCrLf &
                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                         " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                         " INNER JOIN dbo.CacheInvoices inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                         " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                          ") Charges "

                Select Case GroupBy
                    Case "ByOrganizationGroup"
                        SqlGroupBy &= ", Charges.ChargeAmount"
                    Case Else
                        SqlGroupBy &= " Charges.ChargeAmount"
                End Select



                '"TransInvoiceItems inner join TransInvoice  " & vbCrLf &
                '"on TransInvoiceItems.InvoiceID = transinvoice.ID  " & vbCrLf &
                '"INNER join MobizLibraryOrganizationFOWST " & vbCrLf &
                '"on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                '"left join Mobiz.dbo.LibraryOrganizationFranchisee " & vbCrLf &
                '"on TransInvoice.FOWSTID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                '"left join LibraryFranchiseGroup franchisgroupMain " & vbCrLf &
                '"on franchisgroupMain.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID "

                '.SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) " & vbCrLf &
                '            "AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                '            "AND IsFranchisorConfirmed=1 and IsFranchiseeConfirmed=1 " & vbCrLf &
                '            "AND FranchisorOrgID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID")))

                .SqlWhere = "WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf

                'If New SecurityRole().CheckSecurityCurrentUserAcccess("On the Dashbooards and Reports Main Page, user is allowed to view all main page information?", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID).Rows.Count = 0 Then
                '    .SqlWhere += " AND franchisgroupMain.ID IN (SELECT FranchiseGroupID FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & ") "
                'End If


                If Not CStr2(Common.WebRequest.Data("Group")) = vbNullString Then
                    '.SqlWhere += " AND FranchiseGroupID IN (" & CStr2(Common.WebRequest.Data("Group")) & ") "
                Else
                    'If New SecurityRole().CheckSecurityCurrentUserAcccess("On the Dashbooards and Reports Main Page, user is allowed to view all main page information?", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID).Rows.Count = 0 Then
                    .SqlWhere += " AND FranchiseGroupID IN (SELECT FranchiseGroupID FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & ") "
                    'End If
                End If

                If Not CStr2(Common.WebRequest.Data("Region")) = vbNullString Then
                    .SqlWhere += " AND RegionID IN(" & CStr2(Common.WebRequest.Data("Region")) & ") "
                End If

                If Not CStr2(Common.WebRequest.Data("Overview")) = vbNullString AndAlso Not CStr2(Common.WebRequest.Data("Overview")) = "All" Then
                    If CStr2(Common.WebRequest.Data("Overview")) = "CompanyOwnedStores" Then
                        .SqlWhere += " AND ISNULL(FromFranchisee,0) = 0 "
                    Else
                        .SqlWhere += " AND ISNULL(FromFranchisee,0) = 1 "
                    End If

                End If

                If Not SqlGroupBy = vbNullString Then
                    .SqlGroup = "GROUP BY " & SqlGroupBy
                End If

                '.SqlGroup = "GROUP BY tblReturns.TotalReturns, tblReturns.SalesReturnsCount, Charges.ChargeAmount, tblReturns.TotalReturns, tblInvoiceCount.InvoiceCount  " & SqlGroupBy

                pDT = .Requery

            End With

            'Sales Returns

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields

                    Select Case GroupBy
                        Case "ByOrganizationGroup"
                            With .Add("GroupID", SqlDbType.VarChar)
                                .Expression = "LibraryFranchiseGroup_2.ID"
                            End With
                    End Select

                    With .Add("TotalSrv", SqlDbType.VarChar)
                        .Expression = "SUM(GrossSrvAmount)"
                    End With

                    With .Add("TotalRefund", SqlDbType.VarChar)
                        .Expression = "SUM(GrossRefundAmount)"
                    End With

                    With .Add("TotalReturns", SqlDbType.VarChar)
                        .Expression = "SUM(GrossSrvAmount) + SUM(GrossRefundAmount)"
                    End With

                    With .Add("SalesReturnsCount", SqlDbType.VarChar)
                        .Expression = "COUNT(TransSalesReturnLedger.ID)"
                    End With
                End With

                .SqlFrom += "FROM TransSalesReturnLedger " & vbCrLf &
                        "--inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                        "--inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then 'by group or fowst dapat ni
                    .SqlFrom += "inner join Mobiz.dbo.LibraryOrganizationFranchisee LibraryOrganizationFranchisee_2 on TransSalesReturnLedger.FOWSTID = LibraryOrganizationFranchisee_2.FranchiseeOrgFowstID " & vbCrLf &
                                "inner join LibraryFranchiseGroup LibraryFranchiseGroup_2 on LibraryFranchiseGroup_2.ID = LibraryOrganizationFranchisee_2.FranchiseGroupID " & vbCrLf
                End If

                .SqlFrom += "WHERE  TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf

                'If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                '    .SqlFrom += " AND ISNULL(CONVERT(varchar(36), LibraryFranchiseGroup_2.ID), '') = ISNULL(CONVERT(varchar(36), franchisgroupMain.ID), '') " & vbCrLf
                'End If

                .SqlFrom += " AND convert(date,TransSalesReturnLedger.LastUpdated) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf

                If ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") Then
                    'PC Franchise DB ORG ID
                    'dapat kwaon lang ang returns sa mga specific stores ni franchisee, dili tanan GROUPS sa mga franchisee
                    .SqlFrom += " AND FowstID IN (SELECT FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID IN (SELECT FranchiseGroupID FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & "))"
                End If

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                    .SqlFrom += "GROUP BY LibraryFranchiseGroup_2.ID " & vbCrLf
                End If

                pDTSalesReturn = .Requery

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                    If pDTSalesReturn.Rows.Count > 0 Then
                        For i As Integer = 0 To pDTSalesReturn.Rows.Count - 1
                            For j As Integer = 0 To pDT.Rows.Count - 1
                                If pDT.Rows(j).Item("GroupID").ToString() = pDTSalesReturn.Rows(i).Item("GroupID").ToString() Then
                                    pDT.Rows(j).Item("TotalSrv") = pDTSalesReturn.Rows(i).Item("TotalSrv")
                                    pDT.Rows(j).Item("TotalRefund") = pDTSalesReturn.Rows(i).Item("TotalRefund")
                                    pDT.Rows(j).Item("TotalReturns") = pDTSalesReturn.Rows(i).Item("TotalReturns")
                                    pDT.Rows(j).Item("SalesReturnsCount") = pDTSalesReturn.Rows(i).Item("SalesReturnsCount")
                                End If
                            Next
                        Next
                    End If
                Else
                    'GroupBy all
                    If pDT.Rows.Count > 0 Then
                        pDT.Rows(0).Item("TotalSrv") = pDTSalesReturn.Rows(0).Item("TotalSrv")
                        pDT.Rows(0).Item("TotalRefund") = pDTSalesReturn.Rows(0).Item("TotalRefund")
                        pDT.Rows(0).Item("TotalReturns") = pDTSalesReturn.Rows(0).Item("TotalReturns")
                        pDT.Rows(0).Item("SalesReturnsCount") = pDTSalesReturn.Rows(0).Item("SalesReturnsCount")
                    End If
                End If

            End With

            'e-sort pdt by total sales
            pDT.DefaultView.Sort = "GrossSales ASC"

            pDT = pDT.DefaultView.ToTable
        Else
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    Select Case GroupBy
                        Case "ByOrganizationGroup"
                            With .Add("GroupID", SqlDbType.VarChar)
                                .Expression = "''"
                            End With
                            With .Add("GroupName", SqlDbType.VarChar)
                                .Expression = "'Unassigned'"
                            End With
                    End Select

                    With .Add("GrossSales", SqlDbType.Decimal)
                        .Expression = "SUM(CacheInvoices.GrossSalesNetOfVATExemptions)"
                    End With

                    With .Add("TotalDiscounts", SqlDbType.Decimal)
                        '.Expression = "SUM(TransInvoiceItems.DiscountAmount) + tblSCDiscount.SCDiscount" ' old code
                        .Expression = "SUM(CacheInvoices.Discount)"
                    End With

                    With .Add("TotalReturns", SqlDbType.VarChar)
                        .Expression = "tblReturns.TotalReturns"
                    End With

                    With .Add("ForegoneVAT", SqlDbType.VarChar)
                        .Expression = "VATExempt.ForegoneVAT"
                    End With

                    With .Add("ChargeAmount", SqlDbType.VarChar)
                        .Expression = "Charges.ChargeAmount"
                    End With

                    With .Add("InvoiceCount", SqlDbType.VarChar)
                        .Expression = "COUNT(CacheInvoices.ID)"
                    End With

                    With .Add("SalesReturnsCount", SqlDbType.VarChar)
                        .Expression = "ISNULL(tblReturns.SalesReturnsCount,0)"
                    End With

                End With

                '.SqlFrom = "FROM " & vbCrLf &
                '    "TransInvoiceItems inner join TransInvoice " & vbCrLf &
                '    "on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf
                .SqlFrom = " FROM CacheInvoices" & vbCrLf
                .SqlFrom &= "left join MobizLibraryOrganizationFOWST  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FOWSTID " & vbCrLf

                .SqlFrom += "outer apply (  " & vbCrLf &
                    "SELECT SUM(TotalAmount) AS [TotalReturns], COUNT(TransMemo.ID) [SalesReturnsCount] " & vbCrLf

                .SqlFrom += "FROM TransSalesReturnLedger " & vbCrLf &
                    "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                    "inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf

                .SqlFrom += "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND " & vbCrLf

                .SqlFrom += " convert(date,TransMemo.MemoDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf

                .SqlFrom += ") tblReturns "

                .SqlFrom += "outer apply (" & vbCrLf &
                            " Select SUM(SCDiscount) as SCDiscount from TransInvoice " & vbCrLf &
                            "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                             ") tblSCDiscount "

                .SqlFrom += "outer apply (" & vbCrLf &
                             "Select SUM(ForegoneVAT) as ForegoneVAT from TransInvoice " & vbCrLf &
                            "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            ") VATExempt "

                .SqlFrom += "outer apply (" & vbCrLf &
                         "SELECT SUM(ISNULL(custpay.ChargeAmount,0)) AS ChargeAmount " & vbCrLf &
                          " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                           " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                           " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                           " WHERE (inv.InvoiceNumber>0 OR inv.FranchiseeInvoiceNumber>0) AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            ") Charges "

                .SqlWhere = "WHERE /*(Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND*/ -- old code" & vbCrLf &
                    " CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " And " & ReadyString(DateTo.Date) & vbCrLf &
                            SqlWhere

                .SqlGroup = "GROUP BY tblReturns.TotalReturns,  tblReturns.SalesReturnsCount, VATExempt.ForegoneVAT, tblSCDiscount.SCDiscount, Charges.ChargeAmount " & SqlGroupBy

                pDT = .Requery
            End With
        End If

        Return pDT

    End Function

    Function GetTotalGrossAndDiscountsByEmployeeFOWST(ByVal dtEmployeeFowstList As DataTable) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        'Dim SqlWhereSalesReturns As String
        'Dim SqlGroupBySalesReturns As String
        'Dim SqlGroupBy As String
        'Dim SqlFrom As String
        'Dim Fowst As String()

        SqlWhere += "AND (MobizLibraryOrganizationFOWST.ID = " & ReadyString(dtEmployeeFowstList.Rows(0).Item("FowstID"))

        For i As Integer = 1 To dtEmployeeFowstList.Rows.Count - 1
            SqlWhere += " OR MobizLibraryOrganizationFOWST.ID = " & ReadyString(dtEmployeeFowstList.Rows(i).Item("FowstID"))
        Next

        SqlWhere += ")"

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("FOWSTID", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.ID"
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With

                With .Add("TotalDiscounts", SqlDbType.Decimal)
                    .Expression = "SUM(TransInvoiceItems.DiscountAmount) + SUM(TransInvoice.SCDiscount)"
                End With

                With .Add("TotalReturns", SqlDbType.VarChar)
                    .Expression = "tblReturns.TotalReturns"
                End With

                With .Add("ForegoneVAT", SqlDbType.VarChar)
                    .Expression = "SUM(TransInvoice.ForegoneVAT)"
                End With

            End With

            .SqlFrom = "FROM " & vbCrLf &
                "TransInvoiceItems inner join TransInvoice " & vbCrLf &
                "on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                "inner join MobizLibraryOrganizationFOWST  " & vbCrLf &
                "on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf

            .SqlFrom += "outer apply (  " & vbCrLf &
                "SELECT SUM(TotalAmount) AS [TotalReturns] " & vbCrLf

            .SqlFrom += "FROM TransSalesReturnLedger " & vbCrLf &
                "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                "inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf

            If GroupBy = "ByOrganizationFOWST" And CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                .SqlFrom += "inner join MobizLibraryOrganizationFOWST MobizLibraryOrganizationFOWST_2 on MobizLibraryOrganizationFOWST_2.ID = TransSalesReturnLedger.FOWSTID " & vbCrLf
            End If

            .SqlFrom += "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND " & vbCrLf

            .SqlFrom += " convert(date,TransMemo.MemoDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf

            'If GroupBy = "ByOrganizationFOWST" Then
            .SqlFrom += "AND ISNULL(CONVERT(varchar(36), TransSalesReturnLedger.FOWSTID), '') = ISNULL(CONVERT(varchar(36), MobizLibraryOrganizationFOWST.ID), '')"
            'End If

            If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                .SqlFrom += "GROUP BY TransSalesReturnLedger.FOWSTID " & vbCrLf
            End If

            .SqlFrom += ") tblReturns"

            .SqlWhere = "WHERE Transinvoice.InvoiceNumber>0 AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                        SqlWhere

            .SqlGroup = "GROUP BY TotalReturns, MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTotalGrossAndDiscountsByFOWST() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlWhereSalesReturns As String
        Dim SqlGroupBySalesReturns As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()

        Dim dtSecurity As DataTable
        Dim dtEmployeeFowstList As DataTable
        Dim mSecurity As New SecurityRole
        Dim pSql As String

        dtSecurity = mSecurity.CheckSecurityCurrentUserAcccess("Show Dashboard First Page", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID)

        If dtSecurity.Rows.Count = 0 Then
            '0 means the user dnt have access to view first page.
            pSql = " SELECT FowstID " & vbCrLf &
                " FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
                " WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                " UNION" & vbCrLf &
                " SELECT LibraryFranchiseGroup.FranchisorFowstID AS FowstID" & vbCrLf &
                " FROM LibraryFranchiseGroupHeads" & vbCrLf &
                " JOIN LibraryFranchiseGroup ON LibraryFranchiseGroup.ID = LibraryFranchiseGroupHeads.FranchiseGroupID" & vbCrLf &
                " WHERE LibraryFranchiseGroupHeads.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
            dtEmployeeFowstList = Common.WebRequest.ProData.Execute(pSql)
            If dtEmployeeFowstList.Rows.Count > 0 Then
                pDT = GetTotalGrossAndDiscountsByEmployeeFOWST(dtEmployeeFowstList)
                Return pDT
            End If
        End If

        pFinder = New Finder

        If IsFranchisor() Then
            'If Franchisor

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields

                    With .Add("SalesDate", SqlDbType.VarChar)
                        .Expression = "CONVERT(date, CacheInvoices.InvoiceDate)"
                    End With

                    With .Add("FOWSTID", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWST.ID"
                    End With
                    With .Add("FOWSTName", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWST.Name"
                    End With

                    With .Add("FOWSTAddress", SqlDbType.VarChar)
                        .Expression = "dbo.SplitJoinComma(ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '') + ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '') + ', ' + ISNULL(MobizLibraryCities.Name, '') + ', ' + ISNULL(MobizLibraryCountries.Name, ''))"
                    End With

                    With .Add("MallCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.MallCode"
                    End With

                    With .Add("TenantNo", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo"
                    End With

                    With .Add("ClassCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"
                    End With

                    With .Add("TradeCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"
                    End With

                    With .Add("OutletNo", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo"
                    End With

                    With .Add("SaleType", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.SaleType"
                    End With

                    With .Add("Region", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWST.Region"
                    End With

                    If CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                        If GroupBy = "ByOrganization" Then
                            SqlWhereSalesReturns = " AND TransMemoSalesReturn.InvoiceID = CacheInvoices.ID " & vbCrLf &
                                                    " AND TransMemo.FowstID = MobizLibraryOrganizationFOWST.ID"
                        Else
                            SqlWhere = " AND LibraryFranchiseGroup.ID IS NULL"
                            SqlWhereSalesReturns = " AND TransMemoSalesReturn.InvoiceID = CacheInvoices.ID " & vbCrLf &
                                                    " AND TransMemo.FowstID = MobizLibraryOrganizationFOWST.ID"
                        End If
                    Else
                        SqlWhere = " AND LibraryFranchiseGroup.ID = " & ReadyString(Common.WebRequest.Data("GroupID"))
                    End If

                    With .Add("GrossSales", SqlDbType.Decimal)
                        '.Expression = "(CASE WHEN SUM(ISNULL(TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(TransInvoiceItems.SubTotal) ELSE SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                        .Expression = "SUM(ISNULL(CacheInvoices.GrossSalesNetOfVATExemptions, 0))"
                    End With

                    With .Add("TotalDiscounts", SqlDbType.Decimal)
                        '.Expression = "SUM(CacheInvoices.ItemsDiscount) + SUM(CacheInvoices.SCDiscount)" 'commented by Arnie 090125 dapat tanan discounts ang makuha
                        .Expression = "SUM(CacheInvoices.Discount)"
                    End With

                    With .Add("TotalReturns", SqlDbType.VarChar)
                        .Expression = "tblReturns.TotalReturns"
                    End With

                    With .Add("ForegoneVAT", SqlDbType.VarChar)
                        .Expression = "SUM(CacheInvoices.ForegoneVAT)"
                        '.Expression = "VATExempt.ForegoneVAT"
                    End With

                    With .Add("ChargeAmount", SqlDbType.VarChar)
                        .Expression = "Charges.ChargeAmount"
                    End With

                    With .Add("BusinessOwner", SqlDbType.VarChar)
                        .Expression = " (SELECT STUFF(" & vbCrLf &
                            " 	(SELECT '; ' + " & BrowserController.ComposePeopleName() & vbCrLf &
                            "     FROM MobizLibraryOrganizationOwner" & vbCrLf &
                            " 	JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationOwner.PeopleID" & vbCrLf &
                            " 	LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                            "     WHERE MobizLibraryOrganizationOwner.OrganizationID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
                            " 	FOR XML PATH('')" & vbCrLf &
                            "     ), 1, 2, '')" & vbCrLf &
                            " )"
                    End With

                    With .Add("OrganizationName", SqlDbType.VarChar)
                        .Expression = "(SELECT STUFF(" & vbCrLf &
                            " 	(SELECT '; ' + MobizLibraryOrganization.CompanyName" & vbCrLf &
                            "     FROM MobizLibraryOrganization" & vbCrLf &
                            "     WHERE MobizLibraryOrganization.ID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
                            " 	FOR XML PATH('')" & vbCrLf &
                            "     ), 1, 2, '')" & vbCrLf &
                            " )"
                    End With

                    'With .Add("FranchiseeInvoiceNumber", SqlDbType.VarChar)
                    '    .Expression = "TransInvoice.FranchiseeInvoiceNumber"
                    'End With

                    'With .Add("PurchaseOrderNo", SqlDbType.VarChar)
                    '    .Expression = "TransInvoice.PurchaseOrderNo"
                    'End With

                    With .Add("OwnershipType", SqlDbType.VarChar)
                        .Expression = "ISNULL(Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType, '')"
                    End With
                End With


                '"TransInvoiceItems inner join TransInvoice " & vbCrLf &
                '"on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &

                .SqlFrom = "FROM " & vbCrLf &
                    "CacheInvoices " & vbCrLf &
                    "inner join MobizLibraryOrganizationFOWST  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FOWSTID " & vbCrLf &
                    "left join Mobiz.dbo.LibraryOrganizationFranchisee  " & vbCrLf &
                    "on CacheInvoices.FOWSTID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                    "left join LibraryFranchiseGroup " & vbCrLf &
                    "on LibraryFranchiseGroup.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
                    "LEFT join MobizLibraryOrganizationFOWSTMallInfoSM  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = CacheInvoices.FOWSTID " & vbCrLf &
                    "LEFT join MobizLibraryCities  " & vbCrLf &
                    "on MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID " & vbCrLf &
                    "LEFT join MobizLibraryCountries  " & vbCrLf &
                    "on MobizLibraryCountries.ID = MobizLibraryOrganizationFOWST.FowstCountryID " & vbCrLf

                .SqlFrom += "outer apply (  " & vbCrLf &
                    "SELECT SUM(TotalAmount) AS [TotalReturns] " & vbCrLf

                .SqlFrom += "FROM TransSalesReturnLedger " & vbCrLf &
                    "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                    "inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then 'by group or fowst dapat ni
                    .SqlFrom += "left join Mobiz.dbo.LibraryOrganizationFranchisee LibraryOrganizationFranchisee_2 on TransSalesReturnLedger.FOWSTID = LibraryOrganizationFranchisee_2.FranchiseeOrgFowstID " & vbCrLf &
                                "left join LibraryFranchiseGroup LibraryFranchiseGroup_2 on LibraryFranchiseGroup_2.ID = LibraryOrganizationFranchisee_2.FranchiseGroupID " & vbCrLf
                End If

                If GroupBy = "ByOrganizationFOWST" And CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                    .SqlFrom += "left join MobizLibraryOrganizationFOWST MobizLibraryOrganizationFOWST_2 on MobizLibraryOrganizationFOWST_2.ID = TransSalesReturnLedger.FOWSTID " & vbCrLf
                End If

                .SqlFrom += "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND " & vbCrLf

                'If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                '    .SqlFrom += "ISNULL(CONVERT(varchar(36), LibraryFranchiseGroup_2.ID), '') = ISNULL(CONVERT(varchar(36), LibraryFranchiseGroup.ID), '') " & vbCrLf &
                '        " and "
                'End If

                '.SqlFrom += " convert(date,TransMemo.MemoDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                .SqlFrom += " convert(date,TransMemo.MemoDate) = convert(date,CacheInvoices.InvoiceDate) " & vbCrLf &
                            " AND TransSalesReturnLedger.FowstID = CacheInvoices.FowstID " & vbCrLf

                'If GroupBy = "ByOrganizationFOWST" Then
                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                    .SqlFrom += "AND ISNULL(CONVERT(varchar(36), TransSalesReturnLedger.FOWSTID), '') = ISNULL(CONVERT(varchar(36), MobizLibraryOrganizationFOWST.ID), '') " & vbCrLf &
                            " AND LibraryOrganizationFranchisee_2.FranchisorOrgID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & vbCrLf
                End If

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                    .SqlFrom += "GROUP BY TransSalesReturnLedger.FOWSTID " & vbCrLf

                End If

                .SqlFrom += ") tblReturns "

                .SqlFrom += "outer apply (" & vbCrLf &
                       "SELECT SUM(ROUND(ISNULL(custpay.ChargeAmount,0), 2)) AS ChargeAmount " & vbCrLf &
                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                         " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                         " INNER JOIN dbo.CacheInvoices inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                         " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                          " AND inv.FOWSTID = CacheInvoices.FOWSTID " & vbCrLf &
                          ") Charges "

                '.SqlFrom += "outer apply (" & vbCrLf &
                '            " Select SUM(SCDiscount) as SCDiscount from TransInvoice tDisc " & vbCrLf &
                '            "WHERE tDisc.InvoiceNumber>0 AND CONVERT(date, tDisc.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '            " AND tDisc.FowstID = TransInvoice.FowstID " & vbCrLf &
                '             ") tblSCDiscount "

                '.SqlFrom += "outer apply (" & vbCrLf &
                '             "Select SUM(ForegoneVAT) as ForegoneVAT from TransInvoice " & vbCrLf &
                '            "WHERE (Transinvoice.InvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '            ") VATExempt "

                '.SqlFrom += "outer apply (" & vbCrLf &
                '         "SELECT SUM(ISNULL(custpay.ChargeAmount,0)) AS ChargeAmount " & vbCrLf &
                '          " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                '           " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                '           " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                '           " WHERE (inv.InvoiceNumber>0 OR inv.FranchiseeInvoiceNumber>0) AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '            ") Charges "

                Dim OwnTypeSqlWhere As String = " AND ("
                Dim isSetPrefix As Boolean = False

                For Each pObj As Object In Common.WebRequest.AllData("FilterOwnType")
                    If isSetPrefix = False Then
                        OwnTypeSqlWhere += "Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(pObj.ToString() = "NULL", "IS NULL", "=" & ReadyString(pObj.ToString()))
                        isSetPrefix = True
                    Else
                        OwnTypeSqlWhere += " OR Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(pObj.ToString() = "NULL", "IS NULL", "=" & ReadyString(pObj.ToString()))
                    End If
                Next

                If Common.WebRequest.AllData("FilterOwnType").Length <> 0 Then
                    OwnTypeSqlWhere += ")"
                    SqlWhere += OwnTypeSqlWhere
                End If

                '"WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) " & vbCrLf &

                .SqlWhere = "WHERE " & vbCrLf &
                            "CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            "AND IsFranchisorConfirmed=1 and IsFranchiseeConfirmed=1 " & vbCrLf &
                            "AND FranchisorOrgID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & vbCrLf &
                            SqlWhere

                'If ObjFilterOwnType.Length = 0 Then
                '    .SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '                SqlWhere
                'ElseIf ObjFilterOwnType.Length = 1 Then
                '    If ObjFilterOwnType(0) = "All" Then
                '        .SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '                SqlWhere
                '    Else
                '        .SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '                " AND Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(0) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(0))) & vbCrLf &
                '                SqlWhere
                '    End If
                'ElseIf ObjFilterOwnType.Length = 2 Then
                '    .SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '                " AND (Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(0) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(0))) & " OR Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(1) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(1))) & ")" & vbCrLf &
                '    SqlWhere
                'Else
                '    .SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                '                " AND (Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(0) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(0))) & " OR Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(1) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(1))) & " OR Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType " & IIf(ObjFilterOwnType(2) = "NULL", "IS NULL", "=" & ReadyString(ObjFilterOwnType(2))) & ")" & vbCrLf &
                '                SqlWhere
                'End If

                'TransInvoice.FranchiseeInvoiceNumber, TransInvoice.PurchaseOrderNo,
                .SqlGroup = "GROUP BY CONVERT(date, CacheInvoices.InvoiceDate), TotalReturns, MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                    ",MobizLibraryOrganizationFOWST.OrganizationID,MobizLibraryOrganizationFOWST.Region" & vbCrLf &
                    ",Mobiz.dbo.LibraryOrganizationFranchisee.OwnershipType, MobizLibraryOrganizationFOWSTMallInfoSM.MallCode, MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo, MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode " & vbCrLf &
                    ",MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode, MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo, MobizLibraryOrganizationFOWSTMallInfoSM.SaleType" & vbCrLf &
                    ",MobizLibraryOrganizationFOWST.StreetNumber, MobizLibraryOrganizationFOWST.Street, MobizLibraryCities.Name, MobizLibraryCountries.Name, Charges.ChargeAmount --, tblSCDiscount.SCDiscount"

                .SqlOrder = " ORDER BY CONVERT(date, CacheInvoices.InvoiceDate) "

                pDT = .Requery

            End With
        Else
            'If Not Franchisor and no group created

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields

                    With .Add("FOWSTID", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWST.ID"
                    End With
                    With .Add("FOWSTName", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWST.Name"
                    End With

                    With .Add("FOWSTAddress", SqlDbType.VarChar)
                        .Expression = "dbo.SplitJoinComma(ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '') + ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '') + ', ' + ISNULL(MobizLibraryCities.Name, '') + ', ' + ISNULL(MobizLibraryCountries.Name, ''))"
                    End With

                    With .Add("MallCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.MallCode"
                    End With

                    With .Add("TenantNo", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo"
                    End With

                    With .Add("ClassCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"
                    End With

                    With .Add("TradeCode", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"
                    End With

                    With .Add("OutletNo", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo"
                    End With

                    With .Add("SaleType", SqlDbType.VarChar)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.SaleType"
                    End With

                    With .Add("GrossSales", SqlDbType.Decimal)
                        .Expression = "SUM(CacheInvoices.GrossSalesNetOfVATExemptions)"
                    End With

                    With .Add("TotalDiscounts", SqlDbType.Decimal)
                        .Expression = "SUM(CacheInvoices.Discount)"
                    End With

                    With .Add("TotalReturns", SqlDbType.VarChar)
                        .Expression = "tblReturns.TotalReturns"
                    End With

                    With .Add("ForegoneVAT", SqlDbType.VarChar)
                        .Expression = "VATExempt.ForegoneVAT"
                    End With

                    With .Add("ChargeAmount", SqlDbType.VarChar)
                        .Expression = "Charges.ChargeAmount"
                    End With

                End With

                .SqlFrom = "FROM CacheInvoices" & vbCrLf &
                    "inner join MobizLibraryOrganizationFOWST  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FOWSTID " & vbCrLf &
                    "inner join MobizLibraryOrganizationFOWSTMallInfoSM  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = CacheInvoices.FOWSTID " & vbCrLf &
                    "inner join MobizLibraryCities  " & vbCrLf &
                    "on MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID " & vbCrLf &
                    "inner join MobizLibraryCountries  " & vbCrLf &
                    "on MobizLibraryCountries.ID = MobizLibraryOrganizationFOWST.FowstCountryID " & vbCrLf

                .SqlFrom += "outer apply (  " & vbCrLf &
                    "SELECT SUM(TotalAmount) AS [TotalReturns] " & vbCrLf

                .SqlFrom += "FROM TransSalesReturnLedger " & vbCrLf &
                    "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                    "inner join TransMemoSalesReturn on TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf

                If GroupBy = "ByOrganizationFOWST" And CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                    .SqlFrom += "inner join MobizLibraryOrganizationFOWST MobizLibraryOrganizationFOWST_2 on MobizLibraryOrganizationFOWST_2.ID = TransSalesReturnLedger.FOWSTID " & vbCrLf
                End If

                .SqlFrom += "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND " & vbCrLf

                .SqlFrom += " convert(date,TransMemo.MemoDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf

                'If GroupBy = "ByOrganizationFOWST" Then
                .SqlFrom += "AND ISNULL(CONVERT(varchar(36), TransSalesReturnLedger.FOWSTID), '') = ISNULL(CONVERT(varchar(36), MobizLibraryOrganizationFOWST.ID), '')"
                'End If

                If GroupBy = "ByOrganizationGroup" Or GroupBy = "ByOrganizationFOWST" Then
                    .SqlFrom += "GROUP BY TransSalesReturnLedger.FOWSTID " & vbCrLf
                End If

                .SqlFrom += ") tblReturns "

                .SqlFrom += "outer apply (" & vbCrLf &
                            " Select SUM(SCDiscount) as SCDiscount from TransInvoice tDisc " & vbCrLf &
                            "WHERE tDisc.InvoiceNumber>0 AND CONVERT(date, tDisc.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            " AND tDisc.FowstID = CacheInvoices.FowstID " & vbCrLf &
                             ") tblSCDiscount "

                .SqlFrom += "outer apply (" & vbCrLf &
                             "Select SUM(ForegoneVAT) as ForegoneVAT from TransInvoice " & vbCrLf &
                            "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            ") VATExempt "

                .SqlFrom += "outer apply (" & vbCrLf &
                         "SELECT SUM(ISNULL(custpay.ChargeAmount,0)) AS ChargeAmount " & vbCrLf &
                          " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                           " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                           " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                           " WHERE (inv.InvoiceNumber>0 OR inv.FranchiseeInvoiceNumber>0) AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            ") Charges "

                .SqlWhere = "WHERE /*Transinvoice.InvoiceNumber>0 AND*/ CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom.Date) & " AND " & ReadyString(DateTo.Date) & vbCrLf &
                            SqlWhere

                .SqlGroup = "GROUP BY VATExempt.ForegoneVAT, TotalReturns, MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                            ", MobizLibraryOrganizationFOWSTMallInfoSM.MallCode, MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo, MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode " & vbCrLf &
                            ", MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode, MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo, MobizLibraryOrganizationFOWSTMallInfoSM.SaleType" & vbCrLf &
                            ", MobizLibraryOrganizationFOWST.StreetNumber, MobizLibraryOrganizationFOWST.Street, MobizLibraryCities.Name, MobizLibraryCountries.Name, tblSCDiscount.SCDiscount, Charges.ChargeAmount"

                pDT = .Requery

            End With
        End If
        Return pDT

    End Function

    Function IsFranchisor() As Boolean
        'Dim pSql As String

        'pSql = " SELECT COUNT(*)" & vbCrLf &
        '    " FROM LibraryFranchiseGroup" & vbCrLf &
        '    " JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
        '    " WHERE LibraryFranchiseGroup.IsFranchisee = 0" & vbCrLf &
        '    " AND MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1" & vbCrLf &
        '    " AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed = 1" & vbCrLf &
        '    " AND MobizLibraryOrganizationFranchisee.IsActive = 1" & vbCrLf &
        '    " AND MobizLibraryOrganizationFranchisee.FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
        'Dim pOut = CInt2(Common.WebRequest.ProData.ExecuteScalar(pSql))

        'Return pOut > 0

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "Select * from LibraryFranchiseGroup"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            pSql = "Select * from MobizLibraryOrganizationFranchisee  WHERE FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                Return True
            End If
        End If

        Return False

    End Function

    Function GetTotalSalesReturnsData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                Select Case GroupBy
                    Case "ByOrganizationGroup"
                        With .Add("GroupID", SqlDbType.VarChar)
                            .Expression = "LibraryFranchiseGroup.ID"
                        End With
                        With .Add("GroupName", SqlDbType.VarChar)
                            .Expression = "ISNULL(GroupName,'Unassigned')"
                        End With
                        SqlGroupBy = "GROUP BY LibraryFranchiseGroup.ID, LibraryFranchiseGroup.GroupName"

                        If CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                            SqlWhere = " AND LibraryFranchiseGroup.ID IS NULL"
                        Else
                            SqlWhere = " AND LibraryFranchiseGroup.ID " & ReadyString(CStr(Common.WebRequest.Data("GroupID")))
                        End If


                    Case "ByOrganizationFOWST"
                        With .Add("FOWSTID", SqlDbType.VarChar)
                            .Expression = "MobizLibraryOrganizationFOWST.ID"
                        End With
                        With .Add("FOWSTName", SqlDbType.VarChar)
                            .Expression = "MobizLibraryOrganizationFOWST.Name"
                        End With
                        SqlGroupBy = "GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name"
                        If CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                            SqlWhere = " AND LibraryFranchiseGroup.ID IS NULL AND InvoiceNumber > 0"
                        Else
                            SqlWhere = " AND LibraryFranchiseGroup.ID = " & ReadyString(Common.WebRequest.Data("GroupID"))
                        End If

                End Select

                With .Add("Total", SqlDbType.Decimal)
                    .Expression = "SUM(Amount)"
                End With

            End With

            'CStr(Common.WebRequest.Data("GroupID")) = vbNullString -> for unsorted fowst
            If GroupBy = "ByOrganizationFOWST" And CStr(Common.WebRequest.Data("GroupID")) = vbNullString Then
                .SqlFrom = "FROM " & vbCrLf &
                    "TransInvoiceItems inner join TransInvoice " & vbCrLf &
                    "on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                    "inner join MobizLibraryOrganizationFOWST  " & vbCrLf &
                    "on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                    "left join Mobiz.dbo.LibraryOrganizationFranchisee  " & vbCrLf &
                    "on TransInvoice.FOWSTID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                    "left join LibraryFranchiseGroup " & vbCrLf &
                    "on LibraryFranchiseGroup.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf
            Else
                .SqlFrom = "FROM TransSalesReturnLedger inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID " & vbCrLf &
                            " LEFT join Mobiz.dbo.LibraryOrganizationFranchisee  " & vbCrLf &
                            "on TransSalesReturnLedger.FowstID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                            "left join LibraryFranchiseGroup " & vbCrLf &
                            "on LibraryFranchiseGroup.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf
            End If

            .SqlWhere = "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & vbCrLf &
                        "BETWEEN CONVERT(date," & ReadyString(DateFrom.Date) & ")  " & vbCrLf &
                        "AND CONVERT(date," & ReadyString(DateTo.Date) & ") " & vbCrLf &
                        SqlWhere

            .SqlGroup = SqlGroupBy

            pDT = .Requery
        End With
        Return pDT
    End Function

    Function GetFOWSTCount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FOWSTCount", SqlDbType.Int)
                    .Expression = "COUNT(*)"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFowst " & vbCrLf &
                        "left join MobizLibraryOrganizationFranchisee  " & vbCrLf &
                        "on MobizLibraryOrganizationFranchisee.franchiseeorgfowstid = mobizlibraryorganizationfowst.ID " & vbCrLf

            .SqlWhere = "WHERE mobizlibraryorganizationfowst.organizationid = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf &
                        "or MobizLibraryOrganizationFranchisee.franchisororgid = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf &
                        "and MobizLibraryOrganizationFranchisee.IsActive = 1 and IsFranchiseeConfirmed = 1 and IsFranchisorConfirmed = 1"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Public Function GroupInventoryLevels(ByVal vGroupID As String) As DataTable
        ' bookmark: arvin, GroupInventoryLevels

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " DECLARE @OrgID UNIQUEIDENTIFIER = " & ReadyString(OrgID) & ";" & vbCrLf
        pSql &= " SELECT tblFowsts.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,MobizCacheInventoryBalancePerFowst.SrpValue" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,(SELECT STUFF(" & vbCrLf &
            " 	(SELECT '; ' + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '')" & vbCrLf &
            "     FROM MobizLibraryOrganizationOwner" & vbCrLf &
            " 	JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationOwner.PeopleID" & vbCrLf &
            " 	LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            "     WHERE MobizLibraryOrganizationOwner.OrganizationID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
            " 	FOR XML PATH('')" & vbCrLf &
            "     ), 1, 2, '')" & vbCrLf &
            " ) AS OrgOwners" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Region" & vbCrLf &
            " ,MobizCacheInventoryBalancePerFowst.LastUpdated" & vbCrLf
        pSql &= " ,ISNULL((SELECT TOP 1 ISNULL(MobizLibraryOrganizationFranchisee.OwnershipType, 'Unidentified')" & vbCrLf &
            "  	FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            "     WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = tblFowsts.FowstID" & vbCrLf &
            "     AND MobizLibraryOrganizationFranchisee.FranchisorOrgID = @OrgID" & vbCrLf &
            "  ), 'Company-Owned') AS OwnershipType" & vbCrLf
        pSql &= ",(SELECT TOP 1 ISNULL(COUNT(TransInventoryProductCountStock.Variance),0) AS InventoryCountApproval " & vbCrLf &
            "FROM TransInventoryProductCount" & vbCrLf &
            "INNER JOIN TransInventoryProductCountStock ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID" & vbCrLf &
            "WHERE TransInventoryProductCountStock.IsActionTaken = 0" & vbCrLf &
            "AND tblFowsts.FowstID = TransInventoryProductCount.FowstID " & vbCrLf &
            "AND TransInventoryProductCountStock.Variance <> 0 " & vbCrLf &
            ") NoOfUnapprovedCounts"
        pSql &= " FROM (" & vbCrLf &
            "   SELECT MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID AS FowstID" & vbCrLf &
            "   ,MobizLibraryOrganizationFranchisee.OwnershipType" & vbCrLf &
            "   FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            "   WHERE MobizLibraryOrganizationFranchisee.FranchisorOrgID = @OrgID" & vbCrLf &
            "   AND MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1" & vbCrLf &
            "   AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed = 1" & vbCrLf &
            "   AND MobizLibraryOrganizationFranchisee.IsActive = 1" & vbCrLf
        If CStr2(vGroupID) <> "" Then
            pSql &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(vGroupID)
        End If
        pSql &= "   UNION" & vbCrLf &
            "   SELECT MobizLibraryOrganizationFOWST.ID AS FowstID" & vbCrLf &
            "   ,'Company-owned' AS OwnershipType" & vbCrLf &
            "   FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            "   WHERE MobizLibraryOrganizationFOWST.OrganizationID = @OrgID" & vbCrLf &
            " ) tblFowsts" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = tblFowsts.FowstID" & vbCrLf &
            " JOIN MobizCacheInventoryBalancePerFowst ON MobizCacheInventoryBalancePerFowst.FowstID = tblFowsts.FowstID" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
            " ORDER BY FowstName"
        pOut = Common.WebRequest.ProData.Execute(pSql)

        Return pOut

    End Function
#End Region

#Region "POSTransactionDetail"

    Function GenerateNewAuditTrailData()
        Dim pSql As String
        Dim fowstid As String = Common.WebRequest.Data("FOWSTID")
        Dim deviceid As String = Common.WebRequest.Data("DeviceID")
        Dim dFrom As String = CDate(Common.WebRequest.Data("From")).ToString("MM-dd-yyyy")
        Dim dTo As String = CDate(Common.WebRequest.Data("To")).ToString("MM-dd-yyyy")

        pSql = "IF OBJECT_ID('tempdb..#tbl1') IS NOT NULL DROP TABLE #tbl1 " & vbCrLf &
                "   SELECT DISTINCT fowst.Name as Store " & vbCrLf &
                "       , ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') + " & vbCrLf &
                "           ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " & vbCrLf &
                "           ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " & vbCrLf &
                "       , RIGHT(CAST(inv.DeviceFowstTransactionNumber AS VARCHAR(12)), 12) as TransactionNo " & vbCrLf &
                "       , ISNULL(invitem.TransactionType, invlgr.TransactionType) AS TransactionType " & vbCrLf &
                "       , RIGHT(CAST(inv.InvoiceNumber AS VARCHAR(12)), 12) as ReferenceNo " & vbCrLf &
                "       , inv.InvoiceDate as TransactionDate " & vbCrLf &
                "       , prd.ProductNo " & vbCrLf &
                "       , prd.ProductName " & vbCrLf &
                "       , ISNULL(cust.CompanyName,'') + ' ' + ISNULL(PersonSurname,'') + ' ' + ISNULL(PersonFirstName,'') + ' ' + ISNULL(PersonMiddleName,'') as Customer " & vbCrLf &
                "       , cust.TinNumber " & vbCrLf &
                " 	    , CONVERT(float, invitem.Quantity) as Quantity " & vbCrLf &
                " 	    , invitem.UnitMeasure " & vbCrLf &
                " 	    , CONVERT(float, invitem.UnitPrice) as UnitPrice " & vbCrLf &
                " 	    , ROUND(CONVERT(float, invitem.SubTotal), 2) as SubTotal " & vbCrLf &
                "       , invitem.TaxPercent as VATPercent " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) > 0 OR ISNULL(inv.SoloParentDiscount, 0) > 0 THEN 0 ELSE invitem.TaxSales - invitem.DiscountSales END ELSE 0 END)), 2) as VATableSales " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) > 0 OR ISNULL(inv.SoloParentDiscount, 0) > 0 THEN 0 ELSE invitem.TaxAmount - invitem.DiscountTax END ELSE 0 END)), 2) as VATAmount " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 THEN 0 ELSE invitem.TaxSales - invitem.DiscountSales END ELSE 0 END)), 2) as VATableExSales " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 THEN 0 ELSE invitem.SubTotal - (invitem.TaxSales - invitem.DiscountSales) END ELSE 0 END)), 2) as VATExTax " & vbCrLf &
                "       , invdis.DiscountType " & vbCrLf &
                "       , invdis.DiscountPercentage " & vbCrLf &
                "       , ROUND((CASE WHEN fowst.IsVat = 1 THEN CONVERT(decimal, invitem.SubTotal) / (CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 AND ISNULL(inv.PhNatTeamDiscount, 0) = 0 THEN 1 ELSE (1 + (invitem.TaxPercent / 100)) END) ELSE CONVERT(decimal, invitem.SubTotal) END) * ((100 - invdis.DiscountPercentage) / 100) + (CASE WHEN ISNULL(inv.PhNatTeamDiscount, 0) = 0 THEN 0 ELSE (invitem.TaxAmount - invitem.DiscountTax) END), 2) AS AmountDue " & vbCrLf &
                "       , custpay.CashAmount " & vbCrLf &
                "       , custpay.CheckPaymentAmount " & vbCrLf &
                "       , custpay.DebitCardAmount " & vbCrLf &
                "       , custpay.SRVAmount " & vbCrLf &
                "       , custpay.OnCreditAmount " & vbCrLf &
                "       , custpay.OtherTypeAmount " & vbCrLf &
                "       , custpay.PaymentType as OtherPaymentType " & vbCrLf &
                "   INTO #tbl1 " & vbCrLf &
                "   FROM TransInventoryLedger invlgr " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " & vbCrLf &
                "   JOIN ( " & vbCrLf &
                "           SELECT TransInvoiceItems.ID" & vbCrLf &
                "               , TransInvoiceItems.InvoiceID" & vbCrLf &
                "               , TransInvoiceItemsRebates.ProductID" & vbCrLf &
                "               , TransInvoiceItems.PriceLevelID" & vbCrLf &
                "               , TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity AS Quantity" & vbCrLf &
                "               , TransInvoiceItemsRebates.UnitMeasure" & vbCrLf &
                "               , TransInvoiceItemsRebates.Amount * -1 AS UnitPrice" & vbCrLf &
                "               , TransInvoiceItemsRebates.Amount * TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity * -1 AS SubTotal" & vbCrLf &
                "               , 0 AS DiscountAmount" & vbCrLf &
                "               , TransInvoiceItemsRebates.Amount * -1 AS DiscountedUnitPrice" & vbCrLf &
                "               , 0 AS TaxPercent" & vbCrLf &
                "               , 0 AS TaxSales" & vbCrLf &
                "               , 0 AS DiscountSales" & vbCrLf &
                "               , 0 AS TaxAmount" & vbCrLf &
                "               , 0 AS DiscountTax" & vbCrLf &
                "               , 'Returned for Rebate' AS TransactionType " & vbCrLf &
                "           FROM TransInvoiceItems " & vbCrLf &
                "           JOIN TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID " & vbCrLf &
                "           UNION " & vbCrLf &
                "           SELECT TransInvoiceItems.ID" & vbCrLf &
                "        	    , TransInvoiceItems.InvoiceID" & vbCrLf &
                "        	    , TransInvoiceItems.ProductID" & vbCrLf &
                "               , TransInvoiceItems.PriceLevelID" & vbCrLf &
                "               , TransInvoiceItems.Quantity" & vbCrLf &
                "               , TransInvoiceItems.UnitMeasure" & vbCrLf &
                "               , TransInvoiceItems.UnitPrice" & vbCrLf &
                "               , TransInvoiceItems.SubTotal" & vbCrLf &
                "               , TransInvoiceItems.DiscountAmount" & vbCrLf &
                "               , TransInvoiceItems.DiscountedUnitPrice" & vbCrLf &
                "               , TransInvoiceItems.TaxPercent" & vbCrLf &
                "               , TransInvoiceItems.TaxSales" & vbCrLf &
                "               , TransInvoiceItems.DiscountSales" & vbCrLf &
                "               , TransInvoiceItems.TaxAmount" & vbCrLf &
                "               , TransInvoiceItems.DiscountTax" & vbCrLf &
                "               , NULL AS TransactionType " & vbCrLf &
                "           FROM TransInvoiceItems " & vbCrLf &
                "           ) invitem ON invitem.InvoiceID = invlgr.TransactionID AND invitem.ProductID = invlgr.ProductID " & vbCrLf &
                "   INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " & vbCrLf &
                "   LEFT JOIN TransCustomerPaymentInvoices custpayinv ON (custpayinv.InvoiceID = inv.ID) " & vbCrLf &
                "   CROSS APPLY (SELECT " & vbCrLf &
                "       SUM(CONVERT(float, CashAmount - ISNULL(CustomerChangeCash,0))) as CashAmount " & vbCrLf &
                "       , SUM(CONVERT(float, CheckPaymentAmount)) as CheckPaymentAmount " & vbCrLf &
                "       , SUM(CONVERT(float, DebitCardAmount)) as DebitCardAmount " & vbCrLf &
                "       , SUM(CONVERT(float, CreditCardAmount)) as CreditCardAmount " & vbCrLf &
                "       , SUM(CONVERT(float, SRVAmount)) as SRVAmount " & vbCrLf &
                "       , SUM(CONVERT(float, OnCreditAmount)) as OnCreditAmount " & vbCrLf &
                "       , SUM(CONVERT(float, OtherTypeAmount)) as OtherTypeAmount " & vbCrLf &
                "       , TransCustomerPaymentOther.PaymentType " & vbCrLf &
                "       , UserID " & vbCrLf &
                "       FROM TransCustomerPayment" & vbCrLf &
                "       INNER JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPayment.ID AND TransCustomerPaymentInvoices.InvoiceID = inv.ID " & vbCrLf &
                "       LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID " & vbCrLf &
                "       GROUP BY RewardsCardNum, UserID, TransCustomerPaymentOther.PaymentType) custpay " & vbCrLf &
                "   OUTER APPLY( " & vbCrLf &
                "       SELECT TransInvoiceDiscount.*, DiscountType FROM TransInvoiceDiscount " & vbCrLf &
                "           INNER JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID " & vbCrLf &
                "           WHERE InvoiceID = inv.ID " & vbCrLf &
                "   ) as invdis" & vbCrLf &
                "   LEFT JOIN MobizLibraryPeople cshr ON (cshr.ID = custpay.UserID) " & vbCrLf &
                "   INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " & vbCrLf &
                "   WHERE invlgr.TransactionType = 'Invoice' AND invlgr.FowstID = " & ReadyString(fowstid) & vbCrLf &
                "       AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dFrom) & " AND " & ReadyString(dTo) & vbCrLf &
                "       AND inv.DeviceID = " & ReadyString(deviceid)

        Common.WebRequest.ProData.BeginTransaction()
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "IF OBJECT_ID('tempdb..#tbl2') IS NOT NULL DROP TABLE #tbl2 " & vbCrLf &
                "   SELECT DISTINCT fowst.Name as Store " & vbCrLf &
                "       , ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') + " & vbCrLf &
                "           ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " & vbCrLf &
                "           ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " & vbCrLf &
                "       , RIGHT(CAST(sretldr.DeviceFowstTransactionNumber AS VARCHAR(12)), 12) as TransactionNo " & vbCrLf &
                "       , (CASE ISNULL(sretldr.SrvNum,0) WHEN 0 THEN 'Refund' ELSE 'SRV' END) as TransactionType " & vbCrLf &
                "       , RIGHT(CAST((CASE ISNULL(sretldr.SrvNum,0) WHEN 0 THEN sretldr.RefundNum ELSE sretldr.SrvNum END) AS VARCHAR(12)), 12) as ReferenceNo " & vbCrLf &
                "       , memo.MemoDate as TransactionDate " & vbCrLf &
                "       , prd.ProductNo " & vbCrLf &
                "       , prd.ProductName " & vbCrLf &
                "       , ISNULL(cust.CompanyName,'') + ' ' + ISNULL(PersonSurname,'') + ' ' + ISNULL(PersonFirstName,'') + ' ' + ISNULL(PersonMiddleName,'') as Customer " & vbCrLf &
                "       , cust.TinNumber " & vbCrLf &
                "       , CONVERT(float, sret.QuantityReturned) as Quantity " & vbCrLf &
                "       , sret.UnitMeasure " & vbCrLf &
                "       , ROUND(CONVERT(float, sret.UnitPrice), 2) * -1 as UnitPrice" & vbCrLf &
                " 	    , ROUND(CONVERT(float, sret.TotalAmount), 2) * -1 as SubTotal " & vbCrLf &
                "       , invitem.TaxPercent as VATPercent " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) > 0 Or ISNULL(inv.SoloParentDiscount, 0) > 0 THEN 0 ELSE invitem.TaxSales - invitem.DiscountSales END ELSE 0 END)), 2) * -1 as VATableSales " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) > 0 Or ISNULL(inv.SoloParentDiscount, 0) > 0 THEN 0 ELSE invitem.TaxAmount - invitem.DiscountTax END ELSE 0 END)), 2) * -1 as VATAmount " & vbCrLf &
                "       , ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 And ISNULL(inv.SoloParentDiscount, 0) = 0 THEN 0 ELSE invitem.TaxSales - invitem.DiscountSales END ELSE 0 END)), 2) * -1 as VATableExSales " & vbCrLf &
                "       , 0 /*ROUND(CONVERT(float, (CASE WHEN fowst.IsVat = 1 THEN CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 And ISNULL(inv.SoloParentDiscount, 0) = 0 THEN 0 ELSE invitem.SubTotal - (invitem.TaxSales - invitem.DiscountSales) END ELSE 0 END)), 2) * -1*/ as VATExTax " & vbCrLf &
                "       , invdis.DiscountType " & vbCrLf &
                "       , invdis.DiscountPercentage " & vbCrLf &
                "       , ROUND((CASE WHEN fowst.IsVat = 1 THEN CONVERT(decimal, invitem.SubTotal) / (CASE WHEN ISNULL(inv.SCDiscount, 0) = 0 And ISNULL(inv.SoloParentDiscount, 0) = 0 And ISNULL(inv.PhNatTeamDiscount, 0) = 0 THEN 1 ELSE (1 + (invitem.TaxPercent / 100)) END) ELSE CONVERT(decimal, invitem.SubTotal) END) * ((100 - invdis.DiscountPercentage) / 100) + (CASE WHEN ISNULL(inv.PhNatTeamDiscount, 0) = 0 THEN 0 ELSE (invitem.TaxAmount - invitem.DiscountTax) END), 2) * -1 AS AmountDue " & vbCrLf &
                "       , custpay.CashAmount * -1 as CashAmount" & vbCrLf &
                "       , custpay.CheckPaymentAmount * -1 as CheckPaymentAmount" & vbCrLf &
                "       , custpay.DebitCardAmount * -1 as DebitCardAmount" & vbCrLf &
                "       , custpay.SRVAmount " & vbCrLf &
                "       , custpay.OnCreditAmount * -1 as OnCreditAmount" & vbCrLf &
                "       , custpay.OtherTypeAmount * -1 as OtherTypeAmount" & vbCrLf &
                "       , custpay.PaymentType as OtherPaymentType " & vbCrLf &
                "   INTO #tbl2 " & vbCrLf &
                "   FROM TransInventoryLedger invlgr " & vbCrLf &
                "   LEFT JOIN LibraryFowstBin fbin ON (fbin.ID = invlgr.BinID) " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " & vbCrLf &
                "   INNER JOIN TransMemoSalesReturn sret ON (sret.MemoID = invlgr.TransactionID AND sret.ProductID = invlgr.ProductID) " & vbCrLf &
                "   INNER JOIN TransMemo memo ON (sret.MemoID = memo.ID) " & vbCrLf &
                "   INNER JOIN TransSalesReturnLedger sretldr ON (memo.ID = sretldr.SourceMemoID AND sretldr.DestinationPaymentID IS NULL)  " & vbCrLf &
                "   INNER JOIN TransInvoice inv ON (inv.ID = sret.InvoiceID) " & vbCrLf &
                "   INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = sret.InvoiceID) AND invitem.ProductID = sret.ProductID " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " & vbCrLf &
                "   INNER JOIN MobizLibraryPeople cshr ON (cshr.ID = memo.UserID)  " & vbCrLf &
                "   INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " & vbCrLf &
                "   OUTER APPLY( " & vbCrLf &
                "       SELECT TransInvoiceDiscount.*, DiscountType FROM TransInvoiceDiscount " & vbCrLf &
                "           INNER JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID " & vbCrLf &
                "           WHERE InvoiceID = inv.ID " & vbCrLf &
                "   ) as invdis " & vbCrLf &
                "   CROSS APPLY (SELECT " & vbCrLf &
                "       SUM(CONVERT(float, CashAmount - ISNULL(CustomerChangeCash,0))) as CashAmount " & vbCrLf &
                "       , SUM(CONVERT(float, CheckPaymentAmount)) as CheckPaymentAmount " & vbCrLf &
                "       , SUM(CONVERT(float, DebitCardAmount)) as DebitCardAmount " & vbCrLf &
                "       , SUM(CONVERT(float, CreditCardAmount)) as CreditCardAmount " & vbCrLf &
                "       , SUM(CONVERT(float, SRVAmount)) as SRVAmount " & vbCrLf &
                "       , SUM(CONVERT(float, OnCreditAmount)) as OnCreditAmount " & vbCrLf &
                "       , SUM(CONVERT(float, OtherTypeAmount)) as OtherTypeAmount " & vbCrLf &
                "       , TransCustomerPaymentOther.PaymentType " & vbCrLf &
                "       , UserID " & vbCrLf &
                "       FROM TransCustomerPayment" & vbCrLf &
                "       INNER JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPayment.ID AND TransCustomerPaymentInvoices.InvoiceID = inv.ID " & vbCrLf &
                "       LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID " & vbCrLf &
                "       GROUP BY RewardsCardNum, UserID, TransCustomerPaymentOther.PaymentType) custpay " & vbCrLf &
                "   WHERE invlgr.TransactionType = 'Sales Returns' AND invlgr.FowstID = " & ReadyString(fowstid) & vbCrLf &
                "       AND CONVERT(date, memo.MemoDate) BETWEEN " & ReadyString(dFrom) & " AND " & ReadyString(dTo) & vbCrLf &
                "       AND inv.DeviceID = " & ReadyString(deviceid)

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = " SELECT * FROM (SELECT * FROM #tbl1 UNION SELECT * FROM #tbl2) as t ORDER BY TransactionDate "
        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim pSourceURL As String = Common.ServerPath & "temp\AuditTrail"

        If (pDT.Rows.Count > 0) Then
            If Not System.IO.Directory.Exists(pSourceURL) Then
                IO.Directory.CreateDirectory(pSourceURL)
            End If

            pSql = " SELECT ISNULL(dev.UuidDisplay, dev.UUID) as SerialNo, " & vbCrLf &
                    " devfow.POSRegistrationNo, " & vbCrLf &
                    " devfow.BIRMachineNo, " & vbCrLf &
                    " ISNULL(BusinessName, (SELECT TOP 1 BusinessTradeName FROM MobizLibraryOrganizationTradeName WHERE OrganizationID = fowst.OrganizationID)) as BusinessName, " & vbCrLf &
                    " ISNULL(fowst.Code, '') + ' ' + ISNULL(fowst.Name, '') as StoreName, " & vbCrLf &
                    " ISNULL(fowst.IsVAT, CONVERT(bit, 'True')) " & vbCrLf &
                    " , ISNULL(NULLIF(LTRIM(RTRIM(fowst.StreetNumber)) + ' ', ' '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(fowst.Street)) + ', ',', '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(subcrcty.Name)) + ', ',', '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(subcrctry.Name)) + ', ',', '), '') as StoreAddress " & vbCrLf &
                    ", ISNULL((SELECT TinDisplay FROM LibraryDevice WHERE ID = devfow.DeviceID), subcr.TinNumber + '-00' + RIGHT(fowst.TINBranchCode, 3)) as TIN " & vbCrLf &
                    " FROM LibraryDevice as dev " & vbCrLf &
                    " INNER JOIN LibraryDeviceFowst as devfow ON devfow.DeviceID = dev.ID " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON devfow.FowstID = fowst.ID " & vbCrLf &
                    " INNER JOIN dbo.MobizLibraryOrganization subcr ON (fowst.OrganizationID = subcr.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCities subcrcty ON (fowst.FowstCityID = subcrcty.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCountries subcrctry ON (fowst.FowstCountryID = subcrctry.ID) " & vbCrLf &
                    " WHERE devfow.FowstID = " & ReadyString(fowstid) & vbCrLf &
                    " --AND dev.ID = " & ReadyString(fowstid) & vbCrLf
            Dim header As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Dim filename As String = CStr2(header(0)("StoreName") & "_AuditTrail_" & dFrom & "_to_" & dTo).Replace(" ", "_")
            Dim filepath = Common.ServerPath & "temp\AuditTrail\"
            CreateFolder(filepath)
            If (IO.File.Exists(filepath & filename)) Then
                IO.File.Delete(filepath & filename)
            End If

            'TableToExcelFile(pDT, "", "", filepath & filename & ".xls")

            Dim pReport = New ProReport
            pReport.SourceFilename = "AuditTrail"
            pReport.AddTable("Data", pDT)
            pReport.AddTable("Header", header)
            pReport.ExportXmlFile()
            pReport.ExportFile(ProReport.eExportFormat.Pdf, filepath & filename & ".pdf")

            Return filename & ".pdf"
        Else
            Return ""
        End If
    End Function

    Function GetRawData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTFields As DataTable
        Dim pSql As String
        Dim intCntr As Integer
        Dim SqlWhere As String
        Dim strProductColumns As String
        Dim pColumnNames As List(Of String)
        Dim pColumnName As String
        Dim pColumnNameSuffix As Integer

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltcol.[ID]"
                End With
                With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltcol.PIMTemplateID"
                End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltcol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltcol.PIMFeatureID"
                End With
                With .Add("PIMFeatureTable", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureTable"
                End With
                With .Add("PIMFeatureColumn", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureColumn"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "fea.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltcol " &
                " INNER JOIN MobizLibraryPIMTemplate tmplt ON (tmplt.ID = tmpltcol.PIMTemplateID) " &
                " INNER JOIN MobizLibraryPIMFeature fea ON (fea.ID = tmpltcol.PIMFeatureID) "

            .SqlWhere = "WHERE tmplt.OrganizationID = " & ReadyString(OrgID)

            .SqlOrder = "ORDER BY fea.PIMFeatureTable ASC, fea.PIMFeatureColumn ASC"

            pDTFields = .Requery
        End With

        strProductColumns = ""
        pColumnNames = New List(Of String)
        pColumnNames.Add("FOWST".ToLower())
        pColumnNames.Add("DeviceName".ToLower())
        pColumnNames.Add("ProductNo".ToLower())
        pColumnNames.Add("ProductName".ToLower())
        pColumnNames.Add("Bin".ToLower())
        pColumnNames.Add("TransactionType".ToLower())
        pColumnNames.Add("TransactionNo".ToLower())
        pColumnNames.Add("TransactionDate".ToLower())
        pColumnNames.Add("Customer".ToLower())
        pColumnNames.Add("TinNumber".ToLower())
        pColumnNames.Add("Billingaddress".ToLower())
        pColumnNames.Add("DeliveryAddress".ToLower())
        pColumnNames.Add("DeliveryMethod".ToLower())
        pColumnNames.Add("DeliveryDate".ToLower())
        pColumnNames.Add("PriceLevel".ToLower())
        pColumnNames.Add("Quantity".ToLower)
        pColumnNames.Add("UnitMeasure".ToLower())
        pColumnNames.Add("UnitPrice".ToLower())
        pColumnNames.Add("SubTotal".ToLower())
        pColumnNames.Add("DiscountAmount".ToLower())
        pColumnNames.Add("DiscountedUnitPrice".ToLower())
        pColumnNames.Add("DiscountedSubTotal".ToLower())
        pColumnNames.Add("VATPercent".ToLower())
        pColumnNames.Add("VATableSales".ToLower())
        pColumnNames.Add("VATAmount".ToLower())
        pColumnNames.Add("Cost".ToLower())
        pColumnNames.Add("COGS".ToLower())
        pColumnNames.Add("UnitGP".ToLower())
        pColumnNames.Add("UnitGPM".ToLower())
        pColumnNames.Add("LineGP".ToLower())
        pColumnNames.Add("LineGPM".ToLower())
        pColumnNames.Add("LineCost".ToLower())
        pColumnNames.Add("GrossSalesNetOfVATExemptions".ToLower())
        pColumnNames.Add("IsForGovReport".ToLower())
        pColumnNames.Add("SalesOrderNo".ToLower())
        pColumnNames.Add("RefNo".ToLower())
        pColumnNames.Add("ReasonForReconstructingData".ToLower())
        pColumnNames.Add("CashierName".ToLower())
        pColumnNames.Add("SalesRepresentative".ToLower())
        pColumnNames.Add("ProductGroupNo".ToLower())
        pColumnNames.Add("prd.Brand".ToLower())
        pColumnNames.Add("DivisionDescription".ToLower())
        pColumnNames.Add("SRP".ToLower())
        pColumnNames.Add("MarketingText".ToLower())
        pColumnNames.Add("Warranty".ToLower())
        pColumnNames.Add("ModelNo".ToLower())
        pColumnNames.Add("CustomClassification".ToLower())
        pColumnNames.Add("CashAmount".ToLower())
        pColumnNames.Add("CheckPaymentAmount".ToLower())
        pColumnNames.Add("DebitCardAmount".ToLower())
        pColumnNames.Add("CreditCardAmount".ToLower())
        pColumnNames.Add("SRVAmount".ToLower())
        pColumnNames.Add("OnCreditAmount".ToLower())
        pColumnNames.Add("OtherTypeAmount".ToLower())
        pColumnNames.Add("CustomerChangeCash".ToLower())
        pColumnNames.Add("RewardsCardNum".ToLower())

        If (pDTFields.Rows.Count > 0) Then
            intCntr = 0
            While (intCntr < pDTFields.Rows.Count)

                If (pDTFields.Rows(intCntr).Item("PIMFeatureColumn").ToString().Trim().Length > 0) Then
                    pColumnName = pDTFields.Rows(intCntr).Item("ColumnName").ToString()
                    pColumnNameSuffix = 1
                    Do Until pColumnNames.Contains(pColumnName.ToLower()) = False
                        pColumnName = pDTFields.Rows(intCntr).Item("ColumnName").ToString() & pColumnNameSuffix
                        pColumnNameSuffix += 1
                    Loop
                    pColumnNames.Add(pColumnName.ToLower())

                    If (pDTFields.Rows(intCntr).Item("PIMFeatureTable").ToString() = "LibraryPIMProductVariableFeatures001") Then
                        strProductColumns = strProductColumns & ",prd001." & pDTFields.Rows(intCntr).Item("PIMFeatureColumn").ToString() & " as " & EscapeSqlName(pColumnName)
                    ElseIf (pDTFields.Rows(intCntr).Item("PIMFeatureTable").ToString() = "LibraryPIMProductVariableFeatures002") Then
                        strProductColumns = strProductColumns & ",prd002." & pDTFields.Rows(intCntr).Item("PIMFeatureColumn").ToString() & " as " & EscapeSqlName(pColumnName)
                    End If
                End If

                intCntr = intCntr + 1
            End While
        End If

        pSql = "IF OBJECT_ID('tempdb..#tbl1') IS NOT NULL DROP TABLE #tbl1 " & vbCrLf &
                " SELECT DISTINCT " & vbCrLf &
                " invlgr.TransactionID as ID " & vbCrLf &
                " 	,fowst.Name as Store " & vbCrLf &
                " 	,dev.PhoneName as DeviceName " & vbCrLf &
                " 	,prd.ProductNo " & vbCrLf &
                " 	,prd.ProductName " & vbCrLf &
                " 	,fbin.[Name] as Bin" & vbCrLf &
                " 	,ISNULL(invitem.TransactionType, invlgr.TransactionType) AS TransactionType" & vbCrLf &
                " 	,inv.InvoiceNumber as TransactionNo " & vbCrLf &
                " 	,inv.InvoiceDate as TransactionDate " & vbCrLf &
                " 	,ISNULL(cust.CompanyName,'') + ' ' + ISNULL(PersonSurname,'') + ' ' + ISNULL(PersonFirstName,'') + ' ' + ISNULL(PersonMiddleName,'') as Customer" & vbCrLf &
                " 	,cust.TinNumber " & vbCrLf &
                " 	,(CASE inv.DeliveryMethod " & vbCrLf &
                " 		WHEN 'Pickup' THEN ''  " & vbCrLf &
                " 		ELSE ISNULL(NULLIF(LTRIM(RTRIM(custbill.StreetNumber)), ''), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbill.Street)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '')  " & vbCrLf &
                " 	END) as Billingaddress " & vbCrLf &
                " 	,(CASE inv.DeliveryMethod  " & vbCrLf &
                " 		WHEN 'Pickup' THEN ''  " & vbCrLf &
                " 		ELSE ISNULL(NULLIF(LTRIM(RTRIM(custdel.StreetNumber)), ''), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdel.Street)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdelcty.Name)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdelctry.Name)),', '), '')  " & vbCrLf &
                " 	END) as DeliveryAddress " & vbCrLf &
                " 	,ISNULL(inv.DeliveryMethod,'PickUp') as DeliveryMethod " & vbCrLf &
                " 	,inv.DeliveryDate " & vbCrLf &
                " 	,plvl.PriceLevel " & vbCrLf &
                " 	,invitem.Quantity " & vbCrLf &
                " 	,invitem.UnitMeasure " & vbCrLf &
                " 	,invitem.UnitPrice " & vbCrLf &
                " 	,invitem.SubTotal " & vbCrLf &
                " 	,invitem.DiscountAmount " & vbCrLf &
                " 	,invitem.DiscountedUnitPrice " & vbCrLf &
                " 	,invitem.Quantity * invitem.DiscountedUnitPrice as DiscountedSubTotal " & vbCrLf &
                " 	,invitem.TaxPercent as VATPercent " & vbCrLf &
                " 	,invitem.TaxSales - invitem.DiscountSales as VATableSales " & vbCrLf &
                " 	,invitem.TaxAmount - invitem.DiscountTax as VATAmount " & vbCrLf &
                " 	,invitem.Cost " & vbCrLf &
                " 	,invitem.COGS " & vbCrLf &
                " 	,invitem.UnitGP " & vbCrLf &
                " 	,invitem.UnitGPM " & vbCrLf &
                " 	,invitem.LineGP " & vbCrLf &
                " 	,invitem.LineGPM " & vbCrLf &
                " 	,invitem.LineCost " & vbCrLf &
                " 	,ISNULL(invitem.GrossSalesNetOfVATExemptions, 0) as GrossSalesNetOfVATExemptions " & vbCrLf &
                " 	,inv.IsForGovReport " & vbCrLf &
                " 	,inv.SalesOrderNo " & vbCrLf &
                " 	,inv.RefNum as RefNo " & vbCrLf &
                " 	,inv.ReasonForReconstructingData " & vbCrLf &
                " 	,ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') +  " & vbCrLf &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') +  " & vbCrLf &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " & vbCrLf &
                " 	,(SELECT DISTINCT STUFF((SELECT ','+ ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.SurName END,'N/A')" & vbCrLf &
                "    FROM TransInvoiceItemSalesReps LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
                "    WHERE TransInvoiceItemSalesReps.InvoiceItemID = invitem.ID and TransInvoiceItemSalesReps.InvoiceID =inv.ID " & vbCrLf &
                "      FOR XML PATH('')" & vbCrLf &
                "    ), 1, 1,'')" & vbCrLf &
                "    FROM TransInvoiceItemSalesReps) as SalesRepresentative" & vbCrLf &
                " 	,prd.ProductGroupNo " & vbCrLf &
                " 	,prd.Brand " & vbCrLf &
                " 	,prd.DivisionDescription " & vbCrLf &
                " 	,prd.SRP " & vbCrLf &
                " 	,prd.MarketingText " & vbCrLf &
                " 	,prd.Warranty " & vbCrLf &
                " 	,prd.ModelNo " & vbCrLf &
                " 	,prd.CustomClassification " & vbCrLf &
                "   , custpay.CashAmount " & vbCrLf &
                "   , custpay.CheckPaymentAmount " & vbCrLf &
                "   , custpay.DebitCardAmount " & vbCrLf &
                "   , custpay.CreditCardAmount " & vbCrLf &
                "   , custpay.SRVAmount " & vbCrLf &
                "   , custpay.OnCreditAmount " & vbCrLf &
                "   , OtherPayment.[Discount Voucher / On Line Deals] " & vbCrLf &
                "   , OtherPayment.[E-Plus] " & vbCrLf &
                "   , OtherPayment.[FoodPanda] " & vbCrLf &
                "   , OtherPayment.[GCash] " & vbCrLf &
                "   , OtherPayment.[GrabFood] " & vbCrLf &
                "   , OtherPayment.[SMAC] " & vbCrLf &
                "   , OtherPayment.[V-Plus] " & vbCrLf &
                "   , custpay.OtherTypeAmount " & vbCrLf &
                "   , custpay.CustomerChangeCash " & vbCrLf &
                "   , custpay.RewardsCardNum " & vbCrLf

        '"   , custpay.OtherTypeAmount " &

        pSql = pSql & strProductColumns

        pSql = pSql &
                " INTO #tbl1 " & vbCrLf &
                " FROM TransInventoryLedger invlgr  " &
                " LEFT JOIN LibraryFowstBin fbin ON (fbin.ID = invlgr.BinID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " & vbCrLf

        pSql &= " JOIN (" & vbCrLf &
            "           SELECT TransInvoiceItems.ID" & vbCrLf &
            "           ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "           ,TransInvoiceItemsRebates.ProductID" & vbCrLf &
            "           ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity AS Quantity" & vbCrLf &
            "           ,TransInvoiceItemsRebates.UnitMeasure" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * -1 AS UnitPrice" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity * -1 AS SubTotal" & vbCrLf &
            "           ,0 AS DiscountAmount" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * -1 AS DiscountedUnitPrice" & vbCrLf &
            "           ,0 AS TaxPercent" & vbCrLf &
            "           ,0 AS TaxSales" & vbCrLf &
            "           ,0 AS DiscountSales" & vbCrLf &
            "           ,0 AS TaxAmount" & vbCrLf &
            "           ,0 AS DiscountTax" & vbCrLf &
            "           ,0 AS Cost" & vbCrLf &
            "           ,0 AS COGS" & vbCrLf &
            "           ,0 AS UnitGP" & vbCrLf &
            "           ,0 AS UnitGPM" & vbCrLf &
            "           ,0 AS LineGP" & vbCrLf &
            "           ,0 AS LineGPM" & vbCrLf &
            "           ,0 AS LineCost" & vbCrLf &
            "           ,0 AS GrossSalesNetOfVATExemptions" & vbCrLf &
            ",'Returned for Rebate' AS TransactionType" & vbCrLf &
            " 	        FROM TransInvoiceItems" & vbCrLf &
            "            JOIN TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID" & vbCrLf &
            "            UNION" & vbCrLf &
            " 		  SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.InvoiceID" & vbCrLf &
            "        	  ,TransInvoiceItems.ProductID" & vbCrLf &
            "           ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "           ,TransInvoiceItems.Quantity" & vbCrLf &
            "           ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "           ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "           ,TransInvoiceItems.SubTotal" & vbCrLf &
            "           ,TransInvoiceItems.DiscountAmount" & vbCrLf &
            "           ,TransInvoiceItems.DiscountedUnitPrice" & vbCrLf &
            "           ,TransInvoiceItems.TaxPercent" & vbCrLf &
            "           ,TransInvoiceItems.TaxSales" & vbCrLf &
            "           ,TransInvoiceItems.DiscountSales" & vbCrLf &
            "           ,TransInvoiceItems.TaxAmount" & vbCrLf &
            "           ,TransInvoiceItems.DiscountTax" & vbCrLf &
            "           ,TransInvoiceItems.Cost" & vbCrLf &
            "           ,TransInvoiceItems.COGS" & vbCrLf &
            "           ,TransInvoiceItems.UnitGP" & vbCrLf &
            "           ,TransInvoiceItems.UnitGPM" & vbCrLf &
            "           ,TransInvoiceItems.LineGP" & vbCrLf &
            "           ,TransInvoiceItems.LineGPM" & vbCrLf &
            "           ,TransInvoiceItems.LineCost" & vbCrLf &
            "           ,TransInvoiceItems.GrossSalesNetOfVATExemptions" & vbCrLf &
            ",NULL AS TransactionType" & vbCrLf &
            " 		  FROM TransInvoiceItems" & vbCrLf &
            "        )invitem ON invitem.InvoiceID = invlgr.TransactionID" & vbCrLf &
            "        AND invitem.ProductID = invlgr.ProductID" & vbCrLf

        pSql &= " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                " INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " &
                " LEFT JOIN TransCustomerPaymentInvoices custpayinv ON (custpayinv.InvoiceID = inv.ID) " &
                " CROSS APPLY (SELECT   " & vbCrLf &
                " SUM(CashAmount) as CashAmount " & vbCrLf &
                ", SUM(CheckPaymentAmount) as CheckPaymentAmount " & vbCrLf &
                ", SUM(DebitCardAmount) as DebitCardAmount " & vbCrLf &
                ", SUM(CreditCardAmount) as CreditCardAmount " & vbCrLf &
                ", SUM(SRVAmount) as SRVAmount " & vbCrLf &
                ", SUM(OnCreditAmount) as OnCreditAmount " & vbCrLf &
                ", SUM(OtherTypeAmount) as OtherTypeAmount " & vbCrLf &
                ", SUM(CustomerChangeCash) as CustomerChangeCash " & vbCrLf &
                ", RewardsCardNum " & vbCrLf &
                ", UserID " & vbCrLf &
                " FROM TransCustomerPayment " & vbCrLf &
                " INNER JOIN TransCustomerPaymentInvoices on  " & vbCrLf &
                " TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPayment.ID " & vbCrLf &
                " AND TransCustomerPaymentInvoices.InvoiceID = inv.ID " & vbCrLf &
                " GROUP BY RewardsCardNum, UserID) custpay " & vbCrLf &
                "CROSS APPLY (  " & vbCrLf &
                "SELECT * FROM    " & vbCrLf &
                "(SELECT PaymentType, Amount " & vbCrLf &
                "FROM TransCustomerPaymentOther  " & vbCrLf &
                "RIGHT JOIN TransCustomerPaymentInvoices on  TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID  " & vbCrLf &
                "AND TransCustomerPaymentInvoices.InvoiceID = inv.ID ) t  " & vbCrLf &
                "PIVOT(SUM(Amount) " & vbCrLf &
                "FOR PaymentType in ( " & vbCrLf &
                "[Discount Voucher / On Line Deals], [E-Plus], [FoodPanda], [GCash], [GrabFood], [SMAC], [V-Plus])) as OtherPayment) as OtherPayment " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST custbill ON (custbill.ID = inv.BillingAddressID) " &
                " LEFT JOIN MobizLibraryCities custbillcty ON (custbill.FowstCityID = custbillcty.ID)  " &
                " LEFT JOIN MobizLibraryCountries custbillctry ON (custbill.FowstCountryID = custbillctry.ID)  " &
                " LEFT JOIN MobizLibraryOrganizationFOWST custdel ON (custdel.ID = inv.DeliveryAddressID) " &
                " LEFT JOIN MobizLibraryCities custdelcty ON (custdel.FowstCityID = custdelcty.ID)  " &
                " LEFT JOIN MobizLibraryCountries custdelctry ON (custdel.FowstCountryID = custdelctry.ID)  " &
                " INNER JOIN LibraryDevice dev ON (dev.ID = inv.DeviceID) " &
                " LEFT JOIN MobizLibraryPeople cshr ON (cshr.ID = custpay.UserID) " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (prd.ID = prd001.ProductID) " &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures002 prd002 ON (prd.ID = prd002.ProductID) " &
                " LEFT JOIN MobizLibraryPIMPriceLevel plvl ON (plvl.ID = invitem.PriceLevelID) " &
                " WHERE invlgr.TransactionType = 'Invoice' " &
                " AND invlgr.FowstID = " & ReadyString(FOWSTID)

        If CStr2(Common.WebRequest.Data("DeviceID")).Length > 0 Then
            pSql &= " AND inv.DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID")))
        End If

        'If (Convert.ToBoolean(Convert.ToInt16(WebRequest.Instance.Data("IsMonth")))) Then
        '    pSql = pSql & " AND MONTH(inv.InvoiceDate) = " & WebRequest.Instance.Data("Month") & " AND YEAR(inv.InvoiceDate) = " & WebRequest.Instance.Data("Year")
        'Else
        pSql = pSql & " AND CONVERT(date,inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(DateTo.ToString("MM/dd/yyyy"))
        'End If
        Common.WebRequest.ProData.BeginTransaction()
        Common.WebRequest.ProData.CommandTimeout = 10 * 60 * 1000 ' 10 minutes
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        'pSql = pSql & " UNION "

        pSql = "IF OBJECT_ID('tempdb..#tbl2') IS NOT NULL DROP TABLE #tbl2 " &
                " SELECT " & vbCrLf &
                " invlgr.TransactionID as ID " & vbCrLf &
                " 	,fowst.Name as Store " & vbCrLf &
                " 	,dev.PhoneName as DeviceName " & vbCrLf &
                " 	,prd.ProductNo " & vbCrLf &
                " 	,prd.ProductName " & vbCrLf &
                " 	,fbin.[Name] as Bin " & vbCrLf &
                " 	,invlgr.TransactionType + (CASE ISNULL(sretldr.SrvNum,0) " & vbCrLf &
                " 		                        WHEN 0 THEN ' (Sales Refund)' " & vbCrLf &
                " 		                        ELSE ' (SRV)' " & vbCrLf &
                " 	                          END) as TransactionType " & vbCrLf &
                " 	,(CASE ISNULL(sretldr.SrvNum,0) " & vbCrLf &
                " 		WHEN 0 THEN sretldr.RefundNum " & vbCrLf &
                " 		ELSE  sretldr.SrvNum " & vbCrLf &
                " 	END) as TransactionNo " & vbCrLf &
                " 	,memo.MemoDate as TransactionDate " & vbCrLf &
                " 	,cust.CompanyName as Customer " & vbCrLf &
                " 	,cust.TinNumber " & vbCrLf &
                " 	,(CASE inv.DeliveryMethod " & vbCrLf &
                " 		WHEN 'Pickup' THEN ''  " & vbCrLf &
                " 		ELSE ISNULL(NULLIF(LTRIM(RTRIM(custbill.StreetNumber)), ''), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbill.Street)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '')  " & vbCrLf &
                " 	END) as Billingaddress " & vbCrLf &
                " 	,(CASE inv.DeliveryMethod  " & vbCrLf &
                " 		WHEN 'Pickup' THEN ''  " & vbCrLf &
                " 		ELSE ISNULL(NULLIF(LTRIM(RTRIM(custdel.StreetNumber)), ''), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdel.Street)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdelcty.Name)),', '), '') +  " & vbCrLf &
                " 			 ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdelctry.Name)),', '), '')  " & vbCrLf &
                " 	END) as DeliveryAddress " & vbCrLf &
                " 	,ISNULL(inv.DeliveryMethod,'PickUp') as DeliveryMethod " & vbCrLf &
                " 	,inv.DeliveryDate " & vbCrLf &
                " 	,'' as PriceLevel " & vbCrLf &
                " 	,sret.QuantityReturned as Quantity " & vbCrLf &
                " 	,sret.UnitMeasure " & vbCrLf &
                " 	,sret.UnitPrice * -1 as UnitPrice  " & vbCrLf &
                " 	,sret.TotalAmount * -1 as SubTotal " & vbCrLf &
                " 	,0 as DiscountAmount " & vbCrLf &
                " 	,0 as DiscountedUnitPrice " & vbCrLf &
                " 	,0 as DiscountedSubTotal " & vbCrLf &
                " 	,12 as VATPercent " & vbCrLf &
                " 	,(sret.TotalAmount - sret.TaxAmount) * -1 as VATableSales " & vbCrLf &
                " 	,sret.TaxAmount * -1 as VATAmount " & vbCrLf &
                " 	,0 as Cost " & vbCrLf &
                " 	,0 as COGS " & vbCrLf &
                " 	,0 as UnitGP " & vbCrLf &
                " 	,0 as UnitGPM " & vbCrLf &
                " 	,0 as LineGP " & vbCrLf &
                " 	,0 as LineGPM " & vbCrLf &
                " 	,0 as LineCost " & vbCrLf &
                " 	,sret.TotalAmount * -1 as GrossSalesNetOfVATExemptions " & vbCrLf &
                " 	,inv.IsForGovReport " & vbCrLf &
                " 	,inv.SalesOrderNo " & vbCrLf &
                " 	,inv.RefNum as RefNo " & vbCrLf &
                " 	,inv.ReasonForReconstructingData " & vbCrLf &
                " 	,ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') +  " & vbCrLf &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') +  " & vbCrLf &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " & vbCrLf &
                " 	,'' as SalesRepresentative " & vbCrLf &
                " 	,prd.ProductGroupNo " & vbCrLf &
                " 	,prd.Brand " & vbCrLf &
                " 	,prd.DivisionDescription " & vbCrLf &
                " 	,prd.SRP " & vbCrLf &
                " 	,prd.MarketingText " & vbCrLf &
                " 	,prd.Warranty " & vbCrLf &
                " 	,prd.ModelNo " & vbCrLf &
                " 	,prd.CustomClassification " & vbCrLf &
                "   , 0 CashAmount " & vbCrLf &
                "   , 0 CheckPaymentAmount " & vbCrLf &
                "   , 0 DebitCardAmount " & vbCrLf &
                "   , 0 CreditCardAmount " & vbCrLf &
                "   , 0 SRVAmount " & vbCrLf &
                "   , 0 OnCreditAmount " & vbCrLf &
                "   , 0 [Discount Voucher / On Line Deals] " & vbCrLf &
                "   , 0 [E-Plus] " & vbCrLf &
                "   , 0 [FoodPanda] " & vbCrLf &
                "   , 0 [GCash] " & vbCrLf &
                "   , 0 [GrabFood] " & vbCrLf &
                "   , 0 [SMAC] " & vbCrLf &
                "   , 0 [V-Plus] " & vbCrLf &
                "   , 0 OtherTypeAmount " & vbCrLf &
                "   , 0 CustomerChangeCash " & vbCrLf &
                "   , '' RewardsCardNum " & vbCrLf

        '"   , 0 OtherTypeAmount " &

        pSql = pSql & strProductColumns

        pSql = pSql &
                " INTO #tbl2" & vbCrLf &
                " FROM TransInventoryLedger invlgr  " & vbCrLf &
                " LEFT JOIN LibraryFowstBin fbin ON (fbin.ID = invlgr.BinID) " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " & vbCrLf &
                " INNER JOIN TransMemoSalesReturn sret ON (sret.MemoID = invlgr.TransactionID AND sret.ProductID = invlgr.ProductID) " & vbCrLf &
                " INNER JOIN TransMemo memo ON (sret.MemoID = memo.ID) " & vbCrLf &
                " INNER JOIN TransSalesReturnLedger sretldr ON (memo.ID = sretldr.SourceMemoID AND sretldr.DestinationPaymentID IS NULL) " & vbCrLf &
                " INNER JOIN TransInvoice inv ON (inv.ID = sret.InvoiceID) " & vbCrLf &
                " INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST custbill ON (custbill.ID = inv.BillingAddressID) " & vbCrLf &
                " LEFT JOIN MobizLibraryCities custbillcty ON (custbill.FowstCityID = custbillcty.ID)  " & vbCrLf &
                " LEFT JOIN MobizLibraryCountries custbillctry ON (custbill.FowstCountryID = custbillctry.ID)  " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST custdel ON (custdel.ID = inv.DeliveryAddressID) " & vbCrLf &
                " LEFT JOIN MobizLibraryCities custdelcty ON (custdel.FowstCityID = custdelcty.ID)  " & vbCrLf &
                " LEFT JOIN MobizLibraryCountries custdelctry ON (custdel.FowstCountryID = custdelctry.ID)  " & vbCrLf &
                " INNER JOIN LibraryDevice dev ON (dev.ID = memo.DeviceID) " & vbCrLf &
                " INNER JOIN MobizLibraryPeople cshr ON (cshr.ID = memo.UserID) " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (prd.ID = prd001.ProductID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures002 prd002 ON (prd.ID = prd002.ProductID) " & vbCrLf &
                " WHERE invlgr.TransactionType = 'Sales Returns' " & vbCrLf &
                " AND invlgr.FowstID = " & ReadyString(FOWSTID) & vbCr

        'If (Convert.ToBoolean(Convert.ToInt16(WebRequest.Instance.Data("IsMonth")))) Then
        '    pSql = pSql & " AND MONTH(memo.MemoDate) = " & WebRequest.Instance.Data("Month") & " AND YEAR(memo.MemoDate) = " & WebRequest.Instance.Data("Year")
        'Else
        pSql = pSql & " AND CONVERT(date,memo.MemoDate) BETWEEN " & ReadyString(DateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(DateTo.ToString("MM/dd/yyyy")) & vbCrLf
        'End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tbl1" & vbCrLf &
            " UNION" & vbCrLf &
            " SELECT * FROM #tbl2" & vbCrLf
        pSql = pSql & " ORDER BY TransactionDate"

        pDT = Common.WebRequest.ProData.Execute(pSql)
        Common.WebRequest.ProData.RollbackTransaction()

        Return pDT

    End Function

    Function CheckSumForDashboardAndPOSTransaction() As Boolean
        Dim pDTPOSTransaction As DataTable
        Dim dblPOSTransactionGrossSales As Double
        Dim dblPOSTransactionDiscount As Double
        Dim dblPOSTransactionSalesReturn As Double
        Dim dblPOSTransactionNetSales As Double

        Dim pDTDashboardSales As DataTable
        Dim pDTDashboardSalesReturn As DataTable

        Dim dblDTDashboardGrossSales As Double
        Dim dblDTDashboardDiscount As Double
        Dim dblDTDashboardSalesReturn As Double
        Dim dblDTDashboardNetSales As Double

        Dim p As New InsertUpdate("AuditTrailAndDashboardLog")
        Dim pSql As String

        pDTPOSTransaction = GetRawData()
        pDTDashboardSales = GetPOSSalesReportByTrendOrSalesDistribution()
        pDTDashboardSalesReturn = GetSalesReturnsData() '''Please Check Francis H.

        '''Check Criteria
        ''' 1. Check Totals
        dblPOSTransactionGrossSales = CDec2(pDTPOSTransaction.Compute("Sum(GrossSalesNetOfVATExemptions)", "TransactionType = 'Invoice'"))
        dblPOSTransactionDiscount = CDec2(pDTPOSTransaction.Compute("Sum(DiscountAmount)", "TransactionType = 'Invoice'"))
        dblPOSTransactionSalesReturn = CDec2(pDTPOSTransaction.Compute("Sum(GrossSalesNetOfVATExemptions)", "TransactionType = 'Sales Returns (SRV)' OR TransactionType = 'Sales Returns (Sales Refund)'"))
        dblPOSTransactionNetSales = dblPOSTransactionGrossSales - dblPOSTransactionDiscount + dblPOSTransactionSalesReturn

        dblDTDashboardGrossSales = CDec2(pDTDashboardSales.Compute("Sum(Total)", "")) '''Please Check Francis H.
        dblDTDashboardDiscount = CDec2(pDTDashboardSales.Compute("Sum(DiscountTotal)", "")) '''Please Check Francis H.
        dblDTDashboardSalesReturn = CDec2(pDTDashboardSalesReturn.Compute("Sum(Total)", "")) * -1 '''Please Check Francis H.
        dblDTDashboardNetSales = dblDTDashboardGrossSales - dblDTDashboardDiscount + dblDTDashboardSalesReturn '''Please Check Francis H.

        If (dblPOSTransactionGrossSales <> dblDTDashboardGrossSales _
            Or dblPOSTransactionDiscount <> dblDTDashboardDiscount _
            Or dblPOSTransactionSalesReturn <> dblDTDashboardSalesReturn _
            Or dblPOSTransactionNetSales <> dblDTDashboardNetSales) Then

            ''' 2. Check Totals Difference 
            If (Math.Abs(dblPOSTransactionGrossSales - dblDTDashboardGrossSales) > 0.5 _
                Or Math.Abs(dblPOSTransactionDiscount - dblDTDashboardDiscount) > 0.5 _
                Or Math.Abs(dblPOSTransactionSalesReturn - dblDTDashboardSalesReturn) > 0.5 _
                Or Math.Abs(dblPOSTransactionNetSales - dblDTDashboardNetSales) > 0.5) Then

                '''Insert into Log
                p = New InsertUpdate("AuditTrailAndDashboardLog")
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("FOWSTID") = ReadyString(FOWSTID)
                p("DateFrom") = ReadyString(DateFrom.ToString("MM/dd/yyyy"))
                p("DateTo") = ReadyString(DateTo.ToString("MM/dd/yyyy"))
                p("POSTransGrossSales") = dblPOSTransactionGrossSales
                p("POSTransDiscount") = dblPOSTransactionDiscount
                p("POSTransSalesReturn") = dblPOSTransactionSalesReturn
                p("POSTransNetSales") = dblPOSTransactionNetSales
                p("DashboardGrossSales") = dblDTDashboardGrossSales
                p("DashboardDiscount") = dblDTDashboardDiscount
                p("DashboardSalesReturn") = dblDTDashboardSalesReturn
                p("DashboardNetSales") = dblDTDashboardNetSales
                p("GeneratedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("GeneratedDateTime") = "getdate()"
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Dim strBody As String

                strBody = "<div style='font-size: 16; font-family: Arial;'>" &
                    "<h3> Details: </h3><hr/>" &
                    "<br /> <p> FOWST: " & GetFOWSTName(FOWSTID) & "</p>" &
                    "<p> From: " & DateFrom.ToString("MM/dd/yyyy") & "</p>" &
                    "<p> To: " & DateTo.ToString("MM/dd/yyyy") & "</p>" &
                    "<p> Audit Trail Gross Sales: " & Format(dblPOSTransactionGrossSales, "#,###.##") & "  VS  " &
                        "Dashboard Gross Sales: " & Format(dblDTDashboardGrossSales, "#,###.##") & "</p>" &
                    "<p> Audit Trail Discount: " & Format(dblPOSTransactionDiscount, "#,###.##") & "  VS  " &
                        "Dashboard Discount: " & Format(dblDTDashboardDiscount, "#,###.##") & "</p>" &
                    "<p> Audit Trail Sales Return: " & Format(dblPOSTransactionSalesReturn, "#,###.##") & "  VS  " &
                        "Dashboard Sales Return: " & Format(dblDTDashboardSalesReturn, "#,###.##") & "</p>" &
                    "<p> Audit Net Sales: " & Format(dblPOSTransactionNetSales, "#,###.##") & "  VS  " &
                        "Dashboard Net Sales: " & Format(dblDTDashboardNetSales, "#,###.##") & "</p>"

                '#If DEBUG Then
                '                SendMail("customercare@mobiz-apps.com",
                '                        "francis.hernandez@mobiz-apps.com",
                '                        "Mobiz",
                '                        "Error in report " & ReadyString(DateFrom.ToString("MM/dd/yyyy")) & "-" & ReadyString(DateTo.ToString("MM/dd/yyyy")),
                '                        strBody,
                '                        "smtp.live.com",
                '                        587,
                '                        "customercare@mobiz-apps.com",
                '                        "MobizCustCare2221", , , , ,
                '                        True)
                '#Else
                '                SendMail("customercare@mobiz-apps.com",
                '                        "francis.hernandez@mobiz-apps.com",
                '                        "Mobiz",
                '                        "Error in report " & ReadyString(DateFrom.ToString("MM/dd/yyyy")) & "-" & ReadyString(DateTo.ToString("MM/dd/yyyy")),
                '                        strBody,
                '                        "smtp.live.com",
                '                        587,
                '                        "customercare@mobiz-apps.com",
                '                        "MobizCustCare2221", , , , ,
                '                        True)
                '#End If
                Return False
            End If
        End If

        Return True

    End Function

    Function GetSummary() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganization.CompanyName"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("Transaction_MIN", SqlDbType.VarChar)
                    .Expression = "tSource.MIN"
                End With
                With .Add("Total_Sales", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), sum(tSource.VatExcluded), 1),',','')"
                End With
                With .Add("Total_Discounts", SqlDbType.VarChar)
                    .Expression = "sum(tSource.Sold_Amt_Discount)"
                End With
                With .Add("Transaction_Type", SqlDbType.VarChar)
                    .Expression = "dbo.TransInventoryLedger.TransactionType"
                End With
                With .Add("Transaction_Month", SqlDbType.VarChar)
                    .Expression = "tSource.Transaction_Month"
                End With
                With .Add("Transaction_Year", SqlDbType.VarChar)
                    .Expression = "tSource.Transaction_Year"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct ON dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        " LEFT JOIN dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                        "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.TaxSales as VatExcluded, TransInvoice.InvoiceDate as TransactionDate " & vbCrLf &
                        ", CONVERT(varchar, dbo.MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, dbo.LibraryDevice.DeviceNo) AS MIN " & vbCrLf &
                        ", '' as Transaction_ReferenceSI " & vbCrLf &
                        ", TransInvoiceItems.DiscountSales AS Sold_Amt_Discount " & vbCrLf &
                        ", MONTH(TransInvoice.InvoiceDate) AS Transaction_Month " & vbCrLf &
                        ", Year(TransInvoice.InvoiceDate) AS Transaction_Year " & vbCrLf &
                        "FROM dbo.TransInvoice INNER JOIN" & vbCrLf &
                        "dbo.TransInvoiceItems ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN" & vbCrLf &
                        "dbo.LibraryDevice ON dbo.TransInvoice.DeviceID = dbo.LibraryDevice.ID INNER JOIN" & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                        "WHERE TransInvoice.InvoiceNumber > 0 " & vbCrLf &
                        "AND TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount - TransMemoSalesReturn.TaxAmount ) * -1 as VatExcluded, TransMemo.MemoDate as TransactionDate " & vbCrLf &
                        ", CONVERT(varchar, dbo.MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, dbo.LibraryDevice.DeviceNo) AS MIN, CONVERT(varchar,dbo.TransInvoice.InvoiceNumber) as Transaction_ReferenceSI " & vbCrLf &
                        ", '' AS Sold_Amt_Discount " & vbCrLf &
                        ", MONTH(TransMemo.MemoDate) AS Transaction_Month" & vbCrLf &
                        ", Year(TransMemo.MemoDate) AS Transaction_Year" & vbCrLf &
                        "FROM TransMemo JOIN " & vbCrLf &
                        "TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID AND " & vbCrLf &
                        "TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID INNER JOIN " & vbCrLf &
                        "dbo.LibraryDevice ON dbo.TransMemo.DeviceID = dbo.LibraryDevice.ID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.TransMemo.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "INNER JOIN TransInvoice on TransInvoice.ID = TransMemoSalesReturn.InvoiceID " & vbCrLf &
                        "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        ") tSource"

            If CInt(WebRequest.Instance.Data("IsMonth")) Then
                SqlWhere = "AND MONTH(tSource.TransactionDate) = " & WebRequest.Instance.Data("Month") & " AND YEAR(tSource.TransactionDate) = " & WebRequest.Instance.Data("Year")
            Else
                SqlWhere = "AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(CDate(WebRequest.Instance.Data("From"))) & " AND " & ReadyString(CDate(WebRequest.Instance.Data("To")))
            End If

            .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND tSource.TransName > 0 " & SqlWhere

            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganization.CompanyName, dbo.MobizLibraryOrganizationFOWST.Name, tSource.MIN, dbo.TransInventoryLedger.TransactionType, tSource.Transaction_Month, tSource.Transaction_Year"

            pDT = .Requery
            Return pDT
        End With
    End Function

    Function GetSalesReturnsData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                If CBool(IsRawData) Then
                    .Add("MemoNumber", SqlDbType.VarChar)
                    .Add("MemoDate", SqlDbType.VarChar)
                    .Add("SrvNum", SqlDbType.VarChar)
                    .Add("Amount", SqlDbType.VarChar)
                    With .Add("Cashier", SqlDbType.VarChar)
                        .Expression = "cashier.Surname + ', ' + cashier.FirstName"
                    End With
                Else
                    With .Add("Total", SqlDbType.VarChar)
                        .Expression = "SUM(Amount)"
                    End With
                End If

            End With

            .SqlFrom = "FROM TransSalesReturnLedger inner join TransMemo " & vbCrLf &
                        "ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID" & vbCrLf &
                        " INNER JOIN MobizLibraryPeople cashier on cashier.ID = TransSalesReturnLedger.UserID " & vbCrLf

            .SqlWhere = "where TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & vbCrLf &
                        "BETWEEN CONVERT(date," & ReadyString(DateFrom) & ")  " & vbCrLf &
                        "AND CONVERT(date," & ReadyString(DateTo) & ") " & vbCrLf &
                        "AND TransSalesReturnLedger.FowstID = " & ReadyString(FOWSTID)

            If Not CStr2(Common.WebRequest.Data("CashierID")) = vbNullString Then
                .SqlWhere &= " AND TransSalesReturnLedger.UserID = " & ReadyString(CStr2(Common.WebRequest.Data("CashierID")))
            End If

            If CBool(IsRawData) Then
                .SqlOrder = " ORDER BY MemoDate DESC"
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetLastPOSSalesTransaction() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFowst As String() = Split(Common.WebRequest.Data("Fowst"), "|")
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .MaxRecords = 1

            With .Fields
                With .Add("LastInvoiceNumber", SqlDbType.Int)
                    .Expression = "InvoiceNumber"
                End With

                With .Add("LastInvoiceDate", SqlDbType.VarChar)
                    .Expression = "dbo.FormatDate(InvoiceDate,'MMM dd YYY hh:mm TT')"
                End With
            End With

            .SqlFrom = "FROM TransInvoice"
            .SqlWhere = "where InvoiceNumber>0 and FowstID = " & ReadyString(pFowst(0))
            .SqlOrder = "Order By InvoiceDate desc"
            pDT = .Requery
        End With


        Return pDT
    End Function

    Function GetFOWSTName(ByVal FOWSTID) As String
        Dim pSql As String
        Dim dt As DataTable

        pSql = "Select Name from MobizLibraryOrganizationFOWST where ID = " & ReadyString(FOWSTID)

        dt = Common.WebRequest.ProData.Execute(pSql)

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Name")
        Else
            Return ""
        End If
    End Function
#End Region

#Region "Settings"

    Function GetOrganizationProductFeatures() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        Try
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "MobizLibraryPIMFeature.ID"
                    End With
                    .Add("Label", SqlDbType.VarChar)
                End With

                .SqlFrom = "FROM LibraryOrganizationDashboardProductFeatures INNER JOIN " & vbCrLf &
                            "dbo.MobizLibraryPIMFeature ON LibraryOrganizationDashboardProductFeatures.FeatureID = dbo.MobizLibraryPIMFeature.ID INNER JOIN " & vbCrLf &
                            "dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID "

                .SqlOrder = "ORDER BY Label ASC"

                Return .Requery

            End With

        Catch ex As Exception

        End Try

        Return pDT
    End Function

    Function GetAllProductFeatures() As DataTable
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMFeature.ID"
                End With
                .Add("Label", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM dbo.MobizLibraryCountriesLanguageLabel INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMFeature ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID  " & vbCrLf &
                        "inner join MobizLibraryPIMTemplateColumns on MobizLibraryPIMTemplateColumns.PIMFeatureID = MobizLibraryPIMFeature.ID" & vbCrLf &
                        "inner join MobizLibraryPIMTemplate on MobizLibraryPIMTemplate.ID = MobizLibraryPIMTemplateColumns.PIMTemplateID"

            .SqlWhere = "WHERE MobizLibraryPIMFeature.ID NOT IN(SELECT FeatureID FROM LibraryOrganizationDashboardProductFeatures) " & vbCrLf &
                        "AND MobizLibraryPIMTemplate.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf &
                        "AND PIMFeatureColumn <> ''"

            .SqlOrder = "Order By Label"
            Return .Requery
        End With
    End Function

    Function SaveFeatures() As String
        Dim p As New InsertUpdate("LibraryOrganizationDashboardProductFeatures")
        Dim pSql As String
        Dim Features() As String

        Features = Split(WebRequest.Instance.Data("Features"), "|")

        Try

            Common.WebRequest.ProData.BeginTransaction()

            Dim i As Integer

            While i < UBound(Features)
                If Not IsFeatureExist(Features(i)) Then
                    p("FeatureID") = ReadyString(Features(i))
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.Execute(pSql)
                End If
                i += 1
            End While

            Common.WebRequest.ProData.CommitTransaction()

            Return True
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return False
        End Try
    End Function

    Function RemoveFeature() As Boolean
        Dim pSql As String

        pSql = "Delete from LibraryOrganizationDashboardProductFeatures where FeatureID = " & ReadyString(Common.WebRequest.Data("ID"))

        Try
            Common.WebRequest.ProData.Execute(pSql)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function IsFeatureExist(ByVal ID As String) As Boolean
        Dim pSql As String
        Dim dt As DataTable

        pSql = "Select * from LibraryOrganizationDashboardProductFeatures where FeatureID = " & ReadyString(ID)

        dt = Common.WebRequest.ProData.Execute(pSql)

        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

#Region "Order Central"

    Public Enum eSalesOrderReportMode
        QueryNormally
        Recache
        QueryFromCache
    End Enum

    Function GetSalesOrderReport(ByVal searchText As String, vMode As eSalesOrderReportMode) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        'Dim SqlGroupBy As String
        'Dim pFowstID As String = Common.WebRequest.Data("FowstID")
        'Dim pStatus As String = Common.WebRequest.Data("Status")
        Dim pIsInVendorPortal As String = CBool2(Common.WebRequest.Data("IsInVendorPortal"))

        'pFowstID = ReadyGuid(pFowstID)
        'pFinder = New Finder

        'With pFinder
        '    .ProData = Common.WebRequest.ProData
        '    '.MaxRecords = 50
        '    .Distinct = True
        '    .SearchString = searchText
        '    With .Fields
        '        With .Add("ID", SqlDbType.UniqueIdentifier)
        '            .Expression = "inv.ID"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += "inv.ID"

        '        With .Add("SalesOrderNo", SqlDbType.VarChar)
        '            '.Expression = "CONVERT(varchar, fowstMIN.FOWSTNo) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, inv.SalesOrderNo)"
        '            .Expression = "CASE WHEN ISNULL(inv.SalesOrderNo, 0) > 0" & vbCrLf &
        '                " 	THEN 'SO ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.SalesOrderNo), '')" & vbCrLf &
        '                " 	WHEN ISNULL(inv.StrNum, 0) > 0 THEN 'STR ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.StrNum), '')" & vbCrLf &
        '                " 	ELSE 'PO ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.PurchaseOrderNo), '')" & vbCrLf &
        '                " 	END"
        '        End With
        '        SqlGroupBy += ", fowstMIN.FOWSTNo, dev.DeviceNo, inv.SalesOrderNo, inv.PurchaseOrderNo, inv.StrNum" & vbCrLf

        '        With .Add("SalesOrderNoRemoveDash", SqlDbType.VarChar)
        '            .Expression = "CONVERT(varchar, fowstMIN.FOWSTNo) + CONVERT(varchar, dev.DeviceNo) + CONVERT(varchar, inv.SalesOrderNo)"
        '        End With

        '        With .Add("SalesOrderDate", SqlDbType.DateTime)
        '            .Expression = "ISNULL(inv.SalesOrderDate, inv.PurchaseOrderDate)"
        '            .Searchable = False
        '        End With
        '        With .Add("DateNeeded", SqlDbType.DateTime)
        '            .Expression = "inv.DateNeeded"
        '            .Searchable = False
        '        End With
        '        With .Add("StatusSort", SqlDbType.Int)
        '            .Expression = " ISNULL((SELECT TOP 1 CASE Status" & vbCrLf &
        '                "   WHEN 'New' THEN 0" & vbCrLf &
        '                "   WHEN 'Created' THEN 0" & vbCrLf &
        '                "   ELSE 1 END from TransSalesOrderStatus WHERE invoiceid = inv.ID order by lastupdated desc), 0)"
        '            .Searchable = False
        '        End With
        '        With .Add("DateSort", SqlDbType.Int)
        '            .Expression = "CASE WHEN inv.DateNeeded IS NULL THEN 1" & vbCrLf &
        '            " ELSE 0 END"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.SalesOrderDate"

        '        With .Add("DeliveryDate", SqlDbType.DateTime)
        '            .Expression = "inv.DeliveryDate"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.DeliveryDate"


        '        With .Add("SkuCount", SqlDbType.Int)
        '            .Expression = "tblItems.SkuCount"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", tblItems.SkuCount"

        '        With .Add("Total", SqlDbType.Money)
        '            .Expression = "tblItems.Total"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", tblItems.Total"

        '        With .Add("CustomerID", SqlDbType.UniqueIdentifier)
        '            .Expression = "inv.CustomerID"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.CustomerID"

        '        With .Add("Status", SqlDbType.VarChar)
        '            .Expression = "ISNULL((select top 1 status from TransSalesOrderStatus WHERE invoiceid = inv.ID order by lastupdated desc), 'New')"
        '        End With
        '        SqlGroupBy += ", status.Status"

        '        With .Add("CnCUnapprovedReason", SqlDbType.VarChar)
        '            .Expression = "inv.CnCUnapprovedReason"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.CnCUnapprovedReason"

        '        With .Add("CncApprovedByID", SqlDbType.UniqueIdentifier)
        '            .Expression = "inv.CncApprovedByID"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.CncApprovedByID"

        '        With .Add("CustomerName", SqlDbType.VarChar)
        '            .Expression = " (CASE org.IsOrganization " &
        '                          "     WHEN 1 THEN org.CompanyName " &
        '                          "     ELSE " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
        '                          " END)"
        '        End With
        '        SqlGroupBy += ", org.IsOrganization, org.CompanyName, orgprenominal.Code,org.PersonSurname, org.PersonFirstName, org.PersonMiddleName, org.PersonSuffixName "

        '        With .Add("FowstID", SqlDbType.UniqueIdentifier)
        '            .Expression = "inv.FowstID"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.FowstID"

        '        With .Add("FowstName", SqlDbType.VarChar)
        '            .Expression = "fowstMIN.Name"
        '        End With
        '        SqlGroupBy += ", fowstMIN.Name"

        '        With .Add("EncodingFowstName", SqlDbType.VarChar)
        '            .Expression = "tblEncodingFowst.Name"
        '        End With
        '        SqlGroupBy += ", tblEncodingFowst.Name"

        '        .Add("DeliveryFowstName", SqlDbType.VarChar, "fowst.Name")
        '        With .Add("UserName", SqlDbType.VarChar)
        '            .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
        '        End With
        '        SqlGroupBy += ", prenominal.Code, ppl.Surname , ppl.FirstName , ppl.MiddleName,ppl.PersonalSuffixName"

        '        With .Add("UserID", SqlDbType.UniqueIdentifier)
        '            .Expression = "inv.UserID"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.UserID"

        '        With .Add("Address", SqlDbType.VarChar)
        '            .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
        '                            "+ ', ' + ISNULL(fowst.Street, '') " &
        '                            "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
        '        End With
        '        SqlGroupBy += ", fowst.StreetNumber , fowst.Street , cty.Name, ctry.Name "

        '        With .Add("BillingAddress", SqlDbType.VarChar)
        '            .Expression = "dbo.SplitJoinComma(ISNULL(fowstBilling.StreetNumber, '') " &
        '                            "+ ', ' + ISNULL(fowstBilling.Street, '') " &
        '                            "+ ', ' + ISNULL(ctyBilling.Name, '') + ', ' + ISNULL(ctryBilling.Name, ''))"
        '        End With
        '        SqlGroupBy += ", fowstBilling.StreetNumber , fowstBilling.Street , ctyBilling.Name, ctryBilling.Name "


        '        With .Add("SalesRepresentative", SqlDbType.VarChar)
        '            .Expression = "(SELECT TOP 1 ISNULL(NULLIF(LTRIM(RTRIM(prenominalStatus.Code)) + ' ', ' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.Surname)) + ', ',', '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.FirstName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.MiddleName)) + ' ',' '), '') + " &
        '                          "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplStatus.PersonalSuffixName)),', '), '') " &
        '                          "  FROM TransSalesOrderStatus stat LEFT OUTER JOIN MobizLibraryPeople pplStatus ON stat.UserID = pplStatus.ID " &
        '                          " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalStatus ON (pplStatus.TitleID = prenominalStatus.ID)" &
        '                          " WHERE stat.Status IN ('Created', 'New') and stat.InvoiceID = inv.ID) "

        '        End With

        '        With .Add("SupplierName", SqlDbType.VarChar)
        '            .Expression = " (CASE tblSupplier.IsOrganization " &
        '                "     WHEN 1 THEN tblSupplier.CompanyName " &
        '                "     ELSE " &
        '                "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
        '                "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonSurname)) + ', ',', '), '') + " &
        '                "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonFirstName)) + ' ',' '), '') + " &
        '                "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonMiddleName)) + ' ',' '), '') + " &
        '                "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(tblSupplier.PersonSuffixName)),', '), '') " &
        '                " END)"
        '        End With
        '        .Add("SopaApprovedByID", SqlDbType.UniqueIdentifier, "inv.SopaApprovedByID")
        '        .Add("ApprovedDate", SqlDbType.DateTime, "inv.ApprovedDate")

        '        With .Add("NetSuiteReponse", SqlDbType.VarChar)
        '            .Expression = "ISNULL(inv.NetSuiteReponse, inv.NetSuiteError)"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", ISNULL(inv.NetSuiteReponse, inv.NetSuiteError)"

        '        With .Add("LastUpdated", SqlDbType.VarChar)
        '            .Expression = "inv.LastUpdated"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.LastUpdated"

        '        With .Add("Area", SqlDbType.VarChar)
        '            .Expression = "inv.Area"
        '            .Searchable = False
        '        End With
        '        SqlGroupBy += ", inv.Area"

        '        SqlGroupBy += ",inv.PurchaseOrderDate"
        '        SqlGroupBy += ",tblSupplier.IsOrganization"
        '        SqlGroupBy += ",tblSupplier.CompanyName"
        '        SqlGroupBy += ",tblSupplier.PersonSurname"
        '        SqlGroupBy += ",tblSupplier.PersonFirstName"
        '        SqlGroupBy += ",tblSupplier.PersonMiddleName"
        '        SqlGroupBy += ",tblSupplier.PersonSuffixName"
        '        SqlGroupBy &= ",fowst.Name"
        '        SqlGroupBy &= ",inv.SopaApprovedByID"
        '        SqlGroupBy &= ",inv.ApprovedDate"
        '        SqlGroupBy &= ",inv.DateNeeded"

        '        With .Add("SupplierFowstName", SqlDbType.VarChar)
        '            .Expression = "tblSupplierFowst.Name"
        '        End With
        '        SqlGroupBy += ",tblSupplierFowst.Name"
        '        With .Add("CustomerBin", SqlDbType.VarChar)
        '            .Expression = "cusbin.Name"
        '        End With
        '        SqlGroupBy += ",cusbin.Name"
        '        With .Add("CustomerFowst", SqlDbType.VarChar)
        '            .Expression = "cusfowst.Name"
        '        End With
        '        SqlGroupBy += ",cusfowst.Name"
        '        With .Add("Remarks", SqlDbType.VarChar)
        '            .Expression = "inv.Remarks"
        '        End With
        '        SqlGroupBy += ",inv.Remarks"
        '        .Add("PurchaseOrderText", SqlDbType.VarChar, "inv.PurchaseOrderText")
        '        SqlGroupBy += ",inv.PurchaseOrderText"
        '        .Add("DeliveryMethod", SqlDbType.VarChar, "inv.DeliveryMethod")
        '        SqlGroupBy += ",inv.DeliveryMethod"
        '    End With

        '    If vMode = eSalesOrderReportMode.QueryFromCache Then
        '        For Each pField As FinderField In .Fields.Values
        '            pField.Expression = Nothing
        '        Next
        '    End If

        '    .SqlFrom = " FROM TransInvoice inv INNER JOIN " &
        '              "TransInvoiceItems items ON inv.ID = items.InvoiceID INNER JOIN " &
        '              "MobizLibraryPeople ppl ON inv.UserID = ppl.ID INNER JOIN " &
        '              "MobizLibraryOrganization org ON inv.CustomerID = org.ID " &
        '              "INNER JOIN MobizLibraryOrganizationFOWST fowstMIN ON (inv.FowstID = fowstMIN.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryOrganizationFOWST fowst ON (inv.DeliveryAddressID = fowst.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryOrganizationFOWST fowstBilling ON (inv.BillingAddressID = fowstBilling.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON inv.EncodingFowstID = tblEncodingFowst.ID " & vbCrLf &
        '              "LEFT OUTER JOIN LibraryDevice dev ON inv.DeviceID = dev.ID " &
        '              "LEFT OUTER JOIN MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryCities ctyBilling ON (fowstBilling.FowstCityID = ctyBilling.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) " &
        '              "LEFT OUTER JOIN MobizLibraryCountries ctryBilling ON (fowstBilling.FowstCountryID = ctryBilling.ID) " &
        '              "LEFT OUTER JOIN TransSalesOrderStatus status ON inv.ID = status.InvoiceID" & vbCrLf &
        '              "LEFT JOIN MobizLibraryOrganization tblSupplier ON tblSupplier.ID = inv.SupplierID" & vbCrLf &
        '              "LEFT JOIN MobizLibraryOrganizationFOWST tblSupplierFowst ON tblSupplierFowst.ID = inv.SupplierFowstID" & vbCrLf &
        '              " LEFT JOIN LibraryFowstBin as cusbin ON cusbin.ID = inv.CustomerBinID" & vbCrLf &
        '              " LEFT JOIN MobizLibraryOrganizationFowst as cusfowst ON cusfowst.ID = inv.CustomerFowstID " & vbCrLf
        '    .SqlFrom &= " OUTER APPLY (" & vbCrLf &
        '        " 	select top 1 status" & vbCrLf &
        '        "     from TransSalesOrderStatus" & vbCrLf &
        '        "     WHERE invoiceid = inv.ID" & vbCrLf &
        '        "     order by lastupdated desc" & vbCrLf &
        '        " ) tblStatus" & vbCrLf

        '    .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
        '        " 	SELECT SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) AS Total" & vbCrLf & _
        '        "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf & _
        '        " 	FROM TransInvoiceItems" & vbCrLf & _
        '        "   WHERE TransInvoiceItems.InvoiceID = inv.ID" & vbCrLf & _
        '        "   AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf &
        '        " ) tblItems    " & vbCrLf
        '    .SqlWhere = " WHERE (inv.SalesOrderNo IS NOT NULL" & vbCrLf &
        '        "   OR inv.PurchaseOrderNo IS NOT NULL" & vbCrLf &
        '        "   OR inv.StockTransferID IS NOT NULL" & vbCrLf &
        '        ") AND inv.InvoiceNumber < 1" & vbCrLf &
        '        " AND (inv.SalesOrderDate BETWEEN " & ReadyString(Common.WebRequest.Data("OrderDateFrom") & " 00:00 AM") & " AND " & ReadyString(Common.WebRequest.Data("OrderDateTo") & " 23:59 PM") & vbCrLf &
        '        " OR inv.PurchaseOrderDate BETWEEN " & ReadyString(Common.WebRequest.Data("OrderDateFrom") & " 00:00 AM") & " AND " & ReadyString(Common.WebRequest.Data("OrderDateTo") & " 23:59 PM") & vbCrLf &
        '        ") AND tblItems.SkuCount > 0" & vbCrLf &
        '        " -- AND fowstMIN.OrganizationID = " & ReadyString(OrgID) & " -- commendted by Arvin 2019_11_28__16_37_54_7781" & vbCrLf &
        '        " AND inv.IsCancelled = 0" & vbCrLf
        '    If String.IsNullOrWhiteSpace(pStatus) = False Then
        '        .SqlWhere &= " AND ISNULL(tblStatus.Status, 'New') = " & ReadyString(pStatus) & vbCrLf
        '    End If
        '    If String.IsNullOrWhiteSpace(pFowstID) = False _
        '        AndAlso pIsInVendorPortal = False Then

        '        .SqlWhere &= " AND inv.FowstID = " & ReadyString(pFowstID) & vbCrLf
        '    End If
        '    If String.IsNullOrWhiteSpace(Common.WebRequest.Data("VendorID")) Then
        '        .SqlWhere &= " AND (inv.TransType = 'SalesOrder'" & vbCrLf &
        '            " OR (ISNULL(inv.TransType, 'PurchaseOrder') = 'PurchaseOrder'" & vbCrLf
        '        If WebRequest.Instance.Business.SystemDefaults.FcSettingExcludeVendorPoInFc Then
        '            .SqlWhere &= " AND inv.CustomerID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf
        '        End If
        '        .SqlWhere &= ")" & vbCrLf &
        '            ")" & vbCrLf
        '    Else
        '        .SqlWhere &= " AND inv.TransType = 'PurchaseOrder'" & vbCrLf &
        '            " -- inv.CustomerID is vendor ID " & vbCrLf &
        '            " AND inv.CustomerID = " & ReadyString(Common.WebRequest.Data("VendorID")) & vbCrLf
        '    End If
        '    If String.IsNullOrWhiteSpace(Common.WebRequest.Data("Areas")) = False Then
        '        .SqlWhere &= " AND inv.Area IN (" & Common.WebRequest.Data("Areas") & ")" & vbCrLf
        '    End If
        '    ' " AND inv.ApprovedDate IS NOT NULL" & vbCrLf ' removed by Arvin; March 30, 2016
        '    .SqlGroup = " GROUP BY " & SqlGroupBy
        '    .SqlOrder = " ORDER BY StatusSort, "
        '    If vMode = eSalesOrderReportMode.QueryFromCache Then
        '        .SqlGroup = Nothing
        '    End If
        '    If Common.WebRequest.Data("SortBy") = "order date" Then
        '        .SqlOrder &= " ISNULL(SalesOrderDate, PurchaseOrderDate) DESC, DateNeeded DESC"
        '    Else
        '        .SqlOrder &= " DateSort, DateNeeded DESC, ISNULL(SalesOrderDate, PurchaseOrderDate) DESC"
        '    End If

        '    If vMode = eSalesOrderReportMode.Recache Then
        '        If WebRequest.Instance.ProData.TableExist("CacheFc") Then
        '            WebRequest.Instance.ProData.ExecuteSQL("DROP TABLE CacheFc")
        '        End If
        '        .SqlFrom = " INTO CacheFc" & vbCrLf & .SqlFrom
        '    End If
        '    Dim pSql = .CreateSql()
        '    If vMode = eSalesOrderReportMode.Recache Then
        '        If WebRequest.Instance.ProData.TableExist("CacheFc") Then
        '        Else

        '        End If
        '    End If
        '    pDT = Common.WebRequest.ProData.Execute(pSql)
        '    If ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid(DarlingtonTrailOrgID) Then
        '        'AppendToLog("FC query. ref: vb3GQZRDDU5Y!" & vbCrLf & pSql, , "FC Query vb3GQZRDDU5Y")
        '    End If
        '    'pDT = .Requery
        'End With

        pFinder = GetSalesOrderReportFinder(searchText:=searchText,
            vWhere:="",
            vMode:=vMode,
            vFowstID:=Common.WebRequest.Data("FowstID"),
            vStatus:=Common.WebRequest.Data("Status"),
            vIsInVendorPortal:=CBool2(Common.WebRequest.Data("IsInVendorPortal")),
            vOrderDateFrom:=ParseDate(Common.WebRequest.Data("OrderDateFrom")),
            vOrderDateTo:=ParseDate(Common.WebRequest.Data("OrderDateTo")),
            vVendorID:=Common.WebRequest.Data("VendorID"),
            vAreas:=Common.WebRequest.Data("Areas"),
            vSortBy:=Common.WebRequest.Data("SortBy"))
        Dim pSql = pFinder.CreateSql()
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If ReadyGuid(Common.WebRequest.Data("OrgID")) = ReadyGuid(modClient.DucOrgID) _
            OrElse ReadyGuid(Common.WebRequest.Data("OrgID")) = ReadyGuid(modClient.AbcOrgID) Then

            If pIsInVendorPortal = False Then
                For Each pDR As DataRow In pDT.Rows
                    If pDR("CncApprovedByID") Is DBNull.Value Then
                        pDR("Status") = CStr2(pDR("Status")) & "; Unapproved by C&C"
                    Else
                        pDR("Status") = CStr2(pDR("Status")) & "; Approved by C&C"
                    End If

                    If pDR("SopaApprovedByID") Is DBNull.Value Then
                        pDR("Status") = CStr2(pDR("Status")) & "; Unapproved by FC"
                    Else
                        pDR("Status") = CStr2(pDR("Status")) & "; Approved by FC"
                    End If
                Next pDR
            End If
        End If

        Return pDT

    End Function

    Function GetSalesOrderReportFinder(ByVal searchText As String, vWhere As String,
        vMode As eSalesOrderReportMode,
        vFowstID As String,
        vStatus As String,
        vIsInVendorPortal As Boolean,
        vOrderDateFrom As Date?, vOrderDateTo As Date?,
        vVendorID As String, vAreas As String, vSortBy As String) As Finder

        Dim pFinder As Finder
        Dim SqlGroupBy As String
        Dim pCacheTableExist As Boolean
        Dim pSql As String

        If String.IsNullOrWhiteSpace(vFowstID) = False Then
            vFowstID = ReadyGuid(vFowstID)
        End If

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            '.MaxRecords = 50
            .Distinct = True
            .SearchString = searchText
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.ID"
                    .Searchable = False
                End With
                SqlGroupBy += "inv.ID"

                With .Add("SalesOrderNo", SqlDbType.VarChar)
                    '.Expression = "CONVERT(varchar, fowstMIN.FOWSTNo) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, inv.SalesOrderNo)"
                    .Expression = "CASE WHEN ISNULL(inv.SalesOrderNo, 0) > 0" & vbCrLf &
                        " 	THEN 'SO ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.SalesOrderNo), '')" & vbCrLf &
                        " 	WHEN ISNULL(inv.StrNum, 0) > 0 THEN 'STR ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.StrNum), '')" & vbCrLf &
                        " 	ELSE 'PO ' + ISNULL(CONVERT(varchar, fowstMIN.FOWSTNo) + '-', '') + ISNULL(CONVERT(varchar, dev.DeviceNo) + '-', '') + ISNULL(CONVERT(varchar, inv.PurchaseOrderNo), '')" & vbCrLf &
                        " 	END"
                End With
                SqlGroupBy += ", fowstMIN.FOWSTNo, dev.DeviceNo, inv.SalesOrderNo, inv.PurchaseOrderNo, inv.StrNum" & vbCrLf

                With .Add("SalesOrderNoRemoveDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, fowstMIN.FOWSTNo) + CONVERT(varchar, dev.DeviceNo) + CONVERT(varchar, inv.SalesOrderNo)"
                End With

                With .Add("SalesOrderDate", SqlDbType.DateTime)
                    .Expression = "ISNULL(CONVERT(datetime2, inv.SalesOrderDate), CONVERT(datetime2, inv.PurchaseOrderDate))"
                    .Searchable = False
                End With
                With .Add("DateNeeded", SqlDbType.DateTime)
                    .Expression = "inv.DateNeeded"
                    .Searchable = False
                End With
                With .Add("StatusSort", SqlDbType.Int)
                    .Expression = " ISNULL((SELECT TOP 1 CASE Status" & vbCrLf &
                        "   WHEN 'New' THEN 0" & vbCrLf &
                        "   WHEN 'Created' THEN 0" & vbCrLf &
                        "   ELSE 1 END from TransSalesOrderStatus WHERE invoiceid = inv.ID order by lastupdated desc), 0)"
                    .Searchable = False
                End With
                With .Add("DateSort", SqlDbType.Int)
                    .Expression = "CASE WHEN inv.DateNeeded IS NULL THEN 1" & vbCrLf &
                    " ELSE 0 END"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.SalesOrderDate"

                With .Add("DeliveryDate", SqlDbType.DateTime)
                    .Expression = "inv.DeliveryDate"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.DeliveryDate"


                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                    .Searchable = False
                End With
                SqlGroupBy += ", tblItems.SkuCount"

                With .Add("Total", SqlDbType.Money)
                    .Expression = "tblItems.Total"
                    .Searchable = False
                End With
                SqlGroupBy += ", tblItems.Total"

                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CustomerID"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.CustomerID"

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "ISNULL((select top 1 status from TransSalesOrderStatus WHERE invoiceid = inv.ID order by lastupdated desc), 'New')"
                    .Searchable = False
                End With
                SqlGroupBy += ", status.Status"

                With .Add("CnCUnapprovedReason", SqlDbType.VarChar)
                    .Expression = "inv.CnCUnapprovedReason"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.CnCUnapprovedReason"

                With .Add("CncApprovedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CncApprovedByID"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.CncApprovedByID"

                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN org.CompanyName " &
                                  "     ELSE " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                SqlGroupBy += ", org.IsOrganization, org.CompanyName, orgprenominal.Code,org.PersonSurname, org.PersonFirstName, org.PersonMiddleName, org.PersonSuffixName "

                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.FowstID"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.FowstID"

                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowstMIN.Name"
                    .Searchable = False
                End With
                SqlGroupBy += ", fowstMIN.Name"

                With .Add("EncodingFowstName", SqlDbType.VarChar)
                    .Expression = "tblEncodingFowst.Name"
                    .Searchable = False
                End With
                SqlGroupBy += ", tblEncodingFowst.Name"

                .Add("DeliveryFowstName", SqlDbType.VarChar, "fowst.Name")
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                    .Searchable = False
                End With
                SqlGroupBy += ", prenominal.Code, ppl.Surname , ppl.FirstName , ppl.MiddleName,ppl.PersonalSuffixName"

                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.UserID"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.UserID"

                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                    .Searchable = False
                End With
                SqlGroupBy += ", fowst.StreetNumber , fowst.Street , cty.Name, ctry.Name "

                With .Add("BillingAddress", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowstBilling.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowstBilling.Street, '') " &
                                    "+ ', ' + ISNULL(ctyBilling.Name, '') + ', ' + ISNULL(ctryBilling.Name, ''))"
                    .Searchable = False
                End With
                SqlGroupBy += ", fowstBilling.StreetNumber , fowstBilling.Street , ctyBilling.Name, ctryBilling.Name "


                With .Add("SalesRepresentative", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL(NULLIF(LTRIM(RTRIM(prenominalStatus.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplStatus.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplStatus.PersonalSuffixName)),', '), '') " &
                                  "  FROM TransSalesOrderStatus stat LEFT OUTER JOIN MobizLibraryPeople pplStatus ON stat.UserID = pplStatus.ID " &
                                  " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalStatus ON (pplStatus.TitleID = prenominalStatus.ID)" &
                                  " WHERE stat.Status IN ('Created', 'New') and stat.InvoiceID = inv.ID) "
                    .Searchable = False
                End With

                With .Add("CreatedByName", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL(NULLIF(LTRIM(RTRIM(prenominalStatus.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplCreated.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplCreated.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplCreated.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplCreated.PersonalSuffixName)),', '), '') " &
                                  "  FROM MobizLibraryPeople pplCreated " &
                                  " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalStatus ON (pplCreated.TitleID = prenominalStatus.ID)" &
                                  " WHERE pplCreated.ID = inv.CreatedBy) "
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.CreatedBy "

                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = " (CASE tblSupplier.IsOrganization " &
                        "     WHEN 1 THEN tblSupplier.CompanyName " &
                        "     ELSE " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(tblSupplier.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(tblSupplier.PersonSuffixName)),', '), '') " &
                        " END)"
                    .Searchable = False
                End With
                .Add("SopaApprovedByID", SqlDbType.UniqueIdentifier, "inv.SopaApprovedByID")
                With .Add("ApprovedDate", SqlDbType.DateTime, "inv.ApprovedDate")
                    .Searchable = False
                End With

                With .Add("NetSuiteReponse", SqlDbType.VarChar)
                    .Expression = "ISNULL(inv.NetSuiteReponse, inv.NetSuiteError)"
                    .Searchable = False
                End With
                SqlGroupBy += ", ISNULL(inv.NetSuiteReponse, inv.NetSuiteError)"

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "inv.LastUpdated"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.LastUpdated"

                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "inv.Area"
                    .Searchable = False
                End With
                SqlGroupBy += ", inv.Area"
                With .Add("StockTransferID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.StockTransferID"
                    SqlGroupBy += "," & .Expression
                End With

                With .Add("PurchaseOrderNo", SqlDbType.Int)
                    .Expression = "inv.PurchaseOrderNo"
                    SqlGroupBy += "," & .Expression
                    .Searchable = False
                End With

                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                    SqlGroupBy += "," & .Expression
                    .Searchable = False
                End With

                With .Add("PurchaseOrderDate", SqlDbType.DateTime)
                    .Expression = "inv.PurchaseOrderDate"
                    SqlGroupBy += "," & .Expression
                    .Searchable = False
                End With

                With .Add("IsCancelled", SqlDbType.Bit)
                    .Expression = "inv.IsCancelled"
                    SqlGroupBy += "," & .Expression
                    .Searchable = False
                End With

                With .Add("TransType", SqlDbType.VarChar)
                    .Expression = "inv.TransType"
                    SqlGroupBy += "," & .Expression
                    .Searchable = False
                End With

                SqlGroupBy += ",inv.PurchaseOrderDate"
                SqlGroupBy += ",tblSupplier.IsOrganization"
                SqlGroupBy += ",tblSupplier.CompanyName"
                SqlGroupBy += ",tblSupplier.PersonSurname"
                SqlGroupBy += ",tblSupplier.PersonFirstName"
                SqlGroupBy += ",tblSupplier.PersonMiddleName"
                SqlGroupBy += ",tblSupplier.PersonSuffixName"
                SqlGroupBy &= ",fowst.Name"
                SqlGroupBy &= ",inv.SopaApprovedByID"
                SqlGroupBy &= ",inv.ApprovedDate"
                SqlGroupBy &= ",inv.DateNeeded"

                With .Add("SupplierFowstName", SqlDbType.VarChar)
                    .Expression = "tblSupplierFowst.Name"
                    .Searchable = False
                End With
                SqlGroupBy += ",tblSupplierFowst.Name"
                With .Add("CustomerBin", SqlDbType.VarChar)
                    .Expression = "cusbin.Name"
                    .Searchable = False
                End With
                SqlGroupBy += ",cusbin.Name"
                With .Add("CustomerFowst", SqlDbType.VarChar)
                    .Expression = "cusfowst.Name"
                    .Searchable = False
                End With
                SqlGroupBy += ",cusfowst.Name"
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "inv.Remarks"
                    .Searchable = False
                End With
                SqlGroupBy += ",inv.Remarks"
                With .Add("PurchaseOrderText", SqlDbType.VarChar, "inv.PurchaseOrderText")
                    .Searchable = False
                End With
                SqlGroupBy += ",inv.PurchaseOrderText"
                With .Add("DeliveryMethod", SqlDbType.VarChar, "inv.DeliveryMethod")
                    .Searchable = False
                End With
                SqlGroupBy += ",inv.DeliveryMethod"
            End With

            If vMode = eSalesOrderReportMode.QueryFromCache Then
                For Each pField As FinderField In .Fields.Values
                    pField.Expression = Nothing
                Next
            End If

            .SqlFrom = " FROM TransInvoice inv INNER JOIN " &
                      "TransInvoiceItems items ON inv.ID = items.InvoiceID INNER JOIN " &
                      "MobizLibraryPeople ppl ON inv.UserID = ppl.ID INNER JOIN " &
                      "MobizLibraryOrganization org ON inv.CustomerID = org.ID " &
                      "INNER JOIN MobizLibraryOrganizationFOWST fowstMIN ON (inv.FowstID = fowstMIN.ID) " &
                      "LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
                      "LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID) " &
                      "LEFT OUTER JOIN MobizLibraryOrganizationFOWST fowst ON (inv.DeliveryAddressID = fowst.ID) " &
                      "LEFT OUTER JOIN MobizLibraryOrganizationFOWST fowstBilling ON (inv.BillingAddressID = fowstBilling.ID) " &
                      "LEFT OUTER JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON inv.EncodingFowstID = tblEncodingFowst.ID " & vbCrLf &
                      "LEFT OUTER JOIN LibraryDevice dev ON inv.DeviceID = dev.ID " &
                      "LEFT OUTER JOIN MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                      "LEFT OUTER JOIN MobizLibraryCities ctyBilling ON (fowstBilling.FowstCityID = ctyBilling.ID) " &
                      "LEFT OUTER JOIN MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) " &
                      "LEFT OUTER JOIN MobizLibraryCountries ctryBilling ON (fowstBilling.FowstCountryID = ctryBilling.ID) " &
                      "LEFT OUTER JOIN TransSalesOrderStatus status ON inv.ID = status.InvoiceID" & vbCrLf &
                      "LEFT JOIN MobizLibraryOrganization tblSupplier ON tblSupplier.ID = inv.SupplierID" & vbCrLf &
                      "LEFT JOIN MobizLibraryOrganizationFOWST tblSupplierFowst ON tblSupplierFowst.ID = inv.SupplierFowstID" & vbCrLf &
                      " LEFT JOIN LibraryFowstBin as cusbin ON cusbin.ID = inv.CustomerBinID" & vbCrLf &
                      " LEFT JOIN MobizLibraryOrganizationFowst as cusfowst ON cusfowst.ID = inv.CustomerFowstID " & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	select top 1 status" & vbCrLf &
                "     from TransSalesOrderStatus" & vbCrLf &
                "     WHERE invoiceid = inv.ID" & vbCrLf &
                "     order by lastupdated desc" & vbCrLf &
                " ) tblStatus" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                " 	SELECT SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) AS Total" & vbCrLf & _
                "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf & _
                " 	FROM TransInvoiceItems" & vbCrLf & _
                "   WHERE TransInvoiceItems.InvoiceID = inv.ID" & vbCrLf & _
                "   AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf &
                " ) tblItems    " & vbCrLf
            .SqlWhere = " WHERE (" & ComposeFieldName("inv", "SalesOrderNo", vMode) & " IS NOT NULL" & vbCrLf &
                "   OR " & ComposeFieldName("inv", "PurchaseOrderNo", vMode) & " IS NOT NULL" & vbCrLf &
                "   OR " & ComposeFieldName("inv", "StockTransferID", vMode) & " IS NOT NULL" & vbCrLf &
                ") AND " & ComposeFieldName("inv", "InvoiceNumber", vMode) & " < 1" & vbCrLf

            If vOrderDateFrom IsNot Nothing Then
                .SqlWhere &= " AND (CONVERT(date, " & ComposeFieldName("inv", "SalesOrderDate", vMode) & ") BETWEEN " & ReadyString(Format(vOrderDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vOrderDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf &
                    " OR CONVERT(date, " & ComposeFieldName("inv", "PurchaseOrderDate", vMode) & ") BETWEEN " & ReadyString(Format(vOrderDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vOrderDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf &
                    ") " & vbCrLf
            End If

            .SqlWhere &= " AND " & ComposeFieldName("tblItems", "SkuCount", vMode) & " > 0" & vbCrLf &
                 " -- AND fowstMIN.OrganizationID = " & ReadyString(OrgID) & " -- commendted by Arvin 2019_11_28__16_37_54_7781" & vbCrLf &
                 " AND " & ComposeFieldName("inv", "IsCancelled", vMode) & " = 0" & vbCrLf
            If String.IsNullOrWhiteSpace(vStatus) = False Then
                .SqlWhere &= " AND ISNULL(" & ComposeFieldName("tblStatus", "Status", vMode) & ", 'New') = " & ReadyString(vStatus) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vFowstID) = False _
                AndAlso vIsInVendorPortal = False Then

                .SqlWhere &= " AND " & ComposeFieldName("inv", "FowstID", vMode) & " = " & ReadyString(vFowstID) & vbCrLf
            End If

            If String.IsNullOrWhiteSpace(vVendorID) Then
                .SqlWhere &= " AND (" & ComposeFieldName("inv", "TransType", vMode) & " = 'SalesOrder'" & vbCrLf &
                    " OR (ISNULL(NULLIF(LTRIM(" & ComposeFieldName("inv", "TransType", vMode) & "), ''), 'PurchaseOrder') = 'PurchaseOrder'" & vbCrLf
                If WebRequest.Instance.Business.SystemDefaults.FcSettingExcludeVendorPoInFc _
                    AndAlso vMode <> eSalesOrderReportMode.Recache Then

                    .SqlWhere &= " AND " & ComposeFieldName("inv", "CustomerID", vMode) & " = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf
                End If
                .SqlWhere &= ")" & vbCrLf &
                    ")" & vbCrLf
            Else
                .SqlWhere &= " AND " & ComposeFieldName("inv", "TransType", vMode) & " = 'PurchaseOrder'" & vbCrLf &
                    " -- inv.CustomerID is vendor ID " & vbCrLf &
                    " AND " & ComposeFieldName("inv", "CustomerID", vMode) & " = " & ReadyString(vVendorID) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vAreas) = False Then
                .SqlWhere &= " AND " & ComposeFieldName("inv", "Area", vMode) & " IN (" & vAreas & ")" & vbCrLf
            End If

            If String.IsNullOrWhiteSpace(vWhere) = False Then
                Dim pAppendWhere As Boolean

                If vMode = eSalesOrderReportMode.Recache Then
                    pCacheTableExist = WebRequest.Instance.ProData.TableExist("CacheFc")
                    If pCacheTableExist Then
                        pSql = "SELECT TOP 1 COUNT(*) FROM CacheFc"
                        If WebRequest.Instance.ProData.ExecuteScalar(pSql) > 0 Then
                            pAppendWhere = True
                        End If
                    End If
                Else
                    pAppendWhere = True
                End If

                If pAppendWhere Then
                    .SqlWhere &= vbCrLf & " AND " & vWhere & vbCrLf
                End If
            End If
            ' " AND inv.ApprovedDate IS NOT NULL" & vbCrLf ' removed by Arvin; March 30, 2016
            .SqlOrder = " ORDER BY StatusSort, "
            If vMode = eSalesOrderReportMode.QueryFromCache Then
                .SqlFrom = " FROM CacheFc" & vbCrLf
            Else
                .SqlGroup = " GROUP BY " & SqlGroupBy
            End If
            If vSortBy = "order date" Then
                .SqlOrder &= " SalesOrderDate DESC, PurchaseOrderDate DESC, DateNeeded DESC"
            Else
                .SqlOrder &= " DateSort, DateNeeded DESC, SalesOrderDate DESC, PurchaseOrderDate DESC"
            End If

            If vMode = eSalesOrderReportMode.Recache Then
                pCacheTableExist = WebRequest.Instance.ProData.TableExist("CacheFc")
                If pCacheTableExist Then
                    If WebRequest.Instance.ProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#CacheFc', N'U')") IsNot DBNull.Value Then
                        WebRequest.Instance.ProData.ExecuteSQL("DROP TABLE #CacheFc")
                    End If
                    .SqlFrom = " INTO #CacheFc" & vbCrLf & .SqlFrom
                Else
                    .SqlFrom = " INTO CacheFc" & vbCrLf & .SqlFrom
                End If
                If pCacheTableExist = True Then
                    pSql = .CreateSql()
                    WebRequest.Instance.ProData.CommandTimeout = 10 * 60 ' 10 minutes
                    pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only

                    Dim pFields = ""

                    ' for UPDATE
                    For Each pField As FinderField In .Fields.Values
                        If pFields <> "" Then
                            pFields &= ","
                        End If
                        pFields &= pField.FieldName & " = " & "Source." & pField.FieldName & vbCrLf
                    Next
                    pSql = "UPDATE CacheFc" & vbCrLf &
                        " SET " & pFields &
                        " FROM #CacheFc Source" & vbCrLf &
                        " WHERE Source.ID = CacheFc.ID"
                    pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only

                    ' for INSERT 
                    Dim pIU = New InsertUpdate("CacheFc")
                    For Each pField As FinderField In .Fields.Values
                        pIU(pField.FieldName) = "Source." & pField.FieldName
                    Next
                    pIU.FromSql = " FROM #CacheFc Source" & vbCrLf &
                        " WHERE Source.ID NOT IN (" & vbCrLf &
                        "   SELECT ID" & vbCrLf &
                        "   FROM CacheFc" & vbCrLf &
                        ")"
                    pSql = pIU.GetInsert(True)
                    pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                End If ' END pCacheTableExist = True
            End If ' END vMode = eSalesOrderReportMode.Recache
        End With

        Return pFinder

    End Function

    Private Function ComposeFieldName(vTableName As String, vFieldName As String, vMode As eSalesOrderReportMode)

        If vMode = eSalesOrderReportMode.QueryFromCache Then
            Return vFieldName
        ElseIf String.IsNullOrWhiteSpace(vTableName) Then
            Return vFieldName
        Else
            Return vTableName & "." & vFieldName
        End If

    End Function
    Function SaveCreatedBy(ByVal salesRepID As String, ByVal dateCreated As String, Optional vProData As ProData = Nothing) As String
        If vProData Is Nothing Then
            vProData = Common.WebRequest.ProData
        End If
        Dim p As New InsertUpdate("TransSalesOrderStatus")
        Dim pSql As String


        Try

            pSql = "SELECT ID FROM TransSalesOrderStatus where status IN ('Created', 'New') AND InvoiceID=" & ReadyString(InvoiceID)

            Dim pDT As DataTable = vProData.Execute(pSql)

            If (pDT.Rows.Count < 1) Then
                p("Status") = ReadyString("New")

                p("LastUpdated") = ReadyString(dateCreated)
                p("UserID") = ReadyString(salesRepID)

                p("InvoiceID") = ReadyString(InvoiceID)
                p("ID") = "newid()"
                pSql = p.GetInsert()


                vProData.ExecuteSQL(pSql)

            End If

            Return "ok"

        Catch ex As Exception

            Return "Error"
        End Try
    End Function


    Function GetRawSalesOrderData(ByVal fowstID As String, ByVal strID As String, ByVal salesOrderGroupNo As String, ByVal customerID As String, ByVal groupID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlGroupBy As String

        'for CSV Download
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            If (CStr2(salesOrderGroupNo) = "") Then
                'for ordinary sales order
                With .Fields
                    With .Add("SO_NO", SqlDbType.VarChar)
                        .Expression = "' ' + CONVERT(varchar, fowstMIN.FOWSTNo) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, inv.SalesOrderNo)"
                    End With
                    With .Add("SO_DATE", SqlDbType.DateTime)
                        .Expression = "inv.InvoiceDate"
                    End With
                    With .Add("PRD_NO", SqlDbType.VarChar)
                        .Expression = "prd.ProductNo"
                    End With
                    With .Add("PRD_DESC", SqlDbType.VarChar)
                        .Expression = " prd.ProductName"
                    End With
                    With .Add("OR_QTY", SqlDbType.Decimal)
                        .Expression = "invitem.OrderedQuantity"
                    End With
                    With .Add("SVD_QTY", SqlDbType.Decimal)
                        .Expression = "invitem.Quantity"
                    End With
                    With .Add("UNT_MEAS", SqlDbType.VarChar)
                        .Expression = "invitem.UnitMeasure"
                    End With
                    With .Add("UNT_PRC", SqlDbType.Money)
                        .Expression = "invitem.UnitPrice"
                    End With
                    With .Add("SUB_TOTAL", SqlDbType.Money)
                        .Expression = "invitem.GrossSalesNetOfVATExemptions"
                    End With
                    With .Add("CUST_ID", SqlDbType.UniqueIdentifier)
                        .Expression = "inv.CustomerID"
                    End With
                    With .Add("CUST_NAME", SqlDbType.VarChar)
                        .Expression = "(CASE WHEN org.CompanyName  = '' THEN org.PersonSurname + ', ' + org.PersonFirstName ELSE org.CompanyName END)"
                    End With
                End With
                .SqlFrom = " FROM TransInvoiceItems invitem " &
                        " INNER JOIN MobizLibraryPIMProduct prd ON (invitem.ProductID = prd.ID) " &
                        " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                        " INNER JOIN MobizLibraryPeople ppl ON inv.UserID = ppl.ID " &
                        " INNER JOIN MobizLibraryOrganization org ON (inv.CustomerID = org.ID) " &
                        " LEFT JOIN LibraryDevice dev ON inv.DeviceID = dev.ID " &
                        " LEFT JOIN MobizLibraryOrganizationFOWST fowstMIN ON (inv.FowstID = fowstMIN.ID) "

                ' ISNULL(inv.SalesOrderNo, 0) > 0 AND
                .SqlWhere = " WHERE invitem.InvoiceID = " & ReadyString(strID) '& " AND inv.FowstID = " & ReadyString(fowstID) ' last part commented by arvin 2019_01_28__17_48_54_1069

                .SqlOrder = " ORDER BY prd.ProductName, invitem.OrderedQuantity "

            Else
                ' for franchise

                With .Fields

                    With .Add("PRD_NO", SqlDbType.VarChar)
                        .Expression = "pim.ProductNo"
                    End With
                    SqlGroupBy += "pim.ProductNo"

                    With .Add("PRD_DESC", SqlDbType.VarChar)
                        .Expression = "pim.ProductName"
                    End With
                    SqlGroupBy += ", pim.ProductName"

                    With .Add("OR_QTY", SqlDbType.Int)
                        .Expression = "CONVERT(DECIMAL(10,2),tblItems.OrderQty)"
                    End With
                    SqlGroupBy += ", tblItems.OrderQty"

                    With .Add("UNT_MEAS", SqlDbType.VarChar)
                        .Expression = "items.UnitMeasure"
                    End With
                    SqlGroupBy += ", items.UnitMeasure"


                End With

                .SqlFrom = " FROM TransInvoice inv INNER JOIN " &
                         "TransInvoiceItems items ON inv.ID = items.InvoiceID " &
                         "INNER JOIN MobizLibraryPIMProduct pim ON items.ProductID = pim.ID " &
                 " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                    " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                    " INNER JOIN LibraryFranchiseGroupCutOffDetails det on grp.ID = det.FranchiseGroupID " &
                    " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpno on (det.ID = grpno.FranchiseGroupCutOffDetailsID AND grpno.TransInvoiceSalesOrderGroupNo = inv.SalesOrderGroupNo) "


                'add group and customer fowst id to the query
                If (CStr2(customerID) = "" And CStr2(groupID) = "") Then
                    .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                                   " 	SELECT SUM(TransInvoiceItems.OrderedQuantity) AS OrderQty " & vbCrLf & _
                                   " 	FROM TransInvoiceItems INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID)" & vbCrLf & _
                                   "   WHERE (TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND TransInvoiceItems.OrderedQuantity > 0 AND TransInvoiceItems.ProductID = items.ProductID AND TransInvoice.SalesOrderGroupNo = inv.SalesOrderGroupNo " & vbCrLf & _
                                   " ) tblItems    " & vbCrLf
                    .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo =" & salesOrderGroupNo & " AND items.OrderedQuantity > 0 "
                Else
                    .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                                   " 	SELECT SUM(TransInvoiceItems.OrderedQuantity) AS OrderQty " & vbCrLf & _
                                   " 	FROM TransInvoiceItems INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID)" & vbCrLf & _
                                   "   WHERE (TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND TransInvoiceItems.OrderedQuantity > 0 AND TransInvoice.FowstID = inv.FowstID AND TransInvoiceItems.ProductID = items.ProductID AND TransInvoice.SalesOrderGroupNo = inv.SalesOrderGroupNo " & vbCrLf & _
                                   " ) tblItems    " & vbCrLf
                    .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo =" & salesOrderGroupNo & " AND items.OrderedQuantity > 0 "
                    .SqlWhere &= " AND inv.FowstID =" & ReadyString(customerID) & " AND grp.ID = " & ReadyString(groupID)
                End If


                .SqlGroup = " GROUP BY " & SqlGroupBy
                .SqlOrder = " ORDER BY PRD_DESC, OR_QTY"

            End If

            pDT = .Requery

        End With

        Return pDT

    End Function


    Function GetUpdatedStatus(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlGroupBy As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "Status"
                End With

                With .Add("Count", SqlDbType.Int)
                    .Expression = "COUNT(Status)"
                End With
                SqlGroupBy += "Status"


            End With

            .SqlFrom = " FROM TransSalesOrderStatus "
            .SqlWhere = " WHERE InvoiceID = " & ReadyString(id)
            .SqlGroup = " GROUP BY " & SqlGroupBy
            pDT = .Requery
        End With


        Return pDT
    End Function

    Function GetLatestStatus(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "status.LastUpdated"
                End With
                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "status.Status"
                End With
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                End With
            End With

            .SqlFrom = " FROM TransSalesOrderStatus status INNER JOIN MobizLibraryPeople ppl ON ppl.ID = status.UserID " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) "
            .SqlWhere = " WHERE status.InvoiceID = " & ReadyString(id)
            .SqlOrder = " ORDER BY status.LastUpdated DESC"
            pDT = .Requery
        End With



        Return pDT
    End Function

    Function GetSalesOrderPurchaseDetails(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("PurchaserName", SqlDbType.VarChar)
                    .Expression = "PurchaserName"
                End With

                With .Add("PurchaseOrderNo", SqlDbType.VarChar)
                    .Expression = "PurchaseOrderNo"
                End With


                With .Add("PurchaseOrderDate", SqlDbType.Date)
                    .Expression = "PurchaseOrderDate"
                End With

            End With

            .SqlFrom = " FROM TransInvoice "
            .SqlWhere = " WHERE ID = " & ReadyString(id)
            pDT = .Requery
        End With


        Return pDT
    End Function


    Function UpdateOrderStatus(ByVal status As String, ByVal dlCount As Integer, ByVal printCount As Integer) As String
        Dim p As New InsertUpdate("TransSalesOrderStatus")
        Dim pSql As String

        status = CStr2(status).Trim().ToLower()
        If status.Contains("new") Then
            status = "New"
        ElseIf status.Contains("viewed") Then
            status = "Viewed"
        ElseIf status.Contains("modified") Then
            status = "Modified"
        End If
        p("Status") = ReadyString(status)

        If (status = "Printed") Then
            p("PrintCount") = printCount
        ElseIf (status = "Downloaded") Then
            p("DownloadCount") = dlCount
        End If

        p("LastUpdated") = "GetDate()"
        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

        Try

            Common.WebRequest.ProData.BeginTransaction()
            pSql = "SELECT ID FROM TransSalesOrderStatus where UserID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND status=" & ReadyString(status) & " AND InvoiceID=" & ReadyString(InvoiceID)

            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then

                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID")))
            Else
                p("InvoiceID") = ReadyString(InvoiceID)
                p("ID") = "newid()"
                pSql = p.GetInsert()
            End If

            GetSalesOrderReportFinder("", "inv.ID = " & ReadyString(InvoiceID), DashBoardsAndReports.eSalesOrderReportMode.Recache,
                vFowstID:=Nothing,
                vStatus:=Nothing,
                vIsInVendorPortal:=False,
                vOrderDateFrom:=Nothing,
                vOrderDateTo:=Nothing,
                vVendorID:=Nothing,
                vAreas:=Nothing,
                vSortBy:=Nothing)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "ok"

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try
    End Function

    Function GetInvoiceTypes() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.VarChar)
                .Add("InvoiceType", SqlDbType.VarChar)
                .Add("InvoiceAbbreviation", SqlDbType.Date)

            End With

            .SqlFrom = " FROM LibraryInvoiceTypes "

            pDT = .Requery
        End With


        Return pDT
    End Function

    Function SaveInvoiceType() As String
        Dim p As New InsertUpdate("LibraryInvoiceTypes")
        Dim pSql As String

        p("InvoiceType") = ReadyString(Common.WebRequest.Data("InvoiceType"))
        p("InvoiceAbbreviation") = ReadyString(Common.WebRequest.Data("InvoiceAbbreviation"))

        pSql = p.GetInsert

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return "true"
    End Function

    Function CacheCustomerTransactionSummary(pFOWSTID As String, pCustomerID As String, DateFrom As String, DateTo As String, mProData As ProData)
        Dim pFinder As Finder
        Dim pDTSalesAndDiscounts As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        pFinder = New Finder

        With pFinder
            '.Distinct = True
            .ProData = mProData

            With .Fields

                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("InvoiceDate", SqlDbType.VarChar)
                    .Expression = "CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                    SqlOrderBy = "ORDER BY CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                End With
                With .Add("Total", SqlDbType.Decimal)
                    .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With
                With .Add("DiscountTotal", SqlDbType.Decimal)
                    .Expression = "SUM(dbo.TransInvoiceItems.DiscountAmount) "
                End With
                SqlGroupBy = ", CONVERT(date, dbo.TransInvoice.InvoiceDate)"

                SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                            "AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & vbCrLf &
                            " AND " & ReadyString(DateTo) & vbCrLf &
                            " AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf &
                            SqlWhere

                strWhereFowst = " AND (dbo.TransInvoice.FowstID = " & ReadyString(pFOWSTID) & ")"

            End With

            SqlWhere &= strWhereFowst

            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                              "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                              "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                              "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & SqlFrom

            .SqlWhere = SqlWhere

            If Not SqlOrderBy = vbNullString Then
                .SqlOrder = SqlOrderBy
            End If


            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & SqlGroupBy

            pDTSalesAndDiscounts = .Requery

        End With

        Dim pDTReturns As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = mProData
            .Distinct = True

            With .Fields

                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With


                With .Add("Total", SqlDbType.VarChar)
                    .Expression = "SUM(Amount)"
                End With
            End With


            .SqlFrom = "FROM TransSalesReturnLedger inner join TransMemo " & vbCrLf &
                        "ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID INNER JOIN " & vbCrLf &
                        "dbo.TransInvoice ON dbo.TransSalesReturnLedger.SourceInvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

            .SqlWhere = "where TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & vbCrLf &
                        "BETWEEN CONVERT(date," & ReadyString(DateFrom) & ")  " & vbCrLf &
                        "AND CONVERT(date," & ReadyString(DateTo) & ") " & vbCrLf &
                        "AND TransSalesReturnLedger.FowstID = " & ReadyString(pFOWSTID) & vbCrLf &
                        " AND TransInvoice.CustomerID = " & ReadyString(pCustomerID)

            .SqlGroup = " GROUP BY dbo.MobizLibraryOrganizationFOWST.ID , dbo.MobizLibraryOrganizationFOWST.Name "

            pDTReturns = .Requery
        End With


        Dim pIU As New InsertUpdate("TransCustomerTransactionSummaryCache")
        Dim pSql As String
        Dim GrossSales As Double
        Dim Discounts As Double
        Dim Returns As Double
        Dim FowstID As String

        pSql = "Delete from TransCustomerTransactionSummaryCache where convert(date, SalesDate) = " & ReadyString(DateFrom) & vbCrLf &
                " AND FOWSTID = " & ReadyString(CStr2(pDTSalesAndDiscounts.Rows(0).Item("FOWSTID"))) & vbCrLf &
                " AND CustomerID = " & ReadyString(pCustomerID)

        mProData.ExecuteSQL(pSql)

        If pDTSalesAndDiscounts.Rows.Count > 0 Then
            FowstID = CStr2(pDTSalesAndDiscounts.Rows(0).Item("FOWSTID"))
            GrossSales = CDec2(pDTSalesAndDiscounts.Rows(0).Item("Total"))
            Discounts = CDec2(pDTSalesAndDiscounts.Rows(0).Item("DiscountTotal"))
        End If

        If pDTReturns.Rows.Count > 0 Then
            FowstID = CStr2(pDTReturns.Rows(0).Item("FOWSTID"))
            Returns = CDec2(pDTReturns.Rows(0).Item("Total"))
        End If

        pIU("CustomerID") = ReadyString(pCustomerID)
        pIU("FOWSTID") = ReadyString(FowstID)
        pIU("SalesDate") = ReadyString(DateFrom)
        pIU("GrossSales") = GrossSales
        pIU("Discount") = Discounts
        pIU("SalesReturns") = Returns
        pIU("NetSales") = GrossSales - Discounts - Returns
        pIU("COGSGross") = 0
        pIU("COGSNet") = 0

        pSql = pIU.GetInsert()

        mProData.ExecuteSQL(pSql)
    End Function

    Function GetCustomerTransactionInfoForCRM() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.VarChar)
                .Add("FOWSTID", SqlDbType.VarChar)
                .Add("CustomerID", SqlDbType.VarChar)
                .Add("SalesDate", SqlDbType.Date)
                .Add("GrossSales", SqlDbType.Date)
                .Add("NetSales", SqlDbType.Date)
                .Add("SalesReturns", SqlDbType.Date)
                .Add("Discount", SqlDbType.Date)
                .Add("OutStandingReceivables", SqlDbType.Date)
                .Add("OutStandingPayables", SqlDbType.Date)
                .Add("COGSGross", SqlDbType.Date)
                .Add("COGSNet", SqlDbType.Date)
            End With

            .SqlFrom = " FROM TransCustomerTransactionSummaryCache WHERE " & vbCrLf &
                        " convert(date, SalesDate) between " & ReadyString(CStr2(Common.WebRequest.QueryString("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.QueryString("DateTo")))

            If IsNothing(Common.WebRequest.QueryString("CustomerID")) = False Then
                .SqlFrom &= " AND CustomerID = " & ReadyString(CStr2(Common.WebRequest.QueryString("CustomerID"))) & vbCrLf
            End If

            pDT = .Requery

            'Dim pSql As String = ""

            'pSql = "Select SUM(Subtotal) from TransInvoiceItems Where "

        End With

        Return pDT
    End Function

    Function CustomerInfoFromCRM(pJson As String) As String
        Dim pIU As New InsertUpdate("TransCustomerTransactionSummaryCache")
        Dim pSql As String
        Dim pFrown As Frown

        pFrown = JsonConvert.DeserializeObject(Of Frown)(pJson)

        If Not pFrown.d Is Nothing Then
            For Each pRow As Frown.dClass.ResultClass.RowsClass In pFrown.d.Results.Rows
                pIU("ID") = ReadyString(pRow.ID)
                pIU("CustomerID") = ReadyString(pRow.CustomerID)
                pIU("FOWSTID") = ReadyString(pRow.FOWSTID)
                pIU("SalesDate") = ReadyString(pRow.SalesDate)
                pIU("GrossSales") = pRow.GrossSales
                pIU("Discount") = pRow.Discount
                pIU("SalesReturns") = pRow.SalesReturns
                pIU("NetSales") = pRow.GrossSales - pRow.Discount - pRow.SalesReturns
                pIU("COGSGross") = pRow.COGSGross
                pIU("COGSNet") = pRow.COGSNet

                pSql = pIU.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next
        End If

    End Function

    Function GetSeniorCitizenPWDInfo() As DataTable
        Dim pSql As String

        pSql = "Select IDImage + '.jpg' AS IDImage, ISNULL(PersonFullName, PersonFirstName + ' ' + PersonMiddleName + ' ' + PersonSurName) Name from TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                " inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID " & vbCrLf &
                " WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = " & ReadyString(InvoiceID)

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetVouchersReport() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If Status = "All" Then
            whereSearch = " WHERE (voucher.IsActive = 0 OR voucher.IsActive = 1)"
        ElseIf Status = "Redeemed" Then
            whereSearch = " WHERE voucher.IsActive = 0"
        ElseIf Status = "Unredeemed" Then
            whereSearch = " WHERE voucher.IsActive = 1"
        End If

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ReferenceNo + ' ' + ISNULL(VoucherDescription, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY TransactionDate DESC /*" & PageColumnDirection & "*/"
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UserName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.UpdatedOn) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT TransactionDate, ReferenceNo, VoucherDescription as VoucherType, CASE WHEN voucher.IsActive = 0 THEN 'Redeemed' ELSE 'Unredeemed' END as VoucherStatus " & vbCrLf &
                " , fowst.Name as StoreName, fowst.Code as StoreCode " & vbCrLf &
                " , '' as SupercardNumber " & vbCrLf &
                " , (SELECT TOP 1 GroupName " & vbCrLf &
                "   FROM LibraryFranchiseGroup as groups " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganizationFranchisee as franchisee ON franchisee.FranchiseGroupID = groups.ID " & vbCrLf &
                "   AND franchisee.FranchiseeOrgFowstID = inv.FowstID) as Region " & vbCrLf &
                " FROM MobizLibraryVouchers as voucher " & vbCrLf &
                " LEFT JOIN CacheInvoices as inv ON inv.ID = voucher.TransactionID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = inv.FowstID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit & " " & whereOrder

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function
#End Region

#Region "Franchise"
    Function UpdateFranchiseOrderStatus(ByVal status As String, ByVal dlCount As Integer, ByVal printCount As Integer) As String
        Dim p As New InsertUpdate("LibraryFranchiseOrderCutOffDetailsStatus")
        Dim pSql As String

        p("Status") = ReadyString(status)

        If (status = "Printed") Then
            p("PrintCount") = printCount
        ElseIf (status = "Downloaded") Then
            p("DownloadCount") = dlCount
        End If

        p("LastUpdated") = "GetDate()"
        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

        Try

            Common.WebRequest.ProData.BeginTransaction()
            'pSql = "SELECT InvoiceID FROM TransSalesOrderStatus where InvoiceID=" & ReadyString(invoiceid)

            'Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            'If (pDT.Rows.Count > 0) Then

            '    pSql = p.GetUpdate(" InvoiceID = " & ReadyString(invoiceid))
            'Else
            p("InvoiceID") = ReadyString(InvoiceID)
            p("ID") = "newid()"
            pSql = p.GetInsert()
            'End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "ok"

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try
    End Function

    Function GetFranchiseeStatus(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.ID"
                    .Searchable = False
                End With
                With .Add("FranchiseeOrgFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseeOrgFowstID"
                    .Searchable = False
                End With
                With .Add("FranchiseeFowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("OrganizationName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("FranchiseGroupID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseGroupID"
                    .Searchable = False
                End With
                With .Add("FranchiseGroupName", SqlDbType.VarChar)
                    .Expression = "grp.GroupName"
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchisorConfirmed"
                    .Searchable = False
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchiseeConfirmed"
                    .Searchable = False
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "fran.IsActive"
                    .Searchable = False
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.UserID"
                    .Searchable = False
                End With
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "fran.LastUpdated"
                End With
                With .Add("DateRequested", SqlDbType.DateTime)
                    .Expression = "stat.DateRequested"
                End With
                With .Add("DateActivated", SqlDbType.DateTime)
                    .Expression = "stat.DateActivated"
                End With
                With .Add("DateDeactivated", SqlDbType.DateTime)
                    .Expression = "stat.DateDeactivated"
                End With
                With .Add("RequestedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.RequestedByID"
                End With
                With .Add("ActivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ActivatedByID"
                End With
                With .Add("DeactivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.DeactivatedByID"
                End With
                With .Add("RequestedByName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominalRequest.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplRequest.PersonalSuffixName)),', '), '') "
                End With
                With .Add("ActivatedByName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominalApprove.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplApprove.PersonalSuffixName)),', '), '') "
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                    " INNER JOIN MobizLibraryOrganizationFranchiseStatus stat ON (fran.ID = stat.FranchiseID) " &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fran.FranchiseeOrgFowstID = fowst.ID) " &
                    " LEFT OUTER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople ppl ON (fran.UserID = ppl.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
                    " LEFT OUTER JOIN MobizLibraryOrganization org ON (fowst.OrganizationID = org.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople pplRequest ON (stat.RequestedByID = pplRequest.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalRequest ON (pplRequest.TitleID = prenominalRequest.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople pplApprove ON (stat.ActivatedByID = pplApprove.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalApprove ON (pplApprove.TitleID = prenominalApprove.ID) "

            .SqlWhere = " WHERE fran.ID = " + ReadyString(id)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetFranchiseeOrdersUnassigned() As DataTable
        Dim SqlGroupBy As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryFranchiseGroup.ID"
                End With
                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(GroupName,'Unassigned')"
                End With

                With .Add("CutOffName", SqlDbType.VarChar)
                    .Expression = "'All Unassigned Orders'"
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                End With

                With .Add("LastReceivedDate", SqlDbType.DateTime)
                    .Expression = "(select top 1 _InvoiceDateTime FROM TransInvoice INNER JOIN " &
                      "TransInvoiceItems ON TransInvoice.ID = TransInvoiceItems.InvoiceID " &
                       " LEFT JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                       " LEFT JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                      " WHERE LibraryFranchiseGroup.ID IS NULL AND TransInvoice.InvoiceNumber > 0 AND convert(date,TransInvoice._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & " ORDER BY TransInvoice._InvoiceDateTime desc)"
                End With

                With .Add("GrossSales", SqlDbType.Money)
                    .Expression = "SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With

                With .Add("IsRedFlagActive", SqlDbType.Bit)
                    .Expression = "0"
                End With

            End With


            .SqlFrom = "FROM " & vbCrLf &
                "TransInvoiceItems inner join TransInvoice " & vbCrLf &
                "on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                "left join MobizLibraryOrganizationFOWST  " & vbCrLf &
                "on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                "left join Mobiz.dbo.LibraryOrganizationFranchisee  " & vbCrLf &
                "on TransInvoice.FOWSTID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                "left join LibraryFranchiseGroup " & vbCrLf &
                "on LibraryFranchiseGroup.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                    " 	SELECT COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount " & vbCrLf & _
                    " 	FROM TransInvoiceItems " & vbCrLf & _
                    "   LEFT JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID) " & vbCrLf & _
                     " LEFT JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                       " LEFT JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                    "   WHERE TransInvoiceItems.OrderedQuantity > 0 AND LibraryFranchiseGroup.ID IS NULL AND TransInvoice.InvoiceNumber > 0 AND convert(date,TransInvoice._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) &
                    " ) tblItems    " & vbCrLf
            .SqlWhere = " WHERE LibraryFranchiseGroup.ID IS NULL AND TransInvoice.InvoiceNumber > 0 AND convert(date,TransInvoice._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & vbCrLf
            ' added invoicenumber > 0 to filter sales orders. they are not considered as sales yet
            .SqlGroup = " GROUP BY LibraryFranchiseGroup.ID, LibraryFranchiseGroup.GroupName,tblItems.SkuCount "

            '.SqlOrder = " ORDER BY _InvoiceDateTime DESC"

            pDT = .Requery

        End With


        Return pDT


    End Function
    Function GetFranchiseeOrders(ByVal groupID As String, ByVal salesOrderGroupNo As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            .MaxRecords = 50
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "grpCutOffSOGrpNo.ID"
                End With
                With .Add("ActualCutOffDate", SqlDbType.DateTime)
                    .Expression = "grpCutOffSOGrpNo.ActualCutOffDate"
                End With
                With .Add("ActualCutOffStartDate", SqlDbType.DateTime)
                    .Expression = "grpCutOffSOGrpNo.ActualCutOffStartDate"
                End With
                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "grp.GroupName"
                End With

                With .Add("CutOffName", SqlDbType.VarChar)
                    .Expression = "grpCutOffDet.CutOffName"
                End With

                With .Add("FranchiseGroupID", SqlDbType.UniqueIdentifier)
                    .Expression = "grp.ID"
                End With

                With .Add("SalesOrderGroupNo", SqlDbType.Int)
                    .Expression = "grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo"
                End With

                With .Add("FranchisorFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "grp.FranchisorFowstID"
                End With

                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                End With

                With .Add("LastReceivedDate", SqlDbType.DateTime)
                    .Expression = "(select top 1 _InvoiceDateTime FROM TransInvoice INNER JOIN " &
                      "TransInvoiceItems ON TransInvoice.ID = TransInvoiceItems.InvoiceID " &
                       " INNER JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                       " INNER JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                      " INNER JOIN LibraryFranchiseGroupCutOffDetails ON (LibraryFranchiseGroup.ID = LibraryFranchiseGroupCutOffDetails.FranchiseGroupID) " &
                      " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo ON (LibraryFranchiseGroupCutOffDetails.ID = LibraryFranchiseGroupCutOffSOGroupNo.FranchiseGroupCutOffDetailsID " &
                      " AND TransInvoice.SalesOrderGroupNo = LibraryFranchiseGroupCutOffSOGroupNo.TransInvoiceSalesOrderGroupNo ) " &
                      " WHERE LibraryFranchiseGroupCutOffSOGroupNo.ID = grpCutOffSOGrpNo.ID AND TransInvoice.SalesOrderGroupNo = inv.SalesOrderGroupNo ORDER BY TransInvoice._InvoiceDateTime desc)"
                End With

                With .Add("GrossSales", SqlDbType.Money)
                    .Expression = "tblGrossSales.GrossSales"
                End With

                With .Add("IsRedFlagActive", SqlDbType.Bit)
                    .Expression = "ISNULL ((select top 1 TransFranchiseRedFlag.IsActive FROM LibraryFranchiseGroupCutOffSOGroupNo LEFT OUTER JOIN " &
                     " TransFranchiseRedFlag ON (LibraryFranchiseGroupCutOffSOGroupNo.ID = TransFranchiseRedFlag.CutOffGroupID) WHERE LibraryFranchiseGroupCutOffSOGroupNo.ID = grpCutOffSOGrpNo.ID " &
                     " ORDER BY TransFranchiseRedFlag.LastUpdated desc), 0)"
                End With

            End With

            .SqlFrom = " FROM TransInvoice inv INNER JOIN " &
                      "TransInvoiceItems items ON inv.ID = items.InvoiceID " &
                        " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                       " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                      " INNER JOIN LibraryFranchiseGroupCutOffDetails grpCutOffDet ON (grp.ID = grpCutOffDet.FranchiseGroupID) " &
                      " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpCutOffSOGrpNo ON (grpCutOffDet.ID = grpCutOffSOGrpNo.FranchiseGroupCutOffDetailsID AND inv.SalesOrderGroupNo = grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo) "

            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                    " 	SELECT COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount " & vbCrLf & _
                    " 	FROM TransInvoiceItems " & vbCrLf & _
                    "   INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID) " & vbCrLf & _
                     " INNER JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                       " INNER JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                    "   WHERE TransInvoiceItems.OrderedQuantity > 0 AND LibraryFranchiseGroup.ID = grp.ID AND TransInvoice.SalesOrderGroupNo = grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo " & vbCrLf & _
                    " ) tblItems    " & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                   " 	SELECT SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) as GrossSales " & vbCrLf & _
                   " 	FROM TransInvoiceItems " & vbCrLf & _
                   "   INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID) " & vbCrLf & _
                    " INNER JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                       " INNER JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                   "   WHERE TransInvoiceItems.OrderedQuantity > 0 AND LibraryFranchiseGroup.ID = grp.ID AND TransInvoice.SalesOrderGroupNo = grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo " & vbCrLf & _
                   " ) tblGrossSales    " & vbCrLf
            If (CStr2(salesOrderGroupNo) = "") Then
                .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo IS NOT NULL AND grp.ID = " & ReadyString(groupID)
            Else
                .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo = " & ReadyString(salesOrderGroupNo) & " AND grp.ID = " & ReadyString(groupID)
            End If

            .SqlOrder = " ORDER BY grpCutOffSOGrpNo.ActualCutOffDate DESC"
            pDT = .Requery
        End With


        Return pDT
    End Function

    Function GetFranchiseGroupBatchNoID(ByVal franID As String, ByVal groupID As String, ByVal salesOrderGroupNo As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result As String

        pFinder = New Finder
        Try
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "grpCutOffSOGrpNo.ID"
                    End With
                    With .Add("BatchPickingListNo", SqlDbType.Int)
                        .Expression = "grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo"
                    End With

                End With

                .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                         " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                         " INNER JOIN LibraryFranchiseGroupCutOff grpCutOff ON (grp.ID = grpCutOff.FranchiseGroupID) " &
                         " INNER JOIN LibraryFranchiseGroupCutOffDetails grpCutOffDet ON (grp.ID = grpCutOffDet.FranchiseGroupID) " &
                         " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpCutOffSOGrpNo ON (grpCutOffDet.ID = grpCutOffSOGrpNo.FranchiseGroupCutOffDetailsID) "


                .SqlWhere = " WHERE grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo = " & ReadyString(salesOrderGroupNo) & " AND grp.ID = " & ReadyString(groupID)

                If (CStr2(franID) <> "") Then
                    .SqlWhere &= " AND fran.FranchiseeOrgFowstID =" & ReadyString(franID)
                End If

                pDT = .Requery
            End With

            If (pDT.Rows.Count > 0) Then
                result = pDT.Rows(0).Item("ID").ToString()
            End If

            Return result

        Catch ex As Exception
            Return ""
        End Try

    End Function
    Function GetFranchiseeHeader(ByVal fowstID As String, ByVal franID As String, ByVal groupID As String, ByVal salesOrderGroupNo As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("DistributionCenter", SqlDbType.VarChar)
                    .Expression = "fowstFran.Name + ' ' + grp.GroupName"
                End With
                With .Add("BatchPickingListNo", SqlDbType.VarChar)
                    .Expression = "'BL-' + cast(grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo as varchar)"
                End With
                With .Add("SortingListNo", SqlDbType.VarChar)
                    .Expression = "'SL-' + ISNULL(cast(sortList.TransInvoiceSortingListNo as varchar), '')"
                End With
                With .Add("ActualCutOffDate", SqlDbType.Date)
                    .Expression = "CONVERT(VARCHAR(20), grpCutOffSOGrpNo.ActualCutOffDate, 100)"
                End With
                With .Add("FranchiseeOrgFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseeOrgFowstID"
                    .Searchable = False
                End With
                With .Add("FranchiseeFowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "grp.GroupName"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                     " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fran.FranchiseeOrgFowstID = fowst.ID) " &
                     " LEFT OUTER JOIN MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                     " LEFT OUTER JOIN MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) " &
                     " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                     " INNER JOIN MobizLibraryOrganizationFOWST fowstFran ON (grp.FranchisorFowstID = fowstFran.ID) " &
                     " INNER JOIN LibraryFranchiseGroupCutOff grpCutOff ON (grp.ID = grpCutOff.FranchiseGroupID) " &
                     " INNER JOIN LibraryFranchiseGroupCutOffDetails grpCutOffDet ON (grp.ID = grpCutOffDet.FranchiseGroupID) " &
                     " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpCutOffSOGrpNo ON (grpCutOffDet.ID = grpCutOffSOGrpNo.FranchiseGroupCutOffDetailsID) " &
                     " LEFT OUTER JOIN LibraryFranchiseGroupSortingListNo sortList ON (grpCutOffSOGrpNo.ID = sortList.FranchiseGroupBatchNoID AND sortList.FranchiseeOrgFowstID = fran.FranchiseeOrgFowstID) "


            .SqlWhere = " WHERE grpCutOffSOGrpNo.TransInvoiceSalesOrderGroupNo = " & ReadyString(salesOrderGroupNo) & " AND grp.ID = " & ReadyString(groupID) &
                " AND grp.FranchisorFowstID = " & ReadyString(fowstID)

            If (CStr2(franID) <> "") Then
                .SqlWhere &= " AND fran.FranchiseeOrgFowstID =" & ReadyString(franID)
            End If

            pDT = .Requery
        End With


        Return pDT

    End Function

    Function GetProductOrders(ByVal groupCutOffID As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder

        Try
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    With .Add("ProductID", SqlDbType.UniqueIdentifier)
                        .Expression = "items.ProductID"
                    End With
                    With .Add("ProductNo", SqlDbType.VarChar)
                        .Expression = "pim.ProductNo"
                    End With
                    With .Add("ProductName", SqlDbType.VarChar)
                        .Expression = "pim.ProductName"
                    End With
                    With .Add("OrderedQuantity", SqlDbType.Decimal)
                        .Expression = "tblItems.OrderQty"
                    End With
                    With .Add("UnitMeasure", SqlDbType.VarChar)
                        .Expression = "items.UnitMeasure"
                    End With
                End With


                If (CStr2(groupCutOffID) = "") Then

                    .SqlFrom = "FROM " & vbCrLf &
                        "TransInvoiceItems items inner join TransInvoice inv" & vbCrLf &
                        "on items.InvoiceID = inv.ID " & vbCrLf &
                         " INNER JOIN MobizLibraryPIMProduct pim on pim.id = items.ProductID " &
                        "left join MobizLibraryOrganizationFOWST fowst " & vbCrLf &
                        "on fowst.ID = inv.FOWSTID " & vbCrLf &
                        "left join Mobiz.dbo.LibraryOrganizationFranchisee  " & vbCrLf &
                        "on inv.FOWSTID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                        "left join LibraryFranchiseGroup " & vbCrLf &
                        "on LibraryFranchiseGroup.ID = Mobiz.dbo.LibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf

                    .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                  " 	SELECT SUM(TransInvoiceItems.Quantity) AS OrderQty " & vbCrLf & _
                  " 	FROM TransInvoiceItems INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID)" & vbCrLf & _
                        " LEFT JOIN MobizLibraryOrganizationFranchisee fran ON (TransInvoice.FowstID = fran.FranchiseeOrgFowstID) " &
                        " LEFT JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                  "   WHERE TransInvoiceItems.Quantity > 0 AND TransInvoiceItems.ProductID = items.ProductID AND TransInvoice.FowstID = inv.FowstID " & vbCrLf & _
                  " AND grp.ID IS NULL AND TransInvoice.InvoiceNumber > 0 AND convert(date,TransInvoice._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                    " AND " & ReadyString(DateTo.Date) &
                  " ) tblItems    " & vbCrLf


                    .SqlWhere = " WHERE LibraryFranchiseGroup.ID IS NULL AND (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND convert(date,inv._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & vbCrLf

                Else

                    .SqlFrom = " FROM TransInvoice inv " &
                        " INNER JOIN TransInvoiceItems items on inv.id = items.InvoiceID " &
                        " INNER JOIN MobizLibraryPIMProduct pim on pim.id = items.ProductID " &
                        " INNER JOIN MobizLibraryOrganizationFowst fowst on fowst.ID = inv.FowstID  " &
                        " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                        " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                        " INNER JOIN LibraryFranchiseGroupCutOffDetails det on grp.ID = det.FranchiseGroupID " &
                        " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpno on " &
                        " (det.ID = grpno.FranchiseGroupCutOffDetailsID AND grpno.TransInvoiceSalesOrderGroupNo" &
                        "  = inv.SalesOrderGroupNo)"

                    .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                    " 	SELECT SUM(TransInvoiceItems.OrderedQuantity) AS OrderQty " & vbCrLf & _
                    " 	FROM TransInvoiceItems INNER JOIN TransInvoice ON (TransInvoice.ID = TransInvoiceItems.InvoiceID)" & vbCrLf & _
                     " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (TransInvoice.FowstID = fran.FranchiseeOrgFowstID) " &
                        " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                         " INNER JOIN LibraryFranchiseGroupCutOffDetails det on grp.ID = det.FranchiseGroupID " &
                        " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpno on " &
                         " (det.ID = grpno.FranchiseGroupCutOffDetailsID AND grpno.TransInvoiceSalesOrderGroupNo" &
                              "  = TransInvoice.SalesOrderGroupNo)" &
                    "   WHERE TransInvoiceItems.OrderedQuantity > 0 AND TransInvoiceItems.ProductID = items.ProductID AND grpno.ID = " & ReadyString(groupCutOffID) &
                    " ) tblItems    " & vbCrLf


                    .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND items.OrderedQuantity > 0 AND grpno.ID = " & ReadyString(groupCutOffID)
                End If

                .SqlOrder = " ORDER BY ProductName, OrderedQuantity"
                pDT = .Requery
            End With


            Return pDT


        Catch ex As Exception

        End Try

    End Function


    Function GetSalesBreakdownByFranchisee(ByVal groupCutOffID As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder


        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "tblGrossSales.SubTotal"
                End With
            End With


            If (CStr2(groupCutOffID) = "") Then

                .SqlFrom = " FROM TransInvoice inv " &
" LEFT JOIN MobizLibraryOrganizationFowst fowst on fowst.ID = inv.FowstID " &
" LEFT JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
" LEFT JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) "


                .SqlFrom &= " OUTER APPLY (SELECT SUM(GrossSalesNetOfVATExemptions) AS SubTotal" &
                   "  FROM TransInvoiceItems INNER JOIN TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID " &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                    " LEFT JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                   "   WHERE TransInvoiceItems.OrderedQuantity > 0 " &
                   " AND TransInvoice.FowstID = inv.FowstID AND TransInvoice.InvoiceNumber > 0 AND convert(date,TransInvoice._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) &
                " ) tblGrossSales "

                .SqlWhere = " WHERE grp.ID IS NULL AND (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND convert(date,inv._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & vbCrLf

            Else


                .SqlFrom = " FROM TransInvoice inv " &
               " INNER JOIN MobizLibraryOrganizationFowst fowst on fowst.ID = inv.FowstID " &
               " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
               " INNER JOIN LibraryFranchiseGroupCutOffDetails det on grp.ID = det.FranchiseGroupID " &
               "  INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpno on " &
               " (det.ID = grpno.FranchiseGroupCutOffDetailsID AND grpno.TransInvoiceSalesOrderGroupNo" &
               "  = inv.SalesOrderGroupNo) "

                .SqlFrom &= " OUTER APPLY (SELECT SUM(GrossSalesNetOfVATExemptions) AS SubTotal" &
                    "  FROM TransInvoiceItems INNER JOIN TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID " &
                     " INNER JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                     " INNER JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                      " INNER JOIN LibraryFranchiseGroupCutOffDetails on LibraryFranchiseGroup.ID = LibraryFranchiseGroupCutOffDetails.FranchiseGroupID " &
                    "  INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo on LibraryFranchiseGroupCutOffDetails.ID = LibraryFranchiseGroupCutOffSOGroupNo.FranchiseGroupCutOffDetailsID " &
                    "   WHERE TransInvoiceItems.OrderedQuantity > 0 AND LibraryFranchiseGroupCutOffSOGroupNo.ID = grpno.ID " &
                    " AND TransInvoice.FowstID = inv.FowstID AND (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND TransInvoice.SalesOrderGroupNo = inv.SalesOrderGroupNo) tblGrossSales "


                .SqlWhere = " WHERE grpno.ID = " & ReadyString(groupCutOffID)
            End If



            .SqlGroup = " GROUP BY fowst.Name, tblGrossSales.subtotal"

            .SqlOrder = " ORDER BY SubTotal DESC"
            pDT = .Requery
        End With


        Return pDT


    End Function


    Function GetFranchiseeLastReceived(ByVal groupCutOffID As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        pFinder = New Finder


        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.FowstID"
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("ContactNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL('('+ repTelCountry.CallingCode + ')', '') + ISNULL(repTelArea.CallingCode + '-', '') " &
               "+ ISNULL(repTel.ContactNo, '') + ISNULL(' / ' + repMobCountry.CallingCode, '') + " &
                " ISNULL(repMobTelcoPrefixes.Prefix, '') + ISNULL(repMob.ContactNo, '')"
                End With
                With .Add("IsRedFlagActive", SqlDbType.Bit)
                    .Expression = "ISNULL(flg.IsActive, 0)"
                End With
                With .Add("ReasonForRedFlag", SqlDbType.VarChar)
                    .Expression = "ISNULL(flg.ReasonForRedFlag, '')"
                End With
                With .Add("ReasonForRemoveFlag", SqlDbType.VarChar)
                    .Expression = "ISNULL(flg.ReasonForRemoveFlag, '')"
                End With

                If (CStr2(groupCutOffID) = "") Then

                    With .Add("LastReceivedUnassigned", SqlDbType.DateTime)
                        .Expression = "(select top 1 _InvoiceDateTime FROM TransInvoice INNER JOIN " &
                          "TransInvoiceItems ON TransInvoice.ID = TransInvoiceItems.InvoiceID " &
                           " LEFT JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                           " LEFT JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                          " WHERE LibraryFranchiseGroup.ID IS NULL AND TransInvoice.FowstID = inv.FowstID AND TransInvoice.InvoiceNumber > 0 AND convert(date,inv._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & " ORDER BY TransInvoice._InvoiceDateTime desc)"
                    End With
                Else

                    With .Add("LastReceivedDate", SqlDbType.DateTime)
                        .Expression = "(SELECT top 1 _InvoiceDateTime  FROM TransInvoice  " &
                                        " INNER JOIN MobizLibraryOrganizationFranchisee ON (TransInvoice.FowstID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) " &
                        " INNER JOIN LibraryFranchiseGroup ON (MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID) " &
                        " INNER JOIN LibraryFranchiseGroupCutOffDetails on LibraryFranchiseGroup.ID = LibraryFranchiseGroupCutOffDetails.FranchiseGroupID " &
                        " INNER JOIN  LibraryFranchiseGroupCutOffSOGroupNo on (LibraryFranchiseGroupCutOffDetails.ID = LibraryFranchiseGroupCutOffSOGroupNo.FranchiseGroupCutOffDetailsID AND LibraryFranchiseGroupCutOffSOGroupNo.TransInvoiceSalesOrderGroupNo = TransInvoice.SalesOrderGroupNo) " &
                        " WHERE TransInvoice.FowstID = inv.FowstID AND TransInvoice.SalesOrderGroupNo = inv.SalesOrderGroupNo order by _InvoiceDateTime desc)"
                    End With
                End If

            End With

            .SqlFrom = "FROM TransInvoice inv  " &
                    " LEFT JOIN MobizLibraryOrganizationFowst fowst on fowst.ID = inv.FowstID " &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                    " LEFT JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                    " LEFT JOIN LibraryFranchiseGroupCutOffDetails det on grp.ID = det.FranchiseGroupID " &
                    " LEFT JOIN  LibraryFranchiseGroupCutOffSOGroupNo grpno on (det.ID = grpno.FranchiseGroupCutOffDetailsID AND grpno.TransInvoiceSalesOrderGroupNo = inv.SalesOrderGroupNo) " &
                    " LEFT JOIN TransFranchiseRedFlag flg on grpno.ID = flg.CutOffGroupID and flg.FranchiseeFowstID = inv.FowstID " &
                    " LEFT JOIN MobizLibraryOrganizationFowstTel repTel on (fowst.ID = repTel.FOWSTID)" &
                    " LEFT JOIN MobizLibraryCountries repTelCountry on (repTel.CountryID = repTelCountry.ID)" &
                    " LEFT JOIN MobizLibraryAreaCodes repTelArea on (repTel.AreaCodeID = repTelArea.ID)" &
                    " LEFT JOIN MobizLibraryOrganizationFowstMobile repMob on (fowst.ID = repMob.FOWSTID)" &
                    " LEFT JOIN MobizLibraryCountries repMobCountry on (repMob.CountryID = repMobCountry.ID)" &
                    " LEFT JOIN MobizLibraryTelcoPrefixes repMobTelcoPrefixes on (repMob.TelCoPrefixID = repMobTelcoPrefixes.Id)" &
                    " LEFT JOIN MobizLibraryOrganizationFowstEmail repEm on (fowst.ID = repEm.FOWSTID)"

            If (CStr2(groupCutOffID) = "") Then
                .SqlWhere = " WHERE grp.ID IS NULL AND (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND convert(date,inv._InvoiceDateTime) BETWEEN " & ReadyString(DateFrom.Date) &
                " AND " & ReadyString(DateTo.Date) & vbCrLf

                .SqlOrder = " ORDER BY LastReceivedUnassigned DESC "
            Else

                .SqlWhere = " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND grpno.ID = " & ReadyString(groupCutOffID)
                .SqlGroup = " GROUP BY fowst.Name, " &
                  "inv.SalesOrderGroupNo, flg.IsActive," &
                  " flg.ReasonForRedFlag, flg.ReasonForRemoveFlag, repTelCountry.CallingCode ," &
                  " repTelArea.CallingCode,repTel.ContactNo,repMobCountry.CallingCode, " &
                  " repMobTelcoPrefixes.Prefix, repMob.ContactNo, repEm.EmailAddress ,inv.FowstID "
                .SqlOrder = " ORDER BY LastReceivedDate DESC "
            End If



            pDT = .Requery
        End With


        Return pDT




    End Function
    Function CheckFranchiseRedFlag(ByVal grpCutOffID As String) As String
        Dim p As New InsertUpdate("TransFranchiseRedFlag")
        Dim pDT As DataTable
        Dim lastReceived As New DateTime
        Dim pSql As String
        Dim timeDiff As TimeSpan
        Dim pDTExisting As DataTable
        Dim pDTGrp As DataTable

        Dim hrsFormat As String
        Dim limitRedFlag As Integer = WebRequest.Instance.Business.SystemDefaults.FranchiseRedFlagHours
        Dim lastReceivedTime As String
        Dim pDTOpHr As DataTable
        Dim opHourClosingTime As String
        Dim org As New Organization

        Try
            If CDec2(limitRedFlag) > 0 Then



                Dim closingTime As DateTime
                Dim openingTime As DateTime
                Dim lastreceivedplusLimit As DateTime
                Dim totalDiff As Double
                Dim timeDiffOpHrs As TimeSpan

                pDT = GetFranchiseeLastReceived(grpCutOffID)

                If pDT.Rows.Count > 0 Then
                    For Each dataRows As DataRow In pDT.Rows
                        lastReceived = Convert.ToDateTime(dataRows.Item("LastReceivedDate").ToString())
                        timeDiff = DateTime.Now() - lastReceived

                        pSql = "SELECT ID FROM TransFranchiseRedFlag WHERE CutOffGroupID =" & ReadyString(grpCutOffID) & " AND FranchiseeFowstID = " & ReadyString(dataRows.Item("ID").ToString())

                        pDTExisting = Common.WebRequest.ProData.Execute(pSql)

                        If (timeDiff.TotalHours > CDec2(limitRedFlag)) Then

                            hrsFormat = New DateTime(timeDiff.Ticks).ToString("HH:mm")


                            pDTOpHr = org.getOrgFOWSTOperationHRListByWeekday(dataRows.Item("ID").ToString(), lastReceived.DayOfWeek + 1) '+1 because monday = 2
                            If (pDTOpHr.Rows.Count > 0) Then
                                'get the closing datetime
                                closingTime = Convert.ToDateTime(lastReceived.ToShortDateString() & " " & pDTOpHr.Rows(0).Item("ClosingTime").ToString())

                                'add n hours to the lastreceived time to see if it's within the operating hours
                                lastreceivedplusLimit = lastReceived.AddHours(CDec2(limitRedFlag))
                                'check the difference. if greater than 0 then it's beyond closing hrs so no need to flag 
                                timeDiffOpHrs = closingTime - lastreceivedplusLimit
                                If (timeDiffOpHrs.TotalHours > 0) Then


                                    If (pDTExisting.Rows.Count < 1) Then

                                        p("CutOffGroupID") = ReadyString(grpCutOffID)
                                        p("LastReceived") = ReadyString(lastReceived)
                                        p("FranchiseeFowstID") = ReadyString(dataRows.Item("ID").ToString())
                                        p("ReasonForRedFlag") = ReadyString("Last data received as of " & DateTime.Now().ToString("dddd, dd MMMM yyyy hh:mm tt") & " was on " & lastReceived.ToString("dddd, dd MMMM yyyy hh:mm tt") & " (" & hrsFormat & " hours ago" & ")")

                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                                    End If
                                Else
                                    'next, check opening hrs
                                    'default it temporarily to today
                                    'check if franchisee has operating hours today, if none then dont flag.
                                    pDTOpHr = org.getOrgFOWSTOperationHRListByWeekday(dataRows.Item("ID").ToString(), DateTime.Now().DayOfWeek + 1) '+1 because monday = 2
                                    If (pDTOpHr.Rows.Count > 0) Then
                                        openingTime = Convert.ToDateTime(DateTime.Now().ToShortDateString() & " " & pDTOpHr.Rows(0).Item("OpeningTime").ToString())
                                        timeDiff = DateTime.Now() - openingTime
                                        totalDiff = timeDiff.TotalHours + timeDiffOpHrs.TotalHours

                                        If (totalDiff > CDec2(limitRedFlag)) Then

                                            If (pDTExisting.Rows.Count < 1) Then

                                                p("CutOffGroupID") = ReadyString(grpCutOffID)
                                                p("LastReceived") = ReadyString(lastReceived)
                                                p("FranchiseeFowstID") = ReadyString(dataRows.Item("ID").ToString())
                                                p("ReasonForRedFlag") = ReadyString("Last data received as of " & DateTime.Now().ToString("dddd, dd MMMM yyyy hh:mm tt") & " was on " & lastReceived.ToString("dddd, dd MMMM yyyy hh:mm tt") & " (" & hrsFormat & " hours ago" & ")")

                                                pSql = p.GetInsert()
                                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                            End If
                                        End If
                                    End If




                                End If




                            Else


                                If (pDTExisting.Rows.Count < 1) Then

                                    p("CutOffGroupID") = ReadyString(grpCutOffID)
                                    p("LastReceived") = ReadyString(lastReceived)
                                    p("FranchiseeFowstID") = ReadyString(dataRows.Item("ID").ToString())
                                    p("ReasonForRedFlag") = ReadyString("Last data received as of " & DateTime.Now().ToString("dddd, dd MMMM yyyy hh:mm tt") & " was on " & lastReceived.ToString("dddd, dd MMMM yyyy hh:mm tt") & " (" & hrsFormat & " hours ago" & ")")

                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                                End If


                            End If






                        Else
                            If (pDTExisting.Rows.Count > 0) Then
                                p("IsActive") = 0
                                p("ReasonForRemoveFlag") = ReadyString("Last received has been updated. New last received is " & lastReceived.ToString("dddd, dd MMMM yyyy hh:mm tt"))


                                pSql = p.GetUpdate("ID=" & ReadyString(pDTExisting.Rows(0).Item("ID").ToString()))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                            End If





                        End If
                    Next


                End If



            End If

            Return "ok"

        Catch ex As Exception

            Return "Error"
        End Try


    End Function

    Function SaveRemoveRedFlagReason(ByVal grpCutOffID As String, ByVal franchiseeFowstID As String, ByVal reason As String) As String
        Dim p As New InsertUpdate("TransFranchiseRedFlag")
        Dim pDT As DataTable
        Dim pSql As String

        Try
            pSql = "SELECT ID FROM TransFranchiseRedFlag WHERE CutOffGroupID =" & ReadyString(grpCutOffID) & " AND FranchiseeFowstID = " & ReadyString(franchiseeFowstID)

            pDT = Common.WebRequest.ProData.Execute(pSql)
            If (pDT.Rows.Count > 0) Then
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("ReasonForRemoveFlag") = ReadyString(reason)
                p("IsActive") = "0"

                pSql = p.GetUpdate("ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If

            Return "ok"
        Catch ex As Exception
            Return "Error"
        End Try




    End Function

    Function GetFranchiseGroups() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With

                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "GroupName"
                End With

            End With

            .SqlFrom = " FROM LibraryFranchiseGroup"

            pDT = .Requery
        End With


        Return pDT
    End Function

    Function GetFranchiseeGroupInfo(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlGroupBy As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.ID"
                    .Searchable = False
                End With

                With .Add("FranchisorFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchisorFowstID"
                    .Searchable = False
                End With

                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With

                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With

                With .Add("FranchiseeCount", SqlDbType.Int)
                    .Expression = "tblFranchisee.FranchiseeCount"
                End With

            End With


            .SqlFrom = " FROM LibraryFranchiseGroup fran " &
                    " INNER JOIN MobizLibraryOrganizationFranchisee orgfran ON (fran.ID = orgfran.FranchiseGroupID)" &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fran.FranchisorFowstID = fowst.ID) " &
                    " INNER JOIN MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) "
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                 " 	SELECT COUNT(DISTINCT FranchiseeOrgFowstID) AS FranchiseeCount " & vbCrLf & _
                 " 	FROM MobizLibraryOrganizationFranchisee " & vbCrLf & _
                 "   WHERE FranchisorOrgID = " & ReadyString(OrgID) & " AND FranchiseGroupID = fran.ID " & vbCrLf &
                 " ) tblFranchisee    " & vbCrLf
            .SqlWhere = " WHERE orgfran.FranchisorOrgID = " & ReadyString(OrgID) & " AND fran.ID = " & ReadyString(id)

            pDT = .Requery
        End With


        Return pDT
    End Function

    Function GetFranchisees(ByVal searchText As String, ByVal hasGroup As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            .SearchString = searchText

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.ID"
                    .Searchable = False
                End With
                With .Add("FranchiseStatusID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ID"
                    .Searchable = False
                End With
                With .Add("FranchiseeOrgFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseeOrgFowstID"
                    .Searchable = False
                End With
                With .Add("FranchiseeFowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("OrganizationName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("FranchiseGroupID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseGroupID"
                    .Searchable = False
                End With
                With .Add("FranchiseGroupName", SqlDbType.VarChar)
                    .Expression = "grp.GroupName"
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchisorConfirmed"
                    .Searchable = False
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchiseeConfirmed"
                    .Searchable = False
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "fran.IsActive"
                    .Searchable = False
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.UserID"
                    .Searchable = False
                End With
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "fran.LastUpdated"
                End With
                With .Add("DateRequested", SqlDbType.DateTime)
                    .Expression = "stat.DateRequested"
                End With
                With .Add("DateActivated", SqlDbType.DateTime)
                    .Expression = "stat.DateActivated"
                End With
                With .Add("DateDeactivated", SqlDbType.DateTime)
                    .Expression = "stat.DateDeactivated"
                End With
                With .Add("RequestedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.RequestedByID"
                End With
                With .Add("ActivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ActivatedByID"
                End With
                With .Add("DeactivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.DeactivatedByID"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                    " INNER JOIN MobizLibraryOrganizationFranchiseStatus stat ON (fran.ID = stat.FranchiseID) " &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fran.FranchiseeOrgFowstID = fowst.ID) " &
                    " LEFT OUTER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople ppl ON (fran.UserID = ppl.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
                    " LEFT OUTER JOIN MobizLibraryOrganization org ON (fowst.OrganizationID = org.ID) " &
                    " LEFT OUTER JOIN MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                    "LEFT OUTER JOIN MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) "

            .SqlWhere = " WHERE fran.IsActive = 1 AND fran.FranchisorOrgID = " + ReadyString(OrgID)

            If (hasGroup) Then
                .SqlWhere &= " AND fran.FranchiseGroupID IS NOT NULL "
            End If


            .SqlOrder = " ORDER BY fran.LastUpdated DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function


    Function GetMaxSalesOrderGroupNo(ByVal fowstID As String) As Integer
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String
        pSql = "SELECT MAX(inv.SalesOrderGroupNo) as MaxCount FROM TransInvoice inv INNER JOIN MobizLibraryOrganizationFranchisee fran " &
            " ON (fran.FranchiseeOrgFowstID = inv.FowstID) INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
            " WHERE grp.FranchisorFowstID = " & ReadyString(fowstID)

        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Return CDec2(pDT.Rows(0).Item("MaxCount").ToString())
    End Function

    Function GetFranchisorFowstID(ByVal grpID As String) As String
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String

        pSql = "SELECT FranchisorFowstID FROM LibraryFranchiseGroup WHERE ID= " & ReadyString(grpID)

        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Return CStr2(pDT.Rows(0).Item("FranchisorFowstID").ToString())
    End Function


    Function UpdateSalesOrderGroupNo(ByVal grpID As String) As String
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String


        Dim franchisorFowstID As String = GetFranchisorFowstID(grpID)

        Dim dateTo As String
        Dim dateToMatching As String
        Dim newInvoiceDate As String
        Dim dateCoverageStart As String
        Dim pDTFindMatchingCutOff As DataTable
        Dim pDTLatestGroupStartDate As DataTable
        Dim pDTLatestGroupEndDate As DataTable
        Dim pDTLatestCutOff As DataTable
        Dim pDTEffectivityDate As DataTable
        Dim pDTUngroupedInvoices As DataTable

        Dim theBeginningDateMatching As DateTime
        Dim dateToStartMatching As String
        Try

            Common.WebRequest.ProData.BeginTransaction()

            ' this query gets the first date of the latest salesordergroupno
            'todo
            pSql = "SELECT TOP 1 inv._InvoiceDateTime, inv.SalesOrderGroupNo " &
                " FROM TransInvoice inv INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo IS NOT NULL AND grp.FranchisorFowstID = " & ReadyString(franchisorFowstID) &
                " ORDER BY inv.SalesOrderGroupNo desc, _InvoiceDateTime"

            pDTLatestGroupStartDate = Common.WebRequest.ProData.Execute(pSql)

            Dim today As Date = Date.Today
            Dim dayDiff As Integer

            Dim isNew As Boolean = False

            Dim dayDiffMatching As Integer
            Dim theLimitDateMatching As Date

            Dim count As Integer = 0


            If (pDTLatestGroupStartDate.Rows.Count > 0 Or HasExistingCutOff) Then
                dateCoverageStart = pDTLatestGroupStartDate.Rows(0).Item("_InvoiceDateTime").ToString()


                pSql = "SELECT inv.ID, inv._InvoiceDateTime FROM TransInvoice inv " &
                    " INNER JOIN MobizLibraryOrganizationFranchisee fran ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                    " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                    " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo IS NULL " &
                    " AND grp.FranchisorFowstID = " & ReadyString(franchisorFowstID) &
                    " AND convert(datetime,inv._InvoiceDateTime) > convert(datetime," & ReadyString(dateCoverageStart) & ") "

                pDTUngroupedInvoices = Common.WebRequest.ProData.Execute(pSql)


                If (pDTUngroupedInvoices.Rows.Count > 0) Then
                    'loop all invoice with no group no and check if it belongs to the max group no

                    For Each dataRowsInvoice As DataRow In pDTUngroupedInvoices.Rows
                        Dim groupNo As Integer = GetMaxSalesOrderGroupNo(franchisorFowstID)


                        'get the cut off  time and day of the latest with sales order group no
                        pSql = "SELECT cutOffDetails.ID, cutOffGrpNo.ActualCutOffDate,cutOffGrpNo.ActualCutOffStartDate, cutOffDetails.Weekday, cutOffDetails.CutOffTime FROM LibraryFranchiseGroupCutOff cutOff INNER JOIN " &
                                                   " LibraryFranchiseGroupCutOffDetails cutOffDetails ON cutOff.ID = cutOffDetails.FranchiseGroupCutOffID INNER JOIN " &
                                                   " LibraryFranchiseGroupCutOffSOGroupNo cutOffGrpNo ON cutOffDetails.ID = cutOffGrpNo.FranchiseGroupCutOffDetailsID " &
                                                   " WHERE cutOffGrpNo.TransInvoiceSalesOrderGroupNo = " & groupNo &
                                                   " AND cutOff.FranchiseGroupID = " & ReadyString(grpID)

                        pDTLatestCutOff = Common.WebRequest.ProData.Execute(pSql)

                        If (pDTLatestCutOff.Rows.Count > 0) Then
                            newInvoiceDate = dataRowsInvoice("_InvoiceDateTime").ToString()
                            'check if it belongs to the same group no
                            If (Convert.ToDateTime(pDTLatestCutOff.Rows(0).Item("ActualCutOffStartDate").ToString()) < Convert.ToDateTime(newInvoiceDate) And Convert.ToDateTime(newInvoiceDate) <= Convert.ToDateTime(pDTLatestCutOff.Rows(0).Item("ActualCutOffDate").ToString())) Then
                                isNew = False
                                p = New InsertUpdate("TransInvoice")
                                p("SalesOrderGroupNo") = groupNo ' plus one
                                pSql = p.GetUpdate(" ID = " & ReadyString(dataRowsInvoice("ID").ToString()))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            Else


                                'if it doesnt belong to the latest group, increment group no by 1. 
                                isNew = True
                                p = New InsertUpdate("TransInvoice")
                                p("SalesOrderGroupNo") = groupNo + 1 ' plus one
                                pSql = p.GetUpdate(" ID = " & ReadyString(dataRowsInvoice("ID").ToString()))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)



                            End If


                        End If


                        If (isNew) Then
                            ''find the matching coverage where the invoicedate belongs

                            'get the cut off day and time. greater than the latest group cut off
                            pSql = "SELECT cutOffDetails.ID, cutOffDetails.Weekday, cutOffDetails.CutOffTime FROM LibraryFranchiseGroupCutOff cutOff INNER JOIN " &
                                   " LibraryFranchiseGroupCutOffDetails cutOffDetails ON cutOff.ID = cutOffDetails.FranchiseGroupCutOffID " &
                                   " WHERE cutOff.FranchiseGroupID = " & ReadyString(grpID) & " AND IsActive = 1 ORDER BY cutOffDetails.Weekday , cutOffDetails.CutOffTime"

                            pDTFindMatchingCutOff = Common.WebRequest.ProData.Execute(pSql)

                            Dim j As Integer = 0
                            Dim firstcutoffID As String
                            Dim firstcutoffTime As String
                            Dim firstcutoffDay As Integer
                            Dim triggernewdate As Boolean = False
                            Dim newStartDate As String
                            Dim hasNoMatch As Boolean = False

                            dateCoverageStart = pDTLatestCutOff.Rows(0).Item("ActualCutOffDate").ToString()

                            If (pDTFindMatchingCutOff.Rows.Count > 0) Then
                                'loop too see if it is below the next cut off group. if not, then try another cut off date
                                For Each dataRows As DataRow In pDTFindMatchingCutOff.Rows

                                    If j = 0 Then
                                        firstcutoffID = dataRows("ID").ToString()
                                        firstcutoffTime = dataRows("CutOffTime").ToString()
                                        firstcutoffDay = CDec2(dataRows("Weekday").ToString())

                                    End If
                                    j = j + 1

                                    If (Convert.ToDateTime(dateCoverageStart).DayOfWeek <= CDec2(dataRows("Weekday").ToString())) Then

                                        For i As Integer = 0 To 6

                                            'actual cut off period date
                                            If (Convert.ToDateTime(dateCoverageStart).DayOfWeek + i = CDec2(dataRows("Weekday").ToString())) Then
                                                theLimitDateMatching = Convert.ToDateTime(dateCoverageStart).AddDays(i)
                                                Exit For
                                            End If
                                        Next

                                        If (CDec2(prevWeekday) <> 0) Then
                                            For i As Integer = 0 To 6
                                                'actual cut off period date
                                                If (Convert.ToDateTime(dateCoverageStart).DayOfWeek + i = prevWeekday) Then
                                                    theBeginningDateMatching = Convert.ToDateTime(dateCoverageStart).AddDays(i)
                                                    Exit For
                                                End If

                                            Next
                                        Else
                                            hasNoMatch = True

                                        End If

                                        prevWeekday = CDec2(dataRows("Weekday").ToString())

                                    Else
                                        prevWeekday = CDec2(dataRows("Weekday").ToString())
                                        prevTime = dataRows("CutOffTime").ToString()

                                        Continue For
                                    End If


                                    dateToMatching = theLimitDateMatching.ToShortDateString()

                                    dateToStartMatching = theBeginningDateMatching.ToShortDateString()

                                    If (hasNoMatch Or Convert.ToDateTime(dateCoverageStart).DayOfWeek = CDec2(dataRows("Weekday").ToString())) Then

                                        If (Convert.ToDateTime(dateCoverageStart) < Convert.ToDateTime(newInvoiceDate) And Convert.ToDateTime(newInvoiceDate) <= Convert.ToDateTime(dateToMatching + " " + dataRows("CutOffTime").ToString())) Then

                                            p = New InsertUpdate("LibraryFranchiseGroupCutOffSOGroupNo")
                                            p("FranchiseGroupCutOffDetailsID") = ReadyString(dataRows("ID").ToString())

                                            p("TransInvoiceSalesOrderGroupNo") = groupNo + 1 ' save the group no here
                                            p("ActualCutOffDate") = ReadyString(Convert.ToDateTime(dateToMatching + " " + dataRows("CutOffTime").ToString())) ' save actual cut off date


                                            p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateCoverageStart).AddSeconds(1)) ' save actual cut off date


                                            pSql = p.GetInsert()
                                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                                            'if you got a match, get out of the loop
                                            Exit For

                                        Else



                                            If j = pDTFindMatchingCutOff.Rows.Count Then

                                                Dim datefinal As Date = Convert.ToDateTime(dateCoverageStart)

                                                While datefinal.DayOfWeek <> firstcutoffDay

                                                    datefinal = datefinal.AddDays(1)
                                                End While

                                                dateToMatching = datefinal.ToShortDateString()

                                                p = New InsertUpdate("LibraryFranchiseGroupCutOffSOGroupNo")
                                                p("FranchiseGroupCutOffDetailsID") = ReadyString(firstcutoffID)

                                                p("TransInvoiceSalesOrderGroupNo") = groupNo + 1 ' save the group no here
                                                p("ActualCutOffDate") = ReadyString(Convert.ToDateTime(dateToMatching + " " + firstcutoffTime)) ' save actual cut off date

                                                p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateCoverageStart).AddSeconds(1)) ' save actual cut off start date


                                                pSql = p.GetInsert()
                                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                                'if you got a match, get out of the loop
                                                Exit For
                                            End If


                                        End If

                                        hasNoMatch = False
                                    Else

                                        If (Convert.ToDateTime(dateToStartMatching + " " + prevTime) < Convert.ToDateTime(newInvoiceDate) And Convert.ToDateTime(newInvoiceDate) <= Convert.ToDateTime(dateToMatching + " " + dataRows("CutOffTime").ToString())) Then

                                            p = New InsertUpdate("LibraryFranchiseGroupCutOffSOGroupNo")
                                            p("FranchiseGroupCutOffDetailsID") = ReadyString(dataRows("ID").ToString())

                                            p("TransInvoiceSalesOrderGroupNo") = groupNo + 1 ' save the group no here
                                            p("ActualCutOffDate") = ReadyString(Convert.ToDateTime(dateToMatching + " " + dataRows("CutOffTime").ToString())) ' save actual cut off date


                                            p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateToStartMatching + " " + prevTime).AddSeconds(1)) ' save actual cut off date


                                            pSql = p.GetInsert()
                                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                                            'if you got a match, get out of the loop
                                            Exit For

                                        Else



                                            If j = pDTFindMatchingCutOff.Rows.Count Then
                                                'has limitation! TODO
                                                Dim datefinal As Date = Convert.ToDateTime(dateCoverageStart)

                                                While datefinal.DayOfWeek <> firstcutoffDay

                                                    datefinal = datefinal.AddDays(1)
                                                End While

                                                dateToMatching = datefinal.ToShortDateString()

                                                p = New InsertUpdate("LibraryFranchiseGroupCutOffSOGroupNo")
                                                p("FranchiseGroupCutOffDetailsID") = ReadyString(firstcutoffID)

                                                p("TransInvoiceSalesOrderGroupNo") = groupNo + 1 ' save the group no here
                                                p("ActualCutOffDate") = ReadyString(Convert.ToDateTime(dateToMatching + " " + firstcutoffTime)) ' save actual cut off date

                                                p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateToStartMatching + " " + prevTime).AddSeconds(1)) ' save actual cut off start date


                                                pSql = p.GetInsert()
                                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                                'if you got a match, get out of the loop
                                                Exit For
                                            End If


                                        End If


                                    End If


                                    prevTime = dataRows("CutOffTime").ToString()


                                Next


                            End If




                        End If


                    Next
                End If





            Else

                If HasUngroupedInvoice Then
                    If theLimitDate <= today Then
                        theLimitDate = theLimitDate.AddDays(3)
                    End If
                    lastDay = theLimitDate
                Else
                    lastDay = today
                End If
                'get the cut offs
                pSql = "SELECT cutOffDetails.ID, cutOffDetails.Weekday, cutOffDetails.CutOffTime FROM LibraryFranchiseGroupCutOff cutOff INNER JOIN " &
                       " LibraryFranchiseGroupCutOffDetails cutOffDetails ON cutOff.ID = cutOffDetails.FranchiseGroupCutOffID " &
                       " WHERE cutOff.FranchiseGroupID = " & ReadyString(grpID) & " AND IsActive = 1 ORDER BY cutOffDetails.Weekday, cutOffDetails.CutOffTime"

                pDTFindMatchingCutOff = Common.WebRequest.ProData.Execute(pSql)


                If (pDTFindMatchingCutOff.Rows.Count > 0) Then

                    For Each dataRows As DataRow In pDTFindMatchingCutOff.Rows
                        dayDiff = lastDay.DayOfWeek - CDec2(dataRows("Weekday").ToString())
                        theLimitDate = lastDay.AddDays(-dayDiff)

                        dateTo = theLimitDate.ToShortDateString()

                        If (CDec2(prevWeekday) <> 0) Then
                            dayDiffMatching = lastDay.DayOfWeek - prevWeekday
                            theBeginningDateMatching = lastDay.AddDays(-dayDiffMatching)

                            dateToStartMatching = theBeginningDateMatching.ToShortDateString()

                        End If

                        'if  null get from cut off effectivity date
                        pSql = "SELECT ID, EffectivityDate FROM LibraryFranchiseGroup WHERE ID = " & ReadyString(grpID)

                        pDTEffectivityDate = Common.WebRequest.ProData.Execute(pSql)
                        dateCoverageStart = pDTEffectivityDate.Rows(0).Item("EffectivityDate").ToString()


                        'get the invoice id
                        pSql = "SELECT inv.ID FROM TransInvoice inv INNER JOIN MobizLibraryOrganizationFranchisee fran " &
                                " ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                                " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                                " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo IS NULL " &
                                " AND grp.FranchisorFowstID = " & ReadyString(franchisorFowstID) &
                        " AND convert(datetime,inv._InvoiceDateTime) > convert(datetime," & ReadyString(dateCoverageStart) &
                        ") AND convert(datetime,inv._InvoiceDateTime) <= convert(datetime," & ReadyString(dateTo + " " + dataRows("CutOffTime").ToString()) & ")"

                        pDTUngroupedInvoices = Common.WebRequest.ProData.Execute(pSql)

                        ' set the group no
                        If (pDTUngroupedInvoices.Rows.Count > 0) Then
                            Dim groupNo As Integer = GetMaxSalesOrderGroupNo(franchisorFowstID)

                            For Each dataRowsInvoiceID As DataRow In pDTUngroupedInvoices.Rows
                                p = New InsertUpdate("TransInvoice")
                                p("SalesOrderGroupNo") = groupNo + 1 ' plus one
                                pSql = p.GetUpdate(" ID = " & ReadyString(dataRowsInvoiceID("ID").ToString()))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)


                            Next

                            p = New InsertUpdate("LibraryFranchiseGroupCutOffSOGroupNo")
                            p("FranchiseGroupCutOffDetailsID") = ReadyString(dataRows("ID").ToString())

                            p("TransInvoiceSalesOrderGroupNo") = groupNo + 1 ' save the group no here
                            p("ActualCutOffDate") = ReadyString(Convert.ToDateTime(dateTo + " " + dataRows("CutOffTime").ToString())) ' save actual cut off date

                            If (CDec2(prevWeekday) = 0) Then
                                p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateCoverageStart).AddSeconds(1)) ' save actual cut off start date
                            Else
                                p("ActualCutOffStartDate") = ReadyString(Convert.ToDateTime(dateToStartMatching + " " + prevTime).AddSeconds(1)) ' save actual cut off date
                            End If


                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            HasExistingCutOff = True
                        End If


                        'save the previous dates
                        prevWeekday = CDec2(dataRows("Weekday").ToString())
                        prevTime = dataRows("CutOffTime").ToString()

                    Next



                    'if no match in this week's cutoff scheds, try again next week date
                    pSql = "SELECT inv.ID FROM TransInvoice inv INNER JOIN MobizLibraryOrganizationFranchisee fran " &
                            " ON (inv.FowstID = fran.FranchiseeOrgFowstID) " &
                            " INNER JOIN LibraryFranchiseGroup grp ON (fran.FranchiseGroupID = grp.ID) " &
                            " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) AND inv.SalesOrderGroupNo IS NULL " &
                            " AND grp.FranchisorFowstID = " & ReadyString(franchisorFowstID) &
                    " AND convert(datetime,inv._InvoiceDateTime) > convert(datetime," & ReadyString(dateCoverageStart) &
                    ") AND convert(datetime,inv._InvoiceDateTime) > convert(datetime," & ReadyString(dateTo + " " + prevTime) & ")"

                    pDTUngroupedInvoices = Common.WebRequest.ProData.Execute(pSql)
                    If (pDTUngroupedInvoices.Rows.Count > 0) Then
                        HasUngroupedInvoice = True
                        UpdateSalesOrderGroupNo(grpID)
                    Else
                        HasUngroupedInvoice = False
                    End If


                End If



            End If


            Common.WebRequest.ProData.CommitTransaction()

            'get the latest group id
            pSql = "SELECT LibraryFranchiseGroupCutOffSOGroupNo.ID FROM LibraryFranchiseGroupCutOffDetails INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo ON LibraryFranchiseGroupCutOffDetails.ID = LibraryFranchiseGroupCutOffSOGroupNo.FranchiseGroupCutOffDetailsID " &
                   " WHERE LibraryFranchiseGroupCutOffDetails.FranchiseGroupID = " & ReadyString(grpID) & " ORDER BY LibraryFranchiseGroupCutOffSOGroupNo.TransInvoiceSalesOrderGroupNo DESC"

            pDTLatestCutOff = Common.WebRequest.ProData.Execute(pSql)

            Dim lastGrpCutOffID As String = ""

            If (pDTLatestCutOff.Rows.Count > 0) Then

                lastGrpCutOffID = pDTLatestCutOff.Rows(0).Item("ID").ToString()

            End If

            Return lastGrpCutOffID

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try
    End Function

    Function GetMaxSortingListNo(ByVal franchisorFowstID) As Integer
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String
        pSql = "SELECT MAX(sortList.TransInvoiceSortingListNo) as MaxCount " &
            " from LibraryFranchiseGroup grp" &
            " INNER JOIN LibraryFranchiseGroupCutOff grpCutOff ON (grp.ID = grpCutOff.FranchiseGroupID)" &
            " INNER JOIN LibraryFranchiseGroupCutOffDetails grpCutOffDet ON " &
            " (grp.ID = grpCutOffDet.FranchiseGroupID)" &
            " INNER JOIN LibraryFranchiseGroupCutOffSOGroupNo grpCutOffSOGrpNo ON " &
            " (grpCutOffDet.ID = grpCutOffSOGrpNo.FranchiseGroupCutOffDetailsID) " &
            " LEFT OUTER JOIN LibraryFranchiseGroupSortingListNo sortList ON" &
            " (grpCutOffSOGrpNo.ID = sortList.FranchiseGroupBatchNoID) " &
            " WHERE grp.FranchisorFowstID =" & ReadyString(franchisorFowstID)

        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Return CDec2(pDT.Rows(0).Item("MaxCount").ToString())
    End Function

    Function UpdateSortingListNo(ByVal fowstID As String, ByVal franchiseeID As String, ByVal groupID As String, ByVal salesOrderGroupNo As String, ByVal batchNoID As String) As Boolean
        Dim pIU As New InsertUpdate("TransInvoice")
        Dim pSql As String
        Dim pDR As DataRow
        Dim maxSLNo As Integer
        Dim pDT As DataTable
        Dim isNew As Boolean = False
        Try
            Common.WebRequest.ProData.BeginTransaction()

            'check if already has sorting no


            pSql = "SELECT ID,ISNULL(SortingListNo, 0) as SortingList FROM TransInvoice WHERE " &
                " SalesOrderGroupNo = " & salesOrderGroupNo & " AND FowstID = " & ReadyString(franchiseeID)

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then

                maxSLNo = GetMaxSortingListNo(fowstID)

                For Each pDR In pDT.Rows
                    If (CDec2(pDR("SortingList").ToString()) > 0) Then
                        Exit For
                    Else

                        isNew = True
                        pIU = New InsertUpdate("TransInvoice")
                        pIU("SortingListNo") = maxSLNo + 1 ' plus one
                        pSql = pIU.GetUpdate(" ID = " & ReadyString(pDR("ID").ToString()))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If


                Next pDR

                If (isNew) Then
                    pIU = New InsertUpdate("LibraryFranchiseGroupSortingListNo")
                    pIU("FranchiseGroupBatchNoID") = ReadyString(batchNoID)
                    pIU("TransInvoiceSortingListNo") = maxSLNo + 1 ' plus one
                    pIU("FranchiseeOrgFowstID") = ReadyString(franchiseeID)
                    pSql = pIU.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                End If

            End If

            Common.WebRequest.ProData.CommitTransaction()
            Return True
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return False
        End Try

    End Function


    Function ActivateFranchisee(ByVal id As String, ByVal statId As String, ByVal isFranchisee As Boolean, ByVal groupID As String) As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFranchisee")
        Dim pSql As String
        Dim pDT As DataTable
        Dim pSql2 As String
        Dim pDT2 As DataTable
        Dim pDT3 As DataTable

        Try

            Common.WebRequest.ProData.BeginTransaction()
            If isFranchisee Then
                p("IsFranchisorConfirmed") = 1
                p("FranchiseGroupID") = ReadyString(groupID)
            Else
                p("IsFranchiseeConfirmed") = 1
            End If

            p("LastUpdated") = "GetDate()"
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pSql = p.GetUpdate("ID =" & ReadyString(id))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")
            p("DateActivated") = "GetDate()"
            p("LastUpdated") = "GetDate()"
            p("ActivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
            pSql = p.GetUpdate("ID =" & ReadyString(statId))

            Common.WebRequest.ProData.ExecuteSQL(pSql)


            pSql = "SELECT fowst.ID, fran.FranchisorOrgID FROM MobizLibraryOrganizationFranchisee fran INNER JOIN " &
              " MobizLibraryOrganizationFOWST fowst ON fran.franchiseeOrgFowstID = fowst.id WHERE " &
              " fran.ID=" & ReadyString(id)


            pDT3 = Common.WebRequest.ProData.Execute(pSql)

            If (pDT3.Rows.Count > 0) Then
                p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                p("IsFranchisee") = 1
                p("FranchisorID") = ReadyString(pDT3.Rows(0).Item("FranchisorOrgID"))
                p("LastUpdated") = "GetDate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pSql = p.GetUpdate("ID =" & ReadyString(pDT3.Rows(0).Item("ID")))

                Common.WebRequest.ProData.ExecuteSQL(pSql)


                ' must be exclusive. once activated, rejected in other orgs (aside from their own org)

                pSql = "SELECT fran.FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee fran INNER JOIN " &
                    " MobizLibraryOrganizationFOWST fowst ON fran.franchiseeOrgFowstID = fowst.id WHERE " &
                    " fran.FranchisorOrgID <> fowst.OrganizationID AND fran.ID=" & ReadyString(id)


                pDT = Common.WebRequest.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then

                    pSql2 = "SELECT ID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseeOrgFowstID = " & ReadyString(pDT.Rows(0).Item("FranchiseeOrgFowstID")) &
                        " AND ID <> " & ReadyString(id)
                    pDT2 = Common.WebRequest.ProData.Execute(pSql2)

                    If (pDT2.Rows.Count > 0) Then
                        pSql = "UPDATE MobizLibraryOrganizationFranchisee SET IsActive = 0, " &
                      " LastUpdated = GETDATE() WHERE FranchiseeOrgFowstID = " & ReadyString(pDT.Rows(0).Item("FranchiseeOrgFowstID")) &
                      " AND ID <> " & ReadyString(id)

                        Common.WebRequest.ProData.Execute(pSql)


                        For Each pDR As DataRow In pDT2.Rows
                            p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")
                            p("DateDeactivated") = "GetDate()"
                            p("LastUpdated") = "GetDate()"
                            p("DeactivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetUpdate("ID =" & ReadyString(pDR("ID")))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Next


                    End If

                End If


            End If



            Common.WebRequest.ProData.CommitTransaction()

            Return "ok"

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try
    End Function


    Function RejectFranchisee(ByVal id As String, ByVal statId As String, ByVal isFranchisee As Boolean) As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFranchisee")
        Dim pSql As String

        Try

            Common.WebRequest.ProData.BeginTransaction()
            If isFranchisee Then
                p("IsFranchisorConfirmed") = 0
            Else
                p("IsFranchiseeConfirmed") = 0
            End If

            p("IsActive") = 0
            p("LastUpdated") = "GetDate()"
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pSql = p.GetUpdate("ID =" & ReadyString(id))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")
            p("DateDeactivated") = "GetDate()"
            p("LastUpdated") = "GetDate()"
            p("DeactivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
            pSql = p.GetUpdate("ID =" & ReadyString(statId))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "ok"
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try
    End Function


    Function getFranchiseStockOrders(ByVal salesOrderGroupNo As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = " prd.ProductName"
                End With
                With .Add("OrderedQuantity", SqlDbType.Int)
                    .Expression = "invitem.OrderedQuantity"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "invitem.Quantity"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "invitem.UnitMeasure"
                End With
            End With

            .SqlFrom = " FROM TransInvoice inv INNER JOIN TransInvoiceItems invitem ON (in.ID = invitem.InvoiceID) " &
                    " INNER JOIN MobizLibraryPIMProduct prd ON (invitem.ProductID = prd.ID) "

            .SqlWhere = " WHERE inv.SalesORderGroupNo = " + ReadyString(salesOrderGroupNo)

            .SqlOrder = " ORDER BY prd.ProductNo ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function AddFranchisee(ByVal id As String, ByVal isOwn As Boolean) As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFranchisee")
        Dim pSql As String
        Dim strIDRec As String
        Dim pDT As DataTable
        Dim AutoApproveFranchiseeRequests As Boolean = CBool2(Common.WebRequest.Data("AutoApproveFranchisorRequests"))
        Try


            'if no existing, insert
            pSql = "SELECT ID, IsActive FROM MobizLibraryOrganizationFranchisee Where FranchisorOrgID=" & ReadyString(OrgID) &
            " AND FranchiseeOrgFowstID = " & ReadyString(id) & " AND IsFranchiseeConfirmed = 0"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count < 1) Then

                p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                p("FranchisorOrgID") = ReadyString(OrgID)
                p("FranchiseeOrgFowstID") = ReadyString(id)

                p("IsFranchisorConfirmed") = "1"
                p("IsActive") = "1"
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                p("LastUpdated") = "getdate()"
                p("OwnershipType") = ReadyString(Common.WebRequest.Data("OwnershipType"))

                If isOwn Or AutoApproveFranchiseeRequests Then
                    p("IsFranchiseeConfirmed") = "1"
                End If

                If Not Common.WebRequest.Data("MaxInventoryValue") Is Nothing Then
                    p("MaxInventoryValue") = CDec2(Common.WebRequest.Data("MaxInventoryValue"))
                    p("InventoryValuePriceLevel") = ReadyString(Common.WebRequest.Data("InventoryValuePriceLevel"))
                    p("DefaultSellingPriceLevel") = ReadyString(CStr(Common.WebRequest.Data("DefaultSellingPriceLevelID")))
                    p("POPriceLevel") = ReadyString(CStr(Common.WebRequest.Data("POPriceLevelID")))
                    p("IsPOExclusiveToPrincipal") = CInt2(Common.WebRequest.Data("IsPOExclusiveToPrincipal"))
                End If

                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                ''''''''''''''''''''
                p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                p("FranchiseID") = ReadyString(strIDRec)
                p("DateRequested") = "getdate()"
                p("RequestedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                p("LastUpdated") = "getdate()"

                If isOwn Or AutoApproveFranchiseeRequests Then
                    p("DateActivated") = "getdate()"
                    p("activatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                End If


                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Else

                If (Not CBool2(pDT.Rows(0).Item("IsActive").ToString())) Then
                    p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                    p("FranchisorOrgID") = ReadyString(OrgID)
                    p("FranchiseeOrgFowstID") = ReadyString(id)
                    p("IsFranchisorConfirmed") = "1"
                    p("IsActive") = "1"
                    p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                    p("LastUpdated") = "getdate()"

                    If isOwn Or AutoApproveFranchiseeRequests Then
                        p("IsFranchiseeConfirmed") = "1"
                    End If

                    If Not Common.WebRequest.Data("MaxInventoryValue") Is Nothing Then
                        p("MaxInventoryValue") = CDec2(Common.WebRequest.Data("MaxInventoryValue"))
                        p("InventoryValuePriceLevel") = ReadyString(Common.WebRequest.Data("InventoryValuePriceLevel"))
                        p("DefaultSellingPriceLevel") = ReadyString(CStr(Common.WebRequest.Data("DefaultSellingPriceLevelID")))
                        p("POPriceLevel") = ReadyString(CStr(Common.WebRequest.Data("POPriceLevelID")))
                        p("IsPOExclusiveToPrincipal") = CInt2(Common.WebRequest.Data("IsPOExclusiveToPrincipal"))
                    End If

                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    ''''''''''''''''''''
                    p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                    p("FranchiseID") = ReadyString(strIDRec)
                    p("DateRequested") = "getdate()"
                    p("RequestedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                    p("LastUpdated") = "getdate()"
                    If isOwn Or AutoApproveFranchiseeRequests Then
                        p("DateActivated") = "getdate()"
                        p("activatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                    End If


                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                End If
            End If

            Return "ok"

        Catch ex As Exception

            Return "Error"
        End Try
    End Function


    Function GetFranchiseRequests() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.ID"
                    .Searchable = False
                End With
                With .Add("FranchiseStatusID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ID"
                    .Searchable = False
                End With
                With .Add("FranchisorOrgID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchisorOrgID"
                    .Searchable = False
                End With
                With .Add("FranchiseeOrgFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseeOrgFowstID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "orgFowst.Name"
                End With
                With .Add("OrganizationName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(org.OfficeAddressBuildingNo, '') " &
                                    "+ ', ' + ISNULL(org.OfficeAddressStreet, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("FranchiseGroupID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseGroupID"
                    .Searchable = False
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchisorConfirmed"
                    .Searchable = False
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchiseeConfirmed"
                    .Searchable = False
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "fran.IsActive"
                    .Searchable = False
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.UserID"
                    .Searchable = False
                End With
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "fran.LastUpdated"
                End With
                With .Add("DateRequested", SqlDbType.DateTime)
                    .Expression = "stat.DateRequested"
                End With
                With .Add("DateActivated", SqlDbType.DateTime)
                    .Expression = "stat.DateActivated"
                End With
                With .Add("DateDeactivated", SqlDbType.DateTime)
                    .Expression = "stat.DateDeactivated"
                End With
                With .Add("RequestedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.RequestedByID"
                End With
                With .Add("ActivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ActivatedByID"
                End With
                With .Add("DeactivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.DeactivatedByID"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                    " INNER JOIN MobizLibraryOrganizationFranchiseStatus stat ON (fran.ID = stat.FranchiseID) " &
                    " INNER JOIN MobizLibraryOrganizationFowst orgFowst ON (fran.FranchiseeOrgFowstID = orgFowst.ID) " &
                    " INNER JOIN MobizLibraryOrganization org ON (fran.FranchisorOrgID = org.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople ppl ON (fran.UserID = ppl.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
                    " LEFT OUTER JOIN MobizLibraryCities cty ON (org.OfficeAddressCityID = cty.ID) " &
                    " LEFT OUTER JOIN MobizLibraryCountries ctry ON (org.OfficeAddressCountryID = ctry.ID) "

            .SqlWhere = " WHERE fran.IsActive = 1 AND orgFowst.OrganizationID = " + ReadyString(OrgID)

            .SqlOrder = " ORDER BY fran.LastUpdated DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function


    Function GetFranchisorStatus(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.ID"
                    .Searchable = False
                End With
                With .Add("FranchiseeOrgFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseeOrgFowstID"
                    .Searchable = False
                End With
                With .Add("FranchiseeFowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("OrganizationName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("FranchiseGroupID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.FranchiseGroupID"
                    .Searchable = False
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchisorConfirmed"
                    .Searchable = False
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "fran.IsFranchiseeConfirmed"
                    .Searchable = False
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "fran.IsActive"
                    .Searchable = False
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "fran.UserID"
                    .Searchable = False
                End With
                With .Add("UserName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(ppl.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppl.PersonalSuffixName)),', '), '') "
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "fran.LastUpdated"
                End With
                With .Add("DateRequested", SqlDbType.DateTime)
                    .Expression = "stat.DateRequested"
                End With
                With .Add("DateActivated", SqlDbType.DateTime)
                    .Expression = "stat.DateActivated"
                End With
                With .Add("DateDeactivated", SqlDbType.DateTime)
                    .Expression = "stat.DateDeactivated"
                End With
                With .Add("RequestedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.RequestedByID"
                End With
                With .Add("ActivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.ActivatedByID"
                End With
                With .Add("DeactivatedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "stat.DeactivatedByID"
                End With
                With .Add("RequestedByName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominalRequest.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplRequest.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplRequest.PersonalSuffixName)),', '), '') "
                End With
                With .Add("ActivatedByName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(prenominalApprove.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.Surname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.FirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(pplApprove.MiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(pplApprove.PersonalSuffixName)),', '), '') "
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee fran " &
                    " INNER JOIN MobizLibraryOrganizationFranchiseStatus stat ON (fran.ID = stat.FranchiseID) " &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fran.FranchiseeOrgFowstID = fowst.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople ppl ON (fran.UserID = ppl.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominal ON (ppl.TitleID = prenominal.ID) " &
                    " LEFT OUTER JOIN MobizLibraryOrganization org ON (fran.FranchisorOrgID = org.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople pplRequest ON (stat.RequestedByID = pplRequest.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalRequest ON (pplRequest.TitleID = prenominalRequest.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeople pplApprove ON (stat.ActivatedByID = pplApprove.ID) " &
                    " LEFT OUTER JOIN MobizLibraryPeoplePreNominalSuffix prenominalApprove ON (pplApprove.TitleID = prenominalApprove.ID) "

            .SqlWhere = " WHERE fran.ID = " + ReadyString(id)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetIncomeStatement() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim Slice As String
        Dim pColumn As String
        Dim pTableName As String
        Dim IsSalesRep As Boolean
        Dim SpliceFilter() As String

        Slice = WebRequest.Instance.Data("SliceBy")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                If CInt(WebRequest.Instance.Data("IsReport") = 1) Then
                    With .Add("ProductName", SqlDbType.Decimal)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                    End With
                    With .Add("Quantity", SqlDbType.Decimal)
                        .Expression = "dbo.TransInvoiceItems.Quantity"
                    End With
                    With .Add("TotalAmount", SqlDbType.Decimal)
                        .Expression = "(CASE WHEN ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0) = 0 THEN dbo.TransInvoiceItems.SubTotal ELSE dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions END)"
                    End With
                    With .Add("InvoiceNumber", SqlDbType.Decimal)
                        .Expression = "dbo.TransInvoice.InvoiceNumber"
                    End With
                Else
                    With .Add("Total", SqlDbType.Decimal)
                        .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                    End With
                    'With .Add("TransactionCount", SqlDbType.Int)
                    '    .Expression = "COUNT(dbo.TransInvoice.ID)"
                    'End With
                    'With .Add("ProductCount", SqlDbType.Decimal)
                    '    .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
                    'End With
                    'With .Add("ProductCountSO", SqlDbType.Decimal)
                    '    .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
                    'End With
                End If

                If Not Slice = vbNullString Then

                    SpliceFilter = Split(Slice, ",")

                    For i As Integer = 0 To SpliceFilter.Length - 1
                        If i > 0 Then
                            SqlGroupBy &= ","
                        End If
                        Select Case SpliceFilter(i)
                            Case "FOWST"
                                With .Add("FOWST", SqlDbType.VarChar)
                                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                                End With

                                SqlFrom &= " INNER JOIN dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf
                                SqlGroupBy &= " dbo.MobizLibraryOrganizationFOWST.Name"
                            Case "Brand"
                                With .Add("Brand", SqlDbType.VarChar)
                                    .Expression = "dbo.MobizLibraryPIMProduct.Brand"
                                End With
                                SqlGroupBy &= " dbo.MobizLibraryPIMProduct.Brand"
                            Case "DivisionDescription"
                                With .Add("DivisionDescription", SqlDbType.VarChar)
                                    .Expression = "dbo.MobizLibraryPIMProduct.DivisionDescription"
                                End With
                                SqlGroupBy &= " dbo.MobizLibraryPIMProduct.DivisionDescription"
                            Case "SalesRepresentative"
                                With .Add("SalesRepresentative", SqlDbType.VarChar)
                                    .Expression = "ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName END,'N/A')"
                                End With
                                SqlGroupBy &= " MobizLibraryPeople.SurName, MobizLibraryPeople.FirstName "
                                IsSalesRep = True
                                'additional by janine oct 30 2014
                            Case "Product"
                                With .Add("Product", SqlDbType.VarChar)
                                    .Expression = "ISNULL(dbo.MobizLibraryPIMProduct.ProductName, 'N/A')"
                                End With
                                SqlGroupBy &= " dbo.MobizLibraryPIMProduct.ProductName"
                            Case "DeliveryMethod"
                                With .Add("DeliveryMethod", SqlDbType.VarChar)
                                    .Expression = "ISNULL(dbo.TransInvoice.DeliveryMethod, 'Not Provided')"
                                End With
                                SqlGroupBy &= " dbo.TransInvoice.DeliveryMethod"
                            Case "OrderMethod"
                                With .Add("OrderMethod", SqlDbType.VarChar)
                                    .Expression = "ISNULL(dbo.TransInvoice.OrderMethod, 'Not Provided')"
                                End With
                                SqlGroupBy &= " dbo.TransInvoice.OrderMethod"
                            Case "Gender"
                                With .Add("Gender", SqlDbType.VarChar)
                                    .Expression = "ISNULL(dbo.TransInvoice.CustomerDemographicGender, 'Not Provided')"
                                End With
                                SqlGroupBy &= " dbo.TransInvoice.CustomerDemographicGender"
                            Case "Age"
                                With .Add("Age", SqlDbType.VarChar)
                                    .Expression = "ISNULL(dbo.TransInvoice.CustomerDemographicAge, 'Not Provided')"
                                End With
                                SqlGroupBy &= " dbo.TransInvoice.CustomerDemographicAge"
                                ' end of additional by janine oct 30 2014
                            Case "Time"
                                With .Add("Time", SqlDbType.VarChar)
                                    .Expression = "CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00'"
                                End With

                                SqlGroupBy &= ", DATEPART(hh, InvoiceDate)"
                            Case "Encoder"
                                With .Add("Encoder", SqlDbType.VarChar)
                                    .Expression = "EncodedBy.Surname + ', ' + EncodedBy.FirstName"
                                End With

                                SqlFrom &= " INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                                SqlGroupBy &= " EncodedBy.Surname + ', ' + EncodedBy.FirstName "
                            Case "OtherPayments"
                                With .Add("OtherPayments", SqlDbType.VarChar)
                                    .Expression = "TransCustomerPaymentOther.PaymentType"
                                End With

                                SqlFrom &= "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                        "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                                SqlGroupBy &= ", TransCustomerPaymentOther.PaymentType "
                            Case "PaymentType"

                                SqlFrom &= "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                                SqlGroupBy &= " EncodedBy.Surname + ', ' + EncodedBy.FirstName "
                            Case Else

                                'pColumn = GetFeatureColumnName(Slice, pTableName)

                                'pTableName = "Mobiz" & pTableName

                                'With .Add("ProductFeature", SqlDbType.VarChar)
                                '    .Expression = "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')" '"ISNULL(dbo." & pTableName & "." & pColumn & ",'N/A')"
                                'End With
                                'SqlFrom = "LEFT JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                                'SqlGroupBy &= " " & "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')"

                        End Select
                    Next
                End If
            End With

            SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere

            If Not WebRequest.Instance.Data("Filter1") = vbNullString Then
                Select Case WebRequest.Instance.Data("Filter1Option")
                    Case "Time"
                        SqlWhere &= " AND DATEPART(hh, InvoiceDate) = " & ReadyString(Replace(WebRequest.Instance.Data("Filter1"), ":00", ""))
                    Case "Brand"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                    Case "Division"
                        SqlWhere &= " AND dbo.MobizLibraryPIMProduct.DivisionDescription = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                    Case "SalesRepresentative"
                        If IsSalesRep = True Then
                            If WebRequest.Instance.Data("Filter1") = "N/A" Then
                                SqlWhere &= " AND (MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName IS NULL) "
                            Else
                                SqlWhere &= " AND (MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName = " & ReadyString(WebRequest.Instance.Data("Filter1")) & ")"
                            End If
                        End If
                        'additional by janine oct 30
                    Case "Product"
                        SqlWhere &= " AND ISNULL(dbo.MobizLibraryPIMProduct.ProductName, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'N/A' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "DeliveryMethod"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.DeliveryMethod, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "OrderMethod"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.OrderMethod, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Gender"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicGender, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                    Case "Age"
                        SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicAge, '') = (CASE WHEN " & ReadyString(WebRequest.Instance.Data("Filter1")) &
                            " = 'Not Provided' THEN '' ELSE " & ReadyString(WebRequest.Instance.Data("Filter1")) & " END) "
                        'end of additional
                    Case "Encoder"

                        SqlWhere &= " AND EncodedBy.Surname + ', ' + EncodedBy.FirstName  LIKE " & ReadyStringWild(WebRequest.Instance.Data("Filter1"))

                        SqlFrom &= "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID"

                    Case "OtherPayments"

                        SqlWhere &= " AND TransCustomerPaymentOther.PaymentType like " & ReadyStringWild(WebRequest.Instance.Data("Filter1"))

                        SqlFrom &= "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                    Case Else
                        pColumn = GetFeatureColumnName(WebRequest.Instance.Data("Filter1Option"), pTableName)

                        pTableName = "Mobiz" & pTableName

                        If SqlFrom Is Nothing Then
                            SqlFrom = "LEFT JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                        End If

                        If WebRequest.Instance.Data("Filter1") = "N/A" Then
                            SqlWhere &= " AND (" & pColumn & " = " & ReadyString(WebRequest.Instance.Data("Filter1")) & " OR " & pColumn & " = '')"
                        Else
                            SqlWhere &= " AND " & pColumn & " = " & ReadyString(WebRequest.Instance.Data("Filter1"))
                        End If
                End Select
            End If

            If IsSalesRep Then
                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                          "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                "LEFT join TransInvoiceItemSalesReps on transinvoiceitems.InvoiceID = TransInvoiceItemSalesReps.InvoiceID " & vbCrLf &
                "AND transinvoiceitems.ProductID = TransInvoiceItemSalesReps.ProductID " & vbCrLf &
                "LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID " & vbCrLf &
                SqlFrom & vbCrLf
            Else
                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                    "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                    "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID " & vbCrLf &
                SqlFrom & vbCrLf
            End If

            'Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

            'SqlWhere &= " AND TransInvoice.FowstID = " & ReadyString(Fowst(0))

            'While CtrFowst <= UBound(Fowst)
            '    If Not Fowst(CtrFowst) = vbNullString Then
            '        SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst)) & vbCrLf
            '    End If
            '    CtrFowst += 1
            'End While

            .SqlWhere = SqlWhere

            If CInt(WebRequest.Instance.Data("IsReport") = 1) Then

            Else
                If Not SqlGroupBy = vbNullString Then
                    .SqlGroup = "GROUP BY " & vbCrLf & SqlGroupBy
                End If

                .SqlOrder = "Order by Total"
            End If

            pDT = .Requery

            If CInt(WebRequest.Instance.Data("IsReport") = 1) Then

            Else
                Return pDT
            End If
        End With

    End Function
#End Region

#Region "OffTake"
    Function GetOffTakeData() As DataTable
        Dim pFinder As Finder
        Dim pDt As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        If IsRawData Then

            If Not Common.WebRequest.Data("BinID") = "All" Then
                SqlWhere = " AND "
                SqlWhere += " TransOfftake.BinID = " & ReadyString(Common.WebRequest.Data("BinID"))
            End If

            If CStr(Common.WebRequest.Data("ProductName")).Length > 0 Then
                Dim StrProductSplit() = Common.WebRequest.Data("ProductName").Split(" ")

                For i As Integer = 0 To UBound(StrProductSplit)
                    SqlWhere += " AND ProductName like " & ReadyStringWild(StrProductSplit(i))
                Next

            End If

        End If
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransOfftake.ProductID"
                End With

                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransOfftake.BinID"
                End With

                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransOfftake.FOWSTID"
                End With

                .Add("ProductName", SqlDbType.VarChar)

                With .Add("BinName", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryFOWSTBin.Name"
                End With

                With .Add("Quantity", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "OffTake"
                End With
            End With

            .SqlFrom = "FROM TransOfftake INNER JOIN MobizLibraryPimproduct on MobizLibraryPimproduct.ID = TransOfftake.ProductID " &
                        "inner join LibraryFOWSTBin on LibraryFOWSTBin.ID = TransOfftake.BinID"

            .SqlWhere = "Where convert(date,OffTakeDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND TransOfftake.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        SqlWhere

            '.SqlGroup = "Group by ProductName, LibraryFOWSTBin.Name, TransOfftake.BinID, TransOfftake.ProductID, TransOfftake.FowstID"
            .SqlOrder = "Order By ProductID, BinID"
            pDt = .Requery
        End With

        Return pDt
    End Function

    Function GetProductOfftakeDetails() As DataTable
        Dim pFinder As Finder
        Dim pDt As DataTable
        Dim pSql As String
        Dim pDTOfftakeRecords As DataTable
        Dim dtTemp As DataTable

        pSql = "Select * from TransOfftake " & vbCrLf &
                "Where convert(date, OffTakeDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                 " AND TransOfftake.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                 " AND TransOfftake.ProductID = " & ReadyString(ProductID) & vbCrLf &
                 " AND TransOfftake.BinID = " & ReadyString(BinID)

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        'if has two or more records then proceed, if not, get last takeoff
        If dtTemp.Rows.Count = 1 Then
            pSql = "Select top 1 OffTakeDate from TransOfftake " & vbCrLf &
                "Where convert(date, OffTakeDate) < " & ReadyString(dtTemp.Rows(0).Item("OffTakeDate")) & vbCrLf &
                 " AND TransOfftake.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                 " AND TransOfftake.ProductID = " & ReadyString(ProductID) & vbCrLf &
                 " AND TransOfftake.BinID = " & ReadyString(BinID) & vbCrLf &
                 " Order by OffTakeDate desc"

            dtTemp = Common.WebRequest.ProData.Execute(pSql)

            If dtTemp.Rows.Count > 0 Then
                DateFrom = dtTemp.Rows(0).Item("OffTakeDate")
            End If
        End If

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ProductName", SqlDbType.VarChar)

                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("BinName", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With

                .Add("TransactionType", SqlDbType.Decimal)

                .Add("Qty", SqlDbType.Decimal)

                .Add("Balance", SqlDbType.Decimal)

                With .Add("LastUpdated", SqlDbType.Decimal)
                    .Expression = "TransInventoryLedger.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM TransInventoryLedger " & vbCrLf &
                        "inner join MobizLibraryPimProduct on MobizLibraryPimProduct.ID = TransInventoryLedger.ProductID " & vbCrLf &
                        "inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID " & vbCrLf &
                        "inner join LibraryFowstBin on LibraryFowstBin.ID = TransInventoryLedger.BinID"

            .SqlWhere = "Where convert(date,TransInventoryLedger.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                            " AND TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            " AND TransInventoryLedger.ProductID = " & ReadyString(ProductID) & vbCrLf &
                            " AND TransInventoryLedger.BinID = " & ReadyString(BinID)

            .SqlOrder = "Order By TransInventoryLedger.LastUpdated"
            pDt = .Requery
        End With

        Return pDt
    End Function

    Function GetFowstBin() As DataTable
        Dim pFinder As Finder
        Dim pDt As DataTable
        Dim pSql As String
        Dim pDTOfftakeRecords As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransOfftake.BinID"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With

            End With
            .SqlFrom = "FROM TransOfftake INNER JOIN LibraryFowstBin on TransOfftake.BinID = LibraryFowstBin.ID "

            .SqlWhere = "Where convert(date, OfftakeDate) between " & ReadyString(Common.WebRequest.Data("From")) &
                        " AND " & ReadyString(Common.WebRequest.Data("To"))

            pDt = .Requery
        End With

        Return pDt
    End Function

    Function GetFowstFromLedger() As DataTable
        Dim pFinder As Finder
        Dim pDt As DataTable
        Dim pSql As String
        Dim pDTOfftakeRecords As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransInventoryLedger.FowstID"
                End With

                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFowst.Name"
                End With

            End With
            .SqlFrom = "FROM TransInventoryLedger inner join MobizLibraryOrganizationFowst on MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID"

            pDt = .Requery
        End With

        Return pDt
    End Function
#End Region

    Class Frown
        Public d As dClass

        Public Class dClass
            Public Results As ResultClass

            Public Class ResultClass
                Public Rows As List(Of RowsClass)

                Public Class RowsClass
                    Public ID As String
                    Public CustomerID As String
                    Public FOWSTID As String
                    Public SalesDate As String
                    Public GrossSales As Double
                    Public Discount As Double
                    Public SalesReturns As Double
                    Public NetSales As Double
                    Public COGSGross As Double
                    Public COGSNet As Double
                End Class
            End Class
        End Class
    End Class
End Class

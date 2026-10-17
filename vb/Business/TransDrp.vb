Public Class TransDrp

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function DrpReport(ByVal vDateFrom As Date, ByVal vDateTo As Date, ByVal vWhere As String, _
        ByVal vIncludeActive As Boolean, _
        ByVal vIncludeInactive As Boolean,
        ByVal vSalesOrderPercent As Single,
        ByVal vInvoicePercent As Single,
        ByVal vBackOrderPercent As Single,
        ByVal vPosPercent As Single) As DataTable

        Dim pDT As DataTable
        Dim pSelect As String
        Dim pSql As String
        Dim pSqlStocks As String
        Dim pWhereActive As String
        Dim p As Integer

        ' stocks on hand
        pSql = "SELECT TOP 1 StocksOnHand" &
            " FROM WholesaleStockInventory" &
            " WHERE WholesaleStockInventory.StockID = LibraryStockInformation.ID" &
            " ORDER BY WholesaleStockInventory.LastUpdated DESC"
        pSelect &= ",ISNULL((" & pSql & "), 0) AS StocksOnHand"

        ' sales order qty
        For p = 0 To 1
            pSql = "SELECT SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity)" & _
                " FROM TransSalesOrderItems" &
                " JOIN TransSalesOrder ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & _
                " JOIN LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID" & _
                " WHERE TransSalesOrderItems.StockID = LibraryStockInformation.ID" & _
                " AND TransSalesOrder.IsCancelled = 0" & _
                " AND LibraryCustomers.IsCompanyBranch = " & p & _
                " AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
            '" AND DATEDIFF(day, TransSalesOrder.RegisteredDate, GETDATE()) < TransSalesOrderItems.CancellationPeriod" &
            pSelect &= ",ISNULL((" & pSql & ") * " & vSalesOrderPercent & " / 100, 0) AS "
            If p = 0 Then
                pSelect &= "SalesQty"
            Else
                pSelect &= "SalesBranchQty"
            End If
        Next p

        '// the following code was converted to CROSS APPLY
        '' invoice qty
        'For p = 0 To 1
        '    pSql = "SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)" & _
        '        " FROM TransInvoiceItems" &
        '        " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & _
        '        " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & _
        '        " WHERE TransInvoiceItems.StockID = LibraryStockInformation.ID" & _
        '        " AND TransInvoice.IsCancelled = 0" & _
        '        " AND LibraryCustomers.IsCompanyBranch = " & p & _
        '        " AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
        '    pSelect &= ",ISNULL((" & pSql & ") * " & vInvoicePercent & " / 100, 0) AS "
        '    If p = 0 Then
        '        pSelect &= "InvoiceQty"
        '    Else
        '        pSelect &= "InvoiceBranchQty"
        '    End If
        'Next p

        ' backorder
        For p = 0 To 1
            pSql = "SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " & _
                " FROM TransSalesBackOrderItems" &
                " JOIN TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID" &
                " JOIN LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " & _
                " WHERE TransSalesBackOrderItems.IsCancelled = 0 " & _
                " AND TransSalesBackOrderItems.IsServe = 0" & _
                " AND LibraryCustomers.IsCompanyBranch = " & p & _
                " AND TransSalesBackOrderItems.StockID = LibraryStockInformation.ID" &
                " AND DATEDIFF(day, TransSalesBackOrderItems.RegisteredDate, GETDATE()) < TransSalesBackOrderItems.CancellationPeriod" &
                " AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
            pSelect &= ",ISNULL((" & pSql & ") * " & vBackOrderPercent & " / 100, 0) AS "
            If p = 0 Then
                pSelect &= "BackorderQty"
            Else
                pSelect &= "BackorderBranchQty"
            End If
        Next p

        '// the following code was converted to CROSS APPLY
        ' pos qty
        'pSql = "SELECT SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity) AS Qty" &
        '    " FROM POSSales" &
        '    " JOIN POSSalesItems ON POSSalesItems.POSSalesID = POSSales.ID" &
        '    " WHERE POSSalesItems.StockID = LibraryStockInformation.ID" &
        '    " AND CONVERT(datetime, FLOOR(CONVERT(float, POSSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
        ''pSql &= " AND BranchID = 10 AND MachineNumber = 3"
        'pSelect &= ",ISNULL((" & pSql & ") * " & vPosPercent & " / 100, 0) AS PosQty"

        '// the following code was converted to CROSS APPLY
        '' CashSlipQty
        'pSql = "SELECT SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity) AS Qty" &
        '    " FROM POSCashSlipSales" &
        '    " JOIN POSCashSlipSalesItems ON POSCashSlipSalesItems.CashSlipID = POSCashSlipSales.ID" &
        '    " WHERE POSCashSlipSalesItems.StockID = LibraryStockInformation.ID" &
        '    " AND CONVERT(datetime, FLOOR(CONVERT(float, POSCashSlipSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
        ''pSql &= " AND BranchID = 10 AND MachineNumber = 3"
        'pSelect &= ",ISNULL((" & pSql & ") * " & vPosPercent & " / 100, 0) AS CashSlipQty"

        ' using the code below is slower than using the code above.
        ' the following is not deleted as performance reference 
        'pSql = "SELECT SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity) AS Qty" &
        ' " FROM POSSales" &
        ' " JOIN POSSalesItems ON POSSalesItems.POSSalesID = POSSales.ID" &
        ' " WHERE POSSalesItems.StockID = LibraryStockInformation.ID" &
        ' " AND CONVERT(datetime, FLOOR(CONVERT(float, POSSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
        'pSql &= " UNION "
        'pSql &= "SELECT SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity) AS Qty" &
        '    " FROM POSCashSlipSales" &
        '    " JOIN POSCashSlipSalesItems ON POSCashSlipSalesItems.CashSlipID = POSCashSlipSales.ID" &
        '    " WHERE POSCashSlipSalesItems.StockID = LibraryStockInformation.ID" &
        '    " AND CONVERT(datetime, FLOOR(CONVERT(float, POSCashSlipSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
        'pSql = "SELECT SUM(Qty)" &
        '    " FROM (" & pSql & ")t"
        'pSelect &= ",ISNULL((" & pSql & "), 0) AS PosQty"

        '// create the main query
        If vIncludeActive AndAlso vIncludeInactive Then
        ElseIf vIncludeActive Then
            pWhereActive = " AND LibraryStockInformation.IsActive = 1"
        Else
            pWhereActive = " AND LibraryStockInformation.IsActive = 0"
        End If

        pSql = "SELECT LibraryStockInformation.ID AS StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",(SELECT TOP 1 LibraryUnitMeasures.UnitMeasure" & _
            "   FROM LibraryStockMeasurements" & _
            "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & _
            "   WHERE LibraryStockMeasurements.StockID = LibraryStockInformation.ID" & _
            "   ORDER BY LibraryStockMeasurements.Quantity, LibraryUnitMeasures.UnitMeasure) AS UnitMeasure" &
            ",(SELECT TOP 1 LibraryStockMeasurements.Quantity" & _
            "   FROM LibraryStockMeasurements" & _
            "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & _
            "   WHERE LibraryStockMeasurements.StockID = LibraryStockInformation.ID" & _
            "   ORDER BY LibraryStockMeasurements.Quantity, LibraryUnitMeasures.UnitMeasure) AS UnitQty"
        pSql &= ",ISNULL(tblInvoice.InvoiceQty * " & vInvoicePercent & " / 100, 0) AS InvoiceQty" &
            ",ISNULL(tblInvoice.InvoiceAmount, 0) AS InvoiceAmount" &
            ",ISNULL(tblInvoiceBranch.InvoiceQty * " & vInvoicePercent & " / 100, 0) AS InvoiceBranchQty" &
            ",ISNULL(tblInvoiceBranch.InvoiceAmount, 0) AS InvoiceBranchAmount"
        pSql &= ",ISNULL(tblPos.PosQty * " & vPosPercent & " / 100, 0) AS PosQty" &
            ",ISNULL(tblPos.PosAmount, 0) AS PosAmount" &
            ",ISNULL(tblCashSlip.Qty * " & vPosPercent & " / 100, 0) AS CashSlipQty" &
            ",ISNULL(tblCashSlip.Amount, 0) AS CashSlipAmount"
        pSql &= pSelect
        pSql &= " FROM LibraryStockInformation" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID"

        ' CROSS APPLY invoice qty
        For p = 0 To 1
            pSql &= " CROSS APPLY (" & vbCrLf &
                "SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS InvoiceQty" & _
                ",SUM(TransInvoiceItems.SubTotal) AS InvoiceAmount" & _
                " FROM TransInvoiceItems" &
                " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & _
                " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & _
                " WHERE TransInvoiceItems.StockID = LibraryStockInformation.ID" & _
                " AND TransInvoice.IsCancelled = 0" & _
                " AND LibraryCustomers.IsCompanyBranch = " & p & _
                " AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
                ") tblInvoice" & If(p = 0, "", "Branch")
        Next p

        ' CROSS APPLY pos qty
        pSql &= " CROSS APPLY (" & vbCrLf &
            "SELECT SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity) AS PosQty" &
            ",SUM(POSSalesItems.SubTotal) AS PosAmount" &
            " FROM POSSales" &
            " JOIN POSSalesItems ON POSSalesItems.POSSalesID = POSSales.ID" &
            " WHERE POSSalesItems.StockID = LibraryStockInformation.ID" &
            " AND CONVERT(datetime, FLOOR(CONVERT(float, POSSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            ") tblPos"
        'pSql &= " AND BranchID = 10 AND MachineNumber = 3"

        ' CROSS APPLY  CashSlipQty
        pSql &= " CROSS APPLY (" & vbCrLf &
            "SELECT SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity) AS Qty" &
            ",SUM(POSCashSlipSalesItems.SubTotal) AS Amount" &
            " FROM POSCashSlipSales" &
            " JOIN POSCashSlipSalesItems ON POSCashSlipSalesItems.CashSlipID = POSCashSlipSales.ID" &
            " WHERE POSCashSlipSalesItems.StockID = LibraryStockInformation.ID" &
            " AND CONVERT(datetime, FLOOR(CONVERT(float, POSCashSlipSales.LastUpdated))) BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            ") tblCashSlip"
        'pSql &= " AND BranchID = 10 AND MachineNumber = 3"

        pSql &= " WHERE 1=1" &
        pWhereActive &
        vWhere

        ' some fields are multiplied by zero (instead of just removing them from SQL) to emphasize that they are not included towards Qty
        pSqlStocks = "SELECT * " &
            ",SalesQty * 0 + SalesBranchQty * 0 + InvoiceQty + InvoiceBranchQty * 0 + BackorderQty * 0 + BackorderBranchQty * 0 + PosQty + CashSlipQty AS Qty" &
            " FROM (" & pSql & ")t" &
            " WHERE SalesQty * 0 + SalesBranchQty * 0 + InvoiceQty + InvoiceBranchQty * 0 + BackorderQty * 0 + BackorderBranchQty * 0 + PosQty + CashSlipQty > 0"

        pSql = pSqlStocks &
            " ORDER BY StockDescription"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function DrpReportCrossTab(ByRef oDateFrom As Date, ByRef oDateTo As Date, vWhere As String, _
        vDurationType As eDurationType,
        ByRef oDtColumns As DataTable,
        vKeywords As String,
        vIncludeDurationColumns As Boolean,
        vIncludeSupplierColumns As Boolean,
        vIncludeSourceColumns As Boolean) As DataTable

        Dim pDt As DataTable
        Dim pDR As DataRow
        Dim pDateRange As DateRange
        Dim pDtBestSupplier As DataTable
        Dim pDtUnits As DataTable

        Select Case vDurationType
            Case eDurationType.Weekly
                oDateFrom = oDateFrom.AddDays(-oDateFrom.DayOfWeek)
                oDateTo = oDateTo.AddDays(7 - oDateTo.DayOfWeek).AddTicks(-1)

            Case eDurationType.SemiMonthly
                Throw New NotImplementedException("Semi-monthly is not yet implemented")

            Case eDurationType.Monthly
                oDateFrom = New Date(oDateFrom.Year, oDateFrom.Month, 1)
                oDateTo = New Date(oDateTo.Year, oDateTo.Month, 1).AddMonths(1).AddTicks(-1)

            Case Else
                Throw New ArgumentException("unknown frequency", "vDurationType")

        End Select

        SaveCache(oDateFrom, oDateTo, vDurationType, ProData.MinDate)
        pDt = GetDrpCache(oDateFrom, oDateTo, vDurationType, vWhere, vKeywords, vIncludeDurationColumns, vIncludeSourceColumns)
        If pDt.Rows.Count = 0 Then Return Nothing
        oDtColumns = New DataTable
        oDtColumns.Columns.Add("Text")
        oDtColumns.Columns.Add("DateText")

        For Each pDateRange In GetDateRanges(oDateFrom, oDateTo, vDurationType, True)
            pDR = oDtColumns.NewRow
            pDR("Text") = DisplayableDateRange(pDateRange.DateFrom, pDateRange.DateTo)
            pDR("DateText") = Format(pDateRange.DateFrom, "yyyyMMdd")
            oDtColumns.Rows.Add(pDR)
        Next pDateRange

        If vIncludeSupplierColumns Then
            pDtBestSupplier = mParent.LibraryStock.GetBestSupplierPricesCache(pDt, Date.MaxValue, True)
            pDt = MergeTable(pDtBestSupplier, pDt,
                {"StockId"},
                {"BestPrice", "BestPriceUnit", "BestPriceUnitQty", "BestPriceSupplierID", "SupplierName", "BestPriceUnit2", "ShipmentMode"})
            If pDt.Columns.Contains("BestPriceSupplierID") Then
                pDt.Columns("BestPriceSupplierID").ColumnName = "SupplierId"
            End If
        End If

        pDtUnits = mParent.LibraryStock.GetUnits(pDt)
        pDt = MergeTable(pDtUnits, pDt,
            {"StockId"},
            {"UnitMeasure2", "UnitMeasure", "UnitQty"})

        Return pDt

    End Function

    Public Function GetDrpCache(ByRef oDateFrom As Date, ByRef oDateTo As Date, vDurationType As eDurationType, vWhere As String, vKeywords As String, _
        vIncludeDurationColumns As Boolean,
        vIncludeSourceColumns As Boolean) As DataTable

        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable
        Dim pDateRange As DateRange
        Dim pDateRanges As List(Of DateRange)

        Select Case vDurationType
            Case eDurationType.Weekly
                pSql = "w"

            Case eDurationType.SemiMonthly
                pSql = "sm"

            Case eDurationType.Monthly
                pSql = "m"

            Case Else
                Throw New ArgumentException("vDurationType type must be 1 of the eFrequency values", "vDurationType")

        End Select

        pFinder = New Finder
        With pFinder
            .ProData = mProData
            .SearchString = vKeywords
            With .Fields
                With .Add("StockID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With

                pDateRanges = GetDateRanges(oDateFrom, oDateTo, vDurationType, True)
                If pDateRanges.Count > 0 Then oDateFrom = pDateRanges(0).DateFrom
                For Each pDateRange In pDateRanges
                    If GetRpDate() < pDateRange.DateTo Then Exit For
                    If vIncludeDurationColumns Then
                        .Add(CreateFindField("SalesQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("SalesBranchQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("InvoiceQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("InvoiceBranchQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("BackorderQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("BackorderBranchQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("PosQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("CashSlipQty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                        .Add(CreateFindField("Qty", pDateRange.DateFrom, pDateRange.DateTo, pSql))
                    End If
                    oDateTo = pDateRange.DateTo
                Next pDateRange
                If vIncludeSourceColumns Then
                    .Add(CreateFindField("SalesQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("SalesBranchQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("InvoiceQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("InvoiceBranchQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("BackorderQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("BackorderBranchQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("PosQty", oDateFrom, oDateTo, pSql, False, True))
                    .Add(CreateFindField("CashSlipQty", oDateFrom, oDateTo, pSql, False, True))
                End If

                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "LibraryStockInformation.StockNumber"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
                    " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
                    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
                    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
                    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')"
                End With
                With .Add("StocksOnHand", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "ISNULL((SELECT TOP 1 StocksOnHand" &
                        " FROM WholesaleStockInventory" &
                        " WHERE WholesaleStockInventory.StockID = DrpCacheSalesDetails.StockId" &
                        " ORDER BY WholesaleStockInventory.LastUpdated DESC), 0)"
                End With
                With .Add("Qty", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "ISNULL((SELECT SUM(Qty)), 0)"
                End With
            End With
            .SqlFrom = " FROM DrpCacheSales" &
                " JOIN DrpCacheSalesDetails ON DrpCacheSalesDetails.CacheSalesId = DrpCacheSales.ID" &
                " LEFT JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheSalesDetails.StockId" & _
                " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
                " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID "
            .SqlWhere = "WHERE DrpCacheSales.DateStart >= " & ReadyString(oDateFrom.Date) &
                " AND DrpCacheSales.DateStart <= " & ReadyString(oDateTo.Date) &
                " AND DurationType = " & ReadyString(pSql)
            If CStr2(vWhere).Length > 0 Then
                .SqlWhere &= " AND (" & vWhere & ")"
            End If
            .SqlGroup = "GROUP BY StockID" &
                "," & .Fields("StockNumber").Expression &
                "," & .Fields("StockDescription").Expression
            pDt = .Requery()
        End With

        Return pDt

    End Function

    Private Function CreateFindField(vName As String, vDateFrom As Date, vDateTo As Date, vDuration As String,
        Optional vPrefixStartFrom As Boolean = True,
        Optional vUseSum As Boolean = False) As FinderField

        Dim pOut As FinderField

        pOut = New FinderField
        If vPrefixStartFrom Then
            pOut.FieldName = vName & Format(vDateFrom, "yyyyMMdd")
        Else
            pOut.FieldName = vName
        End If
        pOut.DBType = SqlDbType.Int
        pOut.Expression = "ISNULL((SELECT " & If(vUseSum, "SUM(" & vName & ")", vName) &
            " FROM DrpCacheSales tblMaster" &
            " JOIN DrpCacheSalesDetails tblDetails ON tblDetails.CacheSalesId = tblMaster.ID" &
            " WHERE tblDetails.StockId = DrpCacheSalesDetails.StockId" &
            " AND tblMaster.DateStart >= " & ReadyString(vDateFrom.Date) &
            " AND tblMaster.DateStart <= " & ReadyString(vDateTo.Date) &
            " AND DurationType = " & ReadyString(vDuration) &
            "), 0)"
        pOut.Searchable = False

        Return pOut

    End Function

    Public Sub SaveDrpReportCache(vDateStart As Date, vDurationType As eDurationType, vData As DataTable)

        Dim pIU As InsertUpdate
        Dim pDR As DataRow
        Dim pDate As Date
        Dim pDurationType As String
        Dim pSql As String
        Dim pId As String

        Select Case vDurationType
            Case eDurationType.Weekly
                pDurationType = "w"

            Case eDurationType.SemiMonthly
                pDurationType = "sm"

            Case eDurationType.Monthly
                pDurationType = "m"

            Case Else
                Throw New ArgumentException("vDurationType type must be 1 of the eFrequency values", "vDurationType")

        End Select

        pDate = mProData.GetDate

        Try
            mProData.BeginTransaction()

            pSql = "DELETE FROM DrpCacheSalesDetails" &
                " WHERE CacheSalesId IN " &
                " (SELECT ID FROM DrpCacheSales" &
                " WHERE DurationType = " & ReadyString(pDurationType) &
                " AND DateStart = " & ReadyString(vDateStart.Date) & ")"
            mProData.ExecuteSQL(pSql)

            pSql = "DELETE FROM DrpCacheSales" &
                " WHERE DurationType = " & ReadyString(pDurationType) &
                " AND DateStart = " & ReadyString(vDateStart.Date)
            mProData.ExecuteSQL(pSql)

            pIU = New InsertUpdate("DrpCacheSales")
            pId = Guid.NewGuid.ToString
            pIU("ID") = ReadyString(pId)
            pIU("DurationType") = ReadyString(pDurationType)
            pIU("DateCached") = ReadyString(pDate)
            pIU("DateStart") = ReadyString(vDateStart.Date)
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)

            pIU = New InsertUpdate("DrpCacheSalesDetails")
            pIU("CacheSalesId") = ReadyString(pId)
            For Each pDR In vData.Rows
                pIU("StockID") = ReadyString(pDR("StockID"))
                pIU("SalesQty") = pDR("SalesQty")
                pIU("SalesBranchQty") = pDR("SalesBranchQty")
                pIU("InvoiceQty") = pDR("InvoiceQty")
                pIU("InvoiceBranchQty") = pDR("InvoiceBranchQty")
                pIU("BackorderQty") = pDR("BackorderQty")
                pIU("BackorderBranchQty") = pDR("BackorderBranchQty")
                pIU("PosQty") = pDR("PosQty")
                pIU("CashSlipQty") = pDR("CashSlipQty")
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            Next pDR
            mProData.CommitTransaction()
        Catch ex As Exception
            Try
                mProData.RollbackTransaction()
            Catch ex2 As Exception
                Stop2()
            End Try
            Throw
        End Try

    End Sub

    Private Function GetCacheInfo(vDateStartFrom As Date, vDateStartTo As Date, ByVal vFrequency As eDurationType, vMinDate As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDurationType As String

        Select Case vFrequency
            Case eDurationType.Weekly
                pDurationType = "w"

            Case eDurationType.SemiMonthly
                pDurationType = "sm"

            Case eDurationType.Monthly
                pDurationType = "m"

            Case Else
                Throw New ArgumentException("vDurationType type must be 1 of the eFrequency values", "vDurationType")

        End Select

        pSql = "SELECT DateStart" &
            ",DateCached" &
            " FROM DrpCacheSales" &
            " WHERE DurationType = " & ReadyString(pDurationType) &
            " AND DateStart >= " & ReadyString(vDateStartFrom) &
            " AND DateStart <= " & ReadyString(vDateStartTo) &
            " AND DateCached >= " & ReadyString(vMinDate) &
            " ORDER BY DateStart"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function SaveCache(vDateStartFrom As Date, vDateStartTo As Date, vDurationType As eDurationType, vMinDate As Date) As Integer

        Dim pDtCacheInfo As DataTable
        Dim pDt As DataTable
        Dim pDateRange As DateRange
        Dim pDRs As DataRow()

        pDtCacheInfo = GetCacheInfo(vDateStartFrom, vDateStartTo, vDurationType, vMinDate)

        For Each pDateRange In GetDateRanges(vDateStartFrom, vDateStartTo, vDurationType, True)
            If GetRpDate() < pDateRange.DateTo Then Exit For
            pDRs = pDtCacheInfo.Select("DateStart = " & ReadyString(pDateRange.DateFrom))
            If pDRs.Length = 0 _
                OrElse pDRs(0)("DateCached") < pDateRange.DateTo Then

                pDt = DrpReport(pDateRange.DateFrom, pDateRange.DateTo, "", True, True, 100, 100, 100, 100)
                SaveDrpReportCache(pDateRange.DateFrom, vDurationType, pDt)
            End If
        Next pDateRange

    End Function
End Class

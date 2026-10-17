Imports Aspose.Cells

Public Class TransInvoiceLegacy

    Dim mProData As ProData
    Dim mParent As Business

    Public Const gIdChi As String = "4092334E-CCD7-47B0-B842-738F80CFBA81"
    Public Const gIdChiCas As String = "D90FC260-597C-44F8-835D-1A0ED21CEDFA"
    Public Const gIdDr As String = "CF3104FA-7F23-48A9-A639-963B38E07AFC"
    Public Const gIdTr As String = "F06F9853-F5C1-4D22-99D8-D00DC6D59ADE"
    Public Const gIdBt As String = "C5933A80-DB20-4B88-ACE7-646AB2AE3BAF"
    Public Const gIdCs As String = "A8B3ED0C-977E-4950-A8D2-77B1B8E73BDB"

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public ReadOnly Property Parent() As Business
        Get

            Return mParent

        End Get
    End Property

    Public Function NotYetInvoiced() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT TOP 100 PERCENT " & _
            "TransStockSortingListItems.StockSortingListID AS ID," & _
            "RIGHT('0000000' + CONVERT(varchar, TransSalesOrder.OrderNo), 7) AS [Sales Order No.]," & _
            "TransSalesOrder.OrderDate AS [Sales Order Date]," & _
            "PONum AS [PO Number], PODate AS PODate," & _
            "LibraryCustomers.CompanyName AS CustomerName," & _
            "ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name, '') + COALESCE(LibraryProvinces.Name + LibraryCities.Name, LibraryLocations.LocationName, '') AS [Delivery Address], " & _
            "TransSalesOrder.DeliveryDate AS [Delivery Date]," & _
            "RIGHT('0000000' + CONVERT(varchar, TransStockSortingList.SortingListNo), 7) AS [Sorting List No.]," & _
            "TransStockSortingList.SortingListDate AS [Sorting List Date]," & _
            "RIGHT('0000000' + CONVERT(varchar,TransStockPickingList.PickingListNo), 7) AS [Picking List No.]," & _
            "TransStockPickingList.PickingListDate AS [Picking List Date]," & _
            "TransStockPickingList.IsRushPrint AS [Rush Order]"

        pSql &= ",LibraryCustomers.ID AS CustomerID" & _
            ",TransSalesOrder.CustomerAddressID AS BillingAddressID" & _
            ",TransSalesOrder.DeliverToID AS DeliveryAddressID" & _
            ",TransSalesOrder.DeliveryDate" & _
            ",TransSalesOrder.IsTaxable," & _
            ",TransSalesOrder.IsFromBackOrder," & _
            ",TransSalesOrder.Remarks " & _
            ",TransSalesOrder.PONum" & _
            ",TransSalesOrder.ID AS SalesOrderID"

        pSql &= " FROM TransStockSortingListItems INNER JOIN " & _
            " TransStockSortingList ON TransStockSortingListItems.StockSortingListID = TransStockSortingList.ID INNER JOIN " & _
            " TransSalesOrder ON TransStockSortingListItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " & _
            " LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID INNER JOIN " & _
            " TransStockPickingList ON TransStockSortingList.StockPickingListID = TransStockPickingList.ID" & _
            " LEFT JOIN LibraryCustomersAddresses ON TransSalesOrder.DeliverToID = LibraryCustomersAddresses.ID" & _
            " LEFT JOIN LibraryLocations ON LibraryCustomersAddresses.LocationID = LibraryLocations.ID "

        pSql &= " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" & _
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID"

        pSql &= " WHERE TransStockSortingListItems.IsInvoiced=0" & _
            " AND TransSalesOrder.IsCancelled = 0" & _
            " ORDER BY RIGHT('0000000' + CONVERT(varchar, TransSalesOrder.OrderNo), 7) ASC," & _
            " RIGHT('0000000' + CONVERT(varchar, TransStockSortingList.SortingListNo), 7) ASC "

        pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDefaultId"></param>
    ''' <param name="vExtention">InvoiceNoExtension</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function NextInvoiceNumber(ByVal vDefaultId As String, _
        Optional ByVal vExtention As String = "") As Integer

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT Max(InvoiceNumber)" & _
            " FROM TransInvoice" & _
            " WITH (TABLOCKX)" & _
            " WHERE InvoiceDefaultID = " & ReadyString(vDefaultId) &
            " AND InvoiceNoExtension = " & ReadyString(vExtention)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CDec2(pDT.Rows(0)(0)) + 1
        Else
            Return 1
        End If

    End Function

    Public Function ById(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        'pSql = "spGetInvoiceList " & ReadyString(vID)
        pSql = "SELECT TransInvoice.CustomerID" & vbCrLf &
            ",TransInvoice.InvoiceNumber" & vbCrLf &
            ",TransInvoice.InvoiceDefaultID" & vbCrLf &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            " FROM TransInvoice" &
            " WHERE TransInvoice.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function CanPrintInvoice(ByVal vInvoiceID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT COUNT(*)" & _
            " FROM TransInvoicePrints" & _
            " WHERE InvoiceID = TransInvoice.ID" & _
            " AND ApprovedBy IS NOT NULL" & _
            " AND DeniedBy IS NULL"
        ' the below code has been excluded from the code above
        ' " AND DatePrinted IS NULL" 

        pSql = "SELECT NumberOfPrints" & _
            ",NumberOfReprints" & _
            ",ISNULL((" & pSql & "), 0) AS ApprovedRequests" & _
            " FROM TransInvoice" & _
            " JOIN MainInvoiceDefaults" & _
            " ON TransInvoice.InvoiceDefaultID = MainInvoiceDefaults.ID" & _
            " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            If pDR("NumberOfPrints") <= pDR("NumberOfReprints") + pDR("ApprovedRequests") Then
                CanPrintInvoice = True
            End If
        Else
            CanPrintInvoice = True
        End If

    End Function

    Public Sub RequestReprint(ByVal vInvoiceID As String, ByVal vRemarks As String, ByVal vUserID As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoicePrints")
        pIU("ID") = "newid()"
        pIU("InvoiceID") = ReadyString(vInvoiceID)
        pIU("DateRequested") = "GETDATE()"
        pIU("RequestedBy") = vUserID
        pIU("Remarks") = ReadyString(vRemarks)

        pSql = pIU.GetInsert
        mProData.Execute(pSql)

    End Sub

    Public Function HasPendingPrintRequest(ByVal vID As String) As Boolean

        Dim pSql As String

        pSql = "SELECT COUNT(*) AS Count" & _
            " FROM TransInvoicePrints" & _
            " WHERE InvoiceID = " & ReadyString(vID) & _
            " AND DateApproved IS NULL" & _
            " AND DateDenied IS NULL"
        Return 0 < CDec2(mProData.ExecuteScalar(pSql))

    End Function

    ' this also used by TransSalesOrder
    Public Function AddProfitabilityColumns(ByVal vDT As DataTable) As DataTable

        Dim pDR As DataRow
        Dim pUnitGp As Decimal
        Dim pUnitGpm As Double
        Dim pLineGp As Decimal
        Dim pLineGpm As Decimal
        Dim pLandedCost As Decimal

        If vDT.Columns.Contains("Cost") = False Then vDT.Columns.Add("Cost", GetType(Decimal))
        If vDT.Columns.Contains("UnitGP") = False Then vDT.Columns.Add("UnitGP", GetType(Decimal))
        If vDT.Columns.Contains("UnitGPM") = False Then vDT.Columns.Add("UnitGPM", GetType(Decimal))
        If vDT.Columns.Contains("LineGP") = False Then vDT.Columns.Add("LineGP", GetType(Decimal))
        If vDT.Columns.Contains("LineGPM") = False Then vDT.Columns.Add("LineGPM", GetType(Decimal))
        If vDT.Columns.Contains("LineCost") = False Then vDT.Columns.Add("LineCost", GetType(Decimal))
        If vDT.Columns.Contains("CurrentCost") = False Then vDT.Columns.Add("CurrentCost", GetType(Decimal))
        If vDT.Columns.Contains("LineCost") = False Then vDT.Columns.Add("LineCost", GetType(Decimal))

        For Each pDR In vDT.Rows
            pDR("Cost") = CDec2(pDR("Cost"))
            mParent.LibraryStockLegacy.GetProfitability(
                vStockID:=CStr2(pDR("StockID")),
                vQty:=pDR("Quantity"),
                vUnitQty:=pDR("ActualQuantity"),
                vUnitPrice:=pDR("UnitPrice"),
                oUnitGP:=pUnitGp,
                oUnitGPM:=pUnitGpm,
                oLineGP:=pLineGp,
                oLineGPM:=pLineGpm,
                oLandedCost:=pLandedCost,
                vHistoricCost:=pDR("Cost"))

            If pDR("Cost") = 0 Then pDR("Cost") = pLandedCost
            'pDR("CurrentCost") = pLandedCost
            pDR("CurrentCost") = pLandedCost * pDR("ActualQuantity") ' test code. mar 16, 2013
            pDR("UnitGP") = pUnitGp
            pDR("UnitGPM") = pUnitGpm
            pDR("LineGP") = pLineGp
            pDR("LineGPM") = pLineGpm
            pDR("LineCost") = pDR("Cost") * pDR("ActualQuantity") * pDR("Quantity")
        Next pDR

        Return vDT

    End Function

    Event CreateDone(ByVal sender As Object, ByVal e As CreateDoneArgs)

    Public Class CreateDoneArgs
        Public ErrorMessage As String

        Public Sub New(ByVal vError As String)
            ErrorMessage = vError
        End Sub
    End Class

    Public Function UninvoicedBranchOrders() As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DISTINCT TransStockSortingList.ID" & _
            ",TransStockSortingListItems.SalesOrderID" & _
            ",TransSalesOrder.CustomerID" & _
            ",TransSalesOrder.OrderNo" & _
            ",TransSalesOrder.OrderDate" & _
            ",LibraryCustomers.CompanyName" & _
            " FROM TransStockSortingListItems" & _
            " WITH (TABLOCKX)" & _
            " JOIN TransStockSortingList ON TransStockSortingList.ID = TransStockSortingListItems.StockSortingListID" & _
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransStockSortingListItems.SalesOrderID" & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" & _
            " WHERE TransSalesOrder.IsCancelled = 0" & _
            " AND TransStockSortingListItems.IsInvoiced = 0" & _
            " AND LibraryCustomers.IsCompanyBranch = 1" & _
            " AND ISNULL(TransSalesOrder.InStore, 0) = 0" &
            " AND TransSalesOrder.OrderDate >= '7/1/2010'"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    '    Public Function AutoCreateBranchInvoices(ByRef oError As String) As DataTable

    '        Dim pDT As DataTable
    '        Dim pDR As DataRow
    '        Dim pCreator As Creator
    '        Dim pBranchInvoiceDefaultId, pCustomerInvoiceDefaultId As String
    '        Dim pInvoiceType As String
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = Nothing
    '#If Debug = 0 Then
    '            Try
    '#End If
    '        mProData.BeginTransaction()

    '        If mParent.SystemDefaults.InvoiceTypeForBranchAutoInvoice(pBranchInvoiceDefaultId, pInvoiceType, oError) = False Then
    '            mProData.RollbackTransaction()
    '            RaiseEvent CreateDone(Me, New CreateDoneArgs(oError))
    '            Return Nothing
    '        End If
    '        pCustomerInvoiceDefaultId = TransInvoice.gIdChi

    '        '// uninvoiced, served branch orders
    '        pDT = UninvoicedBranchOrders()
    '        If pDT.Rows.Count = 0 Then
    '            mProData.RollbackTransaction()
    '            'oError = "No withdrawn, uninvoiced order from branch."
    '            Return Nothing
    '        End If

    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID", GetType(Guid))
    '        pDtOut.Columns.Add("InvoiceNum", GetType(Integer))
    '        pDtOut.Columns.Add("OrderNo", GetType(Integer))

    '        pDT = pDT.DefaultView.ToTable(True, "ID", "SalesOrderID", "CustomerID", "OrderNo")
    '        For Each pDR In pDT.Rows
    '            pCreator = New Creator(mProData, Me)
    '            pCreator.StockSortingListID = CStr2(pDR("ID"))
    '            If pCreator.GetStocks() = False Then ' set StockSortingListID before calling this
    '                pCreator.InvoiceNumber = 0
    '            End If
    '            pCreator.InvoiceDefaultID = pBranchInvoiceDefaultId
    '            pCreator.InvoiceType = pInvoiceType
    '            pCreator.InvoiceDate = mProData.GetDate.Date
    '            pCreator.SalesOrderID = CStr2(pDR("SalesOrderID"))
    '            pCreator.CustomerID = CStr2(pDR("CustomerID"))
    '            pCreator.BillingAddressID = mParent.LibraryCustomers.GetBillingAddress(CStr2(pDR("CustomerID")))
    '            pCreator.DeliveryAddressID = mParent.LibraryCustomers.GetDeliveryAddress(CStr2(pDR("CustomerID")))
    '            pCreator.Remarks = "Auto-created invoice"
    '            pCreator.UserID = modClient.EmployeeWebID
    '            pCreator.EncodedBy = modClient.EmployeeWebID
    '            pCreator.InvoiceDateTime = mProData.GetDate
    '            If pCreator.Save(True, oError) = False Then
    '                mProData.RollbackTransaction()
    '                Return Nothing
    '            End If

    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pCreator.ID
    '            pDrOut("InvoiceNum") = pCreator.InvoiceNumber
    '            pDtOut.Rows.Add(pDrOut)
    '        Next pDR
    '        mProData.CommitTransaction()
    '        oError = Nothing
    '        Return pDtOut
    '#If Debug = 0 Then
    '            Catch ex As Exception
    '                oError = ex.Message
    '                Try
    '                    mProData.RollbackTransaction()
    '                Catch ex2 As Exception
    '                End Try
    '                Return Nothing
    '            End Try
    '#End If

    '    End Function

    Public Function IsInvoiceExist(ByVal vInvoiceDefaultID As String, ByVal vInvoicNumber As Long, ByVal vInvoiceNumberExtension As String, _
    ByRef oError As String) As Boolean

        Dim strSQL As String
        Dim rsTmp As DataTable
        Dim rsTmp2 As DataTable

        IsInvoiceExist = False

        oError = ""
        IsInvoiceExist = False

        'rsTmp = New ADODB.Recordset
        strSQL = "SELECT ID," & _
            "StartNumber," & _
            "EndNumber," & _
            "IsRestartCount," & _
            "Bracket " & _
            " FROM MainInvoiceDefaults " & _
            " WHERE ID='" & vInvoiceDefaultID & "'"
        rsTmp = mProData.Execute(strSQL)

        'rsTmp.Open(strSQL, DBConnection, adOpenStatic)

        If rsTmp.Rows.Count > 0 Then

            'IsInvoiceExceed = False

            'rsTmp2 = New ADODB.Recordset

            strSQL = "SELECT InvoiceDefaultID," & _
                "InvoiceNumber," & _
                "BracketNumber " & _
                " FROM TransInvoice " & _
                " WHERE InvoiceDefaultID ='" & vInvoiceDefaultID & "' " & _
                " AND InvoiceNumber = " & vInvoicNumber & _
                " AND InvoiceNoExtension = '" & Replace(Trim(vInvoiceNumberExtension), "'", "''") & "' "
            ' AND BracketNumber = " & rsTmp!Bracket
            'rsTmp2 = GetRS(strSQL)
            rsTmp2 = mProData.Execute(strSQL)

            If rsTmp2.Rows.Count > 0 Then
                oError = "Invoice number already exist"
                IsInvoiceExist = True
            Else
                strSQL = "SELECT OrderNo" & _
                    " FROM TransSalesOrder" & _
                    " WHERE PendingInvoiceNum = " & ReadyString(vInvoicNumber) & _
                    " AND PendingInvoiceType = " & ReadyString(vInvoiceDefaultID) & _
                    " AND IsCancelled = 0"
                rsTmp2.Dispose()
                rsTmp2 = mProData.Execute(strSQL)
                If rsTmp2.Rows.Count Then
                    oError = "Invoice Number is already assigned to in-store Sales Order Number " & rsTmp2.Rows(0)("OrderNo") & ". Please enter another invoice number or cancel that sales order."
                    IsInvoiceExist = True
                    Exit Function
                End If

                'Check if invoice reach the max invoice number count
                If vInvoicNumber > (rsTmp.Rows(0)("EndNumber")) Then
                    If CDec2(rsTmp.Rows(0)("IsRestartCount")) = 0 Then
                        oError = "Invoice Number exceed the max invoice number count."
                        IsInvoiceExist = True
                        Exit Function
                    Else
                        oError = "Invoice Number exceed the max invoice number count."
                        IsInvoiceExist = True
                        'IsInvoiceExceed = True
                        Exit Function
                        '                    strSQL = "Update MainInvoiceDefaults Set " & _
                        '                        "Bracket=" & rsTmp!Bracket + 1 & _
                        '                        " WHERE ='" & vInvoiceDefaultID & "'"
                        '                    DBConnection.Execute strSQL
                    End If
                End If
            End If
        End If

    End Function

    Public Function GetSummary(ByVal vYear1 As Integer, ByVal vYear2 As Integer, ByVal vStockIds As String, ByVal vGroupByStock As Boolean) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String

        If CStr2(vStockIds).Length > 0 Then
            pWhere = " AND LibraryStockInformation.ID IN " & vStockIds
        End If

        '// cash slip
        pSql = " SELECT POSCashSlipSalesItems.StockID" & _
            ",SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) AS QtySold" & _
            ",SUM(POSCashSlipSalesItems.SubTotal) AS Amount" & _
            ",SUM(POSCashSlipSalesItems.COGS) AS COGS" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(POSCashSlipSales.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, POSCashSlipSales.LastUpdated) AS DateMonth" &
            ",DATEPART(year, POSCashSlipSales.LastUpdated) AS YearValue" &
            ",DATEPART(month, POSCashSlipSales.LastUpdated) AS MonthValue" &
            " " & _
            " FROM POSCashSlipSalesItems" & _
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " " & _
            " WHERE (DATEPART(year, POSCashSlipSales.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, POSCashSlipSales.LastUpdated) = " & vYear2 & ")" &
            pWhere &
            " " & _
            " GROUP BY POSCashSlipSalesItems.StockID" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(POSCashSlipSales.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, POSCashSlipSales.LastUpdated)" &
            ",DATEPART(year, POSCashSlipSales.LastUpdated)" &
            ",DATEPART(month, POSCashSlipSales.LastUpdated)" &
            " " & _
            " HAVING SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) > 0"

        '// POS
        pSql &= " UNION ALL "
        pSql &= " SELECT POSSalesItems.StockID" & _
            ",SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) AS QtySold" & _
            ",SUM(POSSalesItems.SubTotal) AS Amount" & _
            ",SUM(POSSalesItems.COGS) AS COGS" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(POSSales.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, POSSales.LastUpdated) AS DateMonth" &
            ",DATEPART(year, POSSales.LastUpdated) AS YearValue" &
            ",DATEPART(month, POSSales.LastUpdated) AS MonthValue" &
            " " & _
            " FROM POSSalesItems" & _
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " " & _
            " WHERE (DATEPART(year, POSSales.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, POSSales.LastUpdated) = " & vYear2 & ")" &
            pWhere &
            " " & _
            " GROUP BY POSSalesItems.StockID" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(POSSales.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, POSSales.LastUpdated)" &
            ",DATEPART(year, POSSales.LastUpdated)" &
            ",DATEPART(month, POSSales.LastUpdated)" &
            " " & _
            " HAVING SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) > 0"

        '// invoice
        pSql &= " UNION ALL "
        pSql &= " SELECT TransInvoiceItems.StockID" & _
            ",SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) AS QtySold" & _
            ",SUM(TransInvoiceItems.SubTotal) AS Amount" & _
            ",SUM(TransInvoiceItems.COGS) AS COGS" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(TransInvoice.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, TransInvoice.LastUpdated) AS DateMonth" &
            ",DATEPART(year, TransInvoice.LastUpdated) AS YearValue" &
            ",DATEPART(month, TransInvoice.LastUpdated) AS MonthValue" &
            " " & _
            " FROM TransInvoiceItems" & _
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " LEFT JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" &
            " " & _
            " WHERE (DATEPART(year, TransInvoice.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, TransInvoice.LastUpdated) = " & vYear2 & ")" &
            " AND LibraryCustomers.IsCompanyBranch = 0" &
            " AND TransInvoice.IsCancelled = 0" &
            pWhere &
            " " & _
            " GROUP BY TransInvoiceItems.StockID" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(TransInvoice.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, TransInvoice.LastUpdated)" &
            ",DATEPART(year, TransInvoice.LastUpdated)" &
            ",DATEPART(month, TransInvoice.LastUpdated)" &
            " " & _
            " HAVING SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) > 0"

        'pSql &= " ORDER BY ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')"

        If vGroupByStock Then
            pSql = "SELECT *" &
                ",(SELECT TOP 1 LibraryUnitMeasures.UnitMeasure" &
                "   FROM LibraryStockMeasurements" &
                "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" &
                "   WHERE LibraryStockMeasurements.StockID = t.StockID" &
                "   ORDER BY LibraryStockMeasurements.Quantity) AS UnitMeasure" &
                " FROM (" & pSql & ") t"
        Else
            pSql = "SELECT SUM(QtySold) AS QtySold" &
                ",SUM(Amount) AS Amount" &
                ",SUM(COGS) AS COGS" &
                ",YearMonth" &
                ",YearValue" &
                ",MonthValue" &
                " FROM (" & pSql & ") t" &
                " GROUP BY YearMonth" &
                ",YearValue" &
                ",MonthValue" &
                " ORDER BY YearMonth"
        End If

        mProData.CommandTimeout = 10 * 60 * 1000
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <param name="vSupplierID">if nothing, include all suppliers</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSummaryPerSupplierAndBranch(ByVal vDateFrom As Date, ByVal vDateTo As Date,
        Optional ByVal vSupplierID As String = Nothing,
        Optional ByVal vShowCustomer As Boolean = False,
        Optional vIncludeAllSupplierStocks As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String

        If CStr2(vSupplierID).Length = 0 Then
            pWhere = " AND LibrarySuppliers.SendEmail = 1 OR LibrarySupplierStocks.SendEmail = 1" & vbCrLf &
                " AND LEN(LTRIM(RTRIM(ISNULL(LibrarySuppliers.EmailAddress, '')))) > 0" & vbCrLf &
                " AND LibrarySuppliers.IsActive = 1" & vbCrLf
        Else
            pWhere = " AND LibrarySuppliers.ID = " & ReadyString(vSupplierID) & vbCrLf
        End If

        '// cash slip
        pSql = " SELECT POSCashSlipSalesItems.StockID" & vbCrLf &
            ",SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(POSCashSlipSalesItems.UnitPrice * POSCashSlipSalesItems.Quantity) AS Amount" &
            ",POSCashSlipSales.BranchID" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",LibrarySuppliers.SupplierName" &
            ",LibrarySupplierStocks.SupplierID" &
            ",LibrarySuppliers.EmailAddress"
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If
        pSql &= " FROM POSCashSlipSalesItems" & _
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = POSCashSlipSales.BranchID" &
            " " & If(vIncludeAllSupplierStocks, "LEFT ", "") & "JOIN LibrarySupplierStocks ON LibrarySupplierStocks.StockID = LibraryStockInformation.ID" &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID"
        If vShowCustomer Then
            pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = POSCashSlipSales.CustomerID"
        End If
        pSql &= " WHERE POSCashSlipSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            pWhere &
            " " & _
            " GROUP BY POSCashSlipSalesItems.StockID" & _
            ",POSCashSlipSales.BranchID" & _
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",LibrarySuppliers.SupplierName" &
            ",LibrarySupplierStocks.SupplierID" &
            ",LibrarySuppliers.EmailAddress"
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If
        pSql &= " HAVING SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) > 0"

        '// POS
        pSql &= " UNION ALL " & vbCrLf
        pSql &= " SELECT POSSalesItems.StockID" & vbCrLf &
            ",SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(POSSalesItems.UnitPrice * POSSalesItems.Quantity) AS Amount" & vbCrLf &
            ",POSSales.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If

        pSql &= " FROM LibrarySupplierStocks" & vbCrLf &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & vbCrLf &
            " JOIN POSSalesItems ON POSSalesItems.StockID = LibrarySupplierStocks.StockID" & _
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & vbCrLf &
            " AND POSSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & vbCrLf &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = POSSales.BranchID"
        If vShowCustomer Then
            pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = POSSales.CustomerID"
        End If
        pSql &= " WHERE 1 = 1 " & pWhere & vbCrLf & vbCrLf &
            " GROUP BY POSSalesItems.StockID" & vbCrLf &
            ",POSSales.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " HAVING SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) > 0" & vbCrLf

        '// invoice
        pSql &= " UNION ALL " & vbCrLf
        pSql &= " SELECT TransInvoiceItems.StockID" & vbCrLf &
            ",SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS Amount" & vbCrLf &
            ",TransSalesOrder.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" & vbCrLf &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & vbCrLf &
            " LEFT JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" & vbCrLf &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = TransSalesOrder.BranchID" & vbCrLf &
            " JOIN LibrarySupplierStocks ON LibrarySupplierStocks.StockID = LibraryStockInformation.ID" & vbCrLf &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & vbCrLf
        'If vShowCustomer Then  ' no need for this condition
        pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf
        'End If
        pSql &= " WHERE TransInvoice.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & vbCrLf &
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf &
            pWhere &
            " " & vbCrLf &
            " GROUP BY TransInvoiceItems.StockID" & vbCrLf &
            ",TransSalesOrder.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " HAVING SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) > 0" & vbCrLf

        pSql = "SELECT *" & vbCrLf &
            ",(SELECT TOP 1 LibraryUnitMeasures.UnitMeasure" & vbCrLf &
            "   FROM LibraryStockMeasurements" & vbCrLf &
            "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & vbCrLf &
            "   WHERE LibraryStockMeasurements.StockID = t.StockID" & vbCrLf &
            "   ORDER BY LibraryStockMeasurements.Quantity) AS UnitMeasure" & vbCrLf &
            " FROM (" & pSql & ") t" & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSummaryPerCustomerAndAgent(
        ByVal vOldDateFrom As Date, ByVal vOldDateTo As Date,
        ByVal vNewDateFrom As Date, ByVal vNewDateTo As Date) As DataTable

        Dim pSql As String

        pSql = "SELECT * FROM (" & _
        GetSummaryPerCustomerAndAgentSubSql(vOldDateFrom, vOldDateTo) & " AS OldTotal," &
        GetSummaryPerCustomerAndAgentSubSql(vNewDateFrom, vNewDateTo) & " AS NewTotal" &
        " " & _
        " 	,(SELECT TOP 1 LibraryProvinces.Name" & _
        "       FROM LibraryCustomersAddresses" & _
        "       JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" & _
        "       JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
        "       JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
        "       JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
        "       WHERE LibraryCustomersAddresses.CustomerID = LibraryCustomers.ID" & _
        "       ORDER BY LibraryCustomersAddresses.IsBillingAddress DESC" & _
        "       ,LibraryProvinces.Name	" & _
        "       ,LibraryCities.Name" & _
        "       ,LibraryDistricts.Name" & _
        "       ,LibraryStreetNames.StreetName" & _
        " 	) AS Location      " & _
        ",LibraryCustomers.CompanyName" & _
        ",EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName AS Agent" & _
        ",EmployeeProfiles.EmailAddress AS AgentEmail" &
        ",LibraryCustomersAgents.EmployeeID" & _
        "" & _
        " FROM LibraryCustomers" & _
        " JOIN LibraryCustomersAgents ON LibraryCustomersAgents.CustomerID = LibraryCustomers.ID" & _
        " JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles ON EmployeeProfiles.ID = LibraryCustomersAgents.EmployeeID" & _
        " ) T" & _
        " WHERE Total > 0" & _
        " ORDER BY CompanyName"

    End Function

    Private Function GetSummaryPerCustomerAndAgentSubSql(ByVal vDateFrom As Date, ByVal vDateTo As Date) As String

        Return "(SELECT SUM(TransInvoiceItems.SubTotal)" & _
            " FROM TransInvoiceItems" & _
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & _
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & _
            " WHERE ISNULL(TransInvoice.PerfectOrder, 0) = 0" & _
            " AND TransInvoice.IsCancelled = 0" & _
            " AND TransInvoice.CustomerID = LibraryCustomers.ID" & _
            " AND MainInvoiceDefaults.Billable = 1" & _
            " AND CONVERT(datetime, FLOOR(CONVERT(float, TransInvoice.InvoiceDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateFrom.Date) &
            ")+ " &
            " (SELECT SUM(TransSalesOrderItems.SubTotal)" & _
            " FROM TransSalesOrderItems" & _
            " JOIN TransInvoice ON TransInvoice.SalesOrderID = TransSalesOrderItems.SalesOrderID" & _
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & _
            " WHERE TransInvoice.PerfectOrder = 1" & _
            " AND TransInvoice.IsCancelled = 0" & _
            " AND TransInvoice.CustomerID = LibraryCustomers.ID" & _
            " AND MainInvoiceDefaults.Billable = 1    " & _
            " AND CONVERT(datetime, FLOOR(CONVERT(float, TransInvoice.InvoiceDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateFrom.Date) &
            ")"

    End Function

    Public Function GetInvoiceID(ByVal vInvoiceTypeAndNumber As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        Do Until vInvoiceTypeAndNumber = vInvoiceTypeAndNumber.Replace("  ", " ")
            vInvoiceTypeAndNumber = vInvoiceTypeAndNumber.Replace("  ", " ")
        Loop

        pSql = "SELECT TransInvoice.ID" &
            ",SalesOrderID" &
            ",LibraryCustomers.CompanyName" & vbCrLf &
            ",TransInvoice.LastUpdated" & vbCrLf &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            ",Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') AS InvoiceNum" & vbCrLf &
            " FROM TransInvoice" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & vbCrLf &
            " LEFT JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " WHERE Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') = " & ReadyString(vInvoiceTypeAndNumber)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT
        End If

        pSql = "SELECT TransInvoice.ID" &
            ",SalesOrderID" &
            ",LibraryCustomers.CompanyName" & vbCrLf &
            ",TransInvoice.LastUpdated" &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            ",Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') AS InvoiceNum" & vbCrLf &
            " FROM TransInvoice" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" &
            " LEFT JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " WHERE Abbrevation + ' - ' + CONVERT(varchar, TransInvoice.InvoiceNumber) + ISNULL(TransInvoice.InvoiceNoExtension,'') = " & ReadyString(vInvoiceTypeAndNumber)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetPrintableInvoice(ByVal vBranchInvoiceDefaultID As String, vCustomerInvoiceDefaultID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 COUNT(*)" & vbCrLf & _
            " FROM TransInvoicePrints" & vbCrLf & _
            " WHERE InvoiceID = TransInvoice.ID" & vbCrLf & _
            " AND ApprovedBy IS NOT NULL" & vbCrLf & _
            " AND DeniedBy IS NULL"
        ' the below code has been excluded from the code above
        ' " AND DatePrinted IS NULL" 

        pSql = "SELECT TransInvoice.ID" & vbCrLf & _
            ",MainInvoiceDefaults.Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') AS InvoiceNumber" & vbCrLf & _
            ",LibraryCustomers.CompanyName" & vbCrLf &
            ",TransInvoice.NumberOfPrints" & vbCrLf & _
            ",MainInvoiceDefaults.NumberOfReprints" & vbCrLf & _
            ",ISNULL((" & pSql & "), 0) AS ApprovedRequests" & vbCrLf & _
            ",TransInvoice.LastUpdated" & vbCrLf &
            ",TransInvoice.CustomerID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf & _
            " JOIN MainInvoiceDefaults ON TransInvoice.InvoiceDefaultID = MainInvoiceDefaults.ID" & vbCrLf & _
            " WHERE TransInvoice.InvoiceDefaultID IN (" & ReadyString(vBranchInvoiceDefaultID) & ", " & ReadyString(vCustomerInvoiceDefaultID) & ")" & vbCrLf & _
            " AND TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND (SELECT SUM(Quantity) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID) >= 1" & vbCrLf & _
            " AND TransInvoice.LastUpdated > '10/8/2011'" & vbCrLf &
            " AND ISNULL(TransInvoice.DontAutoPrint, 0) = 0"

        pSql = "SELECT * FROM (" & pSql & ") t" & vbCrLf &
            " WHERE NumberOfPrints <= NumberOfReprints + ApprovedRequests" & vbCrLf &
            " ORDER BY LastUpdated"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function IncrementPrintCount(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate

        Try
            Try
                mProData.BeginTransaction()
            Catch ex As Exception
                Throw
            End Try

            pIU = New InsertUpdate
            pIU.TableName = "TransInvoice"
            pIU("NumberOfPrints") = "NumberOfPrints + 1"
            pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
            mProData.ExecuteSQL(pSql)

            pSql = "SELECT TOP 1 ID" & _
                " FROM TransInvoicePrints" & _
                " WHERE InvoiceID = " & ReadyString(vID) & _
                " AND ApprovedBy IS NOT NULL" & _
                " AND DatePrinted IS NULL" & _
                " AND DeniedBy IS NULL" & _
                " ORDER BY DateRequested"
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                pIU = New InsertUpdate
                pIU.TableName = "TransInvoicePrints"
                pIU("DatePrinted") = "GETDATE()"
#If _MYTYPE = "Web" Then
                pIU("PrintedBy") = WebSession.Instance.EmployeeID
#Else
                pIU("PrintedBy") = User.EmployeeID
#End If
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDT.Rows(0)("ID")))
                mProData.ExecuteSQL(pSql)
            End If
            mProData.CommitTransaction()
        Catch ex As Exception
            Throw
        End Try

        Return True

    End Function

    ' not used in Mobiz
    'Public Sub ReplaceItems(ByVal vInvoiceID As String, ByVal vSortingListID As String)

    '    Dim pDT As DataTable
    '    Dim pSql As String
    '    Dim pDR As DataRow
    '    Dim pError As String
    '    Dim pIU As InsertUpdate
    '    Dim pTotalAmount As Decimal
    '    Dim pCost As String
    '    Dim pGP, pGPM As Decimal

    '    pSql = "SELECT TransInvoice.InvoiceNumber" & vbCrLf & _
    '        ",TransInvoice.InvoiceNoExtension" & vbCrLf & _
    '        ",MainInvoiceDefaults.Abbrevation" & vbCrLf & _
    '        ",LibraryCustomers.CompanyName" & vbCrLf & _
    '        " FROM TransInvoice" & vbCrLf & _
    '        " LEFT JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & vbCrLf & _
    '        " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf & _
    '        " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID)
    '    pDT = mProData.Execute(pSql)
    '    If pDT.Rows.Count <> 1 Then Exit Sub
    '    pDR = pDT.Rows(0)

    '    mProData.BeginTransaction()

    '    '// delete all items 
    '    pSql = "DELETE FROM TransInvoiceItems WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '    mProData.ExecuteSQL(pSql)

    '    pDT = GetSortingListItems(vSortingListID, False)
    '    SaveStocks(vInvoiceID, pDT, pDR("Abbrevation") & "", pDR("InvoiceNumber") & "", pDR("InvoiceNoExtension") & "", pDR("CompanyName") & "", vSortingListID, pError)

    '    pTotalAmount = CDec2(pDT.Compute("SUM(Subtotal)", ""))
    '    pCost = CDec2(pDT.Compute("SUM(LineCost)", ""))

    '    pGP = pTotalAmount - pCost
    '    If pTotalAmount = 0 Then
    '        If pGP > 0 Then
    '            pGPM = 1
    '        ElseIf pGP < 0 Then
    '            pGPM = -1
    '        Else
    '            pGPM = 0
    '        End If
    '    Else
    '        pGPM = pGP / pTotalAmount
    '    End If
    '    pIU = New InsertUpdate("TransInvoice")
    '    pIU("TotalAmount") = pTotalAmount
    '    pIU("GP") = pGP
    '    pIU("GPM") = pGPM
    '    pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
    '    mProData.ExecuteSQL(pSql)

    '    mProData.CommitTransaction()

    'End Sub

#Region "QtyAmt"
    Private Const mDateFormat As String = "MMM yyyy"

    Public Function QtyAmtExcel(ByVal vDateFrom As Date, ByVal vDateTo As Date, ByVal vSupplierID As String, _
        ByVal vSupplierName As String, Optional ByVal vShowCustomer As Boolean = False) As Workbook

        '// get data
        Dim pQuantityColumns As List(Of DataColumn)
        Dim pAmountColumns As List(Of DataColumn)
        Dim pDates As List(Of Date)
        Dim pDT As DataTable
        Dim pDtBranches As DataTable
        Dim pWB As Workbook

        pWB = New Workbook

        pDT = GetData(vDateFrom, vDateTo, vSupplierID, pQuantityColumns, pAmountColumns, pDates, pDtBranches, vShowCustomer)

        '// generate excel
        Dim pWS As Worksheet
        Dim pDrBranch As DataRow
        Dim pRowIndex As Integer
        Dim p As Integer
        Dim pDtCustomers As DataTable
        Dim pDrvCustomer As DataRowView
        Dim pDV As DataView

        pWS = pWB.Worksheets(0)
        pDT.DefaultView.Sort = "StockDescription"

        pWS.Cells(pRowIndex, 0).PutValue("Amount and Quantity Sold by Supplier")
        pRowIndex += 1
        pWS.Cells(pRowIndex, 0).PutValue("Supplier:")
        pWS.Cells(pRowIndex, 1).PutValue(vSupplierName)

        pRowIndex += 1
        pWS.Cells(pRowIndex, 0).PutValue(Format(vDateFrom, cGlobal.FormatDate) & " to " & Format(vDateTo, cGlobal.FormatDate) & " Sales")

        For Each pDrBranch In pDtBranches.Rows
            pDT.DefaultView.RowFilter = "BranchID = " & pDrBranch("BranchID")
            If pDT.DefaultView.Count > 0 Then
                pRowIndex += 2
                pWS.Cells(pRowIndex, 0).PutValue("Branch:")
                pWS.Cells(pRowIndex, 1).PutValue(pDrBranch("Name"))

                '// merged headers
                pRowIndex += 1
                pWS.Cells(pRowIndex, 2).PutValue("Amount")
                pWS.Cells.Merge(pRowIndex, 2, 1, pDates.Count + 1)
                pWS.Cells(pRowIndex, 2 + pDates.Count + 1).PutValue("Quantity")
                pWS.Cells.Merge(pRowIndex, 2 + pDates.Count + 1, 1, pDates.Count + 1)

                '// headers
                pRowIndex += 1
                pWS.Cells(pRowIndex, 0).PutValue("Stock No.")
                pWS.Cells(pRowIndex, 1).PutValue("Stock Description")
                For p = 0 To pDates.Count - 1
                    pWS.Cells(pRowIndex, 2 + p).PutValue(Format(pDates(p), mDateFormat))
                    pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).PutValue(Format(pDates(p), mDateFormat))
                Next p
                pWS.Cells(pRowIndex, 2 + pDates.Count).PutValue("Total Amount")
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).PutValue("Total Quantity")

                If vShowCustomer Then
                    pDtCustomers = pDT.DefaultView.ToTable(True, "CompanyName")
                    pDtCustomers.DefaultView.Sort = "CompanyName"
                    For Each pDrvCustomer In pDtCustomers.DefaultView
                        pRowIndex += 1
                        pWS.Cells(pRowIndex, 0).PutValue("Customer:")
                        pWS.Cells(pRowIndex, 0).Style.Font.IsBold = True
                        pWS.Cells(pRowIndex, 1).PutValue(CStr2(pDrvCustomer("CompanyName")))
                        pWS.Cells(pRowIndex, 1).Style.Font.IsBold = True
                        pDV = New DataView(pDT, pDT.DefaultView.RowFilter & " AND ISNULL(CompanyName, '') = " & ReadyString(pDrvCustomer("CompanyName")), "StockDescription", DataViewRowState.CurrentRows)
                        RenderItems(pWS, pDV, pDates, pRowIndex)
                    Next pDrvCustomer
                Else
                    pDV = pDT.DefaultView
                    RenderItems(pWS, pDV, pDates, pRowIndex)
                End If

                '// branch totals
                pRowIndex += 1
                pWS.Cells(pRowIndex, 1).PutValue("Branch Total:")
                For p = 0 To pDates.Count - 1
                    If pDT.Columns.Contains(Format(pDates(p), mDateFormat) & " - Amount") Then
                        pWS.Cells(pRowIndex, 2 + p).PutValue(pDT.Compute("SUM([" & Format(pDates(p), mDateFormat) & " - Amount" & "])", "BranchID = " & pDrBranch("BranchID")))
                        pWS.Cells(pRowIndex, 2 + p).Style.Custom = cGlobal.FormatCurrencyGrid
                        pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).PutValue(pDT.Compute("SUM([" & Format(pDates(p), mDateFormat) & " - Quantity" & "])", "BranchID = " & pDrBranch("BranchID")))
                        pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).Style.Custom = cGlobal.FormatIntegerAccounting
                    End If
                Next p
                pWS.Cells(pRowIndex, 2 + pDates.Count).PutValue(pDT.Compute("SUM(TotalAmount)", "BranchID = " & pDrBranch("BranchID")))
                pWS.Cells(pRowIndex, 2 + pDates.Count).Style.Custom = cGlobal.FormatCurrencyGrid
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).PutValue(pDT.Compute("SUM(TotalQty)", "BranchID = " & pDrBranch("BranchID")))
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).Style.Custom = cGlobal.FormatIntegerAccounting
            End If
        Next pDrBranch

        pWS.AutoFitColumns()
        'pWB.Save(pFilename)

        Return pWB

        'If ShowMessage("The report was successfully saved as " & pFilename & ". Do you want to open it?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
        '    Try
        '        Waiter.Show("Opening file")
        '        Process.Start(pFilename)
        '        Waiter.Hide()
        '    Catch ex As Exception
        '        ShowMessage(ex.Message, "Error opening spreadsheet", , MessageBoxIcon.Exclamation)
        '    End Try
        'End If

    End Function

    Private Sub RenderItems(ByVal vWS As Worksheet, ByVal vDV As DataView, ByVal vDates As List(Of Date), ByRef rRowIndex As Integer)

        Dim pDRV As DataRowView
        Dim p As Integer

        For Each pDRV In vDV
            rRowIndex += 1
            vWS.Cells(rRowIndex, 0).PutValue(pDRV("StockNumber"))
            vWS.Cells(rRowIndex, 1).PutValue(pDRV("StockDescription"))
            For p = 0 To vDates.Count - 1
                If vDV.Table.Columns.Contains(Format(vDates(p), mDateFormat) & " - Amount") Then
                    vWS.Cells(rRowIndex, 2 + p).PutValue(pDRV(Format(vDates(p), mDateFormat) & " - Amount"))
                    vWS.Cells(rRowIndex, 2 + p).Style.Custom = cGlobal.FormatCurrencyGrid
                    vWS.Cells(rRowIndex, 2 + p + vDates.Count + 1).PutValue(pDRV(Format(vDates(p), mDateFormat) & " - Quantity"))
                    vWS.Cells(rRowIndex, 2 + p + vDates.Count + 1).Style.Custom = cGlobal.FormatIntegerAccounting
                End If
            Next p
            vWS.Cells(rRowIndex, 2 + vDates.Count).PutValue(pDRV("TotalAmount"))
            vWS.Cells(rRowIndex, 2 + vDates.Count).Style.Custom = cGlobal.FormatCurrencyGrid
            vWS.Cells(rRowIndex, 2 + vDates.Count * 2 + 1).PutValue(pDRV("TotalQty"))
            vWS.Cells(rRowIndex, 2 + vDates.Count * 2 + 1).Style.Custom = cGlobal.FormatIntegerAccounting
        Next pDRV

    End Sub

    Private Function GetData(ByVal vDateFrom As Date, ByVal vDateTo As Date, ByVal vSupplierID As String,
        ByRef oGeneratedQuantityColumns As List(Of DataColumn),
        ByRef oGeneratedAmountColumns As List(Of DataColumn),
        ByRef oDates As List(Of Date),
        ByRef oDtBranches As DataTable,
        Optional ByVal vShowCustomer As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pDtAll As DataTable
        Dim pDateFrom, pDateTo As Date
        Dim pDR As DataRow
        Dim p As Integer
        Dim pQtyColumns, pAmountColumns As CrossColumn
        Dim pFixedColumns As List(Of String)

        oDates = New List(Of Date)
        For p = 0 To DateDiff(DateInterval.Month, vDateFrom, vDateTo)
            pDateFrom = vDateFrom.AddMonths(p)
            pDateFrom = New Date(pDateFrom.Year, pDateFrom.Month, 1)
            If pDateFrom < vDateFrom Then pDateFrom = vDateFrom

            pDateTo = vDateFrom.AddMonths(p + 1)
            pDateTo = New Date(pDateTo.Year, pDateTo.Month, 1).AddMilliseconds(-1)
            If pDateTo > vDateTo Then pDateTo = vDateTo

#If _MyType = "Windows" _
    Or _MyType = "WindowsForms" Then
            Waiter.Show("Getting data from " & pDateFrom & " to " & pDateTo)
#End If
            oDates.Add(pDateTo)
            pDT = mParent.TransInvoice.GetSummaryPerSupplierAndBranch(pDateFrom, pDateTo, vSupplierID, vShowCustomer)
            pDT.Columns.Add("DisplayDate")
            For Each pDR In pDT.Rows
                pDR("DisplayDate") = Format(pDateTo, mDateFormat)
            Next pDR
            If pDtAll Is Nothing Then
                pDtAll = pDT
            Else
                pDtAll.Merge(pDT)
            End If
        Next p
#If _MyType = "Windows" _
    Or _MyType = "WindowsForms" Then
        Waiter.Hide(oDates.Count)
#End If

        pFixedColumns = New List(Of String)
        pFixedColumns.Add("StockNumber")
        pFixedColumns.Add("StockDescription")
        pFixedColumns.Add("BranchID")
        If vShowCustomer Then
            pFixedColumns.Add("CompanyName")
        End If

        pQtyColumns = New CrossColumn("{DisplayDate} - Quantity", "QtySold", "TotalQty")
        pAmountColumns = New CrossColumn("{DisplayDate} - Amount", "Amount", "TotalAmount")
        pDT = CreateCrossTab(pDtAll, pFixedColumns.ToArray,
            {
                pQtyColumns,
                pAmountColumns
            }
        )
        oGeneratedQuantityColumns = pQtyColumns.GeneratedColumns
        oGeneratedAmountColumns = pAmountColumns.GeneratedColumns

        '// insert branches
        oDtBranches = mParent.LibraryBranches.GetAll

        Return pDT

    End Function
#End Region

    Private Const InvoicesWithHeldTaxStart As String = "SELECT SUM(TransMemoTaxWithheldInvoices.TaxWithheldAmount) AS TaxWithheldAmount" & vbCrLf & _
        " ,TransInvoice.InvoiceNumber" & vbCrLf & _
        " ,TransInvoice.InvoiceDate" & vbCrLf & _
        " ,MAX(TransInvoice.TotalAmount) AS InvoiceAmount" & vbCrLf & _
        " ,TransMemo.MemoNumber" & vbCrLf & _
        " ,TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf & _
        " FROM TransMemoTaxWithheldInvoices" & vbCrLf & _
        " JOIN TransMemo ON TransMemo.ID = TransMemoTaxWithheldInvoices.MemoID" & vbCrLf & _
        " JOIN TransInvoice ON TransInvoice.ID = TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf

    Private Const InvoicesWithHeldTaxEnd As String = " GROUP BY TransInvoice.InvoiceNumber" & vbCrLf & _
        " ,TransInvoice.InvoiceDate" & vbCrLf & _
        " ,TransMemo.MemoNumber" & vbCrLf & _
        " ,TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf & _
        " " & vbCrLf & _
        " ORDER BY TransInvoice.InvoiceDate"

    Public Function InvoicesWithHeldTax(vCustomerID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart & _
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND  TransMemo.IsCancelled = 0" & vbCrLf & _
            " AND TransInvoice.CustomerId = " & ReadyString(vCustomerID) &
            " " & vbCrLf & _
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function InvoicesWithHeldTaxByCustomerPayment(vPaymentID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart & _
            " JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND TransCustomerPayment.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerPaymentInvoices.CustomerPaymentID = " & ReadyString(vPaymentID) &
            " " & vbCrLf & _
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function InvoicesWithHeldTaxByBillingStatement(vBillingStatementId As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart & _
            " JOIN TransCustomerBillingStatementInvoices ON TransCustomerBillingStatementInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN TransCustomerBillingStatement ON TransCustomerBillingStatement.ID = TransCustomerBillingStatementInvoices.BillingStatementID" & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND TransCustomerBillingStatement.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerBillingStatement.ID = " & ReadyString(vBillingStatementId) &
            " " & vbCrLf & _
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    'Public Sub SetHasForm2307(vID As String, vForm2306 As Boolean, vForm2307 As Boolean)

    '    Dim pIU As InsertUpdate
    '    Dim pSql As String

    '    pIU = New InsertUpdate("TransBirForms")
    '    pIU("ID") = "newid()"
    '    pIU("InvoiceID") = ReadyString(vID)
    '    pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '    pIU("DateCreated") = "GETDATE()"
    '    pIU("Form2307") = If(vForm2307, 1, 0)
    '    pIU("Form2306") = If(vForm2306, 1, 0)
    '    pSql = pIU.GetInsert
    '    mProData.ExecuteSQL(pSql)

    'End Sub

    ''' <summary>
    ''' returns true if a record was found
    ''' </summary>
    ''' <param name="vInvoiceDefaultID">MainInvoiceDefaults.ID</param>
    ''' <param name="oTransactionType">output MainInvoiceDefaults.TransactionType</param>
    ''' <param name="oAbbrevation">output MainInvoiceDefaults.Abbrevation</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInvoiceType(vInvoiceDefaultID As String, _
        ByRef oTransactionType As String, _
        ByRef oAbbrevation As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        oTransactionType = ""
        oAbbrevation = ""
        pSql = "SELECT MainInvoiceDefaults.TransactionType" & vbCrLf & _
            " ,MainInvoiceDefaults.Abbrevation" & vbCrLf & _
            " FROM MainInvoiceDefaults" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceDefaultID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            oTransactionType = pDT.Rows(0)("TransactionType")
            oAbbrevation = pDT.Rows(0)("Abbrevation")
            Return True
        End If

    End Function

    Public Function CustomerServiceLevel(vInvoiceDateFrom As Date, vInvoiceDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pCalendar As System.Globalization.GregorianCalendar

        pSql = "SELECT COUNT(*) AS Total" & vbCrLf & _
        " ,ISNULL(SUM(CASE WHEN TransInvoiceItems.IsHasBackOrder = 1 OR TransInvoiceItems.IsReplacement = 1 THEN 1 ELSE 0 END), 0) AS Imperfect" & vbCrLf & _
        " FROM TransInvoiceItems" & vbCrLf & _
        " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf & _
        " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
        " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vInvoiceDateFrom.Date) & " AND " & ReadyString(vInvoiceDateTo.Date) & vbCrLf

        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("DateRange")
        pDT.Columns.Add("WeekNum", GetType(Integer))

        pDT.Rows(0)("DateRange") = DisplayableDateRange(vInvoiceDateFrom, vInvoiceDateTo)
        pDT.Rows(0)("WeekNum") = New System.Globalization.GregorianCalendar().GetWeekOfYear(vInvoiceDateFrom, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday)

        Return pDT

    End Function

    Public Function CustomerServiceLevel(vInvoiceDateFrom As Date, vInvoiceDateTo As Date, vDurationType As eDurationType) As DataTable

        Dim pDT As DataTable
        Dim pDateRanges As List(Of DateRange)
        Dim pDateRange As DateRange
        Dim pOut As DataTable

        pDateRanges = GetDateRanges(vInvoiceDateFrom, vInvoiceDateTo, vDurationType, False)
        For Each pDateRange In pDateRanges
            pDT = CustomerServiceLevel(pDateRange.DateFrom, pDateRange.DateTo)
            If pOut Is Nothing Then
                pOut = pDT
            Else
                pOut.Merge(pDT)
            End If
        Next pDateRange
        pOut.Columns.Add("Perfect", GetType(Integer), "Total - Imperfect")

        Return pOut

    End Function

    Public Function IsValidCart(vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
            " AND TransInvoice.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT.Rows.Count > 0

    End Function

    Public Function GetCart(Optional vID As String = Nothing,
        Optional vUserID As String = Nothing, Optional vFowstID As String = Nothing) As DataSet

        Dim pSql As String
        Dim pInvoiceID As String ' TransInvoice.ID
        Dim pDS As DataSet

        pInvoiceID = vID
        If CStr2(pInvoiceID).Length = 0 Then
            pInvoiceID = GetCartID(vUserID, vFowstID)
            If CStr2(pInvoiceID).Length = 0 Then Return Nothing
        End If
        pSql = " WHERE TransInvoice.ID = " & ReadyString(pInvoiceID)
        pDS = GetReportData(pSql, False)
        pDS.Tables("Invoices").Columns("InvoiceID").ColumnName = "ID"

        Return pDS

    End Function

    Public Function GetCartID(vUserID As String, vFowstID As String) As String

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT TOP 1 TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
            " AND EXISTS(SELECT TransInvoiceItems.ID" & vbCrLf & _
            " 	FROM TransInvoiceItems" & vbCrLf & _
            "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)" & vbCrLf &
            " AND TransInvoice.UserID = " & ReadyString(vUserID) &
            " AND TransInvoice.FowstID = " & IIf(vFowstID.Trim().Length > 0, ReadyString(vFowstID), "NULL") &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

    End Function

    Public Function GetCartStock(vInvoiceID As String, vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SUM(TransInvoiceItems.Quantity) AS Quantity" & vbCrLf & _
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf & _
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf & _
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf & _
            " GROUP BY TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSalesReps(vInvoiceID As String, vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        'pSql = "SELECT DISTINCT TransInvoiceItems.ActualQuantity" & vbCrLf & _
        '    ",TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
        '    " ,ISNULL(MobizLibraryPeoplePreNominalSuffix.Code, '') + ' ' +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " & vbCrLf & _
        '    "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)), ', '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))), ' '), '') AS PeopleName" & vbCrLf & _
        '    " FROM TransInvoiceItems " & vbCrLf & _
        '    " JOIN TransInvoiceItemSalesReps ON TransInvoiceItemSalesReps.InvoiceItemID = TransInvoiceItems.ID" & vbCrLf & _
        '    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" & vbCrLf & _
        '    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf & _
        '    " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
        '    " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
        '    " ORDER BY TransInvoiceItems.ActualQuantity" & vbCrLf

        pSql = "SELECT DISTINCT " & vbCrLf & _
            " TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
            " ,ISNULL(MobizLibraryPeoplePreNominalSuffix.Code, '') + ' ' +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " & vbCrLf & _
            "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)), ', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))), ' '), '') AS PeopleName" & vbCrLf & _
            " FROM TransInvoiceItemSalesReps " & vbCrLf & _
            " INNER JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf & _
            " WHERE TransInvoiceItemSalesReps.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND TransInvoiceItemSalesReps.ProductID = " & ReadyString(vProductID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetProductSerial(vInvoiceID As String, vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT " & vbCrLf & _
            " prdserial.ID, " & vbCrLf &
            " prdserial.SerialNumber " & vbCrLf &
            " FROM TransInvoiceItemSerials prdserial " & vbCrLf & _
            " WHERE prdserial.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND prdserial.ProductID = " & ReadyString(vProductID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetMaxLineNumber(vOrderID As String) As Integer

        Dim pSql As String

        pSql = "SELECT MAX(TransInvoiceItems.LineNumber) AS LineNumber" & vbCrLf & _
            " FROM TransInvoiceItems" & vbCrLf & _
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vOrderID)

        Return CDec2(mProData.ExecuteScalar(pSql))

    End Function

    Public Sub DeleteOrderStock(vInvoiceID As String, vProductID As String)

        Dim pSql As String

        pSql = "DELETE t " & vbCrLf & _
            " FROM TransInvoiceItemSalesReps t" & vbCrLf & _
            " JOIN TransInvoiceItems ON TransInvoiceItems.ID = t.InvoiceItemID" & vbCrLf & _
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf & _
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM TransInvoiceItems" & vbCrLf & _
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf & _
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        mProData.ExecuteSQL(pSql)


    End Sub

#Region " invoice print "
#If _MyType = "Windows" _
    Or _MyType = "WindowsForms" Then

    Public Sub ShowPreview(vWhere As String)

        Dim pFrm As ProWin.ProReport
        Dim pDS As DataSet
        Dim pCustomerInvoiceDefaultID As String
        Dim pError As String

        If gBusiness.SystemDefaults.InvoiceTypeForCustomerAutoInvoice(pCustomerInvoiceDefaultID, "", pError) = False Then
            ShowMessage(pError)
            Exit Sub
        End If
        pDS = GetReportData("WHERE TransInvoice.InvoiceDefaultID = " & ReadyString(pCustomerInvoiceDefaultID) & " AND (" & vWhere & ")", False, True)

        pFrm = New Prowin.ProReport
        pFrm.AutoFillHeader = False
        pFrm.SourceFilename = "TransInvoice2"
        pFrm.Variables("CompanyAddress") = gBusiness.SystemDefaults.GetCompanyAddress
        pFrm.Variables("CompanyContactNumber") = gBusiness.SystemDefaults.GetCompanyContactNumbers
        pFrm.Variables("CompanyName") = gBusiness.SystemDefaults.GetCompanyName
        pFrm.Variables("CompanyTin") = "TIN: " & gBusiness.SystemDefaults.GetCompanyTin
        pFrm.Variables("CopyType") = "TRIPLICATE COPY" & vbCrLf & "(Company's Copy)"

#If DEBUG Then
        mProData.CreateTable("rptTransInvoice", pDS.Tables("Invoices"))
        mProData.CreateTable("rptTransInvoiceStocks", pDS.Tables("Stocks"))
        'mProData.CreateTable("rptTransInvoiceBackOrders", pDS.Tables("BackOrders"))
#End If

        Dim pDtInvoices As DataTable
        Dim pDtStocks As DataTable
        Dim pDtBackOrders As DataTable

        pDtInvoices = pDS.Tables("Invoices")
        pDS.Tables.Remove(pDtInvoices) ' have to remove from dataset or else Stimulsoft error

        pDtStocks = pDS.Tables("Stocks")
        pDS.Tables.Remove(pDtStocks)

        'pDtBackOrders = pDS.Tables("BackOrders")
        'pDS.Tables.Remove(pDtBackOrders)

        pFrm.AddTable("rptTransInvoice", pDtInvoices)
        pFrm.AddTable("rptTransInvoiceStocks", pDtStocks)
        'pFrm.AddTable("rptTransInvoiceBackOrders", pDtBackOrders)
        'pFrm.AddTable("rptTransInvoiceBackOrders", New DataTable)

        pFrm.PrintPreview(True)

    End Sub
#End If

    Public Function ComputeTotal(vID As String, _
        ByRef oGrossSrvAmount As Decimal, _
        ByRef oGrossRefundAmount As Decimal, _
        ByRef oTotalAdjustments As Decimal) As Decimal

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT SUM(SubTotal) AS SubTotal" &
            " FROM TransInvoiceItems" &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return 0
        End If
        ComputeTotal = CDec2(pDR("SubTotal"))

        pSql = "SELECT SUM(GrossSrvAmount) AS GrossSrvAmount" & vbCrLf &
            ",SUM(GrossRefundAmount) AS GrossRefundAmount" & vbCrLf &
            ",SUM(Fees) AS Fees" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE SourceInvoiceID = " & ReadyString(vID) & vbCrLf
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            oGrossSrvAmount = CDec2(pDR("GrossSrvAmount"))
            oGrossRefundAmount = CDec2(pDR("GrossRefundAmount"))
            oTotalAdjustments = oGrossSrvAmount + oGrossRefundAmount
        Else
            oGrossSrvAmount = 0
            oGrossRefundAmount = 0
            oTotalAdjustments = 0
        End If

    End Function

    Public Function GetReportData(ByVal vWhere As String,
        Optional ByVal vAddUnitExplaination As Boolean = True) As DataSet

        Dim pDS As DataSet
        Dim pDT As DataTable
        Dim pDtInvoice As DataTable
        Dim pSql As String

        pDS = New DataSet

        '// invoice ==============================================================================================================================================
        pSql = "SELECT TransInvoice.ID AS InvoiceID" & vbCrLf & _
            " ,MobizLibraryOrganization.CompanyName AS CustomerName" & vbCrLf & _
            " ,MobizLibraryOrganization.TinNumber" & vbCrLf & _
            " ,MobizLibraryOrganizationRepresentatives.LastName + ', ' + MobizLibraryOrganizationRepresentatives.FirstName + ' ' + MobizLibraryOrganizationRepresentatives.MiddleName AS Representative" & vbCrLf & _
            " ,ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '') + ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '') AS DeliveryAddress" & vbCrLf & _
            " ,RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS InvoiceNumber" & vbCrLf & _
            " ,TransInvoice.InvoiceDate" & vbCrLf & _
            " ,TransInvoice.Remarks" & vbCrLf & _
            " ,TransInvoice.CustomerID" & vbCrLf & _
            " ,TransInvoice.LastUpdated" & vbCrLf & _
            " ,TransInvoice.UserID" & vbCrLf & _
            " ,TransInvoice.FowstID" & vbCrLf & _
            " ,TransInvoice.RefNum" & vbCrLf & _
            " ,TransInvoice.ReasonForReconstructingData" & vbCrLf & _
            " ,TransInvoice.DeviceID" & vbCrLf & _
            " ,TransInvoice.SalesOrderDate" & vbCrLf & _
            " ,TransInvoice.SalesOrderNo" & vbCrLf & _
            " ,TransInvoice.SalesOrderUserID" & vbCrLf & _
            " ,TransInvoice.BillingAddressID" & vbCrLf & _
            " ,TransInvoice.DeliveryAddressID" & vbCrLf & _
            " ,MobizLibraryPeople.DUCNumericID" & vbCrLf & _
            "  FROM TransInvoice " & vbCrLf & _
            "  JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf & _
            "  LEFT JOIN MobizLibraryOrganizationRepresentatives ON TransInvoice.CusRepresentativeID = MobizLibraryOrganizationRepresentatives.ID " & vbCrLf & _
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf & _
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.BillingAddressID" & vbCrLf & _
            "  LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID " & vbCrLf & _
            "  LEFT JOIN MobizLibraryCountries ON MobizLibraryCountries.ID = MobizLibraryOrganizationFOWST.FowstCountryID " & vbCrLf & _
            "  " & vbCrLf & _
            vWhere & vbCrLf &
            "  ORDER BY TransInvoice.InvoiceNumber" & vbCrLf & _
            " " & vbCrLf
        pDtInvoice = mProData.Execute(pSql)
        pDtInvoice.TableName = "Invoices"
        pDS.Tables.Add(pDtInvoice)

        '// stocks ==============================================================================================================================================
        pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,TransInvoiceItems.Quantity" & vbCrLf & _
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf & _
            " ,CONVERT(varchar, TransInvoiceItems.TermDiscount) + '% ' + CONVERT(varchar, TransInvoiceItems.DiscountIfPaidWithIn) + ' NET ' + CONVERT(varchar, TransInvoiceItems.NetDue) AS Terms" & vbCrLf & _
            " ,TransInvoiceItems.UnitPrice" & vbCrLf & _
            " ,TransInvoiceItems.SubTotal" & vbCrLf & _
            " ,TransInvoiceItems.DiscountIfPaidWithIn" & vbCrLf & _
            " ,TransInvoiceItems.NetDue" & vbCrLf & _
            " ,TransInvoiceItems.TermNumericValue" & vbCrLf & _
            " ,TransInvoiceItems.TermDiscount AS TermDiscountNumeric" & vbCrLf & _
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf & _
            " ,TransInvoiceItems.ProductID" & vbCrLf & _
            " ,TransInvoiceItems.InvoiceID" & vbCrLf & _
            " ,TransInvoiceItems.LineNumber" & vbCrLf & _
            " ,TransInvoiceItems.PriceLevelID " & vbCrLf & _
            " ,TransInvoiceItems.LineCost " & vbCrLf & _
            " ,TransInvoiceItems.RequestedUnitPrice" & vbCrLf & _
            " FROM TransInvoiceItems " & vbCrLf & _
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf & _
            " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID  " & vbCrLf & _
            vWhere & " AND TransInvoiceItems.Quantity > 0"
        ' LEFT JOIN because TransModifiedInvoiceItems sometimes doesnt link to a valid StockID
        pDT = mProData.Execute(pSql)
        If vAddUnitExplaination Then
            pDT = AddUnitsExplanation(pDT)
        End If
        pDT.TableName = "Stocks"
        pDS.Tables.Add(pDT)

        pDtInvoice.Columns.Add("DueDate", GetType(Date))
        pDtInvoice.Columns.Add("Subtotal", GetType(Decimal))
        For Each pDR As DataRow In pDtInvoice.Rows
            'pDR("DueDate") = CDate(pDR("InvoiceDate")).AddDays(pDR("TermNumericValue")) ' not used in Mobiz
            pDR("SubTotal") = CDec2(pDT.Compute("SUM(SubTotal)", "InvoiceID = " & ReadyString(pDR("InvoiceID"))))
        Next pDR

        Return pDS

    End Function

    Private Function AddUnitsExplanation(ByVal vDT As DataTable) As DataTable
        ' PARAMS:
        '   vDT = vwReportStockPickingList

        Dim pStr As String
        Dim pBaseUnit As String
        Dim pDR As DataRow

        If vDT.Rows.Count = 0 Then Return vDT

        For Each pDR In vDT.Rows
            pStr = UnitExplanation(CStr2(pDR("StockID")), pDR("ActualQuantity"), pDR("UnitMeasure"), pBaseUnit)
            If pStr <> "" Then
                pStr = " [" & pStr & "]"
                pStr = pStr & " [" & pDR("ActualQuantity") * pDR("Quantity") & " " & pBaseUnit & " total]"
                If vDT.Columns.Contains("Stock Description") Then
                    pDR("Stock Description") = pDR("Stock Description") & pStr
                Else
                    pDR("StockDescription") = pDR("StockDescription") & pStr
                End If
            End If
        Next pDR

        Return vDT

    End Function

    Private Function UnitExplanation(ByVal vStockID As String, _
        ByVal vActualQuantity As Integer, _
        ByVal vUnit As String, _
        Optional ByRef oBaseUnit As String = Nothing) As String

        Dim pUnit As String

        If vActualQuantity = 1 Then Exit Function
        pUnit = BaseUnit(vStockID)
        If pUnit <> "" Then
            oBaseUnit = pUnit
            UnitExplanation = vActualQuantity & " " & pUnit & " / " & vUnit
        End If

    End Function

    Private Function BaseUnit(ByVal vStockID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryUnitMeasures.UnitMeasure" & _
            " ,LibraryStockMeasurements.Quantity" & _
            " FROM LibraryStockMeasurements" & _
            " JOIN LibraryUnitMeasures" & _
            " ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & _
            " WHERE StockID = " & ReadyString(vStockID) & _
            " AND LibraryStockMeasurements.Quantity = 1"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            BaseUnit = pDT.Rows(0)("UnitMeasure")
        End If

    End Function
#End Region

    Public Function SaveRemarks(vID As String, vRemarks As String) As Boolean

        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("TransInvoice")
        pIU("Remarks") = ReadyString(vRemarks)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pSql = mProData.ExecuteSQL(pSql, True)

        Return CDec(pSql) > 0

    End Function

    Public Sub UpdateAmounts(vID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pGrossSrvAmount As Decimal
        Dim pGrossRefundAmount As Decimal
        Dim pTotalAdjustments As Decimal

        pIU = New InsertUpdate("TransInvoice")
        pIU("TotalAmount") = mParent.TransInvoice.ComputeTotal(vID, pGrossSrvAmount, pGrossRefundAmount, pTotalAdjustments)
        pIU("GrossSrvAmount") = pGrossSrvAmount
        pIU("GrossRefundAmount") = pGrossRefundAmount
        pIU("TotalAdjustments") = pTotalAdjustments
        pIU("NetInvoiceAmount") = CDec2(pIU("TotalAmount")) + CDec2(pIU("TotalAdjustments"))
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub AddToInventoryLedger(vID As String)

        Dim pDS As DataSet
        Dim pDtInvoice As DataTable
        Dim pDtStocks As DataTable
        Dim pDrInvoice As DataRow
        Dim pDtStocksUnique As DataTable

        pDS = GetCart(vID)
        pDtInvoice = pDS.Tables("Invoices")
        If pDtInvoice.Rows.Count = 0 Then
            Exit Sub
        End If
        pDrInvoice = pDtInvoice.Rows(0)
        pDtStocks = pDS.Tables("Stocks")
        pDtStocks.Columns.Add("TotalQty", GetType(Decimal), "Quantity * ActualQuantity")
        pDtStocksUnique = New DataView(pDtStocks, "TotalQty > 0", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID"})
        mProData.BeginTransaction()
        For Each pDR As DataRow In pDtStocksUnique.Rows
            mParent.TransInventoryLedger.Add( _
                CStr2(pDR("ProductID")), _
                CStr2(pDrInvoice("FowstID")), _
                vID, _
                TransInventoryLedger.eTransType.Invoice, _
                CStr2(pDrInvoice("UserID")),
                -(pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDR("ProductID"))))
            )
        Next pDR
        mProData.CommitTransaction()

    End Sub

    Public Function CartSummary(vID As String) As CartSummaryClass

        Dim pOut As CartSummaryClass
        Dim pDtStocks As DataTable

        pOut = New CartSummaryClass

        pDtStocks = GetCart(vID).Tables("Stocks")
        pOut.Qty = pDtStocks.Compute("SUM(Quantity)", "")
        pOut.Amount = pDtStocks.Compute("SUM(SubTotal)", "")
        pDtStocks = New DataView(pDtStocks, "", "", DataViewRowState.CurrentRows).ToTable(True, "ProductID")
        pOut.SkuCount = pDtStocks.Rows.Count

        Return pOut

    End Function

    Public Class CartSummaryClass

        Public Qty As Decimal
        Public SkuCount As Integer
        Private mAmount As Decimal

        Public Property Amount As Decimal
            Get

                Return mAmount

            End Get
            Set(value As Decimal)

                mAmount = Math.Round(value, 2)

            End Set
        End Property

    End Class

    Public Function GetMaxDate(vFowstID As String, vDeviceID As String,
        Optional vExcludeID As String = Nothing) As Date?

        Dim pDR As DataRow
        Dim pSql As String

        If CStr2(vExcludeID).Length > 0 Then
            pSql = " AND TransInvoice.ID <> " & ReadyString(vExcludeID)
        End If
        pSql = "SELECT MAX(LastUpdated)" & vbCrLf & _
            " FROM TransInvoice" & vbCrLf & _
            " WHERE TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf & _
            " AND CONVERT(varchar(36), TransInvoice.DeviceID) = " & ReadyString(vDeviceID) & vbCrLf &
            pSql
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR(0)
        End If

    End Function

    Public Function IsValidAdjustedDate(vDate As Date, vFowstID As String, vDeviceID As String, ByRef oError As String, _
        Optional vID As String = Nothing) As Boolean

        Dim pDate As Date?

        oError = ""
        If vDate > mProData.GetDate Then
            oError = "Date must not be later than the current date and time."
            Return False
        End If

        'pDate = GetMaxDate(vFowstID, vDeviceID, vID)
        'If pDate IsNot Nothing Then
        '    If vDate < pDate Then
        '        oError = "Date Time must not be earlier than latest transaction date"
        '        Return False
        '    End If
        'End If

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns>number of updated records</returns>
    ''' <remarks></remarks>
    Public Function UpdatePaymentDate(vID As String) As Integer


    End Function

    Public Function VerifyCounter(vDeviceID As String, vFowstID As String, ByRef oCounter As Integer) As Boolean

        Dim pSql As String
        Dim pMax As Integer

        pSql = "SELECT MAX(InvoiceNumber)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        pMax = CDec2(mProData.ExecuteScalar(pSql))

        pSql = "SELECT POSLastORNo" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        oCounter = CDec2(mProData.ExecuteScalar(pSql))

        If pMax <= oCounter Then
            Return True
        Else
            oCounter = pMax ' TODO: verify business logic here
        End If

    End Function

    Public Sub UpdateCounter(vDeviceID As String, vFowstID As String, ByVal vValue As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vValue <= 0 Then Return
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("POSLastORNo") = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ISNULL(POSLastORNo, 0) < " & vValue)
        pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)

    End Sub

    Public Sub UpdateSoCounter(vDeviceID As String, vFowstID As String, ByVal vValue As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vValue <= 0 Then Return
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("POSLastSONo") = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ISNULL(POSLastSONo, 0) < " & vValue)
        pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)

    End Sub

    Public Function GetSummary(vFowstID As String, vDeviceID As String, LastUpdatedDate As Date) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.UnitPrice) AS ItemsSellingAmount" & vbCrLf &
            " ,SUM(TransInvoiceItems.SubTotal) AS SellingAmount" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,CONVERT(date, TransInvoice.LastUpdated) AS LastUpdatedDate" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " WHERE CONVERT(date, TransInvoice.LastUpdated) = CONVERT(date, getdate())" & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransInvoice.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND CONVERT(date, TransInvoice.LastUpdated) = " & ReadyString(LastUpdatedDate.Date) & vbCrLf &
            " AND TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.InvoiceNumber <> 0" & vbCrLf &
            " GROUP BY TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,CONVERT(date, TransInvoice.LastUpdated)" & vbCrLf &
            " ORDER BY CONVERT(date, TransInvoice.LastUpdated)"
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetInfoByNumberAndTypeID(vInvoiceNumber As Integer, vInvoiceDefaultID As String) As DataRow

        Dim pSql = " -- GetInfoByID" & vbCrLf &
            " SELECT TOP 1 TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,TransInvoice.InvoiceNumber" & vbCrLf &
            " ,TransInvoice.TotalAmount" & vbCrLf &
            " ,LibraryCustomers.CompanyName" & vbCrLf &
            " ,TransInvoice.InvoiceDefaultID" & vbCrLf &
            " ,MainInvoiceDefaults.Abbrevation" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf &
            " LEFT JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber = " & vInvoiceNumber & vbCrLf &
            " AND TransInvoice.InvoiceDefaultID = " & ReadyString(vInvoiceDefaultID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class
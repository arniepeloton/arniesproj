Public Class TransQr

    Dim mProData As ProData
    Dim mParent As Business
    ''' <summary>
    ''' look in POS and CS that have this date or newer
    ''' </summary>
    ''' <remarks></remarks>
    Private Const mStartDate As Date = #10/5/2011#

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Private Function GetStocks(ByVal vBranchID As Integer, ByVal vDateFrom As Date, ByVal vDateTo As Date _
        , Optional ByVal vMachineNumsToQr As String = Nothing) As DataTable

        Dim pSql As String

        pSql = GetStockSql(vBranchID, vDateFrom, vDateTo, vMachineNumsToQr)

        Return mProData.Execute(pSql)

    End Function

    Private Function GetStocksSummary(ByVal vBranchID As Integer, ByVal vDateFrom As Date, ByVal vDateTo As Date _
        , Optional ByVal vMachineNumsToQr As String = Nothing) As DataTable

        Dim pSql As String

        pSql = GetStockSql(vBranchID, vDateFrom, vDateTo, vMachineNumsToQr)

        pSql = "SELECT StockID" & _
            " ,SUM(QtySold) AS QtySold" & _
            " ,BranchID" & _
            " ,SUM(QtyServed) AS QtyServed" & _
            " ,SUM(QtyToServe) AS QtyToServe" & _
            ",WarehouseID" &
            " " & _
            " FROM " & _
            " (" & _
            pSql & _
            " ) t" & _
            " " & _
            " GROUP BY StockID" & _
            ",WarehouseID" &
            ",BranchID"

        Return mProData.Execute(pSql)

    End Function

    ''' <summary>
    ''' generate common sql used only by GetStocks and GetStocksSummary
    ''' </summary>
    ''' <param name="vBranchID"></param>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStockSql(ByVal vBranchID As Integer, ByVal vDateFrom As Date, ByVal vDateTo As Date, Optional ByVal vMachineNumsToQr As String = Nothing) As String

        Dim pSql As String
        Dim pStrs As String()
        Dim pStr As String
     
        pSql = ""
        If CStr2(vMachineNumsToQr).Trim.Length > 0 Then
            pStrs = vMachineNumsToQr.Split({","}, StringSplitOptions.RemoveEmptyEntries)
            For Each pStr In pStrs
                If pSql.Length > 0 Then pSql &= ","
                pSql &= ReadyString(pStr.Trim)
            Next pStr
            pSql = " AND LTRIM(RTRIM(MachineNumber)) IN (" & pSql & ")"
        End If

        pSql = "SELECT POSSalesItems.ID" & _
            ",POSSalesItems.StockID" & _
            ",POSSalesItems.ActualQuantity * POSSalesItems.Quantity AS QtySold" & _
            ",POSSales.BranchID" & _
            ",ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyServed" & _
            ",POSSalesItems.ActualQuantity * POSSalesItems.Quantity - ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyToServe" & _
            ",0 AS FromCashSlip" & _
            ",0 AS SourceType" & _
            ",POSSales.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " FROM POSSalesItems" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" &
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & _
            " LEFT JOIN TransQrPos ON TransQrPos.PosSalesItemID = POSSalesItems.ID" & _
            " AND TransQrPos.FromCashSlip = 0" & _
            " " & _
            " WHERE ISNULL(POSSales.IsReplenished, 0) = 0" & _
            " AND POSSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " AND POSSales.BranchID = " & vBranchID & _
            pSql &
            " " & _
            " GROUP BY POSSalesItems.StockID" & _
            ",POSSales.BranchID" & _
            ",POSSalesItems.ActualQuantity" & _
            ",POSSalesItems.Quantity" & _
            ",POSSalesItems.ID" & _
            ",POSSales.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " " & _
            " HAVING POSSalesItems.ActualQuantity * POSSalesItems.Quantity > ISNULL(SUM(TransQrPos.QtyServed), 0)" & _
            " " & _
            " UNION " & _
            " " & _
            " SELECT POSCashSlipSalesItems.ID" & _
            ",POSCashSlipSalesItems.StockID" & _
            ",POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity AS QtySold" & _
            ",POSCashSlipSales.BranchID" & _
            ",ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyServed" & _
            ",POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity - ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyToServe" & _
            ",1 AS FromCashSlip" & _
            ",1 AS SourceType" & _
            ",POSCashSlipSales.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " " & _
            " FROM POSCashSlipSalesItems" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" &
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" & _
            " LEFT JOIN TransQrPos ON TransQrPos.PosSalesItemID = POSCashSlipSalesItems.ID" & _
            " AND TransQrPos.FromCashSlip = 1" & _
            " " & _
            " WHERE ISNULL(POSCashSlipSales.IsReplenished, 0) = 0" & _
            " AND POSCashSlipSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " AND POSCashSlipSales.BranchID = " & vBranchID & _
            pSql &
            " " & _
            " GROUP BY POSCashSlipSalesItems.StockID" & _
            ",POSCashSlipSales.BranchID" & _
            ",POSCashSlipSalesItems.ActualQuantity" & _
            ",POSCashSlipSalesItems.Quantity" & _
            ",POSCashSlipSalesItems.ID" & _
            ",POSCashSlipSales.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " " & _
            " HAVING POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity > ISNULL(SUM(TransQrPos.QtyServed), 0)" &
            "" &
            " UNION " &
            " SELECT TransInvoiceItems.ID" &
            ",TransInvoiceItems.StockID" &
            ",TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity AS QtySold" &
            ",TransSalesOrder.InStoreBranchID AS BranchID" &
            ",ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyServed" &
            ",TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity - ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyToServe" &
            ",0 AS FromCashSlip" & _
            ",2 AS SourceType" & _
            ",TransInvoice.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " " & _
            " FROM TransInvoiceItems" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" &
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" & _
            " LEFT JOIN TransQrPos ON TransQrPos.PosSalesItemID = TransInvoiceItems.ID" & _
            " AND TransQrPos.SourceType = 2" & _
            " " & _
            " WHERE TransSalesOrder.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " AND TransSalesOrder.InStoreBranchID = " & vBranchID & _
            " " & _
            " GROUP BY TransInvoiceItems.StockID" & _
            ",TransSalesOrder.InStoreBranchID" & _
            ",TransInvoiceItems.ActualQuantity" & _
            ",TransInvoiceItems.Quantity" & _
            ",TransInvoiceItems.ID" & _
            ",TransInvoice.LastUpdated" & _
            ",LibraryStockInformation.WarehouseID" & _
            " " & _
            " HAVING TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity > ISNULL(SUM(TransQrPos.QtyServed), 0)"

        Dim pPOSInvoiceDefaultID As String

        pPOSInvoiceDefaultID = CStr2(mParent.SystemDefaults.POSInvoiceDefaultID())
        If IsGuid(pPOSInvoiceDefaultID) Then
            pSql = pSql & " UNION" & vbCrLf & _
                " " & vbCrLf & _
                " SELECT TransInvoiceItems.ID" & vbCrLf & _
                " ,TransInvoiceItems.StockID" & vbCrLf & _
                " ,TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity AS QtySold" & vbCrLf & _
                " ,TransInvoice.BranchID" & vbCrLf & _
                " ,ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyServed" & vbCrLf & _
                " ,TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity " & vbCrLf & _
                " - ISNULL(SUM(TransQrPos.QtyServed), 0) AS QtyToServe" & vbCrLf & _
                " ,0 AS FromCashSlip" & vbCrLf & _
                " ,3 AS SourceType" & vbCrLf & _
                " ,TransInvoice.LastUpdated" & vbCrLf & _
                " ,LibraryStockInformation.WarehouseID" & vbCrLf & _
                " FROM TransInvoiceItems" & vbCrLf & _
                " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" & vbCrLf & _
                " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf & _
                " LEFT JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" & vbCrLf & _
                " LEFT JOIN TransQrPos ON TransQrPos.PosSalesItemID = TransInvoiceItems.ID" & vbCrLf & _
                " AND TransQrPos.SourceType = 3" & vbCrLf & _
                " WHERE TransInvoice.LastUpdated BETWEEN  " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
                " AND TransInvoice.InvoiceDefaultID = " & ReadyString(pPOSInvoiceDefaultID) & vbCrLf & _
                " AND TransInvoice.IsCancelled = 0" & vbCrLf & _
                " AND TransInvoice.InvoiceNumber <> 0" & vbCrLf & _
                " AND TransInvoice.BranchID = " & vBranchID & _
                " GROUP BY TransInvoiceItems.StockID" & vbCrLf & _
                " ,TransInvoiceItems.ActualQuantity" & vbCrLf & _
                " ,TransInvoiceItems.Quantity" & vbCrLf & _
                " ,TransInvoiceItems.ID" & vbCrLf & _
                " ,TransInvoice.LastUpdated" & vbCrLf & _
                " ,LibraryStockInformation.WarehouseID" & vbCrLf & _
                " ,TransInvoice.BranchID" & vbCrLf & _
                " HAVING TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity > ISNULL(SUM(TransQrPos.QtyServed), 0)" & vbCrLf
        End If

        Return pSql

    End Function

    Public Function GetStocks(ByVal vBranchID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date, _
        ByVal vID As String, ByVal vEditable As Boolean) As DataTable

        Dim pSql As String
        Dim pSqlCommon As String

        pSqlCommon = " SELECT TransQrModifiedStocks.ID" & _
            ",TransQrModifiedStocks.StockID" & _
            ",LibraryStockInformation.StockNumber" & _
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & _
            " ,TransQrModifiedStocks.QtySold" & _
            " ,TransQrModifiedStocks.UnitQty" & _
            " ,TransQrModifiedStocks.UnitMeasure AS UnitMeasure2" & _
            " ,TransQrModifiedStocks.ModifiedQty" & _
            ",LibraryBranch.Name AS BranchName" & _
            ",LibraryBranch.ID AS BranchID" & _
            ",tEmp.LastName + ', ' + tEmp.FirstName + ' ' + tEmp.MiddleName AS EditedBy" & _
            " FROM TransQrModifiedStocks " & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransQrModifiedStocks.StockID"
        If CStr2(vID).Length > 0 Then pSqlCommon &= " AND TransQrModifiedStocks.QrModifiedID = " & ReadyString(vID)
        pSqlCommon &= " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & _
            " JOIN TransQrModified ON TransQrModified.ID = TransQrModifiedStocks.QrModifiedID" & _
            " JOIN LibraryBranches ON LibraryBranches.ID = TransQrModified.BranchID" & _
            " JOIN DavaoUnifiedPayroll.dbo.LibraryBranch LibraryBranch ON LibraryBranch.ID = LibraryBranches.BranchID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tEmp ON tEmp.ID = TransQrModified.UserID"

        If vEditable Then
            pSql = "SELECT CONVERT(uniqueidentifier, NULL) AS ID" & _
                ",vwReportPOSStockSales.StockID" & _
                ",StockNumber" & _
                ",StockDescription" & _
                ",vwReportPOSStockSales.Quantity2 AS QtySold" & _
                ",vwReportPOSStockSales.ActualQuantity AS UnitQty" & _
                ",vwReportPOSStockSales.UnitMeasure2"
            'If CStr2(vID).Length > 0 Then
            '    pSql &= ",ISNULL(TransQrModifiedStocks.ModifiedQty, vwReportPOSStockSales.Quantity2) AS ModifiedQty"
            'Else
            pSql &= ",vwReportPOSStockSales.Quantity2 AS ModifiedQty"
            'End If
            pSql &= ",vwReportPOSStockSales.BranchName" & _
                ",vwReportPOSStockSales.BranchID" & _
                ",CONVERT(varchar, NULL) AS EditedBy"
            pSql &= " FROM vwReportPOSStockSales" & _
                " JOIN LibraryBranches ON LibraryBranches.BranchID = vwReportPOSStockSales.BranchID" & _
                " AND LibraryBranches.ID = " & ReadyString(vBranchID) & _
                " AND vwReportPOSStockSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo)
            pSql &= " AND vwReportPOSStockSales.QRSalesOrderID IS NULL"

            If CStr2(vID).Length > 0 Then
                pSql &= " LEFT JOIN TransQrModifiedStocks ON TransQrModifiedStocks.StockID = vwReportPOSStockSales.StockID" & _
                    " AND vwReportPOSStockSales.ActualQuantity = TransQrModifiedStocks.UnitQty" & _
                    " AND vwReportPOSStockSales.UnitMeasure2 = TransQrModifiedStocks.UnitMeasure" & _
                    " AND TransQrModifiedStocks.QrModifiedID = " & ReadyString(vID) & _
                    " AND vwReportPOSStockSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
                    " WHERE TransQrModifiedStocks.ID IS NULL"

                pSql &= " UNION" & _
                    " " & _
                    pSqlCommon & _
                        " LEFT JOIN vwReportPOSStockSales ON TransQrModifiedStocks.StockID = vwReportPOSStockSales.StockID" & _
                        " 	AND vwReportPOSStockSales.ActualQuantity = TransQrModifiedStocks.UnitQty " & _
                        "   AND vwReportPOSStockSales.UnitMeasure = TransQrModifiedStocks.UnitMeasure" & _
                        "   AND vwReportPOSStockSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
                        "   AND vwReportPOSStockSales.QRSalesOrderID IS NULL" & _
                        " WHERE vwReportPOSStockSales.StockID IS NULL" & _
                        " AND LibraryBranches.ID = " & ReadyString(vBranchID)
            End If
        Else
            pSql = pSqlCommon
        End If

        pSql = "SELECT ID" & _
            ",StockID" & _
            ",StockNumber" & _
            ",StockDescription" & _
            ",UnitQty" & _
            ",UnitMeasure2 AS UnitMeasure" & _
            ",CONVERT(int, ISNULL(SUM(QtySold), 0)) AS QtySold" & _
            ",CONVERT(int, ISNULL(SUM(ModifiedQty), 0)) AS ModifiedQty" & _
            ",BranchName" & _
            ",BranchID" & _
            ",EditedBy" & _
            " FROM (" & pSql & ") t" & _
            " GROUP BY ID" & _
            ",StockID" & _
            ",StockNumber" & _
            ",StockDescription" & _
            ",UnitQty" & _
            ",BranchName" & _
            ",BranchID" & _
            ",EditedBy" & _
            ",UnitMeasure2"

        Return mProData.Execute(pSql)

    End Function

    Public Function ByBranchAndTime(ByVal vBranchID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransQrModified.ID" & _
            ",TransQrModified.Num" & _
            "," & mProData.Dbo & ".FormatBranchTrans(LibraryBranches.BranchID, TransQrModified.Num) AS NumText" & _
            ",TransQrModified.LastUpdated" & _
            " FROM TransQrModified" & _
            " JOIN LibraryBranches ON LibraryBranches.ID = TransQrModified.BranchID" & _
            " WHERE TransQrModified.BranchID = " & ReadyString(vBranchID) & _
            " AND DateFrom = " & ReadyString(vDateFrom) & _
            " AND DateTo = " & ReadyString(vDateTo)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function LatestActive(ByVal vBranchId As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID" & _
            " FROM TransQrModified" & _
            " WHERE BranchID = " & ReadyString(vBranchId) & _
            " AND SubmissionDate < GETDATE()" & _
            " AND QRSalesOrderID IS NULL" & _
            " AND IsCancelled = 0" & _
            " ORDER BY SubmissionDate DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

#If ProjName = "ERP" Then
    Public Sub Preview(Optional ByVal vDateTo As String = Nothing, Optional ByVal vRemoveFirstUnits As Boolean = True)

        Dim pFrm As ProWin.ProReport
        Dim pDT As DataTable

        Waiter.Show()
        pDT = GetItems(Nothing, , vDateTo, vRemoveFirstUnits)
        If pDT Is Nothing Then
            ShowMessage("No items to view", "Nothing", , MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        pDT = AddStockColumns(pDT)
        pDT = AddBranchColumn(pDT)

        If pDT IsNot Nothing Then
            'TODO: fix this pre-compilation directive
#If DEBUG Then
            mProData.CreateTable("rptPOSStockSales", pDT)
#End If
            pFrm = New ProWin.ProReport
            pFrm.SourceFilename = "SumPOSStockSales"
            pFrm.AddTable("rptPOSStockSales", pDT)
            pFrm.PrintPreview()
        End If
        Waiter.Hide()

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oDtBranches"></param>
    ''' <param name="oDtStockDetails">If this is not nothing, this will return GetStocks. If nothing, this will return nothing</param>
    ''' <param name="vDateTo">If nothing, auto-determine vDateTo base on branch schedule. If set to date, vDateTo to that date</param>
    ''' <returns>output from GetStocksSummary</returns>
    ''' <remarks></remarks>
    Public Function GetItems(ByRef oDtBranches As DataTable, Optional ByRef oDtStockDetails As DataTable = Nothing, _
        Optional ByVal vDateTo As String = Nothing, _
        Optional ByVal vRemoveFirstUnits As Boolean = True) As DataTable

        Dim pDrBranch As DataRow
        Dim pDvSched As DataView
        Dim pDtItems As DataTable
        Dim pDtItemsAll As DataTable
        Dim pDateTo As Date
        Dim pDR As DataRow
        Dim pId As String
        Dim pBranchID As Integer
        Dim pDtDetails, pDtDetailsAll As DataTable

        pBranchID = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 5)
        oDtBranches = mParent.LibraryBranches.GetWithoutSchedule(pBranchID)
        If oDtBranches.Rows.Count > 0 Then
            ShowMessage("The following branches has no schedule:" & vbCrLf & ExtractFieldValues(oDtBranches, "Name").Rows(0)(0), "No schedule", , MessageBoxIcon.Exclamation)
            Exit Function
        End If

        oDtBranches = mParent.LibraryBranches.GetAll(pBranchID)
        oDtBranches.Columns.Add("DateTo", GetType(Date))
        oDtBranches.Columns.Add("SubmissionDate", GetType(Date))
        oDtBranches.Columns.Add("LastestQrModifiedID")
        For Each pDrBranch In oDtBranches.Rows
            If vDateTo Is Nothing Then
                pDvSched = mParent.LibraryBranches.GetModifiedQr(CStr2(pDrBranch("ID")))
            Else
                pDateTo = vDateTo
            End If
            If pDvSched IsNot Nothing _
                OrElse vDateTo IsNot Nothing Then

                If vDateTo Is Nothing Then
                    If pDvSched.Count > 0 Then
                        pDateTo = pDvSched(0)("DateTo")
                        pDrBranch("SubmissionDate") = pDvSched(0)("SubmissionDate")
                    Else
                        pDateTo = Now
                    End If
                Else
                    pDateTo = vDateTo
                    pDrBranch("SubmissionDate") = vDateTo
                End If

                pDrBranch("DateTo") = pDateTo
                pDR = mParent.TransQr.LatestActive(CStr2(pDrBranch("ID")))
                If pDR IsNot Nothing Then
                    pId = CStr2(pDR("ID"))
                    pDrBranch("LastestQrModifiedID") = pId
                Else
                    pId = Nothing
                End If

                pDtItems = GetStocksSummary(pDrBranch("BranchID"), mStartDate, pDateTo, CStr2(pDrBranch("MachineNumsToQr")))
                If pDtItemsAll Is Nothing Then
                    pDtItemsAll = pDtItems
                Else
                    pDtItemsAll.Merge(pDtItems)
                End If

                If oDtStockDetails IsNot Nothing Then
                    pDtDetails = GetStocks(pDrBranch("BranchID"), mStartDate, pDateTo, CStr2(pDrBranch("MachineNumsToQr")))
                    If pDtDetailsAll Is Nothing Then
                        pDtDetailsAll = pDtDetails
                    Else
                        pDtDetailsAll.Merge(pDtDetails)
                    End If
                End If
            End If
        Next pDrBranch

        If pDtItemsAll IsNot Nothing Then
            pDtItemsAll.Columns.Add("UnitMeasure")
            pDtItemsAll.Columns.Add("ActualQuantity", GetType(Integer))
            pDtItemsAll.Columns.Add("UnitMeasure2", GetType(String), "ISNULL(UnitMeasure + ' ', '') + '[' + ActualQuantity + ']'")
            If vRemoveFirstUnits Then
                pDtItems = RemoveFirstUnitMeasure(pDtItemsAll)
            Else
                pDtItems = pDtItemsAll
            End If
            pDtItems.Columns("QtyToServe").ColumnName = "Quantity"
            pDtItems.Columns.Add("Editedby")
        End If

        If oDtStockDetails IsNot Nothing Then
            oDtStockDetails = pDtDetailsAll
        End If

        Return pDtItems

    End Function

    ''' <summary>
    ''' Remove the 1st unit measure
    ''' </summary>
    ''' <param name="vDtItems">from GetItems</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RemoveFirstUnitMeasure(ByVal vDtItems As DataTable) As DataTable

        Dim pDrItem As DataRow
        Dim pDtUnits As DataTable
        Dim pDrvUnit As DataRowView
        Dim pQtyToServe As Integer
        Dim pQtyLeftToServe As Integer
        Dim pDtAdd As DataTable ' add this to the table
        Dim pDrAdd As DataRow
        Dim pDrs As DataRow()
        Dim p As Integer
        Dim pDvUnits As DataView

        pDtAdd = vDtItems.Clone
        For Each pDrItem In vDtItems.Rows
            pDtUnits = mParent.LibraryStock.Measurements(CStr2(pDrItem("StockID")))
            '// remove multiple "1 quantities". leave only 1 (if any) 
            pDtUnits.DefaultView.RowFilter = "Quantity = 1"
            Do Until pDtUnits.DefaultView.Count <= 1
                pDtUnits.DefaultView(0).Delete()
            Loop
            pDtUnits.AcceptChanges()
            pDtUnits.DefaultView.RowFilter = ""

            pQtyLeftToServe = pDrItem("QtyToServe")
            pDtUnits.DefaultView.Sort = "Quantity DESC" ' start with the biggest unit measure

            For Each pDrvUnit In pDtUnits.DefaultView
                If Not (pDrvUnit("Quantity") = 1 _
                    AndAlso pDtUnits.Rows.Count > 1) Then

                    pQtyToServe = Math.Floor(pQtyLeftToServe / pDrvUnit("Quantity"))
                    If pQtyToServe > 0 Then
                        pDrAdd = pDtAdd.NewRow
                        pDrAdd("StockID") = pDrItem("StockID")
                        pDrAdd("QtyToServe") = pQtyToServe
                        pDrAdd("ActualQuantity") = pDrvUnit("Quantity")
                        pDrAdd("UnitMeasure") = pDrvUnit("UnitMeasure")

                        '// copy fields
                        pDrAdd("BranchID") = pDrItem("BranchID")
                        pDrAdd("QtySold") = pDrItem("QtySold")
                        pDrAdd("QtyServed") = pDrItem("QtyServed")
                        pDrAdd("WarehouseID") = pDrItem("WarehouseID")

                        pDtAdd.Rows.Add(pDrAdd)
                        pQtyLeftToServe -= pQtyToServe * pDrvUnit("Quantity")
                    End If
                Else
                    ' this was excluded from QR because its qty is below the minimum deliverable qty (tingi-tingi)
                End If
            Next pDrvUnit
        Next pDrItem

        Return pDtAdd

    End Function

    Public Function AddStockColumns(ByVal vDtItems As DataTable) As DataTable

        Dim pDt As DataTable
        Dim pDr As DataRow
        Dim pDtStock, pDtStockAll As DataTable

        pDt = ExtractFieldValues(vDtItems, "StockID")
        For Each pDr In pDt.Rows
            pDtStock = mParent.LibraryStock.ByIds(pDr("StockID"))
            If pDtStockAll Is Nothing Then
                pDtStockAll = pDtStock
            Else
                pDtStockAll.Merge(pDtStock)
            End If
        Next pDr

        If vDtItems.Columns.Contains("StockNumber") = False Then vDtItems.Columns.Add("StockNumber")
        If vDtItems.Columns.Contains("StockDescription") = False Then vDtItems.Columns.Add("StockDescription")

        For Each pDr In vDtItems.Rows
            pDtStockAll.DefaultView.RowFilter = "ID = " & ReadyString(pDr("StockID"))
            If pDtStockAll.DefaultView.Count > 0 Then
                pDr("StockNumber") = pDtStockAll.DefaultView(0)("StockNumber")
                pDr("StockDescription") = pDtStockAll.DefaultView(0)("StockDescription")
            End If
        Next pDr

        Return vDtItems

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>Sales Order numbers (comma-delimited) that are generated by this function</returns>
    ''' <remarks></remarks>
    Public Function GenerateOrders(Optional ByVal vDateTo As String = Nothing) As String

        Dim pCreator As TransSalesOrder.Creator
        Dim pDT As DataTable
        Dim pDtBranches As DataTable
        Dim pDrBranch As DataRow
        Dim pDrv As DataRowView
        Dim pItem As TransSalesOrder.Creator.Item
        Dim pDR As DataRow
        Dim pSql As String
        Dim pOrderNum As String
        Dim pDtStockDetails As DataTable
        Dim pDrvStockDetail As DataRowView
        Dim pQty As Integer
        Dim pIU As InsertUpdate
        Dim pHasOrder As Boolean
        Dim pCancellationPeriod As Integer
        Dim pStrs As String()
        Dim pStr As String
        Dim pDtWarehouses As DataTable

        pDtStockDetails = New DataTable
        pDT = GetItems(pDtBranches, pDtStockDetails, vDateTo)
        If pDT Is Nothing Then
            Return Nothing
        End If
        pDtStockDetails.Columns.Add("QrServed", GetType(Integer))   ' in base unit
        pDtStockDetails.DefaultView.Sort = "LastUpdated"
        pIU = New InsertUpdate("TransQrPos")
        pIU("ID") = "newid()"

        For Each pDrBranch In pDtBranches.Rows
            pDrBranch("CustomerID") = mParent.LibraryBranches.GetCustomerID(pDrBranch("BranchID"))
            If pDrBranch("CustomerID") Is DBNull.Value Then
                If ShowMessage(pDrBranch("BranchName") & " does not have a customer account." & vbCrLf & _
                    "Do you want to assign one now?", "No customer account", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then

                    With New ucLibraryBranches
                        .HasFind = False
                        .ShowDialog(eFindMode.None)
                    End With
                End If
                Exit Function
            End If

            If CStr2(mParent.LibraryCustomers.GetBillingAddress(CStr2(pDrBranch("CustomerID")))) = "" Then
                pDR = mParent.LibraryCustomers.ById(CStr2(pDrBranch("CustomerID")))
                If pDR Is Nothing Then
                    ShowMessage("Invalid customer. CustomerID: " & CStr2(pDrBranch("CustomerID") & " for branch, " & pDrBranch("BranchName")), , , MessageBoxIcon.Exclamation)
                    Exit Function
                Else
                    ShowMessage("Customer, " & pDR("CompanyName") & " has no address", "No address", , MessageBoxIcon.Exclamation)
                    Exit Function
                End If
            End If

            If CStr2(mParent.LibraryCustomers.GetContactPerson(CStr2(pDrBranch("CustomerID")))) = "" Then
                pDR = mParent.LibraryCustomers.ById(CStr2(pDrBranch("CustomerID")))
                If pDR Is Nothing Then
                    ShowMessage("Invalid customer. CustomerID: " & CStr2(pDrBranch("CustomerID") & " for branch, " & pDrBranch("BranchName")), , , MessageBoxIcon.Exclamation)
                    Exit Function
                Else
                    ShowMessage("Customer, " & pDR("CompanyName") & " has no contact person", "No contact person", , MessageBoxIcon.Exclamation)
                    Exit Function
                End If
            End If
        Next pDrBranch

        pCreator = mParent.TransSalesOrder.CreateCreator
        pCreator.BranchID = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 5)
        pCreator.UserID = EmployeeWebID

        pSql = mParent.SystemDefaults.GetOrAddSystemDefault("BackOrder Cancellation Period", 15)

        If Integer.TryParse(pSql, pCancellationPeriod) = False Then
            pSql = 15
        End If
        pCancellationPeriod = pSql
        For Each pDrBranch In pDtBranches.Rows
            pDtWarehouses = New DataView(pDT, "BranchID = " & pDrBranch("BranchID"), "", DataViewRowState.CurrentRows).ToTable(True, "WarehouseID")
            For Each pDrWarehouse As DataRow In pDtWarehouses.Rows
                pDT.DefaultView.RowFilter = "BranchID = " & pDrBranch("BranchID") &
                    " AND WarehouseID = " & ReadyString(pDrWarehouse("WarehouseID"))
                If pDT.DefaultView.Count > 0 Then
                    pCreator.CustomerID = CStr2(pDrBranch("CustomerID"))
                    pCreator.CustomerAddressID = mParent.LibraryCustomers.GetBillingAddress(pCreator.CustomerID)
                    pCreator.DeliverToID = mParent.LibraryCustomers.GetDeliveryAddress(pCreator.CustomerID)
                    pCreator.CusRepresentativeID = mParent.LibraryCustomers.GetContactPerson(pCreator.CustomerID)
                    'pCreator.IsBranch = 1
                    pCreator.UserID = modClient.EmployeeWebID
                    pCreator.DeliveryDate = Now
                    pCreator.Remarks = "Generated by Quick Response (QR) module."
                    pCreator.DontAutoInvoice = False

                    '// append the machine numbers (for branches with specified machine numbers)
                    If CStr2(pDrBranch("MachineNumsToQr")).Trim.Length > 0 Then
                        pStrs = CStr2(pDrBranch("MachineNumsToQr")).Split({","}, StringSplitOptions.RemoveEmptyEntries)
                        pSql = ""
                        For Each pStr In pStrs
                            If pSql.Length > 0 Then pSql &= ", "
                            pSql &= pStr.Trim
                        Next pStr
                        pCreator.Remarks &= " For machine #'s: " & pSql
                    End If

                    pCreator.Items.Clear()
                    For Each pDrv In pDT.DefaultView
                        pItem = New TransSalesOrder.Creator.Item
                        With pItem
                            .StockID = CStr2(pDrv("StockID"))
                            .UnitMeasure = pDrv("UnitMeasure") & ""
                            .PriceLevelID = ReadyString("{01000012-0006-0000-0000-000000000000}")
                            .ActualQuantity = pDrv("ActualQuantity")
                            .UnitPrice = mParent.LibraryStock.GetBranchCost(.StockID, .PriceLevelID, .ActualQuantity, .UnitMeasure)
                            .OriginalUnitPrice = .UnitPrice
                            .UnitPriceWOTax = .UnitPrice
                            If .UnitPrice <= 0 Then
                                Continue For
                            End If
                            .Quantity = pDrv("Quantity")
                            .SubTotal = .UnitPrice * .Quantity
                            .Cost = mParent.LibraryStock.GetLandedCost(.StockID) * .ActualQuantity
                            .CancellationPeriod = pCancellationPeriod
                        End With
                        pCreator.Items.Add(pItem)
                    Next pDrv
                    mProData.BeginTransaction()
                    pCreator.OrderNo = 0    ' zero this to generate a new unique sales order number
                    If pCreator.Save(True) Then
                        For Each pItem In pCreator.Items
                            pDtStockDetails.DefaultView.RowFilter = "BranchID = " & pDrBranch("BranchID") & _
                                " AND StockID = " & ReadyString(pItem.StockID) &
                                " AND WarehouseID = " & ReadyString(pDrWarehouse("WarehouseID"))
                            For Each pDrvStockDetail In pDtStockDetails.DefaultView
                                If pDrvStockDetail("QtyServed") < pDrvStockDetail("QtySold") Then
                                    If pDrvStockDetail("QtySold") < pDrvStockDetail("QtyServed") + pItem.ActualQuantity * pItem.Quantity Then
                                        pQty = pDrvStockDetail("QtySold") - pDrvStockDetail("QtyServed")
                                    Else
                                        pQty = pDrvStockDetail("QtyServed") + pItem.ActualQuantity * pItem.Quantity
                                    End If
                                    pDrvStockDetail("QtyServed") += pQty

                                    pIU("SalesOrderID") = ReadyString(pCreator.ID)
                                    pIU("PosSalesItemID") = ReadyString(pDrvStockDetail("ID"))
                                    pIU("QtyServed") = pQty
                                    pIU("FromCashSlip") = pDrvStockDetail("FromCashSlip")
                                    pIU("SourceType") = pDrvStockDetail("SourceType")
                                    pSql = pIU.GetInsert
                                    mProData.ExecuteSQL(pSql)
                                    pHasOrder = True
                                End If
                            Next pDrvStockDetail
                        Next pItem
                        mParent.LibraryBranches.CreateAndTagQr(CStr2(pDrBranch("ID")), pDrBranch("DateTo"), pDrBranch("SubmissionDate"), pCreator.ID)

                        mProData.CommitTransaction()
                        AppendToFile(AppPath & "QR.log", vbCrLf & "[" & Now & "] Sales Order " & pCreator.OrderNo & " was generated by QR")
                        If CStr2(pOrderNum).Length > 0 Then pOrderNum &= ", "
                        pOrderNum &= pCreator.OrderNo
                    Else
                        mProData.RollbackTransaction()
                    End If
                End If  ' pDT.DefaultView.Count > 0
            Next pDrWarehouse
        Next pDrBranch
        If pHasOrder Then TagReplenished()

        Return pOrderNum

    End Function

    Public Function TagReplenished() As Integer

        Dim pSql As String
        Dim pCount As Integer
        Dim pOldCommandTimeout As Integer

        Try
            pOldCommandTimeout = mProData.CommandTimeout
            mProData.CommandTimeout = 60 * 5 ' 5 minutes
            pSql = "SELECT POSSalesItems.POSSalesID" & _
               " FROM POSSalesItems" & _
               " JOIN POSSales ON POSSalesItems.POSSalesID = POSSales.ID " & _
               " LEFT JOIN TransQrPos ON POSSalesItems.ID = TransQrPos.PosSalesItemID" & _
               " WHERE ISNULL(POSSales.IsReplenished, 0) = 0" & _
               " AND ISNULL(TransQrPos.FromCashSlip, 0) = 0" & _
               " AND POSSales.LastUpdated >= " & ReadyString(mStartDate) &
               " GROUP BY POSSalesItems.POSSalesID" & _
               " HAVING SUM(TransQrPos.QtyServed) >= SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity)"
            pSql = "UPDATE POSSales" & _
                " SET IsReplenished = 1" & _
                " WHERE ID IN (" & pSql & ")"
            pCount = mProData.ExecuteSQL(pSql)

            pSql = "SELECT POSCashSlipSalesItems.CashSlipID" & _
                " FROM POSCashSlipSalesItems" & _
                " JOIN POSCashSlipSales ON POSCashSlipSalesItems.CashSlipID = POSCashSlipSales.ID " & _
                " LEFT JOIN TransQrPos ON POSCashSlipSalesItems.ID = TransQrPos.PosSalesItemID" & _
                " WHERE ISNULL(POSCashSlipSales.IsReplenished, 0) = 0" & _
                " AND ISNULL(TransQrPos.FromCashSlip, 1) = 1" & _
                " AND POSCashSlipSales.LastUpdated >= " & ReadyString(mStartDate) &
                " GROUP BY POSCashSlipSalesItems.CashSlipID" & _
                " HAVING SUM(TransQrPos.QtyServed) >= SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity)"
            pSql = "UPDATE POSCashSlipSales" & _
                " SET IsReplenished = 1" & _
                " WHERE ID IN (" & pSql & ")"
            pCount += mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Throw
        Finally
            mProData.CommandTimeout = pOldCommandTimeout
        End Try

        Return pCount

    End Function

    Private Sub GetPosIds(ByVal vBranchID As Integer, ByVal vDateTo As Date, _
       ByRef oPOSSales As DataTable, ByRef oCashSlipSales As DataTable)

        Dim pSql As String

        pSql = "SELECT ID" & _
            " FROM POSSales" & _
            " WHERE LastUpdated <= " & ReadyString(vDateTo) & _
            " AND BranchID = " & vBranchID & _
            " AND QRSalesOrderID IS NULL"
        oPOSSales = mProData.Execute(pSql)

        pSql = "SELECT ID" & _
            " FROM POSCashSlipSales" & _
            " WHERE LastUpdated <= " & ReadyString(vDateTo) & _
            " AND BranchID = " & vBranchID & _
            " AND QRSalesOrderID IS NULL"
        oCashSlipSales = mProData.Execute(pSql)

    End Sub
#End If

    Public Function AddBranchColumn(ByVal vDtItems As DataTable) As DataTable

        Dim pDr As DataRow
        Dim pDtBranch As DataTable

        pDtBranch = mParent.LibraryBranches.GetAll

        If vDtItems.Columns.Contains("BranchName") = False Then vDtItems.Columns.Add("BranchName")

        For Each pDr In vDtItems.Rows
            pDtBranch.DefaultView.RowFilter = "BranchID = " & pDr("BranchID")
            If pDtBranch.DefaultView.Count > 0 Then
                pDr("BranchName") = pDtBranch.DefaultView(0)("Name")
            End If
        Next pDr

        Return vDtItems

    End Function

    Public Function GetBranchesStatus() As DataTable

        Dim pDtBranches As DataTable
        Dim pDrBranch As DataRow
        Dim pDvSched As DataView
        Dim pDate As Date?
        Dim pDrSalesOrder As DataRow

        pDtBranches = mParent.LibraryBranches.GetAll()
        pDtBranches = New DataView(pDtBranches, "Num IS NOT NULL", "", DataViewRowState.CurrentRows).ToTable
        pDtBranches = AddBranchColumn(pDtBranches)
        pDtBranches.Columns.Add("DateTo", GetType(Date))
        pDtBranches.Columns.Add("SubmissionDate", GetType(Date))
        pDtBranches.Columns.Add("LastServedDate")
        pDtBranches.Columns.Add("LastImportDate", GetType(Date))
        pDtBranches.Columns.Add("LastSoNum")    ' last sales order number
        pDtBranches.Columns.Add("LastSoDate", GetType(Date)) ' last sales order date

        For Each pDrBranch In pDtBranches.Rows
            pDrBranch("LastServedDate") = mParent.LibraryBranches.LastServedDate(CStr2(pDrBranch("ID")))
            pDvSched = mParent.LibraryBranches.GetModifiedQr(CStr2(pDrBranch("ID")))
            If pDvSched IsNot Nothing _
                AndAlso pDvSched.Count > 0 Then

                pDrBranch("SubmissionDate") = pDvSched(0)("SubmissionDate")
                pDrBranch("DateTo") = pDvSched(0)("DateTo")
            End If
            pDate = mParent.LibraryBranches.LastImported(pDrBranch("Num"))
            pDrBranch("LastImportDate") = IIf(pDate Is Nothing, DBNull.Value, pDate)
            pDrSalesOrder = mParent.LibraryBranches.LastQrSalesOrder(CStr2(pDrBranch("ID")))
            If pDrSalesOrder IsNot Nothing Then
                pDrBranch("LastSoNum") = pDrSalesOrder("OrderNo")
                pDrBranch("LastSoDate") = pDrSalesOrder("LastUpdated")
            End If
        Next pDrBranch

        Return pDtBranches

    End Function
End Class

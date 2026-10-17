Public Class InventoryLedgerController
    Inherits System.Web.Mvc.Controller

    ReadOnly mBusiness As Business

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If
        'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()
        'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.StockInventory_InventoryLedger, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetReport() As String

        Dim pDT As DataTable

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.StockInventory_InventoryLedger.IsEnable _
        '    ) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Select Case CStr2(Common.WebRequest.Data("DisplayType"))
            Case "ledger"
                pDT = GetLedger()

            Case "bin"
                pDT = GetBinLedger()

            Case "fowst"
                pDT = GetFowstLedger()

            Case "product"
                pDT = GetProductLedger()

                Dim dtSummary As New DataTable

                dtSummary.Columns.Add("Transaction")
                dtSummary.Columns.Add("Total")

                Dim TransType() As String = {"Sales Return", "Receiving", "Invoice", "Inventory Count", "TransferOut"}

                For i As Integer = 0 To 4
                    'pDT.DefaultView.RowFilter = "Source = 'Sales Invoice'"

                    'If pDT.DefaultView.ToTable.Rows.Count > 0 Then
                    Dim dr As DataRow = dtSummary.NewRow
                    Dim total As Double

                    If (TransType(i) = "Inventory Count") Then
                        total = CDec2(pDT.Compute("SUM(Quantity)", "TransName like " & ReadyStringWild(TransType(i))))
                        'total = CDec2(pDT.Compute("SUM(Balance)", "TransName like " & ReadyStringWild(TransType(i))))
                    Else
                        total = CDec2(pDT.Compute("SUM(Quantity)", "TransName like " & ReadyStringWild(TransType(i))))
                    End If

                    dr(0) = TransType(i)
                    dr(1) = IIf(TransType(i) = "Invoice", Math.Abs(total), total)

                    dtSummary.Rows.Add(dr)
                    'End If
                Next

                Dim JsonTemp As New JsonDictionary

                JsonTemp.Add("Details", pDT)
                JsonTemp.Add("Summary", dtSummary)

                Return JsonTemp.Serialize 'JsonDictionary.Serialize("data", )
        End Select

        Return JsonDictionary.Serialize("data", pDT)

    End Function

    Dim mProData As ProData = Common.WebRequest.ProData

    Private Function GetLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,TransInventoryLedger.Qty Quantity" & vbCrLf & _
            " ,TransInventoryLedger.Balance" & vbCrLf & _
            " ,MobizLibraryOrganizationFowst.Name AS Fowst" & vbCrLf & _
            " ,TransInventoryLedger.LastUpdated" & vbCrLf & _
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf & _
            ",ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS Source " & vbCrLf & _
            ",ISNULL(LibraryOrganizationPIM.AllowDecimal, 0) AS AllowDecimal" & vbCrLf & _
            " " & vbCrLf & _
            " FROM TransInventoryLedger" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf & _
            " JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID" & vbCrLf & _
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf & _
            " LEFT JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = TransInventoryLedger.ProductID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf & _
            " 	SELECT 'Sales Invoice ' + CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName" & vbCrLf & _
            "     FROM TransInvoice" & vbCrLf & _
            "     WHERE TransInvoice.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Receiving ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            "     FROM TransReceiving" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Receiving'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Transfer Out ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            "     FROM TransReceiving" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'TransferOut'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Sales Return ' + CONVERT(varchar, TransMemo.MemoNumber) AS TransName" & vbCrLf & _
            "     FROM TransMemo" & vbCrLf & _
            "     JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf & _
            "     WHERE TransMemo.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Sales Returns'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Inventory Count ' + CONVERT(varchar, TransInventoryProductCount.InventoryCountDate) AS TransName" & vbCrLf & _
            "     FROM TransInventoryProductCount" & vbCrLf & _
            "     WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'InventoryCount'" & vbCrLf & _
            " ) tSource" & vbCrLf &
            " ORDER BY LibraryFowstBin.Name, TransInventoryLedger.LastUpdated" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetProductLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS TransName" & vbCrLf & _
            " ,TransInventoryLedger.Qty Quantity" & vbCrLf & _
            " ,TransInventoryLedger.Balance" & vbCrLf & _
            " ,MobizLibraryOrganizationFowst.Name AS Fowst" & vbCrLf & _
            " ,TransInventoryLedger.LastUpdated" & vbCrLf & _
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf & _
            " ,LibraryFowstBin.ID AS BinID" & vbCrLf & _
            " ,ISNULL(tSource.UpdatedBy, MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName) AS UpdatedBy" & vbCrLf & _
            ",ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS Source " & vbCrLf & _
            " " & vbCrLf & _
            " FROM TransInventoryLedger" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf & _
            " JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID" & vbCrLf & _
            " JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryLedger.UserID" & vbCrLf & _
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf & _
            " OUTER APPLY (" & vbCrLf & _
            " 	SELECT 'Sales Invoice ' + CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName" & vbCrLf & _
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf & _
            "     FROM TransInvoice inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf & _
            "     WHERE TransInvoice.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Receiving ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf & _
            "     FROM TransReceiving inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Receiving'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Transfer Out ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf & _
            "     FROM TransReceiving inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'TransferOut'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Sales Return ' + CONVERT(varchar, TransMemo.MemoNumber) AS TransName" & vbCrLf & _
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf & _
            "     FROM TransMemo inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransMemo.UserID" & vbCrLf & _
            "     JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf & _
            "     WHERE TransMemo.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Sales Returns'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Inventory Count ' + CONVERT(varchar, TransInventoryProductCount.InventoryCountDate) AS TransName" & vbCrLf & _
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf & _
            "     FROM TransInventoryProductCount inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID" & vbCrLf & _
            "     WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'InventoryCount'" & vbCrLf & _
            " ) tSource" & vbCrLf &
            " Where dbo.MobizLibraryPIMProduct.ID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & vbCrLf &
            " AND MobizLibraryOrganizationFowst.ID = " & ReadyString(WebRequest.Instance.Data("FOWSTID")) & vbCrLf &
            " AND convert(date, TransInventoryLedger.LastUpdated) between " & vbCrLf &
            ReadyString(WebRequest.Instance.Data("DateFrom")) & " AND " & ReadyString(WebRequest.Instance.Data("DateTo")) & vbCrLf &
            " ORDER BY MobizLibraryOrganizationFowst.Name, TransInventoryLedger.LastUpdated" & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetBinLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        mProData.BeginTransaction()
        Try
            CreateTempTables()
            pSql = "SELECT * FROM #t2"
            pDT = mProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Throw
        Finally
            mProData.RollbackTransaction()
        End Try

    End Function

    Private Function GetFowstLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        mProData.BeginTransaction()
        Try
            CreateTempTables()
            pSql = "SELECT ProductID" & vbCrLf & _
                " ,ProductNo" & vbCrLf & _
                " ,ProductName" & vbCrLf & _
                " ,FowstID" & vbCrLf & _
                " ,Fowst" & vbCrLf & _
                " ,SUM(Balance) AS Balance" & vbCrLf & _
                " FROM #t2" & vbCrLf & _
                " GROUP BY ProductID" & vbCrLf & _
                " ,ProductNo" & vbCrLf & _
                " ,ProductName" & vbCrLf & _
                " ,FowstID" & vbCrLf & _
                " ,Fowst" & vbCrLf & _
                " ORDER BY ProductName" & vbCrLf &
                ",Fowst"
            pDT = mProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Throw
        Finally
            mProData.RollbackTransaction()
        End Try

    End Function

    ''' <summary>
    ''' this must be wrapped in transaction and error handler
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateTempTables()

        Dim pSql As String

        pSql = "SELECT DISTINCT TransInventoryLedger.ProductID" & vbCrLf & _
            " ,dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,TransInventoryLedger.FowstID" & vbCrLf & _
            " ,MobizLibraryOrganizationFOWST.Name AS Fowst" & vbCrLf & _
            " ,TransInventoryLedger.BinID" & vbCrLf & _
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf & _
            " INTO #t" & vbCrLf & _
            " FROM TransInventoryLedger" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf & _
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID" & vbCrLf & _
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf
        mProData.ExecuteSQL(pSql)

        pSql = " SELECT #t.*" & vbCrLf & _
            " ,t.*" & vbCrLf & _
            " INTO #t2" & vbCrLf & _
            " FROM #t" & vbCrLf & _
            " CROSS APPLY (" & vbCrLf & _
            " 	SELECT TOP 1 TransInventoryLedger_1.LastUpdated" & vbCrLf & _
            "     ,TransInventoryLedger_1.Qty AS Quantity" & vbCrLf & _
            " 	,TransInventoryLedger_1.Balance" & vbCrLf & _
            "     FROM TransInventoryLedger TransInventoryLedger_1" & vbCrLf & _
            "     WHERE #t.FowstID = TransInventoryLedger_1.FowstID" & vbCrLf & _
            "     AND #t.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf & _
            "     AND ISNULL(CONVERT(char(36), #t.BinID), '') = ISNULL(CONVERT(char(36), TransInventoryLedger_1.BinID), '')" & vbCrLf & _
            "     ORDER BY TransInventoryLedger_1.LastUpdated DESC" & vbCrLf & _
            " ) t" & vbCrLf & _
            " ORDER BY t.LastUpdated" & vbCrLf
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function SetBeginningBalance() As String
        'get top record -product, fowst, bin, date - if transtype = inventory count, then cover then get the next record?
        Dim pSql As String
        Dim pDTFirstRecord As DataTable
        Dim pDTInventoryCountRecord As DataTable
        Dim pDTRecordList As DataTable
        Dim BegBalance As Double = 0.0

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Try

            BegBalance = CDec2(Common.WebRequest.Data("BegBalance"))

            pSql = "Select top 1 * from TransInventoryLedger where " & vbCrLf &
                    "FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                    " AND convert(date, LastUpdated) >= " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
                    " order by LastUpdated"

            pDTFirstRecord = mProData.Execute(pSql)

            If pDTFirstRecord.Rows.Count > 0 Then
                'get next inventory count record - if none, then reconstruct records greater than the given date
                pSql = "Select * from TransInventoryLedger where " & vbCrLf &
                    "FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                    " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("DateFrom")) & vbCrLf &
                    " AND TransactionType = 'InventoryCount' " & vbCrLf &
                    " order by LastUpdated"

                pDTInventoryCountRecord = mProData.Execute(pSql)

                pSql = "Select * from TransInventoryLedger where " & vbCrLf &
                    "FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))

                If pDTInventoryCountRecord.Rows.Count > 0 Then
                    pSql += " AND LastUpdated between " & ReadyString(pDTFirstRecord.Rows(0).Item("LastUpdated")) & vbCrLf &
                            " AND " & ReadyString(pDTInventoryCountRecord.Rows(0).Item("LastUpdated"))
                Else
                    pSql += " AND LastUpdated > " & ReadyString(pDTFirstRecord.Rows(0).Item("LastUpdated"))
                End If

                pSql += " order by LastUpdated"

                pDTRecordList = mProData.Execute(pSql)

                If pDTRecordList.Rows.Count > 0 Then
                    'Beggining Balance
                    pSql = "Insert into TransInventoryLedger(ProductID, FowstID, BinID, TransactionID, TransactionType, " &
                            "Qty, Balance, UserID, LastUpdated) values(" &
                            ReadyString(Common.WebRequest.Data("ProductID")) & "," &
                            ReadyString(Common.WebRequest.Data("FOWSTID")) & "," &
                            ReadyString(Common.WebRequest.Data("BinID")) & "," &
                            ReadyString(Guid.NewGuid.ToString) & "," &
                            ReadyString("Beginning Balance") & "," &
                            0 & "," &
                            BegBalance & "," &
                            ReadyString(WebSession.Instance.PeopleUserID) & "," &
                            ReadyString(DateAdd(DateInterval.Second, -1, CDate(pDTFirstRecord.Rows(0).Item("LastUpdated")))) &
                            ")"

                    mProData.Execute(pSql)

                    For i As Integer = 0 To pDTRecordList.Rows.Count - 1
                        BegBalance = BegBalance + CDec2(pDTRecordList.Rows(i).Item("Qty"))

                        pSql = "Update TransInventoryLedger set Balance = " & BegBalance & " where id = " & ReadyString(pDTRecordList.Rows(i).Item("ID"))
                        mProData.Execute(pSql)
                    Next
                End If
            End If

            Return JsonDictionary.Serialize("Results", "Success")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", "Error")
        End Try
    End Function

    Function FirstTimeBeginningBalance() As String
        Dim pSql As String
        Dim dtTemp As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        pSql = "Select top 1 * from TransInventoryLedger where " & vbCrLf &
                    "FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                    " AND TransactionType = 'Beginning Balance'"

        dtTemp = mProData.Execute(pSql)

        If dtTemp.Rows.Count > 0 Then
            Return JsonDictionary.Serialize("Results", "Error")
        Else
            Return JsonDictionary.Serialize("Results", "Success")
        End If
    End Function
End Class

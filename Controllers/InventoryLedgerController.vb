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
    Public Function GetFOWSTBinProductEndingBalance() As String
  
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String
        Dim pStr As String

        mProData.BeginTransaction()
        Try
            WebRequest.Instance.Business.TransInventoryLedger.CreateTempTables()
            pSql = "SELECT ProductID" & vbCrLf & _
              " ,FowstID" & vbCrLf & _
              " ,BinID" & vbCrLf & _
              " ,Balance AS Balance" & vbCrLf & _
              " ,Quantity AS Quantity" & vbCrLf & _
              " FROM #t2 " & vbCrLf & _
              " Order by FowstID, ProductID, BinID"
            pDT = mProData.Execute(pSql)
        Catch ex As Exception
            Try
                pStr = ex.Message & vbCrLf & vbCrLf &
                    "Common.WebRequest.InputStreamText: " & Common.WebRequest.InputStreamText & vbCrLf & vbCrLf
                If TypeOf ex Is ProSqlException Then
                    pStr &= "Sql: " & CType(ex, ProSqlException).Sql & vbCrLf & vbCrLf
                    pStr &= "Database: " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf
                End If
                AppendToLog(pStr,
                    Common.ServerPath & "temp\logs\GetFOWSTBinProductEndingBalance")
            Catch ex2 As Exception
                Stop2() ' just ignore any errors here
            End Try
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            mProData.RollbackTransaction()
        End Try

        Return JsonDictionary.Serialize("data", pDT)

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
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied") 'comment para ma-access sa log in page

        Select Case CStr2(Common.WebRequest.Data("DisplayType"))
            Case "ledger"
                pDT = GetLedger()

            Case "bin"
                pDT = GetBinLedger()

            Case "fowst"
                pDT = GetFowstLedger(Not String.IsNullOrWhiteSpace(Common.WebRequest.Data("SearchString")))
                'pDT = GetFowstLedger2()

                Dim pOut As New JsonDictionary
                Dim nCount As Integer

#If DEBUG Then
                nCount = 50
#Else
                nCount= 5000
#End If
                If pDT.Rows.Count < nCount Then
                    pOut.Add("data", pDT)
                Else
                    Dim dtTemp As DataTable

                    Try
                        pDT.DefaultView.RowFilter = "ROW_NUMBER> " & CInt2(Common.WebRequest.Data("InventoryDataCount")) * nCount & " AND ROW_NUMBER <= " & (CInt2(Common.WebRequest.Data("InventoryDataCount") + 1)) * nCount
                        dtTemp = pDT.DefaultView.ToTable
                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try

                    pOut.Add("data", dtTemp)
                    pOut.Add("RowCount", pDT.Rows.Count)
                End If

                Return pOut.Serialize

            Case "product"
                pDT = GetProductLedger()

                Dim dtSummary As New DataTable

                dtSummary.Columns.Add("Transaction")
                dtSummary.Columns.Add("Total", GetType(Decimal))
                dtSummary.Columns.Add("SubtransactionType")

                Dim TransType() As String = {"Sales Return", "SR", "Sales Invoice", "Inventory Count Positive", "Inventory Count Negative", "Transfer Out"}

                For i As Integer = 0 To 5
                    'pDT.DefaultView.RowFilter = "Source = 'Sales Invoice'"

                    'If pDT.DefaultView.ToTable.Rows.Count > 0 Then
                    Dim dr As DataRow = dtSummary.NewRow
                    Dim total As Double

                    If (TransType(i) = "Inventory Count Positive") Then
                        total = CDec2(pDT.Compute("SUM(Quantity)", "(TransType like " & ReadyStringWild("Inventory") & " AND Quantity>0) OR TransName like '%Beginning Balance%'"))
                    ElseIf (TransType(i) = "Inventory Count Negative") Then
                        total = CDec2(pDT.Compute("SUM(Quantity)", "(TransType like " & ReadyStringWild("Inventory") & " AND Quantity<0)"))
                    Else
                        total = CDec2(pDT.Compute("SUM(Quantity)", "TransType like " & ReadyStringWild(TransType(i))))
                    End If

                    dr(0) = TransType(i)
                    dr(1) = Math.Abs(total) 'IIf(TransType(i) = "Sales Invoice", Math.Abs(total), total)

                    dtSummary.Rows.Add(dr)
                    'End If
                Next
                For Each pSubTransType As String In New String() {"Production", "Usage"}
                    Dim pDR As DataRow
                    Dim pTotal As Decimal
                    Dim pSql As String
                    pDR = dtSummary.NewRow
                    pDR("Transaction") = "Sales Invoice"
                    pDR("SubtransactionType") = pSubTransType
                    pSql = "TransType LIKE '%Invoice%'" & vbCrLf &
                        " AND SubtransactionType = " & ReadyString(pSubTransType)
                    pDR("Total") = Math.Abs(CDec2(pDT.Compute("SUM(Quantity)", pSql)))
                    dtSummary.Rows.Add(pDR)
                Next pSubTransType

                Dim JsonTemp As New JsonDictionary

                JsonTemp.Add("Details", pDT)
                JsonTemp.Add("Summary", dtSummary)
                JsonTemp.Add("BaseUnit", WebRequest.Instance.Business.LibraryStock.GetFirstUnit(WebRequest.Instance.Data("ProductID")))

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
            " ,TransInventoryLedger.Price" & vbCrLf & _
            " ,MobizLibraryOrganizationFowst.Name AS Fowst" & vbCrLf & _
            " ,TransInventoryLedger.TransactionDate" & vbCrLf & _
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
            " ORDER BY LibraryFowstBin.Name, TransInventoryLedger.TransactionDate, TransInventoryLedger.LastUpdated" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetProductLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSelect, pFrom As String
        BrowserController.ComposeCustomer(pSelect, pFrom)

        pSql = "SELECT dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS TransName" & vbCrLf &
            " ,ISNULL(tSource.TransType, TransInventoryLedger.TransactionType) AS TransType" & vbCrLf &
            " ,TransInventoryLedger.Qty Quantity" & vbCrLf &
            " ,TransInventoryLedger.Balance" & vbCrLf &
            " ,TransInventoryLedger.Price" & vbCrLf &
            " ,MobizLibraryOrganizationFowst.Name AS Fowst" & vbCrLf &
            " -- , tSource.LastUpdated AS TransactionDate -- -- 2023-08-24 swap TransInventoryLedger and tSource" & vbCrLf &
            " , TransInventoryLedger.LastUpdated AS TransactionDate" & vbCrLf &
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf &
            " ,LibraryFowstBin.ID AS BinID" & vbCrLf &
            " ,ISNULL(tSource.UpdatedBy, MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName) AS UpdatedBy" & vbCrLf &
            ",ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS Source " & vbCrLf &
            ",TransInventoryLedger.SubtransactionType" & vbCrLf &
            ",tSource.CustomerOrVendorName" & vbCrLf &
            " " & vbCrLf &
            " FROM TransInventoryLedger" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID" & vbCrLf &
            " JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryLedger.UserID" & vbCrLf &
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            "      SELECT CASE WHEN TransInventoryLedger.TransactionType = 'Cancel Invoice' THEN 'Cancel ' ELSE '' END + ISNULL(LibraryDocumentControl.DocumentName, 'Sales Invoice') + ' ' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName" & vbCrLf &
            " 	   ,MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransInvoice.LastUpdated" & vbCrLf &
            " 	   ,'Sales Invoice' AS TransType" & vbCrLf &
            "," & pSelect & " AS CustomerOrVendorName" & vbCrLf &
            "      FROM TransInvoice inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            "      LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "      LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            "      LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
            pFrom &
            "      WHERE TransInvoice.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "       AND (" & vbCrLf &
            "       	TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf &
            " 		OR TransInventoryLedger.TransactionType = 'Cancel Invoice'" & vbCrLf &
            "  	  )" & vbCrLf &
            "     " & vbCrLf &
            "     UNION" & vbCrLf &
            "     " & vbCrLf &
            " SELECT CASE WHEN TransInventoryLedger.TransactionType = 'Cancel Receiving' THEN 'Cancel SR ' " & vbCrLf &
            "         WHEN TransInventoryLedger.TransactionType = 'TransferOut' THEN 'Transfer Out ' " & vbCrLf &
            "       	ELSE 'SR ' " & vbCrLf &
            "         END + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf &
            " 	   ,MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransReceiving.LastUpdated" & vbCrLf &
            " 	   ,NULL AS TransType" & vbCrLf &
            "," & pSelect & " AS CustomerOrVendorName" & vbCrLf &
            "      FROM TransReceiving inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
            "      LEFT JOIN MobizLibraryOrganizationFOWST ON TransReceiving.ReceivingPlaceID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "      LEFT JOIN LibraryDevice ON TransReceiving.DeviceID = LibraryDevice.ID" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceiving.SupplierID" & vbCrLf &
            pFrom &
            "      WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "      AND (TransInventoryLedger.TransactionType = 'Receiving'" & vbCrLf &
            " 		OR TransInventoryLedger.TransactionType = 'Cancel Receiving'" & vbCrLf &
            "       )" & vbCrLf &
            "     " & vbCrLf &
            "     UNION" & vbCrLf &
            "     " & vbCrLf &
            "     SELECT 'Transfer Out ' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf &
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransReceiving.LastUpdated" & vbCrLf &
            " 	   ,NULL AS TransType" & vbCrLf &
            "," & pSelect & " AS CustomerOrVendorName" & vbCrLf &
            "     FROM TransReceiving inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceiving.SupplierID" & vbCrLf &
            "   LEFT JOIN LibraryDevice ON TransReceiving.DeviceID = LibraryDevice.ID" & vbCrLf &
            pFrom &
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "     AND TransInventoryLedger.TransactionType = 'TransferOut'" & vbCrLf &
            "     " & vbCrLf &
            "     UNION" & vbCrLf &
            "     " & vbCrLf &
            "     SELECT 'Sales Return ' + CONVERT(varchar, TransMemo.MemoNumber) AS TransName" & vbCrLf &
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransMemo.LastUpdated" & vbCrLf &
            " 	   ,NULL AS TransType" & vbCrLf &
            "," & pSelect & " AS CustomerOrVendorName" & vbCrLf &
            "     FROM TransMemo inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransMemo.UserID" & vbCrLf &
            "     JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransMemoSalesReturn.CustomerID" & vbCrLf &
            pFrom &
            "     WHERE TransMemo.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "     AND TransInventoryLedger.TransactionType = 'Sales Returns'" & vbCrLf &
            "     " & vbCrLf &
            "     UNION" & vbCrLf &
            "     " & vbCrLf &
            "     SELECT 'Inventory Count: ' + CONVERT(varchar, CONVERT(decimal,TransInventoryLedger.Balance)) + ' on ' + CONVERT(varchar(19), TransInventoryProductCount.InventoryCountDate) AS TransName" & vbCrLf &
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransInventoryProductCount.InventoryCountDate" & vbCrLf &
            " 	   ,NULL AS TransType" & vbCrLf &
            " 	   ,NULL AS CustomerOrVendorName" & vbCrLf &
            "     FROM TransInventoryProductCount inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID" & vbCrLf &
            "     WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "     AND TransInventoryLedger.TransactionType = 'InventoryCount'" & vbCrLf &
            "     " & vbCrLf &
            "     UNION" & vbCrLf &
            "     " & vbCrLf &
            "     SELECT 'Cancel Sales Return ' + CONVERT(varchar, TransMemo.MemoNumber) AS TransName" & vbCrLf &
            ",   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName as UpdatedBy" & vbCrLf &
            " 	   ,TransInvoiceAdjustments.DateDeactivated AS LastUpdated" & vbCrLf &
            " 	   ,NULL AS TransType" & vbCrLf &
            "," & pSelect & " AS CustomerOrVendorName" & vbCrLf &
            "     FROM TransMemo" & vbCrLf &
            "     JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf &
            "     JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.MemoID = TransMemo.ID" & vbCrLf &
            "     join MobizLibraryPeople on MobizLibraryPeople.ID = TransInvoiceAdjustments.DeactivatedBy" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransMemoSalesReturn.CustomerID" & vbCrLf &
            pFrom &
            "     WHERE TransMemo.ID = TransInventoryLedger.TransactionID" & vbCrLf &
            "     AND TransInventoryLedger.TransactionType = 'Cancel Sales Returns'" & vbCrLf &
            " ) tSource" & vbCrLf &
            " Where dbo.MobizLibraryPIMProduct.ID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & vbCrLf &
            " AND MobizLibraryOrganizationFowst.ID = " & ReadyString(WebRequest.Instance.Data("FOWSTID")) & vbCrLf &
            " AND convert(date, TransInventoryLedger.TransactionDate) between " & vbCrLf &
            ReadyString(CDate(WebRequest.Instance.Data("DateFrom"))) & " AND " & ReadyString(CDate(WebRequest.Instance.Data("DateTo"))) & vbCrLf &
            " ORDER BY MobizLibraryOrganizationFowst.Name, TransInventoryLedger.LastUpdated, tSource.LastUpdated -- 2023-08-24 swap TransInventoryLedger and tSource " & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetBinLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        mProData.BeginTransaction()
        Try
            WebRequest.Instance.Business.TransInventoryLedger.CreateTempTables()
            pSql = "SELECT * FROM #t2"
            pDT = mProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Throw
        Finally
            mProData.RollbackTransaction()
        End Try

    End Function

    Private Function GetFowstLedger(Optional vIncludeNoInventory As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim FOWSTID As String
        Dim ProductID As String
        Dim sqlWhere As String

        FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
        ProductID = CStr2(Common.WebRequest.Data("ProductID"))

        'mProData.BeginTransaction()
        Try
            'kelangan ibalhin ang UpdateLibraryFowstBinProductDetails na function
            'WebRequest.Instance.Business.TransInventoryLedger.UpdateLibraryFowstBinProductDetails()


            'WebRequest.Instance.Business.TransInventoryLedger.CreateTempTables()

            sqlWhere = " WHERE ISNULL(LibraryOrganizationPIM.IsDeactivated, 0) = " & CInt2(Common.WebRequest.Data("IsDeactivated")) & vbCrLf

            If IsGuid(ProductID) Then
                sqlWhere += " AND LibraryFOWSTBinProduct.ProductID = " & ReadyString(ProductID) & vbCrLf
            End If

            pSql = "SELECT ROW_NUMBER() OVER (ORDER BY LibraryFOWSTBinProduct.ProductNo) ROW_NUMBER, LibraryFOWSTBinProduct.ProductID" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.ProductNo,'') AS ProductNo" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.ProductName,'') AS ProductName" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.Brand,'') AS Brand" & vbCrLf & _
                " ,LibraryFOWSTBinProduct.FowstID" & vbCrLf & _
                " ,SUM(LibraryFOWSTBinProduct.Balance) AS Balance" & vbCrLf & _
                " ,MAX(LibraryFOWSTBinProduct.LastUpdated) AS LastUpdated" & vbCrLf & _
                " ,ISNULL(SUM(LibraryFOWSTBinProduct.Balance) * MAX(LibraryFOWSTBinProduct.COGS),0) AS COGS" & vbCrLf & _
                " ,ISNULL(SUM(LibraryFOWSTBinProduct.Balance) * MAX(LibraryFOWSTBinProduct.SRP),0) AS SRP" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.UnitMeasure,'') AS UnitMeasure" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.InventoryCountApproval,0) AS InventoryCountApproval" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.IsDeactivated, 0) as IsDeactivated" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.LastInventoryCountDate, '') as LastInventoryCountDate" & vbCrLf & _
                " ,ISNULL(LibraryFOWSTBinProduct.LastInventoryCountBy, '') as LastInventoryCountBy" & vbCrLf
            If vIncludeNoInventory Then
                pSql &= " FROM (SELECT MobizLibraryPIMProduct.*" & vbCrLf &
                    ",LibraryFOWSTBinProduct.IsDeactivated" & vbCrLf &
                    ",LibraryFOWSTBinProduct.UnitMeasure" & vbCrLf &
                    ",LibraryFOWSTBinProduct.InventoryCountApproval" & vbCrLf &
                    ",MobizLibraryPIMProduct.ID AS ProductID" & vbCrLf &
                    ",LibraryFOWSTBinProduct.FowstID" & vbCrLf &
                    ",ISNULL(LibraryFOWSTBinProduct.Balance, 0) AS Balance" & vbCrLf &
                    ",LibraryFOWSTBinProduct.COGS" & vbCrLf &
                    " FROM MobizLibraryPIMProduct" & vbCrLf &
                    " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf & _
                    " LEFT JOIN LibraryFOWSTBinProduct ON LibraryFOWSTBinProduct.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                    " AND LibraryFOWSTBinProduct.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                    " ) LibraryFOWSTBinProduct" & vbCrLf
            Else
                pSql &= " FROM LibraryFOWSTBinProduct" & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = LibraryFOWSTBinProduct.ProductID" & vbCrLf

                sqlWhere &= " AND LibraryFOWSTBinProduct.FowstID = " & ReadyString(FOWSTID) & vbCrLf

                If Not CStr2(Common.WebRequest.Data("MovementYear")) = vbNullString Then
                    sqlWhere &= " AND LibraryFOWSTBinProduct.Lastupdated >= " & ReadyString(Common.WebRequest.Data("MovementYear")) & vbCrLf
                End If
            End If

            If CBool2(Common.WebRequest.Data("WithComponentsOnly")) Then

                pSql &= "INNER JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = LibraryFOWSTBinProduct.ProductID "

                'added para mafilter by FOWST ang SKU 10-26-2020
                pSql &= " AND LibraryFOWSTBinProduct.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND ISNULL(LibraryOrganizationPIM.IsDeactivated, 0) = " & CInt2(Common.WebRequest.Data("IsDeactivated")) & vbCrLf

                sqlWhere &= " AND EXISTS(" & vbCrLf &
                    " 	SELECT *" & vbCrLf &
                    "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductContents.ParentProductID = LibraryFOWSTBinProduct.ProductID" & vbCrLf &
                    " 	AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                    " )" & vbCrLf &
                    " OR NOT EXISTS(" & vbCrLf &
                    " 	SELECT *" & vbCrLf &
                    "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductContents.ChildProductID = LibraryFOWSTBinProduct.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                    " )"
            End If

            If CBool2(Common.WebRequest.Data("SubcomponentsOnly")) Then
                pSql &= "INNER JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ChildProductID = LibraryFOWSTBinProduct.ProductID"

                'added para mafilter by FOWST ang SKU 10-26-2020
                pSql &= " AND LibraryFOWSTBinProduct.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND ISNULL(LibraryOrganizationPIM.IsDeactivated, 0) = " & CInt2(Common.WebRequest.Data("IsDeactivated")) & vbCrLf

                'sqlWhere &= " AND EXISTS(" & vbCrLf &
                '    " 	SELECT *" & vbCrLf &
                '    "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                '    "     WHERE MobizLibraryPIMProductContents.ChildProductID = LibraryFOWSTBinProduct.ProductID" & vbCrLf &
                '    "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                '    " )"

                'sqlWhere &= " AND MobizLibraryPIMProductContents.ChildProductID = LibraryFOWSTBinProduct.ProductID " & vbCrLf
            End If

            If Not CStr2(Common.WebRequest.Data("Category")) = vbNullString Then

                pSql &= " INNER JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryFOWSTBinProduct.ProductID"

                sqlWhere &= " AND MobizLibraryPIMProductVariableFeatures001.C0020 = " & ReadyString(Common.WebRequest.Data("Category"))

            End If

            If Not CStr2(Common.WebRequest.Data("VendorID")) = vbNullString Then
                pSql &= " INNER JOIN LibraryVendorPrices ON LibraryVendorPrices.ProductID = LibraryFOWSTBinProduct.ProductID " & vbCrLf

                sqlWhere &= " AND LibraryVendorPrices.VendorID = " & ReadyString(Common.WebRequest.Data("VendorID"))
            End If

            pSql &= sqlWhere

            pSql &=
            " GROUP BY LibraryFOWSTBinProduct.ProductID" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.ProductNo" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.ProductName" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.Brand" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.FowstID" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.UnitMeasure" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.InventoryCountApproval" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.IsDeactivated" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.LastInventoryCountDate" & vbCrLf & _
            " ,LibraryFOWSTBinProduct.LastInventoryCountBy" & vbCrLf & _
            " ORDER BY ROW_NUMBER, ProductName" & vbCrLf
            '",Fowst "

            pDT = mProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Throw
        Finally
            'mProData.RollbackTransaction()
        End Try

    End Function

    Private Function GetFowstLedger2() As DataTable
        Return WebRequest.Instance.Business.TransInventoryLedger.GetFowstLedger()
    End Function

    Public Function SetBeginningBalance() As String
        'get top record -product, fowst, bin, date - if transtype = inventory count, then cover then get the next record?
        Dim pSql As String
        Dim pDTFirstCurrentRecord As DataTable
        Dim pDTInventoryCountRecord As DataTable
        Dim pDTRecordList As DataTable
        Dim BegBalance As Double = 0.0
        Dim PrevBalance As Double = 0.0
        Dim BinID As String
        Dim ProductID As String
        Dim FOWSTID As String
        Dim DateFrom As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Try

            BegBalance = CDec2(Common.WebRequest.Data("BegBalance"))
            BinID = CStr(Common.WebRequest.Data("BinID"))
            ProductID = CStr(Common.WebRequest.Data("ProductID"))
            FOWSTID = CStr(Common.WebRequest.Data("FOWSTID"))
            DateFrom = CDate(Common.WebRequest.Data("DateFrom"))

            If BinID Is Nothing Then
                BinID = GetDefaultBIN(ProductID, FOWSTID)
                If BinID = vbNullString Then
                    BinID = Guid.NewGuid.ToString
                End If
            End If

            'recompute previous trasaction records

            Dim pDTPreviousTransactions As DataTable
            Dim PreviousBalance As Double
            'Dim pQty As Decimal
            'Dim pBalance As Decimal

            'If Not String.IsNullOrWhiteSpace(DateFrom) Then
            '    pDTPreviousTransactions = GetPreviousInventoryLedgerTransactionCount(DateFrom, FOWSTID, BinID, ProductID)

            '    If pDTPreviousTransactions.Rows.Count > 0 Then

            '        PreviousBalance = GetInventoryLedgerPreviousBalance(DateFrom, FOWSTID, BinID, ProductID)

            '        If vTransactionType = eTransType.InventoryCount Then

            '            pQty = vBalance - PreviousBalance

            '            PreviousBalance = pBalance
            '        Else
            '            PreviousBalance += pQty

            '            pBalance = PreviousBalance
            '        End If

            '    End If
            'End If

            Dim pTransDate As String

            pTransDate = ProSqlite.ReadyDate(DateFrom)

            pDTPreviousTransactions = GetPreviousInventoryLedgerTransactionCount(pTransDate, FOWSTID, BinID, ProductID)

            PreviousBalance = BegBalance

            If pDTPreviousTransactions IsNot Nothing Then
                For i As Integer = 0 To pDTPreviousTransactions.Rows.Count - 1
                    If pDTPreviousTransactions.Rows(i).Item("TransactionType") = "InventoryCount" Then

                        Dim InventoryCountMovement As Double

                        InventoryCountMovement = CDec2(pDTPreviousTransactions.Rows(i).Item("Balance")) - PreviousBalance
                        PreviousBalance = pDTPreviousTransactions.Rows(i).Item("Balance")

                        pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & ", Qty = " & InventoryCountMovement & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                        mProData.ExecuteSQL(pSql)
                    Else
                        PreviousBalance += CDbl(pDTPreviousTransactions.Rows(i).Item("Qty"))

                        pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                        mProData.ExecuteSQL(pSql)
                    End If
                Next
            End If

            ''get previous balance, for computing variance
            'pSql = "Select top 1 Balance from TransInventoryLedger where " & vbCrLf &
            '        "FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
            '        " AND BinID = " & ReadyString(BinID) & vbCrLf &
            '        " AND ProductID = " & ReadyString(ProductID) & vbCrLf &
            '        " AND LastUpdated < " & ReadyString(DateFrom) & vbCrLf &
            '        " order by LastUpdated desc"

            'PrevBalance = CDec2(mProData.ExecuteScalar(pSql))

            'get first record
            pSql = "Select top 1 * from TransInventoryLedger where " & vbCrLf &
                    "FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                    " AND BinID = " & ReadyString(BinID) & vbCrLf &
                    " AND ProductID = " & ReadyString(ProductID) & vbCrLf &
                    " AND LastUpdated >= " & ReadyString(DateFrom) & vbCrLf &
                    " order by LastUpdated"

            pDTFirstCurrentRecord = mProData.Execute(pSql)

            'Beggining Balance
            pSql = "Insert into TransInventoryLedger(ProductID, FowstID, BinID, TransactionID, TransactionType, " &
                    "Qty, Balance, UserID, LastUpdated, TransactionDate) values(" &
                    ReadyString(ProductID) & "," &
                    ReadyString(FOWSTID) & "," &
                    ReadyString(BinID) & "," &
                    ReadyString(Guid.NewGuid.ToString) & "," &
                    ReadyString("Beginning Balance") & "," &
                    CDec2(BegBalance - PrevBalance) & "," &
                    BegBalance & "," &
                    ReadyString(WebSession.Instance.PeopleUserID) & "," &
                    ReadyString(DateFrom) & "," &
                    ReadyString(DateFrom) &
                    ")"

            mProData.Execute(pSql)

            ''' LibraryFowstBinProduct Table
            ''' 
            Dim dtFowstBinProduct As DataTable
            Dim pIU As InsertUpdate
            Dim IsNewFowstBinProduct As Boolean 'important for Fowst Product computation
            Dim pOut As String

            Dim mSql As String = "Select Surname + ', ' + Firstname from MobizLibraryPeople where ID =  " & ReadyString(WebSession.Instance.PeopleUserID)
            Dim mName As String = CStr2(Common.WebRequest.ProData.ExecuteScalar(mSql))

            dtFowstBinProduct = GetDataFromLibraryFowstBinProduct(ProductID, FOWSTID, BinID)

            pIU = New InsertUpdate("LibraryFowstBinProduct")

            pIU("Balance") = BegBalance
            pIU("LastUpdated") = "GETDATE()"
            pIU("LastInventoryCountDate") = ReadyString(DateFrom)
            pIU("LastInventoryCountBy") = ReadyString(mName)

            If dtFowstBinProduct.Rows.Count > 0 Then

                pSql = pIU.GetUpdate(" ID = " & ReadyString(CStr2(dtFowstBinProduct.Rows(0).Item("ID"))))

            Else

                IsNewFowstBinProduct = True

                pOut = Guid.NewGuid().ToString()

                pIU("ID") = ReadyString(pOut)
                pIU("FowstID") = ReadyString(FOWSTID)
                pIU("BinID") = ReadyString(BinID)
                pIU("ProductID") = ReadyString(ProductID)

                pSql = pIU.GetInsert()

            End If

            mProData.ExecuteSQL(pSql)

            WebRequest.Instance.Business.TransInventoryLedger.UpdateLibraryFowstBinProductDetails(FOWSTID)

            Return JsonDictionary.Serialize("Results", "Success")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", "Error")
        End Try
    End Function

    Function GetDefaultBIN(ByVal vProductID As String, ByVal vFOWSTID As String) As String
        Dim pSql As String

        pSql = "Select BinID from LibraryFowstBinProduct where IsDefault = 1 AND ProductID = " & ReadyString(vProductID) & " AND FOWSTID = " & ReadyString(vFOWSTID)

        Return CStr2(mProData.ExecuteScalar(pSql))

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

    Function InventoryBalance() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function GetStockUnitMeasures() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryPIMMeasure.ID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With
                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductMeasurement.Quantity"
                End With
                'With .Add("Price", SqlDbType.VarChar)
                '    .Expression = "MobizLibraryPIMproductPriceLevel.Price"
                'End With

            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProductMeasurement INNER JOIN " & vbCrLf &
                  "dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID INNER JOIN " & vbCrLf &
                  "dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID " & vbCrLf
            '"inner join MobizLibraryPIMproductPriceLevel on MobizLibraryPIMproductPriceLevel.ProductID = MobizLibraryPIMProductMeasurement.ProductID " & vbCrLf &
            '"inner join MobizLibraryPIMPriceLevel on MobizLibraryPIMPriceLevel.ID = MobizLibraryPIMproductPriceLevel.PriceLevelID"

            .SqlWhere = "WHERE dbo.MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID"))
            '" and MobizLibraryPIMPriceLevel.PriceLevel='Retail Price' AND MobizLibraryPIMproductPriceLevel.OrganizationID =" & ReadyString(WebRequest.Instance.OrgID)

            .SqlOrder = "Order by Quantity"
            pDT = .Requery

        End With

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    Function SetZeroBeginningBalance() As ActionResult
        Dim pSql As String
        Dim dtProducts As DataTable
        Dim dtFOWST As DataTable

        If WebRequest.Instance.HandleAccess = False Then Exit Function
        If WebRequest.Instance.HandleOrganization(True) = False Then Return View()

        'pSql = "Select DISTINCT ProductID from LibraryOrganizationPIM where ProductID not in (select ProductID from TransInventoryLedger) order by ProductID"
        pSql = "Select DISTINCT ProductID from LibraryOrganizationPIM " 'where ProductID not in (select ProductID from TransInventoryLedger) order by ProductID

        dtProducts = mProData.Execute(pSql)

        'query string ang gamit kay wala pa nastore sa session ang orgid

        pSql = "Select ID from MobizLibraryOrganizationFOWST where organizationid = " & ReadyString(Common.WebRequest.QueryString("OrgID"))
        dtFOWST = mProData.Execute(pSql)

        If dtProducts.Rows.Count > 0 AndAlso dtFOWST.Rows.Count > 0 Then

            For i As Integer = 0 To dtFOWST.Rows.Count - 1

                For j As Integer = 0 To dtProducts.Rows.Count - 1

                    pSql = "Select * FROM TransInventoryLedger where ProductID = " & ReadyString(CStr2(dtProducts.Rows(j).Item("ProductID"))) & " AND FOWSTID = " & ReadyString(CStr2(dtFOWST.Rows(i).Item("ID")))

                    Dim dtIsExist As DataTable

                    dtIsExist = mProData.Execute(pSql)

                    If dtIsExist.Rows.Count = 0 Then
                        WebRequest.Instance.Business.TransInventoryLedger.Add( _
                       vStockID:=CStr2(dtProducts.Rows(j).Item("ProductID")), _
                        vFowstID:=CStr2(dtFOWST.Rows(i).Item("ID")), _
                        vTransactionID:=Guid.NewGuid.ToString, _
                        vTransactionType:=TransInventoryLedger.eTransType.BeginningBalance, _
                        vQty:=0.0,
                        vUserID:=WebSession.Instance.PeopleUserID
                    )
                    End If


                Next

            Next

        End If

        Return View()

    End Function

    Function GetCategories() As String
        Dim pDT As DataTable
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pSql = "SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category] " & vbCrLf &
                " FROM MobizLibraryPIMProductVariableFeatures001 INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.id = LibraryOrganizationPIM.ProductID " & vbCrLf &
                " where LibraryOrganizationPIM.IsDeactivated=0"

        pDT = mProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)
    End Function

    Private Function GetPreviousInventoryLedgerTransactionCount(vTransDate, vFowstID, vBinID, vProductID) As DataTable
        Dim pSql As String

        pSql = "Select * from TransInventoryLedger " & vbCrLf &
                "WHERE TransactionDate > " & vTransDate & vbCrLf &
                " AND BinID = " & ReadyString(vBinID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                " ORDER BY TransactionDate"
        Return mProData.Execute(pSql)
    End Function

    Function UpdateInventoryBalance() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        WebRequest.Instance.Business.TransInventoryLedger.UpdateLibraryFowstBinProductDetails(CStr2(Common.WebRequest.Data("FOWSTID")))

        Return True
    End Function

    Private Function GetDataFromLibraryFowstBinProduct(ByVal vStockID As String, ByVal vFowstID As String, ByVal vBinID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT TOP 1 *" & vbCrLf &
            " FROM LibraryFowstBinProduct" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND BinID = " & ReadyString(vBinID) & vbCrLf

        Return mProData.Execute(pSql)
    End Function

    Public Function ExportPimSalesCachingToJson() As String

        ' If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pFowstID = WebRequest.Instance.Data("FowstID")
        If String.IsNullOrWhiteSpace(pFowstID) Then
            Return JsonDictionary.Serialize("Error", "FowstID must not be blank. ref: vbPVDV8FJHM9! ")
        End If
        Dim pDT = WebRequest.Instance.Business.TransInventoryLedger.GetTransPimSalesCaching(pFowstID, Date.Now.AddDays(-7))

        Dim pOut = New JsonDictionary()
        pOut.Add("TransPimSalesCaching", pDT)

        pOut.Add("Results", "ok")
        Return pOut.Serialize()

    End Function
End Class

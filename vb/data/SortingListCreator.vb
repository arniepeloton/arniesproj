' ReSharper disable once CheckNamespace
Public Class SortingListCreator

    Private Function GetCreatables() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT TransInvoiceItemWarehouseSections.InvoiceID" & vbCrLf &
            " ,TransInvoice.InvoiceNumber" & vbCrLf &
            " ,TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransInvoice.InvoiceTypeCode" & vbCrLf &
            " ,TransInvoice.CustomerID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.SalesOrderID" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " FROM TransInvoiceItemWarehouseSections" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItemWarehouseSections.InvoiceID" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
            " WHERE TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.ReadyForInvoicingDate IS NOT NULL" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.SortlingListID IS NULL" & vbCrLf &
            " AND TransInvoice.SalesOrderID IS NOT NULL" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetCreatables_old() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT tblConfirmed.Count" & vbCrLf &
            " ,tblTotal.Count" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.ID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT COUNT(*) AS Count" & vbCrLf &
            "     FROM TransInvoiceItems" & vbCrLf &
            "     WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "     AND TransInvoiceItems.IsQtyConfirmed = 1" & vbCrLf &
            " ) tblConfirmed" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT COUNT(*) AS Count" & vbCrLf &
            "     FROM TransInvoiceItems" & vbCrLf &
            "     WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " ) tblTotal" & vbCrLf &
            " WHERE tblConfirmed.Count = tblTotal.Count" & vbCrLf &
            " AND tblConfirmed.Count > 0" & vbCrLf &
            " AND NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM wholesale.dbo.TransStockSortingList TransStockSortingList" & vbCrLf &
            "     WHERE TransStockSortingList.SourceMobizInvoiceID = TransInvoice.ID" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CreateSortingLists() As String

        Dim pDtInvoices As DataTable
        Dim pGenerateds As String = ""

        pDtInvoices = GetCreatables()
        For Each pDrCreateable As DataRow In pDtInvoices.Rows
            If pGenerateds <> "" Then
                pGenerateds &= ", "
            End If

            pGenerateds &= CreateSortingList(CStr2(pDrCreateable("InvoiceID")), CStr2(pDrCreateable("SalesOrderID")))
        Next pDrCreateable

        Return pGenerateds

    End Function

    ''' <summary>
    ''' return the SortingListNo of the newly created wholesale.TransStockSortingList
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateSortingList(vMobizInvoiceID As String, vDucSalesOrderID As String) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String
        Dim pNum As Integer
        Dim pDR As DataRow

        Common.WebRequest.ProData.BeginTransaction()
        pIU = New InsertUpdate("wholesale.dbo.TransStockSortingList")
        pID = ReadyGuid(Guid.NewGuid)
        pIU("ID") = ReadyString(pID)
        pIU("StockPickingListID") = ReadyString(Guid.Empty)

        ' no need for these. SortingListNo is an auto-increment field
        'pNum = GetMaxSortingListNum() + 1
        'pIU("SortingListNo") = pNum

        pIU("SortingListDate") = "GETDATE()"
        ' ReSharper disable once RedundantQualifier
        pIU("UserID") = modClient.EmployeeWebNumericID
        pIU("LastUpdated") = "GETDATE()"
        pIU("SourceMobizInvoiceID") = ReadyString(vMobizInvoiceID)
        pSql = pIU.GetInsert()
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        SaveSortingListItem(vMobizInvoiceID, vDucSalesOrderID, pID)

        pDR = GetSortingList(pID)
        If pDR IsNot Nothing Then
            pNum = CDec2(pDR("SortingListNo"))

            pIU = New InsertUpdate("TransInvoiceItemWarehouseSections")
            pIU("SortlingListID") = ReadyString(pID)
            pSql = pIU.GetUpdate("TransInvoiceItemWarehouseSections.InvoiceID = " & ReadyString(vMobizInvoiceID) & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.ReadyForInvoicingDate IS NOT NULL" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.SortlingListID IS NULL")
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' the output is for debugging only
        End If
        Common.WebRequest.ProData.CommitTransaction()

        Return pNum

    End Function

    Private Function GetMaxSortingListNum() As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = " SELECT MAX(SortingListNo) AS SortingListNo" & vbCrLf &
            " FROM wholesale.dbo.TransStockSortingList"
        pOut = CDec2(Common.WebRequest.ProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Private Sub SaveSortingListItem(ByVal vMobizInvoiceID As String, vDucSalesOrderID As String, _
            ByVal vSortingListID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDtSource As DataTable

        pDtSource = GetInvoiceItems(vMobizInvoiceID)
        For Each pDrSource As DataRow In pDtSource.Rows
            pIU = New InsertUpdate("wholesale.dbo.TransStockSortingListItems")
            pIU("StockSortingListID") = ReadyString(vSortingListID)
            pIU("SalesOrderID") = ReadyString(vDucSalesOrderID)
            pIU("StockID") = ReadyString(pDrSource("ProductID"))
            If pDrSource("RefStockID") IsNot DBNull.Value Then
                pIU("RefStockID") = ReadyString(pDrSource("RefStockID"))
            End If
            pIU("Quantity") = CDec2(pDrSource("Quantity"))
            pIU("ActualQuantity") = ReadyString(pDrSource("ActualQuantity"))
            pIU("UnitMeasure") = ReadyString(pDrSource("UnitMeasure"))
            pIU("UnitPrice") = CDec2(pDrSource("UnitPrice"))
            pIU("PriceLevelID") = ReadyString(pDrSource("PriceLevelID"))

            pSql = pIU.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Next pDrSource

    End Sub

    Private Function GetInvoiceItems(vInvoiceID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
            " ,NULL AS RefStockID" & vbCrLf &
            " ,TransInvoiceItemWarehouseWithdrawal.PreparedQuantity AS Quantity" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            " FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " 	SELECT TOP 1 TransInvoiceItems.UnitMeasure" & vbCrLf &
            "     ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "     ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "     ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            "     FROM TransInvoiceItems" & vbCrLf &
            "     WHERE TransInvoiceItems.InvoiceID = TransInvoiceItemWarehouseWithdrawal.InvoiceID" & vbCrLf &
            "     AND TransInvoiceItems.ProductID = TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
            "     AND TransInvoiceItems.ActualQuantity = TransInvoiceItemWarehouseWithdrawal.UnitQty" & vbCrLf &
            " ) AS TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoiceItemWarehouseSections" & vbCrLf &
            " ON TransInvoiceItemWarehouseSections.InvoiceID = TransInvoiceItemWarehouseWithdrawal.InvoiceID" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.ProductID = TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.ReadyForInvoicingDate IS NOT NULL" & vbCrLf &
            " AND TransInvoiceItemWarehouseSections.SortlingListID IS NULL" & vbCrLf &
            " WHERE TransInvoiceItemWarehouseWithdrawal.InvoiceID = " & ReadyString(vInvoiceID)
        pOut = Common.WebRequest.ProData.Execute(pSql)

        Return pOut

    End Function

    Private Function GetInvoiceItems_old(vInvoiceID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT ProductID" & vbCrLf &
            " ,RefStockID" & vbCrLf &
            " ,Quantity" & vbCrLf &
            " ,ActualQuantity" & vbCrLf &
            " ,UnitMeasure" & vbCrLf &
            " ,UnitPrice" & vbCrLf &
            " ,PriceLevelID" & vbCrLf &
            " FROM TransInvoiceItems" &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        pOut = Common.WebRequest.ProData.Execute(pSql)

        Return pOut

    End Function

    Private Function GetSortingList(vID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT SortingListNo" & vbCrLf &
            " FROM wholesale.dbo.TransStockSortingList" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class

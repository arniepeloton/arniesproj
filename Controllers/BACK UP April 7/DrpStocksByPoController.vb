Public Class DrpStocksByPoController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pOut As JsonDictionary
        Dim pDT As DataTable
        Dim pSql As String
        Dim pWeeksSum As String
        Dim pFirmOrderWeeks As Integer
        Dim pFinder As Finder

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pFirmOrderWeeks = WebRequest.Instance.Business.SystemDefaults.FirmOrderWeeks
        pOut = New JsonDictionary
        pWeeksSum = "ISNULL(DrpCacheDrpTable.PastDue, 0)"

        pFinder = New Finder
        pFinder.MaxRecords = 1000
        pFinder.SearchString = Common.WebRequest.Data("txtPpoVendorSearch")
        With pFinder.Fields
            With .Add("DrpCacheDrpStocks.SupplierID", SqlDbType.VarChar)
                .NoAlias = True
            End With
            With .Add("PastDue", SqlDbType.Int)
                .Expression = "SUM(DrpCacheDrpTable.PastDue)"
                .Searchable = False
            End With
            For p As Integer = 1 To pFirmOrderWeeks
                With .Add("Week" & Format(p, "00"), SqlDbType.Int)
                    .Expression = "SUM(DrpCacheDrpTable.Week" & Format(p, "00") & ")"
                    .Searchable = False
                End With

                If pWeeksSum.Length > 0 Then pWeeksSum &= "+"
                pWeeksSum &= "ISNULL(DrpCacheDrpTable.Week" & Format(p, "00") & ", 0)"
            Next p
            With .Add("SupplierName", SqlDbType.VarChar)
                .Expression = "LibrarySuppliers.SupplierName"
            End With
            With .Add("Amount", SqlDbType.Int)
                .Expression = "(SELECT SUM(DrpCacheDrpStockUnits.Qty * DrpCacheDrpStockUnits.Price)" &
                    " FROM DrpCacheDrpStockUnits" &
                    " WHERE DrpCacheDrpStockUnits.SupplierID = DrpCacheDrpStocks.SupplierID" &
                    " AND DrpCacheDrpStockUnits.WeekNum <= " & pFirmOrderWeeks &
                    ")"
                .Searchable = False
            End With
        End With
        pFinder.SqlFrom = " FROM DrpCacheDrpTable" & vbCrLf & _
            " JOIN DrpCacheDrpStocks ON DrpCacheDrpStocks.StockID = DrpCacheDrpTable.StockID" & vbCrLf & _
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpCacheDrpStocks.SupplierID" & vbCrLf
        pFinder.SqlWhere = " WHERE DrpCacheDrpTable.Text = 'Planned Orders'" & vbCrLf
        pFinder.SqlGroup = " GROUP BY DrpCacheDrpStocks.SupplierID" &
            ",LibrarySuppliers.SupplierName" & vbCrLf
        pFinder.SqlHaving = " HAVING SUM(" & pWeeksSum & ") > 0" & vbCrLf
        pFinder.SqlOrder = " ORDER BY " & pFinder.Fields("Amount").Expression & " DESC"
        pSql = pFinder.CreateSql
        pDT = Common.WebRequest.ProData.Execute(pSql)
        With pDT.Columns
            .Add("PpoCount", GetType(Integer))
            .Add("WeekNums") ' comma-delimited
        End With

        For Each pDR As DataRow In pDT.Rows
            pDR("WeekNums") = ""
            If CDec2(pDR("PastDue")) > 0 Then
                pDR("PpoCount") = CDec2(pDR("PpoCount")) + 1
                If CStr2(pDR("WeekNums")).Length > 0 Then pDR("WeekNums") &= ","
                pDR("WeekNums") &= 0
            End If
            For p As Integer = 1 To pFirmOrderWeeks
                If CDec2(pDR("Week" & Format(p, "00"))) > 0 Then
                    pDR("PpoCount") = CDec2(pDR("PpoCount")) + 1
                    If CStr2(pDR("WeekNums")).Length > 0 Then pDR("WeekNums") &= ","
                    pDR("WeekNums") &= p
                End If
            Next p
        Next pDR
        pOut.Add("Data", pDT)

        Return pOut.Serialize

    End Function
End Class

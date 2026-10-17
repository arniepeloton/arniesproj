Public Class DrpPpoController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSupplierID As String
        Dim pWeek As Integer
        Dim pOut As JsonDictionary
        Dim pDate As Date?

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pOut = New JsonDictionary
        With Regex.Match(Common.WebRequest.Data("q"), "\?s=(?<SupplierID>.+)&w=(?<Week>\d+)")
            If .Success = False Then
                Return JsonDictionary.Serialize("Error", "Invalid link")
            End If
            pSupplierID = .Groups("SupplierID").Value
            If IsGuid(pSupplierID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid link. Invalid supplier ID")
            End If
            pSupplierID = ReadyGuid(pSupplierID)
            If Integer.TryParse(.Groups("Week").Value, pWeek) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid link. Invalid week number")
            End If
            If pWeek < 0 OrElse 15 < pWeek Then
                Return JsonDictionary.Serialize("Error", "Invalid link. Invalid week number. Out of range")
            End If
        End With

        pDate = WebRequest.Instance.Business.TransStockPurchase.DrpWeek(pWeek)
        If pDate Is Nothing Then
            Return JsonDictionary.Serialize("Error", "No cache data")
        End If
        pOut("PoDate") = Format(pDate, cGlobal.FormatDate)
        If pWeek = 0 Then pOut("IsPastDue") = 1
        ' header
        pSql = "SELECT LibrarySuppliers.SupplierName" & vbCrLf & _
            ",LibrarySuppliers.ContactNumber AS SupplierContactNumber" & vbCrLf &
            ",LibraryShipper.ShipperName" & vbCrLf & _
            " FROM DrpCacheDrpStocks " & vbCrLf & _
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpCacheDrpStocks.SupplierID" & vbCrLf & _
            " LEFT JOIN LibraryShipper ON LibraryShipper.ID = DrpCacheDrpStocks.ShipperID" & vbCrLf & _
            " WHERE DrpCacheDrpStocks.SupplierID = " & ReadyString(pSupplierID) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Invalid data. Please recache")
        End If
        pOut("SupplierID") = pSupplierID
        pOut("SupplierName") = pDT.Rows(0)("SupplierName")
        pOut("ShipperName") = pDT.Rows(0)("ShipperName")
        pOut("SupplierContactNumber") = pDT.Rows(0)("SupplierContactNumber")
        pOut("WeekNum") = pWeek

        ' supplier addresses
        pSql = "SELECT LibrarySuppliersAddress.Street + ', ' + dbo.LibrarySuppliersAddress.District + ', ' + dbo.LibraryLocations.LocationName AS Address" & vbCrLf & _
            " FROM LibrarySuppliersAddress " & vbCrLf & _
            " LEFT JOIN LibraryLocations ON dbo.LibraryLocations.ID = dbo.LibrarySuppliersAddress.LocationID " & vbCrLf & _
            " WHERE LibrarySuppliersAddress.SupplierID = " & ReadyString(pSupplierID)
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pSql = ""
        For Each pDR As DataRow In pDT.Rows
            If pSql.Length > 0 Then pSql &= vbCrLf
            pSql &= pDR("Address")
        Next pDR
        pOut("SupplierAddresses") = pSql

        ' stocks
        pDT = GetStocks(pSupplierID, pWeek)
        pOut.Add("Stocks", pDT)
        pOut.Add("Total", CDec2(pDT.Compute("SUM(SubTotal)", "")))

        Return pOut.Serialize

    End Function

    Private Function GetStocks(vSupplierID As String, vWeekNum As Integer) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DrpCacheDrpStockUnits.StockID" &
            ",DrpCacheDrpStockUnits.Qty" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & _
            ",DrpCacheDrpStockUnits.Price" &
            ",DrpCacheDrpStockUnits.Unit + ' (' + CONVERT(varchar, DrpCacheDrpStockUnits.UnitQty) + ')' AS UnitMeasure" &
            ",DrpCacheDrpStockUnits.UnitQty" &
            ",DrpCacheDrpStockUnits.Unit" &
            ",DrpCacheDrpStockUnits.SupplierPriceDate" &
            ",DrpCacheDrpStockUnits.UpdatedByName" &
            ",DrpCacheDrpStockUnits.Qty * DrpCacheDrpStockUnits.Price AS SubTotal" & vbCrLf

        pSql &= " FROM DrpCacheDrpStockUnits" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheDrpStockUnits.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" &
            " WHERE DrpCacheDrpStockUnits.SupplierID = " & ReadyString(vSupplierID) &
            " AND DrpCacheDrpStockUnits.WeekNum = " & vWeekNum
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    '<HttpPost()>
    'Public Function CreatePo() As String

    '    Dim pDtStocks As DataTable
    '    Dim pSupplierID As String
    '    Dim pStr As String
    '    Dim pWeekNum As Integer
    '    Dim pSql As String
    '    Dim pDT As DataTable
    '    Dim pDR As DataRow
    '    Dim pDate As Date?

    '    If WebSession.Instance.UserAccess Is Nothing _
    '        OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsNew OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

    '        Return JsonDictionary.Serialize("Error", "Access denied")
    '    End If

    '    pSupplierID = Common.WebRequest.Data("SupplierID")
    '    If IsGuid(pSupplierID) = False Then
    '        Return JsonDictionary.Serialize("Error", "Invalid supplier ID")
    '    End If
    '    pStr = Common.WebRequest.Data("WeekNum")
    '    If Integer.TryParse(pStr, pWeekNum) = False Then
    '        Return JsonDictionary.Serialize("Error", "Invalid week number")
    '    End If
    '    If pWeekNum < 0 OrElse 15 < pWeekNum Then
    '        Return JsonDictionary.Serialize("Error", "Invalid link. Invalid week number. Out of range")
    '    End If

    '    pDtStocks = GetStocks(pSupplierID, pWeekNum)
    '    If pDtStocks.Rows.Count = 0 Then
    '        Return JsonDictionary.Serialize("Error", "No stocks to PO")
    '    End If

    '    Dim pPoCreator As TransStockPurchase.PoCreator

    '    pPoCreator = New TransStockPurchase.PoCreator(WebRequest.Instance.Business)

    '    ' header
    '    pSql = "SELECT LibrarySuppliers.SupplierName" & vbCrLf & _
    '        ",LibrarySuppliers.ContactNumber AS SupplierContactNumber" & vbCrLf &
    '        ",DrpCacheDrpStocks.ShipperID" &
    '        ",LibraryShipper.ShipperName" & vbCrLf & _
    '        " FROM DrpCacheDrpStocks " & vbCrLf & _
    '        " JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpCacheDrpStocks.SupplierID" & vbCrLf & _
    '        " LEFT JOIN LibraryShipper ON LibraryShipper.ID = DrpCacheDrpStocks.ShipperID" & vbCrLf & _
    '        " WHERE DrpCacheDrpStocks.SupplierID = " & ReadyString(pSupplierID) & vbCrLf
    '    pDT = Common.WebRequest.ProData.Execute(pSql)
    '    If pDT.Rows.Count = 0 Then
    '        Return JsonDictionary.Serialize("Error", "Not cached")
    '    End If
    '    pPoCreator.ShipperID = CStr2(pDT.Rows(0)("ShipperID"))
    '    WebRequest.Instance.Business.TransStockPurchase.CacheInfo(Nothing)
    '    pPoCreator.DeliverAddressID = CStr2(WebRequest.Instance.Business.TransStockPurchase.MostCommonDeliveryAddress("ID"))
    '    pPoCreator.BillingAddressID = CStr2(WebRequest.Instance.Business.TransStockPurchase.MostCommonBillingAddress("ID"))

    '    pPoCreator.SupplierID = pSupplierID
    '    pPoCreator.ShipperID = CStr2(pDT.Rows(0)("ShipperID"))
    '    pDate = WebRequest.Instance.Business.TransStockPurchase.DrpWeek(pWeekNum)
    '    If pDate Is Nothing Then pDate = Now
    '    pPoCreator.DateNeeded = pDate.Value.AddDays(1)
    '    pDate = pDate.Value.AddDays(CDec2(WebRequest.Instance.Business.SystemDefaults.GetOrAddSystemDefault("Stock Purchasing Leadtime", 7)))
    '    pPoCreator.CancellationDate = pDate
    '    pPoCreator.DtStocks = pDtStocks

    '    pDR = WebRequest.Instance.Business.LibrarySuppliers.MostUsedAddress(pSupplierID)
    '    If pDR Is Nothing Then
    '        Return JsonDictionary.Serialize("Error", "Supplier has no address")
    '    End If
    '    pPoCreator.SupplierAddressID = CStr2(pDR("ID"))
    '    pPoCreator.Remarks = Truncate(Common.WebRequest.Data("txtRemarks"), 500)
    '    pPoCreator.Create()

    '    '// update drp table for this item
    '    Dim p As CacheDrpTables

    '    p = New CacheDrpTables(WebRequest.Instance.Business)
    '    pDtStocks = pDtStocks.DefaultView.ToTable(True, "StockID") ' get the unique stock IDs
    '    pDtStocks = ExtractFieldValues(pDtStocks, "StockID")
    '    For Each pDR In pDtStocks.Rows
    '        p.CacheDrpStocks(pDR("StockID"))
    '    Next pDR

    '    Return JsonDictionary.Serialize("Result", "ok")

    'End Function
End Class

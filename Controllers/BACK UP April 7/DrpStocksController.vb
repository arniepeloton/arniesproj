Public Class DrpStocksController
    Inherits System.Web.Mvc.Controller

#Region " index "
    Function Index() As ActionResult

        '#If DEBUG Then
        '        Dim pDT As DataTable
        '        pDT = New DataTable
        '        pDT.ReadXml("C:\Users\user\Desktop\LibraryUsers.xml")
        '        Common.WebRequest.ProData.CreateTable("LibraryUsers", pDT)
        '#End If

        If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        App.RunAgent()

        'With New Synonyms()
        '    .ProData = Common.WebRequest.ProData
        '    .SourceDatabase = "Drp"
        '    .Generate()
        'End With

#If DEBUG Then

#End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pDtData As DataTable
        Dim pDtDates As DataTable
        Dim pDrDate As DataRow
        Dim pOut As JsonDictionary
        Dim pSql As String
        Dim pWebSession As WebSession
        Dim pEndUpdate As Date?
        Dim pFinder As Finder

        If WebSession.Instance.UserAccess Is Nothing _
            OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

            Return JsonNode.Serialize("Error", "Access denied")
        End If

        If App.FinishedRun = False Then
            Return JsonDictionary.Serialize("Result", "In progress")
        End If

        ''// check if in-progress
        'If App.DrpCacherSessionID IsNot Nothing Then
        '    If MvcApplication.Instance.Sessions.ContainsKey(App.DrpCacherSessionID) Then
        '        pWebSession = CType(MvcApplication.Instance.Sessions(App.DrpCacherSessionID)("WebSession"), WebSession)
        '        If pWebSession.DrpCacheStart IsNot Nothing Then
        '            Return JsonDictionary.Serialize("Result", "Caching of DRP data is on-going. Started: " & pWebSession.DrpCacheStart)
        '        End If
        '    End If
        'End If

        ''// check if updated
        'pEndUpdate = WebRequest.Instance.Business.TransStockPurchase.DrpCacheEndDate
        'If pEndUpdate Is Nothing _
        '    OrElse pEndUpdate.Value.Date <> Now.Date Then

        '    ' cached date is outdated
        '    pWebSession = WebSession.Instance
        '    App.DrpCacherSessionID = Session.SessionID
        '    pWebSession.DrpCacheStart = Now
        '    'App.CacheDrp()
        '    'TimedThread( _
        '    '    Sub()
        '    '        Dim pCacheDrp As CacheDrpTables
        '    '        pCacheDrp = New CacheDrpTables(New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo)))
        '    '        pCacheDrp.CacheDrp()
        '    '        pWebSession.DrpCacheStart = Nothing
        '    '    End Sub)
        '    Return JsonDictionary.Serialize("Result", "Caching of DRP data has started. Started: " & pWebSession.DrpCacheStart & ". Please check again after 5 minutes")
        'End If

        Try
            pOut = New JsonDictionary
            pSql = "SELECT * FROM DrpCacheDrpSettings"
            pDtData = Common.WebRequest.ProData.Execute(pSql)
            If pDtData.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "No cached records")
            End If
            pOut("WeekNumber") = WeekNumber(pDtData.Rows(0)("Week01Start"))
            pDtDates = New DataTable
            With pDtDates.Columns
                .Add("NowDateRange")
                .Add("DateStart", GetType(Date))
                .Add("DateEnd", GetType(Date))
            End With
            For p As Integer = 1 To 15
                If pDtData.Rows(0)("Week" & Format(p, "00") & "Start") Is DBNull.Value Then Exit For
                pDrDate = pDtDates.NewRow
                pDrDate("DateStart") = pDtData.Rows(0)("Week" & Format(p, "00") & "Start")
                pDrDate("DateEnd") = pDtData.Rows(0)("Week" & Format(p, "00") & "End")
                pDrDate("NowDateRange") = DisplayableDateRange(pDrDate("DateStart"), pDrDate("DateEnd"))
                pDtDates.Rows.Add(pDrDate)
            Next p

            pFinder = New Finder
            pFinder.MaxRecords = 1000
            pFinder.SearchString = Common.WebRequest.Data("txtDrpStocksSearch")
            With pFinder.Fields
                With .Add("DrpCacheDrpTable.*", SqlDbType.VarChar)
                    .NoAlias = True
                End With
                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "LibraryStockInformation.StockNumber"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)), '') + " & _
                        "   ISNULL(', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,') + " & _
                        "   ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,') + " & _
                        "   ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,') + " & _
                        "   ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', ')"
                End With
                With .Add("QtyAvailableToCommit", SqlDbType.Int)
                    .Expression = "DrpCacheDrpStocks.QtyOnHand - DrpCacheDrpStocks.QtyCommitted"
                    .Searchable = False
                End With
                With .Add("SafetyStocks", SqlDbType.Int)
                    .Expression = "DrpCacheDrpStocks.SafetyStocks"
                    .Searchable = False
                End With
                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = "LibrarySuppliers.SupplierName"
                End With
            End With
            pFinder.SqlFrom = " FROM DrpCacheDrpTable" &
                " JOIN DrpCacheDrpStocks ON DrpCacheDrpStocks.StockID = DrpCacheDrpTable.StockID" &
                " LEFT JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpCacheDrpStocks.SupplierID" &
                " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheDrpTable.StockID" & _
                " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
                " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID"
            pFinder.SqlWhere = " WHERE DrpCacheDrpTable.Text = 'Planned Orders'" &
                " AND DrpCacheDrpStocks.SupplierID IS NOT NULL" &
                " AND DrpCacheDrpStocks.ShipperID IS NOT NULL"
            pSql = pFinder.CreateSql

            'pSql = "SELECT DrpCacheDrpTable.*" &
            '    ",LibraryStockInformation.StockNumber" &
            '    ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            '    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            '    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            '    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            '    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & _
            '    ",DrpCacheDrpStocks.QtyOnHand - DrpCacheDrpStocks.QtyCommitted AS QtyAvailableToCommit" &
            '    ",DrpCacheDrpStocks.SafetyStocks" &
            '    ",LibrarySuppliers.SupplierName" &
            '    " FROM DrpCacheDrpTable" &
            '    " JOIN DrpCacheDrpStocks ON DrpCacheDrpStocks.StockID = DrpCacheDrpTable.StockID" &
            '    " LEFT JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpCacheDrpStocks.SupplierID" &
            '    " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheDrpTable.StockID" & _
            '    " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            '    " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" &
            '    " WHERE DrpCacheDrpTable.Text = 'Planned Orders'" &
            '    " AND DrpCacheDrpStocks.SupplierID IS NOT NULL" &
            '    " AND DrpCacheDrpStocks.ShipperID IS NOT NULL"
            pDtData = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("Data", pDtData)
            pOut.Add("Dates", pDtDates)
            pOut.Add("Request_Url_Authority", Request.Url.Authority)
            Return pOut.Serialize
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
#End Region

#Region " details "
    'test: http://localhost:3002/DrpStocks/Details/7f9599ac-4d27-4fba-9c10-7f6666950789?rc=1
    Public Function Details(id As String) As ActionResult

        If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        If IsGuid(id) = False Then
            Return Content("Invalid stock ID")
        End If
        id = ReadyGuid(id)

        If CDec2(Request.QueryString("rc")) = 1 Then
            Dim p As CacheDrpTables
            p = New CacheDrpTables(WebRequest.Instance.Business)
            p.CacheDrpStocks(ReadyString(id))
        End If

        ViewData("StockID") = ReadyGuid(id)

        Return View()

    End Function

    <HttpPost()>
    Public Function GetDetails() As String

        Dim pStockID As String
        Dim pDR As DataRow
        Dim pOut As JsonDictionary
        Dim pDsDrpTable As DataSet
        Dim pDtSuppliers As DataTable
        Dim pCustomerID As String ' brach's customer account
        Dim pDrShipper As DataRow

        If WebSession.Instance.UserAccess Is Nothing _
         OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

            Return JsonNode.Serialize("Error", "Access denied")
        End If

        pStockID = Common.WebRequest.Data("StockID")
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Situational internal error: StockID must be a valid GUID")
        End If

        pDR = GetStockInfo(pStockID)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Stock was not found or stock has not cache info")
        End If

        pOut = New JsonDictionary
        pOut("StockNumber") = pDR("StockNumber")
        pOut("StockDescription") = pDR("StockDescription")
        pOut("QtyOnHand") = pDR("QtyOnHand")
        pOut("LastApprovedCount") = pDR("LastApprovedCount")
        pOut("MustCountReason") = pDR("MustCountReason")
        pOut("RankNum") = pDR("RankNum")
        pOut("RankLetter") = pDR("RankLetter")
        pOut("QtyCommitted") = pDR("QtyCommitted")
        pOut("SafetyStocks") = pDR("SafetyStocks")
        pOut("SafetyStockDuration") = WebRequest.Instance.Business.SystemDefaults.SafetyStockDuration ' HTML: Order Qty

        pOut("ForecastHorizon") = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("Forecast horizon")
        pOut("SafetyStockDuration") = WebRequest.Instance.Business.SystemDefaults.SafetyStockDuration

        pOut("PoProcessingLeadTime") = WebRequest.Instance.Business.SystemDefaults.PoProcessingLeadTime
        pDtSuppliers = WebRequest.Instance.Business.LibraryStock.BestSupplierPrices(ReadyString(pStockID), True)
        If pDtSuppliers.Rows.Count > 0 Then
            pOut("VendorLeadTime") = Format(pDtSuppliers.Rows(0)("LeadTime"), cGlobal.FormatInteger)
            pDR = WebRequest.Instance.Business.LibraryBranches.CurrentBranch()
            If pDR Is Nothing Then
                Return JsonDictionary.Serialize("Error", "Please assign the current branch")
            End If
            If pDR("CustomerID") Is DBNull.Value Then
                Return JsonDictionary.Serialize("Error", "Please assign a customer account to the current branch")
            End If
            pCustomerID = CStr2(pDR("CustomerID"))
            If CStr2(WebRequest.Instance.Business.LibraryCustomers.GetDeliveryAddress(pCustomerID)).Length = 0 Then
                Return JsonDictionary.Serialize("Error", "Please assign a delivery address to the current branch's customer account")
            End If
            pOut.Add("Shippers", WebRequest.Instance.Business.LibraryShippers.GetCustomerSupplierRoutes(pCustomerID, CStr2(pDtSuppliers.Rows(0)("SupplierID"))))
            pDrShipper = WebRequest.Instance.Business.LibraryShippers.GetDefaultShipper
            If pDrShipper IsNot Nothing Then
                With pOut.AddDictionary("Shipper")
                    .Add("ID", CStr2(pDrShipper("ID")))
                    .Add("Name", pDrShipper("ShipperName"))
                End With
            End If
        Else
            pOut("VendorLeadTime") = CDec2(WebRequest.Instance.Business.SystemDefaults.VendorLeadTime)
        End If

        pOut.Add("ShippingLeadTime", CDec2(WebRequest.Instance.Business.SystemDefaults.ShippingLeadTime))
        pOut.Add("Suppliers", pDtSuppliers)
        pDsDrpTable = GetDrpTable(pStockID, CDec2(pOut("PoProcessingLeadTime")) + CDec2(pOut("VendorLeadTime")) + pOut("ShippingLeadTime"))
        pOut("WeekNumber") = WeekNumber(pDsDrpTable.Tables("Dates").Rows(0)("NowDateFrom"))
        With pOut.AddDictionary("Tables")
            .Add("Data", pDsDrpTable.Tables("Data"))
            .Add("Dates", pDsDrpTable.Tables("Dates"))
        End With

        Return pOut.Serialize

    End Function

    Private Function GetStockInfo(vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DrpCacheStockRank.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & _
            ",DrpCacheStockRank.QtyOnHand" &
            ",DrpCacheStockRank.LastApprovedCount" &
            ",DrpCacheStockRank.MustCountReason" &
            ",DrpCacheStockRank.RankNum" &
            ",DrpCacheStockRank.RankLetter" &
            ",DrpCacheStockRank.QtyCommitted" &
            ",DrpCacheDrpStocks.SafetyStocks" &
            " FROM DrpCacheStockRank" &
            " LEFT JOIN DrpCacheDrpStocks ON DrpCacheDrpStocks.StockID = DrpCacheStockRank.StockID" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockRank.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" &
            " WHERE DrpCacheStockRank.StockID = " & ReadyString(vID)
        pDT = Common.WebRequest.ProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Private Function GetDrpTable(vStockID As String, vLeadTime As Integer) As DataSet

        Dim pDS As DataSet
        Dim pDtData As DataTable
        Dim pDtDates As DataTable
        Dim pDrDate As DataRow
        Dim pDtSettings As DataTable
        Dim pDrSettings As DataRow

        Dim pSql As String
     
        '// dates
        pSql = "SELECT * FROM DrpCacheDrpSettings"
        pDtSettings = Common.WebRequest.ProData.Execute(pSql)
        If pDtSettings.Rows.Count = 0 Then
            Throw New Exception("No cached DRP data")
        End If
        pDrSettings = pDtSettings.Rows(0)
        pDtDates = New DataTable
        pDtDates.TableName = "Dates"
        With pDtDates.Columns
            .Add("NowDateFrom", GetType(Date))
            .Add("NowDateTo", GetType(Date))
            .Add("NowDateRange")
        End With
        For p As Integer = 1 To pDrSettings("ForecastHorizon")
            pDrDate = pDtDates.NewRow
            pDrDate("NowDateFrom") = pDrSettings("Week" & Format(p, "00") & "Start")
            pDrDate("NowDateTo") = pDrSettings("Week" & Format(p, "00") & "End")
            pDrDate("NowDateRange") = DisplayableDateRange(pDrDate("NowDateFrom"), pDrDate("NowDateTo"))
            pDtDates.Rows.Add(pDrDate)
        Next p

        '// data
        pSql = "SELECT * FROM DrpCacheDrpTable" &
            " WHERE StockID = " & ReadyString(vStockID) &
            " ORDER BY Ordinal"
        pDtData = Common.WebRequest.ProData.Execute(pSql)
        pDtData.TableName = "Data"

        pDS = New DataSet
        pDS.Tables.Add(pDtDates)
        pDS.Tables.Add(pDtData)

        Return pDS

    End Function
#End Region
End Class

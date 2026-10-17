Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization

Public Class SalesMenuController
    Inherits System.Web.Mvc.Controller

    Shared ReadOnly mDucToMobizInvoiceLock As New Object

    Function Index() As ActionResult

        Dim pStr As String

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

        Return View()

    End Function

    Function BsIndex() As ActionResult
        'bootstrap theme
        Dim pStr As String

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

        Return View()

    End Function

    Function WebPOS() As ActionResult
        'bootstrap theme
        Dim pStr As String

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

        Return View()

    End Function

    Function WebPOSNew() As ActionResult
        'bootstrap theme
        'Dim pStr As String

        'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
        '    If WebRequest.Instance.HandleAccess = False Then Exit Function
        '    If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        'End If
        WebPosFlush.StartAsync()

        Return View()

    End Function

    Function SalesRepresentatives() As ActionResult

        Return View()

    End Function

    Function OrderTaker() As ActionResult

        Return View()

    End Function
    Function CustomerDetails() As ActionResult

        Return View()

    End Function
    Function BsCustomerDetails() As ActionResult
        'bootstrap theme
        Return View()

    End Function
    Function TableView() As ActionResult

        Return View()

    End Function
    Function Header() As ActionResult

        Return View()

    End Function
    Function BsHeader() As ActionResult
        'bootstrap theme
        Return View()

    End Function
    Function POSViewAll() As ActionResult

        Return View()

    End Function
    Function BsPOSViewAll() As ActionResult
        'bootstrap theme
        Return View()

    End Function
    Function Cart() As ActionResult

        Return View()

    End Function
    Function BsCart() As ActionResult

        Return View()

    End Function
    Function HotButtonMode() As ActionResult

        Return View()

    End Function
    Function BsHotButtonMode() As ActionResult
        'bootstrap theme
        Return View()

    End Function

    Function FMCG() As ActionResult
        Return View()
    End Function

    Function FMCGHeader() As ActionResult
        Return View()
    End Function

    Function FMCGPopup() As ActionResult
        Return View()
    End Function

    Function FMCGPayment() As ActionResult
        Return View()
    End Function

    Function FMCGCart() As ActionResult
        Return View()
    End Function

    Function FMCGBasic() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

        Return View()
    End Function

    Function RentableAssetView() As ActionResult
        Return View()
    End Function


    ' commented for reference
    'Private Function VerifySettings() As String

    '    Dim pStr As String

    '    If IsGuid(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Customer ID")) = False Then
    '        pStr &= "POS Customer ID"
    '    End If
    '    If IsGuid(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Delivery Address ID")) = False Then
    '        If CStr2(pStr).Length > 0 Then pStr &= ", "
    '        pStr &= "POS Delivery Address ID"
    '    End If
    '    If IsGuid(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Billing Address ID")) = False Then
    '        If CStr2(pStr).Length > 0 Then pStr &= ", "
    '        pStr &= "POS Billing Address ID"
    '    End If
    '    If IsGuid(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Invoice Default ID")) = False Then
    '        If CStr2(pStr).Length > 0 Then pStr &= ", "
    '        pStr &= "POS Invoice Default ID"
    '    End If
    '    If IsGuid(WebRequest.Instance.Business.SystemDefaults.PosSubscriberCustomerID) = False Then
    '        If CStr2(pStr).Length > 0 Then pStr &= ", "
    '        pStr &= "POS Subscriber Customer ID"
    '    End If

    '    If CStr2(pStr).Length > 0 Then
    '        Return "Please set the following settings first: " & pStr
    '    End If

    'End Function

    <HttpPost()>
    Public Function RecentlyBought() As String

        Dim pCustomerID As String
        Dim pDateRangeType As String
        Dim pDateRange As DateRange
        Dim pDT As DataTable
        Dim pDsCart As DataSet

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid customer ID")
        End If

        pDateRangeType = Common.WebRequest.Data("DateRangeType")
        Select Case pDateRangeType
            Case "Within last week"
                Client.GetWithinLastWeek(pDateRange)

            Case "Within the last 2 weeks"
                Client.GetWithinLast2Weeks(pDateRange)

            Case "Within the last 4 weeks"
                Client.GetWithinLast4Weeks(pDateRange)

            Case "Within the last 3 months"
                Client.GetWithinLast3Months(pDateRange)

            Case "Within the last 6 months"
                Client.GetWithinLast6Months(pDateRange)

            Case "Within last year"
                Client.GetWithinLastYear(pDateRange)

            Case "Week to date"
                Client.GetWtdDates(pDateRange, Nothing)

            Case "Month to date"
                Client.GetMtdDates(pDateRange, Nothing)

            Case "Quarter to date"
                Client.GetQtdDates(pDateRange, Nothing)

            Case "Year to date"
                Client.GetYtdDates(pDateRange, Nothing)

            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid DateRangeType: " & pDateRangeType)

        End Select

        pDT = WebRequest.Instance.Business.LibraryCustomers.RecentlyBought(pCustomerID, pDateRange.DateFrom)
        pDT.Columns.Add("InCart", GetType(Integer))
        If pDT.Rows.Count > 0 Then
            'pDsCart = WebRequest.Instance.Business.LibraryCustomers.GetCart(pCustomerID)
            'If pDsCart IsNot Nothing Then
            '    pDsCart.Tables("Stocks").Columns.Add("SubQty", GetType(Integer), "Quantity * ActualQuantity")
            '    For Each pDR As DataRow In pDT.Rows
            '        pDR("InCart") = CDec2(pDsCart.Tables("Stocks").Compute("SUM(SubQty)", "StockID = " & ReadyString(pDR("ID"))))
            '    Next pDR
            'End If
        End If

        Return JsonDictionary.Serialize("data", pDT)

    End Function

    ' commented as useless 6/3/2014 4:48 PM
    '<HttpPost()>
    'Public Function Cart() As String

    '    Dim pDtUnits As DataTable
    '    Dim pDtStocks As DataTable ' unique
    '    Dim pCustomerID As String
    '    Dim pOrderID As String
    '    Dim pOut As JsonDictionary
    '    Dim pDsCart As DataSet
    '    Dim pMessage As String

    '    If WebSession.Instance.UserAccess Is Nothing _
    '        OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

    '        Return JsonDictionary.Serialize("Error", "Access denied")
    '    End If

    '    pCustomerID = Common.WebRequest.Data("CustomerID")
    '    If IsGuid(pCustomerID) Then
    '        pCustomerID = ReadyGuid(pCustomerID)
    '        pOrderID = Common.WebRequest.Data("OrderID")
    '        If IsGuid(pOrderID) Then
    '            pOrderID = ReadyGuid(pOrderID)
    '            If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID, pCustomerID, pMessage) = False Then
    '                If CStr2(pMessage).Length > 0 Then
    '                    Return JsonDictionary.Serialize("Error", pMessage)
    '                Else
    '                    Return JsonDictionary.Serialize("Error", "Invalid Cart")
    '                End If
    '            End If
    '        Else
    '            pOrderID = Nothing
    '        End If

    '        pDsCart = WebRequest.Instance.Business.LibraryCustomers.GetCart(pCustomerID, pOrderID)
    '        If pDsCart IsNot Nothing Then
    '            WebRequest.Instance.Business.TransSalesOrder.LockSalesOrder(pOrderID)
    '            pOut = New JsonDictionary
    '            pOut.Add("Master", pDsCart.Tables("Master").Rows(0))
    '            pDtUnits = pDsCart.Tables("Stocks")
    '            With pDtUnits.Columns
    '                .Remove("OutputTax")
    '                .Remove("UnitPriceWOTax")
    '                .Remove("Cost")
    '                .Remove("UnitGP")
    '                .Remove("UnitGPM")
    '                .Remove("LineGP")
    '                .Remove("LineGPM")
    '                .Remove("LineCost")
    '                .Remove("CurrentCost")
    '            End With
    '            pDtUnits = New DataView(pDtUnits, "Quantity > 0", "LineNumber DESC", DataViewRowState.CurrentRows).ToTable
    '            pDtStocks = pDtUnits.DefaultView.ToTable(True, {"StockID", "StockNumber", "StockDescription"})
    '            With pDtUnits.Columns
    '                .Remove("StockNumber")
    '                .Remove("StockDescription")
    '            End With
    '            pOut.Add("Stocks", pDtStocks)
    '            pOut.Add("Units", pDtUnits)
    '            Return pOut.Serialize
    '        Else
    '            Return JsonDictionary.Serialize("text", "No item")
    '        End If
    '    Else
    '        Return JsonDictionary.Serialize("error", "Invalid CustomerID")
    '    End If

    'End Function

    <HttpPost()>
    Public Function DeleteStock() As String

        Dim pDT As DataTable
        Dim pOrderID As String
        Dim pStockID As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEdit OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) = False Then
            Return JsonDictionary.Serialize("Error", "OrderID must be a valid GUID")
        End If
        pOrderID = ReadyGuid(pOrderID)

        pStockID = Common.WebRequest.Data("StockID")
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "StockID must be a valid GUID")
        End If
        pStockID = ReadyGuid(pStockID)

        If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID) Then
            pDT = WebRequest.Instance.Business.TransSalesOrder.GetCartStock(pOrderID, pStockID)
            If pDT.Rows.Count > 0 Then
                WebRequest.Instance.Business.TransSalesOrder.DeleteOrderStock(pOrderID, pStockID)
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", "That item was already removed from your cart")
            End If
        Else
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If

    End Function

    <HttpPost()>
    Public Function GetCustomer() As String

        Dim pDR As DataRow
        Dim pCustomerOrganizationID As String
        Dim pOut As New JsonDictionary
        Dim pFowstID As String = CStr2(WebRequest.Instance.Data("FowstID"))

        ' ''If WebSession.Instance.UserAccess Is Nothing _
        ' ''    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEdit _
        ' ''            OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable _
        ' ''            OrElse WebSession.Instance.UserAccess.PointOfSales_PointOfSales.IsEnable _
        ' ''    ) = False Then

        ' ''    Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pCustomerOrganizationID = Common.WebRequest.Data("CustomerOrganizationID")
        If CStr2(pCustomerOrganizationID).Length = 0 Then
            pCustomerOrganizationID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
        End If
        If IsGuid(pCustomerOrganizationID) Then
            pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(pCustomerOrganizationID)
            If pDR IsNot Nothing Then
                pOut("ID") = pCustomerOrganizationID
                pOut("CompanyName") = pDR("CompanyName")

                Dim pRevenueInfo As DataRow = Common.WebRequest.ProData.ExecuteRow("SELECT LibraryOrganizationCustomer.*," & vbCrLf &
                                            "ISNULL(peopleSalesRep.Surname + ', ', '') + ISNULL(peopleSalesRep.FirstName + ' ', '') + ISNULL(peopleSalesRep.MiddleName + ' ', '') as SalesRepName," & vbCrLf &
                                            "LibraryPaymentTerms.Days as CreditTerms" & vbCrLf &
                                            "FROM LibraryOrganizationCustomer" & vbCrLf &
                                            "LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
                                            "LEFT JOIN MobizLibraryPeople peopleSalesRep ON peopleSalesRep.ID = LibraryOrganizationCustomer.SalesRepID" & vbCrLf &
                                            "WHERE LibraryOrganizationCustomer.OrganizationID = " & ReadyString(pCustomerOrganizationID))

                pOut("SalesRepName") = pRevenueInfo("SalesRepName")
                pOut("CreditTerms") = pRevenueInfo("CreditTerms")

                If CBool2(Common.WebRequest.Data("GetOrAddDefaultAddresses")) Then
                    Dim pDrAddress = WebRequest.Instance.Business.LibraryOrganizationFOWST.GetOrAddFowstByName(pCustomerOrganizationID, "Store Pickup")
                    pOut("DefaultDeliveryAddressID") = CStr2(pDrAddress("ID"))
                    pOut("DefaultDeliveryAddressName") = pDrAddress("Name")

                    pDrAddress = WebRequest.Instance.Business.LibraryOrganizationFOWST.GetOrAddFowstByName(pCustomerOrganizationID, "Store Payment")
                    pOut("DefaultBillingAddressID") = CStr2(pDrAddress("ID"))
                    pOut("DefaultBillingAddressName") = pDrAddress("Name")
                End If
            End If
        End If

        If CBool2(Common.WebRequest.Data("IncludeZRead")) Then
            Dim pZReadDate = Date.Now.Date
            Dim pUuid = Common.WebRequest.Data("UUID")
            Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.ByUuid(pUuid)
            If pDrDevice IsNot Nothing Then
                Dim pEnforceZRead = WebRequest.Instance.Business.SystemDefaults.EnforceZRead
                If pEnforceZRead Then
                    pOut.Add("EnforceZRead", 1)
                End If
                Dim pDrDeviceFowst = WebRequest.Instance.Business.LibraryDeviceFowst.ByDeviceFowst(CStr2(pDrDevice("ID")), pFowstID)
                If pDrDeviceFowst IsNot Nothing Then
                    pOut.Add("POSRegistrationNo", CStr2(pDrDeviceFowst("POSRegistrationNo")))
                    If pEnforceZRead Then
                        If pDrDeviceFowst("ZReadDate") Is DBNull.Value Then
                            Dim pIU = New InsertUpdate("LibraryDeviceFowst")
                            pIU("ZReadDate") = ProSqlite.ReadyDate(Now.Date)
                            pIU("LastUpdated") = "GETDATE()"
                            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDrDeviceFowst("ID")))
                            WebRequest.Instance.ProData.ExecuteSQL(pSql)
                        Else
                            pZReadDate = pDrDeviceFowst("ZReadDate")
                            If pZReadDate < Date.Now.Date Then
                                pOut.Add("ZReadForwardable", 1)
                            End If
                        End If
                    End If ' END If pEnforceZRead Then
                End If

                pOut.Add("ZReadDate", Format(pZReadDate, cGlobal.FormatDateTimeSql))
                pOut.Add("ServerTime", Format(Now, cGlobal.FormatDateTimeSql))
            End If
        End If

        Try
            If CBool2(Common.WebRequest.Data("CreateCacheDatabaseIfNeeded")) Then
                Using pProData = ProData.CreateFromConnectionInfo(
                    vServer:=Common.ConnectionInfo.Server,
                    vDatabase:="",
                    vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
                    vUsername:=Common.ConnectionInfo.Username,
                    vPassword:=Common.ConnectionInfo.Password)

                    WebPosDbDownloaderController.CreateCacheDatabaseIfNeeded(pProData, WebRequest.Instance.Database)
                End Using
            End If
        Catch ex As Exception
            If ex.Message.Contains("failed with the operating system error 3(The system cannot find the path specified.)") = False Then
                Throw
            End If
        End Try

        If CBool2(Common.WebRequest.Data("GetRemoteServer")) Then
            Dim pRemoteServer = WebPosDbDownloaderController.GetRemoteServer()
            pOut.Add("RemoteServer", pRemoteServer)

            If String.IsNullOrWhiteSpace(pFowstID) = False Then
                Dim pDrFowstGroupPrice = WebRequest.Instance.Business.LibraryOrganizationFOWST.FowstGroupPrice(pFowstID)
                If pDrFowstGroupPrice IsNot Nothing Then
                    pOut.Add("FowstGroupPriceName", CStr2(pDrFowstGroupPrice("PriceLevel")))
                End If
            End If
        End If

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function IncrementZReadDate() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut As New JsonDictionary
        Dim pUuid = Common.WebRequest.Data("UUID")
        Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.ByUuid(pUuid)
        If pDrDevice IsNot Nothing Then
            Dim pFowstID = Common.WebRequest.Data("FowstID")
            If WebRequest.Instance.Business.SystemDefaults.EnforceZRead Then
                Dim pDrDeviceFowst = WebRequest.Instance.Business.LibraryDeviceFowst.ByDeviceFowst(CStr2(pDrDevice("ID")), pFowstID)
                If pDrDeviceFowst IsNot Nothing Then
                    If pDrDeviceFowst("ZReadDate") IsNot DBNull.Value Then
                        If pDrDeviceFowst("ZReadDate") > Now.Date Then
                            Return JsonDictionary.Serialize("Error", "The Z-Read date is already set to the current date")
                        End If
                        Dim pIU = New InsertUpdate("LibraryDeviceFowst")
                        Dim pZReadDate = CDate(pDrDeviceFowst("ZReadDate")).AddDays(1)

                        pIU("ZReadDate") = ProSqlite.ReadyDate(pZReadDate)
                        pIU("LastUpdated") = "GETDATE()"
                        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDrDeviceFowst("ID")))
                        WebRequest.Instance.ProData.ExecuteSQL(pSql)

                        pOut.Add("Result", "ok")
                        pOut.Add("ZReadDate", Format(pZReadDate, cGlobal.FormatDateTimeSql))
                        pOut.Add("ServerTime", Format(Now, cGlobal.FormatDateTimeSql))
                        If pZReadDate < Date.Now.Date Then
                            pOut.Add("ZReadForwardable", 1)
                        End If
                    End If
                Else
                    JsonDictionary.Serialize("Error", "No matching record in LibraryDeviceFowst. vb3ZR2P5ABHN!")
                End If
            Else
                JsonDictionary.Serialize("Error", "Z-Read Date is not enforced. vbG3DB2CTPJH!")
            End If
        Else
            JsonDictionary.Serialize("Error", "Device with UUID, " & pUuid & ", does not exist. vb8E4MWWV8TX!")
        End If

        Return pOut.Serialize()

    End Function

    <HttpPost()>
    Public Function SetDefaultCustomer() As String

        Dim pCustomerOrganizationID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pCustomerOrganizationID = Common.WebRequest.Data("CustomerOrganizationID")
        If IsGuid(pCustomerOrganizationID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid CustomerOrganizationID")
        End If
        If WebRequest.Instance.Business.LibraryOrganization.IsValidCustomer(pCustomerOrganizationID) = False Then
            Return JsonDictionary.Serialize("Error", "That customer does not exist in your organization")
        End If
        WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID = pCustomerOrganizationID

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function PosCart() As String

        Dim pDtUnits As DataTable
        Dim pDtStocks As DataTable ' unique
        Dim pOrderID As String
        Dim pOut As JsonDictionary
        Dim pDsCart As DataSet
        Dim pFowstID As String
        Dim pForPickingList As Boolean
        Dim pSectionID As String
        Dim pDrInvoice As DataRow
        Dim pError As String
        Dim pDrSalesOrderLegacy As DataRow ' DUC wholesale 
        Dim pIncludeProductSuppliers = CBool2(Common.WebRequest.Data("IncludeSuppliers"))
        Dim pTransType = CStr2(Common.WebRequest.Data("TransType"))

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        If Not CBool2(HttpUtility.ParseQueryString(Request.UrlReferrer.Query())("IsSoaLink")) Then
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        Else
            WebSession.Instance.PeopleUserID = Guid.Empty.ToString()
        End If
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        WebRequest.Instance.Business.TransInvoice.CurrentOrgID = WebRequest.Instance.OrgID

        'TODO
        'WebSession.Instance.PosMode = Common.WebRequest.Data("PosMode")
        Dim posMode As String = Common.WebRequest.Data("PosMode")
        pForPickingList = CBool2(Common.WebRequest.Data("ForPickingList"))
        If pForPickingList Then
            pSectionID = Common.WebRequest.Data("SectionID")
            If IsGuid(pSectionID) = False Then
                Return JsonDictionary.Serialize("Error", "SectionID must be a valid GUID if ForPickingList is set")
            End If
            pSectionID = ReadyGuid(pSectionID)
        End If

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) Then
            pOrderID = ReadyGuid(pOrderID)
            If CBool2(Common.WebRequest.Data("DontCheckCartValidity")) = False Then
                If WebRequest.Instance.Business.TransInvoice.IsValidCart(pOrderID) = False Then
                    Return JsonDictionary.Serialize("text", "No item")
                End If
            End If
        Else
            If CDec2(Common.WebRequest.Data("DontFetchIfBlank")) > 0 Then
                Return JsonDictionary.Serialize("text", "No item")
            End If
            pOrderID = Nothing
        End If
        WebRequest.Instance.Business.TransInvoice.UpdateAccountsPayableAmount(pOrderID, WebRequest.Instance.OrgID, WebRequest.Instance.Data("AccountsPayableFollowCost"))
        Dim pIncludeZeroQty As Boolean = CBool2(Common.WebRequest.Data("IncludeZeroQty"))
        WebRequest.Instance.Business.TransInvoice.FixAccountsPayableBillingAndDeliveryAddress(pOrderID)
        pDsCart = WebRequest.Instance.Business.TransInvoice.GetCart(pOrderID, WebSession.Instance.PeopleUserID, pFowstID, pForPickingList, pSectionID, pIncludeZeroQty, pIncludeProductSuppliers, CBool2(WebRequest.Instance.Data("IncludeChildOrderItems")), pTransType = "PurchaseOrder")
        If pDsCart IsNot Nothing Then
            pOut = New JsonDictionary
            pDrInvoice = pDsCart.Tables("Invoices").Rows(0)
            If pDrInvoice IsNot Nothing Then
                If ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid(modClient.DucOrgID) Then
                    pDsCart.Tables("Invoices").Columns.Add("DucSoNum")
                    If pDrInvoice("SalesOrderID") Is DBNull.Value Then
                        pDrInvoice("DucSoNum") = "None"
                    Else ' ELSE If pDrInvoice("SalesOrderID") Is DBNull.Value Then
                        ' todo: modify TransInvoice.GetCart to include data from legacy sales order
                        If WebRequest.Instance.Business.Utilities.HasWholesaleDatabase Then
                            pDrSalesOrderLegacy = WebRequest.Instance.Business.TransSalesOrderLegacy.ByID(CStr2(pDrInvoice("SalesOrderID")), "wholesale")
                            If pDrSalesOrderLegacy Is Nothing Then
                                pDrInvoice("DucSoNum") = ReadyGuid(pDrInvoice("SalesOrderID")) & " (no entry in Wholesale database)"
                            Else
                                pDrInvoice("DucSoNum") = pDrSalesOrderLegacy("OrderNo")
                            End If
                        Else ' ELSE If WebRequest.Instance.Business.Utilities.HasWholesaleDatabase Then
                            pDrInvoice("DucSoNum") = ReadyGuid(pDrInvoice("SalesOrderID")) & " (no Wholesale database)"
                        End If ' END If WebRequest.Instance.Business.Utilities.HasWholesaleDatabase Then
                    End If ' END If pDrInvoice("SalesOrderID") Is DBNull.Value Then
                End If ' END If ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid(modClient.DucOrgID) Then
            End If ' END If pDrInvoice IsNot Nothing Then
            pOut.Add("Master", pDrInvoice)
            pDtUnits = pDsCart.Tables("Stocks")
            If pDsCart.Tables.Contains("Suppliers") Then
                pOut.Add("ProductSuppliers", pDsCart.Tables("Suppliers"))
            End If
            If pDtUnits.Rows.Count = 0 Then
                If CDec2(Common.WebRequest.Data("DontFetchIfBlank")) > 0 Then
                    Return JsonDictionary.Serialize("text", "No item")
                End If
            End If
            pDtUnits = New DataView(pDtUnits, "", "DateCreated, LineNumber", DataViewRowState.CurrentRows).ToTable

            'additional by janine sept 5 for sales order taker
            If pIncludeZeroQty = False Then
                If (posMode = "so") _
                    OrElse CStr2(pDrInvoice("TransType")) = "SalesOrder" Then

                    pDtUnits = New DataView(pDtUnits, "OrderedQuantity > 0", "", DataViewRowState.CurrentRows).ToTable
                Else
                    pDtUnits = New DataView(pDtUnits, "OrderedQuantity > 0 OR Quantity > 0", "", DataViewRowState.CurrentRows).ToTable
                End If
            End If

            pDtStocks = pDtUnits.DefaultView.ToTable(True, {"ProductID", "ProductNo", "ProductName", "VendorID", "Image", "Brand", "ProductNameOnly", "ThirdPartyProductNumber"})
            With pDtUnits.Columns
                .Remove("ProductNo")
                .Remove("ProductName")
            End With
            pOut.Add("Stocks", pDtStocks)
            pDtUnits = New DataView(pDtUnits, "", "DateCreated, LineNumber", DataViewRowState.CurrentRows).ToTable
            pOut.Add("Units", pDtUnits)

            If CBool2(Common.WebRequest.Data("ShowBackOrderItems")) Then
                Dim backorderRowID As String = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM TransInvoice WHERE BackorderSourceInvoiceID = " & ReadyString(pOrderID)))

                If backorderRowID <> "" Then
                    pDsCart = WebRequest.Instance.Business.TransInvoice.GetCart(backorderRowID, WebSession.Instance.PeopleUserID, pFowstID, pForPickingList, pSectionID, pIncludeZeroQty, pIncludeProductSuppliers, CBool2(WebRequest.Instance.Data("IncludeChildOrderItems")), pTransType = "PurchaseOrder")

                    pDtUnits = pDsCart.Tables("Stocks")
                    If pDsCart.Tables.Contains("Suppliers") Then
                        pOut.Add("ProductSuppliers", pDsCart.Tables("Suppliers"))
                    End If
                    If pDtUnits.Rows.Count = 0 Then
                        If CDec2(Common.WebRequest.Data("DontFetchIfBlank")) > 0 Then
                            Return JsonDictionary.Serialize("text", "No item")
                        End If
                    End If
                    pDtUnits = New DataView(pDtUnits, "", "LineNumber", DataViewRowState.CurrentRows).ToTable

                    'additional by janine sept 5 for sales order taker
                    If pIncludeZeroQty = False Then
                        If (posMode = "so") _
                            OrElse CStr2(pDrInvoice("TransType")) = "SalesOrder" Then

                            pDtUnits = New DataView(pDtUnits, "OrderedQuantity > 0", "", DataViewRowState.CurrentRows).ToTable
                        Else
                            pDtUnits = New DataView(pDtUnits, "OrderedQuantity > 0 OR Quantity > 0", "", DataViewRowState.CurrentRows).ToTable
                        End If
                    End If

                    With pDtUnits.Columns
                        .Remove("ProductName")
                    End With
                    pDtUnits = New DataView(pDtUnits, "", "LineNumber", DataViewRowState.CurrentRows).ToTable
                    pOut.Add("BackOrder", pDtUnits)
                End If
            End If

            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("text", "No item")
        End If

    End Function

    <HttpPost()>
    Public Function DeleteStockPos() As String

        Dim pDT As DataTable
        Dim pOrderID As String
        Dim pProductID As String
        Dim pItemID = Common.WebRequest.Data("ItemID")

        Dim pUnitMeasure As String 'additional by janine sept 9 2014
        Dim pCreator As TransInvoice.Creator

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) = False Then
            Return JsonDictionary.Serialize("Error", "OrderID must be a valid GUID")
        End If
        pOrderID = ReadyGuid(pOrderID)

        pProductID = Common.WebRequest.Data("ProductID")
        If IsGuid(pProductID) = False Then
            Return JsonDictionary.Serialize("Error", "ProductID must be a valid GUID")
        End If
        pProductID = ReadyGuid(pProductID)

        pUnitMeasure = Common.WebRequest.Data("UnitMeasure") 'additional by janine sept 9 2014
        If String.IsNullOrWhiteSpace(pItemID) = False Then
            If IsGuid(pItemID) = False Then
                Return JsonDictionary.Serialize("Error", "Item must be a valid GUID or blank. ref: vbTB4BRW2JU7!")
            End If
        End If

        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pOrderID) Then
            pDT = WebRequest.Instance.Business.TransInvoice.GetCartStock(pOrderID, pProductID, Common.WebRequest.Data("PosMode"))
            If pDT.Rows.Count > 0 Then
                Common.WebRequest.ProData.BeginTransaction()

                If CBool2(Common.WebRequest.Data("AddToItemRequisition")) Then
                    Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pOrderID)
                    For Each pDR As DataRow In pDT.Rows
                        If CStr2(pDR("UnitMeasure")).ToLower() <> CStr2(pUnitMeasure).ToLower() Then
                            Continue For
                        End If
                        SaveToItemRequisition(vProductID:=pProductID, _
                            vQty:=CDec2(pDR("Quantity")), _
                            vUnitQty:=CDec2(pDR("ActualQuantity")), _
                            vUnitMeasure:=pUnitMeasure, _
                            vUserID:=WebSession.Instance.PeopleUserID, _
                            vRemarks:="Returned from PO. Vendor cannot Serve.", _
                            vFowstID:=CStr2(pDrInvoice("FowstID"))
                        )
                    Next
                End If

                WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pOrderID, pProductID, pUnitMeasure, pItemID, vOrgID:=WebRequest.Instance.OrgID)

                pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator()
                pCreator.ID = pOrderID
                pCreator.UserID = WebSession.Instance.PeopleUserID
                pCreator.UpdateTotal()
                WebRequest.Instance.Business.TransInvoice.UpdateProfitabilityTransactionColumns(pOrderID)
                WebRequest.Instance.Business.TransInvoice.UpdateLastUpdated(pOrderID, WebSession.Instance.PeopleUserID)

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", "That item was already removed from your cart")
            End If
        Else
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If

    End Function

    Private Sub SaveToItemRequisition(vProductID As String, _
        vQty As Decimal, _
        vUnitQty As Decimal,
        vUnitMeasure As String, _
        vUserID As String,
        vRemarks As String,
        vFowstID As String)

        Dim pIU As New InsertUpdate("TransItemRequisitions")
        pIU("ID") = "NEWID()"
        pIU("ProductID") = ReadyString(vProductID)
        pIU("RequestedByID") = ReadyString(vUserID)
        pIU("RequestedQty") = CDec2(vQty)
        pIU("RequestedQtyTotal") = CDec2(vQty)
        pIU("UnitMeasure") = ReadyString(vUnitMeasure)
        pIU("UnitQty") = CDec2(vUnitQty)
        pIU("UserID") = ReadyString(vUserID)
        pIU("Remarks") = ReadyString(vRemarks)
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("DateRequested") = "GETDATE()"
        Dim pSql = pIU.GetInsert
        WebRequest.Instance.ProData.ExecuteSQL(pSql)

    End Sub

    '<HttpPost()>
    'Public Function SetPosCustomerAddressID() As String

    '    Dim pCustomerAddressID As String
    '    Dim pDR As DataRow

    '    pCustomerAddressID = Common.WebRequest.Data("CustomerAddressID")
    '    If IsGuid(pCustomerAddressID) = False Then
    '        Return JsonDictionary.Serialize("Error", "CustomerAddressID must be a valid GUID")
    '    End If
    '    pCustomerAddressID = ReadyGuid(pCustomerAddressID)

    '    pDR = WebRequest.Instance.Business.LibraryCustomers.GetByCustomerAddressID(pCustomerAddressID)
    '    If pDR Is Nothing Then
    '        Return JsonDictionary.Serialize("Error", "Customer Address does not exist")
    '    End If
    '    If ReadyGuid(pDR("CustomerID")) <> ReadyGuid(WebSession.Instance.SubscriberOrganizationID) Then
    '        Return JsonDictionary.Serialize("Error", "Your chosen address' customer ID does not match the SubscriberOrganizationID. Please login again")
    '    End If

    '    WebSession.Instance.SubscriberOrganizationBranchID = pCustomerAddressID
    '    WebSession.Instance.SubscriberOrganizationBranchName = pDR("Name") & ""

    '    Return JsonDictionary.Serialize("Result", "ok")

    'End Function

    <HttpPost()>
    Public Function SaveRemarks() As String

        Dim pOrderID As String
        Dim pRemarks As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Order ID")
        End If
        pOrderID = ReadyGuid(pOrderID)
        pRemarks = Common.WebRequest.Data("Remarks") & ""
        If pRemarks.Length > 999 Then
            Return JsonDictionary.Serialize("Error", "Remarks is too long")
        End If

        If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID) = False Then
            Return JsonDictionary.Serialize("Error", "Cart is not valid")
        End If

        If WebRequest.Instance.Business.TransSalesOrder.SaveRemarks(pOrderID, pRemarks) Then
            Return JsonDictionary.Serialize("Remarks", "ok")
        Else
            Return JsonDictionary.Serialize("Error", "No record was updated")
        End If

    End Function

    <HttpPost()>
    Public Function SaveQty() As String

        Dim pInvoiceItemID As String = Common.WebRequest.Data("InvoiceItemID")
        Dim pQtyMode As String = Common.WebRequest.Data("QtyMode")
        Dim pValue As String = Common.WebRequest.Data("Value")
        Dim pError As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        If WebRequest.Instance.Business.TransInvoice.SaveQty(
            vInvoiceItemID:=pInvoiceItemID, vValue:=pValue,
            vUserID:=WebSession.Instance.PeopleUserID,
            vQtyMode:=pQtyMode, oError:=pError) Then

            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Error", pError)
        End If

    End Function

    <HttpPost()>
    Public Function SaveRemarksPos() As String

        Dim pInvoiceID As String
        Dim pRemarks As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
        End If
        pInvoiceID = ReadyGuid(pInvoiceID)
        pRemarks = Common.WebRequest.Data("Remarks") & ""
        If pRemarks.Length > 999 Then
            Return JsonDictionary.Serialize("Error", "Remarks is too long")
        End If

        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Cart is not valid")
        End If

        If WebRequest.Instance.Business.TransInvoice.SaveRemarks(pInvoiceID, pRemarks) Then
            Return JsonDictionary.Serialize("Remarks", "ok")
        Else
            Return JsonDictionary.Serialize("Error", "No record was updated")
        End If

    End Function

    Public Function Hold() As String

        Dim pOrderID As String
        Dim pMessage As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '  OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Order ID")
        End If
        pOrderID = ReadyGuid(pOrderID)
        If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID, , pMessage) Then
            WebRequest.Instance.Business.TransSalesOrder.UnlockSalesOrder(pOrderID)
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Error", pMessage)
        End If

    End Function

    <HttpPost()>
    Public Function SaveRsd() As String

        Dim pDate As String
        Dim pInvoiceID As String
        Dim pRefNum As String
        Dim pReasonForReconstructingData As String
        Dim pError As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If

        pDate = Common.WebRequest.Data("DateTime")
        If IsDate(pDate) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid DateTime")
        End If
        pDate = CDate(pDate)

        pRefNum = Common.WebRequest.Data("RefNum")
        If pRefNum.Trim.Length = 0 Then
            Return JsonDictionary.Serialize("Error", "Invalid RefNum")
        End If

        pReasonForReconstructingData = Common.WebRequest.Data("ReasonForReconstructingData")
        If CStr2(pReasonForReconstructingData).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "Invalid ReasonForReconstructingData")
        End If

        If WebRequest.Instance.Business.TransInvoice.IsValidAdjustedDate(pDate, _
            WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID, pError, pInvoiceID) = False Then

            Return JsonDictionary.Serialize("Error", pError)
        End If

        pIU = New InsertUpdate("TransInvoice")
        'pIU("LastUpdated") = ReadyString(pDate)
        pIU("LastUpdated") = "GETDATE()" ' aug 9, 2013
        pIU("InvoiceDate") = ReadyString(pDate)
        pIU("ReasonForReconstructingData") = ReadyString(pReasonForReconstructingData)
        pIU("RefNum") = ReadyString(pRefNum)
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function SaveSO() As String

        Dim pDate As String
        Dim pInvoiceID As String
        Dim pRefNum As String
        Dim pReasonForReconstructingData As String
        Dim pError As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Result", "Invalid cart")
        End If

        pIU = New InsertUpdate("TransInvoice")
        pIU("SalesOrderNo") = "dbo.GetOrganizationFOWSTSONo(" & ReadyString(WebRequest.Instance.FowstID) & "," & ReadyString(WebRequest.Instance.DeviceID) & ") + 1"
        pIU("SalesOrderDate") = "GETDATE()"
        pIU("SalesOrderUserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("InvoiceDate") = "GETDATE()"
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        '''Update Places POS OR Counter
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("POSLastSONo") = "dbo.GetOrganizationFOWSTSONo(" & ReadyString(WebRequest.Instance.FowstID) & "," & ReadyString(WebRequest.Instance.DeviceID) & ") + 1 "
        pSql = pIU.GetUpdate(" FowstID = " & IIf(WebRequest.Instance.FowstID.Trim().Length = 0, "NULL", ReadyString(WebRequest.Instance.FowstID.Trim())) & " AND DeviceID = " & IIf(WebRequest.Instance.DeviceID.Trim().Length = 0, "NULL", ReadyString(WebRequest.Instance.DeviceID.Trim())))
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "Success")

    End Function

    Public Function CompilePages() As String

        Dim pFolders As List(Of String)
        Dim pFiles As List(Of String)
        Dim pPages As List(Of String)

        '// list your folders here. entire folders (and subfolders) will be copied to output folder
        '   file path format
        pFolders = New List(Of String)
        With pFolders
            .Add("Content")
            .Add("Scripts3")
            .Add("Scripts")
        End With

        '   file path format
        pFiles = New List(Of String)
        With pFiles
            '.Add("Scripts\jQuery.js")
        End With

        '// add your controllers and actions here
        '   url format
        pPages = New List(Of String)
        With pPages
            .Add("SalesMenu")
            .Add("SalesMenu/HotButtons")
            .Add("Crm")
            .Add("CustomerSearch")
            .Add("DeliveryInfo")
            .Add("DeliveryInfo/EmailPriceQuote")
            .Add("device")
            .Add("device/DeviceInfo")
            .Add("device/DeviceInfoSummary")
            .Add("device/DeviceFowst")
            .Add("Login3")
            .Add("MallLessorAyala")
            .Add("Organization")
            .Add("Organization/OrganizationAddress")
            .Add("Organization/OrganizationAffiliation")
            .Add("Organization/OrganizationBrands")
            .Add("Organization/OrganizationContact")
            .Add("Organization/OrganizationCustomer")
            .Add("Organization/OrganizationCustomerFOWST")
            .Add("Organization/OrganizationCustomerFOWSTProductList")
            .Add("Organization/OrganizationCustomerProduct")
            .Add("Organization/OrganizationEmployee")
            .Add("Organization/OrganizationEmployeeAssignWorkSchedule")
            .Add("Organization/OrganizationEmployeeWorkingHours")
            .Add("Organization/OrganizationFinancialReport")
            .Add("Organization/OrganizationFOWSTProduct")
            .Add("Organization/OrganizationFOWSTProductList")
            .Add("Organization/OrganizationInfo")
            .Add("Organization/OrganizationProduct")
            .Add("Organization/OrganizationOwner")
            .Add("Organization/OrganizationRepresentative")
            .Add("Organization/OrganizationSummary")
            .Add("Organization/OrganizationSupplier")
            .Add("OrganizationShort")
            .Add("OrganizationShort/OrganizationAddress")
            .Add("OrganizationShort/OrganizationInfo")
            .Add("OrganizationShort/OrganizationRepresentative")
            .Add("OrganizationShort/OrganizationSummary")
            .Add("People")
            .Add("People/PeopleContactNo")
            .Add("People/PeopleEducation")
            .Add("People/PeopleEmpHistory")
            .Add("People/PeopleInfoAddress")
            .Add("People/PeopleInfoAlias")
            .Add("People/PeopleInfoName")
            .Add("People/PeopleInfoSummary")
            .Add("People/PeopleMenu")
            .Add("People/PeoplePersonalInfo")
            .Add("People/PeoplePrimeIDReference")
            .Add("People/PeopleRelatedPerson")
            .Add("People/PeopleUserAccount")
            .Add("SalesPayments")
            .Add("SalesPayments/DuplicateReceipt")
            .Add("SalesPayments/EmailSalesReceipt")
            .Add("Stock")
            .Add("SalesReturns")
            .Add("SalesReturns/Stock")
            .Add("SalesReturns/Settings")
            .Add("SecurityRole")
            .Add("SecurityRole/SecurityRoleInfo")
            .Add("SecurityRole/SecurityRoleSummary")
            .Add("SecurityRole/SecurityRoleUsers")
            .Add("ReceiptSettings")
            .Add("ReceiptSettings/ReceiptSettingsInfo")
            .Add("ReceiptSettings/ReceiptSettingsMessage")
            .Add("ReceiptSettings/ReceiptSettingsSummary")
            .Add("ReportsPOS/SalesReport")
            .Add("ReportsPOS/SalesAdjustmentReport")
            .Add("ReportsPOS/ZReport")
            .Add("Welcome3")
            .Add("SignIn")
            .Add("DashBoardsAndReports")
            .Add("DashBoardsAndReports/POSTransactionDetail")
            .Add("DashBoardsAndReports/Settings")
            .Add("DashBoardsAndReports/SalesOrder")
            .Add("InventoryLedger")
            .Add("InventoryCount")
            .Add("Receiving")
            .Add("Browser")
            .Add("DeviceMDMConfig")
            .Add("DeviceDashboard")
            .Add("DevicePolicyAppoint")
            .Add("DeviceAppWhiteList")
            .Add("DeviceApproval")
            .Add("AccountManagement")
            .Add("FileNotFound")
            .Add("Signin/DownloadDatabase")
            .Add("AccountRecovery")
            .Add("AccountManagement")
            .Add("SignUp")
            .Add("SignUp/ImportantReminders")
            .Add("SignUp/Success")
            .Add("DiscountSettings")
            .Add("ProductGroup")
            .Add("ProductGroup/AssignFOWST")
            .Add("ProductGroup/MenuTemplate")
            .Add("TableView")
            .Add("POSSettings")
            .Add("SelfTest")
            .Add("SelfTest/Failed")
            .Add("DevicePrinter/PrinterSetup")
            .Add("CheckPrinter")
#If DEBUG Then
            .Add("SqliteAndroid")
#End If
        End With

        Dim pWC As Net.WebClient
        Dim pSourceUrl As String
        Dim pDestinationFilename As String
        Dim pPrefix As String
        Dim pStr As String
        Dim pOutputFolder As String
        Dim pDT As DataTable

        'pOutputFolder = "D:\Mobiz\assets\www\"
        pOutputFolder = "D:\PhoneGapMobiz\workspace\Mobiz\assets\www\"
        '"C:\Users\arvin\workspace2\Mobiz\assets\www\" ' Common.ServerPath & "temp\html\"

        '// copy folders
        pWC = New Net.WebClient

        For Each pStr In pFolders
            pDT = DirectoryToTable(Common.ServerPath & pStr, , {"*.scc"}, , , , , True)
            For Each pDR As DataRow In pDT.Rows
                pDestinationFilename = pOutputFolder & GetRelativePath(Common.ServerPath, pDR("FullName"))
                pDestinationFilename = pDestinationFilename.ToLower
                CreateFolder(pDestinationFilename)

                'If pDestinationFilename.Contains(".js") Then
                '    pStr = pWC.DownloadString(pDR("FullName"))

                '    pStr = AssignLinks(pStr, pDestinationFilename)
                '    FileText(pDestinationFilename) = pStr
                'Else
                '    CopyFile(pDR("FullName"), pDestinationFilename)
                'End If

                CopyFile(pDR("FullName"), pDestinationFilename)

            Next pDR
        Next pStr

        '// copy files

        For Each pStr In pFiles
            pDestinationFilename = pOutputFolder & GetRelativePath(Common.ServerPath, pStr.ToLower)
            pDestinationFilename = pDestinationFilename.ToLower
            CreateFolder(pDestinationFilename)
            CopyFile(Common.ServerPath & pStr, pDestinationFilename)
        Next pStr

        '// copy rendered controllers and actions
        pPrefix = Request.Url.Scheme & Uri.SchemeDelimiter & Request.Url.Authority
        pPrefix = AppendIfNeeded(pPrefix, "/")
        pWC = New Net.WebClient
        For Each pStr In pPages
            pSourceUrl = pPrefix & pStr
            pDestinationFilename = pOutputFolder & pStr.Replace("/", "\").ToLower & ".html"
            pDestinationFilename = pDestinationFilename.ToLower
            CreateFolder(pDestinationFilename)

            'pStr = pWC.DownloadString(pSourceUrl & "?IsMobile=1")
            'pStr = AssignLinks(pStr, pDestinationFilename)
            'FileText(pDestinationFilename) = pStr

            pWC.DownloadFile(pSourceUrl & "?IsMobile=1", pDestinationFilename)
        Next pStr

        ''// custom
        'pStr = "Scripts3/common/init.js"
        'pDestinationFilename = pOutputFolder & pStr.Replace("/", "\").ToLower
        'pSourceUrl = pPrefix & pStr
        'pStr = pWC.DownloadString(pSourceUrl)
        ''pStr = pStr.Replace("server: ''", "server: 'http://philhr.com:8080'") ''http://192.168.2.101 http://philhr.com http://localhost:3002 http://192.168.1.80 
        ''pStr = pStr.Replace("server: ''", "server: " & ReadyString(Request.Url.Scheme & Request.Url.SchemeDelimiter & Request.Url.Authority))
        'FileText(pDestinationFilename) = pStr

        Return DateTimeStamp() & " - Compiled to " & pOutputFolder

    End Function

    'Private Function AssignLinks(ByVal vString As String, ByVal pDestinationFilename As String) As String

    '    Dim ctr As Integer
    '    Dim sInputLine() As String
    '    Dim sStringResult As String
    '    Dim sSplitPath() As String

    '    sInputLine = Split(vString, vbCrLf)

    '    Do Until ctr >= sInputLine.Length
    '        sStringResult = sInputLine(ctr)
    '        If sStringResult.Contains("link") Or sStringResult.Contains("script") Then
    '            If sStringResult.Contains(".js") Or sStringResult.Contains(".css") Then

    '                Dim strSplit() = sStringResult.Split("""")
    '                Dim sFilter As Integer = 0

    '                While sFilter < strSplit.Length
    '                    If strSplit(sFilter).Contains(".js") Or strSplit(sFilter).Contains(".css") Then

    '                        Dim sPath As String = pDestinationFilename.Substring(pDestinationFilename.IndexOf("www"))

    '                        'get folder count from www
    '                        sSplitPath = sPath.Split("\")

    '                        If sSplitPath.Length > 2 Then
    '                            'parth is inside the folder adding '..' to move up one level
    '                            vString = vString.Replace(strSplit(sFilter), ".." & strSplit(sFilter))
    '                        Else
    '                            'remove '/'
    '                            vString = vString.Replace(strSplit(sFilter), strSplit(sFilter).ToString.Substring(1))
    '                        End If
    '                    End If
    '                    sFilter += 1
    '                End While

    '            End If

    '        ElseIf sStringResult.Contains("url") Then
    '            sSplitPath = sStringResult.Split(":")

    '            If sSplitPath.Length > 1 Then
    '                If sSplitPath(1).ToString.Contains("'") OrElse sSplitPath.Contains("""") Then

    '                    Dim sAbsPath As String = sSplitPath(1).Substring(2, sSplitPath(1).ToString.Length - 4)

    '                    If sAbsPath.Split("/").Length = 2 Then
    '                        vString = vString.Replace(sAbsPath, "www.PhilHR.com/" & sAbsPath)
    '                    End If
    '                End If
    '            End If
    '        End If

    '        ctr += 1
    '    Loop

    '    Return vString
    'End Function

    ''' <summary>
    ''' this function is for offline devices only
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    Public Function VerifyCounter() As String

        Dim pDR As DataRow
        Dim pCounter As Integer
        Dim pOut As JsonDictionary
        Dim pDeviceCounter As String
        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pFowstID As String
        Dim pFilename As String
        Dim pTimeStamp As String
        Dim pFieldName = Common.WebRequest.Data("FieldName")
        Dim pInvoiceFieldName = Common.WebRequest.Data("InvoiceFieldName")

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")  ' no need for logged-in account session. do not delete this commented line for reference
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied") ' no need for logged-in account session. do not delete this commented line for reference
        pOrgID = ReadyGuid(WebRequest.Instance.OrgID)

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Device ID")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        pDR = WebRequest.Instance.Business.LibraryDevice.ById(pDeviceID)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Record with the specified device ID does not exist.")
        End If
        pOut = New JsonDictionary
        If pDR("IsActive") = False Then
            pOut.Add("Error", "Record with the specified device ID does not exist.")
            pOut.Add("Deactivated", 1)
            Return pOut.Serialize
        End If

        WebRequest.Instance.Business.TransInvoice.VerifyCounter(pDeviceID, pFowstID, pCounter, , , pFieldName, pInvoiceFieldName)
        pDeviceCounter = Common.WebRequest.Data("DeviceCounter")
        If IsNumeric(pDeviceCounter) Then ' make this optional only. backwards compatible. 6/25/2014 1:07 PM
            If CInt2(pDeviceCounter) <> pCounter Then
                pOut.Add("Skip", CInt2(pDeviceCounter) - pCounter)
                pOut.Add("DeviceCounter", CInt2(pDeviceCounter))
                pTimeStamp = DateTimeStamp()
                pOut.Add("TimeStamp", pTimeStamp)
                pFilename = Common.ServerPath & "temp\SkippingInvoiceNumbers\" & pOrgID & "\" & pDeviceID & "\" & pTimeStamp & ".txt"
                FileText(pFilename) = "OrgID: " & pOrgID & vbCrLf &
                    "DeviceID: " & pDeviceID & vbCrLf &
                    "FowstID: " & pFowstID & vbCrLf &
                    "DeviceCount: " & pDeviceCounter & vbCrLf &
                    "ServerCount: " & pCounter & vbCrLf &
                    "Skip: " & CInt2(pDeviceCounter) - pCounter & vbCrLf &
                    "TimeStamp: " & pTimeStamp
            End If
        End If

        pOut.Add("Counter", pCounter)

        Return pOut.Serialize

    End Function

    Public Function HotButtons() As ActionResult

        Return View()

    End Function

    Public Function BsHotButtons() As ActionResult
        'bootstrap theme
        Return View()

    End Function

    <HttpPost()>
    Public Function GetPOSMode() As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("POSMode", .POSMode)
        End With
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("IsShowTableNo", .IsShowTableNo)
        End With
        'must be in database!!
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("IsShowGasPumpNo", .IsShowGasPumpNo)
        End With
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("NoOfTables", .NoOfTables)
        End With
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("NoOfGasPumps", .NoOfGasPumps)
        End With

        Return pOut.Serialize

    End Function

    ' ADDITIONAL BY janine
    <HttpPost()>
    Public Function GetCustomerPriceLevel() As String
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT org.PriceLevelID, pim.PriceLevel" & vbCrLf &
        " FROM LibraryOrganizationPriceLevel org INNER JOIN MobizLibraryPIMPriceLevel pim" & vbCrLf &
        " ON org.PriceLevelID = pim.ID" & vbCrLf &
        " WHERE org.OrganizationID =" & ReadyString(Common.WebRequest.Data("CustomerOrganizationID"))

        pSql &= " ORDER BY pim.PriceLevel"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("Data", pDT)

        Return strReturn

    End Function


    <HttpPost()>
    Public Function SaveCustomerDetails() As String
        Dim strReturn As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary

        Try

            pIU = New InsertUpdate("TransInvoice")

            'If CStr2(Common.WebRequest.Data("CustomerNameReferenceOnly")).Length > 0 Then pIU("CustomerNameReferenceOnly") = ReadyString(Common.WebRequest.Data("CustomerNameReferenceOnly"))
            If CStr2(Common.WebRequest.Data("CustomerTableNo")).Trim().Length > 0 Then pIU("CustomerTableNo") = ReadyString(Common.WebRequest.Data("CustomerTableNo").Trim())
            'If CStr2(Common.WebRequest.Data("CustomerTableNoOfPeople")).Length > 0 Then pIU("CustomerTableNoOfPeople") = Common.WebRequest.Data("CustomerTableNoOfPeople")
            'If CStr2(Common.WebRequest.Data("CustomerReferenceNumber")).Length > 0 Then pIU("CustomerReferenceNumber") = Common.WebRequest.Data("CustomerReferenceNumber")

            'pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("InvoiceID")))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pOut.Add("Result", "ok")

        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function SaveCustomerDemographics() As String
        Dim strReturn As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary

        Try

            pIU = New InsertUpdate("TransInvoice")

            If CStr2(Common.WebRequest.Data("DeliveryMethod")).Length > 0 Then pIU("DeliveryMethod") = ReadyString(Common.WebRequest.Data("DeliveryMethod"))
            If CStr2(Common.WebRequest.Data("OrderMethod")).Trim().Length > 0 Then pIU("OrderMethod") = ReadyString(Common.WebRequest.Data("OrderMethod"))
            If CStr2(Common.WebRequest.Data("Gender")).Length > 0 Then pIU("CustomerDemographicGender") = ReadyString(Common.WebRequest.Data("Gender"))
            If CStr2(Common.WebRequest.Data("Age")).Length > 0 Then pIU("CustomerDemographicAge") = ReadyString(Common.WebRequest.Data("Age"))

            pSql = pIU.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("ID")))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pOut.Add("Result", "ok")

        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function UpdateOrderedQuantity() As String
        Dim strReturn As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As JsonDictionary
        Dim pDT As DataTable
        Dim intCt As Integer

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary

        Try
            pSql = "SELECT ID, OrderedQuantity, Quantity FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = " & ReadyString(Common.WebRequest.Data("InvoiceID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                While (intCt < pDT.Rows.Count)

                    If (CDec2(pDT.Rows(intCt).Item("Quantity").ToString()) = 0) Then
                        pIU = New InsertUpdate("TransInvoiceItems")

                        pIU("Quantity") = CDec2(pDT.Rows(intCt).Item("OrderedQuantity").ToString())

                        pSql = pIU.GetUpdate("ID = " & ReadyString(pDT.Rows(intCt).Item("ID").ToString()))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If


                    intCt = intCt + 1
                End While
            End If

            pOut.Add("Result", "ok")

        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function Upload() As String

        Dim pFilename As String
        Dim pOrgID As String
        pOrgID = Common.WebRequest.Data("OrgID")
        Dim pOut As JsonDictionary

        pOut = New JsonDictionary
        Try
            pFilename = Common.ServerPath & "temp\Uploads\SalesOrderSignature\" & pOrgID & "\" & Common.WebRequest.Data("Filename") & ".txt"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)
        Catch ex As System.Exception
            pOut.Add("Error", "An error occured while saving imported file: " & ex.Message)
            Return pOut.Serialize
        End Try


        pOut.Add("Result", "ok")

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function GetSalesOrderSignature() As String

        Dim pFilename As String
        Dim pOrgID As String
        pOrgID = Common.WebRequest.Data("OrgID")
        Dim pOut As JsonDictionary
        Dim pDT As DataTable
        Dim pSql As String
        Dim textinfile As String

        pOut = New JsonDictionary
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try

            pFilename = Common.ServerPath & "temp\Uploads\SalesOrderSignature\" & pOrgID & "\" & Common.WebRequest.Data("Filename") & ".txt"

            textinfile = FileText(pFilename)
            If (CStr2(textinfile) <> "") Then
                pOut.Add("ImageURL", FileText(pFilename))
            Else
                pOut.Add("ImageURL", "")

            End If


            pSql = "SELECT SalesOrderSignatory" & vbCrLf &
            " FROM TransInvoice WHERE SalesOrderSignatory IS NOT NULL AND ID =  " & ReadyString(Common.WebRequest.Data("Filename"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                pOut.Add("SignatoryName", pDT.Rows(0).Item("SalesOrderSignatory").ToString())
            Else

                pOut.Add("SignatoryName", "Not provided")
            End If


        Catch ex As Exception



            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize

    End Function

    Public Function DucToMobizInvoice() As String

        Dim pOut As String

        SyncLock mDucToMobizInvoiceLock
            pOut = WebRequest.Instance.Business.TransInvoice.DucToMobizInvoice()
        End SyncLock

        Return pOut

    End Function

    <HttpPost()>
    Public Function TestPrice() As String

        Dim pTestPriceJson As TestPriceJson
        Dim pBusiness As Business
        Dim pPriceRequest As TransSalesOrderLegacy.PriceRequest
        Dim pResult As Boolean
        Dim pMessage As String
        Dim pOut As JsonDictionary
        Dim pInput As String

        Dim pItemCount As Integer = -1

        Try
            pInput = Common.WebRequest.InputStreamText
#If DEBUG Then
            If False Then
                pInput = FileText("C:\dev\for debugging\TestPrice 15_11_30\input.json")
            End If
#End If
            pTestPriceJson = JsonConvert.DeserializeObject(Of TestPriceJson)(pInput)
            If WebRequest.Instance.Business.Utilities.HasWholesaleDatabase Then
                pBusiness = New Business(ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:="wholesale",
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
            ))
                pPriceRequest = New TransSalesOrderLegacy.PriceRequest(pBusiness)
                pPriceRequest.CustomerID = pTestPriceJson.CustomerID
                pItemCount = pPriceRequest.GetItems(pTestPriceJson.Products)
                pResult = pPriceRequest.HandleRequestedPrices(False, pMessage)
            Else
                pResult = True
                pMessage = "Approved"
            End If

            pOut = New JsonDictionary
            pOut.Add("IsOk", If(pResult, 1, 0))
            pOut.Add("Message", pMessage)

            Return pOut.Serialize()
        Catch ex As Exception
            AppendToLog(ex.Message & vbCrLf &
                "pItemCount: " & pItemCount & vbCrLf &
                Common.WebRequest.InputStreamText & vbCrLf & GetStackTrace(, ex), , "TestPrice")
            Return JsonDictionary.Serialize("Message", ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' called from Sales Order Processing & Analytics
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    Public Function Approve() As String

        Dim pInvoiceID As String
        Dim pRecordCount As Integer
        Dim pError As String
        Dim pTrailNetSuite As TrailNetSuite
        Dim pDatabaseName As String
        Dim pProData As ProData
        Dim pOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")

        pRecordCount = WebRequest.Instance.Business.TransInvoice.Approve(pInvoiceID, WebSession.Instance.PeopleUserID, pError, WebRequest.Instance.OrgID)
        WebRequest.Instance.Business.TransInvoice.CopyOrderQtyToServedQtyIfServedQtyIsNotEdited(pInvoiceID)
        pDatabaseName = Common.WebRequest.ProData.GetCurrentDatabase
        pProData = Common.WebRequest.ProData
        pOrgID = WebRequest.Instance.OrgID

        TimedThread(
            Sub()
                Try
                    pProData = pProData.Clone()
                    pTrailNetSuite = New TrailNetSuite(pProData)
                    pTrailNetSuite.OrgID = pOrgID
                    pTrailNetSuite.Start()
                Catch ex As Exception
                    AppendToLog("vbC8WQ3XWPMS! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "Approve vbC8WQ3XWPMS!")
                End Try
            End Sub)

        If 0 < pRecordCount Then
            Return JsonDictionary.Serialize("Result", "ok")
        ElseIf String.IsNullOrWhiteSpace(pError) Then
            Return JsonDictionary.Serialize("Error", "No record was affected. Nothing was approved.")
        Else
            Return JsonDictionary.Serialize("Error", pError)
        End If

    End Function

    <HttpPost()>
    Public Function FinalizePickList() As String

        Dim pInvoiceID As String
        Dim pRecordCount As Integer
        Dim pError As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")

        WebRequest.Instance.ProData.BeginTransaction()
        WebRequest.Instance.Business.TransInvoice.CopyOrderQtyToServedQtyIfServedQtyIsNotEdited(pInvoiceID)
        Try
            pRecordCount = WebRequest.Instance.Business.TransInvoice.FinalizePickList(pInvoiceID, WebSession.Instance.PeopleUserID, WebRequest.Instance.OrgID)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
        WebRequest.Instance.ProData.CommitTransaction()
        If 0 < pRecordCount Then
            Return JsonDictionary.Serialize("Result", "ok")
        ElseIf String.IsNullOrWhiteSpace(pError) Then
            Return JsonDictionary.Serialize("Error", "No record was affected. Nothing was approved.")
        Else
            Return JsonDictionary.Serialize("Error", pError)
        End If

    End Function

    ''' <summary>
    ''' called by on-premise print server
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    Public Function GetAndIncrementPrintCount() As String

        Dim pInvoiceID As String
        Dim pDR As DataRow
        Dim pPrintCount As Integer
        Dim pApprovedUnprintedRequests As DataTable

        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be a GUID")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            pDR = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
            If pDR Is Nothing Then
                Return JsonDictionary.Serialize("Error", "Invoice, " & pInvoiceID & ", does not exist.")
            End If
            pPrintCount = CInt2(pDR("PrintCount"))
            If pPrintCount = 0 Then
                WebRequest.Instance.Business.TransInvoice.IncrementPrintCount(pInvoiceID)
            Else
                pApprovedUnprintedRequests = WebRequest.Instance.Business.TransInvoice.ApprovedUnprintedRequests(pInvoiceID)
                If pApprovedUnprintedRequests.Rows.Count > 0 Then
                    WebRequest.Instance.ProData.BeginTransaction()
                    Try
                        WebRequest.Instance.Business.TransInvoice.TagRequestAsPrinted(
                            vInvoicePrintID:=CStr2(pApprovedUnprintedRequests.Rows(0)("ID")),
                            vPrintedByID:=WebSession.Instance.PeopleUserID)
                        WebRequest.Instance.Business.TransInvoice.IncrementPrintCount(pInvoiceID)
                        WebRequest.Instance.ProData.CommitTransaction()
                        pPrintCount = 0
                    Catch ex As Exception
                        WebRequest.Instance.ProData.RollbackTransaction()
                        Throw
                    End Try
                End If
            End If

            Return JsonDictionary.Serialize("PrintCount", pPrintCount)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", "vbNS96S6K6LS! " & ex.Message)
        End Try

    End Function ' END GetAndIncrementPrintCount

    Public Function GetByInvoiceNumFowstIDDeviceSerial() As String

        Dim pInvoiceID As String
        Dim pRemarks As String
        Dim pInvoiceNum As String
        Dim pFowstID As String
        Dim pDeviceSerial As String
        Dim pDR As DataRow
        Dim pOut As JsonDictionary

        pInvoiceNum = Common.WebRequest.Data("InvoiceNum")
        pFowstID = Common.WebRequest.Data("FowstID")
        pDeviceSerial = Common.WebRequest.Data("DeviceSerial")


        pDR = WebRequest.Instance.Business.TransInvoice.GetByInvoiceNumFowstIDDeviceSerial(pInvoiceNum, pFowstID, pDeviceSerial)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "The specified invoice does not exist.")
        End If

        pOut = New JsonDictionary()
        pOut.Add("data", pDR)

        Return pOut.Serialize()

    End Function

    ''' <summary>
    ''' this should be running in on-premise server
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PrintPdf() As String

        Dim pFilename As String
        Dim pTimestamp As String

        If Request.Files.Count > 0 Then
            pTimestamp = Common.WebRequest.Data("TimeStamp")
            If CStr2(pTimestamp).Trim = "" Then
                pTimestamp = DateTimeStamp()
            End If
            pFilename = Common.ServerPath & "temp\PdfToBePrinted\" & pTimestamp & ".x"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)
        Else
            Return JsonDictionary.Serialize("Error", "No file was uploaded.")
        End If

        Return JsonDictionary.Serialize("TimeStamp", pTimestamp)

    End Function

    <HttpPost()>
    Public Function RequestReprint() As String

        Dim pInvoiceID As String
        Dim pRemarks As String
        Dim pInvoiceNum As String
        Dim pFowstID As String
        Dim pDeviceSerial As String
        Dim pDR As DataRow

        Try
            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                pInvoiceNum = Common.WebRequest.Data("InvoiceNum")
                pFowstID = Common.WebRequest.Data("FowstID")
                pDeviceSerial = Common.WebRequest.Data("DeviceSerial")
            End If
            pRemarks = Common.WebRequest.Data("Remarks")

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDR = WebRequest.Instance.Business.TransInvoice.GetByInvoiceNumFowstIDDeviceSerial(pInvoiceNum, pFowstID, pDeviceSerial)
            If pDR Is Nothing Then
                Return JsonDictionary.Serialize("Error", "The specified invoice does not exist.")
            End If
            pInvoiceID = CStr2(pDR("ID"))

            If WebRequest.Instance.Business.TransInvoice.HasPendingPrintRequest(pInvoiceID) Then
                JsonDictionary.Serialize("Error", "The specified invoice has a pending reprint request. Please wait for your request to be approved.")
            End If

            WebRequest.Instance.Business.TransInvoice.AddReprintRequest(vInvoiceID:=pInvoiceID,
                vRequestedByID:=WebSession.Instance.PeopleUserID,
                vRemarks:=pRemarks)

            Return JsonDictionary.Serialize("Success", 1)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message & "vbM3UV8EW3XN!")
        End Try

    End Function

    <HttpPost()>
    Public Function ApproveReprintRequest() As String

        Dim pInvoiceID As String
        Dim pApprove As Boolean
        Dim pDT As DataTable

        Try
            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
            End If
            pApprove = CBool2(Common.WebRequest.Data("Approve"))

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDT = WebRequest.Instance.Business.TransInvoice.UnapprovedReprintRequests(pInvoiceID)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "There are no unapproved reprint request for this invoice.")
            End If

            WebRequest.Instance.Business.TransInvoice.ApproveReprintRequest(vInvoicePrintID:=CStr2(pDT.Rows(0)("ID")),
                vApprove:=pApprove,
                vApprovedByID:=WebSession.Instance.PeopleUserID)

            Return JsonDictionary.Serialize("Success", 1)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message & "vb6EAYMMYP8A!")
        End Try

    End Function

    Private Class TestPriceJson
        Public CustomerID As String
        Public Products As List(Of TransSalesOrderLegacy.Product)
    End Class

    <HttpPost>
    Public Function GetCacheInvoice() As String

        Dim pInvoiceID As String
        Dim pDrInvoice As DataRow
        Dim pOut As JsonDictionary
        Dim pSql As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")

        pOut = New JsonDictionary

        pSql = "SELECT *" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE ID = " & ReadyString(pInvoiceID)
        pDrInvoice = Common.WebRequest.ProData.ExecuteRow(pSql)
        pOut.Add("Invoice", pDrInvoice)
        pOut.Add("Adjustments", WebRequest.Instance.Business.TransInvoice.GetAdjustments(pInvoiceID))

        Return pOut.Serialize

    End Function

    <HttpPost>
    Public Function GetInvoiceDetails() As String

        Dim pTableNum As String = Common.WebRequest.Data("TableNum")
        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pDT = WebRequest.Instance.Business.TransInvoice.SalesMenuGetInvoiceDetails(vTableNum:=pTableNum)

        Return JsonDictionary.Serialize(pDT)

    End Function

    Private Shared SaveAndGetPeerMessagesLock As New Object

    <HttpPost>
    Public Function SaveAndGetPeerMessages() As String

        Dim pSourceIP As String = Request.UserHostAddress
        Dim pDestinationIP As String = Common.WebRequest.Data("DestinationIP")
        Dim pMessage As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As New JsonDictionary()
        Dim pDT As DataTable

        If Common.WebRequest.ProData.TableExist("LibraryPeople") Then
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If

        SyncLock SaveAndGetPeerMessagesLock
            Try
                pMessage = Common.WebRequest.Data("Message")
            Catch ex As Exception
                pMessage = JsonConvert.SerializeObject(Common.WebRequest.AllData("Message"))
            End Try
            pMessage = ReplaceIp(pMessage, pSourceIP)
            If String.IsNullOrWhiteSpace(pMessage) Then
                Return Nothing
            End If

            Common.WebRequest.ProData.BeginTransaction()
            pSql = "DELETE FROM TransPeerMessages" & vbCrLf &
                " WHERE DateReceived < " & ReadyString(Format(Now.AddDays(-1), cGlobal.FormatDateTimeSql2))
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used for debugging only

            pIU = New InsertUpdate("TransPeerMessages")
            pIU("SourceIP") = ReadyString(pSourceIP)
            pIU("DestinationIP") = ReadyString(pDestinationIP)
            pIU("Message") = ReadyString(pMessage)
            pIU("DateReceived") = "GETDATE()"
            pSql = pIU.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "SELECT TOP 50 Message, SourceIP" & vbCrLf &
                " FROM TransPeerMessages" & vbCrLf &
                " WHERE DestinationIP = " & ReadyString(pSourceIP) & vbCrLf &
                " ORDER BY DateReceived"
            pDT = Common.WebRequest.ProData.Execute(pSql)
            pOut.Add("Messages", pDT)

            pSql = "DELETE FROM TransPeerMessages" & vbCrLf &
                " WHERE Num IN (" & vbCrLf &
                " SELECT TOP 50 Num" & vbCrLf &
                " FROM TransPeerMessages" & vbCrLf &
                " WHERE DestinationIP = " & ReadyString(pSourceIP) & vbCrLf &
                " ORDER BY DateReceived)"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return pOut.Serialize()
        End SyncLock

    End Function

    Function ReplaceIp(vMessage As String, vSourceIp As String) As String

        Dim pJObj As JObject
        Dim pOut As String

        Try
            pJObj = JObject.Parse(vMessage)
            pJObj("ip") = vSourceIp
            pOut = pJObj.ToString(Formatting.None)
            Return pOut
        Catch ex As Exception
            AppendToLog("Error in ReplaceIp. ref: vb3E6J56JB3D!" & vbCrLf &
                "vMessage: " & vMessage & vbCrLf &
                "vSourceIp: " & vSourceIp & vbCrLf &
                ex.Message)
        End Try

    End Function

    <HttpPost>
    Public Function HideServedQuantity() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceItemID As String = Common.WebRequest.Data("InvoiceItemID")
        Dim pHideQuantity As Boolean = CBool2(Common.WebRequest.Data("HideQuantity"))
        Dim pResult As Integer

        If IsGuid(pInvoiceItemID) = False Then
            Return JsonDictionary.Serialize("Error", "InvoiceItemID must be a GUID. ref vbPDN4JSJFLV!")
        End If

        pResult = WebRequest.Instance.Business.TransInvoice.HideServedQuantity(pInvoiceItemID, WebSession.Instance.PeopleUserID, pHideQuantity)
        If pResult = 1 Then
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Result", "fail")
        End If

    End Function

    <HttpPost>
    Public Function GetCommonLineItemCount() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceID1 As String = Common.WebRequest.Data("InvoiceID1")
        Dim pInvoiceID2 As String = Common.WebRequest.Data("InvoiceID2")
        Dim pResult As Integer

        If IsGuid(pInvoiceID1) = False Then
            Return JsonDictionary.Serialize("Error", "InvoiceID1 must be a GUID. ref vbY72DMNVA75!")
        End If

        If IsGuid(pInvoiceID2) = False Then
            Return JsonDictionary.Serialize("Error", "InvoiceID2 must be a GUID. ref vbGU8EJZMKKS!")
        End If

        pResult = WebRequest.Instance.Business.TransInvoice.GetCommonLineItemCount(pInvoiceID1, pInvoiceID2)

        Return JsonDictionary.Serialize("Result", pResult)

    End Function

    <HttpPost>
    Public Function ReaddHideServedQuantity() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceItemID As String = Common.WebRequest.Data("InvoiceItemID")
        Dim pResult As Integer

        If IsGuid(pInvoiceItemID) = False Then
            Return JsonDictionary.Serialize("Error", "InvoiceItemID must be a GUID. ref vbZWNU97DHDQ!")
        End If

        pResult = WebRequest.Instance.Business.TransInvoice.ReaddHideServedQuantity(pInvoiceItemID, WebSession.Instance.PeopleUserID)
        If pResult = 1 Then
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Result", "fail")
        End If

    End Function

    <HttpPost>
    Public Function SaveFowst() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")
        Dim pFowstID As String = Common.WebRequest.Data("FowstID")
        Dim pResult As Boolean

        pResult = WebRequest.Instance.Business.TransInvoice.SaveFowst(pInvoiceID, pFowstID, WebSession.Instance.PeopleUserID)
        If pResult Then
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Result", "fail")
        End If

    End Function

    Public Function GetAvailableToCommitProducts() As String
        Dim strReturn As String
        Dim FOWSTID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pDT As DataTable
        'Dim pSql As String

        FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))

        'pSql = "SELECT t1.FOWSTID, SUM(OrderedQuantity) Quantity FROM TransInvoice t1 " & vbCrLf &
        '        "inner join TransInvoiceItems on t1.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
        '        "WHERE TransType = 'SalesOrder' " & vbCrLf &
        '        " AND ISNULL(t1.IsCancelled, 0) = 0" & vbCrLf &
        '        " AND NOT EXISTS (SELECT * FROM TransInvoice t2 " & vbCrLf &
        '        "WHERE invoicenumber > 0 " & vbCrLf &
        '        "AND t2.SalesOrderID = t1.ID)  " & vbCrLf &
        '        "AND TransInvoiceItems.ProductID = " & ReadyString(CStr2(Common.WebRequest.Data("ProductID")))

        'If FOWSTID.Length > 0 Then
        '    pSql &= "AND t1.FOWSTID = " & ReadyString(FOWSTID)
        'Else
        '    Debug.Print("vb8MNWZ3SF28!")
        'End If

        'pSql &= " GROUP BY t1.FOWSTID"

        pDT = WebRequest.Instance.Business.LibraryStock.GetCachedCommittedQty(Common.WebRequest.Data("ProductID"), FOWSTID)
        pDT.Columns("CommittedQty").ColumnName = "Quantity"
        pDT.Columns("FowstID").ColumnName = "FOWSTID"

        strReturn = JsonDictionary.Serialize("Results", pDT)

        Return strReturn
    End Function

    <HttpPost>
    Public Function GetSettings() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pJson = New JsonDictionary()
        pJson.Add("PickListPrintCount", WebRequest.Instance.Business.SystemDefaults.PickListPrintCount)
        pJson.Add("FcSettingExcludeVendorPoInFc", If(WebRequest.Instance.Business.SystemDefaults.FcSettingExcludeVendorPoInFc, 1, 0))

        Return pJson.Serialize

    End Function

    Public Class SaveSettingsJson
        Public PickListPrintCount As String
        Public FcSettingExcludeVendorPoInFc As String
    End Class

    <HttpPost>
    Public Function SaveSettings() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pInput = JsonConvert.DeserializeObject(Of SaveSettingsJson)(Common.WebRequest.InputStreamText)

        WebRequest.Instance.Business.SystemDefaults.PickListPrintCount = CInt2(pInput.PickListPrintCount)
        WebRequest.Instance.Business.SystemDefaults.FcSettingExcludeVendorPoInFc = CBool2(pInput.FcSettingExcludeVendorPoInFc)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function SaveSomeFields() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Modify Finalized Invoice", False) = False Then
                Return JsonDictionary.Serialize("Error", "You need Modify Finalized Invoice security access.")
            End If

            Dim pInput = JsonConvert.DeserializeObject(Of TransInvoice.SaveSomeFieldsInput)(Common.WebRequest.InputStreamText)
            pInput.UserID = WebSession.Instance.PeopleUserID
            WebRequest.Instance.Business.TransInvoice.SaveSomeFields(pInput)
            WebRequest.Instance.Business.TransReceiving.CacheReceivingsWithCredit()
            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost>
    Public Function IsSopaApproved() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim pResult = WebRequest.Instance.Business.TransInvoice.IsApproved(WebRequest.Instance.Data("InvoiceID"))
            Return JsonDictionary.Serialize("Result", If(pResult, 1, 0))
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function ResaveRoutingSheet()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim DeliveryLoggerID As String = Common.WebRequest.Data("DeliveryLoggerID")
            Dim RoutingSheetNumber As String = Common.WebRequest.Data("RoutingSheetNumber")
            Dim DeliveryMethod As String = Common.WebRequest.Data("DeliveryMethod")
            Dim DeliveryTruckFowstID As String = Common.WebRequest.Data("DeliveryTruckFowstID")
            Dim Delivery3PlCompanyID As String = Common.WebRequest.Data("Delivery3PlCompanyID")
            Dim Delivery3PlCompanyName As String = Common.WebRequest.Data("Delivery3PlCompanyName")
            Dim Delivery3PlContactPerson As String = Common.WebRequest.Data("Delivery3PlContactPerson")
            Dim DeliveryWaybillNumber As String = Common.WebRequest.Data("DeliveryWaybillNumber")
            Dim DeliveryCustomerRepresentative As String = Common.WebRequest.Data("DeliveryCustomerRepresentative")
            Dim DeliveryEncodingDate As String = Common.WebRequest.Data("DeliveryEncodingDate")
            Dim DeliveryRemarks As String = Common.WebRequest.Data("DeliveryRemarks")
            Dim IsDraft As Boolean = CBool2(Common.WebRequest.Data("IsDraft"))
            Dim Invoices As Object = Common.WebRequest.AllData("Invoices")
            Dim DeliveryChecker As Object = Common.WebRequest.AllData("Checker")
            Dim DeliveryDriver As Object = Common.WebRequest.AllData("Driver")
            Dim DeliveryHelper As Object = Common.WebRequest.AllData("Helper")
            Dim Reason As String = Common.WebRequest.Data("Reason")
            Dim pIU As New InsertUpdate("TransDeliveryLogger")
            Dim pSql As String
            Dim newsheet As Boolean = False
            Dim pInvoiceIDs As String = ""

            pIU("DeliveryEncodingDate") = ReadyString(DeliveryEncodingDate)
            pIU("DeliveryMethod") = ReadyString(Truncate(DeliveryMethod, 100))
            pIU("LastUpdated") = "GETDATE()"

            If String.IsNullOrWhiteSpace(DeliveryTruckFowstID) = False Then
                pIU("DeliveryTruckFowstID") = ReadyString(Truncate(DeliveryTruckFowstID, 100))
            Else
                pIU("DeliveryTruckFowstID") = "NULL"
            End If
            If String.IsNullOrWhiteSpace(Delivery3PlCompanyID) = False Then
                pIU("Delivery3PlCompanyID") = ReadyString(Delivery3PlCompanyID)
            Else
                pIU("Delivery3PlCompanyID") = "NULL"
            End If
            'If String.IsNullOrWhiteSpace(Delivery3PlCompanyName) = False Then
            '    pIU("Delivery3PlCompanyName") = ReadyString(Truncate(Delivery3PlCompanyName, 500))
            'Else
            '    pIU("Delivery3PlCompanyName") = "NULL"
            'End If
            If String.IsNullOrWhiteSpace(Delivery3PlContactPerson) = False Then
                pIU("Delivery3PlContactPerson") = ReadyString(Truncate(Delivery3PlContactPerson, 500))
            Else
                pIU("Delivery3PlContactPerson") = "NULL"
            End If
            If String.IsNullOrWhiteSpace(DeliveryWaybillNumber) = False Then
                pIU("DeliveryWaybillNumber") = ReadyString(Truncate(DeliveryWaybillNumber, 500))
            Else
                pIU("DeliveryWaybillNumber") = "NULL"
            End If
            If String.IsNullOrWhiteSpace(DeliveryCustomerRepresentative) = False Then
                pIU("DeliveryCustomerRepresentative") = ReadyString(Truncate(DeliveryCustomerRepresentative, 500))
            Else
                pIU("DeliveryCustomerRepresentative") = "NULL"
            End If
            If String.IsNullOrWhiteSpace(DeliveryRemarks) = False Then
                pIU("DeliveryNotes") = ReadyString(DeliveryRemarks)
            Else
                pIU("DeliveryNotes") = "NULL"
            End If
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsDraft") = CInt2(IsDraft)

            If String.IsNullOrWhiteSpace(DeliveryLoggerID) Then
                'kapag NULL ang DeliveryLoggerID, wala pa sa header table ang RS number, so ibutang nato, by Arnie 3-18-2023
                DeliveryLoggerID = Guid.NewGuid.ToString()
                newsheet = True
                pIU("RoutingSheetNumber") = ReadyString(RoutingSheetNumber)
                pIU("ID") = ReadyString(DeliveryLoggerID)
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate(" ID = " & ReadyString(DeliveryLoggerID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each inv As Object In Invoices
                If pInvoiceIDs <> "" Then
                    pInvoiceIDs &= ", "
                End If
                pInvoiceIDs &= ReadyString(inv("InvoiceID").ToString())

                pIU = New InsertUpdate("TransInvoice")
                pIU("IsDelivered") = IIf(inv("Status") = "Delivered", 1, 0)
                pIU("RoutingSheetNumber") = "NULL"
                pIU("DeliveryStatus") = ReadyString(inv("Status"))
                pIU("DeliveryEncodingDate") = IIf(CStr2(inv("InvoiceStatusInRS").ToString()) = "", ReadyString(DeliveryEncodingDate), "NULL") 'NULL kapag iremove
                pIU("LastUpdated") = "GETDATE()"
                If String.IsNullOrWhiteSpace(DeliveryTruckFowstID) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("DeliveryTruckFowstID") = ReadyString(Truncate(DeliveryTruckFowstID, 100))
                Else
                    pIU("DeliveryTruckFowstID") = "NULL"
                End If
                If String.IsNullOrWhiteSpace(Delivery3PlCompanyID) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("Delivery3PlCompanyID") = ReadyString(Delivery3PlCompanyID)
                Else
                    pIU("Delivery3PlCompanyID") = "NULL"
                End If
                If String.IsNullOrWhiteSpace(Delivery3PlCompanyName) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("Delivery3PlCompanyName") = ReadyString(Truncate(Delivery3PlCompanyName, 500))
                Else
                    pIU("Delivery3PlCompanyName") = "NULL"
                End If
                If String.IsNullOrWhiteSpace(Delivery3PlContactPerson) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("Delivery3PlContactPerson") = ReadyString(Truncate(Delivery3PlContactPerson, 500))
                Else
                    pIU("Delivery3PlContactPerson") = "NULL"
                End If
                If String.IsNullOrWhiteSpace(DeliveryWaybillNumber) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("DeliveryWaybillNumber") = ReadyString(Truncate(DeliveryWaybillNumber, 500))
                Else
                    pIU("DeliveryWaybillNumber") = "NULL"
                End If
                If String.IsNullOrWhiteSpace(DeliveryCustomerRepresentative) = False And CStr2(inv("InvoiceStatusInRS").ToString()) = "" Then
                    pIU("DeliveryCustomerRepresentative") = ReadyString(Truncate(DeliveryCustomerRepresentative, 500))
                Else
                    pIU("DeliveryCustomerRepresentative") = "NULL"
                End If
                If newsheet Or CStr2(inv("Status")) = "Delivered" Or CBool2(inv("IsNewlyAddedInvoice")) Or CStr2(inv("InvoiceStatusInRS").ToString()) = "remove" Then
                    pSql = pIU.GetUpdate(" ID = " & ReadyString(inv("InvoiceID").ToString()))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                pIU = New InsertUpdate("TransDeliveryLoggerInvoices")
                pIU("Status") = ReadyString(inv("Status"))
                pIU("DeliveryEncodingDate") = ReadyString(DeliveryEncodingDate)

                If inv("Status") = "Undelivered" Then
                    pIU("DeliveryLoggerID") = "NULL"
                Else
                    pIU("DeliveryLoggerID") = ReadyString(DeliveryLoggerID)
                End If

                If Common.WebRequest.ProData.ExecuteSQL(pIU.GetUpdate(" DeliveryLoggerID = " & ReadyString(DeliveryLoggerID) & " AND InvoiceID = " & ReadyString(inv("InvoiceID").ToString()))) = 0 Then
                    pIU("InvoiceID") = ReadyString(inv("InvoiceID").ToString())
                    pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = pIU.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                If inv("Status") = "Undelivered" Then
                    'remove invoice from current Routing Sheet and move to logs table for reference
                    pIU = New InsertUpdate("TransDeliveryLogs")
                    pIU("DeliveryLoggerID") = ReadyString(DeliveryLoggerID)
                    pIU("InvoiceID") = ReadyString(inv("InvoiceID").ToString())
                    pIU("Reason") = IIf(String.IsNullOrWhiteSpace(Reason), "NULL", ReadyString(Reason))
                    pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                    pIU("DeliveryEncodingDate") = ReadyString(DeliveryEncodingDate)
                    pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                End If

                If DeliveryMethod = "Inhouse Delivery" Then
                    If inv("Status") = "Undelivered" Then
                        pIU = New InsertUpdate("TransInvoiceDeliveryDetails")
                        pIU("IsActive") = 0
                        pIU("LastUpdated") = "getdate()"
                        pSql = pIU.GetUpdate(" InvoiceID = " & ReadyString(inv("InvoiceID").ToString()))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        pSql = "DELETE FROM TransInvoiceDeliveryDetails WHERE InvoiceID = " & ReadyString(inv("InvoiceID").ToString()) & " AND DeliveryLoggerID = " & ReadyString(DeliveryLoggerID)
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        For Each people As Object In DeliveryChecker
                            pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                            pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                            pIU("DeliveryLoggerID") = ReadyString(DeliveryLoggerID)
                            pIU("InvoiceID") = ReadyString(inv("InvoiceID").ToString())
                            pIU("PeopleID") = ReadyString(people("ID").ToString())
                            pIU("Designation") = ReadyString("checker")
                            pIU("LastUpdated") = "getdate()"
                            pIU("IsActive") = 1
                            Common.WebRequest.ProData.Execute(pIU.GetInsert())
                        Next

                        For Each people As Object In DeliveryDriver
                            pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                            pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                            pIU("DeliveryLoggerID") = ReadyString(DeliveryLoggerID)
                            pIU("InvoiceID") = ReadyString(inv("InvoiceID").ToString())
                            pIU("PeopleID") = ReadyString(people("ID").ToString())
                            pIU("Designation") = ReadyString("driver")
                            pIU("LastUpdated") = "getdate()"
                            pIU("IsActive") = 1
                            Common.WebRequest.ProData.Execute(pIU.GetInsert())
                        Next

                        For Each people As Object In DeliveryHelper
                            pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                            pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                            pIU("DeliveryLoggerID") = ReadyString(DeliveryLoggerID)
                            pIU("InvoiceID") = ReadyString(inv("InvoiceID").ToString())
                            pIU("PeopleID") = ReadyString(people("ID").ToString())
                            pIU("Designation") = ReadyString("helper")
                            pIU("LastUpdated") = "getdate()"
                            pIU("IsActive") = 1
                            Common.WebRequest.ProData.Execute(pIU.GetInsert())
                        Next
                    End If
                End If
            Next

            If String.IsNullOrWhiteSpace(pInvoiceIDs) = False Then
                pSql = "DELETE FROM TransDeliveryLoggerInvoices WHERE InvoiceID NOT IN (" & pInvoiceIDs & ") AND DeliveryLoggerID = " & ReadyString(DeliveryLoggerID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "saved")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Public Function GetRoutingSheetDetails()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim pRoutingSheetNumber As String = CStr2(Common.WebRequest.Data("RoutingSheetNumber"))
            Dim pSql As String
            Dim pDT As DataTable
            Dim pOut As New JsonDictionary
            Dim pIDs As String = ""

            If String.IsNullOrWhiteSpace(pRoutingSheetNumber) Then
                pRoutingSheetNumber = Request.Params.Get("RoutingSheetNumber")
            End If

            pSql = "SELECT TransDeliveryLogger.*, ISNULL(MobizLibraryOrganizationFOWST.Name, '') as DeliveryTruckFowstName " & vbCrLf &
                    " , ISNULL(MobizLibraryOrganization.CompanyName, '') as Delivery3PlCompanyName  " & vbCrLf &
                    " FROM TransDeliveryLogger " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransDeliveryLogger.DeliveryTruckFowstID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransDeliveryLogger.Delivery3PlCompanyID " & vbCrLf &
                    " WHERE RoutingSheetNumber = " & ReadyString(pRoutingSheetNumber)
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count = 0 Then
                'naa pa sa Invoice table ang details sa Rounting Sheet number, old program ni sa Delivery Logger by Sir Arvin
                pSql = " SELECT TOP 100 PERCENT TransInvoice.ID AS [ID] " & vbCrLf &
                        " , TransInvoice.CustomerID AS [CustomerID] " & vbCrLf &
                        " , TransInvoice.InvoiceNumber AS [InvoiceNumber] " & vbCrLf &
                        " , ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0) AS [FOWSTNo] " & vbCrLf &
                        " , RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS [InvoiceNum] " & vbCrLf &
                        " , (CASE MobizLibraryOrganization.IsOrganization WHEN 1 THEN  MobizLibraryOrganization.CompanyName ELSE ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [CompanyName]" & vbCrLf &
                        " , TransInvoice.InvoiceDate AS [InvoiceDate] " & vbCrLf &
                        " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS [MIN] " & vbCrLf &
                        " , TransInvoice.DeliveryMethod AS [DeliveryMethod] " & vbCrLf &
                        " , TransInvoice.TransType AS [TransType] " & vbCrLf &
                        " , ISNULL(tblDelivery.StreetNumber, '') + ',' + ISNULL(tblDelivery.Street, '') + ',' + ISNULL(tblDeliveryCity.Name, '') + ',' + ISNULL(tblDeliveryCountry.Name, '') AS [DeliveryAddress] " & vbCrLf &
                        " , ISNULL(tblDelivery.StreetNumber, '') + ',' + ISNULL(tblDelivery.Street, '') AS [DeliveryStreet] " & vbCrLf &
                        " , ISNULL(tblDeliveryCity.Name, '') + ',' + ISNULL(tblDeliveryCountry.Name, '') AS [DeliveryCity] " & vbCrLf &
                        " , tblDeliveryCity.ID AS [DeliveryCityID] " & vbCrLf &
                        " , TransInvoice.DeliveryEncodingDate AS [DeliveryEncodingDate] " & vbCrLf &
                        " , ISNULL(TransInvoice.DeliveryStatus, CASE WHEN ISNULL(TransInvoice.IsDelivered, 0) = 0 THEN 'Undelivered' ELSE 'Delivered' END) AS [DeliveryStatus] " & vbCrLf &
                        " , TransInvoice.IsDelivered AS [IsDelivered] " & vbCrLf &
                        " , ISNULL(LibraryDocumentControl.DocumentName, '') as DocumentName " & vbCrLf &
                        " , ThirdPLCompany.CompanyName as Delivery3PlCompanyName " & vbCrLf &
                        " , ThirdPLCompany.ID as Delivery3PlCompanyID " & vbCrLf &
                        " , TransInvoice.Delivery3PlContactPerson as Delivery3PlContactPerson " & vbCrLf &
                        " , TransInvoice.DeliveryWaybillNumber as DeliveryWaybillNumber " & vbCrLf &
                        " , TransInvoice.DeliveryCustomerRepresentative " & vbCrLf &
                        " , TransInvoice.RoutingSheetNumber " & vbCrLf &
                        " , TransInvoice.DeliveryNotes " & vbCrLf &
                        " , TransInvoice.UserID " & vbCrLf &
                        " , Truck.ID as DeliveryTruckFowstID " & vbCrLf &
                        " , Truck.Name as DeliveryTruckFowstName " & vbCrLf &
                        " , tblItems.ServedAmount " & vbCrLf &
                        " FROM TransInvoice " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID " & vbCrLf &
                        " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                        " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as ThirdPLCompany ON TransInvoice.Delivery3PlCompanyID = ThirdPLCompany.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as Truck ON TransInvoice.DeliveryTruckFowstID = Truck.ID " & vbCrLf &
                        " OUTER APPLY (" & vbCrLf &
                        " 	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                        "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                        "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                        "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                        " 	FROM TransInvoiceItems" & vbCrLf &
                        "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                        "   AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                        "   OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                        "   AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                        " ) tblItems    " & vbCrLf &
                        " WHERE 1=1 AND TransInvoice.IsCancelled = 0 " & vbCrLf &
                        " AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf &
                        " AND EXISTS(SELECT TransInvoiceItems.ID FROM TransInvoiceItems WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID) " & vbCrLf &
                        " AND TransInvoice.ParentOrderID IS NULL " & vbCrLf &
                        " AND TransInvoice.RoutingSheetNumber = " & ReadyString(pRoutingSheetNumber) & vbCrLf &
                        " ORDER BY TransInvoice.LastUpdated DESC, TransInvoice.InvoiceDate DESC " & vbCrLf
                pDT = Common.WebRequest.ProData.Execute(pSql)
                pOut.Add("Invoices", pDT)
                Dim headerTable As New DataTable
                Dim pDTRow As DataRow

                headerTable.Columns.Add("ID")
                headerTable.Columns.Add("RoutingSheetNumber")
                headerTable.Columns.Add("DeliveryEncodingDate")
                headerTable.Columns.Add("DeliveryMethod")
                headerTable.Columns.Add("DeliveryNotes")
                headerTable.Columns.Add("DeliveryTruckFowstID")
                headerTable.Columns.Add("DeliveryTruckFowstName")
                headerTable.Columns.Add("Delivery3PlCompanyName")
                headerTable.Columns.Add("Delivery3PlCompanyID")
                headerTable.Columns.Add("DeliveryWaybillNumber")
                headerTable.Columns.Add("DeliveryCustomerRepresentative")
                headerTable.Columns.Add("UserID")
                headerTable.Columns.Add("IsDraft")

                If pDT.Rows.Count > 0 Then
                    pDTRow = headerTable.NewRow

                    pDTRow(0) = "" 'NULL since wala sya sa header table
                    pDTRow(1) = pDT.Rows(0)("RoutingSheetNumber")
                    pDTRow(2) = pDT.Rows(0)("DeliveryEncodingDate")
                    pDTRow(3) = pDT.Rows(0)("DeliveryMethod")
                    pDTRow(4) = pDT.Rows(0)("DeliveryNotes")
                    pDTRow(5) = pDT.Rows(0)("DeliveryTruckFowstID")
                    pDTRow(6) = pDT.Rows(0)("DeliveryTruckFowstName")
                    pDTRow(7) = pDT.Rows(0)("Delivery3PlCompanyName")
                    pDTRow(8) = pDT.Rows(0)("Delivery3PlCompanyID")
                    pDTRow(9) = pDT.Rows(0)("DeliveryWaybillNumber")
                    pDTRow(10) = pDT.Rows(0)("DeliveryCustomerRepresentative")
                    pDTRow(11) = pDT.Rows(0)("UserID")
                    pDTRow(12) = 1

                    headerTable.Rows.Add(pDTRow)

                    pOut.Add("Header", headerTable)
                End If
            Else
                'new program na ni by Arnie 3-17-2023, Header table ang gidagdag
                pOut.Add("Header", pDT)

                pSql = " SELECT TOP 100 PERCENT TransInvoice.ID AS [ID] " & vbCrLf &
                        " , TransInvoice.CustomerID AS [CustomerID] " & vbCrLf &
                        " , TransInvoice.InvoiceNumber AS [InvoiceNumber] " & vbCrLf &
                        " , ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0) AS [FOWSTNo] " & vbCrLf &
                        " , RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS [InvoiceNum] " & vbCrLf &
                        " , (CASE MobizLibraryOrganization.IsOrganization WHEN 1 THEN  MobizLibraryOrganization.CompanyName ELSE ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [CompanyName]" & vbCrLf &
                        " , TransInvoice.InvoiceDate AS [InvoiceDate] " & vbCrLf &
                        " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS [MIN] " & vbCrLf &
                        " , TransInvoice.DeliveryMethod AS [DeliveryMethod] " & vbCrLf &
                        " , TransInvoice.TransType AS [TransType] " & vbCrLf &
                        " , ISNULL(tblDelivery.StreetNumber, '') + ',' + ISNULL(tblDelivery.Street, '') + ',' + ISNULL(tblDeliveryCity.Name, '') + ',' + ISNULL(tblDeliveryCountry.Name, '') AS [DeliveryAddress] " & vbCrLf &
                        " , ISNULL(tblDelivery.StreetNumber, '') + ',' + ISNULL(tblDelivery.Street, '') AS [DeliveryStreet] " & vbCrLf &
                        " , ISNULL(tblDeliveryCity.Name, '') + ',' + ISNULL(tblDeliveryCountry.Name, '') AS [DeliveryCity] " & vbCrLf &
                        " , tblDeliveryCity.ID AS [DeliveryCityID] " & vbCrLf &
                        " , TransDeliveryLoggerInvoices.DeliveryEncodingDate AS [DeliveryEncodingDate] " & vbCrLf &
                        " , TransDeliveryLoggerInvoices.Status AS [DeliveryStatus] " & vbCrLf &
                        " , TransInvoice.IsDelivered AS [IsDelivered] " & vbCrLf &
                        " , ISNULL(LibraryDocumentControl.DocumentName, '') as DocumentName " & vbCrLf &
                        " , tblItems.ServedAmount " & vbCrLf &
                        " FROM TransInvoice " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID " & vbCrLf &
                        " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                        " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID " & vbCrLf &
                        " INNER JOIN TransDeliveryLoggerInvoices ON TransDeliveryLoggerInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                        " OUTER APPLY (" & vbCrLf &
                        " 	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                        "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                        "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                        "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                        " 	FROM TransInvoiceItems" & vbCrLf &
                        "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                        "   AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                        "   OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                        "   AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                        " ) tblItems    " & vbCrLf &
                        " WHERE 1=1 AND TransInvoice.IsCancelled = 0 " & vbCrLf &
                        " AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf &
                        " AND EXISTS(SELECT TransInvoiceItems.ID FROM TransInvoiceItems WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID) " & vbCrLf &
                        " AND TransInvoice.ParentOrderID IS NULL " & vbCrLf &
                        " AND TransDeliveryLoggerInvoices.DeliveryLoggerID = " & ReadyString(pDT.Rows(0)("ID")) & vbCrLf &
                        " ORDER BY TransInvoice.LastUpdated DESC, TransInvoice.InvoiceDate DESC " & vbCrLf
                pDT = Common.WebRequest.ProData.Execute(pSql)
                pOut.Add("Invoices", pDT)
            End If

            For Each pInv In pDT.Rows
                If pIDs <> "" Then
                    pIDs &= ", "
                End If
                pIDs &= ReadyString(pInv("ID").ToString())
            Next

            If Not String.IsNullOrWhiteSpace(pIDs) Then
                pOut.Add("Checker", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, pIDs, "checker"))))
                pOut.Add("Driver", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, pIDs, "driver"))))
                pOut.Add("Helper", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, pIDs, "helper"))))
            End If

            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Result", ex.Message)
        End Try
    End Function

    <HttpPost>
    Public Function SetDeliver() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            'If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Modify Finalized Invoice", False) = False Then
            '    Return JsonDictionary.Serialize("Error", "You need Modify Finalized Invoice security access.")
            'End If

            Dim pInput As TransInvoice.SetDeliverInput = JsonConvert.DeserializeObject(Of TransInvoice.SetDeliverInput)(Common.WebRequest.InputStreamText)
            If pInput.InvoiceIDs Is Nothing Then
                Return JsonDictionary.Serialize("Error", "IDs is required")

            End If

            If CBool2(WebRequest.Instance.Business.TransInvoice.SaveToRoutingSheetHeader(pInput)) Then
                WebRequest.Instance.Business.TransInvoice.SetDeliver(pInput)
            End If

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost>
    Public Function GetDeliver() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim pDR As DataRow = WebRequest.Instance.Business.TransInvoice.GetDeliver(Common.WebRequest.Data("ID"))
            Dim pJson As New JsonDictionary()
            pJson.Add("data", pDR)
            pJson.Add("Checker", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, ReadyString(Common.WebRequest.Data("ID")), "checker"))))
            pJson.Add("Driver", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, ReadyString(Common.WebRequest.Data("ID")), "driver"))))
            pJson.Add("Helper", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(DeliveryDetails(Common.WebRequest.ProData, ReadyString(Common.WebRequest.Data("ID")), "helper"))))
            Return pJson.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function PrintDelivery()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim details As Object = GetRoutingSheetDetails()
            Dim header As Object = JsonConvert.DeserializeObject(details)("d")("Header")("Rows")(0)
            Dim invoices As Object = JsonConvert.DeserializeObject(details)("d")("Invoices")("Rows")
            Dim checker As Object = JsonConvert.DeserializeObject(details)("d")("Checker")
            Dim driver As Object = JsonConvert.DeserializeObject(details)("d")("Driver")
            Dim helper As Object = JsonConvert.DeserializeObject(details)("d")("Helper")
            Dim routingsheetnumber = header("RoutingSheetNumber").ToString()
            Dim pOrgShort As New OrganizationShort
            Dim file As System.IO.FileInfo
            Dim pFilename = "RouteSheetReport"
            Dim pReport As ProReport
            Dim newDatatable As New DataTable
            Dim newDatatableRow As DataRow

            pReport = New ProReport()
            pReport.AutoFillHeader = False
            pReport.SourceFilename = pFilename

#Region "Header"
            newDatatable.Columns.Add("DeliveryTruckFowstName")
            newDatatable.Columns.Add("Delivery3PlCompanyName")
            newDatatable.Columns.Add("DeliveryCustomerRepresentative")
            newDatatable.Columns.Add("DeliveryEncodingDate")
            newDatatable.Columns.Add("RoutingSheetNumber")
            newDatatable.Columns.Add("Delivery3PlContactPerson")
            newDatatable.Columns.Add("DeliveryWaybillNumber")
            newDatatable.Columns.Add("Checker")
            newDatatable.Columns.Add("Driver")
            newDatatable.Columns.Add("Helper")
            newDatatable.Columns.Add("Status")
            newDatatable.Columns.Add("DeliveryNotes")

            newDatatableRow = newDatatable.NewRow
            newDatatableRow(0) = CStr2(header("DeliveryTruckFowstName"))
            newDatatableRow(1) = CStr2(header("Delivery3PlCompanyName"))
            newDatatableRow(2) = CStr2(header("DeliveryCustomerRepresentative"))
            newDatatableRow(3) = header("DeliveryEncodingDate").ToString()
            newDatatableRow(4) = header("RoutingSheetNumber").ToString()
            newDatatableRow(5) = CStr2(header("Delivery3PlContactPerson"))
            newDatatableRow(6) = CStr2(header("DeliveryWaybillNumber"))

            Dim name As String = ""
            If checker IsNot Nothing Then
                For Each data As Object In checker
                    If name <> "" Then
                        name &= ", "
                    End If
                    name &= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data("PeopleName").ToString().ToLower())
                Next
            End If
            newDatatableRow(7) = name

            name = ""
            If driver IsNot Nothing Then
                For Each data As Object In driver
                    If name <> "" Then
                        name &= ", "
                    End If
                    name &= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data("PeopleName").ToString().ToLower())
                Next
            End If
            newDatatableRow(8) = name

            name = ""
            If helper IsNot Nothing Then
                For Each data As Object In helper
                    If name <> "" Then
                        name &= ", "
                    End If
                    name &= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data("PeopleName").ToString().ToLower())
                Next
            End If
            newDatatableRow(9) = name
            newDatatableRow(10) = IIf(CBool2(header("IsDraft")), "DRAFT", "FINALIZED")
            newDatatableRow(11) = CStr2(header("DeliveryNotes"))
            newDatatable.Rows.Add(newDatatableRow)
            pReport.AddTable("Header", newDatatable)
#End Region

#Region "Invoices"
            newDatatable = New DataTable

            newDatatable.Columns.Add("InvoiceDate")
            newDatatable.Columns.Add("InvoiceNumber")
            newDatatable.Columns.Add("CustomerName")
            newDatatable.Columns.Add("DeliveryStatus")

            For Each inv As Object In invoices
                newDatatableRow = newDatatable.NewRow
                Dim invnum As String = ""
                If String.IsNullOrWhiteSpace(CStr2(inv("DocumentName").ToString())) Then
                    invnum = "SI"
                Else
                    invnum = inv("DocumentName").ToString()
                End If

                invnum &= "-" & inv("MIN").ToString() & "-" & inv("InvoiceNumber").ToString()

                newDatatableRow(0) = Format(CDate(inv("InvoiceDate").ToString()), "MM-dd-yyyy")
                newDatatableRow(1) = invnum
                newDatatableRow(2) = inv("CompanyName").ToString()
                newDatatableRow(3) = inv("DeliveryStatus").ToString()
                newDatatable.Rows.Add(newDatatableRow)
            Next
            pReport.AddTable("Invoices", newDatatable)
#End Region

            Dim pDtHeader = pOrgShort.GetProspectOrgDetails(WebRequest.Instance.OrgID)
            Dim pFowstID = CStr2(WebRequest.Instance.QueryString("FowstID"))
            If IsGuid(pFowstID) Then
                Dim pDrFowst = WebRequest.Instance.Business.LibraryOrganizationFOWST.ByID(pFowstID)
                If pDrFowst IsNot Nothing Then
                    For Each pDrHeader As DataRow In pDtHeader.Rows
                        pDrHeader("Address") = CStr2(pDrFowst("Name")) & " - " & CStr2(pDrFowst("Address"))
                    Next
                End If
            End If
            pReport.AddTable("CompanyInfo", pDtHeader)
            ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"

            pFilename = Common.ServerPath & "FileTemplates\RS-" & routingsheetnumber & ".pdf"

            pReport.ExportXmlFile()
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

            file = New System.IO.FileInfo(pFilename)

            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=RS-" & routingsheetnumber & ".pdf")
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()
        Catch ex As Exception
            AppendToDebug(ex.Message)
        End Try
    End Function

    Public Function DeliveryDetails(vProdata As ProData, vInvoiceID As String, vDesignation As String) As DataTable

        Dim pSql As String = "SELECT * FROM (SELECT DISTINCT ppol.ID, ppol.FirstName + ' ' + ppol.Surname as PeopleName " & vbCrLf &
                            " FROM TransInvoiceDeliveryDetails as deldev" & vbCrLf &
                            " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = deldev.PeopleID " & vbCrLf &
                            " WHERE InvoiceID IN (" & vInvoiceID & ") " & vbCrLf &
                            " AND Designation = " & ReadyString(vDesignation) & vbCrLf &
                            " AND ISNULL(deldev.IsActive, 1) = 1" & vbCrLf &
                            " UNION ALL " & vbCrLf &
                            " SELECT DISTINCT ppol.ID, ppol.FirstName + ' ' + ppol.Surname as PeopleName " & vbCrLf &
                            " FROM TransInvoice as inv " & vbCrLf &
                            " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = "

        If vDesignation = "checker" Then
            pSql &= " inv.DeliveryCheckedByID"
        ElseIf vDesignation = "helper" Then
            pSql &= " inv.DeliveryHelperID"
        ElseIf vDesignation = "driver" Then
            pSql &= " inv.DeliveryDriverID"
        End If

        pSql &= " WHERE inv.ID IN (" & vInvoiceID & ") " & vbCrLf &
                ") as t ORDER BY PeopleName ASC "

        Return vProdata.Execute(pSql)
    End Function

    Public Class CreateMultiplePurchaseOrdersJson
        Public Items As List(Of Item)
        Public FowstID As String
        Public Expanded As String ' boolean

        Public Function ExpandedProper() As Boolean

            Return CBool2(Expanded)

        End Function

        Public Class Item
            Public ID As String
            Public BackOrderCustomerID As String ' possible only in expanded view
            Public ProductID As String
            Public VendorID As String
            Public Qty As String
            Public BackorderQty As String
            Public Price As String
            Public UnitMeasure As String
            Public UnitQty As String
            Public RequestedByID As String
            Public Remarks As String
        End Class
    End Class

    ' commented 2021_08_31__14_55_32_6874
    '<HttpPost>
    'Public Function GetItemRequisitions() As String

    '    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
    '    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

    '    Dim pDeactivated = CBool2(WebRequest.Instance.Data("Deactivated"))
    '    Dim pFowstID = WebRequest.Instance.Data("FowstID")
    '    Dim pDateRequested = ParseDate(WebRequest.Instance.Data("DateRequested"))
    '    Dim pDT = WebRequest.Instance.Business.TransInvoice.GetItemRequisition(pFowstID, pDateRequested, pDeactivated)

    '    Return JsonDictionary.Serialize(pDT)

    'End Function

    <HttpPost>
    Public Function SaveItemRequisitions() As String

        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pInput As CreateMultiplePurchaseOrdersJson = JsonConvert.DeserializeObject(Of CreateMultiplePurchaseOrdersJson)(Common.WebRequest.InputStreamText)
        WebRequest.Instance.ProData.AddColumn("TransItemRequisitions", "FowstID", "uniqueidentifier")

        WebRequest.Instance.ProData.BeginTransaction()
        For Each pItem In pInput.Items
            Dim pIU As New InsertUpdate("TransItemRequisitions")
            If String.IsNullOrWhiteSpace(pItem.ID) Then
                pIU("ID") = "NEWID()"
            End If
            pIU("ProductID") = ReadyString(pItem.ProductID)
            If String.IsNullOrWhiteSpace(pItem.VendorID) = False Then
                pIU("VendorID") = ReadyString(pItem.VendorID)
            End If
            If String.IsNullOrWhiteSpace(pItem.RequestedByID) = False Then
                pIU("RequestedByID") = ReadyString(pItem.RequestedByID)
            End If
            pIU("RequestedQty") = CDec2(pItem.Qty)
            pIU("RequestedQtyTotal") = CDec2(pItem.Qty) + CDec2(pItem.BackorderQty)
            pIU("UnitMeasure") = ReadyString(pItem.UnitMeasure)
            If CDec2(pItem.UnitQty) <= 0 Then
                pIU("UnitQty") = 1
            Else
                pIU("UnitQty") = CDec2(pItem.UnitQty)
            End If
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("Remarks") = ReadyString(pItem.Remarks)
            pIU("FowstID") = ReadyString(pInput.FowstID)
            pIU("DateRequested") = "GETDATE()"
            If String.IsNullOrWhiteSpace(pItem.ID) Then
                pSql = pIU.GetInsert
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(pItem.ID) & vbCrLf & _
                    " AND CreatedPurchaseOrderID IS NULL"
                )
            End If
            If WebRequest.Instance.ProData.ExecuteSQL(pSql) = 0 Then
                pIU("ID") = "NEWID()"
                pSql = pIU.GetInsert()
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
            End If
        Next
        WebRequest.Instance.ProData.CommitTransaction()

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function DeactivateItemRequisitions() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pIU = New InsertUpdate("TransItemRequisitions")
        pIU("IsDeactivated") = 1
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("ID")))
        Dim pInt = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is for debugging purposes only

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function CreateMultiplePurchaseOrders() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pVendorID As String
        Dim pError As String
        Dim pInput As CreateMultiplePurchaseOrdersJson = JsonConvert.DeserializeObject(Of CreateMultiplePurchaseOrdersJson)(Common.WebRequest.InputStreamText)
        Dim pDtBackOrders As DataTable
        Dim pDrBackOrder As DataRow
        Dim pSql As String
        Dim pVendorIDs As New List(Of String)
        For Each pItem In pInput.Items
            pVendorID = ReadyGuid(pItem.VendorID)
            If pVendorIDs.Contains(pVendorID) = False Then
                pVendorIDs.Add(pVendorID)
            End If
        Next

        WebRequest.Instance.ProData.BeginTransaction()
        For Each pVendorID In pVendorIDs
            Dim pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator()
            pCreator.CustomerID = pVendorID
            pCreator.InvoiceNumber = 0
            pCreator.FowstID = pInput.FowstID
            pCreator.TransType = "PurchaseOrder"
            pCreator.UserID = WebSession.Instance.PeopleUserID
            pCreator.BillingAddressID = Guid.Empty.ToString()
            pCreator.DeliveryAddressID = Guid.Empty.ToString()
            pCreator.DeviceID = WebRequest.Instance.DeviceID
            If String.IsNullOrWhiteSpace(pCreator.DeviceID) Then
                Return JsonDictionary.Serialize("Error", "Your device UUID is " & Common.WebRequest.Data("UUID") & ". Please login then try again. Or login using Mobiz Controller. CreateMultiplePurchaseOrders: DeviceID must not be blank. ref: vbQXBHDJDCZB!")
            End If

            pCreator.Stocks = New DataTable
            With pCreator.Stocks.Columns
                .Add("ID")
                .Add("InvoiceID")
                .Add("StockID")
                .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
                .Add("Quantity", GetType(Decimal))
                .Add("ActualQuantity", GetType(Decimal))
                .Add("UnitMeasure")
                .Add("TermDiscount", GetType(Decimal))
                .Add("DiscountIfPaidWithIn", GetType(Decimal))
                .Add("NetDue")
                .Add("UnitPrice", GetType(Decimal))
                .Add("OriginalUnitPrice", GetType(Decimal))
                .Add("Subtotal", GetType(Decimal))
                .Add("Cost", GetType(Decimal))
                .Add("PriceLevelID")
                .Add("Dimension")
                .Add("IsReplacement", GetType(Boolean))
                .Add("IsHasReplacement", GetType(Boolean))
                .Add("IsHasBackOrder", GetType(Boolean))
                .Add("RefStockID")
                .Add("UnitGP", GetType(Decimal))
                .Add("UnitGPM", GetType(Decimal))
                .Add("LineGP", GetType(Decimal))
                .Add("LineGPM", GetType(Decimal))
                .Add("LineCost", GetType(Decimal))
                .Add("LineNumber", GetType(Integer))
                .Add("TaxPercent", GetType(Decimal))
                .Add("TaxAmount", GetType(Decimal))
                .Add("TaxSales", GetType(Decimal))
                '.Add("SalesRep", GetType(Object))
                .Add("DiscountAmount", GetType(Decimal))
                .Add("DiscountSales", GetType(Decimal))
                .Add("RequestedUnitPrice", GetType(Decimal))
                .Add("VATExemptSale", GetType(Decimal))
                .Add("ZeroRatedSale", GetType(Decimal))
                .Add("NonVATSale", GetType(Decimal))
                .Add("VATableSale", GetType(Decimal))
                .Add("ModifierGroupID")
            End With

            For Each pItem In pInput.Items
                If pVendorID <> ReadyGuid(pItem.VendorID) Then
                    Continue For
                End If
                Dim pDR = pCreator.Stocks.NewRow()
                pDR("StockID") = pItem.ProductID

                pDR("OrderedQuantity") = CDec2(pItem.Qty)
                pDR("Quantity") = 0
                pDR("ActualQuantity") = pItem.UnitQty
                pDR("UnitMeasure") = pItem.UnitMeasure
                pDR("UnitPrice") = CDec2(pItem.Price)
                pDR("Subtotal") = CDec2(pItem.Price) * CDec2(pItem.Qty)
                pDR("Cost") = 0 ' todo
                pDR("PriceLevelID") = WebRequest.Instance.Business.SystemDefaults.GetLandedCostID()
                pCreator.Stocks.Rows.Add(pDR)
            Next pItem

            If pCreator.Save(True, pError) = False Then
                WebRequest.Instance.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", pError)
            End If

            For Each pItem In pInput.Items
                If pVendorID <> ReadyGuid(pItem.VendorID) Then
                    Continue For
                End If

                If String.IsNullOrWhiteSpace(pItem.ID) = False Then
                    Dim pIuItemRequisitions = New InsertUpdate("TransItemRequisitions")
                    pIuItemRequisitions("CreatedPurchaseOrderID") = ReadyString(pCreator.ID)

                    If pInput.ExpandedProper Then
                        If String.IsNullOrWhiteSpace(pItem.BackOrderCustomerID) Then
                            pDtBackOrders = WebRequest.Instance.Business.TransInvoice.ItemRequisitionBackOrders(pItem.ID)
                            If pDtBackOrders.Rows.Count > 0 Then
                                pSql = "SELECT * FROM TransItemRequisitions WHERE ID = " & ReadyString(pItem.ID)
                                Dim pDrItemRequisition = WebRequest.Instance.ProData.ExecuteRow(pSql)
                                Dim pIU = Business.DataRowToInsertUpdate(pDrItemRequisition)
                                pIU.TableName = "TransItemRequisitions"
                                Dim pID = Guid.NewGuid().ToString()
                                pIU("ID") = ReadyString(pID)
                                pIU("RequestedQty") = 0
                                pIU("RequestedQtyTotal") = CDec2(pDrItemRequisition("BackorderQty"))
                                pSql = pIU.GetInsert()
                                WebRequest.Instance.ProData.ExecuteSQL(pSql)

                                For Each pDrBackOrder In pDtBackOrders.Rows
                                    pIU = New InsertUpdate("TransItemRequisitionBackOrder")
                                    pIU("ItemRequisitionID") = ReadyString(pID)
                                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrBackOrder("ItemRequisitionBackOrderID")))
                                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                                Next
                            End If
                        Else ' ELSE If String.IsNullOrWhiteSpace(pItem.BackOrderCustomerID) Then
                            pDtBackOrders = WebRequest.Instance.Business.TransInvoice.ItemRequisitionBackOrders(pItem.ID)
                            If pDtBackOrders.Rows.Count > 1 Then
                                ' count the unique customers not equal to the current item's customer
                                Dim pDtCustomers = pDtBackOrders.DefaultView.ToTable(True, "BackOrderCustomerID")
                                If pDtCustomers.Rows.Count > 1 Then
                                    pSql = "SELECT * FROM TransItemRequisitions WHERE ID = " & ReadyString(pItem.ID)
                                    Dim pDrItemRequisition = WebRequest.Instance.ProData.ExecuteRow(pSql)
                                    Dim pIU = Business.DataRowToInsertUpdate(pDrItemRequisition)
                                    pIU.TableName = "TransItemRequisitions"
                                    Dim pID = Guid.NewGuid().ToString()
                                    pIU("ID") = ReadyString(pID)
                                    pIU("RequestedQty") = 0
                                    pIU("RequestedQtyTotal") = CDec2(pDrItemRequisition("BackorderQty"))
                                    pSql = pIU.GetInsert()
                                    WebRequest.Instance.ProData.ExecuteSQL(pSql)

                                    Dim pBackOrderQty As Decimal = 0
                                    Dim pBackOrderQtyOthers As Decimal = 0
                                    For Each pDrBackOrder In pDtBackOrders.Rows
                                        If ReadyGuid(pDrBackOrder("BackOrderCustomerID")) = ReadyGuid(pItem.BackOrderCustomerID) Then
                                            pBackOrderQty += CDec2(pDrBackOrder("BackOrderQty"))
                                            Continue For
                                        End If
                                        pBackOrderQtyOthers += CDec2(pDrBackOrder("BackOrderQty"))
                                        pIU = New InsertUpdate("TransItemRequisitionBackOrder")
                                        pIU("ItemRequisitionID") = ReadyString(pID)
                                        pSql = pIU.GetUpdate("ID = " & ReadyString(pDrBackOrder("ItemRequisitionBackOrderID")))
                                        WebRequest.Instance.ProData.ExecuteSQL(pSql)
                                    Next

                                    pIU = New InsertUpdate("TransItemRequisitions")
                                    pIU("BackorderQty") = pBackOrderQtyOthers
                                    pIU("RequestedQtyTotal") = pBackOrderQtyOthers
                                    pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
                                    WebRequest.Instance.ProData.ExecuteSQL(pSql)

                                    pIU = New InsertUpdate("TransItemRequisitions")
                                    pIU("BackorderQty") = pBackOrderQty
                                    pIU("RequestedQtyTotal") = pBackOrderQty
                                    pSql = pIU.GetUpdate("ID = " & ReadyString(pItem.ID))
                                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                                End If
                            End If
                        End If ' END If String.IsNullOrWhiteSpace(pItem.BackOrderCustomerID) Then
                    End If

                    pSql = pIuItemRequisitions.GetUpdate("ID = " & ReadyString(pItem.ID))
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                End If
            Next pItem
        Next
        WebRequest.Instance.ProData.CommitTransaction()

        Return JsonDictionary.ToString("Result", "ok")

    End Function

    <HttpPost>
    Public Function EmailReceiptHtml() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pInvoice = Common.WebRequest.Data("InvoiceID")
        Dim pSendEmail = CBool2(Common.WebRequest.Data("SendEmail"))
        Dim pEmailAddresses = Common.WebRequest.Data("EmailAddresses")
        Dim pOut = WebRequest.Instance.Business.TransInvoice.EmailReceiptHtml(pInvoice, pSendEmail, pEmailAddresses)

        Return JsonDictionary.Serialize("Result", pOut)

    End Function

    <HttpPost>
    Public Function GetZReadDate() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pOut As New JsonDictionary()
        Dim pZReadDate = Date.Now.Date
        Dim pDeviceID = Common.WebRequest.Data("DeviceID")
        Dim pFowstID = Common.WebRequest.Data("FowstID")
        If WebRequest.Instance.Business.SystemDefaults.EnforceZRead Then
            Dim pDR = WebRequest.Instance.Business.LibraryDeviceFowst.ByDeviceFowst(pDeviceID, pFowstID)
            If pDR IsNot Nothing Then
                If pDR("ZReadDate") Is DBNull.Value Then
                    pZReadDate = pDR("ZReadDate")
                End If
            End If
        End If
        pOut.Add("ZReadDate", pZReadDate)
        pOut.Add("ServerTime", Now)

        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function CopyTransaction() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pOut As New JsonDictionary()
        Dim pZReadDate = Date.Now.Date
        'Dim pDeviceID = Common.WebRequest.Data("DeviceID")
        'Dim pFowstID = Common.WebRequest.Data("FowstID")
        Dim pSourceID = Common.WebRequest.Data("ID")
        Dim pTransType = Common.WebRequest.Data("TransType")
        Dim pCustomerID = Common.WebRequest.Data("CustomerID")
        Dim pDestinationID = WebRequest.Instance.Business.TransInvoice.CopyTransaction(pSourceID, pTransType, pCustomerID)

        Return JsonDictionary.Serialize("Result", pDestinationID)

    End Function

    Public Function CopyToItemRequisition() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pSourceID = Common.WebRequest.Data("ID")
        Dim pDR = WebRequest.Instance.Business.TransInvoice.ById(pSourceID)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Source transaction does not exist. vb53JLMTNZXC!")
        End If
        Dim pFowstID = CStr2(pDR("FowstID"))
        WebRequest.Instance.Business.TransInvoice.CopyToItemRequisition(
            vID:=pSourceID,
            vQuantityField:="OrderedQuantity",
            vUserID:=WebSession.Instance.PeopleUserID,
            vFowstID:=pFowstID)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function ItemRequisitionDetail() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pOut As New JsonDictionary()
        Dim pZReadDate = Date.Now.Date
        'Dim pDeviceID = Common.WebRequest.Data("DeviceID")
        'Dim pFowstID = Common.WebRequest.Data("FowstID")
        Dim pSourceID = Common.WebRequest.Data("ID")
        Dim pDS = WebRequest.Instance.Business.TransInvoice.ItemRequisitionDetail(Common.WebRequest.Data("ItemRequisitionID"), Common.WebRequest.Data("ProductID"))

        pOut.Add("BackOrders", pDS.Tables("BackOrders"))
        pOut.Add("Product", pDS.Tables("Product"))
        pOut.Add("ItemRequisition", pDS.Tables("ItemRequisition"))
        pOut.Add("Units", pDS.Tables("Units"))

        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function SaveItemRequisitionDetail() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pInput = JsonConvert.DeserializeObject(Of TransInvoice.SaveItemRequisitionInput)(Common.WebRequest.InputStreamText)
        pInput.UserID = WebSession.Instance.PeopleUserID
        WebRequest.Instance.Business.TransInvoice.SaveItemRequisitionDetail(pInput)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function GetItemRequisitionSettings() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pJson = New JsonDictionary()
        pJson.Add("DateCoverage", WebRequest.Instance.Business.SystemDefaults.ItemRequisitionDateCoverage())

        Return pJson.Serialize()

    End Function

    <HttpPost>
    Public Function SaveItemRequisitionSettings() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        WebRequest.Instance.Business.SystemDefaults.ItemRequisitionDateCoverage = Common.WebRequest.Data("DateCoverage")

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    Class CreateSalesOrderInput
        Public ID As String ' TransInvoice.ID. optional. if not null, will
        Public Products As List(Of Product)
        Public CustomerID As String
        Public FowstID As String

        Public Class Product
            'Public ID As String
            Public ProductID As String
            Public Qty As String
            'Public Price As String
            'Public UnitMeasure As String
            'Public UnitQty As String
            'Public Remarks As String
        End Class
    End Class

    <HttpPost>
    Public Function CreateSalesOrder() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pError As String
        Dim pInput = JsonConvert.DeserializeObject(Of CreateSalesOrderInput)(Common.WebRequest.InputStreamText)

        WebRequest.Instance.ProData.BeginTransaction()
        Dim pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator()
        If String.IsNullOrWhiteSpace(pInput.ID) = False Then
            pCreator.ID = pInput.ID
        End If
        pCreator.CustomerID = pInput.CustomerID
        pCreator.InvoiceNumber = 0
        pCreator.FowstID = pInput.FowstID
        pCreator.TransType = "SalesOrder"
        pCreator.UserID = WebSession.Instance.PeopleUserID
        pCreator.BillingAddressID = Guid.Empty.ToString()
        pCreator.DeliveryAddressID = Guid.Empty.ToString()
        pCreator.DeviceID = WebRequest.Instance.DeviceID

        pCreator.Stocks = New DataTable
        With pCreator.Stocks.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Decimal))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
        End With

        For Each pItem In pInput.Products
            Dim pDR = pCreator.Stocks.NewRow()
            pDR("StockID") = pItem.ProductID

            Dim pDtPrice = WebRequest.Instance.Business.LibraryStock.PriceTable(WebRequest.Instance.OrgID, pItem.ProductID, pInput.CustomerID, True, True, , , True)
            Dim pDrPrice As DataRow
            If pDtPrice.Rows.Count > 0 Then
                pDrPrice = pDtPrice.Rows(0)
            End If
            pDR("OrderedQuantity") = CDec2(pItem.Qty)
            pDR("Quantity") = CDec2(pItem.Qty)
            pDR("ActualQuantity") = CDec2(pDrPrice("Quantity"))
            pDR("UnitMeasure") = CStr2(pDrPrice("UnitMeasure"))
            pDR("UnitPrice") = CDec2(pDrPrice("DiscountedPrice"))
            pDR("Subtotal") = CDec2(pDrPrice("DiscountedPrice")) * CDec2(pItem.Qty)
            pDR("Cost") = 0 ' todo
            pDR("PriceLevelID") = CStr2(pDrPrice("PriceLevelID"))
            pCreator.Stocks.Rows.Add(pDR)
        Next pItem

        If pCreator.Save(String.IsNullOrWhiteSpace(pInput.ID), pError) = False Then
            WebRequest.Instance.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", pError)
        End If

        WebRequest.Instance.ProData.CommitTransaction()

        Return JsonDictionary.ToString("Result", pCreator.ID)

    End Function

    <HttpPost>
    Public Function SaveServedDate() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pID = WebRequest.Instance.Data("ID") ' TransInvoiceItems.ID
        If IsGuid(pID) = False Then
            Return JsonDictionary.Serialize("Error", "ID must be a GUID. ref: vbMLSMXV7XA2!")
        End If
        Dim pStr = WebRequest.Instance.Data("ServedDate") ' TransInvoiceItems.ID
        If String.IsNullOrWhiteSpace(pStr) Then
            Return JsonDictionary.Serialize("Error", "Served Date must not be blank. ref: vb6R6G38BHBL!")
        End If
        Dim pServedDate = ParseDate(pStr)
        If pServedDate Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Served Date must be a valid date. You entered " & pStr & ". ref: vbERY47T2TED!")
        End If
        Dim pIU = New InsertUpdate("TransInvoiceItems")
        pIU("ServedDate") = ReadyString(Format(pServedDate.Value, cGlobal.FormatDateTimeSql))
        pStr = pIU.GetUpdate("ID = " & ReadyString(pID))
        pStr = WebRequest.Instance.ProData.ExecuteSQL(pStr) ' output is used for debugging only

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function SaveLineItemCancel() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pID = WebRequest.Instance.Data("ID") ' TransInvoiceItems.ID
        If IsGuid(pID) = False Then
            Return JsonDictionary.Serialize("Error", "ID must be a GUID. ref: vbP6YFU4TN9H!")
        End If
        Dim pStr = WebRequest.Instance.Data("Cancel") ' TransInvoiceItems.ID
        If String.IsNullOrWhiteSpace(pStr) Then
            Return JsonDictionary.Serialize("Error", "Cancel Date must not be blank. ref: vbUCZA5LQD3Q!")
        End If
        Dim pCancel = CBool2(pStr)
        Dim pIU = New InsertUpdate("TransInvoiceItems")
        If pCancel Then
            pIU("CancelledByID") = ReadyString(WebSession.Instance.PeopleUserID)
        Else
            pIU("CancelledByID") = "NULL"
        End If
        pStr = pIU.GetUpdate("ID = " & ReadyString(pID))
        pStr = WebRequest.Instance.ProData.ExecuteSQL(pStr) ' output is used for debugging only

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function SaveLineItemShippingRef() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pID = WebRequest.Instance.Data("ID") ' TransInvoiceItems.ID
        If IsGuid(pID) = False Then
            Return JsonDictionary.Serialize("Error", "ID must be a GUID. ref: vbCY9D57GAA4!")
        End If
        Dim pStr = WebRequest.Instance.Data("ShippingRef") ' TransInvoiceItems.ShippingRef
        pStr = Truncate(pStr, 500)
        Dim pIU = New InsertUpdate("TransInvoiceItems")
        pIU("ShippingRef") = ReadyString(pStr)
        pStr = pIU.GetUpdate("ID = " & ReadyString(pID))
        pStr = WebRequest.Instance.ProData.ExecuteSQL(pStr) ' output is used for debugging only

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function ExportOrgToXmlZip() As String

        ' If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOrgIdToExport = WebRequest.Instance.Data("OrgIdToExport")
        WebRequest.Instance.Business.LibraryOrganization.ExportToXmlZip(pOrgIdToExport)

        Return JsonDictionary.Serialize("Results", "ok")

    End Function

    Public Function DownloadOrgXmlZip() As String

        Dim pFolder As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pServer = WebRequest.Instance.Data("Server")
        Dim pOrgIdToExport = WebRequest.Instance.Data("OrgIdToExport")
        pOrgIdToExport = ReadyGuid(pOrgIdToExport)
        Dim pUrl = AppendIfNeeded(pServer, "/") & "SalesMenu/GetOrgXmlZip?OrgIdToExport=" & pOrgIdToExport

        pFolder = Common.ServerPath & "temp\DownloadedOrgXmlZip"
        Dim pFilename = IO.Path.Combine(pFolder, pOrgIdToExport & ".zip")
        Dim pTempFilename = pFilename & ".temp"
        'pTempFilename = IO.Path.Combine(pFolder, pTempFilename)
        CreateFolder(pTempFilename)
        If IO.File.Exists(pTempFilename) Then
            IO.File.Delete(pTempFilename)
        End If
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If

        Dim pWeb As New Net.WebClient
        pWeb.DownloadFile(pUrl, pTempFilename)
        ProPack.Utility.AddDirectoryUser(pFolder)
        Rename(pTempFilename, pFilename)
        ImportOrgXmlZip(pFilename)

        Return JsonDictionary.Serialize("Results", "ok")

    End Function

    Public Function GetOrgXmlZip() As ActionResult

        Dim pFilename = Common.ServerPath & "temp\ExportedOrganization\ExportedOrganization_" & WebRequest.Instance.QueryString("OrgIdToExport") & ".zip"

        Return File(pFilename, "application/zip", "ExportedOrganization_" & WebRequest.Instance.QueryString("OrgIdToExport") & ".zip")

    End Function

    Public Function ImportOrgXmlZip(vFilename As String) As Integer

        Dim pPrivateDatabaseName As String = WebRequest.Instance.ProData.GetCurrentDatabase()
        Dim pProZip As New ProZip
        pProZip.Open(vFilename)
        Dim pDtFilenames = pProZip.GetFilenames()
        For Each pDrFilename As DataRow In pDtFilenames.Rows
            If CStr2(pDrFilename("Extension")).ToLower() <> "xml" Then
                Continue For
            End If
            Dim pDT = New DataTable
            Dim pFolder = vFilename.Substring(0, vFilename.Length - 4)
            Dim pXmlFilename = IO.Path.Combine(pFolder, pDrFilename("Name"))
            CreateFolder(pXmlFilename)
            pProZip.UnzipFile(pDrFilename("FullName"), pXmlFilename)
            pDT.ReadXml(pXmlFilename)
            Dim pTableName = CStr2(pDrFilename("Name"))
            pTableName = pTableName.Substring(0, pTableName.Length - 4)
            If pTableName.StartsWith("Mobiz") Then
                WebRequest.Instance.ProData.ExecuteSQL("USE Mobiz")
                pTableName = pTableName.Substring(5)
            Else
                WebRequest.Instance.ProData.ExecuteSQL("USE " & EscapeSqlName(pPrivateDatabaseName))
            End If
            BulkInsert(pDT, pTableName)
        Next

    End Function

    Private Sub BulkInsert(vDT As DataTable, vTableName As String)

        Dim pDestinationSchema = WebRequest.Instance.ProData.ColumnsInformation(vTableName)
        Using pBulkCopy As New SqlBulkCopy(WebRequest.Instance.ProData.Connection, SqlBulkCopyOptions.KeepIdentity + SqlBulkCopyOptions.KeepNulls, Nothing)
            'Using pBulkCopy As New SqlBulkCopy(pProData.Connection)
            For Each pSchemaRow As DataRow In pDestinationSchema.Rows
                If CBool2(pSchemaRow("iscomputed")) = False Then
                    pBulkCopy.ColumnMappings.Add(pSchemaRow("name"), pSchemaRow("name"))
                End If
            Next

            pBulkCopy.DestinationTableName = vTableName
            pBulkCopy.BulkCopyTimeout = 0
            pBulkCopy.WriteToServer(vDT)
        End Using

    End Sub

    Function ShowSalesPaymentsInfo()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql = "SELECT siitems.InvoiceID " & vbCrLf &
                ",si.CustomerID ,(SELECT COUNT(DISTINCT ProductID) FROM TransInvoiceItems WHERE InvoiceID=si.ID) AS SkuCount" & vbCrLf &
                ",SUM(ISNULL(siitems.Quantity,0)) as TotalQuantity " & vbCrLf &
                ",(SUM(ISNULL(siitems.VATableSale,SubTotal)) + SUM(ISNULL(siitems.VATExemptSale,0)) + SUM(ISNULL(siitems.ZeroRatedSale,0)) + SUM(ISNULL(siitems.NonVATSale,0))) as TotalAmountDue " & vbCrLf &
                ",(SELECT SUM(ISNULL(SubTotal,0)) FROM TransInvoiceItems WHERE InvoiceID = siitems.InvoiceID AND OriginalUnitPrice = UnitPrice) as TotalAmountDueAllowedForDiscount " & vbCrLf &
                ",SUM(ISNULL(siitems.SubTotal,0)) as TransInvoiceItems " & vbCrLf &
                ",ISNULL(si.RefNum,'') as RefNum " & vbCrLf &
                ",(SELECT TOP 1 ISNULL(TaxPercent, 0) FROM TransInvoiceItems WHERE InvoiceID = si.ID) as TaxPercent " & vbCrLf &
                ",si.DepositAmount " & vbCrLf &
                " FROM TransInvoiceItems siitems " & vbCrLf &
                " INNER JOIN TransInvoice si ON (siitems.InvoiceID = si.ID) " & vbCrLf &
                " WHERE siitems.InvoiceID = " & ReadyString(Common.WebRequest.Data("InvoiceID")) & vbCrLf &
                " GROUP BY si.ID, siitems.InvoiceID " & vbCrLf &
                " ,si.CustomerID " & vbCrLf &
                " ,si.RefNum " & vbCrLf &
                ",si.DepositAmount "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function UpdateInvoiceForDiscount()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim p As New InsertUpdate("TransInvoice")

            p("Vatable") = CDec2(data("Vatable"))
            p("VATExempt") = CDec2(data("VATExempt"))
            p("VAT") = CDec2(data("VAT"))
            p("ForegoneVAT") = CDec2(data("ForegoneVAT"))
            p("SCDiscount") = CDec2(data("SCDiscount"))
            p("TotalNonSCDiscount") = CDec2(data("TotalNonSCDiscount"))
            p("AmountDueBeforeCharges") = CDec2(data("AmountDueBeforeCharges"))
            p("AmountDueNetofCharges") = CDec2(data("AmountDueNetofCharges"))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID").ToString())))

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            AppendToDebug("WebPOS discount: " & ex.Message)
            Return JsonDictionary.Serialize("Result", ex.Message)
        End Try
    End Function

    Function GetInvoiceCharges()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql = " SELECT ISNULL(SUM(ChargeAmount), 0) from TransInvoiceCharges where InvoiceID= " & ReadyString(Common.WebRequest.Data("InvoiceID"))

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.ExecuteScalar(pSql))
    End Function

    Function UpdateInvoiceItemsForDiscount()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim p As New InsertUpdate("TransInvoiceItems")

            p("TaxAmount") = "((SubTotal / (1+(convert(float, TaxPercent)/100)) * (convert(float, (100 - " + CDec2(data("DiscountPercentCeiling")) + ")) /100))  * (1+(convert(float, (TaxPercent))/100))) - TaxSales"
            p("DiscountAmount") = "(SubTotal - (SubTotal * " & CDec2(data("AvePerBill")) & "))"
            p("DiscountSales") = "(SubTotal - (SubTotal * " & CDec2(data("AvePerBill")) & "))"
            p("DiscountTax") = CDec2(0)

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID").ToString())))

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            AppendToDebug("WebPOS discount: " & ex.Message)
            Return JsonDictionary.Serialize("Result", ex.Message)
        End Try
    End Function

    Function getOrgFOWSTPrinterType()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql = "SELECT devfowstpntr.ID, " &
                    " fowstpntr.PrinterIPAddress, " &
                    " fowstpntr.ConnectionType, " &
                    " dp.PrinterType " &
                    " FROM LibraryDeviceFowstPrinter devfowstpntr " &
                    " INNER JOIN LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " &
                    " INNER JOIN LibraryDevicePrinter dp on dp.ID = fowstpntr.PrinterID " &
                    " Where fowstpntr.FOWSTID = '" & Common.WebRequest.Data("FowstID") & "' " &
                    " AND devfowstpntr.DeviceID = '" & WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(Common.WebRequest.Data("UUID")) & "'"

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function PaymentOptions()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut As New JsonDictionary

        pOut.Add("Check", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentCheck"))
        pOut.Add("Debit", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentDebit"))
        pOut.Add("Credit", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentCredit"))
        pOut.Add("SRV", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentSRV"))
        pOut.Add("OtherPayment", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowOtherPayment"))
        pOut.Add("OnCredit", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentOnCredit"))
        pOut.Add("DeliveryInfo", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentDeliveryInfo"))
        pOut.Add("Discounts", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentDiscounts"))
        pOut.Add("Charges", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentCharges"))
        pOut.Add("Rewards", WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("ShowPaymentRewards"))

        Return pOut.Serialize()

    End Function

    Public Function ExportInvoiceToCsvZip() As String

        Try
            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDateFrom = ParseDate(Common.WebRequest.Data("DateFrom"))
            If pDateFrom Is Nothing Then
                Return JsonDictionary.Serialize("Error", "DateFrom must be a valid date.")
            End If

            Dim pDateTo = ParseDate(Common.WebRequest.Data("DateTo"))
            If pDateTo Is Nothing Then
                Return JsonDictionary.Serialize("Error", "DateTo must be a valid date.")
            End If

            Dim pFowstID = Common.WebRequest.Data("FowstID")
            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "FowstID must be a GUID.")
            End If

            Dim pDeviceID = Common.WebRequest.Data("DeviceID")
            If IsGuid(pDeviceID) = False Then
                Return JsonDictionary.Serialize("Error", "DeviceID must be a GUID.")
            End If

            Dim pOut = WebRequest.Instance.Business.TransInvoice.ExportToCsvZip(pDateFrom, pDateTo, pFowstID, pDeviceID)

            Return JsonDictionary.Serialize("Results", pOut)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function DownloadInvoiceCsvZip() As ActionResult

        Dim pFilename = Common.ServerPath & "temp\ExportedInvoicesCsv\ExportedInvoicesCsv_" & WebRequest.Instance.QueryString("BaseFilename") & ".zip"

        Return File(pFilename, "application/zip", "ExportedInvoicesCsv_" & WebRequest.Instance.QueryString("BaseFilename") & ".zip")

    End Function

    <HttpPost>
    Public Function GetFirstEodDate() As String

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pFowstID = Common.WebRequest.Data("FowstID")
        Dim pDeviceID = Common.WebRequest.Data("DeviceID")

        Dim pOut = WebRequest.Instance.Business.TransInvoice.GetFirstEodDate(pFowstID, pDeviceID)
        If pOut.HasValue Then
            Return JsonDictionary.Serialize("Results", pOut.Value.ToString(cGlobal.FormatDateSql))
        Else
            Return JsonDictionary.Serialize("Results", Now.ToString(cGlobal.FormatDateSql))
        End If

    End Function
End Class

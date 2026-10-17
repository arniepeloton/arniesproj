Public Class SalesMenuController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Dim pStr As String

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

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
    Function TableView() As ActionResult

        Return View()

    End Function
    Function Header() As ActionResult

        Return View()

    End Function
    Function POSViewAll() As ActionResult

        Return View()

    End Function
    Function Cart() As ActionResult

        Return View()

    End Function
    Function HotButtonMode() As ActionResult

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
        Dim pOut As JsonDictionary

        ' ''If WebSession.Instance.UserAccess Is Nothing _
        ' ''    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEdit _
        ' ''            OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable _
        ' ''            OrElse WebSession.Instance.UserAccess.PointOfSales_PointOfSales.IsEnable _
        ' ''    ) = False Then

        ' ''    Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''End If

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pCustomerOrganizationID = Common.WebRequest.Data("CustomerOrganizationID")
        If CStr2(pCustomerOrganizationID).Length = 0 Then
            pCustomerOrganizationID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
        End If
        If IsGuid(pCustomerOrganizationID) Then
            pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(pCustomerOrganizationID)
            If pDR IsNot Nothing Then
                pOut = New JsonDictionary
                pOut("ID") = pCustomerOrganizationID
                pOut("CompanyName") = pDR("CompanyName")
                Return pOut.Serialize
            End If
        End If

    End Function

    <HttpPost()>
    Public Function SetDefaultCustomer() As String

        Dim pCustomerOrganizationID As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
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

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        'TODO
        'WebSession.Instance.PosMode = Common.WebRequest.Data("PosMode")
        Dim posMode As String = Common.WebRequest.Data("PosMode")

        pOrderID = Common.WebRequest.Data("OrderID")
        If IsGuid(pOrderID) Then
            pOrderID = ReadyGuid(pOrderID)
            If WebRequest.Instance.Business.TransInvoice.IsValidCart(pOrderID) = False Then
                Return JsonDictionary.Serialize("text", "No item")
            End If
        Else
            If CDec2(Common.WebRequest.Data("DontFetchIfBlank")) > 0 Then
                Return JsonDictionary.Serialize("text", "No item")
            End If
            pOrderID = Nothing
        End If

        pDsCart = WebRequest.Instance.Business.TransInvoice.GetCart(pOrderID, WebSession.Instance.PeopleUserID, pFowstID)
        If pDsCart IsNot Nothing Then
            pOut = New JsonDictionary
            pOut.Add("Master", pDsCart.Tables("Invoices").Rows(0))
            pDtUnits = pDsCart.Tables("Stocks")
            If pDtUnits.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("text", "No item")
            End If
            pDtUnits = New DataView(pDtUnits, "", "LineNumber DESC", DataViewRowState.CurrentRows).ToTable

            'additional by janine sept 5 for sales order taker
            If (posMode = "so") Then
                pDtUnits = New DataView(pDtUnits, "OrderedQuantity > 0", "", DataViewRowState.CurrentRows).ToTable
            Else
                pDtUnits = New DataView(pDtUnits, "Quantity > 0", "", DataViewRowState.CurrentRows).ToTable
            End If


            pDtStocks = pDtUnits.DefaultView.ToTable(True, {"ProductID", "ProductNo", "ProductName"})
            With pDtUnits.Columns
                .Remove("ProductNo")
                .Remove("ProductName")
            End With
            pOut.Add("Stocks", pDtStocks)
            pOut.Add("Units", pDtUnits)
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

        Dim pUnitMeasure As String 'additional by janine sept 9 2014
        Dim pCreator As TransInvoice.Creator

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
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

        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pOrderID) Then
            pDT = WebRequest.Instance.Business.TransInvoice.GetCartStock(pOrderID, pProductID, Common.WebRequest.Data("PosMode"))
            If pDT.Rows.Count > 0 Then
                Common.WebRequest.ProData.BeginTransaction()
                WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pOrderID, pProductID, pUnitMeasure)

                pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator()
                pCreator.ID = pOrderID
                pCreator.UserID = WebSession.Instance.PeopleUserID
                pCreator.UpdateTotal()
                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", "That item was already removed from your cart")
            End If
        Else
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If

    End Function

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
    Public Function SaveRemarksPos() As String

        Dim pInvoiceID As String
        Dim pRemarks As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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

        WebRequest.Instance.Business.TransInvoice.VerifyCounter(pDeviceID, pFowstID, pCounter)
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

    <HttpPost()>
    Public Function GetPOSMode() As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary

        Try
            pSql = "SELECT ID, OrderedQuantity FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = " & ReadyString(Common.WebRequest.Data("InvoiceID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                While (intCt < pDT.Rows.Count)
                    pIU = New InsertUpdate("TransInvoiceItems")

                    pIU("Quantity") = Convert.ToDecimal(pDT.Rows(intCt).Item("OrderedQuantity").ToString())

                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDT.Rows(intCt).Item("ID").ToString()))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

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
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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


End Class
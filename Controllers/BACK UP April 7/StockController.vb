Public Class StockController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function GetInfo() As String

        Dim pStockID As String
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pOut As JsonDictionary
        Dim pCustomerID As String
        Dim pOrderID As String
        Dim pDtCartStock As DataTable
        Dim pDtUnits As DataTable
        Dim pDvUnits As DataView
        Dim pDrvUnit As DataRowView
        Dim pNewCartMode As Boolean

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pOut = New JsonDictionary

        pStockID = WebRequest.Instance.Data("StockID")
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
        End If
        pStockID = ReadyGuid(pStockID)

        pCustomerID = WebRequest.Instance.Data("CustomerID")
        If IsGuid(pCustomerID) Then
            pCustomerID = ReadyGuid(pCustomerID)

            pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
            If pNewCartMode = False Then
                pOrderID = Common.WebRequest.Data("OrderID")
                If IsGuid(pOrderID) Then
                    pOrderID = ReadyGuid(pOrderID)
                    If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID, pCustomerID) = False Then
                        pOrderID = Nothing
                    End If
                Else
                    pOrderID = Nothing
                End If
                If pOrderID Is Nothing Then
                    pOrderID = WebRequest.Instance.Business.LibraryCustomers.GetCartOrderID(pCustomerID)
                End If
                pOrderID = ReadyGuid(pOrderID)
                If CStr2(pOrderID).Length > 0 Then
                    pDtCartStock = WebRequest.Instance.Business.TransSalesOrder.GetCartStock(pOrderID, pStockID)
                End If
            End If
        Else
            Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
        End If

        pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pStockID))
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Non-existing Stock ID")
        End If
        pDR = pDT.Rows(0)

        pOut("StockNumber") = pDR("StockNumber")
        pOut("StockDescription") = pDR("StockDescription")
        pDtUnits = WebRequest.Instance.Business.LibraryStock.PriceTable(
            WebRequest.Instance.OrgID,
            pStockID,
            pCustomerID,
            True,
            CStr2(pCustomerID).Length = 0,
            True,
            True)
        If pDtUnits.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "The selected customer has no price levels")
        End If

        If pDtCartStock IsNot Nothing _
            AndAlso pDtCartStock.Rows.Count > 0 Then

            pDtUnits.Columns.Add("OrderedQty", GetType(Integer))
            pDtUnits.Columns.Add("RequestedUnitPrice", GetType(Decimal))
            For Each pDrCartStock As DataRow In pDtCartStock.Rows
                pDvUnits = New DataView(pDtUnits, "Quantity = " & pDrCartStock("ActualQuantity"), "", DataViewRowState.CurrentRows)
                If pDvUnits.Count > 0 Then
                    pDrvUnit = pDvUnits(0)
                    pDrvUnit("OrderedQty") = pDrCartStock("Quantity")
                    pDrvUnit("RequestedUnitPrice") = pDrCartStock("RequestedUnitPrice")
                    pDrCartStock("Quantity") = 0
                End If
            Next pDrCartStock
            If CDec2(pDtCartStock.Compute("SUM(Quantity)", "")) > 0 Then
                Stop2() ' something is wrong
            End If
        End If

        pOut.Add("UnitMeasures", pDtUnits)

        Return pOut.Serialize

    End Function

    '<HttpPost()>
    'Public Function ValidateAdjustedPrice() As String

    '    Dim pMessage As String

    '    If WebSession.Instance.UserAccess Is Nothing _
    '          OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

    '        Return JsonDictionary.Serialize("Error", "Access denied")
    '    End If

    '    If ValidateAdjustedPricePrivate(pMessage) Then
    '        Return JsonDictionary.Serialize("Result", "ok")
    '    Else
    '        Return JsonDictionary.Serialize("Warning", pMessage)
    '    End If

    'End Function

    'Private Function ValidateAdjustedPricePrivate(ByRef oMessage As String) As Boolean

    '    Dim pPriceRequest As TransSalesOrder.PriceRequest
    '    Dim pOrderID As String
    '    Dim pNewCartMode As Boolean
    '    Dim pDtUnits As DataTable
    '    Dim pDvStocks As DataView
    '    Dim pDrvStock As DataRowView
    '    Dim pStockID As String
    '    Dim pCustomerID As String
    '    Dim pDtOrderItems As DataTable
    '    Dim pDRs As DataRow()
    '    Dim pDrNew As DataRow
    '    Dim pMessage As String
    '    Dim pStockNumber As String
    '    Dim pStockDescription As String
    '    Dim pDtStock As DataTable

    '    oMessage = ""
    '    pCustomerID = Common.WebRequest.Data("CustomerID")
    '    If CStr2(pCustomerID).Length = 0 Then
    '        oMessage = "No Customer"
    '        Return False
    '    End If
    '    If IsGuid(pCustomerID) = False Then
    '        oMessage = "Invalid Customer"
    '        Return False
    '    End If
    '    pCustomerID = ReadyGuid(pCustomerID)

    '    pStockID = WebRequest.Instance.Data("StockID")
    '    If IsGuid(pStockID) = False Then
    '        oMessage = "Invalid Stock ID"
    '        Return False
    '    End If
    '    pStockID = ReadyGuid(pStockID)
    '    pDtStock = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pStockID))
    '    If pDtStock.Rows.Count = 0 Then
    '        Return JsonDictionary.Serialize("Error", "Non-existing SKU")
    '    End If
    '    pStockNumber = pDtStock.Rows(0)("StockNumber")
    '    pStockDescription = pDtStock.Rows(0)("StockDescription")

    '    pDtUnits = WebRequest.Instance.Business.LibraryStock.PriceTable(
    '        WebRequest.Instance.OrgID,
    '        pStockID,
    '        pCustomerID,
    '        True,
    '        False)

    '    pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
    '    If pNewCartMode = False Then
    '        pOrderID = Common.WebRequest.Data("OrderID")
    '        If IsGuid(pOrderID) Then
    '            pOrderID = ReadyGuid(pOrderID)
    '            If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID, pCustomerID) = False Then
    '                oMessage = "Sales Order is not valid anymore. Please choose another cart or create a new cart"
    '                Return False
    '            End If
    '        Else
    '            oMessage = "Invalid Sales Order ID"
    '            Return False
    '        End If
    '    End If

    '    WebRequest.Instance.Business.MainSecurity.UserAccess = WebSession.Instance.UserAccess
    '    pPriceRequest = New TransSalesOrder.PriceRequest(WebRequest.Instance.Business)
    '    pPriceRequest.SalesOrderID = pOrderID
    '    pPriceRequest.CustomerID = pCustomerID
    '    pDtOrderItems = pPriceRequest.Getitems()
    '    pDRs = pDtOrderItems.Select("StockID = " & ReadyString(pStockID))
    '    For Each pDR As DataRow In pDRs
    '        pDR.Delete()
    '    Next pDR
    '    pDtOrderItems.AcceptChanges()

    '    For Each pStock As Object In Common.WebRequest.AllData("Stocks")
    '        pDvStocks = New DataView(pDtUnits, "Quantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
    '        If pDvStocks.Count > 0 Then
    '            pDrvStock = pDvStocks(0)
    '            If CDec2(pDrvStock("Quantity")) >= 0 Then
    '                'pDvOrderItems = New DataView(pDtOrderItems, "StockID = " & ReadyString(pStock) &
    '                '    " AND ActualQuantity = " & pDrvStock("Quantity"), "", DataViewRowState.CurrentRows)
    '                pDrNew = pDtOrderItems.NewRow
    '                pDrNew("StockID") = pStockID
    '                pDrNew("StockNumber") = pStockNumber
    '                pDrNew("StockDescription") = pStockDescription
    '                pDrNew("ActualQuantity") = pDrvStock("Quantity")
    '                pDrNew("UnitPrice") = pDrvStock("DiscountedPrice")
    '                pDrNew("Quantity") = CDec2(pStock("Qty"))
    '                pDrNew("PriceLevelID") = pDrvStock("PriceLevelID")
    '                If CType(pStock, Dictionary(Of String, Object)).ContainsKey("RequestedPrice") Then
    '                    If CDec2(pStock("RequestedPrice")) > 0 Then
    '                        pDrNew("RequestedUnitPrice") = CDec(pStock("RequestedPrice"))
    '                    End If
    '                End If
    '                pDtOrderItems.Rows.Add(pDrNew)
    '            End If
    '        Else
    '            ' error: no matching unit quantity
    '        End If
    '    Next pStock

    '    WebRequest.Instance.Business.TransInvoice.AddProfitabilityColumns(pDtOrderItems)
    '    If pPriceRequest.HandleRequestedPrices(False, pMessage) Then
    '        Return True
    '    Else
    '        oMessage = pMessage
    '        Return False
    '    End If

    'End Function

    <HttpPost()>
    Public Function GetInfoPos() As String

        Dim pProductID As String
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pOut As JsonDictionary
        Dim pInvoiceID As String
        Dim pDtCartStock As DataTable
        Dim pDtUnits As DataTable
        Dim pDvUnits As DataView
        Dim pDrvUnit As DataRowView
        Dim pNewCartMode As Boolean
        Dim pCustomerID As String
        Dim pFilter As String
        Dim pDtSalesReps As DataTable
        Dim pDtProductSerials As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        pProductID = WebRequest.Instance.Data("ProductID")
        If IsGuid(pProductID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
        End If
        pProductID = ReadyGuid(pProductID)

        pCustomerID = WebRequest.Instance.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
        End If
        pCustomerID = ReadyGuid(pCustomerID)

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
        If pNewCartMode = False Then
            pInvoiceID = Common.WebRequest.Data("OrderID")
            If IsGuid(pInvoiceID) Then
                pInvoiceID = ReadyGuid(pInvoiceID)
                If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
                    pInvoiceID = Nothing
                End If
            Else
                pInvoiceID = Nothing
            End If
            If pInvoiceID Is Nothing Then
                pInvoiceID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, WebRequest.Instance.FowstID)
            End If
            If CStr2(pInvoiceID).Length > 0 Then
                pInvoiceID = ReadyGuid(pInvoiceID)
                pDtCartStock = WebRequest.Instance.Business.TransInvoice.GetCartStock(pInvoiceID, pProductID, Common.WebRequest.Data("PosMode"))
            End If
        End If

        pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID))
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Non-existing Product ID")
        End If
        pDR = pDT.Rows(0)

        pOut("ProductNo") = pDR("ProductNo")
        pOut("ProductName") = pDR("ProductName")
        pDtUnits = WebRequest.Instance.Business.LibraryStock.PriceTable(
            WebRequest.Instance.OrgID,
            pProductID,
            pCustomerID,
            ,
            ,
            True)
        If pDtUnits.Rows.Count = 0 Then
            pOut.Add("Warning", "This product has no price or unit.")
            Return pOut.Serialize
        End If

        pFilter = "PriceLevelID = " & ReadyString(ReadyGuid(WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID))
        If pDtCartStock IsNot Nothing _
            AndAlso pDtCartStock.Rows.Count > 0 Then

            pDtSalesReps = WebRequest.Instance.Business.TransInvoice.GetSalesReps(pInvoiceID, pProductID)
            pOut.Add("SalesReps", pDtSalesReps)

            pDtProductSerials = WebRequest.Instance.Business.TransInvoice.GetProductSerial(pInvoiceID, pProductID)
            pOut.Add("ProductSerial", pDtProductSerials)

            pDtUnits.Columns.Add("OrderedQty", GetType(Integer))
            For Each pDrCartStock As DataRow In pDtCartStock.Rows
                pDvUnits = New DataView(pDtUnits, "Quantity = " & pDrCartStock("ActualQuantity") & _
                    " AND PriceLevelID = " & ReadyString(ReadyGuid(pDrCartStock("PriceLevelID"))), "", DataViewRowState.CurrentRows)
                If pDvUnits.Count > 0 Then
                    pDrvUnit = pDvUnits(0)
                    pDrvUnit("OrderedQty") = pDrCartStock("Quantity")
                    pDrCartStock("Quantity") -= pDrCartStock("Quantity")
                End If
            Next pDrCartStock
            If CDec2(pDtCartStock.Compute("SUM(Quantity)", "")) <> 0 Then
                Stop2() ' something is wrong
            End If
            pFilter &= " OR OrderedQty > 0"
        End If

        pOut.Add("data", pDtUnits)
        pDtUnits = New DataView(pDtUnits, pFilter, _
            "Quantity, UnitMeasure", DataViewRowState.CurrentRows).ToTable

        Dim pDtUniqueUnits As DataTable
        pDtUniqueUnits = New DataView(pDtUnits, "", "", DataViewRowState.CurrentRows).ToTable(True, {"Quantity"})
        For Each pDrUniqueUnit As DataRow In pDtUniqueUnits.Rows
            pDvUnits = New DataView(pDtUnits, "Quantity = " & pDrUniqueUnit("Quantity"), "MeasurementLevel DESC", DataViewRowState.CurrentRows)
            If pDvUnits.Count > 1 Then
                pFilter = "Quantity = " & pDrUniqueUnit("Quantity")
                If pDtCartStock IsNot Nothing _
                    AndAlso pDtCartStock.Rows.Count > 0 Then

                    pFilter &= " AND ISNULL(OrderedQty, 0) <= 0"
                    pDvUnits = New DataView(pDtUnits, pFilter, "", DataViewRowState.CurrentRows)
                    For Each pDRV As DataRowView In pDvUnits
                        pDRV.Delete()
                    Next pDRV
                Else
                    pDvUnits = New DataView(pDtUnits, pFilter, "", DataViewRowState.CurrentRows)
                    Do Until pDvUnits.Count = 1
                        pDvUnits(0).Delete()
                    Loop
                End If
            End If
        Next pDrUniqueUnit
        pDtUnits.AcceptChanges()
        pOut.Add("UnitMeasures", pDtUnits)

        Return pOut.Serialize

    End Function

    '<HttpPost()>
    'Public Function Update() As String

    '    If WebSession.Instance.UserAccess Is Nothing _
    '        OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsView OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable) = False Then

    '        Return JsonDictionary.Serialize("Error", "Access denied")
    '    End If

    '    Dim pCustomerID As String
    '    Dim pStockID As String
    '    Dim pMessage As String
    '    Dim pApprovedPrices As Boolean
    '    Dim pDisapprovedMessage As String
    '    Dim pOut As JsonDictionary

    '    pCustomerID = Common.WebRequest.Data("CustomerID")
    '    If CStr2(pCustomerID).Length = 0 Then
    '        Return JsonDictionary.Serialize("Error", "No Customer")
    '    End If
    '    If IsGuid(pCustomerID) = False Then
    '        Return JsonDictionary.Serialize("Error", "Invalid Customer")
    '    End If
    '    pCustomerID = ReadyGuid(pCustomerID)

    '    pStockID = Common.WebRequest.Data("StockID")
    '    If CStr2(pStockID).Length = 0 Then
    '        Return JsonDictionary.Serialize("Error", "No Stock")
    '    End If
    '    If IsGuid(pStockID) = False Then
    '        Return JsonDictionary.Serialize("Error", "Invalid Stock")
    '    End If
    '    pStockID = ReadyGuid(pStockID)

    '    If CStr2(WebSession.Instance.SubscriberOrganizationBranchID).Length = 0 Then
    '        pOut = New JsonDictionary
    '        pOut.Add("Error", "SubscriberOrganizationBranchID is not set.")
    '        pOut.Add("ReloadBrowser", 1)
    '        Return pOut.Serialize
    '    End If

    '    Dim pOrderID As String
    '    Dim pCreator As TransSalesOrder.Creator
    '    Dim pItem As TransSalesOrder.Creator.Item
    '    Dim pDtStocks As DataTable
    '    Dim pDvStocks As DataView
    '    Dim pDrvStock As DataRowView
    '    Dim pNewCartMode As Boolean

    '    Common.WebRequest.ProData.BeginTransaction()
    '    pDtStocks = WebRequest.Instance.Business.LibraryStock.PriceTable(
    '        WebRequest.Instance.OrgID,
    '        pStockID,
    '        pCustomerID,
    '        True,
    '        False)

    '    pCreator = WebRequest.Instance.Business.TransSalesOrder.CreateCreator()

    '    pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
    '    If pNewCartMode = False Then
    '        pOrderID = Common.WebRequest.Data("OrderID")
    '        If IsGuid(pOrderID) Then
    '            pOrderID = ReadyGuid(pOrderID)
    '            If WebRequest.Instance.Business.TransSalesOrder.IsValidCart(pOrderID, pCustomerID) = False Then
    '                pOrderID = Nothing
    '            End If
    '        Else
    '            pOrderID = Nothing
    '        End If
    '        If pOrderID Is Nothing Then
    '            pOrderID = WebRequest.Instance.Business.LibraryCustomers.GetCartOrderID(pCustomerID)
    '        End If
    '        pOrderID = ReadyGuid(pOrderID)
    '    End If

    '    If CStr2(pOrderID).Length > 0 Then
    '        WebRequest.Instance.Business.TransSalesOrder.DeleteOrderStock(pOrderID, pStockID)
    '    Else
    '        pCreator.DeliveryDate = Now
    '    End If
    '    pCreator.BranchID = WebRequest.Instance.Business.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 5)
    '    pCreator.SubscriberBranchID = WebSession.Instance.SubscriberOrganizationBranchID
    '    pCreator.DeviceID = WebSession.Instance.DeviceID
    '    pCreator.CustomerID = pCustomerID
    '    pCreator.CustomerAddressID = WebRequest.Instance.Business.LibraryCustomers.GetBillingAddress(pCreator.CustomerID)
    '    If CStr2(pCreator.CustomerAddressID) = "" Then
    '        Return JsonDictionary.Serialize("Error", "Customer has no billing address")
    '    End If
    '    pCreator.DeliverToID = WebRequest.Instance.Business.LibraryCustomers.GetDeliveryAddress(pCreator.CustomerID)
    '    If CStr2(pCreator.DeliverToID) = "" Then
    '        Return JsonDictionary.Serialize("Error", "Customer has no delivery address")
    '    End If
    '    pCreator.CusRepresentativeID = WebRequest.Instance.Business.LibraryCustomers.GetOrAddContactPerson(pCreator.CustomerID)

    '    pCreator.IsHold = True
    '    pCreator.IsForApproval = True
    '    pCreator.DontAutoInvoice = False

    '    pCreator.UserID = WebSession.Instance.EmployeeID
    '    'pCreator.Remarks = "Made by Mobiz"

    '    Dim pCustomerTerm As String
    '    Dim pUseCustomerTerm As Boolean
    '    Dim pDrStockTerms As DataRow
    '    Dim pStockDiscount As Single
    '    Dim pStockDiscountIfPaidWithIn As Decimal
    '    Dim pStockTerm As String = "0"

    '    pCustomerTerm = WebRequest.Instance.Business.TransSalesOrder.GetCustomerMaxTerms(pCustomerID)
    '    pDrStockTerms = WebRequest.Instance.Business.TransSalesOrder.GetStocksTermsDiscAndNetDue(pStockID)
    '    If pDrStockTerms IsNot Nothing Then
    '        pStockDiscount = pDrStockTerms("DiscountPercentage")
    '        pStockDiscountIfPaidWithIn = pDrStockTerms("DiscountPaidWithin")
    '        pStockTerm = pDrStockTerms("NetDue")
    '    End If
    '    pApprovedPrices = ValidateAdjustedPricePrivate(pDisapprovedMessage)

    '    For Each pStock As Object In Common.WebRequest.AllData("Stocks")
    '        pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
    '        If pDvStocks.Count > 0 Then
    '            pDrvStock = pDvStocks(0)
    '            If CDec2(pStock("Qty")) >= 0 Then
    '                pItem = New TransSalesOrder.Creator.Item
    '                If IsNumeric(pCustomerTerm) Then
    '                    If IsNumeric(pStockTerm) Then
    '                        If CLng(pStockTerm) > 0 Then
    '                            If CLng(pCustomerTerm) < CLng(pStockTerm) Then
    '                                pUseCustomerTerm = True
    '                            Else
    '                                pUseCustomerTerm = False
    '                            End If
    '                        Else
    '                            pUseCustomerTerm = True
    '                        End If
    '                    Else
    '                        pUseCustomerTerm = False
    '                    End If
    '                Else
    '                    If IsNumeric(pStockTerm) Then
    '                        pUseCustomerTerm = False
    '                    Else
    '                        pUseCustomerTerm = True
    '                    End If
    '                End If

    '                pItem.StockID = pStockID
    '                pItem.UnitMeasure = pDrvStock("UnitMeasure")
    '                pItem.PriceLevelID = CStr2(pDrvStock("PriceLevelID"))
    '                pItem.ActualQuantity = pDrvStock("Quantity")
    '                pItem.UnitPrice = pDrvStock("DiscountedPrice")
    '                If CType(pStock, Dictionary(Of String, Object)).ContainsKey("RequestedPrice") _
    '                    AndAlso CDec2(pStock("RequestedPrice")) > 0 Then

    '                    If pApprovedPrices Then
    '                        pItem.UnitPrice = CDec(pStock("RequestedPrice"))
    '                    End If
    '                    pItem.RequestedUnitPrice = CDec(pStock("RequestedPrice"))
    '                End If
    '                pItem.OriginalUnitPrice = pDrvStock("DiscountedPrice")
    '                pItem.UnitPriceWOTax = pItem.UnitPrice
    '                pItem.Quantity = CDec2(pStock("Qty"))
    '                pItem.SubTotal = pItem.UnitPrice * pItem.Quantity
    '                pItem.Cost = WebRequest.Instance.Business.LibraryStock.GetLandedCost(pStockID) * pItem.ActualQuantity
    '                pItem.CancellationPeriod = CDec2(WebRequest.Instance.Business.SystemDefaults.GetOrAddSystemDefault("BackOrder Cancellation Period", 15))

    '                pItem.TermDiscount = pStockDiscount
    '                pItem.DiscountIfPaidWithIn = Math.Min(CDec2(pCustomerTerm), pStockDiscountIfPaidWithIn)
    '                pItem.NetDue = If(pUseCustomerTerm, pCustomerTerm, pStockTerm)

    '                If CStr2(pOrderID).Length > 0 Then
    '                    pItem.LineNumber = Math.Max(0, WebRequest.Instance.Business.TransSalesOrder.GetMaxLineNumber(pOrderID)) + 1
    '                Else
    '                    pItem.LineNumber = 1
    '                End If
    '                pCreator.Items.Add(pItem)
    '            End If
    '        Else
    '            ' error: no matching unit quantity
    '        End If
    '    Next pStock
    '    If CStr2(pOrderID).Length > 0 Then
    '        pCreator.ID = pOrderID
    '        pCreator.OrderNo = -1   ' -1 = dont update OrderNo
    '        pCreator.Save(False)
    '    Else
    '        pCreator.Save(True)
    '    End If
    '    Common.WebRequest.ProData.CommitTransaction()

    '    If pApprovedPrices Then
    '        Return JsonDictionary.Serialize("Result", "ok")
    '    Else
    '        Return JsonDictionary.Serialize("Warning", pDisapprovedMessage)
    '    End If

    'End Function

    <HttpPost()>
    Public Function UpdatePos() As String

        Dim pStockID As String
        Dim pCustomerID As String
        Dim pOut As JsonDictionary
        Dim isNewInvoice As Boolean = True 'additional by janine sept 9 2014. important do not remove

        Dim posMode As String = Common.WebRequest.Data("PosMode")

        pStockID = Common.WebRequest.Data("StockID")
        If CStr2(pStockID).Length = 0 Then
            Return JsonDictionary.Serialize("Error", "No Stock")
        End If
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock")
        End If
        pStockID = ReadyGuid(pStockID)

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
        End If
        pCustomerID = ReadyGuid(pCustomerID)

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        If IsGuid(WebRequest.Instance.FowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
        End If

        Dim pInvoiceID As String
        Dim pCreator As TransInvoice.Creator
        Dim pDtStocks As DataTable
        Dim pDvStocks As DataView
        Dim pDrvStock As DataRowView
        Dim pNewCartMode As Boolean
        Dim pRetailID As String
        Dim pError As String

        pRetailID = WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID
        If IsGuid(pRetailID) = False Then
            Return JsonDictionary.Serialize("Error", "Retail Price ID was not set in System Defaults")
        End If
        pRetailID = ReadyGuid(pRetailID)

        If Client.CompareLocalTimeAndSqlServerTime(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If
        'vPosMode added by janine sept 8 2014 for sales order taker. fowst is not required
        If WebRequest.Instance.Business.LibraryPeople.Validate(
            vPeopleID:=WebSession.Instance.PeopleUserID,
            vOrgID:=WebRequest.Instance.OrgID,
            vFowstID:=WebRequest.Instance.FowstID,
            vPosMode:=posMode,
            vDeviceID:=WebRequest.Instance.DeviceID,
            vDeviceUuid:=Nothing,
            vVerifyUuid:=False,
            oError:=pError) = False Then

            Return JsonDictionary.Serialize("Error", pError)
        End If

        Common.WebRequest.ProData.BeginTransaction()

        pDtStocks = WebRequest.Instance.Business.LibraryStock.PriceTable(
            WebRequest.Instance.OrgID,
            pStockID,
            pCustomerID)
        For Each pStock As Object In Common.WebRequest.AllData("Stocks")
            If pStock Is Nothing Then Continue For
            pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")) &
                " AND PriceLevelID = " & ReadyString(pStock("PriceLevelID")), "", DataViewRowState.CurrentRows)
            If pDvStocks.Count > 0 Then
                pDrvStock = pDvStocks(0)
                If CDec2(pStock("Qty")) >= 0 Then
                    If Math.Round(pDrvStock("DiscountedPrice"), 2) < _
                        Math.Round( _
                                WebRequest.Instance.Business.LibraryStock.GetLandedCost(pStockID, WebRequest.Instance.OrgID) * CDec2(pStock("UnitQty")) _
                        , 2) Then

                        ' tanggal muna trapping. march 14, 2014
                        'Common.WebRequest.ProData.RollbackTransaction()
                        'Return JsonDictionary.Serialize("Error", "Selling price must be greater than or equal to the landed cost. To adjust prices, please go to Home -> System Administration -> Data Libraries -> PIM -> Adjust Product Prices")
                    End If
                End If
            Else
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", "No matching unit quantity and price level")
            End If
        Next pStock

        pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator

        pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
        If pNewCartMode = False Then
            pInvoiceID = Common.WebRequest.Data("OrderID") ' really InvoiceID
            If IsGuid(pInvoiceID) Then
                pInvoiceID = ReadyGuid(pInvoiceID)
                If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
                    pInvoiceID = Nothing
                End If
            Else
                pInvoiceID = Nothing
            End If
            If pInvoiceID Is Nothing Then
                pInvoiceID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.EmployeeID, WebSession.Instance.SubscriberOrganizationBranchID)
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)
            isNewInvoice = False 'additional by janine sept 9 2014. important do not remove
        End If

        'If CStr2(pInvoiceID).Length > 0 Then
        '    pCreator.ID = pInvoiceID
        '    WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pInvoiceID, pStockID)
        'End If

        pCreator.InvoiceNumber = 0
        pCreator.InvoiceDate = Nothing ' nothing to use GETDATE() from SQL Server
        pCreator.FowstID = WebRequest.Instance.FowstID
        pCreator.DeviceID = WebRequest.Instance.DeviceID
        pCreator.CustomerID = pCustomerID
        pCreator.DeliveryAddressID = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
        pCreator.BillingAddressID = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Billing Address ID")
        pCreator.UserID = WebSession.Instance.PeopleUserID

        pCreator.ProductID = Common.WebRequest.Data("ProductID")
        pCreator.SalesRepList = Common.WebRequest.AllData("SalesRep")
        pCreator.ProductSerialList = Common.WebRequest.AllData("ProductSerial")

        ''additional by janine apr 1 2014
        If CStr2(Common.WebRequest.Data("CustomerTableNo")).Length > 0 Then pCreator.CustomerTableNo = Common.WebRequest.Data("CustomerTableNo")

        ''additional oct 24
        If CStr2(Common.WebRequest.Data("DeliveryMethod")).Length > 0 Then pCreator.DeliveryMethod = Common.WebRequest.Data("DeliveryMethod")
        If CStr2(Common.WebRequest.Data("OrderMethod")).Length > 0 Then pCreator.OrderMethod = Common.WebRequest.Data("OrderMethod")
        If CStr2(Common.WebRequest.Data("Gender")).Length > 0 Then pCreator.CustomerDemographicGender = Common.WebRequest.Data("Gender")
        If CStr2(Common.WebRequest.Data("Age")).Length > 0 Then pCreator.CustomerDemographicAge = Common.WebRequest.Data("Age")
        ''end of additional

        Dim pDtStock As DataTable
        Dim pDrStock As DataRow
        Dim orderedQty As DataTable

        pDtStock = New DataTable
        With pDtStock.Columns
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Integer))
            .Add("Quantity", GetType(Integer))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
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
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
        End With

        For Each pStock As Object In Common.WebRequest.AllData("Stocks")
            If pStock Is Nothing Then Continue For
            pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")) &
                " AND PriceLevelID = " & ReadyString(pStock("PriceLevelID")), "", DataViewRowState.CurrentRows)
            If pDvStocks.Count > 0 Then
                pDrvStock = pDvStocks(0)
                If CDec2(pStock("Qty")) >= 0 Then
                    pDrStock = pDtStock.NewRow
                    pDrStock("StockID") = pStockID
                    'additional by janine sept 3 2014 for order taker
                    If Not isNewInvoice Then 'if existing invoice then delete the invoiceitems that match. additional by janine sept 9 2014  Then
                        orderedQty = WebRequest.Instance.Business.TransInvoice.GetOrderedQty(pInvoiceID, pStockID, pDrvStock("UnitMeasure"))
                        pCreator.ID = pInvoiceID
                        WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pInvoiceID, pStockID, pDrvStock("UnitMeasure"))
                    End If

                    If (posMode = "so") Then
                        pDrStock("Quantity") = 0 'need to save it as 0 because it's not yet invoiced!
                        pDrStock("OrderedQuantity") = CDec2(pStock("Qty"))
                    Else
                        pDrStock("Quantity") = CDec2(pStock("Qty"))
                        If (orderedQty Is Nothing) Then
                            pDrStock("OrderedQuantity") = CDec2(pStock("Qty")) 'save ordered qty same as invoiced qty for POS. so ordered = invoiced
                        Else
                            If (orderedQty.Rows.Count > 0) Then
                                pDrStock("OrderedQuantity") = orderedQty.Rows(0).Item("OrderedQuantity")
                            Else
                                pDrStock("OrderedQuantity") = CDec2(pStock("Qty")) 'save ordered qty same as invoiced qty for POS. so ordered = invoiced
                            End If
                        End If


                    End If
                    'end of additional
                    pDrStock("ActualQuantity") = pDrvStock("Quantity")
                    pDrStock("UnitMeasure") = pDrvStock("UnitMeasure")
                    pDrStock("UnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
                    pDrStock("Cost") = WebRequest.Instance.Business.LibraryStock.GetLandedCost(pStockID, WebRequest.Instance.OrgID) * pDrStock("ActualQuantity")
                    If ReadyGuid(pDrvStock("PriceLevelID")) <> pRetailID Then
                        'todo: add security
                        'If WebSession.Instance.UserAccess.PointOfSales_PriceLevelMode.IsEnable _
                        '    OrElse WebSession.Instance.UserAccess.PointOfSales_UnlimitedUserMode.IsEnable Then

                        'Else
                        '    Common.WebRequest.ProData.RollbackTransaction()
                        '    Return JsonDictionary.Serialize("Error", "You are not authorized to set price level other than retail")
                        'End If
                    End If
                    pDrStock("PriceLevelID") = CStr2(pDrvStock("PriceLevelID"))
                    pDrStock("Dimension") = ""
                    pDrStock("IsReplacement") = False
                    pDrStock("IsHasReplacement") = False
                    pDrStock("IsHasBackOrder") = False
                    pDrStock("RefStockID") = ""

                    Dim pUnitGp As Decimal
                    Dim pUnitGpm As Double
                    Dim pLineGp As Decimal
                    Dim pLineGpm As Decimal
                    Dim pLandedCost As Decimal

                    'additional by janine sept 5 for sales order taker
                    If (posMode = "so") Then
                        WebRequest.Instance.Business.LibraryStock.GetProfitability(
                            vStockID:=CStr2(pDrStock("StockID")),
                            vQty:=pDrStock("OrderedQuantity"),
                            vUnitQty:=pDrStock("ActualQuantity"),
                            vUnitPrice:=pDrStock("UnitPrice"),
                            oUnitGP:=pUnitGp,
                            oUnitGPM:=pUnitGpm,
                            oLineGP:=pLineGp,
                            oLineGPM:=pLineGpm,
                            oLandedCost:=pLandedCost,
                            vHistoricCost:=0,
                            vOrgID:=WebRequest.Instance.OrgID)

                    Else

                        WebRequest.Instance.Business.LibraryStock.GetProfitability(
                       vStockID:=CStr2(pDrStock("StockID")),
                       vQty:=pDrStock("Quantity"),
                       vUnitQty:=pDrStock("ActualQuantity"),
                       vUnitPrice:=pDrStock("UnitPrice"),
                       oUnitGP:=pUnitGp,
                       oUnitGPM:=pUnitGpm,
                       oLineGP:=pLineGp,
                       oLineGPM:=pLineGpm,
                       oLandedCost:=pLandedCost,
                       vHistoricCost:=0,
                       vOrgID:=WebRequest.Instance.OrgID)

                    End If



                    pDrStock("UnitGP") = pUnitGp
                    pDrStock("UnitGPM") = pUnitGpm
                    pDrStock("LineGP") = pLineGp
                    pDrStock("LineGPM") = pLineGpm

                    pDrStock("LineCost") = pLandedCost * CDec2(pStock("Qty")) * CDec2(pDrStock("ActualQuantity"))

                    'additional by janine sept 5 for sales order taker
                    If (posMode = "so") Then
                        pDrStock("Subtotal") = pDrStock("OrderedQuantity") * pDrStock("UnitPrice")
                    Else
                        pDrStock("Subtotal") = pDrStock("Quantity") * pDrStock("UnitPrice")
                    End If

                    pDrStock("TaxPercent") = 12 ' todo
                    pDrStock("TaxSales") = RoundDown(pDrStock("SubTotal") / (1 + pDrStock("TaxPercent") / 100), 4)
                    pDrStock("TaxAmount") = pDrStock("SubTotal") - pDrStock("TaxSales")

                    If CStr2(pInvoiceID).Length > 0 Then
                        pDrStock("LineNumber") = Math.Max(0, WebRequest.Instance.Business.TransInvoice.GetMaxLineNumber(pInvoiceID)) + 1
                    Else
                        pDrStock("LineNumber") = 1
                    End If
                    '' ''If pStock.containskey("SalesRep") Then
                    '' ''    pDrStock("SalesRep") = pStock("SalesRep")
                    '' ''End If
                    pDtStock.Rows.Add(pDrStock)
                Else
                    Return JsonDictionary.Serialize("Error", "No matching unit quantity and price level")
                End If
            End If
        Next pStock

        pCreator.Stocks = pDtStock
        If CStr2(pInvoiceID).Length > 0 Then
            pCreator.ID = pInvoiceID
            If pCreator.Save(False, pError) Then
                Common.WebRequest.ProData.CommitTransaction()
                pOut = New JsonDictionary
                pOut.Add("Result", "ok")

                If Not (pDrvStock Is Nothing) Then
                    pOut.Add("DiscountedPrice", Math.Round(pDrvStock("DiscountedPrice"), 2)) 'additional by janine. for hot buttons i need to get the price
                End If

                Return pOut.Serialize
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If
        Else
            If pCreator.Save(True, pError) Then
                Common.WebRequest.ProData.CommitTransaction()
                pOut = New JsonDictionary
                pOut.Add("Result", "ok")
                pOut.Add("OrderID", pCreator.ID)

                If Not (pDrvStock Is Nothing) Then
                    pOut.Add("DiscountedPrice", Math.Round(pDrvStock("DiscountedPrice"), 2)) 'additional by janine. for hot buttons i need to get the price
                End If

                Return pOut.Serialize
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If
        End If


    End Function

    <HttpPost()>
    Public Function AlertNoPrice()

        Dim pProductID As String

        Try
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pProductID = Common.WebRequest.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid ProductID")
            End If
            pProductID = ReadyGuid(pProductID)
            WebRequest.Instance.Business.LibraryStock.AddAlert(pProductID, "No price", WebSession.Instance.PeopleUserID)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function GetInventoryBalance() As String

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True
            With .Fields
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "t.Store_ID"
                End With
                With .Add("Balance", SqlDbType.Float)
                    .Expression = "SUM(t.balance)"
                End With
            End With

            .SqlFrom = "FROM (SELECT DISTINCT " &
                        "dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName, " &
                        "dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID AS Account_ID,  " &
                        "dbo.MobizLibraryOrganizationFOWST.Name AS Store_ID, " &
                        "bal1.Balance " &
                        "FROM dbo.LibraryOrganizationPIM LEFT OUTER JOIN dbo.MobizLibraryOrganization INNER JOIN " &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " &
                        "dbo.TransInventoryLedger AS t0 INNER JOIN " &
                        "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON  " &
                        "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID " &
                        "outer apply(  " &
                     "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " &
                     "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 	 " &
                     "WHERE dbo.MobizLibraryPIMProduct.ID = " & ReadyString(Common.WebRequest.Data("StockID")) & " AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 " &
                     ") t   "
            .SqlGroup = "group by Account_ID, Store_ID, UPC_Code, Product_Code"

            pDT = Common.WebRequest.ProData.Execute(.CreateSql)
        End With

        Return JsonDictionary.Serialize("Results", pDT)
    End Function

    <HttpPost()>
    Public Function GetUnits() As String

        Dim pProductID As String
        Dim pDT As DataTable

        pProductID = WebRequest.Instance.Data("ProductID")
        If IsGuid(pProductID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Product ID")
        End If
        pProductID = ReadyGuid(pProductID)

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pDT = WebRequest.Instance.Business.LibraryStockMeasurements.ByProductID(pProductID)

        Return JsonDictionary.Serialize(pDT)

    End Function

    <HttpPost()>
    Public Function GetBomData() As String

        Dim pProductID As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbME2WL3MQ88!")
            End If
            pProductID = ReadyGuid(pProductID)

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Parent product description did not return a record vb5P4WM7JX2R!")
            End If
            pOut.Add("parent", pDT.Rows(0))

            pDT = WebRequest.Instance.Business.LibraryStock.GetContents(pProductID, WebRequest.Instance.OrgID)
            pOut.Add("contents", pDT)

            Return pOut.Serialize()
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function GetBomPageData() As String

        Dim pParentProductID As String
        Dim pChildProductID As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        Dim pDrCustomUnit As DataRow
        Dim pDrNewUnit As DataRow
        Dim pDtFamilyMeasure As DataTable

        Try
            pParentProductID = WebRequest.Instance.Data("ParentProductID")
            If IsGuid(pParentProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Parent Product ID vb3UMXUNLSA8!")
            End If
            pParentProductID = ReadyGuid(pParentProductID)

            pChildProductID = WebRequest.Instance.Data("ChildProductID")
            If IsGuid(pChildProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Child Product ID vbBUBTAER5Q9!")
            End If
            pChildProductID = ReadyGuid(pChildProductID)

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pParentProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Parent product description did not return a record vbGBBJYQVJC4!")
            End If
            pOut.Add("parent", pDT.Rows(0))

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pChildProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Child product description did not return a record vbFR5PQTX6X8!")
            End If
            pOut.Add("child", pDT.Rows(0))

            pDT = WebRequest.Instance.Business.LibraryStockMeasurements.ByProductID(pChildProductID)

            pDrCustomUnit = WebRequest.Instance.Business.LibraryStockMeasurements.GetCustomUnit(pChildProductID, WebRequest.Instance.OrgID)
            If pDrCustomUnit IsNot Nothing Then
                pDtFamilyMeasure = WebRequest.Instance.Business.LibraryStockMeasurements.GetFamilyMeasurement(pDrCustomUnit("DestinationMeasure"))
                For Each pDrFamilyMeasure As DataRow In pDtFamilyMeasure.Rows
                    pDrNewUnit = pDT.NewRow
                    pDrNewUnit("UnitMeasure") = pDrFamilyMeasure("DestMeasure") & ""
                    pDT.Rows.Add(pDrNewUnit)
                Next pDrFamilyMeasure
            End If
            pOut.Add("units", pDT)

            pDT = WebRequest.Instance.Business.LibraryStock.GetContents(pParentProductID, pChildProductID, WebRequest.Instance.OrgID)
            If pDT.Rows.Count > 0 Then
                pOut.Add("contents", pDT.Rows(0))
            End If

            Return pOut.Serialize()
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveBomPageData() As String

        Dim pParentProductID As String
        Dim pChildProductID As String
        Dim pQty As String
        Dim pUnitMeasureID As String
        Dim pUnitMeasure As String ' source: MobizLibraryPIMMeasureConversion.DestMeasure

        pParentProductID = WebRequest.Instance.Data("ParentProductID")
        If IsGuid(pParentProductID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Parent Product ID vbBKRHPGSNKM!")
        End If
        pParentProductID = ReadyGuid(pParentProductID)

        pChildProductID = WebRequest.Instance.Data("ChildProductID")
        If IsGuid(pChildProductID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Child Product ID vbHE2S2ZQFYF!")
        End If
        pChildProductID = ReadyGuid(pChildProductID)

        pQty = WebRequest.Instance.Data("Qty")
        If CStr2(pQty).Trim() = "" Then
            Return JsonDictionary.Serialize("Error", "Qty is missing vbKZW9PNXGT9!")
        End If
        If IsNumeric(pQty) = False Then
            Return JsonDictionary.Serialize("Error", "Qty must be a valid number vb2ET9D43NPT!")
        End If
        pQty = CDec2(pQty)
        If CDec(pQty) < 0 Then
            Return JsonDictionary.Serialize("Error", "Qty must not be negative vbTG2YV52RX3!")
        End If

        pUnitMeasureID = WebRequest.Instance.Data("UnitMeasureID")
        If CStr2(pUnitMeasureID).Trim() <> "" Then
            If IsGuid(pUnitMeasureID) = False Then
                Return JsonDictionary.Serialize("Error", "UnitMeasureID must be a guid vbEVN949BJ2L!")
            End If
            pUnitMeasureID = ReadyGuid(pUnitMeasureID)
        End If

        pUnitMeasure = WebRequest.Instance.Data("UnitMeasure")
        If CStr2(pUnitMeasureID).Trim() = "" _
            AndAlso CStr2(pUnitMeasure).Trim() = "" Then

            Return JsonDictionary.Serialize("Error", "UnitMeasureID and UnitMeasure cannot both be null. vbJCPNWLYZF8!")
        End If

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pProData As ProData
        Dim pIU As InsertUpdate
        Dim pSql As String

        pProData = Common.WebRequest.ProData

        pSql = "DELETE FROM MobizLibraryPIMProductContents" &
            " WHERE ParentProductID = " & ReadyString(pParentProductID) &
            " AND ChildProductID = " & ReadyString(pChildProductID) &
            " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
        pProData.ExecuteSQL(pSql)

        '// insert/update LibraryPIMProductContents
        pIU = New InsertUpdate("MobizLibraryPIMProductContents")
        pIU("ID") = "NEWID()"
        pIU("ParentProductID") = ReadyString(pParentProductID)
        pIU("ChildProductID") = ReadyString(pChildProductID)
        pIU("Quantity") = pQty
        If CStr2(pUnitMeasureID).Trim() <> "" Then
            pIU("UnitMeasureID") = ReadyString(pUnitMeasureID)
        Else
            pIU("UnitMeasure") = ReadyString(pUnitMeasure)
        End If
        pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
        pSql = pIU.GetInsert()
        pProData.ExecuteSQL(pSql)

        '// update parent LibraryPIMProduct
        pIU = New InsertUpdate("MobizLibraryPIMProduct")
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(pParentProductID))
        pProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function DeleteBomContent() As String

        Try
            Dim pParentProductID As String
            Dim pChildProductID As String

            pParentProductID = WebRequest.Instance.Data("ParentProductID")
            If IsGuid(pParentProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Parent Product ID vbRP7ELVZCGU!")
            End If
            pParentProductID = ReadyGuid(pParentProductID)

            pChildProductID = WebRequest.Instance.Data("ChildProductID")
            If IsGuid(pChildProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Child Product ID vbVK3KZ5Q4BB!")
            End If
            pChildProductID = ReadyGuid(pChildProductID)

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pProData As ProData
            Dim pSql As String

            pProData = Common.WebRequest.ProData

            pSql = "DELETE FROM MobizLibraryPIMProductContents" &
                   " WHERE ParentProductID = " & ReadyString(pParentProductID) &
                   " AND ChildProductID = " & ReadyString(pChildProductID) &
                   " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
            pProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")

        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function GetUnitDefinition() As String

        Dim pProductID As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        Dim pDR As DataRow

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbP4H3KTBL5D!")
            End If
            pProductID = ReadyGuid(pProductID)

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Product description did not return a record vb77BUM6EN4Z!")
            End If
            pOut.Add("parent", pDT.Rows(0))

            pDR = WebRequest.Instance.Business.LibraryStock.GetUnitDefinition(pProductID, WebRequest.Instance.OrgID)
            If pDR IsNot Nothing Then
                pOut.Add("content", pDR)
            End If

            pDT = WebRequest.Instance.Business.LibraryStockMeasurements.MeasurementFamilies()
            pOut.Add("families", pDT)

            Return pOut.Serialize()
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveUnitDefinition() As String

        Dim pProductID As String
        Dim pSourceMeasureID As String
        Dim pQuantity As String
        Dim pDestinationMeasure As String

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbX3S252WJX4!")
            End If
            pProductID = ReadyGuid(pProductID)

            pSourceMeasureID = WebRequest.Instance.Data("SourceMeasureID")
            If IsGuid(pSourceMeasureID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbF6QSKJZDWD!")
            End If
            pSourceMeasureID = ReadyGuid(pSourceMeasureID)

            pQuantity = WebRequest.Instance.Data("Quantity")
            If IsNumeric(pQuantity) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Quantity vbYUHN2ZV8JX!")
            End If
            pQuantity = CDec2(pQuantity)
            If CDec(pQuantity) <= 0 Then
                Return JsonDictionary.Serialize("Error", "Quantity must be greater than zero vbLNB584MKRV!")
            End If

            pDestinationMeasure = WebRequest.Instance.Data("DestinationMeasure")
            If CStr2(pDestinationMeasure).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "Invalid DestinationMeasure vbNPR5P73B2V!")
            End If

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProData As ProData = Common.WebRequest.ProData

            pProData.BeginTransaction()
            Try
                pSql = "DELETE FROM MobizLibraryPIMMeasureDefinition" &
                    " WHERE ProductID = " & ReadyString(pProductID) &
                    " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                pProData.ExecuteSQL(pSql)

                pIU = New InsertUpdate("MobizLibraryPIMMeasureDefinition")
                pIU("ID") = "newid()"
                pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                pIU("ProductID") = ReadyString(pProductID)
                pIU("SourceMeasureID") = ReadyString(pSourceMeasureID)
                pIU("DestinationMeasure") = ReadyString(pDestinationMeasure)
                pIU("Quantity") = pQuantity
                pSql = pIU.GetInsert()
                pProData.ExecuteSQL(pSql)
                pProData.CommitTransaction()
            Catch ex As System.Exception
                pProData.RollbackTransaction(, True)
                Throw
            End Try

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "vbTWMKH8VTGF! " & ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveBomInstruction() As String

        Dim pProductID As String
        Dim pBomInstruction As String

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbX3S252WJX4!")
            End If
            pProductID = ReadyGuid(pProductID)

            pBomInstruction = WebRequest.Instance.Data("BomInstruction")

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProData As ProData = Common.WebRequest.ProData

            pIU = New InsertUpdate("MobizLibraryPIMProduct")
            pIU("BomInstruction") = ReadyString(pBomInstruction)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pProductID))
            pProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
End Class

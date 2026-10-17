Imports Newtonsoft.Json

Public Class StockController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

    Function BsIndex() As ActionResult
        'bootstrap theme
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
        Dim pPickingListMode As Boolean
        Dim pDtWithdrawn As DataTable
        Dim pFowstBinID As String
        Dim pSectionID As String
        Dim pPosMode As String
        Dim pDrInvoice As DataRow
        Dim clsOrganization As New Organization
        Dim pDefaultPriceLevelID As String
        Dim pDefaultPriceLevelIDs As String
        Dim pTransType As String
        Dim pVendorID = WebRequest.Instance.Data("VendorID")

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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

        pPickingListMode = CBool2(Common.WebRequest.Data("PickingListMode"))
        If pPickingListMode Then
            pFowstBinID = WebRequest.Instance.Data("FowstBinID")
            If IsGuid(pFowstBinID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid FowstBinID")
            End If
            pFowstBinID = ReadyGuid(pFowstBinID)

            pSectionID = WebRequest.Instance.Data("SectionID")
            If IsGuid(pSectionID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid SectionID")
            End If
            pSectionID = ReadyGuid(pSectionID)
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
        If pNewCartMode Then
            If Common.WebRequest.Data("TransType") = "PurchaseOrder" Then
                pPosMode = "po"
            End If
        Else
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
                pPosMode = Common.WebRequest.Data("PosMode")
                If pPosMode Is Nothing Then
                    pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
                    If pDrInvoice IsNot Nothing Then
                        pOut("TransType") = CStr2(pDrInvoice("TransType"))
                        If CStr2(pDrInvoice("TransType")) = "AccountsPayable" Then
                            pPosMode = "ap"
                        ElseIf CInt2(pDrInvoice("PurchaseOrderNo")) > 0 _
                            OrElse CStr2(pDrInvoice("TransType")) = "PurchaseOrder" Then

                            pPosMode = "po"
                        End If
                    End If
            End If
            pDtCartStock = WebRequest.Instance.Business.TransInvoice.GetCartStock(pInvoiceID, pProductID, pPosMode)
        End If
        End If

        pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID))
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Non-existing Product ID")
        End If
        pDR = pDT.Rows(0)

        pOut("ProductNo") = pDR("ProductNo")
        pOut("ProductName") = pDR("ProductName")

        pDefaultPriceLevelID = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID
        If IsGuid(pDefaultPriceLevelID) = False Then
            pDefaultPriceLevelID = Nothing
        End If
        pDefaultPriceLevelIDs = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelIDs
        pDtUnits = WebRequest.Instance.Business.LibraryStock.PriceTable(
            WebRequest.Instance.OrgID,
            pProductID,
            pCustomerID,
            , , True, ,
            True, pDefaultPriceLevelID, pDefaultPriceLevelIDs)
        If pDtUnits.Rows.Count = 0 Then
            pOut.Add("Warning", "This product has no price or unit.")
            Return pOut.Serialize
        End If
        If pPosMode = "po" _
            OrElse pPosMode = "ap" Then

            pDtUnits = WebRequest.Instance.Business.LibraryPriceCanvass.ConvertToLandedCost(pCustomerID, pProductID, pDtUnits)
        Else
            pFilter = "PriceLevelID = " & ReadyString(ReadyGuid(WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID))
        End If

        If pDtCartStock IsNot Nothing _
            AndAlso pDtCartStock.Rows.Count > 0 Then

            pOut.Add("Cart", pDtCartStock)

            pDtSalesReps = WebRequest.Instance.Business.TransInvoice.GetSalesReps(pInvoiceID, pProductID)
            pOut.Add("SalesReps", pDtSalesReps)

            pDtProductSerials = WebRequest.Instance.Business.TransInvoice.GetProductSerial(pInvoiceID, pProductID)
            pOut.Add("ProductSerial", pDtProductSerials)

            pDtUnits.Columns.Add("OrderedQty", GetType(Decimal))
            pDtUnits.Columns.Add("TermNumericValue", GetType(Decimal))
            For Each pDrCartStock As DataRow In pDtCartStock.Rows
                Dim pStockFilter As String = "Quantity = " & pDrCartStock("ActualQuantity")
                If Common.WebRequest.Data("TransType") <> "AccountsPayable" Then
                    pStockFilter &= " AND PriceLevelID = " & ReadyString(ReadyGuid(pDrCartStock("PriceLevelID")))
                End If
                pDvUnits = New DataView(pDtUnits, pStockFilter, "", DataViewRowState.CurrentRows)
                If pDvUnits.Count > 0 Then
                    pDrvUnit = pDvUnits(0)
                    pDrvUnit("OrderedQty") = pDrCartStock("Quantity")
                    pDrvUnit("TermNumericValue") = pDrCartStock("TermNumericValue")
                    pDrCartStock("Quantity") -= pDrCartStock("Quantity")
                End If
            Next pDrCartStock
            If CDec2(pDtCartStock.Compute("SUM(Quantity)", "")) <> 0 Then
                Stop2() ' something is wrong
            End If
            If pPosMode <> "po" _
                AndAlso pPosMode <> "ap" Then

                If String.IsNullOrWhiteSpace(pFilter) = False Then
                    pFilter &= " OR "
                End If
                pFilter &= " OrderedQty > 0"
            End If
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
                    AndAlso pDtCartStock.Rows.Count Then

                    If New DataView(pDtUnits, pFilter & " AND OrderedQty > 0", "", DataViewRowState.CurrentRows).Count > 0 Then
                        pFilter &= " AND ISNULL(OrderedQty, 0) <= 0"
                        pDvUnits = New DataView(pDtUnits, pFilter, "", DataViewRowState.CurrentRows)
                        Do Until pDvUnits.Count = 0
                            pDvUnits(0).Delete()
                        Loop
                    Else
                        pDvUnits = New DataView(pDtUnits, pFilter, "", DataViewRowState.CurrentRows)
                        For Each pDRV As DataRowView In pDvUnits
                            If pDvUnits.Count <= 1 Then
                                Exit For ' leave at least 1 unit measure
                            End If
                            pDRV.Delete()
                        Next pDRV
                    End If
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

        If pPickingListMode Then
            pDtWithdrawn = GetWithdrawn(pInvoiceID, pFowstBinID, pSectionID, pProductID)
            pOut.Add("Withdrawn", pDtWithdrawn)
        End If

        pOut.Add("Fowsts", WebRequest.Instance.Business.LibraryOrganizationFOWST.GetByOrgID(WebRequest.Instance.OrgID))
        If IsGuid(pInvoiceID) Then
            pOut.Add("PickingFowsts", WebRequest.Instance.Business.TransInvoice.GetItemFowsts(pInvoiceID, pProductID))
        End If

        Dim pDtPriceBreaks As DataTable
        pDtPriceBreaks = clsOrganization.GetOrganizationProductPriceBreakList(pProductID)
        pOut.Add("PriceBreaks", pDtPriceBreaks)

        If String.IsNullOrWhiteSpace(pVendorID) = False Then
            pDR = WebRequest.Instance.Business.LibraryPriceCanvass.GetVendorPriceInfo(pVendorID, pProductID)
            If pDR IsNot Nothing Then
                pOut.Add("VendorPriceInfo", pDR)
            End If
        End If

        Return pOut.Serialize

    End Function

    Private Function GetWithdrawn(vInvoiceID As String, vFowstBinID As String, vSectionID As String, vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT ProductID" & vbCrLf &
            " ,SUM(PreparedQuantity) AS PreparedQuantity" & vbCrLf &
            " ,ISNULL(UnitQty, 1) AS UnitQty" & vbCrLf &
            " FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND FowstBinID = " & ReadyString(vFowstBinID) & vbCrLf &
            " AND SectionID =" & ReadyString(vSectionID) & vbCrLf &
            " AND ProductID =" & ReadyString(vProductID) & vbCrLf &
            " GROUP BY ProductID" & vbCrLf &
            " ,FowstBinID" & vbCrLf &
            " ,ISNULL(UnitQty, 1)"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

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
        Dim pError As String
        Dim pFowstID As String
        Dim pPickingFowstID As String
        Dim pDontUpdateOrderedQty As Boolean
        Dim pUpdateOrderedQty As Boolean = CBool2(Common.WebRequest.Data("UpdateOrderedQty"))
        Dim pDeviceID As String
        Dim pDefaultPriceLevelID As String
        Dim pDefaultPriceLevelIDs As String
        Dim pUuid As String
        Dim pCommonPersonFowstCount As Integer
        Dim pMsg As String
        Dim pTransType As String = Common.WebRequest.Data("TransType")
        Dim pDrInvoice As DataRow
        Dim pPosMode As String = Common.WebRequest.Data("PosMode")
        Dim pVendorID As String

        Dim pIsVatable As Boolean
        Dim pIsVatExempt As Boolean
        Dim pDrParentInvoice As DataRow
        Dim pParentOrderID As String = Common.WebRequest.Data("ParentOrderID")
        Dim pApproveProfitability As Boolean = CBool2(Common.WebRequest.Data("ApproveProfitability"))
        Dim pQtyField As String = "Quantity" ' Quantity or OrderedQuantity
        Dim pBelowGpmThreshold As Boolean

        pStockID = Common.WebRequest.Data("StockID")
        If CStr2(pStockID).Length = 0 Then
            Return JsonDictionary.Serialize("Error", "No Stock")
        End If
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock")
        End If
        pStockID = ReadyGuid(pStockID)

        If CStr2(pTransType).Trim().ToLower() = "SalesOrder".ToLower() Then
            pPosMode = "so"
        End If
        
        pFowstID = Common.WebRequest.Data("FowstID")
        pPickingFowstID = Common.WebRequest.Data("PickingFowstID")

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleInvoiceCountByMonth(pError) = False Then Return JsonDictionary.Serialize("Error", pError)

        If CBool2(Common.WebRequest.Data("FromEcommerce")) Then
            Dim pDrEcommerceProfile = WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)
            If pDrEcommerceProfile Is Nothing Then
                Throw New ApplicationException("You do not have an e-commerce profile. ref: vbBRQXRKDVJJ!")
            End If
            pCustomerID = CStr2(pDrEcommerceProfile("OrganizationID"))
            pFowstID = WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST

            Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.GetOrCreateDesktop()
            pDeviceID = CStr2(pDrDevice("ID"))
        Else
            pCustomerID = Common.WebRequest.Data("CustomerID")
            If IsGuid(pCustomerID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
            End If
            pCustomerID = ReadyGuid(pCustomerID)
            pUuid = Common.WebRequest.Data("UUID")
            pDeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)

            '// commented on 2020_02_03__15_21_25_5377
            'pCommonPersonFowstCount = WebRequest.Instance.Business.LibraryDevice.CommonPersonFowstCount(WebSession.Instance.PeopleUserID, pUuid)
            'If pCommonPersonFowstCount = 0 Then
            '    pMsg = ""
            '    If String.IsNullOrWhiteSpace(pUuid) Then
            '        pMsg = "Please login using WPA. ref: vbXFH4GC54JW!"
            '    End If
            '    pMsg &= "You and this device must be registered to a common FOWST in the selected organization. ref: vbXFH4GC54JW!"
            '    Return JsonDictionary.Serialize("Error", pMsg)
            'End If

            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
            End If
        End If

        Dim pInvoiceID As String
        Dim pCreator As TransInvoice.Creator
        Dim pDtStocks As DataTable
        Dim pDvStocks As DataView
        Dim pDrvStock As DataRowView
        Dim pNewCartMode As Boolean
        Dim pRetailID As String

        pRetailID = WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID
        If IsGuid(pRetailID) = False Then
            Return JsonDictionary.Serialize("Error", "Retail Price ID was not set in System Defaults")
        End If
        pRetailID = ReadyGuid(pRetailID)

        If Client.CompareLocalTimeAndSqlServerTime(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If
        'vPosMode added by janine sept 8 2014 for sales order taker. fowst is not required
        If WebRequest.Instance.Business.LibraryDevice.IsDesktop(pDeviceID) Then
            WebRequest.Instance.Business.LibraryOrganizationFOWST.AddDevice(pFowstID, pDeviceID)
        End If

        If pTransType = "PurchaseOrder" _
            Or pTransType = "AccountsPayable" Then

            pPosMode = "po"
        End If

        If pPosMode = "po" _
            OrElse pPosMode = "so" Then

            pQtyField = "OrderedQuantity"
        Else
            pQtyField = "Quantity"
        End If

        If WebRequest.Instance.Business.LibraryPeople.Validate(
            vPeopleID:=WebSession.Instance.PeopleUserID,
            vOrgID:=WebRequest.Instance.OrgID,
            vFowstID:=pFowstID,
            vPosMode:=pPosMode,
            vDeviceID:=pDeviceID,
            vDeviceUuid:=Nothing,
            vVerifyUuid:=False,
            vDontVerifyPeopleFowst:=True,
            vDontVerifyOrgEmployee:=CBool2(Common.WebRequest.Data("FromEcommerce")),
            oError:=pError) = False Then

            Return JsonDictionary.Serialize("Error", pError)
        End If

        Common.WebRequest.ProData.BeginTransaction()

        If String.IsNullOrWhiteSpace(pParentOrderID) Then
            pNewCartMode = CDec2(Common.WebRequest.Data("NewCartMode")) <> 0
            If pNewCartMode = False Then
                pInvoiceID = Common.WebRequest.Data("OrderID") ' really InvoiceID
                If IsGuid(pInvoiceID) Then
                    pInvoiceID = ReadyGuid(pInvoiceID)
                    If CBool2(Common.WebRequest.Data("DontCheckCartValidity")) = False Then
                        If WebRequest.Instance.Business.TransInvoice.IsValidCart(pInvoiceID) = False Then
                            pInvoiceID = Nothing
                        End If
                    End If
                Else
                    pInvoiceID = Nothing
                End If
                If pInvoiceID Is Nothing Then
                    pInvoiceID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, pFowstID, CBool2(Common.WebRequest.Data("FromEcommerce")))
                End If
                If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
                    pInvoiceID = ReadyGuid(pInvoiceID)
                    If pPosMode Is Nothing Then
                        pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
                        If pDrInvoice IsNot Nothing Then
                            If CInt2(pDrInvoice("PurchaseOrderNo")) > 0 _
                                OrElse CStr2(pDrInvoice("TransType")) = "PurchaseOrder" Then

                                pPosMode = "po"
                            End If
                        End If
                    End If
                    isNewInvoice = False 'additional by janine sept 9 2014. important do not remove
                End If
            End If
        Else ' If String.IsNullOrWhiteSpace(pParentOrderID) Then
            pInvoiceID = WebRequest.Instance.Business.TransInvoice.GetCartIDByParentOrderID(pParentOrderID, pFowstID)
            isNewInvoice = String.IsNullOrWhiteSpace(pInvoiceID)
            If isNewInvoice Then
                pDrParentInvoice = WebRequest.Instance.Business.TransInvoice.ById(pParentOrderID)
                If pDrParentInvoice IsNot Nothing Then
                    If ReadyGuid(pDrParentInvoice("FowstID")) = ReadyGuid(pFowstID) Then
                        pInvoiceID = pParentOrderID
                        isNewInvoice = False
                        pParentOrderID = Nothing
                    End If
                End If
            End If
        End If ' END If String.IsNullOrWhiteSpace(pParentOrderID) Then

        If pPosMode = "po" Then
            pDefaultPriceLevelID = WebRequest.Instance.Business.SystemDefaults.GetLandedCostID()
        Else
            pDefaultPriceLevelID = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID
        End If
        If IsGuid(pDefaultPriceLevelID) = False Then
            pDefaultPriceLevelID = Nothing
        End If
        pDefaultPriceLevelIDs = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelIDs
        pDtStocks = WebRequest.Instance.Business.LibraryStock.PriceTable(
            WebRequest.Instance.OrgID,
            pStockID,
            pCustomerID, , , , , True, pDefaultPriceLevelID, pDefaultPriceLevelIDs)
        If pPosMode = "po" Then
            pDtStocks = WebRequest.Instance.Business.LibraryPriceCanvass.ConvertToLandedCost(pCustomerID, pStockID, pDtStocks)
        End If
        ' do not verify at the server. 2017_03_09__10_54_14_2155
        'For Each pStock As Object In Common.WebRequest.AllData("Stocks")
        '    If pStock Is Nothing Then Continue For
        '    pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")) &
        '        " AND PriceLevelID = " & ReadyString(pStock("PriceLevelID")), "", DataViewRowState.CurrentRows)
        '    If pDvStocks.Count > 0 Then
        '        pDrvStock = pDvStocks(0)
        '        If CDec2(pStock("Qty")) >= 0 Then
        '            If Math.Round(pDrvStock("DiscountedPrice"), 2) < _
        '                Math.Round( _
        '                        WebRequest.Instance.Business.LibraryStock.GetLandedCost(pStockID, WebRequest.Instance.OrgID) * CDec2(pStock("UnitQty")) _
        '                , 2) Then

        '                ' tanggal muna trapping. march 14, 2014
        '                'Common.WebRequest.ProData.RollbackTransaction()
        '                'Return JsonDictionary.Serialize("Error", "Selling price must be greater than or equal to the landed cost. To adjust prices, please go to Home -> System Administration -> Data Libraries -> PIM -> Adjust Product Prices")
        '            End If
        '        End If
        '    Else
        '        Common.WebRequest.ProData.RollbackTransaction()
        '        Return JsonDictionary.Serialize("Error", "No matching unit quantity and price level")
        '    End If
        'Next pStock

        pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator
        pCreator.ServedQtyIsEdited = CBool2(WebRequest.Instance.Data("ServedQtyIsEdited"))
        Dim pDtOrderedQty As DataTable

        If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
            pDtOrderedQty = WebRequest.Instance.Business.TransInvoice.GetOrderedQty(pInvoiceID, pStockID)
        End If
        If CBool2(Common.WebRequest.Data("DeleteExistingProduct")) _
            AndAlso CStr2(pInvoiceID).Length > 0 Then

            pCreator.ID = pInvoiceID
            WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pInvoiceID, pStockID, "")
        End If

        pCreator.InvoiceNumber = 0
        pCreator.InvoiceDate = Nothing ' nothing to use GETDATE() from SQL Server
        pCreator.CustomerID = pCustomerID
        pCreator.TransType = pTransType
        pCreator.DontSaveTransType = CBool2(WebRequest.Instance.Data("DontSaveTransType"))
        pCreator.ParentOrderID = pParentOrderID

        If CBool2(WebRequest.Instance.Data("ForceUpdateOrderedQty")) = False Then
            If pTransType = "Invoice" _
                OrElse pTransType = "AccountsPayable" Then

                pDontUpdateOrderedQty = True
            End If
        End If

        'additional by janine may 26 2015
        If CStr2(Common.WebRequest.Data("DeliveryAddressID")).Length > 0 Then
            pCreator.DeliveryAddressID = Common.WebRequest.Data("DeliveryAddressID")
        ElseIf pInvoiceID Is Nothing _
            AndAlso String.IsNullOrWhiteSpace(pParentOrderID) = False _
            AndAlso pDrParentInvoice IsNot Nothing Then

            pCreator.DeliveryAddressID = CStr2(pDrParentInvoice("DeliveryAddressID"))
        ElseIf pInvoiceID Is Nothing Then
            pCreator.DeliveryAddressID = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
        End If
        If CStr2(Common.WebRequest.Data("BillingAddressID")).Length > 0 Then
            pCreator.BillingAddressID = Common.WebRequest.Data("BillingAddressID")
        ElseIf pInvoiceID Is Nothing _
            AndAlso String.IsNullOrWhiteSpace(pParentOrderID) = False _
            AndAlso pDrParentInvoice IsNot Nothing Then

            pCreator.BillingAddressID = CStr2(pDrParentInvoice("BillingAddressID"))
        ElseIf pInvoiceID Is Nothing Then
            pCreator.BillingAddressID = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Billing Address ID")
        End If
        If CStr2(Common.WebRequest.Data("DeliveryDate")).Length > 0 Then pCreator.SODeliveryDate = Common.WebRequest.Data("DeliveryDate")

        If pInvoiceID Is Nothing _
            AndAlso String.IsNullOrWhiteSpace(pParentOrderID) = False _
            AndAlso pDrParentInvoice IsNot Nothing Then

            pCreator.DateNeeded = CDateNull(pDrParentInvoice("DateNeeded"))
        End If
        'end of additional

        pCreator.UserID = WebSession.Instance.PeopleUserID

        pCreator.ProductID = pStockID
        pCreator.SalesRepList = Common.WebRequest.AllData("SalesRep")
        pCreator.ProductSerialList = Common.WebRequest.AllData("ProductSerial")
        pCreator.IsPos = CBool2(Common.WebRequest.Data("IsPos"))

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
        Dim pPriceLevelID As String

        pDtStock = TransInvoice.CreateStockTable()
        For Each pStock As Object In Common.WebRequest.AllData("Stocks")
            If pStock Is Nothing Then Continue For
            Try
                pPriceLevelID = pStock("PriceLevelID")
            Catch ex As Exception
                pPriceLevelID = pDefaultPriceLevelID
            End Try
            pPriceLevelID = ReadyGuid(pPriceLevelID)
            pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")) &
                " AND PriceLevelID = " & ReadyString(pPriceLevelID), "", DataViewRowState.CurrentRows)

            If pDvStocks.Count = 0 _
                AndAlso (
                    pStock.ContainsKey("EditPrice") _
                    AndAlso CDec2(pStock("EditPrice")) > 0 _
                    OrElse pTransType = "PurchaseOrder"
                ) Then

                pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
            End If
            If pDvStocks.Count > 0 Then
                If pStock.ContainsKey("VendorID") Then
                    pVendorID = pStock("VendorID")
                    If IsGuid(pVendorID) = False Then
                        pVendorID = Nothing
                    End If
                Else
                    pVendorID = Nothing
                End If

                pDrvStock = pDvStocks(0)
                If CDec2(pStock("Qty")) >= 0 Then
                    pDrStock = pDtStock.NewRow
                    pDrStock("StockID") = pStockID
                    'additional by janine sept 3 2014 for order taker
                    If Not isNewInvoice Then 'if existing invoice then delete the invoiceitems that match. additional by janine sept 9 2014  Then

                        'orderedQty = WebRequest.Instance.Business.TransInvoice.GetOrderedQty(pInvoiceID, pStockID, ) ' old code 2018_08_18__11_58_24_7013
                        If pDtOrderedQty IsNot Nothing Then
                            orderedQty = New DataView(pDtOrderedQty, "UnitMeasure = " & ReadyString(pDrvStock("UnitMeasure")), "", DataViewRowState.CurrentRows).ToTable() ' new code 2018_08_18__11_58_24_7013
                        End If

                        pCreator.ID = pInvoiceID
                        ' WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pInvoiceID, pStockID, pDrvStock("UnitMeasure")) ' old code 2016_10_18__17_15_41_1596

                        WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pInvoiceID, pStockID, pStock("UnitQty"), , pVendorID) ' new code 2016_10_18__17_15_41_1596
                    End If
                    pDrStock("VendorID") = pVendorID
                    If (pPosMode = "so") Then
                        pDrStock("Quantity") = 0 'need to save it as 0 because it's not yet invoiced!
                        If pDontUpdateOrderedQty = False Then
                            pDrStock("OrderedQuantity") = CDec2(pStock("Qty"))
                        End If
                    Else
                        If pTransType = "SalesOrder" Then
                            pDrStock("Quantity") = 0
                        ElseIf pTransType = "AccountsPayable" Then
                            pDrStock("Quantity") = CDec2(pStock("Qty"))
                            pDrStock("OrderedQuantity") = CDec2(pStock("Qty"))
                        ElseIf pTransType <> "PurchaseOrder" Then
                            pDrStock("Quantity") = CDec2(pStock("Qty"))
                        End If
                        If pUpdateOrderedQty Then
                            pDrStock("OrderedQuantity") = CDec2(pStock("Qty")) 'save ordered qty same as invoiced qty for POS. so ordered = invoiced
                        ElseIf (orderedQty Is Nothing) Then
                            If pDontUpdateOrderedQty = False Then
                                pDrStock("OrderedQuantity") = CDec2(pStock("Qty")) 'save ordered qty same as invoiced qty for POS. so ordered = invoiced
                            End If
                        ElseIf pDontUpdateOrderedQty = False Then
                            If (orderedQty.Rows.Count > 0) Then
                                pDrStock("OrderedQuantity") = orderedQty.Rows(0).Item("OrderedQuantity")
                            Else
                                pDrStock("OrderedQuantity") = CDec2(pStock("Qty")) 'save ordered qty same as invoiced qty for POS. so ordered = invoiced
                            End If
                        End If
                    End If
                    If CDec2(pDrStock("OrderedQuantity")) <= 0 _
                            AndAlso CDec2(pDrStock("Quantity")) <= 0 Then

                        Continue For
                    End If

                    'end of additional
                    pDrStock("ActualQuantity") = pDrvStock("Quantity")
                    pDrStock("UnitMeasure") = pDrvStock("UnitMeasure")
                    Dim pCost As Decimal = WebRequest.Instance.Business.LibraryStock.GetLandedCost(pStockID, WebRequest.Instance.OrgID) * pDrStock("ActualQuantity")
                    pDrStock("Cost") = pCost
                    If pStock.ContainsKey("EditPrice") _
                        AndAlso 0 < CDec2(pStock("EditPrice")) Then

                        pDrStock("UnitPrice") = Math.Round(CDec2(pStock("EditPrice")), 4)
                    Else
                        pDrStock("UnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 4)
                    End If
                    If pStock.ContainsKey("OriginalUnitPrice") Then
                        pDrStock("OriginalUnitPrice") = CDec2(pStock("OriginalUnitPrice"))
                    End If

                    Dim pGpm As Decimal = 0
                    If pDrStock("UnitPrice") <> 0 Then
                        Dim pUnitPriceSurchargePercent = CDec2(Common.WebRequest.Data("UnitPriceSurchargePercent"))
                        If pUnitPriceSurchargePercent <> 0 Then
                            pDrStock("UnitPrice") = pDrStock("UnitPrice") * (1 + pUnitPriceSurchargePercent / 100)
                        End If
                        pGpm = (pDrStock("UnitPrice") - pCost) / pDrStock("UnitPrice") * 100
                    End If
                    Dim pMinGpm As Decimal
                    Dim pCategory As String = WebRequest.Instance.Business.LibraryStock.GetCategory(pStockID)

                    If String.IsNullOrWhiteSpace(pCategory) Then
                        pMinGpm = WebRequest.Instance.Business.SystemDefaults.FreeTypeAPriceMinimumGpm
                    Else
                        Dim pMinGpmByCategory As Decimal? = WebRequest.Instance.Business.LibraryStock.GetMinGpmByCategory(pCategory)
                        If pMinGpmByCategory Is Nothing Then
                            pMinGpm = WebRequest.Instance.Business.SystemDefaults.FreeTypeAPriceMinimumGpm
                        Else
                            pMinGpm = pMinGpmByCategory
                        End If
                    End If
                    If pGpm < pMinGpm Then
                        If pPosMode <> "po" _
                            AndAlso pApproveProfitability = False Then

                            pBelowGpmThreshold = True
                            'If pTransType = "Invoice" Then
                            '    Return JsonDictionary.Serialize("Error", "At least 1 item has a line GPM of " & Format(pGpm, "#,##0.00") & "%. The minimum required line GPM is " & Format(pMinGpm, "#,##0.00") & "%.")
                            'End If
                        End If
                    End If
                    If ReadyGuid(pDrvStock("PriceLevelID")) <> pRetailID Then
                        'todo: add security
                        'If WebSession.Instance.UserAccess.PointOfSales_PriceLevelMode.IsEnable _
                        '    OrElse WebSession.Instance.UserAccess.PointOfSales_UnlimitedUserMode.IsEnable Then

                        'Else
                        '    Common.WebRequest.ProData.RollbackTransaction()
                        '    Return JsonDictionary.Serialize("Error", "You are not authorized to set price level other than retail")
                        'End If
                    End If

                    If pStock.ContainsKey("TradeDiscountLess") Then
                        pDrStock("TradeDiscountLess") = pStock("TradeDiscountLess")
                        pDrStock("NetDiscountFactor") = pStock("NetDiscountFactor")
                        pDrStock("NetDiscountRate") = pStock("NetDiscountRate")
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
                    If (pPosMode = "so") Then
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
                       vQty:=pDrStock(pQtyField),
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

                    If CBool2(Common.WebRequest.Data("FromEcommerce")) Then
                        Dim upcharge As Integer = 0
                        Dim pUpcharge = Common.WebRequest.ProData.Execute("SELECT Upcharge FROM MobizLibraryPIMProductParent WHERE ProductID = " & ReadyString(pDrStock("StockID"))).Rows
                        'Upcharge Formula: Selling Price x (100% + Upcharge %)

                        If pUpcharge.Count > 0 Then
                            upcharge = CDec2(pUpcharge(0)("Upcharge"))
                        End If

                        'iapply lang ang upcharge kapag baba sa Base Packing Size Qty or 1
                        If CDec2(pDrStock(pQtyField)) < 1 Then
                            pDrStock("Upcharge") = upcharge
                            pDrStock("Subtotal") = (pDrStock(pQtyField) * pDrStock("UnitPrice")) * ((100 + upcharge) / 100)
                        Else
                            pDrStock("Subtotal") = pDrStock(pQtyField) * pDrStock("UnitPrice")
                        End If
                    Else
                        'additional by janine sept 5 for sales order taker
                        pDrStock("Subtotal") = pDrStock(pQtyField) * pDrStock("UnitPrice")
                    End If

                    pDrStock("TaxPercent") = WebRequest.Instance.Business.SystemDefaults.VatPercent
                    'pDrStock("TaxSales") = RoundDown(pDrStock("SubTotal") / (1 + pDrStock("TaxPercent") / 100), 4)
                    'pDrStock("TaxAmount") = pDrStock("SubTotal") - pDrStock("TaxSales")

                    pIsVatable = WebRequest.Instance.Business.SystemDefaults.Vatable
                    pIsVatExempt = CBool2(Common.WebRequest.Data("IsVATExempt"))
                    If Not pIsVatable OrElse pIsVatExempt Then
                        pDrStock("VATableSale") = 0
                        If pIsVatExempt Then
                            pDrStock("VATExemptSale") = pDrStock("SubTotal")
                            pDrStock("NonVATSale") = 0
                        Else
                            pDrStock("VATExemptSale") = 0
                            pDrStock("NonVATSale") = pDrStock("SubTotal")
                        End If
                        pDrStock("ZeroRatedSale") = 0
                        pDrStock("TaxSales") = 0
                        pDrStock("TaxAmount") = 0
                    Else
                        pDrStock("VATableSale") = pDrStock("SubTotal")
                        pDrStock("VATExemptSale") = 0
                        pDrStock("NonVATSale") = 0
                        pDrStock("ZeroRatedSale") = 0
                        pDrStock("TaxSales") = RoundDown(pDrStock("SubTotal") / (1 + pDrStock("TaxPercent") / 100), 4)
                        pDrStock("TaxAmount") = pDrStock("SubTotal") - pDrStock("TaxSales")

                    End If

                    If CStr2(pInvoiceID).Length > 0 Then
                        pDrStock("LineNumber") = Math.Max(0, WebRequest.Instance.Business.TransInvoice.GetMaxLineNumber(pInvoiceID)) + 1
                    Else
                        pDrStock("LineNumber") = 1
                    End If
                    '' ''If pStock.containskey("SalesRep") Then
                    '' ''    pDrStock("SalesRep") = pStock("SalesRep")
                    '' ''End If
                    If pStock.ContainsKey("PaymentTerm") Then
                        pDrStock("TermNumericValue") = pStock("PaymentTerm")
                    End If
                    pDtStock.Rows.Add(pDrStock)
                Else
                    Return JsonDictionary.Serialize("Error", "No matching unit quantity and price level")
                End If
            Else
                Return JsonDictionary.Serialize("Error", "That product cannot be committed to cart because that product does not have a packing size (unit measure) quantity of 1. ref: vb9NYR53665G!")
            End If
        Next pStock

        pCreator.Stocks = pDtStock

        pOut = New JsonDictionary
        pOut.Add("BelowGpmThreshold", pBelowGpmThreshold)
        If CStr2(pInvoiceID).Length > 0 Then
            pCreator.ID = pInvoiceID
            If pCreator.Save(False, pError) Then
                If pTransType = "Invoice" Then
                    WebRequest.Instance.Business.TransInvoice.UpdateOrderQtyFromOrderToInvoice(pInvoiceID, pStockID)
                End If

                ' START: copy paste code 2020_02_26__10_32_26_4631
                Dim pItemsTotal As Decimal = WebRequest.Instance.Business.TransInvoice.GetTotalSubTotal(pInvoiceID, pQtyField)
                pCreator.CnCUnapprovedReason = WebRequest.Instance.Business.LibraryCustomers.ComputeCnCUnapprovedReason(pCustomerID, pItemsTotal)
                WebRequest.Instance.Business.TransInvoice.SetCnCUnapprovedReason(pInvoiceID, pCreator.CnCUnapprovedReason)
                WebRequest.Instance.Business.TransInvoice.UpdateItemPaymentTerm(pInvoiceID)
                WebRequest.Instance.Business.TransInvoice.UpdateLineNumbers(pInvoiceID)
                ' END: copy paste code 2020_02_26__10_32_26_4631

                If pApproveProfitability Then
                    Dim pDtItems = WebRequest.Instance.Business.TransInvoice.UpdateProfitabilityItemIDs(pInvoiceID, pStockID)
                    For Each pDrItem As DataRow In pDtItems.Rows
                        WebRequest.Instance.Business.TransInvoice.ApproveProfitability(CStr2(pDrItem("ID")), WebSession.Instance.PeopleUserID, False)
                    Next
                    'Else
                    '    WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pInvoiceID, WebRequest.Instance.OrgID)
                End If
                If pPosMode = "po" Then
                    WebRequest.Instance.Business.TransInvoice.SaveVendorPriceHistory(pInvoiceID, pStockID)
                End If
                Common.WebRequest.ProData.CommitTransaction()
                pOut.Add("Result", "ok")
                pOut.Add("OrderID", pCreator.ID)
                pOut.Add("SavedItemsCount", pDtStock.Rows.Count)
                pOut.Add("CnCUnapprovedReason", pCreator.CnCUnapprovedReason)

                If Not (pDrvStock Is Nothing) Then
                    pOut.Add("DiscountedPrice", Math.Round(pDrvStock("DiscountedPrice"), 2)) 'additional by janine. for hot buttons i need to get the price
                End If
                If pPickingFowstID = "any" OrElse IsGuid(pPickingFowstID) Then
                    WebRequest.Instance.Business.TransInvoice.SaveItemFowst(pInvoiceID, pStockID, pPickingFowstID)
                End If

                Return pOut.Serialize
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

            If jss.DeserializeObject(Common.WebRequest.InputStreamText).ContainsKey("SerialNumbers") Then
                For Each n As Object In Common.WebRequest.AllData("SerialNumbers")
                    Dim p As New InsertUpdate("TransInvoiceItemSerials")
                Next n
            End If
        Else
            pCreator.FowstID = pFowstID
            pCreator.DeviceID = pDeviceID
            pCreator.EncodingFowstID = Common.WebRequest.Data("EncodingFowstID")
            If pCreator.Save(True, pError) Then
                pInvoiceID = pCreator.ID
                ' START: copy paste code 2020_02_26__10_32_26_4631
                Dim pItemsTotal As Decimal = WebRequest.Instance.Business.TransInvoice.GetTotalSubTotal(pInvoiceID, pQtyField)
                pCreator.CnCUnapprovedReason = WebRequest.Instance.Business.LibraryCustomers.ComputeCnCUnapprovedReason(pCustomerID, pItemsTotal)
                WebRequest.Instance.Business.TransInvoice.SetCnCUnapprovedReason(pInvoiceID, pCreator.CnCUnapprovedReason)
                WebRequest.Instance.Business.TransInvoice.UpdateItemPaymentTerm(pInvoiceID)
                WebRequest.Instance.Business.TransInvoice.UpdateLineNumbers(pInvoiceID)
                ' END: copy paste code 2020_02_26__10_32_26_4631

                'WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pInvoiceID, WebRequest.Instance.OrgID)
                If pPosMode = "po" Then
                    WebRequest.Instance.Business.TransInvoice.SaveVendorPriceHistory(pInvoiceID)
                End If
                Common.WebRequest.ProData.CommitTransaction()
                pOut.Add("Result", "ok")
                pOut.Add("OrderID", pCreator.ID)
                pOut.Add("SavedItemsCount", pDtStock.Rows.Count)
                pOut.Add("CnCUnapprovedReason", pCreator.CnCUnapprovedReason)

                If Not (pDrvStock Is Nothing) Then
                    pOut.Add("DiscountedPrice", Math.Round(pDrvStock("DiscountedPrice"), 2)) 'additional by janine. for hot buttons i need to get the price
                End If
                If pPickingFowstID = "any" OrElse IsGuid(pPickingFowstID) Then
                    WebRequest.Instance.Business.TransInvoice.SaveItemFowst(pCreator.ID, pStockID, pPickingFowstID)
                End If
                Return pOut.Serialize
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If
        End If

    End Function

    Public Class UpdatePosLiteInput

        Public OrderID As String
        Public ProductID As String
        Public EditMode As String
        Public SalesRep As Object
        Public Stocks As StocksClass

        Public Class StocksClass
            Inherits List(Of Stock)
        End Class

        Public Class Stock
            Public Qty As String ' integer
            Public UnitQty As String ' integer
            Public UnitMeasureID As String
            Public UnitMeasure As String
            Public PriceLevelID As String
            Public EditPrice As String ' decimal
            Public ItemModifier As IEnumerable(Of ItemModifierClass)
        End Class

        Public Class ItemModifierClass
            Public ID As String
            Public SelectedModifier As IEnumerable(Of SelectedModifierClass)
        End Class

        Public Class SelectedModifierClass
            Public ID As String
            Public OrderQty? As Decimal
            Public InputQty? As Decimal
            Public UnitPrice? As Decimal
            Public PremiumPrice? As Decimal
            Public ModifierPrice? As Decimal
            Public PremiumPriceApplied As String ' bool
            Public Others As String ' OtherDescription
        End Class

        Public Function Verify(ByRef oError As String) As Boolean

            If Stocks Is Nothing Then
                Stocks = New StocksClass
            End If

            For Each pStock In Stocks
                If String.IsNullOrWhiteSpace(pStock.Qty) Then
                    oError = "Qty is required. vbTRUNJSSNSE!"
                    Return False
                End If
                If IsNumeric(pStock.Qty) = False Then
                    oError = "Qty must be numeric. vbGQQT7BL5JL!"
                    Return False
                End If
            Next

            Return True
        End Function
    End Class

    <HttpPost>
    Public Function UpdatePosLite() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pError As String
        Dim pInput = JsonConvert.DeserializeObject(Of UpdatePosLiteInput)(Common.WebRequest.InputStreamText)
        If pInput.Verify(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If
        WebRequest.Instance.ProData.BeginTransaction()
        Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInput.OrderID)
        If pDrInvoice Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Invoice does not exist. vbJUQCB5BKK5!")
        End If
        If CInt2(pDrInvoice("InvoiceNumber")) <> 0 Then
            Return JsonDictionary.Serialize("Error", "Invoice Number " & pDrInvoice("InvoiceNumber") & " was already finalized. vbK46Q45QTF8!")
        End If

        For Each pStock In pInput.Stocks
            Dim pID As String = ""
            Dim pIU As New InsertUpdate("TransInvoiceItems")
            Dim pDtCart = WebRequest.Instance.Business.TransInvoice.GetItemByUnitAndInvoiceID(
                vInvoiceID:=pInput.OrderID,
                vProductID:=pInput.ProductID,
                vUnitMeasure:=pStock.UnitMeasure)
            If pDtCart.Rows.Count > 0 Then
                pID = CStr2(pDtCart.Rows(0)("ID"))
                If CBool2(pInput.EditMode) Then
                    pStock.Qty = CDec2(pStock.Qty)
                Else
                    pStock.Qty = CDec2(pStock.Qty) + CDec2(pDtCart.Rows(0)("Quantity"))
                End If
            Else
                pID = Guid.NewGuid.ToString()
                pIU("ID") = ReadyString(pID)
                pIU("InvoiceID") = ReadyString(pInput.OrderID)
                pIU("ProductID") = ReadyString(pInput.ProductID)
                pIU("UnitMeasure") = ReadyString(pStock.UnitMeasure)
            End If
            If CDec2(pStock.UnitQty) <= 0 Then
                WebRequest.Instance.ProData.RollbackTransaction(True, True)
                Return JsonDictionary.Serialize("Error", "Unit Qty must be greater than zero. Please search for the product again then enter the quantity.")
            End If
            pIU("ActualQuantity") = CDec2(pStock.UnitQty)
            pIU("Quantity") = CDec2(pStock.Qty)
            If CDec2(pStock.EditPrice) <= 0 Then
                WebRequest.Instance.ProData.RollbackTransaction(True, True)
                Return JsonDictionary.Serialize("Error", "Unit Price must be greater than zero. Please search for the product again then enter the quantity.")
            End If
            pIU("UnitPrice") = CDec2(pStock.EditPrice)
            If String.IsNullOrWhiteSpace(pStock.PriceLevelID) Then
                pStock.PriceLevelID = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID
            End If
            pIU("PriceLevelID") = ReadyString(pStock.PriceLevelID)

            Dim pTotalModifier As Decimal = 0
            If pStock.ItemModifier IsNot Nothing Then
                For Each pItemModifier In pStock.ItemModifier
                    pTotalModifier += UpdateItemModifier(pID, CDec2(pStock.Qty), pItemModifier)
                Next
            End If

            pIU("SubTotal") = CDec2(pStock.EditPrice) * CDec2(pStock.Qty) + pTotalModifier
            pIU("TaxPercent") = WebRequest.Instance.Business.SystemDefaults.VatPercent
            'pDrStock("TaxSales") = RoundDown(pDrStock("SubTotal") / (1 + pDrStock("TaxPercent") / 100), 4)
            'pDrStock("TaxAmount") = pDrStock("SubTotal") - pDrStock("TaxSales")

            Dim pIsVatable = WebRequest.Instance.Business.SystemDefaults.Vatable
            Dim pIsVatExempt = CBool2(Common.WebRequest.Data("IsVATExempt"))
            If Not pIsVatable OrElse pIsVatExempt Then
                pIU("VATableSale") = 0
                If pIsVatExempt Then
                    pIU("VATExemptSale") = pIU("SubTotal")
                    pIU("NonVATSale") = 0
                Else
                    pIU("VATExemptSale") = 0
                    pIU("NonVATSale") = pIU("SubTotal")
                End If
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = 0
                pIU("TaxAmount") = 0
            Else
                pIU("VATableSale") = pIU("SubTotal")
                pIU("VATExemptSale") = 0
                pIU("NonVATSale") = 0
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = RoundDown(pIU("SubTotal") / (1 + pIU("TaxPercent") / 100), 4)
                pIU("TaxAmount") = pIU("SubTotal") - pIU("TaxSales")

            End If

            If CStr2(pInput.OrderID).Length > 0 Then
                pIU("LineNumber") = Math.Max(0, WebRequest.Instance.Business.TransInvoice.GetMaxLineNumber(pInput.OrderID)) + 1
            Else
                pIU("LineNumber") = 1
            End If
            Dim pSql As String
            pSql = pIU.GetUpdate("ID = " & ReadyString(pID))

            If WebRequest.Instance.ProData.ExecuteSQL(pSql) = 0 Then
                pSql = pIU.GetInsert()
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
            End If

            WebRequest.Instance.ProData.ExecuteSQL("DELETE FROM TransInvoiceItemSalesReps WHERE InvoiceItemID = " & ReadyString(pID) &
                                                       " AND ProductID = " & ReadyString(pInput.ProductID) &
                                                       " AND InvoiceID = " & ReadyString(pInput.OrderID))
                For Each pSalesRep In pInput.SalesRep
                    Dim peopleid As String

                    If pSalesRep("PeopleID") = Nothing Then
                        peopleid = pSalesRep("ID").ToString()
                    Else
                        peopleid = pSalesRep("PeopleID").ToString()
                    End If

                    pIU = New InsertUpdate("TransInvoiceItemSalesReps")
                    pIU("ID") = "NEWID()"
                    pIU("InvoiceItemID") = ReadyString(pID)
                    pIU("PeopleID") = ReadyString(peopleid)
                    pIU("ProductID") = ReadyString(pInput.ProductID)
                    pIU("InvoiceID") = ReadyString(pInput.OrderID)
                    pSql = pIU.GetInsert()
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                Next
            Next
            WebRequest.Instance.Business.TransInvoice.UpdateLineNumbers(pInput.OrderID)

        WebRequest.Instance.ProData.CommitTransaction()

        Dim pOut As New JsonDictionary()
        pOut.Add("Result", "ok")
        pOut.Add("OrderID", pInput.OrderID)

        Return pOut.Serialize()

    End Function

    ''' <summary>
    ''' return total modifier amount for that TransInvoiceItems row
    ''' </summary>
    ''' <param name="vItemID"></param>
    ''' <param name="vProductQty"></param>
    ''' <param name="vItemModifier"></param>
    ''' <returns></returns>
    Function UpdateItemModifier(vItemID As String, vProductQty As Decimal, vItemModifier As UpdatePosLiteInput.ItemModifierClass) As Decimal

        Dim pOut As Decimal

        If vItemModifier.SelectedModifier Is Nothing Then
            Return 0
        End If
        Dim pExcludeIds As New List(Of String)
        For Each pItem In vItemModifier.SelectedModifier
            If IsGuid(pItem.ID) = False Then
                Throw New ApplicationException("SelectedModifier.ID must be a GUID. ref: vb9RDAYWSBU6!")
            End If
            pExcludeIds.Add(ReadyString(pItem.ID))
        Next
        Dim pSql As String
        Dim pIU As InsertUpdate
        If pExcludeIds.Count > 0 Then
            pSql = "DELETE FROM TransInvoiceItemsModifier" & vbCrLf &
                " WHERE InvoiceItemID = " & ReadyString(vItemID) & vbCrLf &
                " AND ItemModifierID = " & ReadyString(vItemModifier.ID) & vbCrLf &
                " AND ModifierID Not IN (" & String.Join(", ", pExcludeIds) & ")"
            pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql)
        End If
        For Each pItem In vItemModifier.SelectedModifier
            pIU = New InsertUpdate("TransInvoiceItemsModifier")
            pIU("InvoiceItemID") = ReadyString(vItemID)
            pIU("ItemModifierID") = ReadyString(vItemModifier.ID)
            pIU("ModifierID") = ReadyString(pItem.ID)
            pIU("OrderedQty") = ReadyString(pItem.OrderQty)
            pIU("InputQty") = ReadyString(pItem.InputQty)
            pIU("ModifierPrice") = CDec2(pItem.UnitPrice)
            pIU("PremiumPrice") = CDec2(pItem.PremiumPrice)
            Dim pSubTotal = CDec2(pItem.OrderQty) * CDec2(pItem.UnitPrice) + vProductQty * CDec2(pItem.PremiumPrice)
            pIU("SubTotal") = pSubTotal
            pOut += pSubTotal
            pIU("OtherDescription") = ReadyString(pItem.Others)
            pSql = pIU.GetUpdate("InvoiceItemID = " & ReadyString(vItemID) & vbCrLf &
                " AND ModifierID = " & ReadyString(pItem.ID))
            Dim pCount = WebRequest.Instance.ProData.ExecuteSQL(pSql)
            If pCount = 0 Then
                pIU("ID") = "NEWID()"
                pSql = pIU.GetInsert()
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
            End If
        Next

        Return pOut

    End Function

    <HttpPost()>
    Public Function AlertNoPrice()

        Dim pProductID As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
    Public Function GetRecursiveContents()

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIssues = WebRequest.Instance.Business.LibraryStock.GetRecursiveContents(WebRequest.Instance.OrgID)
            If String.IsNullOrWhiteSpace(pIssues) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Issues", Truncate(pIssues, 5000))
            End If
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function PriceTable()

        Dim pProductID As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pProductID = Common.WebRequest.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid ProductID")
            End If
            pProductID = ReadyGuid(pProductID)
            Dim pCustomerID = Common.WebRequest.Data("CustomerID")
            Dim pDefaultPriceLevelID = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID
            If IsGuid(pDefaultPriceLevelID) = False Then
                pDefaultPriceLevelID = Nothing
            End If
            Dim pDefaultPriceLevelIDs = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelIDs
            Dim pDT = WebRequest.Instance.Business.LibraryStock.PriceTable(WebRequest.Instance.OrgID, pProductID, pCustomerID, , , , , True, pDefaultPriceLevelID, pDefaultPriceLevelIDs)

            Return JsonDictionary.Serialize(pDT)
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function PurchaseOrderInTransit()

        Dim pProductID As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pProductID = Common.WebRequest.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid ProductID")
            End If
            pProductID = ReadyGuid(pProductID)
            Dim pIncludeReceived = CBool2(Common.WebRequest.Data("IncludeReceived"))
            Dim pDT = WebRequest.Instance.Business.LibraryStock.PurchaseOrderInTransit(pProductID, pIncludeReceived)

            Return JsonDictionary.Serialize(pDT)
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function GetInventoryBalance() As String

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim pInvoiceID As String = ReadyGuid(Common.WebRequest.Data("InvoiceID"))

        If Now.Date = New Date(2020, 2, 22) Then
            Throw New ApplicationException("this is a simulated error")
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        WebRequest.Instance.Business.TransInventoryLedger.CreateTempTables()

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True
            With .Fields
                With .Add("FowstID", SqlDbType.VarChar)
                    .Expression = "t.FOWSTID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "t.FOWST"
                End With
                With .Add("Balance", SqlDbType.Float)
                    .Expression = "SUM(t.balance)"
                End With
                With .Add("CommittedQty", SqlDbType.Float)
                    .Expression = "ISNULL(t.CommittedQty, 0)"
                End With
                With .Add("AvaiableToCommit", SqlDbType.Float)
                    .Expression = "SUM(ISNULL(t.balance, 0)) - ISNULL(t.CommittedQty, 0)"
                End With
                With .Add("AssignedQty", SqlDbType.Decimal)
                End With
                With .Add("DeliveryMethod", SqlDbType.VarChar)
                End With
            End With

            .SqlFrom = "  FROM (SELECT DISTINCT" & vbCrLf &
                " dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName," & vbCrLf &
                " dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID AS Account_ID," & vbCrLf &
                " dbo.MobizLibraryOrganizationFOWST.Name AS Store_ID" & vbCrLf &
                " ,bal1.Balance" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.ID AS FowstID" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.Name AS FOWST" & vbCrLf &
                " ,tblAvailable.Quantity AS CommittedQty" & vbCrLf &
                " ,tblOrdered.Quantity AS AssignedQty" & vbCrLf &
                " ,tblOrdered.DeliveryMethod" & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " LEFT JOIN dbo.TransInventoryLedger AS t0 ON t0.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                " AND t0.ProductID = " & ReadyString(Common.WebRequest.Data("StockID")) & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = t0.ProductID" & vbCrLf &
                " LEFT JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = t0.ProductID" & vbCrLf &
                " OUTER APPLY(" & vbCrLf &
                " select SUM(Balance) AS Balance" & vbCrLf &
                " from #t2" & vbCrLf &
                " where #t2.ProductID = t0.ProductID" & vbCrLf &
                " And #t2.FowstID = t0.FowstID" & vbCrLf &
                " )  bal1" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                "  SELECT t1.FOWSTID, SUM(Quantity) Quantity FROM TransInvoice t1" & vbCrLf &
                " inner join TransInvoiceItems on t1.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                " WHERE TransType = 'SalesOrder' And" & vbCrLf &
                " NOT EXISTS (SELECT * FROM TransInvoice t2" & vbCrLf &
                " WHERE invoicenumber > 0" & vbCrLf &
                " AND t2.SalesOrderID = t1.ID)" & vbCrLf &
                " AND TransInvoiceItems.ProductID = t0.ProductID" & vbCrLf &
                " AND t1.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                " GROUP BY t1.FOWSTID" & vbCrLf &
                " ) tblAvailable" & vbCrLf &
                "   OUTER APPLY (" & vbCrLf &
                "   SELECT FowstID" & vbCrLf &
                "   ,SUM(CommittedQty) AS Quantity" & vbCrLf &
                "   ,'' AS DeliveryMethod" & vbCrLf &
                "   FROM CacheFowstProducts" & vbCrLf &
                "   WHERE CacheFowstProducts.ProductID = " & ReadyString(Common.WebRequest.Data("StockID")) & vbCrLf &
                "   AND CacheFowstProducts.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                "   GROUP BY FowstID" & vbCrLf &
                " " & vbCrLf &
                "/*" & vbCrLf &
                "   SELECT t1.FOWSTID, SUM(OrderedQuantity) Quantity" & vbCrLf &
                "   ,t1.DeliveryMethod" & vbCrLf &
                "   FROM TransInvoice t1" & vbCrLf &
                "   JOIN TransInvoiceItems ON t1.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                "   WHERE t1.TransType = 'SalesOrder'" & vbCrLf &
                "   AND (" & vbCrLf &
                "  	CONVERT(varchar(36), t1.ParentOrderID) = " & ReadyString(pInvoiceID) & vbCrLf &
                "  	OR CONVERT(varchar(36), t1.ID) = " & ReadyString(pInvoiceID) & vbCrLf &
                "  )" & vbCrLf &
                "  AND NOT EXISTS (SELECT * FROM TransInvoice t2" & vbCrLf &
                "  WHERE invoicenumber > 0" & vbCrLf &
                "  AND t2.SalesOrderID = t1.ID)" & vbCrLf &
                "  AND TransInvoiceItems.ProductID = " & ReadyString(Common.WebRequest.Data("StockID")) & vbCrLf &
                "  AND t1.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                "  GROUP BY t1.FOWSTID" & vbCrLf &
                "  ,t1.DeliveryMethod" & vbCrLf &
                "*/" & vbCrLf &
                "  ) tblOrdered" & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "  AND ISNULL(MobizLibraryOrganizationFOWST.IsActive, 1) = 1 -- null is considered active" & vbCrLf &
                "  AND ISNULL(LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner, 0) = 0" & vbCrLf &
                " ) t" & vbCrLf
            .SqlGroup = "group by Account_ID, Store_ID, UPC_Code, Product_Code" & vbCrLf &
                ",t.FOWSTID" & vbCrLf &
                ",t.CommittedQty" & vbCrLf &
                ",AssignedQty" & vbCrLf &
                ",DeliveryMethod" & vbCrLf &
                ",t.FOWST" & vbCrLf
            .SqlOrder = " ORDER BY AvaiableToCommit DESC, t.FOWST" & vbCrLf
            Dim pSql = .CreateSql
            pDT = Common.WebRequest.ProData.Execute(pSql)
        End With

        Return JsonDictionary.Serialize("Results", pDT)
    End Function

    <HttpPost()>
    Public Function GetInventoryBalanceLite() As String

        Dim pDT As DataTable
        Dim pInvoiceID As String = ReadyGuid(Common.WebRequest.Data("InvoiceID"))

        If Now.Date = New Date(2020, 2, 22) Then
            Throw New ApplicationException("this is a simulated error")
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql = " -- GetInventoryBalanceLite" & vbCrLf &
            " SELECT LibraryFowstProduct.FowstID" & vbCrLf &
            " ,LibraryFowstProduct.Balance" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FOWST" & vbCrLf &
            " ,tblOrdered.Quantity AS AssignedQty" & vbCrLf &
            " ,tblOrdered.DeliveryMethod" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " LEFT JOIN LibraryFowstProduct ON MobizLibraryOrganizationFOWST.ID = LibraryFowstProduct.FowstID" & vbCrLf &
            " AND LibraryFowstProduct.ProductID = " & ReadyString(Common.WebRequest.Data("StockID")) & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            "    SELECT t1.FOWSTID, SUM(OrderedQuantity) Quantity" & vbCrLf &
            "    ,t1.DeliveryMethod" & vbCrLf &
            "    FROM TransInvoice t1" & vbCrLf &
            "    JOIN TransInvoiceItems ON t1.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "    WHERE t1.TransType = 'SalesOrder'" & vbCrLf &
            "    AND (" & vbCrLf &
            "   	CONVERT(varchar(36), t1.ParentOrderID) = " & ReadyString(pInvoiceID) & vbCrLf &
            "   	OR CONVERT(varchar(36), t1.ID) = " & ReadyString(pInvoiceID) & vbCrLf &
            "   )" & vbCrLf &
            "   AND NOT EXISTS (SELECT * FROM TransInvoice t2" & vbCrLf &
            "   WHERE invoicenumber > 0" & vbCrLf &
            "   AND t2.SalesOrderID = t1.ID)" & vbCrLf &
            "   AND TransInvoiceItems.ProductID = " & ReadyString(Common.WebRequest.Data("StockID")) & vbCrLf &
            "   AND t1.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "   GROUP BY t1.FOWSTID" & vbCrLf &
            "   ,t1.DeliveryMethod" & vbCrLf &
            " ) tblOrdered" & vbCrLf &
            " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
            " AND LTRIM(RTRIM(MobizLibraryOrganizationFOWST.Name)) <> ''" & vbCrLf &
            " ORDER BY LibraryFowstProduct.Balance - ISNULL(tblOrdered.Quantity, 0) DESC" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

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

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID), , True, True)
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pParentProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Parent product description did not return a record vbGBBJYQVJC4!")
            End If
            pOut.Add("parent", pDT.Rows(0))

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pChildProductID), , True, True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Child product description did not return a record vbFR5PQTX6X8!")
            End If
            pOut.Add("child", pDT.Rows(0))

            pDT = WebRequest.Instance.Business.LibraryStockMeasurements.ByProductID(pChildProductID)

            ' no need for the return value. just let this function create the unit definition if needed.
            WebRequest.Instance.Business.LibraryStock.GetOrCreateUnitDefinition(pChildProductID, WebRequest.Instance.OrgID)

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

            pDT = WebRequest.Instance.Business.LibraryStockMeasurements.MeasurementFamilies()
            pOut.Add("families", pDT)

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
        Dim pError As String

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

        If pParentProductID = pChildProductID Then
            Return JsonDictionary.Serialize("Error", "You may not add a product as a content of itself.")
        End If

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

        Dim pRecursive = WebRequest.Instance.Business.LibraryStock.DetectRecursiveContent(
            vOrgID:=WebRequest.Instance.OrgID,
            vParentProductID:=pParentProductID,
            vChildProductID:=pChildProductID)
        If String.IsNullOrWhiteSpace(pRecursive) = False Then
            Return JsonDictionary.Serialize("Error", "You can't add this product as a content because it would create a loop — the parent you're editing is already listed under the item you're about to add somewhere down the line.")
        End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'pError = WebRequest.Instance.Business.LibraryStock.GetRecursiveContents(WebRequest.Instance.OrgID, pParentProductID, pChildProductID)
        'If pError IsNot Nothing Then

        'End If

        Dim pProData As ProData
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDt As DataTable

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

        WebRequest.Instance.Business.LibraryStock.UpdateProductPriceFromContents(pParentProductID, WebRequest.Instance.OrgID)

        'Notify the TablesLastUpdated that there were changes done in the MobizLibraryPIMProduct
        pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'MobizLibraryPIMProduct'"
        pDt = Common.WebRequest.ProData.Execute(pSql)
        If pDt.Rows.Count > 0 Then
            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Else
            pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'MobizLibraryPIMProduct', NULL, NULL)"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        End If

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function DeleteBomContent() As String

        Try
            Dim pParentProductID As String
            Dim pChildProductID As String
            Dim pDt As DataTable

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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pProData As ProData
            Dim pSql As String

            pProData = Common.WebRequest.ProData

            pSql = "DELETE FROM MobizLibraryPIMProductContents" &
                   " WHERE ParentProductID = " & ReadyString(pParentProductID) &
                   " AND ChildProductID = " & ReadyString(pChildProductID) &
                   " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
            pProData.ExecuteSQL(pSql)
            WebRequest.Instance.Business.LibraryStock.UpdateProductPriceFromContents(pParentProductID, WebRequest.Instance.OrgID)

            'Notify the TablesLastUpdated that there were changes done in the MobizLibraryPIMProduct
            pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'MobizLibraryPIMProduct'"
            pDt = Common.WebRequest.ProData.Execute(pSql)
            If pDt.Rows.Count > 0 Then
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'MobizLibraryPIMProduct', NULL, NULL)"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Product description did not return a record vb77BUM6EN4Z!")
            End If
            pOut.Add("parent", pDT.Rows(0))

            pDR = WebRequest.Instance.Business.LibraryStock.GetOrCreateUnitDefinition(pProductID, WebRequest.Instance.OrgID)
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.LibraryStock.CreateUnitDefinition(
                vProductID:=pProductID,
                vDestinationMeasure:=pDestinationMeasure,
                vQuantity:=pQuantity,
                vSourceMeasureID:=pSourceMeasureID,
                vOrgID:=WebRequest.Instance.OrgID)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "vbTWMKH8VTGF! " & ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveBomInstruction() As String

        Dim pProductID As String
        Dim pBomInstruction As String
        Dim pJit As String
        Dim pDt As DataTable

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID vbX3S252WJX4!")
            End If
            pProductID = ReadyGuid(pProductID)

            pBomInstruction = WebRequest.Instance.Data("BomInstruction")
            pJit = WebRequest.Instance.Data("Jit")

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProData As ProData = Common.WebRequest.ProData

            pIU = New InsertUpdate("MobizLibraryPIMProductParent")
            pIU("Instructions") = ReadyString(pBomInstruction)
            pIU("Jit") = If(CBool2(pJit), 1, 0)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID))
            If pProData.ExecuteSQL(pSql, True) = 0 Then
                pIU("ID") = "newid()"
                pIU("ProductID") = ReadyString(pProductID)
                pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                pSql = pIU.GetInsert()
                pProData.ExecuteSQL(pSql)
            End If

            'Notify the TablesLastUpdated that there were changes done in the MobizLibraryPIMProduct
            pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'MobizLibraryPIMProduct'"
            pDt = Common.WebRequest.ProData.Execute(pSql)
            If pDt.Rows.Count > 0 Then
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'MobizLibraryPIMProduct', NULL, NULL)"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveSeperateIntoComponentDuringSalesReturns() As String
        Dim pProductID As String
        Dim IsSeperateIntoComponentDuringSalesReturns As String

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            IsSeperateIntoComponentDuringSalesReturns = WebRequest.Instance.Data("IsSeperateIntoComponentDuringSalesReturns")

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProData As ProData = Common.WebRequest.ProData

            pIU = New InsertUpdate("MobizLibraryPIMProductParent")
            pIU("IsSeperateIntoComponentDuringSalesReturns") = If(CBool2(IsSeperateIntoComponentDuringSalesReturns), 1, 0)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"

            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID))
            If pProData.ExecuteSQL(pSql, True) = 0 Then
                pIU("ID") = "newid()"
                pIU("ProductID") = ReadyString(pProductID)
                pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                pSql = pIU.GetInsert()
                pProData.ExecuteSQL(pSql)
            End If

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveUpcharge() As String
        Dim pProductID As String
        Dim IsSeperateIntoComponentDuringSalesReturns As String

        Try
            pProductID = WebRequest.Instance.Data("ProductID")
            Dim upcharge As String = WebRequest.Instance.Data("Upcharge")
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProData As ProData = Common.WebRequest.ProData

            pIU = New InsertUpdate("MobizLibraryPIMProductParent")

            pIU("Upcharge") = IIf(CStr2(upcharge) = "", "NULL", upcharge)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"

            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID))
            If pProData.ExecuteSQL(pSql, True) = 0 Then
                pIU("ID") = "newid()"
                pIU("ProductID") = ReadyString(pProductID)
                pIU("Jit") = ReadyString(0)
                pIU("IsSeperateIntoComponentDuringSalesReturns") = ReadyString(0)
                pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                pSql = pIU.GetInsert()
                pProData.ExecuteSQL(pSql)
            End If

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function GetParentProductInformation() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim pProData As ProData = Common.WebRequest.ProData
        Dim vParentProductID As String

        vParentProductID = Common.WebRequest.Data("ParentProductID")

        pSql = "Select * FROM MobizLibraryPIMProductParent" &
            " WHERE ProductID = " & ReadyString(vParentProductID) &
            " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
        Try

            pDT = pProData.Execute(pSql)

            Return JsonDictionary.Serialize("Result", pDT)
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function QuickAdd() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pShortName As String = Common.WebRequest.Data("ShortName")
        Dim pLandedCost As String = Common.WebRequest.Data("LandedCost")
        Dim pRetailPrice As String = Common.WebRequest.Data("RetailPrice")

        Try
            Dim pProductID As String = WebRequest.Instance.Business.LibraryStock.AddPim( _
                vPublishedByID:=WebRequest.Instance.OrgID,
                vProductNumber:=GenerateString(8),
                vProductName:=pShortName,
                vRetailPrice:=pRetailPrice,
                vSrp:=pRetailPrice,
                vLandedCost:=pLandedCost,
                vUserID:=WebSession.Instance.PeopleUserID)
            Return JsonDictionary.Serialize("Result", pProductID)
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function ProductCategories() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT = WebRequest.Instance.Business.LibraryStock.GetProductCategories()
        Return JsonDictionary.Serialize(pDT)

    End Function

    Public Function ProductBrands() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT = WebRequest.Instance.Business.LibraryStock.GetBrands()
        Return JsonDictionary.Serialize(pDT)

    End Function

    Public Function UpdateSerialNumbers() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInput = JsonConvert.DeserializeObject(Of LibraryStock.UpdateSerialsInput)(Common.WebRequest.InputStreamText)
            WebRequest.Instance.Business.LibraryStock.UpdateSerials(pInput)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function GetSerialNumbers() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pProductID = Common.WebRequest.Data("ProductID")
            Dim pDT = WebRequest.Instance.Business.LibraryStock.GetSerials(pInvoiceID, pProductID)
            Dim pOut = New JsonDictionary()
            Dim pSql As String = "SELECT SUM(MobizLibraryPIMProductMeasurement.Quantity) AS TotalBaseUnitQty " & vbCrLf &
                                " FROM MobizLibraryPIMProductMeasurement " & vbCrLf &
                                " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                                " WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(pProductID) & vbCrLf
            pOut.Add("Result", "ok")
            pOut.Add("data", pDT)
            pOut.Add("TotalBaseUnitQty", Common.WebRequest.ProData.Execute(pSql))
            Return pOut.Serialize
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Class ChangeProductsAreaInput

        Public Area As String
        Public FowstID As String
        Public ProductIDs As List(Of String)

    End Class

    Public Function ChangeProductsArea() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInput = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ChangeProductsAreaInput)(Common.WebRequest.InputStreamText)
            For Each pProductID In pInput.ProductIDs
                WebRequest.Instance.Business.LibraryStock.ChangeProductArea(pProductID, pInput.FowstID, pInput.Area, False)
            Next

            Dim pOut = New JsonDictionary()
            pOut.Add("Count", pInput.ProductIDs.Count)
            Return pOut.Serialize()
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function DuplicateProductNumberCount() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pResult = WebRequest.Instance.Business.LibraryStock.DuplicateProductNumberCount()

            Dim pOut = New JsonDictionary()
            pOut.Add("Result", pResult)
            Return pOut.Serialize()
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function DeduplicateProductNumberCount() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pResult = WebRequest.Instance.Business.LibraryStock.DeduplicateProductNumbers()

            Dim pOut = New JsonDictionary()
            pOut.Add("Result", pResult)
            Return pOut.Serialize()
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function ProductsAndContentsExcel() As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFilename = WebRequest.Instance.Business.LibraryStock.ProductsAndContentsExcel(WebRequest.Instance.OrgID)
            Return JsonDictionary.Serialize("Filename", pFilename)
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function DownloadDuplicateProductNumbers() As ActionResult

        Try
            WebRequest.Instance.OrgID = WebRequest.Instance.QueryString("OrgID")
            If WebRequest.Instance.HandleAjax = False Then Return Content("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return Content("Invalid Organization")

            Dim pDT = WebRequest.Instance.Business.LibraryStock.DuplicateProductNumbers()
            pDT = WebRequest.Instance.Business.LibraryStock.FillDeduplicatedProductNumbers(pDT)

            Dim pFilename = Common.ServerPath & "temp\DeduplicateProductNumbers\DeduplicateProductNumbers " & DateTimeStamp(False) & ".xlsx"
            CreateFolder(pFilename)
            TableToExcelFile(pDT, "", "", pFilename)

            Return File(pFilename, "application/xlxs", IO.Path.GetFileName(pFilename))
        Catch ex As Exception
            Return Content(ex.Message)
        End Try

    End Function
End Class
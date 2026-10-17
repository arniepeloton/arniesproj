''' <summary>
''' copy data from MobizDUC.TransInvoice to Wholesale.TransSalesOrder
''' </summary>
''' <remarks></remarks>
Public Class InvoiceToOrder
    ReadOnly mDbSource As ProData
    ReadOnly mDbDestination As ProData
    ReadOnly mBusinessSource As Business

    Public Sub New(vDbSource As ProData, vDbDestination As ProData)

        mDbSource = vDbSource
        mBusinessSource = New Business(mDbSource)
        mDbDestination = vDbDestination

    End Sub

    Public Function Execute(Optional vInvoiceID As String = Nothing, Optional ByRef oMessage As String = Nothing) As Boolean

        Dim pBusiness As Business
        Dim pCreator As TransSalesOrder.Creator
        Dim pDtUnexported As DataTable
        Dim pDS As DataSet
        Dim pDrSource As DataRow
        Dim pItem As TransSalesOrder.Creator.Item
        Dim pSql As String
        Dim pCheckPriceRequest As WholesaleCheckPriceRequest
        Dim pDrOrder As DataRow

        oMessage = Nothing
        pDtUnexported = GetUnexportedSalesOrders(vInvoiceID)
        If pDtUnexported.Rows.Count = 0 Then
            Return True
        End If
        pBusiness = New Business(mDbDestination)
        For Each pDrUnexported As DataRow In pDtUnexported.Rows
            pDrOrder = pBusiness.TransSalesOrder.LegacyGetByMobizInvoiceID(CStr2(pDrUnexported("ID")))
            If pDrOrder IsNot Nothing Then
                If CDec2(pDrOrder("IsHold")) = 0 Then
                    Continue For
                End If
            End If
            pDS = mBusinessSource.TransInvoice.GetReportData("WHERE TransInvoice.ID = " & ReadyString(pDrUnexported("ID")), False)
            If pDS.Tables("Invoices").Rows.Count = 0 Then
                ' invoice not found
                Continue For
            End If
            pDrSource = pDS.Tables("Invoices").Rows(0)
            pCreator = pBusiness.TransSalesOrder.CreateCreator()
            pCreator.DeliveryDate = Now
            pCreator.BranchID = pBusiness.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 5)
            'pCreator.SubscriberBranchID = WebSession.Instance.SubscriberOrganizationBranchID
            pCreator.DeviceID = CStr2(pDrSource("DeviceID"))
            pCreator.CustomerID = CStr2(pDrSource("CustomerID"))
            pCreator.CustomerAddressID = CStr2(pDrSource("BillingAddressID"))
            If CStr2(pCreator.CustomerAddressID) = "" Then
                oMessage = "Customer, " & pCreator.CustomerID & " has no billing address"
                Return False
            End If
            pCreator.DeliverToID = CStr2(pDrSource("DeliveryAddressID"))
            If CStr2(pCreator.DeliverToID) = "" Then
                oMessage = "Customer, " & pCreator.CustomerID & " has no delivery address"
                Return False
            End If
            If CStr2(pDrSource("Remarks")) <> "" Then
                pCreator.Remarks = Truncate(pDrSource("Remarks"), 999)
            End If
            pCreator.CusRepresentativeID = pBusiness.LibraryCustomers.LegacyGetOrAddContactPerson(pCreator.CustomerID)

            pCreator.IsHold = False
            pCreator.IsForApproval = True
            pCreator.ForApprovalReason = "vb2NUWNRUZHS! To be determined by server agent because this order came from Mobiz" ' 81 chars
            pCreator.DontAutoInvoice = False
            pCreator.MobizInvoiceID = CStr2(pDrUnexported("ID"))
            pCreator.InvoiceTypeCode = CStr2(pDrSource("InvoiceTypeCode"))

            pCreator.UserID = CDec2(pDrSource("DUCNumericID"))
            For Each pDrItem As DataRow In pDS.Tables("Stocks").Rows
                pItem = New TransSalesOrder.Creator.Item
                pItem.StockID = CStr2(pDrItem("ProductID"))
                pItem.UnitMeasure = pDrItem("UnitMeasure")
                pItem.PriceLevelID = CStr2(pDrItem("PriceLevelID"))
                pItem.ActualQuantity = pDrItem("ActualQuantity")
                pItem.UnitPrice = pDrItem("UnitPrice")
                If CDec2(pDrItem("RequestedUnitPrice")) > 0 Then
                    pItem.RequestedUnitPrice = CDec(pDrItem("RequestedUnitPrice"))
                End If
                pItem.OriginalUnitPrice = pDrItem("UnitPrice")
                pItem.UnitPriceWOTax = pItem.UnitPrice
                pItem.Quantity = CDec2(pDrItem("OrderedQuantity")) 'edited by janine sept 11 2014 for sales order taker
                pItem.SubTotal = pItem.UnitPrice * pItem.Quantity
                pItem.Cost = pBusiness.LibraryStockLegacy.GetLandedCost(pItem.StockID) '* pItem.ActualQuantity
                pItem.CancellationPeriod = CDec2(pBusiness.SystemDefaults.GetOrAddSystemDefault("BackOrder Cancellation Period", 15))

                pItem.TermDiscount = 0 'pStockDiscount
                pItem.DiscountIfPaidWithIn = 0 'Math.Min(CDec2(pCustomerTerm), pStockDiscountIfPaidWithIn)
                pItem.NetDue = 0 'If(pUseCustomerTerm, pCustomerTerm, pStockTerm)

                If IsNumeric(pDrItem("LineNumber")) Then
                    pItem.LineNumber = pDrItem("LineNumber")
                Else
                    pItem.LineNumber = 1
                End If
                pCreator.Items.Add(pItem)
            Next pDrItem
            If pDrOrder Is Nothing Then
                pCreator.Save(True)
            Else
                pCreator.ID = CStr2(pDrOrder("ID"))
                pCreator.Save(False)
            End If

            Dim pIU As InsertUpdate

            pIU = New InsertUpdate("TransInvoice")
            pIU("SalesOrderID") = ReadyString(pCreator.ID)
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrUnexported("ID")))
            mDbSource.ExecuteSQL(pSql)

            pCheckPriceRequest = New WholesaleCheckPriceRequest(pBusiness, pCreator.ID)
            Debug.Print("BNWD7FRCU2 pCheckPriceRequest.Result: " & pCheckPriceRequest.Result)
            Debug.Print("DG5GQDSJH6 pCheckPriceRequest.Message: " & pCheckPriceRequest.Message)
        Next pDrUnexported

        Return True

    End Function

    Private Function GetUnexportedSalesOrders(Optional vInvoiceID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "(SalesOrderID IS NULL" &
            " AND SalesOrderNo IS NOT NULL)"
        If CStr2(vInvoiceID) <> "" Then
            pSql &= " AND ID = " & ReadyString(vInvoiceID)
        Else

        End If
        pSql = "SELECT ID" &
            " FROM TransInvoice" &
            " WHERE " &
            pSql &
            " ORDER BY LastUpdated"
        pDT = mDbSource.Execute(pSql)

        Return pDT

    End Function
End Class ' InvoiceToOrder

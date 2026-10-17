Public Class TransBackOrders

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

#If DEBUG Then
    Const mMinSalesOrderDate As Date = #3/23/2000#
#Else
    Const mMinSalesOrderDate As Date = #3/23/2016#
#End If

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetBackOrderables() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- mobizFC - derive backorder" & vbCrLf &
            " SELECT SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) AS OrderedQuantity" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.InvoiceID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransBackOrderProducts.BackOrderID" & vbCrLf &
            " ,TransBackOrderProducts.Qty" & vbCrLf &
            " ,tblItems.UnitPrice" & vbCrLf &
            " ,tblItems.PriceLevelID" & vbCrLf &
            " ,tblItems.Cost" & vbCrLf &
            " ,tblItems.ActualQuantity" & vbCrLf &
            " ,tblWithdrawals.WithdrawnQuantity" & vbCrLf &
            " ,ISNULL(SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity), 0)" & vbCrLf &
            " 	- ISNULL(tblWithdrawals.WithdrawnQuantity, 0) AS BackOrderQty" & vbCrLf &
            " FROM  TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " AND TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " -- not really needed." & vbCrLf &
            " -- get BackOrderID if a backroder already exist." & vbCrLf &
            " LEFT JOIN TransBackOrderProducts" & vbCrLf &
            " JOIN TransBackOrders ON TransBackOrders.ID = TransBackOrderProducts.BackOrderID" & vbCrLf &
            " ON TransBackOrderProducts.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            " AND TransBackOrders.OriginalSalesOrderID = TransInvoice.ID" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " 	SELECT SUM(TransInvoiceItemWarehouseWithdrawal.PreparedQuantity * TransInvoiceItemWarehouseWithdrawal.UnitQty) AS WithdrawnQuantity" & vbCrLf &
            " 	FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
            "     WHERE TransInvoiceItems.ProductID = TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
            "     AND TransInvoiceItems.InvoiceID = TransInvoiceItemWarehouseWithdrawal.InvoiceID" & vbCrLf &
            " ) tblWithdrawals" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " 	SELECT TOP 1 tblItems.UnitPrice" & vbCrLf &
            "     ,tblItems.PriceLevelID" & vbCrLf &
            "     ,tblItems.Cost" & vbCrLf &
            "     ,tblItems.ActualQuantity" & vbCrLf &
            "     FROM TransInvoiceItems tblItems" & vbCrLf &
            "     WHERE tblItems.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "     AND tblItems.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "     ORDER BY tblItems.ActualQuantity" & vbCrLf &
            " ) tblItems" & vbCrLf &
            " WHERE TransInvoice.SalesOrderDate >= '3/31/2016'" & vbCrLf &
            " GROUP BY TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.InvoiceID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransBackOrderProducts.BackOrderID" & vbCrLf &
            " ,TransBackOrderProducts.Qty" & vbCrLf &
            " ,tblItems.UnitPrice" & vbCrLf &
            " ,tblItems.PriceLevelID" & vbCrLf &
            " ,tblItems.Cost" & vbCrLf &
            " ,tblItems.ActualQuantity" & vbCrLf &
            " ,tblWithdrawals.WithdrawnQuantity" & vbCrLf &
            " HAVING ISNULL(tblWithdrawals.WithdrawnQuantity, 0) < SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity)" & vbCrLf &
            " -- exclude backorders that already has a serving sales order." & vbCrLf &
            " -- also exclude backorders that have the same backorder quantity as OrderQty - WithdrawnQty" & vbCrLf &
            " AND NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM TransBackOrderProducts" & vbCrLf &
            "     JOIN TransBackOrders ON TransBackOrders.ID = TransBackOrderProducts.BackOrderID" & vbCrLf &
            "     WHERE TransBackOrders.OriginalSalesOrderID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "     AND TransBackOrderProducts.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "     AND (" & vbCrLf &
            "     	TransBackOrderProducts.ServingOrderID IS NOT NULL" & vbCrLf &
            " 		OR TransBackOrderProducts.Qty = ISNULL(SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity), 0) - ISNULL(tblWithdrawals.WithdrawnQuantity, 0)" & vbCrLf &
            " 	)" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CreateBackOrders() As Integer

        Dim pDtBackOrderables As DataTable
        Dim pItemResult As CreateBackOrderResult
        Dim pDV As DataView

        pDtBackOrderables = GetBackOrderables()
        For Each pDrBackOrderable As DataRow In pDtBackOrderables.Rows
            pItemResult = CreateBackOrder(
                vBackOrderID:=CStr2(pDrBackOrderable("BackOrderID")),
                vInvoiceID:=CStr2(pDrBackOrderable("InvoiceID")),
                vProductID:=CStr2(pDrBackOrderable("ProductID")),
                vBackOrderQty:=CDec2(pDrBackOrderable("OrderedQuantity")) - CDec2(pDrBackOrderable("WithdrawnQuantity")),
                vUnitPrice:=CDec2(pDrBackOrderable("UnitPrice")) / CDec2(pDrBackOrderable("ActualQuantity")),
                vPriceLevelID:=CStr2(pDrBackOrderable("PriceLevelID")),
                vCost:=CDec2(pDrBackOrderable("Cost")) / CDec2(pDrBackOrderable("ActualQuantity")),
                vUnitQty:=CDec2(pDrBackOrderable("ActualQuantity"))
            )
            ' pDrBackOrderable("BackOrderID") = pItemResult.BackOrderID

            ' update rows with null BackOrderID and same InvoiceID.
            ' then set their BackOrderID
            pDV = New DataView(pDtBackOrderables, "BackOrderID IS NULL AND InvoiceID = " & ReadyString(pDrBackOrderable("InvoiceID")), "", DataViewRowState.CurrentRows)
            For Each pDRV As DataRowView In pDV
                pDRV("BackOrderID") = pItemResult.BackOrderID
            Next pDRV
        Next pDrBackOrderable

        Return pDtBackOrderables.Rows.Count

    End Function

    Function CreateBackOrder(
        vBackOrderID As String,
        vInvoiceID As String,
        vProductID As String,
        vBackOrderQty As Decimal,
        vUnitPrice As Decimal,
        vPriceLevelID As String,
        vCost As Decimal,
        vUnitQty As Decimal) As CreateBackOrderResult

        Dim pOut As CreateBackOrderResult
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        Try
            mProData.BeginTransaction()
            pOut = New CreateBackOrderResult
            If String.IsNullOrWhiteSpace(vBackOrderID) Then
                pOut.BackOrderID = Guid.NewGuid().ToString()
                pIU = New InsertUpdate("TransBackOrders")
                pIU("ID") = ReadyString(pOut.BackOrderID)
                pIU("OriginalSalesOrderID") = ReadyString(vInvoiceID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
                vBackOrderID = pOut.BackOrderID
            Else
                pOut.BackOrderID = vBackOrderID
            End If

            pIU = New InsertUpdate("TransBackOrderProducts")
            pIU("Qty") = vBackOrderQty
            pIU("UnitPrice") = vUnitPrice
            pIU("PriceLevelID") = ReadyString(vPriceLevelID)
            pIU("Cost") = vCost
            pIU("UnitQty") = vUnitQty
            pSql = pIU.GetUpdate("BackOrderID = " & ReadyString(vBackOrderID) & vbCrLf &
                " AND ProductID = " & ReadyString(vProductID))
            pCount = mProData.ExecuteSQL(pSql, True)
            If pCount = 0 Then
                pIU("ID") = "NEWID()"
                pIU("BackOrderID") = ReadyString(vBackOrderID)
                pIU("ProductID") = ReadyString(vProductID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
            mProData.CommitTransaction()
        Catch ex As Exception
            mProData.RollbackTransaction()
            Throw
        End Try

        Return pOut

    End Function

    Function GetOrderables() As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " -- GetOrderables" & vbCrLf &
            " SELECT TransBackOrders.ID" & vbCrLf &
            " ,TransBackOrders.OriginalSalesOrderID" & vbCrLf &
            " ,TransInvoice.SalesOrderDate" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " FROM TransBackOrders" & vbCrLf &
            " JOIN TransBackOrderProducts ON TransBackOrderProducts.BackOrderID = TransBackOrders.ID" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransBackOrders.OriginalSalesOrderID" & vbCrLf &
            " JOIN TransReceivingStock ON TransReceivingStock.ProductID = TransBackOrderProducts.ProductID" & vbCrLf &
            " JOIN TransReceiving ON TransReceiving.ID = TransReceivingStock.ReceivingID" & vbCrLf &
            " AND ISNULL(TransReceiving.DontServeBackOrder, 0) = 0" & vbCrLf &
            " AND TransBackOrderProducts.ServingOrderID IS NULL" & vbCrLf &
            " GROUP BY TransBackOrders.ID" & vbCrLf &
            " ,TransBackOrders.OriginalSalesOrderID" & vbCrLf &
            " ,TransInvoice.SalesOrderDate" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ORDER BY TransInvoice.SalesOrderDate" & vbCrLf &
            " ,TransInvoice.LastUpdated"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Function GetOrderablesReceivings() As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " -- GetOrderablesReceivings" & vbCrLf &
            " SELECT TransReceivingStock.ProductID" & vbCrLf &
            " ,SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS ReceivedQty" & vbCrLf &
            " FROM TransReceivingStock" & vbCrLf &
            " JOIN TransReceiving ON TransReceiving.ID = TransReceivingStock.ReceivingID" & vbCrLf &
            " AND ISNULL(TransReceiving.DontServeBackOrder, 0) = 0" & vbCrLf &
            " WHERE TransReceivingStock.ProductID IN (" & vbCrLf &
            " 	SELECT TransBackOrderProducts.ProductID" & vbCrLf &
            "     FROM TransBackOrderProducts" & vbCrLf &
            "     WHERE TransBackOrderProducts.ServingOrderID IS NULL" & vbCrLf &
            " )" & vbCrLf &
            " GROUP BY TransReceivingStock.ProductID"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Function GetOrderableProducts(vBackOrderID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT TransBackOrderProducts.ProductID" & vbCrLf &
            " ,TransBackOrderProducts.Qty" & vbCrLf &
            " ,TransBackOrderProducts.PriceLevelID" & vbCrLf &
            " ,TransBackOrderProducts.Cost" & vbCrLf &
            " ,TransBackOrderProducts.UnitQty" & vbCrLf &
            " ,TransBackOrderProducts.UnitPrice" & vbCrLf &
            " FROM TransBackOrderProducts" & vbCrLf &
            " WHERE TransBackOrderProducts.BackOrderID = " & ReadyString(vBackOrderID)
        mProData.Execute(pSql)
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    ''' <summary>
    ''' this function is safe even for non-DUC
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateOrders() As Integer

        Dim pDtOrderables As DataTable
        Dim pDtReceivings As DataTable
        Dim pDrDevice As DataRow

        Try
            mProData.BeginTransaction()
            pDrDevice = mParent.LibraryDevice.GetDesktop()
            If pDrDevice Is Nothing Then
                Throw New Exception("No desktop device. Please go to System Administration -> Authorized Devices, then add a device with blank UUID. vb7MMHFVLEUT!")
            End If

            ' todo: modify query to fetch unique backorder ID's and original sales order ID's
            pDtOrderables = GetOrderables()

            ' work around: group on client side
            pDtOrderables = pDtOrderables.DefaultView.ToTable(True, "ID", "OriginalSalesOrderID")

            If pDtOrderables.Rows.Count = 0 Then
                mParent.TransReceiving.SetDontServeBackOrder()
                mProData.CommitTransaction()
                Return 0
            End If
            pDtReceivings = GetOrderablesReceivings()
            For Each pDrOrderable As DataRow In pDtOrderables.Rows
                CreateOrder(CStr2(pDrOrderable("ID")), _
                    CStr2(pDrOrderable("OriginalSalesOrderID")),
                    CStr2(pDrDevice("ID")),
                    pDtReceivings)
            Next pDrOrderable
            mParent.TransReceiving.SetDontServeBackOrder()
            mProData.CommitTransaction()

            Return pDtOrderables.Rows.Count
        Catch ex As Exception
            AppendToLog(GetStackTrace(, ex), , "TransBackOrders")
            mProData.RollbackTransaction(, True)
            Return -1
        End Try

    End Function

    ''' <summary>
    ''' returns the count of line items
    ''' </summary>
    ''' <param name="vBackOrderID">TransBackOrders.ID</param>
    ''' <param name="vOriginalSalesOrderID">TransInvoice.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CreateOrder(vBackOrderID As String, vOriginalSalesOrderID As String, vDeviceID As String, ByRef rDtReceivingsProducts As DataTable) As Integer

        Dim pDrOriginalSalesOrder As DataRow
        Dim pDtProducts As DataTable
        Dim pCreator As TransInvoice.Creator
        Dim pError As String
        Dim pDrNewProduct As DataRow

        pDtProducts = GetOrderableProducts(vBackOrderID)
        If pDtProducts.Rows.Count = 0 Then
            Return 0
        End If

        ' consume received products
        For Each pDrOrdered As DataRow In pDtProducts.Rows
            rDtReceivingsProducts.DefaultView.RowFilter = "ProductID = " & ReadyString(pDrOrdered("ProductID")) & vbCrLf &
                " AND ReceivedQty > 0"
            If rDtReceivingsProducts.DefaultView.Count = 0 Then
                pDrOrdered("Qty") = 0
                Continue For
            End If
            rDtReceivingsProducts.DefaultView(0)("ReceivedQty") -= CDec2(pDrOrdered("Qty"))
        Next pDrOrdered
        pDtProducts = New DataView(pDtProducts, "Qty > 0", "", DataViewRowState.CurrentRows).ToTable()
        If pDtProducts.Rows.Count = 0 Then
            Return 0
        End If

        pDrOriginalSalesOrder = mParent.TransInvoice.ById(vOriginalSalesOrderID)
        pCreator = mParent.TransInvoice.CreateCreator()
        pCreator.SalesOrderNo = mParent.TransInvoice.GetNextSoNumber(vDeviceID, CStr2(pDrOriginalSalesOrder("FowstID")))
        pCreator.SalesOrderDate = Now
        pCreator.CustomerID = CStr2(pDrOriginalSalesOrder("CustomerID"))
        pCreator.FowstID = CStr2(pDrOriginalSalesOrder("FowstID"))
        pCreator.DeviceID = vDeviceID
        pCreator.InvoiceNumber = 0
        pCreator.UserID = modClient.EmployeeWebGuidID
        pCreator.BillingAddressID = CStr2(pDrOriginalSalesOrder("BillingAddressID"))
        pCreator.DeliveryAddressID = CStr2(pDrOriginalSalesOrder("DeliveryAddressID"))

        pCreator.Stocks = New DataTable
        With pCreator.Stocks.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Integer))
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

        For Each pDrProduct As DataRow In pDtProducts.Rows
            pDrNewProduct = pCreator.Stocks.NewRow()
            pDrNewProduct("StockID") = CStr2(pDrProduct("ProductID"))
            pDrNewProduct("Quantity") = 0 ' dont use null here
            pDrNewProduct("OrderedQuantity") = CDec2(pDrProduct("Qty"))
            pDrNewProduct("ActualQuantity") = 1 '  ' todo: derive the unit qty from library
            pDrNewProduct("UnitMeasure") = "'PC'" ' todo: derive the unit measure with the lowest unit qty from library
            pDrNewProduct("UnitPrice") = CDec2(pDrProduct("UnitPrice"))
            pDrNewProduct("Subtotal") = CDec2(pDrProduct("UnitPrice")) * CDec2(pDrProduct("Qty"))
            pDrNewProduct("Cost") = CDec2(pDrProduct("Cost")) ' todo: derive from library
            pDrNewProduct("PriceLevelID") = CStr2(pDrProduct("PriceLevelID"))
            pCreator.Stocks.Rows.Add(pDrNewProduct)
        Next pDrProduct
        mProData.BeginTransaction()
        Try
            If pCreator.Save(True, pError) Then
                mParent.TransInvoice.UpdateSoCounter(vDeviceID, pCreator.FowstID, pCreator.SalesOrderNo)
                For Each pDrProduct As DataRow In pDtProducts.Rows
                    SetServingOrderID(vBackOrderID, CStr2(pDrProduct("ProductID")), pCreator.ID)
                Next pDrProduct
            End If
            mProData.CommitTransaction()
        Catch ex As Exception
            mProData.RollbackTransaction()
            Throw
        End Try

    End Function

    Function SetServingOrderID(vBackOrderID As String, vProductID As String, vServingInvoiceID As String) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer

        pIU = New InsertUpdate("TransBackOrderProducts")
        pIU("ServingOrderID") = ReadyString(vServingInvoiceID)
        pSql = pIU.GetUpdate("BackOrderID = " & ReadyString(vBackOrderID) &
            " AND ProductID = " & ReadyString(vProductID) &
            " AND ServingOrderID IS NULL")
        pRecords = mProData.ExecuteSQL(pSql, True)

        Return pRecords

    End Function

    Class CreateBackOrderResult

        Public BackOrderID As String

    End Class

    Public Class CreateServingSalesOrderInput
        Public BackOrderID As String
        Public Products As List(Of CreateServingSalesOrderProductInput)

        Public Sub New()

            Products = New List(Of CreateServingSalesOrderProductInput)

        End Sub
    End Class

    Public Class CreateServingSalesOrderProductInput
        Public ProductID As String
        Public Qty As Decimal
    End Class

    Public Function CreateServingSalesOrder(vInput As CreateServingSalesOrderInput)

        Dim pIU As InsertUpdate


        pIU = New InsertUpdate("")

    End Function
End Class


Public Class TransSalesOrder

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ByID(vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransSalesOrder.OrderNo" & vbCrLf & _
            " ,TransSalesOrder.CustomerID" & vbCrLf & _
            " ,TransSalesOrder.CustomerAddressID" & vbCrLf & _
            " ,TransSalesOrder.DeliverToID" & vbCrLf & _
            ",TransSalesOrder.DeliveryDate" & vbCrLf &
            ",TransSalesOrder.Remarks" & vbCrLf &
            ",TransSalesOrder.LastUpdated" & vbCrLf &
            ",LibraryCustomers.CompanyName" & vbCrLf &
            ",LibraryCustomers.IsCompanyBranch" & vbCrLf &
            " FROM TransSalesOrder" & vbCrLf & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" & vbCrLf &
            " WHERE TransSalesOrder.ID = " & ReadyString(vID) & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetOrderDetails(ByVal vID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT TransMemoSalesReturn.ID" & _
            ",TransMemoSalesReturn.MemoID" & _
            ",TransMemoSalesReturn.InvoiceID," & _
            "MainInvoiceDefaults.Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') + ' - ' + CONVERT(varchar, TransMemoSalesReturn.NetDue) AS InvoiceNum," & _
            "TransInvoice.InvoiceDate," & _
            "TransMemoSalesReturn.StockID," & _
            "LibraryStockInformation.StockNumber," & _
            "   ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription," & _
            "TransMemoSalesReturn.QuantityReturned," & _
            "TransMemoSalesReturn.UnitMeasure," & _
            "TransMemoSalesReturn.UnitPrice," & _
            "TransMemoSalesReturn.TotalAmount," & _
            "TransMemoSalesReturn.Remarks," & _
            "TransMemoSalesReturn.BracketNumber," & _
            "TransMemoSalesReturn.NetDue," & _
            "TransMemoSalesReturn.CustomerID," & _
            "LibraryCustomers.CompanyName AS CustomerName," & _
            "TransMemoSalesReturn.OrigQuantity," & _
            "TransMemoSalesReturn.OrigActualQuantity," & _
            "TransMemoSalesReturn.OrigNetInvoicePrice," & _
            "TransMemoSalesReturn.ActualQuantity," & _
            "TransMemoSalesReturn.AdditionalRemark, " & _
            "TransMemoSalesReturn.OrigQuantity * TransMemoSalesReturn.OrigActualQuantity - TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity AS QtyRemaining" & _
            ",TransMemoSalesReturn.Remarks"
        pSql = pSql & _
            " FROM TransMemoSalesReturn" & _
            " LEFT JOIN TransInvoice ON TransMemoSalesReturn.InvoiceID = TransInvoice.ID" & _
            " LEFT JOIN MainInvoiceDefaults ON TransInvoice.InvoiceDefaultID = MainInvoiceDefaults.ID" & _
            " JOIN LibraryStockInformation ON TransMemoSalesReturn.StockID = LibraryStockInformation.ID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & _
            " JOIN LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " & _
            " WHERE CONVERT(varchar(36), TransMemoSalesReturn.MemoID) = " & ReadyString(vID)
        pSql &= " ORDER BY ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') "
        Return mProData.Execute(pSql)

    End Function

    Public Function NextNum() As Integer

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MAX(OrderNo) AS OrderNo" & _
            " FROM TransSalesOrder"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CDec2(pDT.Rows(0)("OrderNo")) + 1
        Else
            Return 1
        End If

    End Function

    Public Function GetStockSummary(ByVal vBranchID As Integer, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql, pSqlStock As String

        '// main stock info
        pSqlStock = "SELECT TransSalesOrderItems.StockID" & _
            ",StockNumber" & _
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & _
            ",SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS QtyOrdered"

        pSqlStock &= ",(SELECT TOP 1 WholesaleStockInventory.StocksOnHand" & _
            " FROM WholesaleStockInventory" & _
            " WHERE StockID = TransSalesOrderItems.StockID" & _
            " AND BranchID = " & vBranchID & _
            " ORDER BY Lastupdated DESC) AS StockOnHand"

        pSqlStock &= ",ISNULL(" & _
            " 	(SELECT SUM(TransSalesOrderItems1.Quantity * TransSalesOrderItems1.ActualQuantity) AS BasedQuantity " & _
            " 	 FROM TransSalesOrderItems TransSalesOrderItems1" & _
            "    JOIN TransSalesOrder TransSalesOrder1 ON TransSalesOrderItems1.SalesOrderID = TransSalesOrder1.ID" & _
            "    LEFT JOIN TransInvoice ON TransSalesOrderItems1.SalesOrderID = TransInvoice.SalesOrderID" & _
            " 	 WHERE TransSalesOrder1.IsCancelled = 0 " & _
            " 	 AND TransSalesOrderItems1.IsCancelled = 0 " & _
            " 	 AND TransInvoice.ID IS NULL " & _
            " 	 AND TransSalesOrder1.IsHold = 0 " & _
            " 	 AND TransSalesOrderItems1.StockID = TransSalesOrderItems.StockID" & _
            "    AND DATEDIFF(day, TransSalesOrder1.RegisteredDate, GETDATE()) < TransSalesOrderItems1.CancellationPeriod" & _
            "    AND TransSalesOrder1.BranchID = " & vBranchID & ")" & _
            " , 0) " & _
            " +" & _
            "  ISNULL(" & _
            " 	(SELECT SUM(Quantity * ActualQuantity) AS BasedQuantity" & _
            " 	 FROM TransSalesBackOrderItems" & _
            "    JOIN TransSalesOrder TransSalesOrder1 ON TransSalesOrder1.ID = TransSalesBackOrderItems.SalesOrderID" & _
            " 	 WHERE TransSalesBackOrderItems.IsCancelled = 0 " & _
            " 	 AND IsServe = 0 " & _
            " 	 AND StockID = TransSalesOrderItems.StockID" & _
            "    AND DATEDIFF(day, TransSalesBackOrderItems.RegisteredDate, GETDATE()) < TransSalesBackOrderItems.CancellationPeriod" & _
            "    AND TransSalesOrder1.BranchID = " & vBranchID & _
            "    )" & _
            ", 0) AS QtyCommited"

        pSqlStock &= ",(SELECT MAX(DateAndTimeEnded) AS DateAndTimeEnded" & _
        " FROM TransCycleCountStocks" & _
        " JOIN TransCycleCount" & _
        " ON TransCycleCountStocks.CycleCountID = TransCycleCount.ID" & _
        " WHERE TransCycleCountStocks.StockID = TransSalesOrderItems.StockID" & _
        " AND IsCancelled = 0" & _
        " AND ApprovedBy <> 0) AS DateOfLastApprovedCount"

        pSqlStock &= " FROM TransSalesOrder" & _
            " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransSalesOrderItems.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & _
            " WHERE CONVERT(datetime, FLOOR(CONVERT(float, OrderDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & _
            " AND TransSalesOrder.IsCancelled = 0" & _
            " GROUP BY TransSalesOrderItems.StockID" & _
            ",StockNumber" & _
            ",LibraryBrands.BrandName" & _
            ",LibraryProducts.ProductName" & _
            ",LibraryStockInformation.Make" & _
            ",LibraryStockInformation.UnitSize" & _
            ",LibraryStockInformation.Content" & _
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & _
            " HAVING SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) > 0"

        '// prefered supplier
        pSql = "(SELECT TOP 1 LibrarySupplierStockPrices.ID" & _
            " FROM LibrarySupplierStockPrices" & _
            " JOIN LibrarySupplierStocks LibrarySupplierStocks1" & _
            " ON LibrarySupplierStocks1.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " WHERE LibrarySupplierStocks1.StockID = t.StockID" & _
            " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.Quantity" & _
            ") AS SupplierStockPricesId"

        pSql = "SELECT *, " & pSql & _
            " FROM (" & pSqlStock & ") t"

        pSql = "SELECT t.*" & _
            ",LibrarySuppliers.SupplierName" & _
            " FROM (" & pSql & ") t" & _
            " LEFT JOIN LibrarySupplierStockPrices ON LibrarySupplierStockPrices.ID = t.SupplierStockPricesId" & _
            " LEFT JOIN LibrarySupplierStocks ON LibrarySupplierStocks.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " LEFT JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & _
            " ORDER BY StockDescription"

        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("AvailableToCommit", GetType(Integer), "StockOnHand - QtyCommited")

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <param name="vMaxLeadTime">If less than zero, lead times will be ignored</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StockSummary(ByVal vDateFrom As Date, ByVal vDateTo As Date, _
        Optional ByVal vMaxLeadTime As Integer = -1) As DataTable

        Dim pDT As DataTable
        Dim pSql, pSqlOrder As String

        pSqlOrder = "SELECT TransSalesOrderItems.StockID" & _
            ",SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Qty" & _
            " FROM TransSalesOrder" & _
            " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" & _
            " WHERE TransSalesOrder.IsCancelled = 0" & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & _
            " AND CONVERT(datetime, FLOOR(CONVERT(float, OrderDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & _
            " GROUP BY TransSalesOrderItems.StockID"

        If vMaxLeadTime >= 0 Then pSql = " AND LibrarySuppliers.LeadTime <= " & vMaxLeadTime
        pSql = "(SELECT TOP 1 LibrarySupplierStockPrices.ID" & _
            " FROM LibrarySupplierStockPrices" & _
            " JOIN LibrarySupplierStocks LibrarySupplierStocks1" & _
            " ON LibrarySupplierStocks1.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " JOIN LibrarySuppliers" & _
            " ON LibrarySuppliers.ID = LibrarySupplierStocks1.SupplierID" & _
            " WHERE LibrarySupplierStocks1.StockID = t.StockID" & _
            " AND LibrarySuppliers.IsActive = 1" & _
            pSql & _
            " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.Quantity" & _
            ") AS SupplierStockPricesId"

        pSql = "SELECT *, " & pSql & _
            " FROM (" & pSqlOrder & ") t"

        pSql = "SELECT t.StockID" & _
            ",t.Qty" & _
            ",LibrarySupplierStocks.SupplierID" & _
            ",LibrarySuppliers.ShipmentMode" & _
            " FROM (" & pSql & ") t" & _
            " LEFT JOIN LibrarySupplierStockPrices ON LibrarySupplierStockPrices.ID = t.SupplierStockPricesId" & _
            " LEFT JOIN LibrarySupplierStocks ON LibrarySupplierStocks.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " LEFT JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function StockSummary(ByVal vMonth As Byte, ByVal vIsSecondHalf As Boolean, ByVal vYearFrom As Integer, ByVal vYearTo As Integer) As DataTable

        Dim pDT As DataTable
        Dim pSql, pSqlOrder As String

        If vIsSecondHalf Then
            pSqlOrder = " AND DATEPART(day, OrderDate) > 15"
        Else
            pSqlOrder = " AND DATEPART(day, OrderDate) <= 15"
        End If

        pSqlOrder = "SELECT TransSalesOrderItems.StockID" & _
               ",SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Qty" & _
               " FROM TransSalesOrder" & _
               " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & _
               " WHERE TransSalesOrder.IsCancelled = 0" & _
               " AND DATEPART(month, OrderDate) = " & vMonth & _
               pSqlOrder & _
               " AND DATEPART(year, OrderDate) BETWEEN " & vYearFrom & " AND " & vYearTo & _
               " GROUP BY TransSalesOrderItems.StockID"

        pSql = "(SELECT TOP 1 LibrarySupplierStockPrices.ID" & _
            " FROM LibrarySupplierStockPrices" & _
            " JOIN LibrarySupplierStocks LibrarySupplierStocks1" & _
            " ON LibrarySupplierStocks1.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " WHERE LibrarySupplierStocks1.StockID = t.StockID" & _
            " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.Quantity" & _
            ") AS SupplierStockPricesId"

        pSql = "SELECT *, " & pSql & _
            " FROM (" & pSqlOrder & ") t"

        pSql = "SELECT t.StockID" & _
            ", t.Qty" & _
            ",LibrarySupplierStocks.SupplierID" & _
            " FROM (" & pSql & ") t" & _
            " LEFT JOIN LibrarySupplierStockPrices ON LibrarySupplierStockPrices.ID = t.SupplierStockPricesId" & _
            " LEFT JOIN LibrarySupplierStocks ON LibrarySupplierStocks.ID = LibrarySupplierStockPrices.SupplierStockID"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CreateCreator() As Creator

        Return New Creator(Me)

    End Function

    Public Class Creator
        ReadOnly mProData As ProData

        ''' <summary>
        ''' read-only
        ''' </summary>
        ''' <remarks></remarks>
        Public ID As String
        Public OrderNo As Integer   ' if 0, generate
        '                           ' if -1, dont assign number
        Public SubscriberBranchID As String
        Public DeviceID As String
        Public CustomerID As String
        Public CustomerAddressID As String
        Public CusRepresentativeID As String
        Public DeliverToID As String
        Public DeliveryDate As Date?
        Public IsBranch As Boolean
        Public BranchID As String
        Public UserID As Integer
        Public Remarks As String
        Public DontAutoInvoice As Boolean = True    ' 1 by default on the database schema
        Public IsHold As Boolean
        Public IsForApproval As Boolean
        Public ForApprovalReason As String
        Public MobizInvoiceID As String
        Public InvoiceTypeCode As String ' wholesale.MainInvoiceDefaults.Abbrevation

        Private ReadOnly mItems As ItemsClass
        ReadOnly mParent As TransSalesOrder

        Friend Sub New(vParent As TransSalesOrder)

            mParent = vParent
            mProData = mParent.mProData
            mItems = New ItemsClass(Me)

        End Sub

        Public ReadOnly Property ProData() As ProData
            Get

                Return mProData

            End Get
        End Property

        Public Function Save(ByVal vNew As Boolean) As Boolean

            Dim pInsertUpdate As InsertUpdate
            Dim pSql As String
            Dim pDrInvoiceType As DataRow

            pInsertUpdate = New InsertUpdate("TransSalesOrder")
            If OrderNo <> -1 Then
                If OrderNo = 0 Then OrderNo = mParent.NextNum
                If OrderNo <> 0 Then pInsertUpdate("OrderNo") = OrderNo
            End If
            If CStr2(CustomerID).Length > 0 Then pInsertUpdate("CustomerID") = ReadyString(CustomerID)
            If CStr2(CustomerAddressID).Length > 0 Then pInsertUpdate("CustomerAddressID") = ReadyString(CustomerAddressID)
            If CStr2(CusRepresentativeID).Length > 0 Then pInsertUpdate("CusRepresentativeID") = ReadyString(CusRepresentativeID)
            If CStr2(DeliverToID).Length > 0 Then pInsertUpdate("DeliverToID") = ReadyString(DeliverToID)
            If DeliveryDate IsNot Nothing Then pInsertUpdate("DeliveryDate") = ReadyString(DeliveryDate)
            pInsertUpdate("DeliveryMethod") = "'Deliver'"
            pInsertUpdate("IsBranch") = CInt(IsBranch)
            If CStr2(BranchID).Length > 0 Then pInsertUpdate("BranchID") = ReadyString(BranchID)
            pInsertUpdate("UserID") = UserID
            pInsertUpdate("LastUpdated") = "GETDATE()"
            pInsertUpdate("IsRushOrder") = 1
            If Remarks IsNot Nothing Then pInsertUpdate("Remarks") = ReadyString(Remarks)
            pInsertUpdate("DontAutoInvoice") = If(DontAutoInvoice, 1, 0)
            pInsertUpdate("IsHold") = If(IsHold, 1, 0)
            pInsertUpdate("IsForApproval") = If(IsForApproval, 1, 0)
            If CStr2(ForApprovalReason) <> "" Then
                pInsertUpdate("ForApprovalReason") = ReadyString(ForApprovalReason)
            End If
            If CStr2(SubscriberBranchID).Length > 0 Then
                pInsertUpdate("SubscriberBranchID") = ReadyString(SubscriberBranchID)
            Else
                pInsertUpdate("SubscriberBranchID") = "null"
            End If
            If CStr2(DeviceID).Length > 0 Then
                pInsertUpdate("DeviceID") = ReadyString(DeviceID)
            Else
                pInsertUpdate("DeviceID") = "null"
            End If
            If CStr2(MobizInvoiceID).Length > 0 Then
                pInsertUpdate("MobizInvoiceID") = ReadyString(MobizInvoiceID)
            End If
            pDrInvoiceType = mParent.LegacyGetInvoiceTypeIDByAbbrevation(InvoiceTypeCode)
            If pDrInvoiceType IsNot Nothing Then
                pInsertUpdate("DefaultInvoiceTypeID") = ReadyString(pDrInvoiceType("ID"))
            End If

            If vNew Then
                ID = CStr2(Guid.NewGuid)
                pInsertUpdate("ID") = ReadyString(ID)
                pInsertUpdate("OrderDate") = "CONVERT(datetime, FLOOR(CONVERT(float, GETDATE())))"
                pInsertUpdate("EncodedBy") = UserID
                pInsertUpdate("ImportDate") = "GETDATE()"
                pInsertUpdate("DeliverWithin") = 0
                pInsertUpdate("IsDowntown") = 1
                pSql = pInsertUpdate.GetInsert
            Else
                pSql = pInsertUpdate.GetUpdate("ID = " & ReadyString(ID))
            End If

            mProData.BeginTransaction()
            mProData.ExecuteSQL(pSql)
            If mItems.Save Then
                mProData.CommitTransaction()
            Else
                mProData.RollbackTransaction()
            End If

            Return True

        End Function

        Public ReadOnly Property Items() As ItemsClass
            Get

                Return mItems

            End Get
        End Property

        Public Class ItemsClass
            Inherits List(Of Item)

            ReadOnly mParent As Creator

            Public Sub New(ByVal vParent As Creator)

                mParent = vParent

            End Sub

            Public ReadOnly Property Parent() As Creator
                Get

                    Return mParent

                End Get
            End Property

            Public Shadows Function Add() As Item

                Dim pItem As Item

                pItem = New Item(Me)
                MyBase.Add(pItem)
                Return pItem

            End Function

            Public Shadows Sub Add(ByVal vItem As Item)

                vItem.Parent = Me
                MyBase.Add(vItem)

            End Sub

            Friend Function Save() As Boolean

                Dim pItem As Item

                Delete()
                For Each pItem In Me
                    If pItem.Save = False Then Return False
                Next pItem

                Return True

            End Function

            Private Sub Delete()

                Dim pSql As String

                pSql = "DELETE FROM TransSalesOrderItems" & vbCrLf & _
                    " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(mParent.ID) & vbCrLf
                mParent.ProData.ExecuteSQL(pSql)

            End Sub
        End Class

        Public Class Item

            Public Parent As ItemsClass
            Public StockID As String
            Public Quantity As Integer
            Public ActualQuantity As Integer
            Public UnitMeasure As String
            Public UnitPrice As Decimal
            Public SubTotal As Decimal
            Public TermDiscount As Decimal
            Public DiscountIfPaidWithIn As Integer
            Public NetDue As String
            Public OutputTax As Decimal
            Public UnitPriceWOTax As Decimal
            Public PriceLevelID As String
            Public CancellationPeriod As Integer
            Public Cost As Decimal
            Public Dimension As String
            Public OriginalUnitPrice As Decimal
            Public LineNumber As Integer
            Public RequestedUnitPrice As Decimal?

            Friend Sub New()

            End Sub

            Friend Sub New(ByVal vParent As ItemsClass)

                Parent = vParent

            End Sub

            Friend Function Save() As Boolean

                Dim pInsertUpdate As InsertUpdate
                Dim pSql As String

                If Parent Is Nothing Then
                    Throw New Exception("Parent not yet set")
                End If
                pInsertUpdate = New InsertUpdate("TransSalesOrderItems")
                pInsertUpdate("StockID") = ReadyString(StockID)
                pInsertUpdate("SalesOrderID") = ReadyString(Parent.Parent.ID)
                pInsertUpdate("Quantity") = Quantity
                pInsertUpdate("ActualQuantity") = ActualQuantity
                pInsertUpdate("UnitMeasure") = ReadyString(UnitMeasure)
                pInsertUpdate("UnitPrice") = UnitPrice
                pInsertUpdate("SubTotal") = SubTotal
                pInsertUpdate("TermDiscount") = TermDiscount
                pInsertUpdate("DiscountIfPaidWithIn") = DiscountIfPaidWithIn
                pInsertUpdate("NetDue") = ReadyString(NetDue)
                pInsertUpdate("OutputTax") = OutputTax
                pInsertUpdate("UnitPriceWOTax") = UnitPriceWOTax
                pInsertUpdate("PriceLevelID") = ReadyString(PriceLevelID)
                pInsertUpdate("CancellationPeriod") = CancellationPeriod
                pInsertUpdate("Cost") = Cost
                pInsertUpdate("Dimension") = ReadyString(Dimension)
                pInsertUpdate("OriginalUnitPrice") = OriginalUnitPrice
                If LineNumber > 0 Then
                    pInsertUpdate("LineNumber") = LineNumber
                End If
                If RequestedUnitPrice IsNot Nothing Then
                    pInsertUpdate("RequestedUnitPrice") = RequestedUnitPrice
                End If
                pSql = pInsertUpdate.GetInsert
                Parent.Parent.ProData.ExecuteSQL(pSql)

                Return True

            End Function
        End Class
    End Class

    Public Function IsPrintedPickingList(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SalesOrderID " & _
               "FROM TransStockPickingListItems Where SalesOrderID='" & vID & "'"

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function IsInvoiced(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID " & _
               "FROM TransInvoice " & _
               "WHERE SalesOrderID = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function IsCancelled(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim sDataRow As DataRow
        Dim pSql As String

        pSql = "SELECT DISTINCT " & _
               "ID," & _
               "IsCancelled " & _
               "From TransSalesOrder " & _
               "WHERE (ID ='" & vID & "')"

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            sDataRow = pDT.Rows(0)

            If sDataRow("IsCancelled") Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Public Function GetStocksTermsDiscAndNetDue(ByVal vStockID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryTermsDiscount.DiscountPercentage," & _
               "LibraryTermsDiscount.DiscountPaidWithin," & _
               "LibraryTerms.Term AS NetDue " & _
                " FROM LibraryStockTermsDiscount INNER JOIN " & _
                " LibraryTermsDiscount ON LibraryStockTermsDiscount.TermsDiscountID = LibraryTermsDiscount.ID INNER JOIN " & _
                " LibraryTerms ON LibraryStockTermsDiscount.TermsID = LibraryTerms.ID " & _
                " Where (LibraryStockTermsDiscount.StockID = '" & vStockID.ToUpper & "')"

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then

            Dim strStockDiscount As Single, _
                strStockDiscountIfPaidWithIn As Decimal, _
                GetStockTerm As String

            strStockDiscount = 0
            strStockDiscountIfPaidWithIn = 0
            GetStockTerm = "0"

            For Each Trm As DataRow In pDT.Rows

                If IsNumeric(Trm("NetDue")) Then
                    If IsNumeric(GetStockTerm) Then
                        strStockDiscount = Trm("DiscountPercentage")
                        strStockDiscountIfPaidWithIn = Trm("DiscountPaidWithin")
                        GetStockTerm = IIf(Trm("NetDue") > CLng(GetStockTerm), Trm("NetDue"), GetStockTerm)
                    Else
                        strStockDiscount = Trm("DiscountPercentage")
                        strStockDiscountIfPaidWithIn = Trm("DiscountPaidWithin")
                        GetStockTerm = Trm("NetDue")
                    End If
                Else
                    If GetStockTerm = "0" Then
                        strStockDiscount = Trm("DiscountPercentage")
                        strStockDiscountIfPaidWithIn = Trm("DiscountPaidWithin")
                        GetStockTerm = Trm("NetDue")
                    End If
                End If

            Next

            '---Store the Data base from the condition above
            pDT.Rows(0)("DiscountPercentage") = strStockDiscount
            pDT.Rows(0)("DiscountPaidWithin") = strStockDiscountIfPaidWithIn
            pDT.Rows(0)("NetDue") = GetStockTerm

            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetCustomerMaxTerms(ByVal vCustomerID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryTerms.Term " & _
               "FROM LibraryCustomersTerms INNER JOIN " & _
               "LibraryTerms ON LibraryCustomersTerms.TermsID = LibraryTerms.ID " & _
               "WHERE (LibraryCustomersTerms.CustomerID ='" & vCustomerID & "')"

        pDT = mProData.Execute(pSql)

        Dim GetCustMaxTerm As String = "0"

        If pDT.Rows.Count > 0 Then

            For Each Trm As DataRow In pDT.Rows

                If IsNumeric(pDT.Rows(0)("Term")) Then
                    If IsNumeric(GetCustMaxTerm) Then
                        GetCustMaxTerm = IIf(pDT.Rows(0)("Term") > CLng(GetCustMaxTerm), pDT.Rows(0)("Term"), GetCustMaxTerm)
                    Else
                        GetCustMaxTerm = pDT.Rows(0)("Term")
                    End If
                Else
                    If GetCustMaxTerm = "0" Then
                        GetCustMaxTerm = pDT.Rows(0)("Term")
                    End If
                End If
                Return GetCustMaxTerm
            Next
        Else
            Return "COD"
        End If

    End Function

    Public Function IsRushOrder(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID " & _
               " From TransSalesOrder " & _
               " WHERE (ID ='" & vID & "') " & _
               " AND (IsRushOrder=1) "

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetLackingInfo(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pLack As String

        pSql = "SELECT TinNumber" & _
               ",FaxNumber" & _
               ",EmailAddress" & _
               " FROM LibraryCustomers" & _
               " WHERE ID = '" & vID & "'"

        pDT = mProData.Execute(pSql)
        pLack = ""

        If pDT.Rows.Count > 0 Then
            If Trim(pDT.Rows(0)("TinNumber")) = vbNullString Or pDT.Rows(0)("TinNumber") Is Nothing Or
                pDT.Rows(0)("TinNumber") = "000000000" Or pDT.Rows(0)("TinNumber") = "*********" Then
                pLack = pLack & "Tin Number, "
            End If

            If Trim(pDT.Rows(0)("FaxNumber")) = vbNullString Or pDT.Rows(0)("TinNumber") Is Nothing Then
                pLack = pLack & "Fax Number, "
            End If

            If Trim(pDT.Rows(0)("EmailAddress")) = vbNullString Or pDT.Rows(0)("EmailAddress") Is Nothing Then
                pLack = pLack & "Email Address"
            End If
            Return pLack
        End If

    End Function

    Public Function GetWorkingEndTime(ByRef hrsOfNoWork As String, ByVal isHaveNoWork As Boolean) As String
        Dim pDT As DataTable
        Dim pDT2 As DataTable
        Dim pSql As String
        Dim restDays As String()
        Dim ctr As Integer
        Dim isRestday As Boolean

        pSql = "Select DefaultValue from SystemDefaults where Description = 'Branch distribution'"

        pDT = mProData.Execute(pSql)

        pSql = "SELECT DavaoUnifiedPayroll.dbo.LibraryBranch.ID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, LibraryWorkingHours.MondayFrom, " _
                & "LibraryWorkingHours.MondayTo, LibraryWorkingHours.TuesdayFrom, LibraryWorkingHours.TuesdayTo, LibraryWorkingHours.WednesdayFrom, " _
                & "LibraryWorkingHours.WednesdayTo, LibraryWorkingHours.ThursdayFrom, LibraryWorkingHours.ThursdayTo, LibraryWorkingHours.FridayFrom, " _
                & "LibraryWorkingHours.FridayTo, LibraryWorkingHours.SaturdayFrom, LibraryWorkingHours.SaturdayTo, LibraryWorkingHours.SundayFrom, " _
                & "LibraryWorkingHours.SundayTo, dbo.LibraryWorkingHours.Restdays FROM DavaoUnifiedPayroll.dbo.LibraryBranch RIGHT OUTER JOIN " _
                & "LibraryBranches ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = LibraryBranches.BranchID INNER JOIN " _
                & "LibraryWorkingHours ON LibraryBranches.ID = LibraryWorkingHours.BranchID " _
                & "Where DavaoUnifiedPayroll.dbo.LibraryBranch.ID = '" & CStr2(pDT.Rows(0)("DefaultValue")) & "'"

        pDT2 = mProData.Execute(pSql)

        If pDT2.Rows.Count > 0 Then
            restDays = Split(pDT2.Rows(0)("Restdays"))

            While ctr <= UBound(restDays)
                If CInt(Weekday(mProData.GetDate)) = restDays(ctr) Then
                    isRestday = True
                    isHaveNoWork = True
                End If
                ctr = ctr + 1
            End While

            Select Case CInt(Weekday(mProData.GetDate))
                Case 1 'sunday
                    GetWorkingEndTime = pDT2.Rows(0)("SundayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("TuesdayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("MondayFrom")
                    End If

                Case 2
                    GetWorkingEndTime = pDT2.Rows(0)("MondayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("WednesdayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("TuesdayFrom")
                    End If

                Case 3
                    GetWorkingEndTime = pDT2.Rows(0)("TuesdayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("ThursdayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("WednesdayFrom")
                    End If

                Case 4
                    GetWorkingEndTime = pDT2.Rows(0)("WednesdayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("FridayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("ThursdayFrom")
                    End If

                Case 5
                    GetWorkingEndTime = pDT2.Rows(0)("ThursdayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("SaturdayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("FridayFrom")
                    End If

                Case 6
                    GetWorkingEndTime = pDT2.Rows(0)("FridayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("SundayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("SaturdayFrom")
                    End If

                Case 7
                    GetWorkingEndTime = pDT2.Rows(0)("SaturdayTo")
                    If isRestday Then
                        hrsOfNoWork = pDT2.Rows(0)("MondayFrom")
                    Else
                        hrsOfNoWork = pDT2.Rows(0)("SundayFrom")
                    End If

            End Select
        End If
    End Function

    Public Function GetWorkingStartTime(ByVal weekdayNo As Integer, ByVal hrsOfNoWork As String, ByVal isHaveNoWork As Boolean) As String
        Dim pDT As DataTable
        Dim pDT2 As DataTable
        Dim pSql As String
        Dim ctr As Integer
        Dim restDays() As String
        Dim isRestday As Boolean

        pSql = "Select DefaultValue from SystemDefaults where Description = 'Branch distribution'"

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            pSql = "SELECT DavaoUnifiedPayroll.dbo.LibraryBranch.ID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, LibraryWorkingHours.MondayFrom, " _
                & "LibraryWorkingHours.MondayTo, LibraryWorkingHours.TuesdayFrom, LibraryWorkingHours.TuesdayTo, LibraryWorkingHours.WednesdayFrom, " _
                & "LibraryWorkingHours.WednesdayTo, LibraryWorkingHours.ThursdayFrom, LibraryWorkingHours.ThursdayTo, LibraryWorkingHours.FridayFrom, " _
                & "LibraryWorkingHours.FridayTo, LibraryWorkingHours.SaturdayFrom, LibraryWorkingHours.SaturdayTo, LibraryWorkingHours.SundayFrom, " _
                & "LibraryWorkingHours.SundayTo, dbo.LibraryWorkingHours.Restdays FROM DavaoUnifiedPayroll.dbo.LibraryBranch RIGHT OUTER JOIN " _
                & "LibraryBranches ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = LibraryBranches.BranchID INNER JOIN " _
                & "LibraryWorkingHours ON LibraryBranches.ID = LibraryWorkingHours.BranchID " _
                & "Where DavaoUnifiedPayroll.dbo.LibraryBranch.ID = " & ReadyString(pDT.Rows(0)("DefaultValue"))

            pDT2 = mProData.Execute(pSql)

            If pDT2.Rows.Count > 0 Then
                restDays = Split(pDT2.Rows(0)("Restdays"))

                While ctr <= UBound(restDays)
                    If CInt(weekdayNo) = restDays(ctr) Then
                        isRestday = True
                        isHaveNoWork = True
                    End If
                    ctr = ctr + 1
                End While
                Select Case CInt(weekdayNo)
                    Case 1 'sunday
                        GetWorkingStartTime = pDT2.Rows(0)("SundayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("FridayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("SaturdayTo")
                        End If

                    Case 2
                        GetWorkingStartTime = pDT2.Rows(0)("MondayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("SaturdayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("SundayTo")
                        End If

                    Case 3
                        GetWorkingStartTime = pDT2.Rows(0)("TuesdayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("SundayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("MondayTo")
                        End If

                    Case 4
                        GetWorkingStartTime = pDT2.Rows(0)("WednesdayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("MondayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("TuesdayTo")
                        End If

                    Case 5
                        GetWorkingStartTime = pDT2.Rows(0)("ThursdayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("TuesdayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("WednesdayTo")
                        End If

                    Case 6
                        GetWorkingStartTime = pDT2.Rows(0)("FridayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("WednesdayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("ThursdayTo")
                        End If

                    Case 7
                        GetWorkingStartTime = pDT2.Rows(0)("SaturdayFrom")
                        If isRestday Then
                            hrsOfNoWork = pDT2.Rows(0)("ThursdayTo")
                        Else
                            hrsOfNoWork = pDT2.Rows(0)("FridayTo")
                        End If
                End Select
            End If

        End If
    End Function

    Public Function GetDefaultNotRushTime() As Integer

        Dim RSTable As DataTable
        Dim Sql As String

        Sql = "Select Description, DefaultValue from SystemDefaults where Description = 'DowntownNotRush'"

        RSTable = mProData.Execute(Sql)

        If RSTable.Rows.Count > 0 Then
            'If (CDec2(RSTable.Rows(0)("DefaultValue")) / 60) >= 1 Then
            Return CDec2(RSTable.Rows(0)("DefaultValue")) '/ 60
            'Else
            '   Return CDec2(RSTable.Rows(0)("DefaultValue"))
            'End If
        Else
            Return 0
        End If
        RSTable.Dispose()

    End Function

    Public Sub CheckAndUpdateLoadingDay()

        Dim rs As DataTable
        Dim sql As String
        Dim strDispatch() As String
        Dim addDateDaily As Integer

        sql = "Select ID, DispatchDay, DeliveryMode, NextDayOfLoading from LibraryRoute"

        If Weekday(Now) = 2 Then
            addDateDaily = 2
        Else
            addDateDaily = 1
        End If

        rs = mProData.Execute(sql)

        If rs.Rows.Count > 0 Then
            strDispatch = Split(rs.Rows(0)("DispatchDay"), "/")
            For Each rsData As DataRow In rs.Rows
                If DateDiff("n", Now, rsData("NextDayOfLoading")) < 0 Then
                    Select Case rsData("DeliveryMode")
                        Case "vliDaily"
                            sql = "Update LibraryRoute set NextDayOfLoading = " & ReadyString(DateAdd("d", addDateDaily, rsData("NextDayOfLoading"))) & " WHERE ID = " & ReadyString(rsData("ID"))
                        Case "vliWeekly"
                            sql = "Update LibraryRoute set NextDayOfLoading = " & ReadyString(DateAdd("d", 7, rsData("NextDayOfLoading"))) & " WHERE ID = " & ReadyString(rsData("ID"))
                        Case "vliEveryOtherWeek"
                            sql = "Update LibraryRoute set NextDayOfLoading = " & ReadyString(DateAdd("d", 14, rsData("NextDayOfLoading"))) & " WHERE ID = " & ReadyString(rsData("ID"))
                        Case "vliMonthly"
                            sql = "Update LibraryRoute set NextDayOfLoading = " & ReadyString(DateAdd("m", 1, DateAdd("d", 1, rsData("NextDayOfLoading")))) & " WHERE ID = " & ReadyString(rsData("ID"))
                    End Select
                    mProData.ExecuteSQL(sql)
                End If
            Next
        End If
    End Sub

    ' ''' <summary>
    ' ''' creates sorting list from TransStockWithdrawalToWarehouse
    ' ''' </summary>
    ' ''' <param name="vOrderID"></param>
    ' ''' <returns>number of created sortinglist created</returns>
    ' ''' <remarks></remarks>
    'Public Function CreateSortingLists(ByVal vOrderID As String) As Integer

    '    Dim pDtPickingLists As DataTable
    '    Dim pDrPickingList As DataRow
    '    Dim pDT As DataTable
    '    Dim pDR As DataRow
    '    Dim pSql As String
    '    Dim pIU As InsertUpdate

    '    pDtPickingLists = GetPickingList(vOrderID)
    '    For Each pDrPickingList In pDtPickingLists.Rows
    '        If GetSortingListByPickingList(CStr2(pDrPickingList("ID"))).Rows.Count = 0 _
    '            AndAlso IsPickingListFullyWithdrawn(CStr2(pDrPickingList("ID"))) Then

    '            CreateSortingList(CStr2(pDrPickingList("ID")), vOrderID)
    '        End If
    '    Next pDrPickingList

    'End Function

    ' ''' <summary>
    ' ''' this function is not yet finished.
    ' ''' create a sorting list base on warehouse withdrawal.
    ' ''' </summary>
    ' ''' <param name="vPickingListID"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Private Function CreateSortingList(ByVal vPickingListID As String, ByVal vSalesOrderID As String) As Boolean

    '    Dim pDtWithdraw As DataTable
    '    Dim pDR As DataRow
    '    Dim pSql As String
    '    Dim pIU As InsertUpdate
    '    Dim pID As String

    '    pIU = New InsertUpdate("TransStockSortingList")
    '    pID = CStr2(Guid.NewGuid)
    '    pIU("ID") = ReadyString(pID)
    '    pIU("StockPickingListID") = ReadyString(vPickingListID)
    '    pIU("SortingListNo") = mProData.MaxNum("TransStockSortingList", , "SortingListNo") + 1
    '    pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '    pSql = pIU.GetInsert
    '    mProData.ExecuteSQL(pSql)

    '    pIU = New InsertUpdate("TransStockSortingListItems")
    '    pIU("StockSortingListID") = ReadyString(pID)
    '    pIU("SalesOrderID") = ReadyString(vSalesOrderID)
    '    pSql = "SELECT TransStockWithdrawalToWarehouseItems.StockID" &
    '        ",TransStockWithdrawalToWarehouseItems.ServedQuantity AS Quantity" &
    '        ",TransStockWithdrawalToWarehouseItems.ActualQuantity" &
    '        ",TransStockWithdrawalToWarehouseItems.UnitMeasure" &
    '        ",TransStockWithdrawalToWarehouseItems.IsHasReplacement" &
    '        ",TransStockWithdrawalToWarehouseItems.IsHasBackOrder" &
    '        ",NULL AS RefStockID" &
    '        " FROM TransStockWithdrawalToWarehouseItems" &
    '        " JOIN TransStockWithdrawalToWarehouse ON TransStockWithdrawalToWarehouse.ID = TransStockWithdrawalToWarehouseItems.StockWithdrawalToWarehouseID" &
    '        " WHERE TransStockWithdrawalToWarehouse.StockPickingListID = " & ReadyString(vPickingListID) &
    '        " AND TransStockWithdrawalToWarehouse.IsSorted = 0"
    '    pSql &= " UNION "
    '    pSql &= "SELECT TransStockWithdrawalReplacement.StockID" &
    '        ",TransStockWithdrawalReplacement.Quantity" &
    '        ",TransStockWithdrawalReplacementActualQuantity" &
    '        ",TransStockWithdrawalReplacementUnitMeasure" &
    '        ",1 AS IsHasReplacement" &
    '        ",0 AS IsHasBackOrder" &
    '        ",RefStockID" &
    '        " FROM TransStockWithdrawalReplacement" &
    '        " JOIN TransStockWithdrawalToWarehouse ON TransStockWithdrawalToWarehouse.ID = TransStockWithdrawalReplacement.StockWithdrawalToWarehouseID" &
    '        " WHERE TransStockWithdrawalToWarehouse.StockPickingListID = " & ReadyString(vPickingListID) &
    '        " AND TransStockWithdrawalToWarehouse.IsSorted = 0"
    '    pDtWithdraw = mProData.Execute(pSql)
    '    For Each pDR In pDtWithdraw.Rows
    '        If pDR("RefStockID") Is DBNull.Value Then
    '            pIU("RefStockID") = "NULL"
    '        Else
    '            pIU("RefStockID") = ReadyString(pDR("RefStockID"))
    '        End If
    '        pIU("StockID") = ReadyString(pDR("StockID"))
    '        pIU("Quantity") = pDR("Quantity")
    '        pIU("ActualQuantity") = pDR("ActualQuantity")
    '        pIU("UnitMeasure") = ReadyString(pDR("UnitMeasure"))
    '        ' this part is not yet finished
    '        pSql = pIU.GetInsert
    '        mProData.ExecuteSQL(pSql)
    '    Next pDR

    'End Function

    Private Function GetPickingList(ByVal vOrderID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransStockPickingList.ID" & _
            " FROM TransStockPickingListItems" & _
            " JOIN TransStockPickingList ON TransStockPickingList.ID = TransStockPickingListItems.StockPickingListID" & _
            " WHERE TransStockPickingListItems.SalesOrderID = " & ReadyString(vOrderID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function IsPickingListFullyWithdrawn(ByVal vPickingListID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT IsWithdraw " & _
            " FROM TransStockPickingListMeasurements " & _
            " WHERE StockPickingListID = " & ReadyString(vPickingListID) &
            " AND IsWithdraw = 0"
        pDT = mProData.Execute(pSql)

        Return pDT.Rows.Count = 0

    End Function

    Private Function GetSortingListByPickingList(ByVal vPickingListID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID" &
            " FROM TransStockSortingList" &
            " WHERE StockPickingListID = " & ReadyString(vPickingListID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ActionTaken(ByVal vID As String) As String

        Dim pRS As DataTable
        Dim pSql As String

        pSql = "SELECT ActionTaken" & _
            " FROM LibraryCustomers" & _
            " WHERE ID = " & ReadyString(vID)

        pRS = cGlobal.SharedProData.Execute(pSql)

        If pRS.Rows.Count > 0 Then
            ActionTaken = pRS.Rows(0)("ActionTaken") & ""
        Else
            ActionTaken = ""
        End If

    End Function

    Public Sub RecordTransLogs(ByVal vID As String, ByVal vTransactionDate As DateTime, _
                               ByVal vTypeOfTransactions As String, _
                               ByVal vStartDate As DateTime, ByVal vEndDate As DateTime, _
                               ByVal vUserID As Integer, _
                               ByVal vTranType As String, _
                               Optional ByVal RefNum As String = "")
        Try
            Dim StrUpdate As String = "INSERT INTO RecordTransLogs " & _
                                             "(ID,TransSactionID,DateOfTrans,TransSactionType,StartTransDate,EndTransDate,UserID,TranType,Refnum) VALUES " & _
                                             "(NewID(),'" & vID & "','" & vTransactionDate & "','" & vTypeOfTransactions & "', " & _
                                             "'" & vStartDate & "','" & vEndDate & "'," & vUserID & ",'" & vTranType & "','" & RefNum & "')"

            cGlobal.SharedProData.ExecuteSQL(StrUpdate)

        Catch ex As Exception
#If _MyType = "Windows" _
    Or _MyType = "WindowsForms" Then

            ShowMessage(ex.Message, "Saving Record Trans Log", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
#End If

        End Try

    End Sub

#Region " auto-create picklists for orders w/o PL's "
    'Public Sub CreatePickingLists()

    '    Dim pDT As DataTable
    '    Dim pDR As DataRow
    '    Dim pPickingListID As String    ' TransStockPickingList.ID

    '    Try
    '        pDT = OrdersWithoutPickingList()
    '    Catch ex As Exception
    '        Throw
    '    End Try

    '    Try
    '        For Each pDR In pDT.Rows
    '            mProData.BeginTransaction()
    '            pPickingListID = SavePickingList(CStr2(pDR("ID")))
    '            If pPickingListID IsNot Nothing Then
    '                Try
    '                    SavePickingListItems(pPickingListID, CStr2(pDR("ID")))
    '                    SavePickingListMeasurements(pPickingListID)
    '                    mProData.CommitTransaction()
    '                Catch ex As Exception
    '                    mProData.RollbackTransaction()
    '                    Exit For
    '                End Try
    '            Else
    '                mProData.RollbackTransaction()
    '            End If
    '        Next pDR
    '    Catch ex As Exception
    '        Throw
    '    End Try

    'End Sub

    Private Function OrdersWithoutPickingList() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransSalesOrder.ID" & _
            ",TransSalesOrder.OrderNo" & _
            ",TransSalesOrder.OrderDate" & _
            ",TransSalesOrder.LastUpdated" & _
            " FROM TransSalesOrder" & _
            " LEFT JOIN TransStockPickingListItems" & _
            " ON TransStockPickingListItems.SalesOrderID = TransSalesOrder.ID" & _
            " LEFT JOIN TransInvoice" & _
            " ON TransInvoice.SalesOrderID = TransSalesOrder.ID" & _
            " WHERE ISNULL(TransSalesOrder.IsCancelled, 0) = 0" & _
            " AND ISNULL(IsForApproval, 0) = 0" & _
            " AND ISNULL(Dubious, 0) = 0" & _
            " AND TransStockPickingListItems.ID IS NULL" & _
            " AND TransInvoice.ID IS NULL" & _
            " AND TransSalesOrder.IsHold = 0" & _
            " AND ISNULL(TransSalesOrder.InStore, 0) = 0" & _
            " AND EXISTS(SELECT * FROM TransSalesOrderItems WHERE SalesOrderID = TransSalesOrder.ID)" & _
            " AND TransSalesOrder.OrderDate >= '9/14/2010'" & _
            " ORDER BY TransSalesOrder.OrderDate" & _
            ",TransSalesOrder.LastUpdated"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    'Private Function SavePickingList(ByVal vOrderID As String) As String

    '    Dim pDT As DataTable
    '    Dim pSql As String
    '    Dim pIU As InsertUpdate
    '    Dim pID As String

    '    pSql = "SELECT ID" & _
    '        " FROM TransStockPickingListItems" & _
    '        " WHERE SalesOrderID = " & ReadyString(vOrderID)
    '    pDT = mProData.Execute(pSql)
    '    If pDT.Rows.Count > 0 Then
    '        Exit Function
    '    End If

    '    pIU = New InsertUpdate
    '    pIU.TableName = "TransStockPickingList"
    '    pIU("ID") = "newid()"
    '    pIU("IsRushPrint") = 1
    '    pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '    pSql = pIU.GetInsert
    '    pID = mProData.InsertRow(pSql)

    '    pSql = "SELECT TOP 1" & _
    '        " PickingListNo" & _
    '        " From TransStockPickingList " & _
    '        " WHERE ID = " & ReadyString(pID)
    '    pDT = mProData.Execute(pSql)

    '    Return pID

    'End Function

    Private Sub SavePickingListItems(ByVal vPickingListID As String, ByVal vOrderID As String)

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pIU As InsertUpdate

        pSql = "SELECT SalesOrderID," & _
            "StockID," & _
            "Quantity," & _
            "ActualQuantity," & _
            "UnitMeasure," & _
            "TermDiscount," & _
            "DiscountIfPaidWithIn," & _
            "NetDue," & _
            "UnitPrice," & _
            "SubTotal," & _
            "PriceLevelID," & _
            "IsCancelled," & _
            "CancellationPeriod " & _
            ",Dimension" & _
            " From TransSalesOrderItems " & _
            " WHERE (SalesOrderID ='" & vOrderID & "') "
        pDT = mProData.Execute(pSql)

        pIU = New InsertUpdate
        pIU.TableName = "TransStockPickingListItems"
        For Each pDR In pDT.Rows
            pIU("StockPickingListID") = ReadyString(vPickingListID)
            pIU("SalesOrderID") = ReadyString(vOrderID)
            pIU("StockID") = ReadyString(pDR("StockID"))
            pIU("Quantity") = pDR("Quantity")
            pIU("ActualQuantity") = pDR("ActualQuantity")
            pIU("UnitMeasure") = ReadyString(pDR("UnitMeasure"))
            pIU("TermDiscount") = pDR("TermDiscount")
            pIU("DiscountIfPaidWithIn") = pDR("DiscountIfPaidWithIn")
            pIU("NetDue") = ReadyString(pDR("NetDue"))
            pIU("UnitPrice") = pDR("UnitPrice")
            pIU("SubTotal") = pDR("SubTotal")
            pIU("PriceLevelID") = ReadyString(pDR("PriceLevelID"))
            pIU("IsBackOrder") = 0
            pIU("CancellationPeriod") = ReadyString(pDR("CancellationPeriod"))
            If pDR("Dimension") Is DBNull.Value Then
                pIU("Dimension") = "NULL"
            Else
                pIU("Dimension") = ReadyString(pDR("Dimension"))
            End If
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Sub

    Private Sub SavePickingListMeasurements(ByVal vPickingListID As String)

        Dim pTotalQuantity As Long
        Dim pQuantity As Long
        Dim pBasedMeasure As String
        Dim pDtItems As DataTable
        Dim pDrItem As DataRow
        Dim pDtMeasurements As DataTable
        Dim pDrMeasurement As DataRow
        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate
        pIU.TableName = "TransStockPickingListMeasurements"

        pSql = "SELECT TransStockPickingListItems.StockID," & _
            "SUM(TransStockPickingListItems.Quantity * TransStockPickingListItems.ActualQuantity) AS Quantity," & _
            "LibraryStockInformation.WarehouseID," & _
            "LibraryWarehouses.WarehouseHeadID " & _
            ",TransStockPickingListItems.UnitMeasure" & _
            ",TransStockPickingListItems.Dimension" & _
            " From TransStockPickingListItems INNER JOIN " & _
            " LibraryStockInformation ON TransStockPickingListItems.StockID = LibraryStockInformation.ID  INNER JOIN " & _
            " LibraryWarehouses ON LibraryStockInformation.WarehouseID = LibraryWarehouses.ID " & _
            " WHERE (TransStockPickingListItems.StockPickingListID = '" & vPickingListID & "') " & _
            " GROUP BY TransStockPickingListItems.StockID," & _
            " LibraryStockInformation.WarehouseID," & _
            " LibraryWarehouses.WarehouseHeadID " & _
            ",TransStockPickingListItems.UnitMeasure" & _
            ",TransStockPickingListItems.Dimension"
        pDtItems = mProData.Execute(pSql)


        For Each pDrItem In pDtItems.Rows
            pTotalQuantity = pDrItem("Quantity")
            pSql = "SELECT LibraryStockMeasurements.MeasurementLevel" & _
                ",LibraryStockMeasurements.Quantity" & _
                ",LibraryUnitMeasures.UnitMeasure" & _
                " FROM LibraryStockMeasurements" &
                " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID " & _
                " WHERE LibraryStockMeasurements.StockID = " & ReadyString(pDrItem("StockID")) &
                " ORDER BY LibraryStockMeasurements.Quantity DESC, LibraryStockMeasurements.MeasurementLevel"
            pDtMeasurements = mProData.Execute(pSql)
            If pDtMeasurements.Rows.Count > 0 Then
                pBasedMeasure = pDtMeasurements.Rows(pDtMeasurements.Rows.Count - 1)("UnitMeasure") & ""
                For Each pDrMeasurement In pDtMeasurements.Rows
                    '''Get the quantity for unit measure X
                    pQuantity = Fix(pTotalQuantity / pDrMeasurement("Quantity"))
                    pTotalQuantity -= pQuantity * pDrMeasurement("Quantity")
                    If pQuantity > 0 Then
                        pIU("StockPickingListID") = ReadyString(vPickingListID)
                        pIU("MeasurementLevel") = pDrMeasurement("MeasurementLevel")
                        pIU("StockID") = ReadyString(pDrMeasurement("StockID"))
                        pIU("Quantity") = pQuantity
                        pIU("ActualQuantity") = pDrMeasurement("Quantity")
                        pIU("UnitMeasure") = ReadyString(pDrMeasurement("UnitMeasure"))
                        pIU("BasedUnitMeasure") = ReadyString(pBasedMeasure)
                        pIU("WarehousePersonnel") = pDrMeasurement("WarehouseHeadID")
                        If pDrItem("Dimension") Is DBNull.Value Then
                            pIU("Dimension") = "NULL"
                        Else
                            pIU("Dimension") = ReadyString(pDrItem("Dimension"))
                        End If
                        pSql = pIU.GetInsert
                        mProData.ExecuteSQL(pSql)
                    End If
                Next pDrMeasurement
            End If
        Next pDrItem

    End Sub
#End Region

    ''' <summary>
    ''' compares items from sales order and it's corresponding uninvoiced sorting list.
    ''' Returns true if sorted qty is the same with ordered qty and has no backorder nor alternative
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsPerfectlySorted(vID As String,
        Optional ByRef oLineCount As Integer = 0,
        Optional ByRef oSortedLineCount As Integer = 0) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT StockID" & vbCrLf & _
            " ,SUM(Quantity * ActualQuantity) AS Qty" & vbCrLf & _
            " ,ISNULL((SELECT SUM(Quantity * ActualQuantity) AS Qty" & vbCrLf & _
            " 	FROM TransStockSortingListItems" & vbCrLf & _
            "     WHERE TransStockSortingListItems.SalesOrderID = TransSalesOrderItems.SalesOrderID" & vbCrLf & _
            "     AND TransStockSortingListItems.StockID = TransSalesOrderItems.StockID" & vbCrLf & _
            "     AND TransStockSortingListItems.IsInvoiced = 0" & vbCrLf & _
            "     AND TransStockSortingListItems.IsHasBackOrder = 0" & vbCrLf & _
            "     AND TransStockSortingListItems.IsHasReplacement = 0" & vbCrLf & _
            "     AND TransStockSortingListItems.IsReplacement = 0" & vbCrLf & _
            " ), 0) AS SortedQty" & vbCrLf & _
            " FROM TransSalesOrderItems" & vbCrLf & _
            " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(vID) & vbCrLf & _
            " GROUP BY StockID" & vbCrLf & _
            " ,TransSalesOrderItems.SalesOrderID" & vbCrLf
        pDT = mProData.Execute(pSql)
        oLineCount = pDT.Rows.Count
        pDT.DefaultView.RowFilter = "Qty = SortedQty"
        oSortedLineCount = pDT.DefaultView.Count
        If oLineCount <> oSortedLineCount Then Return False

        '// determine if the sorted list came from different picking lists
        pSql = "SELECT TransStockSortingList.StockPickingListID" & vbCrLf & _
            " FROM TransStockSortingListItems" & vbCrLf & _
            " JOIN TransStockSortingList ON TransStockSortingList.ID = TransStockSortingListItems.StockSortingListID" & vbCrLf & _
            " WHERE TransStockSortingListItems.SalesOrderID = " & ReadyString(vID) & vbCrLf & _
            " GROUP BY TransStockSortingList.StockPickingListID" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 1 Then Return False

        '// this is the easiest condition but this is the last coz we still have to compute for oLineCount and oSortedLineCount
        pSql = "SELECT TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.SalesOrderID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then Return False

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vExcludeSetPerfectlySorted">only records with TransSalesOrder.PerfectlySorted null</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InvoiceableOrders(Optional ByVal vExcludeSetPerfectlySorted As Boolean = False,
        Optional vPerfectlySortedOnly As Boolean = False) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        If vExcludeSetPerfectlySorted Then pSql = " AND TransSalesOrder.PerfectlySorted IS NULL"
        If vPerfectlySortedOnly Then pSql &= " AND TransSalesOrder.PerfectlySorted = 1"

        pSql = "SELECT DISTINCT TransStockSortingList.ID" & _
            ",TransStockSortingListItems.SalesOrderID" & _
            ",TransSalesOrder.CustomerID" & _
            ",TransSalesOrder.OrderNo" & _
            ",TransSalesOrder.OrderDate" & _
            ",LibraryCustomers.CompanyName" & _
            ",LibraryCustomers.IsCompanyBranch" &
            ",TransSalesOrder.PerfectlySorted" &
            ",ISNULL(TransSalesOrder.DontAutoInvoice, 0) AS DontAutoInvoice" &
            " FROM TransStockSortingListItems" & _
            " WITH (TABLOCKX)" & _
            " JOIN TransStockSortingList ON TransStockSortingList.ID = TransStockSortingListItems.StockSortingListID" & _
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransStockSortingListItems.SalesOrderID" & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" & _
            " WHERE TransSalesOrder.IsCancelled = 0" & _
            " AND TransStockSortingListItems.IsInvoiced = 0" & _
            " AND TransSalesOrder.OrderDate >= '7/15/2011'" &
            " AND ISNULL(TransSalesOrder.InStore, 0) = 0" &
        pSql
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub DetermineAndTagPerfectlySorted()

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pPerfectlySorted As Boolean
        Dim pLineCount As Integer
        Dim pSortedLineCount As Integer

        pDT = InvoiceableOrders(True)
        For Each pDR In pDT.Rows
            pPerfectlySorted = IsPerfectlySorted(CStr2(pDR("SalesOrderID")), pLineCount, pSortedLineCount)
            pSql = "UPDATE TransSalesOrder" &
                " SET PerfectlySorted = " & If(pPerfectlySorted, 1, 0) &
                ",PerfectlySortedDate = GETDATE()" &
                ",LineCount = " & pLineCount &
                ",SortedLineCount = " & pSortedLineCount &
                " WHERE ID = " & ReadyString(CStr2(pDR("SalesOrderID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Sub

    ''' <summary>
    ''' used to save summary (CacheStockSalesDaily)
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupItems(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransSalesOrderItems.StockID" & vbCrLf & _
            " ,SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Qty" & vbCrLf & _
            " ,SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity * TransSalesOrderItems.Cost) AS Cost" & vbCrLf & _
            " FROM TransSalesOrderItems" & vbCrLf & _
            " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(vID) & vbCrLf & _
            " GROUP BY TransSalesOrderItems.StockID" & vbCrLf &
            ",TransSalesOrderItems.Cost"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub EditSummary(vID As String, vAdd As Boolean)

        Dim pDT As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pDrOrder As DataRow

        pDrOrder = ByID(vID)
        If pDrOrder Is Nothing Then Exit Sub

        pIU = New InsertUpdate("DrpCacheStockSalesDaily")
        pDT = GetGroupItems(vID)
        For Each pDR As DataRow In pDT.Rows
            pIU.Reset()
            If pDrOrder("IsCompanyBranch") Then
                pIU("QtyInternal") = "QtyInternal " & If(vAdd, "+", "-") & pDR("Qty")
            Else
                pIU("QtyExternal") = "QtyExternal " & If(vAdd, "+", "-") & pDR("Qty")
            End If
            pIU("COGS") = "COGS " & If(vAdd, "+", "-") & pDR("Cost")

            pSql = pIU.GetUpdate("StockID = " & ReadyString(pDR("StockID")) &
                " AND TransDate = " & ReadyString(CDate(pDrOrder("LastUpdated")).Date))
            mProData.ExecuteSQL(pSql)
            If CDec2(mProData.ExecuteScalar("SELECT @@ROWCOUNT")) = 0 Then
                pIU("ID") = "newid()"
                pIU("StockID") = ReadyString(pDR("StockID"))
                pIU("TransDate") = ReadyString(CDate(pDrOrder("LastUpdated")).Date)
                If pDrOrder("IsCompanyBranch") Then
                    pIU("QtyInternal") = pDR("Qty")
                    pIU("QtyExternal") = 0
                Else
                    pIU("QtyInternal") = 0
                    pIU("QtyExternal") = pDR("Qty")
                End If
                pIU("COGS") = pDR("Cost")
                pIU("WeekNum") = WeekNumber(CDate(pDrOrder("LastUpdated")).Date)
                pIU("WeekYear") = CDate(pDrOrder("LastUpdated")).Year
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Next pDR

    End Sub

    Public Function GetCartStock(vOrderID As String, vStockID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SUM(TransSalesOrderItems.Quantity) AS Quantity" & vbCrLf & _
            ",TransSalesOrderItems.ActualQuantity" & vbCrLf &
            ",MAX(TransSalesOrderItems.RequestedUnitPrice) AS RequestedUnitPrice" & vbCrLf &
            " FROM TransSalesOrderItems" & vbCrLf & _
            " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(vOrderID) & vbCrLf & _
            " AND TransSalesOrderItems.StockID = " & ReadyString(vStockID) & vbCrLf & _
            " GROUP BY TransSalesOrderItems.ActualQuantity" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetMaxLineNumber(vOrderID As String) As Integer

        Dim pSql As String

        pSql = "SELECT MAX(TransSalesOrderItems.LineNumber) AS LineNumber" & vbCrLf & _
            " FROM TransSalesOrderItems" & vbCrLf & _
            " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(vOrderID)

        Return CDec2(mProData.ExecuteScalar(pSql))

    End Function

    Public Sub DeleteOrderStock(vOrderID As String, vStockID As String)

        Dim pSql As String

        pSql = "DELETE FROM TransSalesOrderItems" & vbCrLf & _
            " WHERE TransSalesOrderItems.SalesOrderID = " & ReadyString(vOrderID) & vbCrLf & _
            " AND TransSalesOrderItems.StockID = " & ReadyString(vStockID) & vbCrLf
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function IsValidCart(vID As String, Optional vCustomerID As String = Nothing,
        Optional ByRef oMessage As String = Nothing) As Boolean

        Dim pDR As DataRow
        Dim pSql As String
        Dim pUser As String

        oMessage = ""
        pSql = "SELECT TransSalesOrder.OrderNo" & vbCrLf &
            ",IsHold" &
            ",IsCancelled" &
            ",CustomerID" &
            ",EditingUserID" & vbCrLf &
            ",tblEditor.LastName + ', ' + tblEditor.FirstName AS EditorName" & vbCrLf &
            " FROM TransSalesOrder" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEditor ON tblEditor.ID = TransSalesOrder.EditingUserID" & vbCrLf &
            " WHERE TransSalesOrder.ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            If pDR("IsCancelled") Then
                oMessage = "Sales Order " & pDR("OrderNo") & " was cancelled"
                Return False
            End If
            If pDR("IsHold") = False Then
                oMessage = "Sales Order " & pDR("OrderNo") & " was already checked out"
                Return False
            End If
#If _MYTYPE = "Web" Then
            pUser = WebSession.Instance.UserID
#End If
            If CStr2(pDR("EditingUserID")).Length > 0 _
                AndAlso CStr2(pDR("EditingUserID")) <> CStr2(pUser) Then

                oMessage = pDR("EditorName") & " is currently editing Sales Order " & pDR("OrderNo") & ". You may not edit this sales order at the moment. Only 1 user is allowed to edit a sales order at a time. Please try again later."
                Return False
            End If
            If CStr2(vCustomerID).Length > 0 Then
                If ReadyGuid(vCustomerID) <> ReadyGuid(pDR("CustomerID")) Then
                    oMessage = "Sales Order " & pDR("OrderNo") & " is from a different customer than the one you're expecting"
                    Return False
                End If
            End If
            If GetPickingList(vID).Rows.Count > 0 Then
                oMessage = "Sales Order " & pDR("OrderNo") & " already has a picking list"
                Return False
            End If
            If IsInvoiced(vID) Then
                oMessage = "Sales Order " & pDR("OrderNo") & " is already cancelled"
                Return False
            End If
            Return True
        Else
            oMessage = "Sales order does not exist"
        End If

    End Function

    Public Function LegacyGetInvoiceTypeIDByAbbrevation(vAbbrevation As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        If CStr2(vAbbrevation).Trim() = "" Then
            Return Nothing
        End If

        pSql = "SELECT ID" & vbCrLf &
            " FROM wholesale.dbo.MainInvoiceDefaults" & vbCrLf &
            " WHERE Abbrevation = " & ReadyString(vAbbrevation) & vbCrLf
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

#If _MYTYPE = "WindowsForms" Then

    Public Function AutoCreateInvoices(ByRef oError As String) As DataTable

        Dim pDT As DataTable
        Dim pDR As DataRow

        Dim pDtOut As DataTable
        Dim pInvoiceDefaultID, pBranchInvoiceDefaultId As String
        'Dim pInvoiceType, pBranchInvoiceType As String

        Try
            DetermineAndTagPerfectlySorted()
            mProData.BeginTransaction()
            pDT = InvoiceableOrders()
            pDT = New DataView(pDT, "DontAutoInvoice = 0 AND (IsCompanyBranch = 1 OR PerfectlySorted = 1)", "", DataViewRowState.CurrentRows).ToTable(True, "ID", "SalesOrderID", "OrderNo", "CustomerID", "CompanyName", "IsCompanyBranch", "PerfectlySorted")
            If pDT.Rows.Count = 0 Then
                mProData.RollbackTransaction()
                Return Nothing
            End If

            mParent.SystemDefaults.InvoiceTypeForCustomerAutoInvoice(pInvoiceDefaultID, "", oError)
            mParent.SystemDefaults.InvoiceTypeForBranchAutoInvoice(pBranchInvoiceDefaultId, "", oError)

            pDtOut = New DataTable
            With pDtOut.Columns
                .Add("ID", GetType(Guid))
                .Add("InvoiceNum", GetType(Integer))
                .Add("CompanyName")
                .Add("InvoiceNoExtension")
                .Add("OrderNo", GetType(Integer))
            End With

            For Each pDR In pDT.Rows
                AutoCreateInvoice(oError, CStr2(pDR("ID")), CStr2(pDR("SalesOrderID")), If(pDR("IsCompanyBranch"), pBranchInvoiceDefaultId, pInvoiceDefaultID), True, pBranchInvoiceDefaultId)
            Next pDR
            mProData.CommitTransaction()
            Return pDtOut
        Catch ex As Exception
            oError = ex.Message
            Try
                mProData.RollbackTransaction()
            Catch ex2 As Exception
                Stop2()
            End Try
        End Try

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oError"></param>
    ''' <param name="vStockSortingListID"></param>
    ''' <param name="vSalesOrderID"></param>
    ''' <param name="vInvoiceDefaultID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AutoCreateInvoice(ByRef oError As String,
        vStockSortingListID As String,
        vSalesOrderID As String,
        vInvoiceDefaultID As String,
        vAllowNoStocks As Boolean,
        vNoStockInvoiceDefaultID As String) As DataRow

        Dim pCreator As TransInvoice.Creator
        Dim pAbbrevation As String
        Dim pDrOrder As DataRow
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        If mParent.TransInvoice.GetInvoiceType(vInvoiceDefaultID, "", pAbbrevation) = False Then
            Throw New ApplicationException("Non-existing invoice default ID: " & vInvoiceDefaultID)
            Return Nothing
        End If

        pDrOrder = ByID(vSalesOrderID)
        If pDrOrder Is Nothing Then
            Throw New ApplicationException("Non-existing sales order ID: " & vSalesOrderID)
            Return Nothing
        End If

        Try
            pCreator = New TransInvoice.Creator(mProData, mParent.TransInvoice)
            pCreator.StockSortingListID = vStockSortingListID
            If pCreator.GetStocks() Then ' set StockSortingListID before calling this
                pCreator.InvoiceDefaultID = vInvoiceDefaultID
            Else
                pCreator.InvoiceNumber = 0
                If vAllowNoStocks = False Then Return Nothing
                pCreator.InvoiceDefaultID = vNoStockInvoiceDefaultID
            End If
            pCreator.InvoiceType = pAbbrevation ' not really needed to save but needed to display in log
            pCreator.InvoiceDate = mProData.GetDate.Date
            pCreator.SalesOrderID = vSalesOrderID
            pCreator.CustomerID = CStr2(pDrOrder("CustomerID"))
            pCreator.BillingAddressID = CStr2(pDrOrder("CustomerAddressID"))
            pCreator.DeliveryAddressID = CStr2(pDrOrder("DeliverToID"))
            pCreator.Remarks = Truncate(pDrOrder("Remarks") & ". Auto-created invoice", 999)
            pCreator.UserID = User.EmployeeID
            pCreator.EncodedBy = modClient.EmployeeWebID
            pCreator.InvoiceDateTime = mProData.GetDate
            If pCreator.Save(True, oError) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
        Catch ex As System.Exception
            Throw
        End Try

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID", GetType(Guid))
            .Add("InvoiceNum", GetType(Integer))
            .Add("CompanyName")
            .Add("InvoiceTypeAbbrevation")
            .Add("OrderNo", GetType(Integer))
            .Add("InvoiceDefaultID")
        End With

        pDrOut = pDtOut.NewRow
        pDrOut("ID") = pCreator.ID
        pDrOut("InvoiceNum") = pCreator.InvoiceNumber
        pDrOut("CompanyName") = pDrOrder("CompanyName")
        pDrOut("OrderNo") = pDrOrder("OrderNo")
        pDrOut("InvoiceTypeAbbrevation") = pAbbrevation
        pDrOut("InvoiceDefaultID") = vInvoiceDefaultID
        pDtOut.Rows.Add(pDrOut)

        Return pDrOut

    End Function
#End If

    Public Function SaveRemarks(vID As String, vRemarks As String) As Boolean

        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("TransSalesOrder")
        pIU("Remarks") = ReadyString(vRemarks)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pSql = mProData.ExecuteSQL(pSql, True)

        Return CDec(pSql) > 0

    End Function

    Public Sub LockSalesOrder(vID As String)

        Dim pUserID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

#If _MYTYPE = "Web" Then
        pUserID = WebSession.Instance.UserID
#End If
        pIU = New InsertUpdate("TransSalesOrder")
        pIU("EditingUserID") = CDec2(pUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub UnlockSalesOrder(vID As String)

        Dim pUserID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransSalesOrder")
        pIU("EditingUserID") = "null"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    ''' <summary>
    ''' wholesale database
    ''' </summary>
    ''' <param name="vMobizInvoiceID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LegacyGetByMobizInvoiceID(vMobizInvoiceID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" &
            ",IsHold" &
            " FROM TransSalesOrder" &
            " WHERE MobizInvoiceID = " & ReadyString(vMobizInvoiceID)
        Return mProData.ExecuteRow(pSql)

    End Function
End Class

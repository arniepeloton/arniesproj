Imports System.Linq

Public Class TransReceiving

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetItems(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ProductID" &
            ",QuantityGood" &
            ",ActualQuantityGood" &
            ",TransReceivingStock.ID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " FROM TransReceivingStock" &
            " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransReceivingStock.ProductID" & vbCrLf &
            " WHERE ReceivingID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function NonZeroItemCount(ByVal vID As String) As Integer

        Dim pOut As Integer
        Dim pSql As String

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM TransReceivingStock" & vbCrLf &
            " WHERE ReceivingID = " & ReadyString(vID) & vbCrLf &
            " AND (QuantityGood > 0" & vbCrLf &
            " OR QuantityDamaged > 0)" & vbCrLf
        pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function ByID(vID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT TransReceiving.ReceivingPlaceID" &
            ",TransReceiving.InvoiceID" &
            ",TransReceiving.ReceivingNo" &
            ",TransReceiving.SupplierID" &
            ",TransReceiving.IsReceiving" &
            ",TransReceiving.UserID" &
            ",TransReceiving.LastUpdated" &
            ",TransReceiving.BillingAddressID" &
            ",TransReceiving.DeliveryAddressID" &
            ",TransReceiving.ReceivingPlaceID" &
            ",TransReceiving.DeviceID" &
            ",TransReceiving.DateReceived" &
            ",TransReceiving.PurchaseOrderID" &
            ",TransReceiving.ReceiveFromOutlet" &
            ",TransReceiving.Total" &
            ",TransReceiving.IsRefund" &
            ",ISNULL(TransReceiving.IsCancelled, 0) AS IsCancelled" &
            ",TransReceiving.SalesRepText" &
            ",TransReceiving.ReceiveInDepartment" &
            ",TransReceiving.ReceiveFromDepartment" &
            ",TransInvoice.PaymentTerm" &
            " FROM TransReceiving" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.ID = TransReceiving.PurchaseOrderID" & vbCrLf &
            " WHERE TransReceiving.ID = " & ReadyString(vID)

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Sub SaveAccountsPayableID(vReceivingID As String, vAccountsPayableID As String)

        Dim pIU = New InsertUpdate("TransReceiving")
        pIU("AccountsPayableID") = ReadyString(vAccountsPayableID)
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vReceivingID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetByAccountsPayableID(vAccountsPayableID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            ",TransReceiving.SupplierID" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " WHERE AccountsPayableID = " & ReadyString(vAccountsPayableID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function AddToInventoryLedger(ByVal vID As String,
        Optional ByVal IsHasPossibleReconstruction As Boolean = False, Optional vIsCancel As Boolean = False,
        Optional vOrgID As String = Nothing)

        Dim pDrReceiving As DataRow
        Dim pItems As DataTable
        Dim pQty As Decimal
        Dim pMultiplier As Integer = 1

        If vIsCancel Then
            pMultiplier = -1
        End If

        mProData.BeginTransaction()
        Try
            pDrReceiving = ByID(vID)
            pItems = GetItems(vID)
            For Each pDR As DataRow In pItems.Rows
                If CDec2(pDR("QuantityGood")) = 5184000000 Then
                    AppendToLog("vb2RRL3FKDT7! QuantityGood = " & CDec2(pDR("QuantityGood")))
                    pDR("QuantityGood") = 1
                End If

                If CDec2(pDR("ActualQuantityGood")) = 373248000000000 _
                    OrElse CDec2(pDR("ActualQuantityGood")) = 70000000 Then

                    AppendToLog("vbG5M25KWHX3! ActualQuantityGood = " & CDec2(pDR("QuantityGood")))
                    pDR("ActualQuantityGood") = 1
                End If

                Dim pQtyInBaseUnit = CDec2(pDR("QuantityGood")) * CDec2(pDR("ActualQuantityGood"))
                If pQtyInBaseUnit > 10000000 Then
                    AppendToLog("vbSL5KKUAKB8! pQtyInBaseUnit=" & pQtyInBaseUnit & vbCrLf &
                        "ProductID=" & CStr2(pDR("ProductID")) & vbCrLf &
                        "QuantityGood=" & CDec2(pDR("QuantityGood")) & vbCrLf &
                        "ActualQuantityGood=" & CDec2(pDR("ActualQuantityGood")))
                    If CDec2(pDR("QuantityGood")) > CDec2(pDR("ActualQuantityGood")) Then
                        pDR("QuantityGood") = 1
                    Else
                        pDR("ActualQuantityGood") = 1
                    End If
                End If

                'If ReadyGuid(pDR("ID")) = ReadyGuid("9f8a3070-4720-4a28-b2cb-9f7af5fa0d0a") Then
                '    AppendToLog("vb2SNME8V375! QuantityGood = " & CDec2(pDR("QuantityGood")))
                'End If
                pQty = CDec2(pDR("QuantityGood")) * CDec2(pDR("ActualQuantityGood")) * pMultiplier

                Dim TransactionBalanceBeforeCount As String
                Dim pDataNeedsReconstruction As Boolean
                Dim pSql As String
                Dim dtTransactionAfterCount As DataTable

                If IsHasPossibleReconstruction Then
                    Dim pOut As String
                    Dim pInventoryCountDate As String

                    pSql = "SELECT BinID" &
                            " FROM LibraryFowstBinProduct" &
                            " WHERE FowstID = " & ReadyString(pDrReceiving("ReceivingPlaceID")) &
                            " AND ProductID = " & ReadyString(pDR("ProductID")) &
                            " AND IsDefault = 1 "
                    pOut = CStr2(mProData.ExecuteScalar(pSql))

                    If pOut <> "" Then
                        pSql = "Select top 1 InventoryCountDate from TransInventoryProductCountStock " & vbCrLf &
                                "INNER JOIN TransInventoryProductCount " & vbCrLf &
                                "ON TransInventoryProductCountStock.InventoryProductCountID = TransInventoryProductCount.ID " & vbCrLf &
                                "WHERE " & vbCrLf &
                                "TransInventoryProductCount.FowstID = " & ReadyString(pDrReceiving("ReceivingPlaceID")) & vbCrLf &
                                "AND TransInventoryProductCountStock.BinID = " & ReadyString(pOut) & vbCrLf &
                                "AND TransInventoryProductCount.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                                " Order by InventoryCountDate desc"

                        pInventoryCountDate = CStr2(mProData.ExecuteScalar(pSql))

                        If IsDate(pDrReceiving("LastUpdated")) = False Then
                            Throw New ApplicationException(CStr2(pDrReceiving("LastUpdated")) & " is not a valid date. ref: vb8CDMZU8YTU!")
                        End If
                        If IsDate(pInventoryCountDate) = False Then
                            pInventoryCountDate = Now
                            'Throw New ApplicationException(pInventoryCountDate & " is not a valid date. ref: vbWS7WNYZZJL!")
                        End If
                        If (DateDiff(DateInterval.Second, CDate(pDrReceiving("LastUpdated")), CDate(pInventoryCountDate)) > 0) Then
                            'pDataNeedsReconstruction = True

                            pSql = "Select * from TransInventoryLedger " & vbCrLf &
                                    "where LastUpdated >= " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                                    "AND TransInventoryLedger.FowstID = " & ReadyString(pDrReceiving("ReceivingPlaceID")) & vbCrLf &
                                    "AND TransInventoryLedger.BinID = " & ReadyString(pOut) & vbCrLf &
                                    "AND TransInventoryLedger.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                                    " ORDER BY LastUpdated asc"
                            dtTransactionAfterCount = mProData.Execute(pSql)

                            pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                                    "where LastUpdated < " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                                    "AND TransInventoryLedger.FowstID = " & ReadyString(pDrReceiving("ReceivingPlaceID")) & vbCrLf &
                                    "AND TransInventoryLedger.BinID = " & ReadyString(pOut) & vbCrLf &
                                    "AND TransInventoryLedger.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                                    " ORDER BY LastUpdated desc"

                            TransactionBalanceBeforeCount = mProData.ExecuteScalar(pSql)

                        End If

                    End If

                End If

                If pDataNeedsReconstruction Then
                    mParent.TransInventoryLedger.Add(
                    vStockID:=CStr2(pDR("ProductID")),
                    vFowstID:=CStr2(pDrReceiving("ReceivingPlaceID")),
                    vTransactionID:=vID,
                    vTransactionType:=If(CDec2(pDrReceiving("IsReceiving")), TransInventoryLedger.eTransType.Receiving, TransInventoryLedger.eTransType.TransferOut),
                    vUserID:=CStr2(pDrReceiving("UserID")),
                    vQty:=If(CDec2(pDrReceiving("IsReceiving")), pQty, -pQty),
                    vBalance:=TransactionBalanceBeforeCount + If(CDec2(pDrReceiving("IsReceiving")), pQty, -pQty)
                )
                    If dtTransactionAfterCount.Rows.Count > 0 Then
                        For i As Integer = 0 To dtTransactionAfterCount.Rows.Count - 1
                            Dim mTransType As TransInventoryLedger.eTransType

                            Select Case CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionType"))
                                Case "Invoice"
                                    mTransType = TransInventoryLedger.eTransType.Invoice
                                Case "InventoryCount"
                                    mTransType = TransInventoryLedger.eTransType.InventoryCount
                                Case "Receiving"
                                    mTransType = TransInventoryLedger.eTransType.Receiving
                                Case "TransferOut"
                                    mTransType = TransInventoryLedger.eTransType.TransferOut
                                Case "Sales Returns"
                                    mTransType = TransInventoryLedger.eTransType.SalesReturn
                                Case "Product Maker"
                                    mTransType = TransInventoryLedger.eTransType.SalesReturn
                            End Select

                            If mTransType <> TransInventoryLedger.eTransType.InventoryCount Then
                                mParent.TransInventoryLedger.Add(
                                vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                                vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                                vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                                vTransactionType:=mTransType,
                                vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                                vQty:=CDec2(dtTransactionAfterCount.Rows(i).Item("Qty"))
                            )
                            Else
                                mParent.TransInventoryLedger.Add(
                                vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                                vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                                vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                                vTransactionType:=mTransType,
                                vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                                vQty:=Nothing,
                                vBalance:=CDec2(dtTransactionAfterCount.Rows(i).Item("Balance"))
                            )
                            End If

                        Next
                    End If
                Else
                    Dim pTransactionType As TransInventoryLedger.eTransType
                    Dim pFowstID = CStr2(pDrReceiving("ReceivingPlaceID"))
                    If vIsCancel Then
                        pTransactionType = TransInventoryLedger.eTransType.CancelReceiving
                    ElseIf CBool2(pDrReceiving("IsReceiving")) Then
                        pTransactionType = TransInventoryLedger.eTransType.Receiving

                    Else
                        pTransactionType = TransInventoryLedger.eTransType.TransferOut
                        If CStr2(pDrReceiving("ReceiveFromOutlet")).Trim() <> "" Then ' 2024_01_16__12_19_43_3479. dapat unconditional ni. ki-condition lang para maka flush
                            pFowstID = CStr2(pDrReceiving("ReceiveFromOutlet"))
                        End If
                    End If

                    Dim pSubTransactionType As String
                    If CStr2(pDrReceiving("InvoiceID")) <> "" Then
                        If pTransactionType = TransInventoryLedger.eTransType.Receiving Then
                            pSubTransactionType = "Payment"
                        ElseIf pTransactionType = TransInventoryLedger.eTransType.TransferOut Then
                            If CBool2(pDrReceiving("IsRefund")) Then
                                pSubTransactionType = "Refund"
                            Else
                                pSubTransactionType = "Change"
                            End If
                        End If
                    End If

#If DEBUG Then
                    If ReadyGuid(vOrgID) = modClient.DemoCorpOrgID _
                        AndAlso ReadyGuid(pDR("ProductID")) = ReadyGuid("03B1092A-CECE-4916-999B-4DDD8F2DD696") Then
#Else
                    If ReadyGuid(vOrgID) = ReadyGuid(modClient.CincoOrgID) _
                        AndAlso ReadyGuid(pDR("ProductID")) = ReadyGuid("d4d5277f-aba0-407c-8f6d-43c6fae15c5b") Then
#End If
                        mParent.TransInvoice.AddItemToInventoryLedger(
                            vProductID:=CStr2(pDR("ProductID")),
                            vOrgID:=vOrgID,
                            vFowstID:=pFowstID,
                            vTransactionID:=vID,
                            vUserID:=CStr2(pDrReceiving("UserID")),
                            vQty:=If(CDec2(pDrReceiving("IsReceiving")), pQty, -pQty),
                            vTransType:=pTransactionType,
                            vSubTransactionType:=pSubTransactionType
                        )
                    Else

                        mParent.TransInventoryLedger.Add(
                            vStockID:=CStr2(pDR("ProductID")),
                            vFowstID:=pFowstID,
                            vTransactionID:=vID,
                            vTransactionType:=pTransactionType,
                            vUserID:=CStr2(pDrReceiving("UserID")),
                            vSubTransactionType:=pSubTransactionType,
                            vBinID:=CStr2(pDrReceiving("ReceiveInDepartment")),
                            vQty:=If(CDec2(pDrReceiving("IsReceiving")), pQty, -pQty)
                        )
                    End If
                End If
            Next pDR
            mProData.CommitTransaction()
        Catch ex As System.Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Function

    Public Sub UpdateCounter(ByVal vDeviceID As String, ByVal vFowstID As String, ByVal vValue As Integer, vIsReceiving As Boolean)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vValue <= 0 Then Return

        If vIsReceiving Then
            pSql = "ReceivingNum"
        Else
            pSql = "ReceivingTransferNum"
        End If
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU(pSql) = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ISNULL(" & pSql & ", 0) < " & vValue)
        pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)

    End Sub

    Public Function CreateFromInvoice(vInvoiceID As String, ByRef oError As String, vSupplierOrgID As String) As Boolean

        Dim pDsInvoice As DataSet
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDtInvoice As DataTable
        Dim pDrInvoice As DataRow
        Dim pDrCustomer As DataRow
        Dim pReceivingID As String

        oError = Nothing
        Try
            mProData.BeginTransaction()
            pDsInvoice = mParent.TransInvoice.GetCart(vInvoiceID)
            pDtInvoice = pDsInvoice.Tables("Invoices")
            If pDtInvoice.Rows.Count = 0 Then
                oError = "Invoice query for invoice " & vInvoiceID & " returns no result"
                Return False
            End If
            pDrInvoice = pDtInvoice.Rows(0)
            pDrCustomer = mParent.LibraryOrganization.ByID(CStr2(pDrInvoice("CustomerID")))
            If pDrCustomer Is Nothing Then
                oError = "Organization query for customer " & CStr2(pDrInvoice("CustomerID")) & " returns no result"
                Return False
            End If
            If pDrCustomer("Database") Is DBNull.Value Then
                oError = "Organization query for customer " & CStr2(pDrInvoice("CustomerID")) & " returns no database"
                mProData.RollbackTransaction(, True)
                Return True
            End If
            mProData.ExecuteSQL("USE " & EscapeSqlName(pDrCustomer("Database")))

            pSql = "SELECT MAX(ReceivingNo)" &
                            " FROM TransReceiving" &
                            " WHERE IsReceiving = 0 AND DeviceID = " & ReadyString(pDrInvoice("DeviceID")) & vbCrLf &
                            " AND ReceivingPlaceID = " & If(pDrInvoice("FowstID") Is DBNull.Value, "null", ReadyString(CStr2(pDrInvoice("FowstID"))))

            pIU = New InsertUpdate("TransReceiving")
            pReceivingID = Guid.NewGuid.ToString
            pIU("ID") = ReadyString(pReceivingID)
            pIU("ReceivingNo") = "ISNULL((" & pSql & "), 0) + 1"
            pIU("IsReceiving") = 0
            pIU("ReceivingPlaceID") = If(pDrInvoice("DeliveryAddressID") Is DBNull.Value, "null", ReadyString(CStr2(pDrInvoice("DeliveryAddressID"))))
            pIU("SupplierID") = ReadyString(vSupplierOrgID)
            pIU("DocumentDate") = ReadyString(Now)
            pIU("DocumentType") = ReadyString("")
            pIU("DocumentNo") = ReadyString("")
            pIU("Is3PL") = 0
            pIU("IsResaleable") = 0
            pIU("ResaleableReason") = ReadyString("")
            pIU("UserID") = ReadyString(pDrInvoice("UserID"))
            pIU("DeviceID") = "NULL" ' ReadyString(pDrInvoice("DeviceID")) ' todo: what device id?
            pIU("TransportedBy") = ReadyString("")
            pIU("LastUpdated") = ReadyString(Now)
            pIU("HasIssues") = 0
            pIU("IssueRemarks") = "null"
            pIU("PONo") = ReadyString("")
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)

            For Each pDrProduct As DataRow In pDsInvoice.Tables("Stocks").Rows
                pIU = New InsertUpdate("TransReceivingStock")
                'pIU("ID") = ReadyString(pDrProduct("ID"))
                pIU("ReceivingID") = ReadyString(pReceivingID)
                pIU("ProductID") = ReadyString(pDrProduct("ProductID"))
                pIU("QuantityGood") = CDec2(pDrProduct("Quantity"))
                pIU("ActualQuantityGood") = CDec2(pDrProduct("ActualQuantity"))
                pIU("UnitMeasureGood") = ReadyString(pDrProduct("UnitMeasureID"))
                pIU("IsResaleable") = 1
                pIU("UnitMeasureDamaged") = pIU("UnitMeasureGood")

                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            Next pDrProduct

            mProData.CommitTransaction()

            Return True
        Catch ex As Exception
            oError = ex.Message
            mProData.RollbackTransaction(, True)
        End Try

    End Function

    Public Sub SetDontServeBackOrder()

        Dim pSql As String

        pSql = "UPDATE TransReceiving" & vbCrLf &
            " SET DontServeBackOrder = 1" & vbCrLf &
            " WHERE ISNULL(DontServeBackOrder, 0) = 0"
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Class StockInfo
        Public ProductID As String
        Public ProductName As String
        Public ProductNumber As String
        Public BaseUnit As String
        Public BaseUnitQty As Decimal
        Public Units As New List(Of Unit)

        Public Class Unit
            Public ReceivingStockID ' TransReceivingStock.ID
            Public MeasureID As String ' MobizLibraryPIMMeasure.ID
            Public UnitMeasure As String
            Public UnitQuantity As Decimal ' MobizLibraryPIMProductMeasurement.Quantity
            Public QtyPo As Decimal
            Public QtyGood As Decimal
            Public QtyBad As Decimal
            Public PoUnitPrice As Decimal
            Public BadOrderReason As String
        End Class
    End Class

    Public Function GetStockInfo(vProductID As String, Optional vReceivingID As String = Nothing,
        Optional vReceivingStockID As String = Nothing) As StockInfo

        Dim pOut As StockInfo
        Dim pDtProduct As DataTable
        Dim pDtUnits As DataTable
        Dim pDtCart As DataTable

        vProductID = ReadyGuid(vProductID)
        vReceivingStockID = ReadyGuid(vReceivingStockID)
        pOut = New StockInfo()
        pDtProduct = mParent.LibraryStock.ByIds(ReadyString(vProductID))
        For Each pDR As DataRow In pDtProduct.Rows
            pOut.ProductID = CStr2(pDR("ID"))
            pOut.ProductName = pDR("ProductName")
            pOut.ProductNumber = CStr2(pDR("ProductNo"))
        Next pDR

        Dim pSelectedUnitMeasureID As String ' guid
        Dim pSelectedUnitMeasureQty As Decimal

        pDtCart = GetStocks(vReceivingID)
        Dim pRows =
            From pDR In pDtCart.Rows
            Where ReadyGuid(pDR("ProductID")) = ReadyGuid(vProductID)
            Select pDR
        For Each pRow In pRows
            If String.IsNullOrWhiteSpace(vReceivingStockID) Then
            Else
                If vReceivingStockID = ReadyGuid(pRow("ID")) Then
                    pSelectedUnitMeasureID = ReadyGuid(pRow("UnitMeasureGood"))
                    pSelectedUnitMeasureQty = CDec2(pRow("ActualQuantityGood"))
                End If
            End If
        Next

        pDtUnits = mParent.LibraryStockMeasurements.ByProductID(vProductID)
        'pDtUnits.Columns.Add("CartLineCount", GetType(Integer))
        'For Each pDR As DataRow In pDtUnits.Rows
        '    If pSelectedUnitMeasureQty <> 0 Then
        '        If pDR("Quantity") <> pSelectedUnitMeasureQty Then
        '            pDR("CartLineCount") = CInt2(pDR("CartLineCount")) + 1
        '        End If
        '    End If
        'Next

        For Each pDR As DataRow In pDtUnits.Rows
            If CDec2(pDR("BaseUnitQty")) = 1 Then
                pOut.BaseUnit = CStr2(pDR("BaseUnit"))
                pOut.BaseUnitQty = CDec2(pDR("BaseUnitQty"))
            End If

            Dim pUnit = New StockInfo.Unit With {
                .MeasureID = ReadyGuid(pDR("ID")),
                .UnitMeasure = CStr2(pDR("UnitMeasure")),
                .UnitQuantity = CDec2(pDR("Quantity"))
            }
            If String.IsNullOrWhiteSpace(vReceivingStockID) Then
                ' just allow this unit
            ElseIf pSelectedUnitMeasureQty = CDec2(pDR("Quantity")) Then
                pUnit.ReceivingStockID = vReceivingStockID
            Else
                Dim pCartItems = From pDrCart As DataRow In pDtCart.Rows
                                 Where CDec2(pDrCart("ActualQuantityGood")) = CDec2(pDR("Quantity")) _
                                 And ReadyGuid(pDrCart("ProductID")) = vProductID
                                 Select pDrCart

                If pCartItems.Count = 1 Then
                    pUnit.ReceivingStockID = ReadyGuid(pCartItems.First()("ID"))
                ElseIf pCartItems.Count > 1 Then
                    Continue For
                End If
            End If

            pOut.Units.Add(pUnit)
        Next pDR

        If String.IsNullOrWhiteSpace(vReceivingID) = False Then
            For Each pDR As DataRow In pDtCart.Rows
                If ReadyGuid(pDR("ProductID")) <> vProductID Then
                    Continue For
                End If

                Dim pDR_1 = pDR ' AccessToForEachVariableInClosure
                Dim pUnits =
                    From pUnit In pOut.Units
                    Where pUnit.MeasureID = ReadyGuid(pDR_1("UnitMeasureGood"))
                    Select pUnit

                For Each pUnit In pUnits
                    If String.IsNullOrWhiteSpace(pUnit.ReceivingStockID) = False Then
                        If pUnit.ReceivingStockID <> ReadyGuid(pDR("ID")) Then
                            Continue For
                        End If
                    End If

                    pUnit.PoUnitPrice = CDec2(pDR("PoUnitPrice"))
                    pUnit.QtyPo += CDec2(pDR("PoQty"))
                    pUnit.QtyGood += CDec2(pDR("QuantityGood"))
                    pUnit.QtyBad += CDec2(pDR("QuantityDamaged"))
                    If CStr2(pDR("BadOrderReason")).Trim() <> "" Then
                        pUnit.BadOrderReason = pDR("BadOrderReason")
                    End If
                Next pUnit
            Next pDR
        End If

        Return pOut

    End Function

    Public Class SaveJson
        Public Receiving As New ReceivingClass
        Public IsNew As Boolean
        Public Products As New ProductClass()
        Public SaveOnlyProduct As String
        Public OrgID As String
        Public CreatedPo As New CreatedPoClass
        Public SaveSomeFields As String ' boolean

        Public ReadOnly Property SaveOnlyProductProper As Boolean
            Get
                Return CBool2(SaveOnlyProduct) _
                    AndAlso String.IsNullOrWhiteSpace(Receiving.ID) = False
            End Get
        End Property

        Public ReadOnly Property SaveSomeFieldsProper As Boolean
            Get

                Return CBool2(SaveSomeFields)

            End Get
        End Property

        Public WriteOnly Property UUID As String
            Set(value As String)
                Receiving.UUID = value
            End Set
        End Property

        Public Class ReceivingClass
            Public ID As String
            Public UUID As String
            Public IsReceiving As String ' 1 (default) or 0
            Public DeviceID As String
            Public ReceiveInOrgID As String
            Public ReceivingPlaceID As String ' Transfer To Outlet in stock transfer
            Public BillingAddressID As String
            Public DeliveryAddressID As String
            Public SupplierID As String
            Public DocumentDate As String
            Public DocumentType As String
            Public DocumentNo As String
            Public Is3PL As String
            Public UserID As String
            Public InvoiceNumber As String
            Public InvoiceDate As String
            Public SalesRepText As String
            Public SaveAndFinalize As String
            Public Remarks As String
            Public SalesOrderNumText As String
            Public SalesOrderDate As String
            Public Total As String ' money
            Public ProductVendorCost As String ' money

            '// "Freight and Handling" tab
            Public ShipperID As String
            Public WaybillNumber As String
            Public ShipDate As String ' date
            Public PackageQuantity As String
            Public GrossWeight As String
            Public GrossVolume As String
            Public Freight As String ' money
            Public Handling As String ' money
            Public Arrastre As String ' money
            Public OtherCharges As String ' money

            '// stock transfer
            Public ReceiveFromOutlet As String
            Public ReceiveInOutlet As String ' not saved
            Public ReceiveFromDepartment As String
            Public ReceiveInDepartment As String

            'Start - For Expense Ticket Module
            Public ExpenseReferenceNo As Integer?
            Public ReimburseToEmployeeName As String
            Public TransType As String ' for saving to TransInvoice. not saved in TransReceiving.
            'END - For Expense Ticket Module

            Public ReadOnly Property SaveAndFinalizeProper As Boolean
                Get
                    Return CBool2(SaveAndFinalize)
                End Get
            End Property

            Public ReadOnly Property FreightAndHandling As Decimal
                Get
                    Return CDec2(Freight) +
                        CDec2(Handling) +
                        CDec2(Arrastre) +
                        CDec2(OtherCharges)
                End Get
            End Property

            Public Function Verify(ByRef oError As String) As Boolean

                oError = Nothing

                If String.IsNullOrWhiteSpace(ShipDate) = False Then
                    If IsDate(ShipDate) Then
                        ShipDate = Format(CDate(ShipDate), cGlobal.FormatDateTimeSql2)
                    Else
                        oError = "ShipDate must be a valid date/time. ref: vb8Z24CXL523!"
                        Return False
                    End If
                End If

                Return True

            End Function

            Public Function InvoiceNumberUnique() As String

                If String.IsNullOrWhiteSpace(InvoiceNumber) Then
                    Return ""
                End If

                Dim pTrimmeds As New List(Of String)
                Dim pSplits = InvoiceNumber.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                For Each pSplit In pSplits
                    pTrimmeds.Add(pSplit.Trim())
                Next
                Dim pUniqueItems = UniqueItems(pTrimmeds)
                Dim pOut = String.Join(", ", pUniqueItems)

                Return pOut

            End Function
        End Class

        Public Class ProductClass
            Public ProductID As String
            Public Items As New List(Of Item)

            Public Class Item
                Public ReceivingStockID As String ' TransReceivingStock.ID
                Public GoodQty As String
                Public BadQty As String
                Public UnitMeasure As String
                Public UnitQty As String
                Public BadOrderReason As String
                Public PoUnitPrice As String ' money
            End Class
        End Class

        Public Class CreatedPoClass
            Public Remarks As String
        End Class
    End Class

    Public Function Save(vSaveJson As SaveJson, vOrgID As String) As SaveOutput

        Dim pOut As SaveOutput
        Dim pID As String
        Dim pError As String
        Dim pNum As Integer

        If String.IsNullOrWhiteSpace(vSaveJson.OrgID) Then
            vSaveJson.OrgID = vOrgID
        End If
        pOut = New SaveOutput()
        mProData.BeginTransaction()
        If String.IsNullOrWhiteSpace(vSaveJson.Receiving.ID) = False Then
            Dim pDR = ByID(vSaveJson.Receiving.ID)
            If pDR IsNot Nothing Then
                If CInt2(pDR("ReceivingNo")) <> 0 Then
                    pNum = CInt2(pDR("ReceivingNo"))
                    If vSaveJson.SaveSomeFieldsProper = False Then
                        Throw New ApplicationException("This stock receiving was already saved as final")
                    End If
                End If
            End If
        End If
        If vSaveJson.SaveOnlyProductProper = False Then
            pOut.Receiving = SaveReceiving(vSaveJson)
            If CInt2(pOut.Receiving.Num) = 0 Then
                pOut.Receiving.Num = pNum
            End If
        End If
        If vSaveJson.SaveSomeFieldsProper = False Then
            SaveReceivingStocks(vSaveJson)
            SaveFooter(vSaveJson.Receiving.ID, vSaveJson.Receiving.FreightAndHandling)
            If vSaveJson.Receiving.SaveAndFinalizeProper Then
                If CBool2(vSaveJson.Receiving.IsReceiving) _
                    OrElse vSaveJson.Receiving.TransType = "StockTransfer" Then

                    AddToInventoryLedger(vSaveJson.Receiving.ID, vOrgID:=vOrgID)
                End If

                mParent.TransInvoice.CreatePoFromReceiving(vSaveJson.Receiving.ID, vOrgID, pError, vSaveJson.Receiving.TransType, vSaveJson.CreatedPo.Remarks)

                If CBool2(vSaveJson.Receiving.IsReceiving) Then
                    Dim pShipperReceivingID = CreateStockReceivingForShipper(vSaveJson.Receiving.ID)
                    If String.IsNullOrWhiteSpace(pShipperReceivingID) = False Then
                        mParent.TransInvoice.CreatePoFromReceiving(pShipperReceivingID, vOrgID, pError, vSaveJson.Receiving.TransType, vSaveJson.CreatedPo.Remarks)
                    End If
                End If
            End If
        End If

        If String.IsNullOrWhiteSpace(pError) Then
            If vSaveJson.SaveSomeFieldsProper = False Then
                If vSaveJson.Receiving.SaveAndFinalizeProper Then
                    mParent.TransInvoice.AutoFinalizePurchaseOrdersFromVendor(vOrgID)
                End If
                UpdatePoReceivedPercent(vSaveJson.Receiving.ID)
            End If
            mProData.CommitTransaction()
            pOut.Result = "ok"
        Else
            mProData.RollbackTransaction(, True)
            pOut.Result = pError
        End If

        Return pOut

    End Function

    Public Class SaveOutput

        Public Result As String
        Public Receiving As SaveReceivingOutput

    End Class

    Public Class SaveReceivingOutput

        Public ID As String
        Public Num As Integer?

    End Class

    Public Function GetNextNumber(vFowstID As String, vDeviceID As String, vIsReceiving As Boolean) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = "SELECT MAX(ReceivingNo)" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " WHERE IsReceiving = " & If(vIsReceiving, 1, 0) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ReceivingPlaceID = " & ReadyString(vFowstID)
        pOut = CInt2(mProData.ExecuteScalar(pSql)) + 1

        Return pOut

    End Function

    Private Function SaveReceiving(vJson As SaveJson) As SaveReceivingOutput

        Dim pOut As SaveReceivingOutput
        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String
        Dim pAffectedRecords As Integer
        Dim pDR As DataRow
        Dim pIsReceiving As Boolean
        Dim pFowstID As String
        Dim pReceivingJson As SaveJson.ReceivingClass = vJson.Receiving

        pOut = New SaveReceivingOutput()
        pIU = New InsertUpdate("TransReceiving")
        pIU("InvoiceNumber") = ReadyString(pReceivingJson.InvoiceNumber) ' dont use InvoiceNumberUnique() anymore
        If vJson.SaveSomeFieldsProper = False Then
            pDR = mParent.LibraryDevice.ByUuid(pReceivingJson.UUID)
            If pDR Is Nothing Then
                Throw New ApplicationException("Device with UUID, " & pReceivingJson.UUID & ", is not registered. ref: vbRDNU3GLBFG!")
            End If
            pReceivingJson.DeviceID = CStr2(pDR("ID"))
            If String.IsNullOrWhiteSpace(pReceivingJson.IsReceiving) Then
                pIsReceiving = True
            Else
                pIsReceiving = CBool2(pReceivingJson.IsReceiving)
            End If

            pIU("IsReceiving") = If(pIsReceiving, 1, 0)
            pIU("DeviceID") = ReadyString(pReceivingJson.DeviceID)
            If IsGuid(pReceivingJson.ReceiveInOutlet) Then
                pFowstID = pReceivingJson.ReceiveInOutlet
                pIU("ReceivingPlaceID") = ReadyString(pReceivingJson.ReceiveInOutlet)
            Else
                pIU("ReceivingPlaceID") = ReadyString(pReceivingJson.ReceivingPlaceID)
            End If
            pIU("BillingAddressID") = ReadyString(pReceivingJson.BillingAddressID)
            pIU("DeliveryAddressID") = ReadyString(pReceivingJson.DeliveryAddressID)
            pIU("SupplierID") = ReadyString(pReceivingJson.SupplierID)
            If pReceivingJson.DocumentDate Is Nothing Then
                pIU("DocumentDate") = "GETDATE()"
            Else
                pIU("DocumentDate") = ReadyString(pReceivingJson.DocumentDate)
            End If
            pIU("DocumentType") = ReadyString(pReceivingJson.DocumentType)
            pIU("DocumentNo") = ReadyString(pReceivingJson.DocumentNo)
            pIU("Is3PL") = If(CBool2(pReceivingJson.Is3PL), 1, 0)
            pIU("WaybillNumber") = ReadyString(pReceivingJson.WaybillNumber)
            pIU("UserID") = ReadyString(pReceivingJson.UserID)
            pIU("SalesRepText") = ReadyString(pReceivingJson.SalesRepText)
            pIU("Remarks") = ReadyString(pReceivingJson.Remarks)
            pIU("SalesOrderNumText") = ReadyString(pReceivingJson.SalesOrderNumText)
            If String.IsNullOrWhiteSpace(pReceivingJson.SalesOrderDate) = False Then
                pIU("SalesOrderDate") = ReadyString(pReceivingJson.SalesOrderDate)
            End If
            If IsDate(pReceivingJson.InvoiceDate) Then
                pIU("InvoiceDate") = ReadyString(pReceivingJson.InvoiceDate)
            Else
                pIU("InvoiceDate") = "NULL"
            End If
            pIU("LastUpdated") = "GETDATE()"

            '// "Freight and Handling" tab
            If String.IsNullOrWhiteSpace(pReceivingJson.ShipperID) = False Then
                pIU("ShipperID") = ReadyString(pReceivingJson.ShipperID)
            End If
            pIU("WaybillNumber") = ReadyString(pReceivingJson.WaybillNumber)
            If String.IsNullOrWhiteSpace(pReceivingJson.ShipDate) Then
                pIU("ShipDate") = "NULL"
            Else
                pIU("ShipDate") = ReadyString(pReceivingJson.ShipDate)
            End If
            pIU("PackageQuantity") = ReadyString(pReceivingJson.PackageQuantity)
            pIU("GrossWeight") = ReadyString(pReceivingJson.GrossWeight)
            pIU("GrossVolume") = ReadyString(pReceivingJson.GrossVolume)
            pIU("Freight") = CDec2(pReceivingJson.Freight)
            pIU("Handling") = CDec2(pReceivingJson.Handling)
            pIU("Arrastre") = CDec2(pReceivingJson.Arrastre)
            pIU("OtherCharges") = CDec2(pReceivingJson.OtherCharges)
            pIU("FreightAndHandling") = pReceivingJson.FreightAndHandling
            If IsNumeric(pReceivingJson.Total) Then
                pIU("Total") = CDec2(pReceivingJson.Total)
            End If

            '// stock transfer
            If String.IsNullOrWhiteSpace(pReceivingJson.ReceiveFromOutlet) = False Then
                pIU("ReceiveFromOutlet") = ReadyString(pReceivingJson.ReceiveFromOutlet)
            End If
            If String.IsNullOrWhiteSpace(pReceivingJson.ReceiveInOutlet) = False Then
                pIU("ReceivingPlaceID") = ReadyString(pReceivingJson.ReceiveInOutlet)
            End If
            If String.IsNullOrWhiteSpace(pReceivingJson.ReceiveFromDepartment) = False Then
                pIU("ReceiveFromDepartment") = ReadyString(pReceivingJson.ReceiveFromDepartment)
            End If
            If String.IsNullOrWhiteSpace(pReceivingJson.ReceiveInDepartment) = False Then
                pIU("ReceiveInDepartment") = ReadyString(pReceivingJson.ReceiveInDepartment)
            End If

            If pReceivingJson.SaveAndFinalizeProper Then
                If pIsReceiving Then
                    pFowstID = pReceivingJson.ReceivingPlaceID
                ElseIf String.IsNullOrWhiteSpace(pFowstID) Then
                    pFowstID = pReceivingJson.ReceiveFromOutlet
                End If
                pOut.Num = GetNextNumber(pFowstID, pReceivingJson.DeviceID, pIsReceiving)
                pIU("ReceivingNo") = pOut.Num
                pIU("SaveAndFinalizedByID") = ReadyString(pReceivingJson.UserID)
                pIU("SaveAndFinalizedDate") = "GETDATE()"
                Dim pIuPo = New InsertUpdate("TransInvoice")
                pIuPo("FinalizedForAutoInvoice") = 1
                If String.IsNullOrWhiteSpace(pReceivingJson.ID) = False Then
                    pSql = pIuPo.GetUpdate("ID IN (" & vbCrLf &
                    " SELECT PurchaseOrderID" & vbCrLf &
                    " FROM TransReceiving" & vbCrLf &
                    " WHERE ID = " & ReadyString(pReceivingJson.ID) & vbCrLf &
                    ")")
                    pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only
                End If
            End If
        End If ' END If vJson.SaveSomeFields = False Then

        If String.IsNullOrWhiteSpace(pReceivingJson.ID) Then
            pID = Guid.NewGuid.ToString()
            pIU("ID") = ReadyString(pID)
            If pReceivingJson.SaveAndFinalizeProper = False Then
                pOut.Num = 0
                pIU("ReceivingNo") = pOut.Num
            End If
            pSql = pIU.GetInsert()
            pReceivingJson.ID = pID
        Else
            pID = pReceivingJson.ID
            pSql = "ID = " & ReadyString(pID) & vbCrLf
            If vJson.SaveSomeFieldsProper = False Then
                pSql &= " AND SaveAndFinalizedByID IS NULL" & vbCrLf &
                " AND SaveAndFinalizedDate IS NULL"
            End If
            pSql = pIU.GetUpdate(pSql)
        End If
        pOut.ID = pID
        pAffectedRecords = mProData.ExecuteSQL(pSql, True)
        If pAffectedRecords <> 1 Then
            Throw New ApplicationException("Expected record is 1. ref: vb8J5QSJ5E2A!")
        End If
        Return pOut

    End Function

    Private Function CreateStockReceivingForShipper(vReceivingID As String) As String

        Dim pSql = "SELECT * FROM TransReceiving" & vbCrLf &
            " WHERE ID = " & ReadyString(vReceivingID)
        Dim pDrSource = mProData.ExecuteRow(pSql)

        If pDrSource Is Nothing Then
            Exit Function
        End If

        If pDrSource("ShipperID") Is DBNull.Value Then
            Exit Function
        End If

        Dim pIU = Business.DataRowToInsertUpdate(pDrSource)
        pIU.TableName = "TransReceiving"
        Dim pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("SupplierID") = ReadyString(pDrSource("ShipperID"))
        pIU("ReceivingNo") = GetNextNumber(CStr2(pDrSource("ReceivingPlaceID")), CStr2(pDrSource("DeviceID")), True)
        pSql = pIU.GetInsert()
        mProData.BeginTransaction()
        mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("TransReceivingStock")
        pIU("ID") = "NEWID()"
        pIU("ReceivingID") = ReadyString(pID)
        pIU("ProductID") = ReadyString(mParent.LibraryStock.GetOrCreatePimByName("Shipping Cost"))
        pIU("QuantityGood") = 1
        pIU("ActualQuantityGood") = 1
        pIU("UnitMeasureGood") = ReadyString(mParent.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Order"))
        pIU("QuantityDamaged") = 0
        pIU("ActualQuantityDamaged") = 1
        pIU("UnitMeasureDamaged") = pIU("UnitMeasureGood")
        pIU("BadOrderReason") = "''"
        pIU("LandedCost") = CDec2(pDrSource("FreightAndHandling"))
        pIU("PoUnitPrice") = CDec2(pDrSource("FreightAndHandling"))
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

        Return pID

    End Function

    Private Sub SaveReceivingStocks(vSaveJson As SaveJson)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If String.IsNullOrWhiteSpace(vSaveJson.Products.ProductID) _
            AndAlso vSaveJson.Products.Items.Count = 0 Then

            Exit Sub
        End If
        'pSql = "DELETE FROM TransReceivingStock" & vbCrLf &
        '    " WHERE ReceivingID = " & ReadyString(vSaveJson.Receiving.ID) & vbCrLf &
        '    " AND ProductID = " & ReadyString(vSaveJson.Products.ProductID)
        'mProData.ExecuteSQL(pSql)
        For Each pItem As SaveJson.ProductClass.Item In vSaveJson.Products.Items
            pIU = New InsertUpdate("TransReceivingStock")
            pIU("ReceivingID") = ReadyString(vSaveJson.Receiving.ID)
            pIU("ProductID") = ReadyString(vSaveJson.Products.ProductID)
            pIU("QuantityGood") = CDec2(pItem.GoodQty)
            pIU("ActualQuantityGood") = CDec2(pItem.UnitQty)
            If IsGuid(pItem.UnitMeasure) = False Then
                Dim pDtUnits = mParent.LibraryStock.ByIds(ReadyString(vSaveJson.Products.ProductID), , True)
                If pDtUnits.Rows.Count > 0 Then
                    pItem.UnitMeasure = CStr2(pDtUnits.Rows(0)("BaseUnitMeasureID"))
                End If
            End If
            pIU("UnitMeasureGood") = ReadyString(pItem.UnitMeasure)
            pIU("QuantityDamaged") = CDec2(pItem.BadQty)
            pIU("ActualQuantityDamaged") = CDec2(pItem.UnitQty)
            pIU("UnitMeasureDamaged") = ReadyString(pItem.UnitMeasure)
            pIU("BadOrderReason") = ReadyString(pItem.BadOrderReason)
            pIU("LandedCost") = mParent.LibraryStock.GetLandedCost(vSaveJson.Products.ProductID, vSaveJson.OrgID)
            'pIU("AdditionalInfo") = ReadyString(pDrvItem("AdditionalInfo"))
            'pIU("IsResaleable") = CDec2(pDrvItem("IsResaleable"))
            pIU("PoUnitPrice") = CDec2(pItem.PoUnitPrice)
            If String.IsNullOrWhiteSpace(pItem.ReceivingStockID) Then
                If CDec2(pItem.GoodQty) = 0 _
                    AndAlso CDec2(pItem.BadQty) = 0 Then

                    Continue For
                End If
                pIU("ID") = "NEWID()"
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(pItem.ReceivingStockID))
            End If
            pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next pItem

    End Sub

    Public Sub SaveFooter(vID As String, vFreightAndHandling As Decimal)

        Dim pProductVendorCost As Decimal

        mProData.BeginTransaction()
        Dim pIU As InsertUpdate
        Dim pSql = "SELECT ID" & vbCrLf &
            ",PoUnitPrice" & vbCrLf &
            ",QuantityGood" & vbCrLf &
            ",QuantityDamaged" & vbCrLf &
            ",ActualQuantityGood" & vbCrLf &
            " FROM TransReceivingStock" & vbCrLf &
            " WHERE ReceivingID = " & ReadyString(vID)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pProductVendorCost += (CDec2(pDR("QuantityGood")) + CDec2(pDR("QuantityDamaged"))) * CDec2(pDR("PoUnitPrice"))
        Next

        For Each pDR In pDT.Rows
            pIU = New InsertUpdate("TransReceivingStock")
            Dim pLineFreightCost As Decimal
            If pProductVendorCost * vFreightAndHandling = 0 Then
                pLineFreightCost = 0
            Else
                pLineFreightCost = (CDec2(pDR("QuantityGood")) + CDec2(pDR("QuantityDamaged"))) * CDec2(pDR("PoUnitPrice")) / pProductVendorCost * vFreightAndHandling
            End If
            pIU("LineFreightCost") = pLineFreightCost
            If (CDec2(pDR("QuantityGood")) + CDec2(pDR("QuantityDamaged"))) = 0 Then
                pIU("UnitFreightCost") = 0
            Else
                pIU("UnitFreightCost") = pLineFreightCost / (CDec2(pDR("QuantityGood")) + CDec2(pDR("QuantityDamaged")))
            End If
            pIU("SubtotalBad") = CDec2(pDR("QuantityDamaged")) * CDec2(pDR("PoUnitPrice"))
            pIU("SubtotalGoodBad") = (CDec2(pDR("QuantityGood")) + CDec2(pDR("QuantityDamaged"))) * CDec2(pDR("PoUnitPrice"))
            pIU("LineLandedCost") = CDec2(pIU("SubtotalGoodBad")) + CDec2(pIU("LineFreightCost"))
            If CDec2(pDR("PoUnitPrice")) = 0 Then
                pIU("FreightPercent") = 0
            Else
                pIU("FreightPercent") = CDec2(pIU("UnitFreightCost")) / CDec2(pDR("PoUnitPrice")) * 100
            End If
            pIU("UnitLandedCost") = CDec2(pIU("UnitFreightCost")) + CDec2(pDR("PoUnitPrice"))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next

        pIU = New InsertUpdate("TransReceiving")
        pIU("ProductVendorCost") = pProductVendorCost
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

    End Sub

    Public Function GetByID(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- receiving" & vbCrLf &
            " SELECT TransReceiving.ID AS [ID]" & vbCrLf &
            " , [ReceivingNo]" & vbCrLf &
            " , [IsReceiving]" & vbCrLf &
            " , [ReceivingPlaceID]" & vbCrLf &
            " ,tblReceivingPlaceOrg.CompanyName AS ReceivingPlaceOrg" & vbCrLf &
            " , tblFowstReceivingPlace.Name AS [ReceivingPlace]" & vbCrLf &
            " , TransReceiving.SupplierID" & vbCrLf &
            " , TransReceiving.InvoiceNumber" & vbCrLf &
            " , TransReceiving.InvoiceDate" & vbCrLf &
            " , TransReceiving.BillingAddressID" & vbCrLf &
            " , TransReceiving.DeliveryAddressID" & vbCrLf &
            " ,tblOrgSupplier.CompanyName AS SupplierName" & vbCrLf &
            " ,  TransReceiving.TransportedBy AS [TransportedBy]" & vbCrLf &
            " , (SELECT COUNT(*) FROM TransReceivingStock WHERE ReceivingID = TransReceiving.ID) AS [SKUCount]" & vbCrLf &
            " , [DateReceived]" & vbCrLf &
            " , [DocumentDate]" & vbCrLf &
            " , [DocumentType]" & vbCrLf &
            " , [DocumentNo]" & vbCrLf &
            " , TransReceiving.SaveAndFinalizedByID" & vbCrLf &
            " , TransReceiving.SaveAndFinalizedDate" & vbCrLf &
            " , TransReceiving.SalesRepText" & vbCrLf &
            ",CONVERT(varchar, tblPoFowst.FOWSTNo) + '-' + CONVERT(varchar, tblPoDevice.DeviceNo) AS PoMin" & vbCrLf &
            " , TransInvoice.PurchaseOrderNo" & vbCrLf &
            ",TransInvoice.PaymentTerm" & vbCrLf &
            " , TransInvoice.LastUpdated AS PurchaseOrderDate" & vbCrLf &
            " , TransReceiving.SalesOrderNumText" & vbCrLf &
            " , TransInvoice.SalesOrderDate" & vbCrLf &
            " , TransInvoice.DateNeeded" & vbCrLf &
            " , TransReceiving.Remarks" & vbCrLf &
            " , TransReceiving.LastUpdated" & vbCrLf &
            "," & BrowserController.ComposePeopleName() & " AS UpdatedBy" & vbCrLf &
            ",ISNULL(CONVERT(varchar, tblFowstReceivingPlace.FOWSTNo), 'X') + '-' + ISNULL(CONVERT(varchar, LibraryDevice.DeviceNo), 'X') + '-' + CONVERT(varchar, ReceivingNo) AS MIN" & vbCrLf
        pSql += ",ISNULL(tblSupplierFowst.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblSupplierFowst.Street, '')" & vbCrLf &
            " + ',' + ISNULL(tblSupplierCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblSupplierCountry.Name, '') AS SupplierAddress" & vbCrLf

        pSql += ",ISNULL(tblReceiveFromOutlet.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblReceiveFromOutlet.Street, '')" & vbCrLf &
            " + ',' + ISNULL(tblReceiveFromOutletCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblReceiveFromOutletCountry.Name, '') AS ReceiveFromOutletAddress" & vbCrLf
        pSql += ",ISNULL(tblReceiveFromOutlet.Name, '') AS ReceiveFromOutletName" & vbCrLf

        pSql += ",ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
            " + ',' + ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblDeliveryCountry.Name, '') AS DeliveryAddress" & vbCrLf
        pSql &= " ,ISNULL(tblBilling.StreetNumber, '')" & vbCrLf &
            "  + ',' + ISNULL(tblBilling.Street, '')" & vbCrLf &
            "  + ',' + ISNULL(tblBillingCity.Name, '')" & vbCrLf &
            "  + ',' + ISNULL(tblBillingCountry.Name, '') AS BillingAddress" & vbCrLf
        pSql &= "  ,LibraryPriceCanvass.CanvassNumber" & vbCrLf &
            "  ,LibraryPriceCanvass.LastUpdated AS CanvassDate" & vbCrLf
        pSql &= " -- ""Freight and Handling"" tab" & vbCrLf &
            " ,TransReceiving.ShipperID" & vbCrLf &
            " ,tblOrgShipper.CompanyName AS ShipperName" & vbCrLf &
            " ,TransReceiving.WaybillNumber" & vbCrLf &
            " ,TransReceiving.PackageQuantity" & vbCrLf &
            " ,TransReceiving.ShipDate" & vbCrLf &
            " ,TransReceiving.GrossWeight" & vbCrLf &
            " ,TransReceiving.GrossVolume" & vbCrLf &
            " ,TransReceiving.Freight" & vbCrLf &
            " ,TransReceiving.Handling" & vbCrLf &
            " ,TransReceiving.Arrastre" & vbCrLf &
            " ,TransReceiving.OtherCharges" & vbCrLf &
            " ,TransReceiving.FreightAndHandling" & vbCrLf &
            " ,TransReceiving.PurchaseOrderID" & vbCrLf &
            ",tblTransferFromFowstBin.Name AS TransferFromFowstBinName" & vbCrLf &
            ",tblTransferToFowstBin.Name AS TransferToFowstBinName" & vbCrLf
        pSql &= " FROM TransReceiving" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST tblFowstReceivingPlace ON tblFowstReceivingPlace.ID = TransReceiving.ReceivingPlaceID" & vbCrLf &
            " JOIN MobizLibraryOrganization tblReceivingPlaceOrg ON tblReceivingPlaceOrg.ID = tblFowstReceivingPlace.OrganizationID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransReceiving.DeviceID = LibraryDevice.ID" & vbCrLf &
            " JOIN MobizLibraryOrganization tblOrgSupplier ON tblOrgSupplier.ID = TransReceiving.SupplierID" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.ID = TransReceiving.PurchaseOrderID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryOrganizationFOWST tblSupplierFowst ON tblSupplierFowst.OrganizationID = TransReceiving.SupplierID" & vbCrLf &
            " AND tblSupplierFowst.IsMainAddress = 1" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblSupplierCity ON tblSupplierCity.ID = tblSupplierFowst.FowstCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblSupplierCountry ON tblSupplierCountry.ID = tblSupplierFowst.FowstCountryID" & vbCrLf

        pSql &= " LEFT JOIN MobizLibraryOrganizationFOWST tblReceiveFromOutlet ON tblReceiveFromOutlet.ID = TransReceiving.ReceiveFromOutlet" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblReceiveFromOutletCity ON tblReceiveFromOutletCity.ID = tblReceiveFromOutlet.FowstCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblReceiveFromOutletCountry ON tblReceiveFromOutletCountry.ID = tblReceiveFromOutlet.FowstCountryID" & vbCrLf

        pSql &= " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransReceiving.DeliveryAddressID" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID" & vbCrLf
        pSql &= "  LEFT JOIN MobizLibraryOrganizationFOWST tblBilling ON tblBilling.ID = TransReceiving.BillingAddressID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCities tblBillingCity ON tblBillingCity.ID = tblBilling.FowstCityID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCountries tblBillingCountry ON tblBillingCountry.ID = tblBilling.FowstCountryID " & vbCrLf &
            "  LEFT JOIN LibraryPriceCanvass ON LibraryPriceCanvass.ID = TransInvoice.CanvassID " & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryOrganizationFOWST tblPoFowst ON tblPoFowst.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN LibraryDevice tblPoDevice ON TransInvoice.DeviceID = tblPoDevice.ID" & vbCrLf
        pSql &= " -- ""Freight and Handling"" tab" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization tblOrgShipper ON tblOrgShipper.ID = TransReceiving.ShipperID" & vbCrLf
        pSql &= " LEFT JOIN LibraryFowstBin tblTransferFromFowstBin ON tblTransferFromFowstBin.ID = TransReceiving.ReceiveFromDepartment" & vbCrLf &
            " LEFT JOIN LibraryFowstBin tblTransferToFowstBin ON tblTransferToFowstBin.ID = TransReceiving.ReceiveInDepartment" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
        pSql &= " WHERE TransReceiving.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetStocks(vID As String, Optional vAccountsPayableID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        If String.IsNullOrWhiteSpace(vAccountsPayableID) Then
            vAccountsPayableID = "NULL"
        Else
            vAccountsPayableID = ReadyString(vAccountsPayableID)
        End If
        'pSql = " SELECT TransReceivingStock.ID" & vbCrLf &
        '    " ,TransReceivingStock.ProductID" & vbCrLf &
        '    " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
        '    " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf & _
        '    "  ,TransReceivingStock.QuantityGood" & vbCrLf &
        '    "  ,TransReceivingStock.QuantityDamaged" & vbCrLf &
        '    "  ,TransReceivingStock.UnitMeasureGood" & vbCrLf &
        '    "  ,TransReceivingStock.ActualQuantityGood" & vbCrLf &
        '    "  ,TransReceivingStock.BadOrderReason" & vbCrLf &
        '    "  ,TransReceivingStock.LandedCost" & vbCrLf &
        '    "  ,TransReceivingStock.LineFreightCost" & vbCrLf &
        '    "  ,TransReceivingStock.UnitFreightCost" & vbCrLf &
        '    "  ,TransReceivingStock.SubtotalBad" & vbCrLf &
        '    "  ,TransReceivingStock.SubtotalGoodBad" & vbCrLf &
        '    "  --,TransReceivingStock.LineLandedCost" & vbCrLf &
        '    "  ,TransReceivingStock.FreightPercent" & vbCrLf &
        '    "  ,TransReceivingStock.UnitLandedCost" & vbCrLf &
        '    "  ,TransReceiving.PurchaseOrderID" & vbCrLf &
        '    "  ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
        '    " ,tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
        '    " ,tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
        '    "  ,tblPoItems.OrderedQuantity AS PoQty" & vbCrLf &
        '    "  ,ISNULL(TransReceivingStock.PoUnitPrice, tblPoItems.UnitPrice) AS PoUnitPrice" & vbCrLf &
        '    "  ,tblAccountsPayable.UnitPrice * TransReceivingStock.QuantityGood AS GoodQtySubTotal" & vbCrLf &
        '    "  ,tblVendorPrice.Price * TransReceivingStock.ActualQuantityGood AS VendorPrice" & vbCrLf &
        '    "  ,tblVendorPrice.LastUpdated AS VendorPriceLastUpdated" & vbCrLf &
        '    "  ,tblAccountsPayable.UnitPrice AS ApPrice" & vbCrLf &
        '    "  FROM TransReceivingStock" & vbCrLf &
        '    "  LEFT JOIN TransReceiving ON TransReceiving.ID = TransReceivingStock.ReceivingID" & vbCrLf &
        '    "  LEFT JOIN MobizLibraryPIMProduct ON TransReceivingStock.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
        '    "   LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = TransReceivingStock.UnitMeasureGood  " & vbCrLf &
        '    "   LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
        '    " LEFT JOIN (" & vbCrLf &
        '    "   	SELECT TransInvoiceItems.*" & vbCrLf &
        '    "     FROM TransInvoiceItems" & vbCrLf &
        '    "     JOIN TransReceiving ON TransReceiving.PurchaseOrderID = TransInvoiceItems.InvoiceID" & vbCrLf &
        '    "     WHERE TransReceiving.ID = " & ReadyString(vID) & vbCrLf &
        '    "   ) tblPoItems ON tblPoItems.ProductID = TransReceivingStock.ProductID" & vbCrLf &
        '    "   AND tblPoItems.UnitMeasure = MobizLibraryCountriesLanguageLabel.Label" & vbCrLf &
        '    "  OUTER APPLY (" & vbCrLf &
        '    "  SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
        '    "  	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
        '    "      FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
        '    "      JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
        '    "      JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
        '    "      WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
        '    "      ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
        '    "  ) tblBaseUnit" & vbCrLf
        'pSql &= "    OUTER APPLY (" & vbCrLf &
        '    "    	SELECT TOP 1 LibraryVendorPrices.Price" & vbCrLf &
        '    "    	,LibraryVendorPrices.LastUpdated" & vbCrLf &
        '    "      FROM LibraryVendorPrices" & vbCrLf &
        '    "      WHERE LibraryVendorPrices.ProductID = TransReceivingStock.ProductID" & vbCrLf &
        '    "      AND LibraryVendorPrices.VendorID = TransReceiving.SupplierID" & vbCrLf &
        '    "      ORDER BY LibraryVendorPrices.LastUpdated DESC" & vbCrLf &
        '    "    ) tblVendorPrice" & vbCrLf

        'pSql &= "     OUTER APPLY (" & vbCrLf &
        '    "     	SELECT TOP 1 TransInvoiceItems.UnitPrice" & vbCrLf &
        '    "       FROM TransInvoice" & vbCrLf &
        '    "       JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
        '    "       WHERE TransInvoiceItems.ProductID = TransReceivingStock.ProductID" & vbCrLf &
        '    "       AND TransInvoiceItems.ActualQuantity = TransReceivingStock.ActualQuantityGood" & vbCrLf &
        '    "       AND TransInvoice.ID = TransReceiving.AccountsPayableID" & vbCrLf &
        '    "     ) tblAccountsPayable" & vbCrLf

        'pSql &= " WHERE TransReceivingStock.ReceivingID = " & ReadyString(vID) & vbCrLf &
        '    "  AND (TransReceivingStock.QuantityGood > 0 OR TransReceivingStock.QuantityDamaged > 0 OR ISNULL(tblPoItems.OrderedQuantity, 0) > 0) " & vbCrLf &
        '    "  ORDER BY ProductName, ActualQuantityGood"

        pSql = "  SELECT tblMain.ID" & vbCrLf &
            "  ,tblMain.ProductID" & vbCrLf &
            "  ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            "  ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf &
            "  ,MobizLibraryPIMProduct.ProductName AS ProductNameOnly" & vbCrLf &
            "  ,MobizLibraryPIMProduct.Brand" & vbCrLf &
            "   ,tblMain.QuantityGood" & vbCrLf &
            "   ,tblMain.QuantityDamaged" & vbCrLf &
            "   ,tblMain.UnitMeasureGood" & vbCrLf &
            "   ,tblMain.ActualQuantityGood" & vbCrLf &
            "   ,tblMain.BadOrderReason" & vbCrLf &
            "   ,tblMain.LandedCost" & vbCrLf &
            "   ,tblMain.LineFreightCost" & vbCrLf &
            "   ,tblMain.UnitFreightCost" & vbCrLf &
            "   ,tblMain.SubtotalBad" & vbCrLf &
            "   ,tblMain.SubtotalGoodBad" & vbCrLf &
            "   --,tblMain.LineLandedCost" & vbCrLf &
            "   ,tblMain.FreightPercent" & vbCrLf &
            "   ,tblMain.UnitLandedCost" & vbCrLf &
            "   ,tblMain.PurchaseOrderID --TransReceiving" & vbCrLf &
            "   ,ISNULL(MobizLibraryCountriesLanguageLabel.Label, tblMain.UnitMeasure) AS UnitMeasure" & vbCrLf &
            "  ,tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
            "  ,tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
            "   ,tblPoItems.OrderedQuantity AS PoQty" & vbCrLf &
            "   ,ISNULL(tblMain.PoUnitPrice, tblPoItems.UnitPrice) AS PoUnitPrice" & vbCrLf &
            "   ,tblMain.UnitPrice * tblMain.QuantityGood AS GoodQtySubTotal" & vbCrLf &
            "   ,tblVendorPrice.Price * tblMain.ActualQuantityGood AS VendorPrice" & vbCrLf &
            "   ,tblVendorPrice.LastUpdated AS VendorPriceLastUpdated" & vbCrLf &
            "   ,tblMain.UnitPrice AS ApPrice" & vbCrLf &
            "   ,(SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE Size = 'small' AND ProductID = tblMain.ProductID AND IsDefault = 1 AND IsFromVendor = 0) as Image" & vbCrLf &
            "   FROM" & vbCrLf &
            " (" & vbCrLf &
            "   SELECT tblReceiving.ID" & vbCrLf &
            " ,tblReceiving.PoItemID" & vbCrLf &
            " ,tblReceiving.AccountsPayableID" & vbCrLf &
            " ,ISNULL(tblReceiving.ProductID, tblAccountsPayable.ProductID) AS ProductID" & vbCrLf &
            " ,ISNULL(tblAccountsPayable.Quantity, tblReceiving.QuantityGood) AS QuantityGood" & vbCrLf &
            " ,tblReceiving.QuantityDamaged" & vbCrLf &
            " ,ISNULL(tblReceiving.ActualQuantityGood, tblAccountsPayable.ActualQuantity) AS ActualQuantityGood" & vbCrLf &
            " ,tblReceiving.UnitMeasureGood" & vbCrLf &
            " ,tblAccountsPayable.UnitMeasure" & vbCrLf &
            " ,ISNULL(tblReceiving.SupplierID, tblAccountsPayable.CustomerID) AS SupplierID" & vbCrLf &
            " ,tblReceiving.ReceivingID" & vbCrLf &
            " ,tblReceiving.BadOrderReason" & vbCrLf &
            " ,ISNULL(tblReceiving.LandedCost, tblAccountsPayable.Cost) AS LandedCost" & vbCrLf &
            " ,tblReceiving.LineFreightCost" & vbCrLf &
            " ,tblReceiving.UnitFreightCost" & vbCrLf &
            " ,tblReceiving.SubtotalBad" & vbCrLf &
            " ,tblReceiving.SubtotalGoodBad" & vbCrLf &
            " ,tblReceiving.LineLandedCost" & vbCrLf &
            " ,tblReceiving.FreightPercent" & vbCrLf &
            " ,ISNULL(tblReceiving.UnitLandedCost, tblAccountsPayable.Cost) AS UnitLandedCost" & vbCrLf &
            " ,tblReceiving.PurchaseOrderID" & vbCrLf &
            " ,tblReceiving.PoUnitPrice" & vbCrLf &
            " ,tblAccountsPayable.UnitPrice" & vbCrLf &
            " ,tblAccountsPayable.Quantity" & vbCrLf &
            "  FROM" & vbCrLf &
            "   (" & vbCrLf &
            "     SELECT TransReceivingStock.ID" & vbCrLf &
            "     ,TransReceiving.AccountsPayableID" & vbCrLf &
            "     ,TransReceivingStock.ProductID" & vbCrLf &
            "     ,TransReceivingStock.QuantityGood" & vbCrLf &
            "     ,TransReceivingStock.QuantityDamaged" & vbCrLf &
            "     ,TransReceivingStock.ActualQuantityGood" & vbCrLf &
            "     ,TransReceivingStock.UnitMeasureGood" & vbCrLf &
            "     ,TransReceivingStock.BadOrderReason" & vbCrLf &
            "     ,TransReceivingStock.LandedCost" & vbCrLf &
            "     ,TransReceivingStock.LineFreightCost" & vbCrLf &
            "     ,TransReceivingStock.UnitFreightCost" & vbCrLf &
            "     ,TransReceivingStock.SubtotalBad" & vbCrLf &
            "     ,TransReceivingStock.SubtotalGoodBad" & vbCrLf &
            "     ,TransReceivingStock.LineLandedCost" & vbCrLf &
            "     ,TransReceivingStock.FreightPercent" & vbCrLf &
            "     ,TransReceivingStock.UnitLandedCost" & vbCrLf &
            "     ,TransReceivingStock.PoUnitPrice" & vbCrLf &
            "     ,TransReceivingStock.PoItemID" & vbCrLf &
            "     ,TransReceiving.SupplierID" & vbCrLf &
            "     ,TransReceiving.PurchaseOrderID" & vbCrLf &
            "     ,TransReceivingStock.ReceivingID" & vbCrLf &
            "     FROM TransReceivingStock" & vbCrLf &
            "     JOIN TransReceiving ON TransReceiving.ID = TransReceivingStock.ReceivingID" & vbCrLf &
            "     WHERE TransReceivingStock.ReceivingID = " & ReadyString(vID) & vbCrLf

        If vAccountsPayableID <> "NULL" Then
            pSql &= "     AND (TransReceivingStock.QuantityGood > 0" & vbCrLf &
            "     OR TransReceivingStock.QuantityDamaged > 0" & vbCrLf &
            "     --OR ISNULL(tblPoItems.OrderedQuantity, 0) > 0" & vbCrLf &
            "     )" & vbCrLf
        End If

        pSql &= "   ) tblReceiving" & vbCrLf &
            "   FULL OUTER JOIN (" & vbCrLf &
            "       SELECT TransInvoiceItems.ProductID" & vbCrLf &
            "       ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            "       ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "       ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "       ,TransInvoiceItems.Quantity" & vbCrLf &
            "       ,TransInvoiceItems.Cost" & vbCrLf &
            "       ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "       ,TransInvoice.CustomerID" & vbCrLf &
            "       FROM TransInvoice" & vbCrLf &
            "       JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "       WHERE TransInvoice.ID = " & vAccountsPayableID & vbCrLf &
            "   ) tblAccountsPayable" & vbCrLf &
            "   ON tblAccountsPayable.ProductID = tblReceiving.ProductID" & vbCrLf &
            "   AND tblAccountsPayable.ActualQuantity = tblReceiving.ActualQuantityGood" & vbCrLf &
            "   AND tblAccountsPayable.InvoiceID = tblReceiving.AccountsPayableID" & vbCrLf &
            " ) tblMain" & vbCrLf &
            "   LEFT JOIN MobizLibraryPIMProduct ON tblMain.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "    LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = tblMain.UnitMeasureGood" & vbCrLf &
            "    LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "  LEFT JOIN (" & vbCrLf &
            "    	SELECT TransInvoiceItems.*" & vbCrLf &
            "      FROM TransInvoiceItems" & vbCrLf &
            "      JOIN TransReceiving ON TransReceiving.PurchaseOrderID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "      WHERE TransReceiving.ID = " & ReadyString(vID) & vbCrLf &
            "    ) tblPoItems ON tblPoItems.ID = tblMain.PoItemID" & vbCrLf &
            "   OUTER APPLY (" & vbCrLf &
            "   SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            "   	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
            "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "       WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            "   ) tblBaseUnit" & vbCrLf &
            "     OUTER APPLY (" & vbCrLf &
            "     	SELECT TOP 1 LibraryVendorPrices.Price" & vbCrLf &
            "     	,LibraryVendorPrices.LastUpdated" & vbCrLf &
            "       FROM LibraryVendorPrices" & vbCrLf &
            "       WHERE LibraryVendorPrices.ProductID = tblMain.ProductID" & vbCrLf &
            "       AND LibraryVendorPrices.VendorID = tblMain.SupplierID --TransReceiving.SupplierID" & vbCrLf &
            "       ORDER BY LibraryVendorPrices.LastUpdated DESC" & vbCrLf &
            "     ) tblVendorPrice" & vbCrLf &
            "  WHERE (" & vbCrLf &
            "   	ISNULL(tblMain.QuantityGood, 0) > 0" & vbCrLf &
            "     OR ISNULL(tblMain.QuantityDamaged, 0) > 0" & vbCrLf &
            "     OR ISNULL(tblMain.Quantity, 0) > 0" & vbCrLf &
            "     OR ISNULL(tblPoItems.OrderedQuantity, 0) > 0" & vbCrLf &
            "   )" & vbCrLf &
            "   ORDER BY ProductName, ActualQuantityGood"

        pDT = mProData.Execute(pSql)

        pDT.Columns.Add("UnitPriceText")
        pDT.Columns.Add("BadOrderReasonName")
        For Each pDR As DataRow In pDT.Rows
            If pDR("PurchaseOrderID") IsNot DBNull.Value Then
                pDR("UnitPriceText") = Format(CDec2(pDR("PoUnitPrice")), "#,##0.00;(#,##0.00);-")
            ElseIf pDR("VendorPriceLastUpdated") IsNot DBNull.Value _
                AndAlso CDec2(pDR("VendorPrice")) > 0 Then

                pDR("UnitPriceText") = Format(CDec2(pDR("VendorPrice")), "#,##0.00;(#,##0.00);-") & vbCrLf
                pDR("UnitPriceText") &= " (" & Format(pDR("VendorPriceLastUpdated"), cGlobal.FormatDate) & ")"
                'pDR("GoodQtySubTotal") = CDec2(pDR("VendorPrice")) * CDec2(pDR("QuantityGood"))
            Else
                pDR("UnitPriceText") = "-"
            End If

            If CDec2(pDR("QuantityDamaged")) > 0 _
                AndAlso CStr2(pDR("BadOrderReason")).Trim() <> "" Then

                Select Case CStr2(pDR("BadOrderReason")).Trim().ToLower()
                    Case "Expireditem".ToLower()
                        pDR("BadOrderReasonName") = "Expired item"

                    Case "About-to-expireitem".ToLower()
                        pDR("BadOrderReasonName") = "About-to-expire item"

                    Case "Factory-defectivepackaging".ToLower()
                        pDR("BadOrderReasonName") = "Factory-defective packaging"

                    Case "Factory-defectiveproduct".ToLower()
                        pDR("BadOrderReasonName") = "Factory-defective product"

                    Case "Non-Factory-DamagedGoods".ToLower()
                        pDR("BadOrderReasonName") = "Non-Factory-Damaged Goods"

                    Case "Non-Factory-DamagedPackaging".ToLower()
                        pDR("BadOrderReasonName") = "Non-Factory-Damaged Packaging"

                    Case "Obsoleteitem".ToLower()
                        pDR("BadOrderReasonName") = "Obsolete item"

                    Case "Productrecall".ToLower()
                        pDR("BadOrderReasonName") = "Product recall"

                    Case Else
                        pDR("BadOrderReasonName") = pDR("BadOrderReason")

                End Select
            End If
        Next

        Return pDT

    End Function

    Public Function CreateFromPo(vInvoiceID As String,
        vFowstID As String,
        vDeviceID As String,
        vUserID As String,
        ByRef oError As String) As String

        Dim pDsInvoice As DataSet
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDtInvoice As DataTable
        Dim pDrInvoice As DataRow
        Dim pID As String

        oError = Nothing
        Try
            mProData.BeginTransaction()
            pDsInvoice = mParent.TransInvoice.GetCart(vInvoiceID)
            pDtInvoice = pDsInvoice.Tables("Invoices")
            If pDtInvoice.Rows.Count = 0 Then
                oError = "Query for PO " & vInvoiceID & " returns no result"
                Return False
            End If
            pDrInvoice = pDtInvoice.Rows(0)

            pIU = New InsertUpdate("TransReceiving")
            pID = Guid.NewGuid.ToString
            pIU("ID") = ReadyString(pID)
            pIU("ReceivingNo") = 0
            pIU("IsReceiving") = 1
            pIU("ReceivingPlaceID") = ReadyString(vFowstID)
            pIU("SupplierID") = ReadyString(pDrInvoice("CustomerID"))
            pIU("Remarks") = ReadyString(pDrInvoice("Remarks"))
            pIU("DocumentDate") = ReadyString(Now)
            pIU("DocumentType") = ReadyString("")
            pIU("DocumentNo") = ReadyString("")
            pIU("Is3PL") = 0
            pIU("IsResaleable") = 0
            pIU("ResaleableReason") = ReadyString("")
            pIU("UserID") = ReadyString(vUserID)
            pIU("DeviceID") = ReadyString(vDeviceID)
            pIU("TransportedBy") = ReadyString("")
            pIU("LastUpdated") = "GETDATE()"
            pIU("HasIssues") = 0
            pIU("IssueRemarks") = "null"
            pIU("PurchaseOrderID") = ReadyString(vInvoiceID)
            If pDrInvoice("SalesPartnerOrgID") IsNot DBNull.Value Then
                pIU("SalesPartnerOrgID") = ReadyString(pDrInvoice("SalesPartnerOrgID"))
            End If
            If pDrInvoice("SalesRepPeopleID") IsNot DBNull.Value Then
                pIU("SalesRepPeopleID") = ReadyString(pDrInvoice("SalesRepPeopleID"))
            End If
            If pDrInvoice("SalesRepText") IsNot DBNull.Value Then
                pIU("SalesRepText") = ReadyString(pDrInvoice("SalesRepText"))
            End If
            If pDrInvoice("BillingAddressID") IsNot DBNull.Value Then
                pIU("BillingAddressID") = ReadyString(pDrInvoice("BillingAddressID"))
            End If
            If pDrInvoice("DeliveryAddressID") IsNot DBNull.Value Then
                pIU("DeliveryAddressID") = ReadyString(pDrInvoice("DeliveryAddressID"))
            End If

            '// "Freight and Handling" tab
            If pDrInvoice("ShipperID") IsNot DBNull.Value Then
                pIU("ShipperID") = ReadyString(pDrInvoice("ShipperID"))
            End If
            pIU("WaybillNumber") = ReadyString(pDrInvoice("WaybillNumber"))
            If pDrInvoice("ShipDate") IsNot DBNull.Value Then
                pIU("ShipDate") = ReadyString(pDrInvoice("ShipDate"))
            End If
            pIU("PackageQuantity") = ReadyString(pDrInvoice("PackageQuantity"))
            pIU("GrossWeight") = ReadyString(pDrInvoice("GrossWeight"))
            pIU("GrossVolume") = ReadyString(pDrInvoice("GrossVolume"))
            pIU("Freight") = CDec2(pDrInvoice("Freight"))
            pIU("Handling") = CDec2(pDrInvoice("Handling"))
            pIU("Arrastre") = CDec2(pDrInvoice("Arrastre"))
            pIU("OtherCharges") = CDec2(pDrInvoice("OtherCharges"))
            pIU("FreightAndHandling") = CDec2(pDrInvoice("Freight")) +
                        CDec2(pDrInvoice("Handling")) +
                        CDec2(pDrInvoice("Arrastre")) +
                        CDec2(pDrInvoice("OtherCharges"))

            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)

            For Each pDrProduct As DataRow In pDsInvoice.Tables("Stocks").Rows
                pIU = New InsertUpdate("TransReceivingStock")
                pIU("ID") = "NEWID()"
                pIU("ReceivingID") = ReadyString(pID)
                pIU("ProductID") = ReadyString(pDrProduct("ProductID"))
                pIU("QuantityGood") = 0 'CDec2(pDrProduct("Quantity"))
                pIU("ActualQuantityGood") = CDec2(pDrProduct("ActualQuantity"))
                pIU("UnitMeasureGood") = ReadyString(pDrProduct("UnitMeasureID"))
                pIU("IsResaleable") = 1
                pIU("UnitMeasureDamaged") = pIU("UnitMeasureGood")
                pIU("PoUnitPrice") = CDec2(pDrProduct("UnitPrice"))
                pIU("PoItemID") = ReadyString(pDrProduct("ID"))
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            Next pDrProduct

            mProData.CommitTransaction()

            Return pID
        Catch ex As Exception
            oError = ex.Message
            mProData.RollbackTransaction(, True)
        End Try

    End Function

    Public Function ReceivingsWithPayableSql() As String

        Dim pSql As String

        pSql = "  -- ReceivingsWithPayableSql" & vbCrLf &
            "  SELECT TransReceiving.ID" & vbCrLf &
            "  ,TransReceiving.ReceivingNo" & vbCrLf &
            "  ,TransReceiving.SupplierID" & vbCrLf &
            "  ,TransReceiving.Total AS GrossInvoiceAmount" & vbCrLf &
            "  ,TransReceiving.InvoiceDate" & vbCrLf &
            "  ,TransReceiving.LastUpdated" & vbCrLf &
            "  ,TransReceiving.InvoiceNumber" & vbCrLf &
            "  ,TransReceiving.Remarks" & vbCrLf &
            "  ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.MiddleName AS UpdatedByName" & vbCrLf &
            "  ,ISNULL(tblPo.TotalAmount, 0) AS TotalAmount" & vbCrLf &
            "  ,tblPo.PaymentTerm" & vbCrLf &
            "  ,tblPo.InvoiceDueDate" & vbCrLf &
            "       ,ISNULL(tblAdjustments.Amount, 0) AS AdjustmentAmount" & vbCrLf &
            "       ,ISNULL(tblAdjustments.DebitMemo, 0) AS DebitMemo" & vbCrLf &
            "       ,ISNULL(tblAdjustments.CreditMemo, 0) AS CreditMemo" & vbCrLf &
            " ,ISNULL(tblPayments.AmountPaid, 0) AS TotalPayment" & vbCrLf &
            " ,tblPo.OfficialReceiptRef" & vbCrLf &
            "  FROM TransReceiving" & vbCrLf &
            "  LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
            "  OUTER APPLY (SELECT TransInvoice.TotalAmount" & vbCrLf &
            "  ,TransInvoice.InvoiceDate + ISNULL(TransInvoice.PaymentTerm, 0) AS InvoiceDueDate" & vbCrLf &
            "  ,TransInvoice.PaymentTerm" & vbCrLf &
            "  ,TransInvoice.OfficialReceiptRef" & vbCrLf &
            "  FROM TransInvoice" & vbCrLf &
            "  WHERE TransInvoice.ID = TransReceiving.AccountsPayableID" & vbCrLf &
            " ) tblPo" & vbCrLf &
            "         LEFT JOIN (" & vbCrLf &
            "            SELECT ISNULL(SUM(TransInvoiceAdjustments.Amount), 0) AS Amount" & vbCrLf &
            "            ,SUM(CASE WHEN TransInvoiceAdjustments.Amount > 0 THEN TransInvoiceAdjustments.Amount END) AS DebitMemo" & vbCrLf &
            "            ,SUM(CASE WHEN TransInvoiceAdjustments.Amount < 0 THEN TransInvoiceAdjustments.Amount END) AS CreditMemo" & vbCrLf &
            "            ,TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            "            FROM TransInvoiceAdjustments" & vbCrLf &
            "            WHERE TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
            "            GROUP BY TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            "         ) tblAdjustments ON tblAdjustments.InvoiceID = TransReceiving.AccountsPayableID" & vbCrLf &
            "       LEFT JOIN (" & vbCrLf &
            "       	SELECT TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
            "           ,SUM(TransReceivingPaymentReceivings.AmountPaid) AS AmountPaid" & vbCrLf &
            "           FROM TransReceivingPayment" & vbCrLf &
            "           JOIN TransReceivingPaymentReceivings ON TransReceivingPaymentReceivings.PaymentID = TransReceivingPayment.ID" & vbCrLf &
            "           WHERE TransReceivingPayment.IsActive = 1" & vbCrLf &
            "           GROUP BY TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
            "        ) tblPayments ON tblPayments.ReceivingID = TransReceiving.ID" & vbCrLf &
            "  WHERE ISNULL(TransReceiving.IsCancelled, 0) = 0" & vbCrLf &
            "  AND TransReceiving.IsReceiving = 1"

        Return pSql

    End Function

    Public Function CacheReceivingsWithCredit() As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer

        mProData.BeginTransaction()
        pSql = "DELETE FROM CacheReceivings"
        pOut = mProData.ExecuteSQL(pSql) ' output is for debugging only

        pIU = New InsertUpdate("CacheReceivings")
        pIU("ID") = "ID"
        pIU("SupplierID") = "SupplierID"
        pIU("GrossInvoiceAmount") = "TotalAmount" ' "GrossInvoiceAmount"
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("LastUpdated") = "LastUpdated"
        pIU("InvoiceNumber") = "InvoiceNumber"
        pIU("ReceivingNumber") = "ReceivingNo"
        pIU("CheckPaymentAmount") = 0 ' "CheckPaymentAmount"
        pIU("CreditCardAmount") = 0 '"CreditCardAmount"
        pIU("DebitCardAmount") = 0 ' "DebitCardAmount"
        pIU("SRVAmount") = 0 ' "SRVAmount"
        pIU("OtherTypeAmount") = 0 ' "OtherTypeAmount"
        pIU("TotalPayment") = "ROUND(TotalPayment, 2)"
        pIU("UpdatedByName") = "UpdatedByName"
        pIU("CreditTerms") = "PaymentTerm"
        pIU("InvoiceDueDate") = "InvoiceDueDate"
        pIU("AdjustmentAmount") = "AdjustmentAmount"
        pIU("DebitMemo") = 0 '"DebitMemo"
        pIU("CreditMemo") = 0 ' "CreditMemo"
        pIU("NetInvoiceAmount") = "ROUND(TotalAmount + AdjustmentAmount, 2)" ' add with "minus adjustments"
        pIU("NetInvoiceBalance") = "ROUND(TotalAmount + AdjustmentAmount - TotalPayment, 2)" ' replace GrossInvoiceAmount with "net invoice amount"
        pIU("OfficialReceiptRef") = "OfficialReceiptRef"
        'pIU("IsPrinted") = "IsPrinted"
        pIU("Remarks") = "ISNULL(Remarks, '')"
        pIU.FromSql = "FROM (" & ReceivingsWithPayableSql() & ") t"
        pSql = pIU.GetInsert(True)
        pOut = mProData.ExecuteSQL(pSql)
        mProData.CommitTransaction()

        Return pOut

    End Function

    Public Function GetReceivingsWithPayableFromCache(vSupplierID As String,
        Optional ByVal vMaxRecords As Integer = 0,
        Optional vInvoiceID As String = Nothing,
        Optional vIncludeHasPayable As Boolean = True,
        Optional vIncludeFullyPaid As Boolean = False,
        Optional vExcludeWithOfficialReceiptRef As Boolean = False,
        Optional vIncludeDraft As Boolean = False,
        Optional vOnlyInactiveAccountsPayable As Boolean = False) As DataTable

        Dim pOut As DataTable
        Dim pSql As String
        Dim pSqlWhere As String
        'Dim pDontIncludeDraft As Boolean ' commented on 2022_02_19__17_41_58_1061

        If vMaxRecords <> 0 Then
            pSql = " TOP " & vMaxRecords
        End If
        If CStr2(vInvoiceID).Trim() <> "" Then
            pSqlWhere = " AND (CacheReceivings.ID = " & ReadyString(vInvoiceID) & vbCrLf
        ElseIf vIncludeHasPayable And vIncludeFullyPaid Then
            pSqlWhere = " AND (1=1 "
        ElseIf vIncludeHasPayable Then
            pSqlWhere = " AND (CacheReceivings.NetInvoiceBalance > 0 "
        ElseIf vIncludeFullyPaid Then
            pSqlWhere = " AND (CacheReceivings.NetInvoiceBalance <= 0 " & vbCrLf &
                " AND ISNULL(TransInvoice.AccountsPayableNum, 0) > 0 " & vbCrLf
            If vExcludeWithOfficialReceiptRef Then
                pSqlWhere &= " AND LEN(LTRIM(RTRIM(ISNULL(CacheReceivings.OfficialReceiptRef, '')))) = 0 " & vbCrLf
            End If
            'pDontIncludeDraft = True
        ElseIf vOnlyInactiveAccountsPayable Then
            pSqlWhere = " AND (ISNULL(TransInvoice.IsCancelled, 0) = 1 "
        Else
            pSqlWhere = " AND (1=1 "
        End If
        pSql = "SELECT" & pSql & " CacheReceivings.* " & vbCrLf &
            " ,TransReceiving.AccountsPayableID" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.AccountsPayableNum) AS ApNum" & vbCrLf &
            "  ,TransInvoice.LastUpdated AS ApDate" & vbCrLf &
            "  ,CASE ISNULL(TransInvoice.AccountsPayableNum, 0)" & vbCrLf &
            "  	WHEN 0 THEN 'Draft'" & vbCrLf &
            "     ELSE 'Finalized' END AS ApStatus" & vbCrLf &
            " ,CONVERT(varchar, tblPoFowst.FOWSTNo) + '-' + CONVERT(varchar, tblDevicePo.DeviceNo) + '-' + CONVERT(varchar, tblInvoicePo.PurchaseOrderNo) AS PurchaseOrderText" & vbCrLf &
            " ,CONVERT(varchar, tblReceivingFowst.FOWSTNo) + '-' + CONVERT(varchar, tblReceivingDevice.DeviceNo) + '-' + CONVERT(varchar, TransReceiving.ReceivingNo) AS FullReceivingNum" & vbCrLf &
            " ,tblInvoicePo.PurchaseOrderDate" & vbCrLf &
            " ,TransInvoice.IsCancelled" & vbCrLf &
            " ,(SELECT COUNT(files.ID) FROM TransInvoiceFiles as files WHERE files.InvoiceID = CacheReceivings.ID) as NoOfAttachments " & vbCrLf &
            " ,(SELECT COUNT(files.ID) FROM TransReceivingInvoiceRefImages as files WHERE files.ReceivingID = CacheReceivings.ID) as NoOfReceivingInvRefImages " & vbCrLf &
            " -- ,(SELECT COUNT(ID) FROM TransCRM WHERE Notes LIKE '%'+ CONVERT(varchar, tblReceivingFowst.FOWSTNo) + '-' + CONVERT(varchar, tblReceivingDevice.DeviceNo) + '-' + CONVERT(varchar, TransReceiving.ReceivingNo) +'%') NoOfNotes " & vbCrLf &
            " ,1 AS NoOfNotes " & vbCrLf &
            " FROM CacheReceivings" & vbCrLf &
            " JOIN TransReceiving ON TransReceiving.ID = CacheReceivings.ID" & vbCrLf &
            "  LEFT JOIN TransInvoice ON TransInvoice.ID = TransReceiving.AccountsPayableID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST tblReceivingFowst ON TransReceiving.ReceivingPlaceID = tblReceivingFowst.ID" & vbCrLf &
            "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            "  LEFT JOIN LibraryDevice tblReceivingDevice ON TransReceiving.DeviceID = tblReceivingDevice.ID" & vbCrLf &
            "  LEFT JOIN TransInvoice tblInvoicePo ON tblInvoicePo.ID = TransReceiving.PurchaseOrderID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST tblPoFowst ON tblInvoicePo.FowstID = tblPoFowst.ID" & vbCrLf &
            "  LEFT JOIN LibraryDevice tblDevicePo ON tblInvoicePo.DeviceID = tblDevicePo.ID" & vbCrLf &
            " WHERE CacheReceivings.SupplierID = " & ReadyString(vSupplierID) & vbCrLf
        If vOnlyInactiveAccountsPayable = False Then
            pSql &= " AND ISNULL(TransInvoice.IsCancelled, 0) = 0 " & vbCrLf
        End If
        pSql &= pSqlWhere & vbCrLf

        If vIncludeDraft Then
            pSql &= "  	OR (" & vbCrLf &
             "       CacheReceivings.TotalPayment = 0" & vbCrLf &
             "       AND EXISTS (" & vbCrLf &
             "  	    SELECT *" & vbCrLf &
             "          FROM TransInvoice" & vbCrLf &
             "          JOIN TransReceiving ON TransReceiving.ID = CacheReceivings.ID" & vbCrLf &
             "          AND TransReceiving.AccountsPayableID = TransInvoice.ID" & vbCrLf &
             "          WHERE TransInvoice.TransType = 'AccountsPayable'" & vbCrLf &
             "          AND ISNULL(TransInvoice.AccountsPayableNum, 0) = 0" & vbCrLf &
             "          AND ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
             "      )" & vbCrLf &
             "  )" & vbCrLf
        End If

        pSql &= ")" & vbCrLf &
         " ORDER BY ISNULL(CacheReceivings.InvoiceDueDate, CacheReceivings.InvoiceDate)"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function SuppliersWithReceivable(vFilter As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String
        Dim pWhere As String

        If String.IsNullOrWhiteSpace(vFilter) Then
            pWhere = "  WHERE t.NetInvoiceBalance > 0"
        ElseIf vFilter = "withoutbalance" Then
            pWhere = "  WHERE t.NetInvoiceBalance = 0"
        ElseIf vFilter = "all" Then
            pWhere = " WHERE 1=1"
        ElseIf vFilter = "custom" Then
            'pWhere = vWhere
        Else
            Throw New ApplicationException("Unknown filter: " & vFilter & ". ref: vbKKTLXVVHVB!")
        End If

        pSql = " -- customers with credit" & vbCrLf &
            "SELECT DISTINCT MobizLibraryOrganization.ID" & vbCrLf &
            " ,CASE WHEN MobizLibraryOrganization.CompanyName = '' THEN ISNULL(MobizLibraryOrganization.PersonFirstName + ' ', '') + ISNULL(MobizLibraryOrganization.PersonSurname + ' ', '') ELSE MobizLibraryOrganization.CompanyName END as CustomerName" & vbCrLf &
            " ,MobizLibraryOrganization.IsOrganization" & vbCrLf &
            " ,SUM(t.GrossInvoiceAmount) AS TotalInvoiceAmount" & vbCrLf &
            " ,SUM(t.CashAmount) AS CashAmount" & vbCrLf &
            " ,SUM(t.CheckPaymentAmount) AS CheckPaymentAmount" & vbCrLf &
            " ,SUM(t.CreditCardAmount) AS CreditCardAmount" & vbCrLf &
            " ,SUM(t.DebitCardAmount) AS DebitCardAmount" & vbCrLf &
            " ,SUM(t.SRVAmount) AS SRVAmount" & vbCrLf &
            " ,SUM(t.OtherTypeAmount) AS OtherTypeAmount" & vbCrLf &
            " ,SUM(t.NetInvoiceBalance) AS NetInvoiceBalance" & vbCrLf &
            " ,C.CreditLimit" & vbCrLf &
            " ,C.CreditTerms" & vbCrLf &
            " ,MobizLibraryIndustryClass.Class AS BusinessIndustry" & vbCrLf &
            " , (SELECT TOP 1 ISNULL(CASE WHEN orgwebsite.Website = '' THEN '' ELSE orgwebsite.Website END,'') " & vbCrLf &
            "       FROM LibraryOrganizationContactWebsite as orgwebsite" & vbCrLf &
            "       WHERE(orgwebsite.OrganizationID = MobizLibraryOrganization.ID)) as Website " & vbCrLf &
            " , (SELECT TOP 1 ISNULL(CASE WHEN orgemail.EmailAddress = '' THEN '' ELSE orgemail.EmailAddress END,'') " & vbCrLf &
            "       FROM LibraryOrganizationContactEmail as orgemail" & vbCrLf &
            "       WHERE(orgemail.OrganizationID = MobizLibraryOrganization.ID)) AS Email, " & vbCrLf &
            "   (SELECT TOP 1 " & vbCrLf &
            "       ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + ' ',0) + " & vbCrLf &
            "       ISNULL(' (' + CONVERT(varchar, orgareacode.CallingCode) + ') ',0) + " & vbCrLf &
            "       ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
            "       ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END,0) " & vbCrLf &
            "       FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
            "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
            "               ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
            "           LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
            "               ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
            "           WHERE orgtel.OrganizationID = MobizLibraryOrganization.ID ) AS Telephone, " & vbCrLf &
            "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
            "       CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, orgtelco.Prefix) ELSE '' END + " & vbCrLf &
            "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
            "       FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
            "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
            "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
            "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
            "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
            "           WHERE orgmobile.OrganizationID = MobizLibraryOrganization.ID) AS Mobile " & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " JOIN CacheReceivings t ON t.SupplierID = MobizLibraryOrganization.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID " & vbCrLf &
            " OUTER APPLY( " & vbCrLf &
            "   SELECT CreditLimit, LibraryPaymentTerms.Name AS CreditTerms " & vbCrLf &
            "   FROM LibraryOrganizationSupplier " & vbCrLf &
            "  	LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID " & vbCrLf &
            "   WHERE SupplierID = MobizLibraryOrganization.ID " & vbCrLf &
            " ) as C " & vbCrLf &
            " " & pWhere & vbCrLf &
            " OR EXISTS (" & vbCrLf &
            "  	SELECT *" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            "     JOIN TransReceiving ON TransReceiving.ID = t.ID" & vbCrLf &
            "     AND TransReceiving.AccountsPayableID = TransInvoice.ID" & vbCrLf &
            "     WHERE (TransInvoice.TransType = 'AccountsPayable')" & vbCrLf &
            "     AND ISNULL(TransInvoice.AccountsPayableNum, 0) = 0" & vbCrLf &
            "     AND ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
            "  )" & vbCrLf &
            " GROUP BY MobizLibraryOrganization.ID" & vbCrLf &
            " , MobizLibraryOrganization.CompanyName" & vbCrLf &
            " , MobizLibraryOrganization.PersonFirstName" & vbCrLf &
            " , MobizLibraryOrganization.PersonSurname" & vbCrLf &
            " , MobizLibraryOrganization.IsOrganization" & vbCrLf &
            " , MobizLibraryIndustryClass.Class" & vbCrLf &
            " , C.CreditLimit" & vbCrLf &
            " , C.CreditTerms" & vbCrLf &
            " ORDER BY NetInvoiceBalance DESC, CustomerName "
        ' removed the TransInvoice.TransType = 'ExpenseTicket 2020_09_10__14_14_02_7651
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function DeleteItem(vReceivingID As String, vReceivingStockID As String, vProductID As String, vUnitMeasureID As String) As Integer

        Dim pSql = "DELETE FROM TransReceivingStock" & vbCrLf &
            " WHERE ReceivingID = " & ReadyString(vReceivingID) & vbCrLf &
            " AND ID = " & ReadyString(vReceivingStockID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND UnitMeasureGood = " & ReadyString(vUnitMeasureID)
        Dim pCount = mProData.ExecuteSQL(pSql)

        Return pCount

    End Function

    Public Function IsCancelled(vID As String, ByRef oError As String) As Boolean

        Dim pDR As DataRow

        oError = Nothing
        pDR = ByID(vID)
        If pDR Is Nothing Then
            oError = "Receiving does not exist."
            Return False
        End If

        Return CBool2(pDR("IsCancelled"))

    End Function

    Public Function CancelReceiving(vID As String, vUserID As String, vOrgID As String, ByRef oError As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        oError = Nothing
        mProData.BeginTransaction()
        If IsCancelled(vID, oError) Then
            If oError Is Nothing Then
                oError = "Receiving was already cancelled."
            End If
            mProData.RollbackTransaction()
            Return False
        End If
        pIU = New InsertUpdate("TransReceiving")
        pIU("IsCancelled") = 1
        pIU("CancelledBy") = ReadyString(vUserID)
        pIU("DateCancelled") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)
        AddToInventoryLedger(vID, False, True, vOrgID:=vOrgID)
        mProData.CommitTransaction()

        Return True

    End Function

    Public Function GetByItemSerial(vSerial As String) As DataTable

        Dim pSql = "SELECT TransReceiving.ID" & vbCrLf &
            ",'StockReceiving' AS TransType" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " JOIN TransReceivingStockSerials ON TransReceivingStockSerials.ReceivingID = TransReceiving.ID" & vbCrLf &
            " WHERE TransReceivingStockSerials.SerialNumber = " & ReadyString(vSerial) & vbCrLf &
            " GROUP BY TransReceiving.ID"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetActivity(vID As String) As DataTable

        Dim pLastModifiedByName = BrowserController.ComposePeopleName()
        Dim pSql = "SELECT TOP 1 'Stock Receiving' AS Activity" & vbCrLf &
            ",TransReceiving.ID" & vbCrLf &
            ",'SR-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransReceiving.ReceivingNo, 0)) AS RefNum" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            ",TransReceiving.LastUpdated" & vbCrLf &
            "," & pLastModifiedByName & " AS LastModifiedByName" & vbCrLf &
            ",NULL AS SalesOrderID" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransReceiving.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON TransReceiving.ReceivingPlaceID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE TransReceiving.ID = " & ReadyString(vID)

        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return pDT
        End If

        pDT.DefaultView().Sort = "LastUpdated"
        pDT = pDT.DefaultView().ToTable()

        Return pDT

    End Function

    Public Function GetPoLastPoOrLastCanvass(vReceiving As String, vSupplierID As String, vProductID As String) As DataRow

        Dim pDR = mParent.TransInvoice.LatestPurchaseOrderByVendorProduct(vSupplierID, vProductID)
        If pDR IsNot Nothing Then
            pDR.Table.Columns.Add("PoType")
            pDR("PoType") = "PO"
            Return pDR
        End If

        pDR = mParent.TransInvoice.LatestPurchaseOrderByVendorProduct(vSupplierID, vProductID)
        If pDR IsNot Nothing Then
            pDR.Table.Columns.Add("PoType")
            pDR("PoType") = "LPO"
            Return pDR
        End If

        pDR = mParent.LibraryPriceCanvass.LatestVendorProductCanvass(vSupplierID, vProductID)
        If pDR IsNot Nothing Then
            pDR.Table.Columns.Add("PoType")
            pDR("PoType") = "LC"
            Return pDR
        End If

    End Function

    Public Function GetPurchaseOrderPrice(vReceivingID As String, vProductID As String) As DataRow

        Dim pSql = "SELECT TransInvoiceItems.UnitPrice" & vbCrLf &
            " , TransReceiving.SupplierID" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoice.LastUpdated" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " JOIN TransReceiving ON TransReceiving.PurchaseOrderID = TransInvoice.ID" & vbCrLf &
            " WHERE TransReceiving.ID = " & ReadyString(vReceivingID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function UpdatePoReceivedPercent(vReceivingID As String) As Decimal

        Dim pDrReceiving = ByID(vReceivingID)
        If pDrReceiving Is Nothing Then
            Throw New ApplicationException("The specified receivingID does not exist. ref: vbXNNS6RHVYK!")
        End If
        Dim pPoID = CStr2(pDrReceiving("PurchaseOrderID"))
        If String.IsNullOrWhiteSpace(pPoID) Then
            Return -1
        End If

        Return mParent.TransInvoice.UpdatePoReceivedPercent(pPoID)

    End Function

    Public Function CommonReceivingPlaceID(Optional vDeviceID As String = Nothing) As String

        Dim pSql = "SELECT TOP 1 ReceivingPlaceID" & vbCrLf &
            " FROM TransReceiving" & vbCrLf
        If String.IsNullOrWhiteSpace(vDeviceID) = False Then
            pSql &= " WHERE DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        End If
        pSql &= " GROUP BY ReceivingPlaceID" & vbCrLf &
            " ORDER BY COUNT(*) DESC"

        Dim pOut = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pOut) Then
            If String.IsNullOrWhiteSpace(vDeviceID) = False Then
                Return CommonReceivingPlaceID()
            End If
            pOut = Nothing
        End If

        Return pOut

    End Function
End Class
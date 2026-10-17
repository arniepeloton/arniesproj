Imports Stimulsoft

Public Class TransMemoSalesReturn

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetStocks(ByVal vID As String,
        Optional vProductID As String = Nothing) As DataTable

        Dim pSql As String
        Dim pWhere As String

        vID = ReadyGuid(vID)
        If CStr2(vProductID).Length > 0 Then
            pWhere = " AND TransMemoSalesReturn.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If
        pSql = "SELECT TransMemoSalesReturn.ID" & vbCrLf &
            " ,TransMemo.MemoNumber" & vbCrLf &
            " ,TransMemoSalesReturn.MemoID" & vbCrLf &
            " ,TransMemoSalesReturn.InvoiceID" & vbCrLf &
            " ,RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS InvoiceNum" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,TransMemoSalesReturn.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransMemoSalesReturn.QuantityReturned" & vbCrLf &
            " ,TransMemoSalesReturn.QuantityBad" & vbCrLf &
            " ,TransMemoSalesReturn.UnitMeasure" & vbCrLf &
            " ,TransMemoSalesReturn.UnitPrice" & vbCrLf &
            " ,TransMemoSalesReturn.TotalAmount" & vbCrLf &
            " ,TransMemoSalesReturn.Reason" & vbCrLf &
            " ,TransMemoSalesReturn.CustomerID" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName AS CustomerName" & vbCrLf &
            " ,TransMemoSalesReturn.ActualQuantity" & vbCrLf &
            " /*,TransMemoSalesReturn.OrigQuantity * TransMemoSalesReturn.OrigActualQuantity - TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity AS QtyRemaining*/" & vbCrLf &
            " " & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransMemoSalesReturn.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON TransMemoSalesReturn.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON TransMemoSalesReturn.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
            " WHERE CONVERT(varchar(36), TransMemoSalesReturn.MemoID) = " & ReadyString(vID) & vbCrLf &
            pWhere &
            " ORDER BY MobizLibraryPIMProduct.ProductName" & vbCrLf

        Return mProData.Execute(pSql)

    End Function

    Public Function GetCartMemoByInvoiceID(vInvoiceID As String) As String

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT TOP 1 TransMemoSalesReturn.MemoID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
            " WHERE TransMemo.ApprovedBy IS NULL" & vbCrLf &
            " AND TransMemo.IsCancelled = 0" & vbCrLf &
            " AND TransMemoSalesReturn.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " ORDER BY  TransMemo.LastUpdated DESC" & vbCrLf
        pDR = mProData.ExecuteRow(pSql)

        If pDR IsNot Nothing Then
            Return CStr2(pDR("MemoID"))
        End If

    End Function

    Public Function GetMemoAmount(vMemoID As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT SUM(TransMemoSalesReturn.TotalAmount) AS TotalAmount" & vbCrLf &
            ",TransMemoSalesReturn.InvoiceID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
            " WHERE TransMemo.IsCancelled = 0" & vbCrLf &
            " AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf &
            " GROUP BY TransMemoSalesReturn.InvoiceID" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' vApprovedBy is optional
    ''' </summary>
    ''' <param name="vMemoID"></param>
    ''' <param name="vApprovedBy">optional</param>
    ''' <param name="vUserID"></param>
    ''' <param name="vFowstID"></param>
    ''' <param name="vDeviceID"></param>
    ''' <param name="vIsRefund"></param>
    ''' <remarks></remarks>
    Public Sub Approve(vMemoID As String,
        vApprovedBy As String,
        vUserID As String,
        vFowstID As String,
        vDeviceID As String,
        vIsRefund As Boolean)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDR As DataRow

        pIU = New InsertUpdate("TransMemo")
        pIU("LastUpdated") = "getdate()"
        If CStr2(vApprovedBy) = "" Then
            pIU("RequestedByPeopleID") = ReadyString(vUserID)
        Else
            pIU("ApprovedBy") = ReadyString(vApprovedBy)
        End If
        If IsGuid(vFowstID) Then
            pIU("FowstID") = ReadyString(vFowstID)
        Else
            Throw New ApplicationException("TransMemoSalesReturn.Approve vFowstID must be a GUID. vbCWHUW24ZFJ!")
        End If
        If vUserID IsNot Nothing Then
            pIU("UserID") = ReadyString(vUserID)
        End If
        If IsGuid(vDeviceID) Then
            pIU("DeviceID") = ReadyString(vDeviceID)
        Else
            Throw New ApplicationException("TransMemoSalesReturn.Approve vDeviceID must be a GUID. vbTDGHR7UQKC!")
            pIU("DeviceID") = "null"
        End If
        If vIsRefund Then
            pIU("CheckOutMethod") = "'Refund'"
        Else
            pIU("CheckOutMethod") = "'SRV'"
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vMemoID))
        mProData.ExecuteSQL(pSql)

        If CStr2(vApprovedBy) = "" Then
            pDR = GetMemo(vMemoID)
            If pDR IsNot Nothing Then
                If pDR("DateRequested") Is DBNull.Value Then
                    pIU = New InsertUpdate("TransMemo")
                    pIU("DateRequested") = "LastUpdated"
                    pSql = pIU.GetUpdate("ID = " & ReadyString(vMemoID))
                    mProData.ExecuteSQL(pSql)
                End If
            Else
                ' this is weird
            End If
        End If

    End Sub

    Public Sub Approve(vMemoID As String,
        vApprovedBy As String,
        vApprove As Boolean)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransMemo")
        If vApprove Then
            pIU("ApprovedBy") = ReadyString(vApprovedBy)
            pIU("DateApproved") = "GETDATE()"

            pIU("DeniedBy") = "null"
            pIU("DateDenied") = "null"
        Else
            pIU("DeniedBy") = ReadyString(vApprovedBy)
            pIU("DateDenied") = "GETDATE()"

            pIU("ApprovedBy") = "null"
            pIU("DateApproved") = "null"
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vMemoID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function IsValidCart(vMemoID As String) As Boolean

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT TOP 1 TransMemo.ID AS MemoID" & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " LEFT JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf &
            " WHERE TransMemo.ApprovedBy IS NULL" & vbCrLf &
            " AND TransMemo.IsCancelled = 0" & vbCrLf &
            " AND TransMemo.ID = " & ReadyString(vMemoID) & vbCrLf &
            " AND TransMemoSalesReturn.QuantityReturned + ISNULL(TransMemoSalesReturn.QuantityBad, 0) > 0" & vbCrLf

        pDR = mProData.ExecuteRow(pSql)

        Return pDR IsNot Nothing

    End Function

    Public Sub AddToInventoryLedger(vMemoID As String, ByVal vFowstID As String, Optional vCancel As Boolean = False, Optional vOrgID As String = Nothing)

        Dim pDtStocks, pDtStocksUnique As DataTable
        Dim pError As String
        Dim pDrMemo As DataRow
        Dim pTotalQty As Decimal
        Dim pLedgerID As String
        Dim pLedgerTransType As TransInventoryLedger.eTransType

        mProData.BeginTransaction()
        Try
            If String.IsNullOrWhiteSpace(vOrgID) Then
                vOrgID = WebRequest.Instance.OrgID
            End If
            pDrMemo = GetMemo(vMemoID)
            If pDrMemo Is Nothing Then
                Throw New ApplicationException("AddToInventoryLedger: no memo record")
            End If
            If pDrMemo("FowstID") Is DBNull.Value Then
                Throw New ApplicationException("AddToInventoryLedger: no CustomerAddressID")
            End If
            pDtStocks = GetStocks(vMemoID)

            'For Each pDrStock As DataRow In pDtStocks.Rows
            '    mParent.LibraryStock.SaveIntoInventoryLedger(
            '        vStockID:=CStr2(pDrStock("StockID")),
            '        vTransaction:="Memo " & pDrStock("MemoNumber") & " " & pDrStock("CustomerName") & " (Sales Return)",
            '        vAmount:=pDrStock("UnitPrice"),
            '        vDebit:=0,
            '        vCredit:=pDrStock("QuantityReturned") * pDrStock("ActualQuantity"),
            '        vTransactionType:=LibraryStock.eLedgerTransactionType.LedgerMemo,
            '        vReferenceID:=vMemoID,
            '        vFreight:=0,
            '        vFactory:=0,
            '        vBranchID:=vFowstID)
            'Next pDrStock
            If vCancel Then
                pLedgerTransType = TransInventoryLedger.eTransType.CancelSalesReturn
            Else
                pLedgerTransType = TransInventoryLedger.eTransType.SalesReturn
            End If
            pDtStocks.Columns.Add("TotalQty", GetType(Decimal), "QuantityReturned * ActualQuantity")
            pDtStocksUnique = New DataView(pDtStocks, "TotalQty > 0", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID"})
            For Each pDrStock As DataRow In pDtStocksUnique.Rows
                pTotalQty = pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDrStock("ProductID")))
                'If mParent.LibraryStock.AddToTotalStockOut(CStr2(pDrStock("StockID")), pTotalQty, pError, vBranchID) = False Then
                '    Throw New Exception(pError)
                'End If
                If vCancel Then
                    pTotalQty *= -1
                End If

                Dim pProductID = CStr2(pDrStock("ProductID"))
                Dim pDrLedger = mParent.TransInventoryLedger.ByTransIdAndProductId(vMemoID, pProductID)
                If pDrLedger IsNot Nothing Then
                    Continue For
                End If

                pLedgerID = mParent.TransInventoryLedger.Add(pProductID,
                    CStr2(pDrMemo("FowstID")),
                    vMemoID,
                    pLedgerTransType,
                    CStr2(pDrMemo("UserID")),
                    pTotalQty)

                Dim pDrBom As DataRow
                Dim pDrBomOnSalesReturnBehavior As DataRow

                pDrBomOnSalesReturnBehavior = mParent.LibraryStock.GetBomBehaviorOnSalesReturns(CStr2(pDrStock("ProductID")), vOrgID)

                If Not pDrBomOnSalesReturnBehavior Is Nothing Then
                    If pDrBomOnSalesReturnBehavior("IsSeperateIntoComponentDuringSalesReturns") = True Then

                        pDrBom = mParent.LibraryStock.GetBomHeader(CStr2(pDrStock("ProductID")), vOrgID)

                        If Not pDrBom Is Nothing Then

                            If pDrBom("Jit") = True Then

                                Dim pDtContents As DataTable
                                Dim pChildProductID As String

                                pDtContents = mParent.LibraryStock.GetContents(CStr2(pDrStock("ProductID")), vOrgID)

                                For Each pDrContent As DataRow In pDtContents.Rows
                                    pChildProductID = CStr2(pDrContent("ChildProductID"))
                                    mParent.TransInventoryLedger.Add(
                                        vStockID:=pChildProductID,
                                        vFowstID:=vFowstID,
                                        vTransactionID:=vMemoID,
                                        vTransactionType:=pLedgerTransType,
                                        vUserID:=CStr2(pDrMemo("UserID")),
                                        vQty:=pDrContent("Quantity") * pTotalQty,
                                        vParentID:=pLedgerID,
                                        vSubTransactionType:="Production"
                                    )

                                Next pDrContent

                            End If

                        End If
                    End If
                End If

                Dim pDtRebates As DataTable

                pDtRebates = GetInvoiceItemsRebates(CStr2(pDtStocks.Rows(0).Item("InvoiceID")), CStr2(pDrStock("ProductID")))

                For i As Integer = 0 To pDtRebates.Rows.Count - 1
                    mParent.TransInventoryLedger.Add(
                        vStockID:=CStr2(pDtRebates.Rows(i).Item("ProductID")),
                        vFowstID:=vFowstID,
                        vTransactionID:=vMemoID,
                        vTransactionType:=pLedgerTransType,
                        vUserID:=CStr2(pDrMemo("UserID")),
                        vQty:=pDtRebates.Rows(i).Item("Quantity") * pTotalQty * -1,
                        vParentID:=pLedgerID
                    )
                Next i

            Next pDrStock

            mProData.CommitTransaction()
        Catch ex As Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Sub

    Public Function GetMemoRebatesItems(vMemoID As String) As DataTable
        Dim pDtStocks, pDtStocksUnique As DataTable
        Dim pTotalQty As Decimal
        Dim dtTemp As New DataTable

        dtTemp.Columns.Add("ProductID")
        dtTemp.Columns.Add("Quantity")

        Try
            pDtStocks = GetStocks(vMemoID)

            pDtStocks.Columns.Add("TotalQty", GetType(Decimal), "QuantityReturned * ActualQuantity")
            pDtStocksUnique = New DataView(pDtStocks, "TotalQty > 0", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID"})
            For Each pDrStock As DataRow In pDtStocksUnique.Rows
                pTotalQty = pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDrStock("ProductID")))


                Dim pDtRebates As DataTable

                pDtRebates = GetInvoiceItemsRebates(CStr2(pDtStocks.Rows(0).Item("InvoiceID")), CStr2(pDrStock("ProductID")))

                If pDtRebates.Rows.Count > 0 Then
                    For i As Integer = 0 To pDtRebates.Rows.Count - 1
                        Dim dr As DataRow

                        dr = dtTemp.NewRow
                        dr(0) = CStr2(pDtRebates.Rows(i).Item("ProductID"))
                        dr(1) = pDtRebates.Rows(i).Item("Quantity") * pTotalQty * -1

                        dtTemp.Rows.Add(dr)
                    Next
                End If

            Next pDrStock

            Return dtTemp
        Catch ex As Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Function

    Public Function GetMemo(vMemoID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT TransMemo.FowstID" &
            ",TransMemo.MemoNumber" &
            ",TransMemo.UserID" &
            ",TransMemo.ApprovedBy" & vbCrLf &
            ",TransMemo.DateRequested" & vbCrLf &
            ",TransMemo.DeniedBy" & vbCrLf &
            ",TransMemo.CheckOutMethod" & vbCrLf &
            ",TransMemo.DeviceID" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " FROM TransMemo" &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransMemo.FowstID" & vbCrLf &
            " WHERE TransMemo.ID = " & ReadyString(vMemoID)
        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function GetInvoiceID(vMemoID As String) As String

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT TOP 1 InvoiceID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE MemoID = " & ReadyString(vMemoID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR(0))
        End If

    End Function

    Function GetInvoiceItemsRebates(vInvoiceID As String, vProductID As String) As DataTable
        Dim pSql As String
        Dim pDt As DataTable

        pSql = "SELECT TransInvoiceItemsRebates.Quantity, TransInvoiceItemsRebates.ProductID  from " & vbCrLf & vbCrLf &
                    "TransInvoiceItems inner join TransInvoiceItemsRebates on transinvoiceitems.ModifierGroupID = TransInvoiceItemsRebates.ModifierGroupID " & vbCrLf & vbCrLf &
                "where TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID)

        pDt = mProData.Execute(pSql)

        Return pDt
    End Function

    Public Function GetMemoSalesReturn(vMemoID As String) As DataRow

        Dim pSql = "SELECT InvoiceID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE MemoID = " & ReadyString(vMemoID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function FillMemosFowstID(Optional vMemoID As String = Nothing)

        Dim pSql = "SELECT ID " & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " WHERE FowstID = " & ReadyString(Guid.Empty)
        If vMemoID IsNot Nothing Then
            pSql &= vbCrLf & " AND ID = " & ReadyString(vMemoID)
        End If
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            FillMemoFowstID(CStr2(pDR("ID")))
        Next

    End Function

    Public Function FillMemoFowstID(vMemoID As String) As Boolean

        Dim pSql = " SELECT TransMemoSalesReturn.MemoID" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID" & vbCrLf &
            " WHERE TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return False
        End If

        Dim pIU = New InsertUpdate("TransMemo")
        pIU("FowstID") = ReadyString(pDR("FowstID"))
        pSql = pIU.GetUpdate("ID = " & ReadyString(vMemoID))
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function FillSalesReturnLedgersFowstID(Optional vSalesReturnLedgerID As String = Nothing)

        Dim pSql = "SELECT ID " & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE FowstID = " & ReadyString(Guid.Empty)
        If vSalesReturnLedgerID IsNot Nothing Then
            pSql &= vbCrLf & " AND ID = " & ReadyString(vSalesReturnLedgerID)
        End If
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            FillSalesReturnLedgerFowstID(CStr2(pDR("ID")))
        Next

    End Function

    Public Function FillSalesReturnLedgerFowstID(vSalesReturnLedgerID As String) As Boolean

        Dim pSql = " SELECT TransSalesReturnLedger.ID" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransSalesReturnLedger.SourceInvoiceID" & vbCrLf &
            " WHERE TransSalesReturnLedger.ID = " & ReadyString(vSalesReturnLedgerID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return False
        End If

        Dim pIU = New InsertUpdate("TransSalesReturnLedger")
        pIU("FowstID") = ReadyString(pDR("FowstID"))
        pSql = pIU.GetUpdate("ID = " & ReadyString(vSalesReturnLedgerID))
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function FixSalesReturnDeviceIdFowstId() As Integer

        Dim pOut = 0
        Dim pSql = " SELECT TransMemo.ID AS MemoID" & vbCrLf &
            " ,TransSalesReturnLedger.ID AS LedgerID" & vbCrLf &
            " ,TransSalesReturnLedger.DeviceID AS LedgerDeviceID" & vbCrLf &
            " ,TransMemo.DeviceID AS MemoDeviceID" & vbCrLf &
            " ,TransSalesReturnLedger.FowstID AS LedgerFowstID" & vbCrLf &
            " ,TransMemo.FowstID AS MemoFowstID" & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " JOIN TransSalesReturnLedger ON TransSalesReturnLedger.SourceMemoID = TransMemo.ID" & vbCrLf &
            " WHERE TransSalesReturnLedger.DeviceID IS NULL" & vbCrLf &
            " OR TransMemo.DeviceID IS NULL" & vbCrLf &
            " OR TransSalesReturnLedger.FowstID IS NULL" & vbCrLf &
            " OR TransMemo.FowstID IS NULL"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransSalesReturnLedger")
            If CStr2(pDR("LedgerDeviceID")) = "" Then
                If CStr2(pDR("MemoDeviceID")) = "" Then
                    AddLogFixSalesReturnDeviceIdFowstId("MemoID=" & ReadyGuid(pDR("MemoID")) & "; LedgerID=" & ReadyGuid(pDR("LedgerID")) & "; both MemoDeviceID and LedgerDeviceID are null.")
                    Continue For
                End If
                pIU("DeviceID") = ReadyString(pDR("MemoDeviceID"))
            End If
            If CStr2(pDR("LedgerFowstID")) = "" Then
                If CStr2(pDR("MemoFowstID")) = "" Then
                    AddLogFixSalesReturnDeviceIdFowstId("MemoID=" & ReadyGuid(pDR("MemoID")) & "; LedgerID=" & ReadyGuid(pDR("LedgerID")) & "; both MemoFowstID and LedgerFowstID are null.")
                    Continue For
                End If
                pIU("FowstID") = ReadyString(pDR("MemoFowstID"))
            End If
            If pIU.Count > 0 Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("LedgerID")))
                pOut += mProData.ExecuteSQL(pSql)
            End If

            pIU = New InsertUpdate("TransMemo")
            If CStr2(pDR("MemoDeviceID")) = "" Then
                If CStr2(pDR("LedgerDeviceID")) = "" Then
                    AddLogFixSalesReturnDeviceIdFowstId("MemoID=" & ReadyGuid(pDR("MemoID")) & "; LedgerID=" & ReadyGuid(pDR("LedgerID")) & "; both MemoDeviceID and LedgerDeviceID are null.")
                    Continue For
                End If
                pIU("DeviceID") = ReadyString(pDR("LedgerDeviceID"))
            End If
            If CStr2(pDR("MemoFowstID")) = "" Then
                If CStr2(pDR("LedgerFowstID")) = "" Then
                    AddLogFixSalesReturnDeviceIdFowstId("MemoID=" & ReadyGuid(pDR("MemoID")) & "; LedgerID=" & ReadyGuid(pDR("LedgerID")) & "; both MemoFowstID and LedgerFowstID are null.")
                    Continue For
                End If
                pIU("FowstID") = ReadyString(pDR("LedgerFowstID"))
            End If
            If pIU.Count > 0 Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("MemoID")))
                pOut += mProData.ExecuteSQL(pSql)
            End If
        Next pDR

        Return pOut

    End Function

    Private Sub AddLogFixSalesReturnDeviceIdFowstId(vText)

        AppendToLog(vText, , "FixSalesReturnDeviceIdFowstId vbSR5ANM88PL!")

    End Sub

    'Public Function GetInvoice(vMemoID As String) As DataRow

    '    Dim pSql = "SELECT TransMemoSalesReturn.InvoiceID" & vbCrLf &
    '        " FROM TransMemoSalesReturn.InvoiceID" & vbCrLf &
    '        " WHERE TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID)
    '    Dim pDR = mProData.ExecuteRow(pSql)

    '    Return pDR

    'End Function

    ''' <summary>
    ''' get invoice products and returned and remaining qty
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContents(Optional vInvoiceID As String = Nothing,
        Optional ByVal vProductID As String = Nothing,
        Optional ByVal vCustomerID As String = Nothing,
        Optional vKeywords As String = Nothing,
        Optional vMemoID As String = Nothing,
        Optional vAllowProductsNotInInvoice As Boolean = False) As DataTable

        Dim pSql As String
        Dim pDT As DataTable
        Dim pWhere As String ' removed pWhere2 2021_01_07__11_47_09_6476

        If vProductID IsNot Nothing Then
            pWhere &= " AND MobizLibraryPIMProduct.ID = " & ReadyString(vProductID) & vbCrLf
        End If

        pSql = "SELECT TOP 100 PERCENT MobizLibraryPIMProduct.ID AS ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName + ISNULL('(Rebate Item: ' + CONVERT(varchar(500), CONVERT(decimal(8,0),tblInvoice.Quantity * TransInvoiceItemsRebates.Quantity * TransInvoiceItemsRebates.UnitQuantity)) + ' ' + TransInvoiceItemsRebates.UnitMeasure + ' ' + mb2.ProductName + ')','') AS ProductName" & vbCrLf &
            " ,SUM(tblInvoice.Quantity) AS Quantity" & vbCrLf &
            " ,tblInvoice.ActualQuantity" & vbCrLf &
            " ,tblInvoice.UnitMeasure" & vbCrLf &
            " ,tblInvoice.UnitPrice" & vbCrLf &
            " ,SUM(tblInvoice.TaxAmount) AS TaxAmount" & vbCrLf &
            " ,tblInvoice.InvoiceID" & vbCrLf &
            " ,SUM(tblInvoice.Quantity) AS QtyInvoiced" & vbCrLf

        pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0) + ISNULL(SUM(QuantityBad), 0)" & vbCrLf &
            " 	FROM TransMemoSalesReturn" & vbCrLf &
            " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
            " 	LEFT JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.MemoID = TransMemoSalesReturn.MemoID" & vbCrLf &
            " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf &
            " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf &
            " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf &
            " 	AND TransMemo.IsCancelled = 0" & vbCrLf &
            "   AND (TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
            "      OR TransInvoiceAdjustments.ID IS NULL" & vbCrLf &
            "   )" & vbCrLf
        If CStr2(vMemoID).Length > 0 Then
            pSql &= " 	AND TransMemoSalesReturn.MemoID <> " & ReadyString(vMemoID) & vbCrLf
        End If
        pSql &= " ) AS QtyReturnedOthers" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0)" & vbCrLf &
                " 	FROM TransMemoSalesReturn" & vbCrLf &
                " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
                " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf &
                " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf &
                " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf &
                " 	AND TransMemo.IsCancelled = 0" & vbCrLf
            pSql &= " 	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
            pSql &= " )"
        Else
            pSql &= ",0"
        End If
        pSql &= " AS QtyReturnedOwn" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT ISNULL(SUM(QuantityBad), 0)" & vbCrLf &
                " 	FROM TransMemoSalesReturn" & vbCrLf &
                " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
                " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf &
                " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf &
                " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf &
                " 	AND TransMemo.IsCancelled = 0" & vbCrLf
            pSql &= " 	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
            pSql &= " )"
        Else
            pSql &= ",0"
        End If
        pSql &= " AS QtyReturnedOwnBad" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT TOP 1 TransMemoSalesReturn.Reason" & vbCrLf &
                "  	FROM TransMemoSalesReturn" & vbCrLf &
                "  	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
                "  	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf &
                "  	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf &
                "  	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf &
                "  	AND TransMemo.IsCancelled = 0" & vbCrLf &
                "  	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf &
                "   ORDER BY TransMemoSalesReturn.LastUpdated DESC" & vbCrLf &
                "  ) AS ReturnReason" & vbCrLf
        Else
            pSql &= ",'' AS ReturnReason" & vbCrLf
        End If

        pSql &= " FROM MobizLibraryPIMProduct" & vbCrLf
        pSql &= If(vAllowProductsNotInInvoice, " LEFT", "") & " JOIN (" & vbCrLf &
            " SELECT ProductID" & vbCrLf &
            " ,ActualQuantity" & vbCrLf &
            " ,UnitMeasure" & vbCrLf &
            " ,UnitPrice" & vbCrLf &
            " ,TaxAmount" & vbCrLf &
            " ,InvoiceID" & vbCrLf &
            " ,SUM(Quantity) AS Quantity" & vbCrLf &
            " ,ModifierGroupID" & vbCrLf &
            " FROM (" & vbCrLf &
            " SELECT TransMemoSalesReturn.ProductID" & vbCrLf &
            " ,TransMemoSalesReturn.ActualQuantity" & vbCrLf &
            " ,TransMemoSalesReturn.UnitMeasure" & vbCrLf &
            " ,TransMemoSalesReturn.UnitPrice" & vbCrLf &
            " ,0 AS TaxAmount" & vbCrLf &
            " ," & ReadyString(vInvoiceID) & " AS InvoiceID" & vbCrLf &
            " ,0 AS Quantity" & vbCrLf &
            " ,NULL AS ModifierGroupID" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE 1=1" & vbCrLf
        If vMemoID IsNot Nothing Then
            pSql &= " AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
        End If
        If vInvoiceID IsNot Nothing Then
            pSql &= " AND TransMemoSalesReturn.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        End If

        'pSql &= " UNION" & vbCrLf &
        '    " SELECT  TransInvoiceItems.ProductID" & vbCrLf &
        '    " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
        '    " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
        '    " ,TransInvoiceItems.UnitPrice" & vbCrLf &
        '    " ,TransInvoiceItems.TaxAmount" & vbCrLf &
        '    " ," & ReadyString(vInvoiceID) & " AS InvoiceID" & vbCrLf &
        '    " ,TransInvoiceItems.Quantity" & vbCrLf &
        '    " ,TransInvoiceItems.ModifierGroupID" & vbCrLf &
        '    " FROM TransInvoiceItems" & vbCrLf &
        '    " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
        '    " AND TransInvoice.IsCancelled = 0" & vbCrLf
        'If vInvoiceID IsNot Nothing Then
        '    pSql &= " AND TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        'End If

        pSql &= " ) t" & vbCrLf &
            " GROUP BY ProductID" & vbCrLf &
            " ,ActualQuantity" & vbCrLf &
            " ,UnitMeasure" & vbCrLf &
            " ,UnitPrice" & vbCrLf &
            " ,TaxAmount" & vbCrLf &
            " ,InvoiceID" & vbCrLf &
            " ,ModifierGroupID" & vbCrLf &
            " ) tblInvoice ON tblInvoice.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "LEFT JOIN TransInvoiceItemsRebates on TransInvoiceItemsRebates.ModifierGroupID = tblInvoice.ModifierGroupID" & vbCrLf &
            "LEFT JOIN MobizLibraryPIMProduct mb2 ON TransInvoiceItemsRebates.ProductID = mb2.ID"

        pSql &= " WHERE 1 = 1" & vbCrLf &
            pWhere &
            " GROUP BY MobizLibraryPIMProduct.ID" & vbCrLf &
            "  ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            "  ,MobizLibraryPIMProduct.ProductName + ISNULL('(Rebate Item: ' + CONVERT(varchar(500), CONVERT(decimal(8,0),tblInvoice.Quantity * TransInvoiceItemsRebates.Quantity * TransInvoiceItemsRebates.UnitQuantity)) + ' ' + TransInvoiceItemsRebates.UnitMeasure + ' ' + mb2.ProductName + ')','')" & vbCrLf &
            "  ,tblInvoice.ActualQuantity" & vbCrLf &
            "  ,tblInvoice.UnitMeasure" & vbCrLf &
            "  ,tblInvoice.UnitPrice" & vbCrLf &
            "  ,tblInvoice.InvoiceID" & vbCrLf &
            "  ,tblInvoice.ProductID" & vbCrLf &
            " ORDER BY tblInvoice.ActualQuantity" &
            ",tblInvoice.UnitMeasure" & vbCrLf

        pSql = "WITH CTE AS (" & vbCrLf &
            pSql &
            ") -- end CTE" & vbCrLf &
            " SELECT *" & vbCrLf &
            " ,QtyReturnedOwn * UnitPrice AS SubTotal" & vbCrLf &
            " FROM CTE" & vbCrLf &
            " WHERE QtyReturnedOwn > 0"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CreatePdf(vMemoID As String, vOrgID As String) As String

        Dim pInvoiceID = GetInvoiceID(vMemoID)
        Dim pDrInvoice = mParent.TransInvoice.ById(pInvoiceID)
        Dim pDS = mParent.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pInvoiceID), False)
        Dim pDT = pDS.Tables("Invoices")
        pDT.Columns.Add("CustomerContactInfos")
        pDT.Columns.Add("TotalVatable", GetType(Decimal))
        pDT.Columns.Add("TotalVatAmount", GetType(Decimal))

        Dim pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(CStr2(pDrInvoice("CustomerID")))
        pDT(0)("CustomerContactInfos") = CStr2(pHeaderInfo.ContactInfos)

        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(vOrgID)
        Dim pReport = New ProReport()
        pReport.AutoFillHeader = False
        pReport.SourceFilename = "SalesReturn"
        pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
        pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos

        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(CStr2(pDS.Tables("Invoices").Rows(0)("CustomerID")))
        pReport.Variables("CustomerName") = pHeaderInfo.CompanyName
        pReport.Variables("CustomerAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos

        pReport.AddTable("Invoices", pDS.Tables("Invoices"))
        Dim pDtStocks = GetContents(vInvoiceID:=pInvoiceID, vMemoID:=vMemoID)
        pReport.AddTable("Stocks", pDtStocks)
        pReport.ExportXmlFile()
        Dim pBaseFilename As String = DateTimeStamp() & ".pdf"
        Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

        Return pBaseFilename

    End Function

    Public Function SalesReturnLedgerVsSalesReturn(vLedgerDateFrom As Date?,
        vLedgerDateTo As Date?,
        vIncludeNoIssues As Boolean) As DataTable

        If vLedgerDateFrom Is Nothing _
            AndAlso vLedgerDateTo Is Nothing Then

            Throw New ApplicationException("LedgerDateFrom & LedgerDateTo cannot be both null.")
        End If

        Dim pSql = " -- TransSalesReturnLedger vs TransMemoSalesReturn" & vbCrLf &
            " SELECT tblLedger.LastUpdated" & vbCrLf &
            " ,tblLedger.Amount AS LedgerAmount" & vbCrLf &
            " ,tblLedger.SourceMemoID" & vbCrLf &
            " ,tblSalesReturn.Amount AS ReturnAmount" & vbCrLf &
            " ,tblSalesReturn.InvoiceID" & vbCrLf &
            " ,tblSalesReturn.MemoID" & vbCrLf &
            " FROM (" & vbCrLf &
            " 	SELECT SUM(TransSalesReturnLedger.GrossSrvAmount + TransSalesReturnLedger.GrossRefundAmount) AS Amount" & vbCrLf &
            " 	,TransSalesReturnLedger.SourceInvoiceID" & vbCrLf &
            "   ,TransSalesReturnLedger.LastUpdated" & vbCrLf &
            "   ,TransSalesReturnLedger.SourceMemoID" & vbCrLf &
            "  	FROM TransSalesReturnLedger" & vbCrLf &
            "   WHERE 1=1" & vbCrLf

        If vLedgerDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(DATE, TransSalesReturnLedger.LastUpdated) >= " & ReadyString(vLedgerDateFrom.Value.ToString(cGlobal.FormatDateSql)) & vbCrLf
        End If
        If vLedgerDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(DATE, TransSalesReturnLedger.LastUpdated) <= " & ReadyString(vLedgerDateTo.Value.ToString(cGlobal.FormatDateSql)) & vbCrLf
        End If

        pSql &= "  	GROUP BY TransSalesReturnLedger.SourceInvoiceID" & vbCrLf &
            "   ,TransSalesReturnLedger.LastUpdated" & vbCrLf &
            "   ,TransSalesReturnLedger.SourceMemoID" & vbCrLf &
            " ) tblLedger" & vbCrLf &
            " JOIN (" & vbCrLf &
            " 	SELECT SUM(TransMemoSalesReturn.TotalAmount) AS Amount" & vbCrLf &
            "   ,TransMemoSalesReturn.InvoiceID" & vbCrLf &
            "   ,TransMemoSalesReturn.MemoID" & vbCrLf &
            "   FROM TransMemoSalesReturn" & vbCrLf &
            "   GROUP BY TransMemoSalesReturn.InvoiceID" & vbCrLf &
            "   ,TransMemoSalesReturn.MemoID" & vbCrLf &
            " ) tblSalesReturn" & vbCrLf &
            " ON tblLedger.SourceInvoiceID = tblSalesReturn.InvoiceID" & vbCrLf &
            " WHERE tblLedger.SourceMemoID = tblSalesReturn.MemoID" & vbCrLf
        If vIncludeNoIssues = False Then
            pSql &= " AND ABS(tblLedger.Amount - tblSalesReturn.Amount) > 0.001" & vbCrLf
        End If
        pSql &= " ORDER BY tblLedger.LastUpdated DESC"
        Dim pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Function Reconstructable(vMemoID As String, ByRef oError As String, Optional ByRef oDtMemoSalesReturn As DataTable = Nothing) As DataTable

        oError = Nothing
        Dim pSql = "SELECT * " & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE MemoID = " & ReadyString(vMemoID)
        oDtMemoSalesReturn = mProData.Execute(pSql)

        pSql = "SELECT SUM(TransSalesReturnLedger.GrossSrvAmount + TransSalesReturnLedger.GrossRefundAmount) AS Amount" & vbCrLf &
            ",TransSalesReturnLedger.SourceInvoiceID" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE TransSalesReturnLedger.SourceMemoID = " & ReadyString(vMemoID) & vbCrLf &
            " GROUP BY TransSalesReturnLedger.SourceInvoiceID" & vbCrLf
        Dim pDtLedger = mProData.Execute(pSql)
        If pDtLedger.Rows.Count <> 1 Then
            oError = "Expected sales return ledger row count is 1. actual is " & pDtLedger.Rows.Count & ". vbJLKL9TYVTH!"
            Return Nothing
        End If
        Dim pDrLedger = pDtLedger.Rows(0)

        pSql = "SELECT ID" & vbCrLf &
            ",ProductID" & vbCrLf &
            ",OrderedQuantity" & vbCrLf &
            ",Quantity" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitMeasure" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",SubTotal" & vbCrLf &
            ",TaxAmount" & vbCrLf &
            ",DiscountTax" & vbCrLf &
            "FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(pDrLedger("SourceInvoiceID"))
        Dim pDtInvoiceItems = mProData.Execute(pSql)

        pDtInvoiceItems.Columns.Add("Exclude", GetType(Boolean))
        For Each pDrInvoiceItem In pDtInvoiceItems.Rows
            For Each pDrSalesReturn In oDtMemoSalesReturn.Rows
                If CStr2(pDrInvoiceItem("ProductID")) = CStr2(pDrSalesReturn("ProductID")) Then
                    If pDrInvoiceItem("UnitMeasure") = pDrSalesReturn("UnitMeasure") Then
                        pDrInvoiceItem("Exclude") = True
                    End If
                End If
            Next
        Next

        Dim pLackingAmount = CDec2(pDrLedger("Amount")) - CDec2(oDtMemoSalesReturn.Compute("SUM(TotalAmount)", ""))
        If pLackingAmount = 0 Then
            oError = "The lacking amount is zero."
            Return pDtInvoiceItems
        End If
        Dim pDtInvoiceItemsFiltered = New DataView(pDtInvoiceItems, "ISNULL(Exclude, 0) = 0", "", DataViewRowState.CurrentRows).ToTable()
        Dim pResults As New List(Of List(Of DataRow))
        FindCombinations(pDtInvoiceItemsFiltered.Select(), 0, New List(Of DataRow), 0, pLackingAmount, pResults)

        If pResults.Count = 0 Then
            oError = "No valid combos"
            Return pDtInvoiceItemsFiltered
        End If
        Dim pResult = pResults(0)
        Dim pDtOut = pDtInvoiceItems.Clone()
        For Each pDR As DataRow In pResult
            pDtOut.ImportRow(pDR)
        Next

        Return pDtOut

    End Function

    Shared Sub FindCombinations(vRows As DataRow(), vStartIndex As Integer,
        vCurrentCombo As List(Of DataRow), vCurrentTotal As Decimal,
        vTarget As Decimal, vResults As List(Of List(Of DataRow)))

        If vCurrentTotal = vTarget Then
            vResults.Add(New List(Of DataRow)(vCurrentCombo))
            Return
        End If

        If vCurrentTotal > vTarget Then
            Return
        End If

        For pIndex = vStartIndex To vRows.Count - 1
            vCurrentCombo.Add(vRows(pIndex))
            FindCombinations(vRows, pIndex + 1, vCurrentCombo, vCurrentTotal + vRows(pIndex)("SubTotal"), vTarget, vResults)
            vCurrentCombo.RemoveAt(vCurrentCombo.Count - 1)
        Next

    End Sub

    Public Function Reconstruct(vMemoID As String, vInvoiceItemIDs As String(), ByRef oError As String) As Boolean

        oError = Nothing

        If vInvoiceItemIDs.Length = 0 Then
            oError = "Please input at least 1 row to reconstruct. vbWX3JJ8QVFF!"
            Return False
        End If
        Dim pDtMemoSalesReturn As DataTable
        Dim pDtReconstructable = Reconstructable(vMemoID, oError, oDtMemoSalesReturn:=pDtMemoSalesReturn)
        If oError IsNot Nothing Then
            Return Nothing
        End If

        ' check validity
        Dim pDtInvoiceItems As DataTable
        For Each pInvoiceItemID In vInvoiceItemIDs
            Dim pDRs = pDtReconstructable.Select("ID = " & ReadyString(pInvoiceItemID))
            If pDRs.Length = 0 Then
                oError = "Input row, " & pInvoiceItemID & ", is not among the reconstructable rows."
                Return False
            End If
            If pDtInvoiceItems Is Nothing Then
                pDtInvoiceItems = pDRs(0).Table.Clone()
            End If
            pDtInvoiceItems.ImportRow(pDRs(0))
        Next

        Dim pDrMemoSalesReturn = pDtMemoSalesReturn.Rows(0)
        Dim pIU = New InsertUpdate("TransMemoSalesReturn")
        pIU("MemoID") = ReadyString(vMemoID)
        pIU("CustomerID") = ReadyString(pDrMemoSalesReturn("CustomerID"))
        pIU("Reason") = ReadyString(pDrMemoSalesReturn("Reason"))
        pIU("InvoiceID") = ReadyString(pDrMemoSalesReturn("InvoiceID"))

        mProData.BeginTransaction()
        For Each pDrInvoiceItem In pDtInvoiceItems.Rows
            pIU("ID") = "NEWID()"
            pIU("ProductID") = ReadyString(pDrInvoiceItem("ProductID"))
            pIU("QuantityReturned") = CDec2(pDrInvoiceItem("Quantity"))
            pIU("ActualQuantity") = CDec2(pDrInvoiceItem("ActualQuantity"))
            pIU("UnitPrice") = CDec2(pDrInvoiceItem("UnitPrice"))
            pIU("UnitMeasure") = ReadyString(pDrInvoiceItem("UnitMeasure"))
            pIU("TotalAmount") = CDec2(pDrInvoiceItem("SubTotal"))
            pIU("TaxAmount") = CDec2(pDrInvoiceItem("TaxAmount"))
            pIU("DiscountTax") = CDec2(pDrInvoiceItem("DiscountTax"))
            Dim pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        ' reevalute
        Dim pError As String
        pDtReconstructable = Reconstructable(vMemoID, pError, oDtMemoSalesReturn:=pDtMemoSalesReturn) ' result is for debugging only
        If pError = "The lacking amount is zero." Then
            mProData.CommitTransaction()
            Return True
        End If

        mProData.RollbackTransaction()

        Return False

    End Function
End Class

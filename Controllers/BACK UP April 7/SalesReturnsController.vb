Public Class SalesReturnsController
    Inherits System.Web.Mvc.Controller

#Region " index "
    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Sales Return")) Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetContents() As String

        Dim pInvoiceID As String
        Dim pCustomerID As String
        Dim pKeywords As String
        Dim pDS As DataSet
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) Then
            pInvoiceID = ReadyGuid(pInvoiceID)
            pDS = GetByInvoiceID(pInvoiceID)
        Else
            pCustomerID = Common.WebRequest.Data("CustomerID")
            If IsGuid(pCustomerID) Then
                pKeywords = Common.WebRequest.Data("Keywords")
                pDS = GetByInvoiceID(, , pCustomerID, pKeywords)
            Else
                Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
            End If
        End If

        If pDS IsNot Nothing Then
            pOut = New JsonDictionary
            pOut.Add("Stocks", pDS.Tables("Stocks"))
            pOut.Add("Units", pDS.Tables("Units"))
            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Result", "no data")
        End If

    End Function

    ''' <summary>
    ''' get invoice products and returned and remaining qty
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetByInvoiceID(Optional vInvoiceID As String = Nothing, _
        Optional ByVal vProductID As String = Nothing,
        Optional ByVal vCustomerID As String = Nothing,
        Optional vKeywords As String = Nothing,
        Optional vMemoID As String = Nothing) As DataSet

        Dim pSql As String
        Dim pDT As DataTable
        Dim pWhere As String

        If vProductID IsNot Nothing Then
            pWhere &= " AND tblInvoice.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If
        If vCustomerID IsNot Nothing Then
            pWhere &= " AND tblInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If vInvoiceID IsNot Nothing Then
            pWhere &= " AND tblInvoice.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        End If

        pSql = "SELECT tblInvoice.InvoiceID" & vbCrLf & _
            " ,tblInvoice.ProductID" & vbCrLf & _
            " ,RIGHT('0000000' + CONVERT(varchar, tblInvoice.InvoiceNumber), 7) AS InvoiceNum" & vbCrLf & _
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf & _
            " ,tblInvoice.InvoiceDate" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,tblInvoice.Quantity" & vbCrLf & _
            " ,tblInvoice.ActualQuantity" & vbCrLf & _
            " ,tblInvoice.UnitMeasure" & vbCrLf & _
            " ,tblInvoice.UnitPrice" & vbCrLf & _
            " ,tblInvoice.DiscountedUnitPrice" & vbCrLf & _
            " ,tblInvoice.SubTotal" & vbCrLf & _
            " ,tblInvoice.TaxAmount" & vbCrLf & _
            " ,tblInvoice.Quantity AS QtyInvoiced" & vbCrLf

        pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0)" & vbCrLf & _
            " 	FROM TransMemoSalesReturn" & vbCrLf & _
            " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
            " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
            " 	AND TransMemoSalesReturn.CustomerID = tblInvoice.CustomerID" & vbCrLf & _
            " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
            " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
            " 	AND TransMemo.IsCancelled = 0" & vbCrLf
        If CStr2(vMemoID).Length > 0 Then
            pSql &= " 	AND TransMemoSalesReturn.MemoID <> " & ReadyString(vMemoID) & vbCrLf
        End If
        pSql &= " ) AS QtyReturnedOthers" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0)" & vbCrLf & _
                " 	FROM TransMemoSalesReturn" & vbCrLf & _
                " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
                " 	AND TransMemoSalesReturn.CustomerID = tblInvoice.CustomerID" & vbCrLf & _
                " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
                " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
                " 	AND TransMemo.IsCancelled = 0" & vbCrLf
            pSql &= " 	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
            pSql &= " )"
        Else
            pSql &= ",0"
        End If
        pSql &= " AS QtyReturnedOwn" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT TOP 1 TransMemoSalesReturn.Reason" & vbCrLf & _
                "  	FROM TransMemoSalesReturn" & vbCrLf & _
                "  	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                "  	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
                "  	AND TransMemoSalesReturn.CustomerID = tblInvoice.CustomerID" & vbCrLf & _
                "  	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
                "  	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
                "  	AND TransMemo.IsCancelled = 0" & vbCrLf & _
                "  	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf & _
                "   ORDER BY TransMemoSalesReturn.LastUpdated DESC" & vbCrLf & _
                "  ) AS ReturnReason" & vbCrLf
        Else
            pSql &= ",'' AS ReturnReason" & vbCrLf
        End If

        pSql &= " FROM (" & vbCrLf & _
            " 	SELECT TransInvoiceItems.ProductID" & vbCrLf & _
            "   ,TransInvoiceItems.Quantity" & vbCrLf & _
            "   ,TransInvoiceItems.ActualQuantity" & vbCrLf & _
            "   ,TransInvoiceItems.UnitMeasure" & vbCrLf & _
            " 	,TransInvoiceItems.UnitPrice" & vbCrLf & _
            " 	,TransInvoiceItems.DiscountedUnitPrice" & vbCrLf & _
            "   ,TransInvoice.ID AS InvoiceID    " & vbCrLf & _
            "   ,TransInvoice.InvoiceNumber" & vbCrLf & _
            "   ,TransInvoice.InvoiceDate" & vbCrLf & _
            "   ,TransInvoiceItems.SubTotal" & vbCrLf & _
            "   ,TransInvoiceItems.TaxAmount" & vbCrLf & _
            "   ,TransInvoice.CustomerID" & vbCrLf & _
            "   FROM TransInvoiceItems" & vbCrLf & _
            " 	JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
            "   AND TransInvoice.IsCancelled = 0 " & vbCrLf & _
            " ) tblInvoice" & vbCrLf & _
            " JOIN MobizLibraryOrganization ON tblInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON tblInvoice.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf
        pSql &= " WHERE 1 = 1" & vbCrLf & _
            pWhere &
            " ORDER BY tblInvoice.ActualQuantity" &
            ",tblInvoice.UnitMeasure"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Dim pOut As DataSet
        Dim pDtStocks As DataTable

        Dim pReturnedPrice As String
        Dim pBaseUnitPrice, pBaseUnitPriceMinimum As Decimal
        Dim pTotalAmount As Decimal
        Dim pTotalBaseQty As Integer

        If pDT.Rows.Count > 0 Then
            pDT.TableName = "Units"
            pOut = New DataSet
            pDT = New DataView(pDT, "", "ProductName", DataViewRowState.CurrentRows).ToTable
            pDT = New DataView(pDT, "Quantity > 0", "", DataViewRowState.CurrentRows).ToTable
            pDtStocks = pDT.DefaultView.ToTable(True, {"ProductID", "ProductNo", "ProductName"})
            pDtStocks.TableName = "Stocks"
            With pDT.Columns
                .Remove("ProductNo")
                .Remove("ProductName")
            End With

            pReturnedPrice = WebRequest.Instance.Business.SystemDefaults.SalesReturntReturnedPrice
            If pReturnedPrice = "Average Price" _
                OrElse pReturnedPrice = "Minimum Price" Then

                For Each pDrStock As DataRow In pDtStocks.Rows
                    pDT.DefaultView.RowFilter = "ProductID = " & ReadyString(pDrStock("ProductID"))
                    If pDT.DefaultView.Count <= 1 Then Continue For
                    If pReturnedPrice = "Minimum Price" Then
                        pBaseUnitPriceMinimum = Decimal.MaxValue
                        For Each pDrvUnit As DataRowView In pDT.DefaultView
                            pBaseUnitPrice = pDrvUnit("UnitPrice") / Math.Max(pDrvUnit("ActualQuantity"), 1) ' just in case ActualQuantity is zero
                            If pBaseUnitPrice < pBaseUnitPriceMinimum Then
                                pBaseUnitPriceMinimum = pBaseUnitPrice
                            End If
                        Next pDrvUnit

                        For Each pDrvUnit As DataRowView In pDT.DefaultView
                            pDrvUnit("UnitPrice") = pBaseUnitPrice * Math.Max(pDrvUnit("ActualQuantity"), 1) ' just in case ActualQuantity is zero
                        Next pDrvUnit
                    ElseIf pReturnedPrice = "Average Price" Then
                        pTotalAmount = 0
                        pTotalBaseQty = 0
                        For Each pDrvUnit As DataRowView In pDT.DefaultView
                            pTotalAmount += pDrvUnit("Quantity") * pDrvUnit("UnitPrice")
                            pTotalBaseQty += pDrvUnit("Quantity") * Math.Max(pDrvUnit("ActualQuantity"), 1) ' just in case ActualQuantity is zero
                        Next pDrvUnit
                        pBaseUnitPrice = pTotalAmount / pTotalBaseQty
                        For Each pDrvUnit As DataRowView In pDT.DefaultView
                            pDrvUnit("UnitPrice") = pBaseUnitPrice * Math.Max(pDrvUnit("ActualQuantity"), 1) ' just in case ActualQuantity is zero
                        Next pDrvUnit
                    End If
                Next pDrStock
            End If

            pOut.Tables.Add(pDT)
            pOut.Tables.Add(pDtStocks)

            Return pOut
        End If

    End Function

    <HttpPost()>
    Public Function Cart() As String

        Dim pMemoID As String
        Dim pOut As JsonDictionary
        Dim pDtUnits As DataTable
        Dim pDtStocks As DataTable ' unique

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pInvoiceID As String

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid InvoiceID")
        End If
        pInvoiceID = ReadyGuid(pInvoiceID)

        pMemoID = Common.WebRequest.Data("MemoID")
        If CStr2(pMemoID).Length > 0 Then
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If
            If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                pMemoID = Nothing
            End If
        End If
        If CStr2(pMemoID).Length = 0 Then
            pMemoID = WebRequest.Instance.Business.TransMemoSalesReturn.GetCartMemoByInvoiceID(pInvoiceID)
        End If

        pOut = New JsonDictionary
        If CStr2(pMemoID).Length > 0 Then
            pDtUnits = WebRequest.Instance.Business.TransMemoSalesReturn.GetStocks(pMemoID)
            If pDtUnits.Rows.Count > 0 Then
                pDtUnits = New DataView(pDtUnits, "QuantityReturned > 0", "ProductName", DataViewRowState.CurrentRows).ToTable
                pDtStocks = pDtUnits.DefaultView.ToTable(True, {"ProductID", "ProductNo", "ProductName"})
                With pDtUnits.Columns
                    .Remove("ProductNo")
                    .Remove("ProductName")
                End With
                pOut.Add("Stocks", pDtStocks)
                pOut.Add("Units", pDtUnits)
                pOut.Add("MemoID", pMemoID)
            Else
                pOut.Add("Cart", "no item")
            End If
        Else
            pOut.Add("Cart", "none")
        End If

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function DeleteStock() As String

        Dim pMemoID As String
        Dim pSql As String
        Dim pStockID As String
        Dim pCount As Integer
        Dim pOut As JsonDictionary

        pMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
        End If

        pStockID = Common.WebRequest.Data("StockID")
        If CStr2(pStockID).Length = 0 Then
            Return JsonDictionary.Serialize("Error", "No Stock")
        End If
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock")
        End If
        pStockID = ReadyGuid(pStockID)

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If

        pSql = "DELETE FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE MemoID = " & ReadyString(pMemoID) & vbCrLf &
            " AND ProductID = " & ReadyString(pStockID)
        pCount = Common.WebRequest.ProData.ExecuteSQL(pSql, True)
        If pCount > 0 Then
            pOut = New JsonDictionary
            pOut.Add("Result", "ok")
            If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                pOut.Add("NewMemo", 1)
            End If
            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Error", "Data already changed. Nothing was deleted")
        End If


    End Function

    <HttpPost()>
    Public Function Verify() As String

        Dim pDtCart As DataTable
        Dim pDT As DataTable
        Dim pMemoID As String
        Dim pOut As JsonDictionary

        pMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If
        pDtCart = WebRequest.Instance.Business.TransMemoSalesReturn.GetStocks(pMemoID)
        If pDtCart.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "no data")
        End If

        pOut = New JsonDictionary
        pOut.Add("Result", "ok")
        pOut.Add("TotalAmount", CDec2(pDtCart.Compute("SUM(TotalAmount)", "")))
        pOut.Add("SkuCount", New DataView(pDtCart, "", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID"}).Rows.Count)

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function CheckOut() As String

        Dim pMemoID As String
        Dim pDtCart As DataTable
        Dim pInputTotalAmount, pMemoTotalAmount As Decimal
        Dim pInputSkuCount, pMemoSkuCount As Integer
        Dim pOut As JsonDictionary

        pMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
        End If

        pInputTotalAmount = CDec2(Common.WebRequest.Data("TotalAmount"))
        If pInputTotalAmount <= 0 Then
            Return JsonDictionary.Serialize("Error", "Invalid Total Amount")
        End If

        pInputSkuCount = CDec2(Common.WebRequest.Data("SkuCount"))
        If pInputSkuCount <= 0 Then
            Return JsonDictionary.Serialize("Error", "Invalid SKU Count")
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid cart")
        End If
        pDtCart = WebRequest.Instance.Business.TransMemoSalesReturn.GetStocks(pMemoID)
        If pDtCart.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "no data")
        End If

        pMemoTotalAmount = CDec2(pDtCart.Compute("SUM(TotalAmount)", ""))
        If pInputTotalAmount <> pMemoTotalAmount Then
            Return JsonDictionary.Serialize("Error", "There's a difference of " & Format(pInputTotalAmount - pMemoTotalAmount, cGlobal.FormatCurrency) & " between the verified total amount and the memo total amount.")
        End If

        pMemoSkuCount = New DataView(pDtCart, "", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID"}).Rows.Count
        If pInputSkuCount <> pMemoSkuCount Then
            Return JsonDictionary.Serialize("Error", "There's a difference of " & Format(pInputSkuCount - pMemoSkuCount, cGlobal.FormatCurrency) & " between the verified SKU count and the memo SKU count.")
        End If

        WebRequest.Instance.ProData.BeginTransaction()
        WebRequest.Instance.Business.TransMemoSalesReturn.Approve(pMemoID, WebSession.Instance.PeopleUserID, WebSession.Instance.PeopleUserID, WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID)
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger(pMemoID, WebRequest.Instance.FowstID)
        WebRequest.Instance.Business.TransSalesReturnLedger.Save(pMemoID, CDec2(Common.WebRequest.Data("Refund")) <> 0, WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID)
        WebRequest.Instance.ProData.CommitTransaction()

        Return JsonDictionary.Serialize("Result", "ok")

    End Function
#End Region

#Region " Stock action "
    Public Function Stock() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetInfo() As String

        Dim pProductID As String
        Dim pOut As JsonDictionary
        Dim pMemoID As String
        Dim pInvoiceID As String
        Dim pDS As DataSet
        Dim pDrMemo As DataRow

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pProductID = WebRequest.Instance.Data("ProductID")
        If IsGuid(pProductID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
        End If
        pProductID = ReadyGuid(pProductID)

        pInvoiceID = WebRequest.Instance.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
        End If
        pInvoiceID = ReadyGuid(pInvoiceID)

        pMemoID = Common.WebRequest.Data("MemoID")
        If CStr2(pMemoID).Length > 0 Then
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If
            pMemoID = ReadyGuid(pMemoID)
            If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid cart")
            End If
        End If

        pDrMemo = GetMemoByInvoiceID(pInvoiceID, pMemoID)
        If pDrMemo IsNot Nothing Then
            pMemoID = CStr2(pDrMemo("ID"))
        End If

        pDS = GetByInvoiceID(pInvoiceID, pProductID, , , pMemoID)
        If pDS IsNot Nothing Then
            pOut = New JsonDictionary
            pOut.Add("Stocks", pDS.Tables("Stocks"))
            pOut.Add("Units", pDS.Tables("Units"))
            pOut.Add("AllowDecimal", If(WebRequest.Instance.Business.LibraryStock.GetAllowDecimal(pProductID), 1, 0))
            If CDec2(Common.WebRequest.Data("GetReturnsReasons")) = 1 Then
                GetReturnsReasons(pOut)
            End If
            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Error", "No data found")
        End If

    End Function

    Private Sub GetReturnsReasons(ByRef rJson As JsonDictionary)

        With rJson.AddList("ReturnsReasons")
            .Add("Change of mind")
            .Add("Excess quantity ordered")
            .Add("Goods no longer needed")
            .Add("Quality of goods does not meet expectations")
            .Add("Wrong goods ordered")
            .Add("Defective goods")
        End With

    End Sub

    <HttpPost()>
    Public Function Update() As String

        Dim pStockID As String
        Dim pInvoiceID As String
        Dim pError As String

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid InvoiceID")
        End If
        pInvoiceID = ReadyGuid(pInvoiceID)

        pStockID = Common.WebRequest.Data("StockID")
        If CStr2(pStockID).Length = 0 Then
            Return JsonDictionary.Serialize("Error", "No Stock")
        End If
        If IsGuid(pStockID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Stock")
        End If
        pStockID = ReadyGuid(pStockID)

        If Common.WebRequest.ProData.CompareLocalTimeAndSqlServerTime(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pSql As String
        Dim pIU As InsertUpdate

        Dim pDrMemo As DataRow
        Dim pDrInvoice As DataRow
        Dim pMemoID As String
        Dim pCustomerID As String
        Dim pReturnQty, pRemainingQty As Decimal
        Dim pDsHistory As DataSet
        Dim pDtHistory As DataTable
        Dim pDvHistory As DataView

        pMemoID = Common.WebRequest.Data("MemoID")
        If CStr2(pMemoID).Length > 0 Then
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If
            If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid cart")
            End If
        End If

        Common.WebRequest.ProData.BeginTransaction()

        pDrMemo = GetMemoByInvoiceID(pInvoiceID, pMemoID)
        If pDrMemo IsNot Nothing Then
            pMemoID = CStr2(pDrMemo("ID"))
            pCustomerID = CStr2(pDrMemo("CustomerID"))

            pSql = "DELETE FROM TransMemoSalesReturn" & vbCrLf &
                " WHERE MemoID = " & ReadyString(pMemoID) & vbCrLf &
                " AND InvoiceID = " & ReadyString(pInvoiceID) & vbCrLf &
                " AND ProductID = " & ReadyString(pStockID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Else
            pMemoID = Guid.NewGuid.ToString
            pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
            pCustomerID = CStr2(pDrInvoice("CustomerID"))
        End If
        pDsHistory = GetByInvoiceID(pInvoiceID, pStockID, , , pMemoID)
        pDtHistory = pDsHistory.Tables("Units")
        pDtHistory.Columns.Add("SubQty", GetType(Decimal), "(QtyInvoiced - QtyReturnedOthers)")
        For Each pStock As Object In Common.WebRequest.AllData("Stocks")
            If CDec2(pStock("Qty")) > 0 Then
                pDvHistory = New DataView(pDtHistory, "ActualQuantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
                If pDvHistory.Count = 1 Then
                    pRemainingQty = CDec2(pDtHistory.Compute("SUM(SubQty)", "ActualQuantity = " & CDec2(pStock("UnitQty"))))
                    pReturnQty = CDec2(pStock("Qty"))
                    If pRemainingQty < pReturnQty Then
                        Common.WebRequest.ProData.RollbackTransaction()
                        Return JsonDictionary.Serialize("Error", "Return Quantity (" & pReturnQty & ") must not be greater than Remaining Quantity (" & pRemainingQty & " " & pDvHistory(0)("UnitMeasure") & ")")
                    End If
                Else
                    Common.WebRequest.ProData.RollbackTransaction()
                    Return JsonDictionary.Serialize("Error", "ActualQuantity of " & CDec2(pStock("UnitQty")) & " has " & pDvHistory.Count & " records")
                End If
            End If
        Next pStock

        pIU = New InsertUpdate("TransMemo")
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("LastUpdated") = "GETDATE()"
        pIU("Reason") = "'Sales Returns'"
        pIU("FowstID") = ReadyString(WebRequest.Instance.FowstID)
        If pDrMemo IsNot Nothing Then
            pSql = pIU.GetUpdate("ID = " & ReadyString(pMemoID))
        Else
            pIU("ID") = ReadyString(pMemoID)
            pSql = pIU.GetInsert
        End If
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        '// save stocks
        For Each pStock As Object In Common.WebRequest.AllData("Stocks")
            If CDec2(pStock("Qty")) > 0 Then
                pDvHistory = New DataView(pDtHistory, "ActualQuantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
                ' assume that pDvHistory has 1 item only

                pIU = New InsertUpdate("TransMemoSalesReturn")
                pIU("ID") = "NEWID()"
                pIU("MemoID") = ReadyString(pMemoID)
                pIU("CustomerID") = ReadyString(pCustomerID)
                pIU("LastUpdated") = "GETDATE()"

                pIU("InvoiceID") = ReadyString(pInvoiceID)
                pIU("ProductID") = ReadyString(pStockID)
                pIU("QuantityReturned") = CDec2(pStock("Qty"))
                pIU("UnitMeasure") = ReadyString(pDvHistory(0)("UnitMeasure")) ' ReadyString(pStock("UnitMeasure"))
                pIU("ActualQuantity") = CDec2(pDvHistory(0)("ActualQuantity")) 'CDec2(pStock("UnitQty"))
                pIU("UnitPrice") = CDec2(pDvHistory(0)("DiscountedUnitPrice")) ' CDec2(pStock("UnitPrice"))
                pIU("TotalAmount") = CDec2(pDvHistory(0)("DiscountedUnitPrice")) * CDec2(pStock("Qty"))
                pIU("Reason") = ReadyString(pStock("ReturnReason"))
                pIU("TaxAmount") = CDec2(pDvHistory(0)("TaxAmount")) / CDec2(pDvHistory(0)("Quantity")) * CDec2(pStock("Qty"))
                pSql = pIU.GetInsert
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If
        Next pStock

        Common.WebRequest.ProData.CommitTransaction()

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    ''' <summary>
    ''' returns the latest sales return memo that contains item from the specified invoice only.
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <param name="vMemoID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMemoByInvoiceID(vInvoiceID As String, vMemoID As String) As DataRow

        Dim pSql As String

        If CStr2(vMemoID).Length > 0 Then
            pSql = " AND TransMemo.ID = " & ReadyString(vMemoID) & vbCrLf
        End If
        pSql = "SELECT DISTINCT TransMemo.ID" & vbCrLf & _
            " ,TransMemo.MemoNumber" & vbCrLf & _
            " ,TransMemoSalesReturn.CustomerID" & vbCrLf & _
            " ,TransMemoSalesReturn.InvoiceID" & vbCrLf & _
            " ,TransMemo.LastUpdated" & vbCrLf & _
            " ,t.OtherCount" & vbCrLf & _
            " FROM TransMemo" & vbCrLf & _
            " JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf & _
            " CROSS APPLY (" & vbCrLf & _
            " 	SELECT COUNT(*) AS OtherCount" & vbCrLf & _
            " 	FROM TransMemoSalesReturn TransMemoSalesReturn_1" & vbCrLf & _
            "   WHERE TransMemoSalesReturn.MemoID = TransMemoSalesReturn_1.MemoID" & vbCrLf & _
            "   AND TransMemoSalesReturn_1.InvoiceID <> TransMemoSalesReturn.InvoiceID" & vbCrLf & _
            " ) AS t" & vbCrLf & _
            " WHERE TransMemo.ApprovedBy IS NULL" & vbCrLf & _
            " AND TransMemo.IsCancelled = 0" & vbCrLf & _
            " AND TransMemoSalesReturn.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND t.OtherCount = 0" & vbCrLf & _
            pSql &
            " ORDER BY TransMemo.LastUpdated DESC" & vbCrLf

        Return Common.WebRequest.ProData.ExecuteRow(pSql)

    End Function

#End Region

#Region " settings "
    Function Settings() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Sales Return Settings")) Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetSettings() As String

        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("RestockingFeePercent", .SalesReturnRestockingFeePercent)
            pOut.Add("MaximumReturnablePeriod", .SalesReturntMaximumReturnablePeriod)
            pOut.Add("ReturnedPrice", .SalesReturntReturnedPrice)
            pOut.Add("SRVUseSettings", .SalesReturntSRVSettings)
            pOut.Add("PrintRefundSignUp", If(.SalesReturntPrintRefundSignUp, 1, 0))
        End With

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function SaveSettings() As String

        Dim pRestockingFeePercent As Decimal
        Dim pMaximumReturnablePeriod As Integer
        Dim pReturnedPrice As String
        Dim pPrintRefundSignUp As Byte

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        If Decimal.TryParse(Common.WebRequest.Data("RestockingFeePercent"), pRestockingFeePercent) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid RestockingFeePercent")
        End If

        If Integer.TryParse(Common.WebRequest.Data("MaximumReturnablePeriod"), pMaximumReturnablePeriod) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid MaximumReturnablePeriod")
        End If

        pReturnedPrice = CStr2(Common.WebRequest.Data("ReturnedPrice"))
        Select Case pReturnedPrice
            Case "Average Price", "Minimum Price"
            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid ReturnedPrice")

        End Select

        If Byte.TryParse(Common.WebRequest.Data("PrintRefundSignUp"), pPrintRefundSignUp) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid PrintRefundSignUp")
        End If

        With WebRequest.Instance.Business.SystemDefaults
            '.SalesReturnRestockingFeePercent = pRestockingFeePercent ' temporarily disabled
            .SalesReturntMaximumReturnablePeriod = pMaximumReturnablePeriod
            .SalesReturntReturnedPrice = pReturnedPrice
            .SalesReturntSRVSettings = Common.WebRequest.Data("SRVSettings")
            .SalesReturntPrintRefundSignUp = pPrintRefundSignUp
        End With

        Return JsonDictionary.Serialize("Result", "ok")

    End Function
#End Region
End Class
Public Class SalesReturnsController
    Inherits System.Web.Mvc.Controller

#Region " index "
    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Sales Return")) Then Exit Function
        End If
        WebPosFlush.StartAsync()
        Return PartialView("~/Views/SalesReturns/BsIndex.vbhtml")
        'Return View()

    End Function

    Function BsIndex() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Sales Return")) Then Exit Function
        End If

        Return View()

    End Function

    Public Function SRVHistory() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetSRVHistory() As String
        Dim pSql As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'ORIGINAL QUERY - commented by arnie on 8-26-21
        'pSql = "Select " & vbCrLf &
        '        "TransSalesReturnLedger.LastUpdated SRVDate,  " & vbCrLf &
        '        "SrvNum, " & vbCrLf &
        '        "Amount SRVAmount," & vbCrLf &
        '        "Pin, " & vbCrLf &
        '        "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName SRVIssuedBy, " & vbCrLf &
        '        "MobizLibraryOrganizationFOWST.Name SRVIssuingFOWST, " & vbCrLf &
        '        "TransInvoice.InvoiceDate DateSourceOfInvoice, " & vbCrLf &
        '        "TransInvoice.InvoiceNumber RefSourceOfInvoice, " & vbCrLf &
        '        "SUM(SubTotal) SourceInvoiceAmount " & vbCrLf &
        '        "from TransSalesReturnLedger " & vbCrLf &
        '        "inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransSalesReturnLedger.FowstID " & vbCrLf &
        '        "inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransSalesReturnLedger.UserID " & vbCrLf &
        '        "inner join TransInvoice on TransInvoice.ID = TransSalesReturnLedger.SourceInvoiceID " & vbCrLf &
        '        "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
        '        "where DateDiff(Day, TransSalesReturnLedger.LastUpdated, GETDATE()) < 2 AND SrvNum IS NOT NULL " & vbCrLf &
        '        "Group By " & vbCrLf &
        '        "TransSalesReturnLedger.LastUpdated, " & vbCrLf &
        '        "SrvNum, " & vbCrLf &
        '        "Amount, " & vbCrLf &
        '        "Pin, " & vbCrLf &
        '        "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName, " & vbCrLf &
        '        "MobizLibraryOrganizationFOWST.Name, " & vbCrLf &
        '        "TransInvoice.InvoiceDate, " & vbCrLf &
        '        "TransInvoice.InvoiceNumber "

        Dim fowstid As String = Common.WebRequest.Data("FowstID")
        Dim groupid As String = Common.WebRequest.Data("GroupID")
        Dim datefrom As String = Common.WebRequest.Data("DateFrom")
        Dim dateto As String = Common.WebRequest.Data("DateTo")
        Dim filterby As String = Common.WebRequest.Data("FilterBy")
        Dim search As String = Common.WebRequest.Data("Search")

        pSql = " SELECT * " & vbCrLf &
                "   , T.OrigQtyInInvoice - T.TotalReturnedQtyForThisInvoice as NetQtyInOrigInvoice" & vbCrLf &
                "   FROM (SELECT " & vbCrLf &
                "       sr.LastUpdated as SRDate, " & vbCrLf &
                "       fowst.Name as FowstName, " & vbCrLf &
                "       ppol.Surname + ', ' + ppol.FirstName TransactedBy, " & vbCrLf &
                "       (SELECT Surname + ', ' + FirstName FROM MobizLibraryPeople INNER JOIN TransMemo ON TransMemo.ApprovedBy = MobizLibraryPeople.ID WHERE TransMemo.ID = memo.MemoID) as ApprovedBy, " & vbCrLf &
                "       pim.ProductNo + ' ' + pim.ProductName as ItemDescription, " & vbCrLf &
                "       CASE WHEN sr.GrossSrvAmount = 0 THEN 'Refund' ELSE 'Merchandise Exchange' END SRType, " & vbCrLf &
                "       memo.QuantityReturned as ReturnedQtyForThisTrans, " & vbCrLf &
                "       memo.TotalAmount as TotalValueOfReturnedQty, " & vbCrLf &
                "       (SELECT SUM(QuantityReturned) FROM TransMemoSalesReturn WHERE ProductID = memo.ProductID AND InvoiceID = sr.SourceInvoiceID GROUP BY ProductID) as TotalReturnedQtyForThisInvoice, " & vbCrLf &
                "       (SELECT SUM(Quantity) FROM TransInvoiceItems WHERE InvoiceID = sr.SourceInvoiceID) as OrigQtyInInvoice, " & vbCrLf &
                "       RIGHT('000000000000' + CONVERT(varchar, inv.InvoiceNumber), 12) as InvoiceNumber, " & vbCrLf &
                "       inv.InvoiceDate, SrvNum, Pin, RIGHT('000000000000' + CONVERT(varchar, ISNULL(sr.DeviceFowstTransactionNumber, 0)), 12) as TransactionNumber, " & vbCrLf &
                "       MobizLibraryOrganization.CompanyName, ISNULL(MobizLibraryUserDefinedIndustry.Name,'') IndustryClass " & vbCrLf &
                "       FROM TransMemoSalesReturn as memo " & vbCrLf &
                "       INNER JOIN TransSalesReturnLedger as sr ON sr.SourceMemoID = memo.MemoID " & vbCrLf &
                "       INNER JOIN MobizLibraryPIMProduct as pim ON memo.ProductID = pim.ID " & vbCrLf &
                "       INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = sr.FowstID " & vbCrLf &
                "       INNER join MobizLibraryPeople as ppol on ppol.ID = sr.UserID " & vbCrLf &
                "       INNER join TransInvoice as inv on inv.ID = sr.SourceInvoiceID " & vbCrLf &
                "       INNER JOIN MobizLibraryOrganization on MobizLibraryOrganization.ID = inv.CustomerID " & vbCrLf &
                "       LEFT JOIN MobizLibraryUserDefinedIndustry on MobizLibraryUserDefinedIndustry.ID = MobizLibraryOrganization.UserDefinedIndustryID " & vbCrLf &
                "       WHERE CAST(sr.LastUpdated as Date) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf

        If fowstid <> "" Then
            pSql &= " AND sr.FowstID = " & ReadyString(fowstid) & vbCrLf
        Else
            pSql &= " AND sr.FowstID IN (SELECT FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID = " & ReadyString(GroupID) & ")" & vbCrLf
        End If

        pSql &= "       GROUP BY memo.MemoID, sr.LastUpdated, fowst.Name, ppol.Surname, ppol.FirstName, pim.ProductNo, pim.ProductName, " & vbCrLf &
                "           SrvNum, Pin, sr.DeviceFowstTransactionNumber, memo.QuantityReturned, memo.TotalAmount, sr.SourceInvoiceID, " & vbCrLf &
                "           inv.InvoiceNumber, inv.InvoiceDate, sr.GrossSrvAmount, memo.ProductID, MobizLibraryUserDefinedIndustry.Name, MobizLibraryOrganization.CompanyName " & vbCrLf &
                "   ) as T " & vbCrLf

        pSql &= " WHERE T.ItemDescription + ' ' + T.InvoiceNumber LIKE " & ReadyStringWild(search)

        If filterby <> "all" Then
            pSql &= " AND SRType = " & ReadyString(filterby)
        End If

        pSql &= " ORDER BY SRDate DESC "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function GetContents() As String

        Dim pInvoiceID As String
        Dim pCustomerID As String
        Dim pKeywords As String
        Dim pDS As DataSet
        Dim pOut As JsonDictionary
        Dim pMemoID As String
        Dim pDrMemo As DataRow

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        pMemoID = Common.WebRequest.Data("MemoID")
        pDrMemo = GetMemoByInvoiceID(pInvoiceID, pMemoID)

        If pDrMemo IsNot Nothing Then
            pMemoID = CStr2(pDrMemo("ID"))
        End If
        If IsGuid(pInvoiceID) = False _
            AndAlso IsGuid(pMemoID) Then

            pMemoID = ReadyGuid(pMemoID)
            pInvoiceID = WebRequest.Instance.Business.TransMemoSalesReturn.GetInvoiceID(pMemoID)
        End If

        If IsGuid(pInvoiceID) Then
            pInvoiceID = ReadyGuid(pInvoiceID)
            pDS = GetByInvoiceID(pInvoiceID, , , , pMemoID)
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
            pOut.Add("MemoID", pMemoID)
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
        Optional vMemoID As String = Nothing,
        Optional vAllowProductsNotInInvoice As Boolean = False) As DataSet

        Dim pSql As String
        Dim pDT As DataTable
        Dim pWhere As String ' removed pWhere2 2021_01_07__11_47_09_6476

        If vProductID IsNot Nothing Then
            pWhere &= " AND MobizLibraryPIMProduct.ID = " & ReadyString(vProductID) & vbCrLf
        End If

        pSql = "SELECT MobizLibraryPIMProduct.ID AS ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName + ISNULL('(Rebate Item: ' + CONVERT(varchar(500), CONVERT(decimal(8,0),tblInvoice.Quantity * TransInvoiceItemsRebates.Quantity * TransInvoiceItemsRebates.UnitQuantity)) + ' ' + TransInvoiceItemsRebates.UnitMeasure + ' ' + mb2.ProductName + ')','') AS ProductName" & vbCrLf &
            " ,SUM(tblInvoice.Quantity) AS Quantity" & vbCrLf &
            " ,tblInvoice.ActualQuantity" & vbCrLf &
            " ,tblInvoice.UnitMeasure" & vbCrLf &
            " ,tblInvoice.UnitPrice" & vbCrLf &
            " ,tblInvoice.UnitDiscountAmount" & vbCrLf &
            " ,SUM(tblInvoice.TaxAmount) AS TaxAmount" & vbCrLf &
            " ,SUM(tblInvoice.VATSales) AS VATSales" & vbCrLf &
            " ,SUM(tblInvoice.VATExempt) AS VATExempt" & vbCrLf &
            " ,SUM(tblInvoice.HasSCDiscount) AS HasSCDiscount" & vbCrLf &
            " ,tblInvoice.InvoiceID" & vbCrLf &
            " ,SUM(tblInvoice.Quantity) AS QtyInvoiced" & vbCrLf

        pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0) + ISNULL(SUM(QuantityBad), 0)" & vbCrLf & _
            " 	FROM TransMemoSalesReturn" & vbCrLf & _
            " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
            " 	LEFT JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.MemoID = TransMemoSalesReturn.MemoID" & vbCrLf & _
            " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
            " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
            " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
            " 	AND TransMemo.IsCancelled = 0" & vbCrLf &
            "   AND (TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
            "      OR TransInvoiceAdjustments.ID IS NULL" & vbCrLf &
            "   )" & vbCrLf
        If CStr2(vMemoID).Length > 0 Then
            pSql &= " 	AND TransMemoSalesReturn.MemoID <> " & ReadyString(vMemoID) & vbCrLf
        End If
        pSql &= " ) AS QtyReturnedOthers" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT ISNULL(SUM(QuantityReturned), 0)" & vbCrLf & _
                " 	FROM TransMemoSalesReturn" & vbCrLf & _
                " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
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
            pSql &= " ,(SELECT ISNULL(SUM(QuantityBad), 0)" & vbCrLf & _
                " 	FROM TransMemoSalesReturn" & vbCrLf & _
                " 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                " 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
                " 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
                " 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
                " 	AND TransMemo.IsCancelled = 0" & vbCrLf
            pSql &= " 	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
            pSql &= " )"
        Else
            pSql &= ",0"
        End If
        pSql &= " AS QtyReturnedOwnBad" & vbCrLf

        If CStr2(vMemoID).Length > 0 Then
            pSql &= " ,(SELECT TOP 1 TransMemoSalesReturn.Reason" & vbCrLf & _
                "  	FROM TransMemoSalesReturn" & vbCrLf & _
                "  	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                "  	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID" & vbCrLf & _
                "  	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID" & vbCrLf & _
                "  	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity" & vbCrLf & _
                "  	AND TransMemo.IsCancelled = 0" & vbCrLf & _
                "  	AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf & _
                "   ORDER BY TransMemoSalesReturn.LastUpdated DESC" & vbCrLf & _
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
            " ,UnitDiscountAmount" & vbCrLf &
            " ,TaxAmount" & vbCrLf &
            " ,InvoiceID" & vbCrLf &
            " ,SUM(Quantity) AS Quantity" & vbCrLf &
            " ,ModifierGroupID" & vbCrLf &
            " ,VATSales" & vbCrLf &
            " ,VATExempt" & vbCrLf &
            " ,HasSCDiscount" & vbCrLf &
            " FROM (" & vbCrLf &
            " SELECT TransMemoSalesReturn.ProductID" & vbCrLf &
            " ,TransMemoSalesReturn.ActualQuantity" & vbCrLf &
            " ,TransMemoSalesReturn.UnitMeasure" & vbCrLf &
            " ,TransMemoSalesReturn.UnitPrice" & vbCrLf &
            " ,TransMemoSalesReturn.UnitDiscountAmount" & vbCrLf &
            " ,0 AS TaxAmount" & vbCrLf &
            " ," & ReadyString(vInvoiceID) & " AS InvoiceID" & vbCrLf &
            " ,0 AS Quantity" & vbCrLf &
            " ,NULL AS ModifierGroupID" & vbCrLf &
            " ,TransMemoSalesReturn.VATSales" & vbCrLf &
            " ,TransMemoSalesReturn.VATExempt" & vbCrLf &
            " ,NULL AS HasSCDiscount" & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE 1=1" & vbCrLf
        If vMemoID IsNot Nothing Then
            pSql &= " AND TransMemoSalesReturn.MemoID = " & ReadyString(vMemoID) & vbCrLf
        End If
        If vInvoiceID IsNot Nothing Then
            pSql &= " AND TransMemoSalesReturn.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        End If
        pSql &= " UNION" & vbCrLf &
            " SELECT  TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice AS UnitPrice -- 2025-09-23 dont use DiscountedUnitPrice here" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice - TransInvoiceItems.DiscountedUnitPrice AS UnitDiscountAmount" & vbCrLf &
            " ,CASE WHEN TransInvoice.SCDiscount > 0 OR TransInvoice.SoloParentDiscount > 0 THEN UnitPrice - ROUND(TaxSales, 2) ELSE TransInvoiceItems.TaxAmount - TransInvoiceItems.DiscountTax END AS TaxAmount" & vbCrLf &
            " ," & ReadyString(vInvoiceID) & " AS InvoiceID" & vbCrLf &
            " ,TransInvoiceItems.Quantity" & vbCrLf &
            " ,TransInvoiceItems.ModifierGroupID" & vbCrLf &
            " ,TransInvoiceItems.VATableSale AS VATSales" & vbCrLf &
            " ,CASE WHEN TransInvoice.SCDiscount > 0 OR TransInvoice.SoloParentDiscount > 0 THEN ROUND(TaxSales, 2) ELSE TransInvoiceItems.VATExemptSale END AS VATExempt" & vbCrLf &
            " ,CASE WHEN ISNULL(TransInvoice.SCDiscount, 0) > 0 OR ISNULL(TransInvoice.SoloParentDiscount, 0) > 0 THEN 1 ELSE 0 END AS HasSCDiscount " & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " AND TransInvoice.IsCancelled = 0" & vbCrLf
        If vInvoiceID IsNot Nothing Then
            pSql &= " AND TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        End If
        pSql &= " AND TransInvoiceItems.Quantity > 0" & vbCrLf
        pSql &= " ) t" & vbCrLf &
            " WHERE t.Quantity > 0" & vbCrLf &
            " GROUP BY ProductID" & vbCrLf &
            " ,ActualQuantity" & vbCrLf &
            " ,UnitMeasure" & vbCrLf &
            " ,UnitPrice" & vbCrLf &
            " ,UnitDiscountAmount" & vbCrLf &
            " ,TaxAmount" & vbCrLf &
            " ,VATSales" & vbCrLf &
            " ,VATExempt" & vbCrLf &
            " ,InvoiceID" & vbCrLf &
            " ,ModifierGroupID" & vbCrLf &
            " ,HasSCDiscount" & vbCrLf &
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
            "  ,tblInvoice.UnitDiscountAmount" & vbCrLf &
            "  ,tblInvoice.InvoiceID" & vbCrLf &
            "  ,tblInvoice.ProductID" & vbCrLf &
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
            ' removed vAllowProductsNotInInvoice
            If String.IsNullOrWhiteSpace(vProductID) = False Then
                Dim pDtPrices = WebRequest.Instance.Business.LibraryStock.PriceTable(WebRequest.Instance.OrgID, vProductID, , , True)
                For Each pDrPrice In pDtPrices.Rows
                    Dim pDv = New DataView(pDT, "ActualQuantity = " & pDrPrice("Quantity"), "", DataViewRowState.CurrentRows)
                    If pDv.Count > 0 Then
                        Continue For
                    End If
                    Dim pDrNew = pDT.NewRow
                    'pDrNew.ItemArray = pDT.Rows(0).ItemArray
                    pDrNew("ProductID") = pDT.Rows(0)("ProductID")
                    pDrNew("ProductNo") = pDT.Rows(0)("ProductNo")
                    pDrNew("ProductName") = pDT.Rows(0)("ProductName")
                    pDrNew("QtyInvoiced") = 0
                    pDrNew("Quantity") = 0
                    pDrNew("ActualQuantity") = pDrPrice("Quantity")
                    pDrNew("UnitMeasure") = pDrPrice("UnitMeasure")
                    pDrNew("UnitPrice") = pDrPrice("DiscountedPrice")
                    'pDrNew("DiscountedUnitPrice") = pDrPrice("DiscountedPrice")
                    pDT.Rows.Add(pDrNew)
                Next pDrPrice
                Dim pDvNoPrice = New DataView(pDT, "ActualQuantity IS NULL", "", DataViewRowState.CurrentRows)
                If pDvNoPrice.Count > 0 Then
                    pDvNoPrice(0).Row.Delete()
                End If
            End If

            pDT.TableName = "Units"
            pOut = New DataSet
            pDT = New DataView(pDT, "", "ProductName", DataViewRowState.CurrentRows).ToTable
            'If vAllowProductsNotInInvoice = False Then
            '    pDT = New DataView(pDT, "Quantity > 0", "", DataViewRowState.CurrentRows).ToTable
            'End If
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
                    pDT.DefaultView.RowFilter = "ProductID = " & ReadyString(pDrStock("ProductID")) & " AND Quantity > 0"
                    If pDT.DefaultView.Count <= 1 Then Continue For
                    If pReturnedPrice = "Minimum Price" Then
                        pBaseUnitPriceMinimum = Decimal.MaxValue
                        For Each pDrvUnit As DataRowView In pDT.DefaultView
                            If CDec2(pDrvUnit("UnitPrice")) <= 0 Then
                                Continue For
                            End If
                            pBaseUnitPrice = pDrvUnit("UnitPrice") / Math.Max(pDrvUnit("ActualQuantity"), 1) ' just in case ActualQuantity is zero
                            If pBaseUnitPrice < pBaseUnitPriceMinimum Then
                                pBaseUnitPriceMinimum = pBaseUnitPrice
                            End If
                        Next pDrvUnit

                        pBaseUnitPrice = Math.Min(pBaseUnitPrice, pBaseUnitPriceMinimum)
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

            pDT.DefaultView.RowFilter = ""
            pDT.DefaultView.Sort = "ActualQuantity, UnitMeasure"
            pDT = pDT.DefaultView.ToTable()
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
        Dim pInvoiceID As String
        Dim pDrMemo As DataRow

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pMemoID = Common.WebRequest.Data("MemoID")
        If CStr2(pMemoID).Length > 0 Then
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If
            pDrMemo = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)

            ' jun 10, 2015 - remove checking
            'If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) Then
            '    pDrMemo = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)
            'Else
            '    pMemoID = Nothing
            'End If
        Else
            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid InvoiceID")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)
            pMemoID = WebRequest.Instance.Business.TransMemoSalesReturn.GetCartMemoByInvoiceID(pInvoiceID)
        End If

        pOut = New JsonDictionary
        If CStr2(pMemoID).Length > 0 Then
            pDtUnits = WebRequest.Instance.Business.TransMemoSalesReturn.GetStocks(pMemoID)
            If pDtUnits.Rows.Count > 0 Then
                pDtUnits = New DataView(pDtUnits, "QuantityReturned > 0 OR ISNULL(QuantityBad, 0) > 0", "ProductName", DataViewRowState.CurrentRows).ToTable
                pDtStocks = pDtUnits.DefaultView.ToTable(True, {"ProductID", "ProductNo", "ProductName", "InvoiceID"})
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
            If pDrMemo IsNot Nothing Then
                pOut.Add("Memo", pDrMemo)
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
        If String.IsNullOrWhiteSpace(pMemoID) Then
            Return JsonDictionary.Serialize("Error", "Nothing to finalize for this sales return. Please enter a return quantity for at least 1 item.")
        End If
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

    Friend mCheckOutLock As New Object()

    <HttpPost()>
    Public Function CheckOut() As String

        Dim pMemoID As String
        Dim pDtCart As DataTable
        Dim pInputTotalAmount, pMemoTotalAmount As Decimal
        Dim pInputSkuCount, pMemoSkuCount As Integer
        Dim pOut As JsonDictionary
        Dim pIsRefund As Boolean
        Dim pApprovedBy As String
        Dim pSalesReturnRequiresApproval As Boolean
        Dim pDrMemo As DataRow
        Dim pFromEas As Boolean = CBool2(Common.WebRequest.Data("FromEas"))
        Dim pError As String
        Dim UUID As String

        Try
            UUID = Common.WebRequest.Data("UUID")
            pMemoID = Common.WebRequest.Data("MemoID")
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If

            pInputTotalAmount = CDec2(Common.WebRequest.Data("TotalAmount"))
            'If pInputTotalAmount <= 0 Then
            '    Return JsonDictionary.Serialize("Error", "Invalid Total Amount")
            'End If

            pInputSkuCount = CDec2(Common.WebRequest.Data("SkuCount"))
            If pInputSkuCount <= 0 Then
                Return JsonDictionary.Serialize("Error", "Invalid SKU Count")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If IsGuid(WebRequest.Instance.FowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Checkout requires a FowstID. vbKJU7FG4P3A!")
            End If

            If IsGuid(WebRequest.Instance.DeviceID) = False Then
                Return JsonDictionary.Serialize("Error", "Checkout requires a DeviceID. vbMSN9QKZ37N!")
            End If

            SyncLock mCheckOutLock
                If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid sales return cart. This transaction was already submitted.")
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
                pIsRefund = CBool2(Common.WebRequest.Data("Refund"))

                WebRequest.Instance.ProData.BeginTransaction()
                pSalesReturnRequiresApproval = SecurityRole.Instance.HandleRoleAccess("Ask for Approval", , True)

                If pSalesReturnRequiresApproval = False Then
                    pApprovedBy = WebSession.Instance.PeopleUserID
                End If
                WebRequest.Instance.Business.TransMemoSalesReturn.Approve(pMemoID, pApprovedBy, WebSession.Instance.PeopleUserID, WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID, pIsRefund)
                If pSalesReturnRequiresApproval = False Then
                    WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger(pMemoID, WebRequest.Instance.FowstID)
                    WebRequest.Instance.Business.TransSalesReturnLedger.Save(pMemoID, CDec2(Common.WebRequest.Data("Refund")) <> 0, WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID, Nothing, UUID)
                    'for refunds only
                    If pIsRefund Then
                        WebRequest.Instance.Business.TransSalesReturnLedger.SaveSalesPaymentsRefund(pMemoID)

                        If pFromEas Then
                            Dim pSaveAdjustmentJson As TransInvoice.SaveAdjustmentJson
                            pSaveAdjustmentJson = New TransInvoice.SaveAdjustmentJson()
                            Dim pDrReason As DataRow
                            pDrReason = WebRequest.Instance.Business.LibraryAccountAdjustments.GetByReason("Sales Returns")
                            If pDrReason Is Nothing Then
                                WebRequest.Instance.ProData.RollbackTransaction()
                                Return JsonDictionary.Serialize("Error", "No account adjustment reason for Sales Returns. ref: vb93PPTAF9A5!")
                            End If
                            pSaveAdjustmentJson.ReasonID = CStr2(pDrReason("ID"))
                            pSaveAdjustmentJson.Amount = pInputTotalAmount
                            pSaveAdjustmentJson.InvoiceID = Common.WebRequest.Data("InvoiceID")
                            pSaveAdjustmentJson.Remarks = Common.WebRequest.Data("Remarks")
                            pSaveAdjustmentJson.IsNew = True
                            pSaveAdjustmentJson.MemoID = pMemoID

                            WebRequest.Instance.Business.TransInvoice.SaveAdjustment(pSaveAdjustmentJson, pError)
                            If String.IsNullOrWhiteSpace(pError) = False Then
                                WebRequest.Instance.ProData.RollbackTransaction()
                                Return JsonDictionary.Serialize("Error", pError)
                            End If
                            WebRequest.Instance.Business.TransCustomerPayment.AdjustUnappliedPayment(pSaveAdjustmentJson.InvoiceID, pInputTotalAmount)
                        End If
                    End If
                End If
                WebRequest.Instance.ProData.CommitTransaction()
            End SyncLock

            pOut = New JsonDictionary()
            pDrMemo = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)
            If pDrMemo IsNot Nothing Then
                pOut.Add("MemoID", pMemoID)
                pOut.Add("Num", pDrMemo("MemoNumber"))
                Dim pDrSalesLedger = WebRequest.Instance.Business.TransSalesReturnLedger.GetByMemoID(pMemoID)
                If pDrSalesLedger IsNot Nothing Then
                    pOut.Add("SrvNum", CStr2(pDrSalesLedger("SrvNum")))
                    pOut.Add("Pin", CStr2(pDrSalesLedger("Pin")))
                End If
            Else
                ' something is wrong here. pDrMemo is always not nothing
                Stop2()
            End If

            pOut.Add("Result", "ok")
            If pSalesReturnRequiresApproval Then
                pOut.Add("SalesReturnRequiresApproval", 1)
            End If

            Return pOut.Serialize()
        Catch ex As Exception
            Dim pErrorKey As String = GenerateString(10)
            AppendToDebug(pErrorKey & " - " & ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message & ". Error key in logs: " & pErrorKey)
        End Try

    End Function

    <HttpPost()>
    Public Function GetMemoRebatesItems() As String
        Dim pDt As DataTable
        Dim pOut As JsonDictionary
        Dim pMemoID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pMemoID = Common.WebRequest.Data("MemoID")

        pDt = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemoRebatesItems(pMemoID)

        pOut = New JsonDictionary()

        Return pOut.Serialize("Results", pDt)
    End Function

    <HttpPost>
    Public Function GetApprovalStatus() As String

        Dim pMemoID As String
        Dim pDR As DataRow

        pMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pMemoID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
        End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pDR = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Sales Return with specified ID does not exist. vb5GCZFVEUUC!")
        End If
        If pDR("DeniedBy") IsNot DBNull.Value Then ' "denied" takes precedence over "approved"
            Return JsonDictionary.Serialize("Result", "denied")
        ElseIf pDR("ApprovedBy") IsNot DBNull.Value Then
            Return JsonDictionary.Serialize("Result", "approved")
        Else
            Return JsonDictionary.Serialize("Result", "pending")
        End If

    End Function
#End Region

#Region " Stock action "
    Public Function Stock() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Public Function BsStock() As ActionResult

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
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

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
                ' Return JsonDictionary.Serialize("Error", "Invalid cart") ' commented 2022_10_28__14_50_06_1931
                pMemoID = Nothing
            End If
        End If

        pDrMemo = GetMemoByInvoiceID(pInvoiceID, pMemoID)
        If pDrMemo IsNot Nothing Then
            pMemoID = CStr2(pDrMemo("ID"))
        End If

        Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)

        Dim pAllowProductsNotInInvoice As Boolean = True
        If WebSession.Instance.PeopleUserID IsNot Nothing Then
            pAllowProductsNotInInvoice = New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Accept Sales Return Without Invoice", False)
        End If
        pDS = GetByInvoiceID(pInvoiceID, pProductID, , , pMemoID, pAllowProductsNotInInvoice)
        If pDS IsNot Nothing Then
            pDS.Tables("Units").Columns.Add("GrossUnitPrice", GetType(Decimal))
            For Each pDR As DataRow In pDS.Tables("Units").Rows
                pDR("GrossUnitPrice") = CDec2(pDR("UnitPrice"))
            Next
            Dim pDiscountFactor As Decimal
            pOut = New JsonDictionary
            If pDrInvoice IsNot Nothing Then
                pOut.Add("CustomerID", CStr2(pDrInvoice("CustomerID")))
                Dim pTotalAmount = CDec2(pDrInvoice("TotalAmount"))
                Dim pSCDiscount = CDec2(pDrInvoice("SCDiscount"))
                Dim pSPDiscount = CDec2(pDrInvoice("SoloParentDiscount"))
                Dim pNTDiscount = CDec2(pDrInvoice("PhNatTeamDiscount"))
                If pTotalAmount > 0 Then
                    If pSCDiscount > 0 _
                        OrElse pSPDiscount > 0 _
                        OrElse pNTDiscount > 0 Then

                        If pSCDiscount > 0 Then
                            pDiscountFactor = (pSCDiscount + CDec2(pDrInvoice("ForegoneVAT"))) / pTotalAmount
                        ElseIf pSPDiscount > 0 Then
                            pDiscountFactor = (pSPDiscount + CDec2(pDrInvoice("ForegoneVAT"))) / pTotalAmount
                        ElseIf pNTDiscount > 0 Then
                            pDiscountFactor = (pNTDiscount + CDec2(pDrInvoice("ForegoneVAT"))) / pTotalAmount
                        End If
                        If pDiscountFactor > 0 Then
                            For Each pDR As DataRow In pDS.Tables("Units").Rows
                                pDR("UnitPrice") = CDec2(pDR("UnitPrice")) * (1 - pDiscountFactor)
                            Next
                        End If
                    Else
                        Dim pAmountDueBeforeCharges = CDec2(pDrInvoice("AmountDueBeforeCharges"))
                        If pAmountDueBeforeCharges > 0 Then
                            Dim pPriceFactor = pAmountDueBeforeCharges / pTotalAmount
                            If pPriceFactor > 0 Then
                                For Each pDR As DataRow In pDS.Tables("Units").Rows
                                    pDR("UnitPrice") = CDec2(pDR("UnitPrice")) * pPriceFactor
                                Next
                            End If
                        End If
                    End If
                End If
            End If
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

    Public Function GetMemoStocks() As String
        Dim pMemoID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pMemoID = Common.WebRequest.Data("MemoID")

        If pMemoID.Length > 0 Then
            Return JsonDictionary.Serialize("Results", WebRequest.Instance.Business.TransMemoSalesReturn.GetStocks(pMemoID))
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
        Dim pAllowProductsNotInInvoice As Boolean

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

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        If IsGuid(WebRequest.Instance.FowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Checkout requires a FowstID. vbL8F224X4HF!")
        End If

        If IsGuid(WebRequest.Instance.DeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "Checkout requires a DeviceID. vb9KEPVBT9F6!")
        End If

        Dim pSql As String
        Dim pIU As InsertUpdate

        Dim pDrMemo As DataRow
        Dim pDrInvoice As DataRow
        Dim pMemoID As String
        Dim pCustomerID As String
        Dim pReturnQty, pReturnQtyBad, pRemainingQty As Decimal
        Dim pDsHistory As DataSet
        Dim pDtHistory As DataTable
        Dim pDvHistory As DataView

        pMemoID = Common.WebRequest.Data("MemoID")
        If CStr2(pMemoID).Length > 0 Then
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Memo ID")
            End If
            pDrMemo = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)
            If pDrMemo("ApprovedBy") IsNot DBNull.Value Then
                Return JsonDictionary.Serialize("Error", "This transaction was already approved. Modifications are not allowed anymore.")
            ElseIf pDrMemo("DeniedBy") IsNot DBNull.Value Then
                Return JsonDictionary.Serialize("Error", "This transaction was disapproved. Modifications are not allowed anymore.")
            End If
            If WebRequest.Instance.Business.TransMemoSalesReturn.IsValidCart(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid cart")
            End If
        End If

        Common.WebRequest.ProData.BeginTransaction()
        pAllowProductsNotInInvoice = New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Accept Sales Return Without Invoice", False)

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
            pCustomerID = WebRequest.Instance.Data("CustomerID")
            If String.IsNullOrEmpty(pCustomerID) Then
            pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
            pCustomerID = CStr2(pDrInvoice("CustomerID"))
            End If
        End If
        pDsHistory = GetByInvoiceID(pInvoiceID, pStockID, , , pMemoID, pAllowProductsNotInInvoice)
        If pDsHistory Is Nothing Then
            Return JsonDictionary.Serialize("Error", "The product you are trying to return is not in the invoice.")
        End If
        pDtHistory = pDsHistory.Tables("Units")
        pDtHistory.Columns.Add("SubQty", GetType(Decimal), "(QtyInvoiced - QtyReturnedOthers)")
        pDtHistory.Columns.Add("TotalSubQty", GetType(Decimal), "(SubQty * ActualQuantity)")

        Dim pReturnedByThis As Decimal
        If pAllowProductsNotInInvoice = False Then
            For Each pStock As Object In Common.WebRequest.AllData("Stocks")
                pReturnQtyBad = 0
                If CType(pStock, IDictionary).Contains("QtyBad") Then
                    pReturnQtyBad = CDec2(pStock("QtyBad"))
                End If
                pReturnedByThis += (CDec2(pStock("Qty")) + pReturnQtyBad) * CDec2(pStock("UnitQty"))
            Next
            pRemainingQty = CDec2(pDtHistory.Compute("SUM(TotalSubQty)", ""))
            If pRemainingQty < pReturnedByThis Then
                Return JsonDictionary.Serialize("Error", "Return Quantity (" & Format(pReturnedByThis, "#,###.####") & " in base unit) must not be greater than Remaining Quantity (" & Format(pRemainingQty, "#,###.####") & ")")
            End If
        End If

        'For Each pStock As Object In Common.WebRequest.AllData("Stocks")
        '    pReturnQtyBad = 0
        '    If CType(pStock, IDictionary).Contains("QtyBad") Then
        '        pReturnQtyBad = CDec2(pStock("QtyBad"))
        '    End If
        '    If CDec2(pStock("Qty")) > 0 _
        '        OrElse pReturnQtyBad > 0 Then

        '        If pAllowProductsNotInInvoice = False Then
        '            pDvHistory = New DataView(pDtHistory, "ActualQuantity = " & CDec2(pStock("UnitQty")), "", DataViewRowState.CurrentRows)
        '            If pDvHistory.Count = 1 Then
        '                pRemainingQty = CDec2(pDtHistory.Compute("SUM(SubQty)", "ActualQuantity = " & CDec2(pStock("UnitQty"))))
        '                pReturnQty = CDec2(pStock("Qty"))
        '                If pRemainingQty < pReturnQty + pReturnQtyBad Then
        '                    Common.WebRequest.ProData.RollbackTransaction()
        '                    Return JsonDictionary.Serialize("Error", "Return Quantity (" & pReturnQty & ") must not be greater than Remaining Quantity (" & pRemainingQty & " " & pDvHistory(0)("UnitMeasure") & ")")
        '                End If
        '            Else
        '                Common.WebRequest.ProData.RollbackTransaction()
        '                Return JsonDictionary.Serialize("Error", "ActualQuantity of " & CDec2(pStock("UnitQty")) & " has " & pDvHistory.Count & " records")
        '            End If
        '        End If
        '    End If
        'Next pStock

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
            pReturnQtyBad = 0
            If CType(pStock, IDictionary).Contains("QtyBad") Then
                pReturnQtyBad = CDec2(pStock("QtyBad"))
            End If
            If CDec2(pStock("Qty")) > 0 _
                OrElse pReturnQtyBad > 0 Then

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
                If CType(pStock, IDictionary).Contains("QtyBad") Then
                    pIU("QuantityBad") = CDec2(pStock("QtyBad"))
                End If
                pIU("UnitMeasure") = ReadyString(pDvHistory(0)("UnitMeasure")) ' ReadyString(pStock("UnitMeasure"))
                pIU("ActualQuantity") = CDec2(pDvHistory(0)("ActualQuantity")) 'CDec2(pStock("UnitQty"))
                pIU("UnitPrice") = CDec2(pDvHistory(0)("UnitPrice")) ' CDec2(pStock("UnitPrice"))
                pIU("TotalAmount") = CDec2(pDvHistory(0)("UnitPrice")) * CDec2(pStock("Qty"))
                pIU("Reason") = ReadyString(pStock("ReturnReason"))
                If CDec2(pDvHistory(0)("TaxAmount")) <> 0 _
                    AndAlso CDec2(pDvHistory(0)("Quantity")) * CDec2(pStock("Qty")) <> 0 Then

                    pIU("TaxAmount") = CDec2(pDvHistory(0)("TaxAmount")) / CDec2(pDvHistory(0)("Quantity")) * CDec2(pStock("Qty"))
                End If
                pSql = pIU.GetInsert
                Common.WebRequest.ProData.ExecuteSql(pSql)
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
            pOut.Add("AllowOfflineSrvPayments", If(.AllowOfflineSrvPayments, 1, 0))
            pOut.Add("HideMerchandiseExchange", If(.HideMerchandiseExchange, 1, 0))
            pOut.Add("RestrictedTransactionLookupForSalesReturn", If(.RestrictedTransactionLookupForSalesReturn, 1, 0))
            pOut.Add("AllowedReturnDuration", .AllowedReturnDuration)
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
            .HideMerchandiseExchange = CBool2(WebRequest.Instance.Data("HideMerchandiseExchange"))
            .RestrictedTransactionLookupForSalesReturn = CBool2(WebRequest.Instance.Data("RestrictedTransactionLookupForSalesReturn"))
            .AllowedReturnDuration = CDec2(WebRequest.Instance.Data("AllowedReturnDuration"))
        End With
        Dim pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function
#End Region

#Region " approval "
    <HttpPost>
    Public Function Approve() As String

        Dim pMemoID As String
        Dim pApprove As Boolean
        Dim pViewAll As Boolean
        Dim pViewGroup As Boolean
        Dim pViewBranch As Boolean
        Dim pFowstID As String
        Dim pDrMemo As DataRow
        Dim pUserID As String
        Dim pUsername As String
        Dim pPassword As String
        Dim pDrUser As DataRow
        Dim pIsRefund As Boolean
        Dim UUID As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            UUID = Common.WebRequest.Data("UUID")
            pMemoID = Common.WebRequest.Data("SalesReturnMemoID")
            If IsGuid(pMemoID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid SalesReturnMemoID")
            End If
            pMemoID = ReadyGuid(pMemoID)

            pUsername = Common.WebRequest.Data("Username")
            pPassword = Common.WebRequest.Data("Password")
            If CStr2(pUsername).Trim() <> "" _
                AndAlso CStr2(pPassword).Trim() <> "" Then

                pDrUser = WebRequest.Instance.Business.LibraryPeople.GetByLogin(pUsername, pPassword)
                If pDrUser Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid username and/or password")
                End If
                pUserID = CStr2(pDrUser("PeopleID"))
            Else
                pUserID = WebSession.Instance.PeopleUserID
            End If

            pApprove = CBool2(Common.WebRequest.Data("Approve"))
            pDrMemo = WebRequest.Instance.Business.TransMemoSalesReturn.GetMemo(pMemoID)
            pFowstID = CStr2(pDrMemo("FowstID"))
            If SecurityRoleController.GetRequestApproveModuleName(pFowstID, pUserID) <> SecurityRoleController.RequestApproveModule Then
                WebRequest.Instance.ProData.BeginTransaction()
                WebRequest.Instance.Business.TransMemoSalesReturn.Approve(pMemoID, pUserID, pApprove)
                If pApprove Then
                    pIsRefund = CStr2(pDrMemo("CheckOutMethod")) = "Refund"
                    WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger(pMemoID, pFowstID)
                    WebRequest.Instance.Business.TransSalesReturnLedger.Save(pMemoID, pIsRefund, pFowstID, CStr2(pDrMemo("DeviceID")), pFowstID, UUID)
                    'for refunds only
                    If pIsRefund Then
                        WebRequest.Instance.Business.TransSalesReturnLedger.SaveSalesPaymentsRefund(pMemoID)
                    End If
                End If
                WebRequest.Instance.ProData.CommitTransaction()
                If pApprove Then
                    Return JsonDictionary.Serialize("Result", "approved")
                Else
                    Return JsonDictionary.Serialize("Result", "denied")
                End If
            Else
                Return JsonDictionary.Serialize("NoAccess", 1)
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
#End Region

    Function CreateEasPdf() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pMemoID = Common.WebRequest.Data("MemoID")
        Try
            Dim pBaseFilename = WebRequest.Instance.Business.TransMemoSalesReturn.CreatePdf(pMemoID, WebRequest.Instance.OrgID)
            Return JsonDictionary.Serialize("BaseFilename", pBaseFilename)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function CreatePdf()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim memoID = Common.WebRequest.Data("MemoID")
            Dim template = Common.WebRequest.Data("Template")
            Dim pReport As New ProReport
            Dim pUuid = Common.WebRequest.Data("UUID")
            Dim pDeviceID As String = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
            Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
            If pDrInvoice IsNot Nothing Then
                pDeviceID = CStr2(pDrInvoice("DeviceID"))
            End If

            Dim clsSalesPayments As New SalesPayments
            Dim dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(pInvoiceID, WebRequest.Instance.ProData)

            pReport.AddTable("InvoiceHeader", dtaTable)

            Dim FowstID As String = CStr2(dtaTable.Rows(0).Item("OrganizationBranchID"))
            Dim POSAccredNo = WebRequest.Instance.Business.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")

            dtaTable = clsSalesPayments.getReceiptHeaderInfo(FowstID.ToString().Trim(), WebRequest.Instance.ProData, POSAccredNo, pDeviceID)

            pReport.AddTable("ReceiptHeader", dtaTable)

            If template = "IssueARefund" Then
                dtaTable = clsSalesPayments.getSalesPaymentSRVRefundHeaderInfo(memoID)
            Else
                dtaTable = clsSalesPayments.getSalesPaymentSRVReceiptHeaderInfo(memoID)
            End If

            pReport.AddTable("SRVData", dtaTable)

            pReport.SourceFilename = template
            ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"
            pReport.ExportXmlFile()
            'pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & ";" & pDocumentNameID & ";" & WebRequest.Instance.OrgID & ";" & Format(1, "00") & "-" & DateTimeStamp() & ".pdf"
            'CreateFolder(pFilename)

            Dim pBaseFilename = CreateValidFilename(pUuid) & "-" & DateTimeStamp()
            Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename ' & ".pdf" ' dont add ".pdf" 2021_07_19__09_12_10_0167

            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename & ".pdf")
            Dim pOut = New JsonDictionary()
            pOut.Add("BaseFilename", pBaseFilename)
            pOut.Add("Result", "ok")
            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function GetDeviceID()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(Common.WebRequest.Data("UUID"))

        Return JsonDictionary.Serialize("Results", pDeviceID)
    End Function

    Function GetSalesReturnDetails()

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim dtMemoData As DataTable
        Dim pDT As DataTable
        Dim vMemoID As String

        vMemoID = CStr2(Common.WebRequest.Data("MemoID"))

        pDT = WebRequest.Instance.Business.TransSalesReturnLedger.GetMemoRead(vMemoID, Today)

        pDT.DefaultView.RowFilter = "CurSalesReturnRefund>0 OR CurSalesReturnExchange>0"

        dtMemoData = pDT.DefaultView(0).DataView.ToTable


        Return JsonDictionary.Serialize("Results", dtMemoData)
    End Function

    Public Function ExportInvoiceToXmlZip() As String

        ' If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceID = WebRequest.Instance.Data("InvoiceID")
        WebRequest.Instance.Business.TransInvoice.ExportToXmlZip(pInvoiceID)

        Return JsonDictionary.Serialize("Results", "ok")

    End Function

    Public Function DownloadInvoiceXmlZip() As String

        Dim pFolder As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pServer = WebRequest.Instance.Data("Server")
        Dim pInvoiceID = WebRequest.Instance.Data("InvoiceID")
        pInvoiceID = ReadyGuid(pInvoiceID)
        Dim pUrl = AppendIfNeeded(pServer, "/") & "SalesReturns/GetInvoiceXmlZip?InvoiceID=" & pInvoiceID

        pFolder = Common.ServerPath & "temp\DownloadedInvoiceXmlZip"
        Dim pFilename = IO.Path.Combine(pFolder, pInvoiceID & ".zip")
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
        ImportInvoiceXmlZip(pFilename)

        Return JsonDictionary.Serialize("Results", "ok")

    End Function

    Public Function GetInvoiceXmlZip() As ActionResult

        Dim pFilename = Common.ServerPath & "temp\ExportedInvoices\ExportedInvoices_" & WebRequest.Instance.QueryString("InvoiceID") & ".zip"

        Return File(pFilename, "application/zip", "ExportedInvoices_" & WebRequest.Instance.QueryString("InvoiceID") & ".zip")

    End Function

    Public Function GetInvoiceXmlZipAsJson() As String

        Dim pFilename = Common.ServerPath & "temp\ExportedInvoices\ExportedInvoices_" & WebRequest.Instance.Data("InvoiceID") & ".zip"
        If IO.File.Exists(pFilename) = False Then
            Return JsonDictionary.Serialize("Error", "File does not exist. vb5G2PKZYS6N!")
        End If

        Dim pZip = New ProZip(pFilename)
        Dim pDS = pZip.GetDataSet(Nothing)

        Dim pOut = New JsonDictionary()
        For Each pDT As DataTable In pDS.Tables
            If pDT.Rows.Count > 0 Then
                pOut.Add(pDT.TableName, pDT)
            End If
        Next

        pOut.Add("Results", "ok")
        Return pOut.Serialize()

    End Function

    Public Function ImportInvoiceXmlZip(vFilename As String) As Integer

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

    Function GetInvoiceDiscount()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim deviceid As String = Common.WebRequest.Data("DeviceID")
            Dim fowstid As String = Common.WebRequest.Data("FowstID")
            Dim salesdatefrom As String = Common.WebRequest.Data("SalesDateFrom")
            Dim salesdateto As String = Common.WebRequest.Data("SalesDateTo")
            Dim pSql As String

            pSql = "SELECT ISNULL(SUM(inv.SCDiscount), 0) as Total " +
                    " FROM TransSalesReturnLedger as ledger " +
                    " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " +
                    " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " +
                    " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " +
                    " INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID) " +
                    " WHERE ledger.FowstID = " + ReadyString(fowstid) +
                    " AND ledger.DeviceID = " + ReadyString(deviceid) +
                    " AND CONVERT(date, ledger.LastUpdated) BETWEEN " + ReadyString(salesdatefrom) + " AND " + ReadyString(salesdateto) +
                    " AND ISNULL(RefundNum, 0) > 0 "

            Return JsonDictionary.Serialize("Amount", Common.WebRequest.ProData.ExecuteScalar(pSql))
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function GetMemoPerFowstDevice()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String

        pSql = "SELECT TOP 50 TransMemoSalesReturn.MemoID, InvoiceNumber, CheckoutMethod, TransMemoSalesReturn.TotalAmount, SrvNum as SRVNum, RefundNum, Pin, TransMemoSalesReturn.LastUpdated " +
                " FROM TransMemoSalesReturn " +
                " INNER JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID " +
                " INNER JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID " +
                " INNER JOIN TransSalesReturnLedger ON TransSalesReturnLedger.SourceMemoID = TransMemo.ID " +
                " WHERE TransSalesReturnLedger.FowstID = " + ReadyString(Common.WebRequest.Data("FowstID")) +
                " AND TransSalesReturnLedger.DeviceID = " + ReadyString(Common.WebRequest.Data("DeviceID")) +
                " ORDER BY TransMemoSalesReturn.LastUpdated DESC"

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function
End Class
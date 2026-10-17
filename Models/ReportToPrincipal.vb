Public Class ReportToPrincipal

    Function GetWeeklySalesReport() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID2"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID2"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "tSource.TransName"
                End With
                With .Add("Transaction_Date", SqlDbType.Date)
                    .Expression = "replace(convert(varchar, tSource.TransactionDate, 102),'.','')"
                End With
                With .Add("Product_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC')"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Sold_Qty", SqlDbType.Int)
                    .Expression = "convert(int,dbo.TransInventoryLedger.Qty * -1)"
                End With
                With .Add("Sold_Amt", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.VatExcluded, 1),',','')"
                End With
                With .Add("Sold_Amt_Discount", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.Discount, 1),',','')"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00))"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00))"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00))"
                End With
                With .Add("Net_Cost_of_Goods_Sold_Amt", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0) * convert(decimal(18,2),dbo.TransInventoryLedger.Qty) * -1)"
                    '.Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0) * dbo.TransInventoryLedger.Qty * -1)"
                End With
                With .Add("Transaction_Type", SqlDbType.VarChar)
                    .Expression = "CASE dbo.TransInventoryLedger.TransactionType WHEN 'Invoice' THEN 'SLC' else 'RTC' END"
                    '.Expression = "dbo.TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct LEFT JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID ON  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                        "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.TaxSales as VatExcluded, TransInvoice.InvoiceDate as TransactionDate, TransInvoiceItems.DiscountSales as Discount " & vbCrLf &
                        "FROM TransInvoice INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                        "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount - TransMemoSalesReturn.TaxAmount ) * -1 as VatExcluded, TransMemo.MemoDate as TransactionDate, '' as Discount " & vbCrLf &
                        "FROM TransMemo " & vbCrLf &
                        "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                        "JOIN TransSalesReturnLedger  ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID " & vbCrLf &
                        "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND " &
                        "dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND " &
                        "dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND " &
                        "tSource.TransName> 0 AND " &
                        "CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To"))

            .SqlOrder = "ORDER BY tSource.TransName"

            pDT = .Requery

            Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

            If pDT.Rows.Count > 0 Then
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\Sales_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "Sales_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Else
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\Sales_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "Sales_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            End If

        End With

    End Function

    Function GetWeeklyInventoryReport() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "t.Account_ID"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "t.Store_ID"
                End With
                With .Add("Inv_Balance_Date", SqlDbType.VarChar)
                    .Expression = "t.Inv_Balance_Date"
                End With
                With .Add("Product_Code", SqlDbType.VarChar)
                    .Expression = "t.Product_Code"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "t.Size"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "t.UPC_Code"
                End With
                With .Add("Inv_Balance_Qty", SqlDbType.Int)
                    .Expression = "convert(int,SUM(t.balance))"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "t.AcquiredCost"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "t.AdjustedCost"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "t.StatedCost"
                End With
                With .Add("Retailer_Net_Cost_of_Inventory_Amt", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), t.AcquiredCost * SUM(t.balance))"
                End With
            End With

            ''''Replace By Francis A
            ''''To always follow whatever product listed in the CompanyWide Product List (LibraryOrganizationPIM)
            ''''08/26/2013
            '.SqlFrom = "FROM         dbo.LibraryOrganizationPIM INNER JOIN " & vbCrLf &
            '          "dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
            '          "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
            '          "dbo.TransInventoryLedger AS TransInventoryLedger_1 ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger_1.FowstID ON " & vbCrLf &
            '          "dbo.LibraryOrganizationPIM.ProductID = TransInventoryLedger_1.ProductID INNER JOIN " & vbCrLf &
            '          "dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
            '          "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID ON " & vbCrLf &
            '            "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID"

            '.SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " &
            '          " dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " &
            '          " dbo.TransInventoryLedger AS TransInventoryLedger_1 ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger_1.FowstID RIGHT OUTER JOIN " &
            '          " dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " &
            '          " dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " &
            '          " dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProduct.ID = dbo.LibraryOrganizationPIM.ProductID ON " &
            '          " TransInventoryLedger_1.ProductID = dbo.LibraryOrganizationPIM.ProductID"

            ''''Replace By Francis A
            ''''08/28/2013
            .SqlFrom = "FROM (SELECT DISTINCT " &
                        "dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName, " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID2 AS Account_ID,  " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST.StoreID2 AS Store_ID, replace(convert(varchar, convert(date," & ReadyString(WebRequest.Instance.Data("To")) & "), 102),'.','') AS Inv_Balance_Date,  " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001.C0017 AS Size, " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00)) AS AcquiredCost, " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00)) AS AdjustedCost,  " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00)) AS StatedCost, " & vbCrLf &
                        "bal1.Balance " & vbCrLf &
                        "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.LibraryOrganizationPIM.ProductID LEFT OUTER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger AS t0 INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID " & vbCrLf &
                        "outer apply(  " & vbCrLf &
                     "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " & vbCrLf &
                     "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 	 " & vbCrLf &
                     "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 " & vbCrLf &
                     "AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 " & vbCrLf &
                     ") t   "
            .SqlGroup = "group by Account_ID, Store_ID, Inv_Balance_Date, UPC_Code, Size, Product_Code, AcquiredCost, AdjustedCost, StatedCost"
            pDT = .Requery

            pDT.DefaultView.RowFilter = "Inv_Balance_Qty <> 0"

            Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

            If pDT.Rows.Count > 0 Then
                TableToExcelFile(pDT.DefaultView.ToTable, "", "", Common.ServerPath & "temp\InvtBal_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "InvtBal_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Else
                TableToExcelFile(pDT.DefaultView.ToTable, "", "", Common.ServerPath & "temp\InvtBal_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "InvtBal_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            End If

        End With
    End Function

    Function GetMonthInventoryReport() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID2"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID2"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "ISNULL(tSource.TransName, TransInventoryLedger.TransactionType)"
                End With
                With .Add("Inv_Mvmt_Date", SqlDbType.Date)
                    .Expression = "replace(convert(date, tSource.TransactionDate),'-','')"
                End With
                With .Add("Product_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    '.Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0017"
                    .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC')"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Inv_Mvmt_Qty", SqlDbType.Float)
                    .Expression = "convert(int,TransInventoryLedger.Qty)"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00))"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00))"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00))"
                End With
                With .Add("Transaction_Type", SqlDbType.Int)
                    .Expression = "CASE TransInventoryLedger.TransactionType " &
                                    "WHEN 'Invoice' THEN 'SLC' " &
                                    "WHEN 'Sales Returns' THEN 'RTC' " &
                                    "WHEN 'Receiving' THEN 'TIN' " &
                                    "WHEN 'TransferOut' THEN 'TOT' " &
                                    "WHEN 'InventoryCount' THEN 'ADJ' " &
                                    "END"
                    '.Expression = "TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProduct.ID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInventoryLedger AS TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID ON  " & vbCrLf &
                      "dbo.LibraryOrganizationPIM.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                      "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoice.InvoiceDate As TransactionDate " & vbCrLf &
                         "FROM TransInvoice " & vbCrLf &
                         "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                         "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Receiving' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'TransferOut' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, TransMemo.MemoDate As TransactionDate " & vbCrLf &
                         "FROM TransMemo " & vbCrLf &
                         "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                         "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT replace(CONVERT(varchar, CONVERT(date,TransInventoryProductCount.InventoryCountDate)),'-','') AS TransName,  TransInventoryProductCount.InventoryCountDate As TransactionDate" & vbCrLf &
                         "FROM TransInventoryProductCount " & vbCrLf &
                         "WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'InventoryCount' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To"))

            .SqlOrder = "ORDER BY Inv_Mvmt_Date"
            pDT = .Requery

            Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

            If pDT.Rows.Count > 0 Then
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\InvtMvmt_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "InvtMvmt_" & pDT.Rows(0).Item("Account_ID") & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Else
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\InvtMvmt_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")
                Return "InvtMvmt_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            End If
        End With

    End Function
End Class

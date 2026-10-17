Imports Aspose.Cells

Public Class RetailDashboardController
    Inherits System.Web.Mvc.Controller

    Private Const mDecimals As Integer = 4

    Function Index() As ActionResult

        'todo: change access parameter
        'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSalesReport_Dashboard, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pThisYear As DateRange
        Dim pLastYear As DateRange

        Try
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse (WebSession.Instance.UserAccess.PointOfSalesReport_Dashboard.IsView OrElse WebSession.Instance.UserAccess.PointOfSalesReport_Dashboard.IsEnable) = False Then

            '    Return JsonDictionary.Serialize("Error", "Access denied")
            'End If

            Dim pJson As JsonDictionary
            Dim pBranchID As Integer

            If Integer.TryParse(Common.WebRequest.Data("BranchID"), pBranchID) Then
                pBranchID = Common.WebRequest.Data("BranchID")
            Else
                pBranchID = WebRequest.Instance.Business.LibraryBranches.CurrentBranchDesignation()
            End If
            pJson = New JsonDictionary

            pJson.Add("wtd", GetWtd(pBranchID, , pThisYear, pLastYear))
            pJson.Add("wtdDateRangeThisYear", DisplayableDateRange(pThisYear))
            pJson.Add("wtdDateRangeLastYear", DisplayableDateRange(pLastYear
                                                                   ))
            pJson.Add("mtd", GetMtd(pBranchID, , pThisYear, pLastYear))
            pJson.Add("mtdDateRangeThisYear", DisplayableDateRange(pThisYear))
            pJson.Add("mtdDateRangeLastYear", DisplayableDateRange(pLastYear))

            pJson.Add("qtd", GetQtd(pBranchID, , pThisYear, pLastYear))
            pJson.Add("qtdDateRangeThisYear", DisplayableDateRange(pThisYear))
            pJson.Add("qtdDateRangeLastYear", DisplayableDateRange(pLastYear))

            pJson.Add("ytd", GetYtd(pBranchID, , pThisYear, pLastYear))
            pJson.Add("ytdDateRangeThisYear", DisplayableDateRange(pThisYear))
            pJson.Add("ytdDateRangeLastYear", DisplayableDateRange(pLastYear))

            pJson.Add("Branches", GetBranches)
            pJson.Add("BranchID", pBranchID)
            Return pJson.Serialize
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Private Function GetWtd(vBranchID As Integer, Optional vGrouped As Boolean = False,
        Optional ByRef oThisYear As DateRange = Nothing,
        Optional ByRef oLastYear As DateRange = Nothing) As DataTable

        Dim pDT As DataTable

        Client.GetWtdDates(oThisYear, oLastYear)
        pDT = GetSalesDataAll(vBranchID, oThisYear, oLastYear, vGrouped)

        Return pDT

    End Function

    Private Function GetMtd(vBranchID As Integer, Optional vGrouped As Boolean = False,
        Optional ByRef oThisYear As DateRange = Nothing,
        Optional ByRef oLastYear As DateRange = Nothing) As DataTable

        Dim pDT As DataTable

        Client.GetMtdDates(oThisYear, oLastYear)
        pDT = GetSalesDataAll(vBranchID,
            oThisYear,
            oLastYear,
            vGrouped)

        Return pDT

    End Function

    Private Function GetQtd(vBranchID As Integer, Optional vGrouped As Boolean = False,
        Optional ByRef oThisYear As DateRange = Nothing,
        Optional ByRef oLastYear As DateRange = Nothing) As DataTable

        Dim pDT As DataTable

        Client.GetQtdDates(oThisYear, oLastYear)
        pDT = GetSalesDataAll(vBranchID,
            oThisYear,
            oLastYear,
            vGrouped)

        Return pDT

    End Function

    Private Function GetYtd(vBranchID As Integer, Optional vGrouped As Boolean = False,
        Optional ByRef oThisYear As DateRange = Nothing,
        Optional ByRef oLastYear As DateRange = Nothing) As DataTable

        Dim pDT As DataTable

        Client.GetYtdDates(oThisYear, oLastYear)
        pDT = GetSalesDataAll(vBranchID,
            oThisYear,
            oLastYear,
            vGrouped)

        Return pDT

    End Function

    Private Function GetSalesData(vBranchID As Integer,
        ByVal vDateFrom As Date, vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SUM(DrpCacheStockPosDaily.Amount) AS Amount" & vbCrLf & _
            " ,SUM(DrpCacheStockPosDaily.COGS) AS COGS" & vbCrLf & _
            " FROM DrpCacheStockPosDaily" & vbCrLf & _
            " WHERE CONVERT(date, DrpCacheStockPosDaily.TransDate) " & vbCrLf & _
            " BETWEEN " & ReadyString(vDateFrom.Date) & vbCrLf & _
            " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " AND DrpCacheStockPosDaily.BranchID = " & vBranchID & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetSalesDataAll(vBranchID As Integer,
        vThisYear As DateRange, vLastYear As DateRange,
        Optional vGrouped As Boolean = False) As DataTable

        Dim pDtThisYear As DataTable
        Dim pDtLastYear As DataTable
        Dim pDT As DataTable
        Dim pDR As DataRow

        pDtThisYear = GetSalesData(vBranchID, vThisYear.DateFrom, vThisYear.DateTo)
        pDtLastYear = GetSalesData(vBranchID, vLastYear.DateFrom, vLastYear.DateTo)

        pDT = New DataTable
        With pDT.Columns
            .Add("Text")
            .Add("ThisYear", GetType(Decimal))
            .Add("LastYear", GetType(Decimal))
            .Add("AbsChange", GetType(Single))
            .Add("PercentChange", GetType(Single))
        End With

        pDR = pDT.NewRow
        pDR("Text") = "Sales"
        pDR("ThisYear") = pDtThisYear.Rows(0)("Amount")
        pDR("LastYear") = pDtLastYear.Rows(0)("Amount")
        pDR("AbsChange") = CDec2(pDR("ThisYear")) - CDec2(pDR("LastYear"))
        If CDec2(pDR("LastYear")) = 0 Then
            pDR("PercentChange") = 100
        Else
            pDR("PercentChange") = (CDec2(pDR("ThisYear")) / CDec2(pDR("LastYear")) - 1) * 100
        End If
        pDT.Rows.Add(pDR)

        pDR = pDT.NewRow
        pDR("Text") = "COGS"
        pDR("ThisYear") = pDtThisYear.Rows(0)("COGS")
        pDR("LastYear") = pDtLastYear.Rows(0)("COGS")
        pDR("AbsChange") = CDec2(pDR("ThisYear")) - CDec2(pDR("LastYear"))
        If CDec2(pDR("LastYear")) = 0 Then
            pDR("PercentChange") = 100
        Else
            pDR("PercentChange") = (CDec2(pDR("ThisYear")) / CDec2(pDR("LastYear")) - 1) * 100
        End If
        pDT.Rows.Add(pDR)

        pDR = pDT.NewRow
        pDR("Text") = "Gross Profit"
        pDR("ThisYear") = CDec2(pDtThisYear.Rows(0)("Amount")) - CDec2(pDtThisYear.Rows(0)("COGS"))
        pDR("LastYear") = CDec2(pDtLastYear.Rows(0)("Amount")) - CDec2(pDtLastYear.Rows(0)("COGS"))
        pDR("AbsChange") = CDec2(pDR("ThisYear")) - CDec2(pDR("LastYear"))
        If CDec2(pDR("LastYear")) = 0 Then
            pDR("PercentChange") = 100
        Else
            pDR("PercentChange") = (CDec2(pDR("ThisYear")) / CDec2(pDR("LastYear")) - 1) * 100
        End If
        pDT.Rows.Add(pDR)

        pDR = pDT.NewRow
        pDR("Text") = "Gross Profit Margin"
        If CDec2(pDtThisYear.Rows(0)("Amount")) <> 0 Then
            pDR("ThisYear") = (pDtThisYear.Rows(0)("Amount") - pDtThisYear.Rows(0)("COGS")) / pDtThisYear.Rows(0)("Amount") * 100
        End If
        If CDec2(pDtLastYear.Rows(0)("Amount")) <> 0 Then
            pDR("LastYear") = (pDtLastYear.Rows(0)("Amount") - pDtLastYear.Rows(0)("COGS")) / pDtLastYear.Rows(0)("Amount") * 100
        End If
        pDR("AbsChange") = CDec2(pDR("ThisYear")) - CDec2(pDR("LastYear"))
        If CDec2(pDR("LastYear")) = 0 Then
            pDR("PercentChange") = 100
        Else
            pDR("PercentChange") = (CDec2(pDR("ThisYear")) / CDec2(pDR("LastYear")) - 1) * 100
        End If
        pDT.Rows.Add(pDR)

        For Each pDR In pDT.Rows
            For Each pField As String In New String() {"ThisYear", "LastYear", "AbsChange", "PercentChange"}
                If pDR(pField) IsNot DBNull.Value Then
                    pDR(pField) = Math.Round(pDR(pField), mDecimals)
                End If
            Next pField
        Next pDR

        Return pDT

    End Function

    Private Function GetSalesDataGrouped(vBranchID As Integer,
        ByVal vDateFrom As Date, vDateTo As Date,
        ByVal vFieldNamePrefix As String,
        Optional vIncludeStocks As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SUM(DrpCacheStockPosDaily.Amount) AS Amount" & vFieldNamePrefix & vbCrLf & _
            " ,SUM(DrpCacheStockPosDaily.COGS) AS COGS" & vFieldNamePrefix & vbCrLf & _
            " ,SUM(DrpCacheStockPosDaily.Hits) AS Hits" & vFieldNamePrefix & vbCrLf & _
            ",LibraryDepartments.DepartmentName" & vbCrLf &
            ",LibraryStockInformation.DepartmentID" & vbCrLf

        If vIncludeStocks Then
            pSql &= ",DrpCacheStockPosDaily.StockID" & vbCrLf &
                ",LibraryStockInformation.StockNumber" & vbCrLf & _
                " ,ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & vbCrLf
        End If

        pSql &= " FROM DrpCacheStockPosDaily" & vbCrLf & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockPosDaily.StockID" & vbCrLf &
            " JOIN LibraryDepartments ON LibraryDepartments.ID = LibraryStockInformation.DepartmentID" & vbCrLf

        If vIncludeStocks Then
            pSql &= " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
                " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf
        End If

        pSql &= " WHERE CONVERT(date, DrpCacheStockPosDaily.TransDate) " & vbCrLf & _
            " BETWEEN " & ReadyString(vDateFrom.Date) & vbCrLf & _
            " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " AND DrpCacheStockPosDaily.BranchID = " & vBranchID & vbCrLf & _
            " GROUP BY LibraryDepartments.DepartmentName" & vbCrLf &
            ",LibraryStockInformation.DepartmentID" & vbCrLf

        If vIncludeStocks Then
            pSql &= ",DrpCacheStockPosDaily.StockID" & vbCrLf &
                " ,LibraryStockInformation.StockNumber" & vbCrLf & _
                " ,ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') +" & vbCrLf & _
                " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')" & vbCrLf

        End If

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetSalesDataGrouped(vBranchID As Integer,
        vThisYear As DateRange, vLastYear As DateRange,
        vIncludeStocks As Boolean) As DataTable

        Dim pDtThisYear As DataTable
        Dim pDtLastYear As DataTable
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pKeys, pFields As List(Of String)

        pDtThisYear = GetSalesDataGrouped(vBranchID, vThisYear.DateFrom, vThisYear.DateTo, "ThisYear", vIncludeStocks)
        pDtLastYear = GetSalesDataGrouped(vBranchID, vLastYear.DateFrom, vLastYear.DateTo, "LastYear", vIncludeStocks)

        '#If DEBUG Then
        '        pDtThisYear = New DataView(pDtThisYear, "DepartmentName = 'WRITING & CORRECTION INSTRUMENTS & ACCESSORIES'", "", DataViewRowState.CurrentRows).ToTable
        '        pDtLastYear = New DataView(pDtLastYear, "DepartmentName = 'WRITING & CORRECTION INSTRUMENTS & ACCESSORIES'", "", DataViewRowState.CurrentRows).ToTable
        '#End If

        pKeys = New List(Of String)
        With pKeys
            .Add("DepartmentID")
            If vIncludeStocks Then
                .Add("StockID")
            End If
        End With

        pFields = New List(Of String)
        With pFields
            .Add("DepartmentID")
            .Add("DepartmentName")
            .Add("AmountThisYear")
            .Add("CogsThisYear")
            .Add("HitsThisYear")

            If vIncludeStocks Then
                .Add("StockDescription")
                .Add("StockNumber")
            End If
        End With

        pDT = MergeTable(pDtThisYear, pDtLastYear, pKeys.ToArray, pFields.ToArray)

        For Each pDR In pDT.Rows
            For Each pField As String In New String() {"AmountThisYear", "CogsThisYear", "AmountLastYear", "CogsLastYear"}
                If pDR(pField) IsNot DBNull.Value Then
                    pDR(pField) = Math.Round(pDR(pField), mDecimals)
                End If
            Next pField
        Next pDR

        '#If DEBUG Then
        '        TableToExcelFile(pDtThisYear, "DepartmentName = 'WRITING & CORRECTION INSTRUMENTS & ACCESSORIES'", "DepartmentName", Common.ServerPath & "temp\pDtThisYear.xls")
        '        TableToExcelFile(pDtLastYear, "DepartmentName = 'WRITING & CORRECTION INSTRUMENTS & ACCESSORIES'", "DepartmentName", Common.ServerPath & "temp\pDtLastYear.xls")
        '        TableToExcelFile(pDT, "DepartmentName = 'WRITING & CORRECTION INSTRUMENTS & ACCESSORIES'", "DepartmentName", Common.ServerPath & "temp\pDT.xls")
        '#End If

        Return pDT

    End Function

    Function ByCategory() As ActionResult
        Return View()
    End Function

    Public Function GetDataByCategory() As String

        Dim pJson As JsonDictionary

        pJson = New JsonDictionary
        pJson.Add("wtd", GetWtd(True))
        pJson.Add("mtd", GetMtd(True))
        pJson.Add("qtd", GetQtd(True))
        pJson.Add("ytd", GetYtd(True))

        Return pJson.Serialize

    End Function

    ''' <summary>
    ''' get branches that have sales for the last 2 calendar years
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetBranches() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT tblBranch.ID" & vbCrLf & _
            " ,tblBranch.Name" & vbCrLf & _
            " FROM DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch" & vbCrLf & _
            " JOIN DrpCacheStockPosDaily ON DrpCacheStockPosDaily.BranchID = tblBranch.ID" & vbCrLf & _
            " WHERE YEAR(DrpCacheStockPosDaily.TransDate) BETWEEN YEAR(GETDATE()) - 1 AND YEAR(GETDATE())" & vbCrLf & _
            " GROUP BY tblBranch.ID" & vbCrLf & _
            " ,tblBranch.Name" & vbCrLf & _
            " ORDER BY tblBranch.Name" & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

#Region " export to excel"
    <HttpPost()>
    Public Function GetExcel() As String

        Dim pDT As DataTable
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim pThisYearDates As DateRange
        Dim pLastYearDates As DateRange

        Select Case Common.WebRequest.Data("Period")
            Case "w"
                Client.GetWtdDates(pThisYearDates, pLastYearDates)

            Case "m"
                Client.GetMtdDates(pThisYearDates, pLastYearDates)

            Case "q"
                Client.GetQtdDates(pThisYearDates, pLastYearDates)

            Case "y"
                Client.GetYtdDates(pThisYearDates, pLastYearDates)

        End Select


        pWB = New Workbook

        pWS = pWB.Worksheets(0)
        pWS.Name = DisplayableDateRange(pThisYearDates)
        pDT = GetTransData(pThisYearDates, Common.WebRequest.Data("BranchID"))
        ViewToExcel(pDT.DefaultView, pWS)

        pWS = pWB.Worksheets.Add(DisplayableDateRange(pLastYearDates))
        pDT = GetTransData(pLastYearDates, Common.WebRequest.Data("BranchID"))
        ViewToExcel(pDT.DefaultView, pWS)

        Dim pFileTitle As String
        Dim pFilename As String

        pFileTitle = "POS Trans " & DateTimeStamp() & ".xls"
        pFileTitle = CreateValidFilename(pFileTitle, True)
        pFilename = Common.ServerPath & "temp\documents\" & pFileTitle
        CreateFolder(pFilename)
        pWB.Save(pFilename)

        Return JsonDictionary.SerializeText(HttpUtility.UrlEncode(pFileTitle))

    End Function

    Private Function GetTransData(vDates As DateRange, vBranchID As Integer) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "WITH CTE AS (SELECT 'SI-' + CONVERT(varchar, POSSales.InvoiceNumber) AS TransNum" & vbCrLf &
            ",POSSales.LastUpdated" & vbCrLf & _
            " ,LibraryStockInformation.StockNumber" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & vbCrLf & _
            " ,POSSalesItems.Quantity" & vbCrLf & _
            " ,POSSalesItems.UnitMeasure" & vbCrLf & _
            " ,POSSalesItems.ActualQuantity" & vbCrLf & _
            " ,POSSalesItems.UnitPrice" & vbCrLf & _
            " ,POSSalesItems.UnitPrice * POSSalesItems.Quantity AS Amount" & vbCrLf & _
            " ,POSSalesItems.COGS" & vbCrLf & _
            " ,POSSalesItems.COGS / POSSalesItems.Quantity AS UnitCogs" & vbCrLf & _
            " " & vbCrLf & _
            " FROM POSSales" & vbCrLf & _
            " JOIN POSSalesItems ON POSSalesItems.POSSalesID = POSSales.ID" & vbCrLf & _
            " LEFT JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" & vbCrLf & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf & _
            "             " & vbCrLf & _
            " WHERE IsHold = 0" & vbCrLf & _
            " AND POSSales.BranchID = " & vBranchID & vbCrLf &
            " AND CONVERT(date, POSSales.LastUpdated) BETWEEN " & ReadyString(vDates.DateFrom.Date) & " AND " & ReadyString(vDates.DateTo.Date) & vbCrLf &
            " UNION ALL" & vbCrLf &
            " SELECT 'CS-' + CONVERT(varchar, POSCashSlipSales.CSNumber) AS TransNum" & vbCrLf &
            ",POSCashSlipSales.LastUpdated" & vbCrLf & _
            " ,LibraryStockInformation.StockNumber" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') +" & vbCrLf & _
            " ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & vbCrLf & _
            " ,POSCashSlipSalesItems.Quantity" & vbCrLf & _
            " ,POSCashSlipSalesItems.UnitMeasure" & vbCrLf & _
            " ,POSCashSlipSalesItems.ActualQuantity" & vbCrLf & _
            " ,POSCashSlipSalesItems.UnitPrice" & vbCrLf & _
            " ,POSCashSlipSalesItems.UnitPrice * POSCashSlipSalesItems.Quantity AS Amount" & vbCrLf & _
            " ,POSCashSlipSalesItems.COGS" & vbCrLf & _
            " ,POSCashSlipSalesItems.COGS / POSCashSlipSalesItems.Quantity AS UnitCogs" & vbCrLf & _
            " " & vbCrLf & _
            " FROM POSCashSlipSales" & vbCrLf & _
            " JOIN POSCashSlipSalesItems ON POSCashSlipSalesItems.CashSlipID = POSCashSlipSales.ID" & vbCrLf & _
            " LEFT JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" & vbCrLf & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf & _
            "             " & vbCrLf & _
            " WHERE POSCashSlipSales.BranchID = " & vBranchID & vbCrLf &
            " AND CONVERT(date, POSCashSlipSales.LastUpdated) BETWEEN " & ReadyString(vDates.DateFrom.Date) & " AND " & ReadyString(vDates.DateTo.Date) & vbCrLf &
            ")" & vbCrLf & vbCrLf

        pSql &= " SELECT TransNum" & vbCrLf & _
            ",LastUpdated" & vbCrLf &
            ",StockNumber" & vbCrLf &
            ",StockDescription" & vbCrLf &
            ",SUM(Quantity) AS Quantity" & vbCrLf &
            ",UnitMeasure" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",SUM(Amount) AS Amount" & vbCrLf &
            ",SUM(COGS) AS COGS" & vbCrLf &
            ",UnitCogs" & vbCrLf &
            " ,SUM(Amount) - SUM(COGS) AS GP" & vbCrLf & _
            " ,(SUM(Amount) - SUM(COGS)) / SUM(Amount) AS GPM" & vbCrLf & _
            " FROM CTE" & vbCrLf & _
            " GROUP BY TransNum" & vbCrLf & _
            ",LastUpdated" & vbCrLf &
            ",StockNumber" & vbCrLf &
            ",StockDescription" & vbCrLf &
            ",UnitMeasure" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",UnitCogs" & vbCrLf &
            " ORDER BY LastUpdated" & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            For Each pField As String In New String() {"UnitPrice", "Amount", "COGS", "UnitCogs", "GP", "GPM"}
                If pDR(pField) IsNot DBNull.Value Then
                    pDR(pField) = Math.Round(pDR(pField), mDecimals)
                End If
            Next pField
        Next pDR

        Return pDT

    End Function
#End Region

#Region " By Category export"
    ''' <summary>
    ''' returns JSON
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    Public Function DownloadByCategory() As String

        Dim pFilename As String
        Dim pFileTitle As String
        Dim pDT, pDt2 As DataTable
        Dim pDTs As List(Of DataTable)
        Dim pDtThisYear, pDtLastYear, pDtCompare As DataTable
        Dim pDtThisYear2, pDtLastYear2, pDtCompare2 As DataTable
        Dim pSort As String
        Dim pFormat As ProReport.eExportFormat
        Dim pBranchID As Integer

        Dim pThisYearDates As DateRange
        Dim pLastYearDates As DateRange
        Dim pReport As ProReport
        Dim pIncludeStocks As Boolean

        Select Case Common.WebRequest.Data("Period")
            Case "w"
                Client.GetWtdDates(pThisYearDates, pLastYearDates)

            Case "m"
                Client.GetMtdDates(pThisYearDates, pLastYearDates)

            Case "q"
                Client.GetQtdDates(pThisYearDates, pLastYearDates)

            Case "y"
                Client.GetYtdDates(pThisYearDates, pLastYearDates)

            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid period")

        End Select

        If Integer.TryParse(CStr2(Common.WebRequest.Data("BranchID")), pBranchID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid BranchID")
        End If

        Select Case Common.WebRequest.Data("FileFormat")
            Case "pdf"
                pFormat = ProReport.eExportFormat.Pdf

            Case "excel"
                pFormat = ProReport.eExportFormat.Excel

        End Select
        'pFilename = Common.ServerPath & "data\Sales and Profit Margins by Product Department template.xlsx"
        'If IO.File.Exists(pFilename) = False Then
        '    Return JsonDictionary.Serialize("Error", "Unable to find the template file, " & pFilename)
        'End If

        pDTs = New List(Of DataTable)
        pIncludeStocks = Common.WebRequest.Data("ReportType") = "ByDepartmentAndSku"
        pDT = GetSalesDataGrouped(pBranchID, pThisYearDates, pLastYearDates, False)
        pDTs.Add(pDT)
        If pIncludeStocks Then
            pDt2 = GetSalesDataGrouped(pBranchID, pThisYearDates, pLastYearDates, True)
            pDTs.Add(pDt2)
        End If

        For Each pItem As DataTable In pDTs
            With pItem.Columns
                .Add("ProfitThisYear", GetType(Decimal), "ISNULL(AmountThisYear, 0) - ISNULL(CogsThisYear, 0)")
                .Add("ProfitLastYear", GetType(Decimal), "ISNULL(AmountLastYear, 0) - ISNULL(CogsLastYear, 0)")

                .Add("AmountDiff", GetType(Decimal), "ISNULL(AmountThisYear, 0) - ISNULL(AmountLastYear, 0)")
                .Add("AmountPercentDiff", GetType(Decimal))
                .Add("ProfitDiff", GetType(Decimal), "ISNULL(ProfitThisYear, 0) - ISNULL(ProfitLastYear, 0)")
                .Add("ProfitPercentDiff", GetType(Decimal))
                .Add("MarginDiff", GetType(Decimal))
                .Add("HitsDiff", GetType(Decimal))

                .Add("MarginThisYear", GetType(Decimal))
                .Add("MarginLastYear", GetType(Decimal))

                .Add("CmToTotalThisYear", GetType(Decimal))
                .Add("CmToTotalLastYear", GetType(Decimal))

                .Add("CmToProfitThisYear", GetType(Decimal))
                .Add("CmToProfitLastYear", GetType(Decimal))
            End With
        Next pItem

        Dim pTotalSalesThisYear, pTotalSalesLastYear, pTotalProfitThisYear, pTotalProfitLastYear As Decimal

        pTotalSalesThisYear = CDec2(pDT.Compute("SUM(AmountThisYear)", ""))
        pTotalSalesLastYear = CDec2(pDT.Compute("SUM(AmountLastYear)", ""))
        pTotalProfitThisYear = CDec2(pDT.Compute("SUM(ProfitThisYear)", ""))
        pTotalProfitLastYear = CDec2(pDT.Compute("SUM(ProfitLastYear)", ""))

        For Each pItem As DataTable In pDTs
            For Each pDR As DataRow In pItem.Rows
                If CDec2(pDR("AmountThisYear")) <> 0 Then
                    pDR("MarginThisYear") = CDec2(pDR("ProfitThisYear")) / pDR("AmountThisYear") * 100
                End If
                If CDec2(pDR("AmountLastYear")) <> 0 Then
                    pDR("MarginLastYear") = CDec2(pDR("ProfitLastYear")) / pDR("AmountLastYear") * 100
                    pDR("AmountPercentDiff") = CDec2("AmountDiff") / pDR("AmountLastYear") * 100
                    If CDec2(pDR("MarginLastYear")) <> 0 Then
                        pDR("MarginDiff") = CDec2(pDR("MarginThisYear")) / pDR("MarginLastYear") - 100
                    End If
                End If
                If CDec2(pDR("ProfitLastYear")) <> 0 Then
                    pDR("ProfitPercentDiff") = CDec2(pDR("ProfitDiff")) / pDR("ProfitLastYear") * 100
                End If
                If CDec2(pDR("HitsLastYear")) <> 0 Then
                    pDR("HitsDiff") = CDec2(pDR("HitsThisYear")) / pDR("HitsLastYear") - 100
                End If

                If pTotalSalesThisYear <> 0 Then pDR("CmToTotalThisYear") = CDec2(pDR("AmountThisYear")) / pTotalSalesThisYear * 100
                If pTotalProfitThisYear <> 0 Then pDR("CmToProfitThisYear") = CDec2(pDR("ProfitThisYear")) / pTotalProfitThisYear * 100
                If pTotalSalesLastYear <> 0 Then pDR("CmToTotalLastYear") = CDec2(pDR("AmountLastYear")) / pTotalSalesLastYear * 100
                If pTotalProfitLastYear <> 0 Then pDR("CmToProfitLastYear") = CDec2(pDR("ProfitLastYear")) / pTotalProfitLastYear * 100
            Next pDR
            RoundOffTable(pItem, mDecimals)
        Next pItem

        Select Case CStr2(Common.WebRequest.Data("Sort"))
            Case "Sales value"
                pSort = "AmountThisYear DESC, AmountLastYear DESC"

            Case "CM to total sales"
                pSort = "CmToTotalThisYear DESC, CmToTotalLastYear DESC"

            Case "Gross profit"
                pSort = "ProfitThisYear DESC, ProfitLastYear DESC"

            Case "CM to total gross profit"
                pSort = "CmToProfitThisYear DESC, CmToProfitLastYear DESC"

            Case "Gross margin"
                pSort = "MarginThisYear DESC, MarginLastYear DESC"

            Case "Hits"
                pSort = "HitsThisYear DESC, HitsLastYear DESC"

            Case Else
                pSort = "DepartmentName"

        End Select

        pDT.DefaultView.Sort = pSort
        pDT = pDT.DefaultView.ToTable
        pDT.DefaultView.RowFilter = "HitsThisYear > 0"
        pDtThisYear = New DataView(pDT, "HitsThisYear > 0", "", DataViewRowState.CurrentRows).ToTable
        pDtLastYear = New DataView(pDT, "HitsLastYear > 0", "", DataViewRowState.CurrentRows).ToTable
        pDtCompare = New DataView(pDT, "HitsThisYear > 0 OR HitsLastYear > 0", "", DataViewRowState.CurrentRows).ToTable

        If pIncludeStocks Then
            pDt2.DefaultView.Sort = pSort
            pDt2 = pDt2.DefaultView.ToTable
            pDt2.DefaultView.RowFilter = "HitsThisYear > 0"
            pDtThisYear2 = New DataView(pDt2, "HitsThisYear > 0", "", DataViewRowState.CurrentRows).ToTable
            pDtLastYear2 = New DataView(pDt2, "HitsLastYear > 0", "", DataViewRowState.CurrentRows).ToTable
            pDtCompare2 = New DataView(pDt2, "HitsThisYear > 0 OR HitsLastYear > 0", "", DataViewRowState.CurrentRows).ToTable
        End If

#If DEBUG Then
        Common.WebRequest.ProData.CreateTable("rptRetailByCategoryThisYear", pDtThisYear)
        Common.WebRequest.ProData.CreateTable("rptRetailByCategoryLastYear", pDtLastYear)
        Common.WebRequest.ProData.CreateTable("rptRetailByCategoryCompare", pDtCompare)

        If pIncludeStocks Then
            Common.WebRequest.ProData.CreateTable("rptRetailByCategoryThisYear2", pDtThisYear2)
            Common.WebRequest.ProData.CreateTable("rptRetailByCategoryLastYear2", pDtLastYear2)
            Common.WebRequest.ProData.CreateTable("rptRetailByCategoryCompare2", pDtCompare2)
        End If
#End If

        pFileTitle = "Sales and Profit Margins by Product Department " & DateTimeStamp()
        Select Case pFormat
            Case ProReport.eExportFormat.Excel
                pFileTitle &= ".xls"

            Case ProReport.eExportFormat.Pdf
                pFileTitle &= ".pdf"

        End Select
        pFileTitle = CreateValidFilename(pFileTitle, True)
        pFilename = Common.ServerPath & "temp\documents\" & pFileTitle
        CreateFolder(pFilename)

        pReport = New ProReport
        pReport.AutoFillHeader = False
        If pIncludeStocks Then
            pReport.SourceFilename = "RetailByCategory"
        Else
            pReport.SourceFilename = "RetailByCategory"
            pReport.Report.Dictionary.Relations.Remove(pReport.Report.Dictionary.Relations("r1"))
        End If
        pReport.Variables("DateRangeThisYear") = Format(pThisYearDates.DateFrom, "MM/dd/yyyy") & " to " & Format(pThisYearDates.DateTo, "MM/dd/yyyy")
        pReport.Variables("DateRangeLastYear") = Format(pLastYearDates.DateFrom, "MM/dd/yyyy") & " to " & Format(pLastYearDates.DateTo, "MM/dd/yyyy")
        pReport.AddTable("rptRetailByCategoryThisYear", pDtThisYear)
        pReport.AddTable("rptRetailByCategoryLastYear", pDtLastYear)
        pReport.AddTable("rptRetailByCategoryCompare", pDtCompare)

        If pIncludeStocks Then
            pReport.AddTable("rptRetailByCategoryThisYear2", pDtThisYear2)
            pReport.AddTable("rptRetailByCategoryLastYear2", pDtLastYear2)
            pReport.AddTable("rptRetailByCategoryCompare2", pDtCompare2)
        End If
        pReport.ExportFile(pFormat, pFilename)

        If CDec2(Common.WebRequest.Data("Compress")) <> 0 Then
            Dim pZip As ProZip
            Dim pZipFilename As String

            pFileTitle = IO.Path.GetFileNameWithoutExtension(pFilename) & ".zip"
            pZipFilename = AppendBackslash(IO.Path.GetDirectoryName(pFilename)) & pFileTitle
            pZip = New ProZip(pZipFilename)
            pZip.AddFile(pFilename)
        End If

        Return JsonDictionary.SerializeText(HttpUtility.UrlEncode(pFileTitle))

    End Function
#End Region
End Class

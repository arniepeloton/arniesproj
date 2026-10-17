Public Class DemandDataController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pJson As New JsonDictionary
        Dim pGeneratedColumns As List(Of DataColumn)
        Dim pStr As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pJson("Result") = "ok"

        pStr = Common.WebRequest.Data("q")
        If CStr2(pStr).Length > 0 Then
            pStr = HttpUtility.ParseQueryString(pStr)("id")
            If CStr2(pStr).Length > 0 Then pStr &= " "
        End If
        pJson.Add("Data", GetStockRanking(pStr & Common.WebRequest.Data("txtDemandDataSearch")))

        Return pJson.Serialize

    End Function

    Private Function GetStockRanking(vKeywords As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pTotalCogs, pAccumulatedPercentage As Decimal
        Dim pRank As Integer
        Dim pFinder As Finder

        pFinder = New Finder
        pFinder.MaxRecords = 1000
        pFinder.SearchString = vKeywords
        With pFinder.Fields
            With .Add("StockID", SqlDbType.UniqueIdentifier)
                .Expression = "DrpCacheStockSalesDaily.StockID"
            End With
            With .Add("StockNumber", SqlDbType.VarChar)
                .Expression = "LibraryStockInformation.StockNumber"
            End With
            With .Add("StockDescription", SqlDbType.VarChar)
                .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
                    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
                    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
                    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
                    "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')"
            End With
            With .Add("Qty", SqlDbType.Int)
                .Expression = "SUM(DrpCacheStockSalesDaily.QtyInternal + DrpCacheStockSalesDaily.QtyExternal)"
                .Searchable = False
            End With
            With .Add("COGS", SqlDbType.Decimal)
                .Expression = "SUM(DrpCacheStockSalesDaily.COGS)"
                .Searchable = False
            End With
        End With
        pFinder.SqlFrom = " FROM DrpCacheStockSalesDaily" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDaily.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID"
        pFinder.SqlWhere = " WHERE DATEDIFF(day, DrpCacheStockSalesDaily.TransDate, GETDATE()) < 365"
        pFinder.SqlGroup = " GROUP BY DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')"
        pFinder.SqlOrder = " ORDER BY SUM(DrpCacheStockSalesDaily.COGS) DESC"
        pSql = pFinder.CreateSql()

        pDT = Common.WebRequest.ProData.Execute(pSql)
        With pDT.Columns
            .Add("Rank", GetType(Integer))
            .Add("PercentageOfTotal", GetType(Decimal))
            .Add("AccumulatedPercentage", GetType(Decimal))
        End With

        pTotalCogs = CDec2(pDT.Compute("SUM(COGS)", ""))
        If pTotalCogs > 0 Then
            For Each pDR As DataRow In pDT.Rows
                pRank += 1
                pDR("Rank") = pRank
                pDR("PercentageOfTotal") = pDR("COGS") / pTotalCogs
                pAccumulatedPercentage += pDR("PercentageOfTotal")
                pDR("AccumulatedPercentage") = pAccumulatedPercentage
            Next pDR
        End If

        Return pDT

    End Function

    '<HttpPost()>
    Public Function Resummarize() As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pJson As JsonDictionary

        Dim pFilename As String
        Dim pJsonObj As Dictionary(Of String, Object)
        Dim pLastUpdated As Date
        Dim pTransDate As Date?
        Dim pTransDateStart As Date

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsEnable) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        Try
            pFilename = Common.ServerPath & "temp\DemandResummarize.txt"
            If IO.File.Exists(pFilename) Then
                pJsonObj = New JavaScriptSerializer().DeserializeObject(FileText(pFilename))
                pJsonObj = pJsonObj("d")
                pLastUpdated = CDate(pJsonObj("LastUpdated")).ToLocalTime
                pTransDate = CDate(pJsonObj("TransDate")).ToLocalTime
                pTransDateStart = CDate(pJsonObj("TransDateStart")).ToLocalTime
                If DateDiff(DateInterval.Minute, pLastUpdated, Now) < 15 Then
                    pJson = New JsonDictionary
                    pJson("Result") = "Another process is currently resummarizing the demand data."
                    pJson("LastUpdated") = pLastUpdated
                    pJson("TransDate") = pTransDate
                    pJson("TransDateStart") = pTransDateStart
                    Return pJson.Serialize
                Else
                    If pTransDate = Now.Date Then
                        pJson = New JsonDictionary
                        pJson("Result") = "Summary demand date is up to date"
                        Return pJson.Serialize
                    End If
                End If
            End If
        Catch ex As Exception
            Stop2()
        End Try

        If pTransDate Is Nothing Then
            ' find the earliest sales order
            pSql = "SELECT MIN(LastUpdated) " &
                " FROM TransSalesOrder" &
                " WHERE TransSalesOrder.IsCancelled = 0"
            pTransDate = Common.WebRequest.ProData.ExecuteScalar(pSql)
            If pTransDate Is Nothing Then Exit Function ' no sales order
        End If
        pTransDate = pTransDate.Value.Date

        pJson = New JsonDictionary
        pJson("TransDateStart") = pTransDate

        Do
            pJson("TransDate") = pTransDate
            pJson("TransDateString") = pTransDate.ToString
            pJson("LastUpdated") = Now
            pJson("LastUpdatedString") = Now.ToString
            FileText(pFilename) = pJson.Serialize

            Resummarize(pTransDate)

            If Now.Date <= pTransDate Then Exit Do
            pTransDate = pTransDate.Value.AddDays(1)
        Loop

        pJson = New JsonDictionary
        pJson("Result") = "ok"

        Return pJson.Serialize

    End Function

    Private Sub Resummarize(vTransDate As Date)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDT As DataTable

        ' the ff code was removed. keep for reference on how to use ROW_NUMBER:
        '" ,ROW_NUMBER() OVER(ORDER BY " & vbCrLf & _
        '" 	CONVERT(datetime, FLOOR(CONVERT(float, TransSalesOrder.LastUpdated))) DESC," & vbCrLf & _
        '" 	TransSalesOrderItems.StockID" & vbCrLf & _
        '" ) AS RN" & vbCrLf & _

        ' stocknumber and stockDescription are not needed
        pSql = "WITH CTE AS " & vbCrLf & _
            " (" & vbCrLf & _
            " SELECT TransSalesOrderItems.StockID" & vbCrLf & _
            " ,LibraryStockInformation.StockNumber" & vbCrLf & _
            " ,	ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & vbCrLf & _
            " 	ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & vbCrLf & _
            " 	ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & vbCrLf & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & vbCrLf & _
            " 	ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & vbCrLf & _
            " ,SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Qty" & vbCrLf & _
            " ,SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity * TransSalesOrderItems.Cost) AS COGS" & vbCrLf & _
            " ,CONVERT(datetime, FLOOR(CONVERT(float, TransSalesOrder.LastUpdated))) AS TransDate" & vbCrLf & _
            ",LibraryCustomers.IsCompanyBranch" & vbCrLf & _
            " FROM TransSalesOrder" & vbCrLf & _
            " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & vbCrLf & _
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransSalesOrderItems.StockID" & vbCrLf & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf & _
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" & vbCrLf & _
            " " & vbCrLf & _
            " WHERE CONVERT(datetime, FLOOR(CONVERT(float, TransSalesOrder.LastUpdated))) = " & ReadyString(vTransDate) & vbCrLf & _
            " AND TransSalesOrder.IsCancelled = 0" &
            " " & vbCrLf & _
            " GROUP BY TransSalesOrderItems.StockID" & vbCrLf & _
            " ,LibraryStockInformation.StockNumber" & vbCrLf & _
            " ,LibraryBrands.BrandName" & vbCrLf & _
            " ,LibraryProducts.ProductName" & vbCrLf & _
            " ,LibraryStockInformation.Make" & vbCrLf & _
            " ,LibraryStockInformation.UnitSize" & vbCrLf & _
            " ,LibraryStockInformation.UnitSize" & vbCrLf & _
            " ,LibraryStockInformation.Content" & vbCrLf & _
            " ,CONVERT(datetime, FLOOR(CONVERT(float, TransSalesOrder.LastUpdated)))" & vbCrLf & _
            ",LibraryCustomers.IsCompanyBranch" & vbCrLf & _
            " )" & vbCrLf
        pSql &= " SELECT StockID" &
            ",StockNumber" &
            ",StockDescription" &
            ",TransDate" &
            ",ISNULL((SELECT SUM(Qty)" &
            "   FROM CTE CTE1" &
            "   WHERE CTE1.StockID = CTE.StockID" &
            "   AND CTE1.TransDate = CTE.TransDate" &
            "   AND CTE1.IsCompanyBranch = 0), 0) AS QtyExternal" &
            ",ISNULL((SELECT SUM(Qty)" &
            "   FROM CTE CTE1" &
            "   WHERE CTE1.StockID = CTE.StockID" &
            "   AND CTE1.TransDate = CTE.TransDate" &
            "   AND CTE1.IsCompanyBranch = 1), 0) AS QtyInternal" &
            ",ISNULL((SELECT SUM(COGS)" &
            "   FROM CTE CTE1" &
            "   WHERE CTE1.StockID = CTE.StockID" &
            "   AND CTE1.TransDate = CTE.TransDate), 0) AS COGS" &
            " FROM CTE" & vbCrLf & _
            " GROUP BY StockID" & vbCrLf &
            ",StockNumber" &
            ",StockDescription" &
            ",TransDate" &
            " " & vbCrLf
        Common.WebRequest.ProData.CommandTimeout = Integer.MaxValue
        pDT = Common.WebRequest.ProData.Execute(pSql)

        pSql = "DELETE FROM DrpCacheStockSalesDaily" &
            " WHERE TransDate = " & ReadyString(vTransDate)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("DrpCacheStockSalesDaily")
        pIU("ID") = "newid()"
        For Each pDR As DataRow In pDT.Rows
            pIU("StockID") = ReadyString(pDR("StockID"))
            pIU("QtyExternal") = pDR("QtyExternal")
            pIU("QtyInternal") = pDR("QtyInternal")
            pIU("COGS") = CDec2(pDR("COGS"))
            pIU("TransDate") = ReadyString(pDR(("TransDate")))
            pIU("WeekNum") = WeekNumber(pDR(("TransDate")))
            pIU("WeekYear") = CDate(pDR(("TransDate"))).Year
            pSql = pIU.GetInsert
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Next pDR

    End Sub

    <HttpPost()>
    Public Function GetStock() As String

        Dim pJson As JsonDictionary
        Dim pStockID As String
        Dim pDtStock As DataTable
        Dim pHighestQty As Integer

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        pStockID = Common.WebRequest.Data("StockID")
        pDtStock = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pStockID))

        pJson = New JsonDictionary
        If pDtStock.Rows.Count = 0 Then
            pJson("Result") = "Invalid StockID"
        Else
            pJson("StockNumber") = pDtStock.Rows(0)("StockNumber")
            pJson("StockDescription") = pDtStock.Rows(0)("StockDescription")
            pJson("Unit") = WebRequest.Instance.Business.LibraryStock.GetBasicUnit(pStockID)
            pJson.Add("Data", GetMonthlyData(pStockID, pHighestQty))
            pJson("Year") = Now.Year
            pJson("HighestQty") = pHighestQty
        End If

        Return pJson.Serialize

    End Function

    Private Function GetMonthlyData(vStockID As String, ByRef oHighestQty As Integer) As DataTable

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim p As Integer

        pSql = "SELECT StockID" & vbCrLf & _
            " ,ISNULL(SUM(QtyInternal), 0) AS QtyInternal" & vbCrLf & _
            " ,ISNULL(SUM(QtyExternal), 0) AS QtyExternal" & vbCrLf & _
            " ,DatePart(month, TransDate) AS TransMonth" & vbCrLf & _
            " ,DatePart(year, TransDate) AS TransYear" & vbCrLf & _
            " FROM DrpCacheStockSalesDaily" & vbCrLf & _
            " WHERE StockID = " & ReadyString(vStockID) &
            " AND DatePart(year, TransDate) BETWEEN " & Now.Year - 2 & " AND " & Now.Year &
            " GROUP BY StockID" & vbCrLf & _
            " ,DatePart(month, TransDate)" & vbCrLf & _
            " ,DatePart(year, TransDate)" & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)
        'pDT = GetRandomSalesData()

        '// fill gap for years
        For p = Now.Year - 2 To Now.Year
            pDT.DefaultView.RowFilter = "TransYear = " & p
            If pDT.DefaultView.Count = 0 Then
                pDR = pDT.NewRow
                pDR("TransYear") = p
                pDR("TransMonth") = 1
            End If
        Next p

        pDT.Columns.Add("Qty", GetType(Integer), "QtyInternal + QtyExternal")
        oHighestQty = CDec2(pDT.Compute("MAX(Qty)", ""))
        pDT.DefaultView.RowFilter = ""
        pDT.DefaultView.Sort = "TransYear DESC, TransMonth"
        pDT = pDT.DefaultView.ToTable

        pDT = CreateCrossTab(pDT, {"TransMonth"}, {
            New CrossColumn("Internal {TransYear}", "QtyInternal"),
            New CrossColumn("External {TransYear}", "QtyExternal"),
            New CrossColumn("Total {TransYear}", "Qty")
        })
        pDT.Columns.Add("MonthName")

        For p = 1 To 12
            pDT.DefaultView.RowFilter = "TransMonth = " & p
            If pDT.DefaultView.Count = 0 Then
                pDR = pDT.NewRow
                pDR("TransMonth") = p
                pDT.Rows.Add(pDR)
            Else
                pDR = pDT.DefaultView(0).Row
            End If
            pDR("MonthName") = MonthName(p)
        Next p

        pDT.DefaultView.Sort = "TransMonth"
        pDT.DefaultView.RowFilter = ""
        pDT = pDT.DefaultView.ToTable

        Return pDT

    End Function
End Class

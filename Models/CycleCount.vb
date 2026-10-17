Public Class CycleCount
    Dim pSql As String = vbNullString
    Dim pDT As DataTable

    Public Function getAllWarehouses() As String
        pSql = "SELECT ID, WarehouseName FROM LibraryWarehouses "

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Public Function getAllBranches() As String

        pSql = "SELECT ID, Name as BranchName FROM DavaoUnifiedPayroll.dbo.LibraryBranch "

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)

    End Function

    Public Function searchStock(ByVal sdt As Dictionary(Of String, Object)) As String

        Dim strStock() As String
        Dim ctr As Integer

        strStock = Split(sdt("stock"), " ")

        pSql = "SELECT TOP 20 COUNT(dbo.TransCycleCountStocksDetail.Location) AS Counted, dbo.TransCycleCountStocks.StockID, dbo.LibraryStockInformation.StockNumber, " _
            & "ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '')  " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Content)), ', '), '') AS StockDescription, " _
            & "Drp.dbo.CacheStockRank.RankLetter, Drp.dbo.CacheStockRank.RankNum " _
            & "FROM dbo.TransCycleCountStocksDetail RIGHT OUTER JOIN " _
            & "dbo.TransCycleCountStocks ON dbo.TransCycleCountStocksDetail.StockID = dbo.TransCycleCountStocks.StockID LEFT OUTER JOIN " _
            & "dbo.LibraryStockInformation INNER JOIN dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID INNER JOIN " _
            & "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID ON dbo.TransCycleCountStocks.StockID = dbo.LibraryStockInformation.ID " _
            & "LEFT OUTER JOIN Drp.dbo.CacheStockRank ON Drp.dbo.CacheStockRank.StockID = dbo.TransCycleCountStocksDetail.StockID " _
            & "WHERE dbo.LibraryStockInformation.StockNumber + ' ' + ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '')  " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Content)), ', '), '') " _
            & "like '%" & strStock(ctr) & "%' "

        ctr += 1

        While ctr <= UBound(strStock)
            pSql = pSql & " and dbo.LibraryStockInformation.StockNumber + ' ' + ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '')  " _
            & "+ ISNULL(NULLIF (' ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " _
            & "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Content)), ', '), '') " _
            & "like '%" & strStock(ctr) & "%' "
            ctr += 1
        End While

        pSql = pSql & " GROUP BY Drp.dbo.CacheStockRank.RankLetter, Drp.dbo.CacheStockRank.RankNum, dbo.TransCycleCountStocks.StockID, dbo.LibraryBrands.BrandName, dbo.LibraryStockInformation.StockNumber, dbo.LibraryProducts.ProductName, dbo.LibraryStockInformation.Make,  " _
            & "dbo.LibraryStockInformation.UnitSize, dbo.LibraryStockInformation.Content " _
            & "ORDER BY Drp.dbo.CacheStockRank.RankLetter, Drp.dbo.CacheStockRank.RankNum, StockNumber, StockDescription"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)

    End Function


    Public Function searchUnitMeasure() As String

        'pSql = "SELECT dbo.TransCycleCountStocks.UnitMeasureID1, dbo.LibraryUnitMeasures.UnitMeasure AS UnitMeasure1, dbo.TransCycleCountStocks.UnitMeasureID2, " _
        '    & "LibraryUnitMeasures_1.UnitMeasure AS UnitMeasure2, dbo.TransCycleCountStocks.UnitMeasureID3, " _
        '    & "LibraryUnitMeasures_2.UnitMeasure AS UnitMeasure3, dbo.TransCycleCountStocks.UnitMeasureID4, " _
        '    & "LibraryUnitMeasures_3.UnitMeasure AS UnitMeasure4, COALESCE (dbo.TransCycleCountStocksDetail.Good1, 0) AS Good1,  " _
        '    & "COALESCE (dbo.TransCycleCountStocksDetail.Good2, 0) AS Good2, COALESCE (dbo.TransCycleCountStocksDetail.Good3, 0) AS Good3,  " _
        '    & "COALESCE (dbo.TransCycleCountStocksDetail.Good4, 0) AS Good4, COALESCE (dbo.TransCycleCountStocksDetail.Bad1, 0) AS Bad1,  " _
        '    & "COALESCE (dbo.TransCycleCountStocksDetail.Bad2, 0) AS Bad2, COALESCE (dbo.TransCycleCountStocksDetail.Bad3, 0) AS Bad3,  " _
        '    & "COALESCE (dbo.TransCycleCountStocksDetail.Bad4, 0) AS Bad4, dbo.TransCycleCountStocksDetail.BadReason FROM dbo.TransCycleCountStocks INNER JOIN " _
        '    & "dbo.LibraryUnitMeasures ON dbo.TransCycleCountStocks.UnitMeasureID1 = dbo.LibraryUnitMeasures.ID LEFT OUTER JOIN  " _
        '    & "dbo.LibraryUnitMeasures LibraryUnitMeasures_3 ON dbo.TransCycleCountStocks.UnitMeasureID4 = LibraryUnitMeasures_3.ID LEFT OUTER JOIN  " _
        '    & "dbo.LibraryUnitMeasures LibraryUnitMeasures_2 ON dbo.TransCycleCountStocks.UnitMeasureID3 = LibraryUnitMeasures_2.ID LEFT OUTER JOIN  " _
        '    & "dbo.LibraryUnitMeasures LibraryUnitMeasures_1 ON dbo.TransCycleCountStocks.UnitMeasureID2 = LibraryUnitMeasures_1.ID LEFT OUTER JOIN  " _
        '    & "dbo.TransCycleCountStocksDetail ON dbo.TransCycleCountStocks.StockID = dbo.TransCycleCountStocksDetail.StockID  " _
        '    & " WHERE TransCycleCountStocks.StockID = " & ReadyString(WebSession.Instance.skuID)

        pSql = "SELECT DISTINCT " &
                "dbo.TransCycleCountStocks.UnitMeasureID1, dbo.LibraryUnitMeasures.UnitMeasure AS UnitMeasure1, dbo.TransCycleCountStocks.UnitMeasureID2, " &
                "LibraryUnitMeasures_1.UnitMeasure AS UnitMeasure2, dbo.TransCycleCountStocks.UnitMeasureID3, LibraryUnitMeasures_2.UnitMeasure AS UnitMeasure3,  " &
                "dbo.TransCycleCountStocks.UnitMeasureID4, LibraryUnitMeasures_3.UnitMeasure AS UnitMeasure4, dbo.LibraryStockMeasurements.Quantity AS UnitMeasure1Qty,  " &
                "LibraryStockMeasurements_1.Quantity AS UnitMeasure2Qty, LibraryStockMeasurements_2.Quantity AS UnitMeasure3Qty,  " &
                "LibraryStockMeasurements_3.Quantity AS UnitMeasure4Qty " &
                "FROM dbo.TransCycleCountStocks INNER JOIN " &
                "dbo.LibraryUnitMeasures ON dbo.TransCycleCountStocks.UnitMeasureID1 = dbo.LibraryUnitMeasures.ID INNER JOIN " &
                "dbo.LibraryStockMeasurements ON dbo.TransCycleCountStocks.StockID = dbo.LibraryStockMeasurements.StockID AND  " &
                "dbo.LibraryUnitMeasures.ID = dbo.LibraryStockMeasurements.UnitMeasureID INNER JOIN " &
                "dbo.LibraryStockMeasurements AS LibraryStockMeasurements_1 ON dbo.TransCycleCountStocks.StockID = LibraryStockMeasurements_1.StockID INNER JOIN " &
                "dbo.LibraryStockMeasurements AS LibraryStockMeasurements_2 ON dbo.TransCycleCountStocks.StockID = LibraryStockMeasurements_2.StockID INNER JOIN " &
                "dbo.LibraryStockMeasurements AS LibraryStockMeasurements_3 ON dbo.TransCycleCountStocks.StockID = LibraryStockMeasurements_3.StockID INNER JOIN " &
                "dbo.LibraryUnitMeasures AS LibraryUnitMeasures_1 ON LibraryStockMeasurements_1.UnitMeasureID = LibraryUnitMeasures_1.ID AND  " &
                "dbo.TransCycleCountStocks.UnitMeasureID2 = LibraryUnitMeasures_1.ID INNER JOIN " &
                "dbo.LibraryUnitMeasures AS LibraryUnitMeasures_3 ON LibraryStockMeasurements_3.UnitMeasureID = LibraryUnitMeasures_3.ID AND  " &
                "dbo.TransCycleCountStocks.UnitMeasureID4 = LibraryUnitMeasures_3.ID INNER JOIN " &
                "dbo.LibraryUnitMeasures AS LibraryUnitMeasures_2 ON LibraryStockMeasurements_2.UnitMeasureID = LibraryUnitMeasures_2.ID AND  " &
                "dbo.TransCycleCountStocks.UnitMeasureID3 = LibraryUnitMeasures_2.ID " &
                "WHERE dbo.TransCycleCountStocks.StockID = " & ReadyString(WebSession.Instance.skuID)

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)

    End Function

End Class

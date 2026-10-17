Imports System.Threading
Imports C1.C1Zip

Public Class ErpToMobizComparerDirect
    Public Shared OrgID As String = "9D4B1500-13C0-49F9-A8E5-A16C13EC899E"
    Public SourceZipFilename As String
    Dim mProData As ProData
    Dim mBusiness As Business
    Dim mLogFolder As String
    Dim mCriticalFolder As String
    Dim mFailFolder As String
    Dim mPassFolder As String
    Shared ReadOnly mSemaphore As New SemaphoreSlim(1)

    Private ReadOnly Property LogFolder As String
        Get

            If mLogFolder Is Nothing Then
                mLogFolder = Common.ServerPath & "temp\logs\ErpToMobizComparerDirect\" & DateTimeStamp() & "\"
            End If

            Return mLogFolder

        End Get
    End Property

    Private ReadOnly Property PassFolder As String
        Get

            If mPassFolder Is Nothing Then
                mPassFolder = LogFolder & "Pass\"
            End If

            Return mPassFolder

        End Get
    End Property

    Private ReadOnly Property FailFolder As String
        Get

            If mFailFolder Is Nothing Then
                mFailFolder = LogFolder & "Fail\"
            End If

            Return mFailFolder

        End Get
    End Property

    Private ReadOnly Property CriticalFolder As String
        Get

            If mCriticalFolder Is Nothing Then
                mCriticalFolder = LogFolder & "Critical\"
            End If

            Return mCriticalFolder

        End Get
    End Property

    Public Function Process() As String

        If mSemaphore.Wait(2000) = False Then
            Return "Another export process is in progress. ref: vb7P4UPWZYX5!"
        End If

        Try
            AppendToLog("ref: vb9654YR5V4X! connecting to database", LogFolder, "Info")
            If ConnectToDatabase() = False Then Return "error"

            AppendToLog("ref: vbURQ4VNZHZB! decompressing", LogFolder, "Info")
            mProData.CommandTimeout = 10 * 60
            CompareProductsAndUnits()
            CompareProductsAndPrices()
            CompareVendorPrices()
            AppendToLog("ref: vbBG72Z2M6CL! done", LogFolder, "Info")
            Return "ok - " & DateTimeStamp()
        Catch ex As Exception
            LogToFile(CriticalFolder, "", ex)
            Return "error"
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Private Function ConnectToDatabase() As Boolean

        Dim pDR As DataRow

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=Common.ConnectionInfo.Database,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        mBusiness = New Business(mProData)
        pDR = mBusiness.LibraryOrganization.ByID(OrgID)
        If pDR Is Nothing Then
            'AddError("OrgID, " & OrgID & ", does not exist in the database.")
            Return False
        End If

        If CStr2(pDR("Database")).Trim() = "" Then
            'AddError("OrgID, " & OrgID & ", is not a subscriber.")
            Return False
        End If

        Try
            mProData.ExecuteSQL("USE " + EscapeSqlName(pDR("Database")))
        Catch ex As Exception
            'AddError("An error occured while connecting to database, " & pDR("Database") & ", for OrgID, " & OrgID & ".")
            Return False
        End Try

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=pDR("Database"),
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        mBusiness = New Business(mProData)

        Return True

    End Function

    Private Sub CompareProductsAndPrices()

        Dim pDtDuc = GetDucProductPrices()
        Dim pDtMobiz = GetMobizProductPrices()
        Dim pSwInterval = Stopwatch.StartNew()
        Dim pSwTotal = Stopwatch.StartNew()
        Dim pRowIndex As Integer

        For Each pDrDuc As DataRow In pDtDuc.Rows
            pRowIndex += 1
            If pSwInterval.ElapsedMilliseconds >= 5000 Then
                Dim pMsLeft = pSwTotal.ElapsedMilliseconds / pRowIndex
                pMsLeft *= pDtDuc.Rows.Count - pRowIndex
                AppendToLog("ref: vbVQNULM6XQH! Row " & Format(pRowIndex, cGlobal.FormatInteger) & " of " & Format(pDtDuc.Rows.Count, cGlobal.FormatInteger) & ". Time left: " & FormatDurationBySeconds(pMsLeft / 1000), LogFolder, "Info")
                pSwInterval.Restart()
            End If
            Dim pSql = "ProductID = " & ReadyString(pDrDuc("ProductID")) & vbCrLf &
                " AND PriceLevelID = " & ReadyString(pDrDuc("PriceLevelID")) & vbCrLf

            'Dim pDvMobiz = New DataView(pDtMobiz, pSql, "", DataViewRowState.CurrentRows)
            Dim pDvMobiz = pDtMobiz.Select(pSql)
            If pDvMobiz.Count = 0 Then
                AppendToLog(pSql & " does not exist in MobizLibraryPIMProductPriceLevel", FailFolder, "Product Price")
                ErpToMobizExporter.AddExportableProductID(CStr2(pDrDuc("ProductID")))
                Continue For
            ElseIf pDvMobiz.Count > 1 Then
                AppendToLog(pSql & " is present in " & pDvMobiz.Count & " rows in MobizLibraryPIMProductPriceLevel", FailFolder, "Product Price")
            End If

            Dim pNotEqual = False
            Dim pDrMobiz = pDvMobiz(0)
            If IsNumeric(pDrDuc("ProductNo")) _
                AndAlso IsNumeric(pDrMobiz("ProductNo")) _
                AndAlso CDec2(pDrDuc("ProductNo")) = CDec2(pDrMobiz("ProductNo")) Then
                ' considered equal
            Else
                If CompareRow(pSql, pDrDuc, pDrMobiz, "ProductNo", "MobizLibraryPIMProductPriceLevel") = False Then
                    pNotEqual = True
                End If
            End If

            If CompareRow(pSql, pDrDuc, pDrMobiz, "Price", "MobizLibraryPIMProductPriceLevel") = False Then
                pNotEqual = True
            End If
            If CompareRow(pSql, pDrDuc, pDrMobiz, "DeactivateDiscount", "MobizLibraryPIMProductPriceLevel") = False Then
                pNotEqual = True
            End If
            'CompareRow(pDrDuc, pDrMobiz, "ProductLastUpdated", "MobizLibraryPIMProductPriceLevel")
            'CompareRow(pDrDuc, pDrMobiz, "PriceLastUpdated", "MobizLibraryPIMProductPriceLevel")

            If pNotEqual Then
                ErpToMobizExporter.AddExportableProductID(CStr2(pDrDuc("ProductID")))
            End If
        Next pDrDuc

    End Sub

    Private Function GetDucProductPrices() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        AppendToLog("ref: vb3YYMU7NMJ9! start GetDucProductPrices", LogFolder, "Info")
        pSql = "  SELECT LibraryStockPriceLevels.ID" & vbCrLf &
            "  ,LibraryStockPriceLevels.StockID AS ProductID" & vbCrLf &
            "  ,LibraryStockPriceLevels.PriceLevelID" & vbCrLf &
            "  ,LibraryStockPriceLevels.Price" & vbCrLf &
            "  ,LibraryStockPriceLevels.DeactivateDiscount" & vbCrLf &
            "  ,LibraryStockPriceLevels.LastUpdated AS PriceLastUpdated" & vbCrLf &
            "  ,LibraryStockPriceLevels.UserID" & vbCrLf &
            "  ,LibraryStockInformation.StockNumber AS ProductNo" & vbCrLf &
            "  ,LibraryStockInformation.LastUpdated AS ProductLastUpdated" & vbCrLf &
            "  ,(SELECT MAX(AnyDate) FROM (VALUES (LibraryStockPriceLevels.LastUpdated), (LibraryStockInformation.LastUpdated)) AS AllDates(AnyDate)) AS LastUpdated" & vbCrLf &
            "  FROM wholesale.dbo.LibraryStockPriceLevels LibraryStockPriceLevels" & vbCrLf &
            "  JOIN wholesale.dbo.LibraryStockInformation LibraryStockInformation ON LibraryStockInformation.ID = LibraryStockPriceLevels.StockID" & vbCrLf &
            " ORDER BY LibraryStockInformation.LastUpdated"
        pDT = mProData.Execute(pSql)
        AppendToLog("ref: vbTT74GYJFHF! end GetDucProductPrices. Records " & Format(pDT.Rows.Count, cGlobal.FormatInteger), LogFolder, "Info")

        Return pDT

    End Function

    Private Function GetMobizProductPrices() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        AppendToLog("ref: vbG9NFXKGMMZ! start GetMobizProductPrices", LogFolder, "Info")
        pSql = " -- GetMobizProductPrices" & vbCrLf &
            " SELECT MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.DeactivateDiscount" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.LastUpdated AS PriceLastUpdated" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.LastUpdated AS ProductLastUpdated" & vbCrLf &
            "  ,(SELECT MAX(AnyDate) FROM (VALUES (MobizLibraryPIMProductPriceLevel.LastUpdated), (MobizLibraryPIMProduct.LastUpdated)) AS AllDates(AnyDate)) AS LastUpdated" & vbCrLf &
            " FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductPriceLevel.ProductID"
        pDT = mProData.Execute(pSql)
        AppendToLog("ref: vbTE8VG4FSKC! end GetMobizProductPrices. Records " & Format(pDT.Rows.Count, cGlobal.FormatInteger), LogFolder, "Info")

        Return pDT

    End Function

    Private Sub CompareProductsAndUnits()

        Dim pDtDuc = GetDucProductUnitMeasures()
        Dim pDtMobiz = GetMobizProductUnitMeasures()
        Dim pSwInterval = Stopwatch.StartNew()
        Dim pSwTotal = Stopwatch.StartNew()
        Dim pRowIndex As Integer

        For Each pDrDuc As DataRow In pDtDuc.Rows
            pRowIndex += 1
            If pSwInterval.ElapsedMilliseconds >= 5000 Then
                Dim pMsLeft = pSwTotal.ElapsedMilliseconds / pRowIndex
                pMsLeft *= pDtDuc.Rows.Count - pRowIndex
                AppendToLog("ref: vbYPNEBVX5RH! Row " & Format(pRowIndex, cGlobal.FormatInteger) & " of " & Format(pDtDuc.Rows.Count, cGlobal.FormatInteger) & ". Time left: " & FormatDurationBySeconds(pMsLeft / 1000), LogFolder, "Info")
                pSwInterval.Restart()
            End If
            Dim pSql = "ProductID = " & ReadyString(pDrDuc("ProductID")) & vbCrLf &
                " AND UnitMeasure = " & ReadyString(pDrDuc("UnitMeasure")) & vbCrLf

            If ReadyGuid(pDrDuc("ProductID")) = ReadyGuid("8C5CA746-1410-4E1C-8AE3-D64235E58C7C") Then
                Debug.WriteLine("vb24SAR6GZKB!")
            End If
            'Dim pDvMobiz = New DataView(pDtMobiz, pSql, "", DataViewRowState.CurrentRows)
            Dim pDvMobiz = pDtMobiz.Select(pSql)
            If pDvMobiz.Count = 0 Then
                AppendToLog(pSql & " does not exist in MobizLibraryPIMProductMeasurement", FailFolder, "Product Unit")
                ErpToMobizExporter.AddExportableProductID(CStr2(pDrDuc("ProductID")))
                Continue For
            ElseIf pDvMobiz.Count > 1 Then
                AppendToLog(pSql & " is present in " & pDvMobiz.Count & " rows in MobizLibraryPIMProductMeasurement", FailFolder, "Product Unit")
            End If

            Dim pNotEqual = False
            For Each pDrMobiz In pDvMobiz
                pNotEqual = False
                If CompareRow(pSql, pDrDuc, pDrMobiz, "Quantity", "MobizLibraryPIMProductMeasurement") = False Then
                    pNotEqual = True
                End If
                If CompareRow(pSql, pDrDuc, pDrMobiz, "Discount", "MobizLibraryPIMProductMeasurement") = False Then
                    pNotEqual = True
                End If
                If pNotEqual = False Then
                    Exit For
                End If
            Next

            If pNotEqual Then
                ErpToMobizExporter.AddExportableProductID(CStr2(pDrDuc("ProductID")))
            End If
        Next pDrDuc

    End Sub

    Private Function GetDucProductUnitMeasures() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        AppendToLog("ref: vbUBBCN5YL8U! start GetDucProductUnitMeasures", LogFolder, "Info")
        pSql = " SELECT LibraryStockMeasurements.ID" & vbCrLf &
            ",LibraryStockMeasurements.StockID AS ProductID" & vbCrLf &
            ",LibraryUnitMeasures.UnitMeasure" & vbCrLf &
            " ,LibraryStockMeasurements.Quantity" & vbCrLf &
            " ,LibraryStockMeasurements.Discount" & vbCrLf &
            " FROM wholesale.dbo.LibraryStockMeasurements LibraryStockMeasurements" & vbCrLf &
            " JOIN wholesale.dbo.LibraryUnitMeasures LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & vbCrLf &
            " JOIN wholesale.dbo.LibraryStockInformation LibraryStockInformation ON LibraryStockInformation.ID = LibraryStockMeasurements.StockID" & vbCrLf &
            " ORDER BY LibraryUnitMeasures.LastUpdated"
        pDT = mProData.Execute(pSql)
        AppendToLog("ref: vbGW6SZD5DL2! end GetDucProductUnitMeasures. Records " & Format(pDT.Rows.Count, cGlobal.FormatInteger), LogFolder, "Info")

        Return pDT

    End Function

    Private Function GetMobizProductUnitMeasures() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        AppendToLog("ref: vbWNGRR3TTA7! start GetMobizProductUnitMeasures", LogFolder, "Info")
        pSql = " SELECT MobizLibraryPIMProductMeasurement.ID" & vbCrLf &
            " , MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
            " , MobizLibraryCountriesLanguageLabel.Label AS [UnitMeasure]" & vbCrLf &
            " , MobizLibraryPIMProductMeasurement.Quantity AS [Quantity]" & vbCrLf &
            " , MobizLibraryPIMProductMeasurement.Discount AS [Discount]" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
            " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
            " ORDER BY MobizLibraryPIMProductMeasurement.LastUpdated"
        pDT = mProData.Execute(pSql)
        AppendToLog("ref: vb2TRBJV5ZKK! end GetMobizProductUnitMeasures. Records " & Format(pDT.Rows.Count, cGlobal.FormatInteger), LogFolder, "Info")

        Return pDT

    End Function

    Private Sub CompareVendorPrices()

        Dim pDtDuc = GetDucVendorPrices()
        Dim pDtMobiz = GetMobizVendorPrices()
        Dim pSwInterval = Stopwatch.StartNew()
        Dim pSwTotal = Stopwatch.StartNew()
        Dim pRowIndex As Integer

        For Each pDrDuc As DataRow In pDtDuc.Rows
            pRowIndex += 1
            If pSwInterval.ElapsedMilliseconds >= 5000 Then
                Dim pMsLeft = pSwTotal.ElapsedMilliseconds / pRowIndex
                pMsLeft *= pDtDuc.Rows.Count - pRowIndex
                AppendToLog("ref: vb8Z56SC78XW! Row " & Format(pRowIndex, cGlobal.FormatInteger) & " of " & Format(pDtDuc.Rows.Count, cGlobal.FormatInteger) & ". Time left: " & FormatDurationBySeconds(pMsLeft / 1000), LogFolder, "Info")
                pSwInterval.Restart()
            End If
            Dim pSql = "ProductID = " & ReadyString(pDrDuc("ProductID")) & vbCrLf &
                " AND VendorID = " & ReadyString(pDrDuc("VendorID")) & vbCrLf

            If ReadyGuid(pDrDuc("ProductID")) = ReadyGuid("8C5CA746-1410-4E1C-8AE3-D64235E58C7C") Then
                Debug.WriteLine("vb24SAR6GZKB!")
            End If
            'Dim pDvMobiz = New DataView(pDtMobiz, pSql, "", DataViewRowState.CurrentRows)
            Dim pDvMobiz = pDtMobiz.Select(pSql)
            If pDvMobiz.Count = 0 Then
                AppendToLog(pSql & " does not exist in LibraryVendorPrices", FailFolder, "Vendor Price")
                ErpToMobizExporter.AddExportableVendorPriceProductID(CStr2(pDrDuc("ProductID")))
                Continue For
            ElseIf pDvMobiz.Count > 1 Then
                AppendToLog(pSql & " is present in " & pDvMobiz.Count & " rows in LibraryVendorPrices", FailFolder, "Vendor Price")
            End If

            Dim pNotEqual = False
            For Each pDrMobiz In pDvMobiz
                pNotEqual = False
                If CompareRow(pSql, pDrDuc, pDrMobiz, "Price", "LibraryVendorPrices") = False Then
                    pNotEqual = True
                End If
                If CompareRow(pSql, pDrDuc, pDrMobiz, "LastUpdated", "LibraryVendorPrices") = False Then
                    pNotEqual = True
                End If
                If pNotEqual = False Then
                    Exit For
                End If
            Next

            If pNotEqual Then
                ErpToMobizExporter.AddExportableVendorPriceProductID(CStr2(pDrDuc("ProductID")))
            End If
        Next pDrDuc

    End Sub

    Private Function GetDucVendorPrices() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "  with cte as (" & vbCrLf &
            "  SELECT LibrarySupplierStocks.SupplierID AS VendorID" & vbCrLf &
            "  ,LibrarySupplierStocks.StockID AS ProductID" & vbCrLf &
            "  ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf &
            "  ,LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.ActualQuantity AS Price" & vbCrLf &
            "  FROM wholesale.dbo.LibrarySupplierStocks LibrarySupplierStocks" & vbCrLf &
            "  JOIN wholesale.dbo.LibrarySupplierStockPrices LibrarySupplierStockPrices ON LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf &
            "  )" & vbCrLf &
            " SELECT * FROM (" & vbCrLf &
            "  select TOP 100 PERCENT cte.VendorID" & vbCrLf &
            "  ,cte.ProductID" & vbCrLf &
            "  ,tbl.LastUpdated" & vbCrLf &
            "  ,tbl.Price" & vbCrLf &
            "  from cte" & vbCrLf &
            "  CROSS APPLY (" & vbCrLf &
            "  	SELECT TOP 1 cte2.LastUpdated" & vbCrLf &
            "      ,cte2.Price" & vbCrLf &
            "      FROM cte cte2" & vbCrLf &
            "      WHERE cte.ProductID = cte2.ProductID" & vbCrLf &
            "      AND cte.VendorID = cte2.VendorID" & vbCrLf &
            "      ORDER BY cte2.LastUpdated DESC" & vbCrLf &
            "  ) tbl" & vbCrLf
        pSql &= "  GROUP BY cte.VendorID" & vbCrLf &
            "  ,cte.ProductID" & vbCrLf &
            "  ,tbl.LastUpdated" & vbCrLf &
            "  ,tbl.Price" & vbCrLf
        pSql &= " ORDER BY tbl.LastUpdated DESC" & vbCrLf &
            ", cte.VendorID, cte.ProductID" & vbCrLf &
            "  ) t" & vbCrLf
        mProData.CommandTimeout = 10 * 60
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizVendorPrices() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryVendorPrices.ID" & vbCrLf &
            " , LibraryVendorPrices.Price" & vbCrLf &
            " , LibraryVendorPrices.LastUpdated" & vbCrLf &
            " , LibraryVendorPrices.ProductID" & vbCrLf &
            " , LibraryVendorPrices.VendorID" & vbCrLf &
            " FROM LibraryVendorPrices" & vbCrLf &
            " ORDER BY LibraryVendorPrices.LastUpdated"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function CompareRow(vKey As String, vDrSource As DataRow, vDrDestination As DataRow, vFieldName As String, vTable As String) As Boolean

        Dim pSourceText As String
        Dim pKey As String
        Dim pSourceLastUpdated As String
        Dim pDestinationLastUpdated As String

        If String.IsNullOrWhiteSpace(vKey) = False Then
            pKey = vKey
        ElseIf vDrSource.Table.Columns.Contains("ID") Then
            pKey = "ID: " & ReadyGuid(vDrSource("ID"))
        ElseIf vDrDestination.Table.Columns.Contains("ID") Then
            pKey = "ID: " & ReadyGuid(vDrDestination("ID"))
        End If

        If vDrSource.Table.Columns.Contains("LastUpdated") Then
            If IsDate(vDrSource("LastUpdated")) Then
                pSourceLastUpdated = "Source LastUpdated: " & vDrSource("LastUpdated") & vbCrLf
            End If
        End If

        If vDrDestination.Table.Columns.Contains("LastUpdated") Then
            If IsDate(vDrDestination("LastUpdated")) Then
                pDestinationLastUpdated = "Destination LastUpdated: " & vDrDestination("LastUpdated") & vbCrLf
            End If
        End If

        Dim pMdt = MajorDataType(vDrDestination.Table.Columns(vFieldName).DataType)
        If pMdt = eMajorDataType.String Then
            If CStr2(vDrSource(vFieldName)) <> CStr2(vDrDestination(vFieldName)) Then
                AppendToLog(pKey & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & CStr2(vDrSource(vFieldName)) & vbCrLf &
                    pSourceLastUpdated &
                    "Destination value: " & CStr2(vDrDestination(vFieldName)) &
                    pDestinationLastUpdated, FailFolder, vTable)
                Return False
            End If
            pSourceText = CStr2(vDrSource(vFieldName))
        ElseIf pMdt = eMajorDataType.Numeric Then
            Dim pDecimalDifference = CDec2(vDrSource(vFieldName)) - CDec2(vDrDestination(vFieldName))
            pDecimalDifference = Math.Abs(pDecimalDifference)

            If pDecimalDifference >= 0.01 Then
                AppendToLog(pKey & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & CDec2(vDrSource(vFieldName)) & vbCrLf &
                    pSourceLastUpdated &
                    "Destination value: " & CDec2(vDrDestination(vFieldName)) & vbCrLf &
                    pDestinationLastUpdated, FailFolder, vTable)
                Return False
            End If
            pSourceText = CDec2(vDrSource(vFieldName))
        ElseIf pMdt = eMajorDataType.Date Then
            Dim pSourceDate = CDateNull(vDrSource(vFieldName))
            Dim pDestinationDate = CDateNull(vDrDestination(vFieldName))

            If pSourceDate Is Nothing _
                AndAlso pDestinationDate Is Nothing Then

                ' equal. do nothing here
                pSourceText = ""
            ElseIf pSourceDate Is Nothing _
                AndAlso pDestinationDate IsNot Nothing Then

                AppendToLog(pKey & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & vbCrLf &
                    pSourceLastUpdated &
                    "Destination value: " & Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    pDestinationLastUpdated, FailFolder, vTable)
                Return False
            ElseIf pSourceDate IsNot Nothing _
                AndAlso pDestinationDate Is Nothing Then

                AppendToLog(pKey & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & Format(vDrSource(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    pSourceLastUpdated &
                    "Destination value: " & vbCrLf &
                    pDestinationLastUpdated, FailFolder, vTable)
                Return False
            ElseIf pSourceDate.Value <> pDestinationDate.Value Then
                AppendToLog(pKey & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & Format(vDrSource(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    pSourceLastUpdated &
                    "Destination value: " & Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    pDestinationLastUpdated, FailFolder, vTable)
                Return False
            Else
                pSourceText = Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2)
            End If
        End If

        AppendToLog(pKey & "; same field value in " & vFieldName & ": " & pSourceText, PassFolder, vTable)
        Return True

    End Function

    Private Sub LogToFile(vFolder As String, vText As String, vEx As Exception)

        Dim pStr As String
        Dim pFilename As String

        Try
            pStr = "[" & DateTimeStamp() & "] "
            pStr &= vText & vbCrLf
            pStr &= GetStackTrace(False, vEx)

            pFilename = IO.Path.Combine(vFolder, Format(Now, "yyy_MM_dd") & ".txt")

            CreateFolder(pFilename)
            AppendToFile(pFilename, pStr)
        Catch ex As Exception
            Stop2()
        End Try

    End Sub
End Class

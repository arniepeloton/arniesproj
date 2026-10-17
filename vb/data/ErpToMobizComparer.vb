Imports C1.C1Zip

Public Class ErpToMobizComparer
    Public Shared OrgID As String = "9D4B1500-13C0-49F9-A8E5-A16C13EC899E"
    Public SourceZipFilename As String
    Dim mProData As ProData
    Dim mSqliteFilename As String
    Dim mSqlite As ProSqlite
    Dim mBusiness As Business
    Dim mLogFolder As String
    Dim mCriticalFolder As String
    Dim mFailFolder As String
    Dim mPassFolder As String
    Dim mImportResult As ImportResultClass
    Public Const LogFilename = "ErpToMobizImporter"

    Private ReadOnly Property LogFolder As String
        Get

            If mLogFolder Is Nothing Then
                mLogFolder = Common.ServerPath & "temp\logs\ErpToMobizComparer\" & IO.Path.GetFileNameWithoutExtension(SourceZipFilename) & "\"
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

    Public ReadOnly Property Status As String
        Get

            Return mImportResult.Status

        End Get
    End Property

    Public Sub Process()

        mImportResult = New ImportResultClass()
        Try
            mImportResult.Status = "connecting to database"
            AppendToLog("ref: vbPQPZ44CTUU! connecting to database", , LogFilename)
            If ConnectToDatabase() = False Then Return

            mImportResult.Status = "decompressing"
            AppendToLog("ref: vb82PJ6AFYAF! decompressing", , LogFilename)
            If Extract() = False Then Return

            CompareMobizLibraryPIMProduct()
            ComparePIMPriceLevel()
            'ImportMobizLibraryCities()
            CompareOrganizations()
            CompareVendorPrices()
            mSqlite.Dispose()

            mImportResult.Status = "validating rows"
            AppendToLog("ref: vb992U628N5D! validating rows", , LogFilename)
            Call New RowCountChecksumDuc(mSqliteFilename).Process()
            mSqlite.Dispose()
            mProData.Dispose()

            mImportResult.Status = "done"
            AppendToLog("ref: vbEG5AAASF85! done", , LogFilename)
        Catch ex As Exception
            LogToFile(CriticalFolder, mImportResult.Status, ex)
            mImportResult.Status = "error"
        End Try

    End Sub

    Private Function Extract() As Boolean

        Dim pZip As C1ZipFile

        mSqliteFilename = Common.ServerPath & "temp\ImportErpToMobiz\"
        pZip = New C1ZipFile(SourceZipFilename)
        mSqliteFilename &= pZip.Entries(0).FileName
        CreateFolder(mSqliteFilename)
        pZip.Entries(0).Extract(mSqliteFilename)
        mSqlite = New ProSqlite
        mSqlite.Open(mSqliteFilename)

        Return True

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

    Private Function GetUserID(vNumericID As Integer) As String

        Dim pSql As String

        pSql = "SELECT TOP 1 ID FROM MobizLibraryPeople Where DUCNumericID = " & vNumericID
        pSql = CStr2(mProData.ExecuteScalar(pSql))

        Return pSql

    End Function

    Private Function CompareRow(vDrSource As DataRow, vDrDestination As DataRow, vFieldName As String, vTable As String) As Boolean

        Dim pSourceText As String
        Dim pID As String

        If vDrSource.Table.Columns.Contains("ID") Then
            pID = ReadyGuid(vDrSource("ID"))
        ElseIf vDrDestination.Table.Columns.Contains("ID") Then
            pID = ReadyGuid(vDrDestination("ID"))
        End If
        Dim pMdt = MajorDataType(vDrDestination.Table.Columns(vFieldName).DataType)
        If pMdt = eMajorDataType.String Then
            If CStr2(vDrSource(vFieldName)) <> CStr2(vDrDestination(vFieldName)) Then
                AppendToLog("ID: " & pID & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & CStr2(vDrSource(vFieldName)) & vbCrLf &
                    "Destination value: " & CStr2(vDrDestination(vFieldName)), FailFolder, vTable)
                Return False
            End If
            pSourceText = CStr2(vDrSource(vFieldName))
        ElseIf pMdt = eMajorDataType.Numeric Then
            If CDec2(vDrSource(vFieldName)) <> CDec2(vDrDestination(vFieldName)) Then
                AppendToLog("ID: " & pID & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & CDec2(vDrSource(vFieldName)) & vbCrLf &
                    "Destination value: " & CDec2(vDrDestination(vFieldName)), FailFolder, vTable)
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

                AppendToLog("ID: " & pID & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & vbCrLf &
                    "Destination value: " & Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2), FailFolder, vTable)
                Return False
            ElseIf pSourceDate IsNot Nothing _
                AndAlso pDestinationDate Is Nothing Then

                AppendToLog("ID: " & pID & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & Format(vDrSource(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    "Destination value: ", FailFolder, vTable)
                Return False
            ElseIf pSourceDate.Value <> pDestinationDate.Value Then
                AppendToLog("ID: " & pID & "; different " & vFieldName & vbCrLf &
                    "Source value     : " & Format(vDrSource(vFieldName), cGlobal.FormatDateTimeSql2) & vbCrLf &
                    "Destination value: " & Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2), FailFolder, vTable)
                Return False
            Else
                pSourceText = Format(vDrDestination(vFieldName), cGlobal.FormatDateTimeSql2)
            End If
        End If

        AppendToLog("ID: " & pID & "; same field value in " & vFieldName & ": " & pSourceText, PassFolder, vTable)
        Return True

    End Function

    Private Function CompareMobizLibraryPIMProduct()

        Dim pDT As DataTable
        Dim pSql As String

        AppendToLog("ref: vbKC2DPN5U8H! CompareMobizLibraryPIMProduct", , LogFilename)

        pSql = "SELECT * FROM MobizLibraryPIMProduct"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then Return True

        For Each pDrSourceProduct As DataRow In pDT.Rows
            pSql = "SELECT *" & vbCrLf &
                " FROM MobizLibraryPIMProduct" & vbCrLf &
                " WHERE ID = " & ReadyString(pDrSourceProduct("ID"))
            Dim pDtDestinationProducts = mProData.Execute(pSql)
            If pDtDestinationProducts.Rows.Count = 0 Then
                AppendToLog("Missing product in destination: " & ReadyGuid(pDrSourceProduct("ID")), FailFolder, "Product")
                Continue For
            ElseIf pDtDestinationProducts.Rows.Count > 1 Then
                AppendToLog("Multiple product in destination: " & ReadyGuid(pDrSourceProduct("ID")), FailFolder, "Product")
                Continue For
            End If
            Dim pDrDestinationProduct = pDtDestinationProducts.Rows(0)
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "ProductNo", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "ProductName", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "Brand", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "ProductSearchString", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "IsActive", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "LastUpdated", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "ModelNo", "Product") = False Then Continue For
            If CompareRow(pDrSourceProduct, pDrDestinationProduct, "ProductFamilyID", "Product") = False Then Continue For

            pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM LibraryOrganizationPIM" & vbCrLf &
                " WHERE ProductID = " & ReadyString(pDrSourceProduct("ID"))
            Dim pCount = CInt2(mProData.ExecuteScalar(pSql))
            If pCount > 0 Then
                AppendToLog("ID: " & ReadyGuid(pDrSourceProduct("ID")) & "; exists in LibraryOrganizationPIM", PassFolder, "Product")
            Else
                AppendToLog("ID: " & ReadyGuid(pDrSourceProduct("ID")) & "; does not exist in LibraryOrganizationPIM", FailFolder, "Product")
                Continue For
            End If

            If CompareFeatures(CStr2(pDrSourceProduct("ID"))) = False Then
                Continue For
            End If
            If CompareUnitMeasures(CStr2(pDrSourceProduct("ID"))) = False Then
                Continue For
            End If
            If ComparePIMProductPriceLevel(CStr2(pDrSourceProduct("ID"))) = False Then
                Continue For
            End If
            AppendToLog("ID: " & ReadyGuid(pDrSourceProduct("ID")) & "; same fields values", PassFolder, "Product")
        Next pDrSourceProduct

    End Function

    Private Function CompareFeatures(vProductID As String) As Boolean

        Dim pSql = "SELECT *" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID)
        Dim pDrSource = mSqlite.ExecuteRow(pSql)
        If pDrSource Is Nothing Then
            Return True
        End If

        pSql = "SELECT *" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID)
        Dim pDrDestination = mProData.ExecuteRow(pSql)
        If pDrDestination Is Nothing Then
            AppendToLog("ID: " & ReadyGuid(vProductID) & "; does not exist in MobizLibraryPIMProductVariableFeatures001", FailFolder, "Product")
            Return False
        End If
        AppendToLog("ID: " & ReadyGuid(vProductID) & "; exists in MobizLibraryPIMProductVariableFeatures001", PassFolder, "Product")

        If CompareRow(pDrSource, pDrDestination, "C0020", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0056", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0057", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0058", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0059", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0060", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0061", "Product") = False Then Return False
        If CompareRow(pDrSource, pDrDestination, "C0063", "Product") = False Then Return False

        Return True

    End Function

    Private Function CompareUnitMeasures(vProductID As String) As Boolean

        Dim pSql = "SELECT * FROM MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(vProductID)
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; does not exist in MobizLibraryPIMProductMeasurement", FailFolder, "Product Measurement")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "ID = " & ReadyString(pDrSource("ID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; ID " & ReadyGuid(pDrSource("ID")) & " does not exist in MobizLibraryPIMProductMeasurement", FailFolder, "Product Measurement")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            If CompareRow(pDrSource, pDrDestination, "Quantity", "Product Measurement") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "Discount", "Product Measurement") = False Then Return False
            Dim pUnitMeasureID = ReadyGuid(mBusiness.LibraryStockMeasurements.GetUnitMeasureID(pDrSource("UnitMeasure")))
            If String.IsNullOrWhiteSpace(pUnitMeasureID) Then
                AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; ID " & ReadyGuid(pDrSource("ID")) & "; unit measure not in MobizLibraryPIMMeasure", FailFolder, "Product Measurement")
                Return False
            End If
            If pUnitMeasureID = ReadyGuid(pDrDestination("UnitMeasureID")) Then
                AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; ID " & ReadyGuid(pDrSource("ID")) & "; same UnitMeasureID", PassFolder, "Product Measurement")
            Else
                AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; ID " & ReadyGuid(pDrSource("ID")) & "; different UnitMeasureID", FailFolder, "Product Measurement")
                Return False
            End If
            If CompareUnitMeasuresBarcode(CStr2(pDrSource("ID"))) = False Then
                Return False
            End If
        Next pDrSource

        Return True

    End Function

    Private Function CompareUnitMeasuresBarcode(vStockMeasurementsID As String) As Boolean

        Dim pSql = "SELECT * FROM MobizLibraryPIMProductMeasurementBarcode WHERE StockMeasurementsID = " & ReadyString(vStockMeasurementsID)
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("vStockMeasurementsID: " & ReadyGuid(vStockMeasurementsID) & "; does not exist in MobizLibraryPIMProductMeasurementBarcode", FailFolder, "Product Measurement Barcode")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "ID = " & ReadyString(pDrSource("ID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("vStockMeasurementsID: " & ReadyGuid(vStockMeasurementsID) & "; ID " & ReadyGuid(pDrSource("ID")) & " does not exist in MobizLibraryPIMProductMeasurementBarcode", FailFolder, "Product Measurement")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            If CompareRow(pDrSource, pDrDestination, "Barcode", "Product Measurement Barcode") = False Then Return False
        Next pDrSource

        Return True

    End Function

    Private Function ComparePIMProductPriceLevel(vProductID As String) As Boolean

        Dim pSql = "SELECT * FROM MobizLibraryPIMProductPriceLevel WHERE ProductID = " & ReadyString(vProductID)
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; does not exist in MobizLibraryPIMProductPriceLevel", FailFolder, "Product Price")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "ID = " & ReadyString(pDrSource("ID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("ProductID: " & ReadyGuid(vProductID) & "; ID " & ReadyGuid(pDrSource("ID")) & " does not exist in MobizLibraryPIMProductPriceLevel", FailFolder, "Product Price")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            If CompareRow(pDrSource, pDrDestination, "PriceLevelID", "Product Price") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "Price", "Product Price") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "DeactivateDiscount", "Product Price") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "LastUpdated", "Product Price") = False Then Return False
        Next pDrSource

        Return True

    End Function

    Private Function ComparePIMPriceLevel() As Boolean

        Dim pSql = "SELECT * FROM MobizLibraryPIMPriceLevel"
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("No rows in MobizLibraryPIMPriceLevel", FailFolder, "Price Levels")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "ID = " & ReadyString(pDrSource("ID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("PriceLevelID: " & ReadyGuid(pDrSource("ID")) & " does not exist in MobizLibraryPIMPriceLevel", FailFolder, "Price Level")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            If CompareRow(pDrSource, pDrDestination, "PriceLevel", "Price Level") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "IsActive", "Price Level") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "LastUpdated", "Price Level") = False Then Return False
        Next pDrSource

        AppendToLog("MobizLibraryPIMPriceLevel", PassFolder, "Price Level")

        Return True

    End Function

    Private Function ImportMobizLibraryCities() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String
        Dim pRecords As Integer
        Dim pRowIndex As Integer
        Dim pDrMobizCity As DataRow

        AppendToLog("ref: vbURZMTPUAU4! ImportMobizLibraryCities", , LogFilename)
        pSql = "SELECT * FROM MobizLibraryCities"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then Return True

        For Each pDR As DataRow In pDT.Rows
            Try
                mImportResult.Status = "Importing " & pRowIndex + 1 & " of " & pDT.Rows.Count & " cities"
                pRowIndex += 1
                pDrMobizCity = MobizCityByName(CStr2(pDR("Name")))
                If pDrMobizCity IsNot Nothing Then
                    If ReadyGuid(pDrMobizCity("ID")) <> ReadyGuid(pDR("ID")) Then
                        UpdateMobizFowstCityID(CStr2(pDR("ID")), CStr2(pDrMobizCity("ID")))
                    End If
                    Continue For
                End If

                pIU = New InsertUpdate("MobizLibraryCities")
                pIU("Name") = ReadyString(pDR("Name"))
                pIU("Zip") = CInt2(pDR("Zip"))
                pIU("IsActive") = CInt2(pDR("IsActive"))
                pIU("LastUpdated") = ReadyString(pDR("LastUpdated"))

                pUserID = "NULL"
                If IsNumeric(pDR("UserID")) Then
                    pUserID = GetUserID(pDR("UserID"))
                    If pUserID = "" Then
                        pUserID = "NULL"
                    Else
                        pUserID = ReadyString(pUserID)
                    End If
                End If
                pIU("UserID") = pUserID
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                pRecords = mProData.ExecuteSQL(pSql, True)
                If pRecords = 0 Then
                    pIU("ID") = ReadyString(pDR("ID"))
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    'LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    'LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try
        Next pDR

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryOrganization')"
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function MobizCityByName(vName As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryCities" & vbCrLf &
            " WHERE Name = " & ReadyString(vName)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function UpdateMobizFowstCityID(vOldCityID As String, vNewCityID As String) As Integer

        Dim pRecordsAffected As Integer
        Dim pSql As String

        pSql = "UPDATE MobizLibraryOrganizationFOWST" & vbCrLf &
            " SET FowstCityID = " & ReadyString(vNewCityID) & vbCrLf &
            " WHERE FowstCityID = " & ReadyString(vOldCityID)
        pRecordsAffected = mProData.ExecuteSQL(pSql, True)

        Return pRecordsAffected

    End Function

    Private Function CompareOrganizations() As Boolean

        Dim pCount As Integer
        Dim pSql = "SELECT * FROM MobizLibraryOrganization"
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("No rows in MobizLibraryOrganization", FailFolder, "Organization")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "ID = " & ReadyString(pDrSource("ID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("OrgID: " & ReadyGuid(pDrSource("ID")) & " does not exist in MobizLibraryOrganization", FailFolder, "Organization")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            If CompareRow(pDrSource, pDrDestination, "CompanyName", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "TinNumber", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "IsFranchisor", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "RegistrationType", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "RegistrationNo", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "RegistrationDate", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "OfficeAddressBuildingNo", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "OfficeAddressStreet", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "OfficeAddressCityID", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "OfficeAddressCountryID", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "BusinessAddressBuildingNo", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "BusinessAddressStreet", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "BusinessAddressCityID", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "BusinessAddressCountryID", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "Purpose", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "BusinessScope", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "NoOfRetailOutlet", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "NoOfOffice", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "NoOfWarehouse", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "NoOfEmployees", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "IsHasHoldingCompany", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "HoldingCompanyID", "Organization") = False Then Return False
            'If CompareRow(pDrSource, pDrDestination, "IsOrganization", "Organization") = False Then Return False ' dont compare this
            If CompareRow(pDrSource, pDrDestination, "PersonTitleID", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonSurname", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonFirstName", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonMiddleName", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonSuffixName", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonDateOfBirth", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonMotherMaidenSurname", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonMotherMaidenFirstName", "Organization") = False Then Return False
            If CompareRow(pDrSource, pDrDestination, "PersonMotherMaidenMiddleName", "Organization") = False Then Return False
            'If CompareRow(pDrSource, pDrDestination, "", "Organization") = False Then Return False

            If CBool2(pDrSource("IsCustomer")) Then
                If IsNumeric(pDrSource("Term")) Then
                    pSql = " SELECT COUNT(*)" & vbCrLf &
                        " FROM LibraryOrganizationCustomer" & vbCrLf &
                        " JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
                        " WHERE LibraryPaymentTerms.Days = " & CInt2(pDrSource("Term")) & vbCrLf &
                        " AND LibraryOrganizationCustomer.OrganizationID = " & ReadyString(pDrSource("ID"))
                    pCount = CInt2(mProData.ExecuteScalar(pSql))
                    If pCount = 0 Then
                        AppendToLog("OrgID: " & ReadyGuid(pDrSource("ID")) & " customer term fail", FailFolder, "Organization")
                    End If
                End If
            Else
                If IsNumeric(pDrSource("Term")) Then
                    pSql = " SELECT COUNT(*)" & vbCrLf &
                        " FROM LibraryOrganizationSupplier" & vbCrLf &
                        " JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf &
                        " WHERE LibraryPaymentTerms.Days = " & CInt2(pDrSource("Term")) & vbCrLf &
                        " AND LibraryOrganizationSupplier.SupplierID= " & ReadyString(pDrSource("ID"))
                    pCount = CInt2(mProData.ExecuteScalar(pSql))
                    If pCount = 0 Then
                        AppendToLog("OrgID: " & ReadyGuid(pDrSource("ID")) & " supplier term fail", FailFolder, "Organization")
                    End If
                End If
            End If
            
            'Try
            '    mImportResult.Status = "Importing " & pRowIndex + 1 & " of " & pDT.Rows.Count & " organizations"
            '    pIU = New InsertUpdate("MobizLibraryOrganization")


            '    If pDR("IsCustomer") Then
            '        CreateLibraryOrganizationCustomer(pDR)
            '        ImportLibraryOrganizationPriceLevel(CStr2(pDR("ID")))
            '        ImportLibraryCustomerTerm(pDR)
            '    End If
            '    If pDR("IsSupplier") Then
            '        CreateLibraryOrganizationSupplier(pDR)
            '        ImportLibrarySupplierTerm(pDR)
            '    End If
            '    ImportMobizLibraryOrganizationFOWST(CStr2(pDR("ID")))
            '    ImportMobizLibraryOrganizationContactPerson(CStr2(pDR("ID")))
            'Catch ex As Exception
            '    If TypeOf ex Is ProSqlException Then
            '        'LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
            '    Else
            '        'LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
            '    End If
            'End Try
            'pRowIndex += 1
        Next pDrSource

        AppendToLog("Passed Organization", PassFolder, "Organization")

        Return True

    End Function

    Private Function CompareVendorPrices() As Boolean

        Dim pSql = "SELECT * FROM LibraryVendorPrices"
        Dim pDtSource = mSqlite.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            Return True
        End If

        Dim pDtDestination = mProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            AppendToLog("No rows in LibraryVendorPrices", FailFolder, "Vendor Prices")
            Return False
        End If

        For Each pDrSource As DataRow In pDtSource.Rows
            Dim pDvDestination = New DataView(pDtDestination, "VendorID = " & ReadyString(pDrSource("VendorID")) &
                " AND ProductID = " & ReadyString(pDrSource("ProductID")), "", DataViewRowState.CurrentRows)
            If pDvDestination.Count = 0 Then
                AppendToLog("VendorID: " & ReadyGuid(pDrSource("VendorID")) & "; ProductID: " & ReadyGuid(pDrSource("ProductID")) & " does not exist in LibraryVendorPrices", FailFolder, "Vendor Prices")
                Return False
            End If
            Dim pDrDestination = pDvDestination(0).Row
            Dim pUnitQty = CDec2(pDrSource("ActualQuantity"))
            If pUnitQty < 0.001 Then
                pUnitQty = 1
            End If
            Dim pUnitPrice = CDec2(pDrSource("Price")) / pUnitQty
            Dim pAbsDiff = Math.Abs(pUnitPrice - CDec2(pDrDestination("Price")))
            If pAbsDiff > 0.001 Then
                AppendToLog("VendorID: " & ReadyGuid(pDrSource("VendorID")) & "; ProductID: " & ReadyGuid(pDrSource("ProductID")) & "; price difference in LibraryVendorPrices", FailFolder, "Vendor Prices")
            Else
                AppendToLog("VendorID: " & ReadyGuid(pDrSource("VendorID")) & "; ProductID: " & ReadyGuid(pDrSource("ProductID")) & "; same price in LibraryVendorPrices", PassFolder, "Vendor Prices")
            End If
        Next pDrSource

        AppendToLog("Passed Vendor Prices", PassFolder, "Vendor Prices")

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

    Public Class ImportResultClass
        Public ImportedMobizLibraryPIMProduct As Integer
        Public MobizLibraryPIMProductError As String
        Public MobizLibraryPIMProductErrorSql As String

        Public ImportedMobizLibraryPIMPriceLevel As Integer
        Public MobizLibraryPIMPriceLevelError As String
        Public MobizLibraryPIMPriceLevelErrorSql As String

        Public ImportedMobizLibraryOrganization As Integer
        Public MobizLibraryOrganizationError As String
        Public MobizLibraryOrganizationErrorSql As String

        Public Status As String
    End Class
End Class

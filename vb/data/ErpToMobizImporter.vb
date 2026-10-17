Imports C1.C1Zip

Public Class ErpToMobizImporter
    Public Shared OrgID As String = "9D4B1500-13C0-49F9-A8E5-A16C13EC899E"
    Public SourceZipFilename As String
    Dim mProData As ProData
    Dim mSqliteFilename As String
    Dim mSqlite As ProSqlite
    Dim mBusiness As Business
    Dim mLogFolder As String
    Dim mCriticalFolder As String
    Dim mBypassedErrorFolder As String
    Dim mImportResult As ImportResultClass
    Public Const LogFilename = "ErpToMobizImporter"

    Private ReadOnly Property LogFolder As String
        Get

            If mLogFolder Is Nothing Then
                mLogFolder = Common.ServerPath & "temp\logs\ErpToMobizImporter\"
            End If

            Return mLogFolder

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

    Private ReadOnly Property BypassedErrorFolder As String
        Get

            If mBypassedErrorFolder Is Nothing Then
                mBypassedErrorFolder = LogFolder & "ByPassedError\"
            End If

            Return mBypassedErrorFolder

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

            ' commented for debugging
            ImportMobizLibraryPIMProduct()
            ImportMobizLibraryPIMPriceLevel()
            ImportMobizLibraryCities()
            ImportMobizLibrarySalesChannels()
            ImportMobizLibraryOrganization()
            ImportLibraryVendorPrices()
            ImportCustomerSalesReps()
            mProData.Dispose()

            mImportResult.Status = "validating rows"
            AppendToLog("ref: vb992U628N5D! validating rows", , LogFilename)
            Call New RowCountChecksumDuc(mSqliteFilename).Process()

#If DEBUG Then
            Threading.Thread.Sleep(30000)
#End If
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

    Private Function ImportMobizLibraryPIMProduct()

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String
        Dim pRecords As Integer
        Dim pRowIndex As Integer

        AppendToLog("ref: vbKC2DPN5U8H! ImportMobizLibraryPIMProduct", , LogFilename)

        pSql = "SELECT * FROM MobizLibraryPIMProduct"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then Return True

        For Each pDR As DataRow In pDT.Rows
            Try
                mImportResult.Status = "Importing " & pRowIndex + 1 & " of " & pDT.Rows.Count & " products"
                pIU = New InsertUpdate("MobizLibraryPIMProduct")
                pIU("ProductNo") = ReadyString(pDR("ProductNo"))
                pIU("ProductName") = ReadyString(pDR("ProductName"))
                pIU("Brand") = ReadyString(pDR("Brand"))
                pIU("ProductSearchString") = ReadyString(pDR("ProductSearchString"))
                pIU("IsActive") = If(CBool2(pDR("IsActive")), 1, 0)
                pIU("LastUpdated") = ReadyString(pDR("LastUpdated"))
                pIU("ModelNo") = ReadyString(pDR("ModelNo"))
                pIU("ProductFamilyID") = ReadyString(pDR("ProductFamilyID"))
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
                    pIU("DateAdded") = "GETDATE()"
                    pIU("PublishedByID") = ReadyString(OrgID)
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If
                ImportLibraryOrganizationPIM(pDR("ID"))
                ImportMobizLibraryPIMProductVariableFeatures001(pDR("ID"))
                ImportMobizLibraryPIMProductMeasurement(pDR("ID"))
                ImportMobizLibraryPIMProductPriceLevel(pDR("ID"))
                pRowIndex += 1
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try
        Next pDR

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryPIMProduct', 'LibraryOrganizationPIM')"
        mProData.ExecuteSQL(pSql)

    End Function

    Private Sub ImportLibraryOrganizationPIM(vProductID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryOrganizationPIM" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID)
        pCount = CInt2(mProData.ExecuteScalar(pSql))
        If pCount > 0 Then
            Return
        End If

        pIU = New InsertUpdate("LibraryOrganizationPIM")
        pIU("ProductID") = ReadyString(vProductID)
        pIU("Comments") = "'Created by ErpToMobizImporter'"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Function ImportMobizLibraryPIMProductVariableFeatures001(vProductID As String)

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer

        pSql = "SELECT * FROM MobizLibraryPIMProductVariableFeatures001 WHERE ProductID = " & ReadyString(vProductID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        pSql = "DELETE FROM MobizLibraryPIMProductVariableFeatures001 WHERE ProductID = " & ReadyString(vProductID)
        mProData.ExecuteSQL(pSql, True)

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")
            pIU("ID") = "newid()"
            pIU("ProductID") = ReadyString(vProductID)
            'pIU("C0062") = ReadyString(pDR("C0062")) ' old
            pIU("C0020") = ReadyString(pDR("C0020")) ' new
            pIU("C0056") = ReadyString(pDR("C0056"))
            pIU("C0057") = ReadyString(pDR("C0057"))
            pIU("C0058") = ReadyString(pDR("C0058"))
            pIU("C0059") = ReadyString(pDR("C0059"))
            pIU("C0060") = ReadyString(pDR("C0060"))
            pIU("C0061") = ReadyString(pDR("C0061"))
            pIU("C0063") = ReadyString(pDR("C0063"))
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
            'pSql = pIU.GetUpdate("ProductID = " & ReadyString(vProductID) & vbCrLf &
            '    " AND ISNULL(C0062, '') = " & ReadyString(pDR("C0062")) & vbCrLf &
            '    " AND ISNULL(C0056, '') = " & ReadyString(pDR("C0056")) & vbCrLf &
            '    " AND ISNULL(C0057, '') = " & ReadyString(pDR("C0057")) & vbCrLf &
            '    " AND ISNULL(C0058, '') = " & ReadyString(pDR("C0058")) & vbCrLf &
            '    " AND ISNULL(C0059, '') = " & ReadyString(pDR("C0059")) & vbCrLf &
            '    " AND ISNULL(C0060, '') = " & ReadyString(pDR("C0060")) & vbCrLf &
            '    " AND ISNULL(C0061, '') = " & ReadyString(pDR("C0061")) & vbCrLf &
            '    " AND ISNULL(C0063, '') = " & ReadyString(pDR("C0063")) & vbCrLf _
            ')
            'pRecords = mProData.ExecuteSQL(pSql, True)
            'If pRecords = 0 Then
            '    pIU("ID") = ReadyString(pDR("ID"))
            '    pSql = pIU.GetInsert
            '    mProData.ExecuteSQL(pSql)
            'End If
        Next pDR

    End Function

    Private Function ImportMobizLibraryPIMProductMeasurement(vProductID As String)
        ' trigger: trLibStockMeasurementERPtoMobiz

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(vProductID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        ' LibraryPIMProductMeasurementBarcode_fk
        pSql = "DELETE FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf &
            " WHERE StockMeasurementsID IN " & vbCrLf &
            " (SELECT ID FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & ")"
        pSql = mProData.ExecuteSQL(pSql, True)

        pSql = "DELETE FROM MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(vProductID)
        pSql = mProData.ExecuteSQL(pSql, True)

        If ReadyGuid(vProductID) = ReadyGuid("9d80c340-826f-477b-a8a8-7538f45257b6") Then
            Debug.WriteLine(vProductID)
        End If

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("MobizLibraryPIMProductMeasurement")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("ProductID") = ReadyString(pDR("ProductID"))
            pIU("MeasurementLevel") = 1
            pIU("UnitMeasureID") = ReadyString(mBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID(pDR("UnitMeasure")))
            pIU("Quantity") = pDR("Quantity")
            pIU("Discount") = pDR("Discount")
            pIU("LastUpdated") = "GETDATE()"
            pIU("SourceFlag") = 0 ' 0 - ERP, 1 - Mobiz
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)

            ImportMobizLibraryPIMProductMeasurementBarcode(pDR("ID"))
        Next pDR

    End Function

    Private Function ImportMobizLibraryPIMProductMeasurementBarcode(vStockMeasurementsID As String) As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryPIMProductMeasurementBarcode WHERE StockMeasurementsID = " & ReadyString(vStockMeasurementsID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        pSql = "DELETE FROM MobizLibraryPIMProductMeasurementBarcode WHERE StockMeasurementsID = " & ReadyString(vStockMeasurementsID)
        mProData.ExecuteSQL(pSql, True)

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("StockMeasurementsID") = ReadyString(vStockMeasurementsID)
            pIU("Barcode") = ReadyString(pDR("Barcode"))
            pIU("SourceFlag") = 0 ' 0 - ERP, 1 - Mobiz
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryPIMProductMeasurementBarcode')"
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function ImportMobizLibraryPIMProductPriceLevel(vProductID As String) As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String

        Try
            If ReadyGuid(vProductID) = ReadyGuid("40834BE6-2960-4409-97E3-955C4A349574") Then
                Debug.WriteLine(vProductID)
            End If

            pSql = "SELECT * FROM MobizLibraryPIMProductPriceLevel WHERE ProductID = " & ReadyString(vProductID)
            pDT = mSqlite.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                Return False
            End If

            pSql = "DELETE FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(OrgID)
            pSql = mProData.ExecuteSQL(pSql, True)

            For Each pDR As DataRow In pDT.Rows
                pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
                pIU("ID") = ReadyString(pDR("ID"))
                pIU("OrganizationID") = ReadyString(OrgID)
                pIU("ProductID") = ReadyString(vProductID)
                pIU("PriceLevelID") = ReadyString(pDR("PriceLevelID"))
                pIU("Price") = CDec(pDR("Price"))
                pIU("DeactivateDiscount") = If(CBool2(pDR("DeactivateDiscount")), 1, 0)
                pIU("SourceFlag") = 0 ' 0 - ERP, 1 - Mobiz
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
                pSql = pIU.GetInsert
                Try
                    mProData.ExecuteSQL(pSql)
                Catch ex As SqlException
                    If ex.Message.StartsWith("Violation of PRIMARY KEY constraint") Then
                        LogToFile(BypassedErrorFolder, pSql, ex)
                    End If
                End Try
            Next pDR

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryPIMProductPriceLevel')"
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            LogToFile(CriticalFolder, pSql, ex)
            Throw
        End Try

    End Function

    Private Function ImportMobizLibraryPIMPriceLevel() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String
        Dim pRecords As Integer
        Dim pDrPeople As DataRow

        AppendToLog("ref: vbXFP6HEYZ4C! ImportMobizLibraryPIMPriceLevel", , LogFilename)
        pSql = "SELECT * FROM MobizLibraryPIMPriceLevel"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            Try
                pIU = New InsertUpdate("MobizLibraryPIMPriceLevel")
                pIU("PriceLevel") = ReadyString(pDR("PriceLevel"))
                pIU("IsActive") = If(CBool2(pDR("IsActive")), 1, 0)
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
                mProData.ExecuteSQL(pSql)
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try

        Next pDR

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryPIMPriceLevel')"
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function ImportMobizLibrarySalesChannels() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer

        AppendToLog("ref: vb7JZDHCSMGZ! ImportMobizLibrarySalesChannels", , LogFilename)
        pSql = "SELECT * FROM MobizLibrarySalesChannels"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            Try
                pSql = "SELECT COUNT(*)" & vbCrLf &
                    " FROM MobizLibrarySalesChannels" & vbCrLf &
                    " WHERE Name = " & ReadyString(pDR("CustomerType"))
                pRecords = CInt2(mProData.ExecuteScalar(pSql))
                If pRecords > 0 Then
                    Continue For
                End If
                pIU = New InsertUpdate("MobizLibrarySalesChannels")
                pIU("ID") = ReadyString(Guid.NewGuid())
                pIU("Name") = ReadyString(pDR("CustomerType"))
                pIU("IsActive") = 1
                pIU("LastUpdated") = "GETDATE()"
                pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try

        Next pDR

    End Function

    Private Function ImportCustomerSalesReps() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer

        AppendToLog("ref: vb8KGU3PEWHJ! ImportCustomerSalesReps", , LogFilename)
        pSql = "SELECT * FROM LibraryOrganizationCustomerSalesReps"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            Try
                If IsNumeric(pDR("EmployeeID")) = False Then
                    Continue For
                End If
                Dim pUserID = GetUserID(CInt2(pDR("EmployeeID")))
                If String.IsNullOrWhiteSpace(pUserID) Then
                    Continue For
                End If
                pSql = "SELECT COUNT(*)" & vbCrLf &
                    " FROM LibraryOrganizationCustomerSalesReps" & vbCrLf &
                    " WHERE OrganizationID = " & ReadyString(pDR("CustomerID")) & vbCrLf &
                    " AND PeopleID = " & ReadyString(pUserID) & vbCrLf
                pRecords = CInt2(mProData.ExecuteScalar(pSql))
                If pRecords > 0 Then
                    Continue For
                End If
                pIU = New InsertUpdate("LibraryOrganizationCustomerSalesReps")
                pIU("ID") = ReadyString(Guid.NewGuid())
                pIU("OrganizationID") = ReadyString(pDR("CustomerID"))
                pIU("PeopleID") = ReadyString(pUserID)
                pIU("LastUpdated") = "GETDATE()"
                pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try

        Next pDR

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
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
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

    Private Function ImportMobizLibraryOrganization() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String
        Dim pRecords As Integer
        Dim pRowIndex As Integer = 0

        AppendToLog("ref: vbU5US2J3LJ5! ImportMobizLibraryOrganization", , LogFilename)
        pSql = "SELECT * FROM MobizLibraryOrganization"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then Return True

        For Each pDR As DataRow In pDT.Rows
            Try
                mImportResult.Status = "Importing " & pRowIndex + 1 & " of " & pDT.Rows.Count & " organizations"
                pIU = New InsertUpdate("MobizLibraryOrganization")
                pIU("CompanyName") = ReadyString(pDR("CompanyName"))
                pIU("TinNumber") = ReadyString(pDR("TinNumber"))
                pIU("IsFranchisor") = If(CBool2(pDR("IsFranchisor")), 1, 0)
                pIU("RegistrationType") = ReadyString(pDR("RegistrationType"))
                pIU("RegistrationNo") = ReadyString(pDR("RegistrationNo"))
                If pDR("RegistrationDate") IsNot DBNull.Value Then
                    pIU("RegistrationDate") = ReadyString(pDR("RegistrationDate"))
                End If
                pIU("OfficeAddressBuildingNo") = ReadyString(pDR("OfficeAddressBuildingNo"))
                pIU("OfficeAddressStreet") = ReadyString(pDR("OfficeAddressStreet"))
                If pDR("OfficeAddressCityID") IsNot DBNull.Value Then
                    pIU("OfficeAddressCityID") = ReadyString(pDR("OfficeAddressCityID"))
                End If
                If pDR("OfficeAddressCountryID") IsNot DBNull.Value Then
                    pIU("OfficeAddressCountryID") = ReadyString(pDR("OfficeAddressCountryID"))
                End If
                pIU("BusinessAddressBuildingNo") = ReadyString(pDR("BusinessAddressBuildingNo"))
                pIU("BusinessAddressStreet") = ReadyString(pDR("BusinessAddressStreet"))
                If pDR("BusinessAddressCityID") IsNot DBNull.Value Then
                    pIU("BusinessAddressCityID") = ReadyString(pDR("BusinessAddressCityID"))
                End If
                If pDR("BusinessAddressCountryID") IsNot DBNull.Value Then
                    pIU("BusinessAddressCountryID") = ReadyString(pDR("BusinessAddressCountryID"))
                End If
                pIU("Purpose") = ReadyString(pDR("Purpose"))
                pIU("BusinessScope") = ReadyString(pDR("BusinessScope"))
                pIU("NoOfRetailOutlet") = CDec2(pDR("NoOfRetailOutlet"))
                pIU("NoOfOffice") = CDec2(pDR("NoOfOffice"))
                pIU("NoOfWarehouse") = CDec2(pDR("NoOfWarehouse"))
                pIU("NoOfEmployees") = CDec2(pDR("NoOfEmployees"))
                pIU("IsHasHoldingCompany") = If(CBool2(pDR("IsHasHoldingCompany")), 1, 0)
                If pDR("HoldingCompanyID") IsNot DBNull.Value Then
                    pIU("HoldingCompanyID") = ReadyString(pDR("HoldingCompanyID"))
                End If
                'pIU("IsOrganization") = If(CBool2(pDR("IsOrganization")), 1, 0) ' old logic
                If String.IsNullOrWhiteSpace(CStr2(pDR("CompanyName"))) Then
                    pIU("IsOrganization") = 0
                Else
                    pIU("IsOrganization") = 1
                End If
                If pDR("PersonTitleID") IsNot DBNull.Value Then
                    pIU("PersonTitleID") = ReadyString(pDR("PersonTitleID"))
                End If
                pIU("PersonSurname") = ReadyString(pDR("PersonSurname"))
                pIU("PersonFirstName") = ReadyString(pDR("PersonFirstName"))
                pIU("PersonMiddleName") = ReadyString(pDR("PersonMiddleName"))
                pIU("PersonSuffixName") = ReadyString(pDR("PersonSuffixName"))
                If pDR("PersonDateOfBirth") IsNot DBNull.Value Then
                    pIU("PersonDateOfBirth") = ReadyString(pDR("PersonDateOfBirth"))
                End If
                pIU("PersonMotherMaidenSurname") = ReadyString(pDR("PersonMotherMaidenSurname"))
                pIU("PersonMotherMaidenFirstName") = ReadyString(pDR("PersonMotherMaidenFirstName"))
                pIU("PersonMotherMaidenMiddleName") = ReadyString(pDR("PersonMotherMaidenMiddleName"))

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
                If String.IsNullOrWhiteSpace(CStr2(pDR("BusinessTypeID"))) = False _
                    AndAlso String.IsNullOrWhiteSpace(CStr2(pDR("BusinessType"))) = False Then

                    Dim pUserDefinedIndustryID = CreateLibraryUserDefinedIndustry(pDR)
                    pIU("UserDefinedIndustryID") = ReadyString(pUserDefinedIndustryID)
                End If
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                pRecords = mProData.ExecuteSQL(pSql, True)
                If pRecords = 0 Then
                    pIU("ID") = ReadyString(pDR("ID"))
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If
                If pDR("IsCustomer") Then
                    CreateLibraryOrganizationCustomer(pDR)
                    ImportLibraryOrganizationPriceLevel(CStr2(pDR("ID")))
                    ImportLibraryCustomerTerm(pDR)
                End If
                If pDR("IsSupplier") Then
                    CreateLibraryOrganizationSupplier(pDR)
                    ImportLibrarySupplierTerm(pDR)
                End If
                ImportMobizLibraryOrganizationFOWST(CStr2(pDR("ID")))
                ImportMobizLibraryOrganizationContactPerson(CStr2(pDR("ID")))
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try
            pRowIndex += 1
        Next pDR

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName IN ('MobizLibraryOrganization')"
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function CreateLibraryUserDefinedIndustry(vDR As DataRow) As String

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryUserDefinedIndustry" & vbCrLf &
            " WHERE LTRIM(RTRIM(Name)) = " & ReadyString(CStr2(vDR("BusinessType")).Trim())
        Dim pID = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pID) Then
            Dim pIU = New InsertUpdate("MobizLibraryUserDefinedIndustry")
            pIU("ID") = ReadyString(vDR("BusinessTypeID"))
            pIU("Name") = ReadyString(CStr2(vDR("BusinessType")).Trim())
            pIU("IsActive") = "1"
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            pID = CStr2(vDR("BusinessTypeID"))
        End If

        pID = ReadyGuid(pID)

        Return pID

    End Function

    Private Function CreateLibraryOrganizationCustomer(vDR As DataRow)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pIU = New InsertUpdate("LibraryOrganizationCustomer")
        If IsNumeric(vDR("CreditLimit")) Then
            pIU("CreditLimit") = vDR("CreditLimit")
        End If
        If CStr2(vDR("CustomerType")).Trim()<>"" Then
            pSql = "SELECT ID" & vbCrLf &
                " FROM MobizLibrarySalesChannels" & vbCrLf &
                " WHERE LTRIM(RTRIM(Name)) = " & ReadyString(CStr2(vDR("CustomerType")).Trim())
            Dim pSalesChannelID = CStr2(mProData.ExecuteScalar(pSql))
            If String.IsNullOrWhiteSpace(pSalesChannelID) = False Then
                pIU("SalesChannelID") = ReadyString(pSalesChannelID)
            End If
        End If
        pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vDR("ID")))
        pCount = mProData.ExecuteSQL(pSql, True)
        If pCount = 0 Then
            pIU("OrganizationID") = ReadyString(vDR("ID"))
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Function

    Private Function CreateLibraryOrganizationSupplier(vDR As DataRow)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pIU = New InsertUpdate("LibraryOrganizationSupplier")
        If IsNumeric(vDR("CreditLimit")) Then
            pIU("CreditLimit") = vDR("CreditLimit")
            If vDR("CreditLimit") <> 0 Then
                Debug.WriteLine("vbU52VJCGZAR!")
            End If
        End If
        pSql = pIU.GetUpdate("SupplierID = " & ReadyString(vDR("ID")))
        pCount = mProData.ExecuteSQL(pSql, True)
        If pCount = 0 Then
            pIU("SupplierID") = ReadyString(vDR("ID"))
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Function

    Private Function ImportMobizLibraryOrganizationFOWST(vOrgID As String)

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer
        Dim pUserID As String
        Dim pIDs As String = ""

        pSql = "SELECT * FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(vOrgID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            If pIDs <> "" Then
                pIDs &= ", "
            End If
            pIDs &= ReadyString(pDR("ID"))
        Next pDR

        ' todo: delete from LibraryOrganizationFranchisee
        pSql = "DELETE FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " WHERE FranchisorOrgID = " & ReadyString(vOrgID) & vbCrLf &
            " AND FranchiseeOrgFowstID IN (" & pIDs & ")"
        pSql = mProData.ExecuteSQL(pSql, True)

        ' stop deleting fowst 2022_12_08__10_41_20_0658
        'pSql = "DELETE FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(vOrgID) &
        '    " AND ID IN (" & pIDs & ")"
        'pSql = mProData.ExecuteSQL(pSql, True)

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("OrganizationID") = ReadyString(pDR("OrganizationID"))
            pIU("StreetNumber") = ReadyString(Truncate(CStr2(pDR("StreetNumber")), 500))
            If String.IsNullOrWhiteSpace(CStr2(pDR("Street"))) Then
                pIU("Street") = ReadyString(Truncate(CStr2(pDR("StreetName")), 1000))
            Else
                pIU("Street") = ReadyString(Truncate(CStr2(pDR("Street")), 1000))
            End If
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
            pIU("Code") = ReadyString(pDR("Code"))
            pIU("Name") = ReadyString(pDR("Name"))
            If pDR("BusinessTradeNameID") IsNot DBNull.Value Then
                pIU("Description") = ReadyString(pDR("Description"))
            End If
            If pDR("FowstCityID") IsNot DBNull.Value Then
                pIU("FowstCityID") = ReadyString(pDR("FowstCityID"))
            End If
            If pDR("FowstCountryID") IsNot DBNull.Value Then
                pIU("FowstCountryID") = ReadyString(pDR("FowstCountryID"))
            End If
            pIU("PlateNo") = ReadyString(pDR("PlateNo"))
            pIU("NoOfWheels") = ReadyString(pDR("NoOfWheels"))
            pIU("BodyColor") = ReadyString(pDR("BodyColor"))
            If pDR("Weight") IsNot DBNull.Value Then
                pIU("Weight") = pDR("Weight")
            End If
            pIU("Dimension") = ReadyString(pDR("Dimension"))
            pIU("OwnershipType") = If(CBool2(pDR("OwnershipType")), 1, 0)
            If pDR("ContractStart") IsNot DBNull.Value Then
                pIU("ContractStart") = ReadyString(pDR("ContractStart"))
            End If
            If pDR("ContractEnd") IsNot DBNull.Value Then
                pIU("ContractEnd") = ReadyString(pDR("ContractEnd"))
            End If
            If pDR("CostPerMonth") IsNot DBNull.Value Then
                pIU("CostPerMonth") = ReadyString(pDR("CostPerMonth"))
            End If
            pIU("IsMortgaged") = If(CBool2(pDR("IsMortgaged")), 1, 0)
            pIU("IsFranchisee") = If(CBool2(pDR("IsFranchisee")), 1, 0)
            If pDR("FranchisorID") IsNot DBNull.Value Then
                pIU("FranchisorID") = ReadyString(pDR("FranchisorID"))
            End If
            pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " WHERE ID = " & ReadyString(pDR("ID"))
            If CInt2(mProData.ExecuteScalar(pSql)) > 0 Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Else
                pSql = pIU.GetInsert
            End If
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Private Function ImportMobizLibraryOrganizationContactPerson(vOrgID As String)

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer
        Dim pUserID As String
        Dim pIDs As String = ""

        pSql = "SELECT * FROM MobizLibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(vOrgID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            If pIDs <> "" Then
                pIDs &= ", "
            End If
            pIDs &= ReadyString(pDR("ID"))
        Next pDR

        pSql = "DELETE FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(vOrgID) &
            " AND ID IN (" & pIDs & ")"
        pSql = mProData.ExecuteSQL(pSql, True)

        For Each pDR As DataRow In pDT.Rows
            pSql = " SELECT COUNT(*)" & vbCrLf &
                " FROM MobizLibraryOrganizationContactPerson" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationContactPerson.PeopleID" & vbCrLf &
                " WHERE MobizLibraryPeople.Surname = " & ReadyString(pDR("LastName")) & vbCrLf &
                " AND MobizLibraryPeople.FirstName = " & ReadyString(pDR("FirstName")) & vbCrLf &
                " AND MobizLibraryPeople.MiddleName = " & ReadyString(pDR("MiddleName"))
            Dim pExistingRecords = CInt2(mProData.ExecuteScalar(pSql))
            If pExistingRecords > 0 Then
                Continue For
            End If
            pIU = New InsertUpdate("MobizLibraryOrganizationContactPerson")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("OrganizationID") = ReadyString(pDR("OrganizationID"))
            pIU("PeopleID") = ReadyString(mBusiness.LibraryPeople.GetOrCreatePeople(CStr2(pDR("LastName")), CStr2(pDR("FirstName")), CStr2(pDR("MiddleName"))))
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
            pIU("LastUpdated") = ReadyString(pDR("LastUpdated"))
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Private Function ImportLibraryOrganizationPriceLevel(vOrgID As String)

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecords As Integer
        Dim pUserID As String
        Dim pIDs As String = ""

        pSql = "SELECT * FROM LibraryOrganizationPriceLevel WHERE OrganizationID = " & ReadyString(vOrgID)
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return False
        End If

        For Each pDR As DataRow In pDT.Rows
            If pIDs <> "" Then
                pIDs &= ", "
            End If
            pIDs &= ReadyString(pDR("ID"))
        Next pDR

        pSql = "DELETE FROM LibraryOrganizationPriceLevel WHERE OrganizationID = " & ReadyString(vOrgID) &
            " AND ID IN (" & pIDs & ")"
        pSql = mProData.ExecuteSQL(pSql, True)

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("LibraryOrganizationPriceLevel")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("OrganizationID") = ReadyString(pDR("OrganizationID"))
            pIU("PriceLevelID") = ReadyString(pDR("PriceLevelID"))
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
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Private Sub ImportLibraryCustomerTerm(vDR As DataRow)

        Dim pSql As String
        Dim pDays As Integer

        If IsNumeric(vDR("Term")) = False Then
            Exit Sub
        End If

        Try
            pDays = vDR("Term")
        Catch ex As Exception
            Exit Sub
        End Try

        pSql = " SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
            " WHERE LibraryPaymentTerms.Days = " & pDays & vbCrLf &
            " AND LibraryOrganizationCustomer.OrganizationID = " & ReadyString(vDR("ID"))
        Dim pExisting = CInt2(mProData.ExecuteScalar(pSql))
        If pExisting > 0 Then
            Exit Sub
        End If

        Dim pTermID = GetOrCreatePaymentTerm(pDays, CStr2(vDR("TermDescription")))
        Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
        pIU("CreditTermsID") = ReadyString(pTermID)
        pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vDR("ID")))
        Dim pAffected = mProData.ExecuteSQL(pSql, True)

    End Sub

    Private Sub ImportLibrarySupplierTerm(vDR As DataRow)

        Dim pSql As String
        Dim pDays As Integer

        If IsNumeric(vDR("Term")) = False Then
            Exit Sub
        End If

        Try
            pDays = vDR("Term")
        Catch ex As Exception
            Exit Sub
        End Try

        pSql = " SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryOrganizationSupplier" & vbCrLf &
            " JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf &
            " WHERE LibraryPaymentTerms.Days = " & pDays & vbCrLf &
            " AND LibraryOrganizationSupplier.SupplierID= " & ReadyString(vDR("ID"))
        Dim pExisting = CInt2(mProData.ExecuteScalar(pSql))
        If pExisting > 0 Then
            Exit Sub
        End If

        Dim pTermID = GetOrCreatePaymentTerm(pDays, vDR("Term"))
        Dim pIU = New InsertUpdate("LibraryOrganizationSupplier")
        pIU("CreditTermsID") = ReadyString(pTermID)
        pSql = pIU.GetUpdate("SupplierID = " & ReadyString(vDR("ID")))
        Dim pAffected = mProData.ExecuteSQL(pSql, True)

    End Sub

    Private Function GetOrCreatePaymentTerm(vDays As Integer, vName As String) As String

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryPaymentTerms" & vbCrLf &
            " WHERE Days = " & vDays
        Dim pOut = CStr2(mProData.ExecuteScalar(pSql))
        If IsGuid(pOut) Then
            Return pOut
        End If

        Dim pIU = New InsertUpdate("LibraryPaymentTerms")
        pOut = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pOut)
        pIU("Name") = ReadyString(Truncate(vName, 50))
        pIU("Days") = vDays
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Private Function ImportLibraryVendorPrices() As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pUserID As String
        Dim pRecords As Integer
        Dim pRowIndex As Integer

        AppendToLog("ref: vbU5US2J3LJ5! LibraryVendorPrices", , LogFilename)
        pSql = "SELECT * FROM LibraryVendorPrices"
        pDT = mSqlite.Execute(pSql)
        If pDT.Rows.Count = 0 Then Return True

        For Each pDR As DataRow In pDT.Rows
            Try
                mImportResult.Status = "Importing " & pRowIndex + 1 & " of " & pDT.Rows.Count & " vendor prices"
                pIU = New InsertUpdate("LibraryVendorPrices")
                Dim pUnitQty = CDec2(pDR("ActualQuantity"))
                If pUnitQty < 0.001 Then
                    pUnitQty = 1
                End If
                pIU("Price") = CDec2(pDR("Price")) / pUnitQty
                pIU("IsActive") = "1"
                pIU("LastUpdated") = ReadyString(Format(CDate(pDR("LastUpdated")), cGlobal.FormatDateTimeSql2))
                pUserID = "NULL"
                If IsNumeric(pDR("UserID")) Then
                    pUserID = GetUserID(pDR("UserID"))
                    If pUserID = "" Then
                        pUserID = "NULL"
                    Else
                        pUserID = ReadyString(pUserID)
                    End If
                End If
                pIU("UpdatedBy") = pUserID

                pSql = "SELECT ID" & vbCrLf &
                    " FROM LibraryVendorPrices" & vbCrLf &
                    " WHERE ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                    " AND VendorID = " & ReadyString(pDR("VendorID"))
                Dim pOldId = CStr2(mProData.ExecuteScalar(pSql))
                If String.IsNullOrWhiteSpace(pOldId) Then
                    pOldId = Guid.NewGuid.ToString()
                    pIU("ID") = ReadyString(pOldId)
                    pIU("ProductID") = ReadyString(pDR("ProductID"))
                    pIU("VendorID") = ReadyString(pDR("VendorID"))
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pOldId))
                End If

                mProData.BeginTransaction()
                mProData.ExecuteSQL(pSql)
                mBusiness.TransInvoice.SaveVendorPriceHistory(
                    vVendorID:=CStr2(pDR("VendorID")),
                    vProductID:=CStr2(pDR("ProductID")),
                    vUnitPrice:=CDec2(pDR("Price")) / pUnitQty,
                    vTransDate:=CDate(pDR("LastUpdated")),
                    vTransID:=pOldId,
                    vTransType:="ImportFrom3rdParty")

                mProData.CommitTransaction()
            Catch ex As Exception
                If TypeOf ex Is ProSqlException Then
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf & CType(ex, ProSqlException).Sql & vbCrLf, ex)
                Else
                    LogToFile(BypassedErrorFolder, ex.Message & vbCrLf, ex)
                End If
            End Try
            pRowIndex += 1
        Next pDR

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

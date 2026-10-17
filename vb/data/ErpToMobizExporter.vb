Imports Newtonsoft.Json

Public Class ErpToMobizExporter
    Dim mProData As ProData
    Private Const mTop As Integer = 100
    Public Const OrgID As String = "9D4B1500-13C0-49F9-A8E5-A16C13EC899E"
    Dim mSqlite As ProSqlite
    Dim mSqliteFilename As String
    Dim mZipFilename As String
    Dim mSettingsFile As String
    Dim mStatus As String
    Dim mTotalRecords As Integer
    Dim mDtRowCount As DataTable
    Dim mSettings As SettingsClass
    Public Busy As Boolean
    Public Const LogFilename = "ErpToMobizExporter"
    Public LastestRecordsFirst As Boolean ' used for data verification
    Dim mHasExportableProductIDs As Boolean

    Private ReadOnly Property SettingsFile As String
        Get

            If mSettingsFile Is Nothing Then
                mSettingsFile = Common.ServerPath & "settings\ErpToMobizExporterSettings.txt"
            End If

            Return mSettingsFile

        End Get
    End Property

    Public Property Settings As SettingsClass
        Get
            Return mSettings
        End Get
        Set(value As SettingsClass)
            mSettings = value
        End Set
    End Property

    Public Sub LoadLastUpdated()

        Dim pFileText As String

        Try
            If mSettings IsNot Nothing Then
                Return
            End If
            pFileText = FileText(SettingsFile)
            mSettings = SettingsClass.FromRawString(pFileText)
        Catch ex As Exception
            mSettings = New SettingsClass()
        End Try

    End Sub

    Public Sub SaveLastUpdated()

        Dim pFileText As String

        pFileText = mSettings.ToString()
        FileText(SettingsFile) = pFileText

    End Sub

    Public ReadOnly Property Status As String
        Get

            Return mStatus

        End Get
    End Property

    Public Function Export() As Boolean

        Try
            Busy = True
            mStatus = "initializing"
            LoadLastUpdated()

            mStatus = "connecting"
            mProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:="wholesale",
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password)

            If mProData.ConnectError = "Database 'wholesale' does not exist. Make sure that the name is entered correctly." Then
                Return False
            End If
            Try
                mProData.BeginTransaction()
            Catch ex As Exception
                Return False
            End Try
            mSqliteFilename = Common.ServerPath & "temp\ExportErpToMobiz\" & DateTimeStamp() & ".db"
            CreateFolder(mSqliteFilename)
            mSqlite = New ProSqlite()
            mSqlite.Open(mSqliteFilename)

            mStatus = "exporting"
            mTotalRecords = 0
            mDtRowCount = CreateRowCountTable()
            ExportLibraryStockInformation()
            ExportToLibraryPIMProductVariableFeatures001()
            ExportLibraryStockMeasurements()
            ExportLibraryStockMeasurementsBarcode()
            ExportLibraryStockPriceLevels()

            ExportLibraryPriceLevels()
            ExportSalesChannels()
            ExportCustomerSalesReps()

            ExportLibraryCustomers()
            ExporLibraryCustomersRepresentatives()
            ExportLibraryCities()
            ExportLibraryCustomersAddresses()
            ExportLibraryCustomersPriceLevel()

            ExportLibrarySuppliers()
            ExportLibrarySuppliersAddress()
            ExportLibrarySupplierPerson()
            ExportLibraryVendorPrices()

            ExportRowCount()

            mSqlite.Close()
            Compress()

            mProData.RollbackTransaction()

            mStatus = "done"
            Return True
        Catch ex As Exception
            AppendToLog("vbKBV84MNUGT! ErpToMobizExporter_Export. " & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(True, ex), , ErpToMobizExporter.LogFilename)
            Throw
        Finally
            Busy = False
        End Try

    End Function

#Region " row count check sum functions "
    Private Function CreateRowCountTable() As DataTable

        Dim pOut As DataTable

        pOut = New DataTable
        pOut.Columns.Add("Table").Unique = True
        'pOut.PrimaryKey = New DataColumn() {pOut.Columns("Table")}
        pOut.Columns.Add("Count", GetType(Integer))

        Return pOut

    End Function

    Private Sub AddTableRowCount(vTable As String, vCount As Integer)

        Dim pDV = New DataView(mDtRowCount, "Table = " & ReadyString(vTable), "", DataViewRowState.CurrentRows)
        If pDV.Count > 0 Then
            For Each pDRV As DataRowView In pDV
                pDRV("Count") += vCount
            Next pDRV
        Else
            Dim pDR = mDtRowCount.NewRow
            pDR("Table") = vTable
            pDR("Count") = vCount
            mDtRowCount.Rows.Add(pDR)
        End If

    End Sub
#End Region

    Private Sub AppendTable(vTable As String, vData As DataTable)

        AppendToLog("vbH8MZL63FVV! ErpToMobizExporter.AppendTable " & vTable, , ErpToMobizExporter.LogFilename)
        mSqlite.UpdateTable(vTable, vData)
        AddTableRowCount(vTable, vData.Rows.Count)

    End Sub

    Private Sub Compress()

        Dim pFilename As String
        Dim pZip As ProZip

        pFilename = IO.Path.Combine(IO.Path.GetDirectoryName(mSqliteFilename), IO.Path.GetFileNameWithoutExtension(mSqliteFilename)) & ".zip"
        CreateFolder(pFilename)
        pZip = New ProZip(pFilename)
        pZip.AddFile(mSqliteFilename)
        pZip.Close()
        mZipFilename = pFilename

    End Sub

    Public ReadOnly Property ZipFilename As String
        Get

            Return mZipFilename

        End Get
    End Property

    Private Sub ExportLibraryStockInformation()
        ' trigger: trLibStockInfoERPtoMobiz
        ' destination: MobizLibraryPIMProduct

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryStockInformation.ID" & vbCrLf &
            " ,LibraryStockInformation.StockNumber AS ProductNo" & vbCrLf &
            " ,RTRIM(LTRIM(LibraryBrands.BrandName)) +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryProducts.ProductName)),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[Make])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[UnitSize])),', '), '') +" & vbCrLf &
            " 	--ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[BasisWeight])),', '), '') + -- dont include for DSME" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[Content])),', '), '')" & vbCrLf &
            "     AS ProductName" & vbCrLf &
            " ,LibraryBrands.BrandName AS Brand," & vbCrLf &
            " RTRIM(LTRIM(LibraryBrands.BrandName)) +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryProducts.ProductName)),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryDepartments.DepartmentName)),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[Make])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[BasisWeight])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[UnitSize])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[StockNumber])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[Model])),', '), '') +" & vbCrLf &
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(LibraryStockInformation.[Content])),', '), '')" & vbCrLf &
            " 	AS ProductSearchString" & vbCrLf &
            " ,LibraryStockInformation.IsActive" & vbCrLf &
            " ,LibraryStockInformation.UserID" & vbCrLf &
            " ,LibraryStockInformation.LastUpdated" & vbCrLf &
            " ,LibraryStockInformation.Model AS ModelNo" & vbCrLf &
            " ,LibraryStockInformation.ProductID AS ProductFamilyID" & vbCrLf &
            ",ROW_NUMBER() OVER (ORDER BY LibraryStockInformation.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", LibraryStockInformation.ID) AS rn" & vbCrLf &
            " FROM LibraryStockInformation" & vbCrLf &
            " LEFT JOIN LibraryBrands ON LibraryBrands.ID = LibraryStockInformation.BrandID" & vbCrLf &
            " LEFT JOIN LibraryProducts ON LibraryProducts.ID = LibraryStockInformation.ProductID" & vbCrLf &
            " LEFT JOIN LibraryDepartments ON LibraryDepartments.ID = LibraryStockInformation.DepartmentID" & vbCrLf &
            " WHERE 1=1" & vbCrLf
        Dim pExportableProductIDs = GetExportableProductIDs()
        mHasExportableProductIDs = False
        If String.IsNullOrWhiteSpace(pExportableProductIDs) = False Then
            mHasExportableProductIDs = True
            pSql &= " AND LibraryStockInformation.ID IN (" & pExportableProductIDs & ")" & vbCrLf
            pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
            pSql &= "SELECT * " & vbCrLf &
                " INTO #tblPim" & vbCrLf &
                " FROM t" & vbCrLf &
                " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        ElseIf mSettings.LibraryStockInformationLastUpdated IsNot Nothing Then
            pSql &= " AND LibraryStockInformation.LastUpdated > " & ReadyString(Format(mSettings.LibraryStockInformationLastUpdated, cGlobal.FormatDateTimeSql2)) & vbCrLf
            pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
            pSql &= "SELECT * " & vbCrLf &
                " INTO #tblPim" & vbCrLf &
                " FROM t" & vbCrLf &
                " WHERE rn BETWEEN " & mSettings.LibraryStockInformationLastRowNumber & " AND " & (mSettings.LibraryStockInformationLastRowNumber + mTop - 1) & vbCrLf &
                " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        Else
            pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
            pSql &= "SELECT * " & vbCrLf &
                " INTO #tblPim" & vbCrLf &
                " FROM t" & vbCrLf &
                " WHERE rn BETWEEN " & mSettings.LibraryStockInformationLastRowNumber & " AND " & (mSettings.LibraryStockInformationLastRowNumber + mTop - 1) & vbCrLf &
                " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        End If
        mProData.ExecuteSQL(pSql)
        'AppendToLog("vbJDKEWJVYLY! ErpToMobizExporter.ExportLibraryStockInformation " & pSql, , ErpToMobizExporter.LogFilename)

        pSql = "SELECT * FROM #tblPim"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMProduct", pDT)
        If pDT.Rows.Count > 0 Then
            mSettings.LibraryStockInformationLastRowNumber += pDT.Rows.Count
            Dim pDR As DataRow = pDT.Rows(pDT.Rows.Count - 1)
            Try
                AppendToLog("vbQZSLP9QKS4! ErpToMobizExporter.ExportLibraryStockInformation pDT.Rows.Count=" & pDT.Rows.Count & vbCrLf &
                    "mSettings.LibraryStockInformationLastRowNumber=" & mSettings.LibraryStockInformationLastRowNumber & vbCrLf &
                    "pDR(LastUpdated)=" & CStr2(pDR("LastUpdated")), , ErpToMobizExporter.LogFilename)
            Catch ex As Exception
                AppendToLog("vbLVWBMVW85B! ErpToMobizExporter.ExportLibraryStockInformation " & ex.Message, , ErpToMobizExporter.LogFilename)
            End Try
            
            If pDR("LastUpdated") IsNot DBNull.Value _
                AndAlso String.IsNullOrWhiteSpace(pExportableProductIDs) Then

                If mSettings.LibraryStockInformationLastUpdated <> pDR("LastUpdated") Then
                    mSettings.LibraryStockInformationLastUpdated = pDR("LastUpdated")
                    mSettings.LibraryStockInformationLastRowNumber = 1
                End If
            End If
        Else
            Debug.Print("vbPXF86FZQRT! no record to export")
            mSettings.LibraryStockInformationLastRowNumber = 1
        End If
        mTotalRecords += pDT.Rows.Count

        ' on import:
        ' UserID = (SELECT TOP 1 ID FROM Mobiz.dbo.LibraryPeople Where DUCNumericID = #tblPim.[UserID])  

        ' after import:
        ' Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'
        ' Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'

    End Sub

    Private Shared mExportableProductIDsLock As New Object()

    Public Shared Function GetExportableProductIDs() As String

        SyncLock mExportableProductIDsLock
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterProductIDs.txt"
            Dim pFileText = FileText(pFilename)
            If String.IsNullOrWhiteSpace(pFileText) Then
                Return ""
            End If

            Dim pOut = ""
            Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each pSplit In pSplits
                If pOut <> "" Then
                    pOut &= ", "
                End If
                pOut &= ReadyString(pSplit)
            Next

            Return pOut
        End SyncLock

    End Function

    Public Shared Sub AddExportableProductID(vProductID As String)

        SyncLock mExportableProductIDsLock
            vProductID = ReadyGuid(vProductID)
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterProductIDs.txt"
            Dim pFileText = FileText(pFilename)
            If String.IsNullOrWhiteSpace(pFileText) = False Then
                If pFileText.Contains(vProductID) Then
                    Return
                End If
            End If
            AppendToFile(pFilename, vbCrLf & vProductID)
        End SyncLock

    End Sub

    Public Shared Sub ClearExportableProductID()

        SyncLock mExportableProductIDsLock
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterProductIDs.txt"
            FileText(pFilename) = ""
        End SyncLock

    End Sub

    Public Shared Function GetExportableVendorPriceProductIDs() As String

        SyncLock mExportableProductIDsLock
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterVendorPriceProductIDs.txt"
            Dim pFileText = FileText(pFilename)
            If String.IsNullOrWhiteSpace(pFileText) Then
                Return ""
            End If

            Dim pOut = ""
            Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each pSplit In pSplits
                If pOut <> "" Then
                    pOut &= ", "
                End If
                pOut &= ReadyString(pSplit)
            Next

            Return pOut
        End SyncLock

    End Function

    Public Shared Sub AddExportableVendorPriceProductID(vProductID As String)

        SyncLock mExportableProductIDsLock
            vProductID = ReadyGuid(vProductID)
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterVendorPriceProductIDs.txt"
            Dim pFileText = FileText(pFilename)
            If String.IsNullOrWhiteSpace(pFileText) = False Then
                If pFileText.Contains(vProductID) Then
                    Return
                End If
            End If
            AppendToFile(pFilename, vbCrLf & vProductID)
        End SyncLock

    End Sub

    Public Shared Sub ClearExportableVendorPriceProductID()

        SyncLock mExportableProductIDsLock
            Dim pFilename = Common.ServerPath & "settings\ErpToMobizExporterVendorPriceProductIDs.txt"
            FileText(pFilename) = ""
        End SyncLock

    End Sub

    Private Shared Function GetLastUpdated(vDT As DataTable) As Date

        Return vDT.Compute("MAX(LastUpdated)", "")

    End Function

    Private Sub ExportToLibraryPIMProductVariableFeatures001()
        ' trigger: trLibStockInfoERPtoMobiz
        ' destination: MobizLibraryPIMProductVariableFeatures001

        Dim pDT As DataTable
        Dim pSql As String

        ' old
        '", LibraryDepartments.DepartmentName AS C0062" & vbCrLf &
        pSql = " SELECT" & vbCrLf &
            " LibraryStockInformation.ID AS ProductID" & vbCrLf &
            ", LibraryDepartments.DepartmentName AS C0020" & vbCrLf &
            " ,LibraryStockInformation.StockNumber AS C0056" & vbCrLf &
            " ,LibraryStockInformation.Make AS C0057" & vbCrLf &
            " ,LibraryStockInformation.Model AS C0058" & vbCrLf &
            " ,LibraryStockInformation.Content AS C0059" & vbCrLf &
            " ,LibraryStockInformation.UnitSize AS C0060" & vbCrLf &
            " ,LibraryStockInformation.BasisWeight AS C0061" & vbCrLf &
            " ,LibraryProducts.ProductName AS C0063" & vbCrLf &
            " FROM LibraryStockInformation" & vbCrLf &
            " JOIN LibraryDepartments ON LibraryDepartments.ID = LibraryStockInformation.DepartmentID" & vbCrLf &
            " LEFT JOIN LibraryProducts ON LibraryProducts.ID = LibraryStockInformation.ProductID" & vbCrLf &
            " WHERE LibraryStockInformation.ID IN (SELECT ID FROM #tblPim)"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMProductVariableFeatures001", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryStockMeasurements()
        ' trigger: trLibStockMeasurementERPtoMobiz
        ' filename: ExportLibraryStockMeasurementsToLibraryPIMProductMeasurement.sql
        ' destination: MobizLibraryPIMProductMeasurement
        ' on destination, SourceFlag = 0

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryStockMeasurements.ID" & vbCrLf &
            ",LibraryStockMeasurements.StockID AS ProductID" & vbCrLf &
            ",LibraryUnitMeasures.UnitMeasure" & vbCrLf &
            " ,LibraryStockMeasurements.Quantity" & vbCrLf &
            " ,LibraryStockMeasurements.Discount" & vbCrLf &
            " INTO #tblStockMeasurements" & vbCrLf &
            " FROM LibraryStockMeasurements" & vbCrLf &
            " JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & vbCrLf &
            " WHERE LibraryStockMeasurements.StockID IN (SELECT ID FROM #tblPim)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblStockMeasurements"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMProductMeasurement", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryStockMeasurementsBarcode()
        ' filename: ExportLibraryStockMeasurementsBarcode.sql
        ' destination MobizLibraryPIMProductMeasurementBarcode
        ' on destination, SourceFlag = 0

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryStockMeasurementsBarcode.ID" & vbCrLf &
            " ,LibraryStockMeasurementsBarcode.StockMeasurementsID" & vbCrLf &
            " ,LibraryStockMeasurementsBarcode.Barcode" & vbCrLf &
            " FROM LibraryStockMeasurementsBarcode" & vbCrLf &
            " WHERE LibraryStockMeasurementsBarcode.StockMeasurementsID IN (SELECT ID FROM #tblStockMeasurements)"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMProductMeasurementBarcode", pDT)
        mTotalRecords += pDT.Rows.Count

        ' after import:
        ' Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductMeasurementBarcode'

    End Sub

    Private Sub ExportLibraryStockPriceLevels()
        '-- filename: ExportLibraryStockPriceLevels
        '-- from trigger: trLibStockPriceLevelERPtoMobiz
        '-- source: LibraryStockPriceLevels
        '-- destination: MobizLibraryPIMProductPriceLevel

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- ExportLibraryStockPriceLevels" & vbCrLf &
            " WITH CTE as (" & vbCrLf &
            "  SELECT LibraryStockPriceLevels.ID" & vbCrLf &
            "  ,LibraryStockPriceLevels.StockID AS ProductID" & vbCrLf &
            "  ,LibraryStockPriceLevels.PriceLevelID" & vbCrLf &
            "  --,ROUND(LibraryStockPriceLevels.Price * 2, 1) / 2 AS Price" & vbCrLf &
            "  ,LibraryStockPriceLevels.Price" & vbCrLf &
            "  ,LibraryStockPriceLevels.DeactivateDiscount" & vbCrLf &
            "  ,LibraryStockPriceLevels.LastUpdated" & vbCrLf &
            "  ,LibraryStockPriceLevels.UserID" & vbCrLf &
            "  FROM LibraryStockPriceLevels" & vbCrLf &
            " WHERE LibraryStockPriceLevels.StockID IN (SELECT ID FROM #tblPim)" & vbCrLf &
            "  )" & vbCrLf &
            "  " & vbCrLf &
            "  SELECT * " & vbCrLf &
            "  FROM CTE" & vbCrLf &
            "  CROSS APPLY (" & vbCrLf &
            " 	SELECT TOP 1 CTE2.ID" & vbCrLf &
            " 	FROM CTE CTE2 " & vbCrLf &
            " 	WHERE CTE2.ProductID = CTE.ProductID" & vbCrLf &
            " 	AND CTE2.PriceLevelID = CTE.PriceLevelID" & vbCrLf &
            " 	ORDER BY CTE2.LastUpdated DESC" & vbCrLf &
            "  ) C" & vbCrLf &
            "  WHERE CTE.ID = C.ID"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMProductPriceLevel", pDT)
        mTotalRecords += pDT.Rows.Count

        '-- on import:
        '-- SELECT TOP 1 ID FROM Mobiz.dbo.LibraryPeople Where DUCNumericID = inserted.[UserID])

        '-- after import:
        '-- Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductPriceLevel'

    End Sub

    Private Sub ExportLibraryPriceLevels()

        '-- filename: ExportLibraryPriceLevels
        '-- source: LibraryPriceLevels
        '-- destination: MobizLibraryPIMPriceLevel

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT " & vbCrLf &
            " ID" & vbCrLf &
            " ,PriceLevelName AS PriceLevel" & vbCrLf &
            " ,IsActive" & vbCrLf &
            " ,LastUpdated" & vbCrLf &
            " ,UserID" & vbCrLf &
            ",ROW_NUMBER() OVER (ORDER BY LibraryPriceLevels.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", LibraryPriceLevels.ID) AS rn" & vbCrLf &
            " FROM LibraryPriceLevels" & vbCrLf
        pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
        pSql &= "SELECT * " & vbCrLf &
            " FROM t" & vbCrLf &
            " WHERE rn BETWEEN " & mSettings.LibraryPriceLevelsLastRowNumber & " AND " & (mSettings.LibraryPriceLevelsLastRowNumber + mTop - 1) & vbCrLf &
            " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryPIMPriceLevel", pDT)
        If pDT.Rows.Count > 0 Then
            mSettings.LibraryPriceLevelsLastRowNumber += pDT.Rows.Count
        End If
        mTotalRecords += pDT.Rows.Count

        '-- after import:
        '-- Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMPriceLevel'
    End Sub

    Private Sub ExportSalesChannels()

        '-- source: LibraryCustomers.CustomerType
        '-- destination: MobizLibrarySalesChannels

        Dim pDT As DataTable
        Dim pSql As String
        Dim pTop As String = "100 PERCENT"
        If mHasExportableProductIDs Then
            pTop = "100 PERCENT"
        Else
            pTop = "0"
        End If
        pSql = " SELECT DISTINCT TOP " & pTop & " LibraryCustomers.CustomerType" & vbCrLf &
            " FROM LibraryCustomers" & vbCrLf &
            " WHERE LEN(LTRIM(RTRIM(ISNULL(LibraryCustomers.CustomerType, '')))) > 0" & vbCrLf &
            " ORDER BY LibraryCustomers.CustomerType"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibrarySalesChannels", pDT)
        mTotalRecords += pDT.Rows.Count

        '-- after import:
        '-- Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMPriceLevel'
    End Sub

    Private Sub ExportCustomerSalesReps()

        Dim pDT As DataTable
        Dim pSql As String
        Dim pTop As String = "100 PERCENT"
        If mHasExportableProductIDs Then
            pTop = "100 PERCENT"
        Else
            pTop = "0"
        End If
        pSql = " SELECT DISTINCT TOP " & pTop & " LibraryCustomersAgents.CustomerID" & vbCrLf &
            " ,LibraryCustomersAgents.EmployeeID" & vbCrLf &
            " ,LibraryCustomersAgents.LastUpdated" & vbCrLf &
            " FROM LibraryCustomersAgents" & vbCrLf &
            " ORDER BY LibraryCustomersAgents.LastUpdated"
        pDT = mProData.Execute(pSql)
        AppendTable("LibraryOrganizationCustomerSalesReps", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportBusinessIndustries()

        '-- source: LibraryCustomers.CustomerType
        '-- destination: MobizLibrarySalesChannels

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT LibraryBusinessTypes.ID" & vbCrLf &
            ",LibraryBusinessTypes.BusinessType" & vbCrLf &
            " FROM LibraryBusinessTypes" & vbCrLf &
            " JOIN  LibraryCustomers ON LibraryCustomers.BusinessTypeID = LibraryBusinessTypes.ID" & vbCrLf &
            " WHERE LEN(LTRIM(RTRIM(ISNULL(LibraryBusinessTypes.BusinessType, '')))) > 0" & vbCrLf &
            " ORDER BY LibraryBusinessTypes.BusinessType"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibrarySalesChannels", pDT)
        mTotalRecords += pDT.Rows.Count

        '-- after import:
        '-- Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMPriceLevel'
    End Sub

    Private Sub ExportLibraryCustomers()
        '-- filename: ExportLibraryCustomers
        '-- source: LibraryCustomers
        '-- destination: MobizLibraryOrganization

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT " & vbCrLf &
            " LibraryCustomers.ID" & vbCrLf &
            " ,CompanyName" & vbCrLf &
            " ,TinNumber" & vbCrLf &
            " ,ContactNumber -- to MobizLibraryOrganizationContactTel.ContactNo" & vbCrLf &
            " ,FaxNumber -- to MobizLibraryOrganizationContactFax.ContactNo" & vbCrLf &
            " ,EmailAddress -- to MobizLibraryOrganizationContactEmail.EmailAddress" & vbCrLf &
            " ,LibraryCustomers.UserID" & vbCrLf &
            " ,IsFranchisor" & vbCrLf &
            " ,RegistrationType" & vbCrLf &
            " ,RegistrationNo" & vbCrLf &
            " ,RegistrationDate" & vbCrLf &
            " ,OfficeAddressBuildingNo" & vbCrLf &
            " ,OfficeAddressStreet" & vbCrLf &
            " ,OfficeAddressCityID" & vbCrLf &
            " ,OfficeAddressCountryID" & vbCrLf &
            " ,BusinessAddressBuildingNo" & vbCrLf &
            " ,BusinessAddressStreet" & vbCrLf &
            " ,BusinessAddressCityID" & vbCrLf &
            " ,BusinessAddressCountryID" & vbCrLf &
            " ,Purpose" & vbCrLf &
            " ,BusinessScope" & vbCrLf &
            " ,NoOfRetailOutlet" & vbCrLf &
            " ,NoOfOffice" & vbCrLf &
            " ,NoOfWarehouse" & vbCrLf &
            " ,NoOfEmployees" & vbCrLf &
            " ,IsHasHoldingCompany" & vbCrLf &
            " ,HoldingCompanyID" & vbCrLf &
            " ,IsOrganization" & vbCrLf &
            " ,PersonTitleID" & vbCrLf &
            " ,PersonSurname" & vbCrLf &
            " ,PersonFirstName" & vbCrLf &
            " ,PersonMiddleName" & vbCrLf &
            " ,PersonSuffixName" & vbCrLf &
            " ,PersonDateOfBirth" & vbCrLf &
            " ,PersonMotherMaidenSurname" & vbCrLf &
            " ,PersonMotherMaidenFirstName" & vbCrLf &
            " ,PersonMotherMaidenMiddleName" & vbCrLf &
            " ,LibraryCustomers.LastUpdated" & vbCrLf &
            " ,LibraryCustomers.CreditLimit" & vbCrLf &
            " ,LibraryCustomers.CustomerType" & vbCrLf &
            " ,LibraryCustomers.BusinessTypeID" & vbCrLf &
            " ,LibraryBusinessTypes.BusinessType" & vbCrLf &
            " ,tblTerms.Term" & vbCrLf &
            " ,tblTerms.TermDescription" & vbCrLf &
            " ,1 AS IsCustomer" & vbCrLf &
            " ,0 AS IsSupplier" & vbCrLf &
            ",ROW_NUMBER() OVER (ORDER BY LibraryCustomers.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", LibraryCustomers.ID) AS rn" & vbCrLf &
            " FROM LibraryCustomers" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            " 	SELECT TOP 1 LibraryTerms.Term" & vbCrLf &
            "   ,LibraryTerms.Description AS TermDescription" & vbCrLf &
            " 	FROM LibraryTerms" & vbCrLf &
            " 	JOIN LibraryCustomersTerms ON LibraryCustomersTerms.TermsID = LibraryTerms.ID" & vbCrLf &
            " 	WHERE ISNUMERIC(Term) = 1" & vbCrLf &
            "     AND LibraryCustomersTerms.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " 	ORDER BY CONVERT(decimal, Term) DESC" & vbCrLf &
            "  ) tblTerms" & vbCrLf
        pSql &= " LEFT JOIN LibraryBusinessTypes ON LibraryBusinessTypes.ID = LibraryCustomers.BusinessTypeID" & vbCrLf
#If DEBUG Then
        'pSql &= " WHERE LibraryCustomers.ID IN ('4015e346-3880-420e-8de3-f5e84f2a6043')" & vbCrLf ' HERTZ MARKETING
#End If
        pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
        pSql &= "SELECT * " & vbCrLf &
            " INTO #tblCustomers" & vbCrLf &
            " FROM t" & vbCrLf &
            " WHERE rn BETWEEN " & mSettings.LibraryCustomersLastRowNumber & " AND " & (mSettings.LibraryCustomersLastRowNumber + mTop - 1) & vbCrLf
        pSql &= " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblCustomers"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganization", pDT)
        mSettings.LibraryCustomersLastRowNumber += pDT.Rows.Count
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExporLibraryCustomersRepresentatives()
        '-- filename: ExporLibraryCustomersRepresentatives
        '-- source: LibraryCustomersRepresentatives
        '-- destination: MobizLibraryOrganizationContactPerson

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryCustomersRepresentatives.ID" & vbCrLf &
            " ,LibraryCustomersRepresentatives.CustomerID AS OrganizationID" & vbCrLf &
            " ,LibraryCustomersRepresentatives.LastName" & vbCrLf &
            " ,LibraryCustomersRepresentatives.FirstName" & vbCrLf &
            " ,LibraryCustomersRepresentatives.MiddleName" & vbCrLf &
            " ,LibraryCustomersRepresentatives.UserID" & vbCrLf &
            " ,LibraryCustomersRepresentatives.LastUpdated" & vbCrLf &
            " INTO #tblCustomerPersons" & vbCrLf &
            " FROM LibraryCustomersRepresentatives" & vbCrLf &
            " WHERE LibraryCustomersRepresentatives.CustomerID IN (SELECT ID FROM #tblCustomers)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblCustomerPersons"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganizationContactPerson", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryCities()
        '-- filename: ExportLibraryCities
        '-- source: LibraryCities
        '-- destination: MobizLibraryCities

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT *" & vbCrLf &
            " ,ROW_NUMBER() OVER (ORDER BY t.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", t.ID) AS rn" & vbCrLf &
            " FROM (" & vbCrLf &
            "  SELECT ID" & vbCrLf &
            "  ,[Name]" & vbCrLf &
            "  ,Zip" & vbCrLf &
            "  ,UserID" & vbCrLf &
            "  ,LastUpdated" & vbCrLf &
            "  ,IsActive" & vbCrLf &
            "  FROM LibraryCities" & vbCrLf &
            "  UNION" & vbCrLf &
            " SELECT ID" & vbCrLf &
            " ,LibraryLocations.LocationName" & vbCrLf &
            " ,0" & vbCrLf &
            " ,UserID" & vbCrLf &
            " ,LastUpdated" & vbCrLf &
            " ,IsActive" & vbCrLf &
            " FROM LibraryLocations" & vbCrLf &
            " ) t " & vbCrLf
        pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
        pSql &= "SELECT * " & vbCrLf &
            " FROM t" & vbCrLf &
            " WHERE rn BETWEEN " & mSettings.LibraryCitiesLastRowNumber & " AND " & (mSettings.LibraryCitiesLastRowNumber + mTop - 1) & vbCrLf &
            " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC ", "") & ", ID"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryCities", pDT)
        mSettings.LibraryCitiesLastRowNumber += pDT.Rows.Count
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryCustomersAddresses()
        '-- filename: ExportLibraryCustomersAddresses
        '-- source: LibraryCustomersAddresses
        '-- destination: MobizLibraryOrganizationFOWST

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryCustomersAddresses.ID" & vbCrLf &
            " ,LibraryCustomersAddresses.CustomerID AS OrganizationID" & vbCrLf &
            " ,LibraryCustomersAddresses.StreetNumber" & vbCrLf &
            " ,LibraryCustomersAddresses.Street" & vbCrLf &
            " ,LibraryStreetNames.StreetName" & vbCrLf &
            " ,LibraryCustomersAddresses.UserID" & vbCrLf &
            " ,LibraryCustomersAddresses.LastUpdated" & vbCrLf &
            " ,LibraryCustomersAddresses.Code" & vbCrLf &
            " ,LibraryCustomersAddresses.[Name]" & vbCrLf &
            " ,LibraryCustomersAddresses.Description" & vbCrLf &
            " ,LibraryCustomersAddresses.BusinessTradeNameID" & vbCrLf &
            "  ,COALESCE(LibraryCustomersAddresses.AddressCityID" & vbCrLf &
            " 	,LibraryDistricts.CityID" & vbCrLf &
            "     ,LibraryCustomersAddresses.LocationID" & vbCrLf &
            "  ) AS FowstCityID " & vbCrLf &
            " ,LibraryCustomersAddresses.LocationID" & vbCrLf &
            " ,LibraryCustomersAddresses.AddressCountryID AS FowstCountryID" & vbCrLf &
            " ,LibraryCustomersAddresses.PlateNo" & vbCrLf &
            " ,LibraryCustomersAddresses.NoOfWheels" & vbCrLf &
            " ,LibraryCustomersAddresses.BodyColor" & vbCrLf &
            " ,LibraryCustomersAddresses.Weight" & vbCrLf &
            " ,LibraryCustomersAddresses.Dimension" & vbCrLf &
            " ,LibraryCustomersAddresses.OwnershipType" & vbCrLf &
            " ,LibraryCustomersAddresses.ContractStart" & vbCrLf &
            " ,LibraryCustomersAddresses.ContractEnd" & vbCrLf &
            " ,LibraryCustomersAddresses.CostPerMonth" & vbCrLf &
            " ,LibraryCustomersAddresses.IsMortgaged" & vbCrLf &
            " ,LibraryCustomersAddresses.IsFranchisee" & vbCrLf &
            " ,LibraryCustomersAddresses.FranchisorID" & vbCrLf &
            " INTO #tblCustomersAddresses" & vbCrLf &
            " FROM LibraryCustomersAddresses" & vbCrLf &
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" & vbCrLf &
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & vbCrLf &
            " WHERE LibraryCustomersAddresses.CustomerID IN (SELECT ID FROM #tblCustomers)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblCustomersAddresses"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganizationFOWST", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryCustomersPriceLevel()
        '-- filename: ExportLibraryCustomersPriceLevel
        '-- source: LibraryCustomersPriceLevel
        '-- destination: LibraryOrganizationPriceLevel

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT ID" & vbCrLf &
            " ,CustomerID AS OrganizationID" & vbCrLf &
            " ,PriceLevelID" & vbCrLf &
            " ,LastUpdated" & vbCrLf &
            " ,UserID" & vbCrLf &
            " INTO #tblCustomersPriceLevel" & vbCrLf &
            " FROM LibraryCustomersPriceLevel" & vbCrLf &
            " WHERE LibraryCustomersPriceLevel.CustomerID IN (SELECT ID FROM #tblCustomers)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblCustomersPriceLevel"
        pDT = mProData.Execute(pSql)
        AppendTable("LibraryOrganizationPriceLevel", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibrarySuppliers()
        '-- filename: ExportLibrarySuppliers
        '-- source: LibrarySuppliers
        '-- destination: MobizLibraryOrganization

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT " & vbCrLf &
            " ID" & vbCrLf &
            " ,SupplierName AS CompanyName" & vbCrLf &
            " ,TIN AS TinNumber" & vbCrLf &
            " ,ContactNumber -- to MobizLibraryOrganizationContactTel.ContactNo" & vbCrLf &
            " ,EmailAddress -- to MobizLibraryOrganizationContactEmail.EmailAddress" & vbCrLf &
            " ,UserID" & vbCrLf &
            " ,LibrarySuppliers.LastUpdated" & vbCrLf &
            " ,LibrarySuppliers.Terms AS Term" & vbCrLf &
            " ,LibrarySuppliers.CreditLimit" & vbCrLf &
            " ,0 AS IsCustomer" & vbCrLf &
            " ,1 AS IsSupplier" & vbCrLf &
            ",ROW_NUMBER() OVER (ORDER BY LibrarySuppliers.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", LibrarySuppliers.ID) AS rn" & vbCrLf &
            " FROM LibrarySuppliers" & vbCrLf
#If DEBUG Then
        pSql &= " WHERE LibrarySuppliers.ID IN ('366EA905-A294-4F28-A85C-5B6A58F42D3C')" & vbCrLf
#End If
        pSql = "WITH t AS (" & vbCrLf & pSql & ")" & vbCrLf
        pSql &= "SELECT * " & vbCrLf &
            " INTO #tblSuppliers" & vbCrLf &
            " FROM t" & vbCrLf &
            " WHERE rn BETWEEN " & mSettings.LibrarySuppliersLastRowNumber & " AND " & (mSettings.LibrarySuppliersLastRowNumber + mTop - 1) & vbCrLf
        pSql &= " ORDER BY LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", ID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblSuppliers"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganization", pDT)
        mSettings.LibrarySuppliersLastRowNumber += pDT.Rows.Count
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibrarySuppliersAddress()
        '-- filename: ExportLibrarySuppliersAddress
        '-- source: LibrarySuppliersAddress
        '-- destination: MobizLibraryOrganizationFOWST

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibrarySuppliersAddress.ID" & vbCrLf &
            " ,LibrarySuppliersAddress.SupplierID AS OrganizationID" & vbCrLf &
            " ,LibrarySuppliersAddress.StreetNumber" & vbCrLf &
            " ,LibrarySuppliersAddress.Street" & vbCrLf &
            " ,LibrarySuppliersAddress.UserID" & vbCrLf &
            " ,LibrarySuppliersAddress.LastUpdated" & vbCrLf &
            " ,LibrarySuppliersAddress.LocationID AS FowstCityID " & vbCrLf &
            " INTO #tblSuppliersAddresses" & vbCrLf &
            " FROM LibrarySuppliersAddress" & vbCrLf &
            " WHERE LibrarySuppliersAddress.SupplierID IN (SELECT ID FROM #tblSuppliers)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblSuppliersAddresses"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganizationFOWST", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibrarySupplierPerson()
        '-- filename: ExportLibrarySupplierPerson
        '-- source: LibrarySuppliersRepresentatives
        '-- destination: MobizLibraryOrganizationContactPerson

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibrarySuppliersRepresentatives.ID" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.SupplierID AS OrganizationID" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.LastName" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.FirstName" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.MiddleName" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.UserID" & vbCrLf &
            " ,LibrarySuppliersRepresentatives.LastUpdated" & vbCrLf &
            " INTO #tblSupplierPersons" & vbCrLf &
            " FROM LibrarySuppliersRepresentatives" & vbCrLf &
            " WHERE LibrarySuppliersRepresentatives.SupplierID IN (SELECT ID FROM #tblSuppliers)"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tblSupplierPersons"
        pDT = mProData.Execute(pSql)
        AppendTable("MobizLibraryOrganizationContactPerson", pDT)
        mTotalRecords += pDT.Rows.Count

    End Sub

    Private Sub ExportLibraryVendorPrices()
        ' destination: LibraryVendorPrices

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "  with cte as (" & vbCrLf &
            "  SELECT LibrarySupplierStocks.SupplierID AS VendorID" & vbCrLf &
            "  ,LibrarySupplierStocks.StockID AS ProductID" & vbCrLf &
            "  ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf &
            "  ,LibrarySupplierStockPrices.ActualQuantity" & vbCrLf &
            "  ,LibrarySupplierStockPrices.Price" & vbCrLf &
            "  ,LibrarySupplierStockPrices.UserID" & vbCrLf &
            "  FROM LibrarySupplierStocks" & vbCrLf &
            "  JOIN LibrarySupplierStockPrices ON LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf &
            "  )" & vbCrLf &
            " SELECT * FROM (" & vbCrLf &
            "  select TOP 100 PERCENT cte.VendorID" & vbCrLf &
            "  ,cte.ProductID" & vbCrLf &
            "  ,tbl.LastUpdated" & vbCrLf &
            "  ,tbl.ActualQuantity" & vbCrLf &
            "  ,tbl.Price" & vbCrLf &
            "  ,tbl.UserID" & vbCrLf &
            "  ,ROW_NUMBER() OVER (ORDER BY tbl.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", cte.VendorID, cte.ProductID) AS rn" & vbCrLf &
            "  from cte" & vbCrLf &
            "  CROSS APPLY (" & vbCrLf &
            "  	SELECT TOP 1 cte2.LastUpdated" & vbCrLf &
            "      ,cte2.ActualQuantity" & vbCrLf &
            "      ,cte2.Price" & vbCrLf &
            "      ,cte2.UserID" & vbCrLf &
            "      FROM cte cte2" & vbCrLf &
            "      WHERE cte.ProductID = cte2.ProductID" & vbCrLf &
            "      AND cte.VendorID = cte2.VendorID" & vbCrLf &
            "      ORDER BY cte2.LastUpdated DESC" & vbCrLf &
            "  ) tbl" & vbCrLf
        Dim pExportableProductIDs = GetExportableVendorPriceProductIDs()
        If String.IsNullOrWhiteSpace(pExportableProductIDs) = False Then
            pSql &= " WHERE cte.ProductID IN (" & pExportableProductIDs & ")" & vbCrLf
        ElseIf mSettings.LibraryVendorPricesLastUpdated IsNot Nothing Then
            pSql &= " WHERE tbl.LastUpdated > " & ReadyString(Format(mSettings.LibraryVendorPricesLastUpdated, cGlobal.FormatDateTimeSql2)) & vbCrLf
        End If
        pSql &= "  GROUP BY cte.VendorID" & vbCrLf &
            "  ,cte.ProductID" & vbCrLf &
            "  ,tbl.LastUpdated" & vbCrLf &
            "  ,tbl.ActualQuantity" & vbCrLf &
            "  ,tbl.UserID" & vbCrLf &
            "  ,tbl.Price" & vbCrLf
        pSql &= " ORDER BY tbl.LastUpdated " & If(LastestRecordsFirst, "DESC", "") & ", cte.VendorID, cte.ProductID" & vbCrLf &
            "  ) t" & vbCrLf
        If String.IsNullOrWhiteSpace(pExportableProductIDs) Then
            pSql &= " WHERE rn BETWEEN " & mSettings.LibraryVendorPricesLastRowNumber & " AND " & (mSettings.LibraryVendorPricesLastRowNumber + mTop - 1) & vbCrLf
        End If

        'mProData.ExecuteSQL(pSql)
        'AppendToLog("vbV43SKRM2MJ! ErpToMobizExporter.ExportLibraryVendorPrices " & pSql, , ErpToMobizExporter.LogFilename)
        mProData.CommandTimeout = 10 * 60
        pDT = mProData.Execute(pSql)
        AppendTable("LibraryVendorPrices", pDT)
        If pDT.Rows.Count > 0 Then
            mSettings.LibraryVendorPricesLastRowNumber += pDT.Rows.Count
            Dim pDR As DataRow = pDT.Rows(pDT.Rows.Count - 1)
            If pDR("LastUpdated") IsNot DBNull.Value Then
                If mSettings.LibraryVendorPricesLastUpdated <> pDR("LastUpdated") Then
                    mSettings.LibraryVendorPricesLastUpdated = pDR("LastUpdated")
                    mSettings.LibraryVendorPricesLastRowNumber = 1
                End If
            End If
        Else
            Debug.Print("vbPXF86FZQRT! no record to export")
        End If
        mTotalRecords += pDT.Rows.Count

        ' on import:
        ' UserID = (SELECT TOP 1 ID FROM Mobiz.dbo.LibraryPeople Where DUCNumericID = #tblPim.[UserID])  

        ' after import:
        ' Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'
        ' Update MobizDUC.dbo.TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'

    End Sub

    Private Sub ExportRowCount()

        Dim pTotalRowCount As Integer

        pTotalRowCount = CDec2(mDtRowCount.Compute("SUM(Count)", ""))
        AddTableRowCount("TotalRowCount", pTotalRowCount)
        mSqlite.UpdateTable("RowCount", mDtRowCount)

    End Sub

    Public ReadOnly Property TotalRecords As Integer
        Get

            Return mTotalRecords

        End Get
    End Property

    Public Class SettingsClass
        Public LibraryStockInformationLastRowNumber As Integer = 1
        Public LibraryStockInformationLastUpdated As Date?
        Public LibraryPriceLevelsLastRowNumber As Integer = 1
        Public LibraryCustomersLastRowNumber As Integer = 1
        Public LibrarySuppliersLastRowNumber As Integer = 1
        Public LibraryCitiesLastRowNumber As Integer = 1
        Public LibraryVendorPricesLastRowNumber As Integer = 1
        Public LibraryVendorPricesLastUpdated As Date?

        Public Shared Function FromRawString(vString) As SettingsClass

            Dim pOut As SettingsClass

            pOut = JsonConvert.DeserializeObject(Of SettingsClass)(vString)
            If pOut Is Nothing Then
                pOut = New SettingsClass()
            End If

            Return pOut

        End Function

        Public Shadows Function ToString() As String

            Dim pOut As String

            pOut = JsonConvert.SerializeObject(Me, Formatting.Indented)

            Return pOut

        End Function
    End Class
End Class
Imports System.IO
Imports System.Threading

Public Class CsvDb
    Dim mDbPath As String
    Dim mProData As ProData
    Const mLogFile As String = "CsvDb"
    Const mLogFileReimport As String = "CsvDbReimport"
    Const mSourceTempUploadCsvZipFilename As String = "SourceTempUploadCsvZipFilename"
    Public SourceUploadPath As String

    Public Function ImportOrganizations() As Boolean

        Const pMinDate As Date = #9/27/2014#
        Dim pDirectoryInfo As IO.DirectoryInfo
        Dim pDrOrg As DataRow
        Dim pDbName As String

        If VerifyRequirements("") = False Then Exit Function
        pDirectoryInfo = New IO.DirectoryInfo(SourceUploadPath)
        For Each pOrg As IO.DirectoryInfo In pDirectoryInfo.GetDirectories()
            If IsGuid(pOrg.Name) = False Then
                AppendToLog("vbW9H6PP3D47! " & pOrg.FullName & " is not a GUID", , mLogFile)
                Continue For
            End If
            pDrOrg = WebRequest.Instance.Business.LibraryOrganization.ByID(pOrg.Name)
            If pDrOrg Is Nothing Then
                AppendToLog("vbRSCU2H74X3! " & pOrg.Name & " does not exist in LibraryOrganization", , mLogFile)
                Continue For
            End If
            pDbName = "Csv" & RemoveSymbols(pDrOrg("Database"))
            If Common.WebRequest.ProData.GetDatabases(True, True).Contains(pDbName.ToLower()) Then
                AppendToLog("vb8WN56KYU2N! " & pOrg.Name & " already has a database " & pDbName, , mLogFile)
            Else
                AppendToLog("vb3UXDAQQAYV! " & pOrg.Name & " is yet to have a database " & pDbName & ";" & vbCrLf &
                    "pDbName: " & pDbName & "; " & vbCrLf &
                    "mDbPath: " & mDbPath & ";", , mLogFile)
                Common.WebRequest.ProData.CreateDatabase(pDbName, mDbPath)
                AppendToLog("vb3597EGPQ4S! Successfully created database " & pDbName & " to " & mDbPath, , mLogFile)
            End If
            mProData = ProData.CreateFromConnectionInfo(
                Common.ConnectionInfo.Server,
                EscapeSqlName(pDbName),
                Common.ConnectionInfo.Username,
                Common.ConnectionInfo.Password,
                Common.ConnectionInfo.IntegratedSecurity)
            mProData.ExecuteSQL("USE " & EscapeSqlName(pDbName))
            For Each pDevice As IO.DirectoryInfo In pOrg.GetDirectories()
                If IsGuid(pDevice.Name) = False Then Continue For
                For Each pFile As IO.FileInfo In pDevice.GetFiles()
                    If IO.Path.GetFileNameWithoutExtension(pFile.Name).Length = 25 Then
                        If pFile.LastWriteTime > pMinDate Then
                            AppendToLog("vbPDP37S2EFD! Opening " & pFile.FullName & "; database = " & pDbName, , mLogFile)
                            OpenFile(pFile.FullName)
                        End If
                    End If
                Next pFile
            Next pDevice
        Next pOrg
        AppendToLog("vb8HLX5JNJC2! Done", , mLogFile)

        Return True

    End Function

    Private Function OpenFile(vFilename As String) As Boolean

        OpenTable(vFilename, "TransInvoice")
        OpenTable(vFilename, "TransInvoiceItems")
        OpenTable(vFilename, "TransInvoiceDiscount")
        OpenTable(vFilename, "TransInvoiceSeniorCitizenPWDCustomer")
        OpenTable(vFilename, "TransInvoiceItemSerials")
        OpenTable(vFilename, "TransInvoiceItemSalesReps")
        OpenTable(vFilename, "TransCustomerPayment")
        OpenTable(vFilename, "TransCustomerPaymentInvoices")
        OpenTable(vFilename, "TransCustomerPaymentDebitCard")
        OpenTable(vFilename, "TransCustomerPaymentCreditCard")
        OpenTable(vFilename, "LibraryOrganization")
        OpenTable(vFilename, "LibraryOrganizationCustomer")
        OpenTable(vFilename, "LibraryOrganizationPriceLevel")
        OpenTable(vFilename, "LibraryOrganizationFOWST")
        OpenTable(vFilename, "LibraryFOWSTBinProduct")
        OpenTable(vFilename, "TransInventoryProductCount")
        OpenTable(vFilename, "TransInventoryProductCountStock")
        OpenTable(vFilename, "LibraryFOWSTBin")
        OpenTable(vFilename, "TransReceiving")
        OpenTable(vFilename, "TransReceivingStock")

    End Function

    Private Function OpenTable(vFilename As String, vTableName As String) As Boolean

        Dim pReader As CsvReader
        Dim pDtColumns As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pLackingColumns As List(Of String)
        Dim pCount As Integer

        pReader = New CsvReader(vFilename, vTableName)
        Do
            pLine = pReader.ReadLine()
            If pLine Is Nothing Then Exit Do
            If pDtColumns Is Nothing Then
                pDtColumns = mProData.ColumnsInformation(vTableName)
            End If
            pLackingColumns = GetLackingColumns(pLine, pDtColumns)
            If pLackingColumns.Count > 0 Then
                pDtColumns = Nothing
                CreateOrAlterTable(vTableName, pLackingColumns)
            End If
            InsertOrUpdate(vTableName, pLine, vFilename)
            pCount += 1
        Loop
        AppendToLog("vbYA4SPJZUHD! Inserted to " & vTableName & " = " & pCount, , mLogFile)

    End Function

    Private Function GetLackingColumns(vLine As Dictionary(Of String, String), vDtColumns As DataTable) As List(Of String)

        Dim pOut As List(Of String)

        pOut = New List(Of String)
        For Each pLineColumn As KeyValuePair(Of String, String) In vLine
            vDtColumns.DefaultView.RowFilter = "Name = " & ReadyString(pLineColumn.Key)
            If vDtColumns.DefaultView.Count = 0 Then
                pOut.Add(pLineColumn.Key)
            End If
        Next pLineColumn

        vDtColumns.DefaultView.RowFilter = "Name = " & ReadyString(mSourceTempUploadCsvZipFilename)
        If vDtColumns.DefaultView.Count = 0 Then
            pOut.Add(mSourceTempUploadCsvZipFilename)
        End If

        Return pOut

    End Function

    Private Function CreateOrAlterTable(vTableName As String, vColumns As List(Of String))

        Dim pSql As String = ""

        For Each pColumn As String In vColumns
            If pSql.Length > 0 Then
                pSql &= ", "
            End If
            pSql &= EscapeSqlName(pColumn) & " varchar(MAX) NULL" & vbCrLf
        Next pColumn

        If mProData.TableExist(vTableName) Then
            pSql = "ALTER TABLE " & EscapeSqlName(vTableName) & " ADD" & vbCrLf &
                pSql
        Else
            pSql = "CREATE TABLE " & EscapeSqlName(vTableName) & "(" & vbCrLf &
                pSql & ")"
        End If
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function InsertOrUpdate(vTableName As String, vLine As Dictionary(Of String, String), vFilename As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim p = (From pPair As KeyValuePair(Of String, String) In vLine
            Where pPair.Key.ToLower() = "id"
            Select pPair.Value).ToArray()

        mProData.BeginTransaction()
        pSql = "DELETE FROM " & EscapeSqlName(vTableName) &
            " WHERE ID = " & ReadyString(p(0))
        mProData.Execute(pSql)

        pIU = New InsertUpdate(EscapeSqlName(vTableName))
        pIU(mSourceTempUploadCsvZipFilename) = ReadyString(vFilename)
        For Each pPair As KeyValuePair(Of String, String) In vLine
            pIU(EscapeSqlName(pPair.Key)) = ReadyString(pPair.Value)
        Next pPair
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)
        mProData.CommitTransaction()

    End Function

    Public Function VerifyRequirements(ByRef oMessage As String) As Boolean

        Dim pDrives As IO.DriveInfo()
        Const pRequredFreeSpace As Long = 5L * 1024L * 1024L * 1024L
        Dim pChoosenDrive As String
        Dim pMessage As String

        oMessage = ""
        pDrives = IO.DriveInfo.GetDrives()
        For Each pDrive As IO.DriveInfo In pDrives
            If pDrive.IsReady = False Then Continue For
            oMessage &= "Drive" & pDrive.Name & " has a free space of " & DisplayableSize(pDrive.AvailableFreeSpace)
            If pDrive.AvailableFreeSpace >= pRequredFreeSpace Then
                oMessage &= " and meets the minimum required free space of " & DisplayableSize(pRequredFreeSpace)
                mDbPath = pDrive.Name & "databases\"
                CreateFolder(mDbPath)
                pChoosenDrive = pDrive.Name
            Else
                oMessage &= " and fails to meet the minimum required free space of " & DisplayableSize(pRequredFreeSpace)
            End If
            oMessage &= ". "
        Next pDrive

        If pChoosenDrive Is Nothing Then
            Return False
        End If

        If CheckSettings(pMessage) = False Then
            oMessage &= pMessage
            Return False
        End If
        oMessage &= pMessage

        Return True

    End Function

    Private Function CheckSettings(ByRef oMessage As String) As Boolean

        Dim pFilename As String

        oMessage = ""
        pFilename = Common.ServerPath & "settings\CsvDb.txt"
        If IO.File.Exists(pFilename) = False Then
            oMessage = "File, " & pFilename & ", does not exist."
            Return False
        End If
        SourceUploadPath = GetFileText(pFilename, 1000)
        oMessage &= "File, " & pFilename & ", contains " & SourceUploadPath & "."
        If IO.Directory.Exists(SourceUploadPath) = False Then
            oMessage &= "Folder, " & SourceUploadPath & ", does not exist."
            Return False
        End If

        Return True

    End Function

    Public Function CompareDatabase(vDbName As String)

        Dim pSql As String
        Dim pDtTable As DataTable
        Dim pDtResult As DataTable
        Dim pResultFilename As String
        Dim pOtherTable As DataTable

        mProData = ProData.CreateFromConnectionInfo(
            Common.ConnectionInfo.Server,
            EscapeSqlName("CSV" & vDbName),
            Common.ConnectionInfo.Username,
            Common.ConnectionInfo.Password,
            Common.ConnectionInfo.IntegratedSecurity)
        'If vDbName.StartsWith("CSV", True, Globalization.CultureInfo.InvariantCulture) = False Then
        '    Throw New ArgumentException("CompareDatabase.vDbName must start with CSV. You passed " & vDbName)
        'End If

        pSql = "SELECT sysobjects.ID, sysobjects.Name" & _
            ",sysusers.name AS Owner" & _
            " FROM sysobjects" & _
            " JOIN sysusers" & _
            " ON sysusers.uid = sysobjects.uid" & _
            " WHERE xtype = 'U'" & _
            " AND sysobjects.status >= 0" & _
            " ORDER BY sysobjects.Name"
        pDtTable = mProData.Execute(pSql)
        For Each pDrTable As DataRow In pDtTable.Rows
            pSql = "SELECT " & EscapeSqlName(vDbName) & ".dbo.sysobjects.ID" &
                "," & EscapeSqlName(vDbName) & ".dbo.sysobjects.Name" & _
                "," & EscapeSqlName(vDbName) & ".dbo.sysusers.name AS Owner" & _
                " FROM " & EscapeSqlName(vDbName) & ".dbo.sysobjects" & _
                " JOIN " & EscapeSqlName(vDbName) & ".dbo.sysusers" & _
                " ON " & EscapeSqlName(vDbName) & ".dbo.sysusers.uid = " & EscapeSqlName(vDbName) & ".dbo.sysobjects.uid" & _
                " WHERE xtype = 'U'" & _
                " AND " & EscapeSqlName(vDbName) & ".dbo.sysobjects.status >= 0" & _
                " AND " & EscapeSqlName(vDbName) & ".dbo.sysobjects.Name = " & ReadyString(pDrTable("Name")) & vbCrLf &
                " ORDER BY " & EscapeSqlName(vDbName) & ".dbo.sysobjects.Name"
            pOtherTable = mProData.Execute(pSql)
            If pOtherTable.Rows.Count = 0 Then
                AppendToLog("vbCFWPBSB699! Database, " & pDrTable("Name") & ", does not exist on the original private database. Thus, excluded from comparison", , mLogFile)
                Continue For
            End If
            pDtResult = CompareTable(vDbName, pDrTable("Name"))
            If pDtResult.Rows.Count > 0 Then
                pResultFilename = Common.ServerPath & "temp\CsvDbCompare_result\" & CreateValidFilename(vDbName) & "\" & CreateValidFilename(pDrTable("Name")) & ".xml"
                If IO.File.Exists(pResultFilename) = False Then
                    CreateFolder(pResultFilename)
                    pDtResult.WriteXml(pResultFilename, XmlWriteMode.WriteSchema)
                Else
                    AppendToLog("vbXWMVPJGJAN! File, " & pResultFilename & ", already exist.", , mLogFile)
                End If
            End If
        Next pDrTable

    End Function

    Private Function CompareTable(vDbName As String, vTableName As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pOtherTableName As String
        Dim pAlias As String

        vTableName = EscapeSqlName(vTableName)
        pOtherTableName = EscapeSqlName(vDbName) & ".dbo." & vTableName
        pAlias = EscapeSqlName(vTableName) & "_Orig"
        pSql = "SELECT " & vTableName & ".ID" & vbCrLf &
            "," & vTableName & "." & mSourceTempUploadCsvZipFilename & vbCrLf
        If vTableName.Equals("TransInvoiceItems", StringComparison.InvariantCultureIgnoreCase) Then
            pSql &= "," & vTableName & ".InvoiceID" & vbCrLf
        End If
        pSql &= " FROM " & vTableName & vbCrLf &
            " LEFT JOIN " & pOtherTableName & " " & pAlias & " ON " & pAlias & ".ID = " & vTableName & ".ID" & vbCrLf &
            " WHERE " & pAlias & ".ID IS NULL"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT
        End If
        Return pDT

    End Function

    Public Function Reimport(ByRef oMessage As String)

        Dim pDirectoryInfo As DirectoryInfo
        Dim pDT As DataTable

        If VerifyRequirements(oMessage) = False Then Exit Function
        AppendToLog("vbXUFJUPZDLH! Reimport start", , mLogFileReimport)
        pDirectoryInfo = New DirectoryInfo(Common.ServerPath & "temp\CsvDbCompare_result\")
        For Each pDb As DirectoryInfo In pDirectoryInfo.GetDirectories()
            AppendToLog("vbMTYMAGXHLZ! DB folder: " & pDb.Name, , mLogFileReimport)
            For Each pTable As FileInfo In pDb.GetFiles("*.xml")
                AppendToLog("vbQWMBC5HKWX! Table folder: " & pTable.Name, , mLogFileReimport)
                If IO.Path.GetFileNameWithoutExtension(pTable.Name).Equals("TransInvoiceItems", StringComparison.InvariantCultureIgnoreCase) Then
                    pDT = New DataTable
                    pDT.ReadXml(pTable.FullName)
                    ReimportInvoiceItems(pDT)
                Else
                    AppendToLog("vbCTLB39NET5! Table folder, " & pTable.Name & ", was excluded from importation", , mLogFileReimport)
                End If
            Next pTable
        Next pDb

        Return True

    End Function

    Private Function ReimportInvoiceItems(vDT As DataTable)

        Dim pDtFilename As DataTable
        Dim pDtInvoice As DataTable
        Dim pRegexObj As Regex
        Dim pMatch As Match
        Dim pImportData As ImportDataClass
        Dim pThread As Thread
        Dim pInvoiceIds As List(Of String)

        pRegexObj = New Regex(".+\\(?<OrgID>.+)\\(?<DeviceID>.+)\\.+\.zip$", RegexOptions.IgnoreCase)
        pDtFilename = vDT.DefaultView.ToTable(True, mSourceTempUploadCsvZipFilename)
        AppendToLog("vbLE77QQE6AS! pDtFilename.Rows.Count" & pDtFilename.Rows.Count, , mLogFileReimport)
        For Each pDrFilename As DataRow In pDtFilename.Rows
            AppendToLog("vbN9AXFSHB7J! pDrFilename: " & pDrFilename(0), , mLogFileReimport)
            pInvoiceIds = New List(Of String)
            pDtInvoice = New DataView(vDT, mSourceTempUploadCsvZipFilename & " = " & ReadyString(pDrFilename(0)), "", DataViewRowState.CurrentRows).ToTable(True, "InvoiceID")
            AppendToLog("vbEQLEKJZQUD! pDtInvoice.Rows.Count: " & pDtInvoice.Rows.Count, , mLogFileReimport)
            For Each pDrInvoice As DataRow In pDtInvoice.Rows
                pInvoiceIds.Add(ReadyGuid(pDrInvoice("InvoiceID")))
            Next pDrInvoice
            pMatch = pRegexObj.Match(pDrFilename(0))
            If pMatch.Success Then
                pImportData = New ImportDataClass
                pImportData.UploadsFolder = AppendBackslash(SourceUploadPath)
                pImportData.EnableLogs = True
                pImportData.AllowReimportNonZeroInvoice = True
                pImportData.ImportInvoiceOnly = True
                pImportData.InvoiceIDToImport = pInvoiceIds
                pImportData.OrgID = pMatch.Groups("OrgID").Value()
                pImportData.DeviceID = pMatch.Groups("DeviceID").Value()
                pImportData.InputFilename = IO.Path.GetFileNameWithoutExtension(pDrFilename(0))
                AppendToLog("vbKCP7F8AEPH! IO.Path.GetFileNameWithoutExtension(pDrFilename(0)): " & IO.Path.GetFileNameWithoutExtension(pDrFilename(0)), , mLogFileReimport)
                AppendToLog("vbEKU2PK2527! pImportData.OrgID: " & pImportData.OrgID, , mLogFileReimport)
                AppendToLog("vb2FH7UHXBFE! pImportData.DeviceID: " & pImportData.DeviceID, , mLogFileReimport)
                pThread = New Thread(CType(
                    Sub()
                        Thread.Sleep(Integer.MaxValue)
                    End Sub, ThreadStart))
                AddHandler pImportData.Done,
                    Sub()
                        pThread.Abort()
                    End Sub
                pImportData.Start()
                pThread.Start()
                pThread.Join()
            Else
                AppendToLog("vb9484UCTX3W! Regex must match " & pDrFilename(0), , mLogFileReimport)
            End If
        Next pDrFilename

    End Function
End Class
Public Class ImportFromSqlite

    Public DestinationTablePrefix As String
    Public DestinationProData As ProData
    Public SourceSqliteFilename As String
    ''' <summary>
    ''' dont import data whose LastUpdated is lower than this
    ''' </summary>
    ''' <remarks></remarks>
    Public MinimumDate As Date?
    Dim mProSqlite As ProSqlite
    Public RecordsToImport As Integer
    Dim mLogFileName As String
    Public TotalSqliteRecords As Long
    Public TotalSavedRecords As Long
    Public StartingTable As String
    Public StartingRowIndex As Long
    Dim mStartingTableStarted As Boolean
    Dim mStartingRowIndexStarted As Boolean
    Private ReadOnly mTables As New Dictionary(Of String, Table)
    Public Event Log(vArgs As LogArgs)
    Public Event DataImported(vArgs As DataImportedArgs)

    Public Sub Import()

        Try
            If String.IsNullOrWhiteSpace(StartingTable) Then
                mStartingTableStarted = True
                mStartingRowIndexStarted = True
            End If

            mLogFileName = DateTimeStamp()
            mProSqlite = New ProSqlite
            mProSqlite.Open(SourceSqliteFilename)

            Dim pTables = GetTables()
            EnableDisableAllForeignKeys(pTables, False)
            For Each pDR As DataRow In pTables.Rows
                If mStartingTableStarted = False Then
                    If StartingTable.Trim().ToLower = CStr2(pDR("Name")).Trim().ToLower() Then
                        mStartingTableStarted = True
                    Else
                        Continue For
                    End If
                End If

                Select Case CStr2(pDR("Name")).Trim().ToLower()
                    Case "TransCustomerPayment".ToLower(),
                        "TransCustomerPaymentCheckAmount".ToLower(),
                        "LibraryPIMProductFast".ToLower()

                        Continue For
                End Select
                CopyTableData(pDR("Name"))
            Next pDR
            EnableDisableAllForeignKeys(pTables, True)
            CompareTables(pTables)
            AddToLog("Overall done")
        Catch ex As Exception
            AppendToDebug("ImportFromSqlite.vb", True, ex, True)
        End Try

    End Sub

    Private Sub AddToLog(vText As String)

        Dim pArgs = New LogArgs
        pArgs.Text = vText
        RaiseEvent Log(pArgs)
        AppendToLog(vText, Common.ServerPath & "temp\ImportFromSqlite\", mLogFileName)

    End Sub

    Private Function GetTables() As DataTable

        Dim pSql = " SELECT " & vbCrLf &
            "     name" & vbCrLf &
            " FROM " & vbCrLf &
            "     sqlite_master " & vbCrLf &
            " WHERE " & vbCrLf &
            "     type ='table' AND " & vbCrLf &
            "     name NOT LIKE 'sqlite_%'" & vbCrLf &
            " ORDER BY name"
        Dim pOut = mProSqlite.Execute(pSql)

        Return pOut

    End Function

    Private Function CopyTableData(vTableName As String) As Integer

        Dim pTotalSqliteRecords As Long = SqliteRecordCountFiltered(vTableName)
        If pTotalSqliteRecords = 0 Then
            Return 0
        End If
        AddToLog("Copying " & Format(pTotalSqliteRecords, cGlobal.FormatInteger) & " records from " & vTableName & "...")
        Dim pRecordsSaved As Integer
        Dim pSql = "SELECT * FROM " & vTableName
        If mTables(vTableName).HasLastUpdatedField _
            AndAlso MinimumDate IsNot Nothing Then

            pSql &= " WHERE LastUpdated >= " & ReadyString(Format(MinimumDate, cGlobal.FormatDateTimeSql2))
        End If

        Dim pDT As DataTable = mProSqlite.Execute(pSql)
        pDT.TableName = vTableName
        Dim pCsvFilename = CreateCsvFromDataTable(pDT)
        CreateTable(vTableName, pDT.Clone())
        BulkInsert(pCsvFilename)
        MergeTableData(vTableName, pDT)

        Return 0

        For Each pDR As DataRow In pDT.Rows
            pRecordsSaved += 1
            If mStartingRowIndexStarted = False Then
                If StartingRowIndex < pRecordsSaved Then
                    mStartingRowIndexStarted = True
                Else
                    Continue For
                End If
            End If

            TotalSavedRecords += 1
            AddToLog("Copying " & Format(pRecordsSaved, cGlobal.FormatInteger) & " of " & Format(pTotalSqliteRecords, cGlobal.FormatInteger) & " records in " & vTableName & ". " & Format(TotalSavedRecords, cGlobal.FormatInteger) & " of " & Format(TotalSqliteRecords, cGlobal.FormatInteger) & " records overall")

            Dim pIU = New InsertUpdate(DestinationTablePrefix & vTableName)
            For Each pDC As DataColumn In pDT.Columns
                If ExcludeField(vTableName, pDC.ColumnName) Then
                    Continue For
                End If
                If IsDBNull(pDR(pDC.ColumnName)) Then
                    pIU(pDC.ColumnName) = "NULL"
                Else
                    Select Case MajorDataType(pDC.DataType)
                        Case eMajorDataType.Numeric
                            pIU(pDC.ColumnName) = CDec2(pDR(pDC.ColumnName))

                        Case Else
                            pIU(pDC.ColumnName) = ReadyString(pDR(pDC.ColumnName))

                    End Select
                End If
            Next pDC
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pCount = DestinationProData.ExecuteSQL(pSql, True)
            If pCount = 0 Then
                pSql = pIU.GetInsert()
                DestinationProData.ExecuteSQL(pSql)
            End If

            Dim pDataImportedArgs = New DataImportedArgs()
            pDataImportedArgs.Table = vTableName
            pDataImportedArgs.RowIndex = pRecordsSaved
            RaiseEvent DataImported(pDataImportedArgs)

            If RecordsToImport > 0 _
                AndAlso RecordsToImport <= pRecordsSaved Then

                Return pRecordsSaved
            End If
        Next pDR

    End Function

    Private ReadOnly mFieldTerminator As String = "~|," & Convert.ToChar(9)
    Private ReadOnly mRowTerminator As String = "`|;" ' & vbCrLf

    Private Function CreateCsvFromDataTable(vDT As DataTable) As String

        Dim pSB As New StringBuilder
        Dim pText As String

        Dim pFilename = IO.Path.GetDirectoryName(SourceSqliteFilename)
        pFilename = IO.Path.Combine(pFilename, "data.csv")

        For Each pDR As DataRow In vDT.Rows
            Dim pLine = ""
            For Each pDC As DataColumn In vDT.Columns
                If pLine.Length > 0 Then
                    pLine &= mFieldTerminator
                End If
                If pDR(pDC.ColumnName) Is DBNull.Value Then
                    pLine &= CStr2(pDR(pDC.ColumnName))
                ElseIf MajorDataType(pDC.DataType) = eMajorDataType.Date Then
                    pLine &= Format(pDR(pDC.ColumnName), cGlobal.FormatDateTimeSql2)
                Else
                    pLine &= CStr2(pDR(pDC.ColumnName))
                End If
            Next
            pSB.Append(pLine & mRowTerminator)
        Next pDR

        pText = pSB.ToString()
        FileText(pFilename) = pText

        Return pFilename

    End Function

    Private Sub CreateTable(vTableName As String, vDT As DataTable)

        Dim pSql = "SELECT TOP 0 * FROM " & vTableName
        Dim pDT = DestinationProData.GetSchema(pSql)

        For Each pDR As DataRow In pDT.Rows
            If vDT.Columns.Contains(pDR("ColumnName")) Then
                If CBool2(pDR("AllowDBNull")) = False Then
                    vDT.Columns(pDR("ColumnName")).AllowDBNull = False
                End If
            End If
        Next pDR

        DestinationProData.CreateTable("temp_BulkInsert", vDT.Clone(), True)

    End Sub

    Private Sub BulkInsert(vSourceCsvFilename As String)

        DestinationProData.CommandTimeout = 30 * 60
        Dim pSql = "BULK INSERT temp_BulkInsert" & vbCrLf &
            " FROM " & ReadyString(vSourceCsvFilename) & vbCrLf &
            " WITH (FIELDTERMINATOR = '~|,\t'" & vbCrLf &
            ",ROWTERMINATOR = '`|;')"
        DestinationProData.ExecuteSQL(pSql)

    End Sub

    Private Sub MergeTableData(vTableName As String, vDtSchema As DataTable)

        Dim pSqlUpdate As String = ""
        Dim pSqlInsert As String
        Dim pSqlInsertFields As String = ""
        Dim pSqlInsertValues As String = ""

        For Each pDC As DataColumn In vDtSchema.Columns
            If ExcludeField(vTableName, pDC.ColumnName) Then
                Continue For
            End If

            If pDC.ColumnName.ToLower() <> "id" Then
                If pSqlUpdate.Length > 0 Then
                    pSqlUpdate &= "," & vbCrLf
                End If
                pSqlUpdate &= EscapeSqlName(vTableName) & "." & EscapeSqlName(pDC.ColumnName) & " = " & "temp_BulkInsert." & EscapeSqlName(pDC.ColumnName)
            End If

            If pSqlInsertFields.Length > 0 Then
                pSqlInsertFields &= ", "
                pSqlInsertValues &= ", "
            End If
            pSqlInsertFields &= EscapeSqlName(pDC.ColumnName)
            pSqlInsertValues &= "temp_BulkInsert." & EscapeSqlName(pDC.ColumnName)
        Next pDC

        pSqlUpdate = " UPDATE SET " & pSqlUpdate
        pSqlInsert = " INSERT (" & pSqlInsertFields & ")" & vbCrLf &
            " VALUES (" & pSqlInsertValues & ")"

        DestinationProData.CommandTimeout = 30 * 60
        Dim pSql = "MERGE " & EscapeSqlName(vTableName) & vbCrLf &
            " USING temp_BulkInsert" & vbCrLf &
            " ON (" & EscapeSqlName(vTableName) & ".ID = temp_BulkInsert.ID)" & vbCrLf &
            " WHEN MATCHED THEN" & vbCrLf &
            pSqlUpdate & vbCrLf &
            " WHEN NOT MATCHED BY TARGET THEN" & vbCrLf &
            pSqlInsert & ";"
        Try
            DestinationProData.ExecuteSQL(pSql)
        Catch ex As Exception
            AddToLog("Error while merging " & vTableName & vbCrLf &
                "Error message: " & ex.Message & vbCrLf &
                "SQL: " & pSql)
            Throw
        End Try

    End Sub

    Private Function CopyTableDataOld(vTableName As String) As Integer

        Dim pTotalSqliteRecords As Long = SqliteRecordCountFiltered(vTableName)
        AddToLog("Copying " & Format(pTotalSqliteRecords, cGlobal.FormatInteger) & " records from " & vTableName & "...")
        Dim pRecordsSaved As Integer
        Dim pSql = "SELECT * FROM " & vTableName
        If mTables(vTableName).HasLastUpdatedField _
            AndAlso MinimumDate IsNot Nothing Then

            pSql &= " WHERE LastUpdated >= " & ReadyString(Format(MinimumDate, cGlobal.FormatDateTimeSql2))
        End If

        Dim pDT = mProSqlite.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            pRecordsSaved += 1
            If mStartingRowIndexStarted = False Then
                If StartingRowIndex < pRecordsSaved Then
                    mStartingRowIndexStarted = True
                Else
                    Continue For
                End If
            End If

            TotalSavedRecords += 1
            AddToLog("Copying " & Format(pRecordsSaved, cGlobal.FormatInteger) & " of " & Format(pTotalSqliteRecords, cGlobal.FormatInteger) & " records in " & vTableName & ". " & Format(TotalSavedRecords, cGlobal.FormatInteger) & " of " & Format(TotalSqliteRecords, cGlobal.FormatInteger) & " records overall")

            Dim pIU = New InsertUpdate(DestinationTablePrefix & vTableName)
            For Each pDC As DataColumn In pDT.Columns
                If ExcludeField(vTableName, pDC.ColumnName) Then
                    Continue For
                End If
                If IsDBNull(pDR(pDC.ColumnName)) Then
                    pIU(pDC.ColumnName) = "NULL"
                Else
                    Select Case MajorDataType(pDC.DataType)
                        Case eMajorDataType.Numeric
                            pIU(pDC.ColumnName) = CDec2(pDR(pDC.ColumnName))

                        Case Else
                            pIU(pDC.ColumnName) = ReadyString(pDR(pDC.ColumnName))

                    End Select
                End If
            Next pDC
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pCount = DestinationProData.ExecuteSQL(pSql, True)
            If pCount = 0 Then
                pSql = pIU.GetInsert()
                DestinationProData.ExecuteSQL(pSql)
            End If

            Dim pDataImportedArgs = New DataImportedArgs()
            pDataImportedArgs.Table = vTableName
            pDataImportedArgs.RowIndex = pRecordsSaved
            RaiseEvent DataImported(pDataImportedArgs)

            If RecordsToImport > 0 _
                AndAlso RecordsToImport <= pRecordsSaved Then

                Return pRecordsSaved
            End If
        Next pDR

    End Function

    Public Shared Function ExcludeField(vTableName As String, vFieldName As String) As Boolean

        vTableName = vTableName.ToLower()
        vFieldName = CStr2(vFieldName).ToLower()

        Select Case vTableName
            Case "LibraryOrganizationEmployee".ToLower()
                Select Case vFieldName
                    Case "PeopleNo".ToLower()
                        Return True

                End Select

            Case "TransInvoiceItems".ToLower()
                Select Case vFieldName
                    Case "COGS".ToLower(),
                        "DiscountTax".ToLower(),
                        "GrossSalesNetOfVATExemptions".ToLower(),
                        "DiscountedUnitPrice".ToLower()

                        Return True

                End Select

            Case "TransMemo".ToLower()
                Select Case vFieldName
                    Case "MemoNumber".ToLower()
                        Return True

                End Select

        End Select

    End Function

    Private Function EnableDisableAllForeignKeys(vTables As DataTable, vEnable As Boolean) As Integer

        If vEnable Then
            AddToLog("Enabling all foreign key constraints...")
        Else
            AddToLog("Disabling all foreign key constraints...")
        End If

        For Each pDR As DataRow In vTables.Rows
            EnableDisableForeignKeys(pDR("Name"), vEnable)
        Next pDR

        If vEnable Then
            AddToLog("Finished enabling all foreign key constraints")
        Else
            AddToLog("Finished disabling all foreign key constraints")
        End If

    End Function

    Private Function EnableDisableForeignKeys(vTableName As String, vEnable As Boolean) As Integer

        Dim pSql As String
        Dim pDT As DataTable

        If vEnable Then
            AddToLog("Enabling foreign key constraints of " & vTableName)
        Else
            AddToLog("Disabling foreign key constraints of " & vTableName)
        End If

        pSql = "EXEC sp_fkeys " & EscapeSqlName(vTableName)
        pDT = DestinationProData.Execute(pSql)

        For Each pDR As DataRow In pDT.Rows
            pSql = "ALTER TABLE " & EscapeSqlName(pDR("FKTABLE_NAME")) & vbCrLf &
                If(vEnable, "", "NO") & "CHECK CONSTRAINT " & EscapeSqlName(pDR("FK_NAME"))
            DestinationProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Public Function CountSqliteAllRecords() As Long

        Dim pTotalCount As Long

        mProSqlite = New ProSqlite
        mProSqlite.Open(SourceSqliteFilename)
        Dim pTables = GetTables()

        For Each pDR As DataRow In pTables.Rows
            Dim pTableCount As Long = SqliteRecordCountFiltered(pDR("Name"))
            pTotalCount += pTableCount
        Next pDR

        Return pTotalCount

    End Function

    Private Class Table
        Public HasLastUpdatedField As Boolean
        Public RecordCount As Long
    End Class

    Private Function SqliteRecordCountFiltered(vTable As String) As Long

        If mTables.ContainsKey(vTable) Then
            Return mTables(vTable).RecordCount
        End If

        Dim pTable = New Table
        Dim pDtColumn = mProSqlite.GetColumns(vTable)
        pDtColumn.DefaultView.RowFilter = "name = 'LastUpdated'"
        If pDtColumn.DefaultView.Count > 0 Then
            pTable.HasLastUpdatedField = True
        End If

        Dim pSql = "SELECT COUNT(1)" & vbCrLf &
                " FROM " & vTable
        If pTable.HasLastUpdatedField _
            AndAlso MinimumDate IsNot Nothing Then

            pSql &= " WHERE LastUpdated >= " & ReadyString(Format(MinimumDate, cGlobal.FormatDateTimeSql2))
        End If
        Dim pCount As Long = mProSqlite.ExecuteScalar(pSql)
        pTable.RecordCount = pCount
        mTables.Add(vTable, pTable)

        If pTable.HasLastUpdatedField = False Then
            AppendToLog(DestinationTablePrefix & vTable & "; records: " & Format(pCount, cGlobal.FormatInteger), "temp\logs\TableWithoutLastUpdated", Format(Now, "MMdd"))
        End If

        Return pCount

    End Function

    Private Function SqliteRecordCountComplete(vTable As String) As Long

        Dim pSql = "SELECT COUNT(1)" & vbCrLf &
                " FROM " & vTable
        Dim pCount As Long = mProSqlite.ExecuteScalar(pSql)

        Return pCount

    End Function

    Private Function CompareTables(vTables As DataTable)

        Dim pOkCount As Integer
        Dim pNotOkCount As Integer
        Dim pText As String

        AddToLog("Comparing sqlite and sql server table to table...")
        For Each pDR As DataRow In vTables.Rows
            Dim pCount As Long = SqliteRecordCountFiltered(pDR("Name"))
            Dim pSql = "sp_spaceused " & DestinationTablePrefix & pDR("Name")
            Dim pDrSqlServer = DestinationProData.ExecuteRow(pSql)
            Dim pCountSqlServer = CLng(pDrSqlServer("rows"))
            Dim pCountSqliteComplete = SqliteRecordCountComplete(pDR("Name"))
            pText = pDR("Name") & ": sqlite table contains " & Format(pCountSqliteComplete, cGlobal.FormatInteger) & " records. " & Format(pCount, cGlobal.FormatInteger) & " records were imported. SQL Server table has " & Format(pCountSqlServer, cGlobal.FormatInteger) & " records."
            If pCountSqliteComplete <= pCountSqlServer Then
                pText &= "SQL Server table has equal or more rows. So, it's OK"
                pOkCount += 1
            Else
                pText &= "SQL Server table has fewer rows. So, it's ERROR"
                pNotOkCount += 1
            End If
            AddToLog(pText)
        Next pDR
        pText = "OK's = " & pOkCount & "; Not OK = " & pNotOkCount
        AddToLog(pText)

    End Function

    Public Class LogArgs
        Inherits EventArgs

        Public Text As String
    End Class

    Public Class DataImportedArgs
        Inherits EventArgs

        Public Table As String
        Public RowIndex As Long
    End Class
End Class

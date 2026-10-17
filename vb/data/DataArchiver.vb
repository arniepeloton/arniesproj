Public Class DataArchiver
    Public DatabaseName As String
    Public OrgID As String

    Dim mProData As ProData
    Dim mSourceProData As ProData

    Public Function Execute()

        If String.IsNullOrWhiteSpace(DatabaseName) Then
            Throw New ApplicationException("DatabaseName must not be blank. ref:vbA4YHFSB9FK!")
        End If

        mSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = Common.ConnectionInfo.Server,
            .Database = DatabaseName,
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

        If String.IsNullOrWhiteSpace(OrgID) Then
            Throw New ApplicationException("OrgID must not be blank. vbTM5H9QECT5!")
        End If
        OrgID = ReadyGuid(OrgID)

        mProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = Common.ConnectionInfo.Server,
            .Database = "",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        CreateDatabaseIfNeeded()

    End Function

    Private Sub CreateDatabaseIfNeeded()

        Dim pDatabaseName = "Archive_" & ShortGuid(OrgID)
        If mProData.GetDatabases().Contains(pDatabaseName) Then
            Dim pSql = "USE " & pDatabaseName
            mProData.ExecuteSQL(pSql)
        Else
            CreateDatabase()
        End If

    End Sub

    Private Sub CreateDatabase()

        Dim pCurrentDatabase = mProData.GetCurrentDatabase()
        Dim pFilename As String = mProData.GetFirstDataFilePath(pCurrentDatabase)
        mProData.CreateDatabase("Archive_" & ShortGuid(OrgID), IO.Path.GetDirectoryName(pFilename))

        Dim pDatabase As New DBCompare.Database(mSourceProData)
        pDatabase.LoadFromSql(True, False, False, True)
        pDatabase.SetCompareStatusToUnique()
        DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
        Dim pSql = pDatabase.GetScript()
        For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
            mProData.ExecuteSQL(pLine)
        Next

        With New Synonyms()

            Try
                .Prefix = "Mobiz"
                .SourceDatabaseName = "Mobiz"
                .ProData = mProData
                .Generate()
            Catch ex As System.Exception
                AppendToDebug(, True, ex, True)
            End Try

            Try
                .Prefix = "Archive"
                .SourceDatabaseName = "Archive_" & ShortGuid(OrgID)
                .ProData = mSourceProData
                .Generate()
            Catch ex As System.Exception
                AppendToDebug(, True, ex, True)
            End Try
        End With

    End Sub

End Class

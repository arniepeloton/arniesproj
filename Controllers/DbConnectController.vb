Public Class DbConnectController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If Request.QueryString("SetSynonyms") = "1" Then
            With New Synonyms()
                .Prefix = "Mobiz"

                Try
                    .SourceDatabase = "DB_70111_mbzpubeas420"
                    .ProData = ProData.CreateFromConnectionInfo( _
                        "s18.winhost.com", _
                        "DB_70111_mbzprieas420",
                        "DB_70111_mbzprieas420_user", _
                        "abc123",
                        False)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        "s07.winhost.com", _
                '        "DB_70111_mobizcafefra",
                '        "DB_70111_mobizcafefra_user", _
                '        "mobizcafefrance",
                '        False)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizProton",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizNike",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizMobiz",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizDUC",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizDSME",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizTestCompany",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizTemplate",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    ' MobizAsian_Finds_Inc_
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizAsianfinds_Inc",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizBN_whitening_Shoppe",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizJWS_BEAUTY_AND_BEYOND_CO_",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizK",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizLots_A_Pizza",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizNtek_Global_Inc_",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizResourceful_International",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizStarbike",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizRC_Inc_Test",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try

                'Try
                '    .ProData = ProData.CreateFromConnectionInfo( _
                '        Common.WebRequest.ProData.Server, _
                '        "MobizRobert_Inc_Test",
                '        Common.WebRequest.ProData.Username, _
                '        Common.WebRequest.ProData.Password,
                '        Common.WebRequest.ProData.IntegratedSecurity)
                '    .Generate()
                'Catch ex As System.Exception
                '    AppendToDebug(, True, ex, True)
                'End Try
            End With
            AppendToLog("Synonyms has been triggered successfully")
        End If
        AppendToLog("dbconnect page")
        Return View()

    End Function

    <HttpPost()>
    Public Function Save() As String

        Dim pCI As ConnectionInfo
        Dim pProData As ProData

        If CBool2(Common.WebRequest.Data("NoUpdate")) Then
            If IO.File.Exists(Common.ConnectionInfo.Filename) Then
                Return JsonDictionary.Serialize("Error", "Database connection setting file already exist.")
            End If
        Else
            If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
                Return JsonDictionary.Serialize("Error", "Invalid DB Connect password")
            End If
        End If

        AppendToLog("Composing ConnectionInfo. vbR2USPFA86C!", , "DbConnect")
        '// check if connected
        pCI = Common.ConnectionInfo
        If CBool2(Common.WebRequest.Data("SaveAsEcommerceServerSqlServer")) Then
            pCI.Filename = Common.ServerPath & "settings\EcommerceServerSqlServer.data"
        Else
            pCI.Filename = Common.ServerPath & "settings\db.data"
        End If
        pCI.Server = Common.WebRequest.Data("Server")
        pCI.Database = Common.WebRequest.Data("Database")
        pCI.IntegratedSecurity = Common.WebRequest.Data("IntegratedSecurity")
        pCI.Username = Common.WebRequest.Data("Username")
        pCI.Password = Common.WebRequest.Data("Password")
        'pCI.ConnectionString = pData.TryGet("")
        AppendToLog("Connecting using ConnectionInfo. vb35VG7MTPH9!", , "DbConnect")
        Try
            pProData = New ProData
            pProData.Connect(pCI.Server, _
                pCI.Database, _
                pCI.Username, _
                pCI.Password, _
                pCI.IntegratedSecurity)
        Catch ex As Exception
            AppendToLog("Error connecting using ConnectionInfo. vbWHEF8HLQ6X!" & vbCrLf &
                ex.Message, , "DbConnect")
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

        AppendToLog("Done connecting using ConnectionInfo. vb7DE63LK649!", , "DbConnect")
        If CStr2(Common.WebRequest.Data("WebPosRemoteServerIp")).Length = 0 Then
            If pProData.Connected = False Then
                Return JsonDictionary.Serialize("Error", pProData.ConnectError)
            End If
        End If

        FileText(Common.ServerPath & "settings\WebPosRemoteServer.txt") = Common.WebRequest.Data("WebPosRemoteServerIp")

        '// check if saved
        AppendToLog("Saving ConnectionInfo. vbMTG4GTP8HV!", , "DbConnect")
        If Common.ConnectionInfo.Save() Then
            AppendToLog("Opening ConnectionInfo. vbUCC5JV5RG2!", , "DbConnect")
            Common.ConnectionInfo.Open()
            AppendToLog("Done opening ConnectionInfo. vbKXZC2ET48W!", , "DbConnect")
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Error", "Failed to save connection info. " & Common.ConnectionInfo.ErrorMessage)
        End If

    End Function

    <HttpPost()>
    Public Function SaveFlushingDatabase() As String

        Dim pCI As ConnectionInfo
        Dim pProData As ProData

        If CBool2(Common.WebRequest.Data("NoUpdate")) Then
            If IO.File.Exists(Common.ConnectionInfo.Filename) Then
                Return JsonDictionary.Serialize("Error", "Database connection setting file already exist.")
            End If
        Else
            If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
                Return JsonDictionary.Serialize("Error", "Invalid DB Connect password")
            End If
        End If

        If CStr2(Common.ConnectionInfo.Server).Trim().ToLower() = CStr2(Common.WebRequest.Data("FlushingServer")).Trim().ToLower() Then
            Return JsonDictionary.Serialize("Error", "Flushing server must be different from the main database server.")
        End If

        pCI = New ConnectionInfo()
        pCI.Filename = Common.ServerPath & "settings\FlushingDb.data"
        pCI.Server = Common.WebRequest.Data("FlushingServer")
        pCI.Database = Common.WebRequest.Data("FlushingDatabase")
        pCI.IntegratedSecurity = Common.WebRequest.Data("FlushingIntegratedSecurity")
        pCI.Username = Common.WebRequest.Data("FlushingUsername")
        pCI.Password = Common.WebRequest.Data("FlushingPassword")
        'pCI.ConnectionString = pData.TryGet("")
        AppendToLog("Connecting using FlushingConnectionInfo. vbXU6XJKUYCV!", , "DbConnect")
        Try
            pProData = New ProData
            If pProData.Connect(pCI.Server, _
                "master", _
                pCI.Username, _
                pCI.Password, _
                pCI.IntegratedSecurity) Then

                Dim pDatabases = pProData.GetDatabases(True, True)
                If pDatabases.Contains("mobiz") = False Then
                    CopyDatabase(Common.ConnectionInfo, pCI, Common.WebRequest.Data("DatabaseFolder"))
                End If

                Dim pPublicMdbFolder As String
                For Each pFastFlushingOrgID In ImportDataClass.FastFlushingOrgIDs()
                    Dim pDrOrg = WebRequest.Instance.Business.LibraryOrganization.ByID(pFastFlushingOrgID)
                    If CStr2(pDrOrg("Database")).Trim() = "" Then
                        Continue For
                    End If
                    If pDatabases.Contains(CStr2(pDrOrg("Database")).ToLower()) = False Then
                        If String.IsNullOrWhiteSpace(pPublicMdbFolder) Then
                            pPublicMdbFolder = pProData.GetFirstDataFilePath("Mobiz")
                            pPublicMdbFolder = IO.Path.GetDirectoryName(pPublicMdbFolder)
                        End If
                        CopyDatabase(New ConnectionInfo() With {
                            .Server = Common.ConnectionInfo.Server,
                            .Database = CStr2(pDrOrg("Database")),
                            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                            .Username = Common.ConnectionInfo.Username,
                            .Password = Common.ConnectionInfo.Password}, pCI, pPublicMdbFolder)
                    End If
                Next

                pCI.Save()
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pProData.ConnectError)
            End If
        Catch ex As Exception
            AppendToLog("Error connecting using ConnectionInfo. vbWHEF8HLQ6X!" & vbCrLf &
                ex.Message, , "DbConnect")
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Private Shared Sub CopyDatabase(vSourceConnection As ConnectionInfo, vDestinationConnection As ConnectionInfo, vDestinationFolder As String)

        ProPack.Utility.AddDirectoryUser(vDestinationFolder)
        Using pProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = vDestinationConnection.Server,
            .Database = "master",
            .IntegratedSecurity = vDestinationConnection.IntegratedSecurity,
            .Username = vDestinationConnection.Username,
            .Password = vDestinationConnection.Password})

            pProData.CreateDatabase("temp_" & vSourceConnection.Database, vDestinationFolder)
            pProData.ExecuteSQL("USE " & "temp_" & vSourceConnection.Database)
            ' put the schema to the blank database
            Using pSourceProData = ProData.CreateFromConnectionInfo(vSourceConnection)
                Dim pDatabase As New DBCompare.Database(pSourceProData)
                pDatabase.LoadFromSql(True, False, False, True)
                pDatabase.SetCompareStatusToUnique()
                DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
                Dim pSql = pDatabase.GetScript()
                For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                    Try
                        pProData.ExecuteSQL(pLine)
                    Catch ex As Exception
                        AppendToLog("vb6SDK6YPPP3! CopyDatabase " & ex.Message & vbCrLf & pLine)
                        Throw
                    End Try
                Next

                Dim pDtFiles = pProData.GetDatabaseFiles("temp_" & vSourceConnection.Database)
                pProData.DetachDatabase("temp_" & vSourceConnection.Database)
                ProPack.Utility.AddDirectoryUser(vDestinationFolder)
                Dim pMdbFilename As String
                For Each pDrFiles As DataRow In pDtFiles.Rows
                    Dim pSourceFilename = pDrFiles("physical_name")
                    Dim pDestinationFilename = AppendBackslash(IO.Path.GetDirectoryName(pSourceFilename)) & vSourceConnection.Database & IO.Path.GetExtension(pSourceFilename)
                    If CStr2(pDrFiles("type_desc")).Trim().ToLower() = "rows" Then
                        pMdbFilename = pDestinationFilename
                    End If
                    IO.File.Move(pSourceFilename, pDestinationFilename)
                Next
                pProData.AttachDatabase(vSourceConnection.Database, pMdbFilename)
                pProData.ExecuteSQL("USE " & EscapeSqlName(vSourceConnection.Database))
                With New Synonyms()
                    .Prefix = "Mobiz"
                    Try
                        .SourceDatabase = "Mobiz"
                        .ProData = pProData
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try
                End With
            End Using ' END pProData of blan
        End Using

    End Sub

    <HttpPost()>
    Public Function GetData() As String

        Dim pOut As JsonDictionary

        Try
            pOut = New JsonDictionary
            pOut.Add("Server", Common.ConnectionInfo.Server)
            pOut.Add("Database", Common.ConnectionInfo.Database)
            pOut.Add("WebPosRemoteServerIp", FileText(Common.ServerPath & "settings\WebPosRemoteServer.txt"))

            Dim pFilename = Common.ServerPath & "settings\FlushingDb.data"
            If IO.File.Exists(pFilename) Then
                Dim pFlushingConnectionInfo = New ConnectionInfo()
                pFlushingConnectionInfo.Filename = pFilename
                If pFlushingConnectionInfo.Open() Then
                    pOut.Add("FlushingServer", pFlushingConnectionInfo.Server)
                    pOut.Add("FlushingDatabase", pFlushingConnectionInfo.Database)
                End If
            End If

            pOut.Add("Result", "ok")

            Return pOut.Serialize
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function SetSynonymsForSelectedPrivateDatabase() As String
        Dim DatabaseDestination As String
        Dim DatabaseSource As String
        Dim DatabasePrefix As String

        DatabaseDestination = Request.QueryString("DatabaseName").Trim()
        DatabaseSource = Request.QueryString("DatabaseSource").Trim()
        DatabasePrefix = Request.QueryString("DatabasePrefix").Trim()

        If DatabaseDestination = vbNullString Then Exit Function
        If DatabaseSource = vbNullString Then Exit Function
        If DatabasePrefix = vbNullString Then Exit Function

        With New Synonyms()
            .Prefix = DatabasePrefix

            Try
                .SourceDatabase = DatabaseSource
                .ProData = ProData.CreateFromConnectionInfo( _
                    Common.WebRequest.ProData.Server, _
                    DatabaseDestination,
                    Common.WebRequest.ProData.Username, _
                    Common.WebRequest.ProData.Password,
                    Common.WebRequest.ProData.IntegratedSecurity)
                .Generate()

                '.SourceDatabase = "MobizABC" 'MobizForPotatoCorner
                '.ProData = ProData.CreateFromConnectionInfo( _
                '    Common.WebRequest.ProData.Server, _
                '    "MobizDataTest",
                '    Common.WebRequest.ProData.Username, _
                '    Common.WebRequest.ProData.Password,
                '    Common.WebRequest.ProData.IntegratedSecurity)
                '.Generate()


                'MobizPotatoCorner

                'If pSuccesses Is Nothing Then
                '    pSuccesses = pOut.AddList("Successes")
                'End If
                'If pDB <> "Mobiz" Then
                '    pSecurityAccess = New SecurityAccess()
                '    pSecurityAccess.AddSecurityModules(.ProData)
                'End If
                'pSuccesses.Add(pDB)
                'AppendToLog("Done adding synonyms to " & pDB & ". vbDRNGD3R8SL!", , "SetSynonyms")
            Catch ex As System.Exception
                'AppendToLog("Error occured while adding synonyms to " & pDB & ". " & ex.Message & ". vbCSVD4Y8XZ9!", , "SetSynonyms")

                'If pErrors Is Nothing Then
                '    pErrors = pOut.AddList("Errors")
                'End If
                'With pErrors.AddDictionary
                '    .Add("Database", pDB)
                '    .Add("Message", ex.Message)
                'End With
                AppendToDebug(, True, ex, True)
            Finally
                Try
                    .ProData.Dispose()
                Catch ex As Exception
                    ' just ignore any error here
                End Try
            End Try
        End With
    End Function

    <HttpPost>
    Public Function SetSynonyms() As String

        Dim pDbs As List(Of String)
        Dim pDbsFinal As List(Of String)
        Dim pOut As JsonDictionary
        Dim pErrors As JsonList
        Dim pSuccesses As JsonList
        Dim pSecurityAccess As SecurityAccess
        Dim pPrivateDatabases As String

        Try
            If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
                Return JsonDictionary.Serialize("Error", "Invalid DB Connect password")
            End If

            pOut = New JsonDictionary
            pDbsFinal = New List(Of String)
            pDbsFinal.Add(Common.WebRequest.ProData.Database)
            pPrivateDatabases = Common.WebRequest.Data("PrivateDatabases")
            If String.IsNullOrWhiteSpace(pPrivateDatabases) Then
                pDbs = Common.WebRequest.ProData.GetDatabases()
                AppendToLog("Getting databases to add synonyms to. 2020_04_10__17_23_22_3117 vb84ZFCAE3BL!", , "SetSynonyms")
                For Each pDB As String In pDbs
                    If IsPrivateDatabase(pDB) Then
                        pDbsFinal.Add(pDB)
                    End If
                Next pDB
                AppendToLog("Done getting databases to add synonyms to. vb7THMWGXLZ9!", , "SetSynonyms")
            Else
                For Each pPrivateDatabase In pPrivateDatabases.Split({vbCrLf, vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                    pDbsFinal.Add(pPrivateDatabase)
                Next
            End If

            ' at this point, pDbsFinal contains database name to add synonyms to.

            For Each pDB As String In pDbsFinal
                AppendToLog("Adding synonyms to " & pDB & ". vb6TT9VP6CE9!", , "SetSynonyms")

                With New Synonyms()
                    .Prefix = "Mobiz"

                    Try
                        '.SourceDatabase = "Mobiz" ' old
                        .SourceDatabase = Common.WebRequest.ProData.Database
                        .ProData = ProData.CreateFromConnectionInfo( _
                            Common.WebRequest.ProData.Server, _
                            pDB,
                            Common.WebRequest.ProData.Username, _
                            Common.WebRequest.ProData.Password,
                            Common.WebRequest.ProData.IntegratedSecurity)
                        AppendToLog("Generating synonyms to " & pDB & ". vbDNUE5FCU2Z!", , "SetSynonyms")
                        .Generate()
                        AppendToLog("Done generating synonyms to " & pDB & ". vb4G57JD7XKT!", , "SetSynonyms")

                        If pSuccesses Is Nothing Then
                            pSuccesses = pOut.AddList("Successes")
                        End If
                        If pDB.ToLower() <> "Mobiz".ToLower() Then
                            pSecurityAccess = New SecurityAccess()
                            pSecurityAccess.AddSecurityModules(.ProData)
                        End If
                        pSuccesses.Add(pDB)
                        AppendToLog("Done adding synonyms to " & pDB & ". vbDRNGD3R8SL!", , "SetSynonyms")
                    Catch ex As System.Exception
                        AppendToLog("Error occured while adding synonyms to " & pDB & ". " & ex.Message & ". vbCSVD4Y8XZ9!", , "SetSynonyms")

                        If pErrors Is Nothing Then
                            pErrors = pOut.AddList("Errors")
                        End If
                        With pErrors.AddDictionary
                            .Add("Database", pDB)
                            .Add("Message", ex.Message)
                        End With
                        AppendToDebug(, True, ex, True)
                    Finally
                        Try
                            .ProData.Dispose()
                        Catch ex As Exception
                            ' just ignore any error here
                        End Try
                    End Try
                End With
            Next pDB
            AppendToLog("Done with SetSynonyms. vbGCW3RFT9XK!", , "SetSynonyms")

            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Shared Function IsPrivateDatabase(vDatabaseName As String) As Boolean

        vDatabaseName = vDatabaseName.ToLower()

        If vDatabaseName = "MobizWebsite".ToLower Then Return False
        If vDatabaseName = "MobizOnlineStore".ToLower Then Return False
        If vDatabaseName = "mobiz" Then Return False
        If vDatabaseName.EndsWith("_datatag") Then Return False

        If vDatabaseName.StartsWith("mobiz") Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function DetachWebPosDb() As String

        If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
            Return JsonDictionary.Serialize("Message", "Invalid DB Connect password")
        End If

        Dim pOut = New JsonDictionary()
        Dim pResult As String = ""
        Try

            Using pProData = ProData.CreateFromConnectionInfo( _
                Common.WebRequest.ProData.Server, _
                "master",
                Common.WebRequest.ProData.Username, _
                Common.WebRequest.ProData.Password,
                Common.WebRequest.ProData.IntegratedSecurity)

                Dim pDatabases = pProData.GetDatabases()
                For Each pDatabase In pDatabases
                    Dim pFilename = pProData.GetFirstDataFilePath(pDatabase)
                    If pFilename.ToLower().StartsWith((AppendBackslash(Common.ServerPath) & "WebPosDb").ToLower()) Then
                        pProData.DetachDatabase(pDatabase)
                        pResult &= pDatabase & " was successfully detached" & "<br />" & vbCrLf
                    End If
                Next
            End Using

            pOut.Add("Result", "ok")
            If String.IsNullOrWhiteSpace(pResult) Then
                pResult = "Nothing to detach"
            End If
            pOut.Add("Message", pResult)
        Catch ex As Exception
            pOut.Add("Message", "Error: " & ex.Message & "<br />" & vbCrLf & pResult)
        End Try

        Return pOut.Serialize()

    End Function

    Public Function AttachWebPosDb() As String

        If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
            Return JsonDictionary.Serialize("Message", "Invalid DB Connect password")
        End If

        Dim pOut = New JsonDictionary()
        Dim pResult As String = ""
        Try
            

            Using pProData = ProData.CreateFromConnectionInfo( _
                Common.WebRequest.ProData.Server, _
                "master",
                Common.WebRequest.ProData.Username, _
                Common.WebRequest.ProData.Password,
                Common.WebRequest.ProData.IntegratedSecurity)

                Dim pDatabases = pProData.GetDatabases(True, True)
                For Each pFilename In System.IO.Directory.GetFiles(AppendBackslash(Common.ServerPath) & "WebPosDb", "*.mdf")
                    Dim pDatabaseName = System.IO.Path.GetFileNameWithoutExtension(pFilename)
                    If pDatabases.Contains(pDatabaseName.Trim().ToLower()) Then
                        Continue For
                    End If
                    pProData.AttachDatabase(pDatabaseName, pFilename)
                    pResult &= pFilename & " was successfully attached as " & pDatabaseName & "<br />" & vbCrLf
                Next
            End Using

            pOut.Add("Result", "ok")
            If String.IsNullOrWhiteSpace(pResult) Then
                pResult = "Nothing to attach"
            End If
            pOut.Add("Message", pResult)
        Catch ex As Exception
            pOut.Add("Message", "Error: " & ex.Message & "<br />" & vbCrLf & pResult)
        End Try

        Return pOut.Serialize()

    End Function

End Class
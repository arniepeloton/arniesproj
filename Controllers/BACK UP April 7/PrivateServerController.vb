Namespace WebProj
    Public Class PrivateServerController
        Inherits System.Web.Mvc.AsyncController

        Private Const mZipDllName As String = "C1.C1Zip.2.dll"
        Private Const mZipWebFileName As String = "WebFile.zip"

        Private Shared mLog As Dictionary(Of String, Dictionary(Of UInteger, String))

        Shared Sub New()

            mLog = New Dictionary(Of String, Dictionary(Of UInteger, String))

        End Sub

#Region " as a publisher "
        Public Function DownloadZipDll() As ActionResult

            Dim pFilename As String

            pFilename = Common.ServerPath & "bin\" & mZipDllName
            If IO.File.Exists(pFilename) Then
                Return File(pFilename, "application/dll", mZipDllName)
            Else
                Return Content("Zip, " & mZipDllName & ", does not exist.")
            End If

        End Function

        <AsyncTimeout(Integer.MaxValue)>
        Public Sub InitDownloadWebFilesAsync()

            Dim pOrgID As String
            Dim pPrivateLog As Dictionary(Of UInteger, String)
            Dim pMessageKey As String
            Dim pOut As JsonDictionary

            Try
                AsyncManager.OutstandingOperations.Increment()
                pOrgID = Common.WebRequest.QueryString("OrgID")
                If CStr2(pOrgID).Trim() = "" Then
                    AsyncManager.Parameters("vOut") = JsonDictionary.ToString("Error", "Query string (OrgID) not found vb22P5J59LAA!")
                    AsyncManager.OutstandingOperations.Decrement()
                    Exit Sub

                End If
                If IsGuid(pOrgID) = False Then
                    AsyncManager.Parameters("vOut") = JsonDictionary.ToString("Error", "Query string (OrgID) must be a valid GUID vbBXW54RG6TD!")
                    AsyncManager.OutstandingOperations.Decrement()
                    Exit Sub
                End If

                pPrivateLog = New Dictionary(Of UInteger, String)
                pPrivateLog.Add(TickCount(), JsonDictionary.ToString("message", "Database connection was successfully configured. Please wait for database initialization."))
                pMessageKey = GenerateString(20)
                mLog.Add(pMessageKey, pPrivateLog)

                TimedThread( _
                    Sub()
                        CreateDatabases(pOrgID, pMessageKey)
                    End Sub
                )

                pOut = New JsonDictionary()
                pOut.Add("Result", "ok")
                pOut.Add("MessageKey", pMessageKey)

                AsyncManager.Parameters("vOut") = pOut.ToString()
                AsyncManager.OutstandingOperations.Decrement()
            Catch ex As System.Exception
                AsyncManager.Parameters("vOut") = JsonDictionary.ToString("Error", ex.Message & " vbM4QTL4XUR5!")
                AsyncManager.OutstandingOperations.Decrement()
            End Try

        End Sub

        Public Function InitDownloadWebFilesCompleted(vOut As String) As String

            Return vOut

        End Function

        Private Sub CreateDatabases(vOrgID As String, vMessageKey As String)

            Dim p As New PremiseDatabasesMaker

            p.BlankPublicDatabaseFilename = "E:\database backups-sql2\Mobiz blank.bak"
            p.OrganizationID = vOrgID '"cfe39df6-8809-4973-9515-810df75aed12" ' mobizABC
            p.ConnectionInfo = Common.ConnectionInfo
            'p.ConnectionInfo.Server = "philhr\sql2"
            'p.ConnectionInfo.Username = "sa"
            'p.ConnectionInfo.Password = "ducmain2"
            p.DatabaseFolder = "E:\database backups-sql2 to premise"
            AddHandler p.Completed,
                Sub(sender As PremiseDatabasesMaker, e As PremiseDatabasesMaker.CompletedArgs)
                    Dim pPrivateLog As Dictionary(Of UInteger, String)
                    Dim pJson As JsonDictionary

                    pPrivateLog = mLog(vMessageKey)
                    pJson = New JsonDictionary()
                    If e.Success Then
                        AppendToDebug("2NVX7MT5HJ")
                        pJson.Add("message", "Database backups were successfully created.")
                        Debug.Print("PremiseDatabasesMaker.Completed: success")
                        Debug.Print("PublicDatabaseFilename: " & p.PublicDatabaseFilename)
                        Debug.Print("PrivateDatabaseFilename: " & p.PrivateDatabaseFilename)
                        Compress(p, vMessageKey)
                    Else
                        AppendToDebug("6YPDM7EKV2")
                        pJson.Add("Result", "error")
                        pJson.Add("message", e.GetErrorText())
                        Debug.Print("PremiseDatabasesMaker.Completed: fail: " & e.GetErrorText())
                    End If
                    pPrivateLog.Add(TickCount(), pJson.ToString())
                End Sub
            AppendToDebug("N2J8WKPZV3")
            p.Execute()

        End Sub

        Private Sub Compress(v As PremiseDatabasesMaker, vMessageKey As String)

            Dim pFilename As String
            Dim pDT As DataTable
            Dim pProZip As ProZip
            Dim pPrivateLog As Dictionary(Of UInteger, String)
            Dim pJson As JsonDictionary
            Dim pTimeStamp As String

            AppendToDebug("Y95XERPPEB")
            pDT = CreateFilesList()
            pTimeStamp = DateTimeStamp()
            pFilename = Common.ServerPath & "temp\PrivateServer\" & pTimeStamp & ".zip"
            CreateFolder(pFilename)
            pProZip = New ProZip(pFilename)
            For Each pDR As DataRow In pDT.Rows
                pProZip.AddFile(pDR("FullName"), IO.Path.GetDirectoryName(pDR("RelativePath")))
            Next pDR
            pProZip.AddFileRename(v.PublicDatabaseFilename, "db\Mobiz.bak")
            pProZip.AddFileRename(v.PrivateDatabaseFilename, "db\MobizPrivate.bak")
            pPrivateLog = mLog(vMessageKey)

            pJson = New JsonDictionary()
            pJson.Add("Result", "ok")
            pJson.Add("message", "Database backups were successfully created.")
            pJson.Add("TimeStamp", pTimeStamp)
            pPrivateLog.Add(TickCount(), pJson.ToString())
            AppendToDebug("E34WY9QGFL")

        End Sub

        Public Function Download() As ActionResult

            Dim pTimeStamp As String
            Dim pFilename As String

            pTimeStamp = Common.WebRequest.QueryString("TimeStamp")
            If CStr2(pTimeStamp).Trim() = "" Then
                Return Content("Timestamp query string is required vbRGPJBTBKWQ!")
            End If
            pFilename = Common.ServerPath & "temp\PrivateServer\" & pTimeStamp & ".zip"

            If IO.File.Exists(pFilename) Then
                Return File(pFilename, "application/zip", mZipWebFileName)
            Else
                Return Content("Zip, " & mZipWebFileName & ", does not exist.")
            End If

        End Function

        Public Function DownloadWebFilesList() As ActionResult

            Dim pFilename As String
            Dim pDT As DataTable
            Dim pProZip As ProZip

            Try
                pDT = CreateFilesList()
                pFilename = Common.ServerPath & "temp\PrivateServer\" & DateTimeStamp() & ".zip"
                CreateFolder(pFilename)
                pProZip = New ProZip(pFilename)
                For Each pDR As DataRow In pDT.Rows
                    pProZip.AddFile(pDR("FullName"), IO.Path.GetDirectoryName(pDR("RelativePath")))
                Next pDR


                If IO.File.Exists(pFilename) Then
                    Return File(pFilename, "application/zip", mZipWebFileName)
                Else
                    Return Content("Zip, " & mZipWebFileName & ", does not exist.")
                End If
            Catch ex As System.Exception
                Return Content(ex.Message)
            End Try

        End Function

        Private Function CreateFilesList() As DataTable

            Dim pDT As DataTable

            pDT = DirectoryToTable(vFolder:=Common.ServerPath,
                vExcludeFilePattern:={
                    "*.vb",
                    "*.cs",
                    "*.log",
                    "*.tmp",
                    "*.temp",
                    "*.scc",
                    "*.vspscc",
                    "*.vbproj",
                    "*.sln",
                    "*.suo",
                    "*.user",
                    "*.vssscc",
                    "*.rar",
                    "*.ldf",
                    "*.mdf",
                    "*sublime*",
                    "SqlException.xml",
                    "Web.Debug.config",
                    "Web.Release.config",
                    "thumbs.db"
                },
                vIncludeFolderPattern:={
                    "bin",
                    "Content",
                    "Scripts",
                    "Scripts3",
                    "Views",
                    "db"
                },
                vIncludeSubDirectories:=True,
                vDontFilterSubDirectories:=True,
                vAddRelativePathColumn:=True
            )

            Return pDT

        End Function
#End Region

#Region " as a subscriber "
        Public Function Index() As ActionResult

            If Request.IsLocal = False Then
                Return Content("This feature may only be accessed locally.")
            End If

            Return View()

        End Function

        <HttpPost()>
        Public Function DatabaseSetupStatus() As String

            Dim pOut As JsonDictionary

            pOut = New JsonDictionary()
            Try
                If Common.ConnectionInfo.Opened Then
                    Try
                        If IsDate(Common.WebRequest.ProData.GetDate) Then
                            pOut.Add("DbIsConnected", 1)
                        Else
                            Return JsonDictionary.Serialize("Error", "Can connect but invalid date vbVRK27G99VJ!")
                        End If
                    Catch ex As Exception
                        pOut.Add("CantConnect", 1)
                    End Try
                Else
                    ' no db settings detected
                    pOut.Add("NoSettings", 1)
                End If
            Catch ex As System.Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Public Function SaveDatabaseConnection() As String

            Dim pCI As ConnectionInfo
            Dim pProData As ProData
            Dim pOut As JsonDictionary
            Dim pMessageKey As String
            Dim pPrivateLog As Dictionary(Of UInteger, String)

            If CBool2(Common.WebRequest.Data("NoUpdate")) Then
                If IO.File.Exists(Common.ConnectionInfo.Filename) Then
                    Return JsonDictionary.Serialize("Error", "Database connection setting file already exist.")
                End If
            Else
                If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
                    Return JsonDictionary.Serialize("Error", "Invalid DB Connect password")
                End If
            End If


            '// check if connected
            pCI = Common.ConnectionInfo
            pCI.Filename = Common.ServerPath & "settings\db.data"
            pCI.Server = Common.WebRequest.Data("Server")
            'pCI.Database = Common.WebRequest.Data("Database")
            'pCI.Database = "master"
            pCI.IntegratedSecurity = Common.WebRequest.Data("IntegratedSecurity")
            pCI.Username = Common.WebRequest.Data("Username")
            pCI.Password = Common.WebRequest.Data("Password")
            pProData = New ProData
            pProData.Connect(pCI.Server, _
                "master", _
                pCI.Username, _
                pCI.Password, _
                pCI.IntegratedSecurity)
            If pProData.Connected = False Then
                Return JsonDictionary.Serialize("Error", pProData.ConnectError)
            End If

            '// check if saved
            pCI.Database = Common.WebRequest.Data("Database")
            If Common.ConnectionInfo.Save() Then
                Common.ConnectionInfo.Open()
                pCI.Database = "master"
                RestoreDatabases("Mobiz")
                pOut = New JsonDictionary()
                pOut.Add("Result", "ok")
                pMessageKey = GenerateString(20)
                pOut.Add("MessageKey", pMessageKey)
                pPrivateLog = New Dictionary(Of UInteger, String)
                pPrivateLog.Add(TickCount(), JsonDictionary.ToString("message", "Database connection was successfully configured. Please wait for database initialization."))
                mLog.Add(pMessageKey, pPrivateLog)
                Return pOut.Serialize()
            Else
                Return JsonDictionary.Serialize("Error", "Failed to save connection info. " & Common.ConnectionInfo.ErrorMessage)
            End If

        End Function

        <HttpGet()>
        Public Function GetStatus() As String

            Dim pOut As JsonDictionary
            Dim pLastTick As UInteger
            Dim pMessages As List(Of String)
            Dim pMessageKey As String

            pMessageKey = Common.WebRequest.QueryString("MessageKey")
            If CStr2(pMessageKey).Trim.Length = 0 Then
                Return JsonDictionary.ToString("Error", "No message key vbLEL266CMHY!")
            End If
            If CStr2(pMessageKey).Length <> 20 Then
                Return JsonDictionary.ToString("Error", "Invalid message key length vb5R23HZY4F2!")
            End If

            If CStr2(Common.WebRequest.QueryString("LastTick")) <> "" Then
                If UInteger.TryParse(Common.WebRequest.QueryString("LastTick"), pLastTick) = False Then
                    Return JsonDictionary.ToString("Error", "LastTick must be a valid uinteger vbPQDHZ3BGGB!")
                End If
            End If

            If mLog.ContainsKey(pMessageKey) = False Then
                Return JsonDictionary.ToString("Error", "Message with the specified key does not exist vb7K3L6AS7FW!")
            End If
            pMessages = GetMessage(mLog(pMessageKey), pLastTick)

            pOut = New JsonDictionary()
            If pMessages IsNot Nothing Then
                pOut.Add("Messages", pMessages)
            End If
            pOut.Add("LastTick", pLastTick)

            Return pOut.ToString()

        End Function

        Private Shared Function GetMessage(vMessages As Dictionary(Of UInteger, String), ByRef rLastKey As UInteger) As List(Of String)

            Dim pOut As List(Of String)
            Dim pLastKey As UInteger = rLastKey ''ByRef' parameter 'rLastKey' cannot be used in a query expression.

            If vMessages Is Nothing _
                OrElse vMessages.Count = 0 Then

                Return Nothing
            End If

            Dim p2 =
                From pItem As KeyValuePair(Of UInteger, String) In vMessages
                Select pItem
                Where pItem.Key > pLastKey
                Order By pItem.Key

            Dim p3 As IEnumerable(Of KeyValuePair(Of UInteger, String)) = CType(p2, IEnumerable(Of KeyValuePair(Of UInteger, String)))
            pOut = New List(Of String)
            For Each pItem As KeyValuePair(Of UInteger, String) In p2
                pOut.Add(pItem.Value)
                rLastKey = pItem.Key
            Next pItem

            Return pOut

        End Function

        Private Sub RestoreDatabases(ByVal vDbName As String)

            Dim pDbRestore As DbRestore
            Dim pFilename As String

            pDbRestore = New DbRestore()
            pDbRestore.ProData = Common.WebRequest.ProData
            pFilename = Common.ServerPath & "db\" & vDbName & ".bak"
            pDbRestore.SourceFilename = pFilename
            pDbRestore.DestinationDatabaseName = EscapeSqlName(vDbName)
            pDbRestore.DestinationFolder = Common.ServerPath & "db"
            AddHandler pDbRestore.Completed,
                Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                    If CType(sender, DbRestore).DestinationDatabaseName.Equals("Mobiz", StringComparison.InvariantCultureIgnoreCase) Then
                        RestoreDatabases("MobizPrivate")
                    Else
                        Debug.Print("Done restoring databases STXPRFSEJ4")
                    End If
                End Sub
            pDbRestore.Execute()

        End Sub
#End Region
    End Class
End Namespace
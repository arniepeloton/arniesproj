Imports System.Net

Public Class SchemaSyncSenderController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function CreateSnapshot() As String

        Dim pSync As SchemaSync.Sync
        Dim pConnectionInfo As ConnectionInfo
        Dim pSnapshotFilename As String
        Dim pZipFilename As String
        Dim pProZip As ProZip
        Dim pFolder As String
        Dim pDbMaps As List(Of DatabaseMap)

        Try
            pConnectionInfo = Common.ConnectionInfo
            pSync = New SchemaSync.Sync()
            pSync.SourceDb = New ConnectionInfo()
            pSync.SourceDb.Server = pConnectionInfo.Server
            pSync.SourceDb.Username = pConnectionInfo.Username
            pSync.SourceDb.Password = pConnectionInfo.Password
            pSync.SourceDb.IntegratedSecurity = pConnectionInfo.IntegratedSecurity
            pFolder = Common.ServerPath & "temp\SchemaSyncSender\"
            CreateFolder(pFolder)

            pDbMaps = CreateDatabaseMaps()
            pZipFilename = pFolder & "Snapshots.zip"
            If IO.File.Exists(pZipFilename) Then
                IO.File.Delete(pZipFilename)
            End If
            pProZip = New ProZip(pZipFilename)

            For Each pDbMap As DatabaseMap In pDbMaps
                pSync.SourceDb.Database = pDbMap.SourceDatabaseName
                pSnapshotFilename = pFolder & pDbMap.DestinationBaseFilename & ".snap"
                pSync.SaveSnapshot(pSnapshotFilename)
                pProZip.AddFile(pSnapshotFilename)
            Next pDbMap
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "vbHS4C5ATKWX! " & ex.Message)
        End Try

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function SendSnapshot() As String

        Dim pWC As WebClient
        Dim pRemoteIp As String
        Dim pResponse As String
        Dim pRequestJson As IDictionary(Of String, Object)


        pWC = New WebClient
        Try
            If CStr2(Common.WebRequest.Data("remoteIp")).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "remoteIp POST data is required")
            End If
            pRemoteIp = Common.WebRequest.Data("remoteIp")
            pRemoteIp = AppendIfNeeded(pRemoteIp, "/") & "SchemaSyncReceiver/Receive"
            pResponse = New System.Text.UTF8Encoding().GetString(pWC.UploadFile(pRemoteIp, Common.ServerPath & "temp\SchemaSyncSender\Snapshots.zip"))
            If IsJson(pResponse) Then
                pRequestJson = New JavaScriptSerializer().DeserializeObject(pResponse)
                pRequestJson = pRequestJson("d")
                If pRequestJson.ContainsKey("Result") Then
                    If pRequestJson("Result") = "ok" Then
                        Return JsonDictionary.Serialize("Result", "ok")
                    Else
                        Return JsonDictionary.Serialize("Error", "Result was not ok vbR9BQH5EQGE!")
                    End If
                ElseIf pRequestJson.ContainsKey("Error") Then
                    Return JsonDictionary.Serialize("Error", pRequestJson("Error"))
                Else
                    Return JsonDictionary.Serialize("Error", "SchemaSyncReceiver/Receive replied neither ok nor error vbY6D2JTF24S!")
                End If
            Else
                Return JsonDictionary.Serialize("Error", "Reply from " & pRemoteIp & " was not in JSON vb5N5PGVRTXD!")
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <NonAction()>
    Public Shared Function CreateDatabaseMaps() As List(Of DatabaseMap)

        Dim pOut As List(Of DatabaseMap)

        pOut = New List(Of DatabaseMap)
#If DEBUG Then
        pOut.Add(New DatabaseMap("WidgetDev"))
#End If

        Return pOut

    End Function

    Public Class DatabaseMap
        Public SourceDatabaseName As String
        Public DestinationBaseFilename As String

        Public Sub New(vSourceDatabaseName As String)

            SourceDatabaseName = vSourceDatabaseName
            DestinationBaseFilename = vSourceDatabaseName

        End Sub

        Public Sub New(vSourceDatabaseName As String, vDestinationBaseFilename As String)

            SourceDatabaseName = vSourceDatabaseName
            DestinationBaseFilename = vDestinationBaseFilename

        End Sub
    End Class
End Class

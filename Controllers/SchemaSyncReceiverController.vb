Imports SchemaSync

Public Class SchemaSyncReceiverController
    Inherits System.Web.Mvc.AsyncController

    Private Shared WithEvents mSync As SchemaSync.Sync
    Private Shared mLastSyncResult As SchemaSync.Sync.CompleteArgs

    Public Function Receive() As String

        Dim pFilename As String
        Dim pFolder As String
        Dim pProZip As ProZip
        Dim pDtFilenames As DataTable
        Dim pDtMaps As List(Of SchemaSyncSenderController.DatabaseMap)

        Try
            If Request.Files.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Receiver must receive a file vbU9V8AY8QP8!")
            End If

            pFolder = Common.ServerPath & "temp\SchemaSyncReceiver\"
            pFilename = pFolder & "Snapshots.zip"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)

            If C1.C1Zip.C1ZipFile.IsZipFile(pFilename) = False Then
                Return JsonDictionary.Serialize("Error", "Snapshots.zip is not a valid zip file vbRA33AZ63TS!")
            End If

            pDtMaps = SchemaSyncSenderController.CreateDatabaseMaps()
            pProZip = New ProZip(pFilename)
            pDtFilenames = pProZip.GetFilenames()

            For Each pDtMap As SchemaSyncSenderController.DatabaseMap In pDtMaps
                If pDtFilenames.Select("FullName = " & ReadyString(pDtMap.DestinationBaseFilename & ".snap")).Length = 0 Then
                    Return JsonDictionary.Serialize("Error", pDtMap.DestinationBaseFilename & ".snap was must be present in the zip file vbEWN9KQPD8K!")
                End If
            Next pDtMap

            For Each pDtMap As SchemaSyncSenderController.DatabaseMap In pDtMaps
                pProZip.UnzipFile(pDtMap.DestinationBaseFilename & ".snap", pFolder & pDtMap.DestinationBaseFilename & ".snap")
            Next pDtMap

            pProZip.Close()
            IO.File.Delete(pFilename)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "vbZLYNXNS62R! " & ex.Message)

        End Try

    End Function

    <HttpPost()>
    Public Function Compare() As String

        Dim pFolder As String
        Dim pSnapshotFilename As String
        Dim pConnectionInfo As ConnectionInfo
        Dim pDtMaps As List(Of SchemaSyncSenderController.DatabaseMap)

        Try
            pFolder = Common.ServerPath & "temp\SchemaSyncReceiver\"
            pDtMaps = SchemaSyncSenderController.CreateDatabaseMaps()

            'pSnapshotFilename = pFolder & "Mobiz.snap"
            pSnapshotFilename = pFolder & "Private.snap"

            pConnectionInfo = Common.ConnectionInfo
            mSync = New SchemaSync.Sync()
            mSync.DestinationDb = pConnectionInfo
            mSync.DestinationDb.Server = pConnectionInfo.Server
            'mSync.DestinationDb.Database = "Mobiz"
            'mSync.DestinationDb.Database = "MobizTemplate"
            mSync.DestinationDb.Database = "MobizABC"

            mSync.DestinationDb.Username = pConnectionInfo.Username
            mSync.DestinationDb.Password = pConnectionInfo.Password
            mSync.DestinationDb.IntegratedSecurity = pConnectionInfo.IntegratedSecurity
            mSync.SourceSnapshotFilename = pSnapshotFilename
            TimedThread(
                Sub()
                    mLastSyncResult = Nothing
                    mSync.Execute()
                End Sub)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    <AsyncTimeout(60000 * 2)>
    Public Sub GetMessagesAsync()

        Dim pMessageIndex As Integer
        Dim pJson As New JsonDictionary

        AsyncManager.OutstandingOperations.Increment()

        pJson = New JsonDictionary

        If Integer.TryParse(Common.WebRequest.Data("MessageIndex"), pMessageIndex) = False Then
            pJson.Add("Error", "MessageIndex must be specified")
            AsyncManager.Parameters("vOut") = pJson
            AsyncManager.OutstandingOperations.Decrement()
            Exit Sub
        End If

        Call New Threading.Thread(
            Sub(vInput As Object)
                Dim pStart As Integer
                Dim pOut As JsonDictionary

                pOut = New JsonDictionary
                pStart = TickCount()

                Do Until TickCount() - pStart > 30000
                    If mSync IsNot Nothing _
                        AndAlso pMessageIndex < mSync.GetMessages().Count Then

                        pOut.Add("Index", mSync.GetMessages().Count)
                        If mSync.IsBusy = False Then
                            pOut.Add("IsDone", 1)
                            If mLastSyncResult IsNot Nothing Then
                                If mLastSyncResult.Success Then
                                    pOut.Add("Result", "ok")
                                    pOut.Add("Warnings", Truncate(mLastSyncResult.Result.Warnings, 1000))
                                    pOut.Add("Messages", Truncate(mLastSyncResult.Result.Messages, 1000))
                                Else
                                    pOut.Add("Result", "error")
                                    pOut.Add("Error", mLastSyncResult.Result.Errors)
                                End If
                            Else
                                pOut.Add("Result", "error")
                                pOut.Add("Error", "Unspecified error")
                            End If
                        End If
                        With pOut.AddList("Messages")
                            For p As Integer = pMessageIndex To mSync.GetMessages().Count - 1
                                .Add(mSync.GetMessages(p))
                            Next
                            Exit Do
                        End With
                    End If
                    Threading.Thread.Sleep(5000)
                Loop

                AsyncManager.Parameters("vOut") = pOut
                AsyncManager.OutstandingOperations.Decrement()
            End Sub
        ).Start(Common.WebRequest.AllData())

    End Sub

    Public Function GetMessagesCompleted(vOut As JsonDictionary) As String

        Return vOut.Serialize

    End Function

    Private Shared Sub mSync_Complete(sender As Object, e As SchemaSync.Sync.CompleteArgs) Handles mSync.Complete

        mLastSyncResult = e

    End Sub
End Class

Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class WebPosDb

    Shared ReadOnly mSemaphore As New SemaphoreSlim(1)
    Private Shared mTimeStamp As String

    Public Shared Function Start() As String

        If mSemaphore.Wait(2000) = False Then
            Return "Another export process is in progress."
        End If

        Try
            Return Process()
        Catch ex As Exception
            AddLog(ex.Message)
            Return ex.Message
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Private Shared Sub AddLog(vText As String)

        AppendToLog(vText,, "WebPosDb")

    End Sub

    Private Shared Function Process() As String

        Dim pRemoteServer = WebPosDbDownloaderController.GetRemoteServer()
        If String.IsNullOrWhiteSpace(pRemoteServer) Then
            Return "The ""settings/WebPosRemoteServer.txt"" file is blank or does not exist."
        End If

        mTimeStamp = DateTimeStamp()
        Using pProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="",
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password)

            Dim pHasPublicDatabase As Boolean
            Dim pPrivateDatabases As New List(Of String)
            Dim pHasUnflushedPublicDatabase As Boolean
            Dim pUnflushedPrivateDatabases As New List(Of String)

            Dim pFolder = AppendBackslash(Common.ServerPath) & "WebPosDb\"
            Dim pPath As String

            Dim pDatabases = pProData.GetDatabases(True, True)
            For Each pDatabase In pDatabases
                If pDatabase = "MobizWebPOSCache".ToLower() Then
                    ' just do nothing
                ElseIf pDatabase = "mobiz" Then
                    pPath = pProData.GetFirstDataFilePath(pDatabase)
                    If pPath.StartsWith(pFolder) Then
                        pHasPublicDatabase = True
                    End If
                ElseIf pDatabase.StartsWith("mobiz") Then
                    pPath = pProData.GetFirstDataFilePath(pDatabase)
                    If pPath.StartsWith(pFolder) Then
                        pPrivateDatabases.Add(pDatabase)
                    End If
                ElseIf pDatabase = WebPosFlush.UnflushedPrefix.ToLower() & "mobiz" Then
                    pPath = pProData.GetFirstDataFilePath(pDatabase)
                    If pPath.StartsWith(pFolder) Then
                        pHasUnflushedPublicDatabase = True
                    End If
                ElseIf pDatabase.StartsWith(WebPosFlush.UnflushedPrefix.ToLower()) Then
                    pPath = pProData.GetFirstDataFilePath(pDatabase)
                    If pPath.StartsWith(pFolder) Then
                        pUnflushedPrivateDatabases.Add(pDatabase)
                    End If
                End If
            Next

            Dim pDT As DataTable
            Dim pSourceFilename As String
            Dim pDestinationFilename As String

            If (
                    pHasPublicDatabase _
                    AndAlso pPrivateDatabases.Count = 0
                ) _
                OrElse
                (
                    pHasPublicDatabase = False _
                    AndAlso pPrivateDatabases.Count > 0
                ) Then

                If pHasPublicDatabase Then
                    AddLog("Attached public database has been found but no attached private database")
                    pDT = pProData.GetDatabaseFiles("mobiz")
                    pProData.DetachDatabase("mobiz")
                    For Each pDR As DataRow In pDT.Rows
                        pSourceFilename = pDR("physical_name")
                        pDestinationFilename = pFolder & mTimeStamp & "\" & IO.Path.GetFileName(pSourceFilename)
                        AddLog("Public database file will be moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                        CreateFolder(pDestinationFilename)
                        MoveFile(pSourceFilename, pDestinationFilename)
                        AddLog("Public database was successfully moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                    Next
                End If

                For Each pDatabase In pPrivateDatabases
                    pDT = pProData.GetDatabaseFiles(pDatabase)
                    pProData.DetachDatabase(pDatabase)
                    For Each pDR As DataRow In pDT.Rows
                        pSourceFilename = pDR("physical_name")
                        pDestinationFilename = pFolder & mTimeStamp & "\" & IO.Path.GetFileName(pSourceFilename)
                        AddLog("Private database, " & pDatabase & ", file will be moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                        CreateFolder(pDestinationFilename)
                        MoveFile(pSourceFilename, pDestinationFilename)
                        AddLog("Private database, " & pDatabase & ", file was successfully moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                    Next
                Next
            End If

            If (
                    pHasUnflushedPublicDatabase _
                    AndAlso pUnflushedPrivateDatabases.Count = 0
                ) _
                OrElse
                (
                    pHasUnflushedPublicDatabase = False _
                    AndAlso pUnflushedPrivateDatabases.Count > 0
                ) Then

                If pHasUnflushedPublicDatabase Then
                    pDT = pProData.GetDatabaseFiles(WebPosFlush.UnflushedPrefix & "mobiz")
                    pProData.DetachDatabase(WebPosFlush.UnflushedPrefix & "mobiz")
                    For Each pDR As DataRow In pDT.Rows
                        pSourceFilename = pDR("physical_name")
                        pDestinationFilename = pFolder & mTimeStamp & "\" & IO.Path.GetFileName(pSourceFilename)
                        AddLog("Unflushed public database file will be moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                        CreateFolder(pDestinationFilename)
                        MoveFile(pSourceFilename, pDestinationFilename)
                        AddLog("Unflushed public database was successfully moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                    Next
                End If

                For Each pDatabase In pUnflushedPrivateDatabases
                    pDT = pProData.GetDatabaseFiles(pDatabase)
                    pProData.DetachDatabase(pDatabase)
                    For Each pDR As DataRow In pDT.Rows
                        pSourceFilename = pDR("physical_name")
                        pDestinationFilename = pFolder & mTimeStamp & "\" & IO.Path.GetFileName(pSourceFilename)
                        AddLog("Unattached private database, " & pDatabase & ", file will be moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                        CreateFolder(pDestinationFilename)
                        MoveFile(pSourceFilename, pDestinationFilename)
                        AddLog("Unattached private database, " & pDatabase & ", file was successfully moved from " & pSourceFilename & " to " & pDestinationFilename & ".")
                    Next
                Next
            End If
        End Using

        Return "done"

    End Function
End Class
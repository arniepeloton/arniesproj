Imports Newtonsoft.Json

Public Class BackupArchiver

    Private Shared ReadOnly mExecuteLock As New Object
    Public BasePath As String
    Public ArchivePath As String
    Public NewestFileCountToKeep As Integer = 1
    Public DeleteFilesInsteadOfMove As Boolean

    Public Class Result
        Public Files As New List(Of File)
        Public Message As String

        Public Class File
            Public SourceFile As String
            Public DestinationFile As String
            Public Size As Long
            Public LastWriteTime As Date
            Public IsNewest As Boolean
        End Class

        Public ResultsCrc As String
        Public Function ComputeResultsCrc() As String
            Dim pText = JsonConvert.SerializeObject(Files)
            ResultsCrc = ProPack.Security.Crypto.GetCRC32(pText)

            Return ResultsCrc
        End Function

    End Class

    Public Function Execute() As Result

        Dim pOut As New Result

        BasePath = Trim2(BasePath, "\")
        If String.IsNullOrWhiteSpace(BasePath) Then
            Throw New ApplicationException("BasePath must not be blank. ref: vbF3SH4CHEXV!")
        End If
        If DeleteFilesInsteadOfMove = False Then
            If String.IsNullOrWhiteSpace(ArchivePath) Then
                Throw New ApplicationException("ArchivePath must not be blank. ref: vbNBCR9JPDR9!")
            End If
        End If

        SyncLock mExecuteLock
            Dim pFolders = New IO.DirectoryInfo(BasePath).GetDirectories()
            For Each pFolder In pFolders
                Dim pSubFolderResult = ProcessDirectory(pFolder)
                pOut.Files.AddRange(pSubFolderResult.Files)
            Next
        End SyncLock

        pOut.ComputeResultsCrc()
        Return pOut

    End Function

    Private Function ProcessDirectory(vDirectory As IO.DirectoryInfo) As Result

        Dim pOut As New Result
        Dim pFilteredFiles As New List(Of IO.FileInfo)
        Dim pAllFiles = vDirectory.GetFiles(vDirectory.Name & "*.bak")
        For Each pFile In pAllFiles
            ' put custom code here
            pFilteredFiles.Add(pFile)
        Next pFile

        Dim pSortedFiles = From pFile In pFilteredFiles
            Select pFile
            Order By pFile.LastWriteTime, pFile.Name

        Dim pOldFileCount As Integer
        For Each pFile In pSortedFiles
            Dim pDestinationFilename As String
            Dim pResultFile = New Result.File
            pResultFile.SourceFile = pFile.FullName
            pResultFile.LastWriteTime = pFile.LastWriteTime
            pResultFile.Size = pFile.Length

            If pSortedFiles.Count() - pOldFileCount <= NewestFileCountToKeep Then
                pResultFile.IsNewest = True
            ElseIf DeleteFilesInsteadOfMove = False Then
                pDestinationFilename = GetDestinationFilename(pFile.FullName)
                pResultFile.DestinationFile = pDestinationFilename
            End If
            pOut.Files.Add(pResultFile)
            pOldFileCount += 1
        Next

        Return pOut

    End Function

    Private Function GetDestinationFilename(vSourceFilename As String) As String

        Dim pRelativePath = GetRelativePath(BasePath, vSourceFilename)
        pRelativePath = Trim2(pRelativePath, "\")
        Dim pDestinationFilename = IO.Path.Combine(ArchivePath, pRelativePath)

        Return pDestinationFilename

    End Function

    Public Shared Function MoveFiles(vFiles As List(Of Result.File)) As Integer

        Dim pCount As Integer

        For Each pFile In vFiles
            If pFile.IsNewest = False Then
                MoveFile(pFile.SourceFile, pFile.DestinationFile)
                pCount += 1
            End If
        Next

        Return pCount

    End Function

    Public Shared Function DeleteFiles(vFiles As List(Of Result.File)) As Integer

        Dim pCount As Integer

        For Each pFile In vFiles
            If pFile.IsNewest = False Then
                IO.File.Delete(pFile.SourceFile)
                pCount += 1
            End If
        Next

        Return pCount

    End Function
End Class
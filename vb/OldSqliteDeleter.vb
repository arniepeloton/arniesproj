Imports System.Threading
Imports WebProj.OldSqliteDeleter

Public Class OldSqliteDeleter

    ''' <summary>
    ''' per org and per type (sqlite, sql server)
    ''' </summary>
    Private Const mFilesToRetain = 2
    Shared ReadOnly mSemaphore As New SemaphoreSlim(1)
    Shared mDateStarted As Date
    Shared mFolder As String
    Private Const mLogFilename As String = "OldSqliteDeleter"

    Public Shared Function Execute() As String

        If mSemaphore.Wait(2000) = False Then
            Return "Another old sqlite deleter process is in progress. Time started: " & Format(mDateStarted, cGlobal.FormatDateTimeSql)
        End If
        Try
            mDateStarted = Now
            AppendToLog("BEGIN: " & Format(mDateStarted, cGlobal.FormatDateTimeSql),, mLogFilename)
            mFolder = Common.ServerPath & "temp\DataExport\Upload"
            Dim pOldFiles = GetFiles()
            Dim pUniques = GetUniqueOrgIDAndFileTypes(pOldFiles)
            DisplayGroupedFiles(pUniques, pOldFiles)
            Summary(pOldFiles)
            DeleteFiles(pOldFiles)
            DeleteOldSubFolders(mFolder)
            AppendToLog("END: " & Format(mDateStarted, cGlobal.FormatDateTimeSql),, mLogFilename)
        Finally
            mSemaphore.Release()
        End Try

        Return Format(mDateStarted, cGlobal.FormatDateTimeSql)

    End Function

    Private Shared Function DeleteOldSubFolders(vFolder As String) As Boolean

        Dim pHasNewFile As Boolean = False
        Dim pFolders = System.IO.Directory.GetDirectories(vFolder)
        For Each pFolder In pFolders
            Dim pFiles = New System.IO.DirectoryInfo(pFolder).GetFiles()
            For Each pFile In pFiles
                If (Now - pFile.LastWriteTime).TotalDays > 1 Then
                    Try
                        pFile.Delete()
                        'AppendToLog("Deleting file: " & pFile.FullName,, mLogFilename)
                    Catch ex As Exception
                        AppendToLog("Error deleting file: " & pFile.FullName & vbCrLf &
                            "Error: " & ex.Message,, mLogFilename)
                    End Try
                Else
                    pHasNewFile = True
                End If
            Next

            If DeleteOldSubFolders(pFolder) = False Then
                pHasNewFile = True
            End If

            If pHasNewFile = False Then
                Try
                    IO.Directory.Delete(pFolder)
                    'AppendToLog("Deleting folder: " & pFolder,, mLogFilename)
                Catch ex As Exception
                    AppendToLog("Error deleting folder: " & pFolder & vbCrLf &
                        "Error: " & ex.Message,, mLogFilename)
                End Try
            End If
        Next

        Return Not pHasNewFile

    End Function

    Private Shared Function DeleteFiles(vAllFiles As List(Of OldFile)) As Integer

        Dim pOut As Integer = 0

        Try
            For Each pFile In vAllFiles
                If pFile.Deletable Then
                    Try
                        IO.File.Delete(pFile.Filename)
                        pOut += 1
                    Catch ex As Exception
                        Console.WriteLine("vb88JRH6FVRQ! " & ex.Message)
                    End Try
                End If
            Next
        Catch ex As Exception
            Console.WriteLine("vbDRXFV559SU! " & ex.Message)
        End Try

        Return pOut

    End Function

    Private Shared Function Summary(vAll As List(Of OldFile))

        Dim pSummary =
            From pOldFile In vAll
            Select pOldFile
            Group By pOldFile.Deletable
            Into Count = Count(),
            TotalSize = Sum(pOldFile.FileSize)

        For Each pItem In pSummary
            AppendToLog("Deletable: " & pItem.Deletable & vbCrLf &
                "File count: " & pItem.Count & vbCrLf &
                "File size: " & DisplayableSize(pItem.TotalSize),, mLogFilename)
        Next

    End Function

    Private Shared Function DisplayGroupedFiles(vUniques As List(Of OldFile), vAll As List(Of OldFile))

        For Each pUnique In vUniques
            AppendToLog("OrgID: " & pUnique.OrgID & vbCrLf &
                "FileType: " & [Enum].GetName(GetType(OldFile.eFileType), pUnique.FileType) & vbCrLf &
                "Total Size: " & DisplayableSize(pUnique.FileSize) & vbCrLf &
                "File count: " & pUnique.Count,, mLogFilename)
            Dim pSubItems =
                From pItem In vAll
                Select pItem
                Where pItem.OrgID = pUnique.OrgID _
                AndAlso pItem.FileType = pUnique.FileType
                Order By pItem.FileTime

            Dim pIndex = -1
            For Each pSubItem In pSubItems
                pIndex += 1
                If pSubItem.FileType = OldFile.eFileType.Invalid Then
                    pSubItem.Deletable = True
                ElseIf pSubItems.Count() - pIndex > mFilesToRetain Then
                    pSubItem.Deletable = True
                End If
                AppendToLog("Filename: " & pSubItem.Filename & vbCrLf &
                    "File Size: " & DisplayableSize(pSubItem.FileSize) & vbCrLf &
                    "File Time: " & Format(pSubItem.FileTime, cGlobal.FormatDateTimeSql) & vbCrLf &
                    "Deletable: " & pSubItem.Deletable,, mLogFilename)
            Next
        Next

    End Function

    Private Shared Function GetUniqueOrgIDAndFileTypes(vOldFiles As List(Of OldFile)) As List(Of OldFile)

        Dim pOut As New List(Of OldFile)

        Dim pUniques =
            From pOldFile In vOldFiles
            Select pOldFile
            Group pOldFile By pOldFile.OrgID, pOldFile.FileType
            Into TotalSize = Sum(pOldFile.FileSize),
            Count = Count()
            Order By OrgID, FileType

        For Each pUnique In pUniques
            Dim pOutItem = New OldFile
            pOutItem.OrgID = pUnique.OrgID
            pOutItem.FileType = pUnique.FileType
            pOutItem.FileSize = pUnique.TotalSize
            pOutItem.Count = pUnique.Count
            pOut.Add(pOutItem)
        Next

        Return pOut

    End Function

    Private Shared Function GetFiles() As List(Of OldFile)

        Dim pOut = New List(Of OldFile)
        Dim pAllFilenames = IO.Directory.GetFiles(mFolder, "*.zip")
        For Each pFilename In pAllFilenames
            Dim pZip = New ProZip(pFilename)
            Dim pDtInternalFilenames = pZip.GetFilenames()
            Dim pOrgID As String = ""
            Dim pIsSqlite As Boolean = False
            Dim pIsSqlServer As Boolean = False
            For Each pDrInternalFilename As DataRow In pDtInternalFilenames.Rows
                Dim pInternalFilename = pDrInternalFilename("Name")
                If IsGuid(pOrgID) = False Then
                    pOrgID = ParseOrgID(pInternalFilename)
                End If
                If pIsSqlite = False _
                    AndAlso pIsSqlServer = False Then
                    pIsSqlite = IsSqlite(pInternalFilename)
                End If
                If pIsSqlite = False _
                    AndAlso pIsSqlServer = False Then
                    pIsSqlServer = IsSqlServer(pInternalFilename)
                End If
            Next

            Dim pOldFile As New OldFile()
            pOldFile.Filename = pFilename
            If IsGuid(pOrgID) Then
                pOldFile.OrgID = ReadyGuid(pOrgID)
            End If
            If pIsSqlite Then
                pOldFile.FileType = OldFile.eFileType.Sqlite
            ElseIf pIsSqlServer Then
                pOldFile.FileType = OldFile.eFileType.SqlServer
            End If
            Dim pFileInfo = New IO.FileInfo(pFilename)
            pOldFile.FileSize = pFileInfo.Length
            pOldFile.FileTime = pFileInfo.LastWriteTime
            pOut.Add(pOldFile)
        Next

        Return pOut

    End Function

    Private Shared Function IsSqlite(vFilename As String) As Boolean

        If vFilename.ToLower().EndsWith(".public.db") Then
            Return True
        End If

    End Function

    Private Shared Function IsSqlServer(vFilename As String) As Boolean

        If vFilename.ToLower().EndsWith(".mdf") Then
            Return True
        End If

    End Function

    Private Shared Function ParseOrgID(vFilename As String) As String

        Dim pRegEx = New Regex("(?<OrgID>.+)\.txt")
        Dim pMatch = pRegEx.Match(vFilename)
        If pMatch.Success = False Then
            Return Nothing
        End If
        If pMatch.Groups("OrgID").Success = False Then
            Return Nothing
        End If
        Dim pOut = pMatch.Groups("OrgID").Value
        If IsGuid(pOut) = False Then
            Return Nothing
        End If
        pOut = ReadyGuid(pOut)
        Return pOut

    End Function

    Public Class OldFile
        Public Enum eFileType
            Invalid
            Sqlite
            SqlServer
        End Enum
        Public FileType As eFileType
        Public Filename As String
        Public FileSize As Long
        Public FileTime As Date
        Public OrgID As String
        ''' <summary>
        ''' number of files in the orgid-filetype group
        ''' </summary>
        Public Count As Integer
        Public Deletable As Boolean
    End Class
End Class

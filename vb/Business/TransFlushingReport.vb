Public Class TransFlushingReport

    Dim mProData As ProData
    Dim mParent As Business

    Private Shared ReadOnly mExtractLogsLock As New Object

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ExtractLogs() As Integer

        SyncLock mExtractLogsLock
            Dim pFolder = Common.ServerPath & "\temp\logs\ImportDataClass"

            Dim pFiles = IO.Directory.GetFiles(pFolder, "*.old")
            For Each pFile In pFiles
                MoveOldFile(pFile)
            Next

            pFiles = IO.Directory.GetFiles(pFolder, "*.txt")
            For Each pFile In pFiles
                ExtractLog(pFile)
            Next

            Return pFiles.Count
        End SyncLock

    End Function

    Private Sub MoveOldFile(vFilename As String)

        ' move the source file
        Dim pParentFolder = IO.Path.GetDirectoryName(vFilename)
        Dim pFilename = IO.Path.GetFileName(vFilename)
        Dim pDestinationFilename = IO.Path.Combine(pParentFolder, IO.File.GetLastWriteTime(vFilename).ToString("yyyy_MM_dd"), pFilename)
        CreateFolder(pDestinationFilename)
        IO.File.Move(vFilename, pDestinationFilename)

    End Sub

    Private Function ExtractLog(vFilename As String) As Boolean

        Dim pText = FileText(vFilename)
        Dim pLines = pText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim pLineIndex As Integer
        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pProcessName As String
        Dim pProcessStart As Date
        Dim pProcessEnd As Date
        Dim pProcessDuration As Integer
        Dim pDeviceName As String
        Dim pCompanyName As String

        For Each pLine In pLines
            If pLineIndex = 0 Then
                ExtractOrgIdDeviceId(pLine, pOrgID, pDeviceID)
                Dim pDR = mParent.LibraryOrganization.ByID(pOrgID)
                If pDR IsNot Nothing Then
                    pCompanyName = CStr2(pDR("CompanyName"))
                    pCompanyName = Truncate(pCompanyName, 500)
                    If String.IsNullOrWhiteSpace(CStr2(pDR("Database"))) = False Then
                        mParent.ProData.Execute("USE " & EscapeSqlName(pDR("Database")))
                        pDR = mParent.LibraryDevice.ById(pDeviceID)
                        If pDR IsNot Nothing Then
                            pDeviceName = CStr2(pDR("PhoneName")) & " - " & CStr2(pDR("UUID"))
                            pDeviceName = Truncate(pDeviceName, 500)
                        End If
                    End If
                End If
            Else
                ExtractProcessNameEtc(pLine, pProcessName, pProcessStart, pProcessEnd, pProcessDuration)
                Dim pIU = New InsertUpdate("MobizTransFlushingReport")
                pIU("OrganizationID") = ReadyString(pOrgID)
                pIU("DeviceID") = ReadyString(pDeviceID)
                pIU("CompanyName") = ReadyString(pCompanyName)
                pIU("DeviceName") = ReadyString(pDeviceName)
                pIU("ProcessStart") = ReadyString(Format(pProcessStart, cGlobal.FormatDateTimeSql2))
                pIU("ProcessEnd") = ReadyString(Format(pProcessEnd, cGlobal.FormatDateTimeSql2))
                pIU("ProcessDuration") = pProcessDuration
                Dim pSql = pIU.GetUpdate("LogFilename = " & ReadyString(vFilename) & vbCrLf &
                    " AND ProcessName = " & ReadyString(pProcessName))
                Dim pAffectRecords = mProData.ExecuteSQL(pSql)
                If pAffectRecords = 0 Then
                    pIU("LogFilename") = ReadyString(vFilename)
                    pIU("ProcessName") = ReadyString(pProcessName)
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If
            pLineIndex += 1
        Next

        ' move the source file
        Dim pParentFolder = IO.Path.GetDirectoryName(vFilename)
        Dim pFilename = IO.Path.GetFileName(vFilename)
        Dim pDestinationFilename = IO.Path.Combine(pParentFolder, IO.File.GetLastWriteTime(vFilename).ToString("yyyy_MM_dd"), pFilename & ".old")
        CreateFolder(pDestinationFilename)
        IO.File.Move(vFilename, pDestinationFilename)

    End Function

    Private Sub ExtractOrgIdDeviceId(vLine As String, ByRef oOrgID As String, ByRef oDeviceID As String)

        Dim pIndex1 As Integer
        Dim pIndex2 As Integer

        pIndex2 = vLine.LastIndexOf("\") - 1
        pIndex1 = vLine.LastIndexOf("\", pIndex2) + 1
        oDeviceID = vLine.Substring(pIndex1, pIndex2 - pIndex1 + 1)

        pIndex2 = pIndex1 - 2
        pIndex1 = vLine.LastIndexOf("\", pIndex2) + 1
        oOrgID = vLine.Substring(pIndex1, pIndex2 - pIndex1 + 1)

    End Sub

    Private Sub ExtractProcessNameEtc(vLine As String, ByRef oProcessName As String,
        ByRef oProcessStart As Date,
        ByRef oProcessEnd As Date,
        ByRef oProcessDuration As Integer)

        Dim pIndex1 As Integer
        Dim pIndex2 As Integer

        pIndex2 = vLine.Length - 2
        pIndex1 = vLine.LastIndexOf(" to ", pIndex2) + 4
        oProcessEnd = ParseDate(vLine.Substring(pIndex1, pIndex2 - pIndex1 + 1))

        pIndex2 = pIndex1 - 5
        pIndex1 = vLine.LastIndexOf(" from ", pIndex2) + 6
        oProcessStart = ParseDate(vLine.Substring(pIndex1, pIndex2 - pIndex1 + 1))

        oProcessDuration = (oProcessEnd - oProcessStart).TotalMilliseconds

        pIndex2 = vLine.LastIndexOf("|") - 1
        pIndex1 = vLine.LastIndexOf("|", pIndex2) + 1
        oProcessName = vLine.Substring(pIndex1, pIndex2 - pIndex1 + 1).Trim()
        oProcessName = Truncate(oProcessName, 500)

    End Sub

    ''' <summary>
    ''' requires current organization
    ''' </summary>
    ''' <param name="vDeviceID"></param>
    ''' <param name="vFilename">fileonly, no path, no extension</param>
    ''' <remarks></remarks>
    Public Sub SaveTransImport(vDeviceID As String,
        vFilename As String,
        vFullFilename As String,
        Optional vDontDeleteUnprocessedTransImport As Boolean = False)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vDontDeleteUnprocessedTransImport = False Then
            DeleteUnprocessedTransImport(vDeviceID)
        End If

        pIU = New InsertUpdate("TransImports")
        pIU("ID") = "NEWID()"
        pIU("DeviceID") = ReadyString(vDeviceID)
        pIU("Filename") = ReadyString(vFilename)
        pIU("ServerIdentity") = ReadyString(modClient.ServerIdentity)
        pIU("DateReceived") = "GETDATE()"
        pIU("OrigDateReceived") = "GETDATE()"
        pIU("FileSize") = New IO.FileInfo(vFullFilename).Length
        pSql = pIU.GetInsert()

        mProData.ExecuteSQL(pSql)

    End Sub

    Private Sub DeleteUnprocessedTransImport(vDeviceID As String)

        Dim pSql As String

        pSql = "DELETE FROM TransImports" &
            " WHERE ProcessStart IS NULL" &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

    End Sub
End Class
Imports WebProj.WebProj
Imports Newtonsoft.Json
Imports System.Net
Imports System.Collections.Specialized

Public Class EcommerceDataExporter
    Shared mStatusText As String
    Shared mIsBusy As Boolean
    Dim mMileStone As Milestone
    Dim pSettingsFileDate As Date

    Public Function StatusText() As String

        Return mStatusText

    End Function

    Public Function IsBusy() As String

        Return If(mIsBusy, 1, 0)

    End Function

    Public Sub Execute()

        If mIsBusy Then
            Return
        End If

#If DEBUG Then
        If Debugger.IsAttached = False Then
            Return
        End If
#End If

        mIsBusy = True
        If LoadMileStone() = False Then
            mIsBusy = False
            Return
        End If
        If mMileStone Is Nothing Then
            mIsBusy = False
            Return
        End If
        If mMileStone.IsDone Then
            mIsBusy = False
            Return
        End If

        If String.IsNullOrWhiteSpace(mMileStone.PublicServerUrl) Then
            mMileStone.ErrorMessage = "EcommerceDataSourceServerIP is blank. So, there is no need to export."
            Return
        End If

        If IsDestinationOnline() = False Then
            mMileStone.ErrorMessage = "Destination server is offline."
            Return
        End If

        If ExportToSqlite() = False Then
            mIsBusy = False
            Return
        End If
        If UploadZipFile() = False Then
            mIsBusy = False
            Return
        End If
        FollowUp()
        DownloadResult()
        ExtractResult()
        SaveResult()

    End Sub

    Private Function IsDestinationOnline() As Boolean

        Dim pWebClient As New WebClient
        Dim pUrl = mMileStone.PublicServerUrl & "EcommerceDataImporter/OnlineStatus"
        Dim pResult = pWebClient.DownloadString(pUrl)
        If pResult = "online" Then
            Return True
        End If

    End Function

    Private Function ExportToSqlite() As Boolean

        Dim pFolder = AppPath & "temp\EcommerceExportableTrans\" & DateTimeStamp() & "\"
        Dim pSqliteFilename = pFolder & "private.db"
        Dim pSql As String
        Dim pRecords As Integer

        Using pProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=mMileStone.PrivateDatabaseName,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

            Dim pProSqlite = New ProSqlite()
            CreateFolder(pSqliteFilename)
            pProSqlite.Open(pSqliteFilename)

            pSql = "SELECT TOP 25 *" & vbCrLf &
                " FROM TransInvoice" &
                " WHERE DateImported IS NULL" &
                " AND SalesOrderNo > 1" &
                " AND LastUpdated > '2020-07-02'" &
                " ORDER BY LastUpdated"
            Dim pDT = pProData.Execute(pSql)
            pRecords += pDT.Rows.Count
            pProSqlite.CreateTable("TransInvoice", pDT)

            If pDT.Rows.Count > 0 Then
                Dim pDtIds As DataTable = ExtractFieldValues(pDT, "ID", Integer.MaxValue)
                pSql = "SELECT *" & vbCrLf &
                    " FROM TransInvoiceItems" & vbCrLf &
                    " WHERE InvoiceID IN (" & pDtIds.Rows(0)("ID") & ")"
                pDT = pProData.Execute(pSql)
                pRecords += pDT.Rows.Count
                pProSqlite.CreateTable("TransInvoiceItems", pDT)
            End If
            pProSqlite.Close()
        End Using

        If pRecords = 0 Then
            Return False
        End If

        Dim pZipFilename = pFolder & "Export.zip"
        Dim pProZip = New ProZip()
        pProZip.Open(pZipFilename)
        pProZip.AddFile(pSqliteFilename)
        pProZip.Close()

        mMileStone.ExportableZipFilename = pZipFilename
        SaveMilestone()

        Return True

    End Function

    Private Function UploadZipFile()

        Try
            Dim pValues = New NameValueCollection()
            Dim pJson As New JsonDictionary()
            pJson.Add("OrgID", mMileStone.OrgID)
            pValues.Add("json", pJson.ToString())

            Dim pUrl = mMileStone.PublicServerUrl & "EcommerceDataImporter/Upload"
            Dim pResultText = UploadFile(mMileStone.ExportableZipFilename, pUrl, pValues)
            Dim pResult = JsonConvert.DeserializeObject(Of UploadZipFileResultJson)(pResultText)
            mMileStone.FollowUpKey = pResult.FollowUpKey
            SaveMilestone()
            Return True
        Catch ex As Exception

        End Try

    End Function

    Public Class UploadZipFileResultJson

        Public FollowUpKey As String
        Public Result As String
        Public ErrorMessage As String
        Public Status As String

    End Class

    Private Function FollowUp() As Boolean

        Do
            Try
                Dim pWebClient = New WebClient()
                mStatusText = "Following up"
                Dim pUrl = mMileStone.PublicServerUrl & "EcommerceDataImporter/GetResult?vOrgID=" & mMileStone.OrgID & "&vKey=" & mMileStone.FollowUpKey
                Dim pResultText = pWebClient.DownloadString(pUrl)
                Dim pResult = JsonConvert.DeserializeObject(Of UploadZipFileResultJson)(pResultText)
                If pResult Is Nothing Then
                    mStatusText = pResultText
                ElseIf pResult.Result = "done" Then
                    mMileStone.CanDownloadResult = True
                    Return True
                End If
                Threading.Thread.Sleep(10 * 1000)
            Catch ex As Exception
                mStatusText = ex.Message
            End Try
        Loop

    End Function

    Private Function DownloadResult() As Boolean

        Try
            Dim pWebClient = New WebClient()
            mStatusText = "Downloading result"
            Dim pUrl = mMileStone.PublicServerUrl & "EcommerceDataImporter\DownloadResult?vOrgID=" & mMileStone.OrgID & "&vKey=" & mMileStone.FollowUpKey
            Dim pFilename As String
            pFilename = IO.Path.GetDirectoryName(mMileStone.ExportableZipFilename)
            pFilename = IO.Path.Combine(pFilename, "Result.zip")
            pWebClient.DownloadFile(pUrl, pFilename)
            mMileStone.ResultZipFilename = pFilename
            SaveMilestone()
        Catch ex As Exception
            mStatusText = ex.Message
        End Try

    End Function

    Private Function ExtractResult() As Boolean

        Dim pZip = New ProZip(mMileStone.ResultZipFilename)
        pZip.Unzip(IO.Path.GetDirectoryName(mMileStone.ResultZipFilename))

    End Function

    Private Function SaveResult()

        Dim pSqlite = New ProSqlite
        Dim pFolder = AppendIfNeeded(IO.Path.GetDirectoryName(mMileStone.ResultZipFilename), "\")
        Dim pFilename As String

        pFilename = pFolder & "Result.private.db"
        If IO.File.Exists(pFilename) Then
            pSqlite.Open(pFilename)

            Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=mMileStone.PrivateDatabaseName,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                SaveTableResult(pSqlite, pProData, "TransInvoice")
            End Using
        End If

        mIsBusy = False
        'mMileStone.IsDone = True
        SaveMilestone()

    End Function

    Private Function SaveTableResult(vSourceSqlite As ProSqlite, vDestinationProData As ProData, vTableName As String) As Integer

        If vSourceSqlite.TableExists(vTableName) = False Then
            Return 0
        End If

        Dim pSql = "SELECT *" & vbCrLf &
            " FROM " & vTableName & vbCrLf
        Dim pDT = vSourceSqlite.Execute(pSql)

        For Each pDR As DataRow In pDT.Rows
            pSql = "UPDATE " & vTableName & vbCrLf &
                " SET DateImported = GETDATE()" & vbCrLf &
                " WHERE ID = " & ReadyString(pDR("ID"))
            vDestinationProData.ExecuteSQL(pSql)
        Next pDR

    End Function

#Region "setting"
    Public Shared Sub SaveSettings(vSettings As EcommerceDataExporterSettings)

        Dim pText = JsonConvert.SerializeObject(vSettings)
        FileText(SettingsFilename()) = pText

    End Sub

    Public Shared Function LoadSettings() As EcommerceDataExporterSettings

        Dim pFilename = SettingsFilename()
        If IO.File.Exists(pFilename) Then
            Dim pFileText = FileText(pFilename)
            Dim pOut = JsonConvert.DeserializeObject(Of EcommerceDataExporterSettings)(pFileText)
            Return pOut
        End If

    End Function

    Public Shared Function SettingsFileDate() As Date

        Dim pFilename = SettingsFilename()
        If IO.File.Exists(pFilename) Then
            Return IO.File.GetLastWriteTime(pFilename)
        End If

    End Function

    Private Shared Function SettingsFilename() As String

        Return AppendBackslash(AppPath) & "settings\EcommerceDataExporterSettings.txt"

    End Function

    Public Class EcommerceDataExporterSettings
        Public PublicServerUrl As String
        Public OrgID As String
        Public PrivateDatabaseName As String
    End Class
#End Region

    Public Class Milestone
        Public PublicServerUrl As String
        Public OrgID As String
        Public PrivateDatabaseName As String

        Public ExportableZipFilename As String
        Public FollowUpKey As String
        Public CanDownloadResult As Boolean
        Public ResultZipFilename As String

        Public IsDone As Boolean
        Public ErrorMessage As String
    End Class

    Public Sub SaveMilestone(vPublicServerUrl As String,
        vOrgID As String,
        vPrivateDatabaseName As String)

        mMileStone = New Milestone()
        mMileStone.PublicServerUrl = vPublicServerUrl
        mMileStone.OrgID = vOrgID
        mMileStone.PrivateDatabaseName = vPrivateDatabaseName
        SaveMilestone()

    End Sub

    Private Sub SaveMilestone()

        Dim pText = JsonConvert.SerializeObject(mMileStone, Formatting.Indented)
        FileText(MilestoneFilename()) = pText

    End Sub

    Private Function LoadMileStone() As Boolean

        Try
            Dim pSettings = LoadSettings()
            If pSettings Is Nothing Then
                Return False
            End If
            If String.IsNullOrWhiteSpace(pSettings.PublicServerUrl) Then
                Return False
            End If
            Dim pText = FileText(MilestoneFilename())
            mMileStone = JsonConvert.DeserializeObject(Of Milestone)(pText)
            If mMileStone Is Nothing Then
                mMileStone = New Milestone()
            End If
            mMileStone.OrgID = pSettings.OrgID
            mMileStone.PrivateDatabaseName = pSettings.PrivateDatabaseName
            mMileStone.PublicServerUrl = AppendIfNeeded(pSettings.PublicServerUrl, "/")
            Return True
        Catch ex As Exception
        End Try

    End Function

    Private Function MilestoneFilename() As String

        Return AppPath & "temp\ECommerceDataExporterMilestone.txt"

    End Function
End Class

Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class DeviceDataExporter
    Public OrgID As String
    Public DestinationDeviceID As String
    Private mProData As ProData
    Public Zip As ProZip
    Public Business As Business
    Dim mSettings As WebPosDbDownloaderController.WebPosSettings
    Public Limit As Integer

    ' output
    Public Key As String

    ''' <summary>
    ''' return the number of exported records
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Export() As Integer

        mProData = Business.ProData
        mSettings = WebPosDbDownloaderController.GetWebPosSettings(Business)
        If String.IsNullOrWhiteSpace(mSettings.UUID) Then
            Debug.WriteLine("vbXNEXRGWNHX! " & "The WebPOS setting has no UUID.")
            Return 0
        End If

        If Zip Is Nothing Then
            CreateZipFile()
        End If
        Dim pOut = ExportLibraryOrganizationCustomerCreditTerms()

        Return pOut

    End Function

    Private Sub CreateZipFile()

        Key = DateTimeStamp()
        Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\DeviceDataExporter\Outbound\" & OrgID & "\" & Key & ".zip"
        CreateFolder(pFilename)
        Zip = New ProZip(pFilename)
        Dim pJson As New JsonDictionary()
        pJson.Add("OrgID", OrgID)
        pJson.Add("AppVersion", "DeviceDataExporter")
        pJson.Add("AppVersionName", "DeviceDataExporter")
        pJson.Add("UUID", mSettings.UUID)
        pJson.Add("AppBuildDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
        pJson.Add("AppInstallDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
        Zip.AddString(pJson.ToString(), "info.json")

    End Sub

    Private Function ExportLibraryOrganizationCustomerCreditTerms() As Integer

        Dim pSql = " SELECT TOP " & Limit & " LibraryOrganizationCustomerCreditTerms.*" & vbCrLf &
            " FROM LibraryOrganizationCustomerCreditTerms" & vbCrLf &
            " LEFT JOIN TransDateImported ON TransDateImported.RowID = LibraryOrganizationCustomerCreditTerms.ID" & vbCrLf &
            " AND TransDateImported.TableName = 'LibraryOrganizationCustomerCreditTerms'" & vbCrLf &
            " AND " & ProData.RemoveMilliseconds("TransDateImported.RowLastUpdated") & " >= " & ProData.RemoveMilliseconds("LibraryOrganizationCustomerCreditTerms.LastUpdated") & vbCrLf &
            " AND TransDateImported.DeviceID = " & ReadyString(DestinationDeviceID) & vbCrLf &
            " WHERE TransDateImported.ID IS NULL" & vbCrLf &
            " ORDER BY LibraryOrganizationCustomerCreditTerms.LastUpdated"
        Dim pDT = mProData.Execute(pSql)
        Zip.AddTableCsv(pDT, "LibraryOrganizationCustomerCreditTerms")

        Return pDT.Rows.Count

    End Function
End Class
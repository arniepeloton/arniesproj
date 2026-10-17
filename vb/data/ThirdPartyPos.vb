Imports System.Collections.Specialized
Imports System.Net
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class ThirdPartyPos
    Dim mOrgID As String
    Public Business As Business

    Shared mThirdPartyPosIntegrationUrl As String
    Shared mThirdPartyPosFtpFolder As String
    Public Shared Status As Action(Of String)

    Public Shared Async Function Execute() As Task(Of String)
        ' for azure

        Await Task.Delay(1)
        Dim pUrl = GetThirdPartyPosIntegrationUrl()
        If String.IsNullOrWhiteSpace(pUrl) Then
            Return ""
        End If
        Dim pZipFilenames = DownloadZipFilenames(pUrl)
        ProcessFilenames(pZipFilenames)

        Return pZipFilenames

    End Function

    Shared Function GetThirdPartyPosIntegrationUrl() As String

        If mThirdPartyPosIntegrationUrl IsNot Nothing Then
            Return mThirdPartyPosIntegrationUrl
        End If

        Dim pFilename = Common.ServerPath & "settings\ThirdPartyPosIntegrationUrl.txt"
        If IO.File.Exists(pFilename) = False Then
            Status("Error: file, ThirdPartyPosIntegrationUrl, does not exist in this server. vbVHBUCJKHA8! " & pFilename)
            Return ""
        End If
        Dim pFileText = FileText(pFilename)
        If String.IsNullOrWhiteSpace(pFileText) Then
            Status("Error: file, ThirdPartyPosIntegrationUrl, is blank. vbM7KFEBYEN5! " & pFilename)
            Return ""
        End If
        mThirdPartyPosIntegrationUrl = pFileText
        Return mThirdPartyPosIntegrationUrl

    End Function

    Shared Sub ProcessFilenames(vZipFilenames As String)

        If vZipFilenames = "No file found." Then
            Status("No new zip files were found.")
            Return
        End If
        If vZipFilenames?.StartsWith("Error: ") Then
            Status(vZipFilenames)
            Return
        End If

        Dim pFilenames = vZipFilenames.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Status("Zip files to be downloaded: " & pFilenames.Length)
        For Each pFilename In pFilenames
            pFilename = Trim2(pFilename, "\")
            Dim pFullFilename = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads", pFilename)
            If IO.File.Exists(pFullFilename) Then
                Continue For
            End If
            Dim pTextFilename = IO.Path.Combine(IO.Path.GetDirectoryName(pFullFilename), IO.Path.GetFileNameWithoutExtension(pFullFilename), "._result.txt")
            If IO.File.Exists(pTextFilename) Then
                Continue For
            End If
            DownloadZip(pFilename)
        Next
        Status("Done")

    End Sub

    Shared Sub DownloadZip(vZipFilename As String)

        Dim pUrl = GetThirdPartyPosIntegrationUrl() & "/ThirdPartyPos/DownloadZipFile?filename=" & HttpUtility.UrlEncode(vZipFilename)
        Dim pFullFilename = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads", vZipFilename)
        CreateFolder(pFullFilename)
        Dim pWebClient = New WebClient()
        Status("Downloading zip file, " + pUrl)
        pWebClient.DownloadFile(pUrl, pFullFilename)
        Status("Downloaded zip file, " + pFullFilename)

    End Sub

    Public Shared Function DownloadZipFilenames(vUrl As String) As String
        ' for azure

        Try
            vUrl = AppendIfNeeded(vUrl, "/")
            vUrl &= "ThirdPartyPos/GetZipFilenames"

            Dim pWeb = New WebClient()
            Status("Downloading zip filenames from " & vUrl)
            Dim pDownloadResult = pWeb.DownloadString(vUrl)
            Status("Downloaded zip filenames from " & vUrl)
            If pDownloadResult = "" Then
                Status("Error: end point return an empty string. " & vUrl)
                Return "Error: end point return an empty string. " & vUrl
            End If

            Return pDownloadResult
        Catch ex As Exception
            Status("Error: in DownloadZipFilenames: " & ex.Message)
            Return "Error: " & ex.Message
        End Try

    End Function

    Public Shared Function GetThirdPartyPosFtpFolder() As String

        If mThirdPartyPosFtpFolder IsNot Nothing Then
            Return mThirdPartyPosFtpFolder
        End If

        Dim pFilename = Common.ServerPath & "settings\ThirdPartyPosFtpFolder.txt"
        If IO.File.Exists(pFilename) = False Then
            Throw New ApplicationException("File does not exist. vRGH7JAAEMD! " & pFilename)
        End If
        Dim pFileText = FileText(pFilename)
        If String.IsNullOrWhiteSpace(pFileText) Then
            Throw New ApplicationException("File is blank. vb4UL88JTNTG! " & pFilename)
        End If
        If IO.Directory.Exists(pFileText) = False Then
            Throw New ApplicationException("Folder (" & pFileText.Trim() & ") does not exist. Please setup config file (" & pFilename & ").")
        End If
        mThirdPartyPosFtpFolder = pFileText
        Return mThirdPartyPosFtpFolder

    End Function

    Public Shared Function GetZipFilenames() As String
        ' for ionos

        Dim pOut = ""
        Dim pDT = DirectoryToTable(GetThirdPartyPosFtpFolder(), vIncludeSubDirectories:=True, vMinDate:=Now.AddDays(-30))
        pDT.DefaultView.Sort = "LastWriteTime"
        For Each pDR As DataRowView In pDT.DefaultView
            Dim pExtension = CStr2(pDR("Extension"))
            If String.Equals(".zip", pExtension, StringComparison.InvariantCultureIgnoreCase) = False _
                AndAlso String.Equals(".csv", pExtension, StringComparison.InvariantCultureIgnoreCase) = False Then
                Continue For
            End If
            Dim pTextFilename = IO.Path.Combine(pDR("FullDirectoryName"), pDR("Name"))
            pTextFilename = IO.Path.Combine(pDR("FullDirectoryName"), IO.Path.GetFileNameWithoutExtension(pTextFilename) & "_result.txt")
            If IO.File.Exists(pTextFilename) Then
                Continue For
            End If
            If pOut <> "" Then
                pOut &= vbCrLf
            End If
            pTextFilename = GetRelativePath(GetThirdPartyPosFtpFolder(), pDR("FullName"))
            pOut &= pTextFilename
        Next

        If pOut = "" Then
            pOut = "No file found."
        End If

        Return pOut

    End Function

    Public Shared Function UploadResults() As String

        Dim pUrl = GetThirdPartyPosIntegrationUrl() & "/ThirdPartyPos/GetResultsFilenames"
        Dim pRemoteResultFilenamesString = New WebClient().DownloadString(pUrl)
        If String.IsNullOrWhiteSpace(pRemoteResultFilenamesString) Then
            Throw New ApplicationException("Blank response from " & pUrl & ". vbUQYZC5C8Z5!")
        End If

        Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads")
        Dim pLocalResultFilenamesString = GetResultsFilenames(pFolder)
        If pLocalResultFilenamesString = "No file found." Then
            Return pLocalResultFilenamesString
        End If

        Dim pOut As String = ""
        pRemoteResultFilenamesString = pRemoteResultFilenamesString.ToLower()
        Dim pRemoteResultFilenamesArray = pRemoteResultFilenamesString.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim pLocalResultFilenamesArray = pLocalResultFilenamesString.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        For Each pLocalFilename In pLocalResultFilenamesArray
            If pRemoteResultFilenamesArray.Contains(pLocalFilename.ToLower()) Then
                Continue For
            End If
            Dim pFullLocalFilename = IO.Path.Combine(pFolder, pLocalFilename)
            Dim pPostData = New NameValueCollection()
            pPostData.Add("RelativeFilename", pLocalFilename)
            UploadFile(pFullLocalFilename, GetThirdPartyPosIntegrationUrl() & "/ThirdPartyPos/UploadResultFile", pPostData)
            If pOut <> "" Then
                pOut &= vbCrLf
            End If
            pOut &= pLocalFilename
        Next

        Return pOut

    End Function

    Public Shared Function GetResultsFilenames(vFolder As String) As String
        ' for azure and ionos

        Dim pOut = ""
        Dim pDT = DirectoryToTable(vFolder, vIncludeSubDirectories:=True, vIncludeFilePattern:={"*_result.txt"},
            vMinDate:=Now.AddDays(-3000))
        pDT.DefaultView.Sort = "LastWriteTime"
        For Each pDR As DataRow In pDT.Rows
            If pOut <> "" Then
                pOut &= vbCrLf
            End If
            Dim pRelativeFilename = GetRelativePath(vFolder, pDR("FullName"))
            pRelativeFilename = Trim2(pRelativeFilename, "\")
            pOut &= pRelativeFilename
        Next

        If pOut = "" Then
            pOut = "No file found."
        End If

        Return pOut

    End Function

    Public Shared Sub SaveUploadedResultFile(vFile As HttpPostedFileBase, vRelativeFilename As String)

        Dim pFilename = IO.Path.Combine(GetThirdPartyPosFtpFolder(), vRelativeFilename)
        CreateFolder(pFilename)
        vFile.SaveAs(pFilename)

    End Sub
End Class

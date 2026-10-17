Imports System.Net
Imports ProPack.Security.Crypto
Imports System.Collections.Specialized
Imports System.Linq

Public Class TestController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult

        Return View()

    End Function

    Private Function HideResult() As Boolean

        If Request.IsLocal Then
            Return False
        End If

    End Function

#Region "Js"

    Public Function Js() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function JsTest() As String

        Dim pOut As JsonDictionary

        pOut = New JsonDictionary
        With pOut.AddList("Result")
            DataConnection(.AddDictionary())
        End With

        Return pOut.Serialize

    End Function

#End Region

#Region "Data"

    Public Function Data() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function DataTest() As String

        Dim pOut As JsonDictionary

        pOut = New JsonDictionary
        With pOut.AddList("Result")
            If DataConnection(.AddDictionary()) Then
                DataListDatabases(.AddDictionary())
                DataGetDatabaseFiles(.AddDictionary())
            End If
        End With

        Return pOut.Serialize

    End Function

    Private Function DataConnection(ByRef rJson As JsonDictionary) As Boolean

        Dim pDate As Date

        With rJson
            Try
                .Add("Title", "Data Connection")
                .Add("Description", "Test if app can connect to the database")
                Try
                    pDate = Common.WebRequest.ProData.GetDate
                    .Add("SuccessResult", "App can connect to the database. SQL Server date: " & Format(pDate, cGlobal.FormatDateTime))
                    Return True
                Catch ex As Exception
                    .Add("ErrorResult", "App cannot connect to the database: " & ex.Message)
                End Try
            Catch ex As Exception
                .Add("ErrorResult", ex.Message)
            End Try
        End With

    End Function

    Private Function DataListDatabases(ByRef rJson As JsonDictionary) As Boolean

        Dim pList As List(Of String)

        With rJson
            Try
                .Add("Title", "List Databases")
                .Add("Description", "List all databases in the current SQL Server instance")
                Try
                    pList = Common.WebRequest.ProData.GetDatabases
                    If Request.IsLocal Then
                        .Add("SuccessResult", "Number of databases: " & pList.Count &
                             ". You are on a local machine, so the databases are listed")
                        .Add("ListResult", pList)
                    Else
                        .Add("SuccessResult", "Number of databases: " & pList.Count &
                             ". You are not on a local machine, so the databases are not listed.")
                    End If
                Catch ex As Exception
                    .Add("ErrorResult", "App cannot list databases: " & ex.Message)
                End Try
            Catch ex As Exception
                .Add("ErrorResult", ex.Message)
            End Try
        End With

    End Function

    Private Function DataGetDatabaseFiles(ByRef rJson As JsonDictionary) As Boolean

        Dim pList As List(Of String)
        Dim pCurrentDatabase As String

        With rJson
            Try
                .Add("Title", "GetDatabaseFiles")
                .Add("Description", "GetDatabaseFiles description")
                Try
                    pCurrentDatabase = Common.WebRequest.ProData.GetCurrentDatabase


                    pList = Common.WebRequest.ProData.GetDatabases
                    If Request.IsLocal Then
                        .Add("SuccessResult", "Number of databases: " & pList.Count &
                             ". You are on a local machine, so the databases are listed")
                        .Add("ListResult", pList)
                    Else
                        .Add("SuccessResult", "Number of databases: " & pList.Count &
                             ". You are not on a local machine, so the databases are not listed.")
                    End If
                Catch ex As Exception
                    .Add("ErrorResult", "App cannot list databases: " & ex.Message)
                End Try
            Catch ex As Exception
                .Add("ErrorResult", ex.Message)
            End Try
        End With

    End Function
#End Region

#If DEBUG Then
    <HttpGet>
    Public Function AddToPeopleOrg()

        Const pUsername As String = "dd"
        Const pDatabase As String = "MobizABC"

        Dim pSql As String
        Dim pPeopleID As String
        Dim pOrgID As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate

        pSql = " SELECT PeopleID" & vbCrLf &
            " FROM LibraryPeopleUserAccount" & vbCrLf &
            " WHERE Username = " & ReadyString(pUsername)
        pPeopleID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
        If pPeopleID = "" Then
            Return "People with specified username does not exist"
        End If

        pSql = "SELECT OrganizationID" & vbCrLf &
            " FROM LibrarySubscription" & vbCrLf &
            " WHERE [Database] = " & ReadyString(pDatabase)
        pOrgID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
        If pOrgID = "" Then
            Return "Org with specified database does not exist"
        End If

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryPeopleOrganization" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(pPeopleID) & vbCrLf &
            " AND OrganizationID = " & ReadyString(pOrgID)
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return "PeopleOrg already exist"
        End If
        pIU = New InsertUpdate("LibraryPeopleOrganization")
        pIU("PeopleID") = ReadyString(pPeopleID)
        pIU("OrganizationID") = ReadyString(pOrgID)
        pIU("IsConfirmed") = 1
        pIU("DateTimeConfirmed") = "GETDATE()"
        pSql = pIU.GetInsert
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return "done"

    End Function

    <HttpGet>
    Public Function TestExportErpToMobiz()

        Dim p As New ErpToMobizExporter()

        p.Export()
        TestImportErpToMobiz(p.ZipFilename)

        Return "ok " & DateTimeStamp()

    End Function

    Private Function TestImportErpToMobiz(vSourceZipFilename As String) As Boolean

        Dim p As New ErpToMobizImporter

        p.SourceZipFilename = vSourceZipFilename
        p.Process()

    End Function

#Region " test split upload "
    <HttpGet>
    Public Function Upload() As String

        Dim pSourceFile As String
        Dim pSplitDestination As String
        Dim pChuckSize As Integer = 100 * 1000
        Dim pError As String
        Dim pFileSplits As List(Of String)
        Dim pCrc1, pCrc2 As String

        pCrc1 = ProPack.Security.Crypto.GetCRC32FromFile(pSourceFile)
        pSplitDestination = Common.ServerPath & "temp\SplitUpload\"
        CreateFolder(pSplitDestination)
        SplitFile(pSourceFile, pSplitDestination, pChuckSize, Nothing)

        ' during import
        pSourceFile = Common.ServerPath & "temp\SplitUpload\2015_03_18__15_19_37_0295[001 of 238].zip"
        pFileSplits = CheckParts(pSourceFile, pError)

        pSourceFile = Common.ServerPath & "temp\SplitUpload\merged.zip"
        MergeFiles(pFileSplits, pSourceFile, pError, True)
        pCrc2 = ProPack.Security.Crypto.GetCRC32FromFile(pSourceFile)

        pError = pCrc1 = pCrc2

    End Function

    <HttpGet>
    Public Function TestUploadSplitStart() As String

        Dim pWeb As WebClient
        Dim pSourceFile As String
        Dim pSplitDestination As String
        Dim pChuckSize As Integer = 100 * 1000
        Dim pJsonDictionary As JsonDictionary
        Dim pJsonStr As String
        Dim pCrc As String
        Dim pFileSplits As List(Of String)
        Dim pError As String
        Dim pUploadResult As String
        Dim pSplitFileManifest As SplitFileManifest

        pSplitFileManifest = New SplitFileManifest()
        pSourceFile = Common.ServerPath & "temp\ExportErpToMobiz\2015_03_18__15_19_37_0295.zip"
        pSplitDestination = Common.ServerPath & "temp\SplitUpload\"

        CreateFolder(pSplitDestination)
        SplitFile(pSourceFile, pSplitDestination, pChuckSize, pFileSplits)

        pJsonDictionary = New JsonDictionary
        pSplitFileManifest.Size = New IO.FileInfo(pSourceFile).Length
        pCrc = GetCRC32FromFile(pSourceFile)
        pSplitFileManifest.Crc = pCrc
        pSplitFileManifest.SplitCount = pFileSplits.Count


        For Each pFileSplit As String In pFileSplits
            pCrc = GetCRC32FromFile(pFileSplit)
            pSplitFileManifest.Splits.Add( _
                New SplitFileManifestSplit With { _
                    .Filename = IO.Path.GetFileName(pFileSplit), _
                    .Size = New IO.FileInfo(pFileSplit).Length, _
                    .Crc = pCrc
                } _
            )
        Next pFileSplit
        pJsonStr = New JavaScriptSerializer().Serialize(pSplitFileManifest)

        pWeb = New WebClient()
        pUploadResult = pWeb.UploadString(Common.VirtualPath & "/Test/UploadSplitStart", pJsonStr)

    End Function

    <HttpGet>
    Public Function TestUploadSplitFile() As String

        Dim pSourceFile As String
        Dim pDestination As String
        Dim pValues As NameValueCollection
        Dim pResult As String

        pSourceFile = Common.ServerPath & "temp\SplitUpload\2015_03_18__15_19_37_0295[001 of 238].zip"
        pDestination = Common.VirtualPath & "/Test/UploadSplitFile"
        pValues = New NameValueCollection()
        pValues.Add("Key", "2015_03_19__13_52_24_0738_gxn19edl0d")
        pResult = UploadFile(pSourceFile, pDestination, pValues)

        Return pResult

    End Function

    <HttpGet>
    Public Function TestGetMissingFile() As String

        Dim pWeb As WebClient
        Dim pResult As String
        Dim pJson As JsonDictionary
        Dim pJsonStr As String

        Dim pJsonObj As Dictionary(Of String, Object)
        Dim pAllUploaded As Object
        Dim pFilename As Object

        pJson = New JsonDictionary()
        pJson.Add("Key", "2015_03_19__13_52_24_0738_gxn19edl0d")
        pJsonStr = pJson.ToString()

        pWeb = New WebClient()
        pResult = pWeb.UploadString(Common.VirtualPath & "/Test/GetMissingFile", pJsonStr)

        pJsonObj = New JavaScriptSerializer().DeserializeObject(pResult)
        If pJsonObj.TryGetValue("AllUploaded", pAllUploaded) _
            AndAlso CDec2(pAllUploaded) <> 0 Then

            Return JsonDictionary.ToString("AllUploaded", 1)
        Else
            If pJsonObj.TryGetValue("Filename", pFilename) Then

            Else
                Return JsonDictionary.ToString("Error", "Filename is expected here vb9KEJQMBBSY!")
            End If
        End If

    End Function

    <HttpGet>
    Public Function TestUploadSplitLoop() As String

        Dim pWeb As WebClient
        Dim pResult As String
        Dim pJson As JsonDictionary
        Dim pJsonStr As String

        Dim pJsonObj As Dictionary(Of String, Object)
        Dim pAllUploaded As Object
        Dim pFilename As Object

        pJson = New JsonDictionary()
        pJson.Add("Key", "2015_03_19__13_52_24_0738_gxn19edl0d")
        pJsonStr = pJson.ToString()

        pWeb = New WebClient()
        pResult = pWeb.UploadString(Common.VirtualPath & "/Test/GetMissingFile", pJsonStr)

        Do
            pJsonObj = New JavaScriptSerializer().DeserializeObject(pResult)
            If pJsonObj.TryGetValue("AllUploaded", pAllUploaded) _
                AndAlso CDec2(pAllUploaded) <> 0 Then

                Return JsonDictionary.ToString("AllUploaded", 1)
            Else
                If pJsonObj.TryGetValue("Filename", pFilename) Then
                    Dim pSourceFile As String
                    Dim pDestination As String
                    Dim pValues As NameValueCollection

                    pSourceFile = Common.ServerPath & "temp\SplitUpload\" & pFilename
                    pDestination = Common.VirtualPath & "/Test/UploadSplitFile"
                    pValues = New NameValueCollection()
                    pValues.Add("Key", "2015_03_19__13_52_24_0738_gxn19edl0d")
                    pResult = UploadFile(pSourceFile, pDestination, pValues)
                Else
                    Return JsonDictionary.ToString("Error", "Filename is expected here vb9KEJQMBBSY!")
                End If
            End If
        Loop

    End Function
#End Region

    <HttpPost>
    Public Function UploadSplitStart() As String

        Dim pPath As String = "temp\SplitInbox\"
        Dim pData As Dictionary(Of String, Object)
        Dim pStr As String
        Dim pFolderName As String
        Dim pManifestFilename As String
        Dim pOut As JsonDictionary
        Dim pManifest As SplitFileManifest

        pStr = Common.WebRequest.InputStreamText

        Try
            pManifest = New JavaScriptSerializer().Deserialize(Of SplitFileManifest)(pStr)
        Catch ex As Exception
            Return JsonDictionary.ToString("Error", "Input data must be in JSON vb4S4446WGC2!")
        End Try

        pFolderName = DateTimeStamp() & "_" & GenerateString(10)
        pManifestFilename = Common.ServerPath & "temp\SplitInbox\" & pFolderName & "\manifest.txt"
        FileText(pManifestFilename) = pStr

        pOut = New JsonDictionary()
        pOut.Add("Key", pFolderName)
        pOut.Add("Filename", pManifest.Splits(0).Filename)

        Return pOut.ToString()

    End Function

    <HttpPost>
    Public Function UploadSplitFile() As String

        Dim pKey As String
        Dim pPath As String
        Dim pManifest As SplitFileManifest
        Dim pManifestFilename As String
        Dim pFilename As String
        Dim pCrc As String
        Dim pOut As JsonDictionary

        pKey = Request.Form("Key")
        If CStr2(pKey).Trim().Length = 0 Then
            Return JsonDictionary.ToString("Error", "Key is required vbQMYVLAK7JP!")
        End If
        If Request.Files.Count = 0 Then
            Return JsonDictionary.ToString("Error", "No file uploaded vbZU3FPQGZ49!")
        End If
        pPath = Common.ServerPath & "temp\SplitInbox\" & pKey & "\"
        If IO.Directory.Exists(pPath) = False Then
            Return JsonDictionary.ToString("Error", "Folder with the specified key does not exist vbJABNHKHJEG!")
        End If
        pManifestFilename = pPath & "manifest.txt"
        If IO.File.Exists(pManifestFilename) = False Then
            Return JsonDictionary.ToString("Error", "Manifest file does not exist vbCU4HXW3UGE!")
        End If
        pManifest = LoadManifest(pManifestFilename)
        pFilename = IO.Path.GetFileName(Request.Files(0).FileName)
        Dim pSplit As SplitFileManifestSplit = (From p As SplitFileManifestSplit
                In pManifest.Splits
                Where p.Filename = pFilename
                ).FirstOrDefault
        If pSplit Is Nothing Then
            Return JsonDictionary.ToString("Error", "The uploaded file does not exist in the manifest vbE5VKH8C2Z6!")
        End If

        pFilename = pPath & pFilename
        Request.Files(0).SaveAs(pFilename & "_temp")
        If pSplit.Size <> New IO.FileInfo(pFilename & "_temp").Length Then
            Return JsonDictionary.ToString("Error", "The size of the uploaded file and the size on the manifest do not match vbH7UZFRYJ6Q!")
        End If
        pCrc = GetCRC32FromFile(pFilename & "_temp")
        If pCrc <> pSplit.Crc Then
            Return JsonDictionary.ToString("Error", "The CRC of the uploaded file and the CRC on the manifest do not match vbUCL3A9T44B!")
        End If
        IO.File.Move(pFilename & "_temp", pFilename)

        pOut = New JsonDictionary()
        pOut.Add("Result", "ok")
        pFilename = GetMissingFile(pPath, pManifest.Splits)
        If pFilename Is Nothing Then
            pOut.Add("AllUploaded", 1)
        Else
            pFilename = IO.Path.GetFileName(pFilename)
            pOut.Add("Filename", pFilename)
        End If

        Return pOut.ToString()

    End Function

    <HttpPost>
    Public Function GetMissingFile(vKey As String)

        Dim pKey As String
        Dim pPath As String
        Dim pManifest As SplitFileManifest
        Dim pManifestFilename As String
        Dim pOut As JsonDictionary
        Dim pFilename As String

        pKey = Common.WebRequest.Data("Key")
        If CStr2(pKey).Trim().Length = 0 Then
            Return JsonDictionary.ToString("Error", "Key is required vb67B5N763NH!")
        End If
        pPath = Common.ServerPath & "temp\SplitInbox\" & pKey & "\"
        If IO.Directory.Exists(pPath) = False Then
            Return JsonDictionary.ToString("Error", "Folder with the specified key does not exist vbQ4VLDFGNEF!")
        End If
        pManifestFilename = pPath & "manifest.txt"
        If IO.File.Exists(pManifestFilename) = False Then
            Return JsonDictionary.ToString("Error", "Manifest file does not exist vbWWVZQVPDJU!")
        End If
        pManifest = LoadManifest(pManifestFilename)

        pOut = New JsonDictionary()
        pOut.Add("Result", "ok")
        pFilename = GetMissingFile(pPath, pManifest.Splits)
        If pFilename Is Nothing Then
            pOut.Add("AllUploaded", 1)
        Else
            pFilename = IO.Path.GetFileName(pFilename)
            pOut.Add("Filename", pFilename)
        End If

        Return pOut.ToString()

    End Function

    Private Function GetMissingFile(vFolder As String, vSplits As List(Of SplitFileManifestSplit)) As String

        Dim pFilename As String

        For Each pSplit As SplitFileManifestSplit In vSplits
            pFilename = vFolder & pSplit.Filename
            If IO.File.Exists(pFilename) = False Then
                Return pFilename
            End If
        Next pSplit

    End Function

    Private Function LoadManifest(vFilename As String) As SplitFileManifest

        Dim pStr As String
        Dim pOut As SplitFileManifest

        pStr = FileText(vFilename)
        pOut = New JavaScriptSerializer().Deserialize(Of SplitFileManifest)(pStr)

        Return pOut

    End Function

    'Private Function VerifyManifest(vJsonString As String, ByRef oError As String) As Boolean

    '    oError = ""

    '    Try
    '        Dim p As Dictionary(Of String, Object) = Common.WebRequest.AllData("File")
    '        If p.ContainsKey("Size") = False Then
    '            oError = "JSON File.Size must exist vbZAHJFXK7PE!"
    '            Return False
    '        End If
    '        If Integer.TryParse(p("Size"), Nothing) = False Then
    '            oError = "JSON File.Size must be an integer vbC4W8ULPMP9!"
    '            Return False
    '        End If
    '        If p.ContainsKey("Crc") = False Then
    '            oError = "JSON File.Crc must exist vbYVJNY33YKJ!"
    '            Return False
    '        End If
    '        If p.ContainsKey("SplitCount") = False Then
    '            oError = "JSON File.SplitCount must exist vb56L7KS39PC!"
    '            Return False
    '        End If
    '        If Integer.TryParse(p("SplitCount"), Nothing) = False Then
    '            oError = "JSON File.SplitCount must be an integer vbC6AS4JLL9M!"
    '            Return False
    '        End If
    '    Catch ex As Exception
    '        oError = "vbKCU7B38WK2! " & ex.Message
    '        Return False
    '    End Try

    '    Dim p2 As Object() = Common.WebRequest.AllData("Splits")

    'End Function

    Public Class SplitFileManifest
        Public Crc As String
        Public Size As Integer
        Public SplitCount As Integer
        Public Splits As List(Of SplitFileManifestSplit)

        Public Sub New()

            Splits = New List(Of SplitFileManifestSplit)

        End Sub
    End Class

    Public Class SplitFileManifestSplit
        Public Filename As String
        Public Crc As String
        Public Size As Integer
    End Class

#End If
End Class

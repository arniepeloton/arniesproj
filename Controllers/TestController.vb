Imports ProPack.Security.Crypto
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Net
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports Newtonsoft.Json
Imports System.IO
Imports System.Threading.Tasks
Imports System.Threading
Imports C1.C1Zip
Imports WebProj.WebProj.Data
Imports System.Net.Http
Imports System.Net.Http.Headers

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
            pSplitFileManifest.Splits.Add(
                New SplitFileManifestSplit With {
                    .Filename = IO.Path.GetFileName(pFileSplit),
                    .Size = New IO.FileInfo(pFileSplit).Length,
                    .Crc = pCrc
                }
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

    <HttpGet>
    Public Function GetRecursives() As String

        WebRequest.Instance.Business.LibraryStock.GetRecursiveContents(
            vOrganizationID:="CFE39DF6-8809-4973-9515-810DF75AED12",
            vParentProductID:="0234ED86-2269-46BE-8921-29F0A6DD5DC4",
            vChildProductID:="0234ED86-2269-46BE-8921-29F0A6DD5DC4"
        )

    End Function

    <HttpGet>
    Public Function TestCacheBalanceEachFowst() As String

        WebRequest.Instance.Business.TransInventoryLedger.CacheBalanceEachFowst()

        Return DateTimeStamp()

    End Function

    Public Function TestProDataError() As String

        Dim pSql As String

        Try
            pSql = "SELECT * FROM non_existing_table"
            Common.WebRequest.ProData.Execute(pSql)
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function TestTableToExcel() As String

        Dim pDT As New DataTable
        Dim pDR As DataRow
        Dim pSourceFilename As String = Common.ServerPath & "temp\TableToExcel data.xml"
        Dim pDestinationFilename As String = Common.ServerPath & "temp\TestTableToExcel.xls"

        pDT.ReadXml(pSourceFilename)
        TableToExcelFile(pDT, "", "", pDestinationFilename)

    End Function

    Public Function TestSortingListCreator() As String

        Dim p As New SortingListCreator
        Return p.CreateSortingLists()

    End Function

    Public Function TestPriceRequest() As String

        Dim pBusiness As Business
        Dim pPriceRequest As TransSalesOrderLegacy.PriceRequest
        Dim pMessage As String
        Dim pResult As Boolean
        Dim pProduct As TransSalesOrderLegacy.Product
        Dim pList As List(Of TransSalesOrderLegacy.Product)

        pList = New List(Of TransSalesOrderLegacy.Product)

        pProduct = New TransSalesOrderLegacy.Product
        pProduct.StockID = "f6d112c8-a499-46cf-85f0-440d25624679"
        pProduct.PriceLevelID = "01000012-0005-0000-0000-000000000000"
        pProduct.ActualQuantity = 1
        pProduct.Quantity = 9
        pProduct.UnitPrice = 10
        pProduct.UnitMeasure = "bottle"
        pList.Add(pProduct)

        pProduct = New TransSalesOrderLegacy.Product
        pProduct.StockID = "1597e112-532d-4e8b-8c3f-51c6767198b5"
        pProduct.PriceLevelID = "01000012-0005-0000-0000-000000000000"
        pProduct.ActualQuantity = 1
        pProduct.Quantity = 13
        pProduct.UnitPrice = 100
        pProduct.UnitMeasure = "pc"
        pList.Add(pProduct)

        pProduct = New TransSalesOrderLegacy.Product
        pProduct.StockID = "9dda9d84-d0a5-4f7f-905f-1a003d3a1508"
        pProduct.PriceLevelID = "01000012-0005-0000-0000-000000000000"
        pProduct.ActualQuantity = 1
        pProduct.Quantity = 45
        pProduct.UnitPrice = 79
        pProduct.UnitMeasure = "pc"
        pList.Add(pProduct)

        pProduct = New TransSalesOrderLegacy.Product
        pProduct.StockID = "5af8a535-b6f1-4805-94d9-3a0151714ba1"
        pProduct.PriceLevelID = "01000012-0005-0000-0000-000000000000"
        pProduct.ActualQuantity = 1
        pProduct.Quantity = 52
        pProduct.UnitPrice = 62
        pProduct.UnitMeasure = "pc"
        pList.Add(pProduct)

        pBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))
        pPriceRequest = New TransSalesOrderLegacy.PriceRequest(pBusiness)
        pPriceRequest.CustomerID = "724091d1-f9dc-4409-9b91-fed568622ea7"
        pPriceRequest.GetItems(pList)
        pResult = pPriceRequest.HandleRequestedPrices(False, pMessage)

    End Function
#End If

    Public Function InCacheInvoicesButNotInGroup() As String

        Dim pBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:="(local)\sql2019",
            vDatabase:="MobizCinco_Corporation",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))
        Dim pDT = pBusiness.LibraryOrganizationFOWST.AddInCacheInvoicesButNotInGroup(
            vInvoiceDateFrom:=New Date(2024, 4, 1),
            vInvoiceDateTo:=New Date(2024, 4, 30),
            vGroupName:="CO - Mindanao",
            vOrgID:=modClient.CincoOrgID)

        Return DateTimeStamp()

    End Function

    Public Function TestDucCreateBackOrders() As String

        Dim pOut As Integer

        WebRequest.Instance.OrgID = modClient.DucOrgID
        WebRequest.Instance.HandleOrganization(True)
        'pOut = WebRequest.Instance.Business.TransBackOrders.CreateBackOrders()

        Return pOut

    End Function

    ''' <summary>
    ''' include in production. apr 1, 2016
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateOrdersFromBackOrders() As String

        Dim pOut As Integer

        WebRequest.Instance.OrgID = modClient.DucOrgID
        WebRequest.Instance.HandleOrganization(True)
        'pOut = WebRequest.Instance.Business.TransBackOrders.CreateOrders()

        Return pOut

    End Function

    Public Function TestManagerSalesReport() As String

        Dim pJsonStr As String
        Dim pWeb As WebClient
        Dim pUploadResult As String
        Dim pJson As JsonDictionary

        pJson = New JsonDictionary
        pJson.Add("OrgID", "9d4b1500-13c0-49f9-a8e5-a16c13ec899e")
        pJson.Add("FowstID", "abe6d501-3cad-4e79-88ab-f81bbeac9a93")
        pJson.Add("Username", "cherrieann")
        pJson.Add("HashedPassword", "y+0d2LTLJPYkCMutmCuTc/o73Czijts1jx2fxbyuvE0Kulj/7RlmfiWs/veltx5gDPmUDdrky8FmAtC7KaBuwDAxM3gweA==")
        pJson.Add("RangeType", "Today")
        pJson.Add("RangeYear", "Today")

        pJsonStr = pJson.ToString()
        'pJsonStr = JsonDictionary.Serialize("json", pJsonStr)

        pWeb = New WebClient()
        pUploadResult = pWeb.UploadString(Common.VirtualPath & "/ReportsPOS/getManagerSalesReportPaymentInfoByMachine", "POST", pJsonStr)

        Return pUploadResult

    End Function

    Shared ReadOnly mSemaphoreCompressLatestFlushLogsPerOrgDevice As New SemaphoreSlim(1)
    Shared mGetAllFilesStart As Date

    <HttpGet> Public Function CompressLatestFlushLogsPerOrgDevice() As ActionResult

        Dim pDontRelease As Boolean
        Try
        If mSemaphoreCompressLatestFlushLogsPerOrgDevice.Wait(2000) = False Then
            Return Content("another CompressLatestFlushLogsPerOrgDevice process is running")
        End If

            Dim pCreateNew = CBool2(WebRequest.Instance.QueryString("CreateNew"))
            If pCreateNew = False Then
                Dim pFolder = Common.ServerPath & "temp\Uploads\"
                Dim pDirectory = New IO.DirectoryInfo(pFolder)
                If pDirectory.Exists() Then
                    Dim pFiles = pDirectory.GetFiles("Lastest_*.zip")
                    Dim pSortedFiles = pFiles.OrderBy(Function(vFile) vFile.LastWriteTime).Reverse()
                    For Each pFile In pSortedFiles
                        If pFile.Name.Contains("_pending") Then
                            Exit For
                        Else
                            Return File(pFile.FullName, "application/zip", pFile.Name)
                        End If
                    Next
                End If
            End If
            mGetAllFilesStart = Now
            TimedThread(
                Sub()
        Try
            Dim pFilename As String
            Dim pContentType As String = "application/zip"

            Using p As New FlushLogCompressor
                p.OrgsFolder = Common.ServerPath & "temp\Uploads\"
                p.ArchiveOrgsFolder = Trim2(Common.ServerPath, "\") & "_archive\temp\Uploads\"
                p.Execute()
                pFilename = p.ZipFilename
            End Using

                        'Return File(pFilename, pContentType, IO.Path.GetFileName(pFilename))
        Catch ex As Exception
                        'Return Content(ex.Message & "<br />" & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
                        Debug.WriteLine(ex.Message)
                    Finally
                        mSemaphoreCompressLatestFlushLogsPerOrgDevice.Release()
                    End Try
                End Sub)
            pDontRelease = True
            Return Content("mGetAllFilesStart: " & mGetAllFilesStart)
        Catch ex As Exception
            Dim pStr = ex.Message & vbCrLf &
                GetStackTrace(, ex)
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return Content(pStr)
        Finally
            If pDontRelease = False Then
            mSemaphoreCompressLatestFlushLogsPerOrgDevice.Release()
            End If
        End Try

    End Function

    Private Class FlushLogCompressor
        Implements IDisposable

        Public OrgsFolder As String
        Public ArchiveOrgsFolder As String
        Dim mZip As ProZip
        Dim mZipFilename As String
        Dim mProData As ProData
        Dim mBusiness As Business

        Public Sub Execute()

            mProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
            mBusiness = New Business(mProData)
            mZipFilename = AppendBackslash(OrgsFolder) & "Lastest_" & DateTimeStamp() & ".zip"
            Dim pTempZipFilename = AppendBackslash(OrgsFolder) & "Lastest_" & DateTimeStamp() & "_pending.zip"
            mZip = New ProZip(pTempZipFilename)
            ExecuteRoot()
            MoveFile(pTempZipFilename, mZipFilename)

        End Sub

        Public ReadOnly Property ZipFilename As String
            Get

                Return mZipFilename

            End Get
        End Property

        Private Sub ExecuteRoot()

            Dim pOrgFolders() As String
            Dim pOrgFolderName As String

            pOrgFolders = IO.Directory.GetDirectories(OrgsFolder)
            For Each pOrgFolder As String In pOrgFolders
                Try
                    pOrgFolderName = IO.Path.GetFileName(pOrgFolder)
                    If IsGuid(pOrgFolderName) Then
                        ExecuteOrg(pOrgFolder, pOrgFolderName)
                    End If
                Catch ex As Exception
                    Debug.WriteLine("Just ignore the errors here. vbLE3LG26Q64!")
                End Try
            Next pOrgFolder

        End Sub

        Private Sub ExecuteOrg(vOrgFolder As String, vOrgID As String)

            Dim pDeviceFolders() As String
            Dim pDeviceFolderName As String
            Dim pOrgFolderName As String
            Dim pDR As DataRow

            vOrgID = ReadyGuid(vOrgID)
            pOrgFolderName = vOrgID

            pDR = mBusiness.LibraryOrganization.ByID(vOrgID)
            If pDR Is Nothing Then
                pOrgFolderName &= " not in LibraryOrganization"
            ElseIf pDR("Database") Is DBNull.Value Then
                pOrgFolderName &= " not a subscriber; " & Truncate(CreateValidFilename(CStr2(pDR("CompanyName"))), 50)
            ElseIf CStr(pDR("Database")).ToLower().StartsWith("mobiz") Then
                pOrgFolderName &= " " & Truncate(CreateValidFilename(CStr(pDR("Database")).Substring(5)), 50)
                mProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
            Else
                pOrgFolderName &= " " & Truncate(CreateValidFilename(pDR("Database")), 50)
                mProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
            End If

            pDeviceFolders = IO.Directory.GetDirectories(vOrgFolder)
            For Each pDeviceFolder As String In pDeviceFolders
                pDeviceFolderName = IO.Path.GetFileName(pDeviceFolder)
                If IsGuid(pDeviceFolderName) Then
                    ExecuteDevice(pDeviceFolder, pDeviceFolderName, pOrgFolderName)
                End If
            Next pDeviceFolder

        End Sub

        Private Sub ExecuteDevice(vDeviceFolder As String, vDeviceID As String, vOrgName As String)

            Dim pFolder As IO.DirectoryInfo
            Dim pFiles() As IO.FileInfo
            Dim pLastestFile As IO.FileInfo = Nothing
            Dim pEntryFilename As String
            Dim pDR As DataRow
            Dim pDeviceName As String

            vDeviceID = ReadyGuid(vDeviceID)
            pDeviceName = vDeviceID & " "
            pDR = mBusiness.LibraryDevice.ById(vDeviceID)
            If pDR Is Nothing Then
                pDeviceName &= "not in LibraryDevice"
            Else
                'If CStr2(pDR("UUID")).Equals(FlushZipConverter.DeviceUuid, StringComparison.InvariantCultureIgnoreCase) Then
                '    Exit Sub
                'End If
                pDeviceName &= Truncate(CreateValidFilename(CStr2(pDR("UUID"))), 50)
            End If

            If String.IsNullOrWhiteSpace(ArchiveOrgsFolder) = False Then
                Dim pDtFiles = DirectoryToTable(vFolder:=vDeviceFolder)
                Dim pDvFiles = New DataView(pDtFiles, "", "LastWriteTime", DataViewRowState.CurrentRows)
                Dim pArchivedCount As Integer = 0
                For Each pDrvFile In pDvFiles
                    If pDvFiles.Count - pArchivedCount > 100 Then
                        Dim pRelativePath = GetRelativePath(OrgsFolder, pDrvFile("FullName"))
                        pRelativePath = IO.Path.Combine(ArchiveOrgsFolder, pRelativePath)
                        MoveFile(pDrvFile("FullName"), pRelativePath)
                        pArchivedCount += 1
                    Else
                        Exit For
                    End If
                Next
            End If

            pFolder = New IO.DirectoryInfo(vDeviceFolder)
            pFiles = pFolder.GetFiles("*_result.txt")
            If pFiles.Length > 0 Then
                For Each pFile As IO.FileInfo In pFiles
                    If pLastestFile Is Nothing Then
                        pLastestFile = pFile
                    ElseIf pLastestFile.CreationTime < pFile.CreationTime Then
                        pLastestFile = pFile
                    End If
                Next pFile

                pEntryFilename = AppendBackslash(vOrgName)
                pEntryFilename &= AppendBackslash(pDeviceName)
                pEntryFilename &= pLastestFile.Name
                mZip.AddFileRename(pLastestFile.FullName, pEntryFilename)
            End If


        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Try
                        mZip.Close()
                    Catch ex As Exception
                        Debug.Print("vbLCBMQPT2VW! " & ex.Message)
                    End Try
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    <HttpGet>
    Public Function TestNetSuite() As String

        Dim pJsonStr As String
        Dim pWeb As WebClient
        Dim pUploadResult As String
        Dim pJson As JsonDictionary

        pJson = New JsonDictionary
        pJson.Add("recordtype", "transferorder")
        pJson.Add("location", "22")
        pJson.Add("transferlocation", "20")
        With pJson.AddList("items")
            With .AddDictionary
                .Add("item", "15225")
                .Add("quantity", "123")
            End With
            With .AddDictionary
                .Add("item", "15135")
                .Add("quantity", "456")
            End With
        End With

        pJsonStr = pJson.ToString()
        'pJsonStr = JsonDictionary.Serialize("json", pJsonStr)

        Dim pHeader As String
        Const pAccount As String = "3772489"
        Const pEmail As String = "customercare@mobiz-apps.com"
        Const pSignature As String = "Netsuite123"

        pHeader = "NLAuth nlauth_account=" + pAccount + ", nlauth_email=" + pEmail + ", nlauth_signature= " + pSignature
        pWeb = New WebClient()
        pWeb.Headers.Add("Authorization", pHeader)
        pWeb.Headers.Add("Content-Type", "application/json")
        pUploadResult = pWeb.UploadString("https://rest.netsuite.com/app/site/hosting/restlet.nl?script=196&deploy=1", "POST", pJsonStr)
        ' transfer order transaction number

        Return pUploadResult

    End Function

    <HttpGet>
    Public Function GetPimFromNetSuite() As String



    End Function

    <HttpGet>
    Public Function AutoRunCreateDevAlertsSkipNumberAll() As String

        Dim pDT As DataTable

        pDT = WebRequest.Instance.Business.TransInvoice.AutoRunCreateDevAlertsSkipNumberAll()
        If pDT Is Nothing Then
            Return "nothing " & DateTimeStamp()
        End If
        If pDT.Rows.Count = 0 Then
            Return "no rows " & DateTimeStamp()
        End If

        Return "count: " & pDT.Rows.Count

    End Function

    <HttpGet>
    Public Function ImportPimFromNetSuiteExcel() As String

        Dim pFilename As String
        Dim pPimImporter As PimImporter
        Dim pCount As Integer

        pFilename = Common.ServerPath & "data\from NetSuite Items.xlsx"
        If IO.File.Exists(pFilename) = False Then
            Return pFilename & " does not exist"
        End If

        pPimImporter = New PimImporter()
        pPimImporter.Business = WebRequest.Instance.Business
#If DEBUG Then
        pPimImporter.SetOrgID(modClient.AbcOrgID)
#Else
        pPimImporter.SetOrgID(modClient.DarlingtonTrailOrgID)
#End If
        pCount = pPimImporter.ImportFromExcel(pFilename)

        Return JsonDictionary.Serialize("Count", pCount)

    End Function

    ''' <summary>
    ''' this is still useful. function TrailNetSuiteImportLibraryFromExcel does not make this function obsolete
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet>
    Public Function ImportFowstFromNetSuiteExcel() As String

        Dim pFilename As String
        Const pBaseFilename As String = "Darlington Trail Warehouse Locations.csv"
        Dim pDR As DataRow

        pFilename = Common.ServerPath & "data\" & pBaseFilename
        If IO.File.Exists(pFilename) = False Then
            Return pBaseFilename & " does not exist"
        End If

        pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(DarlingtonTrailOrgID)
        If pDR Is Nothing Then
            Return "Organization, " & DarlingtonTrailOrgID & ", does not exist."
        End If
        If CStr2(pDR("Database")) = "" Then
            Return "Organization, " & DarlingtonTrailOrgID & ", does not have a database."
        End If

        WebRequest.Instance.ProData.Execute("USE " & EscapeSqlName(pDR("Database")))
        WebRequest.Instance.Business.LibraryOrganizationFOWST.CreateFromExcel(DarlingtonTrailOrgID, pFilename)

        Return DateTimeStamp()

    End Function

    <HttpGet>
    Public Function TrailNetSuiteImportSupplierFowst() As String

        Dim pFilename As String
        Const pBaseFilename As String = "customer supplier fowst.csv"
        Dim pDR As DataRow
        Dim pDT As DataTable
        Dim pError As String
        Dim pSupplierFowstID As String
        Dim pDrCustomer As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount, pCountAll As Integer

#If DEBUG Then
        Const pOrgID As String = modClient.AbcOrgID
#Else
        Const pOrgID As String = modClient.DarlingtonTrailOrgID
#End If

        pFilename = Common.ServerPath & "data\" & pBaseFilename
        If IO.File.Exists(pFilename) = False Then
            Return pBaseFilename & " does not exist"
        End If

        pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(pOrgID)
        If pDR Is Nothing Then
            Return "Organization, " & pOrgID & ", does not exist."
        End If
        If CStr2(pDR("Database")) = "" Then
            Return "Organization, " & pOrgID & ", does not have a database."
        End If

        WebRequest.Instance.ProData.Execute("USE " & EscapeSqlName(pDR("Database")))

        pDT = ExcelToTable(pFilename, New String() {"Customer ID", "Customer/Organization", "Supplier Fowst ID", "Supplier/outlet"}, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Return pError
        End If
        For Each pDR In pDT.Rows
            pSupplierFowstID = WebRequest.Instance.Business.LibraryOrganizationFOWST.GetOrCreateByStoreID(pOrgID, pDR("Supplier Fowst ID"), pDR("Supplier/outlet"))
            pDrCustomer = WebRequest.Instance.Business.LibraryOrganization.GetCustomerByName(pDR("Customer/Organization"))
            If pDrCustomer Is Nothing Then
                Debug.Print("vbFGSWW77AU9! Customer not found: " & pDR("Customer/Organization"))
            Else
                pIU = New InsertUpdate("LibraryOrganizationCustomer")
                pIU("NetSuiteSupplierFowstID") = ReadyString(pSupplierFowstID)
                pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(pDrCustomer("OrganizationID")) &
                    " AND ISNULL(CONVERT(varchar(36), NetSuiteSupplierFowstID), '') <> " & ReadyString(pSupplierFowstID))
                pCount = WebRequest.Instance.ProData.ExecuteSQL(pSql)
                pCountAll += pCount
            End If
        Next pDR
        WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryOrganizationCustomer")

        Return pCountAll

    End Function

    Public Function TrailNetSuiteImportLibraryFromExcel() As String

        Dim p As TrailNetSuite
        Dim pError As String

        Try
            p = New TrailNetSuite(Common.WebRequest.ProData)
#If DEBUG Then
            p.OrgID = modClient.AbcOrgID
#Else
        p.OrgID = modClient.DarlingtonTrailOrgID
#End If
            p.ImportLibraryFromExcel()
            Return JsonDictionary.Serialize("Success", True)
        Catch ex As Exception
            pError = ex.Message & vbCrLf & GetStackTrace(, ex)
            pError = pError.Replace(vbCrLf, vbCrLf & "<br />")
            Return JsonDictionary.Serialize("Error", pError)
        End Try

    End Function

    <HttpGet>
    Public Function SaveDailyCustomerInvoice() As Integer

        Dim pCount As Integer
        Dim pError As String

        WebRequest.Instance.OrgID = "cfe39df6-8809-4973-9515-810df75aed12"
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            pCount = WebRequest.Instance.Business.LibraryCustomers.SaveDailyCustomerInvoice()
        Else
            Return pError
        End If

        Return pCount

    End Function

    <HttpGet>
    Public Function SaveMonthyCustomerInvoice() As Integer

        Dim pCount As Integer
        Dim pError As String

        WebRequest.Instance.OrgID = "cfe39df6-8809-4973-9515-810df75aed12"
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            pCount = WebRequest.Instance.Business.LibraryCustomers.SaveMonthlyCustomerInvoice()
        Else
            Return pError
        End If

        Return pCount

    End Function

    Public Function TestFrown()

        Dim pWebClient As New WebClient
        Dim pJson As String
        Dim pFrown As Frown

        pJson = pWebClient.UploadString("http://philhr:8080/CRMDashboard/GetUnsentSmS?OrgID=28b8a792-cb65-4d92-988f-d721123c93b9&IsMobile=0", "POST", "")
        pFrown = JsonConvert.DeserializeObject(Of Frown)(pJson)
        For Each pRow As Frown.dClass.UpdateDataClass.RowsClass In pFrown.d.UpdateData.Rows
            Debug.WriteLine(pRow.Message)
        Next pRow

        Return ""

    End Function

    Public Function Testing() As String

        Return DateTimeStamp() & " ; ref: vb23UDQX7T3Q!"

    End Function

    Class Frown
        Public d As dClass

        Public Class dClass
            Public UpdateData As UpdateDataClass

            Public Class UpdateDataClass
                Public Rows As List(Of RowsClass)

                Public Class RowsClass
                    Public ID As String
                    Public SendTo As String
                    Public SendFrom As String
                    Public Message As String
                    Public LastUpdated As String
                    Public SenttoModemTries As String
                    Public SentbyModem As String
                    Public SentbyTelco As String
                End Class
            End Class
        End Class
    End Class

    Public Function DeleteTransactionsFromFowsts() As String

        Const pCompanyName As String = "ABC Company Inc."
        'Dim pFowsts As String() = {"Abstergo Industries", "Butik"}
        'Const pCompanyName As String = "ATHLETIC CLUB SPORTS COMPANY, INC."
        'Const pCompanyName As String = "ATHLETIC CLUB SPORTS COMPANY, INC."
        'Dim pFowsts As String() = {"EASTWOOD MALL", "ROBINSON GALLERIA", "SHOPPESVILLE", "FAIRVIEW TERRACES"}
        'Dim pFowsts As String() = {"SM CITY NORTH EDSA", "Trinoma"}
        'Dim pFowsts As String() = {"Trinoma", "M PLACE", "SM FAIRVIEW", "FAIRVIEW TERRACES"}
        'Dim pFowsts As String() = {"Trinoma", "SM CITY NORTH EDSA"}
        'Dim pFowsts As String() = {"The Paper Tree Sta. Ana1"} 
        'Dim pFowsts As String() = {"NIKE-GMall1"}
        Dim pFowsts As String() = {"Butik"}
        Const pDate As Date = #10/2/2018#

        Dim pSql As String
        Dim pOrgID As String
        Dim pDR As DataRow
        Dim pFlatFowsts As String = ""
        Dim pDtFowsts As DataTable
        Dim pTotalDeleted As Integer = 0

        Try
            If Now.Date <> pDate Then
                Throw New Exception("pDate: " & pDate)
            End If

            pSql = " SELECT ID" & vbCrLf &
                " FROM MobizLibraryOrganization" & vbCrLf &
                " WHERE CompanyName = " & ReadyString(pCompanyName)
            pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                Throw New Exception("Organization, " & pCompanyName & ", does not exist in MobizLibraryOrganization")
            End If
            pOrgID = CStr2(pDR("ID"))

            pSql = " SELECT [Database]" & vbCrLf &
                " FROM MobizLibrarySubscription" & vbCrLf &
                " WHERE OrganizationID = " & ReadyString(pOrgID)
            pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                Throw New Exception("Organization, " & pCompanyName & ", does not exist in MobizLibrarySubscription")
            End If

            pSql = "USE " & EscapeSqlName(pDR("Database"))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.BeginTransaction()

            For Each pStr As String In pFowsts
                If pFlatFowsts <> "" Then
                    pFlatFowsts &= ", "
                End If
                pFlatFowsts &= ReadyString(pStr)
            Next pStr
            pSql = " SELECT ID" & vbCrLf &
                " INTO #tblFowsts" & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " WHERE Name IN (" & pFlatFowsts & ") " & vbCrLf &
                " AND OrganizationID = " & ReadyString(pOrgID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "SELECT * FROM #tblFowsts"
            pDtFowsts = Common.WebRequest.ProData.Execute(pSql)
            If pDtFowsts.Rows.Count <> pFowsts.Length Then
                Throw New Exception("pFowsts.Length: " & pFowsts.Length & "; #tblFowsts: " & pDtFowsts.Rows.Count)
            End If

            pSql = " SELECT ID" & vbCrLf &
                " INTO #tblInvoices" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE FowstID IN (SELECT ID FROM #tblFowsts)"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " SELECT ID, CustomerPaymentID" & vbCrLf &
                " INTO #tblCustomerPaymentInvoices" & vbCrLf &
                " FROM TransCustomerPaymentInvoices " & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " SELECT ID" & vbCrLf &
                " INTO #tblCustomerPayment" & vbCrLf &
                " FROM TransCustomerPayment  " & vbCrLf &
                " WHERE ID IN (SELECT CustomerPaymentID FROM #tblCustomerPaymentInvoices)"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransCustomerPaymentInvoices " & vbCrLf &
                " WHERE ID IN (SELECT CustomerPaymentID FROM #tblCustomerPaymentInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransCustomerPayment " & vbCrLf &
                " WHERE ID IN (SELECT ID FROM #tblCustomerPayment)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " UPDATE TransInvoice" & vbCrLf &
                " SET InvoiceNumber = 0" & vbCrLf &
                " WHERE ID IN (SELECT ID FROM #tblInvoices)"
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceCharges" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceDiscount" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceItemSerials" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceItemsRebates" & vbCrLf &
                " WHERE ModifierGroupID IN (" & vbCrLf &
                "   SELECT ModifierGroupID" & vbCrLf &
                "   FROM TransInvoiceItems" & vbCrLf &
                "   JOIN #tblInvoices ON #tblInvoices.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                ")"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoiceItems" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransMemoSalesReturn" & vbCrLf &
                " WHERE InvoiceID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInvoice" & vbCrLf &
                " WHERE ID IN (SELECT ID FROM #tblInvoices)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " UPDATE LibraryDeviceFowst" & vbCrLf &
                " SET POSLastORNo = 0" & vbCrLf &
                " ,POSLastSRVNo = 0" & vbCrLf &
                " ,POSLastRefNo = 0" & vbCrLf &
                " ,POSLastSONo = 0" & vbCrLf &
                " ,POSLastQuotationNo = 0" & vbCrLf &
                " ,POSLastReceivingNo = 0" & vbCrLf &
                " ,POSLastTransferOutNo = 0" & vbCrLf &
                " ,BomMakerLastNum = 0" & vbCrLf &
                " ,ReceivingNum = 0" & vbCrLf &
                " ,ReceivingTransferNum = 0" & vbCrLf &
                " ,POSLastPONo = 0" & vbCrLf &
                " WHERE FowstID IN (SELECT ID FROM #tblFowsts)"
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInventoryLedgerOldBalances" & vbCrLf &
                " WHERE TransInventoryLedgerOldBalances.InventoryLedgerID IN (" & vbCrLf &
                " 	SELECT ID" & vbCrLf &
                "     FROM TransInventoryLedger" & vbCrLf &
                "     WHERE FowstID IN (SELECT ID FROM #tblFowsts)" & vbCrLf &
                " )"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInventoryLedgerAdjustment" & vbCrLf &
                " WHERE TransInventoryLedgerAdjustment.TransInventoryLedgerID IN (" & vbCrLf &
                " 	SELECT ID" & vbCrLf &
                "     FROM TransInventoryLedger" & vbCrLf &
                "     WHERE FowstID IN (SELECT ID FROM #tblFowsts)" & vbCrLf &
                " )"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInventoryLedger" & vbCrLf &
                "     WHERE FowstID IN (SELECT ID FROM #tblFowsts)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInventoryProductCountStock" & vbCrLf &
                " WHERE TransInventoryProductCountStock.InventoryProductCountID IN (" & vbCrLf &
                " 	SELECT ID " & vbCrLf &
                "     FROM TransInventoryProductCount" & vbCrLf &
                "     WHERE FowstID IN (SELECT ID FROM #tblFowsts)" & vbCrLf &
                " )"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = " DELETE FROM TransInventoryProductCount" & vbCrLf &
                "     WHERE FowstID IN (SELECT ID FROM #tblFowsts)"
            pTotalDeleted += Common.WebRequest.ProData.ExecuteSQL(pSql)

            WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("MobizLibraryOrganization")
            Common.WebRequest.ProData.CommitTransaction()

            Return "Total deleted rows: " & pTotalDeleted
        Catch ex As Exception
            Return (ex.Message & vbCrLf & GetStackTrace(, ex)).Replace(vbCrLf, "<br />")
        End Try

    End Function
    Public Function PriceChangeUpdate() As String

        Dim p As PriceChanger
        Dim pResult As PriceChanger.UpdateResult
        Dim pProData As ProData
        Dim pBusiness As Business
        Dim pOut As String

        p = New PriceChanger
        pProData = Common.WebRequest.ProData
        p.ProData = pProData
        pBusiness = New Business(pProData)

        p.DestinationOrgID = "945E68D1-611B-4402-BF38-D9E3A01D4620" ' MobizDUC_Mobiz_PIM_Test

        ' p.DestinationOrgID = modClient.AbcOrgID
        p.DestinationPriceLevelID = pBusiness.SystemDefaults().GetLandedCostID()

        p.SourceOrgID = modClient.DucOrgID
        p.SourcePriceLevelID = pBusiness.SystemDefaults().GetWholesalePriceID()

        pResult = p.Update()
        pOut = JsonConvert.SerializeObject(pResult, Formatting.Indented)

        p.DestinationPriceLevelID = pBusiness.SystemDefaults().GetRetailPriceID()
        pResult = p.Update()

        pOut &= JsonConvert.SerializeObject(pResult, Formatting.Indented)
        pOut = pOut.Replace(vbCrLf, "<br />")

        Return pOut

    End Function

    Public Function ImportMeasureConversion() As String

        Dim pFilename As String
        Dim pDT As DataTable
        Dim pError As String
        Dim pDrConversion As DataRow
        Dim pResults As ImportMeasureConversionResults
        Dim pResult As ImportMeasureConversionResults.Item

        pFilename = "Unit-of-Measure-UOM-Conversion-Table-5-2-2014.xls"
        pFilename = Common.ServerPath & "data\" & pFilename
        If IO.File.Exists(pFilename) = False Then
            Return "File, " & pFilename & " does not exist."
        End If

        pDT = ExcelToTable(pFilename, {"Origin UOM", "Destination UOM", "UOM Family", "Conversion Factor"}, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Return pError
        End If

        If pDT Is Nothing _
            OrElse pDT.Rows.Count = 0 Then

            Return "Excel contains no valid row"
        End If

        pResults = New ImportMeasureConversionResults()
        For Each pDR As DataRow In pDT.Rows
            pResult = pResults.Add(pDR("Spreadsheet Row Number"))
            If String.IsNullOrWhiteSpace(CStr2(pDR("Origin UOM"))) Then
                pResult.Message &= "Origin UOM must not be blank. "
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("Destination UOM"))) Then
                pResult.Message &= "Destination UOM must not be blank. "
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("UOM Family"))) Then
                pResult.Message &= "UOM Family must not be blank. "
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("Conversion Factor"))) Then
                pResult.Message &= "Conversion Factor must not be blank. "
            ElseIf IsNumeric(pDR("Conversion Factor")) = False Then
                pResult.Message &= "Conversion Factor must a number. "
            ElseIf CDec2(pDR("Conversion Factor")) <= 0 Then
                pResult.Message &= "Conversion Factor must greater than zero. "
            End If
            If String.IsNullOrWhiteSpace(pResult.Message) = False Then
                Continue For
            End If
            If pDR("Spreadsheet Row Number") = 169 Then
                Stop2()
            End If
            pDrConversion = WebRequest.Instance.Business.LibraryStockMeasurements.GetConversion(
                vBaseUnit:=pDR("Origin UOM"),
                vDestUnit:=pDR("Destination UOM"))
            If pDrConversion Is Nothing Then
                WebRequest.Instance.Business.LibraryStockMeasurements.AddConversion(
                    vBaseUnit:=pDR("Origin UOM"),
                    vDestUnit:=pDR("Destination UOM"),
                    vFamily:=pDR("UOM Family"),
                    vFactor:=pDR("Conversion Factor"))
                pResult.Success = True
            Else

            End If

            ' add the inverse
            pDrConversion = WebRequest.Instance.Business.LibraryStockMeasurements.GetConversion(
                vBaseUnit:=pDR("Destination UOM"),
                vDestUnit:=pDR("Origin UOM"))
            If pDrConversion Is Nothing Then
                WebRequest.Instance.Business.LibraryStockMeasurements.AddConversion(
                    vBaseUnit:=pDR("Destination UOM"),
                    vDestUnit:=pDR("Origin UOM"),
                    vFamily:=pDR("UOM Family"),
                    vFactor:=1 / pDR("Conversion Factor"))
                pResult.Success = True
            Else

            End If
        Next pDR

        Return JsonConvert.SerializeObject(pResults)

    End Function

    Public Class ImportMeasureConversionResults
        Inherits List(Of Item)

        Public Overloads Function Add(vSpreadsheetRow As Integer) As Item

            Dim pOut As Item

            pOut = New Item
            pOut.SpreadsheetRow = vSpreadsheetRow
            MyBase.Add(pOut)

            Return pOut

        End Function

        'Public Function AddSuccess(vSpreadsheetRow As Integer) As Item

        '    Dim pOut As Item

        '    pOut = New Item
        '    pOut.SpreadsheetRow = vSpreadsheetRow
        '    pOut.Success = True
        '    MyBase.Add(pOut)

        '    Return pOut

        'End Function

        'Public Function AddError(vSpreadsheetRow As Integer, vMessage As String) As Item

        '    Dim pOut As Item

        '    pOut = New Item
        '    pOut.SpreadsheetRow = vSpreadsheetRow
        '    pOut.Message = vMessage
        '    MyBase.Add(pOut)

        '    Return pOut

        'End Function

        Public Class Item
            Public SpreadsheetRow As Integer
            Public Success As Boolean
            Public Message As String
        End Class
    End Class

    <HttpGet>
    Public Function ImportOnitsuka() As String

        Dim pJson As JsonDictionary
        Dim pWeb As WebClient
        Dim pJsonStr As String
        Dim pUploadResult As String

        pJson = New JsonDictionary()
        With pJson
            .Add("OrgID", "5f99fbf1-4251-426e-9ac5-e031b7203d5c")
            .Add("DeviceID", "ed07002b-c2df-4f25-bdf1-ad8695bf19e3")
        End With
        pJsonStr = pJson.ToString()
        pWeb = New WebClient()
        Dim pFilename As String

        pFilename = "C:\dev\for debugging\import to onitsuka BEFECC74-AD12-4671-B15D-EDC30410F107 2016_08_25\2016_08_25__16_24_58_8092\2016_08_25__16_24_58_8092 16_08_26.zip"
        Dim pDict As New NameValueCollection
        'pDict.Add("OrgID", "5f99fbf1-4251-426e-9ac5-e031b7203d5c")
        'pDict.Add("DeviceID", "ed07002b-c2df-4f25-bdf1-ad8695bf19e3")
        pDict.Add("json", pJsonStr)

        Dim pUrl As String

        'pUrl = Common.VirtualPath
        pUrl = "http://122.52.123.251"
        pUrl &= "/OfflineData/Dump"
        pUploadResult = UploadFile(pFilename, pUrl, pDict)

        Return pUploadResult

    End Function

    Public Class ImportOnitsukaJson

    End Class

    Public Function Darlington_DeactivePimsNotInExcel() As String

        Dim pSourceFilename As String
        Dim pDT As DataTable
        Dim pError As String
        Dim pRowIndex As Integer = 0
        Dim pSql As String
        Dim pIU As InsertUpdate

#If DEBUG Then
        WebRequest.Instance.OrgID = modClient.AbcOrgID
#Else
        WebRequest.Instance.OrgID = modClient.DarlingtonTrailOrgID
#End If
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        pSourceFilename = Common.ServerPath & "data\Darlington - deactivate SKUs not in here.csv"
        If IO.File.Exists(pSourceFilename) = False Then
            Return "File, " & pSourceFilename & ", does not exist."
        End If

        pDT = ExcelToTable(pSourceFilename, {"Internal ID"}, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Return pError
        End If

        Common.WebRequest.ProData.BeginTransaction()

        pSql = "CREATE TABLE #ExcelContents (InternalID varchar(MAX))"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("#ExcelContents")
        For Each pDR As DataRow In pDT.Rows
            pIU("InternalID") = ReadyString(pDR("Internal ID"))
            pSql = pIU.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            pRowIndex += 1
        Next pDR

        'pSql = "SELECT * FROM #ExcelContents"
        'Dim pDT2 = Common.WebRequest.ProData.Execute(pSql)

        pSql = "UPDATE LibraryOrganizationPIM" & vbCrLf &
            " SET IsDeactivated = 1" & vbCrLf &
            ",DeactivationDate = GETDATE()" & vbCrLf &
            " FROM LibraryOrganizationPIM" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryOrganizationPIM.ProductID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.ProductNo COLLATE DATABASE_DEFAULT NOT IN (SELECT InternalID COLLATE DATABASE_DEFAULT FROM #ExcelContents)" & vbCrLf &
            " AND LibraryOrganizationPIM.IsDeactivated = 0"
        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql)

        WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("MobizLibraryPIMProduct")

        Common.WebRequest.ProData.CommitTransaction()

        Return pSql

    End Function

    Public Function SetDefaultAccess() As String

        Dim pDbs As List(Of String)
        Dim pDbsFinal As List(Of String)
        Dim pOut As JsonDictionary
        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Const pModuleID As String = "FC600EF0-42A4-4BCA-921B-A53A93FE7864"
        Dim pCount As Integer = 0

        Try
            pOut = New JsonDictionary
            pDbs = Common.WebRequest.ProData.GetDatabases()
            pDbsFinal = New List(Of String)
            For Each pDB As String In pDbs
                If DbConnectController.IsPrivateDatabase(pDB) Then
                    pDbsFinal.Add(pDB)
                End If
            Next pDB

            For Each pDB As String In pDbsFinal
                pSql = "USE " & EscapeSqlName(pDB)
                Common.WebRequest.ProData.Execute(pSql)

                pSql = " SELECT LibrarySecurityRole.ID" & vbCrLf &
                    " ,LibrarySecurityRole.Code" & vbCrLf &
                    " ,LibrarySecurityRole.Description" & vbCrLf &
                    " FROM LibrarySecurityRole" & vbCrLf &
                    " WHERE NOT EXISTS (" & vbCrLf &
                    " 	SELECT *" & vbCrLf &
                    "     FROM LibrarySecurityRoleModules" & vbCrLf &
                    "     WHERE LibrarySecurityRoleModules.RoleID = LibrarySecurityRole.ID" & vbCrLf &
                    "     AND LibrarySecurityRoleModules.ModuleID = " & ReadyString(pModuleID) & vbCrLf &
                    " )    "
                Try
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                Catch ex As Exception
                    Continue For
                End Try
                For Each pDR As DataRow In pDT.Rows
                    pIU = New InsertUpdate("LibrarySecurityRoleModules")
                    pIU("ID") = "NEWID()"
                    pIU("RoleID") = ReadyString(pDR("ID"))
                    pIU("ModuleID") = ReadyString(pModuleID)
                    pIU("IsHasAccess") = 1
                    pSql = pIU.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                    pCount += 1
                Next pDR
            Next pDB

            Return pCount
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function FixSalesReturnDeviceIdFowstId() As String

        Dim pDbs As List(Of String)
        Dim pDbsFinal As List(Of String)
        Dim pCount As Integer = 0

        Try
            pDbs = Common.WebRequest.ProData.GetDatabases()
            pDbsFinal = New List(Of String)
            For Each pDB As String In pDbs
                If DbConnectController.IsPrivateDatabase(pDB) Then
                    pDbsFinal.Add(pDB)
                End If
            Next pDB

            For Each pDB As String In pDbsFinal
                Dim pSql = "USE " & EscapeSqlName(pDB)
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
                pCount += WebRequest.Instance.Business.TransMemoSalesReturn.FixSalesReturnDeviceIdFowstId()
            Next pDB

            Return pCount
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function GrantAllToSysAdmin() As String

        Dim pCount As Integer
        Dim pSecurityAccess As SecurityAccess
        Dim pError As String
        Dim pDatabase As String
        Dim pOrgID As String

        Dim pDtDbs = WebRequest.Instance.Business.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = pDR("Database")
                pOrgID = CStr2(pDR("ID"))

                WebRequest.Instance.OrgID = pOrgID
                If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                    Continue For
                End If

                pSecurityAccess = New SecurityAccess()
                pCount += pSecurityAccess.GrantAllToSysAdmin()
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf
                AppendToLog(pError, , "TestController_GrantAllToSysAdmin")
            End Try
        Next pDR

        Return pCount

    End Function

    Public Function EncryptUsingPublicKeyFromJava() As String

        Dim pParams As RSAParameters
        Dim pBase64Modulus As String = "DjTgGLSMXsiIH5h1Lv5COMMOuABwt4i81R9fTj4cisZaI61r+22Qxz+zjQKK3Q3SFW4K+H5l56qMlvs2wy5jlz0milDuJwywGtvZk3/D6pcb1YwcEoTWVwRv/jIkoVD1uw23OJXWRV4VGAgLvKBJOH4IpGXmlrHh0XO0M6313tr/" ' public key from java
        'Dim pBase64Modulus As String = "BgIAAACkAABSU0ExAAQAAAEAAQDhU/AVwKhbs/6A1mqRV9y55M01ryPizMMQ7L44YAS5uJVi9/jZHmxq/XYmfcclikGCKJ2DCJMcZxuAhnsmjwPvke44U/ai91HHfeFVVfm5Z3m759eiyjvz7Q03XXqf4K/qLmndsSE0CYdp4SKujKLkO3H1LF1CMKbz+RZCmvFtpQ==" ' public key from java
        Dim pBytes As Byte()
        Dim pBase64Encrypted As String
        Dim pPlainText As String = "123abc"

        pBytes = Convert.FromBase64String(pBase64Modulus)
        pParams = New RSAParameters
        pParams.Modulus = pBytes
        pParams.Exponent = New Byte() {1, 0, 1}
        pBytes = New System.Text.UTF8Encoding().GetBytes(pPlainText)

        Using pRsa As RSACryptoServiceProvider = New RSACryptoServiceProvider(1024)
            pRsa.ImportParameters(pParams)
            pBytes = pRsa.Encrypt(pBytes, False)
        End Using
        pBase64Encrypted = Convert.ToBase64String(pBytes)

    End Function

    Public Function GetProductSales() As ActionResult

        Dim pSql As String
        Dim pPartialFowstName As String

        pSql = " -- product sales per branch" & vbCrLf &
            " SELECT MobizLibraryOrganizationFOWST.Name AS Fowst" & vbCrLf &
            " ,MobizLibraryPIMProduct.Brand" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS Qty" & vbCrLf &
            " ,SUM(TransInvoiceItems.SubTotal) AS Amount" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND MobizLibraryOrganizationFOWST.Name LIKE " & ReadyStringWild(pPartialFowstName) & vbCrLf &
            " AND (" & vbCrLf &
            " 	ISNULL(MobizLibraryPIMProduct.Brand, '') + ISNULL(MobizLibraryPIMProduct.ProductName, '') LIKE '%gar%'" & vbCrLf &
            "     OR ISNULL(MobizLibraryPIMProduct.Brand, '') + ISNULL(MobizLibraryPIMProduct.ProductName, '') LIKE '%ace%'" & vbCrLf &
            " )" & vbCrLf &
            " GROUP BY TransInvoiceItems.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " ,MobizLibraryPIMProduct.Brand" & vbCrLf &
            " HAVING SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) > 0" & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.ProductName"
        WebRequest.Instance.OrgID = modClient.DucOrgID
        WebRequest.Instance.HandleOrganization(False)

    End Function

    Public Function AddToInventory() As String

        'Dim pOrgID As String = "9da344f3-dadb-4675-b723-15f0c591da3c" ' nike
        'Dim pInvoiceIDs() As String = {"7fcaef85-a5e6-4f55-9d3e-3bc36d8b546c", "227b7554-2414-4bef-84b6-16cfa661c52b"} ' nike abreeza

        Dim pOrgID As String = modClient.DemoCorpOrgID
        Dim pInvoiceIDs() As String = {
            "4b0d97e2-bebf-4eff-9819-cb690f4fa833"}
        Dim pError As String
        Dim pFilename As String

        If Now.Today <> #12/17/2022# Then
            Return "not allowed to execute today"
        End If

        pFilename = IO.Path.Combine(Common.ServerPath, "settings", "AddToInventory vbY3KQ2QTDEP!.txt")
        If IO.File.Exists(pFilename) Then
            Return "File, " & pFilename & ", already exist. Do not call this function again to prevent duplicate data. vbMDNMGXDHYA!"
        End If
        WebRequest.Instance.Business.EnableExtraLogs = True
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "vbW52C7M4FXU! " & pError
        End If
        CreateFolder(pFilename)
        FileText(pFilename) = DateTimeStamp()
        For Each pInvoiceID As String In pInvoiceIDs
            Dim pResult = WebRequest.Instance.Business.TransInvoice.AddToInventoryLedger(pInvoiceID, pOrgID)
            Dim pTextLine = vbCrLf & "Invoice " & pInvoiceID & " result = " & pResult
            AppendToFile(pFilename, pTextLine)
        Next pInvoiceID

        Return "done. vbGKDLAED6H5! " & DateTimeStamp()

    End Function

    'Public Function InvoiceNotInInventoryToInventory() As ActionResult

    '    'Dim pOrgID As String = "9da344f3-dadb-4675-b723-15f0c591da3c" ' nike
    '    'Dim pInvoiceIDs() As String = {"7fcaef85-a5e6-4f55-9d3e-3bc36d8b546c", "227b7554-2414-4bef-84b6-16cfa661c52b"} ' nike abreeza
    '    Try
    '        Dim pOrgID As String = Common.WebRequest.QueryString("OrgID")
    '        If String.IsNullOrWhiteSpace(pOrgID) Then
    '            Return Content("OrgID is required. ref: vbSV88QCSEM2!")
    '        End If

    '        Dim pError As String

    '        WebRequest.Instance.Business.EnableExtraLogs = True
    '        WebRequest.Instance.OrgID = pOrgID
    '        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
    '            Return Content("vbRPWTWARA62! " & pError)
    '        End If

    '        Dim pDT = WebRequest.Instance.Business.TransInvoice.InvoiceNotInInventory()
    '        Dim pFilename = "InvoiceNotInInventoryToInventory_" & DateTimeStamp() & ".xlsx"
    '        Dim pFullFilename = Common.ServerPath & "temp\Downloads\" & pFilename
    '        CreateFolder(pFullFilename)
    '        TableToExcelFile(pDT, "", "", pFullFilename)

    '        Return File(pFullFilename, "application/xlsx", pFilename)
    '    Catch ex As Exception
    '        Dim pStr = "Business.TransInventoryLedger.SaveTransPimSalesCaching() Error" & vbCrLf &
    '            ex.Message & vbCrLf &
    '            GetStackTrace(, ex)
    '        If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
    '            pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
    '        End If
    '        pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
    '        Return Content(pStr)
    '    End Try

    'End Function

    Public Function AddInvoiceNotInInventoryToInventory() As String
        ' http://localhost:8080/test/AddInvoiceNotInInventoryToInventory?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2024-10-01&invoicedateto=2024-10-10
        'Dim pOrgID As String = "9da344f3-dadb-4675-b723-15f0c591da3c" ' nike
        'Dim pInvoiceIDs() As String = {"7fcaef85-a5e6-4f55-9d3e-3bc36d8b546c", "227b7554-2414-4bef-84b6-16cfa661c52b"} ' nike abreeza

        Try
            Dim pOrgID As String = Common.WebRequest.QueryString("OrgID")
            If String.IsNullOrWhiteSpace(pOrgID) Then
                Return ("OrgID is required. ref: vbSV88QCSEM2!")
            End If

            Dim pInvoiceDateFromProper As Date? = ParseDate(WebRequest.Instance.QueryString("invoicedatefrom"))
            Dim pInvoiceDateToProper As Date? = ParseDate(WebRequest.Instance.QueryString("invoicedateto"))
            If pInvoiceDateFromProper Is Nothing Then
                Return "Error: invoicedatefrom must be a valid date"
            End If
            If pInvoiceDateToProper Is Nothing Then
                Return "Error: invoicedateto must be a valid date"
            End If

            If mSemaphoreCache.Wait(2000) = False Then
                Dim pFilename2 = IO.Path.Combine(Common.ServerPath, "temp\logs\AddInvoiceNotInInventory\Mobiz.log")
                If IO.File.Exists(pFilename2) Then
                    Dim pStr = "Another cache process is running." & vbCrLf & GetFileTextEnd(pFilename2, 10000)
                    pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                    Return pStr
                End If
                Return "Another cache process is running. " & mRunningProcess
            End If
            mRunningProcess = "AddInvoiceNotInInventoryToInventory started " & DateTimeStamp()

            Dim pError As String
            WebRequest.Instance.Business.EnableExtraLogs = True
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return ("vbRPWTWARA62! " & pError)
            End If

            AppendToLog("Start of AddInvoiceNotInInventoryToInventory", "temp\logs\AddInvoiceNotInInventory")

            TimedThread(
                Sub()
                    Do
                        Try
                            Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            Dim pDR = New Business(pProData).LibraryOrganization.ByID(pOrgID)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
                            Dim pBusiness = New Business(pProData)
                            pBusiness.OrgID = pOrgID

                            Do
                                Do
                                    Dim pUnflushed = pBusiness.TransInvoice.UnflushedData()
                                    If pUnflushed.Count >= 5 _
                                        OrElse pUnflushed.MaxFileSize >= 10000 Then

                                        AppendToLog("Cant process coz:" & vbCrLf &
                                            "Unflushed count = " & pUnflushed.Count & vbCrLf &
                                            "Unflushed max file size = " & pUnflushed.MaxFileSize.ToString("n0"),
                                            "temp\logs\AddInvoiceNotInInventory")
                                        Thread.Sleep(5000)
                                    Else
                                        Exit Do
                                    End If
                                Loop

                                AppendToLog("Start of loop", "temp\logs\AddInvoiceNotInInventory")
                                pBusiness.ProData.CommandTimeout = 5 * 60 * 1000
                                Dim pDT = pBusiness.TransInvoice.InvoiceNotInInventory(pInvoiceDateFromProper.Value, pInvoiceDateToProper.Value)
                                If pDT.Rows.Count = 0 Then
                                    AppendToLog("done vbN4MJP6BDMA", "temp\logs\AddInvoiceNotInInventory")
                                    mSemaphoreCache.Release()
                                    Return
                                End If
                                Dim pFullFilename = IO.Path.Combine(Common.ServerPath, "temp\logs\AddInvoiceNotInInventory", DateTimeStamp() & ".xlsx")
                                TableToExcelFile(pDT, "", "", pFullFilename)

                                Dim pDtUnique = pDT.DefaultView.ToTable(True, "ID", "InvoiceDate", "InvoiceNum", "Fowst")
                                For Each pDR In pDtUnique.Rows
                                    Dim pResult = pBusiness.TransInvoice.AddToInventoryLedger(CStr2(pDR("ID")), pOrgID)
                                    Dim pTextLine = vbCrLf & "Invoice " & CStr2(pDR("ID")) & vbCrLf &
                                        " Invoice Date: " & pDR("InvoiceDate") & vbCrLf &
                                        " Invoice Number: " & pDR("InvoiceNum") & vbCrLf &
                                        " Fowst: " & pDR("Fowst")
                                    AppendToLog(pTextLine, "temp\logs\AddInvoiceNotInInventory")

                                    Do
                                        Dim pUnflushed = pBusiness.TransInvoice.UnflushedData()
                                        If pUnflushed.Count >= 5 _
                                            OrElse pUnflushed.MaxFileSize >= 10000 Then

                                            AppendToLog("Cant process coz:" & vbCrLf &
                                                "Unflushed count = " & pUnflushed.Count & vbCrLf &
                                                "Unflushed max file size = " & pUnflushed.MaxFileSize.ToString("n0"),
                                                "temp\logs\AddInvoiceNotInInventory")
                                            Thread.Sleep(5000)
                                        Else
                                            Exit Do
                                        End If
                                    Loop
                                Next pDR
                            Loop
                            mSemaphoreCache.Release()
                            Return
                        Catch ex As Exception
                            AppendToLog(ex.Message & GetStackTrace(, ex), "temp\logs\AddInvoiceNotInInventory")
                            If ex.Message.Contains("was deadlocked on lock resources with another process and has been chosen as the deadlock victim") _
                                OrElse ex.Message.Contains("Thread was being aborted.") _
                                OrElse ex.Message.Contains("A connection was successfully established with the server") Then

                                Continue Do
                            End If
                            mSemaphoreCache.Release()
                        End Try
                    Loop
                End Sub
            )

            Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp\logs\AddInvoiceNotInInventory\Mobiz.log")
            If IO.File.Exists(pFilename) Then
                Dim pStr = "Process is now running." & vbCrLf & GetFileTextEnd(pFilename, 10000)
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End If
            Return "run again for logs"

        Catch ex As Exception
            Dim pStr = "AddInvoiceNotInInventoryToInventory() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            mSemaphoreCache.Release()
            Return pStr
        End Try

    End Function

    '#If DEBUG Then
    Private Shared ReadOnly mDataCleanerTest As New Object

    Public Function DataCleanerTest2()

        Dim pConnectionInfo As ConnectionInfo
        Dim pProData As ProData
        Dim pBusiness As Business
        Dim pServer As String
        Dim pPassword As String

        DataCleaner.AddLog("before SyncLock mDataCleanerTest vbTYEC9PB5XL!")
        SyncLock mDataCleanerTest
            DataCleaner.AddLog("after SyncLock mDataCleanerTest vb4Y3M36ZRA2!")
            Try
                pConnectionInfo = Common.ConnectionInfo
                pServer = pConnectionInfo.Server
                pPassword = pConnectionInfo.Password
                If pServer.Trim().ToLower().EndsWith("\sql2") = False Then
                    pServer &= "\sql2"
                    pPassword &= "2"
                End If
                pProData = ProData.CreateFromConnectionInfo(
                    vServer:="(local)\flushing",
                    vDatabase:="MobizAbc",
                    vUsername:="sa",
                    vPassword:="abc123",
                    vIntegratedSecurity:=False
                )
                If String.IsNullOrWhiteSpace(pProData.ConnectError) = False Then
                    DataCleaner.AddLog("vbB635ZADHTF! pProData.ConnectError: " & pProData.ConnectError)
                    Return "pProData.ConnectError: " & pProData.ConnectError & DateTimeStamp() & "; ref: vbSNEZDK2MH2!"
                End If

                TimedThread(
                    Sub()
                        DataCleaner.AddLog("after ProData.CreateFromConnectionInfo vb97U4RFH82P!")
                        pBusiness = New Business(pProData)
                        pBusiness.DataCleaner.Execute()
                    End Sub
                )
                Return "executing. " & DateTimeStamp() & "; ref: vbP7PX2R8BNU!"
            Catch ex As Exception
                Dim pStr = "DataCleanerTest Error" & vbCrLf &
                    ex.Message & vbCrLf &
                    GetStackTrace(, ex)
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    pStr = "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf & pStr
                End If
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End Try
        End SyncLock

    End Function

    Public Function DataCleanerTestResult() As String

        Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\logs\DataCleaner vbPGBRQHPUYY.log"
        If System.IO.File.Exists(pFilename) = False Then
            Return "File, " & pFilename & ", does not exist. " & DateTimeStamp() & " vbU4AJ4NXDLK!"
        End If

        Dim pFileText = FileText(pFilename)
        pFileText = DateTimeStamp() & " vbT37BBDAQWH!" & vbCrLf & TruncateLeft(pFileText, 5000)
        pFileText = pFileText.Replace(vbCrLf, "<br />" & vbCrLf)

        Return pFileText

    End Function

    '#End If

#Region " DTR Export "
    Public Function ExportDtr() As ActionResult

        Dim pDateFrom As Date
        Dim pDateTo As Date
        Dim pDT As DataTable
        Dim pOrgID As String
        Dim pOut As String
        Dim pFilename As String
        Dim pBytes As Byte()

        pOrgID = "cfe39df6-8809-4973-9515-810df75aed12"
        pDateFrom = #1/1/2017#
        pDateTo = #1/31/2017#

        pFilename = DateTimeStamp(False) & ".duc"
        Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(pFilename, True))
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True) = False Then Return Content(WebRequest.InvalidOrganization)
        pOut = MakeDtrHeader(pDateFrom, pDateTo)
        pDT = GetDtr(pDateFrom, pDateTo)
        For Each pDR As DataRow In pDT.Rows
            pOut &= DtrMakeLine(pDR)
        Next pDR
        pOut &= DtrEncryptString("90") & vbCrLf

        pFilename = Common.ServerPath & "temp\deletables\" & pFilename
        ReDim pBytes(pOut.Length - 1)
        For p As Integer = 0 To pOut.Length - 1
            pBytes(p) = Asc(pOut(p))
        Next p
        SetFileBytes(pFilename, pBytes)

        Response.ContentType = "application/duc"
        Response.TransmitFile(pFilename)

        'Return Content(pOut, "txt", Encoding.GetEncoding("ISO-8859-1"))

    End Function

    Private Function MakeDtrHeader(vDateFrom As Date, vDateTo As Date) As String

        Dim pOut As String
        Dim pBranchID As Integer
        Dim pPeriod As String

        ' test input
        pBranchID = 1
        pPeriod = "MyTestPeriod"

        pOut = "10 " & Left(pBranchID & Space(3), 3) &
            Left(pPeriod & Space(5), 5) & Space(1) &
            Format(vDateFrom, "MMddyyyy") & " " &
            Format(vDateTo, "MMddyyyy")
        pOut = DtrEncryptString(pOut) & vbCrLf

        Return pOut

    End Function

    Private Function DtrMakeLine(vDR As DataRow) As String

        Dim pOut As String
        Dim pDateLog As String = vDR("LogInTime")
        Dim pTotalTime As String = ""
        Dim pEmployeeTimeIn As String = vDR("LogInTime")
        Dim pEmployeeTimeOut As String
        Dim pRemarks As String = "from MDTR"
        Dim pIsNightShift As Integer = 0
        Dim pPercentIncrease As String = ""
        Dim pSalaryMade As String = "0"
        Dim pApprovedBy As String = "0"
        Dim pSalaryLevelID As String = CInt2(vDR("SalaryLevelID"))
        Dim pDtrID As Integer = 0

        If IsDate(vDR("LogOutTime")) Then
            pEmployeeTimeOut = vDR("LogOutTime")
        Else
            pEmployeeTimeOut = vDR("LogInTime")
        End If

        pOut = "20 " & Left(pDtrID & Space(10), 10) &
            Left(CInt2(vDR("IntID")) & Space(5), 5) &
            Left(pDateLog & Space(25), 25) &
            Left(vDR("LogInTime") & Space(25), 25) &
            Left(pEmployeeTimeOut & Space(25), 25) &
            Left(pTotalTime & Space(25), 25) &
            Left(pEmployeeTimeIn & Space(25), 25) &
            Left(pEmployeeTimeOut & Space(25), 25) &
            Left(pRemarks & Space(30), 30) &
            Left(pIsNightShift & Space(2), 2) &
            Left(pPercentIncrease & Space(3), 3) &
            Left(pSalaryMade & Space(10), 10) &
            Left(pApprovedBy & Space(60), 60) &
            Left(pSalaryLevelID & Space(5), 5)
        pOut = DtrEncryptString(pOut) & vbCrLf

        Return pOut

    End Function

    Private Function DtrEncryptString(vStr As String) As String

        Dim ctr As Long
        Dim strTemp As String
        Dim strComparator As String
        Dim strNewPass As String

        'Return vStr

        ctr = 1
        strComparator = "HubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporated" &
            "HubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporated" &
            "HubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporatedHubportInteractiveIncorporated"

        strNewPass = vbNullString

        Do While ctr <= Len(vStr)
            strTemp = Mid(vStr, ctr, 1)
            strTemp = Asc(strTemp) + Asc(Mid(strComparator, ctr, 1))
            strNewPass = strNewPass & Chr(strTemp)
            ctr = ctr + 1
        Loop

        Return strNewPass

    End Function

    Private Function GetDtr(vDateFrom As Date, vDateTo As Date) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        vDateFrom = vDateFrom.Date
        vDateTo = vDateTo.Date

        pSql = "  -- GetDtr" & vbCrLf &
            "  SELECT TransDTR.PeopleID" & vbCrLf &
            "  ,MobizLibraryPeople.FirstName" & vbCrLf &
            "  ,MobizLibraryPeople.MiddleName" & vbCrLf &
            "  ,MobizLibraryPeople.SurName" & vbCrLf &
            "  ,MobizLibraryPeople.DUCNumericID" & vbCrLf &
            "  ,LibraryOrganizationEmployee.ThirdPartySystemID" & vbCrLf &
            "  ,TransDTR.LogInTime" & vbCrLf &
            "  ,TransDTR.LogOutTime" & vbCrLf &
            "  ,TransDTR.LogInTime AS EmployeeTimeIn" & vbCrLf &
            "  ,TransDTR.LogOutTime AS EmployeeTimeOut" & vbCrLf &
            "  ,EmployeeProfiles.SalaryLevelID" & vbCrLf &
            "  ,EmployeeProfiles.ID AS IntID" & vbCrLf &
            "  FROM TransDTR" & vbCrLf &
            "  JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = TransDTR.PeopleID" & vbCrLf &
            "  JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransDTR.PeopleID" & vbCrLf &
            "  JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles ON EmployeeProfiles.EmployeeCompanyID = LibraryOrganizationEmployee.ThirdPartySystemID" & vbCrLf &
            " WHERE CONVERT(DATE, TransDTR.LogInTime) >= " & ReadyString(vDateFrom) & vbCrLf &
            " AND CONVERT(DATE, TransDTR.LogInTime) <= " & ReadyString(vDateTo)
        pOut = Common.WebRequest.ProData.Execute(pSql)

        Return pOut

    End Function
#End Region ' END #Region " DTR Export "

    Public Function FillFowstNumber() As String

        Dim pRecordsAffected As Integer

        pRecordsAffected = WebRequest.Instance.Business.LibraryOrganizationFOWST.FillFowstNumber()

        Return pRecordsAffected

    End Function

    Public Function LibraryRepeatingTransactions_SearchAndCreateDefinition() As String

        Dim pOrgID As String = modClient.DemoCorpOrgID
        Dim pError As String
        Dim pSuccess As Integer

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "vb2V49SFJE4D! " & pError
        End If
        pSuccess = WebRequest.Instance.Business.LibraryRepeatingTransactions.Handle1Org(WebRequest.Instance.OrgID)

        Return pSuccess & " _ " & DateTimeStamp()

    End Function

    Public Function SaveTransPimSalesCaching() As String

        Dim pOrgID As String = Common.WebRequest.QueryString("OrgID")
        If String.IsNullOrWhiteSpace(pOrgID) Then
            Return "OrgID is required. ref: vbSV88QCSEM2!"
        End If
        Dim pError As String
        Dim pSuccess As Integer

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "vbUDF5BLX3DC! " & pError
        End If
        Try
            WebRequest.Instance.Business.TransInventoryLedger.SaveTransPimSalesCaching()
        Catch ex As Exception
            Dim pStr = "Business.TransInventoryLedger.SaveTransPimSalesCaching() Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        End Try

        Return pSuccess & " _ " & DateTimeStamp()

    End Function

    Public Function LibraryRepeatingTransactions_Handle1Org() As String

        Dim pSuccess As Integer

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSuccess = WebRequest.Instance.Business.LibraryRepeatingTransactions.Handle1Org(WebRequest.Instance.OrgID)

        Return pSuccess & " _ " & DateTimeStamp()

    End Function

    Public Function LibraryRepeatingTransactions_HandleAllOrg() As String

        Dim pSuccess As Integer

        pSuccess = WebRequest.Instance.Business.LibraryRepeatingTransactions.HandleAllOrg()

        Return pSuccess & " _ " & DateTimeStamp()

    End Function

    Public Function CopyFromHrToMobiz() As String

        Dim pError As String

        'If #5/22/2023# < Now.Date Then
        '    Return "date expired vbQ5DFHAWCRQ"
        'End If

        Dim pEmployeeProfileID = Common.WebRequest.QueryString("EmployeeProfileID")
        If String.IsNullOrWhiteSpace(pEmployeeProfileID) Then
            Return "Error: EmployeeProfileID must not be blank. ref: vbCWS4V9YJRU!"
        End If
        If IsNumeric(pEmployeeProfileID) = False Then
            Return "Error: EmployeeProfileID must be an integer. ref: vbDYQ9X8XAXQ!"
        End If
        pEmployeeProfileID = CInt2(pEmployeeProfileID)

#If DEBUG Then
        WebRequest.Instance.OrgID = modClient.DemoCorpOrgID
#Else
        WebRequest.Instance.OrgID = modClient.DucOrgID
#End If
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "vbN4BRVA4N88! " & pError
        End If
        Dim p = New LibraryPeople.DucToMobizExporter(WebRequest.Instance.Business, WebRequest.Instance.OrgID)
        Dim pOut = p.CopyFromHrToMobiz(pEmployeeProfileID)

        Return "Added: " & pOut.AddedCount & "<br />" & vbCrLf &
            "Updated: " & pOut.UpdatedCount

    End Function

    Public Function AddBanksFromExcel() As String

        Dim pFilename As String
        Dim pDT As DataTable
        Dim pCount As Integer
        Dim pMessage As String
        Dim pRankedNames As New List(Of LibraryOrganization.RankedName)
        Dim pRankedName As LibraryOrganization.RankedName

        pFilename = Common.ServerPath & "data\Banks.xlsx"
        pDT = ExcelToTable(pFilename, {"Organization Name", "Bank Rank"}, pMessage)
        If String.IsNullOrWhiteSpace(pMessage) = False Then
            Return pMessage
        End If
        For Each pDR As DataRow In pDT.Rows
            pRankedName = New LibraryOrganization.RankedName
            pRankedName.Name = Truncate(CStr2(pDR("Organization Name")).Trim(), 100)
            ' ReSharper disable once AccessToModifiedClosure https://www.jetbrains.com/help/resharper/2017.2/AccessToModifiedClosure.html
            Dim pDuplicates =
                From pDuplicate In pRankedNames
                Where pDuplicate.Name.ToLower() = pRankedName.Name.ToLower()
            If pDuplicates.Any() = False Then
                pRankedName.Rank = CInt2(pDR("Bank Rank"))
                pRankedNames.Add(pRankedName)
            Else
                Debug.WriteLine("Bank, " & pRankedName.Name & ", was already added. ref: vb3ZWVGUDM64!")
            End If
        Next pDR

        pCount = WebRequest.Instance.Business.LibraryOrganization.AddBanksIfNeeded(pRankedNames)

        Return pCount

    End Function

    Public Function CopyMobizLibraryOrganization_NetSuiteID_To_LibraryOrganizationCustomer_ThirdPartyID()

#If DEBUG Then
        Dim pOrgID = modClient.AbcOrgID
#Else
        Dim pOrgID = modClient.DarlingtonTrailOrgID
#End If


        Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(pOrgID)
        If pDR Is Nothing Then
            Return "Organization, " & DarlingtonTrailOrgID & ", does not exist."
        End If
        If CStr2(pDR("Database")) = "" Then
            Return "Organization, " & DarlingtonTrailOrgID & ", does not have a database."
        End If

        WebRequest.Instance.ProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))

        Dim pSql = " UPDATE LibraryOrganizationCustomer" & vbCrLf &
            " SET LibraryOrganizationCustomer.ThirdPartyID = MobizLibraryOrganization.NetSuiteID" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " WHERE MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID"
        WebRequest.Instance.ProData.ExecuteSQL(pSql)

        'Select MobizLibraryOrganization.CompanyName
        ',MobizLibraryOrganization.NetSuiteID
        ',LibraryOrganizationCustomer.ThirdPartyID
        'FROM LibraryOrganizationCustomer
        'JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID
        'WHERE NULLIF(MobizLibraryOrganization.NetSuiteID, '') IS NOT NULL
        'OR NULLIF(LibraryOrganizationCustomer.ThirdPartyID, '') IS NOT NULL
        'ORDER BY MobizLibraryOrganization.CompanyName

    End Function

    Public Function UpdateBalanceFromDuc() As String

#If DEBUG Then
        'WebRequest.Instance.OrgID = modClient.AbcOrgID
        WebRequest.Instance.OrgID = modClient.DucOrgID
#Else
        WebRequest.Instance.OrgID = modClient.DucOrgID
#End If
        WebRequest.Instance.HandleOrganization(True)
        WebRequest.Instance.Business.LibraryCustomers.UpdateBalanceFromDuc()

    End Function

    Public Function DucThirdPartyBegBalanceChecksum() As String

#If DEBUG Then
        'WebRequest.Instance.OrgID = modClient.AbcOrgID
        WebRequest.Instance.OrgID = modClient.DucOrgID
#Else
        WebRequest.Instance.OrgID = modClient.DucOrgID
#End If
        WebRequest.Instance.HandleOrganization(True)
        Dim pOut = WebRequest.Instance.Business.LibraryCustomers.DucThirdPartyBegBalanceChecksum()

        Return pOut

    End Function

    Public Function ContinueInvoiceNumberOfOldDevice() As String

        ' sta ana
        Const pOldDeviceID As String = "AFA90B95-6132-4FA7-AA76-4D8100AF64BB"
        Const pNewDeviceID As String = "AF39FF02-B4EA-432A-894C-54ABDC1DDD3D"
        Const pFowstID As String = "A38CA176-C692-4251-B485-991A7AEEF22B"

        ' dev
        'Const pOldDeviceID As String = "FEC708E8-B2F1-49FE-8E74-8061D048EF50"
        'Const pNewDeviceID As String = "41FA817D-A876-4E65-BE6A-E201DB0DC59A"
        'Const pFowstID As String = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"

        Dim pSql As String

#If DEBUG Then
        WebRequest.Instance.OrgID = modClient.AbcOrgID
#Else
        WebRequest.Instance.OrgID = modClient.DucOrgID
#End If

        WebRequest.Instance.HandleOrganization(True)

        WebRequest.Instance.ProData.BeginTransaction()
        pSql = "SELECT MAX(InvoiceNumber)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WITH (TABLOCKX, HOLDLOCK) " & vbCrLf &
            " WHERE DeviceID = " & ReadyString(pOldDeviceID) & vbCrLf &
            " AND FowstID = " & ReadyString(pFowstID)
        Dim pLastOldInvoiceNum As Integer = CInt2(WebRequest.Instance.ProData.ExecuteScalar(pSql))

        pSql = "SELECT ID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WITH (TABLOCKX, HOLDLOCK) " & vbCrLf &
            " WHERE DeviceID = " & ReadyString(pNewDeviceID) & vbCrLf &
            " AND FowstID = " & ReadyString(pFowstID) & vbCrLf &
            " AND InvoiceNumber > 0" & vbCrLf &
            " ORDER BY InvoiceNumber"
        Dim pDtNew As DataTable = WebRequest.Instance.ProData.Execute(pSql)
        For Each pDrNew As DataRow In pDtNew.Rows
            pLastOldInvoiceNum += 1
            Dim pIU As New InsertUpdate("TransInvoice")
            pIU("InvoiceNumber") = pLastOldInvoiceNum
            pIU("DeviceID") = ReadyString(pOldDeviceID)
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrNew("ID")))
            WebRequest.Instance.ProData.ExecuteSQL(pSql)
        Next pDrNew
        WebRequest.Instance.ProData.CommitTransaction()

        Return pDtNew.Rows.Count & " invoices have been converted. " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function TestSqlite() As String

        Dim pFilename = IO.Path.Combine(ProWeb.Common.ServerPath, "temp\tests\Sqlite vbCHKPJN8M67.db")
        UtilityRedundant.CreateFolder(pFilename)
        Dim pResult = ProSqlite.Test(pFilename)
        Return pResult

    End Function

    Public Function CreateOrderFromProject() As String
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        WebRequest.Instance.HandleOrganization(True)
        WebRequest.Instance.Business.TransInvoice.CreateOrderFromProject(modClient.AbcOrgID, "{B6A1238B-D25A-4331-8E70-3D3E409346C3}")
    End Function

    Shared ReadOnly mSemaphoreDeleteAllFilesExceptOne As New SemaphoreSlim(1)
    Public Function DeleteAllFilesExceptOne() As String

        Try
            If mSemaphoreDeleteAllFilesExceptOne.Wait(2000) = False Then
                Return "vb5BW4PVCXX3! Another process is active."
            End If
            TimedThread(
                Sub()
                    Try
#If DEBUG Then
                        Thread.Sleep(20000)
#End If
                        Dim pFileTitle = DateTimeStamp()
                        Dim pFolder As String
                        pFolder = Common.ServerPath & "temp\data import logs\"
                        Dim pOut = ProPack.Utility.DeleteAllFilesExceptOne(pFolder,
                            Sub(vParams As DeleteAllFilesExceptOneResult)
                                Dim pLog As String
                                If vParams.IsDone Then
                                    pLog = "END Folder: " & vParams.Folder & vbCrLf
                                    pLog &= "Total delete files: " & Format(vParams.DeletedFileCount, cGlobal.FormatInteger) & vbCrLf
                                    pLog &= "Total delete file size: " & Format(vParams.DeletedFileSize, cGlobal.FormatInteger) & vbCrLf
                                    pLog &= "Total error delete files: " & Format(vParams.ErrorDeletedFileCount, cGlobal.FormatInteger) & vbCrLf
                                    pLog &= "Total error delete file size: " & Format(vParams.ErrorDeletedFileSize, cGlobal.FormatInteger) & vbCrLf
                                    pLog &= "Duration: " & FormatDurationBySeconds((vParams.EndTime - vParams.StartTime).TotalSeconds) & vbCrLf
                                Else
                                    pLog = "BEGIN Folder: " & vParams.Folder & vbCrLf
                                End If
                                AppendToLog(pLog, "temp\logs\DeleteAllFilesExceptOne", pFileTitle)
                            End Sub)
                        'AppendToLogDeleteAllFilesExceptOne(pOut, pFileTitle)
                        Dim pFilename = Common.ServerPath & "temp\logs\DeleteAllFilesExceptOne\" & pFileTitle & ".log"
                        Dim pText = GetFileText(pFilename, 1000000)
                        pText = pText.Replace(vbCrLf, "<br />" & vbCrLf)
                    Catch ex As Exception
                        AppendToDebug("vb4MDJBFK7ZN! DeleteAllFilesExceptOne", True, ex, True)
                    Finally
                        mSemaphoreDeleteAllFilesExceptOne.Release()
                    End Try
                End Sub)

            Return DateTimeStamp()
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    'Private Sub AppendToLogDeleteAllFilesExceptOne(vDeleteAllFilesExceptOneResult As DeleteAllFilesExceptOneResult, vLogTitle As String)

    '    Dim pText = "Folder: " & vDeleteAllFilesExceptOneResult.Folder & vbCrLf
    '    pText &= "Total delete files: " & Format(vDeleteAllFilesExceptOneResult.DeletedFileCount, cGlobal.FormatInteger) & vbCrLf
    '    pText &= "Total delete file size: " & Format(vDeleteAllFilesExceptOneResult.DeletedFileSize, cGlobal.FormatInteger) & vbCrLf
    '    pText &= "Duration: " & FormatDurationBySeconds((vDeleteAllFilesExceptOneResult.EndTime - vDeleteAllFilesExceptOneResult.StartTime).TotalSeconds) & vbCrLf
    '    AppendToLog(ptext, "temp\logs\DeleteAllFilesExceptOne", vLogTitle)

    '    For Each pSubFolder In vDeleteAllFilesExceptOneResult.Subfolders
    '        AppendToLogDeleteAllFilesExceptOne(pSubFolder, vLogTitle)
    '    Next

    'End Sub

    Public Function UpdateSalesRepText() As Integer

        Dim pDtDbs As DataTable
        Dim pDatabase As String
        Dim pOrgID As String
        Dim pError As String
        Dim pCount As Integer

        pDtDbs = WebRequest.Instance.Business.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = pDR("Database")
                pOrgID = CStr2(pDR("ID"))
                WebRequest.Instance.Business.ProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                pCount += WebRequest.Instance.Business.TransInvoice.UpdateSalesRepText()
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf
                AppendToLog(pError, , "TestController_UpdateSalesRepText")
            End Try
        Next pDR

        Return pCount

    End Function

    Public Function AddAdmin() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.LibraryPeople.AddAdmin(modClient.AbcOrgID)

        Return "success - " & DateTimeStamp()

    End Function

    Public Function SalesOrderToPurchaseOrder() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.TransInvoice.SalesOrderToPurchaseOrder("5FDF30C7-6703-4B4D-A107-183A75FF8A70", modClient.AbcOrgID, pError)
        If String.IsNullOrWhiteSpace(pError) Then
            Return "success - " & DateTimeStamp()
        Else
            Return DateTimeStamp() & " - " & pError
        End If

    End Function

    Public Function ChangeProductArea() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        'WebRequest.Instance.Business.LibraryStock.ChangeProductArea("c81474ff-df36-4691-9dd3-b54ed80be2d5", "fe639d1f-cd83-4b3e-8b0a-586731f479cb", "Bodega 1") ' siom
        'WebRequest.Instance.Business.LibraryStock.ChangeProductArea("53dfd6c5-a817-4729-91c8-8361411e470f", "fe639d1f-cd83-4b3e-8b0a-586731f479cb", "Bodega 1") ' coke
        'WebRequest.Instance.Business.LibraryStock.ChangeProductArea("9a5a46da-f276-473b-ba1b-16769d0341d6", "fe639d1f-cd83-4b3e-8b0a-586731f479cb", "Bodega 1") ' cake
        'WebRequest.Instance.Business.LibraryStock.ChangeProductArea("1a416597-ab70-48fb-bdd1-5651ab343fe6", "fe639d1f-cd83-4b3e-8b0a-586731f479cb", "Bodega 2") ' drink
        If String.IsNullOrWhiteSpace(pError) Then
            Return "success - " & DateTimeStamp()
        Else
            Return DateTimeStamp() & " - " & pError
        End If

    End Function

    Public Function ImportProductAreaFromErp() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.DucOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.LibraryStock.ImportProductAreaFromErp()
        If String.IsNullOrWhiteSpace(pError) Then
            Return "success - " & DateTimeStamp()
        Else
            Return DateTimeStamp() & " - " & pError
        End If

    End Function

    Public Function DeleteDuplicateProductCategories() As String

        Dim pOut = WebRequest.Instance.Business.LibraryStock.DeleteDuplicateProductCategories()

        Return DateTimeStamp() & " - " & pOut

    End Function

    Public Function AddAdminAll() As String

        Dim pDtDbs As DataTable
        Dim pDatabase As String
        Dim pOrgID As String
        Dim pError As String
        Dim pCount As Integer

        pDtDbs = WebRequest.Instance.Business.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = CStr2(pDR("Database"))
                If String.IsNullOrWhiteSpace(pDatabase) Then
                    Continue For
                End If
                pOrgID = CStr2(pDR("ID"))
                WebRequest.Instance.OrgID = pOrgID
                If WebRequest.Instance.HandleOrganization(True, pError) Then
                    WebRequest.Instance.Business.LibraryPeople.AddAdmin(pOrgID)
                End If
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf
                AppendToLog(pError, , "TestController_AddAdminAll")
            End Try
        Next pDR

        Return "success - " & DateTimeStamp()

    End Function

    Public Function GetTime() As String

        Return DateTimeStamp()

    End Function

    Public Function PopulateControllerReservedNames() As String

        Dim pFilename = Common.ServerPath & "Controllers\ErrorController.vb"
        If IO.File.Exists(pFilename) = False Then
            Return pFilename & " does not exist."
        End If
        Dim pControllers = CompilePagesController.GetControllers()
        Dim pControllerText = ""
        For Each pController In pControllers
            pControllerText &= "pOut.Add(""" & pController.ToLower() & """)" & vbCrLf
        Next
        Dim pOrigFileText As String = FileText(pFilename)
        Dim pPattern = "(?s)(?<before>' ReservedNames-BEGIN-TOKEN).*(?<after>' ReservedNames-END-TOKEN)"
        Dim pFileText = Regex.Replace(pOrigFileText, pPattern, "${before}" & vbCrLf & pControllerText & vbCrLf & "${after}", RegexOptions.Multiline)

        If pOrigFileText.ToLower() = pFileText.ToLower() Then
            Return "File text remains the same. No need to update file text."
        End If

        FileText(pFilename) = pFileText

        Return "File text was successfully updated."

    End Function

    Public Function SummarizedImportDataLogs() As String

        Dim pOut = ImportDataClass.SummarizeLogs()
        pOut = pOut.Replace(vbCrLf, "<br />" & vbCrLf)

        Return pOut

    End Function

    Public Function ExtractFlushingReports() As String

        'Dim pError As String

        'WebRequest.Instance.OrgID = modClient.AbcOrgID
        'If WebRequest.Instance.HandleOrganization(True, pError) = False Then
        '    Return pError
        'End If

        Return WebRequest.Instance.Business.TransFlushingReport.ExtractLogs()

        Return DateTimeStamp()

    End Function

    Public Function SaveTransProfitability() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If
        WebRequest.Instance.Business.TransInvoice.SaveTransProfitability("149a46ca-cf6f-4bee-bfab-6a3acd35a632", WebRequest.Instance.OrgID)

        Return DateTimeStamp()

    End Function

    Public Function DeduplicateProductNumbers() As String

        Dim pError As String

        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If
        WebRequest.Instance.Business.LibraryStock.DeduplicateProductNumbers()

        Return DateTimeStamp()

    End Function

    Public Function SplitExcel() As String

        Dim pFolder = Common.ServerPath & "temp\SplitExcel"

        If IO.Directory.Exists(pFolder) = False Then
            Return JsonDictionary.ToString("Error", "Folder does not exist, " & pFolder)
        End If

        Dim pFiles = IO.Directory.GetFiles(pFolder, "*.xls*")
        For Each pFile In pFiles
            Dim pDestinationFolder = pFolder & "\" & DateTimeStamp()
            SplitExcel(pFile, pDestinationFolder)
            Dim pDestinationFilename = IO.Path.Combine(pDestinationFolder, IO.Path.GetFileName(pFile))
            MoveFile(pFile, pDestinationFilename)
        Next

        Return JsonDictionary.ToString("FileCount", pFiles.Length)

    End Function

    Private Function SplitExcel(vSourceFilename As String, vDestinationFolder As String)

        Const pMaxRows As Integer = 1000

        Dim pWS = GetFirstExcelWorksheet(vSourceFilename)
        Dim pRowCount As Integer = pWS.Cells.MaxDataRow ' not count but max row index (0-based)
        pWS = Nothing
        GC.Collect()
        GC.WaitForPendingFinalizers()

        For pIndex = 0 To pRowCount Step pMaxRows
            pWS = GetFirstExcelWorksheet(vSourceFilename)
            If pIndex <> 0 Then
                pWS.Cells.DeleteRows(1, pIndex)
            End If
            pWS.Cells.DeleteRows(pMaxRows + 1, pRowCount)
            Dim pFilename = IO.Path.GetFileNameWithoutExtension(vSourceFilename) &
                " " & Format(pIndex + 1, StrDup(pRowCount.ToString().Length, "0")) &
                "-" & Format(Math.Min(pIndex + 1000, pRowCount), StrDup(pRowCount.ToString().Length, "0")) &
                IO.Path.GetExtension(vSourceFilename)
            pFilename = IO.Path.Combine(vDestinationFolder, pFilename)
            CreateFolder(pFilename)
            pWS.Workbook.Save(pFilename)
            pWS = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        Next

    End Function

    Public Function DeleteTransImportsData() As String

        Dim pLogs As String

        WebRequest.Instance.Business.LibraryOrganization.DeleteTransImportsData(pLogs)
        pLogs = HttpUtility.HtmlEncode(pLogs)
        pLogs = pLogs.Replace(vbCrLf, "<br />" & vbCrLf)

        Return pLogs

    End Function

    Public Function ExecuteBackupArchiver() As String

        Try
            Dim pExecute = CBool2(Common.WebRequest.Data("Execute"))
            Dim pResultsCrc = CStr2(Common.WebRequest.Data("ResultsCrc"))

            If pExecute _
                AndAlso String.IsNullOrWhiteSpace(pResultsCrc) Then

                Return JsonDictionary.Serialize("Error", "You must get files first before deleting old files. ref: vbV6CL9THPWZ!")
            End If

            Dim p As New BackupArchiver
#If DEBUG Then
            p.BasePath = "C:\deletables\database backups"
            p.ArchivePath = "C:\deletables\database archives"

            If IO.Directory.Exists(p.BasePath) = False Then
                p.BasePath = "C:\database backups"
            End If


#Else
            p.BasePath = "E:\database backups"
            If IO.Directory.Exists(p.BasePath) = False Then
                p.BasePath = "C:\database backups"
            End If
            If IO.Directory.Exists(p.BasePath) = False Then
                p.BasePath = "D:\database backups"
            End If
            If IO.Directory.Exists(p.BasePath) = False Then
                p.BasePath = "F:\database backups" ' wow
            End If
            p.ArchivePath = "C:\database archives"
#End If

            Dim pOut = p.Execute()

            If pExecute Then
                If pOut.ResultsCrc <> pResultsCrc Then
                    Return JsonDictionary.Serialize("Error", "Files have changed since your last ""get file"". Get files again. ref: vbN8M8DHBNRK!")
                End If
                Dim pMovedCount = BackupArchiver.MoveFiles(pOut.Files)
                pOut.Message = "Number of moved files: " & pMovedCount.ToString(cGlobal.FormatInteger)
            End If

            Return JsonConvert.SerializeObject(pOut)
        Catch ex As Exception
            AppendToDebug("ref: vbL6KNKEJQG2! " & ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function ExecuteArchiveDeleter() As String

        Try
            Dim pExecute = CBool2(Common.WebRequest.Data("Execute"))
            Dim pResultsCrc = CStr2(Common.WebRequest.Data("ResultsCrc"))

            If pExecute _
                AndAlso String.IsNullOrWhiteSpace(pResultsCrc) Then

                Return JsonDictionary.Serialize("Error", "You must get files first before deleting old archives. ref: vbT3YLAU3J5E!")
            End If

            Dim p As New BackupArchiver
            p.NewestFileCountToKeep = 5
            p.DeleteFilesInsteadOfMove = True
#If DEBUG Then
            p.BasePath = "C:\deletables\database archives"
#Else
            p.BasePath = "C:\database archives"
#End If

            Dim pOut = p.Execute()

            If pExecute Then
                If pOut.ResultsCrc <> pResultsCrc Then
                    Return JsonDictionary.Serialize("Error", "Files have changed since your last ""get file"". Get files again. ref: vbZ22JSWLZA9!")
                End If
                Dim pMovedCount = BackupArchiver.DeleteFiles(pOut.Files)
                pOut.Message = "Number of moved files: " & pMovedCount.ToString(cGlobal.FormatInteger)
            End If

            Return JsonConvert.SerializeObject(pOut)
        Catch ex As Exception
            AppendToDebug("ref: vbQ5348RZ5PF! " & ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function CachePriceTable() As String

        Dim pSqlite As New ProSqlite
        pSqlite.Open("C:\deletables\MCR TPT\data\9d4b1500-13c0-49f9-a8e5-a16c13ec899e.public.db")
        pSqlite.AttachDatabase("C:\deletables\MCR TPT\data\9d4b1500-13c0-49f9-a8e5-a16c13ec899e.private.db", "private")

        Dim pSql = " SELECT DISTINCT  " & vbCrLf &
            "   LibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            "  ,LibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            " ,LibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            "  ,LibraryPIMProductMeasurement.Quantity" & vbCrLf &
            "  ,LibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            "  ,(NOT LibraryPIMProductPriceLevel.DeactivateDiscount) * LibraryPIMProductMeasurement.Discount AS Discount" & vbCrLf &
            "  ,CAST(LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price AS Decimal) AS OriginalPrice" & vbCrLf &
            "  ,LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price - (LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price) * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPrice" & vbCrLf &
            "  ,LibraryPIMProductPriceLevel.Price - LibraryPIMProductPriceLevel.Price * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPricePerUnit" & vbCrLf &
            "  ,LibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            "  ,LibraryPIMProductMeasurement.Discount AS MeasurementDiscount" & vbCrLf &
            "  ,LibraryPIMProductPriceLevel.DeactivateDiscount" & vbCrLf &
            " ,'Piece' AS BaseUnit" & vbCrLf &
            " ,'1' AS BaseUnitQty, LibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " ,LibraryOrganizationPriceLevel.OrganizationID AS OrganizationPriceLevelOrganizationID" & vbCrLf &
            " ,LibraryPIMProductPriceLevel.OrganizationID AS ProductPriceLevelOrganizationID" & vbCrLf &
            " ,LibraryPIMProductMeasurement.ProductID" & vbCrLf &
            "  FROM LibraryPIMProductMeasurement" & vbCrLf &
            "  JOIN LibraryPIMProductPriceLevel ON LibraryPIMProductMeasurement.ProductID = LibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            "  JOIN LibraryPIMPriceLevel ON LibraryPIMProductPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID  " & vbCrLf &
            "  JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
            "  JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "  JOIN LibraryOrganizationPriceLevel ON LibraryOrganizationPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID" & vbCrLf &
            "  WHERE 1=1 -- LibraryPIMProductMeasurement.ProductID = '982fe9cb-bf96-4fe1-b496-1d85044702ee'" & vbCrLf &
            "  AND LibraryPIMProductPriceLevel.PriceLevelID <> 01000012-0001-0000-0000-000000000000" & vbCrLf &
            "  AND LibraryPIMProductPriceLevel.PriceLevelID <> 01000012-0002-0000-0000-000000000000" & vbCrLf &
            "  AND LibraryPIMProductPriceLevel.PriceLevelID <> 01000012-0003-0000-0000-000000000000" & vbCrLf &
            "  -- AND LibraryPIMProductPriceLevel.OrganizationID = '9d4b1500-13c0-49f9-a8e5-a16c13ec899e'" & vbCrLf &
            "  -- AND LibraryOrganizationPriceLevel.OrganizationID = 'cd71a0f7-08a9-4601-876e-5f13c064f873'" & vbCrLf &
            "  ORDER BY LibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " ,LibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            " ,LibraryPIMProductMeasurement.MeasurementLevel"
        pSql = "CREATE TABLE private.CachePriceTable AS " & vbCrLf & pSql
        Dim pSW As New Stopwatch
        pSW.Start()
        Dim pCount = pSqlite.ExecuteSQL(pSql)
        pSW.Stop()

        pSql = "SELECT COUNT(*) " & vbCrLf &
            " FROM private.CachePriceTable"
        pCount = pSqlite.ExecuteScalar(pSql)

        Return "Count: " & Format(pCount, cGlobal.FormatInteger) & "<br />" &
            "Duration: " & Format(pSW.ElapsedMilliseconds, cGlobal.FormatInteger) & " ms"

    End Function

    Public Function CacheInventoryMovement() As String

        Dim pError As String
        Dim ProductID As String
        Dim FowstID As String
        Dim TransactionDate As String

        WebRequest.Instance.OrgID = modClient.CincoOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        ProductID = CStr2(WebRequest.Instance.QueryString("ProductID"))
        FowstID = CStr2(WebRequest.Instance.QueryString("FowstID"))
        TransactionDate = CStr2(WebRequest.Instance.QueryString("TransactionDate"))

        WebRequest.Instance.Business.CacheInventoryMovement.LogToFile = True
        WebRequest.Instance.Business.CacheInventoryMovement.CacheData(
            vProductID:=ProductID,
            vFowstID:=FowstID,
            vTransactionDate:=CDate(TransactionDate))

        Return DateTimeStamp()

    End Function

    Public Function ZRead() As String

        Dim pError As String
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Dim clsSalesReport As New ReportPOS
        Dim pDT = clsSalesReport.getSalesZReportSummaryFromCache("By Date", "2021/5/19", "{A8FDFB5A-CD17-4096-9FB2-3A2640D379CA}", "{41FA817D-A876-4E65-BE6A-E201DB0DC59A}")
        Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, "{A8FDFB5A-CD17-4096-9FB2-3A2640D379CA}", "{C530BDF3-EA03-4DD3-95CD-A384374AA570}")

        Dim pReport = New ProReport
        pReport.SourceFilename = "ZRead"
        pReport.AddTable("Data", pDT)
        pReport.AddTable("Header", pDtHeader)
        pReport.ExportXmlFile()

        Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp\test\zread.pdf")
        CreateFolder(pFilename)
        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

        Return pFilename

    End Function

    Public Function GetRecursiveContents() As String

        Dim pError As String
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Dim pIssues = WebRequest.Instance.Business.LibraryStock.GetRecursiveContents(WebRequest.Instance.OrgID)

        Return pIssues

    End Function

    Public Function FixItemRequisitionQty() As String

        Dim pError As String
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Dim pIssues = WebRequest.Instance.Business.TransInvoice.FixItemRequisitionQty

        Return pIssues

    End Function

    Public Function TestRoundDecimal() As String

        Debug.Assert(RoundDecimal(349.44, 0.32, eRoundingMethod.Up) = 349.64)

    End Function

    Public Function TestSynonym() As String

        Dim pSourcePublicDatabase = "temp_" & ShortGuid(modClient.AbcOrgID) & "_public"

        With New Synonyms()
            .Prefix = "Mobiz"
            .SourceDatabaseName = "Mobiz"

            Try
                .SourceDatabase = pSourcePublicDatabase
                .ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
                    .Server = Common.ConnectionInfo.Server,
                    .Database = pSourcePublicDatabase,
                    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                    .Username = Common.ConnectionInfo.Username,
                    .Password = Common.ConnectionInfo.Password})
                .Generate()
            Catch ex As System.Exception
                AppendToDebug(, True, ex, True)
            End Try

            Try
                .SourceDatabase = pSourcePublicDatabase
                .ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
                    .Server = Common.ConnectionInfo.Server,
                    .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_private",
                    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                    .Username = Common.ConnectionInfo.Username,
                    .Password = Common.ConnectionInfo.Password})
                .Generate()
            Catch ex As System.Exception
                AppendToDebug(, True, ex, True)
            End Try
        End With

        Return DateTimeStamp()

    End Function

    Public Function CopyUdf() As String

        Dim pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "Mobiz",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        Dim pDatabase = New DBCompare.Database(pSourceProData)
        Dim pUdfs = pDatabase.LoadUDFs()
        Dim pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_public",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        For Each pUdf In pUdfs.Values
            pDestinationProData.ExecuteSQL(pUdf.Text)
        Next

        pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "MobizAbc",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        pDatabase = New DBCompare.Database(pSourceProData)
        pUdfs = pDatabase.LoadUDFs()
        pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_private",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        For Each pUdf In pUdfs.Values
            pDestinationProData.ExecuteSQL(pUdf.Text)
        Next

        Return DateTimeStamp()

    End Function

    Public Function CopyTables() As String

        Dim pSql As String

        Dim pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "Mobiz",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

        Dim pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_public",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

        Dim pDtTables = pSourceProData.GetTables()
        For Each pDrTable As DataRow In pDtTables.Rows
            If CStr2(pDrTable("Name")).Trim().ToLower() = "dtproperties".ToLower() Then
                Continue For
            End If
            If pDestinationProData.TableExist(pDrTable("Name")) = False Then
                pSql = pSourceProData.CreateTableSql(pDrTable("Name"))
                pDestinationProData.ExecuteSQL(pSql)
            End If
        Next

        TestSynonym()

        pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "MobizABC",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

        pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_private",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

        pDtTables = pSourceProData.GetTables()
        For Each pDrTable As DataRow In pDtTables.Rows
            If CStr2(pDrTable("Name")).Trim().ToLower() = "dtproperties".ToLower() Then
                Continue For
            End If
            If pDestinationProData.TableExist(pDrTable("Name")) = False Then
                pSql = pSourceProData.CreateTableSql(pDrTable("Name"))
                pDestinationProData.ExecuteSQL(pSql)
            End If
        Next

    End Function

    Public Function DatabaseSchema() As String

        Dim pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "MobizABC",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        Dim pDatabase As New DBCompare.Database(pSourceProData)
        pDatabase.LoadFromSql(True, False, False, True)
        pDatabase.SetCompareStatusToUnique()
        DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
        Dim pSql = pDatabase.GetScript()

        Dim pDestinationProData As ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With {
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_" & ShortGuid(modClient.AbcOrgID) & "_private",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})
        For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
            pDestinationProData.ExecuteSQL(pLine)
        Next

        Return DateTimeStamp()

    End Function

    Public Function ZipSqlMdf() As String

        Dim pProZip = New ProZip("C:\dev\debug\Mobiz\WebErp\temp\DataExport\Upload\cfe39df6-8809-4973-9515-810df75aed12\irw420ntjqbi4p9brus2z84olnx2x22xcp6mkzx3xqfazpboua6mp7joiccv1pm60ecdsospuct\CFE39DF6-8809-4973-9515-810DF75AED12.zip")
        Dim pFilename = "C:\dev\debug\Mobiz\WebErp\temp\DataExport\Upload\cfe39df6-8809-4973-9515-810df75aed12\irw420ntjqbi4p9brus2z84olnx2x22xcp6mkzx3xqfazpboua6mp7joiccv1pm60ecdsospuct\temp_CFE39DF6880949739515810DF75AED12_public.mdf"
        pProZip.AddFile(pFilename)

        Return DateTimeStamp()

    End Function

    Public Function CacheInvoices() As String
        ' usage: http://localhost:8080/test/cacheinvoices?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&InvoiceID=A3FABB32-811E-40D7-B6FB-3D7DDD0C2FA1
        ' http://app.mobiz-apps.com/test/cacheinvoices?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/cacheinvoices?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2
        ' https://pc.mobiz-apps.com/test/cacheinvoices?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=9e9f8468-6a9c-4908-9c25-4dd39aeaaed9

        'wow
        ' https://mobiz-apps.com/test/cacheinvoices?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        ' sports town / purple cow
        ' http://119.93.147.163/test/cacheinvoices?OrgID=f2d5c353-df14-46c8-a613-c6847fffda82

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceID = WebRequest.Instance.QueryString("InvoiceID")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            Try
                Dim pDays As Integer
                Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
                If IsNumeric(pDaysText) Then
                    pDays = CInt2(pDaysText)
                Else
                    pDays = 7
                End If
                Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoicesWithCredit(vMaxAgeInDays:=pDays, vInvoiceID:=pInvoiceID)
                Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                    "Max age in days: " & pDays & "<br />" & vbCrLf
                If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
                    pOut &= "InvoiceID: " & pInvoiceID
                End If
                Return pOut
            Catch ex As Exception
                Dim pStr = "Business.TransInvoice.CacheInvoicesWithCredit() Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
                End If
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End Try
        Else
            Return pError
        End If

    End Function

    Public Function CacheInvoicesModifiers() As String
        ' usage: http://localhost:8080/test/CacheInvoicesModifiers?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' http://app.mobiz-apps.com/test/CacheInvoicesModifiers?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheInvoicesModifiers?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheInvoicesModifiers?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheInvoicesModifiers?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceID = WebRequest.Instance.QueryString("InvoiceID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            Try
                Dim pDays As Integer
                Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
                If IsNumeric(pDaysText) Then
                    pDays = CInt2(pDaysText)
                Else
                    pDays = 7
                End If
                Dim pInvoiceDateFromProper As Date?
                Dim pInvoiceDateToProper As Date?
                If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                    pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                    If pInvoiceDateFromProper Is Nothing Then
                        Return "Error: invoicedatefrom, if specified, must be a valid date"
                    End If
                End If
                If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                    pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                    If pInvoiceDateToProper Is Nothing Then
                        Return "Error: invoicedateto, if specified, must be a valid date"
                    End If
                End If
                Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoicesModifiers(vMaxAgeInDays:=pDays, vInvoiceID:=pInvoiceID,
                    vInvoiceDateFrom:=pInvoiceDateFromProper,
                    vInvoiceDateTo:=pInvoiceDateToProper)
                Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                    "Max age in days: " & pDays & "<br />" & vbCrLf
                If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
                    pOut &= "InvoiceID: " & pInvoiceID
                End If
                Return pOut
            Catch ex As Exception
                Dim pStr = "Business.TransInvoice.CacheInvoicesModifiers() Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
                End If
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End Try
        Else
            Return pError
        End If

    End Function

    Shared ReadOnly mSemaphoreCache As New SemaphoreSlim(1)
    Shared mRunningProcess As String

    Public Function CacheInvoiceFlavors() As String
        ' usage: http://localhost:8080/test/CacheInvoiceFlavors?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2024-1-1&invoicedateto=2024-4-19
        ' http://20.24.43.82//test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoiceFlavors started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoiceFlavors(vMaxAgeInDays:=pDays,
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper)

            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                "Max age in days: " & pDays & "<br />" & vbCrLf
            Return pOut
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceFlavors() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try

    End Function

    Public Function CacheInvoiceComboMix() As String
        ' usage: http://localhost:8080/test/CacheInvoiceComboMix?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2024-1-1&invoicedateto=2024-4-19
        ' http://20.24.43.82//test/CacheInvoiceComboMix?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19

        ' Cafe France Corporation
        ' http://212.227.243.109/test/CacheInvoiceComboMix?OrgID=8c1c2898-0965-4a27-a4a8-9bcc1915568e&invoicedatefrom=2025-06-01&invoicedateto=2025-06-30

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheInvoiceComboMix?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheInvoiceComboMix?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheInvoiceComboMix?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoiceComboMix started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoiceComboMix(
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper)

            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                "<br />" & vbCrLf
            Return pOut
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceComboMix() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try

    End Function

    Public Function CacheInvoiceFinishedProducts() As String
        ' usage: http://localhost:8080/test/CacheInvoiceFinishedProducts?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2000-1-1&invoicedateto=2024-4-19
        ' http://20.24.43.82//test/CacheInvoiceFinishedProducts?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoiceFinishedProducts started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoiceFinishedProducts(vMaxAgeInDays:=pDays,
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper)

            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                "Max age in days: " & pDays & "<br />" & vbCrLf
            Return pOut
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceFinishedProducts() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try

    End Function

    Shared mUpdateCacheInvoiceDailyProgress As String
    Public Function UpdateCacheInvoiceDailyProgress() As String
        Try
            Dim pOut = ""
            If mSemaphoreCache.Wait(2000) Then
                pOut &= "Not busy. " & vbCrLf
                mSemaphoreCache.Release()
            Else
                pOut &= "Busy. " & vbCrLf
            End If

            If String.IsNullOrWhiteSpace(mUpdateCacheInvoiceDailyProgress) Then
                pOut &= "No progress yet" & vbCrLf
                Return pOut
            End If

            pOut &= mUpdateCacheInvoiceDailyProgress
            pOut = pOut.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pOut
        Catch ex As Exception

        End Try
    End Function
    Public Function CacheDailySalesPerSKUReport() As String
        ' usage: http://localhost:8080/test/CacheDailySalesPerSKUReport?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2024-08-11&invoicedateto=2024-08-12
        ' http://20.24.43.82//test/CacheDailySalesPerSKUReport?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2024-9-3invoicedateto=2024-9-3

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheDailySalesPerSKUReport?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheDailySalesPerSKUReport?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheDailySalesPerSKUReport?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")
        Dim pDontRelease As Boolean

        Dim pError As String

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheDailySalesPerSKUReport started " & DateTimeStamp()

        Try
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            Dim pAffectedRecords = pBusiness.TransInvoice.CacheDailySalesPerSKUReport(
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper,
                vOrgID:=pOrgID)

            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf
                            If String.IsNullOrWhiteSpace(pError) = False Then
                                'mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] CacheDailySalesPerSKUReport error: " & pError
                            Else
                                'mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] CacheDailySalesPerSKUReport: " & pOut
                            End If
                        End Using
                    Catch ex As Exception
                        'mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                        GetStackTrace(, ex)
                        AppendToDebug("CacheDailySalesPerSKUReport - vbN4NASSXLQ5! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCache.Release()
                    End Try
                End Sub
            )
            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheDailySalesPerSKUReport() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            If pDontRelease = False Then
            mSemaphoreCache.Release()
            End If
        End Try

    End Function

    Public Function CacheInvoiceProductSizes() As String
        ' usage: http://localhost:8080/test/CacheInvoiceProductSizes?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&invoicedatefrom=2000-1-1&invoicedateto=2024-4-19
        ' http://20.24.43.82//test/CacheInvoiceProductSizes?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19

        ' MobizPotato_Corner
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=91673033-3712-4976-8F0A-D298CDE49793&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2

        'wow
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&InvoiceID=BA7E1BD7-7E34-4CCD-8936-770A43A4B1A2&invoicedatefrom=2022-1-1&invoicedateto=2023-1-1
        ' https://mobiz-apps.com/test/CacheInvoiceFlavors?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&invoicedatefrom=2023-1-6&invoicedateto=2023-1-6&days=99

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoiceProductSizes started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoiceProductSizes(vMaxAgeInDays:=pDays,
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper)

            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                "Max age in days: " & pDays & "<br />" & vbCrLf
            Return pOut
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceProductSizes() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try

    End Function

    Public Function CacheInvoicesHourly() As String
        ' sample: http://localhost:8080/test/CacheInvoicesHourly?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&days=15&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19
        ' sample: http://20.24.43.82/test/CacheInvoicesHourly?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&days=15&invoicedatefrom=2024-4-1&invoicedateto=2024-4-19

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoicesHourly started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Dim pAffectedRecords As Integer

            WebRequest.Instance.ProData.CreateOrUpdateTable("CacheInvoicesHourly", "CacheInvoices")
            pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=pDays,
                vInvoiceDateFrom:=pInvoiceDateFromProper,
                vInvoiceDateTo:=pInvoiceDateToProper,
                vGroupByHour:=True)
            Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
                "Max age in days: " & pDays & "<br />" & vbCrLf
            Return pOut
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoicesHourly() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try

    End Function

    Public Function CacheStatus() As String

        If String.IsNullOrWhiteSpace(mRunningProcess) Then
            Return "never been started."
        End If

        Return mRunningProcess?.Replace(vbCrLf, "<br />" & vbCrLf)

    End Function
    Public Function CacheInvoicesDaily() As String
        ' sample: http://localhost:8080/test/CacheInvoicesDaily?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&days=15&invoicedatefrom=2024-4-18&invoicedateto=2024-4-19
        ' sample: http://20.24.43.82/test/CacheInvoicesDaily?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&days=999&invoicedatefrom=2024-09-13&invoicedateto=2024-09-16

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")
        Dim pFowstID = WebRequest.Instance.QueryString("FowstID")
        Dim pDontRelease As Boolean
        Dim pMsg As String
        If mSemaphoreCache.Wait(2000) = False Then
            pDontRelease = True
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoicesDaily started " & DateTimeStamp()

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If


        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

                Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pDR = New Business(pProData).LibraryOrganization.ByID(pOrgID)
                pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
                TimedThread(
                    Sub()
                        Try
                            Dim pBusiness = New Business(pProData)
                            pProData.CreateOrUpdateTable("CacheInvoicesDaily", "CacheInvoices")
                            Dim pAffectedRecords = pBusiness.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=pDays,
                                vInvoiceDateFrom:=pInvoiceDateFromProper,
                                vInvoiceDateTo:=pInvoiceDateToProper,
                                rProgress:=New Action(Of String)(
                                    Sub(vText As String)
                                        mRunningProcess &= vbCrLf & "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] " & vText
                                End Sub),
                            vFowstID:=pFowstID)
                            pMsg = "CacheInvoicesDaily - vbG2U9M74MCK. pAffectedRecords = " & pAffectedRecords
                            mRunningProcess &= vbCrLf & pMsg
                            AppendToLog(pMsg)
                        Catch ex As Exception
                            pMsg = "CacheInvoicesDaily - vbYQXWH2NMNY! " & vbCrLf & "Error: " & ex.Message
                            mRunningProcess &= vbCrLf & pMsg
                            AppendToDebug(pMsg, True, ex, True)
                        Finally
                            mSemaphoreCache.Release()
                        End Try
                    End Sub
                )
                'Return "maybe ok " & DateTimeStamp()

            'Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
            '    "Max age in days: " & pDays & "<br />" & vbCrLf
            'Return pOut
            pDontRelease = True
            Return "process is still running - " & DateTimeStamp()
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceDaily() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            If pDontRelease = False Then
            mSemaphoreCache.Release()
            End If
        End Try

    End Function

    Public Function CacheInvoicesMonthly() As String
        ' sample: http://localhost:8080/test/CacheInvoicesMonthly?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&days=15&invoicedatefrom=2024-4-1&invoicedateto=2024-5-19
        ' sample: http://20.24.43.82:8080/test/CacheInvoicesMonthly?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&days=9999&invoicedatefrom=2024-1-1&invoicedateto=2024-10-31

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pInvoiceDateFrom = WebRequest.Instance.QueryString("invoicedatefrom")
        Dim pInvoiceDateTo = WebRequest.Instance.QueryString("invoicedateto")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. optional query string: days (default: 7), invoicedatefrom (format: 2000-12-31), invoicedateto (format: 2000-12-31)"
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        If mSemaphoreCache.Wait(2000) = False Then
            Return "Another cache process is running. " & mRunningProcess
        End If
        mRunningProcess = "CacheInvoicesMonthly started " & DateTimeStamp()

        Try
            Dim pDays As Integer
            Dim pDaysText = CStr2(WebRequest.Instance.QueryString("days"))
            If IsNumeric(pDaysText) Then
                pDays = CInt2(pDaysText)
            Else
                pDays = 7
            End If
            Dim pInvoiceDateFromProper As Date?
            Dim pInvoiceDateToProper As Date?
            If String.IsNullOrWhiteSpace(pInvoiceDateFrom) = False Then
                pInvoiceDateFromProper = ParseDate(pInvoiceDateFrom)
                If pInvoiceDateFromProper Is Nothing Then
                    mSemaphoreCache.Release()
                    Return "Error: invoicedatefrom, if specified, must be a valid date"
                End If
            End If
            If String.IsNullOrWhiteSpace(pInvoiceDateTo) = False Then
                pInvoiceDateToProper = ParseDate(pInvoiceDateTo)
                If pInvoiceDateToProper Is Nothing Then
                    mSemaphoreCache.Release()
                    Return "Error: invoicedateto, if specified, must be a valid date"
                End If
            End If

            Try
                Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pDR = New Business(pProData).LibraryOrganization.ByID(pOrgID)
                pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
                TimedThread(
                    Sub()
                        Try
                            '#If DEBUG Then
                            Thread.Sleep(10000)
                            '#End If
                            Dim pBusiness = New Business(pProData)
                            pProData.CreateOrUpdateTable("CacheInvoicesMonthly", "CacheInvoices")
                            AppendToDebug("CacheInvoicesMonthly start - vbMVTVH4GC5A! " & vbCrLf &
                                "pInvoiceDateFromProper = " & pInvoiceDateFromProper & vbCrLf &
                                "pInvoiceDateToProper = " & pInvoiceDateToProper, True)
                            Dim pAffectedRecords = pBusiness.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=pDays,
                                vInvoiceDateFrom:=pInvoiceDateFromProper,
                                vInvoiceDateTo:=pInvoiceDateToProper,
                                vGroupByMonth:=True)
                            AppendToDebug("CacheInvoicesMonthly end - vbMYGTUFJF4G! " & vbCrLf &
                                "pInvoiceDateFromProper = " & pInvoiceDateFromProper & vbCrLf &
                                "pInvoiceDateToProper = " & pInvoiceDateToProper, True)
                        Catch ex As Exception
                            AppendToDebug("CacheInvoicesMonthly - vbYQXWH2NMNY! " & vbCrLf & ex.Message, True, ex, True)
                        Finally
                            mSemaphoreCache.Release()
                        End Try
                    End Sub
                )
                'Return "maybe ok " & DateTimeStamp()
            Catch ex As Exception
                mSemaphoreCache.Release()
                Return ex.Message & " " & DateTimeStamp()
            End Try

            'Dim pOut = "Number of inserted/updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf &
            '    "Max age in days: " & pDays & "<br />" & vbCrLf
            'Return pOut
            Return "process is still running"
        Catch ex As Exception
            mSemaphoreCache.Release()
            Dim pStr = "Business.TransInvoice.CacheInvoiceDaily() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
        End Try

    End Function

    Public Function AddFranchisor() As String

        Dim pOrgID = modClient.DemoCorpOrgID
        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If
        WebRequest.Instance.Business.LibraryOrganization.AddFranchisor(
            vFranchisorOrgID:=modClient.AbcOrgID,
            vFranchisorDatabase:="MobizABC",
            vFranchiseeOrgFowstID:="006a0ead-3fc3-4324-9480-7cc52e388954")

    End Function

    Public Function DbSchemaCoderStart() As String
        Dim p = New DbSchemaCoder().Start()
        If IsNumeric(p) Then
            Return "number of schema changes :" & p
        End If
        p = p.Replace(vbCrLf, "<br />" & vbCrLf)

        Return p
    End Function

    Public Function TestGetYearWeek() As String
        Return DateRange.TestGetYearWeek()
    End Function

    Public Function FixInvoicesDates() As String
        ' usage: http://localhost:8080/test/FixInvoicesDates?OrgID=cfe39df6-8809-4973-9515-810df75aed12&Commit=0
        ' http://cinco.mobiz-apps.com:8080/test/FixInvoicesDates?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&Commit=0

        Dim pDateFrom = New Date(2021, 11, 1)
        Dim pDateTo = New Date(2021, 11, 3)
        Dim pSourceRecordCount As Integer
        Dim pUpdatedRecordCount As Integer

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")
        Dim pCommit = CBool2(WebRequest.Instance.QueryString("Commit"))

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID. Optional: Commit=1"
        End If

        Dim pSqlAll As String = ""
        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            Try
                WebRequest.Instance.ProData.BeginTransaction()
                Dim pSql = "SELECT ID" & vbCrLf &
                    ",InvoiceDate" & vbCrLf &
                    ",LastUpdated" & vbCrLf &
                    ",DateCreated" & vbCrLf &
                    " FROM TransInvoice" & vbCrLf &
                    " WHERE CONVERT(date, DateCreated) BETWEEN " & ReadyString(Format(pDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(pDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf &
                    " ORDER BY DateCreated"
                Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
                pSourceRecordCount = pDT.Rows.Count
                For Each pDR As DataRow In pDT.Rows
                    Dim pIU = New InsertUpdate("TransInvoice")
                    Dim pReverse As Date?
                    If IsDate(pDR("InvoiceDate")) Then
                        Try
                            pReverse = New DateTime(CDate(pDR("InvoiceDate")).Year, CDate(pDR("InvoiceDate")).Day, CDate(pDR("InvoiceDate")).Month, CDate(pDR("InvoiceDate")).Hour, CDate(pDR("InvoiceDate")).Minute, CDate(pDR("InvoiceDate")).Second, CDate(pDR("InvoiceDate")).Millisecond)
                        Catch ex As Exception
                            pReverse = Nothing
                        End Try
                        If pReverse IsNot Nothing Then
                            Dim pDiff = Math.Abs((CDate(pDR("DateCreated")) - CDate(pDR("InvoiceDate"))).TotalSeconds)
                            Dim pReverseDiff = Math.Abs((CDate(pDR("DateCreated")) - pReverse.Value).TotalSeconds)
                            If pDiff > pReverseDiff Then
                                pIU("InvoiceDate") = ReadyString(Format(pReverse, cGlobal.FormatDateTimeSql2)) & " -- InvoiceDate: " & Format(CDate(pDR("InvoiceDate")), cGlobal.FormatDateTimeSql) & "; DateCreated: " & Format(CDate(pDR("DateCreated")), cGlobal.FormatDateTimeSql)
                            End If
                        End If
                    End If
                    If IsDate(pDR("LastUpdated")) Then
                        Try
                            pReverse = New DateTime(CDate(pDR("LastUpdated")).Year, CDate(pDR("LastUpdated")).Day, CDate(pDR("LastUpdated")).Month, CDate(pDR("LastUpdated")).Hour, CDate(pDR("LastUpdated")).Minute, CDate(pDR("LastUpdated")).Second, CDate(pDR("LastUpdated")).Millisecond)
                        Catch ex As Exception
                            pReverse = Nothing
                        End Try
                        If pReverse IsNot Nothing Then
                            Dim pDiff = Math.Abs((CDate(pDR("DateCreated")) - CDate(pDR("LastUpdated"))).TotalSeconds)
                            Dim pReverseDiff = Math.Abs((CDate(pDR("DateCreated")) - pReverse.Value).TotalSeconds)
                            If pDiff > pReverseDiff Then
                                pIU("LastUpdated") = ReadyString(Format(pReverse, cGlobal.FormatDateTimeSql2)) & " -- LastUpdated: " & Format(CDate(pDR("LastUpdated")), cGlobal.FormatDateTimeSql) & "; DateCreated: " & Format(CDate(pDR("DateCreated")), cGlobal.FormatDateTimeSql)
                            End If
                        End If
                    End If
                    If pIU.Count > 0 Then
                        pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                        WebRequest.Instance.ProData.ExecuteSQL(pSql)
                        pUpdatedRecordCount += 1
                        pSqlAll &= pSql & vbCrLf & vbCrLf
                    End If
                Next

                If pCommit Then
                    WebRequest.Instance.ProData.CommitTransaction()
                Else
                    WebRequest.Instance.ProData.RollbackTransaction()
                End If

                pSql = "Date From : " & Format(pDateFrom, cGlobal.FormatDateTime) & "<br />" & vbCrLf &
                    "Date To : " & Format(pDateTo, cGlobal.FormatDateTime) & "<br />" & vbCrLf &
                    "pSourceRecordCount : " & pSourceRecordCount & "<br />" & vbCrLf &
                    "pUpdatedRecordCount : " & pUpdatedRecordCount & "<br />" & vbCrLf &
                    "Commit : " & pCommit & "<br />" & vbCrLf &
                    pSqlAll.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pSql

            Catch ex As Exception
                Dim pStr = "FixInvoicesDates Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
                End If
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End Try
        Else
            Return pError
        End If

    End Function

    Public Function StringToArray() As String

        Dim p = Client.StringToArray("1234567890", 3)
        If p.Length <> 4 Then
            Return "failed vbKP4RH6LHH3!"
        End If
        If p(0) <> "123" Then
            Return "failed vbUDNBVNFTR8!"
        End If
        If p(1) <> "456" Then
            Return "failed vbG2FRPQXHLQ!"
        End If
        If p(2) <> "789" Then
            Return "failed vb8TGUZQLC8G!"
        End If
        If p(3) <> "0" Then
            Return "failed vbR5TJRZ53FB!"
        End If

        Return "success vb9MVU3ZAX82!"

    End Function

    Public Function SendEmail() As String

        Try
            Dim pBody = FileText(Common.ServerPath & "temp\EmailReceipt.html")
            pBody &= "<br /><br />this is the email body " & DateTimeStamp()

            SendMail("customercare@email.mobiz-apps.com",
                "arvinboggs@gmail.com, arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com, arvin.meralpis@mobiz-apps.com",
                "Mobiz",
                "this is the email subject " & DateTimeStamp(),
                pBody,
                "smtp.pepipost.com",
                587,
                "arvinboggs",
                "p$P.p/Kus7v%3<A@~dN^", , ,
                vAttachmentName:="schema.zip",
                vAttachmentFilename:=Common.ServerPath & "temp\schema.zip", True)
            Return "success " & DateTimeStamp()
        Catch ex As Exception
            Dim pStr = "SendEmail() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        End Try

    End Function

    Public Function SendEmail2() As String
        SendMail("customercare@mobiz-apps.com",
                                          "francis.hernandez@mobiz-apps.com",
                                          "Mobiz",
                                          "Device with issues connecting to server as of " & Format(DateAdd(DateInterval.Day, -1, Today), "MM/dd/yy"),
                                          "<div>test 123</div>",
                                          "smtp-mail.outlook.com",
                                          587,
                                          "customercare@mobiz-apps.com",
                                          "MobizCustCare2221", , , , , True)

        Return "success " & DateTimeStamp()

    End Function

    Public Function CacheReceivablePurchaseOrder() As String
        ' usage: http://localhost:8080/test/CacheReceivablePurchaseOrder?OrgID=cfe39df6-8809-4973-9515-810df75aed12
        ' http://app.mobiz-apps.com/test/cacheinvoices?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID."
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            Try
                Dim pAffectedRecords = WebRequest.Instance.Business.TransInvoice.CacheReceivablePurchaseOrder()
                Return "Number of updated rows: " & Format(pAffectedRecords, cGlobal.FormatInteger) & "<br />" & vbCrLf
            Catch ex As Exception
                Dim pStr = "Business.TransInvoice.CacheReceivablePurchaseOrder() Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
                End If
                pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
                Return pStr
            End Try
        Else
            Return pError
        End If

    End Function

    Public Function ClearCurrentDatabaseFromTablesLastUpdated() As String
        ' usage: http://localhost:8080/test/ClearCurrentDatabaseFromTablesLastUpdated?OrgID=cfe39df6-8809-4973-9515-810df75aed12
        ' http://app.mobiz-apps.com/test/ClearCurrentDatabaseFromTablesLastUpdated?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D

        Dim pOrgID = WebRequest.Instance.QueryString("OrgID")

        If IsGuid(pOrgID) = False Then
            Return "Query string OrgID must be a GUID."
        End If

        Dim pError As String
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) Then
            Dim pSql = " DELETE FROM TablesLastUpdated" & vbCrLf &
                " WHERE TableName = 'Current Database'"
            Return WebRequest.Instance.ProData.ExecuteSQL(pSql)
        Else
            Return pError
        End If

    End Function

    Public Function DataArchiving() As String

        Dim pError As String
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Dim pDataArchiver As New DataArchiver
        pDataArchiver.OrgID = modClient.AbcOrgID
        pDataArchiver.DatabaseName = WebRequest.Instance.ProData.GetCurrentDatabase()
        pDataArchiver.Execute()

        Return DateTimeStamp()

    End Function

    Public Function AddFileUser() As String

        Dim wi As WindowsIdentity = WindowsIdentity.GetCurrent()
        Dim wp As WindowsPrincipal = New WindowsPrincipal(wi)
        Dim isAdmin As Boolean = wp.IsInRole(WindowsBuiltInRole.Administrator)

        Dim pFilename = "E:\dev\debug\Mobiz\WebErp\temp\DataExport\Upload\cfe39df6-8809-4973-9515-810df75aed12\zqd9saw44v33qo0xu4ocprg6g0yi1x8wx3isrmzvbhkilep44ku1a25cwg3o9izjpdgqaz2vkjy\temp_CFE39DF6880949739515810DF75AED12_private.mdf"
        WebPosDbDownloaderController.AddFileUser(pFilename)

        Return DateTimeStamp()

    End Function

    Public Class GetWindowsUserResult
        Public Name As String
        Public IsAdmin As Boolean
    End Class

    Public Function GetWindowsUser() As String

        Dim wi As WindowsIdentity = WindowsIdentity.GetCurrent()
        Dim wp As WindowsPrincipal = New WindowsPrincipal(wi)
        Dim isAdmin As Boolean = wp.IsInRole(WindowsBuiltInRole.Administrator)

        Dim pResult As New GetWindowsUserResult
        pResult.Name = wi.Name
        pResult.IsAdmin = isAdmin

        Dim pOut = JsonConvert.SerializeObject(pResult, Formatting.Indented)

        Return pOut

    End Function

    <HttpGet>
    Public Function TestExportErpToMobizSettings()

        Dim p As New ErpToMobizExporter()

        p.SaveLastUpdated()
        p.LoadLastUpdated()

    End Function

    <HttpGet>
    Public Function TestExportErpToMobiz()

        Try
            Dim p As New ErpToMobizExporter()

            p.Settings = New ErpToMobizExporter.SettingsClass()
            p.LastestRecordsFirst = True
            p.Export()
            TestImportErpToMobiz(p.ZipFilename)

            Return "ok " & DateTimeStamp()
        Catch ex As Exception
            Return (ex.Message & vbCrLf & GetStackTrace(, ex)).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Private Function TestImportErpToMobiz(vSourceZipFilename As String) As Boolean

        Dim p As New ErpToMobizImporter

        p.SourceZipFilename = vSourceZipFilename
        p.Process()

    End Function

    <HttpGet>
    Public Function ImportErpToMobiz() As String

        Dim p As New ErpToMobizImporter

        p.SourceZipFilename = Common.ServerPath & "temp\ImportErpToMobiz\" & "2015_07_09__11_19_38_3140.zip"
        p.Process()

    End Function

    <HttpGet>
    Public Function ExportForErpToMobizComparer() As String

        Try
            Dim p As New ErpToMobizExporter()
            p.Settings = New ErpToMobizExporter.SettingsClass()
            p.LastestRecordsFirst = True
            p.Export()
            Debug.WriteLine(p.ZipFilename)

            Return "ok " & DateTimeStamp()
        Catch ex As Exception
            Return (ex.Message & vbCrLf & GetStackTrace(, ex)).Replace(vbCrLf, "<br />")
        End Try

    End Function

    <HttpGet>
    Public Function ErpToMobizComparer() As String

        Dim p As New ErpToMobizComparer

        p.SourceZipFilename = Common.ServerPath & "temp\ExportErpToMobiz\" & "test.zip"
        p.Process()

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function ErpToMobizComparerDirect() As String

        Dim p As New ErpToMobizComparerDirect

        Return p.Process()

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function PoXmlSm() As String

        Dim pError As String
        WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Dim pPoXmlImport = New PoXmlImport(WebRequest.Instance.Business)
        pPoXmlImport.SourceFilename = "C:\Users\admin\Downloads\PO_1073235604_05122022152529.xml"
        pPoXmlImport.ImportSm()

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function SkippedInvoiceNumbers() As String

        Dim pError As String
        WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
        WebRequest.Instance.OrgID = modClient.AbcOrgID
        WebRequest.Instance.OrgID = modClient.DucOrgID
        WebRequest.Instance.OrgID = modClient.DemoCorpOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.TransInvoice.SkippedInvoiceNumbers(New Date(1980, 1, 1))

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function AddSalesReturnToInventoryLedger() As String

        Dim pError As String
        WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
        WebRequest.Instance.OrgID = modClient.SportsTownOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("998C8743-6640-4864-A397-4852059ABD66", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("B8129D9D-A83E-462D-94CC-96B4217F9A85", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("8C08A52B-40FD-4C01-B02A-ED2948E62753", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("B68F4D70-B560-409F-9F58-D738D68F86BC", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("E081D0AD-B69F-4197-96C9-DD6260D17711", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("07262AA3-7EC8-4E34-A277-3795BDA0BB1C", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("4C18DF6B-413F-440C-A162-7B113674EAFA", "2A844025-F9AB-419F-8A65-C02E70950915")
        WebRequest.Instance.Business.TransMemoSalesReturn.AddToInventoryLedger("5B745D56-91F5-4DFA-839E-A806B503ED5F", "2A844025-F9AB-419F-8A65-C02E70950915")

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function ExportToXmlZip() As String

        Dim pError As String
        WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
        WebRequest.Instance.OrgID = modClient.DemoCorpOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        WebRequest.Instance.Business.LibraryOrganization.ExportToXmlZip("11c42710-7ad0-466a-b019-6e97ba9078d1")

        Return "maybe ok " & DateTimeStamp()

    End Function

    <HttpGet>
    Public Function FranchiseeDataExporterDateStarted() As String

        'Try
        '    Dim pOut = "Current server time = " & Format(Now, cGlobal.FormatDateTimeSql) & "<br />" & vbCrLf &
        '    "DateStarted = " & Format(FranchiseeDataExporter.DateStarted, cGlobal.FormatDateTimeSql) & "<br />" & vbCrLf &
        '    "RecordsSaved = " & Format(FranchiseeDataExporter.RecordsSaved, cGlobal.FormatInteger) & "<br />" & vbCrLf &
        '    "IsBusy = " & FranchiseeDataExporter.IsBusy() & "<br />" & vbCrLf

        '    If FranchiseeDataExporter.Instance?.FowstCount > 0 _
        '        AndAlso FranchiseeDataExporter.Instance?.FowstIndex < FranchiseeDataExporter.Instance?.FowstCount Then
        '        pOut &= "Processing fowst " & (FranchiseeDataExporter.Instance?.FowstIndex + 1) & " of " & FranchiseeDataExporter.Instance?.FowstCount.ToString(cGlobal.FormatInteger) & "<br />" & vbCrLf
        '    End If

        '    pOut &= "ErrorMessage = " & FranchiseeDataExporter.ErrorMessage & "<br />" & vbCrLf &
        '    "ForceFranchiseeDatabase = " & FranchiseeDataExporter.Instance?.ForceFranchiseeDatabase & "<br />" & vbCrLf &
        '    "ForceFranchisorDatabase = " & FranchiseeDataExporter.Instance?.ForceFranchisorDatabase & "<br />" & vbCrLf &
        '    "StartDate = " & FranchiseeDataExporter.Instance?.StartDate.ToString(cGlobal.FormatDateTimeSql) & "<br />" & vbCrLf &
        '    "EndDate = " & FranchiseeDataExporter.Instance?.EndDate.ToString(cGlobal.FormatDateTimeSql) & "<br />" & vbCrLf &
        '    "LastMessage = " & FranchiseeDataExporter.Instance?.LastMessage & "<br />" & vbCrLf &
        '    "LastMessageDate = " & FranchiseeDataExporter.Instance?.LastMessageDate.ToString(cGlobal.FormatDateTimeSql) & "<br />" & vbCrLf
        '    If FranchiseeDataExporter.Instance IsNot Nothing Then
        '        pOut &= "LastMessage age = " & FormatTime((Now - FranchiseeDataExporter.Instance.LastMessageDate).TotalSeconds) & "<br />" & vbCrLf
        '    End If

        '    Return pOut
        'Catch ex As Exception
        '    Return (ex.Message & vbCrLf & GetStackTrace(, ex)).Replace(vbCrLf, "<br />")
        'End Try

    End Function

    <HttpGet>
    Public Function FranchiseeDataExporterStart() As String

        Dim pError As String
        Dim pOrgID As String
        WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
        'WebRequest.Instance.OrgID = modClient.DemoCorpOrgID
        'WebRequest.Instance.OrgID = "91673033-3712-4976-8F0A-D298CDE49793" ' MobizPotato_Corner
#If DEBUG Then
        pOrgID = modClient.DemoCorpOrgID
#Else
        pOrgID = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba"
#End If
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Try
            Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
            Dim pDR = New Business(pProData).LibraryOrganization.ByID(pOrgID)
            pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
            TimedThread(
                Sub()
                    Try
                        'Dim pFranchiseeDataExporter = New FranchiseeDataExporter(pProData) ' new
                        'pFranchiseeDataExporter.CopyRecords(pOrgID)
                    Catch ex As Exception
                        AppendToDebug("FranchiseeDataExporterStart - vbLFHEVWVGKW! " & vbCrLf & ex.Message, True, ex, True)
                    End Try
                End Sub)
            Return "maybe ok " & DateTimeStamp()
        Catch ex As Exception
            Return ex.Message & " " & DateTimeStamp()
        End Try

    End Function

    '<HttpGet>
    'Public Function FranchiseeDataComparerStart() As String

    '    Dim pError As String
    '    Dim pOrgID As String
    '    WebSession.Instance.PeopleUserID = modClient.EmployeeWebGuidID
    '    pOrgID = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba"
    '    WebRequest.Instance.OrgID = pOrgID
    '    If WebRequest.Instance.HandleOrganization(True, pError) = False Then
    '        Return pError
    '    End If

    '    Try
    '        Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
    '        Dim pBusiness = New Business(pProData)
    '        Dim pDR = pBusiness.LibraryOrganization.ByID(pOrgID)
    '        pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
    '        Dim unused = pBusiness.FranchiseeDataComparer.Start(pOrgID, #2024/6/23#, #2024/6/23#)

    '        Return "maybe ok " & DateTimeStamp()
    '    Catch ex As Exception
    '        Return ex.Message & " " & DateTimeStamp()
    '    End Try

    'End Function

    Public Function ParseDateTest() As String

        Dim pTransDate As Date?
        pTransDate = ParseDate(Trim2("'2022-07-30 15:24:31'", "'"))
        If pTransDate IsNot Nothing Then
            pTransDate = pTransDate.Value.Date
            Debug.Print("ok man")
        End If

        Return pTransDate

    End Function

    Public Function DbProvisioner() As String

        'Dim pFilename = "C:\databases\non-existing MobizTemplate.bak"
        Dim pFilename = "F:\PC Databases\MobizTemplate.bak"
        Dim pOrgID As String = modClient.CashOrgID ' database must not already be attached

        Dim mDbBackup = New DbBackup
        mDbBackup.ProData = Common.WebRequest.ProData.Clone
        mDbBackup.SourceDatabase = "MobizTemplate"
        mDbBackup.Filename = pFilename

        AddHandler mDbBackup.Completed,
            Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                If e.Sucess Then
                    Dim mDbProvisioner = New DbProvisioner
                    mDbProvisioner.ProData = Common.WebRequest.ProData
                    mDbProvisioner.OrgID = pOrgID
                    AddHandler mDbProvisioner.Completed,
                        Sub(sender2 As Object, e2 As DbProvisioner.CompletedArgs)
                            If e2.Success Then
                                Debug.WriteLine(" ok na ni xa ")
                            Else
                                AppendToLog(e2.ErrorMessage) ' analyze d error
                            End If
                        End Sub
                    mDbProvisioner.Execute()
                Else
                    If e.SqlError IsNot Nothing Then
                        AppendToLog(e.SqlError.Message)
                    ElseIf e.Exception IsNot Nothing Then
                        AppendToLog(e.Exception.Message)
                    Else
                        AppendToLog("Backup database failure: unknown error")
                    End If
                End If
            End Sub
        mDbBackup.Execute()

    End Function

    Public Function AttachDatabase() As String

        Const pFilename = "E:\databases\temp_MobizABCArchive.mdf"
        Const pDatabase = "deletable_vb42UKZQTA22"

        Try
            ProPack.Utility.AddDirectoryUser(IO.Path.GetDirectoryName(pFilename))
            WebRequest.Instance.ProData.AttachDatabase(pDatabase, pFilename)
            WebRequest.Instance.ProData.DetachDatabase(pDatabase)
            Return DateTimeStamp() & " - ok"
        Catch ex As Exception
            Return DateTimeStamp() & " - " & ex.Message
        End Try

    End Function

    Public Function RunDeviceMonitoringCheck() As String

        Try
            MvcApplication1.RunDeviceMonitoringCheck()
            MvcApplication1.RunAWSDataSender()
            Return "ok - " & DateTimeStamp()
        Catch ex As Exception
            Return ex.Message & vbCrLf & GetStackTrace(, ex)
        End Try
    End Function

    Public Function CopyDevices() As String

        Dim pOrgID As String = modClient.DemoCorpOrgID
        Dim pError As String

        WebRequest.Instance.ProData.BeginTransaction()
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "vb2022_10_30__16_11_05_9076! " & pError
        End If

        Dim pSql = "SELECT TOP 1 ID FROM LibraryDevice"
        Dim pDeviceID = CStr2(WebRequest.Instance.ProData.ExecuteScalar(pSql))
        Dim pResult = WebRequest.Instance.Business.LibraryDevice.CopyToDatabase(pDeviceID, "MobizAbc")
        Dim pResultCount = WebRequest.Instance.Business.LibraryDeviceFowst.CopyToDatabase(pDeviceID, "MobizAbc")
        WebRequest.Instance.ProData.RollbackTransaction()

        Return "pResult: " & pResult & vbCrLf & "<br />" &
            "pResultCount: " & pResultCount


    End Function

    Public Function NewDeviceIDInvoiceNumbers() As ActionResult
        ' http://localhost:8080/test/NewDeviceIDInvoiceNumbers?orgid=9262FC46-DE29-4497-BDF2-8832F5E7576D&fowstid=006A0EAD-3FC3-4324-9480-7CC52E388954&sourcedeviceid=43CBDF1F-E563-4BAF-A2C0-381C313E4BE0&destinationdeviceid=833AA1CE-D35A-4EB1-9B58-0BDB2E7FCCF5
        ' http://20.24.43.82:8080/test/NewDeviceIDInvoiceNumbers?orgid=d362ffdb-15d1-451f-97dc-c567b94fb410&fowstid=d29511e9-391a-42e4-9dc6-e2d27c74eee7&sourcedeviceid=fdf65608-f1c7-469b-8b6c-1a3272137bfd&destinationdeviceid=e9190b67-94b2-4eda-9399-b401595ef869


        Try
            Dim pOrgID = Common.WebRequest.QueryString("orgid")
            Dim pFowstID = Common.WebRequest.QueryString("fowstid")
            Dim pSourceDeviceID = Common.WebRequest.QueryString("sourcedeviceid")
            Dim pDestinationDeviceID = Common.WebRequest.QueryString("destinationdeviceid")

            If IsGuid(pOrgID) = False Then
                Return Content("orgid must be guid. this function will not change the record.")
            End If
            pOrgID = ReadyGuid(pOrgID)

            If IsGuid(pFowstID) = False Then
                Return Content("fowstid must be guid")
            End If
            pFowstID = ReadyGuid(pFowstID)

            If IsGuid(pSourceDeviceID) = False Then
                Return Content("sourcedeviceid must be guid")
            End If
            pSourceDeviceID = ReadyGuid(pSourceDeviceID)

            If IsGuid(pDestinationDeviceID) = False Then
                Return Content("destinationdeviceid must be guid.")
            End If
            pDestinationDeviceID = ReadyGuid(pDestinationDeviceID)

            WebRequest.Instance.OrgID = pOrgID
            Dim pError As String
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return Content("vb7WH3QV2N8W! " & pError)
            End If
            Dim pDT = WebRequest.Instance.Business.TransInvoice.NewDeviceIDInvoiceNumbers(
                vFowstID:=pFowstID,
                vSourceDeviceID:=pSourceDeviceID,
                vDestinationDeviceID:=pDestinationDeviceID)
            Dim pFilename = Common.ServerPath & "temp\NewDeviceIDInvoiceNumbers\NewDeviceIDInvoiceNumbers_" & DateTimeStamp(False) & ".xlsx"
            CreateFolder(pFilename)
            TableToExcelFile(pDT, "", "", pFilename)

            Return File(pFilename, "application/xlsx", Path.GetFileName(pFilename))
        Catch ex As Exception
            Return Content(ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
        End Try

    End Function

    Public Function ChangeInvoiceNumbers() As String

        Try
            Dim pError As String
            Dim pOrgID As String = modClient.DucOrgID
            Dim pDeviceID As String
#If DEBUG Then
            pDeviceID = "833AA1CE-D35A-4EB1-9B58-0BDB2E7FCCF5"
            pOrgID = modClient.DemoCorpOrgID
#Else
        pDeviceID = "e9190b67-94b2-4eda-9399-b401595ef869"
        pOrgID = "d362ffdb-15d1-451f-97dc-c567b94fb410"
#End If

            'WebRequest.Instance.ProData.BeginTransaction()
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            'Dim pDtDevices = WebRequest.Instance.Business.LibraryDevice.Unused()
            'If pDtDevices.Rows.Count = 0 Then
            '    Return "No unused device"
            'End If
            'pDeviceID = CStr2(pDtDevices.Rows(0)("ID"))

            Dim pFilename = Common.ServerPath & "temp\ChangeInvoiceNumbers.xlsx"
            If IO.File.Exists(pFilename) = False Then
                Return "File, " & pFilename & ", does not exist. " & DateTimeStamp()
            End If
            Dim pDT = ExcelToTable(pFilename, New String() {"InvoiceID", "NewInvoiceNumber", "InvoiceAmount"}, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return pError
            End If
            Dim pFilename2 = Common.ServerPath & "temp\ChangeInvoiceNumbers_" & DateTimeStamp() & ".xlsx"
            MoveFile(pFilename, pFilename2)
            Dim pResultCount = WebRequest.Instance.Business.TransInvoice.ChangeInvoiceNumbers(pDT, pDeviceID, pError)

            'WebRequest.Instance.ProData.RollbackTransaction()

            Return "pError: " & pError & vbCrLf & "<br />" &
                "pResultCount: " & pResultCount
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function RenumberInvoices() As String

        Try
            Dim pCommit = CBool2(Common.WebRequest.QueryString("Commit"))
            Dim pError As String
            Dim pOrgID As String
            Dim pFowstID As String
            Dim pDeviceID As String
            Dim pNewDeviceID As String
            Dim pInvoiceDate As String = "2023-08-10"

            pInvoiceDate = "2023-09-01"
            pDeviceID = ""
            pNewDeviceID = ""
            pFowstID = ""
            pOrgID = ""

#If DEBUG Then
            pDeviceID = "8C8C4168-1F9F-457A-A758-33E6A75160CB"
            pNewDeviceID = "1C8C4168-1F9F-457A-A758-33E6A75160CB"
            pFowstID = "27C373C5-5026-4B3D-B16A-B0EDA224C5EF"
            pOrgID = modClient.DemoCorpOrgID

#Else
            ' potato corner
            pInvoiceDate = "2023-09-01"
            pDeviceID = "eee9ba93-f088-450d-9265-8630e955bb16"
            pNewDeviceID = "12bccf31-507b-4433-8a7b-d68dfa2ad587"
            pFowstID = "babc86b3-e7b6-429b-9600-c03fef146010"
            pOrgID = "d362ffdb-15d1-451f-97dc-c567b94fb410"
#End If

            WebRequest.Instance.ProData.BeginTransaction()
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            Dim pSql = "SELECT ID" & vbCrLf &
                " ,InvoiceNumber" & vbCrLf &
                " ,TotalAmount" & vbCrLf &
                " ,InvoiceDate" & vbCrLf &
                " ,DeviceID" & vbCrLf &
                " ,FowstID" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE InvoiceDate >= " & ReadyString(pInvoiceDate) & vbCrLf &
                " AND InvoiceNumber > 0" & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
                " AND FowstID = " & ReadyString(pFowstID) & vbCrLf &
                " ORDER BY InvoiceDate, LastUpdated"
            Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
            pDT.Columns.Add("NewInvoiceNumber", GetType(Integer))
            pDT.Columns.Add("NewDeviceID")

            Dim pInvoiceNumber As Integer
            For Each pDR As DataRow In pDT.Rows
                pInvoiceNumber += 1
                Dim pIU = New InsertUpdate("TransInvoice")
                pIU("InvoiceNumber") = pInvoiceNumber
                pIU("DeviceID") = ReadyString(pNewDeviceID)
                pDR("NewInvoiceNumber") = pInvoiceNumber
                pDR("NewDeviceID") = pNewDeviceID
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
            Next

            Dim pTimeStamp = DateTimeStamp()
            Dim pFilename = Common.ServerPath & "temp\RenumberInvoices\RenumberInvoices_" & pTimeStamp & "_before.xlsx"
            CreateFolder(pFilename)
            TableToExcelFile(pDT, "", "", pFilename)

            ' check the result
            pSql = "SELECT ID" & vbCrLf &
                " ,InvoiceNumber" & vbCrLf &
                " ,TotalAmount" & vbCrLf &
                " ,InvoiceDate" & vbCrLf &
                " ,DeviceID" & vbCrLf &
                " ,FowstID" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE InvoiceDate >= " & ReadyString(pInvoiceDate) & vbCrLf &
                " AND InvoiceNumber > 0" & vbCrLf &
                " AND DeviceID = " & ReadyString(pNewDeviceID) & vbCrLf &
                " AND FowstID = " & ReadyString(pFowstID) & vbCrLf &
                " ORDER BY InvoiceDate, LastUpdated"
            pDT = WebRequest.Instance.ProData.Execute(pSql)

            pFilename = Common.ServerPath & "temp\RenumberInvoices\RenumberInvoices_" & pTimeStamp & "_after.xlsx"
            CreateFolder(pFilename)
            TableToExcelFile(pDT, "", "", pFilename)

            If pCommit Then
                WebRequest.Instance.ProData.CommitTransaction() ' kung ready na
            Else
                WebRequest.Instance.ProData.RollbackTransaction() ' kung naga debug pa
            End If

            Return "pError: " & pError & vbCrLf & "<br />" &
                "pCommit: " & pCommit & vbCrLf & "<br />" &
                "pResultCount: " & pDT.Rows.Count
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function
    Public Function PdfSharpMerge() As String

        'Dim pFolder As String = "E:\dev\debug\Mobiz\WebErp\temp\MassInvoicePdfs\2023_03_20__13_10_26_3733"
        'Dim pFilenames = IO.Directory.GetFiles(pFolder, "*.pdf")
        'Dim pOutpudPdf As New PdfSharp.Pdf.PdfDocument()
        'For Each pFilename In pFilenames
        '    Using pSourcePdf = PdfSharp.Pdf.IO.PdfReader.Open(pFilename, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)
        '        For pPage As Integer = 0 To pSourcePdf.PageCount - 1
        '            pOutpudPdf.AddPage(pSourcePdf.Pages(pPage))
        '        Next
        '        pSourcePdf.Close()
        '    End Using
        'Next
        'pOutpudPdf.Save(AppendBackslash(pFolder) & "Merged_" & DateTimeStamp() & ".pdf")

    End Function

    Public Function SalesReturnCreatePdf() As String

        Try
            Dim pError As String
            Dim pOrgID As String = modClient.DucOrgID
            Dim pDeviceID As String
#If DEBUG Then
            pDeviceID = "59e4dd87-1225-4b71-80b1-7ac4516ae723"
            pOrgID = modClient.DemoCorpOrgID
#Else
        pDeviceID = ""
        pOrgID = modClient.DucOrgID
#End If

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            WebRequest.Instance.Business.TransMemoSalesReturn.CreatePdf("{E553E6B0-C381-4B5F-833A-CDFDBC5ACD4C}", WebRequest.Instance.OrgID)

            Return DateTimeStamp()
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function CancelPayment() As String

        Try
            Dim pError As String
            Dim pOrgID As String = modClient.DemoCorpOrgID
            Dim pDeviceID As String

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            WebRequest.Instance.Business.TransCustomerPayment.CancelPayment("a2f70237-bc73-4698-803d-fd71a7f1432a", pError, Guid.NewGuid().ToString())

            Return DateTimeStamp()
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function OldSqliteDeleterExecute() As ActionResult

        Try
            OldSqliteDeleter.Execute()
            Dim pFilename = Common.ServerPath & "temp\logs\OldSqliteDeleter.log"
            Return File(pFilename, "application/txt", "OldSqliteDeleter.log")
        Catch ex As Exception
            Return Content(ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
        End Try

    End Function

    Public Function StartDataImport() As String

        Dim pCount As Integer
        Dim pError As String
        Dim pDatabase As String
        Dim pOrgID As String

        Dim pOfflineDataController = New OfflineDataController()
        Dim pDtDbs = WebRequest.Instance.Business.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = CStr2(pDR("Database"))
                If String.IsNullOrWhiteSpace(pDatabase) Then
                    Continue For
                End If
                pOrgID = CStr2(pDR("ID"))

                WebRequest.Instance.OrgID = pOrgID
                If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                    AppendToLog("vbG2U5V58PKQ! " & pError, , "TestController_StartDataImport")
                    Continue For
                End If

                pOfflineDataController.CallImportOrganization(pOrgID)
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf &
                    GetStackTrace(, ex)
                AppendToLog(pError, , "TestController_StartDataImport")
            End Try
        Next pDR

        Return pCount

    End Function

    Public Function SaveCompanyOwnedOrFranchisee() As String

        Dim pCount As Integer
        Dim pError As String
        Dim pDatabase As String
        Dim pOrgID As String
        Dim pOut = ""

        Dim pOfflineDataController = New OfflineDataController()
        Dim pDtDbs = WebRequest.Instance.Business.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = CStr2(pDR("Database"))
                If String.IsNullOrWhiteSpace(pDatabase) Then
                    Continue For
                End If
                pOrgID = CStr2(pDR("ID"))

                WebRequest.Instance.OrgID = pOrgID
                If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                    AppendToLog("vbG2U5V58PKQ! " & pError, , "TestController_StartDataImport")
                    Continue For
                End If

                pOut &= pOrgID & " (" & CStr2(pDR("CompanyName")) & ") (" & pDatabase & ") = "
                pOut &= WebRequest.Instance.Business.SystemDefaults.SaveCompanyOwnedOrFranchisee(pOrgID)
                pOut &= ",<br />" & vbCrLf
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf &
                    GetStackTrace(, ex)
                AppendToLog(pError, , "TestController_SaveCompanyOwnedOrFranchisee")
            End Try
        Next pDR

        Return pOut

    End Function

    Public Function Ftp() As String
        Try
            '// https://learn.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
            Dim pFtpWebRequest = FtpWebRequest.Create("ftp://ftp.dlptest.com/2024_07_01__09_17_16_9325.txt") ' https://dlptest.com/ftp-test
            pFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile
            pFtpWebRequest.Credentials = New NetworkCredential("dlpuser", "rNrKYTX9g7z3RgJRmxWuGHbeu")

            Dim pFolder = Common.ServerPath & "temp\ftp test vb3QDFWC8J8G"
            Dim pFilename As String = pFolder & "\ftp test 2024_07_01__09_17_16_9325.txt"
            CreateFolder(pFilename)
            FileText(pFilename) = "this is just a test file. vbA4DSYHJFPR!"

            Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                Using requestStream = pFtpWebRequest.GetRequestStream()
                    'fileStream.CopyToAsync(requestStream)
                    fileStream.CopyTo(requestStream)
                    'Using response = pFtpWebRequest.GetResponse() ' stuck dri
                    '    Console.WriteLine("vb3822KY7XDH")
                    'End Using
                End Using
            End Using

            Return False
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vb2Z34MYWJFM: " & ex.Message)
            Return True
        End Try
    End Function

    Public Function FtpMobiz() As String
        Try
            Dim pFtpWebRequest = FtpWebRequest.Create("ftp://212.227.243.109/123456.txt")
            pFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile
            pFtpWebRequest.Credentials = New NetworkCredential("FTPTEST", "FTPTEST")
            Dim pFolder = Common.ServerPath & "temp\ftp test vb2024_07_11__17_10_53_0054"
            Dim pFilename As String = pFolder & "\ftp test " & DateTimeStamp() & ".txt"
            CreateFolder(pFilename)
            FileText(pFilename) = "this is just a test file. " & DateTimeStamp() & " vbUTCK6UKKDH!"

            Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                Using requestStream = pFtpWebRequest.GetRequestStream()
                    'fileStream.CopyToAsync(requestStream)
                    fileStream.CopyTo(requestStream)
                    'Using response = pFtpWebRequest.GetResponse() ' stuck dri
                    '    Console.WriteLine("vb3822KY7XDH")
                    'End Using
                End Using
            End Using

            Return False
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vb2Z34MYWJFM: " & ex.Message)
            Return True
        End Try
    End Function

    Public Function FtpRlc() As String
        Try
            Dim pFtpWebRequest = FtpWebRequest.Create("ftps://rlccloud.robinsonsland.com/123456.txt")
            pFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile
            pFtpWebRequest.Credentials = New NetworkCredential("accredit", "RLC@Partners")

            Dim pFolder = Common.ServerPath & "temp\ftp test vb2024_07_11__17_10_53_0054"
            Dim pFilename As String = pFolder & "\ftp test 2024_07_11__17_10_46_7099.txt"
            CreateFolder(pFilename)
            FileText(pFilename) = "this is just a test file. vbUTCK6UKKDH!"

            Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                Using requestStream = pFtpWebRequest.GetRequestStream()
                    'fileStream.CopyToAsync(requestStream)
                    fileStream.CopyTo(requestStream)
                    'Using response = pFtpWebRequest.GetResponse() ' stuck dri
                    '    Console.WriteLine("vb3822KY7XDH")
                    'End Using
                End Using
            End Using

            Return False
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vb2Z34MYWJFM: " & ex.Message)
            Return True
        End Try
    End Function

    Public Function SFtpMobiz() As String
        Try
            Using pClient = New Renci.SshNet.SftpClient("212.227.243.109", 21, "FTPTEST", "FTPTEST")
                pClient.Connect()
                Dim pFolder = Common.ServerPath & "temp\ftp test vb3QDFWC8J8G"
                Dim pFilename As String = pFolder & "\ftp test vb89M3UQMSJK.txt"
                CreateFolder(pFilename)
                FileText(pFilename) = "this is just a test file. vbSMM5QFEUF4!"
                Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                    pClient.UploadFile(fileStream, "vb89M3UQMSJK.txt")
                End Using
                'pClient.DownloadFile("/documents/document2.docx", Stream)
                'pClient.Delete("/documents/document1.docx")
            End Using
            Return "ok " & DateTimeStamp()
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vbETDSE33N3A: " & ex.Message)
            Return ex.Message
        End Try
    End Function

    Public Function SFtpRlc() As String
        Try
            Using pClient = New Renci.SshNet.SftpClient("rlccloud.robinsonsland.com", "PotatoCornerGal", "RLC@20241017Gal")
                pClient.Connect()
                Dim pFolder = Common.ServerPath & "temp\ftp test vb3QDFWC8J8G"
                Dim pFilename As String = pFolder & "\ftp test vb89M3UQMSJK.txt"
                CreateFolder(pFilename)
                FileText(pFilename) = "this is just a test file. vbSMM5QFEUF4!" & vbCrLf &
                    DateTimeStamp()
                Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                    pClient.UploadFile(fileStream, "FTP test " & DateTimeStamp(False) & ".txt")
                    'pClient.UploadFile(fileStream, "34320306.013")
                End Using
                'pClient.DownloadFile("/documents/document2.docx", Stream)
                'pClient.Delete("/documents/document1.docx")
            End Using
            Return "ok " & DateTimeStamp()
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vbETDSE33N3A: " & ex.Message)
            Return ex.Message
        End Try
    End Function

    Public Function SFtpRlc2() As String
        Try
            'Dim filereader As StreamReader = New StreamReader(ftpfilepath)
            'Dim filedata As Object = objJSSerializer.Deserialize(Of Object)(filereader.ReadLine)
            'filereader.Close()

            ' Setup session options
            Dim sessionOptions As New SessionOptions

            With sessionOptions
                .Protocol = Protocol.Sftp
                .HostName = "rlccloud.robinsonsland.com"
                .UserName = "PotatoCornerGal"
                .Password = "RLC@20241017Gal"
                .SshHostKeyFingerprint = "ssh-rsa 2048 4Rp7OzpN3LU0crtMrMf6fMgteMv0j6OdmsxdyCNXvWc=;ssh-rsa 2048 df:72:a7:bc:25:a9:04:ce:1a:0e:d3:10:39:11:d8:3d"
            End With

            Using session As New Session
                session.DisableVersionCheck = True

                ' Connect
                session.Open(sessionOptions)

                ' Upload files
                Dim transferOptions As New TransferOptions
                transferOptions.TransferMode = TransferMode.Binary

                Dim pFolder = Common.ServerPath & "temp\ftp test vb3QDFWC8J8G"
                Dim pFilename As String = pFolder & "\ftp test vb89M3UQMSJK.txt"
                CreateFolder(pFilename)
                FileText(pFilename) = "this is just a test file. vbSMM5QFEUF4!" & vbCrLf &
                    DateTimeStamp()

                Dim transferResult As TransferOperationResult
                transferResult = session.PutFiles(pFilename, "/*.*", False, transferOptions)
                transferResult.Check()
            End Using

            Return False
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function SFtp() As String
        Try
            Using pClient = New Renci.SshNet.SftpClient("54.254.44.52", 22, "NPPTOC11", "NPPTOC11")
                pClient.Connect()
                Dim pFolder = Common.ServerPath & "temp\ftp test vb3QDFWC8J8G"
                Dim pFilename As String = pFolder & "\ftp test vb89M3UQMSJK.txt"
                CreateFolder(pFilename)
                FileText(pFilename) = "this is just a test file. vbSMM5QFEUF4!"
                Using fileStream = System.IO.File.Open(pFilename, FileMode.Open, FileAccess.Read)
                    pClient.UploadFile(fileStream, "/NPPTOC11/vb89M3UQMSJK.txt")
                End Using
                'pClient.DownloadFile("/documents/document2.docx", Stream)
                'pClient.Delete("/documents/document1.docx")
            End Using
            Return "ok " & DateTimeStamp()
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error vbETDSE33N3A: " & ex.Message)
            Return ex.Message
        End Try
    End Function

    Public Function ImportMallLessor() As String

        Try
            Dim pError As String
            Dim pOrgID As String = modClient.DemoCorpOrgID
            Dim pDeviceID As String

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            WebRequest.Instance.Business.TransInvoice.ImportMallLessor("C:\Users\admin\Downloads\8c8c4168-1f9f-457a-a758-33e6a75160cb (1).zip", pOrgID, pError)
            If String.IsNullOrWhiteSpace(pError) Then
                Return DateTimeStamp()
            Else
                Return pError
            End If
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function OldFiles() As String

        Dim pFolder = Common.ServerPath & "temp\data import logs\success\"
        AppendToLog("vbWUBH3K4T9L! before IO.Directory.GetFiles()")
        Dim pFiles = IO.Directory.GetFiles(pFolder)
        AppendToLog("vbK533JLGNDD! after IO.Directory.GetFiles(): " & pFiles.Count)
        Dim pFolderInfo = New IO.DirectoryInfo(pFolder).GetFiles()
        AppendToLog("vbBXLN392CWS! New IO.DirectoryInfo(pFolder).GetFiles()" & pFolderInfo.Count)

        Return DateTimeStamp()

    End Function

    Public Function ImportStatus() As String

        Dim pOut = "Website started: " & ImportDataClass.FirstStarted.ToString(cGlobal.FormatDateTime) & " (" & FormatDurationBySeconds((Now - ImportDataClass.FirstStarted).TotalSeconds) & " ago)<br />" & vbCrLf & "<br/>" & vbCrLf

        pOut &= "Status: " & ImportDataClass.Status & "<br />" & vbCrLf
        pOut &= "Status time: " & ImportDataClass.StatusLastUpdated.ToString(cGlobal.FormatDateTime) & " (" & FormatDurationBySeconds((Now - ImportDataClass.StatusLastUpdated).TotalSeconds) & " ago)" & "<br/>" & vbCrLf & "<br/>" & vbCrLf

        pOut &= "Last successful import: " & ImportDataClass.LastSuccess & "<br/>" & vbCrLf
        pOut &= "Last successful import date and time: " & ImportDataClass.LastSuccessDate.ToString(cGlobal.FormatDateTime) & " (" & FormatDurationBySeconds((Now - ImportDataClass.LastSuccessDate).TotalSeconds) & " ago)" & "<br/>" & vbCrLf & "<br/>" & vbCrLf

        Return pOut

    End Function

    Public Function CreateDummyInventoryLedgerRecords() As String

        Try
            Dim pError As String
            Dim pOrgID As String = modClient.DemoCorpOrgID
            Dim pDeviceID As String

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vb4XLHRJXYJJ! " & pError
            End If

            WebRequest.Instance.Business.TransInventoryLedger.CreateDummyRecords()
            If String.IsNullOrWhiteSpace(pError) Then
                Return DateTimeStamp()
            Else
                Return pError
            End If
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function RunAWSDataSender() As String
        MvcApplication1.RunAWSDataSender()
    End Function

    Public Function Version() As String
        Return "002 - 2023_12_18__14_20_52_7358"
    End Function

    Public Function DeleteOldFiles() As String

        Dim pDT As DataTable

        Try
            Dim pFolders As New List(Of String)
            pFolders.Add(Common.ServerPath & "temp\logs\ImportDataClass\")
            pFolders.Add(Common.ServerPath & "temp\DataExport\Upload\")
            pFolders.Add(Common.ServerPath & "temp\DataExport\Download\")
            pFolders.Add(Common.ServerPath & "temp\TransPimSalesCachingToExcel\")
            pFolders.Add(Common.ServerPath & "temp\FranchiseeDataExporter\")
            pFolders.Add(Common.ServerPath & "temp\DownloadZReadXml\")
            pFolders.Add(Common.ServerPath & "temp\ThirdPartyPosFtpUploads\Summaries\")
            pFolders.Add(Common.ServerPath & "temp\ThirdPartyPosFtpUploads\DailySales\")
            pFolders.Add(Common.ServerPath & "temp\PrintablePdf\")
            pFolders.Add(Common.ServerPath & "temp\PrintablePdf\Requested\")
            pFolders.Add(Common.ServerPath & "temp\documents\")
            pFolders.Add(Common.ServerPath & "temp\OldDataDeleter\")

            For Each pFolder In pFolders
                If IO.Directory.Exists(pFolder) = False Then
                    Continue For
                End If
                pDT = DirectoryToTable(pFolder)
                If pDT.Rows.Count <= 100 Then
                    Continue For
                End If
                pDT.DefaultView.Sort = "LastWriteTime"
                pDT.Columns.Add("Deletable", GetType(Boolean))
                For p = 0 To pDT.Rows.Count - 101
                    Dim pDrv = pDT.DefaultView(p)
                    If Now.AddDays(-1) < pDrv("LastWriteTime") Then
                        Exit For
                    End If
                    Try
                        pDrv("Deletable") = True
                        'IO.File.Delete(pDrv("FullName"))
                    Catch ex As Exception
                        Stop2() ' just ignore errors
                    End Try
                Next
                Debug.WriteLine("dummy")
            Next
        Catch ex As Exception
            Stop2() ' just ignore errors
        End Try
    End Function

    Shared ReadOnly mSemaphoreOldDataDeleter As New SemaphoreSlim(1)
    Shared mOldDataDeleterStart As Date
    Public Function OldDataDeleter() As String

        Dim pDontRelease As Boolean

        Try
            If mSemaphoreOldDataDeleter.Wait(2000) = False Then
                pDontRelease = True
                Return "Another OldDataDeleter is running since " & mOldDataDeleterStart & ". vb6UELQVHGTP!"
            End If

            Dim pMode As String = Common.WebRequest.QueryString("mode")

            Dim pDateFromText = WebRequest.Instance.Data("DateFrom")
            Dim pDateFrom = ParseDate(pDateFromText)
            If pDateFrom Is Nothing Then
                Return JsonDictionary.Serialize("Error", "DateFrom must be a valid date.")
            End If
            pDateFrom = pDateFrom.Value.Date

            Dim pDateToText = WebRequest.Instance.Data("DateTo")
            Dim pDateTo = ParseDate(pDateToText)
            If pDateTo Is Nothing Then
                Return JsonDictionary.Serialize("Error", "DateTo must be a valid date.")
            End If
            pDateTo = pDateTo.Value.Date

            If pDateTo < pDateFrom Then
                Return JsonDictionary.Serialize("Error", "DateFrom must not be later than DateTo")
            End If

            Dim pConnectionInfo = New ConnectionInfo()
            pConnectionInfo.Server = Common.ConnectionInfo.Server
            pConnectionInfo.Database = WebRequest.Instance.Data("Database")
            pConnectionInfo.Username = Common.ConnectionInfo.Username
            pConnectionInfo.Password = Common.ConnectionInfo.Password
            pConnectionInfo.IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity

            TimedThread(
                Sub()
                    Try
                        mOldDataDeleterStart = Now
                        Dim p = New OldDataDeleter()
#If DEBUG Then
                        'If pMode = "delete" Then
                        '    p.Delete = True
                        'End If
#End If
                        p.DateFrom = pDateFrom.Value
                        p.DateTo = pDateTo.Value
                        p.Execute(pConnectionInfo)
                    Catch ex As Exception
                        AppendToDebug(ex.Message, True, ex, True)
                    Finally
                        mSemaphoreOldDataDeleter.Release()
                    End Try
                End Sub
            )
            pDontRelease = True
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreOldDataDeleter.Release()
            End If
        End Try

        Return JsonDictionary.Serialize("Result", DateTimeStamp())

    End Function

    Public Function DeviceUtilWeeklyReport()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        Return New Triggers().CreateWeeklyReport(Common.WebRequest.QueryStrings("DateFrom"), Common.WebRequest.QueryStrings("DateTo"))
    End Function

    Public Function ProductsAndContentsExcel() As String

        Try
            Dim pError As String
            Dim pOrgID As String = Common.WebRequest.QueryString("OrgID")

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vbZ6CNBJ9XHY! " & pError
            End If

            Dim pFilename = WebRequest.Instance.Business.LibraryStock.ProductsAndContentsExcel(pOrgID)
            Return pFilename
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function ComputedSCDiscountExempt() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim p = New DbSchemaCoder().ChangeTransInvoiceSCDiscountExempt(Common.WebRequest.ProData)
            Common.WebRequest.ProData.AddColumn("TransInvoice", "SCDiscountExempt", "AS (case when [SCDiscount]>(0) AND [InvoiceNumber]>(0) then [VAT]+[Vatable] else (0) end) PERSISTED", False)

            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function LongProcess() As String
        Thread.Sleep(60000)
        Return JsonDictionary.Serialize("ServerTime", DateTimeStamp())
    End Function

    Public Function GetServerUpdate() As String

        Dim COOrgID = "d362ffdb-15d1-451f-97dc-c567b94fb410" '"cfe39df6-8809-4973-9515-810df75aed12" 
        Dim FOOrgID = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba"
        Dim pSql As String
        Dim Result As String
        Dim FinalResult As String

        Dim pError As String
        WebRequest.Instance.OrgID = COOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        Try

            FinalResult = "Date Today : " & CStr2(Date.Today) & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceFinishedProducts where fromfranchisee=1 order by invoicedate  desc " 'franchisee CacheInvoiceFinishedProducts

            Result = "CacheInvoiceFinishedProducts FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceFinishedProducts where ISNULL(fromfranchisee,0)=0 order by invoicedate  desc " 'CO

            Result = "CacheInvoiceFinishedProducts CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoicesHourly where fromfranchisee=1 order by invoicedate  desc " 'franchisee CacheInvoicesHourly

            Result = "CacheInvoicesHourly FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoicesHourly where ISNULL(fromfranchisee,0)=0 order by invoicedate  desc " 'CO

            Result = "CacheInvoicesHourly CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceFlavors where  GroupName like '%FO%' order by invoicedate  desc " 'franchisee CacheInvoiceFlavors

            Result = "CacheInvoiceFlavors FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceFlavors where  GroupName like '%CO%' order by invoicedate  desc " 'CO

            Result = "CacheInvoiceFlavors CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoicesDaily where fromfranchisee=1 order by invoicedate  desc " 'franchisee CacheInvoicesDaily

            Result = "CacheInvoicesDaily FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoicesDaily where ISNULL(fromfranchisee,0)=0 order by invoicedate  desc " 'CO

            Result = "CacheInvoicesDaily  CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 TransactionDate from CacheInventoryMovement where  GroupName like '%FO%' order by TransactionDate  desc " 'franchisee CacheInventoryMovement

            Result = "CacheInventoryMovement FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 TransactionDate from CacheInventoryMovement where  GroupName like '%CO%' order by TransactionDate  desc " 'CO

            Result = "CacheInventoryMovement CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceModifiers where  GroupName like '%FO%' order by invoicedate  desc " 'franchisee CacheInvoiceModifiers

            Result = "CacheInvoiceModifiers FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceModifiers where  GroupName like '%CO%' order by invoicedate  desc " 'CO

            Result = "CacheInvoiceModifiers CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheDailySalesPerSKUReport where  GroupName like '%FO%' order by invoicedate  desc " 'franchisee CacheDailySalesPerSKUReport

            Result = "CacheDailySalesPerSKUReport FO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />"

            pSql = "Select top 1 invoicedate from CacheDailySalesPerSKUReport where  GroupName like '%CO%' order by invoicedate  desc " 'CO

            Result = "CacheDailySalesPerSKUReport CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select top 1 invoicedate from CacheInvoiceComboMix order by invoicedate  desc " 'CO Only CacheInvoiceComboMix

            Result = "CacheInvoiceComboMix CO: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            pSql = "Select DefaultValue  from systemdefaults where [description] = 'LastDeviceMonitoringDataCreation'"
            Result = "Last Device Monitoring Data Creation: " & CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            FinalResult &= Result & " <br />" & " <br />"

            Return FinalResult
        Catch ex As Exception
            Dim pStr = "Business.TransInvoice.CacheInvoiceFinishedProducts() Error" & vbCrLf &
            ex.Message & vbCrLf &
            GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
            End If
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pStr
        Finally
            mSemaphoreCache.Release()
        End Try
    End Function

    Public Function ProductMap() As String

        Try
            Dim pError As String
            Dim pWarning As String
            Dim pOrgID As String = modClient.DemoCorpOrgID

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                Return "vbVVUWHLWVHE! " & pError
            End If

            WebRequest.Instance.Business.LibraryProductMap.ImportExcel(pOrgID,
                "006a0ead-3fc3-4324-9480-7cc52e388954",
                modClient.EmployeeWebGuidID,
                "E:\deletables\ProductMap.xlsx",
                pError, pWarning)
            If String.IsNullOrWhiteSpace(pError) Then
                Return "success"
            Else
                Return pError
            End If
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    'Public Function CyberPosFlushZipConverter() As String

    '    Try
    '        Dim pError As String
    '        Dim pOrgID As String = modClient.DemoCorpOrgID

    '        WebRequest.Instance.OrgID = pOrgID
    '        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
    '            Return "vbVVUWHLWVHE! " & pError
    '        End If

    '        Dim p As New CyberPosFlushZipConverter()
    '        p.Execute("E:\docs\PC x CyberPOS Data Integration\File Header (1).xlsx", pError)
    '        If String.IsNullOrWhiteSpace(pError) Then
    '            Return "success"
    '        Else
    '            Return pError
    '        End If
    '    Catch ex As Exception
    '        Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
    '    End Try

    'End Function





    'Public Async Sub SalesTablesRequestURL()
    '    Try
    '        System.Net.ServicePointManager.SecurityProtocol =
    '            System.Net.SecurityProtocolType.Tls12 Or
    '            System.Net.SecurityProtocolType.Tls11 Or
    '            System.Net.SecurityProtocolType.Tls

    '        ' 1️⃣ Request presigned URL
    '        Dim client As New HttpClient()
    '        Dim apiUrl As String = "https://r799jap2ac.execute-api.ap-southeast-1.amazonaws.com/Dev/mpos/"
    '        Dim jsonData As String = "{""store_code"": ""PC0001"", ""transaction_date"": ""20250929"", ""terminal"": ""1""}"

    '        Dim apiContent = New StringContent(jsonData, Encoding.UTF8, "application/json")
    '        apiContent.Headers.Add("x-api-key", "VvePzFv5dCEZX0fJ2mwj2iLaI04QdiDaa6N6C21e")

    '        Dim apiResponse = Await client.PostAsync(apiUrl, apiContent)
    '        Dim apiBody = Await apiResponse.Content.ReadAsStringAsync()

    '        Dim pJson = JsonConvert.DeserializeObject(Of SalesTablesRequestJsonResult)(apiBody)

    '        ' 2️⃣ Upload JSON file to S3 (presigned URL)
    '        Dim uploadClient As New HttpClient()

    '        Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
    '        Dim pDR = New Business(pProData).LibraryOrganization.ByID(modClient.DemoCorpOrgID)
    '        pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
    '        Dim pSql = "SELECT TOP 100 * FROM CacheSalesTableInvoice"
    '        Dim pDT = pProData.Execute(pSql)
    '        Dim pJsonPayload = CacheSalesTables.SerializeTable(pDT)
    '        Dim payload As Byte() = Encoding.UTF8.GetBytes(pJsonPayload)
    '        Dim uploadContent As New ByteArrayContent(payload)

    '        ' Explicitly set only this header (no others!)
    '        uploadContent.Headers.ContentType = New MediaTypeHeaderValue("application/json")

    '        Dim uploadResponse = Await uploadClient.PutAsync(pJson.invoice_presigned_url, uploadContent)
    '        Dim resultText = Await uploadResponse.Content.ReadAsStringAsync()

    '        uploadContent = New ByteArrayContent(payload)
    '        uploadContent.Headers.ContentType = New MediaTypeHeaderValue("application/json")
    '        uploadResponse = Await uploadClient.PutAsync(pJson.order_presigned_url, uploadContent)
    '        resultText = Await uploadResponse.Content.ReadAsStringAsync()

    '        uploadContent = New ByteArrayContent(payload)
    '        uploadContent.Headers.ContentType = New MediaTypeHeaderValue("application/json")
    '        uploadResponse = Await uploadClient.PutAsync(pJson.payment_presigned_url, uploadContent)
    '        resultText = Await uploadResponse.Content.ReadAsStringAsync()

    '        uploadContent = New ByteArrayContent(payload)
    '        uploadContent.Headers.ContentType = New MediaTypeHeaderValue("application/json")
    '        uploadResponse = Await uploadClient.PutAsync(pJson.financial_presigned_url, uploadContent)
    '        resultText = Await uploadResponse.Content.ReadAsStringAsync()

    '        Console.WriteLine(uploadResponse.StatusCode)
    '        Console.WriteLine(resultText)
    '    Catch ex As Exception
    '        AppendToLog(ex.Message)
    '    End Try
    'End Sub

    Public Class SalesTablesRequestJsonResult
        Public Const SUCCESS_MESSAGE As String = "Successfully generated presigned URL"
        Public message As String
        Public financial_presigned_url As String
        Public order_presigned_url As String
        Public invoice_presigned_url As String
        Public payment_presigned_url As String
    End Class
End Class

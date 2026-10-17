Imports System.IO
Imports C1.C1Zip
Imports System.Reflection

Public Class AndroidJsTestController
    Inherits System.Web.Mvc.Controller
#If DEBUG Then

    Function Index() As ActionResult

        If TestParseDate() = False Then
            ' something is wrong
            Stop2()
        ElseIf TestCDec2() = False Then
            ' something is wrong
            Stop2()
        ElseIf TestCBool2() = False Then
            Stop2()
            'ElseIf TestInvalidSqlConnection() = False Then
            '    Stop2()
        ElseIf TestIncrementFilename() = False Then
            ' something is wrong
            Stop2()
        ElseIf TestExtractDateTimeStamp() = False Then
            Stop2()
        ElseIf TestReseverseSlash() = False Then
            Stop2()
        End If
        TestSyncLock()

        Return View()

    End Function

    Function Plain() As ActionResult

        'If TestParseDate() = False Then
        '    ' something is wrong
        '    Stop2()
        'ElseIf TestCDec2() = False Then
        '    ' something is wrong
        '    Stop2()
        'ElseIf TestCBool2() = False Then
        '    Stop2()
        '    'ElseIf TestInvalidSqlConnection() = False Then
        '    '    Stop2()
        'ElseIf TestIncrementFilename() = False Then
        '    ' something is wrong
        '    Stop2()
        'ElseIf TestExtractDateTimeStamp() = False Then
        '    Stop2()
        'End If
        'TestSyncLock()

        Return View()

    End Function

    Private Function TestInvalidSqlConnection() As Boolean

        Dim pProData As ProData

        pProData = ProData.CreateFromConnectionInfo("non-existing server", "db", "user", "pw", False, True)
        If CStr2(pProData.ConnectError) = "" Then
            Return False
        End If
        Try
            pProData.ExecuteScalar("SELECT GETDATE()")
            Return False
        Catch ex As Exception

        End Try

        Return True

    End Function

    Public Function IFrame() As ActionResult

        Return View()

    End Function

    Public Function IFrame2() As ActionResult

        Return View()

    End Function

    Public Function IFrame3() As ActionResult

        Return View()

    End Function

    Public Function TestControl() As ActionResult

        Return View()

    End Function

    Public Function LongProcess() As ActionResult

        Return View()

    End Function

    Public Function Send() As ActionResult

        Return View()

    End Function

    Public Function LongProcessCall() As String

        Threading.Thread.Sleep(3 * 60 * 1000) ' this job takes a very long time.

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    Dim mFinishedProduct As String
    Shared mProgress As Integer

    Public Function LongProcessCall2() As String

        Static sSL As New Threading.SpinLock
        Dim pStarted As Boolean

        If sSL.IsHeld Then Return False ' app is already running

        pStarted = mProgress = 0
        TimedThread(
            Sub()
                Try
                    If sSL.IsHeld Then Exit Sub
                    sSL.Enter(False)

                    mProgress = 0
                    mFinishedProduct = Nothing
                    ' this job takes a very long time.
                    Do Until mProgress = 3 * 60
                        Threading.Thread.Sleep(1000)
                        mProgress += 1
                    Loop
                    mProgress = 0
                    mFinishedProduct = DateTimeStamp()
                Catch ex As Exception
                    ' do something here
                Finally
                    If sSL.IsHeldByCurrentThread Then
                        ' do something here
                        sSL.Exit()
                    End If
                End Try

            End Sub)

        If pStarted Then
            Return JsonDictionary.Serialize("Result", "This call started the job")
        Else
            Return JsonDictionary.Serialize("Result", "The job was started by a previous call. Progress: " & mProgress)
        End If

    End Function

    Public Function LongProcessCall2FollowUp() As String

        If mFinishedProduct Is Nothing Then
            Return JsonDictionary.Serialize("Result", mProgress)
        Else
            Return JsonDictionary.Serialize("Result", "ok")
        End If

    End Function

    Function RequestDataFromServer() As String
        Dim mKey As String

        If WebRequest.Instance.HandleOrganization(True) Then 'change db
            mKey = GenerateString(75)

            ' RequestAllDataFrom(mKey)

            Return JsonDictionary.Serialize("Results", mKey)
        End If
    End Function

    Public Function TestCsv() As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pZip As ProZip
        Dim pZipFilename As String

        pZipFilename = Common.ServerPath & "temp\DataExport\Upload\ZipCsvTest.zip"
        If IO.File.Exists(pZipFilename) Then
            IO.File.Delete(pZipFilename)
        End If
        CreateFolder(pZipFilename)
        pZip = New ProZip(pZipFilename)
        pZip.CsvEncoderMaxLength = 10000 ' example lang ni

        pSql = "SELECT TOP 10 *" & vbCrLf &
            " FROM SystemDefaults"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pZip.AddTableCsv(pDT, "SystemDefaults")

        pSql = "SELECT TOP 10 *" & vbCrLf &
            " FROM LibraryPIMProduct"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pZip.AddTableCsv(pDT, "LibraryPIMProduct")

        pSql = "SELECT TOP 10 *" & vbCrLf &
            " FROM LibraryPeople"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pZip.AddTableCsv(pDT, "LibraryPeople")

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    Public Function TestCsvWriterServer() As String

        Dim pFilename As String
        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pValues As String
        Dim pRowIndex As Integer
        Dim pOut As JsonDictionary

        pFilename = AndroidJsTestController.FirstFilename(Common.WebRequest.Data("TimeStamp"))
        pCsvReader = New CsvReader(pFilename, "SystemDefaults")
        pValues = ""
        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pValues += "ID: " & pLine("ID")
            pValues += "; DefaultValue: " & pLine("DefaultValue")
            pValues += "; Description: " & pLine("Description") & vbCrLf
            pRowIndex += 1
        Loop

        pOut = New JsonDictionary
        pOut.Add("Result", "ok")
        pOut.Add("Count", pRowIndex)
        If pValues.Length > 1000 Then
            pValues = Truncate(pValues, 1000) & " ... " & TruncateLeft(pValues, 1000)
        End If
        pOut.Add("Sample", pValues)

        Return pOut.Serialize

    End Function

    Public Function TestFranchiseeDataExporter() As String

        'FranchiseeDataExporter.Test()

    End Function
#End If

    Public Function GetApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\mobiz.apk")

    End Function

    Public Function GetApkInfoBeta() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\mobizBeta.apk")

    End Function

    Public Function GetApkInfoAlpha() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\mobizAlpha.apk")

    End Function

    Public Function GetFcApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\mobizfc.apk")

    End Function

    Public Function GetSkApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\mobizsk.apk")

    End Function

    Public Function GetCustomerScreenApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\MobizCustomerScreen.apk")

    End Function

    Public Function GetTraxApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\MobizTrax.apk")

    End Function

    Public Function GetDTRApkInfo() As String

        Return ApkInfo(Common.ServerPath & "temp\Downloads\Downloads\MobizDTR.apk")

    End Function

    Private Function ApkInfo(vFilename As String) As String

        Dim pFile As IO.FileInfo
        Dim pOut As JsonDictionary
        Dim pZip As C1ZipFile
        Dim pZipEntry As C1ZipEntry
        Dim pDate As Date

        pFile = New IO.FileInfo(vFilename)
        If pFile.Exists Then
            If C1ZipFile.IsZipFile(vFilename) Then
                pZip = New C1ZipFile(vFilename)
                pZipEntry = pZip.Entries("classes.dex")
                If pZipEntry Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "The APK on the server does not contain ""classes.dex""")
                End If
                pDate = pZipEntry.Date
            Else
                Return JsonDictionary.Serialize("Error", "The APK on the server is not a valid zip file.")
            End If
            pOut = New JsonDictionary
            pOut.Add("FileDate", pDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture)) ' "u" cannot be parsed by Android
            pOut.Add("FileSize", pFile.Length)
            Dim pBuildHistory As List(Of Date) = UpdateApkInfoText(vFilename, pDate)
            Dim pApkInDeviceBuildDateText As String = Common.WebRequest.Data("BuildDate")
            Dim pApkInDeviceBuildDate As Date? = ParseDate(pApkInDeviceBuildDateText)
            If pApkInDeviceBuildDate IsNot Nothing Then
                pOut.Add("BuildCountDiff", GetApkBuildCountDiff(pApkInDeviceBuildDate, pBuildHistory))
            End If

            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Error", "File not found")
        End If

    End Function

    Shared ReadOnly UpdateApkInfoTextLock As New Object

    Private Function UpdateApkInfoText(vApkFilename As String, vApkInServerBuildDate As Date) As List(Of Date)

        SyncLock UpdateApkInfoTextLock
            Dim pOut As New List(Of Date)
            Dim pHasBuildDate As Boolean
            Dim pFilename As String = GetApkInfoTextFilename(vApkFilename)
            Dim pFileText As String = FileText(pFilename)
            Dim pLines As String() = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each pLine As String In pLines
                Dim pLineDate As Date? = ParseDate(pLine)
                If pLineDate IsNot Nothing Then
                    pOut.Add(pLineDate)
                    If pLineDate = vApkInServerBuildDate Then
                        pHasBuildDate = True
                    End If
                End If
            Next pLine
            If pHasBuildDate = False Then
                pFileText = vApkInServerBuildDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture) & vbCrLf & pFileText.Trim()
                FileText(pFilename) = pFileText
                pOut.Add(vApkInServerBuildDate)
            End If
            Return pOut
        End SyncLock

    End Function

    Private Function GetApkBuildCountDiff(vApkInDeviceBuildDate As Date, vBuildHistory As List(Of Date)) As Integer

        Dim pOut As Integer
        Dim pBuildHistory = vBuildHistory.OrderByDescending(Function(x) x).ToList()
        For Each pDate As Date In pBuildHistory
            If vApkInDeviceBuildDate < pDate Then
                pOut += 1
            Else
                Exit For
            End If
        Next pDate

        Return pOut

    End Function

    Private Function GetApkInfoTextFilename(vApkFilename As String) As String

        Dim pFilename As String = Path.GetFileNameWithoutExtension(vApkFilename)
        pFilename &= ".txt"
        pFilename = Path.Combine(Path.GetDirectoryName(vApkFilename), pFilename)
        Return pFilename

    End Function

    Public Function MobizCompleteRetail() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\mobiz.apk")

    End Function

    Public Function MobizCompleteRetailBeta() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\mobizBeta.apk")

    End Function

    Public Function MobizCompleteRetailAlpha() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\mobizAlpha.apk")

    End Function

    Public Function MobizTrax() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\MobizTrax.apk")

    End Function

    Public Function MobizFC() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\mobizfc.apk")

    End Function

    Public Function MobizSK() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\mobizsk.apk")

    End Function

    Public Function MobizCustomerScreen() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\MobizCustomerScreen.apk")

    End Function

    Public Function MobizDTR() As ActionResult

        Return DownloadApkCommon(Common.ServerPath & "\temp\Downloads\Downloads\MobizDTR.apk")

    End Function

    Private Function DownloadApkCommon(vFilename As String) As ActionResult

        Dim pFile As IO.FileInfo
        Dim pCrc As String

        pFile = New IO.FileInfo(vFilename)
        Try
            If pFile.Exists Then
                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(vFilename)
                Response.AddHeader("CRC", pCrc)
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(vFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(IO.Path.GetFileName(vFilename), True))
                Response.TransmitFile(vFilename)
            Else
                Throw New IO.FileNotFoundException
            End If
        Catch ex As Exception
            Stop2()
            Throw
        End Try

    End Function

    Public Function DownloadApk() As ActionResult

        Return MobizCompleteRetail()

    End Function

    Public Function Dump() As String

        Dim pFilename As String
        Dim pTimestamp As String

        If Request.Files.Count > 0 Then
            pTimestamp = Common.WebRequest.Data("TimeStamp")
            If CStr2(pTimestamp).Trim = "" Then
                pTimestamp = DateTimeStamp()
            End If
            pFilename = Common.ServerPath & "temp\Uploads\" & pTimestamp & "\" & Request.Files(0).FileName
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)
            Return JsonDictionary.Serialize("TimeStamp", pTimestamp)
        End If

    End Function

    Public Shared Function FirstFilename(vDirectoryName As String) As String

        Dim pDirectory As IO.DirectoryInfo
        Dim pFiles As IO.FileInfo()

        pDirectory = New IO.DirectoryInfo(Common.ServerPath & "temp\Uploads\" & vDirectoryName)
        If pDirectory.Exists Then
            pFiles = pDirectory.GetFiles()
            If pFiles.Length > 0 Then
                Return pFiles(0).FullName
            End If
        End If

        Return ""

    End Function

    Public Function DownloadTestDatabase() As ActionResult

        Dim pFilename As String
        Dim pFile As IO.FileInfo
        Dim pZipFilename As String
        Dim pZip As ProZip
        Dim pDB As ProSqlite
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            pDT = New DataTable
            pDT.Columns.Add("DateTime1", GetType(Date))
            pDT.Columns.Add("DateTime2", GetType(Date))

            ' add date via datarow
            pDR = pDT.NewRow
            pDR("DateTime1") = New Date(2014, 1, 2, 15, 30, 52)
            pDT.Rows.Add(pDR)

            pFilename = Common.ServerPath & "temp\test dummy db from server - deletable file.db"
            If IO.File.Exists(pFilename) Then
                IO.File.Delete(pFilename)
            Else
                CreateFolder(pFilename)
            End If
            pDB = New ProSqlite
            pDB.Open(pFilename)

            pDB.CreateTable("Table1", pDT)

            ' add date via sql update
            pDB.BeginTransaction()
            For pInt As Integer = 1 To 100
                pIU = New InsertUpdate("Table1")
                pIU("DateTime2") = ProSqlite.ReadyDate(New Date(2014, 1, 2, 15, 30, 52))
                pSql = pIU.GetInsert
                pDB.ExecuteSQL(pSql)
            Next pInt
            pDB.CommitTransaction()
            pDB.Close()

            pDB = New ProSqlite
            pDB.Open(pFilename)
            Try
                pSql = "SELECT DateTime1 FROM Table1" &
                    " LIMIT 10"
                pDT = pDB.Execute(pSql)
                If pDT.Rows.Count = 0 Then
                    Stop2()
                End If
                If pDT.Rows(0)(0) <> New Date(2014, 1, 2, 15, 30, 52) Then
                    Stop2()
                End If
            Finally
                pDB.Close()
            End Try

            pSql = New IO.FileInfo(pFilename).Length

            pZipFilename = Common.ServerPath & "temp\test dummy db from server - deletable file.zip"
            pZip = New ProZip(pZipFilename)
            pZip.AddFile(pFilename)

            pFile = New IO.FileInfo(pZipFilename)
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(pZipFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(pZipFilename, True))
                Response.TransmitFile(pZipFilename)
            Else
            End If
        Catch ex As Exception
            AppendToDebug("DownloadTestDatabase", , ex, True)
            Return Content(JsonDictionary.Serialize("Error", ex.Message))
        End Try

    End Function

    Private Function TestParseDate() As Boolean

        Dim pDate As Date

        pDate = ParseDate("03/29/2014 01:30:50 PM")
        If Not Date.Equals(ParseDate(New Date(2014, 3, 29, 13, 30, 50)), pDate) Then Return False
        If Not Date.Equals(ParseDate(ParseDate("03/29/2014 01:30:50 PM")), pDate) Then Return False
        If Not Date.Equals(ParseDate("2014-03-29T13:30:50.000Z"), pDate) Then Return False
        If Not Date.Equals(ParseDate("2014-03-29 13:30:50.000Z"), pDate) Then Return False
        If Not Date.Equals(ParseDate("2014-03-29T13:30:50.000"), pDate) Then Return False
        If Not Date.Equals(ParseDate("2014-03-29T13:30:50.000 "), pDate) Then Return False

        Return True

    End Function

    Private Function TestCDec2() As Boolean

        If CDec2("(1200)") <> -1200 Then Return False
        If CDec2("(1,200)") <> -1200 Then Return False
        If CDec2("(1200.00)") <> -1200 Then Return False
        If CDec2("-1200") <> -1200 Then Return False

        If CDec2("1200") <> 1200 Then Return False
        If CDec2("1,200") <> 1200 Then Return False
        If CDec2("1200.00") <> 1200 Then Return False

        If CDec2("1.206e3") <> 1206 Then Return False
        If CDec2("(1.206e3)") <> -1206 Then Return False
        If CDec2("1.206e-3") <> 0.001206 Then Return False
        If CDec2("(1.206e-3)") <> -0.001206 Then Return False

        Return True

    End Function

    Private Function TestCBool2() As Boolean

        If CBool2(True) = False Then Return False
        If CBool2(False) = True Then Return False

        If CBool2("True") = False Then Return False
        If CBool2("False") = True Then Return False

        If CBool2(" true ") = False Then Return False
        If CBool2(" fAlse   ") = True Then Return False

        If CBool2(1) = False Then Return False
        If CBool2(0) = True Then Return False

        If CBool2("1") = False Then Return False
        If CBool2("1.1") = False Then Return False
        If CBool2("-1.1") = False Then Return False
        If CBool2("(1.1)") = False Then Return False
        If CBool2("0") = True Then Return False

        Return True

    End Function

    Private Shared ReadOnly mSyncLockTestLock As New Object
    Private Shared mRunCount As Integer

    Private Function TestSyncLock() As Boolean

        TimedThread(
            Sub()
                Dim pRunCount As Integer

                mRunCount += 1
                pRunCount = mRunCount
                Debug.Print(DateTimeStamp() & " pRunCount: " & pRunCount)
                SyncLock mSyncLockTestLock
                    Debug.Print(DateTimeStamp() & " inside SyncLock pRunCount: " & pRunCount)
                    Threading.Thread.Sleep(30000)
                End SyncLock
            End Sub
        )

        Return True

    End Function

    <HttpPost()>
    Public Function GetAssemblies() As String

        Dim pDT As DataTable

        pDT = CreateTable()
        pDT = AppendAssemblyInfo(pDT)

        Return JsonDictionary.Serialize("Result", pDT)

    End Function

    Private Function CreateTable() As DataTable

        Dim pDT As DataTable

        pDT = New DataTable("Assemblies")
        With pDT.Columns
            .Add("Name")
            .Add("Version")
            .Add("FileDate", GetType(Date))
            .Add("FileSize", GetType(Integer))
            .Add("Filename")
        End With

        Return pDT

    End Function

    Private Function AppendAssemblyInfo(ByVal vDT As DataTable) As DataTable

        Dim pAssembly As Assembly
        Dim pDR As DataRow

        For Each pAssembly In AppDomain.CurrentDomain.GetAssemblies
            pDR = vDT.NewRow
            Try
                pDR("Name") = pAssembly.GetName.Name
                pDR("Version") = pAssembly.GetName.Version
                Try
                    If IO.File.Exists(pAssembly.Location) Then
                        pDR("FileDate") = IO.File.GetLastWriteTime(pAssembly.Location)
                        pDR("FileSize") = New IO.FileInfo(pAssembly.Location).Length
                    End If
                    pDR("Filename") = pAssembly.Location
                Catch ex As NotSupportedException ' The invoked member is not supported in a dynamic assembly

                End Try
            Catch ex As Exception
                Stop2()
            End Try
            vDT.Rows.Add(pDR)
        Next pAssembly

        Return vDT

    End Function

    Private Shared Function TestIncrementFilename() As Boolean

        Dim pFilename As String

        pFilename = "C:\abcde.txt"
        If IncrementFilename(pFilename, 999, 0) <> pFilename Then Stop2() : Return False
        If IncrementFilename(pFilename, 1, 0) <> "C:\a.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\abcde1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\abcde22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\abcd1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\abc1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\abc22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\ab22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\10000.txt" Then Stop2() : Return False

        Try
            IncrementFilename(pFilename, 4, 10000)
            Stop2()
            Return False
        Catch ex As ArgumentException
            ' this is the correct path
        Catch ex As Exception
            Stop2()
        End Try

        Try
            IncrementFilename(pFilename, 3, 10000)
            Stop2()
            Return False
        Catch ex As ArgumentException
            ' this is the correct path
        Catch ex As Exception
            Stop2()
        End Try

        pFilename = "C:\xyz\abcde.txt"
        If IncrementFilename(pFilename, 1, 0) <> "C:\xyz\a.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\xyz\abcde1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\xyz\abcde22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\xyz\abcd1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\xyz\abc1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\xyz\abc22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\xyz\ab22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\xyz\10000.txt" Then Stop2() : Return False

        pFilename = "abcde.txt"
        If IncrementFilename(pFilename, 1, 0) <> "a.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "abcde1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "abcde22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "abcd1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "abc1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "abc22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "ab22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "10000.txt" Then Stop2() : Return False

        pFilename = "abcde"
        If IncrementFilename(pFilename, 1, 0) <> "a" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "abcde1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "abcde22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "abcd1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "abc1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "abc22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "ab22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "10000" Then Stop2() : Return False

        pFilename = "C:\xyz\abcde"
        If IncrementFilename(pFilename, 1, 0) <> "C:\xyz\a" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\xyz\abcde1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\xyz\abcde22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\xyz\abcd1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\xyz\abc1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\xyz\abc22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\xyz\ab22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\xyz\10000" Then Stop2() : Return False

        pFilename = "C:\abcde"
        If IncrementFilename(pFilename, 1, 0) <> "C:\a" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\abcde1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\abcde22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\abcd1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\abc1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\abc22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\ab22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\10000" Then Stop2() : Return False

        pFilename = "C:\xyz.uvw\abcde"
        If IncrementFilename(pFilename, 1, 0) <> "C:\xyz.uvw\a" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\xyz.uvw\abcde1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\xyz.uvw\abcde22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\xyz.uvw\abcd1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\xyz.uvw\abc1" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\xyz.uvw\abc22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\xyz.uvw\ab22" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\xyz.uvw\10000" Then Stop2() : Return False

        pFilename = "C:\xyz.uvw\abcde.txt"
        If IncrementFilename(pFilename, 1, 0) <> "C:\xyz.uvw\a.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 1) <> "C:\xyz.uvw\abcde1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 999, 22) <> "C:\xyz.uvw\abcde22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 1) <> "C:\xyz.uvw\abcd1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 1) <> "C:\xyz.uvw\abc1.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 22) <> "C:\xyz.uvw\abc22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 4, 22) <> "C:\xyz.uvw\ab22.txt" Then Stop2() : Return False
        If IncrementFilename(pFilename, 5, 10000) <> "C:\xyz.uvw\10000.txt" Then Stop2() : Return False

        Return True

    End Function

    Private Function TestExtractDateTimeStamp() As Boolean

        If IsDate(ExtractDateTimeStamp("MobizABC_2014_08_22__13_58_09_9199")) = False Then Return False
        If IsDate(ExtractDateTimeStamp("MobizABC_2014_08_22__13_58_09_9199 MobizABC_2014_08_22__13_58_09_9199")) = False Then Return False
        If IsDate(ExtractDateTimeStamp("MobizABC_2014_08_32__13_58_09_9199")) = True Then Return False
        If IsDate(ExtractDateTimeStamp("")) = True Then Return False

        Return True

    End Function

    Private Function TestReseverseSlash() As Boolean

        Dim p As List(Of String)

        p = ProZip.ReverseSlash(Nothing)
        If p.Count <> 1 Then Return False
        If p.Contains(Nothing) = False Then Return False

        p = ProZip.ReverseSlash("abc123456")
        If p.Count <> 1 Then Return False
        If p.Contains("abc123456") = False Then Return False

        p = ProZip.ReverseSlash("")
        If p.Count <> 1 Then Return False
        If p.Contains("") = False Then Return False

        p = ProZip.ReverseSlash("   ")
        If p.Count <> 1 Then Return False
        If p.Contains("   ") = False Then Return False

        p = ProZip.ReverseSlash("abc/123/456")
        If p.Count <> 2 Then Return False
        If p.Contains("abc/123/456") = False Then Return False
        If p.Contains("abc\123\456") = False Then Return False

        p = ProZip.ReverseSlash("abc\123\456")
        If p.Count <> 2 Then Return False
        If p.Contains("abc/123/456") = False Then Return False
        If p.Contains("abc\123\456") = False Then Return False

        p = ProZip.ReverseSlash("abc/123\456")
        If p.Count <> 3 Then Return False
        If p.Contains("abc/123\456") = False Then Return False
        If p.Contains("abc/123/456") = False Then Return False
        If p.Contains("abc\123\456") = False Then Return False

        Return True

    End Function

    <HttpGet>
    Public Function AddToInventoryLedger() As String

        Dim pSql As String
        Dim pDT As DataTable
        Const pInvoiceID As String = "DCBA0D22-B930-4544-8635-81B8D9DB7FF5"
        Const pOrgID As String = "9da344f3-dadb-4675-b723-15f0c591da3c"

        WebRequest.Instance.OrgID = pOrgID
        WebRequest.Instance.HandleOrganization(True)
        Common.WebRequest.ProData.ExecuteSQL("USE MobizNike")
        pSql = " SELECT * FROM TransInventoryLedger" & vbCrLf &
            " WHERE TransactionID = " & ReadyString(pInvoiceID)
        pDT = Common.WebRequest.ProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pSql = "already has ledger entry"
            AppendToLog(pSql, , "AndroidJsTestController_AddToInventoryLedger")
            Return pSql
        End If

        WebRequest.Instance.Business.TransInvoice.AddToInventoryLedger(pInvoiceID, pOrgID)
        Return "done"

    End Function

#If DEBUG Then
    <HttpGet()>
    Public Function TestBulkCopier() As String

        BulkCopier.Test()

        Return "done"

    End Function

    <HttpGet()>
    Public Function TestPremiseDatabasesMaker() As String

        PremiseDatabasesMaker.Test()

        Return "done"

    End Function
#End If

    Function MyJournal() As ActionResult
        Return View()
    End Function

    Function getMyJournal() As String
        Dim mKey As String

        If WebRequest.Instance.HandleOrganization(True) Then 'change db

            Return JsonDictionary.Serialize("Results", mKey)
        End If
    End Function

End Class

